//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TransporteMorande.Models.App
{
    using System;
    using System.Collections.Generic;
    
    public partial class Domicilio
    {
        public System.Guid Id { get; set; }
        public System.Guid IdCliente { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Referencia { get; set; }
        public bool Activo { get; set; }
    
        public virtual Cliente Cliente { get; set; }
    }
}
