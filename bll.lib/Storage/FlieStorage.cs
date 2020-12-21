using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bll.lib.Storage
{
    /// <summary>
    /// Manages the cache files of verified repositories
    /// </summary>
    public class FlieStorage
    {
        public string path { get; set; }

        private string _file_name;

        public string file_name
        {
            get { return _file_name; }
            set { _file_name = this.setFileName(value); }
        }

        private string setFileName(string value)
        {
            this.file_uri = Path.Combine(path, value);
            
            return value;
        }


        public string file_content { get; private set; }

        private string file_uri { get; set; }

        /// <summary>
        /// Cahce file lifetime
        /// </summary>
        private int cache_expires = 300;

        public FlieStorage(string path)
        {
            this.path = path;
            this.file_name = string.Empty;
            this.file_content = string.Empty;
        }

        private void deleteFile()
        {
            try
            {
                if (File.Exists(this.file_uri))
                {
                    File.Delete(this.file_uri);
                }
            }
            catch
            {

            }
        }

        public bool isvalid()
        {
            if (File.Exists(this.file_uri))
            {
                var filetime = File.GetCreationTime(this.file_uri);

                TimeSpan span = DateTime.Now - filetime;
                int sc = (int)span.TotalSeconds;

                if (sc <= cache_expires)
                {
                    return true;
                }
                else
                {
                    this.deleteFile();
                    return false;
                }
            }
            return false;
        }


        public void createOrUpdate(string data)
        {
            try
            {
                if (string.IsNullOrEmpty(this.file_name))
                {
                    throw new Exception("File name is invalid.");
                }

                string path = this.file_uri;

                if (!File.Exists(path))
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(data);
                    }
                }
                else
                {
                    StreamWriter sw = File.AppendText(path);
                    sw.WriteLine(data);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error handling file on the server.", ex);
            }
        }

        

        public bool readFileContent()
        {
            try
            {
                if (File.Exists(this.file_uri))
                {
                    var content = File.ReadAllText(this.file_uri);
                    this.file_content = content;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex) 
            {

                throw new Exception("Cache file not Exists", ex);
            }
        }
    }
}
