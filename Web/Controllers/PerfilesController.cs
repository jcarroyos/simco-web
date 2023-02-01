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
    public class PerfilesController : BaseController
    {
        #region Administración de Perfiles
        public ActionResult Index(string consulta)
        {
            ConsultarPerfiles(consulta);
            return View();
        }

        public ActionResult ConsultarPerfiles(string consulta)
        {
            try
            {
                ViewBag.Consulta = consulta != null ? consulta : string.Empty;
                ServicioPerfiles servicioPerfiles = new ServicioPerfiles();
                List<PerfilModel> perfiles = servicioPerfiles.CargarPerfilesModel(consulta);
                ViewBag.Perfiles = perfiles;
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            //TODO: Retornar View Model
            return View();
        }

        public ActionResult AdminPerfil(int perfilId)
        {
            PerfilModel perfilModel = new PerfilModel();
            try
            {
                ServicioPerfiles servicioPerfiles = new ServicioPerfiles();
                perfilModel = servicioPerfiles.CargarPerfilesModel(perfilId);
                perfilModel = (perfilModel == null ? new PerfilModel() : perfilModel);
                Session["idPerfil"] = perfilId;
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            //TODO: Retornar View Model
            return View(perfilModel);
        }

        [HttpPost]
        public ActionResult AdminPerfil(PerfilModel perfilModel)
        {
            ModelState.Remove("Estado");
            if (ModelState.IsValid)
            {
                try
                {
                    if (perfilModel.RegistrosModificados != null && perfilModel.RegistrosModificados != "[]")
                    {
                        List<PerfilFuncionalidadesModel> listado = new JavaScriptSerializer().Deserialize<List<PerfilFuncionalidadesModel>>(perfilModel.RegistrosModificados);
                        ServicioFuncionalidades servicioFuncionalidades = new ServicioFuncionalidades();
                        servicioFuncionalidades.ActualizarPerfilFuncionalidades(listado, perfilModel.Id);
                    }

                    perfilModel.Estado = (perfilModel.ChkEstado ? "Activo" : "Inactivo");
                    ServicioPerfiles servicioPerfiles = new ServicioPerfiles();
                    var perfilEntity = servicioPerfiles.CrearActualizarPerfil(perfilModel);

                    if (perfilEntity != null)
                    {
                        Session.Remove("idPerfil");
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

            return RedirectToActionPermanent("Index", "Perfiles");
        }
        #endregion

        #region Administracion de Funcionalidades

        public ActionResult ConsultarFuncionalidades(string consulta)
        {
            try
            {
                ViewBag.Consulta = consulta != null ? consulta : string.Empty;
                CargarFuncionalidadesPadre();
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }
            return View();
        }

        public ActionResult FuncionalidadesConsultar([DataSourceRequest] DataSourceRequest request, string consulta)
        {
            List<FuncionalidadesModel> funcionalidades = null;
            try
            {
                if (ViewBag.Consulta != null) consulta = ViewBag.Consulta;
                ServicioFuncionalidades servicioFuncionalidades = new ServicioFuncionalidades();
                funcionalidades = servicioFuncionalidades.CargarFuncionalidadesModel(consulta);
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return Json(funcionalidades.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FuncionalidadesCrearActualizar([DataSourceRequest] DataSourceRequest request, FuncionalidadesModel funcionalidad)
        {
            ServicioFuncionalidades servicioFuncionalidades = new ServicioFuncionalidades();
            ModelState.Remove("Estado");

            if (funcionalidad != null && ModelState.IsValid)
            {
                try
                {
                    funcionalidad.Estado = (funcionalidad.ChkActivo ? "Activo" : "Inactivo");
                    var entity = servicioFuncionalidades.CrearActualizarFuncionalidad(funcionalidad);
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, false);
                    ModelState.AddModelError("FuncionalidadYaRegistrada", ex.Message);
                }
            }

            return Json(new[] { funcionalidad }.ToDataSourceResult(request, ModelState));
        }

        private void CargarFuncionalidadesPadre()
        {
            List<FuncionalidadesModel> funcionalidades = null;
            ServicioFuncionalidades servicioFuncionalidades = new ServicioFuncionalidades();
            funcionalidades = servicioFuncionalidades.CargarFuncionalidadesPadreModel(0, true);
            ViewData["funcionalidades"] = funcionalidades;
        }

        public ActionResult CargarFuncionalidadesPadre([DataSourceRequest] DataSourceRequest request)
        {
            List<FuncionalidadesModel> funcionalidades = null;
            ServicioFuncionalidades servicioFuncionalidades = new ServicioFuncionalidades();
            funcionalidades = servicioFuncionalidades.CargarFuncionalidadesPadreModel(0, false);

            string perfilId = Session["idPerfil"].ToString();
            if (!string.IsNullOrEmpty(perfilId))
            {
                PerfilFuncionalidadesModel perfilFuncionalidad;
                List<PerfilFuncionalidadesModel> perfilFuncionalidades = servicioFuncionalidades.CargarPerfilFuncionalidades(int.Parse(perfilId));
                foreach (FuncionalidadesModel funcionalidad in funcionalidades)
                {
                    perfilFuncionalidad = perfilFuncionalidades.Where(x => x.FuncionalidadId == funcionalidad.Id).FirstOrDefault();
                    if (perfilFuncionalidad != null)
                    {
                        funcionalidad.PuedeConsultar = perfilFuncionalidad.PuedeConsultar;
                        funcionalidad.PuedeCrear = perfilFuncionalidad.PuedeCrear;
                        funcionalidad.PuedeEditar = perfilFuncionalidad.PuedeEditar;
                        funcionalidad.PuedeEliminar = perfilFuncionalidad.PuedeEliminar;
                    }
                }

            }
            return Json(funcionalidades.ToDataSourceResult(request));
        }

        public ActionResult ConsultarFuncionalidadesPorPadre([DataSourceRequest] DataSourceRequest request, int? Id)
        {
            try
            {
                ServicioFuncionalidades servicioFuncionalidades = new ServicioFuncionalidades();

                var funcionalidades = servicioFuncionalidades.CargarFuncionalidadesModel(string.Empty).Where(con => Id.HasValue ? con.PadreId == Id : con.PadreId == null)
                    .Select(e => new
                    {
                        e.Id,
                        e.PadreId,
                        e.Nombre,
                        hasChildren = (bool)servicioFuncionalidades.CargarFuncionalidadesModel(string.Empty).Where(c => c.PadreId == e.Id).ToList().Any()
                    }).ToList();

                return this.Json(funcionalidades, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            //TODO: Retornar View Model
            return View();
        }


        #endregion

    }

}