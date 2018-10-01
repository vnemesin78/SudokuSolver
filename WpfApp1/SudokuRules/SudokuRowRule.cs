using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverApp.SudokuRules
{
    /// <summary>
    /// Row rule (No same number in a row)
    /// </summary>
    internal class SudokuRowRule : SudokuPositionRule
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rule">rule array</param>
        /// <param name="ruleId">rule ID</param>
        /// <param name="figure">figure</param>
        public SudokuRowRule(SudokuBoxRule[,] rule, int ruleId, int figure) : base(rule, ruleId, figure) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="ruleId">row</param>
        /// <param name="id">col</param>
        public override void RowAndColIDsToRuleAndBoxIds(int row, int col, out int ruleId, out int id)
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
        public override void RuleAndBoxIdsToRowAndColIds(out int row, out int col, int ruleId, int id)
        {
            row = ruleId;
            col = id;
        }

    }
}

