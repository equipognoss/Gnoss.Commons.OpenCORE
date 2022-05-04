using System.Data.Common;
using System.Data;
using Es.Riam.Util;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.AD.EntityModel;

namespace Es.Riam.Gnoss.AD.ServiciosGenerales
{
    /// <summary>
    /// Data adapter para CNAE
    /// </summary>
    public class CnaeAD : BaseAD
    {
        #region Consultas
        string sqlSelectCnaePorId;
        string sqlSelectCnae;
        #endregion

        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public CnaeAD(UtilPeticion utilPeticion, Conexion conexion, Error error, UtilTrazas utilTrazas, EntityContext entityContext)
            : base(utilPeticion, conexion, error, utilTrazas, entityContext)
        {
            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuración de conexión a base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de la conexión a base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public CnaeAD(string pFicheroConfiguracionBD, UtilPeticion utilPeticion, Conexion conexion, Error error, UtilTrazas utilTrazas, EntityContext entityContext)
            : base(pFicheroConfiguracionBD, utilPeticion, conexion, error, utilTrazas, entityContext)
        {
            this.CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Métodos Generales

        #region Privados

        /// <summary>
        /// En caso de que se utilice el GnossConfig.xml por defecto se sigue utilizando el IBD estático
        /// </summary>
        private void CargarConsultasYDataAdapters()
        {
            this.CargarConsultasYDataAdapters(IBD);
        }

        /// <summary>
        /// En caso de que se utilice un GnossConfig.xml que no es el de por defecto se pasa un objeto IBaseDatos creado con respecto
        /// al fichero de configuracion que se ha apsado como parámetro
        /// </summary>
        /// <param name="IBD">Objecto IBaseDatos para el archivo pasado al constructor del AD</param>
        private void CargarConsultasYDataAdapters(IBaseDatos IBD)
        {
            #region Consultas
            sqlSelectCnaePorId = IBD.ReplaceParam("SELECT CnaeID, CnaeSuperiorId, Descripcion FROM Cnae WHERE (CnaeID = @cnaeID)");
            sqlSelectCnae = IBD.ReplaceParam("SELECT CnaeID, CnaeSuperiorId, Descripcion FROM Cnae");
            #endregion
        }

        #endregion

        /// <summary>
        /// Obtiene todas los CNAEs del sistema
        /// </summary>
        /// <returns>CNAEs</returns>
        public virtual CnaeDS ObtenerCnaes()
        {
            CnaeDS cnaeDS = new CnaeDS();
            DbCommand commandsqlSelectCnae = ObtenerComando(sqlSelectCnae);
            CargarDataSet(commandsqlSelectCnae, cnaeDS, "Cnae");
            return cnaeDS;
        }

        /// <summary>
        /// Obtiene un CNAE a partir de su Identificador
        /// </summary>
        /// <param name="pCnaeID">Identificador del CNAE</param>
        /// <returns>CNAE</returns>
        public virtual CnaeDS ObtenerCnaePorID(string pCnaeID)
        {
            CnaeDS cnaeDS = new CnaeDS();
            DbCommand commandsqlSelectCnaePorId = ObtenerComando(sqlSelectCnaePorId);
            AgregarParametro(commandsqlSelectCnaePorId, IBD.ToParam("cnaeID"), DbType.String, pCnaeID);
            CargarDataSet(commandsqlSelectCnaePorId, cnaeDS, "Cnae");
            return cnaeDS;
        }

        #endregion
    }
}
