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
using Newtonsoft.Json;
using static Android.App.DatePickerDialog;

namespace AplicacionMovil
{
    [Activity(Label = "Clientes", Theme = "@style/AppTheme.NoActionBar")]
    public class ReservasActivity : AppCompatActivity
    {
        
        private string sqlPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Utilidades.NombreBD());
        SQLite.SQLiteConnection db;
        List<MReserva> reservas;
        MTrabajador trabajador;

        private TextInputLayout textFechaDesde;
        private EditText textFechaDesdeEditText;
        private DateTime fechaDesde;
        private TextInputLayout textFechaHasta;
        private EditText textFechaHastaEditText;
        private DateTime fechaHasta;
        private FloatingActionButton fab;
        private ListView listViewReservas;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.reservas_Mantenedor);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarMantenedorReservas);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Title = "Mis Reservas";

            
            trabajador = JsonConvert.DeserializeObject<MTrabajador>(Intent.GetStringExtra("trabajador"));
            fab = FindViewById<FloatingActionButton>(Resource.Id.fabNuevaReserva);
            fab.Click += Fab_Click;
            listViewReservas = FindViewById<ListView>(Resource.Id.listReservas);
            listViewReservas.ItemClick += ListViewReservas_ItemClick;

            textFechaDesde = FindViewById<TextInputLayout>(Resource.Id.dateBuscarReservaDesde);
            textFechaDesdeEditText = FindViewById<EditText>(Resource.Id.dateBuscarReservaDesdeValor);
            textFechaDesdeEditText.Click += TextFechaDesdeEditText_Click;

            textFechaHasta = FindViewById<TextInputLayout>(Resource.Id.dateBuscarReservaHasta);
            textFechaHastaEditText = FindViewById<EditText>(Resource.Id.dateBuscarReservaHastaValor);
            textFechaHastaEditText.Click += TextFechaHastaEditText_Click;
            

            db = new SQLite.SQLiteConnection(sqlPath);
            reservas = db.Table<MReserva>().Where(x=>x.IdUsuario == trabajador.Id).OrderBy(x => x.FechaSalida).ToList();

            try
            {
                //fechaDesde = reservas.Where(x => x.Estado == "Sin confirmar").Min(x => x.FechaSalida);
                fechaDesde = DateTime.Now.AddDays(-1);
                fechaHasta = reservas.Max(x => x.FechaSalida);
            }
            catch
            {
                fechaDesde = DateTime.Now;
                fechaHasta = DateTime.Now;
            }

            textFechaDesde.EditText.Text = fechaDesde.Date.ToShortDateString();
            textFechaHasta.EditText.Text = fechaHasta.Date.ToShortDateString();

            buscarReservas();
            listViewReservas.FastScrollEnabled = true;
            Window.SetSoftInputMode(SoftInput.StateHidden);
        }

        private void Fab_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ReservasNuevo2Activity));
            intent.PutExtra("trabajador", JsonConvert.SerializeObject(trabajador));
            StartActivityForResult(intent, 3);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok) //Nueva reserva
            {
                bool guardado = data.GetBooleanExtra("Guardado", false);
                if (guardado)
                {
                    reservas = db.Table<MReserva>().Where(x => x.IdUsuario == trabajador.Id).OrderBy(x => x.FechaSalida).ToList();
                    buscarReservas();
                }
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menuReservas, menu);
            return true;
        }

        private void TextFechaHastaEditText_Click(object sender, EventArgs e)
        {
            DatePickerDialog dateDialog = new DatePickerDialog(this, OnDateSetHasta, fechaHasta.Year, fechaHasta.Month - 1, fechaHasta.Day);
            dateDialog.DatePicker.MinDate = fechaHasta.Millisecond;
            dateDialog.Show();
        }

        private void TextFechaDesdeEditText_Click(object sender, EventArgs e)
        {
            DatePickerDialog dateDialog = new DatePickerDialog(this, OnDateSetDesde, fechaDesde.Year, fechaDesde.Month - 1, fechaDesde.Day);
            dateDialog.DatePicker.MinDate = fechaDesde.Millisecond;
            dateDialog.Show();
        }

        private void buscarReservas()
        {
            var reservasAux = reservas.Where(x => x.FechaSalida >= fechaDesde.Date && x.FechaSalida <= fechaHasta.Date).ToList();
            var ventasAdapter = new ReservaAdapter(this, reservasAux);
            listViewReservas.Adapter = ventasAdapter;
            ventasAdapter.NotifyDataSetChanged();
        }

        private void OnDateSetDesde(object sender, DateSetEventArgs e)
        {
            fechaDesde = e.Date;
            textFechaDesde.EditText.Text = e.Date.ToShortDateString();
            buscarReservas();
        }

        private void OnDateSetHasta(object sender, DateSetEventArgs e)
        {
            fechaHasta = e.Date;
            textFechaHasta.EditText.Text = e.Date.ToShortDateString();
            buscarReservas();
        }

        private void ListViewReservas_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent intent = new Intent(this, typeof(ReservasDetalleActivity));
            string idReserva = e.View.FindViewById<TextView>(Resource.Id.textReservaIdItem).Text;
            Guid reservaGuid = Guid.Parse(idReserva);
            MReserva _reserva = db.Table<MReserva>().First(x => x.Id == reservaGuid);
            intent.PutExtra("reserva", JsonConvert.SerializeObject(_reserva));
            intent.PutExtra("trabajador", JsonConvert.SerializeObject(trabajador));
            StartActivityForResult(intent, 1);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    break;

                case Resource.Id.menuReservasReservasSinPagar:
                    reservas = db.Table<MReserva>().Where(x => x.IdUsuario == trabajador.Id && x.EstadoPago == "Pendiente").OrderBy(x => x.FechaSalida).ToList();
                    fechaDesde = reservas.Min(x => x.FechaSalida);
                    fechaHasta = reservas.Max(x => x.FechaSalida);
                    textFechaDesde.EditText.Text = fechaDesde.Date.ToShortDateString();
                    textFechaHasta.EditText.Text = fechaHasta.Date.ToShortDateString();
                    buscarReservas();
                    break;

                case Resource.Id.menuReservasReservasTodas:
                    reservas = db.Table<MReserva>().Where(x => x.IdUsuario == trabajador.Id).OrderBy(x => x.FechaSalida).ToList();
                    textFechaDesde.EditText.Text = fechaDesde.Date.ToShortDateString();
                    textFechaHasta.EditText.Text = fechaHasta.Date.ToShortDateString();
                    fechaDesde = DateTime.Now.AddDays(-1);
                    fechaHasta = reservas.Max(x => x.FechaSalida);
                    buscarReservas();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}