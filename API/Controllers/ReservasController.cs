using API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    [RoutePrefix("api/Reservas")]
    public class ReservasController : ApiController
    {
        private appmorandeEntities db = new appmorandeEntities();

        [HttpPost]
        [Route("Actualizar")]
        public List<MReserva> ActualizarReservas(parametrosReservas p)
        {
            Error _error = new Error("API Morandé");
            Evento _evento = new Evento("API Morandé", "ReservasController", "Actualizar");
            _evento.InsertarDetalle("Ingreso al método", p.idUsuario + "\n" + p.modificados + "\n" + p.nuevos);

            List<MReserva> lista = new List<MReserva>();

            List<MReserva> reservasNuevas = JsonConvert.DeserializeObject<List<MReserva>>(p.nuevos);
            //List<MReserva> reservasNuevas = p.nuevos;
            try
            {
                foreach (var a in reservasNuevas)
                {
                    int folio = 1;
                    try
                    {
                        folio = db.Reserva.Max(x => x.Folio) + 1;
                    }
                    catch { }
                    db.Reserva.Add(new Reserva()
                    {
                        Cerrada = a.Cerrada,
                        Estado = a.Estado,
                        EstadoPago = a.EstadoPago,
                        EstadoPagoEmpleador = a.EstadoPagoEmpleador,
                        FechaIngreso = a.FechaIngreso,
                        FechaSalida = a.FechaSalida,
                        Habitacion = a.Habitacion,
                        Id = Guid.NewGuid(),
                        IdCliente = a.IdCliente,
                        IdServicio = a.IdServicio,
                        IdUsuario = a.IdUsuario,
                        Observaciones = a.Observaciones,
                        PaxAdulto = a.PaxAdulto,
                        PaxInfante = a.PaxInfante,
                        PrecioAdulto = a.PrecioAdulto,
                        PrecioInfante = a.PrecioInfante,
                        Total = a.Total,
                        Folio = folio,
                        Retiro = a.RetiroNombre,
                        TipoRetiro = a.TipoRetiro
                    });
                    db.SaveChanges();
                }
            }            
            catch(Exception ex) { _error.InsertarError(ex.Message, ex.StackTrace); }
            _evento.InsertarDetalle("Se insertaron las nuevas", "");
            List<MReserva> reservasModificadas = JsonConvert.DeserializeObject<List<MReserva>>(p.modificados);
            //List<MReserva> reservasModificadas = p.modificados;
            try
            {
                foreach (var a in reservasModificadas)
                {
                    var reservaAux = db.Reserva.First(x => x.Id == a.Id);
                    reservaAux.Estado = a.Estado;
                    reservaAux.FechaIngreso = a.FechaIngreso;
                    reservaAux.FechaSalida = a.FechaSalida;
                    reservaAux.IdCliente = a.IdCliente;
                    reservaAux.IdServicio = a.IdServicio;
                    reservaAux.Observaciones = a.Observaciones;
                    reservaAux.PaxAdulto = a.PaxAdulto;
                    reservaAux.PaxInfante = a.PaxInfante;
                    reservaAux.PrecioAdulto = a.PrecioAdulto;
                    reservaAux.PrecioInfante = a.PrecioInfante;
                    reservaAux.Total = a.Total;
                    reservaAux.Retiro = a.RetiroNombre;
                    reservaAux.TipoRetiro = a.TipoRetiro;
                    db.Entry(reservaAux).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception ex) { _error.InsertarError(ex.Message, ex.StackTrace); }
            _evento.InsertarDetalle("Se insertaron las modificadas", "");

            DateTime dateFechaLimite = DateTime.Now.AddMonths(-2);
             try
            {

                db.Configuration.ProxyCreationEnabled = false;
                var reservas = db.Reserva.Where(x => x.IdUsuario == p.idUsuario && DbFunctions.TruncateTime(x.FechaIngreso) >= dateFechaLimite.Date);

                _evento.InsertarDetalle("Se recuperan " + reservas.Count() + " reservas", JsonConvert.SerializeObject(reservas));

                foreach (var reserva in reservas)
                {
                    //if (reserva.IdHotel.HasValue) reserva.Hotel = db.Hotel.First(x => x.Id == reserva.IdHotel);
                    //if (reserva.IdDomicilio.HasValue) reserva.Domicilio = db.Domicilio.First(x => x.Id == reserva.IdDomicilio);
                    reserva.Cliente = db.Cliente.First(x => x.Id == reserva.IdCliente);
                    reserva.Servicio = db.Servicio.First(x => x.Id == reserva.IdServicio);
                    MReserva aux = new MReserva()
                    {
                        Estado = reserva.Estado,
                        EstadoPago = reserva.EstadoPago,
                        EstadoPagoEmpleador = reserva.EstadoPagoEmpleador,
                        FechaIngreso = reserva.FechaIngreso,
                        FechaSalida = reserva.FechaSalida,
                        Folio = reserva.Folio,
                        Habitacion = reserva.Habitacion,
                        Id = reserva.Id,
                        IdCliente = reserva.IdCliente,
                        IdServicio = reserva.IdServicio,
                        IdUsuario = reserva.IdUsuario,
                        Observaciones = reserva.Observaciones,
                        PaxAdulto = reserva.PaxAdulto,
                        PaxInfante = reserva.PaxInfante,
                        PrecioAdulto = reserva.PrecioAdulto,
                        PrecioInfante = reserva.PrecioInfante,
                        Total = reserva.Total,
                        Cerrada = reserva.Cerrada,
                        ClienteNombre = reserva.Cliente.NombreCompleto,
                        //RetiroNombre = reserva.IdDomicilio.HasValue ? reserva.Domicilio.Calle + " " + reserva.Domicilio.Numero + " " + reserva.Domicilio.Complemento : (reserva.Hotel.tipo + " - " + reserva.Hotel.Direccion),
                        RetiroNombre = reserva.Retiro,
                        ServicioNombre = reserva.Servicio.Titulo,
                        TipoRetiro = reserva.TipoRetiro
                    };
                    lista.Add(aux);

                }
            }
            catch (Exception ex) { _error.InsertarError(ex.Message, ex.StackTrace); }
            _evento.InsertarDetalle("Se retornan " + lista.Count + " reservas", JsonConvert.SerializeObject(lista));


            // catch (Exception ex)
            //{
            /*MReserva aux = new MReserva()
            {
                Estado = ex.Message,
                EstadoPago = ex.StackTrace,
                EstadoPagoEmpleador = "ERROR",
                Cerrada = false,
                ClienteNombre = "",
                FechaIngreso = DateTime.Now,
                FechaSalida = DateTime.Now,
                Folio = -1,
                Habitacion = "",
                Id = Guid.NewGuid(),
                IdCliente = Guid.NewGuid(),
                IdDomicilio = Guid.NewGuid(),
                IdHotel = Guid.NewGuid(),
                IdServicio = Guid.NewGuid(),
                IdUsuario = "",
                Observaciones = "",
                PaxAdulto = 0,
                PaxInfante = 0,
                PrecioAdulto = 0,
                PrecioInfante = 0,
                RetiroNombre = "",
                ServicioNombre = "",
                Total = 0

            };*/
            //lista.Add(aux);
            //throw new Exception(ex.InnerException.ToString() + "\n" + ex.StackTrace + "\n" + ex.Message);

            ///}
            return lista;
        }

        public class parametrosReservas
        {
            //public List<MReserva> nuevos { get; set; }
            //public List<MReserva> modificados { get; set; }
            public string nuevos { get; set; }
            public string modificados { get; set; }
            public string idUsuario { get; set; }
        }
    }
}
