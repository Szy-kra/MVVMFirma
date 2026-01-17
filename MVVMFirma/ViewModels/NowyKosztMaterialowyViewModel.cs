using MVVMFirma.Helper;
using MVVMFirma.Models;
using MVVMFirma.Models.Validatory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MVVMFirma.ViewModels
{
    public class NowyKosztMaterialowyViewModel : WorkspaceViewModel, IDataErrorInfo
    {
        private new readonly MVVMFirmaEntities13 db = new MVVMFirmaEntities13();

        #region Fields
        private string _KodMaterialu;
        private string _Opis;
        private string _CenaJednostkowa;
        private string _JednostkaMiary;
        private DateTime _DataModyfikacji;
        private Materialy _WybranyMaterial;
        private ICommand _SaveCommand;
        #endregion

        #region Properties
        public string KodMaterialu { get { return _KodMaterialu; } set { _KodMaterialu = value; OnPropertyChanged(nameof(KodMaterialu)); } }
        public string Opis { get { return _Opis; } set { _Opis = value; OnPropertyChanged(nameof(Opis)); } }
        public string CenaJednostkowa { get { return _CenaJednostkowa; } set { _CenaJednostkowa = value; OnPropertyChanged(nameof(CenaJednostkowa)); } }
        public string JednostkaMiary { get { return _JednostkaMiary; } set { _JednostkaMiary = value; OnPropertyChanged(nameof(JednostkaMiary)); } }
        public DateTime DataModyfikacji { get { return _DataModyfikacji; } set { _DataModyfikacji = value; OnPropertyChanged(nameof(DataModyfikacji)); } }

        // Relacja: Wybór materiału z Twojej listy (Id, KodMaterialu, Nazwa...)
        public List<Materialy> ListaMaterialow { get; set; }
        public Materialy WybranyMaterial
        {
            get { return _WybranyMaterial; }
            set
            {
                _WybranyMaterial = value;
                if (_WybranyMaterial != null)
                {
                    // Automatycznie podpowiadamy kod i jednostkę z materiału
                    this.KodMaterialu = _WybranyMaterial.KodMaterialu;
                    this.JednostkaMiary = _WybranyMaterial.KodJednostki; // Używamy Twojej kolumny KodJednostki
                }
                OnPropertyChanged(nameof(WybranyMaterial));
            }
        }

        public List<string> DostepneJednostki { get; set; }

        public ICommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null) _SaveCommand = new BaseCommand(() => Save());
                return _SaveCommand;
            }
        }
        #endregion

        public NowyKosztMaterialowyViewModel()
        {
            base.DisplayName = "Nowy koszt materiałowy";

            // Pobieramy dane z tabeli nadrzędnej
            this.ListaMaterialow = db.Materialy.ToList();
            this.DostepneJednostki = new List<string> { "szt", "kg" };

            this.DataModyfikacji = DateTime.Now;
            this.CenaJednostkowa = "0,00";
        }

        #region IDataErrorInfo
        public override string Error => null;
        public override string this[string columnName]
        {
            get
            {
                string result = null;
                if (columnName == nameof(KodMaterialu)) result = BiznesValidator.WalidujKodMaterialu(this.KodMaterialu);
                if (columnName == nameof(CenaJednostkowa)) result = BiznesValidator.WalidujCeneMaterialu(this.CenaJednostkowa);
                if (columnName == nameof(WybranyMaterial) && WybranyMaterial == null) result = "Musisz wybrać materiał z bazy!";
                return result;
            }
        }
        #endregion

        public void Save()
        {
            if (string.IsNullOrEmpty(this[nameof(KodMaterialu)]) &&
                string.IsNullOrEmpty(this[nameof(CenaJednostkowa)]) &&
                WybranyMaterial != null)
            {
                try
                {
                    decimal cena = decimal.Parse(this.CenaJednostkowa.Replace(".", ","), new CultureInfo("pl-PL"));

                    KosztyMaterialowe nowy = new KosztyMaterialowe
                    {
                        // To przypisanie rozwiązuje błąd FK (Foreign Key)
                        IdMaterialu = this.WybranyMaterial.Id,

                        KodMaterialu = this.KodMaterialu,
                        Opis = this.Opis,
                        CenaJednostkowa = cena,
                        JednostkaMiary = this.JednostkaMiary,
                        DataModyfikacji = this.DataModyfikacji,
                        DataUtworzenia = DateTime.Now,
                        Aktywne = true
                    };

                    db.KosztyMaterialowe.Add(nowy);
                    db.SaveChanges();

                    MessageBox.Show("Pomyślnie dodano koszt dla materiału: " + WybranyMaterial.KodMaterialu, "Sukces");
                    this.CloseCommand?.Execute(null);
                }
                catch (Exception ex)
                {
                    Exception realError = ex;
                    while (realError.InnerException != null) realError = realError.InnerException;
                    MessageBox.Show("Błąd zapisu: " + realError.Message);
                }
            }
            else
            {
                MessageBox.Show("Wypełnij poprawnie wszystkie pola i wybierz materiał.");
            }
        }
    }
}