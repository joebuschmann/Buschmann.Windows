using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace Buschmann.Windows.Controls
{
    [TemplatePart(Name=InputTextBox, Type=typeof(TextBox))]
    [TemplatePart(Name=ItemsPopup, Type=typeof(Popup))]
    [TemplatePart(Name=ItemsList, Type=typeof(ListBox))]
    public class AutoCompleteTextBox : Control
    {
        private const string InputTextBox = "inputTextBox";
        private const string ItemsPopup = "itemsPopup";
        private const string ItemsList = "itemsList";

        private CollectionView _collectionView;
        private TextBox _textBox;
        private Popup _filteredItemsPopup;
        private ListBox _filteredItemsList;
        private IFilterCommand _activeFilterCommand;

        public AutoCompleteTextBox()
        {
            DefaultStyleKey = typeof(AutoCompleteTextBox);
            FilterStrategy = new DefaultFilterStrategy();
        }

        public override void OnApplyTemplate()
        {
            _textBox = Template.FindName(InputTextBox, this) as TextBox ?? new TextBox();
            _filteredItemsPopup = Template.FindName(ItemsPopup, this) as Popup ?? new Popup();
            _filteredItemsList = Template.FindName(ItemsList, this) as ListBox ?? new ListBox();

            _textBox.KeyDown += TextBoxKeyDown;
            _textBox.PreviewKeyDown += TextBoxPreviewKeyDown;
            _textBox.TextChanged += TextBoxTextChanged;
            _filteredItemsList.KeyDown += FilteredItemsListKeyDown;
            _filteredItemsList.PreviewKeyDown += FilteredItemsListPreviewKeyDown;
            _filteredItemsList.MouseDoubleClick += FilteredItemsListMouseDoubleClick;

            base.OnApplyTemplate();

            DataTemplate dataTemplate = ItemTemplate;

            if (dataTemplate != null)
                _filteredItemsList.ItemTemplate = dataTemplate;
        }

        private void FilteredItemsListKeyDown(object sender, KeyEventArgs e)
        {
            if (_filteredItemsPopup.IsOpen)
            {
                if (e.Key == Key.Enter)
                {
                    SelectCurrentItem();
                }
            }
        }

        private void SelectCurrentItem()
        {
            _filteredItemsPopup.IsOpen = false;

            if (_filteredItemsList.SelectedIndex >= 0)
            {
                IFilterStrategy filterStrategy = FilterStrategy;
                _textBox.Text = filterStrategy.GetDisplayText(_filteredItemsList.SelectedItem);
                _filteredItemsPopup.IsOpen = false;

                object selectedItem = _filteredItemsList.SelectedItem;
                SelectedItem = selectedItem;

                ICommand command = ItemSelectedCommand;

                if ((command != null) && command.CanExecute(selectedItem))
                    command.Execute(selectedItem);
            }
        }

        private void FilteredItemsListPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_filteredItemsPopup.IsOpen)
            {
                if ((e.Key == Key.Up) && (_filteredItemsList.SelectedIndex == 0))
                {
                    _textBox.Focus();
                    e.Handled = true;
                }
            }
        }

        private void FilteredItemsListMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectCurrentItem();
        }

        private void TextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_filteredItemsPopup.IsOpen)
            {
                if (e.Key == Key.Down)
                {
                    _filteredItemsList.SelectedIndex = 0;

                    ListBoxItem listBoxItem =
                        _filteredItemsList.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
                    
                    if (listBoxItem != null)
                        listBoxItem.Focus();

                    e.Handled = true;
                }
            }
        }

        private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshFilteredItems();
        }

        private void TextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && !_filteredItemsPopup.IsOpen)
                _textBox.Clear();

            if (e.Key == Key.Enter)
                _filteredItemsPopup.IsOpen = false;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Handled)
                return;

            if (_filteredItemsPopup.IsOpen)
            {
                if (e.Key == Key.Escape)
                {
                    _textBox.Focus();
                    _filteredItemsPopup.IsOpen = false;
                    e.Handled = true;
                }
            }

            base.OnKeyDown(e);
        }

        private static readonly DependencyPropertyKey SelectedItemPropertyKey =
            DependencyProperty.RegisterReadOnly("SelectedItem", typeof(object), typeof(AutoCompleteTextBox), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedItemProperty = SelectedItemPropertyKey.DependencyProperty;

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            private set { SetValue(SelectedItemPropertyKey, value); }
        }

        private static readonly DependencyProperty ItemSelectedCommandProperty =
            DependencyProperty.Register("ItemSelectedCommand", typeof (ICommand), typeof (AutoCompleteTextBox), new PropertyMetadata(null));

        public ICommand ItemSelectedCommand
        {
            get { return (ICommand)GetValue(ItemSelectedCommandProperty); }
            set { SetValue(ItemSelectedCommandProperty, value); }
        }

        private static readonly DependencyPropertyKey FilteredItemsKey =
            DependencyProperty.RegisterReadOnly("FilteredItems", typeof(IEnumerable), typeof(AutoCompleteTextBox),
                                                new PropertyMetadata(null));

        public static readonly DependencyProperty FilteredItemsProperty = FilteredItemsKey.DependencyProperty;

        public IEnumerable FilteredItems
        {
            get { return (IEnumerable)GetValue(FilteredItemsProperty); }
            private set { SetValue(FilteredItemsKey, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(AutoCompleteTextBox), new PropertyMetadata(null, ItemsSourceChanged));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteTextBox textBox = d as AutoCompleteTextBox;

            if (textBox == null)
                return;

            textBox.RefreshItemsSource();
        }

        private void RefreshItemsSource()
        {
            IEnumerable items = ItemsSource;

            _collectionView = items == null ? null : new CollectionView(items);

            if (_collectionView != null)
                _collectionView.Filter = GetMatchStrategy(FilterStrategy);

            RefreshFilteredItems();
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(AutoCompleteTextBox),
                                        new PropertyMetadata(null, ItemTemplateChanged));

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        private static void ItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteTextBox textBox = d as AutoCompleteTextBox;

            if (textBox == null)
                return;

            if (textBox._filteredItemsList != null)
                textBox._filteredItemsList.ItemTemplate = textBox.ItemTemplate;
        }

        public static readonly DependencyProperty FilterStrategyProperty =
            DependencyProperty.Register("FilterStrategy", typeof(IFilterStrategy), typeof(AutoCompleteTextBox),
                                        new PropertyMetadata(new DefaultFilterStrategy(), FilterStrategyChanged));


        public IFilterStrategy FilterStrategy
        {
            get { return (IFilterStrategy)GetValue(FilterStrategyProperty); }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("FilterStrategy");

                SetValue(FilterStrategyProperty, value);
            }
        }

        private static void FilterStrategyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteTextBox autoCompleteTextBox = d as AutoCompleteTextBox;

            if (autoCompleteTextBox == null)
                return;

            autoCompleteTextBox.RefreshFilterStrategy((IFilterStrategy)e.NewValue);
        }

        private void RefreshFilterStrategy(IFilterStrategy filterStrategy)
        {
            if (_collectionView != null)
                _collectionView.Filter = GetMatchStrategy(filterStrategy);

            RefreshFilteredItems();
        }

        public static readonly DependencyProperty FilterCommandsProperty = DependencyProperty.Register(
            "FilterCommands", typeof(List<IFilterCommand>), typeof(AutoCompleteTextBox),
            new PropertyMetadata(null, FilterCommandsChanged));

        public List<IFilterCommand> FilterCommands
        {
            get { return (List<IFilterCommand>)GetValue(FilterCommandsProperty); }
            set { SetValue(FilterCommandsProperty, value); }
        }

        private static void FilterCommandsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteTextBox autoCompleteTextBox = d as AutoCompleteTextBox;

            if (autoCompleteTextBox == null)
                return;

            var commandTextList = new Stack<IFilterCommand>((IEnumerable<IFilterCommand>)e.NewValue);

            while (commandTextList.Count != 0)
            {
                IFilterCommand filterCommand = commandTextList.Pop();

                if (commandTextList.Count(c => c.CommandText == filterCommand.CommandText) > 0)
                {
                    throw new ArgumentException(string.Format("Two filter commands with the same command text \"{0}\" were added.  The command text must be unique.", filterCommand.CommandText));
                }
            }
        }

        /// <summary>
        /// Used for unit tests.
        /// </summary>
        public void SetText(string text)
        {
            _textBox.Text = text;
        }

        /// <summary>
        /// Used for unit tests.
        /// </summary>
        public bool IsDropDownVisible
        {
            get { return _filteredItemsPopup.IsOpen; }
        }

        private void RefreshFilteredItems()
        {
            if (_collectionView == null)
                return;

            IFilterCommand filterCommand = GetMatchingFilterCommand();

            if (filterCommand != null)
            {
                if (filterCommand != _activeFilterCommand) // Set the new filter command.
                {
                    _activeFilterCommand = filterCommand;
                    _collectionView.Filter = GetMatchStrategy(filterCommand);
                }
            }
            else if (_activeFilterCommand != null)  // Remove the filter command and reset the filter strategy.
            {
                _activeFilterCommand = null;
                _collectionView.Filter = GetMatchStrategy(FilterStrategy);
            }

            var items = _collectionView.Cast<object>();
            var itemsList = new List<object>(items);

            FilteredItems = itemsList;
            _filteredItemsPopup.IsOpen = itemsList.Count > 0;
        }

        private IFilterCommand GetMatchingFilterCommand()
        {
            string text = _textBox.Text.Trim();
            IEnumerable<IFilterCommand> filterCommands = FilterCommands;

            if ((text.Length == 0) || (filterCommands == null))
                return null;

            foreach (IFilterCommand filterCommand in filterCommands)
                if (text.StartsWith(filterCommand.CommandText))
                    return filterCommand;

            return null;
        }

        private Predicate<object> GetMatchStrategy(IFilterStrategy filterStrategy)
        {
            return o =>
            {
                var text = _textBox.Text.Trim();
                return text.Length > 0 && filterStrategy.IsMatch(o, text);
            };
        }

        private Predicate<object> GetMatchStrategy(IFilterCommand filterCommand)
        {
            return o =>
            {
                var text = _textBox.Text.Replace(filterCommand.CommandText, "").Trim();

                return text.Length > 0 &&
                       filterCommand.IsMatch(o, text);
            };
        }

        public class DefaultFilterStrategy : IFilterStrategy
        {
            public virtual bool IsMatch(object item, string filterText)
            {
                return item.ToString().Contains(filterText);
            }

            public virtual string GetDisplayText(object item)
            {
                return item.ToString();
            }
        }
    }
}