using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApplication.Models
{
    public class ChangeEmailModel
    {
        public string emailAttuale { get; set; }

        public string newEmail { get; set; }
        public string newEmailConfirm { get; set; }


        public bool isCompleted { get; set; }
    }
}