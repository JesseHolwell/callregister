using System;
using System.Collections.Generic;
using System.Data.OleDb;
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
    /// Interaction logic for Banker.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {        
        public ErrorWindow(Exception ex)
        {
            InitializeComponent();

            txtError.Text = string.Format(ex.ToString());

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
