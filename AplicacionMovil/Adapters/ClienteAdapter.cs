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
using AplicacionMovil.BLL;

namespace AplicacionMovil.Adapters
{
    public class ClienteAdapter : BaseAdapter
    {
        public List<MCliente> _OriginalClientes { get; set; }

        Activity _activity;
        Filter filter;

        public ClienteAdapter(Activity activity, List<MCliente> clientes)
        {
            _activity = activity;
            _OriginalClientes = clientes;
        }

        public override int Count
        {
            get { return _OriginalClientes.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return position;
            //return _OriginalServicios[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.clientes_Item, parent, false);
            var nombreCliente = view.FindViewById<TextView>(Resource.Id.textClienteItemNombreCompleto);
            var correoCliente = view.FindViewById<TextView>(Resource.Id.textClienteItemEmail);
            var paisCliente = view.FindViewById<TextView>(Resource.Id.textClienteItemPais);
            var idiomaCliente = view.FindViewById<TextView>(Resource.Id.textClienteItemIdioma);
            var idCliente = view.FindViewById<TextView>(Resource.Id.textClienteIdItem);

            nombreCliente.Text = _OriginalClientes[position].NombreCompleto;
            correoCliente.Text = _OriginalClientes[position].Email;
            paisCliente.Text = _OriginalClientes[position].PaisNombre;
            idiomaCliente.Text = _OriginalClientes[position].IdiomaNombre;
            idCliente.Text = _OriginalClientes[position].Id.ToString();
            return view;
        }
    }
}