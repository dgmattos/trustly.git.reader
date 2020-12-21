using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bll.lib.Model
{
    public class FileRepoModel
    {

        private string _file_url;

        public string file_url
        {
            get { return _file_url; }
            set { _file_url = setFileInfo(value); }
        }

        private string setFileInfo(string value)
        {
            string[] exp = value.Split('/');

            this.name = exp.Last();

            string[] expII = this.name.Split('.');

            this.extension = expII.Last();

            return value;
        }

        public string name { get; set; }

        public string extension { get; set; }

        /// <summary>
        /// File's number rows
        /// </summary>
        public int lines { get; set; }

        /// <summary>
        /// File's size in kb
        /// </summary>
        public float size { get; set; }

        public bool isCheckd { get; set; }

        public FileRepoModel(string file_url)
        {
            this.file_url = file_url;

            isCheckd = false;
        }
    }
}
