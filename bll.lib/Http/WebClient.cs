using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace bll.lib.Http
{
    public class WebClient : System.Net.WebClient
    {
        public WebClient()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            this.container = new CookieContainer();
        }
        public WebClient(CookieContainer container)
        {
            this.container = container;
        }

        public CookieContainer CookieContainer
        {
            get { return container; }
            set { container = value; }
        }

        private CookieContainer container = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest r = base.GetWebRequest(address);
            var request = r as HttpWebRequest;
            if (request != null)
            {
                request.CookieContainer = container;
            }
            return r;
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            try
            {
                WebResponse response = base.GetWebResponse(request, result);
                ReadCookies(response);
                return response;
            }
            catch (Exception ex)
            {

                throw new Exception("Response fail code: 54", ex);
            }
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            try
            {
                WebResponse response = base.GetWebResponse(request);
                ReadCookies(response);
                return response;
            }
            catch (Exception)
            {

                throw new Exception("Repository not found");
            }
        }

        private void ReadCookies(WebResponse r)
        {
            var response = r as HttpWebResponse;
            if (response != null)
            {
                CookieCollection cookies = response.Cookies;
                container.Add(cookies);
            }
        }
    }
}
