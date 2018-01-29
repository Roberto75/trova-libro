using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Diagnostics;
using MyUsers;
using System.Web.Mvc;
using System.Web.Security;

namespace MyWebApplication.Controllers
{
    public class MyBaseController : System.Web.Mvc.Controller
    {

        protected MyManagerCSharp.MySessionData MySessionData;


        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (User.Identity.IsAuthenticated && Session != null && Session["MySessionData"] == null)
            {
                FormsAuthentication.SignOut();
            }


            if (Session != null && Session["MySessionData"] != null)
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
                if (Session["MySessionData"] == null || System.Web.HttpContext.Current.User.Identity.IsAuthenticated == false)
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


        protected override void HandleUnknownAction(string actionName)
        {
            Debug.WriteLine("HandleUnknownAction" + actionName);
            //this.View(errorpage).ExecuteResult(this.ControllerContext);
        }


        protected override void OnException(ExceptionContext filterContext)
        {
            Debug.WriteLine("OnException");
            //base.OnException(filterContext);

            // Mark exception as handled
            filterContext.ExceptionHandled = true;

            //  TempData["Exception"] = filterContext.Exception;

            sendMailExceptionAsync(filterContext.Exception);
            MyLogExceptionAsync("ControllerOnException", filterContext.Exception);

            //Errore generico
            filterContext.Result = RedirectToAction("Index", "Errors");
            base.OnException(filterContext);

            //TempData["Exception"] = filterContext.Exception;
            //return RedirectToAction("NotAuthorized", "Errors");

            //   filterContext.Result = new RedirectToRouteResult(
            //              new System.Web.Routing.RouteValueDictionary(
            //                new
            //              {
            //                controller = "Errors",
            //              action = "ControllerOnException",
            //            exception = filterContext.
            //      }));

        }


        protected void sendMailExceptionAsync(Exception ex)
        {
            Debug.WriteLine("sendMailExceptionAsync - START");
            System.Threading.Tasks.Task.Factory.StartNew(() => sendMailException(ex, ""));
            Debug.WriteLine("sendMailExceptionAsync - END");
        }

        protected void sendMailExceptionAsync(Exception ex, string messaggio)
        {
            Debug.WriteLine("sendMailExceptionAsync - START");
            System.Threading.Tasks.Task.Factory.StartNew(() => sendMailException(ex, messaggio));
            Debug.WriteLine("sendMailExceptionAsync - END");
        }

        private void sendMailException(Exception ex, string messaggio)
        {
            Debug.WriteLine("sendMailException - START");

            string currentController = "";
            string currentAction = "";


            if (RouteData != null)
            {
                if (RouteData.Values["controller"] != null && !String.IsNullOrEmpty(RouteData.Values["controller"].ToString()))
                {
                    currentController = RouteData.Values["controller"].ToString();
                }

                if (RouteData.Values["action"] != null && !String.IsNullOrEmpty(RouteData.Values["action"].ToString()))
                {
                    currentAction = RouteData.Values["action"].ToString();
                }
            }

            string referrer = "null";
            if (Request.UrlReferrer != null)
            {
                referrer = Request.UrlReferrer.AbsoluteUri;
            }


            MyManagerCSharp.MailMessageManager mail = new MyManagerCSharp.MailMessageManager(System.Configuration.ConfigurationManager.AppSettings["application.name"], System.Configuration.ConfigurationManager.AppSettings["application.url"]);



            if (String.IsNullOrEmpty(messaggio))
            {
                mail.sendException(ex, String.Format("Controller [{0}] - Action [{1}] - Url [{2}] - Refferer [{3}]", currentController, currentAction, Request.Url.AbsoluteUri, referrer));
            }
            else
            {
                mail.sendException(ex, String.Format("Controller [{0}] - Action [{1}] - Url [{2}] - Refferer [{3}] - {4}", currentController, currentAction, Request.Url.AbsoluteUri, referrer, messaggio));
            }
        
            //System.Threading.Thread.Sleep(15000);

            Debug.WriteLine("sendMailException - END");
        }


        //protected void logReport(string reportName)
        //{
        //    MyManagerCSharp.Log.LogUserManager log = new MyManagerCSharp.Log.LogUserManager("DefaultConnection");
        //    log.mOpenConnection();

        //    try
        //    {
        ////        log.insert(_mySessionData.UserId, _mySessionData.Login, MyManagerCSharp.Log.LogUserManager.LogType.Report, reportName, System.Net.IPAddress.Parse(HttpContext.Request.UserHostAddress));
        //    }
        //    finally
        //    {
        //        log.mCloseConnection();
        //    }

        //}


        protected void MyLogExceptionAsync(string tipo, Exception ex)
        {
            Debug.WriteLine("MyLogExceptionAsync - START");
            System.Threading.Tasks.Task.Factory.StartNew(() => MyLogException(tipo, ex));
            Debug.WriteLine("MyLogExceptionAsync - END");
        }

        private void MyLogException(string tipo, Exception ex)
        {
            Debug.WriteLine("MyLogException - START");
            string currentController = "";
            string currentAction = "";


            if (RouteData != null)
            {
                if (RouteData.Values["controller"] != null && !String.IsNullOrEmpty(RouteData.Values["controller"].ToString()))
                {
                    currentController = RouteData.Values["controller"].ToString();
                }

                if (RouteData.Values["action"] != null && !String.IsNullOrEmpty(RouteData.Values["action"].ToString()))
                {
                    currentAction = RouteData.Values["action"].ToString();
                }
            }

            MyManagerCSharp.Log.LogManager log = new MyManagerCSharp.Log.LogManager("log");
            log.mOpenConnection();
            try
            {
                if (ex.Message.StartsWith("Timeout expired"))
                {
                    //  log.insert("Timeout expired", String.Format("{0}.{1}.{2}", currentController, currentAction, tipo), "", "", MyManagerCSharp.Log.LogManager.Level.Exception);
                    log.insert(tipo, String.Format("{0}.{1}.{2}", currentController, currentAction, tipo), "", "Timeout expired", MyManagerCSharp.Log.LogManager.Level.Exception);
                }
                else
                {
                    log.exception(ex, "", String.Format("{0}.{1}", currentController, currentAction), tipo);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                log.mCloseConnection();
            }

            //System.Threading.Thread.Sleep(15000);

            Debug.WriteLine("MyLogException - END");
        }

    }

}
