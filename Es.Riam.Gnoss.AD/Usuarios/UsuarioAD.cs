using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Suscripcion;
using Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro;
using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios.Model;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Persona = Es.Riam.Gnoss.AD.EntityModel.Models.PersonaDS.Persona;

namespace Es.Riam.Gnoss.AD.Usuarios
{
    /// <summary>
    /// Tipos de clausulas adicionales para el registro de un proyecto.
    /// </summary>
    public enum TipoClausulaAdicional
    {
        /// <summary>
        /// Obligatoria.
        /// </summary>
        Obligatoria = 0,
        /// <summary>
        /// Opcional.
        /// </summary>
        Opcional = 1,
        /// <summary>
        /// Mensaje para usuarios logueados.
        /// </summary>
        MensajeLoguado = 2,
        /// <summary>
        /// Clausulas texto para el registro.
        /// </summary>
        ClausulasTexo = 3,
        /// <summary>
        /// Condiciones de uso.
        /// </summary>
        CondicionesUso = 4,
        /// <summary>
        /// Título de las clausulas texto para el registro.
        /// </summary>
        TituloClausulasTexo = 5,
        /// <summary>
        /// Título de las condiciones de uso.
        /// </summary>
        TituloCondicionesUso = 6,
        /// <summary>
        /// Texto para la personalización del mensaje de bienvenida a los usuario registrados con el API de autenticación.
        /// </summary>
        MensajeEstablecerPassAPIAutenticacion = 7,
        /// <summary>
        /// Texto para la personalización del mensaje de aviso de la política de cookies
        /// </summary>
        PoliticaCookiesCabecera = 8,
        /// <summary>
        /// Texto o URL para la personalización del mensaje de la página de la política de cookies
        /// </summary>
        PoliticaCookiesUrlPagina = 9,
        /// <summary>
        /// Texto para aceptar condiciones de uso
        /// </summary>
        AceptarCondicionesUso = 10,
        /// <summary>
        /// Texto para aceptar política de privacidad
        /// </summary>
        AceptarPoliticaPrivacidad = 11,
        /// <summary>
        /// Texto para aceptar condiciones de uso y política de privacidad
        /// </summary>
        AceptarCondicionesYPolitica = 12
    }

    public enum ValidacionUsuario
    {
        SinVerificar = 0,
        Verificado = 1,
        SinVerificarConAcceso
    }

    public enum TipoRedSocialLogin
    {
        /// <summary>
        /// Gnoss
        /// </summary>
        Gnoss = 0,
        /// <summary>
        /// Facebook
        /// </summary>
        Facebook = 1,
        /// <summary>
        /// Twitter
        /// </summary>
        Twitter = 2,
        /// <summary>
        /// Google
        /// </summary>
        Google = 3,
        /// <summary>
        /// Santillana
        /// </summary>
        Santillana = 4,
        /// <summary>
        /// Otros tipos de red social
        /// </summary>
        Otros = 5,
        /// <summary>
        /// SharePoint
        /// </summary>
        Sharepoint = 6,
        /// <summary>
        /// SharePoint
        /// </summary>
        SharepointRefresh = 7,
        /// <summary>
        /// Apple
        /// </summary>
        Apple = 8,
        /// <summary>
        /// Keycloak
        /// </summary>
        Keycloak = 9
    }

    /// <summary>
    /// Data adapter de usuario
    /// </summary>
    /// 
    public class JoinInicioSesionOrganizacionRolUsuario
    {
        public InicioSesion InicioSesion { get; set; }
        public OrganizacionRolUsuario OrganizacionRolUsuario { get; set; }
    }

    public class JoinHistoricoProyectoUsuarioOrganizacionRolUsuario
    {
        public HistoricoProyectoUsuario HistoricoProyectoUsuario { get; set; }
        public OrganizacionRolUsuario OrganizacionRolUsuario { get; set; }
    }

    public class JoinPersonaPersonaVinculoOrganizacion
    {
        public Persona Persona { get; set; }
        public PersonaVinculoOrganizacion PersonaVinculoOrganizacion { get; set; }
    }

    public class JoinOrganizacionRolUsuarioPerfilPersonaOrg
    {
        public OrganizacionRolUsuario OrganizacionRolUsuario { get; set; }
        public PerfilPersonaOrg PerfilPersonaOrg { get; set; }
    }

    public class JoinDocumentoComentarioComentario
    {
        public DocumentoComentario DocumentoComentario { get; set; }
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
    }

    public class JoinDocumentoComentarioComentarioIdentidad
    {
        public DocumentoComentario DocumentoComentario { get; set; }
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinDocumentoComentarioComentarioIdentidadPerfil
    {
        public DocumentoComentario DocumentoComentario { get; set; }
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinDocumentoComentarioComentarioIdentidadPerfilPersona
    {
        public DocumentoComentario DocumentoComentario { get; set; }
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public Persona Persona { get; set; }
    }

    public class JoinVotoDocumentoVoto
    {
        public VotoDocumento VotoDocumento { get; set; }
        public EntityModel.Models.Voto.Voto Voto { get; set; }
    }

    public class JoinVotoDocumentoVotoIdentidad
    {
        public VotoDocumento VotoDocumento { get; set; }
        public EntityModel.Models.Voto.Voto Voto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinVotoDocumentoVotoIdentidadPerfil
    {
        public VotoDocumento VotoDocumento { get; set; }
        public EntityModel.Models.Voto.Voto Voto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinVotoDocumentoVotoIdentidadPerfilPersona
    {
        public VotoDocumento VotoDocumento { get; set; }
        public EntityModel.Models.Voto.Voto Voto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public Persona Persona { get; set; }
    }

    public class JoinTesauroUsuarioCategoriaTesauro
    {
        public TesauroUsuario TesauroUsuario { get; set; }
        public CategoriaTesauro CategoriaTesauro { get; set; }
    }

    public class JoinProyectoUsuarioIdentidadPersona
    {
        public ProyectoUsuarioIdentidad ProyectoUsuarioIdentidad { get; set; }
        public Persona Persona { get; set; }
    }

    public class JoinProyectoUsuarioIdentidadPersonaPerfil
    {
        public ProyectoUsuarioIdentidad ProyectoUsuarioIdentidad { get; set; }
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinInicioSesionPersona
    {
        public InicioSesion InicioSesion { get; set; }
        public Persona Persona { get; set; }
    }

    public class JoinInicioSesionPersonaPerfil
    {
        public InicioSesion InicioSesion { get; set; }
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinHistoricoProyectoUsuarioPersona
    {
        public HistoricoProyectoUsuario HistoricoProyectoUsuario { get; set; }
        public Persona Persona { get; set; }
    }

    public class JoinHistoricoProyectoUsuarioPersonaPerfil
    {
        public HistoricoProyectoUsuario HistoricoProyectoUsuario { get; set; }
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinProyRolUsuClausulaRegPersona
    {
        public ProyRolUsuClausulaReg ProyRolUsuClausulaReg { get; set; }
        public Persona Persona { get; set; }
    }

    public class JoinProyRolUsuClausulaRegPersonaPerfil
    {
        public ProyRolUsuClausulaReg ProyRolUsuClausulaReg { get; set; }
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinUsuarioContadoresPersona
    {
        public UsuarioContadores UsuarioContadores { get; set; }
        public Persona Persona { get; set; }
    }

    public class JoinUsuarioContadoresPersonaPerfil
    {
        public UsuarioContadores UsuarioContadores { get; set; }
        public Persona Persona { get; set; }

        public Perfil Perfil { get; set; }
    }

    public class JoinUsuarioContadoresPersonaPerfilIdentidad
    {
        public UsuarioContadores UsuarioContadores { get; set; }
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinDocumentoComentarioComentarioProyecto
    {
        public DocumentoComentario DocumentoComentario { get; set; }
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.ProyectoDS.Proyecto Proyecto { get; set; }
    }

    public class JoinAmigoIdentidad
    {
        public Amigo Amigo { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinAmigoIdentidadPerfil
    {
        public Amigo Amigo { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinAmigoIdentidadPerfilIdentidadUsuarios
    {
        public Amigo Amigo { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentidadUsuarios { get; set; }
    }

    public class JoinAmigoIdentidadPerfilIdentidadUsuariosPerfilUsuarios
    {
        public Amigo Amigo { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentidadUsuarios { get; set; }
        public Perfil PerfilUsuarios { get; set; }
    }

    public class JoinAmigoIdentidadPerfilIdentidadUsuariosPerfilUsuariosPersona
    {
        public Amigo Amigo { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentidadUsuarios { get; set; }
        public Perfil PerfilUsuarios { get; set; }
        public Persona Persona { get; set; }
    }

    public class JoinSuscripcionTesauroUsuarioSuscripcion
    {
        public SuscripcionTesauroUsuario SuscripcionTesauroUsuario { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
    }

    public class JoinSuscripcionTesauroUsuarioSuscripcionIdentidad
    {
        public SuscripcionTesauroUsuario SuscripcionTesauroUsuario { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinSuscripcionTesauroUsuarioSuscripcionIdentidadPerfil
    {
        public SuscripcionTesauroUsuario SuscripcionTesauroUsuario { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinSuscripcionTesauroUsuarioSuscripcionIdentidadPerfilPersona
    {
        public SuscripcionTesauroUsuario SuscripcionTesauroUsuario { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public Persona Persona { get; set; }
    }

    public class JoinSuscripcionTesauroUsuarioSuscripcionIdentidadPerfilPersonaPersonaAutor
    {
        public SuscripcionTesauroUsuario SuscripcionTesauroUsuario { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public Persona Persona { get; set; }
        public Persona PersonaAutor { get; set; }
    }

    public class JoinSuscripcionTesauroUsuarioSuscripcionIdentidadPerfilPersonaPersonaAutorPerfilAutor
    {
        public SuscripcionTesauroUsuario SuscripcionTesauroUsuario { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public Persona Persona { get; set; }
        public Persona PersonaAutor { get; set; }
        public Perfil PerfilAutor { get; set; }
    }

    public class JoinSuscripcionIdentidadProyectoSuscripcion
    {
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
    }

    public class JoinSuscripcionIdentidadProyectoSuscripcionIdentidad
    {
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinSuscripcionIdentidadProyectoSuscripcionIdentidadPerfil
    {
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinSuscripcionIdentidadProyectoSuscripcionIdentidadPerfilUsuario
    {
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public Usuario Usuario { get; set; }
    }

    public class JoinSuscripcionIdentidadProyectoSuscripcionIdentidadPerfilPersona
    {
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public Persona Persona { get; set; }
    }

    public class JoinSuscripcionIdentidadProyectoSuscripcionIdentidadPerfilPersonaIdentidadAutor
    {
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public Persona Persona { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentidadAutor { get; set; }
    }

    public static class JoinsUsuario
    {
        public static IQueryable<JoinSuscripcionIdentidadProyectoSuscripcionIdentidadPerfilPersonaIdentidadAutor> JoinIdentidadAutor(this IQueryable<JoinSuscripcionIdentidadProyectoSuscripcionIdentidadPerfilPersona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.SuscripcionIdentidadProyecto.IdentidadID, identidad => identidad.IdentidadID, (item, identidadAutor) => new JoinSuscripcionIdentidadProyectoSuscripcionIdentidadPerfilPersonaIdentidadAutor
            {
                Identidad = item.Identidad,
                IdentidadAutor = identidadAutor,
                Perfil = item.Perfil,
                Persona = item.Persona,
                Suscripcion = item.Suscripcion,
                SuscripcionIdentidadProyecto = item.SuscripcionIdentidadProyecto
            });
        }

        public static IQueryable<JoinSuscripcionIdentidadProyectoSuscripcionIdentidadPerfilPersona> JoinPersona(this IQueryable<JoinSuscripcionIdentidadProyectoSuscripcionIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Persona, item => item.Perfil.PersonaID, persona => persona.PersonaID, (item, persona) => new JoinSuscripcionIdentidadProyectoSuscripcionIdentidadPerfilPersona
            {
                Persona = persona,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Suscripcion = item.Suscripcion,
                SuscripcionIdentidadProyecto = item.SuscripcionIdentidadProyecto
            });
        }

        public static IQueryable<JoinSuscripcionIdentidadProyectoSuscripcionIdentidadPerfil> JoinPerfil(this IQueryable<JoinSuscripcionIdentidadProyectoSuscripcionIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new JoinSuscripcionIdentidadProyectoSuscripcionIdentidadPerfil
            {
                Perfil = perfil,
                Identidad = item.Identidad,
                Suscripcion = item.Suscripcion,
                SuscripcionIdentidadProyecto = item.SuscripcionIdentidadProyecto
            });
        }
        public static IQueryable<JoinSuscripcionIdentidadProyectoSuscripcionIdentidadPerfilUsuario> JoinUsuario(this IQueryable<JoinSuscripcionIdentidadProyectoSuscripcionIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Usuario, item => item.Perfil.NombreCortoUsu, usuario => usuario.NombreCorto, (item, usuario) => new JoinSuscripcionIdentidadProyectoSuscripcionIdentidadPerfilUsuario
            {
                Usuario = usuario,
                Perfil = item.Perfil,
                Identidad = item.Identidad,
                Suscripcion = item.Suscripcion,
                SuscripcionIdentidadProyecto = item.SuscripcionIdentidadProyecto
            });
        }

        public static IQueryable<JoinSuscripcionIdentidadProyectoSuscripcionIdentidad> JoinIdentidad(this IQueryable<JoinSuscripcionIdentidadProyectoSuscripcion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Suscripcion.IdentidadID, identidad => identidad.IdentidadID, (item, identidad) => new JoinSuscripcionIdentidadProyectoSuscripcionIdentidad
            {
                Identidad = identidad,
                Suscripcion = item.Suscripcion,
                SuscripcionIdentidadProyecto = item.SuscripcionIdentidadProyecto
            });
        }

        public static IQueryable<JoinSuscripcionIdentidadProyectoSuscripcion> JoinSuscripcion(this IQueryable<SuscripcionIdentidadProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Suscripcion, suscripcionIdentidadProyecto => suscripcionIdentidadProyecto.SuscripcionID, suscripcion => suscripcion.SuscripcionID, (suscripcionIdentidadProyecto, suscripcion) => new JoinSuscripcionIdentidadProyectoSuscripcion
            {
                Suscripcion = suscripcion,
                SuscripcionIdentidadProyecto = suscripcionIdentidadProyecto
            });
        }

        public static IQueryable<JoinSuscripcionTesauroUsuarioSuscripcionIdentidadPerfilPersonaPersonaAutorPerfilAutor> JoinPerfilAutor(this IQueryable<JoinSuscripcionTesauroUsuarioSuscripcionIdentidadPerfilPersonaPersonaAutor> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.PersonaAutor.PersonaID, perfil => perfil.PersonaID, (item, perfilAutor) => new JoinSuscripcionTesauroUsuarioSuscripcionIdentidadPerfilPersonaPersonaAutorPerfilAutor
            {
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                PerfilAutor = perfilAutor,
                Persona = item.Persona,
                PersonaAutor = item.PersonaAutor,
                Suscripcion = item.Suscripcion,
                SuscripcionTesauroUsuario = item.SuscripcionTesauroUsuario
            });
        }

        public static IQueryable<JoinSuscripcionTesauroUsuarioSuscripcionIdentidadPerfilPersonaPersonaAutor> JoinPersonaAutor(this IQueryable<JoinSuscripcionTesauroUsuarioSuscripcionIdentidadPerfilPersona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Persona, item => item.SuscripcionTesauroUsuario.UsuarioID, persona => persona.PersonaID, (item, personaAutor) => new JoinSuscripcionTesauroUsuarioSuscripcionIdentidadPerfilPersonaPersonaAutor
            {
                Persona = item.Persona,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                PersonaAutor = personaAutor,
                Suscripcion = item.Suscripcion,
                SuscripcionTesauroUsuario = item.SuscripcionTesauroUsuario
            });
        }

        public static IQueryable<JoinSuscripcionTesauroUsuarioSuscripcionIdentidadPerfilPersona> JoinPersona(this IQueryable<JoinSuscripcionTesauroUsuarioSuscripcionIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Persona, item => item.Perfil.PersonaID, persona => persona.PersonaID, (item, persona) => new JoinSuscripcionTesauroUsuarioSuscripcionIdentidadPerfilPersona
            {
                Persona = persona,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Suscripcion = item.Suscripcion,
                SuscripcionTesauroUsuario = item.SuscripcionTesauroUsuario
            });
        }

        public static IQueryable<JoinSuscripcionTesauroUsuarioSuscripcionIdentidadPerfil> JoinPerfil(this IQueryable<JoinSuscripcionTesauroUsuarioSuscripcionIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new JoinSuscripcionTesauroUsuarioSuscripcionIdentidadPerfil
            {
                Perfil = perfil,
                Identidad = item.Identidad,
                Suscripcion = item.Suscripcion,
                SuscripcionTesauroUsuario = item.SuscripcionTesauroUsuario
            });
        }

        public static IQueryable<JoinSuscripcionTesauroUsuarioSuscripcionIdentidad> JoinIdentidad(this IQueryable<JoinSuscripcionTesauroUsuarioSuscripcion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Suscripcion.IdentidadID, identidad => identidad.IdentidadID, (item, identidad) => new JoinSuscripcionTesauroUsuarioSuscripcionIdentidad
            {
                Identidad = identidad,
                Suscripcion = item.Suscripcion,
                SuscripcionTesauroUsuario = item.SuscripcionTesauroUsuario
            });
        }

        public static IQueryable<JoinSuscripcionTesauroUsuarioSuscripcion> JoinSuscripcion(this IQueryable<SuscripcionTesauroUsuario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Suscripcion, suscripcionTesauroUsuario => suscripcionTesauroUsuario.SuscripcionID, suscripcion => suscripcion.SuscripcionID, (suscripcionTesauroUsuario, suscripcion) => new JoinSuscripcionTesauroUsuarioSuscripcion
            {
                SuscripcionTesauroUsuario = suscripcionTesauroUsuario,
                Suscripcion = suscripcion
            });
        }

        public static IQueryable<JoinAmigoIdentidadPerfilIdentidadUsuariosPerfilUsuariosPersona> JoinPersona(this IQueryable<JoinAmigoIdentidadPerfilIdentidadUsuariosPerfilUsuarios> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Persona, item => item.PerfilUsuarios.PersonaID, persona => persona.PersonaID, (item, persona) => new JoinAmigoIdentidadPerfilIdentidadUsuariosPerfilUsuariosPersona
            {
                Amigo = item.Amigo,
                Identidad = item.Identidad,
                IdentidadUsuarios = item.IdentidadUsuarios,
                Perfil = item.Perfil,
                PerfilUsuarios = item.PerfilUsuarios,
                Persona = persona
            });
        }

        public static IQueryable<JoinAmigoIdentidadPerfilIdentidadUsuariosPerfilUsuarios> JoinPerfilUsuarios(this IQueryable<JoinAmigoIdentidadPerfilIdentidadUsuarios> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.IdentidadUsuarios.PerfilID, perfil => perfil.PerfilID, (item, perfilUsuarios) => new JoinAmigoIdentidadPerfilIdentidadUsuariosPerfilUsuarios
            {
                Perfil = item.Perfil,
                Amigo = item.Amigo,
                Identidad = item.Identidad,
                IdentidadUsuarios = item.IdentidadUsuarios,
                PerfilUsuarios = perfilUsuarios
            });
        }

        public static IQueryable<JoinAmigoIdentidadPerfilIdentidadUsuarios> JoinIdentidadUsuarios(this IQueryable<JoinAmigoIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Amigo.IdentidadID, identidad => identidad.IdentidadID, (item, identidadUsuarios) => new JoinAmigoIdentidadPerfilIdentidadUsuarios
            {
                Identidad = item.Identidad,
                Amigo = item.Amigo,
                IdentidadUsuarios = identidadUsuarios,
                Perfil = item.Perfil
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

        public static IQueryable<JoinAmigoIdentidad> JoinIdentidad(this IQueryable<Amigo> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, amigo => amigo.IdentidadAmigoID, identidad => identidad.IdentidadID, (amigo, identidad) => new JoinAmigoIdentidad
            {
                Identidad = identidad,
                Amigo = amigo
            });
        }

        public static IQueryable<JoinDocumentoComentarioComentarioProyecto> JoinProyecto(this IQueryable<JoinDocumentoComentarioComentario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Proyecto, item => item.DocumentoComentario.ProyectoID, proyecto => proyecto.ProyectoID, (item, proyecto) => new JoinDocumentoComentarioComentarioProyecto
            {
                Proyecto = proyecto,
                Comentario = item.Comentario,
                DocumentoComentario = item.DocumentoComentario
            });
        }

        public static IQueryable<JoinUsuarioContadoresPersonaPerfilIdentidad> JoinIdentidad(this IQueryable<JoinUsuarioContadoresPersonaPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Perfil.PerfilID, identidad => identidad.PerfilID, (item, identidad) => new JoinUsuarioContadoresPersonaPerfilIdentidad
            {
                Identidad = identidad,
                Perfil = item.Perfil,
                Persona = item.Persona,
                UsuarioContadores = item.UsuarioContadores
            });
        }

        public static IQueryable<JoinUsuarioContadoresPersonaPerfil> JoinPerfil(this IQueryable<JoinUsuarioContadoresPersona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Persona.PersonaID, perfil => perfil.PersonaID, (item, perfil) => new JoinUsuarioContadoresPersonaPerfil
            {
                Perfil = perfil,
                Persona = item.Persona,
                UsuarioContadores = item.UsuarioContadores
            });
        }

        public static IQueryable<JoinUsuarioContadoresPersona> JoinPersona(this IQueryable<UsuarioContadores> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Persona, usuarioContadores => usuarioContadores.UsuarioID, persona => persona.UsuarioID, (usuarioContadores, persona) => new JoinUsuarioContadoresPersona
            {
                Persona = persona,
                UsuarioContadores = usuarioContadores
            });
        }

        public static IQueryable<JoinProyRolUsuClausulaRegPersona> JoinPersona(this IQueryable<ProyRolUsuClausulaReg> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Persona, proyRolUsuClausulaReg => proyRolUsuClausulaReg.UsuarioID, persona => persona.UsuarioID, (proyRolUsuClausulaReg, persona) => new JoinProyRolUsuClausulaRegPersona
            {
                Persona = persona,
                ProyRolUsuClausulaReg = proyRolUsuClausulaReg
            });
        }

        public static IQueryable<JoinProyRolUsuClausulaRegPersonaPerfil> JoinPerfil(this IQueryable<JoinProyRolUsuClausulaRegPersona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Persona.PersonaID, perfil => perfil.PersonaID, (item, perfil) => new JoinProyRolUsuClausulaRegPersonaPerfil
            {
                Persona = item.Persona,
                ProyRolUsuClausulaReg = item.ProyRolUsuClausulaReg,
                Perfil = perfil
            });
        }

        public static IQueryable<JoinHistoricoProyectoUsuarioPersonaPerfil> JoinPerfil(this IQueryable<JoinHistoricoProyectoUsuarioPersona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Persona.PersonaID, perfil => perfil.PersonaID, (item, perfil) => new JoinHistoricoProyectoUsuarioPersonaPerfil
            {
                Persona = item.Persona,
                HistoricoProyectoUsuario = item.HistoricoProyectoUsuario,
                Perfil = perfil
            });
        }

        public static IQueryable<JoinHistoricoProyectoUsuarioPersona> JoinPersona(this IQueryable<HistoricoProyectoUsuario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Persona, historicoProyectoUsuario => historicoProyectoUsuario.UsuarioID, persona => persona.UsuarioID, (historicoProyectoUsuario, persona) => new JoinHistoricoProyectoUsuarioPersona
            {
                Persona = persona,
                HistoricoProyectoUsuario = historicoProyectoUsuario
            });
        }

        public static IQueryable<JoinInicioSesionPersonaPerfil> JoinPerfil(this IQueryable<JoinInicioSesionPersona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Persona.PersonaID, perfil => perfil.PersonaID, (item, perfil) => new JoinInicioSesionPersonaPerfil
            {
                Persona = item.Persona,
                InicioSesion = item.InicioSesion,
                Perfil = perfil
            });
        }

        public static IQueryable<JoinInicioSesionPersona> JoinPersona(this IQueryable<InicioSesion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Persona, inicioSesion => inicioSesion.UsuarioID, persona => persona.UsuarioID, (inicioSesion, persona) => new JoinInicioSesionPersona
            {
                Persona = persona,
                InicioSesion = inicioSesion
            });
        }

        public static IQueryable<JoinProyectoUsuarioIdentidadPersonaPerfil> JoinPerfil(this IQueryable<JoinProyectoUsuarioIdentidadPersona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Persona.PersonaID, perfil => perfil.PersonaID, (item, perfil) => new JoinProyectoUsuarioIdentidadPersonaPerfil
            {
                Persona = item.Persona,
                ProyectoUsuarioIdentidad = item.ProyectoUsuarioIdentidad,
                Perfil = perfil
            });
        }

        public static IQueryable<JoinProyectoUsuarioIdentidadPersona> JoinPersona(this IQueryable<ProyectoUsuarioIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Persona, proyectoUsuarioIdentidad => proyectoUsuarioIdentidad.UsuarioID, persona => persona.UsuarioID, (proyectoUsuarioIdentidad, persona) => new JoinProyectoUsuarioIdentidadPersona
            {
                Persona = persona,
                ProyectoUsuarioIdentidad = proyectoUsuarioIdentidad
            });
        }

        public static IQueryable<JoinTesauroUsuarioCategoriaTesauro> JoinCategoriaTesauro(this IQueryable<TesauroUsuario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CategoriaTesauro, tesauroUsuario => tesauroUsuario.TesauroID, categoriaTesauro => categoriaTesauro.TesauroID, (tesauroUsuario, categoriaTesauro) => new JoinTesauroUsuarioCategoriaTesauro
            {
                CategoriaTesauro = categoriaTesauro,
                TesauroUsuario = tesauroUsuario
            });
        }

        public static IQueryable<JoinVotoDocumentoVotoIdentidadPerfilPersona> JoinPersona(this IQueryable<JoinVotoDocumentoVotoIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Persona, item => item.Perfil.PersonaID, persona => persona.PersonaID, (item, persona) => new JoinVotoDocumentoVotoIdentidadPerfilPersona
            {
                Voto = item.Voto,
                VotoDocumento = item.VotoDocumento,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Persona = persona
            });
        }

