using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AplicacionMovil.BLL;

namespace AplicacionMovil
{
    [Activity(Label = "Detalle Cliente", Theme = "@style/AppTheme.NoActionBar")]
    public class ServiciosDetalleActivity : AppCompatActivity
    {
        private string sqlPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Utilidades.NombreBD());
        MServicio1 servicio;
        TextView textDescripcion;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.servicios_Detalle);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarServiciosDetalle);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var textTitulo = FindViewById<TextView>(Resource.Id.textServicioDetalleTitulo);
            CollapsingToolbarLayout collapsingToolBar = FindViewById<CollapsingToolbarLayout>(Resource.Id.collapsinToolbarServiciosDetalle);
            ImageView imageView = FindViewById<ImageView>(Resource.Id.imageServicioDetalle);
            textDescripcion = FindViewById<TextView>(Resource.Id.textServicioDetalleDescripcion);
            Guid idServicio = Guid.Parse(Intent.GetStringExtra("idServicio"));
            var db = new SQLite.SQLiteConnection(sqlPath);
            servicio = db.Table<MServicio1>().First(x => x.Id == idServicio);

            collapsingToolBar.Title = servicio.Titulo;
            textTitulo.Text = servicio.Titulo;
            imageView.SetImageURI(Android.Net.Uri.Parse(servicio.Ruta));
            textDescripcion.Text = servicio.Contenido;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    break;
                case Resource.Id.menuServicioDetallePortugues:
                    textDescripcion.Text = servicio.ContenidoPortugues;
                    break;
                case Resource.Id.menuServicioDetalleEspanol:
                    textDescripcion.Text = servicio.Contenido;
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menuDetalleServicio, menu);
            return true;
        }

        private void LoadBackDrop()
        {
            //ImageView imageView = FindViewById<ImageView>(Resource.Id.backdrop);
            //imageView.SetImageResource(Cheeses.RandomCheeseDrawable);
        }
    }
}
 