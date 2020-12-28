using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bll.lib.Model
{
    

    public class SizeResultDataModel
    {
        public string ext { get; set; }
        
        
        public int lines { get; set; }

        
        public float size { get; set; }

        public string size_unity = "kb";

        public SizeResultDataModel()
        {

        }

    }
}
