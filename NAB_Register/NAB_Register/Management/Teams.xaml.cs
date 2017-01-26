using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NAB_Register.Management
{
    /// <summary>
    /// Interaction logic for Teams.xaml
    /// </summary>
    public partial class Teams : Window
    {
        private List<Data.Team> teams = new List<Data.Team>();
        public string connectionString;
        private bool? showInactive = false;

        public Teams(string connstr)
        {
            InitializeComponent();
            connectionString = connstr;
            GetDataItems();
        }

        private void GetDataItems()
        {
            teams.Clear();
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    string query = "SELECT * FROM Team";

                    //get teams
                    OleDbCommand command = new OleDbCommand(query, connection);
                    connection.Open();

                    OleDbDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (showInactive == false)
                        {
                            if (Convert.ToBoolean(reader[4]) == true)
                            {
                                teams.Add(new Data.Team(Convert.ToInt32(reader[0]), Convert.ToInt32(reader[1].ToString()), reader[2].ToString(), reader[3].ToString(), Convert.ToBoolean(reader[4])));
                            }
                        }
                        else
                        {
                            teams.Add(new Data.Team(Convert.ToInt32(reader[0]), Convert.ToInt32(reader[1].ToString()), reader[2].ToString(), reader[3].ToString(), Convert.ToBoolean(reader[4])));
                        }
                    }
                    reader.Close();
                }
                dgTeams.ItemsSource = null;
                dgTeams.ItemsSource = teams.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errors while connecting to the database. Try again in a few moments or try setting the location again", "Error");
            }

            Mouse.OverrideCursor = null;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Team win = new Team(connectionString, null);
            win.ShowDialog();
            GetDataItems();
        }

        private void dgTeams_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Team win = new Team(connectionString, dgTeams.SelectedItem);
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