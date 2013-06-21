using System.Collections.Generic;
using System.Linq;
using Buschmann.Windows.Controls;
using MbUnit.Framework;

namespace Buschmann.Windows.UnitTests
{
    [TestFixture]
    public class AutoCompleteTextBoxFixture
    {
        [Test]
        public void VerifyFilteredItems()
        {
            AutoCompleteTextBox textBox = GetAutoCompleteTextBox();
            textBox.SetText("app");

            var suggestions = textBox.FilteredItems.Cast<string>();

            Assert.AreElementsSame(new List<string> {"app", "apple", "application"}, suggestions);
            Assert.IsTrue(textBox.IsDropDownVisible);
        }

        [Test]
        public void VerifyNoFilteredItems()
        {
            AutoCompleteTextBox textBox = GetAutoCompleteTextBox();
            textBox.SetText("ruby");

            var suggestions = textBox.FilteredItems.Cast<string>();

            Assert.IsEmpty(suggestions);
            Assert.IsFalse(textBox.IsDropDownVisible);
        }

        [Test]
        public void VerifyChangeOfText()
        {
            AutoCompleteTextBox textBox = GetAutoCompleteTextBox();

            // Add text that returns no suggestions.
            textBox.SetText("ruby");

            var suggestions = textBox.FilteredItems.Cast<string>();

            Assert.IsEmpty(suggestions);
            Assert.IsFalse(textBox.IsDropDownVisible);

            // Add text that returns suggestions.
            textBox.SetText("app");

            suggestions = textBox.FilteredItems.Cast<string>();

            Assert.AreElementsSame(new List<string> { "app", "apple", "application" }, suggestions);
            Assert.IsTrue(textBox.IsDropDownVisible);

            // Add text that returns no suggestions.
            textBox.SetText("ruby");

            suggestions = textBox.FilteredItems.Cast<string>();

            Assert.IsEmpty(suggestions);
            Assert.IsFalse(textBox.IsDropDownVisible);
        }

        [Test]
        [Row(true)]
        [Row(false)]
        public void VerifyFilterStrategy(bool useCustomStrategy)
        {
            List<object> suggestions = new List<object>() {"it", "itself", "position"};
            AutoCompleteTextBox textBox = GetAutoCompleteTextBox(suggestions);

            if (useCustomStrategy)
                textBox.FilterStrategy = new StartsWithFilterStrategy();

            textBox.SetText("it");

            if (useCustomStrategy)
                Assert.AreElementsSame(new List<string>() {"it", "itself"}, textBox.FilteredItems.Cast<string>());
            else
                Assert.AreElementsSame(new List<string>() { "it", "itself", "position" }, textBox.FilteredItems.Cast<string>());
        }

        [Test]
        public void VerifyFilteredItemsRefreshAfterEmpty()
        {
            var suggestions = new List<object>() {"A", "Au", "Aut", "Auto"};
            AutoCompleteTextBox textBox = GetAutoCompleteTextBox(suggestions);

            textBox.SetText("A");
            Assert.AreElementsSame(new List<string>() { "A", "Au", "Aut", "Auto" }, textBox.FilteredItems.Cast<string>());

            textBox.SetText("Au");
            Assert.AreElementsSame(new List<string>() { "Au", "Aut", "Auto" }, textBox.FilteredItems.Cast<string>());

            textBox.SetText("Aut");
            Assert.AreElementsSame(new List<string>() { "Aut", "Auto" }, textBox.FilteredItems.Cast<string>());

            textBox.SetText("Auto");
            Assert.AreElementsSame(new List<string>() { "Auto" }, textBox.FilteredItems.Cast<string>());

            textBox.SetText("AutoM");
            Assert.AreElementsSame(new List<string>(), textBox.FilteredItems.Cast<string>());

            textBox.SetText("Auto");
            Assert.AreElementsSame(new List<string>() { "Auto" }, textBox.FilteredItems.Cast<string>());
        }

        [Test]
        [Row(true)]
        [Row(false)]
        public void VerifyEmptyTextReturnsNoFilteredItems(bool useCustomStrategy)
        {
            AutoCompleteTextBox textBox = GetAutoCompleteTextBox();

            if (useCustomStrategy)
                textBox.FilterStrategy = new StartsWithFilterStrategy();

            Assert.IsFalse(textBox.IsDropDownVisible);
            Assert.IsEmpty(textBox.FilteredItems);

            textBox.SetText("");

            Assert.IsFalse(textBox.IsDropDownVisible);
            Assert.IsEmpty(textBox.FilteredItems);
        }

        [Test]
        public void VerifyWhiteSpaceCharactersAreIgnored()
        {
            AutoCompleteTextBox textBox = GetAutoCompleteTextBox();

            textBox.SetText("   b");
            Assert.IsTrue(textBox.IsDropDownVisible);
            Assert.AreElementsSame(new List<string>() { "box" }, textBox.FilteredItems.Cast<string>());

            textBox.SetText("d    ");
            Assert.IsTrue(textBox.IsDropDownVisible);
            Assert.AreElementsSame(new List<string>() { "dog" }, textBox.FilteredItems.Cast<string>());
        }

