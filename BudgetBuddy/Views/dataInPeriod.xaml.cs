using BudgetBuddy.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Költég.Class
{
    /// <summary>
    /// Interaction logic for dataInPeriod.xaml
    /// </summary>
    public partial class dataInPeriod : Window
    {
        public dataInPeriod(List<Transaction> transactions)
        {
            InitializeComponent();
            data.ItemsSource = transactions;
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

    }
}
