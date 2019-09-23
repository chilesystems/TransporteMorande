using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransporteMorande.Models.App
{
    public class DomicilioFormModel
    {
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Referencia { get; set; }
        public Guid Id { get; set; }
    }
}