using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebApplication.Areas.Admin.Controllers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MyAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        //http://www.prideparrot.com/blog/archive/2012/6/customizing_authorize_attribute

        protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
        {
            Debug.WriteLine("MyAuthorizeAttribute HandleUnauthorizedRequest");

            //if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            //{
              //  base.HandleUnauthorizedRequest(filterContext);
            //}
            //else
            //{
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(
                        new
                        {
                            area = "Admin",
                            controller = "Admin",
                            action = "AccessDenied"
                        }));
            //}

            //base.HandleUnauthorizedRequest(filterContext);

        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Debug.WriteLine("MyAuthorizeAttribute AuthorizeCore: " + Roles);

            if (httpContext.Session["MySessionData"] == null)
            {
                return false;
            }

            MyManagerCSharp.MySessionData session = (MyManagerCSharp.MySessionData)httpContext.Session["MySessionData"];

            return session.IsInGroup("Administrators");
            // return base.AuthorizeCore(httpContext);
            //return session.IsInRole("ADMIN");
        }
    }
}