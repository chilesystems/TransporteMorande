using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SitioWeb.Controllers
{
    public class ptController : Controller
    {
        // GET: pt
        public ActionResult Index()
        {
            TempData["idioma"] = "pt";
            return RedirectToAction("Index", "Home");
        }
    }
}