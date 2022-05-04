using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;

namespace Es.Riam.Gnoss.Logica.BASE_BD
{

    /// <summary>
    /// Capa de lógica del metabuscador 
    /// </summary>
    public class MetaBuscadorBaseCN : BaseComunidadCN
    {
        #region Constructor

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración de la base de datos BASE</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        /// <param name="pTablaBaseProyectoID">Identificador numerico del proyecto (-1 si no se va a actualizar tablas de proyecto)</param>
        public MetaBuscadorBaseCN(string pFicheroConfiguracionBD, int pTablaBaseProyectoID, EntityContext entityContext, LoggingService loggingService, EntityContextBASE entityContextBASE, ConfigService configService)
            : base(pFicheroConfiguracionBD, pTablaBaseProyectoID, entityContext, loggingService, entityContextBASE, configService)
        {
            this.BaseComunidadAD = new MetaBuscadorBaseAD(pFicheroConfiguracionBD, pTablaBaseProyectoID, loggingService, entityContext, entityContextBASE, configService);
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Casting mediante una propiedad para poder acceder a los metodos específicos de la clase
        /// </summary>
        public MetaBuscadorBaseAD MetabuscadorBaseAD
        {
            get
            {
                return (MetaBuscadorBaseAD)BaseComunidadAD;
            }
        }

        /// <summary>
        /// Obtiene o establece la identidad con la que está conectado el usuario actual
        /// </summary>
        public Guid IdentidadUsuarioConectado
        {
            get
            {
                return this.BaseComunidadAD.IdentidadUsuarioConectado;
            }
            set
            {
                this.BaseComunidadAD.IdentidadUsuarioConectado = value;
            }
        }

        /// <summary>
        /// Obtiene o establece si la búsqueda se realiza en MyGnoss o en una comunidad
        /// </summary>
        public bool BusquedaEnMyGnoss
        {
            get
            {
                return MetabuscadorBaseAD.BusquedaEnMyGnoss;
            }
            set
            {
                MetabuscadorBaseAD.BusquedaEnMyGnoss = value;
            }
        }

        #endregion

        #region Metodos heredados

        /// <summary>
        /// Elimina los elementos viejos
        /// </summary>
        public override void EliminarElementosColaProcesadosViejos()
        {

        }

        #endregion
    }
}
