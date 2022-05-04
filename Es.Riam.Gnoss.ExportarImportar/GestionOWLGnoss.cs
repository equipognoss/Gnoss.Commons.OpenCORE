using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ListaResultados;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.ExportarImportar.ElementosOntologia;
using Es.Riam.Interfaces;
using Es.Riam.Metagnoss.ExportarImportar;
using Es.Riam.Semantica.OWL;
using System;
using System.Linq;

namespace Es.Riam.Gnoss.ExportarImportar
{
    /// <summary>
    /// Clase que gestiona todo lo relacionado con la lectura y escritura de un OWL de Gnoss.
    /// </summary>
    public class GestionOWLGnoss : GestionOWL
    {
        #region Miembros

        /// <summary>
        /// Generador de URLs
        /// </summary>
        private static IGeneradorURL mGeneradorUrls;

        private EntityContext mEntityContext;

        #endregion

        public GestionOWLGnoss(EntityContext entityContext)
        {
            mEntityContext = entityContext;
        }

        #region Propiedades

        /// <summary>
        /// Obtiene la URL de la intranet de GNOSS
        /// </summary>
        protected string URLIntragnoss
        {
            get
            {
                if (mURLIntragnoss == null)
                {
                    ParametroAplicacion filaParametro = mEntityContext.ParametroAplicacion.First(parametro => parametro.Parametro.Equals("UrlIntragnoss"));

                    mURLIntragnoss = filaParametro.Valor;
                }
                return mURLIntragnoss;
            }
            set
            {
                mURLIntragnoss = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el generador de URLs
        /// </summary>
        public static IGeneradorURL GeneradorUrls
        {
            get
            {
                return mGeneradorUrls;
            }
            set
            {
                mGeneradorUrls = value;
            }
        }

        #endregion

        #region Métodos generales

        #region Pasar a OWL

        /// <summary>
        /// Obtiene la url base para un tipo de entidad
        /// </summary>
        /// <param name="pTipoEntidad">Tipo de entidad</param>
        /// <param name="pEntidad">Elemento de ontología de GNOSS</param>
        /// <returns>URL de la entidad</returns>
        public string ObtenerUrlEntidad(string pTipoEntidad, ElementoOntologiaGnoss pEntidad)
        {
            return ObtenerUrlEntidad(pTipoEntidad, pEntidad.Elemento, pEntidad.ID);
        }

        /// <summary>
        /// Obtiene la url base para un tipo de entidad
        /// </summary>
        /// <param name="pTipoEntidad">Tipo de entidad</param>
        /// <param name="pElemento">Elemento de la entidad GNOSS</param>
        /// <param name="pIDRelativo">ID relativo de la entidad</param>
        /// <returns>URL de la entidad</returns>
        public static string ObtenerUrlEntidad(string pTipoEntidad, IElementoGnoss pElemento, string pIDRelativo)
        {
            string url = GestionOWL.URLIntragnoss;
            bool usaGenerador = false;

            switch (pTipoEntidad)
            {
                case TipoElementoGnoss.Persona:
                    if (GeneradorUrls != null)
                    {
                        usaGenerador = true;
                        url = GeneradorUrls.ObtenerUrlPersona((Persona)pElemento);
                    }
                    else
                    {
                        url += "/perfil.aspx?PersonaID=";//falta OrgID
                    }
                    break;
                case TipoElementoGnoss.Documento:
                case TipoElementoGnoss.Pregunta:
                case TipoElementoGnoss.Debate:
                    if (GeneradorUrls != null && pElemento is Documento)
                    {
                        usaGenerador = true;

                        if (pIDRelativo.Equals(Guid.Empty.ToString()))
                        {
                            //Se está buscando la url de descarga
                            url = GeneradorUrls.ObtenerUrlDescargaRecurso((Documento)pElemento);
                        }
                        else
                        {
                            //Se está buscando la url de la ficha del recurso
                            url = GeneradorUrls.ObtenerUrlRecurso((Documento)pElemento);
                        }
                    }
                    else
                    {
                        url += "/baseRecursos.aspx?documentoID=";//falta
                    }
                    break;
                case TipoElementoGnoss.CategoriasTesauro:
                case TipoElementoGnoss.CategoriasTesauroSkos:
                    if (GeneradorUrls != null && pElemento is CategoriaTesauro)
                    {
                        usaGenerador = true;
                        url = GeneradorUrls.ObtenerUrlCategoriaTesauro((CategoriaTesauro)pElemento);
                    }
                    else
                    {
                        url += "/FichaCategoriaTesauroWiki.aspx?CategoriaID=";
                    }
                    break;
                case TipoElementoGnoss.Comentario:
                case TipoElementoGnoss.ComentarioSioc:
                    if (GeneradorUrls != null)
                    {
                        usaGenerador = true;
                        url = GeneradorUrls.ObtenerUrlComentario(new Guid(pIDRelativo));
                    }
                    else
                    {
                        url += "/VerDafo.aspx?ComentarioID=";//falta
                    }
                    break;
                case TipoElementoGnoss.TagSioc:
                    if (GeneradorUrls != null)
                    {
                        usaGenerador = true;
                        url = GeneradorUrls.ObtenerUrlTag(pIDRelativo);
                    }
                    else
                    {
                        url += "/tag/" + pIDRelativo;
                    }
                    break;
                case TipoElementoGnoss.Comunidad:
                case TipoElementoGnoss.ComunidadSioc:
                    if (GeneradorUrls != null)
                    {
                        usaGenerador = true;
                        url = GeneradorUrls.ObtenerUrlComunidad(((Proyecto)pElemento).FilaProyecto.NombreCorto);
                    }
                    else
                    {
                        url += "/comunidades/";
                    }
                    break;
                case TipoElementoGnoss.PerfilPersona:
                case TipoElementoGnoss.PerfilPersonaFoaf:
                case TipoElementoGnoss.USER_ACCOUNT:
                case TipoElementoGnoss.PerfilPersonaOrg:
                    if (GeneradorUrls != null)
                    {
                        usaGenerador = true;
                        url = GeneradorUrls.ObtenerUrlIdentidad((Identidad)pElemento);

                        if (pTipoEntidad.Equals(TipoElementoGnoss.USER_ACCOUNT))
                        {
                            url += "user";
                        }
                    }
                    else
                    {
                        url += "/perfil/";
                    }
                    break;
                case TipoElementoGnoss.PerfilOrganizacion:
                case TipoElementoGnoss.PerfilOrganizacionFoaf:
                    if (GeneradorUrls != null)
                    {
                        usaGenerador = true;
                        url = GeneradorUrls.ObtenerUrlIdentidad((Identidad)pElemento);
                    }
                    else
                    {
                        url += "/organizacion/";
                    }
                    break;
                case TipoElementoGnoss.ListaResultados:
                    if (GeneradorUrls != null)
                    {
                        usaGenerador = true;
                        url = GeneradorUrls.UrlActualSinQuery();
                    }
                    break;
                case TipoElementoGnoss.Filtro:
                    if (GeneradorUrls != null)
                    {
                        usaGenerador = true;
                        url = GeneradorUrls.UrlActual() + "#FiltroBusqueda_" + ((Filtro)pElemento).NombreCorto;
                    }
                    break;
                default:
                    url += "/items/";
                    break;
            }

            if (!usaGenerador)
            {
                url = GestionOWL.URLIntragnoss + pIDRelativo;
            }
            return url;
        }

        #endregion

        #region Leer OWL

        ///// <summary>//TODO: JAVIER BORRAR, sino traer LeerFicheroGnoss
        ///// Busca en la lista de entidades extraída del fichero todas las entidades relacionadas con la entidad dada
        ///// </summary>
        ///// <param name="pEntidad">Entidad principal</param>
        ///// <param name="pListaEntidades">Lista de entidades que contiene las entidades relacionadas.</param>
        //private static void BuscarEntidadesRelacionadas(Es.Riam.Semantica.OWL.ElementoOntologia pEntidad, List<Es.Riam.Semantica.OWL.ElementoOntologia> pListaEntidades)
        //{
        //    List<Propiedad> entidades = pEntidad.ObtenerEntidadesRelacionadas();
        //    ElementoOntologia entidad = null;

        //    foreach (Propiedad propiedad in entidades)
        //    {
        //        propiedad.ElementoOntologia = pEntidad;
        //        if (!propiedad.FunctionalProperty)
        //        {
        //            string[] claves = new string[propiedad.ListaValores.Keys.Count];
        //            propiedad.ListaValores.Keys.CopyTo(claves, 0);
        //            foreach (string valor in claves)
        //            {
        //                //Obtengo la entidad
        //                entidad = UtilImportarExportar.ObtenerEntidadPorID(valor, pListaEntidades);
        //                UtilImportarExportar.ObtenerNombreRealPropiedad(pEntidad, entidad, propiedad);
        //                propiedad.ListaValores[valor] = entidad;
        //                BuscarEntidadRelacionada(pEntidad, entidad, propiedad);
        //            }
        //        }
        //        else
        //        {
        //            entidad = UtilImportarExportar.ObtenerEntidadPorID(propiedad.UnicoValor.Key, pListaEntidades);
        //            UtilImportarExportar.ObtenerNombreRealPropiedad(pEntidad, entidad, propiedad);
        //            propiedad.UnicoValor = new KeyValuePair<string, ElementoOntologia>(propiedad.UnicoValor.Key, entidad);
        //            BuscarEntidadRelacionada(pEntidad, entidad, propiedad);
        //        }
        //    }
        //}

        ///// <summary>//TODO: JAVIER -> BORRAR
        ///// Busca una entidad relacionada
        ///// </summary>
        ///// <param name="pEntidadPadre">Entidad padre</param>
        ///// <param name="pEntidadRelacionada">Entidad relacionada</param>
        ///// <param name="pPropiedad">Propiedad</param>
        //private static void BuscarEntidadRelacionada(Es.Riam.Semantica.OWL.ElementoOntologia pEntidadPadre, Es.Riam.Semantica.OWL.ElementoOntologia pEntidadRelacionada, Propiedad pPropiedad)
        //{
        //    if (pEntidadRelacionada != null)
        //    {
        //        //La asigno a la principal
        //        pEntidadPadre.EntidadesRelacionadas.Add(pEntidadRelacionada);
        //        if (pPropiedad.PropiedadInversa != null)
        //        {
        //            //Registro la propiedad en ambas entidades como opuesta una de la otra.
        //            Propiedad propiedadInversa = UtilImportarExportar.ObtenerPropiedadDeNombre(pPropiedad.PropiedadInversa.Nombre, pEntidadRelacionada.Propiedades);
        //            if ((propiedadInversa != null) && (!pPropiedad.ListaPropiedadesInversas.Contains(propiedadInversa)))
        //            {
        //                pPropiedad.ListaPropiedadesInversas.Add(propiedadInversa);
        //                propiedadInversa.ListaPropiedadesInversas.Add(pPropiedad);
        //            }
        //        }
        //    }
        //    if ((pEntidadRelacionada != null) && (pEntidadRelacionada.TipoEntidad.Equals(TipoElementoGnoss.TipoGrupo)))
        //    {
        //        if (pEntidadPadre.TipoEntidad.Equals(TipoElementoGnoss.Objetivo))
        //        {
        //            pEntidadRelacionada.TipoEntidad = TipoElementoGnoss.Perspectiva;
        //        }
        //        else if (pEntidadPadre.TipoEntidad.Equals(TipoElementoGnoss.Proceso))
        //        {
        //            pEntidadRelacionada.TipoEntidad = TipoElementoGnoss.TipoProceso;
        //        }
        //        else if (pEntidadPadre.TipoEntidad.Equals(TipoElementoGnoss.GrupoFuncional))
        //        {
        //            pEntidadRelacionada.TipoEntidad = TipoElementoGnoss.TipoGrupoFuncional;
        //        }
        //    }
        //    else if ((pEntidadPadre.TipoEntidad.Equals(TipoElementoGnoss.Ocupacion)) && (pEntidadRelacionada != null) && (pEntidadRelacionada.TipoEntidad.Equals(TipoElementoGnoss.Persona)))
        //    {
        //        pEntidadRelacionada.Descripcion = pEntidadPadre.Descripcion;
        //    }
        //    if ((pEntidadRelacionada != null) && ((pEntidadRelacionada.TipoEntidad.Equals(TipoElementoGnoss.CategoriaDocumentacion)) || (pEntidadRelacionada.TipoEntidad.Equals(TipoElementoGnoss.MetaEstructura)) || (pEntidadRelacionada.TipoEntidad.Equals(TipoElementoGnoss.Libro)) || (pEntidadRelacionada.TipoEntidad.Equals(TipoElementoGnoss.ConjuntoObjetivos)) || (pEntidadRelacionada.TipoEntidad.Equals(TipoElementoGnoss.MiniLibro)) || (pEntidadRelacionada.TipoEntidad.Equals(TipoElementoGnoss.Organizacion))))
        //    {
        //        pEntidadRelacionada.PermitePadre = false;
        //    }

        //    if ((pPropiedad.Nombre.Equals(UtilImportarExportar.PROPIEDAD_ELEMENTO_IDEF0_REFERENCIADO)) && (pEntidadRelacionada != null))
        //    {
        //        pEntidadPadre.Descripcion = pEntidadRelacionada.Descripcion;
        //    }
        //}

        #endregion

        /// <summary>
        /// Crea un elemento de la ontología que representa la entidad pasada por parámetro
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <returns>Elemento de ontología</returns>
        public override ElementoOntologia CrearElementoOntologia(ElementoOntologia pEntidad)
        {
            return new ElementoOntologiaGnoss(pEntidad);
        }

        /// <summary>
        /// Crea un elemento de la ontología que representa el tipo de entidad pasado por parámetro
        /// </summary>
        /// <param name="pTipoEntidad">Tipo de entidad</param>
        /// <param name="pUrlOntologia">URL de la ontología</param>
        /// <param name="pNamespaceOntologia">Namespace de la ontología</param>
        /// <returns>Elemento de ontología</returns>
        public override ElementoOntologia CrearElementoOntologia(string pTipoEntidad, string pUrlOntologia, string pNamespaceOntologia)
        {
            return new ElementoOntologiaGnoss(pTipoEntidad, pUrlOntologia, pNamespaceOntologia, Ontologia);
        }

        /// <summary>
        /// Crea una nueva propiedad a partir del nombre y tipo pasados por parámetro
        /// </summary>
        /// <param name="pNombrePropiedad">Nombre de la propiedad que se va a crear</param>
        /// <param name="pTipoPropiedad">Tipo de la propiedad que se va a crear</param>
        /// <returns>Nueva propiedad</returns>
        public override Propiedad CrearPropiedad(string pNombrePropiedad, TipoPropiedad pTipoPropiedad)
        {
            return new PropiedadGnoss(pNombrePropiedad, pTipoPropiedad, Ontologia);
        }

        /// <summary>
        /// Crea una nueva propiedad a partir de otra pasada por parámetro
        /// </summary>
        /// <param name="pPropiedad">Propiedad original</param>
        /// <returns>Nueva propiedad</returns>
        public override Propiedad CrearPropiedad(Propiedad pPropiedad)
        {
            return new PropiedadGnoss(pPropiedad);
        }

        #endregion

        /// <summary>
        /// Busca la relación entre dos entidades
        /// </summary>
        /// <param name="pEntidadPadre">Entidad padre</param>
        /// <param name="pEntidadRelacionada">Entidad relacionada</param>
        /// <param name="pPropiedad">Propiedad</param>
        protected override void BuscarEntidadRelacionada(Es.Riam.Semantica.OWL.ElementoOntologia pEntidadPadre, Es.Riam.Semantica.OWL.ElementoOntologia pEntidadRelacionada, Propiedad pPropiedad)
        {
            if (pEntidadRelacionada != null)
            {
                //La asigno a la principal
                pEntidadPadre.EntidadesRelacionadas.Add(pEntidadRelacionada);
                if (pPropiedad.PropiedadInversa != null)
                {
                    //Registro la propiedad en ambas entidades como opuesta una de la otra.
                    Propiedad propiedadInversa = UtilImportarExportar.ObtenerPropiedadDeNombre(pPropiedad.PropiedadInversa.Nombre, pEntidadRelacionada.Propiedades);
                    if ((propiedadInversa != null) && (!pPropiedad.ListaPropiedadesInversas.Contains(propiedadInversa)))
                    {
                        pPropiedad.ListaPropiedadesInversas.Add(propiedadInversa);
                        propiedadInversa.ListaPropiedadesInversas.Add(pPropiedad);
                    }
                }
            }
            if (pEntidadRelacionada != null && pEntidadRelacionada.TipoEntidad.Equals(TipoElementoGnoss.Persona))
                pEntidadPadre.Descripcion = UtilImportarExportar.ObtenerPropiedadDeNombre("Nombre", pEntidadRelacionada.Propiedades).UnicoValor.Key + " " + UtilImportarExportar.ObtenerPropiedadDeNombre("Apellidos", pEntidadRelacionada.Propiedades).UnicoValor.Key;
            if (pEntidadRelacionada != null && (pEntidadRelacionada.TipoEntidad.Equals(TipoElementoGnoss.CategoriaDocumentacion) || pEntidadRelacionada.TipoEntidad.Equals(TipoElementoGnoss.Organizacion)))
                pEntidadRelacionada.PermitePadre = false;
        }
    }
}
