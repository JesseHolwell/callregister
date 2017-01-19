using System;
using System.Collections.Generic;
using System.Data.OleDb;
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

namespace NAB_Register.Management
{
    /// <summary>
    /// Interaction logic for Products.xaml
    /// </summary>
    public partial class Products : Window
    {
        List<Data.Product> products = new List<Data.Product>();
        public string connectionString;
        bool? showInactive = false;

        public Products(string connstr)
        {
            InitializeComponent();
            connectionString = connstr;
            GetDataItems();
        }

        private void GetDataItems()
        {
            products.Clear();
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    string query = "SELECT * FROM Product";


                    //get feedback
                    OleDbCommand command = new OleDbCommand(query, connection);
                    connection.Open();

                    OleDbDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (showInactive == false)
                        {
                            if (Convert.ToBoolean(reader[2]) == true)
                            {
                                products.Add(new Data.Product(Convert.ToInt32(reader[0]), reader[1].ToString(), Convert.ToBoolean(reader[2].ToString())));
                            }
                        }
                        else
                        {
                            products.Add(new Data.Product(Convert.ToInt32(reader[0]), reader[1].ToString(), Convert.ToBoolean(reader[2].ToString())));
                        }
                    }
                    reader.Close();
                }
                dgProducts.ItemsSource = null;
                dgProducts.ItemsSource = products;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Errors while connecting to the database. Try again in a few moments or try setting the location again", "Error");
            }

            Mouse.OverrideCursor = null;

        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Product win = new Product(connectionString, null);
            win.ShowDialog();
            GetDataItems();
        }

        private void dgProducts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Product win = new Product(connectionString, dgProducts.SelectedItem);
            win.ShowDialog();
            GetDataItems();
        }

        private void chkInactive_Checked(object sender, RoutedEventArgs e)
        {
            showInactive = true;
            GetDataItems();
        }

        private void chkInactive_Unchecked(object sender, RoutedEventArgs e)
        {
            showInactive = false;
            GetDataItems();
        }
    }
}
