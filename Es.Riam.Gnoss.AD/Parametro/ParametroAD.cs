using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Es.Riam.Gnoss.AD.Parametro
{
    /// <summary>
    /// Enumeración para distinguir tipos de proyectos
    /// </summary>
    public enum TipoBusquedasAutocompletar
    {
        /// <summary>
        /// Proyecto de organización
        /// </summary>
        BBDDTags = 0,
        /// <summary>
        /// Comunidad web
        /// </summary>
        Virtuoso = 1
    }

    /// <summary>
    /// Acceso a datos para parametros de GNOSS.
    /// </summary>
    public class ParametroAD : BaseAD
    {
        #region Constantes

        /// <summary>
        /// Indica el Robots de la comunidad, Ej: NOINDEX, NOFOLLOW.
        /// </summary>
        public const string RobotsComunidad = "RobotsComunidad";

        /// <summary>
        /// Indica el idioma por defecto de la comunidad, Ej: GUID del país correspondiente en BBDD (Guid.empty en caso de que no se quiera que parezca ninguno). Si no existe esta entrada funciona como hasta ahora.
        /// </summary>
        public const string IdiomaRegistroDefecto = "PaisRegistroDefecto";

        /// <summary>
        /// Indica si se tienen que enviar las notificaciones de las suscripciones (si no se especifica coje el valor del ecosistema)
        /// </summary>
        public const string EnviarNotificacionesDeSuscripciones = "EnviarNotificacionesDeSuscripciones";

        /// <summary>
        /// Indica si en la página de recursos tiene que ocultar las facetas de los Objetos de conocimiento cuando hay mas de un tipo
        /// </summary>
        public const string OcultarFacetatasDeOntologiasEnRecursosCuandoEsMultiple = "OcultarFacetatasDeOntologiasEnRecursosCuandoEsMultiple";

        /// <summary>
        /// Indica si se tienen que enviar las notificaciones de las suscripciones (si no se especifica coje el valor del ecosistema)
        /// </summary>
        public const string TamanioPoolRedis = "TamanioPoolRedis";

        /// <summary>
        /// Indica si se tienen que enviar las notificaciones de las suscripciones (si no se especifica coje el valor del ecosistema)
        /// </summary>
        public const string ListaIdiomas = "ListaIdiomas";

        /// <summary>
        /// Indica si el proyecto tiene grafo de dbpedia
        /// </summary>
        public const string ProyectoTieneGrafoDbPedia = "TieneGrafoDbPedia";

        /// <summary>
        /// Indica si en la comunidad solo se tienen que mostrar los contenidos en el idioma del usuario
        /// </summary>
        public const string PropiedadContenidoMultiIdioma = "PropiedadContenidoMultiIdioma";

        /// <summary>
        /// Indica si en la comunidad solo se tienen que mostrar los contenidos en el idioma del usuario
        /// </summary>
        public const string PropiedadCMSMultiIdioma = "PropiedadCMSMultiIdioma";

        /// <summary>
        /// Indica si la URL de la comunidad no tiene nombre corto en la URL
        /// </summary>
        public const string ProyectoSinNombreCortoEnURL = "ProyectoSinNombreCortoEnURL";

        /// <summary>
        /// Indica si se puede leer los ficheros privados a traves de un token de acceso
        /// </summary>
        public const string LecturaFichero = "LecturaFichero";

        /// <summary>
        /// Indica si se oculta el enlace de cambiar contraseña en el menú del usuario
        /// </summary>
        public const string OcultarCambiarPassword = "OcultarCambiarPwd";

        /// <summary>
        /// Indica si se oculta el enlace de cambiar contraseña en el menú del usuario
        /// </summary>
        public const string ParametrosExtraYoutube = "ParametrosExtraYoutube";

        /// <summary>
        /// Tipo de mensaje a enviar en un espacio sin metaproyecto
        /// </summary>
        public const string TipoEnviarMensajeBienvenida = "TipoEnviarMensajeBienvenida";

        /// <summary>
        /// Parametro para establecer una caducidad (en días) a la password del usuario. 
        /// Le obliga a cambiar la password al usuario periódicamente. En este parametro, se define esa periodicidad en días. 
        /// </summary>
        public const string CaducidadPassword = "CaducidadPassword";

        /// <summary>
        /// Define, para una comunidad privada, si el registro está abierto. 
        /// Es decir, los usuarios se van a poder registrar sin necesidad de recibir una invitación. 
        /// Para configurar el registro abierto, el valor debe ser "1". 
        /// </summary>
        public const string RegistroAbierto = "RegistroAbierto";

        /// <summary>
        /// Ancho y alto separados por ',' a los que deben realizarse las caputuras de la comunidad. Por defecto '240,200'.
        /// </summary>
        public const string CaputurasImgSize = "CaputurasImgSize";

        /// <summary>
        /// ID del proyecto patrón del que se deben extraer las ontologías para la comunidad actual.
        /// </summary>
        public const string ProyectoIDPatronOntologias = "ProyectoIDPatronOntologias";

        /// <summary>
        /// Parámetro que determina si es posible duplicar los recursos del proyecto o no. 'true' si es que si y 'false' si es que no
        /// </summary>
        public const string DuplicarRecursosDisponible = "DuplicarRecursosDisponible";

        /// <summary>
        /// Numero de facetas que se traen en la primera petición de facetas
        /// </summary>
        public const string NumeroFacetasPrimeraPeticion = "NumeroFacetasPrimeraPeticion";

        /// <summary>
        /// Numero de facetas que se trean en la segunda petición de facetas
        /// </summary>
        public const string NumeroFacetasSegundaPeticion = "NumeroFacetasSegundaPeticion";

        /// <summary>
        /// Indica si la tercera petición de facetas se traen sólo los títulos de las facetas, sin contenido. 
        /// </summary>
        public const string TerceraPeticionFacetasPlegadas = "TerceraPeticionFacetasPlegadas";

        /// <summary>
        /// Lista de facetas separadas por | que se van a hacer en hilos separados en la tercera petición de facetas.  
        /// </summary>
        public const string FacetasCostosasTerceraPeticion = "FacetasCostosasTerceraPeticion";

        /// <summary>
        /// Indica si en la comunidad se debe recibir la newsletter por defectoo
        /// </summary>
        public const string RecibirNewsletterDefecto = "RecibirNewsletterDefecto";

        /// <summary>
        /// Lista de propiedades separadas por | que tienen enlaces a páginas de dbpedia y se quieren sacar en el grafo de dbpedia.  
        /// </summary>
        public const string PropiedadesConEnlacesDbpedia = "PropiedadesConEnlacesDbpedia";

        /// <summary>
        /// Indica si al registrar un usuario en la comunidad se debe suscribir a todas las categorías del tesauro de la comunidad
        /// </summary>
        public const string SuscribirATodaComunidad = "SuscribirATodaComunidad";

        /// <summary>
        /// Indica la periodicidad por defecto de envío del boletín de la suscripción de los usuarios en la comunidad
        /// </summary>
        public const string PeriodicidadSuscripcion = "PeriodicidadSuscripcion";

        /// <summary>
        /// Indica el día de la semana de envío del boletín de la suscripción de los usuarios en la comunidad
        /// </summary>
        public const string DiaEnvioSuscripcion = "DiaEnvioSuscripcion";

        /// <summary>
        /// Indica el número de caracteres de la descripción del boletín de la suscripción
        /// </summary>
        public const string NumeroCaracteresDescripcion = "NumeroCaracteresDescripcionSuscripcion";

        /// <summary>
        /// Indica si la identidad invitada puede descargar el documento
        /// </summary>
        public const string PermitirDescargaIdentidadInvitada = "PermitirDescargaIdentidadInvitada";

        /// <summary>
        /// Indica si se deben cargar los editores y lectores de cada recurso en una página de búsqueda
        /// </summary>
        public const string CargarEditoresLectoresEnBusqueda = "CargarEditoresLectoresEnBusqueda";

        /// <summary>
        /// Indica el nombre de la cookie de aceptación de política de cookies
        /// </summary>
        public const string NombrePoliticaCookies = "NombrePoliticaCookies";

        /// <summary>
        /// Indica la ruta relativa a los estilos, si no es el ID del proyecto (ej: "EcosistemaDidactalia")
        /// </summary>
        public const string RutaEstilos = "RutaEstilos";

        /// <summary>
        /// Indica si se puden permitir las mayúsculas en el grafo de búsqueda
        /// </summary>
        public const string PermitirMayusculas = "PermitirMayusculas";
        /// <summary>
        /// Indica los items que se muestran en la actividad reciente
        /// </summary>
        public const string FilasPorPagina = "FilasPorPagina";

        /// <summary>
        /// Indica los segundos a esperar para la siguiente comprobación en ServiceBus
        /// </summary>
        public const string ServiceBusSegundos = "ServiceBusSegundos";

        /// <summary>
        /// Indica los reintentos para coger el fichero configurado con ServiceBus
        /// </summary>
        public const string ServiceBusReintentos = "ServiceBusReintentos";

        /// <summary>
        /// Indica si se debe usar la misma variable para entidades en facetas
        /// Si su valor es 1, siempre que se haga referencia a una entidad en una consulta de facetas, la variable usada será la misma. 
        /// Ej: ?s ecidoc:author ?author. ?author dc:name ?name. 
        /// ?s ecidoc:author ?author. ?author ecidoc:birthDate ?birthDate
        /// </summary>
        public const string RegistroAbiertoEnComunidad = "RegistroAbiertoEnComunidad";

        /// <summary>
        /// Indica si el registro en esta comunidad no tiene pasos
        /// </summary>
        public const string RegistroSinPasos = "RegistroSinPasos";

        /// <summary>
        /// Indica si se deben usar las mismas variables para las entidades cuando se calculan las facetas
        /// </summary>
        public const string UsarMismsaVariablesParaEntidadesEnFacetas = "UsarMismsaVariablesParaEntidadesEnFacetas";

        /// <summary>
        /// Id de aplicación de Azure AD para conectar con el API de Microsoft Graph y acceder a funcionalidades de SharePoint
        /// </summary>
        public const string SharepointClientID = "SharepointClientID";

        /// <summary>
        /// Id de inquilino de Azure AD para conectar con el API de Microsoft Graph y acceder a funcionalidades de SharePoint
        /// </summary>
        public const string SharepointTenantID = "SharepointTenantID";

        /// <summary>
        /// Secreto de un cliente de Azure AD para conectar con el API de Microsoft Graph y acceder a funcionalidades de SharePoint
        /// </summary>
        public const string SharepointClientSecret = "SharepointClientSecret";

        public const string PermitirEnlazarDocumentosOneDrive = "PermitirEnlazarDocumentosOneDrive";

        /// <summary>
        /// Las facetas del tipo Tesauro Semántico se mostrarán con el botón VerMas y se ocultarán todas las que excedan el límite establecido en la administración de facetas
        /// </summary>
        public const string VerMasFacetaTesauroSemantico = "VerMasFacetaTesauroSemantico";

        #endregion

        #region Constructor

        private EntityContext mEntityContext;

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ParametroAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD"></param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public ParametroAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
        }

        #endregion

        #region Métodos generales

        #region Públicos

        /// <summary>
        /// Obtiene los parámetros de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <param name="pOrganizacionID">ID de la organización del proyecto</param>
        /// <returns>Lista con los parámetros de la tabla ParametroProyecto de un proyecto</returns>
        public Dictionary<string, string> ObtenerParametrosProyecto(Guid pProyectoID, Guid? pOrganizacionID)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var consulta = mEntityContext.ParametroProyecto.Where(parametroProy => parametroProy.ProyectoID.Equals(pProyectoID));
            if (pOrganizacionID.HasValue)
            {
                consulta = consulta.Where(parametroProy => parametroProy.OrganizacionID.Equals(pOrganizacionID.Value));
            }
            var listaParametros = consulta.Select(item => new
            {
                item.OrganizacionID,
                item.ProyectoID,
                item.Parametro,
                item.Valor
            }).ToList();


            foreach (var fila in listaParametros)
            {
                if (fila.Valor != null/*Valor*/ && !parametros.ContainsKey(fila.Parametro))
                {
                    parametros.Add(fila.Parametro/*Parametro*/, fila.Valor/*Valor*/);
                }
            }


            return parametros;
        }

        /// <summary>
        /// Obtiene el parámetro proyecto indicado si existe
        /// </summary>
        /// <param name="pNombreParametro">Nombre del parametro a obtener</param>
        /// <param name="pProyectoID">Identificador del proyecto donde queremos obtener el parámetro</param>
        /// <returns></returns>
        public ParametroProyecto ObtenerParametroDeProyecto(string pNombreParametro, Guid pProyectoID)
        {
            return mEntityContext.ParametroProyecto.Where(item => item.ProyectoID.Equals(pProyectoID) && item.Parametro.Equals(pNombreParametro)).FirstOrDefault();
        }

        public List<ProyectoServicioWeb> ObtenerProyectoServicioWeb(Guid pProyectoID)
        {
            return mEntityContext.ProyectoServicioWeb.Where(propiedad => propiedad.ProyectoID == pProyectoID).ToList();
        }

        public void GuardarFilasProyectoServicioWeb(List<ProyectoServicioWeb> pListaServicios, Guid pProyectoID)
        {
            List<ProyectoServicioWeb> propiedadesAnteriores = mEntityContext.ProyectoServicioWeb.Where(propiedad => propiedad.ProyectoID == pProyectoID).ToList();

            foreach (ProyectoServicioWeb propiedad in propiedadesAnteriores)
            {
                if (!pListaServicios.Exists(propiedadfind => propiedadfind.AplicacionWeb == propiedad.AplicacionWeb && propiedadfind.Nombre == propiedad.Nombre))
                {
                    mEntityContext.EliminarElemento(propiedad);
                }
            }

            foreach (ProyectoServicioWeb propiedad in pListaServicios)
            {
                ProyectoServicioWeb propiedadAnterior = propiedadesAnteriores.FirstOrDefault(propiedadfind => propiedadfind.AplicacionWeb == propiedad.AplicacionWeb && propiedadfind.Nombre == propiedad.Nombre);

                if (propiedadAnterior == null)
                {
                    propiedadAnterior = propiedad;
                    propiedadAnterior.OrganizacionID = Guid.Parse("11111111-1111-1111-1111-111111111111");
                    propiedadAnterior.ProyectoID = pProyectoID;
                    mEntityContext.ProyectoServicioWeb.Add(propiedadAnterior);
                }
            }

            ActualizarBaseDeDatosEntityContext();
        }



        /// <summary>
        /// Obtiene un parámetro concreto de todos los proyectos.
        /// </summary>
        /// <param name="pParametro">ID de proyecto</param>
        /// <returns>Lista con los parámetros de la tabla ParametroProyecto de todos los proyectos proyecto</returns>
        public Dictionary<Guid, string> ObtenerParametroDeProyectos(string pParametro)
        {
            Dictionary<Guid, string> parametros = new Dictionary<Guid, string>();
            var lista = mEntityContext.ParametroProyecto.Where(item => item.Parametro.Equals(pParametro)).ToList();

            foreach (ParametroProyecto param in lista)
            {
                parametros.Add(param.ProyectoID, param.Valor);
            }

            return parametros;
        }
        /// <summary>
        /// Obtiene de la tabla ConfigAutocompletarProy los valores de autocompletado para cada proyecto
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <returns>Lista con los valores de autocompletado de cada proyecto</returns>
        public List<string> ObtenerConfigAutocompletar(Guid pProyectoID)
        {
            List<string> listaStrings = mEntityContext.ConfigAutocompletarProy.Where(proy => proy.ProyectoID.Equals(pProyectoID)).Select(proy => proy.Valor).ToList();
            string[] separados;
            List<string> listaFinal = new List<string>();
            List<string> listaLectura = new List<string>();
            foreach (string valor in listaStrings)
            {
                separados = valor.Split('|');
                for (int i = 0; i < separados.Length; i++)
                {
                    listaLectura.Add(separados[i]);
                }
                listaFinal = listaFinal.Union(listaLectura).ToList();
            }
            return listaFinal;
        }
        /// <summary>
        /// Actualiza en la tabla ConfigAutocompletarProy el nuevo valor dado
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <param name="configAuto">Lista de nuevos valores para el proyecto</param>
        public void ActualizarConfigAutocompletar(Guid pProyectoID, List<string> configAuto)
        {
            //Controlar los vacios
            string valor = configAuto.Count == 0 ? "" : configAuto[0];
            for (int i = 1; i < configAuto.Count; i++)
            {
                valor = valor + "|" + configAuto[i];
            }
            List<ConfigAutocompletarProy> filas = mEntityContext.ConfigAutocompletarProy.Where(proy => proy.ProyectoID.Equals(pProyectoID)).ToList();
            int numFilas = filas.Count();
            if (numFilas == 0)
            {
                ConfigAutocompletarProy confAuto = new ConfigAutocompletarProy();
                confAuto.OrganizacionID = ProyectoAD.MetaOrganizacion;
                confAuto.ProyectoID = pProyectoID;
                confAuto.Clave = "FacetasCom";
                confAuto.Valor = valor;
                confAuto.PestanyaID = null;
                mEntityContext.ConfigAutocompletarProy.Add(confAuto);
            }
            else if (numFilas == 1)
            {
                ConfigAutocompletarProy filaActualizar = filas.First();
                filaActualizar.Valor = valor;
            }
            else
            {
                foreach (ConfigAutocompletarProy fila in filas)
                {
                    mEntityContext.EliminarElemento(fila);
                }
                ConfigAutocompletarProy confAuto = new ConfigAutocompletarProy();
                confAuto.OrganizacionID = ProyectoAD.MetaOrganizacion;
                confAuto.ProyectoID = pProyectoID;
                confAuto.Clave = "FacetasCom";
                confAuto.Valor = valor;
                confAuto.PestanyaID = null;
                mEntityContext.ConfigAutocompletarProy.Add(confAuto);
            }
            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Obtiene de la tabla ConfigSearchProy los valores de search para cada proyecto
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <returns>Lista con los valores de search de cada proyecto</returns>
        public List<string> ObtenerConfigSearch(Guid pProyectoID)
        {
            List<string> listaStrings = mEntityContext.ConfigSearchProy.Where(proy => proy.ProyectoID.Equals(pProyectoID)).Select(proy => proy.Valor).ToList();
            string[] separados;
            List<string> listaFinal = new List<string>();
            List<string> listaLectura = new List<string>();
            foreach (string valor in listaStrings)
            {
                separados = valor.Split('|');
                for (int i = 0; i < separados.Length; i++)
                {
                    listaLectura.Add(separados[i]);
                }
                listaFinal = listaFinal.Union(listaLectura).ToList();
            }
            return listaFinal;
        }
        /// <summary>
        /// Actualiza en la tabla ConfigSearchProy el nuevo valor dado
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <param name="configSearch">Lista de nuevos valores para el proyecto</param>
        public void ActualizarConfigSearch(Guid pProyectoID, List<string> configSearch)
        {
            //Controlar los vacios
            string valor = configSearch.Count == 0 ? "" : configSearch[0];
            for (int i = 1; i < configSearch.Count; i++)
            {
                valor = valor + "|" + configSearch[i];
            }
            List<ConfigSearchProy> filas = mEntityContext.ConfigSearchProy.Where(proy => proy.ProyectoID.Equals(pProyectoID)).ToList();
            int numFilas = filas.Count();
            if (numFilas == 0)
            {
                ConfigSearchProy confSearch = new ConfigSearchProy();
                confSearch.OrganizacionID = ProyectoAD.MetaOrganizacion;
                confSearch.ProyectoID = pProyectoID;
                confSearch.Clave = "FacetasCom";
                confSearch.Valor = valor;
                mEntityContext.ConfigSearchProy.Add(confSearch);
            }
            else if (numFilas == 1)
            {
                ConfigSearchProy confSearch = filas.FirstOrDefault();
                confSearch.Valor = valor;
            }
            else
            {
                foreach (ConfigSearchProy fila in filas)
                {
                    mEntityContext.EliminarElemento(fila);
                }
                ConfigSearchProy confSearch = new ConfigSearchProy();
                confSearch.OrganizacionID = ProyectoAD.MetaOrganizacion;
                confSearch.ProyectoID = pProyectoID;
                confSearch.Clave = "FacetasCom";
                confSearch.Valor = valor;
                mEntityContext.ConfigSearchProy.Add(confSearch);
            }
            ActualizarBaseDeDatosEntityContext();
        }
        /// <summary>
        /// Obtiene los parámetros de ConfiguracionEnvioCorreo de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <returns>Lista con los parámetros de la tabla ConfiguracionEnvioCorreo de un proyecto</returns>
        public ConfiguracionEnvioCorreo ObtenerConfiguracionEnvioCorreo(Guid pProyectoID)
        {
            return mEntityContext.ConfiguracionEnvioCorreo.FirstOrDefault(item => item.ProyectoID.Equals(pProyectoID));
        }

        /// <summary>
        /// Obtiene los parámetros de ConfiguracionEnvioCorreo de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <returns>Lista con los parámetros de la tabla ConfiguracionEnvioCorreo de un proyecto</returns>
        public ConfiguracionEnvioCorreo ObtenerFilaConfiguracionEnvioCorreo(Guid pProyectoID)
        {
            return mEntityContext.ConfiguracionEnvioCorreo.FirstOrDefault(fila => fila.ProyectoID == pProyectoID);
        }

        /// <summary>
        /// Guarda los parámetros de ConfiguracionEnvioCorreo de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">parámetros de la tabla ConfiguracionEnvioCorreo de un proyecto</</param>
        public void GuardarFilaConfiguracionEnvioCorreo(ConfiguracionEnvioCorreo pFilaConfiguracion, bool pEsFilaNueva)
        {
            if (pEsFilaNueva)
            {
                mEntityContext.ConfiguracionEnvioCorreo.Add(pFilaConfiguracion);
            }
            else
            {
                mEntityContext.Entry(pFilaConfiguracion).State = EntityState.Modified;
            }

            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Guarda los parámetros de ConfiguracionEnvioCorreo de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">parámetros de la tabla ConfiguracionEnvioCorreo de un proyecto</</param>
        public void BorrarFilaConfiguracionEnvioCorreo(Guid pProyectoID)
        {
            ConfiguracionEnvioCorreo filaConfig = new ConfiguracionEnvioCorreo { ProyectoID = pProyectoID };

            mEntityContext.EliminarElemento(filaConfig);

            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Obtiene la lista de proyectos (TablaBaseProyectoID) que tienen grafo de dbpedia
        /// </summary>
        /// <returns></returns>
        public List<int> ObtenerListaTablaBaseProyectoIDConGrafoDbPedia()
        {
           return mEntityContext.ParametroProyecto.Join(mEntityContext.Proyecto, paramProy => paramProy.ProyectoID, proy => proy.ProyectoID, (paramProy, proy) => new
            {
                ParametroProyecto = paramProy,
                Proyecto = proy
            }).Where(item => item.ParametroProyecto.Parametro.Equals(ProyectoTieneGrafoDbPedia) && item.ParametroProyecto.Valor.Equals("1")).Select(item => item.Proyecto.TablaBaseProyectoID).Union(mEntityContext.ParametroProyecto.Join(mEntityContext.Proyecto, paramProy => paramProy.ProyectoID, proy => proy.ProyectoSuperiorID.Value, (paramProy, proy) => new
            {
                ParametroProyecto = paramProy,
                Proyecto = proy
            }).Where(item => item.ParametroProyecto.Parametro.Equals(ProyectoTieneGrafoDbPedia) && item.ParametroProyecto.Valor.Equals("1") && item.Proyecto.ProyectoSuperiorID.HasValue).Select(item => item.Proyecto.TablaBaseProyectoID)).Distinct().ToList();
        }

        /// <summary>
        /// Obtiene una lista con los proyectos en los que se deben enviar las notificaciones de las suscripciones
        /// </summary>
        /// <returns></returns>
        public List<Guid> ObtenerListaProyectosConNotificacionesDeSuscripciones()
        {
            return mEntityContext.ParametroProyecto.Join(mEntityContext.Proyecto, paramProy => paramProy.ProyectoID, proy => proy.ProyectoID, (paramProy, proy) => new
            {
                ParametroProyecto = paramProy,
                Proyecto = proy
            }).Where(item => item.ParametroProyecto.Parametro.Equals(TiposParametrosAplicacion.EnviarNotificacionesDeSuscripciones) && item.ParametroProyecto.Valor.Equals("true")).Select(item => item.Proyecto.ProyectoID).ToList();
        }

        /// <summary>
        /// Obtiene el tamaño del pool para redis configurado para un dominio
        /// </summary>
        /// <param name="pDominio">dominio a obtener su configuración</param>
        /// <returns>Obtiene el tamaño configurado para este dominio, 0 si no hay nada configurado</returns>
        public int ObtenerTamanioPoolRedisParaDominio(string pDominio)
        {
            string sqlConfiguracionPoolRedis = "select \"Valor\" from \"ParametroDominio\" where \"Dominio\" = " + IBD.ToParam("dominio") + " AND \"Parametro\"='" + ParametroAD.TamanioPoolRedis + "'";

            if (EsPostgres())
            {
                sqlConfiguracionPoolRedis = "select \"Valor\" from dbo.\"ParametroDominio\" where \"Dominio\" = " + IBD.ToParam("dominio") + " AND \"Parametro\"='" + ParametroAD.TamanioPoolRedis + "'";
            }

            DbCommand commandsqlConfiguracionPoolRedis = ObtenerComando(sqlConfiguracionPoolRedis);
            AgregarParametro(commandsqlConfiguracionPoolRedis, IBD.ToParam("dominio"), DbType.String, pDominio);

            object resultado = EjecutarEscalar(commandsqlConfiguracionPoolRedis);
            int tamanio = 0;
            if (resultado != null && resultado is string)
            {
                int.TryParse((string)resultado, out tamanio);
            }

            return tamanio;
        }

        /// <summary>
        /// Obtiene los idiomas configurados para un dominio
        /// </summary>
        /// <param name="pDominio">dominio a obtener su configuración</param>
        /// <returns>Obtiene los idiomas configurados para un dominio</returns>
        public Dictionary<string, string> ObtenerIdiomasPorDominio(string pDominio)
        {
            Dictionary<string, string> listaIdiomas = new Dictionary<string, string>();

            string sqlIdiomas = "SELECT \"Valor\" from \"ParametroDominio\" where (\"Dominio\" = " + IBD.ToParam("dominioHTTP") + " OR \"Dominio\" = " + IBD.ToParam("dominioHTTPS") + ") AND \"Parametro\"='" + ListaIdiomas + "'";

            if (EsPostgres())
            {
                sqlIdiomas = "SELECT \"Valor\" from dbo.\"ParametroDominio\" where (\"Dominio\" = " + IBD.ToParam("dominioHTTP") + " OR \"Dominio\" = " + IBD.ToParam("dominioHTTPS") + ") AND \"Parametro\"='" + ListaIdiomas + "'";
            }

            DbCommand commandsqlIdiomas = ObtenerComando(sqlIdiomas);
            AgregarParametro(commandsqlIdiomas, IBD.ToParam("dominioHTTP"), DbType.String, "http://" + pDominio);
            AgregarParametro(commandsqlIdiomas, IBD.ToParam("dominioHTTPS"), DbType.String, "https://" + pDominio);

            object resultado = EjecutarEscalar(commandsqlIdiomas);

            if (resultado != null && resultado is string)
            {
                string[] separador = new string[] { "&&&" };
                string[] idiomasConfigurados = ((string)resultado).Split(separador, StringSplitOptions.RemoveEmptyEntries);

                foreach (string idioma in idiomasConfigurados)
                {
                    listaIdiomas.Add(idioma.Split('|')[0], idioma.Split('|')[1]);
                }
            }

            return listaIdiomas;
        }

        /// <summary>
        /// Obtiene los idiomas configurados para todos los dominios de la plataforma
        /// </summary>
        /// <returns>Obtiene los idiomas configurados para todos los dominios de la plataforma</returns>
        public Dictionary<string, string> ObtenerIdiomasDeTodosLosDominios()
        {
            Dictionary<string, string> listaIdiomas = new Dictionary<string, string>();

            string sqlIdiomas = "SELECT \"Valor\" from \"ParametroDominio\" where \"Parametro\"='" + ListaIdiomas + "'";

            if (EsPostgres())
            {
                sqlIdiomas = "SELECT \"Valor\" from dbo.\"ParametroDominio\" where \"Parametro\"='" + ListaIdiomas + "'";
            }


            DbCommand commandsqlIdiomas = ObtenerComando(sqlIdiomas);

            IDataReader reader = null;
            string[] separador = new string[] { "&&&" };
            try
            {
                reader = EjecutarReader(commandsqlIdiomas);

                while (reader.Read())
                {
                    string idiomasDominio = reader.GetString(0);
                    if (!string.IsNullOrEmpty(idiomasDominio))
                    {
                        string[] idiomasConfigurados = idiomasDominio.Split(separador, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string idioma in idiomasConfigurados)
                        {
                            if (!listaIdiomas.ContainsKey(idioma.Split('|')[0]))
                            {
                                listaIdiomas.Add(idioma.Split('|')[0], idioma.Split('|')[1]);
                            }
                        }
                    }
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }

            return listaIdiomas;
        }

        /// <summary>
        /// Obtiene por dominio el nombrecorto de las comunidades que no tienen nombrecorto en la URL
        /// </summary>
        /// <returns></returns>
        public List<string> ObtenerNombresDeProyectosSinNombreCortoEnURL()
        {
            return mEntityContext.ParametroProyecto.Join(mEntityContext.Proyecto, paramProy => paramProy.ProyectoID, proy => proy.ProyectoID, (paramProy, proy) => new
            {
                ParametroProyecto = paramProy,
                Proyecto = proy
            }).Where(item => item.ParametroProyecto.Parametro.Equals(ParametroAD.ProyectoSinNombreCortoEnURL)).Select(item => new { NombreCorto = item.Proyecto.NombreCorto, Valor = item.ParametroProyecto.Valor }).ToDictionary(item => item.NombreCorto, item => item.Valor).Where(item => item.Value.Equals("1")).Select(item => item.Key).ToList();
        }

        /// <summary>
        /// Indica la ruta relativa a los estilos, si no es el ID del proyecto (ej: "EcosistemaDidactalia")
        /// </summary>
        /// <returns></returns>
        public string ObtenerPathEstilos(Guid pProyectoID)
        {
            string parametro = mEntityContext.ParametroProyecto.Where(paramProy => paramProy.Parametro.Equals(RutaEstilos) && paramProy.ProyectoID.Equals(pProyectoID)).Select(item => item.Valor).FirstOrDefault();

            if (string.IsNullOrEmpty(parametro))
            {
                if (pProyectoID.Equals(ProyectoAD.MetaProyecto))
                {
                    parametro = "ecosistema";
                }
                else
                {
                    parametro = pProyectoID.ToString();
                }
            }

            return parametro;
        }

        /// <summary>
        /// Indica la ruta relativa a los estilos, si no es el ID del proyecto (ej: "EcosistemaDidactalia")
        /// </summary>
        /// <returns></returns>
        public int ObtenerSegundosEntrePeticionServiceBus(Guid pProyectoID)
        {

            string parametro = mEntityContext.ParametroProyecto.Where(item => item.Parametro.Equals(ServiceBusSegundos) && item.ProyectoID.Equals(pProyectoID)).Select(item => item.Valor).FirstOrDefault();
            return int.Parse(parametro);
        }

        /// <summary>
        /// Indica la ruta relativa a los estilos, si no es el ID del proyecto (ej: "EcosistemaDidactalia")
        /// </summary>
        /// <returns></returns>
        public int ObtenerReintentosObtenerFichero(Guid pProyectoID)
        {
            string parametro = mEntityContext.ParametroProyecto.Where(item => item.Parametro.Equals(ServiceBusReintentos) && item.ProyectoID.Equals(pProyectoID)).Select(item => item.Valor).FirstOrDefault();
            return int.Parse(parametro);
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
            ParametroProyecto parametroProyecto = mEntityContext.ParametroProyecto.FirstOrDefault(item => item.ProyectoID.Equals(pProyectoID) && item.OrganizacionID.Equals(pOrganizacionID) && item.Parametro.Equals(pParametro));
            if (parametroProyecto != null) //Hay que crear el parámetro
            {
                parametroProyecto.Valor = pValor;
            }
            else
            {
                parametroProyecto = new ParametroProyecto()
                {
                    OrganizacionID = pOrganizacionID,
                    ProyectoID = pProyectoID,
                    Parametro = pParametro,
                    Valor = pValor
                };
                mEntityContext.ParametroProyecto.Add(parametroProyecto);
            }
            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Borra un parametro de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <param name="pOrganizacionID">ID de la organización del proyecto</param>
        /// <param name="pParametro">Parámetro</param>
        public void BorrarParametroDeProyecto(Guid pProyectoID, Guid pOrganizacionID, string pParametro)
        {
            ParametroProyecto parametroProyecto = mEntityContext.ParametroProyecto.FirstOrDefault(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID) && item.Parametro.Equals(pParametro));
            if (parametroProyecto != null)
            {
                mEntityContext.EliminarElemento(parametroProyecto);
            }

            ActualizarBaseDeDatosEntityContext();
        }

        #endregion

        public List<Guid> ObtenerProyectosQueAgrupanEventosRegistroHome()
        {
            return mEntityContext.ParametroProyecto.Where(item => item.Parametro.Equals("AgruparRegistrosUsuariosEnProyecto") && item.Valor.Equals("1")).Select(item => item.ProyectoID).ToList();
        }

        public bool ExisteNombrePoliticaCookiesMetaproyecto()
        {
            return mEntityContext.ParametroProyecto.Any(item => item.Parametro.Equals("NombrePoliticaCookies") && item.ProyectoID.Equals(ProyectoAD.MetaProyecto));
        }
        
        public string ObtenerNombrePoliticaCookiesMetaproyecto()
        {
            return mEntityContext.ParametroProyecto.Where(item => item.Parametro.Equals("NombrePoliticaCookies") && item.ProyectoID.Equals(ProyectoAD.MetaProyecto)).Select(item => item.Valor).FirstOrDefault();
        }

        #endregion

        #endregion
    }
}
