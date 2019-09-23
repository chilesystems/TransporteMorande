using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using AplicacionMovil.Adapters;
using AplicacionMovil.BLL;
using AplicacionMovil.Resources;
using Newtonsoft.Json;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace AplicacionMovil.Fragments
{
    public class NuevaReserva_Cliente : SupportFragment
    {
        private string sqlPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Utilidades.NombreBD());
        List<MCliente> clientes;
        ClienteAdapter adapterClientes;
        EditText textBusquedaClientes;
        ListView listViewClientes;
       // private TabLayout tabs;


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu = true;
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var root = inflater.Inflate(Resource.Layout.reservas_SetCliente, container, false);
            var db = new SQLite.SQLiteConnection(sqlPath);
            listViewClientes = root.FindViewById<ListView>(Resource.Id.listNuevaReservaClientes);
            listViewClientes.ItemClick += ListViewClientes_ItemClick;
            listViewClientes.ItemLongClick += ListViewClientes_ItemLongClick;
            textBusquedaClientes = root.FindViewById<EditText>(Resource.Id.textNuevaReservaBuscarClientes);
            textBusquedaClientes.TextChanged += TextBusquedaClientes_TextChanged;
            clientes = db.Table<MCliente>().OrderBy(x => x.Nombre).ToList();

          //  tabs = root.FindViewById<TabLayout>(Resource.Id.tabsNuevaReserva);

            clientes = db.Table<MCliente>().OrderBy(x => x.NombreCompleto).ToList();

            //ventas =  db.Table<MVenta>().OrderBy(x => x.FechaEmision).ToList();
            var clientesAdapter = new ClienteAdapter(Activity, clientes);
            listViewClientes.Adapter = clientesAdapter;
            listViewClientes.FastScrollEnabled = true;

            return root;
        }

        private void TextBusquedaClientes_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            var aux = clientes.Where(x => x.NombreCompleto.ToUpper().Contains(textBusquedaClientes.Text.ToUpper()) || x.Email.ToUpper().Contains(textBusquedaClientes.Text.ToUpper())).ToList();
            var ventasAdapter = new ClienteAdapter(Activity, aux);
            listViewClientes.Adapter = ventasAdapter;
            ventasAdapter.NotifyDataSetChanged();
        }

        private void ListViewClientes_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            Intent intent = new Intent(Activity, typeof(ClientesDetalleActivity));
            string idCliente = e.View.FindViewById<TextView>(Resource.Id.textClienteIdItem).Text;
            var db = new SQLite.SQLiteConnection(sqlPath);
            Guid idClienteAux = Guid.Parse(idCliente);
            var cliente = db.Table<MCliente>().First(x => x.Id == idClienteAux);
            intent.PutExtra("cliente", JsonConvert.SerializeObject(cliente));
            StartActivity(intent);
        }

        private void ListViewClientes_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            View anchor = sender as View;
            string idCliente = e.View.FindViewById<TextView>(Resource.Id.textClienteIdItem).Text;
            var db = new SQLite.SQLiteConnection(sqlPath);
            Guid idClienteAux = Guid.Parse(idCliente);
            var cliente = db.Table<MCliente>().First(x => x.Id == idClienteAux);

            string textoMostrar = cliente.NombreCompleto;
            if (textoMostrar.Length > 30) textoMostrar = textoMostrar.Substring(0, 30) + " ..";
            //textoMostrar += ". Deuda: " + Utilidades.ponerpuntos(clienteAux.Cliente.Deuda.ToString());
            Snackbar.Make(anchor, textoMostrar, Snackbar.LengthIndefinite)
            .SetAction("ACEPTAR", v =>
            {
                ((ReservasNuevoActivity)Activity).establecerCliente(cliente);
            })
            .Show();
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.menu_NuevaReserva_Cliente, menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menuNuevaReservaNuevoCliente:
                    var activityNuevoCliente = new Intent(Activity, typeof(ClientesNuevoActivity));
                    StartActivityForResult(activityNuevoCliente, 3);
                    return true;
                default:
                    return false;
            }
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 3 && resultCode == (int)Android.App.Result.Ok)
            {
                var db = new SQLite.SQLiteConnection(sqlPath);
                clientes = db.Table<MCliente>().OrderBy(x => x.NombreCompleto).ToList();
                var clientesAdapter = new ClienteAdapter(Activity, clientes);
                listViewClientes.Adapter = clientesAdapter;
                listViewClientes.FastScrollEnabled = true;
            }
        }
    }
}