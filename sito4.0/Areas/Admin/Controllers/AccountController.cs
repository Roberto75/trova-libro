using MyWebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebApplication.Areas.Admin.Controllers
{
    public class AccountController : MyBaseController
    {
        private MyUsers.UserManager manager = new MyUsers.UserManager("utenti");

        [Authorize]
        [MyAuthorize(Roles = "Administrators")]
        public ActionResult Manage()
        {
            MyWebApplication.Areas.Admin.Models.UserProfile model = new MyWebApplication.Areas.Admin.Models.UserProfile();
            manager.mOpenConnection();

            long userId = -1;

            try
            {
                if (User.Identity is System.Security.Principal.WindowsIdentity)
                {
                    userId = manager.getUserIdFromSID(new System.Security.Principal.SecurityIdentifier((User.Identity as System.Security.Principal.WindowsIdentity).User.Value));
                }
                else if (User.Identity is MyUsers.MyCustomIdentity)
                {
                    userId = (User.Identity as MyUsers.MyCustomIdentity).UserId;
                }
                else if (User.Identity is System.Web.Security.FormsIdentity)
                {
                    userId = (Session["MySessionData"] as MyManagerCSharp.MySessionData).UserId;
                }

               // MyWebApplication.Controllers.AccountController.setUserProfileModel(model, userId);
            }
            finally
            {
                manager.mCloseConnection();
            }
            return View(model);
        }



        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            LogOnModel model = new LogOnModel();
            model.Password = "";
            model.UserName = "roberto.rutigliano";
            model.Password = "r0b3rt0";
            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LogOnModel model, string returnUrl)
        {
            TempData["AREA"] = "Admin";

            MyWebApplication.Controllers.AccountController controller = new MyWebApplication.Controllers.AccountController();
            controller.ControllerContext = ControllerContext;
            controller.TempData = TempData;
            return controller.Login(model, returnUrl);
        }


        //
        // POST: /Account/LogOff

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            TempData["AREA"] = "Admin";

            MyWebApplication.Controllers.AccountController controller = new MyWebApplication.Controllers.AccountController();
            controller.ControllerContext = ControllerContext;
            controller.TempData = TempData;
            return controller.LogOff();

            //WebSecurity.Logout();
            //FormsAuthentication.SignOut();

            //return RedirectToAction("Index", "Admin");
        }
    }
}