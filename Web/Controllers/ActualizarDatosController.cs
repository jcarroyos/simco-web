using Simco.ApplicationLayer.Servicios;
using Simco.CrossInfraestructureLayer;
using Simco.DomainModelLayer.ViewModels;
using Simco.Infrastructure.Notification;
using System;
using System.Web.Mvc;

namespace Simco.Controllers
{
    [Authorize]
    public class ActualizarDatosController : BaseController
    {
        public ActionResult Index()
        {
            return ObtenerSesion();
        }

        #region Persona natural

        public ActionResult PersonaNatural()
        {
            ServicioPersonasNaturales servicioPersonas = new ServicioPersonasNaturales();
            PersonaNaturalRegistroModel personaNaturalRegistroModel = servicioPersonas.ObtenerPersonaNaturalRegistroModel(UsuarioId);

            if (personaNaturalRegistroModel.PersonaNatural != null)
                return View(personaNaturalRegistroModel);

            return RedirectToActionPermanent("Index", "Inicio");
        }

        [HttpPost]
        public ActionResult PersonaNatural(PersonaNaturalRegistroModel personaNaturalRegistroModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (personaNaturalRegistroModel.PersonaNatural.ImageUpload != null)
                        personaNaturalRegistroModel.PersonaNatural.Imagen = Imagenes.ConvertirImagenAByte(personaNaturalRegistroModel.PersonaNatural.ImageUpload);

                    ServicioPersonasNaturales servicioPersonaNatural = new ServicioPersonasNaturales();
                    PersonaNaturalModel personaNaturalModel = personaNaturalRegistroModel.PersonaNatural;

                    if (servicioPersonaNatural.ActualizarPersonaNatural(personaNaturalModel))
                        this.ShowMessage(MessageType.Success, Constante.PersonaNaturalRegistrada);
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return PersonaNatural();
        }

        #endregion

        #region Persona juridica

        public ActionResult PersonaJuridica()
        {
            ServicioPersonasJuridicas servicioPersonas = new ServicioPersonasJuridicas();
            return View(servicioPersonas.ObtenerPersonaJuridicaRegistroModel(UsuarioId));
        }

        [HttpPost]
        public ActionResult PersonaJuridica(PersonaJuridicaRegistroModel personaJuridicaRegistroModel)
        {
            ModelState.Remove("PersonaJuridica.Categoria");
            ModelState.Remove("PersonaJuridica.NombreDeLaInstitucion");
            ModelState.Remove("PersonaJuridica.NombreContacto");
            ModelState.Remove("PersonaJuridica.DescripcionInstitucion");

            if (ModelState.IsValid)
            {
                try
                {
                    personaJuridicaRegistroModel.PersonaJuridica.Categoria = TiposAgente.EntidadMuseal.ToString();
                    personaJuridicaRegistroModel.PersonaJuridica.Horario = "Sí";

                    if (personaJuridicaRegistroModel.PersonaJuridica.ImageUpload != null)
                        personaJuridicaRegistroModel.PersonaJuridica.Imagen = Imagenes.ConvertirImagenAByte(personaJuridicaRegistroModel.PersonaJuridica.ImageUpload);

                    ServicioPersonasJuridicas servicioPersonaJuridica = new ServicioPersonasJuridicas();
                    if (servicioPersonaJuridica.ActualizarEntidadMuseal(personaJuridicaRegistroModel.PersonaJuridica))
                        this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios);

                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return PersonaJuridica();
        }

        #endregion

        #region Institucion aliada

        public ActionResult InstitucionAliada()
        {
            ServicioPersonasJuridicas servicioPersonas = new ServicioPersonasJuridicas();
            return View(servicioPersonas.ObtenerPersonaJuridicaRegistroModel(UsuarioId));
        }

        [HttpPost]
        public ActionResult InstitucionAliada(PersonaJuridicaRegistroModel personaJuridicaRegistroModel)
        {
            ModelState.Remove("PersonaJuridica.Nombre");
            ModelState.Remove("PersonaJuridica.Resena");

            if (ModelState.IsValid)
            {
                try
                {

                    personaJuridicaRegistroModel.PersonaJuridica.Nombre = personaJuridicaRegistroModel.PersonaJuridica.NombreDeLaInstitucion;
                    personaJuridicaRegistroModel.PersonaJuridica.Resena = personaJuridicaRegistroModel.PersonaJuridica.DescripcionInstitucion;
                    personaJuridicaRegistroModel.PersonaJuridica.Horario = "No";

                    if (personaJuridicaRegistroModel.PersonaJuridica.ImageUpload != null)
                        personaJuridicaRegistroModel.PersonaJuridica.Imagen = Imagenes.ConvertirImagenAByte(personaJuridicaRegistroModel.PersonaJuridica.ImageUpload);

                    ServicioPersonasJuridicas servicioPersonaJuridica = new ServicioPersonasJuridicas();
                    if (servicioPersonaJuridica.ActualizarEntidadMuseal(personaJuridicaRegistroModel.PersonaJuridica))
                        this.ShowMessage(MessageType.Success, Constante.PersonaNaturalRegistrada);

                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return InstitucionAliada();
        }

        #endregion

        public ActionResult CambiarContrasena()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CambiarContrasena(ContrasenaCambiarModel recuperarModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (ActualizarContrasena(recuperarModel))
                        this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios);
                    else
                        this.ShowMessage(MessageType.Alert, Constante.ErrorRegistro);
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return View();
        }

        public ActionResult CambiarCorreoElectronico()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CambiarCorreoElectronico(CorreoElectronicoCambiarModel correoElectronicoCambiarModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (ActualizarCorreoElectronico(correoElectronicoCambiarModel))
                        this.ShowMessage(MessageType.Success, Constante.MensajeGuardarCambios);
                    else
                        this.ShowMessage(MessageType.Alert, Constante.ErrorRegistro);
                }
                catch (Exception ex)
                {
                    this.ShowMessage(MessageType.Error, ex.Message, true);
                }
            }

            return View();
        }
    }
}