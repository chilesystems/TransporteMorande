using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransporteMorande.Models.App
{
    public class NuevoClienteFormModel
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Email { get; set; }
        public string TelefonoPrimario { get; set; }
        public string TelefonoSecundario { get; set; }
        public Guid? IdPais { get; set; }
        public Guid? IdIdioma { get; set; }
    }
}