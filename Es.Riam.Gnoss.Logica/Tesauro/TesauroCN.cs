using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Tesauro;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Logica.Tesauro
{
    /// <summary>
    /// Capa de negocio para tesauro
    /// </summary>
    public class TesauroCN : BaseCN, IDisposable
    {

        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public TesauroCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            TesauroAD = new TesauroAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración de base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public TesauroCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            this.TesauroAD = new TesauroAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }

        #endregion

        #region Métodos generales

        /// <summary>
        /// Obtiene el TesauroID y la CategoriaTesauroID que corresponden al nombre del tesauro y al proyecto dados
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pNombre">Nombre del tesauro</param>
        /// <returns>Lista con el TesauroID y CategoriaTesauroID. Lista null si no existen</returns>
        public List<Guid> ObtenerTesauroYCategoria(Guid pProyectoID, string pNombre)
        {
            return TesauroAD.ObtenerTesauroYCategoria(pProyectoID, pNombre);
        }

        /// <summary>
        /// Obtiene el nombre de una categoria a partir de CategoriaTesauroID
        /// </summary>
        /// <param name="pCategoriaTesauroID">CategoriaTesauroID</param>
        /// <returns>string con el nombre de la categoria</returns>
        public string ObtenerNombreCategoriaPorID(Guid pCategoriaTesauroID, string pIdioma)
        {
            return TesauroAD.ObtenerNombreCategoriaPorID(pCategoriaTesauroID, pIdioma);
        }

        public string ObtenerNombreTesauroProyOnt(Guid pProyectoID, string pOntologiaID)
        {
            return TesauroAD.ObtenerNombreTesauroProyOnt(pProyectoID, pOntologiaID);
        }

        /// <summary>
        /// Obtiene el identificador del tesauro de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Identificador de tesauro de proyecto</returns>
        public Guid ObtenerIDTesauroDeProyecto(Guid pProyectoID)
        {
            return TesauroAD.ObtenerIDTesauroDeProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene el identificador del tesauro de un usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Identificador de tesauro de usuario</returns>
        public Guid ObtenerIDTesauroDeUsuario(Guid pUsuarioID)
        {
            return TesauroAD.ObtenerIDTesauroDeUsuario(pUsuarioID);
        }

        /// <summary>
        /// Obtiene las categorias Hijas de las categorías pasadas por parámetro
        /// </summary>
        /// <param name="pListaCategorias"></param>
        /// <returns></returns>
        public List<Guid> ObtenerCategoriasHijas(List<Guid> pListaCategorias)
        {
            return TesauroAD.ObtenerCategoriasHijas(pListaCategorias);
        }

        /// <summary>
        /// Obtiene el identificador del tesauro de una organización
        /// </summary>
        /// <param name="pUsuarioID">Identificador de la organización</param>
        /// <returns>Identificador de tesauro de la organización</returns>
        public Guid ObtenerIDTesauroDeOrganizacion(Guid pOrganizacionID)
        {
            return TesauroAD.ObtenerIDTesauroDeOrganizacion(pOrganizacionID);
        }

        /// <summary>
        /// Obtiene el tesauro del proyecto cuyos identificadores se pasan por parámetro
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de tesauro</returns>
        public DataWrapperTesauro ObtenerTesauroDeProyecto(Guid pProyectoID)
        {
            return TesauroAD.ObtenerTesauroDeProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene el tesauro de la comunidad MyGnoss
        /// </summary>
        /// <returns>Dataset de tesauro</returns>
        public DataWrapperTesauro ObtenerTesauroDeProyectoMyGnoss()
        {
            return TesauroAD.ObtenerTesauroDeProyecto(ProyectoAD.MetaProyecto);
        }

        /// <summary>
        /// Obtiene el identificador de una categoría a partir de su nombre y del proyecto al que pertenece
        /// </summary>
        /// <returns>Identificador de categoría de tesauro</returns>
        public Guid ObtenerCategoriaIDPorNombreYProyecto(string pNombre, Guid pProyectoID, string pIdioma)
        {
            return TesauroAD.ObtenerCategoriaIDPorNombreYProyecto(pNombre, pProyectoID, pIdioma);
        }


        /// <summary>
        /// Actualiza el tesauro
        /// </summary>
        /// <param name="pTesauroDS">Dataset de tesauro a actualizar respecto a la base datos</param>
        /// <param name="pComprobarAutorizacion">TRUE si se debe de comprobar la autorización</param>
        public void ActualizarTesauro()
        {
            base.Actualizar();
        }

        /// <summary>
        /// Obtiene todas las sugerencias pendientes de categorias para un tesauro.
        /// </summary>
        /// <param name="pTesauroID">Identificador del tesauro</param>
        /// <returns></returns>
        public DataWrapperTesauro ObtenerSugerenciasCatDeUnTesauro(Guid pTesauroID)
        {
            return TesauroAD.ObtenerSugerenciasCatDeUnTesauro(pTesauroID);
        }

        /// <summary>
        /// Obtiene las categorías que permiten solo ciertos tipos de recurso.
        /// </summary>
        /// <param name="pTesauroID">ID de tesauro</param>
        /// <returns>DataSet con la tabla CatTesauroPermiteTipoRec</returns>
        public DataWrapperTesauro ObtenerCategoriasPermitidasPorTipoRecurso(Guid pTesauroID)
        {
            return TesauroAD.ObtenerCategoriasPermitidasPorTipoRecurso(pTesauroID);
        }

        /// <summary>
        /// Obtiene un conjunto te tesauros pasando sus IDs como parámetros
        /// </summary>
        /// <param name="pListaTesauros">identificadores de los tesauros</param>
        /// <returns>TesauroDS</returns>
        public DataWrapperTesauro ObtenerTesauroPorListaIDs(List<Guid> pListaTesauros)
        {
            return TesauroAD.ObtenerTesauroPorListaIDs(pListaTesauros);
        }

        /// <summary>
        /// Obtiene un tesauro completo a partir del identificador de tesauro pasado por parámetro, obtiene las tablas: "Tesauro","CategoriaTesauro","CategoriaTesauroPrivados" y "CatTesauroAgCatTesauro"
        /// </summary>
        /// <param name="pTesauroID">Identificador del tesauro</param>
        /// <returns>Dataset de tesauro</returns>
        public DataWrapperTesauro ObtenerTesauroCompletoPorID(Guid pTesauroID)
        {
            return TesauroAD.ObtenerTesauroCompletoPorID(pTesauroID);
        }

        /// <summary>
        /// Obtiene un tesauro a partir del identificador pasado por parámetro
        /// </summary>
        /// <param name="pTesauroID">Identificador de tesauro</param>
        /// <returns>Dataset de tesauro</returns>
        public DataWrapperTesauro ObtenerTesauroPorID(Guid pTesauroID)
        {
            List<Guid> listaIDs = new List<Guid>();
            listaIDs.Add(pTesauroID);

            return ObtenerTesauroPorListaIDs(listaIDs);
        }

        /// <summary>
        /// Obtiene un tesauro a partir del identificador de una base de recursos pasado por parámetro
        /// </summary>
        /// <param name="pBaseRecursosID">Identificador de una base de recursos</param>
        /// <returns>Dataset de tesauro</returns>
        public DataWrapperTesauro ObtenerTesauroPorBaseRecursosID(Guid pBaseRecursosID)
        {
            return TesauroAD.ObtenerTesauroPorBaseRecursosID(pBaseRecursosID);
        }


        /// <summary>
        /// Obtiene el tesauro de un usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Dataset de tesauro con los datos</returns>
        public DataWrapperTesauro ObtenerTesauroUsuario(Guid pUsuarioID)
        {
            List<Guid> listaIDs = new List<Guid>();
            listaIDs.Add(pUsuarioID);

            return ObtenerTesaurosDeListaUsuarios(listaIDs);
        }

        /// <summary>
        /// Obtiene los tesauros de una lista de usuarios
        /// </summary>
        /// <param name="pListaUsuariosID">Identificadores de usuarios</param>
        /// <returns>DataSet de tesauro con los datos</returns>
        public DataWrapperTesauro ObtenerTesaurosDeListaUsuarios(List<Guid> pListaUsuariosID)
        {
            return TesauroAD.ObtenerTesaurosDeListaUsuarios(pListaUsuariosID);
        }

        /// <summary>
        /// Obtiene el tesauro de un usuario a través de su personaID.
        /// </summary>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <returns>DataSet Tesauro de un usuario a través de su personaID</returns>
        public DataWrapperTesauro ObtenerTesauroUsuarioPorPersonaID(Guid pPersonaID)
        {
            return TesauroAD.ObtenerTesauroUsuarioPorPersonaID(pPersonaID);
        }

        /// <summary>
        /// Obtiene el tesauro de una organización. 
        /// Carga las tablas "TesauroOrganizacion", "Tesauro", "CategoriaTesauro", "CatTesauroAgCatTesauro"
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <returns>Dataset de tesauro con los datos</returns>
        public DataWrapperTesauro ObtenerTesauroOrganizacion(Guid pOrganizacionID)
        {
            List<Guid> listaIDs = new List<Guid>();
            listaIDs.Add(pOrganizacionID);

            return ObtenerTesaurosDeListaOrganizaciones(listaIDs);
        }

        /// <summary>
        /// Obtiene los tesauros de un conjunto de organizacioens
        /// </summary>
        /// <param name="pListaOrganizaciones">Lista de identificadores de organizaciones</param>
        /// <returns>TesauroDS</returns>
        public DataWrapperTesauro ObtenerTesaurosDeListaOrganizaciones(List<Guid> pListaOrganizaciones)
        {
            return TesauroAD.ObtenerTesaurosDeListaOrganizaciones(pListaOrganizaciones);
        }

        /// <summary>
        /// Comprueba si una categoría del tesauro está vinculada o no a algún elemento de Gnoss.
        /// </summary>
        /// <param name="pTesauroID">Identificador de tesauro</param>
        /// <param name="pCategoriasID">Lista de identificadores de categorías</param>
        /// <returns>TRUE si está vinculado, FALSE en caso contrario</returns>
        public bool EstanVinculadasCategoriasTesauro(Guid pTesauroID, List<Guid> pCategoriasID)
        {
            return TesauroAD.EstanVinculadasCategoriasTesauro(pTesauroID, pCategoriasID);
        }

        /// <summary>
        /// Obtiene si existen elementos vinculados con una categoría de forma no exclusiva (también están relacionados con otras categorías)
        /// </summary>
        /// <param name="pTesauroID">Identificador de tesauro</param>
        /// <param name="pCategoriasID">Lista de identificadores de categorías</param>
        /// <returns>Obtiene si existen elementos vinculados con una categoría de forma no exclusiva (también están relacionados con otras categorías)</returns>
        public bool ObtenerSiExistenElementosNoHuerfanos(Guid pTesauroID, List<Guid> pCategoriasID)
        {
            return TesauroAD.ObtenerSiExistenElementosNoHuerfanos(pTesauroID, pCategoriasID);
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
        ~TesauroCN()
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
                    if (TesauroAD != null)
                        TesauroAD.Dispose();
                }
                TesauroAD = null;
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Data adapter de tesauro
        /// </summary>
        private TesauroAD TesauroAD
        {
            get
            {
                return (TesauroAD)AD;
            }
            set
            {
                AD = value;
            }
        }

        #endregion

    }
}
