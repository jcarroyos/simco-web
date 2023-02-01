using Simco.DomainModelLayer.ViewModels;
using System.Web.Mvc;
using Simco.ApplicationLayer.Servicios;
using System;
using Simco.CrossInfraestructureLayer;
using Simco.Infrastructure.Notification;
using Simco.Models;
using log4net;

namespace Simco.Controllers
{
    public class HomeController : BaseController
    {
        private static readonly ILog log = SimcoExcepcion.GetLogger(typeof(SolicitudesController));

        public ActionResult Index()
        {
            ServicioDirectorio servicioDirectorio = new ServicioDirectorio();
            InicioSesionModel inicioSesionModel = ObtenerSesionModel();
            inicioSesionModel.Directorio = servicioDirectorio.Buscar(new DirectorioModel(), 3);

            return View(inicioSesionModel);
        }

        [HttpPost]
        public ActionResult Index(InicioSesionModel inicioSesionModel)
        {
            return IniciarSesion(inicioSesionModel, true);
        }

        [HttpPost]
        public ActionResult Buscar(InicioSesionModel inicioSesionModel)
        {
            ServicioDirectorio servicioDirectorio = new ServicioDirectorio();
            ObtenerSesionModel(ref inicioSesionModel);
            inicioSesionModel.Directorio = servicioDirectorio.Buscar(inicioSesionModel.Directorio, null);

            return View(inicioSesionModel);
        }

        public ActionResult Museo(string personaJuridicaId)
        {
            ServicioDirectorio servicioDirectorio = new ServicioDirectorio();
            InicioSesionModel inicioSesionModel = ObtenerSesionModel();
            inicioSesionModel.Directorio = servicioDirectorio.BuscarMuseo(int.Parse(personaJuridicaId));

            return View(inicioSesionModel);
        }

        public ActionResult Simco()
        {
            return ObtenerSesion();
        }

        public ActionResult Confirmacion()
        {
            return View(new ContrasenaRecuperarModel());
        }

        // Directorio en línea
        public ActionResult PersonaNatural()
        {
            ServicioDirectorio servicioDirectorio = new ServicioDirectorio();
            InicioSesionModel inicioSesionModel = ObtenerSesionModel();
            inicioSesionModel.Directorio = servicioDirectorio.PersonaNatural(new DirectorioModel(), null);

            return View(inicioSesionModel);
        }

        [HttpPost]
        public ActionResult PersonaNatural(InicioSesionModel inicioSesionModel)
        {
            ServicioDirectorio servicioDirectorio = new ServicioDirectorio();
            inicioSesionModel.Directorio = servicioDirectorio.PersonaNatural(inicioSesionModel.Directorio, null);

            return View(inicioSesionModel);
        }

        public ActionResult EntidadesAliadas()
        {
            ServicioDirectorio servicioDirectorio = new ServicioDirectorio();
            InicioSesionModel inicioSesionModel = ObtenerSesionModel();
            inicioSesionModel.Directorio = servicioDirectorio.EntidadesAliadas(new DirectorioModel(), null);

            return View(inicioSesionModel);
        }

        [HttpPost]
        public ActionResult EntidadesAliadas(InicioSesionModel inicioSesionModel)
        {
            ServicioDirectorio servicioDirectorio = new ServicioDirectorio();
            inicioSesionModel.Directorio = servicioDirectorio.EntidadesAliadas(inicioSesionModel.Directorio, null);

            return View(inicioSesionModel);
        }

        // Recuperar Contraseña
        public ActionResult RecuperarContrasena()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RecuperarContrasena(ContrasenaRecuperarModel contrasenaRecuperarModel)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    try
                    {
                        CaptchaResponse response = ValidateCaptcha(Request["g-recaptcha-response"]);
                        if (!response.Success)
                        {
                            string mensaje = (response.ErrorMessage.Contains("missing-input-response") ? "Es validar el captcha para poder continuar" : response.ErrorMessage[0]);
                            this.ShowMessage(MessageType.Alert, mensaje);
                            return View();
                        }
                    }
                    catch (Exception ex)
                    {
                        new SimcoExcepcion(ex.Message, ex, log);
                    }
                    

                    ServicioUsuarios servicioUsuariosPerfiles = new ServicioUsuarios();
                    servicioUsuariosPerfiles.RecuperarContrasena(contrasenaRecuperarModel);

                    InicioSesionModel inicioSesionModel = ObtenerSesionModel();
                    inicioSesionModel.CorreoElectronico = contrasenaRecuperarModel.CorreoElectronico;

                    return View("Confirmacion", inicioSesionModel);
                }
                catch (Exception)
                {
                    this.ShowMessage(MessageType.Error, Constante.ErrorRecuperarContrasena, true);
                }
            }

            return View();
        }

    }
}
