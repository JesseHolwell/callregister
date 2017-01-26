using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Input;

namespace NAB_Register.Management
{
    /// <summary>
    /// Interaction logic for Feedbacks.xaml
    /// </summary>
    public partial class Feedbacks : Window
    {
        private List<Data.Feedback> feedbacks = new List<Data.Feedback>();
        public string connectionString;
        private bool? showInactive = false;

        public Feedbacks(string connstr)
        {
            InitializeComponent();
            connectionString = connstr;
            GetDataItems();
        }

        private void GetDataItems()
        {
            feedbacks.Clear();
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    string query = "SELECT * FROM Feedback";

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
                                feedbacks.Add(new Data.Feedback(Convert.ToInt32(reader[0]), reader[1].ToString(), Convert.ToBoolean(reader[2].ToString())));
                            }
                        }
                        else
                        {
                            feedbacks.Add(new Data.Feedback(Convert.ToInt32(reader[0]), reader[1].ToString(), Convert.ToBoolean(reader[2].ToString())));
                        }
                    }
                    reader.Close();
                }
                dgFeedbacks.ItemsSource = null;
                dgFeedbacks.ItemsSource = feedbacks;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errors while connecting to the database. Try again in a few moments or try setting the location again", "Error");
            }

            Mouse.OverrideCursor = null;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Feedback win = new Feedback(connectionString, null);
            win.ShowDialog();
            GetDataItems();
        }

        private void dgFeedbacks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Feedback win = new Feedback(connectionString, dgFeedbacks.SelectedItem);
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