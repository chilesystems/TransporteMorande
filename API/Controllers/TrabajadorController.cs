using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    [RoutePrefix("api/Trabajador")]
    public class TrabajadorController : ApiController
    {
        [HttpGet]
        [Route("Login/{rut}/{pass}")]
        public MTrabajador Login(string rut, string pass)
        {
            appmorandeEntities db = new appmorandeEntities();
            AspNetUsers traChilesystems = db.AspNetUsers.FirstOrDefault(x => x.UserName == rut && x.Pass == pass);

            if (traChilesystems != null)
            {
                MTrabajador trabajador = new MTrabajador();
                trabajador.Nombre = traChilesystems.Nombre;
                trabajador.Apellido = traChilesystems.Apellido;
                trabajador.RUT = rut;
                trabajador.tempPassword = pass;
                trabajador.UserName = traChilesystems.UserName;
                trabajador.Id = traChilesystems.Id;
                trabajador.Email = traChilesystems.Email;
                trabajador.Imagen = traChilesystems.Imagen;
                trabajador.Logueado = true;
                return trabajador;
            }
            else return new MTrabajador() { Logueado = false };
        }
    }
}
