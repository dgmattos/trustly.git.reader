using bll.lib.Http;
using bll.lib.RepoReader;
using bll.lib.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;

namespace api.cli.Controllers
{
    public class GithubController : ApiController
    {

        // POST api/Github/{url}
        
        public Object Get(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    throw new ArgumentException("the url parameter does not exist");
                }

                FlieStorage fs = new FlieStorage(HttpContext.Current.Server.MapPath("~/App_Data/"));
                
                fs.ifDirNotExists();
                
                RepoReader rr = new RepoReader(url, fs);

                var run = Task.Run(() => rr.Get(false));

                string result = run.Result;

                object json = JsonConvert.DeserializeObject<object>(result);

                return Ok(new { json });
            }
            catch (Exception ex)
            {
                var hasInner = ex;

                while (hasInner.InnerException != null)
                {
                    hasInner = hasInner.InnerException;
                }

                return BadRequest(hasInner.Message);
            }
        }
    }
}
