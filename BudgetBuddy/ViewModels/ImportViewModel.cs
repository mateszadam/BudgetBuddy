using BudgetBuddy.Classes;
using BudgetBuddy.Models;
using BudgetBuddy.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExcelDataReader;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;

namespace BudgetBuddy.ViewModels
{
    public record ColumnOption(int Index, string Name)
    {
        public override string ToString() => Name;
    }

    public partial class ImportViewModel : ObservableObject
    {
        private DataTable? _rawDataTable;

        [ObservableProperty]
        private string _fileNameText = "Nincs fájl kiválasztva...";

        [ObservableProperty]
        private int _headerRowIndex = 0;

        [ObservableProperty]
        private DataView? _previewData;

        [ObservableProperty]
        private bool _isConfigVisible = false;

        public ObservableCollection<ColumnOption> AvailableColumns { get; } = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ProcessDataCommand))]
        private ColumnOption? _selectedAmountColumn;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ProcessDataCommand))]
        private ColumnOption? _selectedCurrencyColumn;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ProcessDataCommand))]
        private ColumnOption? _selectedDateColumn;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ProcessDataCommand))]
        private ColumnOption? _selectedDescriptionColumn;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ProcessDataCommand))]
        private ColumnOption? _selectedTypeColumn;

        private readonly IDataService _dataService;
        private readonly IStatementParser _statementParser;
        public ImportViewModel(IDataService dataService, IStatementParser statementParser)
        {
            _dataService = dataService;
            _statementParser = statementParser;
        }

        [RelayCommand]
        private void LoadFile(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return;

            try
            {
                FileNameText = filePath;

                _rawDataTable = _statementParser.Parse(filePath);

                PreviewData = _rawDataTable.DefaultView;
                HeaderRowIndex = DetectHeaderRow(_rawDataTable) + 1;
                IsConfigVisible = true;

                RefreshColumnMappings();
            }
            catch (Exception ex)
            {
                FileNameText = $"Hiba a fájl megnyitásakor: {ex.Message}";
            }
        }

        partial void OnHeaderRowIndexChanged(int value)
        {
            if (_rawDataTable != null && value > 0 && value <= _rawDataTable.Rows.Count)
            {
                RefreshColumnMappings();
            }
        }

        private int DetectHeaderRow(DataTable dt)
        {
            for (int r = 0; r < Math.Min(dt.Rows.Count, 20); r++)
            {
                DataRow row = dt.Rows[r];
                int nonEmptyCells = 0;

                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    if (!string.IsNullOrWhiteSpace(row[c]?.ToString()))
                    {
                        nonEmptyCells++;
                    }
                }

                if (nonEmptyCells == dt.Columns.Count)
                {
                    return r;
                }
            }
            return 0;
        }

        private void RefreshColumnMappings()
        {
            if (_rawDataTable == null || HeaderRowIndex < 1) return;

            AvailableColumns.Clear();
            DataRow row = _rawDataTable.Rows[HeaderRowIndex - 1];

            for (int i = 0; i < _rawDataTable.Columns.Count; i++)
            {
                string cellValue = row[i]?.ToString() ?? $"(Üres oszlop {i})";
                AvailableColumns.Add(new ColumnOption(i, cellValue));
            }

            AutoAssignColumns();
        }

        private void AutoAssignColumns()
        {
            foreach (var option in AvailableColumns)
            {
                string lowerName = option.Name.ToLower();
                if (lowerName.Contains("dátum")) SelectedDateColumn = option;
                if (lowerName.Contains("közlemény")) SelectedDescriptionColumn = option;
                if (lowerName.Contains("összeg")) SelectedAmountColumn = option;
                if (lowerName.Contains("devizanem")) SelectedCurrencyColumn = option;
                if (lowerName.Contains("tranzakciótípus")) SelectedTypeColumn = option;
            }
        }

        private bool CanProcessData()
        {
            return SelectedAmountColumn != null &&
                   SelectedCurrencyColumn != null &&
                   SelectedDateColumn != null &&
                   SelectedDescriptionColumn != null &&
                   SelectedTypeColumn != null &&
                   _rawDataTable != null;
        }

        [RelayCommand(CanExecute = nameof(CanProcessData))]
        private void ProcessData()
        {
            if (_rawDataTable == null) return;

            int amountIndex = SelectedAmountColumn!.Index;
            int currIndex = SelectedCurrencyColumn!.Index;
            int dateIndex = SelectedDateColumn!.Index;
            int descIndex = SelectedDescriptionColumn!.Index;
            int typeIndex = SelectedTypeColumn!.Index;

            int actualHeaderIndex = HeaderRowIndex - 1;

            for (int i = actualHeaderIndex + 1; i < _rawDataTable.Rows.Count; i++)
            {
                DataRow row = _rawDataTable.Rows[i];
                var transactionType = row[typeIndex]?.ToString()?.ToUpper();

                int parsedAmount = int.TryParse(row[amountIndex]?.ToString(), out int amount) ? amount : 0;
                string parsedCurrency = row[currIndex]?.ToString() ?? string.Empty;
                DateTime parsedDate = DateTime.TryParse(row[dateIndex]?.ToString(), out DateTime date) ? date : DateTime.Now;
                string parsedDescription = row[descIndex]?.ToString() ?? string.Empty;

                if (transactionType == "KÁRTYATRANZAKCIÓ")
                {
                    var transaction = new Transaction(parsedAmount, parsedCurrency, parsedDate, parsedDescription);
                    _dataService.AddTransaction(transaction);
                }
                else if (transactionType is "ÁTUTALÁS" or "EGYÉB JÓVÁÍRÁS" or "EGYÉB TERHELÉS")
                {
                    var transfer = new Transfer(parsedAmount, parsedCurrency, parsedDate, parsedDescription);
                    _dataService.AddTransfer(transfer);
                }
            }

            _dataService.SaveData();
            _dataService.MatchCategories();

            FileNameText = "Az adatok sikeresen beimportálva.";
            IsConfigVisible = false;
            PreviewData = null;
        }
    }
}