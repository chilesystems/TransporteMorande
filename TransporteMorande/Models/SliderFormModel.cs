using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransporteMorande.Models
{
    public class SliderFormModel
    {
        public Guid Id { get; set; }

        public string Titulo { get; set; }

        public HttpPostedFileBase Imagen { get; set; }

        public string Mensaje { get; set; }
    }
}