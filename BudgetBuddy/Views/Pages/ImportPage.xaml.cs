using BudgetBuddy.Classes;
using BudgetBuddy.ViewModels;
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
        public ImportPage()
        {
            InitializeComponent();
        }

        private void OnOpenFileClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx;*.xlsm"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                if (DataContext is ImportViewModel viewModel)
                {
                    viewModel.LoadFileCommand.Execute(openFileDialog.FileName);
                }
            }
        }
    }
}
