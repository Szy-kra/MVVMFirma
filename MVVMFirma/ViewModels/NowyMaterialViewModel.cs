using MVVMFirma.Helper;
using MVVMFirma.Models;
using MVVMFirma.Models.Validatory;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace MVVMFirma.ViewModels
{
    public class NowyMaterialViewModel : WorkspaceViewModel, IDataErrorInfo
    {
        private new readonly MVVMFirmaEntities13 db = new MVVMFirmaEntities13();

        #region Fields
        private string _KodMaterialu;
        private string _Nazwa;
        private string _Cena;
        private string _Jednostka; // Jako string z widoku
        private string _RodzajMaterialu;
        private string _KodJednostki;
        private ICommand _SaveCommand;
        #endregion

        #region Properties
        public string KodMaterialu { get { return _KodMaterialu; } set { _KodMaterialu = value; OnPropertyChanged(nameof(KodMaterialu)); } }
        public string Nazwa { get { return _Nazwa; } set { _Nazwa = value; OnPropertyChanged(nameof(Nazwa)); } }
        public string Cena { get { return _Cena; } set { _Cena = value; OnPropertyChanged(nameof(Cena)); } }
        public string Jednostka { get { return _Jednostka; } set { _Jednostka = value; OnPropertyChanged(nameof(Jednostka)); } }
        public string RodzajMaterialu { get { return _RodzajMaterialu; } set { _RodzajMaterialu = value; OnPropertyChanged(nameof(RodzajMaterialu)); } }
        public string KodJednostki { get { return _KodJednostki; } set { _KodJednostki = value; OnPropertyChanged(nameof(KodJednostki)); } }

        public ICommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null) _SaveCommand = new BaseCommand(() => Save());
                return _SaveCommand;
            }
        }
        #endregion

        public NowyMaterialViewModel()
        {
            base.DisplayName = "Nowy Materiał";
            this.Jednostka = "1"; // Domyślnie 1 (np. Granulat/kg)
            this.KodJednostki = "kg";
        }

        #region IDataErrorInfo
        public override string Error => null;
        public override string this[string columnName]
        {
            get
            {
                string result = null;
                if (columnName == nameof(KodMaterialu)) result = BiznesValidator.WalidujKodMaterialu(this.KodMaterialu);
                if (columnName == nameof(Nazwa))
                {
                    if (string.IsNullOrEmpty(Nazwa)) result = "Nazwa jest wymagana!";
                }
                return result;
            }
        }
        #endregion

        public void Save()
        {
            if (string.IsNullOrEmpty(this[nameof(KodMaterialu)]) && string.IsNullOrEmpty(this[nameof(Nazwa)]))
            {
                try
                {
                    // Konwersja Ceny
                    decimal cenaDec = 0;
                    decimal.TryParse(this.Cena?.Replace(".", ","), out cenaDec);

                    // KONWERSJA JEDNOSTKI NA INT (naprawia Twój błąd)
                    int jednostkaInt = 1;
                    int.TryParse(this.Jednostka, out jednostkaInt);

                    Materialy nowy = new Materialy
                    {
                        KodMaterialu = this.KodMaterialu,
                        Nazwa = this.Nazwa,
                        Cena = cenaDec,
                        Jednostka = jednostkaInt, // Przypisujemy int, nie string
                        RodzajMaterialu = this.RodzajMaterialu,
                        KodJednostki = this.KodJednostki
                    };

                    db.Materialy.Add(nowy);
                    db.SaveChanges();

                    MessageBox.Show("Materiał dodany pomyślnie!", "Sukces");
                    this.CloseCommand?.Execute(null);
                }
                catch (Exception ex)
                {
                    // Wyciągamy dokładny błąd SQL
                    Exception realError = ex;
                    while (realError.InnerException != null) realError = realError.InnerException;
                    MessageBox.Show("Błąd bazy: " + realError.Message);
                }
            }
            else
            {
                MessageBox.Show("Popraw błędy w formularzu.");
            }
        }
    }
}