using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
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
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {

        public string username = "";
        // stopwatch for timing answers
        public Stopwatch sw = new Stopwatch();
        // store the saved number that will be guessed
        public Double answer = 0;
        // store the users's guess input
        public Double userGuess = 0;
        // store the time past on the question
        public Double PastTime = 0;


        public Dashboard()
        {
            InitializeComponent();

            Loaded += Page_Load;


            // add options for quantity drop down
            string[] quantity = { "7", "10", "12", "15"};
            foreach (var item in quantity)
            {
                cmbLengthSelector.Items.Add(item);
            }
            // set default value
            cmbLengthSelector.SelectedItem = "7";
        }

        /*
         * on page load request for the username
         */
        protected void Page_Load (object sender, EventArgs e)
        {

            // write current user's ID to program
            int userID = 0;
            userID = GlobalVars.accountID;

            // collapse buttons used for game
            btnSubmitGuess.Visibility = Visibility.Collapsed;
            btnSaveResult.Visibility = Visibility.Collapsed;

            // get the current user's username from the database
            try
            {
                SqlConnection conn = new SqlConnection("Data Source=localhost\\sqlexpress;Initial Catalog=Secure_2;Integrated Security=True");
                using (SqlConnection newConn = conn)
                {
                    newConn.Open();

                    SqlCommand cmdGetName = new SqlCommand("dbo.getUsername", newConn);
                    cmdGetName.CommandType = System.Data.CommandType.StoredProcedure;
                    cmdGetName.Parameters.AddWithValue("@id",userID);
                    SqlDataReader rdrUsername = cmdGetName.ExecuteReader();

                    while (rdrUsername.Read())
                    {
                        // set username equal one found in database
                        username = Convert.ToString(rdrUsername["username"]);
                    }
                    GlobalVars.accountUsername = username;

                    newConn.Close();

                    // title heading 
                    lblHeading.Content = "Welcome " + username + " to a memory game";
                    // prompt for the user
                    lblSelector.Content = "Select the number of digits you would like to try to memorize";


                }
            }
            catch (SqlException exp)
            {
                throw new InvalidOperationException("Data could not be read", exp);
            }


        }

        private void btnStartClick(object sender, RoutedEventArgs e)
        {

            string selected = cmbLengthSelector.Text;
            // stores the length the user would like as an int
            int numLen = int.Parse(selected);
            lblShowNumLen.Content = numLen.ToString() + " digits";

            // gets the radnom number to be memorized
            answer = GenerateRandomNumber(numLen);
            lblDisplayAnswer.Content = answer.ToString();

            btnStartGuess.Visibility = Visibility.Visible;
            lblPoint.Visibility = Visibility.Visible;
            btnStart.Visibility = Visibility.Hidden;

            // start a counter to time the user
            sw.Start();



        }

        /*
         * create a random number with a variable number of digits
         */
        private static Double GenerateRandomNumber(int desiredDigits)
        {
            Random rnd = new Random();

            string hld = "";
            double put = 0;

            //for the deseried length to memorize
            for (int i = 0; i < desiredDigits; i++)
            {
                int h;
                //get a random number
                h = rnd.Next(10); 

                // add it to the previous numbers, saved as a string
                hld = hld + h.ToString();

            }

            // return it as a double
            put = Double.Parse(hld);
            return put;

        }

        /* When the user thinks they have the number memorized,
         * hide the label displaying the number and show the 
         * box for them to enter their answer in and the submit button
         * make the start guess button hidden
         */
        private void btnStartGuessClick(object sender, RoutedEventArgs e)
        {
            lblDisplayAnswer.Visibility = Visibility.Hidden;
            txtResponse.Visibility = Visibility.Visible;
            btnSubmitGuess.Visibility = Visibility.Visible;
            btnStartGuess.Visibility = Visibility.Hidden;
        }

        /*
         * compare the user's guess with the displayed number
         */
        private void btnSubmitGuessClick(object sender, RoutedEventArgs e)
        {
            // save the user's guess
            string holdInput = txtResponse.Text;

            // make sure their input contains only numbers
            // if it doesn't prompt them for a good input
            Regex regex = new Regex(@"^[0-9]*$");
            if (!regex.IsMatch(holdInput))
            {
                lblWarning.Visibility = Visibility.Visible;
                lblWarning.Content = "Your input must be numbers only";
                lblWarning.Background = new SolidColorBrush(Color.FromRgb(255, 51, 92));


            }
            // if is is only numbers
            else
            {
                // stop the stopwatch and find elapsed time 
                sw.Stop();
                PastTime = sw.Elapsed.TotalSeconds;

                // clear warning if it is opened from a previous guess
                lblWarning.Visibility = Visibility.Hidden;

                // show user time take on question
                lblShowTime.Visibility = Visibility.Visible;
                lblShowTime.Content = "seconds past was " + PastTime.ToString();
                // hide the submit guess button once they have made their guess
                btnSubmitGuess.Visibility = Visibility.Hidden;

                // store the user's response
                double userResponse = double.Parse(holdInput);

                // if it matches the stored number
                if (userResponse == answer)
                {
                    lblReveal.Content = "You were correct!";
                    lblReveal.Background = new SolidColorBrush(Color.FromRgb(50, 205, 50));
                    lblDisplayAnswer.Visibility = Visibility.Visible;
                    //display save results button
                    btnSaveResult.Visibility = Visibility.Visible;
                }
                else
                {
                    lblReveal.Content = "You were NOT correct!";
                    lblReveal.Background = new SolidColorBrush(Color.FromRgb(255, 99, 71));
                    lblDisplayAnswer.Visibility = Visibility.Visible;
                }
            }

        }


        /*
         * close the window and end the progam
         */
        private void btnExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /*
         * reload page
         */
        private void btnPlayAgainClick(object sender, RoutedEventArgs e)
        {

            var playAgain = new Dashboard();
            playAgain.Show();
            this.Close();
 
        }
        /*
         * take user to the hope page
         */
        private void btnGoHomeClick(object sender, RoutedEventArgs e)
        {
            var homePage = new MainWindow();
            homePage.Show();
            this.Close();
        }

        /* save the user's ID, the number memorized and the time elapsed
         * to the Results table in the database.
         */
        private void btnSaveResultClick(object sender, RoutedEventArgs e)
        {
            // save the numbers as string to add to database
            string holdNum = answer.ToString();
            int userID = GlobalVars.accountID;

            // add result to the database
            try
            {
                SqlConnection conn = new SqlConnection("Data Source=localhost\\sqlexpress;Initial Catalog=Secure_2;Integrated Security=True");
                using (SqlConnection newConn = conn)
                {
                    newConn.Open();

                    SqlCommand cmdAddResult = new SqlCommand("dbo.AddResult", newConn);
                    cmdAddResult.CommandType = System.Data.CommandType.StoredProcedure;
                    cmdAddResult.Parameters.AddWithValue("@id", userID);
                    cmdAddResult.Parameters.AddWithValue("@number", holdNum);
                    cmdAddResult.Parameters.AddWithValue("@seconds", PastTime);
                    cmdAddResult.ExecuteNonQuery();
                }
            }
            catch (SqlException exp)
            {
                throw new InvalidOperationException("Data could not be added", exp);
            }
            btnSaveResult.Visibility = Visibility.Collapsed;

        }

        // take user to the results page and close the dashboard/ game page
        private void btnViewResultsClick(object sender, RoutedEventArgs e)
        {
            var resultsWindow = new Results();
            resultsWindow.Show();
            this.Close();

        }
    }
}
