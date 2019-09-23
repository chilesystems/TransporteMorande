using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TransporteMorande.Models.App
{
    public class ReservasViewModel
    {
        public List<SelectListItem> Hospedajes { get; set; }
        public List<SelectListItem> Direcciones { get; set; }
        public List<SelectListItem> EstadoPago { get; set; }
        public List<SelectListItem> Servicios { get; set; }
        public List<SelectListItem> Comisiones { get; set; }

        /***PROPIEDADES DE BUSQUEDA****/
        public DateTime? busquedaDesde { get; set; }
        public DateTime? busquedaHasta { get; set; }
        public List<SelectListItem> Estados { get; set; }
        public List<SelectListItem> Vendedores { get; set; }
        public string estadoSeleccionado { get; set; }
        public string vendedorSeleccionado { get; set; }

        public List<Reserva> Reservas { get; set; }
        public List<string> EstadosReservas { get; set; }
        public List<string> EstadosPagoCombo { get; set; }
        public NuevaReservaFormModel Form { get; set; }

        public appmorandeEntities db = new appmorandeEntities();

        public ReservasViewModel()
        {
            busquedaDesde = DateTime.Now.AddDays(-15);
            busquedaHasta = DateTime.Now.AddDays(15);


            Direcciones = new List<SelectListItem>()
            {
                new SelectListItem() { Text="Seleccione", Value=null, Selected = true }
            };

            Hospedajes = new List<SelectListItem>()
            {
                new SelectListItem() { Text="Seleccione", Value=null, Selected = true },
                new SelectListItem() { Text="Domicilio del cliente", Value="Domicilio", Selected = false },
                new SelectListItem() { Text="Hotel", Value="Hotel", Selected = false },
                new SelectListItem() { Text="Hostal", Value="Hostal", Selected = false },
                new SelectListItem() { Text="Residencial", Value="Residencial", Selected = false },
                new SelectListItem() { Text="Motel", Value="Motel", Selected = false },
                new SelectListItem() { Text="Otro", Value="Otro", Selected = false }
            };

            EstadoPago = new List<SelectListItem>()
            {
                new SelectListItem() { Text="Pagado", Value="Pagado", Selected = true },
                new SelectListItem() { Text="Pendiente", Value="Pendiente"},
                new SelectListItem() { Text="Especial", Value="Especial" }
            };

            Estados = new List<SelectListItem>()
            {
                new SelectListItem() { Text="Todos", Value="Todos", Selected = true },
                new SelectListItem() { Text="Sin confirmar", Value="Sin confirmar"},
                new SelectListItem() { Text="Confirmadas", Value="Confirmada" },
                new SelectListItem() { Text="Finalizadas", Value="Finalizada" }
            };

            EstadosReservas = new List<string>();
            EstadosReservas.Add("Sin confirmar");
            EstadosReservas.Add("Confirmada");
            EstadosReservas.Add("Finalizada");

            EstadosPagoCombo = new List<string>();
            EstadosPagoCombo.Add("Pagado");
            EstadosPagoCombo.Add("Pendiente");
            EstadosPagoCombo.Add("Especial");
            //EstadosReservas = new List<SelectListItem>()
            //{
            //    new SelectListItem() { Text="Sin confirmar", Value="Sin confirmar"},
            //    new SelectListItem() { Text="Confirmadas", Value="Confirmada" },
            //    new SelectListItem() { Text="Finalizadas", Value="Finalizada" }
            //};

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

            Servicios = new List<SelectListItem>()
            {
                new SelectListItem() { Text="Seleccione Servicio", Value=null, Selected = true }
            };
            Servicios.AddRange(new SelectList(db.Servicio.Where(x => x.Activo), "Id", "Titulo"));



        }
    }
}