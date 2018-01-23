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
        private Annunci.Libri.LibriManager manager = new Annunci.Libri.LibriManager("mercatino");

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Details(MyWebApplication.Areas.Admin.Models.AnnuncioDetailsModel model)
        {

            model.annuncioId = 727456154;
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
                model.libro = manager.getLibro(model.annuncioId);

                if (model.libro == null)
                {
                    return HttpNotFound();
                }


                Annunci.AnnunciAdminManager adminManager = new Annunci.AnnunciAdminManager(manager.mGetConnection());

                model.trattative = adminManager.getTrattativeByAnnuncio(model.annuncioId, Annunci.Models.Trattativa.TipoTrattativa.Libro);
                long numeroRisposte;
                foreach (Annunci.Models.Trattativa t in model.trattative)
                {
                    numeroRisposte = manager.getNumeroRisposteOfTrattativa(t.trattativaId, model.libro.userId);
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
    }
}