using System;
using System.Collections.Generic;
using System.Text;

namespace BudgetBuddy.Models
{
    public class Transfer : BaseTransaction
    {
        public string Partner { get; set; } = "Undefined";
        public string Message { get; set; } = "Undefined";
        public Transfer(int amount, string currency, DateTime date, string description)
        {
            Amount = amount;
            Date = date;
            Currency = currency;
            Description = description;

            string[] data = description.Split('\n');

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
