using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfApp1
{
    /// <summary>
    /// Abstract class of position rules for sodoku
    /// </summary>
    internal abstract class SudokuPositionRule
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ruleId">rule ID (col, row or subgrid Id)</param>
        /// <param name="figure">figure of the rule (1 to 9)</param> to 
        public SudokuPositionRule(BoxRule[,] rule, int ruleId, int figure)
        {
            RuleID = ruleId;
            Figure = figure;
            sudokuRule = rule;
        }
        /// <summary>
        /// Convert the box id to rule ID and id
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="ruleId"></param>
        /// <param name="id"></param>
        public abstract void ConvertBoxID(int row, int col, out int ruleId, out int id);

        /// <summary>
        /// Retrieve row and col from box id and id
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="ruleId"></param>
        /// <param name="id"></param>
        public abstract void RetrieveBoxID(out int row, out int col, int ruleId, int id);

        /// <summary>
        /// Delete row and col position
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void DeleteRowAndCol(int row, int col)
        {
            ConvertBoxID(row, col, out int ruleId, out int id);
            locker.AcquireWriterLock(-1);
            try
            {
                List<int> idsToBeRemoved = new List<int>();
                foreach(int locId in allowedIDs)
                {
                    RetrieveBoxID(out int row2, out int col2, RuleID, locId);
                    if (row2 == row && col2 == col)
                        continue;
                    if (row2 == row || col2 == col)
                        idsToBeRemoved.Add(locId);

                }

                foreach(int idToBeDel in idsToBeRemoved)
                    allowedIDs.Remove(idToBeDel);
                if (allowedIDs.Count == 1)
                    boxID = allowedIDs[0];
            }
            finally
            {
                locker.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Delete a candidate position
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void Delete(int row, int col)
        {
            ConvertBoxID(row, col, out int ruleId, out int id);
            if (RuleID != ruleId)
                return;
            locker.AcquireWriterLock(-1);
            try
            {
                allowedIDs.Remove(id);
                if (allowedIDs.Count == 1)
                    boxID = allowedIDs[0];
                if (allowedIDs.Count == 0)
                    boxID = -1;
            }
            finally
            {
                locker.ReleaseWriterLock();
            }

        }

        /// <summary>
        /// Set the candidate position
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns>false : invalid position; true valid position</returns>
        public void SetPosition(int row, int col)
        {
            ConvertBoxID(row, col, out int ruleId, out int id);
            if (RuleID != ruleId)
                return;

            locker.AcquireWriterLock(-1);
            try
            {
                for (int boxId = 0; boxId < 9; ++boxId)
                {
                    if (boxId == id)
                        continue;
                    RetrieveBoxID(out row, out col, ruleId, boxId);
                    sudokuRule[row, col].Delete(Figure);

                }


                if (allowedIDs.Contains(id))
                {
                    allowedIDs.Clear();
                    allowedIDs.Add(id);
                    boxID = id;
                }
                else
                {
                    allowedIDs.Clear();
                    boxID = 0;
                }
            }
            finally
            {
                locker.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// List of allowed ids
        /// </summary>
        public List<int> AllowedIDs
        {
            get
            {
                locker.AcquireReaderLock(-1);
                try
                {
                    return allowedIDs;
                }
                finally
                {
                    locker.ReleaseReaderLock();
                }

            }
        }

        /// <summary>
        /// List of allowed ids
        /// </summary>
        public int BoxID
        {
            get
            {
                locker.AcquireReaderLock(-1);
                try
                {
                    return boxID;
                }
                finally
                {
                    locker.ReleaseReaderLock();
                }

            }
        }
        /// <summary>
        /// Is valid?
        /// </summary>
        public bool IsValid
        {
            get { return AllowedIDs.Count > 0; }
        }


        /// <summary>
        /// Rule ID
        /// </summary>
        public readonly int RuleID;
        /// <summary>
        /// Figure
        /// </summary>
        public readonly int Figure;

        private List<int> allowedIDs = new List<int>(9) { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        private ReaderWriterLock locker = new ReaderWriterLock();
        private BoxRule[,] sudokuRule;
        private int boxID = -1;

    }

    /// <summary>
    /// Row rule
    /// </summary>
    internal class SudokuRowRule : SudokuPositionRule
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rule">rule array</param>
        /// <param name="ruleId">rule ID</param>
        /// <param name="figure">figure</param>
        public SudokuRowRule(BoxRule[,] rule, int ruleId, int figure): base(rule, ruleId, figure) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="ruleId">row</param>
        /// <param name="id">col</param>
        public override void ConvertBoxID(int row, int col, out int ruleId, out int id)
        {
            ruleId = row;
            id = col;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="ruleId">row</param>
        /// <param name="id">col</param>
        public override void RetrieveBoxID(out int row, out int col, int ruleId, int id)
        {
            row = ruleId;
            col = id;
        }

    }

    /// <summary>
    /// Column rule
    /// </summary>
    internal class SudokuColumnRule : SudokuPositionRule
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="ruleId">col</param>
        /// <param name="figure"></param>
        public SudokuColumnRule(BoxRule[,] rule, int ruleId, int figure) : base(rule, ruleId, figure) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="ruleId">col</param>
        /// <param name="id">row</param>
        public override void ConvertBoxID(int row, int col, out int ruleId, out int id)
        {
            ruleId = col;
            id = row;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="ruleId">col</param>
        /// <param name="id">row</param>
        public override void RetrieveBoxID(out int row, out int col, int ruleId, int id)
        {
            col = ruleId;
            row = id;
        }
    }
    /// <summary>
    /// Subgrid rule (no same figure in a subgrid)
    /// </summary>
    internal class SudokuSubgridRule : SudokuPositionRule
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rule">rule array</param>
        /// <param name="ruleId">3 * (row / 3) + (col / 3)</param>
        /// <param name="figure"></param>
        public SudokuSubgridRule(BoxRule[,] rule, int ruleId, int figure) : base(rule, ruleId, figure) { }
        public override void ConvertBoxID(int row, int col, out int ruleId, out int id)
        {
            ruleId = 3 * (row / 3) + (col / 3);
            id = 3 * (row % 3) + (col % 3);
        }
        public override void RetrieveBoxID(out int row, out int col, int ruleId, int id)
        {
            col = 3 * (ruleId % 3) + (id % 3);
            row = 3 * (ruleId / 3) + (id / 3);
        }
    }
    /// <summary>
    /// Rule set for a box
    /// </summary>
    internal class BoxRule
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public BoxRule(int row, int col)
        {
            Row = row;
            Col = col;
        }
        /// <summary>
        /// Delete a candidate
        /// </summary>
        /// <param name="figure"></param>
        public void Delete(int figure)
        {
            locker.AcquireWriterLock(-1);
            try
            {
                allowedNumbers.Remove(figure);
                if (allowedNumbers.Count == 1)
                    value = allowedNumbers[0];
            }
            finally { locker.ReleaseWriterLock(); }
        }

        /// <summary>
        /// Set the figure of the box
        /// </summary>
        /// <param name="figure"></param>
        public void Set(int figure)
        {
            locker.AcquireWriterLock(-1);
            try
            {
                if (allowedNumbers.Contains(figure))
                {
                    allowedNumbers.Clear();
                    allowedNumbers.Add(figure);
                    value = figure;
                }
                else
                {
                    allowedNumbers.Clear();
                    value = 0;
                }
            }
            finally { locker.ReleaseWriterLock(); }
        }

        /// <summary>
        /// Found figure
        /// </summary>
        public int Figure
        {
            get {
                 locker.AcquireReaderLock(-1);
                try
                {
                    return value;
                }
                finally
                {
                    locker.ReleaseReaderLock();
                }
            }
        }
        /// <summary>
        /// Is the box valid?
        /// </summary>
        public bool IsValid
        {
            get { return AllowedNumbers.Count > 0; }
        }

        /// <summary>
        /// List of allowed ids
        /// </summary>
        public List<int> AllowedNumbers
        {
            get
            {
                locker.AcquireReaderLock(-1);
                try
                {
                    return allowedNumbers;
                }
                finally
                {
                    locker.ReleaseReaderLock();
                }

            }
        }
        /// <summary>
        /// Row
        /// </summary>
        public readonly int Row;
        /// <summary>
        /// Col
        /// </summary>
        public readonly int Col;
        private int value = 0;
        private List<int> allowedNumbers = new List<int>(9){ 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        private ReaderWriterLock locker = new ReaderWriterLock();
    }





}
