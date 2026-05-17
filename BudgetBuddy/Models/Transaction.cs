using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using System.Windows.Navigation;

namespace BudgetBuddy.Classes
{
    public class Base
    {
        public string? Id { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;

        public string FormattedDate
        {
            get => Date.ToString("yyyy. MM. dd.");
        }

        public static bool operator ==(Base A, Base B) { 
            return String.Equals(A.Description, B.Description);
        }
        public static bool operator !=(Base A, Base B)
        {
            return !String.Equals(A.Description, B.Description);
        }
    }
    public class Transaction : Base
    {
        public string Place
        {
            get;
            set => field = RemoveTrailingDigits(value ?? string.Empty);
        } = string.Empty;

        public string Card { get; set; } = "Undefined";
        public string City { get; set; } = "Undefined";
        public string? Category { get; set; }
        public string CityPlace
        {
            get => $"{City} {Place}";
        }
        public Transaction()
        {
        }
        public Transaction(Base currdata)
        {
            Amount = currdata.Amount;
            Date = currdata.Date;
            Currency = currdata.Currency;
            Description = currdata.Description ?? string.Empty;

            string[] data = currdata.Description!.Split('\n');

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
    public class Transfer : Base
    {
        public string Partner { get; set; } = "Undefined";
        public string Message { get; set; } = "Undefined";
        public Transfer(Base currdata)
        {
            Amount = currdata.Amount;
            Date = currdata.Date;
            Currency = currdata.Currency;

            Description = currdata.Description ?? string.Empty;
            string[] data = currdata.Description!.Split('\n');
            if (data.Length >= 3)
            {
                Partner = data[1];
                Message = data[2].Replace("Közlemény: ", "");
            }
        }
        public Transfer()
        {
        }
    }
}
