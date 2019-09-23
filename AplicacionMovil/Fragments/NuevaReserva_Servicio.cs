using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AplicacionMovil.Adapters;
using AplicacionMovil.BLL;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace AplicacionMovil.Fragments
{
    public class NuevaReserva_Servicio : SupportFragment
    {
        private string sqlPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Utilidades.NombreBD());
        List<MServicio1> servicios;
        ServicioAdapter2 adapterServicios;
        EditText textBusquedaServicios;
        ListView listViewServicios;
        //private TabLayout tabs;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var root = inflater.Inflate(Resource.Layout.reservas_SetServicio, container, false);
            var db = new SQLite.SQLiteConnection(sqlPath);
            listViewServicios = root.FindViewById<ListView>(Resource.Id.listNuevaReservaServicios2);
            listViewServicios.ItemClick += ListViewClientes_ItemClick;
            listViewServicios.ItemLongClick += ListViewClientes_ItemLongClick;
            textBusquedaServicios = root.FindViewById<EditText>(Resource.Id.textNuevaReservaBuscarServicios2);
            textBusquedaServicios.TextChanged += TextBusquedaClientes_TextChanged;
            servicios = db.Table<MServicio1>().OrderBy(x => x.Titulo).ToList();
            adapterServicios = new ServicioAdapter2(Activity, servicios);
            listViewServicios.Adapter = adapterServicios;
            listViewServicios.FastScrollEnabled = true;
            return root;
        }

        private void TextBusquedaClientes_TextChanged(object sender, TextChangedEventArgs e)
        {
            var aux = servicios.Where(x => x.Titulo.ToUpper().Contains(textBusquedaServicios.Text.ToUpper())).ToList();
            var serviciosAdapter = new ServicioAdapter2(Activity, aux);
            listViewServicios.Adapter = serviciosAdapter;
            serviciosAdapter.NotifyDataSetChanged();
        }

        private void ListViewClientes_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            Intent intent = new Intent(Activity, typeof(ServiciosDetalleActivity));
            string idServicio = e.View.FindViewById<TextView>(Resource.Id.textServicioIdItem).Text;
            intent.PutExtra("idServicio", idServicio);
            StartActivity(intent);
        }

        private void ListViewClientes_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            View anchor = sender as View;
            string idServicio = e.View.FindViewById<TextView>(Resource.Id.textServicioIdItem).Text;
            var db = new SQLite.SQLiteConnection(sqlPath);
            Guid idServicioAux = Guid.Parse(idServicio);
            var servicio = db.Table<MServicio1>().First(x => x.Id == idServicioAux);

            string textoMostrar = servicio.Titulo;
            if (textoMostrar.Length > 30) textoMostrar = textoMostrar.Substring(0, 30) + " ..";
            //textoMostrar += ". Deuda: " + Utilidades.ponerpuntos(clienteAux.Cliente.Deuda.ToString());
            Snackbar.Make(anchor, textoMostrar, Snackbar.LengthIndefinite)
            .SetAction("ACEPTAR", v =>
            {
                ((ReservasNuevoActivity)Activity).establecerServicio(servicio);
            })
            .Show();
        }
    }
}