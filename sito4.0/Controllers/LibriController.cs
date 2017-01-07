using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebApplication.Controllers
{
    public class LibriController : Controller
    {

        private trova_libro.manager.LibriManager manager = new trova_libro.manager.LibriManager("mercatino");

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



        [Authorize]
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


    }
}