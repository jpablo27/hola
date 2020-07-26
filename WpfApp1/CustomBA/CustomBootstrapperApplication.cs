using CustomBA.Models;
using CustomBA.ViewModels;
using CustomBA.Views;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Windows.Threading;
namespace CustomBA
{
    public class CustomBootstrapperApplication :
    BootstrapperApplication
    {
        public static Dispatcher Dispatcher { get; set; }

        public static restart restreq;
        public static InstallView view;
        protected override void Run()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            var model = new BootstrapperApplicationModel(this);
            var viewModel = new InstallViewModel(model);
            view = new InstallView(viewModel);
            restreq = new restart();
            model.SetWindowHandle(view);
            this.Engine.Detect();
            view.Show();
            Dispatcher.Run();
            this.Engine.Quit(model.FinalResult);
        }

        public static void Restart()
        {
            view.Hide();
            restreq.ShowDialog();
        }
    }
}