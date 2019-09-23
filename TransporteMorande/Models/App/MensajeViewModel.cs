using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransporteMorande.Models.App
{
    public class MensajeViewModel
    {
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public bool Error { get; set; }
    }
}