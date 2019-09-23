using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransporteMorande.Models.App
{
    public class NuevaReservaFormModel
    {
        public DateTime FechaSalida { get; set; }
        public Guid IdCliente { get; set; }
        public string Habitacion { get; set; }
        public int PaxAdulto { get; set; }
        public int PrecioAdulto { get; set; }
        public int PaxInfante { get; set; }
        public int PrecioInfante { get; set; }
        public int Total { get; set; }
        public string Observaciones { get; set; }
        public string EstadoPago { get; set; }
        public Guid IdServicio { get; set; }
        public Guid IdDireccion { get; set; }
        public string TipoDireccion { get; set; }
        public Guid Id { get; set; }
    }
}