using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.BASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Es.Riam.Gnoss.Logica.Facetado
{
    public class ActualizacionFacetadoCN : IDisposable
    {

        #region Miembros

        /// <summary>
        /// DataAdapter de Blog
        /// </summary>
        private ActualizacionFacetadoAD mActualizacionFacetadoAD;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        public ActualizacionFacetadoCN(string pUrlIntragnoss, EntityContext entityContext, LoggingService loggingService, ConfigService configService, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            this.mActualizacionFacetadoAD = new ActualizacionFacetadoAD(pUrlIntragnoss, loggingService, entityContext, configService, virtuosoAD, servicesUtilVirtuosoAndReplication);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pFicheroConfiguracionBD"></param>
        /// <param name="pUsarVariableEstatica"></param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        public ActualizacionFacetadoCN(string pFicheroConfiguracionBD, string pUrlIntragnoss, EntityContext entityContext, LoggingService loggingService, ConfigService configService, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            mActualizacionFacetadoAD = new ActualizacionFacetadoAD(pFicheroConfiguracionBD, pUrlIntragnoss, loggingService, entityContext, configService, virtuosoAD, servicesUtilVirtuosoAndReplication);
        }
        #endregion


        #region Metodos generales

        public List<QueryTriples> ObtieneInformacionExtraContactos(string ididentidad, string ididentidadAmigo)
        {
            return mActualizacionFacetadoAD.ObtieneInformacionExtraContactos(ididentidad, ididentidadAmigo);
        }

        public List<QueryTriples> ObtieneInformacionGeneralRecursoPersonal(Guid pDocumentoID)
        {
            return mActualizacionFacetadoAD.ObtieneInformacionGeneralRecursoPersonal(pDocumentoID);
        }

        public List<QueryTriples> ObtieneInformacionGeneralRecursoOrganizacion(Guid id)
        {
            return mActualizacionFacetadoAD.ObtieneInformacionGeneralRecursoOrganizacion(id);
        }

        public Guid ObtieneIDBlog(Guid pArticuloBlogID)
        {
            return mActualizacionFacetadoAD.ObtieneIDBlog(pArticuloBlogID);

        }

        public List<QueryTriples> ObtieneInformacionExtraInvitaciones(string pInvitacionID)
        {
            return mActualizacionFacetadoAD.ObtieneInformacionExtraInvitaciones(pInvitacionID);
        }

        /// <summary>
        /// Obtiene la información extra que hace falta para las facetas de suscripciones.
        /// </summary>
        /// <param name="FacetaDS">Data</param>
        /// <param name="pSuscripcionID"></param>
        /// <param name="pRecursoID"></param>
        public List<QueryTriples> ObtieneInformacionExtraSuscripciones(Guid pSuscripcionID, Guid pRecursoID)
        {
            return mActualizacionFacetadoAD.ObtieneInformacionExtraSuscripciones(pSuscripcionID, pRecursoID);
        }

        public void ObtieneInformacionExtraMensajesTo(FacetaDS tConfiguracion, string id, string ididentidad)
        {
            mActualizacionFacetadoAD.ObtieneInformacionExtraMensajesTo(tConfiguracion, id, ididentidad);
        }

        public void ObtieneInformacionExtraMensajesFrom(FacetaDS tConfiguracion, string id, string ididentidad)
        {
            mActualizacionFacetadoAD.ObtieneInformacionExtraMensajesFrom(tConfiguracion, id, ididentidad);
        }

        public void ObtieneInformacionExtraMensajesFromObtenerTo(FacetaDS tConfiguracion, string id, string ididentidad)
        {
            mActualizacionFacetadoAD.ObtieneInformacionExtraMensajesFromObtenerTo(tConfiguracion, id, ididentidad);
        }

        public void ObtieneInformacionMensajesLeidoNoLeido(FacetaDS tConfiguracion, string id, string ididentidad)
        {
            mActualizacionFacetadoAD.ObtieneInformacionMensajesLeidoNoLeido(tConfiguracion, id, ididentidad);
        }

        public Guid ObtenerIdUsuarioDesdeIdentidad(Guid pIdentidadID)
        {
            return mActualizacionFacetadoAD.ObtenerIdUsuarioDesdeIdentidad(pIdentidadID);
        }

        /// <summary>
        /// Obtiene el ID de usuario a partir de su perfil.
        /// </summary>
        /// <param name="pPerfilID">ID de perfil</param>
        /// <returns>ID de usuario</returns>
        public Guid obtenerIdusuarioDesdePerfil(Guid pPerfilID)
        {
            return mActualizacionFacetadoAD.ObtenerIdUsuarioDesdePerfil(pPerfilID);
        }

        public List<QueryTriples> ObtieneInformacionExtraComentarios(Guid pComentarioID)
        {
            return mActualizacionFacetadoAD.ObtieneInformacionExtraComentarios(pComentarioID);
        }

        public List<string> ObtenerTags(Guid id, string tipoitem, Guid idproy)
        {
            List<Guid> lista = new List<Guid>();
            lista.Add(id);
            return ObtenerTagsLista(lista, tipoitem, idproy)[id];
        }

        public Dictionary<Guid, List<string>> ObtenerTagsLista(List<Guid> listaID, string tipoitem, Guid idproy)
        {
            return mActualizacionFacetadoAD.ObtenerTagsLista(listaID, tipoitem, idproy);
        }

        public string ObtenerTituloEntradaBlog(Guid pIdEntradaBlog)
        {
            return mActualizacionFacetadoAD.ObtenerTituloEntradaBlog(pIdEntradaBlog);
        }

        public string ObtenerTituloBlog(Guid pIdBlog)
        {
            return mActualizacionFacetadoAD.ObtenerTituloBlog(pIdBlog);
        }

        public string ObtenerTituloProyecto(Guid pProyectoID)
        {
            return mActualizacionFacetadoAD.ObtenerTituloProyecto(pProyectoID);
        }

        public string ObtenerTituloMensaje(FacetaDS FacetaDS, Guid IDCorreo, String IDFrom, out string descripcion)
        {
            return mActualizacionFacetadoAD.ObtenerTituloMensaje(FacetaDS, IDCorreo, IDFrom, out descripcion);
        }

        /// <summary>
        /// Obtiene el título de una descripción.
        /// </summary>
        /// <param name="pSuscripcionID">ID de suscripción</param>
        /// <param name="pRecursoID">ID de recurso</param>
        /// <returns>Título de la suscripción</returns>
        public string ObtenerTituloSuscripcion(Guid pSuscripcionID, Guid pRecursoID)
        {
            return mActualizacionFacetadoAD.ObtenerTituloSuscripcion(pSuscripcionID, pRecursoID);
        }

        public string ObtenerTituloInvitacion(FacetaDS FacetaDS, Guid IDCorreo, String IDFrom)
        {
            string aux = "";
            return mActualizacionFacetadoAD.ObtenerTituloMensaje(FacetaDS, IDCorreo, IDFrom, out aux);
        }

        public string ObtenerTituloComentario(Guid pComentarioID)
        {
            return mActualizacionFacetadoAD.ObtenerTituloComentario(pComentarioID);
        }

        public string ObtenerDestinatarioMensaje(FacetaDS FacetaDS, string pCorreoID, string pFromID)
        {
            return mActualizacionFacetadoAD.ObtenerDestinatarioMensaje(FacetaDS, pCorreoID, pFromID);
        }

        public string ObtenerNombrePerfilIdentidad(Guid pIdentidadID)
        {
            return mActualizacionFacetadoAD.ObtenerNombrePerfilIdentidad(pIdentidadID);
        }

        public string ObtenerTituloContacto(Guid pContactoID)
        {
            return mActualizacionFacetadoAD.ObtenerTituloContacto(pContactoID);
        }

        public Dictionary<Guid, string> ObtenerDescripcionesRecursos(List<Guid> pListaIdRecursos)
        {
            return mActualizacionFacetadoAD.ObtenerDescripcionesRecursos(pListaIdRecursos);
        }

        public string ObtenerDescripcionRecurso(Guid IDRec)
        {
            List<Guid> listaID = new List<Guid>();
            listaID.Add(IDRec);
            return ObtenerDescripcionesRecursos(listaID)[IDRec];
        }

        public Dictionary<Guid, string> ObtenerTitulosRecursos(List<Guid> ListaIDRec)
        {
            return mActualizacionFacetadoAD.ObtenerTitulosRecursos(ListaIDRec);
        }

        public string ObtenerTituloRecurso(Guid IDRec)
        {
            List<Guid> listaID = new List<Guid>();
            listaID.Add(IDRec);
            return ObtenerTitulosRecursos(listaID)[IDRec];
        }

        public string ObtenerTituloGrupo(Guid pGrupoID)
        {
            return mActualizacionFacetadoAD.ObtenerTituloGrupo(pGrupoID);
        }

        public string ObtenerTituloOrganizacion(Guid pIdentidadID)
        {
            return mActualizacionFacetadoAD.ObtenerTituloOrganizacion(pIdentidadID);
        }

        public string ObtenerTituloPersona(Guid pIdentidadID)
        {
            return mActualizacionFacetadoAD.ObtenerTituloPersona(pIdentidadID);
        }

        public List<QueryTriples> ObtieneTripletasPrivacidadGrupos(Guid pGrupoID)
        {
            return mActualizacionFacetadoAD.ObtieneTripletasPrivacidadGrupos(pGrupoID);
        }

        public List<QueryTriples> ObtieneTripletasPrivacidadOrganizaciones(Guid id, Guid proyid)
        {
            return mActualizacionFacetadoAD.ObtieneTripletasPrivacidadOrganizaciones(id, proyid);
        }

        public List<QueryTriples> ObtieneTripletasPrivacidadPersonas(Guid pIdentidadID, Guid pProyectoID, List<AD.EntityModel.ParametroAplicacion> pParametrosApliacion)
        {
            return mActualizacionFacetadoAD.ObtieneTripletasPrivacidadPersonas(pIdentidadID, pProyectoID, pParametrosApliacion);
        }

        public string ObtieneNombreEditor(Guid pIdentidadID)
        {
            return mActualizacionFacetadoAD.ObtieneNombreEditor(pIdentidadID);
        }

        public List<QueryTriples> ObtieneInformacionExtraPersona(Guid pIdentidadID, Guid pProyectoID)
        {
            return mActualizacionFacetadoAD.ObtieneInformacionExtraPersona(pIdentidadID, pProyectoID);
        }

        public List<QueryTriples> ObtieneInformacionExtraPersonaContactos(Guid pIdentidadID, Guid pProyectoID)
        {
            return mActualizacionFacetadoAD.ObtieneInformacionExtraPersonaContactos(pIdentidadID, pProyectoID);
        }

        public List<QueryTriples> ObtenerIdentidadesSigueIdentidadDeProyecto(Guid pProyectoID, Guid pPersonaID)
        {
            return mActualizacionFacetadoAD.ObtenerIdentidadesSigueIdentidadDeProyecto(pProyectoID, pPersonaID);
        }

        public List<QueryTriples> ObtenerCategoriasTesauroDeProyectoSuscritaIdentidad(Guid pProyectoID, Guid pIdentidadID)
        {
            return mActualizacionFacetadoAD.ObtenerCategoriasTesauroDeProyectoSuscritaIdentidad(pProyectoID, pIdentidadID);
        }

        public List<QueryTriples> ObtenerIdentidadDatoExtraRegistroDeProyecto(Guid pProyectoID, Guid pIdentidadID)
        {
            return mActualizacionFacetadoAD.ObtenerIdentidadDatoExtraRegistroDeProyecto(pProyectoID, pIdentidadID);
        }

        public List<QueryTriples> ObtieneInformacionExtraOrganizacion(Guid pIdentidadID, Guid pProyectoID)
        {
            return mActualizacionFacetadoAD.ObtieneInformacionExtraOrganizacion(pIdentidadID, pProyectoID);
        }

        public List<QueryTriples> ObtieneInformacionExtraOrganizacionContactos(Guid pIdentidadID)
        {
            return mActualizacionFacetadoAD.ObtieneInformacionExtraOrganizacionContactos(pIdentidadID);
        }

        public Guid? ObtieneIDIdentidad(Guid IDproyecto, Guid IDpersona)
        {
            return ObtieneIDIdentidad(IDproyecto, IDpersona, false);
        }

        public Guid? ObtieneIDIdentidad(Guid IDproyecto, Guid IDpersona, bool pTraerEliminados)
        {
            return mActualizacionFacetadoAD.ObtieneIDIdentidad(IDproyecto, IDpersona, pTraerEliminados);
        }

        public Guid? ObtieneIDIdentidadOrg(Guid IDproyecto, Guid IDpersona)
        {
            return ObtieneIDIdentidadOrg(IDproyecto, IDpersona, false);
        }

        public Guid? ObtieneIDIdentidadOrg(Guid IDproyecto, Guid IDpersona, bool pTraerEliminados)
        {
            return mActualizacionFacetadoAD.ObtieneIDIdentidadOrg(IDproyecto, IDpersona, pTraerEliminados);
        }

        public void ObtieneInformacionPrivacidadRecursoMyGnoss(List<Guid> pListaIdRecursos)
        {
            mActualizacionFacetadoAD.ObtieneInformacionPrivacidadRecursoMyGnoss(pListaIdRecursos);
        }

        public void ObtieneInformacionPrivacidadRecursoMyGnoss(FacetaDS FacetaDS, Guid pRecursoId)
        {
            List<Guid> lista = new List<Guid>();
            lista.Add(pRecursoId);

            mActualizacionFacetadoAD.ObtieneInformacionPrivacidadRecursoMyGnoss(lista);
        }

        public List<QueryTriples> ObtieneInformacionComunRecurso(List<Guid> pListaIdrec, Guid pIdproy)
        {
            return mActualizacionFacetadoAD.ObtieneInformacionComunRecurso(pListaIdrec, pIdproy);
        }

        public List<QueryTriples> ObtieneInformacionComunRecurso(Guid idrec, Guid idproy)
        {
            List<Guid> lista = new List<Guid>();
            lista.Add(idrec);

            return mActualizacionFacetadoAD.ObtieneInformacionComunRecurso(lista, idproy);
        }

        public List<QueryTriples> ObtieneInformacionExtraRecurso(List<Guid> pListaIdRecursos, Guid pIdProyecto)
        {
            return mActualizacionFacetadoAD.ObtieneInformacionExtraRecurso(pListaIdRecursos, pIdProyecto);
        }

        public List<QueryTriples> ObtieneInformacionExtraRecurso(Guid idrec, Guid idproy)
        {
            List<Guid> lista = new List<Guid>();
            lista.Add(idrec);

            Dictionary<Guid, FacetaDS> pListaFacetaDS = new Dictionary<Guid, FacetaDS>();
            return mActualizacionFacetadoAD.ObtieneInformacionExtraRecurso(lista, idproy);
        }

        public List<QueryTriples> ObtieneInformacionExtraDebate(Guid idrec, Guid idproy)
        {
            return mActualizacionFacetadoAD.ObtieneInformacionExtraDebate(idrec, idproy);
        }

        public List<QueryTriples> ObtieneInformacionExtraPregunta(Guid idrec, Guid idproy)
        {
            return mActualizacionFacetadoAD.ObtieneInformacionExtraPregunta(idrec, idproy);
        }

        public List<QueryTriples> ObtieneInformacionExtraEncuesta(Guid idrec, Guid idproy)
        {
            return mActualizacionFacetadoAD.ObtieneInformacionExtraEncuesta(idrec, idproy);
        }

        public StringBuilder ObtieneInformacionExtraRecursosContribuciones(Guid idrec, Guid pPerfilid, Guid pProyectoID)
        {
            return mActualizacionFacetadoAD.ObtieneInformacionExtraRecursosContribuciones(new List<Guid>() { idrec }, pPerfilid, pProyectoID);
        }

        public StringBuilder ObtieneInformacionExtraRecursosContribuciones(List<Guid> listaIDrec, Guid pPerfilid, Guid pProyectoID)
        {
            return mActualizacionFacetadoAD.ObtieneInformacionExtraRecursosContribuciones(listaIDrec, pPerfilid, pProyectoID);
        }


        public List<QueryTriples> ObtieneInformacionExtraRecursosContribucionesPer(Guid pDocumentoID, Guid pPerfilID)
        {
            return mActualizacionFacetadoAD.ObtieneInformacionExtraRecursosContribucionesPer(pDocumentoID, pPerfilID);
        }


        public List<QueryTriples> ObtieneInformacionExtraRecursosContribucionesOrg(Guid pDocumentoID, Guid pOrganizaionID)
        {
            return mActualizacionFacetadoAD.ObtieneInformacionExtraRecursosContribucionesOrg(pDocumentoID, pOrganizaionID);
        }

        public List<QueryTriples> ObtieneInformacionExtraComentariosContribuciones(Guid idrec)
        {
            return mActualizacionFacetadoAD.ObtieneInformacionExtraComentariosContribuciones(idrec);
        }

        public List<QueryTriples> ObtieneInformacionExtraCom(Guid idcom)
        {
            return mActualizacionFacetadoAD.ObtieneInformacionExtraCom(idcom);
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~ActualizacionFacetadoCN()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            //impido que se finalice dos veces este objeto
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                if (disposing)
                {
                    //Libero todos los recursos administrados que he añadido a esta clase
                    if (mActualizacionFacetadoAD != null)
                    {
                        mActualizacionFacetadoAD.Dispose();
                    }
                }

                mActualizacionFacetadoAD = null;
            }
        }

        #endregion

        #region Miembros de IDisposable

        void IDisposable.Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
