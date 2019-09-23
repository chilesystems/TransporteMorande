using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransporteMorande.Models.App
{
    public class NuevoHotelFormModel
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string TelefonoPrimario { get; set; }
        public string TelefonoSecundario { get; set; }
        public string Tipo { get; set; }
    }
}