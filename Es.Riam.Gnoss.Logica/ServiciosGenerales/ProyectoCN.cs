using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModel.Models.Carga;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace Es.Riam.Gnoss.Logica.ServiciosGenerales
{
    /// <summary>
    /// Lógica referente a Proyecto Gnoss
    /// </summary>
    public class ProyectoCN : BaseCN, IDisposable
    {

        #region Miembros

        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ProyectoCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;

            ProyectoAD = new ProyectoAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public ProyectoCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;

            ProyectoAD = new ProyectoAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }

        #endregion

        #region Métodos generales

        #region Públicos

        /// <summary>
        /// Obtiene la fecha de alta del grupo de organización en un proyecto
        /// </summary>
        /// <param name="pGrupoID">Identificador del grupo de organización</param>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>DateTime con la fecha de alta del grupo en el proyecto. Null en caso de no encontrarlo</returns>
        public DateTime? ObtenerFechaAltaGrupoOrganizacionEnProyecto(Guid pGrupoID, Guid pOrganizacionID, Guid pProyectoID)
        {
            return ProyectoAD.ObtenerFechaAltaGrupoOrganizacionEnProyecto(pGrupoID, pOrganizacionID, pProyectoID);
        }

        /// <summary>
        /// Obtiene los proyectos en los que participa un determinado grupo de organización
        /// </summary>
        /// <param name="pGrupoID">Identificador del grupo de organización</param>
        /// <returns>Lista de filas de GrupoOrgParticipaProy con los proyectos del grupo</returns>
        public List<GrupoOrgParticipaProy> ObtenerProyectosParticipaGrupoOrganizacion(Guid pGrupoID)
        {
            return ProyectoAD.ObtenerProyectosParticipaGrupoOrganizacion(pGrupoID);
        }

        /// <summary>
        /// Obtiene los gadget por idioma asociados al identificador del gadget pasado por parámetro
        /// </summary>
        /// <param name="pGadgetID">Identificador del gadget</param>
        /// <returns></returns>
        public List<ProyectoGadgetIdioma> ObtenerProyectoGadgetIdiomaDeGadget(Guid pGadgetID)
        {
            return ProyectoAD.ObtenerProyectoGadgetIdiomaDeGadget(pGadgetID);
        }

        /// <summary>
        /// Obtiene los proyectos en los que participa un determinado grupo de organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Lista de filas de GrupoOrgParticipaProy con los grupos del proyecto</returns>
        public List<GrupoOrgParticipaProy> ObtenerGruposOrganizacionParticipanProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return ProyectoAD.ObtenerGruposOrganizacionParticipanProyecto(pOrganizacionID, pProyectoID);
        }

        /// <summary>
        /// Elimina la fila de GrupoOrgParticipaProy.
        /// </summary>
        /// <param name="pGrupoID">Identificador del grupo de organización</param>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void BorrarFilaGrupoOrgParticipaProy(Guid pGrupoID, Guid pOrganizacionID, Guid pProyectoID)
        {
            ProyectoAD.BorrarFilaGrupoOrgParticipaProy(pGrupoID, pOrganizacionID, pProyectoID);
        }
        /// <summary>
        /// Actualiza la tabla PreferenciaProyecto con la preferencia de los proyectos seleccionados
        /// </summary>
        /// <param name="listaCategoriasSeleccionadas">Lista de IDs de las categorias seleccionadas</param>
        /// <param name="pProyectoID">ID del proyecto seleccionado</param>
        public void ActualizarTablaPreferenciaProyecto(List<Guid> listaCategoriasSeleccionadas, Guid pProyectoID)
        {
            ProyectoAD.ActualizarTablaPreferenciaProyecto(listaCategoriasSeleccionadas, pProyectoID);
        }
        /// <summary>
        /// Devuelve los IDs de las categorias seleccionadas
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>Lista de los IDs de las categorias seleccionadas</returns>
        public List<Guid> ObtenerCategoriasSeleccionadas(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerCategoriasSeleccionadas(pProyectoID);
        }
        /// <summary>
        /// Crea una fila de GrupoOrgParticipaProy.
        /// </summary>
        /// <param name="pGrupoID">Identificador del grupo de organización</param>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pTipoIdentidad">Tipo de perfil con el que participan los miembros del grupo</param>
        public void AddFilaGrupoOrgParticipaProy(Guid pGrupoID, Guid pOrganizacionID, Guid pProyectoID, TiposIdentidad pTipoIdentidad)
        {
            ProyectoAD.AddFilaGrupoOrgParticipaProy(pGrupoID, pOrganizacionID, pProyectoID, pTipoIdentidad);
        }

        /// <summary>
        /// Obtiene los permisos de páginas de los usuarios en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Lista de filas de PermisosPaginasUsuarios</returns>
        public List<PermisosPaginasUsuarios> ObtenerPermisosPaginasProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return ProyectoAD.ObtenerPermisosPaginasProyecto(pOrganizacionID, pProyectoID);
        }

        /// <summary>
        /// Obtiene los permisos de páginas del usuario en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Lista de filas de PermisosPaginasUsuarios del usuario en el proyecto</returns>
        public List<PermisosPaginasUsuarios> ObtenerPermisosPaginasProyectoUsuarioID(Guid pOrganizacionID, Guid pProyectoID, Guid pUsuarioID)
        {
            return ProyectoAD.ObtenerPermisosPaginasProyectoUsuarioID(pOrganizacionID, pProyectoID, pUsuarioID);
        }

        public string ObtieneDescripciondeNivelCertificacion(string pOrden, Guid pProyectoID)
        {
            return ProyectoAD.ObtieneDescripciondeNivelCertificacion(pOrden, pProyectoID);
        }

        /// <summary>
        /// Obtiene los permisos de páginas del usuario en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pTipoPagina">Página sobre la que se comprueba el permiso</param>
        /// <returns>True si el usuario tiene permiso sobre el tipo de página en el proyecto</returns>
        public bool TienePermisoUsuarioEnPagina(Guid pOrganizacionID, Guid pProyectoID, Guid pUsuarioID, TipoPaginaAdministracion pTipoPagina)
        {
            return ProyectoAD.TienePermisoUsuarioEnPagina(pOrganizacionID, pProyectoID, pUsuarioID, pTipoPagina);
        }

        /// <summary>
        /// Obtiene la lista de presentaciones mapa semántico para el proyecto indicado
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto del cual queremos obtener el listado</param>
        /// <returns></returns>
        public List<PresentacionMapaSemantico> ObtenerListaPresentacionMapaSemantico(Guid pProyectoID, string pNombreOnto = "")
        {
            return ProyectoAD.ObtenerListaPresentacionMapaSemantico(pProyectoID);
        }

        /// <summary>
        /// Obtiene una RedireccionRegistroRuta por su id y sus RedireccionValorParametro asociadas 
        /// </summary>
        /// <param name="pDominio"></param>
        /// <returns></returns>
        public RedireccionRegistroRuta ObtenerRedireccionRegistroRutaPorRedireccionID(Guid pRedireccionID)
        {
            return ProyectoAD.ObtenerRedireccionRegistroRutaPorRedireccionID(pRedireccionID);
        }

        /// <summary>
        /// Obtiene la lista de pestanyas configuradas para el proyectoID pasado por parámetro
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto del cual queremos obtener las pestanñas</param>
        /// <returns></returns>
        public List<ProyectoPestanyaMenu> ObtenerProyectoPestanyaMenuPorProyectoID(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerProyectoPestanyaMenuPorProyectoID(pProyectoID);
        }

        /// <summary>
        /// Nos indica si actualmente existen permisos para administrar los documentos semánticos
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Si existe o no permisos para que se puedan administrar los documentos semánticos</returns>
        public bool ExisteTipoDocDispRolUsuarioProySemantico(Guid pProyectoID)
        {
            return ProyectoAD.ExisteTipoDocDispRolUsuarioProySemantico(pProyectoID);
        } 
        
        /// <summary>
        /// Nos indica si actualmente existen permisos para administrar los documentos semánticos
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Si existe o no permisos para que se puedan administrar los documentos semánticos</returns>
        public bool ExisteTipoOntoDispRolUsuarioProy(Guid pProyectoID, Guid pDocumentoID)
        {
            return ProyectoAD.ExisteTipoOntoDispRolUsuarioProy(pProyectoID, pDocumentoID);
        }

        /// <summary>
        /// Obtiene las pestanyas del prodecto que tenemos permitido ver si son privadas para ciertas identidades o grupos solo vere
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto del cual queremos obtener las pestanyas</param>
        /// <param name="pIdentidadID">Identificador de la identidad del usuario que esta navegando</param>
        /// <returns></returns>
        public List<ProyectoPestanyaMenu> ObtenerPestanyasDeProyectoSegunPrivacidadDeIdentidad(Guid pProyectoID, Guid pIdentidadID)
        {
            return ProyectoAD.ObtenerPestanyasDeProyectoSegunPrivacidadDeIdentidad(pProyectoID, pIdentidadID);
        }

        /// <summary>
        /// Obtiene los tipo de documentos permitidos para un rol de usuario en un determinado proyecto.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto en el que se hace la comprobación</param>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Lista con los tipos de documentos permitidos para el rol</returns>
        public List<TiposDocumentacion> ObtenerTiposDocumentosPermitidosUsuarioEnProyectoPorUsuID(Guid pProyectoID, Guid pUsuarioID)
        {
            return ProyectoAD.ObtenerTiposDocumentosPermitidosUsuarioEnProyectoPorUsuID(pProyectoID, pUsuarioID);
        }

        /// <summary>
        /// Obtiene las redirecciones de un dominio
        /// </summary>
        /// <param name="pDominio"></param>
        /// <returns></returns>
        public List<RedireccionRegistroRuta> ObtenerRedireccionRegistroRutaPorDominio(string pDominio, bool pMantenerContextoAbierto)
        {
            return ProyectoAD.ObtenerRedireccionRegistroRutaPorDominio(pDominio, pMantenerContextoAbierto);
        }
        /// <summary>
        /// Obtiene la lista de servicios externos si es MetaProyecto
        /// </summary>
        /// <returns>Lista de servicios externos</returns>
        public List<EcosistemaServicioExterno> ObtenerEcosistemaServicioExterno()
        {
            return ProyectoAD.ObtenerEcosistemaServicioExterno();
        }
        /// <summary>
        /// Obtiene las lista de servicios externos si no es MetaProyecto
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <returns>Lista de servicios externos</returns>
        public List<ProyectoServicioExterno> ObtenerProyectoServicioExterno(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerProyectoServicioExterno(pProyectoID);
        }
        /// <summary>
        /// Obtiene el valor del ParametroAplicacion
        /// </summary>
        /// <param name="parametro"></param>
        /// <returns>Valor de parametro pasado</returns>
        public string ObtenerParametroAplicacion(string parametro)
        {
            return ProyectoAD.ObtenerParametroAplicacion(parametro);
        }
        /// <summary>
        /// Guarda un nuevo parametro aplicacion
        /// </summary>
        /// <param name="parametro">Nombre parametro de aplicacion</param>
        /// <param name="valor">Valor del paramentro</param>   
        public void GuardarParamentroAplicacion(string parametro, string valor)
        {
            ProyectoAD.GuardarParametroAplicacion(parametro, valor);
        }
        /// <summary>
        /// Guarda en la tabla EcosistemaServicioExterno una nueva fila
        /// </summary>
        /// <param name="eco">Objeto de tipo EcosistemaServicioExterno</param> 
        public void ActualizarServiceNameEcosistema(EcosistemaServicioExterno eco)
        {
            ProyectoAD.ActualizarServiceNameEcosistema(eco);
        }
        /// <summary>
        ///  Guarda en la tabla ProyectoServicioExterno una nueva fila
        /// </summary>
        /// <param name="proy">Objeto de tipo ProyectoServicioExterno</param> 
        public void ActualizarServiceNameProyecto(ProyectoServicioExterno proy)
        {
            ProyectoAD.ActualizarServiceNameProyecto(proy);
        }
        /// <summary>
        ///  Elimina de la tabla EcosistemaServicioExterno una fila
        /// </summary>
        /// <param name="eco">Objeto de tipo EcosistemaServicioExterno</param> 
        public void EliminarEcosistemaServicioExterno(EcosistemaServicioExterno eco)
        {
            ProyectoAD.EliminarEcosistemaServicioExterno(eco);
        }
        /// <summary>
        ///  Elimina de la tabla ProyectoServicioExterno una fila
        /// </summary>
        /// <param name="proy">Objeto de tipo ProyectoServicioExterno</param>
        public void EliminarProyectoServicioExterno(ProyectoServicioExterno proy)
        {
            ProyectoAD.EliminarProyectoServicioExterno(proy);
        }
        /// <summary>
        ///  Obtiene la OrganizacionID a la que corresponde un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>ID de la organizacion</returns>
        public Guid ObtenerOrganizacionIDAPartirDeProyectoID(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerOrganizacionIDAPartirDeProyectoID(pProyectoID);
        }
        /// <summary>
        /// Obtiene una lista con registros de un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>Lista con los registros de un proyecto</returns>
        public List<string> ObtenerProyectoPasoRegistro(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerProyectoPasoRegistro(pProyectoID);
        }
        /// <summary>
        /// Obtiene una lista con las pestanyas para los pasos del registro
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>Lista con las Pestanyas del proyecto</returns>
        public List<ProyectoPestanyaMenu> ListaPestanyasMenuRegistro(Guid pProyectoID)
        {
            return ProyectoAD.ListaPestanyasMenuRegistro(pProyectoID);
        }
        /// <summary>
        /// Guarda en la base de datos los registros por pasos en la tabla ProyectoPasoRegistro
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="listaPasos">Lista con los registros a guardar</param>
        /// <returns></returns>
        public void GuardarRegistroPorPasos(Guid pProyectoID, List<PasoRegistroModel> listaPasos)
        {
            ProyectoAD.GuardarRegistroPorPasos(pProyectoID, listaPasos);
        }
        /// <summary>
        /// Obtiene una lista con la obligatoriedad de los registros
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>Lista con los la obligatoriedad de los registros</returns>
        public List<bool> ObtenerListaObligatoriedadRegistros(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerListaObligatoriedadRegistros(pProyectoID);
        }

        /// <summary>
        /// Añade a la base de datos la redirección indicada y su valor con parámetro de tener
        /// </summary>
        /// <param name="pRedireccionRegistroRuta"></param>
        public void AniadirRedireccionRegistroRuta(RedireccionRegistroRuta pRedireccionRegistroRuta)
        {
            ProyectoAD.AniadirRedireccionRegistroRuta(pRedireccionRegistroRuta);
        }

        /// <summary>
        /// Añade la el valor parámetro de la redirección indicado a la base de datos
        /// </summary>
        /// <param name="pRedireccionValorParametro"></param>
        public void AniadirRedireccionValorParametro(RedireccionValorParametro pRedireccionValorParametro)
        {
            ProyectoAD.AniadirRedireccionValorParametro(pRedireccionValorParametro);
        }

        /// <summary>
        /// Obtiene todas los valores parametro de la redireccion idicada
        /// </summary>
        /// <param name="pRedireccionID"></param>
        /// <returns></returns>
        public List<RedireccionValorParametro> ObtenerRedireccionValorParametroPorRedireccionID(Guid pRedireccionID)
        {
            return ProyectoAD.ObtenerRedireccionValorParametroPorRedireccionID(pRedireccionID);
        }

        /// <summary>
        /// Crea/Edita las filas de RedireccionRegistroRuta y sus filas de RedireccionValorParametro asociadas.
        /// </summary>
        /// <param name="pRedireccionID"></param>
        /// <param name="pUrlOrigen"></param>
        /// <param name="pDominio"></param>
        /// <param name="pNombreParametro"></param>
        /// <param name="pMantenerFiltros"></param>
        /// <param name="pValoresRedirecciones"></param>
        public void GuardarFilaRedireccionRegistroRuta(Dictionary<RedireccionRegistroRuta, bool> pDicFilasRedireccion, bool pRetrasarGuardado)
        {
            ProyectoAD.GuardarFilaRedireccionRegistroRuta(pDicFilasRedireccion, pRetrasarGuardado);
        }

        public List<IntegracionContinuaPropiedad> ObtenerFilasIntegracionContinuaParametro(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerFilasIntegracionContinuaParametro(pProyectoID);
        }

        /// <summary>
        /// Crea una nueva carga masiva en la base de datos
        /// </summary>
        /// <param name="idCarga">Id de la carga</param>
        /// <param name="estado">Estado de la carga</param>
        /// <param name="fechaAlta">Fecha de creación de la carga</param>
        /// <param name="proyectoId">Id el proyecto al que pertenece la carga</param>
        /// <param name="identidadId">Id de la identidad del sujeto de la carga</param>
        /// <param name="nombre">Nombre de la carga</param>
        /// <param name="organizacionId">Id de la organizacion de la carga</param>
        /// <returns>Devuelve cierto si se ha creado la carga</returns>
        public bool CrearNuevaCargaMasiva(Guid idCarga, int estado, DateTime fechaAlta, Guid proyectoId, Guid identidadId, string nombre = null, Guid? organizacionId = null)
        {
            return ProyectoAD.CrearNuevaCargaMasiva(idCarga, estado, fechaAlta, proyectoId, identidadId, nombre, organizacionId);
        }

        public List<string> ObtenerPropiedadesSearch(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerPropiedadesSearch(pProyectoID);
        }

        /// <summary>
        /// Devuelve la lista de paquetes de una carga
        /// </summary>
        /// <param name="cargaID">Id de la carga</param>
        /// <returns>Lista de los paquetes de la carga</returns>
        public List<CargaPaquete> ObtenerPaquetesPorIDCarga(Guid cargaID)
        {
            return ProyectoAD.ObtenerPaquetesPorIDCarga(cargaID);
        }
        /// <summary>
        /// Devuelve la lista de cargas de una identidadID
        /// </summary>
        /// <param name="identidadId">Id de la identidad de la carga</param>
        /// <returns>Lista de cargas correspondientes a la identidad</returns>
        public List<Carga> ObtenerCargasMasivasPorIdentidadID(Guid identidadId)
        {
            return ProyectoAD.ObtenerCargasMasivasPorIdentidadID(identidadId);
        }
        /// <summary>
        /// Crea un nuevo paquete asociado a una carga
        /// </summary>
        /// <param name="paqueteID">Id del paquete</param>
        /// <param name="cargaId">Id de la carga</param>
        /// <param name="rutaOnto">Ruta del archivo de triples de la ontologia</param>
        /// <param name="rutaSearch">Ruta del archivo de triples busqueda</param>
        /// <param name="rutaSql">Ruta del archivo del sql</param>
        /// <param name="estado">Estado del paquete</param>
        /// <param name="error">Error del paquete</param>
        /// <param name="fechaAlta">Fecha en la que se crea el paquete</param>
        /// <param name="ontologia">Ontologia a la que pertenece el paquete</param>
        /// <param name="comprimido">Los archivos estan comprimidos</param>
        /// <param name="fechaProcesado">Fecha en la que se ha procesado el paquete</param>
        /// <returns>Devuelve cierto si se ha creado el paquete</returns>
        public bool CrearNuevoPaqueteCargaMasiva(Guid paqueteID, Guid cargaId, string rutaOnto, string rutaSearch, string rutaSql, int estado, string error, DateTime? fechaAlta, string ontologia, bool comprimido = false, DateTime? fechaProcesado = null)
        {
            return ProyectoAD.CrearNuevoPaqueteCargaMasiva(paqueteID, cargaId, rutaOnto, rutaSearch, rutaSql, estado, error, fechaAlta, ontologia, comprimido, fechaProcesado);
        }
        public void CrearFilasIntegracionContinuaParametro(List<IntegracionContinuaPropiedad> pListaPropiedades, Guid pProyectoID, TipoObjeto pTipoObjeto, string pID = null)
        {
            ProyectoAD.CrearFilasIntegracionContinuaParametro(pListaPropiedades, pProyectoID, pTipoObjeto, pID);
        }
        public void GuardarFilasIntegracionContinuaParametro(List<IntegracionContinuaPropiedad> pListaPropiedades, Guid pProyectoID)
        {
            ProyectoAD.GuardarFilasIntegracionContinuaParametro(pListaPropiedades, pProyectoID);
        }

        /// <summary>
        /// Elimina la lista de filas de RedireccionRegistroRuta y sus filas de RedireccionValorParametro asociadas.
        /// </summary>
        /// <param name="pGrupoID">Identificador del grupo de organización</param>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void BorrarFilaRedireccionRegistroRuta(List<Guid> pListaRedireccionesID, bool pRetrasarGuardado)
        {
            ProyectoAD.BorrarFilaRedireccionRegistroRuta(pListaRedireccionesID, pRetrasarGuardado);
        }

        /// <summary>
        /// Elimina la fila de RedireccionRegistroRuta y sus filas de RedireccionValorParametro asociadas.
        /// </summary>
        /// <param name="pRedireccionID">Lista de identificadores de redirección</param>
        public void BorrarRedireccionRegistroRuta(Guid pRedireccionID)
        {
            ProyectoAD.BorrarRedireccionRegistroRuta(pRedireccionID);
        }

        /// <summary>
        /// Elimina la fila de RedireccionRegistroRuta y sus filas de RedireccionValorParametro asociadas.
        /// </summary>
        /// <param name="pListaValoresRedireccionesID">Diccionario de identificadores de redirección y lista de valores de parámetros</param>
        public void BorrarFilasRedireccionValorParametro(List<RedireccionValorParametro> pFilasValores, bool pRetrasarGuardado)
        {
            ProyectoAD.BorrarFilasRedireccionValorParametro(pFilasValores, pRetrasarGuardado);
        }

        /// <summary>
        /// Obtiene la lista de proyectos con acciones externas
        /// </summary>
        /// <returns>Lista de Identificadores de proyecto con acciones externas configuradas</returns>
        public List<Guid> ObtenerListaIDsProyectosConAccionesExternas()
        {
            return ProyectoAD.ObtenerListaIDsProyectosConAccionesExternas();
        }
        /// <summary>
        /// Obtiene los datos de una carga a partir de su ID
        /// </summary>
        /// <param name="pCargaID">Identificador de la carga</param>
        /// <returns>Datos de la carga masiva</returns>
        public Carga ObtenerDatosCargaPorID(Guid pCargaID)
        {
            return ProyectoAD.ObtenerDatosCargaPorID(pCargaID);
        }
        /// <summary>
        /// Obtener los datos del paquete de una carga masiva
        /// </summary>
        /// <param name="pPaqueteID"></param>
        /// <returns></returns>
        public CargaPaquete ObtenerDatosPaquetePorID(Guid pPaqueteID)
        {
            return ProyectoAD.ObtenerDatosPaquete(pPaqueteID);
        }
        /// <summary>
        /// Obtienen los proyectos a los que acceden las identidades que tienen acceso a un proyecto pasado como parámetro
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Dataset de proyecto</returns>
        public DataWrapperProyecto ObtenerProyectosDeIdentidadesAccedenAProyecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerProyectosDeIdentidadesAccedenAProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene los niveles de certificación de un proyecto pasado como parámetro
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Dataset de proyecto</returns>
        public DataWrapperProyecto ObtenerNivelesCertificacionRecursosProyecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerNivelesCertificacionRecursosProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene una DataSet sin tipar con una tabla "Emails" que contiene los campos (IdentidadID,PersonaID,Nombre,Email) 
        /// de cada uno de los miembros que participan en un determinado proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>DataSet sin tipar</returns>
        public List<EmailsMiembrosDeProyecto> ObtenerEmailsMiembrosDeProyecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerEmailsMiembrosDeProyecto(pProyectoID);
        }


        /// <summary>
        /// Obtiene una DataSet sin tipar con una tabla "Emails" que contiene los campos (IdentidadID,PersonaID,Nombre,Apellidos,Email) de cada uno de los miembros que participan en un determinado evento
        /// </summary>
        /// <param name="pEventoID">Clave del evento</param>
        /// <returns>DataSet</returns>
        public DataSet ObtenerEmailsMiembrosDeEventoDeProyecto(Guid pEventoID)
        {
            return ProyectoAD.ObtenerEmailsMiembrosDeEventoDeProyecto(pEventoID);
        }

        /// <summary>
        /// Devuelve una lista con los emails de los administradores del proyecto.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>DataSet sin tipar</returns>
        public List<string> ObtenerEmailsAdministradoresDeProyecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerEmailsAdministradoresDeProyecto(pProyectoID);
        }

        /// <summary>
        /// Comprueba si existen documentos cuyo nivel de certificacion sea el pasado por parámetro
        /// </summary>
        /// <param name="pNivelCertificacionID">Identificador de nivel de certificación</param>
        /// <returns>TRUE si existe algún documento, FALSE en caso contrario</returns>
        public bool ExisteDocAsociadoANivelCertif(Guid pNivelCertificacionID)
        {
            return ProyectoAD.ExisteDocAsociadoANivelCertif(pNivelCertificacionID);
        }


        /// <summary>
        /// Comprueba si existen documentos que su nivel de certificación sea el pasado por parámetro
        /// </summary>
        /// <param name="pNivelesCertificacionID">Identificador de los niveles de certificación</param>
        /// <returns>Lista con los niveles de certificacion y un booleano que indica si tiene documentos</returns>
        public Dictionary<Guid, bool> ExisteDocAsociadoANivelCertif(List<Guid> pNivelesCertificacionID)
        {
            return ProyectoAD.ExisteDocAsociadoANivelCertif(pNivelesCertificacionID);
        }

        /// <summary>
        /// Obtiene los gadgets de un proyecto que se le pasa por parametros
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto del que queremos obtener los gadgets</param>
        /// <param name="pProyectoDS">Dataset de proyectos</param>
        /// <returns>DataSet con los gadgets del proyecto que se pasa por parametros</returns>
        public void ObtenerGadgetsProyecto(Guid pProyectoID, DataWrapperProyecto pProyectoDS)
        {
            ProyectoAD.ObtenerGadgetsProyecto(pProyectoID, pProyectoDS);
        }

        /// <summary>
        /// Obtiene los gadgets del tipo indicado de un proyecto que se le pasa como parámetro
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto del que queremos obtener los gadgets</param>
        /// <param name="pProyectoDS">Dataset de proyectos</param>
        /// <param name="pTipoUbicacionGadget">Indica la ubicación de los gadgets que se van a cargar(0-home, 1-ficha recursos)</param>
        /// <returns>DataSet con los gadgets del tipo indicado del proyecto que se pasa por parámetro</returns>
        public void ObtenerGadgetsProyectoUbicacion(Guid pProyectoID, DataWrapperProyecto pProyectoDS, TipoUbicacionGadget pTipoUbicacionGadget)
        {

            ProyectoAD.ObtenerGadgetsProyectoUbicacion(pProyectoID, pProyectoDS, pTipoUbicacionGadget);
        }

        /// <summary>
        /// Comprueba si existe un nombre corto de ProyectoGadget para el proyecto
        /// </summary>
        /// <param name="pNombreCortoGadget">Nombre corto del gadget</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>TRUE si algún gadget tiene ese nombre corto</returns>
        public bool ExisteNombreCortoProyectoGadget(string pNombreCortoGadget, Guid pProyectoID)
        {
            return ProyectoAD.ExisteNombreCortoProyectoGadget(pNombreCortoGadget, pProyectoID);
        }

        /// <summary>
        /// Obtiene los gadgets de un proyecto origen que se le pasa como parámetro
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto origen del que queremos obtener los gadgets</param>
        /// <param name="pDataWrapperProyecto">Dataset de proyectos</param>
        /// <returns>DataSet con los gadgets del proyecto que se pasa por parametros</returns>
        public void ObtenerGadgetsProyectoOrigen(Guid pProyectoOrigenID, DataWrapperProyecto pDataWrapperProyecto)
        {
            ProyectoAD.ObtenerGadgetsProyectoOrigen(pProyectoOrigenID, pDataWrapperProyecto);
        }

        /// <summary>
        /// Comprueba la existencia de gadgets de tipo Recursos Relacionados
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto origen</param>
        /// <returns>True si existen gadgets de tipo Recursos Relacionados</returns>
        public bool TieneGadgetRecursosRelacionados(Guid pProyectoID)
        {
            return ProyectoAD.TieneGadgetRecursosRelacionados(pProyectoID);
        }

        /// <summary>
        /// Obtiene el gadget que se le pasa por parametros
        /// </summary>
        /// <param name="pGadgetID">Clave del gadget</param>
        /// <param name="pDataWrapperProyecto">Dataset de proyectos</param>
        /// <returns>DataSet con los gadgets del proyecto que se pasa por parametros</returns>
        public void ObtenerGadget(Guid pGadgetID, DataWrapperProyecto pDataWrapperProyecto, Guid pProyectoID)
        {
            ProyectoAD.ObtenerGadget(pGadgetID, pDataWrapperProyecto, pProyectoID);
        }
        /// <summary>
        /// Obtiene el gadget que se le pasa por parametros
        /// </summary>
        /// <param name="pGadgetID">Clave del gadget</param>
        /// <param name="pProyectoDS">Dataset de proyectos</param>
        /// <returns>DataSet con los gadgets del proyecto que se pasa por parametros</returns>
        public DataWrapperProyecto ObtenerDataSetGadget(Guid pGadgetID, Guid ProyectoID)
        {
            return ProyectoAD.ObtenerDataSetGadget(pGadgetID, ProyectoID);
        }

        /// <summary>
        /// Obtiene el gadget que se le pasa por parametros
        /// </summary>
        /// <param name="pNombreCorto">Nombrecorto del gadget</param>
        /// <param name="pDataWrapperProyecto">Dataset de proyectos</param>
        /// <returns>DataSet con los gadgets del proyecto que se pasa por parametros</returns>
        public void ObtenerGadgetContextoPorNombreCorto(string pNombreCorto, DataWrapperProyecto pDataWrapperProyecto)
        {
            ProyectoAD.ObtenerGadgetContextoPorNombreCorto(pNombreCorto, pDataWrapperProyecto);
        }

        public bool ExisteGadgetContextoPorNombreCorto(string pNombreCorto)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            ObtenerGadgetContextoPorNombreCorto(pNombreCorto, dataWrapperProyecto);
            return dataWrapperProyecto.ListaProyectoGadget.Any();
        }
        /// <summary>
        /// Obtiene las pestañas de menú un proyecto que se le pasa por parametros
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto del que queremos obtener los gadgets</param>
        /// <returns>Lista con las pestañas del proyecto que se pasa por parametros</returns>
        public Dictionary<Guid, string> ObtenerPestanyasProyectoNombre(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerPestanyasProyectoNombre(pProyectoID);
        }

        /// <summary>
        /// Obtiene las pestañas de un proyecto que se le pasa por parametros
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto del que queremos obtener los gadgets</param>
        /// <param name="pDataWrapperProyecto">Dataset de proyectos</param>
        /// <returns>DataSet con las pestañas del proyecto que se pasa por parametros</returns>
        public void ObtenerPestanyasProyecto(Guid? pProyectoID, DataWrapperProyecto pDataWrapperProyecto)
        {
            ProyectoAD.ObtenerPestanyasProyecto(pProyectoID, pDataWrapperProyecto, false);
        }

        /// <summary>
        /// Obtiene las pestañas de un proyecto que se le pasa por parametros
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto del que queremos obtener los gadgets</param>
        /// <param name="pDataWrapperProyecto">Dataset de proyectos</param>
        /// <returns>DataSet con las pestañas del proyecto que se pasa por parametros</returns>
        public void ObtenerPestanyasProyecto(Guid? pProyectoID, DataWrapperProyecto pDataWrapperProyecto, bool pOmitirGenericas)
        {
            ProyectoAD.ObtenerPestanyasProyecto(pProyectoID, pDataWrapperProyecto, pOmitirGenericas);
        }


        /// <summary>
        /// Obtiene las pestañas de un proyecto que se le pasa por parametros
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto del que queremos obtener los gadgets</param>
        /// <returns>DataSet con las páginas html del proyecto que se pasa por parametros</returns>
        public DataWrapperProyecto ObtenerPaginasHtmlProyecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerPaginasHtmlProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene la tabla RecursosRelacionadosPresentacion del proyecto
        /// </summary>
        /// <param name="pProyectoID">Clave del gadget</param>
        /// <returns>DataSet con RecursosRelacionadosPresentacion</returns>
        public DataWrapperProyecto ObtenerRecursosRelacionadosPresentacion(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerRecursosRelacionadosPresentacion(pProyectoID);
        }

        /// <summary>
        /// Obtiene los proyectos relacionados de un proyecto que se le pasa como parámetro
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto del que queremos obtener los proyectos</param>
        /// <param name="pDataWrapperProyecto">Dataset de proyectos</param>
        /// <returns>DataSet con los proyectos del proyecto que se pasa por parametros</returns>
        public void ObtenerProyectosRelacionados(Guid pProyectoID, DataWrapperProyecto pDataWrapperProyecto)
        {
            ProyectoAD.ObtenerProyectosRelacionados(pProyectoID, pDataWrapperProyecto);
        }

        /// <summary>
        /// Comprueba si existe el proyecto FAQ
        /// </summary>
        /// <returns>TRUE si existe, FALSE en caso contrario</returns>
        public bool ExisteProyectoFAQ()
        {
            return ProyectoAD.ExisteProyectoFAQ();
        }

        /// <summary>
        /// Comprueba si existe el proyecto Noticias de Gnoss
        /// </summary>
        /// <returns>TRUE si existe, FALSE en caso contrario</returns>
        public bool ExisteProyectoNoticias()
        {
            return ProyectoAD.ExisteProyectoNoticias();
        }

        /// <summary>
        /// Comprueba si existe el proyecto Didactalia de Gnoss
        /// </summary>
        /// <returns>TRUE si existe, FALSE en caso contrario</returns>
        public bool ExisteProyectoDidactalia()
        {
            return ProyectoAD.ExisteProyectoDidactalia();
        }

        /// <summary>
        /// Comprueba si existe un nombre corto de proyecto pasado por parámetro
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del proyecto</param>
        /// <returns>TRUE si existe, FALSE en caso contrario</returns>
        public bool ExisteNombreCortoEnBD(string pNombreCorto)
        {
            return ProyectoAD.ExisteNombreCortoEnBD(pNombreCorto);
        }

        /// <summary>
        /// Comprueba si existe un nombre de proyecto pasado por parámetro
        /// </summary>
        /// <param name="pNombre">Nombre del proyecto</param>
        /// <returns>TRUE si existe, FALSE en caso contrario</returns>
        public bool ExisteNombreEnBD(string pNombre)
        {
            return ProyectoAD.ExisteNombreEnBD(pNombre);
        }

        /// <summary>
        /// Obtiene el nombre corto de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Nombre corto de proyecto</returns>
        public string ObtenerNombreCortoProyecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerNombreCortoProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene los nombres cortos de los proyectos
        /// </summary>
        /// <param name="pProyectosID">Identificador de los proyectos</param>
        /// <returns>Nombres cortos de los proyectos</returns>
        public Dictionary<Guid, string> ObtenerNombresCortosProyectos(List<Guid> pProyectosID)
        {
            return ProyectoAD.ObtenerNombresCortosProyectos(pProyectosID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pOrganizacionID"></param>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public Dictionary<string, Dictionary<Guid, string>> ObtenerNombresCortosProyectosConNombresCortosOntologias(Guid? pProyectoID)
        {
            return ProyectoAD.ObtenerNombresCortosProyectosConNombresCortosOntologias(pProyectoID, "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pOrganizacionID"></param>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public Dictionary<string, Dictionary<Guid, string>> ObtenerNombresCortosProyectosConNombresCortosOntologias(string pNombreCortoProyecto)
        {
            return ProyectoAD.ObtenerNombresCortosProyectosConNombresCortosOntologias(null, pNombreCortoProyecto);
        }

        /// <summary>
        /// 
        /// </summary>

        public string ObtenerNombreOntologiaProyectoUsuario(Guid pPerfil, string pTipo)
        {
            return ProyectoAD.ObtenerNombreOntologiaProyectoUsuario(pPerfil, pTipo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pOrganizacionID"></param>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public Dictionary<string, Guid> ObtenerOntologiasConIDPorNombreCortoProy(string pNombreCortoProyecto)
        {
            return ProyectoAD.ObtenerOntologiasConIDPorNombreCortoProy(pNombreCortoProyecto);
        }

        /// <summary>
        /// Obtiene las ontologías de los proyectos en los que participa un determinado perfil
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <returns>Dataset de proyecto con la tabla OntologiaProyecto cargada para el perfil</returns>
        public List<OntologiaProyecto> ObtenerOntologiasPorPerfilID(Guid pPerfilID)
        {
            return ProyectoAD.ObtenerOntologiasPorPerfilID(pPerfilID);
        }

        /// <summary>
        /// Obtiene el identificador de la organización de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>identificador de la organización del proyecto</returns>
        public Guid ObtenerOrganizacionIDProyecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerOrganizacionIDProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene el id autonumérico que se le asigna a cada proyecto para crear la tabla BASE
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public int ObtenerTablaBaseProyectoIDProyectoPorID(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerTablaBaseProyectoIDProyectoPorID(pProyectoID);
        }

        /// <summary>
        /// Obtiene el id autonumérico que se le asigna a cada proyecto para crear la tabla BASE
        /// </summary>
        /// <param name="pListaProyectosID">Identificadores de los proyectos</param>
        /// <returns></returns>
        public Dictionary<Guid, int> ObtenerTablasBaseProyectoIDProyectoPorID(List<Guid> pListaProyectosID)
        {
            return ProyectoAD.ObtenerTablasBaseProyectoIDProyectoPorID(pListaProyectosID);
        }

        /// <summary>
        /// Obtiene el proyecto a partir del id autonumérico que se le asigna al crear la tabla BASE.
        /// </summary>
        /// <param name="pTablaBaseProyectoID">Identificador de la tabla base del proyecto</param>
        /// <returns>DataSet del proyecto con el proyecto cargado</returns>
        public DataWrapperProyecto ObtenerProyectoPorTablaBaseProyectoID(int pTablaBaseProyectoID)
        {
            DataWrapperProyecto dataWrapper = new DataWrapperProyecto();
            dataWrapper.ListaProyecto = ProyectoAD.ObtenerProyectoPorTablaBaseProyectoID(pTablaBaseProyectoID);
            return dataWrapper;
        }

        /// <summary>
        /// Obtiene el identificador proyecto a partir del id autonumérico que se le asigna al crear la tabla BASE.
        /// </summary>
        /// <param name="pTablaBaseProyectoID">Identificador de la tabla base del proyecto</param>
        /// <returns>Identificador del proyecto con el proyecto cargado</returns>
        public Guid ObtenerProyectoIDPorTablaBaseProyectoID(int pTablaBaseProyectoID)
        {
            return ProyectoAD.ObtenerProyectoIDPorTablaBaseProyectoID(pTablaBaseProyectoID);
        }

        /// <summary>
        /// Obtiene los proyectos con ontologias que administra el usuario
        /// </summary>
        /// <param name="pUsuarioID"></param>
        /// <returns></returns>
        public Dictionary<Guid, string> ObtenerProyectosConOntologiasAdministraUsuario(Guid pUsuarioID)
        {
            return ProyectoAD.ObtenerProyectosConOntologiasAdministraUsuario(pUsuarioID);
        }
        /// <summary>
        /// Obtiene los NombreFiltro de los ProyectosSearchPersonalizados del proyecto
        /// </summary>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <returns>Lista con los NombreFiltro del proyecto</returns>
        public List<ProyectoSearchPersonalizado> ObtenerProyectosSearchPersonalizado(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerProyectosSearchPersonalizado(pProyectoID);
        }
        /// <summary>
        /// Obtiene los valores de la consulta SPARQL asociada al filtro
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="nombreFiltro">Nombre del filtro de la consulta</param>
        /// <returns>Lista con los de la consulta de SPARQL</returns>
        public List<string> ObtenerValoresConsultaSPARQL(Guid pProyectoID, string nombreFiltro)
        {
            return ProyectoAD.ObtenerValoresConsultaSPARQL(pProyectoID, nombreFiltro);
        }
        /// <summary>
        /// Actualiza los valores de la consulta de SPARQL 
        /// </summary>
        /// <param name="organizacionID">Id de la organizacion</param>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <param name="nombreFiltro">Nombre del filtro de las consultas</param>
        /// <param name="listaValores">Lista de los valores de las consultas de SPARQL</param>
        public void ActualizarValoresConsultaSPARQL(Guid organizacionID, Guid pProyectoID, string nombreFiltro, List<string> listaValores)
        {
            ProyectoAD.ActualizarValoresConsultaSPARQL(organizacionID, pProyectoID, nombreFiltro, listaValores);
        }
        /// <summary>
        /// Actualiza los valores de la tabla ProyectoSearchPersonalizado con los nuevos valores de búsqueda personalizada
        /// </summary>
        /// <param name="organizacionID">Id de la organizacion</param>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <param name="listaParametros">Lista de los parámetros de búsqueda personalizados</param>
        public void ActualizarParametrosBusquedaPersonalizados(Guid organizacionID, Guid pProyectoID, List<ParametroBusquedaPersonalizadoModel> listaParametros)
        {
            ProyectoAD.ActualizarParametrosBusquedaPersonalizados(organizacionID, pProyectoID, listaParametros);
        }
        /// <summary>
        /// Obtiene la URL propia de un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Url propia de proyecto</returns>
        public string ObtenerURLPropiaProyecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerURLPropiaProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene el nº de proyectos con el mismo dominio que el proyecto pasado por parámetro
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public int NumeroProyectosConMismosDominio(string pDominio)
        {
            return ProyectoAD.NumeroProyectosConMismosDominio(pDominio);
        }

        /// <summary>
        /// Obtiene listado de Guid de proyectos con el mismo dominio que el proyecto pasado por parámetro
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public List<Guid> ObtenerProyectosIdDeDominio(string pDominio)
        {
            return ProyectoAD.ObtenerProyectosIdDeDominio(pDominio);
        }

        /// <summary>
        /// Obtiene la URL propia de un proyecto cuyo nombre corto se pasa por parámetro
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del proyecto</param>
        /// <returns>URL propia del proyecto</returns>
        public string ObtenerURLPropiaProyectoPorNombreCorto(string pNombreCorto)
        {
            return ProyectoAD.ObtenerURLPropiaProyectoPorNombreCorto(pNombreCorto);
        }

        /// <summary>
        /// Obtiene las URLs propias de proyectos cuyos nombres cortos se pasan por parámetro
        /// </summary>
        /// <param name="pNombresCortos">Nombres cortos de los proyectos</param>
        /// <returns>URLS propias de los proyectos</returns>
        public Dictionary<string, string> ObtenerURLSPropiasProyectosPorNombresCortos(List<string> pNombresCortos)
        {
            return ProyectoAD.ObtenerURLSPropiasProyectosPorNombresCortos(pNombresCortos);
        }
        /// <summary>
        /// Obtiene los proyectos carga ligera "Proyecto" y sus administradores "AdministradorProyecto" 
        /// en los que el usuario participa con el perfil personal
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Dataset de proyecto</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaUsuarioConSuPerfilPersonal(Guid pUsuarioID)
        {
            return ProyectoAD.ObtenerProyectosParticipaUsuarioConSuPerfilPersonal(pUsuarioID);
        }

        /// <summary>
        /// Obtiene si el usuario participa en el proyecto con alguna de sus identidades
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>TRUE si el usuario participa con alguna de sus identidadez</returns>
        public bool ParticipaUsuarioEnProyecto(Guid pProyectoID, Guid pUsuarioID)
        {
            return ProyectoAD.ParticipaUsuarioEnProyecto(pProyectoID, pUsuarioID);
        }

        /// <summary>
        ///Obtiene los proyectos en los que participa la organización pasada por parámetro
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pExcluidMyGNOSS">TRUE si se debe excluir MyGNOSS de la búsqueda, FALSE en caso contrario</param>
        /// <returns>Dataset de proyecto</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaOrganizacion(Guid pOrganizacionID, bool pExcluidMyGNOSS)
        {
            return ProyectoAD.ObtenerProyectosParticipaOrganizacion(pOrganizacionID, pExcluidMyGNOSS);
        }

        /// <summary>
        /// Obtiene los proyectos en los que participa una organización ordenados por relevancia (Número de visitas en GNOSS)
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaOrganizacionPorRelevancia(Guid pOrganizacionID)
        {
            return ProyectoAD.ObtenerProyectosParticipaOrganizacionPorRelevancia(pOrganizacionID);
        }

        /// <summary>
        /// Obtiene una lista con los IDs de los proyectos en los que participa la organizacion
        /// </summary>
        /// <param name="pOrganizacion">Identificador de la organizacion</param>
        /// <param name="pObtenerSoloActivos">Indica si se debe traer los proyectos en los que ya no participa</param>
        /// <returns>Lista con los IDs de los proyectos en los que participa la organizacion</returns>
        public List<Guid> ObtenerListaProyectoIDDeOrganizacion(Guid pOrganizacion, bool pObtenerSoloActivos)
        {
            return ProyectoAD.ObtenerListaProyectoIDDeOrganizacion(pOrganizacion, pObtenerSoloActivos);
        }

        /// <summary>
        /// Obtiene los proyectos en los que participa un usuario ordenados por relevancia (Número de visitas del usuario)
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaPerfilUsuarioPorRelevancia(Guid pPerfilID)
        {
            return ProyectoAD.ObtenerProyectosParticipaPerfilUsuarioPorRelevancia(pPerfilID);
        }

        public DataSet ObtenerDatosProyectosDesplegarAcciones(List<Guid> pListaProyectos)
        {
            return ProyectoAD.ObtenerDatosProyectosDesplegarAcciones(pListaProyectos);
        }



        /// <summary>
        ///Obtiene los proyectos en los que participa la organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Dataset de proyecto</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaOrganizacion(Guid pOrganizacionID)
        {
            return ObtenerProyectosParticipaOrganizacion(pOrganizacionID, false);
        }

        /// <summary>
        /// Obtiene los proyectos (carga ligera de "Proyecto") en los que un usuario de organización 
        /// participa con el perfil de la organización pasada por parámetro
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pUsuarioID">Identificador del usuario de la organización</param>
        /// <returns>Dataset de proyecto</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaUsuarioDeLaOrganizacion(Guid pOrganizacionID, Guid pUsuarioID)
        {
            return ProyectoAD.ObtenerProyectosParticipaUsuarioDeLaOrganizacion(pOrganizacionID, pUsuarioID);
        }

        /// <summary>
        /// Obtiene proyectos a partir de una lista de identificadores
        /// </summary>
        /// <param name="pListaProyectoID">Lista de identificadores de proyecto</param>
        /// <returns>Dataset de proyecto</returns>
        public DataWrapperProyecto ObtenerProyectosPorID(List<Guid> pListaProyectoID)
        {
            return ProyectoAD.ObtenerProyectosPorID(pListaProyectoID);
        }

        /// <summary>
        /// Obtiene un proyecto: "Proyecto", "AdministradorProyecto" , "TagProyecto" , "ProyectoAgCatTesauro ", 
        /// "ProyectoCerradoTmp", "ProyectoCerrandose" a partir de su identificador
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Dataset de proyecto</returns>
        public DataWrapperProyecto ObtenerProyectoPorID(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerProyectoPorID(pProyectoID);
        }

        public DataWrapperProyecto ObtenerProyectoPorIDDS(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerProyectoPorID(pProyectoID);
        }

        /// <summary>
        /// Obtiene los datos Extra de un proyecto (incluidos los del ecosistema) (para los registros de usuarios)
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>DataSet de Proyecto con las tablas DatoExtraProyecto y DatoExtraProyectoOpcion</returns>
        public DataWrapperProyecto ObtenerDatosExtraProyectoPorID(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerDatosExtraProyectoPorID(pProyectoID);
        }

        /// <summary>
        /// Obtiene los datos Extra de un proyecto (incluidos los del ecosistema) (para los registros de usuarios)
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>DataSet de Proyecto con las tablas DatoExtraProyecto y DatoExtraProyectoOpcion</returns>
        public DataWrapperProyecto ObtenerDatosExtraProyectoPorListaIDs(List<Guid> pListaProyectosID)
        {
            return ProyectoAD.ObtenerDatosExtraProyectoPorListaIDs(pListaProyectosID);
        }

        /// <summary>
        /// Obtiene las preferencias disponibles en un proyecto (para los registros de usuarios)
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>DataSet de Proyecto con la tabla PreferenciasProyecto</returns>
        public DataWrapperProyecto ObtenerPreferenciasProyectoPorID(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerPreferenciasProyectoPorID(pProyectoID);
        }

        /// <summary>
        /// Obtiene las acciones externas en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>DataSet de Proyecto con la tabla AccionesExternasProyecto</returns>
        public DataWrapperProyecto ObtenerAccionesExternasProyectoPorProyectoID(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerAccionesExternasProyectoPorProyectoID(pProyectoID);
        }

        /// <summary>
        /// Obtiene las acciones externas en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>DataSet de Proyecto con la tabla AccionesExternasProyecto</returns>
        public DataWrapperProyecto ObtenerAccionesExternasProyectoPorListaIDs(List<Guid> pListaProyectosID)
        {
            return ProyectoAD.ObtenerAccionesExternasProyectoPorListaIDs(pListaProyectosID);
        }

        /// <summary>
        /// Obtiene los eventos disponibles en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>DataSet de Proyecto con la tabla EventoProyecto</returns>
        public DataWrapperProyecto ObtenerEventosProyectoPorProyectoID(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerEventosProyectoPorProyectoID(pProyectoID);
        }

        /// <summary>
        /// Obtiene los eventos disponibles en un proyecto para una identidad
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>DataSet de Proyecto con la tabla EventoProyecto</returns>
        public DataSet ObtenerEventoProyectoIdentidadID(Guid pProyectoID, Guid pIdentidadID)
        {
            return ProyectoAD.ObtenerEventoProyectoIdentidadID(pProyectoID, pIdentidadID);
        }

        /// <summary>
        /// Obtiene el evento con ese ID
        /// </summary>
        /// <param name="pEventoID">Clave del evento</param>
        /// <returns>DataSet de Proyecto con la tabla EventoProyecto</returns>
        public DataWrapperProyecto ObtenerEventoProyectoPorEventoID(Guid pEventoID)
        {
            return ObtenerEventoProyectoPorEventoID(pEventoID, false);
        }

        /// <summary>
        /// Obtiene el evento con ese ID
        /// </summary>
        /// <param name="pEventoID">Clave del evento</param>
        /// <returns>DataSet de Proyecto con la tabla EventoProyecto</returns>
        public DataWrapperProyecto ObtenerEventoProyectoPorEventoID(Guid pEventoID, bool pSoloActivos)
        {
            return ProyectoAD.ObtenerEventoProyectoPorEventoID(pEventoID, pSoloActivos);
        }

        /// <summary>
        /// Obtiene los participantes de un evento con ese ID
        /// </summary>
        /// <param name="pEventoID">Clave del evento</param>
        /// <returns>DataSet de Proyecto con la tabla EventoProyectoParticipante</returns>
        public DataWrapperProyecto ObtenerEventoProyectoParticipantesPorEventoID(Guid pEventoID)
        {
            return ProyectoAD.ObtenerEventoProyectoParticipantesPorEventoID(pEventoID);
        }


        public void BorrarConfiguracionSemanticaExtraDeProyecto(Guid pClave, string pNombreOntologia)
        {
            ProyectoAD.BorrarConfiguracionSemanticaExtraDeProyecto(pClave, pNombreOntologia);
        }

        /// <summary>
        /// Obtiene el numero participantes de eventos de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>Diccionario con clave evento y valor numero de participantes</returns>
        public Dictionary<Guid, int> ObtenerNumeroParticipantesEventosPorProyectoID(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerNumeroParticipantesEventosPorProyectoID(pProyectoID);
        }

        /// <summary>
        /// Obtiene si una identidad participa en un evento
        /// </summary>
        /// <param name="pEventoID">Clave del evento</param>
        /// <param name="pIdentidad">Clave de la identidad</param>
        /// <returns>TRUE si participa en el evento</returns>
        public bool ObtenerSiIdentidadParticipaEnEvento(Guid pEventoID, Guid pIdentidad)
        {
            return ProyectoAD.ObtenerSiIdentidadParticipaEnEvento(pEventoID, pIdentidad);
        }

        /// <summary>
        /// Obtiene (carga ligera) los datos del proyecto pasado como parámetro
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>ProyectoDS.ProyectoRow</returns>
        public Proyecto ObtenerProyectoPorIDCargaLigera(Guid pProyectoID)
        {
            List<Guid> lista = new List<Guid>();
            lista.Add(pProyectoID);

            DataWrapperProyecto dataWrapperProyectoDS = ObtenerProyectosPorIDsCargaLigera(lista);

            if (dataWrapperProyectoDS.ListaProyecto.Count > 0)
            {
                return dataWrapperProyectoDS.ListaProyecto[0];
            }
            return null;
        }

        public DataWrapperProyecto ObtenerProyectoPorIDCargaLigeraDataWrapper(Guid pProyectoID)
        {
            List<Guid> lista = new List<Guid>();
            lista.Add(pProyectoID);

            DataWrapperProyecto dataWrapperProyectoDS = ObtenerProyectosPorIDsCargaLigera(lista);
            return dataWrapperProyectoDS;
        }

        /// <summary>
        /// Obtiene (carga ligera) los datos del proyecto pasado como parámetro
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Dataset de proyecto</returns>
        public DataWrapperProyecto ObtenerProyectoCargaLigeraPorID(Guid pProyectoID)
        {
            List<Guid> lista = new List<Guid>();
            lista.Add(pProyectoID);

            return ProyectoAD.ObtenerProyectosPorIDsCargaLigera(lista);
        }

        /// <summary>
        /// Obtiene proyectos (carga ligera) a partir de la lista de sus identificadores pasada como parámetro
        /// </summary>
        /// <param name="pListaProyectoID">Lista de identificadores de proyecto</param>
        /// <returns>Dataset de proyecto</returns>
        public DataWrapperProyecto ObtenerProyectosPorIDsCargaLigera(List<Guid> pListaProyectoID)
        {
            return ProyectoAD.ObtenerProyectosPorIDsCargaLigera(pListaProyectoID);
        }

        /// <summary>
        /// Obtiene (carga ligera) los datos de los proyectos mas populares a los que no pertenece el usuario
        /// </summary>
        /// <param name="pPersonaID">ID de la persona</param>
        /// <param name="pNumeroProyectos">Número de proyectos</param>
        /// <returns>Dataset de proyecto</returns>
        public DataWrapperProyecto ObtenerProyectosRecomendadosPorPersona(Guid pPersonaID, int pNumeroProyectos)
        {
            DataWrapperProyecto dataWrapperProy = new DataWrapperProyecto();
            dataWrapperProy.ListaProyecto = ProyectoAD.ObtenerProyectosRecomendadosPorPersona(pPersonaID, pNumeroProyectos);
            return dataWrapperProy;
        }



        /// <summary>
        /// Recupera todos los proyectos de una organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de proyecto</returns>
        public DataWrapperProyecto ObtenerProyectoOrganizacion(Guid pOrganizacionID, Guid pProyectoID)
        {
            //ChequeoSeguridad.ComprobarAutorizacion((ulong)Capacidad.General.CapacidadesOrganizacion.VerProyectos);

            return ProyectoAD.ObtenerProyectoPorID(pProyectoID);
        }

        /// <summary>
        /// Comprueba si el usuario es administrador del metaproyecto "MYGNOSS"
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>TRUE si lo es, FALSE en caso contrario</returns>
        public bool EsUsuarioAdministradorProyectoMYGnoss(Guid pUsuarioID)
        {
            return ProyectoAD.EsUsuarioAdministradorProyectoMYGnoss(pUsuarioID);
        }

        /// <summary>
        /// Comprueba si el usuario es administrador del proyecto
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>TRUE si lo es, FALSE en caso contrario</returns>
        public bool EsUsuarioAdministradorProyecto(Guid pUsuarioID, Guid pProyectoID)
        {
            return EsUsuarioAdministradorProyecto(pUsuarioID, pProyectoID, TipoRolUsuario.Administrador);
        }

        /// <summary>
        /// Comprueba si el usuario esta bloqueado en el proyecto proyecto
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>TRUE si lo es, FALSE en caso contrario</returns>
        public bool EstaUsuarioBloqueadoEnProyecto(Guid pUsuarioID, Guid pProyectoID)
        {
            return ProyectoAD.EstaUsuarioBloqueadoEnProyecto(pUsuarioID, pProyectoID);
        }

        /// <summary>
        /// Comprueba si el usuario es administrador del proyecto
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pTipo">Tipo del rol que se quiere comprobar</param>
        /// <returns>TRUE si lo es, FALSE en caso contrario</returns>
        public bool EsUsuarioAdministradorProyecto(Guid pUsuarioID, Guid pProyectoID, TipoRolUsuario pTipo)
        {
            return ProyectoAD.EsUsuarioAdministradorProyecto(pUsuarioID, pProyectoID, pTipo);
        }

        /// <summary>
        /// Comprueba si la identidad es administrador del proyecto
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pTipo">Tipo del rol que se quiere comprobar</param>
        /// <returns>TRUE si lo es, FALSE en caso contrario</returns>
        public bool EsIdentidadAdministradorProyecto(Guid pIdentidadID, Guid pProyectoID, TipoRolUsuario pTipo)
        {
            return ProyectoAD.EsIdentidadAdministradorProyecto(pIdentidadID, pProyectoID, pTipo);
        }

        /// <summary>
        /// Obtiene la lista de proyectos en las que participa un usuario pasado por parámetro
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Dataset de proyecto con todos los proyectos en los que participa el usuario</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaUsuario(Guid pUsuarioID)
        {
            return ObtenerProyectosParticipaUsuario(pUsuarioID, false);
        }

        /// <summary>
        /// Obtiene los proyectos en los que participa un usuario pasado por parámetro
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pTipoDocumentoCompartido">Tipo del recurso que va a compartir el usuario</param>
        /// <returns>Dataset de proyecto con todos los proyectos en los que participa el usuario</returns>
        public DataWrapperProyecto ObtenerProyectosUsuarioPuedeCompartirRecurso(Guid pUsuarioID, TiposDocumentacion pTipoDocumentoCompartido)
        {
            return ProyectoAD.ObtenerProyectosUsuarioPuedeCompartirRecurso(pUsuarioID, pTipoDocumentoCompartido);
        }

        /// <summary>
        /// Obtiene la lista de proyectos en las que participa un usuario en modo personal o profesional personal
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Dataset de proyecto con todos los proyectos en los que participa el usuario</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaUsuarioEnModoPersonal(Guid pUsuarioID)
        {
            return ProyectoAD.ObtenerProyectosParticipaUsuarioEnModoPersonal(pUsuarioID);
        }

        /// <summary>
        /// Obtiene los proyectos en los que participa un usuario pasado por parámetro
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pSoloUsuariosSinBloquear">TRUE si deben traerse sólo los usuarios sin bloquear, FALSE si son todos</param>
        /// <returns>Dataset de proyecto con todos los proyectos en los que participa el usuario</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaUsuario(Guid pUsuarioID, bool pSoloUsuariosSinBloquear)
        {
            //ChequeoSeguridad.ComprobarAutorizacion((ulong)Capacidad.General.CapacidadesOrganizacion.VerProyectos);

            return ProyectoAD.ObtenerProyectosParticipaUsuario(pUsuarioID, pSoloUsuariosSinBloquear);
        }

        /// <summary>
        /// Obtiene la lista de proyectos en las que participa un usuario pasado por parámetro
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <returns>Dataset de proyecto con todos los proyectos en los que participa el usuario</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaUsuarioConPerfil(Guid pUsuarioID, Guid pPerfilID)
        {
            return ProyectoAD.ObtenerProyectosParticipaUsuarioConPerfil(pUsuarioID, pPerfilID);
        }

        /// <summary>
        /// Obtiene una lista con los proyectos en los que participa un perfil pasándole una identidad(NO incluye myGnoss)
        /// </summary>
        /// <param name="listaProyectos">Lista con los proyectos obtenidos de Virtuoso</param>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <returns>Lista de los proyectos en los que participa un perfil</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaPerfilIdentidad(string listaProyectos, Guid pIdentidadID)
        {
            return ProyectoAD.ObtenerProyectosParticipaPerfilIdentidad(listaProyectos, pIdentidadID);
        }

        /// <summary>
        /// Obtiene la lista de proyectos en las que participa un usuario pasado por parámetro.
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Diccionario ProyectoID,NombreCorto de cada proyecto</returns>
        public Dictionary<Guid, string> ObtenerNombresCortosProyectosParticipaUsuarioSinBloquearNiAbandonar(Guid pUsuarioID)
        {
            return ProyectoAD.ObtenerNombresCortosProyectosParticipaUsuarioSinBloquearNiAbandonar(pUsuarioID);
        }

        /// <summary>
        /// Obtiene los proyectos administrados por el perfil pasado por parámetro
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Diccionario ProyectoID,NombreCorto de cada proyecto</returns>
        public Dictionary<Guid, string> ObtenerNombresCortosProyectosAdministraUsuarioSinBloquearNiAbandonar(Guid pUsuarioID, bool ecosistema = false)
        {
            return ProyectoAD.ObtenerNombresCortosProyectosAdministraUsuarioSinBloquearNiAbandonar(pUsuarioID, ecosistema);
        }

        /// <summary>
        /// Obtiene el UsuarioID de todos los miembros de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Lista de UsuarioID</returns>
        public List<Guid> ObtenerUsuarioIDMiembrosProyecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerUsuarioIDMiembrosProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene la lista de proyectos en las que participa un usuario pasado por parámetro.
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pTipoDoc">Tipo de documento que se va a cargar</param>
        /// <returns>Diccionario ProyectoID,NombreCorto de cada proyecto</returns>
        public Dictionary<Guid, string> ObtenerNombresCortosProyectosParticipaUsuarioSinBloquearNiAbandonarYConfigurables(Guid pUsuarioID)
        {
            return ProyectoAD.ObtenerNombresCortosProyectosParticipaUsuarioSinBloquearNiAbandonarYConfigurables(pUsuarioID);
        }

        /// <summary>
        /// Obtiene los registros de AdministradorProyecto de un usuario pasado como parámetro
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Dataset de proyecto</returns>
        public DataWrapperProyecto ObtenerAdministradorProyectoDeUsuario(Guid pUsuarioID)
        {
            return ProyectoAD.ObtenerAdministradorProyectoDeUsuario(pUsuarioID);
        }

        /// <summary>
        /// Obtiene los registros de AdministradorProyecto de una persona pasado como parámetro
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>Dataset de proyecto</returns>
        public DataWrapperProyecto ObtenerAdministradorProyectoDePersona(Guid pPersonaID)
        {
            return ProyectoAD.ObtenerAdministradorProyectoDePersona(pPersonaID);
        }

        /// <summary>
        /// Obtiene los datos de la tabla AdministradorProyecto de un proyecto dado
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de proyecto con la tabla AdministradorProyecto cargada</returns>
        public DataWrapperProyecto ObtenerAdministradorProyectoDeProyecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerAdministradorProyectoDeProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene el UsuarioID y PerfilID de los administradores de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Diccionario con perejas de UsuarioID, PerfilID</returns>
        public Dictionary<Guid, Guid> ObtenerUsuarioIDPerfilIDAdministradoresDeProyecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerUsuarioIDPerfilIDAdministradoresDeProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene los proyectos hijos de los proyectos que se pasan por parámetro
        /// </summary>
        /// <param name="pListaProyectosID">Lista de identificadores de proyecto</param>
        /// <returns>Dataset de proyecto con los proyectos hijos cargados</returns>
        public DataWrapperProyecto ObtenerProyectosHijosDeProyectos(List<Guid> pListaProyectosID, Guid pUsuarioID)
        {
            return ProyectoAD.ObtenerProyectosHijosDeProyectos(pListaProyectosID, pUsuarioID);
        }

        /// <summary>
        /// Obtiene los proyectos padres de los proyectos que se pasan por parametros
        /// </summary>
        /// <param name="pListaProyectosID">Lista de identificadores de los proyectos</param>
        /// <returns>Dataset de proyecto con los proyectos padres cargados</returns>
        public DataWrapperProyecto ObtenerProyectosPadresDeProyectos(List<Guid> pListaProyectosID)
        {
            return ProyectoAD.ObtenerProyectosPadresDeProyectos(pListaProyectosID);
        }

        /// <summary>
        /// Obtiene el id del proyecto padre del proyecto pasado por parámetro
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto del cual se quiere obtener el padre</param>
        /// <returns>Id del proyecto padre</returns>
        public Guid ObtenerProyectoPadreIDDeProyecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerProyectoPadreIDDeProyecto(pProyectoID);
        }


        /// <summary>
        /// Obtiene el proyecto en el que participa una identidad (no muestra los que estén cerrados)
        /// </summary>
        /// <param name="pIdentidad">Identificador de identidad</param>
        /// <returns>Dataset de proyecto con el proyecto en el que participa la identidad</returns>
        public DataWrapperProyecto ObtenerProyectoParticipaIdentidad(Guid pIdentidad)
        {
            return ProyectoAD.ObtenerProyectoParticipaIdentidad(pIdentidad);
        }

        /// <summary>
        /// Obtiene los proyectos en los que participa un perfil
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <param name="pExcluirMyGNOSS">TRUE si se quiere excluir la metacomunidad, FALSE en caso contrario</param>
        /// <returns>Dataset de proyecto con los proyectos en los que participa un perfil</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaPerfilUsuario(Guid pPerfilID, bool pExcluirMyGNOSS, Guid pUsuarioID)
        {
            return ProyectoAD.ObtenerProyectosParticipaPerfilUsuario(pPerfilID, -1, pExcluirMyGNOSS, pUsuarioID);
        }

        /// <summary>
        /// Obtiene los proyectos en los que participa un perfil
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <param name="pTipoIdentidad">Tipo de identidad del perfil</param>
        /// <param name="pExcluirMyGNOSS">TRUE si se quiere excluir la metacomunidad, FALSE en caso contrario</param>
        /// <returns>Dataset de proyecto con los proyectos en los que participa un perfil</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaPerfilUsuarioConTipoIdentidad(Guid pPerfilID, short pTipoIdentidad, bool pExcluirMyGNOSS, Guid pUsuarioID)
        {
            return ProyectoAD.ObtenerProyectosParticipaPerfilUsuario(pPerfilID, pTipoIdentidad, pExcluirMyGNOSS, pUsuarioID);
        }

        /// <summary>
        /// Obtiene una lista con los proyectos en los que participa un perfil(NO incluye myGnoss)
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <returns>Lista de los proyectos en los que participa un perfil</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaPerfil(Guid pPerfilID)
        {
            return ProyectoAD.ObtenerProyectosParticipaPerfil(pPerfilID);
        }

        /// <summary>
        /// Obtiene una lista con los proyectos que no son de registro obligatorio.
        /// </summary>
        /// <returns>Lista de los proyectos que no son de registro obligatorio</returns>
        public List<Guid> ObtenerListaIDsProyectosSinRegistroObligatorio()
        {
            return ProyectoAD.ObtenerListaIDsProyectosSinRegistroObligatorio();
        }

        /// <summary>
        /// Obtiene una lista con los proyectos en los que participa un perfil(NO incluye myGnoss)
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <returns>Lista de los proyectos en los que participa un perfil</returns>
        public Dictionary<string, KeyValuePair<string, short>> ObtenerListaProyectosParticipaPerfilUsuario(Guid pPerfilID)
        {
            return ProyectoAD.ObtenerListaProyectosParticipaPerfilUsuario(pPerfilID);
        }

        /// <summary>
        /// Obtiene una lista con los identificadores de los proyectos en los que participa el usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador del Usuario</param>
        /// <returns>Lista con los identificadores de los proyectos en los que participa el Usuario</returns>
        public List<Guid> ObtenerListaIDsProyectosParticipaUsuario(Guid pUsuarioID)
        {
            return ProyectoAD.ObtenerListaIDsProyectosParticipaUsuario(pUsuarioID);
        }

        /// <summary>
        /// Obtiene una lista con los proyectos en los que participa el usuarioID
        /// </summary>
        /// <param name="pUsuarioID">Identificado del Usuario</param>
        /// <returns>Lista de los proyectos en los que participa el Usuario</returns>
        public Dictionary<string, KeyValuePair<string, short>> ObtenerListaProyectosParticipaUsuario(Guid pUsuarioID)
        {
            return ProyectoAD.ObtenerListaProyectosParticipaUsuario(pUsuarioID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pPerfilID"></param>
        /// <returns></returns>
        public Guid ObtenerProyectoIDMasActivoPerfil(Guid pPerfilID)
        {
            return ProyectoAD.ObtenerProyectoIDMasActivoPerfil(pPerfilID);
        }

        /// <summary>
        /// Obtiene una lista con los proyectos comunes en los que participan dos perfiles
        /// </summary>
        /// <param name="pPerfilID1">Identificador del perfil 1</param>
        /// <param name="pTipoIdentidad1">Tipo de identidad del perfil 1</param>
        /// <param name="pPerfilID2">Identificador del perfil 2</param>
        /// <param name="pTipoIdentidad2">Tipo de identidad del perfil 2</param>
        /// <param name="pIncluirMyGNOSS">Indica si se debe de buscar my gnoss</param>
        /// <returns>DataSet de Proyectos</returns>
        public DataWrapperProyecto ObtenerListaProyectosComunesParticipanPerfilesUsuarios(Guid pPerfilID1, TiposIdentidad pTipoIdentidad1, Guid pPerfilID2, TiposIdentidad pTipoIdentidad2, bool pIncluirMyGNOSS)
        {
            return ProyectoAD.ObtenerListaProyectosComunesParticipanPerfilesUsuarios(pPerfilID1, pTipoIdentidad1, pPerfilID2, pTipoIdentidad2, pIncluirMyGNOSS);
        }

        /// <summary>
        /// Actualiza los cambios realizados en proyectos
        /// </summary>
        /// <param name="pDataWrapperProyecto">Dataset de proyectos</param>
        public void ActualizarProyectos()
        {
            ActualizarProyectos(false);
        }

        /// <summary>
        /// Actualiza los cambios realizados en los proyectos más activos. Primero vacia la tabla "ProyectosMasActivosw" y luego actualiza.
        /// </summary>
        /// <param name="pDataWrapperProyecto">Dataset de proyectos</param>
        public void ActualizarProyectosMasActivos(DataWrapperProyecto pDataWrapperProyecto)
        {
            ActualizarProyectos(true);
        }


        /// <summary>
        /// Actualiza los cambios realizados en proyectos
        /// </summary>
        /// <param name="pProyectoDS">Dataset de proyectos</param>
        /// <param name="pRecalculandoProyectosMasActivos">TRUE si se deben recalcular los proyectos más activos</param>
        public void ActualizarProyectos(bool pRecalculandoProyectosMasActivos)
        {
            try
            {
                bool transaccionIniciada = ProyectoAD.IniciarTransaccionEntityContext();

                mEntityContext.SaveChanges();

                if (transaccionIniciada)
                {
                    TerminarTransaccion(true);
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
            catch (SqlException ex)
            {
                TerminarTransaccion(false);
                //MessageBox.Show("No se puede eliminar el proyecto ya que existen elementos vinculados a él. (" + e.Message + ")", "Error en la eliminacion del proyecto", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //Error interno de la aplicación
                throw ex;
            }
            catch
            {
                TerminarTransaccion(false);
                throw;
            }


            //try
            //{
            //    EntityContext.SaveChanges();
            //}
            //catch (DbEntityValidationException e)
            //{
            //    foreach (var eve in e.EntityValidationErrors)
            //    {

            //        foreach (var ve in eve.ValidationErrors)
            //        {

            //        }
            //    }
            //}
            //List<Proyecto> cambiosProyectos;

            //cambiosProyectos =
            //    (List<Proyecto>)pProyectoDS.Proyecto.Select(
            //    null, null,
            //    DataViewRowState.Added |
            //    DataViewRowState.ModifiedCurrent);

            //this.ValidarProyectos(cambiosProyectos);

            //try
            //{
            //    if (Transaccion != null)
            //    {
            //        if (pRecalculandoProyectosMasActivos)
            //        {
            //            ProyectoAD.ActualizarProyectosMasActivos(pProyectoDS);
            //        }
            //        else
            //        {
            //            ProyectoAD.ActualizarProyectos(pProyectoDS, false);
            //        }
            //    }
            //    else
            //    {
            //        IniciarTransaccion();
            //        {
            //            if (pRecalculandoProyectosMasActivos)
            //            {
            //                ProyectoAD.ActualizarProyectosMasActivos(pProyectoDS);
            //            }
            //            else
            //            {
            //                ProyectoAD.ActualizarProyectos(pProyectoDS, false);
            //            }

            //            if (pProyectoDS != null)
            //            {
            //                pProyectoDS.AcceptChanges();
            //            }
            //            TerminarTransaccion(true);
            //        }
            //    }
            //    
            //}
            //catch (DBConcurrencyException ex)
            //{
            //    TerminarTransaccion(false);
            //    // Error de concurrencia
            //    Error.GuardarLogError(ex);
            //    throw new ErrorConcurrencia();
            //}
            //catch (DataException ex)
            //{
            //    TerminarTransaccion(false);
            //    //Error interno de la aplicación	
            //    Error.GuardarLogError(ex);			
            //    throw new ErrorInterno();
            //}
            //catch (SqlException ex)
            //{
            //    TerminarTransaccion(false);
            //    //MessageBox.Show("No se puede eliminar el proyecto ya que existen elementos vinculados a él. (" + e.Message + ")", "Error en la eliminacion del proyecto", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    //Error interno de la aplicación
            //    throw ex;
            //}
            //catch
            //{
            //    TerminarTransaccion(false);
            //    throw;
            //}
        }

        /// <summary>
        /// Carga la presentación de todos los documentos semántico en una comunidad
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>    
        public DataWrapperProyecto ObtenerPresentacionSemantico(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerPresentacionSemantico(pProyectoID);
        }

        /// <summary>
        /// Carga la presentación de todos los documentos semántico en una comunidad
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>    
        public DataWrapperProyecto ObtenerPresentacionListadoSemantico(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerPresentacionListadoSemantico(pProyectoID);
        }

        /// <summary>
        /// Carga la presentación de todos los documentos semántico en una comunidad
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>    
        public DataWrapperProyecto ObtenerPresentacionMosaicoSemantico(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerPresentacionMosaicoSemantico(pProyectoID);
        }

        /// <summary>
        /// Guarda los datos de un dataset de proyecto pasado como parámetro
        /// </summary>
        /// <param name="pProyecto">Dataset de proyecto</param>
        public void GuardarProyectos()
        {
            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Elimina los datos de un dataset de proyecto pasado como parámetro
        /// </summary>
        /// <param name="pProyectoDS">Dataset de proyecto</param>
        public void EliminarProyectos(/*ProyectoDS pProyectoDS*/)
        {
            mEntityContext.SaveChanges();
        }

        ///// <summary>
        ///// Obtiene el peso de todos los  proyectos. Para que el servivio de optimizacion calcule "ProyectosMasActivos"
        ///// </summary>
        ///// <returns>Dataset de proyecto</returns>
        //public ProyectoDS ObtenerProyectosPorPeso()
        //{
        //    return ProyectoAD.ObtenerProyectosPorPeso();
        //}

        /// <summary>
        /// Obtiene la configuracion del login de la comunidad
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet con la tabla 'ProyectoLoginConfiguracion'</returns>
        public DataWrapperProyecto ObtenerProyectoLoginConfiguracion(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerProyectoLoginConfiguracion(pProyectoID);
        }

        /// <summary>
        /// Calcula el peso del proyecto pasado como parámetro
        /// </summary>
        /// <param name="pProyectoID">ProyectoID del que se quiere obtener el peso</param>
        /// <returns>Peso del proyecto</returns>
        public int ObtenerPesoPorProyecto(Guid pProyectoID)
        {
            //Calcular el número de proyectos a partir de los siguientes patrones
            //	10 x Nº Artículos (En los últimos numDias días)
            //	15 x Nº Dafos (En los últimos numDias días)
            //	5 x Nº Miembros (En los últimos numDias días)
            //	5 x Nº Organizaciones (En los últimos numDias días)

            int numRecursos = ProyectoAD.ObtenerNumRecursosProyecto30Dias(pProyectoID, 30);

            return (numRecursos * 10);
        }

        /// <summary>
        /// Obtiene el número de recursos publicados en el proyecto
        /// </summary>
        /// <param name="pProyectoID">ProyectoID del que se quiere obtener el peso.</param>
        /// <returns>Número de recursos publicados</returns>
        public int ObtenerNumRecursosProyecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerNumRecursosProyecto30Dias(pProyectoID, -1);
        }

        /// <summary>
        /// Obtiene el número de dafos publicados en el proyecto
        /// </summary>
        /// <param name="pProyectoID">ProyectoID del que se quiere obtener el peso.</param>
        /// <returns>Número de dafos publicados</returns>
        //public int ObtenerNumDebatesProyecto(Guid pProyectoID)
        //{
        //    return ProyectoAD.ObtenerNumDebatesProyecto(pProyectoID);
        //}

        /// <summary>
        /// Obtiene el número de dafos publicados en el proyecto
        /// </summary>
        /// <param name="pProyectoID">ProyectoID del que se quiere obtener el peso.</param>
        /// <returns>Número de dafos publicados</returns>
        //public int ObtenerNumeroPreguntas(Guid pProyectoID)
        //{
        //    return ProyectoAD.ObtenerNumeroPreguntas(pProyectoID);
        //}





        /// <summary>
        /// Actualiza los contadores del proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto a actualizar</param>
        /// <param name="pNumOrg">Número de miembros de organización</param>
        /// <param name="pNumIden">Número de miembros normales</param>
        /// <param name="pNumRec">Número de recursos</param>
        /// <param name="pNumDafos">Número de dafos</param>
        /// <param name="pNumDebates">Número Debates</param>
        /// <param name="pNumPreg">Número Preguntas</param>
        public void ActualizarContadoresProyecto(Guid pProyectoID, int pNumOrg, int pNumIden, int pNumRec, int pNumDafos, int pNumDebates, int pNumPreg)
        {
            ProyectoAD.ActualizarContadoresProyecto(pProyectoID, pNumOrg, pNumIden, pNumRec, pNumDafos, pNumDebates, pNumPreg);
        }


        /// <summary>
        /// Calcula el rol permitido para un usuario en un proyecto, teniendo en cuenta los roles de los grupos a los que pertenece
        /// </summary>
        /// <param name="pUsuario">Usuario para calcular su rol</param>
        /// <param name="pProyecto">Proyecto para calcular el rol</param>
        /// <param name="pComprobarAutenticacionUsuario">TRUE si debe comprobar que el usuario está autenticado, FALSE en caso contrario</param>
        /// <returns>Rol permitido final del usuario en el proyecto</returns>
        public ulong CalcularRolFinalUsuarioEnProyecto(AD.EntityModel.Models.UsuarioDS.Usuario pUsuario, Proyecto pProyecto, bool pComprobarAutenticacionUsuario)
        {
            return CalcularRolFinalUsuarioEnProyecto(pUsuario.UsuarioID, pUsuario.Login, pProyecto.OrganizacionID, pProyecto.ProyectoID);
        }

        /// <summary>
        /// Calcula el rol permitido para un usuario en un proyecto, teniendo en cuenta los roles de los grupos a los que pertenece
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pLogin">Login del usuario (NULL si no se quiere comprobar que el usuario esté autenticado)</param>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pComprobarAutenticacionUsuario">TRUE si debe comprobar que el usuario está autenticado, FALSE en caso contrario</param>
        /// <returns>Rol permitido final del usuario en el proyecto</returns>
        public ulong CalcularRolFinalUsuarioEnProyecto(Guid pUsuarioID, string pLogin, Guid pOrganizacionID, Guid pProyectoID)
        {
            //Declaramos roles por defecto 
            ulong rolPermitidoUsuario = 0;
            ulong rolDenegadoUsuario = 0;

            //1º Obtenemos los roles del usuario en el proyecto
            UsuarioCN serviciosRolUsuario = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            AD.EntityModel.Models.UsuarioDS.ProyectoRolUsuario proyectoRolUsuario = serviciosRolUsuario.ObtenerRolUsuarioEnProyecto(pProyectoID, pUsuarioID);

            if (proyectoRolUsuario == null)
            {
                throw new ErrorElementoNoExiste();
            }

            if (proyectoRolUsuario.RolPermitido != null)
            {
                rolPermitidoUsuario = Convert.ToUInt64(proyectoRolUsuario.RolPermitido, 16);
            }

            if (proyectoRolUsuario.RolDenegado != null)
            {
                rolDenegadoUsuario = Convert.ToUInt64(proyectoRolUsuario.RolDenegado, 16);
            }

            //2º Calculamos el rol final
            ulong rolPermitidoFinal = rolPermitidoUsuario;
            ulong rolDenegadoFinal = rolDenegadoUsuario;

            return rolPermitidoFinal & ~rolDenegadoFinal;
        }

        /// <summary>
        /// Obtiene un lista con los nombres de determinados proyectos.
        /// </summary>
        /// <param name="pListaProyectos">Lista con los identificadores de los proyectos a nombrar</param>
        /// <returns>Lista con los nombres de determinados proyectos</returns>
        public Dictionary<Guid, Proyecto> ObtenerNombreProyectos(List<Guid> pListaProyectos)
        {
            return ProyectoAD.ObtenerNombreProyectos(pListaProyectos);
        }

        /// <summary>
        /// Obtiene el la URL del API de Integracion Continua
        /// </summary>
        /// <returns>Identificador del proyecto</returns>
        public string ObtenerURLApiIntegracionContinua()
        {
            return ProyectoAD.ObtenerURLApiIntegracionContinua();
        }


        /// <summary>
        /// Obtiene el identificador del Entorno.
        /// </summary>
        /// <returns>Identificador del Entorno</returns>
        public Guid ObtenerEntornoID()
        {
            return ProyectoAD.ObtenerEntornoID();
        }

        /// <summary>
        /// Obtiene el Identificador de un proyecto a partir de su nombre CORTO
        /// </summary>
        /// <param name="pNombre">Nombre CORTO del proyecto buscado</param>
        /// <returns>Identificador del proyecto</returns>
        public Guid ObtenerProyectoIDPorNombre(string pNombre)
        {
            return ProyectoAD.ObtenerProyectoIDPorNombre(pNombre);
        }


        /// <summary>
        /// Obtiene el proyecto a través de su nombre corto
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del proyecto a obtener</param>
        /// <returns></returns>
        public Proyecto ObtenerProyectoPorNombreCorto(string pNombreCorto)
        {
            return ProyectoAD.ObtenerProyectoPorNombreCorto(pNombreCorto);
        }

        /// <summary>
        /// Nos indica si existe algún proyecto con el nombre corto indicado
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto a comprobar</param>
        /// <returns></returns>
        public bool ExisteNombreCortoProyecto(string pNombreCorto)
        {
            return ProyectoAD.ExisteNombreCortoProyecto(pNombreCorto);
        }

        /// <summary>
        /// Obtiene el Identificador de un proyecto a partir de su nombre CORTO
        /// </summary>
        /// <param name="pNombreCorto">Nombre CORTO del proyecto buscado</param>
        /// <returns>Identificador del proyecto</returns>
        public List<Guid> ObtenerProyectoYProyectoSuperiorIDs(string pNombreCorto)
        {
            return ProyectoAD.ObtenerProyectoYProyectoSuperiorIDs(pNombreCorto);
        }

        /// <summary>
        /// Obtiene el identificador de un proyecto a partir de su nombre CORTO pasado por parámetro
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del proyecto</param>
        /// <returns>Identificador del proyecto</returns>
        public List<Guid> ObtenerProyectoIDOrganizacionIDPorNombreCorto(string pNombreCorto)
        {
            return ProyectoAD.ObtenerProyectoIDOrganizacionIDPorNombreCorto(pNombreCorto);
        }

        /// <summary>
        /// Obtiene el Identificador de un proyecto a partir de su nombre corto
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del proyecto</param>
        /// <returns>Identificador del proyecto</returns>
        public Guid ObtenerProyectoIDPorNombreCorto(string pNombreCorto)
        {
            return ProyectoAD.ObtenerProyectoIDPorNombre(pNombreCorto);
        }

        /// <summary>
        /// Obtiene el Identificador de un proyecto a partir de su nombre
        /// </summary>
        /// <param name="pNombre">Nombre del proyecto buscado</param>
        /// <returns>Identificador del proyecto</returns>
        public Guid ObtenerProyectoIDPorNombreLargo(string pNombre)
        {
            return ProyectoAD.ObtenerProyectoIDPorNombreLargo(pNombre);
        }

        /// <summary>
        /// Obtiene nombre completo de un determinado proyecto (pProyectoID)
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto buscado</param>
        /// <returns>Nombre del proyecto</returns>
        public string ObtenerNombreDeProyectoID(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerNombreDeProyectoID(pProyectoID);
        }

        /// <summary>
        /// Obtiene el proyecto superiorID de un determinado proyecto (pProyectoID)
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto buscado</param>
        /// <returns>Nombre del proyecto</returns>
        public Guid ObtenerProyectoSuperiorIDDeProyectoID(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerProyectoSuperiorIDDeProyectoID(pProyectoID);
        }


        /// <summary>
        /// Obtiene el proyecto cuyo identificador se pasa por parámetro, además de sus niveles de certificación 
        /// y los permisos de los roles de usuario sobre los tipos de recursos del proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerProyectoPorIDConNiveles(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerProyectoPorIDConNiveles(pProyectoID);
        }

        /// <summary>
        /// Obtiene el proyecto cuyo identificador se pasa por parámetro
        /// TFG Fran
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerProyectoDashboardPorID(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerProyectoDashboardPorID(pProyectoID);
        }


        /// <summary>
        /// Obtiene los nombres de los proyectos administrados por el usuario pasado por parámetro con el perfil dado
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerNombresProyectosAdministradosPorUsuarioID(Guid pUsuarioID, Guid pPerfilID)
        {

            return ProyectoAD.ObtenerNombresProyectosAdministradosPorUsuarioID(pUsuarioID, pPerfilID);
        }

        /// <summary>
        /// Obtiene los nombres de los proyectos administrados por el usuario pasado por parámetro con el perfil dado
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <returns>Dataset de proyectos</returns>
        public Dictionary<Guid, string> ObtenerNombresProyectosPrivadosAdministradosPorUsuario(Guid pUsuarioID, Guid pPerfilID)
        {
            return ProyectoAD.ObtenerNombresProyectosPrivadosAdministradosPorUsuario(pUsuarioID, pPerfilID);
        }

        /// <summary>
        /// Obtiene las secciones de la home de un proyecto tipo catálogo
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        //public ProyectoDS ObtenerSeccionesHomeCatalogoDeProyecto(Guid pProyectoID)
        public DataWrapperProyecto ObtenerSeccionesHomeCatalogoDeProyecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerSeccionesHomeCatalogoDeProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene los proyectos administrados por el perfil pasado por parámetro
        /// </summary>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerProyectosAdministradosPorPerfilID(Guid pPerfilID)
        {
            return ProyectoAD.ObtenerProyectosAdministradosPorPerfilID(pPerfilID);
        }

        /// <summary>
        /// Indica si el usuario ers el único administrador de un proyecto
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>True si es el único administrador de algun proyecto</returns>
        public bool EsUsuarioAdministradorUnicoDeProyecto(Guid pUsuarioID)
        {
            return ProyectoAD.EsUsuarioAdministradorUnicoDeProyecto(pUsuarioID);
        }

        /// <summary>
        /// Indica si el usuario es el único administrador de un proyecto concreto
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>True si es el único administrador del proyecto</returns>
        public bool EsUsuarioAdministradorUnicoDeProyecto(Guid pUsuarioID, Guid pProyectoID)
        {
            return ProyectoAD.EsUsuarioAdministradorUnicoDeProyecto(pUsuarioID, pProyectoID);
        }

        /// <summary>
        /// Obtiene los proyectos administrados por la organizacion pasado por parámetro
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerProyectosAdministradosPorOrganizacionID(Guid pOrganizacionID)
        {
            return ProyectoAD.ObtenerProyectosAdministradosPorOrganizacionID(pOrganizacionID);
        }

        /// <summary>
        /// Comprueba si en el proyecto existen usuarios que no sean los administradores del mismo
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>TRUE si existen usuarios, FALSE en caso contrario</returns>
        public bool TieneUsuariosExceptoLosAdministradores(Guid pProyectoID)
        {
            return ProyectoAD.TieneUsuariosExceptoLosAdministradores(pProyectoID);
        }

        /// <summary>
        /// Comprueba si existe alguna categoría de tesauro en el proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>TRUE si existen categorías del tesauro, FALSE en caso contrario</returns>
        public bool TienecategoriasDeTesauro(Guid pProyectoID)
        {
            return ProyectoAD.TienecategoriasDeTesauro(pProyectoID);
        }

        /// <summary>
        /// Comprueba si algún usuario de la organización (personas con usuario vinculadas con la organización) 
        /// es administrador del proyecto pasado por parámetro
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>TRUE si se encuentra algun usuario de la organizacion que sea administrador del proyecto</returns>
        public bool EsAlguienDeLAOrganizacionAdministradorProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return ProyectoAD.EsAlguienDeLAOrganizacionAdministradorProyecto(pOrganizacionID, pProyectoID);
        }


        /// <summary>
        /// Carga todos los proyectos que se estén cerrando --> ProyectoDS (Proyecto,ProyectoCerrandose) 
        /// </summary>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerProyectosCerrandose()
        {
            return ProyectoAD.ObtenerProyectosCerrandose();
        }

        /// <summary>
        /// Valida el formato del nombre corto del proyecto
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto</param>
        /// <returns>TRUE si el nombre corto del proyecto es correcto, FALSE en caso contrario</returns>
        public static bool ValidarFormatoNombreCortoProyecto(string pNombreCorto)
        {
            if (pNombreCorto.Contains(" "))
            {
                return false;
            }
            Regex expresionRegular = new Regex(@"(^([a-zA-Z0-9-ñÑ]{4,30})$)");

            if (!expresionRegular.IsMatch(pNombreCorto))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Devuelve si el nombre corto ya esta en la BD para otra organizacion o no
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto de la comunidad</param>
        /// <returns>TRUE si es valido, FALSE si ya se encuentra en la BD</returns>
        public bool ValidarNombreCortoProyecto(string pNombreCorto)
        {
            return ProyectoAD.ExisteNombreCortoEnBD(pNombreCorto);
        }

        /// <summary>
        /// Obtiene los grupos que tienen permisos sobre una ontología en un determinado proyecto
        /// </summary>
        /// <param name="pListaOntologiasID">Lista de identificadores de ontología</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que se hace la comprobación</param>
        /// <returns>Diccionario con los grupos de comunidad y organización que tienen permiso sobre la ontología</returns>
        public Dictionary<Guid, List<Guid>> ObtenerGruposPermitidosOntologiasEnProyecto(List<Guid> pListaOntologiasID, Guid pProyectoID)
        {
            return ProyectoAD.ObtenerGruposPermitidosOntologiasEnProyecto(pListaOntologiasID, pProyectoID);
        }

        /// <summary>
        /// Obtiene las ontologías permitidas para un rol de usuario en un determinado proyecto
        /// </summary>
        /// <param name="pIdentidadEnProyID">Identificador de la identidad del usuario en el proyecto</param>
        /// <param name="pIdentidadEnMyGnossID">Identificador de la identidad del usuario en mygnoss</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que se hace la comprobación</param>
        /// <param name="pTipoRol">Tipo de rol del usuario actual</param>
        /// <param name="pIdentidadDeOtroProyecto">Verdad si la identidad pertenece a otro proyecto distinto a pProyectoID</param>
        /// <returns>Lista con las ontologías permitidas para la identidad</returns>
        public List<Guid> ObtenerOntologiasPermitidasIdentidadEnProyecto(Guid pIdentidadEnProyID, Guid pIdentidadEnMyGnossID, Guid pProyectoID, TipoRolUsuario pTipoRol, bool pIdentidadDeOtroProyecto, Dictionary<Guid, Guid> pOntologiasEcosistema = null)
        {
            return ProyectoAD.ObtenerOntologiasPermitidasIdentidadEnProyecto(pIdentidadEnProyID, pIdentidadEnMyGnossID, pProyectoID, pTipoRol, pIdentidadDeOtroProyecto, pOntologiasEcosistema);
        }

        /// <summary>
        /// Obtiene las ontologías permitidas para un rol de usuario en un determinado proyecto
        /// </summary>
        /// <param name="pIdentidadEnProyID">Identificador de la identidad del usuario en el proyecto</param>
        /// <param name="pIdentidadEnMyGnossID">Identificador de la identidad del usuario en mygnoss</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que se hace la comprobación</param>
        /// <param name="pTipoRol">Tipo de rol del usuario actual</param>
        /// <param name="pIdentidadDeOtroProyecto">Verdad si la identidad pertenece a otro proyecto distinto a pProyectoID</param>
        /// <param name="pDocumentoID">Identificador del documento que representa la ontología</param>
        /// <param name="pOntologiasEcosistema">Ontologías del ecosistema (puede ser null)</param>
        /// <returns>Lista con las ontologías permitidas para la identidad</returns>
        public bool ComprobarOntologiasPermitidaParaIdentidadEnProyecto(Guid pIdentidadEnProyID, Guid pIdentidadEnMyGnossID, Guid pProyectoID, TipoRolUsuario pTipoRol, bool pIdentidadDeOtroProyecto, Guid pDocumentoID, Dictionary<Guid, Guid> pOntologiasEcosistema = null)
        {
            List<Guid> listaResultados = ProyectoAD.ObtenerOntologiasPermitidasIdentidadEnProyecto(pIdentidadEnProyID, pIdentidadEnMyGnossID, pProyectoID, pTipoRol, pIdentidadDeOtroProyecto, pOntologiasEcosistema, pDocumentoID);

            return (listaResultados != null && listaResultados.Contains(pDocumentoID));
        }

        /// <summary>
        /// Obtiene las ontologías del ecosistema
        /// </summary>
        /// <returns>Lista con los DocumentoID de todas las ontologías con su ProyectoID como valor del diccionario</returns>
        public Dictionary<Guid, Guid> ObtenerOntologiasEcosistema()
        {
            return ProyectoAD.ObtenerOntologiasEcosistema();
        }

        ///// <summary>
        ///// Obtiene las ontologías permitidas para un rol de usuario en un determinado proyecto
        ///// </summary>
        ///// <param name="pProyectoID">ID de proyecto</param>
        ///// <returns>DataSet con las ontologías permitidas para un rol de usuario en un determinado proyecto cargadas</returns>
        //public ProyectoDS ObtenerOntologiasDisponiblesProyecto(Guid pProyectoID)
        //{
        //    return ProyectoAD.ObtenerOntologiasDisponiblesProyecto(pProyectoID);
        //}

        /// <summary>
        /// Devuelve una lista con las claves de los proyectos que tienen alguna categoria del tesauro de MyGnoss en comun con el pProyectoID
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto a buscar sus relacionados</param>
        /// <returns>Lista de Claves de Proyectos</returns>
        public List<Guid> ObtenerListaProyectoRelacionados(Guid pProyectoID)
        {
            bool pManual = true;
            return ProyectoAD.ObtenerListaProyectoRelacionados(pProyectoID, out pManual);
        }

        /// <summary>
        /// Devuelve una lista con las claves de los proyectos que tienen alguna categoria del tesauro de MyGnoss en comun con el pProyectoID
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto a buscar sus relacionados</param>
        /// <param name="pManual">Indica si los proyectos son manuales o automaticos</param>
        /// <returns>Lista de Claves de Proyectos</returns>
        public List<Guid> ObtenerListaProyectoRelacionados(Guid pProyectoID, out bool pManual)
        {
            return ProyectoAD.ObtenerListaProyectoRelacionados(pProyectoID, out pManual);
        }

        /// <summary>
        /// Devuelve una lista con las claves de los proyectos que administra el grupo
        /// </summary>
        /// <param name="pUsuarioID">Clave del grupo</param>
        /// <returns>Lista de Claves de Proyectos</returns>
        public List<Guid> ObtenerListaProyectosAdministradosPorGrupo(Guid pGrupoID)
        {
            return ProyectoAD.ObtenerListaProyectosAdministradosPorGrupo(pGrupoID);
        }

        /// <summary>
        /// Devuelve una lista con las claves de los proyectos que administra el usuario
        /// </summary>
        /// <param name="pUsuarioID">Clave del usuario</param>
        /// <param name="pPerfilID">Clave del perfil</param>
        /// <returns>Lista de Claves de Proyectos</returns>
        public List<Guid> ObtenerListaProyectosAdministraUsuarioYPerfil(Guid pUsuarioID, Guid pPerfilID)
        {
            return ProyectoAD.ObtenerListaProyectosAdministraUsuarioYPerfil(pUsuarioID, pPerfilID);
        }

        /// <summary>
        /// Obtiene el identificador del metaproyecto
        /// </summary>
        /// <returns>Identificador del metaproyecto</returns>
        public Guid ObtenerMetaProyectoID()
        {
            return ProyectoAD.ObtenerMetaProyectoID();
        }

        /// <summary>
        /// Obtiene el identificador de la metaorganización
        /// </summary>
        /// <returns>Identificador de la metaorganización</returns>
        public Guid ObtenerMetaOrganizacionID()
        {
            return ProyectoAD.ObtenerMetaOrganizacionID();
        }

        /// <summary>
        /// Obtiene un listado con todas las urls propias de los proyectos
        /// </summary>
        /// <returns>Lista de strings</returns>
        public List<string> ObtenerUrlPropiasProyectos()
        {
            return ProyectoAD.ObtenerUrlPropiasProyectos();
        }

        /// <summary>
        /// Obtiene nombre completo de un determinado proyecto (pProyectoID)
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto buscado</param>
        /// <returns>Nombre del proyecto</returns>
        public Dictionary<Guid, string> ObtenerNombreDeProyectosID(List<Guid> pListaProyectosID)
        {
            return ProyectoAD.ObtenerNombreDeProyectosID(pListaProyectosID);
        }

        /// <summary>
        /// Obtiene un listado con todas las urls propias de los proyectos publicos
        /// </summary>
        /// <returns>Lista de strings</returns>
        public List<string> ObtenerUrlPropiasProyectosPublicos()
        {
            return ProyectoAD.ObtenerUrlPropiasProyectosPublicos();
        }

        /// <summary>
        /// Obtiene los contadores de reucursos, personas y organizaciones de una comunidad.
        /// </summary>
        /// <param name="pProyectoID">ID de la comunidad</param>
        /// <returns>DataSet con Proyecto con los contadores de reucursos, personas y organizaciones de una comunidad</returns>
        public DataWrapperProyecto ObtenerContadoresProyecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerContadoresProyecto(pProyectoID);
        }

        /// <summary>
        /// Devuelve el proyectoID a partir de la base de recursos de un proyecto
        /// </summary>
        /// <param name="pBaseRecursosID"></param>
        /// <returns>Si encuentra el proyectoID sino Guid.empty</returns>
        public Guid ObtenerProyectoIDPorBaseRecursos(Guid pBaseRecursosID)
        {
            return ProyectoAD.ObtenerProyectoIDPorBaseRecursos(pBaseRecursosID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void ObtenerDatosPorBaseRecursosPersona(Guid pBaseRecursosID, Guid pPersonaID, out Guid pProyectoID, out Guid pIdentidadID, out Guid pOrganizacionID)
        {
            ProyectoAD.ObtenerDatosPorBaseRecursosPersona(pBaseRecursosID, pPersonaID, out pProyectoID, out pIdentidadID, out pOrganizacionID);
        }

        /// <summary>
        /// Devuelve la Base de Recursos de un ProyectoID
        /// </summary>
        /// <param name="pProyectoID">Proyecto del que queremos la Base de recursos</param>
        /// <returns>Base de recursos ID</returns>
        public Guid ObtenerBaseRecursosProyectoPorProyectoID(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerBaseRecursosProyectoPorProyectoID(pProyectoID);
        }

        /// <summary>
        /// Obtiene los grafos gráficos configurados en un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <returns>DataSet la tabla 'ProyectoGrafoFichaRec' con los grafos gráficos configurados en un proyecto</returns>
        public DataWrapperProyecto ObtenerGrafosProyecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerGrafosProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene los nombres cortos de todas las comunidades de los tipos especificados
        /// </summary>
        /// <param name="pListaTipos">Lista de tipos de comunidades</param>
        /// <returns>Nombres cortos</returns>
        public List<string> ObtenerNombresCortosProyectosPorTipo(List<TipoProyecto> pListaTipos)
        {
            return ProyectoAD.ObtenerNombresCortosProyectosPorTipo(pListaTipos);
        }

        /// <summary>
        /// Obtiene un listado con los proyectos que tienen configuracion de newasletter por defecto (Guid.Empty especifica que es confgiracion del ecosistema)
        /// </summary>
        /// <returns></returns>
        public Dictionary<Guid, bool> ObtenerProyectosConConfiguracionNewsletterPorDefecto()
        {
            return ProyectoAD.ObtenerProyectosConConfiguracionNewsletterPorDefecto();
        }

        #region Documentación

        /// <summary>
        /// Actualiza el número de recursos de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void ActualizarNumeroDocumentacion(Guid pProyectoID)
        {
            ProyectoAD.ActulizarNumeroDocumentacion(pProyectoID);
        }

        /// <summary>
        /// Obtiene los tipo de documentos permitidos para un rol de usuario en un determinado proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto en el que se hace la comprobación</param>
        /// <param name="pTipoRol">Tipo de rol del usuario actual</param>
        /// <returns>Lista con los tipos de documentos permitidos para el rol</returns>
        public List<TiposDocumentacion> ObtenerTiposDocumentosPermitidosUsuarioEnProyecto(Guid pProyectoID, TipoRolUsuario pTipoRol)
        {
            return ProyectoAD.ObtenerTiposDocumentosPermitidosUsuarioEnProyecto(pProyectoID, pTipoRol);
        }

        /// <summary>
        /// Obtiene el tipo de proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public TipoProyecto ObtenerTipoProyecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerTipoProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene los proyectos en los que participa una organizacion
        /// </summary>
        /// <param name="pOrganizacionID">Identificador del proyecto</param>
        /// <returns></returns>
        public DataWrapperProyecto CargarProyectosDeOrganizacionCargaLigeraParaFiltros(Guid pOrganizacionID)
        {
            return ProyectoAD.CargarProyectosDeOrganizacionCargaLigeraParaFiltros(pOrganizacionID);
        }


        /// <summary>
        /// Obtiene los proyectos en los que participa una persona de una organizacion
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns></returns>
        public List<UsuarioAdministradorComunidad> CargarProyectosParticipaPersonaOrg(Guid pOrganizacionID, Guid pPersonaID)
        {
            return ProyectoAD.CargarProyectosParticipaPersonaOrg(pOrganizacionID, pPersonaID);
        }

        /// <summary>
        /// Obtiene el rol de usuario en un determinado proyecto.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto en el que se hace la comprobación</param>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>El rol de usuario en un determinado proyecto</returns>
        public TipoRolUsuario ObtenerRolUsuarioEnProyecto(Guid pProyectoID, Guid pUsuarioID)
        {
            return ProyectoAD.ObtenerRolUsuarioEnProyecto(pProyectoID, pUsuarioID);
        }

        /// <summary>
        /// Obtiene los filtros de ordenes disponibles para un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet de ProyectoDS con la tabla 'ProyectoPestanyaFiltroOrdenRecursos' cargada para un proyecto</returns>
        public DataWrapperProyecto ObtenerFiltrosOrdenesDeProyecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerFiltrosOrdenesDeProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene los tesauros semánticos configurados para edición.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet de ProyectoDS con la tabla 'ProyectoConfigExtraSem' cargada para un proyecto</returns>
        public DataWrapperProyecto ObtenerTesaurosSemanticosConfigEdicionDeProyecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerTesaurosSemanticosConfigEdicionDeProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene los tesauros semánticos configurados para edición.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet de ProyectoDS con la tabla 'ProyectoConfigExtraSem' cargada para un proyecto</returns>
        public DataWrapperProyecto ObtenerConfiguracionSemanticaExtraDeProyecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerConfiguracionSemanticaExtraDeProyecto(pProyectoID);
        }

        /// <summary>
        /// Indica si el recurso es de un Tipo de recursos que se encuentra en la lista de recursos que no se publican en la actividad reciente
        /// </summary>
        /// <param name="pRecursoID">ID del recurso</param>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns></returns>
        public bool ComprobarSiRecursoSePublicaEnActividadReciente(Guid pRecursoID, Guid pProyectoID)
        {
            return ProyectoAD.ComprobarSiRecursoSePublicaEnActividadReciente(pRecursoID, pProyectoID);
        }

        /// <summary>
        /// Obtiene los tipos de recursos que no deben ir a la actividad reciente de la comunidad.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet con la tabla ProyTipoRecNoActivReciente cargada para el proyecto</returns>
        public DataWrapperProyecto ObtenerTiposRecursosNoActividadReciente(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerTiposRecursosNoActividadReciente(pProyectoID);
        }

        /// <summary>
        /// Obtiene el tipo de acceso a un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public TipoAcceso ObtenerTipoAccesoProyecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerTipoAccesoProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene el estado de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public EstadoProyecto ObtenerEstadoProyecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerEstadoProyecto(pProyectoID);
        }

        /// <summary>
        /// Devuelve las imágenes por defecto según el tipo de imagen por defecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Imagen por defecto según el tipo de imagen por defecto</returns>
        public Dictionary<short, Dictionary<Guid, string>> ObtenerTipoDocImagenPorDefecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerTipoDocImagenPorDefecto(pProyectoID);
        }

        #endregion

        #region Datos Twitter de proyecto

        /// <summary>
        /// Actualiza los tokens para Twitter del proyecto pasado por parámetro
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pTokenTwitter">Token para Twitter</param>
        /// <param name="pTokenSecretoTwitter">Token secreto para Twitter</param>
        public void ActualizarTokenTwitterProyecto(Guid pProyectoID, string pTokenTwitter, string pTokenSecretoTwitter)
        {
            ProyectoAD.ActualizarTokenTwitterProyecto(pProyectoID, pTokenTwitter, pTokenSecretoTwitter);
        }

        #endregion

        #region Administración proyecto

        /// <summary>
        /// Obtiene una lista con los administradores de un proyecto (Sólo los administradores --> Tipo = 0 )
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Lista con las identidades de los administradores del proyecto</returns>
        public List<Guid> ObtenerListaIdentidadesAdministradoresPorProyecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerListaIdentidadesAdministradoresPorProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene una lista con los supervisores de un proyecto (Sólo los supervisores --> Tipo = 1 )
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Lista con las identidades de los supervisores del proyecto</returns>
        public List<Guid> ObtenerListaIdentidadesSupervisoresPorProyecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerListaIdentidadesSupervisoresPorProyecto(pProyectoID);
        }

        #endregion

        #region Administración del tesauro

        /// <summary>
        /// Obtiene todos los documentos que están vinculados a un serie de categorias.
        /// </summary>
        /// <param name="pListaCategorias">Lista con las categorias a las que están agregados los documentos</param>
        /// <param name="pTesauroID">Identificador del tesauro al que pertenecen las categorías</param>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerVinculacionProyectosDeCategoriasTesauro(List<Guid> pListaCategorias, Guid pTesauroID)
        {
            return ProyectoAD.ObtenerVinculacionProyectosDeCategoriasTesauro(pListaCategorias, pTesauroID);
        }

        #endregion

        #region Proyectos hijos

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public List<Guid> ObtenerProyectosHijosDeProyecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerProyectosHijosDeProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene el ID del proyecto origen del actual, si lo tiene o GUID.Empty si no.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>ID del proyecto origen del actual, si lo tiene o GUID.Empty si no</returns>
        public Guid ObtenerProyectoOrigenDeProyecto(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerProyectoOrigenDeProyecto(pProyectoID);
        }

        #endregion

        /// <summary>
        /// Carga una lista con el identificador de todos los proyectos abiertos
        /// </summary>
        /// <returns>ProyectoDS</returns>
        public List<Guid> ObtenerTodosIDProyectosAbiertos()
        {
            return ProyectoAD.ObtenerTodosIDProyectosAbiertos();
        }
        /// <summary>
        /// Obtiene los proyectos en los que participa el usuario
        /// </summary>
        /// <param name="pUsuarioID">Id del usuario</param>
        /// <returns>Devuelve lista con los Id de los proyectos que participa el usuario</returns>
        public List<Guid> ObtenerProyectoIdParticipaUsuario(Guid pUsuarioID)
        {
            return ProyectoAD.ObtenerProyectoIdParticipaUsuario(pUsuarioID);
        }

        /// <summary>
        /// Obtiene un número específico de proyectos en los que participa el usuario
        /// </summary>
        /// <param name="pUsuarioID">Id del usuario</param>
        /// <param name="numeroResultados">Numero de proyectos que se van a devolver</param>
        /// <returns>Devuelve lista con los Id de los proyectos que participa el usuario</returns>
        public List<Guid> ObtenerProyectosIDParticipaUsuario(Guid pUsuarioID, int numeroResultados)
        {
            return ProyectoAD.ObtenerProyectosIDParticipaUsuario(pUsuarioID, numeroResultados);
        }

        /// <summary>
        /// Devuvle los usuarios que no pertenecen al proyecto
        /// </summary>
        /// <param name="listaUsuarios">Lista de los usuarios de la organizacion</param>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <returns>Lista de los usuarios que no pertenecen a la organizacion</returns>
        public List<Guid> ObtenerUsuariosNoParticipanEnComunidad(List<Guid> listaUsuarios, Guid pProyectoID)
        {
            return ProyectoAD.ObtenerUsuariosNoParticipanEnComunidad(listaUsuarios, pProyectoID);
        }
        /// <summary>
        /// Obtiene las urls de la caja de busqueda
        /// </summary>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <returns>Lista con las urls de la caja de busqueda</returns>
        public List<string> ObtenerUrlsComunidadCajaBusqueda(Guid pProyectoID)
        {
            return ProyectoAD.ObtenerUrlsComunidadCajaBusqueda(pProyectoID);
        }
        /// <summary>
        /// Elimina la comunidad de la url de búsqueda
        /// </summary>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <param name="url">Url a la que se le quiere quitar la comunidad</param>
        public void QuitarUrlComunidadCajaBusqueda(Guid pProyectoID, string url)
        {
            ProyectoAD.QuitarUrlComunidadCajaBusqueda(pProyectoID, url);
        }
        /// <summary>
        /// Obtiene la URL de un servicio externo de un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pNombre">Nomrbe del servicio</param>
        /// <returns></returns>
        public string ObtenerUrlServicioExterno(Guid pProyectoID, string pNombre)
        {
            return ProyectoAD.ObtenerUrlServicioExterno(pProyectoID, pNombre);
        }

        public string ObtenerIdiomaPrincipalDominio(string pDominio)
        {
            return ProyectoAD.ObtenerIdiomaPrincipalDominio(pDominio);//select proyectoid, URLPropia from Proyecto where URLPropia like '%pruebasiphoneen.gnoss.net@%'
        }

        public int ObtenerNumeroDeProyectos()
        {
            return ProyectoAD.ObtenerNumeroDeProyectos();
        }

        #endregion

        #region Privados

        /// <summary>
        /// Valida la lista de proyectos pasada como parámetro
        /// </summary>
        /// <param name="pProyectos">Lista de proyectos</param>
        private void ValidarProyectos(List<Proyecto> pProyectos)
        {
            for (int i = 0; i < pProyectos.Count; i++)
            {
                //Nombre de formato superior a 1000 caracteres
                if (pProyectos[i].Nombre.Length > 1000)
                {
                    throw new ErrorDatoNoValido("El nombre del proyecto '" + pProyectos[i].Nombre + "' no puede contener más de 1000 caracteres");
                }

                //Nombre cadena vacía
                if (pProyectos[i].Nombre.Trim().Length == 0)
                {
                    throw new ErrorDatoNoValido("El nombre del proyecto '" + pProyectos[i].Nombre + "' no puede ser una cadena vacía");
                }

                //Descripción no válida
                if (!string.IsNullOrEmpty(pProyectos[i].Descripcion))
                {
                    //Si es vacía la ponemos a Null
                    if (pProyectos[i].Descripcion.Trim().Length == 0)
                    {
                        pProyectos[i].Descripcion = null;
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
        private bool mDisposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~ProyectoCN()
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
        /// <param name="pDisposing">Determina si se está llamando desde el Dispose()</param>
        protected virtual void Dispose(bool pDisposing)
        {
            if (!mDisposed)
            {
                mDisposed = true;

                if (pDisposing)
                {
                    //Libero todos los recursos administrados que he añadido a esta clase
                    if (this.ProyectoAD != null)
                    {
                        ProyectoAD.Dispose();
                    }
                }
                ProyectoAD = null;
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// DataAdapter de proyecto
        /// </summary>
        public ProyectoAD ProyectoAD
        {
            get
            {
                return (ProyectoAD)AD;
            }
            set
            {
                AD = value;
            }
        }

        #endregion
    }
}
