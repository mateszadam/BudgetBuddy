using BudgetBuddy.Classes;
using BudgetBuddy.Services.Interfaces;
using BudgetBuddy.Views.Helpers;
using BudgetBuddy.Views.Pages;
using BudgetBuddy.Views.Popups;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media;

namespace BudgetBuddy.ViewModels
{
    public class BarViewModel
    {
        public string Category { get; set; } = string.Empty;
        public string ShortCategory { get; set; } = string.Empty;
        public string AmountText { get; set; } = string.Empty;
        public double BarHeight { get; set; }
        public SolidColorBrush? FillBrush { get; set; }
    }
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Now;

        [ObservableProperty]
        private string _selectedPeriod = "Hónap";

        [ObservableProperty]
        private string _totalSpendText = "Összes költés: 0 Ft";

        public ObservableCollection<string> Periods { get; } = new() { "Hét", "Hónap" };

        public ObservableCollection<BarViewModel> ChartItems { get; } = new();

        public ObservableCollection<SelectableItem> CategoryFilters { get; } = new();
        private readonly IDataService _dataService;
        public DashboardViewModel(IDataService dataService)
        {
            _dataService = dataService;
            InitializeFilters();

            RenderChart();
        }

        private void InitializeFilters()
        {
            foreach (var category in _dataService.Categories)
            {
                var item = new SelectableItem(category.Type);
                item.PropertyChanged += Filter_PropertyChanged;
                CategoryFilters.Add(item);
            }
        }

        private void Filter_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectableItem.IsSelected))
            {
                RenderChart();
            }
        }

        partial void OnSelectedDateChanged(DateTime value) => RenderChart();
        partial void OnSelectedPeriodChanged(string value) => RenderChart();

        [RelayCommand]
        private void PreviousPeriod() => StepPeriod(-1);

        [RelayCommand]
        private void NextPeriod() => StepPeriod(1);

        [RelayCommand]
        private void ShowDetails()
        {
            var (start, end) = GetDateRange();

            var transactions = _dataService.Transactions
                .Where(t => t.Date >= start && t.Date <= end && !IsCashWithdrawal(t.Category))
                .ToList();

            var detailsWindow = new TransactionDetailsWindow(transactions);
            detailsWindow.Show();
        }

        private void StepPeriod(int direction)
        {
            SelectedDate = SelectedPeriod == "Hét"
                ? SelectedDate.AddDays(7 * direction)
                : SelectedDate.AddMonths(direction);
        }

        private (DateTime Start, DateTime End) GetDateRange()
        {
            if (SelectedPeriod == "Hét")
            {
                int diff = ((int)SelectedDate.DayOfWeek + 6) % 7;
                DateTime start = SelectedDate.AddDays(-diff).Date;
                DateTime end = start.AddDays(6).Date;
                return (start, end);
            }
            else
            {
                DateTime start = new DateTime(SelectedDate.Year, SelectedDate.Month, 1);
                DateTime end = start.AddMonths(1).AddDays(-1);
                return (start, end);
            }
        }

        private void RenderChart()
        {
            (DateTime start, DateTime end) = GetDateRange();

            int transferOutflow = _dataService.Transfers
                .Where(t => t.Date >= start && t.Date <= end && t.Amount < 0)
                .Sum(t => t.Amount);

            List<Transaction> transactions = _dataService.Transactions
                .Where(t => t.Date >= start && t.Date <= end && !IsCashWithdrawal(t.Category))
                .ToList();

            int outflow = transactions.Where(t => t.Amount < 0).Sum(t => -t.Amount);
            int inflow = transactions.Where(t => t.Amount > 0).Sum(t => t.Amount);
            int totalSpendings = outflow > 0 ? outflow : inflow;

            TotalSpendText = $"Összes költés: {FormatCurrency(totalSpendings + (-transferOutflow))} Ft";

            List<(string Category, int Total)> groupedCategories = transactions
                .GroupBy(t => t.Category ?? "Uncategorized")
                .Select(g => (Category: g.Key, Total: g.Sum(x => x.Amount)))
                .ToList();

            if (transferOutflow < 0)
            {
                groupedCategories.Add((Category: "Utalás", Total: transferOutflow));
            }

            DrawBarChart(groupedCategories.OrderByDescending(c => MathF.Abs(c.Total)).ToList());
        }

        private void DrawBarChart(List<(string Category, int Total)> groupedCategories)
        {
            ChartItems.Clear();

            if (groupedCategories.Count == 0) return;

            double maxTotal = groupedCategories.Max(c => Math.Abs(c.Total));
            if (maxTotal == 0) return;

            foreach (var item in groupedCategories)
            {
                ChartItems.Add(new BarViewModel
                {
                    Category = item.Category,
                    ShortCategory = item.Category.Length >= 8 ? item.Category.Substring(0, 8) : item.Category,
                    AmountText = $"{FormatCurrency(Math.Abs(item.Total))} Ft",
                    BarHeight = ((Math.Abs(item.Total) / maxTotal) * 220),
                    FillBrush = GetRawColor(item.Category)
                });
            }
            Console.WriteLine(ChartItems);
        }

        private bool IsCashWithdrawal(string? category)
        {
            if (string.IsNullOrWhiteSpace(category)) return false;

            return CategoryFilters
                .Where(f => !f.IsSelected)
                .Select(f => f.Name)
                .Contains(category.Trim());
        }

        private string FormatCurrency(double amount)
        {
            return amount.ToString("N0", CultureInfo.CurrentCulture);
        }

        private SolidColorBrush GetRawColor(string text)
        {
            using MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(text));
            return new SolidColorBrush(Color.FromRgb(hash[2], hash[3], hash[4]));
        }
    }
}