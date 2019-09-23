using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class MLiquidacion
    {
        public Guid Id { get; set; }
        public string IdUsuario { get; set; }
        public DateTime Fecha { get; set; }
        public int Monto { get; set; }
        public string Estado { get; set; }
        public string IdVendedor { get; set; }

        public List<MDetalleLiquidacion> Detalles { get; set; }
    }
}