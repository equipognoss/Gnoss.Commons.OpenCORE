using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Amigos;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Blog;
using Es.Riam.Gnoss.AD.EntityModel.Models.Comentario;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.AD.Comentario
{
    #region Enumeraciones

    /// <summary>
    /// Tipo de comentario
    /// </summary>
    public enum TipoComentario
    {
        /// <summary>
        /// Comentario a un recurso
        /// </summary>
        Recurso,
        /// <summary>
        /// Comentario a una entrada de blog
        /// </summary>
        EntradaBlog
    }

    #endregion

    public class JoinComentarioIdentidad
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinComentarioIdentidadPerfil
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinComentarioBlogBlog
    {
        public ComentarioBlog ComentarioBlog { get; set; }
        public Blog Blog { get; set; }
    }

    public class JoinDocumentoComentarioDocumento
    {
        public DocumentoComentario DocumentoComentario { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinComentarioIdentidadPerfilComentarioBlog
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public ComentarioBlog ComentarioBlog { get; set; }
    }

    public class JoinVotoComentarioComentarioBlog
    {
        public VotoComentario VotoComentario { get; set; }
        public ComentarioBlog ComentarioBlog { get; set; }
    }

    public class JoinVotoComentarioComentarioBlogComentario
    {
        public VotoComentario VotoComentario { get; set; }
        public ComentarioBlog ComentarioBlog { get; set; }
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
    }

    public class JoinComentarioComentarioBlog
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public ComentarioBlog ComentarioBlog { get; set; }
    }

    public class JoinComentarioComentarioBlogBlog
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public ComentarioBlog ComentarioBlog { get; set; }
        public Blog Blog { get; set; }
    }

    public class JoinVotoComentarioComentarioBlogBlog
    {
        public VotoComentario VotoComentario { get; set; }
        public ComentarioBlog ComentarioBlog { get; set; }
        public Blog Blog { get; set; }
    }

    public class JoinVotoComentarioComentarioBlogBlogComentario
    {
        public VotoComentario VotoComentario { get; set; }
        public ComentarioBlog ComentarioBlog { get; set; }
        public Blog Blog { get; set; }
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
    }

    public class JoinComentarioIdentidadPerfilComentarioBlogEntradaBlog
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public ComentarioBlog ComentarioBlog { get; set; }
        public EntradaBlog EntradaBlog { get; set; }
    }

    public class JoinComentarioIdentidadPerfilComentarioBlogEntradaBlogBlog
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public ComentarioBlog ComentarioBlog { get; set; }
        public EntradaBlog EntradaBlog { get; set; }
        public Blog Blog { get; set; }
    }

    public class JoinComentarioIdentidadPerfilComentarioBlogEntradaBlogBlogIdentidad
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public ComentarioBlog ComentarioBlog { get; set; }
        public EntradaBlog EntradaBlog { get; set; }
        public Blog Blog { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad2 { get; set; }
    }

    public class JoinComentarioIdentidadPerfilComentarioBlogEntradaBlogBlogIdentidadPerfil
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public ComentarioBlog ComentarioBlog { get; set; }
        public EntradaBlog EntradaBlog { get; set; }
        public Blog Blog { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad2 { get; set; }
        public Perfil Perfil2 { get; set; }
    }

    public class JoinComentarioIdentidadPerfilComentarioBlogEntradaBlogBlogIdentidadPerfilPerfilPersonaOrg
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public ComentarioBlog ComentarioBlog { get; set; }
        public EntradaBlog EntradaBlog { get; set; }
        public Blog Blog { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad2 { get; set; }
        public Perfil Perfil2 { get; set; }
        public PerfilPersonaOrg PerfilPersonaOrg { get; set; }
    }

    public class JoinComentarioIdentidadPerfilComentarioBlogEntradaBlogBlogIdentidadPerfilPerfilPersona
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public ComentarioBlog ComentarioBlog { get; set; }
        public EntradaBlog EntradaBlog { get; set; }
        public Blog Blog { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad2 { get; set; }
        public Perfil Perfil2 { get; set; }
        public PerfilPersona PerfilPersona { get; set; }
    }

    public class JoinComentarioIdentidadPerfilDocumentoComentario
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
    }

    public class JoinComentarioIdentidadPerfilDocumentoComentarioDocumento
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoDocumentoWebVinBaseRecursos
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }

    public class JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoDocumentoWebVinBaseRecursosIdentidad
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad2 { get; set; }
    }

    public class JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoDocumentoWebVinBaseRecursosIdentidadPerfil
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad2 { get; set; }
        public Perfil Perfil2 { get; set; }
    }

    public class JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoDocumentoWebVinBaseRecursosIdentidadPerfilPerfilPersonaOrg
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad2 { get; set; }
        public Perfil Perfil2 { get; set; }
        public PerfilPersonaOrg PerfilPersonaOrg { get; set; }
    }

    public class JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoDocumentoWebVinBaseRecursosIdentidadPerfilPerfilPersona
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad2 { get; set; }
        public Perfil Perfil2 { get; set; }
        public PerfilPersona PerfilPersona { get; set; }
    }

    public class JoinComentarioDocumentoComentario
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
    }

    public class JoinComentarioIdentidadPerfilDocumentoComentarioProyecto
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
        public EntityModel.Models.ProyectoDS.Proyecto Proyecto { get; set; }
    }

    public class JoinVotoComentarioDocumentoComentario
    {
        public DocumentoComentario DocumentoComentario { get; set; }
        public VotoComentario VotoComentario { get; set; }
    }

    public class JoinVotoComentarioDocumentoComentarioComentario
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
        public VotoComentario VotoComentario { get; set; }
    }

    public class JoinVotoComentarioDocumentoComentarioComentarioProyecto
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
        public VotoComentario VotoComentario { get; set; }
        public EntityModel.Models.ProyectoDS.Proyecto Proyecto { get; set; }
    }

    public class JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoIdentidad
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
        public Documento Documento { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad2 { get; set; }
    }

    public class JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoIdentidadPerfil
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
        public Documento Documento { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad2 { get; set; }
        public Perfil Perfil2 { get; set; }
    }

    //Perfil ON Perfil.PerfilID = Identidad.PerfilID

    public static class Joins
    {
        public static IQueryable<JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoIdentidadPerfil> JoinPerfil(this IQueryable<JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Identidad2.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoIdentidadPerfil
            {
                Identidad = item.Identidad,
                Documento = item.Documento,
                Comentario = item.Comentario,
                DocumentoComentario = item.DocumentoComentario,
                Identidad2 = item.Identidad2,
                Perfil = item.Perfil,
                Perfil2 = perfil
            });
        }

        public static IQueryable<JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoIdentidad> JoinIdentidad(this IQueryable<JoinComentarioIdentidadPerfilDocumentoComentarioDocumento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Documento.CreadorID, identidad => identidad.IdentidadID, (item, identidad) => new JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoIdentidad
            {
                Identidad = item.Identidad,
                Documento = item.Documento,
                Comentario = item.Comentario,
                DocumentoComentario = item.DocumentoComentario,
                Identidad2 = identidad,
                Perfil = item.Perfil
            });
        }

        public static IQueryable<JoinVotoComentarioDocumentoComentarioComentarioProyecto> JoinProyecto(this IQueryable<JoinVotoComentarioDocumentoComentarioComentario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Proyecto, item => item.DocumentoComentario.ProyectoID, proyecto => proyecto.ProyectoID, (item, proyecto) => new JoinVotoComentarioDocumentoComentarioComentarioProyecto
            {
                Comentario = item.Comentario,
                DocumentoComentario = item.DocumentoComentario,
                VotoComentario = item.VotoComentario,
                Proyecto = proyecto
            });
        }

        public static IQueryable<JoinVotoComentarioDocumentoComentarioComentario> JoinComentario(this IQueryable<JoinVotoComentarioDocumentoComentario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Comentario, item => item.VotoComentario.ComentarioID, comentario => comentario.ComentarioID, (item, comentario) => new JoinVotoComentarioDocumentoComentarioComentario
            {
                Comentario = comentario,
                DocumentoComentario = item.DocumentoComentario,
                VotoComentario = item.VotoComentario
            });
        }

        public static IQueryable<JoinVotoComentarioDocumentoComentario> JoinDocumentoComentario(this IQueryable<VotoComentario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoComentario, votoComentario => votoComentario.ComentarioID, documentoComentario => documentoComentario.ComentarioID, (votoComentario, documentoComentario) => new JoinVotoComentarioDocumentoComentario
            {
                DocumentoComentario = documentoComentario,
                VotoComentario = votoComentario
            });
        }

        public static IQueryable<JoinComentarioIdentidadPerfilDocumentoComentarioProyecto> JoinProyecto(this IQueryable<JoinComentarioIdentidadPerfilDocumentoComentario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Proyecto, item => item.DocumentoComentario.ProyectoID, proyecto => proyecto.ProyectoID, (item, proyecto) => new JoinComentarioIdentidadPerfilDocumentoComentarioProyecto
            {
                Proyecto = proyecto,
                Comentario = item.Comentario,
                DocumentoComentario = item.DocumentoComentario,
                Identidad = item.Identidad,
                Perfil = item.Perfil
            });
        }

        public static IQueryable<JoinComentarioDocumentoComentario> JoinDocumentoComentario(this IQueryable<EntityModel.Models.Comentario.Comentario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoComentario, comentario => comentario.ComentarioID, documentoComentario => documentoComentario.ComentarioID, (comentario, documentoComentario) => new JoinComentarioDocumentoComentario
            {
                DocumentoComentario = documentoComentario,
                Comentario = comentario
            });
        }

        public static IQueryable<JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoDocumentoWebVinBaseRecursosIdentidadPerfilPerfilPersona> JoinPerfilPersona(this IQueryable<JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoDocumentoWebVinBaseRecursosIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.PerfilPersona, item => item.Identidad2.PerfilID, perfilPersona => perfilPersona.PerfilID, (item, perfilPersona) => new JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoDocumentoWebVinBaseRecursosIdentidadPerfilPerfilPersona
            {
                DocumentoComentario = item.DocumentoComentario,
                Comentario = item.Comentario,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                Identidad2 = item.Identidad2,
                Perfil2 = item.Perfil2,
                PerfilPersona = perfilPersona
            });
        }

        public static IQueryable<JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoDocumentoWebVinBaseRecursosIdentidadPerfilPerfilPersonaOrg> JoinPerfilPersonaOrg(this IQueryable<JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoDocumentoWebVinBaseRecursosIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.PerfilPersonaOrg, item => item.Identidad2.PerfilID, perfilPersonaOrg => perfilPersonaOrg.PerfilID, (item, perfilPersonaOrg) => new JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoDocumentoWebVinBaseRecursosIdentidadPerfilPerfilPersonaOrg
            {
                DocumentoComentario = item.DocumentoComentario,
                Comentario = item.Comentario,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                Identidad2 = item.Identidad2,
                Perfil2 = item.Perfil2,
                PerfilPersonaOrg = perfilPersonaOrg
            });
        }

        public static IQueryable<JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoDocumentoWebVinBaseRecursosIdentidadPerfil> JoinPerfil(this IQueryable<JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoDocumentoWebVinBaseRecursosIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Identidad2.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoDocumentoWebVinBaseRecursosIdentidadPerfil
            {
                DocumentoComentario = item.DocumentoComentario,
                Comentario = item.Comentario,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                Identidad2 = item.Identidad2,
                Perfil2 = perfil
            });
        }

        public static IQueryable<JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoDocumentoWebVinBaseRecursosIdentidad> JoinIdentidad(this IQueryable<JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoDocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.DocumentoWebVinBaseRecursos.IdentidadPublicacionID.Value, identidad => identidad.IdentidadID, (item, identidad) => new JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoDocumentoWebVinBaseRecursosIdentidad
            {
                DocumentoComentario = item.DocumentoComentario,
                Comentario = item.Comentario,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                Identidad2 = identidad
            });
        }

        public static IQueryable<JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoDocumentoWebVinBaseRecursos> JoinDocumentoWebVinBaseRecursos(this IQueryable<JoinComentarioIdentidadPerfilDocumentoComentarioDocumento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoWebVinBaseRecursos, item => item.Documento.DocumentoID, documentoWebVinBaseRecursos => documentoWebVinBaseRecursos.DocumentoID, (item, documentoWebVinBaseRecursos) => new JoinComentarioIdentidadPerfilDocumentoComentarioDocumentoDocumentoWebVinBaseRecursos
            {
                DocumentoComentario = item.DocumentoComentario,
                Comentario = item.Comentario,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = documentoWebVinBaseRecursos
            });
        }

        public static IQueryable<JoinComentarioIdentidadPerfilDocumentoComentarioDocumento> JoinDocumento(this IQueryable<JoinComentarioIdentidadPerfilDocumentoComentario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, item => item.DocumentoComentario.DocumentoID, documento => documento.DocumentoID, (item, documento) => new JoinComentarioIdentidadPerfilDocumentoComentarioDocumento
            {
                DocumentoComentario = item.DocumentoComentario,
                Comentario = item.Comentario,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Documento = documento
            });
        }

        public static IQueryable<JoinComentarioIdentidadPerfilDocumentoComentario> JoinDocumentoComentario(this IQueryable<JoinComentarioIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoComentario, item => item.Comentario.ComentarioID, documentoComentario => documentoComentario.ComentarioID, (item, documentoComentario) => new JoinComentarioIdentidadPerfilDocumentoComentario
            {
                DocumentoComentario = documentoComentario,
                Comentario = item.Comentario,
                Identidad = item.Identidad,
                Perfil = item.Perfil
            });
        }

        public static IQueryable<JoinComentarioIdentidadPerfilComentarioBlogEntradaBlogBlogIdentidadPerfilPerfilPersona> JoinPerfilPersona(this IQueryable<JoinComentarioIdentidadPerfilComentarioBlogEntradaBlogBlogIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.PerfilPersona, item => item.Identidad2.PerfilID, perfilPersona => perfilPersona.PerfilID, (item, perfilPersona) => new JoinComentarioIdentidadPerfilComentarioBlogEntradaBlogBlogIdentidadPerfilPerfilPersona
            {
                EntradaBlog = item.EntradaBlog,
                Comentario = item.Comentario,
                ComentarioBlog = item.ComentarioBlog,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Blog = item.Blog,
                Identidad2 = item.Identidad2,
                Perfil2 = item.Perfil2,
                PerfilPersona = perfilPersona
            });
        }

        public static IQueryable<JoinComentarioIdentidadPerfilComentarioBlogEntradaBlogBlogIdentidadPerfilPerfilPersonaOrg> JoinPerfilPersonaOrg(this IQueryable<JoinComentarioIdentidadPerfilComentarioBlogEntradaBlogBlogIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.PerfilPersonaOrg, item => item.Identidad2.PerfilID, perfilPersonaOrg => perfilPersonaOrg.PerfilID, (item, perfilPersonaOrg) => new JoinComentarioIdentidadPerfilComentarioBlogEntradaBlogBlogIdentidadPerfilPerfilPersonaOrg
            {
                EntradaBlog = item.EntradaBlog,
                Comentario = item.Comentario,
                ComentarioBlog = item.ComentarioBlog,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Blog = item.Blog,
                Identidad2 = item.Identidad2,
                Perfil2 = item.Perfil2,
                PerfilPersonaOrg = perfilPersonaOrg
            });
        }

        public static IQueryable<JoinComentarioIdentidadPerfilComentarioBlogEntradaBlogBlogIdentidadPerfil> JoinPerfil(this IQueryable<JoinComentarioIdentidadPerfilComentarioBlogEntradaBlogBlogIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Identidad2.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new JoinComentarioIdentidadPerfilComentarioBlogEntradaBlogBlogIdentidadPerfil
            {
                EntradaBlog = item.EntradaBlog,
                Comentario = item.Comentario,
                ComentarioBlog = item.ComentarioBlog,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Blog = item.Blog,
                Identidad2 = item.Identidad2,
                Perfil2 = perfil
            });
        }

        public static IQueryable<JoinComentarioIdentidadPerfilComentarioBlogEntradaBlogBlogIdentidad> JoinIdentidad(this IQueryable<JoinComentarioIdentidadPerfilComentarioBlogEntradaBlogBlog> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.EntradaBlog.AutorID, identidad => identidad.IdentidadID, (item, identidad) => new JoinComentarioIdentidadPerfilComentarioBlogEntradaBlogBlogIdentidad
            {
                EntradaBlog = item.EntradaBlog,
                Comentario = item.Comentario,
                ComentarioBlog = item.ComentarioBlog,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Blog = item.Blog,
                Identidad2 = identidad
            });
        }

        public static IQueryable<JoinComentarioIdentidadPerfilComentarioBlogEntradaBlogBlog> JoinBlog(this IQueryable<JoinComentarioIdentidadPerfilComentarioBlogEntradaBlog> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Blog, item => item.EntradaBlog.BlogID, blog => blog.BlogID, (item, blog) => new JoinComentarioIdentidadPerfilComentarioBlogEntradaBlogBlog
            {
                EntradaBlog = item.EntradaBlog,
                Comentario = item.Comentario,
                ComentarioBlog = item.ComentarioBlog,
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Blog = blog
            });
        }

        public static IQueryable<JoinComentarioIdentidadPerfilComentarioBlogEntradaBlog> JoinEntradaBlog(this IQueryable<JoinComentarioIdentidadPerfilComentarioBlog> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.EntradaBlog, item => item.ComentarioBlog.EntradaBlogID, entradaBlog => entradaBlog.EntradaBlogID, (item, entradaBlog) => new JoinComentarioIdentidadPerfilComentarioBlogEntradaBlog
            {
                EntradaBlog = entradaBlog,
                Comentario = item.Comentario,
                ComentarioBlog = item.ComentarioBlog,
                Identidad = item.Identidad,
                Perfil = item.Perfil
            });
        }

        public static IQueryable<JoinVotoComentarioComentarioBlogBlogComentario> JoinComentario(this IQueryable<JoinVotoComentarioComentarioBlogBlog> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Comentario, item => item.VotoComentario.ComentarioID, comentario => comentario.ComentarioID, (item, comentario) => new JoinVotoComentarioComentarioBlogBlogComentario
            {
                Comentario = comentario,
                Blog = item.Blog,
                ComentarioBlog = item.ComentarioBlog,
                VotoComentario = item.VotoComentario
            });
        }

        public static IQueryable<JoinVotoComentarioComentarioBlogBlog> JoinBlog(this IQueryable<JoinVotoComentarioComentarioBlog> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Blog, item => item.ComentarioBlog.BlogID, blog => blog.BlogID, (item, blog) => new JoinVotoComentarioComentarioBlogBlog
            {
                Blog = blog,
                ComentarioBlog = item.ComentarioBlog,
                VotoComentario = item.VotoComentario
            });
        }

        public static IQueryable<JoinComentarioComentarioBlogBlog> JoinBlog(this IQueryable<JoinComentarioComentarioBlog> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Blog, item => item.ComentarioBlog.BlogID, blog => blog.BlogID, (item, blog) => new JoinComentarioComentarioBlogBlog
            {
                Blog = blog,
                Comentario = item.Comentario,
                ComentarioBlog = item.ComentarioBlog
            });
        }

        public static IQueryable<JoinComentarioComentarioBlog> JoinComentarioBlog(this IQueryable<EntityModel.Models.Comentario.Comentario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.ComentarioBlog, comentario => comentario.ComentarioID, comentarioBlog => comentarioBlog.ComentarioID, (comentario, comentarioBlog) => new JoinComentarioComentarioBlog
            {
                ComentarioBlog = comentarioBlog,
                Comentario = comentario
            });
        }

        public static IQueryable<JoinVotoComentarioComentarioBlogComentario> JoinComentario(this IQueryable<JoinVotoComentarioComentarioBlog> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Comentario, item => item.VotoComentario.ComentarioID, comentario => comentario.ComentarioID, (item, comentario) => new JoinVotoComentarioComentarioBlogComentario
            {
                Comentario = comentario,
                ComentarioBlog = item.ComentarioBlog,
                VotoComentario = item.VotoComentario
            });
        }

        public static IQueryable<JoinVotoComentarioComentarioBlog> JoinComentarioBlog(this IQueryable<VotoComentario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.ComentarioBlog, votoComentario => votoComentario.ComentarioID, comentarioBlog => comentarioBlog.ComentarioID, (votoComentario, comentarioBlog) => new JoinVotoComentarioComentarioBlog
            {
                ComentarioBlog = comentarioBlog,
                VotoComentario = votoComentario
            });
        }

        public static IQueryable<JoinComentarioIdentidadPerfilComentarioBlog> JoinComentarioBlog(this IQueryable<JoinComentarioIdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.ComentarioBlog, item => item.Comentario.ComentarioID, comentarioBlog => comentarioBlog.ComentarioID, (item, comentarioBlog) => new JoinComentarioIdentidadPerfilComentarioBlog
            {
                ComentarioBlog = comentarioBlog,
                Comentario = item.Comentario,
                Identidad = item.Identidad,
                Perfil = item.Perfil
            });
        }

        public static IQueryable<JoinDocumentoComentarioDocumento> JoinDocumento(this IQueryable<DocumentoComentario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, documentoComentario => documentoComentario.DocumentoID, documento => documento.DocumentoID, (documentoComentario, documento) => new JoinDocumentoComentarioDocumento
            {
                Documento = documento,
                DocumentoComentario = documentoComentario
            });
        }

        public static IQueryable<JoinComentarioBlogBlog> JoinBlog(this IQueryable<ComentarioBlog> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Blog, comentarioBlog => comentarioBlog.BlogID, blog => blog.BlogID, (comentarioBlog, blog) => new JoinComentarioBlogBlog
            {
                Blog = blog,
                ComentarioBlog = comentarioBlog
            });
        }

        public static IQueryable<JoinComentarioIdentidadPerfil> JoinPerfil(this IQueryable<JoinComentarioIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new JoinComentarioIdentidadPerfil
            {
                Perfil = perfil,
                Comentario = item.Comentario,
                Identidad = item.Identidad
            });
        }

        public static IQueryable<JoinComentarioIdentidad> JoinIdentidad(this IQueryable<EntityModel.Models.Comentario.Comentario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, comentario => comentario.IdentidadID, identidad => identidad.IdentidadID, (comentario, identidad) => new JoinComentarioIdentidad
            {
                Identidad = identidad,
                Comentario = comentario
            });
        }
    }

    /// <summary>
    /// Data adapter de comentario
    /// </summary>
    public class ComentarioAD : BaseAD
    {
        #region Miembros

        private EntityContext mEntityContext;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        #endregion

        #region Constructores
        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public ComentarioAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ComentarioAD> logger, ILoggerFactory loggerFactory)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mEntityContext = entityContext;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.CargarConsultasYDataAdapters();          
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD"></param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public ComentarioAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ComentarioAD> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mEntityContext = entityContext;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.CargarConsultasYDataAdapters(IBD);            
        }
        #endregion

        #region Consultas

        #region Select sin from

        private string selectComentarioSimple;

        #endregion

        private string sqlSelectComentario;
        private string sqlSelectVotoComentario;
        private string sqlSelectComentariosDeEntradaBlog;
        private string sqlSelectVotosComentariosDeEntradaBlog;
        private string sqlSelectComentariosDeBlogDePersona;
        private string sqlSelectComentariosDeBlog;
        private string sqlSelectVotosComentariosDeBlogDePersona;
        private string sqlSelectComentariosDeDocumento;
        private string sqlSelectVotosComentariosDeDocumento;
        private string sqlSelectComentariosDeDocumentoConFoto;
        private string sqlSelectComentariosDeDocumentoConFotoSinProysPrivExcpActual;
        private string sqlSelectVotosComentariosDeDocumentoSinProysPrivExcpActual;
        private string sqlSelectNumeroComentariosDeDocumento;

        private string sqlSelectComentariosDeDocumentoConFotoRaiz;
        private string sqlSelectComentariosDeDocumentoConFotoSinProysPrivExcpActualRaiz;
        private string sqlSelectComentariosDeDocumentoConFotoHijos;
        private string sqlSelectComentariosDeDocumentoConFotoSinProysPrivExcpActualHijos;

        #endregion

        #region DataAdapter

        #region ComentarioDataAdapter

        private string sqlComentarioInsert;
        private string sqlComentarioDelete;
        private string sqlComentarioModify;

        #endregion

        #region VotoComentario
        private string sqlVotoComentarioInsert;
        private string sqlVotoComentarioDelete;
        private string sqlVotoComentarioModify;
        #endregion

        #endregion

        #region Métodos generales
        class subconsulta
        {
            public Guid ComentarioID { get; set; }
            public short Tipo { get; set; }
            public Guid ProyectoID { get; set; }
            public string TituloElemento { get; set; }
        }
        /// <summary>
        /// Obtiene los comentarios de una lista de comentarios pasados por parámetro
        /// </summary>
        /// <param name="pListaComentarioID">Lista de identificadores de comentario</param>
        /// <returns>Dataset de comentarios</returns>
        public DataWrapperComentario ObtenerComentariosPorID(List<Guid> pListaComentarioID)
        {
            DataWrapperComentario comentarioDW = new DataWrapperComentario();

            if (pListaComentarioID.Count > 0)
            {
                var primeraParteSubconsulta = mEntityContext.ComentarioBlog.JoinBlog().Select(item => new
                {
                    ComentarioID = item.ComentarioBlog.ComentarioID,
                    Tipo = (short)TipoComentario.EntradaBlog,
                    ProyectoID = ProyectoAD.MetaProyecto,
                    TituloElemento = item.Blog.Titulo,
                });

                var seguntaParteSubconsulta = mEntityContext.DocumentoComentario.JoinDocumento().Select(item => new
                {
                    ComentarioID = item.DocumentoComentario.ComentarioID,
                    Tipo = (short)TipoComentario.Recurso,
                    ProyectoID = item.DocumentoComentario.ProyectoID.Value,
                    TituloElemento = item.Documento.Titulo
                });

                var consulta1 = mEntityContext.Comentario.Join(primeraParteSubconsulta, comentario => comentario.ComentarioID, join => join.ComentarioID, (comentario, join) => new
                {
                    Comentario = comentario,
                    Temp = join
                }).Join(mEntityContext.Proyecto, item => item.Temp.ProyectoID, proyecto => proyecto.ProyectoID, (item, proyecto) => new
                {
                    Proyecto = proyecto,
                    Comentario = item.Comentario,
                    Temp = item.Temp
                }).Join(mEntityContext.Identidad, item => item.Comentario.IdentidadID, identidad => identidad.IdentidadID, (item, identidad) => new
                {
                    Proyecto = item.Proyecto,
                    Comentario = item.Comentario,
                    Temp = item.Temp,
                    Identidad = identidad
                }).Join(mEntityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new
                {
                    Perfil = perfil,
                    Identidad = item.Identidad,
                    Proyecto = item.Proyecto,
                    Comentario = item.Comentario,
                    Temp = item.Temp
                }).Where(item => pListaComentarioID.Contains(item.Temp.ComentarioID)).ToList().Select(item => new EntityModel.Models.Comentario.Comentario
                {
                    ComentarioID = item.Comentario.ComentarioID,
                    IdentidadID = item.Comentario.IdentidadID,
                    Fecha = item.Comentario.Fecha,
                    Descripcion = item.Comentario.Descripcion,
                    Eliminado = item.Comentario.Eliminado,
                    ComentarioSuperiorID = item.Comentario.ComentarioSuperiorID,
                    Tipo = item.Temp.Tipo,
                    ProyectoID = item.Proyecto.ProyectoID,
                    NombreCorto = item.Proyecto.NombreCorto,
                    TituloElemento = item.Temp.TituloElemento,
                    NombreAutor = item.Perfil.NombrePerfil,
                    NombreOrganizacion = item.Perfil.NombreOrganizacion,
                    TipoPerfil = item.Identidad.Tipo,
                    OrganizacionPerfil = item.Perfil.OrganizacionID,
                    PersonaID = item.Perfil.PersonaID.Value,
                    Leido = false
                }).ToList();

                var consulta2 = mEntityContext.Comentario.Join(seguntaParteSubconsulta, comentario => comentario.ComentarioID, join => join.ComentarioID, (comentario, join) => new
                {
                    Comentario = comentario,
                    Temp = join
                }).Join(mEntityContext.Proyecto, item => item.Temp.ProyectoID, proyecto => proyecto.ProyectoID, (item, proyecto) => new
                {
                    Proyecto = proyecto,
                    Comentario = item.Comentario,
                    Temp = item.Temp
                }).Join(mEntityContext.Identidad, item => item.Comentario.IdentidadID, identidad => identidad.IdentidadID, (item, identidad) => new
                {
                    Proyecto = item.Proyecto,
                    Comentario = item.Comentario,
                    Temp = item.Temp,
                    Identidad = identidad
                }).Join(mEntityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new
                {
                    Perfil = perfil,
                    Identidad = item.Identidad,
                    Proyecto = item.Proyecto,
                    Comentario = item.Comentario,
                    Temp = item.Temp
                }).Where(item => pListaComentarioID.Contains(item.Temp.ComentarioID)).ToList().Select(item => new EntityModel.Models.Comentario.Comentario
                {
                    ComentarioID = item.Comentario.ComentarioID,
                    IdentidadID = item.Comentario.IdentidadID,
                    Fecha = item.Comentario.Fecha,
                    Descripcion = item.Comentario.Descripcion,
                    Eliminado = item.Comentario.Eliminado,
                    ComentarioSuperiorID = item.Comentario.ComentarioSuperiorID,
                    Tipo = item.Temp.Tipo,
                    ProyectoID = item.Proyecto.ProyectoID,
                    NombreCorto = item.Proyecto.NombreCorto,
                    TituloElemento = item.Temp.TituloElemento,
                    NombreAutor = item.Perfil.NombrePerfil,
                    NombreOrganizacion = item.Perfil.NombreOrganizacion,
                    TipoPerfil = item.Identidad.Tipo,
                    OrganizacionPerfil = item.Perfil.OrganizacionID,
                    PersonaID = item.Perfil.PersonaID.Value,
                    Leido = false
                }).ToList();

                comentarioDW.ListaComentario = consulta1.Union(consulta2).ToList();
            }

            return comentarioDW;
        }

        /// <summary>
        /// Obtiene los comentarios de una lista de comentarios pasados por parámetro con el autor y el padre del elemento vinculado
        /// </summary>
        /// <param name="pListaComentarioID">Lista de identificadores de comentario</param>
        /// <returns>Dataset de comentarios</returns>
        public DataWrapperComentario ObtenerEntradasBlogPorIDConPadreElemVinYAutor(List<Guid> pListaComentarioID)
        {
            DataWrapperComentario comentarioDW = new DataWrapperComentario();

            if (pListaComentarioID.Count > 0)
            {
                //Comentario
                var prueba = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().Take(3).ToList();

                comentarioDW.ListaComentario = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().Where(item => pListaComentarioID.Contains(item.Comentario.ComentarioID)).ToList().Select(item => new EntityModel.Models.Comentario.Comentario
                {
                    ComentarioID = item.Comentario.ComentarioID,
                    IdentidadID = item.Comentario.IdentidadID,
                    Fecha = item.Comentario.Fecha,
                    Descripcion = item.Comentario.Descripcion,
                    Eliminado = item.Comentario.Eliminado,
                    ComentarioSuperiorID = item.Comentario.ComentarioSuperiorID,
                    NombreAutor = item.Perfil.NombrePerfil,
                    NombreOrganizacion = item.Perfil.NombreOrganizacion,
                    TipoPerfil = item.Identidad.Tipo
                }).ToList();
            }

            return comentarioDW;
        }

        /// <summary>
        /// Obtiene todos los comentarios
        /// </summary>
        /// <returns></returns>
        public DataWrapperComentario ObtenerTodosComentarios()
        {
            DataWrapperComentario comentarioDW = new DataWrapperComentario();

            //Comentario
            comentarioDW.ListaComentario = mEntityContext.Comentario.ToList();

            return comentarioDW;
        }


        /// <summary>
        /// Obtiene todos los comentarios de una entrada de blog
        /// </summary>
        /// <param name="pBlogID">Identificador de blog</param>
        /// <param name="pEntradaBlogID">Identificador de entrada de blog</param>
        /// <returns></returns>
        public DataWrapperComentario ObtenerComentariosDeEntradaBlog(Guid pBlogID, Guid pEntradaBlogID)
        {
            DataWrapperComentario comentarioDW = new DataWrapperComentario();

            //Comentario
            comentarioDW.ListaComentario = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinComentarioBlog().Where(item => item.Comentario.Eliminado.Equals(false) && item.ComentarioBlog.BlogID.Equals(pBlogID) && item.ComentarioBlog.EntradaBlogID.Equals(pEntradaBlogID)).OrderBy(item => item.Comentario.Fecha).Select(item => new EntityModel.Models.Comentario.Comentario
            {
                ComentarioID = item.Comentario.ComentarioID,
                IdentidadID = item.Comentario.IdentidadID,
                Fecha = item.Comentario.Fecha,
                Descripcion = item.Comentario.Descripcion,
                Eliminado = item.Comentario.Eliminado,
                ComentarioSuperiorID = item.Comentario.ComentarioSuperiorID,
                PersonaID = item.Comentario.PersonaID,
                NombreAutor = item.Perfil.NombrePerfil,
                NombreOrganizacion = item.Perfil.NombreOrganizacion,
                TipoPerfil = item.Identidad.Tipo,
                OrganizacionPerfil = item.Perfil.OrganizacionID,
                ProyectoID = item.Identidad.ProyectoID
            }).ToList();

            //VotoComentario
            comentarioDW.ListaVotoComentario = mEntityContext.VotoComentario.JoinComentarioBlog().JoinComentario().Where(item => item.Comentario.Eliminado.Equals(false) && item.ComentarioBlog.BlogID.Equals(pBlogID) && item.ComentarioBlog.EntradaBlogID.Equals(pEntradaBlogID)).Select(item => item.VotoComentario).ToList();

            return comentarioDW;
        }

        /// <summary>
        /// Obtiene todos los comentarios de un blog
        /// </summary>
        /// <param name="pBlogID">Identificador de blog</param>
        /// <returns></returns>
        public DataWrapperComentario ObtenerComentariosDeBlog(Guid pBlogID)
        {
            DataWrapperComentario comentarioDW = new DataWrapperComentario();

            //Comentario
            comentarioDW.ListaComentario = mEntityContext.Comentario.JoinComentarioBlog().JoinBlog().Where(item => item.Comentario.Eliminado.Equals(false) && item.Blog.BlogID.Equals(pBlogID)).Select(item => item.Comentario).ToList();

            return comentarioDW;
        }

        /// <summary>
        /// Obtiene los comentarios de todas las entradas de todos los blogs de una identidad
        /// </summary>
        /// <param name="pAutorBlog">Identidad del autor</param>
        /// <returns></returns>
        public DataWrapperComentario ObtenerComentariosDeBlogDePersona(Guid pAutorBlog)
        {
            DataWrapperComentario comentarioDW = new DataWrapperComentario();

            //Comentario            
            comentarioDW.ListaComentario = mEntityContext.Comentario.JoinComentarioBlog().JoinBlog().Where(item => item.Comentario.Eliminado.Equals(false) && item.Blog.AutorID.Equals(pAutorBlog)).Select(item => item.Comentario).ToList();

            //VotoComentario
            comentarioDW.ListaVotoComentario = mEntityContext.VotoComentario.JoinComentarioBlog().JoinBlog().JoinComentario().Where(item => item.Comentario.Eliminado.Equals(false) && item.Blog.AutorID.Equals(pAutorBlog)).Select(item => item.VotoComentario).ToList();

            return comentarioDW;
        }

        /// <summary>
        /// Obtiene los comentarios de la persona en myGnoss
        /// </summary>
        /// <param name="pPersonaID">Persona para la que se obtienen los comentarios</param>
        /// <param name="pOrganizazionID">Organizacion del modo en el que se encuentra, null si el modo es personal</param>
        /// <param name="EsAdmin">verdad si el modo es de organizacion y es el administrador de la misma</param>
        /// <param name="pEsProfesor">Indica si la persona está conectada con identidad de profesor</param>
        /// <returns></returns>
        public DataWrapperComentario ObtenerComentariosDeMyGnoss(Guid pPersonaID, Guid? pOrganizazionID, bool EsAdmin, bool pEsProfesor)
        {
            DataWrapperComentario comentarioDW = new DataWrapperComentario();

            if (pOrganizazionID.HasValue)
            {
                if (EsAdmin)
                {
                    var comentarioBlog = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinComentarioBlog().JoinEntradaBlog().JoinBlog().JoinIdentidad().JoinPerfil().JoinPerfilPersonaOrg().Where(item => item.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizazionID) && item.Comentario.Eliminado.Equals(false) && !item.EntradaBlog.Eliminado);

                    var queryComentario = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinDocumentoComentario().JoinDocumento().JoinDocumentoWebVinBaseRecursos().JoinIdentidad().JoinPerfil().JoinPerfilPersonaOrg().Where(item => item.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizazionID) && item.Comentario.Eliminado.Equals(false) && item.Documento.Eliminado.Equals(false) && item.Documento.Tipo.Equals((short)TiposDocumentacion.Wiki));

                    if (pEsProfesor)
                    {
                        comentarioBlog = comentarioBlog.Where(item => item.Identidad2.Tipo.Equals((short)TiposIdentidad.Profesor));
                        queryComentario = queryComentario.Where(item => item.Identidad2.Tipo.Equals((short)TiposIdentidad.Profesor));
                    }
                    else
                    {
                        comentarioBlog = comentarioBlog.Where(item => !item.Identidad2.Tipo.Equals((short)TiposIdentidad.Profesor));
                        queryComentario = queryComentario.Where(item => !item.Identidad2.Tipo.Equals((short)TiposIdentidad.Profesor));
                    }

                    var querySelectComentarioBlog = comentarioBlog.Select(item => new
                    {
                        ComentarioID = item.Comentario.ComentarioID,
                        IdentidadID = item.Comentario.IdentidadID,
                        Fecha = item.Comentario.Fecha,
                        Descripcion = item.Comentario.Descripcion,
                        ComentarioSuperiorID = item.Comentario.ComentarioSuperiorID,
                        PersonaID = item.Perfil.PersonaID,
                        OrganizacionID = item.Perfil.OrganizacionID,
                        NombreAutor = item.Perfil.NombrePerfil,
                        NombreOrganizacion = item.Perfil.NombreOrganizacion,
                        TipoPerfil = item.Identidad.Tipo,
                        OrganizacionPerfil = item.Perfil.OrganizacionID,
                        NombreElemVinculado = item.EntradaBlog.Titulo,
                        ElementoVinculadoID = item.EntradaBlog.EntradaBlogID,
                        Tipo = (int)TipoComentario.EntradaBlog,
                        PadreElemVinID = item.Blog.BlogID,
                        NombrePadreElemVin = item.Blog.Titulo
                    });

                    var querySelectComentario = queryComentario.Select(item => new
                    {
                        ComentarioID = item.Comentario.ComentarioID,
                        IdentidadID = item.Comentario.IdentidadID,
                        Fecha = item.Comentario.Fecha,
                        Descripcion = item.Comentario.Descripcion,
                        ComentarioSuperiorID = item.Comentario.ComentarioSuperiorID,
                        PersonaID = item.Perfil.PersonaID,
                        OrganizacionID = item.Perfil.OrganizacionID,
                        NombreAutor = item.Perfil.NombrePerfil,
                        NombreOrganizacion = item.Perfil.NombreOrganizacion,
                        TipoPerfil = item.Identidad.Tipo,
                        OrganizacionPerfil = item.Perfil.OrganizacionID,
                        NombreElemVinculado = item.Documento.Titulo,
                        ElementoVinculadoID = item.Documento.DocumentoID,
                        Tipo = (int)TipoComentario.Recurso,
                        PadreElemVinID = item.DocumentoComentario.ProyectoID.Value,
                        NombrePadreElemVin = (string)null
                    });

                    comentarioDW.ListaComentario = querySelectComentarioBlog.Union(querySelectComentario).ToList().Select(item => new EntityModel.Models.Comentario.Comentario
                    {
                        ComentarioID = item.ComentarioID,
                        IdentidadID = item.IdentidadID,
                        Fecha = item.Fecha,
                        Descripcion = item.Descripcion,
                        ComentarioSuperiorID = item.ComentarioSuperiorID,
                        PersonaID = item.PersonaID,
                        OrganizacionID = item.OrganizacionID,
                        NombreAutor = item.NombreAutor,
                        NombreOrganizacion = item.NombreOrganizacion,
                        TipoPerfil = item.TipoPerfil,
                        OrganizacionPerfil = item.OrganizacionID,
                        NombreElemVinculado = item.NombreElemVinculado,
                        ElementoVinculadoID = item.ElementoVinculadoID,
                        Tipo = (short)item.Tipo,
                        PadreElemVinID = item.PadreElemVinID,
                        NombrePadreElemVin = item.NombrePadreElemVin
                    }).ToList();
                }
                else
                {
                    var comentarioBlog = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinComentarioBlog().JoinEntradaBlog().JoinBlog().JoinIdentidad().JoinPerfil().JoinPerfilPersonaOrg().Where(item => item.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizazionID) && item.Comentario.Eliminado.Equals(false) && item.EntradaBlog.Eliminado.Equals(0));

                    var queryComentario = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinDocumentoComentario().JoinDocumento().JoinDocumentoWebVinBaseRecursos().JoinIdentidad().JoinPerfil().JoinPerfilPersonaOrg().Where(item => item.PerfilPersonaOrg.PersonaID.Equals(pPersonaID) && item.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizazionID) && item.Comentario.Eliminado.Equals(false) && item.Documento.Eliminado.Equals(false) && item.Documento.Tipo.Equals((short)TiposDocumentacion.Wiki));

                    if (pEsProfesor)
                    {
                        comentarioBlog = comentarioBlog.Where(item => item.Identidad2.Tipo.Equals((short)TiposIdentidad.Profesor));
                        queryComentario = queryComentario.Where(item => item.Identidad2.Tipo.Equals((short)TiposIdentidad.Profesor));
                    }
                    else
                    {
                        comentarioBlog = comentarioBlog.Where(item => !item.Identidad2.Tipo.Equals((short)TiposIdentidad.Profesor));
                        queryComentario = queryComentario.Where(item => !item.Identidad2.Tipo.Equals((short)TiposIdentidad.Profesor));
                    }
                    var querySelectComentarioBlog = comentarioBlog.Select(item => new
                    {
                        ComentarioID = item.Comentario.ComentarioID,
                        IdentidadID = item.Comentario.IdentidadID,
                        Fecha = item.Comentario.Fecha,
                        Descripcion = item.Comentario.Descripcion,
                        ComentarioSuperiorID = item.Comentario.ComentarioSuperiorID,
                        PersonaID = item.Perfil.PersonaID,
                        OrganizacionID = item.Perfil.OrganizacionID,
                        NombreAutor = item.Perfil.NombrePerfil,
                        NombreOrganizacion = item.Perfil.NombreOrganizacion,
                        TipoPerfil = item.Identidad.Tipo,
                        OrganizacionPerfil = item.Perfil.OrganizacionID,
                        NombreElemVinculado = item.EntradaBlog.Titulo,
                        ElementoVinculadoID = item.EntradaBlog.EntradaBlogID,
                        Tipo = (int)TipoComentario.EntradaBlog,
                        PadreElemVinID = item.Blog.BlogID,
                        NombrePadreElemVin = item.Blog.Titulo
                    });

                    var querySelectComentario = queryComentario.Select(item => new
                    {
                        ComentarioID = item.Comentario.ComentarioID,
                        IdentidadID = item.Comentario.IdentidadID,
                        Fecha = item.Comentario.Fecha,
                        Descripcion = item.Comentario.Descripcion,
                        ComentarioSuperiorID = item.Comentario.ComentarioSuperiorID,
                        PersonaID = item.Perfil.PersonaID,
                        OrganizacionID = item.Perfil.OrganizacionID,
                        NombreAutor = item.Perfil.NombrePerfil,
                        NombreOrganizacion = item.Perfil.NombreOrganizacion,
                        TipoPerfil = item.Identidad.Tipo,
                        OrganizacionPerfil = item.Perfil.OrganizacionID,
                        NombreElemVinculado = item.Documento.Titulo,
                        ElementoVinculadoID = item.Documento.DocumentoID,
                        Tipo = (int)TipoComentario.Recurso,
                        PadreElemVinID = item.DocumentoComentario.ProyectoID.Value,
                        NombrePadreElemVin = (string)null
                    });

                    comentarioDW.ListaComentario = querySelectComentarioBlog.Union(querySelectComentario).ToList().Select(item => new EntityModel.Models.Comentario.Comentario
                    {
                        ComentarioID = item.ComentarioID,
                        IdentidadID = item.IdentidadID,
                        Fecha = item.Fecha,
                        Descripcion = item.Descripcion,
                        ComentarioSuperiorID = item.ComentarioSuperiorID,
                        PersonaID = item.PersonaID,
                        OrganizacionID = item.OrganizacionID,
                        NombreAutor = item.NombreAutor,
                        NombreOrganizacion = item.NombreOrganizacion,
                        TipoPerfil = item.TipoPerfil,
                        OrganizacionPerfil = item.OrganizacionID,
                        NombreElemVinculado = item.NombreElemVinculado,
                        ElementoVinculadoID = item.ElementoVinculadoID,
                        Tipo = (short)item.Tipo,
                        PadreElemVinID = item.PadreElemVinID,
                        NombrePadreElemVin = item.NombrePadreElemVin
                    }).ToList();
                }
            }
            else
            {
                var comentarioBlog = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinComentarioBlog().JoinEntradaBlog().JoinBlog().JoinIdentidad().JoinPerfil().JoinPerfilPersona().Where(item => item.PerfilPersona.PersonaID.Equals(pPersonaID) && item.Comentario.Eliminado.Equals(false) && !item.EntradaBlog.Eliminado);

                var queryComentario = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinDocumentoComentario().JoinDocumento().JoinDocumentoWebVinBaseRecursos().JoinIdentidad().JoinPerfil().JoinPerfilPersona().Where(item => item.PerfilPersona.PersonaID.Equals(pPersonaID) && item.Comentario.Eliminado.Equals(false) && item.Documento.Eliminado.Equals(false) && item.Documento.Tipo.Equals((short)TiposDocumentacion.Wiki));

                if (pEsProfesor)
                {
                    comentarioBlog = comentarioBlog.Where(item => item.Identidad2.Tipo.Equals((short)TiposIdentidad.Profesor));
                    queryComentario = queryComentario.Where(item => item.Identidad2.Tipo.Equals((short)TiposIdentidad.Profesor));
                }
                else
                {
                    comentarioBlog = comentarioBlog.Where(item => !item.Identidad2.Tipo.Equals((short)TiposIdentidad.Profesor));
                    queryComentario = queryComentario.Where(item => !item.Identidad2.Tipo.Equals((short)TiposIdentidad.Profesor));
                }

                var querySelectComentarioBlog = comentarioBlog.Select(item => new
                {
                    ComentarioID = item.Comentario.ComentarioID,
                    IdentidadID = item.Comentario.IdentidadID,
                    Fecha = item.Comentario.Fecha,
                    Descripcion = item.Comentario.Descripcion,
                    ComentarioSuperiorID = item.Comentario.ComentarioSuperiorID,
                    PersonaID = item.Perfil.PersonaID,
                    OrganizacionID = item.Perfil.OrganizacionID,
                    NombreAutor = item.Perfil.NombrePerfil,
                    NombreOrganizacion = item.Perfil.NombreOrganizacion,
                    TipoPerfil = item.Identidad.Tipo,
                    OrganizacionPerfil = item.Perfil.OrganizacionID,
                    NombreElemVinculado = item.EntradaBlog.Titulo,
                    ElementoVinculadoID = item.EntradaBlog.EntradaBlogID,
                    Tipo = (int)TipoComentario.EntradaBlog,
                    PadreElemVinID = item.Blog.BlogID,
                    NombrePadreElemVin = item.Blog.Titulo
                }).ToList();

                var querySelectComentario = queryComentario.Select(item => new
                {
                    ComentarioID = item.Comentario.ComentarioID,
                    IdentidadID = item.Comentario.IdentidadID,
                    Fecha = item.Comentario.Fecha,
                    Descripcion = item.Comentario.Descripcion,
                    ComentarioSuperiorID = item.Comentario.ComentarioSuperiorID,
                    PersonaID = item.Perfil.PersonaID,
                    OrganizacionID = item.Perfil.OrganizacionID,
                    NombreAutor = item.Perfil.NombrePerfil,
                    NombreOrganizacion = item.Perfil.NombreOrganizacion,
                    TipoPerfil = item.Identidad.Tipo,
                    OrganizacionPerfil = item.Perfil.OrganizacionID,
                    NombreElemVinculado = item.Documento.Titulo,
                    ElementoVinculadoID = item.Documento.DocumentoID,
                    Tipo = (int)TipoComentario.Recurso,
                    PadreElemVinID = item.DocumentoComentario.ProyectoID.Value,
                    NombrePadreElemVin = (string)null
                }).ToList();

                comentarioDW.ListaComentario = querySelectComentarioBlog.Union(querySelectComentario).ToList().Select(item => new EntityModel.Models.Comentario.Comentario
                {
                    ComentarioID = item.ComentarioID,
                    IdentidadID = item.IdentidadID,
                    Fecha = item.Fecha,
                    Descripcion = item.Descripcion,
                    ComentarioSuperiorID = item.ComentarioSuperiorID,
                    PersonaID = item.PersonaID,
                    OrganizacionID = item.OrganizacionID,
                    NombreAutor = item.NombreAutor,
                    NombreOrganizacion = item.NombreOrganizacion,
                    TipoPerfil = item.TipoPerfil,
                    OrganizacionPerfil = item.OrganizacionID,
                    NombreElemVinculado = item.NombreElemVinculado,
                    ElementoVinculadoID = item.ElementoVinculadoID,
                    Tipo = (short)item.Tipo,
                    PadreElemVinID = item.PadreElemVinID,
                    NombrePadreElemVin = item.NombrePadreElemVin
                }).ToList();

            }
            return comentarioDW;
        }

        /// <summary>
        /// Compruebo si una identidad ha hecho algún comentario
        /// </summary>
        /// <param name="pIdentidadID">Identidad a comprobar</param>
        /// <returns></returns>
        public bool ComprobarIdentidadHaComentado(Guid pIdentidadID)
        {
            return mEntityContext.Comentario.Any(item => item.IdentidadID.Equals(pIdentidadID) && item.Eliminado.Equals(false));
        }

        /// <summary>
        /// Obtiene los comentarios de las identidades
        /// </summary>
        /// <param name="pListaIdentidadesID">Identidades para la que se obtienen los comentarios</param>
        /// <returns></returns>
        public DataWrapperComentario ObtenerComentariosDeIdentidades(List<Guid> pListaIdentidadesID)
        {
            DataWrapperComentario comentarioDW = new DataWrapperComentario();

            var comentarioBlog = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinComentarioBlog().JoinEntradaBlog().JoinBlog().JoinIdentidad().JoinPerfil().Where(item => pListaIdentidadesID.Contains(item.Comentario.IdentidadID)).Select(item => new
            {
                ComentarioID = item.Comentario.ComentarioID,
                IdentidadID = item.Comentario.IdentidadID,
                Fecha = item.Comentario.Fecha,
                Descripcion = item.Comentario.Descripcion,
                Eliminado = item.Comentario.Eliminado,
                ComentarioSuperiorID = item.Comentario.ComentarioSuperiorID,
                PersonaID = item.Perfil.PersonaID,
                OrganizacionID = item.Perfil.OrganizacionID,
                NombreAutor = item.Perfil.NombrePerfil,
                NombreOrganizacion = item.Perfil.NombreOrganizacion,
                TipoPerfil = item.Identidad.Tipo,
                OrganizacionPerfil = item.Perfil.OrganizacionID,
                NombreElemVinculado = item.EntradaBlog.Titulo,
                ElementoVinculadoID = item.EntradaBlog.EntradaBlogID,
                Tipo = (int)TipoComentario.EntradaBlog,
                PadreElemVinID = item.Blog.BlogID,
                NombrePadreElemVin = item.Blog.NombreCorto
            });

            var comentarioRecurso = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinDocumentoComentario().JoinDocumento().JoinIdentidad().JoinPerfil().Where(item => pListaIdentidadesID.Contains(item.Comentario.IdentidadID)).Select(item => new
            {
                ComentarioID = item.Comentario.ComentarioID,
                IdentidadID = item.Comentario.IdentidadID,
                Fecha = item.Comentario.Fecha,
                Descripcion = item.Comentario.Descripcion,
                Eliminado = item.Comentario.Eliminado,
                ComentarioSuperiorID = item.Comentario.ComentarioSuperiorID,
                PersonaID = item.Perfil.PersonaID,
                OrganizacionID = item.Perfil.OrganizacionID,
                NombreAutor = item.Perfil.NombrePerfil,
                NombreOrganizacion = item.Perfil.NombreOrganizacion,
                TipoPerfil = item.Identidad.Tipo,
                OrganizacionPerfil = item.Perfil.OrganizacionID,
                NombreElemVinculado = item.Documento.Titulo,
                ElementoVinculadoID = item.Documento.DocumentoID,
                Tipo = (int)TipoComentario.Recurso,
                PadreElemVinID = item.DocumentoComentario.ProyectoID.Value,
                NombrePadreElemVin = (string)null
            });

            comentarioDW.ListaComentario = comentarioBlog.Union(comentarioRecurso).ToList().Select(item => new EntityModel.Models.Comentario.Comentario
            {
                ComentarioID = item.ComentarioID,
                IdentidadID = item.IdentidadID,
                Fecha = item.Fecha,
                Descripcion = item.Descripcion,
                Eliminado = item.Eliminado,
                ComentarioSuperiorID = item.ComentarioSuperiorID,
                PersonaID = item.PersonaID,
                OrganizacionID = item.OrganizacionID,
                NombreAutor = item.NombreAutor,
                NombreOrganizacion = item.NombreOrganizacion,
                TipoPerfil = item.TipoPerfil,
                OrganizacionPerfil = item.OrganizacionID,
                NombreElemVinculado = item.NombreElemVinculado,
                ElementoVinculadoID = item.ElementoVinculadoID,
                Tipo = (int)TipoComentario.EntradaBlog,
                PadreElemVinID = item.PadreElemVinID,
                NombrePadreElemVin = item.NombrePadreElemVin
            }).ToList();

            return comentarioDW;
        }

        /// <summary>
        /// Obtiene los comentarios de las identidades
        /// </summary>
        /// <param name="pListaComentariosID">Identidades para la que se obtienen los comentarios</param>
        /// <returns></returns>
        public DataWrapperComentario ObtenerComentariosDeDocumentosPorComentariosID(List<Guid> pListaComentariosID)
        {
            DataWrapperComentario comentarioDW = new DataWrapperComentario();

            if (pListaComentariosID.Count > 0)
            {
                comentarioDW.ListaComentario = mEntityContext.Comentario.Where(item => pListaComentariosID.Contains(item.ComentarioID)).ToList();
            }
            return comentarioDW;
        }

        /// <summary>
        /// Obtiene el numero de comentarios de un documento por su clave
        /// </summary>
        /// <param name="pClave"></param>
        /// <returns></returns>
        public int ObtenerNumeroDeComentariosDeDocumento(Guid pClave)
        {
            return mEntityContext.DocumentoComentario.Where(item => item.DocumentoID.Equals(pClave)).Count();
        }

        public int ObtenerComentariosdeUsuarioEnProyecto(Guid pIdentidadID, Guid pProyectoID, DateTime? pFechaInit, DateTime? pFechaFin)
        {
            var consulta = mEntityContext.Comentario.JoinDocumentoComentario().Where(item => item.DocumentoComentario.ProyectoID.HasValue && item.DocumentoComentario.ProyectoID.Value.Equals(pProyectoID) && item.Comentario.IdentidadID.Equals(pIdentidadID));
            if (pFechaInit != null)
            {
                consulta = consulta.Where(item => item.Comentario.Fecha > pFechaInit.Value);
            }
            if (pFechaFin != null)
            {
                consulta = consulta.Where(item => item.Comentario.Fecha < pFechaFin.Value);
            }
            return consulta.Count();
        }


        /// <summary>
        /// Obtiene los comentarios de un documento.
        /// </summary>
        ///<param name="pDocumentoID">Identificador de documento</param>
        /// <param name="pSoloProyectoSinPrivadosID">Contiene el proyecto del que se deben traer los coment a parte de los publicos
        /// y restringidos. Empty si se deben traer todos</param>
        /// <returns>DataSet de Comentario</returns>
        public DataWrapperComentario ObtenerComentariosDeDocumento(Guid pDocumentoID, Guid pSoloProyectoSinPrivadosID)
        {
            DataWrapperComentario comentarioDW = new DataWrapperComentario();

            if (pSoloProyectoSinPrivadosID == Guid.Empty)
            {
                //Comentario
                comentarioDW.ListaComentario = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinDocumentoComentario().Where(item => item.Comentario.Eliminado.Equals(false) && item.DocumentoComentario.DocumentoID.Equals(pDocumentoID)).ToList().Select(item => new EntityModel.Models.Comentario.Comentario
                {
                    ComentarioID = item.Comentario.ComentarioID,
                    IdentidadID = item.Comentario.IdentidadID,
                    Fecha = item.Comentario.Fecha,
                    Descripcion = item.Comentario.Descripcion,
                    Eliminado = item.Comentario.Eliminado,
                    ComentarioSuperiorID = item.Comentario.ComentarioSuperiorID,
                    NombreAutor = item.Perfil.NombrePerfil,
                    NombreOrganizacion = item.Perfil.NombreOrganizacion,
                    TipoPerfil = item.Identidad.Tipo,
                    OrganizacionPerfil = item.Perfil.OrganizacionID,
                    PersonaID = item.Perfil.PersonaID
                }).ToList();

                //VotoComentario
                comentarioDW.ListaVotoComentario = mEntityContext.VotoComentario.JoinDocumentoComentario().JoinComentario().Where(item => item.Comentario.Eliminado.Equals(false) && item.DocumentoComentario.DocumentoID.Equals(pDocumentoID)).Select(item => item.VotoComentario).ToList();
            }
            else
            {
                //Comentario
                comentarioDW.ListaComentario = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinDocumentoComentario().JoinProyecto().Where(item => item.Comentario.Eliminado.Equals(false) && item.DocumentoComentario.DocumentoID.Equals(pDocumentoID) && (!item.Proyecto.TipoAcceso.Equals((short)TipoAcceso.Privado) && !item.Proyecto.TipoAcceso.Equals((short)TipoAcceso.Reservado) || item.Proyecto.ProyectoID.Equals(pSoloProyectoSinPrivadosID))).ToList().Select(item => new EntityModel.Models.Comentario.Comentario
                {
                    ComentarioID = item.Comentario.ComentarioID,
                    IdentidadID = item.Comentario.IdentidadID,
                    Fecha = item.Comentario.Fecha,
                    Descripcion = item.Comentario.Descripcion,
                    Eliminado = item.Comentario.Eliminado,
                    ComentarioSuperiorID = item.Comentario.ComentarioSuperiorID,
                    NombreAutor = item.Perfil.NombrePerfil,
                    NombreOrganizacion = item.Perfil.NombreOrganizacion,
                    TipoPerfil = item.Identidad.Tipo,
                    OrganizacionPerfil = item.Perfil.OrganizacionID,
                    PersonaID = item.Perfil.PersonaID
                }).ToList();

                //VotoComentario
                comentarioDW.ListaVotoComentario = mEntityContext.VotoComentario.JoinDocumentoComentario().JoinComentario().JoinProyecto().Where(item => item.Comentario.Eliminado.Equals(false) && item.DocumentoComentario.DocumentoID.Equals(pDocumentoID) && (!item.Proyecto.TipoAcceso.Equals((short)TipoAcceso.Privado) && !item.Proyecto.TipoAcceso.Equals((short)TipoAcceso.Reservado)) || item.Proyecto.ProyectoID.Equals(pSoloProyectoSinPrivadosID)).Select(item => item.VotoComentario).ToList();
            }

            return comentarioDW;
        }

        /// <summary>
        /// Obtiene los comentarios de unos documentos.
        /// </summary>
        /// <param name="pComentarioDW">DataSet de Comentario</param>
        ///<param name="pDocumentosID">Identificadores de documentos</param>
        /// <param name="pSoloProyectoSinPrivadosID">Contiene el proyecto del que se deben traer los coment a parte de los publicos
        /// y restringidos. Empty si se deben traer todos</param>
        /// <param name="pInicio">Inicio paginación</param>
        /// <param name="pLimite">Limite paginación</param>
        /// <param name="pMostrarComentariorSoloProyecto">Indica si se deben mostrar solo los comentarios este proyecto</param>
        /// <returns>Numero de resultados raíz totales</returns>
        public int ObtenerComentariosDeDocumentos(DataWrapperComentario pComentarioDW, List<Guid> pDocumentosID, Guid pSoloProyectoSinPrivadosID, int pInicio, int pLimite, bool pMostrarComentariorSoloProyecto)
        {
            DataWrapperComentario comentarioDW = new DataWrapperComentario();

            List<EntityModel.Models.Comentario.Comentario> listaComentarios = new List<EntityModel.Models.Comentario.Comentario>();

            var query = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinDocumentoComentario();
            int cuantos = pLimite - pInicio;

            if (pSoloProyectoSinPrivadosID != Guid.Empty)
            {
                var soloProyecto = query.JoinProyecto();

                if (pMostrarComentariorSoloProyecto)
                {
                    listaComentarios = soloProyecto.Where(item => !item.Comentario.Eliminado && pDocumentosID.Contains(item.DocumentoComentario.DocumentoID) && !item.Comentario.ComentarioSuperiorID.HasValue && item.Proyecto.ProyectoID.Equals(pSoloProyectoSinPrivadosID)).ToList().Select(item => new EntityModel.Models.Comentario.Comentario
                    {
                        ComentarioID = item.Comentario.ComentarioID,
                        IdentidadID = item.Comentario.IdentidadID,
                        Fecha = item.Comentario.Fecha,
                        Descripcion = item.Comentario.Descripcion,
                        Eliminado = item.Comentario.Eliminado,
                        ComentarioSuperiorID = item.Comentario.ComentarioSuperiorID,
                        NombreAutor = item.Perfil.NombrePerfil,
                        NombreOrganizacion = item.Perfil.NombreOrganizacion,
                        TipoPerfil = item.Identidad.Tipo,
                        OrganizacionPerfil = item.Perfil.OrganizacionID,
                        PersonaID = item.Perfil.PersonaID
                    }).OrderByDescending(item => item.Fecha).Skip(pInicio).Take(cuantos).ToList();
                }
                else
                {
                    listaComentarios = soloProyecto.Where(item => !item.Comentario.Eliminado && pDocumentosID.Contains(item.DocumentoComentario.DocumentoID) && !item.Comentario.ComentarioSuperiorID.HasValue && ((!item.Proyecto.TipoAcceso.Equals((short)TipoAcceso.Privado)) && !item.Proyecto.TipoAcceso.Equals((short)TipoAcceso.Reservado) || item.Proyecto.ProyectoID.Equals(pSoloProyectoSinPrivadosID))).ToList().Select(item => new EntityModel.Models.Comentario.Comentario
                    {
                        ComentarioID = item.Comentario.ComentarioID,
                        IdentidadID = item.Comentario.IdentidadID,
                        Fecha = item.Comentario.Fecha,
                        Descripcion = item.Comentario.Descripcion,
                        Eliminado = item.Comentario.Eliminado,
                        ComentarioSuperiorID = item.Comentario.ComentarioSuperiorID,
                        NombreAutor = item.Perfil.NombrePerfil,
                        NombreOrganizacion = item.Perfil.NombreOrganizacion,
                        TipoPerfil = item.Identidad.Tipo,
                        OrganizacionPerfil = item.Perfil.OrganizacionID,
                        PersonaID = item.Perfil.PersonaID
                    }).OrderByDescending(item => item.Fecha).Skip(pInicio).Take(cuantos).ToList();
                }
            }
            else
            {
                listaComentarios = query.Where(item => !item.Comentario.Eliminado && pDocumentosID.Contains(item.DocumentoComentario.DocumentoID) && !item.Comentario.ComentarioSuperiorID.HasValue).ToList().Select(item => new EntityModel.Models.Comentario.Comentario
                {
                    ComentarioID = item.Comentario.ComentarioID,
                    IdentidadID = item.Comentario.IdentidadID,
                    Fecha = item.Comentario.Fecha,
                    Descripcion = item.Comentario.Descripcion,
                    Eliminado = item.Comentario.Eliminado,
                    ComentarioSuperiorID = item.Comentario.ComentarioSuperiorID,
                    NombreAutor = item.Perfil.NombrePerfil,
                    NombreOrganizacion = item.Perfil.NombreOrganizacion,
                    TipoPerfil = item.Identidad.Tipo,
                    OrganizacionPerfil = item.Perfil.OrganizacionID,
                    PersonaID = item.Perfil.PersonaID
                }).OrderByDescending(item => item.Fecha).Skip(pInicio).Take(cuantos).ToList();
            }


            cuantos = listaComentarios.Count;

            if (cuantos > 0 && pComentarioDW != null)
            {
                listaComentarios = listaComentarios.Skip(pInicio - 1).Take(cuantos).ToList();
                comentarioDW.ListaComentario = listaComentarios;

                //Traigo hijos de comentarios:
                ObtenerComentariosDocHijosDeComentariosCargados(comentarioDW, pSoloProyectoSinPrivadosID);

                if (comentarioDW.ListaComentario.Count > 0)
                {
                    List<Guid> listaIDs = comentarioDW.ListaComentario.Select(item => item.ComentarioID).ToList();

                    comentarioDW.ListaVotoComentario = mEntityContext.VotoComentario.Where(item => listaIDs.Contains(item.ComentarioID)).ToList();
                }

                pComentarioDW.Merge(comentarioDW);
            }

            return cuantos;
        }

        /// <summary>
        /// Obtiene los comentarios de documentos hijos de los comentarios ya cargados.
        /// </summary>
        /// <param name="pComentarioDW">DataSet de comentario</param>
        /// <param name="pSoloProyectoSinPrivadosID">Contiene el proyecto del que se deben traer los coment a parte de los publicos
        /// y restringidos. Empty si se deben traer todos</param>
        public void ObtenerComentariosDocHijosDeComentariosCargados(DataWrapperComentario pComentarioDW, Guid pSoloProyectoSinPrivadosID)
        {
            if (pComentarioDW.ListaComentario.Count > 0)
            {
                DataWrapperComentario comentarioDW = new DataWrapperComentario();

                List<Guid> listaComentarioID = pComentarioDW.ListaComentario.Select(item => item.ComentarioID).ToList();

                var query = mEntityContext.Comentario.JoinIdentidad().JoinPerfil().JoinDocumentoComentario();

                if (pSoloProyectoSinPrivadosID != Guid.Empty)
                {
                    var finalQuery = query.JoinProyecto().Where(item => !item.Comentario.Eliminado && listaComentarioID.Contains(item.Comentario.ComentarioSuperiorID.Value) && ((!item.Proyecto.TipoAcceso.Equals((short)TipoAcceso.Privado) && !item.Proyecto.TipoAcceso.Equals((short)TipoAcceso.Reservado)) || item.Proyecto.ProyectoID.Equals(pSoloProyectoSinPrivadosID))).ToList().Select(item => new EntityModel.Models.Comentario.Comentario
                    {
                        ComentarioID = item.Comentario.ComentarioID,
                        IdentidadID = item.Comentario.IdentidadID,
                        Fecha = item.Comentario.Fecha,
                        Descripcion = item.Comentario.Descripcion,
                        Eliminado = item.Comentario.Eliminado,
                        ComentarioSuperiorID = item.Comentario.ComentarioSuperiorID,
                        NombreAutor = item.Perfil.NombrePerfil,
                        NombreOrganizacion = item.Perfil.NombreOrganizacion,
                        TipoPerfil = item.Identidad.Tipo,
                        OrganizacionPerfil = item.Perfil.OrganizacionID,
                        PersonaID = item.Perfil.PersonaID
                    }).OrderByDescending(item => item.Fecha);

                    comentarioDW.ListaComentario = finalQuery.ToList();
                }
                else
                {
                    comentarioDW.ListaComentario = query.Where(item => item.Comentario.Eliminado.Equals(false) && listaComentarioID.Contains(item.Comentario.ComentarioSuperiorID.Value)).ToList().Select(item => new EntityModel.Models.Comentario.Comentario
                    {
                        ComentarioID = item.Comentario.ComentarioID,
                        IdentidadID = item.Comentario.IdentidadID,
                        Fecha = item.Comentario.Fecha,
                        Descripcion = item.Comentario.Descripcion,
                        Eliminado = item.Comentario.Eliminado,
                        ComentarioSuperiorID = item.Comentario.ComentarioSuperiorID,
                        NombreAutor = item.Perfil.NombrePerfil,
                        NombreOrganizacion = item.Perfil.NombreOrganizacion,
                        TipoPerfil = item.Identidad.Tipo,
                        OrganizacionPerfil = item.Perfil.OrganizacionID,
                        PersonaID = item.Perfil.PersonaID
                    }).OrderByDescending(item => item.Fecha).ToList();
                }

                ObtenerComentariosDocHijosDeComentariosCargados(comentarioDW, pSoloProyectoSinPrivadosID);

                pComentarioDW.Merge(comentarioDW);
            }
        }

        public EntityModel.Models.Comentario.Comentario ObtenerComentarioPorID(Guid pComentarioID)
        {
            return mEntityContext.Comentario.Where(item => item.ComentarioID.Equals(pComentarioID)).FirstOrDefault();
        }

        public List<EntityModel.Models.Comentario.Comentario> ObtenerTodosComentariosHijosDeComentarios(List<EntityModel.Models.Comentario.Comentario> pListaComentarios)
        {
            List<EntityModel.Models.Comentario.Comentario> listaComentariosHijos = new List<EntityModel.Models.Comentario.Comentario>();

            foreach (EntityModel.Models.Comentario.Comentario comentario in pListaComentarios)
            {
                listaComentariosHijos.AddRange(mEntityContext.Comentario.Where(item => item.ComentarioSuperiorID.HasValue && item.ComentarioSuperiorID.Value.Equals(comentario.ComentarioID)).ToList());
            }

            if (listaComentariosHijos.Count > 0)
            {
                listaComentariosHijos.AddRange(ObtenerTodosComentariosHijosDeComentarios(listaComentariosHijos));
            }

            return listaComentariosHijos;
        }

        /// <summary>
        /// Obtiene los comentarios de un documento.
        /// </summary>
        ///<param name="pDocumentoID">Identificador de documento</param>
        /// <param name="pSoloProyectoSinPrivadosID">Contiene el proyecto del que se deben traer los coment a parte de los publicos
        /// y restringidos. Empty si se deben traer todos</param>
        /// <returns>DataSet de Comentario</returns>
        public string ObtenerUltimoComentarioDeDocumento(Guid pDocumentoID)
        {
            return mEntityContext.Comentario.JoinDocumentoComentario().Where(item => item.DocumentoComentario.DocumentoID.Equals(pDocumentoID) && item.Comentario.Eliminado.Equals(false) && !item.Comentario.ComentarioSuperiorID.HasValue).OrderByDescending(item => item.Comentario.Fecha).Select(item => item.Comentario.Descripcion).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene los comentarios de un documento.
        /// </summary>
        ///<param name="pDocumentoID">Identificador de documento</param>
        /// <param name="pSoloProyectoSinPrivadosID">Contiene el proyecto del que se deben traer los coment a parte de los publicos
        /// y restringidos. Empty si se deben traer todos</param>
        /// <returns>DataSet de Comentario</returns>
        public DataWrapperComentario ObtenerUltimoComentarioDeDocumentoCompleto(Guid pDocumentoID)
        {
            DataWrapperComentario dataWrapperComentario = new DataWrapperComentario();

            dataWrapperComentario.ListaComentario = mEntityContext.Comentario.JoinDocumentoComentario().Where(item => item.DocumentoComentario.DocumentoID.Equals(pDocumentoID) && item.Comentario.Eliminado.Equals(false) && !item.Comentario.ComentarioSuperiorID.HasValue).OrderByDescending(item => item.Comentario.Fecha).Select(item => item.Comentario).Take(1).ToList();

            return dataWrapperComentario;
        }

        /// <summary>
        /// Indica si un documento tiene comentario en otra comunidad que no es la indicada.
        /// </summary>
        /// <param name="pDocumentoID">Documento</param>
        /// <param name="pProyectoID">Comunindad</param>
        /// <returns>TRUE si un documento tiene comentario en otra comunidad que no es la indicada, FALSE en caso contrario</returns>
        public bool DocumentoTieneComentarioOtraComunidad(Guid pDocumentoID, Guid pProyectoID)
        {
            return mEntityContext.DocumentoComentario.Any(item => item.DocumentoID.Equals(pDocumentoID) && item.ProyectoID.HasValue && !item.ProyectoID.Value.Equals(pProyectoID));
        }

        /// <summary>
        /// Actualiza el comentario de Entity
        /// </summary>
        public void ActualizarComentarioEntity()
        {
            base.ActualizarBaseDeDatosEntityContext();
        }

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

            #region Select sin from

            this.selectComentarioSimple = "SELECT " + IBD.CargarGuid("Comentario.ComentarioID") + ", " + IBD.CargarGuid("Comentario.IdentidadID") + ", Comentario.Fecha, Comentario.Descripcion, Comentario.Eliminado, " + IBD.CargarGuid("Comentario.ComentarioSuperiorID") + " ";

            #endregion

            this.sqlSelectComentario = "SELECT " + IBD.CargarGuid("Comentario.ComentarioID") + ", " + IBD.CargarGuid("Comentario.IdentidadID") + ", Comentario.Fecha, Comentario.Descripcion, Comentario.Eliminado, " + IBD.CargarGuid("Comentario.ComentarioSuperiorID") + " FROM Comentario";

            this.sqlSelectVotoComentario = "SELECT " + IBD.CargarGuid("VotoComentario.VotoID") + ", " + IBD.CargarGuid("VotoComentario.ComentarioID") + " FROM VotoComentario";

            this.sqlSelectComentariosDeEntradaBlog = selectComentarioSimple + ",Perfil.PersonaID, Perfil.OrganizacionID, Perfil.NombrePerfil NombreAutor, Perfil.NombreOrganizacion NombreOrganizacion, Identidad.Tipo TipoPerfil, Perfil.OrganizacionID OrganizacionPerfil, Identidad.ProyectoID ProyectoID FROM Comentario INNER JOIN Identidad ON Identidad.IdentidadID = Comentario.IdentidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN ComentarioBlog ON Comentario.ComentarioID = ComentarioBlog.ComentarioID WHERE Comentario.Eliminado=0 AND ComentarioBlog.BlogID = " + IBD.GuidParamValor("blogID") + " AND ComentarioBlog.EntradaBlogID = " + IBD.GuidParamValor("entradaBlogID") + " ORDER BY Comentario.Fecha DESC ";

            this.sqlSelectVotosComentariosDeEntradaBlog = sqlSelectVotoComentario + " INNER JOIN ComentarioBlog ON VotoComentario.ComentarioID = ComentarioBlog.ComentarioID INNER JOIN Comentario ON (VotoComentario.ComentarioID = Comentario.ComentarioID) WHERE Comentario.Eliminado=0 AND  ComentarioBlog.BlogID = " + IBD.GuidParamValor("blogID") + " AND ComentarioBlog.EntradaBlogID = " + IBD.GuidParamValor("entradaBlogID");
            this.sqlSelectComentariosDeBlogDePersona = sqlSelectComentario + " INNER JOIN ComentarioBlog ON Comentario.ComentarioID = ComentarioBlog.ComentarioID INNER JOIN Blog on Blog.BlogID = ComentarioBlog.BlogID WHERE Comentario.Eliminado=0 AND  Blog.AutorID = " + IBD.GuidParamValor("AutorID");
            this.sqlSelectComentariosDeBlog = sqlSelectComentario + " INNER JOIN ComentarioBlog ON Comentario.ComentarioID = ComentarioBlog.ComentarioID INNER JOIN Blog on Blog.BlogID = ComentarioBlog.BlogID WHERE Comentario.Eliminado=0 AND  Blog.BlogID = " + IBD.GuidParamValor("BlogID");
            this.sqlSelectVotosComentariosDeBlogDePersona = sqlSelectVotoComentario + " INNER JOIN ComentarioBlog ON VotoComentario.ComentarioID = ComentarioBlog.ComentarioID INNER JOIN Blog on Blog.BlogID = ComentarioBlog.BlogID INNER JOIN Comentario ON (VotoComentario.ComentarioID = Comentario.ComentarioID) WHERE Comentario.Eliminado=0 AND  Blog.AutorID = " + IBD.GuidParamValor("AutorID");

            this.sqlSelectComentariosDeDocumento = sqlSelectComentario + " INNER JOIN DocumentoComentario ON Comentario.ComentarioID = DocumentoComentario.ComentarioID WHERE Comentario.Eliminado=0 AND DocumentoComentario.DocumentoID = " + IBD.GuidParamValor("DocumentoID");

            this.sqlSelectComentariosDeDocumentoConFoto = selectComentarioSimple + ", Perfil.NombrePerfil NombreAutor, Perfil.NombreOrganizacion NombreOrganizacion, Identidad.Tipo TipoPerfil, Perfil.OrganizacionID OrganizacionPerfil, Perfil.PersonaID FROM Comentario INNER JOIN Identidad ON Identidad.IdentidadID = Comentario.IdentidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID  INNER JOIN DocumentoComentario ON Comentario.ComentarioID = DocumentoComentario.ComentarioID WHERE Comentario.Eliminado=0 AND DocumentoComentario.DocumentoID = " + IBD.GuidParamValor("DocumentoID") + " ORDER BY Fecha DESC";

            this.sqlSelectComentariosDeDocumentoConFotoSinProysPrivExcpActual = selectComentarioSimple + ", Perfil.NombrePerfil NombreAutor, Perfil.NombreOrganizacion NombreOrganizacion, Identidad.Tipo TipoPerfil, Perfil.OrganizacionID OrganizacionPerfil, Perfil.PersonaID FROM Comentario INNER JOIN Identidad ON Identidad.IdentidadID = Comentario.IdentidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID  INNER JOIN DocumentoComentario ON Comentario.ComentarioID = DocumentoComentario.ComentarioID INNER JOIN Proyecto ON DocumentoComentario.ProyectoID=Proyecto.ProyectoID WHERE Comentario.Eliminado=0 AND DocumentoComentario.DocumentoID = " + IBD.GuidParamValor("DocumentoID") + " AND ((Proyecto.TipoAcceso!=" + (short)TipoAcceso.Privado + " AND Proyecto.TipoAcceso!=" + (short)TipoAcceso.Reservado + ") OR  Proyecto.ProyectoID= " + IBD.GuidParamValor("ProyectoID") + ") ORDER BY Fecha DESC";

            this.sqlSelectVotosComentariosDeDocumento = sqlSelectVotoComentario + " INNER JOIN DocumentoComentario ON VotoComentario.ComentarioID = DocumentoComentario.ComentarioID INNER JOIN Comentario ON (VotoComentario.ComentarioID = Comentario.ComentarioID) WHERE Comentario.Eliminado=0 AND DocumentoComentario.DocumentoID = " + IBD.GuidParamValor("DocumentoID");

            this.sqlSelectVotosComentariosDeDocumentoSinProysPrivExcpActual = sqlSelectVotoComentario + " INNER JOIN DocumentoComentario ON VotoComentario.ComentarioID = DocumentoComentario.ComentarioID INNER JOIN Comentario ON (VotoComentario.ComentarioID = Comentario.ComentarioID) INNER JOIN Proyecto ON DocumentoComentario.ProyectoID=Proyecto.ProyectoID WHERE Comentario.Eliminado=0 AND DocumentoComentario.DocumentoID = " + IBD.GuidParamValor("DocumentoID") + " AND ((Proyecto.TipoAcceso!=" + (short)TipoAcceso.Privado + " AND Proyecto.TipoAcceso!=" + (short)TipoAcceso.Reservado + ") OR  Proyecto.ProyectoID= " + IBD.GuidParamValor("ProyectoID") + ") ";

            this.sqlSelectNumeroComentariosDeDocumento = "SELECT count (" + IBD.CargarGuid("DocumentoComentario.ComentarioID") + ") FROM DocumentoComentario WHERE DocumentoComentario.DocumentoID= " + IBD.GuidParamValor("DocumentoID");

            this.sqlSelectComentariosDeDocumentoConFotoRaiz = selectComentarioSimple + ", Perfil.NombrePerfil NombreAutor, Perfil.NombreOrganizacion NombreOrganizacion, Identidad.Tipo TipoPerfil, Perfil.OrganizacionID OrganizacionPerfil, Perfil.PersonaID FROM Comentario INNER JOIN Identidad ON Identidad.IdentidadID = Comentario.IdentidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID  INNER JOIN DocumentoComentario ON Comentario.ComentarioID = DocumentoComentario.ComentarioID WHERE Comentario.Eliminado=0 AND @WHEREDOCUMENTOID AND ComentarioSuperiorID is NULL ORDER BY Fecha DESC";

            this.sqlSelectComentariosDeDocumentoConFotoSinProysPrivExcpActualRaiz = selectComentarioSimple + ", Perfil.NombrePerfil NombreAutor, Perfil.NombreOrganizacion NombreOrganizacion, Identidad.Tipo TipoPerfil, Perfil.OrganizacionID OrganizacionPerfil, Perfil.PersonaID FROM Comentario INNER JOIN Identidad ON Identidad.IdentidadID = Comentario.IdentidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID  INNER JOIN DocumentoComentario ON Comentario.ComentarioID = DocumentoComentario.ComentarioID INNER JOIN Proyecto ON DocumentoComentario.ProyectoID=Proyecto.ProyectoID WHERE Comentario.Eliminado=0 AND @WHEREDOCUMENTOID AND ComentarioSuperiorID is NULL AND (@PROYPRIVADOS Proyecto.ProyectoID= " + IBD.GuidParamValor("ProyectoID") + ") ORDER BY Fecha DESC";

            this.sqlSelectComentariosDeDocumentoConFotoHijos = selectComentarioSimple + ", Perfil.NombrePerfil NombreAutor, Perfil.NombreOrganizacion NombreOrganizacion, Identidad.Tipo TipoPerfil, Perfil.OrganizacionID OrganizacionPerfil, Perfil.PersonaID FROM Comentario INNER JOIN Identidad ON Identidad.IdentidadID = Comentario.IdentidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID  INNER JOIN DocumentoComentario ON Comentario.ComentarioID = DocumentoComentario.ComentarioID WHERE Comentario.Eliminado=0 AND ComentarioSuperiorID IN (@ComentariosSuperioresID) ORDER BY Fecha DESC";

            this.sqlSelectComentariosDeDocumentoConFotoSinProysPrivExcpActualHijos = selectComentarioSimple + ", Perfil.NombrePerfil NombreAutor, Perfil.NombreOrganizacion NombreOrganizacion, Identidad.Tipo TipoPerfil, Perfil.OrganizacionID OrganizacionPerfil, Perfil.PersonaID FROM Comentario INNER JOIN Identidad ON Identidad.IdentidadID = Comentario.IdentidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID  INNER JOIN DocumentoComentario ON Comentario.ComentarioID = DocumentoComentario.ComentarioID INNER JOIN Proyecto ON DocumentoComentario.ProyectoID=Proyecto.ProyectoID WHERE Comentario.Eliminado=0 AND ComentarioSuperiorID IN (@ComentariosSuperioresID) AND ((Proyecto.TipoAcceso!=" + (short)TipoAcceso.Privado + " AND Proyecto.TipoAcceso!=" + (short)TipoAcceso.Reservado + ") OR  Proyecto.ProyectoID= " + IBD.GuidParamValor("ProyectoID") + ") ORDER BY Fecha DESC";

            #endregion

            #region DataAdapter

            #region Comentario
            this.sqlComentarioInsert = IBD.ReplaceParam("INSERT INTO Comentario (ComentarioID, IdentidadID, Fecha, Descripcion, Eliminado, ComentarioSuperiorID) VALUES (" + IBD.GuidParamColumnaTabla("ComentarioID") + ", " + IBD.GuidParamColumnaTabla("IdentidadID") + ", @Fecha, @Descripcion, @Eliminado, " + IBD.GuidParamColumnaTabla("ComentarioSuperiorID") + ")");
            this.sqlComentarioDelete = IBD.ReplaceParam("DELETE FROM Comentario WHERE (ComentarioID = " + IBD.GuidParamColumnaTabla("O_ComentarioID") + ") AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ") AND (Fecha = @O_Fecha) AND " + IBD.ComparacionCamposTextoPesadoConOriginal("Descripcion", false) + " AND (Eliminado = @O_Eliminado) AND (ComentarioSuperiorID = " + IBD.GuidParamColumnaTabla("O_ComentarioSuperiorID") + " OR " + IBD.GuidParamColumnaTabla("O_ComentarioSuperiorID") + " IS NULL AND ComentarioSuperiorID IS NULL)");
            this.sqlComentarioModify = IBD.ReplaceParam("UPDATE Comentario SET ComentarioID = " + IBD.GuidParamColumnaTabla("ComentarioID") + ", IdentidadID = " + IBD.GuidParamColumnaTabla("IdentidadID") + ", Fecha = @Fecha, Descripcion = @Descripcion, Eliminado = @Eliminado, ComentarioSuperiorID = " + IBD.GuidParamColumnaTabla("ComentarioSuperiorID") + " WHERE (ComentarioID = " + IBD.GuidParamColumnaTabla("O_ComentarioID") + ") AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ") AND (Fecha = @O_Fecha) AND " + IBD.ComparacionCamposTextoPesadoConOriginal("Descripcion", false) + " AND (Eliminado = @O_Eliminado) AND (ComentarioSuperiorID = " + IBD.GuidParamColumnaTabla("O_ComentarioSuperiorID") + " OR " + IBD.GuidParamColumnaTabla("O_ComentarioSuperiorID") + " IS NULL AND ComentarioSuperiorID IS NULL)");
            #endregion

            #region VotoComentario
            this.sqlVotoComentarioInsert = IBD.ReplaceParam("INSERT INTO VotoComentario (VotoID, ComentarioID) VALUES (" + IBD.GuidParamColumnaTabla("VotoID") + ", " + IBD.GuidParamColumnaTabla("ComentarioID") + ")");
            this.sqlVotoComentarioDelete = IBD.ReplaceParam("DELETE FROM VotoComentario WHERE (VotoID = " + IBD.GuidParamColumnaTabla("O_VotoID") + ") AND (ComentarioID = " + IBD.GuidParamColumnaTabla("O_ComentarioID") + ")");
            this.sqlVotoComentarioModify = IBD.ReplaceParam("UPDATE VotoComentario SET VotoID = " + IBD.GuidParamColumnaTabla("VotoID") + ", ComentarioID = " + IBD.GuidParamColumnaTabla("ComentarioID") + " WHERE (VotoID = " + IBD.GuidParamColumnaTabla("O_VotoID") + ") AND (ComentarioID = " + IBD.GuidParamColumnaTabla("O_ComentarioID") + ")");
            #endregion

            #endregion
        }

        #endregion
    }
}
