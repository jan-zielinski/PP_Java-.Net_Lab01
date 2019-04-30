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

namespace Lab01
{
    /// <summary>
    /// Logika interakcji dla klasy InsertPage.xaml
    /// </summary>
    public partial class InsertPage : Window
    {
        DBEntities db = new DBEntities();

        public InsertPage()
        {
            InitializeComponent();
        }

        private void InsertBtn_Click(object sender, RoutedEventArgs e)
        {
            Customer newCustomer = new Customer()
            {
                FirstName = firstnameTextBox.Text,
                LastName = lastnameTextBox.Text,
                City = cityTextBox.Text
            };

            db.Customer.Add(newCustomer);
            db.SaveChanges();
            MainWindow.datagrid.ItemsSource = db.Customer.ToList();
            this.Hide();
        }
    }
}
