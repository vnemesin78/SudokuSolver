using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfApp1
{
    /// <summary>
    /// Soduku solver
    /// </summary>
    public class SodukuSolver
    {
        private struct BoxInformation
        {
            public int Row;
            public int Col;
            public int Figure;
        }

        /// <summary>
        /// 
        /// </summary>;
        public SodukuSolver(int[,] grid = null)
        {
            if (grid == null)
                grid = new int[9, 9];

            for (int i = 0; i < 9; ++i)
                for (int j = 0; j < 9; ++j)
                {
                    boxRules[i, j] = new BoxRule(i, j);
                    rowRules[i, j] = new SudokuRowRule(boxRules, i, j + 1);
                    colRules[i, j] = new SudokuColumnRule(boxRules, i, j + 1);
                    subgridRules[i, j] = new SudokuSubgridRule(boxRules, i, j + 1);
                }

            for (int i = 0; i < 9; ++i)
                for (int j = 0; j < 9; ++j)
                    this[i, j] = grid[i, j];


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public int this[int row, int col]
        {
            set
            {
                if (value == 0 || this[row, col] == value)
                    return;
                rowRules[row, value - 1].SetPosition(row, col);
                for (int i = 0; i < 9; ++i)
                    rowRules[i, value - 1].DeleteRowAndCol(row, col);
                colRules[col, value - 1].SetPosition(row, col);
                for (int i = 0; i < 9; ++i)
                    colRules[i, value - 1].DeleteRowAndCol(row, col);
                subgridRules[0, 0].ConvertBoxID(row, col, out int ruleId, out int id);
                subgridRules[ruleId, value - 1].SetPosition(row, col);
                for (int i = 0; i < 9; ++i)
                    subgridRules[i, value - 1].DeleteRowAndCol(row, col);

                for (int nFigure = 0; nFigure <9; ++nFigure)
                {
                    if (nFigure == value - 1)
                        continue;
                    for(int i = 0; i < 9; ++i)
                    {
                        rowRules[i, nFigure].Delete(row, col);
                        colRules[i, nFigure].Delete(row, col);
                        subgridRules[i, nFigure].Delete(row, col);
                    }

                }



                boxRules[row, col].Set(value);
                Grid[row, col] = value;
                ++numberOfFilledBoxes;
            }
            get
            {
                return Grid[row, col];
            }
        }

        private Dictionary<int, BoxInformation> DetectBoxesToBeFilled()
        {
            Dictionary<int, BoxInformation> dico = new Dictionary<int, BoxInformation>();
            object locker = new object();

            Parallel.For(0, 81, n =>
            {
                int row = n / 9;
                int col = n % 9;
                int figure = boxRules[row, col].Figure;
                if (figure != 0 && figure != this[row, col])
                {
                    lock (locker)
                    {
                        int p = row * 9 + col;
                        if (!dico.Keys.Contains(p))
                            dico.Add(p, new BoxInformation { Row = row, Col = col, Figure = figure });
                    }
                }

            });

            Parallel.For(0, 81, n =>
            {
                int row = n / 9;
                int nFigure = n % 9;
                int figure = nFigure + 1;
                int col = rowRules[row, nFigure].BoxID;
                if (col >= 0 && figure != this[row, col])
                {
                    lock (locker)
                    {
                        int p = row * 9 + col;
                        if (!dico.Keys.Contains(p))
                            dico.Add(p, new BoxInformation { Row = row, Col = col, Figure = figure });
                    }
                }

            });
            
            Parallel.For(0, 81, n =>
            {
                int col = n / 9;
                int nFigure = n % 9;
                int figure = nFigure + 1;
                int row = colRules[col, nFigure].BoxID;
                if (row >= 0 && figure != this[row, col])
                {
                    lock (locker)
                    {
                        int p = row * 9 + col;
                        if (!dico.Keys.Contains(p))
                            dico.Add(p, new BoxInformation { Row = row, Col = col, Figure = figure });
                    }
                }

            });

            Parallel.For(0, 81, n =>
            {
                int gridId = n / 9;
                int nFigure = n % 9;
                int figure = nFigure + 1;
                int id = subgridRules[gridId, nFigure].BoxID;
                subgridRules[0, 0].RetrieveBoxID(out int row, out int col, gridId, id);

                if (id >= 0 && figure != this[row, col])
                {
                    lock (locker)
                    {
                        int p = row * 9 + col;
                        if (!dico.Keys.Contains(p))
                            dico.Add(p, new BoxInformation { Row = row, Col = col, Figure = figure });
                    }
                }

            });
            
            return dico;
        }

        private List<BoxInformation> GetHypothesis()
        {
            List<BoxInformation> info = new List<BoxInformation>();
            for (int count = 2; count <= 9; ++count)
            {
                for (int row = 0; row < 9; ++row)
                {
                    for (int col = 0; col < 9; ++col)
                    {
                        if (boxRules[row, col].AllowedNumbers.Count == count)
                        {
                            for (int i = 0; i < count; ++i)
                                info.Add(new BoxInformation() { Row = row, Col = col, Figure = boxRules[row, col].AllowedNumbers[i] });
                            return info;
                        }

                    }

                }
            }
            return info;

        }

        /// <summary>
        /// Solve the grid
        /// </summary>
        /// <returns>Different solutions of the grid</returns>
        public List<int[,]> Solve()
        {
            List<int[,]> gridRules = new List<int[,]>();

            while (IsValid && !IsFilled)
            {
                IEnumerable<BoxInformation> boxesToBeFilled = DetectBoxesToBeFilled().Values;

                if (boxesToBeFilled.Count() == 0)
                {
                    IEnumerable<BoxInformation> hypothesis = GetHypothesis();
                    for (int n = 1; n < hypothesis.Count(); ++n)

                    {
                        var rule = new SodukuSolver(Grid);
                        rule[hypothesis.ElementAt(n).Row, hypothesis.ElementAt(n).Col] = hypothesis.ElementAt(n).Figure;
                        gridRules.AddRange(rule.Solve());
                    }
                    this[hypothesis.ElementAt(0).Row, hypothesis.ElementAt(0).Col] = hypothesis.ElementAt(0).Figure;
                    continue;
                }

                foreach (var info in boxesToBeFilled)
                    this[info.Row, info.Col] = info.Figure;

            }

            if (IsValid)
                gridRules.Add(this.Grid);

            return gridRules;
        }

        /// <summary>
        /// Is Valid
        /// </summary>
        public bool IsValid
        {
            get
            {
                bool isValid = true;
                foreach (var rule in boxRules)
                    isValid = isValid && rule.IsValid;
                foreach (var rule in rowRules)
                    isValid = isValid && rule.IsValid;
                foreach (var rule in colRules)
                    isValid = isValid && rule.IsValid;
                foreach (var rule in subgridRules)
                    isValid = isValid && rule.IsValid;
                return isValid;
            }
        }

        /// <summary>
        /// Is Completed?
        /// </summary>
        public bool IsFilled
        {
            get { return numberOfFilledBoxes == 81; }
        }

        private BoxRule[,] boxRules = new BoxRule[9, 9];
        private SudokuRowRule[,] rowRules = new SudokuRowRule[9, 9];
        private SudokuColumnRule[,] colRules = new SudokuColumnRule[9, 9];
        private SudokuSubgridRule[,] subgridRules = new SudokuSubgridRule[9, 9];
        private int[,] Grid = new int[9,9];
        private int numberOfFilledBoxes = 0;
    }
}
