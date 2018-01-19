﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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


        private Annunci.Libri.LibriManager manager = new Annunci.Libri.LibriManager("mercatino");

        private MyManagerCSharp.RegioniProvinceComuniManager regioniProvinceComuniManager = new MyManagerCSharp.RegioniProvinceComuniManager("RegioniProvinceComuni");


        [AllowAnonymous]
        public ActionResult Index(Annunci.Libri.Models.SearchLibri model)
        {


            manager.mOpenConnection();
            try
            {
                model.comboCategorie = manager.getComboCategoria(1000000);

                if (model.filter.categoriaId != null &&
                    ((model.filter.categoriaId >= 1130000 && model.filter.categoriaId < 1140000)  || (model.filter.categoriaId >= 1140000 && model.filter.categoriaId < 1150000)) )
                {
                    model.comboSubCategorie = manager.getComboCategoria((int)model.filter.categoriaId);
                }
                model.comboRegioni = regioniProvinceComuniManager.getComboRegioni();

                manager.getList(model);
            }
            finally
            {
                manager.mCloseConnection();
            }

            return View(model);
        }

        public ActionResult MyAnnunci(Annunci.Libri.Models.SearchLibri model)
        {
            System.Collections.Hashtable hashtableRisposte = new System.Collections.Hashtable();
            try
            {
                manager.mOpenConnection();

                manager.getMyListAnnunci(MySessionData.UserId, model);

                Annunci.AnnuncioManager annuncioManager = new Annunci.AnnuncioManager(manager.mGetConnection());
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

            Annunci.Models.Trattativa risultato;

            try
            {
                risultato = manager.getTrattativa((long)id);

                if (risultato != null)
                {
                    manager.setRisposteFromTrattativa(risultato);
                }

                model.trattativa = risultato;

                model.libro = manager.getLibro(risultato.annuncioId);

                if (model.libro == null)
                {
                    //vuol dire che l'annuncio è stato rimosso ... 

                }

            }
            finally
            {
                manager.mCloseConnection();
            }


            return View(model);
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
                    return HttpNotFound();
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
                }
                else
                {
                    model.testo = "";
                    if (rispostaId == null)
                    {
                        model.rispostaId = -1;
                    }
                    else
                    {
                        model.rispostaId = (long)rispostaId;
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
            //long userId = (User.Identity as MyUsers.MyCustomIdentity).UserId;
            long userId = userId = (Session["MySessionData"] as MyManagerCSharp.MySessionData).UserId;


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

                    Annunci.TrattativaManager managerVb = new Annunci.TrattativaManager(manager.mGetConnection());


                    if (rispostaId == null || rispostaId == -1)
                    {
                        //' si tratta di una nuova trattativa!!
                        trattativaId = manager.insertTrattativa(annuncioId, userId);
                        managerVb.rispondi((long)trattativaId, userId, testo);
                    }
                    else
                    {
                        managerVb.rispondi((long)trattativaId, userId, testo, (long)rispostaId);
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
        public ActionResult GetSubCategoria(long categoriaId)
        {
            Debug.WriteLine("GetSubCategoria: " + categoriaId);

            List<MyManagerCSharp.Models.MyItem> items = manager.getComboCategoria(categoriaId);

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
            if (model.libro.categoriaId == null)
            {
                ModelState.AddModelError("", "La categoria è un campo obbligatorio");
            }

            if (model.libro.titolo == null)
            {
                ModelState.AddModelError("", "Il titolo è un campo obbligatorio");
            }

            if (model.libro.tipo == null)
            {
                ModelState.AddModelError("", "Il tipo di annuncio è un campo obbligatorio");
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

            try
            {
                manager.insertAnnuncio(model.libro, MySessionData.UserId);
            }
            finally
            {
                manager.mCloseConnection();
            }



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

                manager.deleteAnnuncioLogic((long)MyId, Annunci.AnnuncioManager.StatoAnnuncio.Da_cancellare, Server.MapPath("~"));

            }
            finally
            {
                manager.mCloseConnection();
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
                    return HttpNotFound();
                }

                if (model.libro.userId != MySessionData.UserId)
                {
                    return RedirectToAction("NotAuthorized", "Home");
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
                        mail.Body = mail.getBodyModificaTestoAnnuncio((long)annuncioId, i.categoria.ToString() + " - " + i.annuncioId.ToString());

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
                        mail.Body = mail.getBodyAggiornamentoPrezzoAnnuncio((long)annuncioId, i.categoria.ToString() + " - " + i.titolo, i.prezzo, (decimal)dNuovoPrezzo);

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

    }
}