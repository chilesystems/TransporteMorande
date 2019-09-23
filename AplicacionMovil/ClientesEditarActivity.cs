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
    [Activity(Label = "Editar Cliente", Theme = "@style/AppTheme.NoActionBar")]
    public class ClientesEditarActivity : AppCompatActivity
    {
        private MCliente cliente;
        private string sqlPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Utilidades.NombreBD());
        private SQLite.SQLiteConnection db;
        private TextInputLayout textNombre;
        private TextInputLayout textApellidoPaterno;
        private TextInputLayout textApellidoMaterno;
        private TextInputLayout textEmail;
        private TextInputLayout textTelefono2;
        private TextInputLayout textTelefono1;
        private Spinner spinnerIdiomas, spinnerPaises;
        //private CardView cardDirecciones, cardPaises, cardIdiomas;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.clientes_Editar);
            db = new SQLite.SQLiteConnection(sqlPath);
            cliente = JsonConvert.DeserializeObject<MCliente>(Intent.GetStringExtra("cliente"));
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarEditarCliente);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Title = "Detalle de cliente ";


            textNombre = FindViewById<TextInputLayout>(Resource.Id.textEditarClienteNombre);
            textApellidoPaterno = FindViewById<TextInputLayout>(Resource.Id.textEditarClienteApellidoPaterno);
            textApellidoMaterno = FindViewById<TextInputLayout>(Resource.Id.textEditarClienteApellidoMaterno);
            textEmail = FindViewById<TextInputLayout>(Resource.Id.textEditarClienteEmail);
            textTelefono1 = FindViewById<TextInputLayout>(Resource.Id.textEditarClienteTelefono1);
            textTelefono2 = FindViewById<TextInputLayout>(Resource.Id.textEditarClienteTelefono2);
            spinnerPaises = FindViewById<Spinner>(Resource.Id.spinnerEditarClientePais);
            spinnerIdiomas = FindViewById<Spinner>(Resource.Id.spinnerEditarClienteIdioma);


            textNombre.EditText.Text = cliente.Nombre;
            textApellidoPaterno.EditText.Text = cliente.ApellidoPaterno;
            textApellidoMaterno.EditText.Text = cliente.ApellidoMaterno;
            textEmail.EditText.Text = cliente.Email;
            textTelefono1.EditText.Text = cliente.TelefonoPrimario;
            textTelefono2.EditText.Text = cliente.TelefonoSecundario;

            //loadDirecciones();
            int indexIdioma = loadIdiomas(cliente.IdIdioma);
            int indexPaises = loadPaises(cliente.IdPais);

            try
            {
                spinnerIdiomas.SetSelection(indexIdioma);
                spinnerPaises.SetSelection(indexPaises);
            }
            catch { }
        }

        private void CardDirecciones_LongClick(object sender, View.LongClickEventArgs e)
        {
            var transaction = SupportFragmentManager.BeginTransaction();
            Dialogs.FragmentClientesDialogOpciones dialog = new Dialogs.FragmentClientesDialogOpciones();
            Bundle args = new Bundle();
            args.PutString("idCliente", cliente.Id.ToString());
            dialog.Arguments = args;
            dialog.Show(transaction, "Opciones");
        }

        //public void loadDirecciones()
        //{
        //    var direccionesCliente = db.Table<MDomicilio>().Where(x => x.IdCliente == cliente.Id).ToList();
        //    if (direccionesCliente.Count > 0)
        //    {
        //        List<string> direccionesSpinner = direccionesCliente.Select(x => x.DireccionCompleta).ToList();
        //        ArrayAdapter adapter = new ArrayAdapter(this, global::Android.Resource.Layout.SimpleSpinnerDropDownItem, direccionesSpinner);
        //        spinnerDirecciones.Adapter = adapter;
        //    }
        //}

        public int loadIdiomas(Guid idIdioma)
        {
            var idiomas = db.Table<MIdioma>().ToList();
            if (idiomas.Count > 0)
            {
                List<string> idiomasSpinner = idiomas.OrderBy(x=>x.Nombre).Select(x => x.Nombre).ToList();
                ArrayAdapter adapter = new ArrayAdapter(this, global::Android.Resource.Layout.SimpleSpinnerDropDownItem, idiomasSpinner);
                spinnerIdiomas.Adapter = adapter;
                return idiomasSpinner.IndexOf(db.Table<MIdioma>().First(x => x.Id == idIdioma).Nombre);
            }
            return 0;
        }

        public int loadPaises(Guid idPais)
        {
            var paises = db.Table<MPais>().ToList();
            if (paises.Count > 0)
            {
                List<string> paisesSpinner = paises.OrderBy(x=>x.Nombre).Select(x => x.Nombre).ToList();
                ArrayAdapter adapter = new ArrayAdapter(this, global::Android.Resource.Layout.SimpleSpinnerDropDownItem, paisesSpinner);
                spinnerPaises.Adapter = adapter;
                return paisesSpinner.IndexOf(db.Table<MPais>().First(x => x.Id == idPais).Nombre);
            }
            return 0;
        }

        private void SpinnerDirecciones_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menuEditarCliente, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    EliminarDatosSinConfirmar();
                    Intent myIntent1 = Intent;
                    myIntent1.PutExtra("Guardado", false);
                    SetResult(Result.Ok, myIntent1);
                    Finish();
                    break;
                case Resource.Id.menuClienteEditarBotonGuardar:
                    ConfirmarNuevosDatos();
                    Intent myIntent2 = Intent;
                    myIntent2.PutExtra("Guardado", true);
                    SetResult(Result.Ok, myIntent2);
                    Finish();
                    Toast.MakeText(this, "Cliente modificado correctamente", ToastLength.Long).Show();
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void EliminarDatosSinConfirmar()
        {
            db.Table<MDomicilio>().Delete(x => x.Nuevo && x.Confirmado == false);
        }


        private void ConfirmarNuevosDatos()
        {
            var domiciliosSinConfirmar = db.Table<MDomicilio>().Where(x => x.Nuevo && x.Confirmado == false).ToList();
            foreach (var a in domiciliosSinConfirmar)
            {
                db.Execute("UPDATE MDomicilio SET Confirmado = ? WHERE IdCliente = ? AND Calle = ? AND Numero = ? ", true, a.IdCliente, a.Calle, a.Numero);
            }

            var paisSeleccinado = spinnerPaises.SelectedItem.ToString();
            MPais paisDb = db.Table<MPais>().FirstOrDefault(x => x.Nombre == paisSeleccinado);

            var idiomaSeleccionado = spinnerIdiomas.SelectedItem.ToString();
            MIdioma idiomaDb = db.Table<MIdioma>().FirstOrDefault(x => x.Nombre == idiomaSeleccionado);

            db.Execute(
            @"UPDATE MCliente SET Nombre = ?, ApellidoPaterno = ?, ApellidoMaterno = ?, 
            Email = ?, TelefonoPrimario = ?, TelefonoSecundario = ?, NombreCompleto = ?, IdPais = ?,
            IdIdioma = ?, PaisNombre = ?, IdiomaNombre = ?, Modificado = ? WHERE Id = ?",
            textNombre.EditText.Text.ToUpper(),
            textApellidoPaterno.EditText.Text.ToUpper(),
            textApellidoMaterno.EditText.Text.ToUpper(),
            textEmail.EditText.Text.ToUpper(),
            textTelefono1.EditText.Text.ToUpper(),
            textTelefono2.EditText.Text.ToUpper(),
            textNombre.EditText.Text.ToUpper() + " " + textApellidoPaterno.EditText.Text.ToUpper() + " " + textApellidoMaterno.EditText.Text.ToUpper(),
            paisDb.Id, idiomaDb.Id, paisDb.Nombre, idiomaDb.Nombre,
            true, 
            cliente.Id);
        }
    }
}