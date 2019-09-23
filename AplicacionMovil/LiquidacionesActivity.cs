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
using AplicacionMovil.BLL;

namespace AplicacionMovil
{
    [Activity(Label = "Liquidaciones", Theme = "@style/AppTheme.NoActionBar")]
    public class LiquidacionesActivity : AppCompatActivity
    {
        private string sqlPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Utilidades.NombreBD());
        SQLite.SQLiteConnection db;
        List<MReserva> reservas;
        MTrabajador trabajador;

        private TextInputLayout textFechaDesde;
        private TextInputLayout textFechaHasta;
        private EditText textFechaDesdeEditText;
        private DateTime fechaDesde;
        private DateTime fechaHasta;
       
        private ListView listViewLiquidaciones;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            /*SetContentView(Resource.Layout.reservas_Mantenedor);
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
            reservas = db.Table<MReserva>().Where(x => x.IdUsuario == trabajador.Id).OrderBy(x => x.FechaSalida).ToList();

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
            Window.SetSoftInputMode(SoftInput.StateHidden);*/
        }
    }
}