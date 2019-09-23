using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TransporteMorande.Models;
using TransporteMorande.Models.App;

namespace TransporteMorande.Controllers
{
    [Authorize]
    public class UsuarioController : Controller
    {
        private appmorandeEntities db = new appmorandeEntities();
        private ApplicationUserManager _userManager;
        private PerfilViewModel model = new PerfilViewModel();

        public async System.Threading.Tasks.Task<ActionResult> Perfil()
        {
            model.Usuario = await UserManager.FindByNameAsync(User.Identity.Name);
            var dbUser = new ApplicationDbContext();
            var Roles = dbUser.Roles.ToList();
            var rolUsuario = model.Usuario.Roles.First();
            model.Rol = Roles.First(x => x.Id == rolUsuario.RoleId).Name;
            model.ViajesRealizados = db.Reserva.Where(x => x.Estado == "Finalizada" && x.IdUsuario == model.Usuario.Id).OrderBy(x => x.FechaSalida).Take(10).ToList();
            model.Reservas = db.Reserva.Where(x => x.Estado != "Finalizada" && x.IdUsuario == model.Usuario.Id).OrderBy(x => x.FechaSalida).Take(10).ToList();

            model.ReservasConfirmadas = db.Reserva.Where(x => x.Estado == "Confirmada" && x.IdUsuario == model.Usuario.Id).Count();
            model.ReservasPendientes = db.Reserva.Where(x => x.Estado == "Sin confirmar" && x.IdUsuario == model.Usuario.Id).Count();
            var totalPendienteReservas = db.Reserva.Where(x => x.EstadoPago == "Pendiente" && x.IdUsuario == model.Usuario.Id);
            model.TotalPendienteReservas = totalPendienteReservas.Any() ? totalPendienteReservas.Sum(x => x.Total) : 0;

            model.Liquidaciones = db.Liquidacion.Where(x=>x.IdVendedor == model.Usuario.Id).OrderBy(x => x.Fecha).Take(10).ToList();
            model.CantidadLiquidacionesPendientes = db.Liquidacion.Where(x => x.Estado == "Pendiente").Count();
            var totalLiquidacionesPendientes = db.Liquidacion.Where(x => x.Estado == "Pendiente");
            model.TotalLiquidacionesPendientes = totalLiquidacionesPendientes.Any() ? totalLiquidacionesPendientes.Sum(x => x.Monto) : 0;

            return View(model);
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [HttpPost]
        public async Task<PartialViewResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("../Shared/Mensaje", new Models.App.MensajeViewModel() { Mensaje = "Verificar los datos.", Error = true });
            }
            var user = await UserManager.FindByNameAsync(User.Identity.Name);
            string resetToken = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var result = await UserManager.ResetPasswordAsync(user.Id, resetToken, model.Password);
            if (result.Succeeded)
            {
                var usuarioDb = db.AspNetUsers.First(x => x.Id == user.Id);
                usuarioDb.Pass = model.Password;
                db.Entry(usuarioDb).State = EntityState.Modified;
                db.SaveChanges();
                return PartialView("../Shared/Mensaje", new Models.App.MensajeViewModel() { Mensaje = "Contraseña restablecida correctamente", Error = false });
            }
            return PartialView("../Shared/Mensaje", new Models.App.MensajeViewModel() { Mensaje = "Error", Error = true });
        }

    }
}