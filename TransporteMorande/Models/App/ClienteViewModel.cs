using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TransporteMorande.Models.App
{
    public class ClienteViewModel
    {
        public List<SelectListItem> Paises { get; set; }
        public List<SelectListItem> Idiomas { get; set; }

        public List<SelectListItem> PaisesNuevoCliente { get; set; }
        public List<SelectListItem> IdiomasNuevoCliente { get; set; }

        public string NombreSeleccionado { get; set; }
        public string EmailSeleccionado { get; set; }
        public Guid? PaisSeleccionado { get; set; }
        public Guid? IdiomaSeleccionado { get; set; }
        public List<Cliente> Clientes { get; set; }

        public List<SelectListItem> Domicilios { get; set; }
        appmorandeEntities db = new appmorandeEntities();

        public ClienteViewModel()
        {
            Domicilios = new List<SelectListItem>()
            {
                new SelectListItem() { Text="Domicilios del cliente", Value=null, Selected = true },
            };
            //Domicilios.AddRange(new SelectList(db.Domicilio.Select(x => new { Id = x.Id, Nombre = x.Calle + " " + x.Numero + " " + x.Complemento }).OrderBy(x => x.Id), "Id", "Nombre")); 
            Paises = new List<SelectListItem>()
            {
                new SelectListItem() { Text="Todos", Value=null, Selected = true }
            };
            //Paises.AddRange(new SelectList(db.Pais.OrderBy(x => x.Nombre), "Id", "Nombre"));

            Idiomas = new List<SelectListItem>()
            {
                new SelectListItem() { Text="Todos", Value=null, Selected = true }
            };
           // Idiomas.AddRange(new SelectList(db.Idioma.OrderBy(x => x.Nombre), "Id", "Nombre"));

            //var paisChile = db.Pais.First(x => x.Sigla == "CL");
            PaisesNuevoCliente = new List<SelectListItem>()
            {
                //new SelectListItem() { Text=paisChile.Nombre, Value= paisChile.Id.ToString(), Selected = true },
                new SelectListItem() { Text="Seleccione País", Value=null }
            };
            //PaisesNuevoCliente.AddRange(new SelectList(db.Pais.Where(x => x.Sigla != "CL").OrderBy(x => x.Nombre), "Id", "Nombre"));

           // var idiomaEspanol = db.Idioma.First(x => x.Sigla == "ES");
            IdiomasNuevoCliente = new List<SelectListItem>()
            {
               // new SelectListItem() { Text=idiomaEspanol.Nombre, Value=idiomaEspanol.Id.ToString(), Selected = true },
                new SelectListItem() { Text="Seleccione Idioma", Value=null }
            };
            //IdiomasNuevoCliente.AddRange(new SelectList(db.Idioma.Where(x => x.Sigla != "ES").OrderBy(x => x.Nombre), "Id", "Nombre"));
        }
    }
}