using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;

namespace MyWebApplication.Controllers
{
    [Authorize]
    public class LibriController : MyBaseController
    {


        public const int MaxWidthImage = 500;
        public const int MaxHeightImage = 500;

        public const int WidthThumbnail = 200;
        public const int HeightThumbnail = 200;

        private const string SESSSION_FILTER_SEARCH = "SearchLibriModel";

        private Annunci.Libri.LibriManager manager = new Annunci.Libri.LibriManager("mercatino");

        private MyManagerCSharp.RegioniProvinceComuniManager regioniProvinceComuniManager = new MyManagerCSharp.RegioniProvinceComuniManager("RegioniProvinceComuni");


        [AllowAnonymous]
        public ActionResult Index(Annunci.Libri.Models.SearchLibri model)
        {

            if (Request.HttpMethod.ToString() == "GET" && Session[SESSSION_FILTER_SEARCH] != null && Request.UrlReferrer != null && Request.UrlReferrer.LocalPath != "/Libri/Categorie")
            {
                model = (Session[SESSSION_FILTER_SEARCH] as Annunci.Libri.Models.SearchLibri);
                //model.filter = (Session[SESSSION_FILTER_SEARCH] as Annunci.Libri.Models.Libro);
                Debug.WriteLine("Leggo i parametri di ricerca dalla sessione ...");
                Debug.WriteLine("Filtro Days: " + model.days);
            }

            if (model.filter == null)
            {
                model.filter = new Annunci.Libri.Models.Libro();
            }
            Debug.WriteLine("collapseShow: " + model.collapseShow);
            Debug.WriteLine("CategoriaId: " + model.filter.categoriaId);
            Debug.WriteLine("SubCategoriaId: " + model.filter.subCategoriaId);

            System.Collections.Hashtable hashtablePhoto = new System.Collections.Hashtable();
            try
            {
                manager.mOpenConnection();

                model.comboCategorie = manager.getComboCategoria(1000000);
                if (model.filter.categoriaId != null &&
                    ((model.filter.categoriaId >= 1130000 && model.filter.categoriaId < 1140000) || (model.filter.categoriaId >= 1140000 && model.filter.categoriaId < 1150000)))
                {
                    model.comboSubCategorie = manager.getComboCategoria((int)model.filter.categoriaId);
                }

                model.comboRegioni = regioniProvinceComuniManager.getComboRegioni();
                if (model.filter.regioneId != null)
                {
                    model.comboProvince = regioniProvinceComuniManager.getComboProvince((int)model.filter.regioneId);
                }

                if (model.filter.provinciaId != null)
                {
                    model.comboComuni = regioniProvinceComuniManager.getComboComuni(model.filter.provinciaId);
                }

                manager.getList(model);


                long numeroPhoto;
                foreach (Annunci.Libri.Models.Libro i in model.Libri)
                {
                    numeroPhoto = manager.countPhoto(i.annuncioId);
                    hashtablePhoto.Add(i.annuncioId, numeroPhoto);
                }


                if (model.filter.categoriaId == 1130000 && model.filter.subCategoriaId == null)
                {
                    model.filter.categoria = "Testi scolastici";
                }

                if (model.filter.categoriaId == 1140000 && model.filter.subCategoriaId == null)
                {
                    model.filter.categoria = "Testi universitari";
                }

               /* 
                //gestione nome categoria 
                switch (model.filter.subCategoriaId)
                {
                    case 1130100:
                        model.filter.categoria = "Testi scolastici elemetari";
                        break;
                    case 1130200:
                        model.filter.categoria = "Testi scolastici medie";
                        break;
                    case 1130300:
                        model.filter.categoria = "Testi scolastici superiori";
                        break;
                    case 1130400:
                        model.filter.categoria = "Testi scolastici altro";
                        break;
                }

    */

            }
            finally
            {
                manager.mCloseConnection();
            }

            Session[SESSSION_FILTER_SEARCH] = model;
            ViewData["hashtablePhoto"] = hashtablePhoto;
            return View("Index", model);
        }


        [AllowAnonymous]
        public ActionResult Categorie()
        {
            return View();
        }


