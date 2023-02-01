using Simco.ApplicationLayer.Servicios;
using Simco.CrossInfraestructureLayer;
using Simco.DomainModelLayer.Entidades;
using Simco.DomainModelLayer.ViewModels;
using Simco.Infrastructure.Notification;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;

namespace Simco.Controllers
{
    public class EncuestaServicioController : BaseController
    {
        #region "Encuesta Satisfaccion Solicitud"
        [HttpGet]
        public ActionResult EncuestaServicio(string encuestaCalificar)
        {
            if (!string.IsNullOrEmpty(encuestaCalificar))
            {
                try
                {
                    encuestaCalificar = Criptografia.DesencriptarUrl(encuestaCalificar);
                    int solicitudId = 0;
                    int usuarioId = 0;

                    if (encuestaCalificar.Split('&').Length == 2 &&
                        encuestaCalificar.Split('&')[0].Split('=')[0] == "SolicitudId" &&
                        encuestaCalificar.Split('&')[1].Split('=')[0] == "UsuarioId")
                    {
                        solicitudId = int.Parse(encuestaCalificar.Split('&')[0].Split('=')[1]);
                        usuarioId = int.Parse(encuestaCalificar.Split('&')[1].Split('=')[1]);

                        ServicioEncuentas servicioEncuentas = new ServicioEncuentas();
                        InicioSesionModel inicioSesionModel = new InicioSesionModel();

                        if (!servicioEncuentas.VerificarRegistroPrevioRespuestasEncuestaServicio(solicitudId))
                        { 
                            inicioSesionModel.EncuestaServicioSolicitud = servicioEncuentas.ObtenerEncuestaServicioModel(1);
                            return View(inicioSesionModel);
                        }
                        else
                        {
                            this.ShowMessage(MessageType.Success, Constante.MensajeEncuestaServicioContestada, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, Constante.ErrorRegistrarEncuestas, true);
                }
            }
            else
            {
                this.ShowMessage(MessageType.Error, Constante.MensajeEncuestaServicioNoDetectada, true);
            }

            return RedirectToActionPermanent("Index", "Home");
        }

        [HttpPost]
        public ActionResult EncuestaServicio(EncuestaServicioSolicitudModel encuestaServicioSolicitud)
        {
            string encuestaCalificar = string.Empty;
            int encuestaId = 0;
            bool contestoTodasObligatorias = false;
            bool preguntaContestada = false;

            try
            {
                if(Request.Form.GetValues("encuestaCalificar").Length > 0 
                    && Request.Form.GetValues("EncuestaServicioSolicitud.EncuestaServicioModel.Id").Length > 0)
                {
                    encuestaCalificar = Request.Form.GetValues("encuestaCalificar")[0];
                    encuestaId = int.Parse(Request.Form.GetValues("EncuestaServicioSolicitud.EncuestaServicioModel.Id")[0]);
                    encuestaServicioSolicitud.RespuestaSeleccionadaEncuestaModel = new List<RespuestaSeleccionadaEncuestaModel>(); 

                    if (!string.IsNullOrEmpty("encuestaCalificar") )
                    {
                        encuestaCalificar = Criptografia.DesencriptarUrl(encuestaCalificar);
                        int solicitudId = 0;
                        int usuarioId = 0;

                        if (encuestaCalificar.Split('&').Length == 2 &&
                            encuestaCalificar.Split('&')[0].Split('=')[0] == "SolicitudId" &&
                            encuestaCalificar.Split('&')[1].Split('=')[0] == "UsuarioId")
                        {
                            solicitudId = int.Parse(encuestaCalificar.Split('&')[0].Split('=')[1]);
                            usuarioId = int.Parse(encuestaCalificar.Split('&')[1].Split('=')[1]);

                            ServicioEncuentas servicioEncuentas = new ServicioEncuentas();
                            InicioSesionModel inicioSesionModel = new InicioSesionModel();

                            if(!servicioEncuentas.VerificarRegistroPrevioRespuestasEncuestaServicio(solicitudId))
                            {
                                inicioSesionModel.EncuestaServicioSolicitud = servicioEncuentas.ObtenerEncuestaServicioModel(encuestaId);

                                #region "Obtener Encuesta"
                                foreach (var pregunta in inicioSesionModel.EncuestaServicioSolicitud.PreguntaEncuestaServicioModel
                                    .FindAll(x => x.Estado.Equals("Activo")))
                                {
                                    preguntaContestada = false;

                                    foreach (var respuesta in inicioSesionModel.EncuestaServicioSolicitud.RespuestaEncuestaServicioModel
                                        .FindAll(x => x.PreguntaEncuestaId == pregunta.Id && x.Estado.Equals("Activo")))
                                    {
                                        if (Request.Form.GetValues(string.Format("{0}", pregunta.Id)) != null)
                                        {
                                            encuestaServicioSolicitud.RespuestaSeleccionadaEncuestaModel.Add(new RespuestaSeleccionadaEncuestaModel()
                                            {
                                                EncuestaId = pregunta.EncuestaId,
                                                PreguntaEncuestaId = pregunta.Id,
                                                RespuestaEncuestaId = respuesta.Id,
                                                SolicitudId = solicitudId,
                                                UsuarioId = usuarioId,
                                                FechaRespuesta = DateTime.Now
                                            });

                                            preguntaContestada = true;
                                            break;
                                        }
                                    }

                                    if (pregunta.TipoPregunta == 2)
                                    {
                                        if (Request.Form.GetValues(string.Format("{0}_{1}", pregunta.Id, 1)) != null 
                                            && !Request.Form.GetValues(string.Format("{0}_{1}", pregunta.Id, 1))[0].Equals(string.Empty) )
                                        {
                                            string texto = Request.Form.GetValues(string.Format("{0}_{1}", pregunta.Id, 1))[0];
                                            texto = texto.Length > 200 ? texto.Substring(0, 200) : texto;

                                            encuestaServicioSolicitud.RespuestaSeleccionadaEncuestaModel.Add(new RespuestaSeleccionadaEncuestaModel()
                                            {
                                                EncuestaId = pregunta.EncuestaId,
                                                PreguntaEncuestaId = pregunta.Id,
                                                RespuestaEncuestaId = null,
                                                Descripcion = Server.HtmlEncode(texto),
                                                SolicitudId = solicitudId,
                                                UsuarioId = usuarioId,
                                                FechaRespuesta = DateTime.Now
                                            });

                                            preguntaContestada = true;
                                        }
                                    }

                                    if (pregunta.Obligatoria)
                                    {
                                        if (preguntaContestada)
                                        {
                                            contestoTodasObligatorias = true;
                                        }
                                        else
                                        {
                                            contestoTodasObligatorias = false;
                                            break;
                                        }
                                    }
                                }
                                #endregion "Obtener Encuesta"

                                if (contestoTodasObligatorias)
                                {
                                    servicioEncuentas.RegistrarRespuestasEncuestaServicio(encuestaServicioSolicitud.RespuestaSeleccionadaEncuestaModel);
                                    inicioSesionModel.EncuestaServicioSolicitud.RespuestaSeleccionadaEncuestaModel = encuestaServicioSolicitud.RespuestaSeleccionadaEncuestaModel;
                                    servicioEncuentas.EnviarNotificacionResultadoEncuestaServicio(inicioSesionModel.EncuestaServicioSolicitud, solicitudId);
                                    this.ShowMessage(MessageType.Success, Constante.MensajeEncuestaRegistrada, true);
                                    return RedirectToActionPermanent("Index", "Home");
                                }
                                else
                                {
                                    this.ShowMessage(MessageType.Success, Constante.MensajeEncuestaIncompleta, true);
                                }

                                return View(inicioSesionModel);
                            }
                            else
                            {
                                this.ShowMessage(MessageType.Success, Constante.MensajeEncuestaServicioContestada, true);
                            }
                        }
                    }
                }
                else
                {
                    this.ShowMessage(MessageType.Error, Constante.MensajeEncuestaServicioNoDetectada, true);
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, Constante.ErrorRegistrarEncuestas, true);
            }

            return RedirectToActionPermanent("Index", "Home");
        }

        #endregion "Encuesta Satisfaccion Solicitud"
    }
}