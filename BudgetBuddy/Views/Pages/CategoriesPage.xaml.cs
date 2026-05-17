using BudgetBuddy.Classes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using BudgetBuddy.Class;
using BudgetBuddy.Models;
namespace BudgetBuddy.Views.Pages
{
    /// <summary>
    /// Interaction logic for CategoriesPage.xaml
    /// </summary>
    public partial class CategoriesPage : UserControl
    {
        private Aliasess? _selectedCategory;

        public CategoriesPage()
        {
            InitializeComponent();
            RenderTypes();
        }

        private void RenderTypes()
        {
            spL.Children.Clear();
            foreach (var alias in GlobalStore.Categories)
            {
                var row = CreateRow(alias.Type, () => RenderPlaces(alias), () => DeleteType(alias));
                spL.Children.Add(row);
            }
            AddTypeInputRow();
        }

        private void AddTypeInputRow()
        {
            var grid = new Grid { Margin = new Thickness(0, 10, 0, 0) };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var txt = new TextBox { 
                Style = (Style)FindResource("ModernTextBox"), 
                Text = "New type...", 
                Foreground = Brushes.Gray 
            };

            txt.GotFocus += (s, e) => { 
                if (txt.Text == "New type...") { 
                    txt.Text = ""; 
                    txt.Foreground = Brushes.White; 
                } 
            };
            txt.LostFocus += (s, e) => {
                if (string.IsNullOrWhiteSpace(txt.Text)) {
                    txt.Text = "New type..."; 
                    txt.Foreground = Brushes.Gray; 
                } 
            };

            var btn = new Button { 
                Content = "Add Type", 
                Style = (Style)FindResource("AccentButton"), 
                Margin = new Thickness(5, 0, 0, 0)
            };

            btn.Click += (s, e) => {
                if (!string.IsNullOrWhiteSpace(txt.Text) && txt.Text != "New type...")
                {
                    GlobalStore.Categories.Add(new Aliasess { Type = txt.Text, Places = new string[0] });
                    GlobalStore.Store(DataType.Transactions | DataType.Categories); 
                    RenderTypes();
                }
            };

            Grid.SetColumn(txt, 0); 
            Grid.SetColumn(btn, 1);
            
            grid.Children.Add(txt); 
            grid.Children.Add(btn);
            
            spL.Children.Add(grid);
        }

        private void RenderPlaces(Aliasess selected)
        {
            _selectedCategory = selected;
            spR.Children.Clear();
            foreach (var place in selected.Places)
            {
                var row = CreateRow(place, null, () => DeletePlace(selected, place));
                spR.Children.Add(row);
            }
            AddPlaceInputRow(selected);
        }

        private void AddPlaceInputRow(Aliasess selected)
        {
            var grid = new Grid { Margin = new Thickness(0, 10, 0, 0) };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var cb = new ComboBox
            {
                Style = (Style)FindResource("ModernComboBox"),
                ItemsSource = GlobalStore.Transactions
                    .Where(x => x.Category == null || x.Category == "Undefined")
                    .Select(x => x.CityPlace)
                    .Distinct().OrderBy(s => s).ToList(),
                Height = 40,
                VerticalContentAlignment = VerticalAlignment.Center
            };

            var btn = new Button { Content = "Add Place", Style = (Style)FindResource("AccentButton"), Margin = new Thickness(8, 0, 0, 0), Height = 40};
            btn.Click += (s, e) => {
                if (cb.SelectedItem is string place && !selected.Places.Contains(place))
                {
                    var list = selected.Places.ToList();
                    list.Add(place);
                    selected.Places = list.ToArray();
                    GlobalStore.MatchCategories();
                    GlobalStore.Store(DataType.Categories);
                    RenderPlaces(selected);
                }
            };
            Grid.SetColumn(cb, 0);
            Grid.SetColumn(btn, 1);

            if (cb.Items.Count != 0)
            {
                grid.Children.Add(cb);
                grid.Children.Add(btn);
            }
            spR.Children.Add(grid);
        }

        private Border CreateRow(string text, Action? onSelect, Action onDelete)
        {
            var border = new Border { Style = (Style)FindResource("ModernCard") };
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var txt = new TextBlock {
                Text = text, 
                Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 102)),
                FontSize = 16,
                Cursor = Cursors.Hand
            };

            if (onSelect != null)
            {
                txt.MouseLeftButtonUp += (s, e) => onSelect();
            }

            var btn = new Button {
                Content = "✕",
                Style = (Style)FindResource("AccentButton"),
                Width = 30, 
                Cursor = Cursors.Hand
            };

            btn.Click += (s, e) => onDelete();

            Grid.SetColumn(txt, 0); 
            Grid.SetColumn(btn, 1);
            
            grid.Children.Add(txt); 
            grid.Children.Add(btn);
            
            border.Child = grid;
            return border;
        }

        private void DeleteType(Aliasess a) { 
            GlobalStore.Categories.Remove(a);
            GlobalStore.MatchCategories();
            GlobalStore.Store(DataType.Transactions | DataType.Categories); 
            RenderTypes();
            spR.Children.Clear(); 
        }
        private void DeletePlace(Aliasess a, string p) {
            a.Places = a.Places.Where(x => x != p).ToArray();
            GlobalStore.MatchCategories();
            GlobalStore.Store(DataType.Transactions | DataType.Categories); 
            RenderPlaces(a);
        }

        private void ScrollFix(object sender, MouseWheelEventArgs e) { 
            var scv = (ScrollViewer)sender; 
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta); 
            e.Handled = true;
        }
    }
}

