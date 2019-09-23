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
using AplicacionMovil.Adapters;
using AplicacionMovil.BLL;
using Newtonsoft.Json;

namespace AplicacionMovil
{
    [Activity(Label = "Servicios", Theme = "@style/AppTheme.NoActionBar")]
    public class ServiciosActivity : AppCompatActivity
    {
        private string sqlPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Utilidades.NombreBD());
        List<MServicio1> servicios;
        SQLite.SQLiteConnection db;
        EditText textBusquedaServicio;
        ListView listViewServicios;

        public bool mostrarSnackBar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.servicios_Mantenedor);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarMantenedorServicios);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Title = "Servicios";
            mostrarSnackBar = Intent.GetStringExtra("origen") == "reserva";
            listViewServicios = FindViewById<ListView>(Resource.Id.listServicios);
            listViewServicios.ItemClick += ListViewServicios_ItemClick;
            listViewServicios.ItemLongClick += ListViewServicios_ItemLongClick;
            textBusquedaServicio = FindViewById<EditText>(Resource.Id.textBuscarServicio);
            textBusquedaServicio.TextChanged += TextBusquedaServicio_TextChanged;

            db = new SQLite.SQLiteConnection(sqlPath);
            servicios = db.Table<MServicio1>().OrderBy(x => x.Titulo).ToList();

            //ventas =  db.Table<MVenta>().OrderBy(x => x.FechaEmision).ToList();
            var ventasAdapter = new ServicioAdapter(this, servicios);
            listViewServicios.Adapter = ventasAdapter;
            listViewServicios.FastScrollEnabled = true;

            Window.SetSoftInputMode(SoftInput.StateHidden);

        }

        private void ListViewServicios_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            if (mostrarSnackBar)
            {
                Intent intent = new Intent(this, typeof(ServiciosDetalleActivity));
                string idServicio = e.View.FindViewById<TextView>(Resource.Id.textServicioIdItem).Text;
                intent.PutExtra("idServicio", idServicio);
                StartActivity(intent);
            }
        }

        private void TextBusquedaServicio_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            var aux = servicios.Where(x => x.Titulo.ToUpper().Contains(textBusquedaServicio.Text.ToUpper())).ToList();
            var ventasAdapter = new ServicioAdapter(this, aux);
            listViewServicios.Adapter = ventasAdapter;
            ventasAdapter.NotifyDataSetChanged();
        }

        private void ListViewServicios_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string idServicio = e.View.FindViewById<TextView>(Resource.Id.textServicioIdItem).Text;            
            if (mostrarSnackBar)
            {
                Guid idServicioAux = Guid.Parse(idServicio);
                MServicio1 servicioSeleccionado = db.Table<MServicio1>().First(x => x.Id == idServicioAux);
                Snackbar.Make(sender as View, servicioSeleccionado.Titulo, Snackbar.LengthIndefinite)
                   .SetAction("ACEPTAR", v =>
                   {
                       Intent myIntent = Intent;
                       myIntent.PutExtra("servicio", JsonConvert.SerializeObject(servicioSeleccionado));
                       SetResult(Result.Ok, myIntent);
                       Finish();
                   })
                   .Show();
            }
            else
            {
                Intent intent = new Intent(this, typeof(ServiciosDetalleActivity));                
                intent.PutExtra("idServicio", idServicio);
                StartActivity(intent);
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}