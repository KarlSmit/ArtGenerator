using ArtGenerator.Views;
using ArtGenerator.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ArtGenerator.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }
        public double ScaleY { get; set; }
        public double ScaleX { get; set; }

        private void leftPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            LeftPanelZoomInHover.IsEnabled = true;
        }

        private void leftPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            LeftPanelZoomInHover.IsEnabled = false;
        }

        private void rightPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            RightPanelZoomInHover.IsEnabled = true;
        }

        private void rightPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            RightPanelZoomInHover.IsEnabled = false;
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
            double yScale = ActualHeight / 1080f;
            double xScale = ActualWidth / 1920f;
            double value = Math.Min(xScale, yScale);
            GridSize.ScaleY = value;
            GridSize.ScaleX = value;
        }

        /// <summary>
        /// TitleBar_MouseDown - Drag if single-click, resize if double-click
        /// </summary>
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                if (e.ClickCount == 2)
                {
                    AdjustWindowSize();
                }
                else
                {
                    Application.Current.MainWindow.DragMove();
                }
        }

        /// <summary>
        /// CloseButton_Clicked
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// MaximizedButton_Clicked
        /// </summary>
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            AdjustWindowSize();
        }

        /// <summary>
        /// Minimized Button_Clicked
        /// </summary>
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Adjusts the WindowSize to correct parameters when Maximize button is clicked
        /// </summary>
        private void AdjustWindowSize()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }

        }
        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

    }
}
