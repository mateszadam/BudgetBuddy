using BudgetBuddy.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BudgetBuddy.ViewModels
{
    public partial class TransactionDetailsViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Transaction> _transactions = new();

        public TransactionDetailsViewModel(IEnumerable<Transaction> transactions)
        {
            Transactions = new ObservableCollection<Transaction>(transactions);
        }
    }
}