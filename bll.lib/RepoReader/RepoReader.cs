using bll.lib.Http;
using bll.lib.Model;
using bll.lib.Storage;
using bll.lib.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bll.lib.RepoReader
{
    /// <summary>
    /// Get repository content
    /// </summary>
    public class RepoReader
    {
        public string url_repo { get; set; }
        
        public string html { get; set; }

        public string repo_name { get; set; }

        private void setFileName()
        {
            this.storage.file_name = string.Format("{0}.json", makeHash.Make(this.repo_name+this.repo_user));
        }

        public string repo_user { get; set; }

        public FlieStorage storage { get; set; }


        public RepoReader(string url_repo, FlieStorage storage)
        {
            this.url_repo = url_repo;
            this.html = string.Empty;
            this.storage = storage;
        }


        public async Task<string> Get(bool only_ft_dir = false)
        {
            try
            {
                if (isUrlGithub(this.url_repo))
                {
                    if (storage.isvalid() && storage.readFileContent())
                    {
                        return storage.file_content;
                    }
                    else
                    {
                        HttpClient cli = new HttpClient();
                        
                        html = cli.getUrlContent(url_repo);

                        //READ DATA
                        RepoFinder finder = new RepoFinder(html);

                        finder.url_repo_base = string.Format("{0}{1}",this.repo_user,this.repo_name);

                        Task.Run(async () => await finder.walker(only_ft_dir));

                        int x = 0;

                        do
                        {
                            x++;
                        } while (finder.walking);

                        int dirs = finder.Directories.Count();
                        int files = finder.Directories.Count();

                        //SUM SIZE
                        var size = new JsonResultDataModel(this.url_repo,"finished", finder.repositorySize());


                           
                        
                        string json = JsonConvert.SerializeObject(size);
                        
                        storage.createOrUpdate(json);

                        if (storage.readFileContent())
                        {
                            html = storage.file_content;
                        }
                    }

                    return html;
                }
                else
                {
                    throw new Exception("This url is not a valid github repository.");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Check is a github url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool isUrlGithub(string url)
        {
            Uri uri = new Uri(url);

            if (uri.Host.Equals("github.com"))
            {
                this.getRepoInfo(uri);

                return true;
            }

            return false;
        }

        private void getRepoInfo(Uri uri)
        {
            try
            {
                if (uri.Segments.Count() >= 3)
                {
                    this.repo_name = uri.Segments[2];
                    this.repo_user = uri.Segments[1];
                    this.setFileName();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("not possible to extract the property data from the repository",ex);
            }
        }
    }
}
