using System.Text.RegularExpressions;

namespace MVVMFirma.Models.Validatory
{
    public class BiznesValidator : Validator
    {
        // Walidacja dla NowyWykonawca
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

        // Walidacja dla NoweZlecenie
        public static string WalidujNumerZlecenia(string numer)
        {
            if (string.IsNullOrWhiteSpace(numer)) return null;
            if (!Regex.IsMatch(numer, @"^[A-Z0-9/]+$")) return "Tylko duże litery, cyfry i /";
            return null;
        }

        // DODANO: Walidacja ilości (rozwiązuje błąd CS0117)
        public static string WalidujIloscZlecona(string wartosc)
        {
            if (string.IsNullOrWhiteSpace(wartosc)) return null;
            if (!int.TryParse(wartosc, out int wynik)) return "Tylko cyfry";
            if (wynik <= 0) return "Musi być > 0";
            return null;
        }
    }
}