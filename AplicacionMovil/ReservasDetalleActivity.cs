using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AplicacionMovil.BLL;
using Newtonsoft.Json;

namespace AplicacionMovil
{
    [Activity(Label = "Detalle Reserva", Theme = "@style/AppTheme.NoActionBar")]
    public class ReservasDetalleActivity : AppCompatActivity
    {
        private MReserva reserva;
        private MTrabajador trabajador;
        private string sqlPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Utilidades.NombreBD());
        SQLite.SQLiteConnection db;
        private bool guardado = false;

        private TextView textNombreCliente;
        private TextView textNombreServicio;
        private TextView textFechaSalida;
        private TextView textCantidadAdultos;
        private TextView textCantidadNinos;
        private TextView textPrecioAdulto;
        private TextView textPrecioNino;
        private TextView textObservaciones;
        private TextView textTotal;
        private TextView textRetiro;
        private TextView textEstadoPago;
        private TextView textEstado;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.reservas_Detalle);
            db = new SQLite.SQLiteConnection(sqlPath);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarDetalleReserva);
            
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            reserva = JsonConvert.DeserializeObject<MReserva>(Intent.GetStringExtra("reserva"));
            trabajador = JsonConvert.DeserializeObject<MTrabajador>(Intent.GetStringExtra("trabajador"));
            SupportActionBar.Title = "Detalle de la reserva" + (reserva.Folio != 0 ? " N° " + reserva.Folio : "");

            textNombreCliente = FindViewById<TextView>(Resource.Id.textDetalleReservaResumenNombreCliente);
            textFechaSalida = FindViewById<TextView>(Resource.Id.textDetalleReservaResumenFechaSalida);
            textCantidadAdultos = FindViewById<TextView>(Resource.Id.textDetalleReservaResumenCantidadAdultos);
            textCantidadNinos = FindViewById<TextView>(Resource.Id.textDetalleReservaResumenCantidadNinos);
            textPrecioAdulto = FindViewById<TextView>(Resource.Id.textDetalleReservaResumenPrecioAdultos);
            textPrecioNino = FindViewById<TextView>(Resource.Id.textDetalleReservaResumenPrecioNinos);
            textObservaciones = FindViewById<TextView>(Resource.Id.textDetalleReservaResumenObservaciones);
            textTotal = FindViewById<TextView>(Resource.Id.textDetalleReservaResumenTotal);
            textNombreServicio = FindViewById<TextView>(Resource.Id.textDetalleReservaResumenNombreServicio);
            textRetiro = FindViewById<TextView>(Resource.Id.textDetalleReservaResumenRetiro);
            textEstadoPago = FindViewById<TextView>(Resource.Id.textDetalleReservaResumenEstadoPago);
            textEstado = FindViewById<TextView>(Resource.Id.textDetalleReservaResumenEstadoReserva);

            textNombreCliente.Text = reserva.ClienteNombre;
            textNombreServicio.Text = reserva.ServicioNombre;
            textFechaSalida.Text = reserva.FechaSalida.ToShortDateString();
            textCantidadAdultos.Text = Utilidades.ponerpuntos(reserva.PaxAdulto.ToString());
            textCantidadNinos.Text = Utilidades.ponerpuntos(reserva.PaxInfante.ToString());
            textPrecioAdulto.Text = Utilidades.ponerpuntos(reserva.PrecioAdulto.ToString());
            textPrecioNino.Text = Utilidades.ponerpuntos(reserva.PrecioInfante.ToString());
            textObservaciones.Text = reserva.Observaciones;
            textTotal.Text = Utilidades.ponerpuntos(reserva.Total.ToString());
            textEstadoPago.Text = reserva.EstadoPago;
            textRetiro.Text = reserva.RetiroNombre;
            textEstado.Text = reserva.Estado;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Intent myIntent = Intent;
                    myIntent.PutExtra("Guardado", guardado);
                    SetResult(Result.Ok, myIntent);
                    Finish();
                    break;

                case Resource.Id.menuReservaEditar:
                    if (reserva.Cerrada)
                    {
                        Toast.MakeText(this.BaseContext, "No se puede modificar una reserva finalizada", ToastLength.Long).Show();
                    }
                    else
                    {
                        var activityEdit = new Intent(this, typeof(ReservasNuevo2Activity));
                        activityEdit.PutExtra("reserva", JsonConvert.SerializeObject(reserva));
                        activityEdit.PutExtra("trabajador", JsonConvert.SerializeObject(trabajador));
                        StartActivityForResult(activityEdit, 1);
                    }
                    
                    break;
                case Resource.Id.menuReservaCancelar:
                    
                    Android.Support.V7.App.AlertDialog.Builder alertDialog = new Android.Support.V7.App.AlertDialog.Builder(this);
                    alertDialog.SetTitle("Cancelación de reserva");
                    alertDialog.SetMessage("Se cancelará esta reserva.\n\n¿Desea continuar?");
                    alertDialog.SetCancelable(true);
                    alertDialog.SetNegativeButton("Cancelar", delegate
                    {
                        alertDialog.Dispose();
                    });
                    alertDialog.SetPositiveButton("Aceptar", delegate
                    {
                        reserva.Cerrada = true;
                        textEstado.Text = "Cancelada";
                        reserva.Estado = "Cancelada";
                        db.Update(reserva);
                        guardado = true;
                        alertDialog.Dispose();
                    });
                    alertDialog.Show();
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menuDetalleReserva, menu);
            return true;
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                if (requestCode == 1) // clientes
                {
                    string reservaAux = data.GetStringExtra("reserva");
                    if (!string.IsNullOrEmpty(reservaAux))
                    {
                        reserva = JsonConvert.DeserializeObject<MReserva>(reservaAux);
                        textNombreCliente.Text = reserva.ClienteNombre;
                        textNombreServicio.Text = reserva.ServicioNombre;
                        textFechaSalida.Text = reserva.FechaSalida.ToShortDateString();
                        textCantidadAdultos.Text = Utilidades.ponerpuntos(reserva.PaxAdulto.ToString());
                        textCantidadNinos.Text = Utilidades.ponerpuntos(reserva.PaxInfante.ToString());
                        textPrecioAdulto.Text = Utilidades.ponerpuntos(reserva.PrecioAdulto.ToString());
                        textPrecioNino.Text = Utilidades.ponerpuntos(reserva.PrecioInfante.ToString());
                        textObservaciones.Text = reserva.Observaciones;
                        textTotal.Text = Utilidades.ponerpuntos(reserva.Total.ToString());
                        textEstadoPago.Text = reserva.EstadoPago;
                        textRetiro.Text = reserva.RetiroNombre;
                        textEstado.Text = reserva.Estado;
                        guardado = true;
                    }
                }
            }
        }
    }
}
