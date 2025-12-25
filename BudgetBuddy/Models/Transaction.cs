using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Text;

namespace BudgetBuddy.Classes
{
    public class Base
    {
        public string Id { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }

        public string FormattedDate
        {
            get
            {
                return Date.ToString("yyyy. MM. dd.");
            }
            set
            {
            }
        }
    }
    public class Transaction : Base
    {
        public string? Place { get; set; }
        public string? Card { get; set; }
        public string? City { get; set; }
        public string? Category 
        {
            get{
                return field == null ? "Undefined" : field;
            }
            set {
                field = value;
            }
        }
        public string CityPlace
        {
            get
            {
                return $"{City} {Place}";
            }
            set { }
        }
        public Transaction()
        {
        }
        public Transaction(IExcelDataReader reader)
        {
            Amount = (int)reader.GetDouble(3);
            Date = reader.GetDateTime(0);
            Currency = reader.GetString(4);

            Description = reader.GetString(2);
            string[] data = reader.GetString(2).Split('\n');
            if (data.Length >= 4)
            {
                if (data[0].Contains('G'))
                    Card = data[0].Split('G')[0].Trim();
                else Card = data[0];

                if (data[2].Contains("HU "))
                    City = data[2].Split("HU ")[1].Trim();
                else City = data[2];

                if (data[3].Contains("   "))
                    Place = data[3].Split("   ")[0].Trim();
                else Place = data[3];
                Place = RemoveTrailingDigits(Place);
            }
        }

        public static string RemoveTrailingDigits(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;
            return System.Text.RegularExpressions.Regex.Replace(input, @"\d+$", "").TrimEnd();
        }
    }
    public class Transfer : Base
    {
        public string? Partner { get; set; }
        public string? Message { get; set; }
        public Transfer(IExcelDataReader reader)
        {
            Amount = (int)reader.GetDouble(3);
            Date = reader.GetDateTime(0);
            Currency = reader.GetString(4);

            Description = reader.GetString(2);
            string[] data = reader.GetString(2).Split('\n');
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
