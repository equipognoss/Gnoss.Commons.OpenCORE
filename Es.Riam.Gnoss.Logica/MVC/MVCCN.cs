using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Blog;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.MVC;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro;
using Es.Riam.Gnoss.AD.MVC;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data;

namespace Es.Riam.Gnoss.Logica.MVC
{
    /// <summary>
    /// Lógica de negocio para el MVC de GNOSS
    /// </summary>
    public class MVCCN : BaseCN, IDisposable
    {
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #region Constructores

        /// <summary>
        /// Constructor para MVCCN
        /// </summary>
        public MVCCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<MVCCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.MVCAD = new MVCAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<MVCAD>(), mLoggerFactory);
        }

        /// <summary>
        /// Constructor para MVCCN
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public MVCCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<MVCCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.MVCAD = new MVCAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<MVCAD>(), mLoggerFactory);
        }
        #endregion

        #region Métodos generales

        public List<ObtenerComunidades> ObtenerComunidadesPorID(List<Guid> pListaComunidadesID)
        {
            return MVCAD.ObtenerComunidadesPorID(pListaComunidadesID);
        }

        public List<ObtenerTesauroProyectoMVC> ObtenerCategoriasComunidadesPorID(List<Guid> pListaComunidadesID)
        {
            return MVCAD.ObtenerCategoriasComunidadesPorID(pListaComunidadesID);
        }

        public List<Blog> ObtenerBlogsPorID(List<Guid> pListaBlogsID)
        {
            return MVCAD.ObtenerBlogsPorID(pListaBlogsID);
        }

        /// <summary>
        /// Obtiene un recurso en u proyecto a partir de su identificador y el del proyecto
        /// </summary>
        /// <param name="pListaDocumentoID">Identificador de los recursos</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Ficha del recurso</returns>
        public List<ObtenerRecursoMVC> ObtenerRecursosPorID(List<Guid> pListaDocumentoID, Guid pProyectoID)
        {
            return MVCAD.ObtenerRecursosPorID(pListaDocumentoID, pProyectoID);
        }

        /// <summary>
        /// Obtiene los eventos de un recurso en u proyecto a partir de su identificador y el del proyecto
        /// </summary>
        /// <param name="pListaDocumentoID">Identificador de los recursos</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Ficha del recurso</returns>
        public List<ObtenerRecursosEvento> ObtenerEventosRecursosPorID(List<Guid> pListaDocumentoID, Guid pProyectoID)
        {
            return MVCAD.ObtenerEventosRecursosPorID(pListaDocumentoID, pProyectoID);
        }

        public List<Guid> ObtenerCategoriaDeTesauroPorID(List<Guid> pListaDocumentoID)
        {
            return MVCAD.ObtenerCategoriaDeTesauroPorID(pListaDocumentoID);
        }

        public Dictionary<Guid, List<Guid>> ObtenerDiccionarioCategoriaDeTesauroPorID(List<Guid> pListaDocumentoID)
        {
            return MVCAD.ObtenerDiccionarioCategoriaDeTesauroPorID(pListaDocumentoID);
        }

        public List<ObtenerCategoriaDeRecursos> ObtenerCategoriasDeRecursosPorID(List<Guid> pListaDocumentoID, Guid pProyectoID)
        {
            return MVCAD.ObtenerCategoriasDeRecursosPorID(pListaDocumentoID, pProyectoID);
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
            return MVCAD.ObtenerCategoriasDeRecursosPorIDEspacioPersonal(pListaDocumentoID, pBaseRecursosPersonalID, pBaseRecursosOrganizacion);
        }

        /// <summary>
        /// Obtiene los comentarios de los recursos pasados por id
        /// </summary>
        /// <param name="pListaDocumentoID"></param>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public List<AD.EntityModel.Models.Comentario.Comentario> ObtenerComentariosDeRecursosPorID(List<Guid> pListaDocumentoID)
        {
            return MVCAD.ObtenerComentariosDeRecursosPorID(pListaDocumentoID);
        }

        /// <summary>
        /// Obtiene un recurso a partir de su identificador.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        public List<AD.EntityModel.Models.IdentidadDS.IdentidadMVC> ObtenerIdentidadesPorID(List<Guid> pListaIdentidadesID)
        {
            return MVCAD.ObtenerIdentidadesPorID(pListaIdentidadesID);
        }

        public List<DatoExtraFichasdentidad> ObtenerDatosExtraFichasIdentidadesPorIdentidadesID(List<Guid> pListaIdentidadesID)
        {
            return MVCAD.ObtenerDatosExtraFichasIdentidadesPorIdentidadesID(pListaIdentidadesID);
        }

        public List<DatoExtraFichasdentidad> ObtenerDatosExtraFichasIdentidadesPorPerfilesID(List<Guid> pListaPerfilesID)
        {
            return MVCAD.ObtenerDatosExtraFichasIdentidadesPorPerfilesID(pListaPerfilesID);
        }

        /// <summary>
        /// Obtiene los contadores de recuros de una lista de identidades
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        public List<IdentidadContadoresRecursos> ObtenerContadoresRecursosIdentiadesPorID(List<Guid> pListaIdentidadesID)
        {
            return MVCAD.ObtenerContadoresRecursosIdentiadesPorID(pListaIdentidadesID);
        }

        /// <summary>
        /// Obtiene un DataReader a partir de una lista de identificadores de grupos
        /// </summary>
        /// <param name="pListaGruposID">Lista de identificadores de grupos</param>
        public List<GrupoIdentidadesPorId> ObtenerGruposPorID(List<Guid> pListaGruposID)
        {
            return MVCAD.ObtenerGruposPorID(pListaGruposID);
        }

        /// <summary>
        /// Obtiene un DataReader a partir de una lista de identificadores de mensajes
        /// </summary>
        /// <param name="pListaMensajesID">Lista de identificadores de mensajes</param>
        public IDataReader ObtenerMensajesPorID(List<Guid> pListaMensajesID, Guid pIdentidadID, short? pTipoBandeja)
        {
            return MVCAD.ObtenerMensajesPorID(pListaMensajesID, pIdentidadID, pTipoBandeja);
        }

        /// <summary>
        /// Obtiene un DataReader a partir de una lista de identificadores de comentarios
        /// </summary>
        /// <param name="pListaComentariosID">Lista de identificadores de comentarios</param>
        public List<ComentarioDocumentoProyecto> ObtenerComentariosPorID(List<Guid> pListaComentariosID, Guid pIdentidadID)
        {
            return MVCAD.ObtenerComentariosPorID(pListaComentariosID, pIdentidadID);
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
            return MVCAD.ObtenerContactosPorID(pListaPersonas, pListaOrganizaciones, pListaGrupos, pIdentidadID);
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
            return MVCAD.ObtenerParticipantesGruposContactosPorID(pListaPersonas, pListaOrganizaciones, pListaGrupos, pIdentidadID);
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
        ~MVCCN()
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
                if (MVCAD != null)
                {
                    MVCAD.Dispose();
                }
                MVCAD = null;
            }
        }

        #endregion

        #region Propiedades

        private MVCAD MVCAD
        {
            get
            {
                return (MVCAD)AD;
            }
            set
            {
                this.AD = value;
            }
        }

        #endregion

    }
}
