using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AplicacionMovil.Adapters;
using AplicacionMovil.BLL;
using AplicacionMovil.Resources;
using Newtonsoft.Json;

namespace AplicacionMovil
{
    [Activity(Label = "Clientes", Theme = "@style/AppTheme.NoActionBar")]
    public class ClientesActivity : AppCompatActivity
    {
        private string sqlPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Utilidades.NombreBD());
        List<MCliente> clientes;
        ListViewGroupedAdapter adapterClientes;
        EditText textBusquedaClientes;
        ListView listViewClientes;
        private FloatingActionButton fab;
        public bool mostrarSnackBar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.clientes_mantenedor);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarMantenedorClientes);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Title = "Clientes";
            mostrarSnackBar = Intent.GetStringExtra("origen") == "reserva";
            listViewClientes = FindViewById<ListView>(Resource.Id.listClientes);
            listViewClientes.ItemClick += ListViewClientes_ItemClick;
            listViewClientes.ItemLongClick += ListViewClientes_ItemLongClick;
            textBusquedaClientes = FindViewById<EditText>(Resource.Id.textBuscarClientes);
            textBusquedaClientes.TextChanged += TextBusquedaClientes_TextChanged;

            var db = new SQLite.SQLiteConnection(sqlPath);
            clientes = db.Table<MCliente>().OrderBy(x => x.Nombre).ToList();

            var data = new ListItemCollection<ListItemValueCliente>();
            foreach (var cli in clientes)
                data.Add(new ListItemValueCliente(cli));

            var sortedData = data.GetSortedData();
            adapterClientes = CrearAdapter(sortedData);
            listViewClientes.Adapter = adapterClientes;
            listViewClientes.FastScrollEnabled = true;
            fab = FindViewById<FloatingActionButton>(Resource.Id.fabNuevoCliente);
            fab.Click += Fab_Click;

            Window.SetSoftInputMode(SoftInput.StateHidden);

        }

        private void ListViewClientes_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            if (mostrarSnackBar)
            {
                Intent intent = new Intent(this, typeof(ClientesDetalleActivity));
                var clienteAux = e.Parent.GetItemAtPosition(e.Position) as ListItemValueCliente;
                intent.PutExtra("cliente", JsonConvert.SerializeObject(clienteAux.Cliente));
                StartActivityForResult(intent, 1);
            }
        }

        private void TextBusquedaClientes_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            var auxclientes = clientes.Where(x => x.NombreCompleto.ToUpper().Contains(textBusquedaClientes.Text.ToUpper())).ToList();

            var data = new ListItemCollection<ListItemValueCliente>();
            foreach (var cli in auxclientes)
                data.Add(new ListItemValueCliente(cli));

            var sortedData = data.GetSortedData();
            adapterClientes = CrearAdapter(sortedData);
            adapterClientes.NotifyDataSetChanged();
            listViewClientes.Adapter = adapterClientes;
        }

        private void Fab_Click(object sender, EventArgs e)
        {
            var activityNuevoCliente = new Intent(this, typeof(ClientesNuevoActivity));
            StartActivityForResult(activityNuevoCliente, 0);
        }

        private void ListViewClientes_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var clienteAux = e.Parent.GetItemAtPosition(e.Position) as ListItemValueCliente;
            if (mostrarSnackBar)
            {
                Snackbar.Make(sender as View, clienteAux.Cliente.NombreCompleto, Snackbar.LengthIndefinite)
                   .SetAction("ACEPTAR", v =>
                   {
                       Intent myIntent = Intent;
                       myIntent.PutExtra("cliente", JsonConvert.SerializeObject(clienteAux.Cliente));
                       SetResult(Result.Ok, myIntent);
                       Finish();
                   })
                   .Show();
            }
            else
            {
                Intent intent = new Intent(this, typeof(ClientesDetalleActivity));                
                intent.PutExtra("cliente", JsonConvert.SerializeObject(clienteAux.Cliente));
                StartActivityForResult(intent, 1);
            }
           
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                bool guardado = data.GetBooleanExtra("Guardado", false);
                MCliente clienteAux = JsonConvert.DeserializeObject<MCliente>(data.GetStringExtra("cliente"));
                if (guardado)
                {
                    var db = new SQLite.SQLiteConnection(sqlPath);
                    clientes = db.Table<MCliente>().OrderBy(x => x.Nombre).ToList();
                    var dataAux = new ListItemCollection<ListItemValueCliente>();
                    foreach (var cli in clientes)
                        dataAux.Add(new ListItemValueCliente(cli));
                    var sortedData = dataAux.GetSortedData();
                    adapterClientes = CrearAdapter(sortedData);
                    adapterClientes.NotifyDataSetChanged();
                    listViewClientes.Adapter = adapterClientes;
                    textBusquedaClientes.Text = clienteAux.NombreCompleto;
                }

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

        ListViewGroupedAdapter CrearAdapter<T>(Dictionary<string, List<T>> sortedObjects)
            where T : Resources.IHasLabel, IComparable<T>
        {
            var adapter = new ListViewGroupedAdapter(this);
            foreach (var e in sortedObjects.OrderBy(de => de.Key))
            {
                var label = e.Key;
                var section = e.Value;
                adapter.AddSection(label, new ArrayAdapter<T>(this, Resource.Layout.ListView, section));
            }
            return adapter;
        }
    }
}