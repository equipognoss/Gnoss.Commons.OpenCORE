using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Live;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.CL.Live
{
    public class LiveUsuariosCL : BaseCL
    {
        #region Miembros

        /// <summary>
        /// Clave MAESTRA de la caché
        /// </summary>
        private readonly string[] mMasterCacheKeyArray = { NombresCL.LIVE + "Usuarios" };

        /// <summary>
        /// Duracion de las claves (30 dias)
        /// </summary>
        public const double DURACION_CLAVES = (double)(60 * 60 * 24 * 30);

        /// <summary>
        /// Duración de la cache de tesauro (72 horas)
        /// </summary>
        private readonly string CADENA_GRUPOS_PROYECTO_LIVE = NombresCL.LIVE + "GrupoProyectoUsuarios";
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructores

        /// <summary>
        /// Constructor a partir del fichero de configuración de base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos LIVE</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones en el LIVE: FALSE. En caso contrario TRUE</param>
        public LiveUsuariosCL(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<LiveUsuariosCL> logger, ILoggerFactory loggerFactory)
            : base(null, "liveUsuarios", entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #endregion

        #region Metodos

        #region Perfil

        /// <summary>
        /// Obtiene el HTML del live de un usuario de GNOSS conectado en un idioma concreto
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pPerfilID">Identificador del perfil del usuario</param>
        /// <param name="pIdioma">Idioma del usuario</param>
        /// <param name="pFilasIdentidadSinActualizacion">Filas del perfil actual del usuario de las identidades para las que este usuario no quiere ver notificaciones en su home</param>
        /// <returns>String con el HTML</returns>
        public List<object> ObtenerLivePerfilUsuario(Guid pUsuarioID, Guid pPerfilID, string pIdioma, List<Guid> pListaProyectosSinActualizacion)
        {
            return ObtenerLivePerfilUsuario(pUsuarioID, pPerfilID, pIdioma, pListaProyectosSinActualizacion, 1, 10);
        }

        /// <summary>
        /// Obtiene el HTML del live de un usuario de GNOSS conectado en un idioma concreto
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pPerfilID">Identificador del perfil del usuario</param>
        /// <param name="pIdioma">Idioma del usuario</param>
        /// <param name="pFilasIdentidadSinActualizacion">Filas del perfil actual del usuario de las identidades para las que este usuario no quiere ver notificaciones en su home</param>
        /// <returns>String con el HTML</returns>
        public List<object> ObtenerLivePerfilUsuario(Guid pUsuarioID, Guid pPerfilID, string pIdioma, List<Guid> pListaProyectosSinActualizacion, int pInicio, int pFin)
        {
            string rawKey = string.Concat("LiveUsuario_" + pUsuarioID, "_", pPerfilID, "_");

            List<object> listaResultados = ObtenerRangoDeSortedList(rawKey, pInicio, pFin);

            return listaResultados;

            //if (listaResultados != null)
            //{
            //    return ObtenerListaHTMLDeLista(listaResultados, pIdioma, pListaProyectosSinActualizacion);
            //}
            //else
            //{
            //    return null;
            //}
        }

        public int ObtenerNumElementosPerfilUsuario(Guid pUsuarioID, Guid pPerfilID)
        {
            int numElementos = 0;

            string rawKey = string.Concat("LiveUsuario_" + pUsuarioID, "_", pPerfilID, "_");

            numElementos = base.ObtenerNumElementosDeSortedSet(rawKey);
            return numElementos;
        }

        public void EliminaElementosPerfilUsuario(Guid pUsuarioID, Guid pPerfilID, int pInicio, int pFin)
        {
            string rawKey = string.Concat("LiveUsuario_" + pUsuarioID, "_", pPerfilID, "_");

            base.EliminaElementosDeSortedSet(rawKey, pInicio, pFin);
        }

        /// <summary>
        /// Agrega el HTML del live de una comunidad para un usuario que no esta registrado en GNOSS
        /// </summary>
        /// <param name="pIdioma">Idioma del usuario</param>
        public int AgregarLivePerfilUsuario(Guid pUsuarioID, Guid pPerfilID, object pObjeto, int pScore)
        {
            string rawKey = string.Concat("LiveUsuario_" + pUsuarioID, "_", pPerfilID, "_");

            return AgregarObjetoASortedSet(rawKey, pObjeto, pScore);
        }

        /// <summary>
        /// Elimina el HTML del live de una comunidad para un usuario que no esta registrado en GNOSS
        /// </summary>
        /// <param name="pIdioma">Idioma del usuario</param>
        public bool EliminarLivePerfilUsuario(Guid pUsuarioID, Guid pPerfilID, object pObjeto)
        {
            string rawKey = string.Concat("LiveUsuario_" + pUsuarioID, "_", pPerfilID, "_");

            return EliminarObjetoDeSortedSet(rawKey, pObjeto.ToString());
        }

        #endregion

        #region Proyecto_Usuario_Suscripciones

        /// <summary>
        /// Obtiene el HTML del live de un usuario de GNOSS conectado en un idioma concreto
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pIdioma">Idioma del usuario</param>
        /// <param name="pFilasIdentidadSinActualizacion">Filas del perfil actual del usuario de las identidades para las que este usuario no quiere ver notificaciones en su home</param>
        /// <returns>String con el HTML</returns>
        public List<object> ObtenerLiveProyectoUsuarioSuscripciones(Guid pUsuarioID, Guid pProyectoID, string pIdioma)
        {
            return ObtenerLiveProyectoUsuarioSuscripciones(pUsuarioID, pProyectoID, pIdioma, 1, 10);
        }

        /// <summary>
        /// Obtiene el HTML del live de un usuario de GNOSS conectado en un idioma concreto
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pIdioma">Idioma del usuario</param>
        /// <param name="pFilasIdentidadSinActualizacion">Filas del perfil actual del usuario de las identidades para las que este usuario no quiere ver notificaciones en su home</param>
        /// <returns>String con el HTML</returns>
        public List<object> ObtenerLiveProyectoUsuarioSuscripciones(Guid pUsuarioID, Guid pProyectoID, string pIdioma, int pInicio, int pFin)
        {
            string rawKey = string.Concat("LiveProyectoSusc_", pProyectoID, "_", pUsuarioID, "_");

            List<object> listaResultados = ObtenerRangoDeSortedList(rawKey, pInicio, pFin);

            return listaResultados;
            //return ObtenerListaHTMLDeLista(listaResultados, pIdioma);
        }

        /// <summary>
        /// Obtiene el HTML del live de un usuario de GNOSS conectado en un idioma concreto
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pIdioma">Idioma del usuario</param>
        /// <returns>String con el HTML</returns>
        public int ObtenerLiveProyectoUsuarioSuscripcionesPorScore(Guid pUsuarioID, Guid pProyectoID, string pIdioma, int pMinScore, int pNumElementos, List<object> pListaResultados)
        {
            string rawKey = string.Concat("LiveProyectoSusc_", pProyectoID, "_", pUsuarioID, "_");

            List<object> listaResultados = ObtenerRangoDeSortedListPorScore(rawKey, pMinScore, pNumElementos);

            int score = pMinScore;

            pListaResultados.AddRange(listaResultados);

            if (pListaResultados.Count > 0)
            {
                score = (int)ClienteRedisEscritura.CreateSequence(ObtenerClaveCache(rawKey).ToLower()).ZScore(listaResultados[0].ToString()).Result + 1;
            }
            return score;
        }

        public int ObtenerNumElementosProyectoUsuarioSuscripciones(Guid pUsuarioID, Guid pProyectoID)
        {
            int numElementos = 0;

            string rawKey = string.Concat("LiveProyectoSusc_", pProyectoID, "_", pUsuarioID, "_");

            numElementos = base.ObtenerNumElementosDeSortedSet(rawKey);
            return numElementos;
        }

        public void EliminaElementosProyectoUsuarioSuscripciones(Guid pUsuarioID, Guid pProyectoID, int pInicio, int pFin)
        {
            string rawKey = string.Concat("LiveProyectoSusc_", pProyectoID, "_", pUsuarioID, "_");

            base.EliminaElementosDeSortedSet(rawKey, pInicio, pFin);
        }

        /// <summary>
        /// Agrega el HTML del live de una comunidad para un usuario que no esta registrado en GNOSS
        /// </summary>
        /// <param name="pIdioma">Idioma del usuario</param>
        public int AgregarLiveProyectoUsuarioSuscripciones(Guid pUsuarioID, Guid pProyectoID, object pObjeto, int pScore)
        {
            string rawKey = string.Concat("LiveProyectoSusc_", pProyectoID, "_", pUsuarioID, "_");

            return AgregarObjetoASortedSet(rawKey, pObjeto, pScore);
        }

        /// <summary>
        /// Elimina el HTML del live de una comunidad para un usuario que no esta registrado en GNOSS
        /// </summary>
        /// <param name="pIdioma">Idioma del usuario</param>
        public bool EliminarLiveProyectoUsuarioSuscripciones(Guid pUsuarioID, Guid pProyectoID, object pObjeto)
        {
            string rawKey = string.Concat("LiveProyectoSusc_", pProyectoID, "_", pUsuarioID, "_");

            return EliminarObjetoDeSortedSet(rawKey, pObjeto.ToString());
        }

        #endregion

        #region Usuario_Suscripciones


        /// <summary>
        /// Obtiene el HTML del live de un usuario de GNOSS conectado en un idioma concreto
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pIdioma">Idioma del usuario</param>
        /// <param name="pFilasIdentidadSinActualizacion">Filas del perfil actual del usuario de las identidades para las que este usuario no quiere ver notificaciones en su home</param>
        /// <returns>String con el HTML</returns>
        public List<object> ObtenerLiveUsuarioSuscripciones(Guid pUsuarioID, string pIdioma)
        {
            return ObtenerLiveUsuarioSuscripciones(pUsuarioID, pIdioma, 1, 10);
        }

        /// <summary>
        /// Obtiene el HTML del live de un usuario de GNOSS conectado en un idioma concreto
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pIdioma">Idioma del usuario</param>
        /// <param name="pFilasIdentidadSinActualizacion">Filas del perfil actual del usuario de las identidades para las que este usuario no quiere ver notificaciones en su home</param>
        /// <returns>String con el HTML</returns>
        public List<object> ObtenerLiveUsuarioSuscripciones(Guid pUsuarioID, string pIdioma, int pInicio, int pFin)
        {
            string rawKey = string.Concat("LiveSusc_", pUsuarioID, "_");

            List<object> listaResultados = ObtenerRangoDeSortedList(rawKey, pInicio, pFin);

            return listaResultados;
            //return ObtenerListaHTMLDeLista(listaResultados, pIdioma);
        }

        public int ObtenerNumElementosUsuarioSuscripciones(Guid pUsuarioID)
        {
            int numElementos = 0;

            string rawKey = string.Concat("LiveSusc_", pUsuarioID, "_");

            numElementos = base.ObtenerNumElementosDeSortedSet(rawKey);
            return numElementos;
        }

        public void EliminaElementosUsuarioSuscripciones(Guid pUsuarioID, int pInicio, int pFin)
        {
            string rawKey = string.Concat("LiveSusc_", pUsuarioID, "_");

            base.EliminaElementosDeSortedSet(rawKey, pInicio, pFin);
        }

        /// <summary>
        /// Agrega el una clave de cache de las suscripciones para un usuario registrado en GNOSS
        /// </summary>
        /// <param name="pUsuarioID"></param>
        /// <param name="pObjeto"></param>
        /// <param name="pScore"></param>
        public int AgregarLiveUsuarioSuscripciones(Guid pUsuarioID, object pObjeto, int pScore)
        {
            string rawKey = string.Concat("LiveSusc_", pUsuarioID, "_");

            EliminarObjetoDeSortedSet(rawKey, pObjeto + "_leido");

            return AgregarObjetoASortedSet(rawKey, pObjeto, pScore);
        }

        /// <summary>
        /// Elimina el una clave de cache de las suscripciones para un usuario registrado en GNOSS
        /// </summary>
        /// <param name="pIdioma">Idioma del usuario</param>
        public bool EliminarLiveUsuarioSuscripciones(Guid pUsuarioID, object pObjeto)
        {
            string rawKey = string.Concat("LiveSusc_", pUsuarioID, "_");

            return EliminarObjetoDeSortedSet(rawKey, pObjeto.ToString());
        }

        public void MarcarSuscripcionLeida(Guid pUsuarioID, Guid pElementoID, Guid pProyectoID, string pTipo)
        {
            string rawKey = string.Concat("LiveSusc_", pUsuarioID, "_");

            string objeto = pTipo + "_" + pElementoID + "_" + pProyectoID;

            int pScore = (int)ClienteRedisEscritura.CreateSequence(ObtenerClaveCache(rawKey.ToLower())).ZScore(objeto).Result;

            EliminarObjetoDeSortedSet(rawKey, objeto);
            AgregarObjetoASortedSet(rawKey, objeto + "_leido", pScore);
        }

        #endregion 

        #region Proyecto_Usuario

        /// <summary>
        /// Obtiene el HTML del live de un usuario de GNOSS conectado en un idioma concreto
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pIdioma">Idioma del usuario</param>
        /// <param name="pFilasIdentidadSinActualizacion">Filas del perfil actual del usuario de las identidades para las que este usuario no quiere ver notificaciones en su home</param>
        /// <returns>String con el HTML</returns>
        public List<object> ObtenerLiveProyectoUsuario(Guid? pUsuarioID, Guid pProyectoID, string pIdioma)
        {
            return ObtenerLiveProyectoUsuario(pUsuarioID, pProyectoID, pIdioma, 1, 10);
        }

        /// <summary>
        /// Obtiene el HTML del live de un usuario de GNOSS conectado en un idioma concreto
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pIdioma">Idioma del usuario</param>
        /// <param name="pFilasIdentidadSinActualizacion">Filas del perfil actual del usuario de las identidades para las que este usuario no quiere ver notificaciones en su home</param>
        /// <returns>String con el HTML</returns>
        public List<object> ObtenerLiveProyectoUsuario(Guid? pUsuarioID, Guid pProyectoID, string pIdioma, int pInicio, int pFin)
        {
            string rawKey = string.Concat("LiveProyecto_", pProyectoID, "_");
            if (pUsuarioID.HasValue)
            {
                rawKey = rawKey + pUsuarioID + "_";
            }

            List<object> listaResultados = ObtenerRangoDeSortedList(rawKey, pInicio, pFin);

            return listaResultados;
            //return ObtenerListaHTMLDeLista(listaResultados, pIdioma);
        }

        public List<object> ObtenerLiveProyectoUsuarioInvitado(Guid pProyectoID, string pIdioma, int pInicio, int pFin)
        {
            string rawKey = string.Concat("LiveProyectoInvitado_", pProyectoID, "_");

            if (!ExisteClaveEnCache(rawKey))
            {
                // Si no existe la clave para el invitado, se clona la del proyecto normal.
                ClonarLiveProyectoAInvitado(pProyectoID);
            }

            List<object> listaResultados = ObtenerRangoDeSortedList(rawKey, pInicio, pFin);

            return listaResultados;
            //return ObtenerListaHTMLDeLista(listaResultados, pIdioma);
        }

        public int ObtenerNumElementosProyectoUsuario(Guid? pUsuarioID, Guid pProyectoID)
        {
            int numElementos = 0;

            string rawKey = string.Concat("LiveProyecto_", pProyectoID, "_");
            if (pUsuarioID.HasValue)
            {
                rawKey += pUsuarioID + "_";
            }

            numElementos = base.ObtenerNumElementosDeSortedSet(rawKey);
            return numElementos;
        }

        public List<string> ObtenerListaHTMLDeLista(List<object> pLista, string pIdioma)
        {
            return ObtenerListaHTMLDeLista(pLista, pIdioma, null);
        }

        private List<string> ObtenerListaHTMLDeLista(List<object> pLista, string pIdioma, List<Guid> pListaProyectosSinActualizacion)
        {
            List<string> listaHtml = new List<string>();

            if (pLista != null)
            {
                string[] listaClaves = new string[pLista.Count];
                bool[] listaClavesSuscrLeida = new bool[pLista.Count];

                Dictionary<Guid, List<Guid>> listaClavesRecursosProyecto = new Dictionary<Guid, List<Guid>>();

                int cont = 0;
                foreach (string id in pLista)
                {
                    string[] parametros = id.Split('_');

                    if (pListaProyectosSinActualizacion != null && pListaProyectosSinActualizacion.Count > 0)
                    {
                        Guid proyID = Guid.Empty;

                        if (!Guid.TryParse(parametros[0], out proyID))
                        {
                            Guid.TryParse(parametros[1], out proyID);
                        }

                        if (pListaProyectosSinActualizacion.Contains(proyID))
                        {
                            //El usuario no quiere ver actualizaciones de este proyecto, voy a por el siguiente item
                            continue;
                        }
                    }

                    //if (parametros[0] == "0")
                    //{ 
                    //    Guid proyID = new Guid(parametros[1]);
                    //    Guid docID = new Guid(parametros[2]);

                    //    if (!listaClavesRecursosProyecto.ContainsKey(proyID))
                    //    {
                    //        listaClavesRecursosProyecto.Add(proyID, new List<Guid>());
                    //    }
                    //}

                    string rawKey = string.Concat("LiveElemento_", id + "_" + pIdioma);
                    rawKey = ObtenerClaveCache(rawKey).ToLower();
                    listaClavesSuscrLeida[cont] = rawKey.Contains("_leido");
                    listaClaves[cont] = rawKey.Replace("_leido", "");
                    cont++;
                }

                if (listaClaves.Length > 0)
                {
                    //foreach (Guid proyectoID in listaClavesRecursosProyecto.Keys)
                    //{
                    //    Es.Riam.Gnoss.Web.MVC.Controles.ControladorProyectoMVC controladorMVC = new ControladorProyectoMVC(UtilIdiomas.LanguageCode, BaseURL, BaseURLsContent, BaseURLStatic, mProyecto);
                    //}

                    Dictionary<string, object> listaResultados = ObtenerListaObjetosCache(listaClaves);
                    if (listaResultados != null)
                    {
                        foreach (string claveResultado in listaResultados.Keys)
                        {
                            //Marcar que no tiene html para montarlo en el control de actividad reciente


                            if (string.IsNullOrEmpty((string)listaResultados[claveResultado]))
                            {
                                string html = "[NOT_FOUND]" + claveResultado;

                                if (listaClavesSuscrLeida[listaHtml.Count])
                                {
                                    html = "[LEIDO]" + html;
                                }

                                listaHtml.Add(html);
                            }
                            else
                            {
                                string html = (string)listaResultados[claveResultado];

                                if (listaClavesSuscrLeida[listaHtml.Count] && html.IndexOf("class=\"resource") > 0)
                                {
                                    html = html.Insert(html.IndexOf("class=\"resource") + "class=\"resource".Length, " leido");
                                }

                                listaHtml.Add(html);
                            }

                        }
                    }
                }
            }

            return listaHtml;
        }

        public void EliminaElementosProyectoUsuario(Guid? pUsuarioID, Guid pProyectoID, int pInicio, int pFin)
        {
            string rawKey = string.Concat("LiveProyecto_", pProyectoID, "_");
            if (pUsuarioID.HasValue)
            {
                rawKey += pUsuarioID + "_";
            }
            base.EliminaElementosDeSortedSet(rawKey, pInicio, pFin);
        }

        public int ClonarLiveProyectoAUsu(Guid pUsuarioID, Guid pProyectoID)
        {
            string rawKey1 = string.Concat("LiveProyecto_", pProyectoID, "_");
            string rawKey2 = rawKey1 + pUsuarioID + "_";

            return ClonarSortedSet(rawKey1, rawKey2);
        }

        public int ClonarLiveProyectoAInvitado(Guid pProyectoID)
        {
            string rawKey1 = string.Concat("LiveProyecto_", pProyectoID, "_");
            string rawKey2 = string.Concat("LiveProyectoInvitado_", pProyectoID, "_");

            return ClonarSortedSet(rawKey1, rawKey2);
        }

        public int ClonarLiveProyectoAGrupo(Guid pProyectoID, Guid pGrupoID)
        {
            string rawKey1 = string.Concat("LiveProyecto_", pProyectoID, "_");
            string rawKey2 = string.Concat(CADENA_GRUPOS_PROYECTO_LIVE, "_", pProyectoID, "_", pGrupoID, "_");

            return ClonarSortedSet(rawKey1, rawKey2);
        }

        public int ClonarLiveGrupoProyectoAUsu(Guid pUsuarioID, Guid pProyectoID, Guid pGrupoID)
        {
            string rawKey1 = string.Concat(CADENA_GRUPOS_PROYECTO_LIVE, "_", pProyectoID, "_", pGrupoID, "_");
            string rawKey2 = string.Concat("LiveProyecto_", pProyectoID, "_", pUsuarioID, "_");

            return ClonarSortedSet(rawKey1, rawKey2);
        }

        public int ClonarLiveProyectoAHomeUsu(Guid pUsuarioID, Guid pPerfilID, Guid pProyectoID)
        {
            string rawKey1 = string.Concat("LiveProyecto_", pProyectoID, "_");
            string rawKey2 = string.Concat("LiveUsuario_" + pUsuarioID, "_", pPerfilID, "_");

            return ClonarSortedSet(rawKey1, rawKey2);
        }

        /// <summary>
        /// Agrega el HTML del live de una comunidad para un usuario que no esta registrado en GNOSS
        /// </summary>
        /// <param name="pIdioma">Idioma del usuario</param>
        public int AgregarLiveProyectoUsuario(Guid? pUsuarioID, Guid pProyectoID, object pObjeto, int pScore)
        {
            string rawKey = string.Concat("LiveProyecto_", pProyectoID, "_");
            if (pUsuarioID.HasValue)
            {
                rawKey += pUsuarioID + "_";
            }
            int score = -1;

            score = AgregarObjetoASortedSet(rawKey, pObjeto, pScore);

            return score;
        }

        public int AgregarLiveProyectoUsuarioInvitado(Guid pProyectoID, object pObjeto, int pScore)
        {
            string rawKey = string.Concat("LiveProyectoInvitado_", pProyectoID, "_");

            int score = -1;

            score = AgregarObjetoASortedSet(rawKey, pObjeto, pScore);

            return score;
        }

        /// <summary>
        /// Elimina el HTML del live de una comunidad para un usuario que no esta registrado en GNOSS
        /// </summary>
        /// <param name="pIdioma">Idioma del usuario</param>
        public bool EliminarLiveProyectoUsuario(Guid? pUsuarioID, Guid pProyectoID, object pObjeto)
        {
            string rawKey = string.Concat("LiveProyecto_", pProyectoID, "_");
            if (pUsuarioID.HasValue)
            {
                rawKey += pUsuarioID + "_";
            }

            return EliminarObjetoDeSortedSet(rawKey, pObjeto.ToString());
        }

        /// <summary>
        /// Elimina el HTML del live de una comunidad para un usuario que no esta registrado en GNOSS
        /// </summary>
        /// <param name="pIdioma">Idioma del usuario</param>
        public bool EliminarLiveProyectoUsuarioInvitado(Guid pProyectoID, object pObjeto)
        {
            string rawKey = string.Concat("LiveProyectoInvitado_", pProyectoID, "_");
            return EliminarObjetoDeSortedSet(rawKey, pObjeto.ToString());
        }

        /// <summary>
        /// Agrega el HTML del live de una comunidad para un usuario que no esta registrado en GNOSS
        /// </summary>
        /// <param name="pIdioma">Idioma del usuario</param>
        public int AgregarLiveProyectoRegistrados(Guid pProyectoID, object pObjeto, int pScore)
        {
            string rawKey = string.Concat("LiveProyectoRegistrados_", pProyectoID, "_");
            int score = -1;
            score = AgregarObjetoASortedSet(rawKey, pObjeto, pScore);
            return score;
        }

        /// <summary>
        /// Elimina el HTML del live de una comunidad para un usuario que no esta registrado en GNOSS
        /// </summary>
        /// <param name="pIdioma">Idioma del usuario</param>
        public bool EliminarLiveProyectoRegistrados(Guid pProyectoID, object pObjeto)
        {
            string rawKey = string.Concat("LiveProyectoRegistrados_", pProyectoID, "_");

            return EliminarObjetoDeSortedSet(rawKey, pObjeto.ToString());
        }

        #endregion

        #region Proyecto_Grupo

        public int ObtenerNumElementosProyectoGrupo(Guid? pGrupoID, Guid pProyectoID)
        {
            int numElementos = 0;

            string rawKey = string.Concat(CADENA_GRUPOS_PROYECTO_LIVE, "_", pProyectoID, "_");
            if (pGrupoID.HasValue)
            {
                rawKey += pGrupoID + "_";
            }

            numElementos = base.ObtenerNumElementosDeSortedSet(rawKey);
            return numElementos;
        }

        /// <summary>
        /// Agrega el HTML del live de una comunidad para un usuario que no esta registrado en GNOSS
        /// </summary>
        /// <param name="pIdioma">Idioma del usuario</param>
        public int AgregarLiveProyectoGrupo(Guid? pGrupoID, Guid pProyectoID, object pObjeto, int pScore)
        {
            string rawKey = string.Concat(CADENA_GRUPOS_PROYECTO_LIVE, "_", pProyectoID, "_");
            if (pGrupoID.HasValue)
            {
                rawKey += pGrupoID + "_";
            }
            int score = -1;

            score = AgregarObjetoASortedSet(rawKey, pObjeto, pScore);

            return score;
        }

        public void EliminaElementosProyectoGrupo(Guid? pGrupoID, Guid pProyectoID, int pInicio, int pFin)
        {
            string rawKey = string.Concat(CADENA_GRUPOS_PROYECTO_LIVE, "_", pProyectoID, "_");
            if (pGrupoID.HasValue)
            {
                rawKey += pGrupoID + "_";
            }
            base.EliminaElementosDeSortedSet(rawKey, pInicio, pFin);
        }

        /// <summary>
        /// Elimina el HTML del live de una comunidad para un grupo
        /// </summary>
        public bool EliminarLiveProyectoGrupo(Guid? pGrupoID, Guid pProyectoID, object pObjeto)
        {
            string rawKey = string.Concat(CADENA_GRUPOS_PROYECTO_LIVE, "_", pProyectoID, "_");
            if (pGrupoID.HasValue)
            {
                rawKey += pGrupoID + "_";
            }

            return EliminarObjetoDeSortedSet(rawKey, pObjeto.ToString());
        }

        #endregion

        #region Proyecto_Perfil_Usuario

        /// <summary>
        /// Obtiene el HTML del live de un usuario de GNOSS conectado en un idioma concreto
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pIdioma">Idioma del usuario</param>
        /// <param name="pFilasIdentidadSinActualizacion">Filas del perfil actual del usuario de las identidades para las que este usuario no quiere ver notificaciones en su home</param>
        /// <returns>String con el HTML</returns>
        public List<object> ObtenerLiveProyectoPerfilUsuario(Guid? pUsuarioID, Guid pPerfilID, Guid pProyectoID, string pIdioma)
        {
            return ObtenerLiveProyectoPerfilUsuario(pUsuarioID, pPerfilID, pProyectoID, pIdioma, 1, 10);
        }

        /// <summary>
        /// Obtiene el HTML del live de un usuario de GNOSS conectado en un idioma concreto
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pIdioma">Idioma del usuario</param>
        /// <param name="pFilasIdentidadSinActualizacion">Filas del perfil actual del usuario de las identidades para las que este usuario no quiere ver notificaciones en su home</param>
        /// <returns>String con el HTML</returns>
        public List<object> ObtenerLiveProyectoPerfilUsuario(Guid? pUsuarioID, Guid pPerfilID, Guid pProyectoID, string pIdioma, int pInicio, int pFin)
        {
            string rawKey = string.Concat("LiveProyectoPerfil_", pProyectoID, "_", pPerfilID, "_");

            if (pUsuarioID.HasValue)
            {
                rawKey = rawKey + pUsuarioID + "_";
            }

            List<object> listaResultados = ObtenerRangoDeSortedList(rawKey, pInicio, pFin);

            return listaResultados;
            //return ObtenerListaHTMLDeLista(listaResultados, pIdioma);
        }

        public int ObtenerNumElementosProyectoPerfilUsuario(Guid pPerfilID, Guid pProyectoID)
        {
            int numElementos = 0;

            string rawKey = string.Concat("LiveProyectoPerfil_", pProyectoID, "_", pPerfilID, "_");

            numElementos = base.ObtenerNumElementosDeSortedSet(rawKey);
            return numElementos;
        }


        public void EliminaElementosProyectoPerfilUsuario(Guid pPerfilID, Guid pProyectoID, int pInicio, int pFin)
        {
            string rawKey = string.Concat("LiveProyectoPerfil_", pProyectoID, "_", pPerfilID, "_");

            base.EliminaElementosDeSortedSet(rawKey, pInicio, pFin);
        }

        /// <summary>
        /// Agrega el HTML del live de una comunidad para un usuario que no esta registrado en GNOSS
        /// </summary>
        /// <param name="pIdioma">Idioma del usuario</param>
        public int AgregarLiveProyectoPerfilUsuario(Guid pPerfilID, Guid pProyectoID, object pObjeto, int pScore)
        {
            string rawKey = string.Concat("LiveProyectoPerfil_", pProyectoID, "_", pPerfilID, "_");

            return AgregarObjetoASortedSet(rawKey, pObjeto, pScore);
        }

        /// <summary>
        /// Elimina el HTML del live de una comunidad para un usuario que no esta registrado en GNOSS
        /// </summary>
        /// <param name="pIdioma">Idioma del usuario</param>
        public bool EliminarLiveProyectoPerfilUsuario(Guid pPerfilID, Guid pProyectoID, object pObjeto)
        {
            string rawKey = string.Concat("LiveProyectoPerfil_", pProyectoID, "_", pPerfilID, "_");

            return EliminarObjetoDeSortedSet(rawKey, pObjeto.ToString());
        }

        #endregion

        #region Proyecto_Perfil_Organizacion

        /// <summary>
        /// Obtiene el HTML del live de un usuario de GNOSS conectado en un idioma concreto
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pIdioma">Idioma del usuario</param>
        /// <param name="pFilasIdentidadSinActualizacion">Filas del perfil actual del usuario de las identidades para las que este usuario no quiere ver notificaciones en su home</param>
        /// <returns>String con el HTML</returns>
        public List<object> ObtenerLiveProyectoPerfilOrg(Guid? pUsuarioID, Guid pOrganizacionID, Guid pProyectoID, string pIdioma)
        {
            return ObtenerLiveProyectoPerfilOrg(pUsuarioID, pOrganizacionID, pProyectoID, pIdioma, 1, 10);
        }

        /// <summary>
        /// Obtiene el HTML del live de un usuario de GNOSS conectado en un idioma concreto
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pIdioma">Idioma del usuario</param>
        /// <param name="pFilasIdentidadSinActualizacion">Filas del perfil actual del usuario de las identidades para las que este usuario no quiere ver notificaciones en su home</param>
        /// <returns>String con el HTML</returns>
        public List<object> ObtenerLiveProyectoPerfilOrg(Guid? pUsuarioID, Guid pOrganizacionID, Guid pProyectoID, string pIdioma, int pInicio, int pFin)
        {
            string rawKey = string.Concat("LiveProyectoPerfilOrg_", pProyectoID, "_", pOrganizacionID, "_");

            if (pUsuarioID.HasValue)
            {
                rawKey = rawKey + pUsuarioID + "_";
            }

            List<object> listaResultados = ObtenerRangoDeSortedList(rawKey, pInicio, pFin);

            return listaResultados;
            //return ObtenerListaHTMLDeLista(listaResultados, pIdioma);
        }

        public int ObtenerNumElementosProyectoPerfilOrg(Guid pOrganizacionID, Guid pProyectoID)
        {
            int numElementos = 0;

            string rawKey = string.Concat("LiveProyectoPerfilOrg_", pProyectoID, "_", pOrganizacionID, "_");

            numElementos = base.ObtenerNumElementosDeSortedSet(rawKey);
            return numElementos;
        }

        public void EliminaElementosProyectoPerfilOrg(Guid pOrganizacionID, Guid pProyectoID, int pInicio, int pFin)
        {
            string rawKey = string.Concat("LiveProyectoPerfilOrg_", pProyectoID, "_", pOrganizacionID, "_");

            base.EliminaElementosDeSortedSet(rawKey, pInicio, pFin);
        }

        /// <summary>
        /// Agrega el HTML del live de una comunidad para un usuario que no esta registrado en GNOSS
        /// </summary>
        /// <param name="pIdioma">Idioma del usuario</param>
        public int AgregarLiveProyectoPerfilOrg(Guid pOrganizacionID, Guid pProyectoID, object pObjeto, int pScore)
        {
            string rawKey = string.Concat("LiveProyectoPerfilOrg_", pProyectoID, "_", pOrganizacionID, "_");

            return AgregarObjetoASortedSet(rawKey, pObjeto, pScore);
        }

        /// <summary>
        /// Elimina el HTML del live de una comunidad para un usuario que no esta registrado en GNOSS
        /// </summary>
        /// <param name="pIdioma">Idioma del usuario</param>
        public bool EliminarLiveProyectoPerfilOrg(Guid pOrganizacionID, Guid pProyectoID, object pObjeto)
        {
            string rawKey = string.Concat("LiveProyectoPerfilOrg_", pProyectoID, "_", pOrganizacionID, "_");

            return EliminarObjetoDeSortedSet(rawKey, pObjeto.ToString());
        }

        #endregion

        /// <summary>
        /// Agrega el HTML del live de una comunidad para un usuario que no esta registrado en GNOSS
        /// </summary>
        /// <param name="pClaveElementoProyecto">Clave del elemento más la clave del proyecto (ej: docID_proyID)</param>
        /// <param name="pIdioma">Idioma del usuario</param>
        public object ObtenerLiveElementoEnCache(string pClaveElementoProyecto, string pIdioma)
        {
            string rawKey = string.Concat("LiveElemento_", pClaveElementoProyecto + "_" + pIdioma);

            return ObtenerObjetoDeCache(rawKey, typeof(string));
        }

        /// <summary>
        /// Agrega el HTML del live de una comunidad para un usuario que no esta registrado en GNOSS
        /// </summary>
        /// <param name="pIdioma">Idioma del usuario</param>
        public void AgregarLiveElementoACache(Guid ProyectoID, int pTipo, Guid pDocumentoID, string pInfoExtra, string pIdioma, string pHtml)
        {
            string rawKey = string.Concat("LiveElemento_", pTipo + "_" + pDocumentoID + "_" + ProyectoID + pInfoExtra + "_" + pIdioma);
            if (pTipo == (int)TipoLive.AgrupacionNuevosMiembros)
            {
                rawKey = "LiveElemento_" + pTipo + "_" + ProyectoID + "_" + pIdioma;
            }

            AgregarObjetoCache(rawKey, pHtml, DURACION_CLAVES);
        }

        /// <summary>
        /// Elimina el HTML del live de una comunidad para un usuario.
        /// </summary>
        /// <param name="pIdioma">Idioma del usuario</param>
        public void EliminarLiveElementoDeCache(Guid ProyectoID, int pTipo, Guid pDocumentoID, string pInfoExtra, string pIdioma)
        {
            string rawKey = "LiveElemento_" + pTipo + "_" + pDocumentoID + "_" + ProyectoID + pInfoExtra + "_" + pIdioma;

            InvalidarCache(rawKey);
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Clave para la caché
        /// </summary>
        public override string[] ClaveCache
        {
            get
            {
                return mMasterCacheKeyArray;
            }
        }

        /// <summary>
        /// No queremos que se pieran las cachés LIVE, porque no se regeneran de manera automática
        /// </summary>
        protected override string VersionCache
        {
            get
            {
                return string.Empty;
            }
        }

        #endregion
    }
}
