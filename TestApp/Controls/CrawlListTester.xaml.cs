using System.Windows.Controls;

namespace Buschmann.Windows.TestApp.Controls
{
    /// <summary>
    /// Interaction logic for CrawlListTester.xaml
    /// </summary>
    public partial class CrawlListTester : UserControl
    {
        public CrawlListTester()
        {
            InitializeComponent();
            DataContext = new CrawlListTesterViewModel();
        }
    }
}
