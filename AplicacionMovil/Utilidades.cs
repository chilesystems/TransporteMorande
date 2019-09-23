using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AplicacionMovil
{
    public class Utilidades
    {
        public static async Task performBlobOperation(string imagen, string ruta)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=transportemorandeapp;AccountKey=1zTOYuNzSnkTTK/Qm0CBhC4GZyFSQQjKpJcZaAjUIKWHmYzoSu9sFbr8jRToZvDC9A+TppyvICdENfjF/BSv3g==;EndpointSuffix=core.windows.net");

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("imagenesturismo");

            // Create the container if it doesn't already exist.
            await container.CreateIfNotExistsAsync();

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(imagen);
            var stream = File.OpenWrite(ruta);
            await blockBlob.DownloadToStreamAsync(stream);
        }

        public static string NombreBD()
        {
            return "TransporteMorande_30_08_2018.db";
        }
        public static string VerificaRut(string rut)
        {
            try
            {
                int i;
                string nueva = "";
                int auxiliar = 0;
                string devuelve = "";
                for (i = 0; i <= rut.Length - 2; i++)
                {
                    if (!rut[i].Equals('.') && !rut[i].Equals('-') && (i != (rut.Length - 1) && (i != (rut.Length - 1) && (!rut[i].Equals('K')))))
                        nueva = nueva + rut[i].ToString();
                }
                auxiliar = int.Parse(nueva);
                devuelve = digitoVerificador(auxiliar);
                if (!devuelve.Equals(rut[rut.Length - 1].ToString()))
                {
                    return "";
                }
                else
                    devuelve = OrdenaRut(nueva + rut[rut.Length - 1].ToString());
                return devuelve;
            }
            catch
            {
                return "";
            }
        }

        public static string VerificaRutConMensaje(string rut)
        {
            try
            {
                int i;
                string nueva = "";
                int auxiliar = 0;
                string devuelve = "";
                for (i = 0; i <= rut.Length - 2; i++)
                {
                    if (!rut[i].Equals('.') && !rut[i].Equals('-') && (i != (rut.Length - 1) && (i != (rut.Length - 1) && (!rut[i].Equals('K')))))
                        nueva = nueva + rut[i].ToString();
                }
                auxiliar = int.Parse(nueva);
                devuelve = digitoVerificador(auxiliar);
                if (!devuelve.Equals(rut[rut.Length - 1].ToString()))
                {
                    return "RUT Incorrecto";
                }
                else
                    devuelve = OrdenaRut(nueva + rut[rut.Length - 1].ToString());
                return devuelve;
            }
            catch
            {
                return "RUT Incorrecto";
            }
        }

        private static string digitoVerificador(int rut)
        {
            int Digito;
            int Contador;
            int Multiplo;
            int Acumulador;
            string RutDigito;
            Contador = 2;
            Acumulador = 0;
            while (rut != 0)
            {
                Multiplo = (rut % 10) * Contador;
                Acumulador = Acumulador + Multiplo;
                rut = rut / 10;
                Contador = Contador + 1;
                if (Contador == 8)
                {
                    Contador = 2;
                }
            }
            Digito = 11 - (Acumulador % 11);
            RutDigito = Digito.ToString().Trim();
            if (Digito == 10)
            {
                RutDigito = "K";
            }
            if (Digito == 11)
            {
                RutDigito = "0";
            }
            return (RutDigito);
        }

        private static string OrdenaRut(string palabrapg)
        {
            int i, j;
            int largo2, largo1;
            string palabranueva = "", auxiliar = palabrapg + "   ";
            if (palabrapg.Length == 9)
            {
                largo2 = palabrapg.Length;
                for (j = 0; j <= largo2 - 1; j++)
                {
                    if ((j == 2) || (j == 5))
                    {
                        palabranueva = palabranueva + ".";
                        palabranueva = palabranueva + auxiliar[j].ToString();
                    }
                    else if (j == 8)
                    {
                        palabranueva = palabranueva + "-";
                        palabranueva = palabranueva + auxiliar[j].ToString();
                    }
                    else
                        palabranueva = palabranueva + auxiliar[j].ToString();
                }
            }
            if (palabrapg.Length == 8)
            {
                largo1 = palabrapg.Length;
                for (i = 0; i <= largo1 - 1; i++)
                {
                    if ((i == 1) || (i == 4))
                    {
                        palabranueva = palabranueva + ".";
                        palabranueva = palabranueva + auxiliar[i].ToString();
                    }
                    else if (i == 7)
                    {
                        palabranueva = palabranueva + "-";
                        palabranueva = palabranueva + auxiliar[i].ToString();
                    }
                    else
                        palabranueva = palabranueva + auxiliar[i].ToString();
                }
            }
            return palabranueva;
        }

        public static string ponerpuntos(string monto2)
        {
            string decimales = obtenerdecimales(monto2);
            string monto = quitardecimales(monto2);
            //string monto = monto2;
            string aux = "";
            int cont = 0, cont2 = 1;
            while (cont < monto.Length)
            {
                cont++;
                cont2++;
                aux = monto[monto.Length - cont].ToString() + aux;
                if (cont2 == 4)
                {
                    if (!(cont == monto.Length))
                        aux = "." + aux;
                    cont2 = 1;
                }
            }
            if (decimales != monto2) return aux + "," + decimales;
            return aux;
        }

        private static string obtenerdecimales(string aux)
        {
            string monto = "";
            for (int i = aux.Length - 1; i >= 0; i--)
            {
                if (aux[i] == ',') break;
                monto = aux[i].ToString() + monto;
            }
            return monto;
        }

        private static string quitardecimales(string aux)
        {
            string montofinal = "";
            for (int i = 0; i < aux.Length; i++)
            {
                if (aux[i] == ',') break;
                montofinal = montofinal + aux[i].ToString();
            }
            return montofinal;
        }

        public static string sacarpuntos(string plata)
        {
            string diner = "";
            for (int i = 0; i < plata.Length; i++)
            {
                if (plata[i] != ',')
                {
                    if (!plata[i].Equals('.')) diner = diner + plata[i].ToString();
                }
                else
                    diner = diner + ",";
            }
            diner = sacacero(diner);
            return diner;
        }

        private static string sacacero(string numerocero)
        {
            string sincero = numerocero;
            int i;
            if (numerocero.Length == 1) return sincero;
            try
            {
                if (int.Parse(numerocero[0].ToString()) == 0)
                {
                    sincero = "";
                    for (i = 1; i < numerocero.Length; i++)
                        sincero = sincero + numerocero[i].ToString();
                    return sacacero(sincero);
                }
            }
            catch (Exception) { }
            return sincero;
        }
    }
}