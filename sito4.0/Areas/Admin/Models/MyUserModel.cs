using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApplication.Areas.Admin.Models
{
    public class MyUserModel
    {
        public MyUsers.Models.MyUser Utente { get; set; }

        public List<MyManagerCSharp.RGraph.Models.RGraphModel> Reports { get; set; }

        public List<MyUsers.Models.MyGroup>  Gruppi { get; set; }
        //public System.Web.Mvc.MultiSelectList  Gruppi { get; set; }
        public System.Web.Mvc.SelectList Profilo { get; set; }


        public MyUserModel()
        {
            Reports = new List<MyManagerCSharp.RGraph.Models.RGraphModel>();
        }

    }
}