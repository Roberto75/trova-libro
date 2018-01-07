using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebApplication.Controllers
{
    [Authorize]
    public class LibriController : Controller
    {
        protected MyManagerCSharp.MySessionData MySessionData;

        private trova_libro.manager.LibriManager manager = new trova_libro.manager.LibriManager("mercatino");

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
        public ActionResult Index(trova_libro.manager.Models.SearchLibri model)
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
            return View();
        }

        public ActionResult MyTrattative()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Details(long id)
        {
            trova_libro.manager.Models.Libro libro;

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

                trova_libro.manager.Models.Libro libro;
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

            model.libro = new trova_libro.manager.Models.Libro();

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
    }
}