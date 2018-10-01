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
    /// Logique d'interaction pour SodukuBox.xaml
    /// </summary>
    public partial class SudokuBox : UserControl
    {
        /// <summary>
        /// Status
        /// </summary>
        public enum Status
        {
            EMPTY,
            INITIAL,
            SOLVED,
            ERROR,
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SudokuBox()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int locFigure;
            if (textBox.Text == "")
                locFigure = 0;
            else if (!int.TryParse(textBox.Text, out locFigure))
                locFigure = -1;
            Figure = locFigure;
            if (0 == Figure)
                ChangeBackGroudColor(textBox.IsReadOnly ? Status.ERROR : Status.EMPTY);
            else
                ChangeBackGroudColor(textBox.IsReadOnly ? Status.SOLVED : Status.INITIAL);
        }

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!textBox.IsReadOnly)
            {
                Figure = 0;
                ChangeBackGroudColor(Status.EMPTY);
            }
        }

        /// <summary>
        /// Change background color
        /// </summary>
        /// <param name="status">Status</param>
        public void ChangeBackGroudColor(Status status)
        {
            switch(status)
            {
                case (Status.INITIAL):
                    this.textBox.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
                    break;
                case (Status.EMPTY):
                    this.textBox.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    break;
                case (Status.SOLVED):
                    this.textBox.Background = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
                    break;
                case (Status.ERROR):
                    this.textBox.Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                    break;


            }
        }

        /// <summary>
        /// Lock the textbox
        /// </summary>
        public void Lock()
        {
            textBox.IsReadOnly = true;
        }

        /// <summary>
        /// Unlock the textbox
        /// </summary>
        public void Unlock()
        {
            textBox.IsReadOnly = false;
        }

        /// <summary>
        /// Figure
        /// </summary>
        public int Figure
        {
            get { return figure; }
            set
            {
                int prevFigure = figure;
                figure = value;
                if (figure <= 0 || figure > 9)
                    figure = 0;
                textBox.Text = figure == 0 ? "" : figure.ToString();
                if (0 == figure)
                    ChangeBackGroudColor(textBox.IsReadOnly ? Status.ERROR : Status.EMPTY);
            }
        }

        private int figure;


    }
}
