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
    public class MDomicilio
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public Guid? IdCliente { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Referencia { get; set; }
        public bool Activo { get; set; }

        [JsonIgnore]
        public bool Nuevo { get; set; }
        [JsonIgnore]
        public bool Confirmado { get; set; }
        [JsonIgnore]
        public string DireccionCompleta { get
            {
                return Calle + " " + Numero + " " + Complemento;
            }
        }

        public MDomicilio() { }

        public override string ToString()
        {
            return Calle + " " + Numero + " " + Complemento;
        }
    }
}