using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TransporteMorande.Models.App
{
    public class LiquidacionesViewModel
    {
        /***PROPIEDADES DE BUSQUEDA****/
        public DateTime? busquedaDesde { get; set; }
        public DateTime? busquedaHasta { get; set; }
        public List<SelectListItem> Estados { get; set; }
        public List<SelectListItem> Vendedores { get; set; }
        public List<SelectListItem> VendedoresNuevaLiquidacion { get; set; }
        public string estadoSeleccionado { get; set; }
        public string vendedorSeleccionado { get; set; }
        public string vendedorNuevaLiquidacionSeleccionado { get; set; }
        public List<string> EstadosCombo { get; set; }

        public List<Liquidacion> Liquidaciones { get; set; }
        public NuevaLiquidacionFormModel Form { get; set; }

        public appmorandeEntities db = new appmorandeEntities();

        public LiquidacionesViewModel()
        {
            busquedaDesde = DateTime.Now.AddDays(-15);
            busquedaHasta = DateTime.Now.AddDays(15);

            Estados = new List<SelectListItem>()
            {
                new SelectListItem() { Text="Todos", Value="Todos", Selected = true },
                new SelectListItem() { Text="Pendientes", Value="Pendiente"},
                new SelectListItem() { Text="Pagadas", Value="Pagada" }
            };

            EstadosCombo = new List<string>();
            EstadosCombo.Add("Pendiente");
            EstadosCombo.Add("Pagada");


            Vendedores = new List<SelectListItem>()
            {
                new SelectListItem() { Text="Todos", Value="Todos", Selected = true }
            };
            Vendedores.AddRange(new SelectList((from s in db.AspNetUsers
                                                select new
                                                {
                                                    Id = s.Id,
                                                    NombreCompleto = s.Nombre + " " + s.Apellido
                                                }), "Id", "NombreCompleto"));

            VendedoresNuevaLiquidacion = new List<SelectListItem>();
            VendedoresNuevaLiquidacion.AddRange(new SelectList((from s in db.AspNetUsers
                                                select new
                                                {
                                                    Id = s.Id,
                                                    NombreCompleto = s.Nombre + " " + s.Apellido
                                                }), "Id", "NombreCompleto"));
        }
    }
}