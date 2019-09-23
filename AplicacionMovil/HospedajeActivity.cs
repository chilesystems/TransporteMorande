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
using AplicacionMovil.Resources;
using Newtonsoft.Json;

namespace AplicacionMovil
{
    [Activity(Label = "Hospedajes", Theme = "@style/AppTheme.NoActionBar")]
    public class HospedajeActivity : AppCompatActivity
    {
        private string sqlPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Utilidades.NombreBD());
        List<MHospedaje> hospedajes;
        ListViewGroupedAdapter adapterHospedajes;
        EditText textBusquedaHospedaje;
        ListView listViewHospedajes;
        private FloatingActionButton fab;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.hospedaje_mantenedor);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarMantenedorHospedajes);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Title = "Hospedajes";

            listViewHospedajes = FindViewById<ListView>(Resource.Id.listHospedajes);
            listViewHospedajes.ItemClick += ListViewHospedajes_ItemClick;
            textBusquedaHospedaje = FindViewById<EditText>(Resource.Id.textBuscarHospedaje);
            textBusquedaHospedaje.TextChanged += TextBusquedaHospedaje_TextChanged;
            fab = FindViewById<FloatingActionButton>(Resource.Id.fabNuevoHospedaje);
            fab.Click += Fab_Click;

            var db = new SQLite.SQLiteConnection(sqlPath);
            hospedajes = db.Table<MHospedaje>().OrderBy(x => x.Tipo).ToList();

            var data = new ListItemCollection<ListItemValueHospedaje>();
            foreach (var hos in hospedajes)
                data.Add(new ListItemValueHospedaje(hos));

            var sortedData = data.GetSortedData();
            adapterHospedajes = CrearAdapter(sortedData);
            listViewHospedajes.Adapter = adapterHospedajes;
            listViewHospedajes.FastScrollEnabled = true;

            Window.SetSoftInputMode(SoftInput.StateHidden);
        }

        private void Fab_Click(object sender, EventArgs e)
        {
            var activityNuevoCliente = new Intent(this, typeof(HospedajeNuevoActivity));
            StartActivityForResult(activityNuevoCliente, 0);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                bool guardado = data.GetBooleanExtra("Guardado", false);
                if (guardado)
                {
                    var db = new SQLite.SQLiteConnection(sqlPath);
                    hospedajes = db.Table<MHospedaje>().OrderBy(x => x.Tipo).ToList();

                    var dataAux = new ListItemCollection<ListItemValueHospedaje>();
                    foreach (var hos in hospedajes)
                        dataAux.Add(new ListItemValueHospedaje(hos));

                    var sortedData = dataAux.GetSortedData();
                    adapterHospedajes = CrearAdapter(sortedData);
                    listViewHospedajes.Adapter = adapterHospedajes;
                }
            }
        }

        private void TextBusquedaHospedaje_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            var aux = hospedajes.Where(x => x.Nombre.ToUpper().Contains(textBusquedaHospedaje.Text.ToUpper())).ToList();

            var data = new ListItemCollection<ListItemValueHospedaje>();
            foreach (var cli in aux)
                data.Add(new ListItemValueHospedaje(cli));

            var sortedData = data.GetSortedData();
            adapterHospedajes = CrearAdapter(sortedData);
            listViewHospedajes.Adapter = adapterHospedajes;
            adapterHospedajes.NotifyDataSetChanged();
        }

        private void ListViewHospedajes_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent intent = new Intent(this, typeof(HospedajeDetalleActivity));
            var clienteAux = e.Parent.GetItemAtPosition(e.Position) as ListItemValueHospedaje;
            intent.PutExtra("hospedaje", JsonConvert.SerializeObject(clienteAux.Hospedaje));
            StartActivityForResult(intent, 1);
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