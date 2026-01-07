using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Facetado;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Es.Riam.Gnoss.UtilServiciosWeb
{
    public class UtilServiciosFacetas
    {

        private VirtuosoAD mVirtuosoAD;
        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public UtilServiciosFacetas(LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<UtilServiciosFacetas> logger, ILoggerFactory loggerFactory)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mVirtuosoAD = virtuosoAD;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        private const string GETIDENTIDADID = "GETIDENTIDADID()";
        private const string GETUSUARIOID = "GETUSUARIOID()";
        private const string GNOSS_RELEVANCIA = "gnoss:relevancia";

        /// <summary>
        /// A partir de una URI obtiene el identificador del elemento (ej: http:gnoss.com/1111-1111 = 1111-1111)
        /// </summary>
        /// <param name="pUri">Uri que contiene el identificador de un elemento</param>
        /// <returns></returns>
        public Guid ObtenerIDDesdeURI(string pUri)
        {
            Guid idGuid = Guid.Empty;

            string id = "";
            if (pUri.Contains(":"))
            {
                id = pUri.Substring(pUri.IndexOf("gnoss") + 6);
            }
            else
            {
                id = pUri.ToUpper();
            }

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    idGuid = new Guid(id);
                }
            }
            catch (Exception e)
            {
                mLoggingService.GuardarLogError(e, mlogger);
            }

            return idGuid;
        }

        /// <summary>
        /// A partir de una URI obtiene el identificador del elemento (ej: http:gnoss.com/1111-1111 = 1111-1111)
        /// </summary>
        /// <param name="pUri">Uri que contiene el identificador de un elemento</param>
        /// <returns></returns>
        public string ObtenerIDTextoDesdeURI(string pUri)
        {
            string idGuid = Guid.Empty.ToString();

            string id = "";
            if (pUri.Contains(":"))
            {
                id = pUri.Substring(pUri.IndexOf("gnoss") + 6);
            }
            else
            {
                id = pUri.ToUpper();
            }

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    idGuid = id;
                }
            }
            catch (Exception e)
            {
                mLoggingService.GuardarLogError(e, mlogger);
            }

            return idGuid;
        }

        /// <summary>
        /// Extrae los parámetros de una búsqueda
        /// </summary>
        /// <param name="pParametros">Parámetros que ha introducido el usuario (los que van por la URL después de #)</param>
        public void ExtraerParametros(DataWrapperFacetas pFacetaDW, Guid pProyectoID, string pParametros, List<string> pListaItemsBusqueda, Dictionary<string, List<string>> pListaFiltros, Dictionary<string, List<string>> pListaFiltrosUsuario, Guid pIdentidadID)
        {
            bool orderDesc = false;
            int pag = 0;
            string filtroOrden = "";
            ExtraerParametros(pFacetaDW, pProyectoID, pParametros, pListaItemsBusqueda, ref orderDesc, ref pag, ref filtroOrden, pListaFiltros, pListaFiltrosUsuario, pIdentidadID);
        }

        public static Dictionary<string, List<string>> ExtraerParametrosNegados(string pParametros, out string pParametrosSinParametrosNegados)
        {
            char[] separador = { '|' };
            string[] args = pParametros.Split(separador, StringSplitOptions.RemoveEmptyEntries);
            pParametrosSinParametrosNegados = "";
            char[] separadores = { '=' };

            Dictionary<string, List<string>> listaFacetasNegadas = new Dictionary<string, List<string>>();
            string separadorParametrosSinNegar = "";
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("!"))
                {
                    string[] filtro = args[i].Split(separadores, StringSplitOptions.RemoveEmptyEntries);
                    string key = filtro[0];
                    string valor = HttpUtility.UrlDecode(filtro[1].Replace("+", "%2B"));

                    if (!listaFacetasNegadas.ContainsKey(key))
                    {
                        listaFacetasNegadas.Add(key, new List<string>());
                    }
                    listaFacetasNegadas[key].Add(valor.Replace("gnoss:", "http://gnoss/"));
                }
                else
                {
                    pParametrosSinParametrosNegados += $"{separadorParametrosSinNegar}{args[i]}";
                    separadorParametrosSinNegar = "|";
                }
            }

            return listaFacetasNegadas;
        }

        /// <summary>
        /// Extrae los parámetros de una búsqueda
        /// </summary>
        /// <param name="pParametros">Parámetros que ha introducido el usuario (los que van por la URL después de #)</param>
        /// <param name="pFiltroOrden"></param>
        /// <param name="pFiltrosSearchPersonalizados">Diccionario con los filtros tipo 'search' personalizados la clave es el nombre del filtro y el valor es 'WhereSPARQL','OrderBySPARQL','WhereFacetasSPARQL'</param>
        public void ExtraerParametros(DataWrapperFacetas pFacetaDW, Guid pProyectoID, string pParametros, List<string> pListaItemsBusqueda, ref bool pOrdenDescendente, ref int pPaginaActual, ref string pFiltroOrden, Dictionary<string, List<string>> pListaFiltros, Dictionary<string, List<string>> pListaFiltrosUsuario, Guid pIdentidadID, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados = null)
        {
            char[] separador = { '|' };
            string[] args = pParametros.Split(separador, StringSplitOptions.RemoveEmptyEntries);

            char[] separadores = { '=' };

            bool tieneFiltroOrden = !string.IsNullOrEmpty(pFiltroOrden) && !pFiltroOrden.Equals(GNOSS_RELEVANCIA);
            bool filtrarPorRelevancia = false;

            for (int i = 0; i < args.Length; i++)
            {
                if (!string.IsNullOrEmpty(args[i]))
                {
                    if (args[i].StartsWith("("))
                    {
                        if (args[i].Contains(";"))
                        {
                            // Es una búsqueda del prado chunga
                            // Mantener la parte del filtro:
                            string filtroOriginal = args[i].Substring(0, args[i].IndexOf(")") + 1);
                            string filtroTemp = args[i].Replace(filtroOriginal, "");

                            string[] filtros = filtroTemp.Split(separadores, StringSplitOptions.RemoveEmptyEntries);
                            string key = filtros[0].Substring(1);

                            string value = string.Empty;
                            for (int trozo = 1; trozo < filtros.Length; trozo++)
                            {
                                value += filtros[trozo] + "=";
                            }

                            if (value.EndsWith("="))
                            {
                                value = value.Substring(0, value.Length - 1);
                            }

                            AgregarKeyValorAListaFiltros(pFacetaDW, pProyectoID, pListaFiltros, pListaFiltrosUsuario, filtroOriginal + ";" + key, value);
                        }
                        else
                        {

                            string[] filtros = args[i].Split(separadores, StringSplitOptions.RemoveEmptyEntries);
                            string key = filtros[0].Substring(1);

                            string value = string.Empty;
                            for (int trozo = 1; trozo < filtros.Length; trozo++)
                            {
                                value += filtros[trozo] + "=";
                            }

                            if (value.EndsWith(")="))
                            {
                                value = value.Substring(0, value.Length - 2);
                            }

                            AgregarKeyValorAListaFiltros(pFacetaDW, pProyectoID, pListaFiltros, pListaFiltrosUsuario, key, value);
                        }
                    }
                    else
                    {
                        if (args[i].StartsWith("SPARQL", StringComparison.OrdinalIgnoreCase))
                        {
                            string filtroSPARQL = args[i].Substring(6);
                            if (filtroSPARQL.Contains("GETUSERID()"))
                            {
                                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                                Guid usuarioID = usuarioCN.ObtenerGuidUsuarioIDporIdentidadID(pIdentidadID);
                                filtroSPARQL = filtroSPARQL.Replace("GETUSERID()", $"<http://gnoss/{usuarioID.ToString().ToUpper()}>");
                            }

                            pListaFiltros.Add("SPARQL", new List<string>() { filtroSPARQL });

                        }
                        else
                        {
                            string[] filtro = args[i].Split(separadores, StringSplitOptions.RemoveEmptyEntries);
                            string key = filtro[0];
                            key = key.Replace("#", "");
                            if (key != "" && filtro.Length > 1)
                            {
                                //Facetas y resultados para IE8, 9, 10, si quitas el + y después haces un decode, te lo vueve a poner =(
                                filtro[1] = filtro[1].Replace("+", "%2B");
                                string valor = HttpUtility.UrlDecode(filtro[1]);
                                if (valor.Equals(GETIDENTIDADID))
                                {
                                    valor = $"gnoss:{pIdentidadID.ToString().ToUpper()}";
                                }
                                if (valor.Equals(GETUSUARIOID))
                                {
                                    UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                                    Guid usuarioID = usuarioCN.ObtenerGuidUsuarioIDporIdentidadID(pIdentidadID);
                                    valor = $"gnoss:{usuarioID.ToString().ToUpper()}";
                                }
                                if (filtro.Length > 2)
                                {
                                    // Hay más datos en el filtro separados por = que pertenecen al valor
                                    for (int j = 2; j < filtro.Length; j++)
                                    {
                                        valor += "=" + filtro[j];
                                    }
                                }

                                switch (key)
                                {
                                    case "orden":
                                        if (valor == "asc")
                                        {
                                            pOrdenDescendente = false;
                                        }
                                        else if (valor == "desc")
                                        {
                                            pOrdenDescendente = true;
                                        }
                                        break;
                                    case "pagina":
                                        int paginaTemp = 1;
                                        if (int.TryParse(valor, out paginaTemp))
                                        {
                                            pPaginaActual = paginaTemp;
                                        }
                                        break;
                                    case "ordenarPor":
                                        if (!tieneFiltroOrden)
                                        {
                                            pFiltroOrden = valor;
                                            if (valor != GNOSS_RELEVANCIA)
                                            {
                                                tieneFiltroOrden = true;
                                            }
                                        }
                                        break;
                                    default:
                                        AgregarKeyValorAListaFiltros(pFacetaDW, pProyectoID, pListaFiltros, pListaFiltrosUsuario, key, valor);
                                        bool priorizarOrdenResultados = FacetaAD.PriorizarFacetasEnOrden(pFacetaDW, pListaFiltros);

                                        if (key.Equals("search") || (pFiltrosSearchPersonalizados != null && pFiltrosSearchPersonalizados.ContainsKey(key)) || priorizarOrdenResultados)
                                        {
                                            filtrarPorRelevancia = true;
                                        }
                                        break;
                                }
                            }
                            else if (key != "" && filtro.Length == 1)
                            {
                                if (key == "recibidos")
                                {
                                    AgregarKeyValorAListaFiltros(pFacetaDW, pProyectoID, pListaFiltros, pListaFiltrosUsuario, "dce:type", "Entrada");
                                }
                                else if (key == "enviados")
                                {
                                    AgregarKeyValorAListaFiltros(pFacetaDW, pProyectoID, pListaFiltros, pListaFiltrosUsuario, "dce:type", "Enviados");
                                }
                                else if (key == "eliminados")
                                {
                                    AgregarKeyValorAListaFiltros(pFacetaDW, pProyectoID, pListaFiltros, pListaFiltrosUsuario, "dce:type", "Eliminados");
                                }
                            }
                        }
                    }
                }
            }

            //Orden
            if ((pListaItemsBusqueda.Contains(FacetadoAD.BUSQUEDA_PERSONA) || pListaItemsBusqueda.Contains(FacetadoAD.BUSQUEDA_ORGANIZACION)) && !tieneFiltroOrden && !filtrarPorRelevancia && !(pListaFiltros.ContainsKey("rdf:type") && pListaFiltros.Count == 1))
            {
                pFiltroOrden = "foaf:familyName";
            }
            else if (filtrarPorRelevancia && !tieneFiltroOrden)
            {
                pFiltroOrden = GNOSS_RELEVANCIA;
            }
        }

        /// <summary>
        /// Agregar el key y valor a las listas de filtros.
        /// </summary>
        /// <param name="pListaFiltros">Lista filtros</param>
        /// <param name="pListaFiltrosUsuario">Lista filtros de usuarios</param>
        /// <param name="pKey">Key</param>
        /// <param name="pValor">Valor</param>
        private static void AgregarKeyValorAListaFiltros(DataWrapperFacetas pFacetaDW, Guid pProyectoID, Dictionary<string, List<string>> pListaFiltros, Dictionary<string, List<string>> pListaFiltrosUsuario, string pKey, string pValor)
        {
            if (pKey.StartsWith(FacetadoAD.PARTE_FILTRO_RECIPROCO))
            {
                // Se trata de un filtro reciproco, quitar esa parte del filtro y poner la de la faceta correspondiente
                if (pFacetaDW != null)
                {
                    List<FacetaObjetoConocimientoProyecto> filas = (List<FacetaObjetoConocimientoProyecto>)pFacetaDW.ListaFacetaObjetoConocimientoProyecto.Where(item => item.ProyectoID.Equals(pProyectoID) && item.Reciproca > 0 && item.Faceta.StartsWith("{"));
                    if (filas.Count > 0)
                    {
                        FacetaObjetoConocimientoProyecto fila = filas.First();
                        pKey = fila.Faceta;
                    }
                }
            }

            if (!pListaFiltros.ContainsKey(pKey))
            {
                pListaFiltros.Add(pKey, new List<string>());
            }

            if (pValor.StartsWith("GETDATE(") && (pValor.EndsWith(")") || (pValor.EndsWith(")-"))))
            {
                string date = DateTime.Now.ToString("yyyyMMdd");
                string diferencia = pValor.Replace("GETDATE(", "").Replace(")-", "").Replace(")", "");
                if (diferencia.Length > 0)
                {
                    int dias = 0;
                    if (int.TryParse(diferencia, out dias))
                    {
                        date = DateTime.Now.AddDays(dias).ToString("yyyyMMdd");
                    }
                }

                pListaFiltros[pKey].Add(date + "-");
            }
            else if (pValor.StartsWith("-GETDATE(") && pValor.EndsWith(")"))
            {
                string date = DateTime.Now.ToString("yyyyMMdd");
                string diferencia = pValor.Replace("-GETDATE(", "").Replace(")", "");

                if (diferencia.Length > 0)
                {
                    int dias = 0;
                    if (int.TryParse(diferencia, out dias))
                    {
                        date = DateTime.Now.AddDays(dias).ToString("yyyyMMdd");
                    }
                }

                pListaFiltros[pKey].Add("-" + date);
            }
            else if (!pListaFiltros[pKey].Contains(pValor))
            {
                pListaFiltros[pKey].Add(pValor);
            }

            if (!pListaFiltrosUsuario.ContainsKey(pKey))
            {
                pListaFiltrosUsuario.Add(pKey, new List<string>());
            }

            if (!pListaFiltrosUsuario[pKey].Contains(pValor) && pValor != "GETDATE()" && pValor != "GETDATE()-")
            {
                pListaFiltrosUsuario[pKey].Add(pValor);
            }
        }

        public static bool ChequearUsuarioTieneRecursosPrivados(bool pFacetaPrivadaGrupo, Guid pPerfilIdentidadID, TipoBusqueda pTipoBusqueda, Guid pProyectoID, FacetadoCL pFacetadoCL)
        {
            bool tienePrivados = false;

            if (!pPerfilIdentidadID.Equals(Guid.Empty))
            {
                if (pTipoBusqueda.Equals(FacetadoAD.TipoBusquedaToString(TipoBusqueda.Mensajes)))
                {
                    tienePrivados = true;
                }
                else
                {
                    //Quitado porque es mejor que virtuoso muestre todos los recursos, que atacar a la BD para detectar si tiene recursos privados este perfil.
                    /*if (pFacetaPrivadaGrupo || pFacetadoCL.TienePrivados(pProyectoID, pPerfilIdentidadID))*/
                    {
                        tienePrivados = true;
                    }
                }
            }

            return tienePrivados;
        }

        #region Métodos buscador facetado

        /// <summary>
        /// Obtiene la lista de items extra que se obtendrá de la búsqueda y su prefijo (recetas, peliculas, etc)
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que está buscando el usuario</param>
        /// <param name="pFacetaDS">Dataset de facetas</param>
        public static Dictionary<string, List<string>> ObtenerInformacionOntologiasSinArroba(string pUrlIntragnoss, Dictionary<string, List<string>> pInformacionOntologias)
        {
            Dictionary<string, List<string>> dicNombreAbreviatura = new Dictionary<string, List<string>>();
            if (pInformacionOntologias != null)
            {
                foreach (string clave in pInformacionOntologias.Keys)
                {
                    string claveFormateada = clave;
                    if (!clave.Contains("@"))
                    {
                        claveFormateada = pUrlIntragnoss + "Ontologia/" + clave + ".owl#";
                    }
                    else
                    {
                        claveFormateada = clave.Replace("@", "");
                    }
                    if (!dicNombreAbreviatura.ContainsKey(claveFormateada))
                    {
                        dicNombreAbreviatura.Add(claveFormateada, pInformacionOntologias[clave]);
                    }
                }
            }

            return dicNombreAbreviatura;
        }

        /// <summary>
        /// Obtiene la lista de items extra que se obtendrá de la búsqueda y su prefijo (recetas, peliculas, etc)
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que está buscando el usuario</param>
        /// <param name="pFacetaDS">DataWrapper de facetas</param>
        public Dictionary<string, List<string>> ObtenerInformacionOntologias(Guid pOrganizacionID, Guid pProyectoID)
        {
            DataWrapperFacetas facetaDW = new DataWrapperFacetas();
            Dictionary<string, List<string>> resultado = ObtenerInformacionOntologias(pOrganizacionID, pProyectoID, facetaDW);

            return resultado;
        }

        /// <summary>
        /// Obtiene la lista de items extra que se obtendrá de la búsqueda y su prefijo (recetas, peliculas, etc)
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que está buscando el usuario</param>
        /// <param name="pFacetaDW">DataWrapper de facetas</param>
        public Dictionary<string, List<string>> ObtenerInformacionOntologias(Guid pOrganizacionID, Guid pProyectoID, DataWrapperFacetas pFacetaDW)
        {
            FacetaCL facetaCL = new FacetaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetaCL>(), mLoggerFactory);

            List<OntologiaProyecto> listaOntologias = facetaCL.ObtenerOntologiasProyecto(pOrganizacionID, pProyectoID);
            pFacetaDW.ListaOntologiaProyecto = pFacetaDW.ListaOntologiaProyecto.Union(listaOntologias).ToList();

            return FacetadoAD.ObtenerInformacionOntologias(listaOntologias);
        }


        /// <summary>
        /// Obtiene la lista con las propiedades de tipo rango
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto en el que está buscando el usuario</param>
        public List<string> ObtenerPropiedadesRango(GestionFacetas pGestorFacetas)
        {
            List<string> propiedadesRango = new List<string>();

            FacetaCL facetaCL = new FacetaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetaCL>(), mLoggerFactory);
            List<Faceta> lista = pGestorFacetas.ListaFacetas.Where(faceta => faceta.TipoPropiedad.Equals(TipoPropiedadFaceta.Numero)).ToList();

            foreach (Faceta fac in lista)
            {
                propiedadesRango.Add(fac.ClaveFaceta.Substring(fac.ClaveFaceta.LastIndexOf(":") + 1));
            }

            //TablasDeConfiguracionCL tablasDeConfiguracionCL = new TablasDeConfiguracionCL();            
            //ConfiguracionFacetadoDS tConfiguracion = tablasDeConfiguracionCL.ObtenerConfiguracionOrdenFacetado();

            //DataRow[] filas = tConfiguracion.ConfiguracionOrdenFacetado.Select("IDComunidad = '" + pProyectoID + "'");

            //foreach (DataRow myrow in filas)
            //{

            //    if (myrow["TipoPropiedad"].ToString().Equals("Numero"))
            //    {
            //        string numeroAux = myrow["Faceta"].ToString();
            //        propiedadesRango.Add(numeroAux.Substring(numeroAux.LastIndexOf(":") + 1));
            //    }

            //}                               

            return propiedadesRango;
        }



        /// <summary>
        /// Obtiene la lista con las propiedades de tipo rango
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto en el que está buscando el usuario</param>
        public List<string> ObtenerPropiedadesFecha(GestionFacetas pGestorFacetas)
        {
            List<string> propiedadesFecha = new List<string>();

            FacetaCL facetaCL = new FacetaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetaCL>(), mLoggerFactory);
            List<Faceta> lista = pGestorFacetas.ListaFacetas.Where(faceta => faceta.TipoPropiedad.Equals(TipoPropiedadFaceta.Fecha) || faceta.TipoPropiedad.Equals(TipoPropiedadFaceta.Calendario) || faceta.TipoPropiedad.Equals(TipoPropiedadFaceta.CalendarioConRangos)).ToList();

            foreach (Faceta fac in lista)
            {
                propiedadesFecha.Add(fac.ClaveFaceta.Substring(fac.ClaveFaceta.LastIndexOf(":") + 1));
            }

            return propiedadesFecha;
        }

        /// <summary>
        /// Obtiene la lista de las comunidades privadas de un usuario
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad con la que está conectada el usuario</param>
        /// <param name="pEsUsuarioInvitado">Verdad si es el usuario invitado</param>
        public List<Guid> ObtenerListaComunidadesPrivadasUsuario(Guid pIdentidadID, bool pEsUsuarioInvitado)
        {
            List<Guid> listaComunidadesPrivadasUsuario = null;

            if (pEsUsuarioInvitado)
            {
                listaComunidadesPrivadasUsuario = new List<Guid>();
            }
            else
            {
                IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                listaComunidadesPrivadasUsuario = identCN.ObtenerComunidadesPrivadas(pIdentidadID);
            }

            return listaComunidadesPrivadasUsuario;
        }

        /// <summary>
        /// Obtiene la lista de items extra que se obtendrá de la búsqueda (recetas, peliculas, etc)
        /// </summary>
        /// <param name="pLaguageCode">LaguageCode del idioma actual del usuario (es, en...)</param>
        /// <param name="pListaFiltrosFacetasUsuario">Lista de filtros que ha realizado el usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que está buscando el usuario</param>
        /// <param name="pTipoBusqueda">Tipo de búsqueda</param>
        public List<string> ObtenerListaItemsBusquedaExtra(Dictionary<string, List<string>> pListaFiltrosFacetasUsuario, TipoBusqueda pTipoBusqueda, Guid pOrganizacionID, Guid pProyectoID)
        {
            return ObtenerListaItemsBusquedaExtra(pListaFiltrosFacetasUsuario, pTipoBusqueda, pOrganizacionID, pProyectoID, null);
        }


        /// Obtiene la lista de items extra que se obtendrá de la búsqueda (recetas, peliculas, etc)
        /// </summary>
        /// <param name="pLaguageCode">LaguageCode del idioma actual del usuario (es, en...)</param>
        /// <param name="pListaFiltrosFacetasUsuario">Lista de filtros que ha realizado el usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que está buscando el usuario</param>
        /// <param name="pTipoBusqueda">Tipo de búsqueda</param>
        public Dictionary<string, Tuple<string, string, string, bool>> ObtenerListaFiltrosSearchPersonalizados(Guid pProyecto)
        {
            Elementos.ServiciosGenerales.Proyecto proyecto = null;
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
            GestionProyecto gestorProy = new GestionProyecto(proyCL.ObtenerProyectoPorID(pProyecto), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionProyecto>(), mLoggerFactory);
            proyecto = gestorProy.ListaProyectos[pProyecto];

            Dictionary<string, Tuple<string, string, string, bool>> filtrosSearchPersonalizados = new Dictionary<string, Tuple<string, string, string, bool>>();

            if (proyecto != null && proyecto.GestorProyectos.DataWrapperProyectos.ListaProyectoSearchPersonalizado != null)
            {
                foreach (ProyectoSearchPersonalizado fila in proyecto.GestorProyectos.DataWrapperProyectos.ListaProyectoSearchPersonalizado)
                {

                    filtrosSearchPersonalizados.Add(fila.NombreFiltro, new Tuple<string, string, string, bool>(fila.WhereSPARQL.Replace("[PARAMETROIDIOMAUSUARIO]", "es"), fila.OrderBySPARQL.Replace("[PARAMETROIDIOMAUSUARIO]", "es"), fila.WhereFacetasSPARQL.Replace("[PARAMETROIDIOMAUSUARIO]", "es"), fila.OmitirRdfType));
                }
            }

            return filtrosSearchPersonalizados;

        }



        /// <summary>
        /// Obtiene la lista de items extra que se obtendrá de la búsqueda (recetas, peliculas, etc)
        /// </summary>
        /// <param name="pLaguageCode">LaguageCode del idioma actual del usuario (es, en...)</param>
        /// <param name="pListaFiltrosFacetasUsuario">Lista de filtros que ha realizado el usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que está buscando el usuario</param>
        /// <param name="pTipoBusqueda">Tipo de búsqueda</param>
        public List<string> ObtenerListaItemsBusquedaExtra(Dictionary<string, List<string>> pListaFiltrosFacetasUsuario, TipoBusqueda pTipoBusqueda, Guid pOrganizacionID, Guid pProyectoID, string pFicheroConfiguracionBD)
        {
            List<string> listaItemsExtra = new List<string>();

            if ((!pListaFiltrosFacetasUsuario.ContainsKey("rdf:type")) && (pTipoBusqueda.Equals(TipoBusqueda.Recursos) || pTipoBusqueda.Equals(TipoBusqueda.BusquedaAvanzada)))
            {
                FacetaCL facetaCL = null;

                if (!string.IsNullOrEmpty(pFicheroConfiguracionBD))
                {
                    facetaCL = new FacetaCL(pFicheroConfiguracionBD, pFicheroConfiguracionBD, null, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetaCL>(), mLoggerFactory);
                }
                else
                {
                    facetaCL = new FacetaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetaCL>(), mLoggerFactory);
                }

                //ObtenerOntologias
                DataWrapperFacetas facetaDW = new DataWrapperFacetas();
                List<OntologiaProyecto> listaOntologias = facetaCL.ObtenerOntologiasProyecto(pOrganizacionID, pProyectoID);
                facetaDW.ListaOntologiaProyecto = listaOntologias;
                foreach (OntologiaProyecto myrow in facetaDW.ListaOntologiaProyecto)
                {
                    if (myrow.EsBuscable)
                    {
                        listaItemsExtra.Add(myrow.OntologiaProyecto1);
                    }
                }
            }

            return listaItemsExtra;
        }

        /// <summary>
        /// Obtiene los predicados semánticos de un proyecto concreto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto en el que está buscando el usuario</param>
        /// <param name="pTipoBusqueda">Tipo de búsqueda</param>
        /// <returns></returns>
        public List<string> ObtenerFormulariosSemanticos(TipoBusqueda pTipoBusqueda, Guid pOrganizacionID, Guid pProyectoID)
        {
            return ObtenerFormulariosSemanticos(pTipoBusqueda, pOrganizacionID, pProyectoID, null);
        }

        /// <summary>
        /// Obtiene los predicados semánticos de un proyecto concreto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto en el que está buscando el usuario</param>
        /// <param name="pTipoBusqueda">Tipo de búsqueda</param>
        /// <returns></returns>
        public List<string> ObtenerFormulariosSemanticos(TipoBusqueda pTipoBusqueda, Guid pOrganizacionID, Guid pProyectoID, string pFicheroConfiguracionBD)
        {
            List<string> formulariosSemanticos = null;

            if (pTipoBusqueda.Equals(TipoBusqueda.BusquedaAvanzada) || pTipoBusqueda.Equals(TipoBusqueda.Recursos) || pTipoBusqueda.Equals(TipoBusqueda.Contribuciones) || pTipoBusqueda.Equals(TipoBusqueda.EditarRecursosPerfil))
            {
                FacetaCL facetaCL = null;

                if (!string.IsNullOrEmpty(pFicheroConfiguracionBD))
                {
                    facetaCL = new FacetaCL(pFicheroConfiguracionBD, pFicheroConfiguracionBD, null, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetaCL>(), mLoggerFactory);
                }
                else
                {
                    facetaCL = new FacetaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetaCL>(), mLoggerFactory);
                }

                formulariosSemanticos = facetaCL.ObtenerPredicadosSemanticos(pOrganizacionID, pProyectoID);
            }
            else
            {
                formulariosSemanticos = new List<string>();
            }

            return formulariosSemanticos;
        }

        /// <summary>
        /// Obtiene el filtro para la consulta de mapas.
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <param name="pOrganizacionID">ID de org</param>
        /// <param name="pTipoBusqueda">Tipo de búsqueda</param>
        /// <returns></returns>
        public DataWrapperFacetas ObtenerDataSetConsultaMapaProyecto(Guid pOrganizacionID, Guid pProyectoID, TipoBusqueda pTipoBusqueda)
        {
            FacetaCL facetaCL = new FacetaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetaCL>(), mLoggerFactory);
            DataWrapperFacetas facetaDS = facetaCL.ObtenerPropsMapaPerYOrgProyecto(pOrganizacionID, pProyectoID, pTipoBusqueda);
            facetaCL.Dispose();

            return facetaDS;
        }

        /// <summary>
        /// Obtiene el filtro para la consulta de chart.
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <param name="pChartID">ID de chart</param>
        /// <param name="pOrganizacionID">ID de organización</param>
        /// <param name="pIdioma">Idioma del usuario</param>
        /// <returns>filtro para la consulta de chart</returns>
        public KeyValuePair<string, string> ObtenerSelectYFiltroConsultaChartProyecto(Guid pOrganizacionID, Guid pProyectoID, Guid pChartID, string pIdioma, VirtuosoAD mVirtuosoAD)
        {
            FacetaCL facetaCL = new FacetaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetaCL>(), mLoggerFactory);
            DataWrapperFacetas chartDW = facetaCL.ObtenerDatosChartProyecto(pOrganizacionID, pProyectoID);
            facetaCL.Dispose();

            FacetaConfigProyChart facetaConfigProyChart = chartDW.ListaFacetaConfigProyChart.Where(item => item.ChartID.Equals(pChartID)).FirstOrDefault();


            ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
            DataWrapperProyecto dashboardDW = proyectoCL.ObtenerProyectoPorID(pProyectoID);
            proyectoCL.Dispose();

            ProyectoPestanyaDashboardAsistente proyectoPestanyaDashboardAsistente = dashboardDW.ListaProyectoPestanyaDashboardAsistente.FirstOrDefault(item => item.AsisID.Equals(pChartID));

            KeyValuePair<string, string> selectFilt = new KeyValuePair<string, string>();

            if (facetaConfigProyChart != null)
            {
                selectFilt = new KeyValuePair<string, string>(facetaConfigProyChart.SelectConsultaVirtuoso, facetaConfigProyChart.FiltrosConsultaVirtuoso.Replace("@lang@", pIdioma));
                return selectFilt;
            }

            if (proyectoPestanyaDashboardAsistente != null)
            {   
                selectFilt = new KeyValuePair<string, string>(proyectoPestanyaDashboardAsistente.Select, proyectoPestanyaDashboardAsistente.Where);
                return selectFilt;
            }

            return selectFilt;

        }

        #endregion
    }
}