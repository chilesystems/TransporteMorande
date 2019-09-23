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
    public class MServicio1
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public string Contenido { get; set; }
        public string Imagen { get; set; }
        public string NombreArchivo { get; set; }
        public string Ruta { get; set; }
        public int Precio { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string Usuario { get; set; }
        public bool Activo { get; set; }
        public bool Web { get; set; }
        public string ContenidoPortugues { get; set; }
    }
}