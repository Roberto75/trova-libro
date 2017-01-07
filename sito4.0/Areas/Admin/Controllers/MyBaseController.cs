using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebApplication.Areas.Admin.Controllers
{
    public class MyBaseController : Controller
    {

        protected override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {

            Debug.WriteLine("IsAuthenticated " + System.Web.HttpContext.Current.User.Identity.IsAuthenticated);

            if (filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                || filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                base.OnAuthorization(filterContext);
            }
            else
            {


                // MyHelper.printRequest(Request);

                Debug.WriteLine("MyBaseController.OnAuthorization " + System.Web.HttpContext.Current.User.Identity.IsAuthenticated);

                //if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == false && (User.Identity.GetType() != typeof(System.Security.Principal.WindowsIdentity)))
                if (Session["MySessionData"] == null || (System.Web.HttpContext.Current.User.Identity.IsAuthenticated == false && (User.Identity.GetType() != typeof(System.Security.Principal.WindowsIdentity))))
                {
                    filterContext.Result = new RedirectToRouteResult(
                          new System.Web.Routing.RouteValueDictionary(
                              new
                              {
                                  area = "Admin",
                                  controller = "Account",
                                  action = "Login",
                                  ReturnUrl = filterContext.HttpContext.Request.RawUrl
                              }));

                }
            }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Debug.WriteLine("Admin.MyBaseController.OnException");

            if (filterContext.Exception != null)
            {
                Debug.WriteLine("Admin.MyBaseController.OnException: " + filterContext.Exception.Message);

                MyManagerCSharp.Log.LogManager log = new MyManagerCSharp.Log.LogManager("DefaultConnection");
                log.mOpenConnection();
                try
                {
                    log.exception("Admin.MyBaseController", filterContext.Exception);
                }
                finally
                {
                    log.mCloseConnection();
                }

            }

            filterContext.Result = new ViewResult { ViewName = "~/Areas/Admin/Views/Admin/Error" };


        }


    }
}