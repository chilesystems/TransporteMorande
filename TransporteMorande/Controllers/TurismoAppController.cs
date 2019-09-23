using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransporteMorande.Models;
using System.Text.RegularExpressions;
using TransporteMorande.Models.App;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Data.Entity;

namespace TransporteMorande.Controllers
{
    public class TurismoAppController : Controller
    {
        appmorandeEntities db = new appmorandeEntities();
        TurismoViewModel model = new TurismoViewModel();

        CloudStorageAccount storageAccount = null;
        CloudBlobContainer cloudBlobContainer = null;
        string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=transportemorandeapp;AccountKey=1zTOYuNzSnkTTK/Qm0CBhC4GZyFSQQjKpJcZaAjUIKWHmYzoSu9sFbr8jRToZvDC9A+TppyvICdENfjF/BSv3g==;EndpointSuffix=core.windows.net";
        //string storageConnectionString = "UseDevelopmentStorage=true";
        public ActionResult Index()
        {
            model.Publicaciones = db.Servicio.Where(x => x.Activo && x.Web).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Modificar(TurismoFormModel Form)
        {
            Servicio aux = db.Servicio.First(x => x.Id == Form.Id);
            try
            {
                aux.Contenido = Form.Contenido;
                aux.Titulo = Form.Titulo;
                aux.Precio = Form.Precio;
                aux.Usuario = User.Identity.Name;
                aux.FechaIngreso = DateTime.Now;
                aux.Activo = true;
                aux.Web = true;
                db.Entry(aux).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                Form.Mensaje = "Error";
            }

            if (Request.Files.Count > 0 && Form.Imagen != null)
            {
                /*Intento eliminar la foto anterior*/
                Uri uri = new Uri(aux.Imagen);
                string filename = Path.GetFileName(uri.LocalPath);
                var blob = ObtenerConexionAzureStorage().GetBlockBlobReference(filename);
                blob.DeleteIfExists();

                HttpPostedFileBase file = this.Request.Files[0];
                string nuevoUri = ProcesarBlob(file);
                aux.Imagen = nuevoUri;
                db.Entry(aux).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Nuevo(TurismoFormModel Form)
        {
            if (Request.Files.Count > 0 && Form.Imagen != null)
            {
                HttpPostedFileBase file = this.Request.Files[0];
                try
                {
                    string uri = ProcesarBlob(file);

                    Servicio aux = new Servicio()
                    {
                        Contenido = Form.Contenido,
                        FechaIngreso = DateTime.Now,
                        Imagen = uri,
                        Precio = Form.Precio,
                        Titulo = Form.Titulo,
                        Usuario = User.Identity.Name,
                        Id = Guid.NewGuid()
                    };
                    db.Servicio.Add(aux);
                    db.SaveChanges();

                }
                catch
                {
                    Form.Mensaje = "Error";
                }

            }
            else
                Form.Mensaje = "Es obligatorio subir una fotografia";
            return RedirectToAction("Index");
        }

        public JsonResult Eliminar(Guid id)
        {
            Servicio aux = db.Servicio.First(x => x.Id == id);
            try
            {
                Uri uri = new Uri(aux.Imagen);
                string filename = Path.GetFileName(uri.LocalPath);
                var blob = ObtenerConexionAzureStorage().GetBlockBlobReference(filename);
                blob.DeleteIfExists();
                db.Servicio.Remove(aux);
                db.SaveChanges();
                return Json("Exito");

            }
            catch (Exception ex)
            {
                ViewData["message"] = ex.Message;
                ViewData["trace"] = ex.StackTrace;
                return Json("Error: " + ex.Message + " " + ex.StackTrace);
            }
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
                cloudBlobContainer = cloudBlobClient.GetContainerReference("imagenesturismo");
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