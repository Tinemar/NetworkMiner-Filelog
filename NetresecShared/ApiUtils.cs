using System;
using System.Collections.Generic;
using System.Text;

namespace NetresecShared
{
    public class ApiUtils {

        public static Version GetLatestVersion(string productCode, out string releasePost, out string downloadUrl) {
#if DEBUG
            //string requestURL = "http://localhost:57978/updatecheck.ashx?l=" + System.Web.HttpUtility.UrlEncode(productCode);
            string requestURL = "https://www.netresec.com/updatecheck.ashx?l=" + System.Web.HttpUtility.UrlEncode(productCode);
#else
            string requestURL = "https://www.netresec.com/updatecheck.ashx?l=" + System.Web.HttpUtility.UrlEncode(productCode);
#endif

            System.Net.HttpWebRequest request = System.Net.WebRequest.Create(requestURL) as System.Net.HttpWebRequest;

            string versionString;
            //using (System.IO.TextReader reader = new System.IO.StreamReader(resultStream)) {

            using (System.Net.WebResponse response = request.GetResponse()) {
                using (System.IO.Stream stream = response.GetResponseStream()) {
                    using (System.IO.TextReader reader = new System.IO.StreamReader(stream)) {

                        versionString = reader.ReadLine();
                        releasePost = reader.ReadLine();
                        downloadUrl = reader.ReadLine();
                    }
                }
            }
            return Version.Parse(versionString);
        }
    }
}
