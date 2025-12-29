using MVVMFirma.Models;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;

namespace MVVMFirma.ViewModels
{
    public class BomViewModel : WorkspaceViewModel
    {
        private MVVMFirmaEntities13 _db = new MVVMFirmaEntities13();

        private ObservableCollection<IndeksBOM> _ListaBOM;
        public ObservableCollection<IndeksBOM> ListaBOM
        {
            get => _ListaBOM;
            set
            {
                _ListaBOM = value;
                OnPropertyChanged(nameof(ListaBOM));
            }
        }

        public BomViewModel()
        {
            base.DisplayName = "Lista BOM";
            ZaladujDane();
        }

        private void ZaladujDane()
        {
            // Pobieramy dane wraz ze wszystkimi 6 powiązanymi materiałami
            var dane = _db.IndeksBOM
                .Include(b => b.Indeksy)
                .Include(b => b.Materialy)  // M1
                .Include(b => b.Materialy1) // M2
                .Include(b => b.Materialy2) // M3
                .Include(b => b.Materialy3) // M4
                .Include(b => b.Materialy4) // M5
                .Include(b => b.Materialy5) // M6
                .ToList();
            this.ListaBOM = new ObservableCollection<IndeksBOM>(dane);
        }
    }
}