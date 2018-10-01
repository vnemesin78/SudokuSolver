using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SudokuSolverApp.SudokuRules
{
    /// <summary>
    /// Rule of a box.
    /// Just a list of numbers
    /// </summary>
    internal class SudokuBoxRule
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public SudokuBoxRule(int row, int col)
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
        /// Found figure
        /// </summary>
        public int Figure
        {
            get
            {
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
            set
            {
                locker.AcquireWriterLock(-1);
                try
                {
                    if (allowedNumbers.Contains(value))
                    {
                        allowedNumbers.Clear();
                        allowedNumbers.Add(value);
                        this.value = value;
                    }
                    else
                    {
                        allowedNumbers.Clear();
                        this.value = 0;
                    }
                }
                finally { locker.ReleaseWriterLock(); }

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
        /// 
        public readonly int Row;
        /// <summary>
        /// Col
        /// </summary>
        public readonly int Col;

        private int value = 0;
        private List<int> allowedNumbers = new List<int>(9) { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        private ReaderWriterLock locker = new ReaderWriterLock();
    }
}
