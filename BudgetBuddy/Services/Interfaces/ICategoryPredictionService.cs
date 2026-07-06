using BudgetBuddy.Models;
using System.Collections.Generic;

namespace BudgetBuddy.Services.Interfaces
{
    public interface ICategoryPredictionService
    {
        void TrainModel(IEnumerable<Transaction> historicalData);
        string? PredictCategory(string description);
    }
}