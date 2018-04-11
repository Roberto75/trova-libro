using MyWebApplication.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebApplication.Areas.Admin.Controllers
{
    public class AdminController : MyBaseController
    {
        [MyAuthorize(Roles = "Administrators")]
        public ActionResult Index()
        {

            Models.ModelHome model = new Models.ModelHome();

            //System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            //System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            //model.version = fvi.FileVersion;
            //model.versionMVC = typeof(System.Web.Mvc.Controller).Assembly.GetName().Version.ToString();



            model.versionMyWebApplication = typeof(MyWebApplication.MvcApplication).Assembly.GetName().Version.ToString();
            model.versionAnnunci = typeof(Annunci.ImmobiliareManager).Assembly.GetName().Version.ToString();
            // model.versionImmobiliareVb = typeof(ImmobiliareVb.RevoAgent).Assembly.GetName().Version.ToString();
            model.versionMyManagerCSharp = typeof(MyManagerCSharp.ManagerDB).Assembly.GetName().Version.ToString();
            model.versionMyUsers = typeof(MyUsers.UserManager).Assembly.GetName().Version.ToString();
            model.versionMVC = typeof(Controller).Assembly.GetName().Version.ToString();

            model.mailIsEnabled = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["mail.isEnabled"]);

            return View(model);
        }


        public ActionResult Error()
        {
            Debug.WriteLine("MyError:" + TempData["MyError"]);

            /*  if (TempData["MyError"] != null && !String.IsNullOrEmpty(TempData["MyError"].ToString()))
              {
                  MyManagerCSharp.Log.LogManager log = new MyManagerCSharp.Log.LogManager("DefaultConnection");
                  log.mOpenConnection();
                  try
                  {
                      log.error(TempData["MyError"].ToString(), "AdminController.Error");
                  }
                  finally
                  {
                      log.mCloseConnection();
                  }
              }*/
            return View();
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult MD5(string textinput)
        {
            if (!String.IsNullOrEmpty(textinput))
            {
                ViewBag.valore = textinput;
                ViewBag.risultato = MyManagerCSharp.SecurityManager.getMD5Hash(textinput);
            }
            return View();
        }


        [AllowAnonymous]
        public ActionResult Email(EmailModel model)
        {

           // model.server = System.Configuration.ConfigurationManager.AppSettings["mail.server"];

            if (System.Configuration.ConfigurationManager.AppSettings["mail.server.port"] != null && !String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["mail.server.port"]))
            {
                model.port = int.Parse(System.Configuration.ConfigurationManager.AppSettings["mail.server.port"]);
            }
            else
            {
                model.port = 465;
            }
            model.server = "smtps.aruba.it";
            model.enableSsl = true;
            model.enableTsl = true;

            model.to = "roberto.rutigliano@gmail.com";
            model.subject = "Test invio email";
            model.body = "Test del " + DateTime.Now;
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendEmail(EmailModel model)
        {
            MyManagerCSharp.MailMessageManager mail = new MyManagerCSharp.MailMessageManager(System.Configuration.ConfigurationManager.AppSettings["application.name"], System.Configuration.ConfigurationManager.AppSettings["application.url"]);

            mail.MailServer = model.server;
            mail.port = model.port;
            mail.enableSsl = model.enableSsl;
            mail.enableTls = model.enableTsl;

            mail.Subject = model.subject;
            mail.To(model.to);
            mail.Body = model.body;


            model.esito = mail.send();

            if (model.esito == "")
            {
                model.esito = "Email inviata con successo";
            }


            return View("Email", model);
        }

        public ActionResult Whatsnew()
        {
            return View();
        }



        public ActionResult Check()
        {

            MyManagerCSharp.ManagerDB manager = new MyManagerCSharp.ManagerDB("utenti");

            MyWebApplication.Areas.Admin.Models.CheckModel model = new Models.CheckModel();
            MyManagerCSharp.Models.MyCheckItem check;

            try
            {
                manager.mOpenConnection();

                //check = new MyManagerCSharp.Models.MyCheckItem();
                //check.Name = "@@DATEFIRST (Transact-SQL)";
                //check.Description = "T-SQL SET DATEFIRST(1) per fare iniziare la settimana da lunedì";
                //check.Value = manager._executeScalar("SELECT @@DATEFIRST");
                //if (check.Value == "1")
                //{
                //    check.Status = MyManagerCSharp.Models.MyCheckItem.StatusType.Passed;
                //}
                //else
                //{
                //    check.Status = MyManagerCSharp.Models.MyCheckItem.StatusType.Failed;
                //}
                //model.checkList.Add(check);


                check = new MyManagerCSharp.Models.MyCheckItem();
                check.Name = "Mail is enabled";
                check.Description = "Web.config ";
                check.Value = System.Configuration.ConfigurationManager.AppSettings["mail.isEnabled"];
                if (check.Value.ToLower() == "true")
                {
                    check.Status = MyManagerCSharp.Models.MyCheckItem.StatusType.Passed;
                }
                else
                {
                    check.Status = MyManagerCSharp.Models.MyCheckItem.StatusType.Failed;
                }
                model.checkList.Add(check);




                check = new MyManagerCSharp.Models.MyCheckItem();
                check.Name = "#DEBUG";

#if DEBUG
                check.Value = "true";
                check.Status = MyManagerCSharp.Models.MyCheckItem.StatusType.Failed;
                check.Description = "Debug is enabled";
#endif

#if !DEBUG
                check.Value = "false";
                check.Status = MyManagerCSharp.Models.MyCheckItem.StatusType.Passed;
                check.Description = "Debug is disabled";
#endif
                model.checkList.Add(check);





                /*


                check = new MyManagerCSharp.Models.MyCheckItem();
                check.Name = "Twitter";
                check.Description = "Connection to Twitter service";

                if (String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["twitter.consumerKey"]))
                {
                    check.Value = "twitter.consumerKey is NULL";
                    check.Status = MyManagerCSharp.Models.MyCheckItem.StatusType.Failed;

                }
                else
                {
                    MyTwitter.TwitterManager twitter = new MyTwitter.TwitterManager(System.Configuration.ConfigurationManager.AppSettings["twitter.consumerKey"], System.Configuration.ConfigurationManager.AppSettings["twitter.consumerSecret"]);
                    twitter.setKey(DataModel.MySecurityManager.Key, DataModel.MySecurityManager.IV);

                    try
                    {
                        string esito = twitter.isAuthenticated();
                        if (String.IsNullOrEmpty(esito))
                        {
                            check.Status = MyManagerCSharp.Models.MyCheckItem.StatusType.Passed;
                            check.Value = "Authenticated";

                        }
                        else
                        {
                            check.Status = MyManagerCSharp.Models.MyCheckItem.StatusType.Failed;
                            check.Value = esito;
                        }

                    }
                    catch (System.Net.WebException ex)
                    {
                        Debug.WriteLine("System.Net.WebException: " + ex.Message);
                        check.Value = ex.Message;
                        check.Status = MyManagerCSharp.Models.MyCheckItem.StatusType.Failed;
                    }
                }
                model.checkList.Add(check);

    */



            }
            finally
            {
                manager.mCloseConnection();
            }






            return View(model);
        }
    }
}