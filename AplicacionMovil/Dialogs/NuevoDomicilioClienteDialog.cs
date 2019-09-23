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

namespace AplicacionMovil.Dialogs
{
    public class NuevoDomicilioClienteDialog : Android.Support.V4.App.DialogFragment
    {
        private string sqlPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Utilidades.NombreBD());
        private SQLite.SQLiteConnection db;
        TextInputLayout textCalle;
        TextInputLayout textNumero;
        TextInputLayout textComplemento;
        TextInputLayout textReferencias;
        string idCliente;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.clientes_AgregarDomicilio, container, false);
            db = new SQLite.SQLiteConnection(sqlPath);
            idCliente = Arguments.GetString("idCliente");
            textCalle = view.FindViewById<TextInputLayout>(Resource.Id.textClientesNuevoDomicilioCalle);
            textNumero = view.FindViewById<TextInputLayout>(Resource.Id.textClientesNuevoDomicilioNumero);
            textComplemento = view.FindViewById<TextInputLayout>(Resource.Id.textClientesNuevoDomicilioComplemento);
            textReferencias = view.FindViewById<TextInputLayout>(Resource.Id.textClientesNuevoDomicilioReferencia);

            Button botonGuardar = view.FindViewById<Button>(Resource.Id.botonClientesGuardarNuevoDomicilio);
            botonGuardar.Click += BotonGuardar_Click;
            return view;
        }

        private void BotonGuardar_Click(object sender, EventArgs e)
        {
            MDomicilio nuevoDomicilio = new MDomicilio();
            nuevoDomicilio.Calle = textCalle.EditText.Text.ToUpper();
            nuevoDomicilio.Numero = textNumero.EditText.Text;
            nuevoDomicilio.Complemento = textComplemento.EditText.Text.ToUpper();
            nuevoDomicilio.Referencia = textReferencias.EditText.Text.ToUpper();
            nuevoDomicilio.Id = Guid.NewGuid();
            nuevoDomicilio.Nuevo = true;
            nuevoDomicilio.Confirmado = false;
            if (idCliente != "0")
                nuevoDomicilio.IdCliente = Guid.Parse(idCliente);
            else nuevoDomicilio.IdCliente = null;
            Toast.MakeText(Activity, "Domicilio guardado localmente", ToastLength.Long);
            db.Insert(nuevoDomicilio);
            Dismiss();
        }

        /*public override void OnDismiss(IDialogInterface dialog)
        {
            if (idCliente == "0")
            {
                var parent = (ClientesNuevoActivity)Activity;
                //parent.loadDirecciones();
                base.OnDismiss(dialog);
            }
            else
            {
                var parent = (ClientesEditarActivity)Activity;
                //parent.loadDirecciones();
                base.OnDismiss(dialog);
            }
            
        }*/
    }
}