using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace UnitTestProject
{
    [TestClass]
    public class TestEmail
    {
        [TestMethod]
        public void TestMethod1()
        {

           // SMTP =  25 - 465 - 587 
           
            //connessione: NON protetta - SSL - TLS (Consigliata)

            // pop3s.aruba.it(995)
            //smtps.aruba.it(465)


            MyManagerCSharp.MailMessageManager mail = new MyManagerCSharp.MailMessageManager(System.Configuration.ConfigurationManager.AppSettings["application.url"], System.Configuration.ConfigurationManager.AppSettings["application.name"]);

            mail.Subject = System.Configuration.ConfigurationManager.AppSettings["application.name"] + " - Generazione Nuova Password";

            mail.Body = "TEST by Unit Test Project";

            String esito;

            try
            {
                mail.To("roberto.rutigliano@gmail.com");
                esito = mail.send();

                Debug.WriteLine(esito);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Assert.Fail(ex.Message);
            }

        }
    }
}
