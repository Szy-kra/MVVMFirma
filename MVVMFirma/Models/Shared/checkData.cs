using System;
using System.Windows;

namespace MVVMFirma.Models.Shared
{
    /// <summary>
    /// Klasa pomocnicza do wyświetlania komunikatów w aplikacji.
    /// Wszystkie metody są statyczne, więc można je wywoływać w całym projekcie.
    /// </summary>
    public static class CheckData
    {
        /// <summary>
        /// Wyświetla komunikat o sukcesie zapisu lub innej operacji.
        /// </summary>
        /// <param name="message">Treść komunikatu. Domyślnie "Zapisano zmiany".</param>
        public static void ShowSuccess(string message = "Zapisano zmiany")
        {
            MessageBox.Show(message, "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Wyświetla komunikat o błędzie z wyjątkiem.
        /// </summary>
        /// <param name="ex">Wyjątek, którego komunikat wyświetlamy.</param>
        /// <param name="prefix">Opcjonalny nagłówek komunikatu. Domyślnie "Błąd zapisu".</param>
        public static void ShowError(Exception ex, string prefix = "Błąd zapisu")
        {
            string msg = $"{prefix}:\n{ex.Message}";
            MessageBox.Show(msg, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Wyświetla prosty komunikat informacyjny.
        /// </summary>
        /// <param name="message">Treść komunikatu.</param>
        /// <param name="title">Tytuł okienka. Domyślnie "Info".</param>
        public static void ShowInfo(string message, string title = "Info")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
