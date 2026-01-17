using MVVMFirma.ViewModels;
using System.Windows.Controls;

namespace MVVMFirma.Views
{
    public partial class RozliczenieView : UserControl
    {
        public RozliczenieView()
        {
            InitializeComponent();
        }

        // TO JEST KLUCZOWE: 
        // W .NET 4.8 Widok musi otrzymać ViewModel i przypisać go do DataContext
        public RozliczenieView(RozliczenieViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel; // TO ŁĄCZY C# Z XAML
        }
    }
}