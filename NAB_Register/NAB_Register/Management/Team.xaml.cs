using System;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Input;

namespace NAB_Register.Management
{
    /// <summary>
    /// Interaction logic for Team.xaml
    /// </summary>
    public partial class Team : Window
    {
        public string connectionString;
        private Data.Team team;

        public Team(string connStr, object t)
        {
            InitializeComponent();
            connectionString = connStr;
            chkActive.Visibility = Visibility.Visible;

            if (t == null)
            {
                chkActive.Visibility = Visibility.Collapsed;
                chkActive.IsChecked = true;
            }
            else
            {
                team = t as Data.Team;
                txtNumber.Text = team.Number.ToString();
                txtName.Text = team.Name;
                txtEmail.Text = team.Email;
                chkActive.IsChecked = team.IsActive;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            int num;
            if (!string.IsNullOrWhiteSpace(txtNumber.Text) && !string.IsNullOrWhiteSpace(txtName.Text) && !string.IsNullOrWhiteSpace(txtEmail.Text) && int.TryParse(txtNumber.Text, out num))
            {
                Mouse.OverrideCursor = Cursors.Wait;

                string query = "";
                int? id = null;

                if (team == null)
                {
                    query = "INSERT into TEAM (TeamNumber, TeamName, Email, IsActive) VALUES (@TeamNumber, @TeamName, @Email, @IsActive)";
                }
                else
                {
                    id = team.ID;
                    query = "UPDATE Team SET TeamNumber = ? , TeamName = ?, Email = ?, IsActive = ? WHERE TeamID = ? ";
                }

                team = new Data.Team(id, Convert.ToInt32(txtNumber.Text), txtName.Text, txtEmail.Text, Convert.ToBoolean(chkActive.IsChecked));

                try
                {
                    using (OleDbConnection conn = new OleDbConnection(connectionString))
                    {
                        OleDbCommand cmd = new OleDbCommand(query, conn);
                        conn.Open();

                        if (team.ID != null)
                        {
                            cmd.Parameters.AddWithValue("@TeamNumber", team.Number);
                            cmd.Parameters.AddWithValue("@TeamName", team.Name);
                            cmd.Parameters.AddWithValue("@Email", team.Email);
                            cmd.Parameters.AddWithValue("@IsActive", team.IsActive);
                            cmd.Parameters.AddWithValue("@BankerID", team.ID);
                        }
                        else
                        {
                            cmd.Parameters.Add("@TeamNumber", OleDbType.VarChar).Value = team.Number;
                            cmd.Parameters.Add("@TeamName", OleDbType.VarChar).Value = team.Name;
                            cmd.Parameters.Add("@Email", OleDbType.VarChar).Value = team.Email;
                            cmd.Parameters.Add("@IsActive", OleDbType.Boolean).Value = team.IsActive;
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
                if (int.TryParse(txtNumber.Text, out num))
                {
                    MessageBox.Show("Please complete all fields before saving!", "Error");
                }
                else
                {
                    MessageBox.Show("Team number must be an integer!", "Error");
                }
            }
        }
    }
}