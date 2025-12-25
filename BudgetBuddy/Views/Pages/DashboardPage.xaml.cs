using BudgetBuddy.Class;
using BudgetBuddy.Views.Helpers;
using Költég.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BudgetBuddy.Views.Pages
{

/// <summary>
/// Interaction logic for DashboardPage.xaml
/// </summary>
/// 
public class BarViewModel
    {
        public string Category { get; set; }
        public string ShortCategory { get; set; }
        public string AmountText { get; set; }
        public double BarHeight { get; set; }
        public System.Windows.Media.Brush FillBrush { get; set; }
    }

    public partial class DashboardPage : UserControl
    {

        public List<SelectableItem> MyFilters { get; set; }
        public bool _showKpLevetel { get; set; }





        public DashboardPage()
        {
            InitializeComponent();
            _showKpLevetel = true;
            MyFilters = new List<SelectableItem>();
            foreach (var item in GlobalStore.Categories)
            {
                MyFilters.Add(new SelectableItem(item.Type));
            }
            DataContext = this;
        }

        private void StepPeriod(int direction)
        {
            DateTime date = _datePicker.SelectedDate ?? DateTime.Today;
            string period = ((((ContentControl)_periodCombo.SelectedItem).Content.ToString()) ?? "Hónap");

            DateTime newDate = period == "Hét"
                ? date.AddDays(7 * direction)
                : date.AddMonths(direction);

            _datePicker.SelectedDate = newDate;
        }

        private void RenderChart()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (ChartItemsControl == null) return;

                ChartItemsControl.ItemsSource = new List<Tx>();


                DateTime date = _datePicker.SelectedDate!.Value.Date;
                string period = ((((ContentControl)_periodCombo.SelectedItem).Content.ToString()) ?? "Hónap");
             
                (DateTime start, DateTime end) = period == "Hét"
                    ? GetWeekRange(date)
                    : GetMonthRange(date);

                int total = GlobalStore.Transfers.Where(x => x.Date >= start && x.Date <= end && x.Amount < 0).Sum(x => x.Amount);
                var txs = GetTransactionsInRange(start, end);

                txs = [.. txs.Where(t => !IsKpLevetel(t.Category))];
         

                decimal outflow = txs.Where(t => t.Amount < 0m).Sum(t => -t.Amount);
                decimal inflowAsPositive = txs.Where(t => t.Amount > 0m).Sum(t => t.Amount);
                decimal totalSpendings = outflow > 0m ? outflow : inflowAsPositive;
                _totalSpendText.Text = $"Összes költés: {FormatCurrency(totalSpendings + (-total))} ft";

                var grouped = txs
                    .GroupBy(t => t.Category ?? "Uncategorized")
                    .Select(g => new CategoryTotal(g.Key, g.Sum(x => x.Amount)))
                    .ToList();


                if (total < 0)
                {
                    grouped.Add(new CategoryTotal
                    {
                        Category = "Utalás",
                        Total = total
                    });
                }


                DrawBarChart(grouped.OrderByDescending(ct => Math.Abs(ct.Total)).ToList());
            });
        }

        private void DrawBarChart(List<CategoryTotal> grouped)
        {
            if(grouped.Count == 0) { ChartItemsControl.ItemsSource = new List<BarViewModel>(); return; };
            List<BarViewModel> barList = new List<BarViewModel>();

            decimal max = grouped.Max(x => Math.Abs(x.Total));


            foreach (var item in grouped)
            {

                barList.Add(
                    new BarViewModel()
                    {
                        AmountText = FormatCurrency(Math.Abs(item.Total)) + " ft",
                        Category = item.Category,
                        BarHeight = (double)((Math.Abs(item.Total) / max) * 220),
                        FillBrush = GetRawColor(item.Category),
                        ShortCategory = item.Category.Substring(0, item.Category.Length >= 8 ? 8 : item.Category.Length)
                    }
                    );
            };

            ChartItemsControl.ItemsSource = barList;
        }
        

        private  (DateTime start, DateTime end) GetWeekRange(DateTime date)
        {
            int diff = ((int)date.DayOfWeek + 6) % 7;
            var start = date.AddDays(-diff).Date;
            var end = start.AddDays(6).Date;
            return (start, end);
        }

        private  (DateTime start, DateTime end) GetMonthRange(DateTime date)
        {
            var start = new DateTime(date.Year, date.Month, 1);
            var end = start.AddMonths(1).AddDays(-1);
            return (start, end);
        }


        private readonly record struct CategoryTotal(string Category, decimal Total);

        private record struct Tx(DateTime Date, string Category, decimal Amount);

        private List<Tx> GetTransactionsInRange(DateTime start, DateTime end)
        {
            var result = new List<Tx>();

            foreach (var t in GlobalStore.Transactions)
            {
                if (t.Date < start || t.Date > end) continue;

                result.Add(new Tx(t.Date, t.Category!, t.Amount));
            }

            return [.. result];
        }


        private string FormatCurrency(decimal amount)
        {
            return amount.ToString("N0", CultureInfo.CurrentCulture);
        }

        private bool IsKpLevetel(string? category)
        {
            if (string.IsNullOrWhiteSpace(category)) return false;
            var c = category.Trim();
            if (MyFilters.Where(x => !x.IsSelected).Select(x => x.Name).ToList().Contains(category))
                return true;
            return false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateChart_Event(object sender, SelectionChangedEventArgs e)
        {
            RenderChart();
        }

        private void PrevBtn_Click(object sender, RoutedEventArgs e)
        {
            StepPeriod(-1);
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            StepPeriod(1);
        }

        private void ToggleKpBtn_Click(object sender, RoutedEventArgs e)
        {
            _showKpLevetel = !_showKpLevetel;
            RenderChart();
        }

        private void DatePicker_Loaded(object sender, RoutedEventArgs e)
        {
            _datePicker.SelectedDate = DateTime.Now;
        }

        private void OnItemClick(object sender, RoutedEventArgs e)
        {

        }

        private void MultiSelectComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            RenderChart();
        }

        private void OnShowEachTransactionClick(object? sender, RoutedEventArgs e)
        {
            var date = _datePicker.SelectedDate!.Value.Date;
            var period = (_periodCombo.SelectedItem as string) ?? "Hónap";

            (DateTime start, DateTime end) = period == "Hét"
                ? GetWeekRange(date)
                : GetMonthRange(date);

            var transanctions = GlobalStore.Transactions.Where(x => x.Date >= start && x.Date <= end).ToList();

            transanctions = transanctions.Where(t => !IsKpLevetel(t.Category)).ToList();
            dataInPeriod page = new dataInPeriod(transanctions);
            page.Show();
        }

        public static SolidColorBrush GetRawColor(string text)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(text));

                byte r = hash[0];
                byte g = hash[1];
                byte b = hash[2];

                return new SolidColorBrush(Color.FromRgb(r, g, b));
            }
        }
    }
}