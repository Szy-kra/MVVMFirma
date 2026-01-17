using MVVMFirma.ViewModels; // Upewnij się, że namespace jest poprawny

namespace MVVMFirma.Helper
{
    public class ArkuszRozliczeniowy : BaseViewModel
    {
        private readonly RozliczenieViewModel _parent;

        public ArkuszRozliczeniowy(RozliczenieViewModel parent)
        {
            _parent = parent;
        }

        public string Material { get; set; }
        public string Jednostka { get; set; }
        public decimal CenaJednostkowa { get; set; }

        // PLAN - Dane wyświetlane (Norma na 1 szt.)
        public decimal? ZaplanowaneIlosc { get; set; }
        public decimal? ZaplanowaneKoszt { get; set; }

        // WYKONANIE - Tu wpisujesz dane w tabeli
        private decimal? _wykonaneIlosc;
        public decimal? WykonaneIlosc
        {
            get => _wykonaneIlosc;
            set
            {
                _wykonaneIlosc = value;
                OnPropertyChanged("WykonaneIlosc");
                OnPropertyChanged("WykonaneKoszt");
                OnPropertyChanged("Scrap");
                // PRZEKAZANIE DO NAGŁÓWKA
                if (_parent != null) _parent.IloscWykonanaNaglowek = value;
            }
        }

        // Naprawa błędu z obrazka: dodajemy pusty setter (set {;}), żeby WPF nie sypał błędami
        public decimal WykonaneKoszt
        {
            get => (WykonaneIlosc ?? 0) * CenaJednostkowa;
            set {; }
        }

        // ZAFAKTUROWANE
        private decimal? _zafakturowaneIlosc;
        public decimal? ZafakturowaneIlosc
        {
            get => _zafakturowaneIlosc;
            set
            {
                _zafakturowaneIlosc = value;
                OnPropertyChanged("ZafakturowaneIlosc");
                OnPropertyChanged("ZafakturowaneKoszt");
                OnPropertyChanged("Scrap");
                // PRZEKAZANIE DO NAGŁÓWKA
                if (_parent != null) _parent.IloscZafakturowanaNaglowek = value;
            }
        }

        public decimal ZafakturowaneKoszt
        {
            get => (ZafakturowaneIlosc ?? 0) * CenaJednostkowa;
            set {; }
        }

        // SCRAP w sztukach
        public decimal Scrap
        {
            get => (WykonaneIlosc ?? 0) - (ZafakturowaneIlosc ?? 0);
            set {; }
        }
    }
}