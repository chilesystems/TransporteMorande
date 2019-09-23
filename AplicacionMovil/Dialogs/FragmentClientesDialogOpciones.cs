using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AplicacionMovil.Dialogs
{
    public class FragmentClientesDialogOpciones : Android.Support.V4.App.DialogFragment
    {
        private ListView lv;
        private String[] opciones = { "Agregar Domicilio" };
        private ArrayAdapter adapter;
        private string idCliente;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            View v = inflater.Inflate(Resource.Layout.clientes_Opciones, container, true);
            idCliente = Arguments.GetString("idCliente");
            //SetStyle(StyleNoFrame, Resource.Style.CustomDialog);
            //SET TITLE FOR DIALOG
            this.Dialog.SetTitle("Opciones");
            lv = v.FindViewById<ListView>(Resource.Id.clientesListViewOpciones);
            adapter = new ArrayAdapter(this.Activity, Android.Resource.Layout.SimpleListItem1, opciones);
            lv.Adapter = adapter;
            //ITEM CLICKS
            lv.ItemClick += Lv_ItemClick;
            return v;
        }

        private void Lv_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (e.Position == 0) //agregar
            {
                var transaction = FragmentManager.BeginTransaction();
                Dialogs.NuevoDomicilioClienteDialog dialog = new Dialogs.NuevoDomicilioClienteDialog();
                Bundle args = new Bundle();
                args.PutString("idCliente", idCliente);
                dialog.Arguments = args;
                dialog.Show(transaction, "NuevoDomicilio");
                Dismiss();
            }
        }
    }
}