using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransporteMorande.Models.App
{
    public class NuevaLiquidacionFormModel
    {
        public Guid Id { get; set; }
        public Guid IdUsuario { get; set; }
        public string IdVendedor { get; set; }
        public List<Guid> IdReservas { get; set; }
        public string Estado { get; set; }
        public string Total { get; set; }

        public NuevaLiquidacionFormModel()
        {
            IdReservas = new List<Guid>();
        }
    }
}