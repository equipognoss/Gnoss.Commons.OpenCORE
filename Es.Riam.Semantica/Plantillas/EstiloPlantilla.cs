using System;
using System.Collections.Generic;
using System.Text;
using Es.Riam.Semantica.OWL;
using System.Data;


namespace Es.Riam.Semantica.Plantillas
{
    /// <summary>
    /// Clase para gestionar los estilos de un elemento de la plantilla.
    /// </summary>

    [Serializable]
    public class EstiloPlantilla: IDisposable
    {
        #region Miembros

        #region Est�ticos

        /// <summary>
        /// Ontolog�as ya cargadas.
        /// </summary>
        public volatile static Dictionary<Guid, KeyValuePair<Guid, Ontologia>> OntologiasCargadas = new Dictionary<Guid, KeyValuePair<Guid, Ontologia>>();

        /// <summary>
        /// Ontolog�as ya cargadas.
        /// </summary>
        public volatile static Dictionary<string, KeyValuePair<Guid, Ontologia>> OntologiasTrozosCargadas = new Dictionary<string, KeyValuePair<Guid, Ontologia>>();

        #endregion

        #region Constantes

        /// <summary>
        /// Propiedad Collection para tesauro sem�ntico.
        /// </summary>
        public const string Collection_TesSem = "http://www.w3.org/2008/05/skos#Collection";

        /// <summary>
        /// Propiedad Source para tesauro sem�ntico.
        /// </summary>
        public const string Source_TesSem = "http://purl.org/dc/elements/1.1/source";

        /// <summary>
        /// Propiedad Member para tesauro sem�ntico.
        /// </summary>
        public const string Member_TesSem = "http://www.w3.org/2008/05/skos#member";

        /// <summary>
        /// Propiedad Identifier para tesauro sem�ntico.
        /// </summary>
        public const string Identifier_TesSem = "http://purl.org/dc/elements/1.1/identifier";

        /// <summary>
        /// Propiedad PrefLabel para tesauro sem�ntico.
        /// </summary>
        public const string PrefLabel_TesSem = "http://www.w3.org/2008/05/skos#prefLabel";

        /// <summary>
        /// Propiedad Broader para tesauro sem�ntico.
        /// </summary>
        public const string Broader_TesSem = "http://www.w3.org/2008/05/skos#broader";

        /// <summary>
        /// Propiedad Narrower para tesauro sem�ntico.
        /// </summary>
        public const string Narrower_TesSem = "http://www.w3.org/2008/05/skos#narrower";

        /// <summary>
        /// Propiedad Symbol para tesauro sem�ntico.
        /// </summary>
        public const string Symbol_TesSem = "http://www.w3.org/2008/05/skos#symbol";

        /// <summary>
        /// Propiedad Concept para tesauro sem�ntico.
        /// </summary>
        public const string Concept_TesSem = "http://www.w3.org/2008/05/skos#Concept";

        #endregion

        #endregion

        #region M�todos

        /// <summary>
        /// Busca a partir de la entidad Raiz la propiedad que contiene la entidad contenida.
        /// </summary>
        /// <param name="pEntidadesRaiz">Entidades ra�z</param>
        /// <param name="pEntidadContenida">Entidad contenida por la propiedad</param>
        /// <returns>Propiedad que contiene la entidad contenida</returns>
        public static Propiedad ObtenerPropiedadContieneEntidad(List<ElementoOntologia> pEntidadesRaiz, ElementoOntologia pEntidadContenida)
        {
            foreach (ElementoOntologia entidad in pEntidadesRaiz)
            {
                Propiedad prop = ObtenerPropiedadContieneEntidad(entidad, pEntidadContenida);

                if (prop != null)
                {
                    return prop;
                }
            }

            return null;
        }

