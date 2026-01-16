using MVVMFirma.Helper;
using MVVMFirma.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;

namespace MVVMFirma.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Fields
        private ReadOnlyCollection<CommandViewModel> _Commands;
        private ObservableCollection<WorkspaceViewModel> _Workspaces;
        #endregion

        #region Commands

        public ReadOnlyCollection<CommandViewModel> Commands
        {
            get
            {
                if (_Commands == null)
                {
                    List<CommandViewModel> cmds = this.CreateCommands();
                    _Commands = new ReadOnlyCollection<CommandViewModel>(cmds);
                }
                return _Commands;
            }
        }

        private List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel>
    {
        // TOWARY
        new CommandViewModel("Wszystkie towary",
            new BaseCommand(() => this.ShowView<WszystkieTowaryViewModel>())),

        new CommandViewModel("Nowy towar",
            new BaseCommand(() => this.CreateView(new NowyTowarViewModel()))),

        // FAKTURY
        new CommandViewModel("Faktury",
            new BaseCommand(() => this.ShowView<FakturyViewModel>())),

        new CommandViewModel("Nowa faktura",
            new BaseCommand(() => this.CreateView(new NowaFakturaViewModel()))),

        // BOM
        new CommandViewModel("Indeks BOM",
            new BaseCommand(() => this.ShowView<BomViewModel>())),

        new CommandViewModel("Nowy BOM",
            new BaseCommand(() => this.CreateView(new NowyBomViewModel()))),

        // INDEKSY
        new CommandViewModel("Lista indeksów",
            new BaseCommand(() => this.ShowView<ListaIndeksowViewModel>())),

        new CommandViewModel("Nowy indeks",
            new BaseCommand(() => this.CreateView(new NowyIndeksViewModel()))),

        // KOSZTY OPERACYJNE
        new CommandViewModel("Koszty operacyjne",
            new BaseCommand(() => this.ShowView<KosztyOperacyjneViewModel>())),

        new CommandViewModel("Nowy koszt operacyjny",
            new BaseCommand(() => this.CreateView(new NowyKosztOperacyjnyViewModel()))),

        // KOSZTY MATERIAŁOWE
        new CommandViewModel("Koszty materiałowe",
            new BaseCommand(() => this.ShowView<KosztyMaterialoweViewModel>())),

        new CommandViewModel("Nowy koszt materiałowy",
            new BaseCommand(() => this.CreateView(new NowyKosztMaterialowyViewModel()))),

        // ZLECENIA
        new CommandViewModel("Lista zleceń",
            new BaseCommand(() => this.ShowView<ListaZlecenViewModel>())),

        // WYKONAWCY
        new CommandViewModel("Wykonawcy",
            new BaseCommand(() => this.ShowView<WykonawcyViewModel>())),

        new CommandViewModel("Nowy wykonawca",
            new BaseCommand(() => this.CreateView(new NowyWykonawcaViewModel()))),


         new CommandViewModel("Nowe Zlecenie",
               new BaseCommand(() => this.CreateView(new NoweZlecenieViewModel())))
          };
        }



        #endregion

        #region Workspaces

        public ObservableCollection<WorkspaceViewModel> Workspaces
        {
            get
            {
                if (_Workspaces == null)
                {
                    _Workspaces = new ObservableCollection<WorkspaceViewModel>();
                    _Workspaces.CollectionChanged += this.OnWorkspacesChanged;
                }
                return _Workspaces;
            }
        }
        private void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.NewItems)
                    workspace.RequestClose += this.OnWorkspaceRequestClose;

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.OldItems)
                    workspace.RequestClose -= this.OnWorkspaceRequestClose;
        }
        private void OnWorkspaceRequestClose(object sender, EventArgs e)
        {
            WorkspaceViewModel workspace = sender as WorkspaceViewModel;
            //workspace.Dispos();
            this.Workspaces.Remove(workspace);
        }

        #endregion // Workspaces

        #region Private Helpers


        private void CreateView(WorkspaceViewModel workspace)
        {

            this.Workspaces.Add(workspace);//dodajemy zakładkę do kolekcji zakładek
            this.SetActiveWorkspace(workspace);
        }

        private void ShowView<T>() where T : WorkspaceViewModel, new()
        {
            T workspace = this.Workspaces.FirstOrDefault(vm => vm is T) as T;

            if (workspace == null)
            {
                workspace = new T();
                this.Workspaces.Add(workspace);
            }

            this.SetActiveWorkspace(workspace);
        }



        // =======================
        // AKTYWACJA
        // =======================

        private void SetActiveWorkspace(WorkspaceViewModel workspace)
        {
            Debug.Assert(this.Workspaces.Contains(workspace));

            ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.Workspaces);
            if (collectionView != null)
                collectionView.MoveCurrentTo(workspace);
        }



        private void OtworzRozliczenie(Zlecenia zlecenie)
        {
            // Tworzymy nowy ViewModel rozliczenia. 
            // Jeśli nadal masz błąd CS1729, upewnij się, że w RozliczenieViewModel.cs 
            // konstruktor przyjmuje (Zlecenia zlecenie).
            var workspace = new RozliczenieViewModel(zlecenie);

            // Rzutowanie na WorkspaceViewModel naprawia błąd CS1503 (Argument 1: cannot convert)
            // pod warunkiem, że RozliczenieViewModel dziedziczy po WorkspaceViewModel.
            this.Workspaces.Add(workspace as WorkspaceViewModel);

            this.SetActiveWorkspace(workspace);
        }


        #endregion

        //== połacenie z NowyBomViewModel.cs ====
        public MainWindowViewModel()
        {
            // Twoja obecna linia (ok. 159):
            Messenger.Register<Zlecenia>(zlecenie => this.OtworzRozliczenie(zlecenie));

            // DOPISZ TĘ LINIĘ TUTAJ (ok. 161):
            Messenger.Register<string>(message => { if (message == "NowyBom") this.CreateView(new NowyBomViewModel()); });


            // Obsługa zleceń
            Messenger.Register<Zlecenia>(zlecenie => this.OtworzRozliczenie(zlecenie));

            // TO MA BYĆ TUTAJ (Odbieranie kodu z BomViewModel):
            Messenger.Register<string>(message =>
            {
                var nowyViewModel = new NowyBomViewModel();
                nowyViewModel.WybranyIndeks = message;
                this.CreateView(nowyViewModel);
            });
        }

    }
}
