using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Simco.ApplicationLayer.Servicios;
using Simco.CrossInfraestructureLayer;
using Simco.DomainModelLayer.ViewModels;
using Simco.Infrastructure.Notification;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Simco.Controllers
{
    [Authorize]
    public class PlanFortalecimientoController : BaseController
    {
        #region "Certificados PDF"

        public ActionResult DescargarCertificado()
        {
            try
            {
                return View();
                //return View(new PlanFortalecimientoModel());
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Index", "Inicio");
        }

        public FileContentResult GenerarPDFCertificado()
        {
            try
            {
                ServicioPlanesFortalecimiento servicioPlanesFortalecimiento = new ServicioPlanesFortalecimiento();
                PlanFortalecimientoModel planFortalecimientoModel = servicioPlanesFortalecimiento.GenerarPDFCertificadoRegistro(PersonaId);

                if (planFortalecimientoModel.ArchivoPDF != null)
                {
                    byte[] archivo = System.IO.File.ReadAllBytes(planFortalecimientoModel.ArchivoPDF.ToString());
                    HttpContext.Response.AddHeader("content-disposition", "attachment; filename=PlantillaCertificadoRegistro.pdf");
                    return File(archivo, "application/pdf");
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return null;
        }

        public FileContentResult GenerarPDFPlanPersona(int personaId, int planId)
        {
            return GenerarPDF(personaId, planId);
        }

        #endregion

        #region "Categorías"

        public ActionResult CargarCategorias(string consulta)
        {
            if (PersonaId != 0)
            {
                try
                {
                    Session.Remove("CategoriaId");
                    Session.Remove("TemaId");
                    ViewBag.Consulta = consulta;

                    ServicioPlanesFortalecimiento servicioPlanesFortalecimiento = new ServicioPlanesFortalecimiento();
                    return View(servicioPlanesFortalecimiento.ObtenerCategorias(consulta));
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return RedirectToActionPermanent("CargarCategorias", "PlanFortalecimiento");
        }

        public ActionResult NuevaCategoria(CategoriaNuevaModel categoriaNuevaModel, string id, string pantalla)
        {
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    ServicioPlanesFortalecimiento servicioPlanesFortalecimiento = new ServicioPlanesFortalecimiento();
                    categoriaNuevaModel = servicioPlanesFortalecimiento.ObtenerCategoriaPorId(pantalla, Convert.ToInt32(Criptografia.DesencriptarUrl(id)));
                    categoriaNuevaModel.PantallaMostrar = pantalla;

                    return View(categoriaNuevaModel);
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }
            else
            {
                try
                {
                    return View(new CategoriaNuevaModel());
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return RedirectToActionPermanent("CargarCategorias", "PlanFortalecimiento");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult NuevaCategoria(CategoriaNuevaModel categoriaNuevaModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["CategoriaId"] != null && categoriaNuevaModel.Categoria.Id == 0)
                        categoriaNuevaModel.Categoria.Id = (int)Session["CategoriaId"];

                    if (Session["TemaId"] != null && categoriaNuevaModel.Tema.Id == 0)
                        categoriaNuevaModel.Tema.Id = (int)Session["TemaId"];

                    ServicioPlanesFortalecimiento servicioPlanesFortalecimiento = new ServicioPlanesFortalecimiento();
                    if (servicioPlanesFortalecimiento.CrearActualizarCategoria(categoriaNuevaModel))
                    {
                        Session["CategoriaId"] = categoriaNuevaModel.Categoria.Id;

                        if (categoriaNuevaModel.PantallaMostrar == "Tema")
                            Session["TemaId"] = categoriaNuevaModel.Tema.Id;

                        if (categoriaNuevaModel.PantallaMostrar == "Categoria")
                            Session.Remove("TemaId");

                        if (categoriaNuevaModel.PantallaMostrar == "NuevoCategoria")
                        {
                            Session.Remove("CategoriaId");
                            Session.Remove("TemaId");

                            this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                            return RedirectToActionPermanent("CargarCategorias", "PlanFortalecimiento");
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return View(categoriaNuevaModel);
        }

        #endregion

        #region "Planes"

        public ActionResult CargarPlanes(string consulta)
        {
            if (PersonaId != 0)
            {
                try
                {
                    ViewBag.Consulta = consulta;
                    ServicioPlanesFortalecimiento servicioPlanesFortalecimiento = new ServicioPlanesFortalecimiento();
                    return View(servicioPlanesFortalecimiento.ObtenerPlanes(consulta));
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return RedirectToActionPermanent("Index", "Home");
        }

        public ActionResult MostrarPlanes(string consulta)
        {
            if (PersonaId != 0)
            {
                try
                {
                    ViewBag.Consulta = consulta;
                    ServicioPlanesFortalecimiento servicioPlanesFortalecimiento = new ServicioPlanesFortalecimiento();
                    return View(servicioPlanesFortalecimiento.ObtenerPlanes(consulta));
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return RedirectToActionPermanent("Index", "Home");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Eliminar(int id)
        {
            if (PersonaId != 0)
            {
                try
                {
                    ServicioPlanesFortalecimiento servicioPlanesFortalecimiento = new ServicioPlanesFortalecimiento();
                    servicioPlanesFortalecimiento.EliminarPlan(id);

                    ViewBag.Consulta = string.Empty;
                    return Json(servicioPlanesFortalecimiento.ObtenerPlanes(string.Empty), JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return RedirectToActionPermanent("CargarPlanes", "PlanFortalecimiento");
        }

        public ActionResult NuevoPlan(string id)
        {
            try
            {
                ServicioPlanesFortalecimiento servicioPlanesFortalecimiento = new ServicioPlanesFortalecimiento();
                if (!string.IsNullOrEmpty(id))
                    return View(servicioPlanesFortalecimiento.ObtenerPlan(Convert.ToInt32(Criptografia.DesencriptarUrl(id))));
                else
                    return View(servicioPlanesFortalecimiento.NuevoPlan(UsuarioId));

            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("CargarPlanes", "PlanFortalecimiento");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult NuevoPlan(PlanConsultaModel planConsultaModel)
        {
            PlanConsultaModel nuevoPlanConsultaModel = new PlanConsultaModel();
            if (ModelState.IsValid)
            {
                try
                {

                    if (planConsultaModel.RegistrosModificados != null && planConsultaModel.RegistrosModificados != "[]")
                        planConsultaModel.PlanTemas = new JavaScriptSerializer().Deserialize<List<PlanTemaModel>>(planConsultaModel.RegistrosModificados);

                    ServicioPlanesFortalecimiento servicioPlanesFortalecimiento = new ServicioPlanesFortalecimiento();
                    if (servicioPlanesFortalecimiento.CrearActualizarPlan(planConsultaModel))
                    {
                        if (planConsultaModel.Notificar)
                            EnviarNotificacionPlanUsuario(planConsultaModel);
                        else
                            this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);

                        return RedirectToActionPermanent("CargarPlanes", "PlanFortalecimiento");
                    }

                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return View(planConsultaModel);
        }

        [ValidateInput(false)]
        public ActionResult CrearPlan(int pejId, string anio, string nombrePlan, string diagnostico, string registrosModificados)
        {
            PlanConsultaModel nuevoPlanConsultaModel = new PlanConsultaModel();
            PlanConsultaModel planConsultaModel = new PlanConsultaModel();

            try
            {
                planConsultaModel.PlanTemas = new List<PlanTemaModel>();
                planConsultaModel.Plan = new PlanModel();
                planConsultaModel.Plan.PejId = pejId;
                planConsultaModel.Plan.Anio = anio;
                planConsultaModel.Plan.Plan = nombrePlan;
                planConsultaModel.Plan.Diagnostico = diagnostico;

                if (registrosModificados != null && registrosModificados != "[]")
                    planConsultaModel.PlanTemas = new JavaScriptSerializer().Deserialize<List<PlanTemaModel>>(registrosModificados);

                ServicioPlanesFortalecimiento servicioPlanesFortalecimiento = new ServicioPlanesFortalecimiento();
                if (servicioPlanesFortalecimiento.CrearActualizarPlan(planConsultaModel))
                {
                    if (planConsultaModel.Notificar)
                        EnviarNotificacionPlanUsuario(planConsultaModel);
                    else
                        this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                }

            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public void EnviarNotificacionPlanUsuario(PlanConsultaModel planConsultaModel)
        {
            try
            {
                ServicioPlanesFortalecimiento servicioPlanesFortalecimiento = new ServicioPlanesFortalecimiento();
                if (servicioPlanesFortalecimiento.EnviarNotificacionPlanEntidad(planConsultaModel))
                    this.ShowMessage(MessageType.Success, Constante.MensajePlanFortalecimientoEnviarPlan, true);
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }
        }

        public ActionResult CargarObjetivo(int categoriaId, int nivel)
        {
            try
            {
                ServicioPlanesFortalecimiento servicioPlanesFortalecimiento = new ServicioPlanesFortalecimiento();
                return Json(servicioPlanesFortalecimiento.ObtenerTema(categoriaId, nivel), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }
            return null;
        }

        #endregion

        #region "PlanesPorMuseo"

        public ActionResult PlanesPorMuseo()
        {
            if (PersonaId != 0)
            {
                try
                {
                    ServicioPlanesFortalecimiento servicioPlanesFortalecimiento = new ServicioPlanesFortalecimiento();
                    PlanActividadModel planActividadModel = servicioPlanesFortalecimiento.ObtenerPlanPorMuseo(PersonaId);
                    if (planActividadModel != null)
                    {
                        Session["PlanActividadModel"] = planActividadModel;
                        return View(planActividadModel);
                    }
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return RedirectToActionPermanent("Index", "Inicio");
        }

        public ActionResult ObtenerActividadesPlan([DataSourceRequest]DataSourceRequest request)
        {
            if (PersonaId != 0)
            {
                try
                {
                    ServicioPlanesFortalecimiento servicioPlanesFortalecimiento = new ServicioPlanesFortalecimiento();
                    IQueryable<TemaModel> actividades = servicioPlanesFortalecimiento.ObtenerActividadesPlan(PersonaId);
                    DataSourceResult result = actividades.ToDataSourceResult(request, actividad => new
                    {
                        actividad.Id,
                        actividad.Objetivo,
                        actividad.Meta,
                        actividad.Indicador,
                        actividad.RecursoSugerido,
                        actividad.MedioVerificacion
                    });

                    return Json(result);
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return RedirectToActionPermanent("Index", "Home");
        }

        public ActionResult ObtenerDetallePlan([DataSourceRequest]DataSourceRequest request, int TemaId)
        {
            if (PersonaId != 0)
            {
                try
                {
                    ServicioPlanesFortalecimiento servicioPlanesFortalecimiento = new ServicioPlanesFortalecimiento();
                    IQueryable<PlanesActividadesModel> actividades = servicioPlanesFortalecimiento.ObtenerPlanPorMuseoDetalle(PersonaId, TemaId);
                    DataSourceResult result = actividades.ToDataSourceResult(request, actividad => new
                    {
                        actividad.ActividadId,
                        actividad.Actividad,
                        actividad.Realizada,
                        actividad.Descripcion,

                    });

                    return Json(result);
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return RedirectToActionPermanent("Index", "Home");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PlanesPorMuseo(string registros)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(registros))
            {
                try
                {
                    PlanActividadModel planSeguimientoModel = (Session["PlanActividadModel"] != null ? (Session["PlanActividadModel"] as PlanActividadModel) : new PlanActividadModel());

                    string nombreMuseo = planSeguimientoModel.Plan.NombreMuseo;
                    bool registrarActividad = planSeguimientoModel.CambiarNivel;

                    List<PlanesActividadesModel> listaPlanTemaModel = new JavaScriptSerializer().Deserialize<List<PlanesActividadesModel>>(registros);
                    planSeguimientoModel.PlanesActividades = listaPlanTemaModel;

                    ServicioPlanesFortalecimiento servicioPlanesFortalecimiento = new ServicioPlanesFortalecimiento();
                    if (servicioPlanesFortalecimiento.CrearActualizarPlanPorMuseo(planSeguimientoModel, PersonaId))
                    {
                        this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);

                        if (registrarActividad)
                        {
                            EtiquetaDominioModel etiquetaDominioModel = new EtiquetaDominioModel();
                            SolicitudRegistroModel registroSolicitudModel = new SolicitudRegistroModel();

                            ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
                            etiquetaDominioModel = servicioConfiguracion.CargarEtiquetaDominio(NombreDominios.Solicitud_RegistroSolicitud_ModificarPlan);

                            registroSolicitudModel.Solicitud = new SolicitudModel();
                            registroSolicitudModel.Solicitud.Descripcion = String.Format(etiquetaDominioModel.Valor, nombreMuseo);

                            ServicioSolicitudes servicioSolicitudes = new ServicioSolicitudes();
                            if (servicioSolicitudes.RegistrarSolicitud(registroSolicitudModel, UsuarioId))
                                this.ShowMessage(MessageType.Success, Constante.MensajeSolicitudesCrear, true);

                        }
                    }
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return PlanesPorMuseo();
        }

        public FileContentResult GenerarPDFPlan(int planId)
        {
            if (PersonaId != 0)
                return GenerarPDF(PersonaId, planId);

            return null;
        }

        [HttpPost]
        public ActionResult ActualizarActividad(int id)
        {
            if (PersonaId != 0)
            {
                try
                {
                    PlanActividadModel planActividadModel = null;

                    if (Session["PlanActividadModel"] != null)
                        planActividadModel = (PlanActividadModel)Session["PlanActividadModel"];

                    ServicioPlanesFortalecimiento servicioPlanesFortalecimiento = new ServicioPlanesFortalecimiento();
                    return Json(servicioPlanesFortalecimiento.ActualizarActividad(PersonaId, planActividadModel, id, true), JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return RedirectToActionPermanent("CargarPlanes", "PlanFortalecimiento");
        }
        #endregion

        #region "Reportes"

        public ActionResult Reportes(PlanConsultaModel planReporteModel)
        {
            try
            {
                if (planReporteModel == null)
                    planReporteModel = new PlanConsultaModel();

                planReporteModel.UsarioId = UsuarioId;

                ServicioPlanesFortalecimiento servicioPlanesFortalecimiento = new ServicioPlanesFortalecimiento();
                return View(servicioPlanesFortalecimiento.ConsultarReporte(planReporteModel));
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Index", "Home");

        }

        #endregion

        #region "Semaforo"

        public ActionResult Semaforo(string nombreMuseo, string fechaActivacionInicial, string fechaActivacionFinal)
        {
            try
            {
                DateTime? dtFechaInicial = null;
                DateTime? dtFechaFinal = null;

                if (!string.IsNullOrEmpty(fechaActivacionInicial))
                    dtFechaInicial = DateTime.Parse(fechaActivacionInicial);

                if (!string.IsNullOrEmpty(fechaActivacionFinal))
                    dtFechaFinal = DateTime.Parse(fechaActivacionFinal);

                ServicioPlanesFortalecimiento servicioPlanesFortalecimiento = new ServicioPlanesFortalecimiento();
                return View(servicioPlanesFortalecimiento.CargarSemaforo(nombreMuseo, dtFechaInicial, dtFechaFinal));
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Index", "Home");
        }

        public ActionResult DesactivarUsuario(string idUsuario)
        {
            try
            {
                ServicioUsuarios servicioUsuarios = new ServicioUsuarios();
                return Json(servicioUsuarios.CambiarEstado(int.Parse(idUsuario), UsuarioEstados.PreActivo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Semaforo", "PlanFortalecimiento");
        }

        public ActionResult DesactivarUsuariosDesactualizados()
        {
            try
            {
                ServicioUsuarios servicioUsuarios = new ServicioUsuarios();
                int resultado = servicioUsuarios.DesactivarUsuariosDesactualizados();
                this.ShowMessage(MessageType.Success, string.Format(Constante.MensajeConfirmacionDesactivacion, resultado), true);

                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Semaforo", "PlanFortalecimiento");
        }

        public FileContentResult ExportarExcel(string nombreMuseo, DateTime? fechaActivacionInicial, DateTime? fechaActivacionFinal)
        {
            try
            {
                ServicioPlanesFortalecimiento servicioPlanesFortalecimiento = new ServicioPlanesFortalecimiento();
                FileInfo nombreArchivo = servicioPlanesFortalecimiento.GenerarExcelSemaforo(nombreMuseo, fechaActivacionInicial, fechaActivacionFinal);
                if (nombreArchivo.Exists)
                {
                    byte[] archivo = System.IO.File.ReadAllBytes(nombreArchivo.FullName);
                    HttpContext.Response.AddHeader("content-disposition", "attachment; filename=Semaforo.xls");
                    return File(archivo, "application/docx");
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return null;

        }

        #endregion

        #region "Común"

        public ActionResult ObtenerPersonaJuridicaDepto(string personaId)
        {
            ServicioPersonasJuridicas servicioPersonasJuridicas = new ServicioPersonasJuridicas();
            return Json(servicioPersonasJuridicas.ObtenerPersonaJuridicaDepto(Convert.ToInt32(personaId)), JsonRequestBehavior.AllowGet);
        }

        private FileContentResult GenerarPDF(int personaId, int idPlan)
        {
            try
            {
                ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
                List<EtiquetaDominioModel> configuraciones = servicioConfiguracion.CargarEtiquetasDominios(NombreDominios.Configuracion);
                ServicioPlanesFortalecimiento servicioPlanesFortalecimiento = new ServicioPlanesFortalecimiento();

                string nombrePlantilla;
                string nombreArchivoModificado = string.Empty;
                object NombrePDF = null;

                if (configuraciones.Any())
                {
                    EtiquetaDominioModel configuracion = configuraciones.Where(conf => conf.Etiqueta == "PlantillaPlanFortalecimiento").FirstOrDefault();
                    nombrePlantilla = configuracion.Valor;

                    configuracion = configuraciones.Where(conf => conf.Etiqueta == "CarpetadeArchivos").FirstOrDefault();
                    nombreArchivoModificado = string.Concat(configuracion.Valor, "PlantillaPlanFortalecimientoGenerando.pdf");

                    NombrePDF = servicioPlanesFortalecimiento.GenerarPDF(personaId, idPlan, nombrePlantilla, nombreArchivoModificado);
                }

                if (NombrePDF != null)
                {
                    byte[] archivo = System.IO.File.ReadAllBytes(nombreArchivoModificado);
                    HttpContext.Response.AddHeader("content-disposition", "attachment; filename=PlantillaPlanFortalecimientoGenerando.pdf");
                    return File(archivo, "application/docx");
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return null;
        }

        #endregion

    }
}