using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class MReserva
    {
        public Guid Id { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string IdUsuario { get; set; }
        public DateTime FechaSalida { get; set; }
        public Guid IdCliente { get; set; }
        public int PaxAdulto { get; set; }
        public int PrecioAdulto { get; set; }
        public int PaxInfante { get; set; }
        public int PrecioInfante { get; set; }
        public int Total { get; set; }
        public string Observaciones { get; set; }
        public string EstadoPago { get; set; }
        public string Estado { get; set; }
        public Guid IdServicio { get; set; }
        public bool Cerrada { get; set; }
        public string Habitacion { get; set; }
        //public Guid? IdHotel { get; set; }
        //public Guid? IdDomicilio { get; set; }
        public string EstadoPagoEmpleador { get; set; }
        public int Folio { get; set; }

        public string ClienteNombre { get; set; }
        public string RetiroNombre { get; set; }
        public string ServicioNombre { get; set; }

        public string TipoRetiro { get; set; }
    }
}