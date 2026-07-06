using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace BudgetBuddy.Models.ML
{
    public class TransactionData
    {
        [LoadColumn(0)]
        public string Place { get; set; } = string.Empty;

        [LoadColumn(1)]
        public string Category { get; set; } = string.Empty;
    }
}
