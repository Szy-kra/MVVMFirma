using System;

namespace MVVMFirma.Models.Validatory
{
    public class StringValidator : Validator
    {
        public static string SprawdzCzyZaczynaSieOdDuzej(string wartosc)
        {
            try
            {
                if (string.IsNullOrEmpty(wartosc))
                {
                    return "Pole nie może być puste.";
                }

                if (!char.IsUpper(wartosc[0]))
                {
                    return "Rozpocznij dużą literą.";
                }
            }
            catch (Exception)
            {
                return "Wystąpił błąd walidacji.";
            }

            return null;
        }

        // Nowa metoda sprawdzająca czy ciąg jest poprawną liczbą dodatnią
        public static string SprawdzCzyLiczba(string wartosc)
        {
            if (string.IsNullOrEmpty(wartosc)) return "Wpisz ilość.";

            if (!decimal.TryParse(wartosc, out decimal wynik))
            {
                return "Wpisz poprawną liczbę (cyfry).";
            }

            if (wynik <= 0)
            {
                return "Ilość musi być większa od 0.";
            }

            return null;
        }
    }
}