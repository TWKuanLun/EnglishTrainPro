using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EnglishTrainPro.IO
{
    static class Download
    {
        public static bool WebDownloadFile(string source, string destination)
        {
            bool success = true;
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("User-Agent: Other");
                try
                {
                    client.DownloadFile(new Uri(source), destination);
                }
                catch (Exception)
                {
                    success = false;
                }
            }
            return success;
        }

        public static bool IsURLExist(string url)
        {
            try
            {
                WebRequest req = WebRequest.Create(url);

                WebResponse res = req.GetResponse();
            }
            catch (WebException)
            {
                return false;
            }
            return true;
        }
    }
}
