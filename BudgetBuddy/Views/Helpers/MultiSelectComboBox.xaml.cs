using BudgetBuddy.Views.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

namespace BudgetBuddy.Views.Helpers
{
    /// <summary>
    /// Interaction logic for MultiSelectComboBox.xaml
    /// </summary>
    public class SelectableItem : INotifyPropertyChanged
    {
        private bool _isSelected;
        private string _name;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }

        public SelectableItem(string name, bool isSelected = true)
        {
            Name = name;
            IsSelected = isSelected;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

public partial class MultiSelectComboBox : UserControl, INotifyPropertyChanged
        {
            public MultiSelectComboBox()
            {
                InitializeComponent();
                SelectedItems = new ObservableCollection<SelectableItem>();
                RemoveItemCommand = new RelayCommand(RemoveItem);
            }

            public static readonly RoutedEvent SelectionChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(SelectionChanged),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(MultiSelectComboBox));

            public event RoutedEventHandler SelectionChanged
            {
                add { AddHandler(SelectionChangedEvent, value); }
                remove { RemoveHandler(SelectionChangedEvent, value); }
            }

            public static readonly DependencyProperty ItemsSourceProperty =
                DependencyProperty.Register("ItemsSource", typeof(IEnumerable<SelectableItem>), typeof(MultiSelectComboBox),
                    new PropertyMetadata(null));

            public IEnumerable<SelectableItem> ItemsSource
            {
                get { return (IEnumerable<SelectableItem>)GetValue(ItemsSourceProperty); }
                set { SetValue(ItemsSourceProperty, value); }
            }

            // 2. Dependency Property for Placeholder Text
            public static readonly DependencyProperty PlaceholderProperty =
                DependencyProperty.Register("Placeholder", typeof(string), typeof(MultiSelectComboBox),
                    new PropertyMetadata("Select Options"));

            public string Placeholder
            {
                get { return (string)GetValue(PlaceholderProperty); }
                set { SetValue(PlaceholderProperty, value); }
            }

            // 3. Observable Collection for the Chips (Visuals only)
            public ObservableCollection<SelectableItem> SelectedItems { get; set; }

            // 4. Command to handle clicking the "X" on a chip
            public ICommand RemoveItemCommand { get; }

            private void RemoveItem(object parameter)
            {
                if (parameter is SelectableItem item)
                {
                    item.IsSelected = false; // This unchecks the box
                    UpdateSelectedList();    // This removes the chip
                }
            }

            // Triggered when a Checkbox in the dropdown is clicked
            private void OnItemClick(object sender, RoutedEventArgs e)
            {
                UpdateSelectedList();
            }

            // Syncs the Chips list with the Checked items
            private void UpdateSelectedList()
            {
                if (ItemsSource == null) return;

                SelectedItems.Clear();
                foreach (var item in ItemsSource.Where(i => i.IsSelected))
                {
                    SelectedItems.Add(item);
                }

                // Notify UI to update the placeholder visibility
                OnPropertyChanged(nameof(SelectedItems));
                RaiseEvent(new RoutedEventArgs(SelectionChangedEvent));
        }

            public event PropertyChangedEventHandler? PropertyChanged;
            protected void OnPropertyChanged(string name)
                => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Simple Command Helper
        public class RelayCommand : ICommand
        {
            private readonly Action<object> _execute;
            public RelayCommand(Action<object> execute) => _execute = execute;
            public bool CanExecute(object? parameter) => true;
            public void Execute(object? parameter) => _execute(parameter!);
            public event EventHandler? CanExecuteChanged;
        }

}

