using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SudokuSolverApp.SudokuRules;

namespace SudokuSolverApp
{
    /// <summary>
    /// Soduku solver
    /// </summary>
    public class SudokuSolver
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="grid">soduku grid</param>
        public SudokuSolver(int[,] grid = null)
        {
            if (grid == null)
                grid = new int[9, 9];

            for (int i = 0; i < 9; ++i)
                for (int j = 0; j < 9; ++j)
                {
                    boxRules[i, j] = new SudokuBoxRule(i, j);
                    rowRules[i, j] = new SudokuRowRule(boxRules, i, j + 1);
                    colRules[i, j] = new SudokuColumnRule(boxRules, i, j + 1);
                    subgridRules[i, j] = new SudokuSubgridRule(boxRules, i, j + 1);
                }
            for (int i = 0; i < 9; ++i)
                for (int j = 0; j < 9; ++j)
                    this[i, j] = grid[i, j];


        }

        /// <summary>
        /// Return the sodoku grid value
        /// </summary>
        /// <param name="row">row id</param>
        /// <param name="col">col id</param>
        /// <returns></returns>
        public int this[int row, int col]
        {
            set
            {
                if (value == 0 || this[row, col] == value)
                    return;
                rowRules[row, value - 1].SetPosition(row, col);
                for (int i = 0; i < 9; ++i)
                    rowRules[i, value - 1].DeleteRowAndColumn(row, col);
                colRules[col, value - 1].SetPosition(row, col);
                for (int i = 0; i < 9; ++i)
                    colRules[i, value - 1].DeleteRowAndColumn(row, col);
                subgridRules[0, 0].RowAndColIDsToRuleAndBoxIds(row, col, out int ruleId, out int id);
                subgridRules[ruleId, value - 1].SetPosition(row, col);
                for (int i = 0; i < 9; ++i)
                    subgridRules[i, value - 1].DeleteRowAndColumn(row, col);

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
                boxRules[row, col].Figure = value;
                grid[row, col] = value;
                lock(numberLock)
                    ++numberOfFilledBoxes;
            }
            get
            {
                return grid[row, col];
            }
        }

        /// <summary>
        /// Solve the grid
        /// </summary>
        /// <returns>List of the solutions</returns>
        public List<int[,]> Solve()
        {
            List<int[,]> gridRules = new List<int[,]>();

            while (IsValid && !IsFilled)
            {
                // Detect the box to be filled
                IEnumerable<BoxInformation> boxesToBeFilled = DetectBoxesToBeFilled().Values;

                // If no boxes, then make hypothesis
                if (boxesToBeFilled.Count() == 0)
                {
                    IEnumerable<BoxInformation> hypothesis = GetHypothesis();
                    for (int n = 1; n < hypothesis.Count(); ++n)

                    {
                        var rule = new SudokuSolver(grid);
                        rule[hypothesis.ElementAt(n).Row, hypothesis.ElementAt(n).Col] = hypothesis.ElementAt(n).Figure;
                        gridRules.AddRange(rule.Solve());
                    }
                    this[hypothesis.ElementAt(0).Row, hypothesis.ElementAt(0).Col] = hypothesis.ElementAt(0).Figure;
                    continue;
                }
                // Fill the boxes
                Parallel.ForEach(boxesToBeFilled, info => { 
                    this[info.Row, info.Col] = info.Figure;
            });
            }

            if (IsValid)
                gridRules.Add(grid);

            return gridRules;
        }

        /// <summary>
        /// Is Valid?
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
                subgridRules[0, 0].RuleAndBoxIdsToRowAndColIds(out int row, out int col, gridId, id);

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

        private struct BoxInformation
        {
            public int Row;
            public int Col;
            public int Figure;
        }

        private readonly SudokuBoxRule[,] boxRules = new SudokuBoxRule[9, 9];
        private readonly SudokuRowRule[,] rowRules = new SudokuRowRule[9, 9];
        private readonly SudokuColumnRule[,] colRules = new SudokuColumnRule[9, 9];
        private readonly SudokuSubgridRule[,] subgridRules = new SudokuSubgridRule[9, 9];
        private readonly int[,] grid = new int[9,9];
        private int numberOfFilledBoxes = 0;
        private readonly object numberLock = new object();
    }
}
