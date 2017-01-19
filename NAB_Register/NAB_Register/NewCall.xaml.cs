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

namespace NAB_Register
{
    //UPDATE YourTable
    //SET YourColumnToUpdate = 'your_value'
    //WHERE YourUniqueColumn = @Id


    /// <summary>
    /// Interaction logic for Window1.xaml
    /// 
    /// </summary>
    public partial class NewCall : Window
    {
        #region Variables

        private bool Submitted;
        System.Timers.Timer timer = new System.Timers.Timer();
        Brush brushText;
        Brush brushError;
        Brush brushControl;
        Brush brushReadOnly;

        List<string> teamlist = new List<string>();

        string connString = @"Data Source=Jesse\SQLEXPRESS;Initial Catalog=db_NAB_CallRegister;Integrated Security=True";

        #endregion

        #region Methods

        public NewCall()
        {
            InitializeComponent();
            PopulateControls();

            Submitted = false;
            brushError = Brushes.Red;
            brushText = Brushes.Black;
            brushReadOnly = Brushes.LightGray;
            brushControl = Brushes.White;

            txtTeam.IsReadOnly = true;
            txtTeam.Background = brushReadOnly;
        }

        private bool ValidateFields()
        {
            bool result = true;
            if (Submitted)
            {
                if (cmbCaller.SelectedItem == null)
                {
                    result = false;
                    lblCaller.Foreground = brushError;
                    lblTeam.Foreground = brushError;
                }
                else
                {
                    lblCaller.Foreground = brushText;
                    lblTeam.Foreground = brushText;
                }
                if (cmbSubject.SelectedItem == null)
                {
                    result = false;
                    lblSubject.Foreground = brushError;
                }
                else
                {
                    lblSubject.Foreground = brushText;
                }
                if (cmbNature.SelectedItem == null)
                {
                    result = false;
                    lblNature.Foreground = brushError;
                }
                else
                {
                    lblNature.Foreground = brushText;
                }
                if (cmbResoultion.SelectedItem == null)
                {
                    result = false;
                    lblResolution.Foreground = brushError;
                }
                else
                {
                    lblResolution.Foreground = brushText;
                }
                if (string.IsNullOrWhiteSpace(txtDescription.Text))
                {
                    result = false;
                    lblDescription.Foreground = brushError;
                }
                else
                {
                    lblDescription.Foreground = brushText;
                }
            }
            else
            {
                result = false;
            }

            return result;
        }

        public int? SaveData()
        {
            string caller = cmbCaller.SelectedItem.ToString();
            string team = txtTeam.Text;
            string subject = cmbSubject.SelectedItem.ToString();
            string nature = cmbNature.SelectedItem.ToString();
            string resolution = cmbResoultion.SelectedItem.ToString();
            string description = txtDescription.Text;
            DateTime time = DateTime.Now;

            string title = "delete this field";

            try
            {
                string cmdString = "INSERT INTO [Calls] "
                + "(Caller, Team, Subject, Nature, Resolution, Title, Description, Time) "
                + "OUTPUT INSERTED.CallID "
                + "VALUES (@caller, @team, @subject, @nature, @resolution, @title, @description, @time)";
                            
                using (SqlConnection myConn = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand(cmdString, myConn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = myConn;
                    cmd.Parameters.AddWithValue("@caller", caller);
                    cmd.Parameters.AddWithValue("@team", team);
                    cmd.Parameters.AddWithValue("@subject", subject);
                    cmd.Parameters.AddWithValue("@nature", nature);
                    cmd.Parameters.AddWithValue("@resolution", resolution);
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.Parameters.AddWithValue("@time", time);

                    myConn.Open();
                    //cmd.ExecuteNonQuery();
                    int id = (int)cmd.ExecuteScalar();

                    return id;
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        private void PopulateControls()
        {
            string cmdString = "SELECT * FROM [Callers] ORDER BY (LName), (FName)";

            try
            {
                using (SqlConnection myConn = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand(cmdString, myConn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = myConn;

                    myConn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        string FName = dr[1].ToString().Trim();
                        string LName = dr[2].ToString().Trim();
                        cmbCaller.Items.Add(FName + " " + LName);
                        teamlist.Add(dr[3].ToString());
                    }
                    dr.Close();
                }

                cmbCaller.IsReadOnly = true;
                cmbCaller.Background = brushReadOnly;
                txtTeam.IsReadOnly = true;
                txtTeam.Background = brushReadOnly;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not connect to database. " + ex.Message.ToString(), "Error");
            }

            cmbSubject.Items.Add("Google Chrome");
            cmbSubject.Items.Add("Mozzila Firefox");
            cmbSubject.Items.Add("Microsoft Edge");

            cmbNature.Items.Add("Hardware");
            cmbNature.Items.Add("Software");
            cmbNature.Items.Add("Seeking advice");
            cmbNature.Items.Add("Non-standard issue");

            cmbResoultion.Items.Add("First call resolution");
            cmbResoultion.Items.Add("Awaiting further action");
            cmbResoultion.Items.Add("Awaiting further information");
            cmbResoultion.Items.Add("Unable to resolve");
            cmbResoultion.Items.Add("NABit article insufficient");

        }

        private void ResetWindow()
        {
            NewCall win = new NewCall();
            win.Show();
            this.Close();
        }

        #endregion

        #region Events

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Submitted = true;
            if (ValidateFields() == true)
            {
                int? id = SaveData();
                    if ((id != null))
                    {
                        MessageBox.Show("Call saved successfully\n\nReference No: " + id.ToString(), "Success");
                        ResetWindow();
                    }
                    else
                        MessageBox.Show("Could not connect to database", "Error");
            }
            else
                MessageBox.Show("Please complete all fields before submtting", "Error");
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetWindow();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            ValidateFields();
        }

        private void cmbSubject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ValidateFields();
        }

        private void cmbNature_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ValidateFields();
        }

        private void cmbResoultion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ValidateFields();
        }
        #endregion

        private void cmbCaller_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ValidateFields();

            txtTeam.Text = teamlist[cmbCaller.SelectedIndex];
        }

    }
}


