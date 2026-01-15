using MVVMFirma.Helper;
using MVVMFirma.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows.Input;

namespace MVVMFirma.ViewModels
{
    public class BomViewModel : WorkspaceViewModel
    {
        // Używamy nazwy bazy z Twojego projektu
        private MVVMFirmaEntities13 _db = new MVVMFirmaEntities13();

        private string _searchText;
        private string _wybranyKodIndeksu;
        private List<IndeksBOM> _wszystkieDaneZBazy;

        // Lista kodów do ListBoxa (Lewa strona)
        private ObservableCollection<string> _listaKodow;
        public ObservableCollection<string> ListaKodow
        {
            get => _listaKodow;
            set { _listaKodow = value; OnPropertyChanged(nameof(ListaKodow)); }
        }

        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(nameof(SearchText)); FiltrujBomy(); }
        }

        // To co użytkownik kliknie na liście
        public string WybranyKodIndeksu
        {
            get => _wybranyKodIndeksu;
            set
            {
                _wybranyKodIndeksu = value;
                OnPropertyChanged(nameof(WybranyKodIndeksu));
                LadujSkladnikiDlaWybranego();
                OnPropertyChanged(nameof(IsEditorVisible));
            }
        }

        // Składniki wyświetlane w DataGrid (Prawa strona)
        private ObservableCollection<IndeksBOM> _skladnikiWybranegoBomu;
        public ObservableCollection<IndeksBOM> SkladnikiWybranegoBomu
        {
            get => _skladnikiWybranegoBomu;
            set { _skladnikiWybranegoBomu = value; OnPropertyChanged(nameof(SkladnikiWybranegoBomu)); }
        }

        public System.Windows.Visibility IsEditorVisible =>
            !string.IsNullOrEmpty(WybranyKodIndeksu) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

        public ObservableCollection<Materialy> DostepneMaterialy { get; set; }
        public ICommand CreateNewBomCommand { get; }
        public ICommand SaveBomCommand { get; }

        public BomViewModel()
        {
            // DisplayName pochodzi z BaseViewModel (upewnij się, że WorkspaceViewModel po nim dziedziczy)
            this.DisplayName = "Baza BOM";

            // Używamy BaseCommand lub RelayCommand zależnie od tego co masz w Helperach
            CreateNewBomCommand = new BaseCommand(() => OpenNowyBom());
            SaveBomCommand = new BaseCommand(() => SaveChanges());

            ZaladujDane();
        }

        public void ZaladujDane()
        {
            try
            {
                // Pobieramy dane uwzględniając relacje z tabelami Materialy i Indeksy
                _wszystkieDaneZBazy = _db.IndeksBOM
                    .Include(b => b.Materialy)
                    .Include(b => b.Indeksy)
                    .ToList();

                // Wyciągamy unikalne kody indeksów
                var kody = _wszystkieDaneZBazy
                    .Where(x => x.Indeksy != null)
                    .Select(x => x.Indeksy.KodIndeksu)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                ListaKodow = new ObservableCollection<string>(kody);
                DostepneMaterialy = new ObservableCollection<Materialy>(_db.Materialy.ToList());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Błąd ładowania: " + ex.Message);
            }
        }

        private void LadujSkladnikiDlaWybranego()
        {
            if (string.IsNullOrEmpty(WybranyKodIndeksu)) return;

            var skladniki = _wszystkieDaneZBazy
                .Where(x => x.Indeksy != null && x.Indeksy.KodIndeksu == WybranyKodIndeksu)
                .ToList();

            SkladnikiWybranegoBomu = new ObservableCollection<IndeksBOM>(skladniki);
        }

        private void FiltrujBomy()
        {
            if (_wszystkieDaneZBazy == null) return;

            var kody = _wszystkieDaneZBazy
                .Where(x => x.Indeksy != null && x.Indeksy.KodIndeksu.ToLower().Contains(SearchText.ToLower()))
                .Select(x => x.Indeksy.KodIndeksu)
                .Distinct()
                .ToList();

            ListaKodow = new ObservableCollection<string>(kody);
        }

        private void SaveChanges()
        {
            try
            {
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Błąd zapisu: " + ex.Message);
            }
        }

        private void OpenNowyBom()
        {
            // Tutaj logika otwierania nowej zakładki dla NowyBomViewModel
        }
    }
}