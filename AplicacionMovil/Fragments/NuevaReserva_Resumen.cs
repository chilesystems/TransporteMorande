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
using AplicacionMovil.BLL;
using Newtonsoft.Json;
using static Android.App.DatePickerDialog;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace AplicacionMovil.Fragments
{
    public class NuevaReserva_Resumen : SupportFragment
    {
        private SQLite.SQLiteConnection db;
        private string sqlPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Utilidades.NombreBD());
        /*private TextView textCliente;
        private TextView textTotal;
        private Spinner spinnerFormasPago;
        private Spinner spinnerDirecciones;
        private CheckBox checkFacturable;*/
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

        private MReserva _reserva;
        public MReserva Reserva
        {
            get { return _reserva; }
            set
            {
                _reserva = value;
                try {

                    _reserva.Total = (_reserva.PaxAdulto * _reserva.PrecioAdulto) + (_reserva.PaxInfante * _reserva.PrecioInfante);
                    textFechaSalida.Text = _reserva.FechaSalida.ToShortDateString();
                    textCantidadAdultos.Text = Utilidades.ponerpuntos(_reserva.PaxAdulto.ToString());
                    textCantidadNinos.Text = Utilidades.ponerpuntos(_reserva.PaxInfante.ToString());
                    textPrecioAdulto.Text = Utilidades.ponerpuntos(_reserva.PrecioAdulto.ToString());
                    textPrecioNino.Text = Utilidades.ponerpuntos(_reserva.PrecioInfante.ToString());
                    textObservaciones.Text = _reserva.Observaciones;
                    textTotal.Text = Utilidades.ponerpuntos(_reserva.Total.ToString());

                    /*if (_reserva.TipoRetiro != "Domicilio Cliente")
                    {
                        textRetiro.Text = _reserva.Hospedaje.Nombre + ". " + _reserva.Hospedaje.Direccion;
                    }
                    else
                    {
                        textRetiro.Text = _reserva.Domicilio.DireccionCompleta;
                    }                    
                    textEstadoPago.Text = _reserva.EstadoPago;*/
                }
                catch { }
            }
        }
        private MCliente _clienteSeleccionado;
        private MServicio1 _servicioSeleccionado;
        public MTrabajador Trabajador { get; set; }

        private long LastButtonClickTime;

        public MCliente ClienteSeleccionado
        {
            get { return _clienteSeleccionado; }
            set
            {
                _clienteSeleccionado = value;
                try
                {
                    textNombreCliente.Text = _clienteSeleccionado.NombreCompleto;
                }
                catch { }
            }
        }

        public MServicio1 ServicioSeleccionado
        {
            get { return _servicioSeleccionado; }
            set
            {
                _servicioSeleccionado = value;
                try
                {
                    textNombreServicio.Text = _servicioSeleccionado.Titulo;
                }
                catch { }
            }
        }

        public object JSonConvert { get; private set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu = true;
            if (_reserva == null)
                _reserva = new MReserva();
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var root = inflater.Inflate(Resource.Layout.reservas_setResumen, container, false);

            textNombreCliente = root.FindViewById<TextView>(Resource.Id.textNuevaReservaResumenNombreCliente);
            textFechaSalida = root.FindViewById<TextView>(Resource.Id.textNuevaReservaResumenFechaSalida);
            textCantidadAdultos = root.FindViewById<TextView>(Resource.Id.textNuevaReservaResumenCantidadAdultos);
            textCantidadNinos = root.FindViewById<TextView>(Resource.Id.textNuevaReservaResumenCantidadNinos);
            textPrecioAdulto = root.FindViewById<TextView>(Resource.Id.textNuevaReservaResumenPrecioAdultos);
            textPrecioNino = root.FindViewById<TextView>(Resource.Id.textNuevaReservaResumenPrecioNinos);
            textObservaciones = root.FindViewById<TextView>(Resource.Id.textNuevaReservaResumenObservaciones);
            textTotal = root.FindViewById<TextView>(Resource.Id.textNuevaReservaResumenTotal);
            textNombreServicio = root.FindViewById<TextView>(Resource.Id.textNuevaReservaResumenNombreServicio);
            textRetiro = root.FindViewById<TextView>(Resource.Id.textNuevaReservaResumenRetiro);
            textEstadoPago = root.FindViewById<TextView>(Resource.Id.textNuevaReservaResumenEstadoPago);
            try
            {
                textNombreCliente.Text = _clienteSeleccionado.NombreCompleto;
            }
            catch { }
            try
            {
                textNombreServicio.Text = _servicioSeleccionado.Titulo;
            }
            catch { }
            try
            {
                _reserva.Total = (_reserva.PaxAdulto * _reserva.PrecioAdulto) + (_reserva.PaxInfante * _reserva.PrecioInfante);
                textFechaSalida.Text = _reserva.FechaSalida.ToShortDateString();
                textCantidadAdultos.Text = Utilidades.ponerpuntos(_reserva.PaxAdulto.ToString());
                textCantidadNinos.Text = Utilidades.ponerpuntos(_reserva.PaxInfante.ToString());
                textPrecioAdulto.Text = Utilidades.ponerpuntos(_reserva.PrecioAdulto.ToString());
                textPrecioNino.Text = Utilidades.ponerpuntos(_reserva.PrecioInfante.ToString());
                textObservaciones.Text = _reserva.Observaciones;
                textTotal.Text = Utilidades.ponerpuntos(_reserva.Total.ToString());
                textEstadoPago.Text = _reserva.EstadoPago;
                /*if (_reserva.TipoRetiro != "Domicilio Cliente")
                {
                    textRetiro.Text = _reserva.Hospedaje.Nombre + ".\n" + _reserva.Hospedaje.Direccion;
                }
                else
                {
                    textRetiro.Text = _reserva.Domicilio.DireccionCompleta;
                }*/
            }
            catch { }
            return root;
            //return base.OnCreateView(inflater, container, savedInstanceState);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.menu_NuevaReserva_Resumen, menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menuNuevaReservaGuardar:

                    if (_clienteSeleccionado == null)
                    {
                        Toast.MakeText(Activity, "No ha seleccionado un cliente", ToastLength.Long).Show();
                    }
                    else if (_servicioSeleccionado == null)
                    {
                        Toast.MakeText(Activity, "No ha seleccionado un servicio", ToastLength.Long).Show();
                    }                    
                    else
                    {
                        try
                        {
                            db = new SQLite.SQLiteConnection(sqlPath);
                            Reserva.IdCliente = _clienteSeleccionado.Id;
                            Reserva.IdServicio = _servicioSeleccionado.Id;
                            Reserva.FechaIngreso = DateTime.Now;
                            Reserva.IdUsuario = Trabajador.Id;
                            
                            Reserva.ClienteNombre = _clienteSeleccionado.NombreCompleto;
                            Reserva.ServicioNombre = _servicioSeleccionado.Titulo;
                            //Reserva.RetiroNombre = Reserva.tipoRetiro == "Domicilio Cliente" ? _reserva.Domicilio.DireccionCompleta : (_reserva.Hospedaje.Tipo + " - " + _reserva.Hospedaje.Nombre);
                            if (Reserva.Nuevo) //nueva reserva
                            {
                                db.Insert(Reserva);
                                Toast.MakeText(Activity, "Reserva creada correctamente", ToastLength.Long).Show();
                            }
                            else
                            {
                                db.Update(Reserva);
                                Toast.MakeText(Activity, "Reserva actualizada correctamente", ToastLength.Long).Show();
                            }
                            Intent myIntent = new Intent();
                            myIntent.PutExtra("Guardado", true);
                            
                            Activity.SetResult(Result.Ok, myIntent);
                            
                            Activity.Finish();
                        }
                        catch(Exception ex)
                        {
                            Toast.MakeText(Activity, ex.Message, ToastLength.Long).Show();
                        }
                        
                    }
                    return true;
                default:
                    return false;
            }
        }
    }
}
