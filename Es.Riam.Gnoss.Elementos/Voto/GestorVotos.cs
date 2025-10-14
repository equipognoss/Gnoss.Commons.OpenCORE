using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Voto;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Es.Riam.Gnoss.Elementos.Voto
{
    #region Enumeraciones

    /// <summary>
    /// Enumeración que representa el tipo de voto
    /// </summary>
    public enum TiposVotos
    {
        /// <summary>
        /// Voto de un comentario
        /// </summary>
        Comentario = 0,
        /// <summary>
        /// Voto de un blog
        /// </summary>
        Blog = 1,
        /// <summary>
        /// Voto de una entrada de blog
        /// </summary>
        EntradaBlog = 2,
        /// <summary>
        /// Voto de un mensaje de un foro
        /// </summary>
        MensajeForo = 3,
        /// <summary>
        /// Voto de un factor de DAFO
        /// </summary>
        FactorDafo = 4,
        /// <summary>
        /// Voto de un documento
        /// </summary>
        Documento = 5
    }

    #endregion

    /// <summary>
    /// Clase para controlar los votos
    /// </summary>
    [Serializable]
    public class GestorVotos : GestionGnoss
    {
        #region Miembros

        /// <summary>
        /// Lista de votos
        /// </summary>
        private Dictionary<Guid, AD.EntityModel.Models.Voto.Voto> mListaVotos;

        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructores

        public GestorVotos() { }

        /// <summary>
        /// Constructor vacío
        /// </summary>
        public GestorVotos(LoggingService loggingService,  EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<GestorVotos> logger, ILoggerFactory loggerFactory)
            : base()
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected GestorVotos(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        /// <summary>
        /// Constructor a partir del dataset de votos
        /// </summary>
        /// <param name="pVotoDS">Dataset de votos</param>
        public GestorVotos(DataWrapperVoto pVotoDS, LoggingService loggingService,  EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<GestorVotos> logger, ILoggerFactory loggerFactory)
            : base()
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;

            VotoDW = pVotoDS;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Crea un nuevo voto
        /// </summary>
        /// <param name="pVoto">Valor del voto</param>
        /// <param name="pIdentidadID">Identificador de la identidad que vota</param>
        /// <param name="pElementoID">Identificador del elemento votado</param>
        /// <param name="pIdentidadVotadaID">Identificador de la identidad votada</param>
        /// <param name="pTipo">Tipo de voto</param>
        /// <returns>Nuevo voto</returns>
        public AD.EntityModel.Models.Voto.Voto AgregarVoto(double pVoto, Guid pIdentidadID, Guid pElementoID, Guid pIdentidadVotadaID, short pTipo)
        {
            AD.EntityModel.Models.Voto.Voto filaVoto = new AD.EntityModel.Models.Voto.Voto();

            filaVoto.VotoID = Guid.NewGuid();
            filaVoto.Voto1 = pVoto;
            filaVoto.IdentidadID = pIdentidadID;
            filaVoto.ElementoID = pElementoID;
            filaVoto.IdentidadVotadaID = pIdentidadVotadaID;
            filaVoto.Tipo = pTipo;
            filaVoto.FechaVotacion = DateTime.Now;

            ListaVotos.Add(filaVoto.VotoID, filaVoto);

            VotoDW.ListaVotos.Add(filaVoto);
            mEntityContext.Voto.Add(filaVoto);

            return filaVoto;
        }

        /// <summary>
        /// Elimina un voto
        /// </summary>
        /// <param name="pVoto">Voto que se debe eliminar</param>
        public void EliminarVoto(Voto pVoto)
        {
            if (ListaVotos.ContainsKey(pVoto.Clave))
                ListaVotos.Remove(pVoto.Clave);

            mEntityContext.EliminarElemento(pVoto.FilaVoto);
            VotoDW.ListaVotos.Remove(pVoto.FilaVoto);
        }

        /// <summary>
        /// Guarda los datos referentes a votos
        /// </summary>
        public void Guardar()
        {
            VotoCN votoCN = new VotoCN( mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VotoCN>(), mLoggerFactory);
            votoCN.ActualizarEntity();
            votoCN.Dispose();
        }

        /// <summary>
        /// Recarga la lista de votos del gestor
        /// </summary>
        public void RecargarVotos()
        {
            mListaVotos = new Dictionary<Guid, AD.EntityModel.Models.Voto.Voto>();
            foreach (AD.EntityModel.Models.Voto.Voto filaVoto in VotoDW.ListaVotos)
            {
                if (!ListaVotos.ContainsKey(filaVoto.VotoID))
                {
                    mListaVotos.Add(filaVoto.VotoID, filaVoto);
                }
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la lista de votos
        /// </summary>
        public Dictionary<Guid, AD.EntityModel.Models.Voto.Voto> ListaVotos
        {
            get
            {
                if (mListaVotos == null)
                {
                    mListaVotos = new Dictionary<Guid, AD.EntityModel.Models.Voto.Voto>();
                    RecargarVotos();
                }
                return mListaVotos;
            }
        }

        /// <summary>
        /// Obtiene o establece el dataset de votos
        /// </summary>
        public DataWrapperVoto VotoDW
        {
            get
            {
                return (DataWrapperVoto)DataWrapper;
            }
            set
            {
                DataWrapper = value;
            }
        }

        #endregion
    }
}
