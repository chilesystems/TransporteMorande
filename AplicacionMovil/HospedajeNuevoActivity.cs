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
using Newtonsoft.Json;

namespace AplicacionMovil
{
    [Activity(Label = "Nuevo Hospedaje", Theme = "@style/AppTheme.NoActionBar")]
    public class HospedajeNuevoActivity : AppCompatActivity
    {
        private string sqlPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Utilidades.NombreBD());
        private SQLite.SQLiteConnection db;

        private TextInputLayout textNombre;
        private TextInputLayout textDireccion;
        private TextInputLayout textTelefono1;
        private TextInputLayout textTelefono2;
        private Spinner spinnerTipo;

        private Button botonGuardar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.hospedaje_Nuevo);

            db = new SQLite.SQLiteConnection(sqlPath);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarNuevoHospedaje);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Title = "Nuevo Hospedaje";

            textNombre = FindViewById<TextInputLayout>(Resource.Id.textNuevoHospedajeNombre);
            textDireccion = FindViewById<TextInputLayout>(Resource.Id.textNuevoHospedajeDireccion);
            textTelefono2 = FindViewById<TextInputLayout>(Resource.Id.textNuevoHospedajeTelefono2);
            textTelefono1 = FindViewById<TextInputLayout>(Resource.Id.textNuevoHospedajeTelefono1);
            spinnerTipo = FindViewById<Spinner>(Resource.Id.spinnerNuevoHospedajeTipos);

            List<string> tiposSpinner = new List<string>();
            tiposSpinner.Add("Hotel");
            tiposSpinner.Add("Hostal");
            tiposSpinner.Add("Residencial");
            tiposSpinner.Add("Motel");
            tiposSpinner.Add("Otro");
            ArrayAdapter adapter = new ArrayAdapter(this, global::Android.Resource.Layout.SimpleSpinnerDropDownItem, tiposSpinner);
            spinnerTipo.Adapter = adapter;
           
            botonGuardar = FindViewById<Button>(Resource.Id.botonGuardarNuevoHospedaje);

            botonGuardar.Click += BotonGuardar_Click;
        }

        private void BotonGuardar_Click(object sender, EventArgs e)
        {
            var tipoSeleccionado = spinnerTipo.SelectedItem.ToString();
            MHospedaje hospedaje = new MHospedaje();

            hospedaje.Direccion = textDireccion.EditText.Text.ToUpper();
            hospedaje.Id = Guid.NewGuid();
            hospedaje.Nombre = textNombre.EditText.Text.ToUpper();
            hospedaje.Nuevo = true;
            hospedaje.TelefonoPrimario = textTelefono1.EditText.Text.ToUpper();
            hospedaje.TelefonoSecundario = textTelefono2.EditText.Text.ToUpper();
            hospedaje.Tipo = tipoSeleccionado;
            
            if (string.IsNullOrEmpty(hospedaje.Nombre)
                || string.IsNullOrEmpty(hospedaje.Direccion))
            {
                Toast.MakeText(this, "Recuerde llenar los campos obligatorios marcados con *", ToastLength.Long).Show();
            }
            else
            {
                db.Insert(hospedaje);
                Toast.MakeText(this, "Hospedaje guardado correctamente", ToastLength.Long).Show();
                Intent myIntent = Intent;
                myIntent.PutExtra("Guardado", true);
                myIntent.PutExtra("hospedaje", JsonConvert.SerializeObject(hospedaje));
                SetResult(Result.Ok, myIntent);
                Finish();
            }
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

                case Resource.Id.menuClienteNuevoBotonGuardar:
                    BotonGuardar_Click(null, null);
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menuNuevoCliente, menu);
            return true;
        }
    }
}