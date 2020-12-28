using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using nHttp = System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Threading;

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
            try
            {
                return this._get((string.IsNullOrEmpty(url) ? this.url : url));
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string _Get(string EndPoint, string parametro)
        {
            try
            {
                var run = Task.Run(() => this._get(EndPoint, parametro));

                return run.Result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private async Task<string> _get(string EndPoint, string parametro)
        {
            try
            {
                using (var httpClient = new nHttp.HttpClient { BaseAddress = new Uri(EndPoint) })
                {
                    using (var response = await httpClient.GetAsync("?url="+ parametro))
                    {
                        string responseData = await response.Content.ReadAsStringAsync();

                        return responseData;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
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

        public async Task<string> _post(string url, string EndPoint, string url_repo)
        {
            try
            {
                using (var httpClient = new nHttp.HttpClient { BaseAddress = new Uri(url) })
                {
                    Thread.Sleep(100);

                    var p = new List<KeyValuePair<string, string>>
                                    {
                                        new KeyValuePair<string, string>("url_repo", url_repo)
                                    };

                    using (var content = new nHttp.FormUrlEncodedContent(p))
                    {
                        Thread.Sleep(100);
                        using (var response = await httpClient.PostAsync(EndPoint, content))
                        {
                            Thread.Sleep(100);
                            string responseData = await response.Content.ReadAsStringAsync();

                            //Log

                            return responseData;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("The request could not be executed.Reason:" + ex.Message, ex);
            }
        }
    }
}
