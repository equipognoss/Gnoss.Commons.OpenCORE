using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Blog;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.PersonaDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Suscripcion;
using Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.AD.Suscripcion
{
    #region Enumeraciones

    /// <summary>
    /// Contiene las posibles periodicidades.
    /// </summary>
    public enum PeriodicidadSuscripcion
    {
        /// <summary>
        /// No enviar
        /// </summary>
        NoEnviar = -1,
        /// <summary>
        /// Diaria
        /// </summary>
        Diaria = 1,
        /// <summary>
        /// Semanal
        /// </summary>
        Semanal = 7
    }

    /// <summary>
    /// Enumeración para los tipos de suscripción
    /// </summary>
    public enum TipoSuscripciones
    {
        /// <summary>
        /// Suscripción a comunidades
        /// </summary>
        Comunidades = 0,
        /// <summary>
        /// Suscripción a personas
        /// </summary>
        Personas = 1,
        /// <summary>
        /// Suscripción a blogs
        /// </summary>
        Blogs = 2,
        /// <summary>
        /// Suscripción a organizacion
        /// </summary>
        Organizacion = 3
    }

    #endregion

    public class JoinIdentidadOrgPerfilOrg
    {
        public EntityModel.Models.IdentidadDS.Identidad IdentidadOrg { get; set; }
        public Perfil PerfilOrg { get; set; }
    }

    public class JoinIdentidadOrgPerfilOrgSuscripcionTesauroOrganizacion
    {
        public EntityModel.Models.IdentidadDS.Identidad IdentidadOrg { get; set; }
        public Perfil PerfilOrg { get; set; }
        public SuscripcionTesauroOrganizacion SuscripcionTesauroOrganizacion { get; set; }
    }

    public class JoinIdentidadOrgPerfilOrgSuscripcionTesauroOrganizacionSuscripcion
    {
        public EntityModel.Models.IdentidadDS.Identidad IdentidadOrg { get; set; }
        public Perfil PerfilOrg { get; set; }
        public SuscripcionTesauroOrganizacion SuscripcionTesauroOrganizacion { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
    }

    public class JoinIdentidadOrgPerfilOrgSuscripcionTesauroOrganizacionSuscripcionIdentidad
    {
        public EntityModel.Models.IdentidadDS.Identidad IdentidadOrg { get; set; }
        public Perfil PerfilOrg { get; set; }
        public SuscripcionTesauroOrganizacion SuscripcionTesauroOrganizacion { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
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

    public class JoinSuscripcionTesauroUsuarioSuscripcion
    {
        public SuscripcionTesauroUsuario SuscripcionTesauroUsuario { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
    }

    public class JoinSuscripcionTesauroOrganizacionSuscripcion
    {
        public SuscripcionTesauroOrganizacion SuscripcionTesauroOrganizacion { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
    }

    public class JoinSuscripcionTesauroProyectoSuscripcion
    {
        public SuscripcionTesauroProyecto SuscripcionTesauroProyecto { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
    }

    public class JoinCategoriaTesVinSuscripSuscripcion
    {
        public CategoriaTesVinSuscrip CategoriaTesVinSuscrip { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
    }

    public class JoinSuscripcionIdentidadProyectoSuscripcion
    {
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
    }

    public class JoinSuscripcionSuscripcionTesauroProyectoIdentidad
    {
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public SuscripcionTesauroProyecto SuscripcionTesauroProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinSuscripcionSuscripcionTesauroProyectoIdentidadProyecto
    {
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public SuscripcionTesauroProyecto SuscripcionTesauroProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Proyecto Proyecto { get; set; }
    }

    public class JoinSuscripcionTesauroOrganizacionSuscripcionIdentidad
    {
        public SuscripcionTesauroOrganizacion SuscripcionTesauroOrganizacion { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinCategoriaTesVinSuscripSuscripcionIdentidad
    {
        public CategoriaTesVinSuscrip CategoriaTesVinSuscrip { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinSuscripcionIdentidadProyectoSuscripcionIdentidad
    {
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinSuscripcionIdentidadProyectoSuscripcionIdentidadIdentSuscProy
    {
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentSuscProy { get; set; }
    }

    public class JoinSuscripcionTesauroOrganizacionSuscripcionIdentidadPerfil
    {
        public SuscripcionTesauroOrganizacion SuscripcionTesauroOrganizacion { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinSuscripcionTesauroProyectoCategoriaTesVinSuscrip
    {
        public SuscripcionTesauroProyecto SuscripcionTesauroProyecto { get; set; }
        public CategoriaTesVinSuscrip CategoriaTesVinSuscrip { get; set; }
    }

    public class JoinSuscripcionTesauroProyectoCategoriaTesVinSuscripSuscripcion
    {
        public SuscripcionTesauroProyecto SuscripcionTesauroProyecto { get; set; }
        public CategoriaTesVinSuscrip CategoriaTesVinSuscrip { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
    }

    public class JoinSuscripcionTesauroProyectoCategoriaTesVinSuscripSuscripcionIdentidad
    {
        public SuscripcionTesauroProyecto SuscripcionTesauroProyecto { get; set; }
        public CategoriaTesVinSuscrip CategoriaTesVinSuscrip { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinSuscripcionTesauroProyectoCategoriaTesVinSuscripSuscripcionIdentidadDocumentoWebAgCatTesauro
    {
        public SuscripcionTesauroProyecto SuscripcionTesauroProyecto { get; set; }
        public CategoriaTesVinSuscrip CategoriaTesVinSuscrip { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
    }

    public class JoinSuscripcionTesauroProyectoCategoriaTesVinSuscripSuscripcionIdentidadDocumentoWebAgCatTesauroTesauroProyecto
    {
        public SuscripcionTesauroProyecto SuscripcionTesauroProyecto { get; set; }
        public CategoriaTesVinSuscrip CategoriaTesVinSuscrip { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
        public TesauroProyecto TesauroProyecto { get; set; }
    }

    public class JoinSuscripcionTesauroProyectoSuscripcionIdentidad
    {
        public SuscripcionTesauroProyecto SuscripcionTesauroProyecto { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinSuscripcionTesauroProyectoSuscripcionIdentidadPerfil
    {
        public SuscripcionTesauroProyecto SuscripcionTesauroProyecto { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinCategoriaTesVinSuscripSuscripcionIdentidadPerfil
    {
        public CategoriaTesVinSuscrip CategoriaTesVinSuscrip { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinSuscripcionIdentidadProyectoSuscripcionIdentidadPerfil
    {
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinSuscripcionIdentidadProyectoSuscripcionIdentidadPerfilPersona
    {
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public Persona Persona { get; set; }
    }

    public class JoinSuscripcionIdentidadProyectoIdentidad
    {
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinSuscripcionIdentidadProyectoIdentidadPerfil
    {
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinSuscripcionIdentidadProyectoIdentidadPerfilPersona
    {
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public Persona Persona { get; set; }
    }

    public class JoinSuscripcionSuscripcionIdentidadProyecto
    {
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
    }

    public class JoinSuscripcionSuscripcionIdentidadProyectoIdentidad
    {
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinSuscripcionSuscripcionIdentidadProyectoIdentidadIdentidadSeguidor
    {
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentidadSeguidor { get; set; }
    }

    public class JoinSuscripcionSuscripcionIdentidadProyectoIdentidadIdentidadSeguidorIdentidadPerfilSeguidor
    {
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public SuscripcionIdentidadProyecto SuscripcionIdentidadProyecto { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentidadSeguidor { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad IdentidadPerfilSeguidor { get; set; }
    }

    public class JoinSuscripcionTesauroUsuarioPersona
    {
        public SuscripcionTesauroUsuario SuscripcionTesauroUsuario { get; set; }
        public Persona Persona { get; set; }
    }

    public class JoinSuscripcionTesauroUsuarioPersonaPerfil
    {
        public SuscripcionTesauroUsuario SuscripcionTesauroUsuario { get; set; }
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinSuscripcionTesauroUsuarioPersonaPerfilIdentidad
    {
        public SuscripcionTesauroUsuario SuscripcionTesauroUsuario { get; set; }
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinSuscripcionBlogBlog
    {
        public SuscripcionBlog SuscripcionBlog { get; set; }
        public Blog Blog { get; set; }
    }

    // SuscripcionBlog inner join Blog on Blog.BlogID=SuscripcionBlog.BlogID where

    public static class Joins
    {
        public static IQueryable<JoinSuscripcionBlogBlog> JoinBlog(this IQueryable<SuscripcionBlog> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Blog, suscripcionBlog => suscripcionBlog.BlogID, blog => blog.BlogID, (suscripcionBlog, blog) => new JoinSuscripcionBlogBlog
            {
                Blog = blog,
                SuscripcionBlog = suscripcionBlog
            });
        }

        public static IQueryable<JoinSuscripcionTesauroUsuarioPersonaPerfilIdentidad> JoinIdentidad(this IQueryable<JoinSuscripcionTesauroUsuarioPersonaPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Perfil.PerfilID, identidad => identidad.PerfilID, (item, identidad) => new JoinSuscripcionTesauroUsuarioPersonaPerfilIdentidad
            {

                Identidad = identidad,
                Persona = item.Persona,
                Perfil = item.Perfil,
                SuscripcionTesauroUsuario = item.SuscripcionTesauroUsuario
            });
        }

        public static IQueryable<JoinSuscripcionTesauroUsuarioPersonaPerfil> JoinPerfil(this IQueryable<JoinSuscripcionTesauroUsuarioPersona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Persona.PersonaID, perfil => perfil.PersonaID, (item, perfil) => new JoinSuscripcionTesauroUsuarioPersonaPerfil
            {
                Perfil = perfil,
                Persona = item.Persona,
                SuscripcionTesauroUsuario = item.SuscripcionTesauroUsuario
            });
        }

        public static IQueryable<JoinSuscripcionTesauroUsuarioPersona> JoinPersona(this IQueryable<SuscripcionTesauroUsuario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Persona, suscripcionTesauroUsuario => suscripcionTesauroUsuario.UsuarioID, persona => persona.UsuarioID, (suscripcionTesaruoUsuario, persona) => new JoinSuscripcionTesauroUsuarioPersona
            {
                Persona = persona,
                SuscripcionTesauroUsuario = suscripcionTesaruoUsuario
            });
        }

        public static IQueryable<JoinSuscripcionSuscripcionIdentidadProyectoIdentidadIdentidadSeguidorIdentidadPerfilSeguidor> JoinIdentidadPerfilSeguidor(this IQueryable<JoinSuscripcionSuscripcionIdentidadProyectoIdentidadIdentidadSeguidor> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.IdentidadSeguidor.PerfilID, identidadPerfilSeguidor => identidadPerfilSeguidor.PerfilID, (item, identidadPerfilSeguidor) => new JoinSuscripcionSuscripcionIdentidadProyectoIdentidadIdentidadSeguidorIdentidadPerfilSeguidor
            {
                Identidad = item.Identidad,
                IdentidadPerfilSeguidor = identidadPerfilSeguidor,
                IdentidadSeguidor = item.IdentidadSeguidor,
                Suscripcion = item.Suscripcion,
                SuscripcionIdentidadProyecto = item.SuscripcionIdentidadProyecto
            });
        }

        public static IQueryable<JoinSuscripcionSuscripcionIdentidadProyectoIdentidadIdentidadSeguidor> JoinIdentidadSeguidor(this IQueryable<JoinSuscripcionSuscripcionIdentidadProyectoIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.SuscripcionIdentidadProyecto.IdentidadID, identidad => identidad.IdentidadID, (item, identidadSeguidor) => new JoinSuscripcionSuscripcionIdentidadProyectoIdentidadIdentidadSeguidor
            {
                Identidad = item.Identidad,
                IdentidadSeguidor = identidadSeguidor,
                Suscripcion = item.Suscripcion,
                SuscripcionIdentidadProyecto = item.SuscripcionIdentidadProyecto
            });
        }

        public static IQueryable<JoinSuscripcionSuscripcionIdentidadProyectoIdentidad> JoinIdentidad(this IQueryable<JoinSuscripcionSuscripcionIdentidadProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Suscripcion.IdentidadID, identidad => identidad.IdentidadID, (item, identidad) => new JoinSuscripcionSuscripcionIdentidadProyectoIdentidad
            {
                Identidad = identidad,
                Suscripcion = item.Suscripcion,
                SuscripcionIdentidadProyecto = item.SuscripcionIdentidadProyecto
            });
        }

        public static IQueryable<JoinSuscripcionSuscripcionIdentidadProyecto> JoinSuscripcionIdentidadProyecto(this IQueryable<EntityModel.Models.Suscripcion.Suscripcion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.SuscripcionIdentidadProyecto, suscripcion => suscripcion.SuscripcionID, suscripcionIdentidadProyecto => suscripcionIdentidadProyecto.SuscripcionID, (suscripcion, suscripcionIdentidadProyecto) => new JoinSuscripcionSuscripcionIdentidadProyecto
            {
                SuscripcionIdentidadProyecto = suscripcionIdentidadProyecto,
                Suscripcion = suscripcion
            });
        }

        public static IQueryable<JoinSuscripcionIdentidadProyectoIdentidadPerfilPersona> JoinPersona(this IQueryable<JoinSuscripcionIdentidadProyectoIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Persona, item => item.Perfil.PersonaID, persona => persona.PersonaID, (item, persona) => new JoinSuscripcionIdentidadProyectoIdentidadPerfilPersona
            {
                Persona = persona,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                SuscripcionIdentidadProyecto = item.SuscripcionIdentidadProyecto
            });
        }

        public static IQueryable<JoinSuscripcionIdentidadProyectoIdentidadPerfil> JoinPerfil(this IQueryable<JoinSuscripcionIdentidadProyectoIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new JoinSuscripcionIdentidadProyectoIdentidadPerfil
            {
                Perfil = perfil,
                Identidad = item.Identidad,
                SuscripcionIdentidadProyecto = item.SuscripcionIdentidadProyecto
            });
        }

        public static IQueryable<JoinSuscripcionIdentidadProyectoIdentidad> JoinIdentidad(this IQueryable<SuscripcionIdentidadProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, suscripcionIdentidadProyecto => suscripcionIdentidadProyecto.IdentidadID, identidad => identidad.IdentidadID, (suscripcionIdentidadProyecto, identidad) => new JoinSuscripcionIdentidadProyectoIdentidad
            {
                Identidad = identidad,
                SuscripcionIdentidadProyecto = suscripcionIdentidadProyecto
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
                Suscripcion = item.Suscripcion,
                Identidad = item.Identidad,
                SuscripcionIdentidadProyecto = item.SuscripcionIdentidadProyecto
            });
        }

        public static IQueryable<JoinCategoriaTesVinSuscripSuscripcionIdentidadPerfil> JoinPerfil(this IQueryable<JoinCategoriaTesVinSuscripSuscripcionIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new JoinCategoriaTesVinSuscripSuscripcionIdentidadPerfil
            {
                Perfil = perfil,
                CategoriaTesVinSuscrip = item.CategoriaTesVinSuscrip,
                Suscripcion = item.Suscripcion,
                Identidad = item.Identidad
            });
        }

        public static IQueryable<JoinSuscripcionTesauroProyectoSuscripcionIdentidadPerfil> JoinPerfil(this IQueryable<JoinSuscripcionTesauroProyectoSuscripcionIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new JoinSuscripcionTesauroProyectoSuscripcionIdentidadPerfil
            {
                Perfil = perfil,
                Identidad = item.Identidad,
                Suscripcion = item.Suscripcion,
                SuscripcionTesauroProyecto = item.SuscripcionTesauroProyecto
            });
        }

        public static IQueryable<JoinSuscripcionTesauroProyectoSuscripcionIdentidad> JoinIdentidad(this IQueryable<JoinSuscripcionTesauroProyectoSuscripcion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Suscripcion.IdentidadID, identidad => identidad.IdentidadID, (item, identidad) => new JoinSuscripcionTesauroProyectoSuscripcionIdentidad
            {
                Identidad = identidad,
                Suscripcion = item.Suscripcion,
                SuscripcionTesauroProyecto = item.SuscripcionTesauroProyecto
            });
        }

        public static IQueryable<JoinSuscripcionTesauroProyectoCategoriaTesVinSuscripSuscripcionIdentidadDocumentoWebAgCatTesauroTesauroProyecto> JoinTesauroProyecto(this IQueryable<JoinSuscripcionTesauroProyectoCategoriaTesVinSuscripSuscripcionIdentidadDocumentoWebAgCatTesauro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.TesauroProyecto, item => item.DocumentoWebAgCatTesauro.TesauroID, tesauroProyecto => tesauroProyecto.TesauroID, (item, tesauroProyecto) => new JoinSuscripcionTesauroProyectoCategoriaTesVinSuscripSuscripcionIdentidadDocumentoWebAgCatTesauroTesauroProyecto
            {
                TesauroProyecto = tesauroProyecto,
                CategoriaTesVinSuscrip = item.CategoriaTesVinSuscrip,
                Identidad = item.Identidad,
                Suscripcion = item.Suscripcion,
                DocumentoWebAgCatTesauro = item.DocumentoWebAgCatTesauro,
                SuscripcionTesauroProyecto = item.SuscripcionTesauroProyecto
            });
        }

        public static IQueryable<JoinSuscripcionTesauroProyectoCategoriaTesVinSuscripSuscripcionIdentidadDocumentoWebAgCatTesauro> JoinDocumentoWebAgCatTesauro(this IQueryable<JoinSuscripcionTesauroProyectoCategoriaTesVinSuscripSuscripcionIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoWebAgCatTesauro, item => item.CategoriaTesVinSuscrip.CategoriaTesauroID, docuementoWebAgCatTesauro => docuementoWebAgCatTesauro.CategoriaTesauroID, (item, documentoWebAgCatTesauro) => new JoinSuscripcionTesauroProyectoCategoriaTesVinSuscripSuscripcionIdentidadDocumentoWebAgCatTesauro
            {
                DocumentoWebAgCatTesauro = documentoWebAgCatTesauro,
                CategoriaTesVinSuscrip = item.CategoriaTesVinSuscrip,
                Identidad = item.Identidad,
                Suscripcion = item.Suscripcion,
                SuscripcionTesauroProyecto = item.SuscripcionTesauroProyecto
            });
        }

        public static IQueryable<JoinSuscripcionTesauroProyectoCategoriaTesVinSuscripSuscripcionIdentidad> JoinIdentidad(this IQueryable<JoinSuscripcionTesauroProyectoCategoriaTesVinSuscripSuscripcion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Suscripcion.IdentidadID, identidad => identidad.IdentidadID, (item, identidad) => new JoinSuscripcionTesauroProyectoCategoriaTesVinSuscripSuscripcionIdentidad
            {
                Identidad = identidad,
                CategoriaTesVinSuscrip = item.CategoriaTesVinSuscrip,
                Suscripcion = item.Suscripcion,
                SuscripcionTesauroProyecto = item.SuscripcionTesauroProyecto
            });
        }

        public static IQueryable<JoinSuscripcionTesauroProyectoCategoriaTesVinSuscripSuscripcion> JoinSuscripcion(this IQueryable<JoinSuscripcionTesauroProyectoCategoriaTesVinSuscrip> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Suscripcion, item => item.SuscripcionTesauroProyecto.SuscripcionID, suscripcion => suscripcion.SuscripcionID, (item, suscripcion) => new JoinSuscripcionTesauroProyectoCategoriaTesVinSuscripSuscripcion
            {
                Suscripcion = suscripcion,
                CategoriaTesVinSuscrip = item.CategoriaTesVinSuscrip,
                SuscripcionTesauroProyecto = item.SuscripcionTesauroProyecto
            });
        }

        public static IQueryable<JoinSuscripcionTesauroProyectoCategoriaTesVinSuscrip> JoinCategoriaTesVinSuscrip(this IQueryable<SuscripcionTesauroProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CategoriaTesVinSuscrip, suscripcionTesauroProyecto => suscripcionTesauroProyecto.SuscripcionID, categoriaTesVinSuscrip => categoriaTesVinSuscrip.SuscripcionID, (suscripcionTesauroProyecto, categoriaTesVinSuscrip) => new JoinSuscripcionTesauroProyectoCategoriaTesVinSuscrip
            {
                CategoriaTesVinSuscrip = categoriaTesVinSuscrip,
                SuscripcionTesauroProyecto = suscripcionTesauroProyecto
            });
        }

        public static IQueryable<JoinSuscripcionTesauroOrganizacionSuscripcionIdentidadPerfil> JoinPerfil(this IQueryable<JoinSuscripcionTesauroOrganizacionSuscripcionIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.SuscripcionTesauroOrganizacion.OrganizacionID, perfil => perfil.PerfilID, (item, perfil) => new JoinSuscripcionTesauroOrganizacionSuscripcionIdentidadPerfil
            {
                Perfil = perfil,
                Identidad = item.Identidad,
                Suscripcion = item.Suscripcion,
                SuscripcionTesauroOrganizacion = item.SuscripcionTesauroOrganizacion
            });
        }

        public static IQueryable<JoinSuscripcionIdentidadProyectoSuscripcionIdentidadIdentSuscProy> JoinIdentSuscProy(this IQueryable<JoinSuscripcionIdentidadProyectoSuscripcionIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.SuscripcionIdentidadProyecto.IdentidadID, identidad => identidad.IdentidadID, (item, identSuscProy) => new JoinSuscripcionIdentidadProyectoSuscripcionIdentidadIdentSuscProy
            {
                Identidad = item.Identidad,
                Suscripcion = item.Suscripcion,
                SuscripcionIdentidadProyecto = item.SuscripcionIdentidadProyecto,
                IdentSuscProy = identSuscProy
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

        public static IQueryable<JoinCategoriaTesVinSuscripSuscripcionIdentidad> JoinIdentidad(this IQueryable<JoinCategoriaTesVinSuscripSuscripcion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Suscripcion.IdentidadID, identidad => identidad.IdentidadID, (item, identidad) => new JoinCategoriaTesVinSuscripSuscripcionIdentidad
            {
                Identidad = identidad,
                CategoriaTesVinSuscrip = item.CategoriaTesVinSuscrip,
                Suscripcion = item.Suscripcion
            });
        }

        public static IQueryable<JoinSuscripcionTesauroOrganizacionSuscripcionIdentidad> JoinIdentidad(this IQueryable<JoinSuscripcionTesauroOrganizacionSuscripcion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Suscripcion.IdentidadID, identidad => identidad.IdentidadID, (item, identidad) => new JoinSuscripcionTesauroOrganizacionSuscripcionIdentidad
            {
                Identidad = identidad,
                Suscripcion = item.Suscripcion,
                SuscripcionTesauroOrganizacion = item.SuscripcionTesauroOrganizacion
            });
        }

        public static IQueryable<JoinSuscripcionSuscripcionTesauroProyectoIdentidadProyecto> JoinProyecto(this IQueryable<JoinSuscripcionSuscripcionTesauroProyectoIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Proyecto, item => item.SuscripcionTesauroProyecto.ProyectoID, proyecto => proyecto.ProyectoID, (item, proyecto) => new JoinSuscripcionSuscripcionTesauroProyectoIdentidadProyecto
            {
                Proyecto = proyecto,
                Identidad = item.Identidad,
                Suscripcion = item.Suscripcion,
                SuscripcionTesauroProyecto = item.SuscripcionTesauroProyecto
            });
        }

        public static IQueryable<JoinSuscripcionSuscripcionTesauroProyectoIdentidad> JoinIdentidad(this IQueryable<JoinSuscripcionSuscripcionTesauroProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Suscripcion.IdentidadID, identidad => identidad.IdentidadID, (item, identidad) => new JoinSuscripcionSuscripcionTesauroProyectoIdentidad
            {
                Identidad = identidad,
                Suscripcion = item.Suscripcion,
                SuscripcionTesauroProyecto = item.SuscripcionTesauroProyecto
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

        public static IQueryable<JoinCategoriaTesVinSuscripSuscripcion> JoinSuscripcion(this IQueryable<CategoriaTesVinSuscrip> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Suscripcion, categoriaTesVinSuscrip => categoriaTesVinSuscrip.SuscripcionID, suscripcion => suscripcion.SuscripcionID, (categoriaTesVinSuscrip, suscripcion) => new JoinCategoriaTesVinSuscripSuscripcion
            {
                Suscripcion = suscripcion,
                CategoriaTesVinSuscrip = categoriaTesVinSuscrip
            });
        }

        public static IQueryable<JoinSuscripcionTesauroProyectoSuscripcion> JoinSuscripcion(this IQueryable<SuscripcionTesauroProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Suscripcion, suscripcionTesauroProyecto => suscripcionTesauroProyecto.SuscripcionID, suscripcion => suscripcion.SuscripcionID, (suscripcionTesauroProyecto, suscripcion) => new JoinSuscripcionTesauroProyectoSuscripcion
            {
                Suscripcion = suscripcion,
                SuscripcionTesauroProyecto = suscripcionTesauroProyecto
            });
        }

        public static IQueryable<JoinSuscripcionTesauroOrganizacionSuscripcion> JoinSuscripcion(this IQueryable<SuscripcionTesauroOrganizacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Suscripcion, suscripcionTesauroOrganizacion => suscripcionTesauroOrganizacion.SuscripcionID, suscripcion => suscripcion.SuscripcionID, (suscripcionTesauroOrganizacion, suscripcion) => new JoinSuscripcionTesauroOrganizacionSuscripcion
            {
                Suscripcion = suscripcion,
                SuscripcionTesauroOrganizacion = suscripcionTesauroOrganizacion
            });
        }

        public static IQueryable<JoinSuscripcionTesauroUsuarioSuscripcion> JoinSuscripcion(this IQueryable<SuscripcionTesauroUsuario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Suscripcion, suscripcionTesauroUsuario => suscripcionTesauroUsuario.SuscripcionID, suscripcion => suscripcion.SuscripcionID, (suscripcionTesauroUsuario, suscripcion) => new JoinSuscripcionTesauroUsuarioSuscripcion
            {
                Suscripcion = suscripcion,
                SuscripcionTesauroUsuario = suscripcionTesauroUsuario
            });
        }

        public static IQueryable<JoinIdentidadOrgPerfilOrgSuscripcionTesauroOrganizacionSuscripcionIdentidad> JoinIdentidad(this IQueryable<JoinIdentidadOrgPerfilOrgSuscripcionTesauroOrganizacionSuscripcion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Suscripcion.IdentidadID, identidad => identidad.IdentidadID, (item, identidad) => new JoinIdentidadOrgPerfilOrgSuscripcionTesauroOrganizacionSuscripcionIdentidad
            {
                Identidad = identidad,
                IdentidadOrg = item.IdentidadOrg,
                PerfilOrg = item.PerfilOrg,
                Suscripcion = item.Suscripcion,
                SuscripcionTesauroOrganizacion = item.SuscripcionTesauroOrganizacion
            });
        }

        public static IQueryable<JoinIdentidadOrgPerfilOrgSuscripcionTesauroOrganizacionSuscripcion> JoinSuscripcion(this IQueryable<JoinIdentidadOrgPerfilOrgSuscripcionTesauroOrganizacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Suscripcion, item => item.SuscripcionTesauroOrganizacion.SuscripcionID, suscripcion => suscripcion.SuscripcionID, (item, suscripcion) => new JoinIdentidadOrgPerfilOrgSuscripcionTesauroOrganizacionSuscripcion
            {
                Suscripcion = suscripcion,
                IdentidadOrg = item.IdentidadOrg,
                PerfilOrg = item.PerfilOrg,
                SuscripcionTesauroOrganizacion = item.SuscripcionTesauroOrganizacion
            });
        }

        public static IQueryable<JoinIdentidadOrgPerfilOrgSuscripcionTesauroOrganizacion> JoinSuscripcionTesauroOrganizacion(this IQueryable<JoinIdentidadOrgPerfilOrg> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.SuscripcionTesauroOrganizacion, item => item.PerfilOrg.OrganizacionID, suscripcionTesauroOrganizacion => suscripcionTesauroOrganizacion.OrganizacionID, (item, suscripcionTesauroOrganizacion) => new JoinIdentidadOrgPerfilOrgSuscripcionTesauroOrganizacion
            {
                IdentidadOrg = item.IdentidadOrg,
                PerfilOrg = item.PerfilOrg,
                SuscripcionTesauroOrganizacion = suscripcionTesauroOrganizacion
            });
        }

        public static IQueryable<JoinIdentidadOrgPerfilOrg> JoinPerfilOrg(this IQueryable<EntityModel.Models.IdentidadDS.Identidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, identidad => identidad.PerfilID, perfil => perfil.PerfilID, (identidad, perfil) => new JoinIdentidadOrgPerfilOrg
            {
                IdentidadOrg = identidad,
                PerfilOrg = perfil
            });
        }

        public static IQueryable<JoinSuscripcionSuscripcionTesauroProyectoCategoriaTesVinSuscrip> JoinCategoriaTesVinSuscrip(this IQueryable<JoinSuscripcionSuscripcionTesauroProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CategoriaTesVinSuscrip, item => item.Suscripcion.SuscripcionID, categoriaTesVincSuscrip => categoriaTesVincSuscrip.SuscripcionID, (item, categoriaTesVincSuscrip) => new JoinSuscripcionSuscripcionTesauroProyectoCategoriaTesVinSuscrip
            {
                Suscripcion = item.Suscripcion,
                SuscripcionTesauroProyecto = item.SuscripcionTesauroProyecto,
                CategoriaTesVinSuscrip = categoriaTesVincSuscrip
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
    }

    /// <summary>
    /// Clase que conecta con la BD para tratar los datos de suscripciones.
    /// </summary>
    public class SuscripcionAD : BaseAD
    {
        private EntityContext mEntityContext;

        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public SuscripcionAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD"></param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public SuscripcionAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
            this.CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Consultas

        private string sqlSelectSuscripciones;
        private string sqlSelectSuscripcionTesauroUsuario;
        private string sqlSelectSuscripcionTesauroOrganizacion;
        private string sqlSelectSuscripcionTesauroUsuarioSimple;
        private string sqlSelectSuscripcionTesauroOrganizacionSimple;
        private string sqlSelectSuscripcionTesauroProyecto;
        private string sqlSelectSuscripcionTesauroProyectoSimple;
        private string sqlSelectCategoriasTesVinSuscrip;
        private string sqlSelectCategoriasTesVinSuscripSimple;
        private string sqlSelectSuscripcionBlog;
        private string sqlSelectSuscripcionBlogSimple;
        private string sqlSelectSuscripcionIdentidadProyecto;
        private string sqlSelectSuscripcionesAUsuario;

        private string sqlSelectSuscripcionIdentidadProyectoSimple;

        #endregion

        #region DataAdapter

        #region Suscripcion

        private string sqlSuscripcionInsert;
        private string sqlSuscripcionDelete;
        private string sqlSuscripcionModify;

        #endregion

        #region SuscripcionBlog

        private string sqlSuscripcionBlogInsert;
        private string sqlSuscripcionBlogDelete;
        private string sqlSuscripcionBlogModify;

        #endregion

        #region SuscripcionTesauroProyecto

        private string sqlSuscripcionTesauroProyectoInsert;
        private string sqlSuscripcionTesauroProyectoDelete;
        private string sqlSuscripcionTesauroProyectoModify;

        #endregion

        #region SuscripcionTesauroUsuario

        private string sqlSuscripcionTesauroUsuarioInsert;
        private string sqlSuscripcionTesauroUsuarioDelete;
        private string sqlSuscripcionTesauroUsuarioModify;

        #endregion

        #region SuscripcionTesauroOrganizacion

        private string sqlSuscripcionTesauroOrganizacionInsert;
        private string sqlSuscripcionTesauroOrganizacionDelete;
        private string sqlSuscripcionTesauroOrganizacionModify;

        #endregion


        #region CategoriaTesVinSuscrip

        private string sqlCategoriaTesVinSuscripInsert;
        private string sqlCategoriaTesVinSuscripDelete;
        private string sqlCategoriaTesVinSuscripModify;

        #endregion

        #region SuscripcionIdentidadProyecto

        private string sqlSuscripcionIdentidadProyectoInsert;
        private string sqlSuscripcionIdentidadProyectoDelete;
        private string sqlSuscripcionIdentidadProyectoModify;

        #endregion

        #endregion

        #region Métodos

        #region Públicos

        /// <summary>
        /// Actualiza los datos del dataset en la BD
        /// </summary>
        /// <param name="pSuscripcionDS">Dataset de suscripciones</param>
        public void ActualizarSuscripcion()
        {
            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Obtiene una lista con las identidades que son seguidas por la identidad
        /// </summary>
        /// <param name="pPerfilID">Perfil del seguidor</param>
        /// <returns>Lista con los ids de las identidades seguidas por la identidad</returns>
        public List<Guid> ObtenerListaIdentidadesSuscritasPerfil(Guid pPerfilID)
        {
            var query = mEntityContext.Suscripcion.JoinSuscripcionIdentidadProyecto().JoinIdentidad().JoinIdentidadSeguidor().JoinIdentidadPerfilSeguidor().Where(item => item.Identidad.PerfilID.Equals(pPerfilID)).Select(item => item.IdentidadPerfilSeguidor.IdentidadID);

            var query2 = mEntityContext.Identidad.JoinPerfilOrg().JoinSuscripcionTesauroOrganizacion().JoinSuscripcion().JoinIdentidad().Where(item => item.Identidad.PerfilID.Equals(pPerfilID) && (item.IdentidadOrg.Tipo.Equals((short)TiposIdentidad.ProfesionalCorporativo) || item.IdentidadOrg.Tipo.Equals((short)TiposIdentidad.Organizacion))).Select(item => item.IdentidadOrg.IdentidadID);

            List<Guid> listaIdentidadesSeguidas = query.Concat(query2).ToList();

            return listaIdentidadesSeguidas;
        }

        /// <summary>
        /// Obtiene una lista con las identidades que son seguidas por la identidad de la lista de claves
        /// </summary>
        /// <param name="pPerfilID">Perfil del seguidor</param>
        /// <returns>Lista con los ids de las identidades seguidas por la identidad</returns>
        public List<Guid> ComprobarListaIdentidadesSuscritasPerfil(Guid pPerfilID, List<Guid> pListaIdentidades)
        {

            var query = mEntityContext.Suscripcion.JoinSuscripcionIdentidadProyecto().JoinIdentidad().JoinIdentidadSeguidor().JoinIdentidadPerfilSeguidor().Where(item => item.Identidad.PerfilID.Equals(pPerfilID) && pListaIdentidades.Contains(item.IdentidadPerfilSeguidor.IdentidadID)).Select(item => item.IdentidadPerfilSeguidor.IdentidadID);

            var query2 = mEntityContext.Identidad.JoinPerfilOrg().JoinSuscripcionTesauroOrganizacion().JoinSuscripcion().JoinIdentidad().Where(item => item.Identidad.PerfilID.Equals(pPerfilID) && pListaIdentidades.Contains(item.IdentidadOrg.IdentidadID) &&  (item.IdentidadOrg.Tipo.Equals((short)TiposIdentidad.ProfesionalCorporativo) || item.IdentidadOrg.Tipo.Equals((short)TiposIdentidad.Organizacion))).Select(item => item.IdentidadOrg.IdentidadID);

            List<Guid> listaIdentidadesSeguidas = query.Concat(query2).ToList();

            return listaIdentidadesSeguidas;

        } 

        /// <summary>
        /// Obtiene las suscripciones de las identidades (las suscripciones, no sus elementos)
        /// </summary>
        /// <param name="pListaIdentidadesID">Lista de IdentidadID</param>
        /// <param name="pObtenerBloqueadas">True si queremos obtener las suscripciones bloquedas ademas de las no bloqueadas</param>
        /// <returns>SuscripcionDS cargado con las suscripciones (Suscripcion,SuscripcionTesauroUsuario,SuscripcionTesauroProyecto,
        /// CategoriaTesVinSuscrip)</returns>
        public DataWrapperSuscripcion ObtenerSuscripcionesDeListaIdentidades(List<Guid> pListaIdentidadesID, bool pObtenerBloqueadas)
        {
            DataWrapperSuscripcion suscDW = new DataWrapperSuscripcion();

            if (pListaIdentidadesID == null || pListaIdentidadesID.Count == 0)
            {
                return suscDW;
            }

            //Suscripcion
            var querySuscripcion = mEntityContext.Suscripcion.Where(item => pListaIdentidadesID.Contains(item.IdentidadID));

            //SuscripcionTesauroUsuario
            var querySuscTesUsuario = mEntityContext.SuscripcionTesauroUsuario.JoinSuscripcion().Where(item => pListaIdentidadesID.Contains(item.Suscripcion.IdentidadID));

            //SuscripcionTesauroOrganizacion
            var querySuscTesOrganizacion = mEntityContext.SuscripcionTesauroOrganizacion.JoinSuscripcion().Where(item => pListaIdentidadesID.Contains(item.Suscripcion.IdentidadID));

            //SuscripcionTesauroProyecto
            var querySuscTesProyecto = mEntityContext.SuscripcionTesauroProyecto.JoinSuscripcion().Where(item => pListaIdentidadesID.Contains(item.Suscripcion.IdentidadID));

            //CategoriaTesVinSuscrip
            var queryCatTesVinSuscrip = mEntityContext.CategoriaTesVinSuscrip.JoinSuscripcion().Where(item => pListaIdentidadesID.Contains(item.Suscripcion.IdentidadID));

            //SuscripcionIdentidadProyecto
            var querySuscIdentidadProyecto = mEntityContext.SuscripcionIdentidadProyecto.JoinSuscripcion().Where(item => pListaIdentidadesID.Contains(item.Suscripcion.IdentidadID));

            if (!pObtenerBloqueadas)
            {
                querySuscripcion = querySuscripcion.Where(item => item.Bloqueada.Equals(false));
                querySuscTesUsuario = querySuscTesUsuario.Where(item => item.Suscripcion.Bloqueada.Equals(false));
                querySuscTesOrganizacion = querySuscTesOrganizacion.Where(item => item.Suscripcion.Bloqueada.Equals(false));
                querySuscTesProyecto = querySuscTesProyecto.Where(item => item.Suscripcion.Bloqueada.Equals(false));
                queryCatTesVinSuscrip = queryCatTesVinSuscrip.Where(item => item.Suscripcion.Bloqueada.Equals(false));
                querySuscIdentidadProyecto = querySuscIdentidadProyecto.Where(item => item.Suscripcion.Bloqueada.Equals(false));
            }

            suscDW.ListaSuscripcion = querySuscripcion.ToList();

            suscDW.ListaSuscripcionTesauroUsuario = querySuscTesUsuario.Select(item => item.SuscripcionTesauroUsuario).ToList();

            suscDW.ListaSuscripcionTesauroOrganizacion = querySuscTesOrganizacion.Select(item => item.SuscripcionTesauroOrganizacion).ToList();

            suscDW.ListaSuscripcionTesauroProyecto = querySuscTesProyecto.Select(item => item.SuscripcionTesauroProyecto).ToList();

            suscDW.ListaCategoriaTesVinSuscrip = queryCatTesVinSuscrip.Select(item => item.CategoriaTesVinSuscrip).ToList();

            suscDW.ListaSuscripcionIdentidadProyecto = querySuscIdentidadProyecto.Select(item => item.SuscripcionIdentidadProyecto).ToList();

            return suscDW;
        }

        /// <summary>
        /// Obtiene la identidad de un seguidor.
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Clave de la base de recursos del usuario</returns>
        public Guid ObtenerIdentidadSeguidorPorIDSuscripcion(Guid pSuscripcionID)
        {
            return mEntityContext.SuscripcionTesauroUsuario.JoinPersona().JoinPerfil().JoinIdentidad().Where(item => item.SuscripcionTesauroUsuario.SuscripcionID.Equals(pSuscripcionID) && item.Identidad.ProyectoID.Equals(new Guid("11111111-1111-1111-1111-111111111111"))).Select(item => item.Identidad.IdentidadID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene la identidad de un seguidor en una comunidad.
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Clave de la base de recursos del usuario</returns>
        public Guid ObtenerIdentidadSeguidorComPorIDSuscripcion(Guid pSuscripcionID)
        {
            return mEntityContext.SuscripcionIdentidadProyecto.Where(item => item.SuscripcionID.Equals(pSuscripcionID)).Select(item => item.IdentidadID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene la identidad de un seguidor de un blog.
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Identidad del seguidor de un blog</returns>
        public Guid ObtenerIdentidadSeguidorBlogPorIDSuscripcion(Guid pSuscripcionID)
        {
            return mEntityContext.SuscripcionBlog.JoinBlog().Where(item => item.SuscripcionBlog.SuscripcionID.Equals(pSuscripcionID)).Select(item => item.Blog.BlogID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene las suscripciones de una identidad (las suscripciones, no sus elementos)
        /// </summary>
        /// <param name="pIdentidadID">Identificador de identidad</param>
        /// <param name="pObtenerBloqueadas">TRUE si queremos obtener las suscripciones bloquedas además de las no bloqueadas</param>
        /// <returns>Dataset de suscripciones cargado con las suscripciones (Suscripcion,SuscripcionTesauroUsuario,
        /// SuscripcionTesauroProyecto,CategoriaTesVinSuscrip)</returns>
        public DataWrapperSuscripcion ObtenerSuscripcionesDeIdentidad(Guid pIdentidadID, bool pObtenerBloqueadas)
        {
            DataWrapperSuscripcion suscDW = new DataWrapperSuscripcion();

            //Suscripcion
            var querySuscripcion = mEntityContext.Suscripcion.Where(item => item.IdentidadID.Equals(pIdentidadID));

            //SuscripcionTesauroUsuario
            var querySuscTesUsuario = mEntityContext.SuscripcionTesauroUsuario.JoinSuscripcion().Where(item => item.Suscripcion.IdentidadID.Equals(pIdentidadID));

            //SuscripcionTesauroOrganizacion
            var querySuscTesOrganizacion = mEntityContext.SuscripcionTesauroOrganizacion.JoinSuscripcion().Where(item => item.Suscripcion.IdentidadID.Equals(pIdentidadID));

            //SuscripcionTesauroProyecto
            var querySuscTesProyecto = mEntityContext.SuscripcionTesauroProyecto.JoinSuscripcion().Where(item => item.Suscripcion.IdentidadID.Equals(pIdentidadID));

            //CategoriaTesVinSuscrip
            var queryCatTesVinSuscrip = mEntityContext.CategoriaTesVinSuscrip.JoinSuscripcion().Where(item => item.Suscripcion.IdentidadID.Equals(pIdentidadID));

            //SuscripcionIdentidadProyecto
            var querySuscIdentidadProyecto = mEntityContext.SuscripcionIdentidadProyecto.JoinSuscripcion().Where(item => item.Suscripcion.IdentidadID.Equals(pIdentidadID));

            if (!pObtenerBloqueadas)
            {
                querySuscripcion = querySuscripcion.Where(item => item.Bloqueada.Equals(false));
                querySuscTesUsuario = querySuscTesUsuario.Where(item => item.Suscripcion.Bloqueada.Equals(false));
                querySuscTesOrganizacion = querySuscTesOrganizacion.Where(item => item.Suscripcion.Bloqueada.Equals(false));
                querySuscTesProyecto = querySuscTesProyecto.Where(item => item.Suscripcion.Bloqueada.Equals(false));
                queryCatTesVinSuscrip = queryCatTesVinSuscrip.Where(item => item.Suscripcion.Bloqueada.Equals(false));
                querySuscIdentidadProyecto = querySuscIdentidadProyecto.Where(item => item.Suscripcion.Bloqueada.Equals(false));
            }

            suscDW.ListaSuscripcion = querySuscripcion.ToList();
            suscDW.ListaSuscripcionTesauroUsuario = querySuscTesUsuario.Select(item => item.SuscripcionTesauroUsuario).ToList();
            suscDW.ListaSuscripcionTesauroOrganizacion = querySuscTesOrganizacion.Select(item => item.SuscripcionTesauroOrganizacion).ToList();
            suscDW.ListaSuscripcionTesauroProyecto = querySuscTesProyecto.Select(item => item.SuscripcionTesauroProyecto).ToList();
            suscDW.ListaCategoriaTesVinSuscrip = queryCatTesVinSuscrip.Select(item => item.CategoriaTesVinSuscrip).ToList();
            suscDW.ListaSuscripcionIdentidadProyecto = querySuscIdentidadProyecto.Select(item => item.SuscripcionIdentidadProyecto).ToList();

            return suscDW;
        }

        /// <summary>
        /// Obtiene un diccionario con clave las identidades para comprobar y un booleano que nos dice si le seguiomos en la comunidad(o en toda su actividad) o no
        /// </summary>
        /// <param name="pIdentidadID">IdentidadID</param>
        /// <param name="pListaIdentidadesComprobar">Lista de identidades para comprobar</param>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns></returns>
        public List<Guid> ObtenerListaIdentidadesSuscritasPorProyecto(Guid pIdentidadID, List<Guid> pListaIdentidadesComprobar, Guid pProyectoID)
        {
            List<Guid> identidadesSuscritas = null;

            if (pListaIdentidadesComprobar.Count > 0)
            {
                //Identidades a las que estoy suscrito de forma Explicita en la comunidad
                var queryIdentidadesExplicitas = mEntityContext.SuscripcionIdentidadProyecto.JoinSuscripcion().Where(item => item.Suscripcion.IdentidadID.Equals(pIdentidadID) && item.SuscripcionIdentidadProyecto.ProyectoID.Equals(pProyectoID) && pListaIdentidadesComprobar.Contains(item.SuscripcionIdentidadProyecto.IdentidadID)).Select(item => item.SuscripcionIdentidadProyecto.IdentidadID);

                //Identidades a las que estoy suscrito por estar suscrito a toda la actividad
                var subconsulta = mEntityContext.SuscripcionIdentidadProyecto.JoinSuscripcion().Where(item => item.Suscripcion.IdentidadID.Equals(pIdentidadID) && item.SuscripcionIdentidadProyecto.ProyectoID.Equals(ProyectoAD.MetaProyecto)).Select(item => item.SuscripcionIdentidadProyecto.IdentidadID);

                var queryIdentidadesTodaActividad = subconsulta.Join(mEntityContext.Identidad, subQuery => subQuery, identidad => identidad.IdentidadID, (subQuery, identidad) => new
                {
                    Identidad = identidad,
                    IdentidadesEnTodaSuActividad = subQuery
                }).Join(mEntityContext.Identidad, item => item.IdentidadesEnTodaSuActividad, identidad => identidad.PerfilID, (item, identidadEnProyecto) => new
                {
                    Identidad = item.Identidad,
                    IdentidadesEnTodaSuActividad = item.IdentidadesEnTodaSuActividad,
                    IdentidadEnProyecto = identidadEnProyecto
                }).Where(item => item.IdentidadEnProyecto.ProyectoID.Equals(pProyectoID) && pListaIdentidadesComprobar.Contains(item.IdentidadEnProyecto.IdentidadID)).Select(item => item.IdentidadEnProyecto.IdentidadID);

                identidadesSuscritas = queryIdentidadesExplicitas.Union(queryIdentidadesTodaActividad).ToList();
            }

            return identidadesSuscritas;
        }

        /// <summary>
        /// Obtiene las suscripciones a blogs de una identidad
        /// </summary>
        /// <param name="pIdentidadID">Identificador de identidad</param>
        /// <param name="pObtenerBloqueadas">TRUE si queremos obtener las suscripciones bloquedas además de las no bloqueadas</param>
        /// <returns>SuscripcionDS cargado con las suscripciones 
        /// (Suscripcion, SuscripcionTesauroUsuario, SuscripcionTesauroProyecto, CategoriaTesVinSuscrip)</returns>
        public DataWrapperSuscripcion ObtenerSuscripcionesDeBlog(Guid pIdentidadID, bool pObtenerBloqueadas)
        {
            DataWrapperSuscripcion suscDW = new DataWrapperSuscripcion();

            //Suscripcion
            var querySuscripcion = mEntityContext.Suscripcion.Where(item => item.IdentidadID.Equals(pIdentidadID));

            //SuscripcionBlog
            var querySuscripcionBlog = mEntityContext.SuscripcionBlog.Where(item => item.Suscripcion.IdentidadID.Equals(pIdentidadID));

            if (!pObtenerBloqueadas)
            {
                querySuscripcion = querySuscripcion.Where(item => item.Bloqueada.Equals(false));
                querySuscripcionBlog = querySuscripcionBlog.Where(item => item.Suscripcion.Bloqueada.Equals(false));
            }

            suscDW.ListaSuscripcion = querySuscripcion.ToList();
            suscDW.ListaSuscripcionBlog = querySuscripcionBlog.ToList();

            return suscDW;
        }

        /// <summary>
        /// Obtiene las suscripciones de un perfil (las suscripciones, no sus elementos)
        /// </summary>
        /// <param name="pPerfilID">PerfilID</param>
        /// <returns>SuscripcionDS cargado con las suscripciones (Suscripcion, SuscripcionTesauroProyecto)</returns>
        public DataWrapperSuscripcion ObtenerSuscripcionesDePerfilParaEnviar(Guid pPerfilID)
        {
            DataWrapperSuscripcion suscDW = new DataWrapperSuscripcion();

            //Suscripcion
            suscDW.ListaSuscripcion = mEntityContext.Suscripcion.JoinSuscripcionTesauroProyecto().JoinIdentidad().Where(item => item.Identidad.PerfilID.Equals(pPerfilID) && item.Suscripcion.Bloqueada.Equals(false) && !item.Suscripcion.Periodicidad.Equals(-1) && item.Suscripcion.UltimoEnvio <= DateTime.Now.AddDays(item.Suscripcion.Periodicidad)).Select(item => item.Suscripcion).ToList();

            //SuscripcionTesauroProyectoProyecto
            suscDW.ListaSuscripcionTesauroProyectoProyecto = mEntityContext.Suscripcion.JoinSuscripcionTesauroProyecto().JoinIdentidad().JoinProyecto().Where(item => item.Identidad.PerfilID.Equals(pPerfilID) && item.Suscripcion.Bloqueada.Equals(false) && !item.Suscripcion.Periodicidad.Equals(-1) && item.Suscripcion.UltimoEnvio <= DateTime.Now.AddDays(item.Suscripcion.Periodicidad)).Select(item => new SuscripcionTesauroProyectoProyecto
            {
                Suscripcion = item.SuscripcionTesauroProyecto.Suscripcion,
                NombreProyecto = item.Proyecto.Nombre,
                NombreCortoProyecto = item.Proyecto.NombreCorto,
                OrganizacionID = item.SuscripcionTesauroProyecto.OrganizacionID,
                ProyectoID = item.SuscripcionTesauroProyecto.ProyectoID,
                SuscripcionID = item.SuscripcionTesauroProyecto.SuscripcionID,
                TesauroID = item.SuscripcionTesauroProyecto.TesauroID
            }).ToList();

            return suscDW;
        }

        /// <summary>
        /// Obtiene las suscripciones de un perfil (las suscripciones, no sus elementos)
        /// </summary>
        /// <param name="pPerfilID">PerfilID</param>
        /// <param name="pObtenerCamposDesnormalizados">True si se quieren obtener campos desnormalizados como el nombre de  las categorias suscritas, de los blogs, usuarios etc...</param>
        /// <param name="pObtenerBloqueadas">True si queremos obtener las suscripciones bloquedas ademas de las no bloqueadas</param>
        /// <returns>SuscripcionDS cargado con las suscripciones (Suscripcion,SuscripcionTesauroUsuario, SuscripcionTesauroProyecto,CategoriaTesVinSuscrip)</returns>
        public DataWrapperSuscripcion ObtenerSuscripcionesDePerfil(Guid pPerfilID, bool pObtenerBloqueadas)
        {
            DataWrapperSuscripcion suscDW = new DataWrapperSuscripcion();

            //Suscripcion
            var querySuscripcion = mEntityContext.Suscripcion.JoinIdentidad().Where(item => item.Identidad.PerfilID.Equals(pPerfilID));

            if (!pObtenerBloqueadas)
            {
                querySuscripcion = querySuscripcion.Where(item => item.Suscripcion.Bloqueada.Equals(false));
            }
            suscDW.ListaSuscripcion = querySuscripcion.Select(item => item.Suscripcion).ToList();

            //SuscripcionTesauroUsuario
            var querySuscTesUsuario = mEntityContext.Suscripcion.JoinIdentidad().JoinSuscripcionTesauroUsuario().Where(item => item.Identidad.PerfilID.Equals(pPerfilID) && !item.Identidad.Tipo.Equals((short)TiposIdentidad.Profesor));

            if (!pObtenerBloqueadas)
            {
                querySuscTesUsuario = querySuscTesUsuario.Where(item => item.Suscripcion.Bloqueada.Equals(false));
            }
            suscDW.ListaSuscripcionTesauroUsuario = querySuscTesUsuario.Select(item => item.SuscripcionTesauroUsuario).ToList();

            //SuscripcionTesauroOrganizacion
            var querySuscTesOrganizacion = mEntityContext.SuscripcionTesauroOrganizacion.JoinSuscripcion().JoinIdentidad().Where(item => item.Identidad.PerfilID.Equals(pPerfilID) && item.Identidad.Tipo.Equals((short)TiposIdentidad.Organizacion));

            if (!pObtenerBloqueadas)
            {
                querySuscTesOrganizacion = querySuscTesOrganizacion.Where(item => item.Suscripcion.Bloqueada.Equals(false));
            }
            suscDW.ListaSuscripcionTesauroOrganizacion = querySuscTesOrganizacion.Select(item => item.SuscripcionTesauroOrganizacion).ToList();

            //SuscripcionTesauroProyecto
            var querySuscTesProyecto = mEntityContext.Suscripcion.JoinSuscripcionTesauroProyecto().JoinIdentidad().Where(item => item.Identidad.PerfilID.Equals(pPerfilID));

            if (!pObtenerBloqueadas)
            {
                querySuscTesProyecto = querySuscTesProyecto.Where(item => item.Suscripcion.Bloqueada.Equals(false));
            }
            suscDW.ListaSuscripcionTesauroProyecto = querySuscTesProyecto.Select(item => item.SuscripcionTesauroProyecto).ToList();

            //CategoriaTesVinSuscrip
            var queryCatTesVinSuscrip = mEntityContext.CategoriaTesVinSuscrip.JoinSuscripcion().JoinIdentidad().Where(item => item.Identidad.PerfilID.Equals(pPerfilID));

            if (!pObtenerBloqueadas)
            {
                queryCatTesVinSuscrip = queryCatTesVinSuscrip.Where(item => item.Suscripcion.Bloqueada.Equals(false));
            }
            suscDW.ListaCategoriaTesVinSuscrip = queryCatTesVinSuscrip.Select(item => item.CategoriaTesVinSuscrip).ToList();

            //SuscripcionIdentidadProyecto            
            if (!pObtenerBloqueadas)
            {
                var querySuscripcionIdentidadProyectoPrimeraSubconsulta = mEntityContext.SuscripcionIdentidadProyecto.JoinSuscripcion().JoinIdentidad().JoinIdentSuscProy().Where(item => !item.IdentSuscProy.FechaBaja.HasValue && !item.IdentSuscProy.FechaExpulsion.HasValue && item.Identidad.PerfilID.Equals(pPerfilID) && item.Suscripcion.Bloqueada.Equals(false)).Select(item => item.SuscripcionIdentidadProyecto).Distinct();

                var querySuscripcionIdentidadProyectoSegundaSubconsulta = mEntityContext.SuscripcionIdentidadProyecto.JoinSuscripcion().JoinIdentidad().JoinIdentSuscProy().Where(item => !item.IdentSuscProy.FechaBaja.HasValue && !item.IdentSuscProy.FechaExpulsion.HasValue && item.SuscripcionIdentidadProyecto.ProyectoID.Equals(ProyectoAD.MetaProyecto) && item.Identidad.PerfilID.Equals(pPerfilID) && item.Suscripcion.Bloqueada.Equals(false)).Select(item => item.SuscripcionIdentidadProyecto).Distinct();

                suscDW.ListaSuscripcionIdentidadProyecto = querySuscripcionIdentidadProyectoPrimeraSubconsulta.Union(querySuscripcionIdentidadProyectoSegundaSubconsulta).ToList();
            }
            else
            {
                suscDW.ListaSuscripcionIdentidadProyecto = mEntityContext.SuscripcionIdentidadProyecto.JoinSuscripcion().JoinIdentidad().Where(item => item.Identidad.PerfilID.Equals(pPerfilID)).Select(item => item.SuscripcionIdentidadProyecto).ToList();
            }

            if (!pObtenerBloqueadas)
            {
                //Borramos las filas de suscripción que no pertenecen a ningún tipo de suscripción no bloqueado:
                List<EntityModel.Models.Suscripcion.Suscripcion> listaFilasBorrar = new List<EntityModel.Models.Suscripcion.Suscripcion>();
                foreach (EntityModel.Models.Suscripcion.Suscripcion filaSuscp in suscDW.ListaSuscripcion)
                {
                    if (suscDW.ListaSuscripcionIdentidadProyecto.Where(item => item.SuscripcionID.Equals(filaSuscp.SuscripcionID)).Count() == 0 && suscDW.ListaSuscripcionTesauroProyecto.Where(item => item.SuscripcionID.Equals(filaSuscp.SuscripcionID)).Count() == 0 && suscDW.ListaSuscripcionTesauroUsuario.Where(item => item.SuscripcionID.Equals(filaSuscp.SuscripcionID)).Count() == 0 && suscDW.ListaSuscripcionTesauroOrganizacion.Where(item => item.SuscripcionID.Equals(filaSuscp.SuscripcionID)).Count() == 0)
                    {
                        listaFilasBorrar.Add(filaSuscp);
                    }
                }

                foreach (EntityModel.Models.Suscripcion.Suscripcion filaSuscp in listaFilasBorrar.ToList())
                {
                    suscDW.ListaSuscripcion.Remove(filaSuscp);
                }
            }

            return suscDW;
        }

        public DataWrapperSuscripcion ObtenerSuscripcionDePerfilAPerfil(Guid pPerfilOrigenID, Guid pPerfilDestinoID, bool pObtenerBloqueadas, bool pIncluirBRPersonal)
        {
            DataWrapperSuscripcion suscDW = new DataWrapperSuscripcion();

            if (pIncluirBRPersonal)
            {
                //SuscripcionTesauroUsuario
                var querySuscTesUsuario = mEntityContext.Suscripcion.JoinIdentidad().JoinSuscripcionTesauroUsuario().JoinPersona().JoinPerfil().Where(item => item.Identidad.PerfilID.Equals(pPerfilOrigenID) && item.Perfil.PerfilID.Equals(pPerfilDestinoID) && !item.Identidad.Tipo.Equals((short)TiposIdentidad.Profesor));

                if (!pObtenerBloqueadas)
                {
                    querySuscTesUsuario = querySuscTesUsuario.Where(item => item.Suscripcion.Bloqueada.Equals(false));
                }
                suscDW.ListaSuscripcionTesauroUsuario = querySuscTesUsuario.Select(item => item.SuscripcionTesauroUsuario).ToList();
            }

            //SuscripcionIdentidadProyecto
            var querySuscripcionIdentidadProyecto = mEntityContext.SuscripcionIdentidadProyecto.JoinSuscripcion().JoinIdentidad().JoinIdentSuscProy().Where(item => item.Identidad.PerfilID.Equals(pPerfilOrigenID) && item.IdentSuscProy.PerfilID.Equals(pPerfilDestinoID));

            if (!pObtenerBloqueadas)
            {
                querySuscripcionIdentidadProyecto = querySuscripcionIdentidadProyecto.Where(item => item.Suscripcion.Bloqueada.Equals(false));
            }
            suscDW.ListaSuscripcionIdentidadProyecto = querySuscripcionIdentidadProyecto.Select(item => item.SuscripcionIdentidadProyecto).ToList();

            //Suscripcion            
            List<Guid> listaSuscripcionID = suscDW.ListaSuscripcionTesauroUsuario.Select(item => item.SuscripcionID).Union(suscDW.ListaSuscripcionIdentidadProyecto.Select(item => item.SuscripcionID)).ToList();

            suscDW.ListaSuscripcion = mEntityContext.Suscripcion.Where(item => listaSuscripcionID.Contains(item.SuscripcionID)).ToList();

            return suscDW;
        }

        public DataWrapperSuscripcion ObtenerSuscripcionDePerfilAOrganizacion(Guid pPerfilOrigenID, Guid pOrganizacionID, bool pObtenerBloqueadas)
        {
            DataWrapperSuscripcion suscDW = new DataWrapperSuscripcion();

            //SuscripcionTesauroUsuario
            var querySuscTesOrganizacion = mEntityContext.SuscripcionTesauroOrganizacion.JoinSuscripcion().JoinIdentidad().Where(item => item.Identidad.PerfilID.Equals(pPerfilOrigenID) && item.SuscripcionTesauroOrganizacion.OrganizacionID.Equals(pOrganizacionID) && !item.Identidad.Tipo.Equals((short)TiposIdentidad.Profesor));

            if (!pObtenerBloqueadas)
            {
                querySuscTesOrganizacion = querySuscTesOrganizacion.Where(item => item.Suscripcion.Bloqueada.Equals(false));
            }
            suscDW.ListaSuscripcionTesauroOrganizacion = querySuscTesOrganizacion.Select(item => item.SuscripcionTesauroOrganizacion).ToList();

            List<Guid> listaSuscripcionesID = suscDW.ListaSuscripcionTesauroOrganizacion.Select(item2 => item2.SuscripcionID).ToList();
            //Suscripcion
            suscDW.ListaSuscripcion = mEntityContext.Suscripcion.Where(item => listaSuscripcionesID.Contains(item.SuscripcionID)).ToList();

            return suscDW;
        }

        public List<Guid> ListaPerfilesSuscritosAPerfilEnComunidad(List<Guid> pPerfilesDestinoID, Guid pProyectoID, bool pObtenerSuscritosMetaProyecto = true)
        {
            List<Guid> listaPerfiles = new List<Guid>();

            if (pPerfilesDestinoID.Count > 0)
            {
                var queryEstaSuscrito = mEntityContext.SuscripcionIdentidadProyecto.JoinSuscripcion().JoinIdentidad().JoinIdentSuscProy();

                if (pObtenerSuscritosMetaProyecto)
                {
                    queryEstaSuscrito = queryEstaSuscrito.Where(item => item.SuscripcionIdentidadProyecto.ProyectoID.Equals(pProyectoID) || item.SuscripcionIdentidadProyecto.ProyectoID.Equals(ProyectoAD.MetaProyecto));
                }
                else
                {
                    queryEstaSuscrito = queryEstaSuscrito.Where(item => item.SuscripcionIdentidadProyecto.ProyectoID.Equals(pProyectoID));
                }

                var primeraSubconsultaEstaSuscrito = queryEstaSuscrito.Where(item => pPerfilesDestinoID.Contains(item.IdentSuscProy.PerfilID)).Select(item => item.Identidad.PerfilID).Distinct();
                var segundaSubconsultaEstaSuscrito = mEntityContext.SuscripcionTesauroOrganizacion.JoinSuscripcion().JoinIdentidad().JoinPerfil().Where(item => pPerfilesDestinoID.Contains(item.Perfil.PerfilID)).Select(item => item.Identidad.PerfilID).Distinct();

                listaPerfiles = primeraSubconsultaEstaSuscrito.Concat(segundaSubconsultaEstaSuscrito).ToList();
            }

            return listaPerfiles;
        }

        public List<Guid> ListaPerfilesSuscritosAAlgunaCategoriaDeDocumento(Guid pDocumentoID, Guid pProyectoID)
        {
            return mEntityContext.SuscripcionTesauroProyecto.JoinCategoriaTesVinSuscrip().JoinSuscripcion().JoinIdentidad().JoinDocumentoWebAgCatTesauro().JoinTesauroProyecto().Where(item => item.DocumentoWebAgCatTesauro.DocumentoID.Equals(pDocumentoID) && item.TesauroProyecto.ProyectoID.Equals(pProyectoID)).Select(item => item.Identidad.PerfilID).Distinct().ToList();
        }

        /// <summary>
        /// Obtiene SuscripcionDS con todas las suscripciones "SuscripcionTesauroUsuario","SuscripcionTesauroProyecto",
        /// "CategoriaTesVinSuscrip", "TesVinSuscrip","SuscripcionBlog" de todas las identidades del usuario, 
        /// a través de la persona 'pPersonaID' vinculada a él
        /// </summary>
        /// <param name="pPersonaID">Clave de la persona vinculada al usuario</param>
        /// <param name="pObtenerBloqueadas">True si queremos obtener las suscripciones bloquedas ademas de las no bloqueadas</param>
        /// <returns>SuscripcionDS</returns>
        public DataWrapperSuscripcion ObtenerSuscripcionesDeUsuario(Guid pPersonaID, bool pObtenerBloqueadas)
        {
            DataWrapperSuscripcion suscDW = new DataWrapperSuscripcion();

            //Suscripcion
            var querySuscripcion = mEntityContext.Suscripcion.JoinIdentidad().JoinPerfil().Where(item => item.Perfil.PersonaID.Value.Equals(pPersonaID));

            if (!pObtenerBloqueadas)
            {
                querySuscripcion = querySuscripcion.Where(item => item.Suscripcion.Bloqueada.Equals(false));
            }
            suscDW.ListaSuscripcion = querySuscripcion.Select(item => item.Suscripcion).ToList();

            //SuscripcionTesauroUsuario
            var querySuscTesUsuario = mEntityContext.Suscripcion.JoinIdentidad().JoinSuscripcionTesauroUsuario().JoinPerfil().Where(item => item.Perfil.PersonaID.Value.Equals(pPersonaID));

            if (!pObtenerBloqueadas)
            {
                querySuscTesUsuario = querySuscTesUsuario.Where(item => item.Suscripcion.Bloqueada.Equals(false));
            }
            suscDW.ListaSuscripcionTesauroUsuario = querySuscTesUsuario.Select(item => item.SuscripcionTesauroUsuario).ToList();

            //SuscripcionTesauroOrganizacion
            var querySuscTesOrganizacion = mEntityContext.SuscripcionTesauroOrganizacion.JoinSuscripcion().JoinIdentidad().JoinPerfil().Where(item => item.Perfil.PersonaID.Value.Equals(pPersonaID));

            if (!pObtenerBloqueadas)
            {
                querySuscTesOrganizacion = querySuscTesOrganizacion.Where(item => item.Suscripcion.Bloqueada.Equals(false));
            }
            suscDW.ListaSuscripcionTesauroOrganizacion = querySuscTesOrganizacion.Select(item => item.SuscripcionTesauroOrganizacion).ToList();

            //SuscripcionTesauroProyecto
            var querySuscTesProyecto = mEntityContext.SuscripcionTesauroProyecto.JoinSuscripcion().JoinIdentidad().JoinPerfil().Where(item => item.Perfil.PersonaID.Value.Equals(pPersonaID));

            if (!pObtenerBloqueadas)
            {
                querySuscTesProyecto = querySuscTesProyecto.Where(item => item.Suscripcion.Bloqueada.Equals(false));
            }
            suscDW.ListaSuscripcionTesauroProyecto = querySuscTesProyecto.Select(item => item.SuscripcionTesauroProyecto).ToList();

            //CategoriaTesVinSuscrip
            var queryCatTesVinSuscrip = mEntityContext.CategoriaTesVinSuscrip.JoinSuscripcion().JoinIdentidad().JoinPerfil().Where(item => item.Perfil.PersonaID.Value.Equals(pPersonaID));

            if (!pObtenerBloqueadas)
            {
                queryCatTesVinSuscrip = queryCatTesVinSuscrip.Where(item => item.Suscripcion.Bloqueada.Equals(false));
            }
            suscDW.ListaCategoriaTesVinSuscrip = queryCatTesVinSuscrip.Select(item => item.CategoriaTesVinSuscrip).ToList();

            //SuscripcionIdetnidadProyectog
            var querySuscripcionIdentidadProyecto = mEntityContext.SuscripcionIdentidadProyecto.JoinSuscripcion().JoinIdentidad().JoinPerfil().Where(item => item.Perfil.PersonaID.Value.Equals(pPersonaID));

            if (!pObtenerBloqueadas)
            {
                querySuscripcionIdentidadProyecto = querySuscripcionIdentidadProyecto.Where(item => item.Suscripcion.Bloqueada.Equals(false));
            }
            suscDW.ListaSuscripcionIdentidadProyecto = querySuscripcionIdentidadProyecto.Select(item => item.SuscripcionIdentidadProyecto).ToList();

            return suscDW;
        }

        /// <summary>
        /// Obtiene las vinculaciones de las suscripciones que poseen ciertas categorías.
        /// </summary>
        /// <param name="pListaCategorias">Identificadores de las categorías</param>
        /// <param name="pTesauroID">Identificador del tesauro al que pertenecen las categorías</param>
        /// <returns>DataSet con las vinculaciones cargadas</returns>
        public DataWrapperSuscripcion ObtenerVinculacionesSuscripcionesDeCategoriasTesauro(List<Guid> pListaCategorias, Guid pTesauroID)
        {
            DataWrapperSuscripcion suscripcionDW = new DataWrapperSuscripcion();

            if (pListaCategorias.Count > 0)
            {
                //ListaCategoriaTesVinSuscrip
                suscripcionDW.ListaCategoriaTesVinSuscrip = mEntityContext.CategoriaTesVinSuscrip.Where(item => pListaCategorias.Contains(item.CategoriaTesauroID) && item.TesauroID.Equals(pTesauroID)).ToList();
            }

            //Suscripcion
            suscripcionDW.ListaSuscripcion = mEntityContext.Suscripcion.JoinSuscripcionTesauroProyecto().Where(item => item.SuscripcionTesauroProyecto.TesauroID.Equals(pTesauroID)).Select(item => item.Suscripcion).ToList();

            //SuscripcionTesauroProyecto
            suscripcionDW.ListaSuscripcionTesauroProyecto = mEntityContext.SuscripcionTesauroProyecto.Where(item => item.TesauroID.Equals(pTesauroID)).ToList();

            return suscripcionDW;
        }

        /// <summary>
        /// Devuelve si se han enviado resultados de la suscripción pasada como parámetro después de la fecha dada
        /// </summary>
        /// <param name="pSuscripcionID">Identificador de la suscripción</param>
        /// <param name="pFecha">Fecha a partir de la cual se comprueba si tiene boletin</param>
        /// <returns>TRUE si existe boletin posterior a la fecha, FALSE en caso contrario</returns>
        public bool TienePerfilBoletinPosteriorAFecha(Guid pSuscripcionID, DateTime pFecha)
        {
            return mEntityContext.Suscripcion.Where(item => item.UltimoEnvio >= pFecha && item.SuscripcionID.Equals(pSuscripcionID)).Any();
        }

        /// <summary>
        /// Devuelve el ultimo score que se ha enviado de una suscripción 
        /// </summary>
        /// <param name="pSuscripcionID">Identificador de la suscripción</param>
        /// <returns></returns>
        public int UltimoScoreSuscripcionEnviado(Guid pSuscripcionID)
        {
            int? score = mEntityContext.Suscripcion.Where(item => item.SuscripcionID.Equals(pSuscripcionID)).Select(item => item.ScoreUltimoEnvio).FirstOrDefault();
            if (score.HasValue)
            {
                return score.Value;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// Suscribe al tesauro personal del usuario a cada persona que esté suscrita a todo el perfil del usuario.
        /// </summary>
        /// <param name="pUsuarioID">ID del usuario</param>
        public void CrearSuscripcionesAUsuarioSiPersonaSuscriptaATodo(Guid pUsuarioID)
        {
            List<Guid> listaSuscripcionIDSubconsulta = mEntityContext.SuscripcionTesauroUsuario.Where(item => item.UsuarioID.Equals(pUsuarioID)).Select(item => item.SuscripcionID).ToList();

            List<Guid> listaSuscripcionIDQuery = mEntityContext.SuscripcionIdentidadProyecto.JoinIdentidad().JoinPerfil().JoinPersona().Where(item => item.Persona.UsuarioID.Value.Equals(pUsuarioID) && item.SuscripcionIdentidadProyecto.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !listaSuscripcionIDSubconsulta.Contains(item.SuscripcionIdentidadProyecto.SuscripcionID)).Select(item => item.SuscripcionIdentidadProyecto.SuscripcionID).ToList();

            if (listaSuscripcionIDQuery.Count > 0)
            {
                Guid tesauroID = mEntityContext.TesauroUsuario.Where(item => item.UsuarioID.Equals(pUsuarioID)).Select(item => item.TesauroID).FirstOrDefault();

                foreach (Guid id in listaSuscripcionIDQuery)
                {
                    SuscripcionTesauroUsuario suscripcionTesauroUsuario = new SuscripcionTesauroUsuario();
                    suscripcionTesauroUsuario.SuscripcionID = id;
                    suscripcionTesauroUsuario.UsuarioID = pUsuarioID;
                    suscripcionTesauroUsuario.TesauroID = tesauroID;

                    mEntityContext.SuscripcionTesauroUsuario.Add(suscripcionTesauroUsuario);
                }

                ActualizarBaseDeDatosEntityContext();
            }
        }

        /// <summary>
        /// Elimina las suscripciones al tesauro personal del usuario a cada persona que NO esté suscrita a todo el perfil del usuario.
        /// </summary>
        /// <param name="pUsuarioID">ID del usuario</param>
        public void EliminarSuscripcionesAUsuarioSiPersonaNoSuscriptaATodo(Guid pUsuarioID)
        {
            List<SuscripcionTesauroUsuario> listaSuscripcionTesauroUsuario = mEntityContext.SuscripcionTesauroUsuario.Where(item => item.UsuarioID.Equals(pUsuarioID)).ToList();

            List<Guid> listaSuscripcionTesauroUsuarioSuscripcionID = listaSuscripcionTesauroUsuario.Select(item => item.SuscripcionID).ToList();

            foreach (SuscripcionTesauroUsuario suscripcionTesauroUsuario in listaSuscripcionTesauroUsuario)
            {
                mEntityContext.EliminarElemento(suscripcionTesauroUsuario);
            }

            List<Guid> listaSuscripcionIdentidadProyectoSuscripcionID = mEntityContext.SuscripcionIdentidadProyecto.Where(item => listaSuscripcionTesauroUsuarioSuscripcionID.Contains(item.SuscripcionID)).Select(item => item.SuscripcionID).ToList();

            List<EntityModel.Models.Suscripcion.Suscripcion> listaSuscripcion = mEntityContext.Suscripcion.Where(item => listaSuscripcionTesauroUsuarioSuscripcionID.Contains(item.SuscripcionID) && !listaSuscripcionIdentidadProyectoSuscripcionID.Contains(item.SuscripcionID)).ToList();

            foreach (EntityModel.Models.Suscripcion.Suscripcion suscripcion in listaSuscripcion)
            {
                mEntityContext.EliminarElemento(suscripcion);
            }

            ActualizarBaseDeDatosEntityContext();
        }

        public DataWrapperDatoExtra ObtenerCategoriasTesauroDeProyectoSuscritaIdentidad(Guid pProyectoID, Guid pIdentidadID)
        {
            DataWrapperDatoExtra dataWrapperDatoExtra = new DataWrapperDatoExtra();

            dataWrapperDatoExtra.ListaTriples = mEntityContext.Suscripcion.JoinSuscripcionTesauroProyecto().JoinCategoriaTesVinSuscrip().Where(item => item.SuscripcionTesauroProyecto.ProyectoID.Equals(pProyectoID) && item.Suscripcion.IdentidadID.Equals(pIdentidadID)).Select(item => new Triples
            {
                IdentidadID = item.Suscripcion.IdentidadID,
                PredicadorRDF = "http://xmlns.com/foaf/0.1/interest",
                Opcion = item.CategoriaTesVinSuscrip.CategoriaTesauroID.ToString()
            }).ToList();

            return dataWrapperDatoExtra;
        }

        #endregion

        #region Privados

        /// <summary>
        /// En caso de que se utilice el GnossConfig.xml por defecto se sigue utilizando el IBD estático
        /// </summary>
        private void CargarConsultasYDataAdapters()
        {
            this.CargarConsultasYDataAdapters(IBD);
        }

        /// <summary>
        /// En caso de que se utilice un GnossConfig.xml que no es el de por defecto se pasa un objeto IBaseDatos creado con respecto
        /// al fichero de configuracion que se ha apsado como parámetro
        /// </summary>
        /// <param name="IBD">Objecto IBaseDatos para el archivo pasado al constructor del AD</param>
        private void CargarConsultasYDataAdapters(IBaseDatos IBD)
        {
            #region Consultas

            this.sqlSelectSuscripciones = "SELECT " + IBD.CargarGuid("Suscripcion.SuscripcionID") + ", " + IBD.CargarGuid("Suscripcion.IdentidadID") + ", Suscripcion.Periodicidad, Suscripcion.Bloqueada, Suscripcion.UltimoEnvio, Suscripcion.FechaSuscripcion, Suscripcion.ScoreUltimoEnvio FROM Suscripcion";

            this.sqlSelectSuscripcionTesauroProyecto = "SELECT " + IBD.CargarGuid("SuscripcionTesauroProyecto.SuscripcionID") + "," + IBD.CargarGuid("SuscripcionTesauroProyecto.organizacionID") + "," + IBD.CargarGuid("SuscripcionTesauroProyecto.proyectoID") + "," + IBD.CargarGuid("SuscripcionTesauroProyecto.tesauroID") + " FROM SuscripcionTesauroProyecto";

            this.sqlSelectSuscripcionTesauroProyectoSimple = "SELECT " + IBD.CargarGuid("SuscripcionTesauroProyecto.SuscripcionID") + "," + IBD.CargarGuid("SuscripcionTesauroProyecto.organizacionID") + "," + IBD.CargarGuid("SuscripcionTesauroProyecto.proyectoID") + "," + IBD.CargarGuid("SuscripcionTesauroProyecto.tesauroID");

            this.sqlSelectSuscripcionTesauroUsuario = "SELECT " + IBD.CargarGuid("SuscripcionTesauroUsuario.SuscripcionID") + "," + IBD.CargarGuid("SuscripcionTesauroUsuario.usuarioID") + "," + IBD.CargarGuid("SuscripcionTesauroUsuario.tesauroID") + " FROM SuscripcionTesauroUsuario";

            this.sqlSelectSuscripcionTesauroUsuarioSimple = "SELECT " + IBD.CargarGuid("SuscripcionTesauroUsuario.SuscripcionID") + "," + IBD.CargarGuid("SuscripcionTesauroUsuario.usuarioID") + "," + IBD.CargarGuid("SuscripcionTesauroUsuario.tesauroID");

            this.sqlSelectSuscripcionTesauroOrganizacion = "SELECT " + IBD.CargarGuid("SuscripcionTesauroOrganizacion.SuscripcionID") + "," + IBD.CargarGuid("SuscripcionTesauroOrganizacion.OrganizacionID") + "," + IBD.CargarGuid("SuscripcionTesauroOrganizacion.tesauroID") + " FROM SuscripcionTesauroOrganizacion";

            this.sqlSelectSuscripcionTesauroOrganizacionSimple = "SELECT " + IBD.CargarGuid("SuscripcionTesauroOrganizacion.SuscripcionID") + "," + IBD.CargarGuid("SuscripcionTesauroOrganizacion.OrganizacionID") + "," + IBD.CargarGuid("SuscripcionTesauroOrganizacion.tesauroID");


            this.sqlSelectCategoriasTesVinSuscrip = "SELECT DISTINCT " + IBD.CargarGuid("CategoriaTesVinSuscrip.SuscripcionID") + ", " + IBD.CargarGuid("CategoriaTesVinSuscrip.TesauroID") + ", " + IBD.CargarGuid("CategoriaTesVinSuscrip.CategoriaTesauroID") + " FROM CategoriaTesVinSuscrip";

            this.sqlSelectCategoriasTesVinSuscripSimple = "SELECT DISTINCT " + IBD.CargarGuid("CategoriaTesVinSuscrip.SuscripcionID") + ", " + IBD.CargarGuid("CategoriaTesVinSuscrip.TesauroID") + ", " + IBD.CargarGuid("CategoriaTesVinSuscrip.CategoriaTesauroID");

            this.sqlSelectSuscripcionBlog = "SELECT " + IBD.CargarGuid("SuscripcionBlog.SuscripcionID") + ", " + IBD.CargarGuid("SuscripcionBlog.BlogID") + " FROM SuscripcionBlog";

            this.sqlSelectSuscripcionBlogSimple = "SELECT " + IBD.CargarGuid("SuscripcionBlog.SuscripcionID") + ", " + IBD.CargarGuid("SuscripcionBlog.BlogID");

            this.sqlSelectSuscripcionIdentidadProyecto = "SELECT " + IBD.CargarGuid("SuscripcionIdentidadProyecto.SuscripcionID") + ", " + IBD.CargarGuid("SuscripcionIdentidadProyecto.IdentidadID") + ", " + IBD.CargarGuid("SuscripcionIdentidadProyecto.ProyectoID") + ", " + IBD.CargarGuid("SuscripcionIdentidadProyecto.OrganizacionID") + " FROM SuscripcionIdentidadProyecto ";

            this.sqlSelectSuscripcionesAUsuario = sqlSelectSuscripciones + " INNER JOIN SuscripcionTesauroUsuario ON SuscripcionTesauroUsuario.SuscripcionID = Suscripcion.SuscripcionID WHERE SuscripcionTesauroUsuario.UsuarioID = " + IBD.GuidParamValor("UsuarioID");

            this.sqlSelectSuscripcionIdentidadProyectoSimple = "SELECT " + IBD.CargarGuid("SuscripcionIdentidadProyecto.SuscripcionID") + ", " + IBD.CargarGuid("SuscripcionIdentidadProyecto.IdentidadID") + ", " + IBD.CargarGuid("SuscripcionIdentidadProyecto.ProyectoID") + ", " + IBD.CargarGuid("SuscripcionIdentidadProyecto.OrganizacionID") + " ";

            #endregion

            #region DataAdapter

            #region Suscripcion

            this.sqlSuscripcionInsert = IBD.ReplaceParam("INSERT INTO Suscripcion (SuscripcionID, Periodicidad, IdentidadID, Bloqueada, UltimoEnvio,FechaSuscripcion, ScoreUltimoEnvio) VALUES (" + IBD.GuidParamColumnaTabla("SuscripcionID") + ", @Periodicidad, " + IBD.GuidParamColumnaTabla("IdentidadID") + ", @Bloqueada, @UltimoEnvio, @FechaSuscripcion, @ScoreUltimoEnvio)");

            this.sqlSuscripcionDelete = IBD.ReplaceParam("DELETE FROM Suscripcion WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ") AND (Periodicidad = @O_Periodicidad) AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ") AND (Bloqueada = @O_Bloqueada) AND (UltimoEnvio = @O_UltimoEnvio)");

            this.sqlSuscripcionModify = IBD.ReplaceParam("UPDATE Suscripcion SET SuscripcionID = " + IBD.GuidParamColumnaTabla("SuscripcionID") + ", Periodicidad = @Periodicidad, IdentidadID = " + IBD.GuidParamColumnaTabla("IdentidadID") + " , Bloqueada = @Bloqueada, UltimoEnvio = @UltimoEnvio, FechaSuscripcion = @FechaSuscripcion, ScoreUltimoEnvio = @ScoreUltimoEnvio WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ") AND (Periodicidad = @O_Periodicidad) AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ") AND (Bloqueada = @O_Bloqueada) AND (UltimoEnvio = @O_UltimoEnvio)");

            #endregion

            #region SuscripcionBlog

            this.sqlSuscripcionBlogInsert = IBD.ReplaceParam("INSERT INTO SuscripcionBlog (SuscripcionID, BlogID) VALUES (" + IBD.GuidParamColumnaTabla("SuscripcionID") + ", " + IBD.GuidParamColumnaTabla("BlogID") + ")");

            this.sqlSuscripcionBlogDelete = IBD.ReplaceParam("DELETE FROM SuscripcionBlog WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ") AND (BlogID = " + IBD.GuidParamColumnaTabla("O_BlogID") + ")");

            this.sqlSuscripcionBlogModify = IBD.ReplaceParam("UPDATE SuscripcionBlog SET SuscripcionID = " + IBD.GuidParamColumnaTabla("SuscripcionID") + ", BlogID = " + IBD.GuidParamColumnaTabla("BlogID") + " WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ") AND (BlogID = " + IBD.GuidParamColumnaTabla("O_BlogID") + ")");

            #endregion

            #region SuscripcionTesauroProyecto

            this.sqlSuscripcionTesauroProyectoInsert = IBD.ReplaceParam("INSERT INTO SuscripcionTesauroProyecto (SuscripcionID, OrganizacionID, ProyectoID, TesauroID) VALUES (" + IBD.GuidParamColumnaTabla("SuscripcionID") + ", " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("TesauroID") + ")");

            this.sqlSuscripcionTesauroProyectoDelete = IBD.ReplaceParam("DELETE FROM SuscripcionTesauroProyecto WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (TesauroID = " + IBD.GuidParamColumnaTabla("O_TesauroID") + ")");
            this.sqlSuscripcionTesauroProyectoModify = IBD.ReplaceParam("UPDATE SuscripcionTesauroProyecto SET SuscripcionID = " + IBD.GuidParamColumnaTabla("SuscripcionID") + ", OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", TesauroID = " + IBD.GuidParamColumnaTabla("TesauroID") + " WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (TesauroID = " + IBD.GuidParamColumnaTabla("O_TesauroID") + ")");

            #endregion

            #region SuscripcionTesauroUsuario

            this.sqlSuscripcionTesauroUsuarioInsert = IBD.ReplaceParam("INSERT INTO SuscripcionTesauroUsuario (SuscripcionID, UsuarioID, TesauroID) VALUES (" + IBD.GuidParamColumnaTabla("SuscripcionID") + ", " + IBD.GuidParamColumnaTabla("UsuarioID") + ", " + IBD.GuidParamColumnaTabla("TesauroID") + ")");

            this.sqlSuscripcionTesauroUsuarioDelete = IBD.ReplaceParam("DELETE FROM SuscripcionTesauroUsuario WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ") AND (UsuarioID = " + IBD.GuidParamColumnaTabla("O_UsuarioID") + ") AND (TesauroID = " + IBD.GuidParamColumnaTabla("O_TesauroID") + ")");

            this.sqlSuscripcionTesauroUsuarioModify = IBD.ReplaceParam("UPDATE SuscripcionTesauroUsuario SET SuscripcionID = " + IBD.GuidParamColumnaTabla("SuscripcionID") + ", UsuarioID = " + IBD.GuidParamColumnaTabla("UsuarioID") + ", TesauroID = " + IBD.GuidParamColumnaTabla("TesauroID") + " WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ") AND (UsuarioID = " + IBD.GuidParamColumnaTabla("O_UsuarioID") + ") AND (TesauroID = " + IBD.GuidParamColumnaTabla("O_TesauroID") + ")");

            #endregion

            #region SuscripcionTesauroOrganizacion

            this.sqlSuscripcionTesauroOrganizacionInsert = IBD.ReplaceParam("INSERT INTO SuscripcionTesauroOrganizacion (SuscripcionID, OrganizacionID, TesauroID) VALUES (" + IBD.GuidParamColumnaTabla("SuscripcionID") + ", " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("TesauroID") + ")");

            this.sqlSuscripcionTesauroOrganizacionDelete = IBD.ReplaceParam("DELETE FROM SuscripcionTesauroOrganizacion WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (TesauroID = " + IBD.GuidParamColumnaTabla("O_TesauroID") + ")");

            this.sqlSuscripcionTesauroOrganizacionModify = IBD.ReplaceParam("UPDATE SuscripcionTesauroOrganizacion SET SuscripcionID = " + IBD.GuidParamColumnaTabla("SuscripcionID") + ", OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", TesauroID = " + IBD.GuidParamColumnaTabla("TesauroID") + " WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (TesauroID = " + IBD.GuidParamColumnaTabla("O_TesauroID") + ")");

            #endregion

            #region CategoriaTesVinSuscrip

            this.sqlCategoriaTesVinSuscripInsert = IBD.ReplaceParam("INSERT INTO CategoriaTesVinSuscrip (SuscripcionID, TesauroID, CategoriaTesauroID) VALUES (" + IBD.GuidParamColumnaTabla("SuscripcionID") + ", " + IBD.GuidParamColumnaTabla("TesauroID") + ", " + IBD.GuidParamColumnaTabla("CategoriaTesauroID") + ")");

            this.sqlCategoriaTesVinSuscripDelete = IBD.ReplaceParam("DELETE FROM CategoriaTesVinSuscrip WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ") AND (TesauroID = " + IBD.GuidParamColumnaTabla("O_TesauroID") + ") AND (CategoriaTesauroID = " + IBD.GuidParamColumnaTabla("O_CategoriaTesauroID") + ")");

            this.sqlCategoriaTesVinSuscripModify = IBD.ReplaceParam("UPDATE CategoriaTesVinSuscrip SET SuscripcionID = " + IBD.GuidParamColumnaTabla("SuscripcionID") + ", TesauroID = " + IBD.GuidParamColumnaTabla("TesauroID") + ", CategoriaTesauroID = " + IBD.GuidParamColumnaTabla("CategoriaTesauroID") + " WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ") AND (TesauroID = " + IBD.GuidParamColumnaTabla("O_TesauroID") + ") AND (CategoriaTesauroID = " + IBD.GuidParamColumnaTabla("O_CategoriaTesauroID") + ")");

            #endregion

            #region SuscripcionIdentidadProyecto

            this.sqlSuscripcionIdentidadProyectoInsert = IBD.ReplaceParam("INSERT INTO SuscripcionIdentidadProyecto (SuscripcionID, IdentidadID, ProyectoID, OrganizacionID) VALUES (" + IBD.GuidParamColumnaTabla("SuscripcionID") + ", " + IBD.GuidParamColumnaTabla("IdentidadID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("OrganizacionID") + ")");

            this.sqlSuscripcionIdentidadProyectoDelete = IBD.ReplaceParam("DELETE FROM SuscripcionIdentidadProyecto WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ") AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ")");

            this.sqlSuscripcionIdentidadProyectoModify = IBD.ReplaceParam("UPDATE SuscripcionIdentidadProyecto SET SuscripcionID = " + IBD.GuidParamColumnaTabla("SuscripcionID") + ", IdentidadID = " + IBD.GuidParamColumnaTabla("IdentidadID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + " , OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + " WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ") AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ")");

            #endregion

            #endregion
        }

        #endregion

        #endregion
    }
}
