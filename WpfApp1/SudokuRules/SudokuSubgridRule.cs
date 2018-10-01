using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverApp.SudokuRules
{
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
        public SudokuSubgridRule(SudokuBoxRule[,] rule, int ruleId, int figure) : base(rule, ruleId, figure) { }
        public override void RowAndColIDsToRuleAndBoxIds(int row, int col, out int ruleId, out int id)
        {
            ruleId = 3 * (row / 3) + (col / 3);
            id = 3 * (row % 3) + (col % 3);
        }
        public override void RuleAndBoxIdsToRowAndColIds(out int row, out int col, int ruleId, int id)
        {
            col = 3 * (ruleId % 3) + (id % 3);
            row = 3 * (ruleId / 3) + (id / 3);
        }
    }
}
