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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NAB_Register
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            //TODO: login stuff
        }

        private void btnNewCall_Click(object sender, RoutedEventArgs e)
        {
            //NewCall win = new NewCall();
            Home win = new Home();
            win.Show();
        }

        private void btnCallList_Click(object sender, RoutedEventArgs e)
        {
            CallList win = new CallList();
            win.Show();
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            AdminWindows.Settings win = new AdminWindows.Settings();
            win.ShowDialog();
        }
    }
}
