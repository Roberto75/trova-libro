using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApplication.Areas.Admin.Models
{
    public class ReportsModel
    {

        public List<MyManagerCSharp.RGraph.Models.RGraphModel> Reports { get; set; }
        public MyManagerCSharp.Log.LogManager.Days Days { get; set; }
        public System.Data.DataTable Table { get; set; }

        public DateTime? DataDiRiferimento { get; set; }

        public ReportsModel()
        {
            Reports = new List<MyManagerCSharp.RGraph.Models.RGraphModel>();
            Days = MyManagerCSharp.Log.LogManager.Days.Oggi;
        }
    }
}