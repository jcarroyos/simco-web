using log4net;
using Simco.ApplicationLayer.Servicios;
using Simco.CrossInfraestructureLayer;
using Simco.DomainModelLayer.ViewModels;
using Simco.Infrastructure.Notification;
using Simco.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;


namespace Simco.Controllers
{
    public class TiposController : BaseController
    {
        private static readonly ILog log = SimcoExcepcion.GetLogger(typeof(TiposController));

        public ActionResult Index()
        {
            return View();
        }

        // Tipos de Servicios
        public ActionResult ConsultarTiposServicios([DataSourceRequest] DataSourceRequest request, string consulta)
        {
            ViewBag.Consulta = consulta != null ? consulta : string.Empty;
            ServicioTipos servicioTipos = new ServicioTipos();
            List<TiposModel> lista = servicioTipos.CargarTiposServicios(consulta);
            ViewData["TiposServicios"] = lista;
            return View(lista);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CrearActualizarTiposServicios([DataSourceRequest] DataSourceRequest request, TiposModel tipo)
        {
            if (tipo != null && ModelState.IsValid)
            {
                try
                {
                    ServicioTipos servicioTipos = new ServicioTipos();
                    if (servicioTipos.CrearActualizarTiposServicios(tipo))
                    {
                        this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                        return RedirectToActionPermanent("ConsultarTiposServicios", "Tipos");
                    }
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, false);
                    ModelState.AddModelError("FuncionalidadYaRegistrada", ex.Message);
                }
            }

            return Json(new[] { tipo }.ToDataSourceResult(request, ModelState));
        }


        // Tipos de Entidades Museales
        public ActionResult ConsultarTiposEntidad([DataSourceRequest] DataSourceRequest request, string consulta)
        {
            ViewBag.Consulta = consulta != null ? consulta : string.Empty;
            ServicioTipos servicioTipos = new ServicioTipos();
            List<TiposModel> lista = servicioTipos.CargarTiposEntidadesMuseales(consulta);
            ViewData["TiposEntidades"] = lista;
            return View(lista);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CrearActualizarTiposEntidades([DataSourceRequest] DataSourceRequest request, TiposModel tipo)
        {
            if (tipo != null && ModelState.IsValid)
            {
                try
                {
                    ServicioTipos servicioTipos = new ServicioTipos();
                    if (servicioTipos.CrearActualizarTiposEntidades(tipo))
                    {
                        this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios, true);
                    }

                    return RedirectToActionPermanent("ConsultarTiposEntidad", "Tipos");
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, false);
                    ModelState.AddModelError("FuncionalidadYaRegistrada", ex.Message);
                }
            }

            return Json(new[] { tipo }.ToDataSourceResult(request, ModelState));
        }

    }
}