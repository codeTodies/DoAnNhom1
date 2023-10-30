using DoAnNhom1.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnNhom1.Controllers
{
    public class UserController : Controller
    {
        GredEntities database = new GredEntities();
        // GET: User
        public ActionResult Index()
        {
            return View(database.AdminUsers.ToList());
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(AdminUser user)
        {
            try
            {
                database.AdminUsers.Add(user);
                database.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return Content("Error");
            }
        }
        public ActionResult Edit(int id)
        {
            return View(database.AdminUsers.Where(s => s.ID == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult Edit(int id, AdminUser user)
        {
            database.Entry(user).State = System.Data.Entity.EntityState.Modified;
            database.SaveChanges();
            TempData["nofi"] = "Update Success";
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            var admin = database.AdminUsers.Where(s => s.ID == id).FirstOrDefault();
            if (admin == null)
            {
                return HttpNotFound();
            }
            database.Entry(admin).State = EntityState.Deleted;
            if (database.SaveChanges() > 0)
            {
                TempData["nofi"] = "User Deleted";
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}