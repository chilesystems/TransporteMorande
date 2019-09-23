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
    public class ServicioAdapter : BaseAdapter
    {
        public List<MServicio1> _OriginalServicios { get; set; }

        Activity _activity;
        Filter filter;

        public ServicioAdapter(Activity activity, List<MServicio1> servicios)
        {
            _activity = activity;
            _OriginalServicios = servicios;
        }

        public override int Count
        {
            get { return _OriginalServicios.Count; }
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
            var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.servicioItem, parent, false);
            var nombreCliente = view.FindViewById<TextView>(Resource.Id.textServicioItemNombre);
            var precioServicio = view.FindViewById<TextView>(Resource.Id.textServicioItemPrecio);
            var idServicio = view.FindViewById<TextView>(Resource.Id.textServicioIdItem);
            var imagenServicio = view.FindViewById<ImageView>(Resource.Id.imagenServicioItem);
            //var descripcionServicio = view.FindViewById<TextView>(Resource.Id.textServicioItemDescripcion);

            nombreCliente.Text = _OriginalServicios[position].Titulo;
            precioServicio.Text = _OriginalServicios[position].Precio.ToString("N0");
            idServicio.Text = _OriginalServicios[position].Id.ToString();
            try
            { imagenServicio.SetImageURI(Android.Net.Uri.Parse(_OriginalServicios[position].Ruta));
            }
            catch {
                imagenServicio.SetImageResource(Resource.Drawable.icon_question);
            }
            
            //descripcionServicio.Text = _OriginalServicios[position].Contenido;
            //folioVenta.Text = _OriginalVentas[position].CorrelativoString;
            return view;
        }
    }
}