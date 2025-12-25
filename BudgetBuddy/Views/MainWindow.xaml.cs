using BudgetBuddy.Class;
using BudgetBuddy.Classes;
using BudgetBuddy.Views.Pages;
using ExcelDataReader;
using Microsoft.Win32;
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

        public MainWindow()
        {
            InitializeComponent();
            GlobalStore.LoadDataFromJson();
            DashboardPage = new DashboardPage();
            IncomePage = new IncomePage();



        }

        private void Stat_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = DashboardPage;

        }
        private void Categories_Click(object sender, RoutedEventArgs e)
        {

        }


        private void Data_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.AddExtension = true;
                ofd.Multiselect = false;
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\Phone Link";
                ofd.Filter = "Excel Files (*.xls;*.xlsx)|*.xls;*.xlsx|All Files (*.*)|*.*";
                var result = ofd.ShowDialog();

                if (result == true)
                {
                    string selectedFile = ofd.FileName;
                    DataSet dataSet = null;

                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    using (var stream = File.Open(selectedFile, FileMode.Open, FileAccess.Read))
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {

                        // Skip unnecesary lines
                        for (int i = 0; i < 9; i++)
                        {
                            reader.Read();
                        }
                        do
                        {
                            while (reader.Read())
                            {
                                Base data = null;
                                if (reader.GetValue(1).ToString() == "KÁRTYATRANZAKCIÓ")
                                    GlobalStore.Add(new Transaction(reader));
                                else if (new[] { "ÁTUTALÁS", "EGYÉB JÓVÁÍRÁS", "EGYÉB TERHELÉS" }.Contains(reader.GetValue(1).ToString()))
                                    GlobalStore.Add(new Transfer(reader));
                                else
                                {

                                    string ss = reader.GetValue(1).ToString();
                                    Console.WriteLine();
                                    continue;
                                }
                            }
                        } while (reader.NextResult());
                    }

                    if (dataSet != null && dataSet.Tables.Count > 0)
                    {
                        DataTable table = dataSet.Tables[0];
                    }
                }

                GlobalStore.Store();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Income_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = IncomePage;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
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