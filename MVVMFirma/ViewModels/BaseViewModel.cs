using MVVMFirma.Helper;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls; // Potrzebne dla UserControl
using System.Windows.Input;

namespace MVVMFirma.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        #region DisplayName
        public virtual string DisplayName { get; protected set; }
        #endregion

        #region Window Commands
        public ICommand Close => new BaseCommand(() => Application.Current.Shutdown());

        public ICommand Maximize => new BaseCommand(() =>
        {
            var win = Application.Current.MainWindow;
            if (win != null)
            {
                win.WindowState = win.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }
        });

        public ICommand Minimize => new BaseCommand(() =>
        {
            var win = Application.Current.MainWindow;
            if (win != null)
            {
                win.WindowState = WindowState.Minimized;
            }
        });

        public ICommand DragMove => new BaseCommand(() =>
        {
            try
            {
                Application.Current.MainWindow?.DragMove();
            }
            catch { }
        });

        public ICommand Restart => new BaseCommand(() =>
        {
            var processName = Process.GetCurrentProcess().MainModule?.FileName;
            if (processName != null)
            {
                Process.Start(processName);
                Application.Current.Shutdown();
            }
        });
        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region IDataErrorInfo Members
        // Tu bêdzie Twoja walidacja z format hints
        public virtual string Error => null;
        public virtual string this[string columnName] => null;
        #endregion
    }
}

// KLASY BAZOWE DLA WIDOKÓW (Wrzucamy tutaj, ¿eby nie mno¿yæ plików)
namespace MVVMFirma.Views
{
    public class WszystkieViewBase : UserControl { }
    public class JedenViewBase : UserControl { }
}