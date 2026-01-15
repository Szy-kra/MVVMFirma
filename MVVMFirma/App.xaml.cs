using MVVMFirma.ViewModels;
using MVVMFirma.Views;
using System.Windows;

namespace MVVMFirma
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow window = new MainWindow();
            window.DataContext = new MainWindowViewModel();
            window.Show();
        }
    }
}
