using BudgetBuddy.Models.ML;
using BudgetBuddy.Services;
using BudgetBuddy.Services.Interfaces;
using BudgetBuddy.ViewModels;
using BudgetBuddy.Views.Pages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace BudgetBuddy
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            services.AddSingleton<IDataService, JsonDataService>();
            services.AddSingleton<IStatementParser, ExcelStatementParser>();
            services.AddSingleton<ICategoryPredictionService, CategoryPredictionService>();


            services.AddTransient<MainViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<DataViewModel>();
            services.AddTransient<CategoriesViewModel>();
            services.AddTransient<IncomeViewModel>();
            services.AddTransient<ImportViewModel>();

            Services = services.BuildServiceProvider();


            var dataService = Services.GetRequiredService<IDataService>();
            dataService.LoadData();

            var predictionService = Services.GetRequiredService<ICategoryPredictionService>();
            predictionService.TrainModel(dataService.Transactions);



            var mainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainViewModel>()
            };
            mainWindow.Show();
        }
    }
}