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
    public class MHospedaje
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string TelefonoPrimario { get; set; }
        public string TelefonoSecundario { get; set; }
        public string Tipo { get; set; }

        [JsonIgnore]
        public bool Nuevo { get; set; }

        public override string ToString()
        {
            return Nombre;
        }
    }
}