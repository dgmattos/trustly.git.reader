using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bll.lib.Model
{
    public class DirRepoModel
    {
        public string dir_url { get; set; }

        public string dir_name { get; set; }

        public List<FileRepoModel> flies { get; set; }

        public bool isCheckd { get; set; }

        public DirRepoModel(string dir_url)
        {
            flies = new List<FileRepoModel>();
            this.dir_url = dir_url;
            isCheckd = false;
        }
    }
}
