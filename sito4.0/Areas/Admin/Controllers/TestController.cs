using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebApplication.Areas.Admin.Controllers
{
    [MyAuthorize(Roles = "Administrators")]
    public class TestController : MyBaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Tabs()
        {
            return View();
        }

        public ActionResult Table()
        {
            return View();
        }
    }
}
