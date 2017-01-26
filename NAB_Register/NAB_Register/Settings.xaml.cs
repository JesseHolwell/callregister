using Microsoft.Win32;
using NAB_Register.Management;
using System;
using System.IO;
using System.Windows;

namespace NAB_Register
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnChangeDatabase_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "All Files (*.*)|*.*";
            choofdlog.FilterIndex = 1;
            choofdlog.Multiselect = true;
            string fileName = "";

            try
            {
                if (choofdlog.ShowDialog() == true)
                {
                    if (choofdlog.FileName != null)
                    {
                        if (Directory.Exists("Data") == false)
                        {
                            Directory.CreateDirectory("Data");
                        }

                        fileName = choofdlog.FileName;
                        System.IO.File.WriteAllText("Data\\config.txt", fileName);
                        MessageBox.Show("Database location has been set", "Success");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was a problem setting the database location", "Error");
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

        private void btnManageBankers_Click(object sender, RoutedEventArgs e)
        {
            Bankers win = new Bankers(GetConnectionString());
            win.ShowDialog();
        }

        private void btnManageProducts_Click(object sender, RoutedEventArgs e)
        {
            Products win = new Products(GetConnectionString());
            win.ShowDialog();
        }

        private void btnManageRequests_Click(object sender, RoutedEventArgs e)
        {
            Requests win = new Requests(GetConnectionString());
            win.ShowDialog();
        }

        private void btnManageFeedbacks_Click(object sender, RoutedEventArgs e)
        {
            Feedbacks win = new Feedbacks(GetConnectionString());
            win.ShowDialog();
        }

        private void btnManageTeams_Click(object sender, RoutedEventArgs e)
        {
            Teams win = new Teams(GetConnectionString());
            win.ShowDialog();
        }

        private void btnReporting_Click(object sender, RoutedEventArgs e)
        {
            Reporting win = new Reporting(GetConnectionString());
            win.ShowDialog();
        }
    }
}