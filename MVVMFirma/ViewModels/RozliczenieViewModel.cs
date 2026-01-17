using MVVMFirma.Helper;
using MVVMFirma.Models;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;

namespace MVVMFirma.ViewModels
{
    public class RozliczenieViewModel : WorkspaceViewModel
    {
        private Zlecenia _zlecenie;
        public Zlecenia Zlecenie
        {
            get => _zlecenie;
            set { _zlecenie = value; OnPropertyChanged("Zlecenie"); OnPropertyChanged("OpisDetalu"); }
        }

        // POLE: WYPRODUKOWANO (GÓRA)
        private decimal? _iloscWykonanaNaglowek;
        public decimal? IloscWykonanaNaglowek
        {
            get => _iloscWykonanaNaglowek;
            set
            {
                if (_iloscWykonanaNaglowek == value) return;
                _iloscWykonanaNaglowek = value;
                OnPropertyChanged("IloscWykonanaNaglowek");
                OnPropertyChanged("ScrapNaglowek");

                if (ArkuszRozliczeniowyLista != null)
                {
                    foreach (var wiersz in ArkuszRozliczeniowyLista)
                    {
                        // Tutaj 'value' jest nullable, więc musimy go zabezpieczyć
                        decimal iloscZNaglowka = value ?? 0m;

                        wiersz.WykonaneIlosc = value;
                        // Tutaj wiersz.CenaJednostkowa prawdopodobnie NIE jest nullable, więc usuwamy ??
                        wiersz.WykonaneKoszt = iloscZNaglowka * wiersz.CenaJednostkowa;

                        // Scrap: Wykonano (nullable) - Zafakturowano (nullable)
                        wiersz.Scrap = (wiersz.WykonaneIlosc ?? 0m) - (wiersz.ZafakturowaneIlosc ?? 0m);
                    }
                }
            }
        }

        // POLE: ZAFAKTUROWANO (GÓRA)
        private decimal? _iloscZafakturowanaNaglowek;
        public decimal? IloscZafakturowanaNaglowek
        {
            get => _iloscZafakturowanaNaglowek;
            set
            {
                if (_iloscZafakturowanaNaglowek == value) return;
                _iloscZafakturowanaNaglowek = value;
                OnPropertyChanged("IloscZafakturowanaNaglowek");
                OnPropertyChanged("ScrapNaglowek");

                if (ArkuszRozliczeniowyLista != null)
                {
                    foreach (var wiersz in ArkuszRozliczeniowyLista)
                    {
                        decimal iloscZNaglowka = value ?? 0m;

                        wiersz.ZafakturowaneIlosc = value;
                        // Usuwamy ?? przy CenaJednostkowa
                        wiersz.ZafakturowaneKoszt = iloscZNaglowka * wiersz.CenaJednostkowa;
                        wiersz.Scrap = (wiersz.WykonaneIlosc ?? 0m) - (wiersz.ZafakturowaneIlosc ?? 0m);
                    }
                }
            }
        }

        public decimal? ScrapNaglowek => (IloscWykonanaNaglowek ?? 0m) - (IloscZafakturowanaNaglowek ?? 0m);

        public string OpisDetalu => Zlecenie?.Indeksy?.Opis ?? "BRAK OPISU W BAZIE";

        public ObservableCollection<ArkuszRozliczeniowy> ArkuszRozliczeniowyLista { get; set; }
        public ObservableCollection<BOMResult> BomItems { get; set; }
        public ObservableCollection<Faktury> Faktury { get; set; }

        public RozliczenieViewModel(Zlecenia wybrane)
        {
            ArkuszRozliczeniowyLista = new ObservableCollection<ArkuszRozliczeniowy>();
            BomItems = new ObservableCollection<BOMResult>();
            Faktury = new ObservableCollection<Faktury>();

            if (wybrane != null)
            {
                this.Zlecenie = db.Zlecenia.Include("Indeksy").FirstOrDefault(z => z.Id == wybrane.Id);
                LoadData();
            }
        }

        private void LoadData()
        {
            if (Zlecenie == null || Zlecenie.Indeksy == null) return;

            var daneBOM = BOMCalculator.PobierzZKosztami(db, Zlecenie.Indeksy.KodIndeksu, "1");

            if (daneBOM != null)
            {
                foreach (var item in daneBOM)
                {
                    BomItems.Add(item);

                    // Jeśli item.Ilosc i item.Cena NIE są nullable (są zwykłymi decimal), 
                    // to po prostu je przypisujemy bez żadnych znaków zapytania.
                    ArkuszRozliczeniowyLista.Add(new ArkuszRozliczeniowy(this)
                    {
                        Material = item.Material,
                        Jednostka = item.Jednostka,
                        CenaJednostkowa = item.Cena,
                        ZaplanowaneIlosc = item.Ilosc,
                        ZaplanowaneKoszt = item.Ilosc * item.Cena
                    });
                }
            }

            var listaFaktur = db.Faktury.Where(f => f.IdZlecenia == Zlecenie.Id).Take(5).ToList();
            foreach (var f in listaFaktur) Faktury.Add(f);
            while (Faktury.Count < 5) Faktury.Add(new Faktury());
        }
    }
}