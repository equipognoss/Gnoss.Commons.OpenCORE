using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using System.Text;
using static Es.Riam.Gnoss.Web.MVC.Models.Tesauro.TesauroModels;
using Es.Riam.Semantica.Plantillas;
using Microsoft.Extensions.Logging;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Serilog.Core;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;

namespace Es.Riam.Gnoss.Logica.Facetado
{
    public class FacetadoCN : BaseCN, IDisposable
    {

        #region Miembros

        /// <summary>
        /// idGrafo;
        /// </summary>
        private string mIdGrafo;

        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        public FacetadoCN(string pUrlIntragnoss, string pIdGrafo, EntityContext entityContext, LoggingService loggingService, ConfigService configService, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<FacetadoCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication,loggerFactory.CreateLogger<BaseCN>(),loggerFactory)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.FacetadoAD = new FacetadoAD(pUrlIntragnoss, loggingService, entityContext, configService, virtuosoAD, servicesUtilVirtuosoAndReplication,mLoggerFactory.CreateLogger<FacetadoAD>(),mLoggerFactory);
            this.mIdGrafo = pIdGrafo;
        }

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        public FacetadoCN(string pUrlIntragnoss, EntityContext entityContext, LoggingService loggingService, ConfigService configService, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<FacetadoCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, loggerFactory.CreateLogger<BaseCN>(), loggerFactory)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.FacetadoAD = new FacetadoAD(pUrlIntragnoss, loggingService, entityContext, configService, virtuosoAD, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoAD>(), mLoggerFactory);
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración
        /// </summary>
        /// <param name="pTipoBD">Tipo de BD de configuración de base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        public FacetadoCN(string pTipoBD, string pUrlIntragnoss, string pIdGrafo, EntityContext entityContext, LoggingService loggingService, ConfigService configService, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<FacetadoCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, loggerFactory.CreateLogger<BaseCN>(), loggerFactory)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.FacetadoAD = new FacetadoAD(pTipoBD, pUrlIntragnoss, loggingService, entityContext, configService, virtuosoAD, servicesUtilVirtuosoAndReplication,mLoggerFactory.CreateLogger<FacetadoAD>(),mLoggerFactory);
            this.mIdGrafo = pIdGrafo;
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración
        /// </summary>
        /// <param name="pTipoBD">Tipo de BD de configuración de base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        ///<param name="pTablaReplica">Tabla donde se va a insertar la consulta ("ColaReplicacionMaster" o "ColaReplicacionMasterHome")</param>
        public FacetadoCN(string pTipoBD, string pUrlIntragnoss, string pIdGrafo, string pTablaReplica, EntityContext entityContext, LoggingService loggingService, ConfigService configService, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<FacetadoCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, loggerFactory.CreateLogger<BaseCN>(), loggerFactory)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.FacetadoAD = new FacetadoAD(pTipoBD, pUrlIntragnoss, pTablaReplica, loggingService, entityContext, configService, virtuosoAD, servicesUtilVirtuosoAndReplication,mLoggerFactory.CreateLogger<FacetadoAD>(),mLoggerFactory);
            this.mIdGrafo = pIdGrafo;
        }

        public static bool Replicacion
        {
            get { return FacetadoAD.Replicacion; }
            set { FacetadoAD.Replicacion = value; }
        }
        /// <summary>
        /// Constructor para el usuario que tiene permiso de ver todas las personas
        /// </summary>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        /// <param name="pObtenerPrivados">Verdad si el usuario actual puede obtener los privados</param>
        public FacetadoCN(string pUrlIntragnoss, bool pObtenerPrivados, EntityContext entityContext, LoggingService loggingService, ConfigService configService, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<FacetadoCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.FacetadoAD = new FacetadoAD(pUrlIntragnoss, pObtenerPrivados, loggingService, entityContext, configService, virtuosoAD, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoAD>(), mLoggerFactory);
        }

        /// <summary>
        /// Constructor para el usuario que tiene permiso de ver todas las personas
        /// </summary>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        /// <param name="pObtenerPrivados">Verdad si el usuario actual puede obtener los privados</param>
        public FacetadoCN(string pUrlIntragnoss, bool pObtenerPrivados, string pIdGrafo, EntityContext entityContext, LoggingService loggingService, ConfigService configService, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<FacetadoCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.FacetadoAD = new FacetadoAD(pUrlIntragnoss, pObtenerPrivados, loggingService, entityContext, configService, virtuosoAD, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoAD>(), mLoggerFactory);
            this.mIdGrafo = pIdGrafo;
        }


        /// <summary>
        /// Obtiene la información de lugares, personas y organizaciones de dbpedia
        /// </summary>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        public void ObtieneLugaresPersonasOrganizacionesDBPedia(FacetadoDS pFacetadoDS)
        {
            FacetadoAD.ObtieneLugaresPersonasOrganizacionesDBPedia(pFacetadoDS);
        }


        /// <summary>
        /// Obtiene la información de organizaciones de dbpedia
        /// </summary>
        /// <param name="Tag">Lista con los Tags</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        public void ObtieneOrganizacionesDBPedia(FacetadoDS pFacetadoDS)
        {
            FacetadoAD.ObtieneOrganizacionesDBPedia(pFacetadoDS);
        }

        /// <summary>
        /// Obtiene la información de personas  de dbpedia
        /// </summary>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        public void ObtienePersonasDBPedia(FacetadoDS pFacetadoDS)
        {
            FacetadoAD.ObtienePersonasDBPedia(pFacetadoDS);
        }

        /// <summary>
        /// Obtiene la información de lugares de dbpedia
        /// </summary>        
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        public void ObtieneLugaresDBPedia(FacetadoDS pFacetadoDS)
        {
            FacetadoAD.ObtieneLugaresDBPedia(pFacetadoDS);
        }
        #endregion

        #region Metodos generales

        /// <summary>
        /// Obtiene los recursos de una comunidad que cumplan una determinada regla de compartición y unas reglas de mapeo
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto destino</param>
        /// <param name="pRegla">Regla de compartición</param>
        /// <param name="pListaCategoriasMapping">Lista de identificadores de categorias</param>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns></returns>
        public bool CumpleReglaRecurso(Guid pProyectoID, string pRegla, string pReglaMapping, Guid pDocumentoID)
        {
            return FacetadoAD.CumpleReglaRecurso(pProyectoID, pRegla, pReglaMapping, pDocumentoID);
        }

        /// <summary>
        /// Inserta  en virtuoso las tripletas necesarias para un nuevo contacto
        /// </summary>
        /// <param name="pIdentidad1">pIdentidad1</param>
        /// <param name="pIdentidad2">pIdentidad2</param>
        public void InsertarNuevoContacto(string pIdentidad1, string pIdentidad2)
        {
            FacetadoAD.InsertarNuevoContacto(pIdentidad1, pIdentidad2);
        }

        /// <summary>
        ///Borra en virtuoso las tripletas necesarias cuando se elimina contacto
        /// </summary>
        /// <param name="pIdentidad1">Identidad1</param>
        /// <param name="pIdentidad2">Identidad2</param>
        public void BorrarContacto(Guid pIdentidad1, Guid pIdentidad2)
        {
            FacetadoAD.BorrarContacto(pIdentidad1.ToString(), pIdentidad2.ToString());
        }

        /// <summary>
        ///Borra en virtuoso las tripletas necesarias cuando se elimina un grupo
        /// </summary>
        /// <param name="identidadId">identidadId</param>
        /// <param name="grupoID">grupoID</param>
        /// <param name="pnombreGrupo">nombreGrupo</param>
        public void BorrarGrupoContactos(Guid identidadId, Guid grupoID, string pnombreGrupo)
        {
            FacetadoAD.BorrarGrupoContactos(identidadId.ToString(), grupoID.ToString(), pnombreGrupo);
        }

        /// <summary>
        /// Inserta  en virtuoso las tripletas necesarias para un nuevo seguidor
        /// </summary>
        /// <param name="perfil1">Perfil1 Perfil que sigue</param>
        /// <param name="perfil1">Perfil2 Perfil Seguido</param>
        public void InsertarNuevoSeguidor(string perfil1, string perfil2, string proyecto)
        {
            FacetadoAD.InsertarNuevoSeguidor(perfil1, perfil2, proyecto);
        }

        /// <summary>
        ///Borra en virtuoso las tripletas necesarias cuando se elimina contacto
        /// </summary>
        /// <param name="perfil1">Perfil1 Perfil que sigue</param>
        /// <param name="perfil1">Perfil2 Perfil Seguido</param>
        public void BorrarSeguidor(string perfil1, string perfil2, string proyecto)
        {
            FacetadoAD.BorrarSeguidor(perfil1, perfil2, proyecto);
        }


        /// <summary>
        /// Inserta  en virtuoso las tripletas necesarias para un nuevo seguidor
        /// </summary>
        /// <param name="pIdentidad1">Identidad1</param>
        /// <param name="pIdentidad2">Identidad2</param>
        public void InsertarNuevoSeguidorProyecto(string pIdentidad1, string pIdentidad2, string proyecto)
        {
            FacetadoAD.InsertarNuevoSeguidorProyecto(pIdentidad1, pIdentidad2, proyecto);
        }

        /// <summary>
        /// Inserta  en virtuoso las tripletas necesarias para un nuevo seguidor
        /// </summary>
        /// <param name="pIdentidad1">Identidad1</param>
        /// <param name="pIdentidad2">Identidad2</param>
        public void BorrarSeguidorProyecto(string pIdentidad1, string pIdentidad2, string proyecto)
        {
            FacetadoAD.BorrarSeguidorProyecto(pIdentidad1, pIdentidad2, proyecto);
        }

        /// <summary>
        ///Borra en virtuoso las tripletas necesarias cuando se elimina un grupo
        /// </summary>
        /// <param name="grupoID">ID del grupo</param>
        /// <param name="nombre">Nombre</param>
        /// <param name="proyectoID">ID del proyecto</param>
        public void BorrarGrupo(Guid grupoID, string nombre, Guid proyectoID)
        {
            FacetadoAD.BorrarGrupo(grupoID, nombre, proyectoID);
        }

        /// <summary>
        ///Borra en virtuoso las tripletas necesarias cuando se elimina un grupo
        /// </summary>
        /// <param name="grupoID">ID del grupo</param>
        /// <param name="nombreIdentidad">Identiddad del participante</param>
        /// <param name="nombre">Nombre</param>
        /// <param name="proyectoID">ID del proyecto</param>
        public void BorrarParticipanteDeGrupo(Guid grupoID, Guid identidadID, string nombreIdentidad, Guid proyectoID)
        {
            FacetadoAD.BorrarParticipanteDeGrupo(grupoID, identidadID, nombreIdentidad, proyectoID);
        }

        public void CerrarConexion()
        {
            FacetadoAD.CerrarConexion();
        }

        public void BorrarPopularidad(string pProyectoID, List<string> pTipoItem)
        {
            FacetadoAD.BorrarPopularidad(pProyectoID, pTipoItem);
        }

        /// <summary>
        /// Borra recomendaciones de personas
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pTipoItem">Tipo de item del cual se desea borrar la popularidad: recurso, identidad, blog, etc.</param>
        public void BorrarRecomendacionesDePersonas(string pIdentidad)
        {
            FacetadoAD.BorrarRecomendacionesDePersonas(pIdentidad);
        }
        /// <summary>
        /// Modifica nombre de una categoria
        /// </summary>

        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pNombreNuevo">Nombre nuevo de la categoria</param>
        /// <param name="pCategoriaID">Identificador de la categoria</param>
        /// <param name="UrlIntragnoss"></param>
        public void ModificarNombreCategoria(string pProyectoID, string pNombreNuevo, string pCategoriaID, bool pCategoriaDeProyecto)
        {
            FacetadoAD.ModificarNombreCategoria(pProyectoID, pNombreNuevo, pCategoriaID, pCategoriaDeProyecto);
        }

        public void ModificarPopularidadIdentidad(string pIDIdentidad)
        {
            FacetadoAD.ModificarPopularidadIdentidad(pIDIdentidad);
        }

        /// <summary>
        /// Llamada con un parámetro adiccional que será la condición en el where
        /// </summary>
        /// <param name="pProyectoID">ProyectoID del grafo a modificar</param>
        /// <param name="pElementoAModificarID">Consulta a modificiar</param>
        public void InsertarPopularidad(Guid pProyectoID, string pElementoAModificarID)
        {
            InsertarPopularidad(pProyectoID, pElementoAModificarID, null);
        }

        /// <summary>
        /// Llamada con un parámetro adiccional que será la condición en el where
        /// </summary>
        /// <param name="pProyectoID">ProyectoID del grafo a modificar</param>
        /// <param name="pElementoAModificarID">Consulta a modificiar</param>
        /// <param name="pProyectoID_2">ProyectoID para el where.</param>
        public void InsertarPopularidad(Guid pProyectoID, string pElementoAModificarID, Guid? pProyectoID_2)
        {
            FacetadoAD.InsertarPopularidad(pProyectoID, pElementoAModificarID, pProyectoID_2);
        }

        ///// <summary>
        ///// Modifica la participación de un usuario de corporativo a personal
        ///// </summary>
        ///// <param name="pProyectoID">Identificador del proyecto</param>
        ///// <param name="pIdentidadID">Identificadro de la identidad</param>
        ///// <returns>Triples con la forma: Dictionary<Nombre,Dictionary<Predicado,List<idrecurso>>></returns>
        //public Dictionary<string, Dictionary<string, List<Guid>>> ModificarParticipaciondeCooperativoaPersonal(Guid pProyectoID, Guid pIdentidadID)
        //{
        //    return FacetadoAD.ModificarParticipaciondeCooperativoaPersonal(pProyectoID, pIdentidadID);
        //}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pListaTipos"></param>
        /// <returns></returns>
        public Dictionary<string, int> ObtenerNumeroRecursosPorListaTipos(Guid pProyectoID, List<string> pListaTipos)
        {
            return FacetadoAD.ObtenerNumeroRecursosPorListaTipos(pProyectoID, pListaTipos);
        }

        /// <summary>
        /// Obtiene las comunidades que le pueden interesar a un perfil
        /// </summary>     
        /// <param name="pIdentidadMyGnoss">Identidad en MyGnoss del perfil</param>     
        public DataSet ComunidadesQueTePuedanInteresar(Guid pIdentidadMyGnoss, int pInicio, int pLimite, bool pResultados, Dictionary<string, List<string>> pListaFiltros)
        {
            return FacetadoAD.ComunidadesQueTePuedanInteresar(pIdentidadMyGnoss, pInicio, pLimite, pResultados, pListaFiltros);
        }

        /// <summary>
        /// Obtiene el número de comunidades que le pueden interesar a un perfil
        /// </summary>     
        /// <param name="pIdentidadMyGnoss">Identidad en MyGnoss del perfil</param>     
        public DataSet NumeroComunidadesQueTePuedanInteresar(Guid pIdentidadMyGnoss, Dictionary<string, List<string>> pListaFiltros)
        {
            return FacetadoAD.NumeroComunidadesQueTePuedanInteresar(pIdentidadMyGnoss, pListaFiltros);
        }

        public DataSet ObtenerContactos(Guid pPerfilID1, Guid pPerfilID2, Guid pProyectoID, int pInicio, int pLimite)
        {
            return FacetadoAD.ObtenerContactos(pPerfilID1, pPerfilID2, pProyectoID, pInicio, pLimite);
        }


        public DataSet ObtenerSeguidoresComunidad(Guid pPerfilID1, Guid pPerfilID2, Guid pProyectoID, int pInicio, int pLimite)
        {
            return FacetadoAD.ObtenerSeguidoresComunidad(pPerfilID1, pPerfilID2, pProyectoID, pInicio, pLimite);
        }


        public void ObtenerIDDocCVDesdeVirtuoso(FacetadoDS pFacetadoDS, string pidproyecto, string pididentidad)
        {
            FacetadoAD.ObtenerIDDocCVDesdeVirtuoso(pFacetadoDS, pidproyecto, pididentidad);
        }

        /// <summary>
        /// Modifica la comunidad de en Definición a Abierta
        /// </summary>

        /// <param name="ProyectoID">Identificador de la comunidad</param>

        public void ModificarEstadoComunidadCerrar(string ProyectoID)
        {
            FacetadoAD.ModificarEstadoComunidadCerrar(ProyectoID);

        }

        public void ModificarEstadoPreguntaDebate(string IDproyecto, string IDElementoaModificar, string nuevoestado)
        {
            FacetadoAD.ModificarEstadoPreguntaDebate(IDproyecto, IDElementoaModificar, nuevoestado);
        }

        public void BorrarRecurso(string IDGrafo, Guid IDElementoaEliminar)
        {
            BorrarRecurso(IDGrafo, IDElementoaEliminar, 0);
        }

        public void BorrarRecurso(string IDGrafo, Guid IDElementoaEliminar, int pPrioridad)
        {
            BorrarRecurso(IDGrafo, IDElementoaEliminar, pPrioridad, "", true);
        }

        public void BorrarRecurso(string IDGrafo, Guid IDElementoaEliminar, int pPrioridad, string pInfoExtra, bool pUsarColaActualizacion)
        {
            BorrarRecurso(IDGrafo, IDElementoaEliminar, pPrioridad, "", pUsarColaActualizacion, false);
        }

        public void BorrarRecurso(string IDGrafo, Guid IDElementoaEliminar, int pPrioridad, string pInfoExtra, bool pUsarColaActualizacion, bool pBorrarAuxiliar, bool pBorrarMyGnoss = false)
        {
            try
            {
                FacetadoAD.BorrarRecurso(IDGrafo, IDElementoaEliminar, pPrioridad, pInfoExtra, pUsarColaActualizacion, pBorrarAuxiliar, pBorrarMyGnoss);
            }
            catch (Exception e)
            {
                mLoggingService.GuardarLogError(e,mlogger);
            }
        }

        /// <summary>
        /// Inserta una tripleta que dice que un usuario ignora a otro.
        /// </summary>
        /// <param name="pIdentidadIgnorante">Identidad que ignora</param>
        /// <param name="pIdentidadIgnorado">Identidad que es ignorada</param>
        public void InsertUsuarioIgnoraContacto(Guid pIdentidadIgnorante, Guid pIdentidadIgnorado)
        {
            FacetadoAD.InsertUsuarioIgnoraContacto(pIdentidadIgnorante, pIdentidadIgnorado);
        }

        public void ModificarVotosNegativo(string IDproyecto, string IDElementoaModificar, string TipoElementoAModificar)
        {
            FacetadoAD.ModificarVotosNegativo(IDproyecto, IDElementoaModificar, TipoElementoAModificar);
        }

        public void ModificarVotosVisitasComentarios(string IDproyecto, string IDElementoaModificar, string TipoElementoAModificar)
        {
            ModificarVotosVisitasComentarios(IDproyecto, IDElementoaModificar, TipoElementoAModificar, 1);
        }

        public void ModificarVotosVisitasComentarios(string IDproyecto, string IDElementoaModificar, string TipoElementoAModificar, long pNumVisitas)
        {
            FacetadoAD.ModificarVotosVisitasComentarios(IDproyecto, IDElementoaModificar, TipoElementoAModificar, pNumVisitas);
        }

        public void ModificarCertificacionesRecursos(Guid pProyectoID, Dictionary<int, List<Guid>> pElementosAModificar)
        {
            FacetadoAD.ModificarCertificacionesRecursos(pProyectoID, pElementosAModificar);
        }

		//Eliminado en la versión 2.1.1795.1



		/// <summary>
		/// Dado un recurso en un proyecto devuelve el contenido de su triple search
		/// </summary>
		/// <param name="pProyectoID">identificador del proyecto seleccionado.</param>
		/// <param name="pGuidRecurso">identificador del recurso a buscar.</param>
		/// <returns>Contenido del triple search del recurso.</returns>
		public string ObtenerSearchDeRecurso(string pProyectoID, string pGuidRecurso)
        {
            return FacetadoAD.ObtenerSearchDeRecurso(pProyectoID, pGuidRecurso);
        }

		/// <summary>
		/// Devuelve una lista con los identificadores de los recursos del proyecto que coincidan con los filtros introducidos
		/// </summary>
		/// <param name="pListaFiltros">filtros que deben cumplir los recursos. Es un diccionario donde la clave es el tipo de filtro (por ejemplo: rdf:type, schema:name, etc.) y el valor es una lista con los valores a verificar</param>
		/// <param name="pProyectoId">identificador del proyecto seleccionado</param>
		/// <param name="pTipoProyecto">tipo del proyecto seleccionado</param>
		/// <returns>Lista de los identificadores de los recursos del proyecto que cumplen el filtro</returns>
		public List<string> ObtenerIdsRecursos(Dictionary<string, List<string>> pListaFiltros, string pProyectoId, TipoProyecto pTipoProyecto, bool pConsultaFiltrosBusqueda = false)
        {
            return FacetadoAD.ObtenerIdsRecursos(pListaFiltros, pProyectoId, pTipoProyecto, pConsultaFiltrosBusqueda);
        }


		/// <summary>
		/// Devuelve una lista con los identificadores de los recursos del proyecto que coincidan con los filtros introducidos
		/// </summary>
		/// <param name="pListaFiltros">filtros que deben cumplir los recursos. Es un diccionario donde la clave es el tipo de filtro (por ejemplo: rdf:type, schema:name, etc.) y el valor es una lista con los valores a verificar</param>
		/// <param name="pProyectoId">identificador del proyecto seleccionado</param>
		/// <param name="pTipoProyecto">tipo del proyecto seleccionado</param>
		/// <returns>Lista de los identificadores de los recursos del proyecto que cumplen el filtro</returns>
		/// <returns></returns>
		public int ObtenerNumRecursos(Dictionary<string, List<string>> pListaFiltros, string pProyectoId, TipoProyecto pTipoProyecto, bool pConsultaFiltrosBusqueda = false)
		{
            return FacetadoAD.ObtenerNumRecursos(pListaFiltros, pProyectoId, pTipoProyecto, pConsultaFiltrosBusqueda);
		}

		/// <summary>
		/// Comprueba si un proyecto contiene un recurso concreto
		/// </summary>
		/// <param name="pProyectoID">Identificador del proyecto seleccionado</param>
		/// <param name="pRecurso">Identificador del recurso</param>
		/// <returns>Cierto si el recurso pertenece al proyecto y falso en caso contrario</returns>
		public bool RecursoEstaEnProyecto(string pProyectoID, string pRecurso)
        {
            return FacetadoAD.RecursoEstaEnProyecto(pProyectoID, pRecurso);
        }


		/// <summary>
		/// Devuelve los IDs de los recursos del proyecto dado que contienen pTermino en su triple search 
		/// </summary>
		/// <param name="pProyectoID"></param>
		/// <param name="pTermino"></param>
		/// <returns></returns>
		public List<string> ObtenerIdRecursosConBusquedaPorTextoLibre(string pProyectoID, string pTermino)
        {
            return FacetadoAD.ObtenerIdRecursosConBusquedaPorTextoLibre(pProyectoID, pTermino);
        }

		/// <summary>
		/// Comprueba si la búsqueda mediante la instrucción bif:contains por un término encuentra un recurso concreto
		/// </summary>
		/// <param name="pProyectoID">Identificador del proyecto seleccionado</param>
		/// <param name="pGuidRecurso">Identificador del recurso a buscar</param>
		/// <param name="pTermino">término de búsqueda</param>
		/// <returns>Cierto si el recurso es indexable por el término y falso en caso contrario.</returns>
		public bool RecursoBuscablePorTermino(string pProyectoID, string pRecurso, string pTermino)
        {
            return FacetadoAD.RecursoBuscablePorTermino(pProyectoID, pRecurso, pTermino);
        }

		public void ObtenerAutocompletar(string proyectoID, FacetadoDS pFacetadoDS, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool estaMyGnoss, bool EsMienbroComunidad, bool EsInvitado, string Identidad, int pInicio, int pLimite, List<string> pSemanticos, string pFiltrosContexto)
        {
            //Obtener predicados semánticos. arecipe:type, arecipe:nutrition
            FacetadoAD.ObtenerAutocompletar(proyectoID, pFacetadoDS, pListaFiltros, pListaFiltrosExtra, estaMyGnoss, EsMienbroComunidad, EsInvitado, Identidad, pInicio, pLimite, pSemanticos, pFiltrosContexto);
        }

        public void ObtenerFaceta(string proyectoID, FacetadoDS pFacetadoDS, string pNombreFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool estaMyGnoss, bool EsMienbroComunidad, bool EsInvitado, string Identidad, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, bool pExcluyente, bool pInmutable, bool pEsMovil = false, Guid pPestanyaID = new Guid(), List<Guid> pListaExcluidos = null)
        {
            FacetadoAD.ObtenerFaceta(proyectoID, pFacetadoDS, pNombreFaceta, pListaFiltros, pListaFiltrosExtra, estaMyGnoss, EsMienbroComunidad, EsInvitado, Identidad, pTipoDisenio, pInicio, pLimite, pSemanticos, "", pExcluyente, pInmutable, pEsMovil, pListaExcluidos, pPestanyaID);
        }

        public void ObtenerFaceta(string proyectoID, FacetadoDS pFacetadoDS, string pNombreFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool estaMyGnoss, bool EsMienbroComunidad, bool EsInvitado, string Identidad, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, bool pExcluyente, bool pInmutable, bool pEsMovil = false, List<Guid> pListaExcluidos = null)
        {
            FacetadoAD.ObtenerFaceta(proyectoID, pFacetadoDS, pNombreFaceta, pListaFiltros, pListaFiltrosExtra, estaMyGnoss, EsMienbroComunidad, EsInvitado, Identidad, pTipoDisenio, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pExcluyente, pInmutable, pEsMovil, pListaExcluidos);
        }

        public void ObtenerFaceta(string proyectoID, FacetadoDS pFacetadoDS, string pNombreFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool estaMyGnoss, bool EsMienbroComunidad, bool EsInvitado, string Identidad, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pExcluyente, bool pInmutable = false, bool pEsMovil = false, List<Guid> pListaExcluidos = null)
        {
            FacetadoAD.ObtenerFaceta(proyectoID, pFacetadoDS, pNombreFaceta, pListaFiltros, pListaFiltrosExtra, estaMyGnoss, EsMienbroComunidad, EsInvitado, Identidad, pTipoDisenio, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pTipoProyecto, false, null, pExcluyente, pInmutable, pEsMovil, pListaExcluidos);
        }

        public void ObtenerFaceta(string proyectoID, FacetadoDS pFacetadoDS, string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool estaMyGnoss, bool EsMienbroComunidad, bool EsInvitado, string Identidad, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsRango, List<int> pListaRangos, bool pExcluyente, bool pInmutable, bool pEsMovil = false, List<Guid> pListaExcluidos = null)
        {
            FacetadoAD.ObtenerFaceta(proyectoID, pFacetadoDS, pClaveFaceta, pListaFiltros, pListaFiltrosExtra, estaMyGnoss, EsMienbroComunidad, EsInvitado, Identidad, pTipoDisenio, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pTipoProyecto, pEsRango, pListaRangos, pExcluyente, pInmutable, pEsMovil, pListaExcluidos);
        }

        public void ObtenerFaceta(string proyectoID, FacetadoDS pFacetadoDS, string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool estaMyGnoss, bool EsMienbroComunidad, bool EsInvitado, string Identidad, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsRango, List<int> pListaRangos, bool pExcluida, bool pUsarHilos, bool pExcluirPersonas, bool pInmutable = false, bool pEsMovil = false, List<Guid> pListaExcluidos = null)
        {
            FacetadoAD.ObtenerFaceta(proyectoID, pFacetadoDS, pClaveFaceta, pListaFiltros, pListaFiltrosExtra, estaMyGnoss, EsMienbroComunidad, EsInvitado, Identidad, pTipoDisenio, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pTipoProyecto, pEsRango, pListaRangos, pExcluida, pUsarHilos, pExcluirPersonas, pInmutable);
        }

        public void ObtenerFaceta(string proyectoID, FacetadoDS pFacetadoDS, string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool estaMyGnoss, bool EsMienbroComunidad, bool EsInvitado, string Identidad, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsRango, List<int> pListaRangos, bool pExcluida, bool pUsarHilos, bool pExcluirPersonas, bool pPermitirRecursosPrivados, bool pOmitirPalabrasNoRelevantesSearch, int pReciproca, TipoPropiedadFaceta pTipoPropiedadesFaceta, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pInmutable, bool pEsMovil = false, List<Guid> pListaExcluidos = null)
        {
            FacetadoAD.ObtenerFaceta(proyectoID, pFacetadoDS, pClaveFaceta, pListaFiltros, pListaFiltrosExtra, estaMyGnoss, EsMienbroComunidad, EsInvitado, Identidad, pTipoDisenio, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pTipoProyecto, pEsRango, pListaRangos, pExcluida, pUsarHilos, pExcluirPersonas, pPermitirRecursosPrivados, pOmitirPalabrasNoRelevantesSearch, pReciproca, pTipoPropiedadesFaceta, pFiltrosSearchPersonalizados, pInmutable, pEsMovil, pListaExcluidos);
        }

        /// <summary>
        /// Obtiene una faceta concreta
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pNombreFaceta">Nombre de la faceta que se debe cargar</param>
        /// <param name="pListaFiltros">Lista de filtros</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda se hace en MyGnoss, no en una comunidad</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pOrden">Orden de los resultados</param>
        /// <param name="pInicio">Inicio</param>
        /// <param name="pLimite">límite de resultados</param>
        /// <param name="pEsCatalogoNosocial">Verdad si es un catálogo no social</param>
        /// <param name="pFiltroContextoWhere">Filtros de contexto</param>
        /// <param name="pListaFiltrosExtra">Lista de filtros extra</param>
        /// <param name="pSemanticos">Lista de formularios semánticos</param>
        /// <param name="pFiltrosSearchPersonalizados">Diccionario con los filtros tipo 'search' personalizados</param>
        public void ObtenerTituloFacetas(string pProyectoID, FacetadoDS pFacetadoDS, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, List<int> pListaRangos, bool pUsarHilos, bool pExcluirPersonas, bool pPermitirRecursosPrivados, bool pOmitirPalabrasNoRelevantesSearch, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, Dictionary<string, int> pListaFacetas, Dictionary<string, string> pListaFacetasExtraContexto)
        {
            FacetadoAD.ObtenerTituloFacetas(pProyectoID, pFacetadoDS, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pTipoProyecto, pListaRangos, pUsarHilos, pExcluirPersonas, pPermitirRecursosPrivados, pOmitirPalabrasNoRelevantesSearch, pFiltrosSearchPersonalizados, pListaFacetas, pListaFacetasExtraContexto);
        }

        public void ObtenerContadoresRecursosAgrupadosParaFacetaRangos(string pProyectoID, FacetadoDS pFacetadoDS, string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsRango, List<int> pListaRangos, bool pExcluyente, bool pUsarHilos, bool pExcluirPersonas, bool pPermitirRecursosPrivados, bool pOmitirPalabrasNoRelevantesSearch, int pReciproca, TipoPropiedadFaceta pTipoPropiedadesFaceta, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pInmutable, bool pEsMovil)
        {
            FacetadoAD.ObtenerContadoresRecursosAgrupadosParaFacetaRangos(pProyectoID, pFacetadoDS, pClaveFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pTipoDisenio, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pTipoProyecto, pEsRango, pListaRangos, pExcluyente, pUsarHilos, pExcluirPersonas, pPermitirRecursosPrivados, pOmitirPalabrasNoRelevantesSearch, pReciproca, pTipoPropiedadesFaceta, pFiltrosSearchPersonalizados, pInmutable, pEsMovil);
        }

        public void ObtenerFacetaMultiple(string pProyectoID, FacetadoDS pFacetadoDS, string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsRango, List<int> pListaRangos, bool pExcluyente, bool pUsarHilos, bool pExcluirPersonas, bool pPermitirRecursosPrivados, bool pOmitirPalabrasNoRelevantesSearch, int pReciproca, TipoPropiedadFaceta pTipoPropiedadesFaceta, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pEsMovil, string pConsulta)
        {
            FacetadoAD.ObtenerFacetaMultiple(pProyectoID, pFacetadoDS, pClaveFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pTipoDisenio, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pTipoProyecto, pEsRango, pListaRangos, pExcluyente, pUsarHilos, pExcluirPersonas, pPermitirRecursosPrivados, pOmitirPalabrasNoRelevantesSearch, pReciproca, pTipoPropiedadesFaceta, pFiltrosSearchPersonalizados, pEsMovil, pConsulta);
        }

        public void ObtenerSubrangosDeCantidad(string pProyectoID, FacetadoDS pFacetadoDS, string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsRango, List<int> pListaRangos, bool pExcluyente, bool pUsarHilos, bool pExcluirPersonas, bool pPermitirRecursosPrivados, bool pOmitirPalabrasNoRelevantesSearch, int pReciproca, TipoPropiedadFaceta pTipoPropiedadesFaceta, int pNumCifrasCantidad, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pInmutable, bool pEsMovil)
        {
            FacetadoAD.ObtenerSubrangosDeCantidad(pProyectoID, pFacetadoDS, pClaveFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pTipoDisenio, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pTipoProyecto, pEsRango, pListaRangos, pExcluyente, pUsarHilos, pExcluirPersonas, pPermitirRecursosPrivados, pOmitirPalabrasNoRelevantesSearch, pReciproca, pTipoPropiedadesFaceta, pNumCifrasCantidad, pFiltrosSearchPersonalizados, pInmutable, pEsMovil);
        }

        public void ObtenerFacetaSinOrdenDBLP(string proyectoID, FacetadoDS pFacetadoDS, string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool estaMyGnoss, bool EsMienbroComunidad, bool EsInvitado, string Identidad, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsRango, List<int> pListaRangos, bool pExcluida, bool pUsarHilos)
        {
            FacetadoAD.ObtenerFacetaSinOrdenDBLP(proyectoID, pFacetadoDS, pClaveFaceta, pListaFiltros, pListaFiltrosExtra, estaMyGnoss, EsMienbroComunidad, EsInvitado, Identidad, pTipoDisenio, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pTipoProyecto, pEsRango, pListaRangos, pExcluida, pUsarHilos);
        }

        /// <summary>
        /// Obtiene una faceta concreta
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pNombreFaceta">Nombre de la faceta que se debe cargar</param>
        /// <param name="pListaFiltros">Lista de filtros</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda se hace en MyGnoss, no en una comunidad</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pOrden">Orden de los resultados</param>
        /// <param name="pInicio">Inicio</param>
        /// <param name="pLimite">límite de resultados</param>
        /// <param name="pEsCatalogoNosocial">Verdad si es un catálogo no social</param>
        /// <param name="pFiltroContextoWhere">Filtros de contexto</param>
        /// <param name="pListaFiltrosExtra">Lista de filtros extra</param>
        /// <param name="pSemanticos">Lista de formularios semánticos</param>
        public void ObtenerFacetaEspecialDBLPJournalPartOF(string pProyectoID, FacetadoDS pFacetadoDS, string pNombreFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere)
        {
            FacetadoAD.ObtenerFacetaEspecialDBLPJournalPartOF(pProyectoID, pFacetadoDS, pNombreFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pInicio, pLimite, pSemanticos, pFiltroContextoWhere);
        }

        /// <summary>
        /// Obtiene una faceta concreta
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pNombreFaceta">Nombre de la faceta que se debe cargar</param>
        /// <param name="pListaFiltros">Lista de filtros</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda se hace en MyGnoss, no en una comunidad</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pOrden">Orden de los resultados</param>
        /// <param name="pInicio">Inicio</param>
        /// <param name="pLimite">límite de resultados</param>
        /// <param name="pEsCatalogoNosocial">Verdad si es un catálogo no social</param>
        /// <param name="pFiltroContextoWhere">Filtros de contexto</param>
        /// <param name="pListaFiltrosExtra">Lista de filtros extra</param>
        /// <param name="pSemanticos">Lista de formularios semánticos</param>
        public void ObtenerFacetaEspecialDBLP(string pProyectoID, FacetadoDS pFacetadoDS, string pNombreFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere)
        {
            FacetadoAD.ObtenerFacetaEspecialDBLP(pProyectoID, pFacetadoDS, pNombreFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pInicio, pLimite, pSemanticos, pFiltroContextoWhere);
        }


        /// <summary>
        /// Alberto: Obtiene la información que se va a pintar en la ficha de cada recurso del catálogo
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pDocumentoID">Identificador del documento que se quiere cargar</param>
        /// <returns></returns>
        public void ObtieneInformacionRecursosCatalogoMuseos(Guid pProyectoID, FacetadoDS pFacetadoDS, Guid pDocumentoID)
        {
            List<Guid> listaDocumentos = new List<Guid>();
            listaDocumentos.Add(pDocumentoID);
            ObtieneInformacionRecursosCatalogoMuseos(pProyectoID, pFacetadoDS, listaDocumentos);
        }

        /// <summary>
        /// Alberto: Obtiene la información que se va a pintar en la ficha de cada recurso del catálogo
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pListaDocumentos">Lista de documentos que se quieren cargar</param>
        /// <returns></returns>
        public void ObtieneInformacionRecursosCatalogoMuseos(Guid pProyectoID, FacetadoDS pFacetadoDS, List<Guid> pListaDocumentos)
        {
            FacetadoAD.ObtieneInformacionRecursosCatalogoMuseos(pProyectoID, pFacetadoDS, pListaDocumentos);
        }


        /// <summary>
        /// Obtiene los recursos relacionados de un recurso
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pRecursoID">Identificador del recurso</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda si hace en MyGnoss</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad del usuario</param>
        /// <param name="pInicio">Inicio de los resultados</param>
        /// <param name="pLimite">Límite de los resultados</param>
        public void ObtenerRecursosRelacionados(string pProyectoID, string pRecursoID, FacetadoDS pFacetadoDS, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite)
        {
            FacetadoAD.ObtenerRecursosRelacionados(pProyectoID, pRecursoID, pFacetadoDS, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pInicio, pLimite);
        }

        /// <summary>
        /// Obtiene los mensajes relacionados de un mensaje dado
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pMensajeID">Identificador del mensaje</param>
        /// <param name="pIdentidadID">Identificador de la identidad del usuario</param>
        /// <param name="pLimite">Límite de los resultados</param>
        /// <param name="pNombreUsuarioActual">Nombre del usuario a descartar de los tags</param>
        public FacetadoDS ObtenerMensajesRelacionados(string pUsuarioID, string pMensajeID, string pIdentidadID, int pLimite, string pNombreUsuarioActual)
        {
            return FacetadoAD.ObtenerMensajesRelacionados(pUsuarioID, pMensajeID, pIdentidadID, pLimite, pNombreUsuarioActual);
        }

        /// <summary>
        /// Obtiene las personas recomendadas para otra
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto en el que buscamos personas relacionadas</param>
        /// <param name="pIdentidadID">Identificador de la identidad de la que buscamos personas relacionadas</param>
        /// <param name="pDatosOpcionesExtraRegistro">Datos extra del registro de la persona. El diccionario esta formado de la siguiente manera: predicado -> Lista de Opciones con ese predicado</param>
        /// <param name="pListaCategoriasSuscrita">Lista de categorías a las que está suscrita la persona en esta comunidad</param>
        /// <param name="pLocalidad">Localidad de la persona</param>
        /// <param name="pPais">País de la persona</param>
        /// <param name="pProvincia">Provincia de la persona</param>
        /// <param name="pNumeroPersonas">Numnero de personas</param>
        public FacetadoDS ObtenerPersonasRecomendadas(Guid pProyectoID, Guid pIdentidadID, string pLocalidad, string pProvincia, string pPais, Dictionary<string, string> pDatosOpcionesExtraRegistro, List<Guid> pListaCategoriasSuscrita, int pNumeroPersonas)
        {
            return FacetadoAD.ObtenerPersonasRecomendadas(pProyectoID, pIdentidadID, pLocalidad, pProvincia, pPais, pDatosOpcionesExtraRegistro, pListaCategoriasSuscrita, pNumeroPersonas);
        }

        /// <summary>
        /// Obtiene las personas recomendadas para otra
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <param name="pIdentidadID"></param>
        /// <param name="pNumeroPersonas"></param>
        public FacetadoDS ObtenerPersonasRecomendadas(Guid pProyectoID, Guid pIdentidadID, Dictionary<string, float> pListaPropiedadesPeso, Dictionary<string, Object> pValorPropiedades, int pNumeroPersonas)
        {
            return FacetadoAD.ObtenerPersonasRecomendadas(pProyectoID, pIdentidadID, pListaPropiedadesPeso, pValorPropiedades, pNumeroPersonas);
        }

        /// <summary>
        /// Obtiene los recursos relacionados de un recurso
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pRecursoID">Identificador del recurso</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda si hace en MyGnoss</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad del usuario</param>
        /// <param name="pInicio">Inicio de los resultados</param>
        /// <param name="pLimite">Límite de los resultados</param>
        public void ObtenerRecursosRelacionadosNuevo(string pProyectoID, string pRecursoID, FacetadoDS pFacetadoDS, int pInicio, int pLimite, string pTags, string pConceptID, string pPestanyaRecurso)
        {
            ObtenerRecursosRelacionadosNuevo(pProyectoID, pRecursoID, pFacetadoDS, pInicio, pLimite, pTags, pConceptID, false, pPestanyaRecurso);
        }

        /// <summary>
        /// Obtiene los recursos relacionados de un recurso
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pRecursoID">Identificador del recurso</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda si hace en MyGnoss</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad del usuario</param>
        /// <param name="pInicio">Inicio de los resultados</param>
        /// <param name="pLimite">Límite de los resultados</param>
        public void ObtenerRecursosRelacionadosNuevo(string pProyectoID, string pRecursoID, FacetadoDS pFacetadoDS, int pInicio, int pLimite, string pTags, string pConceptID, bool pEsCatalogoNoSocial, string pPestanyaRecurso)
        {
            FacetadoAD.ObtenerRecursosRelacionadosNuevo(pProyectoID, pRecursoID, pFacetadoDS, pInicio, pLimite, pTags, pConceptID, pEsCatalogoNoSocial, pPestanyaRecurso);
        }

        public void ObtenerResultadosBusqueda(FacetadoDS pFacetadoDS, bool ascOdes, string pTipoFiltro, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, int pInicio, int pLimite, List<string> pSemanticos, Guid pProyectoID, bool pEsUsuarioInvitado, bool pEsIdentidadInvitada, Guid pIdentidadID, bool pEsMovil = false, List<Guid> pListaExcluidos = null, bool pUsarAfinidad = false)
        {
            ObtenerResultadosBusqueda(ascOdes, pFacetadoDS, pTipoFiltro, pListaFiltros, pListaFiltrosExtra, pProyectoID.Equals(ProyectoAD.MyGnoss), !pEsIdentidadInvitada, pEsUsuarioInvitado, pIdentidadID.ToString().ToUpper(), pInicio, pLimite, pSemanticos, "", "", "", 0, pEsMovil, pListaExcluidos, pUsarAfinidad);
        }

        public void ObtenerResultadosBusqueda(FacetadoDS pFacetadoDS, bool ascOdes, string pTipoFiltro, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, int pInicio, int pLimite, List<string> pSemanticos, TipoProyecto pTipoProyecto, Guid pProyectoID, bool pEsUsuarioInvitado, bool pEsIdentidadInvitada, Guid pIdentidadID, bool pEsMovil = false, List<Guid> pListaExcluidos = null, bool pUsarAfinidad = false)
        {
            ObtenerResultadosBusqueda(ascOdes, pFacetadoDS, pTipoFiltro, pListaFiltros, pListaFiltrosExtra, pProyectoID.Equals(ProyectoAD.MyGnoss), !pEsIdentidadInvitada, pEsUsuarioInvitado, pIdentidadID.ToString().ToUpper(), pInicio, pLimite, pSemanticos, "", "", "", 0, pTipoProyecto, "", "", pEsMovil, pListaExcluidos, pUsarAfinidad);
        }

        public void ObtenerResultadosBusqueda(bool pDescendente, FacetadoDS pFacetadoDS, string pTipoFiltro, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoSelect, string pFiltroContextoWhere, string pFiltroContextoOrderBy, int pFiltroContextoPesoMinimo, bool pEsMovil = false, List<Guid> pListaExcluidos = null, bool pUsarAfinidad = false)
        {
            FacetadoAD.ObtenerResultadosBusqueda(mIdGrafo, pDescendente, pFacetadoDS, pTipoFiltro, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID.ToUpper(), pInicio, pLimite, pSemanticos, pFiltroContextoSelect, pFiltroContextoWhere, pFiltroContextoOrderBy, pFiltroContextoPesoMinimo, pEsMovil, pListaExcluidos, string.Empty, pUsarAfinidad);
        }

        public void ObtenerResultadosBusqueda(bool pDescendente, FacetadoDS pFacetadoDS, string pTipoFiltro, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoSelect, string pFiltroContextoWhere, string pFiltroContextoOrderBy, int pFiltroContextoPesoMinimo, TipoProyecto pTipoProyecto, string pNamespacesExtra, string pResultadosEliminar, bool pEsMovil = false, List<Guid> pListaExcluidos = null, bool pUsarAfinidad = false)
        {
            FacetadoAD.ObtenerResultadosBusqueda(mIdGrafo, pDescendente, pFacetadoDS, pTipoFiltro, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID.ToUpper(), pInicio, pLimite, pSemanticos, pFiltroContextoSelect, pFiltroContextoWhere, pFiltroContextoOrderBy, pFiltroContextoPesoMinimo, pTipoProyecto, pNamespacesExtra, pResultadosEliminar, pEsMovil, pListaExcluidos, string.Empty, pUsarAfinidad);
        }

        public void ObtenerResultadosBusqueda(bool pDescendente, FacetadoDS pFacetadoDS, string pTipoFiltro, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoSelect, string pFiltroContextoWhere, string pFiltroContextoOrderBy, int pFiltroContextoPesoMinimo, TipoProyecto pTipoProyecto, string pNamespacesExtra, string pResultadosEliminar, bool pPermitirRecursosPrivados, bool pOmitirPalabrasNoRelevantesSearch, TiposAlgoritmoTransformacion pTipoAlgoritmoTransformacion, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, string pLanguageCode, bool pEsMovil = false, List<Guid> pListaExcluidos = null, bool pUsarAfinidad = false)
        {
            FacetadoAD.ObtenerResultadosBusqueda(mIdGrafo, pDescendente, pFacetadoDS, pTipoFiltro, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID.ToUpper(), pInicio, pLimite, pSemanticos, pFiltroContextoSelect, pFiltroContextoWhere, pFiltroContextoOrderBy, pFiltroContextoPesoMinimo, pTipoProyecto, pNamespacesExtra, pResultadosEliminar, pPermitirRecursosPrivados, pOmitirPalabrasNoRelevantesSearch, pTipoAlgoritmoTransformacion, pFiltrosSearchPersonalizados, pEsMovil, pListaExcluidos, pLanguageCode, pUsarAfinidad);
        }


        public void ObtenerResultadosBusquedaTags(string pProyectoID, FacetadoDS pFacetadoDS, List<string> pTagsDocumento, int pInicio, int pLimite, string pNamespaceExtra, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEsMiembroComunidad, List<string> pSemanticos, string pIdentidadID, bool pEstaEnMyGnoss, bool pEsInvitado)
        {
            FacetadoAD.ObtenerResultadosBusquedaTags(pProyectoID, pFacetadoDS, pTagsDocumento, pInicio, pLimite, pNamespaceExtra, pListaFiltros, pListaFiltrosExtra, pEsMiembroComunidad, pSemanticos, pIdentidadID, pEstaEnMyGnoss, pEsInvitado);
        }

        /// <summary>
        /// Obtiene los resultados de una búsqueda
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDescendente">Indica si el orden es descendente (false si es descendente)</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pTipoFiltro">Tipo de filtro</param>
        /// <param name="pListaFiltros">Lista de filtros del usuario</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda se hace en MyGnoss</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad del usuario</param>
        /// <param name="pInicio">Inicio de los resultados</param>
        /// <param name="pLimite">Fin de los resultados</param>
        /// <param name="pListaFiltrosExtra"></param>
        /// <param name="pSemanticos"></param>
        /// <param name="pFiltroContextoSelect"></param>
        /// <param name="pFiltroContextoWhere"></param>
        /// <param name="pFiltroContextoOrderBy"></param>
        /// <param name="pEsCatalogoNoSocial">Verdad si es un catálogo no social</param>        
        /// <param name="pNamespaceExtra">NamespacesExtra</param>
        public void ObtenerResultadosBusquedaFormatoMapa(FacetadoDS pFacetadoDS, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, List<string> pSemanticos, string pFiltroContextoSelect, string pFiltroContextoWhere, string pFiltroContextoOrderBy, TipoProyecto pTipoProyecto, string pNamespaceExtra, string pResultadosEliminar, DataWrapperFacetas pFiltroMapaDataWrapper, bool pPermitirRecursosPrivados, TipoBusqueda pTipoBusqueda, bool pEsMovil, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, List<PresentacionMapaSemantico> pListaPresentacionMapaSemantico = null, string pLanguageCode = null)
        {
            FacetadoAD.ObtenerResultadosBusquedaFormatoMapa(mIdGrafo, pFacetadoDS, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID.ToUpper(), pSemanticos, pFiltroContextoSelect, pFiltroContextoWhere, pFiltroContextoOrderBy, pTipoProyecto, pNamespaceExtra, pResultadosEliminar, pFiltroMapaDataWrapper, pPermitirRecursosPrivados, pTipoBusqueda, pEsMovil, pFiltrosSearchPersonalizados, pListaPresentacionMapaSemantico, pLanguageCode);
        }

        /// <summary>
        /// Obtiene los resultados de una búsqueda
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDescendente">Indica si el orden es descendente (false si es descendente)</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pTipoFiltro">Tipo de filtro</param>
        /// <param name="pListaFiltros">Lista de filtros del usuario</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda se hace en MyGnoss</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad del usuario</param>
        /// <param name="pInicio">Inicio de los resultados</param>
        /// <param name="pLimite">Fin de los resultados</param>
        /// <param name="pListaFiltrosExtra"></param>
        /// <param name="pSemanticos"></param>
        /// <param name="pFiltroContextoSelect"></param>
        /// <param name="pFiltroContextoWhere"></param>
        /// <param name="pFiltroContextoOrderBy"></param>
        /// <param name="pEsCatalogoNoSocial">Verdad si es un catálogo no social</param>        
        /// <param name="pNamespaceExtra">NamespacesExtra</param>
        /// <param name="pResultadosEliminar"></param>
        /// <param name="pSelectChart">Select chart</param>
        /// <param name="pFiltroChart">Filtros chart</param>
        public void ObtenerResultadosBusquedaFormatoChart(FacetadoDS pFacetadoDS, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, List<string> pSemanticos, string pFiltroContextoSelect, string pFiltroContextoWhere, string pFiltroContextoOrderBy, TipoProyecto pTipoProyecto, string pNamespaceExtra, string pResultadosEliminar, string pSelectChart, string pFiltroChart, bool pPermitirRecursosPrivados, bool pEsMovil, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados)
        {
            FacetadoAD.ObtenerResultadosBusquedaFormatoChart(mIdGrafo, pFacetadoDS, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID.ToUpper(), pSemanticos, pFiltroContextoSelect, pFiltroContextoWhere, pFiltroContextoOrderBy, pTipoProyecto, pNamespaceExtra, pResultadosEliminar, pSelectChart, pFiltroChart, pPermitirRecursosPrivados, pEsMovil, pFiltrosSearchPersonalizados);
        }

        public void ObtenerResultadosBusqueda(string pQuery, string pGrafoID, FacetadoDS pFacetadoDS, int pInicio, int pLimite, string pNamespaceExtra)
        {
            FacetadoAD.ObtenerResultadosBusqueda(pQuery, pGrafoID, pFacetadoDS, pInicio, pLimite, pNamespaceExtra);
        }

        public void ObtienePersonasExacto(FacetadoDS pFacetadoDS, bool ascOdes, string pTipoFiltro, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, int pInicio, int pLimite, bool pEsIdentidadInvitada, bool pEsUsuarioInvitado, Guid pIdentidadID, Guid pProyectoID)
        {
            ObtienePersonasExacto(pFacetadoDS, ascOdes, pTipoFiltro, pListaFiltros, pListaFiltrosExtra, pProyectoID.Equals(ProyectoAD.MyGnoss), !pEsIdentidadInvitada, pEsUsuarioInvitado, pIdentidadID, pInicio, pLimite);
        }

        public void ObtienePersonasExacto(FacetadoDS pFacetadoDS, bool ascOdes, string pTipoFiltro, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEsMyGnoss, bool pEstaEnProyecto, bool pEsUsuarioInvitado, Guid pIdentidadID, int pInicio, int pLimite)
        {
            FacetadoAD.ObtenerPersonas(mIdGrafo, ascOdes, pFacetadoDS, pTipoFiltro, pListaFiltros, pListaFiltrosExtra, pEsMyGnoss, pEstaEnProyecto, pEsUsuarioInvitado, pIdentidadID.ToString().ToUpper(), pInicio, pLimite, new List<string>());
        }

        public int ObtenerValorSegundosParametroAplicacion()
        {
            return FacetadoAD.ObtenerValorSegundosParametroAplicacion();
        }

        public void ObtieneNumeroResultados(FacetadoDS pFacetadoDS, string pNombreFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, List<string> pSemanticos, TiposAlgoritmoTransformacion pTiposAlgoritmoTransformacion, Guid pProyectoID, bool pEsIdentidadInvitada, bool pEsUsuarioInvitado, Guid pIdentidadID, bool pEsMovil = false, List<Guid> pListaExcluidos = null)
        {
            ObtieneNumeroResultados(pFacetadoDS, pNombreFaceta, pListaFiltros, pListaFiltrosExtra, pProyectoID.Equals(ProyectoAD.MyGnoss), !pEsIdentidadInvitada, pEsUsuarioInvitado, pIdentidadID.ToString().ToUpper(), pSemanticos, "", pTiposAlgoritmoTransformacion, pEsMovil, pListaExcluidos);
        }

        public void ObtieneNumeroResultados(FacetadoDS pFacetadoDS, string pNombreFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, List<string> pSemanticos, string pFiltroContextoWhere, TiposAlgoritmoTransformacion pTiposAlgoritmoTransformacion, bool pEsMovil = false, List<Guid> pListaExcluidos = null)
        {
            ObtieneNumeroResultados(pFacetadoDS, pNombreFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID.ToUpper(), pSemanticos, pFiltroContextoWhere, TipoProyecto.Catalogo, true, true, pTiposAlgoritmoTransformacion, null, pEsMovil, pListaExcluidos);
        }

        public void ObtieneNumeroResultados(FacetadoDS pFacetadoDS, string pNombreFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pPermitirRecursosPrivados, bool pOmitirPalabrasNoRelevantesSearch, TiposAlgoritmoTransformacion pTiposAlgoritmoTransformacion, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pEsMovil = false, List<Guid> pListaExcluidos = null)
        {
            FacetadoAD.ObtieneNumeroResultados(mIdGrafo, pFacetadoDS, pNombreFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID.ToUpper(), pSemanticos, pFiltroContextoWhere, pTipoProyecto, pPermitirRecursosPrivados, pOmitirPalabrasNoRelevantesSearch, pTiposAlgoritmoTransformacion, pFiltrosSearchPersonalizados, pEsMovil, pListaExcluidos);
        }

        public void ObtieneInformacionPersonas(FacetadoDS pFacetadoDS, Guid pProyectoID)
        {
            ObtieneInformacionPersonas(pProyectoID, pFacetadoDS);
        }

        public void ObtieneInformacionPersonas(Guid pProyectoID, FacetadoDS pFacetadoDS)
        {
            FacetadoAD.ObtieneInformacionPersonas(pProyectoID.ToString(), pFacetadoDS);
        }

        /// <summary>
        /// Obtiene la información que se va a pintar en la ficha de las personas
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns></returns>
        public FacetadoDS ObtieneInformacionPersonas(Guid pProyectoID, Guid pPersonaID)
        {
            return FacetadoAD.ObtieneInformacionPersonas(pProyectoID, pPersonaID);
        }

        /// <summary>
        /// Obtiene la información que se va a pintar en la ficha de las personas
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pPersonasID">Identificador de la persona</param>
        /// <returns></returns>
        public FacetadoDS ObtieneInformacionPersonas(Guid pProyectoID, List<Guid> pPersonasID)
        {
            return FacetadoAD.ObtieneInformacionPersonas(pProyectoID, pPersonasID);
        }

        /// <summary>
        /// Alberto: Obtiene la información que se va a pintar en la ficha de cada recurso del catálogo
        /// </summary>
        /// <returns></returns>
        public void ObtieneInformacionRecursosCatalogo(Guid pProyectoID, FacetadoDS pFacetadoDS)
        {
            FacetadoAD.ObtieneInformacionRecursosCatalogo(pProyectoID, pFacetadoDS);
        }

        /// <summary>
        /// Inserta  en virtuoso las tripletas necesarias para un nuevo grupo de contactos
        /// </summary>
        /// <param name="pIdentidad">Identidad</param>
        /// <param name="pIdGrupo">idGrupo</param>
        /// /// <param name="pnombreGrupo">nombreGrupo</param>
        public void InsertarNuevoGrupoContactos(Guid pIdentidad, Guid pIdGrupo, string pnombreGrupo)
        {
            FacetadoAD.InsertarNuevoGrupoContactos(pIdentidad.ToString(), pIdGrupo.ToString(), pnombreGrupo);
        }

        /// <summary>
        /// Inserta  en virtuoso las tripletas necesarias para un nuevo grupo de contactos
        /// </summary>
        /// <param name="pIdentidad">Identidad</param>
        /// <param name="pIdGrupo">idGrupo</param>
        /// <param name="idProfesor">id del profesor que crea ek grupo</param>
        /// /// <param name="pnombreGrupo">nombreGrupo</param>
        public void InsertarNuevoGrupoContactos(string pIdentidad, string pIdGrupo, string pnombreGrupo, string idProfesor)
        {
            FacetadoAD.InsertarNuevoGrupoContactos(pIdentidad, pIdGrupo, pnombreGrupo, idProfesor);
        }

        /// <summary>
        /// Inserta  en virtuoso las tripletas necesarias para un nuevo grupo de contacto
        /// </summary>
        /// <param name="perfil1">Identidad</param>
        /// <param name="perfil1">idGrupo</param>
        /// <param name="nombreGrupo">nombre del grupo</param>
        /// <param name="idProfesor">Id del profesor que crea la clase.</param>
        public void InsertarNuevoGrupoContactos(string identidad, string idGrupo, string nombreGrupo, string idProfesor, bool pUsarColaActualizacion)
        {
            FacetadoAD.InsertarNuevoGrupoContactos(identidad, idGrupo, nombreGrupo, idProfesor, pUsarColaActualizacion);
        }

        /// <summary>
        /// Modifica en virtuoso las tripletas necesarias para cambiar el nomrbe a un nuevo grupo de contactos
        /// </summary>
        /// <param name="pIdGrupo">Identidad</param>
        /// <param name="pnombreGrupo">idGrupo</param>
        public void ModificarGrupoContactos(string pIdGrupo, string pNombreGrupo)
        {
            FacetadoAD.ModificarGrupoContactos(pIdGrupo, pNombreGrupo);
        }

        /// <summary>
        /// Obtiene si la identidad tiene foto
        /// </summary>
        /// <param name="pIdentidadID">Identidad ID</param>
        /// <returns>Devuelve si la identidad tiene foto</returns>
        public bool ObtenerSiIdentidadTieneFoto(Guid pIdentidadID, Guid pProyectoID)
        {
            return FacetadoAD.ObtenerSiIdentidadTieneFoto(pIdentidadID, pProyectoID);
        }

        public void ObtieneDatosAutocompletar(string nombregrafo, string filtro, FacetadoDS pfacetadoDS)
        { 
            FacetadoAD.ObtieneDatosAutocompletar(nombregrafo, filtro, pfacetadoDS); 
        }

        /// <summary>
        /// Inserta una serie valores para unos determinados grafos.
        /// </summary>
        /// <param name="pValoresGrafos">Lista con los grafos y sus valores</param>
        /// <param name="pPrioridad">Prioridad que se le va a dar a la replicación de esta transacción</param>
        public void InsertarValoresGrafos(Dictionary<string, List<string>> pValoresGrafos, short pPrioridad)
        {
            foreach (string grafo in pValoresGrafos.Keys)
            {
                foreach (string valor in pValoresGrafos[grafo])
                {
                    FacetadoAD.InsertaTripleta(grafo, "<http://gnoss/" + grafo + ">", "<http://gnoss/has" + grafo + ">", "\"" + valor + "\"", pPrioridad, false);
                }
            }
        }

        /// <summary>
        /// Borra una serie valores para unos determinados grafos.
        /// </summary>
        /// <param name="pValoresGrafos">Lista con los grafos y sus valores</param>
        /// <param name="pPrioridad">Prioridad que se le va a dar a la replicación de esta transacción</param>
        public void BorrarValoresGrafos(Dictionary<string, List<string>> pValoresGrafos, short pPrioridad)
        {
            foreach (string grafo in pValoresGrafos.Keys)
            {
                foreach (string valor in pValoresGrafos[grafo])
                {
                    List<TripleWrapper> triples = new List<TripleWrapper>();
                    triples.Add(new TripleWrapper { Subject = "<http://gnoss/" + grafo + ">", Predicate = "<http://gnoss/has" + grafo + ">", Object = "\"" + valor + "\"" });
                    FacetadoAD.BorrarGrupoTripletasEnLista(grafo, triples, false, "");
                }
            }
        }

        /// <summary>
        /// Inserta un valor en un grafo
        /// </summary>
        /// <param name="pGrafo">grafo en el que se va a insertar el valor</param>
        /// <param name="pValor">valor a insertar en el grafo</param>
        /// <param name="pPrioridad">prioridad que se le va a dar a la replicación de esta transacción</param>
        public void InsertarValorGrafo(string pGrafo, string pValor, short pPrioridad)
        {
            FacetadoAD.InsertaTripleta(pGrafo, "<http://gnoss/" + pGrafo + ">", "<http://gnoss/has" + pGrafo + ">", "\"" + pValor + "\"", pPrioridad, true);
        }

        /// <summary>
        /// Inserta una serie de tripletas en Virtuoso.
        /// </summary>
        /// <param name="pNombreGrafo">Nombre del grafo donde se insertarán las tripletas</param>
        /// <param name="ptripletas">Tripletas</param>
        /// <param name="pPrioridad">Prioridad que se le va a dar a la replicación de esta transacción</param>
        public void InsertaTripletas(string pNombreGrafo, string ptripletas, short pPrioridad)
        {
            InsertaTripletas(pNombreGrafo, ptripletas, pPrioridad, false);
        }

        public void InsertaTripletas(string pNombreGrafo, string ptripletas, short pPrioridad, bool pUsarColaActualizacion)
        {
            FacetadoAD.InsertaTripletas(pNombreGrafo, ptripletas, pPrioridad, pUsarColaActualizacion, false, "");
        }

        public void InsertaTripletas(string pNombreGrafo, string ptripletas, short pPrioridad, bool pUsarColaActualizacion, bool pEscribirNT)
        {
            FacetadoAD.InsertaTripletas(pNombreGrafo, ptripletas, pPrioridad, pUsarColaActualizacion, pEscribirNT, "");
        }

        public void InsertaTripletas(string pNombreGrafo, string ptripletas, short pPrioridad, bool pUsarColaActualizacion, bool pEscribirNT, string pInfoExtra)
        {
            FacetadoAD.InsertaTripletas(pNombreGrafo, ptripletas, pPrioridad, pUsarColaActualizacion, pEscribirNT, pInfoExtra);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pNombreGrafo">Nombre del grafo en el que se guardará el RDF</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        /// <param name="pFicheroConfiguracion">Fichero de configuración de la BD</param>
        /// <param name="pProyectoID">ID del proyecto actual</param>
        /// <param name="pEsribirNT">Escribe o no NT</param>
        /// <param name="pInfoExtra">Info extra</param>
        /// <param name="pTripletas">Tripletas a guardar en virtuoso</param>
        /// <param name="pElementoaEliminarID">ID del documento a eliminar</param>
        /// <param name="pPrioridad"></param>>
        /// <param name="pInfoExtraReplicacion"></param>
        /// <param name="pUsarColareplicacion"></param>
        public void InsertaTripletasRecursoSemanticoConModify(string pNombreGrafo, string pUrlIntragnoss, string pFicheroConfiguracion, Guid pProyectoID, string pInfoExtraReplicacion, bool pUsarColareplicacion, string pElementoaEliminarID, string pTripletas, short pPrioridad, bool pEscribirNT)
        {
            FacetadoAD.InsertaTripletasRecursoSemanticoConModify(pNombreGrafo, pUrlIntragnoss, pFicheroConfiguracion, pProyectoID, pInfoExtraReplicacion, pUsarColareplicacion, pElementoaEliminarID, pTripletas, pPrioridad, pEscribirNT);
        }

        public string ObtieneTripletasFormularios(DataWrapperFacetas FacetaDW, FacetadoDS facetadoDS, string idproyecto, string iddoc, List<string> pListRdfTypePadre)
        {
            return FacetadoAD.ObtieneTripletasFormularios(facetadoDS, idproyecto, iddoc, pListRdfTypePadre);
        }

        public void ObtieneTripletasFormulariosCV(FacetadoDS facetadoDS, string idproyecto, string iddoc)
        {
            FacetadoAD.ObtieneTripletasFormulariosCV(facetadoDS, idproyecto, iddoc);
        }

        /// <summary>
        /// Obtiene las triplas de un ELEMENTO cualquiera.
        /// </summary>
        /// <param name="pGrafo">Grafo</param>
        /// <param name="pDocumentoID">ID del elemento</param>
        /// <returns>DataSet con una tabla con las triples</returns>
        public DataSet ObtieneTripletasRecursoEnGrafo(string pGrafo, Guid pElementoID)
        {
            return FacetadoAD.ObtieneTripletasRecursoEnGrafo(pGrafo, pElementoID);
        }

        /// <summary>
        /// Obtiene las triplas de un ELEMENTO cualquiera.
        /// </summary>
        /// <param name="pGrafo">Grafo</param>
        /// <param name="pDocumentoID">ID del elemento</param>
        /// <returns>DataSet con una tabla con las triples</returns>
        public DataSet ObtieneTripletasRecursoEspecificoEnGrafo(string pGrafo, string pSujeto)
        {
            return FacetadoAD.ObtieneTripletasRecursoEspecificoEnGrafo(pGrafo, pSujeto);
        }

        /// <summary>
        /// Obtiene el objeto de las triplas de un documento en un grafo filtrado por una faceta (puede ser jerárquica).
        /// </summary>
        /// <param name="pGrafo">Grafo</param>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <param name="pFaceta">Faceta</param>
        /// <returns>DataSet con una tabla con las triplas</returns>
        public DataSet ObtenerValorFacetaDocumentoEnGrafo(string pGrafo, Guid pDocumentoID, string pFaceta)
        {
            return FacetadoAD.ObtenerValorFacetaDocumentoEnGrafo(pGrafo, pDocumentoID, pFaceta);
        }

        /// <summary>
        /// Devuelve las primeras categorías de un tesauro semántico.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pPropiedad">Propiedad vinculante</param>
        /// <param name="pTipoEntidadSolicitada">Tipo de entidad solicitada</param>
        /// <param name="pPropSolicitadas">Propiedades de la entidad solicitadas</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerCatPrimerNivelTesSemanticoFormulario(string pGrafo, string pPropiedad, string pTipoEntidadSolicitada, List<string> pPropSolicitadas)
        {
            return FacetadoAD.ObtenerCatPrimerNivelTesSemanticoFormulario(pGrafo, pPropiedad, pTipoEntidadSolicitada, pPropSolicitadas);
        }

        /// <summary>
        /// Devuelve las categorías hijas de una categoría de un tesauro semántico.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pIDCategoria">ID de la categoría padre</param>
        /// <param name="pPropRelacion">Propiedad de unión entre categorías</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerCatHijasCatTesSemanticoFormulario(string pGrafo, string pIDCategoria, string pPropRelacion)
        {
            return FacetadoAD.ObtenerCatHijasCatTesSemanticoFormulario(pGrafo, pIDCategoria, pPropRelacion);
        }

        /// <summary>
        /// Devuelve el RDF de un formulario semántico.
        /// </summary>
        /// <param name="pNombreontologia">Nombre de la ontología</param>
        /// <param name="pIDDocSem">ID del documento semántico</param>
        /// <returns>DataSet con el RDF del documento</returns>
        public FacetadoDS ObtenerRDFXMLdeFormulario(string pNombreontologia, string pIDDocSem, bool pUsarAfinidad = false)
        {
            return FacetadoAD.ObtenerRDFXMLdeFormulario(pNombreontologia, pIDDocSem, pUsarAfinidad);
        }

        /// <summary>
        /// Devuelve el trozo del RDF de un formulario semántico perteneciente al filtro para seleccionar una entidad.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pEntContenedora">Entidad contenedora</param>
        /// <param name="pPropiedad">Propiedad vinculante</param>
        /// <param name="pTipoEntidadSolicitada">Tipo de entidad solicitada</param>
        /// <param name="pPropSolicitadas">Propiedades de la entidad solicitadas</param>
        public FacetadoDS ObtenerRDFXMLSelectorEntidadFormulario(string pGrafo, string pEntContenedora, string pPropiedad, string pTipoEntidadSolicitada, List<string> pPropSolicitadas)
        {
            return FacetadoAD.ObtenerRDFXMLSelectorEntidadFormulario(pGrafo, pEntContenedora, pPropiedad, pTipoEntidadSolicitada, pPropSolicitadas, null, null, null, Guid.Empty, Guid.Empty);
        }

        /// <summary>
        /// Devuelve el trozo del RDF de un formulario semántico perteneciente al filtro para seleccionar una entidad.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pEntContenedora">Entidad contenedora</param>
        /// <param name="pPropiedad">Propiedad vinculante</param>
        /// <param name="pTipoEntidadSolicitada">Tipo de entidad solicitada</param>
        /// <param name="pPropSolicitadas">Propiedades de la entidad solicitadas</param>
        /// <param name="pFiltro">Filtro</param>
        /// <param name="pExtraWhere">Cadena extra para el where de la consulta</param>
        /// <param name="pIdioma">Idioma</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerRDFXMLSelectorEntidadFormulario(string pGrafo, string pEntContenedora, string pPropiedad, string pTipoEntidadSolicitada, List<string> pPropSolicitadas, string pFiltro, string pExtraWhere, string pIdioma, Guid pIdentidadID, Guid pProyectoID)
        {
            pExtraWhere = ExtraerYHacerConsultasInternasWhere(pExtraWhere);

            FacetadoDS facetadoAuxDS = null;

            List<string> propsSinJerarquia = new List<string>();
            List<string> propsConJerarquia = new List<string>();

            foreach (string prop in pPropSolicitadas)
            {
                if (prop.Contains("|") || prop.Contains("(")) //Entidad externa
                {
                    propsConJerarquia.Add(prop);
                }
                else
                {
                    propsSinJerarquia.Add(prop);
                }
            }

            if (propsSinJerarquia.Count == 0 && propsConJerarquia.Count > 0)
            {
                propsSinJerarquia.Add(propsConJerarquia[0]);//Solo la primera propiedad, si no falla la consulta.
                propsConJerarquia.RemoveAt(0);
            }

            if (propsSinJerarquia.Count > 0)
            {
                facetadoAuxDS = FacetadoAD.ObtenerRDFXMLSelectorEntidadFormulario(pGrafo, pEntContenedora, pPropiedad, pTipoEntidadSolicitada, propsSinJerarquia, pFiltro, pExtraWhere, pIdioma, pIdentidadID, pProyectoID);

                if (propsConJerarquia.Count > 0)
                {
                    List<string> sujetos = new List<string>();

                    foreach (DataRow fila in facetadoAuxDS.Tables[0].Rows)
                    {
                        string sujeto = (string)fila[0];

                        if (!sujetos.Contains(sujeto))
                        {
                            sujetos.Add(sujeto);
                        }
                    }

                    if (sujetos.Count > 0)
                    {
                        //Bucle para hacer las consutlas DRFP
                        while (sujetos.Count > 0)
                        {
                            int numeroFin = 2000;
                            if (sujetos.Count < 2000)
                            {
                                numeroFin = sujetos.Count;
                            }
                            facetadoAuxDS.Merge(ObtenerValoresPropiedadesEntidadesConJerarquiaYExternas(pGrafo, sujetos.GetRange(0, numeroFin), propsConJerarquia, false));
                            sujetos.RemoveRange(0, numeroFin);
                        }
                    }
                }
            }

            return facetadoAuxDS;
        }

        /// <summary>
        /// Extrae las consultas internas del where extra, las hace y las reemplaza por el resultado.
        /// </summary>
        /// <param name="pExtraWhere">Where extra</param>
        /// <returns>Where extra con las consultas reemplazadas por los resultados</returns>
        private string ExtraerYHacerConsultasInternasWhere(string pExtraWhere)
        {
            if (pExtraWhere != null && pExtraWhere.Contains("["))
            {
                string trozo1 = pExtraWhere.Substring(0, pExtraWhere.IndexOf("["));
                int indiceCierre = indiceCierreCorchete(pExtraWhere);
                string consulta = pExtraWhere.Substring(0, indiceCierre);
                consulta = consulta.Substring(pExtraWhere.IndexOf("[") + 1);
                string trozo2 = pExtraWhere.Substring(indiceCierre + 1);

                return trozo1 + ResultadoConsultaWhere(consulta) + trozo2;
            }
            else
            {
                return pExtraWhere;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pConsulta"></param>
        /// <returns></returns>
        private string ResultadoConsultaWhere(string pConsulta)
        {
            pConsulta = ExtraerYHacerConsultasInternasWhere(pConsulta);

            FacetadoDS facDS = RealizarConsultaAVirtuoso("", pConsulta);

            string sujetos = "";
            foreach (DataRow fila in facDS.Tables[0].Rows)
            {
                sujetos += "<" + (string)fila[0] + ">,";
            }

            if (!string.IsNullOrEmpty(sujetos))
            {
                sujetos = sujetos.Substring(0, sujetos.Length - 1);
            }

            facDS.Dispose();

            if (string.IsNullOrEmpty(sujetos)) //Ponemos un resultado malo que no traiga nada, pero que no casque.
            {
                sujetos = "<sin_resultado>";
            }

            return sujetos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pExtraWhere"></param>
        /// <returns></returns>
        private int indiceCierreCorchete(string pExtraWhere)
        {
            int indice1abertura = pExtraWhere.IndexOf("[");
            int indice1Cierre = pExtraWhere.IndexOf("]");

            string intermedio = pExtraWhere.Substring(0, indice1Cierre);
            intermedio = intermedio.Substring(indice1abertura + 1);

            while (intermedio.Contains("["))
            {
                pExtraWhere = pExtraWhere.Substring(pExtraWhere.IndexOf("]") + 1);
                int indiceSigCierre = pExtraWhere.IndexOf("]");
                indice1Cierre += indiceSigCierre + 1;
                intermedio = intermedio.Substring(intermedio.IndexOf("[") + 1);
            }

            return indice1Cierre;
        }

        /// <summary>
        /// Devuelve el trozo del RDF de un formulario semántico perteneciente al filtro para seleccionar una entidad.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pEntidadID">ID de la entidad origen</param>
        /// <param name="pConsulta">Consulta a realizar</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerRDFXMLSelectorEntidadFormularioPorConsulta(SelectorEntidad pSelectorEntidad, string pEntidadID, string pConsulta, string pIdioma)
        {
            return FacetadoAD.ObtenerRDFXMLSelectorEntidadFormularioPorConsulta(pSelectorEntidad, pEntidadID, pConsulta, pIdioma);
        }

        /// <summary>
        /// Realiza un consulta a virtuoso.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pConsulta">Consulta</param>
        /// <returns>Resultado de la consulta a virtuoso</returns>
        public FacetadoDS RealizarConsultaAVirtuoso(string pGrafo, string pConsulta)
        {
            return FacetadoAD.RealizarConsultaAVirtuoso(pGrafo, pConsulta);
        }

        /// <summary>
        /// Devuelve el trozo del RDF de un formulario semántico perteneciente al filtro para seleccionar una entidad.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pEntsContenedoras">IDs de la entidad origen</param>
        /// <param name="pConsulta">Consulta a realizar</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerValoresPropiedadesEntidadesPorConsulta(string pGrafo, List<string> pEntsContenedoras, string pConsulta)
        {
            return FacetadoAD.ObtenerValoresPropiedadesEntidadesPorConsulta(pGrafo, pEntsContenedoras, pConsulta);
        }

        /// <summary>
        /// Devuelve las entidades grafo dependientes que cumplen un filtro y son hijas de un determindado padre.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pTipoEntDep">Tipo de entidad solicitada</param>
        /// <param name="pIDValorPadre">ID del padre de las entidades filtradas</param>
        /// <param name="pFiltro">Filtro</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerValoresGrafoDependientesFormulario(string pGrafo, string pTipoEntDep, string pIDValorPadre, string pFiltro)
        {
            return FacetadoAD.ObtenerValoresGrafoDependientesFormulario(pGrafo, pTipoEntDep, pIDValorPadre, pFiltro);
        }

        /// <summary>
        /// Devuelve las entidades grafo dependientes con sus valores.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pEntidades">Entidades solicitadas</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerValoresGrafoDependientesDeEntidades(string pGrafo, List<string> pEntidades)
        {
            return FacetadoAD.ObtenerValoresGrafoDependientesDeEntidades(pGrafo, pEntidades);
        }

        public void EliminarConceptEHijos(string pGrafo, string pSujetoConcept, bool pEliminarHijos)
        {
            FacetadoAD.EliminarConceptEHijos(pGrafo, pSujetoConcept, pEliminarHijos);
        }

        /// <summary>
        /// Devuelve el trozo del RDF de un formulario semántico perteneciente al filtro para seleccionar unas entidades.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pEntsContenedora">Entidades contenedoras</param>
        /// <param name="pPropiedades">Propiedades solicitadas</param>
        /// <param name="pEntsContenedoras">Indica si hay que traer el idoma de los triples</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerValoresPropiedadesEntidadesTodas(string pGrafo, List<string> pPropiedades, bool pTraerIdioma, string pIdioma = "es")
        {
            return FacetadoAD.ObtenerValoresPropiedadesEntidadesTodas(pGrafo, pPropiedades, pTraerIdioma, pIdioma);
        }

        /// <summary>
        /// Devuelve el trozo del RDF de un formulario semántico perteneciente al filtro para seleccionar una entidad.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pEntContenedora">Entidad contenedora</param>
        /// <param name="pPropiedades">Propiedades solicitadas</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerValoresPropiedadesEntidad(string pGrafo, string pEntContenedora, List<string> pPropiedades)
        {
            List<string> listaEntidades = new List<string>();
            listaEntidades.Add(pEntContenedora);
            return FacetadoAD.ObtenerValoresPropiedadesEntidades(pGrafo, listaEntidades, pPropiedades, false);
        }

        /// <summary>
        /// Devuelve el trozo del RDF de un formulario semántico perteneciente al filtro para seleccionar unas entidades.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pEntsContenedora">Entidades contenedoras</param>
        /// <param name="pPropiedades">Propiedades solicitadas</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerValoresPropiedadesEntidades(string pGrafo, List<string> pEntsContenedoras, List<string> pPropiedades, bool pUsarAfinidad = false)
        {
            return ObtenerValoresPropiedadesEntidades(pGrafo, pEntsContenedoras, pPropiedades, true, pUsarAfinidad);
        }

        /// <summary>
        /// Devuelve el trozo del RDF de un formulario semántico perteneciente al filtro para seleccionar unas entidades.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pEntsContenedora">Entidades contenedoras</param>
        /// <param name="pPropiedades">Propiedades solicitadas</param>
        /// <param name="pEntsContenedoras">Indica si hay que traer el idoma de los triples</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerValoresPropiedadesEntidades(string pGrafo, List<string> pEntsContenedoras, List<string> pPropiedades, bool pTraerIdioma, bool pUsarAfinidad = false)
        {
            return FacetadoAD.ObtenerValoresPropiedadesEntidades(pGrafo, pEntsContenedoras, pPropiedades, pTraerIdioma, pUsarAfinidad);
        }

        /// <summary>
        /// Devuelve el trozo del RDF de un formulario semántico perteneciente al filtro para seleccionar unas entidades.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pEntContenedora">Entidad contenedora</param>
        /// <param name="pEntsContenedoras">Indica si hay que traer el idoma de los triples</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerValoresPropiedadesEntidad(string pGrafo, string pEntContenedora, bool pTraerIdioma)
        {
            List<string> listaEntidades = new List<string>();
            listaEntidades.Add(pEntContenedora);
            return ObtenerValoresPropiedadesEntidades(pGrafo, listaEntidades, pTraerIdioma);
        }

        /// <summary>
        /// Devuelve el trozo del RDF de un formulario semántico perteneciente al filtro para seleccionar unas entidades.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pEntsContenedora">Entidades contenedoras</param>
        /// <param name="pEntsContenedoras">Indica si hay que traer el idoma de los triples</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerValoresPropiedadesEntidades(string pGrafo, List<string> pEntsContenedoras, bool pTraerIdioma)
        {
            return FacetadoAD.ObtenerValoresPropiedadesEntidades(pGrafo, pEntsContenedoras, pTraerIdioma);
        }

        /// <summary>
        /// Devuelve el trozo del RDF de un formulario semántico perteneciente al filtro para seleccionar unas entidades anidadas.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pEntsContenedora">Entidades contenedoras</param>
        /// <param name="pPropiedades">Propiedades solicitadas</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerValoresPropiedadesEntidadesAnidadas(string pGrafo, List<string> pEntsContenedoras, List<string> pPropiedades, bool pUsarAfinidad = false
)
        {
            return FacetadoAD.ObtenerValoresPropiedadesEntidadesAnidadas(pGrafo, pEntsContenedoras, pPropiedades);
        }

        /// <summary>
        /// Devuelve el trozo del RDF de un formulario semántico perteneciente al filtro para seleccionar unas entidades.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pEntsContenedora">Entidades contenedoras</param>
        /// <param name="pPropiedades">Propiedades solicitadas</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerValoresPropiedadesEntidadesConJerarquiaYExternas(string pGrafo, List<string> pEntsContenedoras, List<string> pPropiedades, bool pAnidadas)
        {
            FacetadoDS facetadoAuxDS = null;

            if (pEntsContenedoras.Count > 0)
            {
                List<string> propsSinEntExternas = new List<string>();
                List<string> propsConEntExternas = new List<string>();

                foreach (string prop in pPropiedades)
                {
                    if (prop.Contains("(")) //Entidad externa
                    {
                        propsSinEntExternas.Add(prop.Substring(0, prop.IndexOf("(")));
                        propsConEntExternas.Add(prop);
                    }
                    else
                    {
                        propsSinEntExternas.Add(prop);
                    }
                }

                if (!pAnidadas)
                {
                    facetadoAuxDS = FacetadoAD.ObtenerValoresPropiedadesEntidades(pGrafo, pEntsContenedoras, propsSinEntExternas, true);
                }
                else
                {
                    facetadoAuxDS = FacetadoAD.ObtenerValoresPropiedadesEntidadesAnidadas(pGrafo, pEntsContenedoras, propsSinEntExternas);
                }

                #region Entidades externas

                foreach (string prop in propsConEntExternas)
                {
                    string propEntExt = prop.Substring(0, prop.IndexOf("("));

                    List<string> listaValorPropExt = new List<string>();
                    List<string> entidadesEntExt = new List<string>();

                    foreach (string entidadID in pEntsContenedoras)
                    {
                        foreach (string sujetoID in ObtenerObjetosDataSetSegunPropiedad(facetadoAuxDS, entidadID, propEntExt))
                        {
                            string entidadExtID = sujetoID;
                            entidadesEntExt.Add(entidadExtID);
                        }
                    }

                    if (entidadesEntExt.Count > 0)
                    {
                        string grafoProps = prop.Substring(prop.IndexOf("(") + 1);
                        grafoProps = grafoProps.Substring(0, grafoProps.LastIndexOf(")"));
                        string[] arrayPropsEntExt = grafoProps.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                        string grafo = arrayPropsEntExt[0];
                        List<string> listPropsEntExt = new List<string>();
                        int hayParent = 0;

                        foreach (string propArray in arrayPropsEntExt)
                        {
                            if (hayParent == 0)
                            {
                                listPropsEntExt.Add(propArray);

                                if (propArray.Contains("("))
                                {
                                    hayParent++;
                                }
                            }
                            else
                            {
                                listPropsEntExt[listPropsEntExt.Count - 1] += ";" + propArray;

                                if (propArray.Contains("("))
                                {
                                    hayParent++;
                                }
                                else if (propArray.Contains(")"))
                                {
                                    hayParent--;
                                }
                            }
                        }

                        listPropsEntExt.RemoveAt(0); //Quito el grafo de las propiedades

                        facetadoAuxDS.Merge(ObtenerValoresPropiedadesEntidadesConJerarquiaYExternas(grafo, entidadesEntExt, listPropsEntExt, pAnidadas));

                    }
                }

                #endregion
            }

            return facetadoAuxDS;
        }

        /// <summary>
        /// Obtiene el objeto del dataSet de una propiedad.
        /// </summary>
        /// <param name="pDataSet">DataSet</param>
        /// <param name="pSujeto">Sujeto</param>
        /// <param name="pPropiedad">Propiedad</param>
        /// <returns>Objeto del dataSet de una propiedad</returns>
        public static List<string> ObtenerSujetosDataSetSegunPropiedad(DataSet pDataSet, string pPropiedad)
        {
            List<string> sujetos = new List<string>();

            foreach (DataRow fila in pDataSet.Tables[0].Select("p='" + pPropiedad + "'"))
            {
                if (!sujetos.Contains((string)fila[0]))
                {
                    sujetos.Add((string)fila[0]);
                }
            }

            return sujetos;
        }

        /// <summary>
        /// Obtiene los sujetos con una propiedad y un objeto
        /// </summary>
        /// <param name="pDataSet">DataSet</param>
        /// <param name="pPropiedad">Propiedad</param>
        /// <param name="pObjeto">Objeto</param>
        /// <returns>Objeto del dataSet de una propiedad</returns>
        public static List<string> ObtenerSujetosDataSetSegunPropiedadYObjeto(DataSet pDataSet, string pPropiedad, string pObjeto)
        {
            List<string> sujetos = new List<string>();

            foreach (DataRow fila in pDataSet.Tables[0].Select("p='" + pPropiedad + "' AND o='" + pObjeto + "'"))
            {
                if (!sujetos.Contains((string)fila[0]))
                {
                    sujetos.Add((string)fila[0]);
                }
            }

            return sujetos;
        }

        /// <summary>
        /// Obtiene el objeto del dataSet de una propiedad.
        /// </summary>
        /// <param name="pDataSet">DataSet</param>
        /// <param name="pSujeto">Sujeto</param>
        /// <param name="pPropiedad">Propiedad</param>
        /// <returns>Objeto del dataSet de una propiedad</returns>
        public static List<string> ObtenerObjetosDataSetSegunPropiedad(DataSet pDataSet, string pSujeto, string pPropiedad)
        {
            return ObtenerObjetosDataSetSegunPropiedad(pDataSet, pSujeto, pPropiedad, "");
        }

        /// <summary>
        /// Obtiene el objeto del dataSet de una propiedad.
        /// </summary>
        /// <param name="pDataSet">DataSet</param>
        /// <param name="pSujeto">Sujeto</param>
        /// <param name="pPropiedad">Propiedad</param>
        /// <returns>Objeto del dataSet de una propiedad</returns>
        public static List<string> ObtenerObjetosDataSetSegunPropiedad(DataSet pDataSet, string pSujeto, string pPropiedad, string pIdioma)
        {
            string idiomaUsado;
            return ObtenerObjetosDataSetSegunPropiedad(pDataSet, pSujeto, pPropiedad, pIdioma, out idiomaUsado);
        }

        /// <summary>
        /// Obtiene el objeto del dataSet de una propiedad.
        /// </summary>
        /// <param name="pDataSet">DataSet</param>
        /// <param name="pSujeto">Sujeto</param>
        /// <param name="pPropiedad">Propiedad</param>
        /// <returns>Objeto del dataSet de una propiedad</returns>
        public static List<string> ObtenerObjetosDataSetSegunPropiedad(DataSet pDataSet, string pSujeto, string pPropiedad, string pIdioma, out string pIdiomaUsado)
        {
            pIdiomaUsado = "";
            Dictionary<string, List<string>> listaConIdioma = ObtenerObjetosDataSetSegunPropiedadConIdioma(pDataSet, pSujeto, pPropiedad);

            if (listaConIdioma.ContainsKey(pIdioma))
            {
                pIdiomaUsado = pIdioma;
                return listaConIdioma[pIdioma];
            }
            else if (!string.IsNullOrEmpty(pIdioma) && listaConIdioma.ContainsKey(""))
            {
                return listaConIdioma[""];
            }
            else
            {
                if (listaConIdioma.Count > 0)
                {
                    KeyValuePair<string, List<string>> clave = listaConIdioma.First();
                    pIdiomaUsado = clave.Key;
                    return clave.Value;
                }
            }

            return new List<string>();
        }

        /// <summary>
        /// Obtiene el objeto del dataSet de una propiedad.
        /// </summary>
        /// <param name="pDataSet">DataSet</param>
        /// <param name="pSujeto">Sujeto</param>
        /// <param name="pPropiedad">Propiedad</param>
        /// <returns>Objeto del dataSet de una propiedad</returns>
        public static Dictionary<string, List<string>> ObtenerObjetosDataSetSegunPropiedadConIdioma(DataSet pDataSet, string pSujeto, string pPropiedad)
        {
            Dictionary<string, List<string>> listaObjetos = new Dictionary<string, List<string>>();

            if (pPropiedad.Contains("("))
            {
                string propBuscar = pPropiedad.Substring(0, pPropiedad.IndexOf("("));
                pPropiedad = pPropiedad.Substring(pPropiedad.IndexOf("(") + 1);
                pPropiedad = pPropiedad.Substring(0, pPropiedad.LastIndexOf(")"));
                pPropiedad = pPropiedad.Substring(pPropiedad.IndexOf(";") + 1);
                string[] propsEntExt = pPropiedad.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> propsEntExtFinal = new List<string>();

                int hayParent = 0;

                foreach (string prop in propsEntExt) //Si hay mas ent ext dentro de este hay que juntar sus cachos.
                {
                    if (hayParent == 0)
                    {
                        propsEntExtFinal.Add(prop);

                        if (prop.Contains("("))
                        {
                            hayParent++;
                        }
                    }
                    else
                    {
                        propsEntExtFinal[propsEntExtFinal.Count - 1] += ";" + prop;

                        if (prop.Contains("("))
                        {
                            hayParent++;
                        }
                        else if (prop.Contains(")"))
                        {
                            hayParent--;
                        }
                    }
                }

                List<string> listaSujetos = ObtenerObjetosDataSetSegunPropiedad(pDataSet, pSujeto, propBuscar);

                foreach (string sujeto in listaSujetos)
                {
                    foreach (string prop in propsEntExtFinal)
                    {
                        Dictionary<string, List<string>> listaObjetosAux = ObtenerObjetosDataSetSegunPropiedadConIdioma(pDataSet, sujeto, prop);
                        foreach (string idioma in listaObjetosAux.Keys)
                        {
                            if (!listaObjetos.ContainsKey(idioma))
                            {
                                listaObjetos.Add(idioma, new List<string>());
                            }

                            listaObjetos[idioma].AddRange(listaObjetosAux[idioma]);
                        }
                    }
                }
            }
            else if (pPropiedad.Contains("|"))
            {
                string propBuscar = pPropiedad.Substring(0, pPropiedad.IndexOf("|"));
                pPropiedad = pPropiedad.Substring(pPropiedad.IndexOf("|") + 1);

                foreach (DataRow fila in pDataSet.Tables[0].Select("s='" + pSujeto + "' AND p='" + propBuscar + "'"))
                {
                    Dictionary<string, List<string>> listaObjetosAux = ObtenerObjetosDataSetSegunPropiedadConIdioma(pDataSet, (string)fila[2], pPropiedad);
                    foreach (string idioma in listaObjetosAux.Keys)
                    {
                        if (!listaObjetos.ContainsKey(idioma))
                        {
                            listaObjetos.Add(idioma, new List<string>());
                        }

                        listaObjetos[idioma].AddRange(listaObjetosAux[idioma]);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(pPropiedad))
            {
                string[] propDeNivel = pPropiedad.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string propNivel in propDeNivel)
                {
                    foreach (DataRow fila in pDataSet.Tables[0].Select("s='" + pSujeto + "' AND p='" + propNivel + "'"))
                    {
                        string idioma = "";

                        if (fila.ItemArray.Length > 3 && !fila.IsNull(3))
                        {
                            idioma = (string)fila[3];
                        }

                        if (!listaObjetos.ContainsKey(idioma))
                        {
                            listaObjetos.Add(idioma, new List<string>());
                        }

                        string valor = "";
                        if (!fila.IsNull(2))
                        {
                            valor = (string)fila[2];
                        }

                        listaObjetos[idioma].Add(valor);
                    }
                }
            }

            return listaObjetos;
        }

        /// <summary>
        /// Devuelve el trozo del RDF de un formulario semántico perteneciente al filtro para seleccionar unas entidades.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pListaDocumentosID">Listado de documentos</param>
        /// <param name="pPropiedades">Propiedades solicitadas</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerValoresPropiedadesEntidadesPorDocumentoID(string pGrafo, List<Guid> pListaDocumentosID, List<string> pPropiedades)
        {
            return ObtenerValoresPropiedadesEntidadesPorDocumentoID(pGrafo, pListaDocumentosID, pPropiedades, null, true);
        }

        /// <summary>
        /// Devuelve el trozo del RDF de un formulario semántico perteneciente al filtro para seleccionar unas entidades.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pListaDocumentosID">Listado de documentos</param>
        /// <param name="pNombreTabla">Nombre de la tabla</param>
        /// <param name="pSelect">Select</param>
        /// <param name="pWhere">Where</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerValoresPropiedadesPersonalizadasEntidadesPorDocumentoID(string pGrafo, List<Guid> pListaDocumentosID, string pNombreTabla, string pSelect, string pWhere, string pIdimoa)
        {
            return FacetadoAD.ObtenerValoresPropiedadesPersonalizadasEntidadesPorDocumentoID(pGrafo, pListaDocumentosID, pNombreTabla, pSelect, pWhere, true, pIdimoa);
        }

        /// <summary>
        /// Devuelve el trozo del RDF de un formulario semántico perteneciente al filtro para seleccionar unas entidades.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pListaDocumentosID">Listado de documentos</param>
        /// <param name="pPropiedades">Propiedades solicitadas</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerValoresPropiedadesEntidadesPorDocumentoID(string pGrafo, List<Guid> pListaDocumentosID, List<string> pPropiedades, string pIdioma, bool pUsarClienteWeb, bool pUsarAfinidad = false)
        {
            return ObtenerValoresPropiedadesEntidadesPorDocumentoID(pGrafo, pListaDocumentosID, pPropiedades, pIdioma, pUsarClienteWeb, null, null, false, string.Empty, null, string.Empty, TipoProyecto.Catalogo, false, false, null, false, false, string.Empty, false, pUsarAfinidad);
        }

        /// <summary>
        /// Devuelve el trozo del RDF de un formulario semántico perteneciente al filtro para seleccionar unas entidades.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pListaDocumentosID">Listado de documentos</param>
        /// <param name="pPropiedades">Propiedades solicitadas</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerValoresPropiedadesEntidadesPorDocumentoID(string pGrafo, List<Guid> pListaDocumentosID, List<string> pPropiedades, string pIdioma, bool pUsarClienteWeb, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEsMiembroComunidad, string pProyectoID, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pExcluirPersonas, bool pOmitirPalabrasNoRelevantesSearch, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pEstaEnMyGnoss, bool pEsInvitado, string pIdentidadID, bool pEsExportacionExcel = false, bool pUsarAfinidad = false)
        {
            return FacetadoAD.ObtenerValoresPropiedadesEntidadesPorDocumentoID(pGrafo, pListaDocumentosID, pPropiedades, pIdioma, pUsarClienteWeb, pListaFiltros, pListaFiltrosExtra, pEsMiembroComunidad, pProyectoID, pSemanticos, pFiltroContextoWhere, pTipoProyecto, pExcluirPersonas, pOmitirPalabrasNoRelevantesSearch, pFiltrosSearchPersonalizados, pEstaEnMyGnoss, pEsInvitado, pIdentidadID, pEsExportacionExcel, pUsarAfinidad);
        }

        /// <summary>
        /// Obtiene los recursos relacionados con el actual para montar un grafo gráfico.
        /// </summary>
        /// <param name="pGrafo">Grafo de búsqueda</param>
        /// <param name="pDocumentoID">ID del documento actual</param>
        /// <param name="pPropEnlace">Propiedad de enlace entre recursos</param>
        /// <param name="pNodosLimiteNivel">Número de nodos a partir del cual hay que detenerse</param>
        /// <param name="pExtra">Configuración extra</param>
        /// <param name="pIdioma">Idioma del usuario</param>
        /// <param name="pTipoRecurso">Tipo del recurso actual</param>
        /// <returns>Array JS con las relacionados con el actual para montar un grafo gráfico</returns>
        public string ObtenerRelacionesGrafoGraficoDeDocumento(string pGrafo, Guid pDocumentoID, string pPropEnlace, int pNodosLimiteNivel, string pExtra, string pIdioma, string pTipoRecurso, string pGrafoDbpedia = null)
        {
            return FacetadoAD.ObtenerRelacionesGrafoGraficoDeDocumento(pGrafo, pDocumentoID, pPropEnlace, pNodosLimiteNivel, pExtra, pIdioma, pTipoRecurso, pGrafoDbpedia);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pUsuarioID">Grafo de la consulta</param>
        /// <param name="pListaComentariosID">Listado de comentarios</param>
        /// <returns></returns>
        public Dictionary<Guid, bool> ObtenerLeidoListaComentarios(Guid pUsuarioID, List<Guid> pListaComentariosID)
        {
            return FacetadoAD.ObtenerLeidoListaComentarios(pUsuarioID, pListaComentariosID);
        }


        /// <summary>
        /// Borra una tripleta concreta
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pSujeto">Sujeto</param>
        /// <param name="pPredicado">Predicado</param>
        /// <param name="pObjeto">Objeto</param>
        public void BorrarTripleta(string pProyectoID, string pSujeto, string pPredicado, string pObjeto)
        {
            FacetadoAD.BorrarTripleta(pProyectoID, pSujeto, pPredicado, pObjeto, false);
        }

        /// <summary>
        /// Borra una tripleta concreta
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pSujeto">Sujeto</param>
        /// <param name="pPredicado">Predicado</param>
        /// <param name="pObjeto">Objeto</param>
        public void BorrarTripleta(string pProyectoID, string pSujeto, string pPredicado, string pObjeto, bool pUsarColaActualizacion)
        {
            FacetadoAD.BorrarTripleta(pProyectoID, pSujeto, pPredicado, pObjeto, pUsarColaActualizacion);
        }

        /// <summary>
        /// Borra un grupo de tripletas de un grafo.
        /// </summary>
        /// <param name="pGrafo">Identificador del grafo</param>
        /// <param name="pTripletas">Tripletas a borrar</param>
        /// <param name="pUsarColaActualizacion">TRUE si se va a mandar </param>
        public void BorrarGrupoTripletas(string pGrafo, string pTripletas, bool pUsarColaActualizacion)
        {
            FacetadoAD.BorrarGrupoTripletas(pGrafo, pTripletas, pUsarColaActualizacion);
        }

        /// <summary>
        /// Borra un grupo de tripletas en lista de un grafo.
        /// </summary>
        /// <param name="pGrafo">Identificador del grafo</param>
        /// <param name="pTripletas">Tripletas a borrar</param>
        /// <param name="pUsarColaActualizacion">TRUE si se va a mandar </param>
        public int BorrarGrupoTripletasEnLista(string pGrafo, List<TripleWrapper> pTripletas, bool pUsarColaActualizacion)
        {
            return FacetadoAD.BorrarGrupoTripletasEnLista(pGrafo, pTripletas, pUsarColaActualizacion, "");
        }

        /// <summary>
        /// Borra un una lista de triples de un sujeto que contengan unos predicados concretos
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pSujeto">Sujeto del que se van a eliminar los triples</param>
        /// <param name="pListaPredicados">Predicados que deben contener los triples a borrar del sujeto</param>
        public void BorrarListaPredicadosDeSujeto(string pProyectoID, string pSujeto, List<string> pListaPredicados)
        {
            FacetadoAD.BorrarListaPredicadosDeSujeto(pProyectoID, pSujeto, pListaPredicados);
        }

        /// <summary>
        /// Borra todos los triples de los sujetos indicados
        /// </summary>
        /// <param name="pGrafo">Identificador del proyecto</param>
        /// <param name="pSujeto">Lista de sujetos del que se van a eliminar los triples</param>
        public void BorrarListaTriplesDeSujeto(string pGrafo, List<string> pSujeto)
        {
            FacetadoAD.BorrarListaTriplesDeSujeto(pGrafo, pSujeto);
        }

        /// <summary>
        /// Borra un grupo de tripletas en lista de un grafo.
        /// </summary>
        /// <param name="pGrafo">Identificador del grafo</param>
        /// <param name="pTripletas">Tripletas a borrar</param>
        /// <param name="pUsarColaActualizacion">TRUE si se va a mandar </param>
        public int BorrarGrupoTripletasEnLista(string pGrafo, List<TripleWrapper> pTripletas, bool pUsarColaActualizacion, string pInfoExtra)
        {
            return FacetadoAD.BorrarGrupoTripletasEnLista(pGrafo, pTripletas, pUsarColaActualizacion, pInfoExtra);
        }

        /// <summary>
        /// Borra un grupo de tripletas en lista de un grafo.
        /// </summary>
        /// <param name="pGrafo">Identificador del grafo</param>
        /// <param name="pTripletas">Tripletas a borrar</param>
        /// <param name="pUsarColaActualizacion">TRUE si se va a mandar </param>
        public int ModificarGrupoTripletasEnLista(string pGrafo, string[] pTripletas, string pTripletaInsertar, bool pUsarColaActualizacion)
        {
            return ModificarGrupoTripletasEnLista(pGrafo, pTripletas, pTripletaInsertar, pUsarColaActualizacion, "");
        }

        /// <summary>
        /// Borra un grupo de tripletas en lista de un grafo.
        /// </summary>
        /// <param name="pGrafo">Identificador del grafo</param>
        /// <param name="pTripletas">Tripletas a borrar</param>
        /// <param name="pUsarColaActualizacion">TRUE si se va a mandar </param>
        public int ModificarGrupoTripletasEnLista(string pGrafo, string[] pTripletas, string pTripletaInsertar, bool pUsarColaActualizacion, string pInfoExtra)
        {
            return FacetadoAD.ModificarGrupoTripletasEnLista(pGrafo, pTripletas, pTripletaInsertar, pUsarColaActualizacion, pInfoExtra);
        }

        /// <summary>
        /// Modifica una lista de triples en un grafo, sustituyendo los triples viejos por los nuevos
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pTriplesInsertar">Tripletas a borrar</param>
        /// <param name="pPrioridad">Prioridad de la replicación</param>
        /// <param name="pSujeto">Sujeto al que se le van a modificar los triples. (Opcional, si no se le da valor, se eliminaran todos los triples que contenan los predicados en pListaPredicadosEliminar)</param>
        public void ModificarListaTripletas(string pProyectoID, string pTriplesInsertar, short pPrioridad, string pSujeto)
        {
            FacetadoAD.ModificarListaTripletas(pProyectoID, pTriplesInsertar, null, pPrioridad, pSujeto, true);
        }

        /// <summary>
        /// Modifica una lista de triples en un grafo, sustituyendo los triples que tengan algún predicado en pListaPredicadosEliminar por los nuevos triples
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pTriplesInsertar">Tripletas a borrar</param>
        /// <param name="pListaPredicadosEliminar">Lista de predicados que van a actualizar los triples.</param>
        /// <param name="pPrioridad">Prioridad de la replicación</param>
        /// <param name="pSujeto">Sujeto al que se le van a modificar los triples. (Opcional, si no se le da valor, se eliminaran todos los triples que contenan los predicados en pListaPredicadosEliminar)</param>
        public void ModificarListaTripletas(string pProyectoID, string pTriplesInsertar, List<string> pListaPredicadosEliminar, short pPrioridad, string pSujeto)
        {
            FacetadoAD.ModificarListaTripletas(pProyectoID, pTriplesInsertar, pListaPredicadosEliminar, pPrioridad, pSujeto);
        }

        /// <summary>
        /// Borra lsa tripletas de un formulario semántico
        /// </summary>
        /// <param name="pNombreGrafo">Nombre del grafo en virtuoso</param>
        /// <param name="pElementoaEliminarID">Identificador del elemento a eliminar</param>
        public void BorrarTripletasFormularioSemantico(string pNombreGrafo, string pElementoaEliminarID)
        {
            FacetadoAD.BorrarTripletasFormularioSemantico(pNombreGrafo, pElementoaEliminarID, false);
        }

        /// <summary>
        /// Borra lsa tripletas de un formulario semántico
        /// </summary>
        /// <param name="pNombreGrafo">Nombre del grafo en virtuoso</param>
        /// <param name="pElementoaEliminarID">Identificador del elemento a eliminar</param>
        public void BorrarTripletasFormularioSemantico(string pNombreGrafo, string pElementoaEliminarID, bool pActualizarVirtuoso)
        {
            FacetadoAD.BorrarTripletasFormularioSemantico(pNombreGrafo, pElementoaEliminarID, pActualizarVirtuoso);
        }

        /// <summary>
        /// Borra lsa tripletas de un formulario semántico
        /// </summary>
        /// <param name="pNombreGrafo">Nombre del grafo en virtuoso</param>
        /// <param name="pElementoaEliminarID">Identificador del elemento a eliminar</param>
        public void BorrarTripletasFormularioSemantico(string pNombreGrafo, string pElementoaEliminarID, bool pActualizarVirtuoso, string pInfoExtra)
        {
            FacetadoAD.BorrarTripletasFormularioSemantico(pNombreGrafo, pElementoaEliminarID, pActualizarVirtuoso, pInfoExtra);
        }

        /// <summary>
        /// Actualiza virtuoso (mediante un insert / update / delete)
        /// </summary>
        /// <param name="pQuery">Query a ejecutar (insert / update / delete)</param>
        /// <param name="pGrafo">Grafo que se va a actualiar</param>
        /// <param name="pReplicar">Verdad si esta consulta debe replicarse (por defecto TRUE)</param>
        /// <param name="pPrioridad">Prioridad que se le va a dar a la replicación de esta transacción</param>
        public int ActualizarVirtuoso(string pQuery, string pGrafo, bool pReplicar, short pPrioridad)
        {
            return FacetadoAD.ActualizarVirtuoso(pQuery, pGrafo, pReplicar, pPrioridad);
        }

        /// <summary>
        /// Devuelve el peso para el proyectoID seleccionado
        /// </summary>
        /// <param name="pProyectoID">ProyectoID del que quiero obtener el peso</param>
        /// <returns>Peso del proyectoID pasado.</returns>
        public int ObtenerPopularidadProyecto(Guid pProyectoID)
        {
            return FacetadoAD.ObtenerPopularidadProyecto(pProyectoID);
        }

        public string ObtenerGrafoOntologiaRecurso(string pRecursoID)
        {
            return FacetadoAD.ObtenerGrafoEliminarSujeto(pRecursoID);
        }

        /// <summary>
        /// Devuelve el ranking que tiene actualmente el Documento en el proyecto pasado como parámetro
        /// </summary>
        /// <param name="pDocumentoID"></param>
        /// <param name="pProyID"></param>
        /// <returns></returns>
        public int ObtenerRankingDocumento(Guid pDocumentoID, Guid pProyID)
        {
            return FacetadoAD.ObtenerRankingDocumento(pDocumentoID, pProyID);
        }


        public DataSet ObtenerIdentidadesEnProyectoID(Guid pProyectoID, List<string> pRdfTypeList)
        {
            return FacetadoAD.ObtenerIdentidadesEnProyectoID(pProyectoID, pRdfTypeList);
        }

        /// <summary>
        /// Lee datos de virtuoso (mediante una consulta SELECT)
        /// </summary>
        /// <param name="pQuery">Consulta a ejecutar (select)</param>
        /// <param name="pNombreTabla">Nombre de la tabla a cargar en el dataset</param>
        /// <param name="pGrafo">Grafo sobre el que se realiza la consulta</param>
        public FacetadoDS LeerDeVirtuoso(string pQuery, string pNombreTabla, string pGrafo)
        {
            return FacetadoAD.LeerDeVirtuoso(pQuery, pNombreTabla, pGrafo);
        }

        /// <summary>
        /// Lee datos de virtuoso (mediante una consulta SELECT)
        /// </summary>
        /// <param name="pQuery">Consulta a ejecutar (select)</param>
        /// <param name="pNombreTabla">Nombre de la tabla a cargar en el dataset</param>
        /// <param name="pFacetadoDS">Dataset a cargar</param>
        /// <param name="pGrafo">Grafo sobre el que se realiza la consulta</param>
        public void LeerDeVirtuoso(string pQuery, string pNombreTabla, FacetadoDS pFacetadoDS, string pGrafo)
        {
            FacetadoAD.LeerDeVirtuoso(pQuery, pNombreTabla, pFacetadoDS, pGrafo);
        }

        public IDataReader EjecutarDataReader(string pQuery, string pGrafo)
        {
            return FacetadoAD.EjecutarDataReader(pQuery, pGrafo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IDGrafo"></param>
        /// <param name="IDElementoaEliminar"></param>
        public void ModificarMensajeAEliminado(string IDGrafo, string IDElementoaEliminar)
        {
            try
            {
                FacetadoAD.ModificarMensajeAEliminado(IDGrafo, IDElementoaEliminar);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IDGrafo"></param>
        /// <param name="IDElementoaEliminar"></param>
        public void ModificarSuscripcionCaducidad(string IDGrafo, string IDElementoaEliminar)
        {
            try
            {
                FacetadoAD.ModificarSuscripcionCaducidad(IDGrafo, IDElementoaEliminar);
            }
            catch (Exception) { }
        }

        public FacetadoDS ObtenerTriplesOntologiaACompartir(Guid pProyectoID, string pEnlace)
        {
            return FacetadoAD.ObtenerTriplesOntologiaACompartir(pProyectoID, pEnlace);
        }

        public int ObtenerMensajesPendientesLeerPerfilID(Guid pUsuarioID, Guid pIdentidadID, string pBandeja)
        {
            return FacetadoAD.ObtenerNumMensajesPerfilID_Bandeja(pUsuarioID, pIdentidadID, pBandeja);
        }

        /// <summary>
        /// Obtiene los sujetos de una propriedad por su valor.
        /// </summary>
        /// <param name="pGrafo">Grafo</param>
        /// <param name="pPropiedad">Propiedad</param>
        /// <param name="pListaValores">Valores</param>
        /// <returns>Lista con los sujetos de una propriedad por su valor</returns>
        public List<string> ObjeterSujetosDePropiedadPorValor(string pGrafo, string pPropiedad, List<string> pListaValores)
        {
            return FacetadoAD.ObjeterSujetosDePropiedadPorValor(pGrafo, pPropiedad, pListaValores);
        }

        public string ObtenerFiltroConsultaMapaProyectoDesdeDataSetParaFacetas(DataWrapperFacetas dataWrapperFacetas, TipoBusqueda pTipoBusqueda, Dictionary<string, List<string>> pListaFiltros, Dictionary<string, List<string>> pInformacionOntologias)
        {
            return FacetadoAD.ObtenerFiltroConsultaMapaProyectoDesdeDataSetParaFacetas(dataWrapperFacetas, pTipoBusqueda, pListaFiltros);
        }

        public string ObtenerFiltroConsultaMapaProyectoDesdeDataSet(DataWrapperFacetas pDataWrapperFacetas, TipoBusqueda pTipoBusqueda, bool pPuntos, bool pRutas)
        {
            return FacetadoAD.ObtenerFiltroConsultaMapaProyectoDesdeDataSet(pDataWrapperFacetas, pTipoBusqueda, pPuntos, pRutas);
        }

        public FacetadoDS ObtieneTripletasOtrasEntidadesDS(string pRecursoID, string pGrafoID, List<FacetaEntidadesExternas> pEntExt)
        {
            return FacetadoAD.ObtieneTripletasOtrasEntidadesDS(pRecursoID, pGrafoID, pEntExt);
        }

        public bool ComprobarSiComentarioNoExisteOHaSidoLeidoPerfil(string pComentarioId, string pPerfilId)
        {
            return FacetadoAD.ComprobarSiComentarioNoExisteOHaSidoLeidoPerfil(pComentarioId, pPerfilId);
        }

        /// <summary>
        /// Obtiene todas las triples de un tesauro semántico.
        /// </summary>
        /// <param name="pGrafo">Grafo del tesaro</param>
        /// <param name="pSource">Source del tesauro</param>
        /// <returns>Triples de un tesauro semántico</returns>
        public FacetadoDS ObtenerTesauroSemantico(string pGrafo, string pSource)
        {
            return FacetadoAD.ObtenerTesauroSemantico(pGrafo, pSource);
        }

        public FacetadoDS ObtenerEntidadesEnProyectoIDDocumentoID(Guid pProyectoID, Guid pDocumentoID)
        {
            return FacetadoAD.ObtenerEntidadesEnProyectoIDDocumentoID(pProyectoID, pDocumentoID);
        }


		public string ObtenerConsultaDeFaceta(string pProyectoID, string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra,
            bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, TipoDisenio pTipoDisenio, int pInicio, int pLimite, 
            List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsRango, List<int> pListaRangos, bool pExcluyente, 
            bool pExcluirPersonas, bool pPermitirRecursosPrivados, bool pOmitirPalabrasNoRelevantesSearch, int pReciproca, 
            TipoPropiedadFaceta pTipoPropiedadesFaceta, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pInmutable, 
            bool pEsMovil, List<Guid> pListaExcluidos = null, Guid pPestanyaID = new Guid()) 
        {
            int numeroAuxiliarVariablesIguales = 2;
            string nombreFacetaSinPrefijo = string.Empty;
            string consultaReciproca = string.Empty;
			string ultimaFacetaAux = string.Empty;
			if (pListaFiltros.ContainsKey("autocompletar"))
			{
				ultimaFacetaAux = "autocompletar";
			}
			FacetadoAD.ObtenerDatosFiltroReciproco(out consultaReciproca, pClaveFaceta, out pClaveFaceta);

			return FacetadoAD.ObtenerConsultaDeFaceta(numeroAuxiliarVariablesIguales, nombreFacetaSinPrefijo, consultaReciproca, ultimaFacetaAux,
                pProyectoID, pClaveFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID,
                pTipoDisenio, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pTipoProyecto, pEsRango, pListaRangos, pExcluyente, pExcluirPersonas,
                pPermitirRecursosPrivados, pOmitirPalabrasNoRelevantesSearch, pReciproca, pTipoPropiedadesFaceta, pFiltrosSearchPersonalizados,
                pInmutable, pEsMovil, pListaExcluidos, pPestanyaID);
        }



        #endregion

        #region Propiedades

        public List<Guid> ListaComunidadesPrivadasUsuario
        {
            get
            {
                return this.FacetadoAD.ListaComunidadesPrivadasUsuario;
            }
            set
            {
                this.FacetadoAD.ListaComunidadesPrivadasUsuario = value;
            }
        }


        public List<string> ListaItemsBusquedaExtra
        {
            get
            {
                return this.FacetadoAD.ListaItemsBusquedaExtra;
            }
            set
            {
                this.FacetadoAD.ListaItemsBusquedaExtra = value;
            }
        }

        public List<string> PropiedadesRango
        {
            get
            {
                return this.FacetadoAD.PropiedadesRango;
            }
            set
            {
                this.FacetadoAD.PropiedadesRango = value;
            }
        }

        public List<string> PropiedadesFecha
        {
            get
            {
                return this.FacetadoAD.PropiedadesFecha;
            }
            set
            {
                this.FacetadoAD.PropiedadesFecha = value;
            }
        }

        public Dictionary<string, List<string>> InformacionOntologias
        {
            get
            {
                return this.FacetadoAD.InformacionOntologias;
            }
            set
            {
                this.FacetadoAD.InformacionOntologias = value;
            }
        }

        public string MandatoryRelacion
        {
            get
            {
                return this.FacetadoAD.MandatoryRelacion;
            }
            set
            {
                this.FacetadoAD.MandatoryRelacion = value;
            }
        }

        /// <summary>
        /// DataAdapter de Blog
        /// </summary>
        public FacetadoAD FacetadoAD
        {
            get
            {
                return (FacetadoAD)AD;
            }
            set
            {
                this.AD = value;
            }
        }

        public string GrafoID
        {
            get
            {
                return mIdGrafo;
            }
        }

        /// <summary>
        /// Condición extra para la consulta de facetas.
        /// </summary>
        public string CondicionExtraFacetas
        {
            get
            {
                return FacetadoAD.CondicionExtraFacetas;
            }
            set
            {
                FacetadoAD.CondicionExtraFacetas = value;
            }
        }

        public Dictionary<string, bool> DiccionarioFacetasExcluyentes
        {
            get
            {
                return FacetadoAD.DiccionarioFacetasExcluyentes;
            }
            set
            {
                FacetadoAD.DiccionarioFacetasExcluyentes = value;
            }
        }

        public DataWrapperFacetas FacetaDW
        {
            get
            {
                return this.FacetadoAD.FacetaDW;
            }
            set
            {
                this.FacetadoAD.FacetaDW = value;
            }
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
        ~FacetadoCN()
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
                    if (FacetadoAD != null && !mEntityContext.NoConfirmarTransacciones)
                    {
                        FacetadoAD.CerrarConexion();
                        FacetadoAD.Dispose();
                    }
                }

                FacetadoAD = null;
            }
        }

        #endregion

        #region Actualizaciones

        /// <summary>
        /// Generate the Subject of the Collection given by parameter
        /// </summary>
        /// <param name="pCollection">Collection to generate the Subject</param>
        /// <param name="pUrlIntragnoss">UrlIntragnoss</param>
        /// <param name="pSource">Source of the thesaurus</param>
        /// <returns>The Subject of the Collection</returns>
        private string GenerarSujetoCollection(Collection pCollection, string pUrlIntragnoss, string pSource)
        {
            string sujeto = pCollection.Subject;
            if (string.IsNullOrEmpty(sujeto))
            {
                sujeto = $"{pUrlIntragnoss}items/{pSource}";
            }

            if (!sujeto.StartsWith(pUrlIntragnoss))
            {
                sujeto = $"{pUrlIntragnoss}items/{sujeto}";
            }

            return sujeto;
        }

        /// <summary>
        /// Add triples to the string builder to generate the Collection in the ontology graph (Source)
        /// </summary>
        /// <param name="pCollection">Collection to be generated</param>
        /// <param name="pStringBuilder">StringBuilder where the triples will be stored</param>
        /// <param name="pSujeto">Subject of the Collection to generate</param>
        private void GenerarTriplesCollectionGrafoBusqueda(Collection pCollection, StringBuilder pStringBuilder, string pSujeto, string pSource)
        {
            pStringBuilder.AppendLine($"<{pSujeto}> <http://purl.org/dc/elements/1.1/source> \"{pSource}\" . ");
        }

        /// <summary>
        /// Add triples to the string builder to generate the Collection in the ontology graph (RdfType, Label, Source)
        /// </summary>
        /// <param name="pStringBuilder">StringBuilder where the triples will be stored</param>
        /// <param name="pSujeto">Subject of the Collection to generate</param>
        /// <param name="pSource">Source of the tesaurus</param>
        private void GenerarTriplesCollectionGrafoOntologia(StringBuilder pStringBuilder, string pSujeto, string pSource)
        {
            pStringBuilder.AppendLine($"<{pSujeto}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2008/05/skos#Collection> . ");
            pStringBuilder.AppendLine($"<{pSujeto}> <http://www.w3.org/2000/01/rdf-schema#label> \"http://www.w3.org/2008/05/skos#Collection\" . ");
            pStringBuilder.AppendLine($"<{pSujeto}> <http://purl.org/dc/elements/1.1/source> \"{pSource}\" . ");
        }

        /// <summary>
        /// Generate the triples of the property ScopeNote with their language
        /// </summary>
        /// <param name="pCollection">Collection that has the property ScopeNote</param>
        /// <param name="pStringBuilderOntologia">StringBuilder where the triples of the ontology graph will be storage</param>
        /// <param name="pStringBuilderBusqueda">StringBuilder where the triples of the search graph will be storage</param>
        /// <param name="pSujeto">Subject of the Collection</param>
        private void GenerarScopeNoteMultiIdioma(Collection pCollection, StringBuilder pStringBuilderOntologia, StringBuilder pStringBuilderBusqueda, string pSujeto)
        {
            foreach (string clave in pCollection.ScopeNote.Keys)
            {
                pStringBuilderOntologia.AppendLine($"<{pSujeto}> <http://www.w3.org/2008/05/skos#scopeNote> \"{pCollection.ScopeNote[clave]}\"@{clave} . ");
                pStringBuilderBusqueda.AppendLine($"<{pSujeto}> <http://www.w3.org/2008/05/skos#scopeNote> \"{pCollection.ScopeNote[clave]}\"@{clave}  . ");
            }
        }

        /// <summary>
        /// Generate the triples that define the relation between the Collection and their narrowers
        /// </summary>
        /// <param name="pCollection">Collection to generate the triples</param>
        /// <param name="pStringBuilderOntologia">StringBuilder where the triples will be storage to the ontology graph</param>
        /// <param name="pStringBuilderBusqueda">StringBuilder where the triples will be storage to the search graph</param>
        /// <param name="pSujeto">Subject of the collection</param>
        /// <param name="pUrlIntraGnoss">UrlIntragnoss</param>
        /// <param name="pSource">Source of the Thesaurus</param>
        /// <exception cref="Exception"></exception>
        private void GenerarTriplesRelacionesCollection(Collection pCollection, StringBuilder pStringBuilderOntologia, StringBuilder pStringBuilderBusqueda, string pSujeto, string pUrlIntraGnoss, string pSource)
        {
            if (pCollection.Member != null)
            {
                foreach (Concept narrower in pCollection.Member)
                {
                    pStringBuilderOntologia.AppendLine($"<{pSujeto}> <http://www.w3.org/2008/05/skos#member> <{GenerarSujetoConcept(narrower, pUrlIntraGnoss, pSource)}> . ");
                    pStringBuilderBusqueda.AppendLine($"<{pSujeto}> <http://www.w3.org/2008/05/skos#member> <{GenerarSujetoConcept(narrower, pUrlIntraGnoss, pSource)}> . ");
                }
            }           
        }

        /// <summary>
        /// Generate the Subject of the Concept given by parameter
        /// </summary>
        /// <param name="pConcept">Concept to generate the Subject</param>
        /// <param name="pUrlIntragnoss">UrlIntragnoss</param>
        /// <param name="pSource">Source of the tesaurus</param>
        /// <returns>The Subject of the Concept</returns>
        private string GenerarSujetoConcept(Concept pConcept, string pUrlIntragnoss, string pSource)
        {
            string sujeto = pConcept.Subject;
            if (string.IsNullOrEmpty(sujeto))
            {
                sujeto = $"{pUrlIntragnoss}items/{pSource}_{pConcept.Identifier}";
            }

            if (!sujeto.StartsWith(pUrlIntragnoss))
            {
                sujeto = $"{pUrlIntragnoss}items/{sujeto}";
            }

            return sujeto;
        }

        /// <summary>
        /// Insert all the triples for the list of the Collections given in the Thesaurus parameter
        /// </summary>
        /// <param name="pTesauro">Thesaurus to load</param>
        /// <param name="pUrlIntragnoss">Url intragnoss</param>
        /// <param name="pProyectoID">Identifier of the project</param>
        /// <param name="pFacetadoCN">FacetadoCN initializated</param>
        /// <exception cref="Exception"></exception>
        public void InsertarTriplesCollection(Thesaurus pTesauro, string pUrlIntragnoss, Guid pProyectoID)
        {
            StringBuilder triplesCollectionGrafoOntologia = new StringBuilder();
            StringBuilder triplesCollectionGrafoBusqueda = new StringBuilder();

            string sujeto = GenerarSujetoCollection(pTesauro.Collection, pUrlIntragnoss, pTesauro.Source);

            GenerarTriplesCollectionGrafoOntologia(triplesCollectionGrafoOntologia, sujeto, pTesauro.Source);
            GenerarTriplesCollectionGrafoBusqueda(pTesauro.Collection, triplesCollectionGrafoBusqueda, sujeto, pTesauro.Source);
            GenerarScopeNoteMultiIdioma(pTesauro.Collection, triplesCollectionGrafoOntologia, triplesCollectionGrafoBusqueda, sujeto);
            GenerarTriplesRelacionesCollection(pTesauro.Collection, triplesCollectionGrafoOntologia, triplesCollectionGrafoBusqueda, sujeto, pUrlIntragnoss, pTesauro.Source);

            try
            {
                InsertaTripletas(pTesauro.Ontology, triplesCollectionGrafoOntologia.ToString(), 1);
                InsertaTripletas(pProyectoID.ToString(), triplesCollectionGrafoBusqueda.ToString(), 1);
            }
            catch (Exception ex)
            {
                TerminarTransaccion(false);
                throw new Exception($"Ha ocurrido un error al escribir los triples de los Collection en virtuoso.\n {ex.Message}", ex);
            }
        }

        public void ModificarPendienteLeerALeido(string pGrafoID, string pElementoID)
        {
            FacetadoAD.ModificarPendienteLeerALeido(pGrafoID, pElementoID);
        }

        public void ActualizarPublicadoresRecursos(string pUrlIntragnoss, Guid pProyID, Guid pIdentidadID, string pNombrePublicador)
        {
            FacetadoAD.ActualizarPublicadoresRecursos(pUrlIntragnoss, pProyID, pIdentidadID, pNombrePublicador);
        }

        public void ActualizarEditoresRecursos(string pUrlIntragnoss, Guid pProyID, Guid pIdentidadID, string pNombreEditor, List<Guid> pListaRecursos)
        {
            FacetadoAD.ActualizarEditoresRecursos(pUrlIntragnoss, pProyID, pIdentidadID, pNombreEditor, pListaRecursos);
        }

        /// <summary>
        /// Obtiene las tripletas de un sujeto que tiene como objeto uno específico.
        /// </summary>
        /// <param name="pGrafo">Grafo del Tesauro Semántico</param>
        /// <param name="pObjeto">Objeto</param>
        /// <returns>Tripletas de un sujeto que tiene como objeto uno específico</returns>
        public FacetadoDS ObtenerTripletasDeSujetoConObjeto(string pGrafo, string pObjeto)
        {
            return FacetadoAD.ObtenerTripletasDeSujetoConObjeto(pGrafo, pObjeto);
        }

        /// <summary>
        /// Obtiene una lista de sujetos del del tesauro según el grafo seleccionado y source indicado por parámetro.
        /// </summary>
        /// <param name="pGrafo">Grafo del Tesauro Semántico</param>
        /// <param name="pSource">Source</param>
        /// <returns>Lista de sujetos del tesauro segun el grafo y el source indicado</returns>
        public List<string> ObtenerListaSujetosTesauroDeGrafoPorSource(string pGrafo, string pSource)
        {
            return FacetadoAD.ObtenerListaSujetosTesauroDeGrafoPorSource(pGrafo, pSource);
        }


        public List<string> ObtenerListaTriplesDeSujeto(string pGrafo, string pSujeto)
        {
            return FacetadoAD.ObtenerListaTriplesDeSujeto(pGrafo, pSujeto);

        }

        /// <summary>
        /// Obtiene las tripletas con el mismo predicado de un sujeto que tiene como objeto uno, el cúal tiene como objeto uno específico.
        /// </summary>
        /// <param name="pGrafo">Grafo del Tesauro Semántico</param>
        /// <param name="pObjeto">Objeto</param>
        /// <returns>Tripletas con el mismo predicado de un sujeto que tiene como objeto uno, el cúal tiene como objeto uno específico</returns>
        public FacetadoDS ObtenerTripletasMismoPredicadoDeSujetoConObjetoQueTieneUnObjeto(string pGrafo, string pObjeto)
        {
            return FacetadoAD.ObtenerTripletasMismoPredicadoDeSujetoConObjetoQueTieneUnObjeto(pGrafo, pObjeto);
        }

        /// <summary>
        /// Obtiene las tripletas de un sujeto que tiene como objeto uno específico.
        /// </summary>
        /// <param name="pGrafo">Grafo del Tesauro Semántico</param>
        /// <param name="pObjeto">Objeto</param>
        /// <returns>Tripletas de un sujeto que tiene como objeto uno específico</returns>
        public FacetadoDS ObtenerTripletasConObjeto(string pGrafo, string pObjeto)
        {
            return FacetadoAD.ObtenerTripletasConObjeto(pGrafo, pObjeto, null);
        }

        /// <summary>
        /// Obtiene las tripletas de un sujeto que tiene como objeto uno específico de una propiedad.
        /// </summary>
        /// <param name="pGrafo">Grafo del Tesauro Semántico</param>
        /// <param name="pObjeto">Objeto</param>
        /// <param name="pPropiedad">Propiedad</param>
        /// <returns>Tripletas de un sujeto que tiene como objeto uno específico</returns>
        public FacetadoDS ObtenerTripletasConObjetoDePropiedad(string pGrafo, string pObjeto, string pPropiedad)
        {
            return FacetadoAD.ObtenerTripletasConObjeto(pGrafo, pObjeto, pPropiedad);
        }

        /// <summary>
        /// Obtiene los sujetos que tiene como objeto uno específico de una propiedad.
        /// </summary>
        /// <param name="pGrafo">Grafo del Tesauro Semántico</param>
        /// <param name="pObjeto">Objeto</param>
        /// <param name="pPropiedad">Propiedad</param>
        /// <returns>Lista de sujetos que tienen como objetos unos específicos</returns>
        public List<string> ObtenerSujetosConObjetoDePropiedad(string pGrafo, string pObjeto, string pPropiedad)
        {
            FacetadoDS facDS = ObtenerTripletasConObjetoDePropiedad(pGrafo, pObjeto, pPropiedad);
            List<string> suj = new List<string>();

            foreach (DataRow fila in facDS.Tables[0].Rows)
            {
                if (!suj.Contains((string)fila[0]))
                {
                    suj.Add((string)fila[0]);
                }
            }

            facDS.Dispose();
            return suj;
        }

        /// <summary>
        /// Obtiene los sujetos que tienen como objetos unos específicos.
        /// </summary>
        /// <param name="pGrafo">Grafo del Tesauro Semántico</param>
        /// <param name="pObjeto">Objeto</param>
        /// <returns>Lista de sujetos que tienen como objetos unos específicos</returns>
        public List<string> ObtenerSujetosConObjetos(string pGrafo, List<string> pObjetos)
        {
            return FacetadoAD.ObtenerSujetosConObjetos(pGrafo, pObjetos);
        }

        public Dictionary<string, List<string>> ObtenerInformacionOntologias(Guid organizacionID, Guid proyectoID)
        {
            return FacetadoAD.ObtenerInformacionOntologias(organizacionID, proyectoID);
        }

        #endregion
    }
}

