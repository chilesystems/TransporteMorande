using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransporteMorande.Models.App
{
    public class PerfilViewModel
    {
        public ApplicationUser Usuario { get; set; }
        public string Rol { get; set; }
        public List<Reserva> ViajesRealizados { get; set; }
        public List<Reserva> Reservas { get; set; }
        public List<Liquidacion> Liquidaciones { get; set; }
        public int ReservasPendientes { get; set; }
        public int ReservasConfirmadas { get; set; }
        public int TotalPendienteReservas { get; set; }
        public int TotalLiquidacionesPendientes { get; set; }
        public int CantidadLiquidacionesPendientes { get; set; }
        public PerfilViewModel()
        {
            ViajesRealizados = new List<Reserva>();
            Reservas = new List<Reserva>();
            Liquidaciones = new List<Liquidacion>();
        }
        
    }
}