using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AplicacionMovil.BLL;
using Newtonsoft.Json;

namespace AplicacionMovil
{
    [Activity(Label = "Transportes Morande",Theme = "@style/Theme.AppCompat.Light", MainLauncher = true)]
    public class Login : Activity
    {
        private Button botonLogin;
        private EditText textRut;
        private EditText textPass;
        private string baseURL = "http://api-transporemorande.azurewebsites.net/api/";
        //
        //private string baseURL = "http://localhost:61657/api/";
        private string sqlPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Utilidades.NombreBD());

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Login);
            botonLogin = FindViewById<Button>(Resource.Id.botonEntrar);
            textRut = FindViewById<EditText>(Resource.Id.textRUT);
            textPass = FindViewById<EditText>(Resource.Id.textPassword);
            var db = new SQLite.SQLiteConnection(sqlPath);
            db.CreateTable<MTrabajador>();
            botonLogin.Click += async delegate
            {
                string rut = textRut.Text;
                if (rut != "")
                {
                    string pass = textPass.Text;
                    textRut.Text = rut;
                    ProgressDialog progressBar = new ProgressDialog(this);
                    progressBar.SetCancelable(false);
                    progressBar.SetMessage("Cargando...");
                    progressBar.SetProgressStyle(ProgressDialogStyle.Spinner);
                    progressBar.Indeterminate = true;
                    progressBar.Show();

                    var trabajador = db.Table<MTrabajador>().Where(x => x.Email == rut && x.tempPassword == pass).FirstOrDefault();
                    /*doesn't exists on the local db*/
                    if (trabajador == null)
                    {
                        progressBar.SetMessage("Conectando...");
                        using (var client = new HttpClient())
                        {
                            try
                            {
                                string url = baseURL + "Trabajador/Login/" + rut + "/" + pass;
                                var result = await client.GetStringAsync(url);
                                var tra = JsonConvert.DeserializeObject<MTrabajador>(result);
                                progressBar.Hide();
                                if (tra.Logueado)
                                {
                                    db.Insert(tra);
                                    trabajador = tra;
                                    progressBar.Hide();
                                    var activityPrincipal = new Intent(this, typeof(MainActivity));
                                    activityPrincipal.PutExtra("trabajador", JsonConvert.SerializeObject(tra));
                                    StartActivity(activityPrincipal);
                                    Finish();
                                }
                                else
                                {
                                    RunOnUiThread(() => Toast.MakeText(this.BaseContext, "Error al entrar. Verifique sus credenciales.", ToastLength.Long).Show());
                                }
                            }
                            catch (Exception ex)
                            {
                                RunOnUiThread(() => Toast.MakeText(this.BaseContext, "Ocurrió el siguiente error: " + ex.Message, ToastLength.Long).Show());
                            }
                        }
                    }
                    else
                    {
                        //RunOnUiThread(() => Toast.MakeText(this.BaseContext, "Entré de manera local", ToastLength.Long).Show());
                        progressBar.Hide();
                        var activityPrincipal = new Intent(this, typeof(MainActivity));
                        activityPrincipal.PutExtra("trabajador", JsonConvert.SerializeObject(trabajador));
                        StartActivity(activityPrincipal);
                        Finish();
                    }
                }
                else
                {
                    RunOnUiThread(() => Toast.MakeText(this.BaseContext, "Correo incorrecto", ToastLength.Long).Show());
                }
            };
        }
    }
}