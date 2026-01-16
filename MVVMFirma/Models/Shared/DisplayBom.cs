using System.Collections.ObjectModel;
using System.Linq;

namespace MVVMFirma.Models.Shared
{
    public class DisplayBom
    {
        #region Właściwości
        public int IdBOM { get; set; }           // Z tabeli IndeksBOM
        public string KodIndeksu { get; set; }    // Z tabeli Indeksy
        public string Material { get; set; }     // Z tabeli Materialy (Nazwa)
        public decimal Ilosc { get; set; }       // Z tabeli IndeksBOM
        public string Jednostka { get; set; }    // Z tabeli Materialy (KodJednostki)
        public decimal Udzial { get; set; }      // Z tabeli IndeksBOM (UdzialProcentowy)
        #endregion

        #region Metody Statyczne
        public static ObservableCollection<DisplayBom> LoadForIndex(MVVMFirmaEntities13 db, string kod)
        {
            if (db == null || string.IsNullOrEmpty(kod))
                return new ObservableCollection<DisplayBom>();

            // Wykonujemy Join między IndeksBOM, Indeksy i Materialy
            var query = from bom in db.IndeksBOM
                        join ind in db.Indeksy on bom.IdIndeksu equals ind.Id
                        join mat in db.Materialy on bom.IdMaterialu equals mat.Id
                        where ind.KodIndeksu == kod
                        select new DisplayBom
                        {
                            IdBOM = bom.IdBOM,
                            KodIndeksu = ind.KodIndeksu,
                            Material = mat.Nazwa,
                            Ilosc = bom.Ilosc,
                            Jednostka = mat.KodJednostki,
                            Udzial = bom.UdzialProcentowy
                        };

            return new ObservableCollection<DisplayBom>(query.ToList());
        }
        #endregion
    }
}