using log4net;
using Simco.ApplicationLayer.Servicios;
using Simco.CrossInfraestructureLayer;
using Simco.DomainModelLayer.ViewModels;
using Simco.Infrastructure.Notification;
using Simco.Models;
using System;
using System.IO;
using System.Web.Mvc;

namespace Simco.Controllers
{
    public class SolicitudesController : BaseController
    {
        private static readonly ILog log = SimcoExcepcion.GetLogger(typeof(SolicitudesController));

        // GET: Solicitudes
        public ActionResult Index()
        {
            ServicioSolicitudes servicioSolicitudes = new ServicioSolicitudes();
            InicioSesionModel inicioSesionModel = ObtenerSesionModel();
            inicioSesionModel.SolicitudRegistro = servicioSolicitudes.ObtenerModeloRegistroServicio();

            return View(inicioSesionModel);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Index(InicioSesionModel inicioSesionModel)
        {
            ModelState.Remove("SolicitudRegistro.Categoria");
            ModelState.Remove("SolicitudRegistro.Tipo");
            ModelState.Remove("SolicitudRegistro.Solicitud.Categoria");
            ModelState.Remove("SolicitudRegistro.Solicitud.Tipo");

            ServicioSolicitudes servicioSolicitudes = new ServicioSolicitudes();
            if (ModelState.IsValid)
            {
                try
                {

                    try
                    {
                        CaptchaResponse response = ValidateCaptcha(Request["g-recaptcha-response"]);
                        if (!response.Success)
                        {
                            string mensaje = (response.ErrorMessage.Contains("missing-input-response") ? "Favor debe validar el campo de seguridad o captcha para poder continuar" : response.ErrorMessage[0]);
                            this.ShowMessage(MessageType.Alert, mensaje);
                            return Index();
                        }
                    }
                    catch (Exception ex)
                    {
                        new SimcoExcepcion(ex.Message, ex, log);
                    }

                    DocumentoModel docModelReference;
                    if (inicioSesionModel.SolicitudRegistro.NuevoDocumento.ImageUpload != null)
                    {
                        docModelReference = inicioSesionModel.SolicitudRegistro.NuevoDocumento;
                        RegistrarDocumento(ref docModelReference, inicioSesionModel.SolicitudRegistro.Solicitud.Nombre);
                    }

                    if (inicioSesionModel.SolicitudRegistro.NuevoDocumento2.ImageUpload != null)
                    {
                        docModelReference = inicioSesionModel.SolicitudRegistro.NuevoDocumento2;
                        RegistrarDocumento(ref docModelReference, inicioSesionModel.SolicitudRegistro.Solicitud.Nombre);
                    }

                    if (inicioSesionModel.SolicitudRegistro.NuevoDocumento3.ImageUpload != null)
                    {
                        docModelReference = inicioSesionModel.SolicitudRegistro.NuevoDocumento3;
                        RegistrarDocumento(ref docModelReference, inicioSesionModel.SolicitudRegistro.Solicitud.Nombre);
                    }

                    if (servicioSolicitudes.RegistrarSolicitud(inicioSesionModel.SolicitudRegistro, 0, Constante.MensajeSolicitudCreacion))
                    {
                        this.ShowMessage(MessageType.Success, Constante.SolicitudRegistrada, true);
                        return RedirectToActionPermanent("Solicitudes", "Solicitudes");
                    }

                }
                catch (Exception ex)
                {
                    new SimcoExcepcion(ex.Message, ex, log);
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            // Carga los tipos de documentos y departamentos
            servicioSolicitudes.ObtenerModeloRegistroServicio(inicioSesionModel.SolicitudRegistro);

            return View(inicioSesionModel);
        }

        public ActionResult Solicitudes(InicioSesionModel inicioSesionModel)
        {
            ServicioSolicitudes servicioSolicitudes = new ServicioSolicitudes();
            ObtenerSesionModel(ref inicioSesionModel);

            if (inicioSesionModel.SolicitudRegistro == null)
            {
                inicioSesionModel.SolicitudRegistro = new SolicitudRegistroModel();
                inicioSesionModel.SolicitudRegistro.Solicitud = new SolicitudModel();
            }
            else
                inicioSesionModel.SolicitudPublicas = servicioSolicitudes.ObtenerSolicitudes(inicioSesionModel.SolicitudRegistro.Solicitud);

            return View(inicioSesionModel);
        }

        public ActionResult VistaSolicitud(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    ServicioSolicitudes servicioSolicitudes = new ServicioSolicitudes();
                    InicioSesionModel inicioSesionModel = ObtenerSesionModel();
                    inicioSesionModel.SolicitudConsulta = servicioSolicitudes.ObtenerSolicitud(Convert.ToInt32(Criptografia.DesencriptarUrl(id)));

                    return View(inicioSesionModel);
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return RedirectToActionPermanent("Index", "Solicitudes");
        }

        [AllowAnonymous]
        public ActionResult ConsultarSolicitudes(string consulta)
        {
            ViewBag.Consulta = consulta;
            ServicioSolicitudes servicioSolicitudes = new ServicioSolicitudes();
            return View(servicioSolicitudes.ObtenerTodasSolicitudes(consulta));
        }

        public ActionResult Solicitud(string id)
        {
            ServicioSolicitudes servicioSolicitudes = new ServicioSolicitudes();
            try
            {
                if (!string.IsNullOrEmpty(id))
                    return View("VistaSolicitudInterna", servicioSolicitudes.ObtenerSolicitud(Convert.ToInt32(Criptografia.DesencriptarUrl(id))));
                else
                    return View(servicioSolicitudes.ObtenerModeloRegistroSolicitudAgente(UsuarioId));

            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("ConsultarSolicitudes", "Solicitudes");
        }

        // Nueva solicitud modo agente
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Solicitud(SolicitudRegistroModel registroSolicitudModel)
        {
            ModelState.Remove("SolicitudRegistro.Solicitud.Categoria");
            ModelState.Remove("SolicitudRegistro.Solicitud.Tipo");

            try
            {

                if (registroSolicitudModel.NuevoDocumento.ImageUpload != null)
                {
                    registroSolicitudModel.NuevoDocumento.Archivo = Imagenes.ConvertirImagenAByte(registroSolicitudModel.NuevoDocumento.ImageUpload);
                    registroSolicitudModel.NuevoDocumento.Nombre = Path.GetFileName(registroSolicitudModel.NuevoDocumento.ImageUpload.FileName);
                    registroSolicitudModel.NuevoDocumento.TipoMime = registroSolicitudModel.NuevoDocumento.ImageUpload.ContentType;
                    registroSolicitudModel.NuevoDocumento.Autor = NombreCompleto;
                }

                ServicioSolicitudes servicioSolicitudes = new ServicioSolicitudes();
                if (servicioSolicitudes.RegistrarSolicitud(registroSolicitudModel, UsuarioId))
                {
                    this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);

                    if (Perfil == Perfiles.Administrador)
                        return RedirectToActionPermanent("ConsultarSolicitudes", "Solicitudes");
                    else
                        return RedirectToActionPermanent("SolicitudesAgentes", "Solicitudes");
                }

            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Alert, ex.Message);
            }

            return RedirectToActionPermanent("ConsultarSolicitudes", "Solicitudes");
        }

        [AllowAnonymous]
        public ActionResult NuevaSolicitud()
        {
            try
            {
                ServicioSolicitudes servicioSolicitudes = new ServicioSolicitudes();
                SolicitudRegistroModel solicitudRegistroModel = new SolicitudRegistroModel();
                servicioSolicitudes.ObtenerModeloNuevaSolicitud(ref solicitudRegistroModel);

                return View(solicitudRegistroModel);
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Alert, ex.Message);
            }

            return RedirectToActionPermanent("Index", "Administrador");
        }

        // Nueva solicitud modo admin o asesor
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult NuevaSolicitud(SolicitudRegistroModel registroSolicitudModel)
        {
            ModelState.Remove("NuevoComentario.FechaRegistro");
            ModelState.Remove("NuevoDocumento.FechaRegistro");
            ModelState.Remove("Solicitud.FechaRegistro");
            ModelState.Remove("Solicitud.Categoria");
            ModelState.Remove("Solicitud.Tipo");
            ModelState.Remove("Categoria");
            ModelState.Remove("Tipo");

            ServicioSolicitudes servicioSolicitudes = new ServicioSolicitudes();
            if (ModelState.IsValid)
            {
                try
                {
                    DocumentoModel docModelReference;
                    if (registroSolicitudModel.NuevoDocumento.ImageUpload != null)
                    {
                        docModelReference = registroSolicitudModel.NuevoDocumento;
                        RegistrarDocumento(ref docModelReference, NombreCompleto);
                    }

                    if (registroSolicitudModel.NuevoDocumento2.ImageUpload != null)
                    {
                        docModelReference = registroSolicitudModel.NuevoDocumento2;
                        RegistrarDocumento(ref docModelReference, NombreCompleto);
                    }

                    if (registroSolicitudModel.NuevoDocumento3.ImageUpload != null)
                    {
                        docModelReference = registroSolicitudModel.NuevoDocumento3;
                        RegistrarDocumento(ref docModelReference, NombreCompleto);
                    }

                    
                    if (servicioSolicitudes.RegistrarSolicitud(registroSolicitudModel, 0, Constante.MensajeSolicitudCreacion))
                    {
                        this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);

                        if (Perfil == Perfiles.Administrador || Perfil == Perfiles.Asesor)
                            return RedirectToActionPermanent("ConsultarSolicitudes", "Solicitudes");
                        else
                            return RedirectToActionPermanent("SolicitudesAgentes", "Solicitudes");
                    }

                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Alert, ex.Message);
                }
            }

