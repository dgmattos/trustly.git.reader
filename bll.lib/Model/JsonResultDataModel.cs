using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bll.lib.Model
{
    public class JsonResultDataModel
    {
        public string GitHub { get; set; }
        public string status { get; set; }
        public DateTime dt_processing { get; set; }
        public List<SizeResultDataModel> files { get; set; }

        public JsonResultDataModel(string GitHub, string status, List<SizeResultDataModel> files)
        {
            this.status = status;
            this.files = files;
            dt_processing = DateTime.Now;
            this.GitHub = GitHub;


        }
    }
}
