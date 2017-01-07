using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class MyMessage
{
    public enum MyMessageType
    {
        Success,
        Failed,
        Warning,
        Exception
    }

    public string testo { get; set; }
    public MyMessageType tipo { get; set; }

    public MyMessage(MyMessageType tipo, string messaggio)
    {
        this.testo = messaggio;
        this.tipo = tipo;
    }

    public MyMessage(Exception ex)
    {
        this.testo = ex.Message;
        if (ex.InnerException != null)
        {
            this.testo += Environment.NewLine + "InnerException: " + ex.InnerException.Message;
        }
        this.tipo = MyMessageType.Exception;
    }
}
