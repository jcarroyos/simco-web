using Simco.ApplicationLayer.Servicios;
using Simco.DomainModelLayer.ViewModels;
using Simco.Infrastructure.Notification;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Simco.Controllers
{
    [Authorize]
    public class ReportesController : BaseController
    {
        public ActionResult Index()
        {
            try
            {
                ServicioReportes servicioReportes = new ServicioReportes();
                return View(servicioReportes.ObtenerIndicadores());
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Index", "Administrador");
        }

        public List<EtiquetaDominioModel> ObtenerEncuentasPorUsuarioID(int usuarioId)
        {
            try
            {
                ServicioEncuentas servicioEncuentas = new ServicioEncuentas();
                return servicioEncuentas.ObtenerEncuentasRespondidas(usuarioId);
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return new List<EtiquetaDominioModel>();
        }

        public JsonResult ListaEncuestasRespondidas(string usuarioId)
        {
            try
            {
                if (!string.IsNullOrEmpty(usuarioId))
                {
                    ServicioEncuentas servicioEncuentas = new ServicioEncuentas();
                    return Json(servicioEncuentas.ObtenerEncuentasRespondidas(Convert.ToInt32(usuarioId)), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return Json(null);
        }

        public JsonResult ListaRespuestas(string usuarioId, string encuestaId)
        {
            try
            {
                if (!string.IsNullOrEmpty(usuarioId) && !string.IsNullOrEmpty(encuestaId))
                {
                    ServicioEncuentas servicioEncuentas = new ServicioEncuentas();
                    return Json(servicioEncuentas.ObtenerRespuestas(Convert.ToInt32(usuarioId), Convert.ToInt32(encuestaId)), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return Json(null);
        }

        public FileContentResult FichaCompleta(string usuarioId)
        {
            try
            {
                int id;
                if (Int32.TryParse(usuarioId, out id))
                {
                    ServicioPersonasNaturales servicioPersonaNatural = new ServicioPersonasNaturales();
                    ServicioPersonasJuridicas servicioPersonaJuridica = new ServicioPersonasJuridicas();

                    int personaId = 0;
                    bool esPersonaNatural = servicioPersonaNatural.EsPersonaNatural(id, ref personaId);

                    string nombreArchivo;
                    byte[] archivo = esPersonaNatural ?
                        servicioPersonaNatural.FichaCompletaPersonaNatural(personaId, out nombreArchivo) :
                        servicioPersonaJuridica.FichaCompletaPersonaJuridica(id, personaId, out nombreArchivo);

                    HttpContext.Response.AddHeader("content-disposition", "attachment; filename=" + nombreArchivo);
                    return File(archivo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return null;
        }

        public FileContentResult EncuestaResultadosMatrizPorUsuario(string respuestaId)
        {
            try
            {
                if (!string.IsNullOrEmpty(respuestaId))
                {
                    string nombreArchivo;
                    ServicioReportes servicioReportes = new ServicioReportes();
                    byte[] archivo = servicioReportes.EncuestaResultadosMatrizPorUsuario(respuestaId, out nombreArchivo);

                    HttpContext.Response.AddHeader("content-disposition", "attachment; filename=" + nombreArchivo);
                    return File(archivo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return null;
        }
    }
}