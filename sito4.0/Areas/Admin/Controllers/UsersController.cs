using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyManagerCSharp;
using MyUsers.Models;
using MyUsers;
using System.Diagnostics;

namespace MyWebApplication.Areas.Admin.Controllers
{
    [MyAuthorize(Roles = "Administrators")]
    public class UsersController : MyBaseController
    {

        private UserManager manager = new UserManager("utenti");

        public ActionResult Index(MyUsers.Models.SearchUsers model)
        {
           
            TryUpdateModel(model.filter, "filter");

            Debug.WriteLine(String.Format("Filtri di ricerca  Nome: {0} Email: {1}", model.filter.nome, model.filter.email));


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

        public ActionResult Details(long id = 0)
        {
            Models.MyUserModel model = new Models.MyUserModel();

            Annunci.AnnunciAdminManager annunciAdminManager = new Annunci.AnnunciAdminManager("mercatino");


            try
            {
                manager.mOpenConnection();
                annunciAdminManager.mOpenConnection();

                model.Utente = manager.getUser(id);

                if (model.Utente == null)
                {
                    return HttpNotFound();
                }
                manager.setProfili(model.Utente);
                manager.setGroups(model.Utente);
                manager.setRoles(model.Utente);

                CustomerManager c = new CustomerManager(manager.mGetConnection());
                c.set(model.Utente);


                ////REPORTS
                //MyManagerCSharp.RGraph.Models.RGraphModel report;
                //MyUsers.Reports.ReportsUsers reportManager = new MyUsers.Reports.ReportsUsers(manager.mGetConnection());

                //try
                //{
                //    reportManager.mOpenConnection();
                //    report = reportManager.getLoginByDate("LoginByUser", ManagerDB.Days.Oggi, id);

                //    model.Reports.Add(report);
                //}
                //finally
                //{
                //    reportManager.mCloseConnection();
                //}



                //reportManager.EnableOnClick = false;
                //report = reportManager.getLoginSuccessAndFailure("report01", 10, ManagerDB.Days.Tutti);
                //report.Width = "400px";
                //report.Height = "400px";
                //model.Reports.Add(report);


                //ANNUNCI
                //cerco tutti gli annunci di un utente
                //Annunci.Libri.LibriManager libriManager = new Annunci.Libri.LibriManager(manager.mGetConnection());
                //Annunci.Libri.Models.SearchLibri search = new Annunci.Libri.Models.SearchLibri();
                //search.filter.login = model.Utente.login;

                //libriManager.getList(search);

                //Verifico che l'utente sia registrato anche nel db degli ANNUNCI
                if (!annunciAdminManager.checkUserExists((long)model.Utente.userId))
                {

                    throw new MyManagerCSharp.MyException("L'utente " + model.Utente.userId + " non è presente nel db degli annunci");

                    //Rutigliano 24/01/2018
                    //Decommento questo codice solo se i serve fare l'inserimento automatico
                    //al momento voglio vedere l'errore ....

                    //if (model.Utente.customerId != null)
                    //{
                      //  annunciAdminManager.insertUser((long)model.Utente.userId, model.Utente.nome, model.Utente.cognome, model.Utente.email, model.Utente.login, (long)model.Utente.customerId);
                    //}
                    //else
                    //{
                      //  annunciAdminManager.insertUser((long)model.Utente.userId, model.Utente.nome, model.Utente.cognome, model.Utente.email, model.Utente.login, -1);
                    //}

                }

                model.Annunci = annunciAdminManager.getListAnnunciByUserId((long)model.Utente.userId);

                model.ContaAnnunciByStato = annunciAdminManager.countAnnunciByStato((long)model.Utente.userId);
                model.ContaTrattativeByStato = annunciAdminManager.countTrattativeByStato((long)model.Utente.userId);

            }
            finally
            {
                manager.mCloseConnection();
                annunciAdminManager.mCloseConnection();
            }
            return View(model);
        }

        public ActionResult Create()
        {
            List<string> t = new List<string>  {
                        "Brendan Enrick",
                        "Kevin Kuebler",
                        "Todd Ropog"
                    };

            ViewBag.lista = new SelectList(t);

            //populateComboBox(null);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MyUsers.Models.MyUser myuser)
        {
            string t = Request.Form["Gruppi"];
            Debug.WriteLine("Gruppi: " + t);

            string gruppiSelected = Request.Form["gruppiIDs"];
            Debug.WriteLine("gruppiIDs: " + gruppiSelected);


            //http://www.hanselman.com/blog/ASPNETWireFormatForModelBindingToArraysListsCollectionsDictionaries.aspx

            MyHelper.printRequest(Request);


            //if (myuser.Gruppi == null)
            //{
            //    ModelState.AddModelError(string.Empty, "Occorre selezionare almeno un Gruppo");
            //}

            if (String.IsNullOrEmpty(gruppiSelected))
            {
                ModelState.AddModelError(string.Empty, "Occorre selezionare almeno un Gruppo");
            }



            if (ModelState.IsValid)
            {
                long newId;
                manager.mOpenConnection();
                try
                {
                    newId = manager.insert(myuser);
                    if (newId != -1)
                    {
                        List<MyGroup> lista = new List<MyGroup>();

                        foreach (string id in gruppiSelected.Split(','))
                        {
                            lista.Add(new MyGroup(int.Parse(id)));
                        }

                        myuser.Gruppi = lista;


                        GroupManager groupManager = new GroupManager(manager.mGetConnection());
                        groupManager.addUser(myuser.Gruppi, newId);
                    }
                }
                finally
                {
                    manager.mCloseConnection();
                }

                return RedirectToAction("Index");
            }



            var error = ModelState.SelectMany(x => x.Value.Errors);


            foreach (var value in ModelState.Values)
            {

                foreach (var merror in value.Errors)
                {

                    //throw new Exception(merror.ErrorMessage, merror.Exception);
                    Debug.WriteLine(merror.ErrorMessage + merror.Exception);

                }

            }
            return View(myuser);
        }

        public ActionResult Edit(long id)
        {
            Models.MyUserModel model = new Models.MyUserModel();

            //CustomerManager m1 = new CustomerManager(manager.mGetConnection());

            //List<MyGroup> listaGruppi = null;
            //List<MyCustomer> listaClienti = null;
            //List<MyProfile> listaProfili = null;

            manager.mOpenConnection();
            try
            {
                model.Utente = manager.getUser(id);

                if (model.Utente == null)
                {
                    return HttpNotFound();
                }


                manager.setProfili(model.Utente);
                manager.setGroups(model.Utente);


                //listaGruppi = m.getList();
                //listaClienti = m1.getList();
                //listaProfili = manager.getProfileList();
                GroupManager groupManager = new GroupManager(manager.mGetConnection());
                model.Gruppi = groupManager.getList();

                //if (model.Utente.Gruppi != null)
                //{
                //    model.Gruppi = new MultiSelectList(groupManager.getList(), "gruppoId", "nome", model.Utente.Gruppi.Select(x => x.gruppoId).ToArray());
                //}
                //else
                //{
                //    model.Gruppi = new MultiSelectList(groupManager.getList(), "gruppoId", "nome", null);
                //}



                if (model.Utente.Profili != null && model.Utente.Profili.Count != 0)
                {
                    model.Profilo = new SelectList(manager.getProfili(), "profiloId", "nome", model.Utente.Profili[0].profiloId);
                }
                else
                {
                    model.Profilo = new SelectList(manager.getProfili(), "profiloId", "nome", null);
                }
            }
            finally
            {
                manager.mCloseConnection();
            }




            ////MultiSelectList sl = new MultiSelectList(listaGruppi.ToList(), "gruppoId", "nome", new int[] {2}  );


            //MultiSelectList sl = new MultiSelectList(listaGruppi.ToList(), "gruppoId", "nome", listaGruppi.ToList());
            //ViewBag.listaGruppi = sl;

            //ViewBag.listaGruppi = listaGruppi;

            //ViewBag.clienti = new SelectList(listaClienti.ToList(), "customerId", "ragioneSociale");

            //ViewBag.profili = new SelectList(listaProfili.ToList(), "profileId", "nome");


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.MyUserModel model)
        {
            //MyHelper.printRequest(Request);

            //if (myuser.Gruppi == null)
            //{
            //    ModelState.AddModelError(string.Empty, "Occorre selezionare almeno un Gruppo");
            //}

            //string gruppiSelected = Request.Form["gruppiIDs"];
            //Debug.WriteLine("gruppiIDs: " + gruppiSelected);

            //if (String.IsNullOrEmpty(gruppiSelected))
            //{
            //    ModelState.AddModelError(string.Empty, "Occorre selezionare almeno un Gruppo");
            //}



            if (ModelState.IsValid)
            {

                Debug.WriteLine("Nome: " + model.Utente.nome);

                bool esito;
                manager.mOpenConnection();
                try
                {
                    //TODO: Rutigliano da modificare 02/02/2014
                    model.Utente.isEnabled = true;
                    //esito = manager.update(model.Utente);
                    esito = manager.updateEmail((long)model.Utente.userId, model.Utente.email);
                    //in questo caso gestiamo un solo profilo!
                    List<MyUsers.Models.MyProfile> lista = new List<MyProfile>();


                    Debug.WriteLine("ProfiloId: " + Request.Form["ProfiloId"]);


                    if (!String.IsNullOrEmpty(Request.Form["ProfiloId"]))
                    {
                        MyUsers.Models.MyProfile profilo = new MyProfile(Request.Form["ProfiloId"]);
                        lista.Add(profilo);
                    }

                    esito = manager.updateProfili(lista, (long)model.Utente.userId);
                    if (esito)
                    {

                        //List<MyGroup> lista = new List<MyGroup>();

                        //foreach (string id in gruppiSelected.Split(','))
                        //{
                        //    lista.Add(new MyGroup(int.Parse(id)));
                        //}

                        //model.Utente.Gruppi = lista;

                        //GroupManager groupManager = new GroupManager(manager.mGetConnection());
                        //groupManager.update(model.Utente.Gruppi, model.Utente.userId);
                    }
                }
                finally
                {
                    manager.mCloseConnection();
                }
                return RedirectToAction("Details", "Users", new { id = model.Utente.userId });
            }





            var error = ModelState.SelectMany(x => x.Value.Errors);


            foreach (var value in ModelState.Values)
            {

                foreach (var merror in value.Errors)
                {

                    //throw new Exception(merror.ErrorMessage, merror.Exception);
                    Debug.WriteLine(merror.ErrorMessage + merror.Exception);

                }

            }

            //manager.mOpenConnection();

            //populateComboBox(myuser);

            return View(model);
        }

        public ActionResult Delete(long id = 0)
        {
            MyUser myuser = null;

            manager.mOpenConnection();
            try
            {
                myuser = manager.getUser(id);
            }
            finally
            {
                manager.mCloseConnection();
            }


            if (myuser == null)
            {
                return HttpNotFound();
            }


            return View(myuser);
        }

        /*  [HttpPost]
          [ValidateAntiForgeryToken]
          public ActionResult DeleteUserLogic(long? id)
          {

              if (id == null)
              {
                  return HttpNotFound();
              }

              bool esito;
              manager.mOpenConnection();
              try
              {
                  esito = manager.delete((long)id);
              }
              finally
              {
                  manager.mCloseConnection();
              }

              return RedirectToAction("Index");
          }
          */


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUser(long? id)
        {
            //CANCELLAZIONE FISICA
            if (id == null)
            {
                return HttpNotFound();
            }

            bool esito;

            Annunci.AnnunciManager annunciManager = new Annunci.AnnunciManager("mercatino");
            try
            {
                annunciManager.mOpenConnection();
                esito = annunciManager.deleteUser((long)id, Server.MapPath("~"));
            }
            finally
            {
                annunciManager.mCloseConnection();
            }

            //Rutigliano 07/03/2014 potrei avere un utente che sta registato in Utenti e non su Immobiliare!
            manager.mOpenConnection();
            try
            {
                esito = manager.delete((long)id);
            }
            finally
            {
                manager.mCloseConnection();
            }

            return RedirectToAction("Index");
        }



        private void populateComboBoxOLD(MyUser myuser)
        {
            GroupManager m = new GroupManager(manager.mGetConnection());
            CustomerManager customerManager = new CustomerManager(manager.mGetConnection());

            m.mOpenConnection();

            List<MyGroup> listaGruppi;
            List<MyCustomer> listaClienti;
            List<MyProfile> listaProfili;

            try
            {
                listaGruppi = m.getList();
                listaClienti = customerManager.getList();
                listaProfili = manager.getProfili();
            }
            finally
            {
                m.mCloseConnection();
            }


            //MultiSelectList sl = new MultiSelectList(listaGruppi.ToList(), "gruppoId", "nome");
            //ViewBag.listaGruppi = sl;
            //ViewBag.Gruppi = listaGruppi.ToList();
            //ViewBag.listaClienti = new SelectList(listaClienti.ToList(), "customerId", "ragioneSociale");

            if (myuser == null)
            {
                ViewBag.listaClienti = listaClienti.OrderBy(p => p.ragioneSociale).Select(p => new SelectListItem { Selected = false, Text = p.ragioneSociale, Value = p.customerId.ToString() });
                //ViewBag.customerId = new SelectList(listaClienti, "customerId", "ragioneSociale");
                ViewBag.Gruppi = new MultiSelectList(listaGruppi, "gruppoId", "nome");
            }
            else
            {
                ViewBag.listaClienti = listaClienti.OrderBy(p => p.ragioneSociale).Select(p => new SelectListItem { Selected = (p.customerId == myuser.customerId), Text = p.ragioneSociale, Value = p.customerId.ToString() });
                //ViewBag.customerId = new SelectList(db.Clienti, "customerId", "ragioneSociale", myuser.customerId);

                MultiSelectList msl;
                if (myuser.Gruppi != null)
                {
                    msl = new MultiSelectList(listaGruppi, "gruppoId", "nome", myuser.Gruppi.Select(x => x.gruppoId).ToArray());
                }
                else
                {
                    msl = new MultiSelectList(listaGruppi, "gruppoId", "nome", null);
                }

                ViewBag.Gruppi = msl;

                // ViewBag.Gruppi = new MultiSelectList(listaGruppi, "gruppoId", "nome", myuser.Gruppi);
            }
            ViewBag.listaProfili = new SelectList(listaProfili.ToList(), "profileId", "nome");

        }


        public ActionResult CSV()
        {

            byte[] content;

            manager.mOpenConnection();
            try
            {
                content = manager.exportToCSV();
            }
            finally
            {
                manager.mCloseConnection();
            }


            return File(content, "text/csv", String.Format("Export_users_{0}.csv", DateTime.Now.ToString("yyyy-MM-dd")));
        }


        public ActionResult SID(string sid)
        {

            System.Security.Principal.SecurityIdentifier sidObj;
            sidObj = new System.Security.Principal.SecurityIdentifier(sid);

            MyUser user = null; ;
            try
            {
                ActiveDirectoryManager manager = new ActiveDirectoryManager();
                user = manager.getUser(sidObj);
            }
            catch (Exception ex)
            {
                //http://rachelappel.com/when-to-use-viewbag-viewdata-or-tempdata-in-asp.net-mvc-3-applications
                TempData["MyError"] = ex.Message;
                return RedirectToAction("Error", "Admin");
            }


            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddGroup(long? userId)
        {
            if (userId == null)
            {
                throw new ArgumentNullException("userId");
            }

            Debug.WriteLine("UserId: " + userId);
            Debug.WriteLine("gruppiIDs: " + Request["gruppiIDs"]);

            manager.mOpenConnection();
            try
            {

                List<MyGroup> lista = new List<MyGroup>();

                foreach (string id in Request["gruppiIDs"].Split(','))
                {
                    Debug.WriteLine("GruppoId: " + id);
                    lista.Add(new MyGroup(int.Parse(id)));
                }

                GroupManager groupManager = new GroupManager(manager.mGetConnection());
                groupManager.update(lista, (long)userId);

                //List<MyGroup> lista = new List<MyGroup>();

                //foreach (string id in gruppiSelected.Split(','))
                //{
                //    lista.Add(new MyGroup(int.Parse(id)));
                //}

                //model.Utente.Gruppi = lista;

                //GroupManager groupManager = new GroupManager(manager.mGetConnection());
                //groupManager.update(model.Utente.Gruppi, model.Utente.userId);

            }
            finally
            {
                manager.mCloseConnection();
            }


            // return RedirectToAction("Details", new { id = userId} , ) ;

            return new RedirectResult(Url.Action("Details", new { id = userId }) + "#tabs-3");
        }


        public ActionResult AutoCompleteLogin(string value)
        {
            Debug.WriteLine("AutoCompleteLogin: " + value);

            List<MyManagerCSharp.Models.MyItem> risultato;

            manager.mOpenConnection();

            try
            {
                risultato = manager.getAutoCompleteLogin(value);
            }
            finally
            {
                manager.mCloseConnection();
            }

            return Json(risultato, JsonRequestBehavior.AllowGet);
        }






    }
}