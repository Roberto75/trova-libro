using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApplication.Areas.Admin.Models
{
    public class ModelHome
    {
        public string version { get; set; }
        public string versionMVC { get; set; }

        public bool mailIsEnabled { get; set; }
    }
}