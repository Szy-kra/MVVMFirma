using MVVMFirma.Helper;
using MVVMFirma.Models;
using MVVMFirma.Models.Shared;
using MVVMFirma.Models.Validatory;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows.Input;

namespace MVVMFirma.ViewModels
{
    public class NoweZlecenieViewModel : WorkspaceViewModel, IDataErrorInfo
    {
        private readonly MVVMFirmaEntities13 _db;

        #region Fields
        private string _NrZlecenia;
        private string _Ilosc = "1";
        private int? _IdWykonawcy;
        private Indeksy _WybranyIndeks;
        private string _TekstWstawek;
        private string _OpisDetalu;
        private ObservableCollection<DisplayBom> _WidokBOM;
        private ICommand _SaveCommand;
        private ObservableCollection<Wykonawcy> _ListaWykonawcow;
        private ObservableCollection<Indeksy> _ListaIndeksow;
        #endregion

        #region Properties
        public ObservableCollection<Wykonawcy> ListaWykonawcow
        {
            get => _ListaWykonawcow;
            set { _ListaWykonawcow = value; OnPropertyChanged(nameof(ListaWykonawcow)); }
        }

        public ObservableCollection<Indeksy> ListaIndeksow
        {
            get => _ListaIndeksow;
            set { _ListaIndeksow = value; OnPropertyChanged(nameof(ListaIndeksow)); }
        }

        public string NrZlecenia
        {
            get => _NrZlecenia;
            set { _NrZlecenia = value; OnPropertyChanged(nameof(NrZlecenia)); }
        }

        public string Ilosc
        {
            get => _Ilosc;
            set { _Ilosc = value; OnPropertyChanged(nameof(Ilosc)); UpdateDaneTechniczne(); }
        }

        public string OpisDetalu
        {
            get => _OpisDetalu;
            set { _OpisDetalu = value; OnPropertyChanged(nameof(OpisDetalu)); }
        }

        public int? IdWykonawcy
        {
            get => _IdWykonawcy;
            set { _IdWykonawcy = value; OnPropertyChanged(nameof(IdWykonawcy)); }
        }

        public Indeksy WybranyIndeks
        {
            get => _WybranyIndeks;
            set
            {
                if (_WybranyIndeks == value) return;
                _WybranyIndeks = value;
                OnPropertyChanged(nameof(WybranyIndeks));
                UpdateDaneTechniczne();
            }
        }

        public string TekstWstawek
        {
            get => _TekstWstawek;
            set { _TekstWstawek = value; OnPropertyChanged(nameof(TekstWstawek)); }
        }

        public ObservableCollection<DisplayBom> WidokBOM
        {
            get => _WidokBOM;
            set { _WidokBOM = value; OnPropertyChanged(nameof(WidokBOM)); }
        }

        public ICommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null)
                {
                    _SaveCommand = new BaseCommand(() => Save());
                }
                return _SaveCommand;
            }
        }
        #endregion

        public NoweZlecenieViewModel()
        {
            this.DisplayName = "Nowe zlecenie";
            _db = new MVVMFirmaEntities13();
            LoadData();
            NrZlecenia = ZlecenieHelper.GenerujNastępnyNumerZlecenia(_db);
        }

        private void LoadData()
        {
            try
            {
                ListaWykonawcow = new ObservableCollection<Wykonawcy>(_db.Wykonawcy.OrderBy(w => w.Nazwa).ToList());
                var indeksy = _db.Indeksy
                    .Include(i => i.Formy)
                    .Include(i => i.GrupyWstawek.Wstawki)
                    .OrderBy(i => i.KodIndeksu).ToList();
                ListaIndeksow = new ObservableCollection<Indeksy>(indeksy);
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }
        }

        private void UpdateDaneTechniczne()
        {
            if (WybranyIndeks == null) return;

            OpisDetalu = WybranyIndeks.Opis ?? "Brak opisu";

            if (WybranyIndeks.GrupyWstawek?.Wstawki != null && WybranyIndeks.GrupyWstawek.Wstawki.Any())
                TekstWstawek = string.Join(", ", WybranyIndeks.GrupyWstawek.Wstawki.Select(w => w.KodWstawki));
            else
                TekstWstawek = "Brak";

            var listaSkladnikowZDb = _db.IndeksBOM
                .Where(b => b.IdIndeksu == WybranyIndeks.Id)
                .Include(b => b.Materialy)
                .ToList();

            WidokBOM = new ObservableCollection<DisplayBom>(BOMCalculator.ObliczZapotrzebowanie(listaSkladnikowZDb, Ilosc));
        }

        public void Save()
        {
            // Zmieniono na decimal.TryParse, aby wspierać ułamki (np. kg, mb)
            // Jeśli baza Zlecenia.Ilosc przyjmuje tylko int, rzutujemy na (int)
            if (string.IsNullOrEmpty(this[nameof(Ilosc)]) && WybranyIndeks != null && !string.IsNullOrEmpty(NrZlecenia))
            {
                decimal.TryParse(Ilosc, out decimal iloscDecimal);

                var nz = new Zlecenia
                {
                    NrZlecenia = NrZlecenia,
                    // Jeśli w bazie masz int, zostaw (int)iloscDecimal, 
                    // ale jeśli masz decimal, przypisz po prostu iloscDecimal
                    Ilosc = (int)iloscDecimal,
                    IdWykonawcy = IdWykonawcy ?? 0,
                    IdIndeksu = WybranyIndeks.Id,
                    DataZlecenia = DateTime.Now
                };
                _db.Zlecenia.Add(nz);
                _db.SaveChanges();
                if (this.CloseCommand != null) this.CloseCommand.Execute(null);
            }
        }

        public override string Error => null;
        public override string this[string name]
        {
            get
            {
                string result = null;
                if (name == nameof(NrZlecenia)) result = BiznesValidator.WalidujNumerZlecenia(NrZlecenia);
                if (name == nameof(Ilosc)) result = StringValidator.SprawdzCzyLiczba(Ilosc);
                return result;
            }
        }
    }
}