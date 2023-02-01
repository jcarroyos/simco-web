using log4net;
using Simco.ApplicationLayer.Servicios;
using Simco.CrossInfraestructureLayer;
using Simco.DomainModelLayer.ViewModels;
using Simco.Infrastructure.Notification;
using Simco.Models;
using System;
using System.Web.Mvc;

namespace Simco.Controllers
{
    public class RegistroController : BaseController
    {
        private static readonly ILog log = SimcoExcepcion.GetLogger(typeof(SolicitudesController));
        public ActionResult Index()
        {
            return ObtenerSesion();
        }

        #region Persona natural

        public ActionResult PersonaNatural()
        {
            ServicioPersonasNaturales servicioPersonas = new ServicioPersonasNaturales();
            InicioSesionModel inicioSesionModel = ObtenerSesionModel();
            inicioSesionModel.PersonaNaturalRegistroModel = servicioPersonas.ObtenerPersonaNaturalRegistroModel();

            return View(inicioSesionModel);
        }

        [HttpPost]
        public ActionResult PersonaNatural(InicioSesionModel inicioSesionModel)
        {
            ModelState.Remove("CorreoElectronico");
            ModelState.Remove("Contrasena");

            ServicioPersonasNaturales servicioPersonaNatural = new ServicioPersonasNaturales();
            if (ModelState.IsValid)
            {
                try
                {
                    inicioSesionModel.PersonaNaturalRegistroModel.PersonaNatural.Imagen = Imagenes.ConvertirImagenAByte(inicioSesionModel.PersonaNaturalRegistroModel.PersonaNatural.ImageUpload);

                    try
                    {
                        CaptchaResponse response = ValidateCaptcha(Request["g-recaptcha-response"]);
                        if (!response.Success)
                        {
                            string mensaje = (response.ErrorMessage.Contains("missing-input-response") ? "Es validar el captcha para poder continuar" : response.ErrorMessage[0]);
                            this.ShowMessage(MessageType.Alert, mensaje);
                            return PersonaJuridica();
                        }
                    }
                    catch (Exception ex)
                    {
                        new SimcoExcepcion(ex.Message, ex, log);
                    }

                    PersonaNaturalModel personaNaturalModel = inicioSesionModel.PersonaNaturalRegistroModel.PersonaNatural;

                    if (servicioPersonaNatural.RegistrarUsuarioPersonaNatural(personaNaturalModel))
                    {
                        this.ShowMessage(MessageType.Success, Constante.PersonaNaturalRegistrada);

                        InicioSesionModel inicioSesion = new InicioSesionModel();
                        inicioSesion.CorreoElectronico = inicioSesionModel.PersonaNaturalRegistroModel.PersonaNatural.CorreoElectronico;
                        inicioSesion.Contrasena = inicioSesionModel.PersonaNaturalRegistroModel.PersonaNatural.Contrasena;
                        inicioSesion.Recordarme = true;

                        return IniciarSesion(inicioSesion, true);
                    }
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            servicioPersonaNatural.ObtenerPersonaNaturalRegistroModel(inicioSesionModel.PersonaNaturalRegistroModel);
            return View(inicioSesionModel);
        }

        #endregion

        #region Persona juridica

        public ActionResult PersonaJuridica()
        {
            ServicioPersonasJuridicas servicioPersonas = new ServicioPersonasJuridicas();
            InicioSesionModel inicioSesionModel = ObtenerSesionModel();
            inicioSesionModel.PersonaJuridicaRegistroModel = servicioPersonas.ObtenerPersonaJuridicaRegistroModel();

            return View(inicioSesionModel);
        }

        [HttpPost]
        public ActionResult PersonaJuridica(InicioSesionModel inicioSesionModel)
        {
            ModelState.Remove("CorreoElectronico");
            ModelState.Remove("Contrasena");

            ModelState.Remove("PersonaJuridicaRegistroModel.PersonaJuridica.Categoria");
            ModelState.Remove("PersonaJuridicaRegistroModel.PersonaJuridica.NombreDeLaInstitucion");
            ModelState.Remove("PersonaJuridicaRegistroModel.PersonaJuridica.NombreContacto");
            ModelState.Remove("PersonaJuridicaRegistroModel.PersonaJuridica.DescripcionInstitucion");

            ServicioPersonasJuridicas servicioPersonaJuridica = new ServicioPersonasJuridicas();
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
                            return PersonaJuridica();
                        }
                    }
                    catch (Exception ex)
                    {
                        new SimcoExcepcion(ex.Message, ex, log);
                    }

                    System.Web.HttpPostedFileBase httpPostedFileBase = inicioSesionModel.PersonaJuridicaRegistroModel.PersonaJuridica.ImageUpload;
                    inicioSesionModel.PersonaJuridicaRegistroModel.PersonaJuridica.Categoria = TiposAgente.EntidadMuseal.ToString();
                    inicioSesionModel.PersonaJuridicaRegistroModel.PersonaJuridica.Horario = "Sí";
                    inicioSesionModel.PersonaJuridicaRegistroModel.PersonaJuridica.DocumentoFotoFachada =
                        new DocumentoModel() { ImageUpload = httpPostedFileBase };
                    inicioSesionModel.PersonaJuridicaRegistroModel.PersonaJuridica.Imagen = Imagenes.ConvertirImagenAByte(httpPostedFileBase);

                    if (servicioPersonaJuridica.RegistrarUsuarioPersonaJuridica(inicioSesionModel.PersonaJuridicaRegistroModel.PersonaJuridica, true))
                    {
                        InicioSesionModel inicioSesion = new InicioSesionModel();
                        inicioSesion.CorreoElectronico = inicioSesionModel.PersonaJuridicaRegistroModel.PersonaJuridica.CorreoElectronico;
                        inicioSesion.Contrasena = inicioSesionModel.PersonaJuridicaRegistroModel.PersonaJuridica.Contrasena;
                        inicioSesion.Recordarme = true;

                        IniciarSesion(inicioSesion, false);

                        this.ShowMessage(MessageType.Success, Constante.PersonaJuridicaRegistrada);
                        return RedirectToActionPermanent("Index", "Encuesta");
                    }
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            servicioPersonaJuridica.ObtenerPersonaJuridicaRegistroModel(inicioSesionModel.PersonaJuridicaRegistroModel);

            return View(inicioSesionModel);
        }

