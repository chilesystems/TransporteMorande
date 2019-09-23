using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransporteMorande.Models;
using TransporteMorande.Models.App;

namespace TransporteMorande.Controllers
{
    [Authorize]
    public class SliderController : Controller
    {
        appmorandeEntities db = new appmorandeEntities();
        SliderViewModel model = new SliderViewModel();

        CloudStorageAccount storageAccount = null;
        CloudBlobContainer cloudBlobContainer = null;
        string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=transportemorandeapp;AccountKey=1zTOYuNzSnkTTK/Qm0CBhC4GZyFSQQjKpJcZaAjUIKWHmYzoSu9sFbr8jRToZvDC9A+TppyvICdENfjF/BSv3g==;EndpointSuffix=core.windows.net";
        //string storageConnectionString = "UseDevelopmentStorage=true";

        public ActionResult Index()
        {
            model.Sliders = db.Slider.ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Nuevo(SliderFormModel Form)
        {
            if (Request.Files.Count > 0 && Form.Imagen != null)
            {
                HttpPostedFileBase file = this.Request.Files[0];
                try
                {
                    string uri = ProcesarBlob(file);

                    Slider aux = new Slider()
                    {
                        FechaIngreso = DateTime.Now,
                        Imagen = uri,
                        Titulo = Form.Titulo,
                        Usuario = User.Identity.Name,
                        Id = Guid.NewGuid()                        
                    };
                    db.Slider.Add(aux);
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

        [HttpPost]
        public ActionResult Modificar(SliderFormModel Form)
        {
            if (Request.Files.Count > 0 && Form.Imagen != null)
            {
                HttpPostedFileBase file = this.Request.Files[0];
                Slider aux = db.Slider.First(x => x.Id == Form.Id);
                Uri uri = new Uri(aux.Imagen);
                string filename = Path.GetFileName(uri.LocalPath);
                var blob = ObtenerConexionAzureStorage().GetBlockBlobReference(filename);
                blob.DeleteIfExists();
                try
                {
                    string nuevoUri = ProcesarBlob(file);
                    aux.Titulo = Form.Titulo;
                    aux.Imagen = nuevoUri;
                    aux.Usuario = User.Identity.Name;
                    aux.FechaIngreso = DateTime.Now;
                    db.Entry(aux).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch
                {
                    Form.Mensaje = "Error";
                }
                db.SaveChanges();
            }
            else
                Form.Mensaje = "Es obligatorio subir una fotografia";
            return RedirectToAction("Index");
        }

        public JsonResult Eliminar(Guid id)
        {
            Slider aux = db.Slider.First(x => x.Id == id);
            try
            {
                Uri uri = new Uri(aux.Imagen);
                string filename = Path.GetFileName(uri.LocalPath);
                var blob = ObtenerConexionAzureStorage().GetBlockBlobReference(filename);
                blob.DeleteIfExists();
                db.Slider.Remove(aux);
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
                cloudBlobContainer = cloudBlobClient.GetContainerReference("imagenesslider");
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