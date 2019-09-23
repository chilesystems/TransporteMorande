using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    [RoutePrefix("api/Config")]
    public class ConfigController : ApiController
    {
        private appmorandeEntities db = new appmorandeEntities();
        [HttpGet]
        [Route("Paises")]
        public List<MPais> ObtenerPaises()
        {
            List<MPais> lista = new List<MPais>();
            var paises = db.Pais.ToList();
            foreach (var pais in paises)
            {
                MPais aux = new MPais()
                {
                    Id = pais.Id,
                    Nombre = pais.Nombre,
                    Sigla = pais.Sigla
                };
                lista.Add(aux);
            }
            //_evento.InsertarDetalle("Retorno", JsonConvert.SerializeObject(clientes));
            return lista;
        }

        [HttpGet]
        [Route("Idiomas")]
        public List<MIdioma> ObtenerIdiomas()
        {
            List<MIdioma> lista = new List<MIdioma>();
            var idiomas = db.Idioma.ToList();
            foreach (var pais in idiomas)
            {
                MIdioma aux = new MIdioma()
                {
                    Id = pais.Id,
                    Nombre = pais.Nombre,
                    Sigla = pais.Sigla
                };
                lista.Add(aux);
            }
            //_evento.InsertarDetalle("Retorno", JsonConvert.SerializeObject(clientes));
            return lista;
        }
    }
}
