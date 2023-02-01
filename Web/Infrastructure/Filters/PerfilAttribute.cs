using Simco.CrossInfraestructureLayer;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Simco.Infrastructure.Filters
{
    public class PerfilAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null || !httpContext.Request.IsAuthenticated)
                return false;

            Perfiles perfil = Perfiles.Ninguno;

            // 0 - Nombre Usuario, 1 - Nombre Completo, 2 - Perfil, 3 - Id Usuario, 4 - Id Persona, 5 - Tipo Usuario, 6 - Tipo Persona
            HttpCookie httpCookie = httpContext.Request.Cookies["SIMCONU"];
            if (httpCookie != null && httpCookie["Token"] != null)
            {
                string temp = Criptografia.Desencriptar(httpCookie["Token"]);
                if (!string.IsNullOrEmpty(temp))
                {
                    string[] data = temp.Split('|');
                    if (data.Length > 2)
                    {
                        switch (data[2].ToUpper())
                        {
                            case "ADMINISTRADOR": perfil = Perfiles.Administrador; break;
                            case "AGENTE": perfil = Perfiles.Agente; break;
                            case "ASESOR": perfil = Perfiles.Asesor; break;
                            case "ASESORCONSULTA": perfil = Perfiles.AsesorConsulta; break;
                            default: perfil = Perfiles.Ninguno; break;
                        }
                    }
                }
            }

            if (perfil == Perfiles.Ninguno)
                return false;

            string[] perfilesAdmitidos = Roles.Split(',');
            return perfilesAdmitidos.Any(perfilAdmitido => perfilAdmitido.ToUpper() == perfil.ToString().ToUpper());
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                    {
                        {"action", "CerrarSesion"},
                        {"controller", "InicioSesion"}
                    });

            //base.HandleUnauthorizedRequest(filterContext);
        }
    }
}