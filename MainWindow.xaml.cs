using Menu;
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
using System.Windows.Threading;

namespace SoundConsole
{
    public partial class MainWindow : Window
    {
        public static Dispatcher MainDispatcher;

        public const int Order = 6;
        public const int Length = 1 << Order;

        MenuBar menuBar;

        public MainWindow()
        {
            InitializeComponent();
            HideBoundingBox(this);

            MainWindow.MainDispatcher = this.Dispatcher;
            this.menuBar = new MenuBar(this);
        }

        #region 标题栏按钮
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }
        #endregion

        #region 拖动标题栏移动窗口位置
        private void HeadlineRectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try
                {
                    this.DragMove();
                }
                catch
                {

                }
            }
        }
        #endregion

        #region 隐藏虚线
        public static void HideBoundingBox(object root)
        {
            Control control = root as Control;
            if (control != null)
            {
                control.FocusVisualStyle = null;
            }

            if (root is DependencyObject)
            {
                foreach (object child in LogicalTreeHelper.GetChildren((DependencyObject)root))
                {
                    HideBoundingBox(child);
                }
            }
        }
        #endregion
    }
}
