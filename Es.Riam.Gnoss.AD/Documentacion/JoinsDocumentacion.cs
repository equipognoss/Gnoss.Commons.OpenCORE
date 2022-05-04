using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using System.Linq;

namespace Es.Riam.Gnoss.AD.Documentacion
{

    public static class JoinsDocumentacion
    {
        public static IQueryable<IdentidadPerfil> JoinPerfilIdentidad(this IQueryable<Perfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, perf => perf.PerfilID, ident => ident.PerfilID, (perf, ident) =>
                new IdentidadPerfil
                {
                    Perfil = perf,
                    Identidad = ident
                });
        }

        public static IQueryable<IdentidadPerfilDocumento> JoinDocumento(this IQueryable<IdentidadPerfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Documento, objeto => objeto.Identidad.IdentidadID, documento => documento.CreadorID.Value, (objeto, documento) => new IdentidadPerfilDocumento
            {
                Perfil = objeto.Perfil,
                Identidad = objeto.Identidad,
                Documento = documento
            });
        }

        public static IQueryable<DocumentoWebVinBaseRecursosDocumentoDocumentoWebAgCatTesauro> JoinDocumentoWebAgCatTesauro(this IQueryable<DocumentoWebVinBaseRecursosDocumento> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.DocumentoWebAgCatTesauro, objeto => new { objeto.DocumentoWebVinBaseRecursos.DocumentoID, objeto.DocumentoWebVinBaseRecursos.BaseRecursosID }, docWebAg => new { docWebAg.DocumentoID, docWebAg.BaseRecursosID }, (objeto, docWebAg) => new DocumentoWebVinBaseRecursosDocumentoDocumentoWebAgCatTesauro
            {
                Documento = objeto.Documento,
                DocumentoWebAgCatTesauro = docWebAg,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos
            });
        }
        public static IQueryable<DocumentoWebVinBaseRecursosDocumento> JoinDocumentoWebVinBaseRecursosDocumento(this IQueryable<Documento> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.DocumentoWebVinBaseRecursos, doc => doc.DocumentoID, docWebvin => docWebvin.DocumentoID, (doc, docWebvin) => new DocumentoWebVinBaseRecursosDocumento
            {
                Documento = doc,
                DocumentoWebVinBaseRecursos = docWebvin
            });
        }

        public static IQueryable<DocumentoWebVinBaseRecursosDocumentoDocumento> LeftJoinDocumento(this IQueryable<DocumentoWebVinBaseRecursosDocumento> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.GroupJoin(entityContext.Documento, item => new { DocumentoID = item.Documento.ElementoVinculadoID.Value }, docOntologia => new
            { DocumentoID = docOntologia.DocumentoID }, (item, docOntologia) => new
            {
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                DocOntologia = docOntologia
            }).SelectMany(item => item.DocOntologia.DefaultIfEmpty(), (objetos, agrup) => new DocumentoWebVinBaseRecursosDocumentoDocumento
            {
                Documento = objetos.Documento,
                DocumentoWebVinBaseRecursos = objetos.DocumentoWebVinBaseRecursos,
                DocOntolologia = agrup
            });
        }


        public static IQueryable<DocumentoWebVinBaseRecursosDocumentoDocumentoDocumentoWebVinBaseRecursosExtra> JoinDocumentoWebVinBaseRecursosExtra(this IQueryable<DocumentoWebVinBaseRecursosDocumentoDocumento> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.DocumentoWebVinBaseRecursosExtra, item => new { DocumentoID = item.DocumentoWebVinBaseRecursos.DocumentoID, BaseRecursosID = item.DocumentoWebVinBaseRecursos.BaseRecursosID }, doc => new
            { DocumentoID = doc.DocumentoID, BaseRecursosID = doc.BaseRecursosID }, (item, doc) => new DocumentoWebVinBaseRecursosDocumentoDocumentoDocumentoWebVinBaseRecursosExtra
            {
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                DocOntolologia = item.DocOntolologia,
                DocumentoWebVinBaseRecursosExtra = doc
            });
        }

        public static IQueryable<DocumentoWebVinBaseRecursosDocumentoBaseRecursosProyectoDocumentoDocumentoWebVinBaseRecursosExtra> JoinDocumentoWebVinBaseRecursosExtra(this IQueryable<DocumentoWebVinBaseRecursosDocumentoBaseRecursosProyectoDocumento> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.DocumentoWebVinBaseRecursosExtra, item => new { DocumentoID = item.DocumentoWebVinBaseRecursos.DocumentoID, BaseRecursosID = item.DocumentoWebVinBaseRecursos.BaseRecursosID }, doc => new
            { DocumentoID = doc.DocumentoID, BaseRecursosID = doc.BaseRecursosID }, (item, doc) => new DocumentoWebVinBaseRecursosDocumentoBaseRecursosProyectoDocumentoDocumentoWebVinBaseRecursosExtra
            {
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                DocOntolologia = item.DocOntolologia,
                DocumentoWebVinBaseRecursosExtra = doc,
                BaseRecursosProyecto = item.BaseRecursosProyecto
            });
        }

        public static IQueryable<DocumentoWebVinBaseRecursosDocumentoBaseRecursosProyectoDocumento> LeftJoinDocumento(this IQueryable<DocumentoWebVinBaseRecursosDocumentoBaseRecursosProyecto> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.GroupJoin(entityContext.Documento, item => new { DocumentoID = item.Documento.ElementoVinculadoID.Value }, docOntologia => new
            { DocumentoID = docOntologia.DocumentoID }, (item, docOntologia) => new
            {
                Documento = item.Documento,
                DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                DocOntologia = docOntologia,
                BaseRecursosProyecto = item.BaseRecursosProyecto
            }).SelectMany(item => item.DocOntologia.DefaultIfEmpty(), (objetos, agrup) => new DocumentoWebVinBaseRecursosDocumentoBaseRecursosProyectoDocumento
            {
                Documento = objetos.Documento,
                DocumentoWebVinBaseRecursos = objetos.DocumentoWebVinBaseRecursos,
                DocOntolologia = agrup,
                BaseRecursosProyecto = objetos.BaseRecursosProyecto
            });
        }

        public static IQueryable<DocumentoWebVinBaseRecursosDocumentoBaseRecursosOrganizacion> JoinBaseRecursosOrganizacion(this IQueryable<DocumentoWebVinBaseRecursosDocumento> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.BaseRecursosOrganizacion, objeto => objeto.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecOrg => baseRecOrg.BaseRecursosID, (objeto, baseRecOrg) => new DocumentoWebVinBaseRecursosDocumentoBaseRecursosOrganizacion
            {
                BaseRecursoOrganizacion = baseRecOrg,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                Documento = objeto.Documento
            });
        }

        public static IQueryable<DocumentoWebVinBaseRecursosDocumentoBaseRecursosProyecto> JoinBaseRecursosProyecto(this IQueryable<DocumentoWebVinBaseRecursosDocumento> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.BaseRecursosProyecto, objeto => objeto.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecProy => baseRecProy.BaseRecursosID, (objeto, baseRecProy) => new DocumentoWebVinBaseRecursosDocumentoBaseRecursosProyecto
            {
                BaseRecursosProyecto = baseRecProy,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                Documento = objeto.Documento
            });
        }

        public static IQueryable<DocumentoWebVinBaseRecursosDocumentoBaseRecursosProyectoDocumentoWebVinBaseRecursosExtra> JoinDocumentoWebVinBaseRecursosExtra(this IQueryable<DocumentoWebVinBaseRecursosDocumentoBaseRecursosProyecto> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.DocumentoWebVinBaseRecursosExtra, objeto => new { objeto.DocumentoWebVinBaseRecursos.DocumentoID, objeto.DocumentoWebVinBaseRecursos.BaseRecursosID }, docWebVinExtra => new { docWebVinExtra.DocumentoID, docWebVinExtra.BaseRecursosID }, (objeto, docWebVinExtra) => new DocumentoWebVinBaseRecursosDocumentoBaseRecursosProyectoDocumentoWebVinBaseRecursosExtra
            {
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                Documento = objeto.Documento,
                BaseRecursosProyecto = objeto.BaseRecursosProyecto,
                DocumentoWebVinBaseRecursosExtra = docWebVinExtra
            });
        }

        public static IQueryable<DocumentoWebVinBaseRecursosDocumentoBaseRecursosProyectoProyecto> JoinProyecto(this IQueryable<DocumentoWebVinBaseRecursosDocumentoBaseRecursosProyecto> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Proyecto, objeto => objeto.BaseRecursosProyecto.ProyectoID, proyecto => proyecto.ProyectoID, (objeto, proyecto) => new DocumentoWebVinBaseRecursosDocumentoBaseRecursosProyectoProyecto
            {
                BaseRecursosProyecto = objeto.BaseRecursosProyecto,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                Documento = objeto.Documento,
                Proyecto = proyecto
            });
        }

        public static IQueryable<DocumentoRolIdentidadDocumentoWebVinBaseRecursos> JoinDocumentoRolIdentidadDocumentoWebVinBaseRecursos(this IQueryable<DocumentoRolIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return entityContext.DocumentoRolIdentidad.Join(entityContext.DocumentoWebVinBaseRecursos, docRolId => docRolId.DocumentoID, webVin => webVin.DocumentoID, (docRolIden, webVin) => new DocumentoRolIdentidadDocumentoWebVinBaseRecursos
            {
                DocumentoRolIdentidad = docRolIden,
                DocumentoWebVinBaseRecursos = webVin
            });
        }

        public static IQueryable<DocumentoRolIdentidadDocumentoWebVinBaseRecursosBaseRecursosProyecto> JoinDocumentoRolIdentidadDocumentoWebVinBaseRecursos(this IQueryable<DocumentoRolIdentidadDocumentoWebVinBaseRecursos> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.BaseRecursosProyecto, objeto => objeto.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRec => baseRec.BaseRecursosID, (objeto, baseRec) => new DocumentoRolIdentidadDocumentoWebVinBaseRecursosBaseRecursosProyecto
            {
                DocumentoRolIdentidad = objeto.DocumentoRolIdentidad,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = baseRec
            });
        }


        public static IQueryable<DocumentoDocumentoRolIdentidad> JoinDocumentoDocumentoRolIdentidad(this IQueryable<Documento> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return entityContext.Documento.Join(entityContext.DocumentoRolIdentidad, doc => doc.DocumentoID, docRolIden => docRolIden.DocumentoID, (doc, docRolIden) => new DocumentoDocumentoRolIdentidad
            {
                Documento = doc,
                DocumentoRolIdentidad = docRolIden
            });
        }
        public static IQueryable<DocumentoDocumentoRolIdentidadDocumentoWebVinBaseRecursos> JoinDocumentoWebVinBaseRecursos(this IQueryable<DocumentoDocumentoRolIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.DocumentoWebVinBaseRecursos, objeto => objeto.Documento.DocumentoID, docWebVin => docWebVin.DocumentoID, (objeto, docWebVin) => new DocumentoDocumentoRolIdentidadDocumentoWebVinBaseRecursos
            {
                Documento = objeto.Documento,
                DocumentoRolIdentidad = objeto.DocumentoRolIdentidad,
                DocumentoWebVinBaseRecursos = docWebVin
            });
        }

        public static IQueryable<DocumentoDocumentoRolIdentidadIdentidad> JoinIdentidad(this IQueryable<DocumentoDocumentoRolIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, objeto => objeto.DocumentoRolIdentidad.PerfilID, idenEditor => idenEditor.PerfilID, (objeto, idenEditor) => new DocumentoDocumentoRolIdentidadIdentidad
            {
                Documento = objeto.Documento,
                DocumentoRolIdentidad = objeto.DocumentoRolIdentidad,
                Identidad = idenEditor
            });
        }

        public static IQueryable<DocumentoDocumentoRolIdentidadIdentidadProyectoUsuarioIdentidad> JoinProyectoUsuarioIdentidad(this IQueryable<DocumentoDocumentoRolIdentidadIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.ProyectoUsuarioIdentidad, objeto => objeto.Identidad.IdentidadID, proyUserIden => proyUserIden.IdentidadID, (objeto, proyUserIden) => new DocumentoDocumentoRolIdentidadIdentidadProyectoUsuarioIdentidad
            {
                Documento = objeto.Documento,
                DocumentoRolIdentidad = objeto.DocumentoRolIdentidad,
                Identidad = objeto.Identidad,
                ProyectoUsuarioIdentidad = proyUserIden
            });
        }

        public static IQueryable<DocumentoDocumentoRolIdentidadDocumentoWebVinBaseRecursos> JoinDocumentoRolIdentidad(this IQueryable<DocumentoWebVinBaseRecursosDocumento> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.DocumentoRolIdentidad, objeto => objeto.DocumentoWebVinBaseRecursos.DocumentoID, docRolIden => docRolIden.DocumentoID, (objeto, docRolIden) => new DocumentoDocumentoRolIdentidadDocumentoWebVinBaseRecursos
            {
                Documento = objeto.Documento,
                DocumentoRolIdentidad = docRolIden,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos
            });
        }

        public static IQueryable<DocumentoDocumentoRolIdentidadDocumentoWebVinBaseRecursosBaseRecursosProyecto> JoinBaseRecursosProyecto(this IQueryable<DocumentoDocumentoRolIdentidadDocumentoWebVinBaseRecursos> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.BaseRecursosProyecto, objeto => objeto.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRec => baseRec.BaseRecursosID, (objeto, baseRec) => new DocumentoDocumentoRolIdentidadDocumentoWebVinBaseRecursosBaseRecursosProyecto
            {
                Documento = objeto.Documento,
                DocumentoRolIdentidad = objeto.DocumentoRolIdentidad,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = baseRec
            });
        }

        public static IQueryable<DocumentoDocumentoRolIdentidadDocumentoWebVinBaseRecursosIdentidad> JoinIdentidad(this IQueryable<DocumentoDocumentoRolIdentidadDocumentoWebVinBaseRecursos> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, objeto => objeto.DocumentoRolIdentidad.PerfilID, identidad => identidad.PerfilID, (objeto, identidad) => new DocumentoDocumentoRolIdentidadDocumentoWebVinBaseRecursosIdentidad
            {
                Documento = objeto.Documento,
                DocumentoRolIdentidad = objeto.DocumentoRolIdentidad,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                Identidad = identidad
            });
        }

        public static IQueryable<DocumentoWebVinBaseRecursosDocumentoIdentidad> JoinIdentidad(this IQueryable<DocumentoWebVinBaseRecursosDocumento> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Identidad, objeto => objeto.DocumentoWebVinBaseRecursos.IdentidadPublicacionID.Value, identidad => identidad.IdentidadID, (objeto, identidad) => new DocumentoWebVinBaseRecursosDocumentoIdentidad
            {
                Documento = objeto.Documento,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                Identidad = identidad
            }).Where(objeto => objeto.DocumentoWebVinBaseRecursos.IdentidadPublicacionID.HasValue);
        }

        public static IQueryable<DocumentoIdentidad> JoinDocumentoIdentidad(this IQueryable<Documento> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return entityContext.Documento.Join(entityContext.Identidad, documento => documento.CreadorID.Value, identidad => identidad.IdentidadID, (documento, identidad) => new DocumentoIdentidad
            {
                Documento = documento,
                Identidad = identidad
            }).Where(objeto => objeto.Documento.CreadorID.HasValue);
        }

        public static IQueryable<DocumentoIdentidadPerfil> JoinPerfil(this IQueryable<DocumentoIdentidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Perfil, objeto => objeto.Identidad.PerfilID, perfil => perfil.PerfilID, (objeto, perfil) => new DocumentoIdentidadPerfil
            {
                Documento = objeto.Documento,
                Identidad = objeto.Identidad,
                Perfil = perfil
            });
        }

        public static IQueryable<DocumentoIdentidadPerfilPersona> JoinPersona(this IQueryable<DocumentoIdentidadPerfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Persona, objeto => objeto.Perfil.PersonaID.Value, persona => persona.PersonaID, (objeto, persona) => new DocumentoIdentidadPerfilPersona
            {
                Documento = objeto.Documento,
                Identidad = objeto.Identidad,
                Perfil = objeto.Perfil,
                Persona = persona
            }).Where(objeto => objeto.Perfil.PersonaID.HasValue);
        }

        public static IQueryable<DocumentoIdentidadPerfilPersonaProyecto> JoinProyecto(this IQueryable<DocumentoIdentidadPerfilPersona> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Proyecto, objeto => objeto.Documento.ProyectoID.Value, proyecto => proyecto.ProyectoID, (objeto, proyecto) => new DocumentoIdentidadPerfilPersonaProyecto
            {
                Documento = objeto.Documento,
                Identidad = objeto.Identidad,
                Perfil = objeto.Perfil,
                Persona = objeto.Persona,
                Proyecto = proyecto
            }).Where(objeto => objeto.Documento.ProyectoID.HasValue);
        }

        public static IQueryable<DocumentoWebVinBaseRecursosBaseRecursosProyecto> JoinDocumentoWebVinBaseRecursosBaseRecursosProyecto(this IQueryable<DocumentoWebVinBaseRecursos> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return entityContext.DocumentoWebVinBaseRecursos.Join(entityContext.BaseRecursosProyecto, docWebVin => docWebVin.BaseRecursosID, baseRecProy => baseRecProy.BaseRecursosID, (docWebVin, baseRecProy) => new DocumentoWebVinBaseRecursosBaseRecursosProyecto
            {
                BaseRecursosProyecto = baseRecProy,
                DocumentoWebVinBaseRecursos = docWebVin
            });
        }

        public static IQueryable<DocumentoWebVinBaseRecursosBaseRecursosOrganizacion> JoinDocumentoWebVinBaseRecursosBaseRecursosOrganizacion(this IQueryable<DocumentoWebVinBaseRecursos> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return entityContext.DocumentoWebVinBaseRecursos.Join(entityContext.BaseRecursosOrganizacion, docWebVin => docWebVin.BaseRecursosID, baseRecOrg => baseRecOrg.BaseRecursosID, (docWebVin, baseRecOrg) => new DocumentoWebVinBaseRecursosBaseRecursosOrganizacion
            {
                BaseRecursosOrganizacion = baseRecOrg,
                DocumentoWebVinBaseRecursos = docWebVin
            });
        }

        public static IQueryable<DocumentoIdentidadPerfilAdministradorOrganizacion> JoinAdministradorOrganizacion(this IQueryable<DocumentoIdentidadPerfil> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.AdministradorOrganizacion, objeto => objeto.Perfil.OrganizacionID, admin => admin.OrganizacionID, (objeto, admin) => new DocumentoIdentidadPerfilAdministradorOrganizacion
            {
                AdministradorOrganizacion = admin,
                Documento = objeto.Documento,
                Identidad = objeto.Identidad,
                Perfil = objeto.Perfil
            });
        }

        public static IQueryable<DocumentoWebVinBaseRecursosBaseRecursos> JoinDocumentoWebVinBaseRecursosBaseRecursos(this IQueryable<DocumentoWebVinBaseRecursos> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return entityContext.DocumentoWebVinBaseRecursos.Join(entityContext.BaseRecursos, docWebVin => docWebVin.BaseRecursosID, baseRec => baseRec.BaseRecursosID, (docWebVin, baseRec) => new DocumentoWebVinBaseRecursosBaseRecursos
            {
                BaseRecursos = baseRec,
                DocumentoWebVinBaseRecursos = docWebVin
            });
        }

        public static IQueryable<DocumentoWebVinBaseRecursosBaseRecursosDocumento> JoinDocumento(this IQueryable<DocumentoWebVinBaseRecursosBaseRecursos> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Documento, objeto => objeto.DocumentoWebVinBaseRecursos.DocumentoID, documento => documento.DocumentoID, (objeto, documento) => new DocumentoWebVinBaseRecursosBaseRecursosDocumento
            {
                BaseRecursos = objeto.BaseRecursos,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                Documento = documento

            });
        }

        public static IQueryable<DocumentoWebVinBaseRecursosBaseRecursosProyectoDocumento> JoinDocumento(this IQueryable<DocumentoWebVinBaseRecursosBaseRecursosProyecto> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Documento, objeto => objeto.DocumentoWebVinBaseRecursos.DocumentoID, documento => documento.DocumentoID, (objeto, documento) => new DocumentoWebVinBaseRecursosBaseRecursosProyectoDocumento
            {
                BaseRecursosProyecto = objeto.BaseRecursosProyecto,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                Documento = documento

            });
        }

        public static IQueryable<DocumentoWebVinBaseRecursosBaseRecursosUsuario> JoinDocumentoWebVinBaseRecursosBaseRecursosUsuario(this IQueryable<DocumentoWebVinBaseRecursos> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return entityContext.DocumentoWebVinBaseRecursos.Join(entityContext.BaseRecursosUsuario, docWebVin => docWebVin.BaseRecursosID, baseRecUs => baseRecUs.BaseRecursosID, (docWebVin, baseRecUs) => new DocumentoWebVinBaseRecursosBaseRecursosUsuario
            {
                BaseRecursosUsuario = baseRecUs,
                DocumentoWebVinBaseRecursos = docWebVin
            });
        }

        public static IQueryable<DocumentoWebVinBaseRecursosBaseRecursosUsuarioDocumento> JoinDocumento(this IQueryable<DocumentoWebVinBaseRecursosBaseRecursosUsuario> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Documento, objeto => objeto.DocumentoWebVinBaseRecursos.DocumentoID, documento => documento.DocumentoID, (objeto, documento) => new DocumentoWebVinBaseRecursosBaseRecursosUsuarioDocumento
            {
                BaseRecursosUsuario = objeto.BaseRecursosUsuario,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                Documento = documento

            });
        }

        public static IQueryable<DocumentoWebVinBaseRecursosBaseRecursosUsuarioDocumentoPersona> JoinPersona(this IQueryable<DocumentoWebVinBaseRecursosBaseRecursosUsuarioDocumento> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Persona, objeto => objeto.BaseRecursosUsuario.UsuarioID, persona => persona.UsuarioID, (objeto, persona) => new DocumentoWebVinBaseRecursosBaseRecursosUsuarioDocumentoPersona
            {
                BaseRecursosUsuario = objeto.BaseRecursosUsuario,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                Documento = objeto.Documento,
                Persona = persona
            });
        }

        public static IQueryable<IdentidadDocumentoWebVinBaseRecursos> JoinIdentidadDocumentoWebVinBaseRecursos(this IQueryable<EntityModel.Models.IdentidadDS.Identidad> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return entityContext.Identidad.Join(entityContext.DocumentoWebVinBaseRecursos, identidad => new { IdentidadID = identidad.IdentidadID }, docWebvin => new { IdentidadID = docWebvin.IdentidadPublicacionID.Value }, (identidad, docWebvin) => new IdentidadDocumentoWebVinBaseRecursos
            {
                Identidad = identidad,
                DocumentoWebVinBaseRecursos = docWebvin
            }).Where(objeto => objeto.DocumentoWebVinBaseRecursos.IdentidadPublicacionID.HasValue);
        }

        public static IQueryable<IdentidadDocumentoWebVinBaseRecursosDocumento> JoinDocumento(this IQueryable<IdentidadDocumentoWebVinBaseRecursos> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Documento, objeto => objeto.DocumentoWebVinBaseRecursos.DocumentoID, documento => documento.DocumentoID, (objeto, documento) => new IdentidadDocumentoWebVinBaseRecursosDocumento
            {
                Documento = documento,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                Identidad = objeto.Identidad
            });
        }

        public static IQueryable<DocumentoWebAgCatTesauroBaseRecursosProyecto> JoinDocumentoWebAgCatTesauroBaseRecursosProyecto(this IQueryable<DocumentoWebAgCatTesauro> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return entityContext.DocumentoWebAgCatTesauro.Join(entityContext.BaseRecursosProyecto, doc => doc.BaseRecursosID, baseRecProy => baseRecProy.BaseRecursosID, (doc, baseRecProy) => new DocumentoWebAgCatTesauroBaseRecursosProyecto
            {
                BaseRecursosProyecto = baseRecProy,
                DocumentoWebAgCatTesauro = doc
            });
        }

        public static IQueryable<DocumentoWebAgCatTesauroDocumentoWebVinBaseRecursos> JoinDocumentoWebAgCatTesauroDocumentoWebVinBaseRecursos(this IQueryable<DocumentoWebAgCatTesauro> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return entityContext.DocumentoWebAgCatTesauro.Join(entityContext.DocumentoWebVinBaseRecursos, documentoWebAgCatTesauro => new { documentoWebAgCatTesauro.DocumentoID, documentoWebAgCatTesauro.BaseRecursosID }, doc => new { doc.DocumentoID, doc.BaseRecursosID }, (documentoWebAgCatTesauro, doc) => new DocumentoWebAgCatTesauroDocumentoWebVinBaseRecursos
            {
                DocumentoWebAgCatTesauro = documentoWebAgCatTesauro,
                DocumentoWebVinBaseRecursos = doc
            });
        }

        public static IQueryable<DocumentoWebAgCatTesauroDocumentoWebVinBaseRecursosBaseRecursosOrganizacion> JoinBaseRecursosOrganizacion(this IQueryable<DocumentoWebAgCatTesauroDocumentoWebVinBaseRecursos> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.BaseRecursosOrganizacion, objeto => objeto.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecOrg => baseRecOrg.BaseRecursosID, (objeto, baseRecOrg) => new DocumentoWebAgCatTesauroDocumentoWebVinBaseRecursosBaseRecursosOrganizacion
            {
                BaseRecursosOrganizacion = baseRecOrg,
                DocumentoWebAgCatTesauro = objeto.DocumentoWebAgCatTesauro,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos
            });
        }

        public static IQueryable<DocumentoWebAgCatTesauroDocumentoWebVinBaseRecursosDocumento> JoinDocumento(this IQueryable<DocumentoWebAgCatTesauroDocumentoWebVinBaseRecursos> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Documento, objeto => objeto.DocumentoWebVinBaseRecursos.DocumentoID, documento => documento.DocumentoID, (objeto, documento) => new DocumentoWebAgCatTesauroDocumentoWebVinBaseRecursosDocumento
            {
                Documento = documento,
                DocumentoWebAgCatTesauro = objeto.DocumentoWebAgCatTesauro,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos
            });
        }

        public static IQueryable<DocumentoWebVinBaseRecursosDocumentoWebVinBaseRecursosExtra> JoinDocumentoWebVinBaseRecursosDocumentoWebVinBaseRecursosExtra(this IQueryable<DocumentoWebVinBaseRecursos> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return entityContext.DocumentoWebVinBaseRecursos.Join(entityContext.DocumentoWebVinBaseRecursosExtra, docWebVin => new { docWebVin.DocumentoID, docWebVin.BaseRecursosID }, docWebVinExtra => new { docWebVinExtra.DocumentoID, docWebVinExtra.BaseRecursosID }, (docWebVin, docWebVinExtra) => new DocumentoWebVinBaseRecursosDocumentoWebVinBaseRecursosExtra
            {
                DocumentoWebVinBaseRecursos = docWebVin,
                DocumentoWebVinBaseRecursosExtra = docWebVinExtra
            });
        }

        public static IQueryable<BaseRecursosProyectoDocumentoWebVinBaseRecursosExtra> JoinBaseRecursosProyectoDocumentoWebVinBaseRecursosExtra(this IQueryable<BaseRecursosProyecto> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return entityContext.BaseRecursosProyecto.Join(entityContext.DocumentoWebVinBaseRecursosExtra, baseRecProy => baseRecProy.BaseRecursosID, docWebVinExtra => docWebVinExtra.BaseRecursosID, (baseRecProy, docWebVinExtra) => new BaseRecursosProyectoDocumentoWebVinBaseRecursosExtra
            {
                BaseRecursosProyecto = baseRecProy,
                DocumentoWebVinBaseRecursosExtra = docWebVinExtra
            });
        }

        public static IQueryable<DocumentoWebVinBaseRecursosBaseRecursosUsuarioPersona> JoinPersona(this IQueryable<DocumentoWebVinBaseRecursosBaseRecursosUsuario> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Persona, objeto => objeto.BaseRecursosUsuario.UsuarioID, persona => persona.UsuarioID, (objeto, persona) => new DocumentoWebVinBaseRecursosBaseRecursosUsuarioPersona
            {
                BaseRecursosUsuario = objeto.BaseRecursosUsuario,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                Persona = persona
            });
        }

        public static IQueryable<DocumentoWebVinBaseRecursosBaseRecursosUsuarioPersonaPerfil> JoinPerfil(this IQueryable<DocumentoWebVinBaseRecursosBaseRecursosUsuarioPersona> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.Perfil, objeto => objeto.Persona.PersonaID, perfil => perfil.PersonaID, (objeto, perfil) => new DocumentoWebVinBaseRecursosBaseRecursosUsuarioPersonaPerfil
            {
                BaseRecursosUsuario = objeto.BaseRecursosUsuario,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                Persona = objeto.Persona,
                Perfil = perfil
            });
        }

        public static IQueryable<DocumentoProyecto> JoinDocumentoProyecto(this IQueryable<Documento> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return entityContext.Documento.Join(entityContext.Proyecto, doc => new { ProyectoID = doc.ProyectoID.Value }, proy => new { ProyectoID = proy.ProyectoID }, (doc, proy) => new DocumentoProyecto
            {
                Documento = doc,
                Proyecto = proy
            }).Where(objeto => objeto.Documento.ProyectoID.HasValue);
        }

        public static IQueryable<DocumentoDocumento> JoinDocumentoDocumento(this IQueryable<Documento> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return entityContext.Documento.Join(entityContext.Documento, doc => new { DocumentoID = doc.ElementoVinculadoID.Value }, docuVin => new { DocumentoID = docuVin.DocumentoID }, (doc, docuVin) => new DocumentoDocumento
            {
                Documento = doc,
                DocumentoVinculado = docuVin
            }).Where(objeto => objeto.Documento.ProyectoID.HasValue);
        }

        public static IQueryable<DocumentoDocumentoRolGrupoIdentidades> JoinDocumentoDocumentoRolGrupoIdentidades(this IQueryable<Documento> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return entityContext.Documento.Join(entityContext.DocumentoRolGrupoIdentidades, documento => documento.DocumentoID, docRolGrupId => docRolGrupId.DocumentoID, (documento, docRolGrupId) => new DocumentoDocumentoRolGrupoIdentidades
            {
                Documento = documento,
                DocumentoRolGrupoIdentidades = docRolGrupId
            });
        }

        public static IQueryable<DocumentoDocumentoRolGrupoIdentidadesGrupoIdentidades> JoinGrupoIdentidades(this IQueryable<DocumentoDocumentoRolGrupoIdentidades> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.GrupoIdentidades, objeto => objeto.DocumentoRolGrupoIdentidades.GrupoID, grupoIdentidades => grupoIdentidades.GrupoID, (objeto, grupoIdentidades) => new DocumentoDocumentoRolGrupoIdentidadesGrupoIdentidades
            {
                Documento = objeto.Documento,
                DocumentoRolGrupoIdentidades = objeto.DocumentoRolGrupoIdentidades,
                GrupoIdentidades = grupoIdentidades
            });
        }

        public static IQueryable<DocumentoDocumentoRolGrupoIdentidadesGrupoIdentidadesParticipacion> JoinGrupoIdentidadesParticipacion(this IQueryable<DocumentoDocumentoRolGrupoIdentidades> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.GrupoIdentidadesParticipacion, objeto => objeto.DocumentoRolGrupoIdentidades.GrupoID, grupIdPart => grupIdPart.GrupoID, (objeto, grupIdPart) => new DocumentoDocumentoRolGrupoIdentidadesGrupoIdentidadesParticipacion
            {
                Documento = objeto.Documento,
                DocumentoRolGrupoIdentidades = objeto.DocumentoRolGrupoIdentidades,
                GrupoIdentidadesParticipacion = grupIdPart
            });
        }
    }

    public class DocumentoWebVinBaseRecursosDocumentoBaseRecursosOrganizacion
    {
        public BaseRecursosOrganizacion BaseRecursoOrganizacion { get; set; }
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }

    public class DocumentoWebVinBaseRecursosDocumentoBaseRecursosProyecto
    {
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }

    public class DocumentoWebVinBaseRecursosDocumentoBaseRecursosProyectoDocumento
    {
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public Documento DocOntolologia { get; set; }
    }
    public class DocumentoWebVinBaseRecursosDocumentoBaseRecursosProyectoDocumentoDocumentoWebVinBaseRecursosExtra
    {
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public Documento DocOntolologia { get; set; }
        public DocumentoWebVinBaseRecursosExtra DocumentoWebVinBaseRecursosExtra { get; set; }
    }

    public class DocumentoWebVinBaseRecursosDocumentoBaseRecursosProyectoDocumentoWebVinBaseRecursosExtra
    {
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public DocumentoWebVinBaseRecursosExtra DocumentoWebVinBaseRecursosExtra { get; set; }
    }

    public class DocumentoWebVinBaseRecursosDocumento
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }

    public class DocumentoWebVinBaseRecursosDocumentoDocumento
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public Documento DocOntolologia { get; set; }
    }

    public class DocumentoWebVinBaseRecursosDocumentoDocumentoDocumentoWebVinBaseRecursosExtra
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public Documento DocOntolologia { get; set; }
        public DocumentoWebVinBaseRecursosExtra DocumentoWebVinBaseRecursosExtra { get; set; }
    }

    public class IdentidadPerfil
    {
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class IdentidadPerfilDocumento
    {
        public Perfil Perfil { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }

        public Documento Documento { get; set; }
    }

    public class DocumentoRolIdentidadDocumentoWebVinBaseRecursos
    {
        public DocumentoRolIdentidad DocumentoRolIdentidad { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }

    public class DocumentoRolIdentidadDocumentoWebVinBaseRecursosBaseRecursosProyecto : DocumentoRolIdentidadDocumentoWebVinBaseRecursos
    {
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
    }

    public class DocumentoDocumentoRolIdentidad
    {
        public DocumentoRolIdentidad DocumentoRolIdentidad { get; set; }
        public Documento Documento { get; set; }
    }

    public class DocumentoDocumentoRolIdentidadDocumentoWebVinBaseRecursos
    {
        public DocumentoRolIdentidad DocumentoRolIdentidad { get; set; }
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }

    public class DocumentoDocumentoRolIdentidadDocumentoWebVinBaseRecursosBaseRecursosProyecto
    {
        public DocumentoRolIdentidad DocumentoRolIdentidad { get; set; }
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
    }

    public class DocumentoDocumentoRolIdentidadDocumentoWebVinBaseRecursosIdentidad
    {
        public DocumentoRolIdentidad DocumentoRolIdentidad { get; set; }
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class DocumentoWebVinBaseRecursosDocumentoIdentidad
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class DocumentoWebVinBaseRecursosDocumentoBaseRecursosProyectoProyecto
    {
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public EntityModel.Models.ProyectoDS.Proyecto Proyecto { get; set; }
    }

    public class DocumentoIdentidad
    {
        public Documento Documento { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class DocumentoIdentidadPerfil
    {
        public Documento Documento { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class DocumentoIdentidadPerfilPersona
    {
        public Documento Documento { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.PersonaDS.Persona Persona { get; set; }
    }

    public class DocumentoIdentidadPerfilPersonaProyecto
    {
        public Documento Documento { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.PersonaDS.Persona Persona { get; set; }
        public EntityModel.Models.ProyectoDS.Proyecto Proyecto { get; set; }
    }

    public class DocumentoWebVinBaseRecursosBaseRecursosProyecto
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
    }
    public class DocumentoWebVinBaseRecursosBaseRecursosOrganizacion
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosOrganizacion BaseRecursosOrganizacion { get; set; }
    }

    public class DocumentoWebVinBaseRecursosBaseRecursos
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursos BaseRecursos { get; set; }
    }

    public class DocumentoWebVinBaseRecursosBaseRecursosDocumento
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursos BaseRecursos { get; set; }
        public Documento Documento { get; set; }
    }

    public class DocumentoWebVinBaseRecursosBaseRecursosProyectoDocumento
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public Documento Documento { get; set; }
    }

    public class DocumentoWebVinBaseRecursosBaseRecursosUsuario
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
    }

    public class DocumentoWebVinBaseRecursosBaseRecursosUsuarioDocumento
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
        public Documento Documento { get; set; }
    }

    public class DocumentoWebVinBaseRecursosBaseRecursosUsuarioDocumentoPersona
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
        public Documento Documento { get; set; }
        public EntityModel.Models.PersonaDS.Persona Persona { get; set; }
    }

    public class IdentidadDocumentoWebVinBaseRecursos
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }

    public class IdentidadDocumentoWebVinBaseRecursosDocumento
    {
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public Documento Documento { get; set; }
    }

    public class DocumentoWebAgCatTesauroBaseRecursosProyecto
    {
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
    }

    public class DocumentoWebVinBaseRecursosDocumentoDocumentoWebAgCatTesauro
    {
        public Documento Documento { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
    }

    public class DocumentoWebAgCatTesauroDocumentoWebVinBaseRecursos
    {
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }
    public class DocumentoWebAgCatTesauroDocumentoWebVinBaseRecursosBaseRecursosOrganizacion
    {
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosOrganizacion BaseRecursosOrganizacion { get; set; }
    }
    public class DocumentoWebAgCatTesauroDocumentoWebVinBaseRecursosDocumento
    {
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public Documento Documento { get; set; }
    }

    public class DocumentoWebVinBaseRecursosDocumentoWebVinBaseRecursosExtra
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public DocumentoWebVinBaseRecursosExtra DocumentoWebVinBaseRecursosExtra { get; set; }
    }

    public class BaseRecursosProyectoDocumentoWebVinBaseRecursosExtra
    {
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public DocumentoWebVinBaseRecursosExtra DocumentoWebVinBaseRecursosExtra { get; set; }
    }

    public class DocumentoWebVinBaseRecursosBaseRecursosUsuarioPersona
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
        public EntityModel.Models.PersonaDS.Persona Persona { get; set; }
    }

    public class DocumentoWebVinBaseRecursosBaseRecursosUsuarioPersonaPerfil
    {
        public DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
        public EntityModel.Models.PersonaDS.Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class DocumentoProyecto
    {
        public Documento Documento { get; set; }
        public EntityModel.Models.ProyectoDS.Proyecto Proyecto { get; set; }
    }
    public class DocumentoDocumento
    {
        public Documento Documento { get; set; }
        public Documento DocumentoVinculado { get; set; }
    }
    

    public class DocumentoDocumentoRolGrupoIdentidades
    {
        public Documento Documento { get; set; }
        public DocumentoRolGrupoIdentidades DocumentoRolGrupoIdentidades { get; set; }
    }

    public class DocumentoDocumentoRolGrupoIdentidadesGrupoIdentidadesParticipacion
    {
        public Documento Documento { get; set; }
        public DocumentoRolGrupoIdentidades DocumentoRolGrupoIdentidades { get; set; }
        public GrupoIdentidadesParticipacion GrupoIdentidadesParticipacion { get; set; }
    }

    public class DocumentoDocumentoRolGrupoIdentidadesGrupoIdentidades
    {
        public Documento Documento { get; set; }
        public DocumentoRolGrupoIdentidades DocumentoRolGrupoIdentidades { get; set; }
        public GrupoIdentidades GrupoIdentidades { get; set; }
    }

    public class DocumentoDocumentoRolIdentidadIdentidad
    {
        public DocumentoRolIdentidad DocumentoRolIdentidad { get; set; }
        public Documento Documento { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class DocumentoDocumentoRolIdentidadIdentidadProyectoUsuarioIdentidad
    {
        public DocumentoRolIdentidad DocumentoRolIdentidad { get; set; }
        public Documento Documento { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public EntityModel.Models.UsuarioDS.ProyectoUsuarioIdentidad ProyectoUsuarioIdentidad { get; set; }
    }

    public class DocumentoIdentidadPerfilAdministradorOrganizacion
    {
        public Documento Documento { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
        public EntityModel.Models.UsuarioDS.AdministradorOrganizacion AdministradorOrganizacion { get; set; }
    }
}
