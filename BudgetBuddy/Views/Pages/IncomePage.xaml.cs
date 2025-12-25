using BudgetBuddy.Class;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BudgetBuddy.Views.Pages
{
    public class ListViewModel
    {
        public string AmountText { get; set; }
        public DateTime Date{ get; set; }
        public string Description { get; set; }
    }
    public partial class IncomePage : UserControl
    {
        public IncomePage()
        {
            InitializeComponent();

            InitializeDefaults();
            RenderChart();
        }

        private void InitializeDefaults()
        {
            var now = DateTime.Now;
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            DatePickerFrom.SelectedDateChanged -= OnDateChanged;
            DatePickerTo.SelectedDateChanged -= OnDateChanged;

            DatePickerFrom.SelectedDate = firstDayOfMonth;
            DatePickerTo.SelectedDate = lastDayOfMonth;

            DatePickerFrom.SelectedDateChanged += OnDateChanged;
            DatePickerTo.SelectedDateChanged += OnDateChanged;
        }

        private void RenderChart()
        {
            if (DatePickerFrom == null || DatePickerTo == null) return;

            var from = DatePickerFrom.SelectedDate ?? DateTime.MinValue;
            var to = DatePickerTo.SelectedDate ?? DateTime.MaxValue;

            var items = GlobalStore.Transfers
                .Where(x => x.Date >= from && x.Date <= to && x.Amount > 0)
                .OrderByDescending(x => x.Date)
                .ToList();

            var hu = CultureInfo.GetCultureInfo("hu-HU");
            var total = items.Sum(x => x.Amount);
            if(TotalLabel != null)
                TotalLabel.Content = $"Összeses bevétel: {total.ToString("C", hu)}";

            List<ListViewModel> listView = new List<ListViewModel>();

            foreach (var item in items) {
                listView.Add(new ListViewModel
                {
                    AmountText = item.Amount.ToString("C", hu),
                    Date = item.Date,
                    Description = item.Description,
                });
            }

            if(TransfersListView != null)
                TransfersListView.ItemsSource = listView;
        }

        private void OnDateChanged(object sender, SelectionChangedEventArgs e)
        {
            RenderChart();
        }

        private void OnListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView lv)
            {
                lv.UnselectAll();
            }
        }

        private void OnDatePickerLoaded(object sender, RoutedEventArgs e)
        {
        }

        private static T? FindVisualChild<T>(DependencyObject? parent) where T : DependencyObject
        {
            if (parent == null) return null;
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t) return t;
                var result = FindVisualChild<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}

