using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using BudgetBuddy.Classes;

namespace BudgetBuddy.Class
{
    public class GlobalStore
    {
        public static List<Transaction> Transactions { get; set {
                MatchCategories();
                field = value;
            } 
        } = new List<Transaction>() ;
        public static List<Transfer> Transfers { get; set; } = new List<Transfer>();
        public static List<Aliasess> Categories {
            get; set
            {
                MatchCategories();
                field = value;
            }
        } = new List<Aliasess>();


        public static void Store()
        {
            WriteFile(Transactions, "KoltegData.json");
            WriteFile(Transfers, "TransferData.json");
            WriteFile(Categories, "KategoriaData.json");
        }

        private static void WriteFile<T>(List<T> data, string fileName)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = System.IO.Path.Combine(documentsPath, fileName);
            var options = new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
            };
            string json = System.Text.Json.JsonSerializer.Serialize(data, options);
            System.IO.File.WriteAllText(filePath, json);
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
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
                };
                string json = System.IO.File.ReadAllText(filePath);
                return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json, options) ?? new List<T>();
            }
        }

        public static void MatchCategories()
        {
            for (int i = 0; i < Transactions.Count; i++)
            {
                Transactions[i].Place = Transaction.RemoveTrailingDigits(Transactions[i].Place);
                Aliasess? a = Categories.FirstOrDefault(x => x.Places.Contains(Transactions[i].CityPlace));
                if (a != null)
                {
                    Transactions[i].Category = a.Type;
                }
            }
        }

        public static void Add(Base transaction)
        {
            if (transaction.GetType() == typeof(Transaction))
            {
                if (Transactions.FirstOrDefault(x => x.Description.Equals(transaction.Description) && x.Date.Equals(transaction.Date)) == null)
                    Transactions.Add(transaction as Transaction);
            }
            else
            {
                if (Transfers.FirstOrDefault(x => x.Description.Equals(transaction.Description) && x.Date.Equals(transaction.Date)) == null)
                    Transfers.Add(transaction as Transfer);
            }
        }


    }
}


