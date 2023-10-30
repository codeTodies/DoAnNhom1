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
    public class RegionController : Controller
    {
        GredEntities database = new GredEntities();
        // GET: Region
        public ActionResult Index()
        {
            return View(database.Regions.ToList());
        }
        public ActionResult Create()
        {
            Region region = new Region();
            return View(region);
        }
        [HttpPost]
        public ActionResult Create(Region region)
        {
            try
            {
                if (region.UploadImage != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(region.UploadImage.FileName);
                    string extend = Path.GetExtension(region.UploadImage.FileName);
                    fileName = fileName + extend;
                    region.ImgRe = "~/image/" + fileName;
                    if (extend.ToLower() == ".jpg" || extend.ToLower() == ".jpeg" || extend.ToLower() == ".png")
                    {
                        if (region.UploadImage.ContentLength <= 6000000)
                        {
                            database.Regions.Add(region);
                            if (database.SaveChanges() > 0)
                            {
                                region.UploadImage.SaveAs(Path.Combine(Server.MapPath("~/image/"), fileName));
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
            // Lấy đối tượng Region từ ID
            var region=database.Regions.Where(s => s.Id == id).FirstOrDefault();
            Session["imgPath"] = region.ImgRe;
            if (region == null)
            {
                return HttpNotFound(); // Trả về lỗi 404 nếu không tìm thấy đối tượng
            }

            return View(region);
        }

        [HttpPost]
        public ActionResult Edit(Region region)
        {
            if (ModelState.IsValid)
            {

                if (region.UploadImage != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(region.UploadImage.FileName);
                    string extend = Path.GetExtension(region.UploadImage.FileName);
                    fileName = fileName + extend;
                    region.ImgRe = "~/image/" + fileName;
                    if (extend.ToLower() == ".jpg" || extend.ToLower() == ".jpeg" || extend.ToLower() == ".png")
                    {
                        if (region.UploadImage.ContentLength <= 6000000)
                        {
                            database.Entry(region).State = EntityState.Modified;

                            string oldImgPath = Request.MapPath(Session["imgPath"].ToString());
                            if (database.SaveChanges() > 0)
                            {
                                region.UploadImage.SaveAs(Path.Combine(Server.MapPath("~/image/"), fileName));
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
                    region.ImgRe = Session["imgPath"].ToString();
                    database.Entry(region).State = EntityState.Modified;
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
            var region = database.Regions.Where(s => s.Id == id).FirstOrDefault();
            if (region == null)
            {
                return HttpNotFound();
            }
            string curImg = Request.MapPath(region.ImgRe);
            database.Entry(region).State = EntityState.Deleted;
            if (database.SaveChanges() > 0)
            {
                if (System.IO.File.Exists(curImg))
                {
                    System.IO.File.Delete(curImg);
                }
                TempData["nofi"] = "Region Deleted";
                return RedirectToAction("Index");
            }
            return View();
        }

    }
}