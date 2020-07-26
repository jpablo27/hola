using CustomBA.Models;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Windows.Input;
using CustomBA.Views;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace CustomBA.ViewModels
{
    public class InstallViewModel : NotificationObject
    {
        public enum InstallState
        {
            Initializing,
            Present,
            NotPresent,
            Applying,
            Cancelled
        }
        private InstallState state;
        private string message;
        private BootstrapperApplicationModel model;
        public ICommand InstallCommand { get; private set; }
        public ICommand UninstallCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public string Message
        {
            get
            {
                return this.message;
            }
            set
            {
                if (this.message != value)
                {
                    this.message = value;
                    this.RaisePropertyChanged(() => this.Message);
                }
            }
        }
        public InstallState State
        {
            get
            {
                return this.state;
            }
            set
            {
                if (this.state != value)
                {
                    this.state = value;
                    this.Message = this.state.ToString();
                    this.RaisePropertyChanged(() => this.State);
                    this.Refresh();
                }
            }
        }
        public InstallViewModel(
        BootstrapperApplicationModel model)
        {
            this.model = model;
            this.State = InstallState.Initializing;
            this.WireUpEventHandlers();
            this.InstallCommand = new DelegateCommand(() =>
            this.model.PlanAction(LaunchAction.Install),
            () => this.State == InstallState.NotPresent);
            this.UninstallCommand = new DelegateCommand(() =>
            this.model.PlanAction(LaunchAction.Uninstall),
            () => this.State == InstallState.Present);
            this.CancelCommand = new DelegateCommand(() =>
            {
                this.model.LogMessage("Cancelling...");
                if (this.State == InstallState.Applying)
                {
                    this.State = InstallState.Cancelled;
                }
                else
                {
                    CustomBootstrapperApplication.Dispatcher
                    .InvokeShutdown();
                }
            }, () => this.State != InstallState.Cancelled);
        }
        protected void DetectPackageComplete(
        object sender,
        DetectPackageCompleteEventArgs e)
        {
            if (e.PackageId.Equals(
            "vcredist_x64.exe", StringComparison.Ordinal))
            {
                this.State = e.State == PackageState.Present ?
                    InstallState.Present : InstallState.NotPresent;
            }
        }
        protected void PlanComplete(
        object sender, PlanCompleteEventArgs e)
        {
            if (this.State == InstallState.Cancelled)
            {
                CustomBootstrapperApplication.Dispatcher
                .InvokeShutdown();
                return;
            }
            this.model.ApplyAction();
        }
        protected void ApplyBegin(
        object sender, ApplyBeginEventArgs e)
        {
            this.State = InstallState.Applying;
        }
        protected void ExecutePackageBegin(
        object sender, ExecutePackageBeginEventArgs e)
        {
            if (this.State == InstallState.Cancelled)
            {
                e.Result = Result.Cancel;
            }
        }
        protected void ExecutePackageComplete(
        object sender, ExecutePackageCompleteEventArgs e)
        {
            if (this.State == InstallState.Cancelled)
            {
                e.Result = Result.Cancel;
            }
        }
        protected void ApplyComplete(
        object sender, ApplyCompleteEventArgs e)
        {
            this.model.FinalResult = e.Status;

            NewWindowHandler(null, new RoutedEventArgs());

            this.model.LogMessage("OOOOOOOOOOOOOOOOOOOOO");
            //CustomBootstrapperApplication.Dispatcher
            //.InvokeShutdown();
            
        }
        private void Refresh()
        {
            CustomBootstrapperApplication.Dispatcher.Invoke(
            (Action)(() =>
            {
                ((DelegateCommand)this.InstallCommand)
    .RaiseCanExecuteChanged();
                ((DelegateCommand)this.UninstallCommand)
    .RaiseCanExecuteChanged();
                ((DelegateCommand)this.CancelCommand)
    .RaiseCanExecuteChanged();
            }));
        }
        private void WireUpEventHandlers()
        {
            this.model.BootstrapperApplication.DetectPackageComplete
            += this.DetectPackageComplete;
            this.model.BootstrapperApplication.PlanComplete += this.
            PlanComplete;
            this.model.BootstrapperApplication.ApplyComplete += this.
            ApplyComplete;
            this.model.BootstrapperApplication.ApplyBegin += this.
            ApplyBegin;
            this.model.BootstrapperApplication.ExecutePackageBegin +=
            this.ExecutePackageBegin;
            this.model.BootstrapperApplication.ExecutePackageComplete
            += this.ExecutePackageComplete;

            this.model.BootstrapperApplication.RestartRequired += BootstrapperApplication_RestartRequired;
        }

        private void BootstrapperApplication_RestartRequired(object sender, RestartRequiredEventArgs e)
        {
            e.Restart = true;

  
        }
        private static Thread newWindowThread;
        private void NewWindowHandler(object sender, RoutedEventArgs e)
        {
            newWindowThread = new Thread(new ThreadStart(ThreadStartingPoint));
            newWindowThread.SetApartmentState(ApartmentState.STA);
            newWindowThread.IsBackground = true;
            newWindowThread.Start();
            
        }
        public static Dispatcher rst;
        private void ThreadStartingPoint()
        {
            
            var tempWindow = new restart();
            tempWindow.Show();
            
            Dispatcher.Run();
            CustomBootstrapperApplication.Dispatcher.InvokeShutdown();


        }
    }
}