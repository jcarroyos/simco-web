using Simco.ApplicationLayer.Servicios;
using Simco.CrossInfraestructureLayer;
using Simco.DomainModelLayer.ViewModels;
using Simco.Infrastructure.Notification;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Simco.Controllers
{
    [Authorize]
    public class AdministradorController : BaseController
    {
        public ActionResult Index()
        {
            return RedirectToActionPermanent("SolicitudesAsignadas", "Administrador");
        }

        #region "Solicitudes"

        [AllowAnonymous]
        public ActionResult SolicitudesAsignadas(string consulta)
        {
            ViewBag.Consulta = consulta;
            ServicioSolicitudes servicioSolicitudes = new ServicioSolicitudes();
            return View(servicioSolicitudes.ObtenerSolicitudesAsignadas(UsuarioId, consulta));
        }

        public ActionResult SolicitudAsignada(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    ServicioSolicitudes servicioSolicitudes = new ServicioSolicitudes();
                    return View("SolicitudAsignada", servicioSolicitudes.ObtenerSolicitud(Convert.ToInt32(Criptografia.DesencriptarUrl(id)), UsuarioId));
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return RedirectToActionPermanent("SolicitudesAsignadas", "Administrador");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SolicitudAsignada(SolicitudRegistroModel registroSolicitudModel)
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
                    return RedirectToActionPermanent("SolicitudesAsignadas", "Administrador");
                }

            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("SolicitudesAsignadas", "Administrador");
        }

        public ActionResult CerrarSolicitud(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                SolicitudCerrarModel solicitudCerrarModel = new SolicitudCerrarModel { Token = id };
                return View(solicitudCerrarModel);
            }

            return RedirectToActionPermanent("Solicitudes", "Administrador");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CerrarSolicitud(SolicitudCerrarModel solicitudCerrarModel)
        {
            try
            {
                ServicioSolicitudes servicioSolicitudes = new ServicioSolicitudes();
                if (servicioSolicitudes.CerrarSolicitud(solicitudCerrarModel, UsuarioId))
                {
                    this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                    return RedirectToActionPermanent("SolicitudesAsignadas", "Administrador");
                }

                this.ShowMessage(MessageType.Alert, Constante.ErrorRegistro);
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Solicitudes", "Administrador");
        }

        public ActionResult SolicitudHija(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                ViewBag.Id = id;
                ServicioSolicitudes servicioSolicitudes = new ServicioSolicitudes();
                return View(servicioSolicitudes.ObtenerModeloRegistroSolicitudHija(Convert.ToInt32(Criptografia.DesencriptarUrl(id))));
            }

            return RedirectToActionPermanent("Solicitudes", "Administrador");
        }

        [HttpPost]
        public ActionResult SolicitudHija(SolicitudHijaRegistroModel solicitudHijaRegistroModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (solicitudHijaRegistroModel.NuevoDocumento.ImageUpload != null)
                    {
                        solicitudHijaRegistroModel.NuevoDocumento.Archivo = Imagenes.ConvertirImagenAByte(solicitudHijaRegistroModel.NuevoDocumento.ImageUpload);
                        solicitudHijaRegistroModel.NuevoDocumento.Nombre = Path.GetFileName(solicitudHijaRegistroModel.NuevoDocumento.ImageUpload.FileName);
                        solicitudHijaRegistroModel.NuevoDocumento.TipoMime = solicitudHijaRegistroModel.NuevoDocumento.ImageUpload.ContentType;
                        solicitudHijaRegistroModel.NuevoDocumento.Autor = NombreCompleto;
                    }

                    ServicioSolicitudes servicioSolicitudes = new ServicioSolicitudes();
                    if (servicioSolicitudes.RegistrarSolicitudHija(solicitudHijaRegistroModel, UsuarioId))
                    {
                        this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                        return RedirectToActionPermanent("SolicitudAsignada", "Administrador", new { id = Criptografia.EncriptarUrl(solicitudHijaRegistroModel.SolicitudPadreId.ToString()) });
                    }

                    this.ShowMessage(MessageType.Alert, Constante.ErrorRegistro);
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return RedirectToActionPermanent("Solicitudes", "Administrador");
        }

        public ActionResult Solicitud(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    ServicioSolicitudes servicioSolicitudes = new ServicioSolicitudes();
                    return View("Solicitud", servicioSolicitudes.ObtenerSolicitud(Convert.ToInt32(Criptografia.DesencriptarUrl(id)), UsuarioId));
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return RedirectToActionPermanent("Solicitudes", "Administrador");
        }

        #endregion

        #region "Encuestas
        public ActionResult Encuestas(string consulta)
        {
            ViewBag.Consulta = consulta;
            ServicioEncuentas servicioEncuestas = new ServicioEncuentas();
            return View(servicioEncuestas.ObtenerEncuestas(consulta));
        }

        public ActionResult Encuesta(string id)
        {
            try
            {
                ServicioEncuentas servicioEncuestas = new ServicioEncuentas();
                if (!string.IsNullOrEmpty(id))
                    return View(servicioEncuestas.ObtenerEncuesta(Convert.ToInt32(Criptografia.DesencriptarUrl(id))));

                return View(servicioEncuestas.ObtenerModeloRegistroEncuesta());
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Encuestas", "Administrador");
        }

        [HttpPost]
        public ActionResult EncuestasRegistroClasificacion()
        {
            try
            {
                List<int> listaIds = new List<int>();
                List<int> listaOrdenes = new List<int>();

                if (!string.IsNullOrEmpty(Request["SecuenciaEncuestasRegistroClasificacion"]))
                {
                    var temp = Request["SecuenciaEncuestasRegistroClasificacion"].Split(',');
                    listaIds = temp.Select(t => Convert.ToInt32(t)).ToList();

                    foreach (int id in listaIds)
                    {
                        string t = Request[string.Format("orden_{0}", id)];
                        int orden;
                        if (Int32.TryParse(t, out orden))
                            listaOrdenes.Add(orden);
                        else
                            listaOrdenes.Add(-1);
                    }
                }

                ServicioEncuentas servicioEncuestas = new ServicioEncuentas();
                if (servicioEncuestas.RegistrarEncuestasRegistroClasificacion(listaIds, listaOrdenes))
                    this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                else
                    this.ShowMessage(MessageType.Alert, Constante.ErrorRegistro, true);

            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Encuestas", "Administrador");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Encuesta(EncuestaRegistroModel encuestaRegistroModel)
        {
            ServicioEncuentas servicioEncuestas = new ServicioEncuentas();

            if (ModelState.IsValid)
            {
                try
                {
                    string idEncuesta = servicioEncuestas.RegistrarEncuesta(encuestaRegistroModel, UsuarioId);
                    if (!string.IsNullOrEmpty(idEncuesta))
                        return RedirectToActionPermanent("Preguntas", "Administrador", new { id = idEncuesta });

                    this.ShowMessage(MessageType.Alert, Constante.ErrorRegistro, true);
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            int id = encuestaRegistroModel.Encuesta.Id;
            if (id != -1)
            {
                return View(servicioEncuestas.ObtenerEncuesta(id));
            }

            return View(servicioEncuestas.ObtenerModeloRegistroEncuesta());
        }

        public ActionResult Preguntas(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    ServicioEncuentas servicioEncuestas = new ServicioEncuentas();
                    return View(servicioEncuestas.ObtenerModeloRegistroPregunta(id));
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Encuestas", "Administrador");
        }

        [HttpPost]
        public ActionResult Preguntas(PreguntaModel preguntaModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ServicioEncuentas servicioEncuestas = new ServicioEncuentas();
                    if (servicioEncuestas.RegistrarPregunta(preguntaModel))
                        this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                    else
                        this.ShowMessage(MessageType.Alert, Constante.ErrorRegistro, true);

                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }

            }

            if (preguntaModel != null)
                return RedirectToActionPermanent("Preguntas", "Administrador", new { id = Criptografia.EncriptarUrl(preguntaModel.EncuestaId.ToString()) });

            return RedirectToActionPermanent("Preguntas", "Administrador");
        }

        [HttpPost]
        public ActionResult Pregunta(string preguntaId, string tokenEncuesta)
        {
            try
            {
                int id;
                if (Int32.TryParse(preguntaId, out id))
                {
                    ServicioEncuentas servicioEncuestas = new ServicioEncuentas();
                    return PartialView("_PreguntaNueva", servicioEncuestas.ObtenerPregunta(id, tokenEncuesta));
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Encuestas", "Administrador");
        }


        public ActionResult Opciones(string id, string psid)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    ServicioEncuentas servicioEncuestas = new ServicioEncuentas();
                    PreguntaOpcionRegistroModel preguntaOpcionRegistroModel = servicioEncuestas.ObtenerModeloRegistroOpcion(id);

                    int temp;
                    if (preguntaOpcionRegistroModel != null && Int32.TryParse(psid, out temp))
                        preguntaOpcionRegistroModel.PreguntaSeleccionadaId = psid;

                    return View(preguntaOpcionRegistroModel);
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }


            return RedirectToActionPermanent("Encuestas", "Administrador");
        }

        [HttpPost]
        public JsonResult ListaDeOpciones(string preguntaId)
        {
            try
            {
                ServicioEncuentas servicioEncuestas = new ServicioEncuentas();
                return Json(new { items = servicioEncuestas.ObtenerOpciones(Convert.ToInt32(preguntaId)) });
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return Json(null);
        }

        [HttpPost]
        public ActionResult Opcion(string tokenEncuesta, string preguntaId, string opcionId)
        {
            try
            {
                int pId;
                int oId;
                if (Int32.TryParse(preguntaId, out pId) && Int32.TryParse(opcionId, out oId))
                {
                    ServicioEncuentas servicioEncuestas = new ServicioEncuentas();
                    return PartialView("_PreguntaOpcionNueva", servicioEncuestas.ObtenerOpcion(tokenEncuesta, pId, oId));
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Encuestas", "Administrador");
        }

        [HttpPost]
        public ActionResult Opciones(PreguntaOpcionModel preguntaOpcionModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ServicioEncuentas servicioEncuestas = new ServicioEncuentas();
                    if (servicioEncuestas.RegistrarOpcion(preguntaOpcionModel))
                        this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                    else
                        this.ShowMessage(MessageType.Alert, Constante.ErrorRegistro, true);
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            if (preguntaOpcionModel != null)
                return RedirectToActionPermanent("Opciones", "Administrador", new { id = Criptografia.EncriptarUrl(preguntaOpcionModel.EncuestaId.ToString()), psid = preguntaOpcionModel.PreguntaId });

            return RedirectToActionPermanent("Preguntas", "Administrador");
        }


        public ActionResult Filtros(string id, string psid)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    ServicioEncuentas servicioEncuestas = new ServicioEncuentas();
                    PreguntaFiltroRegistroModel preguntaFiltroRegistroModel = servicioEncuestas.ObtenerModeloRegistroFiltro(id);

                    int temp;
                    if (preguntaFiltroRegistroModel != null && Int32.TryParse(psid, out temp))
                        preguntaFiltroRegistroModel.PreguntaSeleccionadaId = psid;

                    return View(preguntaFiltroRegistroModel);
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Encuestas", "Administrador");
        }

        [HttpPost]
        public JsonResult ListaDeFiltros(string tokenEncuesta, string preguntaId, string opcionId)
        {
            try
            {
                int pId;
                int oId;
                if (Int32.TryParse(preguntaId, out pId) && Int32.TryParse(opcionId, out oId))
                {
                    ServicioEncuentas servicioEncuestas = new ServicioEncuentas();
                    return Json(new { items = servicioEncuestas.ObtenerFiltros(tokenEncuesta, pId, oId) });
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return Json(null);
        }

        [HttpPost]
        public ActionResult Filtros(PreguntaFiltroRegistroModel preguntaFiltroRegistroModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ServicioEncuentas servicioEncuestas = new ServicioEncuentas();
                    if (servicioEncuestas.RegistrarFiltro(preguntaFiltroRegistroModel))
                        this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                    else
                        this.ShowMessage(MessageType.Alert, Constante.ErrorRegistro, true);
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            if (preguntaFiltroRegistroModel != null)
                return RedirectToActionPermanent("Filtros", "Administrador", new { id = preguntaFiltroRegistroModel.TokenEncuesta, psid = preguntaFiltroRegistroModel.PreguntaSeleccionadaId });

            return RedirectToActionPermanent("Preguntas", "Administrador");
        }


        public ActionResult EliminarEncuesta(string encuestaId)
        {
            try
            {
                ServicioEncuentas servicioEncuestas = new ServicioEncuentas();
                if (servicioEncuestas.EliminarEncuesta(Convert.ToInt32(encuestaId)))
                    this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                else
                    this.ShowMessage(MessageType.Alert, Constante.ErrorEliminacion, true);
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Encuestas", "Administrador");
        }

        public ActionResult EliminarPregunta(string encuestaId, string preguntaId)
        {
            try
            {
                ServicioEncuentas servicioEncuestas = new ServicioEncuentas();
                if (servicioEncuestas.EliminarPregunta(Convert.ToInt32(preguntaId)))
                    this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                else
                    this.ShowMessage(MessageType.Alert, Constante.ErrorEliminacion, true);

            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Preguntas", "Administrador", new { id = Criptografia.EncriptarUrl(encuestaId.ToString()) });
        }

        public ActionResult EliminarOpcion(string encuestaId, string preguntaId, string opcionId)
        {
            try
            {
                ServicioEncuentas servicioEncuestas = new ServicioEncuentas();
                if (servicioEncuestas.EliminarOpcion(Convert.ToInt32(preguntaId), Convert.ToInt32(opcionId)))
                    this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                else
                    this.ShowMessage(MessageType.Alert, Constante.ErrorEliminacion, true);
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Opciones", "Administrador", new { id = Criptografia.EncriptarUrl(encuestaId.ToString()), psid = preguntaId });
        }

        public ActionResult DuplicarEncuesta(string encuestaFuenteId)
        {
            try
            {
                if (!string.IsNullOrEmpty(encuestaFuenteId))
                {
                    ServicioEncuentas servicioEncuestas = new ServicioEncuentas();
                    servicioEncuestas.DuplicarEncuesta(Convert.ToInt32(encuestaFuenteId));
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Encuestas", "Administrador");
        }

        #endregion

        #region "Configuracion"

        public ActionResult Configuracion(string variable, string campoActualizar = "")
        {
            try
            {
                if (campoActualizar != "")
                    ActualizarCampos(campoActualizar);

                ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
                return View(servicioConfiguracion.ObtenerModeloConfiguracion(variable));
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Index", "Administrador");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Configuracion(ConfiguracionModel configuracionModel)
        {
            try
            {
                ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
                servicioConfiguracion.ActivarPublicacionindicadores(configuracionModel);

                return View(servicioConfiguracion.ObtenerModeloConfiguracion(null));
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Index", "Administrador");
        }

        public void ActualizarCampos(string campoActualizar)
        {
            ConfiguracionModel configuracionModel = new ConfiguracionModel();
            configuracionModel.Actualizar = campoActualizar;

            ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
            if (servicioConfiguracion.ActivarPublicacionindicadores(configuracionModel))
                this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
            else
                this.ShowMessage(MessageType.Alert, Constante.ErrorEliminacion, true);
        }


        [HttpPost]
        public ActionResult CategoriaServicio(string id)
        {
            try
            {
                ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
                return PartialView("_ConfiguracionFormularioCategoriasServicios", servicioConfiguracion.ObtenerModeloCategoriaServicio(id));
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Configuracion", "Administrador");
        }

        [HttpPost]
        public ActionResult CategoriasServicios(ConfiguracionCategoriaServicioModel categoriaServicioModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
                    if (servicioConfiguracion.RegistrarCategoriaServicio(categoriaServicioModel))
                        this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                    else
                        this.ShowMessage(MessageType.Alert, Constante.ErrorEliminacion, true);
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return RedirectToActionPermanent("Configuracion", "Administrador", new { variable = "CategoriasServicios" });
        }

        public ActionResult EliminarCategoriaServicio(string id)
        {
            try
            {
                ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
                if (servicioConfiguracion.EliminarCategoriaServicio(id))
                    this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                else
                    this.ShowMessage(MessageType.Alert, Constante.ErrorEliminacion, true);

            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Configuracion", "Administrador", new { variable = "CategoriasServicios" });
        }

        [HttpPost]
        public ActionResult CategoriaSolicitud(int id)
        {
            try
            {
                ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
                return PartialView("_ConfiguracionFormularioCategoriaSolicitud", servicioConfiguracion.ObtenerModeloCategoriaSolicitud(id));
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Configuracion", "Administrador");
        }

        [HttpPost]
        public ActionResult CategoriasSolicitudes(ConfiguracionCategoriaSolicitudModel categoriaSolicitudModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
                    if (servicioConfiguracion.RegistrarCategoriaSolicitud(categoriaSolicitudModel))
                        this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                    else
                        this.ShowMessage(MessageType.Alert, Constante.ErrorEliminacion, true);
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return RedirectToActionPermanent("Configuracion", "Administrador", new { variable = "CategoriasSolicitudes" });
        }

        public ActionResult EliminarCategoriaSolicitud(string id)
        {
            try
            {
                ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
                if (servicioConfiguracion.EliminarCategoriaSolicitud(Convert.ToInt32(id)))
                    this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                else
                    this.ShowMessage(MessageType.Alert, Constante.ErrorEliminacion, true);

            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Configuracion", "Administrador", new { variable = "CategoriasSolicitudes" });
        }


        [HttpPost]
        public ActionResult TipoDeDocumento(string id)
        {
            try
            {
                ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
                return PartialView("_ConfiguracionFormularioTipoDeDocumento", servicioConfiguracion.ObtenerModeloTipoDeDocumento(id));
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Configuracion", "Administrador");
        }


        [HttpPost]
        public ActionResult TiposDeDocumentos(ConfiguracionTipoDocumentoModel tipoDocumentoModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
                    if (servicioConfiguracion.RegistrarTipoDeDocumento(tipoDocumentoModel))
                        this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                    else
                        this.ShowMessage(MessageType.Alert, Constante.ErrorEliminacion, true);
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return RedirectToActionPermanent("Configuracion", "Administrador", new { variable = "TiposDeDocumentos" });
        }

        public ActionResult EliminarTipoDeDocumento(string id)
        {
            try
            {
                ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
                if (servicioConfiguracion.EliminarTipoDeDocumento(id))
                    this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                else
                    this.ShowMessage(MessageType.Alert, Constante.ErrorEliminacion, true);

            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Configuracion", "Administrador", new { variable = "TiposDeDocumentos" });
        }


        [HttpPost]
        public ActionResult TipoDeRestauracion(string id)
        {
            try
            {
                ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
                return PartialView("_ConfiguracionFormularioTipoDeRestauracion", servicioConfiguracion.ObtenerModeloTipoDeRestauracion(id));
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Configuracion", "Administrador");
        }

        [HttpPost]
        public ActionResult TiposDeRestauraciones(ConfiguracionTipoRestauracionModel tipoRestauracionModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
                    if (servicioConfiguracion.RegistrarTipoDeRestauracion(tipoRestauracionModel))
                        this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                    else
                        this.ShowMessage(MessageType.Alert, Constante.ErrorEliminacion, true);
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return RedirectToActionPermanent("Configuracion", "Administrador", new { variable = "TiposDeRestauraciones" });
        }

        public ActionResult EliminarTipoDeRestauracion(string id)
        {
            try
            {
                ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
                if (servicioConfiguracion.EliminarTipoDeRestauracion(id))
                    this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                else
                    this.ShowMessage(MessageType.Alert, Constante.ErrorEliminacion, true);

            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Configuracion", "Administrador", new { variable = "TiposDeRestauraciones" });
        }


        [HttpPost]
        public ActionResult RedSocial(string id)
        {
            try
            {
                ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
                return PartialView("_ConfiguracionFormularioRedSocial", servicioConfiguracion.ObtenerModeloRedSocial(id));
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }


            return RedirectToActionPermanent("Configuracion", "Administrador");
        }

        [HttpPost]
        public ActionResult RedesSociales(ConfiguracionRedSocialModel redSocialModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
                    if (servicioConfiguracion.RegistrarRedSocial(redSocialModel))
                        this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                    else
                        this.ShowMessage(MessageType.Alert, Constante.ErrorEliminacion, true);
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return RedirectToActionPermanent("Configuracion", "Administrador", new { variable = "RedesSociales" });
        }

        public ActionResult EliminarRedSocial(string id)
        {
            try
            {
                ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
                if (servicioConfiguracion.EliminarRedSocial(id))
                    this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                else
                    this.ShowMessage(MessageType.Alert, Constante.ErrorEliminacion, true);

            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Configuracion", "Administrador", new { variable = "RedesSociales" });
        }


        [HttpPost]
        public ActionResult ZonaGeografica(string id)
        {
            try
            {
                ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
                return PartialView("_ConfiguracionFormularioZonaGeografica", servicioConfiguracion.ObtenerModeloZonaGeografica(id));
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }


            return RedirectToActionPermanent("Configuracion", "Administrador");
        }

        [HttpPost]
        public ActionResult ZonasGeograficas(ConfiguracionZonaGeograficaModel zonaGeograficaModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
                    if (servicioConfiguracion.RegistrarZonaGeografica(zonaGeograficaModel))
                        this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                    else
                        this.ShowMessage(MessageType.Alert, Constante.ErrorEliminacion, true);
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return RedirectToActionPermanent("Configuracion", "Administrador", new { variable = "ZonasGeograficas" });
        }

        public ActionResult EliminarZonaGeografica(string id)
        {
            try
            {
                ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
                if (servicioConfiguracion.EliminarZonaGeografica(id))
                    this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                else
                    this.ShowMessage(MessageType.Alert, Constante.ErrorEliminacion, true);

            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Configuracion", "Administrador", new { variable = "ZonasGeograficas" });
        }

        [HttpPost]
        public ActionResult CorreoElectronico(string id)
        {
            try
            {
                ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
                return PartialView("_ConfiguracionFormularioCorreoElectronico", servicioConfiguracion.ObtenerModeloCorreoElectronico(id));
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Configuracion", "Administrador");
        }

        [HttpPost]
        public ActionResult CorreosElectronicos(ConfiguracionCorreoElectronicoModel correoElectronicoModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
                    if (servicioConfiguracion.RegistrarCorreoElectronico(correoElectronicoModel))
                        this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                    else
                        this.ShowMessage(MessageType.Alert, Constante.ErrorEliminacion, true);
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return RedirectToActionPermanent("Configuracion", "Administrador", new { variable = "CorreosElectronicos" });
        }

        #endregion

        #region "Usuarios"

        public ActionResult Usuarios(string palabrasClaves, List<string> perfilesUsuarios, List<string> tiposPersonas, List<string> estados, List<string> estadosEncuestas)
        {
            try
            {
                ViewBag.PalabrasClaves = palabrasClaves;

                if (perfilesUsuarios == null)
                    perfilesUsuarios = new List<string>();

                if (tiposPersonas == null)
                    tiposPersonas = new List<string>();

                if (estados == null)
                    estados = new List<string>();

                if (estadosEncuestas == null)
                    estadosEncuestas = new List<string>();

                // Tipos usuarios

                var listaPerfilesUsuarios = (from Perfiles perfil in Enum.GetValues(typeof(Perfiles))
                                             select new SelectListItem
                                             {
                                                 Selected = perfilesUsuarios.Contains(perfil.ToString()),
                                                 Text = Enumeraciones.GetEnumDescription(perfil),
                                                 Value = perfil.ToString()
                                             }).ToList();

                ViewBag.PerfilesUsuarios = listaPerfilesUsuarios;

                // Tipos de personas

                var listaTiposPersonas = (from TiposPersona tipoPersona in Enum.GetValues(typeof(TiposPersona))
                                          select new SelectListItem
                                          {
                                              Selected = tiposPersonas.Contains(tipoPersona.ToString()),
                                              Text = Enumeraciones.GetEnumDescription(tipoPersona),
                                              Value = tipoPersona.ToString()
                                          }).ToList();

                ViewBag.TiposPersonas = listaTiposPersonas;

                // Estados

                var listaEstados = (from UsuarioEstados estado in Enum.GetValues(typeof(UsuarioEstados))
                                    select new SelectListItem
                                    {
                                        Selected = estados.Contains(estado.ToString()),
                                        Text = Enumeraciones.GetEnumDescription(estado),
                                        Value = estado.ToString()
                                    }).ToList();

                ViewBag.Estados = listaEstados;

                var listaEstadosEncuestas = (from EstadosRespuestas estadoEncuesta in Enum.GetValues(typeof(EstadosRespuestas))
                                             select new SelectListItem
                                             {
                                                 Selected = estadosEncuestas.Contains(estadoEncuesta.ToString()),
                                                 Text = Enumeraciones.GetEnumDescription(estadoEncuesta),
                                                 Value = estadoEncuesta.ToString()
                                             }).ToList();

                ViewBag.EstadosEncuestas = listaEstadosEncuestas;

                ServicioUsuarios servicioUsuarios = new ServicioUsuarios();
                return View(servicioUsuarios.ObtenerUsuarios(UsuarioId, palabrasClaves, perfilesUsuarios, tiposPersonas, estados));

            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return View();
        }

        public ActionResult Usuario(string id, List<string> estadosEncuestas)
        {
            try
            {

                if (!string.IsNullOrEmpty(id))
                {

                    UsuarioPersonaModel usuarioPersonaActualizarUsuarioModel = new UsuarioPersonaModel();

                    ServicioUsuarios servicioUsuarios = new ServicioUsuarios();
                    usuarioPersonaActualizarUsuarioModel = servicioUsuarios.ObtenerModeloUsuarioPersona(Convert.ToInt32(id));

                    if (estadosEncuestas == null)
                        estadosEncuestas = new List<string>();

                    if (usuarioPersonaActualizarUsuarioModel != null)
                    {
                        ServicioEncuentas servicioEncuentas = new ServicioEncuentas();
                        usuarioPersonaActualizarUsuarioModel.Encuestas = servicioEncuentas.ObtenerEncuentasRespondidas(Convert.ToInt32(id));

                        usuarioPersonaActualizarUsuarioModel.EstadosEncuestas = (from EstadosRespuestas estadoEncuesta in Enum.GetValues(typeof(EstadosRespuestas))
                                                                                 select estadoEncuesta.ToString()).ToList();

                        ViewBag.EstadosEncuestas = usuarioPersonaActualizarUsuarioModel.EstadosEncuestas;

                    }

                    ViewBag.Perfil = Perfil;
                    ViewBag.PerfilActual = Perfil.ToString();
                    ViewBag.ExtensionesImagen = Utilities.Documents.ObtenerExtensionesImagen("TiposArchivo");
                    ViewBag.ExtensionesDocumento = Utilities.Documents.ObtenerExtensionesDocumento("TiposArchivo");


                    return View(usuarioPersonaActualizarUsuarioModel);
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Usuarios", "Administrador");
        }

        [HttpPost]
        public ActionResult Usuario(UsuarioPersonaModel usuarioPersonaActualizarUsuarioModel)
        {
            ModelState.Remove("Usuario.Perfil.Estado");
            ModelState.Remove("Usuario.Perfil.Nombre");
            ModelState.Remove("Usuario.Contrasena");

            ModelState.Remove("PersonaJuridica.PersonaJuridica.CorreoElectronico");
            ModelState.Remove("PersonaJuridica.PersonaJuridica.Contrasena");
            ModelState.Remove("PersonaJuridica.PersonaJuridica.ConfirmarContrasena");
            ModelState.Remove("PersonaJuridica.PersonaJuridica.NombreDeLaInstitucion");
            ModelState.Remove("PersonaJuridica.PersonaJuridica.NombreContacto");
            ModelState.Remove("PersonaJuridica.PersonaJuridica.DescripcionInstitucion");
            ModelState.Remove("PersonaJuridica.PersonaJuridica.ResenaHistorica");
            ModelState.Remove("PersonaJuridica.PersonaJuridica.Categoria");
            ModelState.Remove("PersonaJuridica.PersonaJuridica.Nombre");
            ModelState.Remove("PersonaJuridica.PersonaJuridica.Nit");
            ModelState.Remove("PersonaJuridica.PersonaJuridica.CodigoDepartamento");
            ModelState.Remove("PersonaJuridica.PersonaJuridica.CodigoMunicipio");
            ModelState.Remove("PersonaJuridica.PersonaJuridica.Resena");

            ModelState.Remove("PersonaNatural.PersonaNatural.CorreoElectronico");
            ModelState.Remove("PersonaNatural.PersonaNatural.Contrasena");
            ModelState.Remove("PersonaNatural.PersonaNatural.ConfirmarContrasena");
            ModelState.Remove("PersonaNatural.PersonaNatural.SubTipo");

            ServicioUsuarios servicioUsuarios = new ServicioUsuarios();
            string mensajeResultadoValidacion = string.Empty;
            Dictionary<string, string> modelError = new Dictionary<string, string>();

            if (ModelState.IsValid)
            {
                try
                {
                    if (usuarioPersonaActualizarUsuarioModel.PersonaJuridica != null)
                    {
                        if (ValidarDocumentosEntidadMuseal(usuarioPersonaActualizarUsuarioModel.PersonaJuridica.PersonaJuridica, out mensajeResultadoValidacion, out modelError))
                        {
                            if (servicioUsuarios.ActualizarUsuarioPersona(usuarioPersonaActualizarUsuarioModel))
                            {
                                ServicioEncuentas servicioEncuentas = new ServicioEncuentas();
                                if (servicioEncuentas.ActualizarRespuestas(usuarioPersonaActualizarUsuarioModel.EncuestaId, usuarioPersonaActualizarUsuarioModel.Usuario.Id, usuarioPersonaActualizarUsuarioModel.EstadoEncuesta))
                                {
                                    this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                                    return RedirectToActionPermanent("Usuarios", "Administrador");
                                }
                            }

                            this.ShowMessage(MessageType.Alert, Constante.ErrorRegistro);
                        }
                        else
                        {
                            foreach (var item in modelError)
                                ModelState.AddModelError(item.Key, item.Value);

                            this.ShowMessage(MessageType.Error, mensajeResultadoValidacion);
                        }
                    }
                    else
                    {
                        if (servicioUsuarios.ActualizarUsuarioPersona(usuarioPersonaActualizarUsuarioModel))
                        {
                            ServicioEncuentas servicioEncuentas = new ServicioEncuentas();
                            if (servicioEncuentas.ActualizarRespuestas(usuarioPersonaActualizarUsuarioModel.EncuestaId, usuarioPersonaActualizarUsuarioModel.Usuario.Id, usuarioPersonaActualizarUsuarioModel.EstadoEncuesta))
                            {
                                this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                                return RedirectToActionPermanent("Usuarios", "Administrador");
                            }
                        }

                        this.ShowMessage(MessageType.Alert, Constante.ErrorRegistro);
                    }
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return this.ObtenerView(usuarioPersonaActualizarUsuarioModel);
        }

        private ActionResult ObtenerView(UsuarioPersonaModel usuarioPersonaActualizarUsuarioModel)
        {
            List<string> estadosEncuestas = new List<string>();
            ServicioUsuarios servicioUsuarios = new ServicioUsuarios();

            usuarioPersonaActualizarUsuarioModel = servicioUsuarios.ObtenerModeloUsuarioPersona(usuarioPersonaActualizarUsuarioModel.Usuario.Id);
            if (usuarioPersonaActualizarUsuarioModel != null)
            {
                ServicioEncuentas servicioEncuentas = new ServicioEncuentas();
                usuarioPersonaActualizarUsuarioModel.Encuestas = servicioEncuentas.ObtenerEncuentasRespondidas(usuarioPersonaActualizarUsuarioModel.Usuario.Id);

                usuarioPersonaActualizarUsuarioModel.EstadosEncuestas = (from EstadosRespuestas estadoEncuesta in Enum.GetValues(typeof(EstadosRespuestas))
                                                                         select estadoEncuesta.ToString()).ToList();

                ViewBag.EstadosEncuestas = usuarioPersonaActualizarUsuarioModel.EstadosEncuestas;
                ViewBag.ExtensionesImagen = Utilities.Documents.ObtenerExtensionesImagen("TiposArchivo");
                ViewBag.ExtensionesDocumento = Utilities.Documents.ObtenerExtensionesDocumento("TiposArchivo");
            }

            return View(usuarioPersonaActualizarUsuarioModel);
        }

        public static bool ValidarDocumentosEntidadMuseal(PersonaJuridicaModel personaJuridicaModel, out string mensajeResultadoValidacion, out Dictionary<string, string> modelError)
        {
            List<EtiquetaDominioModel> listaEtiquetaDominioModel = Utilities.Documents.ObtenerConfiguracionEtiquetasDominio("ConfiguracionCargaArchivoImagen");
            List<EtiquetaDominioModel> listaEtiquetaDominioModelDcto = Utilities.Documents.ObtenerConfiguracionEtiquetasDominio("ConfiguracionCargaArchivoDocumentos");
            modelError = new Dictionary<string, string>();

            if (!Utilities.Documents.ValidarDocumentoEntidadMuseal(personaJuridicaModel.DocumentoFotoFachada.ImageUpload, listaEtiquetaDominioModel, out mensajeResultadoValidacion))
            {
                modelError.Add("PersonaJuridica.PersonaJuridica.DocumentoFotoFachada.ImageUpload", mensajeResultadoValidacion);
            }
            if (!Utilities.Documents.ValidarDocumentoEntidadMuseal(personaJuridicaModel.DocumentoFotoInteraccion.ImageUpload, listaEtiquetaDominioModel, out mensajeResultadoValidacion))
            {
                modelError.Add("PersonaJuridica.PersonaJuridica.DocumentoFotoInteraccion.ImageUpload", mensajeResultadoValidacion);
            }
            if (!Utilities.Documents.ValidarDocumentoEntidadMuseal(personaJuridicaModel.DocumentoFotoColeccion1.ImageUpload, listaEtiquetaDominioModel, out mensajeResultadoValidacion))
            {
                modelError.Add("PersonaJuridica.PersonaJuridica.DocumentoFotoColeccion1.ImageUpload", mensajeResultadoValidacion);
            }
            if (!Utilities.Documents.ValidarDocumentoEntidadMuseal(personaJuridicaModel.DocumentoFotoColeccion2.ImageUpload, listaEtiquetaDominioModel, out mensajeResultadoValidacion))
            {
                modelError.Add("PersonaJuridica.PersonaJuridica.DocumentoFotoColeccion2.ImageUpload", mensajeResultadoValidacion);
            }
            if (!Utilities.Documents.ValidarDocumentoEntidadMuseal(personaJuridicaModel.DocumentoFotoColeccion3.ImageUpload, listaEtiquetaDominioModel, out mensajeResultadoValidacion))
            {
                modelError.Add("PersonaJuridica.PersonaJuridica.DocumentoFotoColeccion3.ImageUpload", mensajeResultadoValidacion);
            }
            if (!Utilities.Documents.ValidarDocumentoEntidadMuseal(personaJuridicaModel.DocumentoLegalExistencia.ImageUpload, listaEtiquetaDominioModelDcto, out mensajeResultadoValidacion))
            {
                modelError.Add("PersonaJuridica.PersonaJuridica.DocumentoLegalExistencia.ImageUpload", mensajeResultadoValidacion);
            }
            if (!Utilities.Documents.ValidarDocumentoEntidadMuseal(personaJuridicaModel.DocumentoPoliticaPlanColecciones.ImageUpload, listaEtiquetaDominioModelDcto, out mensajeResultadoValidacion))
            {
                modelError.Add("PersonaJuridica.PersonaJuridica.DocumentoPoliticaPlanColecciones.ImageUpload", mensajeResultadoValidacion);
            }

            if (modelError.Count > 0)
                mensajeResultadoValidacion = modelError.First().Value;

            return modelError.Count > 0 ? false : true;
        }

        public ActionResult NuevoUsuarioPersonaNatural()
        {

            ServicioPersonasNaturales servicioPersonasNatural = new ServicioPersonasNaturales();
            return View(servicioPersonasNatural.ObtenerPersonaNaturalRegistroModel());
        }

        [HttpPost]
        public ActionResult NuevoUsuarioPersonaNatural(PersonaNaturalRegistroModel personaNaturalRegistroModel)
        {
            ModelState.Remove("PersonaNatural.SubTipo");

            ModelState.Remove("Usuario.CorreoElectronico");
            ModelState.Remove("Usuario.Contrasena");
            ModelState.Remove("Usuario.Estado");
            ModelState.Remove("Usuario.Perfil.Nombre");
            ModelState.Remove("Usuario.Perfil.Estado");

            if (ModelState.IsValid)
            {
                try
                {
                    ServicioPersonasNaturales servicioPersonasNatural = new ServicioPersonasNaturales();
                    if (servicioPersonasNatural.RegistrarUsuarioPersonaNatural(personaNaturalRegistroModel.PersonaNatural))
                        this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                    else
                        this.ShowMessage(MessageType.Alert, Constante.ErrorRegistro);
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return NuevoUsuarioPersonaNatural();
        }

        public ActionResult NuevoUsuarioPersonaJuridica()
        {

            ServicioPersonasJuridicas servicioPersonasJuridicas = new ServicioPersonasJuridicas();
            return View(servicioPersonasJuridicas.ObtenerPersonaJuridicaRegistroModel());
        }

        [HttpPost]
        public ActionResult NuevoUsuarioPersonaJuridica(PersonaJuridicaRegistroModel personaJuridicaRegistroModel)
        {
            ModelState.Remove("PersonaJuridica.SubTipo");
            ModelState.Remove("Usuario.CorreoElectronico");
            ModelState.Remove("Usuario.Contrasena");
            ModelState.Remove("Usuario.Estado");
            ModelState.Remove("Usuario.Perfil.Nombre");
            ModelState.Remove("Usuario.Perfil.Estado");

            if (ModelState.IsValid)
            {
                try
                {
                    ServicioPersonasJuridicas servicioPersonasJuridicas = new ServicioPersonasJuridicas();
                    if (servicioPersonasJuridicas.RegistrarUsuarioPersonaJuridica(personaJuridicaRegistroModel.PersonaJuridica, false))
                        this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                    else
                        this.ShowMessage(MessageType.Alert, Constante.ErrorRegistro);
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return NuevoUsuarioPersonaJuridica();
        }

        public ActionResult EntidadesMuseales(string palabrasClaves, List<string> estados)
        {
            ViewBag.PalabrasClaves = palabrasClaves;

            if (estados == null)
                estados = new List<string>();

            // Estados
            var listaEstados = (from UsuarioEstados estado in Enum.GetValues(typeof(UsuarioEstados))
                                select new SelectListItem
                                {
                                    Selected = estados.Contains(estado.ToString()),
                                    Text = Enumeraciones.GetEnumDescription(estado),
                                    Value = estado.ToString()
                                }).ToList();

            ViewBag.Estados = listaEstados;

            ServicioEntidadesMuseales servicioEntidadesMuseales = new ServicioEntidadesMuseales();
            return View(servicioEntidadesMuseales.ObtenerUsuariosEntidadesMuseales(UsuarioId, palabrasClaves, estados));
        }

        public ActionResult EntidadMuseal(string personaJuridicaId)
        {
            if (!string.IsNullOrEmpty(personaJuridicaId))
            {
                try
                {
                    ServicioEntidadesMuseales servicioEntidades = new ServicioEntidadesMuseales();
                    return View(servicioEntidades.ObtenerModeloRegistroEntidadMuseal(int.Parse(personaJuridicaId)));
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return RedirectToActionPermanent("EntidadesMuseales", "Administrador");
        }

        [HttpPost]
        public ActionResult EntidadMuseal(EntidadMusealRegistroModel entidadMusealRegistroModel)
        {
            ModelState.Remove("EntidadMuseal.CorreoElectronico");
            ModelState.Remove("EntidadMuseal.Contrasena");
            ModelState.Remove("EntidadMuseal.ConfirmarContrasena");

            if (ModelState.IsValid)
            {
                try
                {
                    ServicioEntidadesMuseales servicioEntidades = new ServicioEntidadesMuseales();
                    if (servicioEntidades.RegistrarEntidadMuseal(entidadMusealRegistroModel))
                        this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                    else
                        this.ShowMessage(MessageType.Alert, Constante.ErrorRegistro, true);
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return RedirectToActionPermanent("EntidadesMuseales", "Administrador");
        }

        #endregion

        #region "Grupos"
        public ActionResult Grupos()
        {
            ServicioGrupos servicioGrupos = new ServicioGrupos();
            return View(servicioGrupos.ObtenerGruposDeUsuarios());
        }

        [AllowAnonymous]
        public ActionResult Grupo(string id)
        {
            ServicioGrupos servicioGrupos = new ServicioGrupos();
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    int grupoId = Convert.ToInt32(Criptografia.DesencriptarUrl(id));
                    return View(servicioGrupos.ObtenerRegistroNuevoGrupoDeUsuariosModel(grupoId));
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }
            else
                return View(servicioGrupos.ObtenerRegistroNuevoGrupoDeUsuariosModel());

            return RedirectToActionPermanent("Grupos", "Administrador");
        }

        [HttpPost]
        public ActionResult Grupo(GrupoDeUsuariosRegistroModel grupoDeUsuariosRegistroModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ServicioGrupos servicioGrupos = new ServicioGrupos();
                    if (servicioGrupos.ValidarRegistrarGrupo(grupoDeUsuariosRegistroModel))
                    {
                        if (servicioGrupos.RegistrarGrupo(grupoDeUsuariosRegistroModel))
                            this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                        else
                            this.ShowMessage(MessageType.Alert, Constante.ErrorRegistro, true);
                    }
                    else
                        this.ShowMessage(MessageType.Alert, Constante.ErrorRegistroGrupo, true);

                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return RedirectToActionPermanent("Grupos", "Administrador");
        }

        #endregion
    }
}
