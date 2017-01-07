using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApplication.Areas.Admin.Models
{
    public class UserProfile
    {
        public long userId { get; set; }
        public long customerId { get; set; }
        public string login { get; set; }
        public string pathImageProfile { get; set; }


        //public string 

        public MyUsers.Models.MyUser utente;
    }
}