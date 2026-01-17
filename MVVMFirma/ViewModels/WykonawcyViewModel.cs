using MVVMFirma.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace MVVMFirma.ViewModels
{
    public class WykonawcyViewModel : WorkspaceViewModel
    {
        private readonly MVVMFirmaEntities13 _db;

        private ObservableCollection<object> _listaZbiorcza;
        public ObservableCollection<object> ListaZbiorcza
        {
            get => _listaZbiorcza;
            set { _listaZbiorcza = value; OnPropertyChanged(nameof(ListaZbiorcza)); }
        }

        public WykonawcyViewModel()
        {
            base.DisplayName = "Wykonawcy i Zlecenia";
            _db = new MVVMFirmaEntities13();
            LoadData();
        }

        private void LoadData()
        {
            // Pobieramy dane do pamięci .ToList(), aby uniknąć błędów translatora SQL w .NET 4.8
            var wykonawcy = _db.Wykonawcy.ToList();
            var zlecenia = _db.Zlecenia.ToList();

            // Zgodnie z Twoim SQL: 
            // JOIN Wykonawcy w ON w.Id = z.IdWykonawcy
            var query = from w in wykonawcy
                        join z in zlecenia on w.Id equals z.IdWykonawcy
                        select new
                        {
                            w.NrWykonawcy, // To jest ten numer np. 250512
                            w.Nazwa,
                            w.NIP,
                            w.Miasto,
                            w.Ulica,
                            w.NrDomu,
                            z.NrZlecenia,  // Dane ze zlecenia
                            z.DataZlecenia,
                            z.Ilosc,
                            z.Uwagi
                        };

            ListaZbiorcza = new ObservableCollection<object>(query.ToList());
        }
    }
}