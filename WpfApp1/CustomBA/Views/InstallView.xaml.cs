using CustomBA.ViewModels;
using System.Windows;
namespace CustomBA.Views
{
    public partial class InstallView : Window
    {
        public InstallView(InstallViewModel viewModel)
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
            this.Closed += (sender, e) =>
            viewModel.CancelCommand.Execute(this);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var lv = new restart();
            lv.Show();
        }
    }
}