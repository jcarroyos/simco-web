using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Simco.ApplicationLayer.Servicios;
using Simco.CrossInfraestructureLayer;
using Simco.DomainModelLayer.ViewModels;
using Simco.Infrastructure.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Simco.Controllers
{
    [Authorize]
    public class ParticipantesController : BaseController
    {
        #region Administración de Participantes
        public ActionResult Index(string consulta)
        {
            ConsultarParticipantes(consulta);
            return View();
        }

        public ActionResult ConsultarParticipantes(string consulta)
        {
            try
            {
                ViewBag.Consulta = consulta != null ? consulta : string.Empty;

            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            //TODO: Retornar View Model
            return View();
        }

        public ActionResult AdminParticipante(int participanteId)
        {
            ParticipanteModel participanteModel = new ParticipanteModel();
            try
            {
                ServicioParticipantes servicioParticipantes = new ServicioParticipantes();
                participanteModel = servicioParticipantes.CargarParticipantesModel(participanteId);
                if (participanteModel == null)
                {
                    participanteModel = new ParticipanteModel();
                    participanteModel.Activo = true;
                }

                List<ComiteModel> comites = new List<ComiteModel>();

                ServicioComitesParticipantes servicioComitesParticipantes = new ServicioComitesParticipantes();
                var seleccionados = servicioComitesParticipantes.CargarComiteParticipantesParticipante(participanteId);

                foreach (ComiteParticipanteModel comite in seleccionados)
                {
                    comites.Add(comite.Comites);
                }

                Session["idParticipante"] = participanteId;
                ViewBag.Comites = comites;
                ViewBag.MostrarEliminarComites = seleccionados.Count == 0 && participanteId != 0; //Mostrar boton de Eliminar Participante, si no tiene comites
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            //TODO: Retornar View Model
            return View(participanteModel);
        }

        public ActionResult EliminarParticipante(int participanteId)
        {
            ServicioParticipantes servicioParticipantes = new ServicioParticipantes();
            bool resultado = true;

            try
            {
                resultado = servicioParticipantes.EliminarParticipantes(participanteId);

                if (resultado)
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

            return RedirectToActionPermanent("Index", "Participantes");
        }

        [HttpPost]
        public ActionResult AdminParticipante(ParticipanteModel participanteModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ServicioParticipantes servicioParticipantes = new ServicioParticipantes();
                    var ParticipanteEntity = servicioParticipantes.CrearActualizarParticipante(participanteModel);

                    if (participanteModel.RegistrosModificados != null && participanteModel.RegistrosModificados != "[]")
                    {
                        List<ComiteParticipanteModel> listado = new JavaScriptSerializer().Deserialize<List<ComiteParticipanteModel>>(participanteModel.RegistrosModificados);
                        ServicioComitesParticipantes servicioComitesParticipantes = new ServicioComitesParticipantes();
                        servicioComitesParticipantes.ActualizarComiteParticipantes(listado, ParticipanteEntity.PAR_ID);
                    }

                    if (ParticipanteEntity != null)
                    {
                        Session.Remove("idParticipante");
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

            return RedirectToActionPermanent("Index", "Participantes");
        }

        public ActionResult ParticipantesConsultar([DataSourceRequest] DataSourceRequest request, string consulta)
        {
            ServicioParticipantes servicioParticipantes = new ServicioParticipantes();
            List<ParticipanteModel> Participantes = servicioParticipantes.CargarParticipantesModel(consulta, true);
            return Json(Participantes.ToDataSourceResult(request));
        }

        [HttpPost]
        public virtual JsonResult ParticipantesEliminar(ParticipanteModel participante)
        {
            ServicioParticipantes servicioParticipantes = new ServicioParticipantes();
            bool resultado = true;

            if (participante.ComiteParticipantes == null)
                resultado = servicioParticipantes.EliminarParticipantes(participante.Id);
            else
                if (participante.ComiteParticipantes.Count == 0)
                resultado = servicioParticipantes.EliminarParticipantes(participante.Id);
            else
                resultado = false;

            return Json(new { EliminarParticipantes = resultado });
        }


        #endregion

    }

}