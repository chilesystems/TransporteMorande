using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransporteMorande.Models.App
{
    public class ComisionFormModel
    {
        public int PrecioInicial { get; set; }
        public int PrecioFinal { get; set; }
        public int Porcentaje { get; set; }
        public Guid Id { get; set; }
    }
}