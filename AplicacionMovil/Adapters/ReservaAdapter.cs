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
    public class ReservaAdapter : BaseAdapter
    {
        public List<MReserva> _OriginalReservas { get; set; }

        Activity _activity;
        Filter filter;

        public ReservaAdapter(Activity activity, List<MReserva> reservas)
        {
            _activity = activity;
            _OriginalReservas = reservas;
        }

        public override int Count
        {
            get { return _OriginalReservas.Count; }
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
            var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.reservas_Item, parent, false);
            var idReserva = view.FindViewById<TextView>(Resource.Id.textReservaIdItem);
            var nombreCliente = view.FindViewById<TextView>(Resource.Id.textReservaItemNombreCliente);
            var nombreServicio = view.FindViewById<TextView>(Resource.Id.textReservaItemNombreServicio);
            var lugarRetiro = view.FindViewById<TextView>(Resource.Id.textReservaItemLugarRetiro);
            var total = view.FindViewById<TextView>(Resource.Id.textReservaItemTotal);
            var fechaRetiro = view.FindViewById<TextView>(Resource.Id.textReservaItemFecha);
            var folio = view.FindViewById<TextView>(Resource.Id.textReservaItemFolio);
            var icon = view.FindViewById<ImageView>(Resource.Id.imageEstadoReserva);
            //var descripcionServicio = view.FindViewById<TextView>(Resource.Id.textServicioItemDescripcion);

            idReserva.Text = _OriginalReservas[position].Id.ToString();
            nombreCliente.Text = _OriginalReservas[position].ClienteNombre;
            nombreServicio.Text = _OriginalReservas[position].ServicioNombre;
            lugarRetiro.Text = _OriginalReservas[position].RetiroNombre;
            total.Text = Utilidades.ponerpuntos(_OriginalReservas[position].Total.ToString());
            fechaRetiro.Text = _OriginalReservas[position].FechaSalida.ToShortDateString();
            folio.Text = _OriginalReservas[position].FolioMostrar;

            string estado = _OriginalReservas[position].Estado;
            icon.SetImageResource(estado == "Sin confirmar" ? Resource.Drawable.icon_question : (estado == "Confirmada" ? Resource.Drawable.icon_check : (estado == "Cancelada" ? Resource.Drawable.icon_reservaCancelada : Resource.Drawable.icon_doublecheck)));
            //descripcionServicio.Text = _OriginalServicios[position].Contenido;
            //folioVenta.Text = _OriginalVentas[position].CorrelativoString;
            return view;
        }
    }
}
