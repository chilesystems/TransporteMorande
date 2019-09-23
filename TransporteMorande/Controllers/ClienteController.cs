using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransporteMorande.Models;
using TransporteMorande.Models.App;

namespace TransporteMorande.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private appmorandeEntities db = new appmorandeEntities();
        private ClienteViewModel model = new ClienteViewModel();

        public ActionResult Index(ClienteViewModel model)
        {
            List<Cliente> lista = db.Cliente.ToList();
            if (!string.IsNullOrEmpty(model.NombreSeleccionado))
                lista = lista.Where(x => x.NombreCompleto.ToUpper().Contains(model.NombreSeleccionado.ToUpper())).ToList();
            if (!string.IsNullOrEmpty(model.EmailSeleccionado))
                lista = lista.Where(x => x.Email.ToUpper().Contains(model.EmailSeleccionado.ToUpper())).ToList();
            if (model.IdiomaSeleccionado.HasValue)
                lista = lista.Where(x => x.IdIdioma.Value == model.IdiomaSeleccionado).ToList();
            if (model.PaisSeleccionado.HasValue)
                lista = lista.Where(x => x.IdPais.Value == model.PaisSeleccionado).ToList();
            model.Clientes = lista;
            return View(model);
        }


        [HttpPost]
        public PartialViewResult Nuevo(NuevoClienteFormModel model)
        {
            if (model.IdPais == null)
            {
                return PartialView("../Shared/Mensaje", new Models.App.MensajeViewModel() { Mensaje = "Debe seleccionar un país.", Error = true });
            }
            if (model.IdIdioma == null)
            {
                return PartialView("../Shared/Mensaje", new Models.App.MensajeViewModel() { Mensaje = "Debe seleccionar un idioma.", Error = true });
            }
            if (ModelState.IsValid)
            {
                var cliente = new Cliente
                {
                    ApellidoMaterno = model.ApellidoMaterno,
                    ApellidoPaterno = model.ApellidoPaterno,
                    Email = model.Email,
                    Id = Guid.NewGuid(),
                    IdIdioma = model.IdIdioma,
                    IdPais = model.IdPais,
                    Nombre = model.Nombre,
                    TelefonoPrimario = model.TelefonoPrimario,
                    TelefonoSecundario = model.TelefonoSecundario
                };
                db.Cliente.Add(cliente);

                var listaDomicilios = TempData["DomiciliosNuevos"] as List<DomicilioFormModel>;
                if (listaDomicilios != null)
                {
                    foreach (var a in listaDomicilios)
                        db.Domicilio.Add(new Domicilio()
                        {
                            Id = a.Id,
                            Calle = a.Calle,
                            Activo = true,
                            Complemento = a.Complemento,
                            IdCliente = cliente.Id,
                            Numero = a.Numero,
                            Referencia = a.Referencia
                        });
                }

                db.SaveChanges();
                return PartialView("../Shared/Mensaje", new Models.App.MensajeViewModel() { Mensaje = "Cliente creado exitosamente", Error = false });
            }
            else
            {
                return PartialView("../Shared/Mensaje", new Models.App.MensajeViewModel() { Mensaje = "Error de formulario. Verifique los campos.", Error = true });

            }
            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            // return PartialView("../Shared/Mensaje", new Models.App.MensajeViewModel() { Mensaje = "Error", Error = true });
            //return Json("Error", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public PartialViewResult Modificar(NuevoClienteFormModel model)
        {
            if (!model.IdPais.HasValue)
            {
                return PartialView("../Shared/Mensaje", new Models.App.MensajeViewModel() { Mensaje = "Debe seleccionar un país.", Error = true });
            }
            if (!model.IdIdioma.HasValue)
            {
                return PartialView("../Shared/Mensaje", new Models.App.MensajeViewModel() { Mensaje = "Debe seleccionar un idioma.", Error = true });
            }
            if (ModelState.IsValid)
            {
                var cliente = db.Cliente.First(x => x.Id == model.Id);
                cliente.ApellidoMaterno = model.ApellidoMaterno;
                cliente.ApellidoPaterno = model.ApellidoPaterno;
                cliente.Email = model.Email;
                cliente.IdIdioma = model.IdIdioma;
                cliente.IdPais = model.IdPais;
                cliente.Nombre = model.Nombre;
                cliente.TelefonoPrimario = model.TelefonoPrimario;
                cliente.TelefonoSecundario = model.TelefonoSecundario;
                try
                {
                    db.Entry(cliente).State = EntityState.Modified;
                    var listaDomicilios = TempData["DomiciliosNuevos"] as List<DomicilioFormModel>;
                    db.Domicilio.RemoveRange(cliente.Domicilio);
                    if (listaDomicilios != null)
                    {
                        foreach (var a in listaDomicilios)
                            db.Domicilio.Add(new Domicilio()
                            {
                                Id = a.Id,
                                Calle = a.Calle,
                                Activo = true,
                                Complemento = a.Complemento,
                                IdCliente = cliente.Id,
                                Numero = a.Numero,
                                Referencia = a.Referencia
                            });
                    }
                    db.SaveChanges();

                }
                catch (Exception ex)
                {
                    return PartialView("../Shared/Mensaje", new Models.App.MensajeViewModel() { Mensaje = "Error al guardar en la base de datos: " + ex.Message, Error = true });
                }

                return PartialView("../Shared/Mensaje", new Models.App.MensajeViewModel() { Mensaje = "Cliente modificado exitosamente", Error = false });
            }
            else
            {
                return PartialView("../Shared/Mensaje", new Models.App.MensajeViewModel() { Mensaje = "Error de formulario. Verifique los campos.", Error = true });
            }
        }

        public JsonResult Obtener(string id)
        {
            var cliente = db.Cliente.First(x => x.Id.ToString() == id);
            var domiciliosAux = db.Domicilio.Where(x => x.IdCliente.ToString() == id).ToList();
            var listaDomicilios = new List<DomicilioFormModel>();
            foreach (var a in domiciliosAux)
                listaDomicilios.Add(new DomicilioFormModel()
                {
                    Id = a.Id,
                    Calle = a.Calle,
                    Complemento = a.Complemento,
                    Numero = a.Numero,
                    Referencia = a.Referencia
                });
            TempData["DomiciliosNuevos"] = listaDomicilios;

            return Json(new
            {
                nombre = cliente.Nombre,
                apellidoPaterno = cliente.ApellidoPaterno,
                apellidoMaterno = cliente.ApellidoMaterno,
                email = cliente.Email,
                telefono1 = cliente.TelefonoPrimario,
                telefono2 = cliente.TelefonoSecundario,
                idPais = cliente.IdPais,
                idIdioma = cliente.IdIdioma,
                domicilios = listaDomicilios
            }, JsonRequestBehavior.AllowGet);
        }
    

        public JsonResult EliminarCliente(string id)
        {
            try
            {
                var cliente = db.Cliente.First(x => x.Id.ToString() == id);
                db.Domicilio.RemoveRange(cliente.Domicilio);
                db.Cliente.Remove(cliente);                
                db.SaveChanges();
                return Json("Exito", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error: " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetClientes(string texto, string tipo)
        {
            List<Cliente> retorno = new List<Cliente>();
            if (!string.IsNullOrEmpty(texto))
            {
                retorno = db.Cliente.Where(x => x.NombreCompleto.ToUpper().Contains(texto.ToUpper())).ToList();
            }
            if(tipo=="NuevaReserva")
                return PartialView("../Reservas/Partial/BusquedaClientes", retorno);
            else 
                return PartialView("../Reservas/Partial/BusquedaClientesModificar", retorno);
        }

        [HttpGet]
        public ActionResult GetCliente(Guid id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var cliente = db.Cliente.FirstOrDefault(x => x.Id == id);

            return Json(new { Id = cliente.Id, Nombre = cliente.NombreCompleto, Telefono1 = cliente.TelefonoPrimario, Telefono2 = cliente.TelefonoSecundario }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddDomicilioTemp(DomicilioFormModel model)
        {
            if (string.IsNullOrEmpty(model.Calle))
            {
                return Json(new { Retorno = "Es obligatorio un nombre de calle", Error = true }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(model.Numero))
            {
                return Json(new { Retorno = "La numeración no puede ir vacía", Error = true }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(model.Complemento)) model.Complemento = " ";
            var lista = TempData["DomiciliosNuevos"] as List<DomicilioFormModel>;
            model.Id = Guid.NewGuid();
            if (lista == null)
                lista = new List<DomicilioFormModel>();
            lista.Add(model);
            TempData["DomiciliosNuevos"] = lista;
            return Json(new { Retorno = lista, Error = false }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteDomicilioTemp(Guid id)
        {
            var lista = TempData["DomiciliosNuevos"] as List<DomicilioFormModel>;
            var temp = lista.First(x => x.Id == id);
            lista.Remove(temp);
            TempData["DomiciliosNuevos"] = lista;
            return Json(new { Retorno = lista, Error = false }, JsonRequestBehavior.AllowGet);
        }
    }
}