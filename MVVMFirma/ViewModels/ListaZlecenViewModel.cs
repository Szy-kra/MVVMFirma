using MVVMFirma.Helper;
using MVVMFirma.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;


namespace MVVMFirma.ViewModels
{
    public class ListaZlecenViewModel : WorkspaceViewModel
    {
        #region Bazadanych
        private readonly MVVMFirmaEntities13 Bazadanych;
        private ObservableCollection<Zlecenia> _ListaZlecen;
        #endregion

        #region WybraneZlecenie
        // Ta właściwość przechowa zlecenie, które PAN kliknie w tabeli
        private Zlecenia _WybraneZlecenie;
        public Zlecenia WybraneZlecenie
        {
            get => _WybraneZlecenie;
            set
            {
                _WybraneZlecenie = value;
                OnPropertyChanged(nameof(WybraneZlecenie));
            }
        }
        #endregion

        #region Commands
        private BaseCommand _LoadCommand;
        public ICommand LoadCommand
        {
            get
            {
                if (_LoadCommand == null)
                    _LoadCommand = new BaseCommand(Load);
                return _LoadCommand;
            }
        }

        // Komenda dla przycisku "Szczegóły"
        private BaseCommand _PokazSzczegolyCommand;
        public ICommand PokazSzczegolyCommand
        {
            get
            {
                if (_PokazSzczegolyCommand == null)
                    _PokazSzczegolyCommand = new BaseCommand(PokazSzczegoly);
                return _PokazSzczegolyCommand;
            }
        }

        private void PokazSzczegoly()
        {
            if (WybraneZlecenie != null)
            {
                // Wysyłamy wybrane zlecenie do MainWindowViewModel, aby otworzyć Rozliczenie
                Messenger.Send(WybraneZlecenie);
            }
        }
        #endregion

        #region Lista
        public ObservableCollection<Zlecenia> ListaZlecen
        {
            get
            {
                if (_ListaZlecen == null)
                    Load();
                return _ListaZlecen;
            }
            set
            {
                if (_ListaZlecen != value)
                {
                    _ListaZlecen = value;
                    OnPropertyChanged(nameof(ListaZlecen));
                }
            }
        }

        private void Load()
        {
            // Odświeżamy listę z bazy danych
            ListaZlecen = new ObservableCollection<Zlecenia>(
                Bazadanych.Zlecenia.ToList()
            );
        }

        public ListaZlecenViewModel()
        {
            base.DisplayName = "Lista Zleceń";
            Bazadanych = new MVVMFirmaEntities13();
            Load();
            WybranaKolumna = DostepneKolumny.FirstOrDefault();
        }
        #endregion

        #region Filtrowanie Excel-style
        private string _FrazaFiltrowania;
        public string FrazaFiltrowania
        {
            get => _FrazaFiltrowania;
            set
            {
                _FrazaFiltrowania = value;
                Filtr();
            }
        }

        public ObservableCollection<string> DostepneKolumny { get; set; } =
            new ObservableCollection<string>
            {
                "NrZlecenia",
                "Uwagi",
                "Ilosc"
            };

        private string _WybranaKolumna;
        public string WybranaKolumna
        {
            get => _WybranaKolumna;
            set
            {
                _WybranaKolumna = value;
                OnPropertyChanged(nameof(WybranaKolumna));
                Filtr();
            }
        }

        public void Filtr()
        {
            if (string.IsNullOrEmpty(FrazaFiltrowania) || string.IsNullOrEmpty(WybranaKolumna))
            {
                Load();
                return;
            }

            string fraza = FrazaFiltrowania.ToLowerInvariant();
            var query = Bazadanych.Zlecenia.AsQueryable();

            switch (WybranaKolumna)
            {
                case "NrZlecenia":
                    query = query.Where(x => x.NrZlecenia != null && x.NrZlecenia.ToLower().Contains(fraza));
                    break;
                case "Uwagi":
                    query = query.Where(x => x.Uwagi != null && x.Uwagi.ToLower().Contains(fraza));
                    break;
                case "Ilosc":
                    query = query.Where(x => x.Ilosc.ToString().Contains(fraza));
                    break;
            }

            ListaZlecen = new ObservableCollection<Zlecenia>(query.ToList());
        }
        #endregion
    }
}