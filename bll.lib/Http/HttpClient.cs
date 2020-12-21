using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace bll.lib.Http
{
    /// <summary>
    ///  Perform the requests and read the repositories
    /// </summary>
    public class HttpClient
    {
        public string url { get; set; }

        public HttpClient()
        {

        }

       

       
        public string getUrlContent(string url)
        {
            return this._get((string.IsNullOrEmpty(url)?this.url:url));
        }

        private string _get(string url)
        {
            Uri address = new Uri(url);

            using (WebClient webClient = new WebClient())
            {
                try
                {
                    var stream = webClient.OpenRead(address);

                    using (StreamReader sr = new StreamReader(stream))
                    {
                        var page = sr.ReadToEnd();
                        return page.ToString();
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }
    }
}
