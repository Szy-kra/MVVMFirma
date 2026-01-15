using System.Windows.Controls;

namespace MVVMFirma.Views
{
    /// <summary>
    /// Interaction logic for RozliczenieView.xaml
    /// </summary>
    public partial class RozliczenieView : UserControl
    {
        public RozliczenieView()
        {
            InitializeComponent();
        }

        // Ta metoda już była
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        // TEJ METODY BRAKOWAŁO - to ona powoduje błąd kompilacji
        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
        }
    }
}