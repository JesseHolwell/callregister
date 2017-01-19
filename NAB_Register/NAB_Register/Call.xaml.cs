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

namespace NAB_Register
{
    /// <summary>
    /// Interaction logic for Call.xaml
    /// </summary>
    public partial class Call : Window
    {
        public Call(Data.Call call)
        {
            InitializeComponent();
            try
            {
                txtID.Text = call.CallID.ToString();
                txtTime.Text = call.Time.ToString();
                txtUser.Text = call.UserID.ToString();
                txtTeam.Text = call.Team.ToString();
                txtBanker.Text = call.Banker.ToString();
                txtProduct.Text = call.Product.ToString();
                txtRequest.Text = call.Request.ToString();
                txtFeedback.Text = call.Feedback.ToString();
                txtArticle.Text = call.Article.ToString();
                txtComments.Text = call.Comments.ToString();

                if (!call.Important)
                {
                    lblImportant.Visibility = Visibility.Collapsed;
                }

                if (string.IsNullOrWhiteSpace(txtArticle.Text))
                {
                    txtArticle.Visibility = Visibility.Collapsed;
                    lblArticle.Visibility = Visibility.Collapsed;
                }

                this.Title += call.CallID.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Issues opening the call\n" + ex.ToString(), "Error");
            }

        }
    }
}
