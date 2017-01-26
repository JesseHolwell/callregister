using System;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Input;

namespace NAB_Register.Management
{
    /// <summary>
    /// Interaction logic for Feedback.xaml
    /// </summary>
    public partial class Feedback : Window
    {
        public string connectionString;
        private Data.Feedback feedback;

        public Feedback(string connStr, object f)
        {
            InitializeComponent();
            connectionString = connStr;
            chkActive.Visibility = Visibility.Visible;

            if (f == null)
            {
                chkActive.Visibility = Visibility.Collapsed;
                chkActive.IsChecked = true;
            }
            else
            {
                feedback = f as Data.Feedback;
                txtFeedbackName.Text = feedback.Name;
                chkActive.IsChecked = feedback.IsActive;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtFeedbackName.Text))
            {
                Mouse.OverrideCursor = Cursors.Wait;

                string query = "";
                int? id = null;

                if (feedback == null)
                {
                    query = "INSERT into Feedback (FeedbackName, IsActive) VALUES (@FeedbackName, @IsActive)";
                }
                else
                {
                    id = feedback.ID;
                    query = "UPDATE Feedback SET FeedbackName = ? , IsActive = ? WHERE FeedbackID = ? ";
                }

                feedback = new Data.Feedback(id, txtFeedbackName.Text, Convert.ToBoolean(chkActive.IsChecked));

                try
                {
                    using (OleDbConnection conn = new OleDbConnection(connectionString))
                    {
                        OleDbCommand cmd = new OleDbCommand(query, conn);
                        conn.Open();

                        if (feedback.ID != null)
                        {
                            cmd.Parameters.AddWithValue("@FeedbackName", feedback.Name);
                            cmd.Parameters.AddWithValue("@IsActive", feedback.IsActive);
                            cmd.Parameters.AddWithValue("@BankerID", feedback.ID);
                        }
                        else
                        {
                            cmd.Parameters.Add("@FeedbackName", OleDbType.VarChar).Value = feedback.Name;
                            cmd.Parameters.Add("@IsActive", OleDbType.Boolean).Value = feedback.IsActive;
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