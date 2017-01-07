﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using MyUsers;


namespace MyWebApplication.Areas.Admin.Controllers
{

    [MyAuthorize(Roles = "Administrators")]
    public class GroupsController : MyBaseController
    {
        private GroupManager manager = new GroupManager("utenti");

        public ActionResult Index(MyUsers.Models.SearchGroups model)
        {
            

            manager.mOpenConnection();
            try
            {
                 manager.getList(model);

                foreach (MyUsers.Models.MyGroup g in model.Gruppi )
                {
                    g.countUsers = manager.countUsers(g.gruppoId);
                    g.countRoles = manager.countRoles(g.gruppoId);
                }


                model.ListaTipi = manager.getTypeList(); 

            }
            finally
            {
                manager.mCloseConnection();
            }

          

            return View(model);
        }


        public ActionResult Create()
        {
            Models.MyGroupModel model = new Models.MyGroupModel();
            model.Gruppo = new MyUsers.Models.MyGroup();

            populateComboBox(model.Gruppo);

            return View(model);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.MyGroupModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Gruppo == null)
                {
                    //group = new MyUsers.Models.MyGroup();
                    //TryUpdateModel(group);
                }

                manager.mOpenConnection();
                try
                {
                    manager.insert(model.Gruppo);
                }
                finally
                {
                    manager.mCloseConnection();
                }

                return RedirectToAction("Index");
            }

            populateComboBox(model.Gruppo);
            return View(model);
        }

        public ActionResult Delete(int id = 0)
        {
            Models.MyGroupModel model = new Models.MyGroupModel();
            manager.mOpenConnection();
            try
            {
                model.Gruppo = manager.getGroup(id);
                if (model.Gruppo == null)
                {
                    return HttpNotFound();
                }


                if (model.Gruppo.nome.ToUpper() == "GUEST" )
                {
                    ModelState.AddModelError(string.Empty , "Non è possibile cancellare il gruppo di sistema 'Guest'");
                }

                if (model.Gruppo.nome.ToUpper() == "ADMINISTRATORS")
                {
                    ModelState.AddModelError(string.Empty, "Non è possibile cancellare il gruppo di sistema 'Administrators'");
                }
            }
            finally
            {
                manager.mCloseConnection();
            }
            return View(model);
        }



        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            manager.mOpenConnection();
            try
            {
                manager.delete((long)id);
            }
            finally
            {
                manager.mCloseConnection();
            }

            return RedirectToAction("Index");
        }




        public ActionResult Edit(int id = 0)
        {
            Models.MyGroupModel model = new Models.MyGroupModel();
            manager.mOpenConnection();
            try
            {
                model.Gruppo = manager.getGroup(id);
                if (model.Gruppo == null)
                {
                    return HttpNotFound();
                }

                manager.setRoles(model.Gruppo);
                model.Utenti = manager.getUsers(model.Gruppo.gruppoId);

                model.ListaTipi = manager.getTypeList(); 

                GroupManager groupManager = new GroupManager(manager.mGetConnection());
                if (model.Gruppo.Ruoli != null)
                {
                    model.Ruoli = new MultiSelectList(groupManager.getRoleList(), "ruoloId", "nome", model.Gruppo.Ruoli.Select(x => x.ruoloId).ToArray());
                }
                else
                {
                    model.Ruoli = new MultiSelectList(groupManager.getRoleList(), "ruoloId", "nome", null);
                }

                //populateComboBox(group);
            }
            finally
            {
                manager.mCloseConnection();
            }



            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.MyGroupModel model)
        {
            if (ModelState.IsValid)
            {
                //TryUpdateModel(group);  

                manager.mOpenConnection();
                try
                {
                    manager.update(model.Gruppo);
                }
                finally
                {
                    manager.mCloseConnection();
                }

                return RedirectToAction("Index");
            }

            model.Utenti = manager.getUsers(model.Gruppo.gruppoId);

            GroupManager groupManager = new GroupManager(manager.mGetConnection());

            if (model.Gruppo.Ruoli != null)
            {
                model.Ruoli = new MultiSelectList(groupManager.getRoleList(), "ruoloId", "nome", model.Gruppo.Ruoli.Select(x => x.ruoloId).ToArray());
            }
            else
            {
                model.Ruoli = new MultiSelectList(groupManager.getRoleList(), "ruoloId", "nome", null);
            }
            model.ListaTipi = manager.getTypeList(); 
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Roles(Models.MyGroupModel model)
        {

            string permessiSelected = Request.Form["permessiIDs"];
            Debug.WriteLine("permessiIDs: " + permessiSelected);

            Debug.WriteLine("GruppoId: " + model.Gruppo.gruppoId);


            //if (String.IsNullOrEmpty(permessiSelected))
            //{
            //    ModelState.AddModelError(string.Empty, "Occorre selezionare almeno un permersso");
            //}


            if (ModelState.IsValid)
            {
                List<MyUsers.Models.MyRole> lista = new List<MyUsers.Models.MyRole>();

                if (permessiSelected != null)
                {
                    foreach (string id in permessiSelected.Split(','))
                    {
                        lista.Add(new MyUsers.Models.MyRole(id));
                    }

                }
                model.Gruppo.Ruoli = lista;


                manager.mOpenConnection();
                try
                {
                    manager.updateRuoli(model.Gruppo);
                }
                finally
                {
                    manager.mCloseConnection();
                }

                //return RedirectToAction("Details", new { id = model.Gruppo.gruppoId });
                return new RedirectResult(Url.Action("Details", new { id = model.Gruppo.gruppoId }) + "#tabs-3");
            }

            populateComboBox(model.Gruppo);
            return View("Edit", model);
        }




        public ActionResult Details(long id = 0)
        {
            Models.MyGroupModel model = new Models.MyGroupModel();
            manager.mOpenConnection();
            try
            {
                model.Gruppo = manager.getGroup(id);

                if (model.Gruppo == null)
                {
                    return HttpNotFound();
                }

                manager.setRoles(model.Gruppo);
                model.Utenti = manager.getUsers(model.Gruppo.gruppoId);
            }
            finally
            {
                manager.mCloseConnection();
            }
            return View(model);
        }



        public ActionResult Users(long id = 0)
        {
            List<MyUsers.Models.MyUser> lista;

            lista = manager.getUsers(id);


            return PartialView("listaUtenti", lista);
        }




        private void populateComboBox(MyUsers.Models.MyGroup group)
        {
            List<MyUsers.Models.MyRole> listaPermessi;
            manager.mOpenConnection();
            try
            {
                listaPermessi = manager.getRoleList();


                MultiSelectList msl;
                if (group != null && group.Ruoli != null)
                {
                    msl = new MultiSelectList(listaPermessi, "permessoId", "nome", group.Ruoli.Select(x => x.ruoloId).ToArray());
                }
                else
                {
                    msl = new MultiSelectList(listaPermessi, "permessoId", "nome", null);
                }

                ViewBag.Permessi = msl;

            }
            finally
            {
                manager.mCloseConnection();
            }




        }


        public ActionResult SearchAD()
        {
            Debug.WriteLine("Search in Active Directory");


            MyUsers.LdapManager managerAD = new MyUsers.LdapManager(System.Configuration.ConfigurationManager.AppSettings["ldap.server"], System.Configuration.ConfigurationManager.AppSettings["ldap.login"], System.Configuration.ConfigurationManager.AppSettings["ldap.password"], System.Configuration.ConfigurationManager.AppSettings["ldap.container"]);
            managerAD.getGroups();


            return View();
        }





        public ActionResult SearchUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(long gruppoId)
        {
            Debug.WriteLine("gruppoId: " + gruppoId);
            Debug.WriteLine("myItems: " + Request["myItems"]);

            manager.mOpenConnection();
            try
            {
                foreach (string s in Request["myItems"].Split(','))
                {
                    manager.addUser(gruppoId, long.Parse(s));
                }
            }
            finally
            {
                manager.mCloseConnection();
            }


           // return RedirectToAction("Details", new { id = gruppoId });
            return new RedirectResult(Url.Action("Edit", new { id = gruppoId }) + "#tabs-2");
        }



        public ActionResult DeleteUser(long id, long userId)
        {
            Debug.WriteLine(String.Format("DeleteUser - GruppoId: {0}  UserId: {1}", id, userId));


            Models.MyGroupModel model = new Models.MyGroupModel();
            manager.mOpenConnection();
            try
            {
                model.Gruppo = manager.getGroup(id);

                if (model.Gruppo == null)
                {
                    return HttpNotFound();
                }


                MyUsers.UserManager userManager = new UserManager(manager.mGetConnection());

                MyUsers.Models.MyUser user = null;
                user = userManager.getUser(userId);


                List<MyUsers.Models.MyUser> risultato = new List<MyUsers.Models.MyUser>();

                risultato.Add(user);
                model.Utenti = risultato;


            }
            finally
            {
                manager.mCloseConnection();
            }


            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeleteUser")]
        public ActionResult DeleteUserPost(long gruppoId, long userId)
        {
            Debug.WriteLine(String.Format("DeleteUser - GruppoId: {0}  UserId: {1}", gruppoId, userId));

            manager.mOpenConnection();

            try
            {
                manager.deleteUser(gruppoId, userId);
            }
            finally
            {
                manager.mCloseConnection();
            }

            return RedirectToAction("Edit", "Groups", new { id = gruppoId });
        }


    }
}