        public static IQueryable<JoinVotoDocumentoVotoIdentidadPerfil> JoinPerfil(this IQueryable<JoinVotoDocumentoVotoIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new JoinVotoDocumentoVotoIdentidadPerfil
            {
                Voto = item.Voto,
                VotoDocumento = item.VotoDocumento,
                Identidad = item.Identidad,
                Perfil = perfil
            });
        }

        public static IQueryable<JoinVotoDocumentoVotoIdentidad> JoinIdentidad(this IQueryable<JoinVotoDocumentoVoto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Voto.IdentidadID, identidad => identidad.IdentidadID, (item, identidad) => new JoinVotoDocumentoVotoIdentidad
            {
                Voto = item.Voto,
                VotoDocumento = item.VotoDocumento,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinVotoDocumentoVoto> JoinVoto(this IQueryable<VotoDocumento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Voto, votoDocumento => votoDocumento.VotoID, voto => voto.VotoID, (votoDocumento, voto) => new JoinVotoDocumentoVoto
            {
                Voto = voto,
                VotoDocumento = votoDocumento
            });
        }

        public static IQueryable<JoinDocumentoComentarioComentarioIdentidadPerfilPersona> JoinPersona(this IQueryable<JoinDocumentoComentarioComentarioIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Persona, item => item.Perfil.PersonaID, persona => persona.PersonaID, (item, persona) => new JoinDocumentoComentarioComentarioIdentidadPerfilPersona
            {
                Comentario = item.Comentario,
                DocumentoComentario = item.DocumentoComentario,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Persona = persona
            });
        }

