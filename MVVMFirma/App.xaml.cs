using MVVMFirma.ViewModels;
using MVVMFirma.Views;
using System.Windows;

namespace MVVMFirma
{
    /// <summary>
    /// Główna klasa aplikacji zarządzająca startem systemu.
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Tworzymy główne okno
            MainWindow window = new MainWindow();

            // Przypisujemy ViewModel do okna - to serce Twojego MVVM
            // Upewnij się, że klasa MainWindowViewModel istnieje w namespace MVVMFirma.ViewModels
            window.DataContext = new MainWindowViewModel();

            // Wyświetlamy okno
            window.Show();
        }
    }
}