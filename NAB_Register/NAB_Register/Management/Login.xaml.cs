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

namespace NAB_Register.Management
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public bool success = false;
        private const string user = "nabsupport";
        private const string password = "redteam2016";

        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (txtUsername.Text.ToString().ToUpper() == user.ToUpper() && txtPassword.Password.ToString() == password)
            {
                success = true;
                this.Close();
            }
            else
                MessageBox.Show("Invalid Credentials", "Error");

        }
    }
}
