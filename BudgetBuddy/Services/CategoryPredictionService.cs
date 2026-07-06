using BudgetBuddy.Models;
using BudgetBuddy.Models.ML;
using BudgetBuddy.Services.Interfaces;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Text;

namespace BudgetBuddy.Services
{
    public class CategoryPredictionService : ICategoryPredictionService
    {
        private readonly MLContext _mlContext;
        private PredictionEngine<TransactionData, CategoryPrediction>? _predictionEngine;

        public CategoryPredictionService()
        {
            _mlContext = new MLContext(seed: 0);
        }

        public void TrainModel(IEnumerable<Transaction> historicalData)
        {
            var validData = historicalData
                .Where(t => !string.IsNullOrEmpty(t.Category) && t.Category != "Undefined")
                .Select(t => new TransactionData
                {
                    Place = t.Place,
                    Category = t.Category!
                })
                .ToList();

            if (validData.Count < 5) return;

            var trainingData = _mlContext.Data.LoadFromEnumerable(validData);

            var pipeline = _mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: nameof(TransactionData.Category), outputColumnName: "Label")
                .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: nameof(TransactionData.Place), outputColumnName: "Features"))
                .Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            var model = pipeline.Fit(trainingData);

            _predictionEngine = _mlContext.Model.CreatePredictionEngine<TransactionData, CategoryPrediction>(model);
        }

        public string? PredictCategory(string place)
        {
            if (_predictionEngine == null || string.IsNullOrWhiteSpace(place))
                return null;

            var prediction = _predictionEngine.Predict(new TransactionData { Place = place });

            return prediction.Category;
        }
    }
}
