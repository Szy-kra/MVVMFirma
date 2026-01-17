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
        private readonly MVVMFirmaEntities13 Bazadanych;

        #region Properties
        private string _FrazaWyszukiwania;
        public string FrazaWyszukiwania
        {
            get => _FrazaWyszukiwania;
            set
            {
                _FrazaWyszukiwania = value;
                OnPropertyChanged(nameof(FrazaWyszukiwania));
                Filtr();
            }
        }

        private ObservableCollection<Indeksy> _ListaIndeksow;
        public ObservableCollection<Indeksy> ListaIndeksow
        {
            get => _ListaIndeksow;
            set { _ListaIndeksow = value; OnPropertyChanged(nameof(ListaIndeksow)); }
        }

        private Indeksy _WybranyIndeks;
        public Indeksy WybranyIndeks
        {
            get => _WybranyIndeks;
            set
            {
                _WybranyIndeks = value;
                OnPropertyChanged(nameof(WybranyIndeks));
                LoadStruktura();
            }
        }

        private ObservableCollection<DisplayBom> _StrukturaBOM;
        public ObservableCollection<DisplayBom> StrukturaBOM
        {
            get => _StrukturaBOM;
            set { _StrukturaBOM = value; OnPropertyChanged(nameof(StrukturaBOM)); }
        }
        #endregion

        #region Commands
        private ICommand _ShowNowyBomCommand;
        public ICommand ShowNowyBomCommand
        {
            get
            {
                if (_ShowNowyBomCommand == null)
                    _ShowNowyBomCommand = new BaseCommand(() => OtworzNowyBom());
                return _ShowNowyBomCommand;
            }
        }

        private void OtworzNowyBom()
        {
            if (WybranyIndeks != null)
            {
                // To wysyła sygnał do MainWindowViewModel
                Messenger.Send(WybranyIndeks.KodIndeksu);
            }
            else
            {
                // Jeśli nic nie zaznaczono, otwórz pusty formularz
                Messenger.Send("NowyBom");
            }
        }
        #endregion

        #region Logic
        private void Load()
        {
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
                Bazadanych.Indeksy.Where(x => x.KodIndeksu.ToLower().Contains(fraza)).ToList()
            );
        }
        #endregion

        public BomViewModel()
        {
            base.DisplayName = "Zarządzanie BOM";
            Bazadanych = new MVVMFirmaEntities13();
            _StrukturaBOM = new ObservableCollection<DisplayBom>();
            Load();
        }
    }
}