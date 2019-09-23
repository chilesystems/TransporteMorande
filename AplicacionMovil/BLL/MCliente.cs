using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SQLite;

namespace AplicacionMovil.BLL
{
    public class MCliente
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string TelefonoPrimario { get; set; }
        public string TelefonoSecundario { get; set; }
        public string Email { get; set; }
        public string NombreCompleto { get; set; }
        public Guid IdPais { get; set; }
        public Guid IdIdioma { get; set; }

        public string PaisNombre { get; set; }
        public string IdiomaNombre { get; set; }


        [JsonIgnore]
        public bool Modificado { get; set; }

        [JsonIgnore]
        public bool Nuevo { get; set; }
    }
}