using MVVMFirma.Models;
using MVVMFirma.Models.Shared;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MVVMFirma.ViewModels
{
    public class ListaIndeksowViewModel : WorkspaceViewModel
    {
        #region Pola
        private readonly MVVMFirmaEntities13 _db;
        private ObservableCollection<IndeksDisplayModel> _listaIndeksow;
        #endregion

        #region Właściwości
        public ObservableCollection<IndeksDisplayModel> ListaIndeksow
        {
            get => _listaIndeksow;
            set
            {
                _listaIndeksow = value;
                OnPropertyChanged(nameof(ListaIndeksow));
            }
        }
        #endregion

        #region Konstruktor
        public ListaIndeksowViewModel()
        {
            base.DisplayName = "Lista Indeksów";
            _db = new MVVMFirmaEntities13();
            ZaladujDane();
        }
        #endregion

        #region Metody
        private void ZaladujDane()
        {
            if (_db == null) return;

            try
            {
                // Łączymy Indeksy i Parametry (Left Join)
                var dane = (from ind in _db.Indeksy
                            join par in _db.IndeksyParametry on ind.Id equals par.IdIndeksu into p
                            from param in p.DefaultIfEmpty()
                            select new IndeksDisplayModel
                            {
                                Id = ind.Id,
                                KodIndeksu = ind.KodIndeksu,
                                Rodzina = ind.Rodzina,
                                Opis = ind.Opis,
                                KodFormy = ind.KodFormy,
                                GrupaWstawki = ind.GrupaWstawki,

                                // Przekazujemy surowy bool/int z bazy danych
                                PracaAutomatyczna = ind.PracaAutomatyczna,

                                // Dane techniczne
                                WagaBrutto = param != null ? param.WagaWtryskuBrutto_g : 0,
                                Krotnosc = param != null ? param.KrotnoscFormy : 0,
                                CzasCyklu = param != null ? param.NormaCzasCyklu_s : 0
                            }).ToList();

                ListaIndeksow = new ObservableCollection<IndeksDisplayModel>(dane);
            }
            catch (Exception ex)
            {
                CheckData.ShowError(ex, "Błąd podczas pobierania połączonych danych indeksów");
            }
        }
        #endregion
    }

    /// <summary>
    /// Model pomocniczy dla listy, łączy dane z dwóch tabel
    /// </summary>
    public class IndeksDisplayModel
    {
        public int Id { get; set; }
        public string KodIndeksu { get; set; }
        public string Rodzina { get; set; }
        public string Opis { get; set; }
        public string KodFormy { get; set; }
        public string GrupaWstawki { get; set; }

        // Właściwość typu bool (lub int w zależności od mapowania EF), którą obsłuży BooleanAut
        public object PracaAutomatyczna { get; set; }

        public decimal? WagaBrutto { get; set; }
        public int? Krotnosc { get; set; }
        public int? CzasCyklu { get; set; }
    }
}