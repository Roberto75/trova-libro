

public class MyConstants
{
    public enum Esito
    {
        Ok,
        Errore
    }

    public const string MY_SESSION_ACTIVITY_LOG_ID = "__MySessionActivityLog";

    public static System.Globalization.CultureInfo CultureInfoEN = new System.Globalization.CultureInfo("en-GB");

    public static MyManagerCSharp.ManagerDB.Days DefaultDay = MyManagerCSharp.ManagerDB.Days.Anno_corrente;






    // jqueryMobile
    /*public const string TABLE_STYLE = "MyTable ui-responsive";
    public const string TABLE_ROW_STYLE = "ui-bar-a";
    public const string TABLE_ALTERNATING_ROW_STYLE = "ui-bar-b";
    public const string TABLE_HEADER_STYLE = "ui-bar-c";
    public const string TABLE_FOOTER_STYLE = "ui-bar-b";
    */


    public const string TABLE_STYLE = "table table-hover table-striped table-bordered";
    public const string TABLE_ROW_STYLE = "";
    public const string TABLE_ALTERNATING_ROW_STYLE = "";
    public const string TABLE_HEADER_STYLE = "";
    public const string TABLE_FOOTER_STYLE = "";


    public const string FIXED_FOOTER_THEME = "b";
}
