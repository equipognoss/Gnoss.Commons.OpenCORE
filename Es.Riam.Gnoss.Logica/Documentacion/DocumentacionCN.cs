using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace Es.Riam.Gnoss.Logica.Documentacion
{
    /// <summary>
    /// Lógica de negocio para la documentación de GNOSS
    /// </summary>
    public class DocumentacionCN : BaseCN, IDisposable
    {
        private LoggingService mLoggingService;

        #region Constructores

        /// <summary>
        /// Constructor para DocumentacionCN
        /// </summary>
        public DocumentacionCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            mLoggingService = loggingService;

            this.DocumentacionAD = new DocumentacionAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
            this.DesdeCL = false;
        }

        /// <summary>
        /// Constructor para DocumentacionCN
        /// </summary>
        public DocumentacionCN(bool pDesdeCL, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            mLoggingService = loggingService;

            this.DocumentacionAD = new DocumentacionAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
            this.DesdeCL = pDesdeCL;
        }

        /// <summary>
        /// Constructor para DocumentacionCN
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public DocumentacionCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            mLoggingService = loggingService;

            this.DocumentacionAD = new DocumentacionAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
            this.DesdeCL = false;
        }

        /// <summary>
        /// Constructor para DocumentacionCN
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public DocumentacionCN(string pFicheroConfiguracionBD, bool pDesdeCL, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            mLoggingService = loggingService;

            this.DocumentacionAD = new DocumentacionAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
            this.DesdeCL = pDesdeCL;
        }
        #endregion

        #region Métodos generales

        #region Públicos

        #region Metodos muy generales

        /// <summary>
        /// Obtiene los IDs de las ontologías de un proyecto a partir del enlace de las ontologías encontradas en otro proyecto
        /// </summary>
        /// <param name="pListaDocumentosID">Identificadores de las ontologías encontradas en un proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que se quieren buscar ontologías con el mismo enlace</param>
        /// <returns>Diccionario del tipo IDOntologiaOriginal -> IDOntologíaEnProyecto</returns>
        public Dictionary<Guid, Guid> ObtenerElementoVinculadoIDDeOtroProyectoConMismoEnlace(List<Guid> pListaDocumentosID, Guid pProyectoID)
        {
            return this.DocumentacionAD.ObtenerElementoVinculadoIDDeOtroProyectoConMismoEnlace(pListaDocumentosID, pProyectoID);
        }

        /// <summary>
        /// Obtiene un array cuyo primer elemento es la identidad del creador del recurso  la segunda componente es el usuario
        /// </summary>
        /// <param name="pRecursoID">Recurso que busca</param>        
        /// <returns>Array cuyo primer elemento es el perfil de la identidad del creador del recurso  la segunda componente es el usuario</returns>
        public Dictionary<Guid, Guid> ObtenerIdentidadyUsuarioIDdeRecurso(Guid pRecursoID)
        { return DocumentacionAD.ObtenerIdentidadyUsuarioIDdeRecurso(pRecursoID); }

        /// <summary>
        /// Obtiene el Identificador del elemento vinculado al documento
        /// </summary>
        /// <param name="pDocumentosID">Lista de identificadores de documentos de los que se quiere obtner su elemento vinculado</param>
        /// <returns>Diccionario con el documentoID como clave y el elementovinculadoID como valor. Si no tiene, elementovinculadoID será Guid empty</returns>
        public Dictionary<Guid, Guid> ObtenerElementoVinculadoIDPorDocumentoID(List<Guid> pDocumentosID)
        {
            return this.DocumentacionAD.ObtenerElementoVinculadoIDPorDocumentoID(pDocumentosID);
        }

        /// <summary>
        /// Obtiene si un usuario tiene acceso a un recurso concreto para leerlo o editarlo
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto en el que se está viendo el recurso</param>
        /// <param name="pDocumentoID">Identificador del recurso que se está viendo</param>
        /// <param name="pPerfilID">Identificador del perfil con el que está conectado el usuario (NULL si el usuario no está conectado)</param>
        /// <param name="pIdentidadComunidadID">Identificador de la identidad del usuario en la comunidad (NULL si el usuario no participa en la comunidad)</param>
        /// <param name="pIdentidadMyGnossID">Identificador de la identidad del usuario en MyGnoss (NULL si el usuario no está conectado)</param>
        /// <param name="pEditando">Verdad si el usuario quiere editar el recurso</param>
        /// <param name="pUsuarioPerteneceACom">Verdad si el usuario pertenece a la comunidad</param>
        /// <returns>True si el usuario tiene acceso al recurso, False en caso contrario</returns>
        public bool TieneUsuarioAccesoADocumentoEnProyecto(Guid pProyectoID, Guid pDocumentoID, Guid? pPerfilID, Guid? pIdentidadComunidadID, Guid? pIdentidadMyGnossID, bool pEditando, bool pUsuarioPerteneceACom)
        {
            List<Guid> listaAux = new List<Guid>();
            listaAux.Add(pProyectoID);
            return DocumentacionAD.TieneUsuarioAccesoADocumentoEnProyecto(listaAux, pDocumentoID, pPerfilID, pIdentidadComunidadID, pIdentidadMyGnossID, pEditando, pUsuarioPerteneceACom);
        }

        /// <summary>
        /// Obtiene si un usuario tiene acceso a un recurso concreto para leerlo o editarlo
        /// </summary>
        /// <param name="pProyectosID">Identificador de los proyecto en el que se desea comprobar el permiso</param>
        /// <param name="pDocumentoID">Identificador del recurso que se está viendo</param>
        /// <param name="pPerfilID">Identificador del perfil con el que está conectado el usuario (NULL si el usuario no está conectado)</param>
        /// <param name="pIdentidadComunidadID">Identificador de la identidad del usuario en la comunidad (NULL si el usuario no participa en la comunidad)</param>
        /// <param name="pIdentidadMyGnossID">Identificador de la identidad del usuario en MyGnoss (NULL si el usuario no está conectado)</param>
        /// <param name="pEditando">Verdad si el usuario quiere editar el recurso</param>
        /// <param name="pUsuarioPerteneceACom">Verdad si el usuario pertenece a la comunidad</param>
        /// <returns>True si el usuario tiene acceso al recurso, False en caso contrario</returns>
        public bool TieneUsuarioAccesoADocumentoEnProyecto(List<Guid> pProyectosID, Guid pDocumentoID, Guid? pPerfilID, Guid? pIdentidadComunidadID, Guid? pIdentidadMyGnossID, bool pEditando, bool pUsuarioPerteneceACom)
        {
            return DocumentacionAD.TieneUsuarioAccesoADocumentoEnProyecto(pProyectosID, pDocumentoID, pPerfilID, pIdentidadComunidadID, pIdentidadMyGnossID, pEditando, pUsuarioPerteneceACom);
        }

        /// <summary>
        /// Obtiene el valor de los votos que un usuario ha realizado a los documentos
        /// </summary>
        /// <param name="pListaDocumentosID">Lista de documentos</param>
        /// <param name="pIdentidadID">Identidad del votante</param>
        /// <returns>Diccionario con el DocumentoID y el valor del voto realizado</returns>
        public Dictionary<Guid, double> ObtenerVotoRecurso(List<Guid> pListaDocumentosID, Guid pIdentidadID)
        {
            return this.DocumentacionAD.ObtenerVotoRecurso(pListaDocumentosID, pIdentidadID);
        }

        public List<Guid> ObtenerRecursosCompartidosEnBRUsuario(List<Guid> pListaDocumentosID, Guid pUsuarioID)
        {
            return this.DocumentacionAD.ObtenerRecursosCompartidosEnBRUsuario(pListaDocumentosID, pUsuarioID);
        }

        public List<Guid> ObtenerRecursosCompartidosEnBROrganizacion(List<Guid> pListaDocumentosID, Guid pOrganizacionID)
        {
            return this.DocumentacionAD.ObtenerRecursosCompartidosEnBROrganizacion(pListaDocumentosID, pOrganizacionID);
        }

        public List<Documento> ObtenerListaRecursosUsuarioActualizarPorComunidad(Guid pPerfilID)
        {
            return this.DocumentacionAD.ObtenerListaRecursosUsuarioActualizarPorComunidad(pPerfilID);
        }

        /// <summary>
        /// Carga la tabla DocumentoWebVinBaseRecursos con la lista de los documentos modificados
        /// </summary>
        /// <param name="pDocumentosID">Lista de ids de los documentos modificados</param>
        /// <returns>Devuelve el dataset con la tabla cargada</returns>
        public List<DocumentoWebVinBaseRecursos> ObtenerWebVinBaseRecursosDocumentosModificados(List<Guid> pDocumentosID)
        {
            return this.DocumentacionAD.ObtenerWebVinBaseRecursosDocumentosModificados(pDocumentosID);
        }

        /// <summary>
        /// Obtiene una lista de identificadores de los documentos que han sido modificados, comentados o votados a partir de una fecha en una comunidad
        /// </summary>
        /// <param name="pProyectoID">Identificador de la comunidad</param>
        /// <param name="pFechaBusqueda">Fecha a partir de la cual buscar recursos con actividad</param>
        /// <returns>Lista de identificadores de documento</returns>
        public List<Guid> ObtenerDocumentosActivosEnFecha(Guid pProyectoID, DateTime pFechaBusqueda)
        {
            return this.DocumentacionAD.ObtenerDocumentosActivosEnFecha(pProyectoID, pFechaBusqueda);
        }

        /// <summary>
        /// Comprueba si el enlace del recurso ya existe y devuelve la fila del documento
        /// </summary>
        /// <param name="pNombreEnlaceTemporal">Nombre del enlace del documento temporal</param>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pObtenerSimilares">Obtner los documentos que se llaman parecidos</param>
        /// <param name="pBuscarEnTitulo">Verdad si se debe buscar también en el titulo del documento</param>
        /// <returns></returns>
        public DataWrapperDocumentacion ObtenerDocumentoDeEnlace(string pNombreEnlaceTemporal, Guid pUsuarioID, bool pObtenerSimilares, bool pBuscarEnTitulo)
        {
            return this.DocumentacionAD.ObtenerDocumentoDeEnlace(pNombreEnlaceTemporal, pUsuarioID, pObtenerSimilares, pBuscarEnTitulo);
        }

        /// <summary>
        /// Obtiene un documento por el enlace del mismo
        /// </summary>
        /// <param name="pEnlace"></param>
        /// <returns></returns>
        public Documento ObtenerDocumentoPorEnlace(string pEnlace)
        {
            return DocumentacionAD.ObtenerDocumentoPorEnlace(pEnlace);
        }

        /// <summary>
        /// Obtiene la BR de una Organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la Organización</param>
        /// <returns>BR de Organización</returns>
        public List<Documento> ObtenerDocumentacion(Guid pOrganizacionID)
        {
            return this.DocumentacionAD.ObtenerDocumentacion(pOrganizacionID);
        }


        /// <summary>
        /// Obtiene los parámetros web del ID del documento pasado como parámetro.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentacion</param>
        public void ObtenerDocumentoWebPorIDWEB(Guid pDocumentoID, DataWrapperDocumentacion pDataWrapperDocumentacion)
        {
            List<Guid> listaDocumentos = new List<Guid>();
            listaDocumentos.Add(pDocumentoID);

            DocumentacionAD.ObtenerDocumentosWebPorIDWEB(listaDocumentos, pDataWrapperDocumentacion);
        }

        /// <summary>
        /// Obtiene los documentos web según la lista de IDs pasada como parámetro.
        /// </summary>
        /// <param name="pDocumentos">Lista con los id de documentos</param>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentacion</param>
        public void ObtenerDocumentosWebPorIDWEB(List<Guid> pDocumentos, DataWrapperDocumentacion pDataWrapperDocumentacion)
        {
            DocumentacionAD.ObtenerDocumentosWebPorIDWEB(pDocumentos, pDataWrapperDocumentacion);
        }

        /// <summary>
        /// Obtiene todos los documentos Web que ha subido una persona.
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pListaIdentidadesID">Identificador de las identidades</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pInicio">Fila de inicio para la paginacion</param>
        /// <param name="pLimite">Fila limite para la paginacion</param>
        /// <param name="pOrderBy"></param>
        /// <param name="pFiltroTexto">Fitro por el que se busca</param>
        /// <param name="pDocumentosAutorPropio"></param>
        /// <param name="pSoloCompartidos"></param>
        public void ObtenerDocumentosDeIdentidadEnProyecto(DataWrapperDocumentacion pDataWrapperDocumentacion, Guid pListaIdentidadesID, Guid pProyectoID)
        {
            DocumentacionAD.ObtenerDocumentosDeIdentidadEnProyecto(pDataWrapperDocumentacion, pListaIdentidadesID, pProyectoID);
        }

        /// <summary>
        /// Obtiene todos los documentos que están vinculados a un serie de categorias (y las relaciones con otras categorías).
        /// </summary>
        /// <param name="pListaCategorias">Lista con las categorias a las que están agregados los documentos</param>
        /// <param name="pTesauroID">Identificador del tesauro al que pertenecen las categorías</param>
        /// <returns>DataSet de documentación con los documentos</returns>
        public DataWrapperDocumentacion ObtenerVinculacionDocumentosDeCategoriasTesauro(List<Guid> pListaCategorias, Guid pTesauroID)
        {
            return DocumentacionAD.ObtenerVinculacionDocumentosDeCategoriasTesauro(pListaCategorias, pTesauroID);
        }

        /// <summary>
        /// Obtiene un documento a partir de su identificador.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <returns>DataSet de documentación con el documento cargado.</returns>
        public DataWrapperDocumentacion ObtenerDocumentoPorID(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerDocumentoPorID(pDocumentoID);
        }

 

        /// <summary>
        /// Obtiene un documento a partir de su identificador.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <returns>Dataset de documentación con el documento cargado</returns>
        public DataWrapperDocumentacion ObtenerDocumentoPorIDConSubEventos(Guid pDocumentoID, Guid pProyectoID)
        {
            return DocumentacionAD.ObtenerDocumentoPorIDConSubEventos(pDocumentoID, pProyectoID);
        }

        public List<DocumentoWebVinBaseRecursos> ObtenerDocumentoWebVinBRPorDocIDYProyID(Guid pDocumentoID, Guid pProyectoID)
        {
            return DocumentacionAD.ObtenerDocumentoWebVinBRPorDocIDYProyID(pDocumentoID, pProyectoID);
        }

        /// <summary>
        /// Obtiene las filas de GrupoEditorRecurso 
        /// </summary>
        /// <param name="pGrupoID">Identificador del grupo</param>
        /// <returns>DataSet con las filas de GrupoEditorRecurso </returns>
        public List<DocumentoRolGrupoIdentidades> ObtenerFilasGrupoEditorRecurso(Guid pGrupoID)
        {
            return DocumentacionAD.ObtenerFilasGrupoEditorRecurso(pGrupoID);
        }

        /// <summary>
        /// Obtiene el Dataset del token de brightcove de un documento a partir de su identificador.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <returns>Dataset del token de brightcove del documento</returns>
        public DataWrapperDocumentacion ObtenerDocumentoTokenBrightcovePorID(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerDocumentoTokenBrightcovePorID(pDocumentoID);
        }

        /// <summary>
        /// Obtiene el Dataset de tokens de brightcove pendientes
        /// </summary>
        /// <returns>Dataset de tokens de brightcove pendientes</returns>
        public DataWrapperDocumentacion ObtenerDocumentoTokenBrightcovePendientes()
        {
            return DocumentacionAD.ObtenerDocumentoTokenBrightcovePendientes();
        }

        /// <summary>
        /// Obtiene el Dataset de tokens de brightcove pendientes
        /// </summary>
        /// <returns>Dataset de tokens de brightcove pendientes</returns>
        public DataWrapperDocumentacion ObtenerDocumentoTokenBrightcoveFinalizadas()
        {
            return DocumentacionAD.ObtenerDocumentoTokenBrightcoveFinalizadas();
        }

        /// <summary>
        /// Obtiene el Dataset del token de brightcove de un documento a partir de su token.
        /// </summary>
        /// <param name="pTokenID">Identificador del token</param>
        /// <returns>Dataset del token de brightcove del documento</returns>
        public DataWrapperDocumentacion ObtenerDocumentoTokenBrightcovePorTokenID(Guid pTokenID)
        {
            return DocumentacionAD.ObtenerDocumentoTokenBrightcovePorTokenID(pTokenID);
        }

        /// <summary>
        /// Obtiene el Dataset del token de TOP de un documento a partir de su identificador.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <returns>Dataset del token de TOP del documento</returns>
        public DataWrapperDocumentacion ObtenerDocumentoTokenTOPPorID(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerDocumentoTokenTOPPorID(pDocumentoID);
        }

        /// <summary>
        /// Obtiene el Dataset de tokens de TOP pendientes
        /// </summary>
        /// <returns>Dataset de tokens de TOP pendientes</returns>
        public DataWrapperDocumentacion ObtenerDocumentoTokenTOPPendientes()
        {
            return DocumentacionAD.ObtenerDocumentoTokenTOPPendientes();
        }

        /// <summary>
        /// Obtiene el Dataset de tokens de TOP pendientes
        /// </summary>
        /// <returns>Dataset de tokens de TOP pendientes</returns>
        public DataWrapperDocumentacion ObtenerDocumentoTokenTOPFinalizadas()
        {
            return DocumentacionAD.ObtenerDocumentoTokenTOPFinalizadas();
        }

        /// <summary>
        /// Obtiene el Dataset del token de TOP de un documento a partir de su token.
        /// </summary>
        /// <param name="pTokenID">Identificador del token</param>
        /// <returns>Dataset del token de TOP del documento</returns>
        public DataWrapperDocumentacion ObtenerDocumentoTokenTOPPorTokenID(Guid pTokenID)
        {
            return DocumentacionAD.ObtenerDocumentoTokenTOPPorTokenID(pTokenID);
        }

        /// <summary>
        /// Obtiene el Dataset de DocumentoEnvioNewsLetter pendientes de enviar.
        /// </summary>
        /// <returns>DocumentoEnvioNewsLetter</returns>
        public List<NewsletterPendientes> ObtenerDocumentoEnvioNewsletterPendienteEnvio(Guid? pDocumentoID = null)
        {
            return DocumentacionAD.ObtenerDocumentoEnvioNewsletterPendienteEnvio(pDocumentoID);
        }

        public List<NewsletterPendientes> ObtenerDocumentoEnvioNewsletterPendienteEnvioRabbit(DocumentoEnvioNewsLetter pDocumentoEnvioNewsLetter)
        {
            return DocumentacionAD.ObtenerDocumentoEnvioNewsletterPendienteEnvioRabbit(pDocumentoEnvioNewsLetter);
        }

        /// <summary>
        /// Actualiza el campo EnvioRealizado de la tabla DocumentoEnvioNewsletter
        /// </summary>
        /// <param name="pEnvioRealizado">True/False que indica si se ha realizado el envío</param>
        /// <param name="pDocumentoID">Identificador de la newsletter</param>
        /// <param name="pIdentidadID">Identidad publicadora de la newsletter</param>
        /// <param name="pFecha">Fecha de publicación de la newsletter</param>
        public void ActuarlizarEnvioRealizadoDocumentoEnvioNewsletter(bool pEnvioSolicitado, bool pEnvioRealizado, Guid pDocumentoID, Guid pIdentidadID, DateTime pFecha)
        {
            DocumentacionAD.ActuarlizarEnvioRealizadoDocumentoEnvioNewsletter(pEnvioSolicitado, pEnvioRealizado, pDocumentoID, pIdentidadID, pFecha);
        }

        /// <summary>
        /// Obtiene el Dataset de la newsletter a partir de su identificador.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <returns>Dataset</returns>
        public DataWrapperDocumentacion ObtenerDocumentoNewsletterPorDocumentoID(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerDocumentoNewsletterPorDocumentoID(pDocumentoID);
        }

        /// <summary>
        /// Obtiene la descripcion de un documento a partir de su identificador.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <returns>DataSet de documentación con el documento cargado.</returns>
        public string ObtenerDescripcionDocumentoPorID(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerDescripcionDocumentoPorID(pDocumentoID);
        }
        /// <summary>
        /// Obtiene la descripcion de una suscripcion de un documento a partir de su identificador.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <returns>DataSet de documentación con el documento cargado.</returns>
        public string ObtenerDescripcionSuscripcionDocumentoPorID(Guid pDocumentoID, Guid pSuscripcionID)
        {
            return DocumentacionAD.ObtenerDescripcionSuscripcionDocumentoPorID(pDocumentoID, pSuscripcionID);
        }

        /// <summary>
        /// Obtiene las tablas "Documento", "DocumentoWebVinBaseRecursos" a partir del identificador de un documento en un determinado proyecto.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>DocumentacionDS de documentación con el documento cargado</returns>
        public DataWrapperDocumentacion ObtenerDocumentoDocumentoWebVinBRPorID(Guid pDocumentoID, Guid pProyectoID)
        {
            return DocumentacionAD.ObtenerDocumentoDocumentoWebVinBRPorID(pDocumentoID, pProyectoID);
        }



        /// <summary>
        /// Obtiene las tablas "Documento", "DocumentoWebVinBaseRecursos" a partir del identificador de un documento en un determinado proyecto.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>DocumentacionDS de documentación con el documento cargado</returns>
        public DataWrapperDocumentacion ObtenerDocumentoDocumentoWebVinBRPorIDDeOrganizacion(Guid pDocumentoID, Guid pOrganizacionID)
        {
            return DocumentacionAD.ObtenerDocumentoDocumentoWebVinBRPorIDDeOrganizacion(pDocumentoID, pOrganizacionID);
        }

        /// <summary>
        /// Obtiene las tablas "Documento", "DocumentoWebVinBaseRecursos" a partir del identificador de un documento en un determinado proyecto.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>DocumentacionDS de documentación con el documento cargado</returns>
        public DataWrapperDocumentacion ObtenerDocumentoDocumentoWebVinBRPorIDDeUsuario(Guid pDocumentoID, Guid pUsuarioID)
        {
            return DocumentacionAD.ObtenerDocumentoDocumentoWebVinBRPorIDDeUsuario(pDocumentoID, pUsuarioID);
        }

        /// <summary>
        /// Carga toda la información de la encuesta de la home de una comunidad
        /// </summary>       
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pBaseDeRecursosID">Identificador de la base de recursos</param>
        public DataWrapperDocumentacion ObtenerEncuestaParaHome(Guid pProyectoID, Guid pBaseDeRecursosID)
        {
            return DocumentacionAD.ObtenerEncuestaParaHome(pProyectoID, pBaseDeRecursosID);
        }


        /// <summary>
        /// Carga el ID de la ultima encuesta de una comunidad
        /// </summary>       
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public Guid? ObtenerIDEncuestaParaHome(Guid pProyectoID)
        {
            return DocumentacionAD.ObtenerIDEncuestaParaHome(pProyectoID);
        }

        /// <summary>
        /// Carga toda la información de un documento menos las versiones del mismo.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pDatosCompletos">Especifica si debe cargarse todo 
        /// del documento o solo los complementos(historial, version, ect.)</param>
        /// <param name="pTraerBaseRecursos">Indica si se traen las BR o no</param>
        /// <param name="pBaseRecursosID">Identificador de la base de recursos a la que pertenece el documento o Guid.Empty si se desea obtener de la que se publico o NULL si se desea traer todas</param>
        public void ObtenerDocumentoPorIDCargarTotal(Guid pDocumentoID, DataWrapperDocumentacion pDataWrapperDocumentacion, bool pDatosCompletos, bool pTraerBaseRecursos, Guid? pBaseRecursosID)
        {
            DocumentacionAD.ObtenerDocumentoPorIDCargarTotal(pDocumentoID, pDataWrapperDocumentacion, pDatosCompletos, pTraerBaseRecursos, pBaseRecursosID);
        }

        /// <summary>
        /// Carga toda la información de un documento menos las versiones del mismo.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pDatosCompletos">Especifica si debe cargarse todo 
        /// del documento o solo los complementos(historial, version, ect.)</param>
        /// <param name="pTraerBaseRecursos">Indica si se traen las BR o no</param>
        /// <param name="pBaseRecursosID">Identificador de la base de recursos a la que pertenece el documento o Guid.Empty si se desea obtener de la que se publico o NULL si se desea traer todas</param>
        public void ObtenerDocumentosPorIDCargarTotal(List<Guid> pDocumentosID, DataWrapperDocumentacion pDataWrapperDocumentacion, bool pDatosCompletos, bool pTraerBaseRecursos, Guid? pBaseRecursosID)
        {
            DocumentacionAD.ObtenerDocumentosPorIDCargarTotal(pDocumentosID, pDataWrapperDocumentacion, pDatosCompletos, pTraerBaseRecursos, pBaseRecursosID);
        }

        public DataWrapperDocumentacion ObtenerOpcionesEncuesta(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerOpcionesEncuesta(pDocumentoID);
        }

        /// <summary>
        /// Obtiene las vinculaciones de un recurso (DocumentoVin..).
        /// </summary>
        /// <param name="pDocumentoID">Lista de identificadores de documento</param>
        public DataWrapperDocumentacion ObtenerVinculacionesRecursos(List<Guid> pDocumentosID)
        {
            return DocumentacionAD.ObtenerVinculacionesRecursos(pDocumentosID);
        }

        /// <summary>
        /// Obtiene las vinculaciones de un recurso (DocumentoVin..).
        /// </summary>
        /// <param name="pDocumentoID">ID del documento</param>
        public DataWrapperDocumentacion ObtenerVinculacionesRecurso(Guid pDocumentoID, bool pTraerRelacionesInversas = false)
        {
            return DocumentacionAD.ObtenerVinculacionesRecursos(new List<Guid>() { pDocumentoID }, pTraerRelacionesInversas);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pDataWrapperDocumentacion"></param>
        /// <param name="pDocumentoID"></param>
        /// <param name="pInicio"></param>
        /// <param name="pFinal"></param>
        /// <param name="pNombreTablaCOMUNIDADES"></param>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public void ObtenerListaRecursosRelacionados(DataWrapperDocumentacion pDataWrapperDocumentacion, FacetadoDS pFacetadoDS, Guid pDocumentoID, int pInicio, int pFinal, string pNombreTablaCOMUNIDADES, Guid pProyectoID)
        {
            DocumentacionAD.ObtenerListaRecursosRelacionados(pDataWrapperDocumentacion, pFacetadoDS, pDocumentoID, pInicio, pFinal, pNombreTablaCOMUNIDADES, pProyectoID);
        }

        ///// <summary>
        ///// Obtiene una lista de recursos para la vista prensa.
        ///// </summary>
        ///// <param name="pDocumentos">IDs de documento</param>
        ///// <param name="pProyectoID">ProyectoID</param>
        ///// <returns>DataSet de documento con una lista de recursos para la vista prensa</returns>
        //public List<DocumentoTieneImagenConNombresCortoProy> ObtenerListaRecursosPrensa(List<Guid> pDocumentos, Guid pProyectoID)
        //{
        //    return DocumentacionAD.ObtenerListaRecursosPrensa(pDocumentos, pProyectoID);
        //}

        #region Recursos Vinculados

        /// <summary>
        /// Comprueba se un recurso tiene algún recurso vinculado
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento a comprobar</param>
        /// <returns>True si el documento tiene algún recurso vinculado, false en caso contrario</returns>
        public bool TieneDocumentoDocumentosVinculados(Guid pDocumentoID)
        {
            return DocumentacionAD.TieneDocumentoDocumentosVinculados(pDocumentoID);
        }

        /// <summary>
        /// Obtiene los documento vinculados que puede ver el perfil actual.
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pProyectoID">proyecto id del documento</param>
        /// <param name="pDocumentoID">DocumentoID del que se traerán los vinculos</param>
        /// <param name="pPerfilActualID">PerfilID actual</param>
        /// <param name="pInicio">Inicio paginación</param>
        /// <param name="pLimite">Fin paginación</param>
        /// <returns>Documento vinculados que puede ver el perfil actual</returns>
        public int ObtenerDocumentosVinculadosDocumento(DataWrapperDocumentacion pDataWrapperDocumentacion, Guid pProyectoID, Guid pDocumentoID, Guid pPerfilActualID, int pInicio, int pLimite)
        {
            return DocumentacionAD.ObtenerDocumentosVinculadosDocumento(pDataWrapperDocumentacion, pProyectoID, pDocumentoID, pPerfilActualID, pInicio, pLimite);
        }

        /// <summary>
        /// Obtiene los documento vinculados que puede ver el perfil actual.
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pProyectoID">proyecto id del documento</param>
        /// <param name="pDocumentoID">DocumentoID del que se traerán los vinculos</param>
        /// <param name="pPerfilActualID">PerfilID actual</param>
        /// <param name="pInicio">Inicio paginación</param>
        /// <param name="pLimite">Fin paginación</param>
        /// <returns>Documento vinculados que puede ver el perfil actual</returns>
        public Dictionary<Guid, List<Guid>> ObtenerListaDocumentosVinculadosDocumento(DataWrapperDocumentacion pDataWrapperDocumentacion, Guid pProyectoID, Guid pDocumentoID, Guid pPerfilActualID, int pInicio, int pLimite, out int pNumVinculados)
        {
            return DocumentacionAD.ObtenerListaDocumentosVinculadosDocumento(pDataWrapperDocumentacion, pProyectoID, pDocumentoID, pPerfilActualID, pInicio, pLimite, out pNumVinculados);
        }


        /// <summary>
        /// Obtiene el numero de documentos vinculados a un documento por ID.
        /// </summary>

        /// <param name="pDocumentoID">DocumentoID del que se traerán los vinculos</param>
        /// <param name="pPerfilActualID">PerfilID actual</param>
        /// <param name="pInicio">Inicio paginación</param>
        /// <param name="pLimite">Fin paginación</param>
        /// <returns>Documento vinculados que puede ver el perfil actual</returns>
        public int ObtenerNumeroDocumentosVinculadosDocuemntoPorID(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerNumeroDocumentosVinculadosDocuemntoPorID(pDocumentoID);
        }

        /// <param name="pDocumentoID">DocumentoID del que se traerán los vinculos</param>
        /// <param name="pPerfilActualID">PerfilID actual</param>
        /// <param name="pInicio">Inicio paginación</param>
        /// <param name="pLimite">Fin paginación</param>
        /// <returns>Documento vinculados que puede ver el perfil actual</returns>
        public int ObtenerNumeroRecursosPublicados(Guid pIdentidadID)
        {
            return DocumentacionAD.ObtenerNumeroRecursosPublicados(pIdentidadID);
        }

        /// <summary>
        /// Desvincula directamente 2 recursos de la BD.
        /// </summary>
        /// <param name="pDocumento1ID">ID del documento 1</param>
        /// <param name="pDocumento2ID">ID del documento 2</param>
        /// <returns>TRUE si se han desvinculado correctamente, FALSE en caso contrario</returns>
        public bool DesvincularRecursos(Guid pDocumento1ID, Guid pDocumento2ID)
        {
            return DocumentacionAD.DesvincularRecursos(pDocumento1ID, pDocumento2ID);
        }

        #endregion

        /// <summary>
        /// Obtiene unos documentos a partir de sus identificadores.
        /// </summary>
        /// <param name="pListaDocumentoID">Identificadores de documento</param>
        /// <param name="pTraerBasesRecurso">Indica si se debe traer las tablas baseRecursos o no</param>
        /// <returns>DataSet de documentación con los documentos cargados</returns>
        public DataWrapperDocumentacion ObtenerDocumentosPorID(List<Guid> pListaDocumentoID, bool pTraerBasesRecurso)
        {
            return DocumentacionAD.ObtenerDocumentosPorID(pListaDocumentoID, pTraerBasesRecurso);
        }

        /// <summary>
        /// Carga las tablas: Documento, DocumentoWebVinBaseRecursos, VotoDocumento, BaseRecursos, BaseRecursosProyecto y BaseRecursosUsuario necesarias para pintar el listado de acciones
        /// </summary>
        /// <param name="pListaDocumentos">Lista de documentos para traer</param>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pUsuarioID">ID del usuario</param>
        public DataWrapperDocumentacion ObtenerDocumentosPorIDParaListadoDeAcciones(List<Guid> pListaDocumentos, Guid pProyectoID, Guid pUsuarioID)
        {
            return DocumentacionAD.ObtenerDocumentosPorIDParaListadoDeAcciones(pListaDocumentos, pProyectoID, pUsuarioID);
        }

        /// <summary>
        /// Carga toda la información de un documento menos las versiones del mismo.
        /// </summary>
        /// <param name="pListaDocumentos">Lista de documentos para traer</param>
        public DataWrapperDocumentacion ObtenerDocumentosPorID(List<Guid> pListaDocumentos)
        {
            return DocumentacionAD.ObtenerDocumentosPorID(pListaDocumentos, false);
        }

        /// <summary>
        /// Obtiene unos documentos a partir de sus identificadores, solo tabla Documento.
        /// </summary>
        /// <param name="pListaDocumentoID">Identificadores de documento</param>
        /// <returns>Dataset de documentación con los documentos cargados, solo tabla Documento</returns>
        public List<Documento> ObtenerDocumentosPorIDSoloDocumento(List<Guid> pListaDocumentoID)
        {
            return DocumentacionAD.ObtenerDocumentosPorIDSoloDocumento(pListaDocumentoID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pListaDefinitiva"></param>
        /// <returns></returns>
        public DataWrapperDocumentacion ObtenerDocumentosPorIDYComunidadesComparticion(List<Guid> pListaDefinitiva)
        {
            return DocumentacionAD.ObtenerDocumentosPorIDYComunidadesComparticion(pListaDefinitiva);
        }

        ///// <summary>
        ///// Actualiza la puntuaciónj de los recursos más populares en el último mes
        ///// </summary>
        //public List<string> ActualizarRankingRecursos()
        //{
        //    return DocumentacionAD.ActualizarRankingRecursos();
        //}

        /// <summary>
        /// Actualiza el ranking del documento web vin base recursos pasado como parámetro
        /// </summary>
        /// <param name="pDocID">Documento que se va a actualizar</param>
        /// <param name="pBaseRecursos">Base de recursos a la que pertenece el recurso</param>
        /// <param name="pRank">Ranking a actualizar</param>
        public void ActualizarRankingDocumentoWebVinBaseRecurso(Guid pDocID, Guid pBaseRecursos, int pRank)
        {
            DocumentacionAD.ActualizarRankingDocumentoWebVinBaseRecurso(pDocID, pBaseRecursos, pRank);
        }


        /// <summary>
        /// Actualiza el ranking del Documento.
        /// </summary>
        /// <param name="pDocID">Documento que se va a actualizar</param>
        /// <param name="pRank">Ranking del documento que se va a actualizar.</param>
        public void ActualizarRankingDocumento(Guid pDocID, int pRank)
        {
            DocumentacionAD.ActualizarRankingDocumento(pDocID, pRank);
        }

        /// <summary>
        /// Obtiene los documentos más populares de las comunidades GNOSS sin tener en cuenta ayuda y FAQS y Noticias
        /// </summary>
        /// <param name="numDocumentos">Numero de documentos para traer</param>
        public List<Documento> ObtenerDocumentosMasVistos(int numDocumentos)
        {
            return DocumentacionAD.ObtenerDocumentosMasVistos(numDocumentos);
        }

        public List<Guid> ObtenerDocumentosMasVistosProyecto(Guid pProyectoID, int numDocumentos)
        {
            //DataSet dataSet = DocumentacionAD.ObtenerDocumentosMasVistosProyecto(pProyectoID, numDocumentos);
            return DocumentacionAD.ObtenerDocumentosMasVistosProyecto(pProyectoID, numDocumentos);
            //List<Guid> listaDocumentos = new List<Guid>();

            //foreach (DataRow fila in dataSet.Tables["MasVisitados"].Select("", "NumeroConsultas desc"))
            //{
            //    listaDocumentos.Add((Guid)fila["DocumentoID"]);
            //}
            //return listaDocumentos;
        }

        /// <summary>
        /// Obtiene los documentos publicos más populares de la comunidad indicada
        /// </summary>
        /// <param name="pProyecto">ID del proyecto</param>
        /// <param name="pNumDocumentos">Numero de documentos que queremos traer</param>
        /// <returns></returns>
        public DataWrapperDocumentacion ObtenerRecursosPopularesProyecto(Guid pProyectoID, int pNumDocumentos)
        {
            return DocumentacionAD.ObtenerRecursosPopularesProyecto(pProyectoID, pNumDocumentos);
        }

        /// <summary>
        /// Obtiene los IDs de los documentos publicos más populares de la comunidad indicada
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pNumDocumentos">Numero de documentos que queremos traer</param>
        /// <returns>Lista de DocumentoID</returns>
        public List<Guid> ObtenerListaRecursosPopularesProyecto(Guid pProyectoID, int pNumDocumentos)
        {
            return DocumentacionAD.ObtenerListaRecursosPopularesProyecto(pProyectoID, pNumDocumentos);
        }

        /// <summary>
        /// Actualiza la documentación
        /// </summary>
        /// <param name="pDocumentacionDS">Documentos a actualizar</param>
        public void ActualizarDocumentacion()
        {
            base.Actualizar();
        }


        /// <summary>
        /// Borra del ácido los documentos
        /// </summary>
        /// <param name="pDocumentosID">Lista de identificadores de los documentos a borrar</param>
        public void EliminarDocumentos(List<Guid> pDocumentosID)
        {
            DocumentacionAD.EliminarDocumentos(pDocumentosID);
        }

        /// <summary>
        /// Devuelve los documentos de una entidad
        /// </summary>
        /// <param name="pEntidadID">ID de entidad</param>
        /// <returns></returns> 
        public DataWrapperDocumentacion ObtenerDocumentosDeEntidad(Guid pEntidadID)
        {
            return ObtenerDocumentosDeEntidad(pEntidadID, false);
        }

        /// <summary>
        /// Devuelve los documentos de una entidad
        /// </summary>
        /// <param name="pEntidadID">ID de entidad</param>
        /// <param name="pSoloUltimaVersionNoEliminados">TRUE si solo se deben traer los que sean última version y NO eliminados, FALSE para traer todos</param>
        /// <returns></returns> 
        public DataWrapperDocumentacion ObtenerDocumentosDeEntidad(Guid pEntidadID, bool pSoloUltimaVersionNoEliminados)
        {
            List<Guid> lista = new List<Guid>();
            lista.Add(pEntidadID);

            return ObtenerDocumentosDeEntidades(lista, pSoloUltimaVersionNoEliminados);
        }

        /// <summary>
        /// Obtiene la documentación de una lista de entidades
        /// </summary>
        /// <param name="pListaEntidades">Lista de claves</param>
        /// <returns>DocumentacionDS</returns>
        public DataWrapperDocumentacion ObtenerDocumentosDeEntidades(List<Guid> pListaEntidades)
        {
            return ObtenerDocumentosDeEntidades(pListaEntidades, false);
        }

        /// <summary>
        /// Obtiene la documentación de una lista de entidades
        /// </summary>
        /// <param name="pListaEntidades">Lista de claves</param>
        /// <param name="pSoloUltimaVersionNoEliminados">TRUE si solo se deben traer los que sean última version y NO eliminados, FALSE para traer todos</param>
        /// <returns>DocumentacionDS</returns>
        public DataWrapperDocumentacion ObtenerDocumentosDeEntidades(List<Guid> pListaEntidades, bool pSoloUltimaVersionNoEliminados)
        {
            return DocumentacionAD.ObtenerDocumentosDeEntidades(pListaEntidades, pSoloUltimaVersionNoEliminados);
        }




        /// <summary>
        /// Devuelve una lista con los perfiles que tienen acceso a algun recurso privado
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Lista con los perfiles</returns>
        public List<Guid> ObtenerPerfilesConRecursosPrivados(Guid pProyectoID)
        {
            return DocumentacionAD.ObtenerPerfilesConRecursosPrivados(pProyectoID);
        }

        /// <summary>
        /// Devuelve una lista con los perfiles que tienen acceso a algun recurso privado
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Lista con los perfiles</returns>
        public List<Guid> ObtenerPerfilesConDebatesPrivados(Guid pProyectoID)
        {
            return DocumentacionAD.ObtenerPerfilesConDebatesPrivados(pProyectoID);
        }

        public bool TienePerfilRecursosPrivadosEnComunConElPerfilPagina(Guid? pProyectoID, Guid pPerfilActualID, Guid pPerfilPaginaID)
        {
            return DocumentacionAD.TienePerfilRecursosPrivadosEnComunConElPerfilPagina(pProyectoID, pPerfilActualID, pPerfilPaginaID);
        }

        /// <summary>
        /// Obtiene el tipo de un documento
        /// </summary>
        /// <param name="pDocumentoID">DocumentoID</param>
        public TiposDocumentacion ObtenerTipoDocumentoPorDocumentoID(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerTipoDocumentoPorDocumentoID(pDocumentoID);
        }

        /// <summary>
        /// Obtiene los tuipos de varios documentos
        /// </summary>
        /// <param name="pListaDocumentoID">Lista de IDs de documentos</param>
        public Dictionary<Guid, TiposDocumentacion> ObtenerTiposDocumentosPorDocumentosID(List<Guid> pListaDocumentoID)
        {
            return DocumentacionAD.ObtenerTiposDocumentosPorDocumentosID(pListaDocumentoID);
        }

        /// <summary>
        /// Carga los tipos de fichas bibliograficas.
        /// </summary>
        /// <param name="pDocumentacionDW">DataSet de documentacion</param>
        public void ObtenerTiposFichaBibliografica(DataWrapperDocumentacion pDocumentacionDW)
        {
            DocumentacionAD.ObtenerTiposFichaBibliografica(pDocumentacionDW);
        }

        /// <summary>
        /// Obtiene los atributos de las ficha bibliograficas de una lista de documentos.
        /// </summary>
        /// <param name="pListaDocumentosIDs">Lista de documentos</param>
        /// <param name="pDocumentacionDW">DataSet de documentación</param>
        public void ObtenerFichaBibliograficaDocumentos(List<Guid> pListaDocumentosIDs, DataWrapperDocumentacion pDocumentacionDW)
        {
            DocumentacionAD.ObtenerFichaBibliograficaDocumentos(pListaDocumentosIDs, pDocumentacionDW);
        }

        /// <summary>
        /// Devuelve un dictionary con los enlaces y un booleano que determina si existe o no
        /// </summary>
        /// <param name="pListaEnlaces">Lista de enlaces para comprobar</param>
        /// <param name="pBaseRecursosID">Base de recursos en la que buscar</param>
        /// <returns>dictionary con los enlaces y un booleano que determina si existe o no</returns>
        public Dictionary<string, bool> DocumentosRepetidosEnlaces(List<string> pListaEnlaces, Guid pBaseRecursosID)
        {
            return DocumentacionAD.DocumentosRepetidosEnlaces(pListaEnlaces, pBaseRecursosID);
        }

        /// <summary>
        /// Comprueba si el título o el enlace de un doc están repetidos.
        /// </summary>
        /// <param name="pEnlace">Enlace del doc</param>
        /// <param name="pTitulo">Titulo del doc</param>
        /// <param name="pDocumentoID">Identificador del documento que se está revisando</param>
        /// <returns>0 si no hay repetidos, 1 si se repite el título, 2 si se repite el enlace, 3 si se repiten ambos</returns>
        public int DocumentoRepetidoTituloEnlace(string pTitulo, string pEnlace, Guid pDocumentoID)
        {
            Guid documentoRepID;
            return DocumentacionAD.DocumentoRepetidoTituloEnlace(pTitulo, pEnlace, pDocumentoID, Guid.Empty, out documentoRepID);
        }

        /// <summary>
        /// Comprueba si el título o el enlace de un doc están repetidos.
        /// </summary>
        /// <param name="pEnlace">Enlace del doc</param>
        /// <param name="pTitulo">Titulo del doc</param>
        /// <param name="pDocumentoID">Identificador del documento que se está revisando</param>
        /// <param name="pBaseRecursosID">Identificador de base de recursos</param>
        /// <param name="pDocumentoRepetidoID">Identificador del documento repetido</param>
        /// <returns>0 si no hay repetidos, 1 si se repite el título, 2 si se repite el enlace, 3 si se repiten ambos</returns>
        public int DocumentoRepetidoTituloEnlace(string pTitulo, string pEnlace, Guid pDocumentoID, Guid pBaseRecursosID, out Guid pDocumentoRepetidoID)
        {
            return DocumentacionAD.DocumentoRepetidoTituloEnlace(pTitulo, pEnlace, pDocumentoID, pBaseRecursosID, out pDocumentoRepetidoID);
        }

        /// <summary>
        /// Comprueba si el título o el enlace de un doc están repetidos en las BRs especificadas.
        /// </summary>
        /// <param name="pEnlace">Enlace del doc</param>
        /// <param name="pTitulo">Titulo del doc</param>
        /// <param name="pDocumentoID">Identificador del documento que se está revisando o Guid.Empty si no se desea omitir ninguno</param>
        /// <param name="pBasesRecursosID">Lista con los identificadores de las bases de recursos</param>
        /// <returns>0 si no hay repetidos, 1 si se repite el título, 2 si se repite el enlace, 3 si se repiten ambos</returns>
        public int DocumentoRepetidoTituloEnlaceEnVariasBRs(string pTitulo, string pEnlace, Guid pDocumentoID, List<Guid> pBasesRecursosID)
        {
            Guid docID;
            return DocumentoRepetidoTituloEnlaceEnVariasBRs(pTitulo, pEnlace, pDocumentoID, pBasesRecursosID, out docID);
        }

        /// <summary>
        /// Comprueba si el título o el enlace de un doc están repetidos en las BRs especificadas.
        /// </summary>
        /// <param name="pEnlace">Enlace del doc</param>
        /// <param name="pTitulo">Titulo del doc</param>
        /// <param name="pDocumentoID">Identificador del documento que se está revisando o Guid.Empty si no se desea omitir ninguno</param>
        /// <param name="pBasesRecursosID">Lista con los identificadores de las bases de recursos</param>
        /// <param name="pDocumentoRepetidoID">Identificador del documento repetido</param>
        /// <returns>0 si no hay repetidos, 1 si se repite el título, 2 si se repite el enlace, 3 si se repiten ambos</returns>
        public int DocumentoRepetidoTituloEnlaceEnVariasBRs(string pTitulo, string pEnlace, Guid pDocumentoID, List<Guid> pBasesRecursosID, out Guid pDocumentoRepetidoID)
        {
            return DocumentacionAD.DocumentoRepetidoTituloEnlaceEnVariasBRs(pTitulo, pEnlace, pDocumentoID, pBasesRecursosID, out pDocumentoRepetidoID);
        }

        /// <summary>
        /// Obtiene el historial de un documento.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        public void ObtenerHistorialDocumentoPorID(Guid pDocumentoID, DataWrapperDocumentacion pDataWrapperDocumentacion)
        {
            DocumentacionAD.ObtenerHistorialDocumentoPorID(pDocumentoID, pDataWrapperDocumentacion);
        }

        /// <summary>
        /// Obtiene id del publicador a partir del id del comentario 
        /// </summary>
        /// <param name="pComentarioID">ID del comentario</param>
        /// <returns>ID de la identidad del publicador</returns>
        public Guid ObtenerPublicadorAPartirIDsComentario(Guid pComentarioID)
        {
            return DocumentacionAD.ObtenerPublicadorAPartirIDsComentario(pComentarioID);
        }

        /// <summary>
        /// Obtiene el documento que se encuentra vinculado a un elemento y no este marcado como eliminado("pClaveElementoVinculadoID")
        /// </summary>
        /// <param name="pClaveElementoVinculadoID"></param>
        /// <returns>DocumentacionDS</returns>
        public DataWrapperDocumentacion ObtenerDocumentoDeElementoVinculado(Guid pClaveElementoVinculadoID)
        {
            return DocumentacionAD.ObtenerDocumentoDeElementoVinculado(pClaveElementoVinculadoID, true);
        }


        /// <summary>
        /// Obtiene id del publicador a partir del id del documento y del proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pDocumentoID">ID del documento</param>
        /// <returns>ID de la identidad del publicador</returns>
        public Guid ObtenerPublicadorAPartirIDsRecursoYProyecto(Guid pProyectoID, Guid pDocumentoID)
        { return DocumentacionAD.ObtenerPublicadorAPartirIDsRecursoYProyecto(pProyectoID, pDocumentoID); }
        /// <summary>
        /// Obtienen si un proyecto tiene o no articulos wiki (no eliminados)
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>True si tiene</returns>
        public bool TieneArticulosWikiProyecto(Guid pProyectoID)
        {
            return DocumentacionAD.TieneArticulosWikiProyecto(pProyectoID);
        }

        /// <summary>
        /// Comprueba si un documento tiene algún comentario
        /// </summary>
        /// <param name="pDocumentoID">Clave del documento</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que se buscan comentarios</param>
        /// <returns>True si tiene</returns>
        public bool TieneComentariosDocumento(Guid pDocumentoID, Guid pProyectoID)
        {
            return DocumentacionAD.TieneComentariosDocumento(pDocumentoID, pProyectoID);
        }

        /// <summary>
        /// Comprueba si una lista de documentos son borrador
        /// </summary>
        /// <param name="pListaDocumentoID">Claves de documentos</param>
        /// <returns>True si el documento es borrador</returns>
        public Dictionary<Guid, bool> EsDocumentoBorradorLista(List<Guid> pListaDocumentoID)
        {
            return DocumentacionAD.EsDocumentoBorradorLista(pListaDocumentoID);
        }

        /// <summary>
        /// Comprueba si un documento es borrador
        /// </summary>
        /// <param name="pDocumentoID">Clave del documento</param>
        /// <returns>True si el documento es borrador</returns>
        public bool EsDocumentoBorrador(Guid pDocumentoID)
        {
            List<Guid> lista = new List<Guid>();
            lista.Add(pDocumentoID);
            return EsDocumentoBorradorLista(lista)[pDocumentoID];
        }

        /// <summary>
        /// Obtiene el estado de una pregunta en un proyecto concreto
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documeto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public EstadoPregunta ComprobarEstadoPregunta(Guid pDocumentoID, Guid pProyectoID)
        {
            return DocumentacionAD.ComprobarEstadoPregunta(pDocumentoID, pProyectoID);
        }

        /// <summary>
        /// Comprueba si un documento es pregunta o debate
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns></returns>
        public bool EsRecursoPreguntaODebate(Guid pDocumentoID)
        {
            return DocumentacionAD.EsRecursoPreguntaODebate(pDocumentoID);
        }

        /// <summary>
        /// Obtiene el enlace del documento vinculado al documento dado
        /// </summary>
        /// <param name="pDocumentoID"></param>
        /// <returns></returns>
        public string ObtenerEnlaceDocumentoVinculadoADocumento(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerEnlaceDocumentoVinculadoADocumento(pDocumentoID);
        }

        /// <summary>
        /// Obtiene el documento que se encuentra vinculado a un elemento y no este marcado como eliminado("pClaveElementoVinculadoID")
        /// </summary>
        /// <param name="pClaveElementoVinculadoID">Identificador del elemento al que están vinculados los documentos</param>
        /// <param name="pHacerCargaTotalDocumentos">Indica si debe hacerse una carga completa de los documentos resultado, o solo debe traerse sus datos de la tabla documento</param>
        /// <returns>DocumentacionDS</returns>
        public DataWrapperDocumentacion ObtenerDocumentoDeElementoVinculado(Guid pClaveElementoVinculadoID, bool pHacerCargaTotalDocumentos)
        {
            return DocumentacionAD.ObtenerDocumentoDeElementoVinculado(pClaveElementoVinculadoID, pHacerCargaTotalDocumentos);
        }

        /// <summary>
        /// Obtiene un dataset con una 3 tablas, una 'Tags' con los campos "Nombre" , "DocumentoID" con todos los documentos y sus tags de la comunidad pasada por parametro, teniendo en cuenta  "Documento.Eliminado=0 "+"Documento.UltimaVersion = 1"+"Documento.Borrador=0 "+ "DocumentoWebVinBaseRecursos.Eliminado=0 " +"AND TagDocumento.Tipo = 0 ", otra tabla 'Documentos' con solo las claves de los documentos del proyecto,  otra tabla 'TagsAutomaticos' con los campos "DocumentoID", "Nombre" que contienen  los tags automáticos (nombre de las categorías + nombre del autor del documento + fechaDePublicacion yyyymmdd + NivelCertificacion )
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto a buscar los tags de sus documentos</param>
        /// <param name="pFecha">Fecha inicial para la búsqueda</param>
        /// <returns>DataSet no tipado</returns>

        /// <summary>
        /// Obtiene las versiones de las fotos de una serie de documentos
        /// </summary>
        /// <param name="pListaDocumentosID">Listado de ID de los documentos</param>
        public Dictionary<Guid, int?> ObtenerVersionesDeFotosDeDocumentosPorIDs(List<Guid> pListaDocumentosID)
        {
            Dictionary<Guid, int?> listaVersionesFotos = new Dictionary<Guid, int?>();
            DataWrapperDocumentacion docDW = ObtenerDocumentosPorID(pListaDocumentosID, false);
            foreach (Documento filaDoc in docDW.ListaDocumento)
            {
                if (filaDoc.VersionFotoDocumento.HasValue)
                {
                    listaVersionesFotos.Add(filaDoc.DocumentoID, filaDoc.VersionFotoDocumento.Value);
                }
                else
                {
                    listaVersionesFotos.Add(filaDoc.DocumentoID, null);
                }
            }
            return listaVersionesFotos;
        }

        /// <summary>
        /// Obtiene los tags de un documento
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns></returns>
        public string ObtenerTagsDeDocumento(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerTagsDeDocumento(pDocumentoID);
        }

        /// <summary>
        /// Obtiene el título, la descripción y los tags de un documento
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns>Fila del documento con el título la descripción y los tags cargados</returns>
        public Documento ObtenerTituloDescripcionTagsDeDocumento(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerTituloDescripcionTagsDeDocumento(pDocumentoID);
        }

        /// <summary>
        /// Obtiene la lista de categorías de tesauro vinculadas con un documento
        /// </summary>
        /// <param name="pDocumentoID">Identificador de un documento</param>
        /// <returns></returns>
        public List<string> ObtenerNombresCategoriasVinculadoDocumento(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerNombresCategoriasVinculadoDocumento(pDocumentoID);
        }

        /// <summary>
        /// Obtiene el nombre de la entidad vinculada de un documento.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de un documento</param>
        /// <returns>Nombre de la entidad vinculada de un documento</returns>
        public string ObtenerNombreElementoVinculadoDocumento(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerNombreElementoVinculadoDocumento(pDocumentoID);
        }

        /// <summary>
        /// Obtiene el campo 'NombreCategoriaDoc' de un documento.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de un documento</param>
        /// <returns>Campo 'NombreCategoriaDoc' de un documento</returns>
        public string ObtenerNombreCategoriaDocDocumento(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerNombreCategoriaDocDocumento(pDocumentoID);
        }

        /// <summary>
        /// Obtiene el creador del documentoID pasado como parámetro.
        /// </summary>
        /// <param name="pDocID">DocumentoID del que queremos obtener el creador.</param>
        /// <returns>CreadorID del documento.</returns>
        public Guid ObtenerCreadorDocumentoID(Guid pDocID)
        {
            return DocumentacionAD.ObtenerCreadorDocumentoID(pDocID);
        }

        /// <summary>
        /// Obtiene el creador del documentoID pasado como parámetro.
        /// </summary>
        /// <param name="pDocID">DocumentoID del que queremos obtener el creador.</param>
        /// <returns>CreadorID del documento.</returns>
        public Dictionary<Guid, string> ObtenerEmailCreadoresDocumentosID(List<Guid> pDocsID)
        {
            return DocumentacionAD.ObtenerEmailCreadoresDocumentosID(pDocsID);
        }

        /// <summary>
        /// Obtiene la imagen principal de los documentos. Lo devuelve en formato "DocumentoID|tamaño,rutaImagenPrincipal"
        /// </summary>
        /// <param name="pListaIDsDocumentos">Lista de identificadores de los documentos</param>
        /// <returns>Array de cadenas con el documentoID y la ruta de la imagen principal</returns>
        public string[] ObtenerImagenesPrincipalesDocumentos(List<Guid> pListaIDsDocumentos)
        {
            return DocumentacionAD.ObtenerImagenesPrincipalesDocumentos(pListaIDsDocumentos);
        }

        public List<Documento> ObtenerRecursosIdentidadProyectoEditor(Guid pProyID, Guid pIdentidadID, bool pUnicoEditor)
        {
            return DocumentacionAD.ObtenerRecursosIdentidadProyectoEditor(pProyID, pIdentidadID, pUnicoEditor);
        }

        /// <summary>
        /// Obtiene un diccionario de ciertos documentos los nombres cortos de los proyectos donde están subidos y compartidos.
        /// </summary>
        /// <param name="pDocumentoIDs">IDs de documentos</param>
        /// <returns>Diccionario de ciertos documentos con los nombres cortos de los proyectos donde están subidos y compartidos</returns>
        public Dictionary<Guid, List<string>> ObtenerProyectosDocumentos(List<Guid> pDocumentoIDs)
        {
            return DocumentacionAD.ObtenerProyectosDocumentos(pDocumentoIDs);
        }

        /// <summary>
        /// Obtiene un diccionario de ciertos documentos con sus tipos y los IDs de los proyectos donde están subidos y compartidos.
        /// </summary>
        /// <param name="pDocumentoIDs">IDs de documentos</param>
        /// <returns>Diccionario de ciertos documentos con sus tipos y los IDs de los proyectos donde están subidos y compartidos</returns>
        public Dictionary<Guid, KeyValuePair<short, List<Guid>>> ObtenerTipoYProyectosDocumentos(List<Guid> pDocumentoIDs)
        {
            return DocumentacionAD.ObtenerTipoYProyectosDocumentos(pDocumentoIDs);
        }

        #endregion

        #region Ontologías

        /// <summary>
        /// Carga el dataSet con las ontologías del proyecto.
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pTraerProtegidos">Indica si se deben obtener las ontologías protegidas o no</param>
        /// <param name="pTraerSecundarias">Indica si hay que cargar ontologías secundarias</param>
        /// <param name="pTraerOntosEntorno">Indica si deben traerse las ontologías del entorno</param>
        public void ObtenerOntologiasProyecto(Guid pProyectoID, DataWrapperDocumentacion pDataWrapperDocumentacion, bool pTraerProtegidos, bool pTraerSecundarias, bool pTraerOntosEntorno, bool pTraerDocWebVinBaseRecursos = false)
        {
            DocumentacionAD.ObtenerOntologiasProyecto(pProyectoID, pDataWrapperDocumentacion, pTraerProtegidos, pTraerSecundarias, pTraerOntosEntorno, pTraerDocWebVinBaseRecursos);
        }

        public List<Documento> ObtenerOntologiasSecundarias(Guid pProyectoID)
        {
            return DocumentacionAD.ObtenerOntologiasSecundarias(pProyectoID);
        }
        /// <summary>
        /// Obtiene los documentos que son Ontologias para el borrado masivo.
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        public Dictionary<Guid, string> ObtenerOntologiasParaBorrado(Guid pProyectoID)
        {
            return DocumentacionAD.ObtenerOntologiasParaBorrado(pProyectoID);
        }

        /// <summary>
        /// Devuleve la ontologia seleccionada.
        /// </summary>
        /// <param name="pOntologiaID">Identificador del documento ontologia</param>
        public Documento OntologiaSeleccionada(Guid pOntologiaID)
        {
            return DocumentacionAD.OntologiaSeleccionada(pOntologiaID);
        }
        /// <summary>
        /// Devuleve la ontologia seleccionada.
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        public Documento HayOntologiaSeleccionada(Guid pProyectoID)
        {
            return DocumentacionAD.HayOntologiaSeleccionada(pProyectoID);
        }
        /// <summary>
        /// Borra todos los documentos vinculados a la ontologia y al proyecto
        /// </summary>
        /// <param name="pOntologiaID">Guid del DocumentoID que pertenece a la ontologia</param>
        /// <param name="pProyectoID">Proyecto desde el que se trabaja</param>
        /// <returns>Devuelve cierto si todo va bien, devuelve cierto y falso en caso contrario</returns>
        public void BorradoMasivoOntologias(List<Guid> pOntologiaID, Guid pProyectoID)
        {
            List<Guid> documentos = new List<Guid>();
            DocumentacionAD.BorrarDocumentosScript(pOntologiaID, pProyectoID);
        }
        /// <summary>
        /// Obtiene las ontologías de un entorno
        /// </summary>
        /// <param name="pDocumentacionDS">Dataset de documentación ya inicializado</param>
        public DataWrapperDocumentacion ObtenerOntologiasEntorno()
        {
            return DocumentacionAD.ObtenerOntologiasEntorno();
        }

        /// <summary>
        /// Obtiene un dataset con una ontología a partir de su nombre en una comunidad.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pNombre">Nombre de la ontología</param>
        /// <returns>ID de la ontología</returns>
        public void ObtenerDatasetConOntologiaAPartirNombre(Guid pProyectoID, string pNombre, DataWrapperDocumentacion pDataWrapperDocumentacion)
        {
            DocumentacionAD.ObtenerDatasetConOntologiaAPartirNombre(pProyectoID, pNombre, pDataWrapperDocumentacion);
        }
        /// <summary>
        /// Obtiene una ontología a partir de su nombre en una comunidad.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pNombre">Nombre de la ontología</param>
        /// <returns>ID de la ontología</returns>
        public Guid ObtenerOntologiaAPartirNombre(Guid pProyectoID, string pNombre, bool pTraerSecundarias = true)
        {
            return DocumentacionAD.ObtenerOntologiaAPartirNombre(pProyectoID, pNombre, pTraerSecundarias);
        }

        /// <summary>
        /// Actualiza la foto de los recursos de una ontología.
        /// </summary>
        /// <param name="pOntologiaID">ID de la ontologia</param>
        /// <param name="pRutaFoto">Ruta de la foto</param>
        /// <param name="pBorrar">Indica si se ha borrado la foto</param>
        public void ActualizarFotoDocumentosDeOntologia(Guid pOntologiaID, string pRutaFoto, bool pBorrar)
        {
            DocumentacionAD.ActualizarFotoDocumentosDeOntologia(pOntologiaID, pRutaFoto, pBorrar);
        }

        /// <summary>
        /// Obtiene los últimos recursos de una determinada ontología.
        /// </summary>
        /// <param name="pOntologiaID">ID de ontología</param>
        /// <param name="pNumRec">Número de recursos</param>
        /// <returns>Lista con los IDs de los últimos recursos de la ontología</returns>
        public List<Guid> ObtenerUltimosRecursosDeOnto(Guid pOntologiaID, int pNumRec)
        {
            return DocumentacionAD.ObtenerUltimosRecursosDeOnto(pOntologiaID, pNumRec);
        }

        /// <summary>
        /// Indica si existe una ontología con determinado nombre en un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <param name="pEnlaceOntologia">Enlace de la ontología</param>
        /// <returns>TRUE si existe una ontología con determinado nombre en un proyecto, FALSE si no</returns>
        public bool ExisteOntologiaEnProyecto(Guid pProyectoID, string pEnlaceOntologia)
        {
            return DocumentacionAD.ExisteOntologiaEnProyecto(pProyectoID, pEnlaceOntologia, null);
        }

        /// <summary>
        /// Indica si existe una ontología con determinado nombre en un proyecto a parte de la indicada.
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <param name="pEnlaceOntologia">Enlace de la ontología</param>
        /// <param name="pDocumentoID">ID del documento con dicho enlace</param>
        /// <returns>TRUE si existe una ontología con determinado nombre en un proyecto, FALSE si no</returns>
        public bool ExisteOtraOntologiaEnProyecto(Guid pProyectoID, string pEnlaceOntologia, Guid pDocumentoID)
        {
            return DocumentacionAD.ExisteOntologiaEnProyecto(pProyectoID, pEnlaceOntologia, pDocumentoID);
        }

        /// <summary>
        /// Obtiene una lista con IDs de documentos y sus elementosvinculadosID.
        /// </summary>
        /// <param name="pDocIDs">IDs de documentos</param>
        /// <returns>Lista con IDs de documentos y sus elementosvinculadosID</returns>
        public Dictionary<Guid, Guid> ObtenerListaRecursosConElementoVinculadoID(List<Guid> pDocIDs)
        {
            return DocumentacionAD.ObtenerListaRecursosConElementoVinculadoID(pDocIDs);
        }

        /// <summary>
        /// Comprueba si un usuario es administrador de alguna comunidad que tenga una ontologia concreta
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pOntologia">Ontología de la que el usuario debe ser administrador</param>
        /// <returns>Verdad si el usuario administra alguna comunidad que contenga esta ontología</returns>
        public bool ComprobarUsuarioAdministraOntologia(Guid pUsuarioID, string pOntologia)
        {
            return DocumentacionAD.ComprobarUsuarioAdministraOntologia(pUsuarioID, pOntologia);
        }

        #endregion

        #region Documentos Web

        /// <summary>
        /// Comprueba si un documento ha sido editado por otra persona al mismo tiempo
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pFechaModificacion">Última fecha de modificación del documento</param>
        /// <param name="pNuevaVersionDocumentoID">Si esta versión del documento está obsoleta, esta variable devolverá el ID de la versión actual del documento</param>
        /// <returns>Verdad si hay error de concurrencia</returns>
        public ErroresConcurrencia ComprobarConcurrenciaDocumento(Guid pDocumentoID, DateTime pFechaModificacion, out Guid? pNuevaVersionDocumentoID)
        {
            return DocumentacionAD.ComprobarConcurrenciaDocumento(pDocumentoID, pFechaModificacion, out pNuevaVersionDocumentoID);
        }

        /// <summary>
        /// Obtiene los documentos web de una base de recursos pasada como parámetro
        /// </summary>
        public DataWrapperDocumentacion ObtenerDocumentosWebDeBaseRecursos(Guid pBaseRecursosID)
        {
            return DocumentacionAD.ObtenerDocumentosWebDeBaseRecursos(pBaseRecursosID);
        }

        /// <summary>
        /// Obtiene las vinculaciones de los documentos obsoletos de una base de recursos
        /// </summary>
        /// <param name="pBaseRecursosID">Base de recursos</param>
        /// <returns></returns>
        public DataWrapperDocumentacion ObtenerDocumentosObsoletosDeBaseRecursos(Guid pBaseRecursosID)
        {
            return DocumentacionAD.ObtenerDocumentosObsoletosDeBaseRecursos(pBaseRecursosID);
        }

        /// <summary>
        /// Obtiene identificadores de los documentos de la tabla Documento vinculados a un proyecto
        /// </summary>
        /// <param name="pTop">Tope de documentos recuperados</param>
        /// <param name="pProyectoID">ID del proyecto</param>
        public List<Guid> ObtenerDocumentosIDVinculadosAProyecto(Guid pProyectoID)
        {
            return DocumentacionAD.ObtenerDocumentosIDVinculadosAProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene registros de la tabla Documento vinculados a un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        public DataWrapperDocumentacion ObtenerDocumentosVinculadosAProyecto(Guid pProyectoID)
        {
            return DocumentacionAD.ObtenerDocumentosVinculadosAProyecto(pProyectoID);
        }

        public void ActualizarNumeroComentariosDocumento(Guid pDocumentoID)
        {
            DocumentacionAD.ActualizarNumeroComentariosDocumento(pDocumentoID);
        }

        /// <summary>
        /// Obtiene una lista de identificadores cuyo elemento vinculado es el ID de la ontología
        /// </summary>
        /// <param name="pOntologiaID">Identificador de la ontología</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Lista de identificadores de los documentos cuyo elemento vinculado es la ontología</returns>
        public List<Guid> ObtenerDocumentosIDVinculadosAOntologiaProyecto(Guid pOntologiaID, Guid pProyectoID)
        {
            return DocumentacionAD.ObtenerDocumentosIDVinculadosAOntologiaProyecto(pOntologiaID, pProyectoID);
        }

        /// <summary>
        /// Obtiene registros de la tabla Documento y DocumentoWevBin vinculados a un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        public DataWrapperDocumentacion ObtenerDocumentosVinculadosAProyectoYVinculaciones(Guid pProyectoID)
        {
            return DocumentacionAD.ObtenerDocumentosVinculadosAProyectoYVinculaciones(pProyectoID);
        }

        /// <summary>
        /// Devuelve true si el proyecto que se le pasa en el parametro contiene algún documento de tipo plantilla, false en caso contrario
        /// </summary>
        /// <param name="pProyectoID">Id del proyecto que queremos comprobar</param>
        /// <returns></returns>
        public bool ProyectoTieneDocsPlantillas(Guid pProyectoID)
        {
            return DocumentacionAD.ProyectoTieneDocsPlantillas(pProyectoID);
        }

        /// <summary>
        /// Obtiene registros de la tabla Documento de la base de recursos personal de un usuario
        /// </summary>
        /// <param name="pUsuarioID">ID del usuario</param>
        public DataWrapperDocumentacion ObtenerDocumentosDeBaseRecursosUsuario(Guid pUsuarioID)
        {
            return DocumentacionAD.ObtenerDocumentosDeBaseRecursosUsuario(pUsuarioID);
        }

        /// <summary>
        /// Obtiene los ids y su tipos de los documentos de una comunidad.
        /// </summary>
        /// <param name="pProyectoID">ID de comunidad</param>
        /// <returns>ids y su tipos de los documentos de una comunidad</returns>
        public Dictionary<Guid, short> ObtenerDocumentosYTipodeProyecto(Guid pProyectoID)
        {
            return DocumentacionAD.ObtenerDocumentosYTipodeProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene nivel certificacion
        /// </summary>
        /// <param name="pUsuarioID">ID del usuario</param>
        public short ObtenerNivelCertificacion(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerNivelCertificacion(pDocumentoID);
        }

        /// <summary>
        /// Obtiene nivel certificacion de la lista de documentos
        /// </summary>
        /// <param name="pUsuarioID">ID del usuario</param>
        public Dictionary<Guid, Dictionary<Guid, string>> ObtenerNivelCertificacionDeDocumentos(List<Guid> pDocumentosID, Guid pProyectoID)
        {
            return DocumentacionAD.ObtenerNivelCertificacionDeDocumentos(pDocumentosID, pProyectoID);
        }

        /// <summary>
        /// Obtiene niveles de certificacion de recursos de un proyecto
        /// </summary>
        /// <param name="pUsuarioID">ID del usuario</param>
        public Dictionary<int, List<Guid>> ObtenerNivelesCertificacionDeDocsEnProyecto(Guid pProyectoID)
        {
            return DocumentacionAD.ObtenerNivelesCertificacionDeDocsEnProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene registros de la tabla Documento de la base de recursos de una organización
        /// </summary>
        /// <param name="pOrganizacionID">ID de la organización</param>
        public DataWrapperDocumentacion ObtenerDocumentosDeBaseRecursosOrganizacion(Guid pOrganizacionID)
        {
            return DocumentacionAD.ObtenerDocumentosDeBaseRecursosOrganizacion(pOrganizacionID);
        }

        /// <summary>
        /// Obtiene registros de la tabla Documento de la base de recursos de un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        public List<Documento> ObtenerDocumentosIDDeBaseRecursosProyecto(Guid pProyectoID)
        {
            return DocumentacionAD.ObtenerDocumentosDeBaseRecursosProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene la fecha de creación del documento
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <returns>Devuelve DateTime con la fecha de creación del documento</returns>
        public long ObtenerFechaCreacionDocumento(Guid pDocumentoID)
        {
            return this.DocumentacionAD.ObtenerFechaCreacionDocumento(pDocumentoID);
        }

        /// <summary>
        /// Obtiene las comunidades web de la persona según un documento que tenga en estas, el cual es pasado como parametro.
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pIdentidadID">Identificador de persona</param>
        /// <param name="pDocumentoID">Identificador de documento</param>
        public void ObtenerComunidadesDePersonaPorDocumentoWEB(DataWrapperDocumentacion pDataWrapperDocumentacion, Guid pIdentidadID, Guid pDocumentoID)
        {
            this.DocumentacionAD.ObtenerComunidadesDePersonaPorDocumentoWEB(pDataWrapperDocumentacion, pIdentidadID, pDocumentoID);
        }

        /// <summary>
        /// Obtiene una lista con las bases de recursos en las que esta compartido un documento.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <returns>Lista con las bases de recursos en las que esta compartido el documento</returns>
        public List<Guid> ObtenerBREstaCompartidoDocPorID(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerBREstaCompartidoDocPorID(pDocumentoID);
        }

        /// <summary>
        /// Obtiene un diccionario con clave documento y valor las lista con los proyectos en los que esta compartido un documento.
        /// </summary>
        ///<param name="pListaDocumentoID">Lista de documentos</param>
        /// <returns>Lista con los proyectos en los que estan compartidos los documentos</returns>
        public Dictionary<Guid, List<Guid>> ObtenerProyectosEstanCompartidosDocsPorID(List<Guid> pListaDocumentoID)
        {
            return DocumentacionAD.ObtenerProyectosEstanCompartidosDocsPorID(pListaDocumentoID);
        }

        /// <summary>
        /// Obtiene una lista con las bases de recursos en las que esta compartida una wiki.
        /// </summary>
        /// <param name="pNombreDoc">Identificador de documento</param>
        /// <returns>Lista con las bases de recursos en las que esta compartida la wiki</returns>
        public List<Guid> ObtenerBREstaCompartidoWikiPorNombre(string pNombreDoc)
        {
            return DocumentacionAD.ObtenerBREstaCompartidoWikiPorNombre(pNombreDoc);
        }

        /// <summary>
        /// Obtiene las comunidades web de la persona según un documento wiki que tenga en estas, el cual es pasado como parametro.
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pIdentidadID">Identificador de persona</param>
        /// <param name="pNombreDoc">Nombre del documento Wiki</param>
        public void ObtenerComunidadesDePersonaPorNombreDocumentoWiki(DataWrapperDocumentacion pDataWrapperDocumentacion, Guid pIdentidadID, string pNombreDoc)
        {
            this.DocumentacionAD.ObtenerComunidadesDePersonaPorNombreDocumentoWiki(pDataWrapperDocumentacion, pIdentidadID, pNombreDoc);
        }

        /// <summary>
        /// Obtiene todas las categorías en la que está compartido un documento.
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pListaDocumentos">Identificadores de documento</param>
        public void ObtenerTodasCategoriasTesauroPorDocumentoID(DataWrapperDocumentacion pDataWrapperDocumentacion, List<Guid> pListaDocumentos)
        {
            this.DocumentacionAD.ObtenerTodasCategoriasTesauroPorDocumentoID(pDataWrapperDocumentacion, pListaDocumentos);
        }

        /// <summary>
        /// Obtiene el ID de un recurso a traves de su nombre y del proyecto al que pertenece
        /// </summary>
        /// <returns></returns>
        public Guid ObtenerDocumentoIDPorNombreYProyecto(string pNombre, Guid pProyectoID)
        {
            return DocumentacionAD.ObtenerDocumentoIDPorNombreYProyecto(pNombre, pProyectoID);
        }

        /// <summary>
        /// Obtiene el título del documento de un documento.
        /// </summary>
        /// <param name="pVotoID">Identificador del documento</param>
        /// <returns>Título del documento</returns>
        public string ObtenerTituloDocumentoPorID(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerTituloDocumentoPorID(pDocumentoID);
        }

        /// <summary>
        /// Obtiene el ID de un recurso a traves de su nombre y de la persona a la que pertenece
        /// </summary>
        /// <returns></returns>
        public Guid ObtenerDocumentoIDPorNombreYPersona(string pNombre, Guid pPersonaID)
        {
            return DocumentacionAD.ObtenerDocumentoIDPorNombreYPersona(pNombre, pPersonaID);
        }

        /// <summary>
        /// Obtiene el Enlace de un recurso
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns></returns>
        public string ObtenerEnlaceDocumentoPorDocumentoID(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerEnlaceDocumentoPorDocumentoID(pDocumentoID);
        }

        /// <summary>
        /// Obtiene los Enlaces de varios recursos
        /// </summary>
        /// <param name="pListaDocumentoID">Identificadores de los documentos</param>
        /// <returns></returns>
        public Dictionary<Guid, string> ObtenerEnlacesDocumentosPorDocumentoID(List<Guid> pListaDocumentoID)
        {
            return DocumentacionAD.ObtenerEnlacesDocumentosPorDocumentoID(pListaDocumentoID);
        }

        /// <summary>
        /// Obtiene el ID de un recurso a traves de su nombre y su proyecto
        /// </summary>
        /// <param name="pNombre">Nombre del documento</param>
        /// <param name="pProyectoID">Proyecto</param>
        /// <returns></returns>
        public Guid ObtenerDocumentoWikiPorNombreyProyecto(string pNombre, Guid pProyectoID)
        {
            return DocumentacionAD.ObtenerDocumentoWikiPorNombreyProyecto(pNombre, pProyectoID);
        }

        /// <summary>
        /// Comprueba si un recurso está compartido en una comunidad y no está eliminado
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento a comprobar</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que se quiere comprobar que esté compartido el recurso</param>
        /// <returns>Verdad si el recurso está compartido o publicado en esa comunidad</returns>
        public bool EstaDocumentoCompartidoEnProyecto(Guid pDocumentoID, Guid pProyectoID)
        {
            return DocumentacionAD.EstaDocumentoCompartidoEnProyecto(pDocumentoID, pProyectoID);
        }

        /// <summary>
        /// Obtiene los campos Titulo, ElementoVinculadoID y Tipo de un recurso
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento a obtener sus datos</param>
        /// <param name="pTitulo">Variable en la que se devovlerá el título del recurso</param>
        /// <param name="pElementoVinculadoID">Variable en la que se devolverá el campo ElementoVinculadoID (NULL si no tiene valor)</param>
        /// <param name="pTipo">Variable en la que se devolverá el campo Tipo del recurso</param>
        public void ObtenerTituloElementoVinculadoIDTipoDeRecurso(Guid pDocumentoID, out string pTitulo, out Guid? pElementoVinculadoID, out short pTipo)
        {
            DocumentacionAD.ObtenerTituloElementoVinculadoIDTipoDeRecurso(pDocumentoID, out pTitulo, out pElementoVinculadoID, out pTipo);
        }

        /// <summary>
        /// Actualiza el campo UltimaVersion de un documento que ha dejado de ser la última versión
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        public void ActualizarUltimaVersionDocumento(Guid pDocumentoID)
        {
            try
            {
                if (Transaccion != null)
                {
                    DocumentacionAD.ActualizarUltimaVersionDocumento(pDocumentoID);
                }
                else
                {
                    IniciarTransaccion();
                    {
                        DocumentacionAD.ActualizarUltimaVersionDocumento(pDocumentoID);
                        TerminarTransaccion(true);
                    }
                }
            }
            catch (DBConcurrencyException ex)
            {
                TerminarTransaccion(false);
                // Error de concurrencia
                mLoggingService.GuardarLogError(ex);
                throw new ErrorConcurrencia();
            }
            catch (DataException ex)
            {
                TerminarTransaccion(false);
                //Error interno de la aplicación				
                mLoggingService.GuardarLogError(ex);
                throw new ErrorInterno();
            }
            catch
            {
                TerminarTransaccion(false);
                throw;
            }
        }

        /// <summary>
        /// Actualiza el número de descargas y consultas de un documento.
        /// </summary>
        /// <param name="pDocID">Identificador del documento.</param>
        /// <param name="pNumVisitas">Número de visitas realizadas al documento</param>
        /// <param name="baseRecursosID">Identificados de BR de proyecto</param>
        public void ActualizarNumeroDescargasDocumento(Guid pDocID, Guid pBaseRecursosID)
        {
            DocumentacionAD.ActualizarNumeroDescargasDocumento(pDocID, pBaseRecursosID);
        }

        /// <summary>
        /// Actualiza el número de descargas y consultas de un documento.
        /// </summary>
        /// <param name="pDocID">Identificador del documento.</param>
        /// <param name="pNumVisitas">Número de visitas realizadas al documento</param>
        /// <param name="baseRecursosID">Identificados de BR de proyecto</param>
        public int ActualizarNumeroConsultasDocumento(Guid pDocID, int pNumVisitas, Guid pBaseRecursosID)
        {
            return DocumentacionAD.ActualizarNumeroConsultasDocumento(pDocID, pNumVisitas, pBaseRecursosID);
        }

        /// <summary>
        /// Actualiza el número de comentarios de un documento
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pBaseRecursosID">Identificador de la base de recursos en la que se ha añadido o eliminado el comentario</param>
        /// <param name="pEliminado">Verdad si se ha eliminado un comentario, falso en caso de que sea agregado</param>
        /// <param name="pEnPrivado">Verdad si el comentario se ha hecho en una comunidad privada</param>
        public void ActualizarNumeroComentariosDocumento(Guid pDocumentoID, Guid pBaseRecursosID, bool pEliminado, bool pEnPrivado)
        {
            DocumentacionAD.ActualizarNumeroComentariosDocumento(pDocumentoID, pBaseRecursosID, pEliminado, pEnPrivado);
        }


        public void ActualizarNumeroComentariosDocumento(Guid pDocumentoID, Guid pBaseRecursosID, int pNumComentarios, bool pEnPrivado)
        {
            DocumentacionAD.ActualizarNumeroComentariosDocumento(pDocumentoID, pBaseRecursosID, pNumComentarios, pEnPrivado);
        }

        /// <summary>
        /// Devuelve las filas de los documentos compartio
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pBaseRecursosID">Identificador de la base de recursos</param>
        /// <param name="pListaDocumentosID">Lista con los documentos</param>
        public void ObtenerFilasDocumentoWebDeBRDeListaDocumentos(DataWrapperDocumentacion pDataWrapperDocumentacion, Guid pBaseRecursosID, List<Guid> pListaDocumentosID)
        {
            DocumentacionAD.ObtenerFilasDocumentoWebDeBRDeListaDocumentos(pDataWrapperDocumentacion, pBaseRecursosID, pListaDocumentosID);
        }

        /// <summary>
        /// Devuelve las filas de los documentos compartio
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        public List<DocumentoWebVinBaseRecursosConProyectoID> ObtenerFilasDocumentoWebVinBaseRecDeDocumento(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerFilasDocumentoWebVinBaseRecDeDocumento(pDocumentoID);
        }

        /// <summary>
        /// Devuelve un DS con los 10 últimos recursos publicados y que no están borrados.
        /// </summary>
        /// <param name="pProyectoID">ProyectoID del que se traen los recursos publicados.</param>
        /// <returns></returns>
        public DataWrapperDocumentacion ObtenerUltimosRecursosPublicados(Guid pProyectoID, int pNumElementos)
        {
            return DocumentacionAD.ObtenerUltimosRecursosPublicados(pProyectoID, pNumElementos);
        }

        /// <summary>
        /// Devuelve una lista con los últimos ids de los recursos publicados en la comunidad pProyID
        /// </summary>
        /// <param name="pProyectoID">Proyecto del que se quieren ver los últimos recursos publicados.</param>
        /// <returns>lista Con los últimos ids de los recursos publicados.</returns>
        public List<Guid> ObtenerUltimosRecursosIDPublicados(Guid pProyectoID, int pNumElementos, string pOntologia = null)
        {
            return DocumentacionAD.ObtenerUltimosRecursosIDPublicados(pProyectoID, pNumElementos, pOntologia);
        }

        /// <summary>
        /// Devuelve un DS con los últimos recursos publicados por un perfil
        /// </summary>
        /// <param name="pProyectoID">Proyecto del que se quieren ver los últimos recursos publicados.</param>
        /// <returns>DS Con los últimos recursos publicados.</returns>
        [Obsolete]
        public DataWrapperDocumentacion ObtenerUltimosRecursosPublicadosPorPerfil(Guid pPerfilID, Guid pUsuarioID, int pNumElementos)
        {
            return DocumentacionAD.ObtenerUltimosRecursosPublicadosPorPerfil(pPerfilID, pUsuarioID, pNumElementos);
        }

        /// <summary>
        /// Devuelve un DS con los últimos debates  publicados y que no están borrados.
        /// </summary>
        /// <param name="pProyectoID">ProyectoID del que se traen los recursos publicados.</param>
        /// <returns></returns>
        public DataWrapperDocumentacion ObtenerUltimosDebatesPublicados(Guid pProyectoID, int pNumElementos)
        {
            return DocumentacionAD.ObtenerUltimosDebatesPublicados(pProyectoID, pNumElementos);
        }

        /// <summary>
        /// Devuelve un DS con los últimos preguntas publicados y que no están borrados.
        /// </summary>
        /// <param name="pProyectoID">ProyectoID del que se traen los recursos publicados.</param>
        /// <returns></returns>
        public DataWrapperDocumentacion ObtenerUltimasPreguntasPublicados(Guid pProyectoID, int pNumElementos)
        {
            return DocumentacionAD.ObtenerUltimasPreguntasPublicados(pProyectoID, pNumElementos);
        }

        /// <summary>
        /// Devuelve un DS con los últimos debates y preguntas publicados y que no están borrados.
        /// </summary>
        /// <param name="pProyectoID">ProyectoID del que se traen los recursos publicados.</param>
        /// <returns></returns>
        public DataWrapperDocumentacion ObtenerUltimosDebatesPreguntasPublicados(Guid pProyectoID, int pNumElementos)
        {
            return DocumentacionAD.ObtenerUltimosDebatesPreguntasPublicados(pProyectoID, pNumElementos);
        }

        /// <summary>
        /// Devuelve una lista con los IDs de los últimos debates y preguntas publicados y que no están borrados.
        /// </summary>
        /// <param name="pProyectoID">ProyectoID del que se traen los recursos publicados.</param>
        /// <returns>Lista de documentoID</returns>
        public List<Guid> ObtenerListaUltimosDebatesPreguntasPublicados(Guid pProyectoID, int pNumElementos)
        {
            return DocumentacionAD.ObtenerListaUltimosDebatesPreguntasPublicados(pProyectoID, pNumElementos, true, true);
        }

        /// <summary>
        /// Devuelve una lista con los IDs de los últimos debates  publicados y que no están borrados.
        /// </summary>
        /// <param name="pProyectoID">ProyectoID del que se traen los recursos publicados.</param>
        /// <returns>Lista de documentoID</returns>
        public List<Guid> ObtenerListaUltimosDebatesPublicados(Guid pProyectoID, int pNumElementos)
        {
            return DocumentacionAD.ObtenerListaUltimosDebatesPreguntasPublicados(pProyectoID, pNumElementos, true, false);
        }

        /// <summary>
        /// Devuelve una lista con los IDs de los últimos preguntas publicados y que no están borrados.
        /// </summary>
        /// <param name="pProyectoID">ProyectoID del que se traen los recursos publicados.</param>
        /// <returns>Lista de documentoID</returns>
        public List<Guid> ObtenerListaUltimasPreguntasPublicados(Guid pProyectoID, int pNumElementos)
        {
            return DocumentacionAD.ObtenerListaUltimosDebatesPreguntasPublicados(pProyectoID, pNumElementos, false, true);
        }

        /// <summary>
        /// Actualiza la fecha del documento de la base de recursos pasada
        /// </summary>
        /// <param name="pDocumentoID"></param>
        /// <param name="pBaseRecursosID"></param>
        /// <param name="pFechaUltimaVisita"></param>
        public void ActualizarFechaUltimaVisitaDocumento(Guid pDocumentoID, Guid pBaseRecursosID, DateTime pFechaUltimaVisita)
        {
            DocumentacionAD.ActualizarFechaUltimaVisitaDocumento(pDocumentoID, pBaseRecursosID, pFechaUltimaVisita);
        }

        #endregion

        #region Base de Recursos

        /// <summary>
        /// Obtiene la base de recursos de un determinado proyecto.
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pOrganizacionID">Identificador de organziación</param>
        public void ObtenerBaseRecursosProyecto(DataWrapperDocumentacion pDataWrapperDocumentacion, Guid pProyectoID, Guid pOrganizacionID, Guid pUsuarioID)
        {
            if (pProyectoID == ProyectoAD.MetaProyecto)
            {
                //Si el proyecto es MetaProyecto obtengo la base de recursos del usuario.
                ObtenerBaseRecursosUsuario(pDataWrapperDocumentacion, pUsuarioID);
            }
            else
            {
                DocumentacionAD.ObtenerBaseRecursosProyecto(pDataWrapperDocumentacion, pProyectoID, pOrganizacionID);
            }
        }

        /// <summary>
        /// Obtiene la base de recursos de un determinado proyecto.
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        public void ObtenerBaseRecursosProyecto(DataWrapperDocumentacion pDataWrapperDocumentacion, Guid pProyectoID)
        {
            DocumentacionAD.ObtenerBaseRecursosProyecto(pDataWrapperDocumentacion, pProyectoID);
        }

        /// <summary>
        /// Comprueba si una comunidad contiene el recurso o no
        /// </summary>
        /// <param name="id">Identificador del recurso</param>
        /// <param name="pOrganizacionDestinoID">Identificador de la organizacion de búsqueda</param>
        /// <param name="pProyectoDestinoID">Identificador de la comunidad de búsqueda</param>
        /// <returns>Booleano true si esa comunidad contiene ese recurso, false en caso contrario</returns>
        public bool ContieneRecurso(Guid id, Guid pOrganizacionID, Guid pProyectoID)
        {
            return DocumentacionAD.ContieneRecurso(id, pOrganizacionID, pProyectoID);
        }


        /// <summary>
        /// Obtiene los ID de las bases de recursos de proyectos (Proy,BR).
        /// </summary>
        /// <param name="pListaProyectosID">Identificadores de proyecto</param>
        public Dictionary<Guid, Guid> ObtenerBasesRecursosIDporProyectosID(List<Guid> pListaProyectosID)
        {
            return DocumentacionAD.ObtenerBasesRecursosIDporProyectosID(pListaProyectosID);
        }

        /// <summary>
        /// Obtiene los ID de las bases de recursos de organizaciones (Org,BR).
        /// </summary>
        /// <param name="pListaOrgsID">Identificadores de organizaciones</param>
        public Dictionary<Guid, Guid> ObtenerBasesRecursosIDporOrganizacionesID(List<Guid> pListaOrgsID)
        {
            return DocumentacionAD.ObtenerBasesRecursosIDporOrganizacionesID(pListaOrgsID);
        }


        /// <summary>
        /// Obtiene un array cuyo primer elemento es la identidad del creador del recurso  la segunda componente es la organizacion que lo tiene en su base de recursos de organizacion
        /// </summary>
        /// <param name="pRecursoID">Recurso que busca</param>        
        /// <returns>Array cuyo primer elemento es la identidad del creador del recurso  la segunda componente es la organizacion que lo tiene en su base de recursos de organizacion</returns>
        public Dictionary<Guid, Guid> ObtenerIdentidadyOrganizacionIDdeRecurso(Guid pRecursoID)
        { return DocumentacionAD.ObtenerIdentidadyOrganizacionIDdeRecurso(pRecursoID); }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pRecursoID">Recurso que busca</param>        
        /// <returns></returns>
        public List<Guid> ObtenerPerfilesIDEstaCompartidoYEliminadoRecurso(Guid pRecursoID)
        { return DocumentacionAD.ObtenerPerfilesIDEstaCompartidoYEliminadoRecurso(pRecursoID); }

        /// <summary>
        /// Obtiene un Guid que es la organizacion del creador del recurso(Guid.Empty si no es de Organizacion) 
        /// </summary>
        /// <param name="pRecursoID">Recurso que busca</param>        
        /// <returns>Guid que es la organizacion del creador del recurso(Guid.Empty si no es de Organizacion) </returns>
        public Guid ObtenerOrganizacionPublicadorIDdeRecurso(Guid pRecursoID)
        { return DocumentacionAD.ObtenerOrganizacionPublicadorIDdeRecurso(pRecursoID); }

        /// <summary>
        /// Obtiene la base de recursos de un usuario.Obtiene tablas "BaseRecursos" , "BaseRecursosUsuario"
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        public void ObtenerBaseRecursosUsuario(DataWrapperDocumentacion pDataWrapperDocumentacion, Guid pUsuarioID)
        {
            DocumentacionAD.ObtenerBaseRecursosUsuario(pDataWrapperDocumentacion, pUsuarioID);
        }

        /// <summary>
        /// Obtiene la clave de la base de recursos de un usuario.
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Clave de la base de recursos del usuario</returns>
        public Guid ObtenerBaseRecursosIDUsuario(Guid pUsuarioID)
        {
            return DocumentacionAD.ObtenerBaseRecursosIDUsuario(pUsuarioID);
        }

        /// <summary>
        /// Obtiene la clave de la base de recursos de un proyecto.
        /// </summary>
        /// <param name="pUsuarioID">Identificador de proyecto</param>
        /// <returns>Clave de la base de recursos del proyecto</returns>
        public Guid ObtenerBaseRecursosIDProyecto(Guid pProyectoID)
        {
            return DocumentacionAD.ObtenerBaseRecursosIDProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene la clave de la base de recursos de una Organizacion.
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la Organizacion</param>
        /// <returns>Clave de la base de recursos de la Organizacion</returns>
        public Guid ObtenerBaseRecursosIDOrganizacion(Guid pOrganizacionID)
        {
            return DocumentacionAD.ObtenerBaseRecursosIDOrganizacion(pOrganizacionID);
        }

        public List<Guid> ObtenerDocumentosIDSuscripcionPerfilEnProyecto(Guid pPerfilID, Guid pProyectoID, int pNumElementos)
        {
            return DocumentacionAD.ObtenerDocumentosIDSuscripcionPerfilEnProyecto(pPerfilID, pProyectoID, pNumElementos);
        }

        public List<Guid> ObtenerDocumentosIDActividadRecienteEnProyecto(Guid pProyectoID)
        {
            return DocumentacionAD.ObtenerDocumentosIDActividadRecienteEnProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene la base de recursos de una organización. Obtiene tablas "BaseRecursos" , "BaseRecursosOrganizacion"
        /// </summary>
        /// <param name="pDocumentacionDS">DataSet de documentación</param>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        public void ObtenerBaseRecursosOrganizacion(DataWrapperDocumentacion pDocumentacionDS, Guid pOrganizacionID)
        {
            DocumentacionAD.ObtenerBaseRecursosOrganizacion(pDocumentacionDS, pOrganizacionID);
        }

        /// <summary>
        /// Devuelve las vinculaciones de los documentos de una organziación. Carga "DocumentoWebVinBaseRecursos" , "DocumentoWebAgCatTesauro"
        /// </summary>
        /// <param name="pOrganizacionID">Clave de la organizacion</param>
        /// <returns>DocumentacionDS</returns>
        public DataWrapperDocumentacion ObtenerVinculacionesDocumentosDeOrganizacion(Guid pOrganizacionID)
        {
            return DocumentacionAD.ObtenerVinculacionesDocumentosDeOrganizacion(pOrganizacionID);
        }

        #endregion

        #region VersionDocumento

        /// <summary>
        /// Finaliza la edición de un recurso, eliminando la fila de DocumentoEnEdicion
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        public void FinalizarEdicionRecurso(Guid pDocumentoID)
        {
            DocumentacionAD.FinalizarEdicionRecurso(pDocumentoID);
        }

        /// <summary>
        /// Obtiene la fecha de edición de un recurso, si está bloqueado
        /// </summary>
        /// <param name="pDocumentoID">Identificador del recurso</param>
        /// <returns></returns>
        public DateTime? ObtenerFechaRecursoEnEdicion(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerFechaRecursoEnEdicion(pDocumentoID);
        }

        /// <summary>
        /// Le añade al tiempo de bloqueo de un recurso otros 60 segundos
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento a bloquear durante otros 60 segundos</param>
        public DateTime? ActualizarFechaRecursoEnEdicion(Guid pDocumentoID, DateTime pFechaEdicion)
        {
            return DocumentacionAD.ActualizarFechaRecursoEnEdicion(pDocumentoID, pFechaEdicion);
        }

        /// <summary>
        /// Comprueba si un documento está siendo actualizado por algún usuario en este instante. Si no es así, lo marca en edición
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento a comprobar</param>
        /// <param name="pIdentidadID">Identificador de la identidad del editor</param>
        /// <returns>La fila del documento en edición, null si nadie lo está editando</returns>
        public DocumentoEnEdicion ComprobarDocumentoEnEdicion(Guid pDocumentoID, Guid pIdentidadID, int pSegundosDuracionBloqueo = 60, int pNumeroIntentos = 3)
        {
            return DocumentacionAD.ComprobarDocumentoEnEdicion(pDocumentoID, pIdentidadID, pNumeroIntentos, pSegundosDuracionBloqueo);
        }

        /// <summary>
        /// Carga la tabla VersionDocumento para los documentos pasados en la lista.
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pListaDocumentosID">Lista de identificadores de los documentos para traer la información</param>
        /// <param name="pRelaciones">Especifica si deben traerse todas la versiones de los documentos originales o no</param>
        public void ObtenerVersionDocumentosPorIDs(DataWrapperDocumentacion pDataWrapperDocumentacion, List<Guid> pListaDocumentosID, bool pRelaciones)
        {
            DocumentacionAD.ObtenerVersionDocumentosPorIDs(pDataWrapperDocumentacion, pListaDocumentosID, pRelaciones);
        }

        /// <summary>
        /// Carga todas las versiones de un documento.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento del que se quieren obtener las versiones</param>
        /// <returns>Identificadores de los documentos</returns>
        public Dictionary<Guid, int> ObtenerVersionesDocumentoIDPorID(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerVersionesDocumentoIDPorID(pDocumentoID);
        }

        /// <summary>
        /// Carga todas las versiones de un documento.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento del que se quieren obtener las versiones</param>
        /// <returns>DataSet de documentación</returns>
        public DataWrapperDocumentacion ObtenerVersionesDocumentoPorID(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerVersionesDocumentoPorID(pDocumentoID);
        }

        /// <summary>
        /// Verdad si el documento está marcado como privado para editores
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns>Verdad si el documento está marcado como privado para editores</returns>
        public bool EsDocumentoEnProyectoPrivadoEditores(Guid pDocumentoID, Guid pProyectoID)
        {
            return DocumentacionAD.EsDocumentoEnProyectoPrivadoEditores(pDocumentoID, pProyectoID);
        }

        /// <summary>
        /// Verdad si el documento está marcado como publico sólo para los miembros de la comunidad
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns>Verdad si el documento está marcado como publico sólo para los miembros de la comunidad</returns>
        public bool EsDocumentoEnProyectoPublicoSoloParaMiembros(Guid pDocumentoID)
        {
            return DocumentacionAD.EsDocumentoEnProyectoPublicoSoloParaMiembros(pDocumentoID);
        }

        /// <summary>
        /// Obtiene el Identificador del proyecto en el que se ha creado un recurso
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns>Identificador de proyecto</returns>
        public Guid ObtenerProyectoIDPorDocumentoID(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerProyectoIDPorDocumentoID(pDocumentoID);
        }

        /// <summary>
        /// Obtiene la lista de categorias de un recurso en una BR de una persona.
        /// </summary>
        /// <param name="pPersonaID">ID de la persona</param>
        /// <param name="pDocumentoID">ID del documento</param>
        /// <returns>Lista de categorias de un recurso en una BR de una persona</returns>
        public List<Guid> ObtenerListaIDsCategoriasBRPersonalPersonsaDeDocumento(Guid pPersonaID, Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerListaIDsCategoriasBRPersonalPersonsaDeDocumento(pPersonaID, pDocumentoID);
        }

        /// <summary>
        /// Devuelve true si el enlace de un recurso esta en la comunidad dada, false si no contiene ese enlace (recurso).El enlace se busca dentro de la descripcion
        /// </summary>
        /// <param name="pEnlace">Url del enlace del recurso</param>
        /// <param name="pProyectoID">Identificador de la comunidad</param>
        /// <returns>Devuelve si está o no el documento con "pEnlace" en la comunidad "pProyectoID"</returns>
        public bool EstaEnlaceEnComunidad(string pEnlace, Guid pProyectoID)
        {
            return DocumentacionAD.EstaEnlaceEnComunidad(pEnlace, pProyectoID);
        }

        /// <summary>
        /// Devuleve el estado del documento 0 ->No eliminado, 1 -> Eliminado, 2 -> No existe
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns>Devuleve el estado del documento</returns>
        public int EstaDocumentoEliminado(Guid pDocumentoID)
        {
            return DocumentacionAD.EstaDocumentoEliminado(pDocumentoID);
        }


        /// <summary>
        /// Obtiene los editores de un documento
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns></returns>
        public DataWrapperDocumentacion ObtenerEditoresDocumento(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerEditoresDocumento(pDocumentoID);
        }

        public Guid ObtenerPerfilPublicadorDocumento(Guid pDocumentoID, Guid pProyectoID)
        {
            return DocumentacionAD.ObtenerPerfilPublicadorDocumento(pDocumentoID, pProyectoID);
        }

        /// <summary>
        /// Obtiene los editores de una lista de documentos.
        /// </summary>
        /// <param name="pListaDocumentosID">Identificadores de los documentos</param>
        /// <returns>Tabla DocumentoRolIdentidad cargada con los editores de los documentos</returns>
        public DataWrapperDocumentacion ObtenerEditoresDocumentos(List<Guid> pListaDocumentosID)
        {
            return DocumentacionAD.ObtenerEditoresDocumentos(pListaDocumentosID);
        }




        #endregion

        #region Votos y Comentarios

        /// <summary>
        /// Obtiene los comentarios y votos de los documentos.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento.</param>
        /// <param name="pDocumentacionDW">DataSet de documentación</param>
        public void ObtenerComenariosYVotosDocumento(Guid pDocumentoID, DataWrapperDocumentacion pDocumentacionDW)
        {
            DocumentacionAD.ObtenerComenariosYVotosDocumento(pDocumentoID, pDocumentacionDW);
        }

        /// <summary>
        /// Obtiene los comentarios de un documento.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento.</param>
        /// <param name="pDocumentacionDW">DataSet de documentación</param>
        public void ObtenerComenariosDocumento(Guid pDocumentoID, DataWrapperDocumentacion pDocumentacionDW)
        {
            DocumentacionAD.ObtenerComenariosDocumento(pDocumentoID, pDocumentacionDW);
        }

        /// <summary>
        /// Obtiene los comentadores de un documento.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento.</param>
        /// <param name="pDocumentacionDS">Identificador de un proyecto</param>
        public List<string> ObtenerComentadores(Guid pDocumentoID, Guid pProyectoID)
        {
            return DocumentacionAD.ObtenerComentadores(pDocumentoID, pProyectoID);
        }

        /// <summary>
        /// Obtiene las Visitas de un documento.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento.</param>
        /// <param name="pDocumentacionDS">Identificador de un proyecto</param>
        public string ObtenerNumeroVisitas(Guid pDocumentoID, Guid pBaseRecursoID)
        {
            return DocumentacionAD.ObtenerNumeroVisitas(pDocumentoID, pBaseRecursoID);
        }

        /// <summary>
        /// Obtiene los votos de un documento.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento.</param>
        /// <param name="pDocumentacionDS">DataSet de documentación</param>
        public void ObtenerVotosDocumento(Guid pDocumentoID, DataWrapperDocumentacion pDocumentacionDS)
        {
            DocumentacionAD.ObtenerVotosDocumento(pDocumentoID, pDocumentacionDS);
        }

        /// <summary>
        /// Obtiene los perfiles de los autores de los comentarios de un documento en un proyecto.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <param name="pTraerEliminados">Indica si se deben traer los perfiles autores de los comentarios eliminados de documentos</param>
        /// <returns>Lista de PerfilesID de los autores de los comentarios de un documento en un proyecto</returns>
        public List<Guid> ObtenerPerfilesAutoresComenariosDocumento(Guid pDocumentoID, Guid pProyectoID, bool pTraerEliminados)
        {
            return DocumentacionAD.ObtenerPerfilesAutoresComenariosDocumento(pDocumentoID, pProyectoID, pTraerEliminados);
        }

        /// <summary>
        /// Actualiza la valoración del documento según su número de votos.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns>DataSet de documentación con la información actulizada</returns>
        public DataWrapperDocumentacion ActualizarValoracionDocumento(Guid pDocumentoID)
        {
            return DocumentacionAD.ActualizarValoracionDocumento(pDocumentoID);
        }
        public void EjecutarScriptCargaMasiva(string pScript)
        {
            DocumentacionAD.EjecutarScriptCargaMasiva(pScript);
        }
        /// <summary>
        /// Actualiza la fila de la cola de cargas de un CSV a un estado.
        /// </summary>
        /// <param name="pClaveCSV">Clave del archivo CSV</param>
        /// <param name="pEstado">Estado</param>
        /// <returns>TRUE si todo ha ido bien</returns>
        public void ActualizarRecursoColaCargaRecursosAEstado(Guid pClaveCSV, short pEstado)
        {
            DocumentacionAD.ActualizarRecursoColaCargaRecursosAEstado(pClaveCSV, pEstado);
        }

        /// <summary>
        /// Obtiene los comentarios de comunidades públicas y la comunidad actual de los documentos cargados en la tabla Documento.
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pProyectoActual">Proyecto actual en el que está el usuario</param>
        public void ObtenerComentariosPublicosMasProyectoActualDeDocumentos(DataWrapperDocumentacion pDataWrapperDocumentacion, Guid pProyectoActual)
        {
            DocumentacionAD.ObtenerComentariosPublicosMasProyectoActualDeDocumentos(pDataWrapperDocumentacion, pProyectoActual);
        }

        #endregion

        #region Categorías del tesauro

        /// <summary>
        /// Devuelve el dataSet de documentación con las categorías del tesauro del los documentos requeridas.
        /// </summary>
        /// <param name="pListaDocumentosID">Lista con los identificadores de los documentos</param>
        /// <returns>DataSet de documentación con la tabla DocumentoWebAgCatTesauro</returns>
        public List<DocumentoWebAgCatTesauro> ObtenerCategoriasTesauroListaDocumentosProyectoID(List<Guid> pListaDocumentosID, Guid pProyectoID)
        {
            return DocumentacionAD.ObtenerCategoriasTesauroListaDocumentosProyectoID(pListaDocumentosID, pProyectoID);
        }

        /// <summary>
        /// Devuelve el dataSet de documentación con las categorías del tesauro del los documentos requeridas.
        /// </summary>
        /// <param name="pListaDocumentosID">Lista con los identificadores de los documentos</param>
        /// <returns>DataSet de documentación con la tabla DocumentoWebAgCatTesauro</returns>
        public List<DocumentoWebAgCatTesauroConVinculoTesauroID> ObtenerCategoriasTesauroYTesauroDeDocumentos(List<Guid> pListaDocumentosID)
        {
            return DocumentacionAD.ObtenerCategoriasTesauroYTesauroDeDocumentos(pListaDocumentosID);
        }

        /// <summary>
        /// Devuelve la tabla DocumentoWebAgCatTesauro cargada con los datos del las categorías de un determinado tesauro.
        /// </summary>
        /// <param name="pTesauroID">Identificador del tesauro</param>
        /// <returns>Tabla DocumentoWebAgCatTesauro cargada con los datos del las categorías de un determinado tesauro</returns>
        public DataWrapperDocumentacion ObtenerDocAgCatTesauroDeTesauroID(Guid pTesauroID)
        {
            return DocumentacionAD.ObtenerDocAgCatTesauroDeTesauroID(pTesauroID);
        }

        /// <summary>
        /// Devuelve la tabla DocumentoWebAgCatTesauro cargada con las vinculaciones entre recursos publicos y categorías
        /// </summary>
        /// <param name="pTesauroID">Identificador del tesauro</param>
        /// <returns>Tabla DocumentoWebAgCatTesauro cargada con los datos del las categorías de un determinado tesauro</returns>
        public List<DocumentoWebAgCatTesauro> ObtenerDocAgCatRecursosPublicosTesauroDeTesauroID(Guid pTesauroID)
        {
            return DocumentacionAD.ObtenerDocAgCatRecursosPublicosTesauroDeTesauroID(pTesauroID);
        }

        /// <summary>
        /// Devuelve la tabla DocumentoWebAgCatTesauro cargada con las vinculaciones entre debates publicos y categorías
        /// </summary>
        /// <param name="pTesauroID">Identificador del tesauro</param>
        /// <returns>Tabla DocumentoWebAgCatTesauro cargada con los datos del las categorías de un determinado tesauro</returns>
        public List<DocumentoWebAgCatTesauro> ObtenerDocAgCatDebatesPublicosTesauroDeTesauroID(Guid pTesauroID)
        {
            return DocumentacionAD.ObtenerDocAgCatDebatesPublicosTesauroDeTesauroID(pTesauroID);
        }

        /// <summary>
        /// Devuelve la tabla DocumentoWebAgCatTesauro cargada con las vinculaciones entre preguntas publicos y categorías
        /// </summary>
        /// <param name="pTesauroID">Identificador del tesauro</param>
        /// <returns>Tabla DocumentoWebAgCatTesauro cargada con los datos del las categorías de un determinado tesauro</returns>
        public List<DocumentoWebAgCatTesauro> ObtenerDocAgCatPreguntasPublicasTesauroDeTesauroID(Guid pTesauroID)
        {
            return DocumentacionAD.ObtenerDocAgCatPreguntasPublicasTesauroDeTesauroID(pTesauroID);
        }

        /// <summary>
        /// Devuelve los recursos asociados a la lista de categorias que recibe.
        /// </summary>
        /// <param name="pListaCategorias">Lista de categorías de los cuales se quiere obtener los recursos</param>
        /// <returns></returns>
        public DataWrapperDocumentacion ObtenerDocWebAgCatTesauroPorCategoriasId(List<Guid> pListaCategorias)
        {
            return DocumentacionAD.ObtenerDocWebAgCatTesauroPorCategoriasId(pListaCategorias);
        }

        /// <summary>
        /// Devuelve la tabla DocumentoWebAgCatTesauro cargada con las vinculaciones entre recursos privados y categorías
        /// </summary>
        /// <param name="pTesauroID">Identificador del tesauro</param>
        /// <returns>Tabla DocumentoWebAgCatTesauro cargada con los datos del las categorías de un determinado tesauro</returns>
        public List<DocumentoWebAgCatTesauro> ObtenerDocAgCatRecursosPrivadosTesauroDeTesauroID(Guid pTesauroID)
        {
            return DocumentacionAD.ObtenerDocAgCatRecursosPrivadosTesauroDeTesauroID(pTesauroID);
        }

        /// <summary>
        /// Devuelve la tabla DocumentoWebAgCatTesauro cargada con las vinculaciones entre debates privados y categorías
        /// </summary>
        /// <param name="pTesauroID">Identificador del tesauro</param>
        /// <returns>Tabla DocumentoWebAgCatTesauro cargada con los datos del las categorías de un determinado tesauro</returns>
        public List<DocumentoWebAgCatTesauro> ObtenerDocAgCatDebatesPrivadosTesauroDeTesauroID(Guid pTesauroID)
        {
            return DocumentacionAD.ObtenerDocAgCatDebatesPrivadosTesauroDeTesauroID(pTesauroID);
        }

        /// <summary>
        /// Devuelve la tabla DocumentoWebAgCatTesauro cargada con las vinculaciones entre preguntas privados y categorías
        /// </summary>
        /// <param name="pTesauroID">Identificador del tesauro</param>
        /// <returns>Tabla DocumentoWebAgCatTesauro cargada con los datos del las categorías de un determinado tesauro</returns>
        public List<DocumentoWebAgCatTesauro> ObtenerDocAgCatPreguntasPrivadasTesauroDeTesauroID(Guid pTesauroID)
        {
            return DocumentacionAD.ObtenerDocAgCatPreguntasPrivadasTesauroDeTesauroID(pTesauroID);
        }

        /// <summary>
        /// Devuelve una lista de GUID con las vinculaciones entre documentos y una categoría concreta
        /// </summary>
        /// <param name="pCategoriaTesauroID">Identificador de la categoría</param>
        /// <returns>Lista con los documentos
        public List<Guid> ObtenerListaDocsAgCatDeCategoriaTesauroID(Guid pCategoriaTesauroID)
        {
            return DocumentacionAD.ObtenerListaDocsAgCatDeCategoriaTesauroID(pCategoriaTesauroID);
        }

        /// <summary>
        /// Devuelve una lista de GUID con las vinculaciones entre documentos y las categorías de un tesauro
        /// </summary>
        /// <param name="pCategoriaTesauroID">Identificador de la categoría</param>
        /// <returns>Lista con los documentos
        public List<Guid> ObtenerListaDocsAgCatDeTesauroID(Guid pTesauroID)
        {
            return DocumentacionAD.ObtenerListaDocsAgCatDeTesauroID(pTesauroID);
        }

        /// <summary>
        /// Obtiene un dictionary cuya clave es un documento y el valor es una lista de Guid de las identidades de los editores
        /// </summary>
        /// <param name="pDocsID">Lista de identificadores de los documentos</param>
        /// <returns>dictionary cuya clave es un documento y el valor es una lista de Guid de los editores</returns>
        public Dictionary<Guid, List<Guid>> ObtenerListaEditoresDeDocumentosPrivadosEnProyecto(List<Guid> pDocsID, Guid pProyectoID)
        {
            return DocumentacionAD.ObtenerListaEditoresDeDocumentosPrivadosEnProyecto(pDocsID, pProyectoID);
        }

        #endregion

        #region Documentos Temporales

        /// <summary>
        /// Obtiene los documentos temporales creados a partir de otro no temporal.
        /// </summary>
        /// <param name="pDocumentoOriginalID">Documento no temporal del que se ha realizado las versiones temporales</param>
        /// <param name="pIdentidadID">Identidad que ha creado los documentos temporales</param>
        /// <param name="pTipoDocumento">Indica el tipo de documento temporal o null si es indiferente</param>
        /// <returns></returns>
        public DataWrapperDocumentacion ObtenerDocumentosTemporalesDeDocumento(Guid pDocumentoOriginalID, Guid pIdentidadID, TiposDocumentacion? pTipoDocumento)
        {
            return DocumentacionAD.ObtenerDocumentosTemporalesDeDocumento(pDocumentoOriginalID, pIdentidadID, pTipoDocumento);
        }

        /// <summary>
        /// Obtiene los documentos temporales creados a partir de otro no temporal.
        /// </summary>
        /// <param name="pNombreDocumentoOriginal">Nombre del documento temporal</param>
        /// <param name="pIdentidadID">Identidad que ha creado los documentos temporales</param>
        /// <param name="pTipoDocumento">Indica el tipo de documento temporal o null si es indiferente</param>
        /// <returns></returns>
        public DataWrapperDocumentacion ObtenerDocumentosTemporalesDeDocumentoPorNombre(string pNombreDocumentoOriginal, Guid pIdentidadID, TiposDocumentacion? pTipoDocumento)
        {
            return DocumentacionAD.ObtenerDocumentosTemporalesDeDocumentoPorNombre(pNombreDocumentoOriginal, pIdentidadID, pTipoDocumento);
        }

        #endregion

        #region Documento NewsLetter

        /// <summary>
        /// Obtiene los datos de los envios realizados de un documento de tipo newsletter.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento newletter</param>
        /// <returns>DataSet con los datos cargados de envio de documentos</returns>
        public DataWrapperDocumentacion ObtenerEnviosNewsLetterPorDocumentoID(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerEnviosNewsLetterPorDocumentoID(pDocumentoID);
        }

        #endregion

        #region Cola Documento

        /// <summary>
        /// Agrega el documento a la cola para que sea procesado por un servicio.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pDocAgregado">Indica si el documento a sido agregado (TRUE), o es modificado (FALSE)</param>
        public void AgregarDocumentoAColaTareas(Guid pDocumentoID, bool pDocAgregado, PrioridadColaDocumento pPrioridadColaDocumento, long pEstadoCargaID)
        {
            DocumentacionAD.AgregarDocumentoAColaTareas(pDocumentoID, pDocAgregado, pPrioridadColaDocumento, pEstadoCargaID);
        }

        /// <summary>
        /// Obtiene los documentos que están en la ColaDocumento sin procesar y sin desechar.
        /// </summary>
        /// <param name="pNumDoc">Número de documentos para procesar</param>
        /// <returns>DataSet de documentación con los datos cargados</returns>
        public DataWrapperDocumentacion ObtenerDocumentosEnColaParaProcesar(int pNumDoc)
        {
            return DocumentacionAD.ObtenerDocumentosEnColaParaProcesar(pNumDoc);
        }

        public DataWrapperDocumentacion ObtenerDocumentosColaDocumentoRabbitMQ(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerDocumentosColaDocumentoRabbitMQ(pDocumentoID);
        }

        /// <summary>
        /// Devuelve la versión de la imagen o Nulo si no tiene imagen
        /// </summary>
        /// <param name="pDocumentoID">Documentoid</param>
        /// <returns></returns>
        public int? ComprobarSiDocumentoTieneImagen(Guid pDocumentoID)
        {
            List<Guid> listaDocs = new List<Guid>();
            listaDocs.Add(pDocumentoID);
            return ComprobarSiListaDocumentosTienenImagen(listaDocs)[pDocumentoID];
        }

        /// <summary>
        /// Obtiene las imagenes de los documentos
        /// </summary>
        /// <param name="pDocumentosID">Lista de identificadores de documento</param>
        /// <returns>Lista con el campo NombreCategoriaDoc </returns>
        public List<string> ObtenerImagenDocumentos(List<Guid> pDocumentosID)
        {
            return DocumentacionAD.ObtenerImagenDocumentos(pDocumentosID);
        }

        /// <summary>
        /// Comprueba si el documento actual es la última versión
        /// </summary>
        /// <param name="pDocumentoID">Documentoid</param>
        /// <returns></returns>
        public bool ComprobarSiEsUltimaVersionDocumento(Guid pDocumentoID)
        {
            return DocumentacionAD.ComprobarSiEsUltimaVersionDocumento(pDocumentoID);
        }

        /// <summary>
        /// Devuelve las versiónes de la imagenes o Nulo si no tienen imagen
        /// </summary>
        /// <param name="pListaDocumentos">Lista de documentos</param>
        /// <returns></returns>
        public Dictionary<Guid, int?> ComprobarSiListaDocumentosTienenImagen(List<Guid> pListaDocumentos)
        {
            return DocumentacionAD.ComprobarSiListaDocumentosTienenImagen(pListaDocumentos);
        }

        /// <summary>
        /// Devuelve si las ontologias tienen recursos asociados
        /// </summary>
        /// <param name="pListaDocumentos">Lista de documentos</param>
        /// <returns></returns>
        public Dictionary<Guid, bool> ComprobarSiOntologiaTieneRecursos(List<Guid> pListaDocumentos)
        {
            return DocumentacionAD.ComprobarSiOntologiaTieneRecursos(pListaDocumentos);
        }

        /// <summary>
        /// Obtiene el identificador del documento al que pertenece el comentario.
        /// </summary>
        /// <param name="pComentarioID">Identificador del comentario</param>
        /// <returns>Identificador del documento al que pertenece el comentario</returns>
        public Guid ObtenerIDDocumentoDeComentarioPorID(Guid pComentarioID)
        {
            return DocumentacionAD.ObtenerIDDocumentoDeComentarioPorID(pComentarioID);
        }

        /// <summary>
        /// Obtiene el identificador del documento al que pertenece un voto.
        /// </summary>
        /// <param name="pVotoID">Identificador del voto</param>
        /// <returns>Identificador del documento al que pertenece el voto</returns>
        public Guid ObtenerIDDocumentoDeVotoPorID(Guid pVotoID)
        {
            return DocumentacionAD.ObtenerIDDocumentoDeVotoPorID(pVotoID);

        }

        #endregion

        public DataWrapperDocumentacion ObtenerUltimosRecursosVisitados(int pNumHorasIntervalo)
        {
            return DocumentacionAD.ObtenerUltimosRecursosVisitados(pNumHorasIntervalo);
        }

        public DataWrapperDocumentacion ObtenerOntologiasDeDocumentos(List<Guid> pListaDocumentosID)
        {
            return DocumentacionAD.ObtenerOntologiasDeDocumentos(pListaDocumentosID);
        }

        /// <summary>
        /// Obtiene los lectores de una lista de documentos.
        /// </summary>
        /// <param name="pListaDocumentosID">Identificadores de los documentos</param>
        /// <returns>Tabla DocumentoRolIdentidad y DocumentoRolGrupoIdentidades cargada con los editores de los documentos</returns>
        public DataWrapperDocumentacion ObtenerLectoresYGruposLectoresDocumentos(List<Guid> pListaDocumentosID)
        {
            return DocumentacionAD.ObtenerLectoresYGruposLectoresDocumentos(pListaDocumentosID);
        }

        /// <summary>
        /// Obtiene los editores de una lista de documentos.
        /// </summary>
        /// <param name="pListaDocumentosID">Identificadores de los documentos</param>
        /// <returns>Tabla DocumentoRolIdentidad cargada con los editores de los documentos</returns>
        public DataWrapperDocumentacion ObtenerEditoresYGruposEditoresDocumentos(List<Guid> pListaDocumentosID)
        {
            return DocumentacionAD.ObtenerEditoresYGruposEditoresDocumentos(pListaDocumentosID);
        }

        /// <summary>
        /// Obtiene la URL canonica de un documento almacenada en la tabla DocumentoUrlCanonica
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns></returns>
        public string ObtenerDocumentoUrlCanonica(Guid pDocumentoID)
        {
            return DocumentacionAD.ObtenerDocumentoUrlCanonica(pDocumentoID);
        }

        public Dictionary<string, List<Guid>> ObtenerRecursosSubidosPorUsuario(Guid user_id)
        {
            return DocumentacionAD.ObtenerRecursosSubidosPorUsuario(user_id);
        }

        public string ObtenerPathEstilos(Guid id_proyecto)
        {
            return DocumentacionAD.ObtenerPathEstilos(id_proyecto);
        }

        /// <summary>
        /// Descomparte de una comunidad todos los recursos almacenados salvo los excluidos
        /// </summary>
        /// <param name="pListaDocumentosExcluidos">Documentos exluidos de la comprobación</param>
        /// <param name="pProyectoDestinoID">Identificador del proyecto de compartición de destino</param>
        /// <param name="pProyectoOrigenID">Identificador del proyecto de origen de los documentos</param>        
        public void DescompartirDocumentosCompartidos(List<Guid> pListaDocumentosExcluidos, Guid pProyectoOrigenID, Guid pProyectoDestinoID)
        {
            DocumentacionAD.DescompartirDocumentosCompartidos(pListaDocumentosExcluidos, pProyectoOrigenID, pProyectoDestinoID);
        }
        #endregion

        #region Privados

        /// <summary>
        /// Valida los documentos
        /// </summary>
        /// <param name="pDocumentos">Lista de documentos para validar</param>
        private void ValidarDocumentos(List<Documento> pDocumentos)
        {
            if (pDocumentos != null)
            {
                for (int i = 0; i < pDocumentos.Count; i++)
                {
                    //Nombre cadena vacía
                    if (pDocumentos[i].Titulo.Trim().Length == 0)
                    {
                        throw new ErrorDatoNoValido("El título del documento no puede ser una cadena vacía");
                    }

                    //Nombre superior a 255 caracteres
                    if (pDocumentos[i].Titulo.Length > 1000)
                    {
                        throw new ErrorDatoNoValido("El título del documento '" + pDocumentos[i].Titulo + "' no puede contener más de 255 caracteres");
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~DocumentacionCN()
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
        protected void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                //Libero todos los recursos administrados que he añadido a esta clase
                if (DocumentacionAD != null)
                {
                    DocumentacionAD.Dispose();
                }
                DocumentacionAD = null;
            }
        }

        #endregion

        #region Propiedades

        private DocumentacionAD DocumentacionAD
        {
            get
            {
                return (DocumentacionAD)AD;
            }
            set
            {
                this.AD = value;
            }
        }

        private bool DesdeCL { get; set; }

        #endregion
    }
}
