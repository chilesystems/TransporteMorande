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
using SQLite;

namespace AplicacionMovil.BLL
{
    public class MTrabajador
    {
        public bool Logueado { get; set; }
        [PrimaryKey]
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string RUT { get; set; }
        public string Imagen { get; set; }
        public string tempPassword { get; set; }
    }
}