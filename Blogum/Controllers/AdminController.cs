using Blogum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blogum.Controllers
{
    public class AdminController : Controller
    {
        BlogDB db = new BlogDB();
        // GET: Admin
        public ActionResult Index()
        {
            ViewBag.makaleSayisi = db.Makales.Count();
            ViewBag.yorumSayisi = db.Yorums.Count();
            ViewBag.kategoriSayisi = db.Kategoris.Count();
            ViewBag.uyeSayisi = db.Uyes.Count();
            ViewBag.sonYorumTarihi = db.Yorums.Max(x=>x.Tarih).ToString();
            return View(db.Yorums.OrderByDescending(x=>x.Tarih).Take(5).ToList());
        }
    }
}