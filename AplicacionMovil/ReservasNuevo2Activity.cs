using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AplicacionMovil.Adapters;
using AplicacionMovil.BLL;
using Newtonsoft.Json;
using static Android.App.DatePickerDialog;

namespace AplicacionMovil
{
    [Activity(Label = "Nueva Reserva", Theme = "@style/AppTheme.NoActionBar")]
    public class ReservasNuevo2Activity : AppCompatActivity
    {
        private SQLite.SQLiteConnection db;
        private string sqlPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Utilidades.NombreBD());
        private MTrabajador trabajador;
        private MReserva reserva;
        private MServicio1 servicio;
        private MCliente cliente;
        private DateTime? fechaReserva;
        private string tipoRetiro;

        //ArrayAdapter<MDomicilio> adapterDomicilios;
        //ArrayAdapter<MHospedaje> adapterHospedajes;
        //ArrayAdapter<string> adapterErrores;

        private TextInputLayout textNombreCliente;
        private TextInputLayout textNombreServicio;
        private TextInputLayout textFechaSalida;
        private EditText textFechaSalidaValor;
        private TextInputLayout textCantidadAdultos;
        private TextInputLayout textCantidadNinos;
        private TextInputLayout textPrecioAdulto;
        private TextInputLayout textPrecioNino;
        private TextInputLayout textObservaciones;
        private TextView textTotal;
        public bool edit = false;
        private Spinner spinnerNuevaReservaTipoLugarRetiro;
        private TextInputLayout textNuevaReservaTipoLugarEspecifico;
        private Spinner spinnerNuevaReservaEstadoPago;
        private Spinner spinnerNuevaReservaEstadoReserva;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.reservas_Nueva2);
            db = new SQLite.SQLiteConnection(sqlPath);
            trabajador = JsonConvert.DeserializeObject<MTrabajador>(Intent.GetStringExtra("trabajador"));
            try
            {
                reserva = JsonConvert.DeserializeObject<MReserva>(Intent.GetStringExtra("reserva"));
            }
            catch { }
            
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarNuevaReserva2);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Title = "Nueva Reserva";

            textNombreServicio = FindViewById<TextInputLayout>(Resource.Id.textNuevaReservaServicio2);
            textNombreCliente = FindViewById<TextInputLayout>(Resource.Id.textNuevaReservaCliente2);
            spinnerNuevaReservaTipoLugarRetiro = FindViewById<Spinner>(Resource.Id.spinnerNuevaReserva_TipoLugarRetiro2);
            textNuevaReservaTipoLugarEspecifico = FindViewById<TextInputLayout>(Resource.Id.textNuevaReservaLugarEspecifico2);
            textFechaSalida = FindViewById<TextInputLayout>(Resource.Id.textNuevaReservaDesdeFecha2);
            textFechaSalidaValor = FindViewById<EditText>(Resource.Id.textNuevaReservaDesdeFechaVa2);

            textCantidadAdultos = FindViewById<TextInputLayout>(Resource.Id.textNuevaReservaCantidadAdultos2);
            textCantidadNinos = FindViewById<TextInputLayout>(Resource.Id.textNuevaReservaCantidadNinos2);
            textPrecioAdulto = FindViewById<TextInputLayout>(Resource.Id.textNuevaReservaValorAdultos2);
            textPrecioNino = FindViewById<TextInputLayout>(Resource.Id.textNuevaReservaValorNinos2);

            textObservaciones = FindViewById<TextInputLayout>(Resource.Id.textNuevaReservaObservaciones2);
            textTotal = FindViewById<TextView>(Resource.Id.textNuevaReservaTotal2);
            spinnerNuevaReservaEstadoPago = FindViewById<Spinner>(Resource.Id.spinnerNuevaReserva_EstadoPago2);
            spinnerNuevaReservaEstadoReserva = FindViewById<Spinner>(Resource.Id.spinnerNuevaReserva_EstadoReserva2);
            textNombreCliente.EditText.Click += EditText_ClienteClick;
            textNombreServicio.EditText.Click += EditText_ServicioClick;
            textFechaSalida.EditText.Click += EditText_FechaSalidaClick;

            textCantidadAdultos.EditText.AfterTextChanged += EditText_CantidadAfterTextChanged;
            textCantidadNinos.EditText.AfterTextChanged += EditText_CantidadNinosAfterTextChanged;
            textPrecioAdulto.EditText.AfterTextChanged += EditText_ValorAdultoAfterTextChanged;
            textPrecioNino.EditText.AfterTextChanged += EditText_ValorNinoAfterTextChanged;

            textCantidadAdultos.EditText.Text = "1";
            textCantidadNinos.EditText.Text = "0";
            textTotal.Text = "$0";

            List<string> tiposSpinner = new List<string>();
            tiposSpinner.Add("Domicilio Cliente");
            tiposSpinner.Add("Hotel");
            tiposSpinner.Add("Hostal");
            tiposSpinner.Add("Residencial");
            tiposSpinner.Add("Motel");
            tiposSpinner.Add("Otro");
            ArrayAdapter adapter = new ArrayAdapter(this, global::Android.Resource.Layout.SimpleSpinnerDropDownItem, tiposSpinner);

            spinnerNuevaReservaTipoLugarRetiro.Adapter = adapter;
            spinnerNuevaReservaTipoLugarRetiro.ItemSelected += SpinnerNuevaReservaTipoLugarRetiro_ItemSelected;

            
            List<string> estadoPagoSpinner = new List<string>();
            estadoPagoSpinner.Add("Pendiente");
            estadoPagoSpinner.Add("Pagado");
            estadoPagoSpinner.Add("Especial");
            ArrayAdapter adapterEstadoPago = new ArrayAdapter(this, global::Android.Resource.Layout.SimpleSpinnerDropDownItem, estadoPagoSpinner);
            spinnerNuevaReservaEstadoPago.Adapter = adapterEstadoPago;

            List<string> estadoReservaSpinner = new List<string>();
           // estadoReservaSpinner.Add("Sin confirmar");
            estadoReservaSpinner.Add("Confirmada");
            estadoReservaSpinner.Add("Finalizada");
            ArrayAdapter adapterEstadoReserva = new ArrayAdapter(this, global::Android.Resource.Layout.SimpleSpinnerDropDownItem, estadoReservaSpinner);
            spinnerNuevaReservaEstadoReserva.Adapter = adapterEstadoReserva;

            if (reserva == null) reserva = new MReserva() { Nuevo = true, Modificado = false };
            else
            {
                edit = true;
                cliente = db.Table<MCliente>().First(x => x.Id == reserva.IdCliente);
                servicio = db.Table<MServicio1>().First(x => x.Id == reserva.IdServicio);
                fechaReserva = reserva.FechaSalida;
                //tipoRetiro = reserva.IdDomicilio.HasValue ? "Domicilio Cliente" : "Hotel";
                textNombreServicio.EditText.Text = reserva.ServicioNombre;
                textNombreCliente.EditText.Text = reserva.ClienteNombre;
                
                //autoNuevaReservaTipoLugarEspecifico.Text = reserva.RetiroNombre;
                textFechaSalida.EditText.Text = reserva.FechaSalida.ToShortDateString();
                textCantidadAdultos.EditText.Text = reserva.PaxAdulto.ToString();
                textCantidadNinos.EditText.Text = reserva.PaxInfante.ToString();
                textPrecioAdulto.EditText.Text = reserva.PrecioAdulto.ToString();
                textPrecioNino.EditText.Text = reserva.PrecioInfante.ToString();
                textObservaciones.EditText.Text = reserva.Observaciones;
                textNuevaReservaTipoLugarEspecifico.EditText.Text = reserva.RetiroNombre;
                spinnerNuevaReservaEstadoPago.SetSelection(adapterEstadoPago.GetPosition(reserva.EstadoPago));
                spinnerNuevaReservaEstadoReserva.SetSelection(adapterEstadoReserva.GetPosition(reserva.Estado));
                spinnerNuevaReservaTipoLugarRetiro.SetSelection(adapter.GetPosition(reserva.TipoRetiro));
            }

            Window.SetSoftInputMode(SoftInput.StateHidden);

        }


        private void EditText_ValorNinoAfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            calculoTotal();
        }

        private void EditText_ValorAdultoAfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            calculoTotal();
        }

        private void EditText_CantidadNinosAfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            calculoTotal();
        }

        private void EditText_CantidadAfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            calculoTotal();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Intent myIntent = Intent;
                    myIntent.PutExtra("Guardado", false);
                    SetResult(Result.Canceled, myIntent);
                    Finish();
                    break;

                case Resource.Id.menuNuevaReservaGuardar2:
                    Guardar();
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menuNuevaReserva, menu);
            return true;
        }

        public bool Guardar()
        {
            if (cliente == null)
            {
                Toast.MakeText(this, "No ha seleccionado un cliente", ToastLength.Long).Show();
                return false;
            }
            else if (servicio == null)
            {
                Toast.MakeText(this, "No ha seleccionado un servicio", ToastLength.Long).Show();
                return false;
            }
            else if (!fechaReserva.HasValue)
            {
                Toast.MakeText(this, "No ha seleccionado una fecha de retiro", ToastLength.Long).Show();
                return false;
            }

            reserva.Cerrada = false;
            reserva.ClienteNombre = cliente.NombreCompleto;
            reserva.Estado = spinnerNuevaReservaEstadoReserva.SelectedItem.ToString();
            reserva.EstadoPago = spinnerNuevaReservaEstadoPago.SelectedItem.ToString();
            reserva.EstadoPagoEmpleador = "Pendiente";
            reserva.FechaIngreso = DateTime.Now;
            reserva.FechaSalida = fechaReserva.Value;
            reserva.Habitacion = "";
            reserva.IdCliente = cliente.Id;
            reserva.IdServicio = servicio.Id;
            reserva.IdUsuario = trabajador.Id;
            reserva.Observaciones = textObservaciones.EditText.Text.ToUpper();
            reserva.PaxAdulto = int.Parse(textCantidadAdultos.EditText.Text);
            reserva.PaxInfante = int.Parse(textCantidadNinos.EditText.Text);
            reserva.PrecioAdulto = int.Parse(textPrecioAdulto.EditText.Text);
            reserva.PrecioInfante = int.Parse(textPrecioNino.EditText.Text);
            reserva.RetiroNombre = textNuevaReservaTipoLugarEspecifico.EditText.Text.ToUpper();
            reserva.ServicioNombre = servicio.Titulo;
            reserva.TipoRetiro = tipoRetiro;
            reserva.Total = int.Parse(Utilidades.sacarpuntos(textTotal.Text));
            //reserva.IdDomicilio = reserva.Domicilio?.Id;
            //reserva.IdHotel = reserva.Hospedaje?.Id;

            if (!edit)
            {
                reserva.Folio = 0;
                reserva.Id = Guid.NewGuid();
                reserva.Nuevo = true;
                db.Insert(reserva);
                Toast.MakeText(this, "Reserva creada correctamente", ToastLength.Long).Show();
            }
            else
            {
                if (!reserva.Nuevo)
                {
                    reserva.Modificado = true;
                }                
                db.Update(reserva);
                Toast.MakeText(this, "Reserva actualizada correctamente", ToastLength.Long).Show();
            }

            Intent myIntent = new Intent();
            myIntent.PutExtra("Guardado", true);
            myIntent.PutExtra("reserva", JsonConvert.SerializeObject(reserva));
            SetResult(Result.Ok, myIntent);
            Finish();
            return true;
        }

        private void EditText_FechaSalidaClick(object sender, EventArgs e)
        {
            fechaReserva = DateTime.Now;
            DatePickerDialog dateDialog = new DatePickerDialog(this, OnDateSetDesde, fechaReserva.Value.Year, fechaReserva.Value.Month - 1, fechaReserva.Value.Day);
            dateDialog.DatePicker.MinDate = DateTime.Now.Millisecond;
            dateDialog.Show();
        }

        private void OnDateSetDesde(object sender, DateSetEventArgs e)
        {
            
            fechaReserva = e.Date;
            textFechaSalida.EditText.Text = e.Date.ToShortDateString();
        }

        private void EditText_ServicioClick(object sender, EventArgs e)
        {
            var activityClientes = new Intent(this, typeof(ServiciosActivity));
            activityClientes.PutExtra("origen", "reserva");
            StartActivityForResult(activityClientes, 2);
        }

        private void EditText_ClienteClick(object sender, EventArgs e)
        {
            var activityClientes = new Intent(this, typeof(ClientesActivity));
            activityClientes.PutExtra("origen", "reserva");
            StartActivityForResult(activityClientes, 1);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                if (requestCode == 1) // clientes
                {
                    string clienteAux = data.GetStringExtra("cliente");
                    if (!string.IsNullOrEmpty(clienteAux))
                    {
                        cliente = JsonConvert.DeserializeObject<MCliente>(clienteAux);
                        textNombreCliente.EditText.Text = cliente.NombreCompleto;

                        /*try
                        {
                            var listaDomicilios = db.Table<MDomicilio>().Where(x => x.IdCliente == cliente.Id).ToList();
                            if (listaDomicilios.Count == 0) throw new Exception();
                            adapterDomicilios = new ArrayAdapter<MDomicilio>(BaseContext, global::Android.Resource.Layout.SimpleSpinnerDropDownItem, listaDomicilios);
                            autoNuevaReservaTipoLugarEspecifico.Adapter = adapterDomicilios;

                            MDomicilio item = adapterDomicilios.GetItem(0) as MDomicilio;
                            autoNuevaReservaTipoLugarEspecifico.Text = item.DireccionCompleta;
                            reserva.Domicilio = item;
                        }
                        catch (Exception ex)
                        {
                            List<string> listaSinResultados = new List<string>();
                            listaSinResultados.Add("Sin domicilios ingresados");
                            adapterErrores = new ArrayAdapter<string>(BaseContext, global::Android.Resource.Layout.SimpleSpinnerDropDownItem, listaSinResultados);
                            autoNuevaReservaTipoLugarEspecifico.Adapter = adapterErrores;
                        }*/
                    }
                }
                else if (requestCode == 2) // servicios
                {
                    string servicioAux = data.GetStringExtra("servicio");
                    if (!string.IsNullOrEmpty(servicioAux))
                    {
                        servicio = JsonConvert.DeserializeObject<MServicio1>(servicioAux);
                        textNombreServicio.EditText.Text = servicio.Titulo;
                        textPrecioAdulto.EditText.Text = servicio.Precio.ToString();
                        int nino = (int)Math.Round((double)servicio.Precio / 2, 0);
                        textPrecioNino.EditText.Text = nino.ToString();
                        calculoTotal();
                    }
                }
            }
           
        }

        private void calculoTotal()
        {
            try
            {
                int cantidadNinos = int.Parse(textCantidadNinos.EditText.Text);
                int cantidadAdultos = int.Parse(textCantidadAdultos.EditText.Text);
                int valorNino = int.Parse(textPrecioNino.EditText.Text);
                int valorAdulto = int.Parse(textPrecioAdulto.EditText.Text);
                int total = (cantidadNinos * valorNino) + (cantidadAdultos * valorAdulto);
                textTotal.Text = Utilidades.ponerpuntos(total.ToString());
            }
            catch { }            
        }

        private void SpinnerNuevaReservaTipoLugarRetiro_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            List<MHospedaje> listaHospedaje = new List<MHospedaje>();
            List<MDomicilio> listaDomicilios = new List<MDomicilio>();
            List<string> listaSinResultados = new List<string>();
            //List<string> tiposSpinner = new List<string>();
            tipoRetiro = e.Parent.GetItemAtPosition(e.Position).ToString();
            /*if (tipoRetiro == "Domicilio Cliente")
            {
                try
                {
                    listaDomicilios = db.Table<MDomicilio>().Where(x => x.IdCliente == cliente.Id).ToList();
                    if (listaDomicilios.Count == 0) throw new Exception();
                    adapterDomicilios = new ArrayAdapter<MDomicilio>(BaseContext, global::Android.Resource.Layout.SimpleSpinnerDropDownItem, listaDomicilios);
                    autoNuevaReservaTipoLugarEspecifico.Adapter = adapterDomicilios;

                }
                catch (Exception ex)
                {
                    listaSinResultados.Add("Sin domicilios ingresados");
                    adapterErrores = new ArrayAdapter<string>(BaseContext, global::Android.Resource.Layout.SimpleSpinnerDropDownItem, listaSinResultados);
                    autoNuevaReservaTipoLugarEspecifico.Adapter = adapterErrores;
                }
            }
            else
            {
                try
                {
                    listaHospedaje = db.Table<MHospedaje>().Where(x => x.Tipo == tipoRetiro).OrderBy(x => x.Nombre).ToList();
                    //tiposSpinner = db.Table<MHospedaje>().Where(x => x.Tipo == tipoRetiro).OrderBy(x=>x.Nombre).ToList().Select(x => x.Nombre).ToList();
                    if (listaHospedaje.Count == 0) throw new Exception();
                    adapterHospedajes = new ArrayAdapter<MHospedaje>(BaseContext, global::Android.Resource.Layout.SimpleSpinnerDropDownItem, listaHospedaje);
                    autoNuevaReservaTipoLugarEspecifico.Adapter = adapterHospedajes;
                    autoNuevaReservaTipoLugarEspecifico.Text = "";
                }
                catch (Exception ex)
                {
                    listaSinResultados.Add("Sin registros");
                    adapterErrores = new ArrayAdapter<string>(BaseContext, global::Android.Resource.Layout.SimpleSpinnerDropDownItem, listaSinResultados);
                    autoNuevaReservaTipoLugarEspecifico.Adapter = adapterErrores;
                }
            }*/
        }


    }
}
