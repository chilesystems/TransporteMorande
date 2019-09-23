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
    public class MLiquidacion
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public string IdUsuario { get; set; }
        public DateTime Fecha { get; set; }
        public int Monto { get; set; }
        public string Estado { get; set; }
        public string IdVendedor { get; set; }

        public MLiquidacion()
        {
            Detalles = new List<MDetalleLiquidacion>();
        }

        [Ignore]
        public List<MDetalleLiquidacion> Detalles { get; set; }
    }
}