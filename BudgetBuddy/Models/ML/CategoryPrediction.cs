using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace BudgetBuddy.Models.ML
{
    public class CategoryPrediction
    {
        [ColumnName("PredictedLabel")]
        public string Category { get; set; } = string.Empty;

        public float[] Score { get; set; } = [];
    }
}