        /// <summary>
        /// Busca a partir de la entidad Raiz la propiedad que contiene la entidad contenida.
        /// </summary>
        /// <param name="pEntidadRaiz">Entidad ra�z</param>
        /// <param name="pEntidadContenida">Entidad contenida por la propiedad</param>
        /// <returns>Propiedad que contiene la entidad contenida</returns>
        public static Propiedad ObtenerPropiedadContieneEntidad(ElementoOntologia pEntidadRaiz, ElementoOntologia pEntidadContenida)
        {
            if (pEntidadRaiz != null)
            {
                if (pEntidadRaiz.EntidadesRelacionadas.Contains(pEntidadContenida))
                {
                    foreach (Propiedad propiedad in pEntidadRaiz.Propiedades)
                    {
                        if (propiedad.ValoresUnificados.ContainsValue(pEntidadContenida))
                        {
                            return propiedad;
                        }
                    }
                }
                else
                {
                    foreach (ElementoOntologia entidad in pEntidadRaiz.EntidadesRelacionadas)
                    {
                        Propiedad prop = ObtenerPropiedadContieneEntidad(entidad, pEntidadContenida);

                        if (prop != null)
                        {
                            return prop;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Obtiene la propiedad con un determinado nombre que pertenece a la entidad o una de sus relaciones.
        /// </summary>
        /// <param name="pNombre">Nombre de la propiedad</param>
        /// <param name="pEntidad">Entidad</param>
        /// <returns>Propiedad con un determinado nombre que pertenece a la entidad o una de sus relaciones</returns>
        public static Propiedad ObtenerPropiedadACualquierNivelPorNombre(string pNombre, ElementoOntologia pEntidad)
        {
            return ObtenerPropiedadACualquierNivelPorNombre(pNombre, null, pEntidad);
        }

         /// <summary>
        /// Obtiene la propiedad con un determinado nombre que pertenece a la entidad o una de sus relaciones.
        /// </summary>
        /// <param name="pNombre">Nombre de la propiedad</param>
        /// <param name="pTipoEntidad">Tipo de entidad de la propiedad</param>
        /// <param name="pEntidad">Entidad</param>
        /// <returns>Propiedad con un determinado nombre que pertenece a la entidad o una de sus relaciones</returns>
        public static Propiedad ObtenerPropiedadACualquierNivelPorNombre(string pNombre, string pTipoEntidad, List<ElementoOntologia> pEntidades)
        {
            Propiedad propiedad = null;

            foreach (ElementoOntologia entidad in pEntidades)
            {
                propiedad = ObtenerPropiedadACualquierNivelPorNombre(pNombre, pTipoEntidad, entidad);

                if (propiedad != null)
                {
                    break;
                }
            }

            return propiedad;
        }

        /// <summary>
        /// Obtiene la propiedad con un determinado nombre que pertenece a la entidad o una de sus relaciones.
        /// </summary>
        /// <param name="pNombre">Nombre de la propiedad</param>
        /// <param name="pTipoEntidad">Tipo de entidad de la propiedad</param>
        /// <param name="pEntidad">Entidad</param>
        /// <returns>Propiedad con un determinado nombre que pertenece a la entidad o una de sus relaciones</returns>
        public static Propiedad ObtenerPropiedadACualquierNivelPorNombre(string pNombre, string pTipoEntidad, ElementoOntologia pEntidad)
        {
            if (pEntidad != null)
            {
                string nombrePropiedadEncaminante = null;
                string nombreVerdaderaPropiedad = null;

                if (pNombre.Contains("/") && !pNombre.Contains("http://"))
                {
                    nombrePropiedadEncaminante = pNombre.Substring(0, pNombre.IndexOf("/"));
                    nombreVerdaderaPropiedad = pNombre.Substring(pNombre.IndexOf("/") + 1);
                }

                foreach (Propiedad propiedad in pEntidad.Propiedades)
                {
                    if (nombrePropiedadEncaminante == null)
                    {
                        if (propiedad.Nombre == pNombre && EsPropiedadDeTipoEntidad(propiedad, pEntidad, pTipoEntidad))
                        {
                            return propiedad;
                        }

                        if (propiedad.Tipo == TipoPropiedad.ObjectProperty)
                        {
                            if (propiedad.FunctionalProperty && propiedad.UnicoValor.Key != null)
                            {
                                Propiedad propAux = ObtenerPropiedadACualquierNivelPorNombre(pNombre, pTipoEntidad, propiedad.UnicoValor.Value);
                                if (propAux != null)
                                {
                                    return propAux;
                                }
                            }
                            else if (!propiedad.FunctionalProperty && propiedad.ListaValores.Count > 0)
                            {
                                //Devolvemos la 1� entidad ya que solo nos intersa el nombre  de la propiedad que tenga �sta:
                                foreach (ElementoOntologia entidad in propiedad.ListaValores.Values)
                                {
                                    Propiedad propAux = ObtenerPropiedadACualquierNivelPorNombre(pNombre, pTipoEntidad, entidad);
                                    if (propAux != null)
                                    {
                                        return propAux;
                                    }
                                    break;
                                }
                            }
                            else
                            {
                                ElementoOntologia entAux = propiedad.Ontologia.GetEntidadTipo(propiedad.Rango, false);
                                Propiedad propAux = ObtenerPropiedadACualquierNivelPorNombre(pNombre, pTipoEntidad, entAux);
                                if (propAux != null)
                                {
                                    return propAux;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (propiedad.Tipo == TipoPropiedad.ObjectProperty && propiedad.Nombre == nombrePropiedadEncaminante)
                        {
                            if (propiedad.FunctionalProperty && propiedad.UnicoValor.Key != null)
                            {
                                Propiedad propAux = ObtenerPropiedadACualquierNivelPorNombre(nombreVerdaderaPropiedad, pTipoEntidad, propiedad.UnicoValor.Value);
                                if (propAux != null)
                                {
                                    return propAux;
                                }
                            }
                            else if (!propiedad.FunctionalProperty && propiedad.ListaValores.Count > 0)
                            {
                                //Devolvemos la 1� entidad ya que solo nos intersa el nombre  de la propiedad que tenga �sta:
                                foreach (ElementoOntologia entidad in propiedad.ListaValores.Values)
                                {
                                    Propiedad propAux = ObtenerPropiedadACualquierNivelPorNombre(nombreVerdaderaPropiedad, pTipoEntidad, entidad);
                                    if (propAux != null)
                                    {
                                        return propAux;
                                    }
                                    break;
                                }
                            }
                            else
                            {
                                ElementoOntologia entAux = propiedad.Ontologia.GetEntidadTipo(propiedad.Rango, false);
                                Propiedad propAux = ObtenerPropiedadACualquierNivelPorNombre(nombreVerdaderaPropiedad, pTipoEntidad, entAux);
                                if (propAux != null)
                                {
                                    return propAux;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Obtiene todas las instancias de las propiedades que existan con un nombre y un tipo de entidad partidendo de unas entidades Ra�z.
        /// </summary>
        /// <param name="pNombre">Nombre de la propiedad</param>
        /// <param name="pTipoEntidad">Tipo de la entiad que contiene la propiedad</param>
        /// <param name="pEntidades">Entidades de partida</param>
        /// <returns>Todas las instancias de las propiedades que existan con un nombre y un tipo de entidad partidendo de unas entidades Ra�z</returns>
        public static List<Propiedad> ObtenerTodasInstanciasPropiedadACualquierNivelPorNombre(string pNombre, string pTipoEntidad, List<ElementoOntologia> pEntidades)
        {
            List<Propiedad> valores = new List<Propiedad>();

            foreach (ElementoOntologia entidad in pEntidades)
            {
                List<Propiedad> valoresInt = ObtenerTodasInstanciasPropiedadACualquierNivelPorNombre(pNombre, pTipoEntidad, entidad);
                valores.AddRange(valoresInt);
            }

            return valores;
        }

        /// <summary>
        /// Obtiene los valores de cada una de las propiedades que existan con un nombre y un tipo de entidad partidendo de una entidad Ra�z.
        /// </summary>
        /// <param name="pNombre">Nombre de la propiedad</param>
        /// <param name="pTipoEntidad">Tipo de la entiad que contiene la propiedad</param>
        /// <param name="pEntidad">Entidad de partida</param>
        /// <returns>Todos los valores de cada una de las propiedades que existan con un nombre y un tipo de entidad partidendo de una entidad Ra�z</returns>
        public static List<Propiedad> ObtenerTodasInstanciasPropiedadACualquierNivelPorNombre(string pNombre, string pTipoEntidad, ElementoOntologia pEntidad)
        {
            List<Propiedad> valores = new List<Propiedad>();

            if (pEntidad != null)
            {
                foreach (Propiedad propiedad in pEntidad.Propiedades)
                {
                    if (propiedad.Nombre == pNombre && EsPropiedadDeTipoEntidad(propiedad, pEntidad, pTipoEntidad))
                    {
                        if (!valores.Contains(propiedad))
                        {
                            valores.Add(propiedad);
                        }
                    }

                    if (propiedad.Tipo == TipoPropiedad.ObjectProperty)
                    {
                        foreach (ElementoOntologia entidad in propiedad.ValoresUnificados.Values)
                        {
                            List<Propiedad> valoresAux = ObtenerTodasInstanciasPropiedadACualquierNivelPorNombre(pNombre, pTipoEntidad, entidad);
                            valores.AddRange(valoresAux);
                        }
                    }
                }
            }

            return valores;
        }

        /// <summary>
        /// Comprueba si una propiedad es de una entidad de determinado tipo.
        /// </summary>
        /// <param name="pPropiedad">Propiedad</param>
        /// <param name="pEntidad">Entidad a la que pertenece la propiedad o NULL si se desea revisar el 'ElementoOntologia' de la propiedad</param>
        /// <param name="pTipoEntidad">Tipo de entidad o NULL (devolver� TRUE)</param>
        /// <returns>TRUE si una propiedad es de una entidad de determinado tipo, FALSE en caso contrario</returns>
        public static bool EsPropiedadDeTipoEntidad(Propiedad pPropiedad, ElementoOntologia pEntidad, string pTipoEntidad)
        {
            if (pTipoEntidad == null)
            {
                return true;
            }
            else if (pEntidad != null)
            {
                return (pEntidad.TipoEntidad == pTipoEntidad || pEntidad.TipoEntidad.Contains($"{pTipoEntidad}_bis") || pEntidad.SuperclasesUtiles.Contains(pTipoEntidad));
            }
            else if (pPropiedad.ElementoOntologia != null)
            {
                return (pPropiedad.ElementoOntologia.TipoEntidad == pTipoEntidad || pPropiedad.ElementoOntologia.TipoEntidad.Contains($"{pTipoEntidad}_bis") || pPropiedad.ElementoOntologia.SuperclasesUtiles.Contains(pTipoEntidad));
            }
            else
            {
                foreach (string dominio in pPropiedad.Dominio)
                {
                    if (dominio == pTipoEntidad || dominio.Contains($"{pTipoEntidad}_bis"))
                    {
                        return true;
                    }
                    else
                    {
                        ElementoOntologia entAux = pPropiedad.Ontologia.GetEntidadTipo(dominio, false);

                        if (entAux != null && entAux.SuperclasesUtiles.Contains(pTipoEntidad))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Obtiene el ID del grupo gnoss vinculado al form sem.
        /// </summary>
        /// <param name="pGrupo">Url del grupo</param>
        /// <returns>ID del grupo gnoss vinculado al form sem</returns>
        public static Guid IDGrupoGnoss(string pGrupo)
        {
            return Guid.Parse(pGrupo.Substring(pGrupo.IndexOf("g_") + 2));
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si est� disposed
        /// </summary>
        protected bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~EstiloPlantilla()
        {
            //Libero los recursos
            Dispose(false,false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        public void Dispose()
        {
            Dispose(true, false);

            //impido que se finalice dos veces este objeto
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se est� llamando desde el Dispose()</param>
        /// <param name="pEliminarListasComunes">Indica si hay que eliminar listas comunes entre elementos ontolog�as copiados</param>
        protected virtual void Dispose(bool disposing, bool pEliminarListasComunes)
        {
        }

        #endregion
    }

    /// <summary>
    /// Representa una entidad externa editable a un recurso.
    /// </summary>

    [Serializable]
    public class EntidadExtEditableDoc
    {
        /// <summary>
        /// ID de la entidad externa
        /// </summary>
        public string EntidadID { get; set; }

        /// <summary>
        /// ID del documento que contiene o contendr� la entidad externa.
        /// </summary>
        public Guid DocumentoID { get; set; }

        /// <summary>
        /// Propiedad que vincula con la entidad externa.
        /// </summary>
        public string Propiedad { get; set; }

        /// <summary>
        /// Tipo de entidad de la propiedad que vincula con la entidad externa.
        /// </summary>
        public string TipoEntidad { get; set; }

        /// <summary>
        /// Orden de la entidad en la lista de valores de la propiedad que vincula con la entidad externa.
        /// </summary>
        public int NumValorPropiedad { get; set; }

        /// <summary>
        /// Indica si el documento es nuevo.
        /// </summary>
        public bool NuevoDoc { get; set; }

        /// <summary>
        /// Indica si el recurso est� eliminado.
        /// </summary>
        public bool Eliminado { get; set; }
    }
}
