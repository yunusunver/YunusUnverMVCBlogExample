using Blogum.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blogum.Controllers
{
    public class HomeController : Controller
    {
        BlogDB db = new BlogDB();
        // GET: Home
        public ActionResult Index(int Page=1)
        {
            var makale = db.Makales.OrderByDescending(x => x.MakaleId).ToPagedList(Page,5);
            return View(makale);
        }

        public ActionResult BlogAra(string Ara=null)
        {
            var aranan = db.Makales.Where(x => x.Baslik.Contains(Ara)).ToList();


            return View(aranan.OrderByDescending(x=>x.Tarih));
        }

      public ActionResult SonYorumlarPartial()
        {
            return View(db.Yorums.ToList());
        }

        public ActionResult PopulerMakalelerPartial()
        {
            return View(db.Makales.OrderByDescending(x=>x.Okunan).Take(5));
        }

        



        public ActionResult KategoriMakale(int id)
        {
            var makaleler = db.Makales.Where(x => x.KategoriId == id).ToList();
            return View(makaleler);
        }

        public ActionResult MakaleDetay(int id)
        {
            var makale = db.Makales.Where(x=>x.MakaleId==id).SingleOrDefault();
            if (makale==null)
            {
                return HttpNotFound();
            }
            return View(makale);
        }

        public JsonResult YorumYap(string yorum, int MakaleId)
        {
            var uyeid = Session["uyeid"];

            if (yorum == null)
            {
                return Json(true,JsonRequestBehavior.AllowGet);
            }

            db.Yorums.Add(new Yorum { UyeId = Convert.ToInt32(uyeid), MakaleId = MakaleId, Tarih = DateTime.Now, Icerik = yorum });
            db.SaveChanges();

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult YorumSil(int id)
        {
            var uyeid = Session["uyeid"];
            var yorum = db.Yorums.Where(x => x.YorumId == id).SingleOrDefault();
            var makale = db.Makales.Where(x=>x.MakaleId==yorum.MakaleId).SingleOrDefault();
            if (yorum.UyeId==Convert.ToInt32(uyeid))
            {
                db.Yorums.Remove(yorum);
                db.SaveChanges();
                return RedirectToAction("MakaleDetay","Home",new {id=makale.MakaleId });
            }
            else
            {
                return HttpNotFound();
            }
        }
        public ActionResult Hakkımızda()
        {
            return View();
        }

        public ActionResult Iletisim()
        {
            return View();
        }

        public ActionResult KategoriPartial()
        {
            return View(db.Kategoris.ToList());
        }

        public ActionResult OkunmaArttir(int MakaleId)
        {
            var makale = db.Makales.Where(x => x.MakaleId == MakaleId).SingleOrDefault();
            makale.Okunan += 1;
            db.SaveChanges();
            return View();
        }

    }
}