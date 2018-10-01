using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverApp.SudokuRules
{
    /// <summary>
    /// Column rule  (No same number in a column)
    /// </summary>
    internal class SudokuColumnRule : SudokuPositionRule
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="ruleId">col</param>
        /// <param name="figure"></param>
        public SudokuColumnRule(SudokuBoxRule[,] rule, int ruleId, int figure) : base(rule, ruleId, figure) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="ruleId">col</param>
        /// <param name="id">row</param>
        public override void RowAndColIDsToRuleAndBoxIds(int row, int col, out int ruleId, out int id)
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
        public override void RuleAndBoxIdsToRowAndColIds(out int row, out int col, int ruleId, int id)
        {
            col = ruleId;
            row = id;
        }
    }

}
