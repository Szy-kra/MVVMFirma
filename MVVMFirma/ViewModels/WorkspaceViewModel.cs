using MVVMFirma.Helper;
using MVVMFirma.Models; // Upewnij się, że ta przestrzeń nazw odpowiada Twoim modelom EF
using System;
using System.Windows.Input;

namespace MVVMFirma.ViewModels
{
    public abstract class WorkspaceViewModel : BaseViewModel
    {
        #region Fields
        private BaseCommand _CloseCommand;

        // Tu dodajemy dostęp do bazy danych, który będzie używany w całym projekcie
        // Nazwa MVVMFirmaEntities13 musi być zgodna z Twoim plikiem .Context.cs
        protected readonly MVVMFirmaEntities13 db;
        #endregion

        #region Constructor
        public WorkspaceViewModel()
        {
            // Inicjalizacja bazy danych raz dla każdej zakładki
            db = new MVVMFirmaEntities13();
        }
        #endregion

        #region CloseCommand
        public ICommand CloseCommand
        {
            get
            {
                if (_CloseCommand == null)
                    _CloseCommand = new BaseCommand(() => this.OnRequestClose());
                return _CloseCommand;
            }
        }
        #endregion

        #region RequestClose [event]
        public event EventHandler RequestClose;
        protected void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
        #endregion
    }
}