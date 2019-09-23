using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class MDomicilio
    {
        public Guid Id { get; set; }
        public Guid IdCliente { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Referencia { get; set; }
        public bool Activo { get; set; }

        public string DireccionCompleta
        {
            get
            {
                return Calle + " " + Numero + " " + Complemento;
            }
        }

    }
}