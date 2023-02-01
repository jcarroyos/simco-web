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
    public class EncuestaController : BaseController
    {

        public ActionResult Index()
        {
            try
            {
                return this.ObtenerView();
            }
            catch (Exception)
            {
                this.ShowMessage(MessageType.Error, Constante.ErrorCargarEncuestas, true);
            }

            return RedirectToActionPermanent("Index", "Inicio");
        }

        private ActionResult ObtenerView()
        {
            ServicioEncuentas servicioEncuentas = new ServicioEncuentas();
            ServicioEntidadesMuseales servicioEntidades = new ServicioEntidadesMuseales();

            InicioSesionModel inicioSesionModel = new InicioSesionModel();
            inicioSesionModel.EncuestasDisponibles = servicioEncuentas.ObtenerEncuestasDisponibles(UsuarioId);
            inicioSesionModel.EncuestasPreguntas = new List<EncuestaPreguntasRegistroModel>();
            inicioSesionModel.EntidadMuseal = servicioEntidades.ObtenerEntidadMuseal(UsuarioId);

            EncuestaPreguntasRegistroModel encuestaPreguntasRegistroModel = new EncuestaPreguntasRegistroModel();
            foreach (EncuestasActivas item in inicioSesionModel.EncuestasDisponibles)
            {
                encuestaPreguntasRegistroModel = servicioEncuentas.ObtenerRespuestasEncuesta(item.ENC_ID, UsuarioId);
                inicioSesionModel.EncuestasPreguntas.Add(encuestaPreguntasRegistroModel);
            }

            ViewBag.ExtensionesImagen = Utilities.Documents.ObtenerExtensionesImagen("TiposArchivo");
            ViewBag.ExtensionesDocumento = Utilities.Documents.ObtenerExtensionesDocumento("TiposArchivo");

            return View(inicioSesionModel);
        }

        public ActionResult Encuesta(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    return this.ObtenerView(id);
                }
            }
            catch (Exception)
            {
                this.ShowMessage(MessageType.Error, Constante.ErrorCargarEncuesta, true);
            }

            return RedirectToActionPermanent("Index", "Inicio");
        }

        private ActionResult ObtenerView(string id)
        {
            ServicioEncuentas servicioEncuentas = new ServicioEncuentas();
            EncuestaPreguntasRegistroModel encuestaPreguntasRegistroModel = servicioEncuentas.ObtenerRespuestasEncuesta(Convert.ToInt32(Criptografia.DesencriptarUrl(id)), UsuarioId);

            if (encuestaPreguntasRegistroModel.Encuesta != null && encuestaPreguntasRegistroModel.Encuesta.Nombre.Contains("documentos"))
            {
                ServicioEntidadesMuseales servicioEntidades = new ServicioEntidadesMuseales();
                encuestaPreguntasRegistroModel.EntidadMuseal = servicioEntidades.ObtenerEntidadMuseal(UsuarioId);
            }

            ViewBag.ExtensionesImagen = Utilities.Documents.ObtenerExtensionesImagen("TiposArchivo");
            ViewBag.ExtensionesDocumento = Utilities.Documents.ObtenerExtensionesDocumento("TiposArchivo");

            return View(encuestaPreguntasRegistroModel);
        }

        [HttpPost]
        public ActionResult Encuesta(FormCollection formCollection)
        {
            string btnAccion = "BtnGuardarCambios";

            try
            {
                EntidadMusealModel entidadMusealModel = new EntidadMusealModel();

                #region "Obtener Encuesta Diliganciada"
                DataTable dataTable = new DataTable();
                DataColumn dcPregunta = new DataColumn("Pregunta", Type.GetType("System.Int32"));
                DataColumn dcOpcion = new DataColumn("Opcion", Type.GetType("System.Int32"));
                DataColumn dcValor = new DataColumn("Valor", Type.GetType("System.String"));
                DataColumn dcIdEncuesta = new DataColumn("IdEncuesta", Type.GetType("System.Int32"));

                int estado = (int)EstadosRespuestas.PorTerminar;

                dataTable.Columns.Add(dcPregunta);
                dataTable.Columns.Add(dcOpcion);
                dataTable.Columns.Add(dcValor);
                dataTable.Columns.Add(dcIdEncuesta);

                int encuestaId = -1;
                int entidadMusealId = 0;

                foreach (var fc in formCollection)
                {
                    if (fc.ToString().Equals("__RequestVerificationToken"))
                        continue;

                    if (fc.ToString().Equals("Encuesta.Id") || fc.ToString().Contains("EncuestaId"))
                    {
                        if (!string.IsNullOrEmpty(Request.Form[fc.ToString()]))
                        {
                            int.TryParse(Request.Form[fc.ToString()], out encuestaId);
                            if (encuestaId == 0)
                                int.TryParse(Request.Form[fc.ToString()].Replace("EncuestaId", string.Empty), out encuestaId);
                        }

                        continue;
                    }

                    if (fc.ToString().Equals("EntidadMusealId"))
                    {
                        if (!string.IsNullOrEmpty(Request.Form[fc.ToString()]))
                            int.TryParse(Request.Form[fc.ToString()], out entidadMusealId);

                        entidadMusealModel.Id = entidadMusealId;
                        continue;
                    }

                    if (fc.ToString().Contains("_"))
                    {
                        string[] temp = fc.ToString().Split('_');

                        if (fc.ToString().EndsWith("_Valor_Otro"))
                        {
                            int pregunta;
                            if (int.TryParse(temp[0], out pregunta))
                            {
                                string valor = Request.Form[fc.ToString()];
                                int opcion;
                                if (!string.IsNullOrEmpty(valor) && int.TryParse(temp[1], out opcion))
                                {
                                    DataRow dataRow = dataTable.NewRow();
                                    dataRow["Pregunta"] = pregunta;
                                    dataRow["Opcion"] = opcion;
                                    dataRow["Valor"] = valor;
                                    dataRow["IdEncuesta"] = encuestaId;

                                    dataTable.Rows.Add(dataRow);
                                }
                            }
                        }
                        else if (fc.ToString().EndsWith("_Valor") || fc.ToString().EndsWith("_Otro"))
                        {
                            int pregunta;
                            if (int.TryParse(temp[0], out pregunta))
                            {
                                string valor = Request.Form[fc.ToString()];
                                int opcion;
                                if (!string.IsNullOrEmpty(valor) && int.TryParse(temp[1], out opcion))
                                {
                                    DataRow dataRow = dataTable.NewRow();
                                    dataRow["Pregunta"] = pregunta;
                                    dataRow["Opcion"] = opcion;
                                    dataRow["Valor"] = valor;
                                    dataRow["IdEncuesta"] = encuestaId;

                                    dataTable.Rows.Add(dataRow);
                                }
                            }
                        }
                        else
                        {
                            int pregunta;
                            if (int.TryParse(temp[0], out pregunta))
                            {
                                string valor = Request.Form[fc.ToString()];
                                int opcion;
                                if (valor.Equals("true,false") && int.TryParse(temp[1], out opcion))
                                {
                                    DataRow dataRow = dataTable.NewRow();
                                    dataRow["Pregunta"] = pregunta;
                                    dataRow["Opcion"] = opcion;
                                    dataRow["Valor"] = string.Empty;
                                    dataRow["IdEncuesta"] = encuestaId;

                                    dataTable.Rows.Add(dataRow);
                                }
                            }
                        }
                    }
                    else
                    {
                        int pregunta;
                        if (int.TryParse(fc.ToString(), out pregunta))
                        {
                            var t = Request.Form[fc.ToString()];
                            int opcion;
                            if (int.TryParse(t, out opcion))
                            {
                                DataRow dataRow = dataTable.NewRow();
                                dataRow["Pregunta"] = pregunta;
                                dataRow["Opcion"] = opcion;
                                dataRow["Valor"] = string.Empty;
                                dataRow["IdEncuesta"] = encuestaId;

                                dataTable.Rows.Add(dataRow);
                            }
                        }
                        else
                        {
                            if (fc.ToString().Contains("Btn"))
                            {
                                btnAccion = fc.ToString();
                            }
                        }
                    }
                }
                #endregion "Obtener Encuesta Diliganciada"

                entidadMusealModel.DocumentoFotoFachada = Request.Files["EntidadMuseal.DocumentoFotoFachada"];
                entidadMusealModel.DocumentoFotoInteraccion = Request.Files["EntidadMuseal.DocumentoFotoInteraccion"];
                entidadMusealModel.DocumentoFotoColeccion1 = Request.Files["EntidadMuseal.DocumentoFotoColeccion1"];
                entidadMusealModel.DocumentoFotoColeccion2 = Request.Files["EntidadMuseal.DocumentoFotoColeccion2"];
                entidadMusealModel.DocumentoFotoColeccion3 = Request.Files["EntidadMuseal.DocumentoFotoColeccion3"];
                entidadMusealModel.DocumentoLegalExistencia = Request.Files["EntidadMuseal.DocumentoLegalExistencia"];
                entidadMusealModel.DocumentoPoliticaPlanColecciones = Request.Files["EntidadMuseal.DocumentoPoliticaPlanColecciones"];

                string resultadoValidacion = string.Empty;
                Dictionary<string, string> modelError = new Dictionary<string, string>();

                if (!Utilities.Documents.ValidarDocumentosEntidadMuseal(entidadMusealModel, out resultadoValidacion, out modelError))
                {
                    foreach (var item in modelError)
                        ModelState.AddModelError(item.Key, item.Value);

                    this.ShowMessage(MessageType.Error, resultadoValidacion, true);

                    if(Request.Path.EndsWith("/Encuesta/Encuesta") && btnAccion == "BtnEnviar")
                        return RedirectToActionPermanent("Index", "Encuesta"); 
                    else
                    {
                        if (!string.IsNullOrEmpty(Request.Path.Split('/')[Request.Path.Split('/').Length - 1]))
                        {
                            return this.ObtenerView(Request.Path.Split('/')[Request.Path.Split('/').Length - 1]);
                        }
                        else
                            return RedirectToActionPermanent("Encuesta", "Encuesta");
                    }
                }
                else
                {
                    if (btnAccion == "BtnEnviar")
                        estado = (int)EstadosRespuestas.Finalizada;

                    ServicioEncuentas servicioEncuentas = new ServicioEncuentas();
                    if (servicioEncuentas.RegistrarRespuestas(UsuarioId, dataTable, estado))
                        this.ShowMessage(MessageType.Success, Constante.MensajeGuardarEncuesta, true);
                    else
                        this.ShowMessage(MessageType.Alert, Constante.ErrorRegistrarEncuestas);

                    ServicioEntidadesMuseales servicioEntidades = new ServicioEntidadesMuseales();
                    ENTIDADESMUSEALES entidadMusealEntity = new ENTIDADESMUSEALES();
                    servicioEntidades.ActualizarDocumentoEntidadMuseal(entidadMusealModel, UsuarioId, out entidadMusealEntity);

                    if (entidadMusealEntity != null)
                    {
                        ServicioPersonasJuridicas servicioPersonasJuridicas = new ServicioPersonasJuridicas();
                        servicioPersonasJuridicas.ActualizarDocumentoPersonaJuridica(entidadMusealEntity, entidadMusealEntity.PEJ_ID);
                    }

                    SolicitarActivacionUsuario();
                    servicioEncuentas.IntentarActualizarEstadoActivacion(UsuarioId); 
                }

            }
            catch (Exception)
            {
                this.ShowMessage(MessageType.Error, Constante.ErrorRegistrarEncuestas, true);
            }

            if (Request.Path == "/Encuesta/Encuesta" && btnAccion == "BtnGuardarCambios")
                return RedirectToActionPermanent("Index", "Encuesta");
            else
                return RedirectToActionPermanent("Index", "Inicio");
        }

        private void SolicitarActivacionUsuario()
        {
            ServicioEncuentas servicioEncuentas = new ServicioEncuentas();
            InicioSesionModel inicioSesionModel = new InicioSesionModel();
            inicioSesionModel.EncuestasDisponibles = servicioEncuentas.ObtenerEncuestasDisponibles(UsuarioId);

            if (inicioSesionModel.EncuestasDisponibles.Count == 1)
            {
                ServicioUsuarios servicioUsuarios = new ServicioUsuarios();
                servicioUsuarios.CambiarEstado(UsuarioId, UsuarioEstados.PreActivo);

                // Registrar solicitud y Enviar solicitud de activación
                ServicioSolicitudes servicioSolicitudes = new ServicioSolicitudes();
                servicioSolicitudes.RegistrarSolicitud(null, UsuarioId, Constante.MensajeSolicitudActivacionUsuario);
            }
        }

        public ActionResult HistorialEncuestas()
        {
            try
            {
                ServicioEncuentas servicioEncuentas = new ServicioEncuentas();
                return View(servicioEncuentas.ObtenerModeloConsultaIndicadoresPorUsuarioID(UsuarioId));
            }
            catch (Exception)
            {
                this.ShowMessage(MessageType.Error, Constante.ErrorRegistrarEncuestas, true);
            }

            return RedirectToActionPermanent("Encuesta", "HistorialEncuestas");
        }


        [HttpPost]
        public JsonResult ListaRespuestas(string encuestaId)
        {
            try
            {
                if (UsuarioId != 0 && !string.IsNullOrEmpty(encuestaId))
                {
                    ServicioEncuentas servicioEncuentas = new ServicioEncuentas();
                    return Json(new { items = servicioEncuentas.ObtenerRespuestas(UsuarioId, Convert.ToInt32(encuestaId)) });
                }
            }
            catch (Exception)
            {
                this.ShowMessage(MessageType.Error, Constante.ErrorRegistrarEncuestas, true);
            }

            return Json(null);
        }

    }
}