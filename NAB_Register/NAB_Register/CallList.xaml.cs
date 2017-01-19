using System;
using System.Collections.Generic;
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
using System.Data;
using System.Data.SqlClient;
using NAB_Register.Data;
using System.Data.OleDb;
using System.IO;
using Microsoft.Win32;

namespace NAB_Register
{
    /// <summary>
    /// Interaction logic for CallList.xaml
    /// </summary>
    public partial class CallList : Window
    {
        #region Variables

        List<Data.Call> Calls = new List<Data.Call>();
        List<Data.Call> FilteredCalls = new List<Data.Call>();

        List<Banker> Bankers = new List<Banker>();
        List<string> Products = new List<string>();
        List<Request> Requests = new List<Request>();
        List<string> Feedbacks = new List<string>();
        List<Team> Teams = new List<Team>();

        public string connectionString;

        #endregion

        #region Methods
        public CallList(string connStr)
        {

            InitializeComponent();

            connectionString = connStr;
            GetDataItems();
            GetCallList();
            FilterSelection();
        }

        private void GetCallList()
        {
            string query = "SELECT * FROM Call";

            try
            {
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

                dgCalls.ItemsSource = null;
                dgCalls.ItemsSource = Calls.ToList();
            }
            catch (Exception ex)
            {

            }

        }

        private void GetDataItems()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                Bankers.Clear();
                Products.Clear();
                Requests.Clear();
                Feedbacks.Clear();
                Teams.Clear();

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    string queryBankers = "SELECT * FROM Banker";
                    string queryProducts = "SELECT * FROM Product";
                    string queryRequests = "SELECT * FROM Request";
                    string queryFeedbacks = "SELECT * FROM Feedback";
                    string queryTeams = "SELECT * FROM Team";

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

                    //get teams
                    command.CommandText = queryTeams;

                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (Convert.ToBoolean(reader[4]) == true)
                        {
                            Teams.Add(new Data.Team(Convert.ToInt32(reader[0]), Convert.ToInt32(reader[1].ToString()), reader[2].ToString(), reader[3].ToString(), Convert.ToBoolean(reader[4])));
                        }
                    }
                    reader.Close();

                    connection.Close();

                }

                //TODO: nullable combobox
                //Bankers.Add(null);

                cmbBanker.ItemsSource = Bankers;
                cmbProduct.ItemsSource = Products;
                cmbRequest.ItemsSource = Requests;
                cmbFeedback.ItemsSource = Feedbacks;
                cmbTeam.ItemsSource = Teams;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errors while connecting to the database. Try again in a few moments or try setting the location again", "Error");
            }

            Mouse.OverrideCursor = null;

        }

        public void FilterSelection()
        {
            FilteredCalls = new List<Data.Call>();
            //= Calls.ToList();
            //.Where userid = txtuserid
            //.Where(x => filteredCalls.All(call => x.CallId.Contains(txtCallId)));

            foreach (Data.Call call in Calls)
            {
                bool success = true;

                while (success == true)
                {
                    if (dpFromDate.SelectedDate != null && dpToDate.SelectedDate != null)
                    {
                        DateTime? from = dpFromDate.SelectedDate;
                        DateTime? to = dpToDate.SelectedDate;

                        DateTime callDate = call.Time;
                        if ((callDate > from && callDate < to) == false)
                        {
                            success = false;
                            break;
                        }
                    }

                    if (call.UserID.ToUpper().Contains(txtUserID.Text.ToUpper()) == false)
                    {
                        success = false;
                        break;
                    }
                    if (cmbBanker.SelectedValue != null)
                    {
                        if ((call.Banker == cmbBanker.SelectedValue.ToString()) == false)
                        {
                            success = false;
                            break;
                        }
                    }
                    if (cmbTeam.SelectedValue != null)
                    {
                        if ((call.Team == cmbTeam.SelectedValue.ToString()) == false)
                        {
                            success = false;
                            break;
                        }
                    }
                    if (cmbProduct.SelectedValue != null)
                    {
                        if ((call.Product == cmbProduct.SelectedValue.ToString()) == false)
                        {
                            success = false;
                            break;
                        }
                    }
                    if (cmbRequest.SelectedValue != null)
                    {
                        if ((call.Request == cmbRequest.SelectedValue.ToString()) == false)
                        {
                            success = false;
                            break;
                        }
                    }
                    if (cmbFeedback.SelectedValue != null)
                    {
                        if ((call.Feedback == cmbFeedback.SelectedValue.ToString()) == false)
                        {
                            success = false;
                            break;
                        }
                    }
                    if (chkImportant.IsChecked.HasValue)
                    {
                        if (chkImportant.IsChecked.Value)
                        {
                            if (call.Important == false)
                            {
                                success = false;
                                break;
                            }
                        }
                    }

                    if (success)
                    {
                        FilteredCalls.Add(call);
                        success = false;
                    }
                }
            }

            dgCalls.ItemsSource = FilteredCalls.ToList();
        }

        public void ExportToCsv()
        {
            if (FilteredCalls.Count > 0)
            {
                string fileName = "Custom Banker Feedback Report " + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".csv";

                SaveFileDialog sfdlg = new SaveFileDialog();
                sfdlg.FileName = fileName;
                sfdlg.DefaultExt = ".csv";
                sfdlg.Filter = "CSV Files (.csv)|*.csv";

                bool? result = sfdlg.ShowDialog();
                if (result == true)
                {
                    string file = sfdlg.FileName;
                    string csvString = "Call ID,Time,User ID,Team,Banker,Product,Request,Feedback,Article,Comments,Important\n";

                    try
                    {
                        foreach (Data.Call c in FilteredCalls)
                        {
                            csvString += c.ToString();
                        }

                        System.IO.File.WriteAllText(file, csvString);
                    }
                    catch
                    {
                        MessageBox.Show("Could not save the file", "Error");
                    }
                }
                else
                {
                    MessageBox.Show("File not saved", "Warning");
                }
            }
            else
            {
                MessageBox.Show("There are no calls to export", "Error");
            }
        }

        #endregion

        #region EventListeners

        private void dgCalls_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (dgCalls.SelectedItem is Data.Call)
            {
                Data.Call call = dgCalls.SelectedItem as Data.Call;

                Call win = new Call(call);
                win.Show();
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void dpFromDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterSelection();
        }

        private void dpToDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterSelection();
        }


        private void chkImportant_Checked(object sender, RoutedEventArgs e)
        {
            FilterSelection();
        }

        private void chkImportant_Unchecked(object sender, RoutedEventArgs e)
        {
            FilterSelection();
        }


        private void txtUserID_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterSelection();
        }

        private void cmbBanker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterSelection();
        }

        private void cmbTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterSelection();
        }


        private void cmbProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterSelection();
        }

        private void cmbRequest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterSelection();
        }

        private void cmbFeedback_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterSelection();
        }


        #endregion

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            CallList win = new CallList(connectionString);

            win.Top = this.Top;
            win.Left = this.Left;
            win.Height = this.Height;
            win.Width = this.Width;

            win.Show();
            this.Close();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            btnExport.IsEnabled = false;
            ExportToCsv();
            btnExport.IsEnabled = true;
        }
    }
}
