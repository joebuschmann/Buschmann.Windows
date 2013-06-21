using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Buschmann.Windows.TestApp.Controls
{
    /// <summary>
    /// Interaction logic for CommandTextBoxTester.xaml
    /// </summary>
    public partial class CommandTextBoxTester : UserControl
    {
        public CommandTextBoxTester()
        {
            InitializeComponent();
            DataContext = new CommandTextBoxTesterViewModel();
        }
    }
}
