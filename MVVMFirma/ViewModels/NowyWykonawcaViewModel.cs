using MVVMFirma.Helper;
using MVVMFirma.Models;
using MVVMFirma.Models.Validatory;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace MVVMFirma.ViewModels
{
    public class NowyWykonawcaViewModel : WorkspaceViewModel, IDataErrorInfo
    {
        // Dodajemy 'new', ponieważ WorkspaceViewModel prawdopodobnie ma już pole 'db'
        private new readonly MVVMFirmaEntities13 db = new MVVMFirmaEntities13();

        #region Fields
        private string _Numer;
        private string _Nazwa;
        private string _NIP;
        private string _Telefon;
        private string _Kraj;
        private string _Miasto;
        private string _KodPocztowy;
        private string _Ulica;
        private string _NrDomu;
        private ICommand _SaveCommand;
        #endregion

        #region Properties
        public string Numer { get { return _Numer; } set { _Numer = value; OnPropertyChanged(nameof(Numer)); } }
        public string Nazwa { get { return _Nazwa; } set { _Nazwa = value; OnPropertyChanged(nameof(Nazwa)); } }
        public string NIP { get { return _NIP; } set { _NIP = value; OnPropertyChanged(nameof(NIP)); } }
        public string Telefon { get { return _Telefon; } set { _Telefon = value; OnPropertyChanged(nameof(Telefon)); } }
        public string Kraj { get { return _Kraj; } set { _Kraj = value; OnPropertyChanged(nameof(Kraj)); } }
        public string Miasto { get { return _Miasto; } set { _Miasto = value; OnPropertyChanged(nameof(Miasto)); } }
        public string KodPocztowy { get { return _KodPocztowy; } set { _KodPocztowy = value; OnPropertyChanged(nameof(KodPocztowy)); } }
        public string Ulica { get { return _Ulica; } set { _Ulica = value; OnPropertyChanged(nameof(Ulica)); } }
        public string NrDomu { get { return _NrDomu; } set { _NrDomu = value; OnPropertyChanged(nameof(NrDomu)); } }

        public ICommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null) _SaveCommand = new BaseCommand(() => Save());
                return _SaveCommand;
            }
        }
        #endregion

        public NowyWykonawcaViewModel()
        {
            base.DisplayName = "Nowy wykonawca";
        }

        #region IDataErrorInfo
        // Używamy override, aby naprawić ostrzeżenia kompilatora CS0114
        public override string Error => null;

        public override string this[string columnName]
        {
            get
            {
                string result = null;
                if (columnName == nameof(Nazwa)) result = BiznesValidator.WalidujNazweWykonawcy(this.Nazwa);
                if (columnName == nameof(NIP)) result = BiznesValidator.WalidujNIP(this.NIP);
                if (columnName == nameof(KodPocztowy)) result = BiznesValidator.WalidujKodPocztowy(this.KodPocztowy);
                return result;
            }
        }
        #endregion

        public void Save()
        {
            // Sprawdzamy walidację najważniejszych pól
            if (string.IsNullOrEmpty(this[nameof(Nazwa)]) &&
                string.IsNullOrEmpty(this[nameof(NIP)]) &&
                string.IsNullOrEmpty(this[nameof(KodPocztowy)]))
            {
                try
                {
                    Wykonawcy nowy = new Wykonawcy
                    {
                        NrWykonawcy = this.Numer,
                        Nazwa = this.Nazwa,
                        NIP = this.NIP,
                        Telefon = this.Telefon,
                        Kraj = this.Kraj,
                        Miasto = this.Miasto,
                        KodPocztowy = this.KodPocztowy,
                        Ulica = this.Ulica,
                        NrDomu = this.NrDomu,
                        IdKraju = 1 // Domyślna wartość dla relacji
                    };

                    db.Wykonawcy.Add(nowy);
                    db.SaveChanges();

                    MessageBox.Show("Pomyślnie zapisano wykonawcę do bazy danych!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (this.CloseCommand != null)
                        this.CloseCommand.Execute(null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Wystąpił błąd podczas zapisu: " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Popraw błędy w formularzu przed zapisem.", "Uwaga", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}