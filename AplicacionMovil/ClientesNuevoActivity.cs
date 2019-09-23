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
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AplicacionMovil.BLL;
using Newtonsoft.Json;

namespace AplicacionMovil
{
    [Activity(Label = "Nuevo Cliente", Theme = "@style/AppTheme.NoActionBar")]
    public class ClientesNuevoActivity : AppCompatActivity
    {
        private string sqlPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Utilidades.NombreBD());
        private SQLite.SQLiteConnection db;

        private TextInputLayout textNombre;
        private TextInputLayout textApellidoPaterno;
        private TextInputLayout textApellidoMaterno;
        private TextInputLayout textEmail;
        private TextInputLayout textTelefono1;
        private TextInputLayout textTelefono2;
        private Spinner spinnerIdiomas, spinnerPaises;

        private Button botonGuardar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.clientes_Nuevo);

            db = new SQLite.SQLiteConnection(sqlPath);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarNuevoCliente);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Title = "Nuevo Cliente";

            textNombre = FindViewById<TextInputLayout>(Resource.Id.textNuevoClienteNombre);
            textApellidoPaterno = FindViewById<TextInputLayout>(Resource.Id.textNuevoClienteApellidoPaterno);
            textApellidoMaterno = FindViewById<TextInputLayout>(Resource.Id.textNuevoClienteApellidoMaterno);
            textEmail = FindViewById<TextInputLayout>(Resource.Id.textNuevoClienteEmailFacturacion);
            textTelefono2 = FindViewById<TextInputLayout>(Resource.Id.textNuevoClienteTelefono2);
            textTelefono1 = FindViewById<TextInputLayout>(Resource.Id.textNuevoClienteTelefono1);
            spinnerIdiomas = FindViewById<Spinner>(Resource.Id.spinnerNuevoClienteIdioma);
            spinnerPaises = FindViewById<Spinner>(Resource.Id.spinnerNuevoClientePais);
            //var direcciones = db.Table<MDomicilio>().ToList();
            //List<string> domicilioSpinner = direcciones.Select(x => x.DireccionCompleta).ToList();
            ///ArrayAdapter adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, domicilioSpinner);
            //spinnerDirecciones.Adapter = adapter;
            int indexIdioma = loadIdiomas();
            int indexPaises = loadPaises();

            spinnerIdiomas.SetSelection(indexIdioma);
            spinnerPaises.SetSelection(indexPaises);

            botonGuardar = FindViewById<Button>(Resource.Id.botonGuardarNuevoCliente);

            botonGuardar.Click += BotonGuardar_Click;
        }

        public int loadIdiomas()
        {
            var idiomas = db.Table<MIdioma>().ToList();
            if (idiomas.Count > 0)
            {
                List<string> idiomasSpinner = idiomas.OrderBy(x => x.Nombre).Select(x => x.Nombre).ToList();
                ArrayAdapter adapter = new ArrayAdapter(this, global::Android.Resource.Layout.SimpleSpinnerDropDownItem, idiomasSpinner);
                spinnerIdiomas.Adapter = adapter;
                return idiomasSpinner.IndexOf(db.Table<MIdioma>().First(x => x.Sigla.ToUpper() == "ES").Nombre);
            }
            return 0;
        }

        public int loadPaises()
        {
            var paises = db.Table<MPais>().ToList();
            if (paises.Count > 0)
            {
                List<string> paisesSpinner = paises.OrderBy(x => x.Nombre).Select(x => x.Nombre).ToList();
                ArrayAdapter adapter = new ArrayAdapter(this, global::Android.Resource.Layout.SimpleSpinnerDropDownItem, paisesSpinner);
                spinnerPaises.Adapter = adapter;
                return paisesSpinner.IndexOf(db.Table<MPais>().First(x => x.Sigla == "CL").Nombre);
            }
            return 0;
        }

        private void CardDirecciones_LongClick(object sender, View.LongClickEventArgs e)
        {
            var transaction = SupportFragmentManager.BeginTransaction();
            Dialogs.FragmentClientesDialogOpciones dialog = new Dialogs.FragmentClientesDialogOpciones();
            Bundle args = new Bundle();
            args.PutString("idCliente", "0");
            dialog.Arguments = args;
            dialog.Show(transaction, "Opciones");
        }

       /* public void loadDirecciones()
        {
            var direcciones = db.Table<MDomicilio>().Where(x => x.IdCliente == null).ToList();
            List<string> domicilioSpinner = direcciones.Select(x => x.DireccionCompleta).ToList();
            ArrayAdapter adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, domicilioSpinner);
            spinnerDirecciones.Adapter = adapter;
        }*/

        private void BotonGuardar_Click(object sender, EventArgs e)
        {
            var paisSeleccinado = spinnerPaises.SelectedItem.ToString();
            MPais paisDb = db.Table<MPais>().FirstOrDefault(x => x.Nombre == paisSeleccinado);

            var idiomaSeleccionado = spinnerIdiomas.SelectedItem.ToString();
            MIdioma idiomaDb = db.Table<MIdioma>().FirstOrDefault(x => x.Nombre == idiomaSeleccionado);

            MCliente cliente = new MCliente();
            cliente.Nombre = textNombre.EditText.Text.ToUpper();
            cliente.ApellidoMaterno = textApellidoMaterno.EditText.Text.ToUpper();
            cliente.ApellidoPaterno = textApellidoPaterno.EditText.Text.ToUpper();
            cliente.TelefonoPrimario = textTelefono1.EditText.Text.ToUpper();
            cliente.TelefonoSecundario = textTelefono2.EditText.Text.ToUpper();
            cliente.NombreCompleto = cliente.Nombre + " " + cliente.ApellidoPaterno + " " + cliente.ApellidoMaterno;
            cliente.Email = textEmail.EditText.Text.ToUpper();
            cliente.Id = Guid.NewGuid();
            cliente.IdIdioma = idiomaDb.Id;
            cliente.IdPais = paisDb.Id;
            cliente.IdiomaNombre = idiomaDb.Nombre;
            cliente.PaisNombre = paisDb.Nombre;
            cliente.Nuevo = true;
            cliente.Modificado = false;

            if (string.IsNullOrEmpty(cliente.Nombre)
                || string.IsNullOrEmpty(cliente.Email))
            {
                Toast.MakeText(this, "Recuerde llenar los campos obligatorios marcados con *", ToastLength.Long).Show();
            }
            else
            {
                db.Insert(cliente);
                var direccionesNuevas = db.Table<MDomicilio>().Where(x => x.IdCliente == null).ToList();
                foreach (var direccion in direccionesNuevas)
                {
                    direccion.IdCliente = cliente.Id;
                    db.Update(direccion);
                }
                Toast.MakeText(this, "Cliente guardado correctamente", ToastLength.Long).Show();
                Intent myIntent = Intent;
                myIntent.PutExtra("Guardado", true);
                myIntent.PutExtra("cliente", JsonConvert.SerializeObject(cliente));
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