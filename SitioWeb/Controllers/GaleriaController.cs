using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SitioWeb.Controllers
{
    public class GaleriaController : Controller
    {
        // GET: Galeria
        public ActionResult Index()
        {
            TempData.Keep();
            if (TempData["idioma"] == null)
            {
                TempData["idioma"] = "es";
            }
            return View();
        }
    }
}