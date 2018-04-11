﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApplication.Areas.Admin.Models
{
    public class EmailModel
    {
        public string to { get; set; }
        public string subject { get; set; }
        public string body { get; set; }


        public string server { get; set; }
        public int? port { get; set; }

        public bool enableSsl { get; set; }
        public bool enableTsl { get; set; }

        public string esito { get; set; }
    }
}