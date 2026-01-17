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
        // Twoje istniejące komendy
        new CommandViewModel("BOM", new BaseCommand(() => this.ShowView<BomViewModel>())),
        new CommandViewModel("Nowy BOM", new BaseCommand(() => this.CreateView(new NowyBomViewModel()))),
        new CommandViewModel("Lista zleceń", new BaseCommand(() => this.ShowView<ListaZlecenViewModel>())),
        new CommandViewModel("Nowe Zlecenie", new BaseCommand(() => this.CreateView(new NoweZlecenieViewModel()))),
        
        // --- NOWE KOMENDY DLA MATERIAŁÓW ---
       
        new CommandViewModel("Nowy materiał", new BaseCommand(() => this.CreateView(new NowyMaterialViewModel()))),
        // ------------------------------------

        new CommandViewModel("Lista indeksów", new BaseCommand(() => this.ShowView<ListaIndeksowViewModel>())),
        new CommandViewModel("Nowy indeks", new BaseCommand(() => this.CreateView(new NowyIndeksViewModel()))),
        new CommandViewModel("Koszty materiałowe", new BaseCommand(() => this.ShowView<KosztyMaterialoweViewModel>())),
        new CommandViewModel("Nowy koszt materiałowy", new BaseCommand(() => this.CreateView(new NowyKosztMaterialowyViewModel()))),
        new CommandViewModel("Wykonawcy", new BaseCommand(() => this.ShowView<WykonawcyViewModel>())),
        new CommandViewModel("Nowy wykonawca", new BaseCommand(() => this.CreateView(new NowyWykonawcaViewModel())))
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
            this.Workspaces.Remove(workspace);
        }
        #endregion

        #region Helpers
        private void CreateView(WorkspaceViewModel workspace)
        {
            this.Workspaces.Add(workspace);
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

        private void SetActiveWorkspace(WorkspaceViewModel workspace)
        {
            Debug.Assert(this.Workspaces.Contains(workspace));
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.Workspaces);
            if (collectionView != null)
                collectionView.MoveCurrentTo(workspace);
        }

        private void OtworzRozliczenie(Zlecenia zlecenie)
        {
            var workspace = new RozliczenieViewModel(zlecenie);
            this.Workspaces.Add(workspace);
            this.SetActiveWorkspace(workspace);
        }
        #endregion

        public MainWindowViewModel()
        {
            // Obsługa zleceń
            Messenger.Register<Zlecenia>(zlecenie => this.OtworzRozliczenie(zlecenie));

            // JEDNA rejestracja dla stringów - obsługuje menu i przesyłanie indeksu z BOM
            Messenger.Register<string>(message =>
            {
                if (message == "NowyBom")
                {
                    this.CreateView(new NowyBomViewModel());
                }
                else
                {
                    // To jest kluczowe dla Twojego GitHubowego flow: 
                    // Jeśli przyjdzie kod indeksu, otwórz NowyBom i przypisz mu ten kod.
                    var nowyViewModel = new NowyBomViewModel();
                    nowyViewModel.WybranyIndeks = message;
                    this.CreateView(nowyViewModel);
                }
            });
        }
    }
}