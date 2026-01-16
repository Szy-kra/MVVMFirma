using MVVMFirma.Helper;
using MVVMFirma.Models;
using MVVMFirma.Models.Shared;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows.Input;

namespace MVVMFirma.ViewModels
{
    public class RozliczenieViewModel : WorkspaceViewModel
    {
        #region Pola i Właściwości

        private readonly MVVMFirmaEntities13 db;

        private Zlecenia _zlecenie;
        public Zlecenia Zlecenie
        {
            get => _zlecenie;
            set
            {
                _zlecenie = value;
                OnPropertyChanged(nameof(Zlecenie));
            }
        }

        private ObservableCollection<BOMResult> _bomItems;
        public ObservableCollection<BOMResult> BomItems
        {
            get => _bomItems;
            set
            {
                _bomItems = value;
                OnPropertyChanged(nameof(BomItems));
            }
        }

        public ObservableCollection<Faktury> Faktury { get; set; }
        public ObservableCollection<ArkuszRozliczeniowyItem> ArkuszRozliczeniowy { get; set; }

        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                    _saveCommand = new BaseCommand(SaveAction);
                return _saveCommand;
            }
        }

        #endregion

        #region Konstruktor

        public RozliczenieViewModel(Zlecenia wybraneZlecenie) : base()
        {
            db = new MVVMFirmaEntities13();

            if (wybraneZlecenie == null) return;

            BomItems = new ObservableCollection<BOMResult>();
            Faktury = new ObservableCollection<Faktury>();
            ArkuszRozliczeniowy = new ObservableCollection<ArkuszRozliczeniowyItem>();

            // Pobranie zlecenia
            Zlecenie = db.Zlecenia
                .Include(z => z.Indeksy)
                .Include(z => z.Faktury)
                .FirstOrDefault(z => z.Id == wybraneZlecenie.Id);

            base.DisplayName = "Rozliczenie: " + (Zlecenie?.NrZlecenia ?? "");

            OdswiezDane();
        }

        #endregion

        #region Metody

        private void OdswiezDane()
        {
            if (Zlecenie?.Indeksy == null) return;

            try
            {
                // 1. BOM – Wykorzystanie BOMCalculator
                // NAPRAWA BŁĘDU: Zlecenie.Ilosc to int, więc po prostu wywołujemy ToString()
                // bez operatora '?', ponieważ int nie może być nullem.
                string iloscZleceniaStr = Zlecenie.Ilosc.ToString();

                var wynikiBOM = BOMCalculator.PobierzIOblicz(db, Zlecenie.Indeksy.KodIndeksu, iloscZleceniaStr);

                BomItems.Clear();
                foreach (var item in wynikiBOM)
                {
                    BomItems.Add(item);
                }

                // Dopełnienie do 5 wierszy (sztywny widok)
                while (BomItems.Count < 5)
                {
                    BomItems.Add(new BOMResult { KodIndeksu = "", Material = "", Ilosc = 0m, Jednostka = "", Udzial = 0m });
                }

                // 2. Faktury
                Faktury.Clear();
                if (Zlecenie.Faktury != null)
                {
                    foreach (var f in Zlecenie.Faktury) Faktury.Add(f);
                }
                while (Faktury.Count < 5) Faktury.Add(new Faktury());

                // 3. Arkusz rozliczeniowy
                ArkuszRozliczeniowy.Clear();

                // Filtrujemy tylko rzeczywiste pozycje BOM do arkusza
                foreach (var b in wynikiBOM.Where(x => !string.IsNullOrEmpty(x.Material)))
                {
                    ArkuszRozliczeniowy.Add(new ArkuszRozliczeniowyItem
                    {
                        Material = b.Material,
                        ZaplanowaneIlosc = b.Ilosc // Już przeliczone przez kalkulator
                    });
                }

                while (ArkuszRozliczeniowy.Count < 10)
                    ArkuszRozliczeniowy.Add(new ArkuszRozliczeniowyItem());
            }
            catch (Exception ex)
            {
                CheckData.ShowError(ex, "Błąd podczas odświeżania danych");
            }
        }

        private void SaveAction()
        {
            try
            {
                db.SaveChanges();
                CheckData.ShowSuccess("Zapisano zmiany.");
            }
            catch (Exception ex)
            {
                CheckData.ShowError(ex, "Błąd zapisu");
            }

            OdswiezDane();
        }

        #endregion
    }

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