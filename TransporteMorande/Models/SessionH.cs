using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransporteMorande.Models
{
    public class SessionH
    {
        private const string VAR_LOGUEADO = "logueado";

        private static T Lee<T>(string variable)
        {
            object obj = HttpContext.Current.Session[variable];
            if (obj == null)
                return default(T);
            return (T)obj;
        }

        private static void Escribe(string variable, object valor)
        {
            HttpContext.Current.Session[variable] = valor;
        }

        public static bool Logueado
        {
            get
            {
                return SessionH.Lee<bool>("logueado");
            }
            set
            {
                SessionH.Escribe("logueado", (object)value);
            }
        }
    }
}