using System;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Input;

namespace NAB_Register.Management
{
    /// <summary>
    /// Interaction logic for Product.xaml
    /// </summary>
    public partial class Product : Window
    {
        public string connectionString;
        private Data.Product product;

        public Product(string connStr, object p)
        {
            InitializeComponent();
            connectionString = connStr;
            chkActive.Visibility = Visibility.Visible;

            if (p == null)
            {
                chkActive.Visibility = Visibility.Collapsed;
                chkActive.IsChecked = true;
            }
            else
            {
                product = p as Data.Product;
                txtProductName.Text = product.Name;
                chkActive.IsChecked = product.IsActive;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                Mouse.OverrideCursor = Cursors.Wait;

                string query = "";
                int? id = null;

                if (product == null)
                {
                    query = "INSERT into Product (ProductName, IsActive) VALUES (@ProductName, @IsActive)";
                }
                else
                {
                    id = product.ID;
                    query = "UPDATE product SET ProductName = ? , IsActive = ? WHERE ProductID = ? ";
                }

                product = new Data.Product(id, txtProductName.Text, Convert.ToBoolean(chkActive.IsChecked));

                try
                {
                    using (OleDbConnection conn = new OleDbConnection(connectionString))
                    {
                        OleDbCommand cmd = new OleDbCommand(query, conn);
                        conn.Open();

                        if (product.ID != null)
                        {
                            cmd.Parameters.AddWithValue("@ProductName", product.Name);
                            cmd.Parameters.AddWithValue("@IsActive", product.IsActive);
                            cmd.Parameters.AddWithValue("@ProductID", product.ID);
                        }
                        else
                        {
                            cmd.Parameters.Add("@ProductName", OleDbType.VarChar).Value = product.Name;
                            cmd.Parameters.Add("@IsActive", OleDbType.Boolean).Value = product.IsActive;
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