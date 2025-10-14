using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ParametrosProyecto;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Logica.ParametrosProyecto
{
    /// <summary>
    /// Lógica referente a Parámetros Generales de Proyecto
    /// </summary>
    public class ParametroGeneralCN : BaseCN, IDisposable
    {

        #region Miembros

        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ParametroGeneralCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ParametroGeneralCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.ParametroGeneralAD = new ParametroGeneralAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication,mLoggerFactory.CreateLogger<ParametroGeneralAD>(),mLoggerFactory);
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public ParametroGeneralCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ParametroGeneralCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            ParametroGeneralAD = new ParametroGeneralAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication,mLoggerFactory.CreateLogger<ParametroGeneralAD>(),mLoggerFactory);
            mFicheroConfiguracionBD = pFicheroConfiguracionBD;
        }

        #endregion

        #region Métodos generales

        #region Públicos

        /// <summary>
        /// Obtiene los textos perdonalizados de un proyecto de una organización
        /// </summary>
        /// <param name="pProyectoID">Identificador de la personalización</param>
        /// <returns>Dataset de parámetros generales</returns>
        public List<TextosPersonalizadosPersonalizacion> ObtenerTextosPersonalizacionProyecto(Guid pProyectoID)
        {
            return this.ParametroGeneralAD.ObtenerTextosPersonalizacionProyecto(pProyectoID);
        }

        /// <summary>
        /// Actualiza las versiones de las caches.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void VersionarCSSYJS(Guid pProyectoID)
        {
            this.ParametroGeneralAD.VersionarCSSYJS(pProyectoID);
        }

        public List<TextosPersonalizadosPersonalizacion> ObtenerTextosPersonalizadosDominio(string pDominio, Guid pPersonalizacionEcosistema)
        {
            return this.ParametroGeneralAD.ObtenerTextosPersonalizadosDominio(pDominio, pPersonalizacionEcosistema);
        }

        public List<TextosPersonalizadosProyecto> ObtenerTextosPersonalizadosProyecto(string pDominio)
        {
            return this.ParametroGeneralAD.ObtenerTextosPersonalizadosProyecto(pDominio);
        }

        public List<TextosPersonalizadosPlataforma> ObtenerTextosPersonalizadosPlataforma()
        {
            return this.ParametroGeneralAD.ObtenerTextosPersonalizadosPlataforma();
        }



        /// <summary>
        /// Obtiene la política de certificación de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <returns>String con la política de certificación</returns>
        public string ObtenerPoliticaCertificacionDeProyecto(Guid pProyectoID)
        {
            return this.ParametroGeneralAD.ObtenerPoliticaCertificacionDeProyecto(pProyectoID);
        }

        /// <summary>
        /// Comprueba si un proyecto tiene establecida la imagen de la home
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public bool TieneProyectoImagenHome(Guid pProyectoID)
        {
            return this.ParametroGeneralAD.TieneProyectoImagenHome(pProyectoID);
        }

        /// <summary>
        /// Actualiza parámetros generales de proyecto
        /// </summary>
        /// <param name="pParametrosGeneralesDS">Dataset de parámetros generales</param>
        /// <param name="pComprobarPermisos">TRUE si debe comprobar los permisos</param>
        //public void ActualizarParametrosGenerales(ParametroGeneralDS pParametrosGeneralesDS, bool pComprobarPermisos)
        public void ActualizarParametrosGenerales()
        {
            try
            {
                bool transaccionIniciada = ParametroGeneralAD.IniciarTransaccionEntityContext();

                mEntityContext.SaveChanges();

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
                throw new ErrorConcurrencia();
            }
            catch (DataException ex)
            {
                TerminarTransaccion(false);
                //Error interno de la aplicación	
                mLoggingService.GuardarLogError(ex,mlogger);
                throw new ErrorInterno();
            }
            catch
            {
                TerminarTransaccion(false);
                throw;
            }
        }

        /// <summary>
        /// Elimina parámetros generales de proyecto
        /// </summary>
        /// <param name="pParametroGeneralDS">Dataset de parámetros generales</param>
        //public void EliminarParametrosGenerales(ParametroGeneralDS pParametroGeneralDS, bool pComprobarPermisos)
        public void EliminarParametrosGenerales()
        {
            mEntityContext.SaveChanges();
            //try
            //{
            //if (Transaccion != null)
            //{
            //    this.ParametroGeneralAD.EliminarParametrosGenerales(pParametroGeneralDS);
            //}
            //else
            //{
            //    IniciarTransaccion();
            //    {
            //        this.ParametroGeneralAD.EliminarParametrosGenerales(pParametroGeneralDS);

            //        if (pParametroGeneralDS != null)
            //        {
            //            pParametroGeneralDS.AcceptChanges();
            //        }

            //        TerminarTransaccion(true);
            //    }
            //}
            //}
            //catch (DBConcurrencyException ex)
            //{
            //    TerminarTransaccion(false);
            //    // Error de concurrencia
            //    Error.GuardarLogError(ex);
            //    throw new ErrorConcurrencia();
            //}
            //catch (DataException ex)
            //{
            //    TerminarTransaccion(false);
            //    //Error interno de la aplicación	
            //    Error.GuardarLogError(ex);
            //    throw new ErrorInterno();
            //}
            //catch
            //{
            //    TerminarTransaccion(false);
            //    throw;
            //}
        }

        /// <summary>
        /// Guarda parámetros generales de proyecto
        /// </summary>
        /// <param name="pParametroGeneralDS">Dataset de parámetros generales</param>
        //public void GuardarParametrosGenerales(ParametroGeneralDS pParametroGeneralDS, bool pComprobarPermisos)
        public void GuardarParametrosGenerales()
        {
            mEntityContext.SaveChanges();
            //try
            //{
            //    if (Transaccion != null)
            //    {
            //        this.ParametroGeneralAD.GuardarParametrosGenerales(pParametroGeneralDS);
            //    }
            //    else
            //    {
            //        IniciarTransaccion();
            //        {
            //            this.ParametroGeneralAD.GuardarParametrosGenerales(pParametroGeneralDS);

            //            if (pParametroGeneralDS != null)
            //            {
            //                pParametroGeneralDS.AcceptChanges();
            //            }
            //            TerminarTransaccion(true);
            //        }
            //    }
            //    
            //}
            //catch (DBConcurrencyException ex)
            //{
            //    TerminarTransaccion(false);
            //    // Error de concurrencia
            //    Error.GuardarLogError(ex);
            //    throw new ErrorConcurrencia();
            //}
            //catch (DataException ex)
            //{
            //    TerminarTransaccion(false);
            //    //Error interno de la aplicación	
            //    Error.GuardarLogError(ex);
            //    throw new ErrorInterno();
            //}
            //catch
            //{
            //    TerminarTransaccion(false);
            //    throw;
            //}
        }

        /// <summary>
        /// Obtiene la fila de parametros generales de un proyecto CARGA LIGERA
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>fila de ParametroGeneral</returns>
        //public ParametroGeneralDS.ParametroGeneralRow ObtenerFilaParametrosGeneralesDeProyecto(Guid pProyectoID)
        public ParametroGeneral ObtenerFilaParametrosGeneralesDeProyecto(Guid pProyectoID)
        {
            //return ParametroGeneralAD.ObtenerFilaParametrosGeneralesDeProyecto(pProyectoID);
            return ParametroGeneralAD.ObtenerFilaParametrosGeneralesDeProyecto(pProyectoID);

        }
        /// <summary>
        /// Obtiene la fila de parametros generales de un proyecto CARGA LIGERA
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>fila de ParametroGeneral</returns>
        //public ParametroGeneralDS.ParametroGeneralRow ObtenerFilaParametrosGeneralesDeProyecto(Guid pProyectoID)
        public IEnumerable<ProyectoElementoHtml> ObtenerFilaProyectoElementoHtml(Guid pProyectoID)
        {
            //return ParametroGeneralAD.ObtenerFilaParametrosGeneralesDeProyecto(pProyectoID);
            return ParametroGeneralAD.ObtenerFilaProyectoElementoHtml(pProyectoID);

        }

        /// <summary>
        /// Obtiene la fila de parametros generales de un proyecto CARGA LIGERA
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>fila de ParametroGeneral</returns>
        //public ParametroGeneralDS.ParametroGeneralRow ObtenerFilaParametrosGeneralesDeProyecto(Guid pProyectoID)
        public IEnumerable<ProyectoMetaRobots> ObtenerFilaProyectoMetaRobots(Guid pProyectoID)
        {
            //return ParametroGeneralAD.ObtenerFilaParametrosGeneralesDeProyecto(pProyectoID);
            return ParametroGeneralAD.ObtenerProyectoMetaRobots(pProyectoID);

        }

        //public ParametroGeneralDS ObtenerTextosPersonalizadosPersonalizacionEcosistema(Guid pPersonalizacionEcosistemaID)
        public List<TextosPersonalizadosPersonalizacion> ObtenerTextosPersonalizadosPersonalizacionEcosistema(Guid pPersonalizacionEcosistemaID)
        {
            return ParametroGeneralAD.ObtenerTextosPersonalizadosPersonalizacionEcosistema(pPersonalizacionEcosistemaID);
        }
        /// <summary>
        /// Obtiene las traducciones de una personalizacion por el idioma
        /// </summary>
        /// <param name="pPersonalizacionEcosistemaID"></param>
        /// <param name="pLanguage"></param>
        /// <returns></returns>
        public List<TextosPersonalizadosPersonalizacion> ObtenerTextosPersonalizadosPersonalizacionEcosistemaPorIdioma(Guid pPersonalizacionEcosistemaID, string pLanguage)
        {
            return ParametroGeneralAD.ObtenerTextosPersonalizadosPersonalizacionEcosistemaPorIdioma(pPersonalizacionEcosistemaID, pLanguage);
        } 

        /// <summary>
        /// Obtiene la traducción indicado para la personalización indicada
        /// </summary>
        /// <param name="pPersonalizacionID">Personalización del proyecto a obtener la traducción</param>
        /// <param name="pTextoID">Identificador de la traducción a obtener</param>
        /// <returns></returns>
		public List<TextosPersonalizadosPersonalizacion> ObtenerTraduccionPorTextoIdDePersonalizacion(Guid pPersonalizacionID, string pTextoID)
		{
            return ParametroGeneralAD.ObtenerTraduccionPorTextoIdDePersonalizacion(pPersonalizacionID, pTextoID);
		}

		/// <summary>
		/// Obtiene el texto de una clave concreta de personalizacion
		/// </summary>
		/// <param name="pPersonalizacionID">Identificador de la personalizacion</param>
		/// <param name="pClaveIdioma">Clave del idioma (es, en...)</param>
		/// <param name="pTextoID">Identificador del texto</param>
		/// <returns></returns>
		public string ObtenerTextoPersonalizadoPersonalizacion(Guid pPersonalizacionID, string pClaveIdioma, string pTextoID)
        {
            return ParametroGeneralAD.ObtenerTextoPersonalizadoPersonalizacion(pPersonalizacionID, pClaveIdioma, pTextoID);
        }

        /// <summary>
        /// Obtiene la fila de parametros generales de un proyecto CARGA LIGERA
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>fila de ParametroGeneral</returns>
        public List<ProyectoRDFType> ObtenerProyectoRDFType(Guid pProyectoID, int pTipoDoc)
        {
            return ParametroGeneralAD.ObtenerProyectoRDFType(pProyectoID, pTipoDoc);
        }

        /// <summary>
        /// Obtiene la fila de los parametros generales de una lista de proyectos CARGA LIGERA
        /// </summary>
        /// <param name="pListaProyectos">lista con las claves de los proyectos</param>
        /// <returns>ParametroGeneralDS</returns>
        public List<ParametroGeneral> ObtenerParametrosGeneralesDeListaDeProyectos(List<Guid> pListaProyectos)
        {
            return ParametroGeneralAD.ObtenerParametrosGeneralesDeListaDeProyectos(pListaProyectos);
        }

        /// <summary>
        /// Obtiene las licencias de parametros generales de los proyectos (Proy,licencia).
        /// </summary>
        /// <param name="pListaProyectosID">Identificadores de proyecto</param>
        public Dictionary<Guid, string> ObtenerLicenciasporProyectosID(List<Guid> pListaProyectosID)
        {
            return ParametroGeneralAD.ObtenerLicenciasporProyectosID(pListaProyectosID);
        }

        /// <summary>
        /// Indica si al registrar a un usuario nuevo en el proyecto se le debe hacer miembro de didactalia también.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>TRUE si al registrar a un usuario nuevo en el proyecto se le debe hacer miembro de didactalia también, FALSE en caso contrario</returns>
        public bool ObtenerRegistroDidactaliaProyecto(Guid pProyectoID)
        {
            return ParametroGeneralAD.ObtenerRegistroDidactaliaProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene una lista con los ElementosHeadID de la tabla ProyectoElementoHtmlRol
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pIdentidadUsuarioID">Identificador de la identidad del usuario actual</param>
        /// <returns>Lista de enteros con los ElementosHeadID de la tabla ProyectoElementoHtmlRol</returns>
        public List<int> ObtenerElementosHeadIDRol(Guid pProyectoID, Guid pIdentidadUsuarioID)
        {
            return ParametroGeneralAD.ObtenerElementosHeadIDRol(pProyectoID, pIdentidadUsuarioID);
        }
        /// <summary>
        /// Elimina de  una lista de tags las palabras que estén definidas como inapropiadas para este proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que se va a etiquetar algo</param>
        /// <param name="pListaTags">Lista de tags a comprobar</param>
        public void EliminarPalabrasInapropiadasDeListaTagsParaProyecto(Guid pOrganizacionID, Guid pProyectoID, List<string> pListaTags)
        {
            List<string> listaTagsInapropiados = ParametroGeneralAD.ObtenerPalabrasInapropiadasDeListaTagsParaProyecto(pOrganizacionID, pProyectoID, pListaTags);

            foreach (string tag in listaTagsInapropiados)
            {
                if (pListaTags.Contains(tag))
                {
                    pListaTags.Remove(tag);
                }
            }
        }
        #endregion

        #region Privados

        /// <summary>
        /// Valida parámetros generales
        /// </summary>
        /// <param name="pParametrosGenerales">Conjunto de filas de parámetros generales para validar</param>
        private void ValidarParametrosGenerales(List<ParametroGeneral> pParametrosGenerales)
        {
            if (pParametrosGenerales != null)
            {
                for (int i = 0; i < pParametrosGenerales.Count; i++)
                {
                    //Umbral de suficiencia en mejora no válido
                    //if (!pParametrosGenerales[i].IsUmbralSuficienciaEnMejoraNull())
                    if (!(pParametrosGenerales[i].UmbralSuficienciaEnMejora == null))
                    {
                        if ((pParametrosGenerales[i].UmbralSuficienciaEnMejora < 0) || (pParametrosGenerales[i].UmbralSuficienciaEnMejora > 100))
                            throw new ErrorDatoNoValido("El % de Umbral de suficiencia en mejora debe estar comprendido entre 0 y 100");
                    }
                    //Desviación admitida en evaluación integrada no válida
                    //if (!pParametrosGenerales[i].IsDesviacionAdmitidaEnEvaluaNull())
                    if (!(pParametrosGenerales[i].DesviacionAdmitidaEnEvalua == null))
                    {
                        if ((pParametrosGenerales[i].DesviacionAdmitidaEnEvalua < 0) || (pParametrosGenerales[i].DesviacionAdmitidaEnEvalua > 100))
                            throw new ErrorDatoNoValido("La desviación admitida en evaluación integrada debe estar comprendida entre 0 y 100");
                    }
                    //Umbral de determinación de propietarios en proceso no válido
                    //if (!pParametrosGenerales[i].IsUmbralDetPropietariosProcNull())
                    if (!(pParametrosGenerales[i].UmbralDetPropietariosProc == null))
                    {
                        if (pParametrosGenerales[i].UmbralDetPropietariosProc < 0 || pParametrosGenerales[i].UmbralDetPropietariosProc > 100)
                            throw new ErrorDatoNoValido("El % de Umbral de determinación de propietarios en proceso debe estar comprendido entre 0 y 100");
                    }
                    //Umbral de determinación de propietarios en objetivo no válido
                    //if (!pParametrosGenerales[i].IsUmbralDetPropietariosObjNull())
                    if (!(pParametrosGenerales[i].UmbralDetPropietariosObj == null))
                    {
                        if (pParametrosGenerales[i].UmbralDetPropietariosObj < 0 || pParametrosGenerales[i].UmbralDetPropietariosObj > 100)
                            throw new ErrorDatoNoValido("El % de Umbral de determinación de propietarios en objetivo debe estar comprendido entre 0 y 100");
                    }
                    //Umbral de determinación de propietarios en grupo funcional no válido
                    //pParametrosGenerales[i].IsUmbralDetPropietariosGFNull()
                    if (!(pParametrosGenerales[i].UmbralDetPropietariosGF == null))
                    {
                        if (pParametrosGenerales[i].UmbralDetPropietariosGF < 0 || pParametrosGenerales[i].UmbralDetPropietariosGF > 100)
                            throw new ErrorDatoNoValido("El % de Umbral de determinación de propietarios en grupo funcional debe estar comprendido entre 0 y 100");
                    }
                    //Longitud del primer factor de DAFO de proceso no válida
                    if (pParametrosGenerales[i].NombreDebilidadDafoProc.Length > 1000)
                        throw new ErrorDatoNoValido("La longitud del nombre del primer factor de DAFO de proceso debe ser menor que 1000");

                    //Longitud del segundo factor de DAFO de proceso no válida
                    if (pParametrosGenerales[i].NombreAmenazaDafoProc.Length > 1000)
                        throw new ErrorDatoNoValido("La longitud del nombre del segundo factor de DAFO de proceso debe ser menor que 1000");

                    //Longitud del tercer factor de DAFO de proceso no válida
                    if (pParametrosGenerales[i].NombreFortalezaDafoProc.Length > 1000)
                        throw new ErrorDatoNoValido("La longitud del nombre del tercer factor de DAFO de proceso debe ser menor que 1000");

                    //Longitud del cuarto factor de DAFO de proceso no válida
                    if (pParametrosGenerales[i].NombreOportunidadDafoProc.Length > 1000)
                        throw new ErrorDatoNoValido("La longitud del nombre del cuarto factor de DAFO de proceso debe ser menor que 1000");

                    //Longitud del primer factor de DAFO de objetivo no válida
                    if (pParametrosGenerales[i].NombreDebilidadDafoObj.Length > 1000)
                        throw new ErrorDatoNoValido("La longitud del nombre del primer factor de DAFO de objetivo debe ser menor que 1000");

                    //Longitud del segundo factor de DAFO de objetivo no válida
                    if (pParametrosGenerales[i].NombreAmenazaDafoObj.Length > 1000)
                        throw new ErrorDatoNoValido("La longitud del nombre del segundo factor de DAFO de objetivo debe ser menor que 1000");

                    //Longitud del tercer factor de DAFO de objetivo no válida
                    if (pParametrosGenerales[i].NombreFortalezaDafoObj.Length > 1000)
                        throw new ErrorDatoNoValido("La longitud del nombre del tercer factor de DAFO de objetivo debe ser menor que 1000");

                    //Longitud del cuarto factor de DAFO de obketivo no válida
                    if (pParametrosGenerales[i].NombreOportunidadDafoObj.Length > 1000)
                        throw new ErrorDatoNoValido("La longitud del nombre del cuarto factor de DAFO de objetivo debe ser menor que 1000");

                    //Longitud del primer factor de DAFO de grupo funcional no válida
                    if (pParametrosGenerales[i].NombreDebilidadDafoGF.Length > 1000)
                        throw new ErrorDatoNoValido("La longitud del nombre del primer factor de DAFO de grupo funcional debe ser menor que 1000");
                    //Longitud del segundo factor de DAFO de grupo funcional no válida
                    if (pParametrosGenerales[i].NombreAmenazaDafoGF.Length > 1000)
                        throw new ErrorDatoNoValido("La longitud del nombre del segundo factor de DAFO de grupo funcional debe ser menor que 1000");
                    //Longitud del tercer factor de DAFO de grupo funcional no válida
                    if (pParametrosGenerales[i].NombreFortalezaDafoGF.Length > 1000)
                        throw new ErrorDatoNoValido("La longitud del nombre del tercer factor de DAFO de grupo funcional debe ser menor que 1000");
                    //Longitud del cuarto factor de DAFO de grupo funcional no válida
                    if (pParametrosGenerales[i].NombreOportunidadDafoGF.Length > 1000)
                        throw new ErrorDatoNoValido("La longitud del nombre del cuarto factor de DAFO de grupo funcional debe ser menor que 1000");
                }
            }
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
        ~ParametroGeneralCN()
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
                    if (ParametroGeneralAD != null)
                        ParametroGeneralAD.Dispose();
                }
                ParametroGeneralAD = null;
            }
        }

        #endregion

        #region Propiedades

        private ParametroGeneralAD ParametroGeneralAD
        {
            get
            {
                return (ParametroGeneralAD)AD;
            }
            set
            {
                AD = value;
            }
        }

        #endregion
    }
}
