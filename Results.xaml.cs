using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace MemoryGameSecure
{
    /// <summary>
    /// Interaction logic for Results.xaml
    /// </summary>
    public partial class Results : Window
    {
        public Results()
        {
            InitializeComponent();
            Loaded += Page_Load;

        }

        /* when the page is loaded
         * make requests to the database saved results
         */
        protected void Page_Load(object sender, EventArgs e)
        {
            // bring in global vars
            string userNameCur = GlobalVars.accountUsername;
            int userID = GlobalVars.accountID;

            lblHeader.Content = "Welcome " + userNameCur + ", here are your saved results";

            try
            {

                SqlConnection conn = new SqlConnection("Data Source=localhost\\sqlexpress;Initial Catalog=Secure_2;Integrated Security=True");
                using (SqlConnection newConn = conn)
                {
                    newConn.Open();

                    SqlCommand cmdGetResults = new SqlCommand("dbo.getResult", newConn);
                    cmdGetResults.CommandType = System.Data.CommandType.StoredProcedure;
                    cmdGetResults.Parameters.AddWithValue("@id", userID);
                    SqlDataReader reader = cmdGetResults.ExecuteReader();


                    string str = "";
                    while (reader.Read())
                    {
                        // save results from the database
                        string number = Convert.ToString(reader["number"]);
                        string timePased = Convert.ToString(reader["seconds"]);

                        // concat the results
                        // TODO: Get  XLM new line tage to work in adding to string
                        str += "With the number: ";
                        str += number;
                        str += ". You took ";
                        str += timePased;
                        str += " seconds.";
                        str += "<lb>";
                        
                        

                    }

                    lblResults.Content = str;

                }

            }
            catch (SqlException exp)
            {
                throw new InvalidOperationException("Data could not be read", exp);
            }

        }



        /*
         * return user to game window/ dashboard
         */
        private void btnReturnToGameClick(object sender, RoutedEventArgs e)
        {
            var gameWindow = new Dashboard();
            gameWindow.Show();
            this.Close();
        }

        /* sign user out and take them to sign in page
         */
        private void btnSignOutClick(object sender, RoutedEventArgs e)
        {
            // clear user's id
            GlobalVars.accountID = 0;
            var signInWindow = new SignIn();
            signInWindow.Show();
            this.Close();
        }

        // clear saved id and exit app
        private void btnExitClick(object sender, RoutedEventArgs e)
        {
            GlobalVars.accountID = 0;
            this.Close();

        }
    }
}
