using log4net;
using Simco.ApplicationLayer.Servicios;
using Simco.CrossInfraestructureLayer;
using Simco.DomainModelLayer.ViewModels;
using Simco.Infrastructure.Notification;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using System.Net;
using Newtonsoft.Json;
using Simco.Models;

namespace Simco.Controllers
{
    public abstract class BaseController : Controller
    {
        private static readonly ILog log = SimcoExcepcion.GetLogger(typeof(BaseController));

        public string NombreUsuario
        {
            get
            {
                // 0 - Nombre Usuario, 1 - Nombre Completo, 2 - Perfil, 3 - Id Usuario, 4 - Id Persona, 5 - Tipo Usuario, 6 - Tipo Persona, 7 - Estado Usuario
                HttpCookie httpCookie = Request.Cookies["SIMCONU"];
                if (httpCookie != null && httpCookie["Token"] != null)
                {
                    string temp = Criptografia.Desencriptar(httpCookie["Token"]);
                    if (!string.IsNullOrEmpty(temp))
                    {
                        string[] data = temp.Split('|');
                        if (data.Length > 0)
                        {
                            return data[0];
                        }
                    }
                }

                return string.Empty;
            }
        }

        public string NombreCompleto
        {
            get
            {
                // 0 - Nombre Usuario, 1 - Nombre Completo, 2 - Perfil, 3 - Id Usuario, 4 - Id Persona, 5 - Tipo Usuario, 6 - Tipo Persona, 7 - Estado Usuario
                HttpCookie httpCookie = Request.Cookies["SIMCONU"];
                if (httpCookie != null && httpCookie["Token"] != null)
                {
                    string temp = Criptografia.Desencriptar(httpCookie["Token"]);
                    if (!string.IsNullOrEmpty(temp))
                    {
                        string[] data = temp.Split('|');
                        if (data.Length > 1)
                        {
                            return data[1];
                        }
                    }
                }

                return string.Empty;
            }
        }

