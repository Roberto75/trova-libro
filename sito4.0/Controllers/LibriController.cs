using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebApplication.Controllers
{
    public class LibriController : Controller
    {

        private trova_libro.manager.LibriManager manager = new trova_libro.manager.LibriManager("mercatino");

        [AllowAnonymous]
        public ActionResult Index(trova_libro.manager.Models.SearchLibri model)
        {

            manager.mOpenConnection();
            try
            {
                manager.getList(model);
            }
            finally
            {
                manager.mCloseConnection();
            }

            return View(model);
        }

        public ActionResult MyAnnunci()
        {
            return View();
        }

        public ActionResult MyTrattative()
        {
            return View();
        }


    }
}