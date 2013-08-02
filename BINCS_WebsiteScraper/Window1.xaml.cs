using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Threading;
using System.Linq.Expressions;
using System.Reflection;

namespace WpfApplication_WebSiteScraper21
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }



        public delegate void UpdateTextCallback(string message);


        private void TimerThread()
        {
            DateTime dt_start = DateTime.Now;
            int n_frequency_times_per_second = 24;
            int n_sleep_interval_period = Convert.ToInt32(Math.Round(Convert.ToDecimal(1000 / n_frequency_times_per_second)));
            int MAX_INTEVALS = 100000000;
            for (int i = 0; i <= MAX_INTEVALS; i++)
            //while (i<= MAX_INTEVALS)
            {
                Thread.Sleep(n_sleep_interval_period);

                //Calculate ELASPED seconds
                Decimal n_elapsed_seconds = Convert.ToDecimal(DateTime.Now.Ticks - dt_start.Ticks) / 10000000;

                string s_message = string.Format("Elapsed Time: {0:N2}", n_elapsed_seconds);
                textBlock1.Dispatcher.Invoke(
                    new UpdateTextCallback(this.UpdateText),
                    new object[] { s_message }


                );
            }
        }


        private void TestThread(string p_input)
        {

            Thread my_TimerThread = new Thread(new ThreadStart(TimerThread));

            my_TimerThread.Start();
            string s_results = string.Empty;
            try
            {
                s_results = BINCS_Lookup(p_input);
            }
            catch (Exception ex)
            {
                s_results = ex.Message;
            }
            finally
            {
                textBlock1.Dispatcher.Invoke(
                    new UpdateTextCallback(this.UpdateTextAndEnableButton),
                   new object[] { s_results }

                );

                my_TimerThread.Abort();
            }
        }

        private void UpdateText(string message)
        {
            textBlock1.Text = message;
        }

        private void UpdateTextAndEnableButton(string message)
        {
            textBlock1.Text = message;
            button1.IsEnabled = true;
            button1.Content = "Lookup Defense Logistics Info";
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

            string s_movie_name = textBox_Title_Search.Text.Trim();
            if (!String.IsNullOrEmpty(s_movie_name))
            {
                button1.Content = "Searching...";
                button1.IsEnabled = false;
                Thread test = new Thread(() => TestThread(s_movie_name));
                test.Start();
            }
            else
            {
                MessageBox.Show("Please input a title to search");
            }
        }




        private string IMDB_Lookup(string s_movie_name)
        {
            IMDb_Scraper.IMDb my_scraper = new IMDb_Scraper.IMDb(s_movie_name, false);


            StringBuilder sb_output = new StringBuilder();

            sb_output.Append(my_scraper.Title);
            sb_output.Append("\n");
            sb_output.Append(my_scraper.Year);
            return sb_output.ToString();
        }

        private string BINCS_Lookup(string s_cage_code)
        {
            DLA_Scraper.DLA my_scraper = new DLA_Scraper.DLA(s_cage_code, false);

            StringBuilder sb_output = new StringBuilder();

            sb_output.Append(my_scraper.Company_Name);
            sb_output.Append("\n");
            sb_output.Append(my_scraper.Address);
            sb_output.Append("\n");
            sb_output.Append(my_scraper.CAO_ADP);

            return sb_output.ToString().Replace("<br />", "\n");
        }
    }
}
