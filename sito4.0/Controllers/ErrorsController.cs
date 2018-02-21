using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebApplication.Controllers
{
    [AllowAnonymous]
    public class ErrorsController : MyBaseController
    {
        //https://stackoverflow.com/questions/717628/asp-net-mvc-404-error-handling


        public ActionResult Index()
        {
            //Errore generico
            Debug.WriteLine("General:" + TempData["MyError"]);
            Debug.WriteLine("UrlReferrer {0}", Request.UrlReferrer);
            Debug.WriteLine("Error {0}", Request["MyError"]);

            Response.StatusCode = 500;
            return View();
        }
        
        public ActionResult NotAuthorized()
        {
            Debug.WriteLine(Request.Url.AbsoluteUri);

            if (TempData["MyException"] != null)
            {
                MyManagerCSharp.MyException exception = (MyManagerCSharp.MyException)TempData["MyException"];
                MyLogExceptionAsync("NotAuthorized", exception);
                sendMailExceptionAsync(exception);
            }

            //In summary, a 401 Unauthorized response should be used for missing or bad authentication, and a 403 Forbidden response should be used afterwards
            //, when the user is authenticated but isn’t authorized to perform the requested operation on the given resource.

            Response.StatusCode = 403;
            return View();
        }


        public ActionResult NotAvailable()
        {
            Debug.WriteLine(Request.Url.AbsoluteUri);

            if (TempData["MyException"] != null)
            {
                MyManagerCSharp.MyException exception = (MyManagerCSharp.MyException)TempData["MyException"];
                MyLogExceptionAsync("NotAvailable", exception);
                sendMailExceptionAsync(exception);
            }

            Response.Clear();
            //Response.StatusCode = 404;
            //Response.StatusCode = 301;
            return View("~/Views/Errors/NotAvailable.cshtml");
        }


        public ActionResult HttpError404(Exception exception)
        {
            //Dal global.asax gli errori 404 vengono indirizzati a questa action

            //l'eccezione viene passata come parametro alla action!

            if (exception != null)
            {
                MyLogExceptionAsync("HttpError404", exception);
                sendMailExceptionAsync(exception);
            }
            Debug.WriteLine(Response.StatusCode);
            return View("NotFound");
        }



        /*
                public ActionResult ControllerOnException()
                {
                    Debug.WriteLine(Request.Url.AbsoluteUri);

                    //ho loggato sull MyBaseController
                    //if (TempData["Exception"] != null)
                    //{
                    //  Exception exception = (Exception)TempData["Exception"];
                    //                MyLogExceptionAsync("ControllerOnException", exception);
                    //              sendMailExceptionAsync(exception);
                    //        }

                    Response.StatusCode = 500;
                    return View("Index");
                }

            */



    }
}