        #endregion

        #region Institucion aliada

        public ActionResult InstitucionAliada()
        {
            ServicioPersonasJuridicas servicioPersonas = new ServicioPersonasJuridicas();
            InicioSesionModel inicioSesionModel = ObtenerSesionModel();
            inicioSesionModel.PersonaJuridicaRegistroModel = servicioPersonas.ObtenerPersonaJuridicaRegistroModel();

            return View(inicioSesionModel);
        }

        [HttpPost]
        public ActionResult InstitucionAliada(InicioSesionModel inicioSesionModel)
        {
            ModelState.Remove("CorreoElectronico");
            ModelState.Remove("Contrasena");

            ModelState.Remove("PersonaJuridicaRegistroModel.PersonaJuridica.Nombre");
            ModelState.Remove("PersonaJuridicaRegistroModel.PersonaJuridica.Resena");

            ServicioPersonasJuridicas servicioPersonaJuridica = new ServicioPersonasJuridicas();
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
                            return PersonaJuridica();
                        }
                    }
                    catch (Exception ex)
                    {
                        new SimcoExcepcion(ex.Message, ex, log);
                    }


                    inicioSesionModel.PersonaJuridicaRegistroModel.PersonaJuridica.Nombre = inicioSesionModel.PersonaJuridicaRegistroModel.PersonaJuridica.NombreDeLaInstitucion;
                    inicioSesionModel.PersonaJuridicaRegistroModel.PersonaJuridica.Resena = inicioSesionModel.PersonaJuridicaRegistroModel.PersonaJuridica.DescripcionInstitucion;
                    inicioSesionModel.PersonaJuridicaRegistroModel.PersonaJuridica.Horario = "No";
                    inicioSesionModel.PersonaJuridicaRegistroModel.PersonaJuridica.Imagen = Imagenes.ConvertirImagenAByte(inicioSesionModel.PersonaJuridicaRegistroModel.PersonaJuridica.ImageUpload);

