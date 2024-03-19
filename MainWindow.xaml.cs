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

namespace MemoryGameSecure
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCreateAccountClick(object sender, RoutedEventArgs e)
        {
            // open the create account window and close the current one
            var CreateAccountWindow = new CreateAccount();
            CreateAccountWindow.Show();

            this.Close();
        }

        private void btnSignInClick(object sender, RoutedEventArgs e)
        {
            // open the sign in window and close the current one
            var SignInWindow = new SignIn();
            SignInWindow.Show();
            this.Close();
        }
    }
}
