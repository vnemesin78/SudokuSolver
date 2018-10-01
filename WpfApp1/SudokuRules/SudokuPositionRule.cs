using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SudokuSolverApp.SudokuRules
{
    /// <summary>
    /// Position rules of the Sudoku
    /// </summary>
    internal abstract class SudokuPositionRule
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ruleId">rule ID (col, row or subgrid Id)</param>
        /// <param name="figure">figure of the rule (1 to 9)</param> to 
        public SudokuPositionRule(SudokuBoxRule[,] rule, int ruleId, int figure)
        {
            RuleID = ruleId;
            Figure = figure;
            sudokuRule = rule;
        }

        /// <summary>
        /// Convert the box id to rule ID and id
        /// </summary>
        /// <param name="row">row id</param>
        /// <param name="col">col id</param>
        /// <param name="ruleId">rule id</param>
        /// <param name="id">box id</param>
        public abstract void RowAndColIDsToRuleAndBoxIds(int row, int col, out int ruleId, out int id);

        /// <summary>
        /// Retrieve row and col from rule id and box id
        /// </summary>
        /// <param name="row">row id</param>
        /// <param name="col">col id</param>
        /// <param name="ruleId">rule id</param>
        /// <param name="id">box id</param>
        public abstract void RuleAndBoxIdsToRowAndColIds(out int row, out int col, int ruleId, int id);

        /// <summary>
        /// Delete row and col position, exception if it is the case # row, # col
        /// </summary>
        /// <param name="row">row id</param>
        /// <param name="col">col id</param>
        public void DeleteRowAndColumn(int row, int col)
        {
            RowAndColIDsToRuleAndBoxIds(row, col, out int ruleId, out int id);
            locker.AcquireWriterLock(-1);
            try
            {
                List<int> idsToBeRemoved = new List<int>();
                foreach (int locId in allowedIDs)
                {
                    RuleAndBoxIdsToRowAndColIds(out int locRow, out int locCol, RuleID, locId);
                    if (locRow == row && locCol == col)
                        continue;
                    if (locRow == row || locCol == col)
                        idsToBeRemoved.Add(locId);
                }

                foreach (int idToBeDel in idsToBeRemoved)
                    allowedIDs.Remove(idToBeDel);
            }
            finally
            {
                locker.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Delete a candidate position
        /// </summary>
        /// <param name="row">row id</param>
        /// <param name="col">col id</param>
        public void Delete(int row, int col)
        {
            RowAndColIDsToRuleAndBoxIds(row, col, out int ruleId, out int id);
            if (RuleID != ruleId)
                return;
            locker.AcquireWriterLock(-1);
            try
            {
                allowedIDs.Remove(id);
            }
            finally
            {
                locker.ReleaseWriterLock();
            }

        }

        /// <summary>
        /// Set the candidate position
        /// </summary>
        /// <param name="row">row id</param>
        /// <param name="col">col id</param>
        /// <returns>false : invalid position; true valid position</returns>
        public void SetPosition(int row, int col)
        {
            RowAndColIDsToRuleAndBoxIds(row, col, out int ruleId, out int id);
            if (RuleID != ruleId)
                return;

            locker.AcquireWriterLock(-1);
            try
            {
                for (int boxId = 0; boxId < 9; ++boxId)
                {
                    if (boxId == id)
                        continue;
                    RuleAndBoxIdsToRowAndColIds(out row, out col, ruleId, boxId);
                    sudokuRule[row, col].Delete(Figure);

                }
                if (allowedIDs.Contains(id))
                {
                    allowedIDs.Clear();
                    allowedIDs.Add(id);
                }
                else
                    allowedIDs.Clear();
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
                    int boxID = -1;
                    if (allowedIDs.Count == 1)
                        boxID = allowedIDs[0];
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
        private readonly SudokuBoxRule[,] sudokuRule;

    }

}
