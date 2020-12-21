using bll.lib.Html;
using bll.lib.Http;
using bll.lib.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


namespace bll.lib.RepoReader
{
    public class RepoFinder
    {
        public string html_base { get; set; }

        public string url_repo_base { get; set; }

        public List<DirRepoModel> Directories { get; set; }

        private void addDirIfNotExist(DirRepoModel dir)
        {
            if (Directories.Where(w => w.dir_url.Equals(dir.dir_url)).Count() == 0)
            {
                Directories.Add(dir);
            }
        }

        public List<FileRepoModel> Files { get; set; }

        private void addFileIfNotExist(FileRepoModel file)
        {
            if (Files.Where(w => w.file_url.Equals(file.file_url)).Count() == 0)
            {
                Files.Add(file);
            }
        }

        public bool walking { get; private set; }

        public StringBuilder links { get; set; }

        public RepoFinder(string html_base)
        {
            this.html_base = html_base;

            Directories = new List<DirRepoModel>();
            Files = new List<FileRepoModel>();

            this.walking = true;

            links = new StringBuilder(string.Empty);
        }

        public async Task walker()
        {
            this.walking = true;

            await _walker_dirs(html_base);

            foreach (var file in Files.Where(w => !w.isCheckd))
            {
                file.isCheckd = true;
                try
                {

                    var res = await this._walker_files(file.file_url);
                    file.lines = res.lines;
                    file.size = res.size;
                }
                catch (Exception ex)
                {

                    links.Append(ex.Message);
                }

                Thread.Sleep(25);
            }

            this.walking = false;
        }

        public bool urlIsRepoDirOrFile(string url, bool isTree = false)
        {
            string tree = this.url_repo_base.TrimEnd('/') + "/" + "tree/";
            string blob = this.url_repo_base.TrimEnd('/') + "/" + "blob/";
            //
            if (!url.Contains("&quot;") && (url.Contains(tree) || url.Contains(blob)))
            {
                if (isTree)
                {
                    if (url.Contains(tree))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private string htmlAffter(Regex rexg, string text)
        {
            string ht = string.Empty;

            using (StringReader sr = new StringReader(text))
            {
                int count = 0;
                string line;

                bool mach = false;
                while ((line = sr.ReadLine()) != null)
                {
                    if (!mach)
                    {
                        var m = rexg.Match(line);
                        if (m.Success)
                        {
                            mach = true;
                            ht += line;
                        }
                    }
                    else
                    {
                        ht += line;
                    }
                }
            }

            return ht;
        }

        private string getLink(Regex regex, string str)
        {
            str = str.Replace("\\", string.Empty);
            string st = regex.Match(str).ToString();

            return st.Replace("href=\"", string.Empty).Replace("\"", string.Empty);
        }

        private async Task _walker_dirs(string str)
        {
            this.walking = true;

            try
            {
                //GET DIV FILE LIST

                string divFiles = htmlAffter(new Regex("(<div[^>]*? aria-labelledby=\"files\"[^>]*>)"), str);

                //GET HREF FROM FILE LIST
                var regex = new Regex("<a[^>]*? *>", RegexOptions.IgnoreCase);
                var urls = regex.Matches(divFiles);



                foreach (var item in urls)
                {
                    string st = getLink(new Regex("(href=\").*(\")"), item.ToString());

                    if (urlIsRepoDirOrFile(st))
                    {
                        links.Append(string.Format("{0}\n", item));

                        if (urlIsRepoDirOrFile(st, true))
                        {
                            addDirIfNotExist(new DirRepoModel(st));
                        }
                        else
                        {
                            addFileIfNotExist(new FileRepoModel(st));
                        }
                        Thread.Sleep(25);
                    }
                }

                //TODO - check all directory
                foreach (var dir in Directories.Where(w => !w.isCheckd))
                {
                    dir.isCheckd = true;

                    try
                    {
                        string html = _webcli(dir.dir_url);
                        await this._walker_dirs(html);
                    }
                    catch (Exception ex)
                    {

                        links.Append(ex.Message);
                    }
                    Thread.Sleep(25);
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }
        }



        private async Task<FileDataModel> _walker_files(string url)
        {
            this.walking = true;

            try
            {
                string html = _webcli(url);
                FileDataModel fd = new FileDataModel();

                //TODO - Get file info
                var rgxLines = new Regex(@"([\d]+( lines))");
                var resultsLines = rgxLines.Matches(html);

                foreach (var item in resultsLines)
                {
                    string st = item.ToString();
                    if (!string.IsNullOrEmpty(st))
                    {
                        int i = int.Parse(Regex.Replace(st, @"[^\d]", ""));
                        fd.lines = i;
                    }
                }

                var rgxSize = new Regex(@"((\d+(\.\d*)?|\.\d+)+( KB))");
                var resultsSize = rgxSize.Matches(html);

                foreach (var item in resultsSize)
                {
                    string st = item.ToString();
                    if (!string.IsNullOrEmpty(st))
                    {
                        float f = float.Parse(Regex.Replace(st, @"[^\d]", "")) / 100;
                        fd.size = f;
                    }

                }



                return fd;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string _webcli(string url)
        {
            try
            {
                string html = string.Empty;

                HttpClient cli = new HttpClient();

                html = cli.getUrlContent("https://github.com/" + url);

                if (string.IsNullOrEmpty(html))
                {
                    throw new Exception("Empty file content");
                }

                return html;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<ResultDataModel> repositorySize()
        {
            List<ResultDataModel> sizes = new List<ResultDataModel>();

            foreach (var item in this.Files.GroupBy(g => g.extension))
            {
                sizes.Add(new ResultDataModel
                {
                    ext = item.First().extension,
                    lines = item.Sum(s => s.lines),
                    size = item.Sum(s => s.size)
                });
            }

            return sizes;
        }
    }
}
