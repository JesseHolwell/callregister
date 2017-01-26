using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Input;

namespace NAB_Register.Management
{
    /// <summary>
    /// Interaction logic for Request.xaml
    /// </summary>
    public partial class Request : Window
    {
        public string connectionString;
        private Data.Request request;
        private List<Data.Product> products = new List<Data.Product>();

        public Request(string connStr, object r)
        {
            InitializeComponent();
            connectionString = connStr;
            GetDataItems();
            chkActive.Visibility = Visibility.Visible;

            if (r == null)
            {
                chkActive.Visibility = Visibility.Collapsed;
                chkActive.IsChecked = true;
            }
            else
            {
                request = r as Data.Request;
                txtRequestName.Text = request.Name;
                cmbProduct.Text = request.Product;
                chkActive.IsChecked = request.IsActive;
            }
        }

        private void GetDataItems()
        {
            products.Clear();
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    string query = "SELECT * FROM Product";

                    OleDbCommand command = new OleDbCommand(query, connection);
                    connection.Open();

                    OleDbDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (Convert.ToBoolean(reader[2]))
                        {
                            products.Add(new Data.Product(null, reader[1].ToString(), null));
                        }
                    }
                    reader.Close();
                }

                cmbProduct.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errors while connecting to the database. Try again in a few moments or try setting the location again", "Error");
            }

            Mouse.OverrideCursor = null;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtRequestName.Text) && cmbProduct.SelectedItem != null)
            {
                Mouse.OverrideCursor = Cursors.Wait;

                string query = "";
                int? id = null;

                if (request == null)
                {
                    query = "INSERT into Request (RequestName, Product, IsActive) VALUES (@RequestName, @Product, @IsActive)";
                }
                else
                {
                    id = request.ID;
                    query = "UPDATE Request SET RequestName = ? , Product = ? , IsActive = ? WHERE RequestID = ? ";
                }

                request = new Data.Request(id, txtRequestName.Text, cmbProduct.SelectedItem.ToString(), Convert.ToBoolean(chkActive.IsChecked));

                try
                {
                    using (OleDbConnection conn = new OleDbConnection(connectionString))
                    {
                        OleDbCommand cmd = new OleDbCommand(query, conn);
                        conn.Open();

                        if (request.ID != null)
                        {
                            cmd.Parameters.AddWithValue("@RequestName", request.Name);
                            cmd.Parameters.AddWithValue("@Product", request.Product);
                            cmd.Parameters.AddWithValue("@IsActive", request.IsActive);

                            cmd.Parameters.AddWithValue("@RequestID", request.ID);
                        }
                        else
                        {
                            cmd.Parameters.Add("@Name", OleDbType.VarChar).Value = request.Name;
                            cmd.Parameters.Add("@Product", OleDbType.VarChar).Value = request.Product;
                            cmd.Parameters.Add("@IsActive", OleDbType.Boolean).Value = request.IsActive;
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