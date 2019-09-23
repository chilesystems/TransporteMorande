using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class MServicio
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public string Contenido { get; set; }
        public string Imagen { get; set; }
        public string NombreArchivo { get; set; }
        public int Precio { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string Usuario { get; set; }
        public bool Activo { get; set; }
        public bool Web { get; set; }
        public string ContenidoPortugues { get; set; }

    }
}