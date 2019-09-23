using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AplicacionMovil.BLL;
using Newtonsoft.Json;
using static Android.App.DatePickerDialog;

namespace AplicacionMovil
{
    //@style/Theme.DesignDemo style/AppTheme.NoActionBar
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener, IDialogInterfaceOnMultiChoiceClickListener, IDialogInterfaceOnClickListener
    {
        MTrabajador trabajador;
        TextView textReservasPendientes, textReservasConfirmadas, textReservasPendientesDinero;
        TextView textLiquidacionesTotal;
        TextView textPanelTotalReservasPendientes, textPanelCantidadReservasPendientes;
        ProgressDialog progressSincronizacion;
        string[] opciones = { "Reservas", "Servicios", "Liquidaciones" };
        bool[] itemsChecked = new bool[3];
        private string sqlPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Utilidades.NombreBD());
        private SQLite.SQLiteConnection db;
        private bool syncClientes, syncPaises, syncIdiomas, syncServicios, syncReservas, syncLiquidaciones;
        private string baseURL = "http://api-transporemorande.azurewebsites.net/api/";
        DateTime panelLiquidacionesDesde, panelLiquidacionesHasta;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.floatingButtonAgregarReserva);
            fab.Click += FabOnClick;

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            setSincronizacionFalse();

            db = new SQLite.SQLiteConnection(sqlPath);
            db.CreateTable<MTrabajador>();
            db.CreateTable<MCliente>();
            db.CreateTable<MDomicilio>();
            db.CreateTable<MPais>();
            db.CreateTable<MIdioma>();
            db.CreateTable<MHospedaje>();
            db.CreateTable<MServicio1>();
            db.CreateTable<MReserva>();
            db.CreateTable<MLiquidacion>();
            db.CreateTable<MDetalleLiquidacion>();

            textReservasPendientes = FindViewById<TextView>(Resource.Id.textReservasPendientes);
            textReservasConfirmadas = FindViewById<TextView>(Resource.Id.textReservasConfirmadas);
            textReservasPendientesDinero = FindViewById<TextView>(Resource.Id.textReservasPendientesDinero);
            textLiquidacionesTotal = FindViewById<TextView>(Resource.Id.textPanelTotalPendientePago);
            textPanelTotalReservasPendientes = FindViewById<TextView>(Resource.Id.textPanelTotalReservasSinLiquidar);
            textPanelCantidadReservasPendientes = FindViewById<TextView>(Resource.Id.textPanelReservasSinLiquidar);
            panelLiquidacionesDesde = DateTime.Now.AddMonths(-1);
            panelLiquidacionesHasta = DateTime.Now.AddDays(1);

            /*trabajador = new MTrabajador()
            {
                Nombre = "Pablo",
                Apellido = "Marmol",
                RUT = "17.096.073-4",
                Email = "prueba@chilesystems.com",
                Logueado = true,
                UserName = "Pablo Marmol",
                Id = "500a9b91-4416-4cd9-80a6-16e123c18e18",
                Imagen = "",
                tempPassword = "123123morande"
            };*/

            /*string rutaImagenPerfil = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), serv.NombreArchivo);
            await Utilidades.performBlobOperation(serv.NombreArchivo, ruta);
            serv.Ruta = ruta;*/
            trabajador = JsonConvert.DeserializeObject<MTrabajador>(Intent.GetStringExtra("trabajador"));

            calculosPanel();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            View header = navigationView.GetHeaderView(0);
            TextView textNombreCliente = header.FindViewById<TextView>(Resource.Id.textNombreUsuario);
            textNombreCliente.Text = trabajador.Nombre + " " + trabajador.Apellido;
            TextView textCorreoCliente = header.FindViewById<TextView>(Resource.Id.textCorreoUsuario);
            textCorreoCliente.Text = trabajador.Email;
            
        }

        private void calculosPanel()
        {
            try {
                int ReservasPendientes = db.Table<MReserva>().Where(x => x.Estado == "Sin confirmar" && x.IdUsuario == trabajador.Id).Count();
                int ReservasConfirmadas = db.Table<MReserva>().Where(x => x.Estado == "Confirmada" && x.IdUsuario == trabajador.Id).Count();

                var resultado1 = db.Table<MReserva>().Where(x => x.EstadoPago == "Pendiente" && x.IdUsuario == trabajador.Id);
                int TotalPendienteReservas = resultado1.Any() ? resultado1.Sum(x => x.Total) : 0;

                int DineroLiquidacionesPendientes = db.Table<MLiquidacion>().Where(x => x.Estado == "Pendiente").Count();

                //var tablaReservas = db.Table
                var ReservasSinLiquidar = (from a in db.Table<MReserva>().ToList()
                                           join b in db.Table<MDetalleLiquidacion>().ToList() on a.Id equals b.IdReserva
                                           join c in db.Table<MLiquidacion>().ToList() on b.IdLiquidacion equals c.Id
                                           where a.IdUsuario == trabajador.Id && a.EstadoPagoEmpleador == "Pendiente" && a.Estado == "Finalizada"
                                           && c.Estado == "Pendiente"
                                           select a).Count();
                var TotalReservasSinLiquidar = (from a in db.Table<MReserva>().ToList()
                                                join b in db.Table<MDetalleLiquidacion>().ToList() on a.Id equals b.IdReserva
                                                join c in db.Table<MLiquidacion>().ToList() on b.IdLiquidacion equals c.Id
                                                where a.IdUsuario == trabajador.Id && a.EstadoPagoEmpleador == "Pendiente" && a.Estado == "Finalizada"
                                                && c.Estado == "Pendiente"
                                                select a).Sum(x => x.Total);


                textReservasPendientes.Text = Utilidades.ponerpuntos(ReservasPendientes.ToString());
                textReservasConfirmadas.Text = Utilidades.ponerpuntos(ReservasConfirmadas.ToString());
                textReservasPendientesDinero.Text = "$" + Utilidades.ponerpuntos(TotalPendienteReservas.ToString());


                textPanelTotalReservasPendientes.Text = "$" + Utilidades.ponerpuntos(TotalReservasSinLiquidar.ToString());
                textPanelCantidadReservasPendientes.Text = Utilidades.ponerpuntos(ReservasSinLiquidar.ToString());
                textLiquidacionesTotal.Text = "$" + Utilidades.ponerpuntos(DineroLiquidacionesPendientes.ToString());

            }
            catch
            {
                textReservasPendientes.Text = "0";
                textReservasConfirmadas.Text = "0";
                textReservasPendientesDinero.Text = "$" + "0";


                textPanelTotalReservasPendientes.Text = "$" + "0";
                textPanelCantidadReservasPendientes.Text = "0";
                textLiquidacionesTotal.Text = "$" + "0";

            }


        }

        private void PanelBotonHasta_Click(object sender, EventArgs e)
        {
            DateTime today = DateTime.Today;
            today = today.AddDays(2);
            DatePickerDialog dateDialog = new DatePickerDialog(this, OnDateSetHasta, today.Year, today.Month - 1, today.Day);
            dateDialog.DatePicker.MinDate = today.Millisecond;
            dateDialog.Show();
        }

        public void OnClick(IDialogInterface dialog, int which, bool isChecked)
        {
            itemsChecked[which] = isChecked;
        }

        protected override Dialog OnCreateDialog(int id)
        {
            switch (id)
            {
                case 0:
                    {
                        for (int i = 0; i < opciones.Length; i++) itemsChecked[i] = false;
                        return new Android.Support.V7.App.AlertDialog.Builder(this)
                            .SetIcon(Resource.Drawable.icon_sincronizar)
                            .SetTitle("Elementos a sincronizar")
                            .SetPositiveButton("OK", this)
                            .SetNegativeButton("Cancelar", this)
                            .SetMultiChoiceItems(opciones, itemsChecked, this)
                            .Create();
                    }
                default:
                    break;
            }
            return null;
        }

        private void PanelBotonDesde_Click(object sender, EventArgs e)
        {
            DateTime today = DateTime.Today;
            today = today.AddDays(2);
            DatePickerDialog dateDialog = new DatePickerDialog(this, OnDateSetDesde, today.Year, today.Month - 1, today.Day);
            dateDialog.DatePicker.MinDate = today.Millisecond;
            dateDialog.Show();
        }

        private void OnDateSetDesde(object sender, DateSetEventArgs e)
        {
            panelLiquidacionesDesde = e.Date;
        }

        private void OnDateSetHasta(object sender, DateSetEventArgs e)
        {
            panelLiquidacionesHasta = e.Date;
        }

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if (drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(this, typeof(ReservasNuevo2Activity));
            intent.PutExtra("trabajador", JsonConvert.SerializeObject(trabajador));
            StartActivityForResult(intent, 3);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 3 && resultCode == Result.Ok)
            {
                bool guardado = data.GetBooleanExtra("Guardado", false);
                if (guardado)
                {
                    /*Actualizar paneles*/
                }
            }
            calculosPanel();
        }

        //private async System.Threading.Tasks.Task obtenerDireccionesAsync()
        //{
        //    using (var client = new HttpClient())
        //    {
        //        try
        //        {
        //            var domNuevos = db.Table<MDomicilio>().Where(x => x.Nuevo).ToList();
        //            string contenidoNuevos = JsonConvert.SerializeObject(domNuevos);

        //            string newUrl = baseURL + "Clientes/Domicilios";

        //            var p = new Dictionary<string, string>
        //            {
        //                { "nuevos", contenidoNuevos }
        //            };
        //            var content = new FormUrlEncodedContent(p);
        //            var response = await client.PostAsync(newUrl, content);

        //            var responseString = await response.Content.ReadAsStringAsync();

        //            List<MDomicilio> domicilios = JsonConvert.DeserializeObject<List<MDomicilio>>(responseString);
        //            db.DeleteAll<MDomicilio>();
        //            if (domicilios != null && domicilios.Count > 0)
        //                db.InsertAll(domicilios);
        //            sincronizarReservas();
        //        }
        //        catch (System.Exception ex)
        //        {
        //            RunOnUiThread(() => Toast.MakeText(this.BaseContext, "Ocurrió el siguiente error al sincronizar domicilios: " + ex.Message, ToastLength.Long).Show());
        //        }
        //    }
        //}

        private async System.Threading.Tasks.Task obtenerPaisesAsync()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    string url = baseURL + "Config/Paises";
                    var result = await client.GetStringAsync(url);
                    List<MPais> paises = JsonConvert.DeserializeObject<List<MPais>>(result);
                    db.DeleteAll<MPais>();
                    if (paises != null && paises.Count > 0)
                        db.InsertAll(paises);
                    syncPaises = true;
                    sincronizarReservas();
                }
                catch (System.Exception ex)
                {
                    RunOnUiThread(() => Toast.MakeText(this.BaseContext, "Ocurrió el siguiente error al sincronizar paises: " + ex.Message, ToastLength.Long).Show());
                }
            }
        }

        private async System.Threading.Tasks.Task obtenerClientesAsync()
        {

            using (var client = new HttpClient())
            {
                try
                {
                    var clientesNuevos = db.Table<MCliente>().Where(x => x.Nuevo).ToList();
                    string contenidoNuevos = JsonConvert.SerializeObject(clientesNuevos);

                    var clientesModificados = db.Table<MCliente>().Where(x => x.Modificado).ToList();
                    string contenidoModificados = JsonConvert.SerializeObject(clientesModificados);

                    string newUrl = baseURL + "Clientes/Actualizar";

                    var p = new Dictionary<string, string>
                        {
                           { "nuevos", contenidoNuevos },
                            { "modificados", contenidoModificados }
                        };
                    var content = new FormUrlEncodedContent(p);
                    var response = await client.PostAsync(newUrl, content);

                    var responseString = await response.Content.ReadAsStringAsync();

                    List<MCliente> clientes = JsonConvert.DeserializeObject<List<MCliente>>(responseString);
                    db.DeleteAll<MCliente>();
                    if (clientes != null && clientes.Count > 0)
                    {
                        db.InsertAll(clientes);
                    }
                    syncClientes = true;
                    sincronizarReservas();                   
                }
                catch (System.Exception ex)
                {
                    RunOnUiThread(() => Toast.MakeText(this.BaseContext, "Ocurrió el siguiente error al sincronizar nuevas ventas: " + ex.Message, ToastLength.Long).Show());
                }
            }
        }

        private async System.Threading.Tasks.Task obtenerIdiomasAsync()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    string url = baseURL + "Config/Idiomas";
                    var result = await client.GetStringAsync(url);
                    List<MIdioma> idiomas = JsonConvert.DeserializeObject<List<MIdioma>>(result);
                    db.DeleteAll<MIdioma>();
                    if (idiomas != null && idiomas.Count > 0)
                        db.InsertAll(idiomas);
                    syncIdiomas = true;
                }
                catch (System.Exception ex)
                {
                    RunOnUiThread(() => Toast.MakeText(this.BaseContext, "Ocurrió el siguiente error al sincronizar idiomas: " + ex.Message, ToastLength.Long).Show());
                }
            }
            sincronizarReservas();
        }

        //private async System.Threading.Tasks.Task obtenerHospedajesAsync()
        //{

        //    using (var client = new HttpClient())
        //    {
        //        try
        //        {
        //            var objNuevos = db.Table<MHospedaje>().Where(x => x.Nuevo).ToList();
        //            string contenidoNuevos = JsonConvert.SerializeObject(objNuevos);

        //            string newUrl = baseURL + "Hospedaje/Actualizar";

        //            var p = new Dictionary<string, string>
        //                {
        //                   { "nuevos", contenidoNuevos }
        //                };
        //            var content = new FormUrlEncodedContent(p);
        //            var response = await client.PostAsync(newUrl, content);

        //            var responseString = await response.Content.ReadAsStringAsync();

        //            List<MHospedaje> objetos = JsonConvert.DeserializeObject<List<MHospedaje>>(responseString);
        //            db.DeleteAll<MHospedaje>();
        //            if (objetos != null && objetos.Count > 0)
        //            {
        //                db.InsertAll(objetos);
        //            }
        //        }
        //        catch (System.Exception ex)
        //        {
        //            RunOnUiThread(() => Toast.MakeText(this.BaseContext, "Ocurrió el siguiente error al sincronizar nuevas ventas: " + ex.Message, ToastLength.Long).Show());
        //        }
        //    }
        //    sincronizarReservas();
        //}

        private async System.Threading.Tasks.Task obtenerServiciosAsync()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    string url = baseURL + "Servicios/Listado";
                    var result = await client.GetStringAsync(url);
                    List<MServicio1> servicios = JsonConvert.DeserializeObject<List<MServicio1>>(result);
                    db.DeleteAll<MServicio1>();
                    
                    foreach (var serv in servicios)
                    {
                        try
                        {
                            string ruta = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), serv.NombreArchivo);
                            await Utilidades.performBlobOperation(serv.NombreArchivo, ruta);
                            serv.Ruta = ruta;
                        }
                        catch { }
                        
                    }
                    if (servicios != null && servicios.Count > 0)
                        db.InsertAll(servicios);
                    syncServicios = true;
                    TerminoSincronizacion();
                }
                catch (System.Exception ex)
                {
                    RunOnUiThread(() => Toast.MakeText(this.BaseContext, "Ocurrió el siguiente error al sincronizar idiomas: " + ex.Message, ToastLength.Long).Show());
                }
            }
        }

        private async System.Threading.Tasks.Task obtenerReservasAsync()
        {

            using (var client = new HttpClient())
            {
                try
                {
                    var reservasNuevas = db.Table<MReserva>().Where(x => x.Nuevo && x.IdUsuario == trabajador.Id && x.Estado != "Cancelada").ToList();
                    string reservasNuevasJson = JsonConvert.SerializeObject(reservasNuevas);

                    var reservasModificadas = db.Table<MReserva>().Where(x => x.Modificado && x.IdUsuario == trabajador.Id && x.Estado != "Cancelada").ToList();
                    string reservasModificadasJson = JsonConvert.SerializeObject(reservasModificadas);

                    string newUrl = baseURL + "Reservas/Actualizar";

                    var p = new Dictionary<string, string>
                        {
                            { "nuevos", reservasNuevasJson },
                            { "modificados", reservasModificadasJson },
                            { "idUsuario", trabajador.Id.ToString()}
                        };
                    var content = new FormUrlEncodedContent(p);
                    var response = await client.PostAsync(newUrl, content);

                    var responseString = await response.Content.ReadAsStringAsync();

                    List<MReserva> reservas = JsonConvert.DeserializeObject<List<MReserva>>(responseString);
                    if (reservas != null && reservas.Count > 0)
                    {
                        var reservasEliminar = db.Table<MReserva>().Where(x => x.IdUsuario == trabajador.Id);
                        foreach (var reserva in reservasEliminar)
                        {
                            db.Delete(reserva);
                        }
                        db.InsertAll(reservas);
                    }
                    syncReservas = true;
                    TerminoSincronizacion();
                }
                catch (System.Exception ex)
                {
                    RunOnUiThread(() => Toast.MakeText(this.BaseContext, "Ocurrió el siguiente error al sincronizar nuevas ventas: " + ex.Message, ToastLength.Long).Show());
                }
            }
            TerminoSincronizacion();
        }

        private async System.Threading.Tasks.Task obtenerLiquidacionesAsync()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    string url = baseURL + "Liquidaciones/Listado/" + trabajador.Id;
                    var result = await client.GetStringAsync(url);
                    List<MLiquidacion> liquidaciones = JsonConvert.DeserializeObject<List<MLiquidacion>>(result);
                    db.DeleteAll<MLiquidacion>();
                    db.DeleteAll<MDetalleLiquidacion>();
                    foreach (var a in liquidaciones)
                    {
                        db.Insert(a);
                        foreach (var b in a.Detalles)
                        {
                            db.Insert(b);
                        }
                    }
                    syncLiquidaciones = true;
                    TerminoSincronizacion();
                }
                catch (System.Exception ex)
                {
                    RunOnUiThread(() => Toast.MakeText(this.BaseContext, "Ocurrió el siguiente error al sincronizar idiomas: " + ex.Message, ToastLength.Long).Show());
                }
            }
        }

        private void TerminoSincronizacion()
        {
            if (syncServicios && syncReservas && syncClientes && syncPaises && syncIdiomas && syncLiquidaciones)
            {
                progressSincronizacion.Hide();
                Toast.MakeText(this, "Sincronización Completada", ToastLength.Short).Show();
                setSincronizacionFalse();
                calculosPanel();
            }
        }

        private void sincronizarReservas()
        {
            if (itemsChecked[0] && syncClientes && syncPaises && syncIdiomas)
            {
                obtenerReservasAsync();
            }
            if (!itemsChecked[0])
            {
                syncReservas = true;
                TerminoSincronizacion();
            }
        }

        private void setSincronizacionFalse()
        {
            syncLiquidaciones = 
            syncReservas =
            syncServicios =
            syncIdiomas =
            syncClientes =
            syncPaises = false;
        }


        bool NavigationView.IOnNavigationItemSelectedListener.OnNavigationItemSelected(IMenuItem menuItem)
        {
            int id = menuItem.ItemId;

            if (id == Resource.Id.nav_reservas)
            {
                var activityReservas = new Intent(this, typeof(ReservasActivity));
                activityReservas.PutExtra("trabajador", JsonConvert.SerializeObject(trabajador));
                StartActivityForResult(activityReservas, 3);
            }
            else if (id == Resource.Id.nav_liquidaciones)
            {

            }
            else if (id == Resource.Id.nav_clientes)
            {
                var activityClientes = new Intent(this, typeof(ClientesActivity));
                StartActivity(activityClientes);
            }
            //else if (id == Resource.Id.nav_hospedajes)
            //{
            //    var activityHospedaje = new Intent(this, typeof(HospedajeActivity));
            //    StartActivity(activityHospedaje);
            //}
            else if (id == Resource.Id.nav_servicios)
            {
                var activityHospedaje = new Intent(this, typeof(ServiciosActivity));
                StartActivity(activityHospedaje);
            }
            else if (id == Resource.Id.nav_Sincronizar)
            {
                ShowDialog(0);                
            }
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }

        public void OnClick(IDialogInterface dialog, int which)
        {
            if (which == -1) //OK BUTTON
            {
                progressSincronizacion = new ProgressDialog(this);
                progressSincronizacion.SetCancelable(false);
                progressSincronizacion.SetProgressStyle(ProgressDialogStyle.Spinner);
                progressSincronizacion.Indeterminate = true;
                progressSincronizacion.Show();
                progressSincronizacion.SetMessage("Sincronizando ...");

                obtenerClientesAsync();
                obtenerPaisesAsync();
                obtenerIdiomasAsync();


                if (!itemsChecked[0]) //Reservas
                {
                    syncReservas = true;
                }
                if (itemsChecked[1]) //Servicios
                {
                    obtenerServiciosAsync();
                }
                else
                {
                    syncServicios = true;
                }
                if (itemsChecked[2]) //Liquidaciones
                {
                    obtenerLiquidacionesAsync();
                }
                else
                {
                    syncLiquidaciones = true;
                }
                TerminoSincronizacion();
            }
        }

        

    }
}

