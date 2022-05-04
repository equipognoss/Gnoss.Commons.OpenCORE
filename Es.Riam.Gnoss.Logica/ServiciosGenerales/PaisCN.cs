using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;

namespace Es.Riam.Gnoss.Logica.ServiciosGenerales
{
    /// <summary>
    /// Lógica referente a pais
    /// </summary>
    public class PaisCN : BaseCN, IDisposable
    {

        #region Constructores
        /// <summary>
        /// Constructor de PaisCN
        /// </summary>
        public PaisCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            PaisAD = new PaisAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }
        #endregion

        #region Métodos generales
        #region Públicos

        /// <summary>
        /// Recupera todos los paises
        /// </summary>
        /// <returns>Lista de paises</returns>
        public DataWrapperPais ObtenerPaises()
        {
            return PaisAD.ObtenerPaises();
        }

        /// <summary>
        /// Recupera todas las provincias
        /// </summary>
        /// <returns>Lista de provincias</returns>
        public DataWrapperPais ObtenerProvincias()
        {
            return PaisAD.ObtenerProvincias();
        }


        /// <summary>
        /// Recupera todos los paises y todas las provincias
        /// </summary>
        /// <returns>Países y provincias</returns>
        public DataWrapperPais ObtenerPaisesProvincias()
        {
            return PaisAD.ObtenerPaisesProvincias();
        }


        /// <summary>
        /// Recupera las provincias de un pais
        /// </summary>
        /// <param name="pPaisID">Identificador del pais</param>
        /// <returns>Lista de provincias del pais</returns>
        public DataWrapperPais ObtenerProvinciasDePais(Guid pPaisID)
        {
            return PaisAD.ObtenerProvinciasDePais(pPaisID);
        }

        /// <summary>
        /// Obtiene el identificador del pais a partir de su nombre
        /// </summary>
        /// <param name="pNombrePais">Nombre del país</param>
        /// <returns>Identificador del país. Guid.Empty en caso de no encontrarlo</returns>
        public Guid ObtenerPaisIDPorNombre(string pNombrePais)
        {
            return PaisAD.ObtenerPaisIDPorNombre(pNombrePais);
        }

        /// <summary>
        /// Obtiene el nombre del país
        /// </summary>
        /// <param name="pPaisID">Identificador del país</param>
        /// <returns></returns>
        public string ObtenerNombrePais(Guid pPaisID)
        {
            return PaisAD.ObtenerNombrePais(pPaisID);
        }

        /// <summary>
        /// Obtiene el nombre de la provincia
        /// </summary>
        /// <param name="pProvinciaID">Identificador de la provincia</param>
        /// <returns>Nombre de la provincia</returns>
        public string ObtenerNombreProvincia(Guid pPaisID, Guid pProvinciaID)
        {
            return PaisAD.ObtenerNombreProvincia(pPaisID, pProvinciaID);
        }

        #endregion

        #endregion

        #region Dispose


        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~PaisCN()
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
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                if (disposing)
                {
                    //Libero todos los recursos administrados que he añadido a esta clase
                    if (PaisAD != null)
                    {
                        PaisAD.Dispose();
                    }
                }

                PaisAD = null;

            }
        }

        #endregion

        #region Propiedades

        private PaisAD PaisAD
        {
            get
            {
                return (PaisAD)AD;
            }
            set
            {
                AD = value;
            }
        }

        #endregion

    }
}
