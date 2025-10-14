using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Amigos;
using Es.Riam.Gnoss.AD.BASE_BD.Model;
using Es.Riam.Gnoss.AD.ComparticionAutomatica;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.EntityModelBASE.Models;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Notificacion;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.RabbitMQ;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces;
using Es.Riam.Util;
using Es.Riam.Util.Correo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using static Es.Riam.Gnoss.AD.BASE_BD.Model.BaseComunidadDS;


namespace Es.Riam.Gnoss.AD.BASE_BD
{
    #region Enumeraciones

    public enum TipoEstadoActualizacionContexto
    {
        Pendiente = 0,
        Error = 1,
        Actualizado = 2
    }

    /// <summary>
    /// Prioridad para las colas del Base
    /// </summary>
    public enum PrioridadBase
    {
        /// <summary>
        /// Prioridad Alta.
        /// </summary>
        Alta = 0,
        /// <summary>
        /// Prioridad Media.
        /// </summary>
        Media = 1,
        /// <summary>
        /// Prioridad Baja.
        /// </summary>
        Baja = 2,
        /// <summary>
        /// Prioridad Baja.
        /// </summary>
        MuyBaja = 3,
        /// <summary>
        /// Prioridad del ApiRecursos
        /// </summary>
        ApiRecursos = 11,
        /// <summary>
        /// Prioridad del ApiRecursos para borrar caché
        /// </summary>
        ApiRecursosBorrarCache = 21,
        /// <summary>
        /// Resto de elementos en la cola que tiene una prioridad aún más baja que las anteriores
        /// </summary>
        Resto = 100
    }

    /// <summary>
    /// Estados de los elementos que están en la cola
    /// </summary>
    public enum EstadosColaTags
    {
        /// <summary>
        /// En espera
        /// </summary>
        EnEspera = 0,

        /// <summary>
        /// Ha dado un error al intentar actualizar, se volvera a intentar
        /// </summary>
        PrimerError = 1,

        /// <summary>
        /// Ha dado un segundo error, se intetnará dos veces más
        /// </summary>
        Reintento1 = 2,

        /// <summary>
        /// Tercer error del mismo elemento en la cola, se intetnará una veces más
        /// </summary>
        Reintento2 = 3,

        /// <summary>
        /// Cuarto error del mismo elemento en la cola, se marca como erróneo y no se vuelve a intentar enviar
        /// </summary>
        Erroneo = 4,

        /// <summary>
        /// El elemento se ha procesado correctamente
        /// </summary>
        Procesado = 5,

        /// <summary>
        /// Ha Habido algun error
        /// </summary>
        FalloFreebase = 6,

        /// <summary>
        /// El elemento se ha procesado correctamente
        /// </summary>
        ProcesadoFreebase = 7,

        /// <summary>
        /// El elemento ha fallado al encriptarlo
        /// </summary>
        FalloEncriptacion = 8,

        /// <summary>
        /// El documento se ha encriptado
        /// </summary>
        Encriptado = 9,

        /// <summary>
        /// El elemento se ha descartado.
        /// </summary>
        Descartado = 10
    }

    /// <summary>
    /// Enumeración para los tipos de elementos en las colas
    /// </summary>
    public enum TiposElementosEnCola
    {
        /// <summary>
        /// Se ha agregado o modificado un elemento en una comunidad
        /// </summary>
        Agregado = 0,

        /// <summary>
        /// Se a eliminado un elemento de una comunidad
        /// </summary>
        Eliminado = 1,

        /// <summary>
        /// Se ha eliminado una categoría de una comunidad, hay que eliminar la categoría de virtuoso sin recategorizar
        /// </summary>
        CategoriaEliminadaSinRecategorizar = 2,

        /// <summary>
        /// Se ha eliminado una categoría de una comunidad, hay que eliminar la categoría en virtuoso y 
        /// recategorizar todos los recursos que se encontraban en esa categoría
        /// </summary>
        CategoriaEliminadaRecategorizarTodo = 3,

        /// <summary>
        /// Se ha eliminado una categoría de una comunidad, hay que eliminar la categoría en virtuoso y 
        /// recategorizar todos los recursos que se encontraban en esa categoría y se han quedado huérfanos
        /// </summary>
        CategoriaEliminadaRecategorizarHuerfanos = 4,

        /// <summary>
        /// Se han modificado los niveles de certificación de la comunidad
        /// </summary>
        NivelesCertificacionModificados = 5,

        /// <summary>
        /// Se ha visitado un recurso
        /// </summary>
        VisitaRecurso = 6,

        /// <summary>
        /// La info del recurso ha sido insertada en el grafo de búsqueda desde la Web o el ApiRecursos y hay que generar el search, contribuciones...
        /// </summary>
        InsertadoEnGrafoBusquedaDesdeWeb = 7,


        /// <summary>
        /// Eliminar el autocompletar de un documento concreto
        /// </summary>
        EliminarAutocompletarDocumento = 8,

        /// <summary>
        /// Marca el inicio de una tarea en segundo plano
        /// </summary>
        InicioTarea = 9,

        /// <summary>
        /// Marca el fin de una tarea en segundo plano
        /// </summary>
        FinTarea = 10,

        /// <summary>
        /// Se ha renombrado una categoría
        /// </summary>
        RenombrarCategoria = 11,
    }

    /// <summary>
    /// Tipos de consulta que se pueden realizar a los métodos de ObtenerTags
    /// </summary>
    public enum TiposConsultaObtenerTags
    {
        /// <summary>
        /// Nube de tags de los recursos de una comunidad
        /// </summary>
        RecursosComunidad = 0,

        /// <summary>
        /// Nube de tags de los recursos de una comunidad privada (trae todos los recursos que no son privados en la comunidad más los recursos privados a los que tiene acceso el usuario)
        /// </summary>
        RecursosComunidadPrivada,

        /// <summary>
        /// Nube de tags de los recursos de una comunidad privada, solo trae los recursos privados del usuario
        /// </summary>
        RecursosComunidadSoloPrivados,

        /// <summary>
        /// Nube de tags de las personas y organizaciones de una comunidad
        /// </summary>
        PersonasYOrganizacionesComunidad,

        /// <summary>
        /// Nube de tags de las personas y organizaciones de una comunidad que han decidido ser visibles para usuarios no registrados
        /// </summary>
        PersonasYOrganizacionesVisiblesInvitadoComunidad,

        /// <summary>
        /// Nube de tags de las personas de una organización que participan en una comunidad
        /// </summary>
        PersonasDeOrganizacionParticipanComunidad,

        /// <summary>
        /// Nube de tags de los dafos públicos de una comunidad
        /// </summary>
        DafosDeComunidad,

        /// <summary>
        /// Nube de tags de los dafos de una comunidad para un usuario en concreto
        /// </summary>
        TodosDafosDeComunidadParaUsuario,

        /// <summary>
        /// Nube de tags de los dafos de una comunidad para un usuario en concreto
        /// </summary>
        DafosDeComunidadFiltradosParaUsuario,

        /// <summary>
        /// Nube de tags para el metabuscador
        /// </summary>
        MetaBuscador,

        /// <summary>
        /// Freebase
        /// </summary>
        Freebase,

        /// <summary>
        /// Nube de tags de los blogs de una comunidad y sus entradas
        /// </summary>
        BlogsYEntradasBlogDeComunidad,

        /// <summary>
        /// Nube de tags de las comunidades de MyGnoss
        /// </summary>
        Comunidades
    }

    /// <summary>
    /// Tipos de eventos que refrescan caché
    /// </summary>
    public enum TiposEventosRefrescoCache
    {
        /// <summary>
        /// Refrescar una búsqueda en virtuoso
        /// </summary>
        BusquedaVirtuoso = 0,
        /// <summary>
        /// Refrescar un componente que ha cambiado
        /// </summary>
        ConfiguracionComponentesCambiada = 1,
        /// <summary>
        /// Refrescar los componentes que tengan de caducidad publicacion de recurso
        /// </summary>
        RefrescarComponentesRecursos = 2,
        /// <summary>
        /// Actualizar las cachés de los destinatarios y del remitente.
        /// </summary>
        CambiosBandejaDeMensajes = 3,
        /// <summary>
        /// Modificar la caducidad de la cache de un documento.
        /// </summary>
        ModificarCaducidadCache = 4,
        /// <summary>
        /// Modificar la caducidad de la cache de un documento.
        /// </summary>
        RecalcularContadoresPerfil = 5,
        /// <summary>
        /// Actualizar las cachés de lista de amigos
        /// </summary>
        CambiosAmigosPrivados = 6

    }

    /// <summary>
    /// Valores del campo origen de autocompletar.
    /// </summary>
    public class OrigenAutocompletar
    {
        /// <summary>
        /// Texto para recursos.
        /// </summary>
        public const string Recursos = "Recursos";

        /// <summary>
        /// Texto para debates.
        /// </summary>
        public const string Debates = "Debates";

        /// <summary>
        /// Texto para preguntas.
        /// </summary>
        public const string Preguntas = "Preguntas";

        /// <summary>
        /// Texto para encuestas.
        /// </summary>
        public const string Encuestas = "Encuestas";

        /// <summary>
        /// Texto para personas y organizaciones.
        /// </summary>
        public const string PersyOrg = "PerYOrg";
    }

    #endregion

    public class JoinColaCorreoColaCorreoDestinatario
    {
        public ColaCorreo ColaCorreo { get; set; }
        public ColaCorreoDestinatario ColaCorreoDestinatario { get; set; }
    }

    public static class Joins
    {
        public static IQueryable<JoinColaCorreoColaCorreoDestinatario> JoinColaCorreoDestinatario(this IQueryable<ColaCorreo> pQuery)
        {
            EntityContextBASE entityContext = (EntityContextBASE)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.ColaCorreoDestinatario, colaCorreo => colaCorreo.CorreoID, colaCorreoDestinatario => colaCorreoDestinatario.CorreoID, (colaCorreo, colaCorreoDestinatario) => new JoinColaCorreoColaCorreoDestinatario
            {
                ColaCorreoDestinatario = colaCorreoDestinatario,
                ColaCorreo = colaCorreo
            });
        }
    }


    /// <summary>
    /// Clase base para los AD del modelo BASE
    /// </summary>
    public class BaseComunidadAD : BaseAD
    {
        #region Constantes

        /// <summary>
        ///Constante para codificar los tags de tipo Nivel de certificacionde documento
        /// </summary>
        public const string SELECT_CONSULTA_TAGS = " SELECT '' Tag1, Tag2, Sum(CercaniaDirecta) + Sum(CercaniaIndirecta) CercaniaDirecta";

        /// <summary>
        ///Constante para codificar los tags de tipo Nivel de certificacionde documento
        /// </summary>
        public const string GROUP_BY_CONSULTA_TAGS = " GROUP BY Tag2 ";

        /// <summary>
        ///Constante para codificar los tags de tipo Nivel de certificacionde documento
        /// </summary>
        public const string ORDER_BY_CONSULTA_TAGS = " ORDER BY CercaniaDirecta DESC ";

        /// <summary>
        /// Límite para las nubes de tag.
        /// </summary>
        public const int LIMITE_NUBE = 20;

        /// <summary>
        ///Constante para codificar el origen del tag, de comentario o recurso
        /// </summary>
        public const string COM_U_REC = "##COM-REC##";

        #endregion

        #region Miembros

        /// <summary>
        /// Identificador numerico del proyecto
        /// </summary>
        protected int mTablaBaseProyectoID;

        /// <summary>
        /// Indica si la comunidad sobre la que se hacen las búsquedas es privada
        /// </summary>
        private bool mEsComunidadPrivada = false;

        /// <summary>
        /// Indica si el usuario que está realizando la consulta ha hecho login.
        /// </summary>
        private bool mEstaUsuarioConectado = false;

        /// <summary>
        /// Identificador de la identidad del usuario actual conectado
        /// </summary>
        private Guid mIdentidadUsuarioConectado;

        /// <summary>
        /// Identificador de la tabla freebase
        /// </summary>
        protected string mNomberTablaFreebase;

        /// <summary>
        /// Nombre real de la tabla que deberemos consultar para los tags de los recursos de una coomunidad pubñica en funcion del identificador numerico del proyecto
        /// </summary>
        protected string mNombreTablaCOMUNIDADES;

        /// <summary>
        /// Nombre real de la tabla que deberemos consultar para los tags de los recursos privados por usuario de una coomunidad en funcion del identificador numerico del proyecto
        /// </summary>
        protected string mNombreTablaCOM_USU_PRIV;

        /// <summary>
        /// Nombre real de la tabla que deberemos consultar para los tags de los recursos privados por usuario de una coomunidad en funcion del identificador numerico del proyecto
        /// </summary>
        protected string mNombreTablaREC_BR_PER_PUBLICOS;

        /// <summary>
        /// Nombre real de la tabla que deberemos consultar para los tags de los recursos privados por usuario de una coomunidad en funcion del identificador numerico del proyecto
        /// </summary>
        protected string mNombreTablaREC_PRIVADOS_COM_USU;

        /// <summary>
        /// Mombre real de la tabla que deberemos consultar para los tags de los recursos privados por usuario de una coomunidad en funcion del identificador numerico del proyecto
        /// </summary>
        protected string mNombreTablaREC_PUBLICOS_MYGNOSS;

        /// <summary>
        /// Nombre real de la tabla que deberemos consultar para los tags de los DAFO de una comunidad a la que tienen acceso todos los usuarios en funcion del identificador numerico del proyecto
        /// </summary>
        protected string mNombreTablaDAFO_COM;

        /// <summary>
        /// Nombre real de la tabla que deberemos consultar para los tags de los DAFO en los que participa un usuario de una comunidad y que son accesibles por todos los usuarios en funcion del identificador numerico del proyecto
        /// </summary>
        protected string mNombreTablaDAFO_COM_USU;

        /// <summary>
        /// Nombre real de la tabla que deberemos consultar para los tags de los DAFO en los que participa un usuario de una comunidad y que su visibilidad es sólo para participantes en funcion del identificador numerico del proyecto
        /// </summary>
        protected string mNombreTablaDAFO_COM_USU_PRIV;

        /// <summary>
        /// Nombre real de la tabla que deberemos consultar para los tags de los blogs de una coomunidad en funcion del identificador numerico del proyecto
        /// </summary>
        protected string mNombreTablaCOM_BLOG;

        /// <summary>
        /// Nombre real de la tabla que deberemos consultar para los tags de las personas y organizaciones de una comunidad (en funcion del identificador numerico del proyecto) que participen en el
        /// </summary>
        protected string mNombreTablaCOM_PER_ORG;

        /// <summary>
        /// Nombre real de la tabla que deberemos consultar para los tags de las personas y organizaciones de una comunidad (en funcion del identificador numerico del proyecto) que han decidido ser buscables para externos.
        /// </summary>
        protected string mNombreTablaCOM_PER_ORG_VI;

        /// <summary>
        /// Nombre real de la tabla que deberemos consultar para los tags de las personas y organizaciones de una organizacion que participa en una determinada comunidad (en funcion del identificador numerico del proyecto y el identificador numerico de la organización)
        /// </summary>
        protected string mNombreTablaCOM_X_ORG_X;

        /// <summary>
        /// Almacena el campo IdentidadID de las tablas que tienen este campo (simpre es IdentidadID menos ne MyGnoss que es UsuarioID)
        /// </summary>
        private string mCampoIdentidadID = "IdentidadID";

        private bool mBusquedaEnMyGnoss = false;

        protected bool mUsarRabbitSiEstaConfigurado = true;

        protected EntityContextBASE mEntityContextBASE;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public BaseComunidadAD(LoggingService loggingService, EntityContext entityContext, EntityContextBASE entityContextBASE, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication,ILogger<BaseComunidadAD> logger, ILoggerFactory loggerFactory)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mEntityContextBASE = entityContextBASE;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            mNomberTablaFreebase = "GnossToFreebase";
            if (mTablaBaseProyectoID > -1)
            {
                mNomberTablaFreebase = "GnossToFreebase0000000000000" + mTablaBaseProyectoID;
            }
            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración de la base de datos BASE</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        /// <param name="pTablaBaseProyectoID">Identificador numerico del proyecto (-1 si no se va a actualizar tablas de proyecto)</param>
        public BaseComunidadAD(int pTablaBaseProyectoID, LoggingService loggingService, EntityContext entityContext, EntityContextBASE entityContextBASE, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<BaseComunidadAD> logger, ILoggerFactory loggerFactory)
            : base(loggingService, entityContext, configService, entityContextBASE, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mEntityContextBASE = entityContextBASE;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            mTablaBaseProyectoID = pTablaBaseProyectoID;
            mNomberTablaFreebase = "GnossToFreebase";
            if (mTablaBaseProyectoID > -1)
            {
                mNomberTablaFreebase = "GnossToFreebase0000000000000" + mTablaBaseProyectoID;
            }
            this.CargarConsultasYDataAdapters(IBD);
        }


        #endregion

        #region Consultas

        private string sqlSelectGnossToFreebase;
        private string sqlColaComparticionAutomaticaInsert;
        private string sqlColaModificacionSearchInsert;
        //private string sqlSelectColaCorreo;
        private string sqlSelectColaCorreoDestinatario;

        #endregion

        #region DataAdapter

        #region Metodos generales

        #region GnossToFreebase
        private string sqlGnossToFreebaseInsert;
        private string sqlGnossToFreebaseDelete;
        private string sqlGnossToFreebaseModify;
        #endregion

        #region ColaCorreo
        private string sqlColaCorreoInsert;
        private string sqlColaCorreoDelete;
        private string sqlColaCorreoModify;
        #endregion

        #region ColaCorreoDestinatario
        private string sqlColaCorreoDestinatarioInsert;
        private string sqlColaCorreoDestinatarioDelete;
        private string sqlColaCorreoDestinatarioModify;
        #endregion

        #region Metodos AD


        public void InsertarFilasEnRabbit(string pColaRabbit, DataSet pDataSet, string pNombreTabla = null)
        {
            if (mUsarRabbitSiEstaConfigurado)
            {
                if (string.IsNullOrEmpty(pNombreTabla))
                {
                    pNombreTabla = pColaRabbit;
                }

                DataRowCollection filas = pDataSet.Tables[pNombreTabla].GetChanges(DataRowState.Added)?.Rows;

                if (filas != null)
                {
                    string rabbitBD = RabbitMQClient.BD_SERVICIOS_WIN;
                    string exchange = "";

                    if (!string.IsNullOrWhiteSpace(mConfigService.ObtenerRabbitMQClient(rabbitBD)))
                    {
                        try
                        {
                            using (RabbitMQClient rMQ = new RabbitMQClient(rabbitBD, pColaRabbit, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<RabbitMQClient>(), mLoggerFactory))
                            {
                                List<string> elementos = new List<string>();
                                foreach (DataRow fila in filas)
                                {
                                    elementos.Add(JsonConvert.SerializeObject(fila.ItemArray));
                                }
                                rMQ.AgregarElementosACola(elementos);
                                pDataSet.Tables[pNombreTabla].AcceptChanges();
                            }
                        }
                        catch (Exception ex)
                        {
                            mLoggingService.GuardarLogError(ex, mlogger);
                        }
                    }
                }
            }
        }

        public virtual void ActualizarBD(DataSet pDataSet, bool pUsarRabbitSiEstaConfigurado = false)
        {
            mUsarRabbitSiEstaConfigurado = pUsarRabbitSiEstaConfigurado;

            EliminarBorrados(pDataSet);
            GuardarActualizaciones(pDataSet);
        }
        public virtual void EliminarBorrados(DataSet pDataSet)
        {
            try
            {

                DataSet deletedDataSet;
                deletedDataSet = pDataSet.GetChanges(DataRowState.Deleted);
                if (deletedDataSet != null)
                {

                    #region Deleted
                    #region Eliminar tabla GnossToFreebase
                    DbCommand DeleteGnossToFreebaseCommand = ObtenerComando(sqlGnossToFreebaseDelete);
                    AgregarParametro(DeleteGnossToFreebaseCommand, IBD.ToParam("Original_Tag"), DbType.String, "Tag", DataRowVersion.Original);
                    AgregarParametro(DeleteGnossToFreebaseCommand, IBD.ToParam("Original_GUIDFreebase"), DbType.String, "GUIDFreebase", DataRowVersion.Original);
                    AgregarParametro(DeleteGnossToFreebaseCommand, IBD.ToParam("Original_Coincidencia"), DbType.Int64, "Coincidencia", DataRowVersion.Original);
                    AgregarParametro(DeleteGnossToFreebaseCommand, IBD.ToParam("Original_Entidad"), DbType.String, "Entidad", DataRowVersion.Original);
                    AgregarParametro(DeleteGnossToFreebaseCommand, IBD.ToParam("Original_Ruta"), DbType.String, "Ruta", DataRowVersion.Original);
                    AgregarParametro(DeleteGnossToFreebaseCommand, IBD.ToParam("Original_WikipediaID"), DbType.String, "WikipediaID", DataRowVersion.Original);
                    AgregarParametro(DeleteGnossToFreebaseCommand, IBD.ToParam("Original_Tipos"), DbType.String, "Tipos", DataRowVersion.Original);
                    AgregarParametro(DeleteGnossToFreebaseCommand, IBD.ToParam("Original_Descripcion"), DbType.String, "Descripcion", DataRowVersion.Original);
                    AgregarParametro(DeleteGnossToFreebaseCommand, IBD.ToParam("Original_RutaNYT"), DbType.String, "RutaNYT", DataRowVersion.Original);
                    AgregarParametro(DeleteGnossToFreebaseCommand, IBD.ToParam("Original_RutaDbpedia"), DbType.String, "RutaDbpedia", DataRowVersion.Original);
                    AgregarParametro(DeleteGnossToFreebaseCommand, IBD.ToParam("Original_RutaGeonames"), DbType.String, "RutaGeonames", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "GnossToFreebase", null, null, DeleteGnossToFreebaseCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #endregion

                    deletedDataSet.Dispose();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual void GuardarActualizaciones(DataSet pDataSet)
        {
            try
            {
                DataSet addedAndModifiedDataSet;
                addedAndModifiedDataSet = pDataSet.GetChanges(DataRowState.Added | DataRowState.Modified);
                if (addedAndModifiedDataSet != null)
                {

                    #region AddedAndModified

                    #region Actualizar Tabla ColaComparticionAutomatica

                    DbCommand InsertColaComparticionAutomaticaCommand = ObtenerComando(sqlColaComparticionAutomaticaInsert);
                    AgregarParametro(InsertColaComparticionAutomaticaCommand, IBD.ToParam("OrdenEjecucion"), IBD.TipoGuidToObject(DbType.Guid), "OrdenEjecucion", DataRowVersion.Current);
                    AgregarParametro(InsertColaComparticionAutomaticaCommand, IBD.ToParam("ID"), IBD.TipoGuidToObject(DbType.Guid), "ID", DataRowVersion.Current);
                    AgregarParametro(InsertColaComparticionAutomaticaCommand, IBD.ToParam("Tipo"), IBD.TipoGuidToObject(DbType.Int16), "Tipo", DataRowVersion.Current);
                    AgregarParametro(InsertColaComparticionAutomaticaCommand, IBD.ToParam("Estado"), IBD.TipoGuidToObject(DbType.Int16), "Estado", DataRowVersion.Current);
                    AgregarParametro(InsertColaComparticionAutomaticaCommand, IBD.ToParam("Fecha"), IBD.TipoGuidToObject(DbType.DateTime), "Fecha", DataRowVersion.Current);
                    AgregarParametro(InsertColaComparticionAutomaticaCommand, IBD.ToParam("Prioridad"), IBD.TipoGuidToObject(DbType.Int16), "Prioridad", DataRowVersion.Current);
                    AgregarParametro(InsertColaComparticionAutomaticaCommand, IBD.ToParam("IncidenciaComparticion"), IBD.TipoGuidToObject(DbType.String), "IncidenciaComparticion", DataRowVersion.Current);

					InsertarFilasEnRabbit("ColaComparticionAutomatica", addedAndModifiedDataSet);              
                    //ActualizarBaseDeDatos(addedAndModifiedDataSet, "ColaComparticionAutomatica", InsertColaComparticionAutomaticaCommand, null, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion Actualizar Tabla ColaComparticionAutomatica

                    #region Actualizar Tabla ColaModificacionSearch

                    DbCommand InsertColaModificacionSearchCommand = ObtenerComando(sqlColaModificacionSearchInsert);
                    //AgregarParametro(InsertColaModificacionSearchCommand, IBD.ToParam("OrdenEjecucion"), DbType.Int32, "OrdenEjecucion", DataRowVersion.Current);
                    AgregarParametro(InsertColaModificacionSearchCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertColaModificacionSearchCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(InsertColaModificacionSearchCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(InsertColaModificacionSearchCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);
                    AgregarParametro(InsertColaModificacionSearchCommand, IBD.ToParam("FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Current);
                    AgregarParametro(InsertColaModificacionSearchCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);

                    //ActualizarBaseDeDatos(addedAndModifiedDataSet, "ColaModificacionSearch", InsertColaModificacionSearchCommand, null, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion Tabla ColaModificacionSearch

                    #endregion

                    addedAndModifiedDataSet.Dispose();
                }
            }
            catch (Exception)
            {
                throw;
            }
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
        /// <param name="pIBD">Objecto IBaseDatos para el archivo pasado al constructor del AD</param>
        private void CargarConsultasYDataAdapters(IBaseDatos pIBD)
        {
            //TODO: poner consultas comunes
            #region Consultas

            this.sqlSelectGnossToFreebase = "SELECT " + mNomberTablaFreebase + ".Tag, " + mNomberTablaFreebase + ".GUIDFreebase, " + mNomberTablaFreebase + ".Coincidencia, " + mNomberTablaFreebase + ".Entidad, " + mNomberTablaFreebase + ".Ruta, " + mNomberTablaFreebase + ".WikipediaID, " + mNomberTablaFreebase + ".Tipos, " + mNomberTablaFreebase + ".Descripcion, " + mNomberTablaFreebase + ".RutaNYT, " + mNomberTablaFreebase + ".RutaDbpedia, " + mNomberTablaFreebase + ".RutaGeonames FROM " + mNomberTablaFreebase + " ";

            /*            this.sqlSelectColaCorreo = "SELECT CorreoID, Remitente, Asunto, HtmlTexto, EsHtml, Prioridad, FechaPuestaEnCola, MascaraRemitente, DireccionRespuesta, MascaraDireccionRespuesta, SMTP, Usuario, Password, Puerto, EsSeguro, tipo FROM ColaCorreo "*/
            ;

            sqlSelectColaCorreoDestinatario = "SELECT CorreoID, Email, MascaraDestinatario, Estado, FechaProcesado FROM ColaCorreoDestinatario ";

            #endregion

            #region DataAdapter

            #region GnossToFreebase
            this.sqlGnossToFreebaseInsert = IBD.ReplaceParam("INSERT INTO " + mNomberTablaFreebase + " (Tag, GUIDFreebase, Coincidencia, Entidad, Ruta, WikipediaID, Tipos, Descripcion, RutaNYT, RutaDbpedia, RutaGeonames) VALUES (@Tag, @GUIDFreebase, @Coincidencia, @Entidad, @Ruta, @WikipediaID, @Tipos, @Descripcion, @RutaNYT, @RutaDbpedia, @RutaGeonames)");
            this.sqlGnossToFreebaseDelete = IBD.ReplaceParam("DELETE FROM " + mNomberTablaFreebase + " WHERE (Tag = @Original_Tag OR @Original_Tag IS NULL AND Tag IS NULL) AND (GUIDFreebase = @Original_GUIDFreebase OR @Original_GUIDFreebase IS NULL AND GUIDFreebase IS NULL) AND (Coincidencia = @Original_Coincidencia OR @Original_Coincidencia IS NULL AND Coincidencia IS NULL) AND (Entidad = @Original_Entidad OR @Original_Entidad IS NULL AND Entidad IS NULL) AND (Ruta = @Original_Ruta OR @Original_Ruta IS NULL AND Ruta IS NULL) AND (WikipediaID = @Original_WikipediaID OR @Original_WikipediaID IS NULL AND WikipediaID IS NULL) AND (Tipos = @Original_Tipos OR @Original_Tipos IS NULL AND Tipos IS NULL) AND (Descripcion = @Original_Descripcion OR @Original_Descripcion IS NULL AND Descripcion IS NULL) AND (RutaNYT = @Original_RutaNYT OR @Original_RutaNYT IS NULL AND RutaNYT IS NULL) AND (RutaDbpedia = @Original_RutaDbpedia OR @Original_RutaDbpedia IS NULL AND RutaDbpedia IS NULL) AND (RutaGeonames = @Original_RutaGeonames OR @Original_RutaGeonames IS NULL AND RutaGeonames IS NULL)");
            this.sqlGnossToFreebaseModify = IBD.ReplaceParam("UPDATE " + mNomberTablaFreebase + " SET Tag = @Tag, GUIDFreebase = @GUIDFreebase, Coincidencia = @Coincidencia, Entidad = @Entidad, Ruta = @Ruta, WikipediaID = @WikipediaID, Tipos = @Tipos, Descripcion = @Descripcion, RutaNYT = @RutaNYT, RutaDbpedia = @RutaDbpedia, RutaGeonames = @RutaGeonames WHERE (Tag = @Original_Tag OR @Original_Tag IS NULL AND Tag IS NULL) AND (GUIDFreebase = @Original_GUIDFreebase OR @Original_GUIDFreebase IS NULL AND GUIDFreebase IS NULL) AND (Coincidencia = @Original_Coincidencia OR @Original_Coincidencia IS NULL AND Coincidencia IS NULL) AND (Entidad = @Original_Entidad OR @Original_Entidad IS NULL AND Entidad IS NULL) AND (Ruta = @Original_Ruta OR @Original_Ruta IS NULL AND Ruta IS NULL) AND (WikipediaID = @Original_WikipediaID OR @Original_WikipediaID IS NULL AND WikipediaID IS NULL) AND (Tipos = @Original_Tipos OR @Original_Tipos IS NULL AND Tipos IS NULL) AND (Descripcion = @Original_Descripcion OR @Original_Descripcion IS NULL AND Descripcion IS NULL) AND (RutaNYT = @Original_RutaNYT OR @Original_RutaNYT IS NULL AND RutaNYT IS NULL) AND (RutaDbpedia = @Original_RutaDbpedia OR @Original_RutaDbpedia IS NULL AND RutaDbpedia IS NULL) AND (RutaGeonames = @Original_RutaGeonames OR @Original_RutaGeonames IS NULL AND RutaGeonames IS NULL)");
            #endregion

            #region ColaComparticionAutomatica

            this.sqlColaComparticionAutomaticaInsert = IBD.ReplaceParam("INSERT INTO ColaComparticionAutomatica (OrdenEjecucion, ID, Tipo, Estado, Fecha, Prioridad, IncidenciaComparticion) VALUES (" + IBD.GuidParamColumnaTabla("OrdenEjecucion") + ", " + IBD.GuidParamColumnaTabla("ID") + ", @Tipo, @Estado, @Fecha, @Prioridad, @IncidenciaComparticion)");

            #endregion ColaComparticionAutomatica

            #region ColaModificacionSearch

            this.sqlColaModificacionSearchInsert = IBD.ReplaceParam("INSERT INTO ColaModificacionSearch (ProyectoID, Tipo, Estado, Prioridad, FechaPuestaEnCola, FechaProcesado) VALUES (" + IBD.GuidParamColumnaTabla("ProyectoID") + ", @Tipo, @Estado, @Prioridad, @FechaPuestaEnCola, @FechaProcesado)");

            #endregion ColaModificacionSearch

            #region ColaCorreo
            sqlColaCorreoInsert = IBD.ReplaceParam("INSERT INTO ColaCorreo (Remitente, Asunto, HtmlTexto, EsHtml, Prioridad, FechaPuestaEnCola, MascaraRemitente, DireccionRespuesta, MascaraDireccionRespuesta, SMTP, Usuario, Password, Puerto, EsSeguro, tipo) VALUES (@Remitente, @Asunto, @HtmlTexto, @EsHtml, @Prioridad, @FechaPuestaEnCola, @MascaraRemitente, @DireccionRespuesta, @MascaraDireccionRespuesta, @SMTP, @Usuario, @Password, @Puerto, @EsSeguro, @tipo)");

            sqlColaCorreoDelete = IBD.ReplaceParam("DELETE FROM ColaCorreo WHERE CorreoID = @O_CorreoID ");

            sqlColaCorreoModify = IBD.ReplaceParam("UPDATE ColaCorreo SET Remitente=@Remitente, Asunto=@Asunto, HtmlTexto=@HtmlTexto, EsHtml=@EsHtml, Prioridad=@Prioridad, FechaPuestaEnCola=@FechaPuestaEnCola, MascaraRemitente=@MascaraRemitente, DireccionRespuesta=@DireccionRespuesta, MascaraDireccionRespuesta=@MascaraDireccionRespuesta, SMTP=@SMTP, Usuario=@Usuario, Password=@Password, Puerto=@Puerto, EsSeguro=@EsSeguro, tipo=@tipo WHERE (CorreoID=@O_CorreoID)");
            #endregion

            #region ColaCorreoDestinatario
            sqlColaCorreoDestinatarioInsert = IBD.ReplaceParam("INSERT INTO ColaCorreoDestinatario (CorreoID, Email, MascaraDestinatario, Estado) VALUES (@CorreoID, @Email, @MascaraDestinatario, @Estado)");

            sqlColaCorreoDestinatarioDelete = IBD.ReplaceParam("DELETE FROM ColaCorreoDestinatario WHERE (CorreoID = @O_CorreoID AND Email = @O_Email)");

            sqlColaCorreoDestinatarioModify = IBD.ReplaceParam("UPDATE ColaCorreoDestinatario SET CorreoID=@CorreoID, Email=@Email, MascaraDestinatario=@MascaraDestinatario, Estado=@Estado, FechaProcesado=@FechaProcesado WHERE (CorreoID=@O_CorreoID AND Email = @O_Email)");
            #endregion

            #endregion
        }

        /// <summary>
        /// Genera un script con las inserciones y actualizaciones necesarias para actualizar la base de datos
        /// </summary>
        /// <param name="pDataSet">DataSet con los cambios</param>
        /// <returns>Devuelve una cadena con los errores ocurridos</returns>
        public string GuardarActualizacionInsercionMedianteScript(DataSet pDataSet, bool pLanzarExcepciones)
        {
            string errores = "";

            pDataSet = pDataSet.GetChanges();
            int numParamInsert = 0;
            int numParamUpdate = 0;
            if (pDataSet != null)
            {
                foreach (DataTable tabla in pDataSet.Tables)
                {
                    string nombreTabla = ObtenerNombreCorrectoTabla(tabla.TableName);
                    if (tabla.Rows.Count > 0)
                    {
                        foreach (DataRow fila in tabla.Rows)
                        {
                            try
                            {

                                DbCommand comandoInsert = ObtenerComando("SELECT *");
                                DbCommand comandoUpdate = ObtenerComando("SELECT *");
                                comandoInsert.CommandText = "";
                                comandoUpdate.CommandText = "";

                                if (fila.RowState.Equals(DataRowState.Added))
                                {
                                    //genero insert
                                    comandoInsert.CommandText += System.Environment.NewLine + "INSERT INTO " + nombreTabla + " ";
                                    string columnas = "(";
                                    string coma = "";
                                    string valores = " VALUES (";
                                    foreach (DataColumn columna in tabla.Columns)
                                    {
                                        if (!columna.AutoIncrement)
                                        {
                                            columnas += coma + columna.ColumnName;
                                            if ((columna.DataType.Equals(typeof(string))) || (columna.DataType.Equals(typeof(Guid))))
                                            {
                                                //con parámetro
                                                string param = IBD.ToParam("columna" + numParamInsert++);
                                                valores += coma + param;
                                                AgregarParametro(comandoInsert, param, TypeToDbType(columna.DataType), fila[columna]);
                                            }
                                            else
                                            {
                                                //no hace falta parámetro
                                                valores += coma + fila[columna];
                                            }
                                            coma = ", ";
                                        }
                                    }

                                    comandoInsert.CommandText += columnas + ")" + System.Environment.NewLine + valores + ")";

                                    ActualizarBaseDeDatos(comandoInsert);
                                }
                                else if (fila.RowState.Equals(DataRowState.Modified))
                                {
                                    //genero update
                                    comandoUpdate.CommandText += System.Environment.NewLine + "UPDATE " + nombreTabla + System.Environment.NewLine;
                                    string where = " WHERE ";
                                    string coma = "";
                                    string and = "";
                                    string valores = " SET ";
                                    foreach (DataColumn columna in tabla.Columns)
                                    {
                                        where += and + columna.ColumnName;
                                        if (!columna.AutoIncrement)
                                        {
                                            valores += coma + columna.ColumnName + " = ";
                                            coma = ", ";
                                        }
                                        if ((columna.DataType.Equals(typeof(string))) || (columna.DataType.Equals(typeof(Guid))) || (columna.DataType.Equals(typeof(DateTime))) || (columna.DataType.Equals(typeof(bool))))
                                        {
                                            //con parámetro
                                            string param = IBD.ToParam("columna" + numParamUpdate++);

                                            if (!columna.AutoIncrement)
                                            {
                                                valores += param;
                                                AgregarParametro(comandoUpdate, param, TypeToDbType(columna.DataType), fila[columna]);
                                            }

                                            if (fila.IsNull(columna, DataRowVersion.Original))
                                            {
                                                where += " IS NULL ";
                                            }
                                            else
                                            {
                                                where += " = " + param + "O";
                                                AgregarParametro(comandoUpdate, param + "O", TypeToDbType(columna.DataType), fila[columna, DataRowVersion.Original]);
                                            }

                                        }
                                        else
                                        {
                                            //no hace falta parámetro
                                            if (!columna.AutoIncrement)
                                            {
                                                valores += fila[columna];
                                            }
                                            if (fila.IsNull(columna, DataRowVersion.Original))
                                            {
                                                where += " IS NULL ";
                                            }
                                            else
                                            {
                                                where += " = " + fila[columna, DataRowVersion.Original];
                                            }
                                        }
                                        and = " AND ";
                                    }

                                    comandoUpdate.CommandText += valores + System.Environment.NewLine + where;

                                    ActualizarBaseDeDatos(comandoUpdate);
                                }
                            }
                            catch (Exception e)
                            {
                                errores += System.Environment.NewLine + e.Message + System.Environment.NewLine;
                            }
                        }
                    }
                }
            }

            return errores;
        }

        private DbType TypeToDbType(Type pTipo)
        {
            if (pTipo.Equals(typeof(string)))
            {
                return DbType.String;
            }
            else if (pTipo.Equals(typeof(Guid)))
            {
                return IBD.TipoGuidToString(DbType.Guid);
            }
            else if (pTipo.Equals(typeof(DateTime)))
            {
                return IBD.TipoGuidToString(DbType.DateTime);
            }

            return DbType.Int32;
        }

        /// <summary>
        /// Obtiene el nombre correcto de la tabla en la base de datos
        /// </summary>
        /// <param name="pNombreTabla">Nombre de la tabla en el DataSet</param>
        /// <returns></returns>
        protected virtual string ObtenerNombreCorrectoTabla(string pNombreTabla)
        {
            if (pNombreTabla.Equals("GnossToFreebase"))
            {
                if (mTablaBaseProyectoID > -1)
                {
                    return "GnossToFreebase0000000000000" + mTablaBaseProyectoID;
                }
            }
            return pNombreTabla;
        }

        #endregion

        #region Metodos de consultas

        public int ObtenerListaRecursosRelacionados(DataSet pDataSet, String Recurso, int inicio, int final, Guid pProyectoId)
        {
            string strSQL = "Select tag2, COUNT(tag1) as similitud from " + mNombreTablaCOMUNIDADES + " where Tag1 in (select tag1 from " + mNombreTablaCOMUNIDADES + " where Tag2 = " + IBD.ToParam("Recurso") + " and (tipo = 10000 or Tipo = 10038)) and (tipo = 10000 or Tipo = 10038) group by tag2, Tipo ";

            //string strSQL = "select tag1 from " + mNombreTablaCOMUNIDADES +" where Tag2 = " + IBD.ToParam("Recurso") + " and (tipo = 10000 or Tipo = 10038)";


            DbCommand cmdObtenerGrafoTagsCom = ObtenerComando(strSQL);
            AgregarParametro(cmdObtenerGrafoTagsCom, IBD.ToParam("Recurso"), DbType.String, Recurso);
            // IDataReader iDataReader=EjecutarReader(cmdObtenerGrafoTagsCom);
            // while (iDataReader.NextResult) {
            //     iDataReader.GetString(0);
            // }

            // CargarDataSet(cmdObtenerGrafoTagsCom, pDataSet, "GnossToFreebase");
            return PaginarDataSet(cmdObtenerGrafoTagsCom, "order by similitud desc", pDataSet, inicio, final, "Tabla1");
            //return PaginarDataSet(cmdObtenerGrafoTagsCom, "order by tag1 desc", pDataSet, inicio, final, "Tabla1");
            //if (pDataSet.Tables["Tabla1"].Rows.Count < 15) { }
        }


        public int ObtenerListaRecursosRelacionadosOtrasComunidades(DataSet pDataSet, String Recurso, int inicio, int final, Guid pProyectoId)
        {
            ProyectoAD proyectoAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication,mLoggerFactory.CreateLogger<ProyectoAD>(),mLoggerFactory);
            List<int> proyectosRelacionados = new List<int>();
            proyectoAD.ObtenerListaProyectoRelacionados(pProyectoId, out proyectosRelacionados);

            if (proyectosRelacionados.Count > 0)
            {
                string strSQL = "";
                foreach (int proyectoId in proyectosRelacionados)
                {
                    strSQL = strSQL + "Select tag2, COUNT(tag1) as similitud from " + mNombreTablaCOMUNIDADES + " where Tag1 in (select tag1 from " + mNombreTablaCOMUNIDADES + " where Tag2 = " + IBD.ToParam("Recurso") + " and (tipo = 10000 or Tipo = 10038)) and (tipo = 10000 or Tipo = 10038) group by tag2, Tipo UNION ";


                }
                strSQL = strSQL.Substring(0, strSQL.Length - 6);

                DbCommand cmdObtenerGrafoTagsCom = ObtenerComando(strSQL);
                AgregarParametro(cmdObtenerGrafoTagsCom, IBD.ToParam("Recurso"), DbType.String, Recurso);
                return PaginarDataSet(cmdObtenerGrafoTagsCom, "order by similitud desc", pDataSet, inicio, final, "Tabla1");
            }
            return 0;
        }

        #region Ultimos recursos visitados

        /// <summary>
        /// Obtiene los últimos recursos visitados de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto del que se quieren obtener los útlimos documentos visitados</param>
        /// <returns>Lista de Identificadores de los últimos documentos visitados</returns>
        public List<Guid> ObtenerUltimosRecursosVisitadosDeProyecto(Guid pProyectoID)
        {
            List<Guid> listaDocumentos = null;
            string documentos = mEntityContext.UltimosDocumentosVisitados.Where(item => item.ProyectoID.Equals(pProyectoID)).Select(item => item.Documentos).FirstOrDefault();

            if (!string.IsNullOrEmpty(documentos))
            {
                listaDocumentos = new List<Guid>(Array.ConvertAll(documentos.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), x => new Guid(x)));
            }

            return listaDocumentos;
        }

        /// <summary>
        /// Actualiza los útlimos recursos visitados de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pListaDocumentosID">Lista de los últimos recursos visitados en este proyecto</param>
        public void InsertarUltimosRecursosVisitadosDeProyecto(Guid pProyectoID, List<Guid> pListaDocumentosID)
        {


            string ultimosRecursosVisitadosActualizado = string.Join(",", pListaDocumentosID.ConvertAll(x => x.ToString()));
            UltimosDocumentosVisitados ultimosDocumentosVisitados = new UltimosDocumentosVisitados()
            {
                Documentos = ultimosRecursosVisitadosActualizado,
                FechaActualizacion = DateTime.Now,
                ProyectoID = pProyectoID
            };
            mEntityContext.UltimosDocumentosVisitados.Add(ultimosDocumentosVisitados);
            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Actualiza los útlimos recursos visitados de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pListaDocumentosID">Lista de los últimos recursos visitados en este proyecto</param>
        public void ActualizarUltimosRecursosVisitadosDeProyecto(Guid pProyectoID, List<Guid> pListaDocumentosID)
        {
            string ultimosRecursosVisitadosActualizado = string.Join(",", pListaDocumentosID.ConvertAll(x => x.ToString()));
            var ultimosDocumentosVisitados = mEntityContext.UltimosDocumentosVisitados.FirstOrDefault(item => item.ProyectoID.Equals(pProyectoID));
            ultimosDocumentosVisitados.Documentos = ultimosRecursosVisitadosActualizado;
            ultimosDocumentosVisitados.FechaActualizacion = DateTime.Now;
            mEntityContext.SaveChanges();
        }

        #endregion

        /// <summary>
        /// Obtiene los documentos modificados posteriormente a la fecha pasada como parámetro
        /// </summary>
        /// <param name="pFechaModificacion">Fecha de búsqueda de modificaciones</param>
        /// <returns>Lista con los documentosID obtenidos en la búsqueda</returns>
        public List<Guid> ObtenerDocumentosModificados(DateTime pFechaModificacion, int pTopN)
        {
            //BaseComunidadDS baseDS = new BaseComunidadDS();
            //string sqlDocumentos = "select distinct top " + pTopN + " OrdenEjecucion, DocumentoID, Estado, Prioridad, FechaPuestaEnCola, FechaProcesado from ColaActualizarContextos where FechaPuestaEnCola >= @Fecha and estado = 0 order by Prioridad asc";
            //DbCommand comDocumento = ObtenerComando(sqlDocumentos);
            //AgregarParametro(comDocumento, "Fecha", DbType.DateTime, pFechaModificacion);
            //CargarDataSet(comDocumento, baseDS, "ColaActualizarContextos");

            List<Guid> listaDocumentosID = new List<Guid>();
            string sqlDocumentos = "select distinct top " + pTopN + " DocumentoID, Prioridad from ColaActualizarContextos where FechaPuestaEnCola >= @Fecha and estado = 0 order by Prioridad asc";

            DbCommand command = ObtenerComando(sqlDocumentos);
            AgregarParametro(command, "Fecha", DbType.DateTime, pFechaModificacion);
            IDataReader reader = null;
            try
            {
                reader = EjecutarReader(command);
                while (reader.Read())
                {
                    Guid docID = reader.GetGuid(0);
                    if (!listaDocumentosID.Contains(docID))
                    {
                        listaDocumentosID.Add(docID);
                    }
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }

            return listaDocumentosID;
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de compartición automática
        /// </summary>
        /// <returns></returns>
        public BaseComunidadDS ObtenerColaModificacionSearchPendientes()
        {
            BaseComunidadDS baseComunidadDS = new BaseComunidadDS();
            string sql = "Select TOP 500 OrdenEjecucion, ProyectoID, Tipo, Estado, Prioridad, FechaPuestaEnCola, FechaProcesado from ColaModificacionSearch where Estado < 2 order by Prioridad, OrdenEjecucion";
            DbCommand cmd = ObtenerComando(sql);
            CargarDataSet(cmd, baseComunidadDS, "ColaModificacionSearch");
            return baseComunidadDS;
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de compartición automática
        /// </summary>
        /// <returns></returns>
        public BaseComunidadDS ObtenerColaComparticionAutomaticaPendientes()
        {
            BaseComunidadDS baseComunidadDS = new BaseComunidadDS();
            string sql = "Select top 10 OrdenEjecucion, ID, Tipo, Estado, Fecha, Prioridad, IncidenciaComparticion from ColaComparticionAutomatica where estado = 0 order by Prioridad, OrdenEjecucion";
            DbCommand cmd = ObtenerComando(sql);
            CargarDataSet(cmd, baseComunidadDS, "ColaComparticionAutomatica");
            return baseComunidadDS;
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de sitemaps
        /// </summary>
        /// <returns></returns>
        public BaseComunidadDS ObtenerColaSitemapsPendientes()
        {
            BaseComunidadDS baseComunidadDS = new BaseComunidadDS();
            string sql = "Select top 10 OrdenEjecucion, DocumentoID, Tipo, Estado, Fecha, Prioridad, Comunidad from ColaSitemaps where estado < 2 order by Prioridad, OrdenEjecucion";
            DbCommand cmd = ObtenerComando(sql);
            CargarDataSet(cmd, baseComunidadDS, "ColaSitemaps");
            return baseComunidadDS;
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de sitemaps
        /// </summary>
        /// <returns></returns>
        public BaseComunidadDS ObtenerColaSitemapsPendientesTipoComunidad()
        {
            BaseComunidadDS baseComunidadDS = new BaseComunidadDS();
            string sql = "Select top 10 OrdenEjecucion, DocumentoID, Tipo, Estado, Fecha, Prioridad, Comunidad from ColaSitemaps where estado < 2 and tipo = 3 order by Prioridad, OrdenEjecucion";
            DbCommand cmd = ObtenerComando(sql);
            CargarDataSet(cmd, baseComunidadDS, "ColaSitemaps");
            return baseComunidadDS;
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de sitemaps
        /// </summary>
        /// <returns></returns>
        public BaseComunidadDS ObtenerColaSitemapsPendientesPorProyecto(string pNombreCorto)
        {
            BaseComunidadDS baseComunidadDS = new BaseComunidadDS();
            string sql = "Select top 10 OrdenEjecucion, DocumentoID, Tipo, Estado, Fecha, Prioridad, Comunidad from ColaSitemaps where estado < 2 and Comunidad = '" + pNombreCorto + "' order by Prioridad, OrdenEjecucion";
            DbCommand cmd = ObtenerComando(sql);
            CargarDataSet(cmd, baseComunidadDS, "ColaSitemaps");
            return baseComunidadDS;
        }

        /// <summary>
        /// Obtiene los buzones con correos pendientes en la cola de correo
        /// </summary>
        /// <returns></returns>
        public List<string> ObtenerBuzonesCorreo()
        {
            return mEntityContextBASE.ColaCorreo.JoinColaCorreoDestinatario().Where(item => item.ColaCorreoDestinatario.Estado.Equals(0) && !item.ColaCorreo.EnviadoRabbit).GroupBy(item => item.ColaCorreo.SMTP).Select(item => item.Key).ToList();
        }

        public ColaCorreo ObtenerColaCorreoCorreoID(int pCorreoID)
        {
            return mEntityContextBASE.ColaCorreo.Where(item => item.CorreoID.Equals(pCorreoID)).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de correo
        /// </summary>
        /// <returns></returns>
        public List<Email> ObtenerColaCorreo()
        {
            List<ColaCorreo> listaColaCorreo = mEntityContextBASE.ColaCorreo.JoinColaCorreoDestinatario().Where(item => item.ColaCorreoDestinatario.CorreoID.Equals(0)).OrderByDescending(item => item.ColaCorreo.Prioridad).ThenBy(item => item.ColaCorreo.FechaPuestaEnCola).Select(item => item.ColaCorreo).Take(10).ToList();

            List<Email> listaCorreos = new List<Email>();
            string cadenaIn = string.Empty;

            foreach (ColaCorreo colaCorreo in listaColaCorreo)
            {
                Email email = new Email();
                email.Destinatarios = new List<IDestinatarioEmail>();
                email.CorreoID = colaCorreo.CorreoID;
                email.Remitente = colaCorreo.Remitente;
                email.Asunto = colaCorreo.Asunto;
                email.HtmlTexto = colaCorreo.HtmlTexto;
                email.EsHtml = colaCorreo.EsHtml;
                email.Prioridad = colaCorreo.Prioridad;
                email.FechaPuestaEnCola = colaCorreo.FechaPuestaEnCola;
                if (colaCorreo.MascaraRemitente != null)
                {
                    email.MascaraRemitente = colaCorreo.MascaraRemitente;
                }

                ServidorCorreo servidor = new ServidorCorreo();
                servidor.SMTP = colaCorreo.SMTP;
                servidor.Usuario = colaCorreo.Usuario;
                servidor.Password = colaCorreo.Password;
                servidor.Puerto = colaCorreo.Puerto;
                servidor.EsSeguro = colaCorreo.EsSeguro;

                email.ServidorCorreo = servidor;
                email.Tipo = colaCorreo.tipo;

                listaCorreos.Add(email);
                cadenaIn = $"{cadenaIn}{email.CorreoID},";
            }

            cadenaIn = cadenaIn.Substring(0, cadenaIn.Length - 1);
            List<ColaCorreoDestinatario> listaColaCorreoDestinatario = mEntityContextBASE.ColaCorreoDestinatario.Where(item => cadenaIn.Contains(item.CorreoID.ToString()) && item.Estado.Equals((short)EstadoEnvio.Pendiente)).ToList();

            foreach (ColaCorreoDestinatario colaCorreoDestinatario in listaColaCorreoDestinatario)
            {
                DestinatarioEmail destinatarioEmail = new DestinatarioEmail();
                destinatarioEmail.CorreoID = colaCorreoDestinatario.CorreoID;
                destinatarioEmail.Email = colaCorreoDestinatario.Email;
                destinatarioEmail.MascaraDestinatario = "";
                if (colaCorreoDestinatario.MascaraDestinatario != null)
                {
                    destinatarioEmail.MascaraDestinatario = colaCorreoDestinatario.MascaraDestinatario;
                }
                destinatarioEmail.Estado = colaCorreoDestinatario.Estado;
                if (colaCorreoDestinatario.FechaProcesado.HasValue)
                {
                    destinatarioEmail.FechaProcesado = colaCorreoDestinatario.FechaProcesado.Value;
                }
                listaCorreos.Find(item => item.CorreoID.Equals(destinatarioEmail.CorreoID)).Destinatarios.Add(destinatarioEmail);
            }

            return listaCorreos;
        }

        /// <summary>
        /// Devuelve true o false en función de si hay correos pendientes de enviar para un buzón dado
        /// </summary>
        /// <param name="pBuzon">El buzón al que pertenecen los correos que queremos comprobar</param>
        /// <returns>True si hay algún correo pendiente o false si no los hay</returns>
        public bool HayCorreosPendientesBuzon(int pCorreoID)
        {
            return mEntityContextBASE.ColaCorreo.JoinColaCorreoDestinatario().Any(item => item.ColaCorreoDestinatario.Estado.Equals(0) && item.ColaCorreoDestinatario.CorreoID.Equals(pCorreoID));
        }

        public List<Email> ObtenerColaCorreoEmailCorreoID(int pCorreoID)
        {
            List<Email> listaCorreos = new List<Email>();
            string cadenaIn = string.Empty;

            ColaCorreo colaCorreo = mEntityContextBASE.ColaCorreo.JoinColaCorreoDestinatario().Where(item => item.ColaCorreoDestinatario.Estado.Equals(0) && item.ColaCorreo.CorreoID.Equals(pCorreoID)).Select(item => item.ColaCorreo).FirstOrDefault();

            if (colaCorreo != null)
            {
                Email email = new Email();
                email.Destinatarios = new List<IDestinatarioEmail>();
                email.CorreoID = colaCorreo.CorreoID;
                email.Remitente = colaCorreo.Remitente;
                email.Asunto = colaCorreo.Asunto;
                email.HtmlTexto = colaCorreo.HtmlTexto;
                email.EsHtml = colaCorreo.EsHtml;
                email.Prioridad = colaCorreo.Prioridad;
                email.FechaPuestaEnCola = colaCorreo.FechaPuestaEnCola;
                if (colaCorreo.MascaraRemitente != null)
                {
                    email.MascaraRemitente = colaCorreo.MascaraRemitente;
                }
                if (colaCorreo.DireccionRespuesta != null)
                {
                    email.DireccionRespuesta = colaCorreo.DireccionRespuesta;
                }

                ServidorCorreo servidorCorreo = new ServidorCorreo();
                servidorCorreo.SMTP = colaCorreo.SMTP;
                servidorCorreo.Usuario = colaCorreo.Usuario;
                servidorCorreo.Password = colaCorreo.Password;
                servidorCorreo.Puerto = colaCorreo.Puerto;
                servidorCorreo.EsSeguro = colaCorreo.EsSeguro;
                email.ServidorCorreo = servidorCorreo;

                email.Tipo = colaCorreo.tipo;
                listaCorreos.Add(email);
                cadenaIn = $"{cadenaIn}{email.CorreoID},";

                cadenaIn = cadenaIn.Substring(0, cadenaIn.Length - 1);
            }

            List<ColaCorreoDestinatario> listaColaCorreoDestinatario = mEntityContextBASE.ColaCorreoDestinatario.Where(item => cadenaIn.Contains(item.CorreoID.ToString()) && item.Estado.Equals((short)EstadoEnvio.Pendiente)).ToList();

            foreach (ColaCorreoDestinatario colaCorreoDestinatario in listaColaCorreoDestinatario)
            {
                DestinatarioEmail destinatarioEmail = new DestinatarioEmail();
                destinatarioEmail.CorreoID = colaCorreoDestinatario.CorreoID;
                destinatarioEmail.Email = colaCorreoDestinatario.Email;
                destinatarioEmail.MascaraDestinatario = "";
                if (colaCorreoDestinatario.MascaraDestinatario != null)
                {
                    destinatarioEmail.MascaraDestinatario = colaCorreoDestinatario.MascaraDestinatario;
                }
                destinatarioEmail.Estado = colaCorreoDestinatario.Estado;
                if (colaCorreoDestinatario.FechaProcesado.HasValue)
                {
                    destinatarioEmail.FechaProcesado = colaCorreoDestinatario.FechaProcesado.Value;
                }

                if (listaCorreos.Find(item => item.CorreoID.Equals(destinatarioEmail.CorreoID)) != null)
                {
                    listaCorreos.Find(item => item.CorreoID.Equals(destinatarioEmail.CorreoID)).Destinatarios.Add(destinatarioEmail);
                }


            }

            return listaCorreos;
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de correo para un buzon determinado
        /// </summary>
        /// <returns></returns>
        public List<Email> ObtenerColaCorreoBuzon(string pBuzon)
        {
            List<Email> listaCorreos = new List<Email>();
            string cadenaIn = string.Empty;

            List<ColaCorreo> listaColaCorreo = mEntityContextBASE.ColaCorreo.JoinColaCorreoDestinatario().Where(item => item.ColaCorreoDestinatario.Estado.Equals(0) && item.ColaCorreo.SMTP.Equals(pBuzon) && !item.ColaCorreo.EnviadoRabbit).OrderBy(item => item.ColaCorreo.Prioridad).ThenBy(item => item.ColaCorreo).Take(10).Select(item => item.ColaCorreo).ToList();

            foreach (ColaCorreo colaCorreo in listaColaCorreo)
            {
                Email email = new Email();
                email.Destinatarios = new List<IDestinatarioEmail>();
                email.CorreoID = colaCorreo.CorreoID;
                email.Remitente = colaCorreo.Remitente;
                email.Asunto = colaCorreo.Asunto;
                email.HtmlTexto = colaCorreo.HtmlTexto;
                email.EsHtml = colaCorreo.EsHtml;
                email.Prioridad = colaCorreo.Prioridad;
                email.FechaPuestaEnCola = colaCorreo.FechaPuestaEnCola;
                if (colaCorreo.MascaraRemitente != null)
                {
                    email.MascaraRemitente = colaCorreo.MascaraRemitente;
                }
                if (colaCorreo.DireccionRespuesta != null)
                {
                    email.DireccionRespuesta = colaCorreo.DireccionRespuesta;
                }

                ServidorCorreo servidorCorreo = new ServidorCorreo();
                servidorCorreo.SMTP = colaCorreo.SMTP;
                servidorCorreo.Usuario = colaCorreo.Usuario;
                servidorCorreo.Password = colaCorreo.Password;
                servidorCorreo.Puerto = colaCorreo.Puerto;
                servidorCorreo.EsSeguro = colaCorreo.EsSeguro;
                email.ServidorCorreo = servidorCorreo;

                email.Tipo = colaCorreo.tipo;
                listaCorreos.Add(email);
                cadenaIn = $"{cadenaIn}{email.CorreoID},";
            }

            cadenaIn = cadenaIn.Substring(0, cadenaIn.Length - 1);

            List<ColaCorreoDestinatario> listaColaCorreoDestinatario = mEntityContextBASE.ColaCorreoDestinatario.Where(item => cadenaIn.Contains(item.CorreoID.ToString()) && item.Estado.Equals((short)EstadoEnvio.Pendiente)).ToList();

            foreach (ColaCorreoDestinatario colaCorreoDestinatario in listaColaCorreoDestinatario)
            {
                DestinatarioEmail destinatarioEmail = new DestinatarioEmail();
                destinatarioEmail.CorreoID = colaCorreoDestinatario.CorreoID;
                destinatarioEmail.Email = colaCorreoDestinatario.Email;
                destinatarioEmail.MascaraDestinatario = "";
                if (colaCorreoDestinatario.MascaraDestinatario != null)
                {
                    destinatarioEmail.MascaraDestinatario = colaCorreoDestinatario.MascaraDestinatario;
                }
                destinatarioEmail.Estado = colaCorreoDestinatario.Estado;
                if (colaCorreoDestinatario.FechaProcesado.HasValue)
                {
                    destinatarioEmail.FechaProcesado = colaCorreoDestinatario.FechaProcesado.Value;
                }

                listaCorreos.Find(item => item.CorreoID.Equals(destinatarioEmail.CorreoID)).Destinatarios.Add(destinatarioEmail);
            }

            return listaCorreos;
        }

        /// <summary>
        /// Modifica el estado de la fila en ColaCorreoDestinatario
        /// </summary>
        public void ModificarEstadoCorreo(int pCorreoID, string pEmail, short pEstado)
        {
            ColaCorreoDestinatario colaCorreoDestinatario = mEntityContextBASE.ColaCorreoDestinatario.Where(item => item.CorreoID.Equals(pCorreoID) && item.Email.Equals(pEmail)).FirstOrDefault();

            if (colaCorreoDestinatario != null)
            {
                colaCorreoDestinatario.Estado = pEstado;
            }

            mEntityContextBASE.SaveChanges();
        }

        /// <summary>
        /// Comprueba si el correo tiene filas fallidas o pendientes en ColaCorreoDestinatario
        /// </summary>
        /// <param name="pCorreoID">Identificador del correo</param>
        /// <returns>True si quedan correos pendientes de enviar o fallidos. False en caso contrario</returns>
        public bool ComprobarCorreosPendientesEnviar(int pCorreoID)
        {
            return mEntityContextBASE.ColaCorreoDestinatario.Any(item => item.CorreoID.Equals(pCorreoID) && !item.Estado.Equals((short)EstadoEnvio.Enviado));
        }

        /// <summary>
        /// Elimina la fila de ColaCorreo del correo
        /// </summary>
        /// <param name="pCorreoID">Identificador del correo</param>
        public void BorrarCorreo(int pCorreoID)
        {
            ColaCorreo colaCorreo = mEntityContextBASE.ColaCorreo.Where(item => item.CorreoID.Equals(pCorreoID)).FirstOrDefault();

            if (colaCorreo != null)
            {
                mEntityContextBASE.Entry(colaCorreo).State = EntityState.Deleted;

                mEntityContextBASE.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina las filas de correos procesadas correctamente de ColaCorreoDestinatario
        /// </summary>
        public void BorrarCorreosEnviadosCorrectamente(int pCorreoID)
        {
            List<ColaCorreoDestinatario> listaCorreosDestinatarios = mEntityContextBASE.ColaCorreoDestinatario.Where(item => item.CorreoID.Equals(pCorreoID) && item.Estado.Equals((short)EstadoEnvio.Enviado)).ToList();

            foreach (ColaCorreoDestinatario colaCorreoDestinatario in listaCorreosDestinatarios)
            {
                mEntityContextBASE.ColaCorreoDestinatario.Remove(colaCorreoDestinatario);
            }

            mEntityContextBASE.SaveChanges();
        }

        /// <summary>
        /// Elimina las filas procesadas correctamente anteriores a la fecha actual menos un dia
        /// </summary>
        public void BorrarFilasProcesadasCorrectamente()
        {
            DbCommand cmdEliminarFilasCorrectas = ObtenerComando("DELETE FROM ColaComparticionAutomatica where estado >= 5 and fecha < getdate()-1");
            ActualizarBaseDeDatos(cmdEliminarFilasCorrectas);
        }

        /// <summary>
        /// Inserta una fila en ColaCorreo
        /// </summary>
        /// <param name="pRemitente">Email del remitente del mensaje</param>
        /// <param name="pAsunto">Asunto del mensaje</param>
        /// <param name="pHtmlTexto">Cuerpo del mensaje</param>
        /// <param name="pEsHtml">Indica si el cuerpo del mensaje está en formato HTML</param>
        /// <param name="pPrioridad">Prioridad para procesar el mensaje</param>
        /// <param name="pMascaraRemitente">Máscara del remitente</param>
        /// <param name="pDirecionRespuesta">Email donde recibir la respuesta</param>
        /// <param name="pMascaraDireccionRespuesta">Máscara de la dirección de respuesta</param>
        /// <param name="pSMTP">Url del servicio SMTP</param>
        /// <param name="pUsuario">Usuario del servicio de correo</param>
        /// <param name="pPassword">Contraseña del servicio de correo</param>
        /// <param name="pPuerto">Puerto del servicio de correo</param>
        /// <param name="pEsSeguro">Indica si el servicio de correo usa protocolo seguridad</param>
        /// <returns>Entero con el identificador del correo que se ha insertado</returns>
        public int InsertarFilasEnColaCorreo(string pRemitente, string pAsunto, string pHtmlTexto, bool pEsHtml, short pPrioridad, string pMascaraRemitente, string pDireccionRespuesta, string pMascaraDireccionRespuesta, string pSMTP, string pUsuario, string pPassword, int pPuerto, bool pEsSeguro, string pTipo)
        {
            ColaCorreo colaCorreo = new ColaCorreo();

            colaCorreo.Remitente = pRemitente;
            colaCorreo.Asunto = pAsunto;
            colaCorreo.HtmlTexto = pHtmlTexto;
            colaCorreo.EsHtml = pEsHtml;
            colaCorreo.Prioridad = pPrioridad;
            colaCorreo.FechaPuestaEnCola = DateTime.Now;
            colaCorreo.MascaraRemitente = pMascaraRemitente;
            colaCorreo.DireccionRespuesta = pDireccionRespuesta;
            colaCorreo.MascaraDireccionRespuesta = pMascaraDireccionRespuesta;
            colaCorreo.SMTP = pSMTP;
            colaCorreo.Usuario = pUsuario;
            colaCorreo.Password = pPassword;
            colaCorreo.Puerto = pPuerto;
            colaCorreo.EsSeguro = pEsSeguro;
            colaCorreo.tipo = pTipo;

            if (!string.IsNullOrWhiteSpace(mConfigService.ObtenerRabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN)))
            {
                colaCorreo.EnviadoRabbit = true;
            }

            mEntityContextBASE.ColaCorreo.Add(colaCorreo);
            try
            {
                mEntityContextBASE.SaveChanges();
            }
            catch (DbUpdateException)
            {
                //Por defecto el CorreoID debería asignarlo atomáticamente la base de datos, si falla lo asignamos manualmente y guardamos de nuevo
                int numCorreos = mEntityContextBASE.ColaCorreo.Count();
                colaCorreo.CorreoID = numCorreos + 1;
                mEntityContextBASE.SaveChanges();
            }

            return colaCorreo.CorreoID;
        }

        /// <summary>
        /// Inserta una fila pendiente de procesar en ColaCorreoDestinatario
        /// </summary>
        /// <param name="pCorreoID">Identificador del correo a enviar</param>
        /// <param name="pEmail">Email del destinatario</param>
        /// <param name="pMascaraDestinatario">Máscara del destinatario</param>
        /// <returns>True si se ha insertado. False en caso contrario</returns>
        public bool InsertarFilasEnColaCorreoDestinatarios(int pCorreoID, string pEmail, string pMascaraDestinatario)
        {
            ColaCorreoDestinatario colaCorreoDestinatario = new ColaCorreoDestinatario();
            colaCorreoDestinatario.CorreoID = pCorreoID;
            colaCorreoDestinatario.Email = pEmail;
            colaCorreoDestinatario.MascaraDestinatario = pMascaraDestinatario;
            colaCorreoDestinatario.Estado = (short)EstadoEnvio.Pendiente;

            mEntityContextBASE.ColaCorreoDestinatario.Add(colaCorreoDestinatario);


            return mEntityContextBASE.ColaCorreoDestinatario.Any(item => item.CorreoID.Equals(pCorreoID));
        }

        /// <summary>
        /// Inserta una fila en la cola de sitemaps
        /// </summary>
        /// <param name="pDocumentoID">ID del documento</param>
        /// <param name="pTipoEvento">Tipo de evento del site map</param>
        /// <param name="pEstado">Estado de la fila</param>
        /// <param name="pFechaCreacion">Fecha de creación/comkpartición de recurso en la comunidad</param>
        /// <param name="pPrioridad">Priorida de la fila</param>
        /// <param name="pComunidad">Nombre corto de la comunidad</param>
        public void InsertarFilaEnColaColaSitemaps(Guid pDocumentoID, TiposEventoSitemap pTipoEvento, short pEstado, DateTime pFechaCreacion, short pPrioridad, string pComunidad)
        {
            DbCommand cmdInsertColaSitemaps = ObtenerComando("INSERT INTO ColaSiteMaps (DocumentoID, Tipo, Estado, Fecha, Prioridad, Comunidad) VALUES (@DocumentoID, @Tipo, @Estado, @Fecha, @Prioridad, @Comunidad) ");

            AgregarParametro(cmdInsertColaSitemaps, IBD.ToParam("DocumentoID"), DbType.Guid, pDocumentoID);
            AgregarParametro(cmdInsertColaSitemaps, IBD.ToParam("Tipo"), DbType.Int16, pTipoEvento);
            AgregarParametro(cmdInsertColaSitemaps, IBD.ToParam("Estado"), DbType.Int16, pEstado);
            AgregarParametro(cmdInsertColaSitemaps, IBD.ToParam("Fecha"), DbType.DateTime, pFechaCreacion);
            AgregarParametro(cmdInsertColaSitemaps, IBD.ToParam("Prioridad"), DbType.Int16, pPrioridad);
            AgregarParametro(cmdInsertColaSitemaps, IBD.ToParam("Comunidad"), DbType.String, pComunidad);

            ActualizarBaseDeDatos(cmdInsertColaSitemaps);
        }

        /// <summary>
        /// Inserta una fila en la cola de actualizacion de contextos
        /// </summary>
        /// <param name="pDocumentoID">Id del documento</param>
        /// <param name="pEstado">Estado de la fila</param>
        /// <param name="pPrioridad">Prioridad de la fila</param>
        /// <param name="FechaPuestaEnCola">Fecha de creacion del recurso</param>
        public void InsertarFilaEnColaActualizaContextos(Guid pDocumentoID, short pEstado, short pPrioridad, DateTime pFechaPuestaEnCola)
        {
            DbCommand cmdInsertColaActualizarContextos = ObtenerComando("INSERT INTO ColaActualizarContextos (DocumentoID, Estado, Prioridad, FechaPuestaEnCola) VALUES (@DocumentoID, @Estado, @Prioridad, @FechaPuestaEnCola)");

            AgregarParametro(cmdInsertColaActualizarContextos, IBD.ToParam("DocumentoID"), DbType.Guid, pDocumentoID);
            AgregarParametro(cmdInsertColaActualizarContextos, IBD.ToParam("Estado"), DbType.Int16, pEstado);
            AgregarParametro(cmdInsertColaActualizarContextos, IBD.ToParam("Prioridad"), DbType.Int16, pPrioridad);
            AgregarParametro(cmdInsertColaActualizarContextos, IBD.ToParam("FechaPuestaEnCola"), DbType.DateTime, pFechaPuestaEnCola);

            ActualizarBaseDeDatos(cmdInsertColaActualizarContextos);
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de compartición automática
        /// </summary>
        /// <returns></returns>
        public void ActualizarEstadoColaModificacionSearch(int pOrdenEjecucion, short pEstado)
        {
            BaseComunidadDS baseComunidadDS = new BaseComunidadDS();
            string sql = "UPDATE ColaModificacionSearch SET Estado = " + pEstado + ", FechaProcesado = GETDATE() where OrdenEjecucion = " + pOrdenEjecucion;
            DbCommand cmd = ObtenerComando(sql);
            ActualizarBaseDeDatos(cmd);
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de compartición automática
        /// </summary>
        /// <returns></returns>
        public void ActualizarEstadoColaComparticionAutomatica(Guid pComparticionID, short pEstado, string pIncidencia)
        {
            BaseComunidadDS baseComunidadDS = new BaseComunidadDS();
            string sql = "UPDATE ColaComparticionAutomatica SET Estado = " + pEstado;

            if ((pEstado == (short)EstadoComparticion.ComparticionParcial) && pIncidencia.Length > 0)
            {
                sql += ", IncidenciaComparticion = '" + pIncidencia + "'";
            }

            sql += " where OrdenEjecucion = " + IBD.GuidValor(pComparticionID);

            DbCommand cmd = ObtenerComando(sql);
            ActualizarBaseDeDatos(cmd);
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de sitemaps
        /// </summary>
        /// <returns></returns>
        public void ActualizarEstadoColaSitemaps(int pOrdenEjecucion, short pEstado)
        {
            BaseComunidadDS baseComunidadDS = new BaseComunidadDS();
            string sql = "UPDATE ColaSitemaps SET Estado = " + pEstado + " where OrdenEjecucion = " + pOrdenEjecucion;
            DbCommand cmd = ObtenerComando(sql);
            ActualizarBaseDeDatos(cmd);
        }

        /// <summary>
        /// Obtiene una lista con los nombres cortos de los proyectos que tienen filas pendientes en colasitemaps
        /// </summary>
        /// <returns>Lista de cadenas con los nombres cortos de los proyectos</returns>
        public List<string> ObtenerNombresCortosProyectosPendientesColaSitemap()
        {
            BaseComunidadDS baseComunidadDS = new BaseComunidadDS();
            string sql = "SELECT distinct Comunidad from ColaSitemaps where estado = 0";
            DbCommand cmd = ObtenerComando(sql);
            List<string> proyectos = new List<string>();
            IDataReader reader = null;
            try
            {
                reader = EjecutarReader(cmd);
                while (reader.Read())
                {
                    proyectos.Add(reader.GetString(0));
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }
            return proyectos;
        }


        public void ObtenerTagsRelacionadosFreebase(DataSet pDataSet, List<string> pListaTags)
        {
            if (pListaTags.Count > 0)
            {
                string strSQL = "Select * from " + mNomberTablaFreebase + " where (Ruta is not null or WikipediaID is not null)";

                DbCommand cmdObtenerGrafoTagsCom = ObtenerComando(strSQL);

                int contador = 0;
                string where = " AND Entidad IN (";
                string coma = "";

                foreach (string tag in pListaTags)
                {
                    where += coma + IBD.ToParam("Tag" + contador);

                    AgregarParametro(cmdObtenerGrafoTagsCom, IBD.ToParam("Tag" + contador), DbType.String, tag);

                    coma = ", ";
                    contador++;
                }
                where += ")";
                cmdObtenerGrafoTagsCom.CommandText += where;

                CargarDataSet(cmdObtenerGrafoTagsCom, pDataSet, "GnossToFreebase");
            }
        }

        /// <summary>
        /// Comprueba de una lista de Guids de Freebase, cuáles NO estan ya en la tabla de Freebase de un proyecto
        /// </summary>
        /// <param name="pListaGuidFreebase">Lista de Guids de freebase que se quieren añadir</param>
        /// <returns>Lista de Guids de Freebase que NO estan ya agregados a la tabla de freebase</returns>
        public List<string> ComprobarSiGuidFreebaseEstaEnProyecto(List<string> pListaGuidFreebase)
        {
            List<string> listaGuidsValidos = new List<string>(pListaGuidFreebase);
            if (pListaGuidFreebase.Count > 0)
            {
                string strSQL = "SELECT GUIDFreebase FROM " + mNomberTablaFreebase + " where GUIDFreebase IN (";
                string coma = "";
                foreach (string guid in pListaGuidFreebase)
                {
                    strSQL += coma + "'" + guid + "'";
                    coma = ", ";
                }
                strSQL += ")";

                DbCommand cmdObtenerGrafoTagsCom = ObtenerComando(strSQL);
                IDataReader reader = null;
                try
                {
                    reader = EjecutarReader(cmdObtenerGrafoTagsCom);
                    while (reader.Read())
                    {
                        listaGuidsValidos.Remove(reader.GetString(0));
                    }
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                        reader.Dispose();
                    }
                }
            }
            return listaGuidsValidos;
        }

        public void ObtenerEntidadFreebase(DataSet pDataSet, List<string> LTag)
        {
            if (LTag.Count > 0)
            {
                DbCommand cmdObtenerGrafoTagsCom = ObtenerComando("select *");
                int contador = 0;
                string whereIdentidad = "";
                whereIdentidad = " WHERE Tag IN (";
                string coma = "";

                foreach (string tag in LTag)
                {
                    whereIdentidad += coma + IBD.ToParam("Tag" + contador);
                    AgregarParametro(cmdObtenerGrafoTagsCom, IBD.ToParam("Tag" + contador), DbType.String, tag);
                    coma = ", ";
                    contador++;
                }
                whereIdentidad += ")";

                string strSQL = "Select distinct * from " + mNomberTablaFreebase + whereIdentidad + " order by coincidencia desc";

                cmdObtenerGrafoTagsCom.CommandText = ObtenerComando(strSQL).CommandText;

                try
                {
                    CargarDataSet(cmdObtenerGrafoTagsCom, pDataSet, "GnossToFreebase");
                }
                catch (Exception e)
                {
                    mLoggingService.GuardarLogError(e, mlogger);
                }
            }
        }

        ///// <summary>
        ///// Se inserta la query en la cola de replicación por defecto.
        ///// </summary>
        ///// <param name="pQuery">Query que se va a ejecutar</param>
        ///// <param name="pPrioridad">Prioridad que tiene la query</param>
        ///// <param name="pInfoExtra">Información extra para el procesado por el base.</param>
        //public void InsertarConsultaEnColaReplicacion(string pQuery, int pPrioridad, bool pUsarHttpPost = true, string pInfoExtra = null)
        //{
        //    InsertarConsultaEnColaReplicacion(pQuery, pPrioridad, "ColaReplicacionMaster", pInfoExtra, pUsarHttpPost);
        //}

        ///// <summary>
        ///// Se inserta la query en la cola de replicación pasada como parámetro
        ///// </summary>
        ///// <param name="pQuery">Query que se va a ejecutar</param>
        ///// <param name="pPrioridad">Prioridad que tiene la query</param>
        ///// <param name="pTablaReplicacion">Tabla donde se va a replicar la query.</param>
        ///// <param name="pInfoExtra">Información extra para el procesado por el base.</param>
        //public void InsertarConsultaEnColaReplicacion(string pQuery, int pPrioridad, string pTablaReplicacion, string pInfoExtra, bool pUsarHttpPost = true)
        //{
        //    try
        //    {
        //        DbCommand cmdInsertReplica = ObtenerComando("INSERT INTO " + pTablaReplicacion + " (Consulta, Estado, Prioridad, FechaPuestaEnCola, InfoExtra, UsarHttpPost) VALUES (" + IBD.ToParam("consulta") + ", 0, " + pPrioridad + ", getdate(), '" + pInfoExtra + "', " + Convert.ToInt32(pUsarHttpPost) + ")");
        //        AgregarParametro(cmdInsertReplica, IBD.ToParam("consulta"), DbType.String, pQuery);
        //        ActualizarBaseDeDatos(cmdInsertReplica);
        //    }
        //    catch
        //    {
        //        string jsonObject = "{ \"replicacion\": { " +
        //            "\"Query\": \"" + pQuery.Replace("\"", "\\\"") + "\",  " +
        //            "\"Prioridad\": \"" + pPrioridad + "\",  " +
        //            "\"TablaReplicacion\": \"" + pTablaReplicacion + "\",  " +
        //            "\"InfoExtra\": \"" + pInfoExtra + "\" " +
        //       " }}, ";

        //        AgregarColaReplicacionFichero(jsonObject);
        //    }
        //}

        /// <summary>
        /// Se inserta la query en la cola de replicación pasada como parámetro
        /// </summary>
        /// <param name="pQuery">Query que se va a ejecutar</param>
        /// <param name="pPrioridad">Prioridad que tiene la query</param>
        /// <param name="pTablaReplicacion">Tabla donde se va a replicar la query.</param>
        /// <param name="pInfoExtra">Información extra para el procesado por el base.</param>
        public void InsertarLogConsultaCostosa(string pConsulta, int pTiempo, string pServidor, string pError)
        {
            DbCommand cmdInsertReplica = ObtenerComando(string.Format("INSERT INTO LogConsultasVirtuoso (Fecha, Milisegundos, Servidor, Consulta, Error) VALUES (getdate(), {0}, {1}, {2}, {3})", IBD.ToParam("milisegundos"), IBD.ToParam("servidor"), IBD.ToParam("consulta"), IBD.ToParam("error")));

            AgregarParametro(cmdInsertReplica, IBD.ToParam("milisegundos"), DbType.Int32, pTiempo);
            AgregarParametro(cmdInsertReplica, IBD.ToParam("servidor"), DbType.String, pServidor);
            AgregarParametro(cmdInsertReplica, IBD.ToParam("consulta"), DbType.String, pConsulta);

            AgregarParametro(cmdInsertReplica, IBD.ToParam("error"), DbType.String, pError);

            ActualizarBaseDeDatos(cmdInsertReplica);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BaseComunidadDS ObtenerColaRefrescoCachePendientes()
        {
            BaseComunidadDS baseComunidadDS = new BaseComunidadDS();

            string consulta = "SELECT TOP 10 ColaID, ProyectoID, TipoEvento, TipoBusqueda, Fecha, InfoExtra, Estado FROM ColaRefrescoCache WHERE Estado < 1 AND TipoBusqueda != 12 AND TipoEvento != 3 ORDER BY Fecha";
            DbCommand comando = ObtenerComando(consulta);
            CargarDataSet(comando, baseComunidadDS, "ColaRefrescoCache");

            return baseComunidadDS;
        }

        public void EliminarColaRefrescoCachePendientesRepetidas()
        {
            //Elimina de ColaRefrescoCache las que tengan estado 0 y tengan el mismo ProyectoID, TipoEvento, TipoBusqueda,InfoExtra y Estado (excepto la mas antigua)
            string delete = "   delete from ColaRefrescoCache ";
            delete += "   where estado=0 ";
            delete += "   and ColaRefrescoCache.colaID not in ";
            delete += "   ( ";
            delete += "       select ColaRefrescoCache.ColaID from ColaRefrescoCache ";
            delete += "       INNER JOIN ( ";
            delete += "           SELECT ProyectoID, TipoEvento, TipoBusqueda,min(Fecha) as Fecha, InfoExtra,Estado";
            delete += "           FROM ColaRefrescoCache ";
            delete += "           WHERE Estado =0";
            delete += "           group by ProyectoID, TipoEvento, TipoBusqueda, InfoExtra,Estado";
            delete += "       ) as ColaRefrescoCacheAUX ";
            delete += "       on ColaRefrescoCacheAUX.ProyectoID=ColaRefrescoCache.ProyectoID";
            delete += "       AND ColaRefrescoCacheAUX.TipoEvento=ColaRefrescoCache.TipoEvento";
            delete += "       AND ColaRefrescoCacheAUX.TipoBusqueda=ColaRefrescoCache.TipoBusqueda";
            delete += "       AND ColaRefrescoCacheAUX.Fecha=ColaRefrescoCache.Fecha";
            delete += "       AND (ColaRefrescoCache.InfoExtra=ColaRefrescoCacheAUX.InfoExtra OR (ColaRefrescoCache.InfoExtra is null AND ColaRefrescoCacheAUX.InfoExtra is null))";
            delete += "       AND ColaRefrescoCacheAUX.Estado=ColaRefrescoCache.Estado)";

            DbCommand cmdColaRefrescoCachePendientesRepetidas = ObtenerComando(delete);
            ActualizarBaseDeDatos(cmdColaRefrescoCachePendientesRepetidas);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BaseComunidadDS ObtenerColaRefrescoCacheBandejaMensajesPendientes()
        {
            BaseComunidadDS baseComunidadDS = new BaseComunidadDS();

            string consulta = "SELECT TOP 10 ColaID, ProyectoID, TipoEvento, TipoBusqueda, Fecha, InfoExtra, Estado FROM ColaRefrescoCache WHERE Estado < 1 AND TipoBusqueda = 12 AND TipoEvento = 3 ORDER BY Fecha";
            DbCommand comando = ObtenerComando(consulta);
            CargarDataSet(comando, baseComunidadDS, "ColaRefrescoCache");

            return baseComunidadDS;
        }

        /// <summary>
        /// Elimina una fila de la cola de refresco de caché
        /// </summary>
        /// <param name="pColaID">Identificador de la fila</param>
        public void EliminarFilaColaRefrescoCache(int pColaID)
        {
            DbCommand cmdEliminarColaCache = ObtenerComando("DELETE FROM ColaRefrescoCache WHERE ColaID = " + IBD.ToParam("colaID"));

            AgregarParametro(cmdEliminarColaCache, IBD.ToParam("colaID"), DbType.Int32, pColaID);

            ActualizarBaseDeDatos(cmdEliminarColaCache);
        }

        /// <summary>
        /// Actualiza el estado de una fila de la cola de refresco de caché
        /// </summary>
        /// <param name="pColaID">Identificador de la fila</param>
        /// <param name="pEstado">Estado nuevo de la fila</param>
        public void AcutalizarEstadoColaRefrescoCache(int pColaID, short pEstado)
        {
            DbCommand cmdActualizarColaCache = ObtenerComando("UPDATE ColaRefrescoCache SET Estado = " + IBD.ToParam("estado") + " WHERE ColaID = " + IBD.ToParam("colaID"));

            AgregarParametro(cmdActualizarColaCache, IBD.ToParam("colaID"), DbType.Int32, pColaID);

            AgregarParametro(cmdActualizarColaCache, IBD.ToParam("estado"), DbType.Int16, pEstado);

            ActualizarBaseDeDatos(cmdActualizarColaCache);
        }

        /// <summary>
        /// Actualiza el estado de una fila de la cola de actualización de contextos
        /// </summary>
        /// <param name="pDocumentoID"></param>
        /// <param name="pEstado"></param>
        public void ActualizarEstadoColaActualizarContextos(Guid pDocumentoID, short pEstado, DateTime pFechaProcesado)
        {
            DbCommand cmdActualizarColaActualizarContextos = ObtenerComando("UPDATE ColaActualizarContextos SET Estado = " + IBD.ToParam("estado") + ", FechaProcesado = @FechaProcesado WHERE DocumentoID = " + IBD.CargarGuid("DocumentoID") + " and estado = 0");

            AgregarParametro(cmdActualizarColaActualizarContextos, IBD.ToParam("DocumentoID"), DbType.Guid, pDocumentoID);
            AgregarParametro(cmdActualizarColaActualizarContextos, IBD.ToParam("FechaProcesado"), DbType.DateTime, pFechaProcesado);
            AgregarParametro(cmdActualizarColaActualizarContextos, IBD.ToParam("estado"), DbType.Int16, pEstado);

            ActualizarBaseDeDatos(cmdActualizarColaActualizarContextos);
        }

        /// <summary>
        /// Inserta una fila en la cola de refresco de caché para que se actualice una búsqueda determinada en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pTipoBusqueda">Tipo de búsqueda que hay que refrescar</param>
        /// <param name="pInfoExtra">Información extra (puede ser NULL)</param>
        public void InsertarFilaEnColaRefrescoCache(Guid pProyectoID, TiposEventosRefrescoCache pTipoEvento, TipoBusqueda pTipoBusqueda, string pInfoExtra)
        {
            DbCommand cmdInsertColaCache = ObtenerComando("INSERT INTO ColaRefrescoCache (ProyectoID, TipoEvento, TipoBusqueda, Fecha, InfoExtra, Estado) VALUES (@proyectoID, @tipoEvento, @tipoBusqueda, @fecha, @infoExtra, @estado) ");

            AgregarParametro(cmdInsertColaCache, IBD.ToParam("proyectoID"), DbType.Guid, pProyectoID);
            AgregarParametro(cmdInsertColaCache, IBD.ToParam("tipoEvento"), DbType.Int16, (short)pTipoEvento);
            AgregarParametro(cmdInsertColaCache, IBD.ToParam("tipoBusqueda"), DbType.Int16, (short)pTipoBusqueda);
            AgregarParametro(cmdInsertColaCache, IBD.ToParam("fecha"), DbType.DateTime, DateTime.Now);
            AgregarParametro(cmdInsertColaCache, IBD.ToParam("infoExtra"), DbType.String, pInfoExtra);
            AgregarParametro(cmdInsertColaCache, IBD.ToParam("estado"), DbType.Int16, 0);

            ActualizarBaseDeDatos(cmdInsertColaCache);
        }

        public string PreprarFilaColaRefrescoCacheRabbitMQ(Guid pProyectoID, TiposEventosRefrescoCache pTipoEvento, TipoBusqueda pTipoBusqueda, string pInfoExtra)
        {
            BaseComunidadDS baseComunidadDS = new BaseComunidadDS();
            ColaRefrescoCacheRow filaColaRefrescoCache = baseComunidadDS.ColaRefrescoCache.NewColaRefrescoCacheRow();
            filaColaRefrescoCache.ProyectoID = pProyectoID;
            filaColaRefrescoCache.TipoEvento = (short)pTipoEvento;
            filaColaRefrescoCache.TipoBusqueda = (short)pTipoBusqueda;
            filaColaRefrescoCache.InfoExtra = pInfoExtra;
            filaColaRefrescoCache.Fecha = DateTime.Now;
            filaColaRefrescoCache.Estado = 0;
            baseComunidadDS.Dispose();

            return JsonConvert.SerializeObject(filaColaRefrescoCache.ItemArray);
        }
        public void InsertarFilasColaRefrecoCacheEnRabbitMQ(List<string> pFilasAInsertar, TiposEventosRefrescoCache pTipoEvento)
        {
            string exchange = "";
            string colaRabbit = "ColaRefrescoCache";
            if (pTipoEvento.Equals(TiposEventosRefrescoCache.CambiosBandejaDeMensajes))
            {
                colaRabbit = "ColaRefrescoCacheMensajes";
            }
            if (!string.IsNullOrWhiteSpace(mConfigService.ObtenerRabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN)))
            {
                using (RabbitMQClient rabbitMQ = new RabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN, colaRabbit, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<RabbitMQClient>(), mLoggerFactory, exchange, colaRabbit))
                {
                    List<string> mensajesFallidos = (List<string>)rabbitMQ.AgregarElementosACola(pFilasAInsertar);
                    if (mensajesFallidos.Count > 0)
                    {
                        foreach (string mensajeFallido in mensajesFallidos)
                        {
                            mLoggingService.GuardarLogError("Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla colaRefrescoCache",mlogger);
                            ColaRefrescoCacheRow colaRefrescoCacheRow = JsonConvert.DeserializeObject<ColaRefrescoCacheRow>(mensajeFallido);
                            InsertarFilaEnColaRefrescoCache(colaRefrescoCacheRow.ProyectoID, (TiposEventosRefrescoCache)colaRefrescoCacheRow.TipoEvento, (TipoBusqueda)colaRefrescoCacheRow.TipoBusqueda, colaRefrescoCacheRow.InfoExtra);
                        }
                    }
                }
            }

        }

        public void InsertarFilaColaRefrescoCacheEnRabbitMQ(Guid pProyectoID, TiposEventosRefrescoCache pTipoEvento, TipoBusqueda pTipoBusqueda, string pInfoExtra)
        {
            string filaColaRefrescoCacheSerializado = PreprarFilaColaRefrescoCacheRabbitMQ(pProyectoID, pTipoEvento, pTipoBusqueda, pInfoExtra);

            string exchange = "";
            string colaRabbit = "ColaRefrescoCache";
            if (pTipoEvento.Equals(TiposEventosRefrescoCache.CambiosBandejaDeMensajes) || pTipoEvento.Equals(TiposEventosRefrescoCache.CambiosAmigosPrivados))
            {
                colaRabbit = "ColaRefrescoCacheMensajes";
            }
            
            if (!string.IsNullOrWhiteSpace(mConfigService.ObtenerRabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN)))
            {
                using (RabbitMQClient rabbitMQ = new RabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN, colaRabbit, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<RabbitMQClient>(), mLoggerFactory, exchange, colaRabbit))
                {
                    rabbitMQ.AgregarElementoACola(filaColaRefrescoCacheSerializado);
                }
            }

        }

        /// <summary>
        /// Comprueba si existe o no la cola indicada por parámetro en RabbitMQ
        /// </summary>
        /// <param name="pNombreCola">Nombre de la cola a comprobar si existe</param>
        /// <returns>true o false si existe o no la cola respectivamente</returns>
        public bool ExisteColaRabbit(string pNombreCola)
        {
            using (RabbitMQClient rabbitMQ = new RabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN, pNombreCola, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<RabbitMQClient>(), mLoggerFactory))
            {
                return rabbitMQ.ExisteColaRabbit(pNombreCola);
            }
        }

        #region Verificación de existencia y creación de tablas

        /// <summary>
        /// Crea una tabla en la base de datos del modelo BASE
        /// </summary>
        /// <param name="pSqlCreateTable"></param>
        protected void CrearTabla(string pSqlCreateTable)
        {
            DbCommand cmdCrearTabla = ObtenerComando(pSqlCreateTable);

            ActualizarBaseDeDatos(cmdCrearTabla);
        }

        /// <summary>
        /// Crea una tabla en función de un tipo de conuslta
        /// <param name="pTipoConsulta">Tipo de consulta</param>
        /// </summary>
        protected virtual void CrearTabla(TiposConsultaObtenerTags pTipoConsulta)
        {

            if (pTipoConsulta.Equals(TiposConsultaObtenerTags.Freebase))
            {
                CrearTabla("CREATE TABLE [dbo]." + mNomberTablaFreebase + "(	[Tag] [nvarchar](1000) NOT NULL,	[GUIDFreebase] [nvarchar](4000) NOT NULL,	[Coincidencia] [int] NOT NULL,	[Entidad] [nvarchar](4000) NOT NULL,	[Ruta] [nvarchar](4000) NULL,	[WikipediaID] [nvarchar](4000) NULL,	[Tipos] [nvarchar](4000) NULL,	[Descripcion] [nvarchar](4000) NULL,	[RutaNYT] [nvarchar](4000) NULL,	[RutaDBPedia] [nvarchar](4000) NULL,	[RutaGeonames] [nvarchar](4000) NULL, PRIMARY KEY (GUIDFreebase));");
            }
        }

        /// <summary>
        /// Comprueba si existen las tablas sobre las que está configurado este AD. Si no existen las crea. 
        /// </summary>
        /// <returns>Verdad si la tabla existe (o ha sido recién creada).</returns>
        protected bool VerificarExisteTabla(string pNombreTabla)
        {
            string existeTabla = "SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = " + IBD.ToParam("nombreTabla");

            DbCommand cmdExisteTabla = ObtenerComando(existeTabla);
            AgregarParametro(cmdExisteTabla, IBD.ToParam("nombreTabla"), DbType.String, pNombreTabla);

            object resultado = EjecutarEscalar(cmdExisteTabla);

            return (resultado != null) && (resultado is int) && (resultado.Equals(1));
        }

        #endregion

        #region Metodos abstractos

        /// <summary>
        /// Obtiene el sql correspondiente a un tipo de consulta
        /// </summary>
        /// <param name="pTipoConsulta">Tipo de consulta</param>
        /// <returns></returns>
        protected internal virtual string ObtenerNombreTablaPorTipoConsulta(TiposConsultaObtenerTags pTipoConsulta) { return ""; }
        /// <summary>
        /// Obtiene el sql correspondiente a un tipo de consulta
        /// </summary>
        /// <param name="pTipoConsulta">Tipo de consulta</param>
        /// <returns></returns>
        protected virtual string ObtenerNombreTablaPorTipoConsultaConIDTags(TiposConsultaObtenerTags pTipoConsulta)
        {
            return ObtenerNombreTablaPorTipoConsulta(pTipoConsulta);
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de tags
        /// </summary>
        /// <returns>Devuelve un dataset con los elementos de la cola cargados</returns>
        /// <param name="pEstadoInferior">Estado mínimo de los elementos de la cola</param>
        /// <param name="pEstadoSuperior">Estado máximo de los elementos de la cola</param>
        /// <param name="pTiposElementos">Tipo de los elementos a obtener (Agregado, eliminado, ...). Null para obtener todos</param>
        /// <param name="pNumMaxItems">Numero máximo de items a traer</param>
        public virtual DataSet ObtenerElementosColaPendientes(EstadosColaTags pEstadoInferior, EstadosColaTags pEstadoSuperior, TiposElementosEnCola? pTiposElementos, int pNumMaxItems)
        {
            return null;
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de tags
        /// </summary>
        /// <returns>Devuelve un dataset con los elementos de la cola cargados</returns>
        /// <param name="pEstadoInferior">Estado mínimo de los elementos de la cola</param>
        /// <param name="pEstadoSuperior">Estado máximo de los elementos de la cola</param>
        /// <param name="pTiposElementos">Tipo de los elementos a obtener (Agregado, eliminado, ...). Null para obtener todos</param>
        /// <param name="pNumMaxItems">Numero máximo de items a traer</param>
        public virtual DataSet ObtenerElementosColaPendientes(EstadosColaTags pEstadoInferior, EstadosColaTags pEstadoSuperior, TiposElementosEnCola? pTiposElementos, int pNumMaxItems, bool? pSoloPrioridad0)
        {
            return null;
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de tags
        /// </summary>
        /// <returns>Devuelve un dataset con los elementos de la cola cargados</returns>
        /// <param name="pEstadoInferior">Estado mínimo de los elementos de la cola</param>
        /// <param name="pEstadoSuperior">Estado máximo de los elementos de la cola</param>
        /// <param name="pTiposElementos">Lista de los tipo de elementos a obtener (Agregado, eliminado, ...). Null para obtener todos</param>
        /// <param name="pNumMaxItems">Numero máximo de items a traer</param>
        public virtual DataSet ObtenerElementosColaPendientes(List<TiposElementosEnCola> pTiposElementos, EstadosColaTags pEstadoInferior, EstadosColaTags pEstadoSuperior, int pNumMaxItems, bool? pSoloPrioridad0)
        {
            return null;
        }

        /// <summary>
        /// Obtiene el número de filas que hay entre los estados especificados en las últimas pHoras horas
        /// </summary>
        ///<param name="pHoras">Horas</param>
        ///<param name="pEstadoInferior">Estado inferior</param>
        ///<param name="pEstadoSuperior">Estado superior</param>
        /// <returns></returns>
        public virtual int ObtenerNumeroElementosEnXHoras(int pHoras, EstadosColaTags pEstadoInferior, EstadosColaTags pEstadoSuperior)
        {
            return 0;
        }

        /// <summary>
        /// Elimina de la cola los elementos que han sido procesado exitósamente hace una semana
        /// </summary>
        public void EliminarElementosColaProcesadosViejos(string pNombreTablaCola)
        {
            DbCommand cmdEliminarElementosColaPendientesComunidades = ObtenerComando("DELETE FROM " + pNombreTablaCola + " WHERE ((Estado = " + (short)EstadosColaTags.Procesado + " AND Tipo != 0) OR Estado = " + (short)EstadosColaTags.ProcesadoFreebase + ") AND FechaProcesado < " + IBD.ToParam("fecha"));

            AgregarParametro(cmdEliminarElementosColaPendientesComunidades, IBD.ToParam("fecha"), DbType.DateTime, DateTime.Now.AddDays(-7));

            ActualizarBaseDeDatos(cmdEliminarElementosColaPendientesComunidades);
        }

        /// <summary>
        /// Elimina de la cola los elementos que han sido procesado exitósamente hace una semana
        /// Si es la cola maestra, copia el contenido en la tabla de histórico
        /// </summary>
        /// <param name="pEsMaster">Verdad si es la cola maestra</param>
        /// <param name="pNombreTablaCola">Nombre de la tabla de cola</param>
        /// <param name="pFechaLimiteEliminacion">Fecha límite hasta la cuál se eliminaran los elementos en cola</param>
        public void EliminarElementosColaReplicaProcesados(string pNombreTablaCola, bool pEsMaster, DateTime pFechaLimiteEliminacion)
        {
            //if (pEsMaster)
            //{
            //    DbCommand cmdCopiarElementosColaProcesados = ObtenerComando("INSERT INTO ColaReplicacionHistorico SELECT * FROM " + pNombreTablaCola + " WHERE Estado = " + (short)EstadosColaTags.Procesado + " AND FechaProcesado < " + IBD.ToParam("fecha") + " AND OrdenEjecucion > (SELECT ISNULL(MAX(OrdenEjecucion), 0) FROM ColaReplicacionHistorico)");
            //    cmdCopiarElementosColaProcesados.CommandTimeout = 300;

            //    AgregarParametro(cmdCopiarElementosColaProcesados, IBD.ToParam("fecha"), DbType.DateTime, pFechaLimiteEliminacion);

            //    ActualizarBaseDeDatos(cmdCopiarElementosColaProcesados);
            //}

            DbCommand cmdEliminarElementosColaProcesados = ObtenerComando("DELETE FROM " + pNombreTablaCola + " WHERE Estado = " + (short)EstadosColaTags.Procesado + " AND FechaProcesado < " + IBD.ToParam("fecha"));
            cmdEliminarElementosColaProcesados.CommandTimeout = 1800; // 1800 segundos = 30 minutos

            AgregarParametro(cmdEliminarElementosColaProcesados, IBD.ToParam("fecha"), DbType.DateTime, pFechaLimiteEliminacion);

            ActualizarBaseDeDatos(cmdEliminarElementosColaProcesados);
        }

        /// <summary>
        /// Comprueba si existe la tabla de comunidades. Si no existe la crea 
        /// </summary>
        /// <param name="pCrearTablaSiNoExiste">Verdad si se debe crear la tabla en caso de que no exista</param>
        /// <param name="pTipoConsulta">Tipo de consulta que se va a realizar</param>
        /// <returns>Verdad si la tabla existe (o ha sido recién creada).</returns>
        public virtual bool VerificarExisteTabla(TiposConsultaObtenerTags pTipoConsulta, bool pCrearTablaSiNoExiste)
        {
            bool existeTabla = false;
            if (pTipoConsulta.Equals(TiposConsultaObtenerTags.Freebase))
            {
                existeTabla = VerificarExisteTabla(mNomberTablaFreebase);
            }
            return existeTabla;
        }

        #endregion

        #endregion

        #region Actualizar

        /// <summary>
        /// Actualiza el estado de tags de la cola.
        /// </summary>
        /// <param name="pFila">Fila de la cola</param>
        public void ActualizarEstadoTagsCola(DataRow pFila)
        {
            DbCommand comandoUpdateEstTag = ObtenerComando("UPDATE " + pFila.Table.TableName + " SET EstadoTags=" + IBD.ToParam("EstadoTags") + " WHERE OrdenEjecucion=" + IBD.ToParam("OrdenEjecucion"));
            AgregarParametro(comandoUpdateEstTag, IBD.ToParam("EstadoTags"), DbType.Int16, (short)pFila["EstadoTags"]);
            AgregarParametro(comandoUpdateEstTag, IBD.ToParam("OrdenEjecucion"), DbType.Int32, (int)pFila["OrdenEjecucion"]);
            ActualizarBaseDeDatos(comandoUpdateEstTag);
        }

        /// <summary>
        /// Actualiza el estado de la cola.
        /// </summary>
        /// <param name="pFila">Fila de la cola</param>
        public void ActualizarEstadoCola(DataRow pFila)
        {
            DbCommand comandoUpdateEst = ObtenerComando("UPDATE " + pFila.Table.TableName + " SET Estado =" + IBD.ToParam("Estado") + ", FechaProcesado=" + IBD.ToParam("FechaProc") + " WHERE OrdenEjecucion=" + IBD.ToParam("OrdenEjecucion"));

            if (pFila.IsNull("Estado"))
            {
                mLoggingService.GuardarLog("El campo Estado no puede ser NULL", mlogger);
            }
            if (pFila.IsNull("OrdenEjecucion"))
            {
                mLoggingService.GuardarLog("El campo OrdenEjecucion no puede ser NULL", mlogger);
            }

            AgregarParametro(comandoUpdateEst, IBD.ToParam("Estado"), DbType.Int16, (short)pFila["Estado"]);
            if (!pFila.IsNull("FechaProcesado"))
            {
                AgregarParametro(comandoUpdateEst, IBD.ToParam("FechaProc"), DbType.DateTime, (DateTime)pFila["FechaProcesado"]);
            }
            AgregarParametro(comandoUpdateEst, IBD.ToParam("OrdenEjecucion"), DbType.Int32, (int)pFila["OrdenEjecucion"]);
            ActualizarBaseDeDatos(comandoUpdateEst);
        }

        #endregion

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece si la comunidad sobre la que se hacen las búsquedas es privada
        /// </summary>
        public bool EsComunidadPrivada
        {
            get
            {
                return mEsComunidadPrivada;
            }
            set
            {
                mEsComunidadPrivada = value;
            }
        }

        /// <summary>
        /// Obtiene o establece si el usuario que está realizando la consulta ha hecho login.
        /// </summary>
        public bool EstaUsuarioConectado
        {
            get
            {
                return mEstaUsuarioConectado;
            }
            set
            {
                mEstaUsuarioConectado = value;
            }
        }

        /// <summary>
        /// Obtiene o establece la identidad con la que está conectado el usuario actual
        /// </summary>
        public Guid IdentidadUsuarioConectado
        {
            get
            {
                return mIdentidadUsuarioConectado;
            }
            set
            {
                this.mIdentidadUsuarioConectado = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el campo IdentidadID de las tablas que tienen este campo (simpre es IdentidadID menos en MyGnoss que es UsuarioID)
        /// </summary>
        public string CampoIdentidadID
        {
            get
            {
                return mCampoIdentidadID;
            }
            set
            {
                this.mCampoIdentidadID = value;
            }
        }

        /// <summary>
        /// Obtiene o establece si la búsqueda se realiza en MyGnoss o en una comunidad
        /// </summary>
        public bool BusquedaEnMyGnoss
        {
            get
            {
                return mBusquedaEnMyGnoss;
            }
            set
            {
                this.mBusquedaEnMyGnoss = value;
            }
        }
        /// <summary>
        /// Obtiene la conexión a la base de datos Master
        /// </summary>
        protected override DbConnection ConexionMaster
        {
            get
            {


                var conexion = mEntityContextBASE.Database.GetDbConnection();
                if (conexion.State != ConnectionState.Open)
                {
                    conexion.Open();
                }
                return conexion;
            }
        }
        #endregion

        #endregion

    }
}
