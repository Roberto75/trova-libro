﻿using System;
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


        public ActionResult TestiScolastici()
        {

            Annunci.Libri.Models.SearchLibri model = new Annunci.Libri.Models.SearchLibri();
            model.filter.categoriaId = 1130000;
            return RedirectToAction("Index", model);
        }



        public ActionResult Test401()
        {
            throw new HttpException(401, "Not Authorized");
        }

        public ActionResult TestException()
        {
            throw new Exception("Oh no, some error occurred...");
        }

        public ActionResult TestMyException()
        {
            throw new MyManagerCSharp.MyException("Oh no, some error occurred...");
        }


    }
}