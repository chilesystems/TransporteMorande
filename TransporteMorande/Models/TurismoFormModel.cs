using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransporteMorande.Models
{
    public class TurismoFormModel
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }

        public string Contenido { get; set; }

        public HttpPostedFileBase Imagen { get; set; }

        public string PathImagen { get; set; }

        public string Mensaje { get; set; }

        public int Posicion { get; set; }

        public int Precio { get; set; }
    }
}