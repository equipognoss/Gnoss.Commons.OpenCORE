using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Carga;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.AD.CargaMasiva
{
    public class CargaMasivaConfiguracionAD : BaseAD 
    {
        #region Miembros

        private readonly EntityContext mEntityContext;

        #endregion

        #region Constructores

        public CargaMasivaConfiguracionAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<CargaMasivaConfiguracionAD> logger, ILoggerFactory loggerFactory)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mEntityContext = entityContext;
        }

        #endregion

        #region DominiosPermitidos

        public List<CargaMasivaDominioPermitido> ObtenerDominiosPermitidosProyecto(Guid pProyectoID)
        {
            return mEntityContext.CargaMasivaDominioPermitido.Where(x => x.ProyectoID.Equals(pProyectoID)).ToList();
        }

        public List<string> ObtenerDominiosPermitidosEfectivos(Guid pProyectoID)
        {
            List<string> urls = mEntityContext.CargaMasivaDominioPermitido.Where(x => x.ProyectoID.Equals(pProyectoID)).Select(item => item.Dominio).Distinct().ToList();

            if (pProyectoID.Equals(ProyectoAD.MetaProyecto) || urls.Count > 0)
            {
                return urls;
            }

            return mEntityContext.CargaMasivaDominioPermitido.Where(x => x.ProyectoID.Equals(ProyectoAD.MetaProyecto)).Select(x => x.Dominio).Distinct().ToList();
        }

        /// <summary>
        /// Guarda un dominio y devuelve false si ya existia
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <param name="pDominio"></param>
        /// <param name="pIdentidadID"></param>
        /// <returns></returns>
        public bool GuardarUrlPermitido(Guid pProyectoID, string pDominio, Guid pIdentidadID)
        {
            bool existe = mEntityContext.CargaMasivaDominioPermitido.Any(x => x.ProyectoID.Equals(pProyectoID) && x.Dominio.Equals(pDominio));

            if (!existe)
            {
                mEntityContext.CargaMasivaDominioPermitido.Add(new CargaMasivaDominioPermitido()
                {
                    ProyectoID = pProyectoID,
                    Dominio = pDominio,
                    FechaCreacion = DateTime.Now,
                    IdentidadID = pIdentidadID
                });

                mEntityContext.SaveChanges();
            }

            return !existe;
        }

        /// <summary>
        /// Elimina un dominio y devuelve false si no existia
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <param name="pDominio"></param>
        /// <returns></returns>
        public bool EliminarDominioPermitido(Guid pProyectoID, string pDominio)
        {
            CargaMasivaDominioPermitido cargaMasivaDominioPermitido = mEntityContext.CargaMasivaDominioPermitido.FirstOrDefault(x => x.ProyectoID.Equals(pProyectoID) && x.Dominio.Equals(pDominio));

            if (cargaMasivaDominioPermitido == null)
            {
                return false;
            }

            mEntityContext.CargaMasivaDominioPermitido.Remove(cargaMasivaDominioPermitido);
            mEntityContext.SaveChanges();

            return true;
        }

        public CargaMasivaConfiguracion ObtenerConfiguracion(Guid pProyectoID)
        {
            return mEntityContext.CargaMasivaConfiguracion.FirstOrDefault(x => x.ProyectoID.Equals(pProyectoID));
        }

        #endregion
    }
}
