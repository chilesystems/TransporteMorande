//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebAPI.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Ticket
    {
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> IdServicio { get; set; }
        public int Valor { get; set; }
        public string Nombre { get; set; }
    
        public virtual Servicio Servicio { get; set; }
    }
}
