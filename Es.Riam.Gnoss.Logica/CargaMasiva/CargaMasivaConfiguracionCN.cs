using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.CargaMasiva;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Carga;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Es.Riam.Gnoss.Logica.CargaMasiva
{
    public class CargaMasivaConfiguracionCN : BaseCN, IDisposable
    {
        private static readonly string[] EsquemasPermitidos = { "http", "https" };

        #region Constructores

        public CargaMasivaConfiguracionCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<CargaMasivaConfiguracionCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            this.CargaMasivaConfiguracionAD = new CargaMasivaConfiguracionAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, loggerFactory?.CreateLogger<CargaMasivaConfiguracionAD>(), loggerFactory);
        }

        #endregion

        #region Dominios Permitidos

        public List<CargaMasivaDominioPermitido> ObtenerDominiosPermitidosProyecto(Guid pProyectoID)
        {
            return CargaMasivaConfiguracionAD.ObtenerDominiosPermitidosProyecto(pProyectoID);
        }

        public List<string> ObtenerDominiosPermitidosEfectivos(Guid pProyectoID)
        {
            return CargaMasivaConfiguracionAD.ObtenerDominiosPermitidosEfectivos(pProyectoID);
        }

        public bool GuardarUrlPermitido(Guid pProyectoID, string pDominio, Guid pIdentidadID)
        {
            return CargaMasivaConfiguracionAD.GuardarUrlPermitido(pProyectoID, NormalizarDominio(pDominio), pIdentidadID);
        }

        public bool EliminarDominioPermitido(Guid pProyectoID, string pDominio)
        {
            return CargaMasivaConfiguracionAD.EliminarDominioPermitido(pProyectoID, NormalizarDominio(pDominio));
        }

        public bool EsUrlPermitida(Guid pProyectoID, string pUrl, out string pError)
        {
            pError = null;

            if (!ComprobarEsquemaUrl(pUrl, out Uri uri))
            {
                pError = "URL no válida o esquema no permitido";
                return false;
            }

            string host = NormalizarDominio(uri.Host);

            if (!ObtenerDominiosPermitidosEfectivos(pProyectoID).Contains(host))
            {
                pError = "El dominio no está en la whitelist del proyecto.";
                return false;
            }

            if (!TodasLasIPsSonPublicas(host))
            {
                pError = "El dominio resuelve a una dirección IP no permitida.";
                return false;
            }

            return true;
        }

        public static bool ComprobarEsquemaUrl(string pUrl, out Uri pUri)
        {
            if (!Uri.TryCreate(pUrl, UriKind.Absolute, out pUri) || !EsquemasPermitidos.Contains(pUri.Scheme, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Metodos Privados

        private static string NormalizarDominio(string pDominio)
        {
            return pDominio.Trim().ToLowerInvariant();
        }

        private static bool TodasLasIPsSonPublicas(string pHost)
        {
            IPAddress[] direcciones;
            try
            {
                direcciones = Dns.GetHostAddresses(pHost);
            }
            catch
            {
                return false;
            }

            return direcciones.Length > 0 && direcciones.All(EsIPPublica);
        }

        private static bool EsIPPublica(IPAddress pIp)
        {
            if (pIp.IsIPv4MappedToIPv6)
            {
                pIp = pIp.MapToIPv4();
            }

            if (IPAddress.IsLoopback(pIp) || pIp.IsIPv6LinkLocal || pIp.IsIPv6SiteLocal || pIp.IsIPv6Multicast)
            {
                return false;
            }

            if (pIp.AddressFamily == AddressFamily.InterNetwork)
            {
                byte[] b = pIp.GetAddressBytes();
                if (b[0] == 10) return false;
                if (b[0] == 172 && b[1] >= 16 && b[1] <= 31) return false;
                if (b[0] == 192 && b[1] == 168) return false;
                if (b[0] == 169 && b[1] == 254) return false;
                if (b[0] == 127) return false;
                if (b[0] == 0) return false;
            }
            else if (pIp.AddressFamily == AddressFamily.InterNetworkV6)
            {
                byte[] b = pIp.GetAddressBytes();
                if ((b[0] & 0xFE) == 0xFC) return false;
            }

            return true;
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool mDisposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~CargaMasivaConfiguracionCN()
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

                if (pDisposing && (this.CargaMasivaConfiguracionAD != null))
                {
                    //Libero todos los recursos administrados que he añadido a esta clase
                    CargaMasivaConfiguracionAD.Dispose();
                }
                CargaMasivaConfiguracionAD = null;
            }
        }

        #endregion

        #region Propiedades 

        /// <summary>
        /// AD de CargaMasivaConfiguracion.
        /// </summary>
        protected CargaMasivaConfiguracionAD CargaMasivaConfiguracionAD
        {
            get
            {
                return (CargaMasivaConfiguracionAD)AD;
            }
            set
            {
                this.AD = value;
            }
        }

        #endregion
    }
}
