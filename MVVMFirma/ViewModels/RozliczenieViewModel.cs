using MVVMFirma.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace MVVMFirma.ViewModels
{
    public class RozliczenieViewModel : WorkspaceViewModel
    {
        private Zlecenia _Zlecenie;
        public Zlecenia Zlecenie
        {
            get => _Zlecenie;
            set
            {
                _Zlecenie = value;
                OnPropertyChanged(nameof(Zlecenie));
                if (_Zlecenie != null)
                {
                    ZaladujBOMDlaIndeksu(_Zlecenie.IdIndeksu);
                }
            }
        }

        private ObservableCollection<dynamic> _ElementyBOM;
        public ObservableCollection<dynamic> ElementyBOM
        {
            get => _ElementyBOM;
            set
            {
                _ElementyBOM = value;
                OnPropertyChanged(nameof(ElementyBOM));
            }
        }

        // TUTAJ POPRAWIŁEM NAZWĘ NA TĄ ZE ZDJĘCIA
        private MVVMFirmaEntities13 _db = new MVVMFirmaEntities13();

        public RozliczenieViewModel()
        {
            base.DisplayName = "Rozliczenie";
        }

        public RozliczenieViewModel(Zlecenia zlecenie)
        {
            base.DisplayName = "Rozliczenie: " + (zlecenie?.NrZlecenia ?? "");
            this.Zlecenie = zlecenie;
        }

        private void ZaladujBOMDlaIndeksu(int? idIndeksu)
        {
            if (!idIndeksu.HasValue) return;

            // Szukamy danych w tabeli IndeksBOM
            var bom = _db.IndeksBOM.FirstOrDefault(b => b.IdIndeksu == idIndeksu);
            if (bom == null) return;

            var nowaLista = new ObservableCollection<dynamic>();
            decimal iloscZlecenia = Zlecenie?.Ilosc ?? 0;

            // Mapujemy wszystkie 6 surowców dla PANA
            DodajJesliIstnieje(nowaLista, 1, bom.Materialy?.Nazwa, bom.Udzial_M1, bom.Ilosc_M1, iloscZlecenia);
            DodajJesliIstnieje(nowaLista, 2, bom.Materialy1?.Nazwa, bom.Udzial_M2, bom.Ilosc_M2, iloscZlecenia);
            DodajJesliIstnieje(nowaLista, 3, bom.Materialy2?.Nazwa, bom.Udzial_M3, bom.Ilosc_M3, iloscZlecenia);
            DodajJesliIstnieje(nowaLista, 4, bom.Materialy3?.Nazwa, bom.Udzial_M4, bom.Ilosc_M4, iloscZlecenia);
            DodajJesliIstnieje(nowaLista, 5, bom.Materialy4?.Nazwa, bom.Udzial_M5, bom.Ilosc_M5, iloscZlecenia);
            DodajJesliIstnieje(nowaLista, 6, bom.Materialy5?.Nazwa, bom.Udzial_M6, bom.Ilosc_M6, iloscZlecenia);

            this.ElementyBOM = nowaLista;
        }

        private void DodajJesliIstnieje(ObservableCollection<dynamic> lista, int lp, string nazwa, decimal? udzial, decimal? naSzt, decimal calosc)
        {
            // Jeśli materiał istnieje w bazie, dodajemy go do tabeli widocznej w oknie
            if (!string.IsNullOrEmpty(nazwa))
            {
                lista.Add(new
                {
                    Lp = lp,
                    Material = nazwa,
                    UdzialProcentowy = udzial,
                    IloscNaSzt = naSzt,
                    IloscPotrzebna = (naSzt ?? 0) * calosc
                });
            }
        }
    }
}