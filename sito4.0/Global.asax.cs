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
            Exception exception;
            exception = Server.GetLastError();

            if (exception == null)
            {
                return;
            }

            HttpException httpException = exception as HttpException;

            Debug.WriteLine("Application_Error: " + exception.GetType().Name + " Message:" + exception.Message);

            if (httpException != null)
            {
                Debug.WriteLine("\t HttpCode: " + httpException.GetHttpCode());
            }

            /*   

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
               mail.sendException(exception, String.Format("Application_Error user: {0} referrer: {1}", user, referrer));

              */






        }



        protected void Application_EndRequest(object sender, EventArgs e)
        {
            Debug.WriteLine("Application_EndRequest StatusCode:" + Context.Response.StatusCode, Request.Url.AbsoluteUri);

            if (Context.Response.StatusCode == 404)
            {
                Exception exception;
                exception = Server.GetLastError();

                HttpException httpException = exception as HttpException;

                Response.Clear();
                Server.ClearError();
                RouteData routeData = new RouteData();
                routeData.Values["controller"] = "Errors";
                routeData.Values["action"] = "Index";
                routeData.Values["exception"] = exception;
                Response.StatusCode = 500;

                if (httpException != null)
                {
                    Response.StatusCode = httpException.GetHttpCode();
                    switch (Response.StatusCode)
                    {
                        case 403:
                            routeData.Values["action"] = "HttpError404";
                            break;
                        case 404:
                            routeData.Values["action"] = "HttpError404";
                            break;
                    }
                }
              

                // Avoid IIS7 getting in the middle
                Response.TrySkipIisCustomErrors = true;
                IController errorsController = new MyWebApplication.Controllers.ErrorsController();

                errorsController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
            }
        }
    }
}
