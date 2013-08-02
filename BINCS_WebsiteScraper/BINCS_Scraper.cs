using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Net;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

/*******************************************************************************
Web Scraper for Defense Logicstics Information Service CAGE Codes
 *******************************************************************************/

namespace DLA_Scraper
{
    public class DLA
    {
        public string Company_Name { get; set; }
        public string Address { get; set; }
        public string CAO_ADP { get; set; }




        public bool status { get; set; }

        //Search Engine URLs
        private string DLASearch = "https://www.logisticsinformationservice.dla.mil/BINCS/details.aspx?CAGE=";

        //Constructor
        public DLA(string p_CageCode)
        {
            new DLA(p_CageCode, true);
        }

        //Default parameter specifiers are not permitted
        public DLA(string p_CageCode, bool GetExtraInfo)
        //public BINCS(string MovieName, bool GetExtraInfo = true)
        {
            string BINCSUrl = generateUrl(System.Uri.EscapeUriString(p_CageCode));
            status = false;
            if (!string.IsNullOrEmpty(BINCSUrl))
            {
                parseResultPage(BINCSUrl, GetExtraInfo);
            }
        }

        //Get BINCS URL from search results
        private string generateUrl(string p_CageCode)
        {
            string searchEngine = "DLASearch";
            return generateUrl(p_CageCode, searchEngine);
        }

        private string generateUrl(string p_CageCode, string p_searchEngine)
        //private string getBINCSUrl(string MovieName, string searchEngine = "google")
        {
            string url = DLASearch + p_CageCode;

            return url;
        }

        //Parse BINCS page data
        private void parseResultPage(string p_url, bool GetExtraInfo)
        {
            string temp;
            Match m1;

            string html = getUrlData(p_url);
            //html = stripCode(html);

            string s_no_matching_result_pattern = "<form name=\"aspnetForm\" method=\"post\" action=\"begin_search.aspx\" id=\"aspnetForm\">";
            //string s_noResult
            m1 = Regex.Match(html, s_no_matching_result_pattern);

            if (m1.Success)
            {
                throw new ArgumentException("No result found for this CAGE code OR BINCs website has changed output.");
            }

            string s_company_pre_pattern = "Company Name:</label>\\s*</td>\\s*<td>\\s*<span id\\s*=\\s*[\"'].*cphMainPageBody_lblCompNameData[\"']>";
            string s_company_post_pattern = "</span>";
            string s_company_pattern = s_company_pre_pattern + "(?<1>.*)" + s_company_post_pattern;
            m1 = Regex.Match(html, s_company_pattern);
            temp = Regex.Replace(m1.Value, s_company_pre_pattern, string.Empty);
            temp = temp.Remove(temp.IndexOf(s_company_post_pattern));
            Company_Name = temp;


            string s_address_pre_pattern = "Address:</label>\\s*</td>\\s*<td>\\s*<span id\\s*=\\s*[\"'].*_cphMainPageBody_lblAddressData[\"']>";
            string s_address_post_pattern = "</span>";
            string s_address_pattern = s_address_pre_pattern + "(?<1>.*)" + s_address_post_pattern;
            m1 = Regex.Match(html, s_address_pattern);
            temp = Regex.Replace(m1.Value, s_address_pre_pattern, string.Empty);
            temp = temp.Remove(temp.IndexOf(s_address_post_pattern));
            Address = temp; ;


            string s_CAO_ADP_pre_pattern = "CAO-ADP:</label>\\s*</td>\\s*<td>\\s*<span id\\s*=\\s*[\"'].*_cphMainPageBody_lblCaoAdpData[\"']>";
            string s_CAO_ADP_post_pattern = "</span>";
            string s_CAO_ADP_pattern = s_CAO_ADP_pre_pattern + "(?<1>.*)" + s_CAO_ADP_post_pattern;
            m1 = Regex.Match(html, s_CAO_ADP_pattern);
            temp = Regex.Replace(m1.Value, s_CAO_ADP_pre_pattern, string.Empty);
            temp = temp.Remove(temp.IndexOf(s_CAO_ADP_post_pattern));
            CAO_ADP = temp;

        }


        public string stripCode(string the_html)
        {
            // Remove google analytics code and other JS
            the_html = Regex.Replace(the_html, "<script.*?</script>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            // Remove inline stylesheets
            the_html = Regex.Replace(the_html, "<style.*?</style>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            // Remove HTML tags
            the_html = Regex.Replace(the_html, "</?[a-z][a-z0-9]*[^<>]*>", "");
            // Remove HTML comments
            the_html = Regex.Replace(the_html, "<!--(.|\\s)*?-->", "");
            // Remove Doctype
            the_html = Regex.Replace(the_html, "<!(.|\\s)*?>", "");
            // Remove excessive whitespace
            the_html = Regex.Replace(the_html, "[\t\r\n]", " ");

            return the_html;
        }

        /*******************************[ Helper Methods ]********************************/

        //Match single instance
        private string match(string regex, string html)
        {
            int i = 1;
            return match(regex, html, i);
        }

        private string match(string regex, string html, int i)
        //private string match(string regex, string html, int i = 1)
        {
            return new Regex(regex, RegexOptions.Multiline).Match(html).Groups[i].Value.Trim();
        }

        //Match all instances and return as ArrayList
        private ArrayList matchAll(string regex, string html)
        {
            int i = 1;
            return matchAll(regex, html, i);
        }

        private ArrayList matchAll(string regex, string html, int i)
        //private ArrayList matchAll(string regex, string html, int i = 1)
        {
            ArrayList list = new ArrayList();
            foreach (Match m in new Regex(regex, RegexOptions.Multiline).Matches(html))
                list.Add(m.Groups[i].Value.Trim());
            return list;
        }

        //Strip HTML Tags
        static string StripHTML(string inputString)
        {
            return Regex.Replace(inputString, @"<.*?>", string.Empty);
        }

        //Get URL Data
        private string getUrlData(string url)
        {
            WebClient client = new WebClient();
            Random r = new Random();
            //Random IP Address
            client.Headers["X-Forwarded-For"] = r.Next(0, 255) + "." + r.Next(0, 255) + "." + r.Next(0, 255) + "." + r.Next(0, 255);
            //Random User-Agent
            client.Headers["User-Agent"] = "Mozilla/" + r.Next(3, 5) + ".0 (Windows NT " + r.Next(3, 5) + "." + r.Next(0, 2) + "; rv:2.0.1) Gecko/20100101 Firefox/" + r.Next(3, 5) + "." + r.Next(0, 5) + "." + r.Next(0, 5);


            //Problem to solve: The underlying connection was closed: Could not establish trust relationship for the SSL/TLS secure channel.
            //To by-pass create a new ServerCertificateValidationCallback 
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };


            Stream datastream = client.OpenRead(url);
            StreamReader reader = new StreamReader(datastream);
            StringBuilder sb = new StringBuilder();
            while (!reader.EndOfStream)
                sb.Append(reader.ReadLine());
            return sb.ToString();
        }
    }
}