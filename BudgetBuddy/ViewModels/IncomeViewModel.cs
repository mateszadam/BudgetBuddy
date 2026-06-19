using BudgetBuddy.Classes;
using BudgetBuddy.Models;
using BudgetBuddy.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BudgetBuddy.ViewModels
{
    public class IncomeItemViewModel
    {
        public string AmountText { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public partial class IncomeViewModel : ObservableObject
    {
        [ObservableProperty]
        private DateTime _fromDate;

        [ObservableProperty]
        private DateTime _toDate;

        [ObservableProperty]
        private string _totalIncomeText = "Összes bevétel: 0 Ft";

        public ObservableCollection<IncomeItemViewModel> IncomeItems { get; } = new();
        private readonly IDataService _dataService;
        public IncomeViewModel(IDataService dataService)
        {
            _dataService = dataService;


            DateTime now = DateTime.Now;
            FromDate = new DateTime(now.Year, now.Month, 1);
            ToDate = FromDate.AddMonths(1).AddDays(-1);
            LoadIncomeData();
        }

        partial void OnFromDateChanged(DateTime value) => LoadIncomeData();
        partial void OnToDateChanged(DateTime value) => LoadIncomeData();

        private void LoadIncomeData()
        {
            List<Transfer> items = _dataService.Transfers
                .Where(x => x.Date >= FromDate && x.Date <= ToDate && x.Amount > 0)
                .OrderByDescending(x => x.Date)
                .ToList();

            CultureInfo culture = CultureInfo.GetCultureInfo("hu-HU");
            int total = items.Sum(x => x.Amount);

            TotalIncomeText = $"Összes bevétel: {total.ToString("C0", culture)}";

            IncomeItems.Clear();
            foreach (var item in items)
            {
                IncomeItems.Add(new IncomeItemViewModel
                {
                    AmountText = item.Amount.ToString("C0", culture),
                    Date = item.Date,
                    Description = item.Description,
                });
            }
        }
    }
}