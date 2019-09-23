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
using static Android.App.DatePickerDialog;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace AplicacionMovil.Fragments
{  
    public class NuevaReserva_Detalle : SupportFragment
    {
        private SQLite.SQLiteConnection db;
        private string sqlPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Utilidades.NombreBD());
        /*private TextView textCliente;
        private TextView textTotal;
        private Spinner spinnerFormasPago;
        private Spinner spinnerDirecciones;
        private CheckBox checkFacturable;*/
        ArrayAdapter<MDomicilio> adapterDomicilios;
        ArrayAdapter<MHospedaje> adapterHospedajes;
        ArrayAdapter<string> adapterErrores;

        private TextInputLayout textFechaDesde;
        private DateTime fechaReserva;
        private EditText textFechaDesdeEditText;

        private Spinner spinnerNuevaReservaTipoLugarRetiro;
        private Spinner spinnerNuevaReservaTipoLugarEspecifico;
        private Spinner spinnerNuevaReservaEstadoPago;
        private TextInputLayout textCantidadAdultos;
        private TextInputLayout textCantidadNinos;
        private TextInputLayout textValorAdulto;
        private TextInputLayout textValorNino;
        private TextInputLayout textObservaciones;

        public MReserva Reserva { get; set; }

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
                    //textCliente.Text = _clienteSeleccionado.NombreCompleto;
                    //loadDirecciones();
                    //textTotal.Text = Utilidades.ponerpuntos(_detalles.Sum(x => x.Total).ToString());
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
                    textValorAdulto.EditText.Text = _servicioSeleccionado.Precio.ToString();
                    textValorNino.EditText.Text = Math.Round((decimal)_servicioSeleccionado.Precio / 2, 0).ToString();
                }
                catch { }
            }
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu = true;

            if (Reserva == null)
            {
                fechaReserva = DateTime.Now;
                Reserva = new MReserva();
                Reserva.FechaSalida = fechaReserva;
                Reserva.Observaciones = "";
                Reserva.PaxAdulto = 1;
                Reserva.PaxInfante = 0;
                Reserva.PrecioAdulto = 0;
                Reserva.PrecioInfante = 0;
                Reserva.Total = 0;
            }
            else
            {
                fechaReserva = Reserva.FechaSalida;
            }
            //((ReservasNuevoActivity)Activity).establecerReserva(Reserva);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var root = inflater.Inflate(Resource.Layout.reservas_SetDetalle, container, false);
           
            textFechaDesde = root.FindViewById<TextInputLayout>(Resource.Id.textNuevaReservaDesdeFecha);
            textFechaDesdeEditText = root.FindViewById<EditText>(Resource.Id.textNuevaReservaDesdeFechaVa);
            textFechaDesdeEditText.Click += TextFechaDesdeEditText_Click;

            textCantidadAdultos = root.FindViewById<TextInputLayout>(Resource.Id.textNuevaReservaCantidadAdultos);
            textCantidadNinos = root.FindViewById<TextInputLayout>(Resource.Id.textNuevaReservaCantidadNinos);
            textValorAdulto = root.FindViewById<TextInputLayout>(Resource.Id.textNuevaReservaValorAdultos);
            textValorNino = root.FindViewById<TextInputLayout>(Resource.Id.textNuevaReservaValorNinos);
            textObservaciones = root.FindViewById<TextInputLayout>(Resource.Id.textNuevaReservaObservaciones);

            textObservaciones.EditText.AfterTextChanged += EditText_AfterTextChangedObservaciones;
            textCantidadAdultos.EditText.AfterTextChanged += EditText_AfterTextChangedCantidadAdultos;
            textCantidadNinos.EditText.AfterTextChanged += EditText_AfterTextChangedCantidadNinos;
            textValorAdulto.EditText.AfterTextChanged += EditText_AfterTextChangedValorAdulto;
            textValorNino.EditText.AfterTextChanged += EditText_AfterTextChangedValorNino;

            
            
            //textNuevaReservaTipoRetiro = root.FindViewById<AutoCompleteTextView>(Resource.Id.textNuevaReservaTipoRetiroAutoComplete);
            spinnerNuevaReservaTipoLugarRetiro = root.FindViewById<Spinner>(Resource.Id.spinnerNuevaReserva_TipoLugarRetiro);
            spinnerNuevaReservaTipoLugarEspecifico = root.FindViewById<Spinner>(Resource.Id.spinnerNuevaReserva_TipoLugarEspecifico);
            spinnerNuevaReservaEstadoPago = root.FindViewById<Spinner>(Resource.Id.spinnerNuevaReserva_EstadoPago);
            db = new SQLite.SQLiteConnection(sqlPath);

            List<string> tiposSpinner = new List<string>();
            tiposSpinner.Add("Domicilio Cliente");
            tiposSpinner.Add("Hotel");
            tiposSpinner.Add("Hostal");
            tiposSpinner.Add("Residencial");
            tiposSpinner.Add("Motel");
            tiposSpinner.Add("Otro");
            ArrayAdapter adapter = new ArrayAdapter(Activity, global::Android.Resource.Layout.SimpleSpinnerDropDownItem, tiposSpinner);

            spinnerNuevaReservaTipoLugarRetiro.Adapter = adapter;
            spinnerNuevaReservaTipoLugarRetiro.ItemSelected += SpinnerNuevaReservaTipoLugarRetiro_ItemSelected;

            spinnerNuevaReservaTipoLugarEspecifico.ItemSelected += SpinnerNuevaReservaTipoLugarEspecifico_ItemSelected;
            //textNuevaReservaTipoRetiro.SetOnKeyListener(null);
            //textNuevaReservaTipoRetiro.Touch += TextNuevaReservaTipoRetiro_Touch;

            List<string> estadoPagoSpinner = new List<string>();
            estadoPagoSpinner.Add("Pendiente");
            estadoPagoSpinner.Add("Pagado");
            estadoPagoSpinner.Add("Especial");
            ArrayAdapter adapterEstadoPago = new ArrayAdapter(Activity, global::Android.Resource.Layout.SimpleSpinnerDropDownItem, estadoPagoSpinner);

            spinnerNuevaReservaEstadoPago.Adapter = adapterEstadoPago;
            spinnerNuevaReservaEstadoPago.ItemSelected += SpinnerNuevaReservaEstadoPago_ItemSelected;

            textFechaDesde.EditText.Text = fechaReserva.ToShortDateString();
            if (Reserva.Nuevo)
            {
                textCantidadAdultos.EditText.Text = "1";
                textCantidadNinos.EditText.Text = "0";
            }
            else
            {
                textCantidadAdultos.EditText.Text = "1";
                textCantidadNinos.EditText.Text = "0";
                textValorAdulto.EditText.Text = "1";
                textValorNino.EditText.Text = "0";
                textObservaciones.EditText.Text = "0";

                /*if (Reserva.tipoRetiro == "Domicilio Cliente")
                {
                    var domicilio = db.Table<MDomicilio>().First(x => x.Id == Reserva.IdDomicilio);
                    //MDomicilio item = adapterDomicilios.GetItem(e.Position) as MDomicilio;
                    //Reserva.IdDomicilio = item.Id;
                    //Reserva.Domicilio = item;
                    //spinnerNuevaReservaTipoLugarEspecifico.SetSelection()

                }*/
            }

            return root;
            //return base.OnCreateView(inflater, container, savedInstanceState);
        }

        private void SpinnerNuevaReservaEstadoPago_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var estadoPago = e.Parent.GetItemAtPosition(e.Position).ToString();
            Reserva.EstadoPago = estadoPago;
            ((ReservasNuevoActivity)Activity).establecerReserva(Reserva);
        }

        private void SpinnerNuevaReservaTipoLugarEspecifico_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
            {
                /*if (Reserva.tipoRetiro == "Domicilio Cliente")
                {
                    MDomicilio item = adapterDomicilios.GetItem(e.Position) as MDomicilio;
                    Reserva.IdDomicilio = item.Id;
                    Reserva.Domicilio = item;
                }
                else
                {
                    MHospedaje item = adapterHospedajes.GetItem(e.Position) as MHospedaje;
                    Reserva.IdDomicilio = item.Id;
                    Reserva.Hospedaje = item;
                }*/
            }
            catch
            {

            }
            
            ((ReservasNuevoActivity)Activity).establecerReserva(Reserva);
        }

        private void EditText_AfterTextChangedValorNino(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            try
            {
                Reserva.PrecioInfante = int.Parse(textCantidadNinos.EditText.Text);
                ((ReservasNuevoActivity)Activity).establecerReserva(Reserva);
            }
            catch
            {
                Reserva.PrecioInfante = 0;
                ((ReservasNuevoActivity)Activity).establecerReserva(Reserva);
            }
           
        }

        private void EditText_AfterTextChangedValorAdulto(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            try
            {
                Reserva.PrecioAdulto = int.Parse(textValorAdulto.EditText.Text);
                ((ReservasNuevoActivity)Activity).establecerReserva(Reserva);
            }
            catch
            {
                Reserva.PrecioAdulto = 0;
                ((ReservasNuevoActivity)Activity).establecerReserva(Reserva);
            }
           
        }

        private void EditText_AfterTextChangedCantidadNinos(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            try {
                Reserva.PaxInfante = int.Parse(textCantidadNinos.EditText.Text);
                ((ReservasNuevoActivity)Activity).establecerReserva(Reserva);
            }
            catch {
                Reserva.PaxInfante = 0;
                ((ReservasNuevoActivity)Activity).establecerReserva(Reserva);
            }
            
        }

        private void EditText_AfterTextChangedCantidadAdultos(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            try {
                Reserva.PaxAdulto = int.Parse(textCantidadAdultos.EditText.Text);
                ((ReservasNuevoActivity)Activity).establecerReserva(Reserva);
            }
            catch {
                Reserva.PaxAdulto = 0;
                ((ReservasNuevoActivity)Activity).establecerReserva(Reserva);
            }
            
        }

        private void EditText_AfterTextChangedObservaciones(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            Reserva.Observaciones = textObservaciones.EditText.Text;
            ((ReservasNuevoActivity)Activity).establecerReserva(Reserva);
        }

        /*private void EditText_FocusChangeObservaciones(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus)
            {
                Reserva.Observaciones = textObservaciones.EditText.Text;
                ((ReservasNuevoActivity)Activity).establecerReserva(Reserva);
            }
        }*/

        private void SpinnerNuevaReservaTipoLugarRetiro_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            List<MHospedaje> listaHospedaje = new List<MHospedaje>();
            List<MDomicilio> listaDomicilios = new List<MDomicilio>();
            List<string> listaSinResultados = new List<string>();
            //List<string> tiposSpinner = new List<string>();
            var tipoRetiro = e.Parent.GetItemAtPosition(e.Position).ToString();
            if (tipoRetiro == "Domicilio Cliente")
            {
               /* try
                {
                    listaDomicilios = db.Table<MDomicilio>().Where(x => x.IdCliente == ClienteSeleccionado.Id).ToList();
                    if (listaDomicilios.Count == 0) throw new Exception();
                    Reserva.tipoRetiro = tipoRetiro;
                    ((ReservasNuevoActivity)Activity).establecerReserva(Reserva);
                    adapterDomicilios = new ArrayAdapter<MDomicilio>(Context, global::Android.Resource.Layout.SimpleSpinnerDropDownItem, listaDomicilios);
                    spinnerNuevaReservaTipoLugarEspecifico.Adapter = adapterDomicilios;

                }
                catch (Exception ex)
                {
                    listaSinResultados.Add("Sin domicilios ingresados");
                    adapterErrores = new ArrayAdapter<string>(Context, global::Android.Resource.Layout.SimpleSpinnerDropDownItem, listaSinResultados);
                    spinnerNuevaReservaTipoLugarEspecifico.Adapter = adapterErrores;
                }*/
            }
            else
            {
                /*try
                {
                    listaHospedaje = db.Table<MHospedaje>().Where(x => x.Tipo == tipoRetiro).OrderBy(x => x.Nombre).ToList();
                    //tiposSpinner = db.Table<MHospedaje>().Where(x => x.Tipo == tipoRetiro).OrderBy(x=>x.Nombre).ToList().Select(x => x.Nombre).ToList();
                    if (listaHospedaje.Count == 0) throw new Exception();
                    Reserva.tipoRetiro = tipoRetiro;
                    ((ReservasNuevoActivity)Activity).establecerReserva(Reserva);
                    adapterHospedajes = new ArrayAdapter<MHospedaje>(Context, global::Android.Resource.Layout.SimpleSpinnerDropDownItem, listaHospedaje);
                    spinnerNuevaReservaTipoLugarEspecifico.Adapter = adapterHospedajes;
                }
                catch (Exception ex)
                {
                    listaSinResultados.Add("Sin registros");
                    adapterErrores = new ArrayAdapter<string>(Context, global::Android.Resource.Layout.SimpleSpinnerDropDownItem, listaSinResultados);
                    spinnerNuevaReservaTipoLugarEspecifico.Adapter = adapterErrores;
                }*/
            }
        }

        private void TextFechaDesdeEditText_Click(object sender, EventArgs e)
        {
            DatePickerDialog dateDialog = new DatePickerDialog(Activity, OnDateSetDesde, fechaReserva.Year, fechaReserva.Month - 1, fechaReserva.Day);
            dateDialog.DatePicker.MinDate = fechaReserva.Millisecond;
            dateDialog.Show();
        }

        private void OnDateSetDesde(object sender, DateSetEventArgs e)
        {
            fechaReserva = e.Date;
            Reserva.FechaSalida = fechaReserva;
            textFechaDesde.EditText.Text = e.Date.ToShortDateString();
            ((ReservasNuevoActivity)Activity).establecerReserva(Reserva);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.menu_NuevaReserva_Detalle, menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menuNuevaReservaNuevoHospedaje:
                    var activityNuevoHospedaje = new Intent(Activity, typeof(HospedajeNuevoActivity));
                    StartActivityForResult(activityNuevoHospedaje, 3);
                    return true;
                default:
                    return false;
            }
        }

       /* public override void OnActivityResult(int requestCode, int resultCode, Intent data)
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
        }*/
    }
}