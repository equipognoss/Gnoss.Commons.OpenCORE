using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.BASE_BD.Model;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.Organizador.Correo;
using Es.Riam.Gnoss.Logica.BASE_BD;
using Es.Riam.Gnoss.Logica.Live;
using Es.Riam.Gnoss.RabbitMQ;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;

namespace Es.Riam.Gnoss.Web.Controles.Organizador.Correo
{
    /// <summary>
    /// Controlador de correo.
    /// </summary>
    public class ControladorCorreo : ControladorBase
    {
        #region Estáticos

        private const string EXCHANGE = "";
        private const string COLA_RABBIT = "ColaTagsMensaje";

        #endregion

        #region Miembros

        /// <summary>
        /// Gestor de correo
        /// </summary>
        private GestionCorreo mGestionCorreo = null;

        /// <summary>
        /// Indica si la bandeja de entrada actual es de organización o no.
        /// </summary>
        private bool mBandejaDeOrganizacion;

        /// <summary>
        /// Indica si los mensajes de organización de la bandeja de entrada actual deben mostrarse o no.
        /// </summary>
        private bool mMezclarBandejaOrganizacion;

        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private EntityContext mEntityContext;
        private IHttpContextAccessor mHttpContextAccessor;
        private ConfigService mConfigService;
        private EntityContextBASE mEntityContextBASE;

        #endregion

        #region Constructor

        /// <summary>
        /// Construcor del controlador de correo.
        /// </summary>
        public ControladorCorreo(LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, EntityContextBASE entityContextBASE, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mHttpContextAccessor = httpContextAccessor;
            mEntityContextBASE = entityContextBASE;
        }

        #endregion

        #region Métodos
        /// <summary>
        /// Notifica al live que determinados usuario tienen correos nuevos.
        /// </summary>
        /// <param name="pIdentidades">Identidades destinatarias</param>
        //public static void AgregarNotificacionCorreoNuevoAIdentidades(List<Guid> pIdentidades)
        //{
        //    IdentidadCN identCN = new IdentidadCN();
        //    List<Guid> listaPerfiles = identCN.ObtenerPerfilesDeIdentidades(pIdentidades);
        //    identCN.Dispose();

        //    AgregarNotificacionCorreoNuevoAPerfiles(listaPerfiles);
        //}

        /// <summary>
        /// Notifica al live que determinados usuario tienen correos nuevos.
        /// </summary>
        /// <param name="pPerfilID">Perfil destinatario</param>
        //public static void AgregarNotificacionCorreoNuevoAPerfiles(Guid pPerfilID)
        //{
        //    List<Guid> listaPerfiles = new List<Guid>();
        //    listaPerfiles.Add(pPerfilID);
        //    AgregarNotificacionCorreoNuevoAPerfiles(listaPerfiles);
        //}


        /// <summary>
        /// Notifica al live que determinados usuario tienen correos nuevos.
        /// </summary>
        /// <param name="pPerfiles">Perfiles destinatarios</param>
        //public static void AgregarNotificacionCorreoNuevoAPerfiles(List<Guid> pPerfiles)
        //{
        //    LiveCN liveCN = new LiveCN();
        //    foreach (Guid perfilID in pPerfiles)
        //    {
        //        liveCN.AumentarContadorNuevosMensajes(perfilID);
        //    }
        //    liveCN.Dispose();
        //}

        public void AgregarNotificacionCorreoLeidoAPerfil(Guid pPerfilID)
        {
            LiveCN liveCN = new LiveCN(mEntityContext, mLoggingService, mConfigService, null);
            liveCN.DisminuirContadorMensajesLeidos(pPerfilID);
        }

        /// <summary>
        /// Resetea el contador de nuevos mensajes de un perfil.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil</param>
        public void ResetearContadorNuevosMensajes(Guid pPerfilID)
        {
            LiveCN liveCN = new LiveCN(mEntityContext, mLoggingService, mConfigService, null);
            liveCN.ResetearContadorNuevosMensajes(pPerfilID);
        }

        public void AgregarElementoARabbitMQ(int pProyectoID, Guid pCorreoID, string pDestinatarioID, Guid pIdentidadRemitenteID, PrioridadBase pPrioridadBase, string pMensajeOrigen = "")
        {
            BaseMensajesDS baseMensajesDS = new BaseMensajesDS();
            BaseMensajesDS.ColaTagsMensajeRow filaColaTagsCom = baseMensajesDS.ColaTagsMensaje.NewColaTagsMensajeRow();
            filaColaTagsCom.Estado = (short)EstadosColaTags.EnEspera;
            filaColaTagsCom.FechaPuestaEnCola = DateTime.Now;
            filaColaTagsCom.TablaBaseProyectoID = pProyectoID;
            filaColaTagsCom.Tags = Constantes.ID_MENSAJE + pCorreoID.ToString() + Constantes.ID_MENSAJE + Constantes.ID_MENSAJE_FROM + pIdentidadRemitenteID.ToString() + Constantes.ID_MENSAJE_FROM + Constantes.IDS_MENSAJE_TO + pDestinatarioID + Constantes.IDS_MENSAJE_TO + pMensajeOrigen;
            filaColaTagsCom.Tipo = 0;
            filaColaTagsCom.Prioridad = (short)pPrioridadBase;
            bool agregadoABD = false;

            try
            {

                if (mConfigService.ExistRabbitConnection(RabbitMQClient.BD_SERVICIOS_WIN))
                {
                    using (RabbitMQClient rabbitMQ = new RabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN, COLA_RABBIT, mLoggingService, mConfigService, EXCHANGE, COLA_RABBIT))
                    {
                        rabbitMQ.AgregarElementoACola(JsonConvert.SerializeObject(filaColaTagsCom.ItemArray));
                    }
                }
                else
                {
                    baseMensajesDS.ColaTagsMensaje.AddColaTagsMensajeRow(filaColaTagsCom);
                    agregadoABD = true;
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla Cola");
                baseMensajesDS.ColaTagsMensaje.AddColaTagsMensajeRow(filaColaTagsCom);
                agregadoABD = true;
            }

            if (agregadoABD)
            {
                BaseComunidadCN brComCN = new BaseComunidadCN(mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication);//, -1
                brComCN.InsertarFilasEnRabbit("ColaTagsMensaje", baseMensajesDS);
                baseMensajesDS.Dispose();
            }
        }

        #endregion
    }
}
