using MVVMFirma.Models;
using System;
using System.Linq;

namespace MVVMFirma.Helper
{
    public static class ZlecenieHelper
    {
        public static string GenerujNastępnyNumerZlecenia(MVVMFirmaEntities13 db)
        {
            int biezacyRok = DateTime.Now.Year;
            string suffix = $"/{biezacyRok}";

            // Szukamy ostatniego zlecenia z bieżącego roku
            var ostatniNumerZDb = db.Zlecenia
                .Where(z => z.NrZlecenia.EndsWith(suffix))
                .OrderByDescending(z => z.NrZlecenia)
                .Select(z => z.NrZlecenia)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(ostatniNumerZDb))
            {
                return $"0001{suffix}";
            }

            try
            {
                // Rozdzielamy numer (np. 0005/2026) na części
                string[] czesci = ostatniNumerZDb.Split('/');
                if (int.TryParse(czesci[0], out int numerLiczbowy))
                {
                    // Zwiększamy o 1 i formatujemy do 4 cyfr (0006)
                    return $"{(numerLiczbowy + 1):D4}{suffix}";
                }
            }
            catch
            {
                return $"0001{suffix}";
            }

            return $"0001{suffix}";
        }
    }
}