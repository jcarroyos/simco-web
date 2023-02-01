using System.Web.Mvc;

namespace Simco.Controllers
{
    [Authorize]
    public class AsesorController : BaseController
    {
        // GET: Asesor
        public ActionResult Index()
        {
            return RedirectToActionPermanent("SolicitudesAsignadas", "Administrador");
        }
    }
}