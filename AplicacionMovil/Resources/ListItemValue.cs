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
using AplicacionMovil.BLL;

namespace AplicacionMovil.Resources
{
    class ListItemValueCliente : Java.Lang.Object, IHasLabel, IComparable<ListItemValueCliente>
    {
        public ListItemValueCliente(MCliente cliente)
        {
            Cliente = cliente;
        }

        public MCliente Cliente { get; private set; }

        int IComparable<ListItemValueCliente>.CompareTo(ListItemValueCliente value)
        {
            return Cliente.NombreCompleto.CompareTo(value.Cliente.NombreCompleto);
        }

        public override string ToString()
        {
            return Cliente.NombreCompleto;
        }

        public string Label
        {
            get { return Cliente.NombreCompleto[0].ToString(); }
        }
    }

    class ListItemValueHospedaje : Java.Lang.Object, IHasLabel, IComparable<ListItemValueHospedaje>
    {
        public ListItemValueHospedaje(MHospedaje hospedaje)
        {
            Hospedaje = hospedaje;
        }

        public MHospedaje Hospedaje { get; private set; }

        int IComparable<ListItemValueHospedaje>.CompareTo(ListItemValueHospedaje value)
        {
            return Hospedaje.Nombre.CompareTo(value.Hospedaje.Nombre);
        }

        public override string ToString()
        {
            return Hospedaje.Nombre;
        }

        public string Label
        {
            get { return Hospedaje.Tipo; }
        }
    }
}