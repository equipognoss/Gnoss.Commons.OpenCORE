using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.Util.GeneradorClases;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.GeneradorClases;
using Es.Riam.Gnoss.Web.MVC.Models.GeneradorClases.Enumeraciones;
using Es.Riam.Semantica.OWL;
using Es.Riam.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.InterfacesOpen
{
    public abstract class IMassiveOntologyToClass
    {

        protected LoggingService mLoggingService;
        public string RdfType { get; set; }

        public IMassiveOntologyToClass(LoggingService loggingService)
        {
            mLoggingService = loggingService;
        }


        public abstract void CrearToOntologyGraphTriples(bool pEsPrincipal, ElementoOntologia pEntidad, StringBuilder Clase, List<Propiedad> listentidadesAux, Ontologia ontologia, Dictionary<string, string> dicPref, Dictionary<string, bool> propListiedadesMultidioma, List<ObjetoPropiedad> listaObjetosPropiedad);
        public abstract void CrearToSearchGraphTriples(bool pEsPrincipal, ElementoOntologia pEntidad, List<string> pListaPropiedadesSearch, List<string> pListaPadrePropiedadesAnidadas, StringBuilder Clase, Ontologia ontologia, string nombrePropDescripcion, string nombrePropTitulo, string nombrePropTituloEntero, Dictionary<string, bool> propListiedadesMultidioma, List<ObjetoPropiedad> listaObjetosPropiedad, List<Propiedad> listentidadesAux, Dictionary<string, string> dicPref, Dictionary<Propiedad, bool> dicPropMultiidiomaFalse);
        public abstract void CrearToAcidData(bool pEsPrincipal, ElementoOntologia pEntidad, Ontologia ontologia, StringBuilder Clase, string nombrePropDescripcion, string nombrePropTitulo, string nombrePropTituloEntero, Dictionary<string, bool> propListiedadesMultidioma, List<ObjetoPropiedad> listaObjetosPropiedad, Dictionary<Propiedad, bool> dicPropiedadMultiidiomaFalse);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pPropiedad"></param>
        /// <returns></returns>
        public bool EsPropiedadMultiIdioma(string pPropiedad, Dictionary<string, bool> propListiedadesMultidioma)
        {
            return propListiedadesMultidioma.ContainsKey(pPropiedad) && propListiedadesMultidioma[pPropiedad];
        }

        public bool EsPropiedadExternaMultiIdioma(ElementoOntologia pEntidad, Propiedad prop, List<ObjetoPropiedad> listaObjetosPropiedad)
        {
            return EsPropiedadExternaMultiIdioma(pEntidad, prop.Nombre, listaObjetosPropiedad);
        }

        public bool EsPropiedadTituloDocMultiIdioma(string pNombre, List<ObjetoPropiedad> listaObjetosPropiedad)
        {
            ObjetoPropiedad propiedad = listaObjetosPropiedad.FirstOrDefault(item => item.NombrePropiedad.Equals(pNombre));
            if (propiedad == null)
            {
                throw new Exception($"no está definida la propiedad {pNombre} configurada como título del documento.");
            }
            return propiedad.Multiidioma;
        }

        public bool EsPropiedadExternaMultiIdioma(ElementoOntologia pEntidad, string pNombre, List<ObjetoPropiedad> listaObjetosPropiedad)
        {
            //Cambiar RDFType
            ObjetoPropiedad propiedad = listaObjetosPropiedad.FirstOrDefault(item => item.NombrePropiedad.Equals(pNombre) && item.NombreEntidad.Equals(pEntidad.TipoEntidad) && item.NombreOntologia.ToLower().Equals(RdfType.ToLower()));
            if (propiedad == null)
            {
                throw new Exception($"la entidad {pEntidad.TipoEntidad} no contiene la propiedad {pNombre}");
            }
            return propiedad.Multiidioma;
        }

        /// <summary>
        /// Nos genera el sujeto para una entidad indicada. En función de lo que se indique generará para el grafo de búsqueda o de ontología
        /// </summary>
        /// <param name="pEntidad">Entidad del sujeto del cual se va a generar el triple</param>
        /// <param name="pTipoSujeto">Tipo de sujeto a generar, para el grafo de ontología, búsqueda o para el has entidad</param>
        /// <param name="pItem">Indica el nivel de anidamiento de las entidades auxiliares, si no se indica será una entidad principal</param>
        /// <returns>Devuelve el sujeto del triple de la entidad indicada, para el grafo de búsqueda o de ontología según se indique</returns>
        public string GenerarSujeto(ElementoOntologia pEntidad, TiposSujeto pTipoSujeto, bool pEsPrincipal = true, string pItem = "")
        {
            switch (pTipoSujeto)
            {
                case TiposSujeto.Busqueda:
                    return GenerarSujetoBusqueda(pEsPrincipal);
                case TiposSujeto.Ontologia:
                    return GenerarSujetoOntologia(pEntidad, pEsPrincipal, pItem);
                case TiposSujeto.HasEntidad:
                    return GenerarSujetoHasEntidad(pEsPrincipal);
                default:
                    return string.Empty;
            }
        }


        /// <summary>
        /// Genera el sujeto de los triples para el grafo de búsqueda
        /// </summary>
        /// <returns>Devuelve el sujeto para el triple de la entidad indicada para el grafo de búsqueda</returns>
        private string GenerarSujetoBusqueda(bool pEsPrincipal)
        {
            if (pEsPrincipal)
            {
                return "http://gnoss/{ResourceID.ToString().ToUpper()}";
            }
            else
            {
                //CAMBIOS MINUSCULA 
                //return "{resourceAPI.GraphsUrl.ToLower()}items/{Identificador.ToLower()}";
                return "{resourceAPI.GraphsUrl.ToLower()}items/{Identificador}";
            }
        }

        /// <summary>
        /// Genera el sujeto de los triples para el grafo de ontología
        /// </summary>
        /// <param name="pEntidad">La entidad de la cual vamos a generar el sujeto</param>
        /// <param name="pItem">Indica el nivel de anidamiento de las entidades auxiliares, si no se indica será una entidad principal</param>
        /// <returns>Devuelve el sujeto para el triple de la entidad indicada para el grafo de ontología</returns>
        private string GenerarSujetoOntologia(ElementoOntologia pEntidad, bool pEsPrincipal, string pItem = "")
        {
            if (pEsPrincipal)
            {
                string articleID = "ArticleID";

                if (!string.IsNullOrEmpty(pItem))
                {
                    articleID = $"{pItem}.{articleID}";
                }

                return $"{{resourceAPI.GraphsUrl}}items/{pEntidad.TipoEntidadRelativo}_{{ResourceID}}_{{{articleID}}}";
            }
            else
            {
                return "{resourceAPI.GraphsUrl}items/{Identificador}";
            }
        }

        /// <summary>
        /// Genera el sujeto de los triples para la propiedad HasEntidad
        /// </summary>
        /// <returns>Devuelve el sujeto para el triple de la propiedad HasEntidad</returns>
        private string GenerarSujetoHasEntidad(bool pEsPrincipal)
        {
            if (pEsPrincipal)
            {
                return "{resourceAPI.GraphsUrl}{ResourceID}";
            }
            else
            {
                return "{resourceAPI.GraphsUrl}entidadsecun_{Identificador.ToLower()}";
            }            
        }

        /// <summary>
        /// Se encarga de pintar en la clase de la ontología indicada sus entidades auxiliares.
        /// </summary>
        /// <param name="pElem">Ontología de la cual se van a pintar las entidades auxiliares</param>
        /// <param name="pPropiedadPadre">Propiedad propiedad padre</param>
        /// <param name="pElemPadre">En caso de ser entidades auxiliares anidadas, ontología de donde proviene las entidades que se van a pintar</param>
        /// <param name="pEsOntologia">Nos indica si las entidades auxiliares se van a pintar para el grafo de búsqueda o de ontología. Si no es uno, es otro</param>
        /// <param name="pNombrePadres">Nombre de la jerarquía de entidades y propiedades hasta llegar al nivel actual</param>
        /// <param name="numIteraciones">Numero de veces que se ha utilizado el método recursivamente</param>
        protected void PintarEntidadesAuxiliares(ElementoOntologia pElem, Propiedad pPropiedadPadre, ElementoOntologia pElemPadre, bool pEsOntologia, string pSujetoEntidadSuperior, StringBuilder Clase, Dictionary<string, string> dicPref, Dictionary<string, bool> propListiedadesMultidioma, List<ObjetoPropiedad> listaObjetosPropiedad, bool pEsPrincipal, List<string> pListaPropiedadesSearch = null, List<string> pListaPadrePropiedadesAnidadas = null, string pNombrePadres = "this", int numIteraciones = 0)
        {
            string prefijoPadre = UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, pPropiedadPadre.Nombre, mLoggingService));
            string nombrePropPadre = UtilCadenasOntology.ObtenerNombreProp(pPropiedadPadre.Nombre);
            string nombreCompletoPadre = $"{pNombrePadres}.{prefijoPadre}_{nombrePropPadre}";
            TiposSujeto tipoSujeto = TiposSujeto.Busqueda;
            if (pEsOntologia)
            {
                tipoSujeto = TiposSujeto.Ontologia;
            }

            string item = nombreCompletoPadre;

            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if({nombreCompletoPadre} != null)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");

            if (!pPropiedadPadre.ValorUnico)
            {
                item = $"item{numIteraciones}";
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}foreach(var {item} in {nombreCompletoPadre})");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
            }

            string sujetoEntidadAuxiliar;

            if (pEsOntologia)
            {
                sujetoEntidadAuxiliar = $"{{resourceAPI.GraphsUrl}}items/{pElem.TipoEntidadRelativo}_{{ResourceID}}_{{{item}.ArticleID}}";

                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}AgregarTripleALista($\"{GenerarSujeto(pElem, tipoSujeto, pEsPrincipal, item)}\", \"http://www.w3.org/1999/02/22-rdf-syntax-ns#type\", $\"<{pElem.TipoEntidad}>\", list, \" . \");");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}AgregarTripleALista($\"{GenerarSujeto(pElem, tipoSujeto, pEsPrincipal, item)}\", \"http://www.w3.org/2000/01/rdf-schema#label\", $\"\\\"{pElem.TipoEntidad}\\\"\", list, \" . \");");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}AgregarTripleALista($\"{GenerarSujeto(pElem, TiposSujeto.HasEntidad, pEsPrincipal)}\", \"http://gnoss/hasEntidad\", $\"<{{resourceAPI.GraphsUrl}}items/{pElem.TipoEntidadRelativo}_{{ResourceID}}_{{{item}.ArticleID}}>\", list, \" . \");");
            }
            else
            {
                //CAMBIOS A OBJETOS EN GRAFO DE BÚSQUEDA SIN MINÚSCULAS
                //sujetoEntidadAuxiliar = $"{{resourceAPI.GraphsUrl}}items/{pElem.TipoEntidadRelativo.ToLower()}_{{ResourceID}}_{{{item}.ArticleID}}";
                sujetoEntidadAuxiliar = $"{{resourceAPI.GraphsUrl}}items/{pElem.TipoEntidadRelativo}_{{ResourceID}}_{{{item}.ArticleID}}";
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}AgregarTripleALista($\"http://gnossAuxiliar/{{ResourceID.ToString().ToUpper()}}\", \"http://gnoss/hasEntidadAuxiliar\", $\"<{sujetoEntidadAuxiliar}>\", list, \" . \");");
            }

            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}AgregarTripleALista($\"{pSujetoEntidadSuperior}\", \"{pPropiedadPadre.NombreFormatoUri}\", $\"<{sujetoEntidadAuxiliar}>\", list, \" . \");");

            foreach (ElementoOntologia elem in pElem.Ontologia.EntidadesAuxiliares)
            {
                foreach (Propiedad propiedadPadre in pElem.Propiedades)
                {
                    if (elem.TipoEntidad.Equals(propiedadPadre.Rango) && !pElem.TipoEntidad.Equals(propiedadPadre.Rango))
                    {
                        PintarEntidadesAuxiliares(elem, propiedadPadre, pElem, pEsOntologia, sujetoEntidadAuxiliar, Clase, dicPref, propListiedadesMultidioma, listaObjetosPropiedad, pEsPrincipal, pListaPropiedadesSearch, pListaPadrePropiedadesAnidadas, item, ++numIteraciones);
                    }
                }
            }

            PintarPropiedades(pElem, pEsOntologia, sujetoEntidadAuxiliar, Clase, dicPref, propListiedadesMultidioma, listaObjetosPropiedad, pEsPrincipal, pPropiedadPadre.NombreConNamespace, item, pListaPropiedadesSearch, pListaPadrePropiedadesAnidadas);

            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");

            if (!pPropiedadPadre.ValorUnico)
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
            }
        }

        /// <summary>
        /// Se encarga de pintar en la clase las propiedades principales de la entidad.
        /// </summary>
        /// <param name="pElem">Ontología de la cual se van a pintar las propiedades</param>
        /// <param name="pEsOntologia">Nos indica si las propiedades a pintar son para el grafo de ontología o para el grafo de búsqueda</param>
        /// <param name="pNombrePadres">Nombre de la jerarquía de entidades y propiedades hasta llegar al nivel actual</param>
        protected void PintarPropiedades(ElementoOntologia pElem, bool pEsOntologia, string pSujetoEntidadSuperior, StringBuilder Clase, Dictionary<string, string> dicPref, Dictionary<string, bool> propListiedadesMultidioma, List<ObjetoPropiedad> listaObjetosPropiedad, bool pEsPrincipal, string pRutaPadreSearch = null, string pNombrePadres = "this", List<string> pListaPropiedadesSearch = null, List<string> pListaPadrePropiedadesAnidadas = null)
        {
            foreach (Propiedad prop in pElem.Propiedades)
            {
                bool pintarSearch = false;
                bool esPropiedadSearchAnidada = false;
                string propiedadSearchAnidada = string.Empty;
                if (!pElem.Ontologia.EntidadesAuxiliares.Any(x => x.TipoEntidad.Equals(prop.Rango)))
                {
                    ConfiguracionObjeto configuracionObjeto = new ConfiguracionObjeto(dicPref, prop, pElem, pEsOntologia, mLoggingService);

                    string identificadorValor = $"{configuracionObjeto.Id}{configuracionObjeto.PrefijoPropiedad}_{configuracionObjeto.NombrePropiedad}";
                    string propiedadParaSearch = $"{configuracionObjeto.PrefijoPropiedad}:{configuracionObjeto.NombrePropiedad}".ToLower();

                    if (!string.IsNullOrEmpty(pRutaPadreSearch))
                    {
                        propiedadParaSearch = $"{pRutaPadreSearch}@@@{propiedadParaSearch}";
                    }

                    if (pListaPropiedadesSearch != null && pListaPropiedadesSearch.Contains(propiedadParaSearch) && !pEsOntologia && pEsPrincipal)
                    {
                        pintarSearch = true;
                    }

                    if (pListaPadrePropiedadesAnidadas != null && pListaPadrePropiedadesAnidadas.Where(item => item.StartsWith(propiedadParaSearch)).FirstOrDefault() != null && !pEsOntologia)
                    {
                        esPropiedadSearchAnidada = true;
                        propiedadSearchAnidada = pListaPropiedadesSearch.Where(item => item.Contains(propiedadParaSearch)).FirstOrDefault();
                    }

                    if (configuracionObjeto.Rango.ToLower().Equals("datetime"))
                    {
                        if (prop.CardinalidadMaxima > 1)
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}if({pNombrePadres}.{identificadorValor} != null");
                        }
                        else
                        {
                            if (prop.ValorUnico)
                            {
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}if({pNombrePadres}.{identificadorValor} != null && {pNombrePadres}.{identificadorValor} != DateTime.MinValue)");
                            }
                            else
                            {
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}if ({pNombrePadres}.{identificadorValor} != null)");
                            }
                            
                        }
                    }
                    else
                    {
                        if (pintarSearch && configuracionObjeto.Rango.ToLower().Equals("string") && !EsPropiedadMultiIdioma(prop.Nombre, propListiedadesMultidioma) && !EsPropiedadExternaMultiIdioma(pElem, prop, listaObjetosPropiedad))
                        {
                            if (prop.ValorUnico)
                            {
								Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}if(!string.IsNullOrEmpty({pNombrePadres}.{identificadorValor}))");
                            }
                            else
                            {
								Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}if({pNombrePadres}.{identificadorValor} != null)");
							}                        
                        }
                        else
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}if({pNombrePadres}.{identificadorValor} != null)");
                        }
                    }

                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");

                    if (prop.CardinalidadMinima < 1)
                    {
                        if (!prop.ValorUnico)
                        {
                            if (EsPropiedadMultiIdioma(prop.Nombre, propListiedadesMultidioma) || EsPropiedadExternaMultiIdioma(pElem, prop, listaObjetosPropiedad))
                            {
                                GenerarPropiedadMultiIdioma(prop, pSujetoEntidadSuperior, configuracionObjeto, pNombrePadres, identificadorValor, pintarSearch, Clase);
                            }
                            else
                            {
                                string valorTriple = "item2";
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}foreach(var item2 in {pNombrePadres}.{identificadorValor})");
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}{{");
                                if (!pEsOntologia)
                                {
                                    valorTriple = ModificarAIDCorto(valorTriple, configuracionObjeto.EsObject, pEsOntologia, Clase);
                                }
                                if (configuracionObjeto.Rango.ToLower().Equals("datetime"))
                                {
                                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}if ({valorTriple} != DateTime.MinValue) {{");
                                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(7)}AgregarTripleALista($\"{pSujetoEntidadSuperior}\", \"{prop.NombreFormatoUri}\", $\"{configuracionObjeto.SimboloInicio}{valorTriple}{configuracionObjeto.Aux}{configuracionObjeto.SimboloFin}\", list, \" . \");");
                                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}}}");
                                }
                                else
                                {
                                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}AgregarTripleALista($\"{pSujetoEntidadSuperior}\", \"{prop.NombreFormatoUri}\", $\"{configuracionObjeto.SimboloInicio}{valorTriple}{configuracionObjeto.Aux}{configuracionObjeto.SimboloFin}\", list, \" . \");");
                                }
								if (pintarSearch)
								{
									Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}search += $\"{{{valorTriple}}} \";");
								}
								else if (esPropiedadSearchAnidada)
								{
									GenerarSearch(propiedadSearchAnidada, Clase);
								}
								Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}}}");

                                
                            }
                        }
                        else
                        {
                            if (EsPropiedadMultiIdioma(prop.Nombre, propListiedadesMultidioma) || EsPropiedadExternaMultiIdioma(pElem, prop, listaObjetosPropiedad))
                            {
                                GenerarPropiedadMultiIdioma(prop, pSujetoEntidadSuperior, configuracionObjeto, pNombrePadres, identificadorValor, pintarSearch, Clase);
                            }
                            else
                            {
                                string valorTriple = $"{pNombrePadres}.{identificadorValor}";
                                if (!pEsOntologia)
                                {
                                    valorTriple = ModificarAIDCorto(valorTriple, configuracionObjeto.EsObject, pEsOntologia, Clase);
                                }
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}AgregarTripleALista($\"{pSujetoEntidadSuperior}\",  \"{prop.NombreFormatoUri}\", $\"{configuracionObjeto.SimboloInicio}{valorTriple}{configuracionObjeto.Aux}{configuracionObjeto.SimboloFin}\", list, \" . \");");


                                if (pintarSearch)
                                {
                                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}search += $\"{{{valorTriple}}} \";");
                                }
                                else if (esPropiedadSearchAnidada)
                                {
                                    GenerarSearch(propiedadSearchAnidada, Clase);
                                }
                            }
                        }
                    }
                    else if (!prop.ValorUnico)
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}foreach(var item2 in {pNombrePadres}.{identificadorValor})");

                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}{{");

                        if (EsPropiedadMultiIdioma(prop.Nombre, propListiedadesMultidioma) || EsPropiedadExternaMultiIdioma(pElem, prop, listaObjetosPropiedad))
                        {
                            GenerarPropiedadMultiIdioma(prop, pSujetoEntidadSuperior, configuracionObjeto, pNombrePadres, identificadorValor, pintarSearch, Clase);
                        }
                        else
                        {
                            string valorTriple = $"item2";
                            if (!pEsOntologia)
                            {
                                valorTriple = ModificarAIDCorto(valorTriple, configuracionObjeto.EsObject, pEsOntologia, Clase);
                            }
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}AgregarTripleALista($\"{pSujetoEntidadSuperior}\", \"{prop.NombreFormatoUri}\",  $\"{configuracionObjeto.SimboloInicio}{valorTriple}{configuracionObjeto.Aux}{configuracionObjeto.SimboloFin}\", list, \" . \");");


                            if (pintarSearch)
                            {
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}search += $\"{{{valorTriple}}} \";");
                            }
                            else if (esPropiedadSearchAnidada)
                            {
                                GenerarSearch(propiedadSearchAnidada, Clase);
                            }
                        }
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}}}");
                    }
                    else
                    {
                        if (EsPropiedadMultiIdioma(prop.Nombre, propListiedadesMultidioma) || EsPropiedadExternaMultiIdioma(pElem, prop, listaObjetosPropiedad))
                        {
                            GenerarPropiedadMultiIdioma(prop, pSujetoEntidadSuperior, configuracionObjeto, pNombrePadres, identificadorValor, pintarSearch, Clase);
                        }
                        else
                        {
                            string valorTriple = $"{pNombrePadres}.{identificadorValor}";
                            if (!pEsOntologia)
                            {
                                valorTriple = ModificarAIDCorto(valorTriple, configuracionObjeto.EsObject, pEsOntologia, Clase);
                            }
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}AgregarTripleALista($\"{pSujetoEntidadSuperior}\", \"{prop.NombreFormatoUri}\",  $\"{configuracionObjeto.SimboloInicio}{valorTriple}{configuracionObjeto.Aux}{configuracionObjeto.SimboloFin}\", list, \" . \");");


                            if (pintarSearch)
                            {
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}search += $\"{{{valorTriple}}} \";");
                            }
                            else if (esPropiedadSearchAnidada)
                            {
                                GenerarSearch(propiedadSearchAnidada, Clase);
                            }
                        }
                    }
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
                }
            }
        }

        private string ModificarAIDCorto(string pNombreVariable, bool pEsObject, bool pEsOntologia, StringBuilder Clase)
        {
            if (pEsObject)
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}Regex regex = new Regex(@\"\\/items\\/.+_[0-9A-Fa-f]{{8}}[-]?(?:[0-9A-Fa-f]{{4}}[-]?){{3}}[0-9A-Fa-f]{{12}}_[0-9A-Fa-f]{{8}}[-]?(?:[0-9A-Fa-f]{{4}}[-]?){{3}}[0-9A-Fa-f]{{12}}\");");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}string itemRegex = {pNombreVariable};");

                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}if (regex.IsMatch(itemRegex))");

                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}{{");

                Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}itemRegex = $\"http://gnoss/{{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}}\";");

                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}}}");

                if (!pEsOntologia)
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}else");

                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}{{");

                    //CAMBIOS MINUSCULA
                    //Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}itemRegex = itemRegex.ToLower();");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}itemRegex = itemRegex.ToLower();");

                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}}}");
                }


                return "itemRegex";
            }
            return pNombreVariable;
        }

        private void GenerarSearch(string pPropiedad, StringBuilder Clase, string pNombreVariableEntidadActual = "this")
        {
            string[] listaPropiedadesAnidadas = pPropiedad.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);
            int longitudLista = listaPropiedadesAnidadas.Length;

            string nombreVariableActual = $"{pNombreVariableEntidadActual}.{UtilCadenas.PrimerCaracterAMayuscula(listaPropiedadesAnidadas[0]).Replace(":", "_")}";
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}foreach(dynamic itemProp0 in {nombreVariableActual})");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
            if(listaPropiedadesAnidadas.Length > 1)
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}List<dynamic> lista{UtilCadenas.PrimerCaracterAMayuscula(listaPropiedadesAnidadas[1]).Replace(":", "_")} = ObtenerObjetosDePropiedad(itemProp0.{UtilCadenas.PrimerCaracterAMayuscula(listaPropiedadesAnidadas[1]).Replace(":", "_")});");
            }
            else
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}List<dynamic> lista{UtilCadenas.PrimerCaracterAMayuscula(listaPropiedadesAnidadas[1]).Replace(":", "_")} = ObtenerObjetosDePropiedad(itemProp0);");
            }
            for (int i = 1; i + 1 < listaPropiedadesAnidadas.Length; i++)
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5 + i)}foreach(dynamic itemProp{i} in lista{UtilCadenas.PrimerCaracterAMayuscula(listaPropiedadesAnidadas[i]).Replace(":", "_")})");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5 + i)}{{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5 + i)}List<dynamic> lista{UtilCadenas.PrimerCaracterAMayuscula(listaPropiedadesAnidadas[i + 1]).Replace(":", "_")} = ObtenerObjetosDePropiedad(itemProp{i}.{UtilCadenas.PrimerCaracterAMayuscula(listaPropiedadesAnidadas[i + 1]).Replace(":", "_")});");
            }
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3 + longitudLista)}foreach(dynamic itemProp{longitudLista - 1} in lista{UtilCadenas.PrimerCaracterAMayuscula(listaPropiedadesAnidadas[longitudLista - 1]).Replace(":", "_")})");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3 + longitudLista)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4 + longitudLista)}listaSearch.AddRange(ObtenerStringDePropiedad(itemProp{longitudLista - 1}));");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3 + longitudLista)}}}");

            for (int i = longitudLista - 1; i > 0; i--)
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4 + i)}}}");
            }
        }

        private void GenerarPropiedadMultiIdioma(Propiedad pProp, string pSujetoEntidadSuperior, ConfiguracionObjeto pConfiguracionObjeto, string pNombrePadre, string pIdentificadorValor, bool pPintarSearch, StringBuilder Clase)
        {
            if (!pProp.ValorUnico)
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(7)}foreach (LanguageEnum idioma in {pNombrePadre}.{pIdentificadorValor}.Keys)");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(7)}{{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(8)}List<string> listaValores = {pNombrePadre}.{pIdentificadorValor}[idioma];");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(8)}foreach (string valor in listaValores)");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(8)}{{");
                if (pPintarSearch)
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(9)}search += $\"{{valor}} \";");
                }

                Clase.AppendLine($"{UtilCadenasOntology.Tabs(9)}AgregarTripleALista($\"{pSujetoEntidadSuperior}\", \"{pProp.NombreFormatoUri}\", $\"{pConfiguracionObjeto.SimboloInicio}valor{pConfiguracionObjeto.Aux}{pConfiguracionObjeto.SimboloFin}\", list, $\"@{{idioma}} . \");");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(8)}}}");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(7)}}}");
            }
            else
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(7)}foreach (LanguageEnum idioma in {pNombrePadre}.{pIdentificadorValor}.Keys)");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(7)}{{");
                if (pPintarSearch)
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(8)}search += $\"{{{pNombrePadre}.{pIdentificadorValor}[idioma]}} \";");
                }

                Clase.AppendLine($"{UtilCadenasOntology.Tabs(8)}AgregarTripleALista($\"{pSujetoEntidadSuperior}\", \"{pProp.NombreFormatoUri}\",  $\"{pConfiguracionObjeto.SimboloInicio}{pNombrePadre}.{pIdentificadorValor}[idioma]{pConfiguracionObjeto.Aux}{pConfiguracionObjeto.SimboloFin}\", list,  $\"@{{idioma}} . \");");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(7)}}}");
            }
        }
    }
}
