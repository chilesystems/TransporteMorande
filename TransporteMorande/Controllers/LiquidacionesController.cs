using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransporteMorande.Models.App;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using TransporteMorande.Models;
using System.Data.Entity;

namespace TransporteMorande.Controllers
{
    [Authorize]
    public class LiquidacionesController : Controller
    {
        private appmorandeEntities db = new appmorandeEntities();
        private LiquidacionesViewModel model = new LiquidacionesViewModel();

        public ActionResult Index(LiquidacionesViewModel model)
        {
            string id1 = User.Identity.GetUserId().ToString();
            List<Liquidacion> lista = new List<Liquidacion>();
            if (User.IsInRole("Administrador"))
                lista = db.Liquidacion.ToList();
            else
                lista = db.Liquidacion.Where(x => x.IdVendedor == id1).ToList();
            if (model.busquedaDesde.HasValue)
                lista = lista.Where(x => x.Fecha.Date >= model.busquedaDesde.Value).ToList();
            if (model.busquedaHasta.HasValue)
                lista = lista.Where(x => x.Fecha.Date <= model.busquedaHasta.Value).ToList();

            if (model.estadoSeleccionado != "Todos" && !string.IsNullOrEmpty(model.estadoSeleccionado))
                lista = lista.Where(x => x.Estado == model.estadoSeleccionado).ToList();
            if (model.vendedorSeleccionado != "Todos" && !string.IsNullOrEmpty(model.vendedorSeleccionado))
                lista = lista.Where(x => x.IdVendedor == model.vendedorSeleccionado).ToList();

            model.Liquidaciones = lista.OrderBy(x => x.Fecha).ToList();
            return View(model);
        }

        [HttpPost]
        public JsonResult obtenerReservas(string idVendedor)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<itemReserva> items = new List<itemReserva>();
            try
            {
                var reservas = from a in db.Reserva
                               where a.IdUsuario == idVendedor && a.EstadoPagoEmpleador == "Pendiente" && a.Estado == "Finalizada"
                               select new
                               {
                                   Servicio = a.Servicio.Titulo,
                                   Id = a.Id,
                                   Total = a.Total,
                                   Fecha = a.FechaSalida
                               };

                foreach (var a in reservas)
                {
                    items.Add(new itemReserva()
                    {
                        Fecha = a.Fecha.ToString("dd-MM-yyyy"),
                        Total = a.Total.ToString("N0"),
                        Id = a.Id,
                        Servicio = a.Servicio.Substring(0, a.Servicio.Length > 25 ? 25 : a.Servicio.Length)
                    });
                }
            }
            catch (Exception ex)
            {
                string exa = ex.Message;
            }

            
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerTotal(Guid idReserva)
        {
            var reserva = db.Reserva.First(x => x.Id == idReserva);
            var servicio = db.Servicio.First(x => x.Id == reserva.IdServicio);
            var comisiones = db.Comision.Where(x => x.IdServicio == servicio.Id).ToList();
            double retorno = 0;
            foreach (var a in comisiones)
            {
                if (reserva.PrecioAdulto >= a.PrecioInicial && reserva.PrecioAdulto <= a.PrecioFinal)
                {
                    double aux = a.Porcentaje / 100.0;
                    double adultoAux = reserva.PrecioAdulto * aux * reserva.PaxAdulto;
                    double infanteAux = reserva.PrecioInfante * aux * reserva.PaxInfante;
                    retorno = adultoAux + infanteAux;
                    break;
                }
            }
            var total = Math.Round(retorno, 0);
            return Json(total, JsonRequestBehavior.AllowGet);
        }

        class itemReserva
        {
            public string Servicio { get; set; }
            public Guid Id { get; set; }
            public string Total { get; set; }
            public string Fecha { get; set; }
        }

        [HttpPost]
        public JsonResult cambiarEstado(Guid idLiquidacion, string nuevoEstado)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var liquidacion = db.Liquidacion.First(x => x.Id == idLiquidacion);
                liquidacion.Estado = nuevoEstado;
                db.Entry(liquidacion).State = EntityState.Modified;
                db.SaveChanges();
                return Json("Exito", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Eliminar(Guid id)
        {
            try
            {
                var obj = db.Liquidacion.First(x => x.Id == id);
                foreach (var a in obj.DetalleLiquidacion)
                {
                    var reserva = db.Reserva.First(x => x.Id == a.IdReserva);
                    reserva.EstadoPagoEmpleador = "Pendiente";
                    db.Entry(reserva).State = EntityState.Modified;
                }
                db.DetalleLiquidacion.RemoveRange(obj.DetalleLiquidacion);
                db.Liquidacion.Remove(obj);               
                db.SaveChanges();
                return Json("Exito", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error: " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult NuevaLiquidacion(NuevaLiquidacionFormModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // var user = UserManager.FindById(User.Identity.GetUserId());
                    ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                    Liquidacion obj = new Liquidacion()
                    {
                        Estado = model.Estado,
                        Fecha = DateTime.Now,
                        Id = Guid.NewGuid(),
                        IdUsuario = user.Id,
                        IdVendedor = model.IdVendedor,
                        Monto = int.Parse(Utilities.sacarpuntos(model.Total))
                    };
                    db.Liquidacion.Add(obj);
                    foreach (var a in model.IdReservas)
                    {
                        var reserva = db.Reserva.First(x => x.Id == a);
                        reserva.EstadoPagoEmpleador = "Pagada";
                        db.Entry(reserva).State = EntityState.Modified;
                        DetalleLiquidacion detalle = new DetalleLiquidacion()
                        {
                            Id = Guid.NewGuid(),
                            IdLiquidacion = obj.Id,
                            IdReserva = a,
                            Monto = reserva.Total
                        };
                        db.DetalleLiquidacion.Add(detalle);
                    }
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { Retorno = ex.Message + ". Stack trace: " + ex.StackTrace, Error = true }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { Retorno = "Exito", Error = false }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new { Retorno = "Contacte al administrador", Error = true }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}