using MVVMFirma.Models;
using MVVMFirma.Models.Shared;
using System.Collections.Generic;
using System.Linq;

namespace MVVMFirma.Helper
{
    public static class BOMCalculator
    {
        public static List<DisplayBom> ObliczZapotrzebowanie(IEnumerable<IndeksBOM> daneZDb, string iloscZWidoku)
        {
            decimal variableQty;
            if (!decimal.TryParse(iloscZWidoku, out variableQty) || variableQty <= 0)
            {
                variableQty = 1;
            }

            return daneZDb.Select(wiersz => new DisplayBom(
                wiersz.Materialy?.KodMaterialu ?? "Brak",
                // Teraz to są czyste decimale, więc mnożenie przejdzie bez błędów!
                wiersz.Ilosc * variableQty,
                wiersz.Materialy?.Jednostki?.KodJednostki ?? "-", //POBIERA Z BAZY KG I SZT
                wiersz.UdzialProcentowy
            )).ToList();
        }
    }
}