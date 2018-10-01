using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SudokuSolverApp
{
    /// <summary>
    /// Logique d'interaction pour UserControl3.xaml
    /// </summary>
    public partial class SudokuGrid : UserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SudokuGrid()
        {
            InitializeComponent();
            foreach (var child in grid.Children)
            {
                SudokuSubgrid subgrid = child as SudokuSubgrid;
                int nRow = Grid.GetRow(subgrid);
                int nCol = Grid.GetColumn(subgrid);
                subgrids[nRow, nCol] = subgrid;
            }
            int[,] defaultGrid = new int[9, 9]
            {
                {7,0,3,0,0,0,0,1,0 },
                {6,0,0,0,3,0,0,0,0 },
                {0,0,0,0,0,2,9,0,5 },
                {0,0,0,0,0,0,1,9,0 },
                {9,3,0,0,8,0,0,2,4 },
                {0,6,7,0,0,0,0,0,0 },
                {5,0,4,6,0,0,0,0,0 },
                {0,0,0,0,9,0,0,0,2 },
                {0,7,0,0,0,0,3,0,1 }
            };


            for (int i = 0; i < 9; ++i)
                for (int j = 0; j < 9; ++j)
                    this[i, j] = defaultGrid[i, j];

            
        }

        /// <summary>
        /// Solve the grid
        /// </summary>
        public void Solve()
        {
            Lock();
            int[,] sodukuGrid = new int[9, 9];
            for (int i = 0; i < 9; ++i)
                for (int j = 0; j < 9; ++j)
                    sodukuGrid[i,j] = this[i, j];
            SudokuSolver solver = new SudokuSolver(sodukuGrid);
            solutions = solver.Solve();

            //Grid is invalid
            if (solutions.Count != 1)
            {
                for (int i = 0; i < 9; ++i)
                    for (int j = 0; j < 9; ++j)
                        if (this[i, j] == 0)
                            this[i, j] = -1;


                return;
            }

            for (int i = 0; i < 9; ++i)
                for (int j = 0; j < 9; ++j)
                    this[i, j] = solutions[0][i, j];

        }


        /// <summary>
        /// Lock
        /// </summary>
        public void Lock()
        {
            for (int i = 0; i < 3; ++i)
                for (int j = 0; j < 3; ++j)
                    subgrids[i, j].Lock();

        }

        /// <summary>
        /// Unlock
        /// </summary>
        public void Unlock()
        {
            for (int i = 0; i < 3; ++i)
                for (int j = 0; j < 3; ++j)
                    subgrids[i, j].Unlock();

        }

        /// <summary>
        /// Clear the grid
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < 9; ++i)
                for (int j = 0; j < 9; ++j)
                    this[i, j] = 0;

        }

        /// <summary>
        /// Change the value of box of the grid
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public int this[int row, int col]
        {
            get {
                return subgrids[row / 3, col / 3][row % 3, col % 3]; }
            set { subgrids[row / 3, col / 3][row % 3, col % 3] = value; }
        }



        private SudokuSubgrid[,] subgrids = new SudokuSubgrid[3, 3];

        private List<int[,]> solutions;
    }
}
;