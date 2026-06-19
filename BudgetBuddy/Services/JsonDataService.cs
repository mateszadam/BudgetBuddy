using BudgetBuddy.Models;
using BudgetBuddy.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace BudgetBuddy.Services
{
    public class JsonDataService : IDataService
    {
        public List<Transaction> Transactions { get; private set; } = new();
        public List<Transfer> Transfers { get; private set; } = new();
        public List<CategoryAlias> Categories { get; private set; } = new();

        private readonly string _transactionsFile;
        private readonly string _transfersFile;
        private readonly string _categoriesFile;

        public JsonDataService()
        {
            string docsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _transactionsFile = Path.Combine(docsPath, "expense_data.json");
            _transfersFile = Path.Combine(docsPath, "transfer_data.json");
            _categoriesFile = Path.Combine(docsPath, "category_data.json");
        }

        public void LoadData()
        {
            Transactions = ReadFile<Transaction>(_transactionsFile);
            Transfers = ReadFile<Transfer>(_transfersFile);
            Categories = ReadFile<CategoryAlias>(_categoriesFile);
            MatchCategories();
        }

        public void SaveData(DataType dataToSave = DataType.All)
        {
            if (dataToSave.HasFlag(DataType.Transactions))
            {
                WriteFile(Transactions, _transactionsFile);
            }

            if (dataToSave.HasFlag(DataType.Transfers))
            {
                WriteFile(Transfers, _transfersFile);
            }

            if (dataToSave.HasFlag(DataType.Categories))
            {
                WriteFile(Categories, _categoriesFile);
            }

        }

        private List<T> ReadFile<T>(string filePath)
        {
            if (!File.Exists(filePath)) return new List<T>();
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        private void WriteFile<T>(List<T> data, string filePath)
        {
            File.WriteAllText(filePath, JsonSerializer.Serialize(data));
        }

        public void AddTransaction(Transaction transaction)
        {
            if (!Transactions.Exists(x => x.Id == transaction.Id))
            {
                Transactions.Add(MatchSingleCategory(transaction));
            }
        }

        public void AddTransfer(Transfer transfer)
        {
            if (!Transfers.Exists(x => x.Id == transfer.Id))
            {
                Transfers.Add(transfer);
            }
        }

        public void MatchCategories()
        {
            Transactions = Transactions.Select(MatchSingleCategory).ToList();
        }

        private Transaction MatchSingleCategory(Transaction transaction)
        {
            var category = Categories.FirstOrDefault(x => x.Places.Contains(transaction.CityPlace));
            transaction.Category = category?.Type;
            return transaction;
        }
    }
}