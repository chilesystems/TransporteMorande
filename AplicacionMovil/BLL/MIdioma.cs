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
    public class MIdioma
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Sigla { get; set; }

    }
}