using BudgetBuddy.Models;
using BudgetBuddy.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace BudgetBuddy.ViewModels
{
    public partial class DataViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        private ObservableCollection<Transaction> _transactions = new();

        public DataViewModel(IDataService dataService)
        {
            _dataService = dataService;
            LoadTransactions();
        }

        private void LoadTransactions()
        {
            var sortedData = _dataService.Transactions
                .OrderByDescending(x => x.Date)
                .ToList();

            Transactions = new ObservableCollection<Transaction>(sortedData);
        }
    }
}