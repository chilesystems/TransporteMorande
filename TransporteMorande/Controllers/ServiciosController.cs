using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransporteMorande.Models;
using System.Text.RegularExpressions;
using TransporteMorande.Models.App;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Data.Entity;

namespace TransporteMorande.Controllers
{
    public class ServiciosController : Controller
    {
        appmorandeEntities db = new appmorandeEntities();
        TurismoViewModel model = new TurismoViewModel();

        CloudStorageAccount storageAccount = null;
        CloudBlobContainer cloudBlobContainer = null;
        string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=transportemorandeapp;AccountKey=1zTOYuNzSnkTTK/Qm0CBhC4GZyFSQQjKpJcZaAjUIKWHmYzoSu9sFbr8jRToZvDC9A+TppyvICdENfjF/BSv3g==;EndpointSuffix=core.windows.net";
        //string storageConnectionString = "UseDevelopmentStorage=true";
        public ActionResult Index(TurismoViewModel model)
        {
            model.Publicaciones = db.Servicio.ToList();

            List<Servicio> lista = db.Servicio.Where(x => x.Activo).ToList();
            if (!string.IsNullOrEmpty(model.NombreSeleccionado))
                lista = lista.Where(x => x.Titulo.ToUpper().Contains(model.NombreSeleccionado.ToUpper())).ToList();
            model.Publicaciones = lista;
            TempData["ComisionesNuevas"] = new List<ComisionFormModel>();
            TempData["TicketsNuevos"] = new List<TicketFormModel>();
            return View(model);
        }

