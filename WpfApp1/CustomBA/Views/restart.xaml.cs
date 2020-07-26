using CustomBA.ViewModels;
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

namespace CustomBA.Views
{
    /// <summary>
    /// Lógica de interacción para restart.xaml
    /// </summary>
    public partial class restart : Window
    {
        public restart()
        {
            InitializeComponent();
        }

        private void jeje(object sender, RoutedEventArgs e)
        {
            jeji.Content = "Clic";
            System.Diagnostics.Process.Start("shutdown.exe", "-r -t 10");
            Dispatcher.InvokeShutdown();
        }
    }
}
