using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Semantica.OWL;
using Es.Riam.Semantica.Plantillas;
using Es.Riam.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Metagnoss.ExportarImportar
{
    public class UtilSemCms
    {
        private VirtuosoAD mVirtuosoAD;
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;

        public UtilSemCms(EntityContext entityContext, LoggingService loggingService, ConfigService configService, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            mVirtuosoAD = virtuosoAD;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        #region Métodos

        /// <summary>
        /// Obtiene los sujetos de las entidades externas vinculadas por un selector de entidad recíproco a una entidad.
        /// </summary>
        /// <param name="pUrlIntragnoss">UrlIntragnoss</param>
        /// <param name="pSelectorEntidad">Selector de entidad recíproco</param>
        /// <param name="pFilaPersona">Fila de la persona conectada</param>
        /// <param name="pFilaProy">Fila del proyecto actual</param>
        /// <param name="pEntidades">Entidades principales desde las que se hace la consulta</param>
        public List<string> ObtenerSujetosEntiadesSelectorReciprocoDeEntidad(string pUrlIntragnoss, SelectorEntidad pSelectorEntidad, string pEntidadID, Gnoss.AD.EntityModel.Models.PersonaDS.Persona pFilaPersona, Gnoss.AD.EntityModel.Models.ProyectoDS.Proyecto pFilaProy, List<ElementoOntologia> pEntidades)
        {
            FacetadoCN facetadoCN = new FacetadoCN(pUrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            FacetadoDS facetadoDS;

            if (string.IsNullOrEmpty(pSelectorEntidad.ConsultaReciproca))
            {
                List<string> propsLectura = new List<string>();
                propsLectura.Add("http://www.w3.org/1999/02/22-rdf-syntax-ns#type");//Añadimos rdf:type que lo tienen todas las entidades.

                string orden = null;

                if (pSelectorEntidad.PropOrdenRecipocidad.Key != null)
                {
                    orden = "orderby|" + pSelectorEntidad.PropOrdenRecipocidad.Value + "|" + pSelectorEntidad.PropOrdenRecipocidad.Key;
                }

                string propFiltro = "ANY";

                if (!string.IsNullOrEmpty(pSelectorEntidad.PropiedadReciproca))
                {
                    propFiltro = pSelectorEntidad.PropiedadReciproca;
                }

                mLoggingService.AgregarEntrada("FormSem SelectorEntidad Reciproca para '" + pEntidadID + "' automatica con orden: " + orden);

                facetadoDS = facetadoCN.ObtenerRDFXMLSelectorEntidadFormulario(pSelectorEntidad.Grafo, null, propFiltro, pEntidadID, propsLectura, orden, null, null);
            }
            else
            {
                mLoggingService.AgregarEntrada("FormSem SelectorEntidad Reciproca para '" + pEntidadID + "' con consulta: " + pSelectorEntidad.ConsultaReciproca);
                facetadoDS = facetadoCN.ObtenerRDFXMLSelectorEntidadFormularioPorConsulta(pSelectorEntidad.Grafo, pEntidadID, ObtenerExtraWhereConInfoUsuario(pSelectorEntidad.ConsultaReciproca, pFilaPersona, pFilaProy, pEntidades));
            }

            facetadoCN.Dispose();

            List<string> sujetosReciprocos = new List<string>();

            foreach (DataRow fila in facetadoDS.Tables[0].Rows)
            {
                string sujetoReciproco = (string)fila[0];

                if (!sujetosReciprocos.Contains(sujetoReciproco))
                {
                    sujetosReciprocos.Add(sujetoReciproco);
                }
            }

            facetadoDS.Dispose();

            return sujetosReciprocos;
        }

        /// <summary>
        /// Obtiene el where extra para las consultas con la información del usuario ya sustituida.
        /// </summary>
        /// <param name="pExtraWhere">Extra where</param>
        /// <param name="pFilaPersona">Fila de la persona conectada</param>
        /// <param name="pFilaProy">Fila del proyecto actual</param>
        /// <param name="pEntidades">Entidades principales desde las que se hace la consulta</param>
        /// <returns>Where extra para las consultas con la información del usuario ya sustituida</returns>
        public static string ObtenerExtraWhereConInfoUsuario(string pExtraWhere, Gnoss.AD.EntityModel.Models.PersonaDS.Persona pFilaPersona, Gnoss.AD.EntityModel.Models.ProyectoDS.Proyecto pFilaProy, List<ElementoOntologia> pEntidades)
        {
            string extraWhere = pExtraWhere;

            while (extraWhere != null && extraWhere.Contains("[DataSet") && pFilaPersona != null && pFilaProy != null)
            {
                string trozo1 = extraWhere.Substring(0, extraWhere.IndexOf("[DataSet"));
                string consulta = extraWhere.Substring(extraWhere.IndexOf("[DataSet") + 1);
                string trozo2 = consulta.Substring(consulta.IndexOf("]") + 1);
                consulta = consulta.Substring(0, consulta.IndexOf("]"));
                string[] paramConsulta = consulta.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                string reemplazo = "";
                if (paramConsulta[1].ToLower() == "identidadds" && paramConsulta[2].ToLower() == "persona")
                {
                    object value = UtilReflection.GetValueReflection(pFilaPersona, paramConsulta[3]);
                    if (value is Guid)
                    {
                        reemplazo = value.ToString().ToUpper();
                    }
                    else
                    {
                        reemplazo = value.ToString();
                    }
                }
                else if (paramConsulta[1].ToLower() == "proyectods" && paramConsulta[2].ToLower() == "proyecto")
                {
                    object value = UtilReflection.GetValueReflection(pFilaProy, paramConsulta[3]);

                    if (value is Guid)
                    {
                        reemplazo = value.ToString().ToUpper();
                    }
                    else
                    {
                        reemplazo = value.ToString();
                    }
                }

                extraWhere = trozo1 + reemplazo + trozo2;
            }

            while (extraWhere != null && extraWhere.Contains("[Propiedad") && pEntidades != null)
            {
                string trozo1 = extraWhere.Substring(0, extraWhere.IndexOf("[Propiedad"));
                string consulta = extraWhere.Substring(extraWhere.IndexOf("[Propiedad") + 1);
                string trozo2 = consulta.Substring(consulta.IndexOf("]") + 1);
                consulta = consulta.Substring(0, consulta.IndexOf("]"));
                string[] paramConsulta = consulta.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                string reemplazo = "";
                Propiedad propiedad = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(paramConsulta[1], paramConsulta[2], pEntidades);
                string rempInt = "@PropEntProp@";
                extraWhere = trozo1 + rempInt + trozo2;
                extraWhere = FacetadoAD.AjustarParametroEntidadIDConsultaSelectoresEnt(extraWhere, rempInt);

                if (propiedad != null && propiedad.ValoresUnificados.Count == 1)
                {
                    if (propiedad.PrimerValorPropiedad.StartsWith("http://"))
                    {
                        reemplazo = "= <" + propiedad.PrimerValorPropiedad + ">";
                    }
                    else
                    {
                        reemplazo = "= '" + propiedad.PrimerValorPropiedad + "'";
                    }
                }
                else if (propiedad != null && propiedad.ValoresUnificados.Count > 1)
                {
                    reemplazo = " IN(";

                    foreach (string valor in propiedad.ValoresUnificados.Keys)
                    {
                        if (valor.StartsWith("http://"))
                        {
                            reemplazo = string.Concat(reemplazo, "<", valor, ">,");
                        }
                        else
                        {
                            reemplazo = string.Concat(reemplazo, "'", propiedad.PrimerValorPropiedad, "',");
                        }
                    }

                    reemplazo = string.Concat(reemplazo.Substring(0, reemplazo.Length - 1), ")");
                }
                else
                {
                    reemplazo = "= <http://sin-valor>";//Para que no casque la consulta
                }

                extraWhere = extraWhere.Replace(rempInt, reemplazo);
            }

            return extraWhere;
        }

        /// <summary>
        /// Obtiene los datos de las entidades externas al documento.
        /// </summary>
        /// <param name="pDatosEntidadesExternas">Lista para almacenar los datos</param>
        /// <param name="pUrlIntragnoss">Url de Intragnoss</param>
        /// <param name="pFilaPersona">Fila de la persona conectada</param>
        /// <param name="pFilaProy">Fila del proyecto actual</param>
        /// <param name="pEntidades">Entidades principales desde las que se hace la consulta</param>
        public void ObtenerDatosEntidadesExternas(Dictionary<KeyValuePair<string, string>, object[]> pDatosEntidadesExternas, string pUrlIntragnoss, Gnoss.AD.EntityModel.Models.PersonaDS.Persona pFilaPersona, Gnoss.AD.EntityModel.Models.ProyectoDS.Proyecto pFilaProy, List<ElementoOntologia> pEntidades, bool pUsarAfinidad = false)
        {
            //Consulto a virtuoso o el modelo acido los datos:
            foreach (KeyValuePair<string, string> claveProp in pDatosEntidadesExternas.Keys)
            {
                List<string> listaPropiedades = (List<string>)pDatosEntidadesExternas[claveProp][0];
                List<string> listaEntidadesBusqueda = (List<string>)pDatosEntidadesExternas[claveProp][1];
                SelectorEntidad selectorEntidad = (SelectorEntidad)pDatosEntidadesExternas[claveProp][2];

                if (listaEntidadesBusqueda.Count == 0)
                {
                    continue;
                }

                if (selectorEntidad.TipoSeleccion == "UrlRecurso") //Modelo Acido
                {
                    List<Guid> listaRec = new List<Guid>();

                    foreach (string url in listaEntidadesBusqueda)
                    {
                        string urlRec = url.Trim();
                        if (urlRec.LastIndexOf("/") == urlRec.Length - 1)
                        {
                            urlRec = urlRec.Substring(0, urlRec.Length - 1);
                        }

                        try
                        {
                            listaRec.Add(new Guid(urlRec.Substring(urlRec.LastIndexOf("/") + 1)));
                        }
                        catch (Exception)
                        {
                            mLoggingService.GuardarLog("Error al añadir a la lista de Recursos, el recurso: " + url);
                        }
                    }

                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    DataWrapperDocumentacion dataWrapperDocumentacion = docCN.ObtenerDocumentosPorID(new List<Guid>(listaRec), false);
                    docCN.Dispose();

                    mLoggingService.AgregarEntrada("FormSem Datos entidades externas tipo 'UrlRecurso' traidos: " + dataWrapperDocumentacion.ListaDocumento.Count);

                    pDatosEntidadesExternas[claveProp][3] = dataWrapperDocumentacion;
                }
                else if (selectorEntidad.TipoSeleccion == "PersonaGnoss")
                {
                    pDatosEntidadesExternas[claveProp][3] = ObtenerDatosSelectorPersonasGnoss(listaEntidadesBusqueda);
                }
                else if (selectorEntidad.TipoSeleccion == "GruposGnoss")
                {
                    pDatosEntidadesExternas[claveProp][3] = ObtenerDatosSelectorGruposGnoss(listaEntidadesBusqueda);
                }
                else //Virtuoso
                {
                    if (listaEntidadesBusqueda.Count > 2000)
                    {
                        FacetadoDS facetadoDS = new FacetadoDS();
                        while (listaEntidadesBusqueda.Count > 0)
                        {
                            int numeroFin = 2000;
                            if (listaEntidadesBusqueda.Count < 2000)
                            {
                                numeroFin = listaEntidadesBusqueda.Count;
                            }
                            facetadoDS.Merge(ObtenerDatosSelectorEntidadesExternas(pUrlIntragnoss, selectorEntidad, listaEntidadesBusqueda.GetRange(0, numeroFin), listaPropiedades, pFilaPersona, pFilaProy, pEntidades, pUsarAfinidad));
                            listaEntidadesBusqueda.RemoveRange(0, numeroFin);
                        }
                        pDatosEntidadesExternas[claveProp][3] = facetadoDS;
                    }
                    else
                    {
                        FacetadoDS facetadoDS = ObtenerDatosSelectorEntidadesExternas(pUrlIntragnoss, selectorEntidad, listaEntidadesBusqueda, listaPropiedades, pFilaPersona, pFilaProy, pEntidades, pUsarAfinidad);
                        pDatosEntidadesExternas[claveProp][3] = facetadoDS;
                    }
                }
            }
        }



        /// <summary>
        /// Obtiene los nombres de los usuarios del selector de entidad.
        /// </summary>
        /// <param name="pEntUsuarios">Lista con las URIs de los usuarios</param>
        /// <returns>Lista con el ID del grupo y su nombre</returns>
        private Dictionary<Guid, string> ObtenerDatosSelectorPersonasGnoss(List<string> pEntUsuarios)
        {
            List<Guid> listaUsuarios = new List<Guid>();

            foreach (string valor in pEntUsuarios)
            {
                Guid usuarioID = UtilSemCms.ObtenerIDGnoss(valor);

                if (usuarioID != Guid.Empty && !listaUsuarios.Contains(usuarioID))
                {
                    listaUsuarios.Add(usuarioID);
                }
            }

            PersonaCN perCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Dictionary<Guid, string> nombresUsus = perCN.ObtenerNombresPersonasDeUsuariosID(listaUsuarios);
            perCN.Dispose();

            return nombresUsus;
        }

        /// <summary>
        /// Obtiene los nombres de los grupos del selector de entidad.
        /// </summary>
        /// <param name="pEntGrupos">Lista con las URIs de los grupos</param>
        /// <returns>Lista con el ID del grupo y su nombre</returns>
        private Dictionary<Guid, string> ObtenerDatosSelectorGruposGnoss(List<string> pEntGrupos)
        {
            List<Guid> listaGrupos = new List<Guid>();

            foreach (string valor in pEntGrupos)
            {
                Guid grupoID = UtilSemCms.ObtenerIDGnoss(valor);

                if (grupoID != Guid.Empty && !listaGrupos.Contains(grupoID))
                {
                    listaGrupos.Add(grupoID);
                }
            }

            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Dictionary<Guid, string> nombresGrupos = identCN.ObtenerNombresDeGrupos(listaGrupos);
            identCN.Dispose();

            return nombresGrupos;
        }

        /// <summary>
        /// Obtiene los datos de las entidades externas al documento de virtuoso.
        /// </summary>
        /// <param name="pUrlIntragnoss">Url de intragnoss</param>
        /// <param name="pSelectorEntidad">Selector de entidad</param>
        /// <param name="pEntidades">IDs de las entidades sobre las que se busca</param>
        /// <param name="pPropiedades">Nombres de propiedades buscadas</param>
        /// <param name="pFilaPersona">Fila de la persona conectada</param>
        /// <param name="pFilaProy">Fila del proyecto actual</param>
        /// <param name="pEntidadesPrinc">Entidades principales desde las que se hace la consulta</param>
        /// <returns>DataSet FacetadoDS con lod datos de las entidades externas al documento de virtuoso</returns>
        public FacetadoDS ObtenerDatosSelectorEntidadesExternas(string pUrlIntragnoss, SelectorEntidad pSelectorEntidad, List<string> pEntidades, List<string> pPropiedades, Gnoss.AD.EntityModel.Models.PersonaDS.Persona pFilaPersona, Gnoss.AD.EntityModel.Models.ProyectoDS.Proyecto pFilaProy, List<ElementoOntologia> pEntidadesPrinc, bool pUsarAfinidad = false)
        {
            FacetadoDS facetadoDS = null;
            FacetadoCN facetadoCN = new FacetadoCN(pUrlIntragnoss, true, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            string claveCache = null;

            if (pSelectorEntidad.Cache)
            {
                GnossCacheCL gnossCache = new GnossCacheCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                claveCache = $"{pSelectorEntidad.Grafo}-{pSelectorEntidad.UrlPropiedad}-{pSelectorEntidad.UrlTipoEntSolicitada}-{string.Join(",", pEntidades)}";
                facetadoDS = gnossCache.ObtenerObjetoDeCacheLocal(claveCache) as FacetadoDS;
            }

            if (facetadoDS == null)
            {
                if (!string.IsNullOrEmpty(pSelectorEntidad.Consulta))
                {
                    mLoggingService.AgregarEntrada("FormSem Obtener entidades externas Virtu con consulta: " + pSelectorEntidad.Consulta);
                    facetadoDS = facetadoCN.ObtenerValoresPropiedadesEntidadesPorConsulta(pSelectorEntidad.Grafo, pEntidades, ObtenerExtraWhereConInfoUsuario(pSelectorEntidad.Consulta, pFilaPersona, pFilaProy, pEntidadesPrinc));
                }
                else if (!pSelectorEntidad.AnidamientoGnoss)
                {
                    mLoggingService.AgregarEntrada("FormSem Obtener entidades externas Virtu sin anidamientoGnoss");
                    facetadoDS = facetadoCN.ObtenerValoresPropiedadesEntidades(pSelectorEntidad.Grafo, pEntidades, pPropiedades, (pSelectorEntidad.TipoSeleccion == "Tesauro" || pSelectorEntidad.MultiIdioma), pUsarAfinidad);
                }
                else
                {
                    mLoggingService.AgregarEntrada("FormSem Obtener entidades externas Virtu con anidamientoGnoss");
                    facetadoDS = facetadoCN.ObtenerValoresPropiedadesEntidadesAnidadas(pSelectorEntidad.Grafo, pEntidades, pPropiedades, pUsarAfinidad);
                }

                mLoggingService.AgregarEntrada("FormSem Terminado obtener entidades externas Virtu");

                // Entides externas dentro de las entidades externas

                foreach (EstiloPlantillaEspecifProp estiloProp in SelectorEntidad.PropiedadesLecturaYAuxiliares(pSelectorEntidad.PropiedadesLectura))
                {
                    if (estiloProp.SelectorEntidad != null)
                    {
                        mLoggingService.AgregarEntrada("FormSem Hay Entidades externas dentro de las externas en la propiedad '" + estiloProp.NombreRealPropiedad + "'");

                        // Reciproca

                        if (estiloProp.SelectorEntidad.Reciproca) //Busco entidades reciprocas
                        {
                            List<string> sujetos = new List<string>();

                            foreach (DataRow fila in facetadoDS.Tables[0].Rows)
                            {
                                if (!sujetos.Contains((string)fila[0]))
                                {
                                    sujetos.Add((string)fila[0]);
                                }
                            }

                            foreach (string sujeto in sujetos)
                            {
                                List<string> sujetosReciprocos = ObtenerSujetosEntiadesSelectorReciprocoDeEntidad(pUrlIntragnoss, estiloProp.SelectorEntidad, sujeto, pFilaPersona, pFilaProy, pEntidadesPrinc);

                                //Añadimos al dataSet los triples como si la sujeto actual tuvise la entidad relacionada. No tiene porque es selector reciproco:
                                foreach (string sujetoReciproco in sujetosReciprocos)
                                {
                                    facetadoDS.Tables["SelectPropEnt"].Rows.Add(sujeto, estiloProp.NombreRealPropiedad, sujetoReciproco);
                                }
                            }
                        }

                        List<string> listaPropiedadesHija = new List<string>();

                        foreach (EstiloPlantillaEspecifProp estiloPropHija in estiloProp.SelectorEntidad.PropiedadesLectura)
                        {
                            if (estiloPropHija.NombreRealPropiedad == null || (estiloPropHija.SelectorEntidad != null && estiloPropHija.SelectorEntidad.Reciproca && estiloPropHija.SelectorEntidad.TipoSeleccion != "Edicion"))
                            {
                                continue;
                            }

                            listaPropiedadesHija.Add(ObtenerNombreConJerarquiaProp(estiloPropHija));
                        }

                        List<string> listaEntidadesBusquedaHija = new List<string>();

                        foreach (DataRow fila in facetadoDS.Tables["SelectPropEnt"].Select("p='" + estiloProp.NombreRealPropiedad + "'"))
                        {
                            if (!fila.IsNull("o") && !listaEntidadesBusquedaHija.Contains((string)fila["o"]))
                            {
                                listaEntidadesBusquedaHija.Add((string)fila["o"]);
                            }
                        }

                        if (listaEntidadesBusquedaHija.Count > 0)
                        {
                            if (estiloProp.SelectorEntidad.TipoSeleccion == "PersonaGnoss")
                            {
                                Dictionary<Guid, string> idsNombres = ObtenerDatosSelectorPersonasGnoss(listaEntidadesBusquedaHija);
                                IncluirEnDataSetInfoSelectorPerYGruposGnoss(facetadoDS, idsNombres);
                            }
                            else if (estiloProp.SelectorEntidad.TipoSeleccion == "GruposGnoss")
                            {
                                Dictionary<Guid, string> idsNombres = ObtenerDatosSelectorGruposGnoss(listaEntidadesBusquedaHija);
                                IncluirEnDataSetInfoSelectorPerYGruposGnoss(facetadoDS, idsNombres);
                            }
                            else
                            {
                                FacetadoDS facetadoAuxDS = ObtenerDatosSelectorEntidadesExternas(pUrlIntragnoss, estiloProp.SelectorEntidad, listaEntidadesBusquedaHija, listaPropiedadesHija, pFilaPersona, pFilaProy, pEntidadesPrinc);
                                facetadoDS.Merge(facetadoAuxDS);
                            }
                        }

                        mLoggingService.AgregarEntrada("FormSem Terminado obtener entidades externas dentro de las externas Virtu");
                    }
                }

                if (pSelectorEntidad.Cache)
                {
                    GnossCacheCL gnossCache = new GnossCacheCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);

                    Guid proyectoID = ProyectoAD.MetaProyecto;
                    if (pFilaProy != null)
                    {
                        proyectoID = pFilaProy.ProyectoID;
                    }

                    gnossCache.AgregarObjetoCacheLocal(proyectoID, claveCache, facetadoDS, true);
                }
            }

            facetadoCN.Dispose();

            return facetadoDS;
        }

        /// <summary>
        /// Obtiene el nombre o nombre de las propiedades con jerarquia, si la tiene.
        /// </summary>
        /// <param name="pEstiloProp">Estilo de la propiedad o conjunto de propiedades</param>
        /// <param name="pNombrePadres">Cadena de propiedades padre para no perder los niveles si hay varias auxiliares en diferentes niveles</param>
        /// <returns>Nombre o nombre de las propiedades con jerarquia, si la tiene</returns>
        private static string ObtenerNombreConJerarquiaProp(EstiloPlantillaEspecifProp pEstiloProp, string pNombrePadres = "")
        {
            string nombre = pEstiloProp.NombreRealPropiedad;
            string nombreActual = pEstiloProp.NombreRealPropiedad;
            if (!string.IsNullOrEmpty(pNombrePadres))
            {
                nombreActual = $"{pNombrePadres}|{nombreActual}";
            }
            
            if (pEstiloProp.PropiedadesAuxiliares != null)
            {
                bool primerElemento = true;
                foreach (EstiloPlantillaEspecifProp estiloHijo in pEstiloProp.PropiedadesAuxiliares)
                {
                    string nombreHijo = $"{ObtenerNombreConJerarquiaProp(estiloHijo, nombreActual)},";

                    if (!primerElemento)
                    {
                        nombre += $"{nombreActual}|{nombreHijo}";
                    }
                    else
                    {
                        nombre += $"|{nombreHijo}";
                    }
                    primerElemento = false;
                }

                nombre = nombre.Substring(0, nombre.Length - 1);
            }

            return nombre;
        }

        /// <summary>
        /// Incluye en el dataSet en la tabla "SelectPerGrupoGnoss" con los ids y los nombre de los elementos pasado como parámetros.
        /// </summary>
        /// <param name="pFacetadoDS">DataSet de facetas</param>
        /// <param name="pIdsNombres">ids y los nombre de los elementos</param>
        private static void IncluirEnDataSetInfoSelectorPerYGruposGnoss(FacetadoDS pFacetadoDS, Dictionary<Guid, string> pIdsNombres)
        {
            if (!pFacetadoDS.Tables.Contains("SelectPerGrupoGnoss"))
            {
                DataTable table = new DataTable("SelectPerGrupoGnoss");
                table.Columns.Add("ID");
                table.Columns.Add("Nombre");
                pFacetadoDS.Tables.Add(table);
            }

            foreach (Guid id in pIdsNombres.Keys)
            {
                DataRow fila = pFacetadoDS.Tables["SelectPerGrupoGnoss"].NewRow();
                fila["ID"] = id.ToString();
                fila["Nombre"] = pIdsNombres[id];
                pFacetadoDS.Tables["SelectPerGrupoGnoss"].Rows.Add(fila);
            }
        }

        /// <summary>
        /// Extrae del dataSet en la tabla "SelectPerGrupoGnoss" los ids y los nombre de los elementos guardados previamente.
        /// </summary>
        /// <param name="pFacetadoDS">DataSet de facetas</param>
        /// <returns>ids y los nombre de los elementos</returns>
        public static Dictionary<Guid, string> ExtraerDeDataSetInfoSelectorPerYGruposGnoss(FacetadoDS pFacetadoDS)
        {
            Dictionary<Guid, string> lista = new Dictionary<Guid, string>();

            if (pFacetadoDS.Tables.Contains("SelectPerGrupoGnoss"))
            {
                foreach (DataRow fila in pFacetadoDS.Tables["SelectPerGrupoGnoss"].Rows)
                {
                    if (!lista.ContainsKey(new Guid((string)fila["ID"])))
                    {
                        lista.Add(new Guid((string)fila["ID"]), (string)fila["Nombre"]);
                    }
                }
            }

            return lista;
        }

        /// <summary>
        /// Devuelve el nombre de un perfil de Gnoss apartir de su perfilID.
        /// </summary>
        /// <param name="pPerfil"></param>
        /// <param name="pIdentidadDS"></param>
        /// <returns></returns>
        public static string NombrePerfilGnoss(string pPerfil, DataWrapperIdentidad pIdentidadDS)
        {
            Guid perfilID = ObtenerIDGnoss(pPerfil);
            foreach (Gnoss.AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil in pIdentidadDS.ListaPerfil.Where(perf => perf.PerfilID.Equals(perfilID)).ToList())
            {
                string nombrePerfil = filaPerfil.NombrePerfil;
                if (filaPerfil.OrganizacionID.HasValue && filaPerfil.PersonaID.HasValue)
                {
                    nombrePerfil = filaPerfil.NombrePerfil + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + filaPerfil.NombreOrganizacion;
                }
                else if (filaPerfil.OrganizacionID.HasValue)
                {
                    nombrePerfil = filaPerfil.NombreOrganizacion;
                }

                return nombrePerfil;
            }

            return "";
        }

        /// <summary>
        /// Obtiene el ID del perfil gnoss vinculada al form sem.
        /// </summary>
        /// <param name="pPerfil">Url del perfil</param>
        /// <returns>ID del perfil gnoss vinculada al form sem</returns>
        public static Guid ObtenerIDGnoss(string pPerfil)
        {
            Guid perfilID = Guid.Empty;
            Guid.TryParse(pPerfil.Substring(pPerfil.LastIndexOf("/") + 1), out perfilID);
            return perfilID;
        }

        /// <summary>
        /// Genera la ontología auxiliar para entidades externas.
        /// </summary>
        /// <param name="pPropiedad">Propiedad</param>
        /// <param name="pDatosEntidadesExternas">Datos de las entidades externas</param>
        /// <param name="pFacetadoDSAux">DataSet de facetado auxiliar</param>
        /// <param name="pIdiomaUsuario">Idioma de usuario</param>
        /// <param name="pFacetadoDS">DataSet de facetado con los datos cargados</param>
        /// <param name="pEstiloPropAux">Estilos auxiliares de la nueva ontología</param>
        /// <param name="pOntologia">Nueva ontología</param>
        /// <returns>Lista con las entidades a pintar</returns>
        public static List<string> GenerarOntologiaAuxiliarEntExternas(Propiedad pPropiedad, Dictionary<KeyValuePair<string, string>, object[]> pDatosEntidadesExternas, FacetadoDS pFacetadoDSAux, string pIdiomaUsuario, out FacetadoDS pFacetadoDS, out EstiloPlantillaEspecifProp pEstiloPropAux, out Ontologia pOntologia)
        {
            FacetadoDS facetadoDS = null;
            EstiloPlantillaEspecifProp estiloPropAux = null;
            Ontologia ontologiaAux = null;

            if (pPropiedad.UnicoValor.Key != null || pPropiedad.ListaValores.Count > 0)
            {
                #region Obtenemos datos virtuoso

                if (pFacetadoDSAux != null)
                {
                    facetadoDS = pFacetadoDSAux;
                }
                else if (pPropiedad.EspecifPropiedad.EsSelectorEntidadInterno && pDatosEntidadesExternas.ContainsKey(new KeyValuePair<string, string>(pPropiedad.Nombre + "_AuxEntExt", pPropiedad.ElementoOntologia.TipoEntidad + "_AuxEntExt")))
                {
                    facetadoDS = (FacetadoDS)pDatosEntidadesExternas[new KeyValuePair<string, string>(pPropiedad.Nombre + "_AuxEntExt", pPropiedad.ElementoOntologia.TipoEntidad + "_AuxEntExt")][3];
                }
                else if (pDatosEntidadesExternas.ContainsKey(new KeyValuePair<string, string>(pPropiedad.Nombre, pPropiedad.ElementoOntologia.TipoEntidad)))
                {
                    facetadoDS = (FacetadoDS)pDatosEntidadesExternas[new KeyValuePair<string, string>(pPropiedad.Nombre, pPropiedad.ElementoOntologia.TipoEntidad)][3];
                }
                else if (pDatosEntidadesExternas.ContainsKey(new KeyValuePair<string, string>(pPropiedad.Nombre, pPropiedad.EspecifPropiedad.NombreEntidad)))
                {
                    facetadoDS = (FacetadoDS)pDatosEntidadesExternas[new KeyValuePair<string, string>(pPropiedad.Nombre, pPropiedad.EspecifPropiedad.NombreEntidad)][3];
                }
                else
                {
                    facetadoDS = new FacetadoDS();
                }

                #region Invento ontología

                foreach (EstiloPlantillaEspecifProp estilo in pPropiedad.EspecifPropiedad.SelectorEntidad.PropiedadesLectura)
                {
                    estiloPropAux = estilo;

                    if (estiloPropAux.NombreRealPropiedad != null)
                    {
                        break;
                    }
                }

                string namespaceOnto = "gnossOntoAux";
                string urlOntologia = null;

                if (estiloPropAux.NombreEntidad.Contains("#"))
                {
                    urlOntologia = estiloPropAux.NombreEntidad.Substring(0, estiloPropAux.NombreEntidad.IndexOf("#") + 1);
                }
                else
                {
                    urlOntologia = estiloPropAux.NombreEntidad.Substring(0, estiloPropAux.NombreEntidad.LastIndexOf("/") + 1);
                }

                ontologiaAux = new Ontologia();
                ontologiaAux.OntoAuxiliarInventada = true;
                ontologiaAux.IdiomaUsuario = pIdiomaUsuario;
                ontologiaAux.GenararNamespacesHuerfanos = pPropiedad.Ontologia.GenararNamespacesHuerfanos;
                ontologiaAux.GestorOWL.NamespaceOntologia = namespaceOnto;
                ontologiaAux.GestorOWL.UrlOntologia = urlOntologia;
                ontologiaAux.EstilosPlantilla = new Dictionary<string, List<EstiloPlantilla>>();
                ontologiaAux.EstilosPlantilla.Add("[ConfiguracionGeneral]", pPropiedad.Ontologia.EstilosPlantilla["[ConfiguracionGeneral]"]);
                ontologiaAux.Entidades.Add(new ElementoOntologia(estiloPropAux.NombreEntidad, urlOntologia, namespaceOnto, ontologiaAux));
                ontologiaAux.TiposEntidades.Add(estiloPropAux.NombreEntidad);

                foreach (EstiloPlantillaEspecifProp estilo in pPropiedad.EspecifPropiedad.SelectorEntidad.PropiedadesLectura)
                {
                    if (estilo.NombreRealPropiedad == null)
                    {
                        continue;
                    }

                    AgregarPropiedadAOntologiaAuxiliar(estilo, ontologiaAux, ontologiaAux.Entidades[0]);
                }

                #endregion

                #endregion
            }

            List<string> entidadesPintar = new List<string>();

            if (!pPropiedad.FunctionalProperty && !pPropiedad.CardinalidadMenorOIgualUno && pPropiedad.UnicoValor.Key == null)
            {
                entidadesPintar.AddRange(pPropiedad.ListaValores.Keys);
            }
            else if (pPropiedad.UnicoValor.Key != null)
            {
                entidadesPintar.Add(pPropiedad.UnicoValor.Key);
            }
            else if (pPropiedad.ListaValores.Count > 0)
            {
                entidadesPintar.AddRange(pPropiedad.ListaValores.Keys);
            }

            pFacetadoDS = facetadoDS;
            pEstiloPropAux = estiloPropAux;
            pOntologia = ontologiaAux;

            return entidadesPintar;
        }

        /// <summary>
        /// Agrega la propiedad de un estilo plantilla e una entidad
        /// </summary>
        /// <param name="pEstilo">Estilo de plantilla</param>
        /// <param name="pOntologia">Ontología</param>
        /// <param name="pEntidad">Entidad</param>
        private static void AgregarPropiedadAOntologiaAuxiliar(EstiloPlantillaEspecifProp pEstilo, Ontologia pOntologia, ElementoOntologia pEntidad)
        {
            List<string> dominio = new List<string>();
            dominio.Add(pEntidad.TipoEntidad);

            Propiedad propiedad = new Propiedad(pEstilo.NombreRealPropiedad, TipoPropiedad.DatatypeProperty, dominio, "http://www.w3.org/2001/XMLSchema#string", false, pOntologia);
            pEntidad.Propiedades.Add(propiedad);
            propiedad.ElementoOntologia = pEntidad;

            if (!pOntologia.EstilosPlantilla.ContainsKey(pEstilo.NombreRealPropiedad))
            {
                List<EstiloPlantilla> listaEstilos = new List<EstiloPlantilla>();
                listaEstilos.Add(pEstilo);
                pOntologia.EstilosPlantilla.Add(pEstilo.NombreRealPropiedad, listaEstilos);
            }
            else
            {
                pOntologia.EstilosPlantilla[pEstilo.NombreRealPropiedad].Add(pEstilo);
            }

            if (pEstilo.PropiedadesAuxiliares != null && pEstilo.PropiedadesAuxiliares.Count > 0)
            {
                if (pOntologia.TiposEntidades.Contains(pEstilo.PropiedadesAuxiliares[0].NombreEntidad))
                {
                    throw new Exception("Entidad del selector con propiedad '" + pEstilo.PropiedadesAuxiliares[0].NombreRealPropiedad + "' y entidad '" + pEstilo.PropiedadesAuxiliares[0].NombreEntidad + "' duplicada.");
                }

                propiedad.Tipo = TipoPropiedad.ObjectProperty;
                propiedad.Rango = pEstilo.PropiedadesAuxiliares[0].NombreEntidad;
                ElementoOntologia entidadHija = new ElementoOntologia(pEstilo.PropiedadesAuxiliares[0].NombreEntidad, pOntologia.GestorOWL.UrlOntologia, pOntologia.GestorOWL.NamespaceOntologia, pOntologia);
                pOntologia.Entidades.Add(entidadHija);
                pOntologia.TiposEntidades.Add(entidadHija.TipoEntidad);

                foreach (EstiloPlantillaEspecifProp estiloProp in pEstilo.PropiedadesAuxiliares)
                {
                    AgregarPropiedadAOntologiaAuxiliar(estiloProp, pOntologia, entidadHija);
                }
            }
            else if (pEstilo.SelectorEntidad != null)
            {
                propiedad.Tipo = TipoPropiedad.ObjectProperty;
            }
        }

        /// <summary>
        /// Genera la ontología auxiliar para entidades externas.
        /// </summary>
        /// <param name="pPropiedad">Propiedad</param>
        /// <param name="pDatosEntidadesExternas">Datos de las entidades externas</param>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación con los datos cargados</param>
        /// <returns>Lista con las entidades a pintar</returns>
        public Dictionary<Guid, string> GenerarOntologiaAuxiliarDocumentosExternos(Propiedad pPropiedad, Dictionary<KeyValuePair<string, string>, object[]> pDatosEntidadesExternas, out DataWrapperDocumentacion pDataWrapperDocumentacion)
        {
            Dictionary<Guid, string> listaRec = new Dictionary<Guid, string>();
            DataWrapperDocumentacion dataWrapperDocumentacion = null;

            if (pPropiedad.UnicoValor.Key != null || pPropiedad.ListaValores.Count > 0)
            {
                foreach (string url in pPropiedad.ValoresUnificados.Keys)
                {
                    string urlRec = url.Trim();
                    if (urlRec.LastIndexOf("/") == urlRec.Length - 1)
                    {
                        urlRec = urlRec.Substring(0, urlRec.Length - 1);
                    }

                    try
                    {
                        listaRec.Add(new Guid(urlRec.Substring(urlRec.LastIndexOf("/") + 1)), url);
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.AgregarEntrada("FormSem documento externo con mal formato '" + url + "', error: " + ex.ToString());
                        //mGeneradorPantillaOWL.GuardarLogErrorAJAX(ex.ToString());
                    }
                }

                if (pDatosEntidadesExternas.ContainsKey(new KeyValuePair<string, string>(pPropiedad.Nombre, pPropiedad.ElementoOntologia.TipoEntidad)))
                {
                    dataWrapperDocumentacion = (DataWrapperDocumentacion)pDatosEntidadesExternas[new KeyValuePair<string, string>(pPropiedad.Nombre, pPropiedad.ElementoOntologia.TipoEntidad)][3];
                }
                else if (pDatosEntidadesExternas.ContainsKey(new KeyValuePair<string, string>(pPropiedad.Nombre, pPropiedad.EspecifPropiedad.NombreEntidad)))
                {
                    dataWrapperDocumentacion = (DataWrapperDocumentacion)pDatosEntidadesExternas[new KeyValuePair<string, string>(pPropiedad.Nombre, pPropiedad.EspecifPropiedad.NombreEntidad)][3];
                }
                else
                {
                    dataWrapperDocumentacion = new DataWrapperDocumentacion();
                }
            }

            pDataWrapperDocumentacion = dataWrapperDocumentacion;

            return listaRec;
        }

        /// <summary>
        /// Obtiene el valor según las propiedades de edición.
        /// </summary>
        /// <param name="pFacetadoDS">Dataset de facetas</param>
        /// <param name="pValor">Valor real</param>
        /// <param name="pSepPrin">Separador principal</param>
        /// <param name="pSepEntreProps">Separador entre propiedades</param>
        /// <param name="pSepFin">Separador final</param>
        /// <param name="pPropsEdicion">Propiedades de edición</param>
        /// <param name="pIdioma">Idioma del usuario</param>
        /// <returns>Valor según las propiedades de edición</returns>
        public static string ObtenerValorSegunPropsEdicion(FacetadoDS pFacetadoDS, string pValor, string pSepPrin, string pSepEntreProps, string pSepFin, List<string> pPropsEdicion, string pIdioma)
        {
            string textoValor = "";
            string extra = "";

            foreach (string prop in pPropsEdicion)
            {
                List<string> valoresExtra = FacetadoCN.ObtenerObjetosDataSetSegunPropiedad(pFacetadoDS, pValor, prop, pIdioma);

                foreach (string valorExtra in valoresExtra)
                {
                    if (textoValor == "")
                    {
                        textoValor = valorExtra;
                    }
                    else
                    {
                        extra += valorExtra + pSepEntreProps;
                    }
                }
            }

            if (extra != "")
            {
                if (pSepEntreProps != null)
                {
                    extra = extra.Substring(0, extra.Length - pSepEntreProps.Length);
                }
                extra = pSepPrin + extra + pSepFin;
            }

            textoValor += extra;

            return textoValor;
        }

        /// <summary>
        /// Obtiene las propiedades que son selección de entidades externas.
        /// </summary>
        /// <param name="pEntidad">Entidad sobre la que se busca</param>
        /// <param name="pDatosEntidadesExternas">Datos del selector</param>
        /// <param name="pUrlIntragnoss">Url de intragnosss</param>
        /// <param name="pFilaPersona">Fila de la persona conectada</param>
        /// <param name="pFilaProy">Fila del proyecto actual</param>
        /// <param name="pEntidades">Entidades principales desde las que se hace la consulta</param>
        public void ObtenerPropiedadesSelecEntDeEntidad(ElementoOntologia pEntidad, Dictionary<KeyValuePair<string, string>, object[]> pDatosEntidadesExternas, string pUrlIntragnoss, Gnoss.AD.EntityModel.Models.PersonaDS.Persona pFilaPersona, Gnoss.AD.EntityModel.Models.ProyectoDS.Proyecto pFilaProy, List<ElementoOntologia> pEntidades)
        {
            ObtenerPropiedadesSelecEntDeEntidad(pEntidad, pDatosEntidadesExternas, pUrlIntragnoss, pFilaPersona, pFilaProy, pEntidades, null, null);
        }

        /// <summary>
        /// Obtiene las propiedades que son selección de entidades externas.
        /// </summary>
        /// <param name="pEntidad">Entidad sobre la que se busca</param>
        /// <param name="pDatosEntidadesExternas">Datos del selector</param>
        /// <param name="pUrlIntragnoss">Url de intragnosss</param>
        /// <param name="pInicioPag">NULL si hay que traer todos o Número del inicio de la paginación</param>
        /// <param name="pPropiedadPag">Propiedad por la que se está paginando o NULL si no se está pagiando</param>
        /// <param name="pFilaPersona">Fila de la persona conectada</param>
        /// <param name="pFilaProy">Fila del proyecto actual</param>
        /// <param name="pEntidades">Entidades principales desde las que se hace la consulta</param>
        public void ObtenerPropiedadesSelecEntDeEntidad(ElementoOntologia pEntidad, Dictionary<KeyValuePair<string, string>, object[]> pDatosEntidadesExternas, string pUrlIntragnoss, Gnoss.AD.EntityModel.Models.PersonaDS.Persona pFilaPersona, Gnoss.AD.EntityModel.Models.ProyectoDS.Proyecto pFilaProy, List<ElementoOntologia> pEntidades, int? pInicioPag, string pPropiedadPag)
        {
            ObtenerPropiedadesSelecEntDeEntidad(pEntidad, pDatosEntidadesExternas, pUrlIntragnoss, pFilaPersona, pFilaProy, pEntidades, pInicioPag, pPropiedadPag, false, false);
        }

        /// <summary>
        /// Obtiene las propiedades que son selección de entidades externas.
        /// </summary>
        /// <param name="pEntidad">Entidad sobre la que se busca</param>
        /// <param name="pDatosEntidadesExternas">Datos del selector</param>
        /// <param name="pUrlIntragnoss">Url de intragnosss</param>
        /// <param name="pInicioPag">NULL si hay que traer todos o Número del inicio de la paginación</param>
        /// <param name="pPropiedadPag">Propiedad por la que se está paginando o NULL si no se está pagiando</param>
        /// <param name="pFilaPersona">Fila de la persona conectada</param>
        /// <param name="pFilaProy">Fila del proyecto actual</param>
        /// <param name="pEntidades">Entidades principales desde las que se hace la consulta</param>
        /// <param name="pPaginarEntAux">Indica si se deben paginas las entidades auxiliares</param>
        public void ObtenerPropiedadesSelecEntDeEntidad(ElementoOntologia pEntidad, Dictionary<KeyValuePair<string, string>, object[]> pDatosEntidadesExternas, string pUrlIntragnoss, Gnoss.AD.EntityModel.Models.PersonaDS.Persona pFilaPersona, Gnoss.AD.EntityModel.Models.ProyectoDS.Proyecto pFilaProy, List<ElementoOntologia> pEntidades, int? pInicioPag, string pPropiedadPag, bool pPaginarEntAux, bool pHayPropiedadIdiomaBusquedaComunidad)
        {
            foreach (Propiedad propiedad in pEntidad.Propiedades)
            {
                if (propiedad.Tipo == TipoPropiedad.ObjectProperty)
                {
                    if (propiedad.EspecifPropiedad.SelectorEntidad != null)
                    {
                        if (pPropiedadPag != null && propiedad.Nombre != pPropiedadPag)
                        {//Evitamos que se consulte esta propiedad ya que no se va a usar
                            propiedad.LimpiarValor();
                            continue;
                        }

                        KeyValuePair<string, string> claveProp = new KeyValuePair<string, string>(propiedad.Nombre, pEntidad.TipoEntidad);
                        if (!pDatosEntidadesExternas.ContainsKey(claveProp))
                        {
                            if (pHayPropiedadIdiomaBusquedaComunidad)
                            {
                                propiedad.EspecifPropiedad.SelectorEntidad.MultiIdioma = pHayPropiedadIdiomaBusquedaComunidad;
                            }

                            pDatosEntidadesExternas.Add(claveProp, new object[4]);

                            List<string> listaPropiedades = new List<string>();

                            foreach (EstiloPlantillaEspecifProp estiloProp in propiedad.EspecifPropiedad.SelectorEntidad.PropiedadesLectura)
                            {
                                if (estiloProp.NombreRealPropiedad == null || (estiloProp.SelectorEntidad != null && estiloProp.SelectorEntidad.Reciproca && estiloProp.SelectorEntidad.TipoSeleccion != "Edicion"))
                                {
                                    continue;
                                }

                                listaPropiedades.Add(ObtenerNombreConJerarquiaProp(estiloProp));
                            }

                            pDatosEntidadesExternas[claveProp][0] = listaPropiedades;
                            pDatosEntidadesExternas[claveProp][1] = new List<string>();
                            pDatosEntidadesExternas[claveProp][2] = propiedad.EspecifPropiedad.SelectorEntidad;

                            mLoggingService.AgregarEntrada("FormSem Agrego SelectorEntidad para '" + propiedad.Nombre + "','" + pEntidad.TipoEntidad + "'");

                            if (propiedad.EspecifPropiedad.SelectorEntidad.Reciproca) //Busco entidades reciprocas
                            {
                                List<string> sujetosEntExtReci = ObtenerSujetosEntiadesSelectorReciprocoDeEntidad(pUrlIntragnoss, propiedad.EspecifPropiedad.SelectorEntidad, pUrlIntragnoss + "items/" + pEntidad.ID, pFilaPersona, pFilaProy, pEntidades);

                                if (propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion != "Edicion")//Así si hay una consulta configurada, solo traera los datos que satisfagan esta.
                                {
                                    propiedad.LimpiarValor();
                                }

                                foreach (string sujetoEntExt in sujetosEntExtReci)
                                {
                                    if (propiedad.FunctionalProperty)
                                    {
                                        if (propiedad.UnicoValor.Key == null)
                                        {
                                            propiedad.UnicoValor = new KeyValuePair<string, ElementoOntologia>(sujetoEntExt, null);
                                        }

                                        break;
                                    }
                                    else
                                    {
                                        if (!propiedad.ListaValores.ContainsKey(sujetoEntExt))
                                        {
                                            propiedad.ListaValores.Add(sujetoEntExt, null);
                                        }
                                    }
                                }
                            }
                        }

                        mLoggingService.AgregarEntrada("FormSem SelectorEntidad con " + propiedad.ValoresUnificados.Count + " entidades externas para buscar");

                        if (pInicioPag == null || propiedad.EspecifPropiedad.SelectorEntidad.NumElemPorPag == 0)
                        {
                            ((List<string>)pDatosEntidadesExternas[claveProp][1]).AddRange(propiedad.ValoresUnificados.Keys);
                        }
                        else
                        {
                            List<string> valores = new List<string>(propiedad.ValoresUnificados.Keys);
                            for (int i = pInicioPag.Value; i < (pInicioPag.Value + propiedad.EspecifPropiedad.SelectorEntidad.NumElemPorPag); i++)
                            {
                                if (i < valores.Count)
                                {
                                    ((List<string>)pDatosEntidadesExternas[claveProp][1]).Add(valores[i]);
                                }
                            }
                        }
                    }
                    else
                    {
                        List<ElementoOntologia> entidadesHijas = null;
                        if (pPaginarEntAux && propiedad.EspecifPropiedad.NumElemPorPag > 0 && propiedad.ListaValores.Count > 0)
                        {
                            entidadesHijas = new List<ElementoOntologia>();
                            List<ElementoOntologia> valoresCompl = new List<ElementoOntologia>(propiedad.ListaValores.Values);
                            int inicio = 0;

                            if (pInicioPag.HasValue && pPropiedadPag != null && propiedad.Nombre == pPropiedadPag)
                            {
                                inicio = pInicioPag.Value;
                            }

                            int fin = inicio + propiedad.EspecifPropiedad.NumElemPorPag;

                            for (int i = inicio; i < fin; i++)
                            {
                                if (i < valoresCompl.Count)
                                {
                                    entidadesHijas.Add(valoresCompl[i]);
                                }
                            }
                        }
                        else
                        {
                            entidadesHijas = new List<ElementoOntologia>(propiedad.ValoresUnificados.Values);
                        }

                        foreach (ElementoOntologia entidad in entidadesHijas)
                        {
                            if (entidad != null)
                            {
                                int? inicioPag = null;
                                string propPag = null;

                                if (pPropiedadPag != null && pPropiedadPag != propiedad.Nombre)
                                {
                                    inicioPag = pInicioPag;
                                    propPag = pPropiedadPag;
                                }

                                ObtenerPropiedadesSelecEntDeEntidad(entidad, pDatosEntidadesExternas, pUrlIntragnoss, pFilaPersona, pFilaProy, pEntidades, inicioPag, propPag);
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
