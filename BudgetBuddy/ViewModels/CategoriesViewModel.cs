
using BudgetBuddy.Models;
using BudgetBuddy.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;

namespace BudgetBuddy.ViewModels
{
    public partial class CategoriesViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        private ObservableCollection<CategoryAlias> _categories = new();

        [ObservableProperty]
        private CategoryAlias? _selectedCategory;

        [ObservableProperty]
        private ObservableCollection<string> _selectedCategoryPlaces = new();

        [ObservableProperty]
        private string _newCategoryName = string.Empty;

        [ObservableProperty]
        private ObservableCollection<string> _availablePlaces = new();

        [ObservableProperty]
        private string? _selectedAvailablePlace;

        public CategoriesViewModel(IDataService dataService)
        {
            _dataService = dataService;
            LoadCategories();

        }

        private void LoadCategories()
        {
            Categories = new ObservableCollection<CategoryAlias>(_dataService.Categories);
        }

        partial void OnSelectedCategoryChanged(CategoryAlias? value)
        {
            UpdateSelectedCategoryPlaces();
            UpdateAvailablePlaces();
        }

        private void UpdateSelectedCategoryPlaces()
        {
            SelectedCategoryPlaces.Clear();
            if (SelectedCategory?.Places != null)
            {
                foreach (var place in SelectedCategory.Places)
                {
                    SelectedCategoryPlaces.Add(place);
                }
            }
        }

        private void UpdateAvailablePlaces()
        {
            AvailablePlaces.Clear();
            if (SelectedCategory == null) return;

            List<string> places = _dataService.Transactions
                .Where(x => string.IsNullOrEmpty(x.Category) || x.Category == "Undefined")
                .Select(x => x.CityPlace)
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            foreach (var place in places)
            {
                if (!SelectedCategory.Places.Contains(place))
                {
                    AvailablePlaces.Add(place);
                }
            }
        }

        [RelayCommand]
        private void AddCategory()
        {
            if (string.IsNullOrWhiteSpace(NewCategoryName)) return;

            if (Categories.Any(c => c.Type == NewCategoryName)) return;

            CategoryAlias newCategory = new() { Type = NewCategoryName, Places = [] };
            _dataService.Categories.Add(newCategory);
            Categories.Add(newCategory);

            _dataService.SaveData(DataType.Transactions | DataType.Categories);
            NewCategoryName = string.Empty;
        }

        [RelayCommand]
        private void DeleteCategory(CategoryAlias? category)
        {
            if (category == null) return;

            _dataService.Categories.Remove(category);
            Categories.Remove(category);
            _dataService.MatchCategories();
            _dataService.SaveData(DataType.Transactions | DataType.Categories);

            if (SelectedCategory == category)
            {
                SelectedCategory = null;
            }
        }

        [RelayCommand]
        private void AddPlace()
        {
            if (SelectedCategory == null || string.IsNullOrWhiteSpace(SelectedAvailablePlace)) return;

            if (!SelectedCategory.Places.Contains(SelectedAvailablePlace))
            {
                List<string> list = SelectedCategory.Places.ToList();
                list.Add(SelectedAvailablePlace);
                SelectedCategory.Places = list.ToArray();

                SelectedCategoryPlaces.Add(SelectedAvailablePlace);

                _dataService.MatchCategories();
                _dataService.SaveData(DataType.Categories | DataType.Transactions);

                UpdateAvailablePlaces();
                SelectedAvailablePlace = null;
            }
        }

        [RelayCommand]
        private void DeletePlace(string? place)
        {
            if (SelectedCategory == null || string.IsNullOrWhiteSpace(place)) return;

            SelectedCategory.Places = SelectedCategory.Places.Where(x => x != place).ToArray();
            SelectedCategoryPlaces.Remove(place);

            _dataService.MatchCategories();
            _dataService.SaveData(DataType.Categories | DataType.Transactions);

            UpdateAvailablePlaces();
        }
    }
}