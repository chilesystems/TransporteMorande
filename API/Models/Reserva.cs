//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace API.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Reserva
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Reserva()
        {
            this.DetalleLiquidacion = new HashSet<DetalleLiquidacion>();
        }
    
        public System.Guid Id { get; set; }
        public System.DateTime FechaIngreso { get; set; }
        public string IdUsuario { get; set; }
        public System.DateTime FechaSalida { get; set; }
        public System.Guid IdCliente { get; set; }
        public int PaxAdulto { get; set; }
        public int PrecioAdulto { get; set; }
        public int PaxInfante { get; set; }
        public int PrecioInfante { get; set; }
        public int Total { get; set; }
        public string Observaciones { get; set; }
        public string EstadoPago { get; set; }
        public string Estado { get; set; }
        public System.Guid IdServicio { get; set; }
        public bool Cerrada { get; set; }
        public string Habitacion { get; set; }
        public string EstadoPagoEmpleador { get; set; }
        public int Folio { get; set; }
        public string Retiro { get; set; }
        public string TipoRetiro { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual Cliente Cliente { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DetalleLiquidacion> DetalleLiquidacion { get; set; }
        public virtual Servicio Servicio { get; set; }
    }
}
