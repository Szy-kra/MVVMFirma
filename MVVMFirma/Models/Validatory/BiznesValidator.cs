using System.Text.RegularExpressions;

namespace MVVMFirma.Models.Validatory
{
    public class BiznesValidator : Validator
    {
        #region Walidacja dla NowyWykonawca
        public static string WalidujNazweWykonawcy(string nazwa)
        {
            if (string.IsNullOrWhiteSpace(nazwa)) return null;
            return nazwa.Length < 3 ? "Minimum 3 znaki" : null;
        }

        public static string WalidujNIP(string nip)
        {
            if (string.IsNullOrWhiteSpace(nip)) return null;
            // Sprawdza format xxx-xxx-xx-xx
            if (!Regex.IsMatch(nip, @"^\d{3}-\d{3}-\d{2}-\d{2}$"))
                return "Wymagany format: xxx-xxx-xx-xx";
            return null;
        }

        public static string WalidujKodPocztowy(string kod)
        {
            if (string.IsNullOrWhiteSpace(kod)) return null;
            // Sprawdza format xx-xxx
            if (!Regex.IsMatch(kod, @"^\d{2}-\d{3}$"))
                return "Wymagany format: xx-xxx";
            return null;
        }
        #endregion

        #region Walidacja dla NoweZlecenie
        public static string WalidujNumerZlecenia(string numer)
        {
            if (string.IsNullOrWhiteSpace(numer)) return null;
            if (!Regex.IsMatch(numer, @"^[A-Z0-9/]+$")) return "Tylko duże litery, cyfry i /";
            return null;
        }

        public static string WalidujIloscZlecona(string wartosc)
        {
            if (string.IsNullOrWhiteSpace(wartosc)) return null;
            if (!int.TryParse(wartosc, out int wynik)) return "Tylko cyfry";
            if (wynik <= 0) return "Musi być > 0";
            return null;
        }
        #endregion

        #region Walidacja dla NowyKosztMaterialowy (NOWE)
        public static string WalidujKodMaterialu(string kod)
        {
            if (string.IsNullOrWhiteSpace(kod)) return "Kod materiału jest wymagany";
            if (kod.Length < 2) return "Kod jest za krótki";
            return null;
        }

        public static string WalidujCeneMaterialu(string cena)
        {
            if (string.IsNullOrWhiteSpace(cena)) return "Cena jest wymagana";

            // Walidacja liczby zmiennoprzecinkowej dla .NET 4.8
            if (!decimal.TryParse(cena, out decimal wynik))
                return "Niepoprawny format (użyj przecinka)";

            if (wynik < 0) return "Cena nie może być ujemna";

            return null;
        }
        #endregion
    }
}