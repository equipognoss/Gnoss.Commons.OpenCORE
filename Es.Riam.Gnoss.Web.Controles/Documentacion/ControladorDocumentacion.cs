using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.BASE_BD.Model;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.Live;
using Es.Riam.Gnoss.AD.Live.Model;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.RDF.Model;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Documentacion;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.CL.Tesauro;
using Es.Riam.Gnoss.Elementos;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Documentacion.AddToGnoss;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ParametroAplicacion;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.BASE_BD;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Live;
using Es.Riam.Gnoss.Logica.Notificacion;
using Es.Riam.Gnoss.Logica.ParametrosProyecto;
using Es.Riam.Gnoss.Logica.RDF;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Tesauro;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.RabbitMQ;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Servicios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles.GeneradorPlantillasOWL;
using Es.Riam.Gnoss.Web.Controles.Organizador.Correo;
using Es.Riam.Gnoss.Web.Controles.ServicioImagenesWrapper;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Semantica.OWL;
using Es.Riam.Semantica.Plantillas;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SemWeb;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Xml;

namespace Es.Riam.Gnoss.Web.Controles.Documentacion
{
    #region Enumeraciones

    /// <summary>
    /// Acciones sociales de google analytic.
    /// </summary>
    public enum AccionGoogleAnalytic
    {
        VotarPos = 0,
        Comentar = 1,
        SeguirA = 2,
        GuardarEnFav = 3,
        Compartir = 4,
        AniadirRecCat = 5,
        AniadirRecTag = 6,
        EnviarEnlace = 7,
        VincularRec = 8,
        VotarNeg = 9,
        DejarSeguirA = 10,
        DesCompartir = 11,
        DesVincular = 12
    }

    #endregion

    /// <summary>
    /// Controlador de documentación
    /// </summary>
    public class ControladorDocumentacion : ControladorBase
    {
        #region Constantes

        private const string COLA_TAGS_COMENTARIO = "ColaTagsComentarios";
        private static string COLA_NEWSLETTER = "ColaNewsletter";
        private static string EXCHANGE = "";

        #endregion

        #region Miembros

        /// <summary>
        /// Gestor de documentacion
        /// </summary>
        private GestorDocumental mGestorDocumental;

        /// <summary>
        /// Identificador de la organización para la actualización de número de recursos asincronamente.
        /// </summary>
        private Guid mOrganizacioActuAsincID;

        /// <summary>
        /// Identificador del proyecto para la actualización de número de recursos asincronamente.
        /// </summary>
        private Guid mProyectoActuAsincID;

        /// <summary>
        /// Identificador del documento para la actualización de número de recursos asincronamente.
        /// </summary>
        private List<Guid> mDocumentosAsincIDs;

        /// <summary>
        /// Identificador del documento para la actualización de número de recursos asincronamente.
        /// </summary>
        private bool mEliminadoAsinc;

        /// <summary>
        /// Identificador del documento para la actualización de número de recursos asincronamente.
        /// </summary>
        private PrioridadBase mPrioridadBaseAsinc = PrioridadBase.Alta;

        /// <summary>
        /// Identificador del documento para la actualización de número de recursos asincronamente.
        /// </summary>
        private List<string> mListaTagsViejosAsinc;

        /// <summary>
        /// Lista de categorías del tesauro anteriores a la modificación del documento
        /// </summary>
        private List<CategoriaTesauro> mListaCategoriasViejas;

        /// <summary>
        /// Autor del documento antes de la modificación
        /// </summary>
        private string mAutorViejoAsinc;

        /// <summary>
        /// Publicador del documento antes de la modificación
        /// </summary>
        private string mPublicadorViejoAsinc;

        /// <summary>
        /// Fecha del documento antes de la modificación
        /// </summary>
        private DateTime mFechaAnteriorViejaAsinc;

        /// <summary>
        /// Extensión del documento antes de la modificación
        /// </summary>
        private string mExetnsionViejaAsinc;

        /// <summary>
        /// Verdad si el recurso era privado para editores antes de la modificación
        /// </summary>
        private bool mPrivadoEditoresViejoAsinc;

        /// <summary>
        /// Nombre del documento antes de la modificación
        /// </summary>
        private string mNombreDocViejoAsinc;

        /// <summary>
        /// Enlace del documento antes de la modificación
        /// </summary>
        private string mEnlaceViejoAsinc;

        /// <summary>
        /// Enlace del documento antes de la modificación
        /// </summary>
        private string mNivelCertificacionViejoAsinc;

        /// <summary>
        /// Verdad si se deben actualizar los tags de todos los proyectos en los que está compartido el recurso
        /// </summary>
        public bool mActualizarTodosProyectosCompartido;


        /// <summary>
        /// Fichero de configuracion de la base de datos  del modelo base(especificar solo en caso de que no sea la de la web)
        /// </summary>
        private string mFicheroConfiguracionBDBase = "";

        /// <summary>
        /// Nos indica si actualmente hay conexion a RabbitMQ
        /// </summary>
        private static bool? mHayConexionRabbit = null;

        private EntityContextBASE mEntityContextBASE;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor a partir de la página que contiene al controlador
        /// </summary>
        /// <param name="pPage">Página</param>
        public ControladorDocumentacion(LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, EntityContextBASE entityContextBASE, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication)
        {
            mEntityContextBASE = entityContextBASE;
        }

        /// <summary>
        /// Constructor del controlador
        /// </summary>
        /// <param name="pGestionDocumentacion">Gestor documental</param>
        /// <param name="pPage">Página</param>
        public ControladorDocumentacion(GestorDocumental pGestionDocumentacion, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, EntityContextBASE entityContextBASE, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : this(loggingService, entityContext, configService, redisCacheWrapper, gnossCache, entityContextBASE, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication)
        {
            mGestorDocumental = pGestionDocumentacion;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        /// <summary>
        /// Constructor a partir de la página que contiene al controlador
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero config</param>
        public ControladorDocumentacion(string pFicheroConfiguracionBD, string pFicheroConfiguracionBDBase, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, EntityContextBASE entityContextBASE, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : this(loggingService, entityContext, configService, redisCacheWrapper, gnossCache, entityContextBASE, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication)
        {
            mFicheroConfiguracionBDBase = pFicheroConfiguracionBDBase;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el gestor de documentacion
        /// </summary>
        public GestorDocumental GestorDocumental
        {
            get
            {
                return mGestorDocumental;
            }
            set
            {
                mGestorDocumental = value;
            }
        }

        #endregion

        #region Métodos generales

        #region Públicos

        #region Enlace documento

        /// <summary>
        /// Construye el enlace de descarga de un documento pasado como parámetro
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pIdentidadOrganizacion">Identidad de organización</param>
        /// <returns>Enlace para descargar el documento</returns>
        public string CrearEnlaceDocumento(Documento pDocumento, Identidad pIdentidadOrganizacion, GnossIdentity pUsuario)
        {
            string enlace = "";
            string tipo = "";

            string baseUrl = UtilDominios.ObtenerDominioUrl(ProyectoSeleccionado.UrlPropia(UtilIdiomas.LanguageCode), true);
            string urlComunidad = UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto);

            switch (pDocumento.FilaDocumento.TipoEntidad)
            {
                case (short)TipoEntidadVinculadaDocumento.Web:
                    tipo = TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS;
                    break;
                case (short)TipoEntidadVinculadaDocumento.Curriculum:
                    tipo = TipoEntidadVinculadaDocumentoTexto.CV_Acreditacion;
                    break;
            }
            string extension = Path.GetExtension(pDocumento.NombreDocumento).ToLower();

            Guid PersonaPublicadorID = Guid.Empty;
            Guid OrganizacionPublicadorID = Guid.Empty;

            if (pUsuario.ProyectoID == ProyectoAD.MyGnoss)
            {
                if (pIdentidadOrganizacion == null)
                {
                    PersonaPublicadorID = pUsuario.PersonaID;
                }
                else
                {
                    OrganizacionPublicadorID = (Guid)UtilReflection.GetValueReflection(pIdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID");
                }
            }
            else
            {
                if (pDocumento.ProyectoID != ProyectoAD.MyGnoss)
                {
                    PersonaPublicadorID = Guid.Empty;
                }
                else
                {
                    AD.EntityModel.Models.PersonaDS.Persona filaPersona = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication).ObtenerPersonaPorIdentidadCargaLigera(pDocumento.CreadorID);

                    if (filaPersona != null)
                    {
                        PersonaPublicadorID = filaPersona.PersonaID;
                    }
                }
            }
            if (PersonaPublicadorID != Guid.Empty)
            {

                enlace = $"{urlComunidad}/download-file?tipo={tipo}&doc={pDocumento.Clave}&nombre={HttpUtility.UrlEncode(UtilCadenas.RemoveAccentsWithRegEx(pDocumento.NombreDocumento))}&ext={extension}&personaID={PersonaPublicadorID}";
            }
            else if (OrganizacionPublicadorID != Guid.Empty)
            {
                enlace = $"{urlComunidad}/download-file?tipo={tipo}&org={pDocumento.FilaDocumento.OrganizacionID}&doc={pDocumento.Clave}&nombre={HttpUtility.UrlEncode(UtilCadenas.RemoveAccentsWithRegEx(pDocumento.NombreDocumento))}&ext={extension}";
            }
            else
            {
                string proyectoID = "";

                if (pDocumento.FilaDocumento.ProyectoID.HasValue)
                {
                    proyectoID = $"&proy={pDocumento.FilaDocumento.ProyectoID.Value}";
                }
                enlace = $"{urlComunidad}/download-file?tipo={tipo}&org={pDocumento.FilaDocumento.OrganizacionID}{proyectoID}&doc={pDocumento.Clave}&nombre={HttpUtility.UrlEncode(UtilCadenas.RemoveAccentsWithRegEx(pDocumento.NombreDocumento))}&ext={extension}&proyectoID={ProyectoSeleccionado.Clave}";
            }
            return enlace;
        }

        #endregion

        #region Restaurar recursos

        /// <summary>
        /// Restaura un documento eliminado y redirije a su ficha para agregarlo a las categorias de tesauro
        /// </summary>
        /// <param name="pDocumento">Documento a restaurar</param>
        /// <param name="pIdentidadOrganizacion">Identidad de organización</param>
        /// <param name="BaseURLIdioma">Variable BaseURLIdioma de la página</param>
        /// <param name="NombreProy">Nombre CORTO de proyecto</param>
        /// <param name="UrlPerfil">Variable UrlPerfil de la página</param>
        /// <param name="UtilIdiomas">Variable UtilIdiomas de la página</param>
        public void RestaurarRecursoBorrado(DocumentoWeb pDocumento, Identidad pIdentidadOrganizacion, string pBaseURLIdioma, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil)
        {
            if (pDocumento.FilaDocumento.Eliminado)
            {
                pDocumento.FilaDocumento.Eliminado = false;
                mHttpContextAccessor.HttpContext.Response.Redirect(UrlsSemanticas.GetURLBaseRecursosEditarDocumento(pBaseURLIdioma, pUtilIdiomas, pNombreProy, pUrlPerfil, pDocumento, 0, (pIdentidadOrganizacion != null)));
            }
        }

        #endregion

        #region Certificación

        /// <summary>
        /// Carga los niveles de certificación de un proyecto pasado como parámetro
        /// </summary>
        /// <param name="pProyecto">ID del proyecto</param>
        public void CargarNivelesCertificacionRecursosProyecto(Proyecto pProyecto)
        {
            if (!pProyecto.NivelesCertificacionCargados)
            {
                ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                pProyecto.GestorProyectos.DataWrapperProyectos.ListaNivelCertificacion = pProyecto.GestorProyectos.DataWrapperProyectos.ListaNivelCertificacion.Union(proyectoCL.ObtenerNivelesCertificacionRecursosProyecto(pProyecto.Clave).ListaNivelCertificacion).ToList();
                pProyecto.NivelesCertificacionCargados = true;
            }
        }

        #endregion

        #region Concurrencia

        /// <summary>
        /// Comprueba si un documento ha sido editado por otra persona al mismo tiempo
        /// </summary>
        /// <param name="pDocumento">Documento que se quiere actualizar</param>
        /// <param name="pIdentidadActual">Identidad actual</param>
        /// <returns></returns>
        public ErroresConcurrencia ComprobarConcurrenciaDocumento(Documento pDocumento, Identidad pIdentidadActual, out Guid? pNuevaVersionDocID)
        {
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            ErroresConcurrencia errorConcurrencia = docCN.ComprobarConcurrenciaDocumento(pDocumento.Clave, pDocumento.FilaDocumento.FechaModificacion.Value, out pNuevaVersionDocID);

            return errorConcurrencia;
        }

        #endregion

        #region Versiones documento

        /// <summary>
        /// Carga toda la información de un documento.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pDatosCompletos">Especifica si debe cargarse todo 
        /// del documento o solo los complementos(historial, version, etc.)</param>
        public void CargarNecesarioParaDuplicacion(Guid pDocumentoID, bool pDatosCompletos)
        {
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            if (pDatosCompletos)
            {
                docCN.ObtenerDocumentoPorIDCargarTotal(pDocumentoID, GestorDocumental.DataWrapperDocumentacion, true, false, Guid.Empty);
            }
            else
            {
                docCN.ObtenerDocumentoPorIDCargarTotal(pDocumentoID, GestorDocumental.DataWrapperDocumentacion, false, false, Guid.Empty);

                //Esto solo se carga cuando no quieres los datos completos, así no se trae 2 veces:
                CargarTodoVersionesDocumento(pDocumentoID);
            }

            GestorDocumental.DataWrapperDocumentacion.Merge(docCN.ObtenerVinculacionesRecurso(pDocumentoID));
        }

        /// <summary>
        /// Duplica un archivo fisicamente en el servidor
        /// </summary>
        /// <param name="pOrigen">Documento de origen</param>
        /// <param name="pDestino">Documento de destino</param>
        /// <param name="pMasterComunidad">TRUE si se encuentra en la base de recursos de una comunidad</param>
        /// <param name="pParametrosAplicacionDS">Dataset de parametros de aplicación usado desde la pantalla</param>
        /// <returns>TRUE si la copia se ha hecho bien</returns>
        // public bool DuplicarDocumentoFisicamente(Elementos.Documentacion.Documento pOrigen, Elementos.Documentacion.Documento pDestino, Elementos.Identidad.Identidad pIdentidadOrganizacion, bool pMasterComunidad, ParametroAplicacionDS pParametrosAplicacionDS)
        public bool DuplicarDocumentoFisicamente(Documento pOrigen, Documento pDestino, Identidad pIdentidadOrganizacion, bool pMasterComunidad, GnossIdentity pUsuario)
        {
            GestionDocumental servicioArchivos = null;
            CallInterntService servicioVideos = null;
            ServicioImagenes servicioImagenes = null;
            Stopwatch sw = null;
            bool correcto = false;

            try
            {
                if (pOrigen.TipoDocumentacion == TiposDocumentacion.FicheroServidor)
                {
                    string tipoEntidadSubirOrigen = ObtenerTipoEntidadAdjuntarDocumento(pOrigen.TipoEntidadVinculada);
                    string tipoEntidadSubirDestino = ObtenerTipoEntidadAdjuntarDocumento(pDestino.TipoEntidadVinculada);

                    servicioArchivos = new GestionDocumental(mLoggingService, mConfigService);
                    servicioArchivos.Url = mConfigService.ObtenerUrlServicioDocumental();

                    sw = LoggingService.IniciarRelojTelemetria();

                    if (!pMasterComunidad && pOrigen.TipoEntidadVinculada == TipoEntidadVinculadaDocumento.Web)
                    {
                        if (pIdentidadOrganizacion == null)
                        {
                            correcto = servicioArchivos.CopiarCortarDocumento(true, tipoEntidadSubirOrigen, Guid.Empty, Guid.Empty, pUsuario.PersonaID, pOrigen.Clave, tipoEntidadSubirDestino, Guid.Empty, Guid.Empty, pUsuario.PersonaID, pDestino.Clave, pDestino.Extension);
                        }
                        else
                        {
                            correcto = servicioArchivos.CopiarCortarDocumento(true, tipoEntidadSubirOrigen, (Guid)UtilReflection.GetValueReflection(pIdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID"), Guid.Empty, Guid.Empty, pOrigen.Clave, tipoEntidadSubirDestino, (Guid)UtilReflection.GetValueReflection(pIdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID"), Guid.Empty, Guid.Empty, pDestino.Clave, pDestino.Extension);
                        }
                    }
                    else
                    {
                        correcto = servicioArchivos.CopiarCortarDocumento(true, tipoEntidadSubirOrigen, pOrigen.OrganizacionID, pOrigen.ProyectoID, Guid.Empty, pOrigen.Clave, tipoEntidadSubirDestino, pOrigen.OrganizacionID, pOrigen.ProyectoID, Guid.Empty, pDestino.Clave, pDestino.Extension);
                    }
                    mLoggingService.AgregarEntradaDependencia("Copiado archivo en el gestor documental", false, "DuplicarDocumentoFisicamente", sw, true);
                }
                else if (pOrigen.TipoDocumentacion == TiposDocumentacion.Imagen)
                {
                    sw = LoggingService.IniciarRelojTelemetria();
                    servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService);
                    string url = UrlIntragnossServicios;
                    servicioImagenes.Url = url;

                    if (!pMasterComunidad && pOrigen.TipoEntidadVinculada == TipoEntidadVinculadaDocumento.Web)
                    {
                        if (pIdentidadOrganizacion == null)
                        {
                            correcto = servicioImagenes.CopiarCortarImagenEspecificandoRuta(true, pUsuario.PersonaID, Guid.Empty, pOrigen.Clave, pUsuario.PersonaID, Guid.Empty, pDestino.Clave, pDestino.Extension, $"Documentos");
                        }
                        else
                        {
                            correcto = servicioImagenes.CopiarCortarImagenEspecificandoRuta(true, Guid.Empty, (Guid)UtilReflection.GetValueReflection(pIdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID"), pOrigen.Clave, Guid.Empty, (Guid)UtilReflection.GetValueReflection(pIdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID"), pDestino.Clave, ".jpg", $"Documentos");
                        }
                    }
                    else
                    {
                        correcto = servicioImagenes.CopiarCortarImagenEspecificandoRuta(true, Guid.Empty, Guid.Empty, pOrigen.Clave, Guid.Empty, Guid.Empty, pDestino.Clave, pDestino.Extension, $"Documentos");
                    }
                    mLoggingService.AgregarEntradaDependencia("Copiada imagen en el servicio de imagenes", false, "DuplicarDocumentoFisicamente", sw, true);
                }
                else if (pOrigen.TipoDocumentacion == TiposDocumentacion.Video)
                {
                    servicioVideos = new CallInterntService(mConfigService, mLoggingService);
                    sw = LoggingService.IniciarRelojTelemetria();

                    if (!pMasterComunidad && pOrigen.TipoEntidadVinculada == TipoEntidadVinculadaDocumento.Web)
                    {
                        if (pIdentidadOrganizacion == null)
                        {
                            correcto = servicioVideos.CopiarVideo(pOrigen.Clave, pDestino.Clave, pUsuario.PersonaID, Guid.Empty, pUsuario.PersonaID, Guid.Empty);
                        }
                        else
                        {
                            correcto = servicioVideos.CopiarVideo(pOrigen.Clave, pDestino.Clave, Guid.Empty, (Guid)UtilReflection.GetValueReflection(pIdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID"), Guid.Empty, (Guid)UtilReflection.GetValueReflection(pIdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID"));
                        }
                    }
                    else
                    {
                        correcto = servicioVideos.CopiarVideo(pOrigen.Clave, pDestino.Clave, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty);
                    }

                    mLoggingService.AgregarEntradaDependencia("Copiada video en el servicio de videos", false, "DuplicarDocumentoFisicamente", sw, true);
                }
                else if (pOrigen.TipoDocumentacion == TiposDocumentacion.Semantico)
                {
                    string urlServicioDocs = null;

                    urlServicioDocs = mConfigService.ObtenerUrlServicioDocumental();

                    CrearNuevaVersionDocumentoRDF(pOrigen, pDestino, UrlIntragnossServicios, urlServicioDocs);
                }
            }
            catch
            {
                mLoggingService.AgregarEntradaDependencia("No Copiadarchivo", false, "DuplicarDocumentoFisicamente", sw, false);
                correcto = false;
            }
            finally
            {

            }
            return correcto;
        }

        /// <summary>
        /// Crea una nueva versión del documento rdf.
        /// </summary>
        /// <param name="pDocumentoOriginal">Documento semántico original</param>
        /// <param name="pDocumentoNuevo">Documento semántico nuevo</param>
        /// <param name="pUrlIntragnossServicios">Url intragnoss para los servicios web</param>
        /// <param name="pUrlServicioDocs">Url para el servicio de documentación</param>
        public bool CrearNuevaVersionDocumentoRDF(Documento pDocumentoOriginal, Documento pDocumentoNuevo, string pUrlIntragnossServicios, string pUrlServicioDocs)
        {
            if (!pDocumentoOriginal.GestorDocumental.ListaDocumentos.ContainsKey(pDocumentoOriginal.ElementoVinculadoID))
            {
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                pDocumentoOriginal.GestorDocumental.DataWrapperDocumentacion.Merge(docCN.ObtenerDocumentoPorID(pDocumentoOriginal.ElementoVinculadoID));

                pDocumentoOriginal.GestorDocumental.CargarDocumentos(false);
            }

            string nombreGrafo = pDocumentoOriginal.GestorDocumental.ListaDocumentos[pDocumentoOriginal.ElementoVinculadoID].Enlace;

            FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, pDocumentoOriginal.ProyectoID.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            FacetadoDS facetadoDS = facetadoCN.ObtenerRDFXMLdeFormulario(nombreGrafo, pDocumentoOriginal.Clave.ToString());

            string tripletas = "";
            MemoryStore store = new MemoryStore();
            bool posibleImagen = false;
            bool posibleArchivoLink = false;

            foreach (DataRow fila in facetadoDS.Tables[0].Rows)
            {
                string sujeto = "<" + ((string)fila[0]).Replace(pDocumentoOriginal.Clave + "_", pDocumentoNuevo.Clave + "_") + ">";
                string predicado = "<" + (string)fila[1] + ">";
                string objeto = "";
                if (!fila.IsNull(2))
                {
                    objeto = (string)fila[2];
                }

                if (objeto.IndexOf("http://") == 0 && objeto.Contains(pDocumentoOriginal.Clave + "_"))
                {
                    objeto = "<" + objeto.Replace(pDocumentoOriginal.Clave + "_", pDocumentoNuevo.Clave + "_") + ">";
                }
                else
                {
                    if (objeto.ToLower().Contains(UtilArchivos.DirectorioDocumento(pDocumentoOriginal.Clave).ToLower()))
                    {
                        if (objeto.ToLower().Contains(".jpg"))
                        {
                            posibleImagen = true;
                        }

                        posibleArchivoLink = true;

                        objeto = objeto.Replace(UtilArchivos.DirectorioDocumento(pDocumentoOriginal.Clave), UtilArchivos.DirectorioDocumento(pDocumentoNuevo.Clave));
                    }
                    else if (/*objeto.ToLower().Contains(".flv") ||*/ objeto.ToLower().Contains(".jpg"))
                    {
                        posibleImagen = true;
                        objeto = objeto.Replace(pDocumentoOriginal.Clave.ToString(), pDocumentoNuevo.Clave.ToString());
                    }

                    objeto = "\"" + objeto + "\"";
                }

                tripletas += new FacetadoAD("", mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).GenerarTripletaSinConversionesAbsurdas(sujeto, predicado, objeto);
            }

            #region Copiado Archivos y Archivos Link

            bool archivosOK = true;

            if (posibleImagen)
            {
                archivosOK = (archivosOK && CopiarImagenDocumentoSemantico(pDocumentoOriginal.Clave, pDocumentoNuevo.Clave, pUrlIntragnossServicios));
            }

            if (posibleArchivoLink)
            {
                archivosOK = (archivosOK && CopiarArchivosLinkDocumentoSemantico(pDocumentoOriginal.Clave, pDocumentoNuevo.Clave, pUrlIntragnossServicios));
            }

            archivosOK = (archivosOK && CopiarArchivosDocumentoSemantico(pDocumentoOriginal.Clave, pDocumentoNuevo.Clave, pUrlServicioDocs));

            if (!archivosOK)
            {
                throw new Exception("No se han podido duplicar los archivos para el nuevo documento");
            }

            //Actualizo la foto desnormalizada para los listados de recursos:
            if (!string.IsNullOrEmpty(pDocumentoNuevo.FilaDocumento.NombreCategoriaDoc))
            {
                if (pDocumentoNuevo.FilaDocumento.NombreCategoriaDoc.Contains(UtilArchivos.DirectorioDocumento(pDocumentoOriginal.Clave)))
                {
                    pDocumentoNuevo.FilaDocumento.NombreCategoriaDoc = pDocumentoNuevo.FilaDocumento.NombreCategoriaDoc.Replace(UtilArchivos.DirectorioDocumento(pDocumentoOriginal.Clave), UtilArchivos.DirectorioDocumento(pDocumentoNuevo.Clave));
                }
                else if (pDocumentoNuevo.FilaDocumento.NombreCategoriaDoc.Contains(pDocumentoOriginal.Clave.ToString().ToLower()))
                {
                    pDocumentoNuevo.FilaDocumento.NombreCategoriaDoc = pDocumentoNuevo.FilaDocumento.NombreCategoriaDoc.Replace(pDocumentoOriginal.Clave.ToString().ToLower(), pDocumentoNuevo.Clave.ToString().ToLower());
                }
            }

            #endregion

            #region Genero el RDF para el almacenamiento en SQL Server

            //foreach (DataRow fila in facetadoDS.Tables[0].Rows)
            foreach (DataRow fila in facetadoDS.Tables[0].Select("", "s"))
            {
                Literal objeto = new Literal("");

                if (!fila.IsNull(2))
                {
                    if (((string)fila[2]).ToLower().Contains(UtilArchivos.DirectorioDocumento(pDocumentoOriginal.Clave)))
                    {
                        objeto = new Literal(((string)fila[2]).Replace(UtilArchivos.DirectorioDocumento(pDocumentoOriginal.Clave), UtilArchivos.DirectorioDocumento(pDocumentoNuevo.Clave)));
                    }
                    else
                    {
                        objeto = new Literal(((string)fila[2]).Replace(pDocumentoOriginal.Clave.ToString(), pDocumentoNuevo.Clave.ToString()));
                    }
                }

                store.Add(new Statement(new Entity(((string)fila[0]).Replace(pDocumentoOriginal.Clave + "_", pDocumentoNuevo.Clave + "_")), new Entity((string)fila[1]), objeto));
            }

            facetadoDS.Dispose();

            #region Obtengo Ontología

            Guid ontologiaID = pDocumentoOriginal.ElementoVinculadoID;

            if (!pDocumentoOriginal.GestorDocumental.ListaDocumentos.ContainsKey(ontologiaID))
            {
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                pDocumentoOriginal.GestorDocumental.DataWrapperDocumentacion.Merge(docCN.ObtenerDocumentoPorID(ontologiaID));

                pDocumentoOriginal.GestorDocumental.CargarDocumentos(false);
            }

            //Agrego namespaces y urls:
            string nombreOntologia = pDocumentoOriginal.GestorDocumental.ListaDocumentos[ontologiaID].FilaDocumento.Enlace;
            string urlOntologia = BaseURLFormulariosSem + "/Ontologia/" + nombreOntologia + "#";

            //Obtengo la ontología:
            byte[] arrayOntologia = ObtenerOntologia(ontologiaID, pDocumentoOriginal.FilaDocumento.ProyectoID.Value);

            //Leo la ontología:
            Ontologia ontologia = new Ontologia(arrayOntologia, true);
            ontologia.LeerOntologia();
            ontologia.IdiomaUsuario = IdiomaUsuario;

            #endregion

            MemoryStream archivo = new MemoryStream();
            System.Xml.XmlWriter textWriter = System.Xml.XmlWriter.Create(archivo);

            RdfWriter writer = new RdfXmlWriter(textWriter);
            writer.Namespaces.AddNamespace("http://www.w3.org/1999/02/22-rdf-syntax-ns#", "rdf");
            writer.Namespaces.AddNamespace("http://www.gnoss.net/ontologia.owl#", "gnoss");
            writer.Namespaces.AddNamespace("http://www.w3.org/2001/XMLSchema#", "xsd");
            writer.Namespaces.AddNamespace("http://www.w3.org/2000/01/rdf-schema#", "rdfs");
            writer.Namespaces.AddNamespace("http://www.w3.org/2002/07/owl#", "owl");
            writer.Namespaces.AddNamespace(urlOntologia, GestionOWL.NAMESPACE_ONTO_GNOSS);

            if (ontologia != null)
            {
                foreach (string ns in ontologia.NamespacesDefinidos.Keys)
                {
                    if (ontologia.NamespacesDefinidos[ns] != "rdf" && ontologia.NamespacesDefinidos[ns] != "gnoss" && ontologia.NamespacesDefinidos[ns] != "xsd" && ontologia.NamespacesDefinidos[ns] != "rdfs" && ontologia.NamespacesDefinidos[ns] != "owl" && ontologia.NamespacesDefinidos[ns] != GestionOWL.NAMESPACE_ONTO_GNOSS)
                    {
                        writer.Namespaces.AddNamespace(ns, ontologia.NamespacesDefinidos[ns]);
                    }
                }
            }

            writer.Write(store);
            writer.Close();
            store.Dispose();

            MemoryStream buffer = new MemoryStream(archivo.ToArray());
            StreamReader reader = new StreamReader(buffer);
            string rdfTexto = reader.ReadToEnd();
            reader.Close();
            reader.Dispose();

            try
            {
                GuardarRDFEnBDRDF(rdfTexto, pDocumentoNuevo.Clave, pDocumentoNuevo.FilaDocumento.ProyectoID.Value);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogErrorAJAX("Error al guardar el RDF en SqlServer: " + ex.ToString());
                throw;
            }

            #endregion

            //Introduzco por cada entidad, una vinculación con su elemento padre
            string[] separador = { " \n " };
            string[] filas = tripletas.Split(separador, StringSplitOptions.RemoveEmptyEntries);
            List<string> listaSujetos = new List<string>();
            string sujetoDocumento = "<" + UrlIntragnoss + pDocumentoNuevo.Clave + "> ";
            string saltoDeLinea = "";

            foreach (string fila in filas)
            {
                if (!string.IsNullOrEmpty(fila) && fila.Contains(" "))
                {
                    string sujeto = fila.Substring(0, fila.IndexOf(' '));
                    if (!listaSujetos.Contains(sujeto))
                    {
                        listaSujetos.Add(sujeto);
                        tripletas += saltoDeLinea + sujetoDocumento + "<http://gnoss/hasEntidad> " + sujeto + " .";
                        saltoDeLinea = " \n ";
                    }
                }
            }

            facetadoCN.InsertaTripletas(nombreGrafo, tripletas, 0, true);

            return true;
        }

        /// <summary>
        /// Comprueba si la versión del documento pasado como parámetro es la última creada
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <returns>TRUE si la versión del documento pasado como parámetro es la última, FALSE si hay una versión posterior</returns>
        public bool EsUltimaVersion(Documento pDocumento)
        {
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            bool esUltimaVersion = pDocumento.FilaDocumento.UltimaVersion && docCN.ComprobarSiEsUltimaVersionDocumento(pDocumento.Clave);

            return esUltimaVersion;
        }

        #endregion

        #region Nombre proyecto

        /// <summary>
        /// Carga todos los nombres de los proyectos en los que está compartido el documento.
        /// </summary>
        /// <param name="pGestorDocumental">Gestor documental</param>
        public void CargarNombresProyectos(GestorDocumental pGestorDocumental)
        {
            List<Guid> ListaOrganizacionesIDs = new List<Guid>();
            List<Documento> listDocAux = new List<Documento>();
            listDocAux.AddRange(pGestorDocumental.ListaDocumentos.Values);
            foreach (Documento doc in listDocAux)
            {
                DocumentoWeb docWeb = new DocumentoWeb(doc.FilaDocumento, pGestorDocumental, mLoggingService);
                foreach (Guid baseRecursos in docWeb.BaseRecursos)
                {
                    if (pGestorDocumental.ObtenerTipoBaseRecursos(baseRecursos) == TipoBaseRecursos.BROrganizacion)
                    {
                        Guid organizacionID = pGestorDocumental.ObtenerOrganizacionID(baseRecursos);
                        if (!ListaOrganizacionesIDs.Contains(organizacionID))
                        {
                            ListaOrganizacionesIDs.Add(organizacionID);
                        }
                    }
                }
            }
        }

        #endregion

        #region Envio correo NewsLetter

        /// <summary>
        /// Inserta una fila en DocumentoEnvioNewsLetter para que el servicio de envíos masivos gestione el envío de la Newsletter
        /// </summary>
        /// <param name="pDocumento">Documento newsLetter</param>
        /// <param name="pIdioma">Idioma en el que se envia la newsLetter</param>
        /// <param name="pListaGrupos">Lista de los grupos a los que se le enviará la Newsletter. Si es nulo se enviará a todos los miembros de la comunidad</param>
        public void EnviarCorreoNewsLetter(Documento pDocumento, string pIdioma, List<Guid> pListaGrupos, Guid pIdentidadID)
        {
            AD.EntityModel.Models.Documentacion.DocumentoEnvioNewsLetter documentoEnvioNewsletter = pDocumento.GestorDocumental.AgregarEnvioNewsLetter(pDocumento, pIdioma, pListaGrupos, pIdentidadID);
            bool agregadoARabbit = false;
            try
            {
                if (HayConexionRabbit)
                {
                    InsertarDocumentoIDColaNewsletterRabbitMQ(documentoEnvioNewsletter);
                    mEntityContext.Entry(documentoEnvioNewsletter).State = EntityState.Detached;
                    agregadoARabbit = true;
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
            }

            if (!agregadoARabbit)
            {
                mEntityContext.SaveChanges();
            }
        }

        public bool HayConexionRabbit
        {
            get
            {
                return mConfigService.ExistRabbitConnection(RabbitMQClient.BD_SERVICIOS_WIN);
            }
        }

        private void InsertarDocumentoIDColaNewsletterRabbitMQ(AD.EntityModel.Models.Documentacion.DocumentoEnvioNewsLetter pDocumentoEnvioNewsletter)
        {
            using (RabbitMQClient rabbitMQ = new RabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN, COLA_NEWSLETTER, mLoggingService, mConfigService, EXCHANGE, COLA_NEWSLETTER))
            {
                rabbitMQ.AgregarElementoACola(JsonConvert.SerializeObject(pDocumentoEnvioNewsletter));
            }
        }

        #endregion

        #region Privacidad documento

        /// <summary>
        /// Esblece si un recurso es público en el meta-Buscador o no.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pIdentidadActual">Identidad actual en la Web</param>
        /// <param name="pGuardar">Indica si se debe guardar o no en BD los cambios</param>
        public void EstablecePrivacidadRecursoEnMetaBuscador(Documento pDocumento, Identidad pIdentidadActual, bool pGuardar)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            bool publico = ComprobarPrivacidadRecursoEnMetaBuscador(pDocumento, dataWrapperProyecto, pIdentidadActual);

            if (!publico)
            {
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                List<Guid> listaDocumentosID = new List<Guid>();
                listaDocumentosID.Add(pDocumento.Clave);
                GestorDocumental gestorDocAux = new GestorDocumental(docCN.ObtenerDocumentosPorID(listaDocumentosID, true), mLoggingService, mEntityContext);
                gestorDocAux.CargarDocumentos(false);
                Documento docAux = gestorDocAux.ListaDocumentos[pDocumento.Clave];

                List<Guid> listaProyectos = new List<Guid>();

                foreach (AD.EntityModel.Models.Documentacion.BaseRecursosProyecto filaBRProy in gestorDocAux.DataWrapperDocumentacion.ListaBaseRecursosProyecto)
                {//"DocumentoID='" + pDocumento.Clave + "' AND BaseRecursosID='" + filaBRProy.BaseRecursosID + "'"
                    List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filaDocVinBR = gestorDocAux.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(pDocumento.Clave) && doc.BaseRecursosID.Equals(filaBRProy.BaseRecursosID)).ToList();
                    if (filaDocVinBR.Count > 0 && filaDocVinBR[0].Eliminado == false)
                    {
                        listaProyectos.Add(filaBRProy.ProyectoID);
                    }
                }
                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                dataWrapperProyecto = proyCN.ObtenerProyectosPorID(listaProyectos);

                if (pIdentidadActual.Persona != null)
                {
                    gestorDocAux.GestorTesauro = new GestionTesauro(new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication).ObtenerTesauroUsuario(pIdentidadActual.Persona.UsuarioID), mLoggingService, mEntityContext);
                }
                publico = ComprobarPrivacidadRecursoEnMetaBuscador(docAux, dataWrapperProyecto, pIdentidadActual);

                docAux.Dispose();
                gestorDocAux.Dispose();

            }

            if (pDocumento.FilaDocumento.Publico != publico)
            {
                pDocumento.FilaDocumento.Publico = publico;
            }

            if (pGuardar)
            {
                DataWrapperTesauro tesGuardarDW = null;

                if (pDocumento.GestorDocumental.GestorTesauro != null)
                {
                    tesGuardarDW = pDocumento.GestorDocumental.GestorTesauro.TesauroDW;
                }

                mEntityContext.SaveChanges();
            }
        }

        /// <summary>
        /// Esblece si un recurso es público en el meta-Buscador o no.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pIdentidadActual">Identidad actual en la Web</param>
        /// <param name="pGuardar">Indica si se debe guardar o no en BD los cambios</param>
        public void EstablecePrivacidadRecursoEnMetaBuscadorDesdeServicio(Documento pDocumento, Identidad pIdentidadActual, bool pGuardar, string pFicheroConfiguracionBD, Guid pUsuarioID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            bool publico = ComprobarPrivacidadRecursoEnMetaBuscador(pDocumento, dataWrapperProyecto, pIdentidadActual);

            if (!publico)
            {
                DocumentacionCN docCN = new DocumentacionCN(pFicheroConfiguracionBD, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                List<Guid> listaDocumentosID = new List<Guid>();
                listaDocumentosID.Add(pDocumento.Clave);
                GestorDocumental gestorDocAux = new GestorDocumental(docCN.ObtenerDocumentosPorID(listaDocumentosID, true), mLoggingService, mEntityContext);
                gestorDocAux.CargarDocumentos(false);
                Documento docAux = gestorDocAux.ListaDocumentos[pDocumento.Clave];

                List<Guid> listaProyectos = new List<Guid>();

                foreach (AD.EntityModel.Models.Documentacion.BaseRecursosProyecto filaBRProy in gestorDocAux.DataWrapperDocumentacion.ListaBaseRecursosProyecto)
                {//"DocumentoID='" + pDocumento.Clave + "' AND BaseRecursosID='" + filaBRProy.BaseRecursosID + "'"
                    List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filaDocVinBR = gestorDocAux.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(pDocumento.Clave) && doc.BaseRecursosID.Equals(filaBRProy.BaseRecursosID)).ToList();
                    if (filaDocVinBR.Count > 0 && filaDocVinBR[0].Eliminado == false)
                    {
                        listaProyectos.Add(filaBRProy.ProyectoID);
                    }
                }
                ProyectoCN proyCN = new ProyectoCN(pFicheroConfiguracionBD, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                dataWrapperProyecto = proyCN.ObtenerProyectosPorID(listaProyectos);

                if (pIdentidadActual.Persona != null)
                {
                    gestorDocAux.GestorTesauro = new GestionTesauro(new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication).ObtenerTesauroUsuario(pUsuarioID), mLoggingService, mEntityContext);
                }
                publico = ComprobarPrivacidadRecursoEnMetaBuscador(docAux, dataWrapperProyecto, pIdentidadActual);

                docAux.Dispose();
                gestorDocAux.Dispose();

            }

            if (pDocumento.FilaDocumento.Publico != publico)
            {
                pDocumento.FilaDocumento.Publico = publico;

                if (pGuardar)
                {
                    mEntityContext.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Comprueba si un recurso es público en el meta-Buscador o no.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pDataWrapperProyecto">DataSet de proyecto</param>
        /// <param name="pIdentidadActual">Identidad actual en la Web</param>
        private bool ComprobarPrivacidadRecursoEnMetaBuscador(Documento pDocumento, DataWrapperProyecto pDataWrapperProyecto, Identidad pIdentidadActual)
        {
            bool publico = false;

            bool publicoEnComunidad = false;
            bool publicoEnOrganizacion = false;
            bool publicoEnBRUsuario = false;

            //TODO everybody: Ahora si es PrivadoEditores el recurso es privado, en el futuro, cuando el flag PrivadoEditores sea por
            //   comunidad, hay que comprobar por cada una de ellas, por lo que habrá que meterlo dentro del foreach de comunidades de abajo.
            //if (pDocumento.FilaDocumentoWebVinBR.PrivadoEditores)
            //{
            //    return false;
            //}

            if (pDataWrapperProyecto != null)
            {
                List<Guid> listaProyectos = new List<Guid>();

                foreach (AD.EntityModel.Models.Documentacion.BaseRecursosProyecto filaBRProy in pDocumento.GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosProyecto)
                {
                    listaProyectos.Add(filaBRProy.ProyectoID);
                }

                foreach (AD.EntityModel.Models.ProyectoDS.Proyecto filaProy in pDataWrapperProyecto.ListaProyecto)
                {
                    if (listaProyectos.Contains(filaProy.ProyectoID) && (filaProy.TipoAcceso == (short)TipoAcceso.Publico || filaProy.TipoAcceso == (short)TipoAcceso.Restringido) && filaProy.Estado != (short)EstadoProyecto.Cerrado && filaProy.Estado != (short)EstadoProyecto.CerradoTemporalmente)
                    {
                        publico = true;
                        publicoEnComunidad = true;
                        break;
                    }
                }
            }

            if (!publico)
            {
                foreach (AD.EntityModel.Models.Documentacion.BaseRecursosUsuario filaBRUsu in pDocumento.GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosUsuario)
                {
                    List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filasDocVinBrUsu = pDocumento.GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.BaseRecursosID.Equals(filaBRUsu.BaseRecursosID) && doc.DocumentoID.Equals(pDocumento.Clave)).ToList();

                    if (filasDocVinBrUsu.Count > 0 && !filasDocVinBrUsu.First().Eliminado)
                    {
                        List<AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro> filasDocAgCat = pDocumento.GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(doc => doc.DocumentoID.Equals(pDocumento.Clave) && doc.BaseRecursosID.Equals(filaBRUsu.BaseRecursosID)).ToList();

                        if (pDocumento.GestorDocumental.GestorTesauro != null && pDocumento.GestorDocumental.GestorTesauro.TesauroDW.ListaTesauroUsuario.Count > 0)
                        {
                            Guid clavePublica = (pDocumento.GestorDocumental.GestorTesauro.TesauroDW.ListaTesauroUsuario.FirstOrDefault()).CategoriaTesauroPublicoID.Value;

                            foreach (AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro filaDocAgCat in filasDocAgCat)
                            {
                                Guid categoriaID = filaDocAgCat.CategoriaTesauroID;

                                if (pDocumento.GestorDocumental.GestorTesauro.ListaCategoriasTesauro.ContainsKey(categoriaID) && pDocumento.GestorDocumental.GestorTesauro.ListaCategoriasTesauro[categoriaID].PadreNivelRaiz.Clave == clavePublica)
                                {
                                    publico = true;
                                    publicoEnBRUsuario = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (publico)
                    {
                        break;
                    }
                }
            }

            if (!publico)
            {
                foreach (AD.EntityModel.Models.Documentacion.BaseRecursosOrganizacion filaBROrg in pDocumento.GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosOrganizacion)
                {//"BaseRecursosID='" + filaBROrg.BaseRecursosID + "' AND DocumentoID='" + pDocumento.Clave + "'"
                    List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filasDocVinBrOrg = pDocumento.GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.BaseRecursosID.Equals(filaBROrg.BaseRecursosID) && doc.DocumentoID.Equals(pDocumento.Clave)).ToList();

                    if (filasDocVinBrOrg.Count > 0 && !filasDocVinBrOrg[0].Eliminado)
                    {//"DocumentoID='" + pDocumento.Clave + "' AND BaseRecursosID='" + filaBROrg.BaseRecursosID + "'"
                        List<AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro> filasDocAgCat = pDocumento.GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(doc => doc.DocumentoID.Equals(pDocumento.Clave) && doc.BaseRecursosID.Equals(filaBROrg.BaseRecursosID)).ToList();

                        if (pDocumento.GestorDocumental.GestorTesauro != null && pDocumento.GestorDocumental.GestorTesauro.TesauroDW.ListaTesauroOrganizacion.Count > 0)
                        {
                            Guid clavePublica = pDocumento.GestorDocumental.GestorTesauro.TesauroDW.ListaTesauroOrganizacion.FirstOrDefault().CategoriaTesauroPublicoID.Value;

                            foreach (AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro filaDocAgCat in filasDocAgCat)
                            {
                                Guid categoriaID = filaDocAgCat.CategoriaTesauroID;

                                if (pDocumento.GestorDocumental.GestorTesauro.ListaCategoriasTesauro.ContainsKey(categoriaID) && pDocumento.GestorDocumental.GestorTesauro.ListaCategoriasTesauro[categoriaID].PadreNivelRaiz.Clave == clavePublica)
                                {
                                    publico = true;
                                    publicoEnOrganizacion = true;
                                    break;
                                }
                            }

                            if (publicoEnOrganizacion)
                            {
                                DataWrapperOrganizacion orgaDW = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication).ObtenerOrganizacionPorID(pDocumento.GestorDocumental.GestorTesauro.TesauroDW.ListaTesauroOrganizacion.FirstOrDefault().OrganizacionID);

                                if (orgaDW.ListaOrganizacion.Count > 0)
                                {
                                    publico = orgaDW.ListaOrganizacion.FirstOrDefault().EsBuscable;
                                    publicoEnOrganizacion = publico;
                                }
                            }
                        }
                    }
                    if (publico)
                    {
                        break;
                    }
                }
            }
            if (publico && publicoEnBRUsuario && !publicoEnOrganizacion && !publicoEnComunidad)
            {
                if (pIdentidadActual == null || pDocumento.CreadorID != pIdentidadActual.Clave)
                {
                    PersonaCN perCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    DataWrapperPersona dataWrapperPersona = perCN.ObtenerPersonasPorIdentidad(pDocumento.CreadorID);
                    publico = dataWrapperPersona.ListaPersona.FirstOrDefault().EsBuscable;
                }
                else if (pIdentidadActual != null)
                {
                    publico = pIdentidadActual.Persona.AparecerEnListaPublica;
                }
            }
            return publico;
        }

        /// <summary>
        /// Estable la visibilidad de todos los documentos de una comunidad.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pIdentidadActual">Identidad actual</param>
        /// <param name="pEstablcerVisibilidadDocsPropiosProyAFalse">Indica si debe establecerse al visibilidad de los documentos que 
        /// fueron publicados en la comunidad (no compartidos) a false.</param>
        public void EstablecePrivacidadTodosRecursosComunidadEnMetaBuscador(Guid pProyectoID, Identidad pIdentidadActual, bool pEstablcerVisibilidadDocsPropiosProyAFalse)
        {
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestorDocumental gestorDocumental = new GestorDocumental(docCN.ObtenerDocumentosVinculadosAProyecto(pProyectoID), mLoggingService, mEntityContext);
            gestorDocumental.CargarDocumentos(false);

            foreach (Documento documento in gestorDocumental.ListaDocumentos.Values)
            {
                if (pEstablcerVisibilidadDocsPropiosProyAFalse && documento.ProyectoID == pProyectoID)
                {
                    //El documento fue publicado en la comunidad y además debe ponerse a false su visibilidad:
                    documento.FilaDocumento.Publico = false;
                }
                else
                {
                    EstablecePrivacidadRecursoEnMetaBuscador(documento, pIdentidadActual, false);
                }
            }
            mEntityContext.SaveChanges();
        }

        #endregion

        #region Mapeo Tesauro

        ///// <summary>
        ///// Descarga del servidor el mapeo del tesauro solicitado
        ///// </summary>
        /////<param name="pFicheroConfiguracionBD">Fichero de configuracion de BD</param>
        /////<param name="pProyectoID">Identificador del proyecto</param>
        /////<param name="pNombreDocumento">Nombre del documento de mapeo</param>
        ///// <param name="pTipoMapeo">Cadena que define qué tipo de mapeo se va a almacenar(semántico o comunidad)</param>
        ///// <returns>Contenido del fichero de mapeo</returns>
        //public static byte[] ObtenerMapeoTesauro(Guid pProyectoID, string pNombreDocumento, string pFicheroConfiguracionBD)
        //{
        //    #region movido al proyecto DL
        //    //DocumentacionCL docCL = null;

        //    //if (!string.IsNullOrEmpty(pFicheroConfiguracionBD))
        //    //{
        //    //    docCL = new DocumentacionCL(pFicheroConfiguracionBD, pFicheroConfiguracionBD);
        //    //}
        //    //else
        //    //{
        //    //    docCL = new DocumentacionCL();
        //    //}

        //    //byte[] arrayMapping = docCL.ObtenerDocumentoMapeoTesauro(pProyectoID, pNombreDocumento);

        //    //if (arrayMapping == null)
        //    //{
        //    //    ServicioArchivos servicioArch = new ServicioArchivos();
        //    //    servicioArch.Url = UtilUsuario.UrlServicioArchivos;
        //    //    arrayMapping = servicioArch.ObtenerMappingTesauro(pNombreDocumento);
        //    //    docCL.GuardarDocumentoMapeoTesauro(pProyectoID, arrayMapping, pNombreDocumento);
        //    //    servicioArch.Dispose();
        //    //}

        //    //docCL.Dispose();

        //    //return arrayMapping;
        //    #endregion

        //    return MapeoTesauroDL.ObtenerMapeoTesauro(pProyectoID, pNombreDocumento, pFicheroConfiguracionBD, UtilUsuario.UrlServicioArchivos);
        //}

        #endregion

        #region Ontologías / Plantillas

        /// <summary>
        /// Genera un archivo en la ruta especificada con los estilos genéricos para una plantilla basada en una ontología.
        /// </summary>
        /// <param name="pNamespaceOnto">Namespace ontología</param>
        /// <param name="pOntologia">Ontología</param>
        /// <param name="pOntologiaSecundaria">Indica si es una ontología secundaria</param>
        /// <returns>Array de bytes del fichero de configuración generado</returns>
        public static byte[] GenerarArchivoConfiguracionPlantillaOntologiaGenerico(string pNamespaceOnto, Ontologia pOntologia, bool pOntologiaSecundaria)
        {
            return LectorXmlConfig.GenerarArchivoConfiguracionEstandar(pNamespaceOnto, pOntologia, pOntologiaSecundaria);
        }

        public void InsertLinkedDataRabbit(Guid pProyectoId, string pTipodocumentacion, AD.EntityModel.Models.Documentacion.DocumentoLecturaAumentada pLecturaAumentada)
        {
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            int id = proyCL.ObtenerTablaBaseProyectoIDProyectoPorID(pProyectoId);
            BaseRecursosComunidadDS baseRecursosComDS = new BaseRecursosComunidadDS();

            BaseRecursosComunidadDS.ColaTagsComunidadesRow filaColaTagsDocs = baseRecursosComDS.ColaTagsComunidades.NewColaTagsComunidadesRow();

            filaColaTagsDocs.Estado = (short)EstadosColaTags.Procesado;
            filaColaTagsDocs.FechaPuestaEnCola = DateTime.Now;
            filaColaTagsDocs.TablaBaseProyectoID = id;
            filaColaTagsDocs.Tags = Constantes.ID_TAG_DOCUMENTO + pLecturaAumentada.DocumentoID.ToString() + Constantes.ID_TAG_DOCUMENTO + "," + Constantes.TIPO_DOC + pTipodocumentacion + Constantes.TIPO_DOC + "" + "," + new UtilidadesVirtuoso(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).TagBaseAfinidadVirtuoso;


            filaColaTagsDocs.Tipo = 0;

            filaColaTagsDocs.Prioridad = 0;


            baseRecursosComDS.ColaTagsComunidades.AddColaTagsComunidadesRow(filaColaTagsDocs);
            BaseComunidadCN baseRecursosComunidadCN = new BaseComunidadCN("base", id, mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);

            baseRecursosComunidadCN.InsertarFilasEnRabbit("ColaTagsComunidadesLinkedData", baseRecursosComDS);
        }

        /// <summary>
        /// Genera un archivo de configuración a partir de una lista de estilos, para una plantilla.
        /// </summary>
        /// <param name="pListaEstilos">Lista de estilos</param>
        /// <returns>FileInfo con el archivo generado</returns>
        public static FileInfo GenerarArchivoConfiguracionPlantillaOntologia(Dictionary<string, ClaseCSS> pListaEstilos)
        {
            string rutaArchivoEstandar = LectorCSS.GenerarArchivoConfiguracion(pListaEstilos);

            return new FileInfo(rutaArchivoEstandar);
        }

        /// <summary>
        /// Comprueba si una plantilla tiene un buen formato para funcionar.
        /// </summary>
        /// <param name="pBuffer">Buffer donde está leido el fichero</param>
        /// <returns>TRUE si el archivo es óptimo para ser una plantilla</returns>
        public static bool ComprobarBuenFormatoPlantillaOWL(byte[] pBuffer)
        {
            bool plantillaCorrecta = false;

            try
            {
                string nombreTemporal = Path.GetRandomFileName() + ".owl";
                string ruta = Path.GetTempPath() + nombreTemporal;

                File.WriteAllBytes(ruta, pBuffer);

                plantillaCorrecta = ComprobarOntologiaOWLParaPlantilla(new Ontologia(ruta, true));
            }
            catch (Exception ex)
            {
                plantillaCorrecta = false;
                throw new ExcepcionGeneral(ex.Message);
            }
            return plantillaCorrecta;
        }

        /// <summary>
        /// Comprueba si una ontología puede usarse para generar una plantilla semántica.
        /// </summary>
        /// <param name="pOntologia">Ontología candidata para ser plantilla</param>
        /// <returns>TRUE si la ontología puede ser una plantilla GNOSS, FALSE en caso contrario</returns>
        public static bool ComprobarOntologiaOWLParaPlantilla(Ontologia pOntologia)
        {
            pOntologia.LeerOntologia();
            List<ElementoOntologia> listaEntidadesPrincipales = pOntologia.ObtenerElementosContenedorSuperior();

            if (listaEntidadesPrincipales.Count == 0)
            {
                //Debe contener al mentos una entidad superior, que contendrá a todas las demás.
                return false;
            }

            bool ontoCorrecta = true;

            foreach (ElementoOntologia entidadPrincipal in listaEntidadesPrincipales)
            {
                ontoCorrecta = ComprobarOntologiaOWLParaPlantillaDeHijosDeEntidadPrinpal(entidadPrincipal, pOntologia, new List<string>());

                if (!ontoCorrecta)
                {
                    break;
                }
            }

            return ontoCorrecta;

            #region Comprobación antigua, no Borrar por si las moscas

            //if (listaEntidadesPrincipales.Count != 1)
            //{
            //    //Debe contener una sola entidad superior, que contendrá a todas las demás.
            //    return false;
            //}

            //if (listaEntidadesPrincipales[0].TipoEntidad != AtributosPlantilla.TipoEntidadDocumentoGnoss || listaEntidadesPrincipales[0].Propiedades.Count != 1)
            //{
            //    //El tipo de entidad de la entidad principal debe ser TipoEntidadDocumentoGnoss y debe tener 1 propiedad.
            //    return false;
            //}
            //Propiedad propiedadEntPrin = listaEntidadesPrincipales[0].Propiedades[0];

            //if (propiedadEntPrin.Nombre != AtributosPlantilla.NombrePropiedadContenido || propiedadEntPrin.Dominio.Count != 1 || propiedadEntPrin.Dominio[0] != AtributosPlantilla.TipoEntidadDocumentoGnoss || propiedadEntPrin.Tipo != TipoPropiedad.ObjectProperty || !propiedadEntPrin.FunctionalProperty || propiedadEntPrin.Rango == null || propiedadEntPrin.Rango == "")
            //{
            //    return false;
            //}
            //return ComprobarOntologiaOWLParaPlantillaDeHijosDeEntidadPrinpal(listaEntidadesPrincipales[0], pOntologia);

            #endregion
        }

        /// <summary>
        /// Comprueba que una entidad posee las caracteristicas correctas para formar parte de una plantilla GNOSS.
        /// </summary>
        /// <param name="pEntidad">Entidad que se va a comprobar</param>
        /// <param name="pOntologia">Ontología a la que pertence la entidad</param>
        /// <param name="pListaTiposEntidadesRevisadas">Lista con los tipo de entidades ya revisadas (evita bucles infinitos)</param>
        /// <returns>TRUE si la entidad cumple los requisitos exigidos, FALSE en caso contrario</returns>
        public static bool ComprobarOntologiaOWLParaPlantillaDeHijosDeEntidadPrinpal(ElementoOntologia pEntidad, Ontologia pOntologia, List<string> pListaTiposEntidadesRevisadas)
        {
            bool entidadCorrecta = true;
            List<string> listaTiposEntidadesRevisadas = new List<string>();
            listaTiposEntidadesRevisadas.AddRange(pListaTiposEntidadesRevisadas);
            listaTiposEntidadesRevisadas.Add(pEntidad.TipoEntidad);

            foreach (Propiedad propiedad in pEntidad.Propiedades)
            {
                if (propiedad.Tipo == TipoPropiedad.ObjectProperty)
                {
                    if (pListaTiposEntidadesRevisadas.Contains(propiedad.Rango))
                    {
                        //Ontología Full, no computable
                        //return false;
                    }
                    else
                    {
                        //pListaTiposEntidadesRevisadas.Add(propiedad.Rango);
                        entidadCorrecta = entidadCorrecta && ComprobarOntologiaOWLParaPlantillaDeHijosDeEntidadPrinpal(pOntologia.GetEntidadTipo(propiedad.Rango, true), pOntologia, listaTiposEntidadesRevisadas);
                    }
                }
                else if (propiedad.Rango == null || /*propiedad.Rango == "" ||*/ propiedad.Dominio.Count == 0)
                {
                    //Toda propiedad debe tener rango y dominio
                    return false;
                }
            }
            return entidadCorrecta;
        }

        /// <summary>
        /// Descarga del servidor la ontología solicitada y devuelve la ruta del fichero descargado.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento de la ontología</param>
        /// <param name="pListaEstilos">Lista con los estilos para el formulario semántico</param>
        /// <param name="pDocumentoIDout">Página</param>
        /// <returns>ruta del fichero de ontología descargado</returns>
        public byte[] ObtenerOntologia(Guid pDocumentoID, Guid pProyectoID)
        {
            return ObtenerOntologia(pDocumentoID, pProyectoID, null);
        }

        /// <summary>
        /// Descarga del servidor la ontología solicitada y devuelve la ruta del fichero descargado.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento de la ontología</param>
        /// <param name="pListaEstilos">Lista con los estilos para el formulario semántico</param>
        /// <param name="pDocumentoIDout">Página</param>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuracion de BD</param>
        /// <returns>ruta del fichero de ontología descargado</returns>
        public byte[] ObtenerOntologia(Guid pDocumentoID, Guid pProyectoID, string pFicheroConfiguracionBD)
        {
            DocumentacionCL docCL = null;

            if (!string.IsNullOrEmpty(pFicheroConfiguracionBD))
            {
                docCL = new DocumentacionCL(pFicheroConfiguracionBD, pFicheroConfiguracionBD, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            }
            else
            {
                docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            }
            byte[] arryaOnto = docCL.ObtenerOntologia(pDocumentoID);

            if (arryaOnto == null)
            {
                CallFileService servicioArch = new CallFileService(mConfigService, mLoggingService);
                arryaOnto = servicioArch.ObtenerOntologiaBytes(pDocumentoID);
                docCL.GuardarOntologia(pDocumentoID, arryaOnto);

            }

            docCL.Dispose();

            return arryaOnto;
        }

        /// <summary>
        /// Descarga del servidor la ontología solicitada y devuelve la ruta del fichero descargado.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento de la ontología</param>
        /// <param name="pListaEstilos">Lista con los estilos para el formulario semántico</param>
        /// <param name="pDocumentoIDout">Página</param>
        /// <returns>ruta del fichero de ontología descargado</returns>
        public byte[] ObtenerOntologia(Guid pDocumentoID, out Dictionary<string, List<EstiloPlantilla>> pListaEstilos, Guid pProyectoID)
        {
            return ObtenerOntologia(pDocumentoID, out pListaEstilos, pProyectoID, null);
        }

        /// <summary>
        /// Descarga del servidor la ontología solicitada y devuelve la ruta del fichero descargado.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento de la ontología</param>
        /// <param name="pListaEstilos">Lista con los estilos para el formulario semántico</param>
        /// <param name="pDocumentoIDout">Página</param>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuracion de BD</param>
        /// <returns>ruta del fichero de ontología descargado</returns>
        public byte[] ObtenerOntologia(Guid pDocumentoID, out Dictionary<string, List<EstiloPlantilla>> pListaEstilos, Guid pProyectoID, string pFicheroConfiguracionBD)
        {
            return ObtenerOntologia(pDocumentoID, out pListaEstilos, pProyectoID, pFicheroConfiguracionBD, null);
        }

        /// <summary>
        /// Descarga del servidor la ontología solicitada y devuelve la ruta del fichero descargado.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento de la ontología</param>
        /// <param name="pListaEstilos">Lista con los estilos para el formulario semántico</param>
        /// <param name="pDocumentoIDout">Página</param>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuracion de BD</param>
        /// <param name="pParamSemCms">Parámetro especial para el sem cms</param>
        /// <returns>ruta del fichero de ontología descargado</returns>
        public byte[] ObtenerOntologia(Guid pDocumentoID, out Dictionary<string, List<EstiloPlantilla>> pListaEstilos, Guid pProyectoID, string pFicheroConfiguracionBD, string pParamSemCms)
        {
            return ObtenerOntologia(pDocumentoID, out pListaEstilos, pProyectoID, pFicheroConfiguracionBD, pParamSemCms, true);
        }

        /// <summary>
        /// Descarga del servidor la ontología solicitada y devuelve la ruta del fichero descargado.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento de la ontología</param>
        /// <param name="pListaEstilos">Lista con los estilos para el formulario semántico</param>
        /// <param name="pDocumentoIDout">Página</param>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuracion de BD</param>
        /// <param name="pParamSemCms">Parámetro especial para el sem cms</param>
        /// <returns>ruta del fichero de ontología descargado</returns>
        public byte[] ObtenerOntologia(Guid pDocumentoID, out Dictionary<string, List<EstiloPlantilla>> pListaEstilos, Guid pProyectoID, string pFicheroConfiguracionBD, string pParamSemCms, bool pCargaEstilosObligatoria)
        {
            byte[] arryaOnto = ObtenerOntologia(pDocumentoID, pProyectoID, pFicheroConfiguracionBD);

            try
            {
                CargarEstilosOntologia(pDocumentoID, out pListaEstilos, pProyectoID, pFicheroConfiguracionBD, pParamSemCms);
            }
            catch (Exception ex)
            {
                if (pCargaEstilosObligatoria)
                {
                    mLoggingService.GuardarLogError($"EXCEPCION AL CargarEstilosOntologia --> {ex.Message}");
                    throw;
                }

                pListaEstilos = new Dictionary<string, List<EstiloPlantilla>>();
            }

            return arryaOnto;
        }

        /// <summary>
        /// Descarga del servidor la ontología solicitada y devuelve la ruta del fichero descargado.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento de la ontología</param>
        /// <param name="pNombreOntologia">Nombre de la ontología</param>
        /// <param name="pTipoEntidad">Nombre de la entidad fracción</param>
        /// <param name="pListaEstilos">Lista con los estilos para el formulario semántico</param>
        /// <param name="pProyectoID">ID del proyecto actual</param>
        /// <param name="pDocumentoIDout">Página</param>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuracion de BD</param>
        /// <returns>ruta del fichero de ontología descargado</returns>
        public byte[] ObtenerOntologiaFraccionada(Guid pDocumentoID, string pNombreOntologia, string pTipoEntidad, out Dictionary<string, List<EstiloPlantilla>> pListaEstilos, Guid pProyectoID, string pFicheroConfiguracionBD)
        {
            string nombreFraccion = pNombreOntologia.Substring(0, pNombreOntologia.LastIndexOf(".")).ToLower() + "_" + pTipoEntidad;

            CallFileService servicioArch = new CallFileService(mConfigService, mLoggingService);

            byte[] arryaOnto = servicioArch.ObtenerOntologiaFraccionada(pDocumentoID, nombreFraccion);


            CargarEstilosOntologiaFraccionadoSegunEntidad(pDocumentoID, pNombreOntologia, pNombreOntologia.Replace(".", "") + ":" + pTipoEntidad, out pListaEstilos, pProyectoID, pFicheroConfiguracionBD);

            if (pListaEstilos == null)
            {
                CargarEstilosOntologia(pDocumentoID, out pListaEstilos, pProyectoID, pFicheroConfiguracionBD);
            }

            return arryaOnto;
        }

        /// <summary>
        /// Carga los estilos de los controles del formulario de la ontología.
        /// </summary>
        /// <param name="pOntologiaID">ID de la ontolgía</param>
        /// <param name="pListaEstilos">Lista de configuraciones</param>
        /// <param name="pProyectoID">ID de proyecto actual</param>
        public void CargarEstilosOntologia(Guid pOntologiaID, out Dictionary<string, List<EstiloPlantilla>> pListaEstilos, Guid pProyectoID)
        {
            CargarEstilosOntologia(pOntologiaID, out pListaEstilos, pProyectoID, null);
        }

        /// <summary>
        /// Carga los estilos de los controles del formulario de la ontología.
        /// </summary>
        /// <param name="pOntologiaID">ID de la ontolgía</param>
        /// <param name="pListaEstilos">Lista de configuraciones</param>
        /// <param name="pProyectoID">ID de proyecto actual</param>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuracion</param>
        private void CargarEstilosOntologia(Guid pOntologiaID, out Dictionary<string, List<EstiloPlantilla>> pListaEstilos, Guid pProyectoID, string pFicheroConfiguracionBD)
        {
            CargarEstilosOntologia(pOntologiaID, out pListaEstilos, pProyectoID, pFicheroConfiguracionBD, null);
        }

        /// <summary>
        /// Carga los estilos de los controles del formulario de la ontología.
        /// </summary>
        /// <param name="pOntologiaID">ID de la ontolgía</param>
        /// <param name="pListaEstilos">Lista de configuraciones</param>
        /// <param name="pProyectoID">ID de proyecto actual</param>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuracion</param>
        /// <param name="pParamSemCms">Parámetro especial para el sem cms</param>
        public void CargarEstilosOntologia(Guid pOntologiaID, out Dictionary<string, List<EstiloPlantilla>> pListaEstilos, Guid pProyectoID, string pFicheroConfiguracionBD, string pParamSemCms)
        {
            LectorXmlConfig lector = null;

            if (!string.IsNullOrEmpty(pFicheroConfiguracionBD))
            {
                lector = new LectorXmlConfig(pOntologiaID, pProyectoID, pFicheroConfiguracionBD, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD);
            }
            else
            {
                lector = new LectorXmlConfig(pOntologiaID, pProyectoID, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD);
            }

            lector.ParametroSemCms = pParamSemCms;
            pListaEstilos = lector.ObtenerConfiguracionXml();
        }

        /// <summary>
        /// Carga los estilos de los controles del formulario de la ontología que están fraccionados según la entidad principal del recurso.
        /// </summary>
        /// <param name="pOntologiaID">ID de la ontolgía</param>
        /// <param name="pListaEstilos">Lista de configuraciones</param>
        /// <param name="pProyectoID">ID de proyecto actual</param>
        public void CargarEstilosOntologiaFraccionadoSegunEntidad(Guid pOntologiaID, string pNombreOnto, string pTipoEntidad, out Dictionary<string, List<EstiloPlantilla>> pListaEstilos, Guid pProyectoID, string pFicheroConfiguracionBD)
        {
            LectorXmlConfig lector = new LectorXmlConfig(pOntologiaID, pProyectoID, pFicheroConfiguracionBD, pNombreOnto, pTipoEntidad, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD);
            pListaEstilos = lector.ObtenerConfiguracionXml();
        }

        public List<ElementoOntologia> ObtenerElmOntoRdfDocumento(Documento pDocumento, Guid pProyectoID, out Ontologia pOntologia, out string pRdfTexto)
        {
            throw new Exception("Codigo viejo");
        }

        /// <summary>
        /// Obtiene el ID del XML de una ontología.
        /// </summary>
        /// <returns>ID del XML de una ontología</returns>
        public Guid ObtenerIDXmlOntologia(Guid pOntologiaID, Guid pProyectoID, string pFicheroConfiguracionBD)
        {
            DocumentacionCL docCL = null;
            if (!string.IsNullOrEmpty(pFicheroConfiguracionBD))
            {
                docCL = new DocumentacionCL(pFicheroConfiguracionBD, pFicheroConfiguracionBD, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            }
            else
            {
                docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            }

            Guid? xmlID = docCL.ObtenerObjetoDeCacheLocal(pOntologiaID.ToString()) as Guid?;

            if (xmlID == null)
            {
                xmlID = docCL.ObtenerIDXmlOntologia(pOntologiaID);
                docCL.AgregarObjetoCacheLocal(pProyectoID, pOntologiaID.ToString(), xmlID);
            }

            if (xmlID == null)
            {
                xmlID = Guid.NewGuid();
                docCL.GuardarIDXmlOntologia(pOntologiaID, xmlID.Value);
                docCL.AgregarObjetoCacheLocal(pProyectoID, pOntologiaID.ToString(), xmlID);
            }
            docCL.Dispose();

            return xmlID.Value;
        }

        /// <summary>
        /// Guarda un rdf en virtuoso.
        /// </summary>
        /// <param name="pEntidadesPrinc">Entidades principales</param>
        /// <param name="pNombreGrafo">Nombre del grafo en el que se guardará el RDF</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        /// <param name="pFicheroConfiguracion">Fichero de configuración de la BD</param>
        /// <param name="pProyectoID">ID del proyecto actual</param>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <param name="pEscribirNT">Escribe o no NT</param>
        /// <param name="pInfoExtra">Info extra</param>
        /// <param name="pRecursoBorrador">Indica si es un recurso borrador</param>
        /// <param name="pUsarColareplicacion">Indica si hay que usar la cola de replicación</param>
        /// <param name="pPrioridad">Indica la prioridad en la cola de replicación</param>
        public List<TripleWrapper> GuardarRDFEnVirtuoso(List<ElementoOntologia> pEntidadesPrinc, string pNombreGrafo, string pUrlIntragnoss, string pFicheroConfiguracion, Guid pProyectoID, string pDocumentoID, bool pEscribirNT, string pInfoExtra, bool pRecursoBorrador, bool pUsarColareplicacion, short pPrioridad, GnossStringBuilder pSbInsert = null, GnossStringBuilder pSbDelete = null)
        {
            mLoggingService.AgregarEntrada("Antes de EscribirTripletasEntidad");

            List<TripleWrapper> tripletasDSList = new List<TripleWrapper>();
            StringBuilder sb = new StringBuilder();

            foreach (ElementoOntologia entidad in pEntidadesPrinc)
            {
                EscribirTripletasEntidad(entidad, sb, tripletasDSList, pEscribirNT, pUrlIntragnoss, new List<ElementoOntologia>(), pRecursoBorrador);
            }

            string tripletas = sb.ToString();

            mLoggingService.AgregarEntrada("Después de EscribirTripletasEntidad");

            return GuardarTripletasRDFEnVirtuoso(pNombreGrafo, pUrlIntragnoss, pFicheroConfiguracion, pProyectoID, pDocumentoID, pEscribirNT, pInfoExtra, tripletas, tripletasDSList, pUsarColareplicacion, pPrioridad, pSbInsert, pSbDelete);
        }
        /// <summary>
        /// Escribe la tripletas de una entiadad.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pTripletas">Cadena con las tripletas</param>
        /// <param name="pTripletasDSList">Array con las tripletas</param>
        /// <param name="pEscribirNT">Indica si las tripletas se escribirán en un NT</param>
        /// <param name="pUrlIntragnoss">Url de intragnoss</param>
        /// <param name="pEntidadEscritas">Lista con las entidades escritas</param>
        /// <param name="pRecursoBorrador">Indica si es un recurso borrador</param>
        private void EscribirTripletasEntidad(ElementoOntologia pEntidad, StringBuilder sb, List<TripleWrapper> pTripletasDSList, bool pEscribirNT, string pUrlIntragnoss, List<ElementoOntologia> pEntidadEscritas, bool pRecursoBorrador)
        {
            if (pEntidadEscritas.Contains(pEntidad))
            {
                return;
            }

            pEntidadEscritas.Add(pEntidad);

            string sujeto = $"<{pEntidad.Uri}>";
            string objeto = pEntidad.TipoEntidad;

            sb.Append(new FacetadoAD(pUrlIntragnoss, mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).GenerarTripletaSinConversionesAbsurdas(sujeto, $"<{GestionOWL.PropTipoRdf}>", UtilCadenas.PasarAUtf8($"<{objeto}>")));
            pTripletasDSList.Add(new TripleWrapper() { Subject = sujeto, Predicate = $"<{GestionOWL.PropTipoRdf}>", Object = $"<{objeto}>" });
            sb.Append(new FacetadoAD(pUrlIntragnoss, mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).GenerarTripletaSinConversionesAbsurdas(sujeto, $"<{GestionOWL.PropLabelRdf}>", UtilCadenas.PasarAUtf8($"\"{objeto}\"")));
            pTripletasDSList.Add(new TripleWrapper() { Subject = sujeto, Predicate = $"<{GestionOWL.PropLabelRdf}>", Object = $"<{objeto}>" });

            if (pRecursoBorrador)
            {
                sb.Append(new FacetadoAD(pUrlIntragnoss, mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).GenerarTripletaSinConversionesAbsurdas(sujeto, $"<{GestionOWL.PropBorradorGnossRdf}>", UtilCadenas.PasarAUtf8("\"true\"")));
            }

            List<ElementoOntologia> entidadesHijas = new List<ElementoOntologia>();

            foreach (Propiedad propiedad in pEntidad.Propiedades)
            {
                bool esObjectProperty = (propiedad.Tipo == TipoPropiedad.ObjectProperty);
                string tipo = null;

                if (!esObjectProperty)
                {
                    tipo = propiedad.Rango;
                }

                foreach (string valor in propiedad.ValoresUnificados.Keys)
                {
                    objeto = valor;

                    if (esObjectProperty)
                    {
                        ElementoOntologia entidadHija = propiedad.ValoresUnificados[valor];

                        if (entidadHija != null)
                        {
                            entidadesHijas.Add(entidadHija);
                            objeto = entidadHija.Uri;
                        }
                        else if (!objeto.StartsWith("http"))
                        {
                            objeto = pUrlIntragnoss + "items/" + objeto;
                        }
                    }

                    objeto = objeto.Replace("\r\n", "");
                    objeto = objeto.Replace("\n", "");

                    EscribirTripletaEntidad(sujeto, $"<{propiedad.NombreFormatoUri}>", objeto, sb, pTripletasDSList, pEscribirNT, null, tipo);
                }

                if (esObjectProperty)
                {
                    continue;
                }

                foreach (string idioma in propiedad.ListaValoresIdioma.Keys)
                {
                    foreach (string valor in propiedad.ListaValoresIdioma[idioma].Keys)
                    {
                        objeto = valor;

                        if (propiedad.Tipo == TipoPropiedad.ObjectProperty)
                        {
                            ElementoOntologia entidadHija = propiedad.ListaValoresIdioma[idioma][valor];

                            if (entidadHija != null)
                            {
                                entidadesHijas.Add(entidadHija);
                                objeto = entidadHija.Uri;
                            }
                            else if (!objeto.StartsWith("http"))
                            {
                                objeto = pUrlIntragnoss + "items/" + objeto;
                            }
                        }

                        objeto = objeto.Replace("\r\n", "");
                        objeto = objeto.Replace("\n", "");

                        EscribirTripletaEntidad(sujeto, $"<{propiedad.NombreFormatoUri}>", objeto, sb, pTripletasDSList, pEscribirNT, idioma, tipo);
                    }
                }
            }

            foreach (ElementoOntologia entidad in entidadesHijas)
            {
                EscribirTripletasEntidad(entidad, sb, pTripletasDSList, pEscribirNT, pUrlIntragnoss, pEntidadEscritas, false);
            }
        }

        /// <summary>
        /// Escribe la tripletas de una entiadad.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pTripletas">Cadena con las tripletas</param>
        /// <param name="pTripletasDSList">Array con las tripletas</param>
        /// <param name="pEsribirNT">Indica si las tripletas se escribirán en un NT</param>
        public void EscribirTripletaEntidad(string pSujeto, string pPredicado, string pObjeto, ref string pTripletas, List<TripleWrapper> pTripletasDSList, bool pEsribirNT, string pIdioma)
        {

            if (!pSujeto.StartsWith("<"))
            {
                pSujeto = $"<{pSujeto}>";
            }

            if (!pPredicado.StartsWith("<"))
            {
                pPredicado = $"<{pPredicado}>";
            }

            Uri uri = null;
            Uri.TryCreate(pObjeto, UriKind.Absolute, out uri);

            if (uri != null && pObjeto.Contains("http://") && !pObjeto.Contains("|") && !pObjeto.Contains(" ") && !pObjeto.Contains(","))
            {
                pObjeto = $"<{pObjeto}>";
            }
            else
            {
                if (pObjeto.StartsWith("literal@http"))
                {
                    pObjeto = pObjeto.Replace("literal@", "");
                }

                if (pObjeto.StartsWith("http@"))
                {//Fuerzo a que sea link:
                    pObjeto = $"<{pObjeto.Replace("http@", "")}>";
                }
                else
                {
                    pObjeto = $"\"{pObjeto}\"";
                }

                if (pObjeto.Contains("^^"))
                {
                    pObjeto = pObjeto.Substring(0, pObjeto.IndexOf("^^"));
                }
            }

            pTripletas += new FacetadoAD("", mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).GenerarTripletaSinConversionesAbsurdas(pSujeto, pPredicado, /*FacetadoAD.PasarAUtf8(*/pObjeto/*)*/, pIdioma);

            pTripletasDSList.Add(new TripleWrapper() { Subject = pSujeto, Predicate = pPredicado, Object = pObjeto, ObjectLanguage = pIdioma });
        }

        /// <summary>
        /// Escribe la tripletas de una entiadad.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pTripletas">Cadena con las tripletas</param>
        /// <param name="pTripletasDSList">Array con las tripletas</param>
        /// <param name="pEsribirNT">Indica si las tripletas se escribirán en un NT</param>
        public void EscribirTripletaEntidad(string pSujeto, string pPredicado, string pObjeto, StringBuilder sb, List<TripleWrapper> pTripletasDSList, bool pEsribirNT, string pIdioma, string pTipo)
        {

            if (!pSujeto.StartsWith("<"))
            {
                pSujeto = $"<{pSujeto}>";
            }

            if (!pPredicado.StartsWith("<"))
            {
                pPredicado = $"<{pPredicado}>";
            }

            Uri uri = null;
            Uri.TryCreate(pObjeto, UriKind.Absolute, out uri);

            if (uri != null && pObjeto.Contains("http://") && !pObjeto.Contains("|") && !pObjeto.Contains(" ") && !pObjeto.Contains(",") && (pTipo == null || !pTipo.Equals(FacetadoAD.XSD_STRING) || pTipo.StartsWith("http://gnoss")))
            {
                pObjeto = $"<{pObjeto}>";
            }
            else
            {
                if (pObjeto.StartsWith("literal@http"))
                {
                    pObjeto = pObjeto.Replace("literal@", "");
                }

                if (pObjeto.StartsWith("http@"))
                {//Fuerzo a que sea link:
                    pObjeto = $"<{pObjeto.Replace("http@", "")}>";
                }
                else
                {
                    pObjeto = $"\"{pObjeto}\"";
                }

                if (pObjeto.Contains("^^"))
                {
                    pObjeto = pObjeto.Substring(0, pObjeto.IndexOf("^^"));
                }
            }

            sb.Append(new FacetadoAD("", mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).GenerarTripletaSinConversionesAbsurdas(pSujeto, pPredicado, /*FacetadoAD.PasarAUtf8(*/pObjeto/*)*/, pIdioma));

            pTripletasDSList.Add(new TripleWrapper() { Subject = pSujeto, Predicate = pPredicado, Object = pObjeto, ObjectLanguage = pIdioma, ObjectType = pTipo });
        }

        /// <summary>
        /// Guarda un rdf en virtuoso.
        /// </summary>
        /// <param name="pNombreGrafo">Nombre del grafo en el que se guardará el RDF</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        /// <param name="pFicheroConfiguracion">Fichero de configuración de la BD</param>
        /// <param name="pProyectoID">ID del proyecto actual</param>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <param name="pEscribirNT">Escribe o no NT</param>
        /// <param name="pInfoExtra">Info extra</param>
        /// <param name="pTripletas">Tripletas a guardar en virtuoso</param>
        /// <param name="pTripletasDSList">Tripletas para el grafo de la comunidad</param>
        private List<TripleWrapper> GuardarTripletasRDFEnVirtuoso(string pNombreGrafo, string pUrlIntragnoss, string pFicheroConfiguracion, Guid pProyectoID, string pDocumentoID, bool pEscribirNT, string pInfoExtra, string pTripletas, List<TripleWrapper> pTripletasDSList, bool pUsarColareplicacion, short pPrioridad, GnossStringBuilder pSbInsert = null, GnossStringBuilder pSbDelete = null)
        {
            mLoggingService.AgregarEntrada("Antes de entidadsecun_ en GuardarTripletasRDFEnVirtuoso");

            //Introduzco por cada entidad, una vinculación con su elemento padre
            string[] separador = { " \n " };
            string[] filas = pTripletas.Split(separador, StringSplitOptions.RemoveEmptyEntries);
            List<string> listaSujetos = new List<string>();
            string sujetoDocumento = "<" + pUrlIntragnoss + pDocumentoID + "> ";
            string saltoDeLinea = " \n ";

            mLoggingService.AgregarEntrada("Antes de new StringBuilder");

            StringBuilder sb = new StringBuilder();
            mLoggingService.AgregarEntrada("Antes de Append");
            sb.Append(pTripletas);
            mLoggingService.AgregarEntrada("Después de Append");

            int longitud = filas.Length;

            foreach (string fila in filas)
            {
                if (!string.IsNullOrEmpty(fila) && fila.Contains(" "))
                {
                    string sujeto = fila.Substring(0, fila.IndexOf(' '));
                    if (!listaSujetos.Contains(sujeto))
                    {
                        listaSujetos.Add(sujeto);
                        //pTripletas += saltoDeLinea + sujetoDocumento + "<http://gnoss/hasEntidad> " + sujeto + " ." + saltoDeLinea;
                        sb.Append($"{saltoDeLinea}{sujetoDocumento}<http://gnoss/hasEntidad> {sujeto} .{saltoDeLinea}");
                        longitud++;
                        if (pDocumentoID.Contains("entidadsecun_")) //Es entidad secundaria
                        {
                            TripleWrapper datos = new TripleWrapper { Subject = sujetoDocumento, Predicate = "<http://gnoss/hasEntidad>", Object = sujeto };
                            pTripletasDSList.Add(datos);
                        }
                    }
                }
            }


            mLoggingService.AgregarEntrada("Antes de sb.ToString");
            pTripletas = sb.ToString();

            mLoggingService.AgregarEntrada("Después de entidadsecun_ en GuardarTripletasRDFEnVirtuoso");

            mLoggingService.AgregarEntrada("Antes de InsertaTripletasRDFEnVirtuosoConModify");

            if (pSbDelete == null && pSbInsert == null)
            {
                //si superamos los 1400 triples o los 10Mb de datos
                if (longitud > 1400 || (pTripletas.Length / (1024 * 1024) > 10))
                {
                    BorrarYEscribirEnVirtuosoConCilenteTradicional(pDocumentoID, pNombreGrafo, pUrlIntragnoss, pFicheroConfiguracion, pProyectoID, pInfoExtra, pUsarColareplicacion, pEscribirNT, pTripletas);
                }
                else
                {
                    InsertaTripletasRDFEnVirtuosoConModify(pNombreGrafo, pProyectoID, pUrlIntragnoss, pFicheroConfiguracion, pDocumentoID, pTripletas, pUsarColareplicacion, pInfoExtra, pPrioridad, pEscribirNT);
                }
            }
            else
            {
                FacetadoAD facetadoAD = new FacetadoAD(pUrlIntragnoss, mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                if (pSbDelete != null && pSbDelete.GetStringBuilder().Count > 0)
                {
                    foreach (StringBuilder stringBuilder in pSbDelete.GetStringBuilder())
                    {
                        stringBuilder.Insert(0, $"DELETE DATA FROM <{pUrlIntragnoss}{pNombreGrafo.ToLower()}> {{");
                        stringBuilder.AppendLine("}");
                        facetadoAD.ActualizarVirtuoso(stringBuilder.ToString(), pNombreGrafo.ToLower());
                    }
                }

                if (pSbInsert != null && pSbInsert.GetStringBuilder().Count > 0)
                {
                    foreach (StringBuilder stringBuilder in pSbInsert.GetStringBuilder())
                    {
                        stringBuilder.Insert(0, $"INSERT DATA INTO <{pUrlIntragnoss}{pNombreGrafo.ToLower()}> {{");
                        stringBuilder.AppendLine("}");
                        facetadoAD.ActualizarVirtuoso(stringBuilder.ToString(), pNombreGrafo.ToLower());
                    }
                }
            }

            return pTripletasDSList;
        }


        private void BorrarYEscribirEnVirtuosoConCilenteTradicional(string pDocumentoID, string pNombreGrafo, string pUrlIntragnoss, string pFicheroConfiguracion, Guid pProyectoID, string pInfoExtra, bool pUsarColareplicacion, bool pEscribirNT, string pTripletas, int pNumIntentos = 0)
        {
            FacetadoAD facetadoAD = new FacetadoAD(pUrlIntragnoss, mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

            //if (string.IsNullOrEmpty(mServicesUtilVirtuosoAndReplication.ConexionAfinidad))
            //{
            //    // Necesito tener afinidad con un servidor de virtuoso concreto, si voy contra el HaProxy no sé contra qué servidor he escrito
            //    mServicesUtilVirtuosoAndReplication.ConexionAfinidad = "acid_Master";
            //    mVirtuosoAD.FechaFinAfinidad = DateTime.Now.AddMinutes(1);
            //}

            try
            {
                facetadoAD.UsarClienteTradicional = true;

                facetadoAD.IniciarTransaccion();
                try
                {
                    BorrarRDFDeVirtuoso(pDocumentoID, pNombreGrafo, pUrlIntragnoss, pFicheroConfiguracion, pProyectoID, pInfoExtra, pUsarColareplicacion);
                    InsertaTripletasRDFEnVirtuoso(pUrlIntragnoss, pNombreGrafo, pFicheroConfiguracion, pProyectoID, pEscribirNT, pInfoExtra, pTripletas, pUsarColareplicacion);
                    facetadoAD.TerminarTransaccion(true);

                    // TODO añadir servidor a afinidad
                }
                catch
                {
                    facetadoAD.TerminarTransaccion(false);
                    facetadoAD.CerrarConexion();
                    throw;
                }
                finally
                {
                    facetadoAD.UsarClienteTradicional = false;
                }
            }
            catch
            {
                if (pNumIntentos < 120)
                {
                    Thread.Sleep(2000);
                    if (pNumIntentos == 0)
                    {
                        CallWebMethods.CallGetApi(mConfigService.ObtenerServicioAfinidad(), "RelatedVirtuoso");
                    }
                    BorrarYEscribirEnVirtuosoConCilenteTradicional(pDocumentoID, pNombreGrafo, pUrlIntragnoss, pFicheroConfiguracion, pProyectoID, pInfoExtra, pUsarColareplicacion, pEscribirNT, pTripletas, pNumIntentos + 1);
                }
                else
                {
                    //Hemos alcanzado el número máximo de intentos, relanzo la excepción
                    throw;
                }
            }
        }

        /// <summary>
        /// Guarda un rdf en virtuoso.
        /// </summary>
        /// <param name="pRutaRDF">Ruta del fichero RDF</param>
        /// <param name="pDocumentoID">DataSet para RDF</param>
        /// <param name="pProyectoID">ID del proyecto actual</param>
        /// <param name="pRdfDS">DataSet para RDF</param>
        public void GuardarRDFEnBDRDF(string pFicheroRDF, Guid pDocumentoID, Guid pProyectoID, RdfDS pRdfDS)
        {
            RdfDS.RdfDocumentoRow filaRdfDoc = null;

            if (pRdfDS == null || pRdfDS.RdfDocumento.Select("DocumentoID='" + pDocumentoID + "' AND ProyectoID='" + pProyectoID + "'").Length == 0)
            {
                //si llega nulo puede haber ocurrido un problema temporal de lectura de BD, reintentar la lectura de BD para asegurar que el rdf no existe
                // La segunda condición previene cuando se intentan guardar entidades externas desde un selector
                pRdfDS = ObtenerRDFDeBDRDF(pDocumentoID, pProyectoID);
            }

            if (pRdfDS.RdfDocumento.Count == 0)
            {
                pRdfDS = new RdfDS();
                filaRdfDoc = pRdfDS.RdfDocumento.NewRdfDocumentoRow();
                filaRdfDoc.DocumentoID = pDocumentoID;
                filaRdfDoc.ProyectoID = pProyectoID;
                pRdfDS.RdfDocumento.AddRdfDocumentoRow(filaRdfDoc);
            }
            else
            {
                filaRdfDoc = (RdfDS.RdfDocumentoRow)pRdfDS.RdfDocumento.Select("DocumentoID='" + pDocumentoID + "' AND ProyectoID='" + pProyectoID + "'")[0];
            }

            if (IsValidXmlString(pFicheroRDF))
            {
                pFicheroRDF = RemoveInvalidXmlChars(pFicheroRDF);
            }

            filaRdfDoc.RdfSem = pFicheroRDF;

            RdfCN rdfCN = new RdfCN("rdf", pDocumentoID.ToString().Substring(0, 3), mEntityContext, mEntityContextBASE, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            rdfCN.ActualizarBD(pRdfDS);
        }

        /// <summary>
        /// Guarda un rdf en la BD Rdf.
        /// </summary>
        /// <param name="pRDF">cadena con el RDF</param>
        /// <param name="pDocumentoID">DataSet para RDF</param>
        /// <param name="pProyectoID">ID del proyecto actual</param>
        public void GuardarRDFEnBDRDF(string pRDF, Guid pDocumentoID, Guid pProyectoID)
        {
            RdfDS.RdfDocumentoRow filaRdfDoc = null;

            RdfDS rdfDS = new RdfDS();
            filaRdfDoc = rdfDS.RdfDocumento.NewRdfDocumentoRow();
            filaRdfDoc.DocumentoID = pDocumentoID;
            filaRdfDoc.ProyectoID = pProyectoID;
            rdfDS.RdfDocumento.AddRdfDocumentoRow(filaRdfDoc);

            if (IsValidXmlString(pRDF))
            {
                pRDF = RemoveInvalidXmlChars(pRDF);
            }

            filaRdfDoc.RdfSem = pRDF;

            RdfCN rdfCN = new RdfCN("rdf", pDocumentoID.ToString().Substring(0, 3), mEntityContext, mEntityContextBASE, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            rdfCN.ActualizarBD(rdfDS);
        }

        /// <summary>
        /// Borrar un rdf de la BD Rdf.
        /// </summary>
        /// <param name="pDocumentoID">DataSet para RDF</param>
        /// <param name="pProyectoID">ID del proyecto actual</param>
        public void BorrarRDFDeBDRDF(Guid pDocumentoID)
        {
            RdfCN rdfCN = new RdfCN("rdf", pDocumentoID.ToString().Substring(0, 3), mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            rdfCN.EliminarDocumentoDeRDF(pDocumentoID);
        }

        /// <summary>
        /// Borrar un rdf de la BD Rdf.
        /// </summary>
        /// <param name="pDocumentoID">DataSet para RDF</param>
        /// <param name="pProyectoID">ID del proyecto actual</param>
        public void BorrarRDFDeBDRDFSinTransaccion(Guid pDocumentoID)
        {
            RdfCN rdfCN = new RdfCN("rdf", pDocumentoID.ToString().Substring(0, 3), mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            rdfCN.EliminarDocumentoDeRDFSinTransaccion(pDocumentoID);
        }

        /// <summary>
        /// Obtiene el DataSet de RDF de un rdf en virtuoso.
        /// </summary>
        /// <param name="pDocumentoID">ID del documento</param>
        /// <param name="pProyectoID">ID del proyecto actual</param>
        /// <returns>DataSet para RDF</returns>
        public RdfDS ObtenerRDFDeBDRDF(Guid pDocumentoID, Guid pProyectoID)
        {
            RdfDS rdfDS = null;

            try
            {
                RdfCN rdfCN = new RdfCN("rdf", pDocumentoID.ToString().Substring(0, 3), mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                rdfDS = rdfCN.ObtenerRdfPorDocumentoID(pDocumentoID, pProyectoID);
            }
            catch (Exception)
            {
                rdfDS = new RdfDS();
            }

            return rdfDS;
        }

        /// <summary>
        /// Obtiene el texto RDF de un rdf en virtuoso.
        /// </summary>
        /// <param name="pDocumentoID">ID del documento</param>
        /// <param name="pProyectoID">ID del proyecto actual</param>
        /// <param name="pNamespaceOnto">Namespace de la ontología</param>
        /// <returns>DataSet para RDF</returns>
        public string ObtenerTextoRDFDeBDRDF(Guid pDocumentoID, Guid pProyectoID, string pNamespaceOnto)
        {
            string rdfText = "";
            RdfDS rdfDS = ObtenerRDFDeBDRDF(pDocumentoID, pProyectoID);

            if (rdfDS.RdfDocumento.Count > 0)
            {
                rdfText = rdfDS.RdfDocumento[0].RdfSem;
            }
            rdfDS.Dispose();

            if (!string.IsNullOrEmpty(pNamespaceOnto))
            {
                rdfText = rdfText.Replace(GestionOWL.NAMESPACE_ONTO_GNOSS + ":", pNamespaceOnto + ":").Replace(GestionOWL.NAMESPACE_ONTO_GNOSS + "=", pNamespaceOnto + "=");
            }

            return rdfText;
        }

        /// <summary>
        /// Obtiene la ruta del fichero RDF de un documento semántico almacenado en virtuoso.
        /// </summary>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <param name="pNombreGrafo">Nombre del grafo en virtuoso</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        /// <param name="pUrlOntologia">URL de la ontología</param>
        /// <param name="pNamespaceOntologia">Namespace de la ontología</param>
        /// <param name="pOntologia">Ontología del documento</param>
        /// <param name="pFicheroConfiguracion">Fichero de configuración</param>
        public string ObtenerRDFDeVirtuosoEnArchivo(Guid pDocumentoID, string pNombreGrafo, string pUrlIntragnoss, string pUrlOntologia, string pNamespaceOntologia, Ontologia pOntologia, string pFicheroConfiguracion)
        {
            string nombreTemporal = Path.GetRandomFileName() + ".rdf";
            string ruta = Path.GetTempPath() + nombreTemporal;
            FileStream stream = new FileStream(ruta, FileMode.Create);
            byte[] buffer = ObtenerRDFDeVirtuoso(pDocumentoID, pNombreGrafo, pUrlIntragnoss, pUrlOntologia, pNamespaceOntologia, pOntologia, pFicheroConfiguracion, false);
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
            stream.Close();
            stream.Dispose();

            return ruta;
        }

        /// <summary>
        /// Obtiene los bytes del fichero RDF de un documento semántico almacenado en virtuoso.
        /// </summary>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <param name="pNombreGrafo">Nombre del grafo en virtuoso</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        /// <param name="pUrlOntologia">URL de la ontología</param>
        /// <param name="pNamespaceOntologia">Namespace de la ontología</param>
        /// <param name="pOntologia">Ontología del documento</param>
        public byte[] ObtenerRDFDeVirtuoso(Guid pDocumentoID, string pNombreGrafo, string pUrlIntragnoss, string pUrlOntologia, string pNamespaceOntologia, Ontologia pOntologia, bool pUsarAfinidad = false)
        {
            return ObtenerRDFDeVirtuoso(pDocumentoID, pNombreGrafo, pUrlIntragnoss, pUrlOntologia, pNamespaceOntologia, pOntologia, null, false, pUsarAfinidad);
        }

        /// <summary>
        /// Obtiene los bytes del fichero RDF de un documento semántico almacenado en virtuoso.
        /// </summary>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <param name="pNombreGrafo">Nombre del grafo en virtuoso</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        /// <param name="pUrlOntologia">URL de la ontología</param>
        /// <param name="pNamespaceOntologia">Namespace de la ontología</param>
        /// <param name="pOntologia">Ontología del documento</param>
        public byte[] ObtenerRDFDeVirtuoso(Guid pDocumentoID, string pNombreGrafo, string pUrlIntragnoss, string pUrlOntologia, string pNamespaceOntologia, Ontologia pOntologia, string pFicheroConfiguracion, bool pTraerEntidadesExternas, bool pUsarAfinidad = false)
        {
            return ObtenerRDFDeVirtuoso(pDocumentoID.ToString(), pNombreGrafo, pUrlIntragnoss, pUrlOntologia, pNamespaceOntologia, pOntologia, pFicheroConfiguracion, pTraerEntidadesExternas, pUsarAfinidad);
        }

        /// <summary>
        /// Obtiene los bytes del fichero RDF de un documento semántico almacenado en virtuoso.
        /// </summary>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <param name="pNombreGrafo">Nombre del grafo en virtuoso</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        /// <param name="pUrlOntologia">URL de la ontología</param>
        /// <param name="pNamespaceOntologia">Namespace de la ontología</param>
        /// <param name="pOntologia">Ontología del documento</param>
        public byte[] ObtenerRDFDeVirtuoso(string pDocumentoID, string pNombreGrafo, string pUrlIntragnoss, string pUrlOntologia, string pNamespaceOntologia, Ontologia pOntologia, string pFicheroConfiguracion, bool pTraerEntidadesExternas, bool pUsarAfinidad = false)
        {
            FacetadoCN facetadoCN = null;

            if (string.IsNullOrEmpty(pFicheroConfiguracion))
            {
                facetadoCN = new FacetadoCN(pUrlIntragnoss, "", mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            }
            else
            {
                facetadoCN = new FacetadoCN(pFicheroConfiguracion, pUrlIntragnoss, "", mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            }

            FacetadoDS facetadoDS = facetadoCN.ObtenerRDFXMLdeFormulario(pNombreGrafo.ToLower(), pDocumentoID, pUsarAfinidad);

            if (facetadoDS.Tables[0].Rows.Count == 0)
            {
                return Array.Empty<byte>();
            }

            List<FacetaEntidadesExternas> EntExt = null;
            if (pTraerEntidadesExternas)
            {
                //Obtenemos el Proyecto al que pertenece el documento
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                Guid proyID = docCN.ObtenerProyectoIDPorDocumentoID(new Guid(pDocumentoID));

                DataWrapperFacetas facetasDW = new DataWrapperFacetas();
                FacetaCN facCN = new FacetaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                facCN.CargarFacetasEntidadesExternas(ProyectoAD.MetaOrganizacion, proyID, facetasDW);
                EntExt = facetasDW.ListaFacetaEntidadesExternas.Where(item => item.ProyectoID.Equals(proyID)).ToList();

            }
            List<string> listaEntidadesExternas = new List<string>();

            MemoryStore store = new MemoryStore();
            //foreach (DataRow fila in facetadoDS.Tables[0].Rows)

            string[] delimiter = { "/@/" };
            string type = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type";
            string label = "http://www.w3.org/2000/01/rdf-schema#label";
            Dictionary<string, string> dicSujetosTypeLabel = ObtenerDiccionarioSujetosTypeLabel(facetadoDS.Tables[0], delimiter, type, label);

            foreach (DataRow fila in facetadoDS.Tables[0].Select("", "s"))
            {
                AgregarDatosAlStore(fila, store, dicSujetosTypeLabel, delimiter, type, label);

                //Obtener las entidades externas del proyecto
                if (EntExt != null && Uri.IsWellFormedUriString(fila["o"].ToString(), UriKind.Absolute) && !listaEntidadesExternas.Contains(fila["o"].ToString()) && facetadoDS.Tables[0].Select("s = '" + fila["o"].ToString().Replace("'", "''") + "'").Length == 0)
                {
                    listaEntidadesExternas.Add(fila["o"].ToString());
                }
            }

            if (pTraerEntidadesExternas)
            {
                FacetadoDS facDS = new FacetadoDS();
                foreach (string entidadExterna in listaEntidadesExternas)
                {
                    //Por cada entidad externa del diccionario, debemos obtener sus triples de virtuoso y añadirlos al store.
                    if (EntExt != null)
                    {
                        for (int i = 0; i < EntExt.Count; i++)
                        {
                            if (entidadExterna.ToLower().Contains(EntExt[i].EntidadID.ToLower()))
                            {
                                //Cargamos el DS
                                facDS.Merge(facetadoCN.ObtieneTripletasOtrasEntidadesDS(entidadExterna, EntExt[i].Grafo, EntExt));
                            }
                        }
                    }
                }

                //Agrupar por sujeto
                //Por cada bloque, agregar al store primero el type y después el label
                //Rezar para que el store no desordene las triples....

                Dictionary<string, string> dicSujetosTypeLabelEntidades = ObtenerDiccionarioSujetosTypeLabel(facDS.Tables["OtrasEntidades"], delimiter, type, label);
                foreach (DataRow fila in facDS.Tables["OtrasEntidades"].Select("", "s"))
                {
                    AgregarDatosAlStore(fila, store, dicSujetosTypeLabelEntidades, delimiter, type, label);
                }
            }

            facetadoDS.Dispose();

            MemoryStream archivo = new MemoryStream();
            System.Xml.XmlWriter textWriter = System.Xml.XmlWriter.Create(archivo);

            RdfWriter writer = new RdfXmlWriter(textWriter);
            writer.Namespaces.AddNamespace("http://www.w3.org/1999/02/22-rdf-syntax-ns#", "rdf");
            writer.Namespaces.AddNamespace("http://www.gnoss.net/ontologia.owl#", "gnoss");
            writer.Namespaces.AddNamespace("http://www.w3.org/2001/XMLSchema#", "xsd");
            writer.Namespaces.AddNamespace("http://www.w3.org/2000/01/rdf-schema#", "rdfs");
            writer.Namespaces.AddNamespace("http://www.w3.org/2002/07/owl#", "owl");
            writer.Namespaces.AddNamespace(pUrlOntologia, pNamespaceOntologia);

            if (pOntologia != null)
            {
                foreach (string ns in pOntologia.NamespacesDefinidos.Keys)
                {
                    if (pOntologia.NamespacesDefinidos[ns] != "rdf" && pOntologia.NamespacesDefinidos[ns] != "gnoss" && pOntologia.NamespacesDefinidos[ns] != "xsd" && pOntologia.NamespacesDefinidos[ns] != "rdfs" && pOntologia.NamespacesDefinidos[ns] != "owl" && pOntologia.NamespacesDefinidos[ns] != pNamespaceOntologia)
                    {
                        writer.Namespaces.AddNamespace(ns, pOntologia.NamespacesDefinidos[ns]);
                    }
                }
            }

            writer.Write(store);
            writer.Close();
            store.Dispose();

            return archivo.ToArray();
        }

        public List<ElementoOntologia> ObtenerEntidadesTesauroSemantico(string pGrafo, string pSource, string pUrlIntragnoss, Guid pProyectoID, string pBaseURLFormulariosSem, string pIdiomaUsuario)
        {
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Guid ontologiaID = docCN.ObtenerOntologiaAPartirNombre(pProyectoID, pGrafo);
            string nombreOntologia = docCN.ObtenerEnlaceDocumentoPorDocumentoID(ontologiaID);

            string urlOntologia = pBaseURLFormulariosSem + "/Ontologia/" + nombreOntologia + "#";

            //byte[] arrayOnto = ControladorDocumentacion.ObtenerOntologia(ontologiaID, pProyectoID);
            Dictionary<string, List<EstiloPlantilla>> listaEstilos = new Dictionary<string, List<EstiloPlantilla>>();
            byte[] arrayOnto = ObtenerOntologia(ontologiaID, out listaEstilos, pProyectoID, null, null, false);
            Ontologia ontologia = new Ontologia(arrayOnto, true);
            ontologia.LeerOntologia();
            ontologia.EstilosPlantilla = listaEstilos;
            ontologia.IdiomaUsuario = pIdiomaUsuario;
            ontologia.OntologiaID = ontologiaID;

            MemoryStream buffer = new MemoryStream(ObtenerRDFTesauroSemanticoDeVirtuoso(pGrafo, pSource, pUrlIntragnoss, ontologia, urlOntologia));

            StreamReader reader = new StreamReader(buffer);
            string rdfTexto = reader.ReadToEnd();
            reader.Close();
            reader.Dispose();

            GestionOWL gestorOWL = new GestionOWL();
            gestorOWL.UrlOntologia = urlOntologia;
            gestorOWL.NamespaceOntologia = "tessem";

            List<ElementoOntologia> instanciasPrincipales = gestorOWL.LeerFicheroRDF(ontologia, rdfTexto, true);

            return instanciasPrincipales;
        }

        /// <summary>
        /// Obtiene los bytes del fichero RDF de un documento semántico almacenado en virtuoso.
        /// </summary>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <param name="pNombreGrafo">Nombre del grafo en virtuoso</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        /// <param name="pUrlOntologia">URL de la ontología</param>
        /// <param name="pNamespaceOntologia">Namespace de la ontología</param>
        /// <param name="pOntologia">Ontología del documento</param>
        public byte[] ObtenerRDFTesauroSemanticoDeVirtuoso(string pGrafo, string pSource, string pUrlIntragnoss, Ontologia pOntologia, string pUrlOntologia)
        {
            string urlOnto = pUrlOntologia;
            string namesOnto = "tessem";
            FacetadoCN facetadoCN = new FacetadoCN(pUrlIntragnoss, "", mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            FacetadoDS facetadoDS = facetadoCN.ObtenerTesauroSemantico(pGrafo, pSource);

            if (facetadoDS.Tables[0].Rows.Count == 0)
            {
                return null;
            }

            MemoryStore store = new MemoryStore();

            string[] delimiter = { "/@/" };
            string type = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type";
            string label = "http://www.w3.org/2000/01/rdf-schema#label";
            Dictionary<string, string> dicSujetosTypeLabel = ObtenerDiccionarioSujetosTypeLabel(facetadoDS.Tables[0], delimiter, type, label);

            foreach (DataRow fila in facetadoDS.Tables[0].Select("", "s"))
            {
                AgregarDatosAlStore(fila, store, dicSujetosTypeLabel, delimiter, type, label);
            }

            facetadoDS.Dispose();

            MemoryStream archivo = new MemoryStream();
            System.Xml.XmlWriter textWriter = System.Xml.XmlWriter.Create(archivo);

            RdfWriter writer = new RdfXmlWriter(textWriter);
            writer.Namespaces.AddNamespace("http://www.w3.org/1999/02/22-rdf-syntax-ns#", "rdf");
            writer.Namespaces.AddNamespace("http://www.gnoss.net/ontologia.owl#", "gnoss");
            writer.Namespaces.AddNamespace("http://www.w3.org/2001/XMLSchema#", "xsd");
            writer.Namespaces.AddNamespace("http://www.w3.org/2000/01/rdf-schema#", "rdfs");
            writer.Namespaces.AddNamespace("http://www.w3.org/2002/07/owl#", "owl");
            writer.Namespaces.AddNamespace(urlOnto, namesOnto);

            if (pOntologia != null)
            {
                foreach (string ns in pOntologia.NamespacesDefinidos.Keys)
                {
                    if (pOntologia.NamespacesDefinidos[ns] != "rdf" && pOntologia.NamespacesDefinidos[ns] != "gnoss" && pOntologia.NamespacesDefinidos[ns] != "xsd" && pOntologia.NamespacesDefinidos[ns] != "rdfs" && pOntologia.NamespacesDefinidos[ns] != "owl" && pOntologia.NamespacesDefinidos[ns] != namesOnto)
                    {
                        writer.Namespaces.AddNamespace(ns, pOntologia.NamespacesDefinidos[ns]);
                    }
                }
            }

            writer.Write(store);
            writer.Close();
            store.Dispose();


            return archivo.ToArray();
        }

        private static void AgregarDatosAlStore(DataRow pFila, MemoryStore pStore, Dictionary<string, string> pDicSujetosTypeLabel, string[] pDelimiter, string pType, string pLabel)
        {
            string sujeto = (string)pFila[0];
            if (pDicSujetosTypeLabel.ContainsKey(sujeto) && !string.IsNullOrEmpty(pDicSujetosTypeLabel[sujeto]))
            {
                string[] delimiter = { "|" };

                string[] typeYLabel = pDicSujetosTypeLabel[sujeto].Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                foreach (string triples in typeYLabel)
                {
                    string[] predYObjt = triples.Split(pDelimiter, StringSplitOptions.RemoveEmptyEntries);
                    string pred = predYObjt[0];
                    string objt = predYObjt[1];
                    //Agregamos al store 
                    pStore.Add(new Statement(new Entity(sujeto), new Entity(pred), new SemWeb.Literal(objt, null, null)));
                }

                //Borramos para no agregarlo en la siguiente llamada a este método.
                pDicSujetosTypeLabel[sujeto] = "";
            }

            string idioma = null;

            if (pFila.ItemArray.Length > 3 && !pFila.IsNull(3) && !string.IsNullOrEmpty((string)pFila[3]))
            {
                idioma = (string)pFila[3];
            }

            if (pDicSujetosTypeLabel.ContainsKey(sujeto) && !((string)pFila[1] == pType || (string)pFila[1] == pLabel))
            {
                string objeto = "";
                if (!pFila.IsNull(2))
                {
                    objeto = (string)pFila[2];
                }

                pStore.Add(new Statement(new Entity(sujeto), new Entity((string)pFila[1]), new SemWeb.Literal(objeto, idioma, null)));
            }
        }

        private static Dictionary<string, string> ObtenerDiccionarioSujetosTypeLabel(DataTable pDataTable, string[] pDelimiter, string pType, string pLabel)
        {
            Dictionary<string, string> dicSujetosTypeLabel = new Dictionary<string, string>();

            foreach (DataRow fila in pDataTable.Select("", "s"))
            {
                string sujeto = (string)fila[0];
                string predicado = (string)fila[1];
                string objeto = "";

                if (!fila.IsNull(2))
                {
                    objeto = (string)fila[2];
                }

                string objetoYPredicado = predicado + pDelimiter[0] + objeto;

                if (predicado.Equals(pType) || predicado.Equals(pLabel))
                {
                    if (dicSujetosTypeLabel.ContainsKey(sujeto))
                    {
                        if (dicSujetosTypeLabel[sujeto].Contains(pType))
                        {
                            dicSujetosTypeLabel[sujeto] += "|" + objetoYPredicado;
                        }
                        else if (dicSujetosTypeLabel[sujeto].Contains(pLabel))
                        {
                            dicSujetosTypeLabel[sujeto] = objetoYPredicado + "|" + dicSujetosTypeLabel[sujeto];
                        }
                    }
                    else
                    {
                        dicSujetosTypeLabel.Add(sujeto, objetoYPredicado);
                    }
                }
            }

            return dicSujetosTypeLabel;
        }

        /// <summary>
        /// Obtiene la ruta del fichero RDF de un documento semántico almacenado en virtuoso.
        /// </summary>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <param name="pNombreGrafo">Nombre del grafo en virtuoso</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        public void BorrarRDFDeVirtuoso(Guid pDocumentoID, string pNombreGrafo, string pUrlIntragnoss, bool pUsarColareplicacion, Guid pProyectoID)
        {
            BorrarRDFDeVirtuoso(pDocumentoID, pNombreGrafo, pUrlIntragnoss, "", pUsarColareplicacion, pProyectoID);
        }

        /// <summary>
        /// Obtiene la ruta del fichero RDF de un documento semántico almacenado en virtuoso.
        /// </summary>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <param name="pNombreGrafo">Nombre del grafo en virtuoso</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        /// <param name="pFicheroConfiguracion">Fichero de configuración de la BD</param>
        public void BorrarRDFDeVirtuoso(Guid pDocumentoID, string pNombreGrafo, string pUrlIntragnoss, string pFicheroConfiguracion, bool pUsarColareplicacion, Guid pProyectoID)
        {
            BorrarRDFDeVirtuoso(pDocumentoID.ToString(), pNombreGrafo, pUrlIntragnoss, pFicheroConfiguracion, pProyectoID, pUsarColareplicacion);
        }

        /// <summary>
        /// Obtiene la ruta del fichero RDF de un documento semántico almacenado en virtuoso.
        /// </summary>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <param name="pNombreGrafo">Nombre del grafo en virtuoso</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        /// <param name="pFicheroConfiguracion">Fichero de configuración de la BD</param>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <param name="pInfoExtraReplicacion">Info extra para la replicación</param>
        public void BorrarRDFDeVirtuoso(string pDocumentoID, string pNombreGrafo, string pUrlIntragnoss, string pFicheroConfiguracion, Guid pProyectoID, bool pUsarColareplicacion)
        {
            BorrarRDFDeVirtuoso(pDocumentoID, pNombreGrafo, pUrlIntragnoss, pFicheroConfiguracion, pProyectoID, null, pUsarColareplicacion);
        }

        /// <summary>
        /// Obtiene la ruta del fichero RDF de un documento semántico almacenado en virtuoso.
        /// </summary>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <param name="pNombreGrafo">Nombre del grafo en virtuoso</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        /// <param name="pFicheroConfiguracion">Fichero de configuración de la BD</param>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <param name="pInfoExtraReplicacion">Info extra para la replicación</param>
        public void BorrarRDFDeVirtuoso(string pDocumentoID, string pNombreGrafo, string pUrlIntragnoss, string pFicheroConfiguracion, Guid pProyectoID, string pInfoExtraReplicacion, bool pUsarColareplicacion)
        {
            FacetadoCN facetadoCN = null;
            if (pFicheroConfiguracion == "")
            {
                facetadoCN = new FacetadoCN(pUrlIntragnoss, pProyectoID.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            }
            else
            {
                facetadoCN = new FacetadoCN(pFicheroConfiguracion, pUrlIntragnoss, pProyectoID.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            }

            facetadoCN.BorrarTripletasFormularioSemantico(pNombreGrafo.ToLower(), pDocumentoID, !pUsarColareplicacion, pInfoExtraReplicacion);
        }

        /// <summary>
        /// Obtiene la ruta del fichero RDF de un documento semántico almacenado en virtuoso.
        /// </summary>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <param name="pNombreGrafo">Nombre del grafo en virtuoso</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        /// <param name="pFicheroConfiguracion">Fichero de configuración de la BD</param>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <param name="pInfoExtraReplicacion">Info extra para la replicación</param>
        public void BorrarRDFDeVirtuosoPorGrafo(string pDocumentoID, string pUrlIntragnoss, Guid pProyectoID, string pInfoExtraReplicacion, bool pUsarColareplicacion)
        {
            FacetadoCN facetadoCN = null;

            facetadoCN = new FacetadoCN(pUrlIntragnoss, pProyectoID.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

            string nombreGrafo = facetadoCN.ObtenerGrafoOntologiaRecurso(pDocumentoID);
            facetadoCN.BorrarTripletasFormularioSemantico(nombreGrafo.ToLower(), pDocumentoID, !pUsarColareplicacion, pInfoExtraReplicacion);
        }

        /// <summary>
        /// Obtiene la ruta del fichero RDF de un documento semántico almacenado en virtuoso.
        /// </summary>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <param name="pNombreGrafo">Nombre del grafo en virtuoso</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        /// <param name="pFicheroConfiguracion">Fichero de configuración de la BD</param>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <param name="pInfoExtraReplicacion">Info extra para la replicación</param>
        public void BorrarRDFDeVirtuosoPorGrafo(string pDocumentoID, string pUrlIntragnoss, string pFicheroConfiguracion, Guid pProyectoID, string pInfoExtraReplicacion, bool pUsarColareplicacion)
        {
            FacetadoCN facetadoCN = null;
            if (pFicheroConfiguracion == "")
            {
                facetadoCN = new FacetadoCN(pUrlIntragnoss, pProyectoID.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            }
            else
            {
                facetadoCN = new FacetadoCN(pFicheroConfiguracion, pUrlIntragnoss, pProyectoID.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            }
            string nombreGrafo = facetadoCN.ObtenerGrafoOntologiaRecurso(pDocumentoID);
            facetadoCN.BorrarTripletasFormularioSemantico(nombreGrafo.ToLower(), pDocumentoID, !pUsarColareplicacion, pInfoExtraReplicacion);
        }

        /// <summary>
        /// Borrar el RDF de virtuoso de un documento que ha sido eliminado.
        /// </summary>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <param name="pUrlIntraGnoss">URL de intragnoss</param>
        public void BorrarRDFDeDocumentoEliminado(Guid pDocumentoID, Guid pOntologiaID, string pUrlIntraGnoss, bool pUsarColareplicacion, Guid pProyectoID)
        {
            BorrarRDFDeDocumentoEliminado(pDocumentoID, pOntologiaID, pUrlIntraGnoss, null, pUsarColareplicacion, pProyectoID);
        }

        /// <summary>
        /// Borrar el RDF de virtuoso de un documento que ha sido eliminado.
        /// </summary>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <param name="pUrlIntraGnoss">URL de intragnoss</param>
        public void BorrarRDFDeDocumentoEliminado(Guid pDocumentoID, Guid pOntologiaID, string pUrlIntraGnoss, string pInfoExtraReplicacion, bool pUsarColareplicacion, Guid pProyectoID)
        {
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            string enlace = docCN.ObtenerEnlaceDocumentoPorDocumentoID(pOntologiaID);

            BorrarRDFDeVirtuoso(pDocumentoID.ToString(), enlace, pUrlIntraGnoss, "", pProyectoID, pInfoExtraReplicacion, pUsarColareplicacion);
        }

        public void GuardarRecursoEnGrafoBusqueda(Documento pDocumento, Proyecto pProyecto, List<TripleWrapper> pListaTriplesSemanticos, Ontologia pOntologia, GestionTesauro pGestorTesauro, string pUrlIntragnoss, bool pCrearVersion, Guid? pDocumentoOriginaldId, PrioridadBase pPrioridadBase, StringBuilder pSbVirtuoso = null)
        {
            try
            {
                string rdfConfiguradoRecursoNoSemantico = "";
                if (!pDocumento.TipoDocumentacion.Equals(TiposDocumentacion.Semantico))
                {
                    rdfConfiguradoRecursoNoSemantico = ObtenerRdfRecursoNoSemantico(pProyecto.Clave, (short)pDocumento.TipoDocumentacion);
                }

                new UtilidadesVirtuoso(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).GuardarRecursoEnGrafoBusqueda(pDocumento, true, null, pProyecto, pListaTriplesSemanticos, pOntologia, pGestorTesauro, rdfConfiguradoRecursoNoSemantico, pCrearVersion, pDocumentoOriginaldId, pUrlIntragnoss, "", pPrioridadBase, pSbVirtuoso);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
            }
        }

        #region Grafos Gráficos

        /// <summary>
        /// Obtiene el nombre de una propiedad del grafo grafico.
        /// </summary>
        /// <param name="pOntologiaID">ID de ontologia</param>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <param name="pPropConexion">Nombre propiedad</param>
        /// <param name="pTipoEntidad">Tipo de entidad</param>
        /// <param name="pSubTiposEntidad">SubTipos de entidad</param>
        /// <param name="pIdioma">Idioma</param>
        /// <returns>Nombre de una propiedad del grafo grafico</returns>
        public string ObtenerNombrePropiedadGrafoGraficoOntologia(Guid pOntologiaID, Guid pProyectoID, string pPropConexion, string pTipoEntidad, string pSubTiposEntidad, string pIdioma)
        {
            string nombreOntologia = pTipoEntidad + ".owl";
            string extraPeticion = null;

            if (pOntologiaID == Guid.Empty)
            {
                extraPeticion = "|";

                DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                DataWrapperDocumentacion dataWrapperDocumentacion = docCL.ObtenerOntologiasProyecto(pProyectoID, false);

                foreach (AD.EntityModel.Models.Documentacion.Documento filaDoc in dataWrapperDocumentacion.ListaDocumento)
                {
                    extraPeticion += filaDoc.Enlace.Substring(0, filaDoc.Enlace.LastIndexOf(".")) + "," + filaDoc.DocumentoID + "|";

                    if (filaDoc.Enlace == nombreOntologia)
                    {
                        pOntologiaID = filaDoc.DocumentoID;
                    }
                }

                extraPeticion += "[|Extra|]";

                dataWrapperDocumentacion = null;
            }

            if (pOntologiaID != Guid.Empty)
            {
                Dictionary<string, string> namespacesExtra = ObtenerNamespacesConfigComunidad(pProyectoID);
                Dictionary<string, List<EstiloPlantilla>> listaEstilosEspefEntidad = null;

                if (!string.IsNullOrEmpty(pSubTiposEntidad))
                {
                    foreach (string subTipo in pSubTiposEntidad.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string subTipoConNamesPace = null;

                        foreach (string namesp in namespacesExtra.Keys)
                        {
                            if (subTipo.StartsWith(namespacesExtra[namesp]))
                            {
                                subTipoConNamesPace = subTipo.Replace(namespacesExtra[namesp], namesp + ":");
                                break;
                            }
                        }

                        if (subTipoConNamesPace != null)
                        {
                            CargarEstilosOntologiaFraccionadoSegunEntidad(pOntologiaID, nombreOntologia, subTipoConNamesPace, out listaEstilosEspefEntidad, pProyectoID, null);

                            if (listaEstilosEspefEntidad != null && listaEstilosEspefEntidad.ContainsKey(pPropConexion) && listaEstilosEspefEntidad[pPropConexion][0] is EstiloPlantillaEspecifProp)
                            {
                                EstiloPlantillaEspecifProp estiloProp = (EstiloPlantillaEspecifProp)listaEstilosEspefEntidad[pPropConexion][0];

                                if (estiloProp.AtrNombreLectura.ContainsKey(pIdioma))
                                {
                                    return extraPeticion + estiloProp.AtrNombreLectura[pIdioma];
                                }
                                else
                                {
                                    return extraPeticion + estiloProp.AtrNombreLectura.Values.First();
                                }
                            }
                        }
                    }
                }

                CargarEstilosOntologia(pOntologiaID, out listaEstilosEspefEntidad, pProyectoID);

                if (listaEstilosEspefEntidad != null && listaEstilosEspefEntidad.ContainsKey(pPropConexion) && listaEstilosEspefEntidad[pPropConexion][0] is EstiloPlantillaEspecifProp)
                {
                    EstiloPlantillaEspecifProp estiloProp = (EstiloPlantillaEspecifProp)listaEstilosEspefEntidad[pPropConexion][0];

                    if (estiloProp.AtrNombreLectura.ContainsKey(pIdioma))
                    {
                        return extraPeticion + estiloProp.AtrNombreLectura[pIdioma];
                    }
                    else
                    {
                        return extraPeticion + estiloProp.AtrNombreLectura.Values.First();
                    }
                }
                else if (pPropConexion.ToLower().Contains("gnossedu/relatedto"))
                {
                    UtilIdiomas utilIdiomas = UtilIdiomas;
                    return extraPeticion + utilIdiomas.GetText("COMMON", "RELATEDTO");
                }
            }

            return extraPeticion;
        }

        /// <summary>
        /// Obtiene la lista de namespaces extra configurados en la comunidad.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto en el que está buscando el usuario</param>
        /// <returns>Lista de namespaces extra configurados en la comunidad</returns>
        public Dictionary<string, string> ObtenerNamespacesConfigComunidad(Guid pProyectoID)
        {
            Dictionary<string, string> informacionOntologias = new Dictionary<string, string>();

            FacetaCL facetaCL = new FacetaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);

            List<OntologiaProyecto> listaOntologias = facetaCL.ObtenerOntologiasProyecto(Guid.Empty, pProyectoID);
            foreach (OntologiaProyecto myrow in listaOntologias)
            {
                if (myrow.NamespacesExtra != null && !string.IsNullOrEmpty(myrow.NamespacesExtra))
                {
                    foreach (string namesUrl in myrow.NamespacesExtra.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string names = namesUrl.Substring(0, namesUrl.IndexOf(":"));
                        string urlnam = namesUrl.Substring(namesUrl.IndexOf(":") + 1);

                        if (!informacionOntologias.ContainsKey(names))
                        {
                            informacionOntologias.Add(names, urlnam);
                        }
                    }
                }
            }

            listaOntologias = null;
            facetaCL.Dispose();
            return informacionOntologias;

        }

        #endregion

        #region Servicios Externos

        /// <summary>
        /// Obtiene la URL definitiva de un servicio externo del SEMCMS.
        /// </summary>
        /// <param name="pUrlServicio">Url del servicio externo</param>
        /// <param name="pProyectoID">ID del proyecto actual</param>
        /// <returns>URL definitiva de un servicio externo del SEMCMS</returns>
        public string ObtenerUrlServicioExternoSEMCMS(string pUrlServicio, Guid pProyectoID)
        {
            if (pUrlServicio.Contains("@pref@"))
            {
                string[] prefijoUrl = pUrlServicio.Split(new string[] { "@pref@" }, StringSplitOptions.None);

                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                string urlServicio = proyCN.ObtenerUrlServicioExterno(pProyectoID, prefijoUrl[0]);

                if (string.IsNullOrEmpty(urlServicio))
                {
                    throw new Exception("Error URL servicio externo SEMCMS, no existe el prefijo '" + prefijoUrl[0] + "'.");
                }

                if (urlServicio.EndsWith("/") && prefijoUrl[1].StartsWith("/"))
                {
                    urlServicio = urlServicio.Substring(0, urlServicio.Length - 1);
                }

                urlServicio += prefijoUrl[1];
                return urlServicio;
            }
            else
            {
                return pUrlServicio;
            }
        }

        #endregion

        #endregion

        #region CargasInciales

        /// <summary>
        /// Carga la base de recursos
        /// </summary>
        /// <param name="pGestorDocumental">Gestor de documentos</param>
        /// <param name="pIdentidadOrganizacion">Identidad de organización</param>
        /// <param name="pBaseRecursosComunidad">TRUE si carga la BR de la comunidad</param>
        /// <returns>Gestor de documentos</returns>
        public GestorDocumental CargaIncialDeBaseRecursos(GestorDocumental pGestorDocumental, Identidad pIdentidadOrganizacion, bool pBaseRecursosComunidad, Guid pProyectoID, Guid pUsuarioID, Guid pOrganizacionID)
        {
            bool hayQueRecargar = false;

            if (pGestorDocumental == null || pGestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosUsuario.Count == 0 && pGestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosOrganizacion.Count == 0 && pGestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosProyecto.Count == 0)
            {
                hayQueRecargar = true;
            }
            else if (pIdentidadOrganizacion != null && pGestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosOrganizacion.Count == 0)
            {
                hayQueRecargar = true;
            }
            else if (pBaseRecursosComunidad && pGestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosProyecto.Count == 0)
            {
                hayQueRecargar = true;
            }
            else if (pIdentidadOrganizacion == null && !pBaseRecursosComunidad && pGestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosUsuario.Count == 0)
            {
                hayQueRecargar = true;
            }
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<Guid> listaIdentidadesURLSem = new List<Guid>();

            if (hayQueRecargar)
            {
                pGestorDocumental = new GestorDocumental(new DataWrapperDocumentacion(), mLoggingService, mEntityContext);

                if (pIdentidadOrganizacion == null)
                {
                    if (pProyectoID == ProyectoAD.MetaProyecto)
                    {
                        docCN.ObtenerBaseRecursosUsuario(pGestorDocumental.DataWrapperDocumentacion, pUsuarioID);
                    }
                    else
                    {
                        docCL.ObtenerBaseRecursosProyecto(pGestorDocumental.DataWrapperDocumentacion, pProyectoID, pOrganizacionID, pUsuarioID);
                    }
                }
                else
                {
                    docCL.ObtenerBaseRecursosOrganizacion(pGestorDocumental.DataWrapperDocumentacion, (Guid)UtilReflection.GetValueReflection(pIdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID"), pProyectoID);
                }
                Es.Riam.Gnoss.Logica.Tesauro.TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                if (pIdentidadOrganizacion == null && !pBaseRecursosComunidad)
                {
                    pGestorDocumental.GestorTesauro = new GestionTesauro(tesauroCN.ObtenerTesauroUsuario(pUsuarioID), mLoggingService, mEntityContext);
                }
                else if (pBaseRecursosComunidad)
                {
                    TesauroCL tesauroCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    pGestorDocumental.GestorTesauro = new GestionTesauro(tesauroCL.ObtenerTesauroDeProyecto(pProyectoID), mLoggingService, mEntityContext);
                    pGestorDocumental.GestorTesauro.TesauroDW.Merge(tesauroCL.ObtenerCategoriasPermitidasPorTipoRecurso(pGestorDocumental.GestorTesauro.TesauroActualID, pProyectoID));
                    tesauroCL.Dispose();
                }
                else
                {
                    pGestorDocumental.GestorTesauro = new GestionTesauro(tesauroCN.ObtenerTesauroOrganizacion((Guid)UtilReflection.GetValueReflection(pIdentidadOrganizacion.PerfilUsuario.FilaRelacionPerfil, "OrganizacionID")), mLoggingService, mEntityContext);
                }
                pGestorDocumental.CargarDocumentos(false);
            }

            //Lo devolvemos tal y como estaba si no había que recarga o uno nuevo con los datos cargados:
            return pGestorDocumental;
        }

        #endregion

        #region Comentarios

        /// <summary>
        /// Carga los comentarios públicos realizados en la comunidad pasada por parámetro
        /// </summary>
        /// <param name="pProyectoActual">Identificador del proyecto actual</param>
        public void CargarComentariosPublicosMasComunidadActual(Guid pProyectoActual)
        {
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            docCN.ObtenerComentariosPublicosMasProyectoActualDeDocumentos(GestorDocumental.DataWrapperDocumentacion, pProyectoActual);
        }

        /// <summary>
        /// Pone a 0 el contador de nuevas sucripciones.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public void ResetearContadorNuevosComentarios(Guid pPerfilID)
        {
            LiveCN liveCN = new LiveCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            liveCN.ResetearContadorNuevosComentarios(pPerfilID);
        }

        /// <summary>
        /// Obtiene el total de elementos nuevos de la bandeja de un perfil.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        /// <returns>total de elementos nuevos de la bandeja de un perfil</returns>
        public int ObtenerTotalNuevosElementosBandejaPerfil(Guid pPerfilID)
        {
            int total = 0;

            LiveCN liveCN = new LiveCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            AD.EntityModel.Models.IdentidadDS.ContadorPerfil contadorPerfil = liveCN.ObtenerContadoresPerfil(pPerfilID);

            if (contadorPerfil != null)
            {
                total = contadorPerfil.NumNuevosMensajes + contadorPerfil.NuevosComentarios + contadorPerfil.NuevasInvitaciones + contadorPerfil.NuevasSuscripciones;
            }
            return total;
        }

        /// <summary>
        /// Obtiene el total de elementos nuevos de la bandeja de varios perfiles.
        /// </summary>
        /// <param name="pPerfilesIDs">IDs de los perfiles</param>
        /// <returns>total de elementos nuevos de la bandeja de los perfiles</returns>
        public Dictionary<Guid, int> ObtenerTotalNuevosElementosBandejasPerfiles(List<Guid> pPerfilesIDs)
        {
            Dictionary<Guid, int> totales = new Dictionary<Guid, int>();

            LiveCN liveCN = new LiveCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<AD.EntityModel.Models.IdentidadDS.ContadorPerfil> listaContadorPerfil = liveCN.ObtenerContadoresPerfiles(pPerfilesIDs);

            foreach (Guid perfilID in pPerfilesIDs)
            {
                AD.EntityModel.Models.IdentidadDS.ContadorPerfil fila = listaContadorPerfil.FirstOrDefault(item => item.PerfilID.Equals(perfilID));
                int numTotal = 0;
                if (fila != null)
                {
                    numTotal = fila.NumNuevosMensajes + fila.NuevosComentarios + fila.NuevasInvitaciones + fila.NuevasSuscripciones;
                }

                totales.Add(perfilID, numTotal);
            }
            return totales;
        }

        /// <summary>
        /// Obtiene el texto del botón para bloquear/desbloquear comentarios.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pUtilIdiomas">Util idiomas</param>
        public static string ObtenerTextoBloqDesBloqComentarios(Documento pDocumento, UtilIdiomas pUtilIdiomas)
        {
            if (pDocumento.PermiteComentarios)
            {
                if (pDocumento.TipoDocumentacion == TiposDocumentacion.Pregunta)
                {
                    return pUtilIdiomas.GetText("PERFILBASERECURSOSFICHA", "CERRARPREGUNTA");
                }
                else if (pDocumento.TipoDocumentacion == TiposDocumentacion.Debate)
                {
                    return pUtilIdiomas.GetText("PERFILBASERECURSOSFICHA", "CERRARDEBATE");
                }
                else if (pDocumento.TipoDocumentacion == TiposDocumentacion.Encuesta)
                {
                    return pUtilIdiomas.GetText("PERFILBASERECURSOSFICHA", "CERRARENCUESTA");
                }
                else
                {
                    return pUtilIdiomas.GetText("PERFILBASERECURSOSFICHA", "BLOQUEARCOMENTARIOS");
                }
            }
            else
            {
                if (pDocumento.TipoDocumentacion == TiposDocumentacion.Pregunta)
                {
                    return pUtilIdiomas.GetText("PERFILBASERECURSOSFICHA", "ABRIRPREGUNTA");
                }
                else if (pDocumento.TipoDocumentacion == TiposDocumentacion.Encuesta)
                {
                    return pUtilIdiomas.GetText("PERFILBASERECURSOSFICHA", "ABRIRENCUESTA");
                }
                else if (pDocumento.TipoDocumentacion == TiposDocumentacion.Debate)
                {
                    return pUtilIdiomas.GetText("PERFILBASERECURSOSFICHA", "ABRIRDEBATE");
                }
                else
                {
                    return pUtilIdiomas.GetText("PERFILBASERECURSOSFICHA", "DESBLOQUEARCOMENTARIOS");
                }
            }
        }

        #endregion

        #region Twitter

        /// <summary>
        /// Envia un recurso a la cuenta de twiter de una comunidad.
        /// </summary>
        /// <param name="pDocumento">Recurso que se va a enviar</param>
        /// <param name="pProyecto">Proyecto al que pertenece la cuenta del twitter</param>
        /// <param name="pUrlBaseIdioma">Url base del idioma</param>
        /// <param name="pPage">Página donde se usa el control</param>
        /// <param name="pUtilIdiomas">Util idiomas</param>
        /// <param name="pUrlPerfil">Url del perfil del usuario</param>
        /// <param name="pUrlIntragnoss">Base url</param>
        /// <param name="pRedirigirConCallBack">True si tiene que redirigir con callback</param>
        public void EnviarEnlaceATwitterDeComunidad(Identidad pIdentidadActual, Documento pDocumento, Proyecto pProyecto, string pUrlBaseIdioma, UtilIdiomas pUtilIdiomas)
        {
            List<Proyecto> listaProyectos = new List<Proyecto>();
            listaProyectos.Add(pProyecto);

            Dictionary<Documento, List<Proyecto>> listaDocumentos = new Dictionary<Documento, List<Proyecto>>();
            listaDocumentos.Add(pDocumento, listaProyectos);

            EnviarEnlaceATwitterDeComunidad(pIdentidadActual, listaDocumentos, pUrlBaseIdioma, pUtilIdiomas);
        }


        /// <summary>
        /// Envia un recurso a la cuenta de twiter de una comunidad.
        /// </summary>
        /// <param name="pDocumento">Recurso que se va a enviar</param>
        /// <param name="pListaProyectos">Lista de proyectos que tienen cuenta de twitter y hay que notificar</param>
        /// <param name="pUrlBaseIdioma">Url base del idioma</param>
        /// <param name="pPage">Página donde se usa el control</param>
        /// <param name="pUtilIdiomas">Util idiomas</param>
        /// <param name="pUrlPerfil">Url del perfil del usuario</param>
        /// <param name="pUrlIntragnoss">Base url</param>
        /// <param name="pUrlVuelta">Url a la que debe volver el navegador una vez publicado el twit</param>
        /// <param name="pRedirigirConCallBack">True si tiene que redirigir con callback</param>
        public void EnviarEnlaceATwitterDeComunidad(Identidad pIdentidadActual, Documento pDocumento, List<Proyecto> pListaProyectos, string pUrlBaseIdioma, UtilIdiomas pUtilIdiomas, string pUrlPerfil)
        {
            Dictionary<Documento, List<Proyecto>> listaDocumentos = new Dictionary<Documento, List<Proyecto>>();
            listaDocumentos.Add(pDocumento, pListaProyectos);

            EnviarEnlaceATwitterDeComunidad(pIdentidadActual, listaDocumentos, pUrlBaseIdioma, pUtilIdiomas);
        }

        /// <summary>
        /// Envía un recurso a la cuenta de twiter de una comunidad.
        /// </summary>
        /// <param name="pListaDocumentos">Lista de recurso que se van a enviar</param>
        /// <param name="pListaProyectos">Lista de proyectos que tienen cuenta de twitter y hay que notificar</param>
        /// <param name="pUrlBaseIdioma">Url base del idioma</param>
        /// <param name="pPage">Página donde se usa el control</param>
        /// <param name="pUtilIdiomas">Util idiomas</param>
        /// <param name="pUrlPerfil">Url del perfil del usuario</param>
        /// <param name="pUrlIntragnoss">Base url</param>
        /// <param name="pUrlVuelta">Url a la que debe volver el navegador una vez publicado el twit</param>
        /// <param name="pRedirigirConCallBack">True si tiene que redirigir con callback</param>
        public void EnviarEnlaceATwitterDeComunidad(Identidad pIdentidadActual, Dictionary<Documento, List<Proyecto>> pListaDocumentos, string pUrlBaseIdioma, UtilIdiomas pUtilIdiomas)
        {
            try
            {
                DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();
                ParametroAplicacionCL paramAplicCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);

                string hashTagEntorno = "#GNOSS";
                //if (paramAplicCL.ObtenerParametrosAplicacion().ParametroAplicacion.Select("Parametro = 'HashTagEntorno'").Length > 0)
                List<AD.EntityModel.ParametroAplicacion> busqueda = paramAplicCL.ObtenerParametrosAplicacionPorContext().Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.DuracionCookieUsuario)).ToList();
                if (busqueda.Count > 0)
                {
                    hashTagEntorno = busqueda.First().Valor;
                }

                paramAplicCL.Dispose();

                foreach (Documento documento in pListaDocumentos.Keys)
                {
                    if (!documento.FilaDocumentoWebVinBR.PrivadoEditores)
                    {
                        List<Proyecto> listaProyectos = pListaDocumentos[documento];

                        foreach (Proyecto proyecto in listaProyectos)
                        {
                            if (proyecto.Clave != ProyectoAD.MetaProyecto && proyecto.FilaProyecto.TieneTwitter && !string.IsNullOrEmpty(proyecto.FilaProyecto.TokenTwitter) && !string.IsNullOrEmpty(proyecto.FilaProyecto.TokenSecretoTwitter))
                            {
                                string titulo = System.Text.RegularExpressions.Regex.Replace(HttpUtility.UrlDecode(documento.Titulo), "<.*?>", string.Empty);
                                titulo = UtilCadenas.EliminarHtmlDeTexto(titulo);
                                string hashTag = proyecto.FilaProyecto.TagTwitter.Trim();
                                if (proyecto.FilaProyecto.TagTwitterGnoss)
                                {
                                    //hashTag += " #gnoss";
                                    hashTag += " " + hashTagEntorno;
                                }

                                if (titulo.Length > (108 - hashTag.Length))
                                {
                                    titulo = titulo.Substring(0, (108 - hashTag.Length)) + "...";
                                }
                                titulo += " " + hashTag;

                                string enlaceRecurso = UrlsSemanticas.GetURLBaseRecursosFicha(pUrlBaseIdioma, pUtilIdiomas, proyecto.NombreCorto, "", documento, false);

                                //TODO Alvaro migrar ES.Riam.Gnoss.RedesSociales.
                                /*OAuthTwitter oAuth = new OAuthTwitter();

                                AD.EntityModel.Models.Notificacion.ColaTwitter filaColaTwitter = new AD.EntityModel.Models.Notificacion.ColaTwitter();
                                filaColaTwitter.PerfilID = pIdentidadActual.PerfilID;
                                filaColaTwitter.ConsumerKey = oAuth.ConsumerKey;
                                filaColaTwitter.ConsumerSecret = oAuth.ConsumerSecret;
                                filaColaTwitter.TokenTwitter = proyecto.FilaProyecto.TokenTwitter;
                                filaColaTwitter.TokenSecretoTwitter = proyecto.FilaProyecto.TokenSecretoTwitter;
                                filaColaTwitter.Mensaje = titulo;
                                filaColaTwitter.Enlace = enlaceRecurso;
                                filaColaTwitter.NumIntentos = 0;
                                notificacionDW.ListaColaTwitter.Add(filaColaTwitter);

                                mEntityContext.ColaTwitter.Add(filaColaTwitter);*/
                            }
                        }
                    }
                }

                NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                notificacionCN.ActualizarNotificacion();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Categorías tesauro

        /// <summary>
        /// Actualiza el número de recursos de cada categoría de tesauro, de un determinado tesauro si así se indica.
        /// </summary>
        /// <param name="pTesauroID">Identificador del tesauro del que hay que actualizar las categorías, 
        /// o Guid.Empty si es para todos</param>
        public void ActualizarNumRecCatTesDeTesauro(Guid pTesauroID, Guid pDocumentoID, bool pEliminando, Guid pOrganizacionID, Guid pProyectoID)
        {
            ActualizarNumRecCatTesDeTesauro(pEliminando, pTesauroID, pOrganizacionID, pProyectoID, pDocumentoID, false);
        }

        /// <summary>
        /// Actualiza el número de recursos de cada categoría de tesauro, de un determinado tesauro si así se indica.
        /// </summary>
        /// <param name="pTesauroID">Identificador del tesauro del que hay que actualizar las categorías, 
        /// o Guid.Empty si es para todos</param>
        public void ActualizarNumRecCatTesDeTesauroDeRecursoEliminado(Documento pDocumento, bool pEliminarCacheResultadosYFacetas = true)
        {
            bool actualizadoMyGnoss = false;

            foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaDocVinWeb in pDocumento.FilaDocumento.DocumentoWebVinBaseRecursos)
            {
                if (pDocumento.FilaDocumento.Eliminado || !pDocumento.FilaDocumento.UltimaVersion || filaDocVinWeb.Eliminado) //Si esta eliminado el recursos o descompartido en la BR
                {
                    Guid proyectoActualizarID = Guid.Empty;
                    Guid organizacionActualizarID = Guid.Empty;

                    List<AD.EntityModel.Models.Documentacion.BaseRecursosProyecto> filasBRProy = pDocumento.GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosProyecto.Where(baseRecProy => baseRecProy.BaseRecursosID.Equals(filaDocVinWeb.BaseRecursosID)).ToList();
                    if (filasBRProy.Count > 0) //Si la BR es de proyecto, hay que actualizar las categorías
                    {
                        proyectoActualizarID = filasBRProy[0].ProyectoID;
                        organizacionActualizarID = filasBRProy[0].OrganizacionID;
                    }
                    else if (!actualizadoMyGnoss)
                    {
                        proyectoActualizarID = ProyectoAD.MetaProyecto;
                        organizacionActualizarID = ProyectoAD.MetaOrganizacion;

                        actualizadoMyGnoss = true;
                    }

                    if (proyectoActualizarID != Guid.Empty)
                    {
                        //Hago un controlador diferente, porque el hilo asicrono utiliza miembros de la clase, que no se pueden compartir
                        // entre proyectos:
                        ControladorDocumentacion controDocAuxiliar = new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                        controDocAuxiliar.ActualizarNumRecCatTesDeTesauro(true, Guid.Empty, organizacionActualizarID, proyectoActualizarID, pDocumento.Clave, true);
                    }

                }
            }
        }

        /// <summary>
        /// Actualiza el número de recursos de cada categoría de tesauro, de un determinado tesauro si así se indica.
        /// </summary>
        /// <param name="pTesauroID">Identificador del tesauro del que hay que actualizar las categorías, 
        /// o Guid.Empty si es para todos</param>
        /// <param name="pOrganizacionID">Identificador de la organización para actualizar</param>
        /// <param name="pProyectoID">Identificador del proyecto para actualizar</param>
        public void ActualizarNumRecCatTesDeTesauro(bool pEliminando, Guid pTesauroID, Guid pOrganizacionID, Guid pProyectoID, Guid pDocumentoID, bool pEliminarCacheResultadosYFacetas = true)
        {
            List<Guid> lista = new List<Guid>();
            lista.Add(pDocumentoID);

            ActualizarNumRecCatTesDeTesauro(pEliminando, pTesauroID, pOrganizacionID, pProyectoID, lista, pEliminarCacheResultadosYFacetas);
        }

        /// <summary>
        /// Actualiza el número de recursos de cada categoría de tesauro, de un determinado tesauro si así se indica.
        /// </summary>
        /// <param name="pTesauroID">Identificador del tesauro del que hay que actualizar las categorías, 
        /// o Guid.Empty si es para todos</param>
        /// <param name="pOrganizacionID">Identificador de la organización para actualizar</param>
        /// <param name="pProyectoID">Identificador del proyecto para actualizar</param>
        /// <param name="pDocumentos">Lista de documentos</param>
        public void ActualizarNumRecCatTesDeTesauro(bool pEliminando, Guid pTesauroID, Guid pOrganizacionID, Guid pProyectoID, List<Guid> pDocumentos, bool pEliminarCacheResultadosYFacetas = true)
        {
            this.mEliminadoAsinc = pEliminando;
            this.mOrganizacioActuAsincID = pOrganizacionID;
            this.mProyectoActuAsincID = pProyectoID;
            this.mDocumentosAsincIDs = pDocumentos;

            if (mEliminadoAsinc)
            {
                try
                {
                    foreach (Guid docID in mDocumentosAsincIDs)
                    {
                        FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, pProyectoID.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                        facetadoCN.BorrarRecurso(mProyectoActuAsincID.ToString(), docID);
                        //Documento eliminado

                        DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        TipoBusqueda tipo = TipoBusqueda.Recursos;
                        switch (docCN.ObtenerTipoDocumentoPorDocumentoID(docID))
                        {
                            case TiposDocumentacion.Pregunta:
                                tipo = TipoBusqueda.Preguntas;
                                break;
                            case TiposDocumentacion.Debate:
                                tipo = TipoBusqueda.Debates;
                                break;
                        }

                        #region borramos cache preguntas

                        ParametroAplicacionCL paramAplicCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                        //string urlIntragnoss = (string)paramAplicCL.ObtenerParametrosAplicacion().ParametroAplicacion.Select("Parametro = 'UrlIntragnoss'")[0]["Valor"];
                        string urlIntragnoss = paramAplicCL.ObtenerParametrosAplicacionPorContext().Where(parametro => parametro.Parametro.Equals("UrlIntragnoss")).ToList().First().Valor;
                        string dominio = UtilDominios.ObtenerDominioUrl(new Uri(urlIntragnoss), false);

                        //No sirve invalidarla si antes se ha eliminado el recurso que se haya modificado.
                        if (pEliminarCacheResultadosYFacetas)
                        {
                            FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                            facetadoCL.Dominio = dominio;
                            facetadoCL.InvalidarResultadosYFacetasDeBusquedaEnProyecto(mProyectoActuAsincID, FacetadoAD.TipoBusquedaToString(tipo));
                            facetadoCL.Dispose();
                        }
                        #endregion

                    }
                }
                catch (Exception) { }
            }

            //Thread t = new Thread(new ThreadStart(ActualizarNumRecCatTesDeTesauroAsincronamente));
            //t.SetApartmentState(ApartmentState.STA);
            //t.Start();

            //He tenido que quitar el hilo porque desde la versión MVC no funcionan los hilos
            ActualizarNumRecCatTesDeTesauroAsincronamente();

        }

        /// <summary>
        /// Actualiza asincronamente el número de recursos de cada categoría de tesauro.
        /// </summary>
        public void ActualizarNumRecCatTesDeTesauroAsincronamenteDesdeServicio(bool pEliminando, List<string> pListaTagsViejos, Guid pOrganizacionID, Guid pProyectoID, List<Guid> pDocumentos, PrioridadBase pPrioridadBase)
        {
            this.mEliminadoAsinc = pEliminando;
            this.mPrioridadBaseAsinc = pPrioridadBase;
            this.mListaTagsViejosAsinc = pListaTagsViejos;
            this.mOrganizacioActuAsincID = pOrganizacionID;
            this.mProyectoActuAsincID = pProyectoID;
            this.mDocumentosAsincIDs = pDocumentos;

            ActualizarNumRecCatTesDeTesauroAsincronamente();
        }

        /// <summary>
        /// Actualiza asincronamente el número de recursos de cada categoría de tesauro.
        /// </summary>
        public void ActualizarNumRecCatTesDeTesauroAsincronamente()
        {
            try
            {
                NotificarAgregarRecursosEnComunidad(mDocumentosAsincIDs, mProyectoActuAsincID, mPrioridadBaseAsinc);
            }
            catch (Exception) { }
        }

        #endregion

        #region Modelo BASE

        /// <summary>
        /// Notifica la compartición de uno o más documentos en una comunidad
        /// </summary>
        /// <param name="pDocumentoID">Identificador de los documentos agregados</param>
        public void NotificarAgregarRecursosEnComunidad(List<Guid> pDocumentos, Guid pComunidadID, PrioridadBase pPrioridadBase)
        {
            NotificarAgregarRecursosEnComunidad(pDocumentos, pComunidadID, pPrioridadBase, -1);
        }

        /// <summary>
        /// Notifica la compartición de uno o más documentos en una comunidad
        /// </summary>
        /// <param name="pDocumentoID">Identificador de los documentos agregados</param>
        public void NotificarAgregarRecursosEnComunidad(List<Guid> pDocumentos, Guid pComunidadID, PrioridadBase pPrioridadBase, long pEstadoCargaID)
        {
            BaseRecursosComunidadDS baseRecursosComunidadDS = new BaseRecursosComunidadDS();

            foreach (Guid docID in pDocumentos)
            {
                DocumentacionCN docCN = docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                TiposDocumentacion tipoDoc = docCN.ObtenerTipoDocumentoPorDocumentoID(docID);

                // Bug 9325 - Buscar en el código este bug puesto que si ésto se quita de aquí, en otros sitios no se va a agregar al base ninguna fila (eliminiaciones)
                if (mEliminadoAsinc)
                {
                    //Eliminar el Recurso de Virtuoso.
                    Guid identidadcreador = docCN.ObtenerPublicadorAPartirIDsRecursoYProyecto(ProyectoSeleccionado.Clave, docID);

                    IdentidadCN idenCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    List<Guid> resultado2 = idenCN.ObtenerPerfilyOrganizacionID(identidadcreador);

                    FacetadoCN facetadoCN2 = new FacetadoCN(UrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    facetadoCN2.FacetadoAD.CadenaConexionBase = this.mFicheroConfiguracionBDBase;
                    bool borrarAuxiliar = false;

                    if (tipoDoc == TiposDocumentacion.Semantico)
                    {
                        borrarAuxiliar = true;
                    }

                    if (resultado2.Count > 1 && resultado2[1] != null)
                    {
                        Guid orgID = new Guid(resultado2[1].ToString());
                        facetadoCN2.BorrarRecurso(orgID.ToString(), docID, 0, "", false, borrarAuxiliar);
                    }
                    else if (resultado2.Count > 0)
                    {
                        Guid perfilID = new Guid(resultado2[0].ToString());
                        facetadoCN2.BorrarRecurso(perfilID.ToString(), docID, 0, "", false, borrarAuxiliar);
                    }

                    //Esto si no es un borrador
                    if (!docCN.EsDocumentoBorrador(docID) && !pComunidadID.Equals(ProyectoAD.MetaProyecto))
                    {
                        facetadoCN2.BorrarRecurso(pComunidadID.ToString(), docID, 0, "", false, borrarAuxiliar);
                    }


                    EliminarRecursoModeloBaseSimple(docID, pComunidadID, (short)tipoDoc, baseRecursosComunidadDS);
                }
                else
                {
                    AgregarRecursoModeloBaseSimple(docID, pComunidadID, (short)tipoDoc, baseRecursosComunidadDS, "", pPrioridadBase, false, pEstadoCargaID, null);
                }

            }
        }

        public void GuardarTriplesLectoresEditores(Documento pDocumento, Proyecto pProyecto)
        {
            try
            {
                string triplesEditoresLectores = new UtilidadesVirtuoso(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).GenerarTripletasEditoresLectoresRecurso(pDocumento, pProyecto.Clave);
                string triplesGruposEditoresLectores = new UtilidadesVirtuoso(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).GenerarTripletasGruposEditoresLectoresRecurso(pDocumento);

                string triplesPrivacidad = UtilidadesVirtuoso.GenerarTripletasHasPrivacidadRecurso(pDocumento);

                string grafo = pProyecto.Clave.ToString().ToLower();
                string sujeto = $"http://gnoss/{pDocumento.Clave.ToString().ToUpper()}";
                List<string> listaPredicados = new List<string>() { "http://gnoss/haseditorIdentidadID", "http://gnoss/haseditor", "http://gnoss/hasparticipanteIdentidadID", "http://gnoss/hasparticipante", "http://gnoss/hasgrupoEditor", "http://gnoss/hasgrupoLector", "http://gnoss/hasprivacidadCom" };

                if (triplesEditoresLectores.Length > 0 || triplesGruposEditoresLectores.Length > 0 || triplesPrivacidad.Length > 0)
                {
                    StringBuilder triplesEditores = new StringBuilder();
                    triplesEditores.AppendLine(triplesEditoresLectores);
                    triplesEditores.AppendLine(triplesGruposEditoresLectores);
                    triplesEditores.AppendLine(triplesPrivacidad);

                    FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

                    FacetadoDS facetadoDS = facetadoCN.ObtenerValoresPropiedadesEntidad(grafo, sujeto, listaPredicados);
                    if (facetadoDS.Tables[0].Rows.Count > 0)
                    {
                        facetadoCN.ModificarListaTripletas(grafo, triplesEditores.ToString(), listaPredicados, 11, sujeto);
                    }
                    else
                    {
                        facetadoCN.InsertaTripletas(grafo, triplesEditores.ToString(), 11);
                    }
                }
                else
                {
                    // TODO JUAN: Eliminar
                    FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    facetadoCN.BorrarListaPredicadosDeSujeto(grafo, sujeto, listaPredicados);
                }

                NotificarModificarTagsRecurso(pDocumento.Clave, pProyecto.Clave, PrioridadBase.ApiRecursos);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);

                // Ha fallado la generación de triples, que lo haga el servicio BASE
                NotificarModificarTagsRecurso(pDocumento.Clave, pPrioridad: PrioridadBase.ApiRecursos);
            }
        }

        /// <summary>
        /// Notifica la modificación de los tags de un documento a todas sus comunidades
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento agregado o modificado</param>
        /// <param name="pProyectoID">SOLO para generar el triple search: Identificador del proyecto en el que ya se han generado los triples de búsqueda y sólo se necesita generar el triple search</param>
        public void NotificarModificarTagsRecurso(Guid pDocumentoID, Guid? pProyectoID = null, PrioridadBase pPrioridad = PrioridadBase.Alta)
        {
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursosConProyectoID> listaDocumentoWebVinBaseRecursosConProyectoID = docCN.ObtenerFilasDocumentoWebVinBaseRecDeDocumento(pDocumentoID);
            TiposDocumentacion tipoDoc = docCN.ObtenerTipoDocumentoPorDocumentoID(pDocumentoID);

            BaseRecursosComunidadDS baseRecursosComunidadDS = new BaseRecursosComunidadDS();

            //Calculo los proyectos en los que está el recurso compartido
            foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursosConProyectoID filaDocVinWeb in listaDocumentoWebVinBaseRecursosConProyectoID)
            {
                if (!filaDocVinWeb.Eliminado) //Si no esta descompartido en la BR
                {
                    Guid proyectoID = filaDocVinWeb.ProyectoID;

                    if (proyectoID.Equals(mProyectoActuAsincID) || mActualizarTodosProyectosCompartido)
                    {
                        if ((pProyectoID.HasValue) && proyectoID.Equals(pProyectoID.Value))
                        {
                            AgregarRecursoModeloBaseSimple(pDocumentoID, proyectoID, (short)tipoDoc, baseRecursosComunidadDS, "", pPrioridad, false, -1, (short)TiposElementosEnCola.InsertadoEnGrafoBusquedaDesdeWeb);
                        }
                        else
                        {
                            AgregarRecursoModeloBaseSimple(pDocumentoID, proyectoID, (short)tipoDoc, baseRecursosComunidadDS, pPrioridad);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Notifica la modificación de los tags de un documento a todas sus comunidades insertando los nuevos tags y modificando el campo search
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento agregado o modificado</param>
        public void NotificarModificarTagsSearchRecursoComunidadesCompartido(Documento pDocumento, bool pEliminandoTags, PrioridadBase pPrioridadBase)
        {
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            BaseRecursosComunidadDS baseRecursosComunidadDS = new BaseRecursosComunidadDS();

            //Calculo los proyectos en los que está el recurso compartido
            foreach (Guid proyectoID in pDocumento.ListaProyectos)
            {
                if (proyectoID.Equals(pDocumento.ProyectoID))
                {
                    new UtilidadesVirtuoso(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).GuardarEdicionTagsRecursoEnGrafoBusqueda(pDocumento, proyectoID, UrlIntragnoss, pPrioridadBase, pEliminandoTags);
                }
                else
                {
                    AgregarRecursoModeloBaseSimple(pDocumento.Clave, proyectoID, (short)pDocumento.TipoDocumentacion, baseRecursosComunidadDS, "", pPrioridadBase, (short)TiposElementosEnCola.Agregado);
                }
            }
        }

        /// <summary>
        /// Inserta en el grafo de búsqueda de la comunidad del recurso y, si está compartido, notifica la modificación de los tags de un documento a todas sus comunidades.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento agregado o modificado</param>
        public void NotificarModificarTagsRecurso(Documento pDocumento, Proyecto pProyecto, List<TripleWrapper> pListaTriplesSemanticos, Ontologia pOntologia, GestionTesauro pGestorTesauro, bool pCrearVersion, Documento pDocumentoOriginal, PrioridadBase pPrioridadBase)
        {
            BaseRecursosComunidadDS baseRecursosComunidadDS = new BaseRecursosComunidadDS();

            if (pCrearVersion && pDocumentoOriginal != null)
            {
                //si se está versionando hay que insertar filas en todas las comunidades donde está compartido el recurso
                foreach (Guid proyectoID in pDocumentoOriginal.ListaProyectos)
                {
                    //excepto para la comunidad original porque se borrará al hacer la inserción directa en el grafo de búsqueda más abajo(GuardarRecursoEnGrafoBusqueda)
                    if (!proyectoID.Equals(pDocumentoOriginal.ProyectoID))
                    {
                        EliminarRecursoModeloBaseSimple(pDocumentoOriginal.Clave, proyectoID, pDocumentoOriginal.FilaDocumento.Tipo, null, "acid");
                    }
                }
            }

            foreach (Guid proyectoID in pDocumento.ListaProyectos)
            {
                if (proyectoID.Equals(mProyectoActuAsincID) || mActualizarTodosProyectosCompartido)
                {
                    //si la comunidad es en la que se publicó y coincide con el proyecto pasado como parámetro se inserta directamente, si no, lo hace el BASE
                    if (proyectoID.Equals(pDocumento.ProyectoID) && pProyecto.Clave.Equals(proyectoID))
                    {
                        Guid? documentoOriginalId = null;
                        if (pDocumentoOriginal != null)
                        {
                            documentoOriginalId = pDocumentoOriginal.Clave;
                        }

                        GuardarRecursoEnGrafoBusqueda(pDocumento, pProyecto, pListaTriplesSemanticos, pOntologia, pGestorTesauro, UrlIntragnoss, pCrearVersion, documentoOriginalId, pPrioridadBase);
                    }
                    else
                    {
                        AgregarRecursoModeloBaseSimple(pDocumento.Clave, proyectoID, (short)pDocumento.TipoDocumentacion, baseRecursosComunidadDS, PrioridadBase.Alta);
                    }
                }
            }
        }

        /// <summary>
        /// Verdad si está cerrada
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pComunidadID">Identificador de la comunidad en la que se ha cambiado el estado</param>
        /// <param name="pEnTodasComunidades">Verdad si el estado cambia para todas las comunidades</param>
        /// <param name="pTipoDocumento">Tipo del documento (debate o pregunta)</param>
        public void NotificarCambioEstadoPreguntaDebate(Guid pDocumentoID, Guid pComunidadID, bool pEnTodasComunidades, TiposDocumentacion pTipoDocumento)
        {
            if (pEnTodasComunidades)
            {
                NotificarModificarTagsRecurso(pDocumentoID);
            }
            else
            {
                AgregarRecursoModeloBaseSimple(pDocumentoID, pComunidadID, (short)pTipoDocumento, PrioridadBase.Alta);
            }
        }

        public JsonEstado AccionEnServicioExternoEcosistema(TipoAccionExterna pTipoAccionExterna, GestorParametroAplicacion pParametroAplicacionDS, Guid pProyectoID, Guid pUsuarioID, Guid pDocumentoID, string pTitulo, string pDescripcion, TiposDocumentacion pTipoDocumento)
        {
            if (!pUsuarioID.Equals(UsuarioAD.Invitado))
            {
                if (pParametroAplicacionDS.ListaAccionesExternas != null && pParametroAplicacionDS.ListaAccionesExternas.Where(accion => accion.TipoAccion.Equals((short)pTipoAccionExterna)).ToList().Count == 1)
                {

                    AccionesExternas filaAccionesExrternas = pParametroAplicacionDS.ListaAccionesExternas.Where(acciones => acciones.TipoAccion.Equals((short)pTipoAccionExterna)).ToList()[0];

                    JsonDocumento documento = new JsonDocumento();
                    documento.ProyectoID = pProyectoID;
                    documento.DocumentoID = pDocumentoID;
                    documento.Titulo = pTitulo;
                    documento.Descripcion = pDescripcion;
                    documento.TipoDocumento = pTipoDocumento;

                    //string url = filaAccionesExrternas.URL + "?json=" + HttpUtility.UrlEncode(JsonConvert.SerializeObject(usuario));
                    string url = filaAccionesExrternas.URL;//POST
                    string respuesta = string.Empty;
                    try
                    {
                        respuesta = new UtilWeb(mHttpContextAccessor).WebRequest(UtilWeb.Metodo.POST, url, JsonConvert.SerializeObject(documento), "application/json");//POST
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex, "Error en servicio externo: " + url);
                    }

                    JsonEstado jsonEstado = new JsonEstado();
                    jsonEstado.Correcto = false;
                    jsonEstado.InfoExtra = "";

                    if (!string.IsNullOrEmpty(respuesta))
                    {
                        jsonEstado = JsonConvert.DeserializeObject<JsonEstado>(respuesta);
                    }

                    //log
                    if (!jsonEstado.Correcto)
                    {
                        Exception ex = new Exception(jsonEstado.InfoExtra);
                        mLoggingService.GuardarLogError(ex, "ERROR AccionEnServicioExterno. Respuesta servicio externo : " + respuesta);
                    }

                    return jsonEstado;
                }
            }
            return null;
        }

        #region Privados

        /// <summary>
        /// Notifica al modelo base que se han modificado un documento.
        /// </summary>
        /// <param name="pDocumentoID">ID del documento modificado</param>
        /// <param name="pTipoDoc">Tipo del documento</param>
        /// <param name="pProyectoID">Proyecto en el que se ha modificado</param>
        /// <param name="pPrioridadBase">Prioridad de las filas introducidas en el modelo base</param>
        public void AgregarModificacionRecursoModeloBase(Guid pDocumentoID, short pTipoDoc, Guid pProyectoID, PrioridadBase pPrioridadBase)
        {
            Dictionary<Guid, short> documentos = new Dictionary<Guid, short>();
            documentos.Add(pDocumentoID, pTipoDoc);
            AgregarModificacionRecursosModeloBase(documentos, pProyectoID, pPrioridadBase);
        }

        /// <summary>
        /// Notifica al modelo base que se han modificado una serie de documentos.
        /// </summary>
        /// <param name="pDocumentoID">Lista de documentos con su tipo</param>
        /// <param name="pProyectoID">Proyecto en el que se han modificado</param>
        public void EliminarRecursoModeloBaseSimple(Guid pDocumentoID, Guid pProyectoID, short pTipo)
        {
            EliminarRecursoModeloBaseSimple(pDocumentoID, pProyectoID, pTipo, null);
        }

        /// <summary>
        /// Notifica al modelo base que se han modificado una serie de documentos.
        /// </summary>
        /// <param name="pDocumentoID">Lista de documentos con su tipo</param>
        /// <param name="pProyectoID">Proyecto en el que se han modificado</param>
        public void EliminarRecursoModeloBaseSimple(Guid pDocumentoID, Guid pProyectoID, short pTipo, BaseRecursosComunidadDS pBaseRecursosComDS)
        {
            EliminarRecursoModeloBaseSimple(pDocumentoID, pProyectoID, pTipo, pBaseRecursosComDS, null);
        }

        /// <summary>
        /// Notifica al modelo base que se han modificado una serie de documentos.
        /// </summary>
        /// <param name="pDocumentoID">Lista de documentos con su tipo</param>
        /// <param name="pProyectoID">Proyecto en el que se han modificado</param>
        public void EliminarRecursoModeloBaseSimple(Guid pDocumentoID, Guid pProyectoID, short pTipo, BaseRecursosComunidadDS pBaseRecursosComDS, string pFicheroConfiguracionBD)
        {
            EliminarRecursoModeloBaseSimple(pDocumentoID, pProyectoID, pTipo, pBaseRecursosComDS, null, (short)EstadosColaTags.EnEspera);
        }

        /// <summary>
        /// Notifica al modelo base que se han modificado una serie de documentos.
        /// </summary>
        /// <param name="pDocumentoID">Lista de documentos con su tipo</param>
        /// <param name="pProyectoID">Proyecto en el que se han modificado</param>
        public void EliminarRecursoModeloBaseSimple(Guid pDocumentoID, Guid pProyectoID, short pTipo, BaseRecursosComunidadDS pBaseRecursosComDS, string pFicheroConfiguracionBD, short pEstado)
        {
            EliminarRecursoModeloBaseSimple(pDocumentoID, pProyectoID, pTipo, pBaseRecursosComDS, pFicheroConfiguracionBD, pEstado, (short)PrioridadBase.Alta);
        }

        /// <summary>
        /// Notifica al modelo base que se han modificado una serie de documentos.
        /// </summary>
        /// <param name="pDocumentoID">Lista de documentos con su tipo</param>
        /// <param name="pProyectoID">Proyecto en el que se han modificado</param>
        public void EliminarRecursoModeloBaseSimple(Guid pDocumentoID, Guid pProyectoID, short pTipo, BaseRecursosComunidadDS pBaseRecursosComDS, string pFicheroConfiguracionBD, short pEstado, short pPrioridad)
        {
            int id = -1;

            if (pProyectoID.Equals(Guid.Empty))
            {
                pProyectoID = ProyectoAD.MetaProyecto;
            }

            if (!string.IsNullOrEmpty(pFicheroConfiguracionBD))
            {
                ProyectoCN proyCN = new ProyectoCN(pFicheroConfiguracionBD, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                id = proyCN.ObtenerTablaBaseProyectoIDProyectoPorID(pProyectoID);
            }
            else
            {
                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                id = proyCL.ObtenerTablaBaseProyectoIDProyectoPorID(pProyectoID);
                proyCL.Dispose();
            }

            BaseRecursosComunidadDS baseRecursosComDS = pBaseRecursosComDS;
            if (baseRecursosComDS == null)
            {
                baseRecursosComDS = new BaseRecursosComunidadDS();
            }

            BaseRecursosComunidadDS.ColaTagsComunidadesRow filaColaTagsDocs = baseRecursosComDS.ColaTagsComunidades.NewColaTagsComunidadesRow();

            filaColaTagsDocs.Estado = pEstado;
            filaColaTagsDocs.FechaPuestaEnCola = DateTime.Now;
            filaColaTagsDocs.TablaBaseProyectoID = id;
            filaColaTagsDocs.Tags = Constantes.ID_TAG_DOCUMENTO + pDocumentoID.ToString() + Constantes.ID_TAG_DOCUMENTO + "," + Constantes.TIPO_DOC + pTipo + Constantes.TIPO_DOC + "," + new UtilidadesVirtuoso(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).TagBaseAfinidadVirtuoso;
            filaColaTagsDocs.Tipo = 1;
            filaColaTagsDocs.Prioridad = pPrioridad;

            baseRecursosComDS.ColaTagsComunidades.AddColaTagsComunidadesRow(filaColaTagsDocs);


            BaseComunidadCN brComCN = new BaseComunidadCN("base", -1, mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);
            brComCN.InsertarFilasEnRabbit("ColaTagsComunidades", baseRecursosComDS);
            baseRecursosComDS.Dispose();

        }

        /// <summary>
        /// Notifica al modelo base que se han modificado una serie de documentos.
        /// </summary>
        /// <param name="pDocumentoID">Lista de documentos con su tipo</param>
        /// <param name="pProyectoID">Proyecto en el que se han modificado</param>
        /// <param name="pPrioridadBase">Prioridad de las filas introducidas en el modelo base</param>
        public void AgregarComentarioModeloBaseSimple(Guid pDocumentoID, Guid pProyectoID, short pTipo, PrioridadBase pPrioridadBase)
        {
            AgregarRecursoModeloBaseSimple(pDocumentoID, pProyectoID, pTipo, null, "##COM-REC##c##COM-REC##", pPrioridadBase);
        }

        /// <summary>
        /// Notifica al modelo base que se han modificado una serie de documentos.
        /// </summary>
        /// <param name="pDocumentoID">Lista de documentos con su tipo</param>
        /// <param name="pProyectoID">Proyecto en el que se han modificado</param>
        /// <param name="pPrioridadBase">Prioridad de las filas introducidas en el modelo base</param>
        /// <param name="pEliminado">Indica si el comentario ha sido eliminado</param>
        public void AgregarComentarioModeloBaseSimple(Guid pDocumentoID, Guid pProyectoID, short pTipo, PrioridadBase pPrioridadBase, bool pEliminado)
        {
            AgregarRecursoModeloBaseSimple(pDocumentoID, pProyectoID, pTipo, null, "##COM-REC##c##COM-REC##", pPrioridadBase, pEliminado, -1, null);
        }

        /// <summary>
        /// Notifica al modelo base que se han modificado una serie de documentos.
        /// </summary>
        /// <param name="pDocumentoID">Lista de documentos con su tipo</param>
        /// <param name="pProyectoID">Proyecto en el que se han modificado</param>
        /// <param name="pPrioridadBase">Prioridad de las filas introducidas en el modelo base</param>
        public void AgregarRecursoModeloBaseSimple(Guid pDocumentoID, Guid pProyectoID, short pTipo, PrioridadBase pPrioridadBase)
        {
            AgregarRecursoModeloBaseSimple(pDocumentoID, pProyectoID, pTipo, null, pPrioridadBase);
        }

        /// <summary>
        /// Notifica al modelo base que se han modificado una serie de documentos.
        /// </summary>
        /// <param name="pDocumentoID">Lista de documentos con su tipo</param>
        /// <param name="pProyectoID">Proyecto en el que se han modificado</param>
        /// <param name="pPrioridadBase">Prioridad de las filas introducidas en el modelo base</param>
        public void AgregarRecursoModeloBaseSimple(Guid pDocumentoID, Guid pProyectoID, short pTipo, BaseRecursosComunidadDS pBaseRecursosComDS, PrioridadBase pPrioridadBase)
        {
            AgregarRecursoModeloBaseSimple(pDocumentoID, pProyectoID, pTipo, pBaseRecursosComDS, "", pPrioridadBase);
        }

        /// <summary>
        /// Notifica al modelo base que se han modificado una serie de documentos.
        /// </summary>
        /// <param name="pDocumentoID">Lista de documentos con su tipo</param>
        /// <param name="pProyectoID">Proyecto en el que se han modificado</param>
        /// <param name="pPrioridadBase">Prioridad de las filas introducidas en el modelo base</param>
        public void AgregarRecursoModeloBaseSimple(Guid pDocumentoID, Guid pProyectoID, short pTipo, BaseRecursosComunidadDS pBaseRecursosComDS, string pOtrosArgumentos, PrioridadBase pPrioridadBase)
        {
            AgregarRecursoModeloBaseSimple(pDocumentoID, pProyectoID, pTipo, pBaseRecursosComDS, pOtrosArgumentos, pPrioridadBase, false, -1, null);
        }

        /// <summary>
        /// Notifica al modelo base que se han modificado una serie de documentos.
        /// </summary>
        /// <param name="pDocumentoID">Lista de documentos con su tipo</param>
        /// <param name="pProyectoID">Proyecto en el que se han modificado</param>
        /// <param name="pPrioridadBase">Prioridad de las filas introducidas en el modelo base</param>
        /// <param name="pTipoElementoBase">Enumeración TipoElementoEnCola. Si no se especifica, por defecto será agregado</param>
        public void AgregarRecursoModeloBaseSimple(Guid pDocumentoID, Guid pProyectoID, short pTipo, BaseRecursosComunidadDS pBaseRecursosComDS, string pOtrosArgumentos, PrioridadBase pPrioridadBase, short? pTipoElementoBase)
        {
            AgregarRecursoModeloBaseSimple(pDocumentoID, pProyectoID, pTipo, pBaseRecursosComDS, pOtrosArgumentos, pPrioridadBase, false, -1, pTipoElementoBase);
        }

        /// <summary>
        /// Notifica al modelo base que se han modificado una serie de documentos.
        /// </summary>
        /// <param name="pDocumentoID">Lista de documentos con su tipo</param>
        /// <param name="pProyectoID">Proyecto en el que se han modificado</param>
        /// <param name="pTipo">Tipo de documento</param>
        /// <param name="pBaseRecursosComDS">Dataset de base recursos comunidad</param>
        /// <param name="pFicheroConfiguracionBD">Fichero configuración BD</param>
        /// <param name="pOtrosArgumentos">Argumentos que van en el campo Tags de colatagscomunidades</param>
        /// <param name="pEliminado">Indica si la fila es de tipo eliminado o agregado. Válido si no se especifica pTipoElementoBase</param>
        /// <param name="pPrioridadBase">Prioridad de las filas introducidas en el modelo base</param>
        /// <param name="pEstadoCargaID"></param>
        /// <param name="pTipoElementoBase">Enumeración TipoElementoEnCola. Si no se especifica, será eliminado o agregado en función del parámetro pEliminado</param>
        public void AgregarRecursoModeloBaseSimple(Guid pDocumentoID, Guid pProyectoID, short pTipo, BaseRecursosComunidadDS pBaseRecursosComDS, string pOtrosArgumentos, PrioridadBase pPrioridadBase, bool pEliminado, long pEstadoCargaID, short? pTipoElementoBase)
        {
            int id = -1;

            if (pProyectoID.Equals(Guid.Empty))
            {
                pProyectoID = ProyectoAD.MetaProyecto;
            }

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            id = proyCL.ObtenerTablaBaseProyectoIDProyectoPorID(pProyectoID);
            proyCL.Dispose();

            BaseRecursosComunidadDS baseRecursosComDS = pBaseRecursosComDS;
            if (baseRecursosComDS == null)
            {
                baseRecursosComDS = new BaseRecursosComunidadDS();
            }

            #region Marcar agregado

            BaseRecursosComunidadDS.ColaTagsComunidadesRow filaColaTagsDocs = baseRecursosComDS.ColaTagsComunidades.NewColaTagsComunidadesRow();

            filaColaTagsDocs.Estado = (short)EstadosColaTags.EnEspera;
            filaColaTagsDocs.FechaPuestaEnCola = DateTime.Now;
            filaColaTagsDocs.TablaBaseProyectoID = id;
            filaColaTagsDocs.Tags = Constantes.ID_TAG_DOCUMENTO + pDocumentoID.ToString() + Constantes.ID_TAG_DOCUMENTO + "," + Constantes.TIPO_DOC + pTipo.ToString() + Constantes.TIPO_DOC + pOtrosArgumentos + "," + new UtilidadesVirtuoso(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).TagBaseAfinidadVirtuoso;

            if (pEstadoCargaID != -1)
            {
                filaColaTagsDocs.TipoAccionCarga = 0;
                filaColaTagsDocs.EstadoCargaID = pEstadoCargaID;
            }

            if (!pTipoElementoBase.HasValue || !Enum.IsDefined(typeof(TiposElementosEnCola), (int)pTipoElementoBase.Value))
            {
                if (pEliminado)
                {
                    filaColaTagsDocs.Tipo = 1;
                }
                else
                {
                    filaColaTagsDocs.Tipo = 0;
                }
            }
            else
            {
                filaColaTagsDocs.Tipo = pTipoElementoBase.Value;
            }

            filaColaTagsDocs.Prioridad = (short)pPrioridadBase;

            baseRecursosComDS.ColaTagsComunidades.AddColaTagsComunidadesRow(filaColaTagsDocs);

            #endregion


            BaseComunidadCN brComCN = new BaseComunidadCN("base", -1, mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);
            brComCN.InsertarFilasEnRabbit("ColaTagsComunidades", baseRecursosComDS);

            baseRecursosComDS.Dispose();


        }

        /// <summary>
        /// Notifica al modelo base que se ha compartido una ontología
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <param name="pOntologiaID"></param>
        /// <param name="pListaProyectos"></param>
        /// <param name="pPrioridadBase"></param>
        public void AgregarComparticionOntologia(Guid pProyectoID, Guid pOntologiaID, List<Guid> pListaProyectos, PrioridadBase pPrioridadBase)
        {
            int id = -1;

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            id = proyCL.ObtenerTablaBaseProyectoIDProyectoPorID(pProyectoID);
            proyCL.Dispose();

            BaseRecursosComunidadDS baseRecursosComDS = new BaseRecursosComunidadDS();

            #region Marcar agregado

            BaseRecursosComunidadDS.ColaTagsComunidadesRow filaColaTagsDocs = baseRecursosComDS.ColaTagsComunidades.NewColaTagsComunidadesRow();

            filaColaTagsDocs.Estado = (short)EstadosColaTags.EnEspera;
            filaColaTagsDocs.FechaPuestaEnCola = DateTime.Now;
            filaColaTagsDocs.TablaBaseProyectoID = id;

            string proyectos = string.Empty;
            foreach (Guid proyID in pListaProyectos)
            {
                proyectos += proyID + ",";
            }

            filaColaTagsDocs.Tags = Constantes.ID_TAG_DOCUMENTO + pOntologiaID.ToString() + Constantes.ID_TAG_DOCUMENTO + "," + Constantes.ID_PROY_DESTINO + proyectos + Constantes.ID_PROY_DESTINO + "," + new UtilidadesVirtuoso(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).TagBaseAfinidadVirtuoso;

            filaColaTagsDocs.Tipo = 0;

            filaColaTagsDocs.Prioridad = (short)pPrioridadBase;

            baseRecursosComDS.ColaTagsComunidades.AddColaTagsComunidadesRow(filaColaTagsDocs);

            #endregion

            BaseComunidadCN brComCN = new BaseComunidadCN("base", -1, mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);
            brComCN.InsertarFilasEnRabbit("ColaTagsComunidades", baseRecursosComDS);
            baseRecursosComDS.Dispose();
        }

        /// <summary>
        /// Notifica al modelo base que se ha eliminado un documento.
        /// </summary>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <param name="pProyectoID">Proyecto en el que se han modificado</param>
        /// <param name="pTipo">Tipo de documento</param>
        /// <param name="pPrioridadBase">Prioridad de las filas introducidas en el modelo base</param>
        public void AgregarRecursoEliminadoModeloBaseSimple(Guid pDocumentoID, Guid pProyectoID, short pTipo, PrioridadBase pPrioridadBase)
        {
            AgregarRecursoModeloBaseSimple(pDocumentoID, pProyectoID, pTipo, null, "", pPrioridadBase, true, -1, null);
        }

        /// <summary>
        /// Notifica al modelo base que se han modificado una serie de documentos.
        /// </summary>
        /// <param name="pDocumentoID">Lista de documentos con su tipo</param>
        /// <param name="pProyectoID">Proyecto en el que se han modificado</param>
        /// <param name="pPrioridadBase">Prioridad de las filas introducidas en el modelo base</param>
        public void AgregarSuscripcionFacModeloBaseSimple(Guid pSuscripcionID, Guid pRecursoID, string pFicheroConfiguracionBDBase, string pFicheroConfiguracionBD, string pOtrosArgumentos, PrioridadBase pPrioridadBase)
        {
            BaseSuscripcionesDS pBaseSuscripcionesDS = new BaseSuscripcionesDS();

            if (ProyectoAD.TablaBaseIdMetaProyecto == int.MinValue)
            {
                ProyectoAD.TablaBaseIdMetaProyecto = ObtenerFilaProyecto(ProyectoAD.MetaProyecto).TablaBaseProyectoID;
            }

            BaseSuscripcionesDS baseSuscripcionesDS = pBaseSuscripcionesDS;

            #region Marcar agregado

            BaseSuscripcionesDS.ColaTagsSuscripcionesRow filaColaTagsCom = baseSuscripcionesDS.ColaTagsSuscripciones.NewColaTagsSuscripcionesRow();
            filaColaTagsCom.Estado = (short)EstadosColaTags.EnEspera;
            filaColaTagsCom.FechaPuestaEnCola = DateTime.Now;
            filaColaTagsCom.TablaBaseProyectoID = ProyectoAD.TablaBaseIdMetaProyecto;
            filaColaTagsCom.Tags = Constantes.ID_SUSCRIPCION + pSuscripcionID.ToString() + Constantes.ID_SUSCRIPCION + "," + Constantes.ID_SUSCRIPCION_RECURSO + pRecursoID.ToString() + Constantes.ID_SUSCRIPCION_RECURSO + "," + Constantes.ID_SUSCRIPCION_PERFIL + pOtrosArgumentos + Constantes.ID_SUSCRIPCION_PERFIL;
            filaColaTagsCom.Tipo = 0;
            filaColaTagsCom.Prioridad = (short)pPrioridadBase;

            baseSuscripcionesDS.ColaTagsSuscripciones.AddColaTagsSuscripcionesRow(filaColaTagsCom);

            #endregion

            BaseComunidadCN brComCN = new BaseComunidadCN(mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);//, -1
            brComCN.InsertarFilasEnRabbit("ColaTagsSuscripciones", baseSuscripcionesDS);

            pBaseSuscripcionesDS.Dispose();
        }

        /// <summary>
        /// Notifica al modelo base que se han modificado una serie de documentos.
        /// </summary>
        /// <param name="pDocumentoID">Lista de documentos con su tipo</param>
        /// <param name="pProyectoID">Proyecto en el que se han modificado</param>
        /// <param name="pPrioridadBase">Prioridad de las filas introducidas en el modelo base</param>
        public void AgregarComentarioFacModeloBaseSimple(Guid pComentarioID, Guid pProyectoID, string pFicheroConfiguracionBaseBD, string pFicheroConfiguracionBD, string pOtrosArgumentos, PrioridadBase pPrioridadBase, int pTablaBaseIdMetaProyecto)
        {
            BaseComentariosDS baseComentariosDS = new BaseComentariosDS();

            if (ProyectoAD.TablaBaseIdMetaProyecto == int.MinValue)
            {
                ProyectoAD.TablaBaseIdMetaProyecto = ObtenerFilaProyecto(ProyectoAD.MetaProyecto).TablaBaseProyectoID;
            }

            if (baseComentariosDS == null)
            {
                baseComentariosDS = new BaseComentariosDS();
            }

            #region Marcar agregado

            BaseComentariosDS.ColaTagsComentariosRow filaColaTagsCom = baseComentariosDS.ColaTagsComentarios.NewColaTagsComentariosRow();

            filaColaTagsCom.Estado = (short)EstadosColaTags.EnEspera;
            filaColaTagsCom.FechaPuestaEnCola = DateTime.Now;
            filaColaTagsCom.TablaBaseProyectoID = pTablaBaseIdMetaProyecto;
            filaColaTagsCom.Tags = Constantes.ID_COMENTARIO + pComentarioID.ToString() + Constantes.ID_COMENTARIO + "," + Constantes.IDS_COMENTARIO_PERFIL + pOtrosArgumentos + Constantes.IDS_COMENTARIO_PERFIL;
            filaColaTagsCom.Tipo = 0;
            filaColaTagsCom.Prioridad = (short)pPrioridadBase;

            try
            {
                InsertarFilaEnColaTagsComentarioRabbitMQ(filaColaTagsCom);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla ColaTagsComentarios");
                baseComentariosDS.ColaTagsComentarios.AddColaTagsComentariosRow(filaColaTagsCom);
            }


            #endregion


            BaseComunidadCN brComCN = new BaseComunidadCN(mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);//, -1
            brComCN.InsertarFilasEnRabbit("ColaTagsComentarios", baseComentariosDS);

            baseComentariosDS.Dispose();
        }

        public void InsertarFilaEnColaTagsComentarioRabbitMQ(BaseComentariosDS.ColaTagsComentariosRow pFilaColaTagsComentario)
        {
            if (HayConexionRabbit)
            {
                using (RabbitMQClient rabbitMQ = new RabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN, COLA_TAGS_COMENTARIO, mLoggingService, mConfigService, EXCHANGE, COLA_TAGS_COMENTARIO))
                {
                    rabbitMQ.AgregarElementoACola(JsonConvert.SerializeObject(pFilaColaTagsComentario.ItemArray));
                }
            }
        }

        /// <summary>
        /// Notifica al modelo base que se han modificado una serie de documentos.
        /// </summary>
        /// <param name="pDocumentos">Lista de documentos con su tipo</param>
        /// <param name="pProyectoID">Proyecto en el que se han modificado</param>
        /// <param name="pPrioridadBase">Prioridad de las filas introducidas en el modelo base</param>
        public void AgregarModificacionRecursosModeloBase(Dictionary<Guid, short> pDocumentos, Guid pProyectoID, PrioridadBase pPrioridadBase)
        {
            if (pDocumentos.Count > 0)
            {
                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                int id = proyCL.ObtenerTablaBaseProyectoIDProyectoPorID(pProyectoID);
                proyCL.Dispose();

                BaseRecursosComunidadDS baseRecursosComDS = new BaseRecursosComunidadDS();

                foreach (Guid documentoID in pDocumentos.Keys)
                {
                    #region Marcar agregado

                    BaseRecursosComunidadDS.ColaTagsComunidadesRow filaColaTagsDocs = baseRecursosComDS.ColaTagsComunidades.NewColaTagsComunidadesRow();

                    filaColaTagsDocs.Estado = (short)EstadosColaTags.EnEspera;
                    filaColaTagsDocs.FechaPuestaEnCola = DateTime.Now;
                    filaColaTagsDocs.TablaBaseProyectoID = id;
                    filaColaTagsDocs.Tags = Constantes.ID_TAG_DOCUMENTO + documentoID.ToString() + Constantes.ID_TAG_DOCUMENTO + "," + Constantes.TIPO_DOC + pDocumentos[documentoID] + Constantes.TIPO_DOC + "," + new UtilidadesVirtuoso(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).TagBaseAfinidadVirtuoso;
                    filaColaTagsDocs.Tipo = 0;
                    filaColaTagsDocs.Prioridad = (short)pPrioridadBase;

                    baseRecursosComDS.ColaTagsComunidades.AddColaTagsComunidadesRow(filaColaTagsDocs);

                    #endregion
                }

                BaseComunidadCN brComCN = new BaseComunidadCN("base", -1, mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);
                brComCN.InsertarFilasEnRabbit("ColaTagsComunidades", baseRecursosComDS);

                baseRecursosComDS.Dispose();
            }
        }

        /// <summary>
        /// Notifica al modelo base que se han eliminado categorías y hay que cambiar los vinculos con recursos y Dafos
        /// </summary>
        /// <param name="pCategorias">Dictionary cuya clave es la categoría destino y la lista las categorías que desaparecen</param>
        /// <param name="pProyecto">Proyecto al que le afecta</param>
        ///<param name="pTodo">Indica si se mueven todos los recursos o tan solo los que se queden huerfanos</param>
        /// <param name="pPrioridadBase">Prioridad de las filas introducidas en el modelo base</param>
        public void AgregarEliminacionCategoriasModeloBase(Dictionary<Guid, List<Guid>> pCategorias, Guid pProyectoID, bool pTodo, PrioridadBase pPrioridadBase)
        {
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            int id = proyCL.ObtenerTablaBaseProyectoIDProyectoPorID(pProyectoID);
            proyCL.Dispose();

            BaseRecursosComunidadDS baseRecursosComDS = new BaseRecursosComunidadDS();

            foreach (Guid claveCatDestino in pCategorias.Keys)
            {
                foreach (Guid claveCatBorrada in pCategorias[claveCatDestino])
                {
                    BaseRecursosComunidadDS.ColaTagsComunidadesRow filaColaTagsDocs = baseRecursosComDS.ColaTagsComunidades.NewColaTagsComunidadesRow();

                    filaColaTagsDocs.Estado = (short)EstadosColaTags.EnEspera;
                    filaColaTagsDocs.FechaPuestaEnCola = DateTime.Now;
                    filaColaTagsDocs.TablaBaseProyectoID = id;
                    filaColaTagsDocs.Prioridad = (short)pPrioridadBase;

                    if (pTodo)
                    {
                        //se deben de crear vinculaciones con la categoría claveCatDestino y sus padres de todos los recursos y dafos que esten vinculados con claveCatborrada y elminar todas las vinculaciones que hay con claveCatBorrada
                        filaColaTagsDocs.Tipo = (short)TiposElementosEnCola.CategoriaEliminadaRecategorizarTodo;
                        filaColaTagsDocs.Tags = Constantes.CAT_DOC + claveCatBorrada.ToString() + Constantes.CAT_DOC + "," + Constantes.CAT_DOC + claveCatDestino.ToString() + Constantes.CAT_DOC;

                    }
                    else
                    {
                        //se deben de crear vinculaciones con la categoría claveCatDestino y susu padres de los recursos y dafos que esten vinculados SOLO con claveCatborrada y elminar todas las vinculaciones que hay con claveCatBorrada
                        filaColaTagsDocs.Tipo = (short)TiposElementosEnCola.CategoriaEliminadaRecategorizarHuerfanos;
                        filaColaTagsDocs.Tags = Constantes.CAT_DOC + claveCatBorrada.ToString() + Constantes.CAT_DOC + "," + Constantes.CAT_DOC + claveCatDestino.ToString() + Constantes.CAT_DOC;
                    }

                    filaColaTagsDocs.Tags += "," + new UtilidadesVirtuoso(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).TagBaseAfinidadVirtuoso;
                    baseRecursosComDS.ColaTagsComunidades.AddColaTagsComunidadesRow(filaColaTagsDocs);
                }
            }

            BaseComunidadCN brComCN = new BaseComunidadCN("base", -1, mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);
            brComCN.InsertarFilasEnRabbit("ColaTagsComunidades", baseRecursosComDS);

            baseRecursosComDS.Dispose();
        }



        /// <summary>
        /// Notifica al modelo base que se han eliminado categorías y hay que cambiar los vinculos con recursos y Dafos para un Usuario
        /// </summary>
        /// <param name="pCategorias">Dictionary cuya clave es la categoría destino y la lista las categorías que desaparecen</param>
        /// <param name="pProyecto">Proyecto al que le afecta</param>
        ///<param name="pTodo">Indica si se mueven todos los recursos o tan solo los que se queden huerfanos</param>
        /// <param name="pPrioridadBase">Prioridad de las filas introducidas en el modelo base</param>
        public void AgregarEliminacionCategoriasModeloBaseUsuario(Dictionary<Guid, List<Guid>> pCategorias, bool pTodo, PrioridadBase pPrioridadbase)
        {
            if (ProyectoAD.TablaBaseIdMetaProyecto == int.MinValue)
            {
                ProyectoAD.TablaBaseIdMetaProyecto = ObtenerFilaProyecto(ProyectoAD.MetaProyecto).TablaBaseProyectoID;
            }

            BaseRecursosComunidadDS baseRecursosComDS = new BaseRecursosComunidadDS();

            foreach (Guid claveCatDestino in pCategorias.Keys)
            {
                foreach (Guid claveCatBorrada in pCategorias[claveCatDestino])
                {
                    BaseRecursosComunidadDS.ColaTagsComunidadesRow filaColaTagsDocs = baseRecursosComDS.ColaTagsComunidades.NewColaTagsComunidadesRow();

                    filaColaTagsDocs.Estado = (short)EstadosColaTags.EnEspera;
                    filaColaTagsDocs.FechaPuestaEnCola = DateTime.Now;
                    filaColaTagsDocs.TablaBaseProyectoID = ProyectoAD.TablaBaseIdMetaProyecto;
                    filaColaTagsDocs.Prioridad = (short)pPrioridadbase;

                    if (pTodo)
                    {
                        //se deben de crear vinculaciones con la categoría claveCatDestino y sus padres de todos los recursos y dafos que esten vinculados con claveCatborrada y elminar todas las vinculaciones que hay con claveCatBorrada
                        filaColaTagsDocs.Tipo = (short)TiposElementosEnCola.CategoriaEliminadaRecategorizarTodo;
                        filaColaTagsDocs.Tags = Constantes.CAT_DOC + claveCatBorrada.ToString() + Constantes.CAT_DOC + "," + Constantes.CAT_DOC + claveCatDestino.ToString() + Constantes.CAT_DOC;

                    }
                    else
                    {
                        //se deben de crear vinculaciones con la categoría claveCatDestino y susu padres de los recursos y dafos que esten vinculados SOLO con claveCatborrada y elminar todas las vinculaciones que hay con claveCatBorrada
                        filaColaTagsDocs.Tipo = (short)TiposElementosEnCola.CategoriaEliminadaRecategorizarHuerfanos;
                        filaColaTagsDocs.Tags = Constantes.CAT_DOC + claveCatBorrada.ToString() + Constantes.CAT_DOC + "," + Constantes.CAT_DOC + claveCatDestino.ToString() + Constantes.CAT_DOC;
                    }

                    filaColaTagsDocs.Tags += "," + new UtilidadesVirtuoso(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).TagBaseAfinidadVirtuoso;
                    baseRecursosComDS.ColaTagsComunidades.AddColaTagsComunidadesRow(filaColaTagsDocs);
                }
            }

            BaseComunidadCN brComCN = new BaseComunidadCN("base", -1, mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);
            brComCN.InsertarFilasEnRabbit("ColaTagsComunidades", baseRecursosComDS);

            baseRecursosComDS.Dispose();
        }
        /// <summary>
        /// Notifica al modelo base que se han cambiado de padres varias categorías y hay que cambiar los vinculos con recursos y Dafos
        /// </summary>
        /// <param name="pCategorias">Dictionary cuya clave es la categoría padre nueva y la lista son las categorías que cambian de padre</param>
        /// <param name="pProyecto">Proyecto al que le afecta</param>
        /// <param name="pPrioridadBase">Prioridad de las filas introducidas en el modelo base</param>
        public void AgregarMoverCategoriasModeloBase(Dictionary<Guid, List<Guid>> pCategorias, Guid pProyectoID, PrioridadBase pPrioridadBase)
        {
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            int id = proyCL.ObtenerTablaBaseProyectoIDProyectoPorID(pProyectoID);
            proyCL.Dispose();

            BaseRecursosComunidadDS baseRecursosComDS = new BaseRecursosComunidadDS();

            foreach (Guid claveCatDestino in pCategorias.Keys)
            {
                foreach (Guid claveCatMovida in pCategorias[claveCatDestino])
                {
                    BaseRecursosComunidadDS.ColaTagsComunidadesRow filaColaTagsDocs = baseRecursosComDS.ColaTagsComunidades.NewColaTagsComunidadesRow();

                    filaColaTagsDocs.Estado = (short)EstadosColaTags.EnEspera;
                    filaColaTagsDocs.FechaPuestaEnCola = DateTime.Now;
                    filaColaTagsDocs.TablaBaseProyectoID = id;
                    //se deben eliminar todas las vinculaciones que hay en Virtuoso de los recursos y dafos que esten vinculados a claveCatMovida(excepto esta propia relación) y realizar las vinculaciones con la categoría claevcatDestino

                    filaColaTagsDocs.Tipo = (short)TiposElementosEnCola.CategoriaEliminadaRecategorizarTodo;
                    filaColaTagsDocs.Tags = Constantes.CAT_DOC + claveCatMovida.ToString() + Constantes.CAT_DOC + "," + Constantes.CAT_DOC + claveCatDestino.ToString() + Constantes.CAT_DOC + "," + new UtilidadesVirtuoso(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).TagBaseAfinidadVirtuoso;
                    filaColaTagsDocs.Prioridad = (short)pPrioridadBase;

                    //filaColaTagsDocs.Tags = BaseRecursosComunidadAD.ID_TAG_DOCUMENTO + documentoID.ToString() + BaseRecursosComunidadAD.ID_TAG_DOCUMENTO + "," + BaseRecursosComunidadAD.TIPO_DOC + pDocumentos[documentoID] + BaseRecursosComunidadAD.TIPO_DOC;
                    //filaColaTagsDocs.Tipo = 0;                   

                    baseRecursosComDS.ColaTagsComunidades.AddColaTagsComunidadesRow(filaColaTagsDocs);
                }
            }

            BaseComunidadCN brComCN = new BaseComunidadCN("base", -1, mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);
            brComCN.InsertarFilasEnRabbit("ColaTagsComunidades", baseRecursosComDS);

            baseRecursosComDS.Dispose();

        }

        /// <summary>
        /// Notifica al modelo base que se ha cambiado la política de certificación
        /// </summary>
        /// <param name="pProyecto">Proyecto al que le afecta</param>
        /// <param name="pPrioridadBase">Prioridad de las filas introducidas en el modelo base</param>
        public void EditarPoliticaCertificacionModeloBase(Guid pProyectoID, PrioridadBase pPrioridadBase)
        {
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            int id = proyCL.ObtenerTablaBaseProyectoIDProyectoPorID(pProyectoID);
            proyCL.Dispose();

            BaseRecursosComunidadDS baseRecursosComDS = new BaseRecursosComunidadDS();


            BaseRecursosComunidadDS.ColaTagsComunidadesRow filaColaTagsDocs = baseRecursosComDS.ColaTagsComunidades.NewColaTagsComunidadesRow();

            filaColaTagsDocs.Estado = (short)EstadosColaTags.EnEspera;
            filaColaTagsDocs.FechaPuestaEnCola = DateTime.Now;
            filaColaTagsDocs.TablaBaseProyectoID = id;
            filaColaTagsDocs.Tipo = (short)TiposElementosEnCola.NivelesCertificacionModificados;
            filaColaTagsDocs.Prioridad = (short)pPrioridadBase;
            filaColaTagsDocs.Tags = new UtilidadesVirtuoso(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).TagBaseAfinidadVirtuoso;

            baseRecursosComDS.ColaTagsComunidades.AddColaTagsComunidadesRow(filaColaTagsDocs);

            BaseComunidadCN brComCN = new BaseComunidadCN("base", -1, mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);
            brComCN.InsertarFilasEnRabbit("ColaTagsComunidades", baseRecursosComDS);

            baseRecursosComDS.Dispose();
        }

        /// <summary>
        /// Notifica al modelo base que se han cambiado de padres varias categorías y hay que cambiar los vinculos con recursos y Dafos
        /// </summary>
        /// <param name="pCategorias">Dictionary cuya clave es la categoría padre nueva y la lista son las categorías que cambian de padre</param>
        /// <param name="pProyecto">Proyecto al que le afecta</param>
        /// <param name="pPrioridadBase">Prioridad de las filas introducidas en el modelo base</param>
        public void AgregarMoverCategoriasModeloBaseUsuario(Dictionary<Guid, List<Guid>> pCategorias, PrioridadBase pPrioridadBase)
        {
            if (ProyectoAD.TablaBaseIdMetaProyecto == int.MinValue)
            {
                ProyectoAD.TablaBaseIdMetaProyecto = ObtenerFilaProyecto(ProyectoAD.MetaProyecto).TablaBaseProyectoID;
            }

            BaseRecursosComunidadDS baseRecursosComDS = new BaseRecursosComunidadDS();

            foreach (Guid claveCatDestino in pCategorias.Keys)
            {
                foreach (Guid claveCatMovida in pCategorias[claveCatDestino])
                {
                    BaseRecursosComunidadDS.ColaTagsComunidadesRow filaColaTagsDocs = baseRecursosComDS.ColaTagsComunidades.NewColaTagsComunidadesRow();

                    filaColaTagsDocs.Estado = (short)EstadosColaTags.EnEspera;
                    filaColaTagsDocs.FechaPuestaEnCola = DateTime.Now;
                    filaColaTagsDocs.TablaBaseProyectoID = ProyectoAD.TablaBaseIdMetaProyecto;
                    //se deben eliminar todas las vinculaciones que hay en Virtuoso de los recursos y dafos que esten vinculados a claveCatMovida(excepto esta propia relación) y realizar las vinculaciones con la categoría claevcatDestino

                    filaColaTagsDocs.Tipo = (short)TiposElementosEnCola.CategoriaEliminadaRecategorizarTodo;
                    filaColaTagsDocs.Tags = Constantes.CAT_DOC + claveCatMovida.ToString() + Constantes.CAT_DOC + "," + Constantes.CAT_DOC + claveCatDestino.ToString() + Constantes.CAT_DOC + "," + new UtilidadesVirtuoso(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).TagBaseAfinidadVirtuoso;
                    filaColaTagsDocs.Prioridad = (short)pPrioridadBase;

                    //filaColaTagsDocs.Tags = BaseRecursosComunidadAD.ID_TAG_DOCUMENTO + documentoID.ToString() + BaseRecursosComunidadAD.ID_TAG_DOCUMENTO + "," + BaseRecursosComunidadAD.TIPO_DOC + pDocumentos[documentoID] + BaseRecursosComunidadAD.TIPO_DOC;
                    //filaColaTagsDocs.Tipo = 0;                   

                    baseRecursosComDS.ColaTagsComunidades.AddColaTagsComunidadesRow(filaColaTagsDocs);
                }
            }

            BaseComunidadCN brComCN = new BaseComunidadCN("base", -1, mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);
            brComCN.InsertarFilasEnRabbit("ColaTagsComunidades", baseRecursosComDS);

            baseRecursosComDS.Dispose();
        }

        /// <summary>
        /// Notifica al modelo base que se han modificado una serie de documentos.
        /// </summary>
        /// <param name="pProyectoID">Proyecto en el que se han modificado</param>
        /// <param name="pSoloGenerarTags">Indica las filas deben ir solo al Servicio de Generar Autocompletar (TRUE) o si también debe ir al Servicio Base (FALSE)</param>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración de BD</param>
        /// <param name="pPrioridadBase">Prioridad de las filas introducidas en el modelo base</param>
        public void AgregarTodosLosRecursosComunidadModeloBase(Guid pProyectoID, bool pSoloGenerarTags, string pFicheroConfiguracionBD, PrioridadBase pPrioridadBase)
        {
            int id = -1;

            if (pProyectoID.Equals(Guid.Empty))
            {
                pProyectoID = ProyectoAD.MetaProyecto;
            }

            if (!string.IsNullOrEmpty(pFicheroConfiguracionBD))
            {
                ProyectoCN proyCN = new ProyectoCN(pFicheroConfiguracionBD, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                id = proyCN.ObtenerTablaBaseProyectoIDProyectoPorID(pProyectoID);
            }
            else
            {
                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                id = proyCL.ObtenerTablaBaseProyectoIDProyectoPorID(pProyectoID);
                proyCL.Dispose();
            }

            BaseRecursosComunidadDS baseRecursosComDS = new BaseRecursosComunidadDS();

            #region Marcar agregado

            BaseRecursosComunidadDS.ColaTagsComunidadesRow filaColaTagsDocs = baseRecursosComDS.ColaTagsComunidades.NewColaTagsComunidadesRow();

            filaColaTagsDocs.Estado = 101;
            filaColaTagsDocs.FechaPuestaEnCola = DateTime.Now;
            filaColaTagsDocs.TablaBaseProyectoID = id;

            if (pSoloGenerarTags)
            {
                filaColaTagsDocs.Tags = Constantes.GENERAR_TODOS_RECURSOS + "1" + Constantes.GENERAR_TODOS_RECURSOS;
            }
            else
            {
                filaColaTagsDocs.Tags = Constantes.GENERAR_TODOS_RECURSOS + "0" + Constantes.GENERAR_TODOS_RECURSOS;
            }

            filaColaTagsDocs.Tags += "," + new UtilidadesVirtuoso(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).TagBaseAfinidadVirtuoso;
            filaColaTagsDocs.Tipo = 0;
            filaColaTagsDocs.Prioridad = (short)pPrioridadBase;

            baseRecursosComDS.ColaTagsComunidades.AddColaTagsComunidadesRow(filaColaTagsDocs);

            #endregion

            BaseComunidadCN brComCN = new BaseComunidadCN("base", -1, mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);
            brComCN.InsertarFilasEnRabbit("ColaTagsComunidades", baseRecursosComDS);

            baseRecursosComDS.Dispose();

        }

        #endregion

        #endregion

        #region GnossLive

        /// <summary>
        /// Añade a la cola de GnossLive un elemento para su procesamiento.
        /// </summary>
        /// <param name="pProyectoID">Proyecto al que pertenece el elemento</param>
        /// <param name="pElementoID">Identificador del elemento que se está tratando</param>
        /// <param name="pAccion">Acción que se está realizando</param>
        /// <param name="pTipoElemento">Tipo de elemento que se está tratando</param>
        /// <param name="pPrioridad">Prioridad</param>
        public void ActualizarGnossLive(Guid pProyectoID, Guid pElementoID, AccionLive pAccion, int pTipoElemento, PrioridadLive pPrioridad)
        {
            ActualizarGnossLive(pProyectoID, pElementoID, pAccion, pTipoElemento, false, pPrioridad);
        }

        /// <summary>
        /// Añade a la cola de GnossLive un elemento para su procesamiento.
        /// </summary>
        /// <param name="pProyectoID">Proyecto al que pertenece el elemento</param>
        /// <param name="pElementoID">Identificador del elemento que se está tratando</param>
        /// <param name="pAccion">Acción que se está realizando</param>
        /// <param name="pTipoElemento">Tipo de elemento que se está tratando</param>
        /// <param name="pPrioridad">Prioridad</param>
        /// <param name="pInfoExtra">Info extra</param>
        public void ActualizarGnossLive(Guid pProyectoID, Guid pElementoID, AccionLive pAccion, int pTipoElemento, PrioridadLive pPrioridad, string pInfoExtra)
        {
            ActualizarGnossLive(pProyectoID, pElementoID, pAccion, pTipoElemento, false, "base", pPrioridad, pInfoExtra);
        }

        /// <summary>
        /// Añade a la cola de GnossLive un elemento para su procesamiento.
        /// </summary>
        /// <param name="pProyectoID">Proyecto al que pertenece el elemento</param>
        /// <param name="pElemen0toID">Identificador del elemento que se está tratando</param>
        /// <param name="pAccion">Acción que se está realizando</param>
        /// <param name="pTipoElemento">Tipo de elemento que se está tratando</param>
        /// <param name="pSoloPersonal">Solo afeta a la actividad reciente del menú personal</param>
        /// <param name="pPrioridad">Prioridad</param>
        public void ActualizarGnossLive(Guid pProyectoID, Guid pElementoID, AccionLive pAccion, int pTipoElemento, bool pSoloPersonal, PrioridadLive pPrioridad)
        {
            ActualizarGnossLive(pProyectoID, pElementoID, pAccion, pTipoElemento, pSoloPersonal, "base", pPrioridad);
        }

        /// <summary>
        /// Añade a la cola de GnossLive un elemento para su procesamiento.
        /// </summary>
        /// <param name="pProyectoID">Proyecto al que pertenece el elemento</param>
        /// <param name="pElementoID">Identificador del elemento que se está tratando</param>
        /// <param name="pAccion">Acción que se está realizando</param>
        /// <param name="pTipoElemento">Tipo de elemento que se está tratando</param>
        /// <param name="pPrioridad">Prioridad</param>
        public void ActualizarGnossLive(Guid pProyectoID, Guid pElementoID, AccionLive pAccion, int pTipoElemento, string rutaBase, PrioridadLive pPrioridad)
        {
            ActualizarGnossLive(pProyectoID, pElementoID, pAccion, pTipoElemento, false, rutaBase, pPrioridad);
        }

        /// <summary>
        /// Añade a la cola de GnossLive un elemento para su procesamiento.
        /// </summary>
        /// <param name="pProyectoID">Proyecto al que pertenece el elemento</param>
        /// <param name="pElementoID">Identificador del elemento que se está tratando</param>
        /// <param name="pAccion">Acción que se está realizando</param>
        /// <param name="pTipoElemento">Tipo de elemento que se está tratando</param>
        /// <param name="pPrioridad">Prioridad</param>
        public void ActualizarGnossLive(Guid pProyectoID, Guid pElementoID, AccionLive pAccion, int pTipoElemento, bool pSoloPersonal, string rutaBase, PrioridadLive pPrioridad)
        {
            ActualizarGnossLive(pProyectoID, pElementoID, pAccion, pTipoElemento, pSoloPersonal, rutaBase, pPrioridad, null);
        }

        /// <summary>
        /// Añade a la cola de GnossLive un elemento para su procesamiento.
        /// </summary>
        /// <param name="pProyectoID">Proyecto al que pertenece el elemento</param>
        /// <param name="pElementoID">Identificador del elemento que se está tratando</param>
        /// <param name="pAccion">Acción que se está realizando</param>
        /// <param name="pTipoElemento">Tipo de elemento que se está tratando</param>
        /// <param name="pPrioridad">Prioridad</param>
        /// <param name="pInfoExtra">Infomación extra</param>
        public void ActualizarGnossLive(Guid pProyectoID, Guid pElementoID, AccionLive pAccion, int pTipoElemento, bool pSoloPersonal, string pRutaBase, PrioridadLive pPrioridad, string pInfoExtra)
        {
            Dictionary<Guid, int> ElementoIDTipoElemento = new Dictionary<Guid, int>();
            ElementoIDTipoElemento.Add(pElementoID, pTipoElemento);
            ActualizarGnossLive(pProyectoID, ElementoIDTipoElemento, pAccion, pSoloPersonal, pRutaBase, pPrioridad, pInfoExtra);
        }

        /// <summary>
        /// Añade a la cola de GnossLive un elemento para su procesamiento.
        /// </summary>
        /// <param name="pProyectoID">Proyecto al que pertenece el elemento</param>
        /// <param name="pListaElementoIDs">Lista de identificadores de los elementos que se están tratando</param>
        /// <param name="pAccion">Acción que se está realizando</param>
        /// <param name="pTipoElemento">Tipo de elemento que se está tratando</param>
        /// <param name="pPrioridad">Prioridad</param>
        /// <param name="pInfoExtra">Infomación extra</param>
        public void ActualizarGnossLiveMasivo(Guid pProyectoID, List<Guid> pListaElementoIDs, AccionLive pAccion, int pTipoElemento, bool pSoloPersonal, string pRutaBase, PrioridadLive pPrioridad, string pInfoExtra)
        {
            Dictionary<Guid, int> ElementoIDTipoElemento = new Dictionary<Guid, int>();
            foreach (Guid elementoID in pListaElementoIDs)
            {
                ElementoIDTipoElemento.Add(elementoID, pTipoElemento);
            }

            ActualizarGnossLive(pProyectoID, ElementoIDTipoElemento, pAccion, pSoloPersonal, pRutaBase, pPrioridad, pInfoExtra);
        }


        /// <summary>
        /// Añade a la cola de GnossLive un elemento para su procesamiento.
        /// </summary>
        /// <param name="pProyectoID">Proyecto al que pertenece el elemento</param>
        /// <param name="pElementoIDTipoElemento">Identificador del elemento que se está tratando y tipo de elemento</param>
        /// <param name="pAccion">Acción que se está realizando</param>
        /// <param name="pPrioridad">Prioridad</param>
        public void ActualizarGnossLive(Guid pProyectoID, Dictionary<Guid, int> pElementoIDTipoElemento, AccionLive pAccion, PrioridadLive pPrioridad)
        {
            ActualizarGnossLive(pProyectoID, pElementoIDTipoElemento, pAccion, false, pPrioridad);
        }


        /// <summary>
        /// Añade a la cola de GnossLive un elemento para su procesamiento.
        /// </summary>
        /// <param name="pProyectoID">Proyecto al que pertenece el elemento</param>
        /// <param name="pElementoIDTipoElemento">Identificador del elemento que se está tratando y tipo de elemento</param>
        /// <param name="pAccion">Acción que se está realizando</param>
        /// <param name="pPrioridad">Prioridad</param>
        /// <param name="pInfoExtra">Info extra</param>
        public void ActualizarGnossLive(Guid pProyectoID, Dictionary<Guid, int> pElementoIDTipoElemento, AccionLive pAccion, PrioridadLive pPrioridad, string pInfoExtra)
        {
            ActualizarGnossLive(pProyectoID, pElementoIDTipoElemento, pAccion, false, "base", pPrioridad, pInfoExtra);
        }

        /// <summary>
        /// Añade a la cola de GnossLive un elemento para su procesamiento.
        /// </summary>
        /// <param name="pProyectoID">Proyecto al que pertenece el elemento</param>
        /// <param name="pElementoIDTipoElemento">Identificador del elemento que se está tratando y tipo de elemento</param>
        /// <param name="pAccion">Acción que se está realizando</param>
        /// <param name="pSoloPersonal">Solo afeta a la actividad reciente del menú personal</param>
        /// <param name="pPrioridad">Prioridad</param>
        public void ActualizarGnossLive(Guid pProyectoID, Dictionary<Guid, int> pElementoIDTipoElemento, AccionLive pAccion, bool pSoloPersonal, PrioridadLive pPrioridad)
        {
            ActualizarGnossLive(pProyectoID, pElementoIDTipoElemento, pAccion, pSoloPersonal, "base", pPrioridad);
        }

        /// <summary>
        /// Añade a la cola de GnossLive un elemento para su procesamiento.
        /// </summary>
        /// <param name="pProyectoID">Proyecto al que pertenece el elemento</param>
        /// <param name="pElementoIDTipoElemento">Identificador del elemento que se está tratando con tipo de elemento</param>
        /// <param name="pAccion">Acción que se está realizando</param>
        /// <param name="pPrioridad">Prioridad</param>
        public void ActualizarGnossLive(Guid pProyectoID, Dictionary<Guid, int> pElementoIDTipoElemento, AccionLive pAccion, string rutaBase, PrioridadLive pPrioridad)
        {
            ActualizarGnossLive(pProyectoID, pElementoIDTipoElemento, pAccion, false, rutaBase, pPrioridad);
        }

        /// <summary>
        /// Añade a la cola de GnossLive un elemento para su procesamiento.
        /// </summary>
        /// <param name="pProyectoID">Proyecto al que pertenece el elemento</param>
        /// <param name="pElementoIDTipoElemento">Identificador del elemento que se está tratando con tipo de elemento</param>
        /// <param name="pAccion">Acción que se está realizando</param>
        /// <param name="pPrioridad">Prioridad</param>
        public void ActualizarGnossLive(Guid pProyectoID, Dictionary<Guid, int> pElementoIDTipoElemento, AccionLive pAccion, bool pSoloPersonal, string rutaBase, PrioridadLive pPrioridad)
        {
            ActualizarGnossLive(pProyectoID, pElementoIDTipoElemento, pAccion, pSoloPersonal, rutaBase, pPrioridad, null);
        }

        /// <summary>
        /// Añade a la cola de GnossLive un elemento para su procesamiento.
        /// </summary>
        /// <param name="pProyectoID">Proyecto al que pertenece el elemento</param>
        /// <param name="pElementoIDTipoElemento">Identificador del elemento que se está tratando y tipo de elemento</param>
        /// <param name="pAccion">Acción que se está realizando</param>
        /// <param name="pPrioridad">Prioridad</param>
        /// <param name="pInfoExtra">Infomación extra</param>
        public void ActualizarGnossLive(Guid pProyectoID, Dictionary<Guid, int> pElementoIDTipoElemento, AccionLive pAccion, bool pSoloPersonal, string pRutaBase, PrioridadLive pPrioridad, string pInfoExtra)
        {
            LiveCN liveCN;

            if (pRutaBase == "")
            {
                liveCN = new LiveCN(pRutaBase, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            }
            else
            {
                liveCN = new LiveCN("base", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            }

            LiveDS liveDS = new LiveDS();

            foreach (Guid elementoID in pElementoIDTipoElemento.Keys)
            {
                try
                {
                    InsertarFilaEnColaRabbitMQ(pProyectoID, elementoID, (int)pAccion, pElementoIDTipoElemento[elementoID], 0, DateTime.Now, pSoloPersonal, (short)pPrioridad, pInfoExtra);
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos 'BASE', tabla 'Cola'");
                    liveDS.Cola.AddColaRow(pProyectoID, elementoID, (int)pAccion, pElementoIDTipoElemento[elementoID], 0, DateTime.Now, pSoloPersonal, (short)pPrioridad, pInfoExtra);
                }

            }

            liveCN.ActualizarBD(liveDS);

            liveDS.Dispose();
        }


        /// <summary>
        /// Añade a la cola ColaPopularidad de GnossLive un elemento para su procesamiento.
        /// </summary>
        /// <param name="pProyectoID">Proyecto al que pertenece el elemento</param>
        /// <param name="pElementoID">ElementoID que se va a aumentar su popularidad</param>
        /// <param name="pIdentidadID">IdentidadID del usuario que ha aumentado la popularidad con alguna acción</param>
        /// <param name="pAccion">Acción que se está realizando</param>
        /// <param name="pTipoElemento">Tipo de elemento que se está tratando</param>
        /// <param name="pTipoElemento">Tipo del segundo elemento que se está tratando</param>
        /// <param name=param name="pPrioridad">Prioridad</param>
        public void ActualizarGnossLivePopularidad(Guid pProyectoID, Guid pElementoID, Guid pIdentidadID, AccionLive pAccion, int pTipoElemento, int pTipoElemento2, bool pSoloPersonal, PrioridadLive pPrioridad)
        {
            ActualizarGnossLivePopularidad(pProyectoID, pElementoID, pIdentidadID, pAccion, pTipoElemento, pTipoElemento2, pSoloPersonal, pPrioridad, null);
        }

        /// <summary>
        /// Añade a la cola ColaPopularidad de GnossLive un elemento para su procesamiento.
        /// </summary>
        /// <param name="pProyectoID">Proyecto al que pertenece el elemento</param>
        /// <param name="pElementoID">ElementoID que se va a aumentar su popularidad</param>
        /// <param name="pIdentidadID">IdentidadID del usuario que ha aumentado la popularidad con alguna acción</param>
        /// <param name="pAccion">Acción que se está realizando</param>
        /// <param name="pTipoElemento">Tipo de elemento que se está tratando</param>
        /// <param name="pTipoElemento">Tipo del segundo elemento que se está tratando</param>
        /// <param name=param name="pPrioridad">Prioridad</param>
        public void ActualizarGnossLivePopularidad(Guid pProyectoID, Guid pElementoID, Guid pIdentidadID, AccionLive pAccion, int pTipoElemento, int pTipoElemento2, bool pSoloPersonal, PrioridadLive pPrioridad, string pFicheroConfiguracion)
        {
            LiveCN liveCN;

            if (!string.IsNullOrEmpty(pFicheroConfiguracion))
            {
                liveCN = new LiveCN(pFicheroConfiguracion, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            }
            else
            {
                liveCN = new LiveCN("base", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            }


            LiveDS liveDS = new LiveDS();

            try
            {
                AniadirColaPopularidadRowRabbitMQ(pProyectoID, pElementoID, pIdentidadID, (int)pAccion, pTipoElemento, pTipoElemento2, 0, DateTime.Now, pSoloPersonal, (short)pPrioridad, null);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla ColaPopularidad");
                liveDS.ColaPopularidad.AddColaPopularidadRow(pProyectoID, pElementoID, pIdentidadID, (int)pAccion, pTipoElemento, pTipoElemento2, 0, DateTime.Now, pSoloPersonal, (short)pPrioridad, null);
            }

            liveCN.ActualizarBD(liveDS);

            liveDS.Dispose();
        }


        private void AniadirColaPopularidadRowRabbitMQ(Guid pProyectoID, Guid pElementoID, Guid pIdentidadID, int pAccion, int pTipoElemento, int pTipoElemento2, int pNumIntentos, DateTime pFecha, bool pSoloPersonal, short pPrioridad, string pInfoExtra)
        {
            LiveDS.ColaPopularidadRow colaPopularidad = new LiveDS().ColaPopularidad.NewColaPopularidadRow();

            colaPopularidad.ProyectoId = pProyectoID;
            colaPopularidad.Id = pElementoID;
            colaPopularidad.Id2 = pIdentidadID;
            colaPopularidad.Accion = pAccion;
            colaPopularidad.Tipo = pTipoElemento;
            colaPopularidad.Tipo2 = pTipoElemento2;
            colaPopularidad.NumIntentos = pNumIntentos;
            colaPopularidad.Fecha = pFecha;
            colaPopularidad.SoloPersonal = pSoloPersonal;
            colaPopularidad.Prioridad = pPrioridad;
            colaPopularidad.InfoExtra = pInfoExtra;

            string exchange = "";
            string nombreCola = "ColaPopularidad";


            if (mConfigService.ExistRabbitConnection(RabbitMQClient.BD_SERVICIOS_WIN))
            {
                using (RabbitMQClient rabbitMQ = new RabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN, nombreCola, mLoggingService, mConfigService, exchange, nombreCola))
                {
                    rabbitMQ.AgregarElementoACola(JsonConvert.SerializeObject(colaPopularidad.ItemArray));
                }
            }
        }

        #endregion

        #region Imagenes recursos

        /// <summary>
        /// Obtiene la ruta de la imagen de un blog.
        /// </summary>
        /// <returns>Ruta de la imagen de un blog</returns>
        public static string ObtenerURLImagenBlog()
        {
            string rutaImagenesExtensiones = "img/iconos";
            string nombreImagen = "blog";
            string rutaImagen = rutaImagenesExtensiones + "/" + nombreImagen + ".gif";

            return rutaImagen;
        }

        /// <summary>
        /// Obtiene la ruta de la imagen de un documento.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pBaseURLContent">Url content</param>
        /// <param name="pTieneImagen">Verdad si el documento tiene una imagen generada</param>
        /// <param name="pTamanio">Tamaño del icono</param>
        /// <param name="pVersionImagen">Version de la imagen (null si no es tipo foto o enlace)</param>
        /// <param name="pTamanioImagen">Tamaño de la imagen, por defecto</param>
        /// <returns>Ruta de la imagen de un documento</returns>
        public static string ObtenerRutaImagenDocumento(Documento pDocumento, string pBaseURLContent, string pBaseURLStatic, bool pTieneImagen, string pTamanio, int? pVersionImagen, string pTamanioImagen, Guid pProyectoID)
        {
            string nombreDocumento = null;

            if (pDocumento.EsFicheroDigital)
            {//Si no es de este tipo e intentamos acceder a esta propiedad fallará.
                nombreDocumento = pDocumento.NombreDocumento;
            }
            else if (pDocumento.TipoDocumentacion == TiposDocumentacion.Semantico && !string.IsNullOrEmpty(pDocumento.FilaDocumento.NombreCategoriaDoc))
            {
                nombreDocumento = pDocumento.FilaDocumento.NombreCategoriaDoc;
            }

            string modificador = null;

            if ((pDocumento.FilaDocumentoWebVinBR != null && pDocumento.FilaDocumentoWebVinBR.PrivadoEditores) || (!pDocumento.FilaDocumento.Publico && pProyectoID.Equals(ProyectoAD.MetaProyecto)))
            {
                modificador = "priv";
            }
            else
            {
                if (!pDocumento.PermiteComentarios && (pDocumento.TipoDocumentacion == TiposDocumentacion.Pregunta || pDocumento.TipoDocumentacion == TiposDocumentacion.Debate || pDocumento.TipoDocumentacion == TiposDocumentacion.Encuesta))
                {
                    modificador = "cerr";
                }
            }

            if (pDocumento.EsVideoIncrustado)
            {
                return ObtenerRutaImagenDocumento(pDocumento.Clave, "ejemplo.flv", TiposDocumentacion.Video, pBaseURLContent, pBaseURLStatic, pTieneImagen, pTamanio + modificador, pVersionImagen, pTamanioImagen);
            }
            else if (pDocumento.EsPresentacionIncrustada)
            {
                return ObtenerRutaImagenDocumento(pDocumento.Clave, "ejemplo.ppt", TiposDocumentacion.FicheroServidor, pBaseURLContent, pBaseURLStatic, pTieneImagen, pTamanio + modificador, pVersionImagen, pTamanioImagen);
            }

            return ObtenerRutaImagenDocumento(pDocumento.Clave, nombreDocumento, pDocumento.TipoDocumentacion, pBaseURLContent, pBaseURLStatic, pTieneImagen, pTamanio + modificador, pVersionImagen, pTamanioImagen);
        }


        /// <summary>
        /// Obtiene la ruta de la imagen de un documento.
        /// </summary>
        /// <param name="pDocumentoID">Documento</param>
        /// <param name="pNombreDoc">Nombre del documento</param>
        /// <param name="pTipoDocumento">Tipo de documento</param>
        /// <param name="pBaseURLContent">Url content</param>
        /// <param name="pTieneImagen">Verdad si el documento tiene una imagen generada</param>
        /// <param name="pComplemento">Texto que se añadirá a cada nombre de la imagen para variar ésta. Ej: Tamaño del documento si se especifica, null o Empty si es por defecto</param>
        /// <param name="pVersionImagen">Version de la imagen (null si no es tipo foto o enlace)</param>        
        /// <param name="pTamanioImagen">Tamaño de la imagen, por defecto</param>
        /// <returns>Ruta de la imagen de un documento</returns>
        public static string ObtenerRutaImagenDocumento(Guid pDocumentoID, string pNombreDoc, TiposDocumentacion pTipoDocumento, string pBaseURLContent, string pBaseURLStatic, bool pTieneImagen, string pComplemento, int? pVersionImagen, string pTamanioImagen)
        {
            string vFoto = "";
            if (pVersionImagen.HasValue)
            {
                vFoto = "?" + pVersionImagen.Value.ToString();
            }

            if (pTipoDocumento == TiposDocumentacion.Hipervinculo)
            {
                string fileName = HttpUtility.UrlEncode(pDocumentoID.ToString()) + ".jpg";

                if (pTieneImagen)
                {
                    return pBaseURLContent + "/" + UtilArchivos.ContentImagenesEnlaces + "/" + UtilArchivos.DirectorioDocumento(pDocumentoID) + "/" + fileName + vFoto;
                }
            }
            else if (pTipoDocumento == TiposDocumentacion.Semantico && !string.IsNullOrEmpty(pNombreDoc) && pNombreDoc.Contains(".jpg") && pTamanioImagen != null)
            {
                string fileName = pNombreDoc;

                if (fileName.Contains("|"))
                {
                    fileName = fileName.Split('|')[0];
                }

                if (fileName.IndexOf(pTamanioImagen + ",") == 0 || fileName.Contains("," + pTamanioImagen + ","))
                {
                    return pBaseURLContent + "/" + fileName.Substring(fileName.LastIndexOf(",") + 1).Replace(".jpg", "_" + pTamanioImagen + ".jpg");
                }
            }
            else if (pTipoDocumento == TiposDocumentacion.Imagen)
            {
                string fileName = pDocumentoID.ToString().ToLower() + "_peque.jpg";

                if (pTieneImagen)
                {
                    return pBaseURLContent + "/" + UtilArchivos.ContentImagenes + "/" + UtilArchivos.ContentImagenesDocumentos + "/" + "miniatura/" + UtilArchivos.DirectorioDocumento(pDocumentoID) + "/" + fileName + vFoto;
                }
            }

            string rutaImagenesExtensiones = "img/iconos";

            string extension = "";

            if ((pTipoDocumento == TiposDocumentacion.FicheroServidor || pTipoDocumento == TiposDocumentacion.Video || pTipoDocumento == TiposDocumentacion.Imagen) && !string.IsNullOrEmpty(pNombreDoc))
            {
                extension = pNombreDoc.Substring(pNombreDoc.LastIndexOf(".") + 1);
            }

            TiposDocumentacion tipoDocumento = pTipoDocumento;

            string nombreImagen = "generico";

            if (tipoDocumento == TiposDocumentacion.Hipervinculo)
            {
                nombreImagen = "web";
            }
            else if (tipoDocumento == TiposDocumentacion.Imagen)
            {
                nombreImagen = "img";
            }
            else if (tipoDocumento == TiposDocumentacion.Semantico)
            {
                nombreImagen = ObtenerNombreIconoDocumentoSem(pNombreDoc);
            }
            else if (tipoDocumento == TiposDocumentacion.EntradaBlog || tipoDocumento == TiposDocumentacion.EntradaBlogTemporal)
            {
                nombreImagen = "blog";
            }
            else if (tipoDocumento == TiposDocumentacion.Wiki || tipoDocumento == TiposDocumentacion.WikiTemporal)
            {
                nombreImagen = "wiki";
            }
            else if (tipoDocumento == TiposDocumentacion.Pregunta)
            {
                nombreImagen = "pregunta";
            }
            else if (tipoDocumento == TiposDocumentacion.Encuesta)
            {
                nombreImagen = "encuesta";
            }
            else if (tipoDocumento == TiposDocumentacion.Debate)
            {
                nombreImagen = "debate";
            }
            else if (tipoDocumento == TiposDocumentacion.Video || tipoDocumento == TiposDocumentacion.VideoBrightcove)
            {
                nombreImagen = "video";
            }

            if (nombreImagen == "generico" && tipoDocumento != TiposDocumentacion.ReferenciaADoc && tipoDocumento != TiposDocumentacion.Nota && tipoDocumento != TiposDocumentacion.Newsletter)
            {
                switch (extension)
                {
                    case "mpg":
                    case "mpge":
                    case "avi":
                    case "wma":
                    case "wmp":
                    case "flv":
                        nombreImagen = "video";
                        break;
                    case "wav":
                    case "mp2":
                    case "mp3":
                        nombreImagen = "audio";
                        break;
                    case "doc":
                    case "docx":
                    case "docm":
                    case "dot":
                    case "dotx":
                    case "dotm":
                    case "txt":
                    case "rtf":
                        nombreImagen = "doc";
                        break;
                    case "ppt":
                    case "pptx":
                    case "pptm":
                    case "pps":
                    case "ppsx":
                    case "ppsm":
                    case "pot":
                    case "potx":
                    case "potm":
                        nombreImagen = "presentacion";
                        break;
                    case "xls":
                    case "xlsx":
                    case "xlsb":
                    case "xlsm":
                    case "xlt":
                    case "xltx":
                    case "xltm":
                    case "csv":
                        nombreImagen = "calculo";
                        break;
                    case "pdf":
                        nombreImagen = "pdf";
                        break;
                    case "zip":
                    case "rar":
                    case "zip7":
                        nombreImagen = "zip";
                        break;
                }
            }

            string rutaImagen = "";

            extension += pComplemento;
            nombreImagen += pComplemento;

            if (nombreImagen == "generico" && tipoDocumento != TiposDocumentacion.ReferenciaADoc && tipoDocumento != TiposDocumentacion.Nota && tipoDocumento != TiposDocumentacion.Newsletter)
            {
                rutaImagen = pBaseURLStatic + "/" + rutaImagenesExtensiones + "/" + extension + ".gif";
            }
            else
            {
                rutaImagen = pBaseURLStatic + "/" + rutaImagenesExtensiones + "/" + nombreImagen + ".gif";
            }

            return rutaImagen;
        }

        /// <summary>
        /// Obtiene el nombre del icono de un documento semántico.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <returns>Nombre del icono de un documento semántico</returns>
        public string ObtenerNombreIconoDocumentoSem(Documento pDocumento)
        {
            string nombreCat = null;

            if (pDocumento != null && pDocumento.FilaDocumento.NombreCategoriaDoc != null)
            {
                nombreCat = pDocumento.FilaDocumento.NombreCategoriaDoc;
            }

            nombreCat = ObtenerNombreIconoDocumentoSem(nombreCat);



            return nombreCat + ObtenerNombreOntologiaParaIconoGnoss(pDocumento);
        }

        /// <summary>
        /// Obtiene el nombre del icono de un documento semántico a partir de su nombre de categoría.
        /// </summary>
        /// <param name="pNombreCatDoc">Nombre de categoría del recurso</param>
        /// <returns>Nombre del icono de un documento semántico a partir de su nombre de categoría</returns>
        public static string ObtenerNombreIconoDocumentoSem(string pNombreCatDoc)
        {
            if (!string.IsNullOrEmpty(pNombreCatDoc) && pNombreCatDoc.Contains("class="))
            {
                return pNombreCatDoc.Split('=')[1];
            }

            return "semantico";
        }

        /// <summary>
        /// Obtiene la URL de la imágen principal si la tiene de un recurso semántico.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pPage">Página</param>
        /// <returns></returns>
        public static string ObtenerNombreImagenPrincipalRecSem(Documento pDocumento, string pBaseURLContent)
        {
            if (pDocumento.TipoDocumentacion == TiposDocumentacion.Semantico && !string.IsNullOrEmpty(pDocumento.FilaDocumento.NombreCategoriaDoc) && pDocumento.FilaDocumento.NombreCategoriaDoc.Contains(".jpg"))
            {
                string fileName = pDocumento.FilaDocumento.NombreCategoriaDoc;

                if (fileName.Contains("|"))
                {
                    fileName = fileName.Split('|')[0];
                }

                return pBaseURLContent + "/" + fileName.Substring(fileName.LastIndexOf(",") + 1);
            }

            return null;
        }

        /// <summary>
        /// Obtiene el nombre de la ontología para el icono GNOSS.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <returns>Nombre de la ontología para el icono GNOSS</returns>
        public string ObtenerNombreOntologiaParaIconoGnoss(Documento pDocumento)
        {
            string ontologiaName = null;

            if (pDocumento == null)
            {
                return null;
            }

            if (!pDocumento.GestorDocumental.ListaDocumentos.ContainsKey(pDocumento.ElementoVinculadoID))
            {
                List<Guid> listaOntos = new List<Guid>();

                foreach (AD.EntityModel.Models.Documentacion.Documento filaDoc in pDocumento.GestorDocumental.DataWrapperDocumentacion.ListaDocumento.Where(doc => doc.Tipo.Equals((short)TiposDocumentacion.Semantico)))
                {
                    if (filaDoc.ElementoVinculadoID.HasValue && !listaOntos.Contains(filaDoc.ElementoVinculadoID.Value))
                    {
                        listaOntos.Add(filaDoc.ElementoVinculadoID.Value);
                    }
                }

                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
                dataWrapperDocumentacion.ListaDocumento = docCN.ObtenerDocumentosPorIDSoloDocumento(listaOntos);
                pDocumento.GestorDocumental.DataWrapperDocumentacion.Merge(dataWrapperDocumentacion);
                pDocumento.GestorDocumental.CargarDocumentos(false);
            }

            if (pDocumento.GestorDocumental.ListaDocumentos.ContainsKey(pDocumento.ElementoVinculadoID))
            {
                ontologiaName = pDocumento.GestorDocumental.ListaDocumentos[pDocumento.ElementoVinculadoID].Enlace;
            }

            if (ontologiaName != null)
            {
                ontologiaName = ontologiaName.Substring(0, ontologiaName.IndexOf("."));
                return " " + ontologiaName;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Indica si el documento es privado y la ruta de un documento es una captura web o una imagen en Miniatura.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pRuta">Ruta de la imagen</param>
        /// <returns>TRUE si el documento es privado y la ruta de un documento es una captura web o una imagen en Miniatura, 
        /// FALSE en caso contrario</returns>
        public static bool EsPrivadoYCapturaOMiniaturaRutaImagen(Documento pDocumento, string pRuta, Guid pProyectoID)
        {
            return ((pDocumento.FilaDocumentoWebVinBR != null && pDocumento.FilaDocumentoWebVinBR.PrivadoEditores) || (!pDocumento.FilaDocumento.Publico && pProyectoID.Equals(ProyectoAD.MetaProyecto))) && (pDocumento.TipoDocumentacion == TiposDocumentacion.Hipervinculo || pDocumento.TipoDocumentacion == TiposDocumentacion.Imagen || pDocumento.TipoDocumentacion == TiposDocumentacion.Semantico) && pRuta.Contains(pDocumento.Clave.ToString());
        }

        #endregion

        #region Documentación Entidades



        /// <summary>
        /// Devuelve la cadena de texto que hay que pasarle al servicio de documentación para indicarle la ruta correcta de un documento.
        /// </summary>
        /// <param name="pTipoEntidad">Tipo de entidad del documento</param>
        /// <returns>cadena de texto indicando la ruta correcta de un documento.</returns>
        public static string ObtenerTipoEntidadAdjuntarDocumento(TipoEntidadVinculadaDocumento pTipoEntidad)
        {
            string tipo = "";

            switch ((short)pTipoEntidad)
            {
                case (short)TipoEntidadVinculadaDocumento.Web:
                    tipo = TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS;
                    break;
                case (short)TipoEntidadVinculadaDocumento.Curriculum:
                    tipo = TipoEntidadVinculadaDocumentoTexto.CV_Acreditacion;
                    break;
            }

            return tipo;
        }

        #region Capturar Imagen Web

        /// <summary>
        /// Ordena al servicio correspondiente que capture la web a la que hace referencia un documento.
        /// </summary>
        /// <param name="pDocumento">Identificador del documento</param>
        /// <param name="pDocAgregado">Indica si el documento a sido agregado (TRUE), o es modificado (FALSE)</param>
        public void CapturarImagenWeb(Guid pDocumento, bool pDocAgregado, PrioridadColaDocumento pPrioridadColaDocumento)
        {
            CapturarImagenWeb(pDocumento, pDocAgregado, "", pPrioridadColaDocumento, -1);
        }

        /// <summary>
        /// Ordena al servicio correspondiente que capture la web a la que hace referencia un documento.
        /// </summary>
        /// <param name="pDocumento">Identificador del documento</param>
        /// <param name="pDocAgregado">Indica si el documento a sido agregado (TRUE), o es modificado (FALSE)</param>
        public void CapturarImagenWeb(Guid pDocumento, bool pDocAgregado, PrioridadColaDocumento pPrioridadColaDocumento, long pEstadoCargaID)
        {
            CapturarImagenWeb(pDocumento, pDocAgregado, "", pPrioridadColaDocumento, pEstadoCargaID);
        }

        /// <summary>
        /// Ordena al servicio correspondiente que capture la web a la que hace referencia un documento.
        /// </summary>
        /// <param name="pDocumento">Identificador del documento</param>
        /// <param name="pDocAgregado">Indica si el documento a sido agregado (TRUE), o es modificado (FALSE)</param>
        public void CapturarImagenWeb(Guid pDocumento, bool pDocAgregado, string pFicheroConfiguracion, PrioridadColaDocumento pPrioridadColaDocumento, long pEstadoCargaID)
        {
            DocumentacionCN docCN = null;
            if (pFicheroConfiguracion == "")
            {
                docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            }
            else
            {
                docCN = new DocumentacionCN(pFicheroConfiguracion, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            }
            docCN.AgregarDocumentoAColaTareas(pDocumento, pDocAgregado, pPrioridadColaDocumento, pEstadoCargaID);
        }

        #endregion

        #endregion


        #region Permisos

        /// <summary>
        /// Comprueba si una identidad tiene acceso a recurso.
        /// </summary>
        /// <param name="pIdentidad">Identidad</param>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pProyectoActual">Proyecto actual</param>
        /// <returns>TRUE si una identidad tiene acceso a recurso, FALSE si no</returns>
        public bool TieneAccesoIdentidadActualADocumento(Identidad pIdentidad, Documento pDocumento, Guid pProyectoActual)
        {
            List<Guid> listaComunidades = new List<Guid>();
            listaComunidades.AddRange(pIdentidad.ListaProyectosPerfilActual.Keys);

            if (pDocumento.FilaDocumentoWebVinBR.PrivadoEditores)
            {
                if (pDocumento.ListaPerfilesEditores.ContainsKey(pIdentidad.PerfilID))
                {
                    return true;
                }
            }
            else if (pDocumento.PerteneceDocumentoAComunidad(pProyectoActual))
            {
                return true;
            }
            else if (pDocumento.PerteneceDocumentoAComunidades(listaComunidades))
            {
                return true;
            }
            else
            {
                foreach (Guid proyectoID in pDocumento.ListaProyectos)
                {
                    AD.EntityModel.Models.ProyectoDS.Proyecto filaProy = ObtenerFilaProyecto(proyectoID);
                    if (proyectoID != pProyectoActual && !pIdentidad.ListaProyectosPerfilActual.ContainsKey(proyectoID) && (filaProy.TipoAcceso == (short)TipoAcceso.Publico || filaProy.TipoAcceso == (short)TipoAcceso.Restringido))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Comprueba el número de recursos vinculados que puede ver una identidad de un recurso.
        /// </summary>
        /// <param name="pIdentidad">Identidad</param>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pProyectoActual">Proyecto actual</param>
        /// <returns>Número de recursos vinculados que puede ver una identidad de un recurso</returns>
        public int NumeroRecusosVinculadosDocTienePermisoIdentidad(Identidad pIdentidad, Documento pDocumento, Guid pProyectoActual)
        {
            int count = 0;

            foreach (Guid docVincID in pDocumento.DocumentosVinculados)
            {
                if (TieneAccesoIdentidadActualADocumento(pIdentidad, pDocumento.GestorDocumental.ListaDocumentos[docVincID], pProyectoActual))
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Indica si un perfil es editor o lector de un documento.
        /// </summary>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <param name="pDocumento">Documento</param>
        /// <returns>TRUE si un perfil es editor o lector de un documento, FALSE en caso contrario</returns>
        public bool EsEditorOLectorPerfilDeDocumento(Guid pPerfilID, Documento pDocumento, Guid pUsuarioID)
        {
            return EsEditorPerfilDeDocumento(pPerfilID, pDocumento, false, pUsuarioID);
        }

        /// <summary>
        /// Indica si un perfil es editor o lector de un documento.
        /// </summary>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pSoloEditor">Comprobar solo si es editor</param>
        /// <returns>TRUE si un perfil es editor o lector de un documento, FALSE en caso contrario</returns>
        public bool EsEditorPerfilDeDocumento(Guid pPerfilID, Documento pDocumento, bool pSoloEditor, Guid pUsuarioID)
        {
            if (pDocumento.FilaDocumento.UltimaVersion)
            {
                if (!pSoloEditor && !pDocumento.FilaDocumentoWebVinBR.PrivadoEditores)
                {
                    //No es privado
                    return true;
                }
                else
                {
                    if (pDocumento.ListaPerfilesEditores.ContainsKey(pPerfilID) && (!pSoloEditor || pDocumento.ListaPerfilesEditoresSinLectores.ContainsKey(pPerfilID)))
                    {
                        //El perfil es editor
                        return true;
                    }
                    else//Comprobanmos si pertenece a un grupo editor
                    {
                        IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        GestionIdentidades gestoridentidades = new GestionIdentidades(identidadCN.ObtenerIdentidadesDePerfil(pPerfilID), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                        foreach (GrupoEditorRecurso grupoEditor in pDocumento.ListaGruposEditores.Values)
                        {
                            if (!pSoloEditor || pDocumento.ListaGruposEditoresSinLectores.ContainsKey(grupoEditor.Clave))
                            {
                                if (gestoridentidades.ListaIdentidades.Keys.Count > grupoEditor.listaIdentidadedsParticipacion.Count)
                                {
                                    List<Guid> listaUsuariosParticipacion = usuarioCN.ObtenerUsuariosPertenecenGrupo(grupoEditor.Clave, new List<Guid>() { pUsuarioID });

                                    if (listaUsuariosParticipacion.Count > 0)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    foreach (Guid identidadID in gestoridentidades.ListaIdentidades.Keys)
                                    {
                                        if (grupoEditor.listaIdentidadedsParticipacion.Contains(identidadID))
                                        {
                                            //El perfil tiene alguna identidad que pertenece a un grupo editor
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return false;
            }
            else //Miramos la última version:
            {
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                pDocumento.GestorDocumental.DataWrapperDocumentacion.Merge(docCN.ObtenerVersionesDocumentoPorID(pDocumento.Clave));
                pDocumento.GestorDocumental.DataWrapperDocumentacion.Merge(docCN.ObtenerEditoresDocumento(pDocumento.UltimaVersionID));

                if (pDocumento.GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Any(doc => doc.DocumentoID.Equals(pDocumento.UltimaVersionID) && doc.PrivadoEditores == false))
                {
                    return true;
                }

                if (pDocumento.GestorDocumental.DataWrapperDocumentacion.ListaDocumentoRolIdentidad.Any(doc => doc.DocumentoID.Equals(pDocumento.UltimaVersionID) && doc.PerfilID.Equals(pPerfilID)))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Indica si se debe pintar el check que restringe la visiblidad de un recurso para los miembros de la comunidad.
        /// </summary>
        /// <param name="pTipoRec">Tipo de recurso</param>
        /// <param name="pDocumento">Proyecto</param>
        /// <param name="pIdentidadActual">Identidad actual</param>
        /// <returns>TRUE si se debe pintar el check que restringe la visiblidad de un recurso para los miembros de la comunidad, FALSE si no</returns>
        public static bool PintarCheckVisibilidadMiembrosCom(TiposDocumentacion pTipoDocumento, bool pEsVideoIncrustado, Proyecto pProyecto, Identidad pIdentidadActual, Guid pUsuarioID)
        {
            return (pTipoDocumento == TiposDocumentacion.Imagen || pTipoDocumento == TiposDocumentacion.VideoBrightcove || pEsVideoIncrustado || pTipoDocumento == TiposDocumentacion.Audio || pTipoDocumento == TiposDocumentacion.AudioBrightcove || pTipoDocumento == TiposDocumentacion.FicheroServidor || pTipoDocumento == TiposDocumentacion.Semantico) && (pProyecto.TipoAcceso == TipoAcceso.Publico || pProyecto.TipoAcceso == TipoAcceso.Restringido) && pProyecto.EsAdministradorUsuario(pUsuarioID);
        }

        #endregion

        #region Generales

        /// <summary>
        /// Carga el documento pasado como parámetro.
        /// </summary>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <returns>Documento</returns>
        public Documento CargarDocumento(Guid pDocumentoID, Guid pProyectoID, Identidad pIdentidadOrganizacion, Guid pUsuarioID, Guid pOrganizacionID)
        {
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);

            GestorDocumental gestorDoc = new GestorDocumental(new DataWrapperDocumentacion(), mLoggingService, mEntityContext);

            #region Cargamos la base de recursos del usuario

            if (pIdentidadOrganizacion == null)
            {
                if (pProyectoID == ProyectoAD.MetaProyecto)
                {
                    docCN.ObtenerBaseRecursosUsuario(gestorDoc.DataWrapperDocumentacion, pUsuarioID);
                }
                else
                {
                    docCL.ObtenerBaseRecursosProyecto(gestorDoc.DataWrapperDocumentacion, pProyectoID, pOrganizacionID, pUsuarioID);
                }
            }
            else
            {
                docCL.ObtenerBaseRecursosOrganizacion(gestorDoc.DataWrapperDocumentacion, (Guid)pIdentidadOrganizacion.PerfilUsuario.OrganizacionID, pProyectoID);
            }

            if (gestorDoc.DataWrapperDocumentacion.ListaBaseRecursos.Count > 0)
            {
                docCN.ObtenerDocumentoPorIDCargarTotal(pDocumentoID, gestorDoc.DataWrapperDocumentacion, true, true, gestorDoc.BaseRecursosIDActual);

                //Si no hay fila, hay que consultar BD Master por si acaso no está en una réplica:
                if (gestorDoc.DataWrapperDocumentacion.ListaDocumento.Count == 0)
                {
                    docCN.ObtenerDocumentoPorIDCargarTotal(pDocumentoID, gestorDoc.DataWrapperDocumentacion, true, true, gestorDoc.BaseRecursosIDActual);
                }
            }

            #endregion

            #region Cargamos el tesauro

            TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            if (pIdentidadOrganizacion == null && pProyectoID == ProyectoAD.MetaProyecto)
            {
                gestorDoc.GestorTesauro = new GestionTesauro(tesauroCN.ObtenerTesauroUsuario(pUsuarioID), mLoggingService, mEntityContext);
            }
            else if (pProyectoID != ProyectoAD.MetaProyecto)
            {
                gestorDoc.GestorTesauro = new GestionTesauro(new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication).ObtenerTesauroDeProyecto(pProyectoID), mLoggingService, mEntityContext);
            }
            else
            {
                if (pIdentidadOrganizacion != null)
                {
                    gestorDoc.GestorTesauro = new GestionTesauro(tesauroCN.ObtenerTesauroOrganizacion((Guid)pIdentidadOrganizacion.PerfilUsuario.OrganizacionID), mLoggingService, mEntityContext);
                }
            }

            #endregion

            gestorDoc.CargarDocumentos(false);

            Documento documento = gestorDoc.ListaDocumentos[pDocumentoID];

            TesauroCL tesCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            gestorDoc.GestorTesauro.TesauroDW.Merge(tesCL.ObtenerCategoriasPermitidasPorTipoRecurso(gestorDoc.GestorTesauro.TesauroActualID, pProyectoID));
            tesCL.Dispose();

            gestorDoc.GestorTesauro.EliminarCategoriasNoPermitidasPorTipoDoc((short)documento.TipoDocumentacion, null);

            docCL.Dispose();

            return documento;
        }

        /// <summary>
        /// Obtiene la URl de un recurso.
        /// </summary>
        /// <param name="pDocumento"></param>
        /// <param name="pProyecto"></param>
        /// <returns></returns>
        public string ObtenerURLDocumento(Documento pDocumento, Proyecto pProyecto, Identidad pIdentidadActual, bool pEsIdentOrg, string pBaseUrl, UtilIdiomas pUtilIdiomas, string pUrlPerfil)
        {
            string enlace = null;

            if (pDocumento.EsBorrador && pIdentidadActual.Clave != pDocumento.CreadorID)
            {
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                GestionIdentidades gestIdentidades = new GestionIdentidades(identidadCN.ObtenerIdentidadPorID(pDocumento.CreadorID, true), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                if (pDocumento.ProyectoID == ProyectoAD.MetaProyecto)
                {
                    enlace = GnossUrlsSemanticas.GetURLBaseRecursosFichaMyGnoss(pBaseUrl, pUtilIdiomas, pUrlPerfil, gestIdentidades.ListaIdentidades[pDocumento.CreadorID], pDocumento);
                }
                else
                {
                    enlace = UrlsSemanticas.GetURLBaseRecursosFicha(pBaseUrl, pUtilIdiomas, ObtenerFilaProyecto(pDocumento.ProyectoID).NombreCorto, pUrlPerfil, pDocumento, (pEsIdentOrg && pIdentidadActual.IdentidadOrganizacion != null));
                }
            }
            else
            {
                if (pProyecto.Clave != ProyectoAD.MetaProyecto)
                {
                    enlace = UrlsSemanticas.GetURLBaseRecursosFicha(pBaseUrl, pUtilIdiomas, pProyecto.NombreCorto, "", pDocumento, (pEsIdentOrg && pIdentidadActual.IdentidadOrganizacion != null));
                }
                else
                {
                    enlace = GnossUrlsSemanticas.GetURLBaseRecursosRecursoInvitado(pBaseUrl, pUrlPerfil, pUtilIdiomas, pDocumento);
                }
            }

            return enlace;
        }

        #endregion

        #region Google Analytic

        /// <summary>
        /// Devuelve la función javascript para registrar en Google Analytic una acción.
        /// </summary>
        /// <param name="pAccion">Acción</param>
        /// <param name="pLabel">Label identificador del recurso accionado</param>
        /// <returns>Función javascript para registrar en Google Analytic una acció</returns>
        public static string AcccionGoogleAnalytic(AccionGoogleAnalytic pAccion, string pLabel)
        {
            string funcion = "EnviarAccGogAnac('Acciones sociales','";

            switch (pAccion)
            {
                case AccionGoogleAnalytic.AniadirRecCat:
                    funcion += "Añadir recurso a categoría";
                    break;
                case AccionGoogleAnalytic.AniadirRecTag:
                    funcion += "Añadir etiquetas";
                    break;
                case AccionGoogleAnalytic.Comentar:
                    funcion += "Comentar";
                    break;
                case AccionGoogleAnalytic.Compartir:
                    funcion += "Compartir";
                    break;
                case AccionGoogleAnalytic.DejarSeguirA:
                    funcion += "Dejar de seguir a";
                    break;
                case AccionGoogleAnalytic.DesCompartir:
                    funcion += "Descompartir";
                    break;
                case AccionGoogleAnalytic.DesVincular:
                    funcion += "Desvincular recurso";
                    break;
                case AccionGoogleAnalytic.EnviarEnlace:
                    funcion += "Enviar enlace";
                    break;
                case AccionGoogleAnalytic.GuardarEnFav:
                    funcion += "Guardar en mis favoritos";
                    break;
                case AccionGoogleAnalytic.SeguirA:
                    funcion += "Seguir a";
                    break;
                case AccionGoogleAnalytic.VincularRec:
                    funcion += "Vincular recurso";
                    break;
                case AccionGoogleAnalytic.VotarNeg:
                    funcion += "Votar Negativamente";
                    break;
                case AccionGoogleAnalytic.VotarPos:
                    funcion += "Votar";
                    break;
                default:
                    funcion = "";
                    break;
            }

            if (!string.IsNullOrEmpty(funcion))
            {
                funcion += "','" + pLabel + "');";
            }

            return funcion;
        }

        #endregion Google Analytic

        #region Caché

        /// <summary>
        /// Borra la caché del control de la ficha de un recurso.
        /// </summary>
        /// <param name="pDocumentoID">ID del documento</param>
        public void BorrarCacheControlFichaRecursos(Guid pDocumentoID)
        {
            DocumentacionCL docCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            docCL.BorrarControlFichaRecursos(pDocumentoID);
            docCL.Dispose();
        }

        /// <summary>
        /// Borra la caché del control de comentarios de la ficha de un recurso.
        /// </summary>
        /// <param name="pDocumentoID">ID del documento</param>
        public void BorrarCacheControlComentariosFichaRecursos(Guid pDocumentoID)
        {
            DocumentacionCL docCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            docCL.BorrarControlComentariosFichaRecursos(pDocumentoID);
            docCL.Dispose();
        }

        /// <summary>
        /// Borra la caché del control de recursos vinculados de la ficha de un recurso.
        /// </summary>
        /// <param name="pDocumentoID">ID del documento</param>
        public void BorrarCacheControlVinculadosFichaRecursos(Guid pDocumentoID)
        {
            DocumentacionCL docCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            docCL.BorrarControlVinculadosFichaRecursos(pDocumentoID);
            docCL.Dispose();
        }

        #endregion Caché

        #region Tesauros semánticos

        /// <summary>
        /// Crea una categoría en un tesauro semántico.
        /// </summary>
        /// <param name="pUrlOntologiaTesauro">URl de la ontología del tesauro</param>
        /// <param name="pUrlIntragnoss">Url de intragnoss</param>
        /// <param name="pCategoria">Categoría a crear</param>
        /// <param name="pCatPadres">IDs de los padres de la categoría</param>
        /// <param name="pDocOntologia">Documento de la ontología</param>
        /// <param name="pUsarColaReplicacion">Indica si se debe usar cola de replicación al guardar</param>
        /// <param name="pPropsExtra">Nombre de las propiedades extra</param>
        public void CrearCategoriaTesauroSemantico(string pUrlOntologiaTesauro, string pUrlIntragnoss, ElementoOntologia pCategoria, List<string> pCatPadres, Documento pDocOntologia, bool pUsarColaReplicacion, List<string> pPropsExtra, Guid pProyectoID)
        {
            string[] arrayTesSem = ObtenerDatosFacetaTesSem(pUrlOntologiaTesauro);
            FacetadoCN facCN = new FacetadoCN(pUrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

            List<string> padresAux = new List<string>();

            foreach (string padre in pCatPadres)
            {
                if (!padre.StartsWith("http://"))
                {
                    padresAux.Add(pUrlIntragnoss + "items/" + padre);
                }
                else
                {
                    padresAux.Add(padre);
                }
            }

            pCatPadres = padresAux;

            List<string> propiedades = new List<string>();
            propiedades.Add(arrayTesSem[2]);//identifier

            FacetadoDS dataSetCategorias = facCN.ObtenerValoresPropiedadesEntidad(pUrlOntologiaTesauro, pCategoria.Uri, propiedades);

            if (dataSetCategorias.Tables[0].Rows.Count > 0)
            {
                throw new Exception("La categoría ya existe.");
            }

            if (pCatPadres.Count > 0)
            {
                dataSetCategorias.Merge(facCN.ObtenerValoresPropiedadesEntidades(pUrlOntologiaTesauro, pCatPadres, propiedades, true));

                foreach (string padre in pCatPadres)
                {
                    if (dataSetCategorias.Tables[0].Select("s='" + padre + "'").Length == 0)
                    {
                        throw new Exception("La categoría padre '" + padre + "' no existe.");
                    }
                }
            }

            string triplesInsertar = "";
            List<TripleWrapper> triplesComInsertar = new List<TripleWrapper>();

            //Agrego las propiedades de la categoría:
            AgregarPropiedadANuevaCategoriaTesSem(pCategoria, arrayTesSem[2], ref triplesInsertar, triplesComInsertar);//identifier
            AgregarPropiedadANuevaCategoriaTesSem(pCategoria, arrayTesSem[3], ref triplesInsertar, triplesComInsertar);//prefLabel
            AgregarPropiedadANuevaCategoriaTesSem(pCategoria, arrayTesSem[5], ref triplesInsertar, triplesComInsertar);//source
            AgregarPropiedadANuevaCategoriaTesSem(pCategoria, arrayTesSem[6], ref triplesInsertar, triplesComInsertar);//symbol

            //Agrego Type:
            EscribirTripletaEntidad(pCategoria.Uri, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", pCategoria.TipoEntidad, ref triplesInsertar, triplesComInsertar, false, null);

            //Agrego Label:
            EscribirTripletaEntidad(pCategoria.Uri, "http://www.w3.org/2000/01/rdf-schema#label", "literal@" + pCategoria.TipoEntidad, ref triplesInsertar, triplesComInsertar, false, null);

            //Agrego las propiedades extra:
            if (pPropsExtra != null)
            {
                foreach (string propExtra in pPropsExtra)
                {
                    AgregarPropiedadANuevaCategoriaTesSem(pCategoria, propExtra, ref triplesInsertar, triplesComInsertar, false);
                }
            }

            //Agrego la propiedad hasEntidad de control de entidades secundarias:
            if (pCatPadres.Count > 0)
            {
                FacetadoDS dataSetFilaHasEntidad = facCN.ObtenerTripletasConObjeto(pUrlOntologiaTesauro, pCatPadres[0]);

                List<string> sujHasEntidad = FacetadoCN.ObtenerSujetosDataSetSegunPropiedadYObjeto(dataSetFilaHasEntidad, "http://gnoss/hasEntidad", pCatPadres[0]);

                foreach (string sujeto in sujHasEntidad)
                {
                    EscribirTripletaEntidad(sujeto, "http://gnoss/hasEntidad", pCategoria.Uri, ref triplesInsertar, triplesComInsertar, false, null);
                }

                dataSetFilaHasEntidad.Dispose();

                //Agregamos la triple que relaciona los padres con la nueva categoría:
                foreach (string padre in pCatPadres)
                {
                    EscribirTripletaEntidad(pCategoria.Uri, arrayTesSem[7], padre, ref triplesInsertar, triplesComInsertar, false, null);
                    EscribirTripletaEntidad(padre, arrayTesSem[4], pCategoria.Uri, ref triplesInsertar, triplesComInsertar, false, null);
                }
            }
            else
            {
                string source = pCategoria.ObtenerPropiedad(arrayTesSem[5]).PrimerValorPropiedad;
                List<string> objetos = new List<string>();
                objetos.Add(source);
                objetos.Add("literal@" + arrayTesSem[8]);
                List<string> sujetosCollection = facCN.ObtenerSujetosConObjetos(pUrlOntologiaTesauro, objetos);

                if (sujetosCollection.Count == 0)
                {
                    throw new Exception("No existe ninguna colección cuya fuente sea '" + source + "'.");
                }

                FacetadoDS dataSetFilaHasEntidad = facCN.ObtenerTripletasConObjeto(pUrlOntologiaTesauro, sujetosCollection[0]);

                List<string> sujHasEntidad = FacetadoCN.ObtenerSujetosDataSetSegunPropiedadYObjeto(dataSetFilaHasEntidad, "http://gnoss/hasEntidad", sujetosCollection[0]);

                //Agrego la propiedad hasEntidad de control de entidades secundarias:
                foreach (string sujeto in sujHasEntidad)
                {
                    EscribirTripletaEntidad(sujeto, "http://gnoss/hasEntidad", pCategoria.Uri, ref triplesInsertar, triplesComInsertar, false, null);
                }

                dataSetFilaHasEntidad.Dispose();

                //Agrego los member:
                foreach (string sujeto in sujetosCollection)
                {
                    EscribirTripletaEntidad(sujeto, arrayTesSem[1], pCategoria.Uri, ref triplesInsertar, triplesComInsertar, false, null);
                }
            }

            //Genero las triples de la comunidad:
            string triplesComInsertarDef = "";
            List<string> listaAux = new List<string>();
            Dictionary<string, string> parametroProyecto = ObtenerParametroProyecto(pProyectoID);
            bool textoTesauroInvariable = parametroProyecto.ContainsKey("TextoInvariableTesauroSemantico") && parametroProyecto["TextoInvariableTesauroSemantico"] == "1";

            foreach (TripleWrapper triple in triplesComInsertar)
            {
                if (triple.Predicate == "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>" || triple.Predicate == "<http://www.w3.org/2000/01/rdf-schema#label>")
                {//El type y el label no van al grafo de la comunidad
                    continue;
                }

                string objeto = triple.Object;
                if (!triple.Predicate.Equals("<http://www.w3.org/2008/05/skos#prefLabel>") || !textoTesauroInvariable)
                {
                    objeto = PasarObjetoALower(triple.Object, listaAux);
                }

                string tripleta = new FacetadoAD("", mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).GenerarTripletaSinConversionesAbsurdas(PasarObjetoALower(triple.Subject, listaAux), triple.Predicate, objeto, triple.ObjectLanguage);

                if (triple.Predicate == "<" + arrayTesSem[6] + ">")//symbol
                {
                    tripleta = AgrearTipoEnteroObjetoTriple(tripleta);
                }

                triplesComInsertarDef += tripleta;
            }

            #region Guardamos los datos

            string nombreOntologia = pDocOntologia.FilaDocumento.Enlace;

            //Guardo triples grafo tesauro semántico:
            InsertaTripletasVirtuoso(pUrlIntragnoss, nombreOntologia, triplesInsertar, PrioridadBase.ApiRecursos, pUsarColaReplicacion);

            //Guardo triples en los proyectos en los que está subido y compartido el tesauro semántico:
            foreach (Guid proyectoID in pDocOntologia.ListaProyectos)
            {
                InsertaTripletasVirtuoso(pUrlIntragnoss, proyectoID.ToString().ToLower(), triplesComInsertarDef, PrioridadBase.ApiRecursos, pUsarColaReplicacion);
            }

            #endregion

            dataSetCategorias.Dispose();
        }

        /// <summary>
        /// Renombra una categoría del tesauro semántico.
        /// </summary>
        /// <param name="pUrlOntologiaTesauro">URl de la ontología del tesauro</param>
        /// <param name="pUrlIntragnoss">Url de intragnoss</param>
        /// <param name="pCategoriaID">ID de la categoría a renombrar</param>
        /// <param name="pNombre">Nombre nuevo</param>
        /// <param name="pDocOntologia">Documento de la ontología</param>
        /// <param name="pUsarColaReplicacion">Indica si se debe usar cola de replicación al guardar</param>
        public void RenombrarCategoriaTesauroSemantico(string pUrlOntologiaTesauro, string pUrlIntragnoss, string pCategoriaID, string pNombre, Documento pDocOntologia, bool pUsarColaReplicacion, Guid pProyectoID)
        {
            string[] arrayTesSem = ObtenerDatosFacetaTesSem(pUrlOntologiaTesauro);
            FacetadoCN facCN = new FacetadoCN(pUrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

            List<string> propiedades = new List<string>();
            propiedades.Add(arrayTesSem[3]);//prefLabel

            string triplesBorrar = "";
            string triplesInsertar = "";
            List<TripleWrapper> triplesComBorrar = new List<TripleWrapper>();
            List<TripleWrapper> triplesComInsertar = new List<TripleWrapper>();

            //Obtengo categoría:
            List<string> ent = new List<string>();
            ent.Add(pCategoriaID);
            FacetadoDS dataSetCategorias = facCN.ObtenerValoresPropiedadesEntidades(pUrlOntologiaTesauro, ent, propiedades, true, false);

            if (dataSetCategorias.Tables[0].Rows.Count == 0)
            {
                throw new Exception("No existe una categoría con esa URI.");
            }

            //Borramos todas las tripletas de la categoría a elimianar:
            foreach (DataRow fila in dataSetCategorias.Tables[0].Rows)
            {
                string idioma = null;

                if (fila.ItemArray.Length > 3 && !fila.IsNull(3) && !string.IsNullOrEmpty((string)fila[3]))
                {
                    idioma = (string)fila[3];
                }

                EscribirTripletaEntidad(pCategoriaID, arrayTesSem[3], (string)fila[2], ref triplesBorrar, triplesComBorrar, false, idioma);
            }

            Dictionary<string, string> nombreIdiomas = UtilCadenas.ObtenerTextoPorIdiomas(pNombre);

            if (nombreIdiomas.Count > 0)
            {
                foreach (string idioma in nombreIdiomas.Keys)
                {
                    EscribirTripletaEntidad(pCategoriaID, arrayTesSem[3], nombreIdiomas[idioma], ref triplesInsertar, triplesComInsertar, false, idioma);
                }
            }
            else
            {
                EscribirTripletaEntidad(pCategoriaID, arrayTesSem[3], pNombre, ref triplesInsertar, triplesComInsertar, false, null);
            }

            //Genero las triples de la comunidad:
            string triplesComInsertarDef = "";
            //string triplesComBorrarDef = "";
            List<TripleWrapper> triplesComBorrarDef = new List<TripleWrapper>();
            List<string> listaAux = new List<string>();
            Dictionary<string, string> parametroProyecto = ObtenerParametroProyecto(pProyectoID);
            bool textoTesauroInvariable = parametroProyecto.ContainsKey("TextoInvariableTesauroSemantico") && parametroProyecto["TextoInvariableTesauroSemantico"] == "1";

            foreach (TripleWrapper triple in triplesComInsertar)
            {
                string objeto = triple.Object;
                if (!triple.Predicate.Equals("<http://www.w3.org/2008/05/skos#prefLabel>") || !textoTesauroInvariable)
                {
                    objeto = PasarObjetoALower(triple.Object, listaAux);
                }

                triplesComInsertarDef += new FacetadoAD("", mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).GenerarTripletaSinConversionesAbsurdas(PasarObjetoALower(triple.Subject, listaAux), triple.Predicate, objeto, triple.ObjectLanguage);
            }

            foreach (TripleWrapper triple in triplesComBorrar)
            {
                string objeto = triple.Object;
                if (!triple.Predicate.Equals("<http://www.w3.org/2008/05/skos#prefLabel>") || !textoTesauroInvariable)
                {
                    objeto = PasarObjetoALower(triple.Object, listaAux);
                }
                else if (triple.Predicate.Equals("<http://www.w3.org/2008/05/skos#prefLabel>"))
                {
                    triplesComBorrarDef.Add(new TripleWrapper { Subject = PasarObjetoALower(triple.Subject, listaAux), Predicate = triple.Predicate, Object = PasarObjetoALower(triple.Object, listaAux), ObjectLanguage = triple.ObjectLanguage });
                }

                triplesComBorrarDef.Add(new TripleWrapper { Subject = PasarObjetoALower(triple.Subject, listaAux), Predicate = triple.Predicate, Object = objeto, ObjectLanguage = triple.ObjectLanguage });
            }

            #region Guardamos los datos

            //Guardo tesauro:

            string nombreOntologia = pDocOntologia.FilaDocumento.Enlace;

            //Guardo triples grafo tesauro semántico:
             BorrarGrupoTripletasEnListaVirtuoso(pUrlIntragnoss, nombreOntologia, triplesComBorrar, pUsarColaReplicacion);
            InsertaTripletasVirtuoso(pUrlIntragnoss, nombreOntologia, triplesInsertar, PrioridadBase.ApiRecursos, pUsarColaReplicacion);

            //Guardo triples en los proyectos en los que está subido y compartido el tesauro semántico:
            foreach (Guid proyectoID in pDocOntologia.ListaProyectos)
            {
                BorrarGrupoTripletasEnListaVirtuoso(pUrlIntragnoss, proyectoID.ToString().ToLower(), triplesComBorrarDef, pUsarColaReplicacion);
                InsertaTripletasVirtuoso(pUrlIntragnoss, proyectoID.ToString().ToLower(), triplesComInsertarDef, PrioridadBase.ApiRecursos, pUsarColaReplicacion);
            }

            dataSetCategorias.Dispose();

            #endregion

        }

        /// <summary>
        /// Renombra una categoría del tesauro semántico.
        /// </summary>
        /// <param name="pUrlOntologiaTesauro">URl de la ontología del tesauro</param>
        /// <param name="pUrlIntragnoss">Url de intragnoss</param>
        /// <param name="pCategoriaID">ID de la categoría a renombrar</param>
        /// <param name="pCategoria">Categoría</param>
        /// <param name="pPropsExtra">Propiedades extra de la categoría</param>
        /// <param name="pDocOntologia">Documento de la ontología</param>
        /// <param name="pUsarColaReplicacion">Indica si se debe usar cola de replicación al guardar</param>
        public void EditarPropiedadesExtraCategoriaTesauroSemantico(string pUrlOntologiaTesauro, string pUrlIntragnoss, string pCategoriaID, ElementoOntologia pCategoria, List<string> pPropsExtra, Documento pDocOntologia, bool pUsarColaReplicacion, Guid pProyectoID)
        {
            string[] arrayTesSem = ObtenerDatosFacetaTesSem(pUrlOntologiaTesauro);
            FacetadoCN facCN = new FacetadoCN(pUrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

            List<string> propiedades = new List<string>(pPropsExtra);
            propiedades.Add(arrayTesSem[3]);//prefLabel

            string triplesBorrar = "";
            string triplesInsertar = "";
            List<TripleWrapper> triplesComBorrar = new List<TripleWrapper>();
            List<TripleWrapper> triplesComInsertar = new List<TripleWrapper>();

            //Obtengo categoría:
            List<string> ent = new List<string>();
            ent.Add(pCategoriaID);
            FacetadoDS dataSetCategorias = facCN.ObtenerValoresPropiedadesEntidades(pUrlOntologiaTesauro, ent, propiedades, true);

            if (dataSetCategorias.Tables[0].Rows.Count == 0)
            {
                throw new Exception("No existe una categoría con esa URI.");
            }

            //Borramos todas las tripletas de la categoría a elimianar:
            foreach (DataRow fila in dataSetCategorias.Tables[0].Rows)
            {
                if ((string)fila[1] == arrayTesSem[3])//prefLabel
                {
                    continue;
                }

                string idioma = null;

                if (fila.ItemArray.Length > 3 && !fila.IsNull(3) && !string.IsNullOrEmpty((string)fila[3]))
                {
                    idioma = (string)fila[3];
                }

                EscribirTripletaEntidad(pCategoriaID, (string)fila[1], (string)fila[2], ref triplesBorrar, triplesComBorrar, false, idioma);
            }

            //Agrego las propiedades extra:
            if (pPropsExtra != null)
            {
                foreach (string propExtra in pPropsExtra)
                {
                    AgregarPropiedadANuevaCategoriaTesSem(pCategoria, propExtra, ref triplesInsertar, triplesComInsertar, false);
                }
            }

            //Genero las triples de la comunidad:
            string triplesComInsertarDef = "";
            //string triplesComBorrarDef = "";
            List<TripleWrapper> triplesComBorrarDef = new List<TripleWrapper>();
            List<string> listaAux = new List<string>();
            Dictionary<string, string> parametroProyecto = ObtenerParametroProyecto(pProyectoID);
            bool textoTesauroInvariable = parametroProyecto.ContainsKey("TextoInvariableTesauroSemantico") && parametroProyecto["TextoInvariableTesauroSemantico"] == "1";

            foreach (TripleWrapper triple in triplesComInsertar)
            {
                string objeto = triple.Object;
                if (!triple.Predicate.Equals("<http://www.w3.org/2008/05/skos#prefLabel>") || !textoTesauroInvariable)
                {
                    objeto = PasarObjetoALower(triple.Object, listaAux);
                }

                triplesComInsertarDef += new FacetadoAD("", mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).GenerarTripletaSinConversionesAbsurdas(PasarObjetoALower(triple.Subject, listaAux), triple.Predicate, objeto, triple.ObjectLanguage);
            }

            foreach (TripleWrapper triple in triplesComBorrar)
            {
                string objeto = triple.Object;
                if (!triple.Predicate.Equals("<http://www.w3.org/2008/05/skos#prefLabel>") || !textoTesauroInvariable)
                {
                    objeto = PasarObjetoALower(triple.Object, listaAux);
                }
                else if (triple.Predicate.Equals("<http://www.w3.org/2008/05/skos#prefLabel>"))
                {
                    triplesComBorrarDef.Add(new TripleWrapper { Subject = PasarObjetoALower(triple.Subject, listaAux), Predicate = triple.Predicate, Object = PasarObjetoALower(triple.Object, listaAux), ObjectLanguage = triple.ObjectLanguage });
                }

                triplesComBorrarDef.Add(new TripleWrapper { Subject = PasarObjetoALower(triple.Subject, listaAux), Predicate = triple.Predicate, Object = objeto, ObjectLanguage = triple.ObjectLanguage });
            }

            #region Guardamos los datos

            //Guardo tesauro:

            string nombreOntologia = pDocOntologia.FilaDocumento.Enlace;

            //Guardo triples grafo tesauro semántico:
            BorrarGrupoTripletasEnListaVirtuoso(pUrlIntragnoss, nombreOntologia, triplesComBorrar, pUsarColaReplicacion);
            InsertaTripletasVirtuoso(pUrlIntragnoss, nombreOntologia, triplesInsertar, PrioridadBase.ApiRecursos, pUsarColaReplicacion);

            //Guardo triples en los proyectos en los que está subido y compartido el tesauro semántico:
            foreach (Guid proyectoID in pDocOntologia.ListaProyectos)
            {
                BorrarGrupoTripletasEnListaVirtuoso(pUrlIntragnoss, proyectoID.ToString().ToLower(), triplesComBorrarDef, pUsarColaReplicacion);
                InsertaTripletasVirtuoso(pUrlIntragnoss, proyectoID.ToString().ToLower(), triplesComInsertarDef, PrioridadBase.ApiRecursos, pUsarColaReplicacion);
            }

            dataSetCategorias.Dispose();

            #endregion

        }

        /// <summary>
        /// Mueve una categoría del tesauro semántico de un padre a otro.
        /// </summary>
        /// <param name="pUrlOntologiaTesauro">URl de la ontología del tesauro</param>
        /// <param name="pUrlOntologiaRecursos">Url de la ontología de los recursos</param>
        /// <param name="pUrlIntragnoss">Url de intragnoss</param>
        /// <param name="pCategoriaAMoverID">ID de la categoría a mover</param>
        /// <param name="pPath">IDs de las nuevas categorías padre jerarquicamente ordenadas</param>
        /// <param name="pDocOntologia">Documento de la ontología</param>
        /// <param name="pDocOntologiaRecursos">Documento de la ontología de recursos</param>
        /// <param name="pUsarColaReplicacion">Indica si se debe usar cola de replicación al guardar</param>
        public void MoverCategoriaTesauroSemantico(string pUrlOntologiaTesauro, string pUrlOntologiaRecursos, string pUrlIntragnoss, string pCategoriaAMoverID, string[] pPath, Documento pDocOntologia, Documento pDocOntologiaRecursos, bool pUsarColaReplicacion, Guid pProyectoID)
        {
            string[] arrayTesSem = ObtenerDatosFacetaTesSem(pUrlOntologiaTesauro);
            FacetadoCN facCN = new FacetadoCN(pUrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

            List<string> propiedades = new List<string>();
            propiedades.Add(arrayTesSem[4]);//hasHijo
            propiedades.Add(arrayTesSem[7]);//hasPadre
            propiedades.Add(arrayTesSem[5]);//source
            propiedades.Add(arrayTesSem[6]);//symbol

            string triplesBorrar = "";
            string triplesInsertar = "";
            List<TripleWrapper> triplesComBorrar = new List<TripleWrapper>();
            List<TripleWrapper> triplesComInsertar = new List<TripleWrapper>();

            //Obtengo categoría:
            FacetadoDS dataSetCategorias = facCN.ObtenerValoresPropiedadesEntidad(pUrlOntologiaTesauro, pCategoriaAMoverID, propiedades);

            if (dataSetCategorias.Tables[0].Rows.Count == 0)
            {
                throw new Exception("No existe una categoría con esa URI.");
            }

            //Obtengo padres categoría:
            List<string> padres = FacetadoCN.ObtenerObjetosDataSetSegunPropiedad(dataSetCategorias, pCategoriaAMoverID, arrayTesSem[7]);

            //if (padres.Count == 0)
            //{
            //    facCN.Dispose();
            //    throw new Exception("No se puede mover una categoría Raiz.");
            //}

            if (padres.Count > 0)
            {
                //Desvinculo la relacion entra la categoría y sus antiguos padres:
                foreach (string padreID in padres)
                {
                    EscribirTripletaEntidad(pCategoriaAMoverID, arrayTesSem[7], padreID, ref triplesBorrar, triplesComBorrar, false, null);
                    EscribirTripletaEntidad(padreID, arrayTesSem[4], pCategoriaAMoverID, ref triplesBorrar, triplesComBorrar, false, null);
                }
            }
            else //Hay que eliminar vinculación con la colección, ya que es Raíz:
            {
                FacetadoDS dataSetFilaHasEntidad = facCN.ObtenerTripletasConObjeto(pUrlOntologiaTesauro, pCategoriaAMoverID);
                List<string> sujHasEntidad = FacetadoCN.ObtenerSujetosDataSetSegunPropiedadYObjeto(dataSetFilaHasEntidad, arrayTesSem[1], pCategoriaAMoverID);

                foreach (string sujeto in sujHasEntidad)
                {
                    EscribirTripletaEntidad(sujeto, arrayTesSem[1], pCategoriaAMoverID, ref triplesBorrar, triplesComBorrar, false, null);
                }

                dataSetFilaHasEntidad.Dispose();
                dataSetFilaHasEntidad = null;
            }

            //Elimino el orden para agregar el nuevo más tarde:
            List<string> symbols = FacetadoCN.ObtenerObjetosDataSetSegunPropiedad(dataSetCategorias, pCategoriaAMoverID, arrayTesSem[6]);//source

            foreach (string symbol in symbols)
            {
                EscribirTripletaEntidad(pCategoriaAMoverID, arrayTesSem[6], symbol, ref triplesBorrar, triplesComBorrar, false, null);
            }

            //Obtengo nuevos padres para la categoría:
            List<string> nuevosPadres = null;

            if (pPath != null)
            {
                nuevosPadres = new List<string>(pPath);
            }
            else
            {
                nuevosPadres = new List<string>();
            }

            foreach (string nuevoPadre in nuevosPadres)
            {
                if (string.IsNullOrEmpty(nuevoPadre))
                {
                    throw new Exception("El Path no puede contener elementos vacíos o nulos.");
                }
            }

            //if (nuevosPadres.Count == 0)
            //{
            //    facCN.Dispose();
            //    throw new Exception("El Path debe tener al menos un elemento.");
            //}

            if (padres.Count == 0 && nuevosPadres.Count == 0)
            {
                throw new Exception("Estas intentando mover una categoría Raíz para que sea Raíz, eso no tiene sentido.");
            }

            if (nuevosPadres.Count > 0)
            {
                dataSetCategorias.Merge(facCN.ObtenerValoresPropiedadesEntidades(pUrlOntologiaTesauro, nuevosPadres, propiedades));

                //Compruebo que las categorías padres nuevas son correctas:
                int count = 0;

                foreach (string padreNuevo in nuevosPadres)
                {
                    List<string> padresDePadre = FacetadoCN.ObtenerObjetosDataSetSegunPropiedad(dataSetCategorias, padreNuevo, arrayTesSem[7]);

                    if (count == 0 && padresDePadre.Count > 0)
                    {
                        throw new Exception("La 1º categoría del Path '" + padreNuevo + "' no es Raiz.");
                    }
                    else if (count > 0 && padresDePadre.Count == 0)
                    {
                        throw new Exception("La categoría del Path '" + padreNuevo + "' no existe.");
                    }

                    count++;
                }

                string nuevoPadre = nuevosPadres[nuevosPadres.Count - 1];

                EscribirTripletaEntidad(pCategoriaAMoverID, arrayTesSem[7], nuevoPadre, ref triplesInsertar, triplesComInsertar, false, null);
                EscribirTripletaEntidad(nuevoPadre, arrayTesSem[4], pCategoriaAMoverID, ref triplesInsertar, triplesComInsertar, false, null);
            }
            else
            {
                List<string> sources = FacetadoCN.ObtenerObjetosDataSetSegunPropiedad(dataSetCategorias, pCategoriaAMoverID, arrayTesSem[5]);//source

                if (sources.Count == 0 || string.IsNullOrEmpty(sources[0]))
                {
                    throw new Exception("La categoría no posee la propiedad 'http://purl.org/dc/elements/1.1/source' por lo que no se pude inferir cual es Colección.");
                }

                List<string> objetos = new List<string>();
                objetos.Add(sources[0]);
                objetos.Add("literal@" + arrayTesSem[8]);
                List<string> sujetosCollection = facCN.ObtenerSujetosConObjetos(pUrlOntologiaTesauro, objetos);

                if (sujetosCollection.Count == 0)
                {
                    throw new Exception("No existe ninguna Colección cuya fuente sea '" + sources[0] + "'.");
                }

                //Agrego los member:
                foreach (string sujeto in sujetosCollection)
                {
                    EscribirTripletaEntidad(sujeto, arrayTesSem[1], pCategoriaAMoverID, ref triplesInsertar, triplesComInsertar, false, null);
                }
            }

            //Agrego el nuevo symbol de la categoría:
            int nuevoSymbol = nuevosPadres.Count + 1;
            EscribirTripletaEntidad(pCategoriaAMoverID, arrayTesSem[6], nuevoSymbol.ToString(), ref triplesInsertar, triplesComInsertar, false, null);

            //Genero las triples de la comunidad:
            string triplesComInsertarDef = "";
            List<TripleWrapper> triplesComBorrarDef = new List<TripleWrapper>();
            List<string> listaAux = new List<string>();
            Dictionary<string, string> parametroProyecto = ObtenerParametroProyecto(pProyectoID);
            bool textoTesauroInvariable = parametroProyecto.ContainsKey("TextoInvariableTesauroSemantico") && parametroProyecto["TextoInvariableTesauroSemantico"] == "1";

            foreach (TripleWrapper triple in triplesComInsertar)
            {
                string objeto = triple.Object;
                if (!triple.Predicate.Equals("<http://www.w3.org/2008/05/skos#prefLabel>") || !textoTesauroInvariable)
                {
                    objeto = PasarObjetoALower(triple.Object, listaAux);
                }

                string tripleta = new FacetadoAD("", mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).GenerarTripletaSinConversionesAbsurdas(PasarObjetoALower(triple.Subject, listaAux), triple.Predicate, objeto, triple.ObjectLanguage);

                if (triple.Predicate == "<" + arrayTesSem[6] + ">")//symbol
                {
                    tripleta = AgrearTipoEnteroObjetoTriple(tripleta);
                }

                triplesComInsertarDef += tripleta;
            }

            foreach (TripleWrapper triple in triplesComBorrar)
            {
                string extraObjeto = null;

                if (triple.Predicate == "<" + arrayTesSem[6] + ">")//symbol
                {
                    extraObjeto = "^^xsd:int";
                }

                string objeto = triple.Object;
                if (!triple.Predicate.Equals("<http://www.w3.org/2008/05/skos#prefLabel>") || !textoTesauroInvariable)
                {
                    objeto = PasarObjetoALower(triple.Object, listaAux);
                }
                else if (triple.Predicate.Equals("<http://www.w3.org/2008/05/skos#prefLabel>"))
                {
                    triplesComBorrarDef.Add(new TripleWrapper { Subject = PasarObjetoALower(triple.Subject, listaAux), Predicate = triple.Predicate, Object = PasarObjetoALower(triple.Object, listaAux) + extraObjeto, ObjectLanguage = triple.ObjectLanguage });
                }

                //triplesComBorrarDef += FacetadoAD.GenerarTripletaSinConversionesAbsurdas(PasarObjetoALower(triple[0], listaAux), triple[1], PasarObjetoALower(triple[2], listaAux), idioma);
                triplesComBorrarDef.Add(new TripleWrapper { Subject = PasarObjetoALower(triple.Subject, listaAux), Predicate = triple.Predicate, Object = objeto + extraObjeto, ObjectLanguage = triple.ObjectLanguage });
            }


            #region Modifico Recursos afectados

            List<string> catMoverEHijos = new List<string>();
            List<string> propiedadesHija = new List<string>();
            propiedadesHija.Add(arrayTesSem[4]);
            ObtenerCategoriasHijosRecursivosTesSem(catMoverEHijos, pCategoriaAMoverID, propiedadesHija, facCN, pUrlOntologiaTesauro);

            Dictionary<Guid, Dictionary<string, string>> docPaths = new Dictionary<Guid, Dictionary<string, string>>();
            Dictionary<Guid, string> triplesInsertarRec = new Dictionary<Guid, string>();
            string triplesBorrarRec = "";
            List<TripleWrapper> listaTripAux = new List<TripleWrapper>();
            FacetadoDS dataSetPaths = null;
            DocumentacionCN docCN = new DocumentacionCN("acid", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<string> predicadosCatValidos = new List<string>();
            Dictionary<Guid, Guid> docsElemV = null;
            Dictionary<Guid, string> elemVinGrafo = null;

            if (string.IsNullOrEmpty(pUrlOntologiaRecursos))
            {
                //Obenermos los IDs del grafo de la comunidad:
                dataSetPaths = facCN.ObtenerTripletasDeSujetoConObjeto(pDocOntologia.FilaDocumento.ProyectoID.ToString().ToLower(), pCategoriaAMoverID.ToLower());

                //Extraigo los predicados válidos:
                foreach (DataRow fila in dataSetPaths.Tables[0].Select("o='" + pCategoriaAMoverID.ToLower() + "'"))
                {
                    if (!predicadosCatValidos.Contains((string)fila[1]))
                    {
                        predicadosCatValidos.Add((string)fila[1]);
                    }
                }

                List<Guid> docsAfectados = new List<Guid>();

                foreach (DataRow fila in dataSetPaths.Tables[0].Rows)
                {
                    try
                    {
                        if (!predicadosCatValidos.Contains((string)fila[1]))
                        {
                            continue;
                        }

                        string sujeto = (string)fila[0];
                        string predicado = (string)fila[1];
                        string objeto = (string)fila[2];

                        if (sujeto.Contains("_"))
                        {
                            string docID = sujeto.Substring(0, sujeto.LastIndexOf("_"));
                            if (docID.Contains("_"))
                            {
                                docID = docID.Substring(docID.LastIndexOf("_") + 1);
                                Guid documentoID = Guid.Empty;

                                if (Guid.TryParse(docID, out documentoID) && !docsAfectados.Contains(documentoID))
                                {
                                    docsAfectados.Add(documentoID);
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                //Obtenermos los grafos de los recursos afectados
                elemVinGrafo = new Dictionary<Guid, string>();
                docsElemV = docCN.ObtenerListaRecursosConElementoVinculadoID(docsAfectados);

                foreach (Guid elemVin in docsElemV.Values)
                {
                    if (!elemVinGrafo.ContainsKey(elemVin))
                    {
                        elemVinGrafo.Add(elemVin, docCN.ObtenerEnlaceDocumentoPorDocumentoID(elemVin));
                    }
                }

                //Traemos los triples de los grafos de las ontologías de los recursos afectados:
                dataSetPaths = new FacetadoDS();

                foreach (string grafo in elemVinGrafo.Values)
                {
                    dataSetPaths.Merge(facCN.ObtenerTripletasDeSujetoConObjeto(pUrlIntragnoss + grafo, pCategoriaAMoverID));
                }
            }
            else
            {
                dataSetPaths = facCN.ObtenerTripletasDeSujetoConObjeto(pUrlOntologiaRecursos, pCategoriaAMoverID);
            }

            if (dataSetPaths.Tables.Count > 0)
            {
                predicadosCatValidos = new List<string>();

                //Extraigo los predicados válidos:
                foreach (DataRow fila in dataSetPaths.Tables[0].Select("o='" + pCategoriaAMoverID + "'"))
                {
                    if (!predicadosCatValidos.Contains((string)fila[1]))
                    {
                        predicadosCatValidos.Add((string)fila[1]);
                    }
                }

                foreach (DataRow fila in dataSetPaths.Tables[0].Rows)
                {
                    try
                    {
                        if (!predicadosCatValidos.Contains((string)fila[1]))
                        {
                            continue;
                        }

                        string sujeto = (string)fila[0];
                        string predicado = (string)fila[1];
                        string objeto = (string)fila[2];
                        string docID = sujeto.Substring(0, sujeto.LastIndexOf("_"));
                        docID = docID.Substring(docID.LastIndexOf("_") + 1);
                        Guid documentoID = new Guid(docID);

                        if (!docPaths.ContainsKey(documentoID))
                        {
                            docPaths.Add(documentoID, new Dictionary<string, string>());
                        }

                        if (!docPaths[documentoID].ContainsKey(sujeto))
                        {
                            docPaths[documentoID].Add(sujeto, predicado);
                        }

                        if (!catMoverEHijos.Contains(objeto)) //Borramos las triples de los padres que ya no lo serán
                        {
                            EscribirTripletaEntidad(sujeto, predicado, objeto, ref triplesBorrarRec, listaTripAux, false, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Los datos del recurso con Path '" + (string)fila[0] + "' que depende de la categoría son incorrectos: " + Environment.NewLine + ex.ToString());
                    }
                }
            }

            docsElemV = null;

            if (pDocOntologiaRecursos != null)
            {
                triplesInsertarRec.Add(Guid.Empty, "");
            }
            else //Hay que volver a generar los grafos, no te puedes fiar de lo que haya en el de comunidad.
            {
                docsElemV = docCN.ObtenerListaRecursosConElementoVinculadoID(new List<Guid>(docPaths.Keys));
            }

            if (nuevosPadres.Count > 0)
            {
                //Agrego a cada Path los nuevos padres:
                foreach (Guid documentoID in docPaths.Keys)
                {
                    Guid elemV = Guid.Empty;

                    if (docsElemV != null)
                    {
                        elemV = docsElemV[documentoID];

                        if (!triplesInsertarRec.ContainsKey(elemV))
                        {
                            triplesInsertarRec.Add(elemV, "");
                        }
                    }

                    string triplesIn = triplesInsertarRec[elemV];

                    foreach (string sujeto in docPaths[documentoID].Keys)
                    {
                        foreach (string padre in nuevosPadres)
                        {
                            EscribirTripletaEntidad(sujeto, docPaths[documentoID][sujeto], padre, ref triplesIn, listaTripAux, false, null);
                        }
                    }

                    triplesInsertarRec[elemV] = triplesIn;
                }
            }

            #endregion

            #region Guardamos los datos

            //Guardo tesauro:

            Dictionary<Guid, KeyValuePair<short, List<Guid>>> docsTipoProys = docCN.ObtenerTipoYProyectosDocumentos(new List<Guid>(docPaths.Keys));

            string nombreOntologia = pDocOntologia.FilaDocumento.Enlace;

            //Guardo triples grafo tesauro semántico:
            BorrarGrupoTripletasEnListaVirtuoso(pUrlIntragnoss, nombreOntologia, triplesComBorrar, pUsarColaReplicacion);
            InsertaTripletasVirtuoso(pUrlIntragnoss, nombreOntologia, triplesInsertar, PrioridadBase.ApiRecursos, pUsarColaReplicacion);

            //Guardo triples en los proyectos en los que está subido y compartido el tesauro semántico:
            foreach (Guid proyectoID in pDocOntologia.ListaProyectos)
            {
                BorrarGrupoTripletasEnListaVirtuoso(pUrlIntragnoss, proyectoID.ToString().ToLower(), triplesComBorrarDef, pUsarColaReplicacion);
                InsertaTripletasVirtuoso(pUrlIntragnoss, proyectoID.ToString().ToLower(), triplesComInsertarDef, PrioridadBase.ApiRecursos, pUsarColaReplicacion);
            }


            //Guardo Recursos:
            elemVinGrafo = new Dictionary<Guid, string>();

            if (pDocOntologiaRecursos != null)
            {
                elemVinGrafo.Add(Guid.Empty, pDocOntologiaRecursos.FilaDocumento.Enlace);
            }

            foreach (Guid elementVinID in triplesInsertarRec.Keys)
            {
                if (!elemVinGrafo.ContainsKey(elementVinID))
                {
                    elemVinGrafo.Add(elementVinID, docCN.ObtenerEnlaceDocumentoPorDocumentoID(elementVinID));
                }

                if (!string.IsNullOrEmpty(triplesBorrarRec))
                {
                    BorrarGrupoTripletasVirtuoso(pUrlIntragnoss, elemVinGrafo[elementVinID], triplesBorrarRec, pUsarColaReplicacion);
                }
                if (!string.IsNullOrEmpty(triplesInsertarRec[elementVinID]))
                {
                    InsertaTripletasVirtuoso(pUrlIntragnoss, elemVinGrafo[elementVinID], triplesInsertarRec[elementVinID], PrioridadBase.ApiRecursos, pUsarColaReplicacion);
                }
            }


            foreach (Guid documentoID in docsTipoProys.Keys)
            {
                foreach (Guid proyID in docsTipoProys[documentoID].Value)
                {
                    //Inserto en el base los recursos:
                    AgregarRecursoModeloBaseSimple(documentoID, proyID, docsTipoProys[documentoID].Key, PrioridadBase.ApiRecursos);
                }

                //Borro RDF SQL Server de los recursos:
                BorrarRDFDeBDRDF(documentoID);
            }

            dataSetCategorias.Dispose();
            dataSetPaths.Dispose();

            pDocOntologia.GestorDocumental.Dispose();

            if (pDocOntologiaRecursos != null)
            {
                pDocOntologiaRecursos.GestorDocumental.Dispose();
            }

            #endregion

        }

        /// <summary>
        /// Agrega el tipo entero al objeto de un triple ya formado.
        /// </summary>
        /// <param name="pTriple">Triple</param>
        /// <returns>Triple con el objeto convertido a entero</returns>
        public static string AgrearTipoEnteroObjetoTriple(string pTriple)
        {
            return pTriple.Substring(0, pTriple.LastIndexOf("\"") + 1) + "^^xsd:int" + pTriple.Substring(pTriple.LastIndexOf("\"") + 1);
        }

        /// <summary>
        /// Elimina una categoría del tesauro semántico de un padre a otro.
        /// </summary>
        /// <param name="pUrlOntologiaTesauro">URl de la ontología del tesauro</param>
        /// <param name="pUrlOntologiaRecursos">Url de la ontología de los recursos</param>
        /// <param name="pUrlIntragnoss">Url de intragnoss</param>
        /// <param name="pBaseURLFormulariosSem">Base Url para los formularios semánticos</param>
        /// <param name="pCategoriaAEliminarID">ID de la categoría a eliminar</param>
        /// <param name="pDocOntologia">Documento de la ontología</param>
        /// <param name="pDocOntologiaRecursos">Documento de la ontología de recursos</param>
        /// <param name="pUsarColaReplicacion">Indica si se debe usar cola de replicación al guardar</param>
        public void EliminarCategoriaTesauroSemantico(string pUrlOntologiaTesauro, string pUrlOntologiaRecursos, string pUrlIntragnoss, string pBaseURLFormulariosSem, string pCategoriaAEliminarID, Documento pDocOntologia, Documento pDocOntologiaRecursos, bool pUsarColaReplicacion, Guid pProyectoID)
        {
            FacetadoCN facCN = new FacetadoCN(pUrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            string[] arrayTesSem = ControladorDocumentacion.ObtenerDatosFacetaTesSem(pUrlOntologiaTesauro);

            string triplesBorrar = "";
            string triplesInsertar = "";
            List<TripleWrapper> triplesComBorrar = new List<TripleWrapper>();
            List<TripleWrapper> triplesComInsertar = new List<TripleWrapper>();

            //Obtengo categoría:
            FacetadoDS dataSetCategorias = facCN.ObtenerValoresPropiedadesEntidad(pUrlOntologiaTesauro, pCategoriaAEliminarID, true);

            if (dataSetCategorias.Tables[0].Rows.Count == 0)
            {
                throw new Exception("No existe una categoría con esa URI.");
            }

            List<string> hijos = FacetadoCN.ObtenerObjetosDataSetSegunPropiedad(dataSetCategorias, pCategoriaAEliminarID, arrayTesSem[4]);

            if (hijos.Count > 0)
            {
                throw new Exception("La categoría tiene hijos, solo se pueden eliminar categorías sin descendencia.");
            }

            List<string> padres = FacetadoCN.ObtenerObjetosDataSetSegunPropiedad(dataSetCategorias, pCategoriaAEliminarID, arrayTesSem[7]);
                        
            //Borramos la relación entre el padre e hijo:
            foreach (string padreID in padres)
            {
                EscribirTripletaEntidad(padreID, arrayTesSem[4], pCategoriaAEliminarID, ref triplesBorrar, triplesComBorrar, false, null);
            }

            //Borramos todas las tripletas de la categoría a elimianar:
            foreach (DataRow fila in dataSetCategorias.Tables[0].Rows)
            {
                string idioma = null;

                if (fila.ItemArray.Length > 3 && !fila.IsNull(3) && !string.IsNullOrEmpty((string)fila[3]))
                {
                    idioma = (string)fila[3];
                }

                string objeto = (string)fila[2];

                if ((string)fila[1] == "http://www.w3.org/2000/01/rdf-schema#label" && objeto.StartsWith("http"))
                {
                    objeto = "literal@" + objeto + "";
                }

                EscribirTripletaEntidad(pCategoriaAEliminarID, (string)fila[1], objeto, ref triplesBorrar, triplesComBorrar, false, idioma);
            }

            //Obtengo fila relación entSecond hasEntidad para eliminarla:
            FacetadoDS dataSetFilaHasEntidad = facCN.ObtenerTripletasConObjeto(pUrlOntologiaTesauro, pCategoriaAEliminarID);

            List<string> sujHasEntidad = FacetadoCN.ObtenerSujetosDataSetSegunPropiedadYObjeto(dataSetFilaHasEntidad, "http://gnoss/hasEntidad", pCategoriaAEliminarID);

            foreach (string sujeto in sujHasEntidad)
            {
                EscribirTripletaEntidad(sujeto, "http://gnoss/hasEntidad", pCategoriaAEliminarID, ref triplesBorrar, triplesComBorrar, false, null);
            }

            if (padres.Count == 0) //Hay que eliminar vinculación con la colección, ya que es Raíz:
            {
                sujHasEntidad = FacetadoCN.ObtenerSujetosDataSetSegunPropiedadYObjeto(dataSetFilaHasEntidad, arrayTesSem[1], pCategoriaAEliminarID);

                foreach (string sujeto in sujHasEntidad)
                {
                    EscribirTripletaEntidad(sujeto, arrayTesSem[1], pCategoriaAEliminarID, ref triplesBorrar, triplesComBorrar, false, null);
                }
            }

            dataSetFilaHasEntidad.Dispose();

            //Genero las triples de la comunidad:
            string triplesComInsertarDef = "";
            //string triplesComBorrarDef = "";
            List<TripleWrapper> triplesComBorrarDef = new List<TripleWrapper>();
            List<string> listaAux = new List<string>();
            Dictionary<string, string> parametroProyecto = ObtenerParametroProyecto(pProyectoID);
            bool textoTesauroInvariable = parametroProyecto.ContainsKey("TextoInvariableTesauroSemantico") && parametroProyecto["TextoInvariableTesauroSemantico"] == "1";

            foreach (TripleWrapper triple in triplesComInsertar)
            {
                string objeto = triple.Object;
                if (!triple.Predicate.Equals("<http://www.w3.org/2008/05/skos#prefLabel>") || !textoTesauroInvariable)
                {
                    objeto = PasarObjetoALower(triple.Object, listaAux);
                }

                string tripleta = new FacetadoAD("", mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).GenerarTripletaSinConversionesAbsurdas(PasarObjetoALower(triple.Subject, listaAux), triple.Predicate, objeto, triple.ObjectLanguage);

                if (triple.Predicate == "<" + arrayTesSem[6] + ">")//symbol
                {
                    tripleta = AgrearTipoEnteroObjetoTriple(tripleta);
                }

                triplesComInsertarDef += tripleta;
            }

            foreach (TripleWrapper triple in triplesComBorrar)
            {
                string extraObjeto = null;

                if (triple.Predicate == $"<{arrayTesSem[6]}>")//symbol
                {
                    extraObjeto = "^^xsd:int";
                }

                string objeto = triple.Object;
                if (!triple.Predicate.Equals("<http://www.w3.org/2008/05/skos#prefLabel>") || !textoTesauroInvariable)
                {
                    objeto = PasarObjetoALower(triple.Object, listaAux);
                }
                else if (triple.Predicate.Equals("<http://www.w3.org/2008/05/skos#prefLabel>"))
                {
                    triplesComBorrarDef.Add(new TripleWrapper() { Subject = PasarObjetoALower(triple.Subject, listaAux), Predicate = triple.Predicate, Object = PasarObjetoALower(triple.Object, listaAux) + extraObjeto, ObjectLanguage = triple.ObjectLanguage });
                }

                triplesComBorrarDef.Add(new TripleWrapper { Subject = PasarObjetoALower(triple.Subject, listaAux), Predicate = triple.Predicate, Object = objeto + extraObjeto, ObjectLanguage = triple.ObjectLanguage });
            }

            #region Modifico Recursos afectados

            Dictionary<Guid, Dictionary<string, string>> docPaths = new Dictionary<Guid, Dictionary<string, string>>();
            List<string> sujetosPath = new List<string>();
            Dictionary<Guid, string> triplesInsertarRec = new Dictionary<Guid, string>();
            string triplesBorrarRec = "";
            List<TripleWrapper> triplesBorrarRecAux = new List<TripleWrapper>();
            List<TripleWrapper> litTripAux = new List<TripleWrapper>();
            FacetadoDS dataSetPaths = null;
            DocumentacionCN docCN = new DocumentacionCN("acid", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<string> predicadosCatValidos = new List<string>();
            Dictionary<Guid, Guid> docsElemV = null;
            Dictionary<Guid, string> elemVinGrafo = null;

            if (string.IsNullOrEmpty(pUrlOntologiaRecursos))
            {
                //Obenermos los IDs del grafo de la comunidad:
                dataSetPaths = facCN.ObtenerTripletasDeSujetoConObjeto(pDocOntologia.FilaDocumento.ProyectoID.ToString().ToLower(), pCategoriaAEliminarID.ToLower());

                //Extraigo los predicados válidos:
                foreach (DataRow fila in dataSetPaths.Tables[0].Select("o='" + pCategoriaAEliminarID.ToLower() + "'"))
                {
                    if (!predicadosCatValidos.Contains((string)fila[1]))
                    {
                        predicadosCatValidos.Add((string)fila[1]);
                    }
                }

                List<Guid> docsAfectados = new List<Guid>();

                foreach (DataRow fila in dataSetPaths.Tables[0].Rows)
                {
                    try
                    {
                        if (!predicadosCatValidos.Contains((string)fila[1]))
                        {
                            continue;
                        }

                        string sujeto = (string)fila[0];
                        string predicado = (string)fila[1];
                        string objeto = (string)fila[2];

                        if (sujeto.Contains("_"))
                        {
                            string docID = sujeto.Substring(0, sujeto.LastIndexOf("_"));
                            if (docID.Contains("_"))
                            {
                                docID = docID.Substring(docID.LastIndexOf("_") + 1);
                                Guid documentoID = Guid.Empty;

                                if (Guid.TryParse(docID, out documentoID) && !docsAfectados.Contains(documentoID))
                                {
                                    docsAfectados.Add(documentoID);
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                //Obtenermos los grafos de los recursos afectados
                elemVinGrafo = new Dictionary<Guid, string>();
                docsElemV = docCN.ObtenerListaRecursosConElementoVinculadoID(docsAfectados);

                foreach (Guid elemVin in docsElemV.Values)
                {
                    if (!elemVinGrafo.ContainsKey(elemVin))
                    {
                        elemVinGrafo.Add(elemVin, docCN.ObtenerEnlaceDocumentoPorDocumentoID(elemVin));
                    }
                }

                //Traemos los triples de los grafos de las ontologías de los recursos afectados:
                dataSetPaths = new FacetadoDS();

                foreach (string grafo in elemVinGrafo.Values)
                {
                    dataSetPaths.Merge(facCN.ObtenerTripletasDeSujetoConObjeto(pUrlIntragnoss + grafo, pCategoriaAEliminarID));
                }
            }
            else
            {
                dataSetPaths = facCN.ObtenerTripletasDeSujetoConObjeto(pUrlOntologiaRecursos, pCategoriaAEliminarID);
            }

            if (dataSetPaths.Tables.Count > 0)
            {
                predicadosCatValidos = new List<string>();

                //Extraigo los predicados válidos:
                foreach (DataRow fila in dataSetPaths.Tables[0].Select("o='" + pCategoriaAEliminarID + "'"))
                {
                    if (!predicadosCatValidos.Contains((string)fila[1]))
                    {
                        predicadosCatValidos.Add((string)fila[1]);
                    }
                }

                foreach (DataRow fila in dataSetPaths.Tables[0].Rows)
                {
                    try
                    {
                        if (!predicadosCatValidos.Contains((string)fila[1]))
                        {//Solo hay que conservar los triples que no son de categorías sin hay padres y luego añadimos las categorías nuevas.
                            continue;
                        }

                        string sujeto = (string)fila[0];
                        string predicado = (string)fila[1];
                        string objeto = (string)fila[2];

                        if (predicadosCatValidos.Contains((string)fila[1]))
                        {
                            string docID = sujeto.Substring(0, sujeto.LastIndexOf("_"));
                            docID = docID.Substring(docID.LastIndexOf("_") + 1);
                            Guid documentoID = new Guid(docID);

                            if (!docPaths.ContainsKey(documentoID))
                            {
                                docPaths.Add(documentoID, new Dictionary<string, string>());
                            }

                            if (!docPaths[documentoID].ContainsKey(sujeto))
                            {
                                docPaths[documentoID].Add(sujeto, predicado);
                            }

                            if (!sujetosPath.Contains(sujeto))
                            {
                                sujetosPath.Add(sujeto);
                            }
                        }

                        if (predicado == "http://www.w3.org/2000/01/rdf-schema#label" && objeto.StartsWith("http"))
                        {
                            objeto = $"literal@{objeto}";
                        }
                        else if (predicado == "http://www.w3.org/1999/02/22-rdf-syntax-ns#type" && !objeto.StartsWith("http"))
                        {
                            objeto = $"http@{objeto}";
                        }

                        //Borramos las triples de la categoría eliminada y todos sus padres (todas):
                        EscribirTripletaEntidad(sujeto, predicado, objeto, ref triplesBorrarRec, triplesBorrarRecAux, false, null);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Los datos del recurso con Path '{(string)fila[0]}' que depende de la categoría son incorrectos: {Environment.NewLine}{ex.ToString()}");
                    }
                }
            }

            if (pDocOntologiaRecursos != null)
            {
                triplesInsertarRec.Add(Guid.Empty, "");
            }
            else //Hay que volver a generar los grafos, no te puedes fiar de lo que haya en el de comunidad.
            {
                docsElemV = docCN.ObtenerListaRecursosConElementoVinculadoID(new List<Guid>(docPaths.Keys));
            }


            //Hay que borrar las tiples que tienen como objeto el sujeto de la vinculación con las categorías y comprobar si la ontología admite que se quede sin categorías y si eso ocurre con algún recurso:

            dataSetPaths.Dispose();
            dataSetPaths = null;
            dataSetPaths = facCN.ObtenerTripletasMismoPredicadoDeSujetoConObjetoQueTieneUnObjeto(pUrlOntologiaTesauro, pCategoriaAEliminarID);

            Dictionary<string, int> predicadosRelacion = new Dictionary<string, int>();
            Dictionary<string, int> sujPredNumTrip = new Dictionary<string, int>();

            foreach (DataRow fila in dataSetPaths.Tables[0].Rows)
            {
                string sujeto = (string)fila[0];
                string predicado = (string)fila[1];
                string objeto = (string)fila[2];
                string claveSujPred = sujeto + "|" + predicado;

                if (predicado != "http://gnoss/hasEntidad")
                {
                    if (!predicadosRelacion.ContainsKey(predicado))
                    {
                        predicadosRelacion.Add(predicado, -1);
                    }

                    if (!sujPredNumTrip.ContainsKey(claveSujPred))
                    {
                        sujPredNumTrip.Add(claveSujPred, 0);
                    }
                }

                if (sujetosPath.Contains(objeto))
                {
                    //Borramos las triples de relación del recurso con el sujeto de la categoría eliminada:
                    EscribirTripletaEntidad(sujeto, predicado, objeto, ref triplesBorrarRec, triplesBorrarRecAux, false, null);

                    if (predicado != "http://gnoss/hasEntidad")
                    {
                        sujPredNumTrip[claveSujPred]--;
                    }
                }
                else if (predicado != "http://gnoss/hasEntidad")
                {
                    sujPredNumTrip[claveSujPred]++;
                }
            }

            if (!string.IsNullOrEmpty(pUrlOntologiaRecursos) && pDocOntologiaRecursos != null)
            {
                GestionOWL gestorOWL = new GestionOWL();
                gestorOWL.UrlOntologia = pBaseURLFormulariosSem + "/Ontologia/" + pDocOntologiaRecursos.FilaDocumento.Enlace + "#";
                gestorOWL.NamespaceOntologia = GestionOWL.NAMESPACE_ONTO_GNOSS;
                GestionOWL.FicheroConfiguracionBD = "acid";

                byte[] arrayOnto = ObtenerOntologia(pDocOntologiaRecursos.Clave, pDocOntologiaRecursos.ProyectoID);
                Ontologia ontologia = new Ontologia(arrayOnto, true);
                ontologia.GestorOWL = gestorOWL;
                ontologia.LeerOntologia();

                //Elimino los predicados no obligatorios para no comprobarlos:
                foreach (string predicado in new List<string>(predicadosRelacion.Keys))
                {
                    Propiedad propiedad = null;

                    foreach (ElementoOntologia entidad in ontologia.Entidades)
                    {
                        propiedad = entidad.ObtenerPropiedadPorNombreOUri(predicado);

                        if (propiedad != null)
                        {
                            if (propiedad.FunctionalProperty)
                            {
                                predicadosRelacion[predicado] = 0;
                            }
                            else
                            {
                                foreach (Restriccion restrinccion in entidad.Restricciones)
                                {
                                    if (restrinccion.Propiedad == propiedad.Nombre && (restrinccion.TipoRestriccion == TipoRestriccion.Cardinality || restrinccion.TipoRestriccion == TipoRestriccion.MinCardinality))
                                    {
                                        predicadosRelacion[predicado] = int.Parse(restrinccion.Valor) - 1;
                                        break;
                                    }
                                }

                                if (predicadosRelacion[predicado] == -1)
                                {
                                    predicadosRelacion.Remove(predicado);
                                }
                            }

                            break;
                        }
                    }

                    if (propiedad == null)
                    {
                        throw new Exception("Se están intentado eliminar triples de la propiedad '" + predicado + "' que no pertenece a la ontología.");
                    }
                }

                if (predicadosRelacion.Count > 0)
                {
                    //Compruebo como han quedado las tripletas:
                    foreach (string claveSujPred in sujPredNumTrip.Keys)
                    {
                        string predicado = claveSujPred.Split('|')[1];

                        if (predicadosRelacion.ContainsKey(predicado))
                        {
                            if (sujPredNumTrip[claveSujPred] < predicadosRelacion[predicado])
                            {
                                throw new Exception($"Se está inclumpiendo la restricción de cardinalidad de la propiedad '{predicado}' que es '{(predicadosRelacion[predicado] + 1)}' y la propiedad quedará con '{(sujPredNumTrip[claveSujPred] + 1)}' elementos.");
                            }
                        }
                    }
                }
            }

            #endregion

            #region Guardamos los datos

            //Guardo tesauro:

            Dictionary<Guid, KeyValuePair<short, List<Guid>>> docsTipoProys = docCN.ObtenerTipoYProyectosDocumentos(new List<Guid>(docPaths.Keys));

            string nombreOntologia = pDocOntologia.FilaDocumento.Enlace;

            //Guardo triples grafo tesauro semántico:
            BorrarGrupoTripletasEnListaVirtuoso(pUrlIntragnoss, nombreOntologia, triplesComBorrar, pUsarColaReplicacion);
            InsertaTripletasVirtuoso(pUrlIntragnoss, nombreOntologia, triplesInsertar, PrioridadBase.ApiRecursos, pUsarColaReplicacion);

            //Guardo triples en los proyectos en los que está subido y compartido el tesauro semántico:
            foreach (Guid proyectoID in pDocOntologia.ListaProyectos)
            {
                BorrarGrupoTripletasEnListaVirtuoso(pUrlIntragnoss, proyectoID.ToString().ToLower(), triplesComBorrarDef, pUsarColaReplicacion);
                InsertaTripletasVirtuoso(pUrlIntragnoss, proyectoID.ToString().ToLower(), triplesComInsertarDef, PrioridadBase.ApiRecursos, pUsarColaReplicacion);
            }

            //Guardo Recursos:
            elemVinGrafo = new Dictionary<Guid, string>();

            if (pDocOntologiaRecursos != null)
            {
                elemVinGrafo.Add(Guid.Empty, pDocOntologiaRecursos.FilaDocumento.Enlace);
            }

            foreach (Guid elementVinID in triplesInsertarRec.Keys)
            {
                if (!elemVinGrafo.ContainsKey(elementVinID))
                {
                    elemVinGrafo.Add(elementVinID, docCN.ObtenerEnlaceDocumentoPorDocumentoID(elementVinID));
                }

                if (!string.IsNullOrEmpty(triplesBorrarRec))
                {
                    BorrarGrupoTripletasEnListaVirtuoso(pUrlIntragnoss, elemVinGrafo[elementVinID], triplesBorrarRecAux, pUsarColaReplicacion);
                }

                if (!string.IsNullOrEmpty(triplesInsertarRec[elementVinID]))
                {
                    InsertaTripletasVirtuoso(pUrlIntragnoss, elemVinGrafo[elementVinID], triplesInsertarRec[elementVinID], PrioridadBase.ApiRecursos, pUsarColaReplicacion);
                }
            }

            foreach (Guid documentoID in docsTipoProys.Keys)
            {
                foreach (Guid proyID in docsTipoProys[documentoID].Value)
                {
                    //Inserto en el base los recursos:
                    AgregarRecursoModeloBaseSimple(documentoID, proyID, docsTipoProys[documentoID].Key, PrioridadBase.ApiRecursos);
                }

                //Borro RDF SQL Server de los recursos:
                BorrarRDFDeBDRDF(documentoID);
            }

            dataSetCategorias.Dispose();
            dataSetPaths.Dispose();

            pDocOntologia.GestorDocumental.Dispose();

            if (pDocOntologiaRecursos != null)
            {
                pDocOntologiaRecursos.GestorDocumental.Dispose();
            }

            #endregion
        }

        /// <summary>
        /// Obtiene las categorías hijas recursivamente de una categoría.
        /// </summary>
        /// <param name="pCategoriaID"></param>
        /// <param name="pPropiedadHija"></param>
        private static void ObtenerCategoriasHijosRecursivosTesSem(List<string> pCategorias, string pCategoriaID, List<string> pPropiedadesHija, FacetadoCN pFacCN, string pUrlOntologiaTesauro)
        {
            if (!pCategorias.Contains(pCategoriaID))
            {
                pCategorias.Add(pCategoriaID);
            }

            FacetadoDS dataSetCategorias = pFacCN.ObtenerValoresPropiedadesEntidad(pUrlOntologiaTesauro, pCategoriaID, pPropiedadesHija);

            foreach (DataRow fila in dataSetCategorias.Tables[0].Rows)
            {
                ObtenerCategoriasHijosRecursivosTesSem(pCategorias, (string)fila[2], pPropiedadesHija, pFacCN, pUrlOntologiaTesauro);
            }
        }

        /// <summary>
        /// Borra unas triples en virtuoso con control de checkpoint.
        /// </summary>
        /// <param name="pUrlIntragnoss">Url de intraGnoss</param>
        /// <param name="pGrafo">Grafo</param>
        /// <param name="pTriples">Triples</param>
        /// <param name="pUsarColaReplicacion">Indica si se debe usar la cola de replicación</param>
        private void BorrarGrupoTripletasVirtuoso(string pUrlIntragnoss, string pGrafo, string pTriples, bool pUsarColaReplicacion)
        {
            FacetadoCN facCN = null;

            try
            {
                facCN = new FacetadoCN(pUrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                facCN.BorrarGrupoTripletas(pGrafo, pTriples, pUsarColaReplicacion);

            }
            catch (Exception)
            {
                if (!pUsarColaReplicacion)
                {
                    //Cerramos las conexiones
                    ControladorConexiones.CerrarConexiones();

                    //Realizamos una consulta ask a virtuoso para comprobar si está funcionando
                    while (!new UtilidadesVirtuoso(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).ServidorOperativo("acid", pUrlIntragnoss))
                    {
                        //Dormimos 5 segundos
                        Thread.Sleep(5 * 1000);
                    }

                    facCN = new FacetadoCN(pUrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    facCN.BorrarGrupoTripletas(pGrafo, pTriples, pUsarColaReplicacion);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Borra unas triples en virtuoso con control de checkpoint.
        /// </summary>
        /// <param name="pUrlIntragnoss">Url de intraGnoss</param>
        /// <param name="pGrafo">Grafo</param>
        /// <param name="pTriples">Triples</param>
        /// <param name="pUsarColaReplicacion">Indica si se debe usar la cola de replicación</param>
        private void BorrarGrupoTripletasEnListaVirtuoso(string pUrlIntragnoss, string pGrafo, List<TripleWrapper> pTriples, bool pUsarColaReplicacion)
        {
            FacetadoCN facCN = null;

            try
            {
                facCN = new FacetadoCN(pUrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                facCN.BorrarGrupoTripletasEnLista(pGrafo, pTriples, pUsarColaReplicacion);

            }
            catch (Exception)
            {
                if (!pUsarColaReplicacion)
                {
                    //Cerramos las conexiones
                    ControladorConexiones.CerrarConexiones();

                    //Realizamos una consulta ask a virtuoso para comprobar si está funcionando
                    while (!new UtilidadesVirtuoso(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).ServidorOperativo("acid", pUrlIntragnoss))
                    {
                        //Dormimos 5 segundos
                        Thread.Sleep(5 * 1000);
                    }

                    facCN = new FacetadoCN(pUrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    facCN.BorrarGrupoTripletasEnLista(pGrafo, pTriples, pUsarColaReplicacion);
                }
                else
                {
                    throw;
                }
            }

        }

        /// <summary>
        /// Inserta unas triples en virtuoso con control de checkpoint.
        /// </summary>
        /// <param name="pUrlIntragnoss">Url de intraGnoss</param>
        /// <param name="pGrafo">Grafo</param>
        /// <param name="pTriples">Triples</param>
        /// <param name="Prioridad">Prioridad para el servicio de replicación</param>
        /// <param name="pUsarColaReplicacion">Indica si se debe usar la cola de replicación</param>
        private void InsertaTripletasVirtuoso(string pUrlIntragnoss, string pGrafo, string pTriples, PrioridadBase Prioridad, bool pUsarColaReplicacion)
        {
            FacetadoCN facCN = null;

            try
            {
                facCN = new FacetadoCN(pUrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                facCN.InsertaTripletas(pGrafo, pTriples, (short)Prioridad, pUsarColaReplicacion);

            }
            catch (Exception)
            {
                if (!pUsarColaReplicacion)
                {
                    //Cerramos las conexiones
                    ControladorConexiones.CerrarConexiones();

                    //Realizamos una consulta ask a virtuoso para comprobar si está funcionando
                    while (!new UtilidadesVirtuoso(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).ServidorOperativo("acid", pUrlIntragnoss))
                    {
                        //Dormimos 5 segundos
                        Thread.Sleep(5 * 1000);
                    }

                    facCN = new FacetadoCN(pUrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    facCN.InsertaTripletas(pGrafo, pTriples, (short)Prioridad, pUsarColaReplicacion);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Inserta las triples de un rdf en virtuoso con control de checkpoint.
        /// </summary>
        /// <param name="pNombreGrafo">Nombre del grafo en el que se guardará el RDF</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        /// <param name="pFicheroConfiguracion">Fichero de configuración de la BD</param>
        /// <param name="pProyectoID">ID del proyecto actual</param>
        /// <param name="pEsribirNT">Escribe o no NT</param>
        /// <param name="pInfoExtra">Info extra</param>
        /// <param name="pTripletas">Tripletas a guardar en virtuoso</param>
        private void InsertaTripletasRDFEnVirtuoso(string pUrlIntragnoss, string pNombreGrafo, string pFicheroConfiguracion, Guid pProyectoID, bool pEsribirNT, string pInfoExtra, string pTripletas, bool pUsarColareplicacion)
        {
            FacetadoCN facetadoCN = null;
            if (pFicheroConfiguracion == "")
            {
                facetadoCN = new FacetadoCN(pUrlIntragnoss, pProyectoID.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            }
            else
            {
                facetadoCN = new FacetadoCN(pFicheroConfiguracion, pUrlIntragnoss, pProyectoID.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            }

            facetadoCN.InsertaTripletas(pNombreGrafo.ToLower(), pTripletas, 0, pUsarColareplicacion, pEsribirNT, pInfoExtra);
        }

        /// <summary>
        /// Inserta las triples de un rdf en virtuoso con control de checkpoint.
        /// </summary>
        /// <param name="pNombreGrafo">Nombre del grafo en el que se guardará el RDF</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        /// <param name="pFicheroConfiguracion">Fichero de configuración de la BD</param>
        /// <param name="pProyectoID">ID del proyecto actual</param>
        /// <param name="pEscribirNT">Escribe o no NT</param>
        /// <param name="pInfoExtra">Info extra</param>
        /// <param name="pTripletas">Tripletas a guardar en virtuoso</param>
        private void InsertaTripletasRDFEnVirtuosoConModify(string pNombreGrafo, Guid pProyectoID, string pUrlIntragnoss, string pFicheroConfiguracion, string pElementoaEliminarID, string pTripletas, bool pUsarColareplicacion, string pInfoExtraReplicacion, short pPrioridad, bool pEscribirNT)
        {
            FacetadoCN facetadoCN = null;
            if (pFicheroConfiguracion == "")
            {
                facetadoCN = new FacetadoCN(pUrlIntragnoss, pProyectoID.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            }
            else
            {
                facetadoCN = new FacetadoCN(pFicheroConfiguracion, pUrlIntragnoss, pProyectoID.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            }

            try
            {
                // Inicio una transacción para que si se ha llamado desde el API se pueda deshacer
                facetadoCN.FacetadoAD.IniciarTransaccion();
                facetadoCN.InsertaTripletasRecursoSemanticoConModify(pNombreGrafo, pUrlIntragnoss, pFicheroConfiguracion, pProyectoID, pInfoExtraReplicacion, pUsarColareplicacion, pElementoaEliminarID, pTripletas, pPrioridad, pEscribirNT);
                facetadoCN.FacetadoAD.TerminarTransaccion(true);
            }
            catch (Exception)
            {
                facetadoCN.FacetadoAD.TerminarTransaccion(false);
                //Cerramos las conexiones
                ControladorConexiones.CerrarConexiones();
                //int i = 0;
                bool estaOperativo = new UtilidadesVirtuoso(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).ServidorOperativo(pFicheroConfiguracion, pUrlIntragnoss);

                ////Realizamos una consulta ask a virtuoso para comprobar si está funcionando
                //while ((!estaOperativo) && i < 5)
                //{
                //    i++;
                //    //Dormimos 5 segundos
                //    Thread.Sleep(5 * 1000);
                //    estaOperativo = UtilidadesVirtuoso.ServidorOperativo(pFicheroConfiguracion, pUrlIntragnoss);
                //}

                if (estaOperativo)
                {
                    facetadoCN.InsertaTripletasRecursoSemanticoConModify(pNombreGrafo, pUrlIntragnoss, pFicheroConfiguracion, pProyectoID, pInfoExtraReplicacion, pUsarColareplicacion, pElementoaEliminarID, pTripletas, pPrioridad, pEscribirNT);
                }
                else
                {
                    throw;
                }
            }
        }

        private void AgregarPropiedadANuevaCategoriaTesSem(ElementoOntologia pEntidad, string pNombreProp, ref string pTriplesInsertar, List<TripleWrapper> pTriplesComInsertar)
        {
            AgregarPropiedadANuevaCategoriaTesSem(pEntidad, pNombreProp, ref pTriplesInsertar, pTriplesComInsertar, true);
        }

        private void AgregarPropiedadANuevaCategoriaTesSem(ElementoOntologia pEntidad, string pNombreProp, ref string pTriplesInsertar, List<TripleWrapper> pTriplesComInsertar, bool pObligatoria)
        {
            Propiedad propiedad = pEntidad.ObtenerPropiedad(pNombreProp);

            if (propiedad == null)
            {
                throw new Exception("La categoría no posee la propiedad '" + pNombreProp + "'.");
            }
            else if (propiedad.ListaValoresIdioma.Count > 0)
            {
                foreach (string idioma in propiedad.ListaValoresIdioma.Keys)
                {
                    foreach (string valor in propiedad.ListaValoresIdioma[idioma].Keys)
                    {
                        EscribirTripletaEntidad(pEntidad.Uri, pNombreProp, valor, ref pTriplesInsertar, pTriplesComInsertar, false, idioma);
                    }
                }
            }
            else if (propiedad.ValoresUnificados.Count > 0)
            {
                foreach (string valor in propiedad.ValoresUnificados.Keys)
                {
                    EscribirTripletaEntidad(pEntidad.Uri, pNombreProp, valor, ref pTriplesInsertar, pTriplesComInsertar, false, null);
                }
            }
            else if (pObligatoria)
            {
                throw new Exception("La categoría debe tener valor para la propiedad '" + pNombreProp + "'.");
            }
        }


        /// <summary>
        /// Obtiene los datos de la faceta de tesauro semántica.
        /// </summary>
        /// <param name="pFaceta">Faceta</param>
        /// <returns>Array con la configuración: Grafo, Propiedad de unión de colección con categorías raíz, Propiedad con el Identificador de las categorías, Propiedad con el Nombre de las categorías, Propiedad que relaciona categorías padres con hijas</returns>
        public static string[] ObtenerDatosFacetaTesSem(string pFaceta)
        {
            //TODO JAVIER: Leer de XML de ontología y poner en cada caso las propiedades y grafo correctos.

            string[] array = new string[9];
            array[0] = "taxonomy.owl";
            array[1] = "http://www.w3.org/2008/05/skos#member";//hasHijoRaiz
            array[2] = "http://purl.org/dc/elements/1.1/identifier";//identificador
            array[3] = "http://www.w3.org/2008/05/skos#prefLabel";//nombre
            array[4] = "http://www.w3.org/2008/05/skos#narrower";//hasHijo
            array[5] = "http://purl.org/dc/elements/1.1/source";//Fuente del tesauro
            array[6] = "http://www.w3.org/2008/05/skos#symbol";//orden
            array[7] = "http://www.w3.org/2008/05/skos#broader";//hasPadre
            array[8] = "http://www.w3.org/2008/05/skos#Collection";//Collection

            return array;
        }

        private static string PasarObjetoALower(string pObjeto, List<string> pListaTipos)
        {
            if ((!pObjeto.StartsWith("<http://gnoss/")) && (!pListaTipos.Contains(pObjeto)))
            {
                return pObjeto.ToLowerSearchGraph();
            }
            return pObjeto;
        }

        /// <summary>
        /// Carga el documento de la ontología secundaria.
        /// </summary>
        /// <param name="pUrlOntologia">Url de la ontología</param>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>Documento de la ontología secundaria</returns>
        public Documento ObtenerOntologiaDeEntidadSecundaria(string pUrlOntologia, Guid pProyectoID)
        {
            #region Cargo Ontología

            DocumentacionCN docCN = new DocumentacionCN("acid", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Guid ontologiaID = docCN.ObtenerOntologiaAPartirNombre(pProyectoID, pUrlOntologia);
            List<Guid> listaDos = new List<Guid>();
            listaDos.Add(ontologiaID);

            GestorDocumental pGestorDocumental = new GestorDocumental(docCN.ObtenerDocumentosPorID(listaDos, true), mLoggingService, mEntityContext);
            pGestorDocumental.CargarDocumentos(false);

            if (ontologiaID == Guid.Empty || !pGestorDocumental.ListaDocumentos.ContainsKey(ontologiaID))
            {
                throw new Exception("La ontología proporcionada '" + pUrlOntologia + "' no es válida.");
            }

            if (pGestorDocumental.ListaDocumentos[ontologiaID].TipoDocumentacion != TiposDocumentacion.OntologiaSecundaria)
            {
                throw new Exception("La ontología '" + pGestorDocumental.ListaDocumentos[ontologiaID].Enlace + "' con ID '" + ontologiaID + "' y url '" + pUrlOntologia + "' proporcionada no es secundaria.");
            }

            return pGestorDocumental.ListaDocumentos[ontologiaID];

            #endregion
        }

        #endregion

        #region Entidades Secundarias

        /// <summary>
        /// Guarda en virtuoso una entidad secundaria.
        /// </summary>
        /// <param name="pEntidades">Objetos semánticos de la entidad secundaria</param>
        /// <param name="pUrlIntragnoss">Url de Intragnoss</param>
        /// <param name="pUrlOntologia">Url de la ontología</param>
        /// <param name="pOrgProyID">ID de la organización del proyecto</param>
        /// <param name="pProyectoID">ID del proyecto de la ontología secundaria</param>
        /// <param name="pDocOnto">Documento de la ontología secundaria</param>
        /// <param name="pEditandoEntSec">Indica si se está editando la entidad secundaría o de lo contrario es nueva</param>
        public void GuardarRDFEntidadSecundaria(List<ElementoOntologia> pEntidades, string pUrlIntragnoss, string pUrlOntologia, Guid pOrgProyID, Guid pProyectoID, Documento pDocOnto, bool pEditandoEntSec)
        {
            string idHasEntidadPrincipal = "entidadsecun_" + pEntidades[0].ID.ToLower();

            FacetadoCN facCN = new FacetadoCN(pUrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            List<string> sujetos = facCN.ObtenerSujetosConObjetoDePropiedad(pUrlOntologia, pEntidades[0].Uri, "http://gnoss/hasEntidad");

            if (pEditandoEntSec)
            {
                if (sujetos.Count > 0 && sujetos[0].Contains("entidadsecun_"))
                {
                    idHasEntidadPrincipal = sujetos[0].Substring(sujetos[0].IndexOf("entidadsecun_"));
                }

                //El RDF ya existia por lo que hay que remplazarlo:
                //10/03/2017 ahora se hace un MODIFY delete insert en GuardarRDFEnVirtuoso
                //ControladorDocumentacion.BorrarRDFDeVirtuoso(idHasEntidadPrincipal, pUrlOntologia, pUrlIntragnoss, "acid", pProyectoID, true);
            }
            else if (sujetos.Count > 0)
            {
                throw new Exception("Ya existe una entidad secundaria con ID '" + pEntidades[0].Uri + "' en la ontología '" + pUrlOntologia + "'.");
            }

            #region SemWeb

            List<TripleWrapper> tripletasInsertadasDSList = GuardarRDFEnVirtuoso(pEntidades, pUrlOntologia, pUrlIntragnoss, "acid", pProyectoID, idHasEntidadPrincipal, false, null, false, false, (short)PrioridadBase.ApiRecursos);

            AgregarTipletaGrafoEntSecudTieneEntSecud(tripletasInsertadasDSList, idHasEntidadPrincipal, pUrlOntologia, pUrlIntragnoss);

            #endregion

            #region Tripletas grafo comunidades

            FacetaCN tablasDeConfiguracionCN = new FacetaCN("acid", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperFacetas dataWrapperFacetas = tablasDeConfiguracionCN.ObtenerFacetaObjetoConocimientoProyecto(pOrgProyID, pProyectoID, true);
            List<FacetaObjetoConocimientoProyecto> filas = dataWrapperFacetas.ListaFacetaObjetoConocimientoProyecto.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();

            List<string> Fecha = new List<string>();
            List<string> Numero = new List<string>();

            foreach (FacetaObjetoConocimientoProyecto myrow in filas)
            {
                if (myrow.TipoPropiedad.Equals((short)TipoPropiedadFaceta.Fecha) || myrow.TipoPropiedad.Equals((short)TipoPropiedadFaceta.Calendario) || myrow.TipoPropiedad.Equals((short)TipoPropiedadFaceta.CalendarioConRangos) || myrow.TipoPropiedad.Equals((short)TipoPropiedadFaceta.Siglo))
                {
                    string fechaAux = myrow.Faceta;
                    string propiedadFecha = fechaAux.Substring(fechaAux.LastIndexOf(":") + 1);
                    if (!Fecha.Contains(propiedadFecha))
                    {
                        Fecha.Add(propiedadFecha);
                    }
                }
                if (myrow.TipoPropiedad.Equals((short)TipoPropiedadFaceta.Numero))
                {
                    string numeroAux = myrow.Faceta;
                    string propiedadNumero = numeroAux.Substring(numeroAux.LastIndexOf(":") + 1);
                    if (!Numero.Contains(propiedadNumero))
                    {
                        Numero.Add(propiedadNumero);
                    }
                }


            }

            string[] arrayTesSem = ObtenerDatosFacetaTesSem(null);
            string propSymbol = arrayTesSem[6];

            if (!Numero.Contains(propSymbol))
            {
                Numero.Add(propSymbol);
            }

            FacetadoAD facetadoAD = new FacetadoAD("acid", pUrlIntragnoss, mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

            List<FacetaEntidadesExternas> entExt = dataWrapperFacetas.ListaFacetaEntidadesExternas.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();
            string tripletasComunidad = "";
            Dictionary<string, string> parametroProyecto = ObtenerParametroProyecto(pProyectoID);
            bool textoTesauroInvariable = parametroProyecto.ContainsKey("TextoInvariableTesauroSemantico") && parametroProyecto["TextoInvariableTesauroSemantico"] == "1";

            bool permitirMayus = parametroProyecto.ContainsKey(AD.Parametro.ParametroAD.PermitirMayusculas) && parametroProyecto[AD.Parametro.ParametroAD.PermitirMayusculas] == "1";
            foreach (TripleWrapper triple in tripletasInsertadasDSList)
            {
                try
                {
                    if (!triple.Predicate.Contains("http://www.w3.org/1999/02/22-rdf-syntax-ns#type") && !triple.Predicate.Contains("http://www.w3.org/2000/01/rdf-schema#label"))
                    {
                        string objeto = (string)triple.Object;

                        objeto = objeto.Replace("\r\n", "");
                        objeto = objeto.Replace("\n", "");

                        if (objeto[0] == '"')
                        {
                            objeto = objeto.Substring(1);
                        }

                        if (objeto[objeto.Length - 1] == '"')
                        {
                            objeto = objeto.Substring(0, objeto.Length - 1);
                        }

                        if (objeto[0] == '<')
                        {
                            objeto = objeto.Substring(1);
                        }

                        if (objeto[objeto.Length - 1] == '>')
                        {
                            objeto = objeto.Substring(0, objeto.Length - 1);
                        }

                        string predicado = triple.Predicate;
                        predicado = predicado.Substring(1, predicado.Length - 2);

                        string sujeto = triple.Subject;
                        if (objeto.Contains("/") && (predicado.Contains("fecha") || predicado.Contains("date")))
                        {
                            objeto = ConvertirFormatoFecha(objeto) + " . ";
                        }

                        string aux = string.Empty;

                        if (objeto.StartsWith(UrlIntragnoss))
                        {
                            objeto = PasarObjetoALower(objeto, FacetadoAD.ListaTiposBase);
                        }

                        tripletasComunidad += facetadoAD.GenerarTripletaRecogidadeVirtuosoSinConversionesAbsurdas(PasarObjetoALower(sujeto, FacetadoAD.ListaTiposBase), predicado, objeto, objeto, Fecha, Numero, entExt, ref aux, triple.ObjectLanguage, triple.ObjectType);
                    }
                }
                catch (Exception) { }
            }

            facetadoAD.Dispose();
            facCN = new FacetadoCN(pUrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

            string sujetoBorrar = pEntidades[0].Uri.ToLower();

            foreach (Guid proyectoID in pDocOnto.ListaProyectos)
            {
                if (pEditandoEntSec)
                {
                    BorrarRDFDeVirtuoso(idHasEntidadPrincipal, pProyectoID.ToString().ToLower(), pUrlIntragnoss, "acid", pProyectoID, false);
                }

                facCN.ModificarListaTripletas(pProyectoID.ToString().ToLower(), tripletasComunidad, (short)PrioridadBase.ApiRecursos, PasarObjetoALower(sujetoBorrar, FacetadoAD.ListaTiposBase));
            }
            #endregion
        }

        /// <summary>
        /// Agrega la tripleta que indica que una ontología secundaría tiene una entidad.
        /// </summary>
        /// <param name="pTripletas">Almacen de tripletas</param>
        /// <param name="pIdHasEntidadPrincipal">ID de la entidad secundaría</param>
        /// <param name="pNombreOntologia">Nombre de la ontología</param>
        /// <param name="pUrlIntragnoss">Url de intragnoss</param>
        private static void AgregarTipletaGrafoEntSecudTieneEntSecud(List<TripleWrapper> pTripletas, string pIdHasEntidadPrincipal, string pNombreOntologia, string pUrlIntragnoss)
        {
            pTripletas.Add(new TripleWrapper { Subject = "<" + pUrlIntragnoss + pNombreOntologia.ToLower() + "> ", Predicate = "<http://gnoss/hasEntidad>", Object = "<" + pUrlIntragnoss + pIdHasEntidadPrincipal + ">" });
        }

        /// <summary>
        /// Coge una fecha en formato 01/01/2010 y lo cambia a 20100101000000
        /// </summary>
        /// <returns>Fecha Cambiada</returns>
        public static string ConvertirFormatoFecha(string fecha)
        {
            fecha = fecha.Trim();

            string nfecha;

            if (fecha.IndexOf("/").Equals(2))
            {
                nfecha = fecha.Substring(fecha.LastIndexOf("/") + 1, 4);
                fecha = fecha.Substring(0, fecha.LastIndexOf("/"));
                nfecha += fecha.Substring(fecha.LastIndexOf("/") + 1, 2);
                fecha = fecha.Substring(0, fecha.LastIndexOf("/"));
                nfecha += fecha.Substring(0, 2);
            }
            else
            {
                nfecha = fecha.Substring(0, fecha.IndexOf("/"));
                fecha = fecha.Substring(fecha.IndexOf("/") + 1);
                nfecha += fecha.Substring(0, fecha.IndexOf("/"));
                fecha = fecha.Substring(fecha.IndexOf("/") + 1);
                nfecha += fecha;
            }

            return nfecha + "000000";
        }

        #endregion

        #region Otros

        public void RecargarGestorAddToGnoss(GestorAddToGnoss pGestorAddToGnoss, Guid pUsuarioID)
        {
            if (pGestorAddToGnoss.GestorDocumental == null)
            {
                pGestorAddToGnoss.GestorDocumental = new GestorDocumental(new DataWrapperDocumentacion(), mLoggingService, mEntityContext);
            }
            if (pGestorAddToGnoss.GestorDocumental.GestorTesauro == null)
            {
                TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                if (pGestorAddToGnoss.EsBaseRecursosPersonal)
                {
                    pGestorAddToGnoss.GestorDocumental.GestorTesauro = new GestionTesauro(tesauroCN.ObtenerTesauroUsuario(pUsuarioID), mLoggingService, mEntityContext);
                    pGestorAddToGnoss.EsBaseRecursosPublica = false;
                    Guid catPublicaUsuario = pGestorAddToGnoss.GestorDocumental.GestorTesauro.CategoriaPublicaID;

                    foreach (CategoriaTesauro categoria in pGestorAddToGnoss.ListaCategorias)
                    {
                        if (categoria.PadreNivelRaiz.Clave == catPublicaUsuario)
                        {
                            pGestorAddToGnoss.EsBaseRecursosPublica = true;
                            break;
                        }
                    }
                }
                else if (pGestorAddToGnoss.EsBaseRecursosOrganizacion)
                {
                    pGestorAddToGnoss.GestorDocumental.GestorTesauro = new GestionTesauro(tesauroCN.ObtenerTesauroOrganizacion(pGestorAddToGnoss.OrganizacionBRID), mLoggingService, mEntityContext);
                    pGestorAddToGnoss.EsBaseRecursosPublica = false;
                    Guid catPublicaUsuario = pGestorAddToGnoss.GestorDocumental.GestorTesauro.CategoriaPublicaID;

                    foreach (CategoriaTesauro categoria in pGestorAddToGnoss.ListaCategorias)
                    {
                        if (categoria.PadreNivelRaiz.Clave == catPublicaUsuario)
                        {
                            pGestorAddToGnoss.EsBaseRecursosPublica = true;
                            break;
                        }
                    }
                }
                else
                {
                    TesauroCL tesauroCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    pGestorAddToGnoss.GestorDocumental.GestorTesauro = new GestionTesauro(tesauroCL.ObtenerTesauroDeProyecto(pGestorAddToGnoss.ProyectoBRID), mLoggingService, mEntityContext);
                }
                pGestorAddToGnoss.ListaCategorias = null;
            }
        }


        /// <summary>
        /// Limpia una palabra del título para que sea tag título generado.
        /// </summary>
        /// <param name="pPalabra">Palabra</param>
        /// <returns>palabra del título para que sea tag título generado</returns>
        public static string LimpiarPalabraParaTagGeneradoSegunTitulo(string pPalabra)
        {
            if (pPalabra.Length == 0)
            {
                return pPalabra;
            }
            else if (pPalabra.IndexOf('¿') == 0 || pPalabra.IndexOf('?') == 0 || pPalabra.IndexOf('¡') == 0 || pPalabra.IndexOf('!') == 0)
            {
                return LimpiarPalabraParaTagGeneradoSegunTitulo(pPalabra.Substring(1));
            }
            else if (pPalabra.LastIndexOf('¿') == (pPalabra.Length - 1) || pPalabra.LastIndexOf('?') == (pPalabra.Length - 1) || pPalabra.LastIndexOf('¡') == (pPalabra.Length - 1) || pPalabra.LastIndexOf('!') == (pPalabra.Length - 1))
            {
                return LimpiarPalabraParaTagGeneradoSegunTitulo(pPalabra.Substring(0, pPalabra.Length - 1));
            }
            else
            {
                return pPalabra;
            }
        }
        /// <summary>
        /// Notifica al modelo base que se ha creado una invitacion.
        /// </summary>
        /// <param name="pDocumentoID">Lista de documentos con su tipo</param>
        /// <param name="pProyectoID">Proyecto en el que se han modificado</param>
        /// <param name="pPrioridadBase">Prioridad de las filas introducidas en el modelo base</param>
        public void AgregarInvitacionFacModeloBaseSimple(Guid pInvitacionID, Guid pInvitacionDestinoID, Guid pProyectoID, string pFicheroConfiguracionBD, string pOtrosArgumentos, PrioridadBase pPrioridadBase)
        {
            BaseInvitacionesDS pBaseInvitacionesDS = new BaseInvitacionesDS();

            if (ProyectoAD.TablaBaseIdMetaProyecto == int.MinValue)
            {
                ProyectoAD.TablaBaseIdMetaProyecto = ObtenerFilaProyecto(ProyectoAD.MetaProyecto).TablaBaseProyectoID;
            }

            BaseInvitacionesDS baseInvitacionesDS = pBaseInvitacionesDS;

            #region Marcar agregado

            BaseInvitacionesDS.ColaTagsInvitacionesRow filaColaTagsCom = baseInvitacionesDS.ColaTagsInvitaciones.NewColaTagsInvitacionesRow();

            filaColaTagsCom.Estado = (short)EstadosColaTags.EnEspera;
            filaColaTagsCom.FechaPuestaEnCola = DateTime.Now;
            filaColaTagsCom.TablaBaseProyectoID = ProyectoAD.TablaBaseIdMetaProyecto;
            filaColaTagsCom.Tags = Constantes.ID_INVITACION + pInvitacionID.ToString() + Constantes.ID_INVITACION + "," + Constantes.ID_INVITACION_IDDESTINO + pInvitacionDestinoID.ToString() + Constantes.ID_INVITACION_IDDESTINO;
            filaColaTagsCom.Tipo = 0;
            filaColaTagsCom.Prioridad = (short)pPrioridadBase;

            baseInvitacionesDS.ColaTagsInvitaciones.AddColaTagsInvitacionesRow(filaColaTagsCom);

            #endregion

            BaseComunidadCN brComCN = new BaseComunidadCN(mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);//, -1
            brComCN.InsertarFilasEnRabbit("ColaTagsInvitaciones", baseInvitacionesDS);

            baseInvitacionesDS.Dispose();
        }

        /// <summary>
        /// Notifica al modelo base que se han modificado una serie de documentos.
        /// </summary>
        /// <param name="pDocumentoID">Lista de documentos con su tipo</param>
        /// <param name="pProyectoID">Proyecto en el que se han modificado</param>
        /// <param name="pPrioridadBase">Prioridad de las filas introducidas en el modelo base</param>
        public void AgregarMensajeFacModeloBaseSimple(Guid pMensajeID, Guid pMensajeFrom, Guid pProyectoID, string pFicheroConfiguracionBD, string pMensajeTo, Guid? pMensajeOrigenID, PrioridadBase pPrioridadBase)
        {
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            int id = proyCL.ObtenerTablaBaseProyectoIDProyectoPorID(ProyectoAD.MetaProyecto);
            proyCL.Dispose();

            string mensajeOrigenID = "";
            if (pMensajeOrigenID.HasValue)
            {
                mensajeOrigenID = "," + Constantes.ID_MENSAJE_ORIGEN + pMensajeOrigenID.Value + Constantes.ID_MENSAJE_ORIGEN;
            }

            //Agregamos peticiones a la cola
            new ControladorCorreo(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication).AgregarElementoARabbitMQ(id, pMensajeID, pMensajeTo, pMensajeFrom, pPrioridadBase, mensajeOrigenID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pTipoDato">Voto, visita, comentario, recurso...</param>
        /// <param name="pDocumentoID"></param>
        /// <param name="pProyectoID"></param>
        /// <param name="pIdentidadID"></param>
        /// <param name="pCreadorID"></param>
        public void LlamadaUDP_ServicioSocketsOffline(string pTipoDato, Guid pDocumentoID, Guid pProyectoID, Guid pIdentidadID, Guid pCreadorID)
        {
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Guid baseRecursosID = proyCN.ObtenerBaseRecursosProyectoPorProyectoID(pProyectoID);
            byte[] sdata = Encoding.ASCII.GetBytes(pTipoDato + "|" + pDocumentoID + "|" + pProyectoID + "|" + pIdentidadID + "|" + baseRecursosID + "|" + pCreadorID);

            LlamadaUDP_ServicioSocketsOffline(sdata);
        }

        /// <summary>
        /// Envia al servicio de sockets offline los datos pasados como parámetro.
        /// </summary>
        /// <param name="pData">byte[] con los datos a enviar.</param>
        public void LlamadaUDP_ServicioSocketsOffline(byte[] pData)
        {
            string ip = mConfigService.ObtenerIpServicioSocketsOffline();
            int puerto = mConfigService.ObtenerPuertoServicioSocketsOffline();

            LlamadaUDP_ServicioSocketsOffline(puerto, ip, pData);
        }

        /// <summary>
        /// Envia pData a través de UDP a la ip pasada como parámetro.
        /// </summary>
        /// <param name="pPuerto">Puerto al que se envia el socket.</param>
        /// <param name="pIp">Ip al que se envia el socket.</param>
        /// <param name="pData">Datos que se envian.</param>
        public static void LlamadaUDP_ServicioSocketsOffline(int pPuerto, string pIp, byte[] pData)
        {
            UdpClient udpc = new UdpClient(pIp, pPuerto);
            udpc.Send(pData, pData.Length);
            udpc.Close();
        }

        public void ReprocesarRecursosIdentidadCambiarModoParticipacion(Identidad pIdentidadProyecto)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            docCN.ObtenerDocumentosDeIdentidadEnProyecto(dataWrapperDocumentacion, pIdentidadProyecto.Clave, pIdentidadProyecto.FilaIdentidad.ProyectoID);

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            int tablaBaseProyectoID = proyCN.ObtenerTablaBaseProyectoIDProyectoPorID(pIdentidadProyecto.FilaIdentidad.ProyectoID);


            BaseRecursosComunidadDS baseRecursosComDS = new BaseRecursosComunidadDS();
            foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos row in dataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos)
            {
                BaseRecursosComunidadDS.ColaTagsComunidadesRow filaColaTagsDocs = baseRecursosComDS.ColaTagsComunidades.NewColaTagsComunidadesRow();

                filaColaTagsDocs.Estado = 0;
                filaColaTagsDocs.FechaPuestaEnCola = DateTime.Now;
                filaColaTagsDocs.TablaBaseProyectoID = tablaBaseProyectoID;
                filaColaTagsDocs.Tags = Constantes.ID_TAG_DOCUMENTO + row.DocumentoID.ToString() + Constantes.ID_TAG_DOCUMENTO + "," + Constantes.TIPO_DOC + row.Documento.Tipo + Constantes.TIPO_DOC + "," + new UtilidadesVirtuoso(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).TagBaseAfinidadVirtuoso;
                filaColaTagsDocs.Tipo = 0;
                filaColaTagsDocs.Prioridad = 1;

                baseRecursosComDS.ColaTagsComunidades.AddColaTagsComunidadesRow(filaColaTagsDocs);
            }

            BaseComunidadCN brComCN = new BaseComunidadCN("base", -1, mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);
            brComCN.InsertarFilasEnRabbit("ColaTagsComunidades", baseRecursosComDS);
            baseRecursosComDS.Dispose();
        }

        #endregion

        #endregion

        #region Privados

        /// <summary>
        /// Obtiene el rdf configurado para un tipo de recurso en un proyecto
        /// </summary>
        /// <param name="pProyectoId"></param>
        /// <param name="pTipoDocumentacion"></param>
        /// <returns></returns>
        private string ObtenerRdfRecursoNoSemantico(Guid pProyectoId, short pTipoDocumentacion)
        {
            string rdfConfiguradoRecursoNoSemantico = "";
            ParametroGeneralCN paramGralCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            //ParametroGeneralDS filaProyectoRdfType = paramGralCN.ObtenerProyectoRDFType(pProyectoId, pTipoDocumentacion);
            List<ProyectoRDFType> filaProyectoRdfType = paramGralCN.ObtenerProyectoRDFType(pProyectoId, pTipoDocumentacion);
            if (filaProyectoRdfType.Count > 0)
            {
                rdfConfiguradoRecursoNoSemantico = filaProyectoRdfType[0].RdfType;
            }
            return rdfConfiguradoRecursoNoSemantico;
        }

        /// <summary>
        /// Comprueba si el xml del campo RDF es inválido
        /// </summary>
        /// <param name="pRdfText"></param>
        /// <returns></returns>
        static bool IsValidXmlString(string pRdfText)
        {
            try
            {
                XmlConvert.VerifyXmlChars(pRdfText);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Elimina los caracteres inválidos en un xml
        /// </summary>
        /// <param name="pRdfText"></param>
        /// <returns></returns>
        static string RemoveInvalidXmlChars(string pRdfText)
        {
            string correctedXMlString = Regex.Replace(pRdfText, @"[^\u0000-\u007F]", string.Empty);

            if (IsValidXmlString(pRdfText))
            {
                var validXmlChars = pRdfText.Where(ch => XmlConvert.IsXmlChar(ch)).ToArray();
                return new string(validXmlChars);
            }

            return correctedXMlString;
        }


        /// <summary>
        /// Copia las imagenes de un documento semántico.
        /// </summary>
        /// <param name="pDocumentoOriginalID">Identificador del documento semántico original</param>
        /// <param name="pDocumentoNuevoID">Identificador del documento semántico nuevo</param>
        /// <param name="pUrlIntragnossServicios">Url intragnoss para los servicios web</param>
        /// <returns>TRUE si se han copiado los documento o no había nada que copiar. FALSE si algo ha fallado</returns>
        private bool CopiarImagenDocumentoSemantico(Guid pDocumentoOriginalID, Guid pDocumentoNuevoID, string pUrlIntragnossServicios)
        {
            ServicioImagenes sImagenes = new ServicioImagenes(mLoggingService, mConfigService);
            Stopwatch sw = LoggingService.IniciarRelojTelemetria();

            try
            {
                sImagenes.Url = pUrlIntragnossServicios;
                bool copiado = sImagenes.CopiarImagenesSemanticas(pDocumentoOriginalID, pDocumentoNuevoID);
                mLoggingService.AgregarEntradaDependencia("Copiar imagen desde servicio imagenes", false, "CopiarImagenDocumentoSemantico", sw, true);
                return copiado;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
                mLoggingService.AgregarEntradaDependencia("No copiada imagen desde servicio imagenes", false, "CopiarImagenDocumentoSemantico", sw, false);
            }

            return false;
        }

        /// <summary>
        /// Copia los archivos link de un documento semántico.
        /// </summary>
        /// <param name="pDocumentoOriginalID">Identificador del documento semántico original</param>
        /// <param name="pDocumentoNuevoID">Identificador del documento semántico nuevo</param>
        /// <param name="pUrlIntragnossServicios">Url intragnoss para los servicios web</param>
        /// <returns>TRUE si se han copiado los documento o no había nada que copiar. FALSE si algo ha fallado</returns>
        private bool CopiarArchivosLinkDocumentoSemantico(Guid pDocumentoOriginalID, Guid pDocumentoNuevoID, string pUrlIntragnossServicios)
        {
            //TODO migrar a peticion rest
            /*Es.Riam.Gnoss.Web.Controles.ServicioDocumentosLinkWS.ServicioDocumentosLink servicioDocsLink = new Es.Riam.Gnoss.Web.Controles.ServicioDocumentosLinkWS.ServicioDocumentosLink();
            Stopwatch sw = LoggingService.IniciarRelojTelemetria();

            try
            {
                servicioDocsLink.Url = pUrlIntragnossServicios + "/ServicioDocumentosLink.asmx";
                bool copiado = servicioDocsLink.CopiarArchivosDocumento(pDocumentoOriginalID, pDocumentoNuevoID);
                mLoggingService.AgregarEntradaDependencia("Copiar archivo link desde servicio doc links", false, "CopiarImagenDocumentoSemantico", sw, true);
                return copiado;
            }
            catch (Exception ex)
            {
                mLoggingService.AgregarEntradaDependencia("No copiado archivo link desde servicio doc links", false, "CopiarImagenDocumentoSemantico", sw, false);
            }
            finally
            {
                if (servicioDocsLink != null)
                {
                    servicioDocsLink.Dispose();
                    servicioDocsLink = null;
                }
            }*/
            return false;
        }

        /// <summary>
        /// Copia los archivos de un documento semántico.
        /// </summary>
        /// <param name="pDocumentoOriginalID">Identificador del documento semántico original</param>
        /// <param name="pDocumentoNuevoID">Identificador del documento semántico nuevo</param>
        /// <param name="pUrlServicioDocs">Url para el servicio de documentación</param>
        /// <returns>TRUE si se han copiado los documento o no había nada que copiar. FALSE si algo ha fallado</returns>
        private bool CopiarArchivosDocumentoSemantico(Guid pDocumentoOriginalID, Guid pDocumentoNuevoID, string pUrlServicioDocs)
        {
            GestionDocumental gestionDoc = new GestionDocumental(mLoggingService, mConfigService);
            Stopwatch sw = LoggingService.IniciarRelojTelemetria();

            try
            {
                gestionDoc.Url = pUrlServicioDocs;
                bool copiado = gestionDoc.CopiarDocumentosDeDirectorio(Path.Combine(UtilArchivos.ContentDocumentosSem, UtilArchivos.DirectorioDocumento(pDocumentoOriginalID)), UtilArchivos.ContentDocumentosSem + "\\" + UtilArchivos.DirectorioDocumento(pDocumentoNuevoID));
                mLoggingService.AgregarEntradaDependencia("Copiar archivo desde servicio docs", false, "CopiarImagenDocumentoSemantico", sw, true);
                return copiado;
            }
            catch (Exception)
            {
                mLoggingService.AgregarEntradaDependencia("No copiado archivo desde servicio docs", false, "CopiarImagenDocumentoSemantico", sw, false);
            }

            return false;
        }

        /// <summary>
        /// Carga toda la información de las versiones de un documento pasado por parámetro
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        private void CargarTodoVersionesDocumento(Guid pDocumentoID)
        {
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<Guid> listaDocumentosID = new List<Guid>();
            listaDocumentosID.Add(pDocumentoID);

            docCN.ObtenerVersionDocumentosPorIDs(GestorDocumental.DataWrapperDocumentacion, listaDocumentosID, true);

            List<AD.EntityModel.Models.Documentacion.VersionDocumento> filasVersionDoc = GestorDocumental.DataWrapperDocumentacion.ListaVersionDocumento.Where(version => version.DocumentoID.Equals(pDocumentoID)).ToList();
            if (filasVersionDoc.Count > 0)
            {
                List<Guid> listaDocumentoVersionados = GestorDocumental.ObtenerListaDocumentosVersionadosDe(filasVersionDoc.First().DocumentoOriginalID);
                foreach (Guid documentoID in listaDocumentoVersionados)
                {
                    if (documentoID != pDocumentoID)
                    {
                        CargarNecesarioParaDuplicacion(documentoID, true);
                    }
                }
                CargarNecesarioParaDuplicacion(filasVersionDoc[0].DocumentoOriginalID, true);
            }
        }

        /// <summary>
        /// Obtiene la fila de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public AD.EntityModel.Models.ProyectoDS.Proyecto ObtenerFilaProyecto(Guid pProyectoID)
        {
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            return proyCL.ObtenerFilaProyecto(pProyectoID);
        }

        /// <summary>
        /// Parámetros de un proyecto.
        /// </summary>
        private Dictionary<string, string> ObtenerParametroProyecto(Guid pProyectoID)
        {
            ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            Dictionary<string, string> parametroProyecto = proyectoCL.ObtenerParametrosProyecto(pProyectoID);
            proyectoCL.Dispose();

            return parametroProyecto;
        }

        #endregion

        #endregion
    }
}
