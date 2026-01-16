using System;
using System.Globalization;
using System.Windows.Data;

namespace MVVMFirma.Helper
{
    /// <summary>
    /// Klasa pomocnicza do obsługi zamiany formatu 1/0 (SQL) na TAK/NIE (UI)
    /// </summary>
    public class BooleanAut : IValueConverter
    {
        // Konstruktor domyślny
        public BooleanAut()
        {
        }

        #region Metody dla WPF (IValueConverter)

        // Wywoływane przez DataGrid/UI przy pobieraniu z bazy (int -> string)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ToTakNie(value);
        }

        // Wywoływane przy zapisie z UI do bazy (string -> int)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return FromTakNie(value?.ToString());
        }

        #endregion

        #region Metody Statyczne (Do wywołania w innych klasach/ViewModelach)

        /// <summary>
        /// Zamienia wartość logiczną lub liczbową na tekst TAK/NIE.
        /// Możesz to wywołać: BooleanToTakNieConverter.ToTakNie(item.PracaAutomatyczna);
        /// </summary>
        public static string ToTakNie(object value)
        {
            if (value is bool b)
                return b ? "TAK" : "NIE";

            if (value is int i)
                return i == 1 ? "TAK" : "NIE";

            if (value is string s)
            {
                if (s == "1" || s.ToLower() == "true") return "TAK";
            }

            return "NIE";
        }

        /// <summary>
        /// Zamienia tekst TAK/NIE na wartość int (1/0) gotową do zapisu w SQL.
        /// Możesz to wywołać w innej klasie: int wynik = BooleanToTakNieConverter.FromTakNie("TAK");
        /// </summary>
        public static int FromTakNie(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;

            string normalized = value.Trim().ToUpper();
            return (normalized == "TAK" || normalized == "YES" || normalized == "1") ? 1 : 0;
        }

        #endregion
    }
}