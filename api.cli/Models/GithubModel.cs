using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace api.cli.Models
{
    public class GithubModel
    {
        [Display(Name = "URL GITHUB")]
        [Required]
        [DataType(DataType.Url)]
        public string url { get; set; }
    }
}