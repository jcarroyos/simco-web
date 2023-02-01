using Simco.ApplicationLayer.Servicios;
using Simco.CrossInfraestructureLayer;
using Simco.DomainModelLayer.ViewModels;
using Simco.Infrastructure.Notification;
using Simco.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Simco.Controllers
{
    [Authorize]
    public class ComiteDocumentosController : BaseController
    {
        // GET: ComiteDocumentos
        public ActionResult Index(int comiteId)
        {
            //Consultar Estado Comite
            ComiteModel comite = new ServicioComites().CargarComitesModel(comiteId);
            ViewBag.EstadoComite = comite.EstadoId;

            ComiteDocumentoModel comiteDocModel = new ComiteDocumentoModel();
            comiteDocModel.COM_ID = comiteId;

            //Consultar Documentos Comite
            ViewBag.ComiteDocumentos = new ServicioComiteDocumentos().CargarComiteDocumentos(comiteId);
            Session["idComite"] = comiteId;

            return View(comiteDocModel);
        }

        [HttpPost]
        public ActionResult Index(ComiteDocumentoModel comiteDocModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    comiteDocModel.COM_ID = Convert.ToInt32(Session["idComite"]);
                    var servicioComitedocumenttos = new ServicioComiteDocumentos();
                    comiteDocModel.Comite = new ServicioComites().CargarComitesModel(comiteDocModel.COM_ID);
                    var listaEtiquetaDominioModelDcto = Documents.ObtenerConfiguracionEtiquetasDominio("ConfiguracionCargaArchivoDocumentos");
                    var modelError = new Dictionary<string, string>();
                    string resultadoValidacion = string.Empty;

                    if (!Documents.ValidarTamañoDocumentoComiteDocumento(comiteDocModel.ImageUpload, listaEtiquetaDominioModelDcto, out resultadoValidacion))
                    {
                        foreach (var item in modelError)
                            ModelState.AddModelError(item.Key, item.Value);

                        this.ShowMessage(MessageType.Error, resultadoValidacion, true);

                        return View(comiteDocModel);
                    }

                    var comiteEntity = servicioComitedocumenttos.CrearDocumentoDeComite(comiteDocModel);
                    if (comiteEntity)
                    {
                        this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                    }
                    else
                    {
                        this.ShowMessage(MessageType.Alert, Constante.ErrorEliminacion, true);
                    }

                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }
            else
            {
                var mensajesValidaciones = (from lista in ModelState.Values
                                            where lista.Errors.Count > 0
                                            select lista.Errors.FirstOrDefault().ErrorMessage)
                                            .FirstOrDefault();

                this.ShowMessage(MessageType.Information, mensajesValidaciones, false);
                return View(comiteDocModel);
            }

            return RedirectToActionPermanent("Index", "ComiteDocumentos", new { comiteId = Session["idComite"] });
        }

        public ActionResult EliminarDocumentoComite(int comiteId, int ComiteDocumentoId)
        {
            try
            {
                var servicioComitedocumenttos = new ServicioComiteDocumentos();
                var comiteEntity = servicioComitedocumenttos.EliminarDocumentoComite(comiteId, ComiteDocumentoId);

                if (comiteEntity)
                {
                    this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                }
                else
                {
                    this.ShowMessage(MessageType.Alert, Constante.ErrorEliminacion, true);
                }

            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Index", "ComiteDocumentos", new { comiteId = comiteId });
        }

        public ActionResult FinalizarComite()
        {
            string contrasenaTemporal = string.Empty;
            ServicioComites servicioComites = new ServicioComites();
            Correo correo = new Correo();
            ServicioNotificaciones servicioNotificaciones = new ServicioNotificaciones();
            int comiteId = Convert.ToInt32(Session["idComite"]);

            // Capturar la lista de Entidad Postuladas que fueron marcadas como NO APROBADAS : 3 POSPUESTAS: 4
            var listaEntidadesPostulantes = servicioComites.CargarEntidadesPostulantes(comiteId);

            foreach (var postulante in listaEntidadesPostulantes)
            {
                // Enviar correo con la contraseña
                correo.Para = postulante.PRE_CORREO_CONTACTO;
                correo.Asunto = string.Concat("La Postulación de su Entidad fue: ", postulante.ESTADOENTIDADESMUSEALES.EST_NOMBRE);
                correo.Cuerpo = postulante.PRE_COMENTARIOS;

                // Registro de Entidades Aprobadas
                if (postulante.EST_ID == 2)
                {
                    // Registrar la entidad como Persona Juridica, Crea la Entidad Museal y Crear el Usuario 
                    contrasenaTemporal = servicioComites.RegistrarEntidades(postulante);
                    correo.Cuerpo = string.Concat(postulante.PRE_COMENTARIOS, "\n\n", " Puede ingresar al aplicativo con su correo y su contraseña temporal es: ", contrasenaTemporal);
                }

                servicioNotificaciones.EnviarCorreo(correo);
            }

            var resultado = servicioComites.FinalizarIngresoDocumentosComites(comiteId);

            return RedirectToActionPermanent("Index", "Comites", new { ComiteId = 0 });
        }
    }
}
