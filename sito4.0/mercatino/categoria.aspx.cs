using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MyWebApplication.mercatino
{
    public partial class categoria : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            long categoria_id;

            if (Request["categoria_id"] == null)
            {
                categoria_id = -1;
            }
            else
            {
                try
                {
                    categoria_id = long.Parse(Request["categoria_id"]);
                }
                catch (Exception ex)
                {
                    categoria_id = -1;
                }
            }

            Response.Redirect("~/Libri/Index?filter.categoriaId=" + categoria_id);
            //Server.Transfer("~/Libri/Index?filter.categoriaId=" + categoria_id, false);
        }
    }
}