using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace UnitTestProject
{
    [TestClass]
    public class DataBase
    {
        [TestMethod]
        public void TestConnection()
        {
            MyManagerCSharp.ManagerDB manager = new MyManagerCSharp.ManagerDB("utenti");

            try
            {
                manager.mOpenConnection();

                string strSQL;
                int iConta;



                strSQL = "SELECT COUNT(*) as Exists from MsysObjects WHERE type = 1 AND name = 'UTENTI'";
                strSQL = "SELECT COUNT(*) from MSysObjects";


                string sConta = manager.mExecuteScalar(strSQL);
                Debug.WriteLine("Numero utenti: " + sConta);

                if (int.Parse(sConta) > 0)
                {
                    Debug.WriteLine("ESISTE");
                }
                else
                {
                    Debug.WriteLine("NON ESISTE");
                }


            }
            finally
            {
                manager.mCloseConnection();
            }

        }




        [TestMethod]
        public void SelectFromUtenti()
        {
            MyManagerCSharp.ManagerDB manager = new MyManagerCSharp.ManagerDB("utenti");

            try
            {
                manager.mOpenConnection();

                string strSQL;
                int iConta;



                strSQL = "SELECT COUNT(*) FROM UTENTI";

                string sConta = manager.mExecuteScalar(strSQL);
                Debug.WriteLine("Numero utenti: " + sConta);

                if (int.Parse(sConta) > 0)
                {
                    Debug.WriteLine("ESISTE");
                }
                else
                {
                    Debug.WriteLine("NON ci sono utenti");
                }


            }
            finally
            {
                manager.mCloseConnection();
            }

        }



        [TestMethod]
        public void Upgrade01()
        {
            MyManagerCSharp.ManagerDB manager = new MyManagerCSharp.ManagerDB("utenti");
            try
            {
                manager.mOpenConnection();

                string strSQL;
                int iConta;
                string sConta;

                strSQL = "SELECT COUNT(*) FROM UTENTE";
                sConta = manager.mExecuteScalar(strSQL);
                Debug.WriteLine("Numero utenti: " + sConta);


                strSQL = "SELECT date_last_login FROM UTENTE";
                try
                {
                    sConta = manager.mExecuteScalar(strSQL);

                }
                catch (Exception ex)
                {
                    strSQL = "ALTER TABLE UTENTE ADD date_last_login  DATETIME NULL ";
                    iConta = manager.mExecuteNoQuery(strSQL);
                    Debug.WriteLine(": " + iConta);
                }


                strSQL = "SELECT date_previous_login FROM UTENTE";
                try
                {
                    sConta = manager.mExecuteScalar(strSQL);

                }
                catch (Exception ex)
                {
                    strSQL = "ALTER TABLE UTENTE ADD date_previous_login DATETIME NULL ";
                    iConta = manager.mExecuteNoQuery(strSQL);
                    Debug.WriteLine(": " + iConta);
                }


                strSQL = "SELECT login_success FROM UTENTE";
                try
                {
                    sConta = manager.mExecuteScalar(strSQL);
                }
                catch (Exception ex)
                {
                    strSQL = "ALTER TABLE UTENTE ADD login_success INT  NULL ";
                    iConta = manager.mExecuteNoQuery(strSQL);
                    Debug.WriteLine(": " + iConta);
                }




                strSQL = "SELECT count(*) FROM GRUPPO";
                try
                {
                    sConta = manager.mExecuteScalar(strSQL);
                }
                catch (Exception ex)
                {
                    strSQL = " CREATE TABLE [Gruppo] ( [gruppo_id]  AUTOINCREMENT PRIMARY KEY, " +
                        " [nome] text(50) NULL," +
                        " [date_added]  datetime   NULL " +
                        " )";

                    iConta = manager.mExecuteNoQuery(strSQL);
                    Debug.WriteLine(": " + iConta);
                }




                strSQL = "SELECT count(*) FROM RUOLO";
                try
                {
                    sConta = manager.mExecuteScalar(strSQL);
                }
                catch (Exception ex)
                {
                    strSQL = " CREATE TABLE [RUOLO] ( [ruolo_id]  text(30) PRIMARY KEY, " +
                        " [nome] text(50) NULL," +
                        " [date_added]  datetime   NULL " +
                        " )";

                    iConta = manager.mExecuteNoQuery(strSQL);
                    Debug.WriteLine(": " + iConta);
                }


                strSQL = "SELECT count(*) FROM Profilo";
                try
                {
                    sConta = manager.mExecuteScalar(strSQL);
                }
                catch (Exception ex)
                {
                    strSQL = " CREATE TABLE [Profilo] ( [profilo_id]  text(20) PRIMARY KEY, " +
                        " [nome] text(50) NULL," +
                        " [date_added]  datetime   NULL " +
                        " )";

                    iConta = manager.mExecuteNoQuery(strSQL);
                    Debug.WriteLine(": " + iConta);
                }


    

                strSQL = "SELECT count(*) FROM GruppoRuolo";
                try
                {
                    sConta = manager.mExecuteScalar(strSQL);
                }
                catch (Exception ex)
                {
                    strSQL = " CREATE TABLE [GruppoRuolo] (  " +
                        " [gruppo_id]  number NOT NULL, " +
                        " [ruolo_id]  text(30) NOT NULL, " +
                        " [date_added]  datetime   NULL " +
                        " )";

                    iConta = manager.mExecuteNoQuery(strSQL);
                    Debug.WriteLine(": " + iConta);
                }


                strSQL = "SELECT count(*) FROM UtenteGruppo";
                try
                {
                    sConta = manager.mExecuteScalar(strSQL);
                }
                catch (Exception ex)
                {
                    strSQL = " CREATE TABLE [UtenteGruppo] (  " +
                        " [user_id]  number NOT NULL, " +
                        " [gruppo_id]  number NOT NULL, " +
                        " [date_added]  datetime   NULL " +
                        " )";

                    iConta = manager.mExecuteNoQuery(strSQL);
                    Debug.WriteLine(": " + iConta);
                }



                strSQL = "SELECT count(*) FROM UtenteProfilo";
                try
                {
                    sConta = manager.mExecuteScalar(strSQL);
                }
                catch (Exception ex)
                {
                    strSQL = " CREATE TABLE [UtenteProfilo] (  " +
                        " [user_id]  number NOT NULL, " +
                        " [profilo_id]   text(20) NOT NULL, " +
                        " [date_added]  datetime   NULL " +
                        " )";

                    iConta = manager.mExecuteNoQuery(strSQL);
                    Debug.WriteLine(": " + iConta);
                }



                strSQL = "SELECT COUNT(*) FROM GRUPPO WHERE NOME = 'Administrators'";
                try
                {
                    sConta = manager.mExecuteScalar(strSQL);
                    if (int.Parse(sConta) == 0)
                    {
                        strSQL = "INSERT INTO GRUPPO ( nome, date_added) VALUES('Administrators' ,  GetDate())";
                        iConta = manager.mExecuteNoQuery(strSQL);
                        Debug.WriteLine(": " + iConta);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    Assert.Fail();
                }



                int userdId = 0;
                strSQL = "SELECT USER_ID FROM UTENTE WHERE MY_LOGIN = 'roberto.rutigliano'";
                sConta = manager.mExecuteScalar(strSQL);
                if (sConta != "")
                {
                    userdId = int.Parse(sConta);
                }


                int gruppoId = 0;
                strSQL = "SELECT GRUPPO_ID FROM GRUPPO WHERE NOME  = 'Administrators'";
                sConta = manager.mExecuteScalar(strSQL);
                if (sConta != "")
                {
                    gruppoId = int.Parse(sConta);
                }

                if (userdId != 0 && gruppoId != 0)
                {
                    strSQL = "INSERT INTO UtenteGruppo(user_id, gruppo_id, date_added) VALUES(" +  userdId + ", " + gruppoId +", GetDate())";
                    iConta = manager.mExecuteNoQuery(strSQL);
                    Debug.WriteLine(": " + iConta);
                }

                


            }
            finally
            {
                manager.mCloseConnection();
            }





            




        }


        [TestMethod]
        public void RenameTableUtentiInUtente()
        {
            MyManagerCSharp.ManagerDB manager = new MyManagerCSharp.ManagerDB("utenti");

            try
            {
                manager.mOpenConnection();

                string strSQL;
                int iConta;

                strSQL = "SELECT COUNT(*) FROM UTENTI";

                string sConta = manager.mExecuteScalar(strSQL);
                Debug.WriteLine("Numero utenti: " + sConta);


                if (int.Parse(sConta) > 0)
                {
                    strSQL = "SELECT * INTO UTENTE FROM UTENTI";
                    iConta = manager.mExecuteNoQuery(strSQL);
                    Debug.WriteLine(": " + iConta);


                    strSQL = "DROP TABLE UTENTI";
                    iConta = manager.mExecuteNoQuery(strSQL);
                    Debug.WriteLine(": " + iConta);
                }


                //strSQL = "ALTER TABLE UTENTI RENAME TO UTENTE ";
                //int iConta = manager.mExecuteNoQuery(strSQL);
                //Debug.WriteLine(": " + iConta);




            }
            finally
            {
                manager.mCloseConnection();
            }

        }

    }
}
