using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AplicacionMovil.BLL;
using Newtonsoft.Json;

namespace AplicacionMovil
{
    [Activity(Label = "Detalle Cliente", Theme = "@style/AppTheme.NoActionBar", WindowSoftInputMode = SoftInput.StateHidden)]
    public class ClientesDetalleActivity : AppCompatActivity, IDialogInterfaceOnClickListener
    {
        private MCliente cliente;
        private string sqlPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Utilidades.NombreBD());
        SQLite.SQLiteConnection db;
        List<MDomicilio> direccionesCliente;

        TextView textIdioma;
        TextView textPais;
        TextView textNombreCompleto;
        TextView textTelefono;
        TextView textCelular;
        TextView textEmail;
        bool guardado;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.clientes_Detalle);
            db = new SQLite.SQLiteConnection(sqlPath);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarDetalleCliente);
            guardado = false;
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            cliente = JsonConvert.DeserializeObject<MCliente>(Intent.GetStringExtra("cliente"));
            SupportActionBar.Title = cliente.NombreCompleto;

            textNombreCompleto = FindViewById<TextView>(Resource.Id.textClienteNombreCompleto);
            textTelefono = FindViewById<TextView>(Resource.Id.textClienteTelefono);
            textCelular = FindViewById<TextView>(Resource.Id.textClienteCelular);
            textEmail = FindViewById<TextView>(Resource.Id.textClienteEmail);
            textIdioma = FindViewById<TextView>(Resource.Id.textClienteIdioma);
            textPais = FindViewById<TextView>(Resource.Id.textClientePais);
            loadData();

            //InputMethodManager inputManager = (InputMethodManager)this.GetSystemService(Context.InputMethodService);
            //inputManager.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);
        }

        private void ComboDireccion_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
        }

        private bool loadData()
        {
            textNombreCompleto.Text = cliente.NombreCompleto;
            textTelefono.Text = cliente.TelefonoPrimario;
            textCelular.Text = cliente.TelefonoSecundario;
            textEmail.Text = cliente.Email;
            textPais.Text = cliente.PaisNombre;
            textIdioma.Text = cliente.IdiomaNombre;
            /*direccionesCliente = db.Table<MDomicilio>().Where(x => x.IdCliente == cliente.Id).ToList();

            if (direccionesCliente.Count > 0)
            {
                List<string> direccionesSpinner = direccionesCliente.Select(x => x.Calle + " " + x.Numero + " " + x.Complemento).ToList();
                ArrayAdapter adapter = new ArrayAdapter(this, global::Android.Resource.Layout.SimpleSpinnerDropDownItem, direccionesSpinner);
                comboDireccion.Adapter = adapter;
            }
            else
            {
                var builder = new Android.Support.V7.App.AlertDialog.Builder(this)
                            .SetIcon(Resource.Drawable.icon_about)
                            .SetTitle("Error")
                            .SetMessage("El cliente no tiene una dirección. Pruebe sincronizar.")
                            .SetPositiveButton("OK", this)
                            .Create();
                builder.Show();
            }*/
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Intent myIntent = Intent;
                    myIntent.PutExtra("Guardado", guardado);
                    SetResult(Result.Ok, myIntent);
                    Finish();
                    break;
                case Resource.Id.menuClienteEditar:
                    var activityEdit = new Intent(this, typeof(ClientesEditarActivity));
                    activityEdit.PutExtra("cliente", JsonConvert.SerializeObject(cliente));
                    StartActivityForResult(activityEdit, 1);
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                try
                {
                    guardado = data.GetBooleanExtra("Guardado", false);
                    if (guardado)
                    {
                        cliente = db.Table<MCliente>().First(x => x.Id == cliente.Id);
                        loadData();
                    }
                }
                catch { }
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menuDetalleCliente, menu);
            return true;
        }

        public void OnClick(IDialogInterface dialog, int which)
        {
            
        }
    }
}