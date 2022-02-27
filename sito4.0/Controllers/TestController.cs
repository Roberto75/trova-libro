using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebApplication.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Collapse()
        {
            return View();
        }

        public ActionResult Modal()
        {
            return View();
        }

        public ActionResult Summernote()
        {
            return View();
        }
    }
}