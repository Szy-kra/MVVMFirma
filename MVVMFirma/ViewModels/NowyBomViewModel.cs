using MVVMFirma.Helper;
using MVVMFirma.Models;
using MVVMFirma.Models.Shared;
using MVVMFirma.Models.Validatory;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MVVMFirma.ViewModels
{
    public class NowyBomViewModel : WorkspaceViewModel
    {
        #region Pola
        private MVVMFirmaEntities13 _db;
        private string _wybranyIndeks;

        private string _mat1, _ilosc1, _jedn1;
        private string _mat2, _ilosc2, _jedn2;
        private string _mat3, _ilosc3, _jedn3;
        private string _mat4, _ilosc4, _jedn4;
        private string _mat5, _ilosc5, _jedn5;
        private string _mat6, _ilosc6, _jedn6;
        private string _mat7, _ilosc7, _jedn7;
        #endregion

        #region Właściwości
        public string WybranyIndeks
        {
            get => _wybranyIndeks;
            set
            {
                _wybranyIndeks = value;
                OnPropertyChanged(nameof(WybranyIndeks));
                PobierzDaneZKlasyDisplayBom();
            }
        }

        public ObservableCollection<string> ListaIndeksow { get; set; }
        public ObservableCollection<string> ListaMaterialow { get; set; }
        public ObservableCollection<string> ListaJednostek { get; set; }

        public string Mat1 { get => _mat1; set { _mat1 = value; OnPropertyChanged(nameof(Mat1)); } }
        public string Ilosc1 { get => _ilosc1; set { _ilosc1 = value; OnPropertyChanged(nameof(Ilosc1)); } }
        public string Jedn1 { get => _jedn1; set { _jedn1 = value; OnPropertyChanged(nameof(Jedn1)); } }

        public string Mat2 { get => _mat2; set { _mat2 = value; OnPropertyChanged(nameof(Mat2)); } }
        public string Ilosc2 { get => _ilosc2; set { _ilosc2 = value; OnPropertyChanged(nameof(Ilosc2)); } }
        public string Jedn2 { get => _jedn2; set { _jedn2 = value; OnPropertyChanged(nameof(Jedn2)); } }

        public string Mat3 { get => _mat3; set { _mat3 = value; OnPropertyChanged(nameof(Mat3)); } }
        public string Ilosc3 { get => _ilosc3; set { _ilosc3 = value; OnPropertyChanged(nameof(Ilosc3)); } }
        public string Jedn3 { get => _jedn3; set { _jedn3 = value; OnPropertyChanged(nameof(Jedn3)); } }

        public string Mat4 { get => _mat4; set { _mat4 = value; OnPropertyChanged(nameof(Mat4)); } }
        public string Ilosc4 { get => _ilosc4; set { _ilosc4 = value; OnPropertyChanged(nameof(Ilosc4)); } }
        public string Jedn4 { get => _jedn4; set { _jedn4 = value; OnPropertyChanged(nameof(Jedn4)); } }

        public string Mat5 { get => _mat5; set { _mat5 = value; OnPropertyChanged(nameof(Mat5)); } }
        public string Ilosc5 { get => _ilosc5; set { _ilosc5 = value; OnPropertyChanged(nameof(Ilosc5)); } }
        public string Jedn5 { get => _jedn5; set { _jedn5 = value; OnPropertyChanged(nameof(Jedn5)); } }

        public string Mat6 { get => _mat6; set { _mat6 = value; OnPropertyChanged(nameof(Mat6)); } }
        public string Ilosc6 { get => _ilosc6; set { _ilosc6 = value; OnPropertyChanged(nameof(Ilosc6)); } }
        public string Jedn6 { get => _jedn6; set { _jedn6 = value; OnPropertyChanged(nameof(Jedn6)); } }

        public string Mat7 { get => _mat7; set { _mat7 = value; OnPropertyChanged(nameof(Mat7)); } }
        public string Ilosc7 { get => _ilosc7; set { _ilosc7 = value; OnPropertyChanged(nameof(Ilosc7)); } }
        public string Jedn7 { get => _jedn7; set { _jedn7 = value; OnPropertyChanged(nameof(Jedn7)); } }
        #endregion

        #region Walidacja (IDataErrorInfo Override)
        // Nadpisujemy właściwości z BaseViewModel/WorkspaceViewModel
        public override string Error => null;

        public override string this[string columnName]
        {
            get
            {
                string result = null;
                switch (columnName)
                {
                    case nameof(Ilosc1): result = BiznesValidator.WalidujIloscZlecona(Ilosc1); break;
                    case nameof(Ilosc2): result = BiznesValidator.WalidujIloscZlecona(Ilosc2); break;
                    case nameof(Ilosc3): result = BiznesValidator.WalidujIloscZlecona(Ilosc3); break;
                    case nameof(Ilosc4): result = BiznesValidator.WalidujIloscZlecona(Ilosc4); break;
                    case nameof(Ilosc5): result = BiznesValidator.WalidujIloscZlecona(Ilosc5); break;
                    case nameof(Ilosc6): result = BiznesValidator.WalidujIloscZlecona(Ilosc6); break;
                    case nameof(Ilosc7): result = BiznesValidator.WalidujIloscZlecona(Ilosc7); break;
                }
                return result;
            }
        }
        #endregion

        public ICommand SaveCommand { get; }

        public NowyBomViewModel()
        {
            base.DisplayName = "Zapisz Strukturę";
            _db = new MVVMFirmaEntities13();
            LadowanieDanychSlownikowych();
            SaveCommand = new BaseCommand(() => ZapiszDoBazy());
        }

        #region Metody
        private void PobierzDaneZKlasyDisplayBom()
        {
            if (string.IsNullOrEmpty(WybranyIndeks)) return;
            var daneZBazy = DisplayBom.LoadForIndex(_db, WybranyIndeks);
            if (daneZBazy != null)
            {
                WyczyscPola();
                if (daneZBazy.Count >= 1) { Mat1 = daneZBazy[0].Material; Ilosc1 = daneZBazy[0].Ilosc.ToString(); Jedn1 = daneZBazy[0].Jednostka; }
                if (daneZBazy.Count >= 2) { Mat2 = daneZBazy[1].Material; Ilosc2 = daneZBazy[1].Ilosc.ToString(); Jedn2 = daneZBazy[1].Jednostka; }
                if (daneZBazy.Count >= 3) { Mat3 = daneZBazy[2].Material; Ilosc3 = daneZBazy[2].Ilosc.ToString(); Jedn3 = daneZBazy[2].Jednostka; }
                if (daneZBazy.Count >= 4) { Mat4 = daneZBazy[3].Material; Ilosc4 = daneZBazy[3].Ilosc.ToString(); Jedn4 = daneZBazy[3].Jednostka; }
                if (daneZBazy.Count >= 5) { Mat5 = daneZBazy[4].Material; Ilosc5 = daneZBazy[4].Ilosc.ToString(); Jedn5 = daneZBazy[4].Jednostka; }
                if (daneZBazy.Count >= 6) { Mat6 = daneZBazy[5].Material; Ilosc6 = daneZBazy[5].Ilosc.ToString(); Jedn6 = daneZBazy[5].Jednostka; }
                if (daneZBazy.Count >= 7) { Mat7 = daneZBazy[6].Material; Ilosc7 = daneZBazy[6].Ilosc.ToString(); Jedn7 = daneZBazy[6].Jednostka; }
            }
        }

        private void WyczyscPola()
        {
            Mat1 = Mat2 = Mat3 = Mat4 = Mat5 = Mat6 = Mat7 = string.Empty;
            Ilosc1 = Ilosc2 = Ilosc3 = Ilosc4 = Ilosc5 = Ilosc6 = Ilosc7 = "0";
        }

        private void ZapiszDoBazy()
        {
            try
            {
                var nadrzedny = _db.Indeksy.FirstOrDefault(i => i.KodIndeksu == WybranyIndeks);
                if (nadrzedny == null)
                {
                    CheckData.ShowInfo("Nie znaleziono indeksu nadrzędnego!");
                    return;
                }

                var aktualnyBOM = _db.IndeksBOM.Where(b => b.Indeksy.KodIndeksu == WybranyIndeks).ToList();

                AktualizujRekord(nadrzedny, Mat1, Ilosc1, aktualnyBOM.ElementAtOrDefault(0));
                AktualizujRekord(nadrzedny, Mat2, Ilosc2, aktualnyBOM.ElementAtOrDefault(1));
                AktualizujRekord(nadrzedny, Mat3, Ilosc3, aktualnyBOM.ElementAtOrDefault(2));
                AktualizujRekord(nadrzedny, Mat4, Ilosc4, aktualnyBOM.ElementAtOrDefault(3));
                AktualizujRekord(nadrzedny, Mat5, Ilosc5, aktualnyBOM.ElementAtOrDefault(4));
                AktualizujRekord(nadrzedny, Mat6, Ilosc6, aktualnyBOM.ElementAtOrDefault(5));
                AktualizujRekord(nadrzedny, Mat7, Ilosc7, aktualnyBOM.ElementAtOrDefault(6));

                _db.SaveChanges();
                CheckData.ShowInfo($"Zapisano zmiany dla {WybranyIndeks}!");
            }
            catch (Exception ex)
            {
                CheckData.ShowError(ex, "Błąd zapisu struktury BOM");
            }
        }

        private void AktualizujRekord(Indeksy nadrzedny, string nazwaMat, string iloscStr, IndeksBOM istniejacy)
        {
            if (string.IsNullOrWhiteSpace(nazwaMat)) return;

            decimal nowaIlosc;
            if (!decimal.TryParse(iloscStr, out nowaIlosc)) nowaIlosc = 0;

            if (istniejacy != null)
            {
                istniejacy.Ilosc = nowaIlosc;
            }
            else
            {
                var material = _db.Materialy.FirstOrDefault(m => m.Nazwa == nazwaMat);
                if (material != null)
                {
                    _db.IndeksBOM.Add(new IndeksBOM
                    {
                        Indeksy = nadrzedny,
                        Materialy = material,
                        Ilosc = nowaIlosc
                    });
                }
            }
        }

        private void LadowanieDanychSlownikowych()
        {
            try
            {
                ListaIndeksow = new ObservableCollection<string>(_db.Indeksy.Select(i => i.KodIndeksu).ToList());
                ListaMaterialow = new ObservableCollection<string>(_db.Materialy.Select(m => m.Nazwa).ToList());
                ListaJednostek = new ObservableCollection<string>(_db.Materialy.Where(m => m.KodJednostki != null).Select(m => m.KodJednostki).Distinct().ToList());
            }
            catch (Exception ex) { CheckData.ShowError(ex, "Słowniki"); }
        }
        #endregion
    }
}