using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using TransporteMorande.Models;
using TransporteMorande.Models.App;

namespace TransporteMorande.Controllers
{
    [Authorize]
    public class ReservasController : Controller
    { 
        private appmorandeEntities db = new appmorandeEntities();
        private ReservasViewModel model = new ReservasViewModel();

        public ActionResult Index(ReservasViewModel model)
        {
            string id1 = User.Identity.GetUserId().ToString();
            List<Reserva> lista = new List<Reserva>();
            if (User.IsInRole("Administrador"))
                lista = db.Reserva.ToList();
            else
                lista = db.Reserva.Where(x => x.IdUsuario == id1).ToList();
            if (model.busquedaDesde.HasValue)
                lista = lista.Where(x => x.FechaSalida.Date >= model.busquedaDesde.Value).ToList();
            if (model.busquedaHasta.HasValue)
                lista = lista.Where(x => x.FechaSalida.Date <= model.busquedaHasta.Value).ToList();
            //model.Reservas = db.Reserva.Where(x => x.FechaSalida.Value.Date >= model.busquedaDesde.Date && x.FechaSalida.Value.Date <= model.busquedaHasta.Date).ToList();

            if (model.estadoSeleccionado != "Todos" && !string.IsNullOrEmpty(model.estadoSeleccionado))
                lista = lista.Where(x => x.Estado == model.estadoSeleccionado).ToList();
            if (model.vendedorSeleccionado != "Todos" && !string.IsNullOrEmpty(model.vendedorSeleccionado))
                lista = lista.Where(x => x.IdUsuario == model.vendedorSeleccionado).ToList();

            model.Reservas = lista.OrderBy(x => x.FechaSalida).ToList();
            return View(model);
        }

        [HttpGet]
        public ActionResult Excel(string desde, string hasta, string estado, string vendedor)
        {
            DateTime? desdeModel = DateTime.Parse(desde);
            DateTime? hastaModel = DateTime.Parse(hasta);
            string id1 = User.Identity.GetUserId().ToString();
            List<Reserva> lista = new List<Reserva>();
            if (User.IsInRole("Administrador"))
                lista = db.Reserva.ToList();
            else
                lista = db.Reserva.Where(x => x.IdUsuario == id1).ToList();
            if (desdeModel.HasValue)
                lista = lista.Where(x => x.FechaSalida.Date >= desdeModel.Value).ToList();
            if (hastaModel.HasValue)
                lista = lista.Where(x => x.FechaSalida.Date <= hastaModel.Value).ToList();
            //model.Reservas = db.Reserva.Where(x => x.FechaSalida.Value.Date >= model.busquedaDesde.Date && x.FechaSalida.Value.Date <= model.busquedaHasta.Date).ToList();

            if (estado != "Todos" && !string.IsNullOrEmpty(estado))
                lista = lista.Where(x => x.Estado == estado).ToList();
            if (vendedor != "Todos" && !string.IsNullOrEmpty(vendedor))
                lista = lista.Where(x => x.IdUsuario == vendedor).ToList();

            lista = lista.OrderBy(x => x.FechaSalida).ToList();

            List<ExcelModel> retornoExcel = new List<ExcelModel>();
            foreach (var reserva in lista)
            {
                retornoExcel.Add(new ExcelModel()
                {
                    Servicio = reserva.Servicio.Titulo,
                    Fecha = reserva.FechaSalida.ToShortDateString(),
                    Direccion = reserva.Retiro,
                    Hora = "",
                    Hotel = reserva.TipoRetiro,
                    Nombre = reserva.Cliente.NombreCompleto,
                    Observaciones = reserva.Observaciones,
                    Pax = reserva.PaxAdulto + reserva.PaxInfante,
                    Telefonos = reserva.Cliente.TelefonoPrimario + (string.IsNullOrEmpty(reserva.Cliente.TelefonoSecundario) ? "" : " - " + reserva.Cliente.TelefonoSecundario),
                    Valor = reserva.Total,
                    Vendedor = reserva.AspNetUsers.Nombre
                });
            }

            //List<Technology> technologies = StaticData.Technologies;
            string[] columns = { "Hora", "Fecha", "Servicio", "Nombre", "Hotel", "Direccion", "Telefonos", "Pax", "Valor", "Vendedor", "Observaciones", };
            string fileName = Server.MapPath("/") + "export\\rutas.xlsx";


            ExcelPackage excel = new ExcelPackage();

            ExcelWorksheet workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            int startRowFrom = 1;
            DataTable dt = ExcelHelper.ListToDataTable<ExcelModel>(retornoExcel);
            DataColumn dataColumn = dt.Columns.Add("#", typeof(int));
            dataColumn.SetOrdinal(0);
            int index = 1;
            foreach (DataRow item in dt.Rows)
            {
                item[0] = index;
                index++;
            }
            workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dt, true);

