using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;


public class MyHelper
{

    public static void printRequest(System.Web.HttpRequest request)
    {
        foreach (string k in request.Form.AllKeys)
        {
            Debug.WriteLine(String.Format("Key: {0} \t Value: {1}", k, request[k]));
        }
    }


    public static void printRequest(System.Web.HttpRequestBase request)
    {

        foreach (string k in request.Form.AllKeys)
        {
            Debug.WriteLine(String.Format("Key: {0} \t Value: {1}", k, request[k]));
        }

    }

    public static HtmlString getProfileImage(string ServerRootPath, string login, long customerId)
    {
        return MyHelper.getProfileImage(ServerRootPath, login, customerId, -1);
    }

    public static HtmlString getProfileImage(string ServerRootPath, string login, long customerId, long annuncioId)
    {

        string pathImage;
        pathImage = "/public/UserFiles/" + login + "/photo.gif";

        string temp;

        //System.Diagnostics.Debug.WriteLine(String.Format ("Login: {0} CustomerId: {1}", login , customerId ));

        if (!System.IO.File.Exists(ServerRootPath + pathImage))
        {

            if (customerId == -1)
            {
                //pathImage = "/Content/Images/immobiliare/_privato.gif";
                pathImage = "/Content/Images/shared/anonymous.gif";
            }
            else
            {
                pathImage = "/Content/Images/immobiliare/_agenzia.gif";
                //pathImage = "~/Content/themes/base/images/immobiliare/_agenzia.gif";
            }
        }

        string toolTip;
        if (customerId == -1)
        {
            toolTip = "Utente privato: " + login;
        }
        else
        {
            toolTip = "Agenzia immobiliare: " + login;
        }

        //pathImage = System.Web.Mvc.UrlHelper.GenerateContentUrl(pathImage, Context);

        //temp = String.Format("<img src=\"{0}\" title=\"{1}\" />", pathImage, toolTip);


        temp = String.Format("<img src=\"{0}\" title=\"{1}\" height=\"80\" width=\"80\" /></a>", pathImage, toolTip);

        if (annuncioId != -1)
        {

            temp = String.Format("<a href=\"\\Immobiliare\\Details\\{0}\">{1}</a>", annuncioId, temp);
        }


        return new HtmlString(temp);
    }



    #region "Decode NULL"



    public static HtmlString decodeNull(DateTime? valore)
    {

        if (valore == null || valore == DateTime.MinValue)
        {
            return new HtmlString("N/A");
        }


        return new HtmlString(valore.ToString());
    }

    public static HtmlString decodeNull(bool? valore)
    {

        if (valore == null)
        {
            return new HtmlString("N/A");
        }


        return new HtmlString(valore.ToString());
    }


    public static HtmlString decodeNull(string valore)
    {

        if (String.IsNullOrEmpty(valore))
        {
            return new HtmlString("N/A");
        }


        return new HtmlString(valore.Replace(Environment.NewLine, "<br />"));
    }

    public static HtmlString decodeNull(decimal? valore)
    {

        if (valore == null)
        {
            return new HtmlString("N/A");
        }


        return new HtmlString(valore.Value.ToString());
    }


    public static HtmlString decodeNull(int? valore)
    {

        if (valore == null)
        {
            return new HtmlString("N/A");
        }


        return new HtmlString(valore.Value.ToString());
    }


    public static HtmlString decodeEnum(string value)
    {
        if (String.IsNullOrEmpty(value) || (value == "0") || (value == "Undefined"))
        {
            return new HtmlString("N/A");
        }


        return new HtmlString(value.Replace("_", " "));
    }


    public static HtmlString decodeNull(double? valore)
    {

        if (valore == null)
        {
            return new HtmlString("N/A");
        }


        return new HtmlString(valore.Value.ToString());
    }

    #endregion

}
