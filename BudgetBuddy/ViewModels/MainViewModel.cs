using BudgetBuddy.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace BudgetBuddy.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        private readonly DashboardViewModel _dashboardViewModel;
        private readonly DataViewModel _dataViewModel;
        private readonly CategoriesViewModel _categoriesViewModel;
        private readonly IncomeViewModel _incomeViewModel;
        private readonly ImportViewModel _importViewModel;

        [ObservableProperty]
        private ObservableObject _currentViewModel;

        public MainViewModel(
            IDataService dataService,
            DashboardViewModel dashboardViewModel,
            DataViewModel dataViewModel,
            CategoriesViewModel categoriesViewModel,
            IncomeViewModel incomeViewModel,
            ImportViewModel importViewModel)
        {
            _dataService = dataService;

            _dashboardViewModel = dashboardViewModel;
            _dataViewModel = dataViewModel;
            _categoriesViewModel = categoriesViewModel;
            _incomeViewModel = incomeViewModel;
            _importViewModel = importViewModel;

            _currentViewModel = _dashboardViewModel;
        }

        [RelayCommand]
        private void NavigateToDashboard() => CurrentViewModel = _dashboardViewModel;

        [RelayCommand]
        private void NavigateToData() => CurrentViewModel = _dataViewModel;

        [RelayCommand]
        private void NavigateToCategories() => CurrentViewModel = _categoriesViewModel;

        [RelayCommand]
        private void NavigateToIncome() => CurrentViewModel = _incomeViewModel;

        [RelayCommand]
        private void NavigateToImport() => CurrentViewModel = _importViewModel;

        [RelayCommand]
        private void CloseApplication()
        {
            _dataService.SaveData();
            Application.Current.Shutdown();
        }
    }
}