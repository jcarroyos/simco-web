using Simco.ApplicationLayer.Servicios;
using Simco.DomainModelLayer.ViewModels;
using Simco.Infrastructure.Notification;
using System;
using System.Web.Mvc;

namespace Simco.Controllers
{
    public class ColeccionesController : BaseController
    {
        // GET: Colecciones
        public ActionResult Index()
        {
            try
            {
                ServicioColecciones servicioColecciones = new ServicioColecciones();
                return View(servicioColecciones.CargarControles());
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return View();
        }

        [HttpPost]
        public ActionResult BuscarColecciones(string opcionId, string museoId, string autor, string titulo, string tecnica, string anoSiglo, string acdc, string desde, string hasta, string pagina)
        {
            try
            {
                ColeccionModel coleccionesModel = new ColeccionModel();

                if (!string.IsNullOrEmpty(opcionId))
                    coleccionesModel.OpcId = opcionId;

                if (!string.IsNullOrEmpty(museoId))
                {
                    if (museoId == "11001-33") //Museo Nacional
                        coleccionesModel.CodigoIdentificacion = "01";
                    else
                        coleccionesModel.CodigoIdentificacion = museoId;
                }

                if (!string.IsNullOrEmpty(autor))
                    coleccionesModel.Autor = autor;

                if (!string.IsNullOrEmpty(titulo))
                    coleccionesModel.Titulo = titulo;

                if (!string.IsNullOrEmpty(tecnica))
                    coleccionesModel.Tecnica = tecnica;

                coleccionesModel.ModificadorAnio = "d.C";
                if (acdc == "0")     // a.C
                    coleccionesModel.ModificadorAnio = "a.C";

                if (anoSiglo == "0") // Anio
                {
                    coleccionesModel.AnioDesde = desde;
                    coleccionesModel.AnioHasta = hasta;
                }
                else
                {
                    coleccionesModel.SigloDesde = desde;
                    coleccionesModel.SigloHasta = hasta;
                }

                coleccionesModel.UsuarioId = -1;
                coleccionesModel.PersonaId = -1;

                if (!String.IsNullOrEmpty(pagina))
                {
                    if (Session["Pagina"] != null)
                        coleccionesModel.Pagina = int.Parse(Session["Pagina"].ToString());

                    coleccionesModel.Pagina = coleccionesModel.Pagina + int.Parse(pagina);
                    if (int.Parse(pagina) == 0)
                        coleccionesModel.Pagina = 1;

                    coleccionesModel.Pagina = (coleccionesModel.Pagina == 0 ? 1 : coleccionesModel.Pagina);
                    Session.Add("Pagina", coleccionesModel.Pagina);
                }

                ServicioColecciones servicioColecciones = new ServicioColecciones();
                coleccionesModel = servicioColecciones.ObtenerColeccionEntidadMuseal(coleccionesModel);

                return PartialView("_ColeccionObjeto", coleccionesModel.Coleccion);

            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return Content(@"<div style=""text-align: center; margin: 15px 0px 15px 0px;"">No se han encontrado más elementos</div> <script type=""text/javascript"">$('#divbtnMostrarMas').hide();</script>", "text/html");
        }

        [HttpPost]
        public ActionResult CargarPaginaColeccion(string token, int pagina, int numeroElementos, string autor, string titulo, string nombreColeccion, string palabrasClaves, string opcionId)
        {
            try
            {
                ColeccionModel coleccionModel = new ColeccionModel();

                coleccionModel.Pagina = pagina;
                coleccionModel.ItmesPag = numeroElementos;
                coleccionModel.Autor = autor;
                coleccionModel.Titulo = titulo;
                coleccionModel.NombreColeccion = nombreColeccion;
                coleccionModel.PalabrasClaves = palabrasClaves;
                coleccionModel.OpcId = opcionId;
                coleccionModel.UsuarioId = -1;
                coleccionModel.PersonaId = -1;

                ServicioColecciones servicioColecciones = new ServicioColecciones();
                coleccionModel = servicioColecciones.ObtenerColeccionEntidadMuseal(coleccionModel);

                if (coleccionModel != null)
                {
                    ViewBag.EsImpar = (numeroElementos % 2) != 0;
                    return PartialView("_ColeccionObjeto", coleccionModel.Coleccion);
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return Content(@"<div style=""text-align: center; margin: 15px 0px 15px 0px;"">No se han encontrado más elementos</div> <script type=""text/javascript"">$('#divbtnMostrarMas').hide();</script>", "text/html");
        }

        public ActionResult DetalleObjetoColeccion(string id, string codigomuseo)
        {
            try
            {
                ServicioColecciones servicioColecciones = new ServicioColecciones();
                return View(servicioColecciones.ObtenerDetalleObjeto(id, codigomuseo));
            }
            catch (Exception ex)
            {
                this.ShowMessage(MessageType.Error, ex.Message, true);
            }

            return RedirectToActionPermanent("Index", "Colecciones");
        }

    }
}