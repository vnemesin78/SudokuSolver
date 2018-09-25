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

namespace WpfApp1
{
    /// <summary>
    /// Logique d'interaction pour UserControl2.xaml
    /// </summary>
    public partial class SodukuSubgrid : UserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SodukuSubgrid()
        {
            InitializeComponent();
            foreach(var child in grid.Children)
            {
                SodukuBox box = child as SodukuBox;
                int nRow = Grid.GetRow(box);
                int nCol = Grid.GetColumn(box);
                boxes[nRow, nCol] = box;
            }
        }

        /// <summary>
        /// Set the number
        /// </summary>
        /// <param name="row">row</param>
        /// <param name="col">column</param>
        /// <returns></returns>
        public int this[int row, int col]
        {
            get { return boxes[row, col].Figure; }
            set { boxes[row, col].Figure = value; }
        }

        /// <summary>
        /// Lock the subgrid
        /// </summary>
        public void Lock()
        {
            for (int i = 0; i < 3; ++i)
                for (int j = 0; j < 3; ++j)
                    boxes[i, j].Lock();

        }

        /// <summary>
        /// Unlock the subgrid
        /// </summary>
        public void Unlock()
        {
            for (int i = 0; i < 3; ++i)
                for (int j = 0; j < 3; ++j)
                    boxes[i, j].Unlock();

        }

        private SodukuBox[,] boxes = new SodukuBox[3,3];
    }

}
