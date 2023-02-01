using System;
using Microsoft.Reporting.WebForms;
using Simco.ApplicationLayer.Servicios;

namespace Simco.Web.Reportes
{
    public partial class visorreporte : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                CargarReporte(Request.QueryString["reporte"]);
        }

        #region Metodos
        private void CargarReporte(string reporte)
        {
            string reportDataSourceName = string.Empty;
            string reportPath = string.Empty;
            object reportValues = null;
            ServicioReportes servicioReportes = new ServicioReportes();

            switch (reporte)
            {
                case "UsuariosPersonasNaturales":
                    reportDataSourceName = "UsuariosPersonasNaturalesDataSet";
                    reportValues = servicioReportes.UsuariosPersonasNaturalesListado(Request.QueryString["campofiltro"], Request.QueryString["consultafiltro"]);
                    reportPath = "Reports/ClientReportDefinitionFile/UsuariosPersonasNaturales.rdlc";
                    break;
                case "UsuariosPersonasJuridicas":
                    reportDataSourceName = "UsuariosPersonasJuridicasDataSet";
                    reportValues = servicioReportes.UsuariosPersonasJuridicasListado(Request.QueryString["campofiltro"], Request.QueryString["consultafiltro"]);
                    reportPath = "Reports/ClientReportDefinitionFile/UsuariosPersonasJuridicas.rdlc";
                    break;
                case "EntidadesMuseales":
                    reportDataSourceName = "EntidadesMusealesDataSet";
                    reportValues = servicioReportes.EntidadesMusealesListado(Request.QueryString["campofiltro"], Request.QueryString["consultafiltro"]);
                    reportPath = "Reports/ClientReportDefinitionFile/EntidadesMuseales.rdlc";
                    break;
                case "EncuestaResultadosGenerales":
                    reportDataSourceName = "EncuestaResultadosGeneralesDataSet";
                    reportValues = servicioReportes.EncuestaResultadosGenerales(Request.QueryString["encuesta"]);
                    reportPath = "Reports/ClientReportDefinitionFile/EncuestasResultadosGenerales.rdlc";
                    break;
                case "EncuestaResultadosPorUsuarios":
                    reportDataSourceName = "EncuestaResultadosPorUsuariosDataSet";
                    reportValues = servicioReportes.EncuestaResultadosPorUsuarios(Request.QueryString["encuesta"]);
                    reportPath = "Reports/ClientReportDefinitionFile/EncuestaResultadosPorUsuarios.rdlc";
                    break;
                case "EncuestaResultadosPorPreguntas":
                    reportDataSourceName = "EncuestaResultadosPorPreguntasDataSet";
                    reportValues = servicioReportes.EncuestaResultadosPorPreguntas(Request.QueryString["encuesta"]);
                    reportPath = "Reports/ClientReportDefinitionFile/EncuestaResultadosPorPreguntas.rdlc";
                    break;
                case "EncuestaResultadosPorUsuario":
                    reportDataSourceName = "EncuestaResultadosPorUsuarioDataSet";
                    reportValues = servicioReportes.EncuestaResultadosPorUsuario(Request.QueryString["respuesta"]);
                    reportPath = "Reports/ClientReportDefinitionFile/EncuestaResultadosPorUsuario.rdlc";
                    break;
                case "Curadurias":
                    reportDataSourceName = "CuraduriasDataSet";
                    reportValues = servicioReportes.CuraduriasListado(Request.QueryString["campofiltro"], Request.QueryString["consultafiltro"]);
                    reportPath = "Reports/ClientReportDefinitionFile/Curadurias.rdlc";
                    break;
                case "GuiasMonitorias":
                    reportDataSourceName = "GuiasMonitoriasDataSet";
                    reportValues = servicioReportes.GuiasMonitoriasListado(Request.QueryString["campofiltro"], Request.QueryString["consultafiltro"]);
                    reportPath = "Reports/ClientReportDefinitionFile/GuiasMonitorias.rdlc";
                    break;
                case "Museografias":
                    reportDataSourceName = "MuseografiasDataSet";
                    reportValues = servicioReportes.MuseografiasListado(Request.QueryString["campofiltro"], Request.QueryString["consultafiltro"]);
                    reportPath = "Reports/ClientReportDefinitionFile/Museografias.rdlc";
                    break;
                case "PlanesMuseologicos":
                    reportDataSourceName = "PlanesMuseologicosDataSet";
                    reportValues = servicioReportes.PlanesMuseologicosListado(Request.QueryString["campofiltro"], Request.QueryString["consultafiltro"]);
                    reportPath = "Reports/ClientReportDefinitionFile/PlanesMuseologicos.rdlc";
                    break;
                case "Publicaciones":
                    reportDataSourceName = "PublicacionesDataSet";
                    reportValues = servicioReportes.PublicacionesListado(Request.QueryString["campofiltro"], Request.QueryString["consultafiltro"]);
                    reportPath = "Reports/ClientReportDefinitionFile/Publicaciones.rdlc";
                    break;
                case "RestauracionesConservaciones":
                    reportDataSourceName = "RestauracionesConservacionesDataSet";
                    reportValues = servicioReportes.RestauracionesConservacionesListado(Request.QueryString["campofiltro"], Request.QueryString["consultafiltro"]);
                    reportPath = "Reports/ClientReportDefinitionFile/RestauracionesConservaciones.rdlc";
                    break;
                case "ServiciosEducativosCulturales":
                    reportDataSourceName = "ServiciosEducativosCulturalesDataSet";
                    reportValues = servicioReportes.ServiciosEducativosCulturalesListado(Request.QueryString["campofiltro"], Request.QueryString["consultafiltro"]);
                    reportPath = "Reports/ClientReportDefinitionFile/ServiciosEducativosCulturales.rdlc";
                    break;
                case "ServiciosEmpresas":
                    reportDataSourceName = "ServiciosEmpresasDataSet";
                    reportValues = servicioReportes.ServiciosEmpresasListado(Request.QueryString["campofiltro"], Request.QueryString["consultafiltro"]);
                    reportPath = "Reports/ClientReportDefinitionFile/ServiciosEmpresas.rdlc";
                    break;
                case "ProyectosDeApoyo":
                    reportDataSourceName = "ProyectosDeApoyoDataSet";
                    reportValues = servicioReportes.ProyectosDeApoyoListado(Request.QueryString["campofiltro"], Request.QueryString["consultafiltro"]);
                    reportPath = "Reports/ClientReportDefinitionFile/ProyectosDeApoyo.rdlc";
                    break;
                case "ProgramasAcademicos":
                    reportDataSourceName = "ProgramasAcademicosDataSet";
                    reportValues = servicioReportes.ProgramasAcademicosListado(Request.QueryString["campofiltro"], Request.QueryString["consultafiltro"]);
                    reportPath = "Reports/ClientReportDefinitionFile/ProgramasAcademicos.rdlc";
                    break;
                case "RepresentantesLegales":
                    reportDataSourceName = "RepresentantesLegalesDataSet";
                    reportValues = servicioReportes.RepresentantesLegalesListado(Request.QueryString["campofiltro"], Request.QueryString["consultafiltro"]);
                    reportPath = "Reports/ClientReportDefinitionFile/RepresentantesLegales.rdlc";
                    break;
                case "Documentos":
                    reportDataSourceName = "DocumentosDataSet";
                    reportValues = servicioReportes.DocumentosListado(Request.QueryString["urldocumentos"]);
                    reportPath = "Reports/ClientReportDefinitionFile/Documentos.rdlc";
                    break;
                case "Solicitudes":
                    reportDataSourceName = "SolicitudesDataSet";
                    reportValues = servicioReportes.SolicitudesListado();
                    reportPath = "Reports/ClientReportDefinitionFile/Solicitudes.rdlc";
                    break;
                case "TodasEntidadesMuseales":
                    reportDataSourceName = "TodasEntidadesMusealesDataSet";
                    reportValues = servicioReportes.TodasEntidadesMuseales_Listado(Request.QueryString["campofiltro"], Request.QueryString["consultafiltro"]);
                    reportPath = "Reports/ClientReportDefinitionFile/TodasEntidadesMuseales.rdlc";
                    break;
                case "DirectorioEnLinea":
                    reportDataSourceName = "DirectorioEnLineaDataSet";
                    reportValues = servicioReportes.DirectorioEnLinea_Listado(Request.QueryString["tipo"], Request.QueryString["depto"], Request.QueryString["mpio"], Request.QueryString["plb"]);
                    reportPath = "Reports/ClientReportDefinitionFile/DirectorioEnLinea.rdlc";
                    break;
                case "Correos":
                    reportDataSourceName = "CorreosDataSet";
                    reportValues = servicioReportes.Correos_Listado();
                    reportPath = "Reports/ClientReportDefinitionFile/Correos.rdlc";
                    break;
                case "PlandeFortalecimiento":
                    reportDataSourceName = "ReportePlanFortalecimiento";
                    reportValues = servicioReportes.PlanFortalecimientoListado(Request.QueryString["pejid"], Request.QueryString["pejdepto"], Request.QueryString["pejmpio"], Request.QueryString["anio"], Request.QueryString["fechainicial"], Request.QueryString["fechafinal"]);
                    reportPath = "Reports/ClientReportDefinitionFile/ReportePlanFortalecimiento.rdlc";
                    break;
                case "Cuatrienio":
                    reportDataSourceName = "ReporteCuatrienio";
                    reportValues = servicioReportes.Cuatrienio_Listado();
                    reportPath = "Reports/ClientReportDefinitionFile/ReporteCuatrienio.rdlc";
                    break;
                case "CuatrienioMensual":
                    reportDataSourceName = "ReporteCuatrienioMensual";
                    reportValues = servicioReportes.CuatrienioMensual_Listado(Request.QueryString["fechainicial"], Request.QueryString["fechafinal"]);
                    reportPath = "Reports/ClientReportDefinitionFile/ReporteCuatrienioMensual.rdlc";
                    break;
                case "PlanesdeFortalecimiento":
                    reportDataSourceName = "ReportePlanFortalecimientoContactos";
                    reportValues = servicioReportes.PlanFortalecimientoReporte();
                    reportPath = "Reports/ClientReportDefinitionFile/ReportePlanFortalecimientoContactos.rdlc";
                    break;

            }

            ReportDataSource reportDataSource = new ReportDataSource
            {
                Name = reportDataSourceName,
                Value = reportValues,
            };


            ReportViewer.LocalReport.ReportPath = reportPath;
            ReportViewer.LocalReport.DataSources.Clear();
            ReportViewer.LocalReport.DataSources.Add(reportDataSource);
            ReportViewer.LocalReport.Refresh();
        }

        #endregion
    }
}