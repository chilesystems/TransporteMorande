using API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    [RoutePrefix("api/Hospedaje")]
    public class HospedajeController : ApiController
    {
        private appmorandeEntities db = new appmorandeEntities();

        public class parametrosHospedaje
        {
            //public List<MHospedaje> nuevos { get; set; }
            //public List<MHospedaje> modificados { get; set; }
            public string nuevos { get; set; }
        }

        [HttpPost]
        [Route("Actualizar")]
        public List<MHospedaje> ActualizarHospedajes(parametrosHospedaje p)
        {
            try
            {
                List<MHospedaje> hospedajesNuevos = JsonConvert.DeserializeObject<List<MHospedaje>>(p.nuevos);
                //List<MHospedaje> hospedajesNuevos = p.nuevos;
                foreach (var a in hospedajesNuevos)
                {
                    db.Hotel.Add(new Hotel()
                    {
                        Direccion = a.Direccion,
                        Id = a.Id,
                        Nombre = a.Nombre,
                        TelefonoPrimario = a.TelefonoPrimario,
                        TelefonoSecundario = a.TelefonoSecundario,
                        tipo = a.Tipo
                    });
                }
                db.SaveChanges();
            }
            catch { }
            List<MHospedaje> lista = new List<MHospedaje>();
            var hospedajes = db.Hotel.ToList();
            try
            {
                foreach (var hos in hospedajes)
                {
                    MHospedaje aux = new MHospedaje()
                    {
                        Direccion = hos.Direccion,
                        Tipo = hos.tipo,
                        TelefonoSecundario  = hos.TelefonoSecundario,
                        TelefonoPrimario = hos.TelefonoPrimario,
                        Nombre = hos.Nombre,
                        Id = hos.Id
                    };
                    lista.Add(aux);
                }
            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
            }
            return lista;
        }
    }
}
