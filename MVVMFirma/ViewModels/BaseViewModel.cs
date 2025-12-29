using MVVMFirma.Helper;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MVVMFirma.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        #region DisplayName
        public virtual string DisplayName { get; protected set; }
        #endregion

        #region Window Commands

        public ICommand Close => new BaseCommand(() => Application.Current.Shutdown());

        public ICommand Maximize => new BaseCommand(() =>
        {
            var win = Application.Current.MainWindow;
            win.WindowState = win.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        });

        public ICommand Minimize => new BaseCommand(() =>
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        });

        public ICommand DragMove => new BaseCommand(() =>
        {
            Application.Current.MainWindow.DragMove();
        });

        public ICommand Restart => new BaseCommand(() =>
        {
            Application.Current.Shutdown();
            // tu mo¿na dodaæ restart procesu, jeœli potrzebne
        });

        #endregion

        #region PropertyChanged

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
