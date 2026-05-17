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

            
            public static readonly DependencyProperty PlaceholderProperty =
                DependencyProperty.Register("Placeholder", typeof(string), typeof(MultiSelectComboBox),
                    new PropertyMetadata("Select Options"));

            public string Placeholder
            {
                get { return (string)GetValue(PlaceholderProperty); }
                set { SetValue(PlaceholderProperty, value); }
            }

            public ObservableCollection<SelectableItem> SelectedItems { get; set; }

            public ICommand RemoveItemCommand { get; }

            private void RemoveItem(object parameter)
            {
                if (parameter is SelectableItem item)
                {
                    item.IsSelected = false;
                    UpdateSelectedList(); 
                }
            }

            private void OnItemClick(object sender, RoutedEventArgs e)
            {
                UpdateSelectedList();
            }

            private void UpdateSelectedList()
            {
                if (ItemsSource == null) return;

                SelectedItems.Clear();
                foreach (var item in ItemsSource.Where(i => i.IsSelected))
                {
                    SelectedItems.Add(item);
                }

                OnPropertyChanged(nameof(SelectedItems));
                RaiseEvent(new RoutedEventArgs(SelectionChangedEvent));
        }

            public event PropertyChangedEventHandler? PropertyChanged;
            protected void OnPropertyChanged(string name)
                => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public class RelayCommand : ICommand
        {
            private readonly Action<object> _execute;
            public RelayCommand(Action<object> execute) => _execute = execute;
            public bool CanExecute(object? parameter) => true;
            public void Execute(object? parameter) => _execute(parameter!);
            public event EventHandler? CanExecuteChanged;
        }

}

