using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Blog;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.MVC;
using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.PersonaDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Es.Riam.Gnoss.AD.MVC
{
    #region Enumeraciones


    #endregion

    public class ProyectoProyectoAgCatTesauro
    {
        public Proyecto Proyecto { get; set; }
        public ProyectoAgCatTesauro ProyectoAgCatTesauro { get; set; }
    }

    public class ProyectoProyectoAgCatTesauroCategoriaTesauro
    {
        public Proyecto Proyecto { get; set; }
        public ProyectoAgCatTesauro ProyectoAgCatTesauro { get; set; }
        public CategoriaTesauro CategoriaTesauro { get; set; }
    }

    public class ProyectoParametroGeneral
    {
        public Proyecto Proyecto { get; set; }
        public ParametroGeneral ParametroGeneral { get; set; }
    }

    public class VotoDocumentoVoto
    {
        public EntityModel.Models.Voto.Voto Voto { get; set; }
        public VotoDocumento VotoDocumento { get; set; }
    }

    public class DocumentoWebVinBaseRecursosNivelCertificacion
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public NivelCertificacion NivelCertificacion { get; set; }
    }

    public class DocumentoWebVinBaseRecursosNivelCertificacionBaseRecursosProyecto
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public NivelCertificacion NivelCertificacion { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
    }

    public class ComentarioDocumentoComentario
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
    }

    public class DocumentoDocumentoWebVinBaseRecursos
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }

    public class DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyecto
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
    }

    public class DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauro
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
    }

    public class DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauroCategoriaTesauro
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
        public CategoriaTesauro CategoriaTesauro { get; set; }
    }


    public class DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauroCategoriaTesauroCatTesauroAgCatTesauro
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
        public CategoriaTesauro CategoriaTesauro { get; set; }
        public CatTesauroAgCatTesauro CatTesauroAgCatTesauro { get; set; }
    }

    public class DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauroCategoriaTesauroCatTesauroAgCatTesauroTesauroProyecto
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
        public CategoriaTesauro CategoriaTesauro { get; set; }
        public CatTesauroAgCatTesauro CatTesauroAgCatTesauro { get; set; }
        public TesauroProyecto TesauroProyecto { get; set; }
    }


    public class DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauroCategoriaTesauroCatTesauroAgCatTesauroTesauroProyectoCatTesauroCompartida
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
        public CategoriaTesauro CategoriaTesauro { get; set; }
        public CatTesauroAgCatTesauro CatTesauroAgCatTesauro { get; set; }
        public TesauroProyecto TesauroProyecto { get; set; }
        public CatTesauroCompartida CatTesauroCompartida { get; set; }
    }

    public class DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauroCategoriaTesauroCatTesauroAgCatTesauroCatTesauroCompartida
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
        public CategoriaTesauro CategoriaTesauro { get; set; }
        public CatTesauroAgCatTesauro CatTesauroAgCatTesauro { get; set; }
        public CatTesauroCompartida CatTesauroCompartida { get; set; }
    }

    public class DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauroCategoriaTesauroCatTesauroAgCatTesauroCatTesauroCompartidaTesauroProyecto
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
        public CategoriaTesauro CategoriaTesauro { get; set; }
        public CatTesauroAgCatTesauro CatTesauroAgCatTesauro { get; set; }
        public CatTesauroCompartida CatTesauroCompartida { get; set; }
        public TesauroProyecto TesauroProyecto { get; set; }
    }

    public class CategoriaTesauroTesauroProyecto
    {
        public CategoriaTesauro CategoriaTesauro { get; set; }
        public TesauroProyecto TesauroProyecto { get; set; }
    }

    public class CatTesauroCompartidaTesauroProyecto
    {
        public CatTesauroCompartida CatTesauroCompartida { get; set; }
        public TesauroProyecto TesauroProyecto { get; set; }
    }

    public class DocumentoDocumentoWebVinBaseRecursosBaseRecursos
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursos BaseRecursos { get; set; }
    }

    public class DocumentoDocumentoWebVinBaseRecursosBaseRecursosDocumentoWebAgCatTesauro
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursos BaseRecursos { get; set; }
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
    }

    public class DocumentoDocumentoWebVinBaseRecursosBaseRecursosDocumentoWebAgCatTesauroCategoriaTesauro
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursos BaseRecursos { get; set; }
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
        public CategoriaTesauro CategoriaTesauro { get; set; }
    }

    public class DocumentoDocumentoWebVinBaseRecursosBaseRecursosDocumentoWebAgCatTesauroCategoriaTesauroCatTesauroAgCatTesauro
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursos BaseRecursos { get; set; }
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
        public CategoriaTesauro CategoriaTesauro { get; set; }
        public CatTesauroAgCatTesauro CatTesauroAgCatTesauro { get; set; }
    }

    public class CategoriaTesauroTesauroUsuario
    {
        public CategoriaTesauro CategoriaTesauro { get; set; }
        public TesauroUsuario TesauroUsuario { get; set; }
    }

    public class CategoriaTesauroTesauroUsuarioBaseRecursosUsuario
    {
        public CategoriaTesauro CategoriaTesauro { get; set; }
        public TesauroUsuario TesauroUsuario { get; set; }
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
    }

    public class CategoriaTesauroTesauroOrganizacion
    {
        public CategoriaTesauro CategoriaTesauro { get; set; }
        public TesauroOrganizacion TesauroOrganizacion { get; set; }
    }

    public class CategoriaTesauroTesauroOrganizacionBaseRecursosOrganizacion
    {
        public CategoriaTesauro CategoriaTesauro { get; set; }
        public TesauroOrganizacion TesauroOrganizacion { get; set; }
        public BaseRecursosOrganizacion BaseRecursosOrganizacion { get; set; }
    }

    public class IdentidadDatoExtraProyectoOpcionIdentidad
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DatoExtraProyectoOpcionIdentidad DatoExtraProyectoOpcionIdentidad { get; set; }
    }

    public class IdentidadDatoExtraProyectoOpcionIdentidadDatoExtraProyectoOpcion
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DatoExtraProyectoOpcionIdentidad DatoExtraProyectoOpcionIdentidad { get; set; }
        public DatoExtraProyectoOpcion DatoExtraProyectoOpcion { get; set; }
    }

    public class IdentidadDatoExtraProyectoOpcionIdentidadDatoExtraProyectoOpcionDatoExtraProyecto
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DatoExtraProyectoOpcionIdentidad DatoExtraProyectoOpcionIdentidad { get; set; }
        public DatoExtraProyectoOpcion DatoExtraProyectoOpcion { get; set; }
        public DatoExtraProyecto DatoExtraProyecto { get; set; }
    }

    public class IdentidadDatoExtraProyectoVirtuosoIdentidad
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DatoExtraProyectoVirtuosoIdentidad DatoExtraProyectoVirtuosoIdentidad { get; set; }
    }

    public class IdentidadDatoExtraProyectoVirtuosoIdentidadDatoExtraProyectoVirtuoso
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DatoExtraProyectoVirtuosoIdentidad DatoExtraProyectoVirtuosoIdentidad { get; set; }
        public DatoExtraProyectoVirtuoso DatoExtraProyectoVirtuoso { get; set; }
    }

    public class IdentidadDatoExtraEcosistemaOpcionPerfil
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DatoExtraEcosistemaOpcionPerfil DatoExtraEcosistemaOpcionPerfil { get; set; }
    }

    public class IdentidadDatoExtraEcosistemaOpcionPerfilDatoExtraEcosistemaOpcion
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DatoExtraEcosistemaOpcionPerfil DatoExtraEcosistemaOpcionPerfil { get; set; }
        public DatoExtraEcosistemaOpcion DatoExtraEcosistemaOpcion { get; set; }
    }

    public class IdentidadDatoExtraEcosistemaOpcionPerfilDatoExtraEcosistemaOpcionDatoExtraEcosistema
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DatoExtraEcosistemaOpcionPerfil DatoExtraEcosistemaOpcionPerfil { get; set; }
        public DatoExtraEcosistemaOpcion DatoExtraEcosistemaOpcion { get; set; }
        public DatoExtraEcosistema DatoExtraEcosistema { get; set; }
    }

    public class IdentidadDatoExtraEcosistemaVirtuosoPerfil
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DatoExtraEcosistemaVirtuosoPerfil DatoExtraEcosistemaVirtuosoPerfil { get; set; }
    }

    public class IdentidadDatoExtraEcosistemaVirtuosoPerfilDatoExtraEcosistemaVirtuoso
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DatoExtraEcosistemaVirtuosoPerfil DatoExtraEcosistemaVirtuosoPerfil { get; set; }
        public DatoExtraEcosistemaVirtuoso DatoExtraEcosistemaVirtuoso { get; set; }
    }

    public class GrupoIdentGrupoIdentidadesProyectoProyecto
    {
        public GrupoIdentidades GrupoIdentidades { get; set; }
        public GrupoIdentidadesProyecto GrupoIdentidadesProyecto { get; set; }
        public Proyecto Proyecto { get; set; }
    }

    public class ComentarioDocumentoComentarioDocumento
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
        public Documento Documento { get; set; }
    }

    public class ComentarioDocumentoComentarioDocumentoProyecto
    {
        public EntityModel.Models.Comentario.Comentario Comentario { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
        public Documento Documento { get; set; }
        public Proyecto Proyecto { get; set; }
    }

    public class IdentidadPerfil
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class IdentidadPerfilPersona
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }

        public Persona Persona { get; set; }
    }

    public class IdentidadPerfilOrganizacion
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public Organizacion Organizacion { get; set; }
    }

    public class AmigoAgGrupoGrupoAmigos
    {
        public AmigoAgGrupo AmigoAgGrupo { get; set; }
        public GrupoAmigos GrupoAmigos { get; set; }
    }

    public class IdentidadPerfilAmigoAgGrupo
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public AmigoAgGrupo AmigoAgGrupo { get; set; }
    }

    // "FROM AmigoAgGrupo INNER JOIN Identidad ON AmigoAgGrupo.IdentidadAmigoID = Identidad.IdentidadID 

    public static class JoinMVC
    {
        public static IQueryable<IdentidadPerfilAmigoAgGrupo> JoinAmigoAgGrupo(this IQueryable<IdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.AmigoAgGrupo, item => item.Identidad.IdentidadID, amigoAgGrupo => amigoAgGrupo.IdentidadAmigoID, (item, amigoAgGrupo) => new IdentidadPerfilAmigoAgGrupo
            {
                AmigoAgGrupo = amigoAgGrupo,
                Identidad = item.Identidad,
                Perfil = item.Perfil
            });
        }

        public static IQueryable<AmigoAgGrupoGrupoAmigos> JoinGrupoAmigos(this IQueryable<AmigoAgGrupo> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.GrupoAmigos, amigoAgGrupo => amigoAgGrupo.GrupoID, grupoAmigos => grupoAmigos.GrupoID, (amigoAgGrupo, grupoAmigos) => new AmigoAgGrupoGrupoAmigos
            {
                AmigoAgGrupo = amigoAgGrupo,
                GrupoAmigos = grupoAmigos
            });
        }

        public static IQueryable<IdentidadPerfilOrganizacion> JoinOrganizacion(this IQueryable<IdentidadPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Organizacion, item => item.Perfil.OrganizacionID, organizacion => organizacion.OrganizacionID, (item, organizacion) => new IdentidadPerfilOrganizacion
            {
                Identidad = item.Identidad,
                Organizacion = organizacion,
                Perfil = item.Perfil
            });
        }

        public static IQueryable<IdentidadPerfil> JoinPerfil(this IQueryable<EntityModel.Models.IdentidadDS.Identidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, identidad => identidad.PerfilID, perfil => perfil.PerfilID, (identidad, perfil) => new IdentidadPerfil
            {
                Identidad = identidad,
                Perfil = perfil
            });
        }

        public static IQueryable<IdentidadPerfilPersona> LeftJoinPersona(this IQueryable<IdentidadPerfil> pQuery)
        {
            /*
            return pQuery.Join(EntityContext.Persona, identidadPerfil => identidadPerfil.Perfil.PersonaID, persona => persona.PersonaID, (identidadPerfil, persona) => new IdentidadPerfilPersona
            {
                Identidad = identidadPerfil.Identidad,
                Perfil = identidadPerfil.Perfil,
                Persona = persona
            });*/
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.GroupJoin(entityContext.Persona, item => new { PersonaID = item.Perfil.PersonaID.Value }, persona => new { PersonaID = persona.PersonaID }, (item, persona) => new
            {
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Persona = persona
            }).SelectMany(item => item.Persona.DefaultIfEmpty(), (item, agrup) => new IdentidadPerfilPersona
            {
                Identidad = item.Identidad,
                Perfil = item.Perfil,
                Persona = agrup
            });
        }

        public static IQueryable<ComentarioDocumentoComentarioDocumentoProyecto> JoinProyecto(this IQueryable<ComentarioDocumentoComentarioDocumento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Proyecto, item => item.DocumentoComentario.ProyectoID, proyecto => proyecto.ProyectoID, (item, proyecto) => new ComentarioDocumentoComentarioDocumentoProyecto
            {
                Comentario = item.Comentario,
                Proyecto = proyecto,
                Documento = item.Documento,
                DocumentoComentario = item.DocumentoComentario
            });
        }

        public static IQueryable<ComentarioDocumentoComentarioDocumento> JoinDocumento(this IQueryable<ComentarioDocumentoComentario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, item => item.DocumentoComentario.DocumentoID, documento => documento.DocumentoID, (item, documento) => new ComentarioDocumentoComentarioDocumento
            {
                Comentario = item.Comentario,
                Documento = documento,
                DocumentoComentario = item.DocumentoComentario
            });
        }

        public static IQueryable<GrupoIdentGrupoIdentidadesProyectoProyecto> JoinProyecto(this IQueryable<JoinGrupoIdentGrupoIdentidadesProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Proyecto, item => item.GrupoIdentidadesProyecto.ProyectoID, proyecto => proyecto.ProyectoID, (item, proyecto) => new GrupoIdentGrupoIdentidadesProyectoProyecto
            {
                Proyecto = proyecto,
                GrupoIdentidades = item.GrupoIdentidades,
                GrupoIdentidadesProyecto = item.GrupoIdentidadesProyecto
            });
        }

        public static IQueryable<IdentidadDatoExtraEcosistemaVirtuosoPerfilDatoExtraEcosistemaVirtuoso> JoinDatoExtraEcosistemaVirtuoso(this IQueryable<IdentidadDatoExtraEcosistemaVirtuosoPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DatoExtraEcosistemaVirtuoso, item => item.DatoExtraEcosistemaVirtuosoPerfil.DatoExtraID, datoExtraEcosistemaVirtuoso => datoExtraEcosistemaVirtuoso.DatoExtraID, (item, datoExtraEcosistemaVirtuoso) => new IdentidadDatoExtraEcosistemaVirtuosoPerfilDatoExtraEcosistemaVirtuoso
            {
                DatoExtraEcosistemaVirtuoso = datoExtraEcosistemaVirtuoso,
                DatoExtraEcosistemaVirtuosoPerfil = item.DatoExtraEcosistemaVirtuosoPerfil,
                Identidad = item.Identidad
            });
        }

        public static IQueryable<IdentidadDatoExtraEcosistemaVirtuosoPerfil> JoinDatoExtraEcosistemaVirtuosoPerfil(this IQueryable<EntityModel.Models.IdentidadDS.Identidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DatoExtraEcosistemaVirtuosoPerfil, identidad => identidad.PerfilID, datoExtraEcosistemaVirtuosoPerfil => datoExtraEcosistemaVirtuosoPerfil.PerfilID, (identidad, datoExtraEcosistemaVirtuosoPerfil) => new IdentidadDatoExtraEcosistemaVirtuosoPerfil
            {
                DatoExtraEcosistemaVirtuosoPerfil = datoExtraEcosistemaVirtuosoPerfil,
                Identidad = identidad
            });
        }

        public static IQueryable<IdentidadDatoExtraEcosistemaOpcionPerfilDatoExtraEcosistemaOpcionDatoExtraEcosistema> JoinDatoExtraEcosistema(this IQueryable<IdentidadDatoExtraEcosistemaOpcionPerfilDatoExtraEcosistemaOpcion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DatoExtraEcosistema, item => item.DatoExtraEcosistemaOpcion.DatoExtraID, datoExtraEcosistema => datoExtraEcosistema.DatoExtraID, (item, datoExtraEcosistema) => new IdentidadDatoExtraEcosistemaOpcionPerfilDatoExtraEcosistemaOpcionDatoExtraEcosistema
            {
                DatoExtraEcosistema = datoExtraEcosistema,
                Identidad = item.Identidad,
                DatoExtraEcosistemaOpcion = item.DatoExtraEcosistemaOpcion,
                DatoExtraEcosistemaOpcionPerfil = item.DatoExtraEcosistemaOpcionPerfil
            });
        }

        public static IQueryable<IdentidadDatoExtraEcosistemaOpcionPerfilDatoExtraEcosistemaOpcion> JoinDatoExtraEcosistemaOpcion(this IQueryable<IdentidadDatoExtraEcosistemaOpcionPerfil> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DatoExtraEcosistemaOpcion, item => item.DatoExtraEcosistemaOpcionPerfil.OpcionID, datoExtraEcosistemaOpcion => datoExtraEcosistemaOpcion.OpcionID, (item, datoExtraEcosistemaOpcion) => new IdentidadDatoExtraEcosistemaOpcionPerfilDatoExtraEcosistemaOpcion
            {
                DatoExtraEcosistemaOpcion = datoExtraEcosistemaOpcion,
                DatoExtraEcosistemaOpcionPerfil = item.DatoExtraEcosistemaOpcionPerfil,
                Identidad = item.Identidad
            });
        }

        public static IQueryable<IdentidadDatoExtraEcosistemaOpcionPerfil> JoinDatoExtraEcosistemaOpcionPerfil(this IQueryable<EntityModel.Models.IdentidadDS.Identidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DatoExtraEcosistemaOpcionPerfil, identidad => identidad.PerfilID, datoExtraEcosistemaOpcionPerfil => datoExtraEcosistemaOpcionPerfil.PerfilID, (identidad, datoExtraEcosistemaOpcionPerfil) => new IdentidadDatoExtraEcosistemaOpcionPerfil
            {
                DatoExtraEcosistemaOpcionPerfil = datoExtraEcosistemaOpcionPerfil,
                Identidad = identidad
            });
        }

        public static IQueryable<IdentidadDatoExtraProyectoVirtuosoIdentidadDatoExtraProyectoVirtuoso> JoinDatoExtraProyectoVirtuoso(this IQueryable<IdentidadDatoExtraProyectoVirtuosoIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DatoExtraProyectoVirtuoso, item => new { item.DatoExtraProyectoVirtuosoIdentidad.DatoExtraID, item.DatoExtraProyectoVirtuosoIdentidad.OrganizacionID, item.DatoExtraProyectoVirtuosoIdentidad.ProyectoID }, datoExtraProyectoVirtuoso => new { datoExtraProyectoVirtuoso.DatoExtraID, datoExtraProyectoVirtuoso.OrganizacionID, datoExtraProyectoVirtuoso.ProyectoID }, (item, datoExtraProyectoVirtuoso) => new IdentidadDatoExtraProyectoVirtuosoIdentidadDatoExtraProyectoVirtuoso
            {
                DatoExtraProyectoVirtuoso = datoExtraProyectoVirtuoso,
                DatoExtraProyectoVirtuosoIdentidad = item.DatoExtraProyectoVirtuosoIdentidad,
                Identidad = item.Identidad
            });
        }

        public static IQueryable<IdentidadDatoExtraProyectoVirtuosoIdentidad> JoinDatoExtraVirtuosoIdentidad(this IQueryable<EntityModel.Models.IdentidadDS.Identidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DatoExtraProyectoVirtuosoIdentidad, identidad => identidad.IdentidadID, datoExtraProyectoVirtuosoIdentidad => datoExtraProyectoVirtuosoIdentidad.IdentidadID, (identidad, datoExtraProyectoVirtuosoIdentidad) => new IdentidadDatoExtraProyectoVirtuosoIdentidad
            {
                DatoExtraProyectoVirtuosoIdentidad = datoExtraProyectoVirtuosoIdentidad,
                Identidad = identidad
            });
        }

        public static IQueryable<IdentidadDatoExtraProyectoOpcionIdentidadDatoExtraProyectoOpcionDatoExtraProyecto> JoinDatoExtraProyecto(this IQueryable<IdentidadDatoExtraProyectoOpcionIdentidadDatoExtraProyectoOpcion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DatoExtraProyecto, item => new { item.DatoExtraProyectoOpcion.DatoExtraID, item.DatoExtraProyectoOpcion.OrganizacionID, item.DatoExtraProyectoOpcion.ProyectoID }, datoExtraProyecto => new { datoExtraProyecto.DatoExtraID, datoExtraProyecto.OrganizacionID, datoExtraProyecto.ProyectoID }, (item, datoExtraProyecto) => new IdentidadDatoExtraProyectoOpcionIdentidadDatoExtraProyectoOpcionDatoExtraProyecto
            {
                DatoExtraProyecto = datoExtraProyecto,
                Identidad = item.Identidad,
                DatoExtraProyectoOpcion = item.DatoExtraProyectoOpcion,
                DatoExtraProyectoOpcionIdentidad = item.DatoExtraProyectoOpcionIdentidad
            });
        }

        public static IQueryable<IdentidadDatoExtraProyectoOpcionIdentidadDatoExtraProyectoOpcion> JoinDatoExtraProyectoOpcion(this IQueryable<IdentidadDatoExtraProyectoOpcionIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DatoExtraProyectoOpcion, item => item.DatoExtraProyectoOpcionIdentidad.OpcionID, datoExtraProyectoOpcion => datoExtraProyectoOpcion.OpcionID, (item, datoExtraProyectoOpcion) => new IdentidadDatoExtraProyectoOpcionIdentidadDatoExtraProyectoOpcion
            {
                DatoExtraProyectoOpcion = datoExtraProyectoOpcion,
                DatoExtraProyectoOpcionIdentidad = item.DatoExtraProyectoOpcionIdentidad,
                Identidad = item.Identidad
            });
        }

        public static IQueryable<IdentidadDatoExtraProyectoOpcionIdentidad> JoinDatoExtraProyectoOpcionIdentidad(this IQueryable<EntityModel.Models.IdentidadDS.Identidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DatoExtraProyectoOpcionIdentidad, identidad => identidad.IdentidadID, datoExtraProyectoOpcionIdentidad => datoExtraProyectoOpcionIdentidad.IdentidadID, (identidad, datoExtraProyectoOpcionIdentidad) => new IdentidadDatoExtraProyectoOpcionIdentidad
            {
                DatoExtraProyectoOpcionIdentidad = datoExtraProyectoOpcionIdentidad,
                Identidad = identidad
            });
        }

        public static IQueryable<CategoriaTesauroTesauroOrganizacionBaseRecursosOrganizacion> JoinBaseRercursosOrganizacion(this IQueryable<CategoriaTesauroTesauroOrganizacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.BaseRecursosOrganizacion, item => item.TesauroOrganizacion.OrganizacionID, baseRecursosOrganizacion => baseRecursosOrganizacion.OrganizacionID, (item, baseRecursosOrganizacion) => new CategoriaTesauroTesauroOrganizacionBaseRecursosOrganizacion
            {
                BaseRecursosOrganizacion = baseRecursosOrganizacion,
                CategoriaTesauro = item.CategoriaTesauro,
                TesauroOrganizacion = item.TesauroOrganizacion
            });
        }

        public static IQueryable<CategoriaTesauroTesauroOrganizacion> JoinTesauroOrganizacion(this IQueryable<CategoriaTesauro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.TesauroOrganizacion, categoriaUsuario => categoriaUsuario.TesauroID, tesauroOrganizacion => tesauroOrganizacion.TesauroID, (categoriaTesauro, tesauroOrganizacion) => new CategoriaTesauroTesauroOrganizacion
            {
                TesauroOrganizacion = tesauroOrganizacion,
                CategoriaTesauro = categoriaTesauro
            });
        }

        public static IQueryable<CategoriaTesauroTesauroUsuarioBaseRecursosUsuario> JoinBaseRecursosUsuario(this IQueryable<CategoriaTesauroTesauroUsuario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.BaseRecursosUsuario, item => item.TesauroUsuario.UsuarioID, baseRecursosUsuario => baseRecursosUsuario.UsuarioID, (item, baseRecursosUsuario) => new CategoriaTesauroTesauroUsuarioBaseRecursosUsuario
            {
                BaseRecursosUsuario = baseRecursosUsuario,
                CategoriaTesauro = item.CategoriaTesauro,
                TesauroUsuario = item.TesauroUsuario
            });
        }

        public static IQueryable<CategoriaTesauroTesauroUsuario> JoinTesauroUsuario(this IQueryable<CategoriaTesauro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.TesauroUsuario, categoriaUsuario => categoriaUsuario.TesauroID, tesauroUsuario => tesauroUsuario.TesauroID, (categoriaTesauro, tesauroUsuario) => new CategoriaTesauroTesauroUsuario
            {
                TesauroUsuario = tesauroUsuario,
                CategoriaTesauro = categoriaTesauro
            });
        }

        public static IQueryable<DocumentoDocumentoWebVinBaseRecursosBaseRecursosDocumentoWebAgCatTesauroCategoriaTesauroCatTesauroAgCatTesauro> JoinCatTesauroAgCatTesauro(this IQueryable<DocumentoDocumentoWebVinBaseRecursosBaseRecursosDocumentoWebAgCatTesauroCategoriaTesauro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.GroupJoin(entityContext.CatTesauroAgCatTesauro, item => item.CategoriaTesauro.CategoriaTesauroID, catTesauroAgCatTesauro => catTesauroAgCatTesauro.CategoriaInferiorID, (item, catTesauroAgCatTesauro) => new
            {
                CatTesauroAgCatTesauro = catTesauroAgCatTesauro,
                BaseRecursos = item.BaseRecursos,
                CategoriaTesauro = item.CategoriaTesauro,
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                DocumentoWebAgCatTesauro = item.DocumentoWebAgCatTesauro
            }).SelectMany(item => item.CatTesauroAgCatTesauro.DefaultIfEmpty(), (item, item2) => new DocumentoDocumentoWebVinBaseRecursosBaseRecursosDocumentoWebAgCatTesauroCategoriaTesauroCatTesauroAgCatTesauro
            {
                BaseRecursos = item.BaseRecursos,
                CatTesauroAgCatTesauro = item2,
                Documento = item.Documento,
                DocumentoWebAgCatTesauro = item.DocumentoWebAgCatTesauro,
                CategoriaTesauro = item.CategoriaTesauro,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos
            });
        }

        public static IQueryable<DocumentoDocumentoWebVinBaseRecursosBaseRecursosDocumentoWebAgCatTesauroCategoriaTesauro> JoinCategoriaTesauro(this IQueryable<DocumentoDocumentoWebVinBaseRecursosBaseRecursosDocumentoWebAgCatTesauro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CategoriaTesauro, item => item.DocumentoWebAgCatTesauro.CategoriaTesauroID, categoriaTesauro => categoriaTesauro.CategoriaTesauroID, (item, categoriaTesauro) => new DocumentoDocumentoWebVinBaseRecursosBaseRecursosDocumentoWebAgCatTesauroCategoriaTesauro
            {
                BaseRecursos = item.BaseRecursos,
                CategoriaTesauro = categoriaTesauro,
                Documento = item.Documento,
                DocumentoWebAgCatTesauro = item.DocumentoWebAgCatTesauro,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos
            });
        }

        public static IQueryable<DocumentoDocumentoWebVinBaseRecursosBaseRecursosDocumentoWebAgCatTesauro> JoinDocumentoWebAgCatTesauro(this IQueryable<DocumentoDocumentoWebVinBaseRecursosBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoWebAgCatTesauro, item => new { item.DocumentoWebVinBaseRecursos.BaseRecursosID, item.Documento.DocumentoID }, documentoWebVinBaseRecursos => new { documentoWebVinBaseRecursos.BaseRecursosID, documentoWebVinBaseRecursos.DocumentoID }, (item, documentoWebVinBaseRecursos) => new DocumentoDocumentoWebVinBaseRecursosBaseRecursosDocumentoWebAgCatTesauro
            {
                BaseRecursos = item.BaseRecursos,
                Documento = item.Documento,
                DocumentoWebAgCatTesauro = documentoWebVinBaseRecursos,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos
            });
        }

        public static IQueryable<DocumentoDocumentoWebVinBaseRecursosBaseRecursos> JoinBaseRecursos(this IQueryable<DocumentoDocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.BaseRecursos, item => item.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecursos => baseRecursos.BaseRecursosID, (item, baseRecursos) => new DocumentoDocumentoWebVinBaseRecursosBaseRecursos
            {
                BaseRecursos = baseRecursos,
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos
            });
        }

        public static IQueryable<CatTesauroCompartidaTesauroProyecto> JoinTesauroProyecto(this IQueryable<CatTesauroCompartida> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.TesauroProyecto, catTesauroCompartida => catTesauroCompartida.TesauroDestinoID, tesauroProyecto => tesauroProyecto.TesauroID, (catTesauroCompartida, tesauroProyecto) => new CatTesauroCompartidaTesauroProyecto
            {
                CatTesauroCompartida = catTesauroCompartida,
                TesauroProyecto = tesauroProyecto
            });
        }

        public static IQueryable<CategoriaTesauroTesauroProyecto> JoinTesauroProyecto(this IQueryable<CategoriaTesauro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.TesauroProyecto, categoriaTesauro => categoriaTesauro.TesauroID, tesauroProyecto => tesauroProyecto.TesauroID, (categoriaTesauro, tesauroProyecto) => new CategoriaTesauroTesauroProyecto
            {
                TesauroProyecto = tesauroProyecto,
                CategoriaTesauro = categoriaTesauro
            });
        }
        public static IQueryable<DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauroCategoriaTesauroCatTesauroAgCatTesauro> JoinCatTesauroAgCatTesauro(this IQueryable<DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauroCategoriaTesauro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.GroupJoin(entityContext.CatTesauroAgCatTesauro, item => new { CategoriaTesauroID = item.CategoriaTesauro.CategoriaTesauroID }, catTesauroAgCatTesauro => new
            { CategoriaTesauroID = catTesauroAgCatTesauro.CategoriaInferiorID }, (item, catTesauroAgCatTesauro) => new
            {
                CatTesauroAgCatTesauro = catTesauroAgCatTesauro,
                BaseRecursosProyecto = item.BaseRecursosProyecto,
                CategoriaTesauro = item.CategoriaTesauro,
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                DocumentoWebAgCatTesauro = item.DocumentoWebAgCatTesauro
            }).SelectMany(item => item.CatTesauroAgCatTesauro.DefaultIfEmpty(), (item, item2) => new DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauroCategoriaTesauroCatTesauroAgCatTesauro
            {
                BaseRecursosProyecto = item.BaseRecursosProyecto,
                CatTesauroAgCatTesauro = item2,
                Documento = item.Documento,
                DocumentoWebAgCatTesauro = item.DocumentoWebAgCatTesauro,
                CategoriaTesauro = item.CategoriaTesauro,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,


            });
        }

        public static IQueryable<DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauroCategoriaTesauroCatTesauroAgCatTesauro> LeftJoinCatTesauroAgCatTesauroSegundoSelect(this IQueryable<DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauroCategoriaTesauro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.GroupJoin(entityContext.CatTesauroAgCatTesauro, item => new { CategoriaTesauroID = item.CategoriaTesauro.CategoriaTesauroID }, catTesauroAgCatTesauro => new
            { CategoriaTesauroID = catTesauroAgCatTesauro.CategoriaInferiorID }, (item, catTesauroAgCatTesauro) => new
            {
                CatTesauroAgCatTesauro = catTesauroAgCatTesauro,
                BaseRecursosProyecto = item.BaseRecursosProyecto,
                CategoriaTesauro = item.CategoriaTesauro,
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                DocumentoWebAgCatTesauro = item.DocumentoWebAgCatTesauro
            }).SelectMany(item => item.CatTesauroAgCatTesauro.DefaultIfEmpty(), (item, item2) => new DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauroCategoriaTesauroCatTesauroAgCatTesauro
            {
                BaseRecursosProyecto = item.BaseRecursosProyecto,
                CatTesauroAgCatTesauro = item2,
                Documento = item.Documento,
                DocumentoWebAgCatTesauro = item.DocumentoWebAgCatTesauro,
                CategoriaTesauro = item.CategoriaTesauro,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,


            });
        }

        public static IQueryable<DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauroCategoriaTesauroCatTesauroAgCatTesauroTesauroProyecto> JoinTesauroProyectoPrimerSelect(this IQueryable<DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauroCategoriaTesauroCatTesauroAgCatTesauro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.TesauroProyecto, item => item.CategoriaTesauro.TesauroID, tesauroProyecto => tesauroProyecto.TesauroID, (item, tesauroProyecto) => new DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauroCategoriaTesauroCatTesauroAgCatTesauroTesauroProyecto
            {
                CategoriaTesauro = item.CategoriaTesauro,
                BaseRecursosProyecto = item.BaseRecursosProyecto,
                Documento = item.Documento,
                DocumentoWebAgCatTesauro = item.DocumentoWebAgCatTesauro,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                CatTesauroAgCatTesauro = item.CatTesauroAgCatTesauro,
                TesauroProyecto = tesauroProyecto
            });
        }


        public static IQueryable<DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauroCategoriaTesauroCatTesauroAgCatTesauroCatTesauroCompartida> JoinCatTesauroCompartidaSegundoSelect(this IQueryable<DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauroCategoriaTesauroCatTesauroAgCatTesauro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CatTesauroCompartida, item => item.CategoriaTesauro.CategoriaTesauroID, catTesauroCompartida => catTesauroCompartida.CategoriaOrigenID, (item, catTesauroCompartida) => new DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauroCategoriaTesauroCatTesauroAgCatTesauroCatTesauroCompartida
            {
                CategoriaTesauro = item.CategoriaTesauro,
                BaseRecursosProyecto = item.BaseRecursosProyecto,
                Documento = item.Documento,
                DocumentoWebAgCatTesauro = item.DocumentoWebAgCatTesauro,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                CatTesauroAgCatTesauro = item.CatTesauroAgCatTesauro,
                CatTesauroCompartida = catTesauroCompartida
            });
        }

        public static IQueryable<DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauroCategoriaTesauroCatTesauroAgCatTesauroCatTesauroCompartidaTesauroProyecto> JoinTesauroProyectoSegundoSelect(this IQueryable<DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauroCategoriaTesauroCatTesauroAgCatTesauroCatTesauroCompartida> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.TesauroProyecto, item => item.CatTesauroCompartida.TesauroDestinoID, tesauroProyecto => tesauroProyecto.TesauroID, (item, tesauroProyecto) => new DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauroCategoriaTesauroCatTesauroAgCatTesauroCatTesauroCompartidaTesauroProyecto
            {
                CategoriaTesauro = item.CategoriaTesauro,
                BaseRecursosProyecto = item.BaseRecursosProyecto,
                Documento = item.Documento,
                DocumentoWebAgCatTesauro = item.DocumentoWebAgCatTesauro,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                CatTesauroAgCatTesauro = item.CatTesauroAgCatTesauro,
                CatTesauroCompartida = item.CatTesauroCompartida,
                TesauroProyecto = tesauroProyecto
            });
        }


        public static IQueryable<DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauroCategoriaTesauro> JoinCategoriaTesauro(this IQueryable<DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CategoriaTesauro, item => item.DocumentoWebAgCatTesauro.CategoriaTesauroID, categoriaTesauro => categoriaTesauro.CategoriaTesauroID, (item, categoriaTesauro) => new DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauroCategoriaTesauro
            {
                BaseRecursosProyecto = item.BaseRecursosProyecto,
                CategoriaTesauro = categoriaTesauro,
                Documento = item.Documento,
                DocumentoWebAgCatTesauro = item.DocumentoWebAgCatTesauro,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos
            });
        }

        public static IQueryable<DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauro> JoinDocumentoWebAgCatTesauro(this IQueryable<DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoWebAgCatTesauro, item => new { item.DocumentoWebVinBaseRecursos.BaseRecursosID, item.Documento.DocumentoID }, documentoWebAgCatTesauro => new { documentoWebAgCatTesauro.BaseRecursosID, documentoWebAgCatTesauro.DocumentoID }, (item, documentoWebAgCatTesauro) => new DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauro
            {
                BaseRecursosProyecto = item.BaseRecursosProyecto,
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                DocumentoWebAgCatTesauro = documentoWebAgCatTesauro
            });
        }

        public static IQueryable<DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyecto> JoinBaseRecursosProyecto(this IQueryable<DocumentoDocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.BaseRecursosProyecto, item => item.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecursosProyecto => baseRecursosProyecto.BaseRecursosID, (item, baseRecursosProyecto) => new DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyecto
            {
                BaseRecursosProyecto = baseRecursosProyecto,
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos
            });
        }

        public static IQueryable<DocumentoDocumentoWebVinBaseRecursos> JoinDocumentoWebVinBaseRecursos(this IQueryable<Documento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoWebVinBaseRecursos, documento => documento.DocumentoID, documentoWebVinBaseRecursos => documentoWebVinBaseRecursos.DocumentoID, (documento, documentoWebVinBaseRecursos) => new DocumentoDocumentoWebVinBaseRecursos
            {
                Documento = documento,
                DocumentoWebVinBaseRecursos = documentoWebVinBaseRecursos
            });
        }

        public static IQueryable<ComentarioDocumentoComentario> JoinDocumentoComentario(this IQueryable<EntityModel.Models.Comentario.Comentario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoComentario, comentario => comentario.ComentarioID, documentoComentario => documentoComentario.ComentarioID, (comentario, documentoComentario) => new ComentarioDocumentoComentario
            {
                Comentario = comentario,
                DocumentoComentario = documentoComentario
            });
        }

        public static IQueryable<DocumentoWebVinBaseRecursosNivelCertificacionBaseRecursosProyecto> JoinBaseRecursosProyecto(this IQueryable<DocumentoWebVinBaseRecursosNivelCertificacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.BaseRecursosProyecto, item => item.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecursosProyecto => baseRecursosProyecto.BaseRecursosID, (item, baseRecursosProyecto) => new DocumentoWebVinBaseRecursosNivelCertificacionBaseRecursosProyecto
            {
                BaseRecursosProyecto = baseRecursosProyecto,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                NivelCertificacion = item.NivelCertificacion
            });
        }

        public static IQueryable<DocumentoWebVinBaseRecursosNivelCertificacion> JoinNivelCertificacion(this IQueryable<DocumentoWebVinBaseRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.NivelCertificacion, documentoWebVinBaseRecursos => documentoWebVinBaseRecursos.NivelCertificacionID, nivelCertificacion => nivelCertificacion.NivelCertificacionID, (documentoWebVinBaseRecursos, nivelCertificacion) => new DocumentoWebVinBaseRecursosNivelCertificacion
            {
                DocumentoWebVinBaseRecursos = documentoWebVinBaseRecursos,
                NivelCertificacion = nivelCertificacion
            });
        }

        public static IQueryable<VotoDocumentoVoto> JoinVoto(this IQueryable<VotoDocumento> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Voto, votoDocumento => votoDocumento.VotoID, voto => voto.VotoID, (votoDocumento, voto) => new VotoDocumentoVoto
            {
                Voto = voto,
                VotoDocumento = votoDocumento
            });
        }

        public static IQueryable<ProyectoParametroGeneral> JoinParametroGeneral(this IQueryable<Proyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.ParametroGeneral, proyecto => proyecto.ProyectoID, parametroGeneral => parametroGeneral.ProyectoID, (proyecto, parametroGeneral) => new ProyectoParametroGeneral
            {
                ParametroGeneral = parametroGeneral,
                Proyecto = proyecto
            });
        }

        public static IQueryable<ProyectoProyectoAgCatTesauroCategoriaTesauro> JoinCategoriaTesauro(this IQueryable<ProyectoProyectoAgCatTesauro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CategoriaTesauro, item => item.ProyectoAgCatTesauro.CategoriaTesauroID, categoriaTesauro => categoriaTesauro.CategoriaTesauroID, (item, categoriaTesauro) => new ProyectoProyectoAgCatTesauroCategoriaTesauro
            {
                CategoriaTesauro = categoriaTesauro,
                Proyecto = item.Proyecto,
                ProyectoAgCatTesauro = item.ProyectoAgCatTesauro
            });
        }

        public static IQueryable<ProyectoProyectoAgCatTesauro> JoinProyectoAgCatTesauro(this IQueryable<Proyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.ProyectoAgCatTesauro, proyecto => proyecto.ProyectoID, proyectoAgCatTesauro => proyectoAgCatTesauro.ProyectoID, (proyecto, proyectoAgCatTesauro) => new ProyectoProyectoAgCatTesauro
            {
                Proyecto = proyecto,
                ProyectoAgCatTesauro = proyectoAgCatTesauro
            });
        }
    }

    /// <summary>
    /// DadaAdapter de MVC
    /// </summary>
    /// 
    public class MVCAD : BaseAD
    {
        private EntityContext mEntityContext;

        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public MVCAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuración de conexión a la base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de conexión a la base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public MVCAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
        }

        #endregion

        #region Métodos Generales

        public List<ObtenerTesauroProyectoMVC> ObtenerCategoriasComunidadesPorID(List<Guid> pListaComunidadesID)
        {
            if (pListaComunidadesID.Count > 0)
            {
                List<ObtenerTesauroProyectoMVC> listaTesauroProyecto = mEntityContext.Proyecto.JoinProyectoAgCatTesauro().JoinCategoriaTesauro().Where(item => pListaComunidadesID.Contains(item.Proyecto.ProyectoID)).Select(item => new ObtenerTesauroProyectoMVC
                {
                    ProyectoID = item.Proyecto.ProyectoID,
                    CategoriaTesauroID = item.CategoriaTesauro.CategoriaTesauroID,
                    Nombre = item.CategoriaTesauro.Nombre
                }).ToList();
                return listaTesauroProyecto;
            }

            return null;
        }

        public List<ObtenerComunidades> ObtenerComunidadesPorID(List<Guid> pListaComunidadesID)
        {
            if (pListaComunidadesID.Count > 0)
            {
                List<ObtenerComunidades> listaComunidades = mEntityContext.Proyecto.JoinParametroGeneral().Where(item => pListaComunidadesID.Contains(item.Proyecto.ProyectoID)).Select(item => new ObtenerComunidades
                {
                    ProyectoID = item.Proyecto.ProyectoID,
                    Nombre = item.Proyecto.Nombre,
                    Descripcion = item.Proyecto.Descripcion,
                    NombreCorto = item.Proyecto.NombreCorto,
                    NombreImagenPeque = item.ParametroGeneral.NombreImagenPeque,
                    NombrePresentacion = item.Proyecto.NombrePresentacion,
                    NumeroMiembros = item.Proyecto.NumeroMiembros,
                    NumeroOrgRegistradas = item.Proyecto.NumeroOrgRegistradas.Value,
                    NumeroRecursos = item.Proyecto.NumeroRecursos,
                    Tags = item.Proyecto.Tags,
                    TipoAcceso = item.Proyecto.TipoAcceso
                }).ToList();

                return listaComunidades;
            }

            return null;
        }

        public List<Blog> ObtenerBlogsPorID(List<Guid> pListaBlogsID)
        {
            if (pListaBlogsID.Count > 0)
            {
                return mEntityContext.Blog.Where(item => pListaBlogsID.Contains(item.BlogID)).ToList();
            }
            return null;
        }

        /// <summary>
        /// Obtiene un recurso a partir de su identificador.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <returns>Dataset de documentación con el documento cargado</returns>
        public List<ObtenerRecursoMVC> ObtenerRecursosPorID(List<Guid> pListaDocumentoID, Guid pProyectoID)
        {
            if (pListaDocumentoID.Count > 0)
            {
                if (!pProyectoID.Equals(ProyectoAD.MetaProyecto))
                {
                    var consultaNoMeta = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursosDocumento().JoinBaseRecursosProyecto().LeftJoinDocumento().JoinDocumentoWebVinBaseRecursosExtra().Where(item => item.DocumentoWebVinBaseRecursos.Eliminado == false && item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && pListaDocumentoID.Contains(item.Documento.DocumentoID)).Select(item => new ObtenerRecursoMVC
                    {
                        DocumentoID = item.Documento.DocumentoID,
                        Titulo = item.Documento.Titulo,
                        Descripcion = item.Documento.Descripcion,
                        Tipo = item.Documento.Tipo,
                        Enlace = item.Documento.Enlace,
                        Tags = item.Documento.Tags,
                        IdentidadPublicacionID = item.DocumentoWebVinBaseRecursos.IdentidadPublicacionID,
                        TipoPublicacion = item.DocumentoWebVinBaseRecursos.TipoPublicacion,
                        FechaPublicacion = item.DocumentoWebVinBaseRecursos.FechaPublicacion,
                        VersionFotoDocumento = item.Documento.VersionFotoDocumento,
                        NumeroComentarios = item.DocumentoWebVinBaseRecursos.NumeroComentarios,
                        NombreCategoriaDoc = item.Documento.NombreCategoriaDoc,
                        ElementoVinculadoID = item.Documento.ElementoVinculadoID,
                        DocOntolologiaEnlace = item.DocOntolologia.Enlace,
                        PrivadoEditores = item.DocumentoWebVinBaseRecursos.PrivadoEditores,
                        NumeroConsultas = item.DocumentoWebVinBaseRecursosExtra.NumeroConsultas,
                        NumeroVotos = item.DocumentoWebVinBaseRecursos.NumeroVotos,
                        CompartirPermitido = item.Documento.CompartirPermitido,
                        Borrador = item.Documento.Borrador,
                        UltimaVersion = item.Documento.UltimaVersion,
                        ProyectoID = item.Documento.ProyectoID,
                        NumeroDescargas = item.DocumentoWebVinBaseRecursosExtra.NumeroDescargas,
                        NombreElementoVinculado = item.Documento.NombreElementoVinculado
                    });
                    return consultaNoMeta.ToList();
                }
                else
                {
                    var consulta = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursosDocumento().LeftJoinDocumento().JoinDocumentoWebVinBaseRecursosExtra().Where(item => item.DocumentoWebVinBaseRecursos.Eliminado == false && item.DocumentoWebVinBaseRecursos.TipoPublicacion == 0 && pListaDocumentoID.Contains(item.Documento.DocumentoID)).Select(item => new ObtenerRecursoMVC
                    {
                        DocumentoID = item.Documento.DocumentoID,
                        Titulo = item.Documento.Titulo,
                        Descripcion = item.Documento.Descripcion,
                        Tipo = item.Documento.Tipo,
                        Enlace = item.Documento.Enlace,
                        Tags = item.Documento.Tags,
                        IdentidadPublicacionID = item.DocumentoWebVinBaseRecursos.IdentidadPublicacionID,
                        TipoPublicacion = item.DocumentoWebVinBaseRecursos.TipoPublicacion,
                        FechaPublicacion = item.DocumentoWebVinBaseRecursos.FechaPublicacion,
                        VersionFotoDocumento = item.Documento.VersionFotoDocumento,
                        NumeroComentarios = item.DocumentoWebVinBaseRecursos.NumeroComentarios,
                        NombreCategoriaDoc = item.Documento.NombreCategoriaDoc,
                        ElementoVinculadoID = item.Documento.ElementoVinculadoID,
                        DocOntolologiaEnlace = item.DocOntolologia.Enlace,
                        PrivadoEditores = item.DocumentoWebVinBaseRecursos.PrivadoEditores,
                        NumeroConsultas = item.DocumentoWebVinBaseRecursosExtra.NumeroConsultas,
                        NumeroVotos = item.DocumentoWebVinBaseRecursos.NumeroVotos,
                        CompartirPermitido = item.Documento.CompartirPermitido,
                        Borrador = item.Documento.Borrador,
                        UltimaVersion = item.Documento.UltimaVersion,
                        ProyectoID = item.Documento.ProyectoID,
                        NumeroDescargas = item.DocumentoWebVinBaseRecursosExtra.NumeroDescargas,
                        NombreElementoVinculado = item.Documento.NombreElementoVinculado
                    });
                    return consulta.ToList();
                }
            }
            return new List<ObtenerRecursoMVC>();
        }

        /// <summary>
        /// Obtiene los eventos de un recurso en u proyecto a partir de su identificador y el del proyecto
        /// </summary>
        /// <param name="pListaDocumentoID">Identificador de los recursos</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Ficha del recurso</returns>
        public List<ObtenerRecursosEvento> ObtenerEventosRecursosPorID(List<Guid> pListaDocumentoID, Guid pProyectoID)
        {
            if (pListaDocumentoID.Count > 0)
            {
                var primeraParte = mEntityContext.VotoDocumento.JoinVoto().Where(item => item.VotoDocumento.ProyectoID.Value.Equals(pProyectoID) && pListaDocumentoID.Contains(item.VotoDocumento.DocumentoID)).GroupBy(item => item.VotoDocumento.DocumentoID, objeto => objeto, (agrupacion, objeto) => new
                {
                    DocumentoID = agrupacion,
                    FechaEvento = objeto.Max(item => item.Voto.FechaVotacion),
                }).Select(item => new ObtenerRecursosEvento
                {
                    DocumentoID = item.DocumentoID,
                    TipoEvento = 0,
                    FechaEvento = item.FechaEvento,
                    Evento = ""
                }).ToList();

                var segundaParte = mEntityContext.DocumentoWebVinBaseRecursos.JoinNivelCertificacion().JoinBaseRecursosProyecto().Where(item => item.DocumentoWebVinBaseRecursos.FechaCertificacion.HasValue && pListaDocumentoID.Contains(item.DocumentoWebVinBaseRecursos.DocumentoID) && item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID)).Select(item => new ObtenerRecursosEvento
                {
                    DocumentoID = item.DocumentoWebVinBaseRecursos.DocumentoID,
                    TipoEvento = 1,
                    FechaEvento = item.DocumentoWebVinBaseRecursos.FechaCertificacion,
                    Evento = item.NivelCertificacion.Descripcion
                }).ToList();

                var terceraParte = mEntityContext.Comentario.JoinDocumentoComentario().Where(item => pListaDocumentoID.Contains(item.DocumentoComentario.DocumentoID) && item.DocumentoComentario.ProyectoID.Value.Equals(pProyectoID) && item.Comentario.Eliminado.Equals(false)).Select(item => new ObtenerRecursosEvento
                {
                    DocumentoID = item.DocumentoComentario.DocumentoID,
                    TipoEvento = 2,
                    FechaEvento = item.Comentario.Fecha,
                    Evento = item.Comentario.ComentarioID.ToString()
                }).ToList();

                return primeraParte.Concat(segundaParte).Concat(terceraParte).OrderBy(item => item.DocumentoID).ThenByDescending(item => item.FechaEvento).ToList();
            }
            return null;
        }

        public List<Guid> ObtenerCategoriaDeTesauroPorID(List<Guid> pListaDocumentoID)
        {
            return mEntityContext.DocumentoWebAgCatTesauro.Where(item => pListaDocumentoID.Contains(item.DocumentoID)).Select(item => item.CategoriaTesauroID).ToList();
        }

        public Dictionary<Guid, List<Guid>> ObtenerDiccionarioCategoriaDeTesauroPorID(List<Guid> pListaDocumentoID)
        {
            Dictionary<Guid, List<Guid>> diccionarioClaveCategorias = new Dictionary<Guid, List<Guid>>();
            foreach (Guid documentoID in pListaDocumentoID)
            {
                List<Guid> listaCategoriasDocumento = mEntityContext.DocumentoWebAgCatTesauro.Where(item => item.DocumentoID.Equals(documentoID)).Select(item => item.CategoriaTesauroID).ToList();
                diccionarioClaveCategorias.Add(documentoID, listaCategoriasDocumento);
            }
            return diccionarioClaveCategorias;
        }

        public List<ObtenerCategoriaDeRecursos> ObtenerCategoriasDeRecursosPorID(List<Guid> pListaDocumentoID, Guid pProyectoID)
        {
            if (pListaDocumentoID.Count > 0)
            {

                IQueryable<DocumentoDocumentoWebVinBaseRecursosBaseRecursosProyectoDocumentoWebAgCatTesauroCategoriaTesauroCatTesauroAgCatTesauro> selectCategoriasMVC = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().JoinDocumentoWebAgCatTesauro().JoinCategoriaTesauro().JoinCatTesauroAgCatTesauro();

                IQueryable<ObtenerCategoriaDeRecursos> selectPrimero = selectCategoriasMVC.JoinTesauroProyectoPrimerSelect().Where(item => pListaDocumentoID.Contains(item.Documento.DocumentoID) && item.TesauroProyecto.ProyectoID.Equals(pProyectoID)).Select(item => new ObtenerCategoriaDeRecursos
                {
                    DocumentoID = item.Documento.DocumentoID,
                    CategoriaTesauroID = item.CategoriaTesauro.CategoriaTesauroID,
                    Nombre = item.CategoriaTesauro.Nombre,
                    CategoriaSuperiorID = item.CatTesauroAgCatTesauro.CategoriaSuperiorID
                });


                IQueryable<ObtenerCategoriaDeRecursos> selectSegundo = selectCategoriasMVC.JoinCatTesauroCompartidaSegundoSelect().JoinTesauroProyectoSegundoSelect().Where(item => pListaDocumentoID.Contains(item.Documento.DocumentoID) && item.TesauroProyecto.ProyectoID.Equals(pProyectoID)).Select(item => new ObtenerCategoriaDeRecursos
                {
                    DocumentoID = item.Documento.DocumentoID,
                    CategoriaTesauroID = item.CategoriaTesauro.CategoriaTesauroID,
                    Nombre = item.CategoriaTesauro.Nombre,
                    CategoriaSuperiorID = item.CatTesauroAgCatTesauro.CategoriaSuperiorID
                });

                var query = selectPrimero.Union(selectSegundo);

                return selectPrimero.Union(selectSegundo).ToList();


                //selectCategoriasMVC.JoinTesauroProyecto()


                //List<Guid> listaIdSubconsulta = EntityContext.CategoriaTesauro.JoinTesauroProyecto().Where(item => item.TesauroProyecto.ProyectoID.Equals(pProyectoID)).Select(item => item.CategoriaTesauro.CategoriaTesauroID).Union(EntityContext.CatTesauroCompartida.JoinTesauroProyecto().Where(item => item.TesauroProyecto.ProyectoID.Equals(pProyectoID)).Select(item => item.CatTesauroCompartida.CategoriaOrigenID)).ToList();


                //var finalQuery = EntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().JoinDocumentoWebAgCatTesauro().JoinCategoriaTesauro().JoinCatTesauroAgCatTesauro().Where(item => listaIdSubconsulta.Contains(item.CategoriaTesauro.CategoriaTesauroID) && pListaDocumentoID.Contains(item.Documento.DocumentoID)).Select(item => new ObtenerCategoriaDeRecursos
                //{
                //    DocumentoID = item.Documento.DocumentoID,
                //    CategoriaTesauroID = item.CategoriaTesauro.CategoriaTesauroID,
                //    Nombre = item.CategoriaTesauro.Nombre,
                //    CategoriaSuperiorID = item.CatTesauroAgCatTesauro.CategoriaSuperiorID
                //});

                //return finalQuery.ToList();
            }
            return null;
        }

        /// <summary>
        /// Obtiene las categorías de una serie de recursos de un base de recursos personal.
        /// </summary>
        /// <param name="pListaDocumentoID">IDs de documento</param>
        /// <param name="pBaseRecursosPersonalID">ID de la base de recursos personal</param>
        /// <param name="pBaseRecursosOrganizacion">Indica si la base de recursos es de organización</param>
        /// <returns>categorías de una serie de recursos de un base de recursos personal</returns>
        public List<ObtenerCategoriaDeRecursos> ObtenerCategoriasDeRecursosPorIDEspacioPersonal(List<Guid> pListaDocumentoID, Guid pBaseRecursosPersonalID, bool pBaseRecursosOrganizacion)
        {
            if (pListaDocumentoID.Count > 0)
            {
                List<ObtenerCategoriaDeRecursos> categoriasDeRecursos = null;

                if (!pBaseRecursosOrganizacion)
                {
                    List<Guid> listaIdSubconsulta = mEntityContext.CategoriaTesauro.JoinTesauroUsuario().JoinBaseRecursosUsuario().Where(item => item.BaseRecursosUsuario.BaseRecursosID.Equals(pBaseRecursosPersonalID)).Select(item => item.CategoriaTesauro.CategoriaTesauroID).ToList();

                    categoriasDeRecursos = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursos().JoinDocumentoWebAgCatTesauro().JoinCategoriaTesauro().JoinCatTesauroAgCatTesauro().Where(item => item.BaseRecursos.BaseRecursosID.Equals(pBaseRecursosPersonalID) && listaIdSubconsulta.Contains(item.CategoriaTesauro.CategoriaTesauroID) && pListaDocumentoID.Contains(item.Documento.DocumentoID)).Select(item => new ObtenerCategoriaDeRecursos
                    {
                        DocumentoID = item.Documento.DocumentoID,
                        CategoriaTesauroID = item.CategoriaTesauro.CategoriaTesauroID,
                        Nombre = item.CategoriaTesauro.Nombre,
                        CategoriaSuperiorID = item.CatTesauroAgCatTesauro.CategoriaSuperiorID
                    }).ToList();
                }
                else
                {
                    List<Guid> listaIdSubconsulta = mEntityContext.CategoriaTesauro.JoinTesauroOrganizacion().JoinBaseRercursosOrganizacion().Where(item => item.BaseRecursosOrganizacion.BaseRecursosID.Equals(pBaseRecursosPersonalID)).Select(item => item.CategoriaTesauro.CategoriaTesauroID).ToList();

                    categoriasDeRecursos = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursos().JoinDocumentoWebAgCatTesauro().JoinCategoriaTesauro().JoinCatTesauroAgCatTesauro().Where(item => item.BaseRecursos.BaseRecursosID.Equals(pBaseRecursosPersonalID) && listaIdSubconsulta.Contains(item.CategoriaTesauro.CategoriaTesauroID) && pListaDocumentoID.Contains(item.Documento.DocumentoID)).Select(item => new ObtenerCategoriaDeRecursos
                    {
                        DocumentoID = item.Documento.DocumentoID,
                        CategoriaTesauroID = item.CategoriaTesauro.CategoriaTesauroID,
                        Nombre = item.CategoriaTesauro.Nombre,
                        CategoriaSuperiorID = item.CatTesauroAgCatTesauro.CategoriaSuperiorID
                    }).ToList();
                }
                return categoriasDeRecursos;
            }
            return null;
        }

        /// <summary>
        /// Obtiene los comentarios de los recursos pasados por id
        /// </summary>
        /// <param name="pListaDocumentoID"></param>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public List<EntityModel.Models.Comentario.Comentario> ObtenerComentariosDeRecursosPorID(List<Guid> pListaComentariosID)
        {
            if (pListaComentariosID.Count > 0)
            {
                return mEntityContext.Comentario.Where(item => pListaComentariosID.Contains(item.ComentarioID)).ToList().Select(item => new EntityModel.Models.Comentario.Comentario
                {
                    ComentarioID = item.ComentarioID,
                    Descripcion = item.Descripcion,
                    Fecha = item.Fecha,
                    IdentidadID = item.IdentidadID
                }).ToList();
            }
            return null;
        }

        public List<IdentidadMVC> ObtenerIdentidadesPorID(List<Guid> pListaIdentidadesID)
        {
            if (pListaIdentidadesID.Count > 0)
            {
                var consulta = mEntityContext.Identidad.JoinPerfil().LeftJoinPersona().LeftJoinCurriculum().Where(item => pListaIdentidadesID.Contains(item.Identidad.IdentidadID));

                var consulta2 = consulta.ToList().Select(item => new IdentidadMVC
                {
                    IdentidadID = item.Identidad.IdentidadID,
                    Tipo = item.Identidad.Tipo,
                    NombreCortoOrg = item.Perfil.NombreCortoOrg,
                    NombreCortoUsu = item.Perfil.NombreCortoUsu,
                    NombreOrganizacion = item.Perfil.NombreOrganizacion,
                    NombrePerfil = item.Perfil.NombrePerfil,
                    OrganizacionID = item.Perfil.OrganizacionID,
                    PersonaID = item.Perfil.PersonaID,
                    Foto = item.Identidad.Foto,
                    NumConnexiones = item.Identidad.NumConnexiones,
                    Tags = item.Curriculum != null ? item.Curriculum.Tags : null,
                    FechaNacimiento = item.Persona != null ? item.Persona.FechaNacimiento : null,
                    TieneEmailTutor = (item.Persona != null && !string.IsNullOrEmpty(item.Persona.EmailTutor)) ? true : false
                });
                return consulta2.ToList();
            }
            return new List<IdentidadMVC>();
        }

        public List<DatoExtraFichasdentidad> ObtenerDatosExtraFichasIdentidadesPorPerfilesID(List<Guid> pListaPerfilesID)
        {
            if (pListaPerfilesID != null && pListaPerfilesID.Count > 0)
            {
                var query1 = mEntityContext.Identidad.JoinDatoExtraProyectoOpcionIdentidad().JoinDatoExtraProyectoOpcion().JoinDatoExtraProyecto().Where(item => pListaPerfilesID.Contains(item.Identidad.PerfilID)).Select(item => new DatoExtraFichasdentidad
                {
                    IdentidadID = item.Identidad.IdentidadID,
                    PerfilID = item.Identidad.PerfilID,
                    Titulo = item.DatoExtraProyecto.Titulo,
                    Opcion = item.DatoExtraProyectoOpcion.Opcion,
                    ProyectoID = item.DatoExtraProyecto.OrganizacionID
                });

                var query2 = mEntityContext.Identidad.JoinDatoExtraVirtuosoIdentidad().JoinDatoExtraProyectoVirtuoso().Where(item => pListaPerfilesID.Contains(item.Identidad.PerfilID)).Select(item => new DatoExtraFichasdentidad
                {
                    IdentidadID = item.Identidad.IdentidadID,
                    PerfilID = item.Identidad.PerfilID,
                    Titulo = item.DatoExtraProyectoVirtuoso.Titulo,
                    Opcion = item.DatoExtraProyectoVirtuosoIdentidad.Opcion,
                    ProyectoID = item.DatoExtraProyectoVirtuoso.ProyectoID
                });

                var query3 = mEntityContext.Identidad.JoinDatoExtraEcosistemaOpcionPerfil().JoinDatoExtraEcosistemaOpcion().JoinDatoExtraEcosistema().Where(item => pListaPerfilesID.Contains(item.Identidad.PerfilID)).Select(item => new DatoExtraFichasdentidad
                {
                    IdentidadID = item.Identidad.IdentidadID,
                    PerfilID = item.Identidad.PerfilID,
                    Titulo = item.DatoExtraEcosistema.Titulo,
                    Opcion = item.DatoExtraEcosistemaOpcion.Opcion,
                    ProyectoID = Guid.Empty
                });

                var query4 = mEntityContext.Identidad.JoinDatoExtraEcosistemaVirtuosoPerfil().JoinDatoExtraEcosistemaVirtuoso().Where(item => pListaPerfilesID.Contains(item.Identidad.PerfilID)).Select(item => new DatoExtraFichasdentidad
                {
                    IdentidadID = item.Identidad.IdentidadID,
                    PerfilID = item.Identidad.PerfilID,
                    Titulo = item.DatoExtraEcosistemaVirtuoso.Titulo,
                    Opcion = item.DatoExtraEcosistemaVirtuosoPerfil.Opcion,
                    ProyectoID = Guid.Empty
                });

                return query1.Union(query2).Union(query3).Union(query4).ToList();
            }
            return null;
        }

        public List<DatoExtraFichasdentidad> ObtenerDatosExtraFichasIdentidadesPorIdentidadesID(List<Guid> pListaIdentidadesID)
        {
            if (pListaIdentidadesID != null && pListaIdentidadesID.Count > 0)
            {
                var query1 = mEntityContext.Identidad.JoinDatoExtraProyectoOpcionIdentidad().JoinDatoExtraProyectoOpcion().JoinDatoExtraProyecto().Where(item => pListaIdentidadesID.Contains(item.Identidad.IdentidadID)).Select(item => new DatoExtraFichasdentidad
                {
                    IdentidadID = item.Identidad.IdentidadID,
                    Titulo = item.DatoExtraProyecto.Titulo,
                    Opcion = item.DatoExtraProyectoOpcion.Opcion,
                    ProyectoID = item.DatoExtraProyecto.OrganizacionID
                });

                var query2 = mEntityContext.Identidad.JoinDatoExtraVirtuosoIdentidad().JoinDatoExtraProyectoVirtuoso().Where(item => pListaIdentidadesID.Contains(item.Identidad.IdentidadID)).Select(item => new DatoExtraFichasdentidad
                {
                    IdentidadID = item.Identidad.IdentidadID,
                    Titulo = item.DatoExtraProyectoVirtuoso.Titulo,
                    Opcion = item.DatoExtraProyectoVirtuosoIdentidad.Opcion,
                    ProyectoID = item.DatoExtraProyectoVirtuoso.ProyectoID
                });

                var query3 = mEntityContext.Identidad.JoinDatoExtraEcosistemaOpcionPerfil().JoinDatoExtraEcosistemaOpcion().JoinDatoExtraEcosistema().Where(item => pListaIdentidadesID.Contains(item.Identidad.IdentidadID)).Select(item => new DatoExtraFichasdentidad
                {
                    IdentidadID = item.Identidad.IdentidadID,
                    Titulo = item.DatoExtraEcosistema.Titulo,
                    Opcion = item.DatoExtraEcosistemaOpcion.Opcion,
                    ProyectoID = Guid.Empty
                });

                var query4 = mEntityContext.Identidad.JoinDatoExtraEcosistemaVirtuosoPerfil().JoinDatoExtraEcosistemaVirtuoso().Where(item => pListaIdentidadesID.Contains(item.Identidad.IdentidadID)).Select(item => new DatoExtraFichasdentidad
                {
                    IdentidadID = item.Identidad.IdentidadID,
                    Titulo = item.DatoExtraEcosistemaVirtuoso.Titulo,
                    Opcion = item.DatoExtraEcosistemaVirtuosoPerfil.Opcion,
                    ProyectoID = Guid.Empty
                });

                return query1.Union(query2).Union(query3).Union(query4).ToList();
            }
            return null;
        }

        /// <summary>
        /// Obtiene los contadores de recuros de una lista de identidades
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        public List<IdentidadContadoresRecursos> ObtenerContadoresRecursosIdentiadesPorID(List<Guid> pListaIdentidadesID)
        {
            if (pListaIdentidadesID.Count > 0)
            {
                return mEntityContext.IdentidadContadoresRecursos.Where(item => pListaIdentidadesID.Contains(item.IdentidadID)).ToList();
            }
            return null;
        }

        /// <summary>
        /// Obtiene un DataReader a partir de una lista de identificadores de grupos
        /// </summary>
        /// <param name="pListaGruposID">Lista de identificadores de grupos</param>
        public List<GrupoIdentidadesPorId> ObtenerGruposPorID(List<Guid> pListaGruposID)
        {
            if (pListaGruposID.Count > 0)
            {
                var queryGrupoProy = mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesProyecto().JoinProyecto().Select(item => new GrupoIdentidadesPorId
                {
                    GrupoID = item.GrupoIdentidades.GrupoID,
                    Nombre = item.GrupoIdentidades.Nombre,
                    NombreCorto = item.GrupoIdentidades.NombreCorto,
                    ProyectoID = item.Proyecto.ProyectoID,
                    ProyectoNombreCorto = item.Proyecto.NombreCorto,
                    OrganizacionID = null
                });
                var prueba = queryGrupoProy.Where(item => pListaGruposID.Contains(item.GrupoID)).ToList();
                var queryGrupoOrg = mEntityContext.GrupoIdentidades.JoinGrupoIdentidadesOrganizacion().Select(item => new GrupoIdentidadesPorId
                {
                    GrupoID = item.GrupoIdentidades.GrupoID,
                    Nombre = item.GrupoIdentidades.Nombre,
                    NombreCorto = item.GrupoIdentidades.NombreCorto,
                    ProyectoID = null,
                    ProyectoNombreCorto = null,
                    OrganizacionID = item.GrupoIdentidadesOrganizacion.OrganizacionID
                });
                
                var prueba2 = queryGrupoOrg.Where(item => pListaGruposID.Contains(item.GrupoID)).ToList();
                return queryGrupoProy.Where(item => pListaGruposID.Contains(item.GrupoID)).ToList().Concat(queryGrupoOrg.Where(item => pListaGruposID.Contains(item.GrupoID)).ToList()).Distinct().ToList();
            }
            return null;
        }

        /// <summary>
        /// Obtiene un DataReader a partir de una lista de identificadores de mensajes
        /// </summary>
        /// <param name="pListaMensajesID">Lista de identificadores de mensajes</param>
        public IDataReader ObtenerMensajesPorID(List<Guid> pListaMensajesID, Guid pIdentidadID, short? pTipoBandeja)
        {
            //Preguntar JUAN (Jaleo tablas CorreoInterno)
            if (pListaMensajesID.Count > 0)
            {
                //EntityContext.CorreoInterno.Where(item => pListaMensajesID.Contains(item.CorreoID) && item.EnPapelera.Equals(0) && item.)

                string consulta = "SELECT " + IBD.CargarGuid("CorreoInterno.CorreoID") + ", " + IBD.CargarGuid("CorreoInterno.Destinatario") + ", " + IBD.CargarGuid("CorreoInterno.Autor") + ", CorreoInterno.Asunto, CorreoInterno.Cuerpo, CorreoInterno.Fecha, CorreoInterno.Leido, CorreoInterno.Eliminado, CorreoInterno.EnPapelera, CorreoInterno.DestinatariosID, CorreoInterno.DestinatariosNombres, CorreoInterno.ConversacionID FROM CorreoInterno ";

                string where = " WHERE CorreoID IN (";
                foreach (Guid idCorreo in pListaMensajesID)
                {
                    where += "'" + idCorreo.ToString() + "',";
                }
                where = where.Substring(0, where.Length - 1);
                where += ")";

                if (pTipoBandeja.HasValue)
                {
                    if (pTipoBandeja.Value == 0)
                    {
                        where += " AND EnPapelera=0 AND Destinatario = " + IBD.GuidValor(pIdentidadID);
                    }
                    else if (pTipoBandeja.Value == 1)
                    {
                        where += " AND EnPapelera=0 AND Destinatario = " + IBD.GuidValor(Guid.Empty);
                    }
                    else if (pTipoBandeja.Value == 2)
                    {
                        where += " AND EnPapelera=1";
                    }
                }
                else
                {
                    where += "  AND (Autor = " + IBD.GuidValor(pIdentidadID) + " AND Destinatario = " + IBD.GuidValor(Guid.Empty) + " or destinatario = " + IBD.GuidValor(pIdentidadID) + ")";
                }

                string tablaCorreoInterno = "CorreoInterno_" + pIdentidadID.ToString().Substring(0, 2);

                consulta = (consulta + where).Replace("CorreoInterno", tablaCorreoInterno);

                DbCommand commandsqlSelectCorreoInternoPorID = ObtenerComando(consulta);

                return EjecutarReader(commandsqlSelectCorreoInternoPorID);
            }
            return null;
        }

        /// <summary>
        /// Obtiene un DataReader a partir de una lista de identificadores de comentarios
        /// </summary>
        /// <param name="pListaComentariosID">Lista de identificadores de comentarios</param>
        public List<ComentarioDocumentoProyecto> ObtenerComentariosPorID(List<Guid> pListaComentariosID, Guid pIdentidadID)
        {
            if (pListaComentariosID.Count > 0)
            {
                return mEntityContext.Comentario.JoinDocumentoComentario().JoinDocumento().JoinProyecto().Where(item => pListaComentariosID.Contains(item.Comentario.ComentarioID)).Select(item => new ComentarioDocumentoProyecto
                {
                    ComentarioID = item.Comentario.ComentarioID,
                    IdentidadID = item.Comentario.IdentidadID,
                    DocumentoID = item.DocumentoComentario.DocumentoID,
                    ProyectoID = item.Proyecto.ProyectoID,
                    Fecha = item.Comentario.Fecha,
                    Descripcion = item.Comentario.Descripcion,
                    NombreCorto = item.Proyecto.NombreCorto,
                    Titulo = item.Documento.Titulo,
                    VersionFotoDocumento = item.Documento.VersionFotoDocumento,
                    Tipo = item.Documento.Tipo,
                    NombreCategoriaDoc = item.Documento.NombreCategoriaDoc,
                    ElementoVinculadoID = item.Documento.ElementoVinculadoID,
                    NombreElementoVinculado = item.Documento.NombreElementoVinculado,
                    Enlace = item.Documento.Enlace
                }).ToList();
            }
            return null;
        }

        /// <summary>
        /// Obtiene un DataReader a partir de unas listas de identificadores de contactos
        /// </summary>
        /// <param name="pListaPersonas">Lista de identificadores de personas</param>
        /// <param name="pListaOrganizaciones">Lista de identificadores de organizaciones</param>
        /// <param name="pListaGrupos">Lista de identificadores de grupos</param>
        /// <param name="pIdentidadID"></param>
        /// <returns></returns>
        public List<ContactosPorID> ObtenerContactosPorID(List<Guid> pListaPersonas, List<Guid> pListaOrganizaciones, List<Guid> pListaGrupos, Guid pIdentidadID)
        {
            if (pListaPersonas.Count > 0 || pListaOrganizaciones.Count > 0 || pListaGrupos.Count > 0)
            {
                var queryPersonas = mEntityContext.Identidad.JoinPerfil().Select(item => new ContactosPorID
                {
                    IdentidadID = item.Identidad.IdentidadID,
                    TipoContacto = 0,
                    NombrePerfil = item.Perfil.NombrePerfil,
                    NombreCortoUsu = item.Perfil.NombreCortoUsu,
                    NombreCortoOrg = item.Perfil.NombreCortoOrg,
                    Foto = item.Identidad.Foto
                });

                var queryOrganizacion = mEntityContext.Identidad.JoinPerfil().JoinOrganizacion().Select(item => new ContactosPorID
                {
                    IdentidadID = item.Identidad.IdentidadID,
                    TipoContacto = 1,
                    NombrePerfil = item.Perfil.NombrePerfil,
                    NombreCortoUsu = item.Perfil.NombreCortoUsu,
                    NombreCortoOrg = item.Perfil.NombreCortoOrg,
                    Foto = item.Identidad.Foto,
                    OrganizacionID = item.Organizacion.OrganizacionID
                });

                var queryGrupos = mEntityContext.GrupoAmigos.Select(item => new ContactosPorID
                {
                    GrupoID = item.GrupoID,
                    TipoContacto = 2,
                    Nombre = item.Nombre
                });

                if (pListaPersonas.Count > 0)
                {
                    if (pListaOrganizaciones.Count > 0)
                    {
                        if (pListaGrupos.Count > 0)
                        {
                            //Personas, Organizacion y Grupos
                            return queryPersonas.Where(item => pListaPersonas.Contains(item.IdentidadID)).Concat(queryOrganizacion.Where(item => pListaOrganizaciones.Contains(item.OrganizacionID))).Concat(queryGrupos.Where(item => pListaGrupos.Contains(item.GrupoID))).ToList();
                        }
                        else
                        {
                            //Personas y Organizacion
                            return queryPersonas.Where(item => pListaPersonas.Contains(item.IdentidadID)).Concat(queryOrganizacion.Where(item => pListaOrganizaciones.Contains(item.OrganizacionID))).ToList();
                        }
                    }
                    else
                    {
                        if (pListaGrupos.Count > 0)
                        {
                            //Personas y Grupos
                            return queryPersonas.Where(item => pListaPersonas.Contains(item.IdentidadID)).Concat(queryGrupos.Where(item => pListaGrupos.Contains(item.GrupoID))).ToList();
                        }
                        else
                        {
                            //Personas
                            return queryPersonas.Where(item => pListaPersonas.Contains(item.IdentidadID)).ToList();
                        }
                    }
                }
                else if (pListaOrganizaciones.Count > 0)
                {
                    if (pListaGrupos.Count > 0)
                    {
                        //Organizacion y Grupos
                        return queryOrganizacion.Where(item => pListaOrganizaciones.Contains(item.OrganizacionID)).Concat(queryGrupos.Where(item => pListaGrupos.Contains(item.GrupoID))).ToList();
                    }
                    else
                    {
                        //Organizacion
                        return queryOrganizacion.Where(item => pListaOrganizaciones.Contains(item.OrganizacionID)).ToList();
                    }
                }
                else
                {
                    //Grupos
                    return queryGrupos.Where(item => pListaGrupos.Contains(item.GrupoID)).ToList();
                }
            }
            return null;
        }

        /// <summary>
        /// Obtiene un DataReader a partir de unas listas de identificadores de contactos
        /// </summary>
        /// <param name="pListaPersonas">Lista de identificadores de personas</param>
        /// <param name="pListaOrganizaciones">Lista de identificadores de organizaciones</param>
        /// <param name="pListaGrupos">Lista de identificadores de grupos</param>
        /// <param name="pIdentidadID"></param>
        /// <returns></returns>
        public List<ParticipantesGrupoContactos> ObtenerParticipantesGruposContactosPorID(List<Guid> pListaPersonas, List<Guid> pListaOrganizaciones, List<Guid> pListaGrupos, Guid pIdentidadID)
        {
            if (pListaPersonas.Count > 0 || pListaOrganizaciones.Count > 0 || pListaGrupos.Count > 0)
            {
                var queryPersonas = mEntityContext.AmigoAgGrupo.JoinGrupoAmigos().Select(item => new ParticipantesGrupoContactos
                {
                    GrupoID = item.GrupoAmigos.GrupoID,
                    IdentidadAmigoID = item.AmigoAgGrupo.IdentidadAmigoID,
                    Tipo = 0,
                    Nombre = item.GrupoAmigos.Nombre,
                    IdentidadID = item.AmigoAgGrupo.IdentidadID
                });

                var queryOrganizaciones = mEntityContext.AmigoAgGrupo.JoinGrupoAmigos().Select(item => new ParticipantesGrupoContactos
                {
                    GrupoID = item.GrupoAmigos.GrupoID,
                    IdentidadAmigoID = item.AmigoAgGrupo.IdentidadAmigoID,
                    Tipo = 0,
                    Nombre = item.GrupoAmigos.Nombre,
                    IdentidadID = item.AmigoAgGrupo.IdentidadID
                });

                var queryGrupos = mEntityContext.Identidad.JoinPerfil().JoinAmigoAgGrupo().Select(item => new ParticipantesGrupoContactos
                {
                    GrupoID = item.AmigoAgGrupo.GrupoID,
                    IdentidadID = item.Identidad.IdentidadID,
                    Tipo = 1,
                    Nombre = item.Perfil.NombrePerfil
                });

                if (pListaPersonas.Count > 0)
                {
                    if (pListaOrganizaciones.Count > 0)
                    {
                        if (pListaGrupos.Count > 0)
                        {
                            //1, 2, 3
                            return queryPersonas.Where(item => item.IdentidadID.Equals(pIdentidadID) && pListaPersonas.Contains(item.IdentidadAmigoID)).Concat(queryOrganizaciones.Where(item => item.IdentidadID.Equals(pIdentidadID) && pListaOrganizaciones.Contains(item.IdentidadAmigoID))).Concat(queryGrupos.Where(item => item.IdentidadID.Equals(pIdentidadID) && pListaGrupos.Contains(item.GrupoID))).ToList();
                        }
                        else
                        {
                            //1, 2
                            return queryPersonas.Where(item => item.IdentidadID.Equals(pIdentidadID) && pListaPersonas.Contains(item.IdentidadAmigoID)).Concat(queryOrganizaciones.Where(item => item.IdentidadID.Equals(pIdentidadID) && pListaOrganizaciones.Contains(item.IdentidadAmigoID))).ToList();
                        }
                    }
                    else
                    {
                        if (pListaGrupos.Count > 0)
                        {
                            //1 y 3
                            return queryPersonas.Where(item => item.IdentidadID.Equals(pIdentidadID) && pListaPersonas.Contains(item.IdentidadAmigoID)).Concat(queryGrupos.Where(item => item.IdentidadID.Equals(pIdentidadID) && pListaGrupos.Contains(item.GrupoID))).ToList();
                        }
                        else
                        {
                            //1
                            return queryPersonas.Where(item => item.IdentidadID.Equals(pIdentidadID) && pListaPersonas.Contains(item.IdentidadAmigoID)).ToList();
                        }
                    }
                }
                else if (pListaOrganizaciones.Count > 0)
                {
                    if (pListaGrupos.Count > 0)
                    {
                        //2 y 3
                        return queryOrganizaciones.Where(item => item.IdentidadID.Equals(pIdentidadID) && pListaOrganizaciones.Contains(item.IdentidadAmigoID)).Concat(queryGrupos.Where(item => item.IdentidadID.Equals(pIdentidadID) && pListaGrupos.Contains(item.GrupoID))).ToList();
                    }
                    else
                    {
                        // 2
                        return queryOrganizaciones.Where(item => item.IdentidadID.Equals(pIdentidadID) && pListaOrganizaciones.Contains(item.IdentidadAmigoID)).ToList();
                    }
                }
                else
                {
                    //3
                    return queryGrupos.Where(item => item.IdentidadID.Equals(pIdentidadID) && pListaGrupos.Contains(item.GrupoID)).ToList();
                }
            }
            return null;
        }
        #endregion
    }
}
