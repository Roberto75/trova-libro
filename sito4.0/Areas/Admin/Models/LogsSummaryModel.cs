using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApplication.Areas.Admin.Models
{
    public class LogsSummaryModel
    {
        public MyManagerCSharp.Log.LogManager.Days days { get; set; }

        public System.Data.DataTable summary;

        public System.Data.DataTable summaryV2;

        /*
        public int CountUndefined { get; set; }
        public int CountDebug { get; set; }
        public int CountInfo { get; set; }
        public int CountWarning { get; set; }
        public int CountError { get; set; }
        public int CountException { get; set; }*/

        public LogsSummaryModel()
        {
            days = MyManagerCSharp.ManagerDB.Days.Ultimi_7_giorni;
        }

    }
}