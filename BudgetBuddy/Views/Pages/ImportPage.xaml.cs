using BudgetBuddy.Class;
using BudgetBuddy.Classes;
using BudgetBuddy.Page;
using ExcelDataReader;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BudgetBuddy.Views.Pages
{
    /// <summary>
    /// Interaction logic for ImportPage.xaml
    /// </summary>
    public partial class ImportPage : UserControl
    {
        private DataTable _rawDataTable;

        public ImportPage()
        {
            InitializeComponent();

            
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx;*.xlsm"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                TxtFileName.Text = openFileDialog.FileName;
                ReadExcelFile(openFileDialog.FileName);
            }
        }

        private void ReadExcelFile(string filePath)
        {
            try
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = false
                            }
                        });

                        _rawDataTable = result.Tables[0];

                        GridPreview.ItemsSource = _rawDataTable.DefaultView;

                        int detectedRow = DetectHeaderRow(_rawDataTable) + 1;
                        TxtHeaderRow.Text = detectedRow.ToString();

                        ConfigPanel.Visibility = Visibility.Visible;
                        BtnProcess.IsEnabled = true;

                        RefreshColumnMappings(detectedRow);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening file: {ex.Message}");
            }
        }
        private int DetectHeaderRow(DataTable dt)
        {
            for (int r = 0; r < Math.Min(dt.Rows.Count, 20); r++) 
            {
                var row = dt.Rows[r];
                int nonEmptyCells = 0;

                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    if (!string.IsNullOrWhiteSpace(row[c]?.ToString()))
                        nonEmptyCells++;
                }

                if (nonEmptyCells > (dt.Columns.Count / 2))
                    return r;
            }
            return 0;
        }

        private void TxtHeaderRow_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(TxtHeaderRow.Text, out int rowIndex) && _rawDataTable != null)
            {
                if (rowIndex >= 0 && rowIndex < _rawDataTable.Rows.Count)
                {
                    RefreshColumnMappings(rowIndex);
                }
            }
        }

        private void RefreshColumnMappings(int headerRowIndex)
        {
            if (_rawDataTable == null) return;

            var columnOptions = new Dictionary<int, string>();
            var row = _rawDataTable.Rows[headerRowIndex - 1];

            for (int i = 0; i < _rawDataTable.Columns.Count; i++)
            {
                string cellValue = row[i]?.ToString() ?? $"(Empty Col {i})";
                columnOptions.Add(i, cellValue);
            }

            void BindCombo(ComboBox cmb)
            {
                cmb.ItemsSource = columnOptions;
                cmb.DisplayMemberPath = "Value";
                cmb.SelectedValuePath = "Key";
                cmb.SelectedIndex = -1;
            }

            BindCombo(CmbMapDate);
            BindCombo(CmbMapDesc);
            BindCombo(CmbMapAmount);
            BindCombo(CmbMapCurr);
            BindCombo(CmbMapType);



            foreach (var kvp in columnOptions)
            {
                if (kvp.Value.ToLower().Contains("dátum")) CmbMapDate.SelectedValue = kvp.Key;
                if (kvp.Value.ToLower().Contains("közlemény")) CmbMapDesc.SelectedValue = kvp.Key;
                if (kvp.Value.ToLower().Contains("összeg")) CmbMapAmount.SelectedValue = kvp.Key;
                if (kvp.Value.ToLower().Contains("devizanem")) CmbMapCurr.SelectedValue = kvp.Key;
                if (kvp.Value.ToLower().Contains("tranzakciótípus")) CmbMapType.SelectedValue = kvp.Key;
            }
        }

        private void BtnProcess_Click(object sender, RoutedEventArgs e)
        {

            if (CmbMapAmount.SelectedValue == null || CmbMapCurr.SelectedValue == null || CmbMapDate.SelectedValue == null || CmbMapDesc.SelectedValue == null || CmbMapType.SelectedValue == null)
            {
                MessageBox.Show("Please map all required fields.");
                return;
            }

            int amountIndex = (int)CmbMapAmount.SelectedValue;
            int currIndex = (int)CmbMapCurr.SelectedValue;
            int dateIndex = (int)CmbMapDate.SelectedValue;
            int descIndex = (int)CmbMapDesc.SelectedValue;
            int typeIndex = (int)CmbMapType.SelectedValue;


            int headerRowIndex = int.Parse(TxtHeaderRow.Text) - 1;

            List<string> results = new List<string>();

            for (int i = headerRowIndex + 1; i < _rawDataTable.Rows.Count; i++)
            {
                DataRow row = _rawDataTable.Rows[i];


                if (row[typeIndex].ToString() == "KÁRTYATRANZAKCIÓ")
                    GlobalStore.Add(new Transaction(new Base
                    {
                        Amount = int.Parse(row[amountIndex]?.ToString() ?? "0"),
                        Currency = row[currIndex].ToString() ?? "",
                        Date = DateTime.Parse(row[dateIndex].ToString() ?? DateTime.Now.ToString()),
                        Description = row[descIndex].ToString() ?? ""
                    }));
                else if (new[] { "ÁTUTALÁS", "EGYÉB JÓVÁÍRÁS", "EGYÉB TERHELÉS" }.Contains(row[typeIndex].ToString()))
                    GlobalStore.Add(new Transfer(new Base
                    {
                        Amount = int.Parse(row[amountIndex]?.ToString() ?? "0"),
                        Currency = row[currIndex].ToString() ?? "",
                        Date = DateTime.Parse(row[dateIndex].ToString() ?? DateTime.Now.ToString()),
                        Description = row[descIndex].ToString() ?? ""
                    }));
                else
                {

                    //string ss = row[descIndex].ToString();
                    //MessageBox.Show($"A következő sor feldolgozása nem sikerült: {ss}");
                    continue;
                }

            }
            GlobalStore.Store();
            GlobalStore.MatchCategories();


            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.Refresh();
            }
            else
            {
                MessageBox.Show("Az adatok sikeresen beimportálva.");
            }
        }

        private void PreviewGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
