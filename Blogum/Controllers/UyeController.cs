using Blogum.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Blogum.Controllers
{
    public class UyeController : Controller
    {
        BlogDB db = new BlogDB();

        // GET: Uye
        public ActionResult Index(int id)
        {
            var uye = db.Uyes.Where(x=>x.UyeId==id).SingleOrDefault();
            if (Convert.ToInt32(Session["uyeid"])!=uye.UyeId)
            {
                return HttpNotFound();
            }
            return View(uye);
        }

        public ActionResult LogIn()
        {
            return View();
        }

        public ActionResult Edit(int id) {
            var uye = db.Uyes.Where(x => x.UyeId == id).SingleOrDefault();
            if (Convert.ToInt32(Session["uyeid"]) !=uye.UyeId)
            {
                return HttpNotFound();
            }
            return View(uye);
        }

        [HttpPost]
        public ActionResult Edit(Uye uye,HttpPostedFileBase Foto) {
            var uyeGuncelle = db.Uyes.Where(x=>x.UyeId==uye.UyeId).SingleOrDefault();

            if (Foto != null)
            {
                if (System.IO.File.Exists(Server.MapPath(uye.Foto)))
                {
                    System.IO.File.Delete(Server.MapPath(uye.Foto));
                }
                WebImage img = new WebImage(Foto.InputStream);
                FileInfo fotoInfo = new FileInfo(Foto.FileName);

                string newFoto = Guid.NewGuid().ToString() + fotoInfo.Extension;
                img.Resize(150, 150);
                img.Save("~/Uploads/UyeFoto/" + newFoto);
                uyeGuncelle.Foto = "/Uploads/UyeFoto/" + newFoto;
            }
             uyeGuncelle.AdSoyad = uye.AdSoyad;
             uyeGuncelle.KullaniciAdi = uye.KullaniciAdi;
             uyeGuncelle.Sifre = uye.Sifre;
             uyeGuncelle.Email = uye.Email;
             
             db.SaveChanges();

             Session["uyeid"] = uye.UyeId;
             Session["kullanıcıadı"] = uye.KullaniciAdi;


             return RedirectToAction("Index","Home",new {id=uyeGuncelle.UyeId });


            
            
        }

        [HttpPost]
        public ActionResult Login(Uye uye) {
            var login= db.Uyes.Where(x => x.KullaniciAdi == uye.KullaniciAdi).SingleOrDefault();
            try {
                if (login.KullaniciAdi == uye.KullaniciAdi && login.Sifre == uye.Sifre && login.Email == uye.Email)
                {
                    Session["uyeid"] = login.UyeId;
                    Session["kullanıcıadı"] = login.KullaniciAdi;
                    Session["yetkiid"] = login.YetkiId;

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View();
                }
                
            } catch 
            {
                ViewBag.HataliGiris = "Hatali giriş yaptınız.Bilgilerinizi kontrol edin.";
                return View();
            }
            
            

            
        }

        public ActionResult LogOut()
        {
            Session["uyeid"] = null;
            Session.Abandon();
            return RedirectToAction("Index","Home");
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Uye uye,HttpPostedFileBase Foto)
        {
            if (ModelState.IsValid)
            {
                if (Foto!=null)
                {
                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotoInfo = new FileInfo(Foto.FileName);

                    string newFoto = Guid.NewGuid().ToString() + fotoInfo.Extension;
                    img.Resize(150, 150);
                    img.Save("~/Uploads/UyeFoto/" + newFoto);
                    uye.Foto = "/Uploads/UyeFoto/" + newFoto;

                    uye.YetkiId = 2;
                    db.Uyes.Add(uye);
                    db.SaveChanges();

                    Session["uyeid"] = uye.UyeId;
                    Session["kullanıcıadı"] = uye.KullaniciAdi;
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    ModelState.AddModelError("Fotograf","Fotograf Seciniz.");
                }
            }

            return View();
        }

        public ActionResult UyeProfil(int id) {

            var uye = db.Uyes.Where(x => x.UyeId == id).SingleOrDefault();
            return View(uye);
        }
    }
}