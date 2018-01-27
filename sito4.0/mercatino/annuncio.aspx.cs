using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MyWebApplication.mercatino
{
    public partial class annuncio : System.Web.UI.Page
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            long annuncioId;

            if (Request["annuncioId"] == null)
            {
                annuncioId = -1;
            }
            else
            {
                annuncioId = long.Parse(Request["annuncioId"]);
            }

            /*
                        string referrer;
                        if (Request.UrlReferrer != null)
                        {
                            referrer = Request.UrlReferrer.AbsoluteUri;
                        }
                        else
                        {
                            referrer = "is null";
                        }


                        MyManagerCSharp.MyException ex = new MyManagerCSharp.MyException("Redirect annuncio.aspx");
                        MyManagerCSharp.MailMessageManager mail = new MyManagerCSharp.MailMessageManager(System.Configuration.ConfigurationManager.AppSettings["application.name"], System.Configuration.ConfigurationManager.AppSettings["application.url"]);
                        mail.sendException(ex, String.Format("Referrer: {0}", referrer));
            */

            Response.Redirect("~/Libri/Details/" + annuncioId);
        }
    }
}