        [Test]
        public void VerifyFilterCommandItems()
        {
            var cat = new MockItem("Cat", "animal");
            var dog = new MockItem("Dog", "animal");
            var camry = new MockItem("Camry", "car");
            var ford = new MockItem("Ford", "car");
            var cherryTree = new MockItem("Cherry Tree", "plant");
            var grass = new MockItem("Grass", "plant");

            List<object> items = new List<object>() { cat, dog, camry, ford, cherryTree, grass };

            AutoCompleteTextBox textBox = GetAutoCompleteTextBox(items);
            textBox.FilterCommands = new List<IFilterCommand>() { new AnimalFilterCommand() };

            textBox.SetText("C");
            Assert.IsTrue(textBox.IsDropDownVisible);
            Assert.AreElementsSame(new List<MockItem> { cat, camry, cherryTree }, textBox.FilteredItems.Cast<MockItem>());

            textBox.SetText("animal:C");
            Assert.IsTrue(textBox.IsDropDownVisible);
            Assert.AreElementsSame(new List<MockItem> { cat }, textBox.FilteredItems.Cast<MockItem>());

            textBox.SetText("C");
            Assert.IsTrue(textBox.IsDropDownVisible);
            Assert.AreElementsSame(new List<MockItem> { cat, camry, cherryTree }, textBox.FilteredItems.Cast<MockItem>());

            textBox.SetText("animal:C");
            Assert.IsTrue(textBox.IsDropDownVisible);
            Assert.AreElementsSame(new List<MockItem> { cat }, textBox.FilteredItems.Cast<MockItem>());
        }

        [Test]
        [ExpectedArgumentException("Two filter commands with the same command text \"animal:\" were added.  The command text must be unique.")]
        public void VerifyExceptionWhenFilterCommandAddedTwice()
        {
            AutoCompleteTextBox textBox = GetAutoCompleteTextBox();
            textBox.FilterCommands = new List<IFilterCommand>() { new AnimalFilterCommand(), new AnimalFilterCommand() };
        }

        [Test]
        public void VerifyEmptyCommandTextReturnsNoFilteredItems()
        {
            var cat = new MockItem("Cat", "animal");
            var dog = new MockItem("Dog", "animal");
            var camry = new MockItem("Camry", "car");
            var ford = new MockItem("Ford", "car");
            var cherryTree = new MockItem("Cherry Tree", "plant");
            var grass = new MockItem("Grass", "plant");

            List<object> items = new List<object>() { cat, dog, camry, ford, cherryTree, grass };

            AutoCompleteTextBox textBox = GetAutoCompleteTextBox(items);
            textBox.FilterCommands = new List<IFilterCommand>() { new AnimalFilterCommand() };

            textBox.SetText("animal:");

            Assert.IsFalse(textBox.IsDropDownVisible);
            Assert.AreElementsSame(new List<MockItem>(), textBox.FilteredItems.Cast<MockItem>());
        }

        [Test]
        public void VerifyWhiteSpaceCharactersAreIgnoredInCommandText()
        {
            var cat = new MockItem("Cat", "animal");
            var dog = new MockItem("Dog", "animal");
            var camry = new MockItem("Camry", "car");
            var ford = new MockItem("Ford", "car");
            var cherryTree = new MockItem("Cherry Tree", "plant");
            var grass = new MockItem("Grass", "plant");

            List<object> items = new List<object>() { cat, dog, camry, ford, cherryTree, grass };
            AutoCompleteTextBox textBox = GetAutoCompleteTextBox(items);
            textBox.FilterCommands = new List<IFilterCommand>() { new AnimalFilterCommand() };

            textBox.SetText("animal:  C");
            Assert.IsTrue(textBox.IsDropDownVisible);
            Assert.AreElementsSame(new List<MockItem>() { cat }, textBox.FilteredItems.Cast<MockItem>());

            textBox.SetText("");
            Assert.IsFalse(textBox.IsDropDownVisible);
            Assert.AreElementsSame(new List<MockItem>(), textBox.FilteredItems.Cast<MockItem>());

            textBox.SetText("  animal:C");
            Assert.IsTrue(textBox.IsDropDownVisible);
            Assert.AreElementsSame(new List<MockItem>() { cat }, textBox.FilteredItems.Cast<MockItem>());
        }

        private AutoCompleteTextBox GetAutoCompleteTextBox(List<object> items = null)
        {
            items = items ?? new List<object> { "app", "apple", "application", "box", "car", "dog" };

            var autoCompleteSearchbox = new AutoCompleteTextBox();
            autoCompleteSearchbox.OnApplyTemplate();
            autoCompleteSearchbox.ItemsSource = items;

            return autoCompleteSearchbox;
        }

        private class AnimalFilterCommand : IFilterCommand
        {
            public bool IsMatch(object item, string filterText)
            {
                MockItem mockItem = item as MockItem;

                if ((mockItem == null) || (mockItem.Type != "animal"))
                    return false;

                return mockItem.Name.Contains(filterText);
            }

            public string GetDisplayText(object item)
            {
                return item.ToString();
            }

            public string CommandText
            {
                get { return "animal:"; }
            }
        }

        private class MockItem
        {
            public MockItem(string name, string type)
            {
                Name = name;
                Type = type;
            }

            public string Name { get; private set; }
            public string Type { get; private set; }

            public override string ToString()
            {
                return Name;
            }
        }

        private class StartsWithFilterStrategy : IFilterStrategy
        {
            public bool IsMatch(object item, string filterText)
            {
                return item.ToString().StartsWith(filterText);
            }

            public string GetDisplayText(object item)
            {
                return item.ToString();
            }
        }
    }
}