            servicioSolicitudes.ObtenerModeloNuevaSolicitud(ref registroSolicitudModel);

            return View(registroSolicitudModel);

        }

        public ActionResult SolicitudesAgentes()
        {
            ServicioSolicitudes servicioSolicitudes = new ServicioSolicitudes();
            return View(servicioSolicitudes.ObtenerSolicitudesAgentes(UsuarioId));

        }

        static void RegistrarDocumento(ref DocumentoModel documentoModel, string nombreAutor)
        {
            documentoModel.Archivo = Imagenes.ConvertirImagenAByte(documentoModel.ImageUpload);
            documentoModel.Nombre = Path.GetFileName(documentoModel.ImageUpload.FileName);
            documentoModel.TipoMime = documentoModel.ImageUpload.ContentType;
            documentoModel.FechaRegistro = DateTime.Now;
            documentoModel.Autor = nombreAutor;
        }

        public ActionResult SolicitudConsultada(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    ServicioSolicitudes servicioSolicitudes = new ServicioSolicitudes();
                    return View("SolicitudConsultada", servicioSolicitudes.ObtenerSolicitud(Convert.ToInt32(Criptografia.DesencriptarUrl(id)), UsuarioId));
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return RedirectToActionPermanent("ConsultarSolicitudes", "Solicitudes");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SolicitudConsultada(SolicitudRegistroModel registroSolicitudModel)
        {
            try
            {
                if (registroSolicitudModel.NuevoDocumento.ImageUpload != null)
                {
                    registroSolicitudModel.NuevoDocumento.Archivo = Imagenes.ConvertirImagenAByte(registroSolicitudModel.NuevoDocumento.ImageUpload);
                    registroSolicitudModel.NuevoDocumento.Nombre = Path.GetFileName(registroSolicitudModel.NuevoDocumento.ImageUpload.FileName);
                    registroSolicitudModel.NuevoDocumento.TipoMime = registroSolicitudModel.NuevoDocumento.ImageUpload.ContentType;
                    registroSolicitudModel.NuevoDocumento.Autor = NombreCompleto;
                }

                if (registroSolicitudModel.DocumentoRespuesta.ImageUpload != null)
                {
                    registroSolicitudModel.DocumentoRespuesta.Archivo = Imagenes.ConvertirImagenAByte(registroSolicitudModel.DocumentoRespuesta.ImageUpload);
                    registroSolicitudModel.DocumentoRespuesta.Nombre = Path.GetFileName(registroSolicitudModel.DocumentoRespuesta.ImageUpload.FileName);
                    registroSolicitudModel.DocumentoRespuesta.TipoMime = registroSolicitudModel.DocumentoRespuesta.ImageUpload.ContentType;
                    registroSolicitudModel.DocumentoRespuesta.Autor = NombreCompleto;
                }

                ServicioSolicitudes servicioSolicitudes = new ServicioSolicitudes();
                if (servicioSolicitudes.ActualizarSolicitud(registroSolicitudModel, UsuarioId))
                {
                    this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                    return RedirectToActionPermanent("ConsultarSolicitudes", "Solicitudes");
                }

                //this.ShowMessage(MessageType.Alert, Constante.ErrorRegistro);

            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("ConsultarSolicitudes", "Solicitudes");
        }

    }
}