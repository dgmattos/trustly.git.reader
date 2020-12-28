using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
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
        private int cache_expires = 30000;

        public FlieStorage(string path)
        {
            this.path = path;
            this.file_name = string.Empty;
            this.file_content = string.Empty;
        }

        public bool ifDirNotExists()
        {
            if (Directory.Exists(this.path))
            {
                // This path is a directory
                return true;
            }
            else
            {
                throw new DirectoryNotFoundException("Directory not found");
            }
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
            catch (Exception ex)
            {
                throw ex;
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
                    throw new ArgumentException("File name is invalid.");
                }

                string path = this.file_uri;


                if (!File.Exists(path))
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        int Locked = 0;

                        while (IsFileLocked(path))
                        {
                            Locked++;
                        }

                        sw.WriteLine(data);
                        sw.Close();
                    }
                }
                else
                {
                    int Locked = 0;

                    while (IsFileLocked(path))
                    {
                        Locked++;
                    }

                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine(data);
                        sw.Close();
                    }
                }

                
            }
            catch (Exception ex)
            {
                throw new Exception("Error handling file on the server.", ex);
            }
        }


        public bool IsFileLocked(string filename)
        {
            bool Locked = false;
            try
            {
                FileStream fs =
                    File.Open(filename, FileMode.OpenOrCreate,
                    FileAccess.ReadWrite, FileShare.None);
                fs.Close();
            }
            catch (IOException ex)
            {
                Locked = true;
            }

            return Locked;
        }


        public bool readFileContent()
        {
            try
            {
                if (File.Exists(this.file_uri))
                {
                    using (System.IO.StreamReader file = new System.IO.StreamReader(this.file_uri))
                    {

                        string line;

                        while ((line = file.ReadLine()) != null)
                        {
                            this.file_content = line;
                        }

                        file.Close();

                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (FileNotFoundException)
            {

                throw new FileNotFoundException("Cache file not Exists");
            }
        }
    }
}
