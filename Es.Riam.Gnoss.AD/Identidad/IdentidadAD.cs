using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.CMS;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Blog;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Notificacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.PersonaDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Suscripcion;
using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Util;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;

namespace Es.Riam.Gnoss.AD.Identidad
{
    #region Enumeraciones

    /// <summary>
    /// Tipos posibles de identidad en GNOSS
    /// </summary>
    public enum TiposIdentidad
    {
        /// <summary>
        /// Identidad personal
        /// </summary>
        Personal = 0,
        /// <summary>
        /// Identidad de una persona en una organizaci�n que trabaja en modo personal
        /// </summary>
        ProfesionalPersonal = 1,
        /// <summary>
        /// Identidad de una persona en una organizaci�n que trabaja en modo corporativo
        /// </summary>
        ProfesionalCorporativo = 2,
        /// <summary>
        /// Identidad de una organizaci�n
        /// </summary>
        Organizacion = 3,
        /// <summary>
        /// Identidad de una persona que es profesor.
        /// </summary>
        Profesor = 4
    }

    /// <summary>
    /// Enumeraci�n para seleccionar que tipo de miembros queremos visualizar en la p�gina
    /// </summary>
    public enum TipoMiembros
    {
        /// <summary>
        /// Muestra todo tipo de miembros, incluyendo miembros bloqueados, o que han sido expulsados
        /// </summary>
        Todos,
        /// <summary>
        /// Solo muestra los miembros que se encuentran activos actualmente (no est�n bloqueados ni expulsados)
        /// </summary>
        Activos,
        /// <summary>
        /// Muestra lo miembros bloqueados actualmente
        /// </summary>
        Bloqueados,
        /// <summary>
        /// Muestra los miembros que han sido expulsados de la comunidad
        /// </summary>
        Expulsados,
        /// <summary>
        /// Muestra los miembros que no reciben Newsletter
        /// </summary>
        NoNewsLetter
    }

    #endregion

    public class JoinIdentidadPerfil
    {
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinIdentidadPerfilOrg
    {
        public PerfilOrganizacion PerfilOrganizacion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinIdentidadPerfilOrganizacionPerfilPersonaOrg
    {
        public PerfilOrganizacion PerfilOrganizacion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PerfilPersonaOrg PerfilPersonaOrg { get; set; }
    }

    public class JoinIdentidadPerfilPersona
    {
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Persona Persona { get; set; }
    }

    public class JoinIdentidadPerfilPerfilPersonaOrg
    {
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PerfilPersonaOrg PerfilPersonaOrg { get; set; }
    }

    public class JoinIdentidadPerfilPersonaProvincia
    {
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Persona Persona { get; set; }
        public AD.EntityModel.Models.Pais.Provincia Provincia { get; set; }
    }

    public class JoinIdentidadPerfilPersonaPais
    {
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Persona Persona { get; set; }
        public AD.EntityModel.Models.Pais.Pais Pais { get; set; }
    }

    public class JoinIdentidadPerfilPersonaAdministradorProyecto
    {
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Persona Persona { get; set; }
        public AdministradorProyecto AdministradorProyecto { get; set; }
    }

    public class JoinIdentidadPerfilPersonaAdministradorProyectoProyecto
    {
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Persona Persona { get; set; }
        public AdministradorProyecto AdministradorProyecto { get; set; }
        public Proyecto Proyecto { get; set; }
    }

    public class JoinIdentidadPerfilPersonaUsuario
    {
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Persona Persona { get; set; }
        public Usuario Usuario { get; set; }
    }

    public class JoinIdentidadPerfilPersonaProyecto
    {
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Persona Persona { get; set; }
        public Proyecto Proyecto { get; set; }
    }

    public class JoinIdentidadPerfilProyecto
    {
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Proyecto Proyecto { get; set; }
    }

    public class JoinIdentidadProyecto
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Proyecto Proyecto { get; set; }
    }
    public class JoinIdentidadProyectoIdentidad
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Proyecto Proyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad2 { get; set; }
    }
    public class JoinIdentidadPermisoAmigoOrg
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PermisoAmigoOrg PermisoAmigoOrg { get; set; }
    }

    public class JoinIdentidadSuscripcion
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
    }

    public class JoinResultadoSuscripcionSuscripcion
    {
        public ResultadoSuscripcion ResultadoSuscripcion { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
    }

    public class JoinPerfilIdentidad
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinPerfilRedesSocialesPerfil
    {
        public PerfilRedesSociales PerfilRedesSociales { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinIdentidadPerfilCurriculum
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public Persona Persona { get; set; }
        public EntityModel.Models.PersonaDS.Curriculum Curriculum { get; set; }
    }
    public class JoinPerfilIdentidadSuscripcion
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
    }

    public class JoinIdentidadPerfilRedesSociales
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PerfilRedesSociales PerfilRedesSociales { get; set; }
    }

    public class JoinIdentidadPerfilRedesSocialesSuscripcion
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PerfilRedesSociales PerfilRedesSociales { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }

    }

    public class JoinPerfilPersonaIdentidad
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PerfilPersona PerfilPersona { get; set; }
    }
    public class JoinPerfilPersonaIdentidadSuscripcion
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PerfilPersona PerfilPersona { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
    }

    public class JoinPerfilPersonaOrgIdentidad
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PerfilPersonaOrg PerfilPersonaOrg { get; set; }
    }
    public class JoinPerfilPersonaOrgIdentidadSuscripcion
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PerfilPersonaOrg PerfilPersonaOrg { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
    }

    public class JoinProfesorIdentidad
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Profesor Profesor { get; set; }
    }
    public class JoinProfesorIdentidadSuscripcion
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Profesor Profesor { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
    }

    public class JoinAmigoIdentidad
    {
        public Amigo Amigo { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinPerfilIdentidadPersona
    {
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinPerfilIdentidadPersonaPersonaVinculoOrganizacion
    {
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PersonaVinculoOrganizacion PersonaVinculoOrganizacion { get; set; }
    }

    public class JoinPerfilIdentidadPersonaTesauroUsuario
    {
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public EntityModel.Models.Tesauro.TesauroUsuario TesauroUsuario { get; set; }
    }

    public class JoinPerfilIdentidadPersonaTesauroUsuarioCategoriaTesauro
    {
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public EntityModel.Models.Tesauro.TesauroUsuario TesauroUsuario { get; set; }
        public EntityModel.Models.Tesauro.CategoriaTesauro CategoriaTesauro { get; set; }
    }
    public class JoinPerfilIdentidadPersonaProyectoRolUsuario
    {
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public ProyectoRolUsuario ProyectoRolUsuario { get; set; }
    }
    public class JoinPerfilIdentidadAmigo
    {
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Amigo Amigo { get; set; }
    }
    public class JoinPerfilPersonaIdentidadAmigo
    {
        public PerfilPersona PerfilPersona { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Amigo Amigo { get; set; }
    }

    public class JoinPerfilPersonaOrgIdentidadAmigo
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Amigo Amigo { get; set; }
        public PerfilPersonaOrg PerfilPersonaOrg { get; set; }
    }

    public class JoinPerfilOrganizacionPerfilPersonaOrg
    {
        public PerfilPersonaOrg PerfilPersonaOrg { get; set; }
        public PerfilOrganizacion PerfilOrganizacion { get; set; }
    }

    public class JoinPerfilOrganizacionIdentidad
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PerfilOrganizacion PerfilOrganizacion { get; set; }
    }
    public class JoinPerfilOrganizacionIdentidadAmigo
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PerfilOrganizacion PerfilOrganizacion { get; set; }
        public Amigo Amigo { get; set; }
    }
    public class JoinIdentidadPerfilRedesSocialesAmigo
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PerfilRedesSociales PerfilRedesSociales { get; set; }
        public Amigo Amigo { get; set; }
    }
    public class JoinPerfilPersonaOrgIdentidadOrganizacion
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PerfilPersonaOrg PerfilPersonaOrg { get; set; }
        public Organizacion Organizacion { get; set; }
    }
    public class JoinPersonaVinculoOrganizacionPersona
    {
        public Persona Persona { get; set; }
        public PersonaVinculoOrganizacion PersonaVinculoOrganizacion { get; set; }
    }
    public class JoinPerfilIdentidadPersonaPerfilPersona
    {
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PerfilPersona PerfilPersona { get; set; }
    }
    public class JoinPerfilIdentidadPerfilPersona
    {
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PerfilPersona PerfilPersona { get; set; }
    }

    public class JoinPerfilIdentidadPerfilPersonaOrg
    {
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PerfilPersonaOrg PerfilPersonaOrg { get; set; }
    }
    public class JoinSuscripcionIdentidadProyectoSuscripcion
    {
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
    }
    public class JoinSuscripcionIdentidadProyectoSuscripcionIdentidad
    {
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }
    public class JoinIdentidadSuscripcionSuscripcionIdentidadProyecto
    {
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }
    public class JoinIdentidadSuscripcionSuscripcionTesauroUsuario
    {
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public SuscripcionTesauroUsuario SuscripcionTesauroUsuario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinIdentidadSuscripcionSuscripcionTesauroUsuarioPersona
    {
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public SuscripcionTesauroUsuario SuscripcionTesauroUsuario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Persona Persona { get; set; }
    }
    public class JoinIdentidadSuscripcionSuscripcionTesauroUsuarioPersonaPerfil
    {
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public SuscripcionTesauroUsuario SuscripcionTesauroUsuario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
    }
    public class JoinIdentidadSuscripcionSuscripcionIdentidadProyectoIdentidad2
    {
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad2 { get; set; }
    }
    public class JoinIdentidadSuscripcionSuscripcionIdentidadProyectoIdentidad2Perfil
    {
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad2 { get; set; }
        public Perfil Perfil { get; set; }
    }
    public class JoinIdentidadSuscripcionSuscripcionIdentidadProyectoIdentidad2PerfilPersona
    {
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad2 { get; set; }
        public Perfil Perfil { get; set; }
        public Persona Persona { get; set; }
    }
    public class JoinIdentidadSuscripcionSuscripcionTesauroUsuarioPerfil
    {
        public SuscripcionTesauroUsuario SuscripcionTesauroUsuario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public Perfil Perfil { get; set; }
    }
    public class JoinIdentidadSuscripcionIdentidad2
    {
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad2 { get; set; }
    }
    public class JoinIdentidadSuscripcionIdentidad2Identidad3
    {
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad2 { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad3 { get; set; }
    }

    public class JoinPerfilOrganizacionIdentidadPerfil
    {
        public PerfilOrganizacion PerfilOrganizacion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinIdentidadPerfilRedesSocialesPerfil
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PerfilRedesSociales PerfilRedesSociales { get; set; }
        public Perfil Perfil { get; set; }
    }
    public class JoinProfesorIdentidadPerfil
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Profesor Profesor { get; set; }
        public Perfil Perfil { get; set; }
    }
    public class JoinPerfilPersonaPerfil
    {
        public PerfilPersona PerfilPersona { get; set; }
        public Perfil Perfil { get; set; }
    }
    public class JoinPerfilPersonaOrgPerfil
    {
        public Perfil Perfil { get; set; }
        public PerfilPersonaOrg PerfilPersonaOrg { get; set; }
    }
    public class JoinPerfilOrganizacionPerfil
    {
        public Perfil Perfil { get; set; }
        public PerfilOrganizacion PerfilOrganizacion { get; set; }
    }

    public class JoinProfesorPerfil
    {
        public Perfil Perfil { get; set; }
        public Profesor Profesor { get; set; }
    }

    public class JoinPerfilIdentidadOrganizacion
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Organizacion Organizacion { get; set; }
        public Perfil Perfil { get; set; }
    }
    public class JoinPerfilPersonaIdentidadPersona
    {
        public PerfilPersona PerfilPersona { get; set; }
        public Persona Persona { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }
    public class JoinGrupoIdentGrupoIdentOrganizacion
    {
        public GrupoIdentidades GrupoIdentidades { get; set; }
        public GrupoIdentidadesOrganizacion GrupoIdentidadesOrganizacion { get; set; }
    }
    public class JoinGrupoIdentGrupoIdentOrganizacionPerfilPersonaOrg
    {
        public GrupoIdentidades GrupoIdentidades { get; set; }
        public GrupoIdentidadesOrganizacion GrupoIdentidadesOrganizacion { get; set; }
        public PerfilPersonaOrg PerfilPersonaOrg { get; set; }
    }
    public class JoinGrupoIdentGrupoIdentidadesProyecto
    {
        public GrupoIdentidades GrupoIdentidades { get; set; }
        public GrupoIdentidadesProyecto GrupoIdentidadesProyecto { get; set; }
    }

    public class JoinGrupoIdentidadesParticipacionIdentidad
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public GrupoIdentidadesParticipacion GrupoIdentidadesParticipacion { get; set; }
    }

    public class JoinGrupoIdentidadesParticipacionIdentidadPerfil
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public GrupoIdentidadesParticipacion GrupoIdentidadesParticipacion { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinPerfilIdentidadProyectoUsuarioIdentidad
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public ProyectoUsuarioIdentidad ProyectoUsuarioIdentidad { get; set; }

    }
    public class JoinPerfilIdentidadProyectoUsuarioIdentidadPerfilRedesSociales
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public ProyectoUsuarioIdentidad ProyectoUsuarioIdentidad { get; set; }
        public PerfilRedesSociales PerfilRedesSociales { get; set; }
    }
    public class JoinPerfilPersonaIdentidadProyectoUsuarioIdentidad
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PerfilPersona PerfilPersona { get; set; }
        public ProyectoUsuarioIdentidad ProyectoUsuarioIdentidad { get; set; }

    }
    public class JoinPerfilPersonaOrgIdentidadProyectoUsuarioIdentidad
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PerfilPersonaOrg PerfilPersonaOrg { get; set; }
        public ProyectoUsuarioIdentidad ProyectoUsuarioIdentidad { get; set; }

    }
    public class JoinPerfilPersonaIdentidadProyectoUsuarioIdentidadPerfil
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PerfilPersona PerfilPersona { get; set; }
        public ProyectoUsuarioIdentidad ProyectoUsuarioIdentidad { get; set; }
        public Perfil Perfil { get; set; }

    }
    public class JoinPerfilIdentidadProyectoUsuarioIdentidadPersona
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public ProyectoUsuarioIdentidad ProyectoUsuarioIdentidad { get; set; }
        public Persona Persona { get; set; }
    }
    public class JoinProyectoUsuarioIdentidadIdentidad
    {
        public ProyectoUsuarioIdentidad ProyectoUsuarioIdentidad { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }
    public class JoinPerfilIdentidadPersonaVinculoOrganizacion
    {
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PersonaVinculoOrganizacion PersonaVinculoOrganizacion { get; set; }
    }
    public class JoinPerfilIdentidadPersonaVinculoOrganizacionPersona
    {
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PersonaVinculoOrganizacion PersonaVinculoOrganizacion { get; set; }
        public Persona Persona { get; set; }
    }
    public class JoinIdentidadPerfilRedesSocialesProyectoUsuarioIdentidad
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PerfilRedesSociales PerfilRedesSociales { get; set; }
        public ProyectoUsuarioIdentidad ProyectoUsuarioIdentidad { get; set; }
    }
    public class JoinPerfilIdentidadIdentidad2
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad2 { get; set; }
    }
    public class JoinPerfilPersonaOrgIdentidadPerfil
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public PerfilPersonaOrg PerfilPersonaOrg { get; set; }
    }
    public class JoinPerfilIdentidadPerfilOrganizacion
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public PerfilOrganizacion PerfilOrganizacion { get; set; }
    }
    public class JoinPermisoGrupoAmigoOrgGrupoAmigos
    {
        public PermisoGrupoAmigoOrg PermisoGrupoAmigoOrg { get; set; }
        public GrupoAmigos GrupoAmigos { get; set; }
    }
    public class JoinPermisoGrupoAmigoOrgGrupoAmigosAmigoAgGrupo
    {
        public PermisoGrupoAmigoOrg PermisoGrupoAmigoOrg { get; set; }
        public GrupoAmigos GrupoAmigos { get; set; }
        public AmigoAgGrupo AmigoAgGrupo { get; set; }
    }
    public class JoinPermisoGrupoAmigoOrgGrupoAmigosAmigoAgGrupoIdentidad
    {
        public PermisoGrupoAmigoOrg PermisoGrupoAmigoOrg { get; set; }
        public GrupoAmigos GrupoAmigos { get; set; }
        public AmigoAgGrupo AmigoAgGrupo { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }
    public class JoinPerfilPersonaPerfilPersona
    {
        public Perfil Perfil { get; set; }
        public Persona Persona { get; set; }
        public PerfilPersona PerfilPersona { get; set; }
    }
    public class JoinPerfilPersonaOrgPerfilPersona
    {
        public Perfil Perfil { get; set; }
        public Persona Persona { get; set; }
        public PerfilPersonaOrg PerfilPersonaOrg { get; set; }
    }
    public class JoinPerfilPersonaPersona
    {
        public Persona Persona { get; set; }
        public PerfilPersona PerfilPersona { get; set; }
    }
    public class JoinPerfilPersonaOrgPersona
    {
        public Persona Persona { get; set; }
        public PerfilPersonaOrg PerfilPersonaOrg { get; set; }
    }
    public class JoinPerfilIdentidadPersonaPerfilPersonaOrg
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public Persona Persona { get; set; }
        public PerfilPersonaOrg PerfilPersonaOrg { get; set; }
    }
    public class JoinPerfilPersona
    {
        public Perfil Perfil { get; set; }
        public Persona Persona { get; set; }

    }
    public class JoinPerfilPersonaIdentidadbis
    {
        public Perfil Perfil { get; set; }
        public Persona Persona { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }

    }

    public class JoinPerfilPersonaOrgIdentidadPersona
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Persona Persona { get; set; }
        public PerfilPersonaOrg PerfilPersonaOrg { get; set; }
    }
    public class JoinGrupoIdentidadesParticipacionGrupoIdentidadesProyecto
    {
        public GrupoIdentidadesParticipacion GrupoIdentidadesParticipacion { get; set; }
        public GrupoIdentidadesProyecto GrupoIdentidadesProyecto { get; set; }
    }
    public class JoinGrupoIdentidadesParticipacionGrupoIdentidadesOrganizacion
    {
        public GrupoIdentidadesParticipacion GrupoIdentidadesParticipacion { get; set; }
        public GrupoIdentidadesOrganizacion GrupoIdentidadesOrganizacion { get; set; }
    }

    public class JoinGrupoIdentGrupoIdentidadesProyectoGrupoIdentidadesParticipacion
    {
        public GrupoIdentidades GrupoIdentidades { get; set; }
        public GrupoIdentidadesProyecto GrupoIdentidadesProyecto { get; set; }
        public GrupoIdentidadesParticipacion GrupoIdentidadesParticipacion { get; set; }

    }
    public class JoinGrupoIdentGrupoIdentidadesParticipacion
    {
        public GrupoIdentidades GrupoIdentidades { get; set; }
        public GrupoIdentidadesParticipacion GrupoIdentidadesParticipacion { get; set; }
    }
    public class JoinGrupoIdentidadesParticipacionIdentidadDocumentoRolGrupoIdentidades
    {
        public GrupoIdentidadesParticipacion GrupoIdentidadesParticipacion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoRolGrupoIdentidades DocumentoRolGrupoIdentidades { get; set; }
    }
    public class JoinGrupoIdentidadesParticipacionIdentidadDocumentoRolGrupoIdentidadesDocumentoWebVinBaseRecursos
    {
        public GrupoIdentidadesParticipacion GrupoIdentidadesParticipacion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoRolGrupoIdentidades DocumentoRolGrupoIdentidades { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }
    public class JoinGrupoIdentidadesParticipacionIdentidadDocumentoRolGrupoIdentidadesDocumentoWebVinBaseRecursosBaseRecursosProyecto
    {
        public GrupoIdentidadesParticipacion GrupoIdentidadesParticipacion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoRolGrupoIdentidades DocumentoRolGrupoIdentidades { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
    }
    public class JoinGrupoIdentidadesParticipacionIdentidadDocumentoRolGrupoIdentidadesDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumento
    {
        public GrupoIdentidadesParticipacion GrupoIdentidadesParticipacion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoRolGrupoIdentidades DocumentoRolGrupoIdentidades { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public Documento Documento { get; set; }
    }
    public class JoinGrupoIdentidadesParticipacionIdentidadDocumentoRolGrupoIdentidadesDocumentoWebVinBaseRecursosDocumento
    {
        public GrupoIdentidadesParticipacion GrupoIdentidadesParticipacion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoRolGrupoIdentidades DocumentoRolGrupoIdentidades { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public Documento Documento { get; set; }
    }
    public class JoinDocumentoDocumentoWebVinBaseRecursos
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }

    public class JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosOrganizacion
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosOrganizacion BaseRecursosOrganizacion { get; set; }
    }

    public class JoinDocumentoDocumento
    {
        public Documento Documento { get; set; }
        public Documento DocumentoVinc { get; set; }
    }

    public class JoinDocumentoDocumentoWebVinBaseRecursosDocumentoRolIdentidad
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public DocumentoRolIdentidad DocumentoRolIdentidad { get; set; }
    }

    public class JoinDocumentoDocumentoWebVinBaseRecursosDocumentoRolIdentidadIdentidad
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public DocumentoRolIdentidad DocumentoRolIdentidad { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosUsuario
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
    }

    public class JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosUsuarioIdentidad
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosUsuarioIdentidadPerfil
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinDocumentoDocumentoWebVinBaseRecursosIdentidad
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinDocumentoDocumentoWebVinBaseRecursosIdentidadPerfil
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinDocumentoDocumentoWebVinBaseRecursosIdentidadPerfilPerfil
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public Perfil PerfilPersona { get; set; }
    }

    public class JoinDocumentoDocumentoWebVinBaseRecursosIdentidadPerfilIdentidad
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentidadMyGnoss { get; set; }
    }

    public class JoinDocumentoDocumentoWebVinBaseRecursosIdentidadPerfilConfiguracionGnossPersona
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public ConfiguracionGnossPersona ConfiguracionGnossPersona { get; set; }
    }

    public class JoinDocumentoDocumentoWebVinBaseRecursosIdentidadPerfilConfiguracionGnossPersonaBaseRecursoProyecto
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public ConfiguracionGnossPersona ConfiguracionGnossPersona { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
    }

    public class JoinDocumentoDocumentoWebVinBaseRecursosIdentidadPerfilConfiguracionGnossPersonaBaseRecursoProyectoProyecto
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public ConfiguracionGnossPersona ConfiguracionGnossPersona { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public Proyecto Proyecto { get; set; }
    }

    public class JoinGrupoIdentGrupoIdentOrganizacionGrupoIdentidadesParticipacion
    {
        public GrupoIdentidades GrupoIdentidades { get; set; }
        public GrupoIdentidadesOrganizacion GrupoIdentidadesOrganizacion { get; set; }
        public GrupoIdentidadesParticipacion GrupoIdentidadesParticipacion { get; set; }
    }
    public class JoinGrupoIdentGrupoIdentOrganizacionGrupoIdentidadesParticipacionIdentidad
    {
        public GrupoIdentidades GrupoIdentidades { get; set; }
        public GrupoIdentidadesOrganizacion GrupoIdentidadesOrganizacion { get; set; }
        public GrupoIdentidadesParticipacion GrupoIdentidadesParticipacion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }
    public class JoinGrupoIdentGrupoIdentOrganizacionGrupoIdentidadesParticipacionIdentidadPerfil
    {
        public GrupoIdentidades GrupoIdentidades { get; set; }
        public GrupoIdentidadesOrganizacion GrupoIdentidadesOrganizacion { get; set; }
        public GrupoIdentidadesParticipacion GrupoIdentidadesParticipacion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }
    public class JoinGrupoIdentGrupoIdentOrganizacionGrupoIdentidadesParticipacionIdentidadPerfilIdentidad
    {
        public GrupoIdentidades GrupoIdentidades { get; set; }
        public GrupoIdentidadesOrganizacion GrupoIdentidadesOrganizacion { get; set; }
        public GrupoIdentidadesParticipacion GrupoIdentidadesParticipacion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad2 { get; set; }
    }
    public class JoinGrupoIdentidadesParticipacionIdentidadIdentidad
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad2 { get; set; }
        public GrupoIdentidadesParticipacion GrupoIdentidadesParticipacion { get; set; }
    }
    public class JoinPerfilIdentidadPersonaVisibleEnOrg
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public PersonaVisibleEnOrg PersonaVisibleEnOrg { get; set; }
    }
    public class JoinPerfilPersonaOrgIdentidadPersonaVisibleEnOrg
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PerfilPersonaOrg PerfilPersonaOrg { get; set; }
        public PersonaVisibleEnOrg PersonaVisibleEnOrg { get; set; }
    }
    public class JoinPerfilIdentidadPersonaPerfilPersonaProfesor
    {
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PerfilPersona PerfilPersona { get; set; }
        public Profesor Profesor { get; set; }
    }
    public class JoinPerfilPersonaOrgIdentidadPersonaUsuario
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Persona Persona { get; set; }
        public PerfilPersonaOrg PerfilPersonaOrg { get; set; }
        public Usuario Usuario { get; set; }
    }
    public class JoinPerfilOrganizacionIdentidadOrganizacion
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public PerfilOrganizacion PerfilOrganizacion { get; set; }
        public Organizacion Organizacion { get; set; }
    }

    public class JoinIdentidadIdentidad
    {
        public EntityModel.Models.IdentidadDS.Identidad IdentidadProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentidadPerfil { get; set; }
    }
    public class JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyecto
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
    }

    public class JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoNivelCertificacion
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public NivelCertificacion NivelCertificacion { get; set; }
    }

    public class JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidad
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoProyecto
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public Proyecto Proyecto { get; set; }
    }

    public class JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidadPerfil
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidadPerfilProyecto
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public Proyecto Proyecto { get; set; }
    }

    public class JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidadPerfilDocumentoRolIdentidad
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public DocumentoRolIdentidad DocumentoRolIdentidad { get; set; }
    }

    public class JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidadPerfilDocumentoRolIdentidadIdentidad
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public DocumentoRolIdentidad DocumentoRolIdentidad { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentidadEditor { get; set; }
    }

    public class JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidadPerfilDocumentoRolIdentidadIdentidadPerfil
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public DocumentoRolIdentidad DocumentoRolIdentidad { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentidadEditor { get; set; }
        public Perfil PerfilEditor { get; set; }

    }


    public class JoinDocumentoWebVinBaseRecursosIdentidad
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }
    public class JoinDocumentoWebVinBaseRecursosIdentidadBaseRecursosProyecto
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
    }

    public class JoinComentarioIdentidad
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinComentarioIdentidadDocumentoComentario
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
    }

    public class JoinComentarioIdentidadDocumentoComentarioDocumentoWebVinBaseRecursos
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }

    public class JoinComentarioIdentidadDocumentoComentarioDocumentoWebVinBaseRecursosBaseRecursosProyecto
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
    }

    public class JoinComentarioIdentidadDocumentoComentarioDocumento
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinComentarioIdentidadDocumentoComentarioDocumentoDocumentoWebVinBaseRecursos
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }

    public class JoinComentarioIdentidadDocumentoComentarioDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyecto
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
    }

    public class JoinComentarioIdentidadPerfil
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinComentarioIdentidadPerfilIdentidadMyGnoss
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentidadMyGnoss { get; set; }
    }

    public class JoinComentarioIdentidadPerfilVotoComentario
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.Comentario.VotoComentario VotoComentario { get; set; }
    }

    public class JoinComentarioIdentidadPerfilDocumentoComentario
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
    }

    public class JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoWebVinBaseRecuros
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }

    public class JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoWebVinBaseRecurosBaseRecursosProyecto
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
    }

    public class JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoWebVinBaseRecurosBaseRecursosProyectoDocumento
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinPerfilIdentidadPerfil2
    {
        public Perfil Perfil { get; set; }
        public Perfil Perfil2 { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }
    public class JoinIdentidadSuscripcionIdentidad2Identidad3SuscripcionIdentidadProyecto
    {
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad2 { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad3 { get; set; }
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
    }
    public class JoinSuscripcionIdentidadProyectoSuscripcionIdentidadIdentidad2
    {
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad2 { get; set; }
    }

    public class JoinSuscripcionIdentidadProyectoSuscripcionIdentidadIdentidadEnProyecto
    {
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentidadEnProyecto { get; set; }
    }

    public class JoinSuscripcionIdentidadProyectoSuscripcionIdentidadIdentidad2Identidad3
    {
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad2 { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad3 { get; set; }
    }

    public class JoinIdentidadProyectoPerfil
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Proyecto Proyecto { get; set; }
        public Perfil Perfil { get; set; }
        public Persona Persona { get; set; }
    }

    public class JoinIdentidadProyectoPerfilPersona
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Proyecto Proyecto { get; set; }
        public Perfil Perfil { get; set; }
        public Persona Persona { get; set; }
    }

    public class JoinIdentidadProyectoPerfilIdentidad
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Proyecto Proyecto { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentidadNoMyGnoss { get; set; }
    }

    public class JoinIdentidadProyectoPerfilIdentidadOrgComunidades
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Proyecto Proyecto { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentidadOrgComunidades { get; set; }
    }

    public class JoinIdentidadProyectoPerfilIdentidadProyecto
    {
        public EntityModel.Models.IdentidadDS.Identidad Idenidad { get; set; }
        public Proyecto Proyecto { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentidadNoMyGnoss { get; set; }
        public Proyecto ProyectoNoMyGnoss { get; set; }
    }

    public class JoinTesauroUsuarioPersona
    {
        public EntityModel.Models.Tesauro.TesauroUsuario TesauroUsuario { get; set; }
        public Persona Persona { get; set; }
    }

    public class JoinTesauroUsuarioPersonaPerfil
    {
        public EntityModel.Models.Tesauro.TesauroUsuario TesauroUsuario { get; set; }
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinTesauroUsuarioPersonaPerfilIdentidad
    {
        public EntityModel.Models.Tesauro.TesauroUsuario TesauroUsuario { get; set; }
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinTesauroUsuarioPersonaPerfilIdentidadCategoriaTesauro
    {
        public EntityModel.Models.Tesauro.TesauroUsuario TesauroUsuario { get; set; }
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public EntityModel.Models.Tesauro.CategoriaTesauro CategoriaTesauro { get; set; }
    }

    public class JoinIdentidadPerfilPersonaPersonaVinculoOrganizacion
    {
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Persona Persona { get; set; }
        public PersonaVinculoOrganizacion PersonaVinculoOrganizacion { get; set; }
    }

    public class JoinIdentidadPerfilPersonaPersonaVinculoOrganizacionPersonaVisibleEnOrg
    {
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Persona Persona { get; set; }
        public PersonaVinculoOrganizacion PersonaVinculoOrganizacion { get; set; }
        public PersonaVisibleEnOrg PersonaVisibleEnOrg { get; set; }
    }

    public class JoinIdentidadPerfilUsuario
    {
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Usuario Usuario { get; set; }
    }

    public class JoinPersonaPerfil
    {
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinPersonaPerfilIdentidad
    {
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinPersonaPerfilBaseRecursosUsuario
    {
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
    }

    public class JoinPersonaPerfilBaseRecursosUsuarioDocumentoWebVinBaseRecursos
    {
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }

    public class JoinPersonaPerfilBaseRecursosUsuarioDocumentoWebVinBaseRecursosDocumento
    {
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinPersonaPerfilIdentidadAmigo
    {
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Amigo Amigo { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosIdentidadDocumento
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinDocumentoComentarioDocumento
    {
        public DocumentoComentario DocumentoComentario { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinDocumentoComentarioDocumentoComentario
    {
        public DocumentoComentario DocumentoComentario { get; set; }
        public Documento Documento { get; set; }
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
    }

    public class JoinDocumentoComentarioDocumentoComentarioIdentidad
    {
        public DocumentoComentario DocumentoComentario { get; set; }
        public Documento Documento { get; set; }
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identiad { get; set; }
    }

    public class JoinDocumentoVincDocDocumento
    {
        public DocumentoVincDoc DocumentoVincDoc { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinDocumentoVincDocDocumentoIdentidad
    {
        public DocumentoVincDoc DocumentoVincDoc { get; set; }
        public Documento Documento { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }
    public class JoinIdentidadPerfilOrganizacion
    {
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Organizacion Organizacion { get; set; }
    }

    public class JoinGrupoIdentGrupoIdentidadesParticipacionIdentidad
    {
        public GrupoIdentidades GrupoIdentidades { get; set; }
        public GrupoIdentidadesParticipacion GrupoIdentidadesParticipacion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinGrupoIdentGrupoIdentidadesParticipacionIdentidadProyecto
    {
        public GrupoIdentidades GrupoIdentidades { get; set; }
        public GrupoIdentidadesParticipacion GrupoIdentidadesParticipacion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Proyecto Proyecto { get; set; }
    }

    public class JoinGrupoIdentGrupoIdentidadesProyectoIdentidad
    {
        public GrupoIdentidades GrupoIdentidades { get; set; }
        public GrupoIdentidadesProyecto GrupoIdentidadesProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinGrupoIdentGrupoIdentidadesProyectoIdentidadProyecto
    {
        public GrupoIdentidades GrupoIdentidades { get; set; }
        public GrupoIdentidadesProyecto GrupoIdentidadesProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Proyecto Proyecto { get; set; }
    }

    public class JoinGrupoIdentGrupoIdentidadesProyectoIdentidadPerfil
    {
        public GrupoIdentidades GrupoIdentidades { get; set; }
        public GrupoIdentidadesProyecto GrupoIdentidadesProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinGrupoIdentGrupoIdentidadesProyectoIdentidadPerfilPersona
    {
        public GrupoIdentidades GrupoIdentidades { get; set; }
        public GrupoIdentidadesProyecto GrupoIdentidadesProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public Persona Persona { get; set; }
    }

    public class JoinGrupoIdentGrupoIdentidadesProyectoIdentidadPerfilPersonaUsuario
    {
        public GrupoIdentidades GrupoIdentidades { get; set; }
        public GrupoIdentidadesProyecto GrupoIdentidadesProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public Persona Persona { get; set; }
        public Usuario Usuario { get; set; }
    }

    public class JoinGrupoIdentGrupoIdentidadesProyectoIdentidadPerfilPersonaUsuarioAdministradorProyecto
    {
        public GrupoIdentidades GrupoIdentidades { get; set; }
        public GrupoIdentidadesProyecto GrupoIdentidadesProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public Persona Persona { get; set; }
        public Usuario Usuario { get; set; }
        public AdministradorProyecto AdministradorProyecto { get; set; }
    }

    public class JoinGrupoIdentGrupoIdentidadesProyectoIdentidadPerfilPersonaUsuarioAdministradorProyectoProyecto
    {
        public GrupoIdentidades GrupoIdentidades { get; set; }
        public GrupoIdentidadesProyecto GrupoIdentidadesProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public Persona Persona { get; set; }
        public Usuario Usuario { get; set; }
        public AdministradorProyecto AdministradorProyecto { get; set; }
        public Proyecto Proyecto { get; set; }
    }

    public class JoinGrupoIdentGrupoIdentidadesProyectoIdentidadPerfilPersonaUsuarioAdministradorProyectoProyectoParametroGeneral
    {
        public GrupoIdentidades GrupoIdentidades { get; set; }
        public GrupoIdentidadesProyecto GrupoIdentidadesProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public Persona Persona { get; set; }
        public Usuario Usuario { get; set; }
        public AdministradorProyecto AdministradorProyecto { get; set; }
        public Proyecto Proyecto { get; set; }
        public ParametroGeneral ParametroGeneral { get; set; }
    }

    public class JoinUsuarioPersona
    {
        public Usuario Usuario { get; set; }
        public Persona Persona { get; set; }
    }

    public class JoinUsuarioPersonaPerfil
    {
        public Usuario Usuario { get; set; }
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinUsuarioPersonaPerfilIdentidad
    {
        public Usuario Usuario { get; set; }
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinUsuarioPersonaPerfilIdentidadNotificacionCorreoPersona
    {
        public Usuario Usuario { get; set; }
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public NotificacionCorreoPersona NotificacionCorreoPersona { get; set; }
    }

    public class JoinUsuarioPersonaPerfilIdentidadNotificacionCorreoPersonaNotificacion
    {
        public Usuario Usuario { get; set; }
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public NotificacionCorreoPersona NotificacionCorreoPersona { get; set; }
        public EntityModel.Models.Notificacion.Notificacion Notificacion { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursos
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosBaseRecursosUsuario
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public Documento Documento { get; set; }
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosBaseRecursosUsuarioIdentidad
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public Documento Documento { get; set; }
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosBaseRecursosUsuarioIdentidadPerfil
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public Documento Documento { get; set; }
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil PerfilPublicador { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosBaseRecursosUsuarioIdentidadPerfilPerfil
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public Documento Documento { get; set; }
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil PerfilPublicador { get; set; }
        public Perfil TodosPerfiles { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosBaseRecursosProyecto
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public Documento Documento { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidad
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public Documento Documento { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidadPerfil
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public Documento Documento { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinDatoExtraProyectoDatoExtraProyectoOpcion
    {
        public DatoExtraProyecto DatoExtraProyecto { get; set; }
        public DatoExtraProyectoOpcion DatoExtraProyectoOpcion { get; set; }
    }

    public class JoinDatoExtraProyectoDatoExtraProyectoOpcionDatoExtraProyectoOpcionIdentidad
    {
        public DatoExtraProyecto DatoExtraProyecto { get; set; }
        public DatoExtraProyectoOpcion DatoExtraProyectoOpcion { get; set; }
        public DatoExtraProyectoOpcionIdentidad DatoExtraProyectoOpcionIdentidad { get; set; }
    }

    public class JoinDatoExtraProyectoVirtuosoIdentidadDatoExtraProyectoVirtuoso
    {
        public DatoExtraProyectoVirtuosoIdentidad DatoExtraProyectoVirtuosoIdentidad { get; set; }
        public DatoExtraProyectoVirtuoso DatoExtraProyectoVirtuoso { get; set; }
    }

    public class JoinDatoExtraEcosistemaDatoExtraEcosistemaOpcion
    {
        public DatoExtraEcosistema DatoExtraEcosistema { get; set; }
        public DatoExtraEcosistemaOpcion DatoExtraEcosistemaOpcion { get; set; }
    }

    public class JoinDatoExtraEcosistemaDatoExtraEcosistemaOpcionDatoExtraEcosistemaOpcionPerfil
    {
        public DatoExtraEcosistema DatoExtraEcosistema { get; set; }
        public DatoExtraEcosistemaOpcion DatoExtraEcosistemaOpcion { get; set; }
        public DatoExtraEcosistemaOpcionPerfil DatoExtraEcosistemaOpcionPerfil { get; set; }
    }

    public class JoinDatoExtraEcosistemaDatoExtraEcosistemaOpcionDatoExtraEcosistemaOpcionPerfilIdentidad
    {
        public DatoExtraEcosistema DatoExtraEcosistema { get; set; }
        public DatoExtraEcosistemaOpcion DatoExtraEcosistemaOpcion { get; set; }
        public DatoExtraEcosistemaOpcionPerfil DatoExtraEcosistemaOpcionPerfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinDatoExtraEcosistemaVirtuosoPerfilDatoExtraEcosistemaVirtuoso
    {
        public DatoExtraEcosistemaVirtuosoPerfil DatoExtraEcosistemaVirtuosoPerfil { get; set; }
        public DatoExtraEcosistemaVirtuoso DatoExtraEcosistemaVirtuoso { get; set; }
    }

    public class JoinDatoExtraEcosistemaVirtuosoPerfilDatoExtraEcosistemaVirtuosoIdentidad
    {
        public DatoExtraEcosistemaVirtuosoPerfil DatoExtraEcosistemaVirtuosoPerfil { get; set; }
        public DatoExtraEcosistemaVirtuoso DatoExtraEcosistemaVirtuoso { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinProyectoProyectoUsuarioIdentidad
    {
        public Proyecto Proyecto { get; set; }
        public ProyectoUsuarioIdentidad ProyectoUsuarioIdentiad { get; set; }
    }

    public class JoinProyectoProyectoUsuarioIdentidadIdentidad
    {
        public Proyecto Proyecto { get; set; }
        public ProyectoUsuarioIdentidad ProyectoUsuarioIdentiad { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinBlogIdentidad
    {
        public Blog Blog { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinEntradaBlogIdentidad
    {
        public EntradaBlog EntradaBlog { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinSuscripcionSuscripcionBlog
    {
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public SuscripcionBlog SuscripcionBlog { get; set; }
    }

    public class JoinSuscripcionSuscripcionTesauroProyecto
    {
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public SuscripcionTesauroProyecto SuscripcionTesauroProyecto { get; set; }
    }

    public class JoinSuscripcionSuscripcionTesauroProyectoCategoriaTesVinSuscrip
    {
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public SuscripcionTesauroProyecto SuscripcionTesauroProyecto { get; set; }
        public CategoriaTesVinSuscrip CategoriaTesVinSuscrip { get; set; }
    }

    public class JoinSuscripcionSuscripcionBlogIdentidad
    {
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public SuscripcionBlog SuscripcionBlog { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinIdentidadDocumentoWebVinBaseRecursos
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }

    public class JoinIdentidadIdentidadPerfil
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentidadPerfil { get; set; }
    }

    public class JoinIdentidadIdentidadPerfilPerfil
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentidadPerfil { get; set; }
        public EntityModel.Models.IdentidadDS.Perfil Perfil { get; set; }
    }

    public class JoinInvitacionIdentidad
    {
        public Invitacion Invitacion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinInvitacionIdentidadPerfil
    {
        public Invitacion Invitacion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinIdentidadDocumentoRolIdentidad
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoRolIdentidad DocumentoRolIdentidad { get; set; }
    }

    public class JoinIdentidadDocumentoRolIdentidadDocumento
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoRolIdentidad DocumentoRolIdentidad { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinIdentidadDocumentoRolIdentidadDocumentoPerfil
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoRolIdentidad DocumentoRolIdentidad { get; set; }
        public Documento Documento { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinIdentidadDocumentoRolIdentidadDocumentoPerfilIdentidad
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoRolIdentidad DocumentoRolIdentidad { get; set; }
        public Documento Documento { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentidadMyGnoss { get; set; }
    }

    public class JoinIdentidadDocumentoRolIdentidadDocumentoPerfilProyecto
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoRolIdentidad DocumentoRolIdentidad { get; set; }
        public Documento Documento { get; set; }
        public Perfil Perfil { get; set; }
        public Proyecto Proyecto { get; set; }
    }

    public class JoinIdentidadDocumentoRolIdentidadDocumentoPerfilDocumentoWebVinBaseRecursos
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoRolIdentidad DocumentoRolIdentidad { get; set; }
        public Documento Documento { get; set; }
        public Perfil Perfil { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }

    public class JoinIdentidadDocumentoRolIdentidadDocumentoPerfilDocumentoWebVinBaseRecursosBaseRecursosUsuario
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoRolIdentidad DocumentoRolIdentidad { get; set; }
        public Documento Documento { get; set; }
        public Perfil Perfil { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
    }

    public class JoinIdentidadDocumentoRolIdentidadDocumentoPerfilDocumentoWebVinBaseRecursosBaseRecursosOrganizacion
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoRolIdentidad DocumentoRolIdentidad { get; set; }
        public Documento Documento { get; set; }
        public Perfil Perfil { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosOrganizacion BaseRecursosOrganizacion { get; set; }
    }

    public class JoinIdentidadGrupoIdentidadesParticipacion
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public GrupoIdentidadesParticipacion GrupoIdentidadesParticipacion { get; set; }
    }

    public class JoinIdentidadDocumentoWebVinBaseRecursosDocumeto
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinIdentidadPerfilPerfilesMiembro
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public Perfil PerfilesMiembro { get; set; }
    }

    public class JoinIdentidadPerfilPerfilesMiembroIdentidadesMiembro
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public Perfil PerfilesMiembro { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentidadesMiembro { get; set; }
    }

    public class JoinIdentidadPerfilPerfilesMiembroIdentidadesMiembroDocumentoWebVinBaseRecursos
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public Perfil PerfilesMiembro { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentidadesMiembro { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }

    public class JoinIdentidadPerfilAmigo
    {
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Amigo Amigo { get; set; }
    }

    public class JoinIdentidadPerfilAmigoPermisoAmigoOrg
    {
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Amigo Amigo { get; set; }
        public PermisoAmigoOrg PermisoAmigoOrg { get; set; }
    }

    public class JoinIdentidadPerfilAmigoAmigoAgGrupo
    {
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Amigo Amigo { get; set; }
        public AmigoAgGrupo AmigoAgGrupo { get; set; }
    }

    public class JoinIdentidadPerfilAmigoAmigoAgGrupoGrupoAmigos
    {
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Amigo Amigo { get; set; }
        public AmigoAgGrupo AmigoAgGrupo { get; set; }
        public GrupoAmigos GrupoAmigos { get; set; }
    }

    public class JoinIdentidadPerfilAmigoAmigoAgGrupoGrupoAmigosPermisoGrupoAmigoOrg
    {
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Amigo Amigo { get; set; }
        public AmigoAgGrupo AmigoAgGrupo { get; set; }
        public GrupoAmigos GrupoAmigos { get; set; }
        public PermisoGrupoAmigoOrg PermisoGrupoAmigoOrg { get; set; }
    }

    public class JoinGrupoAmigosPermisoGrupoAmigoOrg
    {
        public GrupoAmigos GrupoAmigos { get; set; }
        public PermisoGrupoAmigoOrg PermisoGrupoAmigosOrg { get; set; }
    }

    public class JoinIdentidadPerfilPermisoAmigoOrg
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public PermisoAmigoOrg PermisoAmigoOrg { get; set; }
    }

    public class JoinIdentidadPerfilPermisoGrupoAmigoOrg
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public PermisoGrupoAmigoOrg PermisoGrupoAmigoOrg { get; set; }
    }

    public class JoinIdentidadPerfilPermisoGrupoAmigoOrgGrupoAmigos
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public PermisoGrupoAmigoOrg PermisoGrupoAmigoOrg { get; set; }
        public GrupoAmigos GrupoAmigos { get; set; }
    }

    public class JoinIdentidadPerfilPermisoGrupoAmigoOrgGrupoAmigosAmigoAgGrupo
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public PermisoGrupoAmigoOrg PermisoGrupoAmigoOrg { get; set; }
        public GrupoAmigos GrupoAmigos { get; set; }
        public AmigoAgGrupo AmigoAgGrupo { get; set; }
    }

    public class JoinGrupoAmigosIdentidad
    {
        public GrupoAmigos GrupoAmigos { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinGrupoAmigosIdentidadPerfil
    {
        public GrupoAmigos GrupoAmigos { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinGrupoAmigosIdentidadPerfilAdministradorOrganizacion
    {
        public GrupoAmigos GrupoAmigos { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public AdministradorOrganizacion AdministradorOrganizacion { get; set; }
    }

    public class JoinGrupoAmigosIdentidadPerfilAdministradorOrganizacionPersona
    {
        public GrupoAmigos GrupoAmigos { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public AdministradorOrganizacion AdministradorOrganizacion { get; set; }
        public Persona Persona { get; set; }
    }

    public class JoinGrupoAmigosIdentidadPerfilAdministradorOrganizacionPersonaPerfil
    {
        public GrupoAmigos GrupoAmigos { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public AdministradorOrganizacion AdministradorOrganizacion { get; set; }
        public Persona Persona { get; set; }
        public Perfil P2 { get; set; }
    }

    public class JoinGrupoAmigosIdentidadPerfilAdministradorOrganizacionPersonaPerfilIdentidad
    {
        public GrupoAmigos GrupoAmigos { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public AdministradorOrganizacion AdministradorOrganizacion { get; set; }
        public Persona Persona { get; set; }
        public Perfil P2 { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad I2 { get; set; }
    }

    public class JoinAmigoIdentidadPerfil
    {
        public Amigo Amigo { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinAmigoIdentidadPerfilIdentidad
    {
        public Amigo Amigo { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentOrg { get; set; }
    }

    public class JoinAmigoIdentidadPerfilIdentidadPerfil
    {
        public Amigo Amigo { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentOrg { get; set; }
        public Perfil PerfOrg { get; set; }
    }

    public class JoinAmigoIdentidadPerfilIdentidadPerfilAdministradorOrganizacion
    {
        public Amigo Amigo { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentOrg { get; set; }
        public Perfil PerfOrg { get; set; }
        public AdministradorOrganizacion AdministradorOrganizacion { get; set; }
    }

    public class JoinAmigoIdentidadPerfilIdentidadPerfilAdministradorOrganizacionPersona
    {
        public Amigo Amigo { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentOrg { get; set; }
        public Perfil PerfOrg { get; set; }
        public AdministradorOrganizacion AdministradorOrganizacion { get; set; }
        public Persona Persona { get; set; }
    }

    public class JoinAmigoIdentidadPerfilIdentidadPerfilAdministradorOrganizacionPersonaPerfil
    {
        public Amigo Amigo { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentOrg { get; set; }
        public Perfil PerfOrg { get; set; }
        public AdministradorOrganizacion AdministradorOrganizacion { get; set; }
        public Persona Persona { get; set; }
        public Perfil PerfAdmin { get; set; }
    }

    public class JoinAmigoIdentidadPerfilIdentidadPerfilAdministradorOrganizacionPersonaPerfilIdentidad
    {
        public Amigo Amigo { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentOrg { get; set; }
        public Perfil PerfOrg { get; set; }
        public AdministradorOrganizacion AdministradorOrganizacion { get; set; }
        public Persona Persona { get; set; }
        public Perfil PerfAdmin { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentAdmin { get; set; }
    }

    public class JoinIdentidadSuscripcionPerfil
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinDocumentoComentarioBaseRecursosProyecto
    {
        public DocumentoComentario DocumentoComentario { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
    }

    public class JoinDocumentoComentarioBaseRecursosProyectoDocumentoWebVinBaseRecursos
    {
        public DocumentoComentario DocumentoComentario { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }

    public class JoinDocumentoComentarioBaseRecursosProyectoDocumentoWebVinBaseRecursosComentario
    {
        public DocumentoComentario DocumentoComentario { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
    }

    public static class Joins
    {
        public static IQueryable<JoinDocumentoComentarioBaseRecursosProyectoDocumentoWebVinBaseRecursosComentario> JoinComentario(this IQueryable<JoinDocumentoComentarioBaseRecursosProyectoDocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Comentario, item => item.DocumentoComentario.ComentarioID, comentario => comentario.ComentarioID, (item, comentario) => new JoinDocumentoComentarioBaseRecursosProyectoDocumentoWebVinBaseRecursosComentario
            {
                Comentario = comentario,
                BaseRecursosProyecto = item.BaseRecursosProyecto,
                DocumentoComentario = item.DocumentoComentario,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinDocumentoComentarioBaseRecursosProyectoDocumentoWebVinBaseRecursos> JoinDocumentoWebVinBaseRecursos(this IQueryable<JoinDocumentoComentarioBaseRecursosProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoWebVinBaseRecursos, item => new { item.BaseRecursosProyecto.BaseRecursosID, item.DocumentoComentario.DocumentoID }, documentoWebVinBaseRecusos => new { documentoWebVinBaseRecusos.BaseRecursosID, documentoWebVinBaseRecusos.DocumentoID }, (item, documentoWebVinBaseRecursos) => new JoinDocumentoComentarioBaseRecursosProyectoDocumentoWebVinBaseRecursos
            {
                BaseRecursosProyecto = item.BaseRecursosProyecto,
                DocumentoComentario = item.DocumentoComentario,
                DocumentoWebVinBaseRecursos = documentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinDocumentoComentarioBaseRecursosProyecto> JoinBaseRecursosProyecto(this IQueryable<DocumentoComentario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.BaseRecursosProyecto, documentoComentario => documentoComentario.ProyectoID, baseRecursosProyecto => baseRecursosProyecto.ProyectoID, (documentoComentario, baseRecursosProyecto) => new JoinDocumentoComentarioBaseRecursosProyecto
            {
                BaseRecursosProyecto = baseRecursosProyecto,
                DocumentoComentario = documentoComentario
            });
        }

        public static IQueryable<JoinIdentidadSuscripcionPerfil> JoinPerfil(this IQueryable<JoinIdentidadSuscripcion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new JoinIdentidadSuscripcionPerfil
            {
                Perfil = perfil,
                Identidad = item.Identidad,
                Suscripcion = item.Suscripcion
            });
        }

        public static IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosDocumentoRolIdentidadIdentidad> JoinIdentidad(this IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosDocumentoRolIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.DocumentoRolIdentidad.PerfilID, identidad => identidad.PerfilID, (item, identidad) => new JoinDocumentoDocumentoWebVinBaseRecursosDocumentoRolIdentidadIdentidad
            {
                Documento = item.Documento,
                DocumentoRolIdentidad = item.DocumentoRolIdentidad,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosDocumentoRolIdentidad> JoinDocumentoRolIdentidad(this IQueryable<JoinDocumentoDocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoRolIdentidad, item => item.DocumentoWebVinBaseRecursos.DocumentoID, documentoRolIdentidad => documentoRolIdentidad.DocumentoID, (item, documentoRolIdentidad) => new JoinDocumentoDocumentoWebVinBaseRecursosDocumentoRolIdentidad
            {
                DocumentoRolIdentidad = documentoRolIdentidad,
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinIdentidadDocumentoWebVinBaseRecursosDocumeto> JoinDocumento(this IQueryable<JoinIdentidadDocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, item => item.DocumentoWebVinBaseRecursos.DocumentoID, documento => documento.DocumentoID, (item, documento) => new JoinIdentidadDocumentoWebVinBaseRecursosDocumeto
            {
                Documento = documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                Identidad = item.Identidad
            });
        }

        public static IQueryable<JoinAmigoIdentidadPerfilIdentidadPerfilAdministradorOrganizacionPersonaPerfilIdentidad> JoinIdentidad(this IQueryable<JoinAmigoIdentidadPerfilIdentidadPerfilAdministradorOrganizacionPersonaPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.PerfAdmin.PerfilID, identidad => identidad.PerfilID, (item, identidad) => new JoinAmigoIdentidadPerfilIdentidadPerfilAdministradorOrganizacionPersonaPerfilIdentidad
            {
                Amigo = item.Amigo,
                Perfil = item.Perfil,
                Identidad = item.Identidad,
                IdentOrg = item.IdentOrg,
                PerfOrg = item.PerfOrg,
                AdministradorOrganizacion = item.AdministradorOrganizacion,
                Persona = item.Persona,
                PerfAdmin = item.PerfAdmin,
                IdentAdmin = identidad
            });
        }

        public static IQueryable<JoinAmigoIdentidadPerfilIdentidadPerfilAdministradorOrganizacionPersonaPerfil> JoinPerfil(this IQueryable<JoinAmigoIdentidadPerfilIdentidadPerfilAdministradorOrganizacionPersona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Persona.PersonaID, perfil => perfil.PersonaID, (item, perfil) => new JoinAmigoIdentidadPerfilIdentidadPerfilAdministradorOrganizacionPersonaPerfil
            {
                Amigo = item.Amigo,
                Perfil = item.Perfil,
                Identidad = item.Identidad,
                IdentOrg = item.IdentOrg,
                PerfOrg = item.PerfOrg,
                AdministradorOrganizacion = item.AdministradorOrganizacion,
                Persona = item.Persona,
                PerfAdmin = perfil
            });
        }

        public static IQueryable<JoinAmigoIdentidadPerfilIdentidadPerfilAdministradorOrganizacionPersona> JoinPersona(this IQueryable<JoinAmigoIdentidadPerfilIdentidadPerfilAdministradorOrganizacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Persona, item => item.AdministradorOrganizacion.UsuarioID, persona => persona.UsuarioID, (item, persona) => new JoinAmigoIdentidadPerfilIdentidadPerfilAdministradorOrganizacionPersona
            {
                Amigo = item.Amigo,
                Perfil = item.Perfil,
                Identidad = item.Identidad,
                IdentOrg = item.IdentOrg,
                PerfOrg = item.PerfOrg,
                AdministradorOrganizacion = item.AdministradorOrganizacion,
                Persona = persona
            });
        }

        public static IQueryable<JoinAmigoIdentidadPerfilIdentidadPerfilAdministradorOrganizacion> JoinAdministradorOrganizacion(this IQueryable<JoinAmigoIdentidadPerfilIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.AdministradorOrganizacion, item => item.PerfOrg.OrganizacionID, administradorOrganizacion => administradorOrganizacion.OrganizacionID, (item, administradorOrganizacion) => new JoinAmigoIdentidadPerfilIdentidadPerfilAdministradorOrganizacion
            {
                Amigo = item.Amigo,
                Perfil = item.Perfil,
                Identidad = item.Identidad,
                IdentOrg = item.IdentOrg,
                PerfOrg = item.PerfOrg,
                AdministradorOrganizacion = administradorOrganizacion
            });
        }

        public static IQueryable<JoinAmigoIdentidadPerfilIdentidadPerfil> JoinPerfil(this IQueryable<JoinAmigoIdentidadPerfilIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.IdentOrg.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new JoinAmigoIdentidadPerfilIdentidadPerfil
            {
                Amigo = item.Amigo,
                Perfil = item.Perfil,
                Identidad = item.Identidad,
                IdentOrg = item.IdentOrg,
                PerfOrg = perfil
            });
        }

        public static IQueryable<JoinAmigoIdentidadPerfilIdentidad> JoinIdentidad(this IQueryable<JoinAmigoIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Amigo.IdentidadID, identidad => identidad.IdentidadID, (item, identidad) => new JoinAmigoIdentidadPerfilIdentidad
            {
                Amigo = item.Amigo,
                Perfil = item.Perfil,
                Identidad = item.Identidad,
                IdentOrg = identidad
            });
        }

        public static IQueryable<JoinAmigoIdentidadPerfil> JoinPerfil(this IQueryable<JoinAmigoIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new JoinAmigoIdentidadPerfil
            {
                Amigo = item.Amigo,
                Perfil = perfil,
                Identidad = item.Identidad
            });
        }

        public static IQueryable<JoinGrupoAmigosIdentidadPerfilAdministradorOrganizacionPersonaPerfilIdentidad> JoinIdentidad(this IQueryable<JoinGrupoAmigosIdentidadPerfilAdministradorOrganizacionPersonaPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.P2.PerfilID, identidad => identidad.PerfilID, (item, identidad) => new JoinGrupoAmigosIdentidadPerfilAdministradorOrganizacionPersonaPerfilIdentidad
            {
                AdministradorOrganizacion = item.AdministradorOrganizacion,
                GrupoAmigos = item.GrupoAmigos,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Persona = item.Persona,
                P2 = item.P2,
                I2 = identidad
            });
        }
        public static IQueryable<JoinGrupoAmigosIdentidadPerfilAdministradorOrganizacionPersonaPerfil> JoinPerfil(this IQueryable<JoinGrupoAmigosIdentidadPerfilAdministradorOrganizacionPersona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Persona.PersonaID, perfil => perfil.PersonaID, (item, perfil) => new JoinGrupoAmigosIdentidadPerfilAdministradorOrganizacionPersonaPerfil
            {
                AdministradorOrganizacion = item.AdministradorOrganizacion,
                GrupoAmigos = item.GrupoAmigos,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Persona = item.Persona,
                P2 = perfil
            });
        }

        public static IQueryable<JoinGrupoAmigosIdentidadPerfilAdministradorOrganizacionPersona> JoinPersona(this IQueryable<JoinGrupoAmigosIdentidadPerfilAdministradorOrganizacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Persona, item => item.AdministradorOrganizacion.UsuarioID, persona => persona.UsuarioID, (item, persona) => new JoinGrupoAmigosIdentidadPerfilAdministradorOrganizacionPersona
            {
                AdministradorOrganizacion = item.AdministradorOrganizacion,
                GrupoAmigos = item.GrupoAmigos,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Persona = persona
            });
        }

        public static IQueryable<JoinGrupoAmigosIdentidadPerfilAdministradorOrganizacion> JoinAdministradorOrganizacion(this IQueryable<JoinGrupoAmigosIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.AdministradorOrganizacion, item => item.Perfil.OrganizacionID, administradorOrganizacion => administradorOrganizacion.OrganizacionID, (item, administradorOrganizacion) => new JoinGrupoAmigosIdentidadPerfilAdministradorOrganizacion
            {
                AdministradorOrganizacion = administradorOrganizacion,
                GrupoAmigos = item.GrupoAmigos,
                Identidad = item.Identidad,
                Perfil = item.Perfil
            });
        }

        public static IQueryable<JoinGrupoAmigosIdentidadPerfil> JoinPerfil(this IQueryable<JoinGrupoAmigosIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new JoinGrupoAmigosIdentidadPerfil
            {
                GrupoAmigos = item.GrupoAmigos,
                Identidad = item.Identidad,
                Perfil = perfil
            });
        }

        public static IQueryable<JoinGrupoAmigosIdentidad> JoinIdentidad(this IQueryable<GrupoAmigos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, grupoAmigos => grupoAmigos.IdentidadID, identidad => identidad.IdentidadID, (grupoAmigos, identidad) => new JoinGrupoAmigosIdentidad
            {
                GrupoAmigos = grupoAmigos,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinIdentidadPerfilPermisoGrupoAmigoOrgGrupoAmigosAmigoAgGrupo> JoinAmigoAgGrupo(this IQueryable<JoinIdentidadPerfilPermisoGrupoAmigoOrgGrupoAmigos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.AmigoAgGrupo, item => item.PermisoGrupoAmigoOrg.GrupoID, amigoAgGrupo => amigoAgGrupo.GrupoID, (item, amigoAgGrupo) => new JoinIdentidadPerfilPermisoGrupoAmigoOrgGrupoAmigosAmigoAgGrupo
            {
                PermisoGrupoAmigoOrg = item.PermisoGrupoAmigoOrg,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                GrupoAmigos = item.GrupoAmigos,
                AmigoAgGrupo = amigoAgGrupo
            });
        }

        public static IQueryable<JoinIdentidadPerfilPermisoGrupoAmigoOrgGrupoAmigos> JoinGrupoAmigos(this IQueryable<JoinIdentidadPerfilPermisoGrupoAmigoOrg> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.GrupoAmigos, item => item.PermisoGrupoAmigoOrg.GrupoID, grupoAmigos => grupoAmigos.GrupoID, (item, grupoAmigos) => new JoinIdentidadPerfilPermisoGrupoAmigoOrgGrupoAmigos
            {
                PermisoGrupoAmigoOrg = item.PermisoGrupoAmigoOrg,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                GrupoAmigos = grupoAmigos
            });
        }

        public static IQueryable<JoinIdentidadPerfilPermisoGrupoAmigoOrg> JoinPermisoGrupoAmigoOrg(this IQueryable<JoinIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.PermisoGrupoAmigoOrg, item => item.Identidad.IdentidadID, permisoGrupoAmigoOrg => permisoGrupoAmigoOrg.IdentidadUsuarioID, (item, permisoGrupoAmigoOrg) => new JoinIdentidadPerfilPermisoGrupoAmigoOrg
            {
                PermisoGrupoAmigoOrg = permisoGrupoAmigoOrg,
                Identidad = item.Identidad,
                Perfil = item.Perfil
            });
        }

        public static IQueryable<JoinIdentidadPerfilPermisoAmigoOrg> JoinPermisoAmigoOrg(this IQueryable<JoinIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.PermisoAmigoOrg, item => item.Identidad.IdentidadID, permisoAmigoOrg => permisoAmigoOrg.IdentidadUsuarioID, (item, permisoAmigoOrg) => new JoinIdentidadPerfilPermisoAmigoOrg
            {
                PermisoAmigoOrg = permisoAmigoOrg,
                Identidad = item.Identidad,
                Perfil = item.Perfil
            });
        }

        public static IQueryable<JoinGrupoAmigosPermisoGrupoAmigoOrg> JoinPermisoGrupoAmigoOrg(this IQueryable<GrupoAmigos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.PermisoGrupoAmigoOrg, grupoAmigos => new { grupoAmigos.IdentidadID, grupoAmigos.GrupoID }, permisoGrupoAmigosOrg => new { IdentidadID = permisoGrupoAmigosOrg.IdentidadOrganizacionID, permisoGrupoAmigosOrg.GrupoID }, (grupoAmigos, permisoGrupoAmigoOrg) => new JoinGrupoAmigosPermisoGrupoAmigoOrg
            {
                GrupoAmigos = grupoAmigos,
                PermisoGrupoAmigosOrg = permisoGrupoAmigoOrg
            });
        }

        public static IQueryable<JoinIdentidadPerfilAmigoAmigoAgGrupoGrupoAmigosPermisoGrupoAmigoOrg> JoinPermisoGrupoAmigoOrg(this IQueryable<JoinIdentidadPerfilAmigoAmigoAgGrupoGrupoAmigos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.PermisoGrupoAmigoOrg, item => new { IdentidadID = item.Amigo.IdentidadID, GrupoID = item.GrupoAmigos.GrupoID }, permisoGrupoAmigoOrg => new { IdentidadID = permisoGrupoAmigoOrg.IdentidadOrganizacionID, GrupoID = permisoGrupoAmigoOrg.GrupoID }, (item, permisoGrupoAmigoOrg) => new JoinIdentidadPerfilAmigoAmigoAgGrupoGrupoAmigosPermisoGrupoAmigoOrg
            {
                Amigo = item.Amigo,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                AmigoAgGrupo = item.AmigoAgGrupo,
                GrupoAmigos = item.GrupoAmigos,
                PermisoGrupoAmigoOrg = permisoGrupoAmigoOrg
            });
        }

        public static IQueryable<JoinIdentidadPerfilAmigoAmigoAgGrupoGrupoAmigos> JoinGrupoAmigos(this IQueryable<JoinIdentidadPerfilAmigoAmigoAgGrupo> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.GrupoAmigos, item => new { item.AmigoAgGrupo.GrupoID, item.AmigoAgGrupo.IdentidadID }, grupoAmigos => new { grupoAmigos.GrupoID, grupoAmigos.IdentidadID }, (item, grupoAmigos) => new JoinIdentidadPerfilAmigoAmigoAgGrupoGrupoAmigos
            {
                Amigo = item.Amigo,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                AmigoAgGrupo = item.AmigoAgGrupo,
                GrupoAmigos = grupoAmigos
            });
        }

        public static IQueryable<JoinIdentidadPerfilAmigoAmigoAgGrupo> JoinAmigoAgGrupo(this IQueryable<JoinIdentidadPerfilAmigo> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.AmigoAgGrupo, item => new { item.Amigo.IdentidadID, item.Amigo.IdentidadAmigoID }, amigoAgGrupo => new { amigoAgGrupo.IdentidadID, amigoAgGrupo.IdentidadAmigoID }, (item, amigoAgGrupo) => new JoinIdentidadPerfilAmigoAmigoAgGrupo
            {
                Amigo = item.Amigo,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                AmigoAgGrupo = amigoAgGrupo
            });
        }

        public static IQueryable<JoinIdentidadPerfilAmigoPermisoAmigoOrg> JoinPermisoAmigoOrg(this IQueryable<JoinIdentidadPerfilAmigo> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.PermisoAmigoOrg, item => item.Amigo.IdentidadID, permisoAmigoOrg => permisoAmigoOrg.IdentidadOrganizacionID, (item, permisoAmigoOrg) => new JoinIdentidadPerfilAmigoPermisoAmigoOrg
            {
                Amigo = item.Amigo,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                PermisoAmigoOrg = permisoAmigoOrg
            });
        }

        public static IQueryable<JoinIdentidadPerfilAmigo> JoinAmigo(this IQueryable<JoinIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Amigo, item => new { IdentidadID = item.Identidad.IdentidadID }, amigo => new
            { IdentidadID = amigo.IdentidadAmigoID }, (item, amigo) => new JoinIdentidadPerfilAmigo
            {
                Amigo = amigo,
                Identidad = item.Identidad,
                Perfil = item.Perfil
            });
        }

        public static IQueryable<JoinIdentidadPerfilPerfilesMiembroIdentidadesMiembroDocumentoWebVinBaseRecursos> JoinDocumentoWebVinBaseRecursos(this IQueryable<JoinIdentidadPerfilPerfilesMiembroIdentidadesMiembro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoWebVinBaseRecursos, item => item.IdentidadesMiembro.IdentidadID, documentoWebVinBaseRecursos => documentoWebVinBaseRecursos.IdentidadPublicacionID, (item, documentoWebVinBaseRecursos) => new JoinIdentidadPerfilPerfilesMiembroIdentidadesMiembroDocumentoWebVinBaseRecursos
            {
                Identidad = item.Identidad,
                IdentidadesMiembro = item.IdentidadesMiembro,
                Perfil = item.Perfil,
                PerfilesMiembro = item.PerfilesMiembro,
                DocumentoWebVinBaseRecursos = documentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinIdentidadPerfilPerfilesMiembroIdentidadesMiembro> JoinIdentidadesMiembro(this IQueryable<JoinIdentidadPerfilPerfilesMiembro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.PerfilesMiembro.PerfilID, identidad => identidad.PerfilID, (item, identidad) => new JoinIdentidadPerfilPerfilesMiembroIdentidadesMiembro
            {
                Identidad = item.Identidad,
                IdentidadesMiembro = identidad,
                Perfil = item.Perfil,
                PerfilesMiembro = item.PerfilesMiembro
            });
        }

        public static IQueryable<JoinIdentidadPerfilPerfilesMiembro> JoinPerfilesMiembro(this IQueryable<JoinIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Perfil.OrganizacionID, perfil => perfil.OrganizacionID, (item, perfil) => new JoinIdentidadPerfilPerfilesMiembro
            {
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                PerfilesMiembro = perfil
            });
        }

        public static IQueryable<JoinIdentidadDocumentoWebVinBaseRecursos> JoinDocumentoWebVinBaseRecursos(this IQueryable<EntityModel.Models.IdentidadDS.Identidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoWebVinBaseRecursos, identidad => identidad.IdentidadID, documentoWebVinBaseRecursos => documentoWebVinBaseRecursos.IdentidadPublicacionID, (identidad, documentoWebVinBaseRecursos) => new JoinIdentidadDocumentoWebVinBaseRecursos
            {
                DocumentoWebVinBaseRecursos = documentoWebVinBaseRecursos,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinIdentidadIdentidadPerfil> JoinIdentidadPerfil(this IQueryable<EntityModel.Models.IdentidadDS.Identidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, identidad => identidad.PerfilID, identidadPerfil => identidadPerfil.PerfilID, (identidad, identidadPerfil) => new JoinIdentidadIdentidadPerfil
            {
                IdentidadPerfil = identidadPerfil,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinIdentidadIdentidadPerfilPerfil> JoinPerfil(this IQueryable<JoinIdentidadIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.IdentidadPerfil.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new JoinIdentidadIdentidadPerfilPerfil
            {
                IdentidadPerfil = item.IdentidadPerfil,
                Identidad = item.Identidad,
                Perfil = perfil
            });
        }

        public static IQueryable<JoinInvitacionIdentidad> JoinIdentidad(this IQueryable<Invitacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, invitacion => invitacion.IdentidadOrigenID, identidad => identidad.IdentidadID, (invitacion, identidad) => new JoinInvitacionIdentidad
            {
                Invitacion = invitacion,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinInvitacionIdentidadPerfil> JoinPerfil(this IQueryable<JoinInvitacionIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new JoinInvitacionIdentidadPerfil
            {
                Invitacion = item.Invitacion,
                Identidad = item.Identidad,
                Perfil = perfil
            });
        }

        public static IQueryable<JoinIdentidadDocumentoRolIdentidad> JoinDocumentoRolIdentidad(this IQueryable<AD.EntityModel.Models.IdentidadDS.Identidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoRolIdentidad, identidad => identidad.PerfilID, documentoRolIdentidad => documentoRolIdentidad.PerfilID, (identidad, documentoRolIdentidad) => new JoinIdentidadDocumentoRolIdentidad
            {
                DocumentoRolIdentidad = documentoRolIdentidad,
                Identidad = identidad,
            });
        }

        public static IQueryable<JoinIdentidadDocumentoRolIdentidadDocumento> JoinDocumento(this IQueryable<JoinIdentidadDocumentoRolIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, item => item.DocumentoRolIdentidad.DocumentoID, documento => documento.DocumentoID, (item, documento) => new JoinIdentidadDocumentoRolIdentidadDocumento
            {
                DocumentoRolIdentidad = item.DocumentoRolIdentidad,
                Identidad = item.Identidad,
                Documento = documento
            });
        }

        public static IQueryable<JoinIdentidadDocumentoRolIdentidadDocumentoPerfil> JoinPerfil(this IQueryable<JoinIdentidadDocumentoRolIdentidadDocumento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new JoinIdentidadDocumentoRolIdentidadDocumentoPerfil
            {
                DocumentoRolIdentidad = item.DocumentoRolIdentidad,
                Identidad = item.Identidad,
                Documento = item.Documento,
                Perfil = perfil
            });
        }

        public static IQueryable<JoinIdentidadDocumentoRolIdentidadDocumentoPerfilIdentidad> JoinIdentidad(this IQueryable<JoinIdentidadDocumentoRolIdentidadDocumentoPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Perfil.PerfilID, identidadMyGnoss => identidadMyGnoss.PerfilID, (item, identidadMyGnoss) => new JoinIdentidadDocumentoRolIdentidadDocumentoPerfilIdentidad
            {
                DocumentoRolIdentidad = item.DocumentoRolIdentidad,
                Identidad = item.Identidad,
                Documento = item.Documento,
                Perfil = item.Perfil,
                IdentidadMyGnoss = identidadMyGnoss
            });
        }

        public static IQueryable<JoinIdentidadDocumentoRolIdentidadDocumentoPerfilProyecto> JoinProyecto(this IQueryable<JoinIdentidadDocumentoRolIdentidadDocumentoPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Proyecto, item => item.Identidad.ProyectoID, proyecto => proyecto.ProyectoID, (item, proyecto) => new JoinIdentidadDocumentoRolIdentidadDocumentoPerfilProyecto
            {
                DocumentoRolIdentidad = item.DocumentoRolIdentidad,
                Identidad = item.Identidad,
                Documento = item.Documento,
                Perfil = item.Perfil,
                Proyecto = proyecto
            });
        }

        public static IQueryable<JoinIdentidadDocumentoRolIdentidadDocumentoPerfilDocumentoWebVinBaseRecursos> JoinDocumentoWebVinBaseRecursos(this IQueryable<JoinIdentidadDocumentoRolIdentidadDocumentoPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoWebVinBaseRecursos, item => item.Documento.DocumentoID, documentoWebVinBaseRecursos => documentoWebVinBaseRecursos.DocumentoID, (item, documentoWebVinBaseRecursos) => new JoinIdentidadDocumentoRolIdentidadDocumentoPerfilDocumentoWebVinBaseRecursos
            {
                DocumentoRolIdentidad = item.DocumentoRolIdentidad,
                Identidad = item.Identidad,
                Documento = item.Documento,
                Perfil = item.Perfil,
                DocumentoWebVinBaseRecursos = documentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinIdentidadDocumentoRolIdentidadDocumentoPerfilDocumentoWebVinBaseRecursosBaseRecursosOrganizacion> JoinBaseRecursosOrganizacion(this IQueryable<JoinIdentidadDocumentoRolIdentidadDocumentoPerfilDocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.BaseRecursosOrganizacion, item => item.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecursosOrganizacion => baseRecursosOrganizacion.BaseRecursosID, (item, baseRecursosOrganizacion) => new JoinIdentidadDocumentoRolIdentidadDocumentoPerfilDocumentoWebVinBaseRecursosBaseRecursosOrganizacion
            {
                DocumentoRolIdentidad = item.DocumentoRolIdentidad,
                Identidad = item.Identidad,
                Documento = item.Documento,
                Perfil = item.Perfil,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                BaseRecursosOrganizacion = baseRecursosOrganizacion
            });
        }

        public static IQueryable<JoinIdentidadDocumentoRolIdentidadDocumentoPerfilDocumentoWebVinBaseRecursosBaseRecursosUsuario> JoinBaseRecursosUsuario(this IQueryable<JoinIdentidadDocumentoRolIdentidadDocumentoPerfilDocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.BaseRecursosUsuario, item => item.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecursosUsuario => baseRecursosUsuario.BaseRecursosID, (item, baseRecursosUsuario) => new JoinIdentidadDocumentoRolIdentidadDocumentoPerfilDocumentoWebVinBaseRecursosBaseRecursosUsuario
            {
                DocumentoRolIdentidad = item.DocumentoRolIdentidad,
                Identidad = item.Identidad,
                Documento = item.Documento,
                Perfil = item.Perfil,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                BaseRecursosUsuario = baseRecursosUsuario
            });
        }

        public static IQueryable<JoinIdentidadGrupoIdentidadesParticipacion> JoinGrupoIdentidadesParticipacion(this IQueryable<EntityModel.Models.IdentidadDS.Identidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.GrupoIdentidadesParticipacion, identidad => identidad.IdentidadID, grupoIdentidadesParticipacion => grupoIdentidadesParticipacion.IdentidadID, (identidad, grupoIdentidadesParticipacion) => new JoinIdentidadGrupoIdentidadesParticipacion
            {
                GrupoIdentidadesParticipacion = grupoIdentidadesParticipacion,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinSuscripcionSuscripcionBlogIdentidad> JoinIdentidad(this IQueryable<JoinSuscripcionSuscripcionBlog> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Suscripcion.IdentidadID, identidad => identidad.IdentidadID, (item, identidad) => new JoinSuscripcionSuscripcionBlogIdentidad
            {
                Suscripcion = item.Suscripcion,
                SuscripcionBlog = item.SuscripcionBlog,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinSuscripcionSuscripcionBlog> JoinSuscripcionBlog(this IQueryable<EntityModel.Models.Suscripcion.Suscripcion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.SuscripcionBlog, suscripcion => suscripcion.SuscripcionID, suscripcionBlog => suscripcionBlog.SuscripcionID, (suscripcion, suscripcionBlog) => new JoinSuscripcionSuscripcionBlog
            {
                Suscripcion = suscripcion,
                SuscripcionBlog = suscripcionBlog
            });
        }

        public static IQueryable<JoinSuscripcionSuscripcionTesauroProyecto> JoinSuscripcionTesauroProyecto(this IQueryable<EntityModel.Models.Suscripcion.Suscripcion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.SuscripcionTesauroProyecto, suscripcion => suscripcion.SuscripcionID, suscripcionTesauroProyecto => suscripcionTesauroProyecto.SuscripcionID, (suscripcion, suscripcionTesauroProyecto) => new JoinSuscripcionSuscripcionTesauroProyecto
            {
                Suscripcion = suscripcion,
                SuscripcionTesauroProyecto = suscripcionTesauroProyecto
            });
        }

        public static IQueryable<JoinSuscripcionSuscripcionTesauroProyectoCategoriaTesVinSuscrip> JoinCategoriaTesVinSuscrip(this IQueryable<JoinSuscripcionSuscripcionTesauroProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CategoriaTesVinSuscrip, item => item.Suscripcion.SuscripcionID, categoriaTesVinSuscrip => categoriaTesVinSuscrip.SuscripcionID, (item, categoriaTesVinSuscrip) => new JoinSuscripcionSuscripcionTesauroProyectoCategoriaTesVinSuscrip
            {
                Suscripcion = item.Suscripcion,
                SuscripcionTesauroProyecto = item.SuscripcionTesauroProyecto,
                CategoriaTesVinSuscrip = categoriaTesVinSuscrip
            });
        }

        public static IQueryable<JoinEntradaBlogIdentidad> JoinIdentidad(this IQueryable<EntradaBlog> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, entradaBlog => entradaBlog.AutorID, identidad => identidad.IdentidadID, (entradaBlog, identidad) => new JoinEntradaBlogIdentidad
            {
                EntradaBlog = entradaBlog,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinBlogIdentidad> JoinIdentidad(this IQueryable<Blog> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, blog => blog.AutorID, identidad => identidad.IdentidadID, (blog, identidad) => new JoinBlogIdentidad
            {
                Blog = blog,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinProyectoProyectoUsuarioIdentidadIdentidad> JoinIdentidad(this IQueryable<JoinProyectoProyectoUsuarioIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.ProyectoUsuarioIdentiad.IdentidadID, identidad => identidad.IdentidadID, (item, identidad) => new JoinProyectoProyectoUsuarioIdentidadIdentidad
            {
                Identidad = identidad,
                Proyecto = item.Proyecto,
                ProyectoUsuarioIdentiad = item.ProyectoUsuarioIdentiad
            });
        }

        public static IQueryable<JoinProyectoProyectoUsuarioIdentidad> JoinProyectoUsuarioIdentidad(this IQueryable<Proyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.ProyectoUsuarioIdentidad, proyecto => proyecto.ProyectoID, proyectoUsuarioIdentidad => proyectoUsuarioIdentidad.ProyectoID, (proyecto, proyectoUsuarioIdentidad) => new JoinProyectoProyectoUsuarioIdentidad
            {
                Proyecto = proyecto,
                ProyectoUsuarioIdentiad = proyectoUsuarioIdentidad
            });
        }

        public static IQueryable<JoinDatoExtraEcosistemaVirtuosoPerfilDatoExtraEcosistemaVirtuosoIdentidad> JoinIdentidad(this IQueryable<JoinDatoExtraEcosistemaVirtuosoPerfilDatoExtraEcosistemaVirtuoso> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.DatoExtraEcosistemaVirtuosoPerfil.PerfilID, identidad => identidad.PerfilID, (item, identidad) => new JoinDatoExtraEcosistemaVirtuosoPerfilDatoExtraEcosistemaVirtuosoIdentidad
            {
                DatoExtraEcosistemaVirtuoso = item.DatoExtraEcosistemaVirtuoso,
                DatoExtraEcosistemaVirtuosoPerfil = item.DatoExtraEcosistemaVirtuosoPerfil,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinDatoExtraEcosistemaVirtuosoPerfilDatoExtraEcosistemaVirtuoso> JoinDatoExtraEcosistemaVirtuoso(this IQueryable<DatoExtraEcosistemaVirtuosoPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DatoExtraEcosistemaVirtuoso, datoExtraEcosistemaVirtuosoPerfil => datoExtraEcosistemaVirtuosoPerfil.DatoExtraID, datoExtraEcosistemaVirtuoso => datoExtraEcosistemaVirtuoso.DatoExtraID, (datoExtraEcosistemaVirtuosoPerfil, datoExtraEcosistemaVirtuoso) => new JoinDatoExtraEcosistemaVirtuosoPerfilDatoExtraEcosistemaVirtuoso
            {
                DatoExtraEcosistemaVirtuoso = datoExtraEcosistemaVirtuoso,
                DatoExtraEcosistemaVirtuosoPerfil = datoExtraEcosistemaVirtuosoPerfil
            });
        }

        public static IQueryable<JoinDatoExtraEcosistemaDatoExtraEcosistemaOpcionDatoExtraEcosistemaOpcionPerfilIdentidad> JoinIdentidad(this IQueryable<JoinDatoExtraEcosistemaDatoExtraEcosistemaOpcionDatoExtraEcosistemaOpcionPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.DatoExtraEcosistemaOpcionPerfil.PerfilID, identidad => identidad.PerfilID, (item, identidad) => new JoinDatoExtraEcosistemaDatoExtraEcosistemaOpcionDatoExtraEcosistemaOpcionPerfilIdentidad
            {
                Identidad = identidad,
                DatoExtraEcosistema = item.DatoExtraEcosistema,
                DatoExtraEcosistemaOpcion = item.DatoExtraEcosistemaOpcion,
                DatoExtraEcosistemaOpcionPerfil = item.DatoExtraEcosistemaOpcionPerfil
            });
        }

        public static IQueryable<JoinDatoExtraEcosistemaDatoExtraEcosistemaOpcionDatoExtraEcosistemaOpcionPerfil> JoinDatoExtraEcosistemaOpcionPerfil(this IQueryable<JoinDatoExtraEcosistemaDatoExtraEcosistemaOpcion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DatoExtraEcosistemaOpcionPerfil, item => new { item.DatoExtraEcosistemaOpcion.DatoExtraID, item.DatoExtraEcosistemaOpcion.OpcionID }, datoExtraEcosisemaOpcionPerfil => new { datoExtraEcosisemaOpcionPerfil.DatoExtraID, datoExtraEcosisemaOpcionPerfil.OpcionID }, (item, datoExtraEcosistemaOpcionPerfil) => new JoinDatoExtraEcosistemaDatoExtraEcosistemaOpcionDatoExtraEcosistemaOpcionPerfil
            {
                DatoExtraEcosistema = item.DatoExtraEcosistema,
                DatoExtraEcosistemaOpcion = item.DatoExtraEcosistemaOpcion,
                DatoExtraEcosistemaOpcionPerfil = datoExtraEcosistemaOpcionPerfil
            });
        }

        public static IQueryable<JoinDatoExtraEcosistemaDatoExtraEcosistemaOpcion> JoinDatoExtraEcosistemaOpcion(this IQueryable<DatoExtraEcosistema> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DatoExtraEcosistemaOpcion, datoExtraEcosistema => datoExtraEcosistema.DatoExtraID, datoExtraEcosistemaOpcion => datoExtraEcosistemaOpcion.DatoExtraID, (datoExtraEcosistema, datoExtraEcosistemaOpcion) => new JoinDatoExtraEcosistemaDatoExtraEcosistemaOpcion
            {
                DatoExtraEcosistema = datoExtraEcosistema,
                DatoExtraEcosistemaOpcion = datoExtraEcosistemaOpcion
            });
        }

        public static IQueryable<JoinDatoExtraProyectoVirtuosoIdentidadDatoExtraProyectoVirtuoso> JoinDatoExtraProyectoVirtuoso(this IQueryable<DatoExtraProyectoVirtuosoIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DatoExtraProyectoVirtuoso, datoExtraProyectoVirtuosoIdentidad => new { datoExtraProyectoVirtuosoIdentidad.OrganizacionID, datoExtraProyectoVirtuosoIdentidad.ProyectoID, datoExtraProyectoVirtuosoIdentidad.DatoExtraID }, datoExtraProyectoVirtuoso => new { datoExtraProyectoVirtuoso.OrganizacionID, datoExtraProyectoVirtuoso.ProyectoID, datoExtraProyectoVirtuoso.DatoExtraID }, (datoExtraProyectoVirtuosoIdentidad, datoExtraProyectoVirtuoso) => new JoinDatoExtraProyectoVirtuosoIdentidadDatoExtraProyectoVirtuoso
            {
                DatoExtraProyectoVirtuosoIdentidad = datoExtraProyectoVirtuosoIdentidad,
                DatoExtraProyectoVirtuoso = datoExtraProyectoVirtuoso
            });
        }

        public static IQueryable<JoinDatoExtraProyectoDatoExtraProyectoOpcionDatoExtraProyectoOpcionIdentidad> JoinDatoExtraProyectoOpcionIdentidad(this IQueryable<JoinDatoExtraProyectoDatoExtraProyectoOpcion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DatoExtraProyectoOpcionIdentidad, item => new { item.DatoExtraProyectoOpcion.DatoExtraID, item.DatoExtraProyectoOpcion.OpcionID }, datoExtraProyectoOpcionIdentidad => new { datoExtraProyectoOpcionIdentidad.DatoExtraID, datoExtraProyectoOpcionIdentidad.OpcionID }, (item, datoExtraProyectoOpcionIdentidad) => new JoinDatoExtraProyectoDatoExtraProyectoOpcionDatoExtraProyectoOpcionIdentidad
            {
                DatoExtraProyecto = item.DatoExtraProyecto,
                DatoExtraProyectoOpcion = item.DatoExtraProyectoOpcion,
                DatoExtraProyectoOpcionIdentidad = datoExtraProyectoOpcionIdentidad
            });
        }

        public static IQueryable<JoinDatoExtraProyectoDatoExtraProyectoOpcion> JoinDatoExtraProyectoOpcion(this IQueryable<DatoExtraProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DatoExtraProyectoOpcion, datoExtraProyecto => datoExtraProyecto.DatoExtraID, datoExtraProyectoOpcion => datoExtraProyectoOpcion.DatoExtraID, (datoExtraProyecto, datoExtraProyectoOpcion) => new JoinDatoExtraProyectoDatoExtraProyectoOpcion
            {
                DatoExtraProyecto = datoExtraProyecto,
                DatoExtraProyectoOpcion = datoExtraProyectoOpcion
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidadPerfil> JoinPerfi(this IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new JoinDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidadPerfil
            {
                BaseRecursosProyecto = item.BaseRecursosProyecto,
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                Identidad = item.Identidad,
                Perfil = perfil
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidad> JoinIdentidad(this IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.DocumentoWebVinBaseRecursos.IdentidadPublicacionID, identidad => identidad.IdentidadID, (item, identidad) => new JoinDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidad
            {
                BaseRecursosProyecto = item.BaseRecursosProyecto,
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosProyecto> JoinBaseRecursosProyecto(this IQueryable<JoinDocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.BaseRecursosProyecto, item => item.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecursosProyecto => baseRecursosProyecto.BaseRecursosID, (item, baseRecursosProyecto) => new JoinDocumentoWebVinBaseRecursosBaseRecursosProyecto
            {
                BaseRecursosProyecto = baseRecursosProyecto,
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosUsuarioIdentidadPerfilPerfil> JoinPerfil(this IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosUsuarioIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.PerfilPublicador.PersonaID, todosPerfiles => todosPerfiles.PersonaID, (item, todosPerfiles) => new JoinDocumentoWebVinBaseRecursosBaseRecursosUsuarioIdentidadPerfilPerfil
            {
                BaseRecursosUsuario = item.BaseRecursosUsuario,
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                Identidad = item.Identidad,
                PerfilPublicador = item.PerfilPublicador,
                TodosPerfiles = todosPerfiles
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosUsuarioIdentidadPerfil> JoinPerfil(this IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosUsuarioIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Identidad.PerfilID, perfilPublicador => perfilPublicador.PerfilID, (item, perfilPublicador) => new JoinDocumentoWebVinBaseRecursosBaseRecursosUsuarioIdentidadPerfil
            {
                BaseRecursosUsuario = item.BaseRecursosUsuario,
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                Identidad = item.Identidad,
                PerfilPublicador = perfilPublicador
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosUsuarioIdentidad> JoinIdentidad(this IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosUsuario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.DocumentoWebVinBaseRecursos.IdentidadPublicacionID, identidad => identidad.IdentidadID, (item, identidad) => new JoinDocumentoWebVinBaseRecursosBaseRecursosUsuarioIdentidad
            {
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                BaseRecursosUsuario = item.BaseRecursosUsuario,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosBaseRecursosUsuario> JoinBaseRecursosUsuario(this IQueryable<JoinDocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.BaseRecursosUsuario, item => item.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecursosUsuario => baseRecursosUsuario.BaseRecursosID, (item, baseRecursosUsuario) => new JoinDocumentoWebVinBaseRecursosBaseRecursosUsuario
            {
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                BaseRecursosUsuario = baseRecursosUsuario
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursos> JoinDocumento(this IQueryable<DocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, documentoWebVinBaseRercursos => documentoWebVinBaseRercursos.DocumentoID, documento => documento.DocumentoID, (documentoWebVinBaseRecursos, documento) => new JoinDocumentoWebVinBaseRecursos
            {
                Documento = documento,
                DocumentoWebVinBaseRecursos = documentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinUsuarioPersonaPerfilIdentidadNotificacionCorreoPersonaNotificacion> JoinNotificacion(this IQueryable<JoinUsuarioPersonaPerfilIdentidadNotificacionCorreoPersona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Notificacion, item => item.NotificacionCorreoPersona.NotificacionID, notificacion => notificacion.NotificacionID, (item, notificacion) => new JoinUsuarioPersonaPerfilIdentidadNotificacionCorreoPersonaNotificacion
            {
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Persona = item.Persona,
                Usuario = item.Usuario,
                NotificacionCorreoPersona = item.NotificacionCorreoPersona,
                Notificacion = notificacion
            });
        }

        public static IQueryable<JoinUsuarioPersonaPerfilIdentidadNotificacionCorreoPersona> JoinNotificacionCorreoPersona(this IQueryable<JoinUsuarioPersonaPerfilIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.NotificacionCorreoPersona, item => item.Persona.PersonaID, notificacionCorreoPersona => notificacionCorreoPersona.PersonaID, (item, notificacionCorreoPersona) => new JoinUsuarioPersonaPerfilIdentidadNotificacionCorreoPersona
            {
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Persona = item.Persona,
                Usuario = item.Usuario,
                NotificacionCorreoPersona = notificacionCorreoPersona
            });
        }

        public static IQueryable<JoinUsuarioPersonaPerfilIdentidad> JoinIdentidad(this IQueryable<JoinUsuarioPersonaPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Perfil.PerfilID, identidad => identidad.PerfilID, (item, identidad) => new JoinUsuarioPersonaPerfilIdentidad
            {
                Identidad = identidad,
                Perfil = item.Perfil,
                Persona = item.Persona,
                Usuario = item.Usuario
            });
        }

        public static IQueryable<JoinUsuarioPersonaPerfil> JoinPerfil(this IQueryable<JoinUsuarioPersona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Persona.PersonaID, perfil => perfil.PersonaID, (item, perfil) => new JoinUsuarioPersonaPerfil
            {
                Perfil = perfil,
                Persona = item.Persona,
                Usuario = item.Usuario
            });
        }

        public static IQueryable<JoinUsuarioPersona> JoinPersona(this IQueryable<EntityModel.Models.UsuarioDS.Usuario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Persona, usuario => usuario.UsuarioID, persona => persona.UsuarioID, (usuario, persona) => new JoinUsuarioPersona
            {
                Persona = persona,
                Usuario = usuario
            });
        }

        public static IQueryable<JoinGrupoIdentGrupoIdentidadesProyectoIdentidadPerfilPersonaUsuarioAdministradorProyectoProyectoParametroGeneral> JoinParametroGeneral(this IQueryable<JoinGrupoIdentGrupoIdentidadesProyectoIdentidadPerfilPersonaUsuarioAdministradorProyectoProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.ParametroGeneral, item => item.Identidad.ProyectoID, parametroGeneral => parametroGeneral.ProyectoID, (item, parametroGeneral) => new JoinGrupoIdentGrupoIdentidadesProyectoIdentidadPerfilPersonaUsuarioAdministradorProyectoProyectoParametroGeneral
            {
                GrupoIdentidades = item.GrupoIdentidades,
                Perfil = item.Perfil,
                GrupoIdentidadesProyecto = item.GrupoIdentidadesProyecto,
                Identidad = item.Identidad,
                Persona = item.Persona,
                Usuario = item.Usuario,
                AdministradorProyecto = item.AdministradorProyecto,
                Proyecto = item.Proyecto,
                ParametroGeneral = parametroGeneral
            });
        }

        public static IQueryable<JoinGrupoIdentGrupoIdentidadesProyectoIdentidadPerfilPersonaUsuarioAdministradorProyectoProyecto> JoinProyecto(this IQueryable<JoinGrupoIdentGrupoIdentidadesProyectoIdentidadPerfilPersonaUsuarioAdministradorProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Proyecto, item => item.Identidad.ProyectoID, proyecto => proyecto.ProyectoID, (item, proyecto) => new JoinGrupoIdentGrupoIdentidadesProyectoIdentidadPerfilPersonaUsuarioAdministradorProyectoProyecto
            {
                GrupoIdentidades = item.GrupoIdentidades,
                Perfil = item.Perfil,
                GrupoIdentidadesProyecto = item.GrupoIdentidadesProyecto,
                Identidad = item.Identidad,
                Persona = item.Persona,
                Usuario = item.Usuario,
                AdministradorProyecto = item.AdministradorProyecto,
                Proyecto = proyecto
            });
        }

        public static IQueryable<JoinGrupoIdentGrupoIdentidadesProyectoIdentidadPerfilPersonaUsuarioAdministradorProyecto> JoinAdministradorProyecto(this IQueryable<JoinGrupoIdentGrupoIdentidadesProyectoIdentidadPerfilPersonaUsuario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.AdministradorProyecto, item => new { item.Usuario.UsuarioID, item.Identidad.ProyectoID }, administradorProyecto => new { administradorProyecto.UsuarioID, administradorProyecto.ProyectoID }, (item, administradorProyecto) => new JoinGrupoIdentGrupoIdentidadesProyectoIdentidadPerfilPersonaUsuarioAdministradorProyecto
            {
                GrupoIdentidades = item.GrupoIdentidades,
                Perfil = item.Perfil,
                GrupoIdentidadesProyecto = item.GrupoIdentidadesProyecto,
                Identidad = item.Identidad,
                Persona = item.Persona,
                Usuario = item.Usuario,
                AdministradorProyecto = administradorProyecto
            });
        }

        public static IQueryable<JoinGrupoIdentGrupoIdentidadesProyectoIdentidadPerfilPersonaUsuario> JoinUsuario(this IQueryable<JoinGrupoIdentGrupoIdentidadesProyectoIdentidadPerfilPersona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Usuario, item => item.Persona.UsuarioID, usuario => usuario.UsuarioID, (item, usuario) => new JoinGrupoIdentGrupoIdentidadesProyectoIdentidadPerfilPersonaUsuario
            {
                GrupoIdentidades = item.GrupoIdentidades,
                Perfil = item.Perfil,
                GrupoIdentidadesProyecto = item.GrupoIdentidadesProyecto,
                Identidad = item.Identidad,
                Persona = item.Persona,
                Usuario = usuario
            });
        }

        public static IQueryable<JoinGrupoIdentGrupoIdentidadesProyectoIdentidadPerfilPersona> JoinPersona(this IQueryable<JoinGrupoIdentGrupoIdentidadesProyectoIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Persona, item => item.Perfil.PersonaID, persona => persona.PersonaID, (item, persona) => new JoinGrupoIdentGrupoIdentidadesProyectoIdentidadPerfilPersona
            {
                GrupoIdentidades = item.GrupoIdentidades,
                Perfil = item.Perfil,
                GrupoIdentidadesProyecto = item.GrupoIdentidadesProyecto,
                Identidad = item.Identidad,
                Persona = persona
            });
        }

        public static IQueryable<JoinGrupoIdentGrupoIdentidadesProyectoIdentidadPerfil> JoinPerfil(this IQueryable<JoinGrupoIdentGrupoIdentidadesProyectoIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new JoinGrupoIdentGrupoIdentidadesProyectoIdentidadPerfil
            {
                GrupoIdentidades = item.GrupoIdentidades,
                Perfil = perfil,
                GrupoIdentidadesProyecto = item.GrupoIdentidadesProyecto,
                Identidad = item.Identidad
            });
        }

        public static IQueryable<JoinGrupoIdentGrupoIdentidadesProyectoIdentidadProyecto> JoinProyecto(this IQueryable<JoinGrupoIdentGrupoIdentidadesProyectoIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Proyecto, item => item.Identidad.ProyectoID, proyecto => proyecto.ProyectoID, (item, proyecto) => new JoinGrupoIdentGrupoIdentidadesProyectoIdentidadProyecto
            {
                GrupoIdentidades = item.GrupoIdentidades,
                GrupoIdentidadesProyecto = item.GrupoIdentidadesProyecto,
                Identidad = item.Identidad,
                Proyecto = proyecto
            });
        }

        public static IQueryable<JoinGrupoIdentGrupoIdentidadesProyectoIdentidad> JoinIdentidad(this IQueryable<JoinGrupoIdentGrupoIdentidadesProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.GrupoIdentidadesProyecto.ProyectoID, identidad => identidad.ProyectoID, (item, identidad) => new JoinGrupoIdentGrupoIdentidadesProyectoIdentidad
            {
                GrupoIdentidades = item.GrupoIdentidades,
                GrupoIdentidadesProyecto = item.GrupoIdentidadesProyecto,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinGrupoIdentGrupoIdentidadesParticipacionIdentidadProyecto> JoinProyecto(this IQueryable<JoinGrupoIdentGrupoIdentidadesParticipacionIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Proyecto, item => item.Identidad.ProyectoID, proyecto => proyecto.ProyectoID, (item, proyecto) => new JoinGrupoIdentGrupoIdentidadesParticipacionIdentidadProyecto
            {
                GrupoIdentidades = item.GrupoIdentidades,
                GrupoIdentidadesParticipacion = item.GrupoIdentidadesParticipacion,
                Identidad = item.Identidad,
                Proyecto = proyecto
            });
        }

        public static IQueryable<JoinGrupoIdentGrupoIdentidadesParticipacionIdentidad> JoinIdentidad(this IQueryable<JoinGrupoIdentGrupoIdentidadesParticipacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.GrupoIdentidadesParticipacion.IdentidadID, identidad => identidad.IdentidadID, (item, identidad) => new JoinGrupoIdentGrupoIdentidadesParticipacionIdentidad
            {
                GrupoIdentidades = item.GrupoIdentidades,
                GrupoIdentidadesParticipacion = item.GrupoIdentidadesParticipacion,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinIdentidadPerfilOrganizacion> JoinOrganizacion(this IQueryable<JoinIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Organizacion, item => item.Perfil.OrganizacionID, organizacion => organizacion.OrganizacionID, (item, organizacion) => new JoinIdentidadPerfilOrganizacion
            {
                Identidad = item.Identidad,
                Organizacion = organizacion,
                Perfil = item.Perfil
            });
        }

        public static IQueryable<JoinDocumentoVincDocDocumentoIdentidad> JoinIdentidad(this IQueryable<JoinDocumentoVincDocDocumento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Documento.CreadorID, identidad => identidad.IdentidadID, (item, identidad) => new JoinDocumentoVincDocDocumentoIdentidad
            {
                Identidad = identidad,
                Documento = item.Documento,
                DocumentoVincDoc = item.DocumentoVincDoc
            });
        }

        public static IQueryable<JoinDocumentoVincDocDocumento> JoinDocumento(this IQueryable<EntityModel.Models.Documentacion.DocumentoVincDoc> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, documentoVincDoc => documentoVincDoc.DocumentoID, documento => documento.DocumentoID, (documentoVincDoc, documento) => new JoinDocumentoVincDocDocumento
            {
                Documento = documento,
                DocumentoVincDoc = documentoVincDoc
            });
        }

        public static IQueryable<JoinDocumentoComentarioDocumentoComentarioIdentidad> JoinIdentiad(this IQueryable<JoinDocumentoComentarioDocumentoComentario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Documento.CreadorID, identidad => identidad.IdentidadID, (item, identidad) => new JoinDocumentoComentarioDocumentoComentarioIdentidad
            {
                Comentario = item.Comentario,
                Documento = item.Documento,
                DocumentoComentario = item.DocumentoComentario,
                Identiad = identidad
            });
        }

        public static IQueryable<JoinDocumentoComentarioDocumentoComentario> JoinComentario(this IQueryable<JoinDocumentoComentarioDocumento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Comentario, item => item.DocumentoComentario.ComentarioID, comentario => comentario.ComentarioID, (item, comentario) => new JoinDocumentoComentarioDocumentoComentario
            {
                Comentario = comentario,
                Documento = item.Documento,
                DocumentoComentario = item.DocumentoComentario
            });
        }

        public static IQueryable<JoinDocumentoComentarioDocumento> JoinDocumento(this IQueryable<DocumentoComentario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, documentoComentario => documentoComentario.DocumentoID, documento => documento.DocumentoID, (documentoComentario, documento) => new JoinDocumentoComentarioDocumento
            {
                DocumentoComentario = documentoComentario,
                Documento = documento
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosIdentidadDocumento> JoinDocumento(this IQueryable<JoinDocumentoWebVinBaseRecursosIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, item => item.DocumentoWebVinBaseRecursos.DocumentoID, documento => documento.DocumentoID, (item, documento) => new JoinDocumentoWebVinBaseRecursosIdentidadDocumento
            {
                Documento = documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                Identidad = item.Identidad
            });
        }

        public static IQueryable<JoinPersonaPerfilIdentidadAmigo> JoinAmigo(this IQueryable<JoinPersonaPerfilIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Amigo, item => item.Identidad.IdentidadID, amigo => amigo.IdentidadID, (item, amigo) => new JoinPersonaPerfilIdentidadAmigo
            {
                Amigo = amigo,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Persona = item.Persona
            });
        }

        public static IQueryable<JoinPersonaPerfilBaseRecursosUsuario> JoinBaseRecursosUsuario(this IQueryable<JoinPersonaPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.BaseRecursosUsuario, item => item.Persona.UsuarioID, baseRecursosUsuario => baseRecursosUsuario.UsuarioID, (item, baseRecursosUsuario) => new JoinPersonaPerfilBaseRecursosUsuario
            {
                BaseRecursosUsuario = baseRecursosUsuario,
                Perfil = item.Perfil,
                Persona = item.Persona
            });
        }

        public static IQueryable<JoinPersonaPerfilBaseRecursosUsuarioDocumentoWebVinBaseRecursos> JoinDocumentoWebVinBaseRecursos(this IQueryable<JoinPersonaPerfilBaseRecursosUsuario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoWebVinBaseRecursos, item => item.BaseRecursosUsuario.BaseRecursosID, documentoWebVinBaseRecursos => documentoWebVinBaseRecursos.BaseRecursosID, (item, documentoWebVinBaseRecursos) => new JoinPersonaPerfilBaseRecursosUsuarioDocumentoWebVinBaseRecursos
            {
                BaseRecursosUsuario = item.BaseRecursosUsuario,
                Perfil = item.Perfil,
                Persona = item.Persona,
                DocumentoWebVinBaseRecursos = documentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinPersonaPerfilBaseRecursosUsuarioDocumentoWebVinBaseRecursosDocumento> JoinDocumento(this IQueryable<JoinPersonaPerfilBaseRecursosUsuarioDocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, item => item.DocumentoWebVinBaseRecursos.DocumentoID, documento => documento.DocumentoID, (item, documento) => new JoinPersonaPerfilBaseRecursosUsuarioDocumentoWebVinBaseRecursosDocumento
            {
                BaseRecursosUsuario = item.BaseRecursosUsuario,
                Perfil = item.Perfil,
                Persona = item.Persona,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                Documento = documento
            });
        }

        public static IQueryable<JoinPersonaPerfilIdentidad> JoinIdentidad(this IQueryable<JoinPersonaPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Perfil.PerfilID, identidad => identidad.PerfilID, (item, identidad) => new JoinPersonaPerfilIdentidad
            {
                Identidad = identidad,
                Perfil = item.Perfil,
                Persona = item.Persona
            });
        }

        public static IQueryable<JoinPersonaPerfil> JoinPerfil(this IQueryable<Persona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, persona => persona.PersonaID, perfil => perfil.PersonaID, (persona, perfil) => new JoinPersonaPerfil
            {
                Perfil = perfil,
                Persona = persona
            });
        }

        public static IQueryable<JoinIdentidadPerfilUsuario> JoinUsuario(this IQueryable<JoinIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Usuario, item => item.Perfil.NombreCortoUsu, usuario => usuario.NombreCorto, (item, usuario) => new JoinIdentidadPerfilUsuario
            {
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Usuario = usuario
            });
        }

        public static IQueryable<JoinIdentidadPerfilPersonaPersonaVinculoOrganizacionPersonaVisibleEnOrg> JoinPersonaVisibleEnOrg(this IQueryable<JoinIdentidadPerfilPersonaPersonaVinculoOrganizacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.PersonaVisibleEnOrg, item => new { PersonaID = item.Perfil.PersonaID.Value, OrganizacionID = item.Perfil.OrganizacionID.Value }, personaVisibleEnOrg => new { personaVisibleEnOrg.PersonaID, personaVisibleEnOrg.OrganizacionID }, (item, personaVisibleEnOrg) => new JoinIdentidadPerfilPersonaPersonaVinculoOrganizacionPersonaVisibleEnOrg
            {
                PersonaVinculoOrganizacion = item.PersonaVinculoOrganizacion,
                Persona = item.Persona,
                Perfil = item.Perfil,
                Identidad = item.Identidad,
                PersonaVisibleEnOrg = personaVisibleEnOrg
            });
        }

        public static IQueryable<JoinIdentidadPerfilPersonaPersonaVinculoOrganizacion> JoinPersonaVinculoOrganizacion(this IQueryable<JoinIdentidadPerfilPersona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.PersonaVinculoOrganizacion, item => new { PersonaID = item.Perfil.PersonaID.Value, OrganizacionID = item.Perfil.OrganizacionID.Value }, personaVinculoOrganizacion => new { personaVinculoOrganizacion.PersonaID, personaVinculoOrganizacion.OrganizacionID }, (item, personaVinculoOrganizacion) => new JoinIdentidadPerfilPersonaPersonaVinculoOrganizacion
            {
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Persona = item.Persona,
                PersonaVinculoOrganizacion = personaVinculoOrganizacion
            });
        }

        public static IQueryable<JoinIdentidadPerfilPersona> JoinPersona(this IQueryable<JoinIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Persona, item => item.Perfil.PersonaID, persona => persona.PersonaID, (item, persona) => new JoinIdentidadPerfilPersona
            {
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Persona = persona
            });
        }

        public static IQueryable<JoinIdentidadPerfilPersonaProvincia> JoinProvincia(this IQueryable<JoinIdentidadPerfilPersona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Provincia, item => item.Persona.ProvinciaPersonalID.Value, provincia => provincia.ProvinciaID, (item, provincia) => new JoinIdentidadPerfilPersonaProvincia
            {
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Persona = item.Persona,
                Provincia = provincia
            });
        }

        public static IQueryable<JoinIdentidadPerfilPersonaPais> JoinPais(this IQueryable<JoinIdentidadPerfilPersona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Pais, item => item.Persona.PaisPersonalID.Value, pais => pais.PaisID, (item, pais) => new JoinIdentidadPerfilPersonaPais
            {
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Persona = item.Persona,
                Pais = pais
            });
        }

        public static IQueryable<JoinIdentidadPerfilPersonaAdministradorProyecto> JoinAdministradorProyecto(this IQueryable<JoinIdentidadPerfilPersona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.AdministradorProyecto, item => item.Identidad.ProyectoID, administradorProyecto => administradorProyecto.ProyectoID, (item, administradorProyecto) => new JoinIdentidadPerfilPersonaAdministradorProyecto
            {
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Persona = item.Persona,
                AdministradorProyecto = administradorProyecto
            });
        }

        public static IQueryable<JoinIdentidadPerfilPersonaAdministradorProyectoProyecto> JoinProyecto(this IQueryable<JoinIdentidadPerfilPersonaAdministradorProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Proyecto, item => item.AdministradorProyecto.ProyectoID, proyecto => proyecto.ProyectoID, (item, proyecto) => new JoinIdentidadPerfilPersonaAdministradorProyectoProyecto
            {
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Persona = item.Persona,
                AdministradorProyecto = item.AdministradorProyecto,
                Proyecto = proyecto
            });
        }

        public static IQueryable<JoinIdentidadPerfilOrg> JoinPerfilOrganizacion(this IQueryable<EntityModel.Models.IdentidadDS.Identidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.PerfilOrganizacion, identidad => identidad.PerfilID, perfilOrganizacion => perfilOrganizacion.PerfilID, (identidad, perfilOrganizacion) => new JoinIdentidadPerfilOrg
            {
                Identidad = identidad,
                PerfilOrganizacion = perfilOrganizacion
            });
        }

        public static IQueryable<JoinIdentidadPerfilOrganizacionPerfilPersonaOrg> JoinPerfilPersonaOrganizacion(this IQueryable<JoinIdentidadPerfilOrg> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.PerfilPersonaOrg, perfilOrganizacion => perfilOrganizacion.PerfilOrganizacion.OrganizacionID, perfilPersonaOrg => perfilPersonaOrg.OrganizacionID, (item, perfilPersonaOrg) => new JoinIdentidadPerfilOrganizacionPerfilPersonaOrg
            {
                Identidad = item.Identidad,
                PerfilOrganizacion = item.PerfilOrganizacion,
                PerfilPersonaOrg = perfilPersonaOrg
            });
        }

        public static IQueryable<JoinIdentidadPerfil> JoinPerfil(this IQueryable<EntityModel.Models.IdentidadDS.Identidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, identidad => identidad.PerfilID, perfil => perfil.PerfilID, (identidad, perfil) => new JoinIdentidadPerfil
            {
                Identidad = identidad,
                Perfil = perfil
            });
        }

        public static IQueryable<JoinTesauroUsuarioPersonaPerfilIdentidadCategoriaTesauro> JoinCategoriaTesauro(this IQueryable<JoinTesauroUsuarioPersonaPerfilIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CategoriaTesauro, item => item.TesauroUsuario.TesauroID, categoriaTesauro => categoriaTesauro.TesauroID, (item, categoriaTesauro) => new JoinTesauroUsuarioPersonaPerfilIdentidadCategoriaTesauro
            {
                Perfil = item.Perfil,
                Persona = item.Persona,
                TesauroUsuario = item.TesauroUsuario,
                Identidad = item.Identidad,
                CategoriaTesauro = categoriaTesauro
            });
        }

        public static IQueryable<JoinTesauroUsuarioPersonaPerfilIdentidad> JoinIdentidad(this IQueryable<JoinTesauroUsuarioPersonaPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Perfil.PerfilID, identidad => identidad.PerfilID, (item, identidad) => new JoinTesauroUsuarioPersonaPerfilIdentidad
            {
                Perfil = item.Perfil,
                Persona = item.Persona,
                TesauroUsuario = item.TesauroUsuario,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinTesauroUsuarioPersonaPerfil> JoinPerfil(this IQueryable<JoinTesauroUsuarioPersona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Persona.PersonaID, perfil => perfil.PersonaID, (item, perfil) => new JoinTesauroUsuarioPersonaPerfil
            {
                Perfil = perfil,
                Persona = item.Persona,
                TesauroUsuario = item.TesauroUsuario
            });
        }

        public static IQueryable<JoinTesauroUsuarioPersona> JoinPersona(this IQueryable<EntityModel.Models.Tesauro.TesauroUsuario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Persona, tesauroUsuario => tesauroUsuario.UsuarioID, persona => persona.UsuarioID, (tesauroUsuario, persona) => new JoinTesauroUsuarioPersona
            {
                Persona = persona,
                TesauroUsuario = tesauroUsuario
            });
        }

        public static IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosUsuario> JoinBaseRecursosUsuario(this IQueryable<JoinDocumentoDocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.BaseRecursosUsuario, item => item.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecusosUsuario => baseRecusosUsuario.BaseRecursosID, (item, baseRecusosUsuario) => new JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosUsuario
            {
                Documento = item.Documento,
                BaseRecursosUsuario = baseRecusosUsuario,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosUsuarioIdentidad> JoinIdentidad(this IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosUsuario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.DocumentoWebVinBaseRecursos.IdentidadPublicacionID.Value, identidad => identidad.IdentidadID, (item, identidad) => new JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosUsuarioIdentidad
            {
                Documento = item.Documento,
                BaseRecursosUsuario = item.BaseRecursosUsuario,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosUsuarioIdentidadPerfil> JoinPerfil(this IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosUsuarioIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosUsuarioIdentidadPerfil
            {
                Documento = item.Documento,
                BaseRecursosUsuario = item.BaseRecursosUsuario,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                Identidad = item.Identidad,
                Perfil = perfil
            });
        }

        public static IQueryable<JoinIdentidadProyectoPerfilIdentidadProyecto> JoinProyecto(this IQueryable<JoinIdentidadProyectoPerfilIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Proyecto, item => item.IdentidadNoMyGnoss.ProyectoID, proyecto => proyecto.ProyectoID, (item, proyecto) => new JoinIdentidadProyectoPerfilIdentidadProyecto
            {
                Idenidad = item.Identidad,
                Proyecto = item.Proyecto,
                IdentidadNoMyGnoss = item.IdentidadNoMyGnoss,
                Perfil = item.Perfil,
                ProyectoNoMyGnoss = proyecto
            });
        }

        public static IQueryable<JoinIdentidadProyectoPerfilIdentidad> JoinIdentidad(this IQueryable<JoinIdentidadProyectoPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Perfil.PerfilID, identidad => identidad.PerfilID, (item, identidad) => new JoinIdentidadProyectoPerfilIdentidad
            {
                Identidad = item.Identidad,
                IdentidadNoMyGnoss = identidad,
                Perfil = item.Perfil,
                Proyecto = item.Proyecto
            });
        }

        public static IQueryable<JoinIdentidadProyectoPerfilIdentidadOrgComunidades> JoinIndetidadOrgComunidades(this IQueryable<JoinIdentidadProyectoPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => new JoinOrganizacionProyecto { OrganizacionID = item.Perfil.OrganizacionID.Value, ProyectoID = item.Identidad.ProyectoID }, identidadOrgComunidades => new JoinOrganizacionProyecto { OrganizacionID = identidadOrgComunidades.OrganizacionID, ProyectoID = identidadOrgComunidades.ProyectoID }, (item, identidadOrgComunidades) => new JoinIdentidadProyectoPerfilIdentidadOrgComunidades
            {
                Identidad = item.Identidad,
                IdentidadOrgComunidades = identidadOrgComunidades,
                Perfil = item.Perfil,
                Proyecto = item.Proyecto
            });
        }

        public class JoinOrganizacionProyecto
        {
            public Guid OrganizacionID { get; set; }
            public Guid ProyectoID { get; set; }
        }

        public static IQueryable<JoinIdentidadProyectoPerfil> JoinPerfil(this IQueryable<JoinIdentidadProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new JoinIdentidadProyectoPerfil
            {
                Perfil = perfil,
                Identidad = item.Identidad,
                Proyecto = item.Proyecto
            });
        }

        public static IQueryable<JoinIdentidadProyectoPerfilPersona> JoinPersona(this IQueryable<JoinIdentidadProyectoPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Persona, item => item.Perfil.PersonaID, persona => persona.PersonaID, (item, persona) => new JoinIdentidadProyectoPerfilPersona
            {
                Perfil = item.Perfil,
                Identidad = item.Identidad,
                Proyecto = item.Proyecto,
                Persona = persona
            });
        }

        public static IQueryable<JoinSuscripcionIdentidadProyectoSuscripcionIdentidadIdentidad2Identidad3> JoinIdentidad3(this IQueryable<JoinSuscripcionIdentidadProyectoSuscripcionIdentidadIdentidad2> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, join => join.Identidad.PerfilID, ident3 => ident3.PerfilID, (join, ident3) =>
            new JoinSuscripcionIdentidadProyectoSuscripcionIdentidadIdentidad2Identidad3
            {
                Suscripcion = join.Suscripcion,
                SuscripcionIdentidadProyecto = join.SuscripcionIdentidadProyecto,
                Identidad = join.Identidad,
                Identidad2 = join.Identidad2,
                Identidad3 = ident3
            });
        }
        public static IQueryable<JoinSuscripcionIdentidadProyectoSuscripcionIdentidadIdentidad2> JoinIdentidad2(this IQueryable<JoinSuscripcionIdentidadProyectoSuscripcionIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, join => join.Suscripcion.IdentidadID, ident2 => ident2.IdentidadID, (join, ident2) =>
            new JoinSuscripcionIdentidadProyectoSuscripcionIdentidadIdentidad2
            {
                Suscripcion = join.Suscripcion,
                SuscripcionIdentidadProyecto = join.SuscripcionIdentidadProyecto,
                Identidad = join.Identidad,
                Identidad2 = ident2
            });
        }

        public static IQueryable<JoinSuscripcionIdentidadProyectoSuscripcionIdentidadIdentidadEnProyecto> JoinIdentidadEnProyecto(this IQueryable<JoinSuscripcionIdentidadProyectoSuscripcionIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, item => item.Identidad.PerfilID, identidadEnProyecto => identidadEnProyecto.PerfilID, (item, identidadEnProyecto) =>
            new JoinSuscripcionIdentidadProyectoSuscripcionIdentidadIdentidadEnProyecto
            {
                Suscripcion = item.Suscripcion,
                SuscripcionIdentidadProyecto = item.SuscripcionIdentidadProyecto,
                Identidad = item.Identidad,
                IdentidadEnProyecto = identidadEnProyecto
            });
        }

        public static IQueryable<JoinSuscripcionIdentidadProyectoSuscripcionIdentidad> JoinIdentidad(this IQueryable<JoinSuscripcionIdentidadProyectoSuscripcion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, join => join.SuscripcionIdentidadProyecto.IdentidadID, ident => ident.IdentidadID, (join, ident) =>
            new JoinSuscripcionIdentidadProyectoSuscripcionIdentidad
            {
                Suscripcion = join.Suscripcion,
                SuscripcionIdentidadProyecto = join.SuscripcionIdentidadProyecto,
                Identidad = ident
            });
        }
        public static IQueryable<JoinIdentidadSuscripcionIdentidad2Identidad3SuscripcionIdentidadProyecto> JoinSuscripcionIdentidadProyecto(this IQueryable<JoinIdentidadSuscripcionIdentidad2Identidad3> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.SuscripcionIdentidadProyecto, suscrIdent => suscrIdent.Identidad.IdentidadID, suscrProyecto => suscrProyecto.IdentidadID, (suscrIdent, suscrProyecto) =>
            new JoinIdentidadSuscripcionIdentidad2Identidad3SuscripcionIdentidadProyecto
            {
                Suscripcion = suscrIdent.Suscripcion,
                Identidad = suscrIdent.Identidad,
                Identidad2 = suscrIdent.Identidad2,
                Identidad3 = suscrIdent.Identidad3,
                SuscripcionIdentidadProyecto = suscrProyecto
            });
        }

        public static IQueryable<JoinPerfilIdentidadPerfil2> JoinPerfil2(this IQueryable<JoinPerfilIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Perfil, join => join.Perfil.PersonaID, perifl2 => perifl2.PersonaID, (join, perifl2) =>
            new JoinPerfilIdentidadPerfil2
            {
                Perfil = join.Perfil,
                Identidad = join.Identidad,
                Perfil2 = perifl2
            });
        }

        public static IQueryable<JoinComentarioIdentidad> JoinIdentidad(this IQueryable<EntityModel.Models.Comentario.Comentario> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, comentario => comentario.IdentidadID, ident => ident.IdentidadID, (comentario, ident) =>
            new JoinComentarioIdentidad
            {
                Comentario = comentario,
                Identidad = ident
            });
        }

        public static IQueryable<JoinComentarioIdentidadPerfil> JoinPerfil(this IQueryable<JoinComentarioIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) =>
            new JoinComentarioIdentidadPerfil
            {
                Comentario = item.Comentario,
                Identidad = item.Identidad,
                Perfil = perfil
            });
        }

        public static IQueryable<JoinComentarioIdentidadPerfilIdentidadMyGnoss> JoinIdentidadMyGnoss(this IQueryable<JoinComentarioIdentidadPerfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, item => item.Perfil.PerfilID, identidadMyGnoss => identidadMyGnoss.PerfilID, (item, identidadMyGnoss) =>
            new JoinComentarioIdentidadPerfilIdentidadMyGnoss
            {
                Comentario = item.Comentario,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                IdentidadMyGnoss = identidadMyGnoss
            });
        }

        public static IQueryable<JoinComentarioIdentidadDocumentoComentario> JoinDocumentoComentario(this IQueryable<JoinComentarioIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.DocumentoComentario, item => item.Comentario.ComentarioID, documentoComentario => documentoComentario.ComentarioID, (item, documentoComentario) =>
            new JoinComentarioIdentidadDocumentoComentario
            {
                Comentario = item.Comentario,
                Identidad = item.Identidad,
                DocumentoComentario = documentoComentario
            });
        }

        public static IQueryable<JoinComentarioIdentidadDocumentoComentarioDocumentoWebVinBaseRecursos> JoinDocumentoWebVinBaseRecursos(this IQueryable<JoinComentarioIdentidadDocumentoComentario> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.DocumentoWebVinBaseRecursos, item => item.DocumentoComentario.DocumentoID, documentoWebVinBaseRecursos => documentoWebVinBaseRecursos.DocumentoID, (item, documentoWebVinBaseRecursos) =>
            new JoinComentarioIdentidadDocumentoComentarioDocumentoWebVinBaseRecursos
            {
                Comentario = item.Comentario,
                Identidad = item.Identidad,
                DocumentoComentario = item.DocumentoComentario,
                DocumentoWebVinBaseRecursos = documentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinComentarioIdentidadDocumentoComentarioDocumentoWebVinBaseRecursosBaseRecursosProyecto> JoinBaseRecursosProyecto(this IQueryable<JoinComentarioIdentidadDocumentoComentarioDocumentoWebVinBaseRecursos> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.BaseRecursosProyecto, item => item.Identidad.ProyectoID, baseRecursosProyecto => baseRecursosProyecto.ProyectoID, (item, baseRecursosProyecto) =>
            new JoinComentarioIdentidadDocumentoComentarioDocumentoWebVinBaseRecursosBaseRecursosProyecto
            {
                Comentario = item.Comentario,
                Identidad = item.Identidad,
                DocumentoComentario = item.DocumentoComentario,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = baseRecursosProyecto
            });
        }

        public static IQueryable<JoinComentarioIdentidadDocumentoComentarioDocumento> JoinDocumento(this IQueryable<JoinComentarioIdentidadDocumentoComentario> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Documento, item => item.DocumentoComentario.DocumentoID, documento => documento.DocumentoID, (item, documento) =>
            new JoinComentarioIdentidadDocumentoComentarioDocumento
            {
                Comentario = item.Comentario,
                Identidad = item.Identidad,
                DocumentoComentario = item.DocumentoComentario,
                Documento = documento
            });
        }

        public static IQueryable<JoinComentarioIdentidadDocumentoComentarioDocumentoDocumentoWebVinBaseRecursos> JoinDocumentoWebVinBaseRecursos(this IQueryable<JoinComentarioIdentidadDocumentoComentarioDocumento> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.DocumentoWebVinBaseRecursos, item => item.DocumentoComentario.DocumentoID, documentoWebVinBaseRecursos => documentoWebVinBaseRecursos.DocumentoID, (item, documentoWebVinBaseRecursos) =>
            new JoinComentarioIdentidadDocumentoComentarioDocumentoDocumentoWebVinBaseRecursos
            {
                Comentario = item.Comentario,
                Identidad = item.Identidad,
                DocumentoComentario = item.DocumentoComentario,
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = documentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinComentarioIdentidadDocumentoComentarioDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyecto> JoinBaseRecursosProyecto(this IQueryable<JoinComentarioIdentidadDocumentoComentarioDocumentoDocumentoWebVinBaseRecursos> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.BaseRecursosProyecto, item => item.Identidad.ProyectoID, baseRecursosProyecto => baseRecursosProyecto.ProyectoID, (item, baseRecursosProyecto) =>
            new JoinComentarioIdentidadDocumentoComentarioDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyecto
            {
                Comentario = item.Comentario,
                Identidad = item.Identidad,
                DocumentoComentario = item.DocumentoComentario,
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = baseRecursosProyecto
            });
        }

        public static IQueryable<JoinComentarioIdentidadPerfilVotoComentario> JoinVotoComentario(this IQueryable<JoinComentarioIdentidadPerfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.VotoComentario, item => item.Comentario.ComentarioID, votoComentario => votoComentario.ComentarioID, (item, votoComentario) =>
            new JoinComentarioIdentidadPerfilVotoComentario
            {
                Comentario = item.Comentario,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                VotoComentario = votoComentario
            });
        }

        public static IQueryable<JoinComentarioIdentidadPerfilDocumentoComentario> JoinDocumentoComentario(this IQueryable<JoinComentarioIdentidadPerfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.DocumentoComentario, item => item.Comentario.ComentarioID, documentoComentario => documentoComentario.ComentarioID, (item, documentoComentario) =>
            new JoinComentarioIdentidadPerfilDocumentoComentario
            {
                Comentario = item.Comentario,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                DocumentoComentario = documentoComentario
            });
        }

        public static IQueryable<JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoWebVinBaseRecuros> JoinDocumentoWebVinBaseRecursos(this IQueryable<JoinComentarioIdentidadPerfilDocumentoComentario> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.DocumentoWebVinBaseRecursos, item => item.DocumentoComentario.DocumentoID, documentoWebVinBaseRecursos => documentoWebVinBaseRecursos.DocumentoID, (item, documentoWebVinBaseRecursos) =>
            new JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoWebVinBaseRecuros
            {
                Comentario = item.Comentario,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                DocumentoComentario = item.DocumentoComentario,
                DocumentoWebVinBaseRecursos = documentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoWebVinBaseRecurosBaseRecursosProyecto> JoinBaseRercursosProyecto(this IQueryable<JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoWebVinBaseRecuros> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.BaseRecursosProyecto, item => item.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecursosProyecto => baseRecursosProyecto.BaseRecursosID, (item, baseRecursosProyecto) =>
            new JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoWebVinBaseRecurosBaseRecursosProyecto
            {
                Comentario = item.Comentario,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                DocumentoComentario = item.DocumentoComentario,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = baseRecursosProyecto
            });
        }

        public static IQueryable<JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoWebVinBaseRecurosBaseRecursosProyectoDocumento> JoinDocumento(this IQueryable<JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoWebVinBaseRecurosBaseRecursosProyecto> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Documento, item => item.DocumentoWebVinBaseRecursos.DocumentoID, documento => documento.DocumentoID, (item, documento) =>
            new JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoWebVinBaseRecurosBaseRecursosProyectoDocumento
            {
                Comentario = item.Comentario,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                DocumentoComentario = item.DocumentoComentario,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = item.BaseRecursosProyecto,
                Documento = documento
            });
        }

        public static IQueryable<JoinDocumentoWebVinBaseRecursosIdentidadBaseRecursosProyecto> JoinBaseRecursosProyecto(this IQueryable<JoinDocumentoWebVinBaseRecursosIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.BaseRecursosProyecto, join => join.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecursos => baseRecursos.BaseRecursosID, (join, baseRecursos) =>
            new JoinDocumentoWebVinBaseRecursosIdentidadBaseRecursosProyecto
            {
                DocumentoWebVinBaseRecursos = join.DocumentoWebVinBaseRecursos,
                Identidad = join.Identidad,
                BaseRecursosProyecto = baseRecursos
            });
        }
        public static IQueryable<JoinDocumentoWebVinBaseRecursosIdentidad> JoinIdentidad(this IQueryable<EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, documento => documento.IdentidadPublicacionID, ident => ident.IdentidadID, (documento, ident) =>
            new JoinDocumentoWebVinBaseRecursosIdentidad
            {
                DocumentoWebVinBaseRecursos = documento,
                Identidad = ident
            });
        }
        public static IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyecto> JoinBaseRecursosProyecto(this IQueryable<JoinDocumentoDocumentoWebVinBaseRecursos> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.BaseRecursosProyecto, join => join.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecursos => baseRecursos.BaseRecursosID, (join, baseRecursos) =>
            new JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyecto
            {
                Documento = join.Documento,
                DocumentoWebVinBaseRecursos = join.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = baseRecursos
            });
        }

        public static IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoNivelCertificacion> JoinNivelCertificacion(this IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyecto> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.NivelCertificacion, join => join.DocumentoWebVinBaseRecursos.NivelCertificacionID, nivelCertificacion => nivelCertificacion.NivelCertificacionID, (join, nivelCertificacion) =>
            new JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoNivelCertificacion
            {
                Documento = join.Documento,
                DocumentoWebVinBaseRecursos = join.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = join.BaseRecursosProyecto,
                NivelCertificacion = nivelCertificacion
            });
        }

        public static IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidad> JoinIdentidad(this IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyecto> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, join => join.DocumentoWebVinBaseRecursos.IdentidadPublicacionID, identidad => identidad.IdentidadID, (join, identidad) =>
            new JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidad
            {
                Documento = join.Documento,
                DocumentoWebVinBaseRecursos = join.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = join.BaseRecursosProyecto,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidad> JoinIdentidadDocumento(this IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyecto> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, join => join.Documento.CreadorID, identidad => identidad.IdentidadID, (join, identidad) =>
            new JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidad
            {
                Documento = join.Documento,
                DocumentoWebVinBaseRecursos = join.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = join.BaseRecursosProyecto,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoProyecto> JoinProyecto(this IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyecto> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Proyecto, join => join.BaseRecursosProyecto.ProyectoID, proyecto => proyecto.ProyectoID, (join, proyecto) =>
            new JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoProyecto
            {
                Documento = join.Documento,
                DocumentoWebVinBaseRecursos = join.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = join.BaseRecursosProyecto,
                Proyecto = proyecto
            });
        }

        public static IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidadPerfil> JoinPerfil(this IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Perfil, join => join.Identidad.PerfilID, perfil => perfil.PerfilID, (join, perfil) =>
            new JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidadPerfil
            {
                Documento = join.Documento,
                DocumentoWebVinBaseRecursos = join.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = join.BaseRecursosProyecto,
                Identidad = join.Identidad,
                Perfil = perfil
            });
        }

        public static IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidadPerfilProyecto> JoinProyecto(this IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidadPerfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Proyecto, join => join.BaseRecursosProyecto.ProyectoID, proyecto => proyecto.ProyectoID, (join, proyecto) =>
            new JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidadPerfilProyecto
            {
                Documento = join.Documento,
                DocumentoWebVinBaseRecursos = join.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = join.BaseRecursosProyecto,
                Identidad = join.Identidad,
                Perfil = join.Perfil,
                Proyecto = proyecto
            });
        }

        public static IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidadPerfilDocumentoRolIdentidad> JoinDocumentoRolIdentidad(this IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidadPerfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.DocumentoRolIdentidad, join => join.Documento.DocumentoID, documentoRolIdentidad => documentoRolIdentidad.DocumentoID, (join, documentoRolIdentidad) =>
            new JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidadPerfilDocumentoRolIdentidad
            {
                Documento = join.Documento,
                DocumentoWebVinBaseRecursos = join.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = join.BaseRecursosProyecto,
                Identidad = join.Identidad,
                Perfil = join.Perfil,
                DocumentoRolIdentidad = documentoRolIdentidad
            });
        }
        public static IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidadPerfilDocumentoRolIdentidadIdentidad> JoinIdentidad(this IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidadPerfilDocumentoRolIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, join => join.DocumentoRolIdentidad.PerfilID, identidadEditor => identidadEditor.PerfilID, (join, identidadEditor) =>
            new JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidadPerfilDocumentoRolIdentidadIdentidad
            {
                Documento = join.Documento,
                DocumentoWebVinBaseRecursos = join.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = join.BaseRecursosProyecto,
                Identidad = join.Identidad,
                Perfil = join.Perfil,
                DocumentoRolIdentidad = join.DocumentoRolIdentidad,
                IdentidadEditor = identidadEditor
            });
        }


        public static IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidadPerfilDocumentoRolIdentidadIdentidadPerfil> JoinPerfil(this IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidadPerfilDocumentoRolIdentidadIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Perfil, join => join.DocumentoRolIdentidad.PerfilID, perfilEditor => perfilEditor.PerfilID, (join, perfilEditor) =>
            new JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoIdentidadPerfilDocumentoRolIdentidadIdentidadPerfil
            {
                Documento = join.Documento,
                DocumentoWebVinBaseRecursos = join.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = join.BaseRecursosProyecto,
                Identidad = join.Identidad,
                Perfil = join.Perfil,
                DocumentoRolIdentidad = join.DocumentoRolIdentidad,
                IdentidadEditor = join.IdentidadEditor,
                PerfilEditor = perfilEditor
            });
        }

        public static IQueryable<JoinIdentidadIdentidad> JoinIdentidad(this IQueryable<EntityModel.Models.IdentidadDS.Identidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, identidadProyecto => identidadProyecto.PerfilID, identidadPerfil => identidadPerfil.PerfilID, (identidadProyecto, identidadPerfil) =>
                new JoinIdentidadIdentidad
                {
                    IdentidadProyecto = identidadProyecto,
                    IdentidadPerfil = identidadPerfil
                });
        }
        public static IQueryable<JoinPerfilOrganizacionIdentidadOrganizacion> JoinOrganizacion(this IQueryable<JoinPerfilOrganizacionIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Organizacion, perfIdent => perfIdent.PerfilOrganizacion.OrganizacionID, organizacion => organizacion.OrganizacionID, (perfIdent, organizacion) =>
            new JoinPerfilOrganizacionIdentidadOrganizacion
            {
                PerfilOrganizacion = perfIdent.PerfilOrganizacion,
                Identidad = perfIdent.Identidad,
                Organizacion = organizacion
            });
        }
        public static IQueryable<JoinPerfilPersonaOrgIdentidadPersonaUsuario> JoinUsuario(this IQueryable<JoinPerfilPersonaOrgIdentidadPersona> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Usuario, join => join.Persona.UsuarioID, usuario => usuario.UsuarioID, (join, usuario) =>
            new JoinPerfilPersonaOrgIdentidadPersonaUsuario
            {
                PerfilPersonaOrg = join.PerfilPersonaOrg,
                Identidad = join.Identidad,
                Persona = join.Persona,
                Usuario = usuario
            });
        }

        public static IQueryable<JoinPerfilIdentidadPersonaPerfilPersonaProfesor> JoinProfesor(this IQueryable<JoinPerfilIdentidadPersonaPerfilPersona> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Profesor, join => join.Perfil.PerfilID, profesor => profesor.PerfilID, (join, profesor) =>
            new JoinPerfilIdentidadPersonaPerfilPersonaProfesor
            {
                Perfil = join.Perfil,
                Identidad = join.Identidad,
                Persona = join.Persona,
                PerfilPersona = join.PerfilPersona,
                Profesor = profesor
            });
        }
        public static IQueryable<JoinPerfilPersonaOrgIdentidadPersonaVisibleEnOrg> JoinPersonaVisibleEnOrg(this IQueryable<JoinPerfilPersonaOrgIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.PersonaVisibleEnOrg, perfilIdentidad => new { PersonaID = perfilIdentidad.PerfilPersonaOrg.PersonaID, OrganizacionID = perfilIdentidad.PerfilPersonaOrg.OrganizacionID }, visible => new { PersonaID = visible.PersonaID, OrganizacionID = visible.OrganizacionID }, (perfilIdentidad, visible) =>
            new JoinPerfilPersonaOrgIdentidadPersonaVisibleEnOrg
            {
                PerfilPersonaOrg = perfilIdentidad.PerfilPersonaOrg,
                Identidad = perfilIdentidad.Identidad,
                PersonaVisibleEnOrg = visible
            });
        }
        public static IQueryable<JoinPerfilIdentidadPersonaVisibleEnOrg> JoinPersonaVisibleEnOrg(this IQueryable<JoinPerfilIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.PersonaVisibleEnOrg, perfilIdentidad => new { PersonaID = perfilIdentidad.Perfil.PersonaID.Value, OrganizacionID = perfilIdentidad.Perfil.OrganizacionID.Value }, visible => new { PersonaID = visible.PersonaID, OrganizacionID = visible.OrganizacionID }, (perfilIdentidad, visible) =>
            new JoinPerfilIdentidadPersonaVisibleEnOrg
            {
                Perfil = perfilIdentidad.Perfil,
                Identidad = perfilIdentidad.Identidad,
                PersonaVisibleEnOrg = visible
            });
        }
        public static IQueryable<JoinGrupoIdentidadesParticipacionIdentidadIdentidad> JoinIdentidad2(this IQueryable<JoinGrupoIdentidadesParticipacionIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, particIdent => particIdent.Identidad.PerfilID, identidad2 => identidad2.PerfilID, (particIdent, identidad2) =>
            new JoinGrupoIdentidadesParticipacionIdentidadIdentidad
            {
                GrupoIdentidadesParticipacion = particIdent.GrupoIdentidadesParticipacion,
                Identidad = particIdent.Identidad,
                Identidad2 = identidad2
            });
        }

        public static IQueryable<JoinGrupoIdentGrupoIdentOrganizacionGrupoIdentidadesParticipacionIdentidadPerfilIdentidad> JoinIdentidad2(this IQueryable<JoinGrupoIdentGrupoIdentOrganizacionGrupoIdentidadesParticipacionIdentidadPerfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, join => join.Perfil.PerfilID, identidad2 => identidad2.PerfilID, (join, identidad2) =>
            new JoinGrupoIdentGrupoIdentOrganizacionGrupoIdentidadesParticipacionIdentidadPerfilIdentidad
            {
                GrupoIdentidades = join.GrupoIdentidades,
                GrupoIdentidadesOrganizacion = join.GrupoIdentidadesOrganizacion,
                GrupoIdentidadesParticipacion = join.GrupoIdentidadesParticipacion,
                Identidad = join.Identidad,
                Perfil = join.Perfil,
                Identidad2 = identidad2
            });
        }
        public static IQueryable<JoinGrupoIdentGrupoIdentOrganizacionGrupoIdentidadesParticipacionIdentidadPerfil> JoinPerfil(this IQueryable<JoinGrupoIdentGrupoIdentOrganizacionGrupoIdentidadesParticipacionIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Perfil, join => join.Identidad.PerfilID, perfil => perfil.PerfilID, (join, perfil) =>
            new JoinGrupoIdentGrupoIdentOrganizacionGrupoIdentidadesParticipacionIdentidadPerfil
            {
                GrupoIdentidades = join.GrupoIdentidades,
                GrupoIdentidadesOrganizacion = join.GrupoIdentidadesOrganizacion,
                GrupoIdentidadesParticipacion = join.GrupoIdentidadesParticipacion,
                Identidad = join.Identidad,
                Perfil = perfil
            });
        }
        public static IQueryable<JoinGrupoIdentGrupoIdentOrganizacionGrupoIdentidadesParticipacionIdentidad> JoinIdentidad(this IQueryable<JoinGrupoIdentGrupoIdentOrganizacionGrupoIdentidadesParticipacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, join => join.GrupoIdentidadesParticipacion.IdentidadID, identidad => identidad.IdentidadID, (join, identidad) =>
            new JoinGrupoIdentGrupoIdentOrganizacionGrupoIdentidadesParticipacionIdentidad
            {
                GrupoIdentidades = join.GrupoIdentidades,
                GrupoIdentidadesOrganizacion = join.GrupoIdentidadesOrganizacion,
                GrupoIdentidadesParticipacion = join.GrupoIdentidadesParticipacion,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinGrupoIdentGrupoIdentOrganizacionGrupoIdentidadesParticipacion> JoinGrupoIdentidadesParticipacion(this IQueryable<JoinGrupoIdentGrupoIdentOrganizacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.GrupoIdentidadesParticipacion, join => join.GrupoIdentidades.GrupoID, participacion => participacion.GrupoID, (join, participacion) =>
            new JoinGrupoIdentGrupoIdentOrganizacionGrupoIdentidadesParticipacion
            {
                GrupoIdentidades = join.GrupoIdentidades,
                GrupoIdentidadesOrganizacion = join.GrupoIdentidadesOrganizacion,
                GrupoIdentidadesParticipacion = participacion
            });
        }

        public static IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosIdentidad> JoinIdentidad(this IQueryable<JoinDocumentoDocumentoWebVinBaseRecursos> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, join => join.DocumentoWebVinBaseRecursos.IdentidadPublicacionID, identidad => identidad.IdentidadID, (join, identidad) =>
            new JoinDocumentoDocumentoWebVinBaseRecursosIdentidad
            {
                Documento = join.Documento,
                DocumentoWebVinBaseRecursos = join.DocumentoWebVinBaseRecursos,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosIdentidadPerfil> JoinPerfil(this IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Perfil, join => join.Identidad.PerfilID, perfil => perfil.PerfilID, (join, perfil) =>
            new JoinDocumentoDocumentoWebVinBaseRecursosIdentidadPerfil
            {
                Documento = join.Documento,
                DocumentoWebVinBaseRecursos = join.DocumentoWebVinBaseRecursos,
                Identidad = join.Identidad,
                Perfil = perfil
            });
        }

        public static IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosIdentidadPerfilPerfil> JoinPerfil(this IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosIdentidadPerfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Perfil, join => join.Perfil.PerfilID, perfilPersona => perfilPersona.PerfilID, (join, perfilPersona) =>
            new JoinDocumentoDocumentoWebVinBaseRecursosIdentidadPerfilPerfil
            {
                Documento = join.Documento,
                DocumentoWebVinBaseRecursos = join.DocumentoWebVinBaseRecursos,
                Identidad = join.Identidad,
                Perfil = join.Perfil,
                PerfilPersona = perfilPersona
            });
        }

        public static IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosIdentidadPerfilIdentidad> JoinIdentidad(this IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosIdentidadPerfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, join => join.Perfil.PerfilID, identidad => identidad.PerfilID, (join, identidad) =>
            new JoinDocumentoDocumentoWebVinBaseRecursosIdentidadPerfilIdentidad
            {
                Documento = join.Documento,
                DocumentoWebVinBaseRecursos = join.DocumentoWebVinBaseRecursos,
                Identidad = join.Identidad,
                Perfil = join.Perfil,
                IdentidadMyGnoss = identidad
            });
        }

        public static IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosIdentidadPerfilConfiguracionGnossPersona> JoinConfiguracionGnossPersona(this IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosIdentidadPerfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.ConfiguracionGnossPersona, join => join.Perfil.PersonaID.Value, configuracionGnossPersona => configuracionGnossPersona.PersonaID, (join, configuracionGnossPersona) =>
            new JoinDocumentoDocumentoWebVinBaseRecursosIdentidadPerfilConfiguracionGnossPersona
            {
                Documento = join.Documento,
                DocumentoWebVinBaseRecursos = join.DocumentoWebVinBaseRecursos,
                Identidad = join.Identidad,
                Perfil = join.Perfil,
                ConfiguracionGnossPersona = configuracionGnossPersona
            });
        }

        public static IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosIdentidadPerfilConfiguracionGnossPersonaBaseRecursoProyecto> JoinBaseRecursosProyecto(this IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosIdentidadPerfilConfiguracionGnossPersona> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.BaseRecursosProyecto, join => join.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecursosProyecto => baseRecursosProyecto.BaseRecursosID, (join, baseRecursosProyecto) =>
            new JoinDocumentoDocumentoWebVinBaseRecursosIdentidadPerfilConfiguracionGnossPersonaBaseRecursoProyecto
            {
                Documento = join.Documento,
                DocumentoWebVinBaseRecursos = join.DocumentoWebVinBaseRecursos,
                Identidad = join.Identidad,
                Perfil = join.Perfil,
                ConfiguracionGnossPersona = join.ConfiguracionGnossPersona,
                BaseRecursosProyecto = baseRecursosProyecto
            });
        }

        public static IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosIdentidadPerfilConfiguracionGnossPersonaBaseRecursoProyectoProyecto> JoinProyecto(this IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosIdentidadPerfilConfiguracionGnossPersonaBaseRecursoProyecto> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Proyecto, join => join.BaseRecursosProyecto.ProyectoID, proyecto => proyecto.ProyectoID, (join, proyecto) =>
             new JoinDocumentoDocumentoWebVinBaseRecursosIdentidadPerfilConfiguracionGnossPersonaBaseRecursoProyectoProyecto
             {
                 Documento = join.Documento,
                 DocumentoWebVinBaseRecursos = join.DocumentoWebVinBaseRecursos,
                 Identidad = join.Identidad,
                 Perfil = join.Perfil,
                 ConfiguracionGnossPersona = join.ConfiguracionGnossPersona,
                 BaseRecursosProyecto = join.BaseRecursosProyecto,
                 Proyecto = proyecto
             });
        }

        public static IQueryable<JoinDocumentoDocumentoWebVinBaseRecursos> JoinDocumentoWebVinBaseRecursos(this IQueryable<Documento> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.DocumentoWebVinBaseRecursos, documento => documento.DocumentoID, docweb => docweb.DocumentoID, (documento, docweb) =>
            new JoinDocumentoDocumentoWebVinBaseRecursos
            {
                Documento = documento,
                DocumentoWebVinBaseRecursos = docweb
            });
        }

        public static IQueryable<JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosOrganizacion> JoinBaseRecursosOrganizacion(this IQueryable<JoinDocumentoDocumentoWebVinBaseRecursos> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.BaseRecursosOrganizacion, item => item.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecursosOrganizacion => baseRecursosOrganizacion.BaseRecursosID, (item, baseRecursosOrganizacion) =>
            new JoinDocumentoDocumentoWebVinBaseRecursosBaseRecursosOrganizacion
            {
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                BaseRecursosOrganizacion = baseRecursosOrganizacion
            });
        }

        public static IQueryable<JoinDocumentoDocumento> JoinDocumentoVinc(this IQueryable<Documento> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Documento, documento => documento.DocumentoID, docweb => docweb.ElementoVinculadoID, (documento, docweb) =>
            new JoinDocumentoDocumento
            {
                Documento = documento,
                DocumentoVinc = docweb
            });
        }


        public static IQueryable<JoinGrupoIdentidadesParticipacionIdentidadDocumentoRolGrupoIdentidadesDocumentoWebVinBaseRecursosDocumento> JoinDocumento(this IQueryable<JoinGrupoIdentidadesParticipacionIdentidadDocumentoRolGrupoIdentidadesDocumentoWebVinBaseRecursos> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Documento, join => join.DocumentoRolGrupoIdentidades.DocumentoID, documento => documento.DocumentoID, (join, documento) =>
            new JoinGrupoIdentidadesParticipacionIdentidadDocumentoRolGrupoIdentidadesDocumentoWebVinBaseRecursosDocumento
            {
                GrupoIdentidadesParticipacion = join.GrupoIdentidadesParticipacion,
                Identidad = join.Identidad,
                DocumentoRolGrupoIdentidades = join.DocumentoRolGrupoIdentidades,
                DocumentoWebVinBaseRecursos = join.DocumentoWebVinBaseRecursos,
                Documento = documento
            });
        }
        public static IQueryable<JoinGrupoIdentidadesParticipacionIdentidadDocumentoRolGrupoIdentidadesDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumento> JoinDocumento(this IQueryable<JoinGrupoIdentidadesParticipacionIdentidadDocumentoRolGrupoIdentidadesDocumentoWebVinBaseRecursosBaseRecursosProyecto> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Documento, join => join.DocumentoWebVinBaseRecursos.DocumentoID, documento => documento.DocumentoID, (join, documento) =>
            new JoinGrupoIdentidadesParticipacionIdentidadDocumentoRolGrupoIdentidadesDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumento
            {
                GrupoIdentidadesParticipacion = join.GrupoIdentidadesParticipacion,
                Identidad = join.Identidad,
                DocumentoRolGrupoIdentidades = join.DocumentoRolGrupoIdentidades,
                DocumentoWebVinBaseRecursos = join.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = join.BaseRecursosProyecto,
                Documento = documento
            });
        }
        public static IQueryable<JoinGrupoIdentidadesParticipacionIdentidadDocumentoRolGrupoIdentidadesDocumentoWebVinBaseRecursosBaseRecursosProyecto> JoinBaseRecursosProyecto(this IQueryable<JoinGrupoIdentidadesParticipacionIdentidadDocumentoRolGrupoIdentidadesDocumentoWebVinBaseRecursos> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.BaseRecursosProyecto, join => join.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecursos => baseRecursos.BaseRecursosID, (join, baseRecursos) =>
            new JoinGrupoIdentidadesParticipacionIdentidadDocumentoRolGrupoIdentidadesDocumentoWebVinBaseRecursosBaseRecursosProyecto
            {
                GrupoIdentidadesParticipacion = join.GrupoIdentidadesParticipacion,
                Identidad = join.Identidad,
                DocumentoRolGrupoIdentidades = join.DocumentoRolGrupoIdentidades,
                DocumentoWebVinBaseRecursos = join.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = baseRecursos
            });
        }
        public static IQueryable<JoinGrupoIdentidadesParticipacionIdentidadDocumentoRolGrupoIdentidadesDocumentoWebVinBaseRecursos> JoinDocumentoWebVinBaseRecursos(this IQueryable<JoinGrupoIdentidadesParticipacionIdentidadDocumentoRolGrupoIdentidades> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.DocumentoWebVinBaseRecursos, join => join.DocumentoRolGrupoIdentidades.DocumentoID, docweb => docweb.DocumentoID, (join, docweb) =>
            new JoinGrupoIdentidadesParticipacionIdentidadDocumentoRolGrupoIdentidadesDocumentoWebVinBaseRecursos
            {
                GrupoIdentidadesParticipacion = join.GrupoIdentidadesParticipacion,
                Identidad = join.Identidad,
                DocumentoRolGrupoIdentidades = join.DocumentoRolGrupoIdentidades,
                DocumentoWebVinBaseRecursos = docweb
            });
        }
        public static IQueryable<JoinGrupoIdentidadesParticipacionIdentidadDocumentoRolGrupoIdentidades> JoinDocumentoRolGrupoIdentidades(this IQueryable<JoinGrupoIdentidadesParticipacionIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.DocumentoRolGrupoIdentidades, particIdent => particIdent.GrupoIdentidadesParticipacion.GrupoID, docrol => docrol.GrupoID, (particIdent, docrol) =>
            new JoinGrupoIdentidadesParticipacionIdentidadDocumentoRolGrupoIdentidades
            {
                GrupoIdentidadesParticipacion = particIdent.GrupoIdentidadesParticipacion,
                Identidad = particIdent.Identidad,
                DocumentoRolGrupoIdentidades = docrol
            });
        }
        public static IQueryable<JoinGrupoIdentGrupoIdentidadesParticipacion> JoinGrupoIdentidadesParticipacion(this IQueryable<EntityModel.Models.IdentidadDS.GrupoIdentidades> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.GrupoIdentidadesParticipacion, grupoIdent => grupoIdent.GrupoID, participacion => participacion.GrupoID, (grupoIdent, participacion) =>
            new JoinGrupoIdentGrupoIdentidadesParticipacion
            {
                GrupoIdentidades = grupoIdent,
                GrupoIdentidadesParticipacion = participacion
            });
        }
        public static IQueryable<JoinGrupoIdentGrupoIdentidadesProyectoGrupoIdentidadesParticipacion> JoinGrupoIdentidadesParticipacion(this IQueryable<JoinGrupoIdentGrupoIdentidadesProyecto> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.GrupoIdentidadesParticipacion, identProy => identProy.GrupoIdentidades.GrupoID, participacion => participacion.GrupoID, (identProy, participacion) =>
            new JoinGrupoIdentGrupoIdentidadesProyectoGrupoIdentidadesParticipacion
            {
                GrupoIdentidades = identProy.GrupoIdentidades,
                GrupoIdentidadesProyecto = identProy.GrupoIdentidadesProyecto,
                GrupoIdentidadesParticipacion = participacion
            });
        }
        public static IQueryable<JoinGrupoIdentidadesParticipacionGrupoIdentidadesOrganizacion> JoinGrupoIdentidadesOrganizacion(this IQueryable<EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.GrupoIdentidadesOrganizacion, participacion => participacion.GrupoID, organizacion => organizacion.GrupoID, (participacion, organizacion) =>
            new JoinGrupoIdentidadesParticipacionGrupoIdentidadesOrganizacion
            {
                GrupoIdentidadesParticipacion = participacion,
                GrupoIdentidadesOrganizacion = organizacion
            });
        }
        public static IQueryable<JoinGrupoIdentidadesParticipacionGrupoIdentidadesProyecto> JoinGrupoIdentidadesProyecto(this IQueryable<EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.GrupoIdentidadesProyecto, participacion => participacion.GrupoID, proyecto => proyecto.GrupoID, (participacion, proyecto) =>
            new JoinGrupoIdentidadesParticipacionGrupoIdentidadesProyecto
            {
                GrupoIdentidadesParticipacion = participacion,
                GrupoIdentidadesProyecto = proyecto
            });
        }

        public static IQueryable<JoinPerfilPersonaOrgIdentidadPersona> JoinPersona(this IQueryable<JoinPerfilPersonaOrgIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Persona, perfilIdentidad => perfilIdentidad.PerfilPersonaOrg.PersonaID, persona => persona.PersonaID, (perfilIdentidad, persona) =>
            new JoinPerfilPersonaOrgIdentidadPersona
            {
                PerfilPersonaOrg = perfilIdentidad.PerfilPersonaOrg,
                Identidad = perfilIdentidad.Identidad,
                Persona = persona
            });
        }
        public static IQueryable<JoinPerfilPersona> JoinPersona(this IQueryable<EntityModel.Models.IdentidadDS.Perfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Persona, perfil => perfil.PersonaID, persona => persona.PersonaID, (perfil, persona) =>
            new JoinPerfilPersona
            {
                Perfil = perfil,
                Persona = persona
            });
        }

        public static IQueryable<JoinPerfilPersonaIdentidadbis> JoinIdentidad(this IQueryable<JoinPerfilPersona> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, objeto => objeto.Perfil.PerfilID, identidad => identidad.PerfilID, (objeto, identidad) =>
            new JoinPerfilPersonaIdentidadbis
            {
                Perfil = objeto.Perfil,
                Persona = objeto.Persona,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinPerfilIdentidadPersonaPerfilPersonaOrg> JoinPerfilPersonaOrg(this IQueryable<JoinPerfilIdentidadPersona> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.PerfilPersonaOrg, perfilIdentidadPersona => perfilIdentidadPersona.Perfil.PerfilID, perfilPersonaOrg => perfilPersonaOrg.PerfilID, (perfilIdentidadPersona, perfilPersonaOrg) =>
            new JoinPerfilIdentidadPersonaPerfilPersonaOrg
            {
                Perfil = perfilIdentidadPersona.Perfil,
                Identidad = perfilIdentidadPersona.Identidad,
                Persona = perfilIdentidadPersona.Persona,
                PerfilPersonaOrg = perfilPersonaOrg
            });
        }
        public static IQueryable<JoinPerfilPersonaOrgPersona> JoinPersona(this IQueryable<EntityModel.Models.IdentidadDS.PerfilPersonaOrg> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Persona, org => org.PersonaID, persona => persona.PersonaID, (org, persona) =>
            new JoinPerfilPersonaOrgPersona
            {
                PerfilPersonaOrg = org,
                Persona = persona
            });
        }
        public static IQueryable<JoinPerfilPersonaPersona> JoinPersona(this IQueryable<EntityModel.Models.IdentidadDS.PerfilPersona> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Persona, perfPers => perfPers.PersonaID, persona => persona.PersonaID, (perfPers, persona) =>
            new JoinPerfilPersonaPersona
            {
                PerfilPersona = perfPers,
                Persona = persona
            });
        }
        public static IQueryable<JoinPerfilPersonaOrgPerfilPersona> JoinPersona(this IQueryable<JoinPerfilPersonaOrgPerfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Persona, orgPerf => orgPerf.PerfilPersonaOrg.PersonaID, persona => persona.PersonaID, (orgPerf, persona) =>
            new JoinPerfilPersonaOrgPerfilPersona
            {
                PerfilPersonaOrg = orgPerf.PerfilPersonaOrg,
                Perfil = orgPerf.Perfil,
                Persona = persona
            });
        }
        public static IQueryable<JoinPerfilPersonaPerfilPersona> JoinPersona(this IQueryable<JoinPerfilPersonaPerfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Persona, perfPers => perfPers.PerfilPersona.PersonaID, persona => persona.PersonaID, (perfPers, persona) =>
            new JoinPerfilPersonaPerfilPersona
            {
                PerfilPersona = perfPers.PerfilPersona,
                Perfil = perfPers.Perfil,
                Persona = persona
            });
        }
        public static IQueryable<JoinPermisoGrupoAmigoOrgGrupoAmigosAmigoAgGrupoIdentidad> JoinIdentidad(this IQueryable<JoinPermisoGrupoAmigoOrgGrupoAmigosAmigoAgGrupo> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, permisoGrupoAmigo => permisoGrupoAmigo.PermisoGrupoAmigoOrg.IdentidadUsuarioID, identidad => identidad.IdentidadID, (permisoGrupoAmigo, identidad) =>
            new JoinPermisoGrupoAmigoOrgGrupoAmigosAmigoAgGrupoIdentidad
            {
                PermisoGrupoAmigoOrg = permisoGrupoAmigo.PermisoGrupoAmigoOrg,
                GrupoAmigos = permisoGrupoAmigo.GrupoAmigos,
                AmigoAgGrupo = permisoGrupoAmigo.AmigoAgGrupo,
                Identidad = identidad
            });
        }
        public static IQueryable<JoinPermisoGrupoAmigoOrgGrupoAmigosAmigoAgGrupo> JoinAmigoAgGrupo(this IQueryable<JoinPermisoGrupoAmigoOrgGrupoAmigos> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.AmigoAgGrupo, permisoGrupo => permisoGrupo.PermisoGrupoAmigoOrg.GrupoID, amigo => amigo.GrupoID, (permisoGrupo, amigo) =>
            new JoinPermisoGrupoAmigoOrgGrupoAmigosAmigoAgGrupo
            {
                PermisoGrupoAmigoOrg = permisoGrupo.PermisoGrupoAmigoOrg,
                GrupoAmigos = permisoGrupo.GrupoAmigos,
                AmigoAgGrupo = amigo
            });
        }
        public static IQueryable<JoinPermisoGrupoAmigoOrgGrupoAmigos> JoinGrupoAmigos(this IQueryable<PermisoGrupoAmigoOrg> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.GrupoAmigos, permiso => permiso.GrupoID, grupo => grupo.GrupoID, (permiso, grupo) =>
            new JoinPermisoGrupoAmigoOrgGrupoAmigos
            {
                PermisoGrupoAmigoOrg = permiso,
                GrupoAmigos = grupo
            });
        }
        public static IQueryable<JoinPerfilIdentidadPerfilOrganizacion> JoinPerfilOrganizacion(this IQueryable<JoinPerfilIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.PerfilOrganizacion, perfilIdentidad => perfilIdentidad.Perfil.PerfilID, perfOrg => perfOrg.PerfilID, (perfilIdentidad, perfOrg) =>
            new JoinPerfilIdentidadPerfilOrganizacion
            {
                Perfil = perfilIdentidad.Perfil,
                Identidad = perfilIdentidad.Identidad,
                PerfilOrganizacion = perfOrg
            });
        }

        public static IQueryable<JoinPerfilPersonaOrgIdentidadPerfil> JoinPerfil(this IQueryable<JoinPerfilPersonaOrgIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Perfil, perfilIdentidad => perfilIdentidad.PerfilPersonaOrg.PerfilID, perfil => perfil.PerfilID, (perfilIdentidad, perfil) =>
            new JoinPerfilPersonaOrgIdentidadPerfil
            {
                PerfilPersonaOrg = perfilIdentidad.PerfilPersonaOrg,
                Identidad = perfilIdentidad.Identidad,
                Perfil = perfil
            });
        }
        public static IQueryable<JoinPerfilIdentidadIdentidad2> JoinIdentidad2(this IQueryable<JoinPerfilIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, perfilIdentidad => perfilIdentidad.Perfil.PerfilID, identidad2 => identidad2.PerfilID, (perfilIdentidad, identidad2) =>
            new JoinPerfilIdentidadIdentidad2
            {
                Perfil = perfilIdentidad.Perfil,
                Identidad = perfilIdentidad.Identidad,
                Identidad2 = identidad2
            });
        }
        public static IQueryable<JoinIdentidadPerfilRedesSocialesProyectoUsuarioIdentidad> JoinProyectoUsuarioIdentidad(this IQueryable<JoinIdentidadPerfilRedesSociales> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.ProyectoUsuarioIdentidad, redesIdent => redesIdent.Identidad.IdentidadID, proy => proy.IdentidadID, (redesIdent, proy) =>
            new JoinIdentidadPerfilRedesSocialesProyectoUsuarioIdentidad
            {
                PerfilRedesSociales = redesIdent.PerfilRedesSociales,
                Identidad = redesIdent.Identidad,
                ProyectoUsuarioIdentidad = proy
            });
        }
        public static IQueryable<JoinPerfilIdentidadPersonaVinculoOrganizacionPersona> JoinPersona(this IQueryable<JoinPerfilIdentidadPersonaVinculoOrganizacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Persona, perfIdentOrg => perfIdentOrg.Perfil.PersonaID, persona => persona.PersonaID, (perfIdentOrg, persona) =>
            new JoinPerfilIdentidadPersonaVinculoOrganizacionPersona
            {
                Perfil = perfIdentOrg.Perfil,
                Identidad = perfIdentOrg.Identidad,
                PersonaVinculoOrganizacion = perfIdentOrg.PersonaVinculoOrganizacion,
                Persona = persona
            });
        }
        public static IQueryable<JoinPerfilIdentidadPersonaVinculoOrganizacion> JoinPersonaVinculoOrganizacion(this IQueryable<JoinPerfilIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.PersonaVinculoOrganizacion, perfilIdentidad => new { PersonaID = perfilIdentidad.Perfil.PersonaID.Value, OrganizacionID = perfilIdentidad.Perfil.OrganizacionID.Value }, vincOrg => new { PersonaID = vincOrg.PersonaID, OrganizacionID = vincOrg.OrganizacionID }, (perfilIdentidad, vincOrg) =>
                        new JoinPerfilIdentidadPersonaVinculoOrganizacion
                        {
                            Perfil = perfilIdentidad.Perfil,
                            Identidad = perfilIdentidad.Identidad,
                            PersonaVinculoOrganizacion = vincOrg
                        });
        }

        public static IQueryable<JoinProyectoUsuarioIdentidadIdentidad> JoinIdentidad(this IQueryable<ProyectoUsuarioIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, proyectoUsuarioIdentidad => proyectoUsuarioIdentidad.IdentidadID, ident => ident.IdentidadID, (proyectoUsuarioIdentidad, ident) =>
            new JoinProyectoUsuarioIdentidadIdentidad
            {
                ProyectoUsuarioIdentidad = proyectoUsuarioIdentidad,
                Identidad = ident
            });
        }
        public static IQueryable<JoinPerfilIdentidadProyectoUsuarioIdentidadPersona> JoinPersona(this IQueryable<JoinPerfilIdentidadProyectoUsuarioIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Persona, perfilIdentidad => perfilIdentidad.Perfil.PersonaID, persona => persona.PersonaID, (perfilIdentidad, persona) =>
            new JoinPerfilIdentidadProyectoUsuarioIdentidadPersona
            {
                Perfil = perfilIdentidad.Perfil,
                Identidad = perfilIdentidad.Identidad,
                ProyectoUsuarioIdentidad = perfilIdentidad.ProyectoUsuarioIdentidad,
                Persona = persona
            });
        }

        public static IQueryable<JoinPerfilPersonaIdentidadProyectoUsuarioIdentidadPerfil> JoinPerfil(this IQueryable<JoinPerfilPersonaIdentidadProyectoUsuarioIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Perfil, perfilIdentidad => perfilIdentidad.Identidad.PerfilID, perfil => perfil.PerfilID, (perfilIdentidad, perfil) =>
            new JoinPerfilPersonaIdentidadProyectoUsuarioIdentidadPerfil
            {
                PerfilPersona = perfilIdentidad.PerfilPersona,
                Identidad = perfilIdentidad.Identidad,
                ProyectoUsuarioIdentidad = perfilIdentidad.ProyectoUsuarioIdentidad,
                Perfil = perfil
            });
        }
        public static IQueryable<JoinPerfilPersonaOrgIdentidadProyectoUsuarioIdentidad> JoinProyectoUsuarioIdentidad(this IQueryable<JoinPerfilPersonaOrgIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.ProyectoUsuarioIdentidad, perfilIdentidad => perfilIdentidad.Identidad.IdentidadID, proy => proy.IdentidadID, (perfilIdentidad, proy) =>
            new JoinPerfilPersonaOrgIdentidadProyectoUsuarioIdentidad
            {
                PerfilPersonaOrg = perfilIdentidad.PerfilPersonaOrg,
                Identidad = perfilIdentidad.Identidad,
                ProyectoUsuarioIdentidad = proy
            });
        }

        public static IQueryable<JoinPerfilPersonaIdentidadProyectoUsuarioIdentidad> JoinProyectoUsuarioIdentidad(this IQueryable<JoinPerfilPersonaIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.ProyectoUsuarioIdentidad, perfilIdentidad => perfilIdentidad.Identidad.IdentidadID, proy => proy.IdentidadID, (perfilIdentidad, proy) =>
            new JoinPerfilPersonaIdentidadProyectoUsuarioIdentidad
            {
                PerfilPersona = perfilIdentidad.PerfilPersona,
                Identidad = perfilIdentidad.Identidad,
                ProyectoUsuarioIdentidad = proy
            });
        }
        public static IQueryable<JoinPerfilIdentidadProyectoUsuarioIdentidadPerfilRedesSociales> JoinPerfilRedesSociales(this IQueryable<JoinPerfilIdentidadProyectoUsuarioIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.PerfilRedesSociales, perfilIdentidad => perfilIdentidad.Perfil.PerfilID, redes => redes.PerfilID, (perfilIdentidad, redes) =>
            new JoinPerfilIdentidadProyectoUsuarioIdentidadPerfilRedesSociales
            {
                Perfil = perfilIdentidad.Perfil,
                Identidad = perfilIdentidad.Identidad,
                ProyectoUsuarioIdentidad = perfilIdentidad.ProyectoUsuarioIdentidad,
                PerfilRedesSociales = redes
            });
        }
        public static IQueryable<JoinPerfilIdentidadProyectoUsuarioIdentidad> JoinProyectoUsuarioIdentidad(this IQueryable<JoinPerfilIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.ProyectoUsuarioIdentidad, perfilIdentidad => perfilIdentidad.Identidad.IdentidadID, proy => proy.IdentidadID, (perfilIdentidad, proy) =>
            new JoinPerfilIdentidadProyectoUsuarioIdentidad
            {
                Perfil = perfilIdentidad.Perfil,
                Identidad = perfilIdentidad.Identidad,
                ProyectoUsuarioIdentidad = proy
            });
        }

        public static IQueryable<JoinGrupoIdentidadesParticipacionIdentidad> JoinIdentidad(this IQueryable<GrupoIdentidadesParticipacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, participacion => participacion.IdentidadID, ident => ident.IdentidadID, (participacion, ident) =>
            new JoinGrupoIdentidadesParticipacionIdentidad
            {
                GrupoIdentidadesParticipacion = participacion,
                Identidad = ident
            });
        }

        public static IQueryable<JoinGrupoIdentidadesParticipacionIdentidadPerfil> JoinPerfil(this IQueryable<JoinGrupoIdentidadesParticipacionIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Perfil, query => query.Identidad.PerfilID, perfil => perfil.PerfilID, (query, perfil) =>
            new JoinGrupoIdentidadesParticipacionIdentidadPerfil
            {
                GrupoIdentidadesParticipacion = query.GrupoIdentidadesParticipacion,
                Identidad = query.Identidad,
                Perfil = perfil
            });
        }

        public static IQueryable<JoinGrupoIdentGrupoIdentidadesProyecto> JoinGrupoIdentidadesProyecto(this IQueryable<GrupoIdentidades> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.GrupoIdentidadesProyecto, grupoIdent => grupoIdent.GrupoID, proy => proy.GrupoID, (grupoIdent, proy) =>
            new JoinGrupoIdentGrupoIdentidadesProyecto
            {
                GrupoIdentidades = grupoIdent,
                GrupoIdentidadesProyecto = proy
            });
        }

        public static IQueryable<JoinGrupoIdentGrupoIdentOrganizacionPerfilPersonaOrg> JoinPerfilPersonaOrg(this IQueryable<JoinGrupoIdentGrupoIdentOrganizacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.PerfilPersonaOrg, grupoIdent => grupoIdent.GrupoIdentidadesOrganizacion.OrganizacionID, perfPersOrg => perfPersOrg.OrganizacionID, (grupoIdent, perfPersOrg) =>
            new JoinGrupoIdentGrupoIdentOrganizacionPerfilPersonaOrg
            {
                GrupoIdentidades = grupoIdent.GrupoIdentidades,
                GrupoIdentidadesOrganizacion = grupoIdent.GrupoIdentidadesOrganizacion,
                PerfilPersonaOrg = perfPersOrg
            });
        }
        public static IQueryable<JoinGrupoIdentGrupoIdentOrganizacion> JoinGrupoIdentidadesOrganizacion(this IQueryable<GrupoIdentidades> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.GrupoIdentidadesOrganizacion, grupoIdent => grupoIdent.GrupoID, org => org.GrupoID, (grupoIdent, org) =>
            new JoinGrupoIdentGrupoIdentOrganizacion
            {
                GrupoIdentidades = grupoIdent,
                GrupoIdentidadesOrganizacion = org
            });
        }
        public static IQueryable<JoinPerfilPersonaIdentidadPersona> JoinPersona(this IQueryable<JoinPerfilPersonaIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Persona, perfPersIdent => perfPersIdent.PerfilPersona.PersonaID, persona => persona.PersonaID, (perfPersIdent, persona) =>
            new JoinPerfilPersonaIdentidadPersona
            {
                PerfilPersona = perfPersIdent.PerfilPersona,
                Identidad = perfPersIdent.Identidad,
                Persona = persona
            });
        }
        public static IQueryable<JoinPerfilIdentidadOrganizacion> JoinOrganizacion(this IQueryable<JoinPerfilIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Organizacion, perfilIdentidad => perfilIdentidad.Perfil.OrganizacionID, organizacion => organizacion.OrganizacionID, (perfilIdentidad, organizacion) =>
            new JoinPerfilIdentidadOrganizacion
            {
                Perfil = perfilIdentidad.Perfil,
                Identidad = perfilIdentidad.Identidad,
                Organizacion = organizacion
            });
        }
        public static IQueryable<JoinProfesorPerfil> JoinPerfil(this IQueryable<EntityModel.Models.IdentidadDS.Profesor> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Perfil, profesor => profesor.PerfilID, perfil => perfil.PerfilID, (profesor, perfil) =>
            new JoinProfesorPerfil
            {
                Profesor = profesor,
                Perfil = perfil
            });
        }
        public static IQueryable<JoinPerfilOrganizacionPerfil> JoinPerfil(this IQueryable<EntityModel.Models.IdentidadDS.PerfilOrganizacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Perfil, perfilOrganizacion => perfilOrganizacion.PerfilID, perfil => perfil.PerfilID, (perfilOrganizacion, perfil) =>
            new JoinPerfilOrganizacionPerfil
            {
                PerfilOrganizacion = perfilOrganizacion,
                Perfil = perfil
            });
        }
        public static IQueryable<JoinPerfilPersonaOrgPerfil> JoinPerfil(this IQueryable<EntityModel.Models.IdentidadDS.PerfilPersonaOrg> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Perfil, org => org.PerfilID, perfil => perfil.PerfilID, (org, perfil) =>
            new JoinPerfilPersonaOrgPerfil
            {
                PerfilPersonaOrg = org,
                Perfil = perfil
            });
        }
        public static IQueryable<JoinPerfilPersonaPerfil> JoinPerfil(this IQueryable<EntityModel.Models.IdentidadDS.PerfilPersona> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Perfil, perfPers => perfPers.PerfilID, perfil => perfil.PerfilID, (perfPers, perfil) =>
            new JoinPerfilPersonaPerfil
            {
                PerfilPersona = perfPers,
                Perfil = perfil
            });
        }
        public static IQueryable<JoinProfesorIdentidadPerfil> JoinPerfil(this IQueryable<JoinProfesorIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Perfil, profIdent => profIdent.Profesor.PerfilID, perfil => perfil.PerfilID, (profIdent, perfil) =>
            new JoinProfesorIdentidadPerfil
            {
                Profesor = profIdent.Profesor,
                Identidad = profIdent.Identidad,
                Perfil = perfil
            });
        }
        public static IQueryable<JoinPerfilIdentidadPerfilPersonaOrg> JoinPerfilPersonaOrgConPerfilID(this IQueryable<JoinPerfilIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.PerfilPersonaOrg, perfilIdentidad => perfilIdentidad.Identidad.PerfilID, perfilPersonaOrg => perfilPersonaOrg.PerfilID, (perfilIdentidad, perfilPersonaOrg) =>
            new JoinPerfilIdentidadPerfilPersonaOrg
            {
                Perfil = perfilIdentidad.Perfil,
                Identidad = perfilIdentidad.Identidad,
                PerfilPersonaOrg = perfilPersonaOrg
            });
        }
        public static IQueryable<JoinIdentidadPerfilRedesSocialesPerfil> JoinPerfil(this IQueryable<JoinIdentidadPerfilRedesSociales> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Perfil, redesIdent => redesIdent.PerfilRedesSociales.PerfilID, perfil => perfil.PerfilID, (redesIdent, perfil) =>
            new JoinIdentidadPerfilRedesSocialesPerfil
            {
                PerfilRedesSociales = redesIdent.PerfilRedesSociales,
                Identidad = redesIdent.Identidad,
                Perfil = perfil
            });
        }
        public static IQueryable<JoinPerfilOrganizacionIdentidadPerfil> JoinPerfil(this IQueryable<JoinPerfilOrganizacionIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Perfil, perfIdent => perfIdent.PerfilOrganizacion.PerfilID, perfil => perfil.PerfilID, (perfIdent, perfil) =>
            new JoinPerfilOrganizacionIdentidadPerfil
            {
                PerfilOrganizacion = perfIdent.PerfilOrganizacion,
                Identidad = perfIdent.Identidad,
                Perfil = perfil
            });
        }
        public static IQueryable<JoinIdentidadSuscripcionIdentidad2Identidad3> JoinIdentidad3(this IQueryable<JoinIdentidadSuscripcionIdentidad2> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, suscrIdent => suscrIdent.Identidad.PerfilID, identidad3 => identidad3.PerfilID, (suscrIdent, identidad3) =>
            new JoinIdentidadSuscripcionIdentidad2Identidad3
            {
                Suscripcion = suscrIdent.Suscripcion,
                Identidad = suscrIdent.Identidad,
                Identidad2 = suscrIdent.Identidad2,
                Identidad3 = identidad3
            });
        }
        public static IQueryable<JoinIdentidadSuscripcionIdentidad2> JoinIdentidad2(this IQueryable<JoinIdentidadSuscripcion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, suscrIdent => suscrIdent.Suscripcion.IdentidadID, identidad2 => identidad2.IdentidadID, (suscrIdent, identidad2) =>
            new JoinIdentidadSuscripcionIdentidad2
            {
                Suscripcion = suscrIdent.Suscripcion,
                Identidad = suscrIdent.Identidad,
                Identidad2 = identidad2
            });
        }

        public static IQueryable<JoinIdentidadSuscripcionSuscripcionTesauroUsuarioPerfil> JoinPerfil(this IQueryable<JoinIdentidadSuscripcionSuscripcionTesauroUsuario> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Perfil, suscrIdent => suscrIdent.Identidad.PerfilID, perfil => perfil.PerfilID, (suscrIdent, perfil) =>
            new JoinIdentidadSuscripcionSuscripcionTesauroUsuarioPerfil
            {
                Suscripcion = suscrIdent.Suscripcion,
                Identidad = suscrIdent.Identidad,
                SuscripcionTesauroUsuario = suscrIdent.SuscripcionTesauroUsuario,
                Perfil = perfil
            });
        }

        public static IQueryable<JoinIdentidadSuscripcionSuscripcionIdentidadProyectoIdentidad2PerfilPersona> JoinPersona(this IQueryable<JoinIdentidadSuscripcionSuscripcionIdentidadProyectoIdentidad2Perfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Persona, suscrIdent => suscrIdent.Perfil.PersonaID, persona => persona.PersonaID, (suscrIdent, persona) =>
            new JoinIdentidadSuscripcionSuscripcionIdentidadProyectoIdentidad2PerfilPersona
            {
                Suscripcion = suscrIdent.Suscripcion,
                Identidad = suscrIdent.Identidad,
                SuscripcionIdentidadProyecto = suscrIdent.SuscripcionIdentidadProyecto,
                Identidad2 = suscrIdent.Identidad2,
                Perfil = suscrIdent.Perfil,
                Persona = persona
            });
        }
        public static IQueryable<JoinIdentidadSuscripcionSuscripcionIdentidadProyectoIdentidad2Perfil> JoinPerfil(this IQueryable<JoinIdentidadSuscripcionSuscripcionIdentidadProyectoIdentidad2> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Perfil, suscrIdent => suscrIdent.Identidad.PerfilID, perfil => perfil.PerfilID, (suscrIdent, perfil) =>
            new JoinIdentidadSuscripcionSuscripcionIdentidadProyectoIdentidad2Perfil
            {
                Suscripcion = suscrIdent.Suscripcion,
                Identidad = suscrIdent.Identidad,
                SuscripcionIdentidadProyecto = suscrIdent.SuscripcionIdentidadProyecto,
                Identidad2 = suscrIdent.Identidad2,
                Perfil = perfil
            });
        }
        public static IQueryable<JoinIdentidadSuscripcionSuscripcionIdentidadProyectoIdentidad2> JoinIdentidad2(this IQueryable<JoinIdentidadSuscripcionSuscripcionIdentidadProyecto> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, suscrIdent => suscrIdent.Identidad.IdentidadID, identidad2 => identidad2.IdentidadID, (suscrIdent, identidad2) =>
            new JoinIdentidadSuscripcionSuscripcionIdentidadProyectoIdentidad2
            {
                Suscripcion = suscrIdent.Suscripcion,
                Identidad = suscrIdent.Identidad,
                SuscripcionIdentidadProyecto = suscrIdent.SuscripcionIdentidadProyecto,
                Identidad2 = identidad2
            });
        }

        public static IQueryable<JoinIdentidadSuscripcionSuscripcionIdentidadProyectoIdentidad2> JoinIdentidadSeguidor(this IQueryable<JoinIdentidadSuscripcionSuscripcionIdentidadProyecto> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, suscrIdent => suscrIdent.SuscripcionIdentidadProyecto.IdentidadID, identidad2 => identidad2.IdentidadID, (suscrIdent, identidad2) =>
            new JoinIdentidadSuscripcionSuscripcionIdentidadProyectoIdentidad2
            {
                Suscripcion = suscrIdent.Suscripcion,
                Identidad = suscrIdent.Identidad,
                SuscripcionIdentidadProyecto = suscrIdent.SuscripcionIdentidadProyecto,
                Identidad2 = identidad2
            });
        }

        public static IQueryable<JoinIdentidadSuscripcionSuscripcionTesauroUsuarioPersonaPerfil> JoinPerfil(this IQueryable<JoinIdentidadSuscripcionSuscripcionTesauroUsuarioPersona> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Perfil, suscrIdent => suscrIdent.Persona.PersonaID, perfil2 => perfil2.PersonaID, (suscrIdent, perfil2) =>
            new JoinIdentidadSuscripcionSuscripcionTesauroUsuarioPersonaPerfil
            {
                Suscripcion = suscrIdent.Suscripcion,
                Identidad = suscrIdent.Identidad,
                SuscripcionTesauroUsuario = suscrIdent.SuscripcionTesauroUsuario,
                Persona = suscrIdent.Persona,
                Perfil = perfil2

            });
        }
        public static IQueryable<JoinIdentidadSuscripcionSuscripcionTesauroUsuarioPersona> JoinPersona(this IQueryable<JoinIdentidadSuscripcionSuscripcionTesauroUsuario> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Persona, suscrIdent => suscrIdent.SuscripcionTesauroUsuario.UsuarioID, persona2 => persona2.UsuarioID, (suscrIdent, persona2) =>
            new JoinIdentidadSuscripcionSuscripcionTesauroUsuarioPersona
            {
                Suscripcion = suscrIdent.Suscripcion,
                Identidad = suscrIdent.Identidad,
                SuscripcionTesauroUsuario = suscrIdent.SuscripcionTesauroUsuario,
                Persona = persona2

            });
        }

        public static IQueryable<JoinIdentidadSuscripcionSuscripcionTesauroUsuario> JoinSuscripcionTesauroUsuario(this IQueryable<JoinIdentidadSuscripcion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.SuscripcionTesauroUsuario, suscrIdent => suscrIdent.Suscripcion.SuscripcionID, tesauro => tesauro.SuscripcionID, (suscrIdent, tesauro) =>
            new JoinIdentidadSuscripcionSuscripcionTesauroUsuario
            {
                Suscripcion = suscrIdent.Suscripcion,
                Identidad = suscrIdent.Identidad,
                SuscripcionTesauroUsuario = tesauro
            });
        }
        public static IQueryable<JoinIdentidadSuscripcionSuscripcionIdentidadProyecto> JoinSuscripcionIdentidadProyecto(this IQueryable<JoinIdentidadSuscripcion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.SuscripcionIdentidadProyecto, suscrIdent => suscrIdent.Suscripcion.SuscripcionID, proyecto => proyecto.SuscripcionID, (suscrIdent, proyecto) =>
            new JoinIdentidadSuscripcionSuscripcionIdentidadProyecto
            {
                Suscripcion = suscrIdent.Suscripcion,
                Identidad = suscrIdent.Identidad,
                SuscripcionIdentidadProyecto = proyecto
            });
        }

        /*public static IQueryable<JoinSuscripcionIdentidadProyectoSuscripcionIdentidad> JoinSuscripcionIdentidadProyecto(this IQueryable<EntityModel.Models.IdentidadDS.JoinSuscripcionIdentidadProyectoSuscripcion> pIQuery)
        {
            return pIQuery.Join(entityContext.Identidad, suscr => suscr.PerfilID, identidad => identidad.PerfilID, (suscr, identidad) =>
            new JoinSuscripcionIdentidadProyectoSuscripcionIdentidad
            {
                Suscripcion = suscr.Suscripcion,
                SuscripcionIdentidadProyecto = suscr.SuscripcionIdentidadProyecto,
                Identidad = identidad
            });
        }*/
        public static IQueryable<JoinSuscripcionIdentidadProyectoSuscripcion> JoinSuscripcionIdentidadProyecto(this IQueryable<EntityModel.Models.Suscripcion.Suscripcion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.SuscripcionIdentidadProyecto, suscripcion => suscripcion.SuscripcionID, suscripcionIdentidadProyecto => suscripcionIdentidadProyecto.SuscripcionID, (suscripcion, suscripcionIdentidadProyecto) =>
            new JoinSuscripcionIdentidadProyectoSuscripcion
            {
                Suscripcion = suscripcion,
                SuscripcionIdentidadProyecto = suscripcionIdentidadProyecto
            });
        }
        public static IQueryable<JoinPerfilIdentidadPerfilPersonaOrg> JoinPerfilPersonaOrgConOrganizacionID(this IQueryable<JoinPerfilIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.PerfilPersonaOrg, perfilIdentidad => perfilIdentidad.Perfil.OrganizacionID.Value, perfilPersonaOrg => perfilPersonaOrg.OrganizacionID, (perfilIdentidad, perfilPersonaOrg) =>
            new JoinPerfilIdentidadPerfilPersonaOrg
            {
                Perfil = perfilIdentidad.Perfil,
                Identidad = perfilIdentidad.Identidad,
                PerfilPersonaOrg = perfilPersonaOrg
            });
        }

        public static IQueryable<JoinPerfilPersonaOrgPerfil> JoinPerfilPersonaOrgConOrganizacionID(this IQueryable<Perfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.PerfilPersonaOrg, perfil => perfil.OrganizacionID.Value, perfilPersonaOrg => perfilPersonaOrg.OrganizacionID, (perfil, perfilPersonaOrg) =>
            new JoinPerfilPersonaOrgPerfil
            {
                Perfil = perfil,
                PerfilPersonaOrg = perfilPersonaOrg
            });
        }

        public static IQueryable<JoinPerfilIdentidadPerfilPersona> JoinPerfilPersona(this IQueryable<JoinPerfilIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.PerfilPersona, perfilIdentidad => perfilIdentidad.Perfil.PerfilID, perfilPersona => perfilPersona.PerfilID, (perfilIdentidad, perfilPersona) =>
            new JoinPerfilIdentidadPerfilPersona
            {
                Perfil = perfilIdentidad.Perfil,
                Identidad = perfilIdentidad.Identidad,
                PerfilPersona = perfilPersona
            });
        }
        public static IQueryable<JoinPerfilIdentidadPersonaPerfilPersona> JoinPerfilPersona(this IQueryable<JoinPerfilIdentidadPersona> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.PerfilPersona, perfilIdentidadPersona => perfilIdentidadPersona.Perfil.PerfilID, perfilPersona => perfilPersona.PerfilID, (perfilIdentidadPersona, perfilPersona) =>
            new JoinPerfilIdentidadPersonaPerfilPersona
            {
                Perfil = perfilIdentidadPersona.Perfil,
                Identidad = perfilIdentidadPersona.Identidad,
                Persona = perfilIdentidadPersona.Persona,
                PerfilPersona = perfilPersona
            });
        }
        public static IQueryable<JoinPersonaVinculoOrganizacionPersona> JoinPersona(this IQueryable<EntityModel.Models.OrganizacionDS.PersonaVinculoOrganizacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Persona, persVincOrg => persVincOrg.PersonaID, persona => persona.PersonaID, (persVincOrg, persona) =>
             new JoinPersonaVinculoOrganizacionPersona
             {
                 PersonaVinculoOrganizacion = persVincOrg,
                 Persona = persona
             });
        }
        public static IQueryable<JoinPerfilPersonaOrgIdentidadOrganizacion> JoinOrganizacion(this IQueryable<JoinPerfilPersonaOrgIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Organizacion, org => org.PerfilPersonaOrg.OrganizacionID, organizacion => organizacion.OrganizacionID, (org, organizacion) =>
            new JoinPerfilPersonaOrgIdentidadOrganizacion
            {
                PerfilPersonaOrg = org.PerfilPersonaOrg,
                Identidad = org.Identidad,
                Organizacion = organizacion
            });
        }
        public static IQueryable<JoinIdentidadPerfilRedesSocialesAmigo> JoinAmigo(this IQueryable<JoinIdentidadPerfilRedesSociales> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Amigo, redesIdent => redesIdent.Identidad.IdentidadID, amigo => amigo.IdentidadID, (redesIdent, amigo) =>
            new JoinIdentidadPerfilRedesSocialesAmigo
            {
                PerfilRedesSociales = redesIdent.PerfilRedesSociales,
                Identidad = redesIdent.Identidad,
                Amigo = amigo
            });
        }
        public static IQueryable<JoinPerfilOrganizacionIdentidadAmigo> JoinAmigo(this IQueryable<JoinPerfilOrganizacionIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Amigo, org => org.Identidad.IdentidadID, amigo => amigo.IdentidadID, (org, amigo) =>
            new JoinPerfilOrganizacionIdentidadAmigo
            {
                PerfilOrganizacion = org.PerfilOrganizacion,
                Identidad = org.Identidad,
                Amigo = amigo
            });
        }
        public static IQueryable<JoinPerfilOrganizacionIdentidad> JoinIdentidad(this IQueryable<EntityModel.Models.IdentidadDS.PerfilOrganizacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, perfilOrganizacion => perfilOrganizacion.PerfilID, ident => ident.PerfilID, (perfilOrganizacion, ident) =>
            new JoinPerfilOrganizacionIdentidad
            {
                PerfilOrganizacion = perfilOrganizacion,
                Identidad = ident
            });
        }

        public static IQueryable<JoinPerfilOrganizacionPerfilPersonaOrg> JoinPerfilPersonaOrg(this IQueryable<EntityModel.Models.IdentidadDS.PerfilOrganizacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.PerfilPersonaOrg, perfilOrganizacion => perfilOrganizacion.OrganizacionID, perfilPersonaOrg => perfilPersonaOrg.OrganizacionID, (perfilOrganizacion, perfilPersonaOrg) =>
            new JoinPerfilOrganizacionPerfilPersonaOrg
            {
                PerfilOrganizacion = perfilOrganizacion,
                PerfilPersonaOrg = perfilPersonaOrg
            });
        }

        public static IQueryable<JoinPerfilPersonaOrgIdentidadAmigo> JoinAmigo(this IQueryable<JoinPerfilPersonaOrgIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Amigo, org => org.Identidad.IdentidadID, amigo => amigo.IdentidadID, (org, amigo) =>
            new JoinPerfilPersonaOrgIdentidadAmigo
            {
                PerfilPersonaOrg = org.PerfilPersonaOrg,
                Identidad = org.Identidad,
                Amigo = amigo
            });
        }
        public static IQueryable<JoinPerfilPersonaIdentidadAmigo> JoinAmigo(this IQueryable<JoinPerfilPersonaIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Amigo, perfPersIdent => perfPersIdent.Identidad.IdentidadID, amigo => amigo.IdentidadID, (perfPersIdent, amigo) =>
            new JoinPerfilPersonaIdentidadAmigo
            {
                PerfilPersona = perfPersIdent.PerfilPersona,
                Identidad = perfPersIdent.Identidad,
                Amigo = amigo
            });
        }

        public static IQueryable<JoinPerfilIdentidadAmigo> JoinAmigo(this IQueryable<JoinPerfilIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Amigo, perfilIdentidad => perfilIdentidad.Identidad.IdentidadID, amigo => amigo.IdentidadID, (perfilIdentidad, amigo) =>
            new JoinPerfilIdentidadAmigo
            {
                Perfil = perfilIdentidad.Perfil,
                Identidad = perfilIdentidad.Identidad,
                Amigo = amigo
            });
        }

        public static IQueryable<JoinPerfilIdentidadPersonaProyectoRolUsuario> JoinProyetoRolUsuario(this IQueryable<JoinPerfilIdentidadPersona> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.ProyectoRolUsuario, perfilIdentidadPersona => perfilIdentidadPersona.Persona.UsuarioID.Value, proy => proy.UsuarioID, (perfilIdentidadPersona, proy) =>
            new JoinPerfilIdentidadPersonaProyectoRolUsuario
            {
                Perfil = perfilIdentidadPersona.Perfil,
                Identidad = perfilIdentidadPersona.Identidad,
                Persona = perfilIdentidadPersona.Persona,
                ProyectoRolUsuario = proy
            });
        }
        public static IQueryable<JoinPerfilIdentidadPersona> JoinPersona(this IQueryable<JoinPerfilIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Persona, perfilIdentidad => perfilIdentidad.Perfil.PersonaID, persona => persona.PersonaID, (perfilIdentidad, persona) =>
            new JoinPerfilIdentidadPersona
            {
                Perfil = perfilIdentidad.Perfil,
                Identidad = perfilIdentidad.Identidad,
                Persona = persona
            });
        }

        public static IQueryable<JoinPerfilIdentidadPersonaPersonaVinculoOrganizacion> JoinPersonaVinculoOrganizacion(this IQueryable<JoinPerfilIdentidadPersona> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.PersonaVinculoOrganizacion, perfilIdentidadPersona => new { PersonaID = perfilIdentidadPersona.Perfil.PersonaID.Value, OrganizacionID = perfilIdentidadPersona.Perfil.OrganizacionID.Value }, persVincOrg => new { PersonaID = persVincOrg.PersonaID, OrganizacionID = persVincOrg.OrganizacionID }, (join, persVincOrg) =>
                        new JoinPerfilIdentidadPersonaPersonaVinculoOrganizacion
                        {
                            Perfil = join.Perfil,
                            Identidad = join.Identidad,
                            Persona = join.Persona,
                            PersonaVinculoOrganizacion = persVincOrg
                        });
        }


        public static IQueryable<JoinPerfilIdentidadPersonaTesauroUsuario> JoinTesauroUsuario(this IQueryable<JoinPerfilIdentidadPersona> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.TesauroUsuario, item => item.Persona.UsuarioID, tesUs => tesUs.UsuarioID, (item, tesUs) =>
            new JoinPerfilIdentidadPersonaTesauroUsuario
            {
                Perfil = item.Perfil,
                Identidad = item.Identidad,
                Persona = item.Persona,
                TesauroUsuario = tesUs
            });
        }

        public static IQueryable<JoinPerfilIdentidadPersonaTesauroUsuarioCategoriaTesauro> JoinCategoriaTesauro(this IQueryable<JoinPerfilIdentidadPersonaTesauroUsuario> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.CategoriaTesauro, item => item.TesauroUsuario.TesauroID, catTes => catTes.TesauroID, (item, catTes) =>
            new JoinPerfilIdentidadPersonaTesauroUsuarioCategoriaTesauro
            {
                Perfil = item.Perfil,
                Identidad = item.Identidad,
                Persona = item.Persona,
                TesauroUsuario = item.TesauroUsuario,
                CategoriaTesauro = catTes
            });
        }

        public static IQueryable<JoinPerfilIdentidad> JoinIdentidad(this IQueryable<Perfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, perfil => perfil.PerfilID, ident => ident.PerfilID, (perfil, ident) =>
            new JoinPerfilIdentidad
            {
                Perfil = perfil,
                Identidad = ident
            });
        }

        public static IQueryable<JoinIdentidadPerfilCurriculum> LeftJoinCurriculum(this IQueryable<MVC.IdentidadPerfilPersona> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.GroupJoin(entityContext.Curriculum, item => new { CurriculumID = item.Identidad.CurriculumID.Value }, curriculum => new { CurriculumID = curriculum.CurriculumID }, (item, curriculum) =>
            new
            {
                Perfil = item.Perfil,
                Identidad = item.Identidad,
                Curriculum = curriculum,
                Persona = item.Persona
            }).SelectMany(item => item.Curriculum.DefaultIfEmpty(), (item, agrup) => new JoinIdentidadPerfilCurriculum
            {
                Perfil = item.Perfil,
                Identidad = item.Identidad,
                Persona = item.Persona,
                Curriculum = agrup
            });
        }

        public static IQueryable<JoinAmigoIdentidad> JoinIdentidad(this IQueryable<Amigo> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, amigo => amigo.IdentidadID, ident => ident.IdentidadID, (amigo, ident) =>
            new JoinAmigoIdentidad
            {
                Amigo = amigo,
                Identidad = ident
            });
        }

        public static IQueryable<JoinProfesorIdentidadSuscripcion> JoinSuscripcion(this IQueryable<JoinProfesorIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Suscripcion, profesor => profesor.Identidad.IdentidadID, suscripcion => suscripcion.IdentidadID, (profesor, suscripcion) =>
            new JoinProfesorIdentidadSuscripcion
            {
                Profesor = profesor.Profesor,
                Identidad = profesor.Identidad,
                Suscripcion = suscripcion
            });
        }
        public static IQueryable<JoinProfesorIdentidad> JoinIdentidad(this IQueryable<EntityModel.Models.IdentidadDS.Profesor> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, profesor => profesor.PerfilID, ident => ident.PerfilID, (profesor, ident) =>
            new JoinProfesorIdentidad
            {
                Profesor = profesor,
                Identidad = ident
            });
        }
        public static IQueryable<JoinPerfilPersonaOrgIdentidadSuscripcion> JoinSuscripcion(this IQueryable<JoinPerfilPersonaOrgIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Suscripcion, org => org.Identidad.IdentidadID, suscripcion => suscripcion.IdentidadID, (org, suscripcion) =>
            new JoinPerfilPersonaOrgIdentidadSuscripcion
            {
                PerfilPersonaOrg = org.PerfilPersonaOrg,
                Identidad = org.Identidad,
                Suscripcion = suscripcion
            });
        }
        public static IQueryable<JoinPerfilPersonaOrgIdentidad> JoinIdentidad(this IQueryable<EntityModel.Models.IdentidadDS.PerfilPersonaOrg> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, org => org.PerfilID, ident => ident.PerfilID, (org, ident) =>
            new JoinPerfilPersonaOrgIdentidad
            {
                PerfilPersonaOrg = org,
                Identidad = ident
            });
        }

        public static IQueryable<JoinPerfilPersonaIdentidad> JoinIdentidad(this IQueryable<PerfilPersona> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, perfPers => perfPers.PerfilID, ident => ident.PerfilID, (perfPers, ident) =>
            new JoinPerfilPersonaIdentidad
            {
                PerfilPersona = perfPers,
                Identidad = ident
            });
        }
        public static IQueryable<JoinPerfilPersonaIdentidadSuscripcion> JoinSuscripcion(this IQueryable<JoinPerfilPersonaIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Suscripcion, perfPersIdent => perfPersIdent.Identidad.IdentidadID, suscripcion => suscripcion.IdentidadID, (perfPersIdent, suscripcion) =>
            new JoinPerfilPersonaIdentidadSuscripcion
            {
                PerfilPersona = perfPersIdent.PerfilPersona,
                Identidad = perfPersIdent.Identidad,
                Suscripcion = suscripcion
            });
        }

        public static IQueryable<JoinIdentidadPerfilRedesSocialesSuscripcion> JoinSuscripcion(this IQueryable<JoinIdentidadPerfilRedesSociales> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Suscripcion, redesIdent => redesIdent.Identidad.IdentidadID, suscripcion => suscripcion.IdentidadID, (redesIdent, suscripcion) =>
            new JoinIdentidadPerfilRedesSocialesSuscripcion
            {
                PerfilRedesSociales = redesIdent.PerfilRedesSociales,
                Identidad = redesIdent.Identidad,
                Suscripcion = suscripcion
            });
        }

        public static IQueryable<JoinPerfilRedesSocialesPerfil> JoinPerfil(this IQueryable<PerfilRedesSociales> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);

            return pIQuery.Join(entityContext.Perfil, perfilRedesSociales => perfilRedesSociales.PerfilID, perfil => perfil.PerfilID, (perfilRedesSociales, perfil) => new JoinPerfilRedesSocialesPerfil
            {
                Perfil = perfil,
                PerfilRedesSociales = perfilRedesSociales
            });
        }

        public static IQueryable<JoinIdentidadPerfilRedesSociales> JoinIdentidad(this IQueryable<PerfilRedesSociales> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, redes => redes.PerfilID, ident => ident.PerfilID, (redes, ident) =>
            new JoinIdentidadPerfilRedesSociales
            {
                PerfilRedesSociales = redes,
                Identidad = ident
            });
        }

        public static IQueryable<JoinIdentidadSuscripcion> JoinIdentidad(this IQueryable<EntityModel.Models.Suscripcion.Suscripcion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, suscripcion => suscripcion.IdentidadID, ident => ident.IdentidadID, (suscripcion, ident) =>
            new JoinIdentidadSuscripcion
            {
                Suscripcion = suscripcion,
                Identidad = ident
            });
        }

        public static IQueryable<JoinResultadoSuscripcionSuscripcion> JoinSuscripcion(this IQueryable<ResultadoSuscripcion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Suscripcion, resultadoSuscripcion => resultadoSuscripcion.SuscripcionID, suscripcion => suscripcion.SuscripcionID, (resultadoSuscripcion, suscripcion) =>
            new JoinResultadoSuscripcionSuscripcion
            {
                Suscripcion = suscripcion,
                ResultadoSuscripcion = resultadoSuscripcion
            });
        }

        public static IQueryable<JoinPerfilIdentidadSuscripcion> JoinSuscripcion(this IQueryable<JoinPerfilIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Suscripcion, perfilIdentidad => perfilIdentidad.Identidad.IdentidadID, suscripcion => suscripcion.IdentidadID, (perfilIdentidad, suscripcion) =>
            new JoinPerfilIdentidadSuscripcion
            {
                Perfil = perfilIdentidad.Perfil,
                Identidad = perfilIdentidad.Identidad,
                Suscripcion = suscripcion
            });
        }



        public static IQueryable<JoinIdentidadPerfil> ObtenerJoinPerfilIdentidad(this IQueryable<Perfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return entityContext.Perfil.Join(entityContext.Identidad, perf => perf.PerfilID, ident => ident.PerfilID, (perf, ident) =>
                new JoinIdentidadPerfil
                {
                    Perfil = perf,
                    Identidad = ident
                });
        }


        public static IQueryable<JoinIdentidadPerfilProyecto> JoinProyecto(this IQueryable<JoinIdentidadPerfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Proyecto, perfilIdentidad => perfilIdentidad.Identidad.ProyectoID, proyecto => proyecto.ProyectoID, (perfilIdentidad, proyecto) =>
                new JoinIdentidadPerfilProyecto
                {
                    Perfil = perfilIdentidad.Perfil,
                    Identidad = perfilIdentidad.Identidad,
                    Proyecto = proyecto
                });
        }

        public static IQueryable<JoinIdentidadProyecto> JoinProyecto(this IQueryable<EntityModel.Models.IdentidadDS.Identidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Proyecto, identidad => identidad.ProyectoID, proyecto => proyecto.ProyectoID, (identidad, proyecto) =>
                new JoinIdentidadProyecto
                {
                    Identidad = identidad,
                    Proyecto = proyecto
                });
        }

        public static IQueryable<JoinIdentidadProyectoIdentidad> JoinProyecto(this IQueryable<JoinIdentidadProyecto> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, item => item.Identidad.PerfilID, identidad2 => identidad2.PerfilID, (item, identidad2) =>
                new JoinIdentidadProyectoIdentidad
                {
                    Identidad = item.Identidad,
                    Proyecto = item.Proyecto,
                    Identidad2 = identidad2
                });
        }

        public static IQueryable<JoinIdentidadPerfilPersona> JoinPerfilIdentidadPersona(this IQueryable<JoinIdentidadPerfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Persona, perfilIdentidad => perfilIdentidad.Perfil.PersonaID.Value, persona => persona.PersonaID, (perfilIdentidad, persona) => new JoinIdentidadPerfilPersona { Identidad = perfilIdentidad.Identidad, Perfil = perfilIdentidad.Perfil, Persona = persona });
        }

        public static IQueryable<JoinIdentidadPerfilPerfilPersonaOrg> JoinPerfilPersonaOrg(this IQueryable<JoinIdentidadPerfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.PerfilPersonaOrg, perfilIdentidad => perfilIdentidad.Perfil.PerfilID, perfilPersonaOrg => perfilPersonaOrg.PerfilID, (perfilIdentidad, pefilPersonaOrg) => new JoinIdentidadPerfilPerfilPersonaOrg
            {
                Identidad = perfilIdentidad.Identidad,
                Perfil = perfilIdentidad.Perfil,
                PerfilPersonaOrg = pefilPersonaOrg
            });
        }

        public static IQueryable<JoinIdentidadPerfilPersonaUsuario> ObtenerJoinPerfilIdentidadPersonaUsuario(this IQueryable<JoinIdentidadPerfilPersona> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Usuario, perf => perf.Persona.UsuarioID.Value, usuario => usuario.UsuarioID, (perfilPersonaUsuario, usuario) =>
                new JoinIdentidadPerfilPersonaUsuario
                {
                    Perfil = perfilPersonaUsuario.Perfil,
                    Identidad = perfilPersonaUsuario.Identidad,
                    Persona = perfilPersonaUsuario.Persona,
                    Usuario = usuario
                });
        }

        public static IQueryable<JoinIdentidadPerfilPersonaProyecto> ObtenerJoinPerfilIdentidadPersonaProyecto(this IQueryable<JoinIdentidadPerfilPersona> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Proyecto, perf => perf.Identidad.ProyectoID, proyecto => proyecto.ProyectoID, (perfilPersonaUsuario, proyecto) =>
                new JoinIdentidadPerfilPersonaProyecto
                {
                    Perfil = perfilPersonaUsuario.Perfil,
                    Identidad = perfilPersonaUsuario.Identidad,
                    Persona = perfilPersonaUsuario.Persona,
                    Proyecto = proyecto
                });
        }

        public static IQueryable<JoinIdentidadPermisoAmigoOrg> JoinIdentidad(this IQueryable<PermisoAmigoOrg> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, amigo => amigo.IdentidadUsuarioID, ident => ident.IdentidadID, (amigo, ident) =>
            new JoinIdentidadPermisoAmigoOrg
            {
                PermisoAmigoOrg = amigo,
                Identidad = ident
            });
        }




    }

    /// <summary>
    /// Data adapter de identidad
    /// </summary>
    public class IdentidadAD : BaseAD
    {
        public List<EntityModel.Models.IdentidadDS.Identidad> TestJoin()
        {
            return mEntityContext.Perfil.ObtenerJoinPerfilIdentidad().JoinPerfilIdentidadPersona().ObtenerJoinPerfilIdentidadPersonaUsuario().Where(item => item.Usuario.Login.Equals("juan")).Select(item => item.Identidad).ToList();
        }

        #region Constantes

        /// <summary>
        /// Sufijo y extension de las im�genes peque�as de perfil de persona y organizacion
        /// </summary>
        public const string SUFIJO_PEQUE = "_peque.png";

        public const string CONTADOR_NUMERO_VISITAS = "NumeroVisitas";

        public const string CONTADOR_NUMERO_DESCARGAS = "NumeroDescargas";

        #endregion

        private EntityContext mEntityContext;
        private LoggingService mLoggingService;

        #region Constructores

        /// <summary>
        /// Constructor por defecto, sin par�metros, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public IdentidadAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;

            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuraci�n de conexi�n a la base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuraci�n de base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se est�n usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        [Obsolete("El constructor con parametros desaparecera en futras versiones. Usar el constructor sin parametros en su lugar")]
        public IdentidadAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;

            this.CargarConsultasYDataAdapters();
        }

        #endregion

        #region Consultas

        private string sqlSelectPerfilDeOrganizacion;
        private string sqlSelectPerfilOrganizacionDeOrganizacion;
        private string sqlSelectPerfilOrganizacionDeOrganizacionNoActivosTambien;
        private string sqlSelectIdentidadesSoloDeOrganizacion;

        private string sqlSelectIdentidadesDeOrganizacion;
        private string sqlSelectIdentidadesDeOrganizacionNoActivosTambien;
        private string sqlSelectPerfilesDeOrganizacion;
        private string sqlSelectPerfilesDeOrganizacionNoActivosTambien;
        private string sqlSelectPerfilPersonaOrgDeOrganizacion;
        private string sqlSelectPerfilPersonaOrgDeOrganizacionNoActivosTambien;

        private string sqlSelectPerfilPersonaPorID;
        private string sqlSelectPerfilPersonaOrgPorID;
        private string sqlSelectPerfilOrganizacionPorID;
        private string sqlSelectProfesorPorID;

        private string sqlSelectPerfilesDePersona;
        private string sqlSelectPerfilRedesSocDePersona;

        private string sqlSelectIdentidadesDePersona;
        private string sqlSelectPerfilPersonaDePersona;
        private string sqlSelectPerfilPersonaOrgDePersona;
        private string sqlSelectPerfilOrganizacionDePersona;
        private string sqlSelectProfesorDePersona;

        private string sqlSelectIdentidadesDeProyecto;
        private string sqlSelectIdentidadesDePersonaEnProyecto;
        private string sqlSelectPerfilesDeProyecto;
        private string sqlSelectPerfilPerfilRedesSocDeProyecto;
        private string sqlSelectIdentidadDeProyectoYUsuario;
        private string sqlSelectIdentidadPorID;
        private string sqlSelectIdentidadPorIDConMyGNOSS;
        private string sqlSelectPerfilPorIdentidadID;
        private string sqlSelectPerfilRedesSocPorIdentidadID;

        private string sqlSelectCVIdentidadPersonaDeUsuario;

        private string sqlSelectIdentidadDePersonaDeMyGNOSS;
        private string sqlSelectPerfilDePersona;

        private string sqlSelectPerfil;
        private string sqlSelectPerfilRedesSociales;
        private string sqlSelectPerfilGadget;
        private string sqlSelectPerfilPersonaOrg;
        private string sqlSelectPerfilOrganizacion;
        private string sqlSelectPerfilPersona;
        private string sqlSelectIdentidad;
        private string sqlSelectPerfilPersonaOrgDeProyecto;
        private string sqlSelectPerfilPersonaDeProyecto;
        private string sqlSelectPerfilOrganizacionDeProyecto;
        private string sqlSelectPerfilesDeUsuario;
        private string sqlSelectProfesor;

        private string sqlSelectIdentidadesDeUsuario;

        private string sqlSelectExistePerfilPersonal;
        private string sqlSelectExistePerfilPersonaOrg;

        private string sqlSelectPerfilPorPerfilID;

        private string sqlSelectDatoExtraProyectoOpcionIdentidadPorIdentidadesID;
        private string sqlSelectDatoExtraProyectoVirtuosoIdentidadPorIdentidadesID;
        private string sqlSelectDatoExtraEcosistemaOpcionPerfilPorIdentidadesID;
        private string sqlSelectDatoExtraEcosistemaVirtuosoPerfilPorIdentidadesID;

        private string sqlSelectIdentidadesPorPerfilID;

        private string sqlSelectPerfilDeUsuarioActivos;
        private string sqlSelectPerfilRedesSocDeUsuarioActivos;
        private string sqlSelectIdentidadesDePersonaActivas;
        private string sqlSelectPerfilPersonaDePersonaActivos;
        private string sqlSelectPerfilPersonaOrgDePersonaActivos;
        private string sqlSelectPerfilOrganizacionDePersonaActivos;
        private string sqlSelectProfesorDePersonaActivos;

        private string sqlSelectIdentidadPorIDActiva;
        private string sqlSelectPerfilPorIdentidadIDActivo;
        private string sqlSelectPerfilRedesSocPorIdentidadIDActivo;
        private string sqlSelectPerfilPersonaPorIDActiva;
        private string sqlSelectPerfilPersonaOrgPorIDActiva;
        private string sqlSelectPerfilOrganizacionPorIDActiva;
        private string sqlSelectProfesorPorIDActiva;

        private string sqlSelectIdentidadesDepersonasConOrgDeProy;
        private string sqlSelectPerfilDepersonasConOrgDeProy;
        private string sqlSelectPerfilPersonaOrgDepersonasConOrgDeProy;

        private string sqlSelectPerfilPersonaDeUsuario;
        private string sqlSelectPerfilPersonaOrgDeUsuario;

        private string sqlSelectIdentidadesDeOrganizacionesEnProyecto;
        private string sqlSelectPerfilesDeOrganizacionesDeProyecto;
        private string sqlSelectPerfilOrganizacionDeOrganizacionesDeProyecto;

        private string sqlSelectPerfilPersonaOrgDePersonasNoCorporativasDeProyecto;
        private string sqlSelectIdentidadesDePersonasNoCorporativasDeProyecto;
        private string sqlSelectPerfilesDePersonasNoCorporativasDeProyecto;

        private string sqlSelectIdentidadesDeOrganizacionAdmin;
        private string sqlSelectPerfilesDeOrganizacionAdmin;
        private string sqlSelectPerfilPersonaOrgDeOrganizacionAdmin;

        private string sqlSelectIdentidadContadoresRecursos;

        private string sqlUpdateNumeroConexionesProyecto;

        private string sqlSelectHaParticipadoConPerfilEnComunidad;
        private string sqlSelectEstaIdentidadExpulsadaDeproyecto;
        private string sqlSelectParticipaPerfilEnComunidad;
        private string sqlSelectParticipaIdentidadEnComunidad;

        private string sqlSelectIdentidadesDePersonasNoCorporativasDeProyectoConNombreYApellidos;
        private string sqlSelectIdentidadesDepersonasConOrgDeProyConNombreYApellidos;
        private string selectIdentidad;
        private string selectPerfil;
        private string selectIdentidadContadoresRecursos;

        private string selectGrupoIdentidades;
        private string selectGrupoIdentidadesProyecto;
        private string selectGrupoIdentidadesOrganizacion;
        private string selectGrupoIdentidadesParticipacion;

        #endregion

        #region Updates

        private string sqlUpdatePerfilCambioNombrePersona;
        private string sqlUpdateIdentidadCambioNombrePersona;

        private string sqlUpdatePerfilCambioNombreOrganizacion;
        private string sqlUpdatePerfilPersOrgCambioNombreOrganizacion;
        private string sqlUpdateIdentidadCambioNombreOrganizacion;

        #endregion

        #region DataAdapter

        #region Perfil

        private string sqlPerfilInsert;
        private string sqlPerfilDelete;
        private string sqlPerfilModify;

        #endregion

        #region PerfilRedesSociales

        private string sqlPerfilRedesSocialesInsert;
        private string sqlPerfilRedesSocialesDelete;
        private string sqlPerfilRedesSocialesModify;

        #endregion

        #region PerfilGadget

        private string sqlPerfilGadgetInsert;
        private string sqlPerfilGadgetDelete;
        private string sqlPerfilGadgetModify;

        #endregion

        #region PerfilPersonaOrg

        private string sqlPerfilPersonaOrgInsert;
        private string sqlPerfilPersonaOrgDelete;
        private string sqlPerfilPersonaOrgModify;

        #endregion

        #region PerfilOrganizacion

        private string sqlPerfilOrganizacionInsert;
        private string sqlPerfilOrganizacionDelete;
        private string sqlPerfilOrganizacionModify;

        #endregion

        #region PerfilPersona

        private string sqlPerfilPersonaInsert;
        private string sqlPerfilPersonaDelete;
        private string sqlPerfilPersonaModify;

        #endregion

        #region Identidad

        private string sqlIdentidadInsert;
        private string sqlIdentidadDelete;
        private string sqlIdentidadModify;

        #endregion

        #region Profesor

        private string sqlProfesorInsert;
        private string sqlProfesorDelete;
        private string sqlProfesorModify;

        #endregion

        #region GrupoIdentidades

        private string sqlGrupoIdentidadesInsert;
        private string sqlGrupoIdentidadesDelete;
        private string sqlGrupoIdentidadesModify;

        #endregion

        #region GrupoIdentidadesProyecto

        private string sqlGrupoIdentidadesProyectoInsert;
        private string sqlGrupoIdentidadesProyectoDelete;
        private string sqlGrupoIdentidadesProyectoModify;

        #endregion

        #region GrupoIdentidadesOrganizacion

        private string sqlGrupoIdentidadesOrganizacionInsert;
        private string sqlGrupoIdentidadesOrganizacionDelete;
        private string sqlGrupoIdentidadesOrganizacionModify;

        #endregion

        #region GrupoIdentidadesParticipacion

        private string sqlGrupoIdentidadesParticipacionInsert;
        private string sqlGrupoIdentidadesParticipacionDelete;
        private string sqlGrupoIdentidadesParticipacionModify;

        #endregion

        #region DatoExtraProyectoOpcionIdentidad

        private string sqlDatoExtraProyectoOpcionIdentidadInsert;
        private string sqlDatoExtraProyectoOpcionIdentidadDelete;
        private string sqlDatoExtraProyectoOpcionIdentidadModify;

        #endregion

        #region DatoExtraProyectoVirtuosoIdentidad

        private string sqlDatoExtraProyectoVirtuosoIdentidadInsert;
        private string sqlDatoExtraProyectoVirtuosoIdentidadDelete;
        private string sqlDatoExtraProyectoVirtuosoIdentidadModify;

        #endregion

        #region DatoExtraEcosistemaOpcionPerfil

        private string sqlDatoExtraEcosistemaOpcionPerfilInsert;
        private string sqlDatoExtraEcosistemaOpcionPerfilDelete;
        private string sqlDatoExtraEcosistemaOpcionPerfilModify;

        #endregion

        #region DatoExtraEcosistemaVirtuosoPerfil

        private string sqlDatoExtraEcosistemaVirtuosoPerfilInsert;
        private string sqlDatoExtraEcosistemaVirtuosoPerfilDelete;
        private string sqlDatoExtraEcosistemaVirtuosoPerfilModify;

        #endregion

        #region IdentidadContadores
        string sqlIdentidadContadoresInsert;
        string sqlIdentidadContadoresDelete;
        string sqlIdentidadContadoresModify;
        #endregion

        #region IdentidadContadoresRecursos
        string sqlIdentidadContadoresRecursosInsert;
        string sqlIdentidadContadoresRecursosDelete;
        string sqlIdentidadContadoresRecursosModify;
        #endregion

        #endregion

        #region M�todos

        #region P�blicos

        //public List<EntityModel.Models.IdentidadDS.Identidad> TestJoin()
        //{
        //    return Joins.ObtenerJoinPerfilIdentidad().JoinPerfilIdentidadPersona().ObtenerJoinPerfilIdentidadPersonaUsuario().Where(item => item.Usuario.Login.Equals("juan")).Select(item => item.Identidad).ToList();
        //}

        public EntityModel.Models.IdentidadDS.Identidad DameUnaIdentidad()
        {
            return mEntityContext.Identidad.First();
        }

        //public IQueryable<JoinIdentidadPerfil> ObtenerJoinPerfilIdentidad()
        //{
        //    return mEntityContext.Perfil.Join(mEntityContext.Identidad, perf => perf.PerfilID, ident => ident.PerfilID, (perf, ident) =>
        //        new JoinIdentidadPerfil
        //        {
        //            Perfil = perf,
        //            Identidad = ident
        //        });
        //}

        public List<Guid> ObtenerComunidadesPrivadas(Guid pIdentidadID)
        {
            List<Guid> listaID = new List<Guid>();
            listaID = mEntityContext.Identidad.JoinProyecto().JoinPerfil().JoinIdentidad().JoinProyecto().Where(item => item.Idenidad.IdentidadID.Equals(pIdentidadID) && (item.ProyectoNoMyGnoss.TipoAcceso.Equals(1) || item.ProyectoNoMyGnoss.TipoAcceso.Equals(3)) && !item.IdentidadNoMyGnoss.FechaBaja.HasValue && !item.IdentidadNoMyGnoss.FechaExpulsion.HasValue && !item.Idenidad.FechaBaja.HasValue && !item.Idenidad.FechaExpulsion.HasValue && item.Perfil.Eliminado.Equals(false)).Select(item => item.ProyectoNoMyGnoss.ProyectoID).ToList();

            return listaID;
        }

        /// <summary>
        /// Actualiza en los perfiles de los usuarios de la persona de la cual ha cambiado su nombre
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <param name="pNombreNuevo">Nombre nuevo de la persona</param>
        /// <param name="pApellidosNuevos">Apellidos nuevos de la persona</param>
        public void ActualizarCambioNombrePersona(Guid pPersonaID, string pNombreNuevo, string pApellidosNuevos)
        {

            var listaPerfiles = mEntityContext.Perfil.Where(item => item.PersonaID.HasValue && item.PersonaID.Value.Equals(pPersonaID)).ToList();

            foreach (var fila in listaPerfiles)
            {
                fila.NombrePerfil = $"{pNombreNuevo} {pApellidosNuevos}";
            }

            var listaIdentidades = mEntityContext.Perfil.JoinIdentidad().Where(item => item.Perfil.PersonaID.HasValue && item.Perfil.PersonaID.Value.Equals(pPersonaID) && (item.Identidad.Tipo < (short)2 || item.Identidad.Tipo.Equals((short)4))).Select(item => item.Identidad).ToList();

            foreach (var fila in listaIdentidades)
            {
                fila.NombreCortoIdentidad = pNombreNuevo;
            }

            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Actualiza el n�mero de conexiones a un proyecto
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        public void ActualizarNumeroConexionesProyecto(Guid pIdentidadID)
        {
            //TODO: revisar
            var resultado = mEntityContext.Identidad.FirstOrDefault(item => item.IdentidadID.Equals(pIdentidadID));
            if (resultado != null)
            {
                resultado.NumConnexiones = resultado.NumConnexiones + 1;

                mEntityContext.SaveChanges();
            }

            //DbCommand commandsqlUpdateNumeroConexionesProyecto = ObtenerComando(this.sqlUpdateNumeroConexionesProyecto = "UPDATE Identidad Set NumConnexiones = NumConnexiones + 1 WHERE IdentidadID = " + IBD.ToParam("IdentidadID"));

            //AgregarParametro(commandsqlUpdateNumeroConexionesProyecto, IBD.ToParam("IdentidadID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadID));

            //ActualizarBaseDeDatos(commandsqlUpdateNumeroConexionesProyecto);
        }

        /// <summary>
        /// Actualiza en los perfiles de los usuarios de la organizaci�n su nombre de perfil
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organziaci�n</param>
        /// <param name="pNombreNuevo">Nombre nuevo de la organizaci�n</param>
        public void ActualizarCambioNombreOrganizacion(Guid pOrganizacionID, string pNombreNuevo, string pAliasNuevo)
        {
            //TODO: revisar 

            List<Guid> listaPerfilID = mEntityContext.Perfil.Join(mEntityContext.Identidad, perfil => perfil.PerfilID, identidad => identidad.PerfilID, (perfil, identidad) =>
            new
            {
                Perfil = perfil,
                Identidad = identidad
            }).Where(item => item.Perfil.OrganizacionID.Equals(pOrganizacionID) && item.Identidad.Tipo.Equals((short)TiposIdentidad.Organizacion)).Select(item => item.Perfil.PerfilID).ToList();

            var resultado = mEntityContext.Perfil.FirstOrDefault(perfil => listaPerfilID.Contains(perfil.PerfilID));
            if (resultado != null)
            {
                resultado.NombrePerfil = pNombreNuevo;
                resultado.NombreOrganizacion = pAliasNuevo;
                mEntityContext.SaveChanges();
            }

            //    //Perfil Org
            //DbCommand commandsqlUpdatePerfilCambioNombre = ObtenerComando(sqlUpdatePerfilCambioNombreOrganizacion = "UPDATE Perfil SET NombrePerfil=" + IBD.ToParam("nombrePerfil") + ", NombreOrganizacion = " + IBD.ToParam("nombreOrganizacion") + " WHERE Perfil.PerfilID IN (Select Perfil.PerfilID FROM Perfil INNER JOIN Identidad ON (Perfil.PerfilID=Identidad.PerfilID) WHERE Perfil.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " AND Identidad.Tipo = " + (short)TiposIdentidad.Organizacion + ")";);
            //AgregarParametro(commandsqlUpdatePerfilCambioNombre, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //AgregarParametro(commandsqlUpdatePerfilCambioNombre, IBD.ToParam("nombreOrganizacion"), DbType.String, pAliasNuevo);
            //AgregarParametro(commandsqlUpdatePerfilCambioNombre, IBD.ToParam("nombrePerfil"), DbType.String, pNombreNuevo);
            //ActualizarBaseDeDatos(commandsqlUpdatePerfilCambioNombre);

            var resultado2 = mEntityContext.Perfil.FirstOrDefault(perfil => perfil.OrganizacionID.Equals(pOrganizacionID));
            if (resultado2 != null)
            {
                resultado2.NombreOrganizacion = pAliasNuevo;
            }

            //    //Perfil de las personas de la Org
            //    DbCommand commandsqlUpdatePerfilPersOrgCambioNombre = ObtenerComando(sqlUpdatePerfilPersOrgCambioNombreOrganizacion = "UPDATE Perfil SET NombreOrganizacion = " + IBD.ToParam("nombreOrganizacion") + " WHERE Perfil.OrganizacionID = " + IBD.GuidParamValor("organizacionID"));
            //AgregarParametro(commandsqlUpdatePerfilPersOrgCambioNombre, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //AgregarParametro(commandsqlUpdatePerfilPersOrgCambioNombre, IBD.ToParam("nombreOrganizacion"), DbType.String, pAliasNuevo);
            //ActualizarBaseDeDatos(commandsqlUpdatePerfilPersOrgCambioNombre);

            List<Guid> listaPerfil = mEntityContext.Perfil.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.PerfilID).ToList();

            var resultado3 = mEntityContext.Identidad.FirstOrDefault(item => listaPerfil.Contains(item.PerfilID) && item.Tipo >= (short)TiposIdentidad.ProfesionalCorporativo && item.Tipo < 4);

            if (resultado3 != null)
            {
                resultado3.NombreCortoIdentidad = pAliasNuevo;
                mEntityContext.SaveChanges();
            }

            //  //Identidades
            //DbCommand commandsqlUpdateIdentidadCambioNombre = ObtenerComando(sqlUpdateIdentidadCambioNombreOrganizacion = "UPDATE Identidad SET NombreCortoIdentidad = " + IBD.ToParam("nombreOrganizacion") + " WHERE Identidad.PerfilID IN (SELECT Perfil.PerfilID FROM Perfil WHERE Perfil.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ") AND Identidad.Tipo >= " + (short)TiposIdentidad.ProfesionalCorporativo + " AND Identidad.Tipo < 4");
            //AgregarParametro(commandsqlUpdateIdentidadCambioNombre, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            //AgregarParametro(commandsqlUpdateIdentidadCambioNombre, IBD.ToParam("nombreOrganizacion"), DbType.String, pAliasNuevo);
            //ActualizarBaseDeDatos(commandsqlUpdateIdentidadCambioNombre);
        }


        /// <summary>
        /// Obtiene los tags de varios grupos en un proyecto
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad de la organizaci�n en el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public Dictionary<Guid, string> ObtenerTagsDeGruposEnProyecto(List<Guid> pListaGrupoID, Guid pProyectoID)
        {
            Dictionary<Guid, string> tags = new Dictionary<Guid, string>();

            foreach (Guid id in pListaGrupoID)
            {
                tags.Add(id, "");
            }

            if (pListaGrupoID.Count > 0)
            {
                var resultado = mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesProyecto().Where(item => item.GrupoIdentidadesProyecto.ProyectoID.Equals(pProyectoID))
                    .Select(item => new { item.GrupoIdentidades.GrupoID, item.GrupoIdentidades.Tags }).ToList();

                if (pListaGrupoID.Count == 1)
                {
                    resultado = resultado.Where(item => item.GrupoID.Equals(pListaGrupoID[0])).ToList();
                }
                else
                {
                    resultado = resultado.Where(item => pListaGrupoID.Contains(item.GrupoID)).ToList();
                }

                foreach (var fila in resultado.Distinct())
                {
                    if (!(fila.Tags == null))
                    {
                        Guid idDoc = (Guid)fila.GrupoID;
                        string tagss = (string)fila.Tags;
                        tags[idDoc] = tagss;
                    }
                }
            }

            return tags;
        }


        /// <summary>
        /// Obtiene los tags de un grupo en un proyecto
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad de la organizaci�n en el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public string ObtenerTagsDeGrupoEnProyecto(Guid pGrupoID, Guid pProyectoID)
        {
            List<Guid> lista = new List<Guid>();
            lista.Add(pGrupoID);
            return ObtenerTagsDeGruposEnProyecto(lista, pProyectoID)[pGrupoID];
        }

        /// <summary>
        /// Actualiza en la base de datos los datos del dataset pasado por par�metro
        /// </summary>
        /// <param name="pDataSet">Dataset</param>
        public void ActualizarBD()
        {
            mEntityContext.SaveChanges();
        }

        private class AmigosPorPrefijoAUX
        {
            public Guid? OrganizacionID { get; set; }
            public Guid? PersonaID { get; set; }
            public string NombreOrganizacion { get; set; }
            public string NombrePerfil { get; set; }
        }

        /// <summary>
        /// Obtiene los primeros nombres de los amigos de una identidad
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pPrefijo">Prefijo por el que tienen que comenzar los nombres</param>
        /// <param name="pNumeroResultados">N�mero de resultados a obtener</param>
        /// <param name="pAmigosConPermiso">TRUE si en vez de obtener los amigos se deben obtener los amigos de organizaci�n a los que se tiene acceso</param>
        /// <param name="pIdentidadOrganizacion">Identidad de organizacion</param>
        public string[] ObtenerNombresIdentidadesAmigosPorPrefijo(Guid pIdentidadID, string pPrefijo, int pNumeroResultados, bool pAmigosConPermiso, Guid pIdentidadOrganizacion, List<string> pListaAnteriores)
        {
            List<Perfil> listaPerfiles = new List<Perfil>();

            if (pAmigosConPermiso)
            {
                if (pIdentidadOrganizacion.Equals(Guid.Empty))
                {
                    var querySelectNombresIdentidadesAmigosPorPrefijoPrimeraParte = mEntityContext.Identidad.JoinPerfil().JoinAmigo().JoinPermisoAmigoOrg().Where(item => item.Amigo.IdentidadID.Equals(pIdentidadOrganizacion) && item.PermisoAmigoOrg.IdentidadUsuarioID.Equals(pIdentidadID) && item.PermisoAmigoOrg.IdentidadAmigoID.Equals(item.Amigo.IdentidadAmigoID) && (item.Perfil.NombreOrganizacion == null ? item.Perfil.NombrePerfil : !item.Perfil.PersonaID.HasValue ? item.Perfil.NombreOrganizacion : item.Perfil.NombrePerfil + " . " + item.Perfil.NombreOrganizacion).Contains(pPrefijo)).Select(item => new AmigosPorPrefijoAUX()
                    {
                        OrganizacionID = item.Perfil.OrganizacionID,
                        PersonaID = item.Perfil.PersonaID,
                        NombreOrganizacion = item.Perfil.NombreOrganizacion,
                        NombrePerfil = item.Perfil.NombrePerfil
                    });

                    var querySelectNombresIdentidadesAmigosPorPrefijoSegundaParte = mEntityContext.Identidad.JoinPerfil().JoinAmigo().JoinAmigoAgGrupo().JoinGrupoAmigos().JoinPermisoGrupoAmigoOrg().Where(item => item.Amigo.IdentidadID.Equals(pIdentidadOrganizacion) && item.PermisoGrupoAmigoOrg.IdentidadUsuarioID.Equals(pIdentidadID) && (item.Perfil.NombreOrganizacion == null ? item.Perfil.NombrePerfil : !item.Perfil.PersonaID.HasValue ? item.Perfil.NombreOrganizacion : item.Perfil.NombrePerfil + " . " + item.Perfil.NombreOrganizacion).Contains(pPrefijo)).Select(item => new AmigosPorPrefijoAUX()
                    {
                        OrganizacionID = item.Perfil.OrganizacionID,
                        PersonaID = item.Perfil.PersonaID,
                        NombreOrganizacion = item.Perfil.NombreOrganizacion,
                        NombrePerfil = item.Perfil.NombrePerfil
                    });

                    var querySelectNombresIdentidadesAmigosPorPrefijoTerceraParte = mEntityContext.Identidad.JoinPerfil().JoinAmigo().Where(item => item.Amigo.IdentidadAmigoID.Equals(pIdentidadID) && (item.Perfil.NombreOrganizacion == null ? item.Perfil.NombrePerfil : !item.Perfil.PersonaID.HasValue ? item.Perfil.NombreOrganizacion : item.Perfil.NombrePerfil + " . " + item.Perfil.NombreOrganizacion).Contains(pPrefijo)).Select(item => new AmigosPorPrefijoAUX()
                    {
                        OrganizacionID = item.Perfil.OrganizacionID,
                        PersonaID = item.Perfil.PersonaID,
                        NombreOrganizacion = item.Perfil.NombreOrganizacion,
                        NombrePerfil = item.Perfil.NombrePerfil
                    });

                    var querySelectNombresIdentidadesAmigosPorPrefijoCuartaParte = mEntityContext.GrupoAmigos.Where(item => item.IdentidadID.Equals(pIdentidadID) && item.Nombre.Contains(pPrefijo)).Select(item => new AmigosPorPrefijoAUX()
                    {
                        OrganizacionID = Guid.Empty,
                        PersonaID = Guid.Empty,
                        NombreOrganizacion = "",
                        NombrePerfil = item.Nombre
                    });

                    var querySelectNombresIdentidadesAmigosPorPrefijoQuintaParte = mEntityContext.GrupoAmigos.JoinPermisoGrupoAmigoOrg().Where(item => item.PermisoGrupoAmigosOrg.IdentidadOrganizacionID.Equals(pIdentidadOrganizacion) && item.PermisoGrupoAmigosOrg.IdentidadUsuarioID.Equals(pIdentidadID) && item.GrupoAmigos.Nombre.Contains(pPrefijo)).Select(item => new AmigosPorPrefijoAUX()
                    {
                        OrganizacionID = Guid.Empty,
                        PersonaID = Guid.Empty,
                        NombreOrganizacion = "",
                        NombrePerfil = item.GrupoAmigos.Nombre
                    });

                    var querySelectNombresIdentidadesAmigosPorPrefijoSextaParte = mEntityContext.Identidad.JoinPerfil().JoinPermisoAmigoOrg().Where(item => item.PermisoAmigoOrg.IdentidadAmigoID.Equals(pIdentidadID) && (item.Perfil.NombreOrganizacion == null ? item.Perfil.NombrePerfil : !item.Perfil.PersonaID.HasValue ? item.Perfil.NombreOrganizacion : item.Perfil.NombrePerfil + " . " + item.Perfil.NombreOrganizacion).Contains(pPrefijo)).Select(item => new AmigosPorPrefijoAUX()
                    {
                        OrganizacionID = item.Perfil.OrganizacionID,
                        PersonaID = item.Perfil.PersonaID,
                        NombreOrganizacion = item.Perfil.NombreOrganizacion,
                        NombrePerfil = item.Perfil.NombrePerfil
                    });

                    var querySelectNombresIdentidadesAmigosPorPrefijoSeptimaParte = mEntityContext.Identidad.JoinPerfil().JoinPermisoGrupoAmigoOrg().JoinGrupoAmigos().JoinAmigoAgGrupo().Where(item => item.AmigoAgGrupo.IdentidadAmigoID.Equals(pIdentidadID) && (item.Perfil.NombreOrganizacion == null ? item.Perfil.NombrePerfil : !item.Perfil.PersonaID.HasValue ? item.Perfil.NombreOrganizacion : item.Perfil.NombrePerfil + " . " + item.Perfil.NombreOrganizacion).Contains(pPrefijo)).Select(item => new AmigosPorPrefijoAUX()
                    {
                        OrganizacionID = item.Perfil.OrganizacionID,
                        PersonaID = item.Perfil.PersonaID,
                        NombreOrganizacion = item.Perfil.NombreOrganizacion,
                        NombrePerfil = item.Perfil.NombrePerfil
                    });

                    var querySelectNombresIdentidadesAmigosPorPrefijoOctavaParte = mEntityContext.GrupoAmigos.JoinIdentidad().JoinPerfil().JoinAdministradorOrganizacion().JoinPersona().JoinPerfil().JoinIdentidad().Where(item => !item.Perfil.PersonaID.HasValue && item.Identidad.Tipo.Equals(3) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && item.I2.IdentidadID.Equals(pIdentidadID) && item.GrupoAmigos.Nombre.Contains(pPrefijo) && !mEntityContext.PersonaVinculoOrganizacion.Any(item2 => item2.OrganizacionID.Equals(item.AdministradorOrganizacion.OrganizacionID) && item2.PersonaID.Equals(item.Persona.PersonaID))).Select(item => new AmigosPorPrefijoAUX()
                    {
                        OrganizacionID = Guid.Empty,
                        PersonaID = Guid.Empty,
                        NombreOrganizacion = "",
                        NombrePerfil = item.GrupoAmigos.Nombre
                    });

                    var querySelectNombresIdentidadesAmigosPorPrefijoNovenaParte = mEntityContext.Amigo.JoinIdentidad().JoinPerfil().JoinIdentidad().JoinPerfil().JoinAdministradorOrganizacion().JoinPersona().JoinPerfil().JoinIdentidad().Where(item => item.IdentAdmin.ProyectoID.Equals(ProyectoAD.MetaProyecto) && item.IdentAdmin.IdentidadID.Equals(pIdentidadID) && (item.Perfil.NombreOrganizacion == null ? item.Perfil.NombrePerfil : !item.Perfil.PersonaID.HasValue ? item.Perfil.NombreOrganizacion : item.Perfil.NombrePerfil + " . " + item.Perfil.NombreOrganizacion).Contains(pPrefijo) && !mEntityContext.PersonaVinculoOrganizacion.Any(item2 => item2.OrganizacionID.Equals(item.AdministradorOrganizacion.OrganizacionID) && item2.PersonaID.Equals(item.Persona.PersonaID))).Select(item => new AmigosPorPrefijoAUX()
                    {
                        OrganizacionID = item.Perfil.OrganizacionID,
                        PersonaID = item.Perfil.PersonaID,
                        NombreOrganizacion = item.Perfil.NombreOrganizacion,
                        NombrePerfil = item.Perfil.NombrePerfil
                    });

                    var querySelectNombresIdentidadesAmigosPorPrefijo = querySelectNombresIdentidadesAmigosPorPrefijoPrimeraParte.Union(querySelectNombresIdentidadesAmigosPorPrefijoSegundaParte).Union(querySelectNombresIdentidadesAmigosPorPrefijoTerceraParte).Union(querySelectNombresIdentidadesAmigosPorPrefijoCuartaParte).Union(querySelectNombresIdentidadesAmigosPorPrefijoQuintaParte).Union(querySelectNombresIdentidadesAmigosPorPrefijoSextaParte).Union(querySelectNombresIdentidadesAmigosPorPrefijoSeptimaParte).Union(querySelectNombresIdentidadesAmigosPorPrefijoOctavaParte).Union(querySelectNombresIdentidadesAmigosPorPrefijoNovenaParte);

                    listaPerfiles = querySelectNombresIdentidadesAmigosPorPrefijo.ToList().Select(item => new Perfil
                    {
                        OrganizacionID = item.OrganizacionID,
                        PersonaID = item.PersonaID,
                        NombreOrganizacion = item.NombreOrganizacion,
                        NombrePerfil = item.NombrePerfil
                    }).OrderBy(item => item.NombrePerfil).Take(pNumeroResultados).ToList();

                    if (pListaAnteriores.Count > 0)
                    {
                        listaPerfiles = querySelectNombresIdentidadesAmigosPorPrefijo.Where(item => !pListaAnteriores.Contains((item.NombreOrganizacion == null ? item.NombrePerfil : !item.PersonaID.HasValue ? item.NombreOrganizacion : item.NombrePerfil + " . " + item.NombreOrganizacion))).ToList().Select(item => new Perfil
                        {
                            OrganizacionID = item.OrganizacionID,
                            PersonaID = item.PersonaID,
                            NombreOrganizacion = item.NombreOrganizacion,
                            NombrePerfil = item.NombrePerfil
                        }).OrderBy(item => item.NombrePerfil).Take(pNumeroResultados).ToList();
                    }
                }
                else
                {


                    var querySelectNombresIdentidadesAmigosPorPrefijoPrimeraParte = mEntityContext.Identidad.JoinPerfil().JoinAmigo().JoinPermisoAmigoOrg().Where(item => item.Amigo.IdentidadID.Equals(pIdentidadOrganizacion) && item.PermisoAmigoOrg.IdentidadUsuarioID.Equals(pIdentidadID) && item.PermisoAmigoOrg.IdentidadAmigoID.Equals(item.Amigo.IdentidadAmigoID) && (item.Perfil.NombreOrganizacion == null ? item.Perfil.NombrePerfil : !item.Perfil.PersonaID.HasValue ? item.Perfil.NombreOrganizacion : item.Perfil.NombrePerfil + " . " + item.Perfil.NombreOrganizacion).Contains(pPrefijo)).Select(item => new AmigosPorPrefijoAUX()
                    {
                        OrganizacionID = item.Perfil.OrganizacionID,
                        PersonaID = item.Perfil.PersonaID,
                        NombreOrganizacion = item.Perfil.NombreOrganizacion,
                        NombrePerfil = item.Perfil.NombrePerfil
                    }).ToList();

                    var querySelectNombresIdentidadesAmigosPorPrefijoSegundaParte = mEntityContext.Identidad.JoinPerfil().JoinAmigo().JoinAmigoAgGrupo().JoinGrupoAmigos().JoinPermisoGrupoAmigoOrg().Where(item => item.Amigo.IdentidadID.Equals(pIdentidadOrganizacion) && item.PermisoGrupoAmigoOrg.IdentidadUsuarioID.Equals(pIdentidadID) && (item.Perfil.NombreOrganizacion == null ? item.Perfil.NombrePerfil : !item.Perfil.PersonaID.HasValue ? item.Perfil.NombreOrganizacion : item.Perfil.NombrePerfil + " . " + item.Perfil.NombreOrganizacion).Contains(pPrefijo)).Select(item => new AmigosPorPrefijoAUX()
                    {
                        OrganizacionID = item.Perfil.OrganizacionID,
                        PersonaID = item.Perfil.PersonaID,
                        NombreOrganizacion = item.Perfil.NombreOrganizacion,
                        NombrePerfil = item.Perfil.NombrePerfil
                    }).ToList();

                    var querySelectNombresIdentidadesAmigosPorPrefijoTerceraParte = mEntityContext.Identidad.JoinPerfil().JoinAmigo().Where(item => item.Amigo.IdentidadAmigoID.Equals(pIdentidadID) && (item.Perfil.NombreOrganizacion == null ? item.Perfil.NombrePerfil : !item.Perfil.PersonaID.HasValue ? item.Perfil.NombreOrganizacion : item.Perfil.NombrePerfil + " . " + item.Perfil.NombreOrganizacion).Contains(pPrefijo)).Select(item => new AmigosPorPrefijoAUX()
                    {
                        OrganizacionID = item.Perfil.OrganizacionID,
                        PersonaID = item.Perfil.PersonaID,
                        NombreOrganizacion = item.Perfil.NombreOrganizacion,
                        NombrePerfil = item.Perfil.NombrePerfil
                    }).ToList();

                    var querySelectNombresIdentidadesAmigosPorPrefijoCuartaParte = mEntityContext.GrupoAmigos.Where(item => item.IdentidadID.Equals(pIdentidadID) && item.Nombre.Contains(pPrefijo)).Select(item => new AmigosPorPrefijoAUX()
                    {
                        OrganizacionID = Guid.Empty,
                        PersonaID = Guid.Empty,
                        NombreOrganizacion = "",
                        NombrePerfil = item.Nombre
                    }).ToList();

                    var querySelectNombresIdentidadesAmigosPorPrefijoQuintaParte = mEntityContext.GrupoAmigos.JoinPermisoGrupoAmigoOrg().Where(item => item.PermisoGrupoAmigosOrg.IdentidadOrganizacionID.Equals(pIdentidadOrganizacion) && item.PermisoGrupoAmigosOrg.IdentidadUsuarioID.Equals(pIdentidadID) && item.GrupoAmigos.Nombre.Contains(pPrefijo)).Select(item => new AmigosPorPrefijoAUX()
                    {
                        OrganizacionID = Guid.Empty,
                        PersonaID = Guid.Empty,
                        NombreOrganizacion = "",
                        NombrePerfil = item.GrupoAmigos.Nombre
                    }).ToList();

                    var querySelectNombresIdentidadesAmigosPorPrefijoSextaParte = mEntityContext.Identidad.JoinPerfil().JoinPermisoAmigoOrg().Where(item => item.PermisoAmigoOrg.IdentidadAmigoID.Equals(pIdentidadID) && (item.Perfil.NombreOrganizacion == null ? item.Perfil.NombrePerfil : !item.Perfil.PersonaID.HasValue ? item.Perfil.NombreOrganizacion : item.Perfil.NombrePerfil + " . " + item.Perfil.NombreOrganizacion).Contains(pPrefijo)).Select(item => new AmigosPorPrefijoAUX()
                    {
                        OrganizacionID = item.Perfil.OrganizacionID,
                        PersonaID = item.Perfil.PersonaID,
                        NombreOrganizacion = item.Perfil.NombreOrganizacion,
                        NombrePerfil = item.Perfil.NombrePerfil
                    }).ToList();

                    var querySelectNombresIdentidadesAmigosPorPrefijoSeptimaParte = mEntityContext.Identidad.JoinPerfil().JoinPermisoGrupoAmigoOrg().JoinGrupoAmigos().JoinAmigoAgGrupo().Where(item => item.AmigoAgGrupo.IdentidadAmigoID.Equals(pIdentidadID) && (item.Perfil.NombreOrganizacion == null ? item.Perfil.NombrePerfil : !item.Perfil.PersonaID.HasValue ? item.Perfil.NombreOrganizacion : item.Perfil.NombrePerfil + " . " + item.Perfil.NombreOrganizacion).Contains(pPrefijo)).Select(item => new AmigosPorPrefijoAUX()
                    {
                        OrganizacionID = item.Perfil.OrganizacionID,
                        PersonaID = item.Perfil.PersonaID,
                        NombreOrganizacion = item.Perfil.NombreOrganizacion,
                        NombrePerfil = item.Perfil.NombrePerfil
                    }).ToList();

                    var querySelectNombresIdentidadesAmigosPorPrefijo = querySelectNombresIdentidadesAmigosPorPrefijoPrimeraParte.Union(querySelectNombresIdentidadesAmigosPorPrefijoSegundaParte).Union(querySelectNombresIdentidadesAmigosPorPrefijoTerceraParte).Union(querySelectNombresIdentidadesAmigosPorPrefijoCuartaParte).Union(querySelectNombresIdentidadesAmigosPorPrefijoQuintaParte).Union(querySelectNombresIdentidadesAmigosPorPrefijoSextaParte).Union(querySelectNombresIdentidadesAmigosPorPrefijoSeptimaParte);

                    listaPerfiles = querySelectNombresIdentidadesAmigosPorPrefijo.ToList().Select(item => new Perfil
                    {
                        OrganizacionID = item.OrganizacionID,
                        PersonaID = item.PersonaID,
                        NombreOrganizacion = item.NombreOrganizacion,
                        NombrePerfil = item.NombrePerfil
                    }).OrderBy(item => item.NombrePerfil).Take(pNumeroResultados).ToList();

                    if (pListaAnteriores.Count > 0)
                    {
                        listaPerfiles = querySelectNombresIdentidadesAmigosPorPrefijo.Where(item => !pListaAnteriores.Contains((item.NombreOrganizacion == null ? item.NombrePerfil : !item.PersonaID.HasValue ? item.NombreOrganizacion : item.NombrePerfil + " . " + item.NombreOrganizacion))).ToList().Select(item => new Perfil
                        {
                            OrganizacionID = item.OrganizacionID,
                            PersonaID = item.PersonaID,
                            NombreOrganizacion = item.NombreOrganizacion,
                            NombrePerfil = item.NombrePerfil
                        }).OrderBy(item => item.NombrePerfil).Take(pNumeroResultados).ToList();
                    }
                }
            }
            else
            {
                if (pIdentidadOrganizacion.Equals(Guid.Empty))
                {
                    var querySelectNombresIdentidadesAmigosPorPrefijoPrimeraParte = mEntityContext.Identidad.JoinPerfil().JoinAmigo().Where(item => item.Amigo.IdentidadID.Equals(pIdentidadID) && (item.Perfil.NombreOrganizacion == null ? item.Perfil.NombrePerfil : !item.Perfil.PersonaID.HasValue ? item.Perfil.NombreOrganizacion : item.Perfil.NombrePerfil + " . " + item.Perfil.NombreOrganizacion).Contains(pPrefijo)).Select(item => new AmigosPorPrefijoAUX()
                    {
                        OrganizacionID = item.Perfil.OrganizacionID,
                        PersonaID = item.Perfil.PersonaID,
                        NombreOrganizacion = item.Perfil.NombreOrganizacion,
                        NombrePerfil = item.Perfil.NombrePerfil
                    });

                    var querySelectNombresIdentidadesAmigosPorPrefijoSegundaParte = mEntityContext.GrupoAmigos.Where(item => item.IdentidadID.Equals(pIdentidadID) && item.Nombre.Contains(pPrefijo)).Select(item => new AmigosPorPrefijoAUX()
                    {
                        OrganizacionID = Guid.Empty,
                        PersonaID = Guid.Empty,
                        NombreOrganizacion = "",
                        NombrePerfil = item.Nombre
                    });

                    var querySelectNombresIdentidadesAmigosPorPrefijoSextaParte = mEntityContext.Identidad.JoinPerfil().JoinPermisoAmigoOrg().Where(item => item.PermisoAmigoOrg.IdentidadAmigoID.Equals(pIdentidadID) && (item.Perfil.NombreOrganizacion == null ? item.Perfil.NombrePerfil : !item.Perfil.PersonaID.HasValue ? item.Perfil.NombreOrganizacion : item.Perfil.NombrePerfil + " . " + item.Perfil.NombreOrganizacion).Contains(pPrefijo)).Select(item => new AmigosPorPrefijoAUX()
                    {
                        OrganizacionID = item.Perfil.OrganizacionID,
                        PersonaID = item.Perfil.PersonaID,
                        NombreOrganizacion = item.Perfil.NombreOrganizacion,
                        NombrePerfil = item.Perfil.NombrePerfil
                    });

                    var querySelectNombresIdentidadesAmigosPorPrefijoSeptimaParte = mEntityContext.Identidad.JoinPerfil().JoinPermisoGrupoAmigoOrg().JoinGrupoAmigos().JoinAmigoAgGrupo().Where(item => item.AmigoAgGrupo.IdentidadAmigoID.Equals(pIdentidadID) && (item.Perfil.NombreOrganizacion == null ? item.Perfil.NombrePerfil : !item.Perfil.PersonaID.HasValue ? item.Perfil.NombreOrganizacion : item.Perfil.NombrePerfil + " . " + item.Perfil.NombreOrganizacion).Contains(pPrefijo)).Select(item => new AmigosPorPrefijoAUX()
                    {
                        OrganizacionID = item.Perfil.OrganizacionID,
                        PersonaID = item.Perfil.PersonaID,
                        NombreOrganizacion = item.Perfil.NombreOrganizacion,
                        NombrePerfil = item.Perfil.NombrePerfil
                    });

                    var querySelectNombresIdentidadesAmigosPorPrefijoOctavaParte = mEntityContext.GrupoAmigos.JoinIdentidad().JoinPerfil().JoinAdministradorOrganizacion().JoinPersona().JoinPerfil().JoinIdentidad().Where(item => !item.Perfil.PersonaID.HasValue && item.Identidad.Tipo.Equals(3) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && item.I2.IdentidadID.Equals(pIdentidadID) && item.GrupoAmigos.Nombre.Contains(pPrefijo) && !mEntityContext.PersonaVinculoOrganizacion.Any(item2 => item2.OrganizacionID.Equals(item.AdministradorOrganizacion.OrganizacionID) && item2.PersonaID.Equals(item.Persona.PersonaID))).Select(item => new AmigosPorPrefijoAUX()
                    {
                        OrganizacionID = Guid.Empty,
                        PersonaID = Guid.Empty,
                        NombreOrganizacion = "",
                        NombrePerfil = item.GrupoAmigos.Nombre
                    });

                    var querySelectNombresIdentidadesAmigosPorPrefijoNovenaParte = mEntityContext.Amigo.JoinIdentidad().JoinPerfil().JoinIdentidad().JoinPerfil().JoinAdministradorOrganizacion().JoinPersona().JoinPerfil().JoinIdentidad().Where(item => item.IdentAdmin.ProyectoID.Equals(ProyectoAD.MetaProyecto) && item.IdentAdmin.IdentidadID.Equals(pIdentidadID) && (item.Perfil.NombreOrganizacion == null ? item.Perfil.NombrePerfil : !item.Perfil.PersonaID.HasValue ? item.Perfil.NombreOrganizacion : item.Perfil.NombrePerfil + " . " + item.Perfil.NombreOrganizacion).Contains(pPrefijo) && !mEntityContext.PersonaVinculoOrganizacion.Any(item2 => item2.OrganizacionID.Equals(item.AdministradorOrganizacion.OrganizacionID) && item2.PersonaID.Equals(item.Persona.PersonaID))).Select(item => new AmigosPorPrefijoAUX()
                    {
                        OrganizacionID = item.Perfil.OrganizacionID,
                        PersonaID = item.Perfil.PersonaID,
                        NombreOrganizacion = item.Perfil.NombreOrganizacion,
                        NombrePerfil = item.Perfil.NombrePerfil
                    });

                    var querySelectNombresIdentidadesAmigosPorPrefijo = querySelectNombresIdentidadesAmigosPorPrefijoPrimeraParte.Union(querySelectNombresIdentidadesAmigosPorPrefijoSegundaParte).Union(querySelectNombresIdentidadesAmigosPorPrefijoSextaParte).Union(querySelectNombresIdentidadesAmigosPorPrefijoSeptimaParte).Union(querySelectNombresIdentidadesAmigosPorPrefijoOctavaParte).Union(querySelectNombresIdentidadesAmigosPorPrefijoNovenaParte);

                    listaPerfiles = querySelectNombresIdentidadesAmigosPorPrefijo.ToList().Select(item => new Perfil
                    {
                        OrganizacionID = item.OrganizacionID,
                        PersonaID = item.PersonaID,
                        NombreOrganizacion = item.NombreOrganizacion,
                        NombrePerfil = item.NombrePerfil
                    }).OrderBy(item => item.NombrePerfil).Take(pNumeroResultados).ToList();

                    if (pListaAnteriores.Count > 0)
                    {
                        listaPerfiles = querySelectNombresIdentidadesAmigosPorPrefijo.Where(item => !pListaAnteriores.Contains((item.NombreOrganizacion == null ? item.NombrePerfil : !item.PersonaID.HasValue ? item.NombreOrganizacion : item.NombrePerfil + " . " + item.NombreOrganizacion))).ToList().Select(item => new Perfil
                        {
                            OrganizacionID = item.OrganizacionID,
                            PersonaID = item.PersonaID,
                            NombreOrganizacion = item.NombreOrganizacion,
                            NombrePerfil = item.NombrePerfil
                        }).OrderBy(item => item.NombrePerfil).Take(pNumeroResultados).ToList();
                    }
                }
                else
                {
                    var querySelectNombresIdentidadesAmigosPorPrefijoPrimeraParte = mEntityContext.Identidad.JoinPerfil().JoinAmigo().Where(item => item.Amigo.IdentidadID.Equals(pIdentidadID) && (item.Perfil.NombreOrganizacion == null ? item.Perfil.NombrePerfil :
                    !item.Perfil.PersonaID.HasValue ? item.Perfil.NombreOrganizacion :
                    item.Perfil.NombrePerfil + " . " + item.Perfil.NombreOrganizacion
                    ).Contains(pPrefijo)).Select(item => new AmigosPorPrefijoAUX()
                    {
                        OrganizacionID = item.Perfil.OrganizacionID,
                        PersonaID = item.Perfil.PersonaID,
                        NombreOrganizacion = item.Perfil.NombreOrganizacion,
                        NombrePerfil = item.Perfil.NombrePerfil
                    });

                    var querySelectNombresIdentidadesAmigosPorPrefijoSegundaParte = mEntityContext.GrupoAmigos.Where(item => item.IdentidadID.Equals(pIdentidadID) && item.Nombre.Contains(pPrefijo)).Select(item => new AmigosPorPrefijoAUX()
                    {
                        OrganizacionID = Guid.Empty,
                        PersonaID = Guid.Empty,
                        NombreOrganizacion = "",
                        NombrePerfil = item.Nombre
                    });

                    var querySelectNombresIdentidadesAmigosPorPrefijoSextaParte = mEntityContext.Identidad.JoinPerfil().JoinPermisoAmigoOrg().Where(item => item.PermisoAmigoOrg.IdentidadAmigoID.Equals(pIdentidadID) && (item.Perfil.NombreOrganizacion == null ? item.Perfil.NombrePerfil : !item.Perfil.PersonaID.HasValue ? item.Perfil.NombreOrganizacion : item.Perfil.NombrePerfil + " . " + item.Perfil.NombreOrganizacion).Contains(pPrefijo)).Select(item => new AmigosPorPrefijoAUX()
                    {
                        OrganizacionID = item.Perfil.OrganizacionID,
                        PersonaID = item.Perfil.PersonaID,
                        NombreOrganizacion = item.Perfil.NombreOrganizacion,
                        NombrePerfil = item.Perfil.NombrePerfil
                    });

                    var querySelectNombresIdentidadesAmigosPorPrefijoSeptimaParte = mEntityContext.Identidad.JoinPerfil().JoinPermisoGrupoAmigoOrg().JoinGrupoAmigos().JoinAmigoAgGrupo().Where(item => item.AmigoAgGrupo.IdentidadAmigoID.Equals(pIdentidadID) && (item.Perfil.NombreOrganizacion == null ? item.Perfil.NombrePerfil : !item.Perfil.PersonaID.HasValue ? item.Perfil.NombreOrganizacion : item.Perfil.NombrePerfil + " . " + item.Perfil.NombreOrganizacion).Contains(pPrefijo)).Select(item => new AmigosPorPrefijoAUX()
                    {
                        OrganizacionID = item.Perfil.OrganizacionID,
                        PersonaID = item.Perfil.PersonaID,
                        NombreOrganizacion = item.Perfil.NombreOrganizacion,
                        NombrePerfil = item.Perfil.NombrePerfil
                    });

                    var querySelectNombresIdentidadesAmigosPorPrefijo = querySelectNombresIdentidadesAmigosPorPrefijoPrimeraParte/*.Union(querySelectNombresIdentidadesAmigosPorPrefijoSegundaParte)*/.Union(querySelectNombresIdentidadesAmigosPorPrefijoSextaParte).Union(querySelectNombresIdentidadesAmigosPorPrefijoSeptimaParte);

                    listaPerfiles = querySelectNombresIdentidadesAmigosPorPrefijo.ToList().Select(item => new Perfil
                    {
                        OrganizacionID = item.OrganizacionID,
                        PersonaID = item.PersonaID,
                        NombreOrganizacion = item.NombreOrganizacion,
                        NombrePerfil = item.NombrePerfil
                    }).OrderBy(item => item.NombrePerfil).Take(pNumeroResultados).ToList();

                    if (pListaAnteriores.Count > 0)
                    {
                        listaPerfiles = querySelectNombresIdentidadesAmigosPorPrefijo.Where(item => !pListaAnteriores.Contains((item.NombreOrganizacion == null ? item.NombrePerfil : !item.PersonaID.HasValue ? item.NombreOrganizacion : item.NombrePerfil + " . " + item.NombreOrganizacion))).ToList().Select(item => new Perfil
                        {
                            OrganizacionID = item.OrganizacionID,
                            PersonaID = item.PersonaID,
                            NombreOrganizacion = item.NombreOrganizacion,
                            NombrePerfil = item.NombrePerfil
                        }).OrderBy(item => item.NombrePerfil).Take(pNumeroResultados).ToList();
                    }
                }
            }

            string[] resultado = Array.Empty<string>();

            if (listaPerfiles.Count > 0)
            {
                resultado = new string[listaPerfiles.Count];
                int i = 0;

                foreach (Perfil filaPerfil in listaPerfiles)
                {
                    if (filaPerfil.OrganizacionID.HasValue && filaPerfil.PersonaID.HasValue)
                    {
                        resultado[i] = filaPerfil.NombrePerfil + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + filaPerfil.NombreOrganizacion;
                    }
                    else if (filaPerfil.OrganizacionID.HasValue)
                    {
                        resultado[i] = filaPerfil.NombreOrganizacion;
                    }
                    else
                    {
                        resultado[i] = filaPerfil.NombrePerfil;
                    }
                    i++;
                }
            }
            return resultado;
        }

        /// <summary>
        /// Obtiene los IDs de las identidades en las comunidades privadas que participa
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <returns>Lista con los IDs de las identiades</returns>
        public List<Guid> ObtenerIdentidadesIDEnMisProyectosPrivados(Guid pIdentidadID)
        {
            List<Guid> listaProyectoID = mEntityContext.Identidad.Join(mEntityContext.Proyecto, ident => ident.ProyectoID, proy => proy.ProyectoID, (ident, proy) =>
            new
            {
                Identidad = ident,
                Proyecto = proy
            }).Join(mEntityContext.Identidad, identPro => identPro.Identidad.PerfilID, id => id.PerfilID, (identPro, id) =>
            new
            {
                Identidad = identPro.Identidad,
                Proyecto = identPro.Proyecto,
                Identidad2 = id
            }).Where(item => item.Identidad2.IdentidadID.Equals(pIdentidadID) && !item.Proyecto.ProyectoID.Equals(ProyectoAD.MetaProyecto) && item.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto) && (item.Proyecto.TipoAcceso.Equals((short)TipoAcceso.Privado) || item.Proyecto.TipoAcceso.Equals((short)TipoAcceso.Reservado))).Select(item => item.Proyecto.ProyectoID).ToList();

            return mEntityContext.Identidad.Where(item => !item.IdentidadID.Equals(pIdentidadID) && listaProyectoID.Contains(item.ProyectoID) && !item.FechaExpulsion.HasValue && !item.FechaBaja.HasValue).Select(item => item.IdentidadID).ToList();
        }

        /// <summary>
        /// Obtiene el nombre al que pertenece ese identificador de perfil
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <returns>Nombre de un perfil</returns>
        public string ObtenerNombredePerfilID(Guid pPerfilID)
        {
            return mEntityContext.Perfil.Where(perfil => perfil.PerfilID.Equals(pPerfilID)).Select(item => item.NombrePerfil).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el nombre corto de una identidad
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <returns></returns>
        public string ObtenerNombreCortoIdentidad(Guid pIdentidadID)
        {
            return mEntityContext.Perfil.Join(mEntityContext.Identidad, perfil => perfil.PerfilID, identidad => identidad.PerfilID, (perfil, identidad) =>
            new
            {
                Perfil = perfil,
                Identidad = identidad
            }).Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID)).Select(item => item.Perfil.NombreCortoUsu).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el nombre corto de un perfil
        /// </summary>
        /// <param name="pPerfilID">Clave del perfil</param>
        /// <returns></returns>
        public KeyValuePair<string, string> ObtenerNombreCortoPerfil(Guid pPerfilID)
        {
            string nombreCortoUsu = "";
            string nombreCortoOrg = "";

            var resultado = mEntityContext.Perfil.Where(perfil => perfil.PerfilID.Equals(pPerfilID)).Select(item => new { item.NombreCortoUsu, item.NombreCortoOrg }).FirstOrDefault();

            if (resultado != null)
            {
                nombreCortoUsu = resultado.NombreCortoUsu;
                nombreCortoOrg = resultado.NombreCortoOrg;
            }

            return new KeyValuePair<string, string>(nombreCortoUsu, nombreCortoOrg);
        }

        /// <summary>
        /// Obtiene el nombre corto de la organizaci�n con la que participa un usuario en un proyecto. NULL si no participa en el proyecto o participa con identidad personal.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pUsuarioID">ID del usuario</param>
        /// <returns>Nombre corto de la organizaci�n con la que participa un usuario en un proyecto. NULL si no participa en el proyecto o participa con identidad personal</returns>
        public string ObtenerNombreCortoOrgPerfilParticipaUsuarioEnProyecto(Guid pProyectoID, Guid pUsuarioID)
        {
            return mEntityContext.Organizacion.Join(mEntityContext.Perfil, org => org.OrganizacionID, perf => perf.OrganizacionID, (org, perf) =>
            new
            {
                Organizacion = org,
                Perfil = perf
            }).Join(mEntityContext.Identidad, orgPerf => orgPerf.Perfil.PerfilID, ident => ident.PerfilID, (orgPerf, ident) =>
            new
            {
                Organizacion = orgPerf.Organizacion,
                Perfil = orgPerf.Perfil,
                Identidad = ident
            }).Join(mEntityContext.Persona, orgPerfId => orgPerfId.Perfil.PersonaID, pers => pers.PersonaID, (orgPerfId, pers) =>
            new
            {
                Organizacion = orgPerfId.Organizacion,
                Perfil = orgPerfId.Perfil,
                Identidad = orgPerfId.Identidad,
                Persona = pers
            }).Where(item => item.Persona.UsuarioID.Value.Equals(pUsuarioID) && item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue).Select(item => item.Organizacion.NombreCorto).FirstOrDefault();
        }


        /// <summary>
        /// Devuelve las organizaciones que han contribuido en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Lista con los identificadores de las organizaciones</returns>
        public List<Guid> ObtenerOrganizacionesDeContribuidoresEnProyecto(Guid pProyectoID)
        {
            return mEntityContext.DocumentoWebVinBaseRecursos.Join(mEntityContext.BaseRecursosProyecto, docWeb => docWeb.BaseRecursosID, docRec => docRec.BaseRecursosID, (docWeb, docRec) =>
            new
            {
                DocumentoWebVinBaseRecursos = docWeb,
                BaseRecursosProyecto = docRec
            }).Join(mEntityContext.Documento, docWebRec => docWebRec.DocumentoWebVinBaseRecursos.DocumentoID, doc => doc.DocumentoID, (docWebRec, doc) =>
            new
            {
                DocumentoWebVinBaseRecursos = docWebRec.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = docWebRec.BaseRecursosProyecto,
                Documento = doc
            }).Join(mEntityContext.Identidad, doc => doc.DocumentoWebVinBaseRecursos.IdentidadPublicacionID.Value, ident => ident.IdentidadID, (doc, ident) =>
            new
            {
                DocumentoWebVinBaseRecursos = doc.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = doc.BaseRecursosProyecto,
                Documento = doc.Documento,
                Identidad = ident
            }).Join(mEntityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) =>
            new
            {
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = item.BaseRecursosProyecto,
                Documento = item.Documento,
                Identidad = item.Identidad,
                Perfil = perfil
            }).Where(item => !item.DocumentoWebVinBaseRecursos.PrivadoEditores && !item.Documento.Eliminado && item.Documento.UltimaVersion.Equals(true) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Perfil.OrganizacionID.HasValue && item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && item.Perfil.OrganizacionID.HasValue).Select(item => item.Perfil.OrganizacionID.Value).Distinct().ToList();
        }


        /// <summary>
        /// Devuelve los perfiles que han contribuido en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Lista con los identificadores de las identidades</returns>
        public List<Guid> ObtenerPerfilesDeContribuidoresEnProyecto(Guid pProyectoID)
        {
            return mEntityContext.DocumentoWebVinBaseRecursos.Join(mEntityContext.BaseRecursosProyecto, docWeb => docWeb.BaseRecursosID, docRec => docRec.BaseRecursosID, (docWeb, docRec) =>
            new
            {
                DocumentoWebVinBaseRecursos = docWeb,
                BaseRecursosProyecto = docRec
            }).Join(mEntityContext.Documento, docWebRec => docWebRec.DocumentoWebVinBaseRecursos.DocumentoID, doc => doc.DocumentoID, (docWebRec, doc) =>
            new
            {
                DocumentoWebVinBaseRecursos = docWebRec.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = docWebRec.BaseRecursosProyecto,
                Documento = doc
            }).Join(mEntityContext.Identidad, doc => doc.DocumentoWebVinBaseRecursos.IdentidadPublicacionID.Value, ident => ident.IdentidadID, (doc, ident) =>
            new
            {
                DocumentoWebVinBaseRecursos = doc.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = doc.BaseRecursosProyecto,
                Documento = doc.Documento,
                Identidad = ident
            }).Join(mEntityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) =>
            new
            {
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = item.BaseRecursosProyecto,
                Documento = item.Documento,
                Identidad = item.Identidad,
                Perfil = perfil
            }).Where(item => !item.DocumentoWebVinBaseRecursos.PrivadoEditores && !item.Documento.Eliminado && item.Documento.UltimaVersion.Equals(true) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue && !item.DocumentoWebVinBaseRecursos.Eliminado && item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && item.Identidad.PerfilID != null).Select(item => item.Identidad.PerfilID).Distinct().ToList();
        }

        /// <summary>
        /// Obtiene los perfiles de una persona (pObtenerSoloActivos--> no eliminado, no fecha de baja,..)
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <param name="pObtenerSoloActivos">TRUE si obtine s�lo lo que est� activo (no eliminado, no fecha de baja,..)</param>
        /// <returns>Dataset de identidad</returns>
        public DataWrapperIdentidad ObtenerPerfilesDePersona(Guid pPersonaID, bool pObtenerSoloActivos)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            var listaPerfil = mEntityContext.Perfil.Where(item => item.PersonaID.HasValue && item.PersonaID.Value.Equals(pPersonaID)).Union(mEntityContext.Perfil.JoinPerfilPersonaOrgConOrganizacionID().Where(item => !item.Perfil.PersonaID.HasValue && item.PerfilPersonaOrg.PersonaID.Equals(pPersonaID)).Select(item => item.Perfil)); //JOIN PERFIL PERSONA ORG??
            var listaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.Where(item => item.PersonaID.Equals(pPersonaID));
            var listaPerfilRedesSociales = mEntityContext.PerfilRedesSociales.JoinIdentidad().JoinPerfil().Where(item => item.Perfil.PersonaID.HasValue && item.Perfil.PersonaID.Value.Equals(pPersonaID));
            var listaIdentidad = mEntityContext.Identidad.JoinPerfil().JoinPersona().Where(item => item.Persona.PersonaID.Equals(pPersonaID)).Select(item => item.Identidad).Union(mEntityContext.Identidad.JoinPerfil().JoinPerfilPersonaOrg().Where(item => item.PerfilPersonaOrg.PersonaID.Equals(pPersonaID)).Select(item => item.Identidad)).Union(mEntityContext.Identidad.JoinPerfilOrganizacion().JoinPerfilPersonaOrganizacion().Where(item => item.PerfilPersonaOrg.PersonaID.Equals(pPersonaID) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto)).Select(item => item.Identidad));
            var listaPerfilPersona = mEntityContext.PerfilPersona.Where(item => item.PersonaID.Equals(pPersonaID));
            var listaPerfilOrganizacion = mEntityContext.PerfilOrganizacion.JoinPerfilPersonaOrg().Where(item => item.PerfilPersonaOrg.PersonaID.Equals(pPersonaID));
            var listaProfesor = mEntityContext.Profesor.JoinPerfil().Where(item => item.Perfil.PersonaID.HasValue && item.Perfil.PersonaID.Equals(pPersonaID));


            if (pObtenerSoloActivos)
            {
                listaPerfil = listaPerfil.Where(item => !item.Eliminado);
                listaIdentidad = listaIdentidad.Where(item => !item.FechaExpulsion.HasValue && !item.FechaBaja.HasValue);
                listaProfesor = listaProfesor.Where(item => !item.Perfil.Eliminado);
            }

            dataWrapper.ListaPerfil = listaPerfil.ToList();
            dataWrapper.ListaPerfilPersonaOrg = listaPerfilPersonaOrg.ToList();
            dataWrapper.ListaPerfilRedesSociales = listaPerfilRedesSociales.Select(item => item.PerfilRedesSociales).ToList();
            dataWrapper.ListaIdentidad = listaIdentidad.ToList();
            dataWrapper.ListaPerfilPersona = listaPerfilPersona.ToList();
            dataWrapper.ListaPerfilOrganizacion = listaPerfilOrganizacion.Select(item => item.PerfilOrganizacion).ToList();
            dataWrapper.ListaProfesor = listaProfesor.Select(item => item.Profesor).ToList();

            if (dataWrapper.ListaPerfilOrganizacion.Any())
            {
                PerfilOrganizacion perfilOrg = dataWrapper.ListaPerfilOrganizacion.First();
                dataWrapper.ListaPerfilRedesSocialesOrganizacion = mEntityContext.PerfilRedesSociales.Where(item => item.PerfilID.Equals(perfilOrg.PerfilID)).ToList();
            }

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene los perfiles de una persona (pObtenerSoloActivos--> no eliminado, no fecha de baja,..)
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <param name="pObtenerSoloActivos">TRUE si obtine s�lo lo que est� activo (no eliminado, no fecha de baja,..)</param>
        /// <returns>Dataset de identidad</returns>
        public DataWrapperIdentidad ObtenerPerfilesDePersona(Guid pPersonaID, bool pObtenerSoloActivos, Guid pIdentidadID)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            var listaPerfil = mEntityContext.Perfil.Where(item => item.PersonaID.HasValue && item.PersonaID.Value.Equals(pPersonaID)).Union(mEntityContext.Perfil.JoinPerfilPersonaOrgConOrganizacionID().Where(item => !item.Perfil.PersonaID.HasValue && item.PerfilPersonaOrg.PersonaID.Equals(pPersonaID)).Select(item => item.Perfil)); //JOIN PERFIL PERSONA ORG??
            var listaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.Where(item => item.PersonaID.Equals(pPersonaID));
            var listaPerfilRedesSociales = mEntityContext.PerfilRedesSociales.JoinIdentidad().JoinPerfil().Where(item => item.Perfil.PersonaID.HasValue && item.Perfil.PersonaID.Value.Equals(pPersonaID));
            var listaIdentidad = mEntityContext.Identidad.Where(item => item.IdentidadID.Equals(pIdentidadID)).Union(mEntityContext.Identidad.JoinPerfil().JoinPersona().Where(item => item.Persona.PersonaID.Equals(pPersonaID) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto)).Select(item => item.Identidad).Union(mEntityContext.Identidad.JoinPerfilOrganizacion().JoinPerfilPersonaOrganizacion().Where(item => item.PerfilPersonaOrg.PersonaID.Equals(pPersonaID) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto)).Select(item => item.Identidad)));
            var listaPerfilPersona = mEntityContext.PerfilPersona.Where(item => item.PersonaID.Equals(pPersonaID));
            var listaPerfilOrganizacion = mEntityContext.PerfilOrganizacion.JoinPerfilPersonaOrg().Where(item => item.PerfilPersonaOrg.PersonaID.Equals(pPersonaID));
            var listaProfesor = mEntityContext.Profesor.JoinPerfil().Where(item => item.Perfil.PersonaID.HasValue && item.Perfil.PersonaID.Equals(pPersonaID));


            if (pObtenerSoloActivos)
            {
                listaPerfil = listaPerfil.Where(item => !item.Eliminado);
                listaIdentidad = listaIdentidad.Where(item => !item.FechaExpulsion.HasValue && !item.FechaBaja.HasValue);
                listaProfesor = listaProfesor.Where(item => !item.Perfil.Eliminado);
            }

            dataWrapper.ListaPerfil = listaPerfil.ToList();
            dataWrapper.ListaPerfilPersonaOrg = listaPerfilPersonaOrg.ToList();
            dataWrapper.ListaPerfilRedesSociales = listaPerfilRedesSociales.Select(item => item.PerfilRedesSociales).ToList();
            dataWrapper.ListaIdentidad = listaIdentidad.ToList();
            dataWrapper.ListaPerfilPersona = listaPerfilPersona.ToList();
            dataWrapper.ListaPerfilOrganizacion = listaPerfilOrganizacion.Select(item => item.PerfilOrganizacion).ToList();
            dataWrapper.ListaProfesor = listaProfesor.Select(item => item.Profesor).ToList();

            if (dataWrapper.ListaPerfilOrganizacion.Any())
            {
                PerfilOrganizacion perfilOrg = dataWrapper.ListaPerfilOrganizacion.First();
                dataWrapper.ListaPerfilRedesSocialesOrganizacion = mEntityContext.PerfilRedesSociales.Where(item => item.PerfilID.Equals(perfilOrg.PerfilID)).ToList();
            }

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene las tablas Perfil e Identidad de personas en un proyecto determinado
        /// </summary>
        /// <param name="pIdentidadesID">Identificadores de las personas</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pIdentidadUsuarioID">Identificador de la identidad del usuario</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerPerfilIdentidadDeIdentidadesEnProyectoNoSuscritas(Guid pIdentidadUsuarioID, List<Guid> pIdentidadesID, Guid pProyectoID)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();
            if (pIdentidadesID.Count > 0)
            {
                dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && pIdentidadesID.Contains(item.Identidad.IdentidadID) && !mEntityContext.Suscripcion.Join(mEntityContext.SuscripcionIdentidadProyecto, suscripcion => suscripcion.SuscripcionID, suscripcionIdentidadProyecto => suscripcionIdentidadProyecto.SuscripcionID, (suscripcion, suscripcionIdentidadProyecto) => new
                {
                    Suscripcion = suscripcion,
                    SuscripcionIdentidadProyecto = suscripcionIdentidadProyecto
                }).Any(item2 => item2.Suscripcion.IdentidadID.Equals(pIdentidadUsuarioID) && item2.SuscripcionIdentidadProyecto.IdentidadID.Equals(item.Identidad.IdentidadID)) && !mEntityContext.Suscripcion.Join(mEntityContext.SuscripcionIdentidadProyecto, suscripcion => suscripcion.SuscripcionID, suscripcionIdentidadProyecto => suscripcionIdentidadProyecto.SuscripcionID, (suscripcion, suscripcionIdentidadProyecto) => new
                {
                    Suscripcion = suscripcion,
                    SuscripcionIdentidadProyecto = suscripcionIdentidadProyecto
                }).Join(mEntityContext.Identidad, join => item.Identidad.PerfilID, identidadEnMyGNOSS => identidadEnMyGNOSS.PerfilID, (join, identidadEnMyGNOSS) => new
                {
                    Suscripcion = join.Suscripcion,
                    SuscripcionIdentidadProyecto = join.SuscripcionIdentidadProyecto,
                    IdentidadEnMyGNOSS = identidadEnMyGNOSS
                }).Any(item2 => item2.Suscripcion.IdentidadID.Equals(pIdentidadUsuarioID) && item2.SuscripcionIdentidadProyecto.IdentidadID.Equals(item2.IdentidadEnMyGNOSS.IdentidadID))).Select(item => item.Perfil).ToList();

                //Identidad
                dataWrapper.ListaIdentidad = mEntityContext.Identidad.JoinPerfil().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && pIdentidadesID.Contains(item.Identidad.IdentidadID) && !mEntityContext.Suscripcion.Join(mEntityContext.SuscripcionIdentidadProyecto, suscripcion => suscripcion.SuscripcionID, suscripcionIdentidadProyecto => suscripcionIdentidadProyecto.SuscripcionID, (suscripcion, suscripcionIdentidadProyecto) => new
                {
                    Suscripcion = suscripcion,
                    SuscripcionIdentidadProyecto = suscripcionIdentidadProyecto
                }).Any(item2 => item2.Suscripcion.IdentidadID.Equals(pIdentidadUsuarioID) && item2.SuscripcionIdentidadProyecto.IdentidadID.Equals(item.Identidad.IdentidadID)) && !mEntityContext.Suscripcion.Join(mEntityContext.SuscripcionIdentidadProyecto, suscripcion => suscripcion.SuscripcionID, suscripcionIdentidadProyecto => suscripcionIdentidadProyecto.SuscripcionID, (suscripcion, suscripcionIdentidadProyecto) => new
                {
                    Suscripcion = suscripcion,
                    SuscripcionIdentidadProyecto = suscripcionIdentidadProyecto
                }).Join(mEntityContext.Identidad, join => item.Identidad.PerfilID, identidadEnMyGNOSS => identidadEnMyGNOSS.PerfilID, (join, identidadEnMyGNOSS) => new
                {
                    Suscripcion = join.Suscripcion,
                    SuscripcionIdentidadProyecto = join.SuscripcionIdentidadProyecto,
                    IdentidadEnMyGNOSS = identidadEnMyGNOSS
                }).Any(item2 => item2.Suscripcion.IdentidadID.Equals(pIdentidadUsuarioID) && item2.SuscripcionIdentidadProyecto.IdentidadID.Equals(item2.IdentidadEnMyGNOSS.IdentidadID))).Select(item => item.Identidad).ToList();

            }
            return dataWrapper;
        }

        /// <summary>
        /// Obtiene una carga ligera de las identidades de la persona
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>DataSet con las identidades de la persona</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDePersonaMuyLigera(Guid pPersonaID)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            dataWrapper.ListaIdentidad = mEntityContext.Perfil.JoinIdentidad().Join(mEntityContext.Proyecto, identPerf => identPerf.Identidad.ProyectoID, proy => proy.ProyectoID, (identPerf, proy) =>
             new
             {
                 Identidad = identPerf.Identidad,
                 Perfil = identPerf.Perfil,
                 Proyecto = proy
             }).Where(item => item.Perfil.PersonaID.HasValue && item.Perfil.PersonaID.Value.Equals(pPersonaID) && !item.Identidad.FechaExpulsion.HasValue && !item.Identidad.FechaBaja.HasValue && !item.Perfil.Eliminado)
             .Select(item => item.Identidad).ToList();

            dataWrapper.ListaPerfil = mEntityContext.Perfil.Where(perfil => perfil.PersonaID.HasValue && perfil.PersonaID.Value.Equals(pPersonaID) && !perfil.Eliminado && !perfil.OrganizacionID.HasValue)
                .Select(item => item)
                .Concat(mEntityContext.Perfil.Join(mEntityContext.Organizacion, perf => perf.OrganizacionID, org => org.OrganizacionID, (perf, org) =>
                new
                {
                    Perfil = perf,
                    Organizacion = org
                }).Where(item => item.Perfil.PersonaID.HasValue && item.Perfil.PersonaID.Value.Equals(pPersonaID) && !item.Perfil.Eliminado)
                .Select(item => item.Perfil)
                .Concat(mEntityContext.Perfil.Join(mEntityContext.Organizacion, perf => new { OrganizacionID = perf.OrganizacionID.Value }, org => new { OrganizacionID = org.OrganizacionID }, (perf, org) =>
                 new
                 {
                     Perfil = perf,
                     Organizacion = org
                 }).Join(mEntityContext.OrganizacionClase, perfOrg => perfOrg.Organizacion.OrganizacionID, clase => clase.OrganizacionID, (perfOrg, clase) =>
                 new
                 {
                     Perfil = perfOrg.Perfil,
                     Organizacion = perfOrg.Organizacion,
                     OrganizacionClase = clase
                 }).Where(item => item.Perfil.PersonaID.HasValue && item.Perfil.PersonaID.Value.Equals(pPersonaID) && !item.Perfil.Eliminado)
                .Select(item => item.Perfil))).ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene las identidades en MyGnoss de la persona pasada por par�metro
        /// </summary>
        ///<param name="pPersonaID">Identidad de persona</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDePersonaDeMyGNOSS(Guid pPersonaID)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            dataWrapper.ListaIdentidad = mEntityContext.Identidad.Join(mEntityContext.Perfil, ident => ident.PerfilID, perf => perf.PerfilID, (ident, perf) =>
            new
            {
                Identidad = ident,
                Perfil = perf
            }).Where(item => item.Perfil.PersonaID.Value.Equals(pPersonaID) && !item.Perfil.OrganizacionID.HasValue && item.Perfil.PersonaID.HasValue && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto)).Select(item => item.Identidad).Distinct().ToList();

            dataWrapper.ListaPerfil = mEntityContext.PerfilPersona.JoinPerfil().Where(item => item.PerfilPersona.PersonaID.Equals(pPersonaID)).Select(item => item.Perfil).ToList();

            dataWrapper.ListaPerfilRedesSociales = mEntityContext.PerfilRedesSociales.Join(mEntityContext.Perfil, redes => redes.PerfilID, perf => perf.PerfilID, (redes, perf) =>
            new
            {
                PerfilRedesSociales = redes,
                Perfil = perf
            }).Where(item => item.Perfil.PersonaID.Value.Equals(pPersonaID))
            .Select(item => item.PerfilRedesSociales).ToList();

            dataWrapper.ListaPerfilPersona = mEntityContext.PerfilPersona.Where(item => item.PersonaID.Equals(pPersonaID)).ToList();

            dataWrapper.ListaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.Where(item => item.PersonaID.Equals(pPersonaID)).ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene el perfil de un usuario en un proyecto
        /// </summary>
        ///<param name="pUsuarioID">UsuarioID</param>
        ///<param name="pProyectoID">ProyectoID</param>
        ///<returns>PerfilID</returns>
        public Guid ObtenerPerfilUsuarioEnProyecto(Guid pUsuarioID, Guid pProyectoID)
        {
            var resultado = mEntityContext.Identidad.Join(mEntityContext.Perfil, ident => ident.PerfilID, perf => perf.PerfilID, (ident, perf) =>
             new
             {
                 Identidad = ident,
                 Perfil = perf
             }).Join(mEntityContext.Persona, identPerf => identPerf.Perfil.PersonaID, pers => pers.PersonaID, (identPerf, pers) =>
             new
             {
                 Identidad = identPerf.Identidad,
                 Perfil = identPerf.Perfil,
                 Persona = pers
             }).Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && item.Persona.UsuarioID.Value.Equals(pUsuarioID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue && (!pProyectoID.Equals(ProyectoAD.MyGnoss) || item.Identidad.Tipo == 0))
              .OrderBy(item => item.Identidad.Tipo)
             .Select(item => item.Identidad.PerfilID).FirstOrDefault();

            return resultado;
        }

        /// <summary>
        /// Obtiene el identidad de un usuario en un proyecto
        /// </summary>
        ///<param name="pUsuarioID">UsuarioID</param>
        ///<param name="pProyectoID">ProyectoID</param>
        ///<returns>identidadID</returns>
        public Guid ObtenerIdentidadUsuarioEnProyecto(Guid pUsuarioID, Guid pProyectoID)
        {
            var resultado = mEntityContext.Identidad.Join(mEntityContext.Perfil, ident => ident.PerfilID, perf => perf.PerfilID, (ident, perf) =>
            new
            {
                Identidad = ident,
                Perfil = perf
            }).Join(mEntityContext.Persona, identPerf => identPerf.Perfil.PersonaID, pers => pers.PersonaID, (identPerf, pers) =>
            new
            {
                Identidad = identPerf.Identidad,
                Perfil = identPerf.Perfil,
                Persona = pers
            }).Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && item.Persona.UsuarioID.Value.Equals(pUsuarioID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue && (!pProyectoID.Equals(ProyectoAD.MyGnoss) || item.Identidad.Tipo == 0))
            .OrderBy(item => item.Identidad.Tipo)
            .Select(item => item.Identidad.IdentidadID).FirstOrDefault();

            return resultado;
        }

        /// <summary>
        /// Obtiene la lista de nombres cortos de los grupos en los que participa un usuario en un proyecto concreto
        /// </summary>
        ///<param name="pUsuarioID">UsuarioID</param>
        ///<param name="pProyectoID">ProyectoID</param>
        ///<returns>Lista de los nombres cortos en los que participa un usuario</returns>
        public List<string> ObtenerGruposDeUsuarioEnProyecto(Guid pUsuarioID, Guid pProyectoID)
        {
            return mEntityContext.GrupoIdentidades.Join(mEntityContext.GrupoIdentidadesProyecto, grupoIdent => grupoIdent.GrupoID, identProy => identProy.GrupoID, (grupoIdent, identProy) =>
            new
            {
                GrupoIdentidades = grupoIdent,
                GrupoIdentidadesProyecto = identProy
            }).Join(mEntityContext.GrupoIdentidadesParticipacion, grupo => grupo.GrupoIdentidades.GrupoID, partic => partic.GrupoID, (grupo, partic) =>
            new
            {
                GrupoIdentidades = grupo.GrupoIdentidades,
                GrupoIdentidadesProyecto = grupo.GrupoIdentidadesProyecto,
                GrupoIdentidadesParticipacion = partic
            }).Join(mEntityContext.Identidad, grupo => grupo.GrupoIdentidadesParticipacion.IdentidadID, ident => ident.IdentidadID, (grupo, ident) =>
            new
            {
                GrupoIdentidades = grupo.GrupoIdentidades,
                GrupoIdentidadesProyecto = grupo.GrupoIdentidadesProyecto,
                GrupoIdentidadesParticipacion = grupo.GrupoIdentidadesParticipacion,
                Identidad = ident
            }).Join(mEntityContext.Perfil, grupo => grupo.Identidad.PerfilID, perf => perf.PerfilID, (grupo, perf) =>
            new
            {
                GrupoIdentidades = grupo.GrupoIdentidades,
                GrupoIdentidadesProyecto = grupo.GrupoIdentidadesProyecto,
                GrupoIdentidadesParticipacion = grupo.GrupoIdentidadesParticipacion,
                Identidad = grupo.Identidad,
                Perfil = perf
            }).Join(mEntityContext.Persona, grupo => grupo.Perfil.PersonaID.Value, pers => pers.PersonaID, (grupo, pers) =>
            new
            {
                GrupoIdentidades = grupo.GrupoIdentidades,
                GrupoIdentidadesProyecto = grupo.GrupoIdentidadesProyecto,
                GrupoIdentidadesParticipacion = grupo.GrupoIdentidadesParticipacion,
                Identidad = grupo.Identidad,
                Perfil = grupo.Perfil,
                Persona = pers
            }).Where(item => item.GrupoIdentidadesProyecto.ProyectoID.Equals(pProyectoID) && item.Persona.UsuarioID.Value.Equals(pUsuarioID) && !item.Persona.Eliminado && !item.Perfil.Eliminado && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue && !item.GrupoIdentidadesParticipacion.FechaBaja.HasValue && !item.GrupoIdentidades.FechaBaja.HasValue)
            .OrderBy(item => item.GrupoIdentidades.NombreCorto)
            .Select(item => item.GrupoIdentidades.NombreCorto).ToList();
        }

        /// <summary>
        /// Obtiene el proyecto al que pertenece la identidad
        /// </summary>
        ///<param name="pIdentidadID">IdentidadID</param>
        ///<returns>ProyectoID</returns>
        public Guid ObtenerProyectoDeIdentidad(Guid pIdentidadID)
        {
            var resultado = mEntityContext.Identidad.Where(identidad => identidad.IdentidadID.Equals(pIdentidadID)).Select(item => item.ProyectoID).FirstOrDefault();

            if (!resultado.Equals(Guid.Empty))
            {
                return resultado;
            }
            else
            {
                return ProyectoAD.MetaProyecto;
            }
        }

        /// <summary>
        /// Obtiene los datos de identidad de una identidad en MyGnoss
        /// </summary>
        ///<param name="pIdentidadID">IdentidadID</param>
        /// <returns>Dataset de identidades</returns>
        public Guid ObtenerIdentidadIDDeMyGNOSSPorIdentidad(Guid pIdentidadID)
        {
            Guid perfilID = mEntityContext.Identidad.Where(item => item.IdentidadID.Equals(pIdentidadID)).Select(item => item.PerfilID).FirstOrDefault();

            var resultado = mEntityContext.Identidad.Where(identidad => identidad.PerfilID.Equals(perfilID) && identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto)).Select(item => item.IdentidadID).FirstOrDefault();

            return resultado;
        }

        /// <summary>
        /// Obtiene el identificador de la identidad en My Gnoss de la organizacion pasada como par�metro por nombre
        /// </summary>
        /// <param name="pNombre">Nombre de la persona</param>
        /// <param name="pNombreOrganizacion">Nombre de la organizaci�n</param>
        /// <returns></returns>
        public Guid ObtenerIdentidadEnOrgIDDeMyGNOSSPorNombre(string pNombre, string pNombreOrganizacion)
        {
            Guid? resultado;
            if (!string.IsNullOrEmpty(pNombreOrganizacion))
            {
                resultado = mEntityContext.Identidad.Join(mEntityContext.Perfil, ident => ident.PerfilID, perf => perf.PerfilID, (ident, perf) =>
             new
             {
                 Identidad = ident,
                 Perfil = perf
             }).Join(mEntityContext.Persona, perfIdent => perfIdent.Perfil.PersonaID, pers => pers.PersonaID, (perfIdent, pers) =>
             new
             {
                 Identidad = perfIdent.Identidad,
                 Perfil = perfIdent.Perfil,
                 Persona = pers
             }).Where(item => !item.Identidad.FechaBaja.HasValue && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && item.Perfil.NombrePerfil.Equals(pNombre) && item.Perfil.NombreOrganizacion.Equals(pNombreOrganizacion) && (item.Identidad.Tipo.Equals((short)TiposIdentidad.ProfesionalPersonal) || item.Identidad.Tipo.Equals((short)TiposIdentidad.ProfesionalCorporativo)))
             .Select(item => item.Identidad.IdentidadID).FirstOrDefault();
            }
            else
            {
                resultado = mEntityContext.Identidad.Join(mEntityContext.Perfil, ident => ident.PerfilID, perf => perf.PerfilID, (ident, perf) =>
            new
            {
                Identidad = ident,
                Perfil = perf
            }).Join(mEntityContext.Persona, perfIdent => perfIdent.Perfil.PersonaID, pers => pers.PersonaID, (perfIdent, pers) =>
            new
            {
                Identidad = perfIdent.Identidad,
                Perfil = perfIdent.Perfil,
                Persona = pers
            }).Where(item => !item.Identidad.FechaBaja.HasValue && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && item.Perfil.NombrePerfil.Equals(pNombre) && item.Identidad.Tipo.Equals((short)TiposIdentidad.Personal))
           .Select(item => item.Identidad.IdentidadID).FirstOrDefault();
            }

            if (resultado.HasValue)
            {
                return resultado.Value;
            }
            else
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Obtiene el identificador del perfil de una identidad
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <returns></returns>
        public Guid? ObtenerPerfilIDDeIdentidadID(Guid pIdentidadID)
        {
            var resultado = mEntityContext.Identidad.Where(item => item.IdentidadID.Equals(pIdentidadID)).Select(item => item.PerfilID).FirstOrDefault();

            if (resultado != null)
            {
                return (Guid)resultado;
            }
            return null;
        }

        /// <summary>
        /// Obtiene los identificadores del perfil de varias identidades
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <returns></returns>
        public Dictionary<Guid, Guid> ObtenerPerfilesIDDeIdentidadesID(List<Guid> pIdentidadesID)
        {
            if (pIdentidadesID != null && pIdentidadesID.Count > 0)
            {
                return mEntityContext.Identidad.Where(item => pIdentidadesID.Contains(item.IdentidadID)).ToDictionary(item => item.IdentidadID, item => item.PerfilID);
            }

            return new Dictionary<Guid, Guid>();
        }

        /// <summary>
        /// Obtiene el identificador del perfil de una identidad
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <returns></returns>
        public Guid? ObtenerPerfilPersonalIDDeUsuarioID(Guid pUsuarioID)
        {
            var resultado = mEntityContext.Perfil.Join(mEntityContext.Persona, perf => perf.PersonaID, pers => pers.PersonaID, (perf, pers) =>
            new
            {
                Perfil = perf,
                Persona = pers
            }).Where(item => item.Persona.UsuarioID.Value.Equals(pUsuarioID) && !item.Perfil.OrganizacionID.HasValue)
            .Select(item => item.Perfil.PerfilID).FirstOrDefault();

            if (resultado != null)
            {
                return (Guid)resultado;
            }
            return null;
        }

        /// <summary>
        /// Obtiene los datos de las identidades de unas identidades en MyGnoss.
        /// </summary>
        ///<param name="pIdentidadesID">IDs de Identidades</param>
        /// <returns>Lista de identidades</returns>
        public List<Guid> ObtenerIdentidadesIDDeMyGNOSSPorIdentidades(List<Guid> pIdentidadesID)
        {
            List<Guid> listaIDsMyGnoss = new List<Guid>();

            if (pIdentidadesID.Count > 0)
            {
                List<Guid> pListaIdentidadID = mEntityContext.Identidad.Where(identidad => pIdentidadesID.Contains(identidad.IdentidadID)).Select(item => item.PerfilID).ToList();

                var resultado = mEntityContext.Identidad.Where(identidad => pListaIdentidadID.Contains(identidad.PerfilID) && identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto)).Select(item => item.IdentidadID);

                foreach (var fila in resultado)
                {
                    listaIDsMyGnoss.Add(fila);
                }
            }

            return listaIDsMyGnoss;
        }

        /// <summary>
        /// Obtiene el identificador de la persona que publica un recurso que es comentado 
        /// </summary>

        /// <param name="pIDComentario">ID del voto</param>
        /// <returns></returns>
        public Guid ObtenerIdentidadComentadorPorIDComentario(Guid pIDComentario)
        {
            var resultado = mEntityContext.Documento.Join(mEntityContext.DocumentoComentario, doc => doc.DocumentoID, coment => coment.DocumentoID, (doc, coment) =>
            new
            {
                Documento = doc,
                DocumentoComentario = coment
            }).Join(mEntityContext.Comentario, doc => doc.DocumentoComentario.ComentarioID, coment => coment.ComentarioID, (doc, coment) =>
            new
            {
                Documento = doc.Documento,
                DocumentoComentario = doc.DocumentoComentario,
                Comentario = coment
            }).Where(item => item.Comentario.ComentarioID.Equals(pIDComentario))
            .Select(item => item.Documento.CreadorID).FirstOrDefault();

            if (resultado != null)
            {
                return (Guid)resultado;
            }
            else
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Obtiene el identificador de la persona que publica un recurso que es votado 
        /// </summary>

        /// <param name="pIDVoto">ID del voto</param>
        /// <returns></returns>
        public Guid ObtenerIdentidadVotadorPorIDVoto(Guid pIDVoto)
        {
            var resultado = mEntityContext.Documento.Join(mEntityContext.VotoDocumento, doc => doc.DocumentoID, voto => voto.DocumentoID, (doc, voto) =>
            new
            {
                Documento = doc,
                VotoDocumento = voto
            }).Join(mEntityContext.Voto, docVoto => docVoto.VotoDocumento.VotoID, voto => voto.VotoID, (docVoto, voto) =>
            new
            {
                Documento = docVoto.Documento,
                VotoDocumento = docVoto.VotoDocumento,
                Voto = voto
            }).Where(item => item.Voto.VotoID.Equals(pIDVoto)).Select(item => item.Documento.CreadorID).FirstOrDefault();

            if (resultado != null)
            {
                return (Guid)resultado;
            }
            else
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Obtiene el identificador de la identidad en My Gnoss de la organizacion pasada como par�metro por nombre
        /// </summary>
        /// <param name="pNombre">Nombre de la persona</param>
        /// <returns></returns>
        public Guid ObtenerIdentidadOrganizacionIDDeMyGNOSSPorNombre(string pNombre)
        {
            var resultado = mEntityContext.Identidad.Join(mEntityContext.Perfil, ident => ident.PerfilID, perf => perf.PerfilID, (ident, perf) =>
            new
            {
                Identidad = ident,
                Perfil = perf
            }).Where(item => !item.Identidad.FechaBaja.HasValue && item.Perfil.NombreOrganizacion.Equals(pNombre) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && item.Identidad.Tipo.Equals((short)TiposIdentidad.Organizacion))
            .Select(item => item.Identidad.IdentidadID).FirstOrDefault();

            return resultado;
        }

        /// <summary>
        /// Obtiene la fecha de alta de una identidad
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <returns>DateTime con la fecha de alta de una identidad. Null en caso de que no exista la identidad</returns>
        public DateTime? ObtenerFechaAltaPorIdentidadID(Guid pIdentidadID)
        {
            return mEntityContext.Identidad.Where(identidad => identidad.IdentidadID.Equals(pIdentidadID)).Select(item => item.FechaAlta).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene identidadID de una persona en un proyecto mediante su nombre corto
        /// </summary>
        /// <param name="pNombre">Nombre corto de la persona</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Guid con la identidadID o Guid.Empty si no existe</returns>
        public Guid ObtenerIdentidadIDPorNombreCorto(string pNombre, Guid pProyectoID)
        {
            var resultado = mEntityContext.Identidad.Join(mEntityContext.Perfil, ident => ident.PerfilID, perf => perf.PerfilID, (ident, perf) =>
            new
            {
                Identidad = ident,
                Perfil = perf
            }).Where(item => !item.Identidad.FechaBaja.HasValue && item.Perfil.NombreCortoUsu.Equals(pNombre) && item.Identidad.ProyectoID.Equals(pProyectoID))
            .Select(item => item.Identidad.IdentidadID).FirstOrDefault();

            return resultado;
        }

        /// <summary>
        /// Obtiene perfilID de una personamediante su nombre corto
        /// </summary>
        /// <param name="pNombre">Nombre corto de la persona</param>
        /// <returns>Guid con la identidadID o Guid.Empty si no existe</returns>
        public Guid ObtenerPerfilIDPorNombreCorto(string pNombre)
        {
            var resultado = mEntityContext.Perfil.Where(item => item.NombreCortoUsu.Equals(pNombre))
                .Select(item => item.PerfilID).FirstOrDefault();

            return resultado;
        }


        /// <summary>
        /// Obtiene el identificador del grupo pasado como par�metro por nombre Nombre grupo . nomrbe comunidad
        /// </summary>
        /// <param name="pNombre">Nombre del grupo</param>
        /// <returns></returns>
        public Guid ObtenerGrupoIDPorNombre(string pNombre)
        {
            Guid resultado = mEntityContext.GrupoIdentidades.Join(mEntityContext.GrupoIdentidadesProyecto, ident => ident.GrupoID, proy => proy.GrupoID, (ident, proy) => new
            {
                GrupoIdentidades = ident,
                GrupoIdentidadesProyecto = proy
            }).Join(mEntityContext.Proyecto, grupo => grupo.GrupoIdentidadesProyecto.ProyectoID, proy => proy.ProyectoID, (grupo, proy) => new
            {
                GrupoIdentidades = grupo.GrupoIdentidades,
                GrupoIdentidadesProyecto = grupo.GrupoIdentidadesProyecto,
                Proyecto = proy
            }).Where(item => pNombre.Equals(item.GrupoIdentidades.Nombre + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + item.Proyecto.NombreCorto)).Select(item => item.GrupoIdentidades.GrupoID).FirstOrDefault();

            if (resultado != Guid.Empty)
            {
                return resultado;
            }
            else
            {
                //Probamos a consultar en el grupo
                return ObtenerGrupoIDPorNombreEnTablaGrupoIdentidades(pNombre);
            }
        }

        /// <summary>
        /// Obtiene el identificador del grupo pasado como par�metro por nombre Nombre grupo . nomrbe comunidad
        /// </summary>
        /// <param name="pNombre">Nombre del grupo</param>
        /// <returns></returns>
        public Guid ObtenerGrupoIDPorNombreYProyectoID(string pNombre, Guid pProyectoID)
        {
            var resultado = mEntityContext.GrupoIdentidades.Join(mEntityContext.GrupoIdentidadesProyecto, gident => gident.GrupoID, gidentProy => gidentProy.GrupoID, (gident, gidentProy) =>
            new
            {
                GrupoIdentidades = gident,
                GrupoIdentidadesProyecto = gidentProy
            }).Where(item => item.GrupoIdentidades.NombreCorto.Equals(pNombre) && item.GrupoIdentidadesProyecto.ProyectoID.Equals(pProyectoID))
            .Select(item => item.GrupoIdentidades.GrupoID).FirstOrDefault();

            return resultado;
        }

        /// <summary>
        /// Obtiene el identificador del grupo pasado como par�metro por nombre Nombre grupo . nomrbe comunidad
        /// </summary>
        /// <param name="pNombre">Nombre del grupo</param>
        /// <returns></returns>
        public Guid ObtenerGrupoIDPorNombreCorto(string pNombreCorto)
        {
            var resultado = mEntityContext.GrupoIdentidades.Where(item => item.NombreCorto.Equals(pNombreCorto)).Select(item => item.GrupoID).FirstOrDefault();

            return resultado;
        }

        /// <summary>
        /// Obtiene el nombrecorto del grupo a partir de su ID.
        /// </summary>
        /// <param name="pGrupoID">ID del grupo</param>
        /// <returns></returns>
        public string ObtenerNombreCortoGrupoPorID(Guid pGrupoID)
        {
            var resultado = mEntityContext.GrupoIdentidades.Where(item => item.GrupoID.Equals(pGrupoID)).Select(item => item.NombreCorto).FirstOrDefault();

            if (resultado != null)
            {
                return (string)resultado;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Obtiene el nombrecorto del grupo a partir de su ID.
        /// </summary>
        /// <param name="pGrupoID">ID del grupo</param>
        /// <returns></returns>
        public List<string> ObtenerNombresCortosGruposPorID(List<Guid> pGruposID)
        {
            List<string> listaGrupos = new List<string>();

            if (pGruposID.Count > 0)
            {
                listaGrupos = mEntityContext.GrupoIdentidades.Where(item => pGruposID.Contains(item.GrupoID)).Select(item => item.NombreCorto).ToList();
            }
            return listaGrupos;
        }

        /// <summary>
        /// Obtiene el identificador del grupo pasado como par�metro por nombre Nombre grupo . nomrbe comunidad
        /// </summary>
        /// <param name="pNombre">Nombre de la persona</param>
        /// <returns></returns>
        public Guid ObtenerGrupoIDPorNombreEnTablaGrupoIdentidades(string pNombre)
        {
            Guid resultado = mEntityContext.GrupoAmigos.Where(item => item.Nombre.Equals(pNombre)).Select(item => item.GrupoID).FirstOrDefault();

            if (resultado != null)
            {
                return resultado;
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Obtiene el identificador de los miembros del grupo en MyGnoss pasado como par�metro
        /// </summary>
        /// <param name="pNombre">Nombre de la persona</param>
        /// <returns></returns>
        public List<Guid> ObtenerListaIdentidadesMyGnossDeGrupos(Guid pGrupoID)
        {
            List<Guid> listaIdentidades = new List<Guid>();

            List<Guid> pListaGrupoID = mEntityContext.AmigoAgGrupo.Join(mEntityContext.Identidad, amigo => amigo.IdentidadAmigoID, ident => ident.IdentidadID, (amigo, ident) =>
            new
            {
                AmigoAGrupo = amigo,
                Identidad = ident
            }).Where(item => item.AmigoAGrupo.GrupoID.Equals(pGrupoID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue)
            .Select(item => item.Identidad.PerfilID)
            .Union(mEntityContext.GrupoIdentidadesParticipacion.Join(mEntityContext.Identidad, grupo => grupo.IdentidadID, ident => ident.IdentidadID, (grupo, ident) =>
            new
            {
                GrupoIdentidadesParticipacion = grupo,
                Identidad = ident
            }).Where(item => item.GrupoIdentidadesParticipacion.GrupoID.Equals(pGrupoID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue)
            .Select(item => item.Identidad.PerfilID)).ToList();

            var resultado = mEntityContext.Identidad.Where(item => pListaGrupoID.Contains(item.PerfilID) && item.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111")) && !item.FechaBaja.HasValue && !item.FechaExpulsion.HasValue)
                .Select(item => item.IdentidadID);

            foreach (var filas in resultado)
            {
                listaIdentidades.Add(filas);
            }

            return listaIdentidades;
        }

        /// <summary>
        /// Obtiene el identificador de la identidad en My Gnoss de la organizacion pasada como par�metro por nombre
        /// </summary>
        /// <param name="pNombre">Nombre de la persona</param>
        /// <returns></returns>
        public Guid ObtenerIdentidadProfesorIDDeMyGNOSSPorNombre(string pNombre)
        {
            var resultado = mEntityContext.Identidad.Join(mEntityContext.Perfil, ident => ident.PerfilID, perf => perf.PerfilID, (ident, perf) =>
            new
            {
                Identidad = ident,
                Perfil = perf
            }).Join(mEntityContext.Persona, identPerf => identPerf.Perfil.PersonaID, pers => pers.PersonaID, (identPerf, pers) =>
            new
            {
                Identidad = identPerf.Identidad,
                Perfil = identPerf.Perfil,
                Persona = pers
            }).Where(item => !item.Identidad.FechaBaja.HasValue && (item.Persona.Nombre + " " + item.Persona.Apellidos).Equals(pNombre) && (item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto)) && item.Identidad.Tipo.Equals((short)TiposIdentidad.Profesor))
            .Select(item => item.Identidad.IdentidadID).FirstOrDefault();

            return resultado;
        }


        private string getNombreBusqueda(Perfil perfil)
        {
            if (perfil.NombreOrganizacion == null)
            {
                return UtilCadenas.RemoveAccentsWithRegEx(perfil.NombrePerfil.ToLower());
            }
            else if (!perfil.PersonaID.HasValue)
            {
                return UtilCadenas.RemoveAccentsWithRegEx(perfil.NombreOrganizacion.ToLower());
            }
            else
            {
                return UtilCadenas.RemoveAccentsWithRegEx(perfil.NombrePerfil.ToLower()) + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + UtilCadenas.RemoveAccentsWithRegEx(perfil.NombreOrganizacion.ToLower());
            }
        }

        /// <summary>
        /// Obtiene los primeros nombres visibles de una identidad en las comunidades privadas que participa
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pPrefijo">Prefijo por el que tienen que comenzar los nombres</param>
        /// <param name="pNumeroResultados">N�mero de resultados a obtener</param>
        public string[] ObtenerNombresIdentidadesPorPrefijoEnMisProyectosPrivados(Guid pIdentidadID, string pPrefijo, int pNumeroResultados, List<string> pListaAnteriores)
        {
            DataWrapperIdentidad dataWrapperIdentidad = new DataWrapperIdentidad();

            var subconsulta = mEntityContext.Identidad.JoinProyecto().JoinProyecto().Where(item => item.Identidad2.IdentidadID.Equals(pIdentidadID) && item.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto) && (item.Proyecto.TipoAcceso.Equals((short)TipoAcceso.Privado) || item.Proyecto.TipoAcceso.Equals((short)TipoAcceso.Reservado)) && !item.Proyecto.ProyectoID.Equals(ProyectoAD.MetaProyecto)).Select(item => item.Proyecto.ProyectoID);

            var selectNombresIdentidadesPorPrefijo = mEntityContext.Perfil.JoinIdentidad().Where(item => subconsulta.Contains(item.Identidad.ProyectoID) && !item.Identidad.FechaExpulsion.HasValue && !item.Identidad.FechaBaja.HasValue && !(mEntityContext.Identidad.Where(ident => ident.IdentidadID.Equals(pIdentidadID)).Select(ident => ident.PerfilID).Distinct()).Contains(item.Perfil.PerfilID)).ToList().Where(item => getNombreBusqueda(item.Perfil).Contains(UtilCadenas.RemoveAccentsWithRegEx(pPrefijo)));
            if (pListaAnteriores.Count > 0)
            {
                selectNombresIdentidadesPorPrefijo = mEntityContext.Perfil.JoinIdentidad().Where(item => subconsulta.Contains(item.Identidad.ProyectoID) && !item.Identidad.FechaExpulsion.HasValue && !item.Identidad.FechaBaja.HasValue && !(mEntityContext.Identidad.Where(ident => ident.IdentidadID.Equals(pIdentidadID)).Select(ident => ident.PerfilID).Distinct()).Contains(item.Perfil.PerfilID)).ToList().Where(item => getNombreBusqueda(item.Perfil).Contains(UtilCadenas.RemoveAccentsWithRegEx(pPrefijo)) && !pListaAnteriores.Contains(getNombreBusqueda(item.Perfil)));

            }

            dataWrapperIdentidad.ListaPerfil = selectNombresIdentidadesPorPrefijo.OrderBy(item => item.Perfil.NombrePerfil).Select(item => item.Perfil).Distinct().Take(pNumeroResultados).ToList();

            string[] resultado = Array.Empty<string>();

            if ((dataWrapperIdentidad.ListaPerfil != null) && (dataWrapperIdentidad.ListaPerfil.Any()))
            {
                resultado = new string[dataWrapperIdentidad.ListaPerfil.Count];
                int i = 0;

                foreach (Perfil filaPerfil in dataWrapperIdentidad.ListaPerfil)
                {
                    if (filaPerfil.OrganizacionID.HasValue && filaPerfil.PersonaID.HasValue)
                    {
                        resultado[i] = filaPerfil.NombrePerfil + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + filaPerfil.NombreOrganizacion;
                    }
                    else if (filaPerfil.OrganizacionID.HasValue)
                    {
                        resultado[i] = filaPerfil.NombreOrganizacion;
                    }
                    else
                    {
                        resultado[i] = filaPerfil.NombrePerfil;
                    }
                    resultado[i] = resultado[i].Trim();

                    i++;
                }
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene la identidad a partir de su identificador
        /// </summary>
        ///<param name="pIdentidadID">Identificador de identidad</param>
        ///<param name="pObtenerSoloActivos">TRUE si debe cargar s�lo lo que no est� eliminado o no tenga fecha de baja</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadPorID(Guid pIdentidadID, bool pObtenerSoloActivos)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();
            if (!pObtenerSoloActivos)
            {
                dataWrapper.ListaIdentidad = mEntityContext.Identidad.Where(identidad => identidad.IdentidadID.Equals(pIdentidadID)).ToList();

                dataWrapper.ListaPerfil = mEntityContext.Perfil.Join(mEntityContext.Identidad, perf => perf.PerfilID, ident => ident.PerfilID, (perf, ident) =>
                new
                {
                    Perfil = perf,
                    Identidad = ident
                }).Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID))
                .Select(item => item.Perfil).ToList();

                dataWrapper.ListaPerfilRedesSociales = mEntityContext.PerfilRedesSociales.Join(mEntityContext.Perfil, redes => redes.PerfilID, perf => perf.PerfilID, (redes, perf) => new
                {
                    PerfilRedesSociales = redes,
                    Perfil = perf
                }).Join(mEntityContext.Identidad, redPerf => redPerf.Perfil.PerfilID, ident => ident.PerfilID, (redPerf, ident) =>
                new
                {
                    PerfilRedesSociales = redPerf.PerfilRedesSociales,
                    Perfil = redPerf.Perfil,
                    Identidad = ident
                }).Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID))
                .Select(item => item.PerfilRedesSociales).ToList();

                dataWrapper.ListaPerfilPersona = mEntityContext.PerfilPersona.JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID))
                .Select(item => item.PerfilPersona).ToList();

                dataWrapper.ListaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID))
                .Select(item => item.PerfilPersonaOrg).ToList();

                dataWrapper.ListaPerfilOrganizacion = mEntityContext.PerfilOrganizacion.JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID))
                .Select(item => item.PerfilOrganizacion).ToList();

                dataWrapper.ListaProfesor = mEntityContext.Profesor.JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID))
                .Select(item => item.Profesor).ToList();

                return dataWrapper;
            }
            else
            {
                dataWrapper.ListaIdentidad = mEntityContext.Identidad.Join(mEntityContext.Identidad, ident => ident.PerfilID, identClave => identClave.PerfilID, (ident, identClave) =>
                new
                {
                    Identidad = ident,
                    IdentClave = identClave
                }).Join(mEntityContext.Identidad, idents => idents.IdentClave.PerfilID, identidadMyGnoss => identidadMyGnoss.PerfilID, (idents, identidadMyGnoss) =>
                new
                {
                    Identidad = idents.Identidad,
                    IdentClave = idents.IdentClave,
                    IdentidadMyGnoss = identidadMyGnoss
                }).Where(item => !item.Identidad.FechaBaja.HasValue && item.IdentidadMyGnoss.ProyectoID.Equals(ProyectoAD.MetaProyecto) && item.IdentClave.IdentidadID.Equals(pIdentidadID) && (item.Identidad.IdentidadID.Equals(item.IdentClave.IdentidadID) || item.Identidad.IdentidadID.Equals(item.IdentidadMyGnoss.IdentidadID)))
                .Select(item => item.Identidad).ToList();

                dataWrapper.ListaPerfil = ObtenerJoinPerfilIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.Perfil.Eliminado)
                .Select(item => item.Perfil).ToList();

                dataWrapper.ListaPerfilRedesSociales = mEntityContext.PerfilRedesSociales.Join(mEntityContext.Perfil, redes => redes.PerfilID, perf => perf.PerfilID, (redes, perf) =>
                 new
                 {
                     PerfilRedesSociales = redes,
                     Perfil = perf
                 }).Join(mEntityContext.Identidad, redesPerf => redesPerf.Perfil.PerfilID, ident => ident.PerfilID, (redesPerf, ident) =>
                 new
                 {
                     PerfilRedesSociales = redesPerf.PerfilRedesSociales,
                     Perfil = redesPerf.Perfil,
                     Identidad = ident
                 }).Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID) && !item.Identidad.FechaBaja.HasValue && !item.Perfil.Eliminado)
                 .Select(item => item.PerfilRedesSociales).ToList();

                dataWrapper.ListaPerfilPersona = mEntityContext.PerfilPersona.Join(mEntityContext.Identidad, perf => perf.PerfilID, ident => ident.PerfilID, (perf, ident) =>
                new
                {
                    PerfilPersona = perf,
                    Identidad = ident
                }).Join(mEntityContext.Perfil, perfIdent => perfIdent.PerfilPersona.PerfilID, perf => perf.PerfilID, (perfIdent, perf) =>
                new
                {
                    PerfilPersona = perfIdent.PerfilPersona,
                    Identidad = perfIdent.Identidad,
                    Perfil = perf
                }).Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID) && !item.Identidad.Perfil.Eliminado && !item.Identidad.FechaBaja.HasValue)
                .Select(item => item.PerfilPersona).ToList();

                dataWrapper.ListaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.Join(mEntityContext.Identidad, org => org.PerfilID, ident => ident.PerfilID, (org, ident) =>
                new
                {
                    PerfilPersonaOrganizacion = org,
                    Identidad = ident
                }).Join(mEntityContext.Perfil, orgIdent => orgIdent.Identidad.PerfilID, perf => perf.PerfilID, (orgIdent, perf) =>
                new
                {
                    PerfilPersonaOrganizacion = orgIdent.PerfilPersonaOrganizacion,
                    Identidad = orgIdent.Identidad,
                    Perfil = perf
                }).Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID) && !item.Perfil.Eliminado && !item.Identidad.FechaBaja.HasValue)
                .Select(item => item.PerfilPersonaOrganizacion).ToList();

                dataWrapper.ListaPerfilOrganizacion = mEntityContext.PerfilOrganizacion.Join(mEntityContext.Identidad, perfOrg => perfOrg.PerfilID, ident => ident.PerfilID, (perfOrg, ident) =>
                new
                {
                    PerfilOrganizacion = perfOrg,
                    Identidad = ident
                }).Join(mEntityContext.Perfil, orgIdent => orgIdent.PerfilOrganizacion.PerfilID, perf => perf.PerfilID, (orgIden, perf) =>
                new
                {
                    PerfilOrganizacion = orgIden.PerfilOrganizacion,
                    Identidad = orgIden.Identidad,
                    Perfil = perf
                }).Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID) && !item.Perfil.Eliminado && !item.Identidad.FechaBaja.HasValue)
                .Select(item => item.PerfilOrganizacion).ToList();

                dataWrapper.ListaProfesor = mEntityContext.Profesor.Join(mEntityContext.Identidad, prof => prof.PerfilID, ident => ident.PerfilID, (prof, ident) =>
                new
                {
                    Profesor = prof,
                    Identidad = ident
                }).Join(mEntityContext.Perfil, profIdent => profIdent.Profesor.PerfilID, perf => perf.PerfilID, (profIdent, perf) =>
                new
                {
                    Profesor = profIdent.Profesor,
                    Identidad = profIdent.Identidad,
                    Perfil = perf
                }).Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID) && !item.Perfil.Eliminado && !item.Identidad.FechaBaja.HasValue)
                .Select(item => item.Profesor).ToList();

                return dataWrapper;
            }
        }

        public IQueryable<JoinIdentidadPerfil> ObtenerJoinPerfilIdentidad()
        {
            return mEntityContext.Perfil.Join(mEntityContext.Identidad, perf => perf.PerfilID, ident => ident.PerfilID, (perf, ident) =>
                new JoinIdentidadPerfil
                {
                    Perfil = perf,
                    Identidad = ident
                });
        }

        public class JoinIdentidadPerfil
        {
            public Perfil Perfil { get; set; }
            public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        }

        /// <summary>
        /// Obtiene los datos Extra de varios usuarios  (En un proyecto y en el ecosistema)
        /// </summary>
        /// <param name="pListaIdentidadesID">Clave de las identidades</param>
        /// <returns>DataSet de Proyecto con las tablas DatoExtraProyecto y DatoExtraProyectoOpcion</returns>
        public DataWrapperIdentidad ObtenerDatosExtraProyectoOpcionIdentidadPorIdentidadID(List<Guid> pListaIdentidadesID)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            if (pListaIdentidadesID.Count > 0)
            {
                dataWrapper.ListaDatoExtraProyectoOpcionIdentidad = mEntityContext.DatoExtraProyectoOpcionIdentidad.Where(item => pListaIdentidadesID.Contains(item.IdentidadID))
                   .ToList();

                var query = mEntityContext.DatoExtraProyectoVirtuosoIdentidad.Join(mEntityContext.DatoExtraProyectoVirtuoso, ident => ident.DatoExtraID, virt => virt.DatoExtraID, (ident, virt) =>
                new
                {
                    DatoExtraProyectoVirtuosoIdentidad = ident,
                    DatoExtraProyectoVirtuoso = virt
                }).Where(item => pListaIdentidadesID.Contains(item.DatoExtraProyectoVirtuosoIdentidad.IdentidadID))
                .OrderBy(item => item.DatoExtraProyectoVirtuoso.Orden)
                    .Select(item => item.DatoExtraProyectoVirtuosoIdentidad);

                dataWrapper.ListaDatoExtraProyectoVirtuosoIdentidad = query.ToList();

                dataWrapper.ListaDatoExtraEcosistemaOpcionPerfil = mEntityContext.DatoExtraEcosistemaOpcionPerfil.Join(mEntityContext.Identidad, perf => perf.PerfilID, ident => ident.PerfilID, (perf, ident) =>
                new
                {
                    DatoExtraEcosistemaOpcionPerfil = perf,
                    Identidad = ident
                }).Where(item => pListaIdentidadesID.Contains(item.Identidad.IdentidadID))
                .Select(item => item.DatoExtraEcosistemaOpcionPerfil).ToList();

                dataWrapper.ListaDatoExtraEcosistemaVirtuosoPerfil = mEntityContext.DatoExtraEcosistemaVirtuosoPerfil.Join(mEntityContext.Identidad, perf => perf.PerfilID, ident => ident.PerfilID, (perf, ident) =>
                new
                {
                    DatoExtraEcosistemaVirtuosoPerfil = perf,
                    Identidad = ident
                }).Where(item => pListaIdentidadesID.Contains(item.Identidad.IdentidadID))
                .Select(item => item.DatoExtraEcosistemaVirtuosoPerfil).ToList();
            }
            return dataWrapper;
        }

        /// <summary>
        /// Obtiene la identidad a partir de su identificador, solo tablas Identidad y Perfil.
        /// </summary>
        ///<param name="pIdentidadID">Identificador de identidad</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadPorIDCargaLigeraTablas(Guid pIdentidadID)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            dataWrapper.ListaIdentidad = mEntityContext.Identidad.Where(identidad => identidad.IdentidadID.Equals(pIdentidadID)).ToList();
            dataWrapper.ListaGrupoIdentidadesParticipacion = mEntityContext.GrupoIdentidadesParticipacion.Where(item => item.IdentidadID.Equals(pIdentidadID)).ToList();
            dataWrapper.ListaPerfil = mEntityContext.Perfil.Join(mEntityContext.Identidad, perf => perf.PerfilID, ident => ident.PerfilID, (perf, ident) =>
            new
            {
                Perfil = perf,
                Identidad = ident
            }).Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID)).Select(item => item.Perfil).ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene la identidadID del profesor a partir de la organizacion que gestiona y la personaID
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>Identificador del perfil del profesor</returns>
        public Guid? ObtenerIdentidadProfesor(Guid pPerfilID)
        {
            return mEntityContext.AdministradorOrganizacion.Join(mEntityContext.Persona, org => org.UsuarioID, pers => pers.UsuarioID, (org, pers) =>
            new
            {
                AdministradorOrganizacion = org,
                Persona = pers
            }).Join(mEntityContext.Perfil, orgPers => orgPers.Persona.PersonaID, perf => perf.PersonaID, (orgPers, perf) =>
            new
            {
                AdministradorOrganizacion = orgPers.AdministradorOrganizacion,
                Persona = orgPers.Persona,
                Perfil = perf
            }).Join(mEntityContext.Identidad, orgPersPerf => orgPersPerf.Perfil.PerfilID, ident => ident.PerfilID, (orgPersPerf, ident) =>
            new
            {
                AdministradorOrganizacion = orgPersPerf.AdministradorOrganizacion,
                Persona = orgPersPerf.Persona,
                Perfil = orgPersPerf.Perfil,
                Identidad = ident
            }).Join(mEntityContext.Perfil, orgPersPerfIdent => orgPersPerfIdent.AdministradorOrganizacion.OrganizacionID, p2 => p2.OrganizacionID, (orgPersPerfIdent, p2) =>
            new
            {
                AdministradorOrganizacion = orgPersPerfIdent.AdministradorOrganizacion,
                Persona = orgPersPerfIdent.Persona,
                Perfil = orgPersPerfIdent.Perfil,
                Identidad = orgPersPerfIdent.Identidad,
                P2 = p2
            }).Join(mEntityContext.Identidad, orgPersPerfIdentP2 => orgPersPerfIdentP2.P2.PerfilID, i2 => i2.PerfilID, (orgPersPerfIdentP2, i2) =>
            new
            {
                AdministradorOrganizacion = orgPersPerfIdentP2.AdministradorOrganizacion,
                Persona = orgPersPerfIdentP2.Persona,
                Perfil = orgPersPerfIdentP2.Perfil,
                Identidad = orgPersPerfIdentP2.Identidad,
                P2 = orgPersPerfIdentP2.P2,
                I2 = i2
            }).Join(mEntityContext.OrganizacionClase, orgPersPerfIdentP2I2 => orgPersPerfIdentP2I2.AdministradorOrganizacion.OrganizacionID, clase => clase.OrganizacionID, (orgPersPerfIdentP2I2, clase) =>
            new
            {
                AdministradorOrganizacion = orgPersPerfIdentP2I2.AdministradorOrganizacion,
                Persona = orgPersPerfIdentP2I2.Persona,
                Perfil = orgPersPerfIdentP2I2.Perfil,
                Identidad = orgPersPerfIdentP2I2.Identidad,
                P2 = orgPersPerfIdentP2I2.P2,
                I2 = orgPersPerfIdentP2I2.I2,
                OrganizacionClase = clase
            }).Join(mEntityContext.GrupoAmigos, orgPersPerfIdentP2I2clase => orgPersPerfIdentP2I2clase.I2.IdentidadID, amigos => amigos.IdentidadID, (orgPersPerfIdentP2I2clase, amigos) =>
            new
            {
                AdministradorOrganizacion = orgPersPerfIdentP2I2clase.AdministradorOrganizacion,
                Persona = orgPersPerfIdentP2I2clase.Persona,
                Perfil = orgPersPerfIdentP2I2clase.Perfil,
                Identidad = orgPersPerfIdentP2I2clase.Identidad,
                P2 = orgPersPerfIdentP2I2clase.P2,
                I2 = orgPersPerfIdentP2I2clase.I2,
                OrganizacionClase = orgPersPerfIdentP2I2clase.OrganizacionClase,
                GrupoAmigos = amigos
            }).Where(item => item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && item.Identidad.Tipo == 4 && item.I2.Tipo == 3 && !item.P2.PersonaID.HasValue && item.I2.ProyectoID.Equals(ProyectoAD.MetaProyecto) && item.P2.PerfilID.Equals(pPerfilID))
            .Select(item => item.Identidad.IdentidadID).Distinct().FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el identificador del perfil de un profesor a partir de un identificador de persona pasado como par�metro
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>Identificador del perfil del profesor</returns>
        public Guid? ObtenerProfesorID(Guid pPersonaID)
        {
            List<Guid> pListaPerfilID = mEntityContext.PerfilPersona.Where(item => item.PersonaID.Equals(pPersonaID)).Select(item => item.PerfilID).ToList();

            return mEntityContext.Profesor.Where(prof => pListaPerfilID.Contains(prof.PerfilID)).Select(item => item.PerfilID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene las identidades que est�n suscritas a algo
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesConSuscripcion()
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            dataWrapper.ListaIdentidad = mEntityContext.Suscripcion.JoinIdentidad().Select(item => item.Identidad).ToList();

            dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().JoinSuscripcion().Select(item => item.Perfil).ToList();

            dataWrapper.ListaPerfilRedesSociales = mEntityContext.PerfilRedesSociales.JoinIdentidad().JoinSuscripcion().Select(item => item.PerfilRedesSociales).ToList();

            dataWrapper.ListaPerfilPersona = mEntityContext.PerfilPersona.JoinIdentidad().JoinSuscripcion().Select(item => item.PerfilPersona).ToList();

            dataWrapper.ListaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.JoinIdentidad().JoinSuscripcion().Select(item => item.PerfilPersonaOrg).ToList();

            dataWrapper.ListaProfesor = mEntityContext.Profesor.JoinIdentidad().JoinSuscripcion().Select(item => item.Profesor).ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene las identidades personales en MyGnoss de las personas pasadas por par�metro
        /// </summary>
        /// <param name="pListaPersonas">Lista de identificadores de personas</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadPersonalDeMyGnossDePersonas(List<Guid> pListaPersonas)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();
            if (pListaPersonas.Count <= 0)
            {
                return dataWrapper;
            }

            dataWrapper.ListaIdentidad = mEntityContext.PerfilPersona.JoinIdentidad().Where(item => item.Identidad.OrganizacionID.Equals(ProyectoAD.MetaProyecto) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && pListaPersonas.Contains(item.PerfilPersona.PersonaID))
            .Select(item => item.Identidad).ToList();

            dataWrapper.ListaPerfilPersona = mEntityContext.PerfilPersona.JoinIdentidad().Where(item => item.Identidad.OrganizacionID.Equals(ProyectoAD.MetaOrganizacion) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && pListaPersonas.Contains(item.PerfilPersona.PersonaID))
            .Select(item => item.PerfilPersona).ToList();

            dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().Join(mEntityContext.PerfilPersona, perfIdent => perfIdent.Identidad.PerfilID, perfPers => perfPers.PerfilID, (perfIdent, perfPers) =>
                new
                {
                    Perfil = perfIdent.Perfil,
                    Identidad = perfIdent.Identidad,
                    PerfilPersona = perfPers
                }).Where(item => item.Identidad.OrganizacionID.Equals(ProyectoAD.MetaOrganizacion) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && pListaPersonas.Contains(item.PerfilPersona.PersonaID))
                .Select(item => item.Perfil).ToList();

            dataWrapper.ListaPerfilRedesSociales = mEntityContext.PerfilRedesSociales.JoinIdentidad().Join(mEntityContext.PerfilPersona, redesIdent => redesIdent.Identidad.PerfilID, perfPers => perfPers.PerfilID, (redesIdent, perfPers) =>
                new
                {
                    PerfilRedesSociales = redesIdent.PerfilRedesSociales,
                    Identidad = redesIdent.Identidad,
                    PerfilPersona = perfPers
                }).Where(item => item.Identidad.OrganizacionID.Equals(ProyectoAD.MetaOrganizacion) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && pListaPersonas.Contains(item.PerfilPersona.PersonaID)
                ).Select(item => item.PerfilRedesSociales).ToList();

            dataWrapper.ListaProfesor = mEntityContext.Profesor.Join(mEntityContext.PerfilPersona, prof => prof.PerfilID, perfPers => perfPers.PerfilID, (prof, perfPers) =>
            new
            {
                Profesor = prof,
                PerfilPersona = perfPers
            }).Where(item => pListaPersonas.Contains(item.PerfilPersona.PersonaID))
            .Select(item => item.Profesor).ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene "Identidad", "Perfil", "PerfilPersonaOrg" y "PerfilPersona" de los usuarios de un determinado proyecto (excepto bloqueados y expulsados) en funci�n de TipoListadoUsuariosCMS
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDeProyectoParaMosaicoIdentidades(Guid pProyectoID, int pNumeroIdentidades, TipoListadoUsuariosCMS pTipoListado)
        {
            List<Guid> listaID = new List<Guid>();

            switch (pTipoListado)
            {
                case TipoListadoUsuariosCMS.MasActivos:
                    var subconsulta = mEntityContext.Identidad.JoinDocumentoWebVinBaseRecursos().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue && !item.Identidad.Tipo.Equals((short)TiposIdentidad.ProfesionalCorporativo) && !item.Identidad.Tipo.Equals((short)TiposIdentidad.Organizacion)).GroupBy(item => item.Identidad.IdentidadID).Select(item => new
                    {
                        //Top = item.Select(item2 => item2.Identidad.IdentidadID).ToList(),
                        IdentidadID = item.Key,
                        NumRecursos = item.Count()
                    }).OrderByDescending(item => item.NumRecursos);
                    var unionSubconsulta = mEntityContext.Identidad.JoinPerfil().JoinPerfilesMiembro().JoinIdentidadesMiembro().JoinDocumentoWebVinBaseRecursos().Where(item => !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue && item.Identidad.Tipo.Equals((short)TiposIdentidad.Organizacion) && item.IdentidadesMiembro.Tipo.Equals((short)TiposIdentidad.ProfesionalCorporativo)).GroupBy(item => item.Identidad.IdentidadID).Select(item => new
                    {
                        //Top = item.Select(item2 => item2.Identidad.IdentidadID).ToList(),
                        IdentidadID = item.Key,
                        NumRecursos = item.Count()
                    }).OrderByDescending(item => item.NumRecursos);

                    listaID = subconsulta.Union(unionSubconsulta).Take(pNumeroIdentidades).OrderByDescending(item => item.NumRecursos).Select(item => item.IdentidadID).ToList();
                    break;
                case TipoListadoUsuariosCMS.MasPopulares:
                    var subQuery = mEntityContext.Identidad.Where(item => item.ProyectoID.Equals(pProyectoID) && !item.FechaBaja.HasValue && !item.FechaExpulsion.HasValue && !item.Tipo.Equals((short)TiposIdentidad.ProfesionalCorporativo)).OrderByDescending(item => item.Rank);

                    listaID = subQuery.Take(pNumeroIdentidades).Select(item => item.IdentidadID).ToList();
                    break;
                case TipoListadoUsuariosCMS.UltimosRegistrados:
                    listaID = mEntityContext.Identidad.Where(item => item.ProyectoID.Equals(pProyectoID) && !item.FechaBaja.HasValue && !item.FechaExpulsion.HasValue).OrderByDescending(item => item.FechaAlta).Take(pNumeroIdentidades).Select(item => item.IdentidadID).ToList();
                    break;
            }
            return ObtenerIdentidadesPorID(listaID, false);
        }

        /// <summary>
        /// Obtiene la identidad personal de MyGNOSS de las personas que tienen un email dado, si son visibles por el usuario con el PersonaID pasado
        /// </summary>
        /// <param name="pEmail">Email buscado</param>
        /// <param name="pPersonaID">PersonaID del usuario que realiza la busqueda</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesVisiblesPorEmail(string pEmail, Guid pPersonaID)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            List<Guid> pListaProyectoID = mEntityContext.Perfil.JoinIdentidad().Where(item => item.Perfil.PersonaID.Value.Equals(pPersonaID) && !item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto))
            .Select(item => item.Identidad.ProyectoID).Distinct().ToList();

            List<Guid> pListaPersonaID = mEntityContext.Perfil.JoinIdentidad().Join(mEntityContext.Persona, identPerf => identPerf.Perfil.PersonaID.Value, pers => pers.PersonaID, (identPerf, pers) =>
            new
            {
                Identidad = identPerf.Identidad,
                Perfil = identPerf.Perfil,
                Persona = pers
            }).Where(item => item.Persona.Email.Equals(pEmail) && !item.Identidad.FechaBaja.HasValue && pListaProyectoID.Contains(item.Identidad.ProyectoID) && !item.Perfil.OrganizacionID.HasValue)
            .Select(item => item.Persona.PersonaID).Distinct().ToList();

            List<Guid> pListaPersonaEmail = mEntityContext.Persona.Where(persona => persona.Email.Equals(pEmail) && persona.EsBuscable.Equals(true))
                .Select(item => item.PersonaID).ToList();


            dataWrapper.ListaIdentidad = mEntityContext.Identidad.Join(mEntityContext.Perfil, ident => ident.PerfilID, perf => perf.PerfilID, (ident, perf) =>
            new
            {
                Identidad = ident,
                Perfil = perf
            }).Where(item => pListaPersonaID.Contains(item.Perfil.PerfilID) && !item.Perfil.OrganizacionID.HasValue && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto))
            .Union(mEntityContext.Identidad.Join(mEntityContext.Perfil, ident => ident.PerfilID, perf => perf.PerfilID, (ident, perf) =>
            new
            {
                Identidad = ident,
                Perfil = perf
            }).Where(item => !item.Perfil.OrganizacionID.HasValue && !item.Identidad.FechaBaja.HasValue && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && item.Perfil.PersonaID.HasValue && pListaPersonaEmail.Contains(item.Perfil.PersonaID.Value)))
            .Select(item => item.Identidad).ToList();

            dataWrapper.ListaPerfil = mEntityContext.Identidad.Join(mEntityContext.Perfil, ident => ident.PerfilID, perf => perf.PerfilID, (ident, perf) =>
           new
           {
               Identidad = ident,
               Perfil = perf
           }).Where(item => pListaPersonaID.Contains(item.Perfil.PerfilID) && !item.Perfil.OrganizacionID.HasValue && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto))
           .Union(mEntityContext.Identidad.Join(mEntityContext.Perfil, ident => ident.PerfilID, perf => perf.PerfilID, (ident, perf) =>
           new
           {
               Identidad = ident,
               Perfil = perf
           }).Where(item => !item.Perfil.OrganizacionID.HasValue && !item.Identidad.FechaBaja.HasValue && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && item.Perfil.PersonaID.HasValue && pListaPersonaEmail.Contains(item.Perfil.PersonaID.Value))).Select(item => item.Perfil).ToList();

            dataWrapper.ListaIdentidadPerfil = mEntityContext.Identidad.Join(mEntityContext.Perfil, ident => ident.PerfilID, perf => perf.PerfilID, (ident, perf) =>
             new
             {
                 Identidad = ident,
                 Perfil = perf
             }).Where(item => pListaPersonaID.Contains(item.Perfil.PerfilID) && !item.Perfil.OrganizacionID.HasValue && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto))
             .Union(mEntityContext.Identidad.Join(mEntityContext.Perfil, ident => ident.PerfilID, perf => perf.PerfilID, (ident, perf) =>
             new
             {
                 Identidad = ident,
                 Perfil = perf
             }).Where(item => !item.Perfil.OrganizacionID.HasValue && !item.Identidad.FechaBaja.HasValue && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && item.Perfil.PersonaID.HasValue && pListaPersonaEmail.Contains(item.Perfil.PersonaID.Value)))
             .ToList().Select
             (
                 item => new EntityModel.Models.IdentidadDS.IdentidadPerfil
                 {
                     PerfilID = item.Perfil.PerfilID,
                     NombrePerfil = item.Perfil.NombrePerfil,
                     Eliminado = item.Perfil.Eliminado,
                     NombreCortoUsu = item.Perfil.NombreCortoUsu,
                     OrganizacionID = item.Identidad.OrganizacionID,
                     PersonaID = item.Perfil.PersonaID,
                     TieneTwitter = item.Perfil.TieneTwitter,
                     CaducidadResSusc = item.Perfil.CaducidadResSusc,
                     IdentidadID = item.Identidad.IdentidadID,
                     ProyectoID = item.Identidad.ProyectoID,
                     FechaAlta = item.Identidad.FechaAlta,
                     NumConnexiones = item.Identidad.NumConnexiones,
                     Tipo = item.Identidad.Tipo,
                     NombreCortoIdentidad = item.Identidad.NombreCortoIdentidad,
                     RecibirNewsLetter = item.Identidad.RecibirNewsLetter,
                     Rank = item.Identidad.Rank,
                     MostrarBienvenida = item.Identidad.MostrarBienvenida,
                     DiasUltActualizacion = item.Identidad.DiasUltActualizacion,
                     ValorAbsoluto = item.Identidad.ValorAbsoluto,
                     ActivoEnComunidad = item.Identidad.ActivoEnComunidad,
                     ActualizaHome = item.Identidad.ActualizaHome,
                     Foto = item.Identidad.Foto,
                     PerfilID1 = item.Identidad.PerfilID
                 }
             ).ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene una apartir de una identidad "pIdentidadEnMygnoss" las identidades que otras organizaciones les han concedido permisos para ser sus contactos. Es decir quien de las organizaciones le han dado permiso para que "pIdentidadEnMygnoss" sea su contacto
        /// </summary>
        /// <param name="pIdentidadEnMygnoss">Clave de la identidad en MyGnoss</param>
        /// <returns>Lista con Guid</returns>
        public List<Guid> ObtenerListaIdentidadesPermitidasPorOrg(Guid pIdentidadEnMygnoss)
        {
            return mEntityContext.PermisoAmigoOrg.JoinIdentidad().Where(item => item.PermisoAmigoOrg.IdentidadAmigoID.Equals(pIdentidadEnMygnoss) && item.Identidad.FechaBaja.HasValue && item.Identidad.FechaExpulsion.HasValue).Select(item => item.PermisoAmigoOrg.IdentidadUsuarioID).Distinct().ToList();
        }


        /// <summary>
        /// Obtiene los GUIDs de los amigos que pertenecen a un proyecto
        /// </summary>
        /// <param name="pIdentidadMyGnossActual">Identificador de la identidad actual en MyGnoss</param>
        /// <param name="pProyecto">Identificador de proyecto</param>
        /// <returns>Dataset de identidades</returns>
        public List<Guid> ObtenerListaIdentidadesAmigosPertenecenProyecto(Guid pIdentidadMyGnossActual, Guid pProyecto)
        {
            var resultado = mEntityContext.Amigo.JoinIdentidad().Join(mEntityContext.Perfil.JoinIdentidad().JoinPersona().JoinProyetoRolUsuario()
                .Where(item => item.Identidad.ProyectoID.Equals(pProyecto) && item.Identidad.ProyectoID.Equals(item.ProyectoRolUsuario.ProyectoID) && !item.ProyectoRolUsuario.EstaBloqueado && !item.Perfil.Eliminado && !item.Identidad.FechaBaja.HasValue)
                .Select(item => item.Identidad)
                .Union(mEntityContext.Perfil.JoinIdentidad()
                .Where(item => item.Identidad.ProyectoID.Equals(pProyecto) && !item.Perfil.PersonaID.HasValue && item.Perfil.OrganizacionID.HasValue && !item.Perfil.Eliminado && !item.Identidad.FechaBaja.HasValue)
                .Select(item => item.Identidad)), joinIdentidad => joinIdentidad.Identidad.PerfilID, perfiles => perfiles.PerfilID, (joinIdentidad, perfiles) =>
            new
            {
                Idenitdad = joinIdentidad.Identidad,
                IdentidadAmigoID = joinIdentidad.Amigo.IdentidadAmigoID
            }).Where(item => item.IdentidadAmigoID.Equals(pIdentidadMyGnossActual)).Select(item => item.Idenitdad.IdentidadID).Distinct().ToList();

            return resultado;
        }

        /// <summary>
        /// Obtiene los datos de Amigos que pertenecen a un proyecto
        /// </summary>
        /// <param name="pIdentidadMyGnossActual">Identificador de la identidad actual en MyGnoss</param>
        /// <param name="pProyecto">Identificador de proyecto</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesAmigosPertenecenProyecto(Guid pIdentidadMyGnossActual, Guid pProyecto)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            var subconsultaIdentidad = mEntityContext.Perfil.JoinIdentidad().JoinPersona().JoinProyetoRolUsuario()
                .Where(item => item.Identidad.ProyectoID.Equals(pProyecto) && item.Identidad.ProyectoID.Equals(item.ProyectoRolUsuario.ProyectoID) && !item.ProyectoRolUsuario.EstaBloqueado && !item.Perfil.Eliminado && !item.Identidad.FechaBaja.HasValue).Select(item => item.Identidad)
                .Union(mEntityContext.Perfil.JoinIdentidad().Where(item => item.Identidad.ProyectoID.Equals(pProyecto) && !item.Perfil.PersonaID.HasValue && item.Perfil.OrganizacionID.HasValue && !item.Perfil.Eliminado && !item.Identidad.FechaBaja.HasValue).Select(item => item.Identidad));

            dataWrapper.ListaIdentidad = mEntityContext.Amigo.JoinIdentidad().Join(subconsultaIdentidad, joinIdentidad => joinIdentidad.Identidad.PerfilID, perfiles => perfiles.PerfilID, (joinIdentidad, perfiles) =>
             new
             {
                 Identidad = joinIdentidad.Identidad,
                 IdentidadAmigoID = joinIdentidad.Amigo.IdentidadAmigoID
             }).Where(objeto => objeto.IdentidadAmigoID.Equals(pIdentidadMyGnossActual)).Select(objeto => objeto.Identidad).Distinct().ToList();

            var subconsultaPerfil = mEntityContext.Perfil.JoinIdentidad().JoinPersona().JoinProyetoRolUsuario().Where(item => item.Identidad.ProyectoID.Equals(pProyecto) && item.Identidad.ProyectoID.Equals(item.ProyectoRolUsuario.ProyectoID) && !item.ProyectoRolUsuario.EstaBloqueado && !item.Perfil.Eliminado && !item.Identidad.FechaBaja.HasValue).Select(item => item.Identidad)
                .Union(mEntityContext.Perfil.JoinIdentidad().Where(item => item.Identidad.ProyectoID.Equals(pProyecto) && !item.Perfil.PersonaID.HasValue && item.Perfil.OrganizacionID.HasValue && !item.Perfil.Eliminado && !item.Identidad.FechaBaja.HasValue).Select(item => item.Identidad));

            var listaPerfil = mEntityContext.Perfil.JoinIdentidad().JoinAmigo().Join(subconsultaPerfil, perfIdentAmigo => perfIdentAmigo.Identidad.PerfilID, perfiles => perfiles.PerfilID, (perfIdentAmigo, perfiles) =>
            new
            {
                Identidad = perfIdentAmigo.Identidad,
                Perfil = perfIdentAmigo.Perfil,
                IdentidadAmigoID = perfIdentAmigo.Amigo.IdentidadAmigoID
            }).Where(item => item.IdentidadAmigoID.Equals(pIdentidadMyGnossActual)).Select(item => item.Perfil).Distinct();

            dataWrapper.ListaPerfil = listaPerfil.ToList();

            var subconsultaPerfPers = mEntityContext.Perfil.JoinIdentidad().JoinPersona().JoinProyetoRolUsuario().Where(item => item.Identidad.ProyectoID.Equals(pProyecto) && item.Identidad.ProyectoID.Equals(item.ProyectoRolUsuario.ProyectoID) && !item.ProyectoRolUsuario.EstaBloqueado && !item.Perfil.Eliminado && !item.Identidad.FechaBaja.HasValue).Select(item => item.Identidad)
                .Union(mEntityContext.Perfil.JoinIdentidad().Where(item => item.Identidad.ProyectoID.Equals(pProyecto) && !item.Perfil.PersonaID.HasValue && item.Perfil.OrganizacionID.HasValue && !item.Perfil.Eliminado && !item.Identidad.FechaBaja.HasValue).Select(item => item.Identidad));

            var listaPerfilPersona = mEntityContext.PerfilPersona.JoinIdentidad().JoinAmigo().Join(subconsultaPerfPers, perfPersIdentAmigo => perfPersIdentAmigo.Identidad.PerfilID, perfiles => perfiles.PerfilID, (perfPersIdentAmigo, perfiles) =>
            new
            {
                Identidad = perfPersIdentAmigo.Identidad,
                PerfilPersona = perfPersIdentAmigo.PerfilPersona,
                IdentidadAmigoID = perfPersIdentAmigo.Amigo.IdentidadAmigoID
            }).Where(item => item.IdentidadAmigoID.Equals(pIdentidadMyGnossActual)).Select(item => item.PerfilPersona).Distinct();

            dataWrapper.ListaPerfilPersona = listaPerfilPersona.ToList();

            var subconsultaPerfilPersonaOrg = mEntityContext.Perfil.JoinIdentidad().JoinPersona().JoinProyetoRolUsuario().Where(item => item.Identidad.ProyectoID.Equals(pProyecto) && item.Identidad.ProyectoID.Equals(item.ProyectoRolUsuario.ProyectoID) && !item.ProyectoRolUsuario.EstaBloqueado && !item.Perfil.Eliminado && !item.Identidad.FechaBaja.HasValue).Select(item => item.Identidad)
                .Union(mEntityContext.Perfil.JoinIdentidad().Where(item => item.Identidad.ProyectoID.Equals(pProyecto) && !item.Perfil.PersonaID.HasValue && item.Perfil.OrganizacionID.HasValue && !item.Perfil.Eliminado && !item.Identidad.FechaBaja.HasValue).Select(item => item.Identidad));

            dataWrapper.ListaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.JoinIdentidad().JoinAmigo().Join(subconsultaPerfilPersonaOrg, perfPersOrgIdentAmigo => perfPersOrgIdentAmigo.Identidad.PerfilID, perfiles => perfiles.PerfilID, (perfPersOrgIdentAmigo, perfiles) =>
            new
            {
                Identidad = perfPersOrgIdentAmigo.Identidad,
                PerfilPersonaOrg = perfPersOrgIdentAmigo.PerfilPersonaOrg,
                IdentidadAmigoID = perfPersOrgIdentAmigo.Amigo.IdentidadAmigoID
            }).Where(item => item.IdentidadAmigoID.Equals(pIdentidadMyGnossActual)).Select(item => item.PerfilPersonaOrg).Distinct().ToList();

            var subconsultaPerfilOrganizacion = mEntityContext.Perfil.JoinIdentidad().JoinPersona().JoinProyetoRolUsuario().Where(item => item.Identidad.ProyectoID.Equals(pProyecto) && item.Identidad.ProyectoID.Equals(item.ProyectoRolUsuario.ProyectoID) && !item.ProyectoRolUsuario.EstaBloqueado && !item.Perfil.Eliminado && !item.Identidad.FechaBaja.HasValue).Select(item => item.Identidad)
                .Union(mEntityContext.Perfil.JoinIdentidad().Where(item => item.Identidad.ProyectoID.Equals(pProyecto) && !item.Perfil.PersonaID.HasValue && item.Perfil.OrganizacionID.HasValue && !item.Perfil.Eliminado && !item.Identidad.FechaBaja.HasValue).Select(item => item.Identidad));

            dataWrapper.ListaPerfilOrganizacion = mEntityContext.PerfilOrganizacion.JoinIdentidad().JoinAmigo().Join(subconsultaPerfilOrganizacion, perfOrgIdentAmigo => perfOrgIdentAmigo.Identidad.PerfilID, perfiles => perfiles.PerfilID, (perfOrgIdentAmigo, perfiles) =>
            new
            {
                Identidad = perfOrgIdentAmigo.Identidad,
                PerfilOrganizacion = perfOrgIdentAmigo.PerfilOrganizacion,
                IdentidadAmigoID = perfOrgIdentAmigo.Amigo.IdentidadAmigoID
            }).Where(item => item.Identidad.IdentidadID.Equals(pIdentidadMyGnossActual)).Select(item => item.PerfilOrganizacion).Distinct().ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene los datos de Amigos
        /// </summary>
        /// <param name="pIdentidadMyGnossActual">Identificador de la identidad actual en MyGnoss</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesAmigos(Guid pIdentidadMyGnossActual)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            dataWrapper.ListaIdentidad = mEntityContext.Amigo.JoinIdentidad().Where(item => item.Amigo.IdentidadAmigoID.Equals(pIdentidadMyGnossActual)).Select(item => item.Identidad).Distinct().ToList();

            var query = mEntityContext.Perfil.JoinIdentidad().JoinAmigo().Where(item => item.Amigo.IdentidadAmigoID.Equals(pIdentidadMyGnossActual)).Select(item => item.Perfil).Distinct();
            dataWrapper.ListaPerfil = query.ToList();

            dataWrapper.ListaPerfilRedesSociales = mEntityContext.PerfilRedesSociales.JoinIdentidad().JoinAmigo().Where(item => item.Amigo.IdentidadAmigoID.Equals(pIdentidadMyGnossActual)).Select(item => item.PerfilRedesSociales).Distinct().ToList();

            dataWrapper.ListaPerfilPersona = mEntityContext.PerfilPersona.JoinIdentidad().JoinAmigo().Where(item => item.Amigo.IdentidadAmigoID.Equals(pIdentidadMyGnossActual)).Select(item => item.PerfilPersona).Distinct().ToList();

            dataWrapper.ListaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.JoinIdentidad().JoinAmigo().Where(item => item.Amigo.IdentidadAmigoID.Equals(pIdentidadMyGnossActual)).Select(item => item.PerfilPersonaOrg).Distinct().ToList();

            dataWrapper.ListaPerfilOrganizacion = mEntityContext.PerfilOrganizacion.JoinIdentidad().JoinAmigo().Where(item => item.Amigo.IdentidadAmigoID.Equals(pIdentidadMyGnossActual)).Select(item => item.PerfilOrganizacion).Distinct().ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Verdad si la identidad es de tipo profesor, flaso en caso contrario
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pNombreCortoOrg">Nombre de la clase a la que intenta acceder</param>
        /// <returns></returns>
        public bool ComprobarIdentidadDeProfesor(Guid pIdentidadID, string pNombreCortoOrg)
        {
            return mEntityContext.PerfilPersonaOrg.JoinIdentidad().JoinOrganizacion().Any(item => item.Identidad.IdentidadID.Equals(pIdentidadID) && item.Identidad.Tipo == 4 && item.Organizacion.NombreCorto.Equals(pNombreCortoOrg));
        }

        /// <summary>
        /// Devuelve una lista con las identidades ID de las clases del profesor.
        /// </summary>
        /// <param name="guid">IdentidadID del profesor</param>
        /// <returns>Lista de ID's de las clases del profesor</returns>
        public List<Guid> ObtenerClasesProfesor(Guid pIdentidadID)
        {
            var query = mEntityContext.Perfil.JoinIdentidad().JoinPersona().Join(mEntityContext.AdministradorOrganizacion, perfIdentPers => perfIdentPers.Persona.UsuarioID.Value, admin => admin.UsuarioID, (perfIdentPers, admin) =>
            new
            {
                Identidad = perfIdentPers.Identidad,
                Perfil = perfIdentPers.Perfil,
                Persona = perfIdentPers.Persona,
                AdministradorOrganizacion = admin
            }).Join(mEntityContext.Perfil, join => join.AdministradorOrganizacion.OrganizacionID, p2 => p2.OrganizacionID.Value, (join, p2) =>
            new
            {
                Identidad = join.Identidad,
                Perfil = join.Perfil,
                Persona = join.Persona,
                AdministradorOrganizacion = join.AdministradorOrganizacion,
                Perfil2 = p2
            }).Join(mEntityContext.Identidad, join => join.Perfil2.PerfilID, i2 => i2.PerfilID, (join, i2) =>
            new
            {
                Identidad = join.Identidad,
                Perfil = join.Perfil,
                Persona = join.Persona,
                AdministradorOrganizacion = join.AdministradorOrganizacion,
                Perfil2 = join.Perfil2,
                Identidad2 = i2
            }).Where(item => item.Identidad2.Tipo == 3 && item.Identidad2.ProyectoID.Equals(ProyectoAD.MetaProyecto) && item.Identidad.IdentidadID.Equals(pIdentidadID)).Select(item => item.Identidad2.IdentidadID);

            return query.ToList();
        }

        /// <summary>
        /// Obtiene las identidades de las organizaciones que el usuario administra pero no participa en ellas
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pProyectoID">Proyecto al que deben pertenecer los amigos (null si se quieren obtener todos)</param>
        /// <returns></returns>
        public List<EntityModel.Models.IdentidadDS.Identidad> ObtenerIdentidadesDeMyGnossDEOrganizacionesAdministradas(Guid pUsuarioID, Guid? pProyectoID)
        {
            List<Guid> listaOrganizacionID = mEntityContext.PersonaVinculoOrganizacion.Join(mEntityContext.Persona, org => org.PersonaID, pers => pers.PersonaID, (org, pers) =>
           new
           {
               PersonaVinculoOrganizacion = org,
               Persona = pers
           }).Where(item => item.Persona.UsuarioID.Value.Equals(pUsuarioID)).Select(item => item.PersonaVinculoOrganizacion.OrganizacionID).ToList();

            List<Guid> listaPerfilID = mEntityContext.Identidad.Where(identidad => identidad.ProyectoID.Equals(pProyectoID.Value)).Select(item => item.PerfilID).ToList();

            var query = mEntityContext.Perfil.JoinIdentidad().Join(mEntityContext.AdministradorOrganizacion, perfIdent => perfIdent.Perfil.OrganizacionID, admin => admin.OrganizacionID, (perfIdent, admin) =>
            new
            {
                Perfil = perfIdent.Perfil,
                Identidad = perfIdent.Identidad,
                AdministradorOrganizacion = admin
            });

            if (pProyectoID != null)
            {
                query = query.Where(item => item.AdministradorOrganizacion.UsuarioID.Equals(pUsuarioID) && !item.Perfil.PersonaID.HasValue && item.Identidad.Tipo == 3 && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !listaOrganizacionID.Contains(item.AdministradorOrganizacion.OrganizacionID) && listaPerfilID.Contains(item.Perfil.PerfilID));
            }
            else
            {
                query = query.Where(item => item.AdministradorOrganizacion.UsuarioID.Equals(pUsuarioID) && !item.Perfil.PersonaID.HasValue && item.Identidad.Tipo == 3 && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !listaOrganizacionID.Contains(item.AdministradorOrganizacion.OrganizacionID));
            }

            var resultado = query.Select(item => item.Identidad).ToList();

            return resultado;
        }

        /// <summary>
        /// Obtiene las identidades en MyGnoss que no pertenecen a una organizaci�n
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organizaci�n</param>
        /// <param name="pNombre">Nombre</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesMyGnossNoPerteneceOrg(Guid pOrganizacionID, string pNombre)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            List<Guid> listTablaTempo = mEntityContext.PersonaVinculoOrganizacion.JoinPersona().Where(item => item.PersonaVinculoOrganizacion.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.Persona.PersonaID).Distinct().ToList();

            dataWrapper.ListaIdentidad = mEntityContext.Perfil.JoinIdentidad().JoinPersona().JoinPerfilPersona().Where(item => item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && (item.Perfil.PersonaID.HasValue && !item.Perfil.OrganizacionID.HasValue) && !listTablaTempo.Contains(item.PerfilPersona.PersonaID) && item.Persona.EsBuscable.Equals(true) && (item.Persona.Nombre + " " + item.Persona.Apellidos).Contains(pNombre)).Select(item => item.Identidad).ToList();

            dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().JoinPersona().JoinPerfilPersona().Where(item => item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && (item.Perfil.PersonaID.HasValue && !item.Perfil.OrganizacionID.HasValue) && !listTablaTempo.Contains(item.PerfilPersona.PersonaID) && item.Persona.EsBuscable.Equals(true) && (item.Persona.Nombre + " " + item.Persona.Apellidos).Contains(pNombre)).Select(item => item.Perfil).ToList();

            dataWrapper.ListaPerfilPersona = mEntityContext.Perfil.JoinIdentidad().JoinPersona().JoinPerfilPersona().Where(item => item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && (item.Perfil.PersonaID.HasValue && !item.Perfil.OrganizacionID.HasValue) && !listTablaTempo.Contains(item.PerfilPersona.PersonaID) && item.Persona.EsBuscable.Equals(true) && (item.Persona.Nombre + " " + item.Persona.Apellidos).Contains(pNombre)).Select(item => item.PerfilPersona).ToList();

            return dataWrapper;
        }

        /// <summary>
        /// carga identidad, perfil y perfilpersona de las identidades personales de mygnoss que son amigos de una identidad de mygnoss, coinciden con la cadena pasada en nombre y no pertenecen a una organizacion
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organizaci�n</param>
        /// <param name="pIdentidadUsuarioMyGnoss">Identificador de la identidad de un usuario en MyGnoss</param>
        /// <param name="pNombre">Nombre</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesAmigosPuedoInvitarOrganizacion(Guid pOrganizacionID, Guid pIdentidadUsuarioMyGnoss, string pNombre)
        {
            //TODO: revisar
            List<Guid> listaPersonaID = mEntityContext.PersonaVinculoOrganizacion.JoinPersona().Where(item => item.PersonaVinculoOrganizacion.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.Persona.PersonaID).Distinct().ToList();

            List<Guid> listaTablaTemp = mEntityContext.Perfil.JoinIdentidad().Where(item => item.Perfil.PersonaID.HasValue && listaPersonaID.Contains(item.Perfil.PersonaID.Value) && !item.Perfil.OrganizacionID.HasValue && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !item.Perfil.Eliminado && !item.Identidad.FechaBaja.HasValue).Select(item => item.Identidad.IdentidadID).ToList();

            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            List<Guid> listaIdentidadID2 = mEntityContext.Perfil.JoinIdentidad().JoinAmigo().Join(mEntityContext.PerfilOrganizacion, join => join.Perfil.PerfilID, org => org.PerfilID, (join, org) =>
           new
           {
               Identidad = join.Identidad,
               Perfil = join.Perfil,
               Amigo = join.Amigo,
               PerfilOrganizacion = org
           }).Join(mEntityContext.Organizacion, join => join.PerfilOrganizacion.OrganizacionID, org => org.OrganizacionID, (join, org) =>
           new
           {
               Identidad = join.Identidad,
               Perfil = join.Perfil,
               Amigo = join.Amigo,
               PerfilOrganizacion = join.PerfilOrganizacion,
               Organizacion = org
           }).Where(item => !item.Identidad.FechaBaja.HasValue && item.Organizacion.OrganizacionID.Equals(pOrganizacionID) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && (!item.Perfil.PersonaID.HasValue && item.Perfil.OrganizacionID.HasValue) && !item.Perfil.Eliminado)
            .Select(item => item.Amigo.IdentidadID).ToList();

            List<Guid> listaIdentidadID1 = mEntityContext.PerfilPersona.JoinIdentidad().JoinAmigo().Where(item => !item.Identidad.FechaBaja.HasValue && item.Amigo.IdentidadID.Equals(pIdentidadUsuarioMyGnoss))
                .Select(item => item.Identidad.IdentidadID)
                .Union(mEntityContext.Perfil.JoinIdentidad().JoinPerfilPersona().Where(item => listaIdentidadID2.Contains(item.Identidad.IdentidadID))
                .Select(item => item.Identidad.IdentidadID)).ToList();

            List<Guid> listaIdentidadID3 = mEntityContext.Invitacion.Where(item => item.ElementoVinculadoID.Value.Equals(pOrganizacionID) && (item.TipoInvitacion.Equals(45) || item.TipoInvitacion.Equals(46)) && item.Estado == 0)
                .Select(item => item.IdentidadDestinoID).ToList();

            var listaIdentidad = mEntityContext.Perfil.JoinIdentidad().JoinPersona().Where(item => listaIdentidadID1.Contains(item.Identidad.IdentidadID) && !listaTablaTemp.Contains(item.Identidad.IdentidadID) && !listaIdentidadID3.Contains(item.Identidad.IdentidadID) && (item.Persona.Nombre + " " + item.Persona.Apellidos).Contains(pNombre))
                .Select(item => item.Identidad);

            dataWrapper.ListaIdentidad = listaIdentidad.ToList();

            dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().JoinPersona().Where(item => listaIdentidadID1.Contains(item.Identidad.IdentidadID) && !listaTablaTemp.Contains(item.Identidad.IdentidadID) && !listaIdentidadID3.Contains(item.Identidad.IdentidadID) && (item.Persona.Nombre + " " + item.Persona.Apellidos).Contains(pNombre))
                 .Select(item => item.Perfil).ToList();

            dataWrapper.ListaPerfilPersona = mEntityContext.Perfil.JoinIdentidad().JoinPersona().JoinPerfilPersona().Where(item => listaIdentidadID1.Contains(item.Identidad.IdentidadID) && !listaTablaTemp.Contains(item.Identidad.IdentidadID) && !listaIdentidadID3.Contains(item.Identidad.IdentidadID) && (item.Persona.Nombre + " " + item.Persona.Apellidos).Contains(pNombre))
                .Select(item => item.PerfilPersona).ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene la urlPerfil de una identidad
        /// </summary>
        /// <param name="pIdentidad">Clave de la identidad</param>
        /// <returns>urlPerfil de una identidad</returns>
        public KeyValuePair<string, string> ObtenerURLPerfilPorIdentidad(Guid pIdentidad)
        {
            string nombreCortoUsu = "";
            string nombreCortoOrg = "";

            var resultado = mEntityContext.Perfil.JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidad)).Select(item => new { item.Perfil.NombreCortoUsu, item.Perfil.NombreCortoOrg }).FirstOrDefault();

            if (resultado != null)
            {
                nombreCortoUsu = resultado.NombreCortoUsu;
                nombreCortoOrg = resultado.NombreCortoOrg;
            }

            return new KeyValuePair<string, string>(nombreCortoUsu, nombreCortoOrg);
        }


        /// <summary>
        /// Obtiene los IDs de identidades suscritas a un perfil
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public List<Guid> ObtenerListaIdentidadesSusucritasAPerfil(Guid pPerfilID)
        {
            var consulta1 = mEntityContext.Suscripcion.JoinIdentidad().JoinSuscripcionTesauroUsuario().JoinPersona().JoinPerfil().Where(item => !item.Suscripcion.Bloqueada && !item.Identidad.FechaExpulsion.HasValue && !item.Identidad.FechaBaja.HasValue && item.Perfil.PerfilID.Equals(pPerfilID))
            .Select(item => new { item.Identidad.IdentidadID, item.Suscripcion.FechaSuscripcion });

            var consulta2 = mEntityContext.Suscripcion.JoinIdentidad().JoinSuscripcionIdentidadProyecto().JoinIdentidadSeguidor().Where(item => !item.Suscripcion.Bloqueada && !item.Identidad.FechaExpulsion.HasValue && !item.Identidad.FechaBaja.HasValue && item.Identidad2.PerfilID.Equals(pPerfilID))
            .Select(item => new { item.Identidad.IdentidadID, item.Suscripcion.FechaSuscripcion });

            var resultado = consulta1.Union(consulta2).OrderByDescending(item => item.FechaSuscripcion).Select(item => item.IdentidadID).ToList();

            return resultado;
        }

        /// <summary>
        /// Obtiene los datos de identidades suscritas a un perfil (carga las identidades privadas para invitados)
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesSusucritasAPerfil(Guid pPerfilID)
        {
            return ObtenerIdentidadesSusucritasAPerfil(pPerfilID, true);
        }

        /// <summary>
        /// Obtiene los datos de identidades suscritas a un perfil
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesSusucritasAPerfil(Guid pPerfilID, bool pCargarIdentidadesPrivadas)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            var consulta1 = mEntityContext.Suscripcion.JoinIdentidad().JoinSuscripcionTesauroUsuario().JoinPersona().JoinPerfil().Where(item => !item.Suscripcion.Bloqueada && !item.Identidad.FechaExpulsion.HasValue && !item.Identidad.FechaBaja.HasValue && item.Perfil.PerfilID.Equals(pPerfilID));

            var consulta2 = mEntityContext.Suscripcion.JoinIdentidad().JoinSuscripcionIdentidadProyecto().JoinIdentidad2().JoinPerfil().JoinPersona().Where(item => !item.Suscripcion.Bloqueada && !item.Identidad.FechaExpulsion.HasValue && !item.Identidad.FechaBaja.HasValue && item.Identidad2.PerfilID.Equals(pPerfilID));

            if (!pCargarIdentidadesPrivadas)
            {
                consulta1 = consulta1.Where(item => item.Persona.EsBuscableExternos);
                consulta2 = consulta2.Where(item => item.Persona.EsBuscableExternos);
            }

            var queryIdentidades = consulta1.Select(item => item.Identidad).Union(consulta2.Select(item => item.Identidad));

            dataWrapper.ListaIdentidad = queryIdentidades.ToList();

            var listaPerfilesID = dataWrapper.ListaIdentidad.Select(ident => ident.PerfilID);

            dataWrapper.ListaPerfil = mEntityContext.Perfil.Where(item => listaPerfilesID.Contains(item.PerfilID)).Distinct().ToList();

            dataWrapper.ListaPerfilPersona = mEntityContext.PerfilPersona.Where(item => listaPerfilesID.Contains(item.PerfilID)).Distinct().ToList();

            dataWrapper.ListaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.Where(item => listaPerfilesID.Contains(item.PerfilID)).Distinct().ToList();

            dataWrapper.ListaPerfilOrganizacion = mEntityContext.PerfilOrganizacion.Where(item => listaPerfilesID.Contains(item.PerfilID)).Distinct().ToList();
            return dataWrapper;
        }

        /// <summary>
        /// Obtiene los IDs de identidades suscritas a un perfil en un determinado proyecto
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public List<Guid> ObtenerListaIdentidadesIDSusucritasAPerfilEnProyecto(Guid pPerfilID, Guid pProyectoID)
        {
            return mEntityContext.Suscripcion.JoinIdentidad().JoinSuscripcionIdentidadProyecto().Join(mEntityContext.Identidad, join => join.Identidad.PerfilID, identidad => identidad.PerfilID, (join, identidad) =>
            new
            {
                Suscripcion = join.Suscripcion,
                Identidad = join.Identidad,
                SuscripcionIdentidadProyecto = join.SuscripcionIdentidadProyecto,
                Identidad2 = identidad
            }).Join(mEntityContext.Identidad, join => join.SuscripcionIdentidadProyecto.IdentidadID, identidad => identidad.IdentidadID, (join, identidad) =>
            new
            {
                Suscripcion = join.Suscripcion,
                Identidad = join.Identidad,
                SuscripcionIdentidadProyecto = join.SuscripcionIdentidadProyecto,
                Identidad2 = join.Identidad2,
                Identidad3 = identidad
            }).Where(item => (!item.Identidad2.FechaExpulsion.HasValue && !item.Identidad2.FechaBaja.HasValue && item.Identidad2.ProyectoID.Equals(pProyectoID)) && ((item.Identidad3.ProyectoID.Equals(pProyectoID) || item.Identidad3.ProyectoID.Equals(ProyectoAD.MetaProyecto)) && item.Identidad3.PerfilID.Equals(pPerfilID)))
            .OrderBy(item => item.Suscripcion.FechaSuscripcion)
            .Select(item => item.Identidad.IdentidadID).ToList();
        }

        /// <summary>
        /// Obtiene los datos de identidades suscritas a un perfil en un determinado proyecto
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesSusucritasAPerfilEnProyecto(Guid pPerfilID, Guid pProyectoID)
        {
            return ObtenerIdentidadesSusucritasAPerfilEnProyecto(pPerfilID, pProyectoID, true);
        }

        /// <summary>
        /// Obtiene los datos de identidades suscritas a un perfil en un determinado proyecto
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesSusucritasAPerfilEnProyecto(Guid pPerfilID, Guid pProyectoID, bool pCargarIdentidadesPrivadas)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            if (!pCargarIdentidadesPrivadas)
            {
                dataWrapper.ListaPerfil = mEntityContext.Suscripcion.JoinIdentidad().JoinSuscripcionIdentidadProyecto().JoinIdentidad2().JoinPerfil().JoinPersona().Join(mEntityContext.Identidad, join => join.SuscripcionIdentidadProyecto.IdentidadID, identidad3 => identidad3.IdentidadID, (join, identidad3) =>
           new
           {
               Identidad = join.Identidad,
               Suscripcion = join.Suscripcion,
               SuscripcionIdentidadProyecto = join.SuscripcionIdentidadProyecto,
               Identidad2 = join.Identidad2,
               Perfil = join.Perfil,
               Persona = join.Persona,
               Identidad3 = identidad3
           }).Where(item => (!item.Identidad2.FechaExpulsion.HasValue && !item.Identidad2.FechaBaja.HasValue && item.Identidad2.ProyectoID.Equals(pProyectoID)) && ((item.Identidad3.ProyectoID.Equals(pProyectoID) || item.Identidad3.ProyectoID.Equals(ProyectoAD.MetaProyecto)) && item.Identidad3.PerfilID.Equals(pPerfilID)) && item.Persona.EsBuscableExternos.Equals(1)).Select(item => item.Perfil).Distinct().ToList();
            }
            else
            {
                var query = mEntityContext.Suscripcion.JoinIdentidad().JoinSuscripcionIdentidadProyecto().JoinIdentidad2().JoinPerfil().Join(mEntityContext.Identidad, join => join.SuscripcionIdentidadProyecto.IdentidadID, identidad3 => identidad3.IdentidadID, (join, identidad3) =>
                new
                {
                    Identidad = join.Identidad,
                    Suscripcion = join.Suscripcion,
                    SuscripcionIdentidadProyecto = join.SuscripcionIdentidadProyecto,
                    Identidad2 = join.Identidad2,
                    Perfil = join.Perfil,
                    Identidad3 = identidad3
                })
                 .Where(item => (!item.Identidad2.FechaExpulsion.HasValue && !item.Identidad2.FechaBaja.HasValue && item.Identidad2.ProyectoID.Equals(pProyectoID)) && ((item.Identidad3.ProyectoID.Equals(pProyectoID) || item.Identidad3.ProyectoID.Equals(ProyectoAD.MetaProyecto)) && item.Identidad3.PerfilID.Equals(pPerfilID))).Select(item => item.Perfil).Distinct();

                dataWrapper.ListaPerfil = query.ToList();
            }

            List<Guid> listaPerfiles = new List<Guid>();
            foreach (var filaPerfil in dataWrapper.ListaPerfil)
            {
                if (!listaPerfiles.Contains(filaPerfil.PerfilID))
                {
                    listaPerfiles.Add(filaPerfil.PerfilID);
                }
            }

            if (dataWrapper.ListaPerfil.Count > 0)
            {
                dataWrapper.ListaIdentidad = mEntityContext.Identidad.Where(item => item.ProyectoID.Equals(pProyectoID) && listaPerfiles.Contains(item.PerfilID)).Distinct().ToList();

                dataWrapper.ListaPerfilPersona = mEntityContext.PerfilPersona.Where(item => listaPerfiles.Contains(item.PerfilID)).Distinct().ToList();

                dataWrapper.ListaPerfilOrganizacion = mEntityContext.PerfilOrganizacion.Where(item => listaPerfiles.Contains(item.PerfilID)).Distinct().ToList();

                dataWrapper.ListaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.Where(item => listaPerfiles.Contains(item.PerfilID)).Distinct().ToList();
            }
            return dataWrapper;
        }

        /// <summary>
        /// Obtiene los datos de identidades a las que esta suscrito un perfil
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesSusucritasPorPerfil(Guid pPerfilID)
        {
            return ObtenerIdentidadesSusucritasPorPerfil(pPerfilID, true);
        }

        /// <summary>
        /// Obtiene los datos de identidades a las que esta suscrito un perfil
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesSusucritasPorPerfil(Guid pPerfilID, bool pCargarIdentidadesPrivadas)
        {
            List<Guid> listaIdentidades = new List<Guid>();

            var consultaListaUsuarioID = mEntityContext.Suscripcion.JoinIdentidad().JoinSuscripcionTesauroUsuario().JoinPerfil().Where(item => item.Perfil.PerfilID.Equals(pPerfilID) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !item.Suscripcion.Bloqueada).Select(item => item.SuscripcionTesauroUsuario.UsuarioID).Distinct();

            List<Guid> listaUsuarioID = consultaListaUsuarioID.ToList();

            var consulta1 = mEntityContext.Perfil.JoinIdentidad().JoinPersona().Where(item => !item.Identidad.FechaExpulsion.HasValue && !item.Identidad.FechaBaja.HasValue && item.Identidad.OrganizacionID.Equals(ProyectoAD.MetaProyecto) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && item.Identidad.Tipo.Equals(0) && item.Persona.UsuarioID.HasValue && listaUsuarioID.Contains(item.Persona.UsuarioID.Value));

            var consultaListaPerfilID = mEntityContext.Suscripcion.JoinIdentidad().JoinSuscripcionIdentidadProyecto().JoinIdentidad2().Where(item => item.Identidad.PerfilID.Equals(pPerfilID) && !item.Suscripcion.Bloqueada).Select(item => item.Identidad2.PerfilID);

            List<Guid> listaPerfilID = consultaListaPerfilID.ToList();

            var consulta2 = mEntityContext.Perfil.JoinIdentidad().JoinPersona().Where(item => !item.Identidad.FechaExpulsion.HasValue && !item.Identidad.FechaBaja.HasValue && item.Identidad.OrganizacionID.Equals(ProyectoAD.MetaProyecto) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && listaPerfilID.Contains(item.Identidad.PerfilID));

            if (!pCargarIdentidadesPrivadas)
            {
                listaIdentidades = consulta1.Where(item => item.Persona.EsBuscableExternos.Equals(1)).Select(item => item.Identidad.IdentidadID).Union(consulta2.Where(item => item.Persona.EsBuscableExternos.Equals(1)).Select(item => item.Identidad.IdentidadID)).ToList();
            }
            else
            {
                listaIdentidades = consulta1.Select(item => item.Identidad.IdentidadID).Union(consulta2.Select(item => item.Identidad.IdentidadID)).ToList();
            }

            return ObtenerIdentidadesPorID(listaIdentidades, true);
        }

        /// <summary>
        /// Obtiene los ids de identidades a las que esta suscrito un perfil
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public List<Guid> ObtenerListaIdentidadesSusucritasPorPerfil(Guid pPerfilID)
        {
            List<Guid> listaUsuarioD = mEntityContext.Suscripcion.JoinIdentidad().JoinSuscripcionTesauroUsuario().JoinPerfil().Where(item => item.Perfil.PerfilID.Equals(pPerfilID) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !item.Suscripcion.Bloqueada).Select(item => item.SuscripcionTesauroUsuario.UsuarioID).ToList();

            var consulta1 = mEntityContext.Perfil.JoinIdentidad().JoinPersona().Where(item => !item.Identidad.FechaExpulsion.HasValue && !item.Identidad.FechaBaja.HasValue && item.Identidad.OrganizacionID.Equals(ProyectoAD.MetaOrganizacion) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && item.Persona.UsuarioID.HasValue && listaUsuarioD.Contains(item.Persona.UsuarioID.Value)).Select(item => item.Identidad.IdentidadID).Distinct();

            List<Guid> listaPerfilID = mEntityContext.Suscripcion.JoinIdentidad().JoinSuscripcionIdentidadProyecto().JoinIdentidadSeguidor().Where(item => item.Identidad.PerfilID.Equals(pPerfilID) && !item.Suscripcion.Bloqueada).Select(item => item.Identidad2.PerfilID).ToList();

            var consulta2 = mEntityContext.Perfil.JoinIdentidad().JoinPersona().Where(item => !item.Identidad.FechaExpulsion.HasValue && !item.Identidad.FechaBaja.HasValue && item.Identidad.OrganizacionID.Equals(ProyectoAD.MetaOrganizacion) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && listaPerfilID.Contains(item.Identidad.PerfilID)).Select(item => item.Identidad.IdentidadID).Distinct();

            var resultado = consulta1.Union(consulta2);

            List<Guid> listaIdentidades = resultado.ToList();

            return listaIdentidades;
        }

        /// <summary>
        /// Obtiene los datos de identidades a las que esta suscrito un perfil en un determinado proyecto
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesSusucritasPorPerfilEnProyecto(Guid pPerfilID, Guid pProyectoID)
        {
            return ObtenerIdentidadesSusucritasPorPerfilEnProyecto(pPerfilID, pProyectoID, true);
        }

        /// <summary>
        /// Obtiene los datos de identidades a las que esta suscrito un perfil en un determinado proyecto
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesSusucritasPorPerfilEnProyecto(Guid pPerfilID, Guid pProyectoID, bool pCargarIdentidadesPrivadas)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            List<Guid> listaPerfilID = mEntityContext.Suscripcion.JoinIdentidad().JoinSuscripcionIdentidadProyecto().JoinIdentidad2().Where(item => item.Identidad2.PerfilID.Equals(pPerfilID) && !item.Suscripcion.Bloqueada).Select(item => item.Identidad.PerfilID).ToList();

            List<Guid> listaIdentidadID = mEntityContext.Perfil.JoinIdentidad().JoinPersona().Where(item => !item.Identidad.FechaExpulsion.HasValue && !item.Identidad.FechaBaja.HasValue && item.Identidad.ProyectoID.Equals(pProyectoID) && listaPerfilID.Contains(item.Identidad.PerfilID) && item.Persona.EsBuscableExternos.Equals(1)).Select(item => item.Identidad.IdentidadID).ToList();

            List<Guid> listaIdentidadIDPrivada = mEntityContext.Identidad.Where(identidad => !identidad.FechaExpulsion.HasValue && !identidad.FechaBaja.HasValue && identidad.ProyectoID.Equals(pProyectoID) && listaPerfilID.Contains(identidad.PerfilID)).Select(item => item.IdentidadID).ToList();

            if (!pCargarIdentidadesPrivadas)
            {
                dataWrapper.ListaIdentidad = mEntityContext.Identidad.Where(item => listaIdentidadID.Contains(item.IdentidadID)).Distinct().ToList();
            }
            else
            {
                dataWrapper.ListaIdentidad = mEntityContext.Identidad.Where(item => listaIdentidadIDPrivada.Contains(item.IdentidadID)).Distinct().ToList();
            }

            List<Guid> listaPerfiles = new List<Guid>();

            foreach (var filaIdentidad in dataWrapper.ListaIdentidad)
            {
                if (!listaPerfiles.Contains(filaIdentidad.PerfilID))
                {
                    listaPerfiles.Add(filaIdentidad.PerfilID);
                }
            }
            if (listaPerfiles.Count > 0)
            {
                foreach (Guid perfilID in listaPerfiles)
                {
                    listaPerfiles.Add(pPerfilID);
                }

                dataWrapper.ListaPerfil = mEntityContext.Perfil.Where(perfil => listaPerfiles.Contains(perfil.PerfilID)).Distinct().ToList();

                dataWrapper.ListaPerfilPersona = mEntityContext.PerfilPersona.Where(perfil => listaPerfiles.Contains(perfil.PerfilID)).Distinct().ToList();

                dataWrapper.ListaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.Where(perfil => listaPerfiles.Contains(perfil.PerfilID)).Distinct().ToList();

                dataWrapper.ListaPerfilOrganizacion = mEntityContext.PerfilOrganizacion.Where(perfil => listaPerfiles.Contains(perfil.PerfilID)).Distinct().ToList();
            }

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene los ids de identidades a las que esta suscrito un perfil en un determinado proyecto
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public List<Guid> ObtenerListaIdentidadesSusucritasPorPerfilEnProyecto(Guid pPerfilID, Guid pProyectoID)
        {
            var query = mEntityContext.Suscripcion.JoinSuscripcionIdentidadProyecto().JoinIdentidad().JoinIdentidad2().JoinIdentidad3().Where(item => item.Identidad2.PerfilID.Equals(pPerfilID) && !item.Suscripcion.Bloqueada && !item.Identidad.PerfilID.Equals(pPerfilID) && !item.Identidad3.FechaExpulsion.HasValue && !item.Identidad3.FechaBaja.HasValue && item.Identidad3.ProyectoID.Equals(pProyectoID))
                .Select(item => item.Identidad3.IdentidadID);


            return query.ToList();
        }

        public Perfil ObtenerFilaPerfilPorID(Guid pPerfilID)
        {
            return mEntityContext.Perfil.Where(perfil => perfil.PerfilID.Equals(pPerfilID)).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene las identidades a partir una lista de identificadores
        /// </summary>
        /// <param name="pListaIdentidades">Lista de identificadores de identidad</param>
        /// <param name="pObtenerSoloActivas">TRUE si s�lo obtiene las activas (no eliminadas o sin fecha de baja)</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesPorID(List<Guid> pListaIdentidades, bool pObtenerSoloActivas)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            if (!pObtenerSoloActivas)
            {
                dataWrapper.ListaIdentidad = mEntityContext.Identidad.Where(item => pListaIdentidades.Contains(item.IdentidadID)).Distinct().ToList();

                dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().Where(item => pListaIdentidades.Contains(item.Identidad.IdentidadID)).Select(item => item.Perfil).Distinct().ToList();

                dataWrapper.ListaPerfilRedesSociales = mEntityContext.PerfilRedesSociales.JoinIdentidad().Where(item => pListaIdentidades.Contains(item.Identidad.IdentidadID)).Select(item => item.PerfilRedesSociales).Distinct().ToList();

                dataWrapper.ListaPerfilPersona = mEntityContext.PerfilPersona.JoinIdentidad().Where(item => pListaIdentidades.Contains(item.Identidad.IdentidadID)).Select(item => item.PerfilPersona).Distinct().ToList();

                dataWrapper.ListaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.JoinIdentidad().Where(item => pListaIdentidades.Contains(item.Identidad.IdentidadID)).Select(item => item.PerfilPersonaOrg).Distinct().ToList();

                dataWrapper.ListaPerfilOrganizacion = mEntityContext.PerfilOrganizacion.JoinIdentidad().JoinPerfil().Where(item => pListaIdentidades.Contains(item.Identidad.IdentidadID)).Select(item => item.PerfilOrganizacion).Distinct().ToList();

                dataWrapper.ListaProfesor = mEntityContext.Profesor.JoinIdentidad().Where(item => pListaIdentidades.Contains(item.Identidad.IdentidadID)).Select(item => item.Profesor).Distinct().ToList();
            }
            else
            {
                dataWrapper.ListaIdentidad = mEntityContext.Perfil.JoinIdentidad().Where(item => pListaIdentidades.Contains(item.Identidad.IdentidadID) && !item.Identidad.FechaBaja.HasValue && !item.Perfil.Eliminado).Select(item => item.Identidad).Distinct().ToList();

                dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().Where(item => pListaIdentidades.Contains(item.Identidad.IdentidadID) && !item.Identidad.FechaBaja.HasValue && !item.Perfil.Eliminado).Select(item => item.Perfil).Distinct().ToList();

                var queryPerfilRedesSociales = mEntityContext.PerfilRedesSociales.JoinIdentidad().JoinPerfil().Where(item => pListaIdentidades.Contains(item.Identidad.IdentidadID) && !item.Identidad.FechaBaja.HasValue && !item.Perfil.Eliminado).Select(item => item.PerfilRedesSociales);

                dataWrapper.ListaPerfilRedesSociales = queryPerfilRedesSociales.ToList();

                dataWrapper.ListaPerfilPersona = mEntityContext.Perfil.JoinIdentidad().JoinPerfilPersona().Where(item => pListaIdentidades.Contains(item.Identidad.IdentidadID) && !item.Identidad.FechaBaja.HasValue && !item.Perfil.Eliminado).Select(item => item.PerfilPersona).Distinct().ToList();

                dataWrapper.ListaPerfilPersonaOrg = mEntityContext.Perfil.JoinIdentidad().JoinPerfilPersonaOrgConPerfilID().Where(item => pListaIdentidades.Contains(item.Identidad.IdentidadID) && !item.Identidad.FechaBaja.HasValue && !item.Perfil.Eliminado).Select(item => item.PerfilPersonaOrg).Distinct().ToList();

                dataWrapper.ListaPerfilOrganizacion = mEntityContext.PerfilOrganizacion.JoinIdentidad().JoinPerfil().Where(item => pListaIdentidades.Contains(item.Identidad.IdentidadID) && !item.Identidad.FechaBaja.HasValue && !item.Perfil.Eliminado).Select(item => item.PerfilOrganizacion).Distinct().ToList();

                dataWrapper.ListaProfesor = mEntityContext.Profesor.JoinIdentidad().Where(item => pListaIdentidades.Contains(item.Identidad.IdentidadID) && !item.Identidad.FechaBaja.HasValue).Select(item => item.Profesor).Distinct().ToList();
            }
            return dataWrapper;
        }

        /// <summary>
        /// Obtiene las identidades a partir una lista de identificadores
        /// </summary>
        /// <param name="pListaIdentidades">Lista de identificadores de identidad</param>
        /// <param name="pObtenerSoloActivas">TRUE si s�lo obtiene las activas (no eliminadas o sin fecha de baja)</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesPorID_Grupo(List<Guid> pListaIdentidades, bool pObtenerSoloActivas)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();
            if (pListaIdentidades.Count > 0)
            {
                if (!pObtenerSoloActivas)
                {
                    dataWrapper.ListaIdentidad = mEntityContext.Identidad.Where(item => pListaIdentidades.Contains(item.IdentidadID)).Distinct().ToList();

                    dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().Where(item => pListaIdentidades.Contains(item.Identidad.IdentidadID)).Select(item => item.Perfil).Distinct().ToList();
                }
                else
                {
                    dataWrapper.ListaIdentidad = mEntityContext.Perfil.JoinIdentidad().Where(item => pListaIdentidades.Contains(item.Identidad.IdentidadID) && !item.Identidad.FechaBaja.HasValue && !item.Perfil.Eliminado).Select(item => item.Identidad).Distinct().ToList();

                    dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().Where(item => pListaIdentidades.Contains(item.Identidad.IdentidadID) && !item.Identidad.FechaBaja.HasValue && !item.Perfil.Eliminado).Select(item => item.Perfil).Distinct().ToList();
                }
            }
            return dataWrapper;
        }

        /// <summary>
        /// Obtiene las identidades a partir de una lista de nombres de perfil
        /// </summary>
        /// <param name="pListaNombres">Lista de nombres de perfil</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesPorNombre(string[] pListaNombres)
        {
            return ObtenerIdentidadesPorNombreYProyecto(pListaNombres, Guid.Empty);
        }

        /// <summary>
        /// Obtiene las identidades a partir de una lista de nombres de perfil
        /// </summary>
        /// <param name="pListaNombres">Lista de nombres de perfil</param>
        /// <param name="pProyectoID">proyectoID</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesPorNombreYProyecto(IEnumerable<string> pListaNombres, Guid pProyectoID)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();
            List<Guid> listaPerfiles = new List<Guid>();

            if (pListaNombres.Any())
            {
                if (pProyectoID != Guid.Empty && !pProyectoID.Equals(ProyectoAD.MetaProyecto))
                {
                    dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().Where(item => pListaNombres.Contains(item.Perfil.NombrePerfil) && item.Identidad.ProyectoID.Equals(pProyectoID)).Select(item => item.Perfil).Distinct().ToList();

                    if (dataWrapper.ListaPerfil.Count < pListaNombres.Count())
                    {
                        dataWrapper.ListaPerfil.AddRange(mEntityContext.Perfil.JoinIdentidad().Where(item => item.Perfil.OrganizacionID.HasValue && pListaNombres.Contains(item.Perfil.NombrePerfil + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + item.Perfil.NombreOrganizacion) && item.Identidad.ProyectoID.Equals(pProyectoID)).Select(item => item.Perfil).Distinct().ToList());
                    }

                    listaPerfiles = dataWrapper.ListaPerfil.Select(item => item.PerfilID).ToList();

                    if (listaPerfiles.Count > 0)
                    {
                        dataWrapper.ListaIdentidad = mEntityContext.Identidad.Where(item => listaPerfiles.Contains(item.PerfilID) && item.ProyectoID.Equals(pProyectoID)).Distinct().ToList();
                    }
                    //dataWrapper.ListaProfesor = mEntityContext.Profesor.JoinIdentidad().JoinPerfil().Where(item => pListaNombres.Contains(item.Perfil.NombrePerfil) && item.Identidad.ProyectoID.Equals(pProyectoID)).Select(item => item.Profesor).Distinct().ToList();

                }
                else
                {
                    dataWrapper.ListaPerfil = mEntityContext.Perfil.Where(item => pListaNombres.Contains(item.NombrePerfil)).Distinct().ToList();

                    if (dataWrapper.ListaPerfil.Count < pListaNombres.Count())
                    {
                        dataWrapper.ListaPerfil.AddRange(mEntityContext.Perfil.Where(item => item.OrganizacionID.HasValue && pListaNombres.Contains(item.NombrePerfil + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + item.NombreOrganizacion)).Distinct().ToList());
                    }

                    listaPerfiles = dataWrapper.ListaPerfil.Select(item => item.PerfilID).ToList();

                    if (listaPerfiles.Count > 0)
                    {
                        dataWrapper.ListaIdentidad = mEntityContext.Identidad.Where(item => listaPerfiles.Contains(item.PerfilID)).ToList();
                    }
                }

                if (listaPerfiles.Count > 0)
                {
                    dataWrapper.ListaPerfilPersona = mEntityContext.PerfilPersona.Where(item => listaPerfiles.Contains(item.PerfilID)).ToList();

                    dataWrapper.ListaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.Where(item => listaPerfiles.Contains(item.PerfilID)).ToList();

                    dataWrapper.ListaPerfilOrganizacion = mEntityContext.PerfilOrganizacion.Where(item => listaPerfiles.Contains(item.PerfilID)).ToList();
                }


            }
            return dataWrapper;
        }

        /// <summary>
        /// Devuelve una lista con todas las identidades, ordenadas por visibles y no visibles
        /// </summary>
        /// <returns></returns>
        public SortedList<Guid, bool> ObtenerListaIdentidadesVisiblesExternos(List<Guid> pListaIdentidades)
        {
            SortedList<Guid, bool> listaIdentidades = new SortedList<Guid, bool>();

            var consulta1 = mEntityContext.Perfil.JoinIdentidad().JoinPersona().Where(item => pListaIdentidades.Contains(item.Identidad.IdentidadID) && !item.Perfil.OrganizacionID.HasValue).Select(item => new { item.Identidad.IdentidadID, item.Persona.EsBuscableExternos }).ToList();

            var consulta2 = mEntityContext.Perfil.JoinIdentidad().JoinOrganizacion().Where(item => pListaIdentidades.Contains(item.Identidad.IdentidadID) && ((item.Perfil.PersonaID.HasValue && item.Perfil.OrganizacionID.HasValue) || (!item.Perfil.PersonaID.HasValue && item.Perfil.OrganizacionID.HasValue))).Select(item => new { item.Identidad.IdentidadID, item.Organizacion.EsBuscableExternos }).ToList(); ;

            var resultado = consulta1.Union(consulta2);

            foreach (var fila in resultado)
            {
                listaIdentidades.Add(fila.IdentidadID, fila.EsBuscableExternos);
            }
            return listaIdentidades;
        }

        /// <summary>
        /// Devuelve una lista con todas las identidades que tienen suscripciones
        /// </summary>
        /// <returns></returns>
        public List<Guid> ObtenerListaIdentidadesIDConSuscripcion()
        {
            return mEntityContext.Suscripcion.Where(item => !item.Bloqueada).Select(item => item.IdentidadID).Distinct().ToList();
        }


        /// <summary>
        /// Devuelve una lista con todos las perfiles que tienen suscripciones
        /// </summary>
        /// <returns></returns>
        public List<Guid> ObtenerListaPerfilesIDConSuscripcionParaEnviar()
        {
            List<Guid> listaPerfiles = mEntityContext.Suscripcion.JoinIdentidad().Where(item => !item.Suscripcion.Bloqueada && !item.Suscripcion.Periodicidad.Equals(-1) && DateTime.Now.AddDays(-item.Suscripcion.Periodicidad) >= item.Suscripcion.UltimoEnvio).Select(item => item.Identidad.PerfilID).Distinct().ToList();

            return listaPerfiles;
        }

        /// <summary>
        /// Devuelve una lista con todos las perfiles que tienen suscripciones
        /// </summary>
        /// <returns></returns>
        public List<Guid> ObtenerListaPerfilesIDConSuscripcion()
        {
            return mEntityContext.Suscripcion.JoinIdentidad().Where(item => !item.Suscripcion.Bloqueada).Select(item => item.Identidad.PerfilID).Distinct().ToList();
        }

        /// <summary>
        /// Devuelve una lista con todos los perfiles que tienen suscripciones en proyectos configurados para EnviarNotificacionesDeSuscripciones
        /// </summary>
        /// <returns>Lista con los identificadores de los perfiles suscritos</returns>
        public List<Guid> ObtenerListaPerfilesIDConSuscripcionPorProyectos(short pDiaSemana)
        {
            string diaSemana = pDiaSemana.ToString();

            List<Guid> listaProyectoID = mEntityContext.ParametroProyecto.Where(item => item.Parametro.Equals("EnviarNotificacionesDeSuscripciones") && item.Valor.Equals("true")).Select(item => item.ProyectoID).ToList();

            foreach (Guid proyectoID in listaProyectoID.ToList())
            {
                DateTime fechaAConsultar = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.Now.Day);
                bool tieneRecursosRecientes = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().Where(item => item.BaseRecursosProyecto.ProyectoID.Equals(proyectoID)).Any(item => DateTime.Compare(item.Documento.FechaCreacion.Value, fechaAConsultar) > 0);
                if (!tieneRecursosRecientes)
                {
                    listaProyectoID.Remove(proyectoID);
                }
            }

            var consulta1 = mEntityContext.Suscripcion.JoinIdentidad().Where(item => !item.Suscripcion.Bloqueada && item.Suscripcion.Periodicidad.Equals(1) && listaProyectoID.Contains(item.Identidad.ProyectoID)).Select(item => item.Identidad.PerfilID).ToList();

            List<Guid> listaProyectoID2 = mEntityContext.ParametroProyecto.Where(item => item.Parametro.Equals("EnviarNotificacionesDeSuscripciones") && item.Valor.Equals(diaSemana) && mEntityContext.ParametroProyecto.Any(item2 => item.ProyectoID.Equals(item2.ProyectoID) && item2.Parametro.Equals("EnviarNotificacionesDeSuscripciones") && item2.Valor.Equals("true"))).Select(item => item.ProyectoID).ToList();

            var consulta2 = mEntityContext.Suscripcion.JoinIdentidad().Where(item => !item.Suscripcion.Bloqueada && item.Suscripcion.Periodicidad.Equals(7) && listaProyectoID2.Contains(item.Identidad.ProyectoID)).Select(item => item.Identidad.PerfilID).ToList();

            var resultado = consulta1.Union(consulta2);

            return resultado.ToList();
        }

        /// <summary>
        /// Devuelve la lista de las identidades a las que pertenecen las direcciones de correo pasadas como par�metro
        /// </summary>
        /// <param name="pListaCorreos">Lista de direcciones de correo</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Lista de identidades con su correo</returns>
        public Dictionary<Guid, string> ObtenerListaIdentidadesDeCorreosEnProyecto(List<string> pListaCorreos, Guid pProyectoID)
        {
            Dictionary<Guid, string> listaIdentidadesCorreos = new Dictionary<Guid, string>();

            var resultado = mEntityContext.PerfilPersona.JoinIdentidad().JoinPersona().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && item.Persona.Email != null && pListaCorreos.Contains(item.Persona.Email)).Select(item => new { item.Identidad.IdentidadID, item.Persona.Email }).ToList();

            foreach (var correo in resultado)
            {
                listaIdentidadesCorreos.Add(correo.IdentidadID, correo.Email);
            }

            return listaIdentidadesCorreos;
        }

        /// <summary>
        /// Devuelve un dataSet con todos los perfiles que tienen suscripciones.
        /// </summary>
        /// <returns></returns>
        public List<Perfil> ObtenerPerfilesConSuscripcion()
        {
            return mEntityContext.Perfil.JoinIdentidad().JoinSuscripcion().Where(item => !item.Suscripcion.Bloqueada).Select(item => item.Perfil).Distinct().ToList();
        }

        /// <summary>
        /// Obtiene el ID de los Perfiles Personales a partir una lista de identificadores de usuario
        /// </summary>
        /// <param name="pListaUsuariosID">Lista de identificadores de identidad</param>
        /// <returns>Dataset de identidades</returns>
        public Dictionary<Guid, Guid> ObtenerListaPerfilPersonalPorUsuarioID(List<Guid> pListaUsuariosID)
        {
            Dictionary<Guid, Guid> ListaPerfiles = new Dictionary<Guid, Guid>();

            var resultado = mEntityContext.Perfil.JoinIdentidad().JoinPersona().Where(item => item.Persona.UsuarioID.HasValue && pListaUsuariosID.Contains(item.Persona.UsuarioID.Value) && !item.Perfil.OrganizacionID.HasValue).Select(item => new { item.Perfil.PerfilID, item.Persona.UsuarioID }).ToList();

            foreach (var item in resultado)
            {
                if (!ListaPerfiles.Keys.Contains(item.PerfilID))
                {
                    ListaPerfiles.Add(item.PerfilID, item.UsuarioID.Value);
                }
            }

            return ListaPerfiles;
        }

        /// <summary>
        /// Obtiene una lista de ProyectoID, IdentidadID en los que participa el usuarioActual
        /// </summary>
        /// <param name="pActivas">True si solo se quiere las activas, False si solo se quiere las No activas, 
        /// Null si se quieren todas</param>
        /// <param name="pPerfilID">Identificador del perfil del que se quieren obtener las identidades (Puede ser NULL)</param>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>lista de ProyectoID, IdentidadID en los que participa el usuarioActual</returns>
        public Dictionary<Guid, Guid> ObtenerListaTodasMisIdentidades(bool? pActivas, Guid? pPerfilID, Guid pPersonaID)
        {
            Dictionary<Guid, Guid> listaTodasMisIdentidades = new Dictionary<Guid, Guid>();

            var query = mEntityContext.Perfil.JoinIdentidad().Where(item => item.Perfil.PersonaID.Value.Equals(pPersonaID) && item.Identidad.ProyectoID != ProyectoAD.MetaProyecto);

            if (pPerfilID.HasValue)
            {
                query = query.Where(item => item.Identidad.PerfilID.Equals(pPerfilID.Value));
            }

            if (pActivas.HasValue)
            {
                if (pActivas.Value)
                {
                    query = query.Where(item => !item.Identidad.FechaBaja.HasValue);
                }
                else
                {
                    query = query.Where(item => item.Identidad.FechaBaja.HasValue);
                }
            }

            var resultado = query.Select(item => new { item.Identidad.ProyectoID, item.Identidad.IdentidadID }).ToList();

            foreach (var item in resultado)
            {
                listaTodasMisIdentidades.Add(item.ProyectoID, item.IdentidadID);
            }

            return listaTodasMisIdentidades;
        }

        /// <summary>
        /// Obtiene una lista de ProyectoID, IdentidadID en los que participa el usuarioActual
        /// </summary>
        /// <param name="pActivas">True si solo se quiere las activas, False si solo se quiere las No activas, 
        /// Null si se quieren todas</param>
        /// <param name="pPerfilID">Identificador del perfil del que se quieren obtener las identidades</param>
        /// <returns>lista de ProyectoID, IdentidadID en los que participa el usuarioActual</returns>
        public Dictionary<Guid, Guid> ObtenerListaTodasMisIdentidadesDePerfil(bool? pActivas, Guid pPerfilID)
        {
            Dictionary<Guid, Guid> listaTodasMisIdentidades = new Dictionary<Guid, Guid>();

            var query = mEntityContext.Perfil.JoinIdentidad();

            if (pActivas.HasValue)
            {
                if (pActivas.Value)
                {
                    query = query.Where(item => item.Identidad.PerfilID.Equals(pPerfilID) && item.Identidad.ProyectoID != ProyectoAD.MetaProyecto && !item.Identidad.FechaBaja.HasValue);
                }
                else
                {
                    query = query.Where(item => item.Identidad.PerfilID.Equals(pPerfilID) && item.Identidad.ProyectoID != ProyectoAD.MetaProyecto && item.Identidad.FechaBaja.HasValue);
                }
            }

            var resultado = query.Select(item => new { item.Identidad.ProyectoID, item.Identidad.IdentidadID }).ToList();

            foreach (var item in resultado)
            {
                listaTodasMisIdentidades.Add(item.ProyectoID, item.IdentidadID);
            }

            return listaTodasMisIdentidades;
        }

        /// <summary>
        /// Obtiene una lista con los perfiles de una persona
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <param name="pNombreOrgPerfilID">Lista con el nombre de la org, su ID de perfil y el ID de la org</param>
        /// <returns>Lista con los perfiles de una persona</returns>
        public Dictionary<string, string> ObtenerListaPerfilesPersona(Guid pPersonaID, Dictionary<string, KeyValuePair<Guid, Guid>> pNombreOrgPerfilID)
        {
            Dictionary<string, string> listaPerfiles = new Dictionary<string, string>();

            var resultado = mEntityContext.Perfil.JoinIdentidad().Where(item => item.Perfil.PersonaID.Value.Equals(pPersonaID) && !item.Perfil.Eliminado && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue && item.Perfil.NombreOrganizacion != null).Select(item => new { item.Perfil.NombreOrganizacion, item.Perfil.NombreCortoOrg, item.Perfil.PerfilID, item.Perfil.OrganizacionID });

            foreach (var item in resultado)
            {
                if (!listaPerfiles.ContainsKey(item.NombreOrganizacion))
                {
                    listaPerfiles.Add(item.NombreOrganizacion, item.NombreCortoOrg);

                    if (pNombreOrgPerfilID != null)
                    {
                        pNombreOrgPerfilID.Add(item.NombreOrganizacion, new KeyValuePair<Guid, Guid>(item.PerfilID, item.OrganizacionID.Value));
                    }
                }
            }
            return listaPerfiles;
        }

        /// <summary>
        /// Obtiene las identidad de una persona en un proyecto "Identidad", "Perfil", "PerfilPersona", "PerfilPersonaOrg"
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>IdentidadDS</returns>
        public DataWrapperIdentidad ObtenerIdentidadDePersonaEnProyecto(Guid pProyectoID, Guid pPersonaID)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            dataWrapper.ListaIdentidad = mEntityContext.Perfil.JoinIdentidad().JoinProyectoUsuarioIdentidad().Where(item => item.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID) && item.Perfil.PersonaID.Value.Equals(pPersonaID)).Select(item => item.Identidad).Distinct().ToList();

            dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().JoinProyectoUsuarioIdentidad().Where(item => item.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID) && item.Perfil.PersonaID.Value.Equals(pPersonaID)).Select(item => item.Perfil).Distinct().ToList();

            dataWrapper.ListaPerfilRedesSociales = mEntityContext.Perfil.JoinIdentidad().JoinProyectoUsuarioIdentidad().JoinPerfilRedesSociales().Where(item => item.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID) && item.Perfil.PersonaID.Value.Equals(pPersonaID)).Select(item => item.PerfilRedesSociales).Distinct().ToList();

            dataWrapper.ListaPerfilPersona = mEntityContext.PerfilPersona.JoinIdentidad().JoinProyectoUsuarioIdentidad().Where(item => item.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID) && item.PerfilPersona.PersonaID.Equals(pPersonaID)).Select(item => item.PerfilPersona).Distinct().ToList();

            dataWrapper.ListaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.JoinIdentidad().JoinProyectoUsuarioIdentidad().Where(item => item.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID) && item.PerfilPersonaOrg.PersonaID.Equals(pPersonaID)).Select(item => item.PerfilPersonaOrg).Distinct().ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene las identidad de una persona en un proyecto "Identidad", "Perfil", "PerfilPersona", "PerfilPersonaOrg"
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <returns>IdentidadDS</returns>
        public DataWrapperIdentidad ObtenerIdentidadDePersonaEnOrganizacion(Guid pPersonaID, Guid pOrganizacionID)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            dataWrapper.ListaIdentidad = mEntityContext.Perfil.JoinIdentidad().Where(item => item.Perfil.PersonaID.Value.Equals(pPersonaID) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID)).Select(item => item.Identidad).Distinct().ToList();

            dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().Where(item => item.Perfil.PersonaID.Value.Equals(pPersonaID) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID)).Select(item => item.Perfil).Distinct().ToList();

            dataWrapper.ListaPerfilPersona = mEntityContext.PerfilPersona.JoinPerfil().Where(item => item.PerfilPersona.PersonaID.Equals(pPersonaID) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID)).Select(item => item.PerfilPersona).Distinct().ToList();

            dataWrapper.ListaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.JoinPerfil().Where(item => item.PerfilPersonaOrg.PersonaID.Equals(pPersonaID) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID)).Select(item => item.PerfilPersonaOrg).Distinct().ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene las identidad de unos usuarios en un proyecto
        /// </summary>
        /// <param name="pListaUsuariosID">Lista de identificadores de usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>IdentidadDS</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDeUsuariosEnProyectoYOrg(List<Guid> pListaUsuariosID, Guid pProyectoID, Guid pOrganizacionID)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            dataWrapper.ListaIdentidad = mEntityContext.Perfil.JoinIdentidad().JoinProyectoUsuarioIdentidad().Where(item => item.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && pListaUsuariosID.Contains(item.ProyectoUsuarioIdentidad.UsuarioID)).Select(item => item.Identidad).Distinct().ToList();

            dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().JoinProyectoUsuarioIdentidad().Where(item => item.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && pListaUsuariosID.Contains(item.ProyectoUsuarioIdentidad.UsuarioID)).Select(item => item.Perfil).Distinct().ToList();

            dataWrapper.ListaPerfilPersona = mEntityContext.PerfilPersona.JoinIdentidad().JoinProyectoUsuarioIdentidad().JoinPerfil().Where(item => item.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && pListaUsuariosID.Contains(item.ProyectoUsuarioIdentidad.UsuarioID)).Select(item => item.PerfilPersona).Distinct().ToList();

            dataWrapper.ListaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.JoinIdentidad().JoinProyectoUsuarioIdentidad().Where(item => item.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID) && item.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizacionID) && pListaUsuariosID.Contains(item.ProyectoUsuarioIdentidad.UsuarioID)).Select(item => item.PerfilPersonaOrg).Distinct().ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene "Identidad", "Perfil", "PerfilPersonaOrg" y "PerfilPersona" de los usuarios de un determinado proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pTraerNombreApellidos">Indica si se deben agregar a Identidad el nombre y apellidos de cada persona</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDePersonasNoCorporativasDeProyecto(Guid pProyectoID, bool pTraerNombreApellidos)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            if (pTraerNombreApellidos)
            {
                dataWrapper.ListaIdentidad = mEntityContext.Perfil.JoinIdentidad().JoinProyectoUsuarioIdentidad().JoinPersona().Where(item => item.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID) && (item.Identidad.Tipo < 2 || item.Identidad.Tipo == 4) && !item.Identidad.FechaBaja.HasValue).Select(item => item.Identidad).ToList();
            }
            else
            {
                dataWrapper.ListaIdentidad = mEntityContext.ProyectoUsuarioIdentidad.JoinIdentidad().Where(item => item.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID) && (item.Identidad.Tipo < 2 || item.Identidad.Tipo == 4) && !item.Identidad.FechaBaja.HasValue).Select(item => item.Identidad).ToList();
            }

            dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().JoinProyectoUsuarioIdentidad().Where(item => item.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID) && (item.Identidad.Tipo < 2 || item.Identidad.Tipo == 4) && !item.Identidad.FechaBaja.HasValue).Select(item => item.Perfil).ToList();

            dataWrapper.ListaPerfilPersona = mEntityContext.PerfilPersona.JoinIdentidad().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue).Select(item => item.PerfilPersona).Distinct().ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene una lista con las identidades que son visibles y las que no
        /// </summary>
        /// <param name="pListaIdentidades">Lista de identidades</param>
        /// <returns></returns>
        public Dictionary<Guid, bool> ObtenerSiListaIdentidadesSonVisibles(IList<Guid> pListaIdentidades, bool pEsIdentidadInvitada)
        {
            //TODO Probar => No se puede acceder a Usuario.UsuarioActual.EsIdentidadInvitada

            Dictionary<Guid, bool> listaIdentidades = new Dictionary<Guid, bool>();

            if (pListaIdentidades.Count > 0)
            {
                var primeraParteConsulta = mEntityContext.Identidad.JoinPerfil().JoinPersona().Where(item => !item.Perfil.OrganizacionID.HasValue && pListaIdentidades.Contains(item.Identidad.IdentidadID)).Select(item => new IdentidadesVisibles
                {
                    IdentidadID = item.Identidad.IdentidadID,
                    EsBuscable = item.Persona.EsBuscable,
                    EsBuscableExternos = item.Persona.EsBuscableExternos
                });

                var segundParteConsulta = mEntityContext.Identidad.JoinPerfil().JoinOrganizacion().Where(item => item.Identidad.Tipo.Equals((short)TiposIdentidad.Organizacion) && pListaIdentidades.Contains(item.Identidad.IdentidadID)).Select(item => new IdentidadesVisibles
                {
                    IdentidadID = item.Identidad.IdentidadID,
                    EsBuscable = item.Organizacion.EsBuscable,
                    EsBuscableExternos = item.Organizacion.EsBuscableExternos
                });

                var terceraParteConsulta = mEntityContext.Identidad.JoinPerfil().JoinOrganizacion().Where(item => item.Identidad.Tipo.Equals((short)TiposIdentidad.ProfesionalPersonal) && pListaIdentidades.Contains(item.Identidad.IdentidadID)).Select(item => new IdentidadesVisibles
                {
                    IdentidadID = item.Identidad.IdentidadID,
                    EsBuscable = mEntityContext.PersonaVisibleEnOrg.Any(item2 => item2.PersonaID.Equals(item.Perfil.PersonaID) && item2.OrganizacionID.Equals(item.Perfil.OrganizacionID)),
                    EsBuscableExternos = item.Organizacion.EsBuscableExternos
                });

                List<IdentidadesVisibles> listaIdentidadesVisibles = primeraParteConsulta.Concat(segundParteConsulta).Concat(terceraParteConsulta).ToList();

                foreach (Guid identID in pListaIdentidades)
                {
                    IdentidadesVisibles identidad = listaIdentidadesVisibles.FirstOrDefault(ident => ident.IdentidadID.Equals(identID));
                    if (identidad != null)
                    {
                        if (pEsIdentidadInvitada)
                        {
                            listaIdentidades.Add(identID, identidad.EsBuscableExternos);
                        }
                        else
                        {
                            listaIdentidades.Add(identID, identidad.EsBuscable);
                        }
                    }
                    else
                    {
                        listaIdentidades.Add(identID, false);
                    }
                }
            }
            return listaIdentidades;
        }

        /// <summary>
        /// Indica si la identidad ya participa en el grupo
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la Identidad</param>
        /// <returns></returns>
        public bool ParticipaIdentidadEnGrupo(Guid pIdentidadID, List<Guid> pListaGrupos)
        {
            return mEntityContext.GrupoIdentidadesParticipacion.Any(item => pListaGrupos.Contains(item.GrupoID) && item.IdentidadID.Equals(pIdentidadID));
        }

        /// <summary>
        /// Indica si la identidad my gnoss de esta persona participa en alguno de los grupos
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la Identidad</param>
        /// <returns></returns>
        public bool ParticipaIdentidadMyGnossParticipaEnGrupo(Guid pIdentidadID, List<Guid> pListaGrupos)
        {
            Guid perfilID = mEntityContext.Identidad.Where(item => item.IdentidadID.Equals(pIdentidadID)).Select(item => item.PerfilID).FirstOrDefault();
            Guid identidadMyGnoss = mEntityContext.Identidad.Where(item => item.PerfilID.Equals(perfilID) && item.ProyectoID.Equals(ProyectoAD.MyGnoss)).Select(item => item.IdentidadID).FirstOrDefault();

            return mEntityContext.GrupoIdentidadesParticipacion.Any(item => pListaGrupos.Contains(item.GrupoID) && item.IdentidadID.Equals(identidadMyGnoss));
        }

        /// <summary>
        /// Indica si la identidad ya participa en el grupo
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la Identidad</param>
        /// <param name="pGrupoID">Identificador del Grupo</param>
        /// <returns></returns>
        public bool ParticipaIdentidadEnGrupo(Guid pIdentidadID, Guid pGrupoID)
        {
            return mEntityContext.GrupoIdentidadesParticipacion.Any(item => item.GrupoID.Equals(pGrupoID) && item.IdentidadID.Equals(pIdentidadID));
        }

        /// <summary>
        /// Indica si el perfil ya participa en el grupo
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <param name="pGrupoID">Identificador del Grupo</param>
        /// <returns></returns>
        public bool ParticipaPerfilEnGrupo(Guid pPerfilID, List<Guid> pListaGruposID)
        {
            return mEntityContext.GrupoIdentidadesParticipacion.JoinIdentidad().Any(item => item.Identidad.PerfilID.Equals(pPerfilID) && pListaGruposID.Contains(item.GrupoIdentidadesParticipacion.GrupoID));
        }

        /// <summary>
        /// Obtiene una lista con los nombres de las identidades que le pasamos por parametro
        /// </summary>
        /// <param name="pListaIdentidades">Lista de identidades</param>
        /// <returns></returns>
        public Dictionary<Guid, string> ObtenerNombresDeIdentidades(IList<Guid> pListaIdentidades)
        {
            var resultado = mEntityContext.Perfil.JoinIdentidad().Where(item => pListaIdentidades.Contains(item.Identidad.IdentidadID)).Select(item => new { item.Identidad.IdentidadID, item.Perfil.NombrePerfil });

            Dictionary<Guid, string> listaIdentidades = new Dictionary<Guid, string>();

            foreach (var filas in resultado)
            {
                if (!listaIdentidades.ContainsKey(filas.IdentidadID))
                {
                    listaIdentidades.Add(filas.IdentidadID, filas.NombrePerfil);
                }

            }

            return listaIdentidades;
        }
        /// <summary>
        /// Obtiene una lista con los nombres de los perfiles que le pasamos por parametro
        /// </summary>
        /// <param name="pListaPerfiles">Lista de perfiles</param>
        /// <returns></returns>
        public Dictionary<Guid, string> ObtenerNombresDePerfiles(IList<Guid> pListaPerfiles)
        {
            var resultado = mEntityContext.Perfil.Where(perfil => pListaPerfiles.Contains(perfil.PerfilID)).Select(item => new { item.PerfilID, item.NombrePerfil });

            Dictionary<Guid, string> listaPerfiles = new Dictionary<Guid, string>();

            foreach (var item in resultado)
            {
                if (!listaPerfiles.ContainsKey(item.PerfilID))
                {
                    listaPerfiles.Add(item.PerfilID, item.NombrePerfil);
                }
            }

            return listaPerfiles;
        }

        /// <summary>
        /// Obtiene una lista con los nombres de los perfiles que le pasamos por parametro
        /// </summary>
        /// <param name="pListaPerfiles">Lista de perfiles</param>
        /// <returns></returns>
        public Dictionary<Guid, string> ObtenerNombresDePerfilesUOrganizacion(IList<Guid> pListaPerfiles)
        {
            var resultado = mEntityContext.Perfil.Where(perfil => pListaPerfiles.Contains(perfil.PerfilID)).Select(item => new { item.PerfilID, item.NombrePerfil, item.NombreOrganizacion });

            Dictionary<Guid, string> listaPerfiles = new Dictionary<Guid, string>();

            foreach (var item in resultado)
            {
                if (!listaPerfiles.ContainsKey(item.PerfilID))
                {
                    if (item.NombreOrganizacion != null)
                    {
                        listaPerfiles.Add(item.PerfilID, item.NombreOrganizacion);
                    }
                    else
                    {
                        listaPerfiles.Add(item.PerfilID, item.NombrePerfil);
                    }
                }
            }

            return listaPerfiles;
        }

        /// <summary>
        /// Obtiene una lista con los nombres de los grupos que le pasamos por parametro
        /// </summary>
        /// <param name="pListaIdentidades">Lista de grupos</param>
        /// <returns></returns>
        public Dictionary<Guid, string> ObtenerNombresDeGrupos(IList<Guid> pListaGrupos)
        {
            Dictionary<Guid, string> listaGrupos = new Dictionary<Guid, string>();

            var resultado = mEntityContext.GrupoIdentidades.GroupJoin(mEntityContext.GrupoIdentidadesOrganizacion, grupoIdent => grupoIdent.GrupoID, grupoIdentOrg => grupoIdentOrg.GrupoID, (grupoIdent, grupoIdentOrg) =>
            new
            {
                GrupoIdentidades = grupoIdent,
                GrupoIdentidadesOrganizacion = grupoIdentOrg
            }).SelectMany(grupoIdentOrg => grupoIdentOrg.GrupoIdentidadesOrganizacion.DefaultIfEmpty(), (item, grupoIdentOrg) =>
            new
            {
                GrupoIdentidades = item.GrupoIdentidades,
                GrupoIdentidadesOrganizacion = grupoIdentOrg
            }).GroupJoin(mEntityContext.Organizacion, join => join.GrupoIdentidadesOrganizacion.OrganizacionID, organizacion => organizacion.OrganizacionID, (join, organizacion) =>
            new
            {
                GrupoIdentidades = join.GrupoIdentidades,
                GrupoIdentidadesOrganizacion = join.GrupoIdentidadesOrganizacion,
                Organizacion = organizacion
            }).SelectMany(org => org.Organizacion.DefaultIfEmpty(), (join, org) =>
            new
            {
                GrupoIdentidades = join.GrupoIdentidades,
                GrupoIdentidadesOrganizacion = join.GrupoIdentidadesOrganizacion,
                Organizacion = org
            }).Where(item => pListaGrupos.Contains(item.GrupoIdentidades.GrupoID))
            .Select(item => new { item.GrupoIdentidades.GrupoID, NombreGrupoIdent = item.GrupoIdentidades.Nombre, NombreOrg = item.Organizacion.Nombre }).ToList();

            foreach (var item in resultado)
            {
                if (!listaGrupos.ContainsKey(item.GrupoID))
                {
                    string nombreGrupo = item.NombreGrupoIdent;
                    if (!string.IsNullOrEmpty(item.NombreOrg))
                    {
                        nombreGrupo += " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + item.NombreOrg;
                    }
                    listaGrupos.Add(item.GrupoID, nombreGrupo);
                }
            }
            return listaGrupos;
        }

        private class IdentidadIDFotoTipoFechaBaja
        {
            public Guid IdentidadID { get; set; }
            public string Foto { get; set; }
            public short Tipo { get; set; }
            public DateTime? FechaBaja { get; set; }
        }

        /// <summary>
        /// Obtiene una lista con las identidades que tienen foto y las que no
        /// </summary>
        /// <param name="pListaIdentidades">Lista de identidades</param>
        /// <returns></returns>
        public Dictionary<Guid, string> ObtenerFotosIdentidades(List<Guid> pListaIdentidades)
        {
            Dictionary<Guid, string> listaIdent = new Dictionary<Guid, string>();

            List<IdentidadIDFotoTipoFechaBaja> resultadoConsulta = new List<IdentidadIDFotoTipoFechaBaja>();

            if (pListaIdentidades.Count > 0)
            {
                resultadoConsulta = mEntityContext.Identidad.Where(item => pListaIdentidades.Contains(item.IdentidadID)).Select(item => new IdentidadIDFotoTipoFechaBaja { IdentidadID = item.IdentidadID, Foto = item.Foto, Tipo = item.Tipo, FechaBaja = item.FechaBaja }).ToList();


                foreach (Guid identID in pListaIdentidades)
                {
                    IdentidadIDFotoTipoFechaBaja filasIdent = resultadoConsulta.Find(identidad => identidad.IdentidadID.Equals(identID));

                    if (filasIdent != null)
                    {
                        if (filasIdent.Foto == null || string.IsNullOrEmpty(filasIdent.Foto)) //Identidad desactualizada, la actualizamos al momento. Por aqu� nunca entrar�a...
                        {
                            string foto = ObtenerSiIdentidadFotoNullTieneFoto(identID, (TiposIdentidad)filasIdent.Tipo);
                            ActulizarFotoIdentidad(identID, foto);
                            listaIdent.Add(identID, foto);
                        }
                        else if (filasIdent.Foto == PersonaAD.SIN_IMAGENES_PERSONA || filasIdent.FechaBaja.HasValue)
                        {
                            listaIdent.Add(identID, null);
                        }
                        else
                        {
                            listaIdent.Add(identID, filasIdent.Foto);
                        }
                    }
                    else
                    {
                        listaIdent.Add(identID, "sinfoto");
                    }
                }
            }
            return listaIdent;
        }

        /// <summary>
        /// Comprueba si la identidad tiene imagen pregenerada o no
        /// </summary>
        /// <param name="pIdentidadID">Identidad id</param>
        /// <param name="pTipo">Tipo de identidad</param>
        /// <returns></returns>
        private string ObtenerSiIdentidadFotoNullTieneFoto(Guid pIdentidadID, TiposIdentidad pTipo)
        {
            string urlFoto = PersonaAD.SIN_IMAGENES_PERSONA;
            List<IdentidadFotoNull> listaIdentidadFoto = null;

            if (pTipo == TiposIdentidad.Personal || pTipo == TiposIdentidad.Profesor)
            {
                listaIdentidadFoto = mEntityContext.Identidad.JoinPerfil().JoinPersona().Where(item => item.Persona.CoordenadasFoto != null && !item.Perfil.OrganizacionID.HasValue && item.Identidad.IdentidadID.Equals(pIdentidadID)).AsEnumerable().Select(item => new IdentidadFotoNull
                {
                    IdentidadID = item.Identidad.IdentidadID,
                    Url = $"/{UtilArchivos.ContentImagenesPersonas}/{item.Perfil.PersonaID.Value.ToString()}{SUFIJO_PEQUE}?{item.Persona.VersionFoto.Value}"
                }).ToList();
            }
            else if (pTipo == TiposIdentidad.Organizacion || pTipo == TiposIdentidad.ProfesionalCorporativo)
            {
                listaIdentidadFoto = mEntityContext.Identidad.JoinPerfil().JoinOrganizacion().Where(item => item.Identidad.Tipo > 1 && item.Identidad.Tipo < 4 && item.Organizacion.CoordenadasLogo != null && item.Identidad.IdentidadID.Equals(pIdentidadID)).AsEnumerable().Select(item => new IdentidadFotoNull
                {
                    IdentidadID = item.Identidad.IdentidadID,
                    Url = $"/{UtilArchivos.ContentImagenesOrganizaciones}/{item.Organizacion.OrganizacionID.ToString()}{SUFIJO_PEQUE}?{item.Organizacion.VersionLogo.Value}"
                }).ToList();
            }
            else if (pTipo == TiposIdentidad.ProfesionalPersonal)
            {
                var primeraParteConsulta = mEntityContext.Identidad.JoinPerfil().JoinPersona().JoinPersonaVinculoOrganizacion().Where(item => item.Persona.CoordenadasFoto != null && item.Identidad.Tipo.Equals(1) && item.PersonaVinculoOrganizacion.UsarFotoPersonal.Equals(true) && item.Identidad.IdentidadID.Equals(pIdentidadID)).AsEnumerable().Select(item => new IdentidadFotoNull
                {
                    IdentidadID = item.Identidad.IdentidadID,
                    Url = $"/{UtilArchivos.ContentImagenesPersonas}/{item.Perfil.PersonaID.Value.ToString()}{SUFIJO_PEQUE}?{item.Persona.VersionFoto.Value}"
                });

                var segundaParteConsutla = mEntityContext.Identidad.JoinPerfil().JoinPersona().JoinPersonaVinculoOrganizacion().Where(item => item.Identidad.Tipo.Equals(1) && item.PersonaVinculoOrganizacion.UsarFotoPersonal.Equals(false) && item.PersonaVinculoOrganizacion.CoordenadasFoto != null && item.Identidad.IdentidadID.Equals(pIdentidadID)).AsEnumerable().Select(item => new IdentidadFotoNull
                {
                    IdentidadID = item.Identidad.IdentidadID,
                    Url = $"/{UtilArchivos.ContentImagenesPersona_Organizacion}/{item.Perfil.OrganizacionID.Value.ToString()}/{item.Perfil.PersonaID.Value}{SUFIJO_PEQUE}?{item.PersonaVinculoOrganizacion.VersionFoto.Value}"
                });

                listaIdentidadFoto = primeraParteConsulta.Concat(segundaParteConsutla).ToList();
            }

            //Si da error al hacer la consulta, devolvera la foto de anonimo
            if (listaIdentidadFoto != null)
            {
                if (listaIdentidadFoto.Select(item => item.IdentidadID.Equals(pIdentidadID)).Any())
                {
                    urlFoto = listaIdentidadFoto.Where(item => item.IdentidadID.Equals(pIdentidadID)).Select(item => item.Url).FirstOrDefault();
                }
            }

            if (urlFoto != null)
            {
                urlFoto = urlFoto.ToLower();
            }

            return urlFoto;
        }

        /// <summary>
        /// Actualiza la foto de una identidad.
        /// </summary>
        /// <param name="pIdentidad">Identidad</param>
        /// <param name="pFoto">Foto</param>
        public void ActulizarFotoIdentidad(Guid pIdentidad, string pFoto)
        {
            AD.EntityModel.Models.IdentidadDS.Identidad identidad = mEntityContext.Identidad.Where(item => item.IdentidadID.Equals(pIdentidad)).FirstOrDefault();

            if (pFoto != null && identidad != null)
            {
                identidad.Foto = pFoto;
            }

            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Actualiza la foto desnormalizada en Identidad de varias personas.
        /// </summary>
        /// <param name="pListaPersonasID">ID de personas</param>
        /// <param name="pBorrar">TRUE si las personas han borrado la foto, FALSE si la ha actualizado</param>
        public void ActualizarFotoIdentidadesPersonas(List<Guid> pListaPersonasID, bool pBorrar)
        {
            if (pListaPersonasID.Count > 0)
            {
                if (!pBorrar)
                {
                    var comando1 = mEntityContext.Identidad.JoinPerfil().JoinPersona().Where(item => item.Persona.VersionFoto.HasValue && item.Persona.CoordenadasFoto != null && !item.Perfil.OrganizacionID.HasValue && pListaPersonasID.Contains(item.Persona.PersonaID)).ToList();
                    foreach (var resultado in comando1)
                    {
                        resultado.Identidad.Foto = $"/{UtilArchivos.ContentImagenesPersonas}/{resultado.Perfil.PersonaID.ToString()}{SUFIJO_PEQUE}?{resultado.Persona.VersionFoto}"/*.ToLower()*/;
                    }

                    var comando2 = mEntityContext.Identidad.JoinPerfil().JoinPersona().JoinPersonaVinculoOrganizacion().Where(item => item.Persona.VersionFoto.HasValue && item.Persona.CoordenadasFoto != null && item.Identidad.Tipo.Equals(1) && item.PersonaVinculoOrganizacion.UsarFotoPersonal.Equals(true) && pListaPersonasID.Contains(item.Persona.PersonaID)).ToList();
                    foreach (var resultado in comando2)
                    {
                        resultado.Identidad.Foto = $"/{UtilArchivos.ContentImagenesPersonas}/{resultado.Perfil.PersonaID.ToString()}{SUFIJO_PEQUE}?{resultado.Persona.VersionFoto}"/*.ToLower()*/;
                    }
                }
                else
                {
                    var comando1 = mEntityContext.Identidad.JoinPerfil().JoinPersona().Where(item => !item.Perfil.OrganizacionID.HasValue && item.Persona.CoordenadasFoto == null && pListaPersonasID.Contains(item.Persona.PersonaID)).ToList();
                    foreach (var resultado in comando1)
                    {
                        resultado.Identidad.Foto = PersonaAD.SIN_IMAGENES_PERSONA;
                    }
                    var comando2 = mEntityContext.Identidad.JoinPerfil().JoinPersona().JoinPersonaVinculoOrganizacion().Where(item => item.Identidad.Tipo.Equals(1) && item.PersonaVinculoOrganizacion.UsarFotoPersonal.Equals(true) && item.Persona.CoordenadasFoto == null && pListaPersonasID.Contains(item.Persona.PersonaID)).ToList();
                    foreach (var resultado in comando2)
                    {
                        resultado.Identidad.Foto = PersonaAD.SIN_IMAGENES_PERSONA;
                    }

                }
                mEntityContext.SaveChanges();
            }
        }

        /// <summary>
        /// Actualiza las fotos desnormalizadas en Identidades de varias organizaciones.
        /// </summary>
        /// <param name="pOrganizacionID">ID de organizaci�n</param>
        /// <param name="pBorrar">TRUE si las organizaciones han borrado la foto, FALSE si la han actualizado</param>
        public void ActualizarFotoIdentidadesOrganizaciones(List<Guid> pListaOrganizacionesID, bool pBorrar)
        {
            if (pListaOrganizacionesID.Count > 0)
            {
                if (!pBorrar)
                {
                    var resultado = mEntityContext.Perfil.JoinIdentidad().JoinOrganizacion().Where(item => item.Identidad.Tipo > 1 && item.Identidad.Tipo < 4 && item.Organizacion.VersionLogo.HasValue && item.Organizacion.CoordenadasLogo != null && pListaOrganizacionesID.Contains(item.Organizacion.OrganizacionID))
                        .Select(item => new { item.Identidad, item.Organizacion.OrganizacionID, item.Organizacion.VersionLogo }).FirstOrDefault();

                    EntityModel.Models.IdentidadDS.Identidad identidad = resultado.Identidad;

                    if (resultado != null)
                    {
                        identidad.Foto = $"/{UtilArchivos.ContentImagenesOrganizaciones.ToLower()}/{resultado.OrganizacionID.ToString().ToLower()}{SUFIJO_PEQUE}?{resultado.VersionLogo.ToString().ToLower()}";
                    }
                }
                else
                {
                    var resultado = mEntityContext.Perfil.JoinIdentidad().JoinOrganizacion().Where(item => item.Identidad.Tipo > 1 && item.Identidad.Tipo < 4 && item.Organizacion.CoordenadasLogo == null && pListaOrganizacionesID.Contains(item.Organizacion.OrganizacionID))
                        .Select(item => item.Identidad).FirstOrDefault();

                    if (resultado != null)
                    {
                        resultado.Foto = $"{PersonaAD.SIN_IMAGENES_PERSONA.ToString()}";
                    }
                }
            }
        }

        /// <summary>
        /// Actualiza las foto desnormalizadas en Identidad de una organizaci�n.
        /// </summary>
        /// <param name="pListaOrgPersona">Lista de pares con Organizacion-Persona</param>
        /// <param name="pBorrar">TRUE si la organizaci�n ha borrado la foto, FALSE si la ha actualizado</param>
        public void ActualizarFotoIdentidadesDePersonasDeOrganizaciones(List<KeyValuePair<Guid, Guid>> pListaOrgPersona, bool pBorrar, bool pUsarFotoPersonal)
        {
            if (pListaOrgPersona.Count > 0)
            {
                List<Guid> listaOrganizacionID = pListaOrgPersona.Select(lista => lista.Key).ToList();
                List<Guid> listaPersonaID = pListaOrgPersona.Select(lista => lista.Value).ToList();
                if (!pBorrar)
                {

                    if (pUsarFotoPersonal)
                    {
                        var resultado = mEntityContext.Perfil.JoinIdentidad().JoinPersona().JoinPersonaVinculoOrganizacion().Where(
                            item => item.Identidad.Tipo.Equals(1) && item.PersonaVinculoOrganizacion.UsarFotoPersonal.Equals(1) && item.Perfil.PersonaID.HasValue && listaPersonaID.Contains(item.Perfil.PersonaID.Value)).ToList();

                        foreach (var fila in resultado)
                        {
                            EntityModel.Models.IdentidadDS.Identidad identidad = fila.Identidad;
                            identidad.Foto = $"/{UtilArchivos.ContentImagenesPersonas.ToLower()}/{fila.Perfil.PersonaID.ToString().ToLower()}{SUFIJO_PEQUE}?{fila.Persona.VersionFoto.ToString().ToLower()}";
                        }
                    }
                    else
                    {
                        var resultado = mEntityContext.Perfil.JoinIdentidad().JoinPersonaVinculoOrganizacion().Where(
                            item => item.Identidad.Tipo.Equals(1) && (item.Perfil.PersonaID.HasValue && listaPersonaID.Contains(item.Perfil.PersonaID.Value) && item.Perfil.OrganizacionID.HasValue && listaOrganizacionID.Contains(item.Perfil.OrganizacionID.Value))).ToList();

                        foreach (var fila in resultado)
                        {
                            EntityModel.Models.IdentidadDS.Identidad identidad = fila.Identidad;
                            identidad.Foto = $"/{UtilArchivos.ContentImagenesPersona_Organizacion.ToLower()}/{fila.Perfil.OrganizacionID.ToString().ToLower()}/{fila.Perfil.PersonaID.ToString()}{SUFIJO_PEQUE}?{fila.PersonaVinculoOrganizacion.VersionFoto.ToString().ToLower()}";
                        }

                    }
                }
                else
                {
                    var identidades = mEntityContext.Perfil.JoinIdentidad().JoinPersonaVinculoOrganizacion().Where(
                        item => item.Identidad.Tipo.Equals(1) && (item.Perfil.PersonaID.HasValue && listaPersonaID.Contains(item.Perfil.PersonaID.Value) && item.Perfil.OrganizacionID.HasValue && listaOrganizacionID.Contains(item.Perfil.OrganizacionID.Value))).Select(item => item.Identidad).ToList();

                    foreach (EntityModel.Models.IdentidadDS.Identidad identidad in identidades)
                    {
                        identidad.Foto = PersonaAD.SIN_IMAGENES_PERSONA;
                    }
                }
                mEntityContext.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene el id de la persona si la identidad es de tipo 0, el de la organizaci�n si es cualquier otro tipo
        /// </summary>
        /// <param name="pIdentidadID">Identidad id</param>
        /// <returns></returns>
        public Guid? ObtenerPersonaIDDeIdentidad(Guid pIdentidadID)
        {
            return mEntityContext.Identidad.JoinPerfil().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID)).Select(item => item.Perfil.PersonaID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el id de la persona si la identidad es de tipo 0, el de la organizaci�n si es cualquier otro tipo
        /// </summary>
        /// <param name="pIdentidadID">Identidad id</param>
        /// <returns></returns>
        public Guid ObtenerPersonaOOrganizacionIDDeIdentidad(Guid pIdentidadID)
        {
            return mEntityContext.Perfil.JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID) && item.Identidad.Tipo.Equals(0))
               .Select(item => item.Perfil.PersonaID.Value)
               .Union(mEntityContext.Perfil.JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID) && item.Identidad.Tipo > 0)
               .Select(item => item.Perfil.OrganizacionID.Value)).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene una lista con las identidades que tienen foto y las que no (trata las identidades de Tipo 2 como si fueran de tipo 1)
        /// </summary>
        /// <param name="pListaIdentidades">Lista de identidades</param>
        /// <param name="pEsMyGNOSS">TRUE si la comprobaci�n se hace en MyGNOSS, FALSE en caso contrario</param>
        /// <returns>Dictionary(identificador de Identidad, urlFoto)</returns>
        public Dictionary<Guid, string> ObtenerSiListaIdentidadesTienenFotoSinImportarModoProfesional(List<Guid> pListaIdentidades, bool pEsMyGNOSS)
        {
            Dictionary<Guid, string> listaIdentidades = new Dictionary<Guid, string>();
            List<IdentidadesFotoSinModoProfesional> listaIdentidadesFoto = new List<IdentidadesFotoSinModoProfesional>();

            if (pListaIdentidades.Count > 0)
            {
                var primeraConsulta = mEntityContext.Identidad.JoinPerfil().JoinPersona().Where(item => item.Persona.CoordenadasFoto != null && !item.Perfil.OrganizacionID.HasValue && pListaIdentidades.Contains(item.Identidad.IdentidadID)).Select(item => new IdentidadesFotoSinModoProfesional
                {
                    IdentidadID = item.Identidad.IdentidadID,
                    Url = "/" + UtilArchivos.ContentImagenesPersonas + "/" + item.Perfil.PersonaID.Value.ToString().ToUpper() + SUFIJO_PEQUE
                }).ToList();

                var terceraConsulta = mEntityContext.Identidad.JoinPerfil().JoinPersona().JoinPersonaVinculoOrganizacion().Where(item => (item.Identidad.Tipo.Equals((short)TiposIdentidad.ProfesionalPersonal) || item.Identidad.Tipo.Equals((short)TiposIdentidad.ProfesionalCorporativo)) && item.PersonaVinculoOrganizacion.UsarFotoPersonal.Equals(false) && item.PersonaVinculoOrganizacion.CoordenadasFoto != null && pListaIdentidades.Contains(item.Identidad.IdentidadID)).Select(item => new IdentidadesFotoSinModoProfesional
                {
                    IdentidadID = item.Identidad.IdentidadID,
                    Url = "/" + UtilArchivos.ContentImagenesPersona_Organizacion + "/" + item.Perfil.OrganizacionID.Value.ToString().ToUpper() + "/" + item.Perfil.PerfilPersona.PersonaID + SUFIJO_PEQUE
                }).ToList();

                if (pEsMyGNOSS)
                {
                    var segundaConsulta = mEntityContext.Identidad.JoinPerfil().JoinPersona().JoinPersonaVinculoOrganizacion().JoinPersonaVisibleEnOrg().Where(item => item.Persona.CoordenadasFoto != null && (item.Identidad.Tipo.Equals((short)TiposIdentidad.ProfesionalPersonal) || item.Identidad.Tipo.Equals((short)TiposIdentidad.ProfesionalCorporativo)) && (item.PersonaVinculoOrganizacion.UsarFotoPersonal.Equals(true) || item.PersonaVinculoOrganizacion.Foto == null) && pListaIdentidades.Contains(item.Identidad.IdentidadID)).Select(item => new IdentidadesFotoSinModoProfesional
                    {
                        IdentidadID = item.Identidad.IdentidadID,
                        Url = "/" + item.Identidad.IdentidadID + "/" + UtilArchivos.ContentImagenesPersonas + "/" + item.Perfil.PersonaID.Value.ToString().ToUpper() + SUFIJO_PEQUE
                    }).ToList();

                    var cuartaConsulta = mEntityContext.Identidad.JoinPerfil().JoinPersona().JoinPersonaVinculoOrganizacion().Where(item => mEntityContext.PersonaVisibleEnOrg.Where(item2 => item2.PersonaID.Equals(item.Perfil.PersonaID.Value) && item.Perfil.OrganizacionID.Value.Equals(item2.OrganizacionID)).Any() && (item.Identidad.Tipo.Equals(1) || item.Identidad.Tipo.Equals(2)) && pListaIdentidades.Contains(item.Identidad.IdentidadID)).Select(item => new IdentidadesFotoSinModoProfesional
                    {
                        IdentidadID = item.Identidad.IdentidadID,
                        Url = "/" + UtilArchivos.ContentImagenesOrganizaciones + "/" + item.Perfil.OrganizacionID.Value.ToString().ToUpper() + SUFIJO_PEQUE
                    }).ToList();

                    listaIdentidadesFoto = primeraConsulta.Concat(segundaConsulta).Concat(terceraConsulta).Concat(cuartaConsulta).ToList();
                }
                else
                {
                    var segundaConsulta = mEntityContext.Identidad.JoinPerfil().JoinPersona().JoinPersonaVinculoOrganizacion().Where(item => item.Persona.CoordenadasFoto != null && (item.Identidad.Tipo.Equals((short)TiposIdentidad.ProfesionalPersonal) || item.Identidad.Tipo.Equals((short)TiposIdentidad.ProfesionalCorporativo)) && (item.PersonaVinculoOrganizacion.UsarFotoPersonal.Equals(true) || item.PersonaVinculoOrganizacion.Foto == null) && pListaIdentidades.Contains(item.Identidad.IdentidadID)).Select(item => new IdentidadesFotoSinModoProfesional
                    {
                        IdentidadID = item.Identidad.IdentidadID,
                        Url = "/" + item.Identidad.IdentidadID + "/" + UtilArchivos.ContentImagenesPersonas + "/" + item.Perfil.PersonaID.Value.ToString().ToUpper() + SUFIJO_PEQUE
                    });

                    listaIdentidadesFoto = primeraConsulta.Concat(segundaConsulta).Concat(terceraConsulta).ToList();
                }
            }

            foreach (Guid identID in pListaIdentidades)
            {
                if (listaIdentidadesFoto.Any(item => item.IdentidadID.Equals(identID)))
                {
                    listaIdentidades.Add(identID, listaIdentidadesFoto.Where(item => item.IdentidadID.Equals(identID)).Select(item => item.Url).FirstOrDefault());
                }
                else
                {
                    listaIdentidades.Add(identID, null);
                }
            }

            return listaIdentidades;
        }



        /// <summary>
        /// Obtiene "Identidad", "Perfil", "PerfilPersonaOrg" y "PerfilPersona" de los usuarios de un determinado proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDeProyecto(Guid pProyectoID)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            dataWrapper.ListaIdentidad = mEntityContext.Identidad.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapper.ListaIdentidad = mEntityContext.Identidad.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().JoinProyectoUsuarioIdentidad().Where(item => item.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID)).Select(item => item.Perfil).Distinct().ToList();

            dataWrapper.ListaPerfilRedesSociales = mEntityContext.PerfilRedesSociales.JoinIdentidad().JoinProyectoUsuarioIdentidad().Where(item => item.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID)).Select(item => item.PerfilRedesSociales).Distinct().ToList();

            dataWrapper.ListaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.JoinIdentidad().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID)).Select(item => item.PerfilPersonaOrg).Distinct().ToList();

            dataWrapper.ListaPerfilPersona = mEntityContext.PerfilPersona.JoinIdentidad().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue).Select(item => item.PerfilPersona).Distinct().ToList();

            dataWrapper.ListaPerfilOrganizacion = mEntityContext.PerfilOrganizacion.JoinIdentidad().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue).Select(item => item.PerfilOrganizacion).Distinct().ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene "Identidad", "Perfil", "PerfilPersonaOrg" y "PerfilPersona" de TODOS (bloqueados y expulsados incluidos) los usuarios de un determinado proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerTodasIdentidadesDeProyecto(Guid pProyectoID)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            dataWrapper.ListaIdentidad = mEntityContext.Identidad.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID)).Select(item => item.Perfil).Distinct().ToList();

            dataWrapper.ListaPerfilRedesSociales = mEntityContext.PerfilRedesSociales.JoinIdentidad().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID)).Select(item => item.PerfilRedesSociales).Distinct().ToList();

            dataWrapper.ListaPerfilPersonaOrg = mEntityContext.Perfil.JoinIdentidad().JoinPerfilPersonaOrgConPerfilID().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID)).Select(item => item.PerfilPersonaOrg).Distinct().ToList();

            dataWrapper.ListaPerfilPersona = mEntityContext.Perfil.JoinIdentidad().JoinPerfilPersona().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID)).Select(item => item.PerfilPersona).Distinct().ToList();

            dataWrapper.ListaPerfilOrganizacion = mEntityContext.PerfilOrganizacion.JoinIdentidad().JoinPerfil().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID)).Select(item => item.PerfilOrganizacion).Distinct().ToList();

            return dataWrapper;
        }


        /// <summary>
        /// Obtiene "Identidad", "Perfil", "PerfilPersonaOrg" y "PerfilPersona"
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerMiembrosDeProyectoParaMosaico(Guid pProyectoID, int pNumeroMiembros, bool pOrdenarPorFechaAlta)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            List<Guid> listaPerfiles = new List<Guid>();

            var query = mEntityContext.Identidad.Where(item => item.ProyectoID.Equals(pProyectoID) && !item.FechaBaja.HasValue && !item.FechaExpulsion.HasValue && !item.Tipo.Equals((short)TiposIdentidad.ProfesionalCorporativo));

            if (pOrdenarPorFechaAlta)
            {
                dataWrapper.ListaIdentidad = query.OrderByDescending(item => item.FechaAlta).Take(pNumeroMiembros).ToList();
            }
            else
            {
                dataWrapper.ListaIdentidad = query.OrderByDescending(item => item.Rank).Take(pNumeroMiembros).ToList();
            }

            if (dataWrapper.ListaIdentidad.Count > 0)
            {
                foreach (var filaIdentidad in dataWrapper.ListaIdentidad)
                {
                    listaPerfiles.Add(filaIdentidad.PerfilID);
                }

                dataWrapper.ListaPerfil = mEntityContext.Perfil.Where(item => listaPerfiles.Contains(item.PerfilID)).ToList();

                dataWrapper.ListaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.Where(item => listaPerfiles.Contains(item.PerfilID)).ToList();

                dataWrapper.ListaPerfilPersona = mEntityContext.PerfilPersona.Where(item => listaPerfiles.Contains(item.PerfilID)).ToList();

                dataWrapper.ListaPerfilOrganizacion = mEntityContext.PerfilOrganizacion.Where(item => listaPerfiles.Contains(item.PerfilID)).ToList();

            }
            return dataWrapper;
        }

        /// <summary>
        /// Obtiene "Identidad", "Perfil", "PerfilPersonaOrg" y "PerfilPersona" de TODOS (bloqueados y expulsados incluidos) los usuarios de un determinado proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de identidades</returns>
        public List<Guid> ObtenerTodasIdentidadesIDMyGnossDeProyectoActivas(Guid pProyectoID)
        {
            var resultado = mEntityContext.Perfil.JoinIdentidad().JoinIdentidad2().Where(item => item.Identidad2.ProyectoID.Equals(pProyectoID) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !item.Identidad2.FechaExpulsion.HasValue && !item.Identidad2.FechaBaja.HasValue).Select(item => item.Identidad.IdentidadID).ToList();

            List<Guid> listaIdentID = new List<Guid>();
            foreach (var item in resultado)
            {
                listaIdentID.Add((Guid)item);
            }

            return listaIdentID;
        }

        /// <summary>
        /// Obtiene "Identidad", "Perfil", "PerfilPersonaOrg" y "PerfilOrganizacion" de los usuarios vinculados a una determinada organizacion y la misma organizacion
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci�n</param>
        /// <param name="pSoloActivos">Indica si debe traer solo las indentidades activas o todas</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDeOrganizacionYEmpleados(Guid pOrganizacionID, bool pSoloActivos)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            var ident = mEntityContext.Perfil.JoinIdentidad().Where(item => item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && !item.Identidad.FechaBaja.HasValue)
                .Select(item => item.Identidad)
                .Union(mEntityContext.Perfil.JoinIdentidad().JoinPerfilPersonaOrgConOrganizacionID().Where(item => !item.Perfil.PersonaID.HasValue && item.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizacionID) && !item.Identidad.FechaBaja.HasValue)
                .Select(item => item.Identidad)).ToList();

            if (!pSoloActivos)
            {
                ident = mEntityContext.Perfil.JoinIdentidad().JoinPerfilPersonaOrgConPerfilID().Where(item => item.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizacionID))
                    .Select(item => item.Identidad)
                    .Union(mEntityContext.PerfilOrganizacion.JoinIdentidad().JoinPerfil().Where(item => item.PerfilOrganizacion.OrganizacionID.Equals(pOrganizacionID))
                    .Select(item => item.Identidad)).ToList();
            }
            dataWrapper.ListaIdentidad = ident;

            var perf = mEntityContext.PerfilPersonaOrg.JoinPerfil().Where(item => item.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizacionID) && !item.Perfil.Eliminado)
                .Select(item => item.Perfil)
                .Concat(mEntityContext.PerfilOrganizacion.JoinPerfil().Where(item => item.PerfilOrganizacion.OrganizacionID.Equals(pOrganizacionID) && !item.Perfil.Eliminado)
                .Select(item => item.Perfil)).ToList();

            if (!pSoloActivos)
            {
                perf = mEntityContext.PerfilPersonaOrg.JoinPerfil().Where(item => item.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizacionID))
                    .Select(item => item.Perfil)
                    .Concat(mEntityContext.PerfilOrganizacion.JoinPerfil().Where(item => item.PerfilOrganizacion.OrganizacionID.Equals(pOrganizacionID))
                    .Select(item => item.Perfil)).ToList();
            }
            dataWrapper.ListaPerfil = perf;

            var perfilPersOrg = mEntityContext.PerfilPersonaOrg.JoinIdentidad().JoinPerfil().Where(item => item.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizacionID) && !item.Identidad.FechaBaja.HasValue && !item.Perfil.Eliminado)
                .Select(item => item.PerfilPersonaOrg).Distinct().ToList();

            if (!pSoloActivos)
            {
                perfilPersOrg = mEntityContext.PerfilPersonaOrg.JoinIdentidad().JoinPerfil().Where(item => item.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizacionID))
                    .Select(item => item.PerfilPersonaOrg).ToList();
            }
            dataWrapper.ListaPerfilPersonaOrg = perfilPersOrg;

            var perfOrg = mEntityContext.PerfilOrganizacion.JoinPerfil().Where(item => item.PerfilOrganizacion.OrganizacionID.Equals(pOrganizacionID) && !item.Perfil.Eliminado)
                .Select(item => item.PerfilOrganizacion).ToList();

            if (!pSoloActivos)
            {
                perfOrg = mEntityContext.PerfilOrganizacion.JoinPerfil().Where(item => item.PerfilOrganizacion.OrganizacionID.Equals(pOrganizacionID))
                    .Select(item => item.PerfilOrganizacion).ToList();
            }
            dataWrapper.ListaPerfilOrganizacion = perfOrg;

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene las identidades de unas organizaciones en un proyecto
        /// </summary>
        /// <param name="pListaOrganizacionesID">Identificadores de las organizaci�nes</param>
        /// <param name="pProyecto">Proyecto del que debe obtener las identidades</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDeOrganizaciones(IList<Guid> pListaOrganizacionesID, Guid pProyecto)
        {
            return ObtenerIdentidadesDeOrganizaciones(pListaOrganizacionesID, pProyecto, null);
        }

        /// <summary>
        /// Obtiene las identidades de unas organizaciones en un proyecto
        /// </summary>
        /// <param name="pListaOrganizacionesID">Identificadores de las organizaci�nes</param>
        /// <param name="pProyecto">Proyecto del que debe obtener las identidades</param>
        /// <param name="pTipoIdentidad">Tipo de identidad que se quiere obtener (NULL para obtener todos los tipos)</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDeOrganizaciones(IList<Guid> pListaOrganizacionesID, Guid pProyecto, TiposIdentidad? pTipoIdentidad)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            if ((pListaOrganizacionesID != null) && (pListaOrganizacionesID.Count > 0))
            {
                var ident = mEntityContext.Perfil.JoinIdentidad().Where(item => !item.Perfil.Eliminado && item.Perfil.OrganizacionID.HasValue && pListaOrganizacionesID.Contains(item.Perfil.OrganizacionID.Value) && item.Identidad.ProyectoID.Equals(pProyecto) && !item.Identidad.FechaBaja.HasValue).Distinct();

                if (pTipoIdentidad != null)
                {
                    ident = ident.Where(item => item.Identidad.Tipo.Equals((short)pTipoIdentidad));
                }

                dataWrapper.ListaIdentidad = ident.Select(item => item.Identidad).ToList();

                dataWrapper.ListaPerfil = ident.Select(item => item.Perfil).ToList();

                dataWrapper.ListaPerfilPersonaOrg = ident.JoinPerfilPersonaOrgConPerfilID().Select(item => item.PerfilPersonaOrg).ToList();

                dataWrapper.ListaPerfilOrganizacion = ident.JoinPerfilOrganizacion().Select(item => item.PerfilOrganizacion).ToList();

            }
            return dataWrapper;
        }

        /// <summary>
        /// Obtiene las identidades de una organizacion en un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci�n</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pObtenerMyGnoss">TRUE para obtener tambi�n la identidad en MyGnoss</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadDeOrganizacion(Guid pOrganizacionID, Guid pProyectoID, bool pObtenerMyGnoss)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            if (pObtenerMyGnoss)
            {
                dataWrapper.ListaIdentidad = mEntityContext.Perfil.JoinIdentidad().Where(item => !item.Perfil.Eliminado && item.Identidad.Tipo.Equals(3) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && !item.Identidad.FechaBaja.HasValue && (item.Identidad.ProyectoID.Equals(pProyectoID) || item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto))).Select(item => item.Identidad).ToList();

                dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().Where(item => !item.Perfil.Eliminado && item.Identidad.Tipo.Equals(3) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && !item.Identidad.FechaBaja.HasValue && (item.Identidad.ProyectoID.Equals(pProyectoID) || item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto))).Select(item => item.Perfil).Distinct().ToList();

                dataWrapper.ListaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.JoinIdentidad().JoinPerfil().Where(item => !item.Perfil.Eliminado && item.Identidad.Tipo.Equals(3) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && !item.Identidad.FechaBaja.HasValue && (item.Identidad.ProyectoID.Equals(pProyectoID) || item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto))).Select(item => item.PerfilPersonaOrg).Distinct().ToList();

                dataWrapper.ListaPerfilOrganizacion = mEntityContext.PerfilOrganizacion.JoinIdentidad().JoinPerfil().Where(item => !item.Perfil.Eliminado && item.Identidad.Tipo.Equals(3) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && !item.Identidad.FechaBaja.HasValue && (item.Identidad.ProyectoID.Equals(pProyectoID) || item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto))).Select(item => item.PerfilOrganizacion).Distinct().ToList();
            }
            else
            {
                dataWrapper.ListaIdentidad = mEntityContext.Perfil.JoinIdentidad().Where(item => !item.Perfil.Eliminado && item.Identidad.Tipo.Equals(3) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && !item.Identidad.FechaBaja.HasValue && item.Identidad.ProyectoID.Equals(pProyectoID)).Select(item => item.Identidad).ToList();

                dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().Where(item => !item.Perfil.Eliminado && item.Identidad.Tipo.Equals(3) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && !item.Identidad.FechaBaja.HasValue && item.Identidad.ProyectoID.Equals(pProyectoID)).Select(item => item.Perfil).Distinct().ToList();

                dataWrapper.ListaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.JoinIdentidad().JoinPerfil().Where(item => !item.Perfil.Eliminado && item.Identidad.Tipo.Equals(3) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && !item.Identidad.FechaBaja.HasValue && item.Identidad.ProyectoID.Equals(pProyectoID)).Select(item => item.PerfilPersonaOrg).Distinct().ToList();

                dataWrapper.ListaPerfilOrganizacion = mEntityContext.PerfilOrganizacion.JoinIdentidad().JoinPerfil().Where(item => !item.Perfil.Eliminado && item.Identidad.Tipo.Equals(3) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && !item.Identidad.FechaBaja.HasValue && item.Identidad.ProyectoID.Equals(pProyectoID)).Select(item => item.PerfilOrganizacion).Distinct().ToList();
            }
            return dataWrapper;
        }

        /// <summary>
        /// Obtiene las identidades de los miembros de una organizacion con permisos para ver al contacto
        /// </summary>
        /// <param name="pIdentidadOrganizacion">Identidad en MyGNOSS de la organizacion</param>
        /// <param name="pIdentidad">Identidad en MyGNOSS del usuario</param>
        /// <returns>IdentidadDS</returns>
        public DataWrapperIdentidad ObtenerMiembrosOrganizacionConPermisoDeContactoAIdentidad(Guid pIdentidadOrganizacion, Guid pIdentidad)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            var subConsulta = mEntityContext.PermisoAmigoOrg.JoinIdentidad().Where(item => item.PermisoAmigoOrg.IdentidadAmigoID.Equals(pIdentidad) && item.PermisoAmigoOrg.IdentidadOrganizacionID.Equals(pIdentidadOrganizacion))
                .Select(item => item.Identidad.IdentidadID)
                .Union(mEntityContext.PermisoGrupoAmigoOrg.JoinGrupoAmigos().JoinAmigoAgGrupo().JoinIdentidad().Where(item => item.AmigoAgGrupo.IdentidadAmigoID.Equals(pIdentidad) && item.PermisoGrupoAmigoOrg.IdentidadOrganizacionID.Equals(pIdentidadOrganizacion))
                .Select(item => item.Identidad.IdentidadID));

            dataWrapper.ListaIdentidad = mEntityContext.Identidad.Where(item => subConsulta.Contains(item.IdentidadID)).ToList();

            dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().Where(item => subConsulta.Contains(item.Identidad.IdentidadID)).Select(item => item.Perfil).Distinct().ToList();

            dataWrapper.ListaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.JoinIdentidad().Where(item => subConsulta.Contains(item.Identidad.IdentidadID))
                .Select(item => item.PerfilPersonaOrg).Distinct().ToList();

            dataWrapper.ListaPerfilOrganizacion = mEntityContext.PerfilOrganizacion.JoinIdentidad().Where(item => subConsulta.Contains(item.Identidad.IdentidadID)).Select(item => item.PerfilOrganizacion).Distinct().ToList();

            return dataWrapper;
        }


        /// <summary>
        /// Obtiene las identidades de los miembros de una organizacion
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>IdentidadDS</returns>
        public List<IdentidadesDeUsuariosDeOrganizacionDeProyectoYDeMetaProyecto> ObtenerTodasIdentidadesDeUsuariosDeOrganizacionDeProyectoYDeMetaProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            List<Guid> listaPersonaID = mEntityContext.Perfil.Where(item => item.OrganizacionID.HasValue && item.OrganizacionID.Value.Equals(pOrganizacionID) && item.PersonaID.HasValue).Select(item => item.PersonaID.Value).ToList();

            var consultaSinWhere = mEntityContext.Perfil.JoinIdentidad().JoinPersona().GroupJoin(mEntityContext.AdministradorProyecto, join => new { UsuarioID = join.Persona.UsuarioID.Value, ProyectoID = join.Identidad.ProyectoID }, admin => new { UsuarioID = admin.UsuarioID, ProyectoID = admin.ProyectoID }, (join, admin) =>
            new
            {
                Perfil = join.Perfil,
                Identidad = join.Identidad,
                Persona = join.Persona,
                AdministradorProyecto = admin
            }).SelectMany(item => item.AdministradorProyecto.DefaultIfEmpty(), (item, admin) =>
            new
            {
                Perfil = item.Perfil,
                Identidad = item.Identidad,
                Persona = item.Persona,
                AdministradorProyecto = admin
            });
            var consultaWhere = consultaSinWhere.Where(item => listaPersonaID.Contains(item.Persona.PersonaID) && item.Persona.UsuarioID.HasValue && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue && !item.Perfil.Eliminado && !item.Persona.Eliminado && ((item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto)) || item.Identidad.ProyectoID.Equals(pProyectoID))).ToList();

            return consultaWhere.Select(item => new IdentidadesDeUsuariosDeOrganizacionDeProyectoYDeMetaProyecto
            {
                PersonaID = item.Perfil.PersonaID,
                OrganizacionID = item.Perfil.OrganizacionID,
                ProyectoID = item.Identidad.ProyectoID,
                IdentidadID = item.Identidad.IdentidadID,
                NombrePerfil = item.Perfil.NombrePerfil,
                TipoParticipacion = item.Identidad.Tipo,
                TipoAdministracion = item.AdministradorProyecto != null ? item.AdministradorProyecto.Tipo : (short)2
            }).OrderByDescending(item => item.NombrePerfil).ThenBy(item => item.PersonaID.Value).ToList();
        }

        /// <summary>
        /// Obtiene a partir del identificador de usuario todos los posibles perfiles e identidades que tenga (eliminados o no)
        /// Carga las tablas "Perfil", "Identidad", "PerfilPersona" y "PerfilPersonaOrg"
        /// </summary>
        /// <param name="pUsuarioID">Clave del usuario</param>
        /// <returns>IdentidadDS</returns>
        public DataWrapperIdentidad ObtenerPerfilesDeUsuario(Guid pUsuarioID)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            List<Perfil> listaPerfiles = mEntityContext.PerfilPersona.JoinPerfil().JoinPersona().Where(item => item.Persona.UsuarioID.Value.Equals(pUsuarioID) && !item.Perfil.Eliminado)
                .Select(item => item.Perfil).ToList();

            Perfil perfil = mEntityContext.Perfil.JoinPersona().Where(item => item.Persona.UsuarioID.Value.Equals(pUsuarioID)).Select(item => item.Perfil).FirstOrDefault();


            dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinPersona().Where(item => item.Persona.UsuarioID.Value.Equals(pUsuarioID) && !item.Perfil.Eliminado)
                .Select(item => item.Perfil).ToList();

            dataWrapper.ListaPerfilPersona = mEntityContext.PerfilPersona.JoinPersona().Where(item => item.Persona.UsuarioID.Value.Equals(pUsuarioID))
                .Select(item => item.PerfilPersona).ToList();

            dataWrapper.ListaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.JoinPersona().Where(item => item.Persona.UsuarioID.Value.Equals(pUsuarioID))
                .Select(item => item.PerfilPersonaOrg).ToList();

            dataWrapper.ListaIdentidad = mEntityContext.Perfil.JoinIdentidad().JoinPersona().Where(item => item.Persona.UsuarioID.Value.Equals(pUsuarioID))
                .Select(item => item.Identidad).ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene a partir del identificador de usuario todos los posibles perfiles que tenga activos en una lista.
        /// </summary>
        /// <param name="pUsuarioID">Clave del usuario</param>
        /// <returns>Lista de IDs de perfiles</returns>
        public List<Guid> ObtenerListaPerfilesDeUsuario(Guid pUsuarioID)
        {
            List<Guid> perfiles = new List<Guid>();

            var resultado = mEntityContext.Perfil.JoinPersona().Where(item => item.Persona.UsuarioID.HasValue && item.Persona.UsuarioID.Value.Equals(pUsuarioID) && !item.Perfil.Eliminado).Select(item => item.Perfil).ToList();

            foreach (var item in resultado)
            {
                perfiles.Add(item.PerfilID);
            }
            return perfiles;
        }

        /// <summary>
        /// Obtiene a partir del identificador de usuario todos los posibles perfiles que tenga activos en una lista.
        /// </summary>
        /// <param name="pUsuarioID">Clave del usuario</param>
        /// <returns>Lista de IDs de perfiles</returns>
        public List<Guid> ObtenerListaIdentidadesDeUsuario(Guid pUsuarioID)
        {
            List<Guid> perfiles = new List<Guid>();

            var resultado = mEntityContext.Perfil.JoinPersona().JoinIdentidad().Where(item => item.Persona.UsuarioID.HasValue && item.Persona.UsuarioID.Value.Equals(pUsuarioID) && !item.Perfil.Eliminado).Select(item => item.Identidad).ToList();

            foreach (var item in resultado)
            {
                perfiles.Add(item.IdentidadID);
            }
            return perfiles;
        }

        /// <summary>
        /// Obtiene a partir de los identificadores de usuario todos los posibles perfiles que tengan activos en una lista.
        /// </summary>
        /// <param name="pListaUsuarioIDs">Lista de identificadores de usuario</param>
        /// <returns>Diccionario de UsuarioID y lista de PerfilesID</returns>
        public Dictionary<Guid, List<Guid>> ObtenerListaIdentidadesDeListaUsuarios(List<Guid> pListaUsuarioIDs)
        {
            Dictionary<Guid, List<Guid>> dicUsuarioIDPerfilID = new Dictionary<Guid, List<Guid>>();

            var resultado = mEntityContext.Perfil.JoinPersona().JoinIdentidad().Where(item => !item.Perfil.Eliminado && item.Persona.UsuarioID.HasValue && pListaUsuarioIDs.Contains(item.Persona.UsuarioID.Value) && item.Persona.UsuarioID.HasValue).Select(item => new { item.Persona.UsuarioID, item.Identidad.IdentidadID}).ToList();

            foreach (var fila in resultado)
            {
                Guid usuarioID = (Guid)fila.UsuarioID.Value;
                Guid identidadID = (Guid)fila.IdentidadID;

                if (!dicUsuarioIDPerfilID.ContainsKey(usuarioID))
                {
                    dicUsuarioIDPerfilID.Add(usuarioID, new List<Guid>());
                }

                if (!dicUsuarioIDPerfilID[usuarioID].Contains(identidadID))
                {
                    dicUsuarioIDPerfilID[usuarioID].Add(identidadID);
                }
            }
            return dicUsuarioIDPerfilID;
        }

        /// <summary>
        /// Obtiene a partir de los identificadores de usuario todos los posibles perfiles que tengan activos en una lista.
        /// </summary>
        /// <param name="pListaUsuarioIDs">Lista de identificadores de usuario</param>
        /// <returns>Diccionario de UsuarioID y lista de PerfilesID</returns>
        public Dictionary<Guid, List<Guid>> ObtenerListaPerfilesDeListaUsuarios(List<Guid> pListaUsuarioIDs)
        {
            Dictionary<Guid, List<Guid>> dicUsuarioIDPerfilID = new Dictionary<Guid, List<Guid>>();

            var resultado = mEntityContext.Perfil.JoinPersona().Where(item => !item.Perfil.Eliminado && item.Persona.UsuarioID.HasValue && pListaUsuarioIDs.Contains(item.Persona.UsuarioID.Value) && item.Persona.UsuarioID.HasValue).Select(item => new { item.Persona.UsuarioID, item.Perfil.PerfilID }).ToList();

            foreach (var fila in resultado)
            {
                Guid usuarioID = (Guid)fila.UsuarioID.Value;
                Guid perfilID = (Guid)fila.PerfilID;

                if (!dicUsuarioIDPerfilID.ContainsKey(usuarioID))
                {
                    dicUsuarioIDPerfilID.Add(usuarioID, new List<Guid>());
                }

                if (!dicUsuarioIDPerfilID[usuarioID].Contains(perfilID))
                {
                    dicUsuarioIDPerfilID[usuarioID].Add(perfilID);
                }
            }
            return dicUsuarioIDPerfilID;
        }

        /// <summary>
        /// Obtiene el ID de un perfil a partir de la tabla TesauroUsuario a partir de TesauroID
        /// </summary>
        /// <param name="pTesauroID">Id del tesauro</param>
        /// <returns></returns>
        public Guid? ObtenerPerfilIDPorIDTesauro(Guid pTesauroID)
        {

            Perfil perfil = mEntityContext.Perfil.JoinIdentidad().JoinPersona().JoinTesauroUsuario().JoinCategoriaTesauro().Where(item => item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && item.Identidad.Tipo == 0 && item.CategoriaTesauro.CategoriaTesauroID.Equals(pTesauroID)).Select(item => item.Perfil).FirstOrDefault();

            if ((perfil != null) && (!perfil.PerfilID.Equals(Guid.Empty)))
            {
                return perfil.PerfilID;
            }

            return null;
        }

        /// <summary>
        /// Obtiene a trav�s del UsuarioID todos el perfil profesor. Carga solo la tabla "Perfil"
        /// </summary>
        /// <param name="pUsuarioID">Clave del usuario</param>
        /// <returns>IdentidadDS</returns>
        public DataWrapperIdentidad ObtenerPerfilProfesorDeUsuario(Guid pUsuarioID)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().JoinPersona().JoinPerfilPersonaOrg().Where(item => item.Persona.UsuarioID.Value.Equals(pUsuarioID) && item.Identidad.Tipo.Equals((short)TiposIdentidad.Profesor)).Select(item => item.Perfil).Distinct().ToList();

            dataWrapper.ListaIdentidad = mEntityContext.Perfil.JoinIdentidad().JoinPersona().JoinPerfilPersonaOrg().Where(item => item.Persona.UsuarioID.Value.Equals(pUsuarioID) && item.Identidad.Tipo.Equals((short)TiposIdentidad.Profesor)).Select(item => item.Identidad).Distinct().ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene atraves del UsuarioID todos los posibles perfiles e identidades que tenga (eliminados o no). Carga tabla "Perfil" y "Identidad" , "PerfilPersona" , "PerfilPersonaOrg"
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerPerfilesDeUsuarioEnProyecto(Guid pUsuarioID, Guid pProyectoID)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().JoinPersona().JoinPerfilPersona().Where(item => item.Persona.UsuarioID.Value.Equals(pUsuarioID) && item.Identidad.ProyectoID.Equals(pProyectoID))
                .Select(item => item.Perfil)
                .Union(mEntityContext.Perfil.JoinIdentidad().JoinPersona().JoinPerfilPersonaOrg().Where(item => item.Persona.UsuarioID.Value.Equals(pUsuarioID) && item.Identidad.ProyectoID.Equals(pProyectoID))
                .Select(item => item.Perfil)).ToList();

            dataWrapper.ListaPerfilPersona = mEntityContext.PerfilPersona.JoinIdentidad().JoinPersona().Where(item => item.Persona.UsuarioID.Value.Equals(pUsuarioID) && item.Identidad.ProyectoID.Equals(pProyectoID)).Select(item => item.PerfilPersona).ToList();

            dataWrapper.ListaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.JoinIdentidad().JoinPersona().Where(item => item.Persona.UsuarioID.Value.Equals(pUsuarioID) && item.Identidad.ProyectoID.Equals(pProyectoID))
                .Select(item => item.PerfilPersonaOrg).ToList();

            List<Guid> listaPerfilID = mEntityContext.Perfil.JoinIdentidad().JoinPersona().JoinPerfilPersona().Where(item => item.Persona.UsuarioID.Value.Equals(pUsuarioID) && item.Identidad.ProyectoID.Equals(pProyectoID))
                .Select(item => item.Perfil.PerfilID)
                .Union(mEntityContext.Perfil.JoinIdentidad().JoinPersona().JoinPerfilPersonaOrg().Where(item => item.Persona.UsuarioID.Value.Equals(pUsuarioID) && item.Identidad.ProyectoID.Equals(pProyectoID))
                .Select(item => item.Perfil.PerfilID)).ToList();

            dataWrapper.ListaIdentidad = mEntityContext.Identidad.Where(item => listaPerfilID.Contains(item.PerfilID)).ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene una lista con todos los perfiles de una organizaci�n (empleados y organizaci�n).
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci�n</param>
        /// <returns>Lista con todos los perfiles de una organizaci�n (empleados y organizaci�n)</returns>
        public List<Guid> ObtenerListaPerfilesDeOrganizacion(Guid pOrganizacionID)
        {
            List<Guid> listaPerfiles = new List<Guid>();

            var resultado = mEntityContext.Perfil.Where(item => item.OrganizacionID.Value.Equals(pOrganizacionID)).ToList();

            foreach (var filaPerfil in resultado)
            {
                listaPerfiles.Add(filaPerfil.PerfilID);
            }

            return listaPerfiles;
        }


        /// <summary>
        /// Obtiene el identificador del perfil de la organizaci�n
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci�n</param>
        /// <returns></returns>
        public Guid? ObtenerPerfilIDDeOrganizacion(Guid pOrganizacionID)
        {
            return mEntityContext.Perfil.Where(item => item.OrganizacionID.Value.Equals(pOrganizacionID) && !item.PersonaID.HasValue).Select(item => item.PerfilID).FirstOrDefault();
        }


        /// <summary>
        /// Obtiene el perfil de la organizaci�n
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci�n</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerPerfilDeOrganizacion(Guid pOrganizacionID)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            dataWrapper.ListaPerfil = mEntityContext.PerfilOrganizacion.JoinPerfil().Where(item => item.PerfilOrganizacion.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.Perfil).ToList();

            dataWrapper.ListaPerfilOrganizacion = mEntityContext.PerfilOrganizacion.JoinPerfil().Where(item => item.PerfilOrganizacion.OrganizacionID.Equals(pOrganizacionID) && !item.Perfil.Eliminado).Select(item => item.PerfilOrganizacion).ToList();

            dataWrapper.ListaIdentidad = mEntityContext.Perfil.JoinIdentidad().JoinPerfilOrganizacion().Where(item => item.PerfilOrganizacion.OrganizacionID.Equals(pOrganizacionID))
                .Select(item => item.Identidad).ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene una lista de identificadores de perfiles de todos los usuarios que participan en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto del que se quieren conocer los perfiles</param>
        /// <returns>Lista de identificadores de perfiles</returns>
        public List<Guid> ObtenerPerfilesIDDeProyecto(Guid pProyectoID)
        {
            List<Guid> listaPerfilesIDs = new List<Guid>();
            var resultado = mEntityContext.Identidad.Where(item => item.ProyectoID.Equals(pProyectoID) && !item.FechaBaja.HasValue && !item.FechaExpulsion.HasValue).ToList();

            foreach (var item in resultado)
            {
                listaPerfilesIDs.Add(item.PerfilID);
            }

            return listaPerfilesIDs;
        }

        /// <summary>
        /// Obtiene el los miembros de una comunidad
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerMiembrosComunidad(Guid pProyectoID)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue && !item.Identidad.Tipo.Equals((short)TiposIdentidad.ProfesionalCorporativo)).Select(item => item.Perfil).ToList();

            dataWrapper.ListaIdentidad = mEntityContext.Identidad.Where(item => item.ProyectoID.Equals(pProyectoID) && !item.FechaBaja.HasValue && !item.FechaExpulsion.HasValue && !item.Tipo.Equals((short)TiposIdentidad.ProfesionalCorporativo)).ToList();

            dataWrapper.ListaGrupoIdentidades = dataWrapper.ListaGrupoIdentidades = mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesProyecto().Where(item => item.GrupoIdentidadesProyecto.ProyectoID.Equals(pProyectoID) && !item.GrupoIdentidades.FechaBaja.HasValue).Select(item => item.GrupoIdentidades).ToList();

            dataWrapper.ListaGrupoIdentidadesParticipacion = mEntityContext.GrupoIdentidadesParticipacion.JoinGrupoIdentidadesProyecto().Where(item => item.GrupoIdentidadesProyecto.ProyectoID.Equals(pProyectoID) && !item.GrupoIdentidadesParticipacion.FechaBaja.HasValue).Select(item => item.GrupoIdentidadesParticipacion).ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene los perfiles de una comunidad a partir del nombre para el autocompletar
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <param name="pIdentidadID">Identidad del usuario que hace la petici�n(para que no aparezca el mismo)</param>
        /// <param name="pBusqueda">Texto a buscar</param>
        /// <param name="pNumero">N�mero de resultados</param>
        /// <returns>Diccionario con:
        /// Clave=PerfilID
        /// Item1=NombrePerfil
        /// Item2=NombreOrg
        /// Item3=PersonaID
        /// Item4=OrganizacionID
        /// </returns>
        public Dictionary<Guid, Tuple<string, string, Guid?, Guid?>> ObtenerPerfilesParaAutocompletar(Guid pProyectoID, Guid pIdentidadID, string pBusqueda, int pNumero, bool pOmitirMiPerfil)
        {
            return ObtenerPerfilesParaAutocompletarDeIdentidadesID(null, pProyectoID, pIdentidadID, pBusqueda, pNumero, pOmitirMiPerfil);
        }

        /// <summary>
        /// Obtiene los perfiles de una comunidad a partir del nombre para el autocompletar
        /// </summary>
        /// <param name="pListaIdentidadesID">Lista de identidades de las que queremos obtener el perfil</param>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <param name="pIdentidadID">Identidad del usuario que hace la petici�n(para que no aparezca el mismo)</param>
        /// <param name="pBusqueda">Texto a buscar</param>
        /// <param name="pNumero">N�mero de resultados</param>
        /// <returns>Diccionario con:
        /// Clave=PerfilID
        /// Item1=NombrePerfil
        /// Item2=NombreOrg
        /// Item3=PersonaID
        /// Item4=OrganizacionID
        /// </returns>
        public Dictionary<Guid, Tuple<string, string, Guid?, Guid?>> ObtenerPerfilesParaAutocompletarDeIdentidadesID(List<Guid> pListaIdentidadesID, Guid pProyectoID, Guid pIdentidadID, string pBusqueda, int pNumero, bool pOmitirMiPerfil)
        {
            Dictionary<Guid, Tuple<string, string, Guid?, Guid?>> listaPerfiles = new Dictionary<Guid, Tuple<string, string, Guid?, Guid?>>();

            var consultaSQL = mEntityContext.Perfil.JoinIdentidad().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue && !item.Identidad.Tipo.Equals((short)TiposIdentidad.ProfesionalCorporativo) && item.Perfil.PersonaID.HasValue && (item.Perfil.NombrePerfil.ToLower().Contains(pBusqueda.ToLower()) || item.Perfil.NombrePerfil.ToLower().StartsWith(pBusqueda.ToLower()))).Select(item => new { item.Perfil.PerfilID, item.Perfil.NombrePerfil, item.Perfil.NombreOrganizacion, item.Perfil.PersonaID, item.Perfil.OrganizacionID }).Take(pNumero).OrderByDescending(item => item.NombrePerfil);
            var resultado = consultaSQL.ToList();
            if (pOmitirMiPerfil)
            {
                resultado = mEntityContext.Perfil.JoinIdentidad().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue && !item.Identidad.Tipo.Equals((short)TiposIdentidad.ProfesionalCorporativo) && item.Perfil.PersonaID.HasValue && (item.Perfil.NombrePerfil.ToLower().Contains(pBusqueda) || item.Perfil.NombrePerfil.ToLower().StartsWith(pBusqueda)) && !item.Identidad.ProyectoID.Equals(pIdentidadID)).Select(item => new { item.Perfil.PerfilID, item.Perfil.NombrePerfil, item.Perfil.NombreOrganizacion, item.Perfil.PersonaID, item.Perfil.OrganizacionID }).Take(pNumero).OrderByDescending(item => item.NombrePerfil).ToList();
            }
            if (pListaIdentidadesID != null && pListaIdentidadesID.Count > 0)
            {
                resultado = mEntityContext.Perfil.JoinIdentidad().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue && !item.Identidad.Tipo.Equals((short)TiposIdentidad.ProfesionalCorporativo) && item.Perfil.PersonaID.HasValue && (item.Perfil.NombrePerfil.ToLower().Contains(pBusqueda) || item.Perfil.NombrePerfil.ToLower().StartsWith(pBusqueda)) && pListaIdentidadesID.Contains(item.Identidad.IdentidadID)).Select(item => new { item.Perfil.PerfilID, item.Perfil.NombrePerfil, item.Perfil.NombreOrganizacion, item.Perfil.PersonaID, item.Perfil.OrganizacionID }).Take(pNumero).OrderByDescending(item => item.NombrePerfil).ToList();
            }

            foreach (var item in resultado)
            {
                if (!listaPerfiles.ContainsKey(item.PerfilID))
                {
                    listaPerfiles.Add(item.PerfilID, new Tuple<string, string, Guid?, Guid?>(item.NombrePerfil, item.NombreOrganizacion, item.PersonaID, item.OrganizacionID));
                }
            }

            return listaPerfiles;
        }

        /// <summary>
        /// Obtiene el los nombres de los grupos de una comunidad 
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>Diccionario con GrupoID-NombreGrupo/returns>
        public Dictionary<Guid, string> ObtenerNombresTodosGruposProyecto(Guid pProyectoID)
        {
            Dictionary<Guid, string> listaGrupos = new Dictionary<Guid, string>();

            var resultado = mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesProyecto().Where(item => item.GrupoIdentidadesProyecto.ProyectoID.Equals(pProyectoID) && !item.GrupoIdentidades.FechaBaja.HasValue).Select(item => new { item.GrupoIdentidades.GrupoID, item.GrupoIdentidades.Nombre }).OrderByDescending(item => item.Nombre).ToList();

            foreach (var item in resultado)
            {
                listaGrupos.Add(item.GrupoID, item.Nombre);
            }

            return listaGrupos;
        }

        /// <summary>
        /// Obtiene el los miembros de una comunidad a partir del nombre para el autocompletar
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <param name="pIdentidadID">Identidad del usuario que hace la petici�n(para que aparezcan los p�blicos o aquellos a los que pertenezca la identidad)</param>
        /// <param name="pBusqueda">Texto a buscar</param>
        /// <param name="pNumero">N�mero de resultados</param>
        /// <param name="pEsSupervisor">Indica si el usuario que hace la petici�n tiene permiso de supervisi�n para mostrarle todos los grupos de la comunidad</param>
        /// <returns>Diccionario con GrupoID-NombreGrupo/returns>
        public Dictionary<Guid, string> ObtenerGruposParaAutocompletar(Guid pProyectoID, Guid pIdentidadID, string pBusqueda, int pNumero, bool pEsSupervisor)
        {
            Dictionary<Guid, string> listaGrupos = new Dictionary<Guid, string>();

            var resultado = mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesProyecto().Where(item => item.GrupoIdentidadesProyecto.ProyectoID.Equals(pProyectoID) && !item.GrupoIdentidades.FechaBaja.HasValue && (item.GrupoIdentidades.Nombre.Contains(pBusqueda) || item.GrupoIdentidades.Nombre.StartsWith(pBusqueda)))
                .Select(item => new { item.GrupoIdentidades.GrupoID, item.GrupoIdentidades.Nombre }).Take(pNumero).OrderByDescending(item => item.Nombre).ToList();

            if (!pEsSupervisor)
            {
                resultado = mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesProyecto().JoinGrupoIdentidadesParticipacion().Where(item => item.GrupoIdentidadesProyecto.ProyectoID.Equals(pProyectoID) && !item.GrupoIdentidades.FechaBaja.HasValue && (item.GrupoIdentidades.Nombre.Contains(pBusqueda) || item.GrupoIdentidades.Nombre.StartsWith(pBusqueda)) && ((item.GrupoIdentidadesParticipacion.IdentidadID.Equals(pIdentidadID) && !item.GrupoIdentidadesParticipacion.FechaBaja.HasValue) || item.GrupoIdentidades.Publico.Equals(true)))
                .Select(item => new { item.GrupoIdentidades.GrupoID, item.GrupoIdentidades.Nombre }).Take(pNumero).OrderByDescending(item => item.Nombre).ToList();
            }

            foreach (var item in resultado)
            {
                if (!listaGrupos.ContainsKey(item.GrupoID))
                {
                    listaGrupos.Add(item.GrupoID, item.Nombre);
                }
            }
            return listaGrupos;
        }

        /// <summary>
        /// Obtiene el los miembros de una organizacion
        /// </summary>
        /// <param name="pOrganizacionID">Clave de la organizacion</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerMiembrosOrganizacionParaFiltroGrupos(Guid pOrganizacionID)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().Where(item => item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue).Select(item => item.Perfil).Distinct().ToList();

            dataWrapper.ListaIdentidad = mEntityContext.Perfil.JoinIdentidad().Where(item => item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue).Select(item => item.Identidad).Distinct().ToList();

            dataWrapper.ListaGrupoIdentidades = mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesOrganizacion().Where(item => item.GrupoIdentidadesOrganizacion.OrganizacionID.Equals(pOrganizacionID) && !item.GrupoIdentidades.FechaBaja.HasValue).Select(item => item.GrupoIdentidades).ToList();

            dataWrapper.ListaGrupoIdentidadesParticipacion = mEntityContext.GrupoIdentidadesParticipacion.JoinGrupoIdentidadesOrganizacion().Where(item => item.GrupoIdentidadesOrganizacion.OrganizacionID.Equals(pOrganizacionID) && !item.GrupoIdentidadesParticipacion.FechaBaja.HasValue).Select(item => item.GrupoIdentidadesParticipacion).ToList();

            return dataWrapper;
        }

        public Perfil ObtenerMiembrosComunidadFiltro(Guid pProyectoID, string pFiltro, Guid pIdentidadOmisionID)
        {
            if (pIdentidadOmisionID != Guid.Empty)
            {
                return mEntityContext.Perfil.JoinIdentidad().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue && !item.Identidad.Tipo.Equals((short)TiposIdentidad.ProfesionalCorporativo) && (item.Perfil.NombrePerfil.StartsWith(pFiltro) || item.Perfil.NombrePerfil.Contains(pFiltro)) && !item.Identidad.IdentidadID.Equals(pIdentidadOmisionID)).Select(item => item.Perfil).FirstOrDefault();
            }
            else
            {
                return mEntityContext.Perfil.JoinIdentidad().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue && !item.Identidad.Tipo.Equals((short)TiposIdentidad.ProfesionalCorporativo) && (item.Perfil.NombrePerfil.StartsWith(pFiltro) || item.Perfil.NombrePerfil.Contains(pFiltro))).Select(item => item.Perfil).FirstOrDefault();
            }
        }

        public GrupoIdentidades ObtenerGruposComunidadFiltro(Guid pProyectoID, string pFiltro, Guid pIdentidadParticipanteID)
        {
            List<Guid> listaGrupoID = mEntityContext.GrupoIdentidadesParticipacion.Where(item => item.IdentidadID.Equals(pIdentidadParticipanteID) && !item.FechaBaja.HasValue).Select(item => item.GrupoID).ToList();

            if (pIdentidadParticipanteID != Guid.Empty)
            {
                return mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesProyecto().Where(item => item.GrupoIdentidadesProyecto.ProyectoID.Equals(pProyectoID) && !item.GrupoIdentidades.FechaBaja.HasValue && (item.GrupoIdentidades.Nombre.StartsWith(pFiltro) || item.GrupoIdentidades.Nombre.Contains(pFiltro)) && item.GrupoIdentidades.Publico.Equals(1) && listaGrupoID.Contains(item.GrupoIdentidades.GrupoID)).Select(item => item.GrupoIdentidades).FirstOrDefault();
            }
            else
            {
                return mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesProyecto().Where(item => item.GrupoIdentidadesProyecto.ProyectoID.Equals(pProyectoID) && !item.GrupoIdentidades.FechaBaja.HasValue && (item.GrupoIdentidades.Nombre.StartsWith(pFiltro) || item.GrupoIdentidades.Nombre.Contains(pFiltro))).Select(item => item.GrupoIdentidades).FirstOrDefault();
            }
        }

        /// <summary>
        /// Obtiene los mismbros de gnoss visibles.
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerMiembrosGnossVisibles()
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().JoinPersona().Where(item => item.Persona.EsBuscable.Equals(true) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue && !item.Identidad.Tipo.Equals((short)TiposIdentidad.ProfesionalCorporativo)).Select(item => item.Perfil).ToList();

            dataWrapper.ListaIdentidad = mEntityContext.Perfil.JoinIdentidad().JoinPersona().Where(item => item.Persona.EsBuscable.Equals(true) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue && !item.Identidad.Tipo.Equals((short)TiposIdentidad.ProfesionalCorporativo)).Select(item => item.Identidad).ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene los grupos a los que la identidad tiene permisos para hacer envios
        /// </summary>
        /// <param name="pIdentidadID">Clave de la identidad</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerGruposEnvios(Guid pIdentidadID)
        {
            //Grupos a los que pertenece la identidad
            var subconsulta1QueryGrupoIdentidades = mEntityContext.Identidad.Where(item => item.IdentidadID.Equals(pIdentidadID)).Select(item => item.PerfilID).ToList();
            var query1GrupoIdentidadesSinSelect = mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesParticipacion().JoinIdentidad().JoinProyecto().Where(item => subconsulta1QueryGrupoIdentidades.Contains(item.Identidad.PerfilID) && item.GrupoIdentidades.PermitirEnviarMensajes.Equals(true)).ToList();
            var query1GrupoIdentidades = query1GrupoIdentidadesSinSelect.Select(item => item.GrupoIdentidades);
            var query1GrupoIdentidadesEnvio = query1GrupoIdentidadesSinSelect.Select(item => new GrupoIdentidadesEnvio
            {
                GrupoID = item.GrupoIdentidades.GrupoID,
                Descripcion = item.GrupoIdentidades.Descripcion,
                FechaAlta = item.GrupoIdentidades.FechaAlta,
                FechaBaja = item.GrupoIdentidades.FechaBaja,
                NombreCorto = item.GrupoIdentidades.NombreCorto,
                PermitirEnviarMensajes = item.GrupoIdentidades.PermitirEnviarMensajes,
                Publico = item.GrupoIdentidades.Publico,
                Tags = item.GrupoIdentidades.Tags,
                Nombre = item.GrupoIdentidades.Nombre,
                NombreBusqueda = item.GrupoIdentidades.Nombre + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + item.Proyecto.NombreCorto
            });

            //Grupos publicos de las comunidades privadas a las que pertenezco
            var subconsulta2QueryGrupoIdentidades = mEntityContext.Identidad.Where(item => item.IdentidadID.Equals(pIdentidadID)).Select(item => item.PerfilID);
            var query2GrupoIdentidadesSinSelect = mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesProyecto().JoinIdentidad().JoinProyecto().Where(item => item.GrupoIdentidades.Publico.Equals(true) && subconsulta2QueryGrupoIdentidades.Contains(item.Identidad.PerfilID) && (item.Proyecto.TipoAcceso.Equals((short)TipoAcceso.Privado) || item.Proyecto.TipoAcceso.Equals((short)TipoAcceso.Reservado)) && item.GrupoIdentidades.PermitirEnviarMensajes.Equals(true)).ToList();
            var query2GrupoIdentidades = query2GrupoIdentidadesSinSelect.Select(item => item.GrupoIdentidades).ToList();
            var query2GrupoIdentidadesEnvio = query2GrupoIdentidadesSinSelect.ToList().Select(item => new GrupoIdentidadesEnvio
            {
                GrupoID = item.GrupoIdentidades.GrupoID,
                Descripcion = item.GrupoIdentidades.Descripcion,
                FechaAlta = item.GrupoIdentidades.FechaAlta,
                FechaBaja = item.GrupoIdentidades.FechaBaja,
                NombreCorto = item.GrupoIdentidades.NombreCorto,
                PermitirEnviarMensajes = item.GrupoIdentidades.PermitirEnviarMensajes,
                Publico = item.GrupoIdentidades.Publico,
                Tags = item.GrupoIdentidades.Tags,
                Nombre = item.GrupoIdentidades.Nombre,
                NombreBusqueda = item.GrupoIdentidades.Nombre + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + item.Proyecto.NombreCorto
            });

            //Grupos de las comunidades de las que soy administrador
            var subconsulta3QueryGrupoIdentidades = mEntityContext.Identidad.Where(item => item.IdentidadID.Equals(pIdentidadID)).Select(item => item.PerfilID);
            var query3GrupoIdentidadesSinSelect = mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesProyecto().JoinIdentidad().JoinPerfil().JoinPersona().JoinUsuario().JoinAdministradorProyecto().JoinProyecto().Where(item => subconsulta3QueryGrupoIdentidades.Contains(item.Identidad.PerfilID) && item.AdministradorProyecto.Tipo.Equals(0) && item.GrupoIdentidades.PermitirEnviarMensajes.Equals(true));
            var query3GrupoIdentidades = query3GrupoIdentidadesSinSelect.Select(item => item.GrupoIdentidades).ToList();
            var query3GrupoIdentidadesEnvio = query3GrupoIdentidadesSinSelect.ToList().Select(item => new GrupoIdentidadesEnvio
            {
                GrupoID = item.GrupoIdentidades.GrupoID,
                Descripcion = item.GrupoIdentidades.Descripcion,
                FechaAlta = item.GrupoIdentidades.FechaAlta,
                FechaBaja = item.GrupoIdentidades.FechaBaja,
                NombreCorto = item.GrupoIdentidades.NombreCorto,
                PermitirEnviarMensajes = item.GrupoIdentidades.PermitirEnviarMensajes,
                Publico = item.GrupoIdentidades.Publico,
                Tags = item.GrupoIdentidades.Tags,
                Nombre = item.GrupoIdentidades.Nombre,
                NombreBusqueda = item.GrupoIdentidades.Nombre + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + item.Proyecto.NombreCorto
            });

            //Grupos de las comunidades de las que soy supervisor y hay permisos
            var subconsulta4QueryGrupoIdentidades = mEntityContext.Identidad.Where(item => item.IdentidadID.Equals(pIdentidadID)).Select(item => item.PerfilID);
            var query4GrupoIdentidadesSinSelect = mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesProyecto().JoinIdentidad().JoinPerfil().JoinPersona().JoinUsuario().JoinAdministradorProyecto().JoinProyecto().JoinParametroGeneral().Where(item => subconsulta4QueryGrupoIdentidades.Contains(item.Identidad.PerfilID) && item.AdministradorProyecto.Tipo.Equals(1) && item.ParametroGeneral.SupervisoresAdminGrupos.Equals(true) && item.GrupoIdentidades.PermitirEnviarMensajes.Equals(true));
            var query4GrupoIdentidades = query4GrupoIdentidadesSinSelect.Select(item => item.GrupoIdentidades).ToList();
            var query4GrupoIdentidadesEnvio = query4GrupoIdentidadesSinSelect.ToList().Select(item => new GrupoIdentidadesEnvio
            {
                GrupoID = item.GrupoIdentidades.GrupoID,
                Descripcion = item.GrupoIdentidades.Descripcion,
                FechaAlta = item.GrupoIdentidades.FechaAlta,
                FechaBaja = item.GrupoIdentidades.FechaBaja,
                NombreCorto = item.GrupoIdentidades.NombreCorto,
                PermitirEnviarMensajes = item.GrupoIdentidades.PermitirEnviarMensajes,
                Publico = item.GrupoIdentidades.Publico,
                Tags = item.GrupoIdentidades.Tags,
                Nombre = item.GrupoIdentidades.Nombre,
                NombreBusqueda = item.GrupoIdentidades.Nombre + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + item.Proyecto.NombreCorto
            });

            var finalQuery = query1GrupoIdentidades.Union(query2GrupoIdentidades).Union(query3GrupoIdentidades).Union(query4GrupoIdentidades).ToList();
            var finalQueryEnvio = query1GrupoIdentidadesEnvio.Union(query2GrupoIdentidadesEnvio).Union(query3GrupoIdentidadesEnvio).Union(query4GrupoIdentidadesEnvio);

            DataWrapperIdentidad identidadDW = new DataWrapperIdentidad();
            identidadDW.ListaGrupoIdentidadesEnvio = finalQueryEnvio.ToList();
            identidadDW.ListaGrupoIdentidades = finalQuery.ToList();

            List<Guid> listaGrupoIdentidadesEnvioGrupoID = identidadDW.ListaGrupoIdentidadesEnvio.Select(item => item.GrupoID).ToList();

            //GrupoIdentidadesProyecto
            identidadDW.ListaGrupoIdentidadesProyecto = mEntityContext.GrupoIdentidadesProyecto.Where(item => listaGrupoIdentidadesEnvioGrupoID.Contains(item.GrupoID)).ToList();

            return identidadDW;
        }

        /// <summary>
        /// Obtiene los perfiles de las organizaci�nes pasada por par�metro 
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci�n</param>
        /// <returns>Dataset de identidades</returns>
        public List<Perfil> ObtenerPerfilesDeOrganizaciones(List<Guid> pOrganizacionesIDs)
        {
            List<Perfil> listaPerfiles = mEntityContext.PerfilOrganizacion.JoinPerfil().Where(item => pOrganizacionesIDs.Contains(item.PerfilOrganizacion.OrganizacionID)).Select(item => item.Perfil).ToList();

            return listaPerfiles;
        }
        /// <summary>
        /// Obtiene los perfiles de la organizaci�n pasada por par�metro 
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci�n</param>
        /// <returns>Dataset de identidades</returns>
        public List<Perfil> ObtenerPerfilesDeUnaOrganizacion(Guid organizacionID)
        {

            List<Perfil> listaPerfiles = mEntityContext.Perfil.Where(item => item.OrganizacionID.Equals(organizacionID)).ToList();

            return listaPerfiles;
        }

        /// <summary>
        /// Comprueba si existe el perfil personal de la persona pasada por par�metro
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>TRUE si existe el perfil personal, FALSE en caso contrario</returns>
        public bool ExistePerfilPersonal(Guid pPersonaID)
        {
            return mEntityContext.PerfilPersona.Any(item => item.PersonaID.Equals(pPersonaID));
        }

        /// <summary>
        /// Comprueba si existe el perfil de organizaci�n de la persona pasada por par�metro
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <param name="pOrganizacionID">Identificador de la organizaci�n</param>
        /// <returns>TRUE si existe el perfil de organizaci�n, FALSE en caso contrario</returns>
        public bool ExistePerfilPersonaOrg(Guid pPersonaID, Guid pOrganizacionID)
        {
            return mEntityContext.PerfilPersonaOrg.Any(item => item.PersonaID.Equals(pPersonaID) && item.OrganizacionID.Equals(pOrganizacionID));
        }

        /// <summary>
        /// Obtiene una lista con los identidades de los participantes del grupo
        /// </summary>
        /// <param name="pGrupoID">Identificador del grupo</param>
        /// <returns></returns>
        public List<Guid> ObtenerParticipantesGrupo(Guid pGrupoID)
        {
            List<Guid> listaIdentidades = new List<Guid>();

            List<Guid> listaPerfilID = mEntityContext.GrupoIdentidadesParticipacion.JoinIdentidad().Where(item => item.GrupoIdentidadesParticipacion.GrupoID.Equals(pGrupoID)).Select(item => item.Identidad.PerfilID).ToList();

            var resultado = mEntityContext.Identidad.Where(item => listaPerfilID.Contains(item.PerfilID) && item.ProyectoID.Equals(ProyectoAD.MetaProyecto));

            foreach (var filaParticipante in resultado)
            {
                listaIdentidades.Add((Guid)filaParticipante.IdentidadID);
            }
            return listaIdentidades;
        }

        /// <summary>
        /// Obtiene una lista con los IDs de los participantes del gurpo
        /// </summary>
        /// <param name="pGrupoID">Identificador del grupo</param>
        /// <returns></returns>
        public List<Guid> ObtenerPerfilesParticipantesGrupo(Guid pGrupoID)
        {
            List<Guid> listaPerfiles = new List<Guid>();

            var resultado = mEntityContext.GrupoIdentidadesParticipacion.JoinIdentidad().Where(item => item.GrupoIdentidadesParticipacion.GrupoID.Equals(pGrupoID)).Select(item => item.Identidad);

            foreach (var filaParticipante in resultado)
            {
                listaPerfiles.Add((Guid)filaParticipante.PerfilID);
            }
            return listaPerfiles;
        }

        /// <summary>
        /// Obtiene los grupos de una identidad
        /// </summary>
        /// <param name="pIdentidadID">Identidad de la que queremos obtener los grupos</param>
        /// <returns>DataSet con el grupo</returns>
        public DataWrapperIdentidad ObtenerGruposParticipaIdentidad(Guid pIdentidadID, bool pCargarPrivados)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            List<Guid> listaGrupoID = new List<Guid>();

            var query = mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesParticipacion().Where(item => item.GrupoIdentidadesParticipacion.IdentidadID.Equals(pIdentidadID)).Select(item => item.GrupoIdentidades).ToList();

            if (!pCargarPrivados)
            {
                query = query.Where(item => item.Publico).ToList();
            }
            dataWrapper.ListaGrupoIdentidades = query;

            if (query.Count > 0)
            {
                foreach (var filaGrupo in query)
                {
                    listaGrupoID.Add(filaGrupo.GrupoID);
                }

                dataWrapper.ListaGrupoIdentidadesParticipacion = mEntityContext.GrupoIdentidadesParticipacion.Where(item => listaGrupoID.Contains(item.GrupoID)).ToList();

                dataWrapper.ListaGrupoIdentidadesProyecto = mEntityContext.GrupoIdentidadesProyecto.Where(item => listaGrupoID.Contains(item.GrupoID)).ToList();

                dataWrapper.ListaGrupoIdentidadesOrganizacion = mEntityContext.GrupoIdentidadesOrganizacion.Where(item => listaGrupoID.Contains(item.GrupoID)).ToList();
            }

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene los IDs de los grupos de un perfil y su nombre corto
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad del usuario</param>
        /// <param name="pIdentidadMyGnossID">Identificador de la identidad del usuario en mygnoss</param>
        /// <returns>Lista con IDs de grupo</returns>
        public Dictionary<Guid, string> ObtenerGruposIDParticipaPerfil(Guid pIdentidadID, Guid pIdentidadMyGnossID)
        {
            Dictionary<Guid, string> grupos = new Dictionary<Guid, string>();

            var resultado = mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesParticipacion().Where(item => item.GrupoIdentidadesParticipacion.IdentidadID.Equals(pIdentidadID) || item.GrupoIdentidadesParticipacion.IdentidadID.Equals(pIdentidadMyGnossID)).Select(item => new { item.GrupoIdentidadesParticipacion.GrupoID, item.GrupoIdentidades.NombreCorto }).ToList();

            foreach (var item in resultado)
            {
                if (!grupos.ContainsKey(item.GrupoID))
                {
                    grupos.Add(item.GrupoID, item.NombreCorto);
                }
            }
            return grupos;
        }

        /// <summary>
        /// Devuelve si la identidad pertenece a algun grupo
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <param name="pPerfilID"></param>
        /// <returns></returns>
        public bool TieneIdentidadGrupos(Guid pProyectoID, Guid pPerfilID)
        {
            return mEntityContext.GrupoIdentidadesParticipacion.JoinIdentidad().Any(item => item.Identidad.PerfilID.Equals(pPerfilID) && (item.Identidad.ProyectoID.Equals(pProyectoID) || item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto)));
        }

        /// <summary>
        /// Devuelve si la identidad pertenece a algun grupo en un proyecto
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <param name="pPerfilID"></param>
        /// <returns></returns>
        public bool TieneIdentidadGrupos(Guid pIdentidadID)
        {
            return mEntityContext.GrupoIdentidadesParticipacion.Any(item => item.IdentidadID.Equals(pIdentidadID));
        }

        /// <summary>
        /// Devuelve si la identidad pertenece a algun grupo con recursos privados
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <param name="pPerfilID"></param>
        /// <returns></returns>
        public bool TieneIdentidadGruposConRecursosPrivados(Guid pProyectoID, Guid pPerfilID)
        {
            var consulta = mEntityContext.GrupoIdentidadesParticipacion.JoinIdentidad().JoinDocumentoRolGrupoIdentidades().JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().JoinDocumento().Where(item => item.Identidad.PerfilID.Equals(pPerfilID) && (item.Identidad.ProyectoID.Equals(pProyectoID) || item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto)) && item.DocumentoWebVinBaseRecursos.PrivadoEditores && item.Documento.Eliminado == false && item.DocumentoWebVinBaseRecursos.Eliminado == false && item.Documento.UltimaVersion && item.Documento.Borrador == false && item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID));
            return consulta.Any();
        }

        /// <summary>
        /// Suma uno a uno de los contadores del usuario
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pContador">String del contador a actualizar. Se pueden usar las constantes definidas en UsuarioAD: CONTADOR_NUMERO_ACCESOS, CONTADOR_NUMERO_DESCARGAS, CONTADOR_NUMERO_VISITAS</param>
        public void ActualizarContadorIdentidad(Guid pIdentidadID, string pContador)
        {
            DbCommand commandsqlActualizarContadorIdentidad = ObtenerComando(string.Format("UPDATE IdentidadContadores SET {0} = {0} + 1 WHERE IdentidadID = {1}", pContador, IBD.GuidValor(pIdentidadID)));

            ActualizarBaseDeDatos(commandsqlActualizarContadorIdentidad);
        }

        /// <summary>
        /// Obtiene los contadores de una identidad
        /// </summary>
        /// <param name="pListaIdentidadesID">Lista de identificadores de identidades</param>
        /// <returns>Devuelve los contadores de la identidad en un diccionario de diccionario, en el que las claves ser�n la identidad y una de las constantes de IdentidadAD: CONTADOR_NUMERO_VISITAS, CONTADOR_NUMERO_DESCARGAS; y el valor ser� el contador de la identidad</returns>
        public Dictionary<Guid, Dictionary<string, int>> ObtenerContadoresDeIdentidad(List<Guid> pListaIdentidadesID)
        {
            Dictionary<Guid, Dictionary<string, int>> resultado = new Dictionary<Guid, Dictionary<string, int>>();

            var query = mEntityContext.IdentidadContadores.Where(item => pListaIdentidadesID.Contains(item.IdentidadID));

            foreach (var item in query)
            {
                int numeroVisitas = item.NumeroVisitas;
                int numeroDescargas = item.NumeroDescargas;

                Dictionary<string, int> dicContadores = new Dictionary<string, int>();
                dicContadores.Add(IdentidadAD.CONTADOR_NUMERO_VISITAS, numeroVisitas);
                dicContadores.Add(IdentidadAD.CONTADOR_NUMERO_DESCARGAS, numeroDescargas);

                resultado.Add(item.IdentidadID, dicContadores);
            }

            return resultado;
        }



        /// <summary>
        /// Actualiza el contador de una identidad identidad
        /// </summary>
        /// <param name="pIdentidadID">Identidad a la que modificar el contador</param>
        /// <param name="pTipoDoc">Tipo de documento</param>
        /// <param name="pNombreSem">Nombre sem�ntico del documento(****.owl) en caso de que sea sem�ntico</param>
        /// <param name="pIncrementarPublicado">Incremento que hay que hacer en el n�mero de publicados (puede ser 0 o negativos)</param>
        /// <param name="pIncrementarCompartido">Incremento que hay que hacer en el n�mero de compartidos (puede ser 0 o negativos)</param>
        /// <param name="pIncrementarComentario">Incremento que hay que hacer en el n�mero de comentarios (puede ser 0 o negativos)</param>
        public void IncrementarIdentidadContadoresRecursos(Guid pIdentidadID, TiposDocumentacion pTipoDoc, string pNombreSem, int pIncrementoPublicados, int pIncrementoCompartidos, int pIncrementoComentarios)
        {
            var resultado = mEntityContext.IdentidadContadoresRecursos.FirstOrDefault(item => item.IdentidadID.Equals(pIdentidadID) && item.Tipo.Equals((short)pTipoDoc) && item.NombreSem.Equals(pNombreSem));

            if (resultado != null)
            {
                resultado.Publicados += pIncrementoPublicados;
                resultado.Compartidos += pIncrementoCompartidos;
                resultado.Comentarios += pIncrementoComentarios;
            }
            else
            {
                IdentidadContadoresRecursos fila = new IdentidadContadoresRecursos();
                fila.IdentidadID = pIdentidadID;
                fila.Tipo = (short)pTipoDoc;
                fila.NombreSem = pNombreSem;
                fila.Publicados = pIncrementoPublicados;
                fila.Compartidos = pIncrementoCompartidos;
                fila.Comentarios = pIncrementoComentarios;

                mEntityContext.IdentidadContadoresRecursos.Add(fila);
            }
            mEntityContext.SaveChanges();

        }

        /// <summary>
        /// Devuelve si la identidad pertenece a algun grupo de proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <param name="pIdentidadID">ID de identidad</param>
        /// <returns></returns>
        public bool TieneIdentidadGruposDeProyeto(Guid pProyectoID, Guid pIdentidadID)
        {
            return mEntityContext.GrupoIdentidadesParticipacion.JoinGrupoIdentidadesProyecto().Any(item => item.GrupoIdentidadesParticipacion.IdentidadID.Equals(pIdentidadID) && item.GrupoIdentidadesProyecto.ProyectoID.Equals(pProyectoID));
        }

        /// <summary>
        /// Devuelve si la identidad pertenece a alg�n grupo
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <param name="pPerfilID"></param>
        /// <returns></returns>
        public bool TienePerfilGruposConRecursosPrivadosEnComunConElPerfilPagina(Guid? pProyectoID, Guid pPerfilID, Guid pPerfilPaginaID)
        {
            if (pProyectoID.HasValue)
            {
                return mEntityContext.GrupoIdentidadesParticipacion.JoinIdentidad().JoinDocumentoRolGrupoIdentidades().JoinDocumentoWebVinBaseRecursos().JoinDocumento().Where(item => item.Identidad.PerfilID.Equals(pPerfilID) && (item.DocumentoWebVinBaseRecursos.PrivadoEditores.Equals(true) || item.Documento.PrivadoEditores.Equals(true)) && !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion.Equals(true) && item.Identidad.ProyectoID.Equals(pProyectoID.Value))
                .Select(item => item.DocumentoRolGrupoIdentidades.DocumentoID)
                .Intersect(mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinIdentidad().Where(item => item.Identidad.PerfilID.Equals(pPerfilID) && (item.DocumentoWebVinBaseRecursos.PrivadoEditores.Equals(true) || item.Documento.PrivadoEditores.Equals(true)) && !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion.Equals(true) && item.Identidad.ProyectoID.Equals(pProyectoID.Value)).Select(item => item.Documento.DocumentoID)).Any();
            }
            else
            {
                return mEntityContext.GrupoIdentidadesParticipacion.JoinIdentidad().JoinDocumentoRolGrupoIdentidades().JoinDocumentoWebVinBaseRecursos().JoinDocumento().Where(item => item.Identidad.PerfilID.Equals(pPerfilID) && (item.DocumentoWebVinBaseRecursos.PrivadoEditores.Equals(true) || item.Documento.PrivadoEditores.Equals(true)) && !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion.Equals(true))
                .Select(item => item.DocumentoRolGrupoIdentidades.DocumentoID)
                .Intersect(mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinIdentidad().Where(item => item.Identidad.PerfilID.Equals(pPerfilID) && (item.DocumentoWebVinBaseRecursos.PrivadoEditores.Equals(true) || item.Documento.PrivadoEditores.Equals(true)) && !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion.Equals(true)).Select(item => item.Documento.DocumentoID)).Any();
            }
        }

        /// <summary>
        /// Obtiene una lista con los usuarios de la comunidad que participan en algun grupo
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public List<Guid> ObtenerPerfilesProyectoParticipanEnGrupos(Guid pProyectoID)
        {
            List<Guid> listaPerfiles = new List<Guid>();

            var perfiles = mEntityContext.GrupoIdentidadesParticipacion.JoinIdentidad().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID)).Select(item => item.Identidad);

            foreach (var fila in perfiles)
            {
                Guid perfilID;
                if (Guid.TryParse(fila.PerfilID.ToString(), out perfilID) && !listaPerfiles.Contains(fila.PerfilID))
                {
                    listaPerfiles.Add(fila.PerfilID);
                }
            }

            return listaPerfiles;
        }

        /// <summary>
        /// Obtiene una lista con los usuarios de la comunidad que participan en algun grupo
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public List<Guid> ObtenerPerfilesProyectoParticipanEnGruposConRecursosPrivados(Guid pProyectoID)
        {
            List<Guid> listaProyectos = new List<Guid>() { pProyectoID, ProyectoAD.MetaProyecto };

            return mEntityContext.Identidad.JoinGrupoIdentidadesParticipacion().Where(item => listaProyectos.Contains(item.Identidad.ProyectoID)).Select(item => item.Identidad.PerfilID).ToList();
        }

        /// <summary>
        /// Obtiene un grupo por el nombre corto y la organizacion
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del grupo</param>
        /// <param name="pOrganizacion">Organizacion del Grupo</param>
        /// <returns>DataSet con el grupo</returns>
        public DataWrapperIdentidad ObtenerGrupoPorNombreCortoYOrganizacion(string pNombreCorto, Guid pOrganizacionID)
        {
            return ObtenerGrupoPorNombreCortoYOrganizacion(pNombreCorto, pOrganizacionID, true);
        }

        /// <summary>
        /// Obtiene un grupo por el nombre corto y la organizacion
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del grupo</param>
        /// <param name="pOrganizacion">Organizacion del Grupo</param>
        /// <returns>DataSet con el grupo</returns>
        public DataWrapperIdentidad ObtenerGrupoPorNombreCortoYOrganizacion(string pNombreCorto, Guid pOrganizacionID, bool pCargarIdentidades)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();
            dataWrapper.ListaGrupoIdentidades = mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesOrganizacion().Where(item => item.GrupoIdentidades.NombreCorto.Equals(pNombreCorto) && item.GrupoIdentidadesOrganizacion.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.GrupoIdentidades).ToList();

            if (dataWrapper.ListaGrupoIdentidades.Count > 0)
            {
                Guid grupoID = dataWrapper.ListaGrupoIdentidades.Select(item => item.GrupoID).FirstOrDefault();

                dataWrapper.ListaGrupoIdentidadesOrganizacion = mEntityContext.GrupoIdentidadesOrganizacion.Where(item => item.GrupoID.Equals(grupoID)).ToList();

                dataWrapper.ListaGrupoIdentidadesParticipacion = mEntityContext.GrupoIdentidadesParticipacion.Where(item => item.GrupoID.Equals(grupoID)).ToList();

                if (dataWrapper.ListaGrupoIdentidadesParticipacion.Count > 0 && pCargarIdentidades)
                {
                    List<Guid> listaParticipantes = new List<Guid>();
                    foreach (var fila in dataWrapper.ListaGrupoIdentidadesParticipacion)
                    {
                        if (!listaParticipantes.Contains(fila.IdentidadID))
                        {
                            listaParticipantes.Add(fila.IdentidadID);
                        }
                    }
                    dataWrapper.Merge(ObtenerIdentidadesPorID(listaParticipantes, false));
                }
            }
            return dataWrapper;
        }

        public DataWrapperIdentidad ObtenerGrupoPorNombreCortoYPerfilID(string pNombreCorto, Guid pPerfilID, bool pCargarIdentidades)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            dataWrapper.ListaGrupoIdentidades = mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesOrganizacion().JoinPerfilPersonaOrg().Where(item => item.PerfilPersonaOrg.PerfilID.Equals(pPerfilID) && item.GrupoIdentidades.NombreCorto.Equals(pNombreCorto)).Select(item => item.GrupoIdentidades).ToList();

            if (dataWrapper.ListaGrupoIdentidades.Count > 0)
            {
                Guid grupoID = dataWrapper.ListaGrupoIdentidades.Select(item2 => item2.GrupoID).FirstOrDefault();
                dataWrapper.ListaGrupoIdentidadesOrganizacion = mEntityContext.GrupoIdentidadesOrganizacion.Where(item => item.GrupoID.Equals(grupoID)).ToList();

                dataWrapper.ListaGrupoIdentidadesParticipacion = mEntityContext.GrupoIdentidadesParticipacion.Where(item => item.GrupoID.Equals(grupoID)).ToList();

                if (dataWrapper.ListaGrupoIdentidadesParticipacion.Count > 0 && pCargarIdentidades)
                {
                    List<Guid> listaParticipantes = new List<Guid>();
                    foreach (var fila in dataWrapper.ListaGrupoIdentidadesParticipacion)
                    {
                        if (!listaParticipantes.Contains(fila.IdentidadID))
                        {
                            listaParticipantes.Add(fila.IdentidadID);
                        }
                    }

                    dataWrapper.Merge(ObtenerIdentidadesPorID(listaParticipantes, false));
                }
            }
            return dataWrapper;
        }

        /// <summary>
        /// Obtiene un grupo por el nombre corto y la organizacion
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del grupo</param>
        /// <param name="pOrganizacion">Organizacion del Grupo</param>
        /// <returns>DataSet con el grupo</returns>
        public List<Guid> ObtenerGruposIDPorNombreCortoYOrganizacion(List<string> pNombresCortos, Guid pOrganizacionID)
        {
            List<Guid> lista = new List<Guid>();

            if (pNombresCortos.Count > 0)
            {
                var resultado = mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesOrganizacion().Where(item => item.GrupoIdentidadesOrganizacion.OrganizacionID.Equals(pOrganizacionID) && pNombresCortos.Contains(item.GrupoIdentidades.NombreCorto)).Select(item => item.GrupoIdentidades);

                foreach (var fila in resultado)
                {
                    lista.Add((Guid)fila.GrupoID);
                }
            }

            return lista;
        }


        /// <summary>
        /// Obtiene un grupo por el nombre corto y el proyecto
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del grupo</param>
        /// <param name="pProyecto">Proyecto del Grupo</param>
        /// <returns>DataSet con el grupo</returns>
        public DataWrapperIdentidad ObtenerGrupoPorNombreCortoYProyecto(string pNombreCorto, Guid pProyectoID)
        {
            return ObtenerGrupoPorNombreCortoYProyecto(pNombreCorto, pProyectoID, true);
        }

        /// <summary>
        /// Obtiene un grupo por el nombre corto y el proyecto
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del grupo</param>
        /// <param name="pProyecto">Proyecto del Grupo</param>
        /// <returns>DataSet con el grupo</returns>
        public DataWrapperIdentidad ObtenerGrupoPorNombreCortoYProyecto(string pNombreCorto, Guid pProyectoID, bool pCargarIdentidades, bool pCargarSoloActivas = false)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            dataWrapper.ListaGrupoIdentidades = mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesProyecto().Where(item => item.GrupoIdentidades.NombreCorto.Equals(pNombreCorto) && item.GrupoIdentidadesProyecto.ProyectoID.Equals(pProyectoID)).Select(item => item.GrupoIdentidades).ToList();

            if (dataWrapper.ListaGrupoIdentidades.Count > 0)
            {
                Guid grupoIdentidades = dataWrapper.ListaGrupoIdentidades.Select(item => item.GrupoID).FirstOrDefault();

                dataWrapper.ListaGrupoIdentidadesProyecto = mEntityContext.GrupoIdentidadesProyecto.Where(item => item.GrupoID.Equals(grupoIdentidades)).ToList();

                if (pCargarSoloActivas)
                {
                    Guid grupoIdentidadID = dataWrapper.ListaGrupoIdentidades.Select(item2 => item2.GrupoID).FirstOrDefault();

                    dataWrapper.ListaGrupoIdentidadesParticipacion = mEntityContext.GrupoIdentidadesParticipacion.JoinIdentidad().Where(item => item.GrupoIdentidadesParticipacion.GrupoID.Equals(grupoIdentidadID) && !item.GrupoIdentidadesParticipacion.FechaBaja.HasValue && !item.Identidad.FechaBaja.HasValue).Select(item => item.GrupoIdentidadesParticipacion).ToList();
                }
                else
                {
                    Guid grupoIdentidadID = dataWrapper.ListaGrupoIdentidades.Select(item2 => item2.GrupoID).FirstOrDefault();

                    dataWrapper.ListaGrupoIdentidadesParticipacion = mEntityContext.GrupoIdentidadesParticipacion.Where(item => item.GrupoID.Equals(grupoIdentidadID) && !item.FechaBaja.HasValue).ToList();
                }

                if (dataWrapper.ListaGrupoIdentidadesParticipacion.Count > 0 && pCargarIdentidades)
                {
                    List<Guid> listaParticipantes = new List<Guid>();
                    foreach (var fila in dataWrapper.ListaGrupoIdentidadesParticipacion)
                    {
                        if (!listaParticipantes.Contains(fila.IdentidadID))
                        {
                            listaParticipantes.Add(fila.IdentidadID);
                        }
                    }
                    int agruparParticipantes = 750;
                    for (int numParticipantesConsulta = 0; numParticipantesConsulta < listaParticipantes.Count; numParticipantesConsulta = numParticipantesConsulta + agruparParticipantes)
                    {
                        if (listaParticipantes.Count - numParticipantesConsulta < agruparParticipantes)
                        {
                            dataWrapper.Merge(ObtenerIdentidadesPorID_Grupo(listaParticipantes.GetRange(numParticipantesConsulta, listaParticipantes.Count - numParticipantesConsulta), false));
                        }
                        else
                        {
                            dataWrapper.Merge(ObtenerIdentidadesPorID_Grupo(listaParticipantes.GetRange(numParticipantesConsulta, agruparParticipantes), false));
                        }
                    }
                }
            }
            return dataWrapper;
        }

        /// <summary>
        /// Obtiene unos grupos por los nombrse cortos y el proyecto
        /// </summary>
        /// <param name="pNombreCortos">Nombres cortos de lps grupos</param>
        /// <param name="pProyecto">Proyecto del Grupo</param>
        /// <returns>Lista de IDs de grupos</returns>
        public List<Guid> ObtenerGruposIDPorNombreCortoYProyecto(List<string> pNombresCortos, Guid pProyectoID)
        {
            List<Guid> lista = new List<Guid>();

            if (pNombresCortos.Count > 0)
            {
                var resultado = mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesProyecto().Where(item => item.GrupoIdentidadesProyecto.ProyectoID.Equals(pProyectoID) && pNombresCortos.Contains(item.GrupoIdentidades.NombreCorto)).Select(item => item.GrupoIdentidades);

                foreach (var fila in resultado)
                {
                    lista.Add((Guid)fila.GrupoID);
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene los identificadores de perfil por los nombres cortos y el proyecto
        /// </summary>
        /// <param name="pNombresCortos">Nombres cortos de los perfiles</param>
        /// <param name="pProyecto">Proyecto del Grupo</param>
        /// <returns>Lista de IDs de grupos</returns>
        public List<Guid> ObtenerPerfilIDPorNombreCortoYProyecto(List<string> pNombresCortos, Guid pProyectoID)
        {
            List<Guid> lista = new List<Guid>();

            if (pNombresCortos.Count > 0)
            {
                bool selectPorIds = false;
                bool selectPorNombresCortos = false;

                var query = mEntityContext.Identidad.Select(item => item.PerfilID);
                List<Guid> listaUsuarioID = new List<Guid>();
                List<string> listaNombresCortos = new List<string>();
                foreach (string nombre in pNombresCortos)
                {
                    Guid usuarioID;
                    if (Guid.TryParse(nombre, out usuarioID))
                    {
                        selectPorIds = true;
                        listaUsuarioID.Add(usuarioID);
                    }
                    else
                    {
                        selectPorNombresCortos = true;
                        listaNombresCortos.Add(nombre);
                    }
                }

                var selectIDs = mEntityContext.Identidad.JoinPerfil().JoinUsuario().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue &&
                            listaUsuarioID.Contains(item.Usuario.UsuarioID)).Select(item => item.Identidad.PerfilID);

                var selectNombresCortos = mEntityContext.Identidad.JoinPerfil().JoinUsuario().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue &&
                            listaNombresCortos.Contains(item.Perfil.NombreCortoUsu)).Select(item => item.Identidad.PerfilID);


                if (selectPorIds && selectPorNombresCortos)
                {
                    lista = selectIDs.Concat(selectNombresCortos).ToList();
                }
                else if (selectPorIds)
                {
                    lista = selectIDs.ToList();
                }
                else if (selectPorNombresCortos)
                {
                    lista = selectNombresCortos.ToList();
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene unos grupos por los nombrse cortos
        /// </summary>
        /// <param name="pNombreCortos">Nombres cortos de lps grupos</param>
        /// <returns>Lista de IDs de grupos</returns>
        public List<Guid> ObtenerGruposIDPorNombreCorto(List<string> pNombresCortos)
        {
            List<Guid> lista = new List<Guid>();

            if (pNombresCortos.Count > 0)
            {
                var resultado = mEntityContext.GrupoIdentidades.Where(item => pNombresCortos.Contains(item.NombreCorto));

                foreach (var fila in resultado)
                {
                    lista.Add((Guid)fila.GrupoID);
                }
            }
            return lista;
        }

        /// <summary>
        /// Obtiene unos grupos por los nombrse cortos de un proyecto y de organizaciones.
        /// </summary>
        /// <param name="pNombreCortos">Nombres cortos de lps grupos</param>
        /// <param name="pNombresCortos">ID de proyecto</param>
        /// <returns>Lista de IDs de grupos</returns>
        public List<Guid> ObtenerGruposIDPorNombreCortoEnProyectoYEnOrganizacion(List<string> pNombresCortos, Guid pProyectoID)
        {
            List<Guid> lista = new List<Guid>();

            if (pNombresCortos.Count > 0)
            {
                var resultado = mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesProyecto().Where(item => item.GrupoIdentidadesProyecto.ProyectoID.Equals(pProyectoID) && pNombresCortos.Contains(item.GrupoIdentidades.NombreCorto))
                    .Select(item => item.GrupoIdentidades)
                    .Concat(mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesOrganizacion().Where(item => pNombresCortos.Contains(item.GrupoIdentidades.NombreCorto))
                    .Select(item => item.GrupoIdentidades)).ToList().Distinct().ToList();

                foreach (var fila in resultado)
                {
                    lista.Add((Guid)fila.GrupoID);
                }

            }

            return lista;
        }

        /// <summary>
        /// Comprueba si existe algun grupo en base de datos con el nombre pasado por par�metro
        /// </summary>
        /// <param name="pNombreGrupo">Nombre del grupo</param>
        /// <param name="pGrupoID">ID del grupo que se va a modificar</param>
        /// <param name="pOrganizacionID">Organizacion a la que pertenece el grupo</param>
        /// <returns>TRUE si existe</returns>
        public bool ExisteGrupoEnOrganizacionPorNombre(string pNombreGrupo, Guid pGrupoID, Guid pOrganizacionID)
        {
            return mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesOrganizacion().Any(item => item.GrupoIdentidades.Nombre.Equals(pNombreGrupo) && !item.GrupoIdentidades.GrupoID.Equals(pGrupoID) && item.GrupoIdentidadesOrganizacion.OrganizacionID.Equals(pOrganizacionID));
        }

        /// <summary>
        /// Comprueba si existe algun grupo en el proyecto con el nombre pasado por par�metro
        /// </summary>
        /// <param name="pNombreGrupo">Nombre del grupo</param>
        /// <param name="pGrupoID">GrupoID que se va a modificar</param>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <returns>TRUE si existe</returns>
        public bool ExisteGrupoEnProyectoPorNombre(string pNombreGrupo, Guid pGrupoID, Guid pProyectoID)
        {
            return mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesProyecto().Any(item => item.GrupoIdentidades.Nombre.Equals(pNombreGrupo) && !item.GrupoIdentidades.GrupoID.Equals(pGrupoID) && item.GrupoIdentidadesProyecto.ProyectoID.Equals(pProyectoID));
        }

        /// <summary>
        /// Comprueba si existe algun grupo en base de datos con el nombre corto pasado por par�metro
        /// </summary>
        /// <param name="pNombreGrupo">Nombre corto del grupo</param>
        /// <returns>TRUE si existe</returns>
        public bool ExisteGrupoEnOrganizacionPorNombreCorto(string pNombreCortoGrupo, Guid pOrganizacionID)
        {
            return mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesOrganizacion().Any(item => item.GrupoIdentidades.NombreCorto.Equals(pNombreCortoGrupo) && item.GrupoIdentidadesOrganizacion.OrganizacionID.Equals(pOrganizacionID));
        }

        /// <summary>
        /// Comprueba si existe algun grupo en el proyecto con el nombre corto pasado por par�metro
        /// </summary>
        /// <param name="pNombreGrupo">Nombre corto del grupo</param>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <returns>TRUE si existe</returns>
        public bool ExisteGrupoEnProyectoPorNombreCorto(string pNombreCortoGrupo, Guid pProyectoID)
        {
            return mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesProyecto().Any(item => item.GrupoIdentidades.NombreCorto.Equals(pNombreCortoGrupo) && item.GrupoIdentidadesProyecto.ProyectoID.Equals(pProyectoID));
        }

        /// <summary>
        /// Obtiene los grupos de la organizacion
        /// </summary>
        /// <param name="pIdentidadID">Identidad de la que queremos obtener los grupos</param>
        /// <returns>Lista con los ids de los grupos</returns>
        public List<Guid> ObtenerGruposDeOrganizacionDeIdentidad(Guid pIdentidadID)
        {
            List<Guid> listaGrupos = new List<Guid>();

            var resultado = mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesOrganizacion().JoinGrupoIdentidadesParticipacion().JoinIdentidad().JoinPerfil().JoinIdentidad2().Where(item => item.Identidad2.IdentidadID.Equals(pIdentidadID)).Select(item => item.GrupoIdentidades).ToList();

            foreach (var item in resultado)
            {
                listaGrupos.Add(item.GrupoID);
            }
            return listaGrupos;
        }

        /// <summary>
        /// Obtiene los grupos de la organizacion
        /// </summary>
        /// <param name="pOrganizacionID">Id de la Organizacion</param>
        /// <returns>DataSet con el grupo</returns>
        public DataWrapperIdentidad ObtenerGruposDeOrganizacion(Guid pOrganizacionID, bool pObtenerMiembros)
        {
            DataWrapperIdentidad identidadDW = new DataWrapperIdentidad();

            identidadDW.ListaGrupoIdentidades = mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesOrganizacion().Where(x => x.GrupoIdentidadesOrganizacion.OrganizacionID.Equals(pOrganizacionID)).Select(x => x.GrupoIdentidades).ToList();

            if (pObtenerMiembros && identidadDW.ListaGrupoIdentidades.Count > 0)
            {
                //GrupoIdentidadesOrganizacion
                identidadDW.ListaGrupoIdentidadesOrganizacion = mEntityContext.GrupoIdentidadesOrganizacion.Where(x => x.OrganizacionID.Equals(pOrganizacionID)).ToList();

                //GrupoIdentidadesParticipacion
                identidadDW.ListaGrupoIdentidadesParticipacion = mEntityContext.GrupoIdentidadesParticipacion.JoinGrupoIdentidadesOrganizacion().Where(x => x.GrupoIdentidadesOrganizacion.OrganizacionID.Equals(pOrganizacionID)).Select(x => x.GrupoIdentidadesParticipacion).ToList();
            }

            return identidadDW;
        }

        /// <summary>
        /// Obtiene los grupos por el proyecto
        /// </summary>
        /// <param name="pProyecto">Proyecto del Grupo</param>
        /// <returns>DataSet con el grupo</returns>
        public DataWrapperIdentidad ObtenerGruposDeProyecto(Guid pProyectoID, bool pObtenerMiembros)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            dataWrapper.ListaGrupoIdentidades = mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesProyecto().Where(item => item.GrupoIdentidadesProyecto.ProyectoID.Equals(pProyectoID)).Select(item => item.GrupoIdentidades).ToList();

            if (pObtenerMiembros && dataWrapper.ListaGrupoIdentidades.Any())
            {
                dataWrapper.ListaGrupoIdentidadesProyecto = mEntityContext.GrupoIdentidadesProyecto.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();

                dataWrapper.ListaGrupoIdentidadesParticipacion = mEntityContext.GrupoIdentidadesParticipacion.JoinGrupoIdentidadesProyecto().Where(item => item.GrupoIdentidadesProyecto.ProyectoID.Equals(pProyectoID)).Select(item => item.GrupoIdentidadesParticipacion).ToList();
            }

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene los grupos por el id
        /// </summary>
        /// <param name="pGruposID">IDs del Grupo</param>
        /// <returns>DataSet con el grupo</returns>
        public DataWrapperIdentidad ObtenerGruposPorIDGrupo(List<Guid> pGruposID, bool pObtenerParticipantes = true)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            dataWrapper.ListaGrupoIdentidades = mEntityContext.GrupoIdentidades.Where(item => pGruposID.Contains(item.GrupoID)).ToList();

            if (dataWrapper.ListaGrupoIdentidades.Any())
            {
                dataWrapper.ListaGrupoIdentidadesProyecto = mEntityContext.GrupoIdentidadesProyecto.Where(item => pGruposID.Contains(item.GrupoID)).ToList();

                dataWrapper.ListaGrupoIdentidadesOrganizacion = mEntityContext.GrupoIdentidadesOrganizacion.Where(item => pGruposID.Contains(item.GrupoID)).ToList();

                if (pObtenerParticipantes)
                {
                    dataWrapper.ListaGrupoIdentidadesParticipacion = mEntityContext.GrupoIdentidadesParticipacion.Where(item => pGruposID.Contains(item.GrupoID)).ToList();

                    if (dataWrapper.ListaGrupoIdentidadesParticipacion.Count > 0)
                    {
                        List<Guid> listaParticipantes = new List<Guid>();
                        foreach (var fila in dataWrapper.ListaGrupoIdentidadesParticipacion)
                        {
                            if (!listaParticipantes.Contains(fila.IdentidadID))
                            {
                                listaParticipantes.Add(fila.IdentidadID);
                            }
                        }
                        dataWrapper.Merge(ObtenerIdentidadesPorID(listaParticipantes, false));
                    }
                }
            }
            return dataWrapper;
        }

        /// <summary>
        /// Obtiene las identidades de MyGnoss de los miembros de los grupos
        /// </summary>
        /// <param name="pGruposID">IDs del Grupo</param>
        /// <returns>DataSet con el grupo</returns>
        public List<Guid> ObtenerIdentidadesDeMyGnossDeParticipantesDeGrupos(List<Guid> pGruposID)
        {
            List<Guid> listaIdentidades = new List<Guid>();

            if (pGruposID.Count > 0)
            {
                List<Guid> listaGrupos = new List<Guid>();

                foreach (Guid grupoID in pGruposID)
                {
                    listaGrupos.Add(grupoID);
                }

                var resultado = mEntityContext.GrupoIdentidadesParticipacion.JoinIdentidad().JoinIdentidad2().Where(item => listaGrupos.Contains(item.GrupoIdentidadesParticipacion.GrupoID) && item.Identidad2.ProyectoID.Equals(ProyectoAD.MyGnoss)).Select(item => item.Identidad2).Distinct();

                foreach (var fila in resultado)
                {
                    if (!listaIdentidades.Contains((Guid)fila.IdentidadID))
                    {
                        listaIdentidades.Add((Guid)fila.IdentidadID);
                    }
                }
            }

            return listaIdentidades;
        }

        /// <summary>
        /// Obtiene el id de los grupos de comunidad en los que participa la identidad
        /// </summary>
        /// <param name="pIdentidadID">Identidad que pertenece al grupo</param>
        /// <returns>DataSet con el grupo</returns>
        public List<Guid> ObtenerIDGruposDeIdentidad(Guid pIdentidadID)
        {
            List<Guid> listaGrupos = new List<Guid>();

            var resultado = mEntityContext.GrupoIdentidadesParticipacion.Where(item => item.IdentidadID.Equals(pIdentidadID)).Distinct().ToList();

            foreach (var fila in resultado)
            {
                listaGrupos.Add(fila.GrupoID);
            }
            return listaGrupos;
        }

        /// <summary>
        /// Compruweba si los frupos existen
        /// </summary>
        /// <param name="pListaGrupos">Lista de grupos para comprobar</param>
        /// <returns>Lista de grupos que sio existen</returns>
        public List<Guid> ComprobarSiIDGruposExisten(List<Guid> pListaGrupos)
        {
            List<Guid> listaGruposExisten = new List<Guid>();
            if (pListaGrupos.Count > 0)
            {
                var resultado = mEntityContext.GrupoIdentidades.Where(item => pListaGrupos.Contains(item.GrupoID)).ToList().Distinct().ToList();

                foreach (var item in resultado)
                {
                    listaGruposExisten.Add(item.GrupoID);
                }
            }

            return listaGruposExisten;
        }

        /// <summary>
        /// Obtiene a partir del identificador de perfil dado todos los posibles perfiles e identidades que tenga (eliminados o no)
        /// Carga tablas "Perfil" e "Identidad"
        /// </summary>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDePerfil(Guid pPerfilID)
        {
            return ObtenerIdentidadesDePerfil(pPerfilID, false);
        }

        /// <summary>
        /// Obtiene a partir del identificador de perfil dado todos los posibles perfiles e identidades que tenga (eliminados o no)
        /// Carga tablas "Perfil" e "Identidad"
        /// </summary>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <param name="pBajas">TRUE para obtener las identidades dadas de baja, FALSE en caso contrario</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDePerfil(Guid pPerfilID, bool pBajas)
        {
            DataWrapperIdentidad dataWrapperIdentidad = new DataWrapperIdentidad();

            //Perfil
            dataWrapperIdentidad.ListaPerfil = mEntityContext.Perfil.Where(perfil => perfil.PerfilID.Equals(pPerfilID)).ToList();

            //Identidad
            var identidades = mEntityContext.Identidad.Where(identidad => identidad.PerfilID.Equals(pPerfilID));
            if (!pBajas)
            {
                identidades.Where(identidad => !identidad.FechaBaja.HasValue && !identidad.FechaExpulsion.HasValue);
            }
            dataWrapperIdentidad.ListaIdentidad = identidades.ToList();

            return dataWrapperIdentidad;
        }

        /// <summary>
        /// Obtiene la lista de identidades que pertenecen al perfil id pasado por par�metro
        /// </summary>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <returns>Dataset de identidades</returns>
        public List<AD.EntityModel.Models.IdentidadDS.Identidad> ObtenerListaIdentidadesPorPerfilID(Guid pPerfilID)
        {
            return mEntityContext.Identidad.Where(item => item.PerfilID.Equals(pPerfilID)).ToList();
        }

        /// <summary>
        /// Obtiene la identidades de los perfiles pasados como par�metro en un determinado proyecto.
        /// Carga tablas "Perfil" , "Identidad"
        /// </summary>
        /// <param name="pPerfilesID">Lista de identificadores de perfil</param>
        /// <param name="pProyectoID">Proyecto (NULL si solo se quiere obtener la tabla perfil)</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadDePerfilEnProyecto(List<Guid> pPerfilesID, Guid? pProyectoID)
        {
            DataWrapperIdentidad dataWrapperIdentidad = new DataWrapperIdentidad();

            if (pPerfilesID.Count > 0)
            {
                //Perfil
                dataWrapperIdentidad.ListaPerfil = mEntityContext.Perfil.Where(perfil => pPerfilesID.Contains(perfil.PerfilID)).ToList();
                if (pProyectoID.HasValue)
                {
                    //Identidad
                    dataWrapperIdentidad.ListaIdentidad = mEntityContext.Identidad.Where(identidad => pPerfilesID.Contains(identidad.PerfilID) && !identidad.FechaBaja.HasValue && identidad.ProyectoID.Equals(pProyectoID.Value)).ToList();
                }
            }
            return dataWrapperIdentidad;
        }

        /// <summary>
        /// Obtiene las identidades de los perfiles pasados como par�metro(eliminados o no)
        /// Carga tablas "Perfil" e "Identidad"
        /// </summary>
        /// <param name="pPerfilesID">Lista de identificadores de perfil</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDePerfiles(List<Guid> pPerfilesID)
        {
            return ObtenerIdentidadesDePerfilesEnProyecto(pPerfilesID, Guid.Empty);
        }

        /// <summary>
        /// Obtiene las identidades de los perfiles pasados como par�metro en un determinado proyecto.
        /// Carga tablas "Perfil" , "Identidad", "PerfilPersonas" y "PerfilOrganizaciones"
        /// </summary>
        /// <param name="pPerfilesID">Lista de identificadores de perfil</param>
        /// <param name="pProyectoID">Proyecto</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDePerfilesEnProyecto(List<Guid> pPerfilesID, Guid pProyectoID)
        {
            DataWrapperIdentidad dataWrapperIdentidad = new DataWrapperIdentidad();

            if (pPerfilesID.Count > 0)
            {
                //Perfil
                dataWrapperIdentidad.ListaPerfil = mEntityContext.Perfil.Where(perfil => pPerfilesID.Contains(perfil.PerfilID)).ToList();
                //PerfilPersona
                dataWrapperIdentidad.ListaPerfilPersona = mEntityContext.PerfilPersona.Where(perfil => pPerfilesID.Contains(perfil.PerfilID)).ToList();

                //PerfilOrganizacion
                dataWrapperIdentidad.ListaPerfilOrganizacion = mEntityContext.PerfilOrganizacion.Where(perfil => pPerfilesID.Contains(perfil.PerfilID)).ToList();

                //Identidad
                if (pProyectoID != Guid.Empty)
                {
                    dataWrapperIdentidad.ListaIdentidad = mEntityContext.Identidad.Where(identidad => pPerfilesID.Contains(identidad.PerfilID) && !identidad.FechaBaja.HasValue && identidad.ProyectoID.Equals(pProyectoID)).ToList();
                }
                dataWrapperIdentidad.ListaIdentidad = mEntityContext.Identidad.Where(identidad => pPerfilesID.Contains(identidad.PerfilID)).ToList();
            }
            return dataWrapperIdentidad;
        }

        /// <summary>
        /// Obtiene las identidades de los perfiles pasados como par�metro en un determinado proyecto.
        /// Carga Lista de IDs de identidades.
        /// </summary>
        /// <param name="pPerfilesID">Lista de identificadores de perfil</param>
        /// <param name="pProyectoID">Proyecto</param>
        /// <returns>Lista de identidades</returns>
        public List<Guid> ObtenerIdentidadesIDDePerfilesEnProyecto(List<Guid> pPerfilesID, Guid pProyectoID)
        {
            List<Guid> identidadesID = new List<Guid>();

            if (pPerfilesID.Count > 0)
            {
                var resultado = mEntityContext.Identidad.Where(item => pPerfilesID.Contains(item.PerfilID)).ToList();

                if (pProyectoID != Guid.Empty)
                {
                    resultado = resultado.Where(item => !item.FechaBaja.HasValue && item.ProyectoID.Equals(pProyectoID)).ToList();
                }

                foreach (var fila in resultado)
                {
                    if (!identidadesID.Contains((Guid)fila.IdentidadID))
                    {
                        identidadesID.Add((Guid)fila.IdentidadID);
                    }
                }
            }
            return identidadesID;
        }

        /// <summary>
        /// Obtiene a partir del identificador de organizaci�n pasado por par�metro todos 
        /// los posibles perfiles e identidades que tenga (eliminados o no). 
        /// Carga tabla "Perfil" e "Identidad"
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci�n de la que se quiere obtener las personas</param>
        /// <param name="pOrganizacionProyectoID">Identificador del proyecto de la organizaci�n del que se quieren obtener los participantes</param>
        /// <param name="pProyectoID">Identidad del proyecto del que se quieren obtener los participantes</param>
        /// <param name="pTraerNombreApellidos">Indica si se deben agregar a Identidad el nombre y apellidos de cada persona</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDePersonasConOrganizacionEnProyecto(Guid pOrganizacionID, Guid pOrganizacionProyectoID, Guid pProyectoID, bool pTraerNombreApellidos)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            dataWrapper.ListaIdentidad = mEntityContext.PerfilPersonaOrg.JoinIdentidad().Where(item => item.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizacionID) && item.Identidad.OrganizacionID.Equals(pOrganizacionProyectoID) && item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue)
                .Select(item => item.Identidad).ToList();

            dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().Where(item => item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && item.Identidad.OrganizacionID.Equals(pOrganizacionProyectoID) && item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.Tipo.Equals(3) && !item.Identidad.FechaBaja.HasValue)
                .Select(item => item.Perfil).ToList();

            dataWrapper.ListaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.JoinIdentidad().Where(item => item.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizacionID) && item.Identidad.OrganizacionID.Equals(pOrganizacionProyectoID) && item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue)
                .Select(item => item.PerfilPersonaOrg).ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene todos los posibles perfiles e identidades de los miembros de una organizacion en un proyectos. Carga tabla "Perfil" e "Identidad"
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci�n de la que se quiere obtener las personas</param>
        /// <param name="pProyectoID">Identificador del proyecto del que se quieren obtener los participantes</param>
        /// <param name="pTraerDadosDeBaja">Indica si se deben traer o no las identidades dadas de baja y expulsadas</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDeMiembrosDeOrganizacionEnProyecto(Guid pOrganizacionID, Guid pProyectoID, bool pTraerDadosDeBaja)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            var listaIdentidad = mEntityContext.Perfil.JoinIdentidad();

            var listaPerfil = mEntityContext.Perfil.JoinIdentidad();

            dataWrapper.ListaIdentidad = listaIdentidad.Where(item => item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && item.Identidad.ProyectoID.Equals(pProyectoID))
                .Select(item => item.Identidad).ToList();

            dataWrapper.ListaPerfil = listaPerfil.Where(item => item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && item.Identidad.ProyectoID.Equals(pProyectoID))
                .Select(item => item.Perfil).ToList();

            if (!pTraerDadosDeBaja)
            {
                dataWrapper.ListaIdentidad = listaIdentidad.Where(item => !item.Identidad.FechaBaja.HasValue && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && item.Identidad.ProyectoID.Equals(pProyectoID)).Select(item => item.Identidad).ToList();

                dataWrapper.ListaPerfil = listaPerfil.Where(item => !item.Identidad.FechaBaja.HasValue && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && item.Identidad.ProyectoID.Equals(pProyectoID)).Select(item => item.Perfil).ToList();
            }

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene todos los gadgets de un perfil
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil del que se quieren obtener los gadgets</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerGadgetsPerfil(Guid pPerfilID)
        {
            DataWrapperIdentidad identidadDW = new DataWrapperIdentidad();

            identidadDW.ListaPerfilGadget = mEntityContext.PerfilGadget.Where(item => item.PerfilID.Equals(pPerfilID)).ToList();

            return identidadDW;
        }

        /// <summary>
        /// Obtiene todos las identidades de una organizacion visibles en un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizaci�n de la que se quiere obtener las personas</param>
        /// <param name="pProyectoID">Identificador del proyecto del que se quieren obtener los participantes</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDePersonasEnOrganizacionVisiblesEnProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();
            if (pProyectoID.Equals(ProyectoAD.MetaProyecto))
            {
                dataWrapper.ListaIdentidad = mEntityContext.Perfil.JoinIdentidad().JoinPersonaVisibleEnOrg().Where(item => item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue && item.Identidad.Tipo.Equals((short)TiposIdentidad.ProfesionalPersonal)).Select(item => item.Identidad).ToList();

                dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().JoinPersonaVisibleEnOrg().Where(item => item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.Tipo.Equals(3) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue).Select(item => item.Perfil).ToList();

                dataWrapper.ListaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.JoinIdentidad().JoinPersonaVisibleEnOrg().Where(item => item.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizacionID) && item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue).Select(item => item.PerfilPersonaOrg).ToList();
            }
            dataWrapper.ListaIdentidad = mEntityContext.Perfil.JoinIdentidad().Where(item => item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue && item.Identidad.Tipo.Equals((short)TiposIdentidad.ProfesionalPersonal)).Select(item => item.Identidad).ToList();

            dataWrapper.ListaPerfil = mEntityContext.Perfil.JoinIdentidad().Where(item => item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.Tipo.Equals(3) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue).Select(item => item.Perfil).ToList();

            dataWrapper.ListaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.JoinIdentidad().Where(item => item.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizacionID) && item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue).Select(item => item.PerfilPersonaOrg).ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene la clave de la identidad de un perfil en un proyecto concreto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <returns>IdentidadID</returns>
        public Guid? ObtenerIdentidadIDDePerfilEnProyecto(Guid pProyectoID, Guid pPerfilID)
        {
            var resultado = mEntityContext.Identidad.Where(item => item.ProyectoID.Equals(pProyectoID) && item.PerfilID.Equals(pPerfilID) && !item.FechaBaja.HasValue).Select(item => item.IdentidadID).FirstOrDefault();
            return resultado;
        }

        /// <summary>
        /// Obtiene la clave de la identidad de un perfil en un proyecto concreto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pListaPerfilID">Identificador del perfil</param>
        /// <returns>IdentidadID</returns>
        public List<Guid> ObtenerIdentidadesIDDePerfilEnProyecto(Guid pProyectoID, List<Guid> pListaPerfilID)
        {
            List<Guid> listaIDentidades = new List<Guid>();

            if (pListaPerfilID.Count > 0)
            {
                var resultado = mEntityContext.Identidad.Where(item => item.ProyectoID.Equals(pProyectoID) && !item.FechaBaja.HasValue && pListaPerfilID.Contains(item.PerfilID)).ToList();

                foreach (var item in resultado)
                {
                    listaIDentidades.Add(item.IdentidadID);
                }
            }

            return listaIDentidades;
        }

        /// <summary>
        /// Obtiene la clave de la identidad de un perfil en un proyecto concreto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pListaPerfilID">Identificador del perfil</param>
        /// <returns>IdentidadID</returns>
        public Dictionary<Guid, Guid> ObtenerIdentidadesIDyPerfilEnProyecto(Guid pProyectoID, List<Guid> pListaPerfilID)
        {
            Dictionary<Guid, Guid> listaIDentidades = new Dictionary<Guid, Guid>();

            if (pListaPerfilID.Any())
            {
                var resultado = mEntityContext.Identidad.Where(item => item.ProyectoID.Equals(pProyectoID) && !item.FechaBaja.HasValue && pListaPerfilID.Contains(item.PerfilID)).ToList();
                foreach (var item in resultado)
                {
                    listaIDentidades.Add(item.PerfilID, item.IdentidadID);
                }
            }
            return listaIDentidades;
        }

        /// <summary>
        /// Obtiene un array cuyo primer elemento es el identificador de la identidad de la persona y el segundo es el identificador del perfil de la identidad (en un proyecto)
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <returns>Identificador de la identidad</returns>
        public Guid[] ObtenerIdentidadIDDePersonaEnProyecto(Guid pProyectoID, Guid pPersonaID)
        {
            SortedDictionary<int, List<Guid[]>> diccIdent = new SortedDictionary<int, List<Guid[]>>();

            var res = mEntityContext.Perfil.JoinIdentidad().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && item.Perfil.PersonaID.HasValue && item.Perfil.PersonaID.Value.Equals(pPersonaID) && !item.Identidad.FechaBaja.HasValue).ToList();

            foreach (var item in res)
            {
                Guid[] resultado = new Guid[2];
                Guid identidadID = item.Identidad.IdentidadID;
                Guid perfilID = item.Identidad.PerfilID;
                short tipo = item.Identidad.Tipo;

                resultado[0] = identidadID;
                resultado[1] = perfilID;

                if (!diccIdent.ContainsKey(tipo))
                {
                    diccIdent.Add(tipo, new List<Guid[]>());
                }

                diccIdent[tipo].Add(resultado);
            }
            if (diccIdent.Count > 0)
            {
                return diccIdent[diccIdent.Keys.First()][0];
            }
            return null;
        }

        /// <summary>
        /// Obtiene una lista de las identidades de la persona en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pUsuariosID">Lista de identificadores de usuario</param>
        /// <param name="pSoloActivos">Indica si obtiene �nicamente las identidades activas en el proyecto</param>
        /// <returns>Identificador de la identidad</returns>
        public List<Guid> ObtenerIdentidadesIDDeusuariosEnProyecto(Guid pProyectoID, List<Guid> pUsuariosID, bool pSoloActivos)
        {
            List<Guid> resultado = new List<Guid>();

            var res = mEntityContext.Perfil.JoinIdentidad().JoinPersona().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && pUsuariosID.Contains(item.Persona.UsuarioID.Value)).ToList();

            if (pSoloActivos)
            {
                res = mEntityContext.Perfil.JoinIdentidad().JoinPersona().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && pUsuariosID.Contains(item.Persona.UsuarioID.Value) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue).ToList();
            }

            foreach (var item in res)
            {
                resultado.Add(item.Identidad.IdentidadID);
            }

            return resultado;
        }

        private class PerfilIDIdentidadID
        {
            public Guid IdentidadID { get; set; }
            public Guid PerfilID { get; set; }

        }

        /// <summary>
        /// Obtiene si el perfil esta o no eliminado
        /// </summary>
        /// <param name="pPerfilID">Perfil id a consultar</param>
        /// <returns>True o false si esta o no eliminado respectivamente</returns>
        public bool EstaPerfilEliminado(Guid pPerfil)
        {
            return mEntityContext.Perfil.Where(item => item.PerfilID.Equals(pPerfil)).Select(item => item.Eliminado).FirstOrDefault();
        }


        /// <summary>
        /// Obtiene un array cuyo primer elemento es el identificador de la identidad del usuario y el segundo es el identificador del perfil de la identidad (en un proyecto)
        /// </summary>
        /// <param name="pUsuario">Usuario</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pOrganizacion">Organizaci�n</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <param name="pObtenerEliminadas">Verdad si se deben de obtener las filas ya eliminadas</param>
        /// <returns>Identificador de la identidad</returns>
        public Guid[] ObtenerIdentidadIDDeUsuarioEnProyectoYOrg(string pUsuario, Guid pProyectoID, string pOrganizacion, Guid pOrganizacionID, bool pObtenerEliminadas)
        {
            var resultadoConsulta = new { IdentidadID = Guid.Empty, PerfilID = Guid.Empty };
            if (pProyectoID.Equals(Guid.Empty))
            {
                pProyectoID = ProyectoAD.MetaProyecto;
            }

            if (!pObtenerEliminadas)
            {

                if (string.IsNullOrEmpty(pOrganizacion))
                {
                    resultadoConsulta = mEntityContext.Perfil.JoinIdentidad().JoinPersona().JoinPerfilPersona().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && item.Perfil.NombreCortoUsu.Equals(pUsuario) && (item.Identidad.Tipo.Equals((short)TiposIdentidad.Personal) || item.Identidad.Tipo.Equals((short)TiposIdentidad.Profesor)) && !item.Identidad.FechaBaja.HasValue).Select(item => new { IdentidadID = item.Identidad.IdentidadID, PerfilID = item.Identidad.PerfilID }).FirstOrDefault();
                }
                else
                {
                    if (pOrganizacionID.Equals(Guid.Empty))
                    {
                        resultadoConsulta = mEntityContext.Perfil.JoinIdentidad().JoinPersona().JoinPerfilPersonaOrg().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && item.Perfil.NombreCortoOrg.Equals(pOrganizacion) && item.Perfil.NombreCortoUsu.Equals(pUsuario) && !item.Identidad.FechaBaja.HasValue)
                             .Select(item => new { IdentidadID = item.Identidad.IdentidadID, PerfilID = item.Identidad.PerfilID })
                             .Union(mEntityContext.Perfil.JoinIdentidad().JoinPersona().JoinPerfilPersona().JoinProfesor().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue && item.Perfil.NombreCortoUsu.Equals(pUsuario))
                             .Select(item => new { IdentidadID = item.Identidad.IdentidadID, PerfilID = item.Identidad.PerfilID })).FirstOrDefault();
                    }
                    else
                    {
                        resultadoConsulta = mEntityContext.PerfilPersonaOrg.JoinIdentidad().JoinPersona().JoinUsuario().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && item.Usuario.NombreCorto.Equals(pUsuario) && item.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizacionID) && !item.Identidad.FechaBaja.HasValue).Select(item => new { IdentidadID = item.Identidad.IdentidadID, PerfilID = item.Identidad.PerfilID }).FirstOrDefault();
                    }
                }
            }

            if (string.IsNullOrEmpty(pOrganizacion))
            {
                resultadoConsulta = mEntityContext.Perfil.JoinIdentidad().JoinPersona().JoinPerfilPersona().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && item.Perfil.NombreCortoUsu.Equals(pUsuario) && (item.Identidad.Tipo.Equals((short)TiposIdentidad.Personal) || item.Identidad.Tipo.Equals((short)TiposIdentidad.Profesor))).Select(item => new { IdentidadID = item.Identidad.IdentidadID, PerfilID = item.Identidad.PerfilID }).FirstOrDefault();
            }
            else
            {
                if (pOrganizacionID.Equals(Guid.Empty))
                {
                    resultadoConsulta = mEntityContext.Perfil.JoinIdentidad().JoinPersona().JoinPerfilPersonaOrg().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && item.Perfil.NombreCortoOrg.Equals(pOrganizacion) && item.Perfil.NombreCortoUsu.Equals(pUsuario))
                         .Select(item => new { IdentidadID = item.Identidad.IdentidadID, PerfilID = item.Identidad.PerfilID })
                         .Union(mEntityContext.Perfil.JoinIdentidad().JoinPersona().JoinPerfilPersona().JoinProfesor().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue && item.Perfil.NombreCortoUsu.Equals(pUsuario))
                         .Select(item => new { IdentidadID = item.Identidad.IdentidadID, PerfilID = item.Identidad.PerfilID })).FirstOrDefault();
                }
                else
                {
                    resultadoConsulta = mEntityContext.PerfilPersonaOrg.JoinIdentidad().JoinPersona().JoinUsuario().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && item.Usuario.NombreCorto.Equals(pUsuario) && item.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizacionID)).Select(item => new { IdentidadID = item.Identidad.IdentidadID, PerfilID = item.Identidad.PerfilID }).FirstOrDefault();
                }
            }

            Guid[] resultado = null;

            if (resultadoConsulta != null)
            {
                resultado = new Guid[2];
                Guid identidadID = resultadoConsulta.IdentidadID;
                Guid perfilID = resultadoConsulta.PerfilID;

                resultado[0] = identidadID;
                resultado[1] = perfilID;
            }
            return resultado;
        }

        /// <summary>
        /// Obtiene un diccionario con el perfil de la identidad dada, el id de la organizaci�n y el nombre corto del perfil (el de la organizaci�n o el del usuario, seg�n corresponda)
        /// </summary>
        /// <param name="pIdentidadID">Identidad que busca</param>        
        /// <param name="pObtenerNombreCortoPerfil">Verdad si se debe obtener el nombre corto del perfil</param>
        /// <returns>Obtiene un diccionario con las claves "PerfilID", "OrganizacionID", "NombreCorto"</returns>
        public Dictionary<string, object> ObtenerPerfilyOrganizacionIDyNombreCortoPerfil(Guid pIdentidadID, bool pObtenerNombreCortoPerfil)
        {
            Dictionary<string, object> listaResultados = new Dictionary<string, object>();

            var perfilObtenerNombreCorto = mEntityContext.Perfil.JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID)).Select(item => new
            {
                PerfilID = item.Identidad.PerfilID,
                OrganizacionID = item.Perfil.OrganizacionID,
                PersonaID = item.Perfil.PersonaID,
                NombreCortoOrg = item.Perfil.NombreCortoOrg,
                NombreCortoUsu = item.Perfil.NombreCortoUsu,
                Tipo = item.Identidad.Tipo
            }).ToList();


            if (perfilObtenerNombreCorto.Count > 0)
            {
                var filaPerfil = perfilObtenerNombreCorto.First();

                listaResultados.Add("PerfilID", filaPerfil.PerfilID);

                if (filaPerfil.PersonaID.HasValue)
                {
                    listaResultados.Add("PersonaID", filaPerfil.PersonaID.Value);
                }

                if (filaPerfil.OrganizacionID.HasValue)
                {
                    listaResultados.Add("OrganizacionID", filaPerfil.OrganizacionID.Value);
                }

                if (pObtenerNombreCortoPerfil)
                {
                    string nombreCorto = filaPerfil.NombreCortoUsu;
                    TiposIdentidad tipoIdentidad = (TiposIdentidad)(short)filaPerfil.Tipo;
                    if (tipoIdentidad.Equals(TiposIdentidad.Organizacion) || tipoIdentidad.Equals(TiposIdentidad.ProfesionalCorporativo))
                    {
                        nombreCorto = filaPerfil.NombreCortoOrg;
                    }
                    listaResultados.Add("NombreCorto", nombreCorto);
                }
            }

            return listaResultados;
        }

        /// <summary>
        /// Obtiene un array cuyo primer elemento es el perfil de la identidad dada y la segunda componente si existe es la identidad de la organizacion
        /// </summary>
        /// <param name="pIdentidadID">Identidad que busca</param>        
        /// <returns>Array cuyo primer elemento es el perfil de la identidad dada y la segunda componente si existe es la identidad de la organizacion</returns>
        public List<Guid> ObtenerPerfilyOrganizacionID(Guid pIdentidadID)
        {
            List<Guid> resultado = new List<Guid>();

            Dictionary<string, object> listaResultados = ObtenerPerfilyOrganizacionIDyNombreCortoPerfil(pIdentidadID, false);

            if (listaResultados != null)
            {
                if (listaResultados.ContainsKey("PerfilID"))
                {
                    resultado.Add((Guid)listaResultados["PerfilID"]);
                }
                if (listaResultados.ContainsKey("OrganizacionID"))
                {
                    resultado.Add((Guid)listaResultados["OrganizacionID"]);
                }
            }

            return resultado;
        }


        /// <summary>
        /// Obtiene el identificador de la identidad de una organizaci�n en un proyecto
        /// </summary>
        /// <param name="pOrganizacion">Nombre de organizaci�n</param> 
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Identificador de la identidad</returns>
        public Guid ObtenerIdentidadIDDeOrganizacionEnProyecto(string pOrganizacion, Guid pProyectoID)
        {
            var resultadoSql = mEntityContext.PerfilOrganizacion.JoinIdentidad().JoinOrganizacion().Where(item => item.Organizacion.NombreCorto.Equals(pOrganizacion) && item.Identidad.ProyectoID.Equals(pProyectoID)).Select(item => item.Identidad.IdentidadID);
            var resultado = resultadoSql.FirstOrDefault();
            return resultado;
        }

        /// <summary>
        /// Obtiene el identificador de la identidad de una organizaci�n en un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">ID de organizaci�n</param> 
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Identificador de la identidad</returns>
        public Guid ObtenerIdentidadIDDeOrganizacionIDEnProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            var resultado = mEntityContext.Perfil.JoinIdentidad().Where(item => item.Identidad.Tipo.Equals((short)TiposIdentidad.Organizacion) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && item.Identidad.ProyectoID.Equals(pProyectoID)).Select(item => item.Identidad.IdentidadID).FirstOrDefault();

            return resultado;
        }

        /// <summary>
        /// Obtiene las Identidades de los Administradores de la organizacion, (perfil de organizacion+persona tipo 1 , 2) en MYGNOSS. "Identidad" , "Perfil", "PerfilPersonaOrg"
        /// </summary>
        /// <param name="pOrganizacionID">Clave de la organizacion</param>
        /// <returns>IdentidadDS</returns>
        public DataWrapperIdentidad ObtenerIdentidadesConRolAdministradorDeOrganizacion(Guid pOrganizacionID)
        {
            DataWrapperIdentidad dataWrapper = new DataWrapperIdentidad();

            List<Guid> listaUsuarioIDIdentidad = mEntityContext.AdministradorOrganizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.Tipo.Equals((short)TipoAdministradoresOrganizacion.Administrador)).Select(item => item.UsuarioID).Distinct().ToList();

            dataWrapper.ListaIdentidad = mEntityContext.Perfil.JoinIdentidad().JoinPersona().JoinPerfilPersonaOrg()
                .Where(item => item.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizacionID) && !item.Identidad.FechaBaja.HasValue && !item.Perfil.Eliminado && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && listaUsuarioIDIdentidad.Contains(item.Persona.UsuarioID.Value) && item.Persona.UsuarioID.HasValue).Select(item => item.Identidad).Distinct().ToList();

            List<Guid> listaUsuarioIDPerfil = mEntityContext.AdministradorOrganizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.Tipo.Equals((short)TipoAdministradoresOrganizacion.Administrador)).Select(item => item.UsuarioID).Distinct().ToList();

            dataWrapper.ListaPerfil = mEntityContext.PerfilPersonaOrg.JoinPerfil().JoinPersona().Where(item => item.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizacionID) && !item.Perfil.Eliminado && listaUsuarioIDPerfil.Contains(item.Persona.UsuarioID.Value) && item.Persona.UsuarioID.HasValue).Select(item => item.Perfil).Distinct().ToList();

            List<Guid> listaUsuarioIDPerfilPersonaOrg = mEntityContext.AdministradorOrganizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.Tipo.Equals((short)TipoAdministradoresOrganizacion.Administrador)).Select(item => item.UsuarioID).Distinct().ToList();

            dataWrapper.ListaPerfilPersonaOrg = mEntityContext.PerfilPersonaOrg.JoinPerfil().JoinPersona().Where(item => item.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizacionID) && !item.Perfil.Eliminado && listaUsuarioIDPerfilPersonaOrg.Contains(item.Persona.UsuarioID.Value) && item.Persona.UsuarioID.HasValue).Select(item => item.PerfilPersonaOrg).Distinct().ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Comprueba si un usuario ha participado con un perfil en una comunidad 
        /// (Comprueba si hay una identidad con fecha de baja con ese perfil en un proyecto)
        /// </summary>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>TRUE si ha participado en el proyecto anteriormente con dicho perfil y abandon�/fue expulsado</returns>
        public bool HaParticipadoConPerfilEnComunidad(Guid pPerfilID, Guid pProyectoID)
        {
            return mEntityContext.Identidad.Any(item => item.PerfilID.Equals(pPerfilID) && item.ProyectoID.Equals(pProyectoID) && item.FechaBaja.HasValue);
        }

        /// <summary>
        /// Comprueba si un usuario participa con un perfil en una comunidad 
        /// (Comprueba si hay una identidad sin fecha de baja con ese perfil en un proyecto)
        /// </summary>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>TRUE si participa en el proyecto anteriormente con dicho perfil</returns>
        public bool ParticipaPerfilEnComunidad(Guid pPerfilID, Guid pProyectoID)
        {
            return mEntityContext.Identidad.Any(item => item.PerfilID.Equals(pPerfilID) && item.ProyectoID.Equals(pProyectoID) && !item.FechaBaja.HasValue);
        }

        /// <summary>
        /// Comprueba si un usuario participa con una identidad en una comunidad
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>TRUE si participa en el proyecto anteriormente con dicha identidad</returns>
        public bool ParticipaIdentidadEnComunidad(Guid pIdentidadID, Guid pProyectoID)
        {
            return mEntityContext.Identidad.JoinIdentidad().Any(item => item.IdentidadPerfil.IdentidadID.Equals(pIdentidadID) && item.IdentidadProyecto.ProyectoID.Equals(pProyectoID) && !item.IdentidadProyecto.FechaBaja.HasValue);
        }

        /// <summary>
        /// Comprueba si la persona participa en una comunidad 
        /// (Comprueba si hay una identidad sin fecha de baja de la persona en una comunidad )
        /// </summary>
        /// <param name="pPersonaID">Identificador de la Persona</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>TRUE si participa en el proyecto con alguna identidad</returns>
        public bool ParticipaPersonaEnProyectoConAlgunaIdentidad(Guid pPersonaID, Guid pProyectoID)
        {
            return mEntityContext.Perfil.JoinIdentidad().Any(item => item.Perfil.PersonaID.Value.Equals(pPersonaID) && item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue);
        }

        /// <summary>
        /// Comprueba para una identidad (a traves de perfil+proyecto) si dicha identidad tiene "FechaExpulsion"
        /// </summary>
        /// <param name="pPerfilID">Perfil de la identidad</param>
        /// <param name="pProyectoID">Proyecto de la identidad</param>
        /// <returns>True si hay Fecha de expulsion, False si no tiene O NO encontramos una identidad para ese perfil+proyecto</returns>
        public bool EstaIdentidadExpulsadaDeproyecto(Guid pPerfilID, Guid pProyectoID)
        {
            return mEntityContext.Identidad.Any(item => item.PerfilID.Equals(pPerfilID) && item.ProyectoID.Equals(pProyectoID) && item.FechaBaja.HasValue && item.FechaExpulsion.HasValue);
        }

        /// <summary>
        /// Comprueba si una identidad est� expulsada
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <returns></returns>
        public bool EstaIdentidadExpulsada(Guid pIdentidadID)
        {
            return mEntityContext.Identidad.Any(item => item.IdentidadID.Equals(pIdentidadID) && item.FechaExpulsion.HasValue);
        }


        /// <summary>
        /// Obtiene la popularidad de una identidad en un proyecto
        /// </summary>
        /// <param name="pIdentidadID">Identidad</param>
        /// <param name="pProyectoID">Proyecto</param>
        /// <returns>N�mero de recursos</returns>
        public double ObtenerPopularidadDeIdentidadEnProyecto(Guid pIdentidadID, Guid pProyectoID)
        {
            double? resultado = mEntityContext.Identidad.Where(item => item.ProyectoID.Equals(pProyectoID) && item.IdentidadID.Equals(pIdentidadID)).Select(item => item.Rank).FirstOrDefault();

            if (resultado.HasValue)
            {
                return resultado.Value;
            }
            else
            {
                return 0;
            }
        }


        /// <summary>
        /// Obtiene la popularidad maxima de las identidades en un proyecto
        /// </summary>        
        /// <param name="pProyectoID">Proyecto</param>
        /// <returns>N�mero de recursos</returns>
        public double ObtenerPopularidadMaxDeIdentidadEnProyecto(Guid pProyectoID)
        {
            double? resultado = mEntityContext.Identidad.Where(item => item.ProyectoID.Equals(pProyectoID)).Select(item => item.Rank).Max();

            if (resultado.HasValue)
            {
                return resultado.Value;
            }
            else
            {
                return 0;
            }
        }


        /// <summary>
        /// Obtiene el n�mero de recursos subidos por una identidad en un proyecto
        /// </summary>
        /// <param name="pIdentidadID">Identidad</param>
        /// <param name="pProyectoID">Proyecto</param>
        /// <returns>N�mero de recursos</returns>
        public int ObtenerNumRecursosDeIdentidadEnProyecto(Guid pIdentidadID, Guid pProyectoID)
        {
            return mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().Where(item => item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && item.DocumentoWebVinBaseRecursos.IdentidadPublicacionID.Value.Equals(pIdentidadID) && !item.Documento.Eliminado && !item.Documento.Borrador && item.Documento.UltimaVersion.Equals(true) && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.TipoEntidad.Equals((short)TipoEntidadVinculadaDocumento.Web) && !item.Documento.Tipo.Equals((short)TiposDocumentacion.Debate) && !item.Documento.Tipo.Equals((short)TiposDocumentacion.Pregunta)).Select(item => item.Documento.DocumentoID).Count();
        }

        /// <summary>
        /// Obtiene el n�mero de recursos subidos por una identidad en un proyecto
        /// </summary>
        /// <param name="pIdentidadID">Identidad</param>
        /// <param name="pProyectoID">Proyecto</param>
        /// <returns>N�mero de recursos</returns>
        public int ObtenerNumDebatesDeIdentidadEnProyecto(Guid pIdentidadID, Guid pProyectoID)
        {
            return mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().Where(item => item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && item.DocumentoWebVinBaseRecursos.IdentidadPublicacionID.HasValue && item.DocumentoWebVinBaseRecursos.IdentidadPublicacionID.Value.Equals(pIdentidadID) && !item.Documento.Eliminado && !item.Documento.Borrador && item.Documento.UltimaVersion.Equals(true) && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.Tipo.Equals((short)TiposDocumentacion.Debate)).Select(item => item.Documento.DocumentoID).Count();
        }

        /// <summary>
        /// Obtiene el n�mero de recursos subidos por las identidades corporativas de una organizacion en un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Organizacion</param>
        /// <param name="pProyectoID">Proyecto</param>
        /// <returns>N�mero de recursos</returns>
        public int ObtenerNumRecursosDeIdentidadesDeOrgEnProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            List<Guid> listaPublicacionID = mEntityContext.Perfil.JoinIdentidad().Where(item => (item.Identidad.Tipo.Equals((short)TiposIdentidad.ProfesionalPersonal) || item.Identidad.Tipo.Equals((short)TiposIdentidad.ProfesionalCorporativo)) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && item.Identidad.ProyectoID.Equals(pProyectoID)).Select(item => item.Identidad.IdentidadID).ToList();

            return mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().Where(item => item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && listaPublicacionID.Contains(item.DocumentoWebVinBaseRecursos.IdentidadPublicacionID.Value) && !item.Documento.Eliminado && !item.Documento.Borrador && item.Documento.UltimaVersion.Equals(true) && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.TipoEntidad.Equals((short)TipoEntidadVinculadaDocumento.Web) && !item.Documento.Tipo.Equals((short)TiposDocumentacion.Debate) && !item.Documento.Tipo.Equals((short)TiposDocumentacion.Pregunta)).Select(item => item.Documento.DocumentoID).Count();
        }

        /// <summary>
        /// Obtiene el n�mero de recursos subidos por las identidades corporativas de una organizacion en un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Organizacion</param>
        /// <param name="pProyectoID">Proyecto</param>
        /// <returns>N�mero de recursos</returns>
        public int ObtenerNumDebatesDeIdentidadesDeOrgEnProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            List<Guid> listaPublicacionID = mEntityContext.Perfil.JoinIdentidad().Where(item => (item.Identidad.Tipo.Equals((short)TiposIdentidad.ProfesionalPersonal) || item.Identidad.Tipo.Equals((short)TiposIdentidad.ProfesionalCorporativo)) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && item.Identidad.ProyectoID.Equals(pProyectoID)).Select(item => item.Identidad.IdentidadID).ToList();

            return mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().Where(item => item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && listaPublicacionID.Contains(item.DocumentoWebVinBaseRecursos.IdentidadPublicacionID.Value) && !item.Documento.Eliminado && !item.Documento.Borrador && item.Documento.UltimaVersion.Equals(true) && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.Tipo.Equals((short)TiposDocumentacion.Debate)).Select(item => item.Documento.DocumentoID).Count();
        }

        /// <summary>
        /// Obtiene todas las identidades en MyGNOSS de las personas que se corresponden con un nombre y apellidos, que son visibles por el usuario conectado
        /// </summary>
        /// <param name="pNombre">Nombre de la persona</param>
        /// <param name="pUsuarioID">Guid del usuario conectado</param>
        /// <param name="pIdentidadID">Guid de la identidad en MyGnoss del usuario conectado</param>
        /// <returns>Data set de personas</returns>
        public DataWrapperIdentidad BuscarIdentidadesDePersonasVisiblesMyGNOSSPorNombre(string pNombre, Guid pUsuarioID, Guid pIdentidadID)
        {
            //Todo probar
            DataWrapperIdentidad idenDW = new DataWrapperIdentidad();

            var primeraParteSubconsulta = mEntityContext.Persona.JoinPerfil().JoinIdentidad().Where(item => item.Persona.EsBuscable.Equals(true) && !item.Perfil.OrganizacionID.HasValue && item.Identidad.OrganizacionID.Equals(ProyectoAD.MetaOrganizacion) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !item.Identidad.FechaBaja.HasValue && $"{item.Persona.Nombre} {item.Persona.Apellidos}".Contains(pNombre)).Select(item => item.Persona.PersonaID).Distinct();

            List<Guid> listaSubconsultaSubconsulta2 = mEntityContext.ProyectoUsuarioIdentidad.Where(item => item.UsuarioID.Equals(pUsuarioID)).Select(item => item.ProyectoID).Distinct().ToList();

            var segundaParteSubconsulta = mEntityContext.Persona.JoinPerfil().JoinIdentidad().Where(item => item.Persona.EsBuscable.Equals(false) && !item.Perfil.OrganizacionID.HasValue && item.Identidad.OrganizacionID.Equals(ProyectoAD.MetaOrganizacion) && !item.Identidad.FechaBaja.HasValue && $"{item.Persona.Nombre} {item.Persona.Apellidos}".Contains(pNombre) && listaSubconsultaSubconsulta2.Contains(item.Identidad.ProyectoID)).Select(item => item.Persona.PersonaID).Distinct();

            List<Guid> listaPrimeraSubconsulta = mEntityContext.Persona.JoinPerfil().JoinIdentidad().JoinAmigo().Where(item => item.Amigo.IdentidadAmigoID.Equals(pIdentidadID)).Select(item => item.Persona.PersonaID).ToList();

            List<Guid> listaSegundaSubconsulta = mEntityContext.Profesor.Select(item => item.PerfilID).ToList();

            idenDW.ListaPerfil = primeraParteSubconsulta.Concat(segundaParteSubconsulta).Join(mEntityContext.Perfil, subconsulta => new { PersonaID = subconsulta }, perfil => new { PersonaID = perfil.PersonaID.Value }, (subconsulta, perfil) => new
            {
                Perfil = perfil,
                SubconsultaPersonaID = subconsulta
            }).Join(mEntityContext.Identidad, item => item.Perfil.PerfilID, identidad => identidad.PerfilID, (item, identidad) => new
            {
                Perfil = item.Perfil,
                SubconsultaPersonaID = item.SubconsultaPersonaID,
                Identidad = identidad
            }).Where(item => !item.Perfil.OrganizacionID.HasValue && item.Identidad.OrganizacionID.Equals(ProyectoAD.MetaOrganizacion) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !listaPrimeraSubconsulta.Contains(item.SubconsultaPersonaID) && !listaSegundaSubconsulta.Contains(item.Perfil.PerfilID)).Select(item => item.Perfil).ToList();

            if (idenDW.ListaPerfil.Count > 0)
            {
                idenDW.ListaIdentidad = mEntityContext.Identidad.Where(item => idenDW.ListaPerfil.Select(item2 => item2.PerfilID).Contains(item.PerfilID) && item.OrganizacionID.Equals(ProyectoAD.MetaOrganizacion) && item.ProyectoID.Equals(ProyectoAD.MetaProyecto)).ToList();
            }

            return idenDW;
        }

        /// <summary>
        /// Actualiza la puntuaci�n de las Identidades m�s activas en los �ltimos n d�as
        /// </summary>
        public void ActualizarRankingIdentidades(int pNumDias)
        {
            //Juan
            //Obtiene los datos de los ultimos numDias dias
            int numDias = pNumDias;

            //La valoraci�n de las identidades siguen los siguientes patrones
            //	20 x N� Recursos creados (En los �ltimos numDias d�as)
            //	5 x N� Comentarios realizados (En los �ltimos numDias d�as)
            //	5 x N� Recursos vinculados (En los �ltimos numDias d�as)
            //	10 x N� Recursos compartidos (En los �ltimos numDias d�as)

            #region numero de recursos

            var queryNumeroRecursos = mEntityContext.DocumentoWebVinBaseRecursos.JoinIdentidad().JoinDocumento().Where(item => item.DocumentoWebVinBaseRecursos.FechaPublicacion > DateTime.Now.AddDays(-numDias) && item.DocumentoWebVinBaseRecursos.TipoPublicacion.Equals((short)TipoPublicacion.Publicado) && item.Documento.Publico.Equals(true)).GroupBy(item => item.Identidad.IdentidadID, item2 => item2, (item, item2) => new
            {
                IdentidadID = item,
                Valoracion = item2.ToList().Count
            }).Select(item => new { item.IdentidadID, Valoracion = item.Valoracion });

            #endregion

            #region numero de comentarios

            var queryNumeroComentarios = mEntityContext.DocumentoComentario.JoinDocumento().JoinComentario().JoinIdentiad().Where(item => item.Comentario.Fecha > DateTime.Now.AddDays(-numDias)).GroupBy(item => item.Identiad.IdentidadID, item2 => item2, (item, item2) => new
            {
                IdentidadID = item,
                Valoracion = item2.ToList().Count
            }).Select(item => new { item.IdentidadID, item.Valoracion });

            #endregion


            #region numero de recursos vinculados

            var queryNumeroRecursosVinculados = mEntityContext.DocumentoVincDoc.JoinDocumento().JoinIdentidad().Where(item => item.DocumentoVincDoc.Fecha > DateTime.Now.AddDays(-numDias)).GroupBy(item => item.Identidad.IdentidadID, item2 => item2, (item, item2) => new
            {
                IdentidadID = item,
                Valoracion = item2.ToList().Count
            }).Select(item => new { item.IdentidadID, item.Valoracion });

            #endregion

            #region numero de recursos compartidos

            var queryNumeroRecursosCompartidos = mEntityContext.DocumentoWebVinBaseRecursos.JoinIdentidad().JoinDocumento().Where(item => item.DocumentoWebVinBaseRecursos.FechaPublicacion > DateTime.Now.AddDays(-numDias) && !item.DocumentoWebVinBaseRecursos.TipoPublicacion.Equals((short)TipoPublicacion.Publicado) && item.Documento.Publico.Equals(true)).GroupBy(item => item.Identidad.IdentidadID, item2 => item2, (item, item2) => new
            {
                IdentidadID = item,
                Valoracion = item2.ToList().Count
            }).Select(item => new { item.IdentidadID, item.Valoracion });

            #endregion

            var finalQuery = mEntityContext.Identidad.GroupJoin(queryNumeroRecursos, ident => ident.IdentidadID, item => item.IdentidadID, (ident, item) => new
            {
                Item = item,
                IdentidadID = ident.IdentidadID
            }).SelectMany(x => x.Item.DefaultIfEmpty(), (x, y) => new
            {
                Recursos = y,
                IdentidadID = x.IdentidadID
            }).GroupJoin(queryNumeroComentarios, item => item.IdentidadID, comentarios => comentarios.IdentidadID, (item, comentarios) => new
            {
                Item = item,
                Comentarios = comentarios
            }).SelectMany(x => x.Comentarios.DefaultIfEmpty(), (x, y) => new
            {
                Comentario = y,
                IdentidadID = x.Item.IdentidadID,
                Recursos = x.Item.Recursos
            }).GroupJoin(queryNumeroRecursosVinculados, item => item.IdentidadID, vinculados => vinculados.IdentidadID, (item, vinculados) => new
            {
                Item = item,
                Vinculados = vinculados
            }).SelectMany(x => x.Vinculados.DefaultIfEmpty(), (x, y) => new
            {
                Vinculados = y,
                IdentidadID = x.Item.IdentidadID,
                Recursos = x.Item.Recursos,
                Comentario = x.Item.Comentario
            }).GroupJoin(queryNumeroRecursosCompartidos, item => item.IdentidadID, recursosCompartidos => recursosCompartidos.IdentidadID, (item, recursosCompartidos) => new
            {
                Item = item,
                RecursosCompartidos = recursosCompartidos
            }).SelectMany(x => x.RecursosCompartidos.DefaultIfEmpty(), (x, y) => new
            {
                RecursosCompartidos = y,
                Vinculados = x.Item.Vinculados,
                IdentidadID = x.Item.IdentidadID,
                Recursos = x.Item.Recursos,
                Comentario = x.Item.Comentario
            }).GroupBy(item => item.IdentidadID, sum => sum, (item, sum) => new
            {
                Item = item,
                SumRecursos = sum.Sum(item2 => 20 * item2.Recursos.Valoracion),
                SumComentarios = sum.Sum(item2 => 5 * item2.Comentario.Valoracion),
                SumVinculados = sum.Sum(item2 => 5 * item2.Vinculados.Valoracion),
                SumCompartidos = sum.Sum(item2 => 10 * item2.RecursosCompartidos.Valoracion)
            }).Select(item => new { IdentidadID = item.Item, Valoracion = item.SumRecursos + item.SumComentarios + item.SumVinculados + item.SumCompartidos });

            var objeto = finalQuery.Join(mEntityContext.Identidad, item => item.IdentidadID, iden => iden.IdentidadID, (item, iden) => new
            {
                ValoracionIdentidad = item.Valoracion,
                Identidad = iden,
                IdentidadID = item.IdentidadID
            }).Where(item => item.IdentidadID.Equals(item.Identidad.IdentidadID)).Select(item => new { item.Identidad, item.ValoracionIdentidad }).FirstOrDefault();

            EntityModel.Models.IdentidadDS.Identidad identidad = objeto.Identidad;

            identidad.Rank = objeto.ValoracionIdentidad;

            mEntityContext.SaveChanges();

            ActualizarRankingIdentidadesOrg();
        }

        public void ActualizarRankingIdentidadesDesdeValorAbsoluto(int pNumDias)
        {
            //Juan

            List<EntityModel.Models.IdentidadDS.Identidad> listaIdentidades = mEntityContext.Identidad.ToList();

            foreach (EntityModel.Models.IdentidadDS.Identidad identidad in listaIdentidades)
            {
                identidad.Rank = identidad.ValorAbsoluto * Math.Atan(8.0 / identidad.DiasUltActualizacion);
            }

            mEntityContext.SaveChanges();

            ActualizarRankingIdentidadesOrg();
        }

        /// <summary>
        /// Actualiza la puntuaci�n de las Identidades en el momento
        /// </summary>
        public void ActualizarPopularidadIdentidades(Guid pIDIdentidad, double pNuevaAportacion)
        {
            EntityModel.Models.IdentidadDS.Identidad identidad = mEntityContext.Identidad.Where(item => item.IdentidadID.Equals(pIDIdentidad)).FirstOrDefault();

            if (pNuevaAportacion > 0)
            {
                identidad.DiasUltActualizacion = 0;
                identidad.ValorAbsoluto = identidad.Rank.Value + pNuevaAportacion;
                identidad.Rank = identidad.ValorAbsoluto;
            }
            else
            {
                if (identidad != null)
                {
                    double rank = identidad.Rank.Value;
                    if (rank + pNuevaAportacion > 0)
                    {
                        identidad.ValorAbsoluto = rank + pNuevaAportacion;
                    }
                    else
                    {
                        identidad.ValorAbsoluto = 0;
                    }
                    identidad.Rank = identidad.ValorAbsoluto;
                }
            }
            mEntityContext.SaveChanges();
            ActualizarRankingIdentidadesOrgdeIdentidad(pIDIdentidad);
        }

        /// <summary>
        /// Actualiza la puntuaci�n de las Identidades por la noche
        /// </summary>
        public List<string> ActualizarIdentidadesNoche()
        {
            List<string> listaErrores = new List<string>();

            List<Guid> listaIdentidadId = mEntityContext.Identidad.Select(item => item.IdentidadID).ToList();

            //Creamos unas listas de 500 en 500 con las idents para actualizarlos por grupos
            Dictionary<int, List<Guid>> listaIdents = new Dictionary<int, List<Guid>>();
            int i = 0;
            int j = 0;
            foreach (Guid docID in listaIdentidadId)
            {
                i++;
                if (listaIdents.ContainsKey(j))
                {
                    listaIdents[j].Add(docID);
                }
                else
                {
                    List<Guid> lista = new List<Guid>();
                    lista.Add(docID);
                    listaIdents.Add(j, lista);
                }
                if (i == 10)
                {
                    i = 0;
                    j++;
                }
            }

            foreach (int numLista in listaIdents.Keys)
            {
                if (listaIdents[numLista].Count > 0)
                {
                    try
                    {
                        List<Guid> listaIdentidades = new List<Guid>();
                        foreach (Guid idRecurso in listaIdents[numLista])
                        {
                            listaIdentidades.Add(idRecurso);
                        }

                        List<EntityModel.Models.IdentidadDS.Identidad> listaActualizar = mEntityContext.Identidad.Where(item => listaIdentidades.Contains(item.IdentidadID)).ToList();
                        foreach (EntityModel.Models.IdentidadDS.Identidad id in listaActualizar)
                        {
                            id.DiasUltActualizacion = id.DiasUltActualizacion + 1;
                            if (id.DiasUltActualizacion > 1)
                            {
                                id.Rank = id.ValorAbsoluto * Math.Exp((double)-id.DiasUltActualizacion / 16);
                            }
                        }

                        mEntityContext.SaveChanges();

                        Thread.Sleep(1 * 1000);
                    }
                    catch (Exception ex)
                    {
                        listaErrores.Add("ERROR : Actualizar Identidades noche (SI NO SE REPITE MUCHO NO ES IMPORTANTE):  Excepci�n: " + ex.ToString() + "\n\n\tTraza: " + ex.StackTrace);
                    }
                }
            }

            ActualizarRankingIdentidadesOrg();

            return listaErrores;
        }


        /// <summary>
        /// Actualiza la puntuaci�n de las Identidades m�s activas en los �ltimos n d�as
        /// </summary>
        public void ActualizarValorAbsoluto()
        {
            //La valoraci�n de las identidades siguen los siguientes patrones
            //	20 x N� Recursos creados (En los �ltimos numDias d�as)
            //	5 x N� Comentarios realizados (En los �ltimos numDias d�as)
            //	5 x N� Recursos vinculados (En los �ltimos numDias d�as)
            //	10 x N� Recursos compartidos (En los �ltimos numDias d�as)
            //	40 x N� Dafos creados (En los �ltimos numDias d�as)
            //	20 x N� Factores de Dafos creados (En los �ltimos numDias d�as)            

            #region numero de recursos

            var queryNumeroRecursos = mEntityContext.DocumentoWebVinBaseRecursos.JoinIdentidad().Where(item => item.DocumentoWebVinBaseRecursos.FechaPublicacion > DateTime.Now.AddDays(-1) && item.DocumentoWebVinBaseRecursos.TipoPublicacion.Equals((short)TipoPublicacion.Publicado)).GroupBy(item => item.Identidad.IdentidadID).Select(item => new
            {
                IdentidadID = item.Key,
                Valoracion = item.ToList().Count
            });

            #endregion

            #region numero de comentarios

            var queryNumeroComentarios = mEntityContext.DocumentoComentario.JoinDocumento().JoinComentario().JoinIdentiad().Where(item => item.Comentario.Fecha > DateTime.Now.AddDays(-1)).GroupBy(item => item.Identiad.IdentidadID).Select(item => new
            {
                IdentidadID = item.Key,
                Valoracion = item.ToList().Count
            });

            #endregion

            #region numero de recursos vinculados

            var queryNumeroRecursosVinculados = mEntityContext.DocumentoVincDoc.JoinDocumento().JoinIdentidad().Where(item => item.DocumentoVincDoc.Fecha > DateTime.Now.AddDays(-1)).GroupBy(item => item.Identidad.IdentidadID).Select(item => new
            {
                IdentidadID = item.Key,
                Valoracion = item.ToList().Count
            });

            #endregion

            #region numero de recursos compartidos

            var queryNumeroRecursosCompartidos = mEntityContext.DocumentoWebVinBaseRecursos.JoinIdentidad().Where(item => item.DocumentoWebVinBaseRecursos.FechaPublicacion > DateTime.Now.AddDays(-1) && !item.DocumentoWebVinBaseRecursos.TipoPublicacion.Equals((short)TipoPublicacion.Publicado)).GroupBy(item => item.Identidad.IdentidadID).Select(item => new
            {
                IdentidadID = item.Key,
                Valoracion = item.ToList().Count
            });

            #endregion

            #region numero de comunidades abiertas

            var queryNumeroComunidadesAbiertas = mEntityContext.Proyecto.JoinProyectoUsuarioIdentidad().JoinIdentidad().Where(item => item.Proyecto.FechaInicio > DateTime.Now.AddDays(-1)).GroupBy(item => item.Identidad.IdentidadID).Select(item => new
            {
                IdentidadID = item.Key,
                Valoracion = item.ToList().Count
            });

            #endregion

            #region numero de blogs creados

            var queryNumeroBlogsCreados = mEntityContext.Blog.JoinIdentidad().Where(item => item.Blog.Fecha > DateTime.Now.AddDays(-1)).GroupBy(item => item.Identidad.IdentidadID).Select(item => new
            {
                IdentidadID = item.Key,
                Valoracion = item.ToList().Count
            });

            #endregion

            #region numero de art�culos blogs creados

            var queryNumeroArticulosBlogsCreados = mEntityContext.EntradaBlog.JoinIdentidad().Where(item => item.EntradaBlog.FechaCreacion > DateTime.Now.AddDays(-1)).GroupBy(item => item.Identidad.IdentidadID).Select(item => new
            {
                IdentidadID = item.Key,
                Valoracion = item.ToList().Count
            });

            #endregion

            #region numero de seguidores de blogs de la identidad

            var queryNumeroSeguidoresBlogs = mEntityContext.Suscripcion.JoinSuscripcionBlog().JoinIdentidad().Where(item => item.Suscripcion.FechaSuscripcion > DateTime.Now.AddDays(-1)).GroupBy(item => item.Identidad.IdentidadID).Select(item => new
            {
                IdentidadID = item.Key,
                Valoracion = item.ToList().Count
            });

            #endregion

            #region numero de seguidores de la identidad

            var queryNumeroSeguidores = mEntityContext.Suscripcion.JoinIdentidad().Where(item => item.Suscripcion.FechaSuscripcion.Value > DateTime.Now.AddDays(-1)).GroupBy(item => item.Identidad.IdentidadID).Select(item => new
            {
                IdentidadID = item.Key,
                Valoracion = item.ToList().Count
            });

            #endregion

            var finalQuery = mEntityContext.Identidad.GroupJoin(queryNumeroRecursos, identidad => identidad.IdentidadID, item => item.IdentidadID, (identidad, item) => new
            {
                IdentidadID = identidad.IdentidadID,
                Item = item
            }).SelectMany(x => x.Item.DefaultIfEmpty(), (x, y) => new
            {
                Recursos = y,
                IdentidadID = x.IdentidadID
            }).GroupJoin(queryNumeroComentarios, item => item.IdentidadID, comentarios => comentarios.IdentidadID, (item, comentarios) => new
            {
                Item = item,
                Comentario = comentarios
            }).SelectMany(x => x.Comentario.DefaultIfEmpty(), (x, y) => new
            {
                Comentario = y,
                IdentidadID = x.Item.IdentidadID,
                Recurso = x.Item.Recursos
            }).GroupJoin(queryNumeroRecursosVinculados, item => item.IdentidadID, vinculado => vinculado.IdentidadID, (item, vinculado) => new
            {
                Item = item,
                Vinculado = vinculado
            }).SelectMany(x => x.Vinculado.DefaultIfEmpty(), (x, y) => new
            {
                IdentidadID = x.Item.IdentidadID,
                Vinculado = y,
                Comentario = x.Item.Comentario,
                Recurso = x.Item.Recurso
            }).GroupJoin(queryNumeroRecursosCompartidos, item => item.IdentidadID, compartido => compartido.IdentidadID, (item, compartido) => new
            {
                Item = item,
                Compartido = compartido
            }).SelectMany(x => x.Compartido.DefaultIfEmpty(), (x, y) => new
            {
                Compartido = y,
                IdentidadID = x.Item.IdentidadID,
                Comentario = x.Item.Comentario,
                Recurso = x.Item.Recurso,
                Vinculado = x.Item.Vinculado
            }).GroupJoin(queryNumeroComunidadesAbiertas, item => item.IdentidadID, comunidadAbierta => comunidadAbierta.IdentidadID, (item, comunidadAbierta) => new
            {
                Item = item,
                ComunidadAbierta = comunidadAbierta
            }).SelectMany(x => x.ComunidadAbierta.DefaultIfEmpty(), (x, y) => new
            {
                ComunidadAbierta = y,
                IdentidadID = x.Item.IdentidadID,
                Comentario = x.Item.Comentario,
                Compartido = x.Item.Compartido,
                Recurso = x.Item.Recurso,
                Vinculado = x.Item.Vinculado
            }).GroupJoin(queryNumeroBlogsCreados, item => item.IdentidadID, blogsCreados => blogsCreados.IdentidadID, (item, blogsCreados) => new
            {
                Item = item,
                BlogsCreados = blogsCreados
            }).SelectMany(x => x.BlogsCreados.DefaultIfEmpty(), (x, y) => new
            {
                BlogsCreados = y,
                IdentidadID = x.Item.IdentidadID,
                Comentario = x.Item.Comentario,
                Compartido = x.Item.Compartido,
                ComunidadAbierta = x.Item.ComunidadAbierta,
                Recurso = x.Item.Recurso,
                Vinculado = x.Item.Vinculado
            }).GroupJoin(queryNumeroArticulosBlogsCreados, item => item.IdentidadID, articulosBlogsCreados => articulosBlogsCreados.IdentidadID, (item, articulosBlogsCreados) => new
            {
                Item = item,
                ArticulosBlogsCreados = articulosBlogsCreados
            }).SelectMany(x => x.ArticulosBlogsCreados.DefaultIfEmpty(), (x, y) => new
            {
                ArticulosBlogsCreados = y,
                IdentidadID = x.Item.IdentidadID,
                BlogsCreados = x.Item.BlogsCreados,
                Comentario = x.Item.Comentario,
                Compartido = x.Item.Compartido,
                ComunidadAbierta = x.Item.ComunidadAbierta,
                Recurso = x.Item.Recurso,
                Vinculado = x.Item.Vinculado
            }).GroupJoin(queryNumeroSeguidoresBlogs, item => item.IdentidadID, seguidoresBlogs => seguidoresBlogs.IdentidadID, (item, seguidoresBlogs) => new
            {
                Item = item,
                SeguidoresBlogs = seguidoresBlogs
            }).SelectMany(x => x.SeguidoresBlogs.DefaultIfEmpty(), (x, y) => new
            {
                SeguidoresBlogs = y,
                IdentidadID = x.Item.IdentidadID,
                ArticulosBlogsCreados = x.Item.ArticulosBlogsCreados,
                BlogsCreados = x.Item.BlogsCreados,
                Comentario = x.Item.Comentario,
                Compartido = x.Item.Compartido,
                ComunidadAbierta = x.Item.ComunidadAbierta,
                Recurso = x.Item.Recurso,
                Vinculado = x.Item.Vinculado
            }).GroupJoin(queryNumeroSeguidores, item => item.IdentidadID, seguidores => seguidores.IdentidadID, (item, seguidores) => new
            {
                Item = item,
                Seguidores = seguidores
            }).SelectMany(x => x.Seguidores.DefaultIfEmpty(), (x, y) => new
            {
                IdentidadID = x.Item.IdentidadID,
                ArticulosBlogsCreados = x.Item.ArticulosBlogsCreados,
                BlogsCreados = x.Item.BlogsCreados,
                Comentario = x.Item.Comentario,
                Compartido = x.Item.Compartido,
                ComunidadAbierta = x.Item.ComunidadAbierta,
                Recurso = x.Item.Recurso,
                Seguidores = y,
                SeguidoresBlogs = x.Item.SeguidoresBlogs,
                Vinculado = x.Item.Vinculado
            }).GroupBy(item => item.IdentidadID, sum => sum, (item, sum) => new
            {
                Item = item,
                SumRecursos = sum.Sum(item2 => 20 * item2.Recurso.Valoracion),
                SumComentarios = sum.Sum(item2 => 5 * item2.Comentario.Valoracion),
                SumVinculados = sum.Sum(item2 => 5 * item2.Vinculado.Valoracion),
                SumCompartidos = sum.Sum(item2 => 10 * item2.Compartido.Valoracion),
                SumComunidadesAbiertas = sum.Sum(item2 => 50 * item2.ComunidadAbierta.Valoracion),
                SumBlogsCreados = sum.Sum(item2 => 30 * item2.BlogsCreados.Valoracion),
                SumArticulosBlogsCreados = sum.Sum(item2 => 30 * item2.ArticulosBlogsCreados.Valoracion),
                SumSeguidoresBlogs = sum.Sum(item2 => item2.SeguidoresBlogs.Valoracion),
                SumSeguidores = sum.Sum(item2 => item2.Seguidores.Valoracion)
            }).Select(item => new
            {
                IdentidadID = item.Item,
                Valoracion = item.SumArticulosBlogsCreados + item.SumBlogsCreados + item.SumComentarios + item.SumCompartidos + item.SumComunidadesAbiertas + item.SumRecursos + item.SumSeguidores + item.SumSeguidoresBlogs + item.SumVinculados
            });

            var queryIdentidad = finalQuery.Join(mEntityContext.Identidad, query => query.IdentidadID, identidadOriginal => identidadOriginal.IdentidadID, (query, identidadOriginal) => new
            {
                IdentidadID = query.IdentidadID,
                ValoracionIdentidad = query.Valoracion,
                Identidad = identidadOriginal
            });

            List<EntityModel.Models.IdentidadDS.Identidad> listaIdentidades = queryIdentidad.Where(item => item.ValoracionIdentidad != 0 && item.IdentidadID.Equals(item.Identidad.IdentidadID)).Select(item => item.Identidad).ToList();

            foreach (EntityModel.Models.IdentidadDS.Identidad identidad in listaIdentidades)
            {
                double ValoracionDIA = queryIdentidad.Where(item => item.IdentidadID.Equals(identidad.IdentidadID)).Select(item => item.ValoracionIdentidad).FirstOrDefault();

                identidad.ValorAbsoluto = identidad.Rank.Value + ValoracionDIA;
                if (ValoracionDIA.Equals(0))
                {
                    identidad.DiasUltActualizacion = 1;
                    identidad.Rank = identidad.ValorAbsoluto;
                }
            }

            List<EntityModel.Models.IdentidadDS.Identidad> listaIdentidades2 = queryIdentidad.Where(item => item.ValoracionIdentidad == 0 && item.IdentidadID.Equals(item.Identidad.IdentidadID)).Select(item => item.Identidad).ToList();
            foreach (EntityModel.Models.IdentidadDS.Identidad identidad2 in listaIdentidades2)
            {
                identidad2.DiasUltActualizacion = identidad2.DiasUltActualizacion + 1;
                identidad2.Rank = identidad2.ValorAbsoluto * Math.Exp((double)-identidad2.DiasUltActualizacion / 16);
            }

            var queryIdentidadPerfilID = queryIdentidad.Join(mEntityContext.Proyecto, item => item.Identidad.ProyectoID, proyecto => proyecto.ProyectoID, (item, proyecto) => new
            {
                Proyecto = proyecto,
                Item = item
            }).Where(item => item.Proyecto.TipoAcceso.Equals((short)TipoAcceso.Publico) || item.Proyecto.TipoAcceso.Equals((short)TipoAcceso.Restringido)).GroupBy(item => item.Item.Identidad.PerfilID).Select(item => new
            {
                PerfilID = item.Key,
                RankPerfilID = item.Sum(item2 => item2.Item.Identidad.Rank.Value)
            }).Where(item => item.PerfilID.Equals(item.RankPerfilID)).Select(item => new
            {
                PerfilID = item.PerfilID,
                RankPerfilID = item.RankPerfilID
            });

            List<EntityModel.Models.IdentidadDS.Identidad> listaIdentiades3 = mEntityContext.Identidad.Where(item => item.IdentidadID.Equals(queryIdentidadPerfilID.Select(item2 => item2.PerfilID))).ToList();

            foreach (EntityModel.Models.IdentidadDS.Identidad identidad in listaIdentiades3)
            {
                identidad.Rank = queryIdentidadPerfilID.Where(item => item.PerfilID.Equals(identidad.PerfilID)).Select(item => item.RankPerfilID).FirstOrDefault();
            }

            mEntityContext.SaveChanges();

            ActualizarRankingIdentidadesOrg();
        }

        /// <summary>
        /// Actualiza la puntuaci�n de las Identidades m�s activas en los �ltimos n d�as
        /// </summary>
        public void ActualizarRankingIdentidadesOrgdeIdentidad(Guid pIdentidad)
        {
            var subconsultaTemp = mEntityContext.Identidad.JoinPerfil().Where(item => item.Identidad.Tipo.Equals((short)3) && item.Identidad.IdentidadID.Equals(pIdentidad) && item.Perfil.OrganizacionID.HasValue).Select(item => new { OrganizacionID = item.Perfil.OrganizacionID.Value, item.Identidad.ProyectoID, item.Identidad.IdentidadID });

            var temp = mEntityContext.Identidad.JoinPerfil().Join(subconsultaTemp, item => new { OrganizacionID = item.Perfil.OrganizacionID.Value, item.Identidad.ProyectoID }, subconsulta => new { subconsulta.OrganizacionID, subconsulta.ProyectoID }, (item, subconsulta) => new
            {
                Item = item,
                IdentidadOrgComunidades = subconsulta
            }).Where(item => item.Item.Identidad.Rank.HasValue && item.Item.Perfil.OrganizacionID.HasValue).Distinct().GroupBy(item => item.IdentidadOrgComunidades.IdentidadID).Select(item => new
            {
                IdentidadID = item.Key,
                Rank = item.Sum(item2 => item2.Item.Identidad.Rank)
            });

            var consultaFinal = mEntityContext.Identidad.Where(item => item.Tipo.Equals((short)3) && temp.Where(item2 => item2.IdentidadID.Equals(item.IdentidadID) && (!item.Rank.HasValue || !item.Rank.Value.Equals(item2.Rank.Value))).Select(item2 => item2.IdentidadID).Contains(item.IdentidadID));

            List<EntityModel.Models.IdentidadDS.Identidad> listaIdentidades = consultaFinal.ToList();

            foreach (EntityModel.Models.IdentidadDS.Identidad identidad in listaIdentidades)
            {
                if (!identidad.Rank.HasValue)
                {
                    identidad.Rank = 0;
                }
            }

            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Actualiza la puntuaci�n de las Identidades de Organizacion
        /// </summary>
        public void ActualizarRankingIdentidadesOrg()
        {
            var subconsultaDeSubconsulta = mEntityContext.Identidad.JoinPerfil().Where(item => item.Identidad.Tipo.Equals(3)).Select(item => new
            {
                OrganizacionID = item.Perfil.OrganizacionID.Value,
                ProyectoID = item.Identidad.ProyectoID,
                IdentidadID = item.Identidad.IdentidadID
            });

            var subconsulta = mEntityContext.Identidad.JoinPerfil().Join(subconsultaDeSubconsulta, item => new { OrganizacionID = item.Perfil.OrganizacionID.Value, item.Identidad.ProyectoID }, identidadOrgComunidades => new { identidadOrgComunidades.OrganizacionID, identidadOrgComunidades.ProyectoID }, (item, identidadOrgComunidades) => new
            {
                Item = item,
                IdentidadOrgComunidades = identidadOrgComunidades
            }).Where(item => item.Item.Identidad.Rank.HasValue).GroupBy(item => item.IdentidadOrgComunidades.IdentidadID).Select(item => new
            {
                IdentidadID = item.Key,
                RankIdentidadOrganizacion = item.Sum(item2 => item2.Item.Identidad.Rank).Value
            });

            var query = mEntityContext.Identidad.Join(subconsulta, identidad => identidad.IdentidadID, rankingOrganizaciones => rankingOrganizaciones.IdentidadID, (identidad, rankingOrganizaciones) => new
            {
                Identidad = identidad,
                RankingOrganizaciones = rankingOrganizaciones
            }).Where(item => !item.RankingOrganizaciones.RankIdentidadOrganizacion.Equals(item.Identidad.Rank));

            List<EntityModel.Models.IdentidadDS.Identidad> listaIdentidades = query.Select(item => item.Identidad).ToList();

            foreach (EntityModel.Models.IdentidadDS.Identidad identidad in listaIdentidades)
            {
                identidad.Rank = query.Where(item => item.Identidad.IdentidadID.Equals(identidad.IdentidadID)).Select(item => item.RankingOrganizaciones.RankIdentidadOrganizacion).FirstOrDefault();
            }

            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Obtiene el ranking de las identidades de un determinado proyecto
        /// </summary>
        public Dictionary<Guid, double> ObtenerRankingIdentidades(Guid pProyecto)
        {
            Dictionary<Guid, double> listaRankingIdentidades = new Dictionary<Guid, double>();

            List<EntityModel.Models.IdentidadDS.Identidad> listaIdentidades = mEntityContext.Identidad.Where(item => item.ProyectoID.Equals(pProyecto)).Distinct().ToList();

            foreach (EntityModel.Models.IdentidadDS.Identidad identidad in listaIdentidades)
            {
                if (!listaRankingIdentidades.ContainsKey(identidad.IdentidadID))
                {
                    try
                    {
                        listaRankingIdentidades.Add(identidad.IdentidadID, (double)identidad.Rank);
                    }
                    catch (Exception e)
                    {
                        mLoggingService.GuardarLogError(e);
                    }
                }
            }
            return listaRankingIdentidades;
        }

        /// <summary>
        /// Obtiene los IDs de los perfiles de unas determinadas identidades.
        /// </summary>
        /// <param name="pPerfilesID">Lista de IDs de perfiles</param>
        /// <returns>Lista con IDs de los perfiles de unas determinadas identidades</returns>
        public DataWrapperIdentidad ObtenerPerfilesPorPerfilesID(List<Guid> pPerfilesID)
        {
            DataWrapperIdentidad dataWrapperIdentidad = new DataWrapperIdentidad();

            if (pPerfilesID.Count > 0)
            {
                dataWrapperIdentidad.ListaPerfil = mEntityContext.Perfil.Where(perfil => pPerfilesID.Contains(perfil.PerfilID)).ToList();
            }

            return dataWrapperIdentidad;
        }

        /// <summary>
        /// Carga el DataSet con la tabla "Perfil" de unos determinados usuario en un proyecto.
        /// </summary>
        /// <param name="pUsuarioIDs">Lista de IDs de usuarios</param>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet con la tabla "Perfil" de unos determinados usuario en un proyecto</returns>
        public Dictionary<Guid, Guid> ObtenerPerfilesIDPorUsuariosIDEnProyecto(List<Guid> pUsuarioIDs, Guid pProyectoID)
        {
            Dictionary<Guid, Guid> usuPerfID = new Dictionary<Guid, Guid>();

            if (pUsuarioIDs.Count > 0)
            {
                var resultado = mEntityContext.Perfil.JoinIdentidad().JoinPersona().Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue).Select(item => new { item.Persona.UsuarioID, item.Perfil.PerfilID }).ToList();

                resultado = resultado.Where(item => pUsuarioIDs.Contains(item.UsuarioID.Value)).ToList();

                foreach (var item in resultado)
                {
                    Guid usuarioID = item.UsuarioID.Value;
                    Guid perfilID = item.PerfilID;

                    if (!usuPerfID.ContainsKey(usuarioID))
                    {
                        usuPerfID.Add(usuarioID, perfilID);
                    }
                }
            }
            return usuPerfID;
        }

        /// <summary>
        /// Obtiene los identificadores de identidad a partir de la lista de identificadores de usuarios
        /// </summary>
        /// <param name="pUsuarioIDs">Lista de usuarios de los que se quiere obtener el identificador de la identidad en el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que se quieren obtener las identidades de los usuarios</param>
        /// <returns>Lista de IDs de identidades en el proyecto</returns>
        public Dictionary<Guid, Guid> ObtenerIdentidadesIDPorUsuariosIDEnProyecto(List<Guid> pUsuarioIDs, Guid pProyectoID)
        {
            Dictionary<Guid, Guid> diccionario = new Dictionary<Guid, Guid>();

            var resultado = mEntityContext.Perfil.JoinIdentidad().JoinPersona().Where(item => pUsuarioIDs.Contains(item.Persona.UsuarioID.Value) && item.Identidad.ProyectoID.Equals(pProyectoID) && item.Persona.UsuarioID.HasValue && !item.Identidad.FechaBaja.HasValue).Select(item => new { item.Persona.UsuarioID, item.Identidad.IdentidadID }).ToList();

            foreach (var item in resultado)
            {
                Guid usuarioID = item.UsuarioID.Value;
                Guid identidadID = item.IdentidadID;
                if (!diccionario.ContainsKey(usuarioID))
                {
                    diccionario.Add(usuarioID, identidadID);
                }
            }

            return diccionario;
        }

        /// <summary>
        /// Obtiene los IDs de los perfiles de unas determinadas identidades.
        /// </summary>
        /// <param name="pIdentidadesID">Lista de IDs de identidades</param>
        /// <returns>Lista con IDs de los perfiles de unas determinadas identidades</returns>
        public List<Guid> ObtenerPerfilesDeIdentidades(List<Guid> pIdentidadesID)
        {
            List<Guid> listaPerfiles = new List<Guid>();

            if (pIdentidadesID.Count > 0)
            {
                var resultado = mEntityContext.Identidad.Where(item => pIdentidadesID.Contains(item.IdentidadID)).ToList();

                foreach (var item in resultado)
                {
                    if (!listaPerfiles.Contains((Guid)item.PerfilID))
                    {
                        listaPerfiles.Add((Guid)item.PerfilID);
                    }
                }
            }

            return listaPerfiles;
        }

        /// <summary>
        /// Obtiene el identificador del perfil del usuario que ha publicado un recurso en una comunidad
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public Guid? ObtenerPerfilIDPublicadorRecursoEnProyecto(Guid pDocumentoID, Guid pProyectoID)
        {
            object resultado = mEntityContext.DocumentoWebVinBaseRecursos.JoinIdentidad().JoinBaseRecursosProyecto().Where(item => item.DocumentoWebVinBaseRecursos.DocumentoID.Equals(pDocumentoID) && item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID)).Select(item => item.Identidad.PerfilID).FirstOrDefault();

            if (resultado != null && resultado is Guid)
            {
                return (Guid)resultado;
            }

            return null;
        }

        /// <summary>
        /// Obtiene el identificador del perfil del usuario que ha publicado un comentario en un recurso
        /// </summary>
        /// <param name="pComentarioID">Identificador del comentario</param>
        /// <returns></returns>
        public Guid? ObtenerPerfilIDPublicadorComentarioEnRecurso(Guid pComentarioID)
        {
            object resultado = mEntityContext.Comentario.JoinIdentidad().Where(item => item.Comentario.ComentarioID.Equals(pComentarioID)).Select(item => item.Identidad.PerfilID).FirstOrDefault();

            if (resultado != null && resultado is Guid)
            {
                return (Guid)resultado;
            }

            return null;
        }

        /// <summary>
        /// Obtiene el UsuarioID a partir de la identidadID
        /// </summary>
        /// <param name="pIdentidadID">Identidad de la que se quiere obtener el UsuarioID</param>
        /// <returns>ID del usuario</returns>
        public Guid ObtenerUsuarioIDConIdentidadID(Guid pIdentidadID)
        {
            object resultado = mEntityContext.Perfil.JoinIdentidad().JoinPersona().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID)).Select(item => item.Persona.UsuarioID).FirstOrDefault();

            if (resultado != null && resultado is Guid)
            {
                return (Guid)resultado;
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Obtiene una lista de IDs de usuario a partir de una con IDs de perfiles.
        /// </summary>
        /// <param name="pPerfilesID">IDs de perfiles</param>
        /// <returns>Diccionario con ID de perfil y ID de usuario</returns>
        public Dictionary<Guid, Guid> ObtenerUsuariosIDPorPerfilID(List<Guid> pPerfilesID)
        {
            Dictionary<Guid, Guid> perfUsu = new Dictionary<Guid, Guid>();

            if (pPerfilesID.Count > 0)
            {
                var resultado = mEntityContext.Perfil.JoinPersona().Where(item => pPerfilesID.Contains(item.Perfil.PerfilID) && item.Persona.UsuarioID.HasValue).Select(item => new { item.Perfil.PerfilID, item.Persona.UsuarioID }).ToList();

                foreach (var item in resultado)
                {
                    Guid perfilID = item.PerfilID;
                    Guid usuarioID = item.UsuarioID.Value;
                    if (!perfUsu.ContainsKey(perfilID))
                    {
                        perfUsu.Add(perfilID, usuarioID);
                    }
                }
            }
            return perfUsu;
        }

        /// <summary>
        /// Obtiene los UsuarioID a partir de la lista ed identidadesID
        /// </summary>
        /// <param name="pListaIdentidadesID">Lista de identidades de las que se quiere obtener el UsuarioID</param>
        /// <returns>Lista de IDs de usuario</returns>
        public Dictionary<Guid, Guid> ObtenerListaUsuarioIDConIdentidadesID(List<Guid> pListaIdentidadesID)
        {
            Dictionary<Guid, Guid> diccionario = new Dictionary<Guid, Guid>();

            var resultado = mEntityContext.Perfil.JoinIdentidad().JoinPersona().Where(item => pListaIdentidadesID.Contains(item.Identidad.IdentidadID) && item.Persona.UsuarioID.HasValue).Select(item => new { item.Identidad.IdentidadID, item.Persona.UsuarioID }).ToList();

            foreach (var item in resultado)
            {
                Guid identidadID = item.IdentidadID;
                Guid usuarioID = item.UsuarioID.Value;
                if (!diccionario.ContainsKey(identidadID))
                {
                    diccionario.Add(identidadID, usuarioID);
                }
            }
            return diccionario;
        }

        /// <summary>
        /// Obtiene la organizaci�n ID a partir de la identidadID
        /// </summary>
        /// <param name="pIdentidadID">Identidad de la que se quiere obtener la OrganizacionID</param>
        /// <returns>ID de la organizaci�n</returns>
        public Guid? ObtenerOrganizacionIDConIdentidadID(Guid pIdentidadID)
        {
            object resultado = mEntityContext.Perfil.JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID)).Select(item => item.Perfil.OrganizacionID).FirstOrDefault();

            if (resultado != null && resultado is Guid)
            {
                return (Guid)resultado;
            }

            return null;
        }

        /// <summary>
        /// Obtiene la organizaci�n ID a partir del perfilID
        /// </summary>
        /// <param name="pPerfilID">Perfil de la que se quiere obtener la OrganizacionID</param>
        /// <returns>ID de la organizaci�n</returns>
        public Guid? ObtenerOrganizacionIDConPerfilID(Guid pPerfilID)
        {
            object resultado = mEntityContext.Perfil.Where(item => item.PerfilID.Equals(pPerfilID)).Select(item => item.OrganizacionID).FirstOrDefault();

            if (resultado != null && resultado is Guid)
            {
                return (Guid)resultado;
            }

            return null;
        }

        /// <summary>
        /// Obtiene el perfil personal a partir de un perfil.
        /// </summary>
        /// <param name="pPerfilID">ID de perfil</param>
        /// <returns>ID del perfil personal a partir de un perfil</returns>
        public Guid ObtenerPerfilPersonalDePerfil(Guid pPerfilID)
        {
            var resultado = mEntityContext.Perfil.JoinIdentidad().JoinPerfil2().Where(item => item.Identidad.Tipo.Equals((short)TiposIdentidad.Personal) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && item.Perfil2.PerfilID.Equals(pPerfilID)).Select(item => item.Identidad.PerfilID).FirstOrDefault();

            Guid perfilID = (Guid)resultado;
            return perfilID;
        }

        public DataWrapperIdentidad ObtenerPerfilesValidosGnoss(DateTime? pFecha = null)
        {
            DataWrapperIdentidad idenDW = new DataWrapperIdentidad();

            var query = mEntityContext.Usuario.JoinPersona().JoinPerfil().JoinIdentidad().JoinNotificacionCorreoPersona().JoinNotificacion().Where(item => item.Identidad.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111")));

            if (pFecha.HasValue)
            {
                query = query.Where(item => item.Notificacion.FechaNotificacion >= pFecha.Value);
            }

            idenDW.ListaUsuarioPerfilIdentidad = query.Select(item => new UsuarioPerfilIdentidad
            {
                IdentidadID = item.Identidad.IdentidadID,
                PerfilID = item.Perfil.PerfilID,
                UsuarioID = item.Usuario.UsuarioID,
                NombrePerfil = item.Perfil.NombrePerfil
            }).ToList();

            return idenDW;
        }

        public int ObtenerNumRecursosEnEspacioPersonalPorPerfil(Guid pPerfilID)
        {
            return mEntityContext.DocumentoWebVinBaseRecursos.JoinDocumento().JoinBaseRecursosUsuario().JoinIdentidad().JoinPerfil().JoinPerfil().Where(item => item.TodosPerfiles.PerfilID.Equals(pPerfilID) && item.Documento.Eliminado.Equals(false) && item.DocumentoWebVinBaseRecursos.Eliminado.Equals(false)).Select(item => item.DocumentoWebVinBaseRecursos.DocumentoID).Distinct().Count();
        }

        public int ObtenerNumRecursosEnComunidadesPorPerfil(Guid pPerfilID)
        {
            return mEntityContext.DocumentoWebVinBaseRecursos.JoinDocumento().JoinBaseRecursosProyecto().JoinIdentidad().JoinPerfi().Where(item => item.Perfil.PerfilID.Equals(pPerfilID) && item.Documento.Eliminado.Equals(false) && item.DocumentoWebVinBaseRecursos.Eliminado.Equals(false)).Select(item => item.DocumentoWebVinBaseRecursos.DocumentoID).Distinct().Count();
        }

        public DataWrapperDatoExtra ObtenerIdentidadDatoExtraRegistroDeProyecto(Guid pProyectoID, Guid pIdentidadID)
        {
            //DatoExtraProyectoOpcion
            DataWrapperDatoExtra dataWrapperDatoExtra = new DataWrapperDatoExtra();

            dataWrapperDatoExtra.ListaTriples = mEntityContext.DatoExtraProyecto.JoinDatoExtraProyectoOpcion().JoinDatoExtraProyectoOpcionIdentidad().Where(item => item.DatoExtraProyecto.ProyectoID.Equals(pProyectoID) && item.DatoExtraProyectoOpcionIdentidad.IdentidadID.Equals(pIdentidadID)).Select(item => new Triples
            {
                IdentidadID = item.DatoExtraProyectoOpcionIdentidad.IdentidadID,
                PredicadorRDF = item.DatoExtraProyecto.PredicadoRDF,
                Opcion = item.DatoExtraProyectoOpcion.Opcion
            }).ToList();

            //DatoExtraProyectoVirtuoso 
            dataWrapperDatoExtra.ListaTripletasDatosExtraVirtuoso = mEntityContext.DatoExtraProyectoVirtuosoIdentidad.JoinDatoExtraProyectoVirtuoso().Where(item => item.DatoExtraProyectoVirtuosoIdentidad.ProyectoID.Equals(pProyectoID) && item.DatoExtraProyectoVirtuosoIdentidad.IdentidadID.Equals(pIdentidadID)).Select(item => new TripletasDatosExtraVirtuoso
            {
                IdentidadID = item.DatoExtraProyectoVirtuosoIdentidad.IdentidadID,
                Opcion = item.DatoExtraProyectoVirtuosoIdentidad.Opcion,
                PredicadorRDF = item.DatoExtraProyectoVirtuoso.PredicadoRDF
            }).ToList();

            foreach (TripletasDatosExtraVirtuoso fila in dataWrapperDatoExtra.ListaTripletasDatosExtraVirtuoso)
            {
                Guid identidadID = fila.IdentidadID;
                string predicadosRDF = fila.PredicadorRDF;
                string opciones = fila.Opcion;

                string[] predicadosRDFArray = predicadosRDF.Split(new string[] { "|" }, StringSplitOptions.None);
                string[] opcionesArray = opciones.Split(new string[] { "|" }, StringSplitOptions.None);

                //Si tiene el mismo n�mero de opciones que de predicados seguimos adelante
                if (predicadosRDFArray.Length > 0 && predicadosRDFArray.Length == opcionesArray.Length)
                {
                    for (int i = 0; i < predicadosRDFArray.Length; i++)
                    {
                        string predicado = predicadosRDFArray[i];
                        string opcion = opcionesArray[i];

                        if (!string.IsNullOrEmpty(predicado) && !string.IsNullOrEmpty(opcion))
                        {
                            if (opcion.StartsWith("http://"))
                            {
                                opcion = "<" + opcion + ">";
                            }

                            Triples triple = new Triples();
                            triple.IdentidadID = identidadID;
                            triple.PredicadorRDF = predicado;
                            triple.Opcion = opcion;

                            dataWrapperDatoExtra.ListaTriples.Add(triple);
                        }
                    }
                }
            }

            //DatoExtraEcosistemaOpcion
            List<Triples> listaTriples = mEntityContext.DatoExtraEcosistema.JoinDatoExtraEcosistemaOpcion().JoinDatoExtraEcosistemaOpcionPerfil().JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID)).Select(item => new Triples
            {
                IdentidadID = item.Identidad.IdentidadID,
                Opcion = item.DatoExtraEcosistemaOpcion.Opcion,
                PredicadorRDF = item.DatoExtraEcosistema.PredicadoRDF
            }).ToList();

            dataWrapperDatoExtra.ListaTriples = dataWrapperDatoExtra.ListaTriples.Union(listaTriples).ToList();

            //DatoExtraEcosistemaVirtuoso 
            dataWrapperDatoExtra.ListaTripletasDatosExtraVirtuoso = mEntityContext.DatoExtraEcosistemaVirtuosoPerfil.JoinDatoExtraEcosistemaVirtuoso().JoinIdentidad().Where(item => item.Identidad.IdentidadID.Equals(pIdentidadID)).Select(item => new TripletasDatosExtraVirtuoso
            {
                IdentidadID = item.Identidad.IdentidadID,
                Opcion = item.DatoExtraEcosistemaVirtuosoPerfil.Opcion,
                PredicadorRDF = item.DatoExtraEcosistemaVirtuoso.PredicadoRDF
            }).ToList();

            foreach (TripletasDatosExtraVirtuoso fila in dataWrapperDatoExtra.ListaTripletasDatosExtraVirtuoso)
            {
                Guid identidadID = fila.IdentidadID;
                string predicadosRDF = fila.PredicadorRDF;
                string opciones = fila.Opcion;

                string[] predicadosRDFArray = predicadosRDF.Split(new string[] { "|" }, StringSplitOptions.None);
                string[] opcionesArray = opciones.Split(new string[] { "|" }, StringSplitOptions.None);

                //Si tiene el mismo n�mero de opciones que de predicados seguimos adelante
                if (predicadosRDFArray.Length > 0 && predicadosRDFArray.Length == opcionesArray.Length)
                {
                    for (int i = 0; i < predicadosRDFArray.Length; i++)
                    {
                        string predicado = predicadosRDFArray[i];
                        string opcion = opcionesArray[i];

                        if (!string.IsNullOrEmpty(predicado) && !string.IsNullOrEmpty(opcion))
                        {
                            if (opcion.StartsWith("http://"))
                            {
                                opcion = "<" + opcion + ">";
                            }
                            Triples triple = new Triples();
                            triple.IdentidadID = identidadID;
                            triple.PredicadorRDF = predicado;
                            triple.Opcion = opcion;

                            dataWrapperDatoExtra.ListaTriples.Add(triple);
                        }
                    }
                }
            }
            return dataWrapperDatoExtra;
        }

        #endregion


        #region Privados

        /// <summary>
        /// En caso de que se utilice un GnossConfig.xml que no es el de por defecto se pasa un objeto IBaseDatos creado con respecto
        /// al fichero de configuracion que se ha pasado como par�metro
        /// </summary>
        /// <param name="IBD">Objecto IBaseDatos para el archivo pasado al constructor del AD</param>
        private void CargarConsultasYDataAdapters()
        {
            #region Consultas

            #region Selects simples sin FROM

            this.selectIdentidad = "SELECT " + IBD.CargarGuid("Identidad.IdentidadID") + ", " + IBD.CargarGuid("Identidad.PerfilID") + ", " + IBD.CargarGuid("Identidad.OrganizacionID") + ", " + IBD.CargarGuid("Identidad.ProyectoID") + ", " + IBD.CargarGuid("Identidad.CurriculumID") + ", Identidad.FechaAlta, Identidad.FechaBaja, Identidad.NumConnexiones, Identidad.Tipo, Identidad.NombreCortoIdentidad, Identidad.FechaExpulsion, Identidad.RecibirNewsLetter, Identidad.Rank, Identidad.MostrarBienvenida, Identidad.DiasUltActualizacion, Identidad.ValorAbsoluto, Identidad.ActivoEnComunidad, Identidad.ActualizaHome, Identidad.Foto ";

            this.selectPerfil = "SELECT " + IBD.CargarGuid("Perfil.PerfilID") + ", Perfil.NombrePerfil, Perfil.NombreOrganizacion, Perfil.Eliminado, Perfil.NombreCortoOrg, Perfil.NombreCortoUsu, " + IBD.CargarGuid("Perfil.OrganizacionID") + ", " + IBD.CargarGuid("Perfil.PersonaID") + ",  Perfil.TieneTwitter,  Perfil.UsuarioTwitter,  Perfil.TokenTwitter,  Perfil.TokenSecretoTwitter,  Perfil.CaducidadResSusc,  Perfil.CurriculumID ";

            this.selectGrupoIdentidades = "SELECT " + IBD.CargarGuid("GrupoIdentidades.GrupoID") + ", GrupoIdentidades.Nombre, GrupoIdentidades.NombreCorto, GrupoIdentidades.Descripcion, GrupoIdentidades.FechaAlta, GrupoIdentidades.FechaBaja, GrupoIdentidades.Tags, GrupoIdentidades.Publico, GrupoIdentidades.PermitirEnviarMensajes ";

            this.selectGrupoIdentidadesParticipacion = "SELECT " + IBD.CargarGuid("GrupoIdentidadesParticipacion.GrupoID") + ", " + IBD.CargarGuid("GrupoIdentidadesParticipacion.IdentidadID") + ", GrupoIdentidadesParticipacion.FechaAlta, GrupoIdentidadesParticipacion.FechaBaja ";

            this.selectGrupoIdentidadesProyecto = "SELECT " + IBD.CargarGuid("GrupoIdentidadesProyecto.GrupoID") + "," + IBD.CargarGuid("GrupoIdentidadesProyecto.OrganizacionID") + "," + IBD.CargarGuid("GrupoIdentidadesProyecto.ProyectoID") + " ";

            this.selectGrupoIdentidadesOrganizacion = "SELECT " + IBD.CargarGuid("GrupoIdentidadesOrganizacion.GrupoID") + "," + IBD.CargarGuid("GrupoIdentidadesOrganizacion.OrganizacionID") + " ";

            this.selectIdentidadContadoresRecursos = "SELECT " + IBD.CargarGuid("IdentidadContadoresRecursos.IdentidadID") + ", " + IBD.CargarGuid("IdentidadContadoresRecursos.Tipo") + ",IdentidadContadoresRecursos.NombreSem,IdentidadContadoresRecursos.Publicados,IdentidadContadoresRecursos.Compartidos, IdentidadContadoresRecursos.Comentarios ";

            #endregion

            this.sqlSelectIdentidad = selectIdentidad + "FROM Identidad";

            this.sqlSelectPerfil = selectPerfil + " FROM Perfil";

            this.sqlSelectPerfilRedesSociales = "SELECT " + IBD.CargarGuid("PerfilRedesSociales.PerfilID") + ", PerfilRedesSociales.NombreRedSocial, PerfilRedesSociales.urlUsuario, PerfilRedesSociales.Usuario, PerfilRedesSociales.Token, PerfilRedesSociales.TokenSecreto FROM PerfilRedesSociales";

            this.sqlSelectPerfilGadget = "SELECT " + IBD.CargarGuid("PerfilGadget.PerfilID") + ", " + IBD.CargarGuid("PerfilGadget.GadgetID") + ", PerfilGadget.Titulo, PerfilGadget.Contenido, PerfilGadget.Orden FROM PerfilGadget";

            this.sqlSelectPerfilPersonaOrg = "SELECT " + IBD.CargarGuid("PerfilPersonaOrg.PersonaID") + ", " + IBD.CargarGuid("PerfilPersonaOrg.OrganizacionID") + ", " + IBD.CargarGuid("PerfilPersonaOrg.PerfilID") + " FROM PerfilPersonaOrg";

            this.sqlSelectPerfilOrganizacion = "SELECT " + IBD.CargarGuid("PerfilOrganizacion.OrganizacionID") + ", " + IBD.CargarGuid("PerfilOrganizacion.PerfilID") + " FROM PerfilOrganizacion";

            this.sqlSelectPerfilPersona = "SELECT " + IBD.CargarGuid("PerfilPersona.PersonaID") + ", " + IBD.CargarGuid("PerfilPersona.PerfilID") + " FROM PerfilPersona";

            this.sqlSelectProfesor = "SELECT " + IBD.CargarGuid("Profesor.ProfesorID") + ", " + IBD.CargarGuid("Profesor.PerfilID") + ", Profesor.Email, Profesor.CentroEstudios, Profesor.AreaEstudios FROM Profesor";

            sqlSelectPerfilDeOrganizacion = sqlSelectPerfil + " INNER JOIN PerfilOrganizacion ON Perfil.PerfilID = PerfilOrganizacion.PerfilID WHERE PerfilOrganizacion.OrganizacionID = " + IBD.GuidParamValor("organizacionID");

            sqlSelectPerfilOrganizacionDeOrganizacion = sqlSelectPerfilOrganizacion + " INNER JOIN Perfil ON PerfilOrganizacion.PerfilID = Perfil.PerfilID WHERE PerfilOrganizacion.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " AND Perfil.Eliminado = 0";

            sqlSelectPerfilOrganizacionDeOrganizacionNoActivosTambien = sqlSelectPerfilOrganizacion + " INNER JOIN Perfil ON PerfilOrganizacion.PerfilID = Perfil.PerfilID WHERE PerfilOrganizacion.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " ";

            sqlSelectIdentidadesSoloDeOrganizacion = sqlSelectIdentidad + " INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN PerfilOrganizacion ON Perfil.PerfilID = PerfilOrganizacion.PerfilID WHERE PerfilOrganizacion.OrganizacionID = " + IBD.GuidParamValor("organizacionID");

            sqlSelectPerfilesDePersona = this.selectPerfil + ", Identidad.NumConnexiones FROM Perfil INNER JOIN Identidad ON Identidad.PerfilID = Perfil.PerfilID WHERE Perfil.PersonaID = " + IBD.GuidParamValor("personaID") + " AND Identidad.ProyectoID = '11111111-1111-1111-1111-111111111111' UNION " + this.selectPerfil + ", Identidad.NumConnexiones FROM Perfil INNER JOIN Identidad ON Identidad.PerfilID = Perfil.PerfilID INNER JOIN PerfilPersonaOrg on Perfil.OrganizacionID = PerfilPersonaOrg.OrganizacionID WHERE Perfil.PersonaID IS NULL AND perfilPersonaOrg.PersonaID = " + IBD.GuidParamValor("personaID") + " AND Identidad.ProyectoID = '11111111-1111-1111-1111-111111111111' ";

            sqlSelectPerfilRedesSocDePersona = this.sqlSelectPerfilRedesSociales + " INNER JOIN Perfil ON Perfil.PerfilID = PerfilRedesSociales.PerfilID WHERE Perfil.PersonaID = " + IBD.GuidParamValor("personaID");

            sqlSelectIdentidadesDeOrganizacion = this.sqlSelectIdentidad + " INNER JOIN Perfil ON Identidad.PerfilID = Perfil.PerfilID WHERE Perfil.OrganizacionID = " + IBD.GuidParamValor("OrganizacionID") + " AND Identidad.FechaBaja IS NULL UNION " + this.sqlSelectIdentidad + " INNER JOIN Perfil ON Identidad.PerfilID = Perfil.PerfilID INNER JOIN PerfilPersonaOrg ON Perfil.OrganizacionID = PerfilPersonaOrg.OrganizacionID WHERE Perfil.PersonaID IS NULL AND PerfilPersonaOrg.OrganizacionID = " + IBD.GuidParamValor("OrganizacionID") + " AND Identidad.FechaBaja IS NULL";

            sqlSelectIdentidadesDeOrganizacionNoActivosTambien = this.sqlSelectIdentidad + " INNER JOIN Perfil ON Identidad.PerfilID = Perfil.PerfilID INNER JOIN PerfilPersonaOrg ON Identidad.PerfilID = PerfilPersonaOrg.PerfilID WHERE PerfilPersonaOrg.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " UNION " + this.sqlSelectIdentidad + " INNER JOIN Perfil ON Identidad.PerfilID = Perfil.PerfilID INNER JOIN PerfilOrganizacion ON Identidad.PerfilID = PerfilOrganizacion.PerfilID WHERE PerfilOrganizacion.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " ";

            sqlSelectPerfilesDeOrganizacion = this.sqlSelectPerfil + " INNER JOIN PerfilPersonaOrg ON Perfil.PerfilID = PerfilPersonaOrg.PerfilID WHERE PerfilPersonaOrg.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " AND Perfil.Eliminado = 0 UNION ALL " + this.sqlSelectPerfil + " INNER JOIN PerfilOrganizacion ON Perfil.PerfilID = PerfilOrganizacion.PerfilID WHERE PerfilOrganizacion.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " AND Perfil.Eliminado = 0";

            sqlSelectPerfilesDeOrganizacionNoActivosTambien = this.sqlSelectPerfil + " INNER JOIN PerfilPersonaOrg ON Perfil.PerfilID = PerfilPersonaOrg.PerfilID WHERE PerfilPersonaOrg.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " UNION ALL " + this.sqlSelectPerfil + " INNER JOIN PerfilOrganizacion ON Perfil.PerfilID = PerfilOrganizacion.PerfilID WHERE PerfilOrganizacion.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " ";

            sqlSelectPerfilPersonaOrgDeOrganizacion = sqlSelectPerfilPersonaOrg.Replace("SELECT", "SELECT DISTINCT ") + " INNER JOIN Perfil ON PerfilPersonaOrg.PerfilID = Perfil.PerfilID INNER JOIN Identidad ON Perfil.PerfilID = Identidad.PerfilID WHERE (PerfilPersonaOrg.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ") AND Identidad.FechaBaja IS NULL AND Perfil.Eliminado = 0";

            sqlSelectPerfilPersonaOrgDeOrganizacionNoActivosTambien = sqlSelectPerfilPersonaOrg.Replace("SELECT", "SELECT DISTINCT ") + " INNER JOIN Perfil ON PerfilPersonaOrg.PerfilID = Perfil.PerfilID INNER JOIN Identidad ON Perfil.PerfilID = Identidad.PerfilID WHERE (PerfilPersonaOrg.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ") ";

            sqlSelectPerfilPersonaOrgDePersona = this.sqlSelectPerfilPersonaOrg + " WHERE PersonaID = " + IBD.GuidParamValor("personaID");

            sqlSelectPerfilOrganizacionDePersona = this.sqlSelectPerfilOrganizacion.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN PerfilPersonaOrg ON PerfilPersonaOrg.OrganizacionID = PerfilOrganizacion.OrganizacionID WHERE PerfilPersonaOrg.PersonaID = " + IBD.GuidParamValor("personaID");

            sqlSelectProfesorDePersona = this.sqlSelectProfesor + " INNER JOIN Perfil ON Perfil.PerfilID = Profesor.PerfilID WHERE Perfil.PersonaID = " + IBD.GuidParamValor("personaID");

            sqlSelectPerfilPersonaDePersona = this.sqlSelectPerfilPersona + " WHERE PersonaID = " + IBD.GuidParamValor("personaID");

            sqlSelectIdentidadesDePersona = this.sqlSelectIdentidad + " INNER JOIN Perfil ON Identidad.PerfilID = Perfil.PerfilID INNER JOIN PerfilPersona ON Identidad.PerfilID = PerfilPersona.PerfilID WHERE PerfilPersona.PersonaID = " + IBD.GuidParamValor("personaID") + " UNION " + this.sqlSelectIdentidad + " INNER JOIN Perfil ON Identidad.PerfilID = Perfil.PerfilID INNER JOIN PerfilPersonaOrg ON Identidad.PerfilID = PerfilPersonaOrg.PerfilID WHERE PerfilPersonaOrg.PersonaID = " + IBD.GuidParamValor("personaID") + " UNION " + this.sqlSelectIdentidad + " INNER JOIN PerfilOrganizacion ON Identidad.PerfilID = PerfilOrganizacion.PerfilID INNER JOIN PerfilPersonaOrg ON PerfilOrganizacion.OrganizacionID = PerfilPersonaOrg.OrganizacionID WHERE PerfilPersonaOrg.PersonaID = " + IBD.GuidParamValor("personaID");

            this.sqlSelectCVIdentidadPersonaDeUsuario = "SELECT " + IBD.CargarGuid("CVIdentidadPersona.IdentidadID") + ", " + IBD.CargarGuid("CVIdentidadPersona.PersonaID") + ", " + IBD.CargarGuid("CVIdentidadPersona.CurriculumID") + " FROM CVIdentidadPersona INNER JOIN ProyectoUsuarioIdentidad ON CVIdentidadPersona.IdentidadID = ProyectoUsuarioIdentidad.IdentidadID WHERE (ProyectoUsuarioIdentidad.UsuarioID = " + IBD.GuidParamValor("UsuarioID") + ")";

            this.sqlSelectIdentidadDePersonaDeMyGNOSS = this.sqlSelectIdentidad.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN Perfil ON Identidad.PerfilID = Perfil.PerfilID  WHERE (Perfil.PersonaID = " + IBD.GuidParamValor("PersonaID") + ") AND Perfil.OrganizacionID IS NULL and Perfil.PersonaID IS NOT NULL AND Identidad.ProyectoID = '" + IBD.ValorDeGuid(ProyectoAD.MetaProyecto) + "' ";

            this.sqlSelectPerfilDePersona = this.sqlSelectPerfil + " INNER JOIN PerfilPersona ON Perfil.PerfilID = PerfilPersona.PerfilID WHERE PerfilPersona.PersonaID = " + IBD.GuidParamValor("PersonaID");

            this.sqlSelectIdentidadPorID = this.sqlSelectIdentidad + " WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("IdentidadID") + ")";

            this.sqlSelectIdentidadPorIDConMyGNOSS = this.sqlSelectIdentidad + " WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("IdentidadID") + ") UNION " + this.sqlSelectIdentidad + " WHERE Identidad.ProyectoID = " + IBD.GuidValor(ProyectoAD.MetaProyecto).ToString() + " AND Identidad.PerfilID in (SELECT PerfilID FROM Identidad WHERE IdentidadID = " + IBD.GuidParamValor("IdentidadID") + ")";

            this.sqlSelectPerfilPorIdentidadID = this.sqlSelectPerfil + " INNER JOIN Identidad ON Perfil.PerfilID = Identidad.PerfilID WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("IdentidadID") + ")";

            this.sqlSelectPerfilRedesSocPorIdentidadID = this.sqlSelectPerfilRedesSociales + " INNER JOIN Perfil ON Perfil.PerfilID = PerfilRedesSociales.PerfilID INNER JOIN Identidad ON Perfil.PerfilID = Identidad.PerfilID WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("IdentidadID") + ")";

            this.sqlSelectPerfilPersonaOrgPorID = this.sqlSelectPerfilPersonaOrg + " INNER JOIN Identidad ON PerfilPersonaOrg.PerfilID = Identidad.PerfilID WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("IdentidadID") + ")";

            this.sqlSelectPerfilOrganizacionPorID = this.sqlSelectPerfilOrganizacion + " INNER JOIN Identidad ON PerfilOrganizacion.PerfilID = Identidad.PerfilID WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("IdentidadID") + ")";

            this.sqlSelectPerfilPersonaPorID = this.sqlSelectPerfilPersona + " INNER JOIN Identidad ON PerfilPersona.PerfilID = Identidad.PerfilID WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("IdentidadID") + ")";

            this.sqlSelectProfesorPorID = this.sqlSelectProfesor + " INNER JOIN Identidad ON Profesor.PerfilID = Identidad.PerfilID WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("IdentidadID") + ")";

            this.sqlSelectIdentidadesDePersonaEnProyecto = this.sqlSelectIdentidad + " INNER JOIN ProyectoUsuarioIdentidad ON Identidad.IdentidadID = ProyectoUsuarioIdentidad.IdentidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID WHERE (ProyectoUsuarioIdentidad.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND Perfil.PersonaID = " + IBD.GuidParamValor("personaID") + " )";


            this.sqlSelectIdentidadesDeProyecto = this.sqlSelectIdentidad + " WHERE Identidad.ProyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlSelectIdentidadesDePersonasNoCorporativasDeProyecto = this.sqlSelectIdentidad + " INNER JOIN ProyectoUsuarioIdentidad ON Identidad.IdentidadID = ProyectoUsuarioIdentidad.IdentidadID WHERE (ProyectoUsuarioIdentidad.ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") AND (Identidad.Tipo < 2 OR Identidad.Tipo = 4) AND Identidad.FechaBaja IS NULL";

            this.sqlSelectIdentidadesDePersonasNoCorporativasDeProyectoConNombreYApellidos = this.selectIdentidad + ", Persona.Nombre, Persona.Apellidos FROM Identidad INNER JOIN ProyectoUsuarioIdentidad ON Identidad.IdentidadID = ProyectoUsuarioIdentidad.IdentidadID INNER JOIN Perfil ON (Identidad.PerfilID=Perfil.PerfilID) INNER JOIN Persona ON (Perfil.PersonaID=Persona.PersonaID) WHERE (ProyectoUsuarioIdentidad.ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") AND (Identidad.Tipo < 2 OR Identidad.Tipo = 4) AND Identidad.FechaBaja IS NULL";

            sqlSelectPerfilesDeProyecto = this.sqlSelectPerfil.Replace("SELECT", "SELECT DISTINCT ") + " INNER JOIN Identidad ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN ProyectoUsuarioIdentidad ON Identidad.IdentidadID = ProyectoUsuarioIdentidad.IdentidadID WHERE (ProyectoUsuarioIdentidad.ProyectoID = " + IBD.GuidParamValor("proyectoID") + ")";

            sqlSelectPerfilesDePersonasNoCorporativasDeProyecto = this.sqlSelectPerfil + " INNER JOIN Identidad ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN ProyectoUsuarioIdentidad ON Identidad.IdentidadID = ProyectoUsuarioIdentidad.IdentidadID WHERE (ProyectoUsuarioIdentidad.ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") AND (Identidad.Tipo < 2 OR Identidad.Tipo = 4) AND Identidad.FechaBaja IS NULL";

            sqlSelectPerfilPerfilRedesSocDeProyecto = this.sqlSelectPerfilRedesSociales.Replace("SELECT", "SELECT DISTINCT ") + " INNER JOIN Identidad ON PerfilRedesSociales.PerfilID = Identidad.PerfilID INNER JOIN ProyectoUsuarioIdentidad ON Identidad.IdentidadID = ProyectoUsuarioIdentidad.IdentidadID WHERE (ProyectoUsuarioIdentidad.ProyectoID = " + IBD.GuidParamValor("proyectoID") + ")";

            this.sqlSelectIdentidadDeProyectoYUsuario = this.sqlSelectIdentidad + " INNER JOIN Identidad ON Identidad.IdentidadID = ProyectoUsuarioIdentidad.IdentidadID WHERE (ProyectoUsuarioIdentidad.ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") AND (ProyectoUsuarioIdentidad.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ")";

            this.sqlSelectPerfilPersonaOrgDeProyecto = sqlSelectPerfilPersonaOrg.Replace("SELECT", "SELECT DISTINCT ") + " INNER JOIN Identidad ON PerfilPersonaOrg.PerfilID = Identidad.PerfilID WHERE (Identidad.ProyectoID = " + IBD.GuidParamValor("proyectoID") + ")";

            this.sqlSelectPerfilPersonaOrgDePersonasNoCorporativasDeProyecto = sqlSelectPerfilPersonaOrg.Replace("SELECT", "SELECT DISTINCT ") + " INNER JOIN Identidad ON PerfilPersonaOrg.PerfilID = Identidad.PerfilID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID WHERE (Identidad.ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") AND (Identidad.Tipo < 2 OR Identidad.Tipo = 4)";

            this.sqlSelectPerfilPersonaDeProyecto = "SELECT DISTINCT " + IBD.CargarGuid("PerfilPersona.PersonaID") + ", " + IBD.CargarGuid("PerfilPersona.PerfilID") + " FROM PerfilPersona INNER JOIN Identidad ON PerfilPersona.PerfilID = Identidad.PerfilID WHERE (Identidad.ProyectoID = " + IBD.GuidParamValor("proyectoID") + ")  AND Identidad.FechaBaja is null";

            sqlSelectPerfilOrganizacionDeProyecto = "SELECT DISTINCT " + IBD.CargarGuid("PerfilOrganizacion.OrganizacionID") + ", " + IBD.CargarGuid("PerfilOrganizacion.PerfilID") + " FROM PerfilOrganizacion INNER JOIN Identidad ON PerfilOrganizacion.PerfilID = Identidad.PerfilID WHERE (Identidad.ProyectoID = " + IBD.GuidParamValor("proyectoID") + ")  AND Identidad.FechaBaja IS NULL";

            sqlSelectPerfilesDeUsuario = sqlSelectPerfil + " INNER JOIN PerfilPersona ON Perfil.PerfilID = PerfilPersona.PerfilID INNER JOIN Persona ON PerfilPersona.PersonaID = Persona.PersonaID WHERE Persona.UsuarioID = " + IBD.GuidParamValor("usuarioID") + " AND Perfil.Eliminado = 0 UNION " + this.sqlSelectPerfil + " INNER JOIN PerfilPersonaOrg ON Perfil.PerfilID = PerfilPersonaOrg.PerfilID INNER JOIN Persona ON PerfilPersonaOrg.PersonaID = Persona.PersonaID WHERE Persona.UsuarioID = " + IBD.GuidParamValor("usuarioID") + " AND Perfil.Eliminado = 0";

            sqlSelectPerfilPersonaDeUsuario = sqlSelectPerfilPersona + " INNER JOIN Persona ON PerfilPersona.PersonaID = Persona.PersonaID WHERE Persona.UsuarioID = " + IBD.GuidParamValor("usuarioID");

            sqlSelectPerfilPersonaOrgDeUsuario = sqlSelectPerfilPersonaOrg + " INNER JOIN Persona ON PerfilPersonaOrg.PersonaID = Persona.PersonaID WHERE Persona.UsuarioID = " + IBD.GuidParamValor("usuarioID");

            sqlSelectIdentidadesDeUsuario = sqlSelectIdentidad + " INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN PerfilPersona ON Perfilpersona.PerfilID = Perfil.PerfilID INNER JOIN Persona ON PerfilPersona.PersonaID = Persona.PersonaID WHERE Persona.UsuarioID = " + IBD.GuidParamValor("usuarioID") + " UNION " + sqlSelectIdentidad + " INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN PerfilPersonaOrg ON PerfilPersonaOrg.PerfilID = Perfil.PerfilID INNER JOIN Persona ON PerfilPersonaOrg.PersonaID = Persona.PersonaID WHERE Persona.UsuarioID = " + IBD.GuidParamValor("usuarioID");

            this.sqlSelectExistePerfilPersonal = sqlSelectPerfilPersona + " WHERE PerfilPersona.PersonaID = " + IBD.GuidParamValor("personaID");

            this.sqlSelectExistePerfilPersonaOrg = sqlSelectPerfilPersonaOrg + " WHERE PerfilPersonaOrg.PersonaID = " + IBD.GuidParamValor("personaID") + " AND PerfilPersonaOrg.OrganizacionID = " + IBD.GuidParamValor("organizacionID");

            this.sqlSelectPerfilPorPerfilID = sqlSelectPerfil + " WHERE Perfil.PerfilID = " + IBD.GuidParamValor("perfilID");

            this.sqlSelectDatoExtraProyectoOpcionIdentidadPorIdentidadesID = "SELECT " + IBD.CargarGuid("DatoExtraProyectoOpcionIdentidad.OrganizacionID") + "," + IBD.CargarGuid("DatoExtraProyectoOpcionIdentidad.ProyectoID") + "," + IBD.CargarGuid("DatoExtraProyectoOpcionIdentidad.DatoExtraID") + "," + IBD.CargarGuid("DatoExtraProyectoOpcionIdentidad.OpcionID") + "," + IBD.CargarGuid("DatoExtraProyectoOpcionIdentidad.IdentidadID") + " FROM DatoExtraProyectoOpcionIdentidad ";

            this.sqlSelectDatoExtraProyectoVirtuosoIdentidadPorIdentidadesID = "SELECT " + IBD.CargarGuid("DatoExtraProyectoVirtuosoIdentidad.OrganizacionID") + "," + IBD.CargarGuid("DatoExtraProyectoVirtuosoIdentidad.ProyectoID") + "," + IBD.CargarGuid("DatoExtraProyectoVirtuosoIdentidad.DatoExtraID") + ", DatoExtraProyectoVirtuosoIdentidad.Opcion," + IBD.CargarGuid("DatoExtraProyectoVirtuosoIdentidad.IdentidadID") + " FROM DatoExtraProyectoVirtuosoIdentidad ";

            this.sqlSelectDatoExtraEcosistemaOpcionPerfilPorIdentidadesID = "SELECT " + IBD.CargarGuid("DatoExtraEcosistemaOpcionPerfil.DatoExtraID") + "," + IBD.CargarGuid("DatoExtraEcosistemaOpcionPerfil.OpcionID") + "," + IBD.CargarGuid("DatoExtraEcosistemaOpcionPerfil.PerfilID") + " FROM DatoExtraEcosistemaOpcionPerfil INNER JOIN Identidad on DatoExtraEcosistemaOpcionPerfil.PerfilID=Identidad.PerfilID ";

            this.sqlSelectDatoExtraEcosistemaVirtuosoPerfilPorIdentidadesID = "SELECT " + IBD.CargarGuid("DatoExtraEcosistemaVirtuosoPerfil.DatoExtraID") + ", DatoExtraEcosistemaVirtuosoPerfil.Opcion," + IBD.CargarGuid("DatoExtraEcosistemaVirtuosoPerfil.PerfilID") + " FROM DatoExtraEcosistemaVirtuosoPerfil INNER JOIN Identidad on DatoExtraEcosistemaVirtuosoPerfil.PerfilID=Identidad.PerfilID ";

            this.sqlSelectIdentidadesPorPerfilID = sqlSelectIdentidad + " WHERE Identidad.PerfilID = " + IBD.GuidParamValor("perfilID");

            this.sqlSelectPerfilDeUsuarioActivos = this.sqlSelectPerfil + " WHERE Perfil.PersonaID = " + IBD.GuidParamValor("personaID") + " AND Perfil.Eliminado = 0 UNION " + this.sqlSelectPerfil + " INNER JOIN PerfilPersonaOrg on Perfil.OrganizacionID = PerfilPersonaOrg.OrganizacionID WHERE Perfil.PersonaID IS NULL AND perfilPersonaOrg.PersonaID = " + IBD.GuidParamValor("personaID") + " AND Eliminado=0";

            this.sqlSelectPerfilRedesSocDeUsuarioActivos = this.sqlSelectPerfilRedesSociales + " INNER JOIN Perfil ON Perfil.PerfilID = PerfilRedesSociales.PerfilID WHERE Perfil.PersonaID = " + IBD.GuidParamValor("personaID") + " AND Perfil.Eliminado = 0";

            this.sqlSelectIdentidadesDePersonaActivas = this.sqlSelectIdentidad + " INNER JOIN Perfil ON Identidad.PerfilID = Perfil.PerfilID WHERE Perfil.PersonaID = " + IBD.GuidParamValor("personaID") + " AND Perfil.Eliminado = 0 AND Identidad.FechaBaja IS NULL UNION " + this.sqlSelectIdentidad + " INNER JOIN Perfil ON Identidad.PerfilID = Perfil.PerfilID INNER JOIN PerfilPersonaOrg ON Perfil.OrganizacionID = PerfilPersonaOrg.OrganizacionID WHERE Perfil.PersonaID IS NULL AND PerfilPersonaOrg.PersonaID = " + IBD.GuidParamValor("personaID") + " AND Perfil.Eliminado = 0 AND Identidad.FechaBaja IS NULL";

            this.sqlSelectPerfilPersonaDePersonaActivos = this.sqlSelectPerfilPersona + " INNER JOIN Perfil ON Perfil.PerfilID = PerfilPersona.PerfilID WHERE Perfil.PersonaID = " + IBD.GuidParamValor("personaID") + " AND Perfil.Eliminado = 0";

            this.sqlSelectPerfilPersonaOrgDePersonaActivos = this.sqlSelectPerfilPersonaOrg + " INNER JOIN Perfil ON Perfil.PerfilID = PerfilPersonaOrg.PerfilID WHERE Perfil.PersonaID = " + IBD.GuidParamValor("personaID") + " AND Perfil.Eliminado = 0";

            this.sqlSelectPerfilOrganizacionDePersonaActivos = this.sqlSelectPerfilOrganizacion.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN PerfilPersonaOrg ON PerfilPersonaOrg.OrganizacionID = PerfilOrganizacion.OrganizacionID INNER JOIN Perfil ON Perfil.PerfilID = PerfilOrganizacion.PerfilID WHERE PerfilPersonaOrg.PersonaID = " + IBD.GuidParamValor("personaID") + " AND Perfil.Eliminado = 0";

            sqlSelectProfesorDePersonaActivos = this.sqlSelectProfesor + " INNER JOIN Perfil ON Perfil.PerfilID = Profesor.PerfilID WHERE Perfil.PersonaID = " + IBD.GuidParamValor("personaID") + " AND Perfil.Eliminado = 0";

            this.sqlSelectIdentidadPorIDActiva = this.sqlSelectIdentidad + " INNER JOIN Identidad identClave on Identidad.PerfilID = identClave.PerfilID INNER JOIN Identidad identidadMyGnoss on identClave.PerfilID = identidadMyGnoss.PerfilID WHERE Identidad.FechaBaja IS NULL AND identidadMyGnoss.ProyectoID = " + IBD.GuidValor(ProyectoAD.MetaProyecto).ToString() + " AND identClave.IdentidadID = " + IBD.GuidParamValor("IdentidadID") + " AND (Identidad.IdentidadID = identClave.IdentidadID OR Identidad.IdentidadID = identidadMyGnoss.IdentidadID)";

            this.sqlSelectPerfilPorIdentidadIDActivo = this.sqlSelectPerfil + " INNER JOIN Identidad ON Perfil.PerfilID = Identidad.PerfilID WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("IdentidadID") + ") AND Identidad.FechaBaja IS NULL AND Perfil.Eliminado = 0";

            this.sqlSelectPerfilRedesSocPorIdentidadIDActivo = this.sqlSelectPerfilRedesSociales + " INNER JOIN Perfil ON Perfil.PerfilID = PerfilRedesSociales.PerfilID INNER JOIN Identidad ON Perfil.PerfilID = Identidad.PerfilID WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("IdentidadID") + ") AND Identidad.FechaBaja IS NULL AND Perfil.Eliminado = 0";

            this.sqlSelectPerfilPersonaPorIDActiva = this.sqlSelectPerfilPersona + " INNER JOIN Identidad ON PerfilPersona.PerfilID = Identidad.PerfilID INNER JOIN Perfil ON Perfil.PerfilID = PerfilPersona.PerfilID WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("IdentidadID") + ") AND Perfil.Eliminado = 0 AND Identidad.FechaBaja IS NULL";

            this.sqlSelectPerfilPersonaOrgPorIDActiva = this.sqlSelectPerfilPersonaOrg + " INNER JOIN Identidad ON PerfilPersonaOrg.PerfilID = Identidad.PerfilID INNER JOIN Perfil ON Perfil.PerfilID = PerfilPersonaOrg.PerfilID WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("IdentidadID") + ") AND Perfil.Eliminado = 0 AND Identidad.FechaBaja IS NULL";

            this.sqlSelectPerfilOrganizacionPorIDActiva = this.sqlSelectPerfilOrganizacion + " INNER JOIN Identidad ON PerfilOrganizacion.PerfilID = Identidad.PerfilID INNER JOIN Perfil ON Perfil.PerfilID = PerfilOrganizacion.PerfilID WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("IdentidadID") + ") AND Perfil.Eliminado = 0 AND Identidad.FechaBaja IS NULL";

            this.sqlSelectProfesorPorIDActiva = this.sqlSelectProfesor + " INNER JOIN Identidad ON Profesor.PerfilID = Identidad.PerfilID INNER JOIN Perfil ON Perfil.PerfilID = Profesor.PerfilID WHERE (Identidad.IdentidadID = " + IBD.GuidParamValor("IdentidadID") + ") AND Perfil.Eliminado = 0 AND Identidad.FechaBaja IS NULL";

            this.sqlSelectIdentidadesDepersonasConOrgDeProy = this.sqlSelectIdentidad + " INNER JOIN PerfilPersonaOrg ON PerfilPersonaOrg.PerfilID = Identidad.PerfilID WHERE PerfilPersonaOrg.OrganizacionID = " + IBD.GuidParamValor("OrganizacionID") + " AND Identidad.OrganizacionID = " + IBD.GuidParamValor("OrganizacionProyectoID") + " AND Identidad.ProyectoID = " + IBD.GuidParamValor("ProyectoID") + " AND Identidad.FechaBaja IS NULL AND Identidad.FechaBaja IS NULL ";

            this.sqlSelectIdentidadesDepersonasConOrgDeProyConNombreYApellidos = this.selectIdentidad + ", Persona.Nombre, Persona.Apellidos FROM Identidad INNER JOIN PerfilPersonaOrg ON PerfilPersonaOrg.PerfilID = Identidad.PerfilID INNER JOIN Persona ON (PerfilPersonaOrg.PersonaID=Persona.PersonaID) WHERE PerfilPersonaOrg.OrganizacionID = " + IBD.GuidParamValor("OrganizacionID") + " AND Identidad.OrganizacionID = " + IBD.GuidParamValor("OrganizacionProyectoID") + " AND Identidad.ProyectoID = " + IBD.GuidParamValor("ProyectoID") + " AND Identidad.FechaBaja IS NULL AND Identidad.FechaBaja IS NULL ";

            this.sqlSelectPerfilDepersonasConOrgDeProy = this.sqlSelectPerfil + " INNER JOIN Identidad ON Perfil.PerfilID = Identidad.PerfilID WHERE Perfil.OrganizacionID = " + IBD.GuidParamValor("OrganizacionID") + " AND Identidad.OrganizacionID = " + IBD.GuidParamValor("OrganizacionProyectoID") + " AND Identidad.ProyectoID = " + IBD.GuidParamValor("ProyectoID") + " AND Identidad.Tipo != 3 AND Identidad.FechaBaja IS NULL AND Identidad.FechaBaja IS NULL ";

            this.sqlSelectPerfilPersonaOrgDepersonasConOrgDeProy = this.sqlSelectPerfilPersonaOrg + " INNER JOIN Identidad ON PerfilPersonaOrg.PerfilID = Identidad.PerfilID WHERE PerfilPersonaOrg.OrganizacionID = " + IBD.GuidParamValor("OrganizacionID") + " AND Identidad.OrganizacionID = " + IBD.GuidParamValor("OrganizacionProyectoID") + " AND Identidad.ProyectoID = " + IBD.GuidParamValor("ProyectoID") + " AND Identidad.FechaBaja IS NULL AND Identidad.FechaBaja IS NULL ";

            this.sqlSelectIdentidadesDeOrganizacionesEnProyecto = this.sqlSelectIdentidad + " INNER JOIN PerfilOrganizacion ON PerfilOrganizacion.PerfilID = Identidad.PerfilID INNER JOIN OrganizacionParticipaProy ON OrganizacionParticipaProy.OrganizacionID = PerfilOrganizacion.OrganizacionID WHERE OrganizacionParticipaProy.ProyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlSelectPerfilesDeOrganizacionesDeProyecto = this.sqlSelectPerfil + " INNER JOIN PerfilOrganizacion ON PerfilOrganizacion.PerfilID = Perfil.PerfilID WHERE PerfilOrganizacion.OrganizacionID = " + IBD.GuidParamValor("organizacionID");

            this.sqlSelectPerfilOrganizacionDeOrganizacionesDeProyecto = this.sqlSelectPerfilOrganizacion + " WHERE PerfilOrganizacion.OrganizacionID = " + IBD.GuidParamValor("organizacionID");

            this.sqlSelectIdentidadesDeOrganizacionAdmin = this.sqlSelectIdentidad.Replace("SELECT", "SELECT DISTINCT ") + " INNER JOIN Perfil ON Identidad.PerfilID = Perfil.PerfilID INNER JOIN PerfilPersonaOrg ON Perfil.PerfilID = PerfilPersonaOrg.PerfilID INNER JOIN Persona ON PerfilPersonaOrg.PersonaID = Persona.PersonaID  WHERE PerfilPersonaOrg.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " AND Identidad.FechaBaja IS NULL AND Perfil.Eliminado = 0 AND Identidad.ProyectoID = " + IBD.GuidValor(ProyectoAD.MetaProyecto).ToString() + " AND Persona.UsuarioID IN (Select Distinct usuarioID from AdministradorOrganizacion where OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " AND Tipo =" + (short)TipoAdministradoresOrganizacion.Administrador + ")";

            this.sqlSelectPerfilesDeOrganizacionAdmin = this.sqlSelectPerfil.Replace("SELECT", "SELECT DISTINCT ") + " INNER JOIN PerfilPersonaOrg ON Perfil.PerfilID = PerfilPersonaOrg.PerfilID INNER JOIN Persona ON PerfilPersonaOrg.PersonaID = Persona.PersonaID WHERE PerfilPersonaOrg.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " AND Perfil.Eliminado = 0 AND Persona.UsuarioID IN (Select Distinct usuarioID from AdministradorOrganizacion where OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " AND Tipo =" + (short)TipoAdministradoresOrganizacion.Administrador + ")";

            this.sqlSelectPerfilPersonaOrgDeOrganizacionAdmin = this.sqlSelectPerfilPersonaOrg.Replace("SELECT", "SELECT DISTINCT ") + " INNER JOIN Perfil ON Perfil.PerfilID = PerfilPersonaOrg.PerfilID INNER JOIN Persona ON PerfilPersonaOrg.PersonaID = Persona.PersonaID WHERE PerfilPersonaOrg.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " AND Perfil.Eliminado = 0 AND Persona.UsuarioID IN (Select Distinct usuarioID from AdministradorOrganizacion where OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " AND Tipo =" + (short)TipoAdministradoresOrganizacion.Administrador + ")";

            this.sqlUpdateNumeroConexionesProyecto = "UPDATE Identidad Set NumConnexiones = NumConnexiones + 1 WHERE IdentidadID = " + IBD.ToParam("IdentidadID");

            this.sqlSelectHaParticipadoConPerfilEnComunidad = "SELECT " + IBD.CargarGuid("Identidad.IdentidadID") + " FROM Identidad WHERE Identidad.PerfilID = " + IBD.GuidParamValor("perfilID") + " AND Identidad.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND Identidad.FechaBaja IS NOT NULL";

            this.sqlSelectEstaIdentidadExpulsadaDeproyecto = "SELECT " + IBD.CargarGuid("Identidad.IdentidadID") + " FROM Identidad WHERE Identidad.PerfilID = " + IBD.GuidParamValor("perfilID") + " AND Identidad.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND Identidad.FechaBaja IS NOT NULL AND FechaExpulsion IS NOT NULL";

            this.sqlSelectParticipaPerfilEnComunidad = "SELECT " + IBD.CargarGuid("Identidad.IdentidadID") + " FROM Identidad WHERE Identidad.PerfilID = " + IBD.GuidParamValor("perfilID") + " AND Identidad.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND Identidad.FechaBaja IS NULL";

            this.sqlSelectIdentidadContadoresRecursos = selectIdentidadContadoresRecursos + " FROM IdentidadContadoresRecursos";

            this.sqlSelectParticipaIdentidadEnComunidad = "SELECT " + IBD.CargarGuid("IdentidadProyecto.IdentidadID") + " FROM Identidad IdentidadProyecto INNER JOIN Identidad IdentidadPerfil ON IdentidadPerfil.PerfilID = IdentidadProyecto.PerfilID WHERE IdentidadPerfil.IdentidadID = " + IBD.GuidParamValor("identidadID") + " AND IdentidadProyecto.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND IdentidadProyecto.FechaBaja IS NULL";

            #endregion

            #region Updates

            sqlUpdatePerfilCambioNombrePersona = "UPDATE Perfil SET NombrePerfil = (SELECT CASE WHEN (Persona.Idioma = 'es' AND Persona.Sexo = 'H') THEN 'Profesor � ' WHEN (Persona.Idioma = 'es' AND Persona.Sexo = 'M') THEN 'Profesora � ' ELSE 'Teacher � ' END + " + IBD.ToParam("nombreCompletoPersona") + " FROM Persona WHERE Persona.PersonaID = Perfil.PersonaID AND Perfil.PerfilID IN (Select PerfilID FROM Profesor) UNION ALL SELECT " + IBD.ToParam("nombreCompletoPersona") + " WHERE Perfil.PerfilID NOT IN (Select PerfilID FROM Profesor)) WHERE Perfil.PersonaID = " + IBD.GuidParamValor("personaID");

            sqlUpdateIdentidadCambioNombrePersona = "UPDATE Identidad SET NombreCortoIdentidad = (SELECT CASE WHEN (Persona.Idioma = 'es' AND Persona.Sexo = 'H') THEN 'Profesor � ' WHEN (Persona.Idioma = 'es' AND Persona.Sexo = 'M') THEN 'Profesora � ' ELSE 'Teacher � ' END + " + IBD.ToParam("nombrePersona") + " FROM Persona INNER JOIN Perfil ON Perfil.PersonaID = Persona.PersonaID WHERE Perfil.PerfilID = Identidad.PerfilID AND Perfil.PerfilID IN (Select PerfilID FROM Profesor) UNION ALL SELECT " + IBD.ToParam("nombrePersona") + " WHERE Identidad.PerfilID NOT IN (Select PerfilID FROM Profesor)) WHERE Identidad.PerfilID IN (SELECT Perfil.PerfilID FROM Perfil WHERE Perfil.PersonaID = " + IBD.GuidParamValor("personaID") + ") AND (Identidad.Tipo < 2 OR Identidad.Tipo = 4)";

            sqlUpdatePerfilCambioNombreOrganizacion = "UPDATE Perfil SET NombrePerfil=" + IBD.ToParam("nombrePerfil") + ", NombreOrganizacion = " + IBD.ToParam("nombreOrganizacion") + " WHERE Perfil.PerfilID IN (Select Perfil.PerfilID FROM Perfil INNER JOIN Identidad ON (Perfil.PerfilID=Identidad.PerfilID) WHERE Perfil.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " AND Identidad.Tipo = " + (short)TiposIdentidad.Organizacion + ")";

            sqlUpdatePerfilPersOrgCambioNombreOrganizacion = "UPDATE Perfil SET NombreOrganizacion = " + IBD.ToParam("nombreOrganizacion") + " WHERE Perfil.OrganizacionID = " + IBD.GuidParamValor("organizacionID");

            sqlUpdateIdentidadCambioNombreOrganizacion = "UPDATE Identidad SET NombreCortoIdentidad = " + IBD.ToParam("nombreOrganizacion") + " WHERE Identidad.PerfilID IN (SELECT Perfil.PerfilID FROM Perfil WHERE Perfil.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ") AND Identidad.Tipo >= " + (short)TiposIdentidad.ProfesionalCorporativo + " AND Identidad.Tipo < 4";

            #endregion

            #region DataAdapter

            #region Perfil

            this.sqlPerfilInsert = IBD.ReplaceParam("INSERT INTO Perfil (PerfilID, NombrePerfil, NombreOrganizacion, Eliminado, NombreCortoOrg, NombreCortoUsu, OrganizacionID, PersonaID, TieneTwitter, UsuarioTwitter, TokenTwitter, TokenSecretoTwitter, CaducidadResSusc, CurriculumID) VALUES (" + IBD.GuidParamColumnaTabla("PerfilID") + ", @NombrePerfil, @NombreOrganizacion, @Eliminado, @NombreCortoOrg, @NombreCortoUsu, " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("PersonaID") + ", @TieneTwitter, @UsuarioTwitter, @TokenTwitter, @TokenSecretoTwitter, @CaducidadResSusc, @CurriculumID)");

            this.sqlPerfilDelete = IBD.ReplaceParam("DELETE FROM Perfil WHERE (PerfilID = " + IBD.GuidParamColumnaTabla("O_PerfilID") + ")  AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + " OR " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + " IS NULL AND OrganizacionID IS NULL) AND (PersonaID = " + IBD.GuidParamColumnaTabla("O_PersonaID") + " OR " + IBD.GuidParamColumnaTabla("O_PersonaID") + " IS NULL AND PersonaID IS NULL)");

            this.sqlPerfilModify = IBD.ReplaceParam("UPDATE Perfil SET PerfilID = " + IBD.GuidParamColumnaTabla("PerfilID") + ", NombrePerfil = @NombrePerfil, NombreOrganizacion = @NombreOrganizacion, Eliminado = @Eliminado, NombreCortoOrg = @NombreCortoOrg, NombreCortoUsu = @NombreCortoUsu, OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", PersonaID = " + IBD.GuidParamColumnaTabla("PersonaID") + ", TieneTwitter = @TieneTwitter, UsuarioTwitter = @UsuarioTwitter, TokenTwitter = @TokenTwitter, TokenSecretoTwitter = @TokenSecretoTwitter, CaducidadResSusc = @CaducidadResSusc, CurriculumID = @CurriculumID WHERE (PerfilID = " + IBD.GuidParamColumnaTabla("O_PerfilID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + " OR " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + " IS NULL AND OrganizacionID IS NULL) AND (PersonaID = " + IBD.GuidParamColumnaTabla("O_PersonaID") + " OR " + IBD.GuidParamColumnaTabla("O_PersonaID") + " IS NULL AND PersonaID IS NULL)");

            #endregion

            #region PerfilRedesSociales

            this.sqlPerfilRedesSocialesInsert = IBD.ReplaceParam("INSERT INTO PerfilRedesSociales (PerfilID, NombreRedSocial, urlUsuario, Usuario, Token, TokenSecreto) VALUES (" + IBD.GuidParamColumnaTabla("PerfilID") + ", @NombreRedSocial, @urlUsuario, @Usuario, @Token, @TokenSecreto)");

            this.sqlPerfilRedesSocialesDelete = IBD.ReplaceParam("DELETE FROM PerfilRedesSociales WHERE (PerfilID = " + IBD.GuidParamColumnaTabla("O_PerfilID") + ") AND (NombreRedSocial = @O_NombreRedSocial)");

            this.sqlPerfilRedesSocialesModify = IBD.ReplaceParam("UPDATE PerfilRedesSociales SET PerfilID = " + IBD.GuidParamColumnaTabla("PerfilID") + ", NombreRedSocial = @NombreRedSocial, urlUsuario = @urlUsuario, Usuario = @Usuario, Token = @Token, TokenSecreto = @TokenSecreto WHERE (PerfilID = " + IBD.GuidParamColumnaTabla("O_PerfilID") + ") AND (NombreRedSocial = @O_NombreRedSocial)");

            #endregion

            #region PerfilGadget

            this.sqlPerfilGadgetInsert = IBD.ReplaceParam("INSERT INTO PerfilGadget (PerfilID, GadgetID, Titulo, Contenido, Orden) VALUES (" + IBD.GuidParamColumnaTabla("PerfilID") + ", " + IBD.GuidParamColumnaTabla("GadgetID") + ", @Titulo, @Contenido, @Orden)");

            this.sqlPerfilGadgetDelete = IBD.ReplaceParam("DELETE FROM PerfilGadget WHERE (PerfilID = " + IBD.GuidParamColumnaTabla("O_PerfilID") + ") AND (GadgetID = " + IBD.GuidParamColumnaTabla("O_GadgetID") + ")");

            this.sqlPerfilGadgetModify = IBD.ReplaceParam("UPDATE PerfilGadget SET PerfilID = " + IBD.GuidParamColumnaTabla("PerfilID") + ", GadgetID = " + IBD.GuidParamColumnaTabla("GadgetID") + ", Titulo = @Titulo, Contenido = @Contenido, Orden = @Orden WHERE (PerfilID = " + IBD.GuidParamColumnaTabla("O_PerfilID") + ") AND (GadgetID = " + IBD.GuidParamColumnaTabla("O_GadgetID") + ")");

            #endregion

            #region PerfilPersonaOrg

            this.sqlPerfilPersonaOrgInsert = IBD.ReplaceParam("INSERT INTO PerfilPersonaOrg (PersonaID, OrganizacionID, PerfilID) VALUES (" + IBD.GuidParamColumnaTabla("PersonaID") + ", " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("PerfilID") + ")");

            this.sqlPerfilPersonaOrgDelete = IBD.ReplaceParam("DELETE FROM PerfilPersonaOrg WHERE (PersonaID = " + IBD.GuidParamColumnaTabla("O_PersonaID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (PerfilID = " + IBD.GuidParamColumnaTabla("O_PerfilID") + ")");

            this.sqlPerfilPersonaOrgModify = IBD.ReplaceParam("UPDATE PerfilPersonaOrg SET PersonaID = " + IBD.GuidParamColumnaTabla("PersonaID") + ", OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", PerfilID = " + IBD.GuidParamColumnaTabla("PerfilID") + " WHERE (PersonaID = " + IBD.GuidParamColumnaTabla("O_PersonaID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (PerfilID = " + IBD.GuidParamColumnaTabla("O_PerfilID") + ")");

            #endregion

            #region PerfilOrganizacion

            this.sqlPerfilOrganizacionInsert = IBD.ReplaceParam("INSERT INTO PerfilOrganizacion (OrganizacionID, PerfilID) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("PerfilID") + ")");

            this.sqlPerfilOrganizacionDelete = IBD.ReplaceParam("DELETE FROM PerfilOrganizacion WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (PerfilID = " + IBD.GuidParamColumnaTabla("O_PerfilID") + ")");

            this.sqlPerfilOrganizacionModify = IBD.ReplaceParam("UPDATE PerfilOrganizacion SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", PerfilID = " + IBD.GuidParamColumnaTabla("PerfilID") + " WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (PerfilID = " + IBD.GuidParamColumnaTabla("O_PerfilID") + ")");

            #endregion

            #region PerfilPersona

            this.sqlPerfilPersonaInsert = IBD.ReplaceParam("INSERT INTO PerfilPersona (PersonaID, PerfilID) VALUES (" + IBD.GuidParamColumnaTabla("PersonaID") + ", " + IBD.GuidParamColumnaTabla("PerfilID") + ")");

            this.sqlPerfilPersonaDelete = IBD.ReplaceParam("DELETE FROM PerfilPersona WHERE (PersonaID = " + IBD.GuidParamColumnaTabla("O_PersonaID") + ") AND (PerfilID = " + IBD.GuidParamColumnaTabla("O_PerfilID") + ")");

            this.sqlPerfilPersonaModify = IBD.ReplaceParam("UPDATE PerfilPersona SET PersonaID = " + IBD.GuidParamColumnaTabla("PersonaID") + ", PerfilID = " + IBD.GuidParamColumnaTabla("PerfilID") + " WHERE (PersonaID = " + IBD.GuidParamColumnaTabla("O_PersonaID") + ") AND (PerfilID = " + IBD.GuidParamColumnaTabla("O_PerfilID") + ")");

            #endregion

            #region Identidad

            this.sqlIdentidadInsert = IBD.ReplaceParam("INSERT INTO Identidad (IdentidadID, PerfilID, OrganizacionID, ProyectoID, CurriculumID, FechaAlta, FechaBaja, NumConnexiones, Tipo, NombreCortoIdentidad, FechaExpulsion, RecibirNewsLetter,Rank,MostrarBienvenida,DiasUltActualizacion,ValorAbsoluto,ActivoEnComunidad, ActualizaHome, Foto) VALUES (" + IBD.GuidParamColumnaTabla("IdentidadID") + ", " + IBD.GuidParamColumnaTabla("PerfilID") + ", " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("CurriculumID") + ", @FechaAlta, @FechaBaja, @NumConnexiones, @Tipo, @NombreCortoIdentidad, @FechaExpulsion, @RecibirNewsLetter,@Rank,@MostrarBienvenida,@DiasUltActualizacion,@ValorAbsoluto,@ActivoEnComunidad, @ActualizaHome, @Foto)");

            this.sqlIdentidadDelete = IBD.ReplaceParam("DELETE FROM Identidad WHERE (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ")");

            this.sqlIdentidadModify = IBD.ReplaceParam("UPDATE Identidad SET IdentidadID = " + IBD.GuidParamColumnaTabla("IdentidadID") + ", PerfilID = " + IBD.GuidParamColumnaTabla("PerfilID") + ", OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", CurriculumID = " + IBD.GuidParamColumnaTabla("CurriculumID") + ", FechaAlta = @FechaAlta, FechaBaja = @FechaBaja, NumConnexiones = @NumConnexiones, Tipo = @Tipo, NombreCortoIdentidad = @NombreCortoIdentidad, FechaExpulsion = @FechaExpulsion, RecibirNewsLetter = @RecibirNewsLetter, Rank = @Rank, MostrarBienvenida = @MostrarBienvenida,DiasUltActualizacion=@DiasUltActualizacion,ValorAbsoluto=@ValorAbsoluto,ActivoEnComunidad=@ActivoEnComunidad, ActualizaHome = @ActualizaHome, Foto = @Foto WHERE (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ")");

            #endregion

            #region Profesor

            this.sqlProfesorInsert = IBD.ReplaceParam("INSERT INTO Profesor (ProfesorID, PerfilID, Email, CentroEstudios, AreaEstudios) VALUES (" + IBD.GuidParamColumnaTabla("ProfesorID") + ", " + IBD.GuidParamColumnaTabla("PerfilID") + ", @Email, @CentroEstudios, @AreaEstudios)");

            this.sqlProfesorDelete = IBD.ReplaceParam("DELETE FROM Profesor WHERE (ProfesorID = " + IBD.GuidParamColumnaTabla("O_ProfesorID") + ") AND (PerfilID = " + IBD.GuidParamColumnaTabla("O_PerfilID") + ")");

            this.sqlProfesorModify = IBD.ReplaceParam("UPDATE Profesor SET ProfesorID = " + IBD.GuidParamColumnaTabla("ProfesorID") + ", PerfilID = " + IBD.GuidParamColumnaTabla("PerfilID") + ", Email = @Email, CentroEstudios = @CentroEstudios, AreaEstudios = @AreaEstudios WHERE (ProfesorID = " + IBD.GuidParamColumnaTabla("O_ProfesorID") + ") AND (PerfilID = " + IBD.GuidParamColumnaTabla("O_PerfilID") + ")");

            #endregion

            #region GrupoIdentidades

            this.sqlGrupoIdentidadesInsert = IBD.ReplaceParam("INSERT INTO GrupoIdentidades (GrupoID, Nombre, NombreCorto, Descripcion, FechaAlta,FechaBaja,Tags,Publico, PermitirEnviarMensajes) VALUES (" + IBD.GuidParamColumnaTabla("GrupoID") + ", @Nombre, @NombreCorto, @Descripcion, @FechaAlta, @FechaBaja, @Tags, @Publico, @PermitirEnviarMensajes)");

            this.sqlGrupoIdentidadesDelete = IBD.ReplaceParam("DELETE FROM GrupoIdentidades WHERE (GrupoID = " + IBD.GuidParamColumnaTabla("O_GrupoID") + ")");

            this.sqlGrupoIdentidadesModify = IBD.ReplaceParam("UPDATE GrupoIdentidades SET GrupoID = " + IBD.GuidParamColumnaTabla("GrupoID") + ", Nombre = @Nombre, NombreCorto = @NombreCorto, Descripcion = @Descripcion, FechaAlta = @FechaAlta, FechaBaja = @FechaBaja, Tags = @Tags, Publico = @Publico, PermitirEnviarMensajes = @PermitirEnviarMensajes WHERE (GrupoID = " + IBD.GuidParamColumnaTabla("O_GrupoID") + ")");
            #endregion

            #region GrupoIdentidadesOrganizacion

            this.sqlGrupoIdentidadesOrganizacionInsert = IBD.ReplaceParam("INSERT INTO GrupoIdentidadesOrganizacion (GrupoID, OrganizacionID) VALUES (" + IBD.GuidParamColumnaTabla("GrupoID") + "," + IBD.GuidParamColumnaTabla("OrganizacionID") + ")");

            this.sqlGrupoIdentidadesOrganizacionDelete = IBD.ReplaceParam("DELETE FROM GrupoIdentidadesOrganizacion WHERE (GrupoID = " + IBD.GuidParamColumnaTabla("O_GrupoID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ")");

            this.sqlGrupoIdentidadesOrganizacionModify = IBD.ReplaceParam("UPDATE GrupoIdentidadesOrganizacion SET GrupoID = " + IBD.GuidParamColumnaTabla("GrupoID") + ", OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + " WHERE (GrupoID = " + IBD.GuidParamColumnaTabla("O_GrupoID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") ");

            #endregion

            #region GrupoIdentidadesProyecto

            this.sqlGrupoIdentidadesProyectoInsert = IBD.ReplaceParam("INSERT INTO GrupoIdentidadesProyecto (GrupoID, OrganizacionID,ProyectoID) VALUES (" + IBD.GuidParamColumnaTabla("GrupoID") + "," + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ")");

            this.sqlGrupoIdentidadesProyectoDelete = IBD.ReplaceParam("DELETE FROM GrupoIdentidadesProyecto WHERE (GrupoID = " + IBD.GuidParamColumnaTabla("O_GrupoID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND " + " (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ")");

            this.sqlGrupoIdentidadesProyectoModify = IBD.ReplaceParam("UPDATE GrupoIdentidadesProyecto SET GrupoID = " + IBD.GuidParamColumnaTabla("GrupoID") + ", OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + " WHERE (GrupoID = " + IBD.GuidParamColumnaTabla("O_GrupoID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND " + " (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") ");

            #endregion

            #region GrupoIdentidadesParticipacion

            this.sqlGrupoIdentidadesParticipacionInsert = IBD.ReplaceParam("INSERT INTO GrupoIdentidadesParticipacion (GrupoID, IdentidadID,  FechaAlta, FechaBaja) VALUES (" + IBD.GuidParamColumnaTabla("GrupoID") + ", " + IBD.GuidParamColumnaTabla("IdentidadID") + ", @FechaAlta, @FechaBaja)");

            this.sqlGrupoIdentidadesParticipacionDelete = IBD.ReplaceParam("DELETE FROM GrupoIdentidadesParticipacion WHERE (GrupoID = " + IBD.GuidParamColumnaTabla("O_GrupoID") + ") AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ")");

            this.sqlGrupoIdentidadesParticipacionModify = IBD.ReplaceParam("UPDATE GrupoIdentidadesParticipacion SET GrupoID = " + IBD.GuidParamColumnaTabla("GrupoID") + ", IdentidadID = " + IBD.GuidParamColumnaTabla("IdentidadID") + ", FechaAlta = @FechaAlta, FechaBaja = @FechaBaja WHERE (GrupoID = " + IBD.GuidParamColumnaTabla("O_GrupoID") + ") AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ")");

            #endregion

            #region DatoExtraProyectoOpcionIdentidad

            this.sqlDatoExtraProyectoOpcionIdentidadInsert = IBD.ReplaceParam("INSERT INTO DatoExtraProyectoOpcionIdentidad (OrganizacionID, ProyectoID, DatoExtraID, OpcionID, IdentidadID) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + "," + IBD.GuidParamColumnaTabla("ProyectoID") + "," + IBD.GuidParamColumnaTabla("DatoExtraID") + "," + IBD.GuidParamColumnaTabla("OpcionID") + "," + IBD.GuidParamColumnaTabla("IdentidadID") + ")");

            this.sqlDatoExtraProyectoOpcionIdentidadDelete = IBD.ReplaceParam("DELETE FROM DatoExtraProyectoOpcionIdentidad WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (DatoExtraID = " + IBD.GuidParamColumnaTabla("O_DatoExtraID") + ") AND (OpcionID = " + IBD.GuidParamColumnaTabla("O_OpcionID") + ") AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ")");

            this.sqlDatoExtraProyectoOpcionIdentidadModify = IBD.ReplaceParam("UPDATE DatoExtraProyectoOpcionIdentidad SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", DatoExtraID = " + IBD.GuidParamColumnaTabla("DatoExtraID") + ", OpcionID = " + IBD.GuidParamColumnaTabla("OpcionID") + ", IdentidadID = " + IBD.GuidParamColumnaTabla("IdentidadID") + " WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (DatoExtraID = " + IBD.GuidParamColumnaTabla("O_DatoExtraID") + ") AND (OpcionID = " + IBD.GuidParamColumnaTabla("O_OpcionID") + ") AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ")");

            #endregion

            #region DatoExtraProyectoVirtuosoIdentidad

            this.sqlDatoExtraProyectoVirtuosoIdentidadInsert = IBD.ReplaceParam("INSERT INTO DatoExtraProyectoVirtuosoIdentidad (OrganizacionID, ProyectoID, DatoExtraID, Opcion, IdentidadID) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + "," + IBD.GuidParamColumnaTabla("ProyectoID") + "," + IBD.GuidParamColumnaTabla("DatoExtraID") + "," + IBD.ToParam("Opcion") + "," + IBD.GuidParamColumnaTabla("IdentidadID") + ")");

            this.sqlDatoExtraProyectoVirtuosoIdentidadDelete = IBD.ReplaceParam("DELETE FROM DatoExtraProyectoVirtuosoIdentidad WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (DatoExtraID = " + IBD.GuidParamColumnaTabla("O_DatoExtraID") + ") AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ")");

            this.sqlDatoExtraProyectoVirtuosoIdentidadModify = IBD.ReplaceParam("UPDATE DatoExtraProyectoVirtuosoIdentidad SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", DatoExtraID = " + IBD.GuidParamColumnaTabla("DatoExtraID") + ", Opcion = " + IBD.ToParam("Opcion") + ", IdentidadID = " + IBD.GuidParamColumnaTabla("IdentidadID") + " WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (DatoExtraID = " + IBD.GuidParamColumnaTabla("O_DatoExtraID") + ") AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ")");

            #endregion

            #region DatoExtraEcosistemaOpcionPerfil

            this.sqlDatoExtraEcosistemaOpcionPerfilInsert = IBD.ReplaceParam("INSERT INTO DatoExtraEcosistemaOpcionPerfil (DatoExtraID, OpcionID, PerfilID) VALUES (" + IBD.GuidParamColumnaTabla("DatoExtraID") + "," + IBD.GuidParamColumnaTabla("OpcionID") + "," + IBD.GuidParamColumnaTabla("PerfilID") + ")");

            this.sqlDatoExtraEcosistemaOpcionPerfilDelete = IBD.ReplaceParam("DELETE FROM DatoExtraEcosistemaOpcionPerfil WHERE (DatoExtraID = " + IBD.GuidParamColumnaTabla("O_DatoExtraID") + ") AND (OpcionID = " + IBD.GuidParamColumnaTabla("O_OpcionID") + ") AND (PerfilID = " + IBD.GuidParamColumnaTabla("O_PerfilID") + ")");

            this.sqlDatoExtraEcosistemaOpcionPerfilModify = IBD.ReplaceParam("UPDATE DatoExtraEcosistemaOpcionPerfil SET DatoExtraID = " + IBD.GuidParamColumnaTabla("DatoExtraID") + ", OpcionID = " + IBD.GuidParamColumnaTabla("OpcionID") + ", PerfilID = " + IBD.GuidParamColumnaTabla("PerfilID") + " WHERE (DatoExtraID = " + IBD.GuidParamColumnaTabla("O_DatoExtraID") + ") AND (OpcionID = " + IBD.GuidParamColumnaTabla("O_OpcionID") + ") AND (PerfilID = " + IBD.GuidParamColumnaTabla("O_PerfilID") + ")");

            #endregion

            #region DatoExtraEcosistemaVirtuosoPerfil

            this.sqlDatoExtraEcosistemaVirtuosoPerfilInsert = IBD.ReplaceParam("INSERT INTO DatoExtraEcosistemaVirtuosoPerfil (DatoExtraID, Opcion, PerfilID) VALUES (" + IBD.GuidParamColumnaTabla("DatoExtraID") + "," + IBD.ToParam("Opcion") + "," + IBD.GuidParamColumnaTabla("PerfilID") + ")");

            this.sqlDatoExtraEcosistemaVirtuosoPerfilDelete = IBD.ReplaceParam("DELETE FROM DatoExtraEcosistemaVirtuosoPerfil WHERE (DatoExtraID = " + IBD.GuidParamColumnaTabla("O_DatoExtraID") + ") AND (PerfilID = " + IBD.GuidParamColumnaTabla("O_PerfilID") + ")");

            this.sqlDatoExtraEcosistemaVirtuosoPerfilModify = IBD.ReplaceParam("UPDATE DatoExtraEcosistemaVirtuosoPerfil SET DatoExtraID = " + IBD.GuidParamColumnaTabla("DatoExtraID") + ", Opcion = " + IBD.ToParam("Opcion") + ", PerfilID = " + IBD.GuidParamColumnaTabla("PerfilID") + " WHERE (DatoExtraID = " + IBD.GuidParamColumnaTabla("O_DatoExtraID") + ") AND (PerfilID = " + IBD.GuidParamColumnaTabla("O_PerfilID") + ")");

            #endregion

            #region IdentidadContadores
            sqlIdentidadContadoresInsert = IBD.ReplaceParam("INSERT INTO IdentidadContadores (IdentidadID, NumeroVisitas, NumeroDescargas) VALUES (" + IBD.GuidParamColumnaTabla("IdentidadID") + ", @NumeroVisitas, @NumeroDescargas)");
            sqlIdentidadContadoresDelete = IBD.ReplaceParam("DELETE FROM IdentidadContadores WHERE (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ")");
            sqlIdentidadContadoresModify = IBD.ReplaceParam("UPDATE IdentidadContadores SET IdentidadID = " + IBD.GuidParamColumnaTabla("IdentidadID") + ", NumeroVisitas = @NumeroVisitas, NumeroDescargas = @NumeroDescargas WHERE (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ")");
            #endregion

            #region IdentidadContadoresRecursos
            sqlIdentidadContadoresRecursosInsert = IBD.ReplaceParam("INSERT INTO IdentidadContadoresRecursos (IdentidadID, Tipo, NombreSem,Publicados,Compartidos,Comentarios) VALUES (" + IBD.GuidParamColumnaTabla("IdentidadID") + ", @Tipo, @NombreSem, @Publicados, @Compartidos, @Comentarios)");
            sqlIdentidadContadoresRecursosDelete = IBD.ReplaceParam("DELETE FROM IdentidadContadoresRecursos WHERE (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ") AND (Tipo = @O_Tipo) AND (NombreSem = @O_NombreSem) ");
            sqlIdentidadContadoresRecursosModify = IBD.ReplaceParam("UPDATE IdentidadContadoresRecursos SET IdentidadID = " + IBD.GuidParamColumnaTabla("IdentidadID") + ", Tipo = @Tipo, NombreSem = @NombreSem, Publicados = @Publicados, Compartidos = @Compartidos, Comentarios = @Comentarios WHERE (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ") AND (Tipo = @O_Tipo) AND (NombreSem = @O_NombreSem)");
            #endregion

            #endregion
        }



        #endregion

        #endregion
    }
}