        public static IQueryable<JoinDocumentoComentarioComentarioIdentidadPerfil> JoinPerfil(this IQueryable<JoinDocumentoComentarioComentarioIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new JoinDocumentoComentarioComentarioIdentidadPerfil
            {
                Comentario = item.Comentario,
                DocumentoComentario = item.DocumentoComentario,
                Identidad = item.Identidad,
                Perfil = perfil
            });
        }

        public static IQueryable<JoinDocumentoComentarioComentarioIdentidad> JoinIdentidad(this IQueryable<JoinDocumentoComentarioComentario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Comentario.IdentidadID, identidad => identidad.IdentidadID, (item, identidad) => new JoinDocumentoComentarioComentarioIdentidad
            {
                Comentario = item.Comentario,
                DocumentoComentario = item.DocumentoComentario,
                Identidad = identidad
            });
        }

        public static IQueryable<JoinDocumentoComentarioComentario> JoinComentario(this IQueryable<DocumentoComentario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Comentario, documentoComentario => documentoComentario.ComentarioID, comentario => comentario.ComentarioID, (documentoComentario, comentario) => new JoinDocumentoComentarioComentario
            {
                Comentario = comentario,
                DocumentoComentario = documentoComentario
            });
        }

        public static IQueryable<JoinOrganizacionRolUsuarioPerfilPersonaOrg> JoinPerfilPersonaOrg(this IQueryable<OrganizacionRolUsuario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.PerfilPersonaOrg, organizacionRolUsuario => organizacionRolUsuario.OrganizacionID, perfilPersonaOrg => perfilPersonaOrg.OrganizacionID, (organizacionRolUsuario, perfilPersonaOrg) => new JoinOrganizacionRolUsuarioPerfilPersonaOrg
            {
                OrganizacionRolUsuario = organizacionRolUsuario,
                PerfilPersonaOrg = perfilPersonaOrg
            });
        }

        public static IQueryable<JoinPersonaPersonaVinculoOrganizacion> JoinPersonaVinculoOrganizacion(this IQueryable<Persona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.PersonaVinculoOrganizacion, persona => persona.PersonaID, personaVinculoOrganizacion => personaVinculoOrganizacion.PersonaID, (persona, personaVinculoOrganizacion) => new JoinPersonaPersonaVinculoOrganizacion
            {
                PersonaVinculoOrganizacion = personaVinculoOrganizacion,
                Persona = persona
            });
        }

        public static IQueryable<JoinHistoricoProyectoUsuarioOrganizacionRolUsuario> JoinOrganizacionRolUsuario(this IQueryable<HistoricoProyectoUsuario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.OrganizacionRolUsuario, historicoProyectoUsuario => historicoProyectoUsuario.UsuarioID, organizacionRolUsuario => organizacionRolUsuario.UsuarioID, (historicoProyectoUsuario, organizacionRolUsuario) => new JoinHistoricoProyectoUsuarioOrganizacionRolUsuario
            {
                HistoricoProyectoUsuario = historicoProyectoUsuario,
                OrganizacionRolUsuario = organizacionRolUsuario
            });
        }

        public static IQueryable<JoinInicioSesionOrganizacionRolUsuario> JoinOrganizacionRolUsuario(this IQueryable<InicioSesion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.OrganizacionRolUsuario, inicioSesion => inicioSesion.UsuarioID, organizacionRolUsuario => organizacionRolUsuario.UsuarioID, (inicioSesion, organizacionRolUsuario) => new JoinInicioSesionOrganizacionRolUsuario
            {
                OrganizacionRolUsuario = organizacionRolUsuario,
                InicioSesion = inicioSesion
            });
        }
    }

    public class UsuarioAD : BaseAD
    {
        #region Constantes

        public const string CONTADOR_NUMERO_ACCESOS = "NumeroAccesos";

        public const string CRITERIO_BUSQUEDA_USUARIO_CONTADORES = "hour";

        public const int CANTIDAD_BUSQUEDA_USUARIO_CONTADORES = 24;

        #endregion

        private EntityContext mEntityContext;

        private LoggingService mLoggingService;

        private ConfigService mConfigService;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public UsuarioAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<UsuarioAD> logger, ILoggerFactory loggerFactory)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication,logger, loggerFactory)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD"></param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public UsuarioAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<UsuarioAD> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la fila para permisos que contiene todos sus valores SIN definir
        /// </summary>
        public static string FilaPermisosSinDefinir
        {
            get
            {
                return ("0000000000000000");
            }
        }

        /// <summary>
        /// Obtiene la fila de permisos para un administrador (todo permitido)
        /// </summary>
        public static string FilaPermisosAdministrador
        {
            get
            {
                return ("FFFFFFFFFFFFFFFF");
            }
        }

        /// <summary>
        /// Obtiene el identificador del usuario "admin"
        /// </summary>
        public static Guid Admin
        {
            get
            {
                return new Guid("11111111-1111-1111-1111-111111111111");
            }
        }

        /// <summary>
        /// Obtiene el identificador del usuario "invitado"
        /// </summary>
        public static Guid Invitado
        {
            get
            {
                return new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff");
            }
        }

        #endregion

        public List<UsuarioIdentidadPersona> ObtenerAdministradoresNombreApellidosPorOrganizacion(Guid pOrganizacionID)
        {
            return mEntityContext.Usuario.Join(mEntityContext.Persona, usuario => usuario.UsuarioID, persona => persona.UsuarioID, (usuario, persona) => new
            {
                Usuario = usuario,
                Persona = persona
            }).Join(mEntityContext.AdministradorOrganizacion, objeto => objeto.Usuario.UsuarioID, adminOrg => adminOrg.UsuarioID, (objeto, adminOrg) => new
            {
                Usuario = objeto.Usuario,
                Persona = objeto.Persona,
                AdministradorOrganizacion = adminOrg
            }).Where(objeto => objeto.AdministradorOrganizacion.OrganizacionID.Equals(pOrganizacionID)).ToList().Select(objeto => new UsuarioIdentidadPersona
            {
                UsuarioID = objeto.Usuario.UsuarioID,
                Login = objeto.Usuario.Login,
                Nombre = objeto.Persona.Nombre,
                Apellidos = objeto.Persona.Apellidos
            }).ToList();
        }
        


        #region Métodos generales

        #region Públicos

        /// <summary>
        /// Comprueba si el usuario está bloqueado en el proyecto pasado por parámetro
        /// </summary>
        /// <param name="pOrganizacionID">Idetnificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>TRUE si el usuario está bloqueado en el proyecto, FALSE en caso contrario</returns>
        public bool EstaUsuarioBloqueadoEnProyecto(Guid pOrganizacionID, Guid pProyectoID, Guid pUsuarioID)
        {
            bool estaBloqueado = true;
            ProyectoRolUsuario resultado = mEntityContext.ProyectoRolUsuario.FirstOrDefault(item => item.OrganizacionGnossID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID) && item.UsuarioID.Equals(pUsuarioID));
            if (resultado != null)
            {
                estaBloqueado = resultado.EstaBloqueado;
            }
            return estaBloqueado;
        }

        /// <summary>
        /// Comprueba si existe un usuario con el nombre corto pasado por parámetro
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto que hay que buscar</param>
        /// <returns>TRUE si existe, FALSE en caso contrario</returns>
        public bool ExisteNombreCortoEnBD(string pNombreCorto)
        {
            bool encontrado = false;
            var resultado = mEntityContext.Usuario.FirstOrDefault(usuario => usuario.NombreCorto.Equals(pNombreCorto));
            if (resultado != null)
            {
                encontrado = true;
            }
            return encontrado;
        }

        /// <summary>
        /// Obtiene los nombres cortos que empizan con los caracteres introducidos.
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto</param>
        /// <returns>nombres cortos que empizan con los caracteres introducidos</returns>
        public List<string> ObtenerNombresCortosEmpiezanPor(string pNombreCorto)
        {
            List<string> listaNombres = new List<string>();
            var resultado = mEntityContext.Usuario.Where(usuario => usuario.NombreCorto.StartsWith(pNombreCorto)).ToList();
            foreach (EntityModel.Models.UsuarioDS.Usuario filaUsu in resultado)
            {
                listaNombres.Add(UtilCadenas.RemoveAccentsWithRegEx(filaUsu.NombreCorto.ToLower()));
            }
            return listaNombres;
        }

        /// <summary>
        /// Obtiene 
        /// </summary>
        /// <param name="pUsuarioID">pUsuarioID</param>
        /// <returns>Login que empizan con los caracteres introducidos</returns>
        public string ObtenerUrlRedirect(Guid pUsuarioID)
        {
            return mEntityContext.UsuarioRedirect.Where(usuarioRedirect => usuarioRedirect.UsuarioID.Equals(pUsuarioID)).Select(usuario => usuario.UrlRedirect).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene 
        /// </summary>
        /// <param name="pUsuarioID">pUsuarioID</param>
        /// <returns>Login que empizan con los caracteres introducidos</returns>
        public void EliminarUrlRedirect(Guid pUsuarioID)
        {
            UsuarioRedirect usuarioRedirect = mEntityContext.UsuarioRedirect.FirstOrDefault(usuario => usuario.UsuarioID.Equals(pUsuarioID));
            if (usuarioRedirect != null)
            {
                mEntityContext.Entry(usuarioRedirect).State = EntityState.Deleted;
                mEntityContext.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene 
        /// </summary>
        /// <param name="pUsuarioID">pUsuarioID</param>
        /// <returns>Login que empizan con los caracteres introducidos</returns>
        public void ModificarUrlRedirect(Guid pUsuarioID, string pRedirect)
        {
            UsuarioRedirect usuarioRedirect = mEntityContext.UsuarioRedirect.FirstOrDefault(usuario => usuario.UsuarioID.Equals(pUsuarioID));
            if (usuarioRedirect != null)
            {
                usuarioRedirect.UrlRedirect = pRedirect;
                mEntityContext.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene 
        /// </summary>
        /// <param name="pUsuarioID">pUsuarioID</param>
        /// <returns>Login que empizan con los caracteres introducidos</returns>
        public void InsertarUrlRedirect(Guid pUsuarioID, string pRedirect)
        {
            UsuarioRedirect usuarioRedirect = new UsuarioRedirect();
            usuarioRedirect.UsuarioID = pUsuarioID;
            usuarioRedirect.UrlRedirect = pRedirect;
            mEntityContext.UsuarioRedirect.Add(usuarioRedirect);
            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Obtiene los Login que empizan con los caracteres introducidos.
        /// </summary>
        /// <param name="pLogin">Login</param>
        /// <returns>Login que empizan con los caracteres introducidos</returns>
        public List<string> ObtenerLoginEmpiezanPor(string pLogin)
        {
            var resultado = mEntityContext.Usuario.Where(item => item.Login.StartsWith(pLogin)).ToList();

            List<string> listaNombres = new List<string>();
            foreach (var filaUsu in resultado)
            {
                listaNombres.Add(filaUsu.Login);
            }
            return listaNombres;
        }

        private class UsuarioIDPerfilID
        {
            public Guid UsuarioID { get; set; }
            public Guid PerfilID { get; set; }
        }

        /// <summary>
        /// Obtiene los usuarios que tienen como contacto al perfil de un usuario o lo siguen
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil del usuario</param>
        /// <param name="pListaUsuarios">Lista inicializada que se va a rellenar con los datos obtenidos de la consulta (lista de string si se busca todos los usuarios o lista de Guid si se filtra por caracteres)</param>
        public void ObtenerUsuariosSonContactoOSeguidorDePerfilUsuario(Guid pPerfilID, Dictionary<Guid, List<Guid>> pListaUsuarios)
        {
            List<UsuarioIDPerfilID> listaContactos = mEntityContext.Amigo.JoinIdentidad().JoinPerfil().JoinIdentidadUsuarios().JoinPerfilUsuarios().JoinPersona().Where(item => item.Perfil.PerfilID.Equals(pPerfilID) && item.Persona.UsuarioID.HasValue && (!item.PerfilUsuarios.OrganizacionID.HasValue || !item.Perfil.OrganizacionID.HasValue || !item.PerfilUsuarios.OrganizacionID.Value.Equals(item.Perfil.OrganizacionID.Value)) && item.PerfilUsuarios.Eliminado.Equals(false) && !item.IdentidadUsuarios.FechaBaja.HasValue).Select(item => new UsuarioIDPerfilID { UsuarioID = item.Persona.UsuarioID.Value, PerfilID = item.PerfilUsuarios.PerfilID }).ToList();

            ObtenerListadoIdsDeQuery(listaContactos, pListaUsuarios);

            List<UsuarioIDPerfilID> listaSeguidores1 = mEntityContext.SuscripcionTesauroUsuario.JoinSuscripcion().JoinIdentidad().JoinPerfil().JoinPersona().JoinPersonaAutor().JoinPerfilAutor().Where(item => item.PerfilAutor.PerfilID.Equals(pPerfilID) && item.Perfil.Eliminado.Equals(false) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue).Select(item => new UsuarioIDPerfilID { UsuarioID = item.Persona.UsuarioID.Value, PerfilID = item.Identidad.PerfilID }).ToList();

            ObtenerListadoIdsDeQuery(listaSeguidores1, pListaUsuarios);

            List<UsuarioIDPerfilID> listaSeguidores2 = mEntityContext.SuscripcionIdentidadProyecto.JoinSuscripcion().JoinIdentidad().JoinPerfil().JoinPersona().JoinIdentidadAutor().Where(item => item.IdentidadAutor.PerfilID.Equals(pPerfilID) && item.Perfil.Eliminado.Equals(false) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue).Select(item => new UsuarioIDPerfilID { UsuarioID = item.Persona.UsuarioID.Value, PerfilID = item.Identidad.PerfilID }).ToList();

            ObtenerListadoIdsDeQuery(listaSeguidores2, pListaUsuarios);

        }

        /// <summary>
        /// Obtiene un listado de identificadores a partir de una query
        /// </summary>
        /// <param name="pQuery">Parte FROM y WHERE de la Query</param>
        /// <param name="pCampoSelect">Campo que contiene los IDs</param>
        /// <param name="pListaResultados">Lista inicializada que se va a rellenar con los datos obtenidos de la consulta (lista de string si se busca todos los usuarios o lista de Guid si se filtra por caracteres)</param>
        private void ObtenerListadoIdsDeQuery(List<UsuarioIDPerfilID> pListaACargar, Dictionary<Guid, List<Guid>> pListaUsuarios)
        {
            for (int i = 0; i < pListaACargar.Count; i++)
            {
                Guid usuarioID = pListaACargar[i].UsuarioID;
                if (!pListaUsuarios.ContainsKey(usuarioID))
                {
                    pListaUsuarios.Add(usuarioID, new List<Guid>());
                }
                Guid perfilID = pListaACargar[i].PerfilID;
                if (!pListaUsuarios[usuarioID].Contains(perfilID))
                {
                    pListaUsuarios[usuarioID].Add(perfilID);
                }
            }
        }

        /// <summary>
        /// Obtiene el ID de un usuario en funcion de sus datos de tipo de red social y el id en esa red social
        /// </summary>
        /// <param name="pTipoRedSocial"></param>
        /// <param name="pIDenRedSocial"></param>
        /// <returns></returns>
        public Guid ObtenerUsuarioPorLoginEnRedSocial(TipoRedSocialLogin pTipoRedSocial, string pIDenRedSocial)
        {
            return mEntityContext.UsuarioVinculadoLoginRedesSociales.Where(item => item.TipoRedSocial.Equals((short)pTipoRedSocial) && item.IDenRedSocial.Equals(pIDenRedSocial)).Select(item => item.UsuarioID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el Login de la red social de un usuario en funcion de sus datos de tipo de red social y el usuario id
        /// </summary>
        /// <param name="pTipoRedSocial"></param>
        /// <param name="pUsuarioID"></param>
        /// <returns></returns>
        public string ObtenerLoginEnRedSocialPorUsuarioId(TipoRedSocialLogin pTipoRedSocial, Guid pUsuarioID)
        {
            return mEntityContext.UsuarioVinculadoLoginRedesSociales.Where(item => item.TipoRedSocial.Equals((short)pTipoRedSocial) && item.UsuarioID.Equals(pUsuarioID)).Select(item => item.IDenRedSocial).FirstOrDefault();
        }

        /// <summary>
        /// Actualiza el Login de la red social de un usuario en funcion de sus datos de tipo de red social y el usuario id 
        /// </summary>
        /// <param name="pTipoRedSocial"></param>
        /// <param name="pUsuarioID"></param>
        /// <param name="pLogin"></param>
        public void ActualizarLoginEnRedSocialPorUsuario(TipoRedSocialLogin pTipoRedSocial, Guid pUsuarioID, string pLogin)
        {
            UsuarioVinculadoLoginRedesSociales usuario = mEntityContext.UsuarioVinculadoLoginRedesSociales.Where(item => item.TipoRedSocial.Equals((short)pTipoRedSocial) && item.UsuarioID.Equals(pUsuarioID)).FirstOrDefault();
            usuario.IDenRedSocial = pLogin;

            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Obtiene la tabla usuariovinculadoredessociales de un usuario
        /// </summary>
        /// <param name="pTipoRedSocial"></param>
        /// <param name="pIDenRedSocial"></param>
        /// <returns></returns>
        public DataWrapperUsuario ObtenerFilaUsuarioVincRedSocialPorUsuarioID(Guid pUsuarioID)
        {
            DataWrapperUsuario dataWrapperUsuario = new DataWrapperUsuario();
            dataWrapperUsuario.ListaUsuarioVinculadoLoginRedesSociales = mEntityContext.UsuarioVinculadoLoginRedesSociales.Where(usu => usu.UsuarioID.Equals(pUsuarioID)).ToList();
            return dataWrapperUsuario;
        }

        /// <summary>
        /// Obtiene un conjunto de usuarios a partir de una lista de identidades como parámetro
        /// </summary>
        /// <param name="pListaIdentidades">Lista de identificadores de las identidades</param>
        /// <returns>Dataset de usuarios</returns>
        public List<EntityModel.Models.UsuarioDS.Usuario> ObtenerUsuariosPorIdentidadesCargaLigera(List<Guid> pListaIdentidades)
        {
            List<Guid> listaPersonaID = mEntityContext.PerfilPersona.Join(mEntityContext.Identidad, perfilPersona => perfilPersona.PerfilID, identidad => identidad.PerfilID, (perfilPersona, Identidad) =>
            new
            {
                PerfilPersona = perfilPersona,
                Identidad = Identidad
            }).Where(item => pListaIdentidades.Contains(item.Identidad.IdentidadID)).Select(item => item.PerfilPersona.PersonaID)
            .Concat(mEntityContext.PerfilPersonaOrg.Join(mEntityContext.Identidad, perfilPersonaOrg => perfilPersonaOrg.PerfilID, identidad => identidad.PerfilID, (perfilPersonaOrg, identidad) =>
            new
            {
                PerfilPersonaOrg = perfilPersonaOrg,
                Identidad = identidad
            }).Where(item => pListaIdentidades.Contains(item.Identidad.IdentidadID)).Select(item => item.PerfilPersonaOrg.PersonaID)).ToList();

            return mEntityContext.Usuario.Join(mEntityContext.Persona, usuario => usuario.UsuarioID, persona => persona.UsuarioID.Value, (usuario, persona) =>
            new
            {
                Usuario = usuario,
                Persona = persona
            }).Where(item => listaPersonaID.Contains(item.Persona.PersonaID)).Select(item => item.Usuario).ToList();
        }


        public Guid SelectGuidUsuarioIDadoIdentidadID(Guid pIDentidadID)
        {
            return mEntityContext.Identidad.Join(mEntityContext.Perfil, id => id.PerfilID, pf => pf.PerfilID, (id, pf) =>
            new
            {
                Identidad = id,
                Perfil = pf
            }).Join(mEntityContext.Persona, idpf => idpf.Perfil.PersonaID, ps => ps.PersonaID, (idpf, ps) =>
            new
            {
                Identidad = idpf.Identidad,
                Perfil = idpf.Perfil,
                Persona = ps,
            }).Where(item => item.Identidad.IdentidadID.Equals(pIDentidadID) && item.Persona.UsuarioID.HasValue).Select(item => item.Persona.UsuarioID.Value).ToList().FirstOrDefault();
        }

        /// <summary>
        /// Obtiene los usuarios de las personas cargadas en el dataSet.
        /// </summary>
        /// <param name="pListaPersonas">DataSet co las personas cargadas</param>
        /// <returns>Dataset de usuarios</returns>
        public List<EntityModel.Models.UsuarioDS.Usuario> ObtenerUsuariosDePersonasCargadas(List<Persona> pListaPersonas)
        {
            List<Guid> listaUsuariosIDs = pListaPersonas.Select(persona => persona.UsuarioID.Value).ToList();
            return mEntityContext.Usuario.Where(item => listaUsuariosIDs.Contains(item.UsuarioID)).ToList();
        }

        /// <summary>
        /// Obtiene un usuario (Usuario e InicioSesion) por el login para un usuario y un proyecto
        /// </summary>
        /// <param name="pLogin">Login del usuario</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Dataset de usuarios</returns>
        public DataWrapperUsuario ObtenerUsuarioPorLoginEnProyecto(string pLogin, Guid pProyectoID)
        {
            DataWrapperUsuario dataWrapper = new DataWrapperUsuario();
            dataWrapper.ListaUsuario = mEntityContext.Usuario.Where(usuario => usuario.Login.Equals(pLogin) && usuario.EstaBloqueado.HasValue && !usuario.EstaBloqueado.Value).ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Comprueba si un usuario dado está en un proyecto pasado por parámetro
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>TRUE si el usuario está en el proyecto, FALSE en caso contrario</returns>
        public bool EstaUsuarioEnProyecto(Guid pUsuarioID, Guid pProyectoID)
        {
            return mEntityContext.Usuario.Join(mEntityContext.ProyectoRolUsuario, usu => usu.UsuarioID, prolusu => prolusu.UsuarioID, (usu, prolusu) => new
            {
                Usuario = usu,
                ProyectoRolUsuario = prolusu
            }).Join(mEntityContext.ProyectoUsuarioIdentidad, usuProl => new { usuProl.ProyectoRolUsuario.UsuarioID, usuProl.ProyectoRolUsuario.ProyectoID }, pusuid => new { pusuid.UsuarioID, pusuid.ProyectoID }, (usuProl, pusuid) => new
            {
                Usuario = usuProl.Usuario,
                ProyectoRolUsuario = usuProl.ProyectoRolUsuario,
                ProyectoUsuarioIdentidad = pusuid
            }).Join(mEntityContext.Identidad, usuProlPusu => usuProlPusu.ProyectoUsuarioIdentidad.IdentidadID, id => id.IdentidadID, (usuProlPusu, id) => new
            {
                Usuario = usuProlPusu.Usuario,
                ProyectoRolUsuario = usuProlPusu.ProyectoRolUsuario,
                ProyectoUsuarioIdentidad = usuProlPusu.ProyectoUsuarioIdentidad,
                Identidad = id
            }).Any(item => item.ProyectoRolUsuario.ProyectoID.Equals(pProyectoID) && item.ProyectoRolUsuario.UsuarioID.Equals(pUsuarioID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue);
        }

        /// <summary>
        /// Obtiene los roles (Usuario y OrganizacionRolUsuario) en las organizaciones de un usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pObtenerSoloSiTieneAlgunPermiso">Verdad si se deben obtener sólo las filas en las que el usuario tiene algún permiso</param>
        /// <returns>Dataset de usuarios</returns>
        public DataWrapperUsuario ObtenerRolesOrganizaciones(Guid pUsuarioID, bool pObtenerSoloSiTieneAlgunPermiso)
        {
            DataWrapperUsuario dataWrapper = new DataWrapperUsuario();

            dataWrapper.ListaUsuario = mEntityContext.Usuario.Where(usuario => usuario.UsuarioID.Equals(pUsuarioID)).ToList();

            var organizacionRolUsuario = mEntityContext.OrganizacionRolUsuario.Where(item => item.UsuarioID.Equals(pUsuarioID));

            if (pObtenerSoloSiTieneAlgunPermiso)
            {
                organizacionRolUsuario = organizacionRolUsuario.Where(item => item.RolPermitido != "0000000000000000");
            }

            dataWrapper.ListaOrganizacionRolUsuario = organizacionRolUsuario.ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Nos indica en que proyectos el usuario tiene o no foto
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Diccionario con proyectoID como clave y true o false en función de si tiene foto o no en ese proyecto</returns>
        public string FotoPerfilPersonalUsuario(Guid pUsuarioID)
        {
            string foto = mEntityContext.Usuario.JoinPersona().JoinPerfil().JoinIdentidad().Where(item => item.Usuario.UsuarioID.Equals(pUsuarioID) && !item.Perfil.OrganizacionID.HasValue).Select(item => item.Identidad.Foto).FirstOrDefault();
            if (foto.Equals("sinfoto"))
            {
                foto = "";
            }
            return foto;
        }

        /// <summary>
        /// Obtiene los roles (Usuario y OrganizacionRolUsuario) en una organización de un usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Dataset de usuarios</returns>
        public DataWrapperUsuario ObtenerOrganizacionRolUsuario(Guid pUsuarioID, Guid pOrganizacionID)
        {
            DataWrapperUsuario dataWrapper = new DataWrapperUsuario();

            dataWrapper.ListaUsuario = mEntityContext.Usuario.Where(usuario => usuario.UsuarioID.Equals(pUsuarioID)).ToList();

            dataWrapper.ListaOrganizacionRolUsuario = mEntityContext.OrganizacionRolUsuario.Where(item => item.UsuarioID.Equals(pUsuarioID) && item.OrganizacionID.Equals(pOrganizacionID)).ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene los usuarios "Usuario" y "OrganizacionRolUsuario" asi como "ProyectoUsuarioIdentidad" 
        /// de todos los usuarios de la organizacion
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Dataset de usuarios</returns>
        public DataWrapperUsuario CargarUsuariosDeOrganizacion(Guid pOrganizacionID)
        {
            DataWrapperUsuario dataWrapper = new DataWrapperUsuario();

            //Usuario
            dataWrapper.ListaUsuario = mEntityContext.Usuario.Join(mEntityContext.Persona, usuario => usuario.UsuarioID, persona => persona.UsuarioID, (usuario, persona) => new
            {
                Usuario = usuario,
                Persona = persona
            }).Join(mEntityContext.PersonaVinculoOrganizacion, usuarioPersona => usuarioPersona.Persona.PersonaID, persVincOrg => persVincOrg.PersonaID, (usuarioPersona, persVincOrg) => new
            {
                Usuario = usuarioPersona.Usuario,
                Persona = usuarioPersona.Persona,
                PersonaVinculoOrganizacion = persVincOrg
            }).Where(item => item.PersonaVinculoOrganizacion.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.Usuario).ToList();

            //ProyectoRolUsuario
            dataWrapper.ListaProyectoRolUsuario = mEntityContext.ProyectoRolUsuario.Join(mEntityContext.ProyectoUsuarioIdentidad, proyRolUsu => new
            {
                proyRolUsu.OrganizacionGnossID,
                proyRolUsu.ProyectoID,
                proyRolUsu.UsuarioID
            },
            proyUsuIdent => new
            {
                proyUsuIdent.OrganizacionGnossID,
                proyUsuIdent.ProyectoID,
                proyUsuIdent.UsuarioID
            },
            (proyRolUsu, proyUsuIdent) => new
            {
                ProyectoRolUsuario = proyRolUsu,
                ProyectoUsuarioIdentidad = proyUsuIdent
            }).Join(mEntityContext.Identidad, proyRolUsuProyUsuIde => proyRolUsuProyUsuIde.ProyectoUsuarioIdentidad.IdentidadID, identidad => identidad.IdentidadID, (proyRolUsuProyUsuIde, identidad) => new
            {
                ProyectoRolUsuario = proyRolUsuProyUsuIde.ProyectoRolUsuario,
                ProyectoUsuarioIdentidad = proyRolUsuProyUsuIde.ProyectoUsuarioIdentidad,
                Identidad = identidad
            }).Join(mEntityContext.PerfilPersonaOrg, proyRolUsuProyUsuIdeIde => proyRolUsuProyUsuIdeIde.Identidad.PerfilID, perfilPersonaOrg => perfilPersonaOrg.PerfilID, (proyRolUsuProyUsuIdeIde, perfilPersonaOrg) => new
            {
                ProyectoRolUsuario = proyRolUsuProyUsuIdeIde.ProyectoRolUsuario,
                ProyectoUsuarioIdentidad = proyRolUsuProyUsuIdeIde.ProyectoUsuarioIdentidad,
                Identidad = proyRolUsuProyUsuIdeIde.Identidad,
                PerfilPersonaOrg = perfilPersonaOrg
            }).Where(item => item.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.ProyectoRolUsuario).ToList();

            //ProyectoUsuarioIdentidad
            dataWrapper.ListaProyectoUsuarioIdentidad = mEntityContext.ProyectoUsuarioIdentidad.Join(mEntityContext.Identidad, proyUsuIdent => proyUsuIdent.IdentidadID, identidad => identidad.IdentidadID, (proyUsuIdent, identidad) => new
            {
                ProyectoUsuarioIdentidad = proyUsuIdent,
                Identidad = identidad
            }).Join(mEntityContext.PerfilPersonaOrg, proyUsuIdeIde => proyUsuIdeIde.Identidad.PerfilID, perfPersOrg => perfPersOrg.PerfilID, (proyUsuIdeIde, perfPersOrg) => new
            {
                ProyectoUsuarioIdentidad = proyUsuIdeIde.ProyectoUsuarioIdentidad,
                Identidad = proyUsuIdeIde.Identidad,
                PerfilPersonaOrg = perfPersOrg
            }).Where(item => item.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.ProyectoUsuarioIdentidad).ToList();

            //OrganizacionRolUsuario
            dataWrapper.ListaOrganizacionRolUsuario = mEntityContext.OrganizacionRolUsuario.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene los usuarios (Usuario y OrganizacionRolUsuario) que son administradores de la organización pasada por parámetro
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Dataset de usuarios</returns>
        public DataWrapperUsuario CargarAdministradoresDeOrg(Guid pOrganizacionID)
        {
            DataWrapperUsuario dataWrapper = new DataWrapperUsuario();

            //Usuario
            dataWrapper.ListaUsuario = mEntityContext.Usuario.Join(mEntityContext.AdministradorOrganizacion, usuario => usuario.UsuarioID, administradorOrganizacion => administradorOrganizacion.UsuarioID, (usuario, administradorOrganizacion) =>
            new
            {
                Usuario = usuario,
                AdministradorOrganizacion = administradorOrganizacion
            }).Where(item => item.AdministradorOrganizacion.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.Usuario).Distinct().ToList();

            //OrganizacionRolUsuario
            dataWrapper.ListaOrganizacionRolUsuario = mEntityContext.OrganizacionRolUsuario.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).ToList();

            return dataWrapper;
        }

        public List<Guid> ObtenerOrganizacionesAdministradasPorUsuario(Guid pUsuario)
        {
            List<Guid> organizaciones = new List<Guid>();

            organizaciones = mEntityContext.AdministradorOrganizacion.Where(item => item.UsuarioID.Equals(pUsuario)).Select(x => x.OrganizacionID).Distinct().ToList();

            return organizaciones;
        }

        /// <summary>
        /// Obtiene los usuarios, sus identidades y sus permisos en proyectos 
        /// en los que tienen perfil para la organización pasada por parámetro
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Dataset de usuarios</returns>
        public DataWrapperUsuario CargarUsuariosDeOrganizacionYsusIdentidadesYPermisosEnProyectosConPerfilDeDichaOrg(Guid pOrganizacionID)
        {
            DataWrapperUsuario dataWrapper = new DataWrapperUsuario();

            //Usuario
            dataWrapper.ListaUsuario = mEntityContext.Usuario.Join(mEntityContext.Persona, usuario => usuario.UsuarioID, persona => persona.UsuarioID, (usuario, persona) =>
            new
            {
                Usuario = usuario,
                Persona = persona
            }).Join(mEntityContext.PersonaVinculoOrganizacion, usuarioPersona => usuarioPersona.Persona.PersonaID, persVincOrg => persVincOrg.PersonaID, (usuarioPersona, persVincOrg) =>
            new
            {
                Usuario = usuarioPersona.Usuario,
                Persona = usuarioPersona.Persona,
                PersonaVinculoOrganizacion = persVincOrg
            }).Where(item => item.PersonaVinculoOrganizacion.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.Usuario).ToList();

            //ProyectoRolUsuario
            dataWrapper.ListaProyectoRolUsuario = mEntityContext.ProyectoRolUsuario.Join(mEntityContext.ProyectoUsuarioIdentidad, proyectoRolUsuario => new { proyectoRolUsuario.OrganizacionGnossID, proyectoRolUsuario.ProyectoID, proyectoRolUsuario.UsuarioID }, proyectoUsuarioIdentidad => new { proyectoUsuarioIdentidad.OrganizacionGnossID, proyectoUsuarioIdentidad.ProyectoID, proyectoUsuarioIdentidad.UsuarioID }, (proyectoRolUsuario, proyectoUsuarioIdentidad) =>
            new
            {
                ProyectoRolUsuario = proyectoRolUsuario,
                ProyectoUsuarioIdentidad = proyectoUsuarioIdentidad
            }).Join(mEntityContext.Identidad, proyRolUsuProyUsuIde => proyRolUsuProyUsuIde.ProyectoUsuarioIdentidad.IdentidadID, identidad => identidad.IdentidadID, (proyRolUsuProyUsuIde, identidad) =>
            new
            {
                ProyectoRolUsuario = proyRolUsuProyUsuIde.ProyectoRolUsuario,
                ProyectoUsuarioIdentidad = proyRolUsuProyUsuIde.ProyectoUsuarioIdentidad,
                Identidad = identidad
            }).Join(mEntityContext.PerfilPersonaOrg, proRolUsuProUsuIdeIde => proRolUsuProUsuIdeIde.Identidad.PerfilID, perfilPersonaOrg => perfilPersonaOrg.PerfilID, (proRolUsuProUsuIdeIde, perfilPersonaOrg) =>
            new
            {
                ProyectoRolUsuario = proRolUsuProUsuIdeIde.ProyectoRolUsuario,
                ProyectoUsuarioIdentidad = proRolUsuProUsuIdeIde.ProyectoUsuarioIdentidad,
                Identidad = proRolUsuProUsuIdeIde.Identidad,
                PerfilPersonaOrg = perfilPersonaOrg
            }).Where(item => item.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.ProyectoRolUsuario).ToList();

            //ProyectoUsuarioIdentidad
            dataWrapper.ListaProyectoUsuarioIdentidad = mEntityContext.ProyectoUsuarioIdentidad.Join(mEntityContext.Identidad, proyectoUsuarioIdentidad => proyectoUsuarioIdentidad.IdentidadID, identidad => identidad.IdentidadID, (proyectoUsuarioIdentidad, identidad) =>
            new
            {
                ProyectoUsuarioIdentidad = proyectoUsuarioIdentidad,
                Identidad = identidad
            }).Join(mEntityContext.PerfilPersonaOrg, proUsuIdeIde => proUsuIdeIde.Identidad.PerfilID, perfPersOrg => perfPersOrg.PerfilID, (proUsuIdeIde, perfPersOrg) =>
            new
            {
                ProyectoUsuarioIdentidad = proUsuIdeIde.ProyectoUsuarioIdentidad,
                Identidad = proUsuIdeIde.Identidad,
                PerfilPersonaOrg = perfPersOrg
            }).Where(item => item.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.ProyectoUsuarioIdentidad).ToList();

            //OrganizacionRolUsuario
            dataWrapper.ListaOrganizacionRolUsuario = mEntityContext.OrganizacionRolUsuario.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).ToList();

            //InicioSesion
            dataWrapper.ListaInicioSesion = mEntityContext.InicioSesion.JoinOrganizacionRolUsuario().Where(item => item.OrganizacionRolUsuario.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.InicioSesion).ToList();

            //HistoricoProyectoUsuario
            dataWrapper.ListaHistoricoProyectoUsuario = mEntityContext.HistoricoProyectoUsuario.JoinOrganizacionRolUsuario().Where(item => item.OrganizacionRolUsuario.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.HistoricoProyectoUsuario).ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene el número de personas que son editores en una organizacion
        /// </summary>
        /// <param name="pOrganizacionID">ID de la organizacion</param>
        /// <returns></returns>
        public int ObtenerNumEditoresOrganizacion(Guid pOrganizacionID)
        {
            List<Guid> listaAdminOrg = mEntityContext.AdministradorOrganizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.UsuarioID).ToList();

            return mEntityContext.Usuario.Join(mEntityContext.Persona, usuario => usuario.UsuarioID, persona => persona.UsuarioID, (usuario, persona) =>
            new
            {
                Usuario = usuario,
                Persona = persona
            }).Join(mEntityContext.PersonaVinculoOrganizacion, usuPers => usuPers.Persona.PersonaID, persVincOrg => persVincOrg.PersonaID, (usuPers, persVincOrg) =>
            new
            {
                Usuario = usuPers.Usuario,
                Persona = usuPers.Persona,
                PersonaVinculoOrganizacion = persVincOrg
            }).Join(mEntityContext.AdministradorOrganizacion, usuPersPersVincOrg => new { usuPersPersVincOrg.Usuario.UsuarioID, usuPersPersVincOrg.PersonaVinculoOrganizacion.OrganizacionID }, adminOrg => new { adminOrg.UsuarioID, adminOrg.OrganizacionID }, (usuPersPersVincOrg, adminOrg) =>
            new
            {
                Usuario = usuPersPersVincOrg.Usuario,
                Persona = usuPersPersVincOrg.Persona,
                PersonaVinculoOrganizacion = usuPersPersVincOrg.PersonaVinculoOrganizacion,
                AdministradorOrganizacion = adminOrg
            }).Join(mEntityContext.Perfil, usuPersPersVincOrgAdminOrg => new { usuPersPersVincOrgAdminOrg.Persona.PersonaID, usuPersPersVincOrgAdminOrg.PersonaVinculoOrganizacion.OrganizacionID }, perfil => new { PersonaID = perfil.PersonaID.Value, OrganizacionID = perfil.OrganizacionID.Value }, (usuPersPersVincOrgAdminOrg, perfil) =>
            new
            {
                Usuario = usuPersPersVincOrgAdminOrg.Usuario,
                Persona = usuPersPersVincOrgAdminOrg.Persona,
                PersonaVinculoOrganizacion = usuPersPersVincOrgAdminOrg.PersonaVinculoOrganizacion,
                AdministradorOrganizacion = usuPersPersVincOrgAdminOrg.AdministradorOrganizacion,
                Perfil = perfil
            }).Join(mEntityContext.Identidad, usPerPerViOrAdOrPerf => usPerPerViOrAdOrPerf.Perfil.PerfilID, Identidad => Identidad.PerfilID, (usPerPerViOrAdOrPerf, identidad) =>
         new
         {
             Usuario = usPerPerViOrAdOrPerf.Usuario,
             Persona = usPerPerViOrAdOrPerf.Persona,
             PersonaVinculoOrganizacion = usPerPerViOrAdOrPerf.PersonaVinculoOrganizacion,
             AdministradorOrganizacion = usPerPerViOrAdOrPerf,
             Perfil = usPerPerViOrAdOrPerf.Perfil,
             Identidad = identidad
         }).Count(item => item.PersonaVinculoOrganizacion.OrganizacionID.Equals(pOrganizacionID) && !item.Identidad.FechaBaja.HasValue && item.Perfil.PersonaID.HasValue && item.Perfil.OrganizacionID.HasValue && item.AdministradorOrganizacion.AdministradorOrganizacion.Tipo.Equals((short)1));
        }

        /// <summary>
        /// Obtiene el número de personas que son usuarios en una organizacion
        /// </summary>
        /// <param name="pOrganizacionID">ID de la organizacion</param>
        /// <returns></returns>
        public int ObtenerNumUsuariosOrganizacion(Guid pOrganizacionID)
        {
            var consulta1 = mEntityContext.AdministradorOrganizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).Select(item => new { UsuarioID = item.UsuarioID, Tipo = item.Tipo });

            var listaId = mEntityContext.AdministradorOrganizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.UsuarioID);

            var consulta2 = mEntityContext.Persona.JoinPersonaVinculoOrganizacion().Where(item => !listaId.Contains(item.Persona.UsuarioID.Value) && item.PersonaVinculoOrganizacion.OrganizacionID.Equals(pOrganizacionID)).Select(item => new { UsuarioID = item.Persona.UsuarioID.Value, Tipo = (short)TipoAdministradoresOrganizacion.Comentarista });

            return consulta1.Union(consulta2).Where(item => item.Tipo.Equals((short)TipoAdministradoresOrganizacion.Comentarista)).Count();
        }

        /// <summary>
        /// Obtiene los usuarios en la pOrganizacionID
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Dataset de usuario</returns>
        public DataWrapperUsuario CargarUsuariosDeOrganizacionCargaLigeraParaFiltros(Guid pOrganizacionID)
        {
            DataWrapperUsuario dataWrapper = new DataWrapperUsuario();

            List<Guid> listaUsuarioID = mEntityContext.AdministradorOrganizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.UsuarioID).ToList();

            //UsuarioIdentidadPersona
            //dataWrapper.ListaUsuarioIdentidadPersona = 
            var primeraconsulta = mEntityContext.Usuario.Join(mEntityContext.Persona, usu => usu.UsuarioID, ps => ps.UsuarioID.Value, (usu, ps) => new
            {
                Usuario = usu,
                Persona = ps
            }).Join(mEntityContext.Perfil, usuPs => usuPs.Persona.PersonaID, pf => pf.PersonaID.Value, (usuPs, pf) => new
            {
                Usuario = usuPs.Usuario,
                Persona = usuPs.Persona,
                Perfil = pf
            }).Join(mEntityContext.Identidad, usuPersPerf => usuPersPerf.Perfil.PerfilID, id => id.PerfilID, (usuPersPerf, id) => new
            {
                Usuario = usuPersPerf.Usuario,
                Persona = usuPersPerf.Persona,
                Perfil = usuPersPerf.Perfil,
                Identidad = id
            }).Join(mEntityContext.AdministradorOrganizacion, usuPersPerfID => new
            {
                UsuarioID = usuPersPerfID.Usuario.UsuarioID,
                OrganizacionID = usuPersPerfID.Perfil.OrganizacionID.Value
            },
                admin => new
                {
                    admin.UsuarioID,
                    admin.OrganizacionID
                },
                (usuPersPerfID, admin) => new
                {
                    Usuario = usuPersPerfID.Usuario,
                    Persona = usuPersPerfID.Persona,
                    Perfil = usuPersPerfID.Perfil,
                    Identidad = usuPersPerfID.Identidad,
                    AdministradorOrganizacion = admin
                })
            .Where(item => item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && item.Perfil.OrganizacionID.HasValue && !item.Persona.Eliminado && !item.Perfil.Eliminado && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto)).ToList()
            .GroupBy(item => new
            {
                item.Usuario.UsuarioID,
                item.Usuario.EstaBloqueado,
                item.Persona.Nombre,
                item.Persona.Apellidos,
                item.Perfil.NombreCortoUsu,
                item.Persona.PersonaID,
                item.Identidad.Foto,
                item.Usuario.TwoFactorAuthentication
            }).ToList().Select(item => new UsuarioIdentidadPersona
            {
                UsuarioID = item.Key.UsuarioID,
                EstaBloqueado = item.Key.EstaBloqueado.Value,
                Tipo = item.Min(obj => obj.AdministradorOrganizacion.Tipo),
                Nombre = item.Key.Nombre,
                Apellidos = item.Key.Apellidos,
                NombreCortoUsu = item.Key.NombreCortoUsu,
                PersonaID = item.Key.PersonaID,
                Foto = item.Key.Foto,
                TwoFactorAuthentication = item.Key.TwoFactorAuthentication
            }).ToList();

            var segundaconsulta = mEntityContext.Usuario.Join(mEntityContext.Persona, usu => usu.UsuarioID, pers => pers.UsuarioID.Value, (usu, pers) => new
            {
                Usuario = usu,
                Persona = pers
            }).Join(mEntityContext.Perfil, usuPers => usuPers.Persona.PersonaID, perf => perf.PersonaID.Value, (usuPers, perf) => new
            {
                Usuario = usuPers.Usuario,
                Persona = usuPers.Persona,
                Perfil = perf
            }).Join(mEntityContext.Identidad, usuPersPerf => usuPersPerf.Perfil.PerfilID, id => id.PerfilID, (usuPersPerf, id) => new
            {
                Usuario = usuPersPerf.Usuario,
                Persona = usuPersPerf.Persona,
                Perfil = usuPersPerf.Perfil,
                Identidad = id
            }).Where(item => item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && !listaUsuarioID.Contains(item.Persona.UsuarioID.Value) && !item.Persona.Eliminado && !item.Perfil.Eliminado && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto)).ToList()
            .Select(item => new UsuarioIdentidadPersona
            {
                UsuarioID = item.Usuario.UsuarioID,
                EstaBloqueado = item.Usuario.EstaBloqueado,
                Tipo = 2,
                Nombre = item.Persona.Nombre,
                Apellidos = item.Persona.Apellidos,
                NombreCortoUsu = item.Perfil.NombreCortoUsu,
                PersonaID = item.Persona.PersonaID,
                Foto = item.Identidad.Foto,
                TwoFactorAuthentication = item.Usuario.TwoFactorAuthentication
            }).ToList();

            dataWrapper.ListaUsuarioIdentidadPersona = primeraconsulta.Union(segundaconsulta).ToList();

            //OrganizacionRolUsuario
            dataWrapper.ListaOrganizacionRolUsuario = mEntityContext.OrganizacionRolUsuario.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).ToList();
            return dataWrapper;
        }

        public string ObtenerEmailPorUsuarioID(Guid pUsuarioID)
        {
            return mEntityContext.Persona.Where(item => item.UsuarioID.HasValue && item.UsuarioID.Value.Equals(pUsuarioID)).Select(item => item.Email).FirstOrDefault();
        }

        /// <summary>
        /// Cuenta los usuarios agrupados por tipo de una organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns></returns>
        public Dictionary<short, int> ContarTiposUsuariosDeOrganizacion(Guid pOrganizacionID)
        {
            Dictionary<short, int> listaContadorTiposUsuario = new Dictionary<short, int>();

            List<Guid> listaOrganizacion = mEntityContext.AdministradorOrganizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.UsuarioID).ToList();

            var resultado = mEntityContext.Usuario.Join(mEntityContext.Persona, usu => new { UsuarioID = usu.UsuarioID }, pers => new { UsuarioID = pers.UsuarioID.Value }, (usu, pers) => new
            {
                Usuario = usu,
                Persona = pers
            }).Join(mEntityContext.Perfil, usuPers => new { PersonaID = usuPers.Persona.PersonaID }, perf => new { PersonaID = perf.PersonaID.Value }, (usuPers, perf) => new
            {
                Usuario = usuPers.Usuario,
                Persona = usuPers.Persona,
                Perfil = perf
            }).Join(mEntityContext.AdministradorOrganizacion, usuPersPerf => new { UsuarioID = usuPersPerf.Usuario.UsuarioID, OrganizacionID = usuPersPerf.Perfil.OrganizacionID.Value }, admin => new { UsuarioID = admin.UsuarioID, OrganizacionID = admin.OrganizacionID }, (usuPersPerf, admin) => new
            {
                Usuario = usuPersPerf.Usuario,
                Persona = usuPersPerf.Persona,
                Perfil = usuPersPerf.Perfil,
                AdministradorOrganizacion = admin
            }).Where(item => item.Perfil.OrganizacionID.HasValue && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID) && !item.Persona.Eliminado).GroupBy(item => item.Usuario.UsuarioID).Select(item => new
            {
                UsuarioID = item.Key,
                Tipo = item.Min(item2 => item2.AdministradorOrganizacion.Tipo)
            }).Union(mEntityContext.Usuario.Join(mEntityContext.Persona, usu => new { UsuarioID = usu.UsuarioID }, pers => new { UsuarioID = pers.UsuarioID.Value }, (usu, pers) => new
            {
                Usuario = usu,
                Persona = pers
            }).Join(mEntityContext.PersonaVinculoOrganizacion, usuPers => usuPers.Persona.PersonaID, persVinc => persVinc.PersonaID, (usuPers, persVinc) => new
            {
                Usuario = usuPers.Usuario,
                Persona = usuPers.Persona,
                PersonaVinculoOrganizacion = persVinc
            }).Join(mEntityContext.Perfil, usuPersVinc => new
            {
                usuPersVinc.Persona.PersonaID,
                usuPersVinc.PersonaVinculoOrganizacion.OrganizacionID
            }, perf => new
            {
                PersonaID = perf.PersonaID.Value,
                OrganizacionID = perf.OrganizacionID.Value
            }, (usuPersVinc, perf) => new
            {
                Usuario = usuPersVinc.Usuario,
                Persona = usuPersVinc.Persona,
                PersonaVinculoOrganizacion = usuPersVinc.PersonaVinculoOrganizacion,
                Perfil = perf
            }).Where(item => item.PersonaVinculoOrganizacion.OrganizacionID.Equals(pOrganizacionID) && !listaOrganizacion.Contains(item.Persona.UsuarioID.Value) && !item.Persona.Eliminado && !item.Perfil.Eliminado).Select(item => new
            {
                UsuarioID = item.Usuario.UsuarioID,
                Tipo = (short)2
            })).GroupBy(item => item.Tipo).Select(item => new { UsuarioID = item.Count(), Tipo = item.Key });

            foreach (var fila in resultado)
            {
                if (!listaContadorTiposUsuario.ContainsKey(fila.Tipo))
                {
                    listaContadorTiposUsuario.Add(fila.Tipo, fila.UsuarioID);
                }
            }
            return listaContadorTiposUsuario;
        }

        /// <summary>
        /// Devuelve...
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Dataset de usuarios</returns>
        public DataWrapperUsuario ObtenerRolesUsuarioPorPerfilYProyecto(Guid pUsuarioID, Guid pProyectoID, Guid pPerfilID)
        {
            DataWrapperUsuario dataWrapper = new DataWrapperUsuario();

            dataWrapper.ListaGeneralRolUsuario = mEntityContext.GeneralRolUsuario.Where(item => item.UsuarioID.Equals(pUsuarioID)).ToList();

            dataWrapper.ListaProyectoRolUsuario = mEntityContext.ProyectoRolUsuario.Where(item => item.ProyectoID.Equals(pProyectoID) && item.UsuarioID.Equals(pUsuarioID)).ToList();

            dataWrapper.ListaOrganizacionRolUsuario = mEntityContext.OrganizacionRolUsuario.JoinPerfilPersonaOrg().Where(item => item.PerfilPersonaOrg.PerfilID.Equals(pPerfilID) && item.OrganizacionRolUsuario.UsuarioID.Equals(pUsuarioID)).Select(item => item.OrganizacionRolUsuario).ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Actualiza usuarios
        /// </summary>
        /// <param name="pUsuarioDS">Dataset de usuarios</param>
        public void ActualizarUsuarios()
        {

            mEntityContext.SaveChanges();


        }
        private void AumentarNumeroMiembrosDelProyecto(Guid pProyectoID)
        {
            //Actualizar en ProyectoDS / Proyecto / "NumeroMiembros"
            ProyectoAD proyectoAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication,mLoggerFactory.CreateLogger<ProyectoAD>(),mLoggerFactory);
            proyectoAD.AumentarNumeroMiembrosDelProyecto(pProyectoID);
            proyectoAD.Dispose();
        }

        private void DisminuirNumeroMiembrosDelProyecto(Guid pProyectoID)
        {
            //Actualizar en ProyectoDS / Proyecto / "NumeroMiembros"
            ProyectoAD proyectoAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication,mLoggerFactory.CreateLogger<ProyectoAD>(),mLoggerFactory);
            proyectoAD.DisminuirNumeroMiembrosDelProyecto(pProyectoID);
            proyectoAD.Dispose();
        }

        /// <summary>
        /// Comprueba si el usuario es administrador general 
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>TRUE si el usuario es administrador general, FALSE en caso contrario</returns>
        public bool EsUsuarioAdministradorGeneral(Guid pUsuarioID)
        {
            return mEntityContext.AdministradorGeneral.Any(item => item.UsuarioID.Equals(pUsuarioID));
        }

        /// <summary>
        /// Comprueba si el usuario esta solo en proyectos públicos, o posee también privados, etc.
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>
        /// TRUE si el usuario solo está en proyectos públicos, FALSE en caso contrario</returns>
        public bool UsuarioSoloEstaEnProyectosPublicos(Guid pUsuarioID)
        {
            return !(mEntityContext.Proyecto.Join(mEntityContext.ProyectoRolUsuario, proyecto => new { proyecto.ProyectoID, proyecto.OrganizacionID }, proyRol => new { proyRol.ProyectoID, OrganizacionID = proyRol.OrganizacionGnossID }, (proyecto, proyRol) =>
              new
              {
                  Proyecto = proyecto,
                  ProyectoRolUsuario = proyRol
              }).Any(item => item.ProyectoRolUsuario.UsuarioID.Equals(pUsuarioID) && !item.ProyectoRolUsuario.EstaBloqueado && item.Proyecto.TipoAcceso != (short)TipoAcceso.Publico));
        }

        /// <summary>
        /// Obtiene las clausulas adicionales de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet de usuario con las clausulas</returns>
        public DataWrapperUsuario ObtenerClausulasRegitroProyecto(Guid pProyectoID)
        {
            DataWrapperUsuario dataWrapperUsuario = new DataWrapperUsuario();

            //ClausulaRegistro
            dataWrapperUsuario.ListaClausulaRegistro = mEntityContext.ClausulaRegistro.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();

            return dataWrapperUsuario;
        }

        /// <summary>
        /// Obtiene la política de cookies de un proyecto (del metaproyecto si no tiene el proyecto)
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet de usuario con las clausulas</returns>
        public DataWrapperUsuario ObtenerPoliticaCookiesProyecto(Guid pProyectoID)
        {
            DataWrapperUsuario dataWrapperUsaurio = new DataWrapperUsuario();

            dataWrapperUsaurio.ListaClausulaRegistro = mEntityContext.ClausulaRegistro.Where(item => item.ProyectoID.Equals(pProyectoID) && (item.Tipo.Equals((short)TipoClausulaAdicional.PoliticaCookiesCabecera) || item.Tipo.Equals((short)TipoClausulaAdicional.PoliticaCookiesUrlPagina))).ToList();

            if (dataWrapperUsaurio.ListaClausulaRegistro.Count == 0)
            {
                dataWrapperUsaurio.ListaClausulaRegistro = mEntityContext.ClausulaRegistro.Where(item => item.ProyectoID.Equals(ProyectoAD.MetaProyecto) && (item.Tipo.Equals((short)TipoClausulaAdicional.PoliticaCookiesCabecera) || item.Tipo.Equals((short)TipoClausulaAdicional.PoliticaCookiesUrlPagina))).ToList();
            }

            return dataWrapperUsaurio;
        }

        /// <summary>
        /// Obtiene la tabla ProyClausulasUsu de un usuario por el identificador
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Dataset de usuarios</returns>
        public DataWrapperUsuario ObtenerProyClausulasUsuPorUsuarioID(Guid pUsuarioID)
        {
            DataWrapperUsuario dataWrapperUsuario = new DataWrapperUsuario();

            dataWrapperUsuario.ListaProyRolUsuClausulaReg = mEntityContext.ProyRolUsuClausulaReg.Where(item => item.UsuarioID.Equals(pUsuarioID)).ToList();

            return dataWrapperUsuario;
        }

        /// <summary>
        /// Obtiene las clausulas adicionales indicadas.
        /// </summary>
        /// <param name="pListaClausulasID">Lista de las clausulas</param>
        /// <returns>DataSet de usuario con las clausulas</returns>
        public DataWrapperUsuario ObtenerClausulasRegitroPorID(List<Guid> pListaClausulasID)
        {
            DataWrapperUsuario dataWrapperUsuario = new DataWrapperUsuario();

            if (pListaClausulasID.Count > 0)
            {
                dataWrapperUsuario.ListaClausulaRegistro = mEntityContext.ClausulaRegistro.Where(item => pListaClausulasID.Contains(item.ClausulaID)).ToList();
            }
            return dataWrapperUsuario;
        }

        /// <summary>
        /// Obtiene las clausulas adicionales de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pUsuarioID">ID del usuario</param>
        /// <returns>DataSet de usuario con las clausulas</returns>
        public DataWrapperUsuario ObtenerClausulasRegitroProyectoYUsuario(Guid pProyectoID, Guid pUsuarioID)
        {
            DataWrapperUsuario dataWrapperUsuario = new DataWrapperUsuario();

            dataWrapperUsuario.ListaClausulaRegistro = mEntityContext.ClausulaRegistro.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperUsuario.ListaProyRolUsuClausulaReg = mEntityContext.ProyRolUsuClausulaReg.Where(item => item.ProyectoID.Equals(pProyectoID) && item.UsuarioID.Equals(pUsuarioID)).ToList();

            return dataWrapperUsuario;
        }

        /// <summary>
        /// Obtiene los usuarios que participan en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto del que se quieren saber sus usuarios</param>
        /// <param name="pListaUsuarios">Lista inicializada que se va a rellenar con los datos obtenidos de la consulta (lista de string si se busca todos los usuarios o lista de Guid si se filtra por caracteres)</param>
        public void ObtenerUsuariosParticipanEnProyecto(Guid pProyectoID, Dictionary<Guid, List<Guid>> pListaUsuarios)
        {
            var resultado = mEntityContext.ProyectoUsuarioIdentidad.Join(mEntityContext.Identidad, proyUsu => proyUsu.IdentidadID, identidad => identidad.IdentidadID, (proyUsu, identidad) =>
            new
            {
                ProyectoUsuarioIdentidad = proyUsu,
                Identidad = identidad
            }).Where(item => item.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID) && item.Identidad.ActualizaHome && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue).Select(item => new { item.ProyectoUsuarioIdentidad.UsuarioID, item.Identidad.PerfilID });

            foreach (var item in resultado)
            {
                if (!pListaUsuarios.ContainsKey(item.UsuarioID))
                {
                    pListaUsuarios.Add(item.UsuarioID, new List<Guid>());
                }
                if (!pListaUsuarios[item.UsuarioID].Contains(item.PerfilID))
                {
                    pListaUsuarios[item.UsuarioID].Add(item.PerfilID);
                }
            }
        }

        /// <summary>
        /// Obtiene el identificador de los usuarios que han hecho algún comentario en un documento en una comunidad
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pListaUsuarios">Lista de usuarios</param>
        /// <returns></returns>
        public void ObtenerUsuarioIDVotantesRecurso(Guid pDocumentoID, Dictionary<Guid, List<Guid>> pListaUsuarios)
        {
            //string query = " FROM VotoDocumento INNER JOIN Voto ON Voto.VotoID = VotoDocumento.VotoID INNER JOIN Identidad ON Voto.IdentidadID = Identidad.IdentidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN Persona ON Persona.PersonaID = Perfil.PersonaID WHERE VotoDocumento.DocumentoID = '" + pDocumentoID + "' AND Identidad.ActualizaHome = 1 AND Identidad.FechaBaja IS NULL AND Identidad.FechaExpulsion IS NULL ";

            var listaUsuario = mEntityContext.VotoDocumento.JoinVoto().JoinIdentidad().JoinPerfil().JoinPersona().Where(item => item.VotoDocumento.DocumentoID.Equals(pDocumentoID) && item.Identidad.ActualizaHome.Equals(true) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue).Select(item => new { item.Persona.UsuarioID, item.Identidad.PerfilID }).Distinct().ToList();

            //string select = "Persona.UsuarioID";
            //select += ", Identidad.PerfilID";

            foreach (var objeto in listaUsuario)
            {
                if (!pListaUsuarios.ContainsKey(objeto.UsuarioID.Value))
                {
                    pListaUsuarios.Add(objeto.UsuarioID.Value, new List<Guid>());
                }
                Guid perfilID = objeto.PerfilID;
                if (!pListaUsuarios[objeto.UsuarioID.Value].Contains(perfilID))
                {
                    pListaUsuarios[objeto.UsuarioID.Value].Add(perfilID);
                }
            }
        }

        /// <summary>
        /// Obtiene el identificador de los usuarios que han hecho algún comentario en un documento en una comunidad
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pListaUsuarios"></param>
        /// <returns></returns>
        public void ObtenerUsuarioIDPublicadoresComentarioRecursoEnProyecto(Guid pDocumentoID, Guid? pProyectoID, Dictionary<Guid, List<Guid>> pListaUsuarios)
        {
            var listaUsuario = mEntityContext.DocumentoComentario.JoinComentario().JoinIdentidad().JoinPerfil().JoinPersona().Where(item => item.DocumentoComentario.DocumentoID.Equals(pDocumentoID) && item.Identidad.ActualizaHome && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue);

            if (pProyectoID.HasValue)
            {
                listaUsuario = listaUsuario.Where(item => item.DocumentoComentario.ProyectoID.Value.Equals(pProyectoID.Value));
            }

            var nuevaLista = listaUsuario.Select(item => new { UsuarioID = item.Persona.UsuarioID.Value, item.Identidad.PerfilID }).Distinct().ToList();

            foreach (var objeto in nuevaLista)
            {
                if (!pListaUsuarios.ContainsKey(objeto.UsuarioID))
                {
                    pListaUsuarios.Add(objeto.UsuarioID, new List<Guid>());
                }
                Guid perfilID = objeto.PerfilID;
                if (!pListaUsuarios[objeto.UsuarioID].Contains(perfilID))
                {
                    pListaUsuarios[objeto.UsuarioID].Add(perfilID);
                }
            }
        }

        /// <summary>
        /// Obtiene el identificador de los perfiles afectados por un comentario en una comunidad
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pListaUsuarios">Identificador del proyecto</param>
        public void ObtenerUsuarioIDEditoresLectoresRecurso(Guid pDocumentoID, Dictionary<Guid, List<Guid>> pListaUsuarios)
        {
            var resultado = mEntityContext.DocumentoRolIdentidad.Join(mEntityContext.Perfil, documentoRol => documentoRol.PerfilID, perfil => perfil.PerfilID, (documentoRol, perfil) =>
            new
            {
                DocumentoRolIdentidad = documentoRol,
                Perfil = perfil
            }).Join(mEntityContext.Persona, docPerfil => docPerfil.Perfil.PersonaID, persona => persona.PersonaID, (docPerfil, persona) =>
            new
            {
                DocumentoRolIdentidad = docPerfil.DocumentoRolIdentidad,
                Perfil = docPerfil.Perfil,
                Persona = persona
            }).Where(item => item.DocumentoRolIdentidad.DocumentoID.Equals(pDocumentoID) && !item.Perfil.Eliminado && item.Persona.UsuarioID.HasValue).Select(item => new { UsuarioID = item.Persona.UsuarioID.Value, item.DocumentoRolIdentidad.PerfilID });

            foreach (var item in resultado)
            {
                if (!pListaUsuarios.ContainsKey(item.UsuarioID))
                {
                    pListaUsuarios.Add(item.UsuarioID, new List<Guid>());
                }
                if (!pListaUsuarios[item.UsuarioID].Contains(item.PerfilID))
                {
                    pListaUsuarios[item.UsuarioID].Add(item.PerfilID);
                }
            }
        }

        #region Métodos antiguos



        /// <summary>
        /// Obtiene un usuario por el identificador
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Nombre corto del usuario</returns>
        public string ObtenerNombreCortoUsuarioPorID(Guid pUsuarioID)
        {
            return mEntityContext.Usuario.Where(usuario => usuario.UsuarioID.Equals(pUsuarioID)).Select(item => item.NombreCorto).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene un usuario por el identificador. Tablas: "Usuario"
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Dataset de usuarios</returns>
        public EntityModel.Models.UsuarioDS.Usuario ObtenerUsuarioPorID(Guid pUsuarioID)
        {
            return mEntityContext.Usuario.Where(usuario => usuario.UsuarioID.Equals(pUsuarioID)).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene usuarios por el identificador. Tablas: "Usuario"
        /// </summary>
        /// <param name="pListaUsuario">Lista de identificadores del usuario</param>
        /// <returns>Dataset de usuario</returns>
        public List<EntityModel.Models.UsuarioDS.Usuario> ObtenerUsuariosPorID(List<Guid> pListaUsuario)
        {
            List<EntityModel.Models.UsuarioDS.Usuario> listaUsuarios = mEntityContext.Usuario.Where(usuario => pListaUsuario.Contains(usuario.UsuarioID)).ToList();
            return listaUsuarios;
        }

        /// <summary>
        /// Obtiene el ID de un usuario a partir de su nombre corto (null si el usuario no existe)
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del usuario</param>
        /// <returns></returns>
        public Guid? ObtenerUsuarioIDPorNombreCorto(string pNombreCorto)
        {
            Guid? resultado = mEntityContext.Usuario.Join(mEntityContext.Persona, usuario => usuario.UsuarioID, persona => persona.UsuarioID, (usuario, persona) =>
           new
           {
               Usuario = usuario,
               Persona = persona
           }).Join(mEntityContext.Perfil, usuarioPersona => usuarioPersona.Persona.PersonaID, perfil => perfil.PersonaID, (usuarioPersona, perfil) =>
           new
           {
               Usuario = usuarioPersona.Usuario,
               Persona = usuarioPersona.Persona,
               Perfil = perfil
           }).Where(item => item.Perfil.NombreCortoUsu.Equals(pNombreCorto)).Select(item => item.Usuario.UsuarioID).FirstOrDefault();
            if (resultado.HasValue && !resultado.Value.Equals(Guid.Empty))
            {
                return resultado.Value;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Obtiene el ID de un usuario a partir de su nombre corto (null si el usuario no existe)
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del usuario</param>
        /// <returns></returns>
        public Guid ObtenerUsuarioIDPorLogin(string pLogin)
        {
            Guid? resultado = mEntityContext.Usuario.Where(usuario => usuario.Login.Equals(pLogin) && usuario.EstaBloqueado.HasValue && !usuario.EstaBloqueado.Value).Select(item => item.UsuarioID).FirstOrDefault();
            if (resultado.HasValue && !resultado.Value.Equals(Guid.Empty))
            {
                return resultado.Value;
            }
            else
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Obtiene el ID de un usuario a partir de su login o email
        /// </summary>
        /// <param name="pNombreCorto">Login o email del usuario</param>
        /// <returns></returns>
        public Guid? ObtenerUsuarioIDPorLoginOEmail(string pLoginOEmail)
        {
            if (pLoginOEmail.Contains("@"))
            {
                return mEntityContext.Usuario.Join(mEntityContext.Persona, usuario => usuario.UsuarioID, persona => persona.UsuarioID, (usuario, persona) =>
            new
            {
                Usuario = usuario,
                Persona = persona
            }).Where(item => item.Persona.Email.Equals(pLoginOEmail.ToLower()) && item.Usuario.EstaBloqueado.HasValue && !item.Usuario.EstaBloqueado.Value && !item.Persona.Eliminado).Select(item => item.Usuario.UsuarioID).FirstOrDefault();
            }
            else
            {
                return mEntityContext.Usuario.Where(usuario => usuario.Login.Equals(pLoginOEmail)).Select(item => item.UsuarioID).FirstOrDefault();
            }

        }

        /// <summary>
        /// Obtiene una persona a partir de su documento acreditativo
        /// </summary>
        /// <param name="pValorDocumentoAcreditativo">Documento acreditativo de la persona</param>
        /// <returns></returns>
        public Guid ObtenerPersonaIDPorValorDocumentoAcreditativo(string pValorDocumentoAcreditativo)
        {
            return mEntityContext.Persona.Where(persona => persona.ValorDocumentoAcreditativo.Equals(pValorDocumentoAcreditativo)).Select(item => item.PersonaID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el login de un usuario a partir de su nombre corto
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del usuario</param>
        /// <returns></returns>
        public string ObtenerLoginUsuarioPorNombreCorto(string pNombreCorto)
        {
            return mEntityContext.Usuario.Where(usuario => usuario.NombreCorto.Equals(pNombreCorto)).Select(item => item.Login).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el ID de un usuario a partir de su id de tesauro (null si el usuario no existe)
        /// </summary>
        /// <param name="pTesauroID">Id del Tesauro</param>
        /// <returns></returns>
        public Guid? ObtenerUsuarioIDPorIDTesauro(Guid pTesauroID)
        {
            Guid usuarioID = mEntityContext.TesauroUsuario.JoinCategoriaTesauro().Where(item => item.CategoriaTesauro.CategoriaTesauroID.Equals(pTesauroID)).Select(item => item.TesauroUsuario.UsuarioID).FirstOrDefault();

            if (!usuarioID.Equals(Guid.Empty))
            {
                return usuarioID;
            }

            return null;
        }

        /// <summary>
        /// Obtiene el ID de un usuario a partir de su id de perfil (null si el usuario no existe)
        /// </summary>
        /// <param name="pPerfilID">Id del perfil</param>
        /// <returns></returns>
        public Guid? ObtenerUsuarioIDPorIDPerfil(Guid pPerfilID)
        {
            Guid? resultado = mEntityContext.Usuario.Join(mEntityContext.Persona, usuario => usuario.UsuarioID, persona => persona.UsuarioID, (usuario, persona) =>
            new
            {
                Usuario = usuario,
                Persona = persona
            }).Join(mEntityContext.Perfil, usuPers => usuPers.Persona.PersonaID, perfil => perfil.PersonaID, (usuPers, perfil) =>
            new
            {
                Usuario = usuPers.Usuario,
                Persona = usuPers.Persona,
                Perfil = perfil
            }).Where(item => item.Perfil.PerfilID.Equals(pPerfilID)).Select(item => item.Usuario.UsuarioID).FirstOrDefault();
            if (resultado.HasValue && !resultado.Value.Equals(Guid.Empty))
            {
                return resultado.Value;
            }
            else
            {
                return null;
            }

        }
        public Dictionary<Guid, String> ObtenerUsuariosIdParaAutocompletar(string pNombrePerfil, int pNumero)
        {
            return ObtenerUsuariosIDParaAutocompletarDeNombrePerfil(pNombrePerfil, pNumero);
        }

        /// <summary>
        /// Devuelve los usuarioID de una comunidad a partir del nombre de perfil para el autocompletar
        /// </summary>
        /// <param name="pNombrePerfil">Texto a buscar</param>
        /// <param name="pNumero">Numero de resultados</param>
        /// <returns>
        /// Diccionario con:
        /// Clave: UsuarioID
        /// Valor: Nombre de perfil
        /// </returns>
        public Dictionary<Guid, string> ObtenerUsuariosIDParaAutocompletarDeNombrePerfil(string pNombrePerfil, int pNumero)
        {
            Dictionary<Guid, string> listaUsuariosIDPorNombrePerfil = new Dictionary<Guid, string>();

            var consultaSQL = mEntityContext.Perfil.JoinPersona().Where(item => item.Perfil.NombrePerfil.ToLower().Contains(pNombrePerfil.ToLower()) || item.Perfil.NombrePerfil.ToLower().StartsWith(pNombrePerfil.ToLower())).Select(item => new { item.Perfil.NombrePerfil, item.Persona.UsuarioID }).Take(pNumero).OrderByDescending(item => item.NombrePerfil);

            var resultado = consultaSQL.ToList();
            foreach (var item in resultado)
            {
                if (item.UsuarioID != null && !listaUsuariosIDPorNombrePerfil.ContainsKey((Guid)item.UsuarioID))
                {
                    listaUsuariosIDPorNombrePerfil.Add((Guid)item.UsuarioID, item.NombrePerfil);
                }
            }
            return listaUsuariosIDPorNombrePerfil;
        }

        public Dictionary<Guid, Guid> ObtenerUsuariosIDPorIDPerfil(List<Guid> pListaPerfilID)
        {
            return mEntityContext.Persona.Join(mEntityContext.Perfil, persona => persona.PersonaID, perfil => perfil.PersonaID, (persona, perfil) => new
            {
                Persona = persona,
                Perfil = perfil
            }).Where(item => pListaPerfilID.Contains(item.Perfil.PerfilID)).ToDictionary(item => item.Perfil.PerfilID, item => item.Persona.UsuarioID.Value);

        }

        /// <summary>
        /// Obtiene un usuario por el identificador
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Dataset de usuarios</returns>
        public DataWrapperUsuario ObtenerUsuarioCompletoPorID(Guid pUsuarioID)
        {
            DataWrapperUsuario dataWrapper = new DataWrapperUsuario();

            dataWrapper.ListaUsuario = mEntityContext.Usuario.Where(item => item.UsuarioID.Equals(pUsuarioID)).ToList();

            dataWrapper.ListaProyectoRolUsuario = mEntityContext.ProyectoRolUsuario.Where(item => item.UsuarioID.Equals(pUsuarioID)).ToList();

            dataWrapper.ListaProyectoUsuarioIdentidad = mEntityContext.ProyectoUsuarioIdentidad.Where(item => item.UsuarioID.Equals(pUsuarioID)).ToList();

            dataWrapper.ListaOrganizacionRolUsuario = mEntityContext.OrganizacionRolUsuario.Where(item => item.UsuarioID.Equals(pUsuarioID)).ToList();

            dataWrapper.ListaHistoricoProyectoUsuario = mEntityContext.HistoricoProyectoUsuario.Where(item => item.UsuarioID.Equals(pUsuarioID)).ToList();

            dataWrapper.ListaProyRolUsuClausulaReg = mEntityContext.ProyRolUsuClausulaReg.Where(item => item.UsuarioID.Equals(pUsuarioID)).ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene los usuarios por el identificador
        /// </summary>
        /// <param name="pListaUsuarioID">Lista de identificadores de los usuario</param>
        /// <returns>Dataset de usuarios</returns>
        public DataWrapperUsuario ObtenerUsuariosCompletosPorID(List<Guid> pListaUsuarioID)
        {
            DataWrapperUsuario dataWrapper = new DataWrapperUsuario();

            if (pListaUsuarioID == null || pListaUsuarioID.Count == 0)
            {
                return dataWrapper;
            }
            dataWrapper.ListaUsuario = mEntityContext.Usuario.Where(item => pListaUsuarioID.Contains(item.UsuarioID)).ToList();

            dataWrapper.ListaProyectoRolUsuario = mEntityContext.ProyectoRolUsuario.Where(item => pListaUsuarioID.Contains(item.UsuarioID)).ToList();

            dataWrapper.ListaProyectoUsuarioIdentidad = mEntityContext.ProyectoUsuarioIdentidad.Where(item => pListaUsuarioID.Contains(item.UsuarioID)).ToList();

            dataWrapper.ListaProyRolUsuClausulaReg = mEntityContext.ProyRolUsuClausulaReg.Where(item => pListaUsuarioID.Contains(item.UsuarioID)).ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene un usuario por una lista de perfiles
        /// </summary>
        /// <param name="pListaPerfiles">Lista de perfiles</param>
        /// <returns>Dataset de usuarios</returns>
        public DataWrapperUsuario ObtenerListaUsuariosPorPerfil(List<Guid> pListaPerfiles)
        {
            DataWrapperUsuario dataWrapper = new DataWrapperUsuario();
            if (pListaPerfiles.Count > 0)
            {
                dataWrapper.ListaUsuario = mEntityContext.Usuario.Join(mEntityContext.Persona, usuario => usuario.UsuarioID, persona => persona.UsuarioID, (usuario, persona) =>
            new
            {
                Usuario = usuario,
                Persona = persona
            }).Join(mEntityContext.Perfil, usuPers => usuPers.Persona.PersonaID, perfil => perfil.PersonaID, (usuPers, perfil) =>
             new
             {
                 Usuario = usuPers.Usuario,
                 Persona = usuPers.Persona,
                 Perfil = perfil
             }).Where(item => pListaPerfiles.Contains(item.Perfil.PerfilID)).Select(item => item.Usuario).ToList();

                dataWrapper.ListaProyectoRolUsuario = mEntityContext.ProyectoRolUsuario.Join(mEntityContext.Persona, usuario => usuario.UsuarioID, persona => persona.UsuarioID, (usuario, persona) =>
            new
            {
                ProyectoRolUsuario = usuario,
                Persona = persona
            }).Join(mEntityContext.Perfil, usuPers => usuPers.Persona.PersonaID, perfil => perfil.PersonaID, (usuPers, perfil) =>
             new
             {
                 ProyectoRolUsuario = usuPers.ProyectoRolUsuario,
                 Persona = usuPers.Persona,
                 Perfil = perfil
             }).Where(item => pListaPerfiles.Contains(item.Perfil.PerfilID)).Select(item => item.ProyectoRolUsuario).ToList();


                dataWrapper.ListaProyectoUsuarioIdentidad = mEntityContext.ProyectoUsuarioIdentidad.Join(mEntityContext.Persona, proyectoUsuarioIdentidad => proyectoUsuarioIdentidad.UsuarioID, persona => persona.UsuarioID, (proyectoUsuarioIdentidad, persona) =>
           new
           {
               ProyectoUsuarioIdentidad = proyectoUsuarioIdentidad,
               Persona = persona
           }).Join(mEntityContext.Perfil, usuPers => usuPers.Persona.PersonaID, perfil => perfil.PersonaID, (usuPers, perfil) =>
            new
            {
                ProyectoUsuarioIdentidad = usuPers.ProyectoUsuarioIdentidad,
                Persona = usuPers.Persona,
                Perfil = perfil
            }).Where(item => pListaPerfiles.Contains(item.Perfil.PerfilID)).Select(item => item.ProyectoUsuarioIdentidad).ToList();
            }

            dataWrapper.ListaProyectoUsuarioIdentidad = mEntityContext.ProyectoUsuarioIdentidad.JoinPersona().JoinPerfil().Where(item => pListaPerfiles.Contains(item.Perfil.PerfilID)).Select(item => item.ProyectoUsuarioIdentidad).ToList();

            dataWrapper.ListaInicioSesion = mEntityContext.InicioSesion.JoinPersona().JoinPerfil().Where(item => pListaPerfiles.Contains(item.Perfil.PerfilID)).Select(item => item.InicioSesion).ToList();

            dataWrapper.ListaHistoricoProyectoUsuario = mEntityContext.HistoricoProyectoUsuario.JoinPersona().JoinPerfil().Where(item => pListaPerfiles.Contains(item.Perfil.PerfilID)).Select(item => item.HistoricoProyectoUsuario).ToList();

            dataWrapper.ListaProyRolUsuClausulaReg = mEntityContext.ProyRolUsuClausulaReg.JoinPersona().JoinPerfil().Where(item => pListaPerfiles.Contains(item.Perfil.PerfilID)).Select(item => item.ProyRolUsuClausulaReg).ToList();

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene una lista de usuariosID y sus perfiles para un proyecto y un documento privado
        /// </summary>
        /// <param name="pProyectoID">ProyectoID proyecto en el que se encuentra el documento</param>
        /// <param name="pDocumentoID">DocumentoID privado</param>
        /// <returns>Lista de usuarios</returns>
        public Dictionary<Guid, List<Guid>> ObtenerDiccionarioUsuariosYPerfilesPorProyectoYDocPrivado(Guid pProyectoID, Guid? pDocumentoID)
        {
            Dictionary<Guid, List<Guid>> lista = new Dictionary<Guid, List<Guid>>();

            var query = mEntityContext.ProyectoUsuarioIdentidad.Join(mEntityContext.Identidad, proyUsu => proyUsu.IdentidadID, identidad => identidad.IdentidadID, (proyUsu, identidad) =>
            new
            {
                ProyectoUsuarioIdentidad = proyUsu,
                Identidad = identidad
            }).Where(item => item.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue).Select(item => new { item.ProyectoUsuarioIdentidad.UsuarioID, item.Identidad.PerfilID });

            if (pDocumentoID.HasValue)
            {
                query = query.Join(mEntityContext.DocumentoRolIdentidad, identidad => identidad.PerfilID, documento => documento.PerfilID, (identidad, documento) =>
                new
                {
                    Identidad = identidad,
                    DocumentoRolIdentdad = documento
                }).Where(item => item.DocumentoRolIdentdad.DocumentoID.Equals(pDocumentoID.Value)).Select(item => new { item.Identidad.UsuarioID, item.Identidad.PerfilID });
            }

            foreach (var id in query)
            {
                if (!lista.ContainsKey(id.UsuarioID))
                {
                    lista.Add(id.UsuarioID, new List<Guid>());
                }

                if (!lista[id.UsuarioID].Contains(id.PerfilID))
                {
                    lista[id.UsuarioID].Add(id.PerfilID);
                }

            }

            return lista;
        }

        /// <summary>
        /// Obtiene una lista de usuariosID y sus perfiles para un proyecto y un documento privado
        /// </summary>
        /// <param name="pProyectoID">ProyectoID proyecto en el que se encuentra el documento</param>
        /// <param name="pDocumentoID">DocumentoID privado</param>
        /// <returns>Lista de usuarios</returns>
        public Dictionary<Guid, List<Guid>> ObtenerDiccionarioGruposYPerfilesPorProyectoYDocPrivado(Guid pProyectoID, Guid? pDocumentoID)
        {
            Dictionary<Guid, List<Guid>> lista = new Dictionary<Guid, List<Guid>>();

            if (pDocumentoID.HasValue)
            {
                var resultado = mEntityContext.ProyectoUsuarioIdentidad.Join(mEntityContext.Identidad, proyecto => proyecto.IdentidadID, id => id.IdentidadID, (proyecto, id) =>
                   new
                   {
                       ProyectoUsuarioIdentidad = proyecto,
                       Identidad = id
                   }).Join(mEntityContext.GrupoIdentidadesParticipacion, proyIden => proyIden.Identidad.IdentidadID, grupo => grupo.IdentidadID, (proyIden, grupo) =>
                   new
                   {
                       ProyectoUsuarioIdentidad = proyIden.ProyectoUsuarioIdentidad,
                       Identidad = proyIden.Identidad,
                       GrupoIdentidadesParticipacion = grupo
                   })
                   .Join(mEntityContext.DocumentoRolGrupoIdentidades, proyIdenGru => proyIdenGru.GrupoIdentidadesParticipacion.GrupoID, docum => docum.GrupoID, (proyIdenGru, docum) =>
                new
                {
                    ProyectoUsuarioIdentidad = proyIdenGru.ProyectoUsuarioIdentidad,
                    Identidad = proyIdenGru.Identidad,
                    GrupoIdentidadesParticipacion = proyIdenGru.GrupoIdentidadesParticipacion,
                    DocumentoRolGrupoIdentidades = docum
                })
                .Where(item => item.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue
                && !item.Identidad.FechaExpulsion.HasValue && item.DocumentoRolGrupoIdentidades.DocumentoID.Equals(pDocumentoID.Value)).Select(item => new { item.ProyectoUsuarioIdentidad.UsuarioID, item.Identidad.PerfilID }).ToList();
                foreach (var fila in resultado)
                {
                    if (!lista.ContainsKey(fila.UsuarioID))
                    {
                        lista.Add(fila.UsuarioID, new List<Guid>());
                    }
                    if (resultado.Count > 1)
                    {
                        if (!lista[fila.UsuarioID].Contains(fila.PerfilID))
                        {
                            lista[fila.UsuarioID].Add(fila.PerfilID);
                        }
                    }
                }
            }
            else
            {
                var resultado = mEntityContext.ProyectoUsuarioIdentidad.Join(mEntityContext.Identidad, proyecto => proyecto.IdentidadID, id => id.IdentidadID, (proyecto, id) =>
                new
                {
                    ProyectoUsuarioIdentidad = proyecto,
                    Identidad = id
                }).Join(mEntityContext.GrupoIdentidadesParticipacion, proyIden => proyIden.Identidad.IdentidadID, grupo => grupo.IdentidadID, (proyIden, grupo) =>
                 new
                 {
                     ProyectoUsuarioIdentidad = proyIden.ProyectoUsuarioIdentidad,
                     Identidad = proyIden.Identidad,
                     GrupoIdentidadesParticipacion = grupo
                 })
                .Where(item => item.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue
                && !item.Identidad.FechaExpulsion.HasValue).Select(item => new { item.ProyectoUsuarioIdentidad.UsuarioID, item.Identidad.PerfilID }).ToList();

                foreach (var fila in resultado)
                {
                    if (!lista.ContainsKey(fila.UsuarioID))
                    {
                        lista.Add(fila.UsuarioID, new List<Guid>());
                    }
                    if (resultado.Count > 1)
                    {
                        if (!lista[fila.UsuarioID].Contains(fila.PerfilID))
                        {
                            lista[fila.UsuarioID].Add(fila.PerfilID);
                        }
                    }

                }
            }
            return lista;
        }



        public Dictionary<Guid, List<Guid>> ObtenerDiccionarioGruposYPerfilesPorListaGruposID(List<Guid> pListaGruposEditoresEliminadosEdiccionRecursoPrivado)
        {
            Dictionary<Guid, List<Guid>> lista = new Dictionary<Guid, List<Guid>>();
            if (pListaGruposEditoresEliminadosEdiccionRecursoPrivado.Count > 0)
            {
                var resultado = mEntityContext.ProyectoUsuarioIdentidad.Join(mEntityContext.Identidad, proyecto => proyecto.IdentidadID, id => id.IdentidadID, (proyecto, id) =>
                 new
                 {
                     ProyectoUsuarioIdentidad = proyecto,
                     Identidad = id
                 }).Join(mEntityContext.GrupoIdentidadesParticipacion, proIde => proIde.Identidad.IdentidadID, grupo => grupo.IdentidadID, (proIde, grupo) =>
                 new
                 {
                     ProyectoUsuarioIdentidad = proIde.ProyectoUsuarioIdentidad,
                     Identidad = proIde.Identidad,
                     GrupoIdentidadesParticipacion = grupo
                 }).Where(item => pListaGruposEditoresEliminadosEdiccionRecursoPrivado.Contains(item.GrupoIdentidadesParticipacion.GrupoID)).Select(item => new { item.ProyectoUsuarioIdentidad.UsuarioID, item.Identidad.PerfilID });

                foreach (var fila in resultado)
                {
                    lista.Add(fila.UsuarioID, new List<Guid>());

                    if (!lista[fila.UsuarioID].Contains(fila.PerfilID))
                    {
                        lista[fila.UsuarioID].Add(fila.PerfilID);
                    }
                }

            }


            return lista;
        }

        /// <summary>
        /// Obtiene una lista de usuarios de un proyecto
        /// </summary>
        /// <param name="pProyectoID">ProyectoID</param>
        /// <returns>Lista de usuarios</returns>
        public List<Guid> ObtenerListaGruposPorProyecto(Guid pProyectoID)
        {
            List<Guid> lista = new List<Guid>();

            lista = mEntityContext.GrupoIdentidadesProyecto.Where(grupoIdentidadesProyecto => grupoIdentidadesProyecto.ProyectoID.Equals(pProyectoID)).Select(item => item.GrupoID).ToList();

            return lista;
        }

        /// <summary>
        /// Obtiene un usuario por el login
        /// </summary>
        /// <param name="pLogin">Login del usuario</param>
        /// <param name="pObtenerInicioSesion">TRUE si se debe obtener el inicio de sesión, FALSE en caso contrario</param>
        /// <param name="pObtenerSoloFilaUsuario">TRUE si se debe obtener sólo la fila de usuario, FALSE en caso contrario</param>
        /// <param name="pBuscarSoloPorLogin">TRUE si se debe buscar sólo por login, FALSE si también se puede buscar por email</param>
        /// <returns>Dataset de usuarios</returns>
        public DataWrapperUsuario ObtenerUsuarioPorLoginOEmail(string pLogin, bool pObtenerInicioSesion, bool pObtenerSoloFilaUsuario, bool pBuscarSoloPorLogin)
        {
            DataWrapperUsuario dataWrapperUsuario = new DataWrapperUsuario();

            if (pBuscarSoloPorLogin)
            {
                dataWrapperUsuario.ListaUsuario = mEntityContext.Usuario.Where(usuario => usuario.Login.Equals(pLogin) && usuario.EstaBloqueado.HasValue && !usuario.EstaBloqueado.Value).ToList();
            }
            else
            {
                dataWrapperUsuario.ListaUsuario = mEntityContext.Usuario.Join(mEntityContext.Persona, usu => usu.UsuarioID, pers => pers.UsuarioID, (usu, pers) =>
                 new
                 {
                     Usuario = usu,
                     Persona = pers
                 }).Where(item => item.Persona.Email.Equals(pLogin.ToLower()) && item.Usuario.EstaBloqueado.HasValue && !item.Usuario.EstaBloqueado.Value && !item.Persona.Eliminado).Select(item => item.Usuario)
                 .Concat(mEntityContext.Usuario.Join(mEntityContext.SolicitudNuevoUsuario, usu => usu.UsuarioID, solNuevo => solNuevo.UsuarioID, (usu, solNuevo) =>
                 new
                 {
                     Usuario = usu,
                     SolicitudNuevoUsuario = solNuevo
                 }).Join(mEntityContext.Solicitud, usuSolNuevo => usuSolNuevo.SolicitudNuevoUsuario.SolicitudID, sol => sol.SolicitudID, (usuSolNuevo, sol) =>
                 new
                 {
                     Usuario = usuSolNuevo.Usuario,
                     SolicitudNuevoUsuario = usuSolNuevo.SolicitudNuevoUsuario,
                     Solicitud = sol
                 }).Where(item => item.SolicitudNuevoUsuario.Email.Equals(pLogin) && item.Solicitud.Estado == 0 && item.Usuario.EstaBloqueado.HasValue && !item.Usuario.EstaBloqueado.Value).Select(item => item.Usuario))
                 .Concat(mEntityContext.Usuario.Join(mEntityContext.Persona, usu => usu.UsuarioID, persona => persona.UsuarioID, (usu, persona) =>
                 new
                 {
                     Usuario = usu,
                     Persona = persona
                 }).Join(mEntityContext.PersonaVinculoOrganizacion, usuPers => usuPers.Persona.PersonaID, pvinc => pvinc.PersonaID, (usuPers, pvinc) =>
                 new
                 {
                     Usuario = usuPers.Usuario,
                     Persona = usuPers.Persona,
                     PersonaVinculoOrganizacion = pvinc
                 }).Where(item => item.PersonaVinculoOrganizacion.EmailTrabajo.Equals(pLogin) && item.Usuario.EstaBloqueado.HasValue && !item.Usuario.EstaBloqueado.Value && !item.Persona.Eliminado).Select(item => item.Usuario)).ToList();
            }

            if (pObtenerSoloFilaUsuario)
            {
                if (pBuscarSoloPorLogin)
                {
                    dataWrapperUsuario.ListaProyectoUsuarioIdentidad = mEntityContext.ProyectoUsuarioIdentidad.Join(mEntityContext.Usuario, proyecto => proyecto.UsuarioID, usuario => usuario.UsuarioID, (proyecto, usuario) =>
                    new
                    {
                        ProyectoUsuarioIdentidad = proyecto,
                        Usuario = usuario
                    }).Join(mEntityContext.Identidad, proyectoUsuario => proyectoUsuario.ProyectoUsuarioIdentidad.IdentidadID, identidad => identidad.IdentidadID, (proyectoUsuario, identidad) =>
                    new
                    {
                        ProyectoUsuarioIdentidad = proyectoUsuario.ProyectoUsuarioIdentidad,
                        Usuario = proyectoUsuario.Usuario,
                        Identidad = identidad
                    }).Join(mEntityContext.PerfilPersona, proyUsuId => proyUsuId.Identidad.PerfilID, pfps => pfps.PerfilID, (proyUsuId, pfps) =>
                    new
                    {
                        ProyectoUsuarioIdentidad = proyUsuId.ProyectoUsuarioIdentidad,
                        Usuario = proyUsuId.Usuario,
                        Identidad = proyUsuId.Identidad,
                        PerfilPersona = pfps
                    }).Where(item => item.Usuario.Login.Equals(pLogin) && item.ProyectoUsuarioIdentidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && item.Identidad.Tipo.Equals((short)TiposIdentidad.Personal)).Select(item => item.ProyectoUsuarioIdentidad).ToList();
                }
                else
                {

                    dataWrapperUsuario.ListaProyectoUsuarioIdentidad = mEntityContext.ProyectoUsuarioIdentidad.Join(mEntityContext.Usuario, proyecto => proyecto.UsuarioID, usuario => usuario.UsuarioID, (proyecto, usuario) =>
                   new
                   {
                       ProyectoUsuarioIdentidad = proyecto,
                       Usuario = usuario
                   }).Join(mEntityContext.Identidad, proyectoUsuario => proyectoUsuario.ProyectoUsuarioIdentidad.IdentidadID, identidad => identidad.IdentidadID, (proyectoUsuario, identidad) =>
                   new
                   {
                       ProyectoUsuarioIdentidad = proyectoUsuario.ProyectoUsuarioIdentidad,
                       Usuario = proyectoUsuario.Usuario,
                       Identidad = identidad
                   }).Join(mEntityContext.PerfilPersona, proyUsuId => proyUsuId.Identidad.PerfilID, pfps => pfps.PerfilID, (proyUsuId, pfps) =>
                   new
                   {
                       ProyectoUsuarioIdentidad = proyUsuId.ProyectoUsuarioIdentidad,
                       Usuario = proyUsuId.Usuario,
                       Identidad = proyUsuId.Identidad,
                       PerfilPersona = pfps
                   }).Join(mEntityContext.Persona, proUsuIdPfps => proUsuIdPfps.Usuario.UsuarioID, persona => persona.UsuarioID, (proUsuIdPfps, persona) =>
                   new
                   {
                       ProyectoUsuarioIdentidad = proUsuIdPfps.ProyectoUsuarioIdentidad,
                       Usuario = proUsuIdPfps.Usuario,
                       Identidad = proUsuIdPfps.Identidad,
                       PerfilPersona = proUsuIdPfps.PerfilPersona,
                       Persona = persona
                   }).Where(item => item.Persona.Email.Equals(pLogin.ToLower()) && item.ProyectoUsuarioIdentidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && item.Identidad.Tipo.Equals((short)TiposIdentidad.Personal)).Select(item => item.ProyectoUsuarioIdentidad).ToList();
                }


            }
            return dataWrapperUsuario;
        }



        /// <summary>
        /// Obtiene la fila de un usuario por login o email
        /// </summary>
        /// <param name="pLogin"></param>
        /// <returns></returns>
        public EntityModel.Models.UsuarioDS.Usuario ObtenerFilaUsuarioPorLoginOEmail(string pLogin)
        {
            if (pLogin != "")
            {
                if (pLogin.Contains("@"))
                {

                    return mEntityContext.Usuario.Join(mEntityContext.Persona, usu => usu.UsuarioID, pers => pers.UsuarioID, (usu, pers) =>
                      new
                      {
                          Usuario = usu,
                          Persona = pers
                      }).Where(item => item.Persona.Email.Equals(pLogin.ToLower()) && (!item.Usuario.EstaBloqueado.HasValue || !item.Usuario.EstaBloqueado.Value) && !item.Persona.Eliminado).Select(item => item.Usuario)
                      .Concat(mEntityContext.Usuario.Join(mEntityContext.SolicitudNuevoUsuario, usu => usu.UsuarioID, solic => solic.UsuarioID, (usu, solic) =>
                      new
                      {
                          Usuario = usu,
                          SolicitudNuevoUsuario = solic,
                      }).Join(mEntityContext.Solicitud, usuSol => usuSol.SolicitudNuevoUsuario.SolicitudID, solicitud => solicitud.SolicitudID, (usuSol, solicitud) =>
                      new
                      {
                          Usuario = usuSol.Usuario,
                          SolicitudNuevoUsuario = usuSol.SolicitudNuevoUsuario,
                          Solicitud = solicitud
                      }).Where(item => item.SolicitudNuevoUsuario.Email.Equals(pLogin) && (!item.Usuario.EstaBloqueado.HasValue || !item.Usuario.EstaBloqueado.Value) && item.Solicitud.Estado == 0).Select(item => item.Usuario)).ToList().FirstOrDefault();
                }
                else
                {
                    return mEntityContext.Usuario.FirstOrDefault(usuario => usuario.Login.Equals(pLogin));
                }
            }
            return null;
        }

        /// <summary>
        /// Obtiene la fila de un usuario por login o email
        /// </summary>
        /// <param name="pLogin"></param>
        /// <returns></returns>
        public EntityModel.Models.UsuarioDS.Usuario ObtenerFilaUsuarioBloqueadoPorLoginOEmail(string pLogin)
        {
            if (pLogin != "")
            {
                if (pLogin.Contains("@"))
                {

                    return mEntityContext.Usuario.Join(mEntityContext.Persona, usu => usu.UsuarioID, pers => pers.UsuarioID, (usu, pers) =>
                      new
                      {
                          Usuario = usu,
                          Persona = pers
                      }).Where(item => item.Persona.Email.Equals(pLogin.ToLower()) && (item.Usuario.EstaBloqueado.HasValue && !item.Usuario.EstaBloqueado.Value) && !item.Persona.Eliminado).Select(item => item.Usuario).ToList().FirstOrDefault();
                }
                else
                {
                    return mEntityContext.Usuario.FirstOrDefault(usuario => usuario.Login.Equals(pLogin));
                }
            }
            return null;
        }

        /// <summary>
        /// Obtiene un dataset con "UsuarioID","Login","Nombre", "Apellidos" entre los administradores de una organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion de la que queremos obtener sus administradores</param>
        /// <returns>Dataset sin tipar</returns>
        public UsuarioPersona ObtenerAdministradoresPorOrganizacion(Guid pOrganizacionID)
        {
            return mEntityContext.Usuario.Join(mEntityContext.Persona, usu => usu.UsuarioID, pers => pers.UsuarioID, (usu, pers) =>
             new
             {
                 Usuario = usu,
                 Persona = pers
             }).Join(mEntityContext.AdministradorOrganizacion, usuPers => usuPers.Usuario.UsuarioID, admin => admin.UsuarioID, (usuPers, admin) =>
             new
             {
                 Usuario = usuPers.Usuario,
                 Persona = usuPers.Persona,
                 AdministradorOrganizacion = admin
             }).Where(item => item.AdministradorOrganizacion.OrganizacionID.Equals(pOrganizacionID)).Select(item => new UsuarioPersona { UsuarioID = item.Usuario.UsuarioID, Login = item.Usuario.Login, Nombre = item.Persona.Nombre, Apellidos = item.Persona.Apellidos }).ToList().FirstOrDefault();

        }

        /// <summary>
        /// Valida si un login ya lo tiene asignado otro usuario del sistema
        /// </summary>
        /// <param name="pUsuario">Fila de usuario</param>
        /// <returns>TRUE en caso de existir, FALSE en caso contrario</returns>
        public bool EstaLoginUsuarioAsignado(EntityModel.Models.UsuarioDS.Usuario pUsuario)
        {
            var resultado = mEntityContext.Usuario.Where(usuario => usuario.Login.Equals(pUsuario.Login) && usuario.EstaBloqueado.HasValue && !usuario.EstaBloqueado.Value).ToList();
            if (resultado.Count > 0)
            {
                if (mEntityContext.Entry(pUsuario).State == EntityState.Added)
                {
                    return true;
                }
                else
                {
                    return resultado.First().UsuarioID.CompareTo(pUsuario.UsuarioID) != 0;
                }
            }
            return false;
        }

        /// <summary>
        /// Valida si un usuario está asignado a personas
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario para validar</param>
        /// <returns>TRUE en caso de estar asignado, FALSE en caso contrario</returns>
        public bool EstaUsuarioAsignadoAPersonas(Guid pUsuarioID)
        {
            return mEntityContext.Usuario.Join(mEntityContext.Persona, usuario => usuario.UsuarioID, persona => persona.UsuarioID, (usuario, persona) =>
            new
            {
                Usuario = usuario,
                Persona = persona
            }).Any(item => item.Usuario.UsuarioID.Equals(pUsuarioID));
        }

        /// <summary>
        /// Comprueba si un usuario tiene actiavada la doble autenticación
        /// </summary>
        /// <param name="pLogin">Login del usuario</param>
        /// <returns></returns>
        public bool ComprobarDobleAutenticacionUsuario(Guid pUsuarioId)
        {
            return mEntityContext.Usuario.Where(usuario => usuario.UsuarioID.Equals(pUsuarioId)).Select(usuario => usuario.TwoFactorAuthentication).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene la contraseña de un usuario
        /// </summary>
        /// <param name="pUsuario">Fila de usuario</param>
        /// <returns>Devuelve el password o NULL en caso de no encontrarse</returns>
        public string ObtenerPasswordUsuario(EntityModel.Models.UsuarioDS.Usuario pUsuario)
        {
            string password = mEntityContext.Usuario.Where(usuario => usuario.Login.Equals(pUsuario.Login)).Select(item => item.Password).FirstOrDefault();
            if (password == null)
            {
                throw new ErrorLoginUsuario();
            }
            return password;
        }

        /// <summary>
        /// Establece la contraseña de un usuario
        /// </summary>
        /// <param name="pUsuario">Fila de usuario</param>
        /// <param name="pPassword">Password del usuario</param>
        public void EstablecerPasswordUsuario(EntityModel.Models.UsuarioDS.Usuario pUsuario, string pPassword)
        {
            var resultado = mEntityContext.Usuario.Where(usuario => usuario.Login.Equals(pUsuario.Login)).FirstOrDefault();
            if (resultado != null)
            {
                resultado.Password = pPassword;
                resultado.Version = 1;
                resultado.FechaCambioPassword = DateTime.Now;
                mEntityContext.SaveChanges();
            }
            else
            {
                throw new ErrorLoginUsuario();
            }

        }

        public void EstablecerPasswordUsuario(Guid pUsuarioID, string pPassword)
        {
            var resultado = mEntityContext.Usuario.Where(usuario => usuario.UsuarioID.Equals(pUsuarioID)).FirstOrDefault();
            if (resultado != null)
            {
                resultado.Password = pPassword;
                resultado.Version = 1;
                resultado.FechaCambioPassword = DateTime.Now;
                mEntityContext.SaveChanges();
            }
            else
            {
                throw new ErrorLoginUsuario();
            }
        }

        public void EstablecerLoginUsuario(Guid pUsuarioID, string pLogin)
        {
            var resultado = mEntityContext.Usuario.Where(usuario => usuario.UsuarioID.Equals(pUsuarioID)).FirstOrDefault();
            if (resultado != null)
            {
                resultado.Login = pLogin;
                mEntityContext.SaveChanges();
            }
        }

        /// <summary>
        /// Establece la caducidad de un usuario a la fecha actual
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        public void EstablecerCaducidadPasswordUsuario(Guid pUsuarioID)
        {
            var resultado = mEntityContext.Usuario.Where(usuario => usuario.UsuarioID.Equals(pUsuarioID)).FirstOrDefault();
            if (resultado != null)
            {
                resultado.FechaCambioPassword = DateTime.Now;
                mEntityContext.SaveChanges();
            }
        }

        /// <summary>
        /// Suma uno a uno de los contadores del usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pContador">String del contador a actualizar. Se pueden usar las constantes definidas en UsuarioAD: CONTADOR_NUMERO_ACCESOS</param>
        public void ActualizarContadorUsuarioNumAccesos(Guid pUsuarioID)
        {
            UsuarioContadores usuarioContadores = mEntityContext.UsuarioContadores.Where(contadores => contadores.UsuarioID.Equals(pUsuarioID)).FirstOrDefault();
            if (usuarioContadores == null)
            {
                // No hay datos para el usuario, agregar una nueva fila con el contador a 1
                UsuarioContadores usuarioContadoresNew = new UsuarioContadores()
                {
                    FechaUltimaVisita = DateTime.Now,
                    NumeroAccesos = 1,
                    UsuarioID = pUsuarioID
                };
                mEntityContext.UsuarioContadores.Add(usuarioContadoresNew);
            }
            else
            {
                // Actualizamos el contador para el usuario que se acaba de conectar
                usuarioContadores.NumeroAccesos = usuarioContadores.NumeroAccesos + 1;
                usuarioContadores.FechaUltimaVisita = DateTime.Now;
            }
            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Obtiene el número de accesos del usuario a la plataforma
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Número de accesos del usuario</returns>
        public int ObtenerContadorDeAccesosDeUsuario(Guid pUsuarioID)
        {
            int resultado = mEntityContext.UsuarioContadores.Where(item => item.UsuarioID.Equals(pUsuarioID)).Select(item => item.NumeroAccesos).FirstOrDefault();
            return resultado;
        }

        /// <summary>
        /// Obtiene la fecha del último acceso de un usuario a la plataforma
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Fecha del último acceso del usuario. DateTime.MinValue en caso de que no haya accedido aún</returns>
        public DateTime? ObtenerFechaUltimoAccesoDeUsuario(Guid pUsuarioID)
        {
            DateTime? resultado = mEntityContext.UsuarioContadores.Where(item => item.UsuarioID.Equals(pUsuarioID) && item.FechaUltimaVisita.HasValue).Select(item => item.FechaUltimaVisita.Value).FirstOrDefault();

            return resultado;
        }

        ///// <summary>
        ///// Obtiene las filas de los accesos a la plataforma de los usuarios desde la fecha actual filtrando según el criterio(años, meses, horas...)
        ///// </summary>
        ///// <param name="pCriterio">Cadena que indica el criterio que se va a aplicar para la diferencia de fechas (ver DATEDIFF SQLServer)</param>
        ///// <param name="pCantidad">Entero que indica la cantidad de años, meses, horas... que se van aplicar en el filtro</param>
        ///// <returns>Lista de filas de UsuarioContadores que cumplen el filtro de accesos desde una determinada fecha</returns>
        //public List<UsuarioContadores> ObtenerFilasUsuarioContadoresPorIntervalo(string pCriterio, int pCantidad, Guid pProyectoID)
        //{
        //    //TODO: Migrar a EF

        //    //DbFunctions.DiffDays
        //    DateTime.Now.Subtract(DateTime.Now).

        //    List<UsuarioContadores> lista = mEntityContext.UsuarioContadores.JoinPersona().JoinPerfil().JoinIdentidad().Where(item => item.UsuarioContadores.FechaUltimaVisita.Value);

        //    string sql = string.Format("SELECT DISTINCT UsuarioContadores.UsuarioID, UsuarioContadores.NumeroAccesos, UsuarioContadores.FechaUltimaVisita AS FechaUltimaVisita FROM UsuarioContadores INNER JOIN Persona ON UsuarioContadores.UsuarioID = Persona.UsuarioID INNER JOIN Perfil ON Persona.PersonaID = Perfil.PersonaID INNER JOIN Identidad ON Perfil.PerfilID = Identidad.PerfilID WHERE DATEDIFF({0}, fechaultimavisita, GETDATE()) <= {1} AND Identidad.ProyectoID = '{2}' AND Identidad.FechaBaja IS NULL AND Identidad.FechaExpulsion IS NULL", pCriterio, pCantidad, pProyectoID);
        //    UsuarioDS usuarioDS = new UsuarioDS();

        //    DbCommand command = ObtenerComando(sql);
        //    CargarDataSet(command, usuarioDS, "UsuarioContadores");

        //    foreach (UsuarioDS.UsuarioContadoresRow fila in usuarioDS.UsuarioContadores.Rows)
        //    {
        //        lista.Add(fila);
        //    }

        //    return lista;
        //}


        /// <summary>
        /// Comprueba si un usuario está bloqueado
        /// </summary>
        /// <param name="pUsuario">Fila de usuario para comprobar</param>
        /// <returns> TRUE en caso de estar bloqueado, FALSE en caso contrario</returns>
        public bool EstaBloqueadoUsuario(EntityModel.Models.UsuarioDS.Usuario pUsuario)
        {
            var resultado = mEntityContext.Usuario.FirstOrDefault(usuario => usuario.UsuarioID.Equals(pUsuario.UsuarioID));
            if (resultado != null && resultado.EstaBloqueado.HasValue)
            {
                return resultado.EstaBloqueado.Value;
            }
            else
            {
                throw new ErrorLoginUsuario();
            }
        }

        /// <summary>
        /// Comprueba si un usuario pasado por parámetro está bloqueado en un proyecto dado
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>TRUE si está bloqueado, FALSE en caso contrario</returns>
        public bool EstaBloqueadoUsuarioEnProyecto(Guid pUsuarioID, Guid pProyectoID)
        {
            var resultado = mEntityContext.ProyectoRolUsuario.FirstOrDefault(item => item.UsuarioID.Equals(pUsuarioID) && item.ProyectoID.Equals(pProyectoID));
            if (resultado != null)
            {
                return resultado.EstaBloqueado;
            }
            else
            {
                throw new ErrorLoginUsuario();
            }
        }

        /// <summary>
        /// Bloquea un usuario
        /// </summary>
        /// <param name="pUsuario">Fila de usuario para bloquear</param>
        public void BloquearUsuario(EntityModel.Models.UsuarioDS.Usuario pUsuario)
        {
            var resultado = mEntityContext.Usuario.Where(usuario => usuario.UsuarioID.Equals(pUsuario)).FirstOrDefault();
            if (resultado != null)
            {
                resultado.EstaBloqueado = true;
                mEntityContext.SaveChanges();
            }
            else
            {
                throw new ErrorLoginUsuario();
            }
        }

        /// <summary>
        /// Desbloquea un usuario
        /// </summary>
        /// <param name="pUsuario">Fila de usuario para desbloquear</param>
        public void DesbloquearUsuario(EntityModel.Models.UsuarioDS.Usuario pUsuario)
        {
            var resultado = mEntityContext.Usuario.Where(usuario => usuario.UsuarioID.Equals(pUsuario.UsuarioID)).FirstOrDefault();
            if (resultado != null)
            {
                resultado.EstaBloqueado = false;
            }
            else
            {
                throw new ErrorLoginUsuario();
            }
        }

        /// <summary>
        /// Asigna un usuario a una persona
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pPersonaID">Identificador de la persona</param>
        public void AsignarUsuarioAPersona(System.Guid pUsuarioID, System.Guid pOrganizacionID, System.Guid pPersonaID)
        {
            var resultado = mEntityContext.Persona.Where(persona => persona.PersonaID.Equals(pPersonaID)).FirstOrDefault();
            if (resultado != null)
            {
                resultado.UsuarioID = pUsuarioID;
            }
            else
            {
                throw new Exception();
            }

        }

        /// <summary>
        /// Comprueba si existe un usuario en la base de datos
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>TRUE si existe, FALSE en caso contrario</returns>
        public bool ExisteUsuarioEnBD(System.Guid pUsuarioID)
        {
            return mEntityContext.Usuario.Any(item => item.UsuarioID.Equals(pUsuarioID));
        }

        /// <summary>
        /// Comprueba si existe un usuario en la base de datos
        /// </summary>
        /// <param name="pNombreUsuario">Nombre del usuario</param>
        /// <returns>TRUE si existe, FALSE en caso contrario</returns>
        public bool ExisteUsuarioEnBD(string pNombreUsuario)
        {
            return mEntityContext.Usuario.Any(item => item.Login.Equals(pNombreUsuario));
        }

        /// <summary>
        /// Comprueba si existe un usuario en la base de datos
        /// </summary>
        /// <param name="pNombreUsuario">Nombre del usuario</param>
        /// <returns>TRUE si existe, FALSE en caso contrario</returns>
        public string ObtenerLoginLibre(string pNombreUsuario)
        {
            string nombreUsuarioCortado = pNombreUsuario;
            int numeroInicioContador = pNombreUsuario.Length;
            int numeroDigitos = 0;
            int numMax = 0;
            int? numero = 0;
            bool obtenidosTodosLosDigitos = false;

            if (numeroInicioContador < 12)
            {
                // Si el nombre no tiene 12 caracteres, el número empezará en la posición siguiente al último carácter (ej: juan -> juan5)
                numeroInicioContador++;

                // El número de dígitos a consultar serán todos los caracteres restantes (menos uno porque al entrar en el bucle se le suma uno)
                numeroDigitos = 12 - pNombreUsuario.Length - 1;
                obtenidosTodosLosDigitos = true;
            }
            while (numero.HasValue && numero.Value.Equals(numMax))
            {
                numeroDigitos++;
                numMax = (int)Math.Pow(10, numeroDigitos) - 1;
                numero = null;

                if (numeroInicioContador + numeroDigitos > 13)
                {
                    // Si el nombre del usuario y el número de dígitos suman más 12 caracteres, el siguiente dígito comenzará una posición antes
                    numeroInicioContador--;
                }

                if (nombreUsuarioCortado.Length + numeroDigitos > 12)
                {
                    // Si el nombre del usuario y el número de digitos suman más de 12 caracteres, hay que quitarle un carácter al nombre del usuario
                    nombreUsuarioCortado = nombreUsuarioCortado.Substring(0, nombreUsuarioCortado.Length - 1);
                }

                var query = mEntityContext.Usuario.Where(item => item.Login.StartsWith(nombreUsuarioCortado)).Select(item => item.Login.Substring(numeroInicioContador - 1, numeroDigitos));
                List<string> listaNumeros = null;
                if (EsPostgres() || EsOracle())
                {
                    listaNumeros = query.ToList();
                    foreach (string numerolist in listaNumeros.ToList())
                    {
                        if (!mEntityContext.IsNumeric(numerolist))
                        {
                            listaNumeros.Remove(numerolist);
                        }
                    }
                }
                else
                {
                    listaNumeros = query.Where(item => mEntityContext.IsNumeric(item)).ToList();
                }

                //Eliminamos el caracter - porque entity interpreta que es un número
                listaNumeros.Remove("-");

                if (listaNumeros != null && listaNumeros.Count > 0)
                {
                    numero = listaNumeros.Max(item => Convert.ToInt32(item));
                }

                //numero = mEntityContext.Usuario.Where(item => item.Login.StartsWith(nombreUsuarioCortado) && SqlFunctions.IsNumeric(item.Login.Substring(numeroInicioContador, numeroDigitos)).Value.Equals(1)).Max(item => Convert.ToInt32(item.Login.Substring(numeroInicioContador, numeroDigitos)));

                if (obtenidosTodosLosDigitos)
                {
                    if (numero.HasValue && numero.Value.ToString().Length < numeroDigitos)
                    {
                        // El nombre no tenía 12 caractéres y nos hemos traído de golpe el número completo de dígitos que había después del nombre. 
                        // Establecemos el número de digitos real y el número máximo asociado a ese número de dígitos: 
                        numeroDigitos = numero.Value.ToString().Length;
                        numMax = (int)Math.Pow(10, numeroDigitos) - 1;

                        // Cambio el valor para que sólo entre la primera vez por aquí
                        obtenidosTodosLosDigitos = false;
                    }
                    else if (!numero.HasValue)
                    {
                        // Pongo el número de dígitos a 1 para que genere el nombre corto con un 1, no con 10, 100...
                        numeroDigitos = 1;
                    }
                }

            }

            string resultado = nombreUsuarioCortado;

            if (numero.HasValue && numero.Value > 0)
            {
                // Hemos encontrado el número máximo, le sumamos uno
                resultado += (numero.Value + 1).ToString();
            }
            else
            {
                // No hemos encontrado el número, porque hay que comenzar serie (1, 10, 100, 1000...)
                resultado += "1";
                for (int i = 1; i < numeroDigitos; i++)
                {
                    resultado += "0";
                }

            }

            return resultado;
        }

        /// <summary>
        /// Obtiene el rol General asignado a un usuario
        /// </summary>
        /// <param name="pUsuario">Fila de usuario</param>
        /// <returns>Dataset de usuarios con el rol de usuario asignado a dicho usuario</returns>
        public GeneralRolUsuario ObtenerGeneralRolUsuario(EntityModel.Models.UsuarioDS.Usuario pUsuario)
        {
            Guid usuarioID = pUsuario.UsuarioID;
            return mEntityContext.GeneralRolUsuario.FirstOrDefault(item => item.UsuarioID.Equals(usuarioID));
        }

        /// <summary>
        /// Obtiene el rol de un usuario en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Dataset de usuarios con el rol de un usuario en un proyecto</returns>
        public ProyectoRolUsuario ObtenerRolUsuarioEnProyecto(Guid pProyectoID, Guid pUsuarioID)
        {
            return mEntityContext.ProyectoRolUsuario.FirstOrDefault(item => item.UsuarioID.Equals(pUsuarioID) && item.ProyectoID.Equals(pProyectoID));

        }

        /// <summary>
        /// Obtiene el rol de los usuarios en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pUsuarioID">Lista de identificadores de los usuarios</param>
        /// <returns>Dataset de usuarios con el rol de los usuarios en un proyecto</returns>
        public DataWrapperUsuario ObtenerRolListaUsuariosEnProyecto(Guid pProyectoID, List<Guid> pUsuariosID)
        {
            DataWrapperUsuario dataWrapperUsuario = new DataWrapperUsuario();
            dataWrapperUsuario.ListaProyectoRolUsuario = mEntityContext.ProyectoRolUsuario.Where(item => item.ProyectoID.Equals(pProyectoID) && pUsuariosID.Contains(item.UsuarioID)).ToList();
            return dataWrapperUsuario;

        }


        /// <summary>
        /// Obtiene los roles de todos los usuarios en un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de usuarios con los roles de usuarios en un proyecto</returns>
        public List<ProyectoRolUsuario> ObtenerRolesUsuariosEnProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return mEntityContext.ProyectoRolUsuario.Where(item => item.OrganizacionGnossID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID)).ToList();
        }

        #endregion

        public List<Guid> ObtenerUsuariosPertenecenGrupo(Guid pGrupoID, List<Guid> pListaUsuariosComprobar = null)
        {
            var query = mEntityContext.GrupoIdentidadesParticipacion.Join(mEntityContext.Identidad, grupo => grupo.IdentidadID, id => id.IdentidadID, (grupo, id) =>
             new
             {
                 GrupoIdentidadesParticipacion = grupo,
                 Identidad = id
             }).Join(mEntityContext.Perfil, grupoId => grupoId.Identidad.PerfilID, perfil => perfil.PerfilID, (grupoID, perfil) =>
             new
             {
                 GrupoIdentidadesParticipacion = grupoID.GrupoIdentidadesParticipacion,
                 Identidad = grupoID.Identidad,
                 Perfil = perfil
             }).Join(mEntityContext.Persona, grupoIdPerf => grupoIdPerf.Perfil.PersonaID, persona => persona.PersonaID, (grupoIdPerf, persona) =>
             new
             {
                 GrupoIdentidadesParticipacion = grupoIdPerf.GrupoIdentidadesParticipacion,
                 Identidad = grupoIdPerf.Identidad,
                 Perfil = grupoIdPerf.Perfil,
                 Persona = persona
             }).Where(item => item.GrupoIdentidadesParticipacion.GrupoID.Equals(pGrupoID) && item.Persona.UsuarioID.HasValue).Select(item => item.Persona.UsuarioID.Value);

            if (pListaUsuariosComprobar != null && pListaUsuariosComprobar.Count > 0)
            {
                //query= $" AND Persona.UsuarioID IN ('{string.Join("','", pListaUsuariosComprobar)}')";
                query = query.Where(item => pListaUsuariosComprobar.Contains(item));
            }

            return query.Distinct().ToList();
        }

        public void ValidarUsuario(Guid pUsuarioID)
        {
            EntityModel.Models.UsuarioDS.Usuario usuario = mEntityContext.Usuario.Where(item => item.UsuarioID.Equals(pUsuarioID)).FirstOrDefault();
            usuario.Validado = 1;
        }


        /// <summary>
        /// Obtiene una lista de identificadores de los usuarios que se han suscrito a alguna categoría del tesauro o a otros usuarios o se han hecho miembros de la comunidad
        /// </summary>
        /// <param name="pProyectoID">Identificador de la comunidad</param>
        /// <param name="pFechaBusqueda">Fecha a partir de la cual buscar recursos con actividad</param>
        /// <returns>Lista de identificadores de usuario</returns>
        public List<Guid> ObtenerUsuariosActivosEnFecha(Guid pProyectoID, DateTime pFechaBusqueda)
        {
            List<Guid> listaIDs = new List<Guid>();

            listaIDs = mEntityContext.Suscripcion.JoinSuscripcionTesauroProyecto().JoinCategoriaTesVinSuscrip().JoinIdentidad().JoinPerfil().JoinUsuario().Where(item => item.Suscripcion.FechaSuscripcion >= pFechaBusqueda && item.SuscripcionTesauroProyecto.ProyectoID == pProyectoID).Select(item => item.Usuario.UsuarioID).Union(mEntityContext.SuscripcionIdentidadProyecto.JoinSuscripcion().JoinIdentidad().JoinPerfil().JoinUsuario().Where(item => item.Suscripcion.FechaSuscripcion >= pFechaBusqueda && item.SuscripcionIdentidadProyecto.ProyectoID == pProyectoID).Select(item => item.Usuario.UsuarioID)).Union(mEntityContext.Identidad.JoinPerfil().JoinUsuario().Where(item => item.Identidad.FechaAlta >= pFechaBusqueda && item.Identidad.ProyectoID == pProyectoID).Select(item => item.Usuario.UsuarioID)).ToList();

            return listaIDs;
        }

        #endregion

        #endregion
    }
}
