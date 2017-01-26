using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Input;

namespace NAB_Register.Management
{
    /// <summary>
    /// Interaction logic for Banker.xaml
    /// </summary>
    public partial class Banker : Window
    {
        public string connectionString;
        private Data.Banker banker;
        private List<Data.Team> teams = new List<Data.Team>();

        public Banker(string connStr, object b)
        {
            InitializeComponent();
            connectionString = connStr;
            GetDataItems();
            chkActive.Visibility = Visibility.Visible;

            if (b == null)
            {
                chkActive.Visibility = Visibility.Collapsed;
                chkActive.IsChecked = true;
            }
            else
            {
                banker = b as Data.Banker;
                txtFName.Text = banker.FName;
                txtLName.Text = banker.LName;
                cmbTeam.Text = banker.Team;
                chkActive.IsChecked = banker.IsActive;
            }
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

                    OleDbCommand command = new OleDbCommand(query, connection);
                    connection.Open();

                    OleDbDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (Convert.ToBoolean(reader[4]))
                        {
                            teams.Add(new Data.Team(null, Convert.ToInt32(reader[1]), reader[2].ToString(), reader[3].ToString(), null));
                        }
                    }
                    reader.Close();
                }

                cmbTeam.ItemsSource = teams;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errors while connecting to the database. Try again in a few moments or try setting the location again", "Error");
            }

            Mouse.OverrideCursor = null;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtFName.Text) && !string.IsNullOrWhiteSpace(txtLName.Text) && cmbTeam.SelectedItem != null)
            {
                Mouse.OverrideCursor = Cursors.Wait;

                string query = "";
                int? id = null;

                if (banker == null)
                {
                    query = "INSERT into Banker (FName, LName, Team, IsActive) VALUES (@FName, @LName, @Team, @IsActive)";
                }
                else
                {
                    id = banker.ID;
                    query = "UPDATE Banker SET FName = ? , LName = ? , Team = ? , IsActive = ? WHERE BankerID = ? ";
                }

                banker = new Data.Banker(id, txtFName.Text, txtLName.Text, cmbTeam.SelectedItem.ToString(), Convert.ToBoolean(chkActive.IsChecked));

                try
                {
                    using (OleDbConnection conn = new OleDbConnection(connectionString))
                    {
                        OleDbCommand cmd = new OleDbCommand(query, conn);
                        conn.Open();

                        if (banker.ID != null)
                        {
                            cmd.Parameters.AddWithValue("@FName", banker.FName);
                            cmd.Parameters.AddWithValue("@LName", banker.LName);
                            cmd.Parameters.AddWithValue("@Team", banker.Team);
                            cmd.Parameters.AddWithValue("@IsActive", banker.IsActive);

                            cmd.Parameters.AddWithValue("@BankerID", banker.ID);
                        }
                        else
                        {
                            cmd.Parameters.Add("@FName", OleDbType.VarChar).Value = banker.FName;
                            cmd.Parameters.Add("@LName", OleDbType.VarChar).Value = banker.LName;
                            cmd.Parameters.Add("@Team", OleDbType.VarChar).Value = banker.Team;
                            cmd.Parameters.Add("@IsActive", OleDbType.Boolean).Value = banker.IsActive;
                        }

                        cmd.ExecuteNonQuery();
                        conn.Close();
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not write to the database. Try again in a few moments", "Error");
                }

                Mouse.OverrideCursor = null;
            }
            else
            {
                MessageBox.Show("Please complete all fields before saving!", "Error");
            }
        }
    }
}