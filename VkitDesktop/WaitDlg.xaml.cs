using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VkitDesktop
{
    /// <summary>
    /// Interaction logic for WaitDlg.xaml
    /// </summary>
    public partial class WaitDlg : Window, IDisposable
    {
        public WaitDlg()
        {
            InitializeComponent();

            Show();

            // Make sure we draw properly
            EmptyDelegate waitForRender = DoNotingFunc;
            Dispatcher.Invoke(waitForRender, System.Windows.Threading.DispatcherPriority.Background);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Make sure we draw properly
            EmptyDelegate waitForRender = DoNotingFunc;
            Dispatcher.Invoke(waitForRender, System.Windows.Threading.DispatcherPriority.Background);
        }


        delegate void EmptyDelegate();

        private void DoNotingFunc()
        {
        }


        public void Dispose()
        {
            Close();
        }

    }
}
