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
    /// Logika interakcji dla klasy UpdatePage.xaml
    /// </summary>
    public partial class UpdatePage : Window
    {
        DBEntities db = new DBEntities();
        int Id;

        public UpdatePage(int customerID)
        {
            InitializeComponent();
            Id = customerID;
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            Customer updateCustomer = (from c in db.Customer
                                      where c.CustomerID == Id
                                      select c).Single();
            db.SaveChanges();
            MainWindow.datagrid.ItemsSource = db.Customer.ToList();
            this.Hide();
        }
    }
}
