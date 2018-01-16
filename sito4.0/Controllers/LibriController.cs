using System;
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
        protected MyManagerCSharp.MySessionData MySessionData;

        public const int MaxWidthImage = 500;
        public const int MaxHeightImage = 500;

        public const int WidthThumbnail = 200;
        public const int HeightThumbnail = 200;


        private Annunci.Libri.LibriManager manager = new Annunci.Libri.LibriManager("mercatino");

        private MyManagerCSharp.RegioniProvinceComuniManager regioniProvinceComuniManager = new MyManagerCSharp.RegioniProvinceComuniManager("RegioniProvinceComuni");

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (Session["MySessionData"] != null)
            {
                MySessionData = (Session["MySessionData"] as MyManagerCSharp.MySessionData);
            }
        }

        [AllowAnonymous]
        public ActionResult Index(Annunci.Libri.Models.SearchLibri model)
        {

            manager.mOpenConnection();
            try
            {
                manager.getList(model);
            }
            finally
            {
                manager.mCloseConnection();
            }

            return View(model);
        }

        public ActionResult MyAnnunci()
        {
            manager.mOpenConnection();

            List<Annunci.Libri.Models.Libro> risultato;
            System.Collections.Hashtable hashtableRisposte = new System.Collections.Hashtable();

            try
            {

                //if (User.Identity is System.Web.Security.FormsIdentity)
                //{
                //    risultato = manager.getListAnnunci((User.Identity as System.Web.Security.FormsIdentity)..UserId);
                //}

                //if (User.Identity is MyUsers.MyCustomIdentity)
                //{
                //    risultato = manager.getListAnnunci((User.Identity as MyUsers.MyCustomIdentity).UserId);
                //}


                risultato = manager.getListAnnunci(MySessionData.UserId);

                Annunci.AnnuncioManager annuncioManager = new Annunci.AnnuncioManager(manager.mGetConnection());
                long numeroRisposte;
                foreach ( Annunci.Libri.Models.Libro i in risultato)
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
            return View(risultato);
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
            Annunci.Libri.Models.Libro libro;

            manager.mOpenConnection();
            try
            {
                libro = manager.getLibro(id);
            }
            finally
            {
                manager.mCloseConnection();
            }

            return View(libro);
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




            return RedirectToAction("Details", new { id = annuncioId });
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
            if (annuncioId != null)
            {
                Debug.WriteLine("Delete MyAnnuncio: " + annuncioId);
                ViewData["Tipo"] = "Annuncio";
                ViewData["MyId"] = annuncioId;


                ViewBag.Title = "Trova-libro: Cancellazione annuncio";
            }
            return View();
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

            Models.UpdateModel model;
            model = new Models.UpdateModel();

            Annunci.Libri.Models.Libro libro = null;
            manager.mOpenConnection();
            try
            {
                libro = manager.getLibro(id);

                if (libro == null)
                {
                    return HttpNotFound();
                }

                if (libro.userId != MySessionData.UserId)
                {
                    return RedirectToAction("NotAuthorized", "Home");
                }

                // PHOTO
                Annunci.PhotoManager photoManager = new Annunci.PhotoManager(manager.mGetConnection());

                List<Annunci.Models.MyPhoto> photos;
                photos = photoManager.getMyPhotosIsNotPlanimetria(id);

                Debug.WriteLine("Trovate {0} immagini", photos.Count);

                if (Request.IsAjaxRequest())
                {
                    Models.GalleryModel modelGallery = new Models.GalleryModel();
                    modelGallery.externalId = libro.annuncioId;
                    modelGallery.photos = photos;
                    return PartialView("GalleryEdit", modelGallery);
                }

                model.photos = photos;
            }
            finally
            {
                manager.mCloseConnection();
            }

            model.libro = libro;
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


    }
}