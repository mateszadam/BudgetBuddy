using System.Data;
using BudgetBuddy.Classes;
using BudgetBuddy.Models;

namespace BudgetBuddy.Class


{
    public class GlobalStore
    {
        public static List<Transaction> Transactions { get; set; } = new List<Transaction>();
        public static List<Transfer> Transfers { get; set; } = new List<Transfer>();
        public static List<Aliasess> Categories { get; set; } = new List<Aliasess>();


        public static void Store(DataType dataToSave = DataType.All)
        {
            if (dataToSave.HasFlag(DataType.Transactions))
            {
                WriteFile(Transactions, "KoltegData.json");
            }

            if (dataToSave.HasFlag(DataType.Transfers))
            {
                WriteFile(Transfers, "TransferData.json");
            }

            if (dataToSave.HasFlag(DataType.Categories))
            {
                WriteFile(Categories, "KategoriaData.json");
            }
        }



        private static void WriteFile<T>(List<T> data, string fileName)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = System.IO.Path.Combine(documentsPath, fileName);
            System.IO.File.WriteAllText(filePath, System.Text.Json.JsonSerializer.Serialize(data));
        }

        public static void LoadDataFromJson()
        {
            Transactions = ReadFile<Transaction>("KoltegData.json");
            Transfers = ReadFile<Transfer>("TransferData.json");
            Categories = ReadFile<Aliasess>("KategoriaData.json");

            MatchCategories();
        }

        private static List<T> ReadFile<T>(string fileName)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = System.IO.Path.Combine(documentsPath, fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return new List<T>();
            }
            else
            {
                string json = System.IO.File.ReadAllText(filePath);
                return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
            }
        }

        public static void MatchCategories()
        {
            Transactions = Transactions.Select(MatchCategory).ToList();
        }

        public static Transaction MatchCategory(Transaction transaction)
        {
            Aliasess? category = Categories.FirstOrDefault(x => x.Places.Contains(transaction.CityPlace));
            transaction.Category = category != null ? category.Type : null;
            return transaction;
        }


        public static void Add(Transaction transaction)
        {
            if (!Transactions.Any(x => x == transaction))
            {
                Transactions.Add(MatchCategory(transaction));
            }
        }

        public static void Add(Transfer transaction)
        {
            if (!Transfers.Any(x => x == transaction))
            {
                Transfers.Add(transaction);
            }
        }
    }

}