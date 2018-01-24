using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public static class MyConstantsAdmin
{
    public enum Esito
    {
        Ok,
        Errore
    }


    public static System.Globalization.CultureInfo CultureInfoEN = new System.Globalization.CultureInfo("en-GB");

    public static MyManagerCSharp.ManagerDB.Days DefaultDay = MyManagerCSharp.ManagerDB.Days.Anno_corrente;

    public const string CONNECTION_LOG = "log";


    public const string TABLE_STYLE = "table table-hover table-striped table-responsive-md";
    public const string TABLE_ROW_STYLE = "";
    public const string TABLE_ALTERNATING_ROW_STYLE = "";
    public const string TABLE_HEADER_STYLE = "thead-light";
    public const string TABLE_FOOTER_STYLE = "";

    //public const string FIXED_FOOTER_THEME = "b";

    public const string UI_BLOCK_HEADER = "ui-bar ui-bar-b";
    public const string UI_BLOCK_BODY = "ui-bar ui-body-a";

    public const string TAB_NAVBAR_BUTTON_THEME = "e";

    public const string BUTTON_THEME_UI = "ui-btn-b";
    public const string BUTTON_THEME = "b";
}
