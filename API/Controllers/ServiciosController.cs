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
    [RoutePrefix("api/Servicios")]
    public class ServiciosController : ApiController
    {
        private appmorandeEntities db = new appmorandeEntities();

        [HttpGet]
        [Route("Listado")]
        public List<MServicio> ActualizarServicios()
        {
            List<MServicio> lista = new List<MServicio>();
            var servicios = db.Servicio.Where(x=>x.Activo).ToList();
            try
            {
                foreach (var serv in servicios)
                {
                    MServicio aux = new MServicio()
                    {
                        Activo = true,
                        Contenido = serv.Contenido,
                        ContenidoPortugues = serv.ContenidoPortugues,
                        FechaIngreso = serv.FechaIngreso,
                        Id = serv.Id,
                        Imagen = serv.Imagen,
                        Precio = serv.Precio,
                        Titulo = serv.Titulo,
                        Usuario = serv.Usuario,
                        Web = serv.Web,
                        NombreArchivo = serv.NombreArchivo
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
