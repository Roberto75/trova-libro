using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;

namespace MyWebApplication.Controllers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MyAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {

        protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
        {
            //Debug.WriteLine("MyAuthorizeAttribute HandleUnauthorizedRequest");

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(
                        new
                        {
                            controller = "Home",
                            action = "AccessDenied"
                        }));
            }

            //base.HandleUnauthorizedRequest(filterContext);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
           // Debug.WriteLine("MyAuthorizeAttribute AuthorizeCore: " + Roles);

            //if (String.IsNullOrEmpty(Roles))
            //{
            //    return false;
            //}

            //return true;

            if (httpContext.Session["MySessionData"] == null)
            {
                return false;
            }


            MyManagerCSharp.MySessionData session = (MyManagerCSharp.MySessionData)httpContext.Session["MySessionData"];

            if (Roles == "REDATTORE")
            {
                return session.IsInProfile(Roles);
            }

            bool esito = session.IsInRole(Roles);


            if (esito == false)
            {
                string url = "";

                url = httpContext.Request.RawUrl;

                MyManagerCSharp.Log.LogUserManager log = new MyManagerCSharp.Log.LogUserManager("DefaultConnection");
                log.mOpenConnection();
                try
                {
                    log.insert(session.UserId,  session.Login, MyManagerCSharp.Log.LogUserManager.LogType.Access_denied, url);
                }
                finally
                {
                    log.mCloseConnection();
                }
            }

            return esito;

            //if (httpContext.User.Identity is System.Security.Principal.WindowsIdentity)
            //{
            //    Debug.WriteLine("MyAuthorizeAttribute -> WindowsIdentity");
            //    return session.IsInRole(Roles);
            //}

            //return base.AuthorizeCore(httpContext);
        }

    }
}