        [HttpPost]
        public JsonResult Modificar(ServicioFormModel Form)
        {
            Servicio aux = db.Servicio.First(x => x.Id == Form.Id);
            try
            {
                var listaComisiones = TempData["ComisionesNuevas"] as List<ComisionFormModel>;
                if (listaComisiones.Count == 0)
                {
                    return Json(new { Retorno = "Debe ingresar al menos una comisión", Error = true }, JsonRequestBehavior.AllowGet);
                }

                aux.Titulo = Form.Nombre;
                aux.Precio = Form.Precio;
                //aux.ValorTicket = Form.ValorTicket;
                aux.Web = Form.SitioWeb;
                aux.Contenido = string.IsNullOrEmpty(Form.Contenido) ? string.Empty : Form.Contenido;
                aux.Usuario = User.Identity.Name;
                aux.FechaIngreso = DateTime.Now;
                db.Entry(aux).State = EntityState.Modified;

                db.Comision.RemoveRange(aux.Comision);
                db.Ticket.RemoveRange(aux.Ticket);

                
                foreach (var a in listaComisiones)
                    db.Comision.Add(new Comision()
                    {
                        Id = a.Id,
                        IdServicio = aux.Id,
                        Nombre = "Desde: $" + a.PrecioInicial.ToString("N0") + " - Hasta: $" + a.PrecioFinal.ToString("N0") + ". Comisión: " + a.Porcentaje + "%",
                        Porcentaje = a.Porcentaje,
                        PrecioFinal = a.PrecioFinal,
                        PrecioInicial = a.PrecioInicial
                    });


                var listaTickets = TempData["TicketsNuevos"] as List<TicketFormModel>;
                foreach (var a in listaTickets)
                    db.Ticket.Add(new Ticket()
                    {
                        Id = a.Id,
                        IdServicio = aux.Id,
                        Nombre = a.Nombre,
                        Valor = a.Valor
                    });

                db.SaveChanges();
                return Json(new { Retorno = aux.Id, Error = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Retorno = "Error. " + ex.Message, Error = true }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Nuevo(ServicioFormModel Form)
        {
            try
            {
                Servicio aux = new Servicio()
                {
                    Contenido = string.IsNullOrEmpty(Form.Contenido) ? string.Empty : Form.Contenido,
                    FechaIngreso = DateTime.Now,
                    Precio = Form.Precio,
                    Titulo = Form.Nombre,
                    Usuario = User.Identity.Name,
                    Id = Guid.NewGuid(),
                    Activo = true,
                    Web = Form.SitioWeb
                };
                db.Servicio.Add(aux);
                var listaComisiones = TempData["ComisionesNuevas"] as List<ComisionFormModel>;
                if (listaComisiones.Count == 0)
                {
                    return Json(new { Retorno = "Debe ingresar al menos una comisión", Error = true }, JsonRequestBehavior.AllowGet);
                }
                foreach (var a in listaComisiones)
                    db.Comision.Add(new Comision()
                    {
                        Id = a.Id,
                        IdServicio = aux.Id,
                        Nombre = "Desde: $" + a.PrecioInicial.ToString("N0") + " - Hasta: $" + a.PrecioFinal.ToString("N0") + ". Comisión: " + a.Porcentaje + "%",
                        Porcentaje = a.Porcentaje,
                        PrecioFinal = a.PrecioFinal,
                        PrecioInicial = a.PrecioInicial
                    });


                var listaTickets = TempData["TicketsNuevos"] as List<TicketFormModel>;
                foreach (var a in listaTickets)
                    db.Ticket.Add(new Ticket()
                    {
                        Id = a.Id,
                        IdServicio = aux.Id,
                        Nombre = a.Nombre,
                        Valor = a.Valor
                    });

                db.SaveChanges();
                return Json(new { Retorno = aux.Id, Error = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Retorno = ex.Message, Error = true }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Eliminar(Guid id)
        {
            Servicio aux = db.Servicio.First(x => x.Id == id);
            aux.Activo = false;
            db.Entry(aux).State = EntityState.Modified;
            db.SaveChanges();
            try
            {
                Uri uri = new Uri(aux.Imagen);
                string filename = Path.GetFileName(uri.LocalPath);
                var blob = ObtenerConexionAzureStorage().GetBlockBlobReference(filename);
                blob.DeleteIfExists();
                // return Json("Exito");
            }
            catch (Exception ex)
            {
                ViewData["message"] = ex.Message;
                ViewData["trace"] = ex.StackTrace;
                //return Json("Error: " + ex.Message + " " + ex.StackTrace);
            }
            return Json("Exito");
        }

        [HttpPost]
        public PartialViewResult SubirImagen(Guid id, string tipo)
        {
            string mensaje = string.Empty;
            var servicio = db.Servicio.First(x => x.Id == id);
            if (tipo == "Modificacion")
            {
                mensaje = "Servicio modificado correctamente";
                if (!string.IsNullOrEmpty(servicio.Imagen))
                {
                    try
                    {
                        Uri uri = new Uri(servicio.Imagen);
                        string filename = Path.GetFileName(uri.LocalPath);
                        var blob = ObtenerConexionAzureStorage().GetBlockBlobReference(filename);
                        blob.DeleteIfExists();
                    }
                    catch { }
                }

            }
            else if (tipo == "Creacion") mensaje = "Servicio creado correctamente";
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                try
                {
                    var uri = ProcesarBlob(file);
                    servicio.Imagen = uri;
                    db.Entry(servicio).State = EntityState.Modified;
                    db.SaveChanges();
                    return PartialView("../Shared/Mensaje", new Models.App.MensajeViewModel() { Mensaje = mensaje, Error = false });

                }
                catch (Exception ex)
                {
                    return PartialView("../Shared/Mensaje", new Models.App.MensajeViewModel() { Mensaje = "Error " + ex.StackTrace, Error = true });
                }
            }
            return PartialView("../Shared/Mensaje", new Models.App.MensajeViewModel() { Mensaje = mensaje, Error = false });
        }

        private string ProcesarBlob(HttpPostedFileBase archivo)
        {
            CloudBlockBlob cloudBlockBlob = ObtenerConexionAzureStorage().GetBlockBlobReference(archivo.FileName);
            cloudBlockBlob.UploadFromStream(archivo.InputStream);
            return cloudBlockBlob.Uri.ToString();
        }

        private CloudBlobContainer ObtenerConexionAzureStorage()
        {
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                cloudBlobContainer = cloudBlobClient.GetContainerReference("imagenesturismo");
                BlobContainerPermissions permissions = new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                };
                cloudBlobContainer.SetPermissions(permissions);
                return cloudBlobContainer;
            }
            return null;
        }

        public JsonResult Obtener(Guid id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var servicio = db.Servicio.First(x => x.Id == id);
            var comisionesAux = db.Comision.Where(x => x.IdServicio == id).ToList();
            var ticketsAux = db.Ticket.Where(x => x.IdServicio == id).ToList();

            var listaComisiones = new List<ComisionFormModel>();
            foreach (var a in comisionesAux)
                listaComisiones.Add(new ComisionFormModel() { Id = a.Id, Porcentaje = a.Porcentaje, PrecioFinal = a.PrecioFinal, PrecioInicial = a.PrecioInicial });
            TempData["ComisionesNuevas"] = listaComisiones;

            var listaTickets = new List<TicketFormModel>();
            foreach (var a in ticketsAux)
                listaTickets.Add(new TicketFormModel() { Id = a.Id, Nombre = a.Nombre, Valor = a.Valor });
            TempData["TicketsNuevos"] = listaTickets;

            return Json(new
            {
                nombre = servicio.Titulo,
                precioCliente = servicio.Precio,
                comisiones = listaComisiones,
                web = servicio.Web,
                contenido = servicio.Contenido,
                imagen = servicio.Imagen,
                tickets = listaTickets
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddComisionTemp(ComisionFormModel model)
        {
            if (model.Porcentaje == 0)
            {
                return Json(new { Retorno = "El porcentaje no puede ser cero.", Error = true }, JsonRequestBehavior.AllowGet);
            }
            if (model.PrecioInicial == 0)
            {
                return Json(new { Retorno = "El porcentaje no puede ser cero.", Error = true }, JsonRequestBehavior.AllowGet);
            }

            if (model.PrecioFinal == 0 || model.PrecioFinal <= model.PrecioInicial)
            {
                return Json(new { Mensaje = "El precio final no puede ser cero o menor o igual que el precio inicial.", Error = true }, JsonRequestBehavior.AllowGet);
            }
            var lista = TempData["ComisionesNuevas"] as List<ComisionFormModel>;
            model.Id = Guid.NewGuid();
            if (lista == null)
                lista = new List<ComisionFormModel>();
            lista.Add(model);
            TempData["ComisionesNuevas"] = lista;
            return Json(new { Retorno = lista, Error = false }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteComisionTemp(Guid id)
        {
            var lista = TempData["ComisionesNuevas"] as List<ComisionFormModel>;
            var temp = lista.First(x => x.Id == id);
            lista.Remove(temp);
            TempData["ComisionesNuevas"] = lista;
            return Json(new { Retorno = lista, Error = false }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddTicketTemp(TicketFormModel model)
        {
            if (model.Valor == 0)
            {
                return Json(new { Retorno = "El valor del ticket no puede ser cero.", Error = true }, JsonRequestBehavior.AllowGet);
            }
            model.Id = Guid.NewGuid();
            var lista = TempData["TicketsNuevos"] as List<TicketFormModel>;
            lista.Add(model);
            if (lista == null)
                lista = new List<TicketFormModel>();
            TempData["TicketsNuevos"] = lista;
            return Json(new { Retorno = lista, Error = false }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteTicketTemp(Guid id)
        {
            var lista = TempData["TicketsNuevos"] as List<TicketFormModel>;
            var temp = lista.First(x => x.Id == id);
            lista.Remove(temp);
            TempData["TicketsNuevos"] = lista;
            return Json(new { Retorno = lista, Error = false }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ResetTemps()
        {
            TempData["ComisionesNuevas"] = new List<ComisionFormModel>();
            TempData["TicketsNuevos"] = new List<TicketFormModel>();
            return Json("Ok", JsonRequestBehavior.AllowGet);
        }
    }
}