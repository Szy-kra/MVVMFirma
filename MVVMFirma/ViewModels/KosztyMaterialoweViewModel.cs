using MVVMFirma.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace MVVMFirma.ViewModels
{
    public class KosztyMaterialoweViewModel : WorkspaceViewModel
    {
        private MVVMFirmaEntities13 _db = new MVVMFirmaEntities13();

        private ObservableCollection<KosztyMaterialowe> _ListaKosztow;
        public ObservableCollection<KosztyMaterialowe> ListaKosztow
        {
            get => _ListaKosztow;
            set
            {
                _ListaKosztow = value;
                OnPropertyChanged(nameof(ListaKosztow));
            }
        }

        public KosztyMaterialoweViewModel()
        {
            base.DisplayName = "Koszty materiałowe";
            ZaladujDane();
        }

        private void ZaladujDane()
        {
            // Pobieramy wszystkie rekordy z tabeli kosztów bez filtrowania kolumny Aktywne
            var dane = _db.KosztyMaterialowe
                          .OrderByDescending(k => k.DataModyfikacji)
                          .ToList();
            this.ListaKosztow = new ObservableCollection<KosztyMaterialowe>(dane);
        }
    }
}