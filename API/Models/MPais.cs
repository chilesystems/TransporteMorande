using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class MPais
    {
        public Guid Id { get; set; }
        public string Sigla { get; set; }
        public string Nombre { get; set; }
    }
}