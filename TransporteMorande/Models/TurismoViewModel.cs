using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransporteMorande.Models.App;

namespace TransporteMorande.Models
{
    public class TurismoViewModel       
    {
        public string NombreSeleccionado { get; set; }
        public List<Servicio> Publicaciones { get; set; }
        public List<SelectListItem> Comisiones { get; set; }
        public List<SelectListItem> Tickets { get; set; }

        appmorandeEntities db = new appmorandeEntities();

        public TurismoViewModel()
        {
            Comisiones = new List<SelectListItem>()
            {
                new SelectListItem() { Text="Debe agregar al menos una comisión", Value="Seleccione", Selected = true },
            };
            //Comisiones.AddRange(new SelectList(db.Comision.Select(x => new { Id = x.Id, Nombre = (x.PrecioInicial + " - " + x.PrecioFinal + " " + x.Nombre) })
            //    .OrderBy(x => x.Id), "Id", "Nombre"));

            Tickets = new List<SelectListItem>()
            {
                new SelectListItem() { Text="No es obligatorio el ingreso de tickets", Value="Seleccione", Selected = true },
            };
            //Tickets.AddRange(new SelectList(db.Ticket
            //    .Select(x => new { Id = x.Id, Nombre = x.Valor.ToString() })
            //    .OrderBy(x => x.Id), "Id", "Nombre"));
        }
    }
}