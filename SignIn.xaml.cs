using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MemoryGameSecure
{
    /// <summary>
    /// Interaction logic for SignIn.xaml
    /// </summary>
    public partial class SignIn : Window
    {
        public SignIn()
        {
            InitializeComponent();
        }


        /*
         *  Take user to the Create Account Page and close the current one
         */
        private void btnCreateAccountClick(object sender, RoutedEventArgs e)
        {
            var CreateAccountWindow = new CreateAccount();
            CreateAccountWindow.Show();
            this.Close();
        }

        /*
         * on user pressing submit
         */
        private void btnSubmitClick(object sender, RoutedEventArgs e)
        {
            // Store the user inputs
            string email = txtEmail.Text;
            string password = psbPassword.Password.ToString();

            Boolean isPass = false;
            Boolean isEmail = false;

            // sanitize inputs
            email = sanitize(email);
            password = sanitize(password);

            // check regex
            isEmail = isGoodEmail(email, isEmail);
            isPass = isGoodPass(password, isPass);
            
            // If email and password meet regex call the database
            if (isEmail && isPass)
            {

                try
                {

                    SqlConnection conn = new SqlConnection("Data Source=localhost\\sqlexpress;Initial Catalog=Secure_2;Integrated Security=True");
                    using (SqlConnection newConn = conn)
                    {

                        int accountID = 0;

                        newConn.Open();

                        // search for record that contains both the given email and password
                        SqlCommand cmdSignIn = new SqlCommand("dbo.SignIn", newConn);
                        cmdSignIn.CommandType = System.Data.CommandType.StoredProcedure;
                        cmdSignIn.Parameters.AddWithValue("@email", email);
                        cmdSignIn.Parameters.AddWithValue("@password", password);
                        SqlDataReader rdrAccount = cmdSignIn.ExecuteReader();

                        while(rdrAccount.Read())
                        {
                            accountID = Convert.ToInt32(rdrAccount["id"]);
                        }

                        // if the account id is 0 it means the login was not successful
                        if (accountID == 0)
                        {
                            lblMessage.Content = "Eiter your email or password is incorrect";
                            lblMessage.Background = new SolidColorBrush(Color.FromRgb(255, 99, 71));
                        }

                        // if the accountID is > 0 that means a login was successful
                        if (accountID > 0)
                        {
                            // save the current User's ID for use in another window
                            GlobalVars.accountID = accountID;

                            // close the current window and open the dashboard
                            var DashWindow = new Dashboard();
                            DashWindow.Show();
                            this.Close();
                        }

                    }
                }
                catch (SqlException exp)
                {
                    throw new InvalidOperationException("Data could not be read", exp);
                }

            }
            // if email or password do not pass regex
            else
            {
                lblMessage.Content = "Either your email or password is incorrect";
                lblMessage.Background = new SolidColorBrush(Color.FromRgb(255, 99, 71));

            }

        }

        /*
         * sanitize the user's input from a black list of common SQL words
         */
        private static String sanitize(string s)
        {
            s = s.Replace("SELECT", "");
            s = s.Replace("DROP", "");
            s = s.Replace("DELETE", "");
            s = s.Replace("*", "");
            s = s.Replace("=", "");
            s = s.Replace("'", "");
            s = s.Replace("%", "");

            return s;
        }

        /*
         * check if user's email meets requirements
         */
        private static Boolean isGoodEmail(String email, Boolean isEmail)
        {
            Regex emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (emailRegex.IsMatch(email))
            {
                return true;
            }
            else { return false; }
        }

        /*
        * Check if the User's password meets requirements
        */
        private static Boolean isGoodPass(String password, Boolean isPass)
        {

            Regex passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,50}$");

            if (passwordRegex.IsMatch(password))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
