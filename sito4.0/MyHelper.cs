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
}
