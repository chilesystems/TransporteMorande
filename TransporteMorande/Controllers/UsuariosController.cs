using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TransporteMorande.Models;
using TransporteMorande.Models.App;

namespace TransporteMorande.Controllers
{
    public class UsuariosController : Controller
    {
        private appmorandeEntities db = new appmorandeEntities();
        private UsuariosViewModel model = new UsuariosViewModel();
        private ApplicationUserManager _userManager;
        CloudStorageAccount storageAccount = null;
        CloudBlobContainer cloudBlobContainer = null;
        string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=transportemorandeapp;AccountKey=1zTOYuNzSnkTTK/Qm0CBhC4GZyFSQQjKpJcZaAjUIKWHmYzoSu9sFbr8jRToZvDC9A+TppyvICdENfjF/BSv3g==;EndpointSuffix=core.windows.net";


        public ActionResult Index(UsuariosViewModel model)
        {
            var dbUser = new ApplicationDbContext();
            if (model.PerfilSeleccionado == "Todos" || string.IsNullOrEmpty(model.PerfilSeleccionado))
            {
                model.Usuarios = dbUser.Users.ToList();
            }
            else
            {
                model.Usuarios = (from a in dbUser.Users
                                  from b in a.Roles
                                  where b.RoleId == model.PerfilSeleccionado
                                  select a
                                  ).ToList();
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult Nuevo(NuevoUsuarioFormModel model)
        {
            string uri = "";
            if (model.RolId == "Seleccione")
            {
                //return PartialView("../Shared/Mensaje", new Models.App.MensajeViewModel() { Mensaje = "Debe seleccionar un perfil", Error = true });
                     return Json(new { Retorno = "Debe seleccionar un perfil", Error = true }, JsonRequestBehavior.AllowGet);               
            }
            if (ModelState.IsValid)
            {                
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Android = model.Android,
                    Apellido = model.Apellido,
                    Imagen = uri,
                    Nombre = model.Nombre,
                    Rut = model.Rut
                };
                var result = UserManager.Create(user, model.Password);
                if (result.Succeeded)
                {
                    var roleresult = UserManager.AddToRole(user.Id, model.RolId);
                    if (roleresult.Succeeded)
                        //return PartialView("../Shared/Mensaje", new Models.App.MensajeViewModel() { Mensaje = "Debe seleccionar un perfil", Error = true });
                        return Json(new { Retorno = user.Id, Error = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Retorno = "La contraseña debe tener un largo mínimo de 6. y l menos 1 número.", Error = true }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Retorno = "La contraseña debe tener un largo mínimo de 6. y l menos 1 número.", Error = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Retorno = "Error", Error = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Modificar(NuevoUsuarioFormModel model)
        {
            string uri = "";
            if (model.RolIdModificar == "Seleccione")
            {
                return Json(new { Retorno = "Debe seleccionar un perfil", Error = true }, JsonRequestBehavior.AllowGet);
            }
            if (ModelState.IsValid)
            {
                
                var user = UserManager.FindById(model.Id);
                user.UserName = model.Email;
                user.Email = model.Email;
                user.Android = model.Android;
                user.Imagen = uri;
                user.Nombre = model.Nombre;
                user.Apellido = model.Apellido;
                user.Rut = model.Rut;
                var result = UserManager.Update(user);

                if (result.Succeeded)
                {
                    var dbUser = new ApplicationDbContext();
                    string rolId = user.Roles.ToList()[0].RoleId.ToUpper();
                    var rol = dbUser.Roles.FirstOrDefault(x => x.Id == rolId);

                    UserManager.RemoveFromRole(user.Id, rol.Name);
                        
                    var roleresult = UserManager.AddToRole(user.Id, model.RolIdModificar);
                    if (roleresult.Succeeded)
                        return Json(new { Retorno = user.Id, Error = false }, JsonRequestBehavior.AllowGet);
                    else return Json(new { Retorno = "Error al asignar un rol.", Error = true }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { Retorno = "La contraseña debe tener un largo mínimo de 6. y l menos 1 número.", Error = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Retorno = "Error de modelo", Error = true }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public PartialViewResult SubirImagen(string id, string tipo)
        {
            string mensaje = string.Empty;
            if (tipo == "Modificacion")
                mensaje = "Usuario modificado correctamente";
            else if(tipo == "Creacion") mensaje = "Usuario creado correctamente";
            if (Request.Files.Count > 0)
            {
                
                HttpPostedFileBase file = Request.Files[0];
                try
                {
                    var uri = ProcesarBlob(file);
                    var user = UserManager.FindById(id);
                    user.Imagen = uri;
                    var result = UserManager.Update(user);

                    //var result = UserManager.Create(user, model.Password);
                    if (result.Succeeded)
                    {
                        return PartialView("../Shared/Mensaje", new Models.App.MensajeViewModel() { Mensaje = mensaje, Error = false });
                    }

                }
                catch (Exception ex)
                {
                    return PartialView("../Shared/Mensaje", new Models.App.MensajeViewModel() { Mensaje = "Error " + ex.StackTrace, Error = true });
                }
            }
            return PartialView("../Shared/Mensaje", new Models.App.MensajeViewModel() { Mensaje = mensaje, Error = false });
        }
        //public async Task<ActionResult> ResetPassword(string id)
        //{
        //    var user = await UserManager.FindByIdAsync(id);
        //    string resetToken = await UserManager.GeneratePasswordResetTokenAsync(id);
        //    ResetPasswordViewModel model = new ResetPasswordViewModel();
        //    model.Email = user.Email;
        //    model.Code = resetToken;
        //    return resetToken == null ? View("Error") : View(model);
        //}

        ////
        //// POST: /Account/ResetPassword
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var user = await UserManager.FindByNameAsync(model.Email);
        //    if (user == null)
        //    {
        //        // No revelar que el usuario no existe
        //        return RedirectToAction("Index", "Usuarios");
        //    }
        //    var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("Index", "Usuarios");
        //    }
        //    return View();
        //}

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

        public ActionResult Create()
        {
            RegisterViewModel model = new RegisterViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {
            //if (ModelState.IsValid)
            //{
            //    var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            //    var result = await UserManager.CreateAsync(user, model.Password);
            //    if (result.Succeeded)
            //    {
            //        var usuario = new usuario
            //        {
            //            Apellidos = model.Apellido,
            //            Email = model.Email,
            //            idUsuario = Guid.NewGuid(),
            //            Nombre = model.Nombre,
            //            IdAspNetUser = user.Id
            //        };
            //        db.usuario.Add(usuario);
            //        db.SaveChanges();
            //        var roleresult = UserManager.AddToRole(user.Id, model.PerfilSeleccionado);
            //        if (roleresult.Succeeded)
            //            return RedirectToAction("Index", "Usuarios");
            //    }
            //    AddErrors(result);
            //}

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }

        public JsonResult EliminarUsuario(string id)
        {
            try
            {
                var user = UserManager.FindById(id);
                UserManager.Delete(user);
                return Json("Exito", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Obtener(string id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var user = UserManager.FindById(id);
            var dbUser = new ApplicationDbContext();
            string rolId = user.Roles.ToList()[0].RoleId.ToUpper();
            var rol = dbUser.Roles.FirstOrDefault(x => x.Id == rolId);
            return Json(new
            {
                nombre = user.Nombre,
                apellido = user.Apellido,
                rut = user.Rut,
                email = user.Email,
                android = user.Android,
                rolid = rol.Name,
                imagen = user.Imagen
            }, JsonRequestBehavior.AllowGet);
        }



        private string ProcesarBlob(HttpPostedFileBase archivo)
        {
            CloudBlockBlob cloudBlockBlob = ObtenerConexionAzureStorage().GetBlockBlobReference(archivo.FileName);
            cloudBlockBlob.UploadFromStream(archivo.InputStream);
            return cloudBlockBlob.Uri.ToString();
        }

        private CloudBlobContainer ObtenerConexionAzureStorage()
        {
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                
                cloudBlobContainer = cloudBlobClient.GetContainerReference("imagenes-usuarios");
                BlobContainerPermissions permissions = new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                    
                };
                cloudBlobContainer.SetPermissions(permissions);
                return cloudBlobContainer;
            }
            return null;
        }
    }
}