                    if (servicioPersonaJuridica.RegistrarUsuarioPersonaJuridica(inicioSesionModel.PersonaJuridicaRegistroModel.PersonaJuridica, false))
                    {
                        this.ShowMessage(MessageType.Success, Constante.InstitucionAliadaRegistrada);

                        InicioSesionModel inicioSesion = new InicioSesionModel();
                        inicioSesion.CorreoElectronico = inicioSesionModel.PersonaJuridicaRegistroModel.PersonaJuridica.CorreoElectronico;
                        inicioSesion.Contrasena = inicioSesionModel.PersonaJuridicaRegistroModel.PersonaJuridica.Contrasena;
                        inicioSesion.Recordarme = true;

                        return IniciarSesion(inicioSesion, true);
                    }
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            servicioPersonaJuridica.ObtenerPersonaJuridicaRegistroModel(inicioSesionModel.PersonaJuridicaRegistroModel);
            return View(inicioSesionModel);
        }

        #endregion

        #region Entidad Museal : PreRegistro

        public ActionResult EntidadMuseal()
        {
            ServicioEntidadesMusealesPreRegistro servicioPreRegistro = new ServicioEntidadesMusealesPreRegistro();
            InicioSesionModel inicioSesionModel = ObtenerSesionModel();
            inicioSesionModel.PostulacionEntidadMuseal = servicioPreRegistro.ObtenerPostulacionEntidadMuseal();

            return View(inicioSesionModel);
        }

        [HttpPost]
        public ActionResult EntidadMuseal(InicioSesionModel inicioSesionModel)
        {
            ServicioEntidadesMusealesPreRegistro servicioPreRegistro = new ServicioEntidadesMusealesPreRegistro();
            ModelState.Remove("CorreoElectronico");
            ModelState.Remove("Contrasena");

            ModelState.Remove("PersonaJuridicaRegistroModel.PersonaJuridica.Nombre");
            ModelState.Remove("PersonaJuridicaRegistroModel.PersonaJuridica.Resena");
            ModelState.Remove("PersonaJuridicaRegistroModel.PersonaJuridica.Categoria");
            ModelState.Remove("PersonaJuridicaRegistroModel.PersonaJuridica.NombreDeLaInstitucion");
            ModelState.Remove("PersonaJuridicaRegistroModel.PersonaJuridica.NombreContacto");
            ModelState.Remove("PersonaJuridicaRegistroModel.PersonaJuridica.DescripcionInstitucion");

            ModelState.Remove("PostulacionEntidadMuseal.Comentarios");

            //Asignando Codigo Del Municipio
            string codigoMunicipio = inicioSesionModel.PostulacionEntidadMuseal.CodigoMunicipio;

            //Validación Tipo Servicios
            bool validateTipoServicio = false;
            int tiposSeleccionados = inicioSesionModel.PostulacionEntidadMuseal.TiposServicio.FindAll(tipo => tipo.Seleccionada == true).Count;
            if (tiposSeleccionados > 0)
            {
                validateTipoServicio = true;
                inicioSesionModel.PostulacionEntidadMuseal.EsInValidoTipoServicios = false;
            }

            //Validando Finalidad de Servicios
            bool validacionFinalidadServicios = true;
            if (
                !inicioSesionModel.PostulacionEntidadMuseal.FinalidadDiversion &&
                !inicioSesionModel.PostulacionEntidadMuseal.FinalidadEducacion &&
                !inicioSesionModel.PostulacionEntidadMuseal.FinalidadMixto
              )
            {
                validacionFinalidadServicios = false;
            }

            inicioSesionModel.PostulacionEntidadMuseal.FinalidadServiciosError = !validacionFinalidadServicios;

            //Validación Tipo de Servicio y Finalidad Servicio
            if (!validateTipoServicio || !validacionFinalidadServicios)
            {
                inicioSesionModel.PostulacionEntidadMuseal = servicioPreRegistro.ObtenerPostulacionEntidadMuseal();
                inicioSesionModel.PostulacionEntidadMuseal.EsInValidoTipoServicios = !validateTipoServicio;
                inicioSesionModel.PostulacionEntidadMuseal.FinalidadServiciosError = !validacionFinalidadServicios;

                inicioSesionModel.PostulacionEntidadMuseal.CodigoMunicipio = codigoMunicipio;
                return View(inicioSesionModel);
            }

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
                            return EntidadMuseal();
                        }
                    }
                    catch (Exception ex)
                    {
                        new SimcoExcepcion(ex.Message, ex, log);
                    }

                    //Validando si la entidad es abierta al publico, eliminar el comentario generado automaticamente.
                    inicioSesionModel.PostulacionEntidadMuseal.EntidadPublicaComentario = inicioSesionModel.PostulacionEntidadMuseal.EntidadPublica ? null : string.Empty;

                    // RegistrarPostulacionEntidadMuseal
                    if (servicioPreRegistro.RegistrarPostulacionEntidadMuseal(inicioSesionModel.PostulacionEntidadMuseal))
                    {
                        this.ShowMessage(MessageType.Success, Constante.PreRegistroEntidadMuseal, true);
                        return RedirectToActionPermanent("EntidadMuseal", "Registro");
                    }
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            // Agregando de nuevo el Codigo del Municipio
            inicioSesionModel.PostulacionEntidadMuseal = servicioPreRegistro.ObtenerPostulacionEntidadMuseal();
            inicioSesionModel.PostulacionEntidadMuseal.CodigoMunicipio = codigoMunicipio;

            //Agregando Validacion Tipo de Servicio
            if (validateTipoServicio)
            {
                inicioSesionModel.PostulacionEntidadMuseal.EsInValidoTipoServicios = false;
            }

            //Agregando Validación de la Finalidad del Servicio
            if (!validacionFinalidadServicios)
            {
                inicioSesionModel.PostulacionEntidadMuseal.FinalidadServiciosError = true;
            }

            return View(inicioSesionModel);
        }

        #endregion
    }
}