using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace TransporteMorande.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetNombreCompleto(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("NombreCompleto");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetNombre(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Nombre");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetNombreApellido(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Apellido");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetRUT(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Rut");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetEmail(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Email");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetImagen(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Imagen");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}