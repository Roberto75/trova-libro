using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;

namespace MyWebApplication.Areas.Admin.Controllers
{
    [MyAuthorize(Roles = "Administrators")]
    public class LogsController : MyBaseController
    {
        private MyManagerCSharp.Log.LogManager manager = new MyManagerCSharp.Log.LogManager("DefaultConnection");

        public ActionResult Index(MyManagerCSharp.Log.Models.SearchMyLogs model)
        {
            Debug.WriteLine("levelSelected: " + Request.Form["levelSelected"]);

            if (model.Sort == "DateAdded")
            {
                model.Sort = "date_added";
            }

            //if (Request.Form["levelSelected"] != null)
            //{
            //    model.levelSelected = new List<MyManagerCSharp.Log.LogManager.Level>();

            //    foreach (string s in Request.Form["levelSelected"].Split(','))
            //    {
            //        model.levelSelected.Add((MyManagerCSharp.Log.LogManager.Level)System.Enum.Parse(typeof(MyManagerCSharp.Log.LogManager.Level), s));
            //    }
            //}


            //if (Request.Form["myTypeSelected"] != null)
            //{
            //    foreach (string s in Request.Form["myTypeSelected"].Split(','))
            //    {
            //        model.myTypeSelected.Add(s);
            //    }
            //}


            manager.mOpenConnection();

            try
            {
                model.mySource = manager.getMySource();

                manager.getList(model);

            }
            finally
            {
                manager.mCloseConnection();
            }

            return View(model);
        }

        public ActionResult Details(string id, string[] LevelSelected)
        {
            Debug.WriteLine("Reference ID: " + id);

            MyManagerCSharp.Log.Models.MyLogDetail model = new MyManagerCSharp.Log.Models.MyLogDetail();

            model.sessionId = id;

            manager.mOpenConnection();
            try
            {
                if (LevelSelected != null)
                {
                    foreach (string s in LevelSelected)
                    {

                        model.LevelSelected.Add((MyManagerCSharp.Log.LogManager.Level)Enum.Parse(typeof(MyManagerCSharp.Log.LogManager.Level), s));
                    }

                    model.Logs = manager.getSessionDetail(id, model.LevelSelected);
                }
                else
                {
                    model.Logs = manager.getSessionDetail(id);
                }
                //model.LevelSelected = 

            }
            finally
            {
                manager.mCloseConnection();
            }
            return View(model);
        }

        public ActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Delete(MyManagerCSharp.Log.LogManager.Days days)
        {
            Debug.WriteLine("Delete: " + days);

            manager.mOpenConnection();
            try
            {
                int recordsDeleted;

                recordsDeleted = manager.delete(days);


            }
            finally
            {
                manager.mCloseConnection();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Summary(MyWebApplication.Areas.Admin.Models.LogsSummaryModel model)
        {
            manager.mOpenConnection();

            try
            {
                model.summary = manager.getSummary();

                model.summaryV2 = manager.getSummaryV2(model.days);

            }
            finally
            {
                manager.mCloseConnection();
            }
            return View(model);
        }


       [HttpPost]
        public ActionResult AjaxSearch(MyManagerCSharp.Log.LogManager.Level level, int days)
        {

            MyManagerCSharp.Log.Models.SearchMyLogs model = new MyManagerCSharp.Log.Models.SearchMyLogs();

           // disabilito la paginazione
            model.PageSize = 0;
            model.levelSelected.Clear();
            model.levelSelected.Add(level);

            switch (days)
            {
                case 0:
                    model.Days = MyManagerCSharp.ManagerDB.Days.Oggi;
                    break;
                case 1:
                    model.Days = MyManagerCSharp.ManagerDB.Days.Ieri;
                    break;
                case 7:
                    model.Days = MyManagerCSharp.ManagerDB.Days.Ultimi_7_giorni;
                    break;
                case 30:
                    model.Days = MyManagerCSharp.ManagerDB.Days.Ultimi_30_giorni;
                    break;
                default:
                     model.Days = MyManagerCSharp.ManagerDB.Days.Oggi;
                    break;
            }


            manager.mOpenConnection();

            try
            {
                manager.getList(model);
            }
            finally
            {
                manager.mCloseConnection();
            }

            return View("_LogList", model);
            //return Json(model.LogsList, JsonRequestBehavior.AllowGet);
        }

    }
}
