using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class Evento
    {
        string Origen { get; set; }
        string Cliente { get { return "Morandé"; } }
        Guid Id { get; set; }
        DateTime Fecha { get; set; }
        string Controlador { get; set; }
        string Metodo { get; set; }
        string Descripcion { get; set; }
        string Comentarios { get; set; }

        private simple_apiEntities db = new simple_apiEntities();

        public Evento(string origen, string controlador, string metodo)
        {
            Origen = origen;
            Controlador = controlador;
            Metodo = metodo;
        }

        public void InsertarDetalle(string descripcion, string comentarios)
        {
            try
            {
                db.EVENTO_WEBAPI.Add(new EVENTO_WEBAPI()
                {
                    Origen = Origen,
                    Cliente = Cliente,
                    Id = Guid.NewGuid(),
                    Fecha = DateTime.Now,
                    Controlador = Controlador,
                    Metodo = Metodo,
                    Descripcion = descripcion,
                    Comentarios = comentarios
                });
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}