        public Perfiles Perfil
        {
            get
            {
                // 0 - Nombre Usuario, 1 - Nombre Completo, 2 - Perfil, 3 - Id Usuario, 4 - Id Persona, 5 - Tipo Usuario, 6 - Tipo Persona, 7 - Estado Usuario
                HttpCookie httpCookie = Request.Cookies["SIMCONU"];
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
                                case "ADMINISTRADOR": return Perfiles.Administrador;
                                case "AGENTE": return Perfiles.Agente;
                                case "ASESOR": return Perfiles.Asesor;
                                case "ASESORCONSULTA": return Perfiles.AsesorConsulta;
                                default: return Perfiles.Ninguno;
                            }
                        }
                    }
                }

                return Perfiles.Ninguno;
            }
        }

        public int UsuarioId
        {
            get
            {
                // 0 - Nombre Usuario, 1 - Nombre Completo, 2 - Perfil, 3 - Id Usuario, 4 - Id Persona, 5 - Tipo Usuario, 6 - Tipo Persona, 7 - Estado Usuario
                int usuarioId = 0;
                HttpCookie httpCookie = Request.Cookies["SIMCONU"];
                if (httpCookie != null && httpCookie["Token"] != null)
                {
                    string temp = Criptografia.Desencriptar(httpCookie["Token"]);
                    if (!string.IsNullOrEmpty(temp))
                    {
                        string[] data = temp.Split('|');
                        if (data.Length > 3)
                        {
                            if (!string.IsNullOrEmpty(data[3]))
                                usuarioId = Convert.ToInt32(data[3]);
                        }
                    }
                }

                return usuarioId;
            }
        }

        public int PersonaId
        {
            get
            {
                // 0 - Nombre Usuario, 1 - Nombre Completo, 2 - Perfil, 3 - Id Usuario, 4 - Id Persona, 5 - Tipo Usuario, 6 - Tipo Persona, 7 - Estado Usuario
                int personaId = -1;
                HttpCookie httpCookie = Request.Cookies["SIMCONU"];
                if (httpCookie != null && httpCookie["Token"] != null)
                {
                    string temp = Criptografia.Desencriptar(httpCookie["Token"]);
                    if (!string.IsNullOrEmpty(temp))
                    {
                        string[] data = temp.Split('|');
                        if (data.Length > 4)
                        {
                            if (!string.IsNullOrEmpty(data[4]))
                                personaId = Convert.ToInt32(data[4]);
                        }
                    }
                }

                return personaId;
            }
        }

        public string TipoPersona
        {
            get
            {
                // 0 - Nombre Usuario, 1 - Nombre Completo, 2 - Perfil, 3 - Id Usuario, 4 - Id Persona, 5 - Tipo Usuario, 6 - Tipo Persona, 7 - Estado Usuario
                HttpCookie httpCookie = Request.Cookies["SIMCONU"];
                if (httpCookie != null && httpCookie["Token"] != null)
                {
                    string temp = Criptografia.Desencriptar(httpCookie["Token"]);
                    if (!string.IsNullOrEmpty(temp))
                    {
                        string[] data = temp.Split('|');
                        if (data.Length > 5)
                        {
                            return data[5];
                        }
                    }
                }

                return string.Empty;
            }
        }

        public string TipoAgente
        {
            get
            {
                // 0 - Nombre Usuario, 1 - Nombre Completo, 2 - Perfil, 3 - Id Usuario, 4 - Id Persona, 5 - Tipo Usuario, 6 - Tipo Agente, 7 - Estado Usuario
                HttpCookie httpCookie = Request.Cookies["SIMCONU"];
                if (httpCookie != null && httpCookie["Token"] != null)
                {
                    string temp = Criptografia.Desencriptar(httpCookie["Token"]);
                    if (!string.IsNullOrEmpty(temp))
                    {
                        string[] data = temp.Split('|');
                        if (data.Length > 6)
                        {
                            return data[6];
                        }
                    }
                }

                return string.Empty;
            }
        }

        public string EstadoUsuario
        {
            get
            {
                // 0 - Nombre Usuario, 1 - Nombre Completo, 2 - Perfil, 3 - Id Usuario, 4 - Id Persona, 5 - Tipo Usuario, 6 - Tipo Persona, 7 - Estado Usuario
                HttpCookie httpCookie = Request.Cookies["SIMCONU"];
                if (httpCookie != null && httpCookie["Token"] != null)
                {
                    string temp = Criptografia.Desencriptar(httpCookie["Token"]);
                    if (!string.IsNullOrEmpty(temp))
                    {
                        string[] data = temp.Split('|');
                        if (data.Length > 7)
                        {
                            return data[7];
                        }
                    }
                }

                return string.Empty;
            }
        }

        public int PerfilId
        {
            get
            {
                // 0 - Nombre Usuario, 1 - Nombre Completo, 2 - Perfil, 3 - Id Usuario, 4 - Id Persona, 5 - Tipo Usuario, 6 - Tipo Persona, 7 - Estado Usuario, 8- PerfilId
                HttpCookie httpCookie = Request.Cookies["SIMCONU"];
                if (httpCookie != null && httpCookie["Token"] != null)
                {
                    string temp = Criptografia.Desencriptar(httpCookie["Token"]);
                    if (!string.IsNullOrEmpty(temp))
                    {
                        string[] data = temp.Split('|');
                        if (data.Length > 8)
                        {
                            return int.Parse(data[8]);
                        }
                    }
                }

                return 0;
            }
        }

        public ActionResult ObtenerSesion()
        {
            HttpCookie httpCookie = Request.Cookies["SIMCONU"];
            if (httpCookie != null && httpCookie["UserName"] != null)
            {
                InicioSesionModel inicioSesionModel = new InicioSesionModel
                {
                    CorreoElectronico = httpCookie["UserName"],
                    Recordarme = true
                };

                return View(inicioSesionModel);
            }

            return View();
        }

        public InicioSesionModel ObtenerSesionModel()
        {
            InicioSesionModel inicioSesionModel = new InicioSesionModel();

            HttpCookie httpCookie = Request.Cookies["SIMCONU"];
            if (httpCookie != null && httpCookie["UserName"] != null)
            {
                inicioSesionModel.CorreoElectronico = httpCookie["UserName"];
                inicioSesionModel.Recordarme = true;
            }

            return inicioSesionModel;
        }

        public void ObtenerSesionModel(ref InicioSesionModel inicioSesionModel)
        {
            HttpCookie httpCookie = Request.Cookies["SIMCONU"];
            if (httpCookie != null && httpCookie["UserName"] != null)
            {
                inicioSesionModel.CorreoElectronico = httpCookie["UserName"];
                inicioSesionModel.Recordarme = true;
            }
        }

        public ActionResult IniciarSesion(InicioSesionModel inicioSesionModel, bool redireccionar)
        {
            if (!String.IsNullOrEmpty(inicioSesionModel.CorreoElectronico) && !String.IsNullOrEmpty(inicioSesionModel.Contrasena))
            {
                try
                {
                    string correoElectronico = inicioSesionModel.CorreoElectronico;
                    string nombreCompleto;
                    string tipoPersona;
                    string tipoAgente;
                    string perfil;
                    string estadoUsuario;
                    int idUsuario;
                    int idPersona;
                    int idPerfil;
                    bool cambiarContrasena;

                    ServicioUsuarios servicioUsuariosPerfiles = new ServicioUsuarios();
                    if (servicioUsuariosPerfiles.AutenticaticarUsuario(inicioSesionModel, out nombreCompleto, out perfil, out idPerfil, out idUsuario, out idPersona, out tipoPersona, out tipoAgente, out estadoUsuario, out cambiarContrasena))
                    {
                        RegistrarCacheSesion(correoElectronico, nombreCompleto, perfil, idUsuario.ToString(), idPersona.ToString(), tipoPersona, tipoAgente, estadoUsuario, idPerfil);
                        FormsAuthentication.SetAuthCookie(nombreCompleto, false);

                        if (redireccionar)
                        {
                            if (cambiarContrasena)
                                this.ShowMessage(MessageType.Information, Constante.ErrorUsuarioCambioContrasena, true);

                            return RedirectToActionPermanent("Index", "Inicio");
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return RedirectToActionPermanent("Index");
        }

        public bool ActualizarContrasena(ContrasenaCambiarModel contrasenaCambiarModel)
        {
            try
            {
                ServicioUsuarios servicioUsuariosPerfiles = new ServicioUsuarios();
                servicioUsuariosPerfiles.CambiarContraseña(contrasenaCambiarModel, UsuarioId);

                return true;
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return false;
        }

        public bool ActualizarCorreoElectronico(CorreoElectronicoCambiarModel correoElectronicoCambiarModel)
        {
            try
            {
                ServicioUsuarios servicioUsuariosPerfiles = new ServicioUsuarios();
                servicioUsuariosPerfiles.CambiarCorreoElectronico(correoElectronicoCambiarModel, UsuarioId);

                return true;
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return false;
        }

        public ActionResult CerrarSesion()
        {
            if (Request.Cookies["SIMCONU"] != null)
                Response.Cookies.Remove("SIMCONU");

            //Request.Cookies.Clear();
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Municipios(string zonaPadreId)
        {
            ServicioZonasGeograficas servicioZonasGeograficas = new ServicioZonasGeograficas();
            return Json(servicioZonasGeograficas.ObtenerZonasGeograficas(zonaPadreId), JsonRequestBehavior.AllowGet);
        }

        public static CaptchaResponse ValidateCaptcha(string response)
        {
            string secret = WebConfigurationManager.AppSettings["ReCaptchaPrivateKey"];
            var client = new WebClient();
            var jsonResult = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
            return JsonConvert.DeserializeObject<CaptchaResponse>(jsonResult.ToString());
        }

        public void RegistrarCacheSesion(string correoUsuario, string nombreCompleto, string perfil, string idUsuario, string idPersona, string tipoPersona, string tipoAgente, string estadoUsuario, int idPerfil)
        {
            HttpCookie httpCookie = new HttpCookie("SIMCONU");
            httpCookie["Token"] = Criptografia.Encriptar(string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}", correoUsuario, nombreCompleto, perfil, idUsuario, idPersona, tipoPersona, tipoAgente, estadoUsuario, idPerfil.ToString())); // Mantener este orden
            httpCookie["UserName"] = correoUsuario;
            httpCookie.Expires = DateTime.Now.AddDays(5d);
            httpCookie.HttpOnly = true;
            Response.Cookies.Add(httpCookie);
        }

        private void CargarMenu()
        {
            List<PerfilFuncionalidadesModel> perfilFuncionalidades = null;
            ServicioFuncionalidades servicioFuncionalidades = new ServicioFuncionalidades();
            if (Session["PerfilFuncionalidades"] == null)
            {
                perfilFuncionalidades = servicioFuncionalidades.CargarPerfilFuncionalidades(PerfilId);

                string lastChar;
                var url = System.Web.HttpContext.Current.Request.Url.Authority;
                foreach (var funcionalidad in perfilFuncionalidades)
                {
                    if (!string.IsNullOrEmpty(funcionalidad.Funcionalidades.Url))
                    {
                        funcionalidad.Funcionalidades.Url = string.Concat("http://", url, funcionalidad.Funcionalidades.Url);
                        lastChar = funcionalidad.Funcionalidades.Url.Substring(funcionalidad.Funcionalidades.Url.Length - 1, 1);
                        if (lastChar == "?") funcionalidad.Funcionalidades.Url = funcionalidad.Funcionalidades.Url.Remove(funcionalidad.Funcionalidades.Url.Length - 1, 1);
                    }
                }
                Session["PerfilFuncionalidades"] = perfilFuncionalidades;
            }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
                return;

            new SimcoExcepcion(filterContext.Exception.Message, filterContext.Exception, log);

            // Redireccionar el error 
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Exception", action = "Index" }));

            // Detener exception 
            filterContext.ExceptionHandled = true;

            // Limpiar response
            filterContext.HttpContext.Response.Clear();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.EstadoUsuario = EstadoUsuario;
            ViewBag.TipoPersona = TipoPersona;
            ViewBag.TipoAgente = TipoAgente;
            ViewBag.Perfil = Perfil;
            ViewBag.CargarPublicacionIndicador = false;
            ViewBag.ActivarColeccionesColombianas = false;

            ViewBag.TieneCodigo = false;
            ViewBag.TienePlan = false;

            if (TipoAgente != TiposAgente.InstitucionAliada.ToString())
            {
                ServicioEntidadesMuseales servicioEntidades = new ServicioEntidadesMuseales();
                ViewBag.TieneCodigo = servicioEntidades.EntidadTieneCodigo(UsuarioId);

                ServicioPlanesFortalecimiento servicioPlanesFortalecimiento = new ServicioPlanesFortalecimiento();
                ViewBag.TienePlan = servicioPlanesFortalecimiento.TienePlan(PersonaId);
            }

            ServicioIndicadores servicioIndicadores = new ServicioIndicadores();
            List<string> configuracion = servicioIndicadores.CargarConfiguracionIndicadores();

            if (configuracion != null && configuracion.Any())
            {
                ViewBag.CargarPublicacionIndicador = bool.Parse(configuracion[0]);
                ViewBag.TextoPublicacionIndicador = configuracion[1];
            }

            ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
            EtiquetaDominioModel etiquetaDominioModel = servicioConfiguracion.CargarEtiquetaDominio(NombreDominios.ActivarColeccionesColombianas);

            if (etiquetaDominioModel != null)
                ViewBag.ActivarColeccionesColombianas = bool.Parse(etiquetaDominioModel.Valor);

            ServicioGrupos servicioGrupos = new ServicioGrupos();
            ViewBag.GruposDeUsuarios = servicioGrupos.ObtenerGruposDeUsuarios(UsuarioId);

            CargarMenu();

            base.OnActionExecuting(filterContext);
        }


    }
}
