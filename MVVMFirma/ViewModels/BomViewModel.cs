using MVVMFirma.Helper;
using MVVMFirma.Models;
using MVVMFirma.Models.Shared;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MVVMFirma.ViewModels
{
    public class BomViewModel : WorkspaceViewModel
    {
        #region Bazadanych
        private readonly MVVMFirmaEntities13 Bazadanych;
        #endregion

        #region Właściwości dla Widoku (Bindowanie z XAML)

        // 1. Obsługa pola szukania: Text="{Binding FrazaWyszukiwania}"
        private string _FrazaWyszukiwania;
        public string FrazaWyszukiwania
        {
            get => _FrazaWyszukiwania;
            set
            {
                _FrazaWyszukiwania = value;
                OnPropertyChanged(nameof(FrazaWyszukiwania));
                Filtr(); // Metoda filtrująca listę po lewej
            }
        }

        // 2. Obsługa listy po lewej: ItemsSource="{Binding ListaIndeksow}"
        private ObservableCollection<Indeksy> _ListaIndeksow;
        public ObservableCollection<Indeksy> ListaIndeksow
        {
            get => _ListaIndeksow;
            set
            {
                _ListaIndeksow = value;
                OnPropertyChanged(nameof(ListaIndeksow));
            }
        }

        // 3. Wybrany element z listy: SelectedItem="{Binding WybranyIndeks}"
        private Indeksy _WybranyIndeks;
        public Indeksy WybranyIndeks
        {
            get => _WybranyIndeks;
            set
            {
                _WybranyIndeks = value;
                OnPropertyChanged(nameof(WybranyIndeks));
                LoadStruktura(); // Ładujemy tabelę po kliknięciu w indeks
            }
        }

        // 4. Obsługa tabeli: ItemsSource="{Binding StrukturaBOM}"
        // Używamy Twojej klasy DisplayBom, ale właściwość nazywamy StrukturaBOM dla widoku
        private ObservableCollection<DisplayBom> _StrukturaBOM;
        public ObservableCollection<DisplayBom> StrukturaBOM
        {
            get => _StrukturaBOM;
            set
            {
                _StrukturaBOM = value;
                OnPropertyChanged(nameof(StrukturaBOM));
            }
        }

        #endregion

        #region Commands
        private BaseCommand _SaveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null)
                    _SaveCommand = new BaseCommand(Save);
                return _SaveCommand;
            }
        }
        #endregion

        #region Metody Logiki

        private void Load()
        {
            // Ładowanie początkowej listy indeksów do ListBoxa
            ListaIndeksow = new ObservableCollection<Indeksy>(
                Bazadanych.Indeksy.OrderBy(i => i.KodIndeksu).ToList()
            );
        }

        private void LoadStruktura()
        {
            if (WybranyIndeks == null)
            {
                StrukturaBOM = new ObservableCollection<DisplayBom>();
                return;
            }

            // Używamy Twojej metody statycznej z DisplayBom przekazując KodIndeksu
            StrukturaBOM = DisplayBom.LoadForIndex(Bazadanych, WybranyIndeks.KodIndeksu);
        }

        public void Filtr()
        {
            if (string.IsNullOrEmpty(FrazaWyszukiwania))
            {
                Load();
                return;
            }

            string fraza = FrazaWyszukiwania.ToLower();
            ListaIndeksow = new ObservableCollection<Indeksy>(
                Bazadanych.Indeksy
                .Where(x => x.KodIndeksu.ToLower().Contains(fraza) || x.KodIndeksu.ToLower().Contains(fraza))
                .ToList()
            );
        }

        private void Save()
        {
            try
            {
                Bazadanych.SaveChanges();
                CheckData.ShowSuccess("Zapisano zmiany w strukturze BOM.");
            }
            catch (System.Exception ex)
            {
                CheckData.ShowError(ex, "Błąd podczas zapisu danych.");
            }
        }

        #endregion

        #region Konstruktor
        public BomViewModel()
        {
            base.DisplayName = "Zarządzanie BOM";
            Bazadanych = new MVVMFirmaEntities13();

            _StrukturaBOM = new ObservableCollection<DisplayBom>();
            Load(); // Ładujemy listę indeksów na starcie
        }
        #endregion
    }
}