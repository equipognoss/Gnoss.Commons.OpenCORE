using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Logica.Parametro
{
    /// <summary>
    /// Lógica de parámetro.
    /// </summary>
    public class ParametroCN : BaseCN, IDisposable
    {
        #region Constructor

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ParametroCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            this.ParametroAD = new ParametroAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public ParametroCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            ParametroAD = new ParametroAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
            mFicheroConfiguracionBD = pFicheroConfiguracionBD;
        }

        #endregion

        #region Métodos generales

        /// <summary>
        /// Obtiene los parámetros de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <returns>Lista con los parámetros de la tabla ParametroProyecto de un proyecto</returns>
        public Dictionary<string, string> ObtenerParametrosProyecto(Guid pProyectoID)
        {
            return ParametroAD.ObtenerParametrosProyecto(pProyectoID, null);
        }

        /// <summary>
        /// Obtiene La lista de los Servicios Web de un proyecto
        /// </summary>
        /// <param name="pParametro">ID de proyecto</param>
        /// <returns>Lista con los servicios Web de la tabla ProyectoServicioWeb de todos los proyectos proyecto</returns>
        public List<ProyectoServicioWeb> ObtenerProyectoServicioWeb(Guid pParametro)
        {
            return ParametroAD.ObtenerProyectoServicioWeb(pParametro);
        }

        public void GuardarFilasProyectoServicioWeb(List<ProyectoServicioWeb> pListaServicios, Guid pParametro)
        {
            ParametroAD.GuardarFilasProyectoServicioWeb(pListaServicios, pParametro);
        }



        /// <summary>
        /// Obtiene un parámetro concreto de todos los proyectos.
        /// </summary>
        /// <param name="pParametro">ID de proyecto</param>
        /// <returns>Lista con los parámetros de la tabla ParametroProyecto de todos los proyectos proyecto</returns>
        public Dictionary<Guid, string> ObtenerParametroDeProyectos(string pParametro)
        {
            return ParametroAD.ObtenerParametroDeProyectos(pParametro);
        }

        /// <summary>
        /// Obtiene los parámetros de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <param name="pOrganizacionID">ID de la organización del proyecto</param>
        /// <returns>Lista con los parámetros de la tabla ParametroProyecto de un proyecto</returns>
        public Dictionary<string, string> ObtenerParametrosProyecto(Guid pProyectoID, Guid pOrganizacionID)
        {
            return ParametroAD.ObtenerParametrosProyecto(pProyectoID, pOrganizacionID);
        }
        /// <summary>
        /// Obtiene la configuracion de autocompletar de un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <returns>Lista con la configuracion de autocompletar de un proyecto</returns>
        public List<string> ObtenerConfigAutocompletar(Guid pProyectoID)
        {
            return ParametroAD.ObtenerConfigAutocompletar(pProyectoID);
        }
        /// <summary>
        /// Actualiza en la tabla ConfigAutocompletarProy el nuevo valor dado
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <param name="configAuto">Lista de nuevos valores para el proyecto</param>
        public void ActualizarConfigAutocompletar(Guid pProyectoID, List<string> configAuto)
        {
            ParametroAD.ActualizarConfigAutocompletar(pProyectoID, configAuto);
        }
        /// <summary>
        /// Obtiene la configuracion de los valores de search de la tabla ConfigSearchProy
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <returns>Lista con la configuracion de search de un proyecto</returns>
        public List<string> ObtenerConfigSearch(Guid pProyectoID)
        {
            return ParametroAD.ObtenerConfigSearch(pProyectoID);
        }
        /// <summary>
        /// Actualiza en la tabla ConfigSearchProy el nuevo valor dado
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <param name="configSearch">Lista de nuevos valores para el proyecto</param>
        public void ActualizarConfigSearch(Guid pProyectoID, List<string> configSearch)
        {
            ParametroAD.ActualizarConfigSearch(pProyectoID, configSearch);
        }
        /// <summary>
        /// Obtiene los parámetros de ConfiguracionEnvioCorreo de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <returns>Lista con los parámetros de la tabla ConfiguracionEnvioCorreo de un proyecto</returns>
        public ConfiguracionEnvioCorreo ObtenerConfiguracionEnvioCorreo(Guid pProyectoID)
        {
            return ParametroAD.ObtenerConfiguracionEnvioCorreo(pProyectoID);
        }

        /// <summary>
        /// Obtiene los parámetros de ConfiguracionEnvioCorreo de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <returns>Lista con los parámetros de la tabla ConfiguracionEnvioCorreo de un proyecto</returns>
        public ConfiguracionEnvioCorreo ObtenerFilaConfiguracionEnvioCorreo(Guid pProyectoID)
        {
            return ParametroAD.ObtenerFilaConfiguracionEnvioCorreo(pProyectoID);
        }

        /// <summary>
        /// Guarda los parámetros de ConfiguracionEnvioCorreo de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">parámetros de la tabla ConfiguracionEnvioCorreo de un proyecto</</param>
        public void GuardarFilaConfiguracionEnvioCorreo(ConfiguracionEnvioCorreo pFilaConfiguracion, bool pEsFilaNueva)
        {
            ParametroAD.GuardarFilaConfiguracionEnvioCorreo(pFilaConfiguracion, pEsFilaNueva);
        }

        /// <summary>
        /// Guarda los parámetros de ConfiguracionEnvioCorreo de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">parámetros de la tabla ConfiguracionEnvioCorreo de un proyecto</</param>
        public void BorrarFilaConfiguracionEnvioCorreo(Guid pProyectoID)
        {
            ParametroAD.BorrarFilaConfiguracionEnvioCorreo(pProyectoID);
        }

        /// <summary>
        /// Obtiene la lista de proyectos (TablaBaseProyectoID) que tienen grafo de dbpedia
        /// </summary>
        /// <returns></returns>
        public List<int> ObtenerListaTablaBaseProyectoIDConGrafoDbPedia()
        {
            return ParametroAD.ObtenerListaTablaBaseProyectoIDConGrafoDbPedia();
        }

        /// <summary>
        /// Obtiene una lista con los proyectos en los que se deben enviar las notificaciones de las suscripciones
        /// </summary>
        /// <returns></returns>
        public List<Guid> ObtenerListaProyectosConNotificacionesDeSuscripciones()
        {
            return ParametroAD.ObtenerListaProyectosConNotificacionesDeSuscripciones();
        }

        /// <summary>
        /// Obtiene el tamaño del pool para redis configurado para un dominio
        /// </summary>
        /// <param name="pDominio">dominio a obtener su configuración</param>
        /// <returns>Obtiene el tamaño configurado para este dominio, 0 si no hay nada configurado</returns>
        public int ObtenerTamanioPoolRedisParaDominio(string pDominio)
        {
            return ParametroAD.ObtenerTamanioPoolRedisParaDominio(pDominio);
        }

        /// <summary>
        /// Obtiene los idiomas configurados para un dominio
        /// </summary>
        /// <param name="pDominio">dominio a obtener su configuración</param>
        /// <returns>Obtiene los idiomas configurados para un dominio</returns>
        public Dictionary<string, string> ObtenerIdiomasPorDominio(string pDominio)
        {
            return ParametroAD.ObtenerIdiomasPorDominio(pDominio);
        }

        /// <summary>
        /// Obtiene los idiomas configurados para todos los dominios de la plataforma
        /// </summary>
        /// <returns>Obtiene los idiomas configurados para todos los dominios de la plataforma</returns>
        public Dictionary<string, string> ObtenerIdiomasDeTodosLosDominios()
        {
            return ParametroAD.ObtenerIdiomasDeTodosLosDominios();
        }

        /// <summary>
        /// Obtiene el nombrecorto de las comunidades que no tienen nombrecorto en la URL
        /// </summary>
        /// <returns></returns>
        public List<string> ObtenerNombresDeProyectosSinNombreCortoEnURL()
        {
            return ParametroAD.ObtenerNombresDeProyectosSinNombreCortoEnURL();
        }

        /// <summary>
        /// Indica la ruta relativa a los estilos, si no es el ID del proyecto (ej: "EcosistemaDidactalia")
        /// </summary>
        /// <returns></returns>
        public string ObtenerPathEstilos(Guid pProyectoID)
        {
            return ParametroAD.ObtenerPathEstilos(pProyectoID);
        }

        /// <summary>
        /// Indica los segundos a esperar para la siguiente comprobación en ServiceBus
        /// </summary>
        /// <returns></returns>
        public int ObtenerSegundosEntrePeticionServiceBus(Guid pProyectoID)
        {
            return ParametroAD.ObtenerSegundosEntrePeticionServiceBus(pProyectoID);
        }

        /// <summary>
        /// Indica los reintentos para coger el fichero configurado con ServiceBus
        /// </summary>
        /// <returns></returns>
        public int ObtenerReintentosObtenerFichero(Guid pProyectoID)
        {
            return ParametroAD.ObtenerReintentosObtenerFichero(pProyectoID);
        }


        #region Actualización

        /// <summary>
        /// Inserta o actualiza un parametro y su valor para un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <param name="pOrganizacionID">ID de la organización del proyecto</param>
        /// <param name="pParametro">Parámetro</param>
        /// <param name="pValor">Valor</param>
        public void ActualizarParametroEnProyecto(Guid pProyectoID, Guid pOrganizacionID, string pParametro, string pValor)
        {
            ParametroAD.ActualizarParametroEnProyecto(pProyectoID, pOrganizacionID, pParametro, pValor);
        }

        /// <summary>
        /// Borra un parametro de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <param name="pOrganizacionID">ID de la organización del proyecto</param>
        /// <param name="pParametro">Parámetro</param>
        public void BorrarParametroDeProyecto(Guid pProyectoID, Guid pOrganizacionID, string pParametro)
        {
            ParametroAD.BorrarParametroDeProyecto(pProyectoID, pOrganizacionID, pParametro);
        }

        #endregion

        public List<Guid> ObtenerProyectosQueAgrupanEventosRegistroHome()
        {
            return ParametroAD.ObtenerProyectosQueAgrupanEventosRegistroHome();
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
        ~ParametroCN()
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
                    if (ParametroAD != null)
                        ParametroAD.Dispose();
                }
                ParametroAD = null;
            }
        }

        #endregion

        #region Propiedades

        private ParametroAD ParametroAD
        {
            get
            {
                return (ParametroAD)AD;
            }
            set
            {
                AD = value;
            }
        }

        #endregion
    }
}
