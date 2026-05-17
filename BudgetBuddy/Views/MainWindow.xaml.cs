using BudgetBuddy.Class;
using BudgetBuddy.Classes;
using BudgetBuddy.Views.Pages;
using ExcelDataReader;
using Microsoft.Win32;
using System.ComponentModel;
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

namespace BudgetBuddy.Page
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DashboardPage DashboardPage { get; set; }
        private IncomePage IncomePage { get; set; }
        private CategoriesPage CategoriesPage { get; set; }
        private DataPage DataPage { get; set; }
        private ImportPage ImportPage { get; set; }




        public MainWindow()
        {
            InitializeComponent();
            GlobalStore.LoadDataFromJson();
            DashboardPage = new DashboardPage();
            IncomePage = new IncomePage();
            CategoriesPage = new CategoriesPage();
            DataPage = new DataPage();
            ImportPage = new ImportPage();

            MainContentArea.Content = DashboardPage;
        }

        private void Stat_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = DashboardPage;

        }
        private void Categories_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = CategoriesPage;
        }


        private void Data_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = DataPage;
        }

        private void Income_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = IncomePage;
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = ImportPage;
        }

        public void Refresh()
        {
            DashboardPage = new DashboardPage();
            IncomePage = new IncomePage();
            CategoriesPage = new CategoriesPage();
            DataPage = new DataPage();
            ImportPage = new ImportPage();
            MainContentArea.Content = DashboardPage;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalStore.Store();
            this.Close();

        }
        private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }
    }
}