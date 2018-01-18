using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Diagnostics;
using MyUsers;
using System.Web.Mvc;

namespace MyWebApplication.Controllers
{
    public class MyBaseController : System.Web.Mvc.Controller
    {

        protected MyManagerCSharp.MySessionData MySessionData;


        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (Session["MySessionData"] != null)
            {
                MySessionData = (Session["MySessionData"] as MyManagerCSharp.MySessionData);
            }
        }

        protected override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {

            if (filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
               || filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                base.OnAuthorization(filterContext);
            }
            else
            {



                if (Session["MySessionData"] == null || System.Web.HttpContext.Current.User.Identity.IsAuthenticated == false )
                {
                    Debug.WriteLine("MySessionData is NULL");
                    filterContext.Result = new RedirectToRouteResult(
                          new System.Web.Routing.RouteValueDictionary(
                              new
                              {

                                  controller = "Account",
                                  action = "Login",
                                  ReturnUrl = filterContext.HttpContext.Request.RawUrl
                              }));

                }



              
                //if (!string.IsNullOrEmpty(SimpleSessionPersister.Username))
                //{
                //    filterContext.HttpContext.User = new CustomPrincipal(new CustomIndentity(SimpleSessionPersister.Username));
                //}

                Debug.WriteLine("IsAuthenticated " + System.Web.HttpContext.Current.User.Identity.IsAuthenticated);


                if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    //  filterContext.HttpContext.User = new MyCustomPrincipal(new MyCustomIndentity() );

                    // Debug.WriteLine("UserId:" + (User.Identity as MyCustomIndentity).UserId); 

                }


                //if (System.Web.HttpContext.Current.Session["MySessionData"] != null && !string.IsNullOrEmpty(System.Web.HttpContext.Current.Session["MySessionData"].ToString()))
                //{

                //    MyCustomIndentity identity = filterContext.HttpContext.User.Identity as MyCustomIndentity;


                //    // do some stuff here and assign a custom principal:
                //    MyCustomPrincipal principal = new MyCustomPrincipal(identity);
                //    // here you can assign some custom property that every user 
                //    // (even the non-authenticated have)

                //    // set the custom principal
                //    filterContext.HttpContext.User = principal;
                //}



                base.OnAuthorization(filterContext);
            }
        }
    }
}