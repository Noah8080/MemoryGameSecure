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
    /// Interaction logic for CreateAccount.xaml
    /// </summary>
    public partial class CreateAccount : Window
    {
        public CreateAccount()
        {
            InitializeComponent();
        }

        // set value to use for checking entered email against ones already saved
        public Boolean uniqEmail = false;
        private void btnSubmitClick(object sender, RoutedEventArgs e)
        {
            // save the user inputs
            string Username = txtUsername.Text;
            string Email = txtEmail.Text;
            string Password = txtPassword.Text;
            string PassCheck = txtPassCheck.Text;

            // Sanitize the inputs
            Username = sanitize(Username);
            Email = sanitize(Email);
            Password = sanitize(Password);

            // Make sure email is in a valid form
            Boolean isEmailGood = false;
            isEmailGood = isGoodEmail(Email, isEmailGood);

            // if email is not in valid form
            if (!isEmailGood)
            {
                lblMessage.Content = "Please enter a valid email";
                lblMessage.Background = new SolidColorBrush(Color.FromRgb(255, 34, 199));
            }

            // Make sure email is not already in database
            if (isEmailGood)
            {
                try
                {


                    SqlConnection conn = new SqlConnection("Data Source=localhost\\sqlexpress;Initial Catalog=Secure_2;Integrated Security=True");
                    using (SqlConnection newConn = conn)
                    {

                        newConn.Open();

                        SqlCommand cmdEmailMatch = new SqlCommand("SELECT COUNT(*) FROM Accounts WHERE Email = '" + Email + "'", newConn);
                        int emailMatch = Convert.ToInt32(cmdEmailMatch.ExecuteScalar());

                        
                        if (emailMatch == 0)
                        {
                            uniqEmail = true;
                        }
                        else
                        {
                            lblMessage.Content = "This Email is already used by an account";
                            lblMessage.Background = new SolidColorBrush(Color.FromRgb(255, 34, 199));

                        }

                        newConn.Close();
                    }
                }
                catch (SqlException exp)
                {
                    throw new InvalidOperationException("Data could not be read", exp);
                }
            }
 

            // Make sure passwords match
            Boolean passMatch = false;
            if (Password == PassCheck)
            {
                passMatch = true;
            }
            else
            {
                lblMessage.Content = "Make sure your passwords match!";
                lblMessage.Background = new SolidColorBrush(Color.FromRgb(255, 34, 199));
            }

            // make sure password meets requirements
            Boolean isPassGood = false;

            isPassGood = isGoodPass(Password, isPassGood);

            if (!isPassGood)
            {
                lblMessage.Content = "Password must have 1 number, 1 special char, and lower and uppcase letter";
                lblMessage.Background = new SolidColorBrush(Color.FromRgb(255, 34, 199));
            }


            // if all the user inputs are good, connect to the database
            if (passMatch && isEmailGood && uniqEmail && isPassGood)
            {

                try
                {

                    SqlConnection conn = new SqlConnection("Data Source=localhost\\sqlexpress;Initial Catalog=Secure_2;Integrated Security=True");
                    using (SqlConnection newConn = conn)
                    {
                        newConn.Open();

                        // add account info to the database
                        SqlCommand addAcc = new SqlCommand("dbo.AddAccount", newConn);
                        addAcc.CommandType = System.Data.CommandType.StoredProcedure;
                        addAcc.Parameters.AddWithValue("@email", Email);
                        addAcc.Parameters.AddWithValue("@password", Password);
                        addAcc.Parameters.AddWithValue("@username", Username);
                        addAcc.ExecuteNonQuery();

                        // validation for user
                        lblMessage.Content = "Account has been created! go to sign in page";
                        lblMessage.Background = new SolidColorBrush(Color.FromRgb(34, 190, 34));

                        // disable input fields
                        txtPassword.Visibility = Visibility.Hidden;
                        txtUsername.Visibility = Visibility.Hidden;
                        txtEmail.Visibility = Visibility.Hidden;
                        txtPassCheck.Visibility = Visibility.Hidden;


                    }

                }
                catch (SqlException exp)
                {
                    throw new InvalidOperationException("Data could not be added", exp);
                }

            }

        }


        /*
         * Sanitize username
         */
        private static String sanitize(string username)
        {
            // blacklist sql words
            username = username.Replace("SELECT", "");
            username = username.Replace("DROP", "");
            username = username.Replace("DELETE", "");
            username = username.Replace("*", "");
            username = username.Replace("=", "");
            username = username.Replace("'", "");
            username = username.Replace("%", "");

            return username;
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

        /*
        * Check if the User's email meets requirements
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
         * takes user to sign in page
         */
        private void btnGoToSignInClick(object sender, RoutedEventArgs e)
        {
            var SignInWindow = new SignIn();
            SignInWindow.Show();
            this.Close();
        }
    }
}
