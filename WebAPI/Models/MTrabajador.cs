using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    public class MTrabajador
    {
        public bool Logueado { get; set; }
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string RUT { get; set; }
        public string Imagen { get; set; }
        public string tempPassword { get; set; }
    }
}