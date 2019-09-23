using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class MDetalleLiquidacion
    {
        public Guid Id { get; set; }
        public Guid IdLiquidacion { get; set; }
        public Guid IdReserva { get; set; }
        public int Monto { get; set; }
    }
}