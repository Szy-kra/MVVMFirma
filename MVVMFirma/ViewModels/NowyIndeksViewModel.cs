using MVVMFirma.Helper;
using MVVMFirma.Models;
using MVVMFirma.Models.Shared;
using System;
using System.Windows.Input;

namespace MVVMFirma.ViewModels
{
    public class NowyIndeksViewModel : WorkspaceViewModel
    {
        #region Pola
        private readonly MVVMFirmaEntities13 _db;

        private string _kodIndeksu, _rodzina, _opis, _kodFormy, _grupaWstawki, _pracaAutomatyczna;
        private string _wagaBrutto, _krotnosc, _czasCyklu;
        #endregion

        #region Właściwości
        public string KodIndeksu { get => _kodIndeksu; set { _kodIndeksu = value; OnPropertyChanged(nameof(KodIndeksu)); } }
        public string Rodzina { get => _rodzina; set { _rodzina = value; OnPropertyChanged(nameof(Rodzina)); } }
        public string Opis { get => _opis; set { _opis = value; OnPropertyChanged(nameof(Opis)); } }
        public string KodFormy { get => _kodFormy; set { _kodFormy = value; OnPropertyChanged(nameof(KodFormy)); } }
        public string GrupaWstawki { get => _grupaWstawki; set { _grupaWstawki = value; OnPropertyChanged(nameof(GrupaWstawki)); } }
        public string PracaAutomatyczna { get => _pracaAutomatyczna; set { _pracaAutomatyczna = value; OnPropertyChanged(nameof(PracaAutomatyczna)); } }

        public string WagaBrutto { get => _wagaBrutto; set { _wagaBrutto = value; OnPropertyChanged(nameof(WagaBrutto)); } }
        public string Krotnosc { get => _krotnosc; set { _krotnosc = value; OnPropertyChanged(nameof(Krotnosc)); } }
        public string CzasCyklu { get => _czasCyklu; set { _czasCyklu = value; OnPropertyChanged(nameof(CzasCyklu)); } }
        #endregion

        public ICommand SaveCommand { get; }

        public NowyIndeksViewModel()
        {
            base.DisplayName = "Nowy Indeks";
            _db = new MVVMFirmaEntities13();

            // Domyślne wartości dla UI
            PracaAutomatyczna = "TAK";
            WagaBrutto = "0.00";
            Krotnosc = "1";
            CzasCyklu = "0";

            SaveCommand = new BaseCommand(() => ZapiszIndeks());
        }

        private void ZapiszIndeks()
        {
            if (string.IsNullOrWhiteSpace(KodIndeksu))
            {
                CheckData.ShowInfo("Podaj kod indeksu!");
                return;
            }

            try
            {
                // Tworzymy obiekt główny (Indeksy)
                var nowyIndeks = new Indeksy
                {
                    KodIndeksu = this.KodIndeksu,
                    Rodzina = this.Rodzina,
                    Opis = this.Opis,
                    KodFormy = this.KodFormy,
                    GrupaWstawki = this.GrupaWstawki,

                    // POPRAWKA: Jeśli baza oczekuje bool, musimy sprawdzić czy wynik to 1
                    // BooleanAut.FromTakNie zwraca int (1 lub 0)
                    PracaAutomatyczna = BooleanAut.FromTakNie(this.PracaAutomatyczna) == 1
                };

                _db.Indeksy.Add(nowyIndeks);

                // Parsowanie liczb
                decimal.TryParse(WagaBrutto.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal waga);
                int.TryParse(Krotnosc, out int krot);
                int.TryParse(CzasCyklu, out int cykl);

                // Tworzymy parametry powiązane
                var paramy = new IndeksyParametry
                {
                    Indeksy = nowyIndeks,
                    WagaWtryskuBrutto_g = waga,
                    KrotnoscFormy = krot,
                    NormaCzasCyklu_s = cykl
                };

                _db.IndeksyParametry.Add(paramy);

                _db.SaveChanges();
                CheckData.ShowInfo($"Zapisano pomyślnie: {KodIndeksu}");
            }
            catch (Exception ex)
            {
                CheckData.ShowError(ex, "Błąd podczas zapisu indeksu i parametrów.");
            }
        }
    }
}