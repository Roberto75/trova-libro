using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebApplication.Controllers
{
    [AllowAnonymous]
    public class HomeController : MyBaseController
    {
       
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FAQ()
        {
            return View();
        }

        public ActionResult Donazione()
        {
            return View();
        }

        public ActionResult NotAuthorized()
        {
            return View();
        }


        public ActionResult NotAvailable()
        {
            return View();
        }

       

        public ActionResult Error()
        {
            Debug.WriteLine("MyError:" + TempData["MyError"]);
            Debug.WriteLine("UrlReferrer {0}", Request.UrlReferrer);
            Debug.WriteLine("Error {0}", Request["MyError"]);

            if (TempData["MyError"] != null && !String.IsNullOrEmpty(TempData["MyError"].ToString()))
            {
                MyManagerCSharp.Log.LogManager log = new MyManagerCSharp.Log.LogManager("DefaultConnection");
                log.mOpenConnection();
                try
                {
                    log.error(TempData["MyError"].ToString(), "HomeController.Error");
                }
                finally
                {
                    log.mCloseConnection();
                }
            }
            return View();
        }

    }
}