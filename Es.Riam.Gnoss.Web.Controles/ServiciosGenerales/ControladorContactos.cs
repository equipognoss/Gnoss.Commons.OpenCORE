using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Amigos.Model;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.BASE_BD.Model;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Amigos;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Logica.Amigos;
using Es.Riam.Gnoss.Logica.BASE_BD;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Data;

namespace Es.Riam.Gnoss.Web.Controles.ServiciosGenerales
{
    /// <summary>
    /// Controlador para Contactos
    /// </summary>
    public class ControladorContactos
    {
        #region Miembros

        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private EntityContextBASE mEntityContextBASE;
        private RedisCacheWrapper mRedisCacheWrapper;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructores

        /// <summary>
        /// Constructor a partir de la página que contiene al controlador
        /// </summary>
        /// <param name="pPage">Página</param>
        public ControladorContactos(LoggingService loggingService, EntityContext entityContext, ConfigService configService, EntityContextBASE entityContextBASE, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ControladorContactos> logger, ILoggerFactory loggerFactory)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mEntityContextBASE = entityContextBASE;
            mRedisCacheWrapper = redisCacheWrapper;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #endregion

        #region Metodos generales

        public void ActualizarModeloBaseSimple(Guid pIdentidadID, Guid pContactoID)
        {
            BaseComunidadCN baseContactosCN = new BaseComunidadCN(mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, null, mLoggerFactory.CreateLogger<BaseComunidadCN>(), mLoggerFactory);
            BaseContactosDS baseContactosDS = new BaseContactosDS();

            string todosTags = Constantes.ID_IDENTIDAD + pIdentidadID + Constantes.ID_IDENTIDAD;
            todosTags += ", " + Constantes.ID_CONTACTO + pContactoID + Constantes.ID_CONTACTO;

            BaseContactosDS.ColaTagsContactoRow filaColaTagsContactos_Add = baseContactosDS.ColaTagsContacto.NewColaTagsContactoRow();
            MeterValoresEnFilaCola(filaColaTagsContactos_Add, todosTags, 11, TiposElementosEnCola.Agregado);
            baseContactosDS.ColaTagsContacto.AddColaTagsContactoRow(filaColaTagsContactos_Add);


            string todosTags2 = Constantes.ID_IDENTIDAD + pContactoID + Constantes.ID_IDENTIDAD;
            todosTags2 += ", " + Constantes.ID_CONTACTO + pIdentidadID + Constantes.ID_CONTACTO;

            BaseContactosDS.ColaTagsContactoRow filaColaTagsContactos2_Add = baseContactosDS.ColaTagsContacto.NewColaTagsContactoRow();
            MeterValoresEnFilaCola(filaColaTagsContactos2_Add, todosTags2, 11, TiposElementosEnCola.Agregado);
            baseContactosDS.ColaTagsContacto.AddColaTagsContactoRow(filaColaTagsContactos2_Add);

            baseContactosCN.InsertarFilasEnRabbit("ColaTagsContacto", baseContactosDS);
        }

        public void ActualizarEliminacionModeloBaseSimple(Guid pIdentidadID, Guid pContactoID)
        {

            BaseComunidadCN baseContactosCN = new BaseComunidadCN(mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, null, mLoggerFactory.CreateLogger<BaseComunidadCN>(), mLoggerFactory);
            BaseContactosDS baseContactosDS = new BaseContactosDS();

            string todosTags = Constantes.ID_IDENTIDAD + pIdentidadID + Constantes.ID_IDENTIDAD;
            todosTags += ", " + Constantes.ID_CONTACTO + pContactoID + Constantes.ID_CONTACTO;

            BaseContactosDS.ColaTagsContactoRow filaColaTagsContactos_Del = baseContactosDS.ColaTagsContacto.NewColaTagsContactoRow();
            MeterValoresEnFilaCola(filaColaTagsContactos_Del, todosTags, 11, TiposElementosEnCola.Eliminado);
            baseContactosDS.ColaTagsContacto.AddColaTagsContactoRow(filaColaTagsContactos_Del);

            string todosTags2 = Constantes.ID_IDENTIDAD + pContactoID + Constantes.ID_IDENTIDAD;
            todosTags2 += ", " + Constantes.ID_CONTACTO + pIdentidadID + Constantes.ID_CONTACTO;

            BaseContactosDS.ColaTagsContactoRow filaColaTagsContactos2_Del = baseContactosDS.ColaTagsContacto.NewColaTagsContactoRow();
            MeterValoresEnFilaCola(filaColaTagsContactos2_Del, todosTags2, 11, TiposElementosEnCola.Eliminado);
            baseContactosDS.ColaTagsContacto.AddColaTagsContactoRow(filaColaTagsContactos2_Del);

            baseContactosCN.InsertarFilasEnRabbit("ColaTagsContacto", baseContactosDS);
        }

        private void MeterValoresEnFilaCola(DataRow pFilaCola, string pTags, int pIdProyecto, TiposElementosEnCola pTipoRelacionTags)
        {
            pFilaCola["TablaBaseProyectoID"] = pIdProyecto;
            pFilaCola["Tags"] = pTags;
            pFilaCola["Tipo"] = (short)pTipoRelacionTags;
            pFilaCola["Estado"] = 0;
            pFilaCola["FechaPuestaEnCola"] = DateTime.Now;
        }

        /// <summary>
        /// Deniega los permiso a la identidadid sobre el grupo y los miembros del grupo
        /// </summary>
        /// <param name="pGrupoID">Grupo a eliminar de los contactos</param>
        /// <param name="pIdentidadID">Identidad del miembro de la org al que se le van a quitar los contactos y el grupo</param>
        /// <param name="pAmigosDS">Dataset con los miembros del grupo.</param>
        public void ProcesarDenegarMiembroAGrupoPorElBase(Guid pGrupoID, Guid pIdentidadID, AmigosDS pAmigosDS, Guid? pMiembroEliminadoID, string pUrlIntraGnoss)
        {
            FacetadoCN facetadoCN = new FacetadoCN(pUrlIntraGnoss, "contactos/", mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
            AmigosCL amigosCL = new AmigosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AmigosCL>(), mLoggerFactory);

            if (pMiembroEliminadoID != null)
            {
                //Insertamos como nuevo contacto a los miembros del grupo.
                facetadoCN.BorrarContacto(pIdentidadID, pMiembroEliminadoID.Value);
                amigosCL.InvalidarAmigos(pMiembroEliminadoID.Value);
            }
            else
            {
                foreach (AmigosDS.AmigoAgGrupoRow amigo in pAmigosDS.AmigoAgGrupo.Select("GrupoID = '" + pGrupoID + "'"))
                {
                    //Insertamos como nuevo contacto a los miembros del grupo.
                    facetadoCN.BorrarContacto(pIdentidadID, amigo.IdentidadAmigoID);
                    amigosCL.InvalidarAmigos(amigo.IdentidadAmigoID);
                }
            }

            amigosCL.Dispose();
            facetadoCN.Dispose();
        }

        #endregion
    }
}
