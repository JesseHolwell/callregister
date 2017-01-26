using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.OleDb;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace NAB_Register.Management
{
    /// <summary>
    /// Interaction logic for Reporting.xaml
    /// </summary>
    public partial class Reporting : Window
    {
        public string connectionString;

        public string senderUsername;
        public string senderPassword;

        private List<Data.Call> calls = new List<Data.Call>();
        private List<Data.Team> teams = new List<Data.Team>();

        private DateTime? selectedDate = new DateTime();

        private readonly Encoding _encoding = Encoding.Unicode;

        public Reporting(string connStr)
        {
            InitializeComponent();
            connectionString = connStr;
            GetCredentials();
        }

        private void GetCredentials()
        {
            string credData = "";

            try
            {
                if (File.Exists("Data\\reporting.txt"))
                {
                    using (StreamReader sr = new StreamReader("Data\\reporting.txt"))
                    {
                        credData = sr.ReadLine();
                    }

                    if (!string.IsNullOrWhiteSpace(credData) && credData.Contains(";"))
                    {
                        senderUsername = credData.Split(';')[0];
                        senderPassword = Unprotect(credData.Split(';')[1]);

                        txtEmail.Text = senderUsername;
                        txtPassword.Password = senderPassword;
                    }
                    else
                    {
                        txtEmail.Text = "";
                        txtPassword.Password = "";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problems while retreiving credentials", "Error");
            }
        }

        public void StoreCredentials()
        {
            try
            {
                string data = senderUsername + ";" + Protect(senderPassword);
                System.IO.File.WriteAllText("Data\\reporting.txt", data);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problems storing credentials", "Error");
            }
        }

        private void GetCallList()
        {
            calls.Clear();
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

                    calls.Add(call);
                }
                reader.Close();
                connection.Close();
            }
        }

        private void GetTeamList()
        {
            teams.Clear();

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
                        if (Convert.ToBoolean(reader[4]) == true)
                        {
                            teams.Add(new Data.Team(Convert.ToInt32(reader[0]), Convert.ToInt32(reader[1].ToString()), reader[2].ToString(), reader[3].ToString(), Convert.ToBoolean(reader[4])));
                        }
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errors while connecting to the database. Try again in a few moments or try setting the location again", "Error");
            }
        }

        public void ProcessReport()
        {
            selectedDate = dpDate.SelectedDate;
            if (selectedDate != null)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                //lblStatus.Content = "Processing call list";

                senderUsername = txtEmail.Text;
                senderPassword = txtPassword.Password;

                //sort calls
                foreach (Data.Team t in teams)
                {
                    //lblStatus.Content = "Processing report for " + t.Name + "...";

                    List<Data.Call> callList = new List<Data.Call>();
                    List<Data.Call> importantCalls = new List<Data.Call>();
                    List<Data.Call> otherCalls = new List<Data.Call>();

                    string attachment = "";

                    foreach (Data.Call c in calls)
                    {
                        DateTime callDate = Convert.ToDateTime(c.Time);
                        if (callDate.Date == selectedDate)
                        {
                            if (c.Team == t.ToString())
                            {
                                if (c.Important)
                                {
                                    importantCalls.Add(c);
                                }
                                else
                                {
                                    otherCalls.Add(c);
                                }
                            }
                        }
                    }
                    foreach (Data.Call c in importantCalls)
                    {
                        callList.Add(c);
                    }
                    foreach (Data.Call c in otherCalls)
                    {
                        callList.Add(c);
                    }

                    try
                    {
                        //make csv
                        string csvString = "";

                        if (importantCalls.Count > 0)
                        {
                            csvString = "Call ID,Time,User ID,Team,Banker,Product,Request,Feedback,Article,Comments,Important\n";
                            foreach (Data.Call c in importantCalls)
                            {
                                csvString += c.ToString();
                            }

                            string fileName = "Team " + t.Number + " - Banker Feedback Report " + selectedDate.Value.ToString("yyyyMMdd");
                            attachment = "Data\\" + fileName + ".csv";
                            System.IO.File.WriteAllText(attachment, csvString);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Errors while creating report\n" + ex.Message, "Error");
                    }

                    //lblStatus.Content = "Sending email to " + t.Email + "...";
                    SendEmail(t, attachment);

                    try
                    {
                        if (!string.IsNullOrEmpty(attachment))
                        {
                            System.IO.File.Delete(attachment);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Errors while deleting the reports\n" + ex.Message, "Error");
                    }
                }

                //lblStatus.Content = "Finished reporting.";
                Mouse.OverrideCursor = null;
                MessageBox.Show("Finished processing reports", "Info");
            }
            else
            {
                MessageBox.Show("Please select a date to process", "Error");
            }
        }

        private void SendEmail(Data.Team team, string file)
        {
            string to = team.Email;
            var input = new EmailAddressAttribute();

            if (!string.IsNullOrEmpty(to) && input.IsValid(to))
            {
                using (SmtpClient client = new SmtpClient())
                {
                    using (MailMessage mail = new MailMessage(senderUsername, to))
                    {
                        client.UseDefaultCredentials = false;
                        client.EnableSsl = true;
                        client.Credentials = new NetworkCredential(senderUsername, senderPassword);
                        client.Port = 25;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.Host = "smtp.gmail.com";

                        mail.Subject = "Team " + team.Number + " - Daily Banker Feedback Report";
                        mail.Body = "Please find attached your team's banker feedback report for " + selectedDate.Value.DayOfWeek + "\n";

                        try
                        {
                            if (!string.IsNullOrEmpty(file))
                            {
                                mail.Attachments.Add(new Attachment(file));
                            }
                            else
                            {
                                mail.Body += "There were no calls on this day for your team\n";
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Errors while attaching file\n" + ex.Message, "Error");
                            mail.Body += "There were errors attaching the file\n";
                        }

                        mail.Body += "\nRegards,\nRED Team";

                        try
                        {
                            client.Send(mail);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Errors while sending email\n" + ex.Message, "Error");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Errors attaching recipient\n" + "Email address is not valid: " + to, "Error");
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                GetCallList();
                GetTeamList();
                ProcessReport();
                StoreCredentials();
            }
            else
            {
                MessageBox.Show("Please enter an email address and password", "Error");
            }
        }

        public string Unprotect(string encryptedString)
        {
            byte[] protectedData = Convert.FromBase64String(encryptedString);
            byte[] unprotectedData = ProtectedData.Unprotect(protectedData,
                null, DataProtectionScope.CurrentUser);

            return _encoding.GetString(unprotectedData);
        }

        public string Protect(string unprotectedString)
        {
            byte[] unprotectedData = _encoding.GetBytes(unprotectedString);
            byte[] protectedData = ProtectedData.Protect(unprotectedData,
                null, DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(protectedData);
        }
    }
}