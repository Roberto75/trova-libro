using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyWebApplication
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }



        protected void Application_Error(Object sender, EventArgs e)
        {
            Exception objError;
            objError = Server.GetLastError();
            if (objError != null)
            {
                Debug.WriteLine("Application_Error: " + objError.GetType().Name + " Message:" + objError.Message);

                string user = "";
                if (String.IsNullOrEmpty(User.Identity.Name))
                {
                    user = "anonymous";
                }
                else
                {
                    user = User.Identity.Name;
                }


                string referrer;
                if (Request.UrlReferrer != null)
                {
                    referrer = Request.UrlReferrer.AbsoluteUri;
                }
                else
                {
                    referrer = "is null";
                }

                MyManagerCSharp.MailMessageManager mail = new MyManagerCSharp.MailMessageManager(System.Configuration.ConfigurationManager.AppSettings["application.name"], System.Configuration.ConfigurationManager.AppSettings["application.url"]);
                mail.sendException(objError, String.Format("Application_Error user: {0} referrer: {1}", user, referrer));

                //switch (objError.GetType().Name)
                //{
                //    case "MyException":
                //        Server.TransferRequest("~/Home/Error?MyError=" + Server.UrlEncode(objError.Message));
                //        break;
                //    case "HttpAntiForgeryException":
                //        Server.TransferRequest("~/Account/Login");
                //        break;
                //}
            }
        }
    }
}
