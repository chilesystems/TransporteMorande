using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransporteMorande.Models.App;

namespace TransporteMorande.Controllers
{
    public class HotelController : Controller
    {
        private appmorandeEntities db = new appmorandeEntities();
        private ClienteViewModel model = new ClienteViewModel();

        public ActionResult Index(HotelViewModel model)
        {
            List<Hotel> lista = db.Hotel.ToList();
            if (!string.IsNullOrEmpty(model.NombreSeleccionado))
                lista = lista.Where(x => x.Nombre.ToUpper().Contains(model.NombreSeleccionado.ToUpper())).ToList();
            if (!string.IsNullOrEmpty(model.DireccionSeleccionada))
                lista = lista.Where(x => x.Direccion.ToUpper().Contains(model.DireccionSeleccionada.ToUpper())).ToList();
            if (model.TipoSeleccionado != "Todos" &&  !string.IsNullOrEmpty(model.TipoSeleccionado))
                lista = lista.Where(x => x.tipo == model.TipoSeleccionado).ToList();
            model.Hoteles = lista;
            return View(model);
        }


        [HttpPost]
        public PartialViewResult Nuevo(NuevoHotelFormModel model)
        {
            if (ModelState.IsValid)
            {
                var obj = new Hotel
                {
                    Id = Guid.NewGuid(),
                    Nombre = model.Nombre,
                    Direccion = model.Direccion,
                    TelefonoPrimario = model.TelefonoPrimario,
                    TelefonoSecundario = model.TelefonoSecundario,
                    tipo = model.Tipo
                };
                db.Hotel.Add(obj);
                db.SaveChanges();
                return PartialView("../Shared/Mensaje", new Models.App.MensajeViewModel() { Mensaje = "Hotel creado exitosamente", Error = false });
            }
            else
            {
                return PartialView("../Shared/Mensaje", new Models.App.MensajeViewModel() { Mensaje = "Error de formulario. Verifique los campos.", Error = true });

            }
        }

        [HttpPost]
        public PartialViewResult Modificar(NuevoHotelFormModel model)
        {
            if (ModelState.IsValid)
            {
                var obj = db.Hotel.First(x => x.Id == model.Id);
                obj.Direccion = model.Direccion;
                obj.Nombre = model.Nombre;
                obj.TelefonoPrimario = model.TelefonoPrimario;
                obj.TelefonoSecundario = model.TelefonoSecundario;
                obj.tipo = model.Tipo;
                try
                {
                    db.Entry(obj).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return PartialView("../Shared/Mensaje", new Models.App.MensajeViewModel() { Mensaje = "Error al guardar en la base de datos: " + ex.Message, Error = true });
                }

                return PartialView("../Shared/Mensaje", new Models.App.MensajeViewModel() { Mensaje = "Hotel modificado exitosamente", Error = false });
            }
            else
            {
                return PartialView("../Shared/Mensaje", new Models.App.MensajeViewModel() { Mensaje = "Error de formulario. Verifique los campos.", Error = true });
            }
        }

        public JsonResult Obtener(string id)
        {
            var obj = db.Hotel.First(x => x.Id.ToString() == id);
            return Json(new
            {
                nombre = obj.Nombre,
                direccion = obj.Direccion,
                telefono1 = obj.TelefonoPrimario,
                telefono2 = obj.TelefonoSecundario,
                tipo = obj.tipo,
                id = obj.Id
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EliminarHotel(string id)
        {
            try
            {
                var obj = db.Hotel.First(x => x.Id.ToString() == id);
                db.Hotel.Remove(obj);
                db.SaveChanges();
                return Json("Exito", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error: " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
    }
}