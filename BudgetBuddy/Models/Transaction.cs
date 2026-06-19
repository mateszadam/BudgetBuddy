using BudgetBuddy.Models;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using System.Windows.Navigation;

namespace BudgetBuddy.Models
{
        
    public class Transaction : BaseTransaction
    {
        public string Place
        {
            get;
            set {
                string data = RemoveTrailingDigits(value ?? string.Empty);
                field = data;
            }
        } = string.Empty;

        public string Card { get; set; } = "Undefined";
        public string City { get; set; } = "Undefined";
        public string? Category { get; set; }
        public string CityPlace
        {
            get => $"{City} {Place}";
        }

        public Transaction(int amount, string currency, DateTime date, string description)
        {
            Amount = amount;
            Date = date;
            Currency = currency;
            Description = description;

            string[] data = description.Split('\n');

            if (data != null && data.Length >= 4)
            {
                Card = ExtractCleanValue(data[0], "G", 0);
                City = ExtractCleanValue(data[2], "HU ", 1);
                Place = ExtractCleanValue(data[3], "   ", 0);
            }
            else
            {
                Card = "Undefined";
                City = "Undefined";
                Place = "Undefined";
            }
        }

        private string ExtractCleanValue(string rawValue, string delimiter, int partIndex)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return "Undefined";

            string parsedValue = rawValue.Contains(delimiter)
                ? rawValue.Split(delimiter , StringSplitOptions.None)[partIndex]
                : rawValue;

            parsedValue = parsedValue.Trim();

            return string.IsNullOrWhiteSpace(parsedValue) ? "Undefined" : parsedValue;
        }

        public static string RemoveTrailingDigits(string input)
        {
            return string.IsNullOrWhiteSpace(input) ?
                input : System.Text.RegularExpressions.Regex.Replace(input, @"\d+$", "").TrimEnd();
        }
    }
}
