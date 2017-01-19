using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NAB_Register.Management
{
    /// <summary>
    /// Interaction logic for Bankers.xaml
    /// </summary>
    public partial class Bankers : Window
    {
        List<Data.Banker> bankers = new List<Data.Banker>();
        public string connectionString;
        bool? showInactive = false;

        public Bankers(string connstr)
        {
            InitializeComponent();
            connectionString = connstr;
            GetDataItems();
        }

        private void GetDataItems()
        {
            bankers.Clear();
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    string query = "SELECT * FROM Banker";
                    

                    //get bankers
                    OleDbCommand command = new OleDbCommand(query, connection);
                    connection.Open();

                    OleDbDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (showInactive == false)
                        {
                            if (Convert.ToBoolean(reader[4]) == true)
                            {
                                bankers.Add(new Data.Banker(Convert.ToInt32(reader[0]), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), Convert.ToBoolean(reader[4])));
                            }
                        }
                        else
                        {
                            bankers.Add(new Data.Banker(Convert.ToInt32(reader[0]), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), Convert.ToBoolean(reader[4])));
                        }
                    }
                    reader.Close();
                }
                dgBankers.ItemsSource = null;
                dgBankers.ItemsSource = bankers;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errors while connecting to the database. Try again in a few moments or try setting the location again", "Error");
            }

            Mouse.OverrideCursor = null;

        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Banker win = new Banker(connectionString, null);
            win.ShowDialog();
            GetDataItems();
        }

        private void dgBankers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Banker win = new Banker(connectionString, dgBankers.SelectedItem);
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
