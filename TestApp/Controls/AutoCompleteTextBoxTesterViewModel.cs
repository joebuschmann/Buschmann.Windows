using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls;
using Buschmann.Windows.Controls;

namespace Buschmann.Windows.TestApp.Controls
{
    internal class AutoCompleteTextBoxTesterViewModel : ViewModelBase
    {
        private List<Type> _typeList;
        private readonly IFilterStrategy _filterStrategy = new TypeListFilterStrategy();
        private readonly List<IFilterCommand> _filterCommands = new List<IFilterCommand>() { new InterfaceFilterCommand(), new BaseFilterCommand() };

        public AutoCompleteTextBoxTesterViewModel()
        {
            LoadTypes();
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

        public IFilterStrategy FilterStrategy
        {
            get { return _filterStrategy; }
        }

        public List<IFilterCommand> FilterCommands
        {
            get { return _filterCommands; }
        }

        private void LoadTypes()
        {
            Assembly assembly = Assembly.GetAssembly(typeof (Button));
            TypeList = assembly.GetExportedTypes();
        }

        private class BaseFilterCommand : TypeListFilterStrategy, IFilterCommand
        {
            public override bool IsMatch(object item, string filterText)
            {
                Type type = item as Type;

                if (type == null)
                    return false;

                Type baseType = type.BaseType;

                while (baseType != null)
                {
                    if (base.IsMatch(baseType, filterText))
                        return true;

                    baseType = baseType.BaseType;
                }

                return false;
            }

            public string CommandText
            {
                get { return "base:"; }
            }
        }

        private class InterfaceFilterCommand : TypeListFilterStrategy, IFilterCommand
        {
            public override bool IsMatch(object item, string filterText)
            {
                Type type = item as Type;

                if (type == null)
                    return false;

                if (type.IsInterface && base.IsMatch(item, filterText))
                    return true;
                
                int count = type.GetInterfaces().Count(i => base.IsMatch(i, filterText));
                
                return count > 0;
            }

            public string CommandText
            {
                get { return "interface:"; }
            }
        }

        private class TypeListFilterStrategy : IFilterStrategy
        {
            public virtual bool IsMatch(object item, string filterText)
            {
                Type type = item as Type;

                if (type == null)
                    return false;

                return type.Name.Contains(filterText);
            }

            public virtual string GetDisplayText(object item)
            {
                Type type = item as Type;

                if (type == null)
                    return "";

                return type.Name;
            }
        }
    }
}