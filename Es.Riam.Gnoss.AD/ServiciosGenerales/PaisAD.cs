using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.AD.ServiciosGenerales
{
    /// <summary>
    /// Data adapter para países
    /// </summary>
    public class PaisAD : BaseAD
    {
        #region Consultas
        string sqlSelectProvinciasDePais;
        string sqlSelectProvincias;
        string sqlSelectPaises;
        #endregion

        #region PaisDataAdapter
        string sqlPaisInsert;
        string sqlPaisDelete;
        string sqlPaisModify;
        #endregion

        #region ProvinciaDataAdapter
        string sqlProvinciaInsert;
        string sqlProvinciaDelete;
        string sqlProvinciaModify;
        #endregion

        private EntityContext mEntityContext;

        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public PaisAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuración de conexión a base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de la conexión a base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public PaisAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
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
            sqlSelectProvinciasDePais = "SELECT " + IBD.CargarGuid("Provincia.ProvinciaID") + ", Provincia.Nombre, Provincia.CP, " + IBD.CargarGuid("Provincia.PaisID") + " FROM Pais INNER JOIN Provincia ON Pais.PaisID = Provincia.PaisID WHERE (Provincia.PaisID = " + IBD.GuidParamValor("PaisID") + ") ORDER BY Provincia.Nombre";
            sqlSelectProvincias = "SELECT " + IBD.CargarGuid("ProvinciaID") + " , Nombre, CP, " + IBD.CargarGuid("PaisID") + "  FROM Provincia ORDER BY Nombre";
            sqlSelectPaises = "SELECT " + IBD.CargarGuid("PaisID") + ", Nombre FROM Pais ORDER BY Nombre";
            #endregion

            #region PaisDataAdapter
            sqlPaisInsert = IBD.ReplaceParam("INSERT INTO Pais (PaisID, Nombre) VALUES (" + IBD.GuidParamColumnaTabla("PaisID") + ", @Nombre)");
            sqlPaisDelete = IBD.ReplaceParam("DELETE FROM Pais WHERE ((PaisID = " + IBD.GuidParamColumnaTabla("O_PaisID") + ") AND (Nombre = @O_Nombre))");
            sqlPaisModify = IBD.ReplaceParam("UPDATE Pais SET PaisID = " + IBD.GuidParamColumnaTabla("PaisID") + ", Nombre = @Nombre WHERE ((PaisID = " + IBD.GuidParamColumnaTabla("O_PaisID") + ") AND (Nombre = @O_Nombre))");
            #endregion

            #region ProvinciaDataAdapter
            sqlProvinciaInsert = IBD.ReplaceParam("INSERT INTO Provincia (ProvinciaID, Nombre, CP, PaisID) VALUES (" + IBD.GuidParamColumnaTabla("ProvinciaID") + ",@Nombre,@CP," + IBD.GuidParamColumnaTabla("PaisID") + ")");
            sqlProvinciaDelete = IBD.ReplaceParam("DELETE FROM Provincia WHERE (ProvinciaID = " + IBD.GuidParamColumnaTabla("O_ProvinciaID") + ") AND (PaisID = " + IBD.GuidParamColumnaTabla("O_PaisID") + ") AND (CP = @O_CP OR @O_CP IS NULL AND CP IS NULL) AND (Nombre = @O_Nombre OR @O_Nombre IS NULL AND Nombre IS NULL)");
            sqlProvinciaModify = IBD.ReplaceParam("UPDATE Provincia SET ProvinciaID = " + IBD.GuidParamColumnaTabla("ProvinciaID") + ", Nombre = @Nombre, CP = @CP, PaisID = " + IBD.GuidParamColumnaTabla("PaisID") + " WHERE (ProvinciaID = " + IBD.GuidParamColumnaTabla("O_ProvinciaID") + ") AND (PaisID = " + IBD.GuidParamColumnaTabla("O_PaisID") + ") AND (CP = @O_CP OR @O_CP IS NULL AND CP IS NULL) AND (Nombre = @O_Nombre OR @O_Nombre IS NULL AND Nombre IS NULL)");
            #endregion
        }

        #endregion

        #region Públicos
        /// <summary>
        /// Obtiene todos los paises
        /// </summary>
        /// <returns>Lista de paises</returns>
        public DataWrapperPais ObtenerPaises()
        {
            DataWrapperPais dataWrapperPais = new DataWrapperPais();

            dataWrapperPais.ListaPais = mEntityContext.Pais.OrderBy(item => item.Nombre).ToList();

            return dataWrapperPais;
        }

        /// <summary>
        /// Obtiene todas las provincias
        /// </summary>
        /// <returns>Lista de provincias</returns>
        public DataWrapperPais ObtenerProvincias()
        {
            DataWrapperPais dataWrapperPais = new DataWrapperPais();

            dataWrapperPais.ListaProvincia = mEntityContext.Provincia.OrderBy(item => item.Nombre).ToList();

            return dataWrapperPais;
        }

        /// <summary>
        /// Obtiene todos los países y todas las provincias
        /// </summary>
        /// <returns>Lista de paises</returns>
        public DataWrapperPais ObtenerPaisesProvincias()
        {
            DataWrapperPais dataWrapperPais = new DataWrapperPais();

            //Pais
            dataWrapperPais.ListaPais = mEntityContext.Pais.OrderBy(item => item.Nombre).ToList();

            //Provincia
            dataWrapperPais.ListaProvincia = mEntityContext.Provincia.OrderBy(item => item.Nombre).ToList();

            return dataWrapperPais;
        }

        /// <summary>
        /// Obtiene las provincias de un pais
        /// </summary>
        /// <param name="pPaisID">Identificador del pais</param>
        /// <returns>Lista de provincias del pais</returns>
        public DataWrapperPais ObtenerProvinciasDePais(Guid pPaisID)
        {
            DataWrapperPais dataWrapperPais = new DataWrapperPais();

            dataWrapperPais.ListaPais = mEntityContext.Pais.OrderBy(item => item.Nombre).ToList();

            dataWrapperPais.ListaProvincia = mEntityContext.Provincia.Where(item => item.PaisID.Equals(pPaisID)).OrderBy(item => item.Nombre).ToList();

            return dataWrapperPais;
        }

        /// <summary>
        /// Obtiene el identificador del pais a partir de su nombre
        /// </summary>
        /// <param name="pNombrePais">Nombre del país</param>
        /// <returns>Identificador del país. Guid.Empty en caso de no encontrarlo</returns>
        public Guid ObtenerPaisIDPorNombre(string pNombrePais)
        {
            return mEntityContext.Pais.Where(item => item.Nombre.Equals(pNombrePais)).Select(item => item.PaisID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el nombre del país
        /// </summary>
        /// <param name="pPaisID">Identificador del país</param>
        /// <returns>Nombre del país</returns>
        public string ObtenerNombrePais(Guid pPaisID)
        {
            return mEntityContext.Pais.Where(item => item.PaisID.Equals(pPaisID)).Select(item => item.Nombre).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el nombre de la provincia
        /// </summary>
        /// <param name="pProvinciaID">Identificador de la provincia</param>
        /// <returns>Nombre de la provincia</returns>
        public string ObtenerNombreProvincia(Guid pPaisID, Guid pProvinciaID)
        {
            return mEntityContext.Provincia.Where(item => item.PaisID.Equals(pPaisID) && item.ProvinciaID.Equals(pProvinciaID)).Select(item => item.Nombre).FirstOrDefault();
        }

        /// <summary>
        /// Actualiza la base de datos
        /// </summary>
        public void ActualizarPaises()
        {
            ActualizarBaseDeDatosEntityContext();
        }

        #endregion
        #endregion
    }
}
