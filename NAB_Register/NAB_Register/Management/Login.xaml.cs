using System.Windows;

namespace NAB_Register.Management
{
    /// <summary>
    ///     Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        //TOOD: hide this!
        private const string user = "admin";

        private const string password = "admin";
        public bool success;

        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (txtUsername.Text.ToUpper() == user.ToUpper() && txtPassword.Password == password)
            {
                success = true;
                Close();
            }
            else
            {
                MessageBox.Show("Invalid Credentials", "Error");
            }
        }
    }
}