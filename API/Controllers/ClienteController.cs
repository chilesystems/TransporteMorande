using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using API.Models;
using System.Data.Entity;
using Newtonsoft.Json;

namespace API.Controllers
{
    [RoutePrefix("api/Clientes")]
    public class ClienteController : ApiController
    {
        private appmorandeEntities db = new appmorandeEntities();

        [HttpPost]
        [Route("Actualizar")]
        public List<MCliente> ActualizarClientes(parametrosClientes p)
        {
            try
            {
                List<MCliente> clientesNuevos = JsonConvert.DeserializeObject<List<MCliente>>(p.nuevos);
                //List<MCliente> clientesNuevos = p.nuevos;
                foreach (var a in clientesNuevos)
                {
                    db.Cliente.Add(new Cliente()
                    {
                        ApellidoMaterno = a.ApellidoMaterno,
                        ApellidoPaterno = a.ApellidoPaterno,
                        Email = a.Email,
                        Id = a.Id,
                        IdIdioma = a.IdIdioma,
                        IdPais = a.IdPais,
                        Nombre = a.Nombre,
                        NombreCompleto = a.NombreCompleto,
                        TelefonoPrimario = a.TelefonoPrimario,
                        TelefonoSecundario = a.TelefonoSecundario
                    });
                }
            }
            catch { }
            try
            {
                List<MCliente> clientesModificados = JsonConvert.DeserializeObject<List<MCliente>>(p.modificados);
                //List<MCliente> clientesModificados = p.modificados;
                foreach (var a in clientesModificados)
                {
                    var clienteAux = db.Cliente.First(x => x.Id == a.Id);
                    clienteAux.ApellidoMaterno = a.ApellidoMaterno;
                    clienteAux.ApellidoPaterno = a.ApellidoPaterno;
                    clienteAux.Email = a.Email;
                    clienteAux.IdIdioma = a.IdIdioma;
                    clienteAux.IdPais = a.IdPais;
                    clienteAux.Nombre = a.Nombre;
                    clienteAux.NombreCompleto = a.NombreCompleto;
                    clienteAux.TelefonoPrimario = a.TelefonoPrimario;
                    clienteAux.TelefonoSecundario = a.TelefonoSecundario;
                    db.Entry(clienteAux).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            catch { }
            
            List<MCliente> lista = new List<MCliente>();
            var clientes = db.Cliente.ToList();
            try
            {
                foreach (var cli in clientes)
                {

                    var idioma = db.Idioma.First(x => x.Id == cli.IdIdioma);
                    var pais = db.Pais.First(x => x.Id == cli.IdPais);
                    MCliente aux = new MCliente()
                    {
                        ApellidoMaterno = cli.ApellidoMaterno,
                        ApellidoPaterno = cli.ApellidoPaterno,
                        Email = cli.Email,
                        Id = cli.Id,
                        IdIdioma = cli.IdIdioma,
                        IdPais = cli.IdPais,
                        Nombre = cli.Nombre.ToUpper(),
                        NombreCompleto = cli.NombreCompleto.ToUpper(),
                        TelefonoPrimario = cli.TelefonoPrimario,
                        TelefonoSecundario = cli.TelefonoSecundario,
                        IdiomaNombre = idioma.Nombre,
                        PaisNombre = pais.Nombre
                    };
                    lista.Add(aux);
                }
            }
            catch(Exception ex)
            {
                string mensaje = ex.Message;
            }
            //_evento.InsertarDetalle("Retorno", JsonConvert.SerializeObject(clientes));
            return lista;
        }

        public class parametrosClientes
        {
            //public List<MCliente> nuevos { get; set; }
            //public List<MCliente> modificados { get; set; }
            public string nuevos { get; set; }
            public string modificados { get; set; }
        }

        public class parametrosDomicilios
        {
            public string nuevos { get; set; }
        }

        [HttpPost]
        [Route("Domicilios")]
        public List<MDomicilio> ObtenerDomicilios(parametrosDomicilios p)
        {
            try
            {
                List<MDomicilio> domiciliosNuevos = JsonConvert.DeserializeObject<List<MDomicilio>>(p.nuevos);
                foreach (var a in domiciliosNuevos)
                {
                    db.Domicilio.Add(new Domicilio()
                    {
                        Activo = true,
                        Calle = a.Calle,
                        Complemento = a.Complemento,
                        Id = a.Id,
                        IdCliente = a.IdCliente,
                        Numero = a.Numero,
                        Referencia = a.Referencia
                    });
                }
                db.SaveChanges();
            }
            catch { }            

            List<MDomicilio> lista = new List<MDomicilio>();
            var domicilios = db.Domicilio.ToList();
            foreach (var d in domicilios)
            {
                MDomicilio aux = new MDomicilio()
                {
                    Activo = d.Activo,
                    Calle = d.Calle,
                    Complemento = d.Complemento,
                    Id = d.Id,
                    IdCliente = d.IdCliente,
                    Numero = d.Numero,
                    Referencia = d.Referencia
                };
                lista.Add(aux);
            }
            //_evento.InsertarDetalle("Retorno", JsonConvert.SerializeObject(clientes));
            return lista;
        }
    }
}
