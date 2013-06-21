using System.Windows.Controls;

namespace Buschmann.Windows.TestApp.Controls
{
    /// <summary>
    /// Interaction logic for AutoCompleteTextBoxTester.xaml
    /// </summary>
    public partial class AutoCompleteTextBoxTester : UserControl
    {
        public AutoCompleteTextBoxTester()
        {
            InitializeComponent();
            DataContext = new AutoCompleteTextBoxTesterViewModel();
        }
    }
}
