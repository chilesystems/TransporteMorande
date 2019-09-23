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
    [Activity(Label = "Detalle Hospedaje", Theme = "@style/AppTheme.NoActionBar")]
    public class HospedajeDetalleActivity : AppCompatActivity
    {
        private MHospedaje hospedaje;
        private string sqlPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Utilidades.NombreBD());
        SQLite.SQLiteConnection db;

        TextView textNombre;
        TextView textTelefono1;
        TextView textTelefono2;
        TextView textDireccion;
        TextView textTipo;
        bool guardado;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.hospedaje_Detalle);
            db = new SQLite.SQLiteConnection(sqlPath);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarDetalleHospedaje);
            guardado = false;
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            hospedaje = JsonConvert.DeserializeObject<MHospedaje>(Intent.GetStringExtra("hospedaje"));
            SupportActionBar.Title = hospedaje.Nombre + " - " + hospedaje.Tipo;

            textNombre = FindViewById<TextView>(Resource.Id.textHospedajeNombre);
            textTelefono1 = FindViewById<TextView>(Resource.Id.textHospedajeTelefono1);
            textTelefono2 = FindViewById<TextView>(Resource.Id.textHospedajeTelefono2);
            textDireccion = FindViewById<TextView>(Resource.Id.textHospedajeDireccion);
            textTipo = FindViewById<TextView>(Resource.Id.textHospedajeTipo);

            loadData();

            //InputMethodManager inputManager = (InputMethodManager)this.GetSystemService(Context.InputMethodService);
            //inputManager.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);
        }

        private bool loadData()
        {
            textNombre.Text = hospedaje.Nombre.ToUpper();
            textTelefono1.Text = hospedaje.TelefonoPrimario;
            textTelefono2.Text = hospedaje.TelefonoSecundario;
            textDireccion.Text = hospedaje.Direccion.ToUpper();
            textTipo.Text = hospedaje.Tipo.ToUpper();
            return true;
        }

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