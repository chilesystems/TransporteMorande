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

namespace AplicacionMovil
{
    [Activity(Label = "Nuevo Cliente", Theme = "@style/AppTheme.NoActionBar")]
    public class ReservasNuevoActivity : AppCompatActivity
    {
        private SQLite.SQLiteConnection db;
        private string sqlPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Utilidades.NombreBD());
        private TabLayout tabs;
        //private List<MDetalleVenta> detallesVenta;
        private ViewPager viewPager;
        private MTrabajador trabajador;
        private MReserva reserva;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.reservas_Nueva);
            db = new SQLite.SQLiteConnection(sqlPath);
            // detallesVenta = new List<MDetalleVenta>();
            trabajador = JsonConvert.DeserializeObject<MTrabajador>(Intent.GetStringExtra("trabajador"));
            try
            {
                reserva = JsonConvert.DeserializeObject<MReserva>(Intent.GetStringExtra("reserva"));
            }
            catch {
                
            }
            
            //trabajador = JsonConvert.DeserializeObject<MTrabajador>(Intent.GetStringExtra("trabajador"));
            //Configuracion = db.Table<MConfig>().First();
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolBarNuevaReserva);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Title = "Nueva Reserva";

            tabs = FindViewById<TabLayout>(Resource.Id.tabsNuevaReserva);

            viewPager = FindViewById<ViewPager>(Resource.Id.viewpagerNuevaReserva);

            TabAdapter adapter = new TabAdapter(SupportFragmentManager);
            adapter.AddFragment(new Fragments.NuevaReserva_Cliente(), "CLIENTE");
            adapter.AddFragment(new Fragments.NuevaReserva_Servicio(), "SERVICIO");
            adapter.AddFragment(new Fragments.NuevaReserva_Detalle(), "DETALLES");
            adapter.AddFragment(new Fragments.NuevaReserva_Resumen() { Trabajador = trabajador }, "RESUMEN");
            viewPager.Adapter = adapter;

            tabs.SetupWithViewPager(viewPager);
            Window.SetSoftInputMode(SoftInput.AdjustPan);

            if (reserva != null)
            {
                SupportActionBar.Title = "Modificar Reserva";
                var servicio = db.Table<MServicio1>().First(x => x.Id == reserva.IdServicio);
                var cliente = db.Table<MCliente>().First(x => x.Id == reserva.IdCliente);
                reserva.Nuevo = false;
                reserva.Modificado = true;
                establecerCliente(cliente);
                establecerServicio(servicio);
                tabs.GetTabAt(3).Select();
            }
            else
            {
                reserva = new MReserva();
                reserva.Nuevo = true;
                reserva.Modificado = false;
                reserva.Id = Guid.NewGuid();
                reserva.Folio = 0;
                reserva.FechaSalida = DateTime.Now;
            }
            establecerReserva(reserva);
        }

        public void establecerCliente(MCliente cliente)
        {
            (((TabAdapter)viewPager.Adapter).GetItem(3) as Fragments.NuevaReserva_Resumen).ClienteSeleccionado = cliente;
            (((TabAdapter)viewPager.Adapter).GetItem(2) as Fragments.NuevaReserva_Detalle).ClienteSeleccionado = cliente;
            tabs.GetTabAt(1).Select();
            Window.SetSoftInputMode(SoftInput.StateHidden);
        }

        public void establecerServicio(MServicio1 servicio)
        {
            (((TabAdapter)viewPager.Adapter).GetItem(3) as Fragments.NuevaReserva_Resumen).ServicioSeleccionado = servicio;
            (((TabAdapter)viewPager.Adapter).GetItem(2) as Fragments.NuevaReserva_Detalle).ServicioSeleccionado = servicio;
            tabs.GetTabAt(2).Select();
            Window.SetSoftInputMode(SoftInput.StateHidden);
        }

        public void establecerReserva(MReserva reserva)
        {
            (((TabAdapter)viewPager.Adapter).GetItem(3) as Fragments.NuevaReserva_Resumen).Reserva = reserva;
            (((TabAdapter)viewPager.Adapter).GetItem(2) as Fragments.NuevaReserva_Detalle).Reserva = reserva;
            //tabs.GetTabAt(2).Select();
            //Window.SetSoftInputMode(SoftInput.StateHidden);
        }

        /*public void agregarDetalle(MDetalleVenta detalle)
        {
            if (Configuracion.MaximoItems <= detallesVenta.Count)
            {
                Toast.MakeText(this, "No se pueden agregar más productos.", ToastLength.Long).Show();
            }
            else
            {
                detallesVenta.Add(detalle);
                (((TabAdapter)viewPager.Adapter).GetItem(2) as Fragments.FragmentNuevaVentaResumen).Detalles = detallesVenta;
            }
        }*/

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