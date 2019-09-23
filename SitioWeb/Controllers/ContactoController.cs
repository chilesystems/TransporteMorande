using Newtonsoft.Json;
using SitioWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace SitioWeb.Controllers
{
    public class ContactoController : Controller
    {
        // GET: Contacto
        public ActionResult Index(string msg)
        {
            TempData.Keep();
            if (TempData["idioma"] == null)
            {
                TempData["idioma"] = "es";
            }
            return View(msg);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(CorreoFormModel model)
        {
            try
            {

                var response = Request["g-recaptcha-response"];
                const string secret = "6LeQ8VAUAAAAAMcOwl8LJqQAWEGDn4fvfCFKDRSr";
                var client = new WebClient();
                var reply =
                    client.DownloadString(
                        string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));

                var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);
                if (!captchaResponse.Success)
                {
                    if (captchaResponse.ErrorCodes.Count <= 0)
                        return View(new { msg = "Ocurrió un error." });

                    var error = captchaResponse.ErrorCodes[0].ToLower();
                    string msj = string.Empty;
                    switch (error)
                    {
                        case ("missing-input-secret"):
                            msj = "The secret parameter is missing.";
                            break;
                        case ("invalid-input-secret"):
                            msj = "The secret parameter is invalid or malformed.";
                            break;

                        case ("missing-input-response"):
                            msj = "Debe pasar el captcha.";
                            break;
                        case ("invalid-input-response"):
                            msj = "The response parameter is invalid or malformed.";
                            break;

                        default:
                            msj = "Ocurrió un error.";
                            break;
                    }
                    ModelState.AddModelError("ErrorCaptcha", msj);
                    return View(new { msg = msj });
                }
                string Cuerpo =
                    @"Ha recibido el siguiente mensaje de contacto: <br><br>
                <b>Nombre:</b> " + model.Nombre + @"<br>
                <b>Email:</b> " + model.Email + @"<br>
                <b>Teléfono:</b> " + model.Telefono + @"<br>
                <b>Mensaje: </b> " + model.Mensaje + @"<br><br></hr>
                <i>Este es un correo generado automáticamente. No responder. </i><br></hr>
                <h4>Sistemas ChileSystems</h4>";

                //MailMessage correo = new MailMessage("morandetransporteapp@gmail.com", "contacto@transportemorande.cl", "Contacto Sitio Web", Cuerpo);
                //correo.IsBodyHtml = true;
                //SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                //smtp.EnableSsl = true;
                //smtp.UseDefaultCredentials = false;
                //smtp.Port = 25;
                //smtp.Credentials = new System.Net.NetworkCredential("morandetransporteapp@gmail.com", "o9imoyax");
                //string retorno = "Contacto enviado correctamente.";
                //try
                //{
                //    smtp.Send(correo);
                //}
                //catch (Exception ex)
                //{
                //    retorno = ex.Message + ". " + ex.StackTrace + ". " + ex.InnerException.Message + ". " + ex.InnerException.ToString() + ". " + ex.InnerException.StackTrace;
                //}

                MailMessage correo = new MailMessage("sistemas@chilesystems.com", "contacto@transportemorande.cl", "Contacto Sitio Web", Cuerpo);
                correo.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient("smtpout.secureserver.net");
                smtp.EnableSsl = false;
                smtp.UseDefaultCredentials = false;
                smtp.Port = 3535;
                smtp.Credentials = new System.Net.NetworkCredential("sistemas@chilesystems.com", "o9imoyax");
                string retorno = "Contacto enviado correctamente.";
                try
                {
                    smtp.Send(correo);
                }
                catch (Exception ex)
                {
                    retorno = ex.Message + ". " + ex.StackTrace + ". " + ex.InnerException.Message + ". " + ex.InnerException.ToString() + ". " + ex.InnerException.StackTrace;
                }

                return View("Index", new { retorno });
            }
            catch (Exception ex)
            {
                string retorno = ex.Message + ". " + ex.StackTrace + ". " + ex.InnerException.Message + ". " + ex.InnerException.ToString() + ". " + ex.InnerException.StackTrace;
                return View("Index", new { retorno });
            }
        }


    }
}