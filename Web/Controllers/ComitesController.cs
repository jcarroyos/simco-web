using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Simco.ApplicationLayer.Servicios;
using Simco.CrossInfraestructureLayer;
using Simco.DomainModelLayer.ViewModels;
using Simco.Infrastructure.Notification;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Simco.Controllers
{
    [Authorize]
    public class ComitesController : BaseController
    {
        #region Administración de Comites
        public ActionResult Index(string consulta, int comiteId)
        {
            try
            {
                if (comiteId != 0)
                {
                    ServicioComites servicioComites = new ServicioComites();
                    var comiteActualizado = servicioComites.FinalizarComites(comiteId);
                    if (comiteActualizado)
                    {
                        this.ShowMessage(MessageType.Success, Constante.MensajeFinalizarComite, true);
                    }
                    else
                    {
                        this.ShowMessage(MessageType.Alert, Constante.ErrorCerrarComite, true);
                    }
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            ConsultarComites(consulta);
            return View();
        }


        public ActionResult ConsultarComites(string consulta)
        {
            try
            {
                ViewBag.MensajeLink = string.Empty;
                ViewBag.Consulta = consulta != null ? consulta : string.Empty;
                ServicioComites servicioComites = new ServicioComites();
                List<ComiteModel> Comites = servicioComites.CargarComitesModel(consulta);
                ViewBag.Comites = Comites;
                ViewBag.DocumentosComite = null;
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            //TODO: Retornar View Model
            return View();
        }

        public ActionResult AdminComite(int comiteId)
        {
            ComiteModel comiteModel = new ComiteModel();
            try
            {
                ServicioTipos servicioTipos = new ServicioTipos();
                ServicioComites servicioComites = new ServicioComites();
                ServicioParticipantes servicioParticipantes = new ServicioParticipantes();
                ServicioEntidadesMusealesPreRegistro servicioPostulantes = new ServicioEntidadesMusealesPreRegistro();

                List<ParticipanteModel> todosParticipantes = null;
                List<PostulacionEntidadMusealModel> todosPostulantes = null;

                comiteModel = servicioComites.CargarComitesModel(comiteId);
                comiteModel = (comiteModel == null ? new ComiteModel() : comiteModel);

                // ObtenerUltimoId
                comiteModel.NumeroActa = comiteModel.Id == 0 ? servicioComites.ObtenerUltimoId().ToString() + " - " + DateTime.Now.Year.ToString() : comiteModel.NumeroActa;

                // Fecha
                comiteModel.Fecha = comiteModel.Fecha == DateTime.MinValue ? DateTime.Now : comiteModel.Fecha;
                comiteModel.Fecha = comiteId == 0 ? comiteModel.Fecha.AddDays(1) : comiteModel.Fecha;
                comiteModel.FechaString = comiteModel.Fecha.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                //Hora Actual
                TimeSpan horaActual = DateTime.Now.TimeOfDay;
                comiteModel.Hora = new TimeSpan(horaActual.Hours, horaActual.Minutes, horaActual.Seconds);
                todosParticipantes = servicioParticipantes.CargarParticipantesModel(null, false);

                // Lista Postulantes
                todosPostulantes = servicioPostulantes.CargarPostulantesDisponiblesModel();

                // Estados Entidades Museales
                List<TiposModel> estadoEntidadesMuseales = servicioTipos.CargarEstadosEntidadesMuseales();

                comiteModel.EsValidoPostulantesModificados = true;
                comiteModel.EsValidoRegistrosModificados = true;

                Session["idComite"] = comiteId;
                ViewBag.TodosParticipantes = todosParticipantes;
                ViewBag.TodosPostulantes = todosPostulantes;

                ViewData["EstadoEntidadesMuseales"] = estadoEntidadesMuseales;
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return View(comiteModel);
        }

        [HttpPost]
        public ActionResult AdminComite(ComiteModel comiteModel)
        {
            //Validación Participantes
            List<ComiteParticipanteModel> listaParticipantes = new JavaScriptSerializer().Deserialize<List<ComiteParticipanteModel>>(comiteModel.RegistrosModificados);
            bool validacionParticipantes = true;

            if (listaParticipantes.Count < 3)
            {
                validacionParticipantes = false;
            }
            comiteModel.EsValidoRegistrosModificados = validacionParticipantes;

            //Validación  Postulantes
            List<ComitePostulanteModel> listaPostulantes = new JavaScriptSerializer().Deserialize<List<ComitePostulanteModel>>(comiteModel.PostulantesModificados);
            bool validacionPostulados = true;

            if (listaPostulantes.Count < 1)
            {
                validacionPostulados = false;
            }
            comiteModel.EsValidoPostulantesModificados = validacionPostulados;

            if (ModelState.IsValid && validacionParticipantes && validacionPostulados)
            {
                try
                {
                    comiteModel.Fecha = DateTime.ParseExact(comiteModel.FechaString, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    ServicioComites servicioComites = new ServicioComites();
                    comiteModel.NumeroActa = comiteModel.Id == 0 ? servicioComites.ObtenerUltimoId().ToString() + " - " + DateTime.Now.Year.ToString() : comiteModel.NumeroActa;
                    var comiteEntity = servicioComites.CrearActualizarComite(comiteModel);

                    if (comiteModel.RegistrosModificados != null && comiteModel.RegistrosModificados != "[]")
                    {
                        List<ComiteParticipanteModel> listado = new JavaScriptSerializer().Deserialize<List<ComiteParticipanteModel>>(comiteModel.RegistrosModificados);
                        ServicioComitesParticipantes servicioComitesParticipantes = new ServicioComitesParticipantes();
                        servicioComitesParticipantes.ActualizarComiteParticipantesComite(listado, comiteEntity.COM_ID);
                    }

                    if (comiteModel.PostulantesModificados != null && comiteModel.PostulantesModificados != "[]")
                    {
                        List<ComitePostulanteModel> listado = new JavaScriptSerializer().Deserialize<List<ComitePostulanteModel>>(comiteModel.PostulantesModificados);
                        ServicioComitesParticipantes servicioComitesParticipantes = new ServicioComitesParticipantes();
                        servicioComitesParticipantes.ActualizarComitePostulantesComite(listado, comiteEntity.COM_ID);
                    }

                    if (comiteEntity != null)
                    {
                        Session.Remove("idComite");
                        comiteModel.TempRegistrosModificados = string.Empty;
                        comiteModel.TempPostulantesModificados = string.Empty;
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

                // Lista Postulantes
                ServicioEntidadesMusealesPreRegistro servicioPostulantes = new ServicioEntidadesMusealesPreRegistro();
                ServicioParticipantes servicioParticipantes = new ServicioParticipantes();
                List<ParticipanteModel> todosParticipantes = null;
                List<PostulacionEntidadMusealModel> todosPostulantes = null;

                todosParticipantes = servicioParticipantes.CargarParticipantesModel(null, false);
                todosPostulantes = servicioPostulantes.CargarPostulantesDisponiblesModel();

                ViewBag.TodosParticipantes = todosParticipantes;
                ViewBag.TodosPostulantes = todosPostulantes;

                comiteModel.TempRegistrosModificados = comiteModel.RegistrosModificados;
                comiteModel.TempPostulantesModificados = comiteModel.PostulantesModificados;
                return View(comiteModel);
            }

            return RedirectToActionPermanent("Index", "Comites", new { comiteId = 0 });
        }
        #endregion

        //Eliminar Comité
        public ActionResult ElimiinarComite(int ComiteId)
        {
            try
            {

                if (ComiteId != 0)
                {
                    var servicioComites = new ServicioComites();

                    var comiteEntity = servicioComites.EliminarComite(ComiteId);

                    if (comiteEntity == true)
                    {
                        this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                    }
                    else
                    {
                        this.ShowMessage(MessageType.Alert, Constante.ErrorEliminacion, true);
                    }
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Index", "Comites", new { ComiteId = 0 });

        }

        #region Administracion de Participantes

        public ActionResult ConsultarParticipantesSeleccionados([DataSourceRequest] DataSourceRequest request)
        {
            ServicioComitesParticipantes servicioComitesParticipantes = new ServicioComitesParticipantes();
            List<ComiteParticipanteModel> comiteParticipante = null;
            List<ParticipanteModel> participantes;

            int comiteId = 0;
            if (Session["idComite"] != null)
                comiteId = (int)Session["idComite"];

            comiteParticipante = servicioComitesParticipantes.CargarComiteParticipantes(comiteId);

            participantes = (from lista in comiteParticipante
                             select lista.Participantes).ToList();

            return Json(participantes.ToDataSourceResult(request));
        }

        //[HttpPost]
        // public virtual JsonResult EliminarParticipantesSeleccionados(ParticipanteModel participante)
        public ActionResult EliminarParticipantesSeleccionados(ParticipanteModel participante)
        {
            ServicioComitesParticipantes servicioComitesParticipantes = new ServicioComitesParticipantes();

            int comiteId = 0;
            if (Session["idComite"] != null)
                comiteId = (int)Session["idComite"];

            var resultado = servicioComitesParticipantes.EliminarParticipantesComite(comiteId, participante.Id);
            return Json(new { EliminarParticipantesSeleccionados = resultado });
        }

        #endregion

        #region Administracion de 
        public ActionResult ConsultarPostulantesSeleccionados([DataSourceRequest] DataSourceRequest request)
        {
            ServicioComitesParticipantes servicioComitesParticipantes = new ServicioComitesParticipantes();
            List<ComitePostulanteModel> comiteParticipante = null;
            List<PostulacionEntidadMusealModel> postulantes;

            int comiteId = 0;
            if (Session["idComite"] != null)
                comiteId = (int)Session["idComite"];

            comiteParticipante = servicioComitesParticipantes.CargarComitePostulantes(comiteId);

            postulantes = (from lista in comiteParticipante
                           select lista.Postulantes).ToList();

            for (int i = 0; i < postulantes.Count; i++)
            {
                if (postulantes[i].Comentarios == null)
                {
                    postulantes[i].Comentarios = "";
                }
            }

            return Json(postulantes.ToDataSourceResult(request));
        }

        [HttpPost]
        public virtual JsonResult EliminarPostulantesSeleccionados(PostulacionEntidadMusealModel participante)
        {
            ServicioComitesParticipantes servicioComitesParticipantes = new ServicioComitesParticipantes();

            int comiteId = 0;
            if (Session["idComite"] != null)
                comiteId = (int)Session["idComite"];

            var resultado = servicioComitesParticipantes.EliminarPostulanteComite(comiteId, participante.Id);
            return Json(new { EliminarPostulantesSeleccionados = resultado });
        }

        [HttpPost]
        public virtual JsonResult ActualizarPostulantesSeleccionados(PostulacionEntidadMusealModel participante)
        {
            ServicioEntidadesMusealesPreRegistro servicioComitesParticipantes = new ServicioEntidadesMusealesPreRegistro();
            bool resultado = servicioComitesParticipantes.ActualizarPostulacionEntidadMuseal(participante);
            return Json(new { ActualizarPostulantesSeleccionados = resultado });
        }

        #endregion

    }

}