using Simco.ApplicationLayer.Servicios;
using Simco.CrossInfraestructureLayer;
using Simco.Infrastructure.Notification;
using System.Web.Mvc;

namespace Simco.Controllers
{
    public class InicioController : BaseController
    {
        // GET: Inicio
        public ActionResult Index()
        {
            // Carga encuestas activas por contestar
            if (TipoAgente == TiposAgente.EntidadMuseal.ToString())
            {
                ServicioEncuentas servicioEncuentas = new ServicioEncuentas();
                ViewBag.ListaEncuestas = servicioEncuentas.ObtenerEncuestasDisponibles(UsuarioId);
            }

            if (Perfil == Perfiles.Administrador)
            {
                try
                {
                    ServicioComites servicioComites = new ServicioComites();
                    servicioComites.ActualizarComites();

                    ServicioSolicitudes servicioSolicitudes = new ServicioSolicitudes();
                    if (servicioSolicitudes.EnviarCorreosSolicitudesVencidas())
                        this.ShowMessage(MessageType.Information, "Correo de solicitudes vencidas, enviado exitosamente", true);

                }
                catch
                {
                    this.ShowMessage(MessageType.Error, Constante.ErrorRegistrarEncuestas, true);
                }

                return RedirectToActionPermanent("Index", "Administrador");
            }

            if (Perfil == Perfiles.Asesor)
                return RedirectToActionPermanent("Index", "Asesor");

            return View();
        }
    }
}