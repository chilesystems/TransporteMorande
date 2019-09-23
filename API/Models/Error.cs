using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class Error
    {
        string Origen { get; set; }
        string Cliente { get { return "Morandé"; } }
        Guid Id { get; set; }
        DateTime Fecha { get; set; }
        string StackTrace { get; set; }
        string Message { get; set; }
        string Comentarios { get; set; }

        private simple_apiEntities db = new simple_apiEntities();

        public Error(string origen)
        {
            Origen = origen;
        }

        public void InsertarError(string message, string stacktrace, string comentarios = "")
        {
            db.ERROR_WEBAPI.Add(new ERROR_WEBAPI()
            {
                Origen = Origen,
                Cliente = Cliente,
                Id = Guid.NewGuid(),
                Fecha = DateTime.Now,
                Comentarios = comentarios,
                Message = message,
                StackTrace = stacktrace
            });
            db.SaveChanges();
        }
    }
}