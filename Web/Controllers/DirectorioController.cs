using Simco.DomainModelLayer.ViewModels;
using System.Web.Mvc;
using Simco.ApplicationLayer.Servicios;

namespace Simco.Controllers
{
    public class DirectorioController : BaseController
    {
        // GET: Directorio
        public ActionResult Index()
        {
            ServicioDirectorio servicioDirectorio = new ServicioDirectorio();
            InicioSesionModel inicioSesionModel = ObtenerSesionModel();
            inicioSesionModel.Directorio = servicioDirectorio.Buscar(new DirectorioModel(), 3);

            return View(inicioSesionModel);
        }

        [HttpPost]
        public ActionResult Buscar(InicioSesionModel inicioSesionModel)
        {
            ServicioDirectorio servicioDirectorio = new ServicioDirectorio();
            inicioSesionModel.Directorio = servicioDirectorio.Buscar(inicioSesionModel.Directorio, null);

            return View(inicioSesionModel);
        }

        public ActionResult Museo(string personaJuridicaId)
        {
            ServicioDirectorio servicioDirectorio = new ServicioDirectorio();
            InicioSesionModel inicioSesionModel = ObtenerSesionModel();

            inicioSesionModel.Directorio = servicioDirectorio.BuscarMuseo(int.Parse(personaJuridicaId));

            return View(inicioSesionModel);
        }

       
    }
}