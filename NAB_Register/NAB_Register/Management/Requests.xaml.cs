using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Input;

namespace NAB_Register.Management
{
    /// <summary>
    /// Interaction logic for Requests.xaml
    /// </summary>
    public partial class Requests : Window
    {
        private List<Data.Request> requests = new List<Data.Request>();
        public string connectionString;
        private bool? showInactive = false;

        public Requests(string connstr)
        {
            InitializeComponent();
            connectionString = connstr;
            GetDataItems();
        }

        private void GetDataItems()
        {
            requests.Clear();
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    string query = "SELECT * FROM Request";

                    //get requests
                    OleDbCommand command = new OleDbCommand(query, connection);
                    connection.Open();

                    OleDbDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (showInactive == false)
                        {
                            if (Convert.ToBoolean(reader[3]) == true)
                            {
                                requests.Add(new Data.Request(Convert.ToInt32(reader[0]), reader[1].ToString(), reader[2].ToString(), Convert.ToBoolean(reader[3])));
                            }
                        }
                        else
                        {
                            requests.Add(new Data.Request(Convert.ToInt32(reader[0]), reader[1].ToString(), reader[2].ToString(), Convert.ToBoolean(reader[3])));
                        }
                    }
                    reader.Close();
                }
                dgRequests.ItemsSource = null;
                dgRequests.ItemsSource = requests;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errors while connecting to the database. Try again in a few moments or try setting the location again", "Error");
            }

            Mouse.OverrideCursor = null;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Request win = new Request(connectionString, null);
            win.ShowDialog();
            GetDataItems();
        }

        private void dgRequests_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Request win = new Request(connectionString, dgRequests.SelectedItem);
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