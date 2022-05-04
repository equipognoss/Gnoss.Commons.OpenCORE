using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.Common;

namespace Es.Riam.Gnoss.AD.BASE_BD
{

    /// <summary>
    /// Data adapter para las búsquedas del metabuscador del modelo BASE
    /// </summary>
    public class MetaBuscadorBaseAD : BaseComunidadAD
    {
        #region Constructor

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración de la base de datos BASE</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        /// <param name="pTablaBaseProyectoID">Identificador numerico del proyecto (-1 si no se va a actualizar tablas de proyecto)</param>
        public MetaBuscadorBaseAD(string pFicheroConfiguracionBD, int pTablaBaseProyectoID, LoggingService loggingService, EntityContext entityContext, EntityContextBASE entityContextBASE, ConfigService configService)
            : base(pTablaBaseProyectoID, loggingService, entityContext, entityContextBASE, configService)
        {
            mNombreTablaCOMUNIDADES = "COMUNIDAD";
            mNombreTablaCOM_USU_PRIV = "COM_USU_PRIV";

            mNombreTablaDAFO_COM = "DAFO_COM";
            mNombreTablaDAFO_COM_USU = "DAFO_COM_USU";
            mNombreTablaDAFO_COM_USU_PRIV = "DAFO_COM_USU_PRIV";

            mNombreTablaCOM_BLOG = "COM_BLOG";

            mNombreTablaCOM_PER_ORG = "COM_PER_ORG";
            mNombreTablaCOM_PER_ORG_VI = "COM_PER_ORG_VI";

            if (pTablaBaseProyectoID > -1)
            {
                if (BusquedaEnMyGnoss)
                {
                    mNombreTablaCOMUNIDADES = "REC_PUBLICOS_MYGNOSS";
                    mNombreTablaCOM_USU_PRIV = "REC_PRIVADOS_COM_USU";
                }
                else
                {
                    mNombreTablaCOMUNIDADES = "COMUNIDAD_000000000000000".Substring(0, 25 - mTablaBaseProyectoID.ToString().Length) + mTablaBaseProyectoID.ToString();
                    mNombreTablaCOM_USU_PRIV = "COM_USU_PRIV_000000000000".Substring(0, 25 - mTablaBaseProyectoID.ToString().Length) + mTablaBaseProyectoID.ToString();
                }

                mNombreTablaDAFO_COM = "DAFO_COM_0000000000000000".Substring(0, 25 - mTablaBaseProyectoID.ToString().Length) + mTablaBaseProyectoID.ToString();
                mNombreTablaDAFO_COM_USU = "DAFO_COM_USU_000000000000".Substring(0, 25 - mTablaBaseProyectoID.ToString().Length) + mTablaBaseProyectoID.ToString();
                mNombreTablaDAFO_COM_USU_PRIV = "DAFO_COM_USU_PRIV_0000000".Substring(0, 25 - mTablaBaseProyectoID.ToString().Length) + mTablaBaseProyectoID.ToString();

                mNombreTablaCOM_BLOG = "COM_BLOG_0000000000000".Substring(0, 22 - mTablaBaseProyectoID.ToString().Length) + mTablaBaseProyectoID.ToString();

                mNombreTablaCOM_PER_ORG = "COM_PER_ORG_0000000000000".Substring(0, 25 - mTablaBaseProyectoID.ToString().Length) + mTablaBaseProyectoID.ToString();
                mNombreTablaCOM_PER_ORG_VI = "COM_PER_ORG_VI_0000000000".Substring(0, 25 - mTablaBaseProyectoID.ToString().Length) + mTablaBaseProyectoID.ToString();
            }

        }

        #endregion

