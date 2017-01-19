using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Media;
using NAB_Register.Data;
using System.Data.OleDb;
using System.IO;
using System.Timers;
using System.Windows.Input;
using System.Linq;
using System.Diagnostics;

namespace NAB_Register
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        #region Variables

        private string connectionString = "";
        private bool Submitted;

        //UI Content
        List<Data.Call> Calls = new List<Data.Call>();
        List<Banker> Bankers = new List<Banker>();
        List<string> Products = new List<string>();
        List<Request> Requests = new List<Request>();
        List<string> Feedbacks = new List<string>();

        //Design
        static readonly Brush brushText = Brushes.Black;
        static readonly Brush brushError = Brushes.Red;
        static readonly Brush brushControl = Brushes.White;
        static readonly Brush brushReadOnly = Brushes.LightGray;

        #endregion

        #region Methods

        public Home()
        {
            InitializeComponent();

            Submitted = false;

            cmbRequest.IsEnabled = false;
            txtArticle.IsEnabled = false;
            txtTeam.IsEnabled = false;

            lblLoggedInUser.Content = Environment.UserName;
            connectionString = GetConnectionString();
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                GetDataItems();
            }

        }

        private string GetConnectionString()
        {
            string connectionString = "";

            try
            {
                using (StreamReader sr = new StreamReader("Data\\config.txt"))
                {
                    connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + sr.ReadLine();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not find the database configuration file. Set the location again", "Error");
            }

            return connectionString;
        }

        private void GetDataItems()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                GetCallList();

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    string queryBankers = "SELECT * FROM Banker";
                    string queryProducts = "SELECT * FROM Product";
                    string queryRequests = "SELECT * FROM Request";
                    string queryFeedbacks = "SELECT * FROM Feedback";

                    //get bankers
                    OleDbCommand command = new OleDbCommand(queryBankers, connection);
                    connection.Open();

                    OleDbDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (Convert.ToBoolean(reader[4]) == true)
                        {
                            Bankers.Add(new Banker(null, reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), null));
                        }
                    }
                    reader.Close();

                    //get products
                    command.CommandText = queryProducts;

                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (Convert.ToBoolean(reader[2]) == true)
                        {
                            Products.Add(reader[1].ToString());
                        }
                    }
                    reader.Close();

                    //get request types
                    command.CommandText = queryRequests;

                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (Convert.ToBoolean(reader[3]) == true)
                        {
                            Requests.Add(new Request(null, reader[1].ToString(), reader[2].ToString(), null));
                        }
                    }
                    reader.Close();

                    //get feedback types
                    command.CommandText = queryFeedbacks;

                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (Convert.ToBoolean(reader[2]) == true)
                        {
                            Feedbacks.Add(reader[1].ToString());
                        }
                    }
                    reader.Close();
                    connection.Close();

                }

                cmbBanker.ItemsSource = Bankers;
                cmbProduct.ItemsSource = Products;
                //cmbRequest.ItemsSource = Requests;
                cmbFeedback.ItemsSource = Feedbacks;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errors while connecting to the database. Try again in a few moments or try setting the location again", "Error");
            }

            Mouse.OverrideCursor = null;

        }

        private void GetCallList()
        {
            string query = "SELECT * FROM Call";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                connection.Open();

                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Data.Call call = new Data.Call();

                    call.CallID = Convert.ToInt32(reader[0]);
                    call.Time = Convert.ToDateTime(reader[1].ToString());
                    call.UserID = reader[2].ToString();
                    call.Team = reader[3].ToString();
                    call.Banker = reader[4].ToString();
                    call.Product = reader[5].ToString();
                    call.Request = reader[6].ToString();
                    call.Feedback = reader[7].ToString();
                    call.Article = reader[8].ToString();
                    call.Comments = reader[9].ToString();
                    call.Important = Convert.ToBoolean(reader[10]);

                    Calls.Add(call);
                }
                reader.Close();
                connection.Close();

            }

            //sort decending
            Calls.Reverse();

            //only show top 50
            if (Calls.Count > 50)
                Calls.RemoveRange(50, Calls.Count - 50);

            dgRecentCalls.ItemsSource = Calls.ToList();
            //dgRecentCalls.ItemsSource = Calls.Select(x => new
            //     { Time = x.Time, UserID = x.UserID, Team = x.Team, Banker = x.Banker,
            //         Product = x.Product, Request = x.Request, Feedback = x.Feedback,
            //         Comments = x.Comments }).ToList();



        }

        public void SaveData()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            DateTime theTime = DateTime.Now;
            string userid = Environment.UserName;
            string team = txtTeam.Text;
            string banker = cmbBanker.SelectedItem.ToString();
            string product = cmbProduct.SelectedItem.ToString();
            string request = cmbRequest.SelectedItem.ToString();
            string feedback = cmbFeedback.SelectedItem.ToString();
            string article = txtArticle.Text;
            string comments = txtComments.Text;
            bool? important = chkImportant.IsChecked;

            string query = "INSERT into Call ([Time], UserID, Team, Banker, Product, Request, Feedback, Article, Comments, Important) VALUES (@Time, @UserID, @Team, @Banker, @Product, @Request, @Feedback, @Article, @Comments, @Important)";

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    OleDbCommand cmd = new OleDbCommand(query, conn);
                    conn.Open();

                    cmd.Parameters.Add("@Time", OleDbType.VarChar).Value = theTime.ToString();
                    cmd.Parameters.Add("@UserID", OleDbType.VarChar).Value = userid;
                    cmd.Parameters.Add("@Team", OleDbType.VarChar).Value = team;
                    cmd.Parameters.Add("@Banker", OleDbType.VarChar).Value = banker;
                    cmd.Parameters.Add("@Product", OleDbType.VarChar).Value = product;
                    cmd.Parameters.Add("@Request", OleDbType.VarChar).Value = request;
                    cmd.Parameters.Add("@Feedback", OleDbType.VarChar).Value = feedback;
                    cmd.Parameters.Add("@Article", OleDbType.VarChar).Value = article;
                    cmd.Parameters.Add("@Comments", OleDbType.VarChar).Value = comments;
                    cmd.Parameters.Add("@Important", OleDbType.Boolean).Value = important;

                    cmd.ExecuteNonQuery();
                    conn.Close();

                    MessageBox.Show("Your call has been saved successfully", "Success");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not write to the database. Try again in a few moments", "Error");

            }

            Mouse.OverrideCursor = null;
        }


        private bool ValidateFields()
        {
            bool result = true;
            if (Submitted)
            {
                if (cmbBanker.SelectedItem == null)
                {
                    result = false;
                    lblBanker.Foreground = brushError;
                }
                else
                {
                    lblBanker.Foreground = brushText;
                }
                if (cmbProduct.SelectedItem == null)
                {
                    result = false;
                    lblProduct.Foreground = brushError;
                }
                else
                {
                    lblProduct.Foreground = brushText;
                }
                if (cmbRequest.SelectedItem == null)
                {
                    result = false;
                    lblRequest.Foreground = brushError;
                }
                else
                {
                    lblRequest.Foreground = brushText;
                }
                if (cmbFeedback.SelectedItem == null)
                {
                    result = false;
                    lblFeedback.Foreground = brushError;
                }
                else
                {
                    lblFeedback.Foreground = brushText;
                }
                if (txtArticle.IsEnabled)
                {
                    if (string.IsNullOrWhiteSpace(txtArticle.Text))
                    {
                        result = false;
                        lblArticle.Foreground = brushError;
                    }
                    else
                    {
                        lblArticle.Foreground = brushText;
                    }
                }
                if (string.IsNullOrWhiteSpace(txtComments.Text))
                {
                    result = false;
                    lblComments.Foreground = brushError;
                }
                else
                {
                    lblComments.Foreground = brushText;
                }
            }
            else
            {
                result = false;
            }

            return result;
        }

        public void SetTeam()
        {
            foreach (Banker b in Bankers)
                if (b == cmbBanker.SelectedItem)
                {
                    txtTeam.Text = b.Team;
                    break;
                }
        }

        public void PopulateRequests()
        {
            cmbRequest.IsEnabled = true;
            cmbRequest.Items.Clear();
            foreach (Request r in Requests)
            {
                if (r.Product == cmbProduct.SelectedItem.ToString())
                    cmbRequest.Items.Add(r.Name);
            }

        }

        public void EnableArticles()
        {
            if (cmbFeedback.SelectedItem.ToString().ToUpper() == "NABIT ARTICLE INSUFFICIENT")
                txtArticle.IsEnabled = true;
            else
            {
                txtArticle.IsEnabled = false;
                txtArticle.Text = "";
                lblArticle.Foreground = brushText;
            }
        }

        private void OpenCall(object c)
        {
            if (c is Data.Call)
            {
                Data.Call call = c as Data.Call;

                Call win = new Call(call);
                win.Show();
            }
        }

        private void ResetWindow()
        {
            Home win = new Home();

            win.Top = this.Top;
            win.Left = this.Left;
            win.Height = this.Height;
            win.Width = this.Width;

            win.Show();
            this.Close();
        }

        private void SaveCall()
        {
            Submitted = true;
            if (ValidateFields())
            {
                SaveData();
                ResetWindow();
            }
            else
            {
                MessageBox.Show("Please complete all fields before saving!", "Error");
            }
        }

        private void OpenSettings()
        {
            if (cmbBanker.SelectedItem != null || cmbFeedback.SelectedItem != null ||
                cmbProduct.SelectedItem != null || cmbRequest.SelectedItem != null ||
                txtComments.Text != "" || txtArticle.Text != "")
            {
                if (MessageBox.Show("You will lose all unsaved data, are you sure?", "Warning", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {

                    Management.Login win = new Management.Login();
                    win.ShowDialog();
                    if (win.success == true)
                    {
                        Settings win2 = new Settings();
                        win2.ShowDialog();
                        ResetWindow();
                    }

                }
            }
            else
            {
                Management.Login win = new Management.Login();
                win.ShowDialog();
                if (win.success == true)
                {
                    Settings win2 = new Settings();
                    win2.ShowDialog();
                    ResetWindow();
                }
            }

        }

        #endregion


        #region EVENTS

        private void btnNewCall_Click(object sender, RoutedEventArgs e)
        {
            ResetWindow();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            btnSave.IsEnabled = false;
            SaveCall();
            btnSave.IsEnabled = true;
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            OpenSettings();
        }

        #endregion

        private void cmbBanker_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SetTeam();
            lblBanker.Foreground = brushText;
        }

        private void cmbProduct_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            PopulateRequests();
            lblProduct.Foreground = brushText;
        }

        private void cmbFeedback_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            EnableArticles();
            lblFeedback.Foreground = brushText;
        }

        private void cmbRequest_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            lblRequest.Foreground = brushText;
        }

        private void txtArticle_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            lblArticle.Foreground = brushText;
        }

        private void txtComments_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            lblComments.Foreground = brushText;
        }

        private void dgRecentCalls_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenCall(dgRecentCalls.SelectedItem);
        }

        private void btnCallList_Click(object sender, RoutedEventArgs e)
        {
            CallList win = new CallList(GetConnectionString());
            win.Show();
        }
    }
}
