using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SitioWeb.Controllers
{
    public class esController : Controller
    {
        // GET: es
        public ActionResult Index()
        {
            TempData["idioma"] = "es";
            return RedirectToAction("Index", "Home");
        }
    }
}