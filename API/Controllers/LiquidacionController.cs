using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    [RoutePrefix("api/Liquidaciones")]
    public class LiquidacionController : ApiController
    {
        private appmorandeEntities db = new appmorandeEntities();

        [HttpGet]
        [Route("Listado/{idUsuario}")]
        public List<MLiquidacion> ActualizarLiquidaciones(string idUsuario)
        {
            List<MLiquidacion> lista = new List<MLiquidacion>();
            var liquidaciones = db.Liquidacion.Where(x=>x.IdVendedor == idUsuario).ToList();
            try
            {
                foreach (var liq in liquidaciones)
                {
                    MLiquidacion aux = new MLiquidacion()
                    {
                       Estado = liq.Estado,
                       Fecha = liq.Fecha,
                       Id = liq.Id,
                       IdUsuario = liq.IdUsuario,
                       IdVendedor = liq.IdVendedor,
                       Monto = liq.Monto
                    };
                    aux.Detalles = new List<MDetalleLiquidacion>();
                    foreach (var det in liq.DetalleLiquidacion)
                    {
                        aux.Detalles.Add(new MDetalleLiquidacion()
                        {
                            Id = det.Id,
                            IdLiquidacion = det.IdLiquidacion,
                            IdReserva = det.IdReserva,
                            Monto = det.Monto
                        });
                    }
                    lista.Add(aux);
                }
            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
            }
            return lista;
        }

        [HttpGet]
        [Route("Listado/Detalles")]
        public List<MDetalleLiquidacion> ObtenerDetalleLiquidaciones()
        {
            List<MDetalleLiquidacion> lista = new List<MDetalleLiquidacion>();
            var liquidaciones = db.DetalleLiquidacion.ToList();
            try
            {
                foreach (var liq in liquidaciones)
                {
                    MDetalleLiquidacion aux = new MDetalleLiquidacion()
                    {
                        Id = liq.Id,
                        Monto = liq.Monto,
                        IdLiquidacion = liq.IdLiquidacion,
                        IdReserva = liq.IdReserva
                    };
                    lista.Add(aux);
                }
            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
            }
            return lista;
        }
    }
}
