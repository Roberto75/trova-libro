using System;
using System.Collections.Generic;
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
    }
}