        [AllowAnonymous]
        public ActionResult Categoria(int categoriaId)
        {
            Debug.WriteLine("Categoria: " + categoriaId);
            Annunci.Libri.Models.SearchLibri model = new Annunci.Libri.Models.SearchLibri();

            if (categoriaId != -1)
            {
                model.filter.categoriaId = categoriaId;
            }
            return RedirectToAction("Index", model);
        }



        public ActionResult MyAnnunci(Annunci.Libri.Models.SearchLibri model)
        {
            System.Collections.Hashtable hashtableRisposte = new System.Collections.Hashtable();
            try
            {
                manager.mOpenConnection();

                manager.getMyListAnnunci(MySessionData.UserId, model);

                Annunci.AnnunciManager annuncioManager = new Annunci.AnnunciManager(manager.mGetConnection());
                long numeroRisposte;
                foreach (Annunci.Libri.Models.Libro i in model.Libri)
                {
                    numeroRisposte = annuncioManager.getNumeroRisposteOfAnnuncio(i.annuncioId, MySessionData.UserId);
                    hashtableRisposte.Add(i.annuncioId, numeroRisposte);
                }
            }
            finally
            {
                manager.mCloseConnection();
            }

            ViewData["hashtableRisposte"] = hashtableRisposte;
            return View(model);
        }

        public ActionResult MyTrattative()
        {
            manager.mOpenConnection();

            List<Annunci.Models.Trattativa> risultato;

            try
            {
                risultato = manager.getListTrattative(MySessionData.UserId, Annunci.Models.Trattativa.TipoTrattativa.Libro);
            }
            finally
            {
                manager.mCloseConnection();
            }


            return View(risultato);
        }


        [Authorize]
        public ActionResult Trattativa(long? id)
        {
            Debug.WriteLine("trattativaId: " + id);
            Models.ModelTrattativa model = new Models.ModelTrattativa();

            manager.mOpenConnection();
            try
            {
                //La coppa trattativaID e annuncioID sono + sicuri per leggere gli annunci
                //if (annuncioId == null) {
                //  annuncioId = manager.getAnnucioIdFromTrattativa((long)trattativaId);
                //}

                model.trattativa = manager.getTrattativa((long)id);
                if (model.trattativa == null)
                {
                    return View("NotAvailable");
                }

                if (!manager.authorizeShowTrattativa(MySessionData.UserId, (long)id))
                {
                    return RedirectToAction("NotAuthorized", "Errors");
                }


                manager.setRisposteFromTrattativa(model.trattativa);


                model.libro = manager.getLibro(model.trattativa.annuncioId);
                if (model.libro == null)
                {
                    //vuol dire che l'annuncio è stato rimosso ... 
                    //return View("NotAvailable");

                    // il controllo lo faccio sulla VIEW così visualizzo i pulsanti corretti
                }


                //Verifico se l'utente che ha inserito la risposta sia il prorietario dell'annuncio
                bool isOwner;
                isOwner = manager.isOwner(model.trattativa.annuncioId, MySessionData.UserId);

                if (isOwner)
                {
                    manager.updateNotificaLetturaRispostaOwner((long)id);
                }
                else
                {
                    manager.updateNotificaLetturaRispostaUser((long)id);
                }

            }
            finally
            {
                manager.mCloseConnection();
            }


            if (TempData["AREA"] != null && TempData["AREA"].ToString() == "Admin")
            {
                return View("~/Areas/Admin/Views/Annunci/Trattativa.cshtml", model);
            }

            return View(model);
        }


        private ActionResult AnnuncioNotFound(long? id)
        {
            string messaggio;
            messaggio = String.Format("Annuncio con id {0} == NULL", id);


            messaggio += getMessageLog();

            MyManagerCSharp.MyException exception = new MyManagerCSharp.MyException(MyManagerCSharp.MyException.ErrorNumber.Codice_id_non_valido, messaggio);
            TempData["MyException"] = exception;
            return RedirectToAction("NotAvailable", "Errors");
        }



        [AllowAnonymous]
        public ActionResult Details(long id)
        {

            Models.DetailsModel model = new Models.DetailsModel();


            manager.mOpenConnection();
            try
            {
                model.libro = manager.getLibro(id);

                if (model.libro == null)
                {
                    return AnnuncioNotFound(id);
                }


                //CLICK
                if (User.Identity.IsAuthenticated && MySessionData != null)
                {
                    //if ((User.Identity as MyUsers.MyCustomIdentity).UserId != immobile.userId)
                    if (MySessionData.UserId != model.libro.userId)
                    {
                        //se non si tratta di un mio annuncio ...
                        manager.annuncioAddClick(id);
                    }
                }
                else
                {
                    //nel caso di connessioni anonime non posso fare nulla
                    manager.annuncioAddClick(id);
                }


                Annunci.PhotoManager photoManager = new Annunci.PhotoManager(manager.mGetConnection());
                model.photos = photoManager.getMyPhotosIsNotPlanimetria(id);
                Debug.WriteLine("Trovate {0} immagini", model.photos.Count);
            }
            finally
            {
                manager.mCloseConnection();
            }

            string temp = "";

            temp = String.IsNullOrEmpty(model.libro.titolo) ? "" : model.libro.titolo;
            temp += String.IsNullOrEmpty(model.libro.autore) ? "" : "," + model.libro.autore;
            temp += String.IsNullOrEmpty(model.libro.isbn) ? "" : ", ISBN " + model.libro.isbn;

            ViewBag.Title = String.Format("{0} libro {1}", model.libro.tipo, temp);
            ViewBag.Description = String.Format("{0}, libro usato, {1}", model.libro.tipo, temp);
            ViewBag.Keywords = String.Format("{0}, libro, libro usato, {1}", model.libro.tipo, temp);

            return View(model);
        }


        public ActionResult Reply(long annuncioId, long? trattativaId, long? quote, long? rispostaId)
        {
            Models.ReplyModel model;
            model = getReplyModel(annuncioId, trattativaId, quote, rispostaId);

            return View(model);
        }


        private Models.ReplyModel getReplyModel(long annuncioId, long? trattativaId, long? quote, long? rispostaId)
        {

            Models.ReplyModel model = new Models.ReplyModel();
            model.annuncioId = annuncioId;

            if (trattativaId == null)
            {
                model.trattativaId = -1;
            }
            else
            {
                model.trattativaId = (long)trattativaId;
            }


            try
            {
                manager.mOpenConnection();

                Annunci.Libri.Models.Libro libro;
                libro = manager.getLibro(annuncioId);

                model.annuncio = libro;

                if (quote != null)
                {
                    Annunci.Models.Risposta risposta;
                    risposta = manager.getRisposta((long)quote);
                    model.testo = "<blockquote><hr/><b>" + risposta.login + "</b> scrive: </br></br>" + risposta.testo + "<hr/></blockquote>";
                    model.rispostaId = (long)quote;
                    model.replyTo = risposta.login;
                }
                else
                {
                    model.testo = "";
                    if (rispostaId == null)
                    {
                        model.rispostaId = -1;
                        model.replyTo = libro.login;
                    }
                    else
                    {
                        model.rispostaId = (long)rispostaId;
                        Annunci.Models.Risposta risposta;
                        risposta = manager.getRisposta((long)rispostaId);
                        model.replyTo = risposta.login;
                    }

                }

            }
            finally
            {
                manager.mCloseConnection();
            }

            return model;
        }



        [HttpPost, ActionName("Reply")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult ReplyPost(long annuncioId, string testo, long? rispostaId, long? trattativaId)
        {

            if (String.IsNullOrEmpty(testo.Trim()))
            {
                ModelState.AddModelError("", "Inserire un messaggio");

                Models.ReplyModel model;
                model = getReplyModel(annuncioId, trattativaId, null, rispostaId);

                model.testo = testo;
                return View(model);
            }


            if (ModelState.IsValid)
            {
                try
                {
                    manager.mOpenConnection();

                    if (rispostaId == null || rispostaId == -1)
                    {
                        //' si tratta di una nuova trattativa!!
                        trattativaId = manager.insertTrattativa(annuncioId, MySessionData.UserId);
                        manager.rispondi((long)trattativaId, MySessionData.UserId, testo);
                    }
                    else
                    {
                        manager.rispondi((long)trattativaId, MySessionData.UserId, testo, (long)rispostaId);
                    }


                    bool isOwner;
                    isOwner = manager.isOwner(annuncioId, MySessionData.UserId);

                    Annunci.Libri.Models.Libro libro;
                    libro = manager.getLibro(annuncioId);

                    //*** EMAIL ***
                    System.Data.DataTable dt;
                    dt = manager.getEmailReplyAnnnuncio((long)trattativaId);

                    Annunci.Libri.LibriMailMessageManager mail = new Annunci.Libri.LibriMailMessageManager(System.Configuration.ConfigurationManager.AppSettings["application.name"], System.Configuration.ConfigurationManager.AppSettings["application.url"]);
                    mail.Subject = System.Configuration.ConfigurationManager.AppSettings["application.name"] + " - Nuovo messaggio";
                    mail.Body = mail.getBodyNuovoMessaggioReply((long)trattativaId, annuncioId, libro.titolo, String.Format("Libri/Trattativa/{0}", trattativaId));

                    if (isOwner)
                    {
                        mail.To(dt.Rows[0]["email"].ToString());
                    }
                    else
                    {
                        mail.To(dt.Rows[0]["email_owner"].ToString());
                    }

                    //MY-DEBUGG
                    //' mail._ToClearField()
                    //'mail._To("roberto.rutigliano@gmail.com")

                    mail.Bcc(System.Configuration.ConfigurationManager.AppSettings["mail.To.Ccn"]);
                    mail.send();


                    //'l'inserimento di una nuova risposta comporta la notifica del messaggio
                    //'ci passo l'id dell'utente per facilitare la ricerca 
                    if (isOwner)
                    {
                        manager.notificaUser((long)trattativaId, long.Parse(dt.Rows[0]["user_id"].ToString()));
                    }
                    else
                    {
                        manager.notificaOwner((long)trattativaId, long.Parse(dt.Rows[0]["user_id_owner"].ToString()));
                    }
                }
                finally
                {
                    manager.mCloseConnection();
                }
            }

            //return RedirectToAction("Details", new { id = annuncioId });
            return RedirectToAction("MyTrattative");
        }





        [AllowAnonymous]
        public ActionResult GetSubCategoria(long? categoriaId)
        {
            Debug.WriteLine("GetSubCategoria: " + categoriaId);

            List<MyManagerCSharp.Models.MyItem> items;
            items = new List<MyManagerCSharp.Models.MyItem>();

            if (categoriaId == null)
            {
                return Json(items, JsonRequestBehavior.AllowGet);
            }

            items = manager.getComboCategoria((long)categoriaId);
            return Json(items, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Create(Models.CreateModel model)
        {
            //Debug.WriteLine(String.Format("Categoria: {0}", model.categoria));
            //Debug.WriteLine(String.Format("Tipo annuncio: {0}", model.autore));
            //< option value = "1000000" > Libri e Riviste +1000000 </ option >
            model.comboCategorie = manager.getComboCategoria(1000000);

            model.comboRegioni = regioniProvinceComuniManager.getComboRegioni();

            model.libro = new Annunci.Libri.Models.Libro();

            return View(model);
        }



        [Authorize]
        [ActionName("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePost(Models.CreateModel model)
        {
            if (model.libro.tipo == null)
            {
                ModelState.AddModelError("", "Il tipo di annuncio è un campo obbligatorio");
            }

            if (model.libro.categoriaId == null)
            {
                ModelState.AddModelError("", "La categoria è un campo obbligatorio");
            }

            if (model.libro.titolo == null)
            {
                ModelState.AddModelError("", "Il titolo è un campo obbligatorio");
            }


            if (!ModelState.IsValid)
            {
                model.comboCategorie = manager.getComboCategoria(1000000);
                model.comboRegioni = regioniProvinceComuniManager.getComboRegioni();
                if (model.libro.categoriaId != null &&
                    ((model.libro.categoriaId >= 1130000 && model.libro.categoriaId < 1140000) || (model.libro.categoriaId >= 1140000 && model.libro.categoriaId < 1150000)))
                {
                    model.comboSubCategorie = manager.getComboCategoria((int)model.libro.categoriaId);
                }
                return View(model);
            }

            Debug.WriteLine("Nota: " + model.libro.nota);


            manager.mOpenConnection();
            long annuncioId;
            try
            {
                annuncioId = manager.insertAnnuncio(model.libro, MySessionData.UserId);
            }
            finally
            {
                manager.mCloseConnection();
            }

            //*** EMAIL ***
            Annunci.Libri.LibriMailMessageManager mail = new Annunci.Libri.LibriMailMessageManager(System.Configuration.ConfigurationManager.AppSettings["application.url"], System.Configuration.ConfigurationManager.AppSettings["application.name"]);
            mail.Subject = System.Configuration.ConfigurationManager.AppSettings["application.name"] + " - Nuovo annuncio";
            mail.Body = mail.getBodyInserimentoNuovoAnnuncio(annuncioId, model.libro.titolo, String.Format("Libri/Details/{0}", annuncioId));
            mail.To(MySessionData.Email);
            //'MY-DEBUGG
            mail.Bcc(System.Configuration.ConfigurationManager.AppSettings["mail.To.Ccn"]);
            mail.send();

            return View("CreateCompleted");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(long? annuncioId)
        {
            if (annuncioId == null)
            {
                return HttpNotFound();
            }
            Debug.WriteLine("Delete MyAnnuncio: " + annuncioId);


            Annunci.Libri.Models.Libro libro;
            try
            {
                manager.mOpenConnection();

                libro = manager.getLibro((long)annuncioId);
                if (libro == null)
                {
                    return HttpNotFound();
                }


            }
            finally
            {
                manager.mCloseConnection();
            }

            ViewBag.Title = "Trova-libro: Cancellazione annuncio";

            return View(libro);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteExecute(long? MyId)
        {

            if (MyId == null)
            {
                throw new MyManagerCSharp.MyException(MyManagerCSharp.MyException.ErrorNumber.ParametroNull, "DeleteExecute");
            }

            Debug.WriteLine("MyAnnuncioId: " + MyId);

            try
            {
                manager.mOpenConnection();

                manager.deleteAnnuncioLogic((long)MyId, Annunci.AnnunciManager.StatoAnnuncio.Da_cancellare, Server.MapPath("~"));

            }
            finally
            {
                manager.mCloseConnection();
            }


            //mando un email a chi ha eseguito la cancellazione
            if (!String.IsNullOrEmpty(MySessionData.Email))
            {
                Annunci.Libri.LibriMailMessageManager mail = new Annunci.Libri.LibriMailMessageManager(System.Configuration.ConfigurationManager.AppSettings["application.name"], System.Configuration.ConfigurationManager.AppSettings["application.url"]);
                mail.Subject = System.Configuration.ConfigurationManager.AppSettings["application.name"] + " - Donazione";
                mail.Body = mail.getBodyDonazione("Home/Donazione");
                mail.To(MySessionData.Email);
                //MY-DEBUGG
                mail.Bcc(System.Configuration.ConfigurationManager.AppSettings["mail.To.Ccn"]);

                mail.send();

            }

            return RedirectToAction("MyAnnunci");
        }





        public ActionResult MyAnnuncio(long id)
        {
            Debug.WriteLine("MyAnnuncioId: " + id);

            System.Collections.Hashtable hashtableRisposte = new System.Collections.Hashtable();

            Models.UpdateModel model;
            model = new Models.UpdateModel();


            manager.mOpenConnection();
            try
            {
                model.libro = manager.getLibro(id);

                if (model.libro == null)
                {
                    return AnnuncioNotFound(id);
                }

                if (model.libro.userId != MySessionData.UserId)
                {
                    string messaggio;
                    messaggio = String.Format("L'utente {0} sta tentando di accedere ad un annuncio non suo", MySessionData.Login);
                    messaggio += String.Format("Url: {0} ", Request.Url.AbsoluteUri);

                    MyManagerCSharp.MyException ex = new MyManagerCSharp.MyException(MyManagerCSharp.MyException.ErrorNumber.UtenteNonAutorizzato, messaggio);
                    TempData["MyException"] = ex;
                    return RedirectToAction("NotAuthorized", "Errors");
                }

                //Trattative in corso 
                model.trattative = manager.getTrattativeOnMyAnnuncio(MySessionData.UserId, id);
                long numeroRisposte;
                foreach (Annunci.Models.Trattativa t in model.trattative)
                {
                    numeroRisposte = manager.getNumeroRisposteOfTrattativa(t.trattativaId, MySessionData.UserId);
                    hashtableRisposte.Add(t.trattativaId, numeroRisposte);
                }

                // PHOTO
                Annunci.PhotoManager photoManager = new Annunci.PhotoManager(manager.mGetConnection());

                List<Annunci.Models.MyPhoto> photos;
                photos = photoManager.getMyPhotosIsNotPlanimetria(id);

                Debug.WriteLine("Trovate {0} immagini", photos.Count);

                if (Request.IsAjaxRequest())
                {
                    Models.GalleryModel modelGallery = new Models.GalleryModel();
                    modelGallery.externalId = model.libro.annuncioId;
                    modelGallery.photos = photos;
                    return PartialView("GalleryEdit", modelGallery);
                }

                model.photos = photos;
            }
            finally
            {
                manager.mCloseConnection();
            }


            ViewData["hashtableRisposte"] = hashtableRisposte;

            string temp = "";

            temp = String.IsNullOrEmpty(model.libro.titolo) ? "" : model.libro.titolo;
            temp += String.IsNullOrEmpty(model.libro.autore) ? "" : "," + model.libro.autore;
            temp += String.IsNullOrEmpty(model.libro.isbn) ? "" : ", ISBN " + model.libro.isbn;

            ViewBag.Title = String.Format("Il mio annuncio: {0} {1}", model.libro.tipo, temp);
            ViewBag.Description = String.Format("{0}, libro usato, {1}", model.libro.tipo, temp);
            ViewBag.Keywords = String.Format("{0}, libro, libro usato, {1}", model.libro.tipo, temp);

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult UpdateNota(long? annuncioId, string nota)
        {
            if (annuncioId == null)
            {
                throw new MyManagerCSharp.MyException(MyManagerCSharp.MyException.ErrorNumber.ParametroNull, "UpdateNota");
            }

            nota = HttpUtility.HtmlDecode(nota);
            Debug.WriteLine("UpdateNota: " + annuncioId + " Nota: " + nota);

            try
            {

                manager.mOpenConnection();

                if (manager.updateAnnuncioDescrizione((long)annuncioId, nota, false) > 0)
                {

                    System.Data.DataTable dt;
                    dt = manager.getEmailUtentiInTrattativa((long)annuncioId);


                    if (dt.Rows.Count > 0)
                    {

                        Annunci.Libri.Models.Libro i;
                        i = manager.getLibro((long)annuncioId);

                        Annunci.Libri.LibriMailMessageManager mail = new Annunci.Libri.LibriMailMessageManager(System.Configuration.ConfigurationManager.AppSettings["application.name"], System.Configuration.ConfigurationManager.AppSettings["application.url"]);
                        mail.Subject = System.Configuration.ConfigurationManager.AppSettings["application.name"] + " - Modifica annuncio";
                        mail.Body = mail.getBodyModificaTestoAnnuncio((long)annuncioId, i.categoria.ToString() + " - " + i.annuncioId.ToString(), String.Format("Libri/Details/{0}", i.annuncioId));

                        foreach (System.Data.DataRow row in dt.Rows)
                        {
                            mail.Bcc(row["email"].ToString());
                        }
                        //MY-DEBUGG
                        mail.Bcc(System.Configuration.ConfigurationManager.AppSettings["mail.To.Ccn"]);

                        mail.send();
                    }
                }
            }
            finally
            {
                manager.mCloseConnection();
            }

            return RedirectToAction("MyAnnuncio", new { id = annuncioId });
        }



        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTrattativa(long trattativaId)
        {

            try
            {
                manager.mOpenConnection();

                manager.deleteTrattativa(trattativaId);
            }
            finally
            {
                manager.mCloseConnection();
            }

            return RedirectToAction("MyTrattative");
        }



        [Authorize]
        [HttpPost]
        public JsonResult AddImage(long? annuncioId, HttpPostedFileBase MyFile)
        {
            if (annuncioId == null)
            {
                throw new MyManagerCSharp.MyException(MyManagerCSharp.MyException.ErrorNumber.ParametroNull, "DeleteExecute");
            }

            Debug.WriteLine("AddImage: " + annuncioId + " " + Request["MyFile"]);

            Models.JsonMessageModel model = new Models.JsonMessageModel();

            if (MyFile == null)
            {
                model.esito = "Failed";
                model.messaggio = "Selezionare un file";
                return Json(model);
            }



            //// Verify that the user selected a file
            //if (MyFile != null && MyFile.ContentLength > 0)
            //{
            //    //Verifico l'estenzione
            //    string  temp ;
            //    temp = MyFile.FileName;

            //    if (! temp.EndsWith(".gif") &&  !temp.EndsWith(".jpg") && ! temp.EndsWith(".jpeg") && ! temp.EndsWith(".png")) {
            //        ModelState.AddModelError("", "Sono ammessi solo file con estenzione .gif, .jpg oppure .png");
            //        return RedirectToAction("MyAnnuncio", new { id = annuncioId });
            //    }
            //}


            string folder;
            folder = System.Configuration.ConfigurationManager.AppSettings["mercatino.images.folder"];
            if (!System.IO.Directory.Exists(Server.MapPath(folder)))
            {
                System.IO.Directory.CreateDirectory(Server.MapPath(folder));
            }
            folder = folder + annuncioId + "/";
            if (!System.IO.Directory.Exists(Server.MapPath(folder)))
            {
                System.IO.Directory.CreateDirectory(Server.MapPath(folder));
            }


            System.Drawing.Image myImage = null;

            try
            {

                myImage = System.Drawing.Image.FromStream(MyFile.InputStream);
            }
            catch (Exception e)
            {
                //NON si tratta di un'immagine!
                model.esito = "Failed";
                model.messaggio = "Il file selezionato NON contiene un'immagine";
                return Json(model);
            }


            if (myImage.Height > MaxHeightImage)
            {
                myImage = Annunci.PhotoManager.resizeImageHeight(myImage, MaxHeightImage);
            }

            if (myImage.Width > MaxWidthImage)
            {
                myImage = Annunci.PhotoManager.resizeImageWidth(myImage, MaxWidthImage);
            }

            //Salvo l'immagine originale su file system
            string pathFile;
            pathFile = folder + System.IO.Path.GetFileName(MyFile.FileName);
            myImage.Save(Server.MapPath(pathFile));

            //'creo la thumbnail con altezza massima di 120px ... mantenendo le proporzioni 
            System.Drawing.Image thumbnail;
            thumbnail = Annunci.PhotoManager.resizeImageHeight(myImage, HeightThumbnail);
            pathFile = folder + "thumbnail_" + System.IO.Path.GetFileName(MyFile.FileName);
            thumbnail.Save(Server.MapPath(pathFile));


            //'inserimento su DB!
            Annunci.PhotoManager m = new Annunci.PhotoManager(manager.mGetConnection());

            try
            {
                m.mOpenConnection();

                //If _isPlanimetria Then
                //    _manager.insertPhoto(folder & IO.Path.GetFileName(AsyncFileUpload1.FileName), "PLANIMETRIA", _externalId, CType(Session("SessionData"), MyManager.SessionData).getUserId)
                //Else
                //    _manager.insertPhoto(folder & IO.Path.GetFileName(AsyncFileUpload1.FileName), "", _externalId, CType(Session("SessionData"), MyManager.SessionData).getUserId)
                //End If

                m.insertPhoto(folder + System.IO.Path.GetFileName(MyFile.FileName), "", (long)annuncioId, MySessionData.UserId);



            }
            finally
            {
                m.mCloseConnection();
            }





            model.esito = "Success";
            model.messaggio = "Operazione conlusa con successo";
            return Json(model);
        }


        [HttpPost]
        public JsonResult DeleteImage(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            Debug.WriteLine(String.Format("ExternalId: {0}", id));

            Annunci.PhotoManager m = new Annunci.PhotoManager(manager.mGetConnection());

            try
            {
                m.mOpenConnection();
                m.deletePhoto((long)id, Server.MapPath("~"));
            }
            finally
            {
                m.mCloseConnection();
            }


            MyWebApplication.Models.JsonMessageModel model = new Models.JsonMessageModel();
            model.esito = "Success";
            model.messaggio = "Operazione conlusa con successo";
            return Json(model);
        }





        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ResetContatore(long? annuncioId)
        {
            if (annuncioId == null)
            {
                throw new MyManagerCSharp.MyException(MyManagerCSharp.MyException.ErrorNumber.ParametroNull, "ResetContatore");
            }

            try
            {
                manager.mOpenConnection();
                manager.resetContatoreParziale((long)annuncioId);
            }
            finally
            {
                manager.mCloseConnection();
            }

            Models.JsonMessageModel model = new Models.JsonMessageModel();
            model.esito = "Success";
            model.messaggio = "Operazione conlusa con successo";
            return Json(model);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult UpdatePrezzo(long? annuncioId, string nuovoPrezzo)
        {
            if (annuncioId == null)
            {
                throw new MyManagerCSharp.MyException(MyManagerCSharp.MyException.ErrorNumber.ParametroNull, "annuncioId");
            }

            //if (nuovoPrezzo == null)
            if (String.IsNullOrEmpty(nuovoPrezzo))
            {
                throw new MyManagerCSharp.MyException(MyManagerCSharp.MyException.ErrorNumber.ParametroNull, "prezzo");
            }

            decimal dNuovoPrezzo;
            dNuovoPrezzo = decimal.Parse(nuovoPrezzo.Replace(".", ","));

            Debug.WriteLine("UpdatePrezzo (String): " + nuovoPrezzo);
            Debug.WriteLine("UpdatePrezzo: " + dNuovoPrezzo);


            try
            {

                manager.mOpenConnection();

                if (manager.updateAnnuncioPrezzo((long)annuncioId, (decimal)dNuovoPrezzo, false) > 0)
                {

                    System.Data.DataTable dt;
                    dt = manager.getEmailUtentiInTrattativa((long)annuncioId);


                    if (dt.Rows.Count > 0)
                    {

                        Annunci.Libri.Models.Libro i;
                        i = manager.getLibro((long)annuncioId);
                        Annunci.Libri.LibriMailMessageManager mail = new Annunci.Libri.LibriMailMessageManager(System.Configuration.ConfigurationManager.AppSettings["application.name"], System.Configuration.ConfigurationManager.AppSettings["application.url"]);
                        mail.Subject = System.Configuration.ConfigurationManager.AppSettings["application.name"] + " - Modifica annuncio";
                        mail.Body = mail.getBodyAggiornamentoPrezzoAnnuncio((long)annuncioId, i.categoria.ToString() + " - " + i.titolo, i.prezzo, (decimal)dNuovoPrezzo, String.Format("Libri/Details/{0}", i.annuncioId));

                        foreach (System.Data.DataRow row in dt.Rows)
                        {
                            mail.Bcc(row["email"].ToString());
                        }
                        //'MY-DEBUGG
                        mail.Bcc(System.Configuration.ConfigurationManager.AppSettings["mail.To.Ccn"]);

                        mail.send();
                    }
                }
            }
            finally
            {
                manager.mCloseConnection();
            }

            return RedirectToAction("MyAnnuncio", new { id = annuncioId });
        }




        public ActionResult Messaggi()
        {

            List<Annunci.Models.Trattativa> risultato;

            try
            {
                risultato = manager.getListMessaggi(MySessionData.UserId);
            }
            finally
            {
                manager.mCloseConnection();
            }


            return View(risultato);


        }


        [AllowAnonymous]
        public ActionResult TestiScolasticiElementari()
        {

            Annunci.Libri.Models.SearchLibri model = new Annunci.Libri.Models.SearchLibri();
            model.filter.categoriaId = 1130000;
            model.filter.subCategoriaId = 1130100;
            //return RedirectToAction("Index", model);

            return Index(model);
        }

    }
}