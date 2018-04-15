using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace UnitTestProject
{
    [TestClass]
    public class TestEmail
    {

        private string to = "";

        [TestMethod]
        public void Aruba()
        {

            MyManagerCSharp.MailMessageManager mail = new MyManagerCSharp.MailMessageManager(System.Configuration.ConfigurationManager.AppSettings["application.url"], System.Configuration.ConfigurationManager.AppSettings["application.name"]);

            mail.Subject = System.Configuration.ConfigurationManager.AppSettings["application.name"] + " - Test Project";

            String esito;

            try
            {
                mail.MailServer = "smtp.trova-libro.it";
                mail.port = 25;
                mail.enableSsl = false;
                mail.enableTls = false;


                mail.username = "webmaster@trova-libro.it";
                mail.password = "";

                mail.To(this.to);

                mail.Body = "TEST by Unit Test Project " + mail.MailServer + " - " + mail.port;

                esito = mail.send();
                Assert.IsNotNull(esito, "Esito is null");
                Assert.AreEqual("", esito, "Errore: " + esito);

                Debug.WriteLine(esito);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Assert.Fail(ex.Message);
            }

        }

        [TestMethod]
        public void ArubaSSL()
        {
            MyManagerCSharp.MailMessageManager mail = new MyManagerCSharp.MailMessageManager(System.Configuration.ConfigurationManager.AppSettings["application.url"], System.Configuration.ConfigurationManager.AppSettings["application.name"]);

            mail.Subject = System.Configuration.ConfigurationManager.AppSettings["application.name"] + " - Generazione Nuova Password";
            

            String esito;

            try
            {


                // SMTP =  25 - 465 - 587 

                //connessione: NON protetta - SSL - TLS (Consigliata)

                // pop3s.aruba.it(995)
                //smtps.aruba.it(465)

                mail.MailServer = "smtps.aruba.it";

                mail.port = 587;
                //mail.port = 465;
                mail.enableSsl = true;
                mail.enableTls = false;

                mail.username = "";
                mail.password = "";

                mail.To(this.to);
                mail.Body = "TEST by Unit Test Project " + mail.MailServer + " - " + mail.port;

                esito = mail.send();
                Assert.IsNotNull(esito, "Esito is null");
                Assert.AreEqual("", esito, "Errore: " + esito);

                Debug.WriteLine(esito);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Assert.Fail(ex.Message);
            }

        }



        [TestMethod]
        public void GMail()
        {

            MyManagerCSharp.MailMessageManager mail = new MyManagerCSharp.MailMessageManager(System.Configuration.ConfigurationManager.AppSettings["application.url"], System.Configuration.ConfigurationManager.AppSettings["application.name"]);
            mail.Subject = System.Configuration.ConfigurationManager.AppSettings["application.name"] + " - Test Project";

            String esito;

            try
            {
                mail.MailServer = "smtp.gmail.com";

                //Try port 587 instead of 465.Port 465 is technically deprecated.
                //The .NET SmtpClient only supports encryption via STARTTLS. If the EnableSsl flag is set, the server must respond to EHLO with a STARTTLS, otherwise it will throw an exception. 
                
                mail.port = 587;
                //mail.port = 465;
                mail.enableSsl = true;
                mail.enableTls = false;

                //CON LA VERIFICA IN 2 PASSAGGI occorre creare una password per l'applicazione
                mail.username = "roberto";
                mail.password = "";


                //SENZA LA VERIFICA IN 2 PASSAGGI
                //Impostazioni su account Google:
                //Accesso e sicurezza => Consenti app meno sicure: ON
                

                // mail.username = "angela";
                //mail.password = "";


                mail.To(this.to);

                mail.Body = "TEST by Unit Test Project " + mail.MailServer + " - " + mail.port;

                esito = mail.send();

                Assert.IsNotNull(esito, "Esito is null");
                Assert.AreEqual("", esito, "Errore: " + esito);

                Debug.WriteLine(esito);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Assert.Fail(ex.Message);
            }
        }


        [TestMethod]
        public void Office365()
        {
            MyManagerCSharp.MailMessageManager mail = new MyManagerCSharp.MailMessageManager(System.Configuration.ConfigurationManager.AppSettings["application.url"], System.Configuration.ConfigurationManager.AppSettings["application.name"]);
            mail.Subject = System.Configuration.ConfigurationManager.AppSettings["application.name"] + " - Test Project";

            String esito;

            try
            {
                mail.MailServer = "smtp.office365.com";
                mail.port = 587;
                mail.enableSsl = true;
                mail.enableTls = false;

                mail.username = "";
                mail.password = "";

                
                mail.To(this.to);

                mail.Body = "TEST by Unit Test Project " + mail.MailServer + " - " + mail.port;

                esito = mail.send();

                Assert.IsNotNull(esito, "Esito is null");
                Assert.AreEqual("", esito, "Errore: " + esito);

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
