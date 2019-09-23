using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransporteMorande.Models.App
{
    public class NuevoUsuarioFormModel
    {
        public string Rut { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Password2 { get; set; }
        public bool Android { get; set; }
        public string RolId { get; set; }
        public string RolIdModificar { get; set; }
        public string Id { get; set; }
    }
}