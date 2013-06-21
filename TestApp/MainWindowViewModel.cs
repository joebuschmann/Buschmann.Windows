using System.Windows.Controls;
using System.Windows.Input;
using Buschmann.Windows.TestApp.Controls;

namespace Buschmann.Windows.TestApp
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private readonly ICommand _controlButtonClicked;
        private string _visibleControl;

        public MainWindowViewModel()
        {
            _controlButtonClicked = new Command(p => VisibleControl = p as string);
            VisibleControl = "";
        }

        public string VisibleControl
        {
            get { return _visibleControl; }
            set
            {
                _visibleControl = value;
                OnPropertyChanged("VisibleControl");
            }
        }

        public ICommand ControlButtonClickedCommand
        {
            get { return _controlButtonClicked; }
        }
    }
}
