using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using Buschmann.Windows.Controls;

namespace Buschmann.Windows.TestApp.Controls
{
    internal class CommandTextBoxTesterViewModel : ViewModelBase
    {
        private List<Type> _typeList;
        private ObservableCollection<Type> _filteredTypeList = new ObservableCollection<Type>();
        private IEnumerable<ITextBoxCommand> _commands;

        public CommandTextBoxTesterViewModel()
        {
            LoadTypes();

            var typeFilter = new TextBoxCommand("", "Retrieves all types whose type name contains the command text.\nThis is the default command.", TypeNameFilter);
            var baseTypeFilter = new TextBoxCommand("base:", "Retrieves all types that have a base type whose name contains the command text.\nThis applies to all base types in the hierarchy.", BaseTypeFilter);
            var interfaceTypeFilter = new TextBoxCommand("interface:", "Retrieves all types that have an interface whose name contains the command text.", InterfaceFilter);
            var methodNameFilter = new TextBoxCommand("method:", "Retrieves all types that have a method whose name contains the command text.", MethodFilter);

            Commands = new List<ITextBoxCommand>() { typeFilter, baseTypeFilter, interfaceTypeFilter, methodNameFilter };
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

        public ObservableCollection<Type> FilteredTypeList
        {
            get { return _filteredTypeList; }
            private set
            {
                _filteredTypeList = value;
                OnPropertyChanged("FilteredTypeList", "FilteredTypeListCount");
            }
        }

        public int FilteredTypeListCount
        {
            get { return _filteredTypeList.Count; }
        }

        public IEnumerable<ITextBoxCommand> Commands
        {
            get { return _commands; }
            set
            {
                _commands = value;
                OnPropertyChanged("Commands");
            }
        }

        private void LoadTypes()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(Button));
            TypeList = assembly.GetExportedTypes();
        }

        private void TypeNameFilter(string commandText)
        {
            ApplyFilter(t => IsMatch(t.Name, commandText));
        }

        private void BaseTypeFilter(string commandText)
        {
            Func<Type, bool> matchFunc = t =>
                                             {
                                                 Type baseType = t.BaseType;

                                                 while (baseType != null)
                                                 {
                                                     if (IsMatch(baseType.Name, commandText))
                                                         return true;

                                                     baseType = baseType.BaseType;
                                                 }

                                                 return false;
                                             };
            ApplyFilter(matchFunc);
        }

        private void InterfaceFilter(string commandText)
        {
            Func<Type, bool> matchFunc = t =>
                                             {
                                                 if (t.IsInterface && IsMatch(t.Name, commandText))
                                                     return true;

                                                 return t.GetInterfaces().Any(i => IsMatch(i.Name, commandText));
                                             };

            ApplyFilter(matchFunc);
        }

        private void MethodFilter(string commandText)
        {
            Func<Type, bool> matchFunc = t => t.GetMethods().Any(m => IsMatch(m.Name, commandText));

            ApplyFilter(matchFunc);
        }

        private void ApplyFilter(Func<Type, bool> matchCriteria)
        {
            var matches = _typeList.Where(matchCriteria);
            FilteredTypeList = new ObservableCollection<Type>(matches);
        }

        private bool IsMatch(string name, string commandText)
        {
            if (string.IsNullOrEmpty(commandText))
                return false;

            return name.Contains(commandText);
        }
    }
}
