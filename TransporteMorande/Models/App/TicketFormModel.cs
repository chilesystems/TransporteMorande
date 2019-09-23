using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransporteMorande.Models.App
{
    public class TicketFormModel
    {
        public int Valor { get; set; }
        public string Nombre { get; set; }
        public Guid Id { get; set; }
    }
}