using MVVMFirma.Helper;
using MVVMFirma.Models;
using MVVMFirma.Models.Shared;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows.Input;

namespace MVVMFirma.ViewModels
{
    public class RozliczenieViewModel : WorkspaceViewModel
    {
        #region Properties
        private Zlecenia _zlecenie;
        public Zlecenia Zlecenie
        {
            get => _zlecenie;
            set { _zlecenie = value; OnPropertyChanged(); }
        }

        // Kolekcje zgodne z Twoim modelem DisplayBom i relacjami SQL
        public ObservableCollection<DisplayBom> BomItems { get; set; }
        public ObservableCollection<Faktury> Faktury { get; set; }
        public ObservableCollection<ArkuszRozliczeniowyItem> ArkuszRozliczeniowy { get; set; }

        public ICommand SaveCommand { get; }
        #endregion

        public RozliczenieViewModel(Zlecenia wybraneZlecenie) : base()
        {
            if (wybraneZlecenie == null) return;

            // Inicjalizacja list
            BomItems = new ObservableCollection<DisplayBom>();
            Faktury = new ObservableCollection<Faktury>();
            ArkuszRozliczeniowy = new ObservableCollection<ArkuszRozliczeniowyItem>();

            // Pobieranie danych z bazy z uwzględnieniem relacji
            this.Zlecenie = db.Zlecenia
                .Include(z => z.Indeksy.Formy)
                .Include(z => z.Indeksy.IndeksBOM.Select(b => b.Materialy.Jednostki))
                .Include(z => z.Faktury)
                .FirstOrDefault(z => z.Id == wybraneZlecenie.Id);

            this.DisplayName = "Rozliczenie: " + (Zlecenie?.NrZlecenia ?? "");

            SaveCommand = new BaseCommand(() => SaveAction());

            OdswiezDane();
        }

        private void OdswiezDane()
        {
            if (Zlecenie?.Indeksy == null) return;

            // 1. BOM - używamy Twojego kalkulatora. 1m to decimal (ilość na 1 sztukę)
            var listaBOM = db.IndeksBOM
                .Where(b => b.IdIndeksu == Zlecenie.IdIndeksu)
                .Include(b => b.Materialy.Jednostki)
                .ToList();

            var wynikiBOM = BOMCalculator.ObliczZapotrzebowanie(listaBOM, "1");

            BomItems.Clear();
            foreach (var item in wynikiBOM) BomItems.Add(item);
            // Uzupełniamy do 5 wierszy dla XAML (używając Twojego konstruktora)
            while (BomItems.Count < 5) BomItems.Add(new DisplayBom("", 0, "", 0));

            // 2. Faktury
            foreach (var f in Zlecenie.Faktury) Faktury.Add(f);
            while (Faktury.Count < 5) Faktury.Add(new Faktury());

            // 3. Arkusz (Plan/Wykonanie)
            foreach (var b in listaBOM)
            {
                ArkuszRozliczeniowy.Add(new ArkuszRozliczeniowyItem
                {
                    Material = b.Materialy?.Nazwa,
                    ZaplanowaneIlosc = b.Ilosc * Zlecenie.Ilosc
                });
            }
            while (ArkuszRozliczeniowy.Count < 10) ArkuszRozliczeniowy.Add(new ArkuszRozliczeniowyItem());
        }

        private void SaveAction()
        {
            db.SaveChanges();
        }
    }

    // Klasa pomocnicza tylko dla dolnej tabeli (bo tam nie masz modelu w bazie)
    public class ArkuszRozliczeniowyItem
    {
        public string Material { get; set; }
        public decimal? ZaplanowaneIlosc { get; set; }
        public decimal? ZaplanowaneKoszt { get; set; }
        public decimal? WykonaneIlosc { get; set; }
        public decimal? WykonaneKoszt { get; set; }
        public decimal? ZafakturowaneIlosc { get; set; }
        public decimal? ZafakturowaneKoszt { get; set; }
    }
}