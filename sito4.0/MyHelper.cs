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
