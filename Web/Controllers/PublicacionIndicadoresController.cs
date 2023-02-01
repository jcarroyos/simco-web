using Simco.ApplicationLayer.Servicios;
using Simco.CrossInfraestructureLayer;
using Simco.DomainModelLayer.ViewModels;
using Simco.Infrastructure.Notification;
using System;
using System.Web.Mvc;

namespace Simco.Controllers
{
    [Authorize]
    public class PublicacionIndicadoresController : BaseController
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            try
            {
                ServicioIndicadores servicioIndicadores = new ServicioIndicadores();
                InicioSesionModel inicioSesionModel = ObtenerSesionModel();

                if (ViewBag.CargarPublicacionIndicador)
                    inicioSesionModel.Indicadores = servicioIndicadores.CargarPublicacion(0);

                if (inicioSesionModel.Indicadores == null)
                    inicioSesionModel.Indicadores = new PublicacionIndicadoresModel();

                return View(inicioSesionModel);

            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Index");
        }

        public ActionResult AdminIndicadores()
        {
            try
            {
                ServicioIndicadores servicioIndicadores = new ServicioIndicadores();
                return View(servicioIndicadores.CargarPublicacion(0));
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Index");
        }

        [HttpPost]
        public ActionResult AdminIndicadores(PublicacionIndicadoresModel publicacionIndicadoresModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ServicioIndicadores servicioIndicadores = new ServicioIndicadores();

                    if (servicioIndicadores.PublicarIndicador(publicacionIndicadoresModel))
                        this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                    else
                        this.ShowMessage(MessageType.Alert, Constante.ErrorRegistro);
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("AdminIndicadores");
        }

        public ActionResult CargarIndicador(string preguntaId, string tipoTorta, string tipoPorcentaje, string titulo)
        {
            ServicioIndicadores servicioIndicadores = new ServicioIndicadores();

            return Json(servicioIndicadores.CargarIndiciadores(false, preguntaId, tipoTorta, tipoPorcentaje, titulo, string.Empty, string.Empty, string.Empty, string.Empty), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CargarIndicadorPublico(string preguntaId, string tipoTorta, string tipoPorcentaje, string titulo, string coleccionId, string claseEntidadId, string codigoDepartamento, string codigoMunicipio)
        {
            ServicioIndicadores servicioIndicadores = new ServicioIndicadores();

            return Json(servicioIndicadores.CargarIndiciadores(true, preguntaId, tipoTorta, tipoPorcentaje, titulo, coleccionId, claseEntidadId, codigoDepartamento, codigoMunicipio), JsonRequestBehavior.AllowGet);
        }
    }
}