using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;

namespace Buschmann.Windows.TestApp.Controls
{
    internal class CrawlListTesterViewModel : ViewModelBase
    {
        private List<Type> _typeList;
        private readonly ICommand _startCrawl;
        private readonly ICommand _endCrawl;
        private bool _isCrawling;

        public CrawlListTesterViewModel()
        {
            LoadTypes();

            _startCrawl = new Command(_ => IsCrawling = true);
            _endCrawl = new Command(_ => IsCrawling = false);
        }

        public ICommand StartCrawlCommand
        {
            get { return _startCrawl; }
        }

        public ICommand EndCrawlCommand
        {
            get { return _endCrawl; }
        }

        public bool IsCrawling
        {
            get { return _isCrawling; }
            set
            {
                _isCrawling = value;
                OnPropertyChanged("IsCrawling");
            }
        }

        public IEnumerable<Type> TypeList
        {
            get { return _typeList; }
            set
            {
                _typeList = new List<Type>(value);
                OnPropertyChanged("TypeList");
            }
        }

        private void LoadTypes()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(Button));
            TypeList = assembly.GetExportedTypes().Take(10);
        }
    }
}
