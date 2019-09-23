using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransporteMorande.Models
{
    public class Utilities
    {
        public static string ponerpuntos(string monto2)
        {
            string str1 = obtenerdecimales(monto2);
            string str2 = quitardecimales(monto2);
            string str3 = "";
            int num1 = 0;
            int num2 = 1;
            while (num1 < str2.Length)
            {
                ++num1;
                ++num2;
                string str4 = str2;
                str3 = str4[str4.Length - num1].ToString() + str3;
                if (num2 == 4)
                {
                    if (num1 != str2.Length)
                        str3 = "." + str3;
                    num2 = 1;
                }
            }
            if (str1 != monto2)
                return str3 + "," + str1;
            return str3;
        }

        public static string sacarpuntos(string plata)
        {
            string numerocero = "";
            for (int index = 0; index < plata.Length; ++index)
            {
                if ((int)plata[index] != 44)
                {
                    char ch = plata[index];
                    if (!ch.Equals('.'))
                    {
                        string str1 = numerocero;
                        ch = plata[index];
                        string str2 = ch.ToString();
                        numerocero = str1 + str2;
                    }
                }
                else
                    numerocero += ",";
            }
            return Utilities.sacacero(numerocero);
        }

        private static string sacacero(string numerocero)
        {
            string numerocero1 = numerocero;
            if (numerocero.Length == 1)
                return numerocero1;
            try
            {
                if (int.Parse(numerocero[0].ToString()) == 0)
                {
                    numerocero1 = "";
                    for (int index = 1; index < numerocero.Length; ++index)
                        numerocero1 += numerocero[index].ToString();
                    return Utilities.sacacero(numerocero1);
                }
            }
            catch (Exception ex)
            {
            }
            return numerocero1;
        }

        private static string quitardecimales(string aux)
        {
            string str = "";
            for (int index = 0; index < aux.Length && (int)aux[index] != 44; ++index)
                str += aux[index].ToString();
            return str;
        }

        private static string obtenerdecimales(string aux)
        {
            string str = "";
            for (int index = aux.Length - 1; index >= 0 && (int)aux[index] != 44; --index)
                str = aux[index].ToString() + str;
            return str;
        }

        public static string QuitarPuntos(string rut)
        {
            return rut.Replace(".", "");
        }
    }
}