using BudgetBuddy.Models;
using System.Collections.Generic;

namespace BudgetBuddy.Services.Interfaces
{
    [Flags]
    public enum DataType
    {
        None = 0,
        Transactions = 1,
        Transfers = 2,
        Categories = 4,
        All = Transactions | Transfers | Categories
    }

    public interface IDataService
    {
        List<Transaction> Transactions { get; }
        List<Transfer> Transfers { get; }
        List<CategoryAlias> Categories { get; }

        void LoadData();
        void SaveData(DataType dataToSave = DataType.All);
        void AddTransaction(Transaction transaction);
        void AddTransfer(Transfer transfer);
        void MatchCategories();
    }
}