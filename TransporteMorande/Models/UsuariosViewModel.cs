using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransporteMorande.Models.App;

namespace TransporteMorande.Models
{
    public class UsuariosViewModel
    {
        public List<SelectListItem> Perfiles { get; set; }
        public List<SelectListItem> PerfilesNuevoUsuario { get; set; }
        public string NombreBusqueda { get; set; }
        public string PerfilSeleccionado { get; set; }
        public string RolId { get; set; }
        public string RolIdModificar { get; set; }
        public bool Android { get; set; }
        public List<ApplicationUser> Usuarios { get; set; }
        public List<IdentityRole> Roles { get; set; }

        public UsuariosViewModel()
        {
            Perfiles = new List<SelectListItem>()
            {
                new SelectListItem() { Text="Todos", Value=null, Selected = true }
            };
            var dbUser = new ApplicationDbContext();
            Roles = dbUser.Roles.ToList();
            Perfiles.AddRange(new SelectList(Roles, "Id", "Name"));

            PerfilesNuevoUsuario = new List<SelectListItem>()
            {
                new SelectListItem() { Text="Seleccione Perfil", Value="Seleccione", Selected = true }
            };
            PerfilesNuevoUsuario.AddRange(new SelectList(Roles, "Name", "Name"));
        }
    }
}