            // autofit width of cells with small content  
            int columnIndex = 1;
            foreach (DataColumn column in dt.Columns)
            {
                ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];
                int maxLength = columnCells.Max(cell => (cell.Value == null ? "" : cell.Value).ToString().Length);
                //int maxLength = columnCells.Max(cell => cell.Value.ToString().Count());
                if (maxLength < 150)
                {
                    workSheet.Column(columnIndex).AutoFit();
                }


                columnIndex++;
            }

            using (ExcelRange r = workSheet.Cells[startRowFrom, 1, startRowFrom, dt.Columns.Count])
            {
                r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                r.Style.Font.Bold = true;
                r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#1fb5ad"));
            }

            // format cells - add borders  
            using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 1, startRowFrom + dt.Rows.Count, dt.Columns.Count])
            {
                r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
            }
            string excelName = "reservas";
            string handle = Guid.NewGuid().ToString();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                
                excel.SaveAs(memoryStream);
                memoryStream.Position = 0;
                TempData[handle] = memoryStream.ToArray();
            }

            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = excelName + ".xlsx" },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

             //var packageExcel = ExcelHelper.ExportExcel(retornoExcel, fileName, "Reservas", true, columns);

            //packageExcel.SaveAs(new FileInfo(@"c:\workbooks\myworkbook.xlsx"));

            //return File(excel.GetAsByteArray(), ExcelHelper.ExcelContentType, "Reservas.xlsx");
            //return View("Index");
            //return File(filecontent, ExcelHelper.ExcelContentType, "Reservas.xls");

        }

        [HttpPost]
        public JsonResult Nuevo(NuevaReservaFormModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                   // var user = UserManager.FindById(User.Identity.GetUserId());
                    ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                    string estadoPago = model.EstadoPago;
                    if (User.IsInRole("Vendedor"))
                        estadoPago = "Pendiente";
                    int ultimoFolio = 1;
                    try
                    {
                        ultimoFolio = db.Reserva.Max(x => x.Folio) + 1;
                    }
                    catch { }
                    Reserva obj = new Reserva()
                    {
                        Cerrada = false,
                        Estado = "Sin confirmar",
                        EstadoPago = estadoPago,
                        EstadoPagoEmpleador = "Pendiente",
                        FechaIngreso = DateTime.Now,
                        FechaSalida = model.FechaSalida,
                        Habitacion = model.Habitacion,
                        Id = Guid.NewGuid(),
                        IdCliente = model.IdCliente,
                        IdServicio = model.IdServicio,
                        IdUsuario = user.Id,
                        Observaciones = model.Observaciones,
                        PaxAdulto = model.PaxAdulto,
                        PaxInfante = model.PaxInfante,
                        PrecioAdulto = model.PrecioAdulto,
                        PrecioInfante = model.PrecioInfante,
                        Total = model.Total,
                        Folio = ultimoFolio
                    };
                    db.Reserva.Add(obj);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new { Retorno = ex.Message + ". Stack trace: " + ex.StackTrace , Error = true }, JsonRequestBehavior.AllowGet);
                }
                
                return Json(new { Retorno = "Exito", Error = false }, JsonRequestBehavior.AllowGet);
               
            }
            else
            {
                return Json(new { Retorno = "Contacte al administrador", Error = true }, JsonRequestBehavior.AllowGet);
            }            
        }

        [HttpPost]
        public JsonResult Editar(NuevaReservaFormModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                    Reserva obj = db.Reserva.First(x => x.Id == model.Id);
                    if (User.IsInRole("Administrador"))
                        obj.EstadoPago = model.EstadoPago;
                    obj.FechaIngreso = DateTime.Now;
                    obj.FechaSalida = model.FechaSalida;
                    obj.Habitacion = model.Habitacion;
                    obj.IdCliente = model.IdCliente;
                    obj.IdServicio = model.IdServicio;
                    obj.IdUsuario = user.Id;
                    obj.Observaciones = model.Observaciones;
                    obj.PaxAdulto = model.PaxAdulto;
                    obj.PaxInfante = model.PaxInfante;
                    obj.PrecioAdulto = model.PrecioAdulto;
                    obj.PrecioInfante = model.PrecioInfante;
                    obj.Total = model.Total;
                    obj.Id = model.Id;

                    db.Entry(obj).State = EntityState.Modified;

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
        //public JsonResult buscarClientes(string term)
        //{
        //    db.Configuration.ProxyCreationEnabled = false;
        //    var busqueda = term.ToUpper();
        //    var resultado = db.Cliente.Where(x => x.NombreCompleto.ToUpper().Contains(busqueda)).Take(10).ToList();
        //    return new JsonResult { Data = resultado, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        //}

        public JsonResult Obtener(Guid id)
        {
            var obj = db.Reserva.First(x => x.Id == id);
            return Json(new
            {
                anoSalida = obj.FechaSalida.Year.ToString(),
                mesSalida = obj.FechaSalida.Month.ToString(),
                diaSalida = obj.FechaSalida.Day.ToString(),
                nombreCliente = obj.Cliente.NombreCompleto,
                idCliente = obj.IdCliente,
                telefono1 = obj.Cliente.TelefonoPrimario,
                telefono2 = obj.Cliente.TelefonoSecundario,
                nombreServicio = obj.Servicio.Titulo,
                idServicio = obj.IdServicio,
                estadoPago = obj.EstadoPago,
                paxAdulto = obj.PaxAdulto,
                paxInfante = obj.PaxInfante,
                precioAdulto = obj.PrecioAdulto,
                precioInfante = obj.PrecioInfante,
                observaciones = obj.Observaciones,
                total = obj.Total,
                id = obj.Id
                //tipoDireccion = obj.IdDomicilio.HasValue ? "Domicilio" : obj.Hotel.tipo,
                //idDireccion = obj.IdDomicilio.HasValue ? obj.IdDomicilio : obj.IdHotel
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult buscarComisionesYTickets(Guid idServicio)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var comisiones = db.Comision.Where(x => x.IdServicio == idServicio).OrderBy(x => x.Porcentaje).ToList();
            var tickets = db.Ticket.Where(x => x.IdServicio == idServicio).OrderBy(x => x.Valor).ToList();
            return Json(new { Comisiones = comisiones, Tickets = tickets }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult cambiarEstado(Guid idReserva, string nuevoEstado)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var reserva = db.Reserva.First(x => x.Id == idReserva);
                reserva.Estado = nuevoEstado;
                db.Entry(reserva).State = EntityState.Modified;
                db.SaveChanges();
                return Json("Exito", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult cambiarEstadoPago(Guid idReserva, string nuevoEstado)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var reserva = db.Reserva.First(x => x.Id == idReserva);
                reserva.EstadoPago = nuevoEstado;
                db.Entry(reserva).State = EntityState.Modified;
                db.SaveChanges();
                return Json("Exito", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult EliminarReserva(Guid id)
        {
            try
            {
                var obj = db.Reserva.First(x => x.Id == id);
                db.Reserva.Remove(obj);
                db.SaveChanges();
                return Json("Exito", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error: " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ObtenerDomicilios(string tipo, Guid idCliente)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<DomicilioAux> Domicilios = new List<DomicilioAux>();
            if (tipo != "Seleccione")
            {
                if (tipo == "Domicilio")
                    Domicilios = db.Domicilio.Where(x => x.IdCliente == idCliente).Select(x => new DomicilioAux
                    {
                        Id = x.Id,
                        Nombre = x.Calle + " " + x.Numero + " " + x.Complemento,
                        Tipo = "Domicilio"
                    }).ToList();
                else
                {
                    Domicilios = db.Hotel.Where(x => x.tipo == tipo).Select(x => new DomicilioAux
                    {
                        Id = x.Id,
                        Nombre = x.Direccion,
                        Tipo = "Hospedaje"
                    }).ToList();
                }
            }
            return Json(Domicilios, JsonRequestBehavior.AllowGet);
        }

        class DomicilioAux
        {
            public Guid Id { get; set; }
            public string Nombre { get; set; }
            public string Tipo { get; set; }
        }
    }
}