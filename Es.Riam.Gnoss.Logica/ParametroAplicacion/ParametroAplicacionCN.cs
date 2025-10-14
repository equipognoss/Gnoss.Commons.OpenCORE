using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Logica.ParametroAplicacion
{
    /// <summary>
    /// Clase, que actua de l�gica entre la aplicaci�n y el AD.
    /// </summary>
    public class ParametroAplicacionCN : BaseCN, IDisposable
    {

        #region Miembros

        private static Dictionary<string, string> mListaIdiomasDictionary = new Dictionary<string, string>();
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructores

        /// <summary>
        /// Constructor sin par�metros
        /// </summary>
        public ParametroAplicacionCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ParametroAplicacionCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            ParametroAplicacionAD = new ParametroAplicacionAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication,mLoggerFactory.CreateLogger<ParametroAplicacionAD>(),mLoggerFactory);
        }

        /// <summary>
        /// Constructor a partir del fichero de configuraci�n
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuraci�n de la base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se est�n usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public ParametroAplicacionCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ParametroAplicacionCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            ParametroAplicacionAD = new ParametroAplicacionAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication,mLoggerFactory.CreateLogger<ParametroAplicacionAD>(),mLoggerFactory);
        }

        #endregion

        #region M�todos genereales

        public string ObtenerParametroBusquedaPorTextoLibrePersonalizado()
        {
            return ParametroAplicacionAD.ObtenerParametroBusquedaPorTextoLibrePersonalizado();
        }

        public List<ConfiguracionBBDD> ObtenerConfiguracionBBDD()
        {
            return ParametroAplicacionAD.ObtenerConfiguracionBBDD();
        }

        /// <summary>
        /// Obtiene las filas de los proyectos en los que es necesario registrar al usuario que se est� registrando.
        /// </summary>
        /// <param name="pOrganizacionRegistroUsuario">Identificador de la organizaci�n en la que se est� registrando el usuario</param>
        /// <param name="pProyectoRegistroUsuario">Identificador de la comunidad en la que se est� registrando el usuario</param>
        /// <returns></returns>
        public List<ProyectoRegistroObligatorio> ObtenerFilasProyectosARegistrarUsuario(Guid pOrganizacionRegistroUsuario, Guid pProyectoRegistroUsuario)
        {
            return ParametroAplicacionAD.ObtenerFilasProyectosARegistrarUsuario(pOrganizacionRegistroUsuario, pProyectoRegistroUsuario);
        }

        /// <summary>
        /// Actualiza los par�metros de configuraci�n
        /// </summary>
        /// <param name="pConfiguracionGnossDS">Dataset de configuraci�n para actualizar</param>
        public void ActualizarConfiguracionGnoss()
        {
            ParametroAplicacionAD.ActualizarParametrosAplicacion();
            try
            {
                bool transaccionIniciada = ParametroAplicacionAD.IniciarTransaccionEntityContext();

                ParametroAplicacionAD.ActualizarParametrosAplicacion();

                if (transaccionIniciada)
                {
                    TerminarTransaccion(true);
                }
            }
            catch (DBConcurrencyException ex)
            {
                TerminarTransaccion(false);
                // Error de concurrencia
                mLoggingService.GuardarLogError(ex,mlogger);
                throw new ErrorConcurrencia(ex.Row);
            }
            catch (DataException ex)
            {
                TerminarTransaccion(false);
                //Error interno de la aplicaci�n
                mLoggingService.GuardarLogError(ex,mlogger);
                throw new ErrorInterno();
            }
            catch
            {
                TerminarTransaccion(false);
                throw;
            }
        }

        public string ObtenerUsarMismaVariablesParaEntidadesfacetas(Guid pProyectoID)
        {
            return ParametroAplicacionAD.ObtenerUsarMismaVariablesParaEntidadesfacetas(pProyectoID);
        }

        /// <summary>
        /// Devuelve un dataset con todos los parametros de la aplicaci�n
        /// </summary>
        /// <returns>Dataset de par�metros de la aplicaci�n</returns>
        public string ObtenerNombreBD()
        {
            return ParametroAplicacionAD.ObtenerNombreBaseDatos();
        }

        /// <summary>
        /// Obtiene los c�digos de idioma y los nombres de la tabla "ParametrosAplicacion"
        /// </summary>
        /// <returns>Diccionario donde las claves son los c�digos de idioma y los valores son los nombres de los idiomas</returns>
        public Dictionary<string, string> ObtenerListaIdiomasDictionary()
        {
            if (mListaIdiomasDictionary.Count == 0)
            {
                string idiomas = ParametroAplicacionAD.ObtenerIdiomas();
                if (string.IsNullOrEmpty(idiomas))
                {
                    mListaIdiomasDictionary = mConfigService.ObtenerListaIdiomasDictionary();
                }
                else
                {
                    // Dividir la cadena en pares de c�digo de idioma y nombre
                    // Ej: es|Espa�ol,en|English,pt|Portuguese,ca|Catal�,eu|Euskera,gl|Galego,fr|Fran�ais,de|Deutsch,it|Italiano
                    string[] idiomasArray = idiomas.Split("&&&");
                    foreach (string idioma in idiomasArray)
                    {
                        string[] partes = idioma.Split('|');
                        mListaIdiomasDictionary.Add(partes[0], partes[1]);
                    }
                }
            }
            return mListaIdiomasDictionary;
        }

        /// <summary>
        /// Obtiene los c�digos de idioma de la tabla "ParametrosAplicacion"
        /// </summary>
        /// <returns>Lista de los c�digos de idioma</returns>
        public List<string> ObtenerListaIdiomas()
        {
            return ObtenerListaIdiomasDictionary().Keys.ToList();
        }

        /// <summary>
        /// Vac�a la lista est�tica de idiomas
        /// </summary>
        public void VaciarListaIdiomas()
        {
            mListaIdiomasDictionary = new Dictionary<string, string>();
        }

        /// <summary>
        /// Obtiene el valor de la UrlIntragnoss de la tabla "ParametrosAplicacion"
        /// </summary>
        /// <returns>Cadena de texto con la url de la intranet de Gnoss</returns>
        public string ObtenerUrl()
        {
            return ParametroAplicacionAD.ObtenerUrl();
        }

        /// <summary>
        /// Obtiene el valor de la UrlContent de la tabla "ParametroDominio", y si es vac�a, de la tabla "ParametrosAplicacion"
        /// </summary>
        /// <returns>Cadena de texto con la url de la intranet de Gnoss</returns>
        public string ObtenerUrlContent(Guid pProyectoID)
        {
            return ParametroAplicacionAD.ObtenerUrlContent(pProyectoID);
        }

        /// <summary>
        /// Obtiene el valor del correo de sugerencias de la tabla "ParametrosAplicacion"
        /// </summary>
        /// <returns>Cadena de texto con la url de la intranet de Gnoss</returns>
        public string ObtenerCorreoSugerencias()
        {
            return ParametroAplicacionAD.ObtenerCorreoSugerencias();
        }

        /// <summary>
        /// Obtiene una lista con los proyectos de registro obligatorio y su nombre corto
        /// </summary>
        /// <returns>Lista con los proyectos y su personalizacion</returns>
        public Dictionary<Guid, string> ObtenerProyectosRegistroObligatorio()
        {
            return ParametroAplicacionAD.ObtenerProyectosRegistroObligatorio();
        }

        /// <summary>
        /// Obtiene una lista con los ids de los proyectos que son obligatorios
        /// </summary>
        /// <returns>Lista con los proyectos y su personalizacion</returns>
        public List<Guid> ObtenerProyectosSinRegistroObligatorio(Guid id)
        {
            return ParametroAplicacionAD.ObtenerProyectosSinRegistroObligatorio(id);
        }

        /// <summary>
        /// Modifica el valor de un parametro en la base de datos
        /// </summary>
        /// <param name="parametro">Nombre del parametro</param>
        /// <param name="valor">Valor que tiene el parametro</param>
        public void ActualizarParametroAplicacion(string parametro, string valor)
        {
            ParametroAplicacionAD.ActualizarParametroAplicacion(parametro, valor);
        }

        /// <summary>
        /// Obtiene el valor de la base de datos del par�emtro indicado
        /// </summary>
        /// <param name="parametro">Nombre del parametro</param>
        /// <param name="valor">Valor que tiene el parametro</param>
        public string ObtenerParametroAplicacion(string parametro)
        {
            return ParametroAplicacionAD.ObtenerParametroAplicacion(parametro);
        }

        /// <summary>
        /// Obtiene los valores de los par�metros de la base de datos que contengan la cadena proporcionada
        /// </summary>
        /// <param name="pParametro">Cadena que debe contener el par�metro</param>
        /// <returns>Valores de parametro aplicacion cuyo paramtro contenga la cadena proporcionada</returns>
        public List<string> ObtenerParametroAplicacionSeaContenidoParametro(string pParametro)
        {
            return ParametroAplicacionAD.ObtenerParametroAplicacionSeaContenidoParametro(pParametro);
        }

        public AD.EntityModel.ParametroAplicacion ObtenerFilaParametroAplicacion(string parametro)
        {
            return ParametroAplicacionAD.ObtenerFilaParametroAplicacion(parametro);
        }
        #endregion

        #region Dispose

        /// <summary>
        /// Determina si est� disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~ParametroAplicacionCN()
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
        /// <param name="disposing">Determina si se est� llamando desde el Dispose()</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                if (disposing)
                {
                    //Libero todos los recursos administrados que he a�adido a esta clase
                    if (ParametroAplicacionAD != null)
                        ParametroAplicacionAD.Dispose();
                }
            }
        }

        #endregion

        #region Propiedades

        private ParametroAplicacionAD ParametroAplicacionAD
        {
            get
            {
                return (ParametroAplicacionAD)AD;
            }
            set
            {
                AD = value;
            }
        }

        #endregion

    }
}
