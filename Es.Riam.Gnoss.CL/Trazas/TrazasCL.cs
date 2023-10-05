using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.CL.Trazas
{
    public class TrazasCL : BaseCL, IDisposable
    {
        private static string KEY_TRAZA = "_traza";
        /// <summary>
        /// Clave MAESTRA de la caché
        /// </summary>
        private readonly string[] mMasterCacheKeyArray = { NombresCL.TRAZAS };
        public TrazasCL(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication) : base(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
        }

        public bool ExisteTrazaEnCache(string pNombreTraza)
        {
            string key = pNombreTraza + KEY_TRAZA;
            var objeto = ObtenerObjetoDeCache(key, true);
            string trazaName = objeto as string;
            return !string.IsNullOrEmpty(trazaName);
        }

        public void InvalidarTraza(string pNombreTraza)
        {
            string key = pNombreTraza + KEY_TRAZA;
            InvalidarCache(key, true);
        }

        public void AgregarTraza(string pNombreTraza, double pTiempo, string pObjeto)
        {
            string key = pNombreTraza + KEY_TRAZA;
            AgregarObjetoCache(key, pObjeto, pTiempo, true);
        }

        public string ObtenerTrazaEnCache(string pNombreTraza)
        {
            string key = pNombreTraza + KEY_TRAZA;
            var objeto = ObtenerObjetoDeCache(key, true);
            string trazaName = objeto as string;
            return trazaName;
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
    }
}