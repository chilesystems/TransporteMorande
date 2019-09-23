using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransporteMorande.Models.App
{
    public class ServicioFormModel
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Contenido { get; set; }
        public HttpPostedFileBase Imagen { get; set; }
        public string PathImagen { get; set; }
        public int Precio { get; set; }
        public bool SitioWeb { get; set; }

    }
}