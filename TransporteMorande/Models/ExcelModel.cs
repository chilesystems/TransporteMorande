using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransporteMorande.Models
{
    public class ExcelModel
    {
        public string Servicio { get; set; }
        public string Hora { get; set; }
        public string Fecha { get; set; }        
        public string Nombre { get; set; }
        public string Hotel { get; set; }
        public string Direccion { get; set; }
        public string Telefonos { get; set; }
        public int Pax { get; set; }
        public int Valor { get; set; }
        public string Vendedor { get; set; }
        public string Observaciones { get; set; }   
        
       
    }
}