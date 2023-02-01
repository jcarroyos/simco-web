using Simco.ApplicationLayer.Servicios;
using Simco.CrossInfraestructureLayer;
using Simco.DomainModelLayer.Entidades;
using Simco.Infrastructure.Notification;
using System;
using System.Web.Mvc;

namespace Simco.Controllers
{
    public class DocumentosController : BaseController
    {
        // GET: Documentos
        public FileContentResult Index(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    ServicioDocumentos servicioDocumentos = new ServicioDocumentos();
                    DOCUMENTOS documentoEntity = servicioDocumentos.ObtenerDocumento(Convert.ToInt32(Criptografia.DesencriptarUrl(id)));

                    if (documentoEntity != null)
                    {
                        HttpContext.Response.AddHeader("content-disposition", "attachment; filename=" + documentoEntity.DOC_NOMBRE);
                        return File(documentoEntity.DOC_ARCHIVO, documentoEntity.DOC_TIPOMIME);
                    }
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return null;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public FileContentResult IndexComitDocum(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var servicioDocumentos = new ServicioComiteDocumentos();
                    var documentoEntity = servicioDocumentos.ObtenerDocumento(Convert.ToInt32(id));

                    if (documentoEntity != null)
                    {
                        HttpContext.Response.AddHeader("content-disposition", "attachment; filename=" + documentoEntity.DOC_NOMBRE);
                        return File(documentoEntity.DOC_ARCHIVO, documentoEntity.DOC_TIPOMIME);
                    }
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