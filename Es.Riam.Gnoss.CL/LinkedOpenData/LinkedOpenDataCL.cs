using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Elementos.LinkedOpenData;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.CL.LinkedOpenDataCL
{
    public class LinkedOpenDataCL : BaseCL
    {

        #region Constantes

        public const string PROPIEDAD_TITULO = "TITULO";
        public const string PROPIEDAD_DESCRIPCION = "DESCRIPCION";
        public const string PROPIEDAD_NOMBRE_ALTERNATIVO = "NOMBREALTERNATIVO";
        public const string PROPIEDAD_SAMEAS = "SAMEAS";
        public const string PROPIEDAD_URI = "URI";

        #endregion

        #region Miembros

        /// <summary>
        /// Clave MAESTRA de la caché
        /// </summary>
        private readonly string[] mMasterCacheKeyArray = { NombresCL.LINKEDOPENDATA };

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor para FacetadoCL
        /// </summary>
        public LinkedOpenDataCL(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
        }

        /// <summary>
        /// Constructor para FacetadoCL
        /// </summary>
        /// <param name="pPool">Fichero de configuración</param>
        public LinkedOpenDataCL(string pPool, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(null, pPool, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Recupera todos los parámetros de la aplicación (carga ligera)
        /// </summary>
        /// <returns>Países y provincias</returns>
        public Dictionary<string, string> ObtenerNombreUriDeEntidadesRelacionadasPorID(Guid pID)
        {
            Dictionary<string, string> listaResultados = new Dictionary<string, string>();
            string[] separadorPropiedad = { "##" };

            List<object> listaEntidades = ObtenerEntidadesRelacionadasPorID(pID);

            foreach (string resultado in listaEntidades)
            {
                string[] separador = { "&&&" };
                string[] contenidoEntidad = resultado.Split(separador, StringSplitOptions.RemoveEmptyEntries);
                string titulo = "";
                string uri = "";

                foreach (string propiedad in contenidoEntidad)
                {
                    string[] propiedadNombreValor = propiedad.Split(separadorPropiedad, StringSplitOptions.RemoveEmptyEntries);

                    string tipoPropiedad = propiedadNombreValor[0];
                    string valor = propiedadNombreValor[1];

                    switch (tipoPropiedad)
                    {
                        case LinkedOpenDataCL.PROPIEDAD_TITULO:
                            titulo = valor;
                            break;
                        case LinkedOpenDataCL.PROPIEDAD_URI:
                            uri = valor;
                            break;
                    }

                    listaResultados.Add(titulo, uri);
                }
            }

            return listaResultados;
        }

        /// <summary>
        /// Recupera todos los parámetros de la aplicación (carga ligera)
        /// </summary>
        /// <returns>Países y provincias</returns>
        public List<object> ObtenerEntidadesRelacionadasPorID(Guid pID)
        {
            string entidadesLod = ObtenerObjetoDeCache("Elemento_" + pID) as string;
            List<object> listaResultados = new List<object>();

            if (entidadesLod != null && entidadesLod.Length > 0)
            {
                string[] separador = { "&&&" };
                string[] entidadesSeparadas = entidadesLod.Split(separador, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < entidadesSeparadas.Length; i++)
                {
                    entidadesSeparadas[i] = ObtenerClaveCache("Entidad_" + entidadesSeparadas[i]);
                }

                listaResultados = ObtenerVariosObjetosDeCache(entidadesSeparadas);
            }

            return listaResultados;
        }

        public Dictionary<string, EntidadLOD> ObtenerListaResourcesDeListaResultados(Guid pID)
        {
            List<object> pListaEntidades = ObtenerEntidadesRelacionadasPorID(pID);
            Dictionary<string, EntidadLOD> resultados = new Dictionary<string, EntidadLOD>();

            if (pListaEntidades != null && pListaEntidades.Count > 0)
            {
                foreach (string resultado in pListaEntidades)
                {
                    EntidadLOD recurso = ObtenerEntidadLODDesdeString(resultado);

                    string titulo = recurso.Nombre.ToLower().Trim();
                    if (!resultados.ContainsKey(titulo))
                    {
                        resultados.Add(titulo, recurso);
                    }
                    else
                    {
                        if (titulo.EndsWith(")") && titulo.Contains(" ("))
                        {
                            // Quito la parte del término que va entre paréntesis. Ejemplo: cwm (software) 
                            // Y lo añado por duplicado, por si el tag viene de una manera (cwm) o de la otra (cwm (software))
                            titulo = titulo.Substring(0, titulo.LastIndexOf(" ("));
                            if (!resultados.ContainsKey(titulo))
                            {
                                resultados.Add(titulo, recurso);
                            }
                        }
                    }

                    foreach (string nombreAlternativo in recurso.NombresAlternativos)
                    {
                        if (!resultados.ContainsKey(nombreAlternativo))
                        {
                            resultados.Add(nombreAlternativo, recurso);
                        }
                    }
                }
            }

            return resultados;
        }

        public EntidadLOD ObtenerEntidadLODDesdeString(string pDatosCacheEntidadLod)
        {
            EntidadLOD recurso = new EntidadLOD();

            string[] separadorPropiedad = { "##" };
            string[] separador = { "&&&" };
            string[] contenidoEntidad = pDatosCacheEntidadLod.Split(separador, StringSplitOptions.RemoveEmptyEntries);

            foreach (string propiedad in contenidoEntidad)
            {
                string[] propiedadNombreValor = propiedad.Split(separadorPropiedad, StringSplitOptions.RemoveEmptyEntries);

                if (propiedadNombreValor.Length == 2)
                {
                    string tipoPropiedad = propiedadNombreValor[0];
                    string valor = propiedadNombreValor[1];

                    switch (tipoPropiedad)
                    {
                        case LinkedOpenDataCL.PROPIEDAD_TITULO:
                            recurso.Nombre = valor;
                            break;
                        case LinkedOpenDataCL.PROPIEDAD_DESCRIPCION:
                            recurso.Descripcion = valor;
                            break;
                        case LinkedOpenDataCL.PROPIEDAD_SAMEAS:
                            recurso.ListaEntidadesSameAs.Add(valor);
                            break;
                        case LinkedOpenDataCL.PROPIEDAD_URI:
                            recurso.Url = valor;
                            break;
                        case LinkedOpenDataCL.PROPIEDAD_NOMBRE_ALTERNATIVO:
                            recurso.NombresAlternativos.Add(valor);
                            break;
                    }
                }
            }

            return recurso;
        }

        public void AgregarEntidadesRelacionadasPorID(Guid pID, string pURIsDbPedia)
        {
            AgregarObjetoCache("Elemento_" + pID, pURIsDbPedia);
        }

        public List<string> ObtenerUrisEntidadesRelacionadasPorID(Guid pID)
        {
            string entidadesLod = ObtenerObjetoDeCache("Elemento_" + pID) as string;
            List<string> listaResultados = new List<string>();

            if (entidadesLod != null && entidadesLod.Length > 0)
            {
                string[] separador = { "&&&" };
                string[] entidadesSeparadas = entidadesLod.Split(separador, StringSplitOptions.RemoveEmptyEntries);
                listaResultados = new List<string>(entidadesSeparadas);
            }

            return listaResultados;
        }

        public void AgregarEntidadDbPediaACache(string pUriDbpedia, string pDatosEntidad, bool pSobrescribir)
        {
            string clave = "Entidad_" + pUriDbpedia;
            if (pSobrescribir || !ExisteClaveEnCache(clave))
            {
                AgregarObjetoCache(clave, pDatosEntidad);
            }
        }

        public string ObtenerEntidadDbPedia(string pUriDbpedia)
        {
            string clave = "Entidad_" + pUriDbpedia;
            if (ExisteClaveEnCache(clave))
            {
                return (string)ObtenerObjetoDeCache(clave, true);
            }

            return "";
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la clave para la caché
        /// </summary>
        public override string[] ClaveCache
        {
            get
            {
                return mMasterCacheKeyArray;
            }
        }

        /// <summary>
        /// No queremos que se pieran las cachés LOD, porque no se regeneran de manera automática
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
