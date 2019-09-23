
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransporteMorande.Models;
using TransporteMorande.Models.App;

namespace TransporteMorande.Extensions
{
    public static class HtmlExtension
    {
        public static string GetRol(this HtmlHelper helper)
        {
            if (HttpContext.Current.User.IsInRole("Administrador"))
                return "Administrador";
            else if (HttpContext.Current.User.IsInRole("Vendedor"))
                return "Vendedor";
            else if (HttpContext.Current.User.IsInRole("Acceso Web"))
                return "Acceso Web";
            return "";
        }
    }
}