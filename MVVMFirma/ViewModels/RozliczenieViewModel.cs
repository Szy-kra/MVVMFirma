using MVVMFirma.Helper;
using MVVMFirma.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace MVVMFirma.ViewModels
{
    public class ArkuszRozliczeniowyItem : BaseViewModel
    {
        private string _material;
        public string Material
        {
            get { return _material; }
            set { _material = value; OnPropertyChanged("Material"); }
        }

        private decimal? _zaplanowaneIlosc;
        public decimal? ZaplanowaneIlosc
        {
            get { return _zaplanowaneIlosc; }
            set { _zaplanowaneIlosc = value; OnPropertyChanged("ZaplanowaneIlosc"); }
        }

        private decimal? _wykonaneIlosc;
        public decimal? WykonaneIlosc
        {
            get { return _wykonaneIlosc; }
            set { _wykonaneIlosc = value; OnPropertyChanged("WykonaneIlosc"); }
        }

        private decimal? _zafakturowaneIlosc;
        public decimal? ZafakturowaneIlosc
        {
            get { return _zafakturowaneIlosc; }
            set { _zafakturowaneIlosc = value; OnPropertyChanged("ZafakturowaneIlosc"); }
        }
    }

    public class RozliczenieViewModel : WorkspaceViewModel
    {
        private Zlecenia _zlecenie;
        public Zlecenia Zlecenie
        {
            get { return _zlecenie; }
            set { _zlecenie = value; OnPropertyChanged("Zlecenie"); }
        }

        // Bindowania danych ze struktur Zlecenie -> Indeksy -> Formy
        public string KodFormy
        {
            get { return (Zlecenie != null && Zlecenie.Indeksy != null && Zlecenie.Indeksy.Formy != null) ? Zlecenie.Indeksy.Formy.KodFormy : string.Empty; }
        }

        public int? IloscGniazd
        {
            get { return (Zlecenie != null && Zlecenie.Indeksy != null && Zlecenie.Indeksy.Formy != null) ? (int?)Zlecenie.Indeksy.Formy.IloscGniazd : null; }
        }

        public decimal? CzasCyklu
        {
            get { return (Zlecenie != null && Zlecenie.Indeksy != null && Zlecenie.Indeksy.Formy != null) ? (decimal?)Zlecenie.Indeksy.Formy.CzasCyklu : null; }
        }

        private decimal? _kosztUslugi;
        public decimal? KosztUslugi { get { return _kosztUslugi; } set { _kosztUslugi = value; OnPropertyChanged("KosztUslugi"); } }

        private decimal? _kosztTransportu;
        public decimal? KosztTransportu { get { return _kosztTransportu; } set { _kosztTransportu = value; OnPropertyChanged("KosztTransportu"); } }

        public ObservableCollection<ArkuszRozliczeniowyItem> ArkuszRozliczeniowy { get; set; }
        public ObservableCollection<BOMResult> BomItems { get; set; }

        public RozliczenieViewModel(Zlecenia wybrane)
        {
            ArkuszRozliczeniowy = new ObservableCollection<ArkuszRozliczeniowyItem>();
            BomItems = new ObservableCollection<BOMResult>();

            if (wybrane != null)
            {
                Zlecenie = db.Zlecenia.FirstOrDefault(z => z.Id == wybrane.Id);
                LoadData();
            }
        }

        private void LoadData()
        {
            if (Zlecenie == null || Zlecenie.Indeksy == null) return;

            // Wywołanie displayBOM / Kalkulatora
            var daneBOM = BOMCalculator.PobierzIOblicz(db, Zlecenie.Indeksy.KodIndeksu, Zlecenie.Ilosc.ToString());

            foreach (var item in daneBOM)
            {
                BomItems.Add(item);
                ArkuszRozliczeniowy.Add(new ArkuszRozliczeniowyItem
                {
                    Material = item.Material,
                    ZaplanowaneIlosc = item.Ilosc
                });
            }

            // Odświeżenie pól wyliczanych
            OnPropertyChanged("KodFormy");
            OnPropertyChanged("IloscGniazd");
            OnPropertyChanged("CzasCyklu");
        }
    }
}