        #region Metodos heredados
        /// <summary>
        /// Crea una tabla en función de un tipo de conuslta
        /// <param name="pTipoConsulta">Tipo de consulta</param>
        /// </summary>
        protected override void CrearTabla(TiposConsultaObtenerTags pTipoConsulta)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// Obtiene el sql correspondiente a un tipo de consulta
        /// </summary>
        /// <param name="pTipoConsulta">Tipo de consulta</param>
        /// <returns></returns>
        protected internal override string ObtenerNombreTablaPorTipoConsulta(TiposConsultaObtenerTags pTipoConsulta)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Obtiene el sql correspondiente a un tipo de consulta
        /// </summary>
        /// <param name="pTipoConsulta">Tipo de consulta</param>
        /// <returns></returns>
        protected override string ObtenerNombreTablaPorTipoConsultaConIDTags(TiposConsultaObtenerTags pTipoConsulta)
        {
            switch (pTipoConsulta)
            {
                case TiposConsultaObtenerTags.RecursosComunidad:
                    if ((BusquedaEnMyGnoss) && (mNombreTablaCOMUNIDADES.StartsWith("COMUNIDAD")))
                    {
                        return "REC_PUBLICOS_MYGNOSS";
                    }
                    else
                    {
                        return mNombreTablaCOMUNIDADES;
                    }
                case TiposConsultaObtenerTags.RecursosComunidadPrivada:
                    if (BusquedaEnMyGnoss)
                    {
                        return "REC_PRIVADOS_COM_USU";
                    }
                    else
                    {
                        return mNombreTablaCOM_USU_PRIV;
                    }
                case TiposConsultaObtenerTags.RecursosComunidadSoloPrivados:
                    return "REC_PRIVADOS_COM_USU";
                case TiposConsultaObtenerTags.DafosDeComunidad:
                    return mNombreTablaDAFO_COM;
                case TiposConsultaObtenerTags.TodosDafosDeComunidadParaUsuario:
                    return mNombreTablaDAFO_COM_USU;
                case TiposConsultaObtenerTags.DafosDeComunidadFiltradosParaUsuario:
                    return mNombreTablaDAFO_COM_USU_PRIV;
                case TiposConsultaObtenerTags.PersonasYOrganizacionesComunidad:
                case TiposConsultaObtenerTags.PersonasDeOrganizacionParticipanComunidad:
                    return mNombreTablaCOM_PER_ORG;
                case TiposConsultaObtenerTags.PersonasYOrganizacionesVisiblesInvitadoComunidad:
                    return mNombreTablaCOM_PER_ORG_VI;
                case TiposConsultaObtenerTags.BlogsYEntradasBlogDeComunidad:
                    return mNombreTablaCOM_BLOG;
            }

            return ObtenerNombreTablaPorTipoConsulta(pTipoConsulta);
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de tags
        /// </summary>
        /// <returns>Devuelve un dataset con los elementos de la cola cargados</returns>
        /// <param name="pEstadoInferior">Estado mínimo de los elementos de la cola</param>
        /// <param name="pEstadoSuperior">Estado máximo de los elementos de la cola</param>
        /// <param name="pTiposElementos">Tipo de los elementos a obtener (Agregado, eliminado, ...). Null para obtener todos</param>
        /// <param name="pPrioridadBase">Prioridad Base</param>
        ///<param name="pNumMaxItems">Numero maximo de itesm a traer</param>
        public override DataSet ObtenerElementosColaPendientes(EstadosColaTags pEstadoInferior, EstadosColaTags pEstadoSuperior, TiposElementosEnCola? pTiposElementos, int pNumMaxItems)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// Comprueba si existe la tabla de comunidades. Si no existe la crea 
        /// </summary>
        /// <param name="pCrearTablaSiNoExiste">Verdad si se debe crear la tabla en caso de que no exista</param>
        /// <param name="pTipoConsulta">Tipo de consulta que se va a realizar</param>
        /// <returns>Verdad si la tabla existe (o ha sido recién creada).</returns>
        public override bool VerificarExisteTabla(TiposConsultaObtenerTags pTipoConsulta, bool pCrearTablaSiNoExiste)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Obtiene el nombre correcto de la tabla en la base de datos
        /// </summary>
        /// <param name="pNombreTabla">Nombre de la tabla en el DataSet</param>
        /// <returns></returns>
        protected override string ObtenerNombreCorrectoTabla(string pNombreTabla)
        {
            return base.ObtenerNombreCorrectoTabla(pNombreTabla);
        }

        #endregion
        /// <summary>
        /// Obtiene la conexión a la base de datos Master
        /// </summary>
        protected override DbConnection ConexionMaster
        {
            get
            {
                var conexion = mEntityContextBASE.Database.GetDbConnection();
                if (conexion.State != ConnectionState.Open)
                {
                    conexion.Open();
                }
                return conexion;
            }
        }
    }
}
