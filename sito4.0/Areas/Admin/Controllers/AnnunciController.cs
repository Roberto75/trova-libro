using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebApplication.Areas.Admin.Controllers
{
    [Authorize]
    public class AnnunciController : MyBaseController
    {
        private Annunci.AnnunciAdminManager manager = new Annunci.AnnunciAdminManager("mercatino");

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Details(MyWebApplication.Areas.Admin.Models.AnnuncioDetailsModel model)
        {

         //   model.annuncioId = 727456154;
            return View(model);
        }



        [HttpPost]
        [ActionName("Details")]
        [ValidateAntiForgeryToken]
        public ActionResult DetailsPost(MyWebApplication.Areas.Admin.Models.AnnuncioDetailsModel model)
        {
            System.Collections.Hashtable hashtableRisposte = new System.Collections.Hashtable();

            manager.mOpenConnection();
            try
            {
                model.annuncio = manager.getAnnuncio(model.annuncioId);

                if (model.annuncio == null)
                {
                   // return HttpNotFound();
                }


                Annunci.AnnunciAdminManager adminManager = new Annunci.AnnunciAdminManager(manager.mGetConnection());

                model.trattative = adminManager.getTrattativeByAnnuncio(model.annuncioId, Annunci.Models.Trattativa.TipoTrattativa.Libro);
                long numeroRisposte;
                foreach (Annunci.Models.Trattativa t in model.trattative)
                {
                    numeroRisposte = manager.getNumeroRisposteOfTrattativa(t.trattativaId, model.annuncio.userId);
                    hashtableRisposte.Add(t.trattativaId, numeroRisposte);
                }

                // PHOTO
                Annunci.PhotoManager photoManager = new Annunci.PhotoManager(manager.mGetConnection());

                //List<Annunci.Models.MyPhoto> photos;
                //photos = photoManager.getMyPhotosIsNotPlanimetria(id);

                // Debug.WriteLine("Trovate {0} immagini", photos.Count);



                //  model.photos = photos;
            }
            finally
            {
                manager.mCloseConnection();
            }

            ViewData["hashtableRisposte"] = hashtableRisposte;
            return View(model);
        }


        

        public ActionResult Trattativa(long? id)
        {
            TempData["AREA"] = "Admin";

            MyWebApplication.Controllers.LibriController  controller = new MyWebApplication.Controllers.LibriController();
            controller.ControllerContext = ControllerContext;
            controller.TempData = TempData;
            return controller.Trattativa(id);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAnnuncio(long? id)
        {
            //CANCELLAZIONE FISICA
            if (id == null)
            {
                return HttpNotFound();
            }

            bool esito;
            manager.mOpenConnection();
            try
            {
                esito = manager.deleteAnnuncio((long)id, Server.MapPath("~"));
            }
            finally
            {
                manager.mCloseConnection();
            }

            return RedirectToAction("Details");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAnnuncioLogic(long? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            bool esito;
            manager.mOpenConnection();
            try
            {
                esito = manager.deleteAnnuncioLogic((long)id, Annunci.AnnunciManager.StatoAnnuncio.Da_cancellare, Server.MapPath("~"));
            }
            finally
            {
                manager.mCloseConnection();
            }

            return RedirectToAction("Details");
        }


    }
}