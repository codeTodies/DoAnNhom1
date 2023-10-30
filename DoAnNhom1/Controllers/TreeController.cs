using DoAnNhom1.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnNhom1.Controllers
{

    public class TreeController : Controller
    {
        GredEntities database = new GredEntities();
        // GET: Tree
        public ActionResult Index()
        {
            return View(database.Trees.ToList());
        }
        public ActionResult SelectRegion()
        {
            Region se_city = new Region();
            se_city.ListRe = database.Regions.ToList<Region>();
            return PartialView(se_city);
        }
        public ActionResult Create()
        {
            Tree tree = new Tree();
            return View(tree);
        }
        [HttpPost]
        public ActionResult Create(Tree tree)
        {
            try
            {
                if (tree.UploadImage != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(tree.UploadImage.FileName);
                    string extend = Path.GetExtension(tree.UploadImage.FileName);
                    fileName = fileName + extend;
                    tree.ImageTree = "~/image/" + fileName;
                    if (extend.ToLower() == ".jpg" || extend.ToLower() == ".jpeg" || extend.ToLower() == ".png")
                    {
                        if (tree.UploadImage.ContentLength <= 6000000)
                        {
                            database.Trees.Add(tree);
                            if (database.SaveChanges() > 0)
                            {
                                tree.UploadImage.SaveAs(Path.Combine(Server.MapPath("~/image/"), fileName));
                                ViewBag.nofi = "Region added success";
                                ModelState.Clear();
                            }
                        }
                        else
                        {
                            ViewBag.nofi = "File Size must be Equal or less than 6mb";
                        }
                    }
                    else
                    {
                        ViewBag.nofi = "Invalid File Type";
                    }
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            var tree = database.Trees.Where(s => s.TreeID == id).FirstOrDefault();
            Session["imgPath"] = tree.ImageTree;
            ViewBag.Regions = new SelectList(database.Regions.OrderBy(r => r.NameRe), "IDRe", "NameRe", tree.Region.IDRe);
            if (tree == null)
            {
                return HttpNotFound(); // Trả về lỗi 404 nếu không tìm thấy đối tượng
            }

            return View(tree);
        }

        [HttpPost]
        public ActionResult Edit(Tree tree)
        {
            if (ModelState.IsValid)
            {

                if (tree.UploadImage != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(tree.UploadImage.FileName);
                    string extend = Path.GetExtension(tree.UploadImage.FileName);
                    fileName = fileName + extend;
                    tree.ImageTree = "~/image/" + fileName;
                    if (extend.ToLower() == ".jpg" || extend.ToLower() == ".jpeg" || extend.ToLower() == ".png")
                    {
                        if (tree.UploadImage.ContentLength <= 6000000)
                        {
                            database.Entry(tree).State = EntityState.Modified;

                            string oldImgPath = Request.MapPath(Session["imgPath"].ToString());
                            if (database.SaveChanges() > 0)
                            {
                                tree.UploadImage.SaveAs(Path.Combine(Server.MapPath("~/image/"), fileName));
                                if (System.IO.File.Exists(oldImgPath))
                                {
                                    System.IO.File.Delete(oldImgPath);
                                }
                                TempData["nofi"] = "Update Success";
                                return RedirectToAction("Index");
                            }
                        }
                        else
                        {
                            ViewBag.nofi = "File Size must be Equal or less than 6mb";
                        }
                    }
                    else
                    {
                        ViewBag.nofi = "Invalid File Type";
                    }
                }
                else
                {
                    tree.ImageTree = Session["imgPath"].ToString();
                    database.Entry(tree).State = EntityState.Modified;
                    if (database.SaveChanges() > 0)
                    {
                        TempData["nofi"] = "Update Success";
                        return RedirectToAction("Index");
                    }
                }
            }
            return View();
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            var tree = database.Trees.Where(s => s.TreeID == id).FirstOrDefault();
            if (tree == null)
            {
                return HttpNotFound();
            }
            string curImg = Request.MapPath(tree.ImageTree);
            database.Entry(tree).State = EntityState.Deleted;
            if (database.SaveChanges() > 0)
            {
                if (System.IO.File.Exists(curImg))
                {
                    System.IO.File.Delete(curImg);
                }
                TempData["nofi"] = "Tree Deleted";
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}