using SitioWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SitioWeb.Controllers
{
    public class HomeController : Controller
    {
        appmorandeEntities db = new appmorandeEntities();

        public ActionResult Index()
        {
            TempData.Keep();
            if (TempData["idioma"] == null)
            {
                TempData["idioma"] = "es";
            }
            var lista = db.Slider.ToList();
            return View(lista);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}