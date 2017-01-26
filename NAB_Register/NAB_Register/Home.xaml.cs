using NAB_Register.Management;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Banker = NAB_Register.Data.Banker;
using Request = NAB_Register.Data.Request;

namespace NAB_Register
{
    /// <summary>
    ///     Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        private void cmbBanker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetTeam();
            lblBanker.Foreground = brushText;
        }

        private void cmbProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateRequests();
            lblProduct.Foreground = brushText;
        }

        private void cmbFeedback_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnableArticles();
            lblFeedback.Foreground = brushText;
        }

        private void cmbRequest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lblRequest.Foreground = brushText;
        }

        private void txtArticle_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblArticle.Foreground = brushText;
        }

        private void txtComments_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblComments.Foreground = brushText;
        }

        private void dgRecentCalls_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenCall(dgRecentCalls.SelectedItem);
        }

        private void btnCallList_Click(object sender, RoutedEventArgs e)
        {
            var win = new CallList(GetConnectionString());
            win.Show();
        }

        #region Variables

        private readonly string connectionString = "";
        private bool Submitted;

        //UI Content
        private readonly List<Data.Call> Calls = new List<Data.Call>();

        private readonly List<Banker> Bankers = new List<Banker>();
        private readonly List<string> Products = new List<string>();
        private readonly List<Request> Requests = new List<Request>();
        private readonly List<string> Feedbacks = new List<string>();

        //Design
        private static readonly Brush brushText = Brushes.Black;

        private static readonly Brush brushError = Brushes.Red;
        private static readonly Brush brushControl = Brushes.White;
        private static readonly Brush brushReadOnly = Brushes.LightGray;

        #endregion Variables

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
                GetDataItems();
        }

        private string GetConnectionString()
        {
            var connectionString = "";

            try
            {
                using (var sr = new StreamReader("Data\\config.txt"))
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

                using (var connection = new OleDbConnection(connectionString))
                {
                    var queryBankers = "SELECT * FROM Banker";
                    var queryProducts = "SELECT * FROM Product";
                    var queryRequests = "SELECT * FROM Request";
                    var queryFeedbacks = "SELECT * FROM Feedback";

                    //get bankers
                    var command = new OleDbCommand(queryBankers, connection);
                    connection.Open();

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                        if (Convert.ToBoolean(reader[4]))
                            Bankers.Add(new Banker(null, reader[1].ToString(), reader[2].ToString(),
                                reader[3].ToString(), null));
                    reader.Close();

                    //get products
                    command.CommandText = queryProducts;

                    reader = command.ExecuteReader();
                    while (reader.Read())
                        if (Convert.ToBoolean(reader[2]))
                            Products.Add(reader[1].ToString());
                    reader.Close();

                    //get request types
                    command.CommandText = queryRequests;

                    reader = command.ExecuteReader();
                    while (reader.Read())
                        if (Convert.ToBoolean(reader[3]))
                            Requests.Add(new Request(null, reader[1].ToString(), reader[2].ToString(), null));
                    reader.Close();

                    //get feedback types
                    command.CommandText = queryFeedbacks;

                    reader = command.ExecuteReader();
                    while (reader.Read())
                        if (Convert.ToBoolean(reader[2]))
                            Feedbacks.Add(reader[1].ToString());
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
                MessageBox.Show(
                    "Errors while connecting to the database. Try again in a few moments or try setting the location again",
                    "Error");
            }

            Mouse.OverrideCursor = null;
        }

        private void GetCallList()
        {
            var query = "SELECT * FROM Call";

            using (var connection = new OleDbConnection(connectionString))
            {
                var command = new OleDbCommand(query, connection);
                connection.Open();

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var call = new Data.Call();

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

            var theTime = DateTime.Now;
            var userid = Environment.UserName;
            var team = txtTeam.Text;
            var banker = cmbBanker.SelectedItem.ToString();
            var product = cmbProduct.SelectedItem.ToString();
            var request = cmbRequest.SelectedItem.ToString();
            var feedback = cmbFeedback.SelectedItem.ToString();
            var article = txtArticle.Text;
            var comments = txtComments.Text;
            var important = chkImportant.IsChecked;

            var query =
                "INSERT into Call ([Time], UserID, Team, Banker, Product, Request, Feedback, Article, Comments, Important) VALUES (@Time, @UserID, @Team, @Banker, @Product, @Request, @Feedback, @Article, @Comments, @Important)";

            try
            {
                using (var conn = new OleDbConnection(connectionString))
                {
                    var cmd = new OleDbCommand(query, conn);
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
            var result = true;
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
                    if (string.IsNullOrWhiteSpace(txtArticle.Text))
                    {
                        result = false;
                        lblArticle.Foreground = brushError;
                    }
                    else
                    {
                        lblArticle.Foreground = brushText;
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
            foreach (var b in Bankers)
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
            foreach (var r in Requests)
                if (r.Product == cmbProduct.SelectedItem.ToString())
                    cmbRequest.Items.Add(r.Name);
        }

        public void EnableArticles()
        {
            if (cmbFeedback.SelectedItem.ToString().ToUpper() == "NABIT ARTICLE INSUFFICIENT")
            {
                txtArticle.IsEnabled = true;
            }
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
                var call = c as Data.Call;

                var win = new Call(call);
                win.Show();
            }
        }

        private void ResetWindow()
        {
            var win = new Home();

            win.Top = Top;
            win.Left = Left;
            win.Height = Height;
            win.Width = Width;

            win.Show();
            Close();
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
                if (
                    MessageBox.Show("You will lose all unsaved data, are you sure?", "Warning", MessageBoxButton.YesNo) ==
                    MessageBoxResult.Yes)
                {
                    var win = new Login();
                    win.ShowDialog();
                    if (win.success)
                    {
                        var win2 = new Settings();
                        win2.ShowDialog();
                        ResetWindow();
                    }
                }
            }
            else
            {
                var win = new Login();
                win.ShowDialog();
                if (win.success)
                {
                    var win2 = new Settings();
                    win2.ShowDialog();
                    ResetWindow();
                }
            }
        }

        #endregion Methods

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

        #endregion EVENTS
    }
}