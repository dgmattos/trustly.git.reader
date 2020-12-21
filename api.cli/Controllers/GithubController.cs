using bll.lib.Http;
using bll.lib.RepoReader;
using bll.lib.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace api.cli.Controllers
{
    public class GithubController : ApiController
    {

        // POST api/Github
        public Object Post(string url_repo)
        {
            FlieStorage fs = new FlieStorage(HttpContext.Current.Server.MapPath("~/App_Data/"));

            RepoReader rr = new RepoReader(url_repo, fs);
            
            string result = rr.Get();

            object json = JsonConvert.DeserializeObject<object>(result);

            return new { data = json, storage = fs };
        }
    }
}
