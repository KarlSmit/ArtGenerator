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

namespace ArtGenerator.Views
{
    /// <summary>
    /// Interaction logic for AboutUsWindow.xaml
    /// </summary>
    public partial class AboutUsWindow : Page
    {
        public AboutUsWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// When config window size changes run the following code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainGrid_SizeChanged(object sender, EventArgs e)
        {
            CalculateScale();
        }

        /// <summary>
        /// Calculate the scale when a window is changing
        /// </summary>
        private void CalculateScale()
        {
            double yScale = ActualHeight / 506f;
            double xScale = ActualWidth / 900f;
            double value = Math.Min(xScale, yScale);
            GridSize.ScaleY = value;
            GridSize.ScaleX = value;
        }
    }
}
