using MVVMFirma.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace MVVMFirma.Helper
{
    public class BOMResult
    {
        public int IdBOM { get; set; }
        public string KodIndeksu { get; set; }
        public string Material { get; set; }
        public decimal Ilosc { get; set; }
        public string Jednostka { get; set; }
        public decimal Udzial { get; set; }
        // DODANE POLE NA POTRZEBY CENY Z BAZY
        public decimal Cena { get; set; }
    }

    public static class BOMCalculator
    {
        /// <summary>
        /// NOWA METODA - Pobiera BOM wraz z cenami jednostkowymi z bazy Materialy.
        /// Używaj jej w RozliczenieViewModel, aby uniknąć błędów z brakiem cen.
        /// </summary>
        public static ObservableCollection<BOMResult> PobierzZKosztami(MVVMFirmaEntities13 db, string kod, string iloscZWidoku)
        {
            if (db == null || string.IsNullOrEmpty(kod))
                return new ObservableCollection<BOMResult>();

            decimal mianownik = ParsujIlosc(iloscZWidoku);

            var query = from bom in db.IndeksBOM
                        join ind in db.Indeksy on bom.IdIndeksu equals ind.Id
                        join mat in db.Materialy on bom.IdMaterialu equals mat.Id
                        where ind.KodIndeksu == kod
                        select new BOMResult
                        {
                            IdBOM = bom.IdBOM,
                            KodIndeksu = ind.KodIndeksu,
                            Material = mat.Nazwa,
                            Ilosc = bom.Ilosc * mianownik, // Całkowita ilość na zlecenie
                            Jednostka = mat.KodJednostki,
                            Udzial = bom.UdzialProcentowy,
                            Cena = mat.Cena ?? 0 // Pobranie ceny bezpośrednio z tabeli Materialy
                        };

            return new ObservableCollection<BOMResult>(query.ToList());
        }

        // --- PONIŻEJ TWOJE NIEZMIENIONE METODY ---

        public static ObservableCollection<BOMResult> PobierzIOblicz(MVVMFirmaEntities13 db, string kod, string iloscZWidoku)
        {
            if (db == null || string.IsNullOrEmpty(kod))
                return new ObservableCollection<BOMResult>();

            var query = from bom in db.IndeksBOM
                        join ind in db.Indeksy on bom.IdIndeksu equals ind.Id
                        join mat in db.Materialy on bom.IdMaterialu equals mat.Id
                        where ind.KodIndeksu == kod
                        select new BOMResult
                        {
                            IdBOM = bom.IdBOM,
                            KodIndeksu = ind.KodIndeksu,
                            Material = mat.Nazwa,
                            Ilosc = bom.Ilosc,
                            Jednostka = mat.KodJednostki,
                            Udzial = bom.UdzialProcentowy
                        };

            var lista = query.ToList();
            decimal mianownik = ParsujIlosc(iloscZWidoku);
            foreach (var item in lista)
            {
                item.Ilosc *= mianownik;
            }
            return new ObservableCollection<BOMResult>(lista);
        }

        public static List<BOMResult> ObliczZapotrzebowanie(IEnumerable<IndeksBOM> daneZDb, string iloscZWidoku)
        {
            decimal mianownik = ParsujIlosc(iloscZWidoku);

            return daneZDb.Select(wiersz => new BOMResult
            {
                IdBOM = wiersz.IdBOM,
                KodIndeksu = wiersz.Indeksy != null ? wiersz.Indeksy.KodIndeksu : (wiersz.KodIndeksu ?? ""),
                Material = wiersz.Materialy != null ? wiersz.Materialy.Nazwa : "Brak",
                Ilosc = wiersz.Ilosc * mianownik,
                Jednostka = wiersz.Materialy != null ? wiersz.Materialy.KodJednostki : "-",
                Udzial = wiersz.UdzialProcentowy
            }).ToList();
        }

        private static decimal ParsujIlosc(string ilosc)
        {
            if (string.IsNullOrEmpty(ilosc)) return 1;
            decimal wynik;
            string formatowana = ilosc.Replace(",", ".");
            if (!decimal.TryParse(formatowana, NumberStyles.Any, CultureInfo.InvariantCulture, out wynik) || wynik <= 0)
            {
                wynik = 1;
            }
            return wynik;
        }
    }
}