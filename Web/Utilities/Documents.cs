using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Simco.ApplicationLayer.Servicios;
using Simco.CrossInfraestructureLayer;
using Simco.DomainModelLayer.Entidades;
using Simco.DomainModelLayer.ViewModels;
using Simco.Infrastructure.Notification;

namespace Simco.Utilities
{
    public static class Documents
    {
        public static bool ValidarDocumentosEntidadMuseal(EntidadMusealModel entidadMusealModel, out string mensajeResultadoValidacion, out Dictionary<string, string> modelError)
        {
            List<EtiquetaDominioModel> listaEtiquetaDominioModel = ObtenerConfiguracionEtiquetasDominio("ConfiguracionCargaArchivoImagen");
            List<EtiquetaDominioModel> listaEtiquetaDominioModelDcto = ObtenerConfiguracionEtiquetasDominio("ConfiguracionCargaArchivoDocumentos");
            modelError = new Dictionary<string, string>();

            if (!ValidarDocumentoEntidadMuseal(entidadMusealModel.DocumentoFotoFachada, listaEtiquetaDominioModel, out mensajeResultadoValidacion))
            {
                modelError.Add("DocumentoFotoFachada", mensajeResultadoValidacion);
            }
            if (!ValidarDocumentoEntidadMuseal(entidadMusealModel.DocumentoFotoInteraccion, listaEtiquetaDominioModel, out mensajeResultadoValidacion))
            {
                modelError.Add("DocumentoFotoInteraccion", mensajeResultadoValidacion);
            }
            if (!ValidarDocumentoEntidadMuseal(entidadMusealModel.DocumentoFotoColeccion1, listaEtiquetaDominioModel, out mensajeResultadoValidacion))
            {
                modelError.Add("DocumentoFotoColeccion1", mensajeResultadoValidacion);
            }
            if (!ValidarDocumentoEntidadMuseal(entidadMusealModel.DocumentoFotoColeccion2, listaEtiquetaDominioModel, out mensajeResultadoValidacion))
            {
                modelError.Add("DocumentoFotoColeccion2", mensajeResultadoValidacion);
            }
            if (!ValidarDocumentoEntidadMuseal(entidadMusealModel.DocumentoFotoColeccion3, listaEtiquetaDominioModel, out mensajeResultadoValidacion))
            {
                modelError.Add("DocumentoFotoColeccion3", mensajeResultadoValidacion);
            }
            if (!ValidarDocumentoEntidadMuseal(entidadMusealModel.DocumentoLegalExistencia, listaEtiquetaDominioModelDcto, out mensajeResultadoValidacion))
            {
                modelError.Add("DocumentoLegalExistencia", mensajeResultadoValidacion);
            }
            if (!ValidarDocumentoEntidadMuseal(entidadMusealModel.DocumentoPoliticaPlanColecciones, listaEtiquetaDominioModelDcto, out mensajeResultadoValidacion))
            {
                modelError.Add("DocumentoPoliticaPlanColecciones", mensajeResultadoValidacion);
            }

            if(modelError.Count > 0)
                mensajeResultadoValidacion = modelError.First().Value;

            return modelError.Count > 0 ? false : true;
        }

        public static bool ValidarDocumentoEntidadMuseal(System.Web.HttpPostedFileBase httpPostedFileBase, List<EtiquetaDominioModel> listaEtiquetaDominioModel,
            out string mensajeResultadoValidacion)
        {
            mensajeResultadoValidacion = string.Empty;

            if (httpPostedFileBase != null && httpPostedFileBase.ContentLength > 0)
            {
                string extension = ObtenerExtensionArchivo(httpPostedFileBase.FileName);
                string valor = listaEtiquetaDominioModel.Find(x => x.Etiqueta.Equals("TamanoMaximoArchivo")).Valor;
                int tamanoMaximoArchivo = 0;
                List<string> extensionesPermitidas = listaEtiquetaDominioModel.Find(x => x.Etiqueta.Equals("TiposArchivo")).Valor.ToLower().Split(',').ToList();

                int.TryParse(valor, out tamanoMaximoArchivo);

                if (!extensionesPermitidas.Exists(x => x.Equals(extension)))
                {
                    mensajeResultadoValidacion = string.Format("{0} [{1}]. Formato requerido {2}.",
                        "El documento tiene un formato no permitido ", httpPostedFileBase.FileName,
                        listaEtiquetaDominioModel.Find(x => x.Etiqueta.Equals("TiposArchivo")).Valor);
                }
                else if (httpPostedFileBase.ContentLength > tamanoMaximoArchivo)
                {
                    mensajeResultadoValidacion = string.Format("{0} [{1}]. Tamaño máximo permitido {2:0.00} Kb.",
                        "El documento tiene un tamaño mayor al permitido ", httpPostedFileBase.FileName,
                        ((decimal)tamanoMaximoArchivo / (decimal)1024));
                }
            }

            return mensajeResultadoValidacion.Equals(string.Empty) ? true : false;
        }

        private static string ObtenerExtensionArchivo(string nombreArchivo)
        {
            string extension = string.Empty;

            extension = System.IO.Path.GetExtension(nombreArchivo);

            if (extension.Equals(string.Empty))
            {
                extension = nombreArchivo.Substring(nombreArchivo.LastIndexOf("/") + 1);
            }

            return extension.Replace(".", string.Empty).ToLower();
        }

        public static List<EtiquetaDominioModel> ObtenerConfiguracionEtiquetasDominio(string nombreDominio)
        {
            try
            {
                ServicioConfiguracion servicioConfiguracion = new ServicioConfiguracion();
                return servicioConfiguracion.ObtenerModeloEtiquetasDominio(nombreDominio);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public static string ObtenerExtensionesImagen(string nombreEtiqueta)
        {
            List<EtiquetaDominioModel> listaEtiquetaDominioModelDcto = ObtenerConfiguracionEtiquetasDominio("ConfiguracionCargaArchivoImagen");
            return listaEtiquetaDominioModelDcto.Find(x => x.Etiqueta.Equals("TiposArchivo")).Valor;
        }

        public static string ObtenerExtensionesDocumento(string nombreEtiqueta)
        {
            List<EtiquetaDominioModel> listaEtiquetaDominioModelDcto = ObtenerConfiguracionEtiquetasDominio("ConfiguracionCargaArchivoDocumentos");
            return listaEtiquetaDominioModelDcto.Find(x => x.Etiqueta.Equals("TiposArchivo")).Valor;
        }

        public static bool ValidarTamañoDocumentoComiteDocumento(System.Web.HttpPostedFileBase httpPostedFileBase, List<EtiquetaDominioModel> listaEtiquetaDominioModel,
            out string mensajeResultadoValidacion)
        {
            string valor = listaEtiquetaDominioModel.Find(x => x.Etiqueta.Equals("TamanoMaximoArchivo")).Valor;
            int tamanoMaximoArchivo = 0;
            List<string> extensionesPermitidas = listaEtiquetaDominioModel.Find(x => x.Etiqueta.Equals("TiposArchivo")).Valor.ToLower().Split(',').ToList();

            int.TryParse(valor, out tamanoMaximoArchivo);
            mensajeResultadoValidacion = string.Empty;

            if (httpPostedFileBase != null && httpPostedFileBase.ContentLength > 0)
            {
                if (httpPostedFileBase.ContentLength > tamanoMaximoArchivo)
                {
                    mensajeResultadoValidacion = string.Format("{0} [{1}]. Tamaño máximo permitido {2:0.00} Kb.",
                        "El documento tiene un tamaño mayor al permitido ", httpPostedFileBase.FileName,
                        ((decimal)tamanoMaximoArchivo / (decimal)1024));
                }
            }

            return mensajeResultadoValidacion.Equals(string.Empty) ? true : false;
        }
    }
}