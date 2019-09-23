using SitioWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SitioWeb.Controllers
{
    public class TurismoController : Controller
    {
        appmorandeEntities db = new appmorandeEntities();
        // GET: Turismo
        public ActionResult Index()
        {
            TempData.Keep();
            if (TempData["idioma"] == null)
            {
                TempData["idioma"] = "es";
            }
            var lista = db.Servicio.Where(x=>x.Web && x.Activo).ToList();
            return View(lista);
        }
    }
}