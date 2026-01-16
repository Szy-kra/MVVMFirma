using MVVMFirma.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace MVVMFirma.Helper
{
    /// <summary>
    /// Klasa wynikowa - używana w całym projekcie zamiast DisplayBom.
    /// </summary>
    public class BOMResult
    {
        public int IdBOM { get; set; }
        public string KodIndeksu { get; set; }
        public string Material { get; set; }
        public decimal Ilosc { get; set; }
        public string Jednostka { get; set; }
        public decimal Udzial { get; set; }
    }

    public static class BOMCalculator
    {
        /// <summary>
        /// TWOJA METODA (Używana w RozliczenieViewModel)
        /// Pobiera dane bezpośrednio z bazy z JOINami.
        /// </summary>
        public static ObservableCollection<BOMResult> PobierzIOblicz(MVVMFirmaEntities13 db, string kod, string iloscZWidoku)
        {
            if (db == null || string.IsNullOrEmpty(kod))
                return new ObservableCollection<BOMResult>();

            // Pobranie danych JOINem
            var query = from bom in db.IndeksBOM
                        join ind in db.Indeksy on bom.IdIndeksu equals ind.Id
                        join mat in db.Materialy on bom.IdMaterialu equals mat.Id
                        where ind.KodIndeksu == kod
                        select new BOMResult
                        {
                            IdBOM = bom.IdBOM,
                            KodIndeksu = ind.KodIndeksu,
                            Material = mat.Nazwa,
                            Ilosc = bom.Ilosc, // Baza
                            Jednostka = mat.KodJednostki,
                            Udzial = bom.UdzialProcentowy
                        };

            var lista = query.ToList();

            // Przeliczenie ilości na podstawie wpisanej wartości
            decimal mianownik = ParsujIlosc(iloscZWidoku);
            foreach (var item in lista)
            {
                item.Ilosc *= mianownik;
            }

            return new ObservableCollection<BOMResult>(lista);
        }

        /// <summary>
        /// STARA NAZWA (Naprawia błąd w NoweZlecenieViewModel)
        /// Przetwarza już pobraną listę obiektów IndeksBOM.
        /// </summary>
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

        /// <summary>
        /// Wspólna logika parsowania liczb dla .NET 8 / 4.8
        /// </summary>
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