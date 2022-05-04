using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.CL.Usuarios
{
    public class UsuarioCL : BaseCL, IDisposable
    {

        #region Miembros

        /// <summary>
        /// Clase de negocio
        /// </summary>
        private UsuarioCN mUsuarioCN = null;

        /// <summary>
        /// Clave MAESTRA de la caché
        /// </summary>
        private readonly string[] mMasterCacheKeyArray = { "Usuario" };

        private ConfigService mConfigService;
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;

        #endregion

        public UsuarioCL(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
        }

        #region Métodos

        /// <summary>
        /// Elimina la caché de usuarios de una organizacion con sus identidades
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        public void EliminarCacheUsuariosCargaLigeraParaFiltros(Guid pOrganizacionID)
        {
            string rawKey = string.Concat("UsuariosDeOrganizacionCargaLigeraParaFiltros_", pOrganizacionID);
            InvalidarCache(rawKey);
        }

        /// <summary>
        /// Agrega la caché de usuarios de una organizacion con sus identidades
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        public DataWrapperUsuario ObtenerCacheUsuariosCargaLigeraParaFiltros(Guid pOrganizacionID)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = string.Concat("UsuariosDeOrganizacionCargaLigeraParaFiltros_", pOrganizacionID);

            // Compruebo si está en la caché
            DataWrapperUsuario dataWrapperUsuario = ObtenerObjetoDeCache(rawKey) as DataWrapperUsuario;
            if (dataWrapperUsuario == null)
            {
                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                // Si no está, lo cargo y lo almaceno en la caché
                dataWrapperUsuario = usuarioCN.CargarUsuariosDeOrganizacionCargaLigeraParaFiltros(pOrganizacionID);
                if (dataWrapperUsuario != null)
                {
                    dataWrapperUsuario.CargaRelacionesPerezosasCache();
                }
                usuarioCN.Dispose();
                AgregarObjetoCache(rawKey, dataWrapperUsuario, 600); //La clave dura 10 minutos
            }
            mEntityContext.UsarEntityCache = false;
            return dataWrapperUsuario;
        }

        /// <summary>
        /// Datos de login de red social
        /// </summary>
        /// <param name="pIdioma">Idioma</param>
        public void GuardarDatosRedSocial(string pID, Dictionary<string, object> pDatos)
        {
            string rawKey = string.Concat("idRedSocial_", pID);
            AgregarObjetoCache(rawKey, pDatos, 7200); //La clave caduca a las 2 horas
        }

        /// <summary>
        /// Obtener datos de login de red social
        /// </summary>
        public Dictionary<string, object> ObtenerDatosRedSocial(string pID)
        {
            Dictionary<string, object> datos = null;

            string rawKey = string.Concat("idRedSocial_", pID);

            // Compruebo si está en la caché
            datos = ObtenerObjetoDeCache(rawKey) as Dictionary<string, object>;
            return datos;
        }

        /// <summary>
        /// Obtener Redireccion Usuario
        /// </summary>
        /// <param name="pIdioma">Idioma</param>
        public string ObtenerRedireccionUsuario(Guid pUsuarioID)
        {
            string rawKey = string.Concat("UsuarioRedireccion_", pUsuarioID);
            string redireccion = ObtenerObjetoDeCache(rawKey) as string;
            if (redireccion == null)
            {
                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                redireccion = UsuarioCN.ObtenerUrlRedirect(pUsuarioID);
                usuarioCN.Dispose();

                if (string.IsNullOrEmpty(redireccion))
                {
                    AgregarObjetoCache(rawKey, string.Empty, 0);
                }
            }
            return redireccion;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Clase de negocio
        /// </summary>
        protected UsuarioCN UsuarioCN
        {
            get
            {
                if (mUsuarioCN == null)
                {
                    mUsuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                }

                return mUsuarioCN;
            }
        }

        /// <summary>
        /// Clave para la caché
        /// </summary>
        public override string[] ClaveCache
        {
            get
            {
                return mMasterCacheKeyArray;
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
        ~UsuarioCL()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);

            //impido que se finalice dos veces este objeto
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.disposed = true;

                try
                {
                    if (disposing)
                    {
                    }
                }
                catch (Exception e)
                {
                    mLoggingService.GuardarLogError(e);
                }
            }
        }

        #endregion
    }
}
