using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TransporteMorande.Models.App
{
    public class HotelViewModel
    {
        public string NombreSeleccionado { get; set; }
        public string DireccionSeleccionada { get; set; }
        public string TipoSeleccionado { get; set; }

        public List<Hotel> Hoteles { get; set; }
        public List<SelectListItem> Tipos { get; set; }

        public List<SelectListItem> TiposNuevoHospedaje { get; set; }

        appmorandeEntities db = new appmorandeEntities();

        public HotelViewModel()
        {
            Tipos = new List<SelectListItem>()
            {
                new SelectListItem() { Text="Todos", Value="Todos", Selected = true },
                new SelectListItem() { Text="Hotel", Value="Hotel", Selected = false },
                new SelectListItem() { Text="Hostal", Value="Hostal", Selected = false },
                new SelectListItem() { Text="Residencial", Value="Residencial", Selected = false },
                new SelectListItem() { Text="Motel", Value="Motel", Selected = false },
                new SelectListItem() { Text="Otro", Value="Otro", Selected = false }
            };

            TiposNuevoHospedaje = new List<SelectListItem>()
            {
                new SelectListItem() { Text="Seleccione tipo de hospedaje", Value=null, Selected = true },
                new SelectListItem() { Text="Hotel", Value="Hotel", Selected = false },
                new SelectListItem() { Text="Hostal", Value="Hostal", Selected = false },
                new SelectListItem() { Text="Residencial", Value="Residencial", Selected = false },
                new SelectListItem() { Text="Motel", Value="Motel", Selected = false },
                new SelectListItem() { Text="Otro", Value="Otro", Selected = false }
            };
        }
    }
}