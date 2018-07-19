using Blogum.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace Blogum.Controllers
{
    public class AdminMakaleController : Controller
    {
        BlogDB db = new BlogDB();
        // GET: AdminMakale
        public ActionResult Index(int Page=1)
        {
            var makale = db.Makales.OrderByDescending(x=>x.Tarih).ToPagedList(Page,10);
            return View(makale);
        }

        // GET: AdminMakale/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

   

        // GET: AdminMakale/Create
        public ActionResult Create()
        {
            ViewBag.KategoriId =new SelectList(db.Kategoris.ToList(), "KategoriId", "KategoriAdi");
            return View();
        }

        // POST: AdminMakale/Create
        [HttpPost]
        public ActionResult Create(Makale makale,string etiketler,HttpPostedFileBase Foto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ViewBag.KategoriId = new SelectList(db.Kategoris.ToList(), "KategoriId", "KategoriAdi");

                    if (Foto != null)
                    {
                        WebImage img = new WebImage(Foto.InputStream);
                        FileInfo fotoInfo = new FileInfo(Foto.FileName);

                        string newFoto = Guid.NewGuid().ToString() + fotoInfo.Extension;
                        img.Resize(800, 350);
                        img.Save("~/Uploads/MakaleFoto/" + newFoto);
                        makale.Foto = "/Uploads/MakaleFoto/" + newFoto;



                    }
                    if (etiketler != null)
                    {
                        string[] etiketDizi = etiketler.Split(',');
                        foreach (var item in etiketDizi)
                        {
                            var yeniEtiket = new Etiket { EtiketAdi = item };
                            db.Etikets.Add(yeniEtiket);
                            makale.Etikets.Add(yeniEtiket);
                        }
                    }
                    makale.UyeId = 1;
                    makale.Okunan = 0;
                    db.Makales.Add(makale);
                    db.SaveChanges();
                    
                }
                return RedirectToAction("Index");
            }
            catch (Exception)
            {

                return View(makale);
            }
           
            
        }

        // GET: AdminMakale/Edit/5
        public ActionResult Edit(int id)
        {
            var makale = db.Makales.Where(x => x.MakaleId == id).SingleOrDefault();
            if (makale==null)
            {
                return HttpNotFound();
            }
            ViewBag.KategoriId = new SelectList(db.Kategoris,"KategoriId","KategoriAdi",makale.KategoriId);
            return View(makale);
        }

        // POST: AdminMakale/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Makale makale,HttpPostedFileBase Foto)
        {
            try
            {
                var makaleEdit = db.Makales.Where(x => x.MakaleId == id).SingleOrDefault();

                if (Foto!=null)
                {
                    if (System.IO.File.Exists(Server.MapPath(makaleEdit.Foto)))
                    {
                        System.IO.File.Delete(Server.MapPath(makaleEdit.Foto));
                    }
                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotoInfo = new FileInfo(Foto.FileName);

                    string newFoto = Guid.NewGuid().ToString() + fotoInfo.Extension;
                    img.Resize(800, 350);
                    img.Save("~/Uploads/MakaleFoto/" + newFoto);
                    makaleEdit.Foto = "/Uploads/MakaleFoto/" + newFoto;
                    makaleEdit.Baslik = makale.Baslik;
                    makaleEdit.Icerik = makale.Icerik;
                    makaleEdit.KategoriId = makale.KategoriId;
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminMakale/Delete/5
        public ActionResult Delete(int id)
        {
            var makale = db.Makales.Where(x=>x.MakaleId==id).SingleOrDefault();
            if (makale==null)
            {
                return HttpNotFound();
            }
            return View(makale);
        }

        // POST: AdminMakale/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteMakale(int id)
        {
            try
            {
                var makale = db.Makales.Where(x => x.MakaleId == id).SingleOrDefault();
                if (makale == null)
                {
                    return HttpNotFound();
                }
                if (System.IO.File.Exists(Server.MapPath(makale.Foto)))
                {
                    System.IO.File.Delete(Server.MapPath(makale.Foto));
                }

                foreach (var item in makale.Yorums.ToList())
                {
                    db.Yorums.Remove(item);
                }

                foreach (var item in makale.Etikets.ToList())
                {
                    db.Etikets.Remove(item);
                }

                db.Makales.Remove(makale);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
