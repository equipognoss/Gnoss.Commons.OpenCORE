using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Facetado;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.ExportarImportar;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.Controles.GeneradorPlantillasOWL;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Metagnoss.ExportarImportar;
using Es.Riam.Semantica.OWL;
using Es.Riam.Semantica.Plantillas;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace Es.Riam.Gnoss.Web.MVC.Controles.Controladores
{
    /// <summary>
    /// Controlador para el SEMCMS.
    /// </summary>
    public class SemCmsController : ControladorBase
    {
        #region Miembros

        /// <summary>
        /// Modelo del SEM CMS.
        /// </summary>
        private SemanticResourceModel mSemRecModel;

        /// <summary>
        /// Ontología a la que pertenecerá el recurso.
        /// </summary>
        private Ontologia mOntologia;

        /// <summary>
        /// ID del recurso
        /// </summary>
        private Guid mDocumentoID;

        /// <summary>
        /// Entidades semánticas raíz del recurso
        /// </summary>
        private List<ElementoOntologia> mEntidades;

        /// <summary>
        /// Tipo de las entidades apañandas, apaño - verdadero valor.
        /// </summary>
        private Dictionary<string, string> mTipoEntidadesApanyadas;

        /// <summary>
        /// Documento asociado al formulario semántico.
        /// </summary>
        private Documento mDocumento;

        /// <summary>
        /// Proyecto actual.
        /// </summary>
        private Proyecto mProyectoActual;

        /// <summary>
        /// Identidad actual conectada.
        /// </summary>
        private Identidad mIdentidadActual;

        /// <summary>
        /// ID de los grupos ocultos.
        /// </summary>
        private List<string> mIDGruposOcultos;

        /// <summary>
        /// Idioma por defecto si hay multiIdioma.
        /// </summary>
        private string mIdiomaDefecto;

        /// <summary>
        /// Lista de idiomas de la forma (es, Español)
        /// </summary>
        private Dictionary<string, string> mIdiomasDisponibles;

        /// <summary>
        /// Util Idiomas.
        /// </summary>
        private UtilIdiomas mUtilIdiomas;

        /// <summary>
        /// Base Url.
        /// </summary>
        private string mBaseUrl;

        /// <summary>
        /// Url del Content
        /// </summary>
        private string mBaseURLContent;

        /// <summary>
        /// Url Static.
        /// </summary>
        private string mBaseURLStatic;

        /// <summary>
        /// Datos de entidades grafo dependientes.
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> mDatosEntidadesGrafoDep;

        /// <summary>
        /// Url de intragnoss.
        /// </summary>
        private string mUrlIntragnoss;

        /// <summary>
        /// Base Url con Idioma
        /// </summary>
        private string mBaseURLIdioma;

        /// <summary>
        /// Entidades que son superclase y sus hijas.
        /// </summary>
        private Dictionary<ElementoOntologia, List<ElementoOntologia>> mEntidadesSuperClassEHijas = new Dictionary<ElementoOntologia, List<ElementoOntologia>>();

        /// <summary>
        /// Entidades que son superclase y su tipo de dominio funcional.
        /// </summary>
        private Dictionary<ElementoOntologia, int> mEntidadesSuperClassTipoDominioFunc = new Dictionary<ElementoOntologia, int>();

        /// <summary>
        /// Entidades que se van a pintar en el SEM CMS.
        /// </summary>
        private List<ElementoOntologia> mEntidadesSeVanAPintar = new List<ElementoOntologia>();

        /// <summary>
        /// Lista con los valores que hay que insertar en un determinado grafo para autocompletar.
        /// </summary>
        private Dictionary<string, List<string>> mValoresGrafoAutocompletar;

        /// <summary>
        /// ID y tipo de entidad de la propiedad cuyo valor está condicionado por otra propiedad y entidad.
        /// </summary>
        private Dictionary<KeyValuePair<string, string>, KeyValuePair<string, string>> mPropsEntGrafoDependientes;

        /// <summary>
        /// Datos de entidades externas.
        /// </summary>
        private Dictionary<KeyValuePair<string, string>, object[]> mDatosEntidadesExternas;

        /// <summary>
        /// Lista con los nombre de las propiedades y tipos de entidad de dataSet de selectores ya fusionados.
        /// </summary>
        private List<string> mDataSetsEntidadesExternasMerged;

        /// <summary>
        /// Indica si la vista que se va usar para el controlador de documento es personalizada.
        /// </summary>
        private bool mVistaPersonalizada;

        /// <summary>
        /// ID del campo del formulario semántico que representa el título del recurso.
        /// </summary>
        private string mTxtTituloDocSemID;

        /// <summary>
        /// ID del campo del formulario semántico que representa la descripción del recurso.
        /// </summary>
        private string mTxtDescripcionDocSemID;

        /// <summary>
        /// Indica si se está editando un recurso pre-creado de la carga masiva.
        /// </summary>
        private bool mEditandoRecursoCargaMasiva;

        /// <summary>
        /// Url de la propiedad que debe contener al menos un idioma para realizar la busqueda por el mismo.
        /// </summary>
        private string mPropiedadIdiomaBusquedaComunidad;

        /// <summary>
        /// Propiedad, ID de entidad y Número de inicio de página de la propiedad sobre la que se está haciendo callback para obtener más datos.
        /// </summary>
        private KeyValuePair<string, KeyValuePair<string, int>> mPropiedadCallbackTraerMas;

        /// <summary>
        /// Lista con las propiedades de la ontología configuradas como selector de personas.
        /// </summary>
        private List<Propiedad> mPropsSelectorPersonas;

        /// <summary>
        /// Urls de los recursos semánticos vinculados a través de un selector de entidad de tipo "UrlRecursoSemantico" y su entidad principal vinculada.
        /// </summary>
        private Dictionary<string, string> mUrlRecursoSemEntidadPrincipal;

        /// <summary>
        /// Propiedades que no deben pintarse.
        /// </summary>
        private Dictionary<string, List<string>> mPropiedadesOmitirPintado;


        private EntityContextBASE mEntityContextBASE;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor del controlador de SEM CMS.
        /// </summary>
        /// <param name="pSemRecModel">Modelo del SEM CMS</param>
        /// <param name="pOntologia">Ontología a la que pertenecerá el recurso</param>
        /// <param name="pDocumentoID">ID del recurso</param>
        /// <param name="pEntidades">Entidades semánticas raíz del recurso</param>
        /// <param name="pProyectoActual">Proyecto actual</param>
        /// <param name="pIdentidadActual">Identidad actual conectada</param>
        /// <param name="pUtilIdiomas">Util idiomas</param>
        /// <param name="pBaseUrl">Base Url</param>
        /// <param name="pBaseURLIdioma">Base Url con Idioma</param>
        /// <param name="pBaseURLContent">Url del Content</param>
        /// <param name="pBaseURLStatic">Url Static</param>
        /// <param name="pUrlIntragnoss">Url de intragnoss</param>
        public SemCmsController(SemanticResourceModel pSemRecModel, Ontologia pOntologia, Guid pDocumentoID, List<ElementoOntologia> pEntidades, Proyecto pProyectoActual, Identidad pIdentidadActual, UtilIdiomas pUtilIdiomas, string pBaseUrl, string pBaseURLIdioma, string pBaseURLContent, string pBaseURLStatic, string pUrlIntragnoss, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IHttpContextAccessor httpContextAccessor, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, EntityContextBASE entityContextBASE, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication)
        {
            GestionOWL.URLIntragnoss = pUrlIntragnoss;
            mSemRecModel = pSemRecModel;
            mOntologia = pOntologia;
            mDocumentoID = pDocumentoID;
            mEntidades = pEntidades;
            mProyectoActual = pProyectoActual;
            mIdentidadActual = pIdentidadActual;
            mUtilIdiomas = pUtilIdiomas;
            mBaseUrl = pBaseUrl;
            mBaseURLIdioma = pBaseURLIdioma;
            mBaseURLContent = pBaseURLContent;
            mBaseURLStatic = pBaseURLStatic;
            mUrlIntragnoss = pUrlIntragnoss;
            mEntityContextBASE = entityContextBASE;
        }

        /// <summary>
        /// Constructor del controlador de SEM CMS.
        /// </summary>
        /// <param name="pSemRecModel">Modelo del SEM CMS</param>
        /// <param name="pOntologia">Ontología a la que pertenecerá el recurso</param>
        /// <param name="pDocumentoID">ID del recurso</param>
        /// <param name="pEntidades">Entidades semánticas raíz del recurso</param>
        /// <param name="pDocumento">Documento asociado al formulario semántico</param>
        /// <param name="pProyectoActual">Proyecto actual</param>
        /// <param name="pIdentidadActual">Identidad actual conectada</param>
        /// <param name="pUtilIdiomas">Util idiomas</param>
        /// <param name="pBaseUrl">Base Url</param>
        /// <param name="pBaseURLIdioma">Base Url con Idioma</param>
        /// <param name="pBaseURLContent">Url del Content</param>
        /// <param name="pBaseURLStatic">Url Static</param>
        /// <param name="pUrlIntragnoss">Url de intragnoss</param>
        public SemCmsController(SemanticResourceModel pSemRecModel, Ontologia pOntologia, Documento pDocumento, List<ElementoOntologia> pEntidades, Proyecto pProyectoActual, Identidad pIdentidadActual, UtilIdiomas pUtilIdiomas, string pBaseUrl, string pBaseURLIdioma, string pBaseURLContent, string pBaseURLStatic, string pUrlIntragnoss, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IHttpContextAccessor httpContextAccessor, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, EntityContextBASE entityContextBASE, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : this(pSemRecModel, pOntologia, pDocumento.Clave, pEntidades, pProyectoActual, pIdentidadActual, pUtilIdiomas, pBaseUrl, pBaseURLIdioma, pBaseURLContent, pBaseURLStatic, pUrlIntragnoss, loggingService, entityContext, configService, httpContextAccessor, redisCacheWrapper, gnossCache, virtuosoAD, entityContextBASE, servicesUtilVirtuosoAndReplication)
        {
            mDocumento = pDocumento;
        }

        #endregion

        #region Métodos

        #region Públicos

        /// <summary>
        /// Completa el modelo semántico del SEM CMS para editar un recurso semántico.
        /// </summary>
        public void ObtenerModeloSemCMSEdicion(Guid pIdentidadID)
        {
            mSemRecModel.SemCmsContainsTitleAndDescription = (mOntologia.ConfiguracionPlantilla.PropiedadTitulo.Key != null);
            if (mEntidades == null)
            {
                List<ElementoOntologia> entidades = mOntologia.ObtenerElementosContenedorSuperior();

                if (entidades.Count > 1)
                {
                    entidades = mOntologia.ConfiguracionPlantilla.ObtenerEntidadesPrincipalesOrdenadas(entidades);
                }

                mEntidades = new List<ElementoOntologia>();

                foreach (ElementoOntologia entidad in entidades)
                {
                    ElementoOntologia instanciaEntidad = mOntologia.GetEntidadTipo(entidad.TipoEntidad, true);
                    instanciaEntidad.ID = instanciaEntidad.TipoEntidad + "_" + mDocumentoID.ToString() + "_" + Guid.NewGuid();
                    mEntidades.Add(instanciaEntidad);
                }
            }
            ObtenerModeloSemCMS();

            ApanyarRepeticionEntidades();
            TraerNombresCategoriasSemyAutocompletar();
            AgregarEntidadesPrincipales(pIdentidadID);
            AgregarInfoExtraElementos();
        }

        /// <summary>
        /// Completa el modelo semántico del SEM CMS para ver un recurso semántico.
        /// </summary>
        public void ObtenerModeloSemCMSLectura(Guid pIdentidadID, bool pUsarAfinidad = false)
        {
            mSemRecModel.ReadMode = true;
            AgregarInfoNoMiembros(pIdentidadID);
            ObtenerModeloSemCMS();
            ObtenerEntidadesExternas(pUsarAfinidad);
            ObtenerTituloConfiguradoPagina();
            AgregarEntidadesPrincipales(pIdentidadID);
            EstablecerConfiguracionesPagina();
        }

        /// <summary>
        /// Ordena las entidades auxiliares configuradas para que se ordenen respecto al valor de una de sus propiedades.
        /// </summary>
        /// <param name="pEntidades">Entidades a revisar sus propiedades</param>
        private void OrdenarEntidadesAuxiliaresConCampoOrden()
        {
            if (mOntologia.ConfiguracionPlantilla.HayCampoOrden)
            {
                OrdenarEntidadesAuxiliaresConCampoOrden(mEntidades);
            }
        }

        /// <summary>
        /// Ordena las entidades auxiliares configuradas para que se ordenen respecto al valor de una de sus propiedades.
        /// </summary>
        /// <param name="pEntidades">Entidades a revisar sus propiedades</param>
        private void OrdenarEntidadesAuxiliaresConCampoOrden(List<ElementoOntologia> pEntidades)
        {
            foreach (ElementoOntologia entidad in pEntidades)
            {
                if (entidad.EntidadesRelacionadas.Count > 0)
                {
                    foreach (Propiedad prop in entidad.Propiedades)
                    {
                        if (prop.Tipo == TipoPropiedad.ObjectProperty)
                        {
                            if (prop.EntidadesHijasConOrden)
                            {
                                prop.ListaValores = prop.ListaValoresOrdCampoEntidad;
                            }
                        }
                    }
                }

                OrdenarEntidadesAuxiliaresConCampoOrden(entidad.EntidadesRelacionadas);
            }
        }

        /// <summary>
        /// Funsiona las ontologías de las entidades externas editables con la actual.
        /// </summary>
        /// <param name="pOntologia">Ontología actual</param>
        /// <param name="pProyectoID">ID del proyecto actual</param>
        /// <param name="pPropiedadesOmitir">Propiedad que se debe omitir su pintado</param>
        public void FusionarOntologiasYXMLEntExtEditables(Ontologia pOntologia, Guid pProyectoID, Dictionary<string, List<string>> pPropiedadesOmitir, Dictionary<string, string> pParametroProyecto)
        {
            List<ElementoOntologia> entidadesOnto = new List<ElementoOntologia>(pOntologia.Entidades);

            foreach (ElementoOntologia entidad in entidadesOnto)
            {
                foreach (Propiedad prop in entidad.Propiedades)
                {
                    prop.ElementoOntologia = entidad;

                    if (prop.EspecifPropiedad.SelectorEntidad != null && prop.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "Edicion")
                    {
                        if (string.IsNullOrEmpty(prop.EspecifPropiedad.SelectorEntidad.Grafo))
                        {
                            throw new Exception("El selector de la propiedad '" + prop.Nombre + "' de la entidad '" + entidad.TipoEntidad + "' no tiene grafo.");
                        }

                        if (pParametroProyecto.ContainsKey(ParametroAD.ProyectoIDPatronOntologias))
                        {
                            pProyectoID = new Guid(pParametroProyecto[ParametroAD.ProyectoIDPatronOntologias]);
                        }

                        DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        Guid ontologiaExtID = docCN.ObtenerOntologiaAPartirNombre(pProyectoID, prop.EspecifPropiedad.SelectorEntidad.Grafo);
                        docCN.Dispose();

                        if (ontologiaExtID == Guid.Empty)
                        {
                            throw new Exception("El grafo '" + prop.EspecifPropiedad.SelectorEntidad.Grafo + "' del selector de la propiedad '" + prop.Nombre + "' de la entidad '" + entidad.TipoEntidad + "' no es una ontología de la comunidad.");
                        }

                        try
                        {
                            Dictionary<string, List<EstiloPlantilla>> listaEstilos = null;
                            byte[] arrayOnto = new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication).ObtenerOntologia(ontologiaExtID, out listaEstilos, pProyectoID);

                            Ontologia ontologiaExt = new Ontologia(arrayOnto, true);
                            ontologiaExt.LeerOntologia();
                            ontologiaExt.EstilosPlantilla = listaEstilos;
                            ontologiaExt.IdiomaUsuario = pOntologia.IdiomaUsuario;
                            ontologiaExt.OntologiaID = ontologiaExtID;

                            if (pOntologia.OntologiasExternas == null)
                            {
                                pOntologia.OntologiasExternas = new Dictionary<string, Ontologia>();
                            }

                            if (!pOntologia.OntologiasExternas.ContainsKey(prop.EspecifPropiedad.SelectorEntidad.Grafo))
                            {
                                pOntologia.OntologiasExternas.Add(prop.EspecifPropiedad.SelectorEntidad.Grafo, ontologiaExt);
                            }

                            List<ElementoOntologia> entidades = ontologiaExt.ObtenerElementosContenedorSuperior();
                            prop.Rango = entidades[0].TipoEntidad;

                            foreach (ElementoOntologia entidadExt in ontologiaExt.Entidades)
                            {
                                pOntologia.Entidades.Add(entidadExt);
                                entidadExt.Ontologia = pOntologia;
                            }

                            foreach (string url in ontologiaExt.NamespacesDefinidos.Keys)
                            {
                                if (!pOntologia.NamespacesDefinidos.ContainsKey(url))
                                {
                                    pOntologia.NamespacesDefinidos.Add(url, ontologiaExt.NamespacesDefinidos[url]);
                                }
                            }

                            foreach (string names in ontologiaExt.NamespacesDefinidosInv.Keys)
                            {
                                if (!pOntologia.NamespacesDefinidosInv.ContainsKey(names))
                                {
                                    pOntologia.NamespacesDefinidosInv.Add(names, ontologiaExt.NamespacesDefinidosInv[names]);
                                }
                            }

                            foreach (string key in listaEstilos.Keys)
                            {
                                if (!pOntologia.EstilosPlantilla.ContainsKey(key))
                                {
                                    pOntologia.EstilosPlantilla.Add(key, new List<EstiloPlantilla>());
                                }

                                pOntologia.EstilosPlantilla[key].AddRange(listaEstilos[key]);
                            }

                            if (ontologiaExt.ConfiguracionPlantilla.HayFechaConHora)
                            {
                                pOntologia.ConfiguracionPlantilla.HayFechaConHora = true;
                            }

                            if (prop.EspecifPropiedad.SelectorEntidad.Reciproca && !string.IsNullOrEmpty(prop.EspecifPropiedad.SelectorEntidad.PropiedadEdicionReciproca) && !string.IsNullOrEmpty(prop.EspecifPropiedad.SelectorEntidad.EntidadEdicionReciproca))
                            {
                                if (!pPropiedadesOmitir.ContainsKey(prop.EspecifPropiedad.SelectorEntidad.PropiedadEdicionReciproca))
                                {
                                    pPropiedadesOmitir.Add(prop.EspecifPropiedad.SelectorEntidad.PropiedadEdicionReciproca, new List<string>());
                                }

                                pPropiedadesOmitir[prop.EspecifPropiedad.SelectorEntidad.PropiedadEdicionReciproca].Add(prop.EspecifPropiedad.SelectorEntidad.EntidadEdicionReciproca);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Error al traer y montar una ontología externa de la propiedad '" + prop.Nombre + "' de la entidad '" + entidad.TipoEntidad + "':" + ex.ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Funsiona las ontologías de las entidades externas editables con la actual.
        /// </summary>
        /// <param name="pEntidades">Entidades recurso editando</param>
        /// <param name="pOntologia">Ontologia</param>
        /// <param name="pProyectoID">ID del proyecto actual</param>
        /// <param name="pDocumentoID">ID del documento que se está editando</param>
        /// <param name="pBaseURLFormulariosSem">URL base para los formularios semánticos</param>
        /// <param name="pUrlIntragnoss">Url de intragnoss</param>
        /// <param name="pEntidadesExtEditablesDocID">IDs de entidades externas editables y su ID de documento que el corresponde</param>
        /// <param name="pFilaPersona">Fila de la persona conectada</param>
        /// <param name="pFilaProy">Fila del proyecto actual</param>
        /// <param name="pImageRepresentativeValue">Cadena de control para las imagenes representantes</param>
        public void FuncionarRDFsEntidadExternaEditable(List<ElementoOntologia> pEntidades, Ontologia pOntologia, Guid pProyectoID, Guid pDocumentoID, string pBaseURLFormulariosSem, string pUrlIntragnoss, out Dictionary<string, EntidadExtEditableDoc> pEntidadesExtEditablesDocID, AD.EntityModel.Models.PersonaDS.Persona pFilaPersona, Gnoss.AD.EntityModel.Models.ProyectoDS.Proyecto pFilaProy, ref string pImageRepresentativeValue)
        {
            pEntidadesExtEditablesDocID = new Dictionary<string, EntidadExtEditableDoc>();
            bool sinEntidades = false;

            if (pEntidades == null)
            {
                pEntidades = pOntologia.Entidades;
                sinEntidades = true;
            }

            foreach (ElementoOntologia entidad in pEntidades)
            {
                foreach (Propiedad prop in entidad.Propiedades)
                {
                    prop.ElementoOntologia = entidad;

                    if (prop.EspecifPropiedad.SelectorEntidad != null && prop.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "Edicion")
                    {
                        if (!sinEntidades)
                        {
                            if (prop.EspecifPropiedad.SelectorEntidad.Reciproca)
                            {
                                List<string> sujetosEntExtReci = new UtilSemCms(mEntityContext, mLoggingService, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).ObtenerSujetosEntiadesSelectorReciprocoDeEntidad(pUrlIntragnoss, prop.EspecifPropiedad.SelectorEntidad, pUrlIntragnoss + "items/" + entidad.ID, pFilaPersona, pFilaProy, pEntidades);

                                foreach (string sujetoEntExt in sujetosEntExtReci)
                                {
                                    prop.AgregarValor(sujetoEntExt);
                                }
                            }

                            if (prop.ValoresUnificados.Count > 0)
                            {
                                FuncionarRDFEntidadExternaEditable(prop, pProyectoID, pBaseURLFormulariosSem, entidad.Ontologia.OntologiasExternas[prop.EspecifPropiedad.SelectorEntidad.Grafo], pUrlIntragnoss, pDocumentoID, pEntidadesExtEditablesDocID, ref pImageRepresentativeValue);
                            }
                        }

                        EntidadExtEditableDoc entExtEditDoc = new EntidadExtEditableDoc();
                        entExtEditDoc.DocumentoID = Guid.NewGuid();
                        entExtEditDoc.NumValorPropiedad = -1;
                        entExtEditDoc.Propiedad = prop.Nombre;
                        entExtEditDoc.TipoEntidad = entidad.TipoEntidad;
                        entExtEditDoc.NuevoDoc = true;
                        pEntidadesExtEditablesDocID.Add(entExtEditDoc.DocumentoID.ToString(), entExtEditDoc);
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene el RDF del recurso de una entidad externa editable para añadir su contenido al recurso que se está editando.
        /// </summary>
        /// <param name="pPropiedad">Propiedad selector enterno editable</param>
        /// <param name="pProyectoID">ID del proyecto actual</param>
        /// <param name="pBaseURLFormulariosSem">URL base para los formularios semánticos</param>
        /// <param name="pUrlIntragnoss">Url de intragnoss</param>
        /// <param name="pEntidadesExtEditablesDocID">IDs de entidades externas editables y su ID de documento que el corresponde</param>
        /// <param name="pOntologia">Ontologia externa</param>
        /// <param name="pDocumentoID">ID del documento que se está editando</param>
        /// <param name="pImageRepresentativeValue">Cadena de control para las imagenes representantes</param>
        private void FuncionarRDFEntidadExternaEditable(Propiedad pPropiedad, Guid pProyectoID, string pBaseURLFormulariosSem, Ontologia pOntologia, string pUrlIntragnoss, Guid pDocumentoID, Dictionary<string, EntidadExtEditableDoc> pEntidadesExtEditablesDocID, ref string pImageRepresentativeValue)
        {
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            string nombreOntologia = docCN.ObtenerEnlaceDocumentoPorDocumentoID(pOntologia.OntologiaID);

            string urlOntologia = pBaseURLFormulariosSem + "/Ontologia/" + nombreOntologia + "#";
            int count = 0;

            foreach (string valor in new List<string>(pPropiedad.ValoresUnificados.Keys))
            {
                pPropiedad.LimpiarValor(valor);

                string docT = valor.Substring(0, valor.LastIndexOf("_"));
                docT = docT.Substring(docT.LastIndexOf("_") + 1);
                Guid documentoID = new Guid(docT);
                bool obtenidoVirtuoso = false;

                string rdfTexto = ObtenerRDFDocumento(documentoID, pProyectoID, GestionOWL.NAMESPACE_ONTO_GNOSS, urlOntologia, nombreOntologia, pOntologia, pUrlIntragnoss, true, out obtenidoVirtuoso);
                List<ElementoOntologia> entPrinc = ObtenerInstanciasPrincipalesDocumento(documentoID, pProyectoID, GestionOWL.NAMESPACE_ONTO_GNOSS, urlOntologia, nombreOntologia, pOntologia, pUrlIntragnoss, rdfTexto, true);
                ReempazarDocumentoIDEntidad(documentoID, pDocumentoID, entPrinc[0]);

                entPrinc[0].Ontologia = pPropiedad.Ontologia;//Cambiamos la ontología para que si hay estilos configurados en la actual, se conjan esos en vez de los de la orginal.
                pPropiedad.AgregarValor(entPrinc[0]);
                //pEntidadesExtEditablesDocID.Add(entPrinc[0].ID, documentoID);

                EntidadExtEditableDoc entExtEditDoc = new EntidadExtEditableDoc();
                entExtEditDoc.EntidadID = entPrinc[0].ID;
                entExtEditDoc.DocumentoID = documentoID;
                entExtEditDoc.NumValorPropiedad = count;
                entExtEditDoc.Propiedad = pPropiedad.Nombre;
                entExtEditDoc.TipoEntidad = pPropiedad.ElementoOntologia.TipoEntidad;
                entExtEditDoc.NuevoDoc = false;
                pEntidadesExtEditablesDocID.Add(entPrinc[0].ID, entExtEditDoc);

                string nombCatDoc = docCN.ObtenerNombreCategoriaDocDocumento(documentoID);

                if (!string.IsNullOrEmpty(nombCatDoc))
                {
                    nombCatDoc = nombCatDoc.Substring(nombCatDoc.LastIndexOf(",") + 1);

                    if (nombCatDoc.Contains("|"))
                    {
                        nombCatDoc = nombCatDoc.Split('|')[0];
                    }

                    nombCatDoc = documentoID + "," + nombCatDoc + "|";
                    pImageRepresentativeValue += nombCatDoc;
                }

                count++;
            }

            docCN.Dispose();
        }

        /// <summary>
        /// Reemplaza el ID de un documento del identificador de una entidad por otro.
        /// </summary>
        /// <param name="pDocID">ID a reemplazar</param>
        /// <param name="pNuevoDocID">ID nuevo</param>
        public static void ReempazarDocumentoIDEntidad(Guid pDocID, Guid pNuevoDocID, ElementoOntologia pEntiadad)
        {
            string docID = pDocID.ToString();
            string nuevoDocID = pNuevoDocID.ToString();

            if (pEntiadad.ID.Contains(docID))
            {
                pEntiadad.ID = pEntiadad.ID.Replace(docID, nuevoDocID);
            }

            foreach (Propiedad prop in pEntiadad.Propiedades)
            {
                prop.ElementoOntologia = pEntiadad;

                foreach (string valor in new List<string>(prop.ValoresUnificados.Keys))
                {
                    if (valor.Contains(docID))
                    {
                        if (prop.ValoresUnificados[valor] != null)
                        {
                            ElementoOntologia hijo = prop.ValoresUnificados[valor];
                            prop.LimpiarValor(hijo);
                            ReempazarDocumentoIDEntidad(pDocID, pNuevoDocID, hijo);
                            prop.AgregarValor(hijo);
                        }
                        else if (prop.EspecifPropiedad.TipoCampo != TipoCampoOntologia.Archivo && prop.EspecifPropiedad.TipoCampo != TipoCampoOntologia.ArchivoLink && prop.EspecifPropiedad.TipoCampo != TipoCampoOntologia.Imagen && prop.EspecifPropiedad.TipoCampo != TipoCampoOntologia.Video)
                        {
                            prop.LimpiarValor(valor);
                            prop.AgregarValor(valor.Replace(docID, nuevoDocID));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Apaña la repetición de propiedades para que se puedan editar.
        /// </summary>
        public static void ApanyarRepeticionPropiedades(EstiloPlantillaConfigGen pEstiloGen, List<ElementoOntologia> pEntidades)
        {
            if (pEstiloGen.PropsRepetidas != null)
            {
                foreach (KeyValuePair<string, string> propEnt in pEstiloGen.PropsRepetidas.Keys)
                {
                    foreach (ElementoOntologia elemPrinc in pEntidades)
                    {
                        List<Propiedad> propsparaAgregar = new List<Propiedad>();
                        foreach (Propiedad propiedad in elemPrinc.Propiedades)
                        {
                            if (propiedad.Nombre == propEnt.Key && (propEnt.Value == elemPrinc.TipoEntidad || elemPrinc.SuperclasesUtiles.Contains(propEnt.Value)))
                            {
                                foreach (string propiedadRepetir in pEstiloGen.PropsRepetidas[propEnt])
                                {
                                    Propiedad propiedadAux = new Propiedad(propiedadRepetir, propiedad.Tipo, propiedad.Dominio, propiedad.Rango, propiedad.FunctionalProperty, elemPrinc.Ontologia);
                                    propiedadAux.ListaValoresPermitidos = propiedad.ListaValoresPermitidos;
                                    propiedadAux.NombrePropiedadRepetidaDe = propiedad.Nombre;
                                    propsparaAgregar.Add(propiedadAux);

                                    if (elemPrinc.Ontologia.EstilosPlantilla.ContainsKey(elemPrinc.TipoEntidad) && ((EstiloPlantillaEspecifEntidad)elemPrinc.Ontologia.EstilosPlantilla[elemPrinc.TipoEntidad][0]).ElementosOrdenados.Count > 0)
                                    {
                                        ElementoOrdenado elemOrd = new ElementoOrdenado();
                                        elemOrd.NombrePropiedad = new KeyValuePair<string, Propiedad>(propiedadAux.Nombre, null);
                                        AgregarElementoOrdenadoAlturaProp(elemOrd, propiedad.Nombre, ((EstiloPlantillaEspecifEntidad)elemPrinc.Ontologia.EstilosPlantilla[elemPrinc.TipoEntidad][0]).ElementosOrdenados);
                                    }
                                }
                            }
                        }

                        if (propsparaAgregar.Count > 0)
                        {
                            elemPrinc.Propiedades.AddRange(propsparaAgregar);
                        }
                    }
                }
            }
        }

        #endregion

        #region Privados

        /// <summary>
        /// Agrega la información para usuarios no miembros.
        /// </summary>
        private void AgregarInfoNoMiembros(Guid pIdentidadID)
        {
            mSemRecModel.HideInfoIsNotMember = OcultarInfoNoEsMiembro(pIdentidadID);

            if (OcultarInfoNoEsMiembro(pIdentidadID))
            {
                mSemRecModel.TitleInfoIsNotMember = Ontologia.ConfiguracionPlantilla.ObtenerTituloSoloMiembros(Ontologia.IdiomaUsuario);

                if (mProyectoActual.TipoAcceso == TipoAcceso.Restringido)
                {
                    mSemRecModel.RegisterLinkInfoIsNotMember = UrlsSemanticas.GetURLSolicitarAccesoComunidad(mBaseURLIdioma, mUtilIdiomas, mProyectoActual.NombreCorto);
                }
                else if (mProyectoActual.TipoAcceso == TipoAcceso.Publico)
                {
                    mSemRecModel.RegisterLinkInfoIsNotMember = UrlsSemanticas.GetURLHacerseMiembroComunidad(mBaseURLIdioma, mUtilIdiomas, mProyectoActual.NombreCorto);
                }
            }
        }

        private void AgregarEntidadesPrincipales(Guid pIdentidadID)
        {
            string urlOntologia = Ontologia.GestorOWL.UrlOntologia;
            string namesOntologia = Ontologia.GestorOWL.NamespaceOntologia;
            mSemRecModel.RootEntities = new List<SemanticEntityModel>();

            foreach (ElementoOntologia entidad in mEntidades)
            {
                mSemRecModel.RootEntities.Add(ObtenerModeloEntidad(entidad, 1, null, null, pIdentidadID));
            }

            //Puede que al traer las entidades externas se hayan machacado la url y el namespace la ontología. Debemos recuperarlos.
            Ontologia.GestorOWL.UrlOntologia = urlOntologia;
            Ontologia.GestorOWL.NamespaceOntologia = namesOntologia;
        }

        /// <summary>
        /// Completa el modelo semántico del SEM CMS para un recurso.
        /// </summary>
        /// <param name="pSemRecModel">Modelo del SEM CMS</param>
        /// <param name="pOntologia">Ontología a la que pertenece el recurso</param>
        private void ObtenerModeloSemCMS()
        {
            OrdenarEntidadesAuxiliaresConCampoOrden();

            mSemRecModel.OntologyNamespace = mOntologia.GestorOWL.NamespaceOntologia;
            mSemRecModel.OntologyUrl = mOntologia.GestorOWL.UrlOntologia;
            mSemRecModel.OntologyNamespaces = mOntologia.NamespacesDefinidos;
            mSemRecModel.DocSemCmsProperty = "gnoss:" + UtilImportarExportar.PROPIEDAD_DOC_SEM;



            CargarIdiomasModel();

            ObtenerValoresDeEntGrafoDependientes();
        }

        /// <summary>
        /// Obtiene el modelo semántico de un entidad ontológica.
        /// </summary>
        /// <param name="pEntidad">Entidad semántica</param>
        /// <param name="pProfundidad">Profundidad de la entidad según la jerarquia de clases</param>
        /// <param name="pEntidadPadre">Entidad padre de la actual</param>
        /// <param name="pSemPropPadreModel">Modelo propiedad padre</param>
        /// <returns>Modelo semántico de un entidad ontológica</returns>
        private SemanticEntityModel ObtenerModeloEntidad(ElementoOntologia pEntidad, int pProfundidad, ElementoOntologia pEntidadPadre, SemanticPropertyModel pSemPropPadreModel, Guid pIdentidadID)
        {
            return ObtenerModeloEntidad(pEntidad, pProfundidad, pEntidadPadre, pSemPropPadreModel, false, pIdentidadID);
        }

        /// <summary>
        /// Obtiene el modelo semántico de un entidad ontológica.
        /// </summary>
        /// <param name="pEntidad">Entidad semántica</param>
        /// <param name="pProfundidad">Profundidad de la entidad según la jerarquia de clases</param>
        /// <param name="pEntidadPadre">Entidad padre de la actual</param>
        /// <param name="pSemPropPadreModel">Modelo propiedad padre</param>
        /// <param name="pOmitirHerencias">Indica si se deben omitir el funcionamiento de herencias. No nos interesa que nos devuelva la entidad padre si hay herencia, si no la hija en cualquier caso</param>
        /// <returns>Modelo semántico de un entidad ontológica</returns>
        private SemanticEntityModel ObtenerModeloEntidad(ElementoOntologia pEntidad, int pProfundidad, ElementoOntologia pEntidadPadre, SemanticPropertyModel pSemPropPadreModel, bool pOmitirHerencias, Guid pIdentidadID)
        {
            SemanticEntityModel entModel = new SemanticEntityModel();
            entModel.SemanticResourceModel = mSemRecModel;
            entModel.Entity = pEntidad;
            entModel.Depth = pProfundidad;
            entModel.ReadMode = (mSemRecModel.ReadMode || (pSemPropPadreModel != null && pSemPropPadreModel.ReadMode));

            ObtenerHijosEntidad(entModel, pIdentidadID);

            if (!mSemRecModel.ReadMode && !pOmitirHerencias)
            {
                if (pEntidad.Subclases.Count > 0 && pEntidad != pEntidadPadre) //Solo si no se está agregando después de entrar por el otro IF
                {
                    entModel.SubEntities = new List<SemanticEntityModel>();
                    mEntidadesSuperClassEHijas.Add(pEntidad, new List<ElementoOntologia>());

                    foreach (string tipoEntidadSubClass in pEntidad.Subclases)
                    {
                        ElementoOntologia instanciaEntidad = mOntologia.GetEntidadTipo(tipoEntidadSubClass, true);
                        instanciaEntidad.ID = instanciaEntidad.TipoEntidad + "_" + mDocumentoID.ToString() + "_" + Guid.NewGuid();
                        SemanticEntityModel entHija = ObtenerModeloEntidad(instanciaEntidad, pProfundidad + 1, pEntidad, pSemPropPadreModel, pIdentidadID);
                        entModel.SubEntities.Add(entHija);
                        entHija.SuperEntity = entModel;
                        mEntidadesSuperClassEHijas[pEntidad].Add(instanciaEntidad);
                        entHija.Hidden = true;
                        //if (entModel.SubEntities.Count > 1)
                        //{
                        //    entHija.Hidden = true;
                        //}
                        //else
                        //{
                        //    mEntidadesSeVanAPintar.Add(instanciaEntidad);
                        //}
                    }
                }
                else if (pEntidad.SuperclasesUtiles.Count > 0 && (pEntidadPadre == null || !pEntidad.SuperclasesUtiles.Contains(pEntidadPadre.TipoEntidad)))//Solo si no se está agregando después de entrar por el otro IF
                {
                    ElementoOntologia instanciaEntidadSuperClass = mOntologia.GetEntidadTipo(pEntidad.SuperclasesUtiles[0], true);
                    instanciaEntidadSuperClass.ID = instanciaEntidadSuperClass.TipoEntidad + "_" + mDocumentoID.ToString() + "_" + Guid.NewGuid();
                    SemanticEntityModel entModelPadre = ObtenerModeloEntidad(instanciaEntidadSuperClass, pProfundidad, instanciaEntidadSuperClass, pSemPropPadreModel, pIdentidadID);
                    mEntidadesSuperClassEHijas.Add(instanciaEntidadSuperClass, new List<ElementoOntologia>());
                    entModelPadre.SubEntities = new List<SemanticEntityModel>();

                    foreach (string tipoEntidadSubClass in instanciaEntidadSuperClass.Subclases)
                    {
                        if (tipoEntidadSubClass == pEntidad.TipoEntidad)
                        {
                            entModel.Depth++;
                            entModelPadre.SelectedSubEntity = entModel;
                            entModelPadre.SubEntities.Add(entModel);
                            entModel.SuperEntity = entModelPadre;
                            entModel = entModelPadre;
                            mEntidadesSeVanAPintar.Add(pEntidad);
                        }
                        else
                        {
                            ElementoOntologia instanciaEntidad = mOntologia.GetEntidadTipo(tipoEntidadSubClass, true);
                            instanciaEntidad.ID = instanciaEntidad.TipoEntidad + "_" + mDocumentoID.ToString() + "_" + Guid.NewGuid();
                            SemanticEntityModel entHija = ObtenerModeloEntidad(instanciaEntidad, pProfundidad + 1, instanciaEntidadSuperClass, pSemPropPadreModel, pIdentidadID);
                            entHija.Hidden = true;
                            entModelPadre.SubEntities.Add(entHija);
                            entHija.SuperEntity = entModelPadre;
                            mEntidadesSuperClassEHijas[instanciaEntidadSuperClass].Add(instanciaEntidad);
                        }
                    }
                }
                else if (pEntidad.Subclases.Count == 0 && pEntidad.SuperclasesUtiles.Count == 0)
                {
                    mEntidadesSeVanAPintar.Add(pEntidad);
                }
            }
            else
            {
                mEntidadesSeVanAPintar.Add(pEntidad);
                ObtenerMapaGoogleEntidad(entModel);
                ObtenerRDFaEntidad(entModel);
            }

            return entModel;
        }

        /// <summary>
        /// Obtiene las entidades externas a la ontología actual.
        /// </summary>
        private void ObtenerEntidadesExternas(bool pUsarAfinidad = false)
        {
            try
            {
                if (!ConfigGenXml.HayEntidadesSelecc)
                {
                    return;
                }

                mLoggingService.AgregarEntrada("FormSem Inicio GeneradorPantillaOWL.ObtenerEntidadesExternas()");

                mDatosEntidadesExternas = new Dictionary<KeyValuePair<string, string>, object[]>();
                AD.EntityModel.Models.PersonaDS.Persona filaPersona = null;

                if (mIdentidadActual != null)
                {
                    filaPersona = mIdentidadActual.Persona.FilaPersona;
                }

                //Preparo que datos hay que cargar:
                UtilSemCms utilSemCms = new UtilSemCms(mEntityContext, mLoggingService, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                foreach (ElementoOntologia elemPrinc in mEntidades)
                {
                    utilSemCms.ObtenerPropiedadesSelecEntDeEntidad(elemPrinc, mDatosEntidadesExternas, mUrlIntragnoss, filaPersona, mProyectoActual.FilaProyecto, Entidades, PropiedadCallbackTraerMas.Value.Value, PropiedadCallbackTraerMas.Key, true, !string.IsNullOrEmpty(PropiedadIdiomaBusquedaComunidad));
                }

                mLoggingService.AgregarEntrada("FormSem IDs Entidades externas obtenidos, ahora a por los datos de las mismas");

                //Consulto a virtuoso o el modelo acido los datos:
                utilSemCms.ObtenerDatosEntidadesExternas(mDatosEntidadesExternas, mUrlIntragnoss, filaPersona, mProyectoActual.FilaProyecto, Entidades, pUsarAfinidad);

                mLoggingService.AgregarEntrada("FormSem Fin GeneradorPantillaOWL.ObtenerEntidadesExternas()");
            }
            catch (Exception ex)
            {
                GuardarMensajeErrorAdmin("Error al obtener las entidades externas de otro grafo distinto al de la ontología configuradas el XML.", ex);
                GuardarLogErrorAJAX("Error docSem '" + mDocumentoID + "' al obtener entidades externas" + Environment.NewLine + ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Obtiene el título configurado para la página en el XML de configuración.
        /// </summary>
        private void ObtenerTituloConfiguradoPagina()
        {
            try
            {
                if (ConfigGenXml.PropiedadesTitulo.Count > 0)
                {
                    string titulo = "";
                    Dictionary<string, string> propiedadesTitulo = ConfigGenXml.PropiedadesTitulo;

                    foreach (string key in propiedadesTitulo.Keys)
                    {
                        if (propiedadesTitulo[key] != null)
                        {
                            Propiedad prop = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(key, propiedadesTitulo[key], mEntidades);

                            if (prop != null)
                            {
                                if (prop.ListaValoresIdioma.Count > 0)
                                {
                                    List<string> listaValores = prop.OntenerValoresIdiomaUsuarioUOtrosSiVacio(Ontologia.IdiomaUsuario);
                                    if (listaValores.Count > 0)
                                    {
                                        titulo += listaValores[0];
                                    }
                                }
                                else
                                {
                                    titulo += prop.PrimerValorPropiedad;
                                }
                            }
                        }
                        else if (key.Equals("[NombreOntologia]"))
                        {
                            string nombreOnt = string.Empty;

                            if (mDocumento.GestorDocumental.ListaDocumentos.ContainsKey(mOntologia.OntologiaID))
                            {
                                string enlaceOnt = mDocumento.GestorDocumental.ListaDocumentos[mOntologia.OntologiaID].Enlace;
                                enlaceOnt = enlaceOnt.Substring(0, enlaceOnt.IndexOf(".owl"));
                                FacetaCN facCN = new FacetaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                                OntologiaProyecto filaOnto = facCN.ObtenerOntologiaProyectoPorEnlace(mDocumento.ProyectoID, enlaceOnt);

                                if (filaOnto != null)
                                {
                                    nombreOnt = UtilCadenas.ObtenerTextoDeIdioma(filaOnto.NombreOnt, Ontologia.IdiomaUsuario, "es");
                                }

                                facCN.Dispose();
                            }

                            titulo += nombreOnt;
                        }
                        else
                        {
                            titulo += UtilCadenas.ObtenerTextoDeIdioma(key, Ontologia.IdiomaUsuario, null);
                        }
                    }

                    mSemRecModel.PageTitle = titulo.Trim();
                }
            }
            catch (Exception ex)
            {
                GuardarMensajeErrorAdmin("El título de la página que se forma a partir de propiedades de la ontología que está configurado en el XML no tiene las propiedades correctas.", ex);
                GuardarLogErrorAJAX("Error docSem '" + mDocumentoID + "' al pintar el título de la página:" + Environment.NewLine + ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Establece las configuraciones de la página del XML.
        /// </summary>
        private void EstablecerConfiguracionesPagina()
        {
            if (mProyectoActual.EsCatalogo)
            {
                mSemRecModel.SemCmsDrawOverMenu = true;
            }

            //Miramos los configurado independientemente de que sea catálogo o no:
            if (Ontologia.ConfiguracionPlantilla.MenuDocumentoAbajo.HasValue)
            {
                if (Ontologia.ConfiguracionPlantilla.MenuDocumentoAbajo.Value)
                {
                    mSemRecModel.SemCmsDrawOverMenu = true;
                }
                else
                {
                    mSemRecModel.SemCmsDrawOverMenu = false;
                }
            }

            if (Ontologia.ConfiguracionPlantilla.OcultarTituloDescpEImg)
            {
                mSemRecModel.HideResourceTitle = true;
            }

            mSemRecModel.DocumentTitle = UtilCadenas.ObtenerTextoDeIdioma(mDocumento.Titulo, Ontologia.IdiomaUsuario, null);
            mSemRecModel.MvcActionsUrl = UrlsSemanticas.GetURLBaseRecursosFicha(mBaseURLIdioma, mUtilIdiomas, mProyectoActual.NombreCorto, null, mDocumento, false);
        }

        /// <summary>
        /// Agrega al modelo la información extra de los elementos.
        /// </summary>
        private void AgregarInfoExtraElementos()
        {
            mSemRecModel.AuxiliaryElementsFeaturesInfo = "$baseUrlStatic=" + mBaseURLStatic + "|" + "$baseUrlContent=" + mBaseURLContent + "|$txtHackImgPrincipal=txtHackValorImgRepresentante|";

            if (!string.IsNullOrEmpty(PropiedadIdiomaBusquedaComunidad))
            {
                mSemRecModel.AuxiliaryElementsFeaturesInfo = string.Concat(mSemRecModel.AuxiliaryElementsFeaturesInfo, "$propLangBusqCom=" + PropiedadIdiomaBusquedaComunidad + "|");
            }

            if (IdiomaDefecto != null)
            {
                mSemRecModel.DefaultLanguage = IdiomaDefecto;
                mSemRecModel.AvailableLanguages = IdiomasDisponibles;
            }

            mSemRecModel.AuxiliaryRDFInfo = "";
            mSemRecModel.AuxiliaryEntityIDRegisterInfo = "";

            foreach (ElementoOntologia entidad in mEntidades)
            {
                mSemRecModel.AuxiliaryRDFInfo += AgregarValoresRDF(entidad, 1);
            }

            //Agrego la cadena que indica el final del RDF:
            mSemRecModel.AuxiliaryRDFInfo += "<||>";
            mSemRecModel.AuxiliaryInheritancesInfo = "";

            foreach (ElementoOntologia entidadSuper in mEntidadesSuperClassEHijas.Keys)
            {
                mSemRecModel.AuxiliaryInheritancesInfo += "<||>" + entidadSuper.TipoEntidad;

                int tipoDominioFun = 0;

                if (mEntidadesSuperClassTipoDominioFunc.ContainsKey(entidadSuper))
                {
                    tipoDominioFun = mEntidadesSuperClassTipoDominioFunc[entidadSuper];
                }

                foreach (ElementoOntologia entidadHija in mEntidadesSuperClassEHijas[entidadSuper])
                {
                    mSemRecModel.AuxiliaryInheritancesInfo += "<|>" + entidadHija.TipoEntidad + "," + AgregarValoresRDF(entidadHija, tipoDominioFun);
                }

                mSemRecModel.AuxiliaryInheritancesInfo += "<|>";
            }

            mSemRecModel.AuxiliaryInheritancesInfo += "<||>";

            //Restro las propiedades del resto de entidades no principales:
            foreach (ElementoOntologia entidad in mOntologia.Entidades)
            {
                if (!mEntidades.Contains(entidad))
                {
                    foreach (Propiedad propiedad in entidad.Propiedades)
                    {
                        if (propiedad.ElementoOntologia == null)
                        {
                            propiedad.ElementoOntologia = entidad;
                        }

                        RegistrarCaracteristicasPropiedad(propiedad, entidad, 0);
                    }
                }

                //Agrego al registro los atributos representantes de la entidad:
                RegistrarCaracteristicasEntidad(entidad);
            }

            mSemRecModel.TitlePropretyIDs = mTxtTituloDocSemID;
            mSemRecModel.DescriptionPropretyIDs = mTxtDescripcionDocSemID;

            mSemRecModel.DateWithTimeAvailable = Ontologia.ConfiguracionPlantilla.HayFechaConHora;

            if (EntidadesExtEditablesDocID != null)
            {
                mSemRecModel.AuxiliarySubOntologiesExtInfo = "";
                Dictionary<string, List<object[]>> entPropDocID = new Dictionary<string, List<object[]>>();

                foreach (EntidadExtEditableDoc entExtEdit in EntidadesExtEditablesDocID.Values)
                {
                    string clave = entExtEdit.TipoEntidad + "," + entExtEdit.Propiedad;

                    if (!entPropDocID.ContainsKey(clave))
                    {
                        entPropDocID.Add(clave, new List<object[]>());
                    }

                    entPropDocID[clave].Add(new object[] { entExtEdit.NumValorPropiedad, entExtEdit.DocumentoID, entExtEdit.EntidadID });
                }

                foreach (string entProp in entPropDocID.Keys)
                {
                    mSemRecModel.AuxiliarySubOntologiesExtInfo = string.Concat(mSemRecModel.AuxiliarySubOntologiesExtInfo, entProp);

                    foreach (object[] par in entPropDocID[entProp])
                    {
                        mSemRecModel.AuxiliarySubOntologiesExtInfo = string.Concat(mSemRecModel.AuxiliarySubOntologiesExtInfo, "|", (int)par[0], ",", (Guid)par[1], ",", (string)par[2]);
                    }

                    mSemRecModel.AuxiliarySubOntologiesExtInfo = string.Concat(mSemRecModel.AuxiliarySubOntologiesExtInfo, "|||");
                }
            }
        }

        /// <summary>
        /// Agrega la entidad al txt de valores rdf.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pTipoDominioFuncional">Indica si el dominio de la propiedad superior es funcional (1), la cardinalidad 1 (2) o no es mayor que 1 (0).</param>
        private string AgregarValoresRDF(ElementoOntologia pEntidad, int pTipoDominioFuncional)
        {
            string texto = "";

            if (pEntidad.Subclases.Count > 0)
            {
                foreach (ElementoOntologia posibleSubClase in mEntidadesSeVanAPintar)
                {
                    if (posibleSubClase.TipoEntidad == pEntidad.Subclases[0])
                    {
                        if (!mEntidadesSuperClassTipoDominioFunc.ContainsKey(pEntidad))
                        {
                            mEntidadesSuperClassTipoDominioFunc.Add(pEntidad, pTipoDominioFuncional);
                        }

                        return AgregarValoresRDF(posibleSubClase, pTipoDominioFuncional);
                    }
                }
            }

            texto += "<" + pEntidad.TipoEntidad + ">";
            string idEntidad = pEntidad.ID;

            if (idEntidad.Contains("/"))
            {
                idEntidad = idEntidad.Substring(idEntidad.LastIndexOf("/") + 1);
            }
            if (idEntidad.Contains("#"))
            {
                idEntidad = idEntidad.Substring(idEntidad.LastIndexOf("#") + 1);
            }

            mSemRecModel.AuxiliaryEntityIDRegisterInfo += "<" + pEntidad.TipoEntidad + "><id>" + idEntidad + "</id>";

            foreach (Propiedad propiedad in pEntidad.Propiedades)
            {
                if (propiedad.ElementoOntologia == null)
                {
                    propiedad.ElementoOntologia = pEntidad;
                }

                if (propiedad.Tipo == TipoPropiedad.DatatypeProperty || (propiedad.EspecifPropiedad.SelectorEntidad != null && propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion != "Edicion"))
                {
                    if (propiedad.ListaValoresIdioma.Count > 0)
                    {
                        List<string> valoresIdiomasUnificados = new List<string>();

                        foreach (string idioma in propiedad.ListaValoresIdioma.Keys)
                        {
                            int count = 0;

                            foreach (string valor in propiedad.ListaValoresIdioma[idioma].Keys)
                            {
                                string valConIdioma = valor + "@" + idioma + "[|lang|]";

                                if (valoresIdiomasUnificados.Count == count)
                                {
                                    valoresIdiomasUnificados.Add(valConIdioma);
                                }
                                else
                                {
                                    valoresIdiomasUnificados[count] += valConIdioma;
                                }

                                count++;
                            }
                        }

                        foreach (string valConIdioma in valoresIdiomasUnificados)
                        {
                            texto += "<" + propiedad.Nombre + ">" + valConIdioma.Replace("<", "[--C]").Replace(">", "[C--]") + "</" + propiedad.Nombre + ">";
                        }
                    }
                    else
                    {
                        if (propiedad.FunctionalProperty || propiedad.CardinalidadMenorOIgualUno || propiedad.EspecifPropiedad.ValoresSepComas || (propiedad.EspecifPropiedad.SelectorEntidad != null && propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "Tesauro"))
                        {
                            string valorProp = "";
                            string nombreProp = propiedad.Nombre;

                            if (propiedad.UnicoValor.Key != null)
                            {
                                if (propiedad.EspecifPropiedad.SelectorEntidad == null || pTipoDominioFuncional == 0 || (propiedad.EspecifPropiedad.SelectorEntidad.TipoPresentacion != "Combo" && propiedad.EspecifPropiedad.SelectorEntidad.TipoPresentacion != "ListaCheck") || propiedad.ListaValoresUsados.Contains(propiedad.UnicoValor.Key))
                                {
                                    valorProp = propiedad.UnicoValor.Key;
                                }
                            }
                            else if (propiedad.ListaValores != null)
                            {
                                string coma = "";
                                List<string> valores = new List<string>();
                                valores.AddRange(propiedad.ListaValores.Keys);

                                if (propiedad.EspecifPropiedad.SelectorEntidad != null && propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "Tesauro" && propiedad.EspecifPropiedad.SelectorEntidad.TipoPresentacion == "Arbol")
                                {
                                    valores.Sort();
                                }

                                foreach (string valor in valores)
                                {
                                    if (propiedad.EspecifPropiedad.SelectorEntidad == null || pTipoDominioFuncional == 0 || (propiedad.EspecifPropiedad.SelectorEntidad.TipoPresentacion != "Combo" && propiedad.EspecifPropiedad.SelectorEntidad.TipoPresentacion != "ListaCheck") || propiedad.ListaValoresUsados.Contains(valor))
                                    {
                                        valorProp += coma + valor;
                                        coma = ",";
                                    }
                                }
                            }

                            texto += "<" + nombreProp + ">" + valorProp.Replace("<", "[--C]").Replace(">", "[C--]") + "</" + nombreProp + ">";
                        }
                        else if (propiedad.ListaValores != null)
                        {
                            foreach (string valor in propiedad.ListaValores.Keys)
                            {
                                if (propiedad.EspecifPropiedad.SelectorEntidad == null || pTipoDominioFuncional == 0 || (propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion != "Combo" && propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion != "ListaCheck") || propiedad.ListaValoresUsados.Contains(valor))
                                {
                                    texto += "<" + propiedad.Nombre + ">" + valor.Replace("<", "[--C]").Replace(">", "[C--]") + "</" + propiedad.Nombre + ">";
                                }
                            }
                        }
                    }
                }
                else
                {//Es ObjectPropert
                    if (string.IsNullOrEmpty(propiedad.Rango))
                    {
                        string error = "La propiedad '" + propiedad.Nombre + "' no tiene rango y no está configurada como selector de entidad.";
                        GuardarMensajeErrorAdmin(error, null);
                        throw new Exception(error);
                    }

                    if (propiedad.FunctionalProperty || propiedad.CardinalidadMenorOIgualUno)
                    {//Solo puede tomar un valor.
                        ElementoOntologia entidad = null;

                        if (propiedad.UnicoValor.Key != null)
                        {
                            entidad = propiedad.UnicoValor.Value;
                        }
                        else if (propiedad.ListaValores != null && propiedad.ListaValores.Any())
                        {
                            entidad = propiedad.ListaValores.FirstOrDefault().Value;
                        }

                        if (entidad == null)//Creamos una nueva:
                        {
                            entidad = mOntologia.GetEntidadTipo(propiedad.Rango, false);
                        }

                        int tipoDomFun = pTipoDominioFuncional;

                        if (tipoDomFun > 0 && propiedad.CardinalidadMenorOIgualUno)
                        {
                            tipoDomFun = 2;
                        }

                        texto += "<" + propiedad.Nombre + ">";
                        mSemRecModel.AuxiliaryEntityIDRegisterInfo += "<" + propiedad.Nombre + ">";
                        texto += AgregarValoresRDF(entidad, tipoDomFun);
                        texto += "</" + propiedad.Nombre + ">";
                        mSemRecModel.AuxiliaryEntityIDRegisterInfo += "</" + propiedad.Nombre + ">";
                    }
                    else
                    {//Puede tomar varios valores.
                        if (propiedad.ListaValores.Count > 0)
                        {
                            foreach (ElementoOntologia entidadProp in propiedad.ListaValores.Values)
                            {
                                if (entidadProp != null)
                                {
                                    texto += "<" + propiedad.Nombre + ">";
                                    mSemRecModel.AuxiliaryEntityIDRegisterInfo += "<" + propiedad.Nombre + ">";
                                    texto += AgregarValoresRDF(entidadProp, 0);
                                    texto += "</" + propiedad.Nombre + ">";
                                    mSemRecModel.AuxiliaryEntityIDRegisterInfo += "</" + propiedad.Nombre + ">";
                                }
                            }
                        }
                    }
                }

                RegistrarCaracteristicasPropiedad(propiedad, pEntidad, pTipoDominioFuncional);
            }

            texto += "</" + pEntidad.TipoEntidad + ">";
            mSemRecModel.AuxiliaryEntityIDRegisterInfo += "</" + pEntidad.TipoEntidad + ">";

            return texto;
        }

        /// <summary>
        /// Registra las caracteristicas de la entidad.
        /// </summary>
        /// <param name="pEntidad">Entidad</param> 
        public void RegistrarCaracteristicasEntidad(ElementoOntologia pEntidad)
        {
            if (mSemRecModel.ReadMode)
            {
                return;
            }

            string idCaracteristica = pEntidad.TipoEntidad + ",,";
            if (!mSemRecModel.AuxiliaryElementsFeaturesInfo.Contains(idCaracteristica))
            {
                string caracteristica = idCaracteristica;

                if (pEntidad.EspecifEntidad.Representantes.Count > 0)
                {
                    caracteristica += "atrRepre=";

                    foreach (Representante representante in pEntidad.EspecifEntidad.Representantes)
                    {
                        caracteristica += representante.Propiedad.Nombre + ";" + (short)representante.TipoRepres + "&";
                    }

                    caracteristica += ",";
                }

                if (pEntidad.SuperclasesUtiles.Count > 0)
                {
                    caracteristica += "subclase=true,";
                    caracteristica += "superclases=";

                    foreach (string superClase in pEntidad.SuperclasesUtiles)
                    {
                        caracteristica += superClase + ";";
                    }

                    caracteristica += ",";
                }

                if (pEntidad.Subclases.Count > 0)
                {
                    caracteristica += "superclase=true,";
                    caracteristica += "subclases=";

                    foreach (string subClase in pEntidad.Subclases)
                    {
                        caracteristica += subClase + ";";
                    }

                    caracteristica += ",";
                }

                if (Entidades[0].TipoEntidad == pEntidad.TipoEntidad)
                {
                    caracteristica += "entPrincipal=true,";
                }

                mSemRecModel.AuxiliaryElementsFeaturesInfo += caracteristica + "|";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pPropiedad"></param>
        /// <param name="pEntidad"></param>
        /// <param name="pTipoDominioFuncional">Indica si el dominio de la propiedad superior es funcional (1), la cardinalidad 1 (2) o no es mayor que 1 (0).</param>
        public void RegistrarCaracteristicasPropiedad(Propiedad pPropiedad, ElementoOntologia pEntidad, int pTipoDominioFuncional)
        {
            if (mSemRecModel.ReadMode)
            {
                return;
            }

            if (pEntidad.Subclases.Count > 0)
            {
                return;
            }

            string clausulaRecoger = "recoger=true,";

            if (pTipoDominioFuncional == 2)
            {
                clausulaRecoger += "domSupCardi1=true,";
            }

            //string idCaracteristica = pEntidad.TipoEntidad + "," + pPropiedad.Nombre;
            string idCaracteristica = pEntidad.TipoEntidad + "," + pPropiedad.Nombre;
            if (!mSemRecModel.AuxiliaryElementsFeaturesInfo.Contains(idCaracteristica + ","))
            {

                string caracteristica = idCaracteristica + ",tipo=";

                if (pPropiedad.Tipo == TipoPropiedad.DatatypeProperty)
                {
                    bool recogerDatos = false;

                    if (pPropiedad.FunctionalProperty)
                    {
                        caracteristica += "FD,";
                        recogerDatos = true;
                    }
                    else if (pPropiedad.CardinalidadMenorOIgualUno)
                    {
                        caracteristica += "CD,";
                        recogerDatos = true;
                    }
                    else if (pPropiedad.EspecifPropiedad.ValoresSepComas)
                    {
                        caracteristica += "VD,";
                        recogerDatos = true;
                    }
                    else
                    {
                        caracteristica += "LD,";
                    }

                    caracteristica += "tipoCampo=" + pPropiedad.EspecifPropiedad.TipoCampo.ToString() + ",";

                    if (!string.IsNullOrEmpty(pPropiedad.EspecifPropiedad.ValorDefectoNoSeleccionable))
                    {
                        caracteristica += "valDefNoSelecc=" + UtilCadenas.ObtenerTextoDeIdioma(pPropiedad.EspecifPropiedad.ValorDefectoNoSeleccionable, mUtilIdiomas.LanguageCode, null) + ",";
                    }

                    if (pPropiedad.EspecifPropiedad.RestrNumCaract != null)
                    {
                        caracteristica += "numCaract=" + pPropiedad.EspecifPropiedad.RestrNumCaract.TipoRestricion + "," + pPropiedad.EspecifPropiedad.RestrNumCaract.Valor + "," + pPropiedad.EspecifPropiedad.RestrNumCaract.ValorHasta + ",";
                    }

                    if (pTipoDominioFuncional > 0 && recogerDatos)
                    {
                        caracteristica += clausulaRecoger;
                    }

                    if (pPropiedad.EspecifPropiedad.GrafoDependiente != null && pPropiedad.ValorUnico)
                    {
                        caracteristica += "propGrafoDep=true,";

                        if (PropsEntGrafoDependientes.ContainsKey(new KeyValuePair<string, string>(pPropiedad.Nombre, pPropiedad.ElementoOntologia.TipoEntidad)))
                        {
                            KeyValuePair<string, string> propHijaDep = PropsEntGrafoDependientes[new KeyValuePair<string, string>(pPropiedad.Nombre, pPropiedad.ElementoOntologia.TipoEntidad)];
                            caracteristica += "propEntDependiente=[" + propHijaDep.Key + "],[" + propHijaDep.Value + "],";
                        }

                        if (pPropiedad.EspecifPropiedad.PropDependiente.Key == null)
                        {
                            caracteristica += "propGrafoDepSinPadres=true,";
                        }
                    }

                    if (pPropiedad.EspecifPropiedad.FechaConHora)
                    {
                        caracteristica += "propFechaConHora=true,";
                    }
                }
                else
                {
                    if (pPropiedad.EspecifPropiedad.SelectorEntidad != null && pPropiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion != "Edicion")
                    {
                        if (pPropiedad.FunctionalProperty)
                        {
                            caracteristica += "FSEO,";

                            if (pTipoDominioFuncional > 0)
                            {
                                caracteristica += clausulaRecoger;
                            }
                        }
                        else if (pPropiedad.CardinalidadMenorOIgualUno || pPropiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "Tesauro")
                        {
                            caracteristica += "CSEO,";

                            if (pTipoDominioFuncional > 0)
                            {
                                caracteristica += clausulaRecoger;
                            }
                        }
                        else
                        {
                            caracteristica += "LSEO,";
                        }

                        caracteristica += "tipoCampo=" + TipoCampoOntologia.Texto.ToString() + ",";

                        KeyValuePair<string, string> claveDepen = new KeyValuePair<string, string>(pPropiedad.Nombre, pPropiedad.ElementoOntologia.TipoEntidad);

                        if (Ontologia.ConfiguracionPlantilla.PropsSelecEntDependientes != null && Ontologia.ConfiguracionPlantilla.PropsSelecEntDependientes.ContainsKey(claveDepen))
                        {
                            caracteristica += "propSelectEntDep=true,propSelectEntDependiente=";

                            foreach (KeyValuePair<string, string> propsDepen in Ontologia.ConfiguracionPlantilla.PropsSelecEntDependientes[claveDepen])
                            {
                                caracteristica += propsDepen.Key + ";" + propsDepen.Value + ";;";
                            }

                            caracteristica += ",";
                        }
                    }
                    else if (pPropiedad.FunctionalProperty)
                    {
                        caracteristica += "FO,";
                    }
                    else if (pPropiedad.CardinalidadMenorOIgualUno)
                    {
                        caracteristica += "CO,";
                    }
                    else
                    {
                        caracteristica += "LO,";
                    }

                    caracteristica += "rango=" + pPropiedad.Rango + ",";
                }

                if (pEntidad.SuperclasesUtiles.Count > 0)
                {
                    caracteristica += "subclase=true,";
                }

                foreach (Restriccion restriccion in pEntidad.Restricciones)
                {
                    if (pPropiedad.Nombre.Equals(restriccion.Propiedad))
                    {
                        caracteristica += "cardi=" + restriccion.TipoRestriccion + "," + restriccion.Valor + ",";
                        break;
                    }
                }

                string claseCSS = "EspefPropiedad_" + pPropiedad.Nombre;

                if (pPropiedad.EspecifPropiedad.TextoEliminarElemSel != null)
                {
                    caracteristica += "textoEliminar=" + pPropiedad.EspecifPropiedad.TextoEliminarElemSel + ",";
                }

                if (pPropiedad.EspecifPropiedad.ImagenMini != null)
                {
                    caracteristica += "imagenMiniVP=";
                    Dictionary<int, int> tamanyos = pPropiedad.EspecifPropiedad.ImagenMini.Tamanios;
                    foreach (int ancho in tamanyos.Keys)
                    {
                        caracteristica += ancho + "." + tamanyos[ancho] + ".";
                    }

                    caracteristica += ",";
                }

                if (pPropiedad.EspecifPropiedad.CapturarFlash.Key != null)
                {
                    caracteristica += "CapturarFlash=" + pPropiedad.EspecifPropiedad.CapturarFlash.Key + "," + pPropiedad.EspecifPropiedad.CapturarFlash.Value + ",";
                }

                int numPropRep = ObtenerNumElemRepeticionProp(pPropiedad);

                if (numPropRep > 0)
                {
                    caracteristica += "numPropRep=" + numPropRep + ",";
                }

                if (pPropiedad.EspecifPropiedad.UsarJcrop)
                {
                    if (pPropiedad.EspecifPropiedad.MinSizeJcrop.Key != 0 && pPropiedad.EspecifPropiedad.MaxSizeJcrop.Key != 0 && (pPropiedad.EspecifPropiedad.MaxSizeJcrop.Key < pPropiedad.EspecifPropiedad.MinSizeJcrop.Key || pPropiedad.EspecifPropiedad.MaxSizeJcrop.Value < pPropiedad.EspecifPropiedad.MinSizeJcrop.Value))
                    {
                        string error = "JCrop de '" + pPropiedad.Nombre + "' mal configurado, el tamano mínimo no puede ser mayor que el máximo.";
                        GuardarMensajeErrorAdmin(error, null);
                        throw new Exception(error);
                    }

                    string datosJcrop = pPropiedad.EspecifPropiedad.MinSizeJcrop.Key + ";" + pPropiedad.EspecifPropiedad.MinSizeJcrop.Value + ";" + pPropiedad.EspecifPropiedad.MaxSizeJcrop.Key + ";" + pPropiedad.EspecifPropiedad.MaxSizeJcrop.Value + ";";
                    caracteristica += "UsarJcrop=" + datosJcrop + ",";
                }

                if (PropiedadEsMultidioma(pPropiedad))
                {
                    caracteristica += "propMultiIdioma=true,";
                }

                mSemRecModel.AuxiliaryElementsFeaturesInfo += caracteristica + "|";
            }
        }

        /// <summary>
        /// Indica si una propiedad es multiidioma.
        /// </summary>
        /// <param name="pPropiedad">Propiedad</param>
        /// <returns>TRUE si una propiedad es multiidioma, FALSE si no</returns>
        private bool PropiedadEsMultidioma(Propiedad pPropiedad)
        {
            return (pPropiedad.Tipo == TipoPropiedad.DatatypeProperty && !string.IsNullOrEmpty(mIdiomaDefecto) && (pPropiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.Tiny || pPropiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.Texto || pPropiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.TextArea || pPropiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.Archivo || pPropiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.ArchivoLink || pPropiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.EmbebedObject || pPropiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.EmbebedLink) && (pPropiedad.EspecifPropiedad.GrafoDependiente == null || !pPropiedad.ValorUnico) && pPropiedad.EspecifPropiedad.GrafoAutocompletar == null && !pPropiedad.EspecifPropiedad.NoMultiIdioma && (string.IsNullOrEmpty(PropiedadIdiomaBusquedaComunidad) || PropiedadIdiomaBusquedaComunidad != pPropiedad.Nombre));
        }

        /// <summary>
        /// Obtiene el número de propiedades que se repiten de la propiedad actual.
        /// </summary>
        /// <param name="pPropiedad">Propiedad</param>
        /// <returns>Número de propiedades que se repiten de la propiedad actual</returns>
        private int ObtenerNumElemRepeticionProp(Propiedad pPropiedad)
        {
            if (Ontologia.ConfiguracionPlantilla.PropsRepetidas != null)
            {
                foreach (KeyValuePair<string, string> propEnt in Ontologia.ConfiguracionPlantilla.PropsRepetidas.Keys)
                {
                    if (pPropiedad.Nombre == propEnt.Key && (propEnt.Value == pPropiedad.ElementoOntologia.TipoEntidad || pPropiedad.ElementoOntologia.SuperclasesUtiles.Contains(propEnt.Value)))
                    {
                        return Ontologia.ConfiguracionPlantilla.PropsRepetidas[propEnt].Count;
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// Agrega un elemento ordenado a la altura de una determinada propiedad.
        /// </summary>
        /// <param name="pElemOrd">Elemento ordenado</param>
        /// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        /// <param name="pListaElemOrd">Lista de elementos ordenados</param>
        /// <returns>TRUE si se ha agregado el elemento, FALSE si no</returns>
        private static bool AgregarElementoOrdenadoAlturaProp(ElementoOrdenado pElemOrd, string pNombrePropiedad, List<ElementoOrdenado> pListaElemOrd)
        {
            foreach (ElementoOrdenado elemOrd in pListaElemOrd)
            {
                if (elemOrd.NombrePropiedad.Key == pNombrePropiedad)
                {
                    bool estaAgregada = false;

                    foreach (ElementoOrdenado elemOrd2 in pListaElemOrd)
                    {
                        if (elemOrd2.NombrePropiedad.Key == pElemOrd.NombrePropiedad.Key)
                        {
                            estaAgregada = true;
                            break;
                        }
                    }

                    if (!estaAgregada)
                    {
                        pListaElemOrd.Insert(pListaElemOrd.IndexOf(elemOrd) + 1, pElemOrd);
                    }

                    return true;
                }

                bool agregado = AgregarElementoOrdenadoAlturaProp(pElemOrd, pNombrePropiedad, elemOrd.Hijos);

                if (agregado)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Apaña la repetición de entiades para que se puedan editar.
        /// </summary>
        private void ApanyarRepeticionEntidades()
        {
            List<string> tipoEntidadesAgregadas = new List<string>();
            List<ElementoOntologia> entidadesRecorrer = new List<ElementoOntologia>();

            foreach (ElementoOntologia elemPrinc in mEntidades)
            {
                foreach (ElementoOntologia entidad in mOntologia.Entidades)
                {
                    if (elemPrinc.TipoEntidad == entidad.TipoEntidad)
                    {
                        entidadesRecorrer.Add(entidad);
                        break;
                    }
                }

                foreach (string tipoeEtidad in elemPrinc.Subclases)
                {
                    foreach (ElementoOntologia entidad in mOntologia.Entidades)
                    {
                        if (entidad.TipoEntidad == tipoeEtidad)
                        {
                            entidadesRecorrer.Add(entidad);
                            break;
                        }
                    }
                }

                foreach (string tipoeEtidad in elemPrinc.SuperclasesUtiles)
                {
                    foreach (ElementoOntologia entidad in mOntologia.Entidades)
                    {
                        if (entidad.TipoEntidad == tipoeEtidad)
                        {
                            entidadesRecorrer.Add(entidad);

                            foreach (string tipoeEtidadHijas in entidad.Subclases)
                            {
                                if (tipoeEtidadHijas == elemPrinc.TipoEntidad)
                                {
                                    continue;
                                }

                                foreach (ElementoOntologia entidadHija in mOntologia.Entidades)
                                {
                                    if (entidadHija.TipoEntidad == tipoeEtidadHijas)
                                    {
                                        entidadesRecorrer.Add(entidadHija);
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }

            List<ElementoOntologia> entNuevasRec = new List<ElementoOntologia>();

            while (entidadesRecorrer.Count > 0)
            {
                foreach (ElementoOntologia entidad in entidadesRecorrer)
                {
                    ModificarTipoEntidadSiRepetida(entidad, tipoEntidadesAgregadas, entNuevasRec);
                }

                entidadesRecorrer = new List<ElementoOntologia>(entNuevasRec);
                entNuevasRec = new List<ElementoOntologia>();
            }
        }

        /// <summary>
        /// Apaña la repetición de entiades para que se puedan editar.
        /// </summary>
        /// <param name="pEntidad">Entidad a revisar</param>
        /// <param name="pTipoEntidadesAgregadas">Tipos de entidad ya agregados</param>
        /// <param name="pNuevasEntRecorrer">Nuevas entidades que hay que revisar</param>
        private void ModificarTipoEntidadSiRepetida(ElementoOntologia pEntidad, List<string> pTipoEntidadesAgregadas, List<ElementoOntologia> pNuevasEntRecorrer)
        {
            foreach (Propiedad propiedad in pEntidad.Propiedades)
            {
                if (propiedad.Tipo == TipoPropiedad.ObjectProperty && !string.IsNullOrEmpty(propiedad.Rango))
                {
                    if (pTipoEntidadesAgregadas.Contains(propiedad.Rango))
                    {
                        string nuevoTipo = propiedad.Rango + "_bis";

                        int count = 0;
                        while (pTipoEntidadesAgregadas.Contains(nuevoTipo + count))
                        {
                            count++;
                        }

                        nuevoTipo = nuevoTipo + count;

                        TipoEntidadesApanyadas.Add(nuevoTipo, propiedad.Rango);

                        mLoggingService.AgregarEntrada("FormSem Entidad apanada, nuevo tipo '" + nuevoTipo + "', antiguo '" + propiedad.Rango + "'");

                        ElementoOntologia nuevaEntidadOnto = mOntologia.GetEntidadTipo(propiedad.Rango);
                        nuevaEntidadOnto.TipoEntidad = nuevoTipo;
                        mOntologia.Entidades.Add(nuevaEntidadOnto);
                        pNuevasEntRecorrer.Add(nuevaEntidadOnto);

                        propiedad.Rango = nuevoTipo;

                        foreach (ElementoOntologia elemPrinc in mEntidades)
                        {
                            ModificarTipoEntidadDePropiedad(elemPrinc, pEntidad.TipoEntidad, propiedad.Nombre, nuevoTipo);
                        }

                        pTipoEntidadesAgregadas.Add(propiedad.Rango);
                    }
                    else
                    {
                        pTipoEntidadesAgregadas.Add(propiedad.Rango);

                        if (!propiedad.Rango.Equals(pEntidad.TipoEntidad))
                        {
                            ElementoOntologia elementoHijo = mOntologia.GetEntidadTipo(propiedad.Rango, false);
                            ModificarTipoEntidadSiRepetida(elementoHijo, pTipoEntidadesAgregadas, pNuevasEntRecorrer);
                        }
                    }
                }
            }

            if (pEntidad.TipoEntidad != mEntidades[0].TipoEntidad)
            {
                foreach (string tipoeEtidad in pEntidad.Subclases)
                {
                    foreach (ElementoOntologia entidad in mOntologia.Entidades)
                    {
                        if (entidad.TipoEntidad == tipoeEtidad)
                        {
                            pNuevasEntRecorrer.Add(entidad);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Modifica el tipo de entidad de una propiedad.
        /// </summary>
        /// <param name="pEntidad">Entidad padre</param>
        /// <param name="pTipoEntidadPadre">Tipo de la entidad padre</param>
        /// <param name="pNombreProp">Nombre propiedad</param>
        /// <param name="pNuevoTipo">Nuevo tipo prop</param>
        private void ModificarTipoEntidadDePropiedad(ElementoOntologia pEntidad, string pTipoEntidadPadre, string pNombreProp, string pNuevoTipo)
        {
            foreach (Propiedad propiedad in pEntidad.Propiedades)
            {
                if (propiedad.Tipo == TipoPropiedad.ObjectProperty && !string.IsNullOrEmpty(propiedad.Rango))
                {
                    if (propiedad.Nombre == pNombreProp && pTipoEntidadPadre == pEntidad.TipoEntidad)
                    {
                        propiedad.Rango = pNuevoTipo;

                        if (propiedad.UnicoValor.Key != null)
                        {
                            propiedad.UnicoValor.Value.TipoEntidad = pNuevoTipo;
                        }
                        else if (propiedad.ListaValores.Count > 0)
                        {
                            foreach (ElementoOntologia elementoHijo in propiedad.ListaValores.Values)
                            {
                                elementoHijo.TipoEntidad = pNuevoTipo;
                            }
                        }
                    }
                    else
                    {
                        if (propiedad.UnicoValor.Key != null)
                        {
                            ModificarTipoEntidadDePropiedad(propiedad.UnicoValor.Value, pTipoEntidadPadre, pNombreProp, pNuevoTipo);
                        }
                        else if (propiedad.ListaValores.Count > 0)
                        {
                            foreach (ElementoOntologia elementoHijo in propiedad.ListaValores.Values)
                            {
                                if (elementoHijo != null)
                                {
                                    ModificarTipoEntidadDePropiedad(elementoHijo, pTipoEntidadPadre, pNombreProp, pNuevoTipo);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Trae los nombres de las categorías de tesauro semánticas y de los autocompletar.
        /// </summary>
        private void TraerNombresCategoriasSemyAutocompletar()
        {
            mLoggingService.AgregarEntrada("FormSem Inicio SemCmsController.TraerNombresCategoriasSemyAutocompletar()");

            if (mSemRecModel.AuxiliaryCategoryTesSemNameInfo == null)
            {
                mSemRecModel.AuxiliaryCategoryTesSemNameInfo = "";
            }

            Dictionary<KeyValuePair<string, string>, object[]> datosEntidadesExternas = new Dictionary<KeyValuePair<string, string>, object[]>();

            //Preparo que datos hay que cargar:
            foreach (ElementoOntologia elemPrinc in mEntidades)
            {
                PrepararPropiedadesNombresCategoriasSemyAutocompletar(elemPrinc, datosEntidadesExternas);
            }

            //Consulto a virtuoso o el modelo acido los datos:
            new UtilSemCms(mEntityContext, mLoggingService, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).ObtenerDatosEntidadesExternas(datosEntidadesExternas, mUrlIntragnoss, mIdentidadActual.Persona.FilaPersona, mProyectoActual.FilaProyecto, Entidades);

            foreach (KeyValuePair<string, string> key in datosEntidadesExternas.Keys)
            {
                if (((SelectorEntidad)datosEntidadesExternas[key][2]).TipoSeleccion == "PersonaGnoss" || ((SelectorEntidad)datosEntidadesExternas[key][2]).TipoSeleccion == "GruposGnoss")
                {
                    Dictionary<Guid, string> idsNombres = (Dictionary<Guid, string>)datosEntidadesExternas[key][3];

                    string extraGrupo = "";

                    if (((SelectorEntidad)datosEntidadesExternas[key][2]).TipoSeleccion == "GruposGnoss")
                    {
                        extraGrupo = "g_";
                    }

                    if (idsNombres != null)
                    {
                        foreach (Guid entidadID in idsNombres.Keys)
                        {
                            if (!mSemRecModel.AuxiliaryCategoryTesSemNameInfo.Contains(extraGrupo + entidadID + "|"))
                            {
                                mSemRecModel.AuxiliaryCategoryTesSemNameInfo = string.Concat(mSemRecModel.AuxiliaryCategoryTesSemNameInfo, extraGrupo + entidadID + "|" + idsNombres[entidadID] + "|||");
                            }
                        }
                    }
                }
                else
                {
                    FacetadoDS facetadoDS = (FacetadoDS)datosEntidadesExternas[key][3];

                    if (facetadoDS != null)
                    {
                        if (((SelectorEntidad)datosEntidadesExternas[key][2]).TipoSeleccion == "Tesauro")
                        {
                            foreach (DataRow fila in facetadoDS.Tables[0].Select("p='" + ((SelectorEntidad)datosEntidadesExternas[key][2]).PropiedadesEdicion[3] + "'", "s"))
                            {
                                string sujeto = (string)fila[0];
                                if (!mSemRecModel.AuxiliaryCategoryTesSemNameInfo.Contains(sujeto + "|"))
                                {
                                    string nombreCat = SemCmsController.ObtenerNombreCatTesSem(facetadoDS, sujeto, ((SelectorEntidad)datosEntidadesExternas[key][2]).PropiedadesEdicion[3], Ontologia.IdiomaUsuario);
                                    mSemRecModel.AuxiliaryCategoryTesSemNameInfo = string.Concat(mSemRecModel.AuxiliaryCategoryTesSemNameInfo, sujeto + "|" + nombreCat + "|||");
                                }
                            }
                        }
                        else
                        {
                            #region Opciones pintado autocompletar

                            string sepPrin = null;
                            string sepFin = null;
                            string sepEntreProps = null;

                            ObtenerSeparadoresSelectorEntidad(((SelectorEntidad)datosEntidadesExternas[key][2]).ExtraWhereAutocompletar, out sepPrin, out sepFin, out sepEntreProps);

                            #endregion

                            foreach (string entidadID in (List<string>)datosEntidadesExternas[key][1])
                            {
                                if (!mSemRecModel.AuxiliaryCategoryTesSemNameInfo.Contains(entidadID + "|"))
                                {
                                    //Comprobar si tiene alguna propiedad nula
                                    string valor = UtilSemCms.ObtenerValorSegunPropsEdicion(facetadoDS, entidadID, sepPrin, sepEntreProps, sepFin, (List<string>)datosEntidadesExternas[key][0], Ontologia.IdiomaUsuario);
                                    if (string.IsNullOrEmpty(valor))
                                    {
                                        mLoggingService.GuardarLogError($"La propiedad auxiliar {entidadID} no tiene valor");
                                    }
                                    else
                                    {
                                        mSemRecModel.AuxiliaryCategoryTesSemNameInfo = string.Concat(mSemRecModel.AuxiliaryCategoryTesSemNameInfo, entidadID + "|" + valor + "|||");
                                    }                                   
                                }
                            }
                        }
                    }
                }
            }

            mLoggingService.AgregarEntrada("FormSem Fin SemCmsController.TraerNombresCategoriasSemyAutocompletar()");
        }

        /// <summary>
        /// Obtiene las propiedades que son selección de entidades externas.
        /// </summary>
        /// <param name="pEntidad">Entidad sobre la que se busca</param>
        /// <param name="pDatosEntidadesExternas">Datos de las propiedades</param>
        private void PrepararPropiedadesNombresCategoriasSemyAutocompletar(ElementoOntologia pEntidad, Dictionary<KeyValuePair<string, string>, object[]> pDatosEntidadesExternas)
        {
            foreach (Propiedad propiedad in pEntidad.Propiedades)
            {
                if (propiedad.Tipo == TipoPropiedad.ObjectProperty)
                {
                    if (propiedad.ElementoOntologia == null)
                    {
                        propiedad.ElementoOntologia = pEntidad;
                    }

                    if (propiedad.EspecifPropiedad.SelectorEntidad != null && propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion != "Edicion")
                    {
                        if (propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "Tesauro" || propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "Autocompletar" || propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "PersonaGnoss" || propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "GruposGnoss")
                        {
                            KeyValuePair<string, string> claveProp = new KeyValuePair<string, string>(propiedad.Nombre, pEntidad.TipoEntidad);
                            if (!pDatosEntidadesExternas.ContainsKey(claveProp))
                            {
                                pDatosEntidadesExternas.Add(claveProp, new object[4]);

                                if (propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "Tesauro")
                                {
                                    //Inserto la propiedad nombre de categoría (3):
                                    List<string> listaPropiedades = new List<string>();
                                    listaPropiedades.Add(propiedad.EspecifPropiedad.SelectorEntidad.PropiedadesEdicion[3]);

                                    pDatosEntidadesExternas[claveProp][0] = listaPropiedades;
                                }
                                else if (propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "Autocompletar")
                                {
                                    pDatosEntidadesExternas[claveProp][0] = propiedad.EspecifPropiedad.SelectorEntidad.PropiedadesEdicion;
                                }

                                pDatosEntidadesExternas[claveProp][1] = new List<string>();
                                pDatosEntidadesExternas[claveProp][2] = propiedad.EspecifPropiedad.SelectorEntidad;

                                mLoggingService.AgregarEntrada("FormSem Agrego SelectorEntidad PrepararPropiedadesNombresCategoriasSemyAutocompletar para '" + propiedad.Nombre + "','" + pEntidad.TipoEntidad + "'");
                            }

                            mLoggingService.AgregarEntrada("FormSem SelectorEntidad PrepararPropiedadesNombresCategoriasSemyAutocompletar con " + propiedad.ValoresUnificados.Count + " entidades externas para buscar");
                            ((List<string>)pDatosEntidadesExternas[claveProp][1]).AddRange(propiedad.ValoresUnificados.Keys);
                        }
                    }
                    else
                    {
                        foreach (ElementoOntologia entidad in propiedad.ValoresUnificados.Values)
                        {
                            if (entidad != null)
                            {
                                PrepararPropiedadesNombresCategoriasSemyAutocompletar(entidad, pDatosEntidadesExternas);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene la información del mapa de la entidad.
        /// </summary>
        /// <param name="pEntModel">Modelo de la entidad</param>
        private void ObtenerMapaGoogleEntidad(SemanticEntityModel pEntModel)
        {
            if (pEntModel.SpecificationEntity.EsMapaGoogle)
            {
                mSemRecModel.MapAgregated = true;
                pEntModel.GoogleMapInfo = new SemanticEntityModel.GoogleMap();
                pEntModel.GoogleMapInfo.JsApiGoogleKey = mConfigService.ObtenerClaveApiGoogle();

                if (!string.IsNullOrEmpty(pEntModel.SpecificationEntity.PropiedadesDatosMapa.Key))
                {
                    pEntModel.GoogleMapInfo.Latitude = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(pEntModel.SpecificationEntity.PropiedadesDatosMapa.Key, pEntModel.Entity).PrimerValorPropiedad;
                }

                if (!string.IsNullOrEmpty(pEntModel.SpecificationEntity.PropiedadesDatosMapa.Value))
                {
                    pEntModel.GoogleMapInfo.Longitude = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(pEntModel.SpecificationEntity.PropiedadesDatosMapa.Value, pEntModel.Entity).PrimerValorPropiedad;
                }

                if (!string.IsNullOrEmpty(pEntModel.SpecificationEntity.PropiedadesDatosMapaRuta.Key))
                {
                    pEntModel.GoogleMapInfo.Route = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(pEntModel.SpecificationEntity.PropiedadesDatosMapaRuta.Key, pEntModel.Entity).PrimerValorPropiedad;


                    if (pEntModel.SpecificationEntity.PropiedadesDatosMapaRuta.Value != null)
                    {
                        pEntModel.GoogleMapInfo.RouteColor = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(pEntModel.SpecificationEntity.PropiedadesDatosMapaRuta.Value, pEntModel.Entity).PrimerValorPropiedad;
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene el RDFa de una entidad.
        /// </summary>
        /// <param name="pEntModel">Modelo de la entidad</param>
        private void ObtenerRDFaEntidad(SemanticEntityModel pEntModel)
        {
            pEntModel.TypeofRDFA = pEntModel.Entity.TipoEntidadConNamespace;
            pEntModel.AboutRDFA = pEntModel.Entity.Uri;

            if (pEntModel.TypeofRDFA.Contains("http://"))
            {
                string urlName = null;
                string tipo = null;

                if (pEntModel.TypeofRDFA.Contains("#"))
                {
                    urlName = pEntModel.TypeofRDFA.Substring(0, pEntModel.TypeofRDFA.LastIndexOf("#") + 1);
                    tipo = pEntModel.TypeofRDFA.Substring(pEntModel.TypeofRDFA.LastIndexOf("#") + 1);
                }
                else if (pEntModel.TypeofRDFA.Contains("/"))
                {
                    urlName = pEntModel.TypeofRDFA.Substring(0, pEntModel.TypeofRDFA.LastIndexOf("/") + 1);
                    tipo = pEntModel.TypeofRDFA.Substring(pEntModel.TypeofRDFA.LastIndexOf("/") + 1);
                }

                if (!string.IsNullOrEmpty(urlName) && !string.IsNullOrEmpty(tipo) && Ontologia.NamespacesDefinidos.ContainsKey(urlName))
                {
                    pEntModel.TypeofRDFA = Ontologia.NamespacesDefinidos[urlName] + ":" + tipo;
                }
            }

            //foreach (string superClase in pEntModel.Entity.SuperclasesUtiles)
            //{
            //    KeyValuePair<string, string> urlEnt = GestionOWL.SepararURLDeElemento(superClase);

            //    if (urlEnt.Key != null && mOntologia.NamespacesDefinidos.ContainsKey(urlEnt.Key))
            //    {
            //        pEntModel.TypeofRDFA += " " + mOntologia.NamespacesDefinidos[urlEnt.Key] + ":" + urlEnt.Value;
            //    }
            //}
        }

        /// <summary>
        /// Obtiene los elementos hijos de la entidad.
        /// </summary>
        /// <param name="pEntModel">Modelo de la entidad</param>
        private void ObtenerHijosEntidad(SemanticEntityModel pEntModel, Guid pIdentidadID)
        {
            List<ElementoOrdenado> propOrdenadas = null;

            if (mSemRecModel.ReadMode)
            {
                if (mVistaPersonalizada)
                {
                    propOrdenadas = ObtenerElementosEntidadParaVistaPersonalizada(pEntModel);
                }
                else
                {
                    propOrdenadas = PropiedadesOrdenadasLectura(pEntModel, pIdentidadID);
                }
            }
            else
            {
                propOrdenadas = PropiedadesOrdenadas(pEntModel, pIdentidadID);
            }

            pEntModel.Properties = new List<SemanticPropertyModel>();

            foreach (ElementoOrdenado elemOrd in propOrdenadas)
            {
                SemanticPropertyModel prop = OntenerModeloElementoOrdenado(elemOrd, pEntModel, pEntModel.Depth + 1, pIdentidadID);

                if (prop != null)
                {
                    pEntModel.Properties.Add(prop);
                }
            }
        }

        /// <summary>
        /// Obtiene los elementos ordenados para la vista personalizada para una entiadad.
        /// </summary>
        /// <param name="pEntModel">Modelo de la entidad</param>
        private List<ElementoOrdenado> ObtenerElementosEntidadParaVistaPersonalizada(SemanticEntityModel pEntModel)
        {
            List<ElementoOrdenado> elemsOrd = new List<ElementoOrdenado>();

            foreach (Propiedad propiedad in pEntModel.Entity.Propiedades)
            {
                ElementoOrdenado elemOrd = new ElementoOrdenado();
                elemOrd.NombrePropiedad = new KeyValuePair<string, Propiedad>(propiedad.Nombre, propiedad);
                elemsOrd.Add(elemOrd);
            }

            return elemsOrd;
        }

        /// <summary>
        /// Obtiene el modelo Propiedad para un modelo de Entidad de un elemento ordenado.
        /// </summary>
        /// <param name="pElemOrdenado">Elemento ordenado</param>
        /// <param name="pEntModelPadre">Modelo entidad padre</param>
        /// <param name="Profundidad">Profundidad del elemento</param>
        private SemanticPropertyModel OntenerModeloElementoOrdenado(ElementoOrdenado pElemOrdenado, SemanticEntityModel pEntModelPadre, int Profundidad, Guid pIdentidadID)
        {
            try
            {
                SemanticPropertyModel semPropMod = new SemanticPropertyModel();
                semPropMod.Element = pElemOrdenado;
                semPropMod.EntityParent = pEntModelPadre;
                semPropMod.Depth = Profundidad;
                semPropMod.ReadMode = mSemRecModel.ReadMode;

                if (semPropMod.ReadMode != pEntModelPadre.ReadMode)
                {
                    semPropMod.ReadMode = pEntModelPadre.ReadMode;
                }
                else if (pElemOrdenado.NoEditable.HasValue)
                {
                    semPropMod.ReadMode = pElemOrdenado.NoEditable.Value;
                }

                bool elementoConContenido = false;

                if (pElemOrdenado.EsGrupo)
                {
                    elementoConContenido = CompletarModeloElementoOrdenadoGrupo(pElemOrdenado, semPropMod, pIdentidadID);
                }
                else if (pElemOrdenado.EsLiteral)
                {
                    elementoConContenido = CompletarModeloElementoOrdenadoLiteral(pElemOrdenado, semPropMod);
                }
                else if (pElemOrdenado.EsSelectorGrupo)
                {
                    elementoConContenido = CompletarModeloElementoOrdenadoSelectorGrupo(pElemOrdenado, semPropMod);
                }
                else if (pElemOrdenado.EsEspecial)
                {
                    elementoConContenido = CompletarModeloElementoOrdenadoEspecial(pElemOrdenado, semPropMod, pIdentidadID);
                }
                else if (pElemOrdenado.Propiedad != null)
                {
                    elementoConContenido = CompletarModeloElementoOrdenadoPropiedadSem(pElemOrdenado, semPropMod, pIdentidadID);
                }

                if (!elementoConContenido)
                {
                    return null;
                }

                return semPropMod;
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(mSemRecModel.AdminGenerationError))
                {
                    string error = "";

                    if (pElemOrdenado.Propiedad != null)
                    {
                        string idEntidad = null;
                        string tipoEntidad = null;

                        if (pElemOrdenado.Propiedad.ElementoOntologia != null)
                        {
                            idEntidad = pElemOrdenado.Propiedad.ElementoOntologia.ID;
                            tipoEntidad = pElemOrdenado.Propiedad.ElementoOntologia.TipoEntidad;
                        }

                        error = " Propiedad '" + pElemOrdenado.Propiedad.Nombre + "', Entidad '" + idEntidad + "' del Tipo '" + tipoEntidad + "'";
                    }
                    else if (pElemOrdenado.EsGrupo)
                    {
                        error = " Grupo '" + pElemOrdenado.TipoGrupo + "', id '" + pElemOrdenado.IdGrupoLectura + "', class '" + pElemOrdenado.ClaseGrupoLectura + "'";
                    }
                    else if (pElemOrdenado.EsLiteral)
                    {
                        error = " Literal '" + pElemOrdenado.NombrePropiedad.Key + "', con Link '" + pElemOrdenado.Link + "'";
                    }

                    GuardarMensajeErrorAdmin("Error relacionado con la configuración del XML o los datos de: " + error, ex);
                    GuardarLogErrorAJAX("Error docSem '" + mDocumentoID + "' en" + error + " Error: " + Environment.NewLine + ex.ToString());
                }

                throw;
            }
        }

        /// <summary>
        /// Completa el modelo de propiedad para una propiedad semántica.
        /// </summary>
        /// <param name="pElemOrdenado">Elemento ordenado del modelo de propiedad</param>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        /// <returns>TRUE si la propiedad tiene que agregarse, FALSE si no</returns>
        private bool CompletarModeloElementoOrdenadoPropiedadSem(ElementoOrdenado pElemOrdenado, SemanticPropertyModel pSemPropModel, Guid pIdentidadID)
        {
            Propiedad propiedad = pSemPropModel.Element.Propiedad;

            if (pSemPropModel.ReadMode && pSemPropModel.EntityParent.SpecificationEntity.EsMapaGoogle && pSemPropModel.EntityParent.SpecificationEntity.NoSustituirEntidadEnMapaGoogle && (pSemPropModel.EntityParent.SpecificationEntity.PropiedadesDatosMapa.Key == propiedad.Nombre || pSemPropModel.EntityParent.SpecificationEntity.PropiedadesDatosMapa.Value == propiedad.Nombre || pSemPropModel.EntityParent.SpecificationEntity.PropiedadesDatosMapaRuta.Key == propiedad.Nombre))
            {
                if (!mSemRecModel.MapAgregated)
                {
                    mSemRecModel.MapAgregated = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            propiedad.ElementoOntologia = pSemPropModel.EntityParent.Entity;

            if (pElemOrdenado.SinTitulo)
            {
                propiedad.EspecifPropiedad.NombreLectura = "";
            }

            bool pintarPropiedad = PintarPropiedad(propiedad, pSemPropModel, pIdentidadID);

            if (!pintarPropiedad)
            {
                return false;
            }

            pSemPropModel.OntologyPropInfo = new SemanticPropertyModel.OntologyProp();

            if (propiedad.Tipo == TipoPropiedad.DatatypeProperty)
            {
                AgregarDataProperty(pElemOrdenado, pSemPropModel, pIdentidadID);
            }
            else
            {//Es ObjectPropert
                if (propiedad.EspecifPropiedad.SelectorEntidad != null && (pSemPropModel.ReadMode || propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion != "Edicion"))
                {
                    return AgregarObjectSelectorEntidad(pElemOrdenado, pSemPropModel, pIdentidadID);
                }
                else
                {
                    return AgregarObjectProperty(pElemOrdenado, pSemPropModel, pIdentidadID);
                }
            }

            return true;
        }

        #region Selector Entidad ObjetProperty

        /// <summary>
        /// Agrega una propiedad de tipo objeto que es selector de entidad.
        /// </summary>
        /// <param name="pElemOrdenado">Elemento ordenado del modelo de propiedad</param>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        private bool AgregarObjectSelectorEntidad(ElementoOrdenado pElemOrdenado, SemanticPropertyModel pSemPropModel, Guid pIdentidadID)
        {
            Propiedad propiedad = pSemPropModel.Element.Propiedad;
            pSemPropModel.OntologyPropInfo.UniqueValue = (propiedad.FunctionalProperty || propiedad.CardinalidadMenorOIgualUno);
            pSemPropModel.OntologyPropInfo.MinCardinality = propiedad.CardinalidadMinima;
            pSemPropModel.OntologyPropInfo.MaxCardinality = propiedad.CardinalidadMaxima;
            pSemPropModel.OntologyPropInfo.FunctionalProperty = propiedad.FunctionalProperty;
            pSemPropModel.OntologyPropInfo.EntitySelector = new SemanticPropertyModel.EntitySelector();

            if (propiedad.EspecifPropiedad.ValorDefectoNoSeleccionable != null)
            {
                pSemPropModel.OntologyPropInfo.DefaultUnselectableValue = UtilCadenas.ObtenerTextoDeIdioma(propiedad.EspecifPropiedad.ValorDefectoNoSeleccionable, Ontologia.IdiomaUsuario, null);
            }

            if (!pSemPropModel.ReadMode)
            {
                pSemPropModel.OntologyPropInfo.ControlID = "selEnt_" + propiedad.ElementoOntologia.TipoEntidadGeneracionIDs + propiedad.NombreGeneracionIDs;
                RegistrarIDControlPropiedad(propiedad, pSemPropModel.OntologyPropInfo.ControlID);
                GenerarDatosEdicionSelectorEntidad(pSemPropModel);
            }
            else
            {
                GenerarDatosLecturaSelectorEntidad(pSemPropModel, pIdentidadID);
            }

            DarTextoCorrectoTituloProp(pSemPropModel);
            AgregarRDFAYMicrodatos(pSemPropModel);

            //Por si hay alguna entidad externaque no trae datos.
            return (!pSemPropModel.ReadMode || (pSemPropModel.PropertyValues != null && pSemPropModel.PropertyValues.Count > 0) || (pSemPropModel.OntologyPropInfo.EntitySelector != null && pSemPropModel.OntologyPropInfo.EntitySelector.LinkedResources != null && pSemPropModel.OntologyPropInfo.EntitySelector.LinkedResources.Count > 0));
        }

        /// <summary>
        /// Genera los datos de lectura para la propiedad selector de entidad.
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        private void GenerarDatosLecturaSelectorEntidad(SemanticPropertyModel pSemPropModel, Guid pIdentidadID)
        {
            if (mDatosEntidadesExternas == null)
            {
                ObtenerEntidadesExternas();
            }

            AjustarPaginacionSelector(pSemPropModel);

            if (pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "UrlRecurso")
            {
                GenerarDatosLecturaSelectorEntidadUrlsRecurso(pSemPropModel);
            }
            else if (pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "PersonaGnoss" || pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "GruposGnoss")
            {
                GenerarDatosLecturaSelectorEntidadPersonasYGruposGnoss(pSemPropModel);
            }
            else
            {
                GenerarDatosLecturaSelectorEntidadVirtuoso(pSemPropModel, pIdentidadID);
            }
        }

        /// <summary>
        /// Ajusta la paginación del selector.
        /// </summary>
        /// <param name="pSemPropModel">Selector</param>
        private void AjustarPaginacionSelector(SemanticPropertyModel pSemPropModel)
        {
            if (pSemPropModel.SpecificationProperty.SelectorEntidad.NumElemPorPag > 0 && pSemPropModel.Element.Propiedad.ListaValores.Count > 0)
            {
                List<string> valores = new List<string>(pSemPropModel.Element.Propiedad.ListaValores.Keys);
                pSemPropModel.Element.Propiedad.LimpiarValor();

                int inicio = 0;

                if (PropiedadCallbackTraerMas.Key != null && pSemPropModel.Element.Propiedad.Nombre == PropiedadCallbackTraerMas.Key && pSemPropModel.Element.Propiedad.ElementoOntologia.ID == PropiedadCallbackTraerMas.Value.Key)
                {
                    inicio = PropiedadCallbackTraerMas.Value.Value;
                }

                int fin = inicio + pSemPropModel.SpecificationProperty.SelectorEntidad.NumElemPorPag;

                for (int i = inicio; i < fin; i++)
                {
                    if (i < valores.Count)
                    {
                        pSemPropModel.Element.Propiedad.ListaValores.Add(valores[i], null);
                    }
                }

                if (inicio == 0 && valores.Count > fin)
                {
                    pSemPropModel.OntologyPropInfo.EntitySelector.NumEntitiesForPage = fin;
                    pSemPropModel.OntologyPropInfo.EntitySelector.TotalEntitiesPagination = valores.Count;
                }
            }
        }

        /// <summary>
        /// Genera los datos de lectura para la propiedad selector de entidad de datos de Virtuoso.
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        private void GenerarDatosLecturaSelectorEntidadVirtuoso(SemanticPropertyModel pSemPropModel, Guid pIdentidadID)
        {
            FacetadoDS facetadoDS = null;
            EstiloPlantillaEspecifProp estiloPropAux = null;
            Ontologia ontologiaAux = null;

            List<string> entidadesPintar = UtilSemCms.GenerarOntologiaAuxiliarEntExternas(pSemPropModel.Element.Propiedad, mDatosEntidadesExternas, null, Ontologia.IdiomaUsuario, out facetadoDS, out estiloPropAux, out ontologiaAux);

            if (!mVistaPersonalizada && pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "Tesauro")
            {
                if (pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad.TipoPresentacion == "Arbol")
                {
                    GenerarDatosLecturaArbolEntidadesTesSem(pSemPropModel, facetadoDS);
                }
                else
                {
                    GenerarDatosLecturaEntidadesTesSem(pSemPropModel, facetadoDS);
                }
            }
            else
            {
                if (pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "Tesauro")
                {//Ordeno las categorías de padre raiz hasta el hijo hoja
                    string ordenValoresTesuroSemantico = ObtenerOrdenValoresTesauroSemantico(facetadoDS, pSemPropModel.Element.Propiedad.ValoresUnificados, pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad);
                    entidadesPintar.Clear();

                    foreach (string entidadVal in ordenValoresTesuroSemantico.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        entidadesPintar.Add(entidadVal);
                    }
                }

                pSemPropModel.OntologyPropInfo.PropertyValues = new List<SemanticPropertyModel.PropertyValue>();

                foreach (string entidadID in entidadesPintar)
                {
                    ElementoOntologia instanciaEntidad = ontologiaAux.GetEntidadTipo(estiloPropAux.NombreEntidad, true);
                    instanciaEntidad.ID = entidadID;

                    foreach (EstiloPlantillaEspecifProp estilo in pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad.PropiedadesLectura)
                    {
                        GenerarDatosPropiedadEHijosSelEnt(pSemPropModel, facetadoDS, entidadID, estilo, instanciaEntidad, pIdentidadID);
                    }

                    if ((pSemPropModel.SpecificationProperty.SelectorEntidad.SoloIdiomaUsuario || !string.IsNullOrEmpty(PropiedadIdiomaBusquedaComunidad)) && (!instanciaEntidad.IdiomaUsuarioEnAlgunaPropiedad && instanciaEntidad.IdiomaNoUsuarioEnAlgunaPropiedad))
                    {
                        continue;
                    }

                    SemanticPropertyModel.PropertyValue propertyValueNuevaInst = new SemanticPropertyModel.PropertyValue();
                    propertyValueNuevaInst.Property = pSemPropModel;

                    propertyValueNuevaInst.Value = instanciaEntidad.ID;
                    propertyValueNuevaInst.RelatedEntity = ObtenerModeloEntidad(instanciaEntidad, pSemPropModel.Depth, pSemPropModel.Element.Propiedad.ElementoOntologia, pSemPropModel, pIdentidadID);
                    pSemPropModel.OntologyPropInfo.PropertyValues.Add(propertyValueNuevaInst);

                    if (instanciaEntidad.ContienePropiedadTesSemArbol && !mVistaPersonalizada)
                    {//Con pintar el árbol una vez vale, si no va a pintar un árbol por cada path del de nodos que haya
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Genera los datos de lectura para una propiedad de una entidad externa del selector de entidad de datos de Virtuoso.
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        /// <param name="pFacetadoDS">Dataset con los datos</param>
        /// <param name="pEntidadID">ID de la entidad externa</param>
        /// <param name="pEstiloProp">Estilo de la propiedad</param>
        /// <param name="pEntidadAux">Entidad externa auxiliar</param>
        private void GenerarDatosPropiedadEHijosSelEnt(SemanticPropertyModel pSemPropModel, FacetadoDS pFacetadoDS, string pEntidadID, EstiloPlantillaEspecifProp pEstiloProp, ElementoOntologia pEntidadAux, Guid pIdentidadID)
        {
            if (pEstiloProp.PropiedadesAuxiliares != null && pEstiloProp.PropiedadesAuxiliares.Count > 0)
            {
                Propiedad propiedad = pEntidadAux.ObtenerPropiedad(pEstiloProp.NombreRealPropiedad);
                propiedad.ElementoOntologia = pEntidadAux;

                ElementoOrdenado elemOrd = pEstiloProp.ElementoOrdenadoAuxiliar;

                if (elemOrd == null)
                {
                    elemOrd = new ElementoOrdenado();
                }

                elemOrd.NombrePropiedad = new KeyValuePair<string, Propiedad>(propiedad.Nombre, propiedad);
                pEntidadAux.EspecifEntidad.ElementosOrdenadosLectura.Add(elemOrd);

                DataRow[] filasProp = pFacetadoDS.Tables[0].Select("s='" + pEntidadID + "' AND p='" + pEstiloProp.NombreRealPropiedad + "'");

                foreach (DataRow filaProp in filasProp)
                {
                    string entidadHijaID = (string)filaProp["o"];
                    ElementoOntologia instanciaEntidad = pEntidadAux.Ontologia.GetEntidadTipo(propiedad.Rango, true);
                    instanciaEntidad.ID = entidadHijaID;
                    instanciaEntidad.EspecifEntidad.NombreLectura = "";
                    propiedad.ListaValores.Add(entidadHijaID, instanciaEntidad);
                    pEntidadAux.EntidadesRelacionadas.Add(instanciaEntidad);

                    foreach (EstiloPlantillaEspecifProp estiloHijo in pEstiloProp.PropiedadesAuxiliares)
                    {
                        GenerarDatosPropiedadEHijosSelEnt(pSemPropModel, pFacetadoDS, entidadHijaID, estiloHijo, instanciaEntidad, pIdentidadID);
                    }
                }
            }
            else
            {
                GenerarDatosPropiedadSelEnt(pSemPropModel, pFacetadoDS, pEntidadID, pEstiloProp, pEntidadAux, pIdentidadID);
            }
        }

        /// <summary>
        /// Genera los datos de lectura para una propiedad de una entidad externa del selector de entidad de datos de Virtuoso.
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        /// <param name="pFacetadoDS">Dataset con los datos</param>
        /// <param name="pEntidadID">ID de la entidad externa</param>
        /// <param name="pEstiloProp">Estilo de la propiedad</param>
        /// <param name="pEntidadAux">Entidad externa auxiliar</param>
        private void GenerarDatosPropiedadSelEnt(SemanticPropertyModel pSemPropModel, FacetadoDS pFacetadoDS, string pEntidadID, EstiloPlantillaEspecifProp pEstiloProp, ElementoOntologia pEntidadAux, Guid pIdentidadID)
        {
            if (pEstiloProp.NombreRealPropiedad == null)
            {
                GenerarDatosElemOrdSelEnt(pSemPropModel, pEstiloProp, pEntidadAux, pIdentidadID);
                return;
            }

            string select = "s='" + pEntidadID + "' AND (p='" + pEstiloProp.NombreRealPropiedad + "')";

            DataRow[] filasProp = pFacetadoDS.Tables[0].Select(select);
            bool propEdicion = (pEstiloProp.ElementoOrdenadoAuxiliar != null && pEstiloProp.ElementoOrdenadoAuxiliar.NoEditable.HasValue && !pEstiloProp.ElementoOrdenadoAuxiliar.NoEditable.Value);

            if (filasProp.Length > 0 || propEdicion)
            {
                Propiedad propiedad = pEntidadAux.ObtenerPropiedad(pEstiloProp.NombreRealPropiedad);
                propiedad.ElementoOntologia = pEntidadAux;

                string idiomaUsado;
                List<string> listaValores = FacetadoCN.ObtenerObjetosDataSetSegunPropiedad(pFacetadoDS, pEntidadID, pEstiloProp.NombreRealPropiedad, Ontologia.IdiomaUsuario, out idiomaUsado);

                if (listaValores.Count > 0 || propEdicion)
                {
                    if (idiomaUsado == Ontologia.IdiomaUsuario)
                    {
                        pEntidadAux.IdiomaUsuarioEnAlgunaPropiedad = true;
                    }
                    else if (!string.IsNullOrEmpty(idiomaUsado))
                    {
                        pEntidadAux.IdiomaNoUsuarioEnAlgunaPropiedad = true;
                    }

                    if (listaValores.Count == 1)
                    {
                        string valor = listaValores[0];

                        if (pSemPropModel.Element.Propiedad.ElementoOntologia.EspecifEntidad.CampoRepresentanteOrden != null && pSemPropModel.Element.Propiedad.ElementoOntologia.EspecifEntidad.CampoRepresentanteOrden == pSemPropModel.Element.Propiedad.Nombre)
                        {
                            valor = pSemPropModel.Element.Propiedad.ElementoOntologia.OrdenEntiadTexto + valor;
                        }

                        propiedad.FunctionalProperty = true;
                        propiedad.UnicoValor = new KeyValuePair<string, ElementoOntologia>(valor, null);
                    }
                    else
                    {
                        foreach (string valor in listaValores)
                        {
                            if (!propiedad.ListaValores.ContainsKey(valor))
                            {
                                propiedad.ListaValores.Add(valor, null);
                            }
                        }
                    }

                    ElementoOrdenado elemOrd = pEstiloProp.ElementoOrdenadoAuxiliar;

                    if (elemOrd == null)
                    {
                        elemOrd = new ElementoOrdenado();
                    }

                    elemOrd.NombrePropiedad = new KeyValuePair<string, Propiedad>(propiedad.Nombre, propiedad);
                    pEntidadAux.EspecifEntidad.ElementosOrdenadosLectura.Add(elemOrd);

                    AgregarLinkARecursoAProp(pSemPropModel, propiedad, pEntidadID);

                    if (listaValores.Count > 0 && pEstiloProp.SelectorEntidad != null)
                    {
                        propiedad.EspecifPropiedad.EsSelectorEntidadInterno = true;
                        string claveMerged = string.Concat(propiedad.Nombre, propiedad.ElementoOntologia.TipoEntidad, pSemPropModel.Element.Propiedad.Nombre, pSemPropModel.Element.Propiedad.ElementoOntologia.TipoEntidad);

                        if (mDataSetsEntidadesExternasMerged == null)
                        {
                            mDataSetsEntidadesExternasMerged = new List<string>();
                        }

                        //Evitamos que se haga el Merge muchas veces de los mismos dataSet:
                        if (!mDataSetsEntidadesExternasMerged.Contains(claveMerged))
                        {
                            KeyValuePair<string, string> claveDatosEnt = new KeyValuePair<string, string>(propiedad.Nombre + "_AuxEntExt", propiedad.ElementoOntologia.TipoEntidad + "_AuxEntExt");

                            if (mDatosEntidadesExternas.ContainsKey(claveDatosEnt))
                            {
                                if (mDatosEntidadesExternas[claveDatosEnt][3] != null)
                                {
                                    ((FacetadoDS)mDatosEntidadesExternas[claveDatosEnt][3]).Merge(pFacetadoDS);
                                }
                                else
                                {
                                    mDatosEntidadesExternas[claveDatosEnt][3] = pFacetadoDS;
                                }
                            }
                            else
                            {
                                mDatosEntidadesExternas.Add(claveDatosEnt, new object[] { null, null, null, pFacetadoDS });
                            }

                            mDataSetsEntidadesExternasMerged.Add(claveMerged);
                        }
                    }
                }
            }
            else
            {
                mLoggingService.AgregarEntrada("FormSem Consulta SelectorEntidad no devuelve datos. Consulta s='" + pEntidadID + "' AND p='" + pEstiloProp.NombreRealPropiedad + "', propiedad '" + pSemPropModel.Element.Propiedad.Nombre + "', entidad '" + pSemPropModel.Element.Propiedad.ElementoOntologia.ID + "', tipoEntidad '" + pSemPropModel.Element.Propiedad.ElementoOntologia.TipoEntidad + "'");
            }
        }

        /// <summary>
        /// Genera los datos de un elemento ordenado para un selector de entidad.
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        /// <param name="pEstiloProp">Estilo de la propiedad</param>
        /// <param name="pEntidadAux">Entidad externa auxiliar</param>
        private void GenerarDatosElemOrdSelEnt(SemanticPropertyModel pSemPropModel, EstiloPlantillaEspecifProp pEstiloProp, ElementoOntologia pEntidadAux, Guid pIdentidadID)
        {
            pEntidadAux.EspecifEntidad.ElementosOrdenadosLectura.Add(pEstiloProp.ElementoOrdenadoAuxiliar);

            if (pEstiloProp.ElementoOrdenadoAuxiliar.EsEspecial)
            {
                CompletarModeloElementoOrdenadoEspecial(pEstiloProp.ElementoOrdenadoAuxiliar, pSemPropModel, pIdentidadID);
            }
        }

        /// <summary>
        /// Agrega el link al recurso si está configurado.
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        /// <param name="pPropiedad">Propiedad</param>
        /// <param name="pEntidadID">Entidad</param>
        private void AgregarLinkARecursoAProp(SemanticPropertyModel pSemPropModel, Propiedad pPropiedad, string pEntidadID)
        {
            if (pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad.LinkARecurso)
            {
                string idRec = pEntidadID.Substring(0, pEntidadID.LastIndexOf("_"));
                string tipoEntidad = idRec.Substring(idRec.LastIndexOf("/") + 1);
                idRec = idRec.Substring(idRec.LastIndexOf("_") + 1);
                tipoEntidad = tipoEntidad.Substring(0, tipoEntidad.LastIndexOf("_"));

                string ontologia = pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad.Grafo;
                if (ontologia.Contains("/"))
                {
                    ontologia = ontologia.Substring(ontologia.LastIndexOf("/") + 1);
                }
                if (ontologia.Contains("#"))
                {
                    ontologia = ontologia.Substring(ontologia.LastIndexOf("#") + 1);
                }

                if ((pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad.TipoEntLinkARecurso == null || pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad.TipoEntLinkARecurso.Contains(tipoEntidad)) && (pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad.PropLinkARecurso == null || pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad.PropLinkARecurso.Contains(pPropiedad.Nombre)))
                {
                    if (pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad.LinkARecursoVaAComunidad)
                    {
                        pPropiedad.EspecifPropiedad.UrlLinkDelValor = UrlsSemanticas.GetURLBaseRecursosFichaConIDs(mBaseURLIdioma, mUtilIdiomas, mProyectoActual.NombreCorto, null, UtilCadenas.EliminarCaracteresUrlSem(pPropiedad.PrimerValorPropiedad), ontologia, new Guid(idRec), false);
                    }
                    else
                    {
                        pPropiedad.EspecifPropiedad.UrlLinkDelValor = mBaseUrl + "/" + mUtilIdiomas.GetText("URLSEM", "RECURSOINVITADO") + "/" + UtilCadenas.EliminarCaracteresUrlSem(pPropiedad.PrimerValorPropiedad) + "/" + idRec;
                    }

                    pPropiedad.EspecifPropiedad.NuevaPestanya = pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad.NuevaPestanya;
                }
            }
        }


        /// <summary>
        /// Genera los datos para las entidades del selector de entidades de tesauro semántico en formato árbol.
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        /// <param name="pFacetadoDS">DataSet con los datos de las entidades externas</param>
        private void GenerarDatosLecturaArbolEntidadesTesSem(SemanticPropertyModel pSemPropModel, FacetadoDS pFacetadoDS)
        {
            Propiedad propiedad = pSemPropModel.Element.Propiedad;
            //Localizo la propiedad que contiene la entidad que tiene la propiedad actual:
            Propiedad propiedadPadre = EstiloPlantilla.ObtenerPropiedadContieneEntidad(new List<ElementoOntologia>(mEntidades), propiedad.ElementoOntologia);

            Dictionary<string, KeyValuePair<string, SortedDictionary<string, List<string>>>> padreHijos = new Dictionary<string, KeyValuePair<string, SortedDictionary<string, List<string>>>>();
            List<string> padres = new List<string>();

            if (propiedadPadre != null)
            {
                foreach (ElementoOntologia entidadHija in propiedadPadre.ValoresUnificados.Values)
                {
                    Propiedad propiedadHija = entidadHija.ObtenerPropiedad(propiedad.Nombre);
                    FacetadoDS facetadoDS = null;

                    if (propiedadHija == propiedad)
                    {
                        facetadoDS = pFacetadoDS;
                    }
                    else
                    {
                        EstiloPlantillaEspecifProp estiloPropAux = null;
                        Ontologia ontologiaAux = null;

                        UtilSemCms.GenerarOntologiaAuxiliarEntExternas(propiedad, mDatosEntidadesExternas, pFacetadoDS, Ontologia.IdiomaUsuario, out facetadoDS, out estiloPropAux, out ontologiaAux);
                    }

                    ExtraerPadresPintarArbolTesSem(pSemPropModel, pFacetadoDS, facetadoDS, propiedadHija.ValoresUnificados, propiedadHija.EspecifPropiedad.SelectorEntidad, padreHijos, padres);
                }
            }
            else //Intento extaer la relación de los datos
            {
                List<string> sujetosEntPadres = FacetadoCN.ObtenerSujetosDataSetSegunPropiedad(pFacetadoDS, propiedad.Nombre);

                foreach (string sujetoPadre in sujetosEntPadres)
                {
                    List<string> listaValores = FacetadoCN.ObtenerObjetosDataSetSegunPropiedad(pFacetadoDS, sujetoPadre, propiedad.Nombre);
                    Dictionary<string, ElementoOntologia> valoresProp = new Dictionary<string, ElementoOntologia>();

                    foreach (string valor in listaValores)
                    {
                        valoresProp.Add(valor, null);
                    }

                    ExtraerPadresPintarArbolTesSem(pSemPropModel, pFacetadoDS, pFacetadoDS, valoresProp, propiedad.EspecifPropiedad.SelectorEntidad, padreHijos, padres);
                }
            }

            if (padres.Count > 0)
            {
                pSemPropModel.OntologyPropInfo.PropertyValues = new List<SemanticPropertyModel.PropertyValue>();

                foreach (string padre in padres)
                {
                    GenerarDatosElementoArbolTesSem(pSemPropModel, padre, padreHijos, pSemPropModel.OntologyPropInfo.PropertyValues);
                }
            }
        }

        /// <summary>
        /// Genera los datos de un elemento al árbol de tesauro semántico.
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        /// <param name="pElemento">Elemento</param>
        /// <param name="pElementos">Elementos con sus relaciones y nombres</param>
        /// <param name="pContenedorValoresProp">Contenedor sobre el que hay agregar los valores que representan categorías semánticas</param>
        private void GenerarDatosElementoArbolTesSem(SemanticPropertyModel pSemPropModel, string pElemento, Dictionary<string, KeyValuePair<string, SortedDictionary<string, List<string>>>> pElementos, List<SemanticPropertyModel.PropertyValue> pContenedorValoresProp)
        {
            SemanticPropertyModel.PropertyValue propertyValueNuevaInst = new SemanticPropertyModel.PropertyValue();
            propertyValueNuevaInst.Property = pSemPropModel;

            propertyValueNuevaInst.Value = pElementos[pElemento].Key;
            pContenedorValoresProp.Add(propertyValueNuevaInst);

            if (pSemPropModel.Element.Propiedad.EspecifPropiedad.ElementoOrdenadoAuxiliar != null && pSemPropModel.Element.Propiedad.EspecifPropiedad.ElementoOrdenadoAuxiliar.Link != null)
            {
                propertyValueNuevaInst.UrlAuxiliaryLinkControl = ObtenerLinkABusquedaFiltrada(pElemento, pSemPropModel.SpecificationProperty.ElementoOrdenadoAuxiliar.Link, null);
            }

            if (pElementos[pElemento].Value.Count > 0)
            {
                propertyValueNuevaInst.ThesaurusSemanticTreeChildren = new List<SemanticPropertyModel.PropertyValue>();

                foreach (string idHijo in pElementos[pElemento].Value.Keys)
                {
                    foreach (string hijo in pElementos[pElemento].Value[idHijo])
                    {
                        GenerarDatosElementoArbolTesSem(pSemPropModel, hijo, pElementos, propertyValueNuevaInst.ThesaurusSemanticTreeChildren);
                    }
                }
            }
        }

        /// <summary>
        /// Extrae los padres para montar los datos de un árbol semántico.
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        /// <param name="pFacetadoDS">DataSet con los datos de las entidades externas</param>
        /// <param name="pFacetadoAuxDS">DataSet auxiliar con los datos de las entidades externas</param>
        /// <param name="pValoresProp">Valores con las entidades categoria</param>
        /// <param name="pSelectorEntidad">Selector de entidad</param>
        /// <param name="pPadreHijos">Lista con Key: ID de cada categoría, Value: el nombre de la categorías y los IDs de sus hijos.</param>
        /// <param name="pPadres">IDs de las categorías padre raiz</param>
        private void ExtraerPadresPintarArbolTesSem(SemanticPropertyModel pSemPropModel, FacetadoDS pFacetadoDS, FacetadoDS pFacetadoAuxDS, Dictionary<string, ElementoOntologia> pValoresProp, SelectorEntidad pSelectorEntidad, Dictionary<string, KeyValuePair<string, SortedDictionary<string, List<string>>>> pPadreHijos, List<string> pPadres)
        {
            string ordenValoresTesuroSemantico = ObtenerOrdenValoresTesauroSemantico(pFacetadoAuxDS, pValoresProp, pSelectorEntidad);
            string padreAnterior = null;

            foreach (string elemento in ordenValoresTesuroSemantico.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string identificador = ObtenerNombreCatTesSem(pFacetadoDS, elemento, pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad.PropiedadesEdicion[2], Ontologia.IdiomaUsuario);

                string nombreCat = ObtenerNombreCatTesSem(pFacetadoDS, elemento, pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad.PropiedadesEdicion[3], Ontologia.IdiomaUsuario);

                if (string.IsNullOrEmpty(nombreCat))
                {
                    break;
                }

                if (!pPadreHijos.ContainsKey(elemento))
                {
                    pPadreHijos.Add(elemento, new KeyValuePair<string, SortedDictionary<string, List<string>>>(nombreCat, new SortedDictionary<string, List<string>>()));
                }

                if (padreAnterior != null)
                {
                    if (!pPadreHijos[padreAnterior].Value.ContainsKey(identificador))
                    {
                        pPadreHijos[padreAnterior].Value.Add(identificador, new List<string>());
                    }

                    if (!pPadreHijos[padreAnterior].Value[identificador].Contains(elemento))
                    {
                        pPadreHijos[padreAnterior].Value[identificador].Add(elemento);
                    }
                }
                else if (!pPadres.Contains(elemento))
                {
                    pPadres.Add(elemento);
                }

                padreAnterior = elemento;
            }
        }

        /// <summary>
        /// Genera los datos para las entidades del selector de tipo tesuaro semántico.
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        /// <param name="pFacetadoDS">DataSet con los datos de las entidades externas</param>
        public void GenerarDatosLecturaEntidadesTesSem(SemanticPropertyModel pSemPropModel, FacetadoDS pFacetadoDS)
        {
            pSemPropModel.OntologyPropInfo.PropertyValues = new List<SemanticPropertyModel.PropertyValue>();
            string[] ordenValoresTesuroSemantico = ObtenerOrdenValoresTesauroSemantico(pFacetadoDS, pSemPropModel.Element.Propiedad.ValoresUnificados, pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad.TipoPresentacion == "NodosHoja" && ordenValoresTesuroSemantico.Length > 1)
            {
                ordenValoresTesuroSemantico = new string[] { ordenValoresTesuroSemantico[ordenValoresTesuroSemantico.Length - 1] };//Nos quedamos solo con la última categoría, la hoja.
            }

            foreach (string entidadVal in ordenValoresTesuroSemantico)
            {
                string nombreCat = ObtenerNombreCatTesSem(pFacetadoDS, entidadVal, pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad.PropiedadesEdicion[3], Ontologia.IdiomaUsuario);

                SemanticPropertyModel.PropertyValue propertyValueNuevaInst = new SemanticPropertyModel.PropertyValue();
                propertyValueNuevaInst.Property = pSemPropModel;

                propertyValueNuevaInst.Value = nombreCat;
                pSemPropModel.OntologyPropInfo.PropertyValues.Add(propertyValueNuevaInst);
            }
        }

        /// <summary>
        /// Genera los datos de lectura para la propiedad selector de entidad de tipo "UrlRecurso".
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        private void GenerarDatosLecturaSelectorEntidadUrlsRecurso(SemanticPropertyModel pSemPropModel)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = null;
            Dictionary<Guid, string> listaRec = new UtilSemCms(mEntityContext, mLoggingService, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mServicesUtilVirtuosoAndReplication).GenerarOntologiaAuxiliarDocumentosExternos(pSemPropModel.Element.Propiedad, mDatosEntidadesExternas, out dataWrapperDocumentacion);

            GestorDocumental gestorDocumental = new GestorDocumental(dataWrapperDocumentacion, mLoggingService, mEntityContext);
            pSemPropModel.OntologyPropInfo.EntitySelector.LinkedResources = new List<SemanticPropertyModel.ResourceLinkedToEntitySelector>();

            foreach (Guid docID in listaRec.Keys)
            {
                List<AD.EntityModel.Models.Documentacion.Documento> filasDoc = dataWrapperDocumentacion.ListaDocumento.Where(doc => doc.DocumentoID.Equals(docID)).ToList();

                if (filasDoc.Count == 0)
                {
                    continue;
                }

                SemanticPropertyModel.ResourceLinkedToEntitySelector resourceLink = new SemanticPropertyModel.ResourceLinkedToEntitySelector();
                pSemPropModel.OntologyPropInfo.EntitySelector.LinkedResources.Add(resourceLink);
                Documento Documento = gestorDocumental.ListaDocumentos[docID];
                AD.EntityModel.Models.Documentacion.Documento filaDoc = filasDoc.FirstOrDefault();
                var filaDocBR = gestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.FirstOrDefault(doc => doc.DocumentoID.Equals(docID) && doc.BaseRecursosID.Equals(BaseRecursosProyectoSeleccionado));

                resourceLink.Key = filaDoc.DocumentoID;

                if (filaDoc.ProyectoID == mProyectoActual.Clave || (filaDocBR != null && !filaDocBR.Eliminado && !filaDocBR.LinkAComunidadOrigen))
                {
                    resourceLink.Link = UrlsSemanticas.GetURLBaseRecursosFichaConIDs(mBaseURLIdioma, mUtilIdiomas, mProyectoActual.NombreCorto, null, UtilCadenas.EliminarCaracteresUrlSem(filaDoc.Titulo), filaDoc.DocumentoID, null, false);
                }
                else
                {
                    ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    resourceLink.Link = UrlsSemanticas.GetURLBaseRecursosFichaConIDs(mBaseURLIdioma, mUtilIdiomas, proyectoCL.ObtenerNombreCortoProyecto(filaDoc.ProyectoID.Value), null, UtilCadenas.EliminarCaracteresUrlSem(filaDoc.Titulo), filaDoc.DocumentoID, null, false);
                    //resourceLink.Link = GnossUrlsSemanticas.GetURLBaseRecursosRecursoInvitadoConIDS(mBaseURLIdioma, null, mUtilIdiomas, UtilCadenas.EliminarCaracteresUrlSem(filaDoc.Titulo), filaDoc.DocumentoID);
                }

                foreach (EstiloPlantillaEspecifProp estiloAtrRec in pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad.PropiedadesLectura)
                {
                    estiloAtrRec.Propiedad = pSemPropModel.Element.Propiedad;

                    switch (estiloAtrRec.NombreRealPropiedad)
                    {
                        case "Titulo":
                            if (!string.IsNullOrEmpty(estiloAtrRec.NombreLectura))
                            {
                                resourceLink.TitleLabel = estiloAtrRec.NombreLectura;
                            }

                            resourceLink.Title = UtilCadenas.ObtenerTextoDeIdioma(filaDoc.Titulo, Ontologia.IdiomaUsuario, null);

                            if (pSemPropModel.Element.Propiedad.ElementoOntologia.EspecifEntidad.CampoRepresentanteOrden != null && pSemPropModel.Element.Propiedad.ElementoOntologia.EspecifEntidad.CampoRepresentanteOrden == pSemPropModel.Element.Propiedad.Nombre)
                            {
                                resourceLink.Title = pSemPropModel.Element.Propiedad.ElementoOntologia.OrdenEntiadTexto + resourceLink.Title;
                            }

                            break;
                        case "Imagen":
                            if (!string.IsNullOrEmpty(estiloAtrRec.NombreLectura))
                            {
                                resourceLink.ImageUrlLabel = estiloAtrRec.NombreLectura;
                            }

                            #region Foto

                            string UrlImagenMini = "";

                            if (Documento != null && (Documento.TipoDocumentacion == TiposDocumentacion.Hipervinculo || Documento.TipoDocumentacion == TiposDocumentacion.Video || Documento.TipoDocumentacion == TiposDocumentacion.VideoBrightcove || Documento.TipoDocumentacion == TiposDocumentacion.FicheroServidor || Documento.TipoDocumentacion == TiposDocumentacion.Nota))
                            {
                                string fileName = $"{HttpUtility.UrlEncode(Documento.Clave.ToString())}.jpg";
                                if (Documento.FilaDocumento.VersionFotoDocumento.HasValue && Documento.FilaDocumento.VersionFotoDocumento > 0)
                                {
                                    UrlImagenMini = $"{mBaseURLContent}/{UtilArchivos.ContentImagenesEnlaces}/{UtilArchivos.DirectorioDocumento(Documento.Clave)}/{fileName}?{Documento.FilaDocumento.VersionFotoDocumento}";
                                }
                            }
                            else if (Documento != null && (Documento.TipoDocumentacion == TiposDocumentacion.Imagen))
                            {
                                string fileName = $"{HttpUtility.UrlEncode(Documento.Clave.ToString().ToLower())}_peque.jpg";
                                if (Documento.FilaDocumento.VersionFotoDocumento.HasValue && Documento.FilaDocumento.VersionFotoDocumento > 0)
                                {
                                    UrlImagenMini = $"{mBaseURLContent}/{UtilArchivos.ContentImagenes}/{UtilArchivos.ContentImagenesDocumentos}/Miniatura/{UtilArchivos.DirectorioDocumento(Documento.Clave)}/{fileName}?{Documento.FilaDocumento.VersionFotoDocumento}";
                                }
                            }
                            else if (Documento != null && (Documento.TipoDocumentacion == TiposDocumentacion.Semantico) && !string.IsNullOrEmpty(Documento.FilaDocumento.NombreCategoriaDoc))
                            {
                                string fileName = Documento.FilaDocumento.NombreCategoriaDoc;

                                string[] elems = fileName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                                string extension = "." + elems.Last();

                                if (fileName.Contains(extension))
                                {
                                    UrlImagenMini = $"{mBaseURLContent}/{fileName.Substring(fileName.LastIndexOf(",") + 1).Replace(extension, "_" + fileName.Substring(0, fileName.IndexOf(",")) + extension)}";

                                }
                            }

                            if (!string.IsNullOrEmpty(UrlImagenMini))
                            {
                                resourceLink.ImageUrl = UrlImagenMini;
                            }

                            #endregion

                            break;
                        case "Descripcion":
                            if (!string.IsNullOrEmpty(estiloAtrRec.NombreLectura))
                            {
                                resourceLink.DescriptionLabel = estiloAtrRec.NombreLectura;
                            }

                            resourceLink.Description = Documento.Descripcion;

                            break;
                        case "Autores":
                            if (!string.IsNullOrEmpty(estiloAtrRec.NombreLectura))
                            {
                                resourceLink.AuthorsLabel = estiloAtrRec.NombreLectura;
                            }

                            resourceLink.Authors = new Dictionary<string, string>();
                            string[] autores = filaDoc.Autor.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                            if (autores.Length > 0)
                            {
                                foreach (string autor in autores)
                                {
                                    string link = null;

                                    if (!pSemPropModel.EntityParent.SemanticResourceModel.HideInfoIsNotMember)
                                    {
                                        link = listaRec[docID] + "/autor/" + autor.Trim().ToLower();
                                    }

                                    resourceLink.Authors.Add(autor, link);
                                }
                            }

                            break;
                    }
                }
            }

            gestorDocumental.Dispose();
        }

        /// <summary>
        /// Genera los datos de lectura para la propiedad selector de entidad de tipo "PersonaGnoss" y tipo "GruposGnoss".
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        private void GenerarDatosLecturaSelectorEntidadPersonasYGruposGnoss(SemanticPropertyModel pSemPropModel)
        {
            pSemPropModel.OntologyPropInfo.EntitySelector.LinkedResources = new List<SemanticPropertyModel.ResourceLinkedToEntitySelector>();
            Dictionary<Guid, string> nombreElem = null;

            if (pSemPropModel.Element.Propiedad.EspecifPropiedad.EsSelectorEntidadInterno && mDatosEntidadesExternas.ContainsKey(new KeyValuePair<string, string>(pSemPropModel.Element.Propiedad.Nombre + "_AuxEntExt", pSemPropModel.Element.Propiedad.ElementoOntologia.TipoEntidad + "_AuxEntExt")))
            {
                nombreElem = UtilSemCms.ExtraerDeDataSetInfoSelectorPerYGruposGnoss((FacetadoDS)mDatosEntidadesExternas[new KeyValuePair<string, string>(pSemPropModel.Element.Propiedad.Nombre + "_AuxEntExt", pSemPropModel.Element.Propiedad.ElementoOntologia.TipoEntidad + "_AuxEntExt")][3]);
            }
            else if (mDatosEntidadesExternas.ContainsKey(new KeyValuePair<string, string>(pSemPropModel.Element.Propiedad.Nombre, pSemPropModel.Element.Propiedad.ElementoOntologia.TipoEntidad)))
            {
                nombreElem = (Dictionary<Guid, string>)mDatosEntidadesExternas[new KeyValuePair<string, string>(pSemPropModel.Element.Propiedad.Nombre, pSemPropModel.Element.Propiedad.ElementoOntologia.TipoEntidad)][3];
            }

            if (nombreElem != null)
            {
                foreach (string valor in pSemPropModel.Element.Propiedad.ValoresUnificados.Keys)
                {
                    if (valor.Contains("/"))
                    {
                        Guid peroGruID = Guid.Empty;
                        Guid.TryParse(valor.Substring(valor.LastIndexOf("/") + 1), out peroGruID);

                        if (peroGruID != Guid.Empty && nombreElem.ContainsKey(peroGruID))
                        {
                            SemanticPropertyModel.ResourceLinkedToEntitySelector resource = new SemanticPropertyModel.ResourceLinkedToEntitySelector();
                            resource.Title = nombreElem[peroGruID];
                            resource.Key = peroGruID;
                            pSemPropModel.OntologyPropInfo.EntitySelector.LinkedResources.Add(resource);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Genera los datos de edición para la propiedad selector de entidad.
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        private void GenerarDatosEdicionSelectorEntidad(SemanticPropertyModel pSemPropModel)
        {
            Propiedad propiedad = pSemPropModel.Element.Propiedad;
            SelectorEntidad selectorEnt = propiedad.EspecifPropiedad.SelectorEntidad;
            Dictionary<string, List<KeyValuePair<string, string>>> triplesEntidadesExt = null;
            SortedDictionary<string, List<string>> entidadesExtOrdenadas = null;
            FacetadoDS facetadoDS = null;
            Dictionary<Guid, string> nombresGrupos = null;
            DataWrapperIdentidad perfilesGnossDW = null;
            CargaDatosValoresSelectorEntidadYaAgregados(pSemPropModel, out facetadoDS, out triplesEntidadesExt, out entidadesExtOrdenadas, out nombresGrupos, out perfilesGnossDW, false);

            if (selectorEnt.TipoSeleccion == "Combo")
            {
                pSemPropModel.OntologyPropInfo.EntitySelector.EditionEntitiesValues = new Dictionary<string, string>();

                string sepPrin = null;
                string sepFin = null;
                string sepEntreProps = null;

                ObtenerSeparadoresSelectorEntidad(pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad.ExtraWhereAutocompletar, out sepPrin, out sepFin, out sepEntreProps);

                foreach (string entidadID in triplesEntidadesExt.Keys)
                {
                    string text = ObtenerValorSegunPropsEdicion(pSemPropModel, facetadoDS, entidadID, sepPrin, sepEntreProps, sepFin);
                    pSemPropModel.OntologyPropInfo.EntitySelector.EditionEntitiesValues.Add(entidadID, text);

                    if (propiedad.ListaValores.ContainsKey(entidadID))
                    {
                        propiedad.AgregarValorPropiedadUsado(entidadID);
                    }
                }

                KeyValuePair<string, string> claveDependencia = new KeyValuePair<string, string>(propiedad.Nombre, propiedad.ElementoOntologia.TipoEntidad);
                KeyValuePair<string, string> claveDependenciaBis = new KeyValuePair<string, string>();


                int indiceBis = propiedad.ElementoOntologia.TipoEntidad.IndexOf("_bis");
                if (indiceBis > 0)
                {
                    claveDependenciaBis = new KeyValuePair<string, string>(propiedad.Nombre, propiedad.ElementoOntologia.TipoEntidad.Substring(0, indiceBis));
                }



                if (Ontologia.ConfiguracionPlantilla.PropsSelecEntDependientes != null && (Ontologia.ConfiguracionPlantilla.PropsSelecEntDependientes.ContainsKey(claveDependencia) || Ontologia.ConfiguracionPlantilla.PropsSelecEntDependientes.ContainsKey(claveDependenciaBis)))
                {
                    if (Ontologia.ConfiguracionPlantilla.PropsSelecEntDependientes.ContainsKey(claveDependencia))
                    {
                        pSemPropModel.OntologyPropInfo.EntitySelector.DependentProperties = Ontologia.ConfiguracionPlantilla.PropsSelecEntDependientes[claveDependencia];
                    }
                    else
                    {
                        pSemPropModel.OntologyPropInfo.EntitySelector.DependentProperties = Ontologia.ConfiguracionPlantilla.PropsSelecEntDependientes[claveDependenciaBis];
                        List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();

                        string propiedadTipoEntidadBis = propiedad.ElementoOntologia.TipoEntidad;
                        string propiedadTipoEntidad = propiedad.ElementoOntologia.TipoEntidad.Substring(0, indiceBis);
                        foreach (KeyValuePair<string, string> value in Ontologia.ConfiguracionPlantilla.PropsSelecEntDependientes[claveDependenciaBis])
                        {
                            list.Add(new KeyValuePair<string, string>(value.Key, value.Value.Replace(propiedadTipoEntidad, propiedadTipoEntidadBis)));
                        }
                        pSemPropModel.OntologyPropInfo.EntitySelector.DependentProperties = list;
                    }

                }
            }
            else if (selectorEnt.TipoSeleccion == "ListaCheck")
            {
                pSemPropModel.OntologyPropInfo.EntitySelector.EditionEntitiesValues = new Dictionary<string, string>();

                string sepPrin = null;
                string sepFin = null;
                string sepEntreProps = null;

                ObtenerSeparadoresSelectorEntidad(pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad.ExtraWhereAutocompletar, out sepPrin, out sepFin, out sepEntreProps);

                foreach (string entidadID in triplesEntidadesExt.Keys)
                {
                    string text = ObtenerValorSegunPropsEdicion(pSemPropModel, facetadoDS, entidadID, sepPrin, sepEntreProps, sepFin);
                    pSemPropModel.OntologyPropInfo.EntitySelector.EditionEntitiesValues.Add(entidadID, text);

                    if (propiedad.ValoresUnificados.ContainsKey(entidadID))
                    {
                        propiedad.AgregarValorPropiedadUsado(entidadID);
                    }
                }
            }
            else if (selectorEnt.TipoSeleccion == "Autocompletar")
            {
                GenerarDatosAutocompletarSelectorEntidad(pSemPropModel, facetadoDS);
            }
            else if (selectorEnt.TipoSeleccion == "Tesauro")
            {
                GenerarDatosTesauroSemantico(pSemPropModel, triplesEntidadesExt, entidadesExtOrdenadas, propiedad.EspecifPropiedad.SelectorEntidad.PropiedadesEdicion[3], propiedad.EspecifPropiedad.SelectorEntidad.PropiedadesEdicion[5], facetadoDS, mUtilIdiomas.LanguageCode);
            }
            else if (selectorEnt.TipoSeleccion == "GruposGnoss" || selectorEnt.TipoSeleccion == "PersonaGnoss")
            {
                GenerarDatosAutocompletarPerYGruposSelectorEntidad(pSemPropModel, nombresGrupos, perfilesGnossDW);
            }
            else if (selectorEnt.TipoSeleccion == "UrlRecursoSemantico")
            {
                GenerarUrlRecursoSemanticoSelectorEntidad(pSemPropModel);
            }

            if (facetadoDS != null)
            {
                facetadoDS.Dispose();
            }
        }

        /// <summary>
        /// Genera los datos para la edición del control selector de entidad de tipo UrlRecursoSemantico.
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        private void GenerarUrlRecursoSemanticoSelectorEntidad(SemanticPropertyModel pSemPropModel)
        {
            if (pSemPropModel.Element.Propiedad.ValoresUnificados.Count > 0)
            {
                List<string> valores = new List<string>(pSemPropModel.Element.Propiedad.ValoresUnificados.Keys);
                pSemPropModel.Element.Propiedad.LimpiarValor();
                pSemPropModel.OntologyPropInfo.EntitySelector.EditionEntitiesValues = new Dictionary<string, string>();

                foreach (string valor in valores)
                {
                    try
                    {
                        string docID = valor.Substring(0, valor.LastIndexOf("_"));
                        string nombreDoc = docID.Substring(0, docID.LastIndexOf("_"));
                        docID = docID.Substring(docID.LastIndexOf("_") + 1);

                        if (nombreDoc.Contains("/"))
                        {
                            nombreDoc = nombreDoc.Substring(nombreDoc.LastIndexOf("/") + 1);
                        }

                        string urlRec = UrlsSemanticas.GetURLBaseRecursosFichaConIDs(mBaseURLIdioma, mUtilIdiomas, mProyectoActual.NombreCorto, null, UtilCadenas.EliminarCaracteresUrlSem(nombreDoc), null, new Guid(docID), false);

                        pSemPropModel.Element.Propiedad.AgregarValor(urlRec);

                        if (mUrlRecursoSemEntidadPrincipal == null)
                        {
                            mUrlRecursoSemEntidadPrincipal = new Dictionary<string, string>();
                        }

                        if (!mUrlRecursoSemEntidadPrincipal.ContainsKey(urlRec))
                        {
                            mUrlRecursoSemEntidadPrincipal.Add(urlRec, valor);
                        }

                        pSemPropModel.OntologyPropInfo.EntitySelector.EditionEntitiesValues.Add(valor, valor);
                    }
                    catch (Exception ex)
                    {
                        GuardarLogErrorAJAX("Selector de Entidad de tipo UrlRecursoSemantico con datos malos: " + ex.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Genera los datos para la edición del control selector de entidad de tipo autocompletar personas y grupos.
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        /// <param name="pNombresGrupos">Nombre de grupos autocompletados</param>
        /// <param name="pPerfilesGnossDS">DataSet con los perfiles autocompletados</param>
        private void GenerarDatosAutocompletarPerYGruposSelectorEntidad(SemanticPropertyModel pSemPropModel, Dictionary<Guid, string> pNombresGrupos, DataWrapperIdentidad pPerfilesGnossDS)
        {
            string organizacionID = null;

            if (mIdentidadActual.OrganizacionID.HasValue)
            {
                organizacionID = mIdentidadActual.OrganizacionID.Value.ToString();
            }

            string tipoConsulta = "2";

            if (pSemPropModel.SpecificationProperty.SelectorEntidad.TipoSeleccion == "PersonaGnoss")
            {
                tipoConsulta = "1";
            }

            pSemPropModel.OntologyPropInfo.EntitySelector.OrganizationID = organizacionID;
            pSemPropModel.OntologyPropInfo.EntitySelector.QueryType = tipoConsulta;

            if (pSemPropModel.Element.Propiedad.ValoresUnificados.Count > 0)
            {
                pSemPropModel.OntologyPropInfo.EntitySelector.EditionEntitiesValues = new Dictionary<string, string>();

                foreach (string valor in pSemPropModel.Element.Propiedad.ValoresUnificados.Keys)
                {
                    string textoValor = valor;

                    if (pSemPropModel.SpecificationProperty.SelectorEntidad.TipoSeleccion == "GruposGnoss" && pNombresGrupos.ContainsKey(EstiloPlantilla.IDGrupoGnoss(valor)))
                    {
                        textoValor = pNombresGrupos[EstiloPlantilla.IDGrupoGnoss(valor)];
                    }
                    else if (pSemPropModel.SpecificationProperty.SelectorEntidad.TipoSeleccion == "PersonaGnoss")
                    {
                        textoValor = UtilSemCms.NombrePerfilGnoss(valor, pPerfilesGnossDS);
                    }

                    pSemPropModel.OntologyPropInfo.EntitySelector.EditionEntitiesValues.Add(valor, textoValor);
                }
            }
        }

        /// <summary>
        /// Genera los datos para la edición del control selector de entidad de tipo autocompletar.
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        /// <param name="pFacetadoDS">DataSet de facetas con los datos de los valores del selector ya añadidos</param>
        private void GenerarDatosAutocompletarSelectorEntidad(SemanticPropertyModel pSemPropModel, FacetadoDS pFacetadoDS)
        {
            Propiedad propiedad = pSemPropModel.Element.Propiedad;
            SelectorEntidad selectorEnt = propiedad.EspecifPropiedad.SelectorEntidad;

            if (selectorEnt.ExtraWhereAutocompletar != null)
            {
                string[] elems = selectorEnt.ExtraWhereAutocompletar.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string elem in elems)
                {
                    if (elem.StartsWith("Titulo="))
                    {
                        pSemPropModel.OntologyPropInfo.EntitySelector.ExtraTitleAutoComplete = elem.Substring(elem.IndexOf("=") + 1);
                        break;
                    }
                }
            }

            string grafo = HttpUtility.UrlEncode(selectorEnt.Grafo); ;
            string urlentCont = "";
            string urlProp = "";
            string urlTipoEntSol = "";
            string extraWhere = "";
            string[] propEdicion = selectorEnt.PropiedadesEdicion.ToArray();
            string idioma = "";

            if (selectorEnt.UrlEntContenedora != null)
            {
                urlentCont = selectorEnt.UrlEntContenedora;
            }
            if (selectorEnt.UrlPropiedad != null)
            {
                urlProp = selectorEnt.UrlPropiedad;
            }
            if (selectorEnt.UrlTipoEntSolicitada != null)
            {
                urlTipoEntSol = selectorEnt.UrlTipoEntSolicitada;
            }

            if (selectorEnt.ExtraWhereAutocompletar != null)
            {
                extraWhere = UtilSemCms.ObtenerExtraWhereConInfoUsuario(selectorEnt.ExtraWhereAutocompletar, mIdentidadActual.Persona.FilaPersona, mProyectoActual.FilaProyecto, Entidades);
                extraWhere = extraWhere.Replace("'", "\\'").Replace("<", "[--C]").Replace(">", "[C--]");
            }

            if (selectorEnt.MultiIdioma)
            {
                idioma = Ontologia.IdiomaUsuario;
            }

            pSemPropModel.OntologyPropInfo.EntitySelector.Graph = grafo;
            pSemPropModel.OntologyPropInfo.EntitySelector.EntityRequestedUrl = urlentCont;
            pSemPropModel.OntologyPropInfo.EntitySelector.PropertyRequestedUrl = urlProp;
            pSemPropModel.OntologyPropInfo.EntitySelector.EntityTypeRequestedUrl = urlTipoEntSol;
            pSemPropModel.OntologyPropInfo.EntitySelector.ExtraWhereAutoComplete = extraWhere;
            pSemPropModel.OntologyPropInfo.EntitySelector.Language = idioma;
            pSemPropModel.OntologyPropInfo.EntitySelector.EditionProperties = new List<string>(propEdicion);

            if (selectorEnt.ExtraWhereAutocompletarExtras != null)
            {
                pSemPropModel.OntologyPropInfo.EntitySelector.AdditionalExtraTitleWhereAutoCompletes = new List<KeyValuePair<string, string>>();

                foreach (string extraWhereExtraL in selectorEnt.ExtraWhereAutocompletarExtras)
                {
                    string titulo = null;
                    string[] elems = extraWhereExtraL.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string elem in elems)
                    {
                        if (elem.StartsWith("Titulo="))
                        {
                            titulo = elem.Substring(elem.IndexOf("=") + 1);
                            break;
                        }
                    }

                    string extraWhereExtra = UtilSemCms.ObtenerExtraWhereConInfoUsuario(extraWhereExtraL, mIdentidadActual.Persona.FilaPersona, mProyectoActual.FilaProyecto, Entidades);
                    extraWhereExtra = extraWhereExtra.Replace("'", "\\'").Replace("<", "[--C]").Replace(">", "[C--]");

                    KeyValuePair<string, string> extraAdicional = new KeyValuePair<string, string>(titulo, extraWhereExtra);
                    pSemPropModel.OntologyPropInfo.EntitySelector.AdditionalExtraTitleWhereAutoCompletes.Add(extraAdicional);
                }
            }

            if (propiedad.ValoresUnificados.Count > 0)
            {
                string sepPrin = null;
                string sepFin = null;
                string sepEntreProps = null;

                ObtenerSeparadoresSelectorEntidad(pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad.ExtraWhereAutocompletar, out sepPrin, out sepFin, out sepEntreProps);
                pSemPropModel.OntologyPropInfo.EntitySelector.EditionEntitiesValues = new Dictionary<string, string>();

                foreach (string valor in propiedad.ValoresUnificados.Keys)
                {
                    pSemPropModel.OntologyPropInfo.EntitySelector.EditionEntitiesValues.Add(valor, ObtenerValorSegunPropsEdicion(pSemPropModel, pFacetadoDS, valor, sepPrin, sepEntreProps, sepFin));
                }
            }
        }

        /// <summary>
        /// Genera los datos de edición para tesauros semánticos.
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        /// <param name="pEntidades">Tiples de entidades externas</param>
        /// <param name="pEntidadesOrd">Entidades externas ordenadas</param>
        /// <param name="pPropCatName">Propiedad que representa el nombre del tesauro semántico</param>
        /// <param name="pPropHasHijo">Propiedad tiene hijo del tesauro semántico</param>
        /// <param name="pFacetadoDS">DataSet de facetas con los datos de los tesauros semánticos</param>
        /// <param name="pIdiomaUsu">Idioma del usuario</param>
        public void GenerarDatosTesauroSemantico(SemanticPropertyModel pSemPropModel, Dictionary<string, List<KeyValuePair<string, string>>> pEntidades, SortedDictionary<string, List<string>> pEntidadesOrd, string pPropCatName, string pPropHasHijo, FacetadoDS pFacetadoDS, string pIdiomaUsu)
        {
            pSemPropModel.OntologyPropInfo.EntitySelector.EntitiesWithChildren = new List<string>();
            pSemPropModel.OntologyPropInfo.EntitySelector.EditionEntitiesValues = new Dictionary<string, string>();

            GenerarDatosTesauroSemanticoCategoriasEdicion(pEntidades, pEntidadesOrd, pPropCatName, pPropHasHijo, pFacetadoDS, pIdiomaUsu, pSemPropModel.OntologyPropInfo.EntitySelector.EditionEntitiesValues, pSemPropModel.OntologyPropInfo.EntitySelector.EntitiesWithChildren);

            if (pSemPropModel.Element.Propiedad.ListaValores.Count > 0)
            {
                string ordenValoresTesuroSemantico = ObtenerOrdenValoresTesauroSemantico(pFacetadoDS, pSemPropModel.Element.Propiedad.ListaValores, pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad);

                string textoValor = "";

                foreach (string entidadVal in ordenValoresTesuroSemantico.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    DataRow[] filasEnt = pFacetadoDS.Tables[1].Select("s='" + entidadVal + "' AND p='" + pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad.PropiedadesEdicion[3] + "'");

                    if (filasEnt.Length == 0)
                    {
                        break;
                    }

                    textoValor += (string)filasEnt[0]["o"] + " > ";
                }

                if (!string.IsNullOrEmpty(textoValor))
                {
                    textoValor = textoValor.Substring(0, textoValor.Length - 3);
                }

                pSemPropModel.OntologyPropInfo.EntitySelector.SemanticThesaurusAddedValue = new KeyValuePair<string, string>(ordenValoresTesuroSemantico, textoValor);
            }
        }

        /// <summary>
        /// Genera los datos para la edición de las categorías de un tesauro semántico
        /// </summary>
        /// <param name="pEntidades">Tiples de entidades externas</param>
        /// <param name="pEntidadesOrd">Entidades externas ordenadas</param>
        /// <param name="pPropCatName">Propiedad que representa el nombre del tesauro semántico</param>
        /// <param name="pPropHasHijo">Propiedad tiene hijo del tesauro semántico</param>
        /// <param name="pFacetadoDS">DataSet de facetas con los datos de los tesauros semánticos</param>
        /// <param name="pIdiomaUsu">Idioma del usuario</param>
        /// <param name="pEditionEntitiesValues">Lista para las categoríasd del tesauro y sus nombres</param>
        /// <param name="pEntitiesWithChildren">Lista con las categorías que tienen hijos</param>
        public static void GenerarDatosTesauroSemanticoCategoriasEdicion(Dictionary<string, List<KeyValuePair<string, string>>> pEntidades, SortedDictionary<string, List<string>> pEntidadesOrd, string pPropCatName, string pPropHasHijo, FacetadoDS pFacetadoDS, string pIdiomaUsu, Dictionary<string, string> pEditionEntitiesValues, List<string> pEntitiesWithChildren)
        {
            foreach (List<string> entidadesID in pEntidadesOrd.Values)
            {
                foreach (string entidadID in entidadesID)
                {
                    bool tieneHijos = false;

                    foreach (KeyValuePair<string, string> propValor in pEntidades[entidadID])
                    {
                        if (propValor.Key == pPropHasHijo)
                        {
                            tieneHijos = true;
                        }

                        if (tieneHijos)
                        {
                            break;
                        }
                    }

                    string textoValor = ObtenerNombreCatTesSem(pFacetadoDS, entidadID, pPropCatName, pIdiomaUsu);
                    pEditionEntitiesValues.Add(entidadID, textoValor);

                    if (tieneHijos)
                    {
                        pEntitiesWithChildren.Add(entidadID);
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene el orden de los valores de los tesauros semánticos.
        /// </summary>
        /// <param name="pFacetadoDS">Dataset de facetas</param>
        /// <param name="pPropiedad">Propiedad de la que se extraen los valores</param>
        /// <returns>Valores ordenados por jerarquia</returns>
        private string ObtenerOrdenValoresTesauroSemantico(FacetadoDS pFacetadoDS, Dictionary<string, ElementoOntologia> pValoresPropiedad, SelectorEntidad pSelectorEntidad)
        {
            Dictionary<string, List<string>> catsEHijos = new Dictionary<string, List<string>>();
            List<string> listaHijosTotal = new List<string>();

            foreach (string valor in pValoresPropiedad.Keys)
            {
                DataRow[] filas = pFacetadoDS.Tables[0].Select("s='" + valor + "' AND p='" + pSelectorEntidad.PropiedadesEdicion[5] + "'");

                catsEHijos.Add(valor, new List<string>());

                foreach (DataRow fila in filas)
                {
                    string catHija = (string)fila[2];

                    if (pValoresPropiedad.ContainsKey(catHija))
                    {
                        catsEHijos[valor].Add(catHija);
                        listaHijosTotal.Add(catHija);
                    }
                }
            }

            string padre = null;

            foreach (string valor in pValoresPropiedad.Keys)
            {
                if (!listaHijosTotal.Contains(valor))
                {
                    padre = valor;
                    break;
                }
            }

            string ordenValoresTesuroSemantico = padre + ",";

            while (pValoresPropiedad.ContainsKey(padre))
            {
                if (catsEHijos[padre].Count > 0)
                {
                    padre = catsEHijos[padre][0];
                    ordenValoresTesuroSemantico += padre + ",";
                }
                else
                {
                    break;
                }
            }

            return ordenValoresTesuroSemantico;
        }

        /// <summary>
        /// Obtiene el valor según las propiedades de edición.
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        /// <param name="pFacetadoDS">Dataset de facetas</param>
        /// <param name="pValor">Valor real</param>
        /// <param name="pSepPrin">Separador principal</param>
        /// <param name="pSepEntreProps">Separador entre propiedades</param>
        /// <param name="pSepFin">Separador final</param>
        private string ObtenerValorSegunPropsEdicion(SemanticPropertyModel pSemPropModel, FacetadoDS pFacetadoDS, string pValor, string pSepPrin, string pSepEntreProps, string pSepFin)
        {
            return UtilSemCms.ObtenerValorSegunPropsEdicion(pFacetadoDS, pValor, pSepPrin, pSepEntreProps, pSepFin, pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad.PropiedadesEdicion, Ontologia.IdiomaUsuario);
        }

        /// <summary>
        /// Obtiene los separadores configurados en el selector de entidad.
        /// </summary>
        /// <param name="pExtraWhereAuto">Información de donde se pueden extraer los separadores</param>
        /// <param name="pSeparadorPrincipal">Separador principal del selector</param>
        /// <param name="pSeparadorFin">Separador final del selector</param>
        /// <param name="pSeparadorEntreProps">Separador entre propiedades</param>
        private void ObtenerSeparadoresSelectorEntidad(string pExtraWhereAuto, out string pSeparadorPrincipal, out string pSeparadorFin, out string pSeparadorEntreProps)
        {
            pSeparadorPrincipal = " ";
            pSeparadorFin = null;
            pSeparadorEntreProps = " ";

            if (pExtraWhereAuto != null)
            {
                string[] parametrosWhere = pExtraWhereAuto.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string parametroWhere in parametrosWhere)
                {
                    if (parametroWhere.StartsWith("SeparadorPropPrinc"))
                    {
                        pSeparadorPrincipal = parametroWhere.Substring(parametroWhere.IndexOf("=") + 1);
                    }

                    if (parametroWhere.StartsWith("SeparadorFinal"))
                    {
                        pSeparadorFin = parametroWhere.Substring(parametroWhere.IndexOf("=") + 1);
                    }

                    if (parametroWhere.StartsWith("SeparadorEntreProps"))
                    {
                        pSeparadorEntreProps = parametroWhere.Substring(parametroWhere.IndexOf("=") + 1);
                    }
                }
            }
        }

        /// <summary>
        /// Carga los valores de las entidades y agregadas del selector de entidad para su edición.FilaPersona
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        /// <param name="pFacetadoDS">DataSet de facetas para cargar los datos</param>
        /// <param name="pTriplesEntidadesExt">Lista con los sujetos, predicados y objetos de las entidades externas obtenidas</param>
        /// <param name="pEntidadesExtOrdenadas">Lista con las entidades ordenadas por una propiedad de estas (Clave). En el Value están las entidades</param>
        /// <param name="pNombresGrupos">Nombre de grupos autocompletados</param>
        /// <param name="pPerfilesGnossDW">DataSet con los perfiles autocompletados</param>
        /// <param name="pCargaDependeciaProps">Indica si la carga es de dependecia de propiedades</param>
        private void CargaDatosValoresSelectorEntidadYaAgregados(SemanticPropertyModel pSemPropModel, out FacetadoDS pFacetadoDS, out Dictionary<string, List<KeyValuePair<string, string>>> pTriplesEntidadesExt, out SortedDictionary<string, List<string>> pEntidadesExtOrdenadas, out Dictionary<Guid, string> pNombresGrupos, out DataWrapperIdentidad pPerfilesGnossDW, bool pCargaDependeciaProps)
        {
            Propiedad propiedad = pSemPropModel.Element.Propiedad;
            pFacetadoDS = null;
            pTriplesEntidadesExt = null;
            pEntidadesExtOrdenadas = null;
            pNombresGrupos = null;
            pPerfilesGnossDW = null;

            if (propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion != "UrlRecurso" && propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion != "UrlRecursoSemantico")
            {
                FacetadoCN facetadoCN = new FacetadoCN(mUrlIntragnoss, true, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                if (!pCargaDependeciaProps && !string.IsNullOrEmpty(propiedad.EspecifPropiedad.SelectorEntidad.ConsultaEdicion))
                {
                    pFacetadoDS = facetadoCN.ObtenerRDFXMLSelectorEntidadFormularioPorConsulta(propiedad.EspecifPropiedad.SelectorEntidad.Grafo, null, propiedad.EspecifPropiedad.SelectorEntidad.ConsultaEdicion);
                }
                else if ((propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "Combo" || propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "ListaCheck"))
                {
                    if (string.IsNullOrEmpty(propiedad.EspecifPropiedad.SelectorEntidad.ConsultaDependiente))
                    {
                        string extraWhere = UtilSemCms.ObtenerExtraWhereConInfoUsuario(propiedad.EspecifPropiedad.SelectorEntidad.ExtraWhereAutocompletar, mIdentidadActual.Persona.FilaPersona, mProyectoActual.FilaProyecto, Entidades);

                        if (extraWhere != null && extraWhere.Contains("|||"))
                        {
                            extraWhere = extraWhere.Substring(0, extraWhere.IndexOf("|||"));
                        }

                        //Descartamos los borradores:
                        extraWhere += " MINUS {?s <" + GestionOWL.PropBorradorGnossRdf + "> ?borrador}";

                        string idioma = null;

                        if (propiedad.EspecifPropiedad.SelectorEntidad.MultiIdioma)
                        {
                            idioma = Ontologia.IdiomaUsuario;
                        }

                        pFacetadoDS = facetadoCN.ObtenerRDFXMLSelectorEntidadFormulario(propiedad.EspecifPropiedad.SelectorEntidad.Grafo, propiedad.EspecifPropiedad.SelectorEntidad.UrlEntContenedora, propiedad.EspecifPropiedad.SelectorEntidad.UrlPropiedad, propiedad.EspecifPropiedad.SelectorEntidad.UrlTipoEntSolicitada, propiedad.EspecifPropiedad.SelectorEntidad.PropiedadesEdicion, null, extraWhere, idioma);
                    }
                    else
                    {
                        //Obtengo los sujetos permitios
                        List<string> listaEntidadesBusqueda = new List<string>();
                        Propiedad propDeLaQueDepende = null;
                        if (pSemPropModel.EntityParent != null && pSemPropModel.EntityParent.Entity != null)
                        {
                            propDeLaQueDepende = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(propiedad.EspecifPropiedad.SelectorEntidad.PropiedadDeLaQueDepende.Key, propiedad.EspecifPropiedad.SelectorEntidad.PropiedadDeLaQueDepende.Value, new List<ElementoOntologia>() { pSemPropModel.EntityParent.Entity });
                        }

                        if (propDeLaQueDepende == null)
                        {
                            propDeLaQueDepende = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(propiedad.EspecifPropiedad.SelectorEntidad.PropiedadDeLaQueDepende.Key, propiedad.EspecifPropiedad.SelectorEntidad.PropiedadDeLaQueDepende.Value, Entidades);
                        }

                        if (propDeLaQueDepende != null && propDeLaQueDepende.ValoresUnificados.Count > 0)
                        {
                            List<string> entidades = new List<string>(propDeLaQueDepende.ValoresUnificados.Keys);

                            FacetadoDS facetadoAuxDS = facetadoCN.ObtenerValoresPropiedadesEntidadesPorConsulta(propiedad.EspecifPropiedad.SelectorEntidad.Grafo, entidades, propiedad.EspecifPropiedad.SelectorEntidad.ConsultaDependiente);

                            foreach (DataRow fila in facetadoAuxDS.Tables[0].Rows)
                            {
                                string sujetoReciproco = (string)fila[0];

                                if (!listaEntidadesBusqueda.Contains(sujetoReciproco))
                                {
                                    listaEntidadesBusqueda.Add(sujetoReciproco);
                                }
                            }

                            facetadoAuxDS.Dispose();
                        }

                        if (listaEntidadesBusqueda.Count > 0)
                        {
                            //Obtengo los datos de esos sujetos
                            pFacetadoDS = facetadoCN.ObtenerValoresPropiedadesEntidadesConJerarquiaYExternas(propiedad.EspecifPropiedad.SelectorEntidad.Grafo, listaEntidadesBusqueda, propiedad.EspecifPropiedad.SelectorEntidad.PropiedadesEdicion, propiedad.EspecifPropiedad.SelectorEntidad.AnidamientoGnoss);
                        }
                    }
                }
                else if (propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "Tesauro")
                {
                    pFacetadoDS = facetadoCN.ObtenerCatPrimerNivelTesSemanticoFormulario(propiedad.EspecifPropiedad.SelectorEntidad.Grafo, propiedad.EspecifPropiedad.SelectorEntidad.UrlPropiedad, propiedad.EspecifPropiedad.SelectorEntidad.UrlTipoEntSolicitada, propiedad.EspecifPropiedad.SelectorEntidad.PropiedadesEdicion);

                    if (!propiedad.FunctionalProperty && !propiedad.CardinalidadMenorOIgualUno)
                    {//No se permite que se añada más de una ruta.
                        propiedad.CardinalidadMenorOIgualUno = true;
                    }

                    if (propiedad.ValoresUnificados.Count > 0)
                    {
                        List<string> propBusqueda = new List<string>(propiedad.EspecifPropiedad.SelectorEntidad.PropiedadesEdicion);
                        propBusqueda.RemoveAt(0); //Elimino las primeras props porque no son de las entidades buscadas
                        propBusqueda.RemoveAt(0);

                        pFacetadoDS.Merge(facetadoCN.ObtenerValoresPropiedadesEntidadesConJerarquiaYExternas(propiedad.EspecifPropiedad.SelectorEntidad.Grafo, new List<string>(propiedad.ValoresUnificados.Keys), propBusqueda, false));
                    }

                    //Establezco los nombres auxiliares de las categorías para su edición JS:
                    if (pSemPropModel.EntityParent.SemanticResourceModel.AuxiliaryCategoryTesSemNameInfo == null)
                    {
                        pSemPropModel.EntityParent.SemanticResourceModel.AuxiliaryCategoryTesSemNameInfo = "";
                    }

                    foreach (DataRow fila in pFacetadoDS.Tables[0].Select("p='" + propiedad.EspecifPropiedad.SelectorEntidad.PropiedadesEdicion[3] + "'"))
                    {
                        string sujeto = (string)fila[0];
                        if (!pSemPropModel.EntityParent.SemanticResourceModel.AuxiliaryCategoryTesSemNameInfo.Contains(sujeto + "|"))
                        {
                            string nombreCat = ObtenerNombreCatTesSem(pFacetadoDS, sujeto, propiedad.EspecifPropiedad.SelectorEntidad.PropiedadesEdicion[3], Ontologia.IdiomaUsuario);
                            pSemPropModel.EntityParent.SemanticResourceModel.AuxiliaryCategoryTesSemNameInfo += sujeto + "|" + nombreCat + "|||";
                        }
                    }
                }
                else
                {
                    if (propiedad.UnicoValor.Key != null || propiedad.ListaValores.Count > 0)
                    {
                        List<string> listaEntidadesBusqueda = new List<string>();

                        if (propiedad.UnicoValor.Key != null)
                        {
                            listaEntidadesBusqueda.Add(propiedad.UnicoValor.Key);
                        }
                        else
                        {
                            listaEntidadesBusqueda = new List<string>(propiedad.ListaValores.Keys);
                        }

                        if (propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "GruposGnoss")
                        {
                            List<Guid> listaGrupos = new List<Guid>();

                            foreach (string valor in propiedad.ValoresUnificados.Keys)
                            {
                                listaGrupos.Add(UtilSemCms.ObtenerIDGnoss(valor));
                            }

                            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                            pNombresGrupos = identCN.ObtenerNombresDeGrupos(listaGrupos);
                            identCN.Dispose();

                            propiedad.LimpiarValor();

                            foreach (Guid grupoID in listaGrupos)
                            {
                                if (pNombresGrupos.ContainsKey(grupoID))
                                {
                                    propiedad.AgregarValor("g_" + grupoID.ToString());
                                }
                            }

                        }
                        else if (propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "PersonaGnoss")
                        {
                            List<Guid> listaUsuarios = new List<Guid>();

                            foreach (string valor in propiedad.ValoresUnificados.Keys)
                            {
                                listaUsuarios.Add(UtilSemCms.ObtenerIDGnoss(valor));
                            }

                            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                            Dictionary<Guid, Guid> usuPerfilID = identCN.ObtenerPerfilesIDPorUsuariosIDEnProyecto(listaUsuarios, mProyectoActual.Clave);
                            pPerfilesGnossDW = identCN.ObtenerPerfilesPorPerfilesID(new List<Guid>(usuPerfilID.Values));
                            identCN.Dispose();

                            propiedad.LimpiarValor();

                            foreach (Guid usuID in listaUsuarios)
                            {
                                if (usuPerfilID.ContainsKey(usuID))
                                {
                                    propiedad.AgregarValor(usuPerfilID[usuID].ToString());
                                    mSemRecModel.AuxiliaryCategoryTesSemNameInfo = mSemRecModel.AuxiliaryCategoryTesSemNameInfo.Replace(usuID + "|", usuPerfilID[usuID] + "|");//Para que coja bien el nombre de edición del perfil.
                                }
                            }
                        }
                        else
                        {
                            pFacetadoDS = facetadoCN.ObtenerValoresPropiedadesEntidadesConJerarquiaYExternas(propiedad.EspecifPropiedad.SelectorEntidad.Grafo, listaEntidadesBusqueda, propiedad.EspecifPropiedad.SelectorEntidad.PropiedadesEdicion, propiedad.EspecifPropiedad.SelectorEntidad.AnidamientoGnoss);
                        }
                    }
                }

                facetadoCN.Dispose();

                DarFormatoDatosValoresSelectorEntidadYaAgregados(pFacetadoDS, out pTriplesEntidadesExt, out pEntidadesExtOrdenadas, propiedad);
            }
        }

        /// <summary>
        /// Da formato a los valores de las entidades y agregadas del selector de entidad para su edición.
        /// </summary>
        /// <param name="pFacetadoDS">DataSet de facetas para cargar los datos</param>
        /// <param name="pTriplesEntidadesExt">Lista con los sujetos, predicados y objetos de las entidades externas obtenidas</param>
        /// <param name="pEntidadesExtOrdenadas">Lista con las entidades ordenadas por una propiedad de estas (Clave). En el Value están las entidades</param>
        /// <param name="pPropiedad">Propiedad del selector</param>
        private void DarFormatoDatosValoresSelectorEntidadYaAgregados(FacetadoDS pFacetadoDS, out Dictionary<string, List<KeyValuePair<string, string>>> pTriplesEntidadesExt, out SortedDictionary<string, List<string>> pEntidadesExtOrdenadas, Propiedad pPropiedad)
        {
            pEntidadesExtOrdenadas = null;
            pTriplesEntidadesExt = new Dictionary<string, List<KeyValuePair<string, string>>>();

            if (pFacetadoDS != null && (pPropiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "Combo" || pPropiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "ListaCheck" || pPropiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "Tesauro"))
            {
                if (pPropiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "Tesauro")
                {
                    pEntidadesExtOrdenadas = new SortedDictionary<string, List<string>>();
                }

                foreach (DataRow fila in pFacetadoDS.Tables[0].Rows)
                {
                    string sujeto = (string)fila[0];
                    string predicado = (string)fila[1];
                    string objeto = (string)fila[2];
                    if (pTriplesEntidadesExt.ContainsKey(sujeto))
                    {
                        pTriplesEntidadesExt[sujeto].Add(new KeyValuePair<string, string>(predicado, objeto));
                    }
                    else
                    {
                        List<KeyValuePair<string, string>> valorProp = new List<KeyValuePair<string, string>>();
                        valorProp.Add(new KeyValuePair<string, string>(predicado, objeto));
                        pTriplesEntidadesExt.Add(sujeto, valorProp);
                    }

                    if (pEntidadesExtOrdenadas != null && predicado == pPropiedad.EspecifPropiedad.SelectorEntidad.PropiedadesEdicion[2])
                    {
                        if (!pEntidadesExtOrdenadas.ContainsKey(objeto))
                        {
                            pEntidadesExtOrdenadas.Add(objeto, new List<string>());
                        }

                        pEntidadesExtOrdenadas[objeto].Add(sujeto);
                    }
                }

                if (pPropiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion != "Tesauro")
                {
                    foreach (DataRow fila in pFacetadoDS.Tables[0].Rows)
                    {
                        string objeto = (string)fila[2];

                        if (objeto.StartsWith("http://"))
                        {
                            pTriplesEntidadesExt.Remove(objeto);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene el nombre de una categoría de tesauro semántico según el idioma.
        /// </summary>
        /// <param name="pFacetadoDS">DataSet de facetas</param>
        /// <param name="pSujeto">Sujeto</param>
        /// <param name="pPropNombre">Propiedad nombre de las categorías</param>
        /// <param name="pIdiomaUsu">Idioma del usuario</param>
        /// <returns>Nombre de una categoría de tesauro semántico según el idioma</returns>
        public static string ObtenerNombreCatTesSem(FacetadoDS pFacetadoDS, string pSujeto, string pPropNombre, string pIdiomaUsu)
        {
            DataRow[] filas = pFacetadoDS.Tables[0].Select("s='" + pSujeto + "' AND p='" + pPropNombre + "'");

            if (filas.Length > 0)
            {
                if (filas.Length > 1 && pFacetadoDS.Tables[0].Columns.Count > 3 && !string.IsNullOrEmpty(pIdiomaUsu))
                {
                    DataRow filaSinIdioma = null;

                    foreach (DataRow fila in filas)
                    {
                        if (!fila.IsNull(3) && !string.IsNullOrEmpty((string)fila[3]))
                        {
                            if ((string)fila[3] == pIdiomaUsu)
                            {
                                return (string)fila[2];
                            }
                        }
                        else
                        {
                            filaSinIdioma = fila;
                        }
                    }

                    if (filaSinIdioma != null)
                    {
                        return (string)filaSinIdioma[2];
                    }
                }

                return (string)filas[0][2];
            }

            return null;
        }

        #endregion

        #region ObjetProperty

        /// <summary>
        /// Agrega una propiedad de tipo objeto.
        /// </summary>
        /// <param name="pElemOrdenado">Elemento ordenado del modelo de propiedad</param>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        /// <returns>TRUE si la propiedad debe agregarse al modelo, FALSE en caso contrario</returns>
        private bool AgregarObjectProperty(ElementoOrdenado pElemOrdenado, SemanticPropertyModel pSemPropModel, Guid pIdentidadID)
        {
            Propiedad propiedad = pSemPropModel.Element.Propiedad;
            pSemPropModel.OntologyPropInfo.UniqueValue = (propiedad.FunctionalProperty || propiedad.CardinalidadMenorOIgualUno);
            pSemPropModel.OntologyPropInfo.MinCardinality = propiedad.CardinalidadMinima;
            pSemPropModel.OntologyPropInfo.MaxCardinality = propiedad.CardinalidadMaxima;
            pSemPropModel.OntologyPropInfo.FunctionalProperty = propiedad.FunctionalProperty;
            pSemPropModel.OntologyPropInfo.PropertyValues = new List<SemanticPropertyModel.PropertyValue>();

            if (pSemPropModel.OntologyPropInfo.UniqueValue)
            {
                ObtenerValorEntidadObjectProperty(pSemPropModel, pIdentidadID);
            }
            else
            {
                pSemPropModel.OntologyPropInfo.ItIsPossibleToSddMoreValues = SePuedeAgregarMasValoresAPropiedad(propiedad);
                ObtenerValoresEntidadObjectProperty(pSemPropModel, pIdentidadID);

                if (!pSemPropModel.EntityParent.SemanticResourceModel.ReadMode)
                {
                    pSemPropModel.OntologyPropInfo.ControlID = "panel_contenedor_Entidades_" + propiedad.NombreGeneracionIDs + Guid.NewGuid();
                    RegistrarIDControlPropiedad(propiedad, pSemPropModel.OntologyPropInfo.ControlID);
                }

                DarTextoCorrectoTituloProp(pSemPropModel);
            }

            AgregarRDFAYMicrodatos(pSemPropModel);

            //Por si hay alguna entidad externa no configurada y el valor de la propiedad va sin entidad relacionada.
            return (!pSemPropModel.ReadMode || pSemPropModel.PropertyValues.Count > 0);
        }

        /// <summary>
        /// Obtiene la entiad que es valor de la propiedad.
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        private void ObtenerValorEntidadObjectProperty(SemanticPropertyModel pSemPropModel, Guid pIdentidadID)
        {
            SemanticPropertyModel.PropertyValue propertyValue = new SemanticPropertyModel.PropertyValue();
            propertyValue.Property = pSemPropModel;
            ElementoOntologia instanciaEntidad = null;

            if (pSemPropModel.Element.Propiedad.UnicoValor.Value != null)
            {
                instanciaEntidad = pSemPropModel.Element.Propiedad.UnicoValor.Value;
            }
            else if (pSemPropModel.Element.Propiedad.ListaValores.Count > 0)
            {
                instanciaEntidad = pSemPropModel.Element.Propiedad.ListaValores.FirstOrDefault().Value;
            }

            if (instanciaEntidad == null)
            {
                instanciaEntidad = mOntologia.GetEntidadTipo(pSemPropModel.Element.Propiedad.Rango, true);
                instanciaEntidad.ID = instanciaEntidad.TipoEntidad + "_" + mDocumentoID.ToString() + "_" + Guid.NewGuid();
                pSemPropModel.Element.Propiedad.UnicoValor = new KeyValuePair<string, ElementoOntologia>(instanciaEntidad.ID, instanciaEntidad);
                pSemPropModel.Element.Propiedad.ElementoOntologia.EntidadesRelacionadas.Add(instanciaEntidad);
            }

            if (pSemPropModel.ReadMode && (!instanciaEntidad.CumpleCondicionesMostrar || !pSemPropModel.Element.CumpleEntidadCondicionesMostrar(instanciaEntidad)))
            {
                return;
            }

            propertyValue.Value = instanciaEntidad.ID;
            propertyValue.RelatedEntity = ObtenerModeloEntidad(instanciaEntidad, pSemPropModel.Depth, pSemPropModel.Element.Propiedad.ElementoOntologia, pSemPropModel, pIdentidadID);
            pSemPropModel.OntologyPropInfo.PropertyValues.Add(propertyValue);
        }

        /// <summary>
        /// Obtiene la entiad que es valor de la propiedad.
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        private void ObtenerValoresEntidadObjectProperty(SemanticPropertyModel pSemPropModel, Guid pIdentidadID)
        {
            AjustarPaginacionPropsEntAuxiliar(pSemPropModel);

            foreach (ElementoOntologia entidad in pSemPropModel.Element.Propiedad.ListaValores.Values)
            {
                if (entidad == null || (pSemPropModel.EntityParent.SemanticResourceModel.ReadMode && (!entidad.CumpleCondicionesMostrar || !pSemPropModel.Element.CumpleEntidadCondicionesMostrar(entidad))))
                {
                    continue;
                }

                SemanticPropertyModel.PropertyValue propertyValue = new SemanticPropertyModel.PropertyValue();
                propertyValue.Property = pSemPropModel;
                propertyValue.Value = entidad.ID;
                propertyValue.RelatedEntity = ObtenerModeloEntidad(entidad, pSemPropModel.Depth, pSemPropModel.Element.Propiedad.ElementoOntologia, pSemPropModel, true, pIdentidadID);
                pSemPropModel.OntologyPropInfo.PropertyValues.Add(propertyValue);

                ObtenerTitulosRepresentatesEntidadValor(propertyValue);

                if (pSemPropModel.EntityParent.SemanticResourceModel.ReadMode && entidad.ContienePropiedadTesSemArbol && !mVistaPersonalizada)
                {
                    break;
                }
            }

            if (!pSemPropModel.EntityParent.SemanticResourceModel.ReadMode)
            {
                //Agregamos una nueva instancia, que será la nueva a rellenar para introducir nuevos valores:
                SemanticPropertyModel.PropertyValue propertyValueNuevaInst = new SemanticPropertyModel.PropertyValue();
                propertyValueNuevaInst.Property = pSemPropModel;
                ElementoOntologia instanciaEntidad = mOntologia.GetEntidadTipo(pSemPropModel.Element.Propiedad.Rango, true);
                instanciaEntidad.ID = instanciaEntidad.TipoEntidad + "_" + mDocumentoID.ToString() + "_" + Guid.NewGuid();

                propertyValueNuevaInst.Value = instanciaEntidad.ID;
                propertyValueNuevaInst.RelatedEntity = ObtenerModeloEntidad(instanciaEntidad, pSemPropModel.Depth, pSemPropModel.Element.Propiedad.ElementoOntologia, pSemPropModel, pIdentidadID);
                pSemPropModel.OntologyPropInfo.PropertyValues.Add(propertyValueNuevaInst);

                ObtenerTitulosRepresentatesEntidadesValor(pSemPropModel);
            }
        }

        /// <summary>
        /// Ajusta la paginación del selector.
        /// </summary>
        /// <param name="pSemPropModel">Selector</param>
        private void AjustarPaginacionPropsEntAuxiliar(SemanticPropertyModel pSemPropModel)
        {
            if (mSemRecModel.ReadMode && pSemPropModel.SpecificationProperty.NumElemPorPag > 0 && pSemPropModel.Element.Propiedad.ListaValores.Count > 0)
            {
                List<string> valores = new List<string>(pSemPropModel.Element.Propiedad.ListaValores.Keys);
                Dictionary<string, ElementoOntologia> valoresCompl = new Dictionary<string, ElementoOntologia>(pSemPropModel.Element.Propiedad.ListaValores);
                pSemPropModel.Element.Propiedad.LimpiarValor();

                int inicio = 0;

                if (PropiedadCallbackTraerMas.Key != null && pSemPropModel.Element.Propiedad.Nombre == PropiedadCallbackTraerMas.Key && pSemPropModel.Element.Propiedad.ElementoOntologia.ID == PropiedadCallbackTraerMas.Value.Key)
                {
                    inicio = PropiedadCallbackTraerMas.Value.Value;
                }

                int fin = inicio + pSemPropModel.SpecificationProperty.NumElemPorPag;

                for (int i = inicio; i < fin; i++)
                {
                    if (i < valores.Count)
                    {
                        pSemPropModel.Element.Propiedad.ListaValores.Add(valores[i], valoresCompl[valores[i]]);
                    }
                }

                if (inicio == 0 && valores.Count > fin)
                {
                    pSemPropModel.OntologyPropInfo.NumEntitiesForPage = fin;
                    pSemPropModel.OntologyPropInfo.TotalEntitiesPagination = valores.Count;
                }
            }
        }

        /// <summary>
        /// Obtiene los títulos de los representantes de la entidad que es valor de la propiedad actual.
        /// </summary>
        /// <param name="pPropertyValue">Modelo de valor de una propiedad</param>
        private void ObtenerTitulosRepresentatesEntidadValor(SemanticPropertyModel.PropertyValue pPropertyValue)
        {
            if (!pPropertyValue.Property.EntityParent.SemanticResourceModel.ReadMode)
            {
                pPropertyValue.RepresentativeEntityTitles = new List<string>();

                if (pPropertyValue.RelatedEntity.Entity.EspecifEntidad.Representantes.Count == 0)
                {
                    string valorColumna = pPropertyValue.RelatedEntity.Entity.ID;

                    if (pPropertyValue.RelatedEntity.Entity != null && pPropertyValue.RelatedEntity.Entity.Descripcion != null)
                    {
                        valorColumna = pPropertyValue.RelatedEntity.Entity.Descripcion;
                    }

                    if (pPropertyValue.RelatedEntity.Entity.Descripcion.Equals(pPropertyValue.RelatedEntity.Entity.TipoEntidad)
                        && pPropertyValue.RelatedEntity.Entity.Propiedades.Count > 0
                        && pPropertyValue.RelatedEntity.Entity.Propiedades[0].EspecifPropiedad != null
                        && pPropertyValue.RelatedEntity.Entity.Propiedades[0].EspecifPropiedad.SelectorEntidad != null
                        && pPropertyValue.RelatedEntity.Entity.Propiedades[0].EspecifPropiedad.SelectorEntidad.TipoSeleccion.Equals("Tesauro")
                        && pPropertyValue.RelatedEntity.SemanticResourceModel != null
                        && !string.IsNullOrEmpty(pPropertyValue.RelatedEntity.SemanticResourceModel.AuxiliaryCategoryTesSemNameInfo))
                    {
                        string auxiliaryCategoryTesSemNameInfo = ParseoAuxiliaryCategoryTesSemNameInfo(pPropertyValue.RelatedEntity.SemanticResourceModel.AuxiliaryCategoryTesSemNameInfo);

                        var categorias = pPropertyValue.RelatedEntity.SemanticResourceModel.AuxiliaryCategoryTesSemNameInfo.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries).ToDictionary(item => item.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries)[0], item => item.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries)[1]);

                        string nombreCategoria = "";
                        string separadorCategorias = "";
                        // Es un tesauro semántico, compongo el nombre con el nombre de las categorías
                        foreach (string uriCategoria in pPropertyValue.RelatedEntity.Entity.Propiedades[0].ValoresUnificados.Keys)
                        {
                            if (categorias.ContainsKey(uriCategoria))
                            {
                                nombreCategoria += $"{separadorCategorias}{categorias[uriCategoria]}";
                                separadorCategorias = " > ";
                            }
                        }

                        if (!string.IsNullOrEmpty(nombreCategoria))
                        {
                            valorColumna = nombreCategoria;
                        }
                    }

                    valorColumna = valorColumna.Replace("_", " ");
                    pPropertyValue.RepresentativeEntityTitles.Add(valorColumna);
                }
                else
                {
                    int numeracionSiCodigoRepresentacionIgualMenosUno = 0;

                    foreach (Representante representante in pPropertyValue.RelatedEntity.Entity.EspecifEntidad.Representantes)
                    {
                        string valorColumna = ObtenerTextoPropiedadRepresentante(representante, pPropertyValue.RelatedEntity, ref numeracionSiCodigoRepresentacionIgualMenosUno, pPropertyValue.Property).Replace("_", " ");
                        pPropertyValue.RepresentativeEntityTitles.Add(valorColumna);
                    }
                }
            }
        }

        private string ParseoAuxiliaryCategoryTesSemNameInfo(string auxiliaryCategoryTesSemNameInfo)
        {
            //Eliminamos cualquier propiedad que este vacia
            string devolver = auxiliaryCategoryTesSemNameInfo;
            while (devolver.Contains("||||"))
            {
                int longitud = devolver.IndexOf("||||") - devolver.LastIndexOf("|||", devolver.IndexOf("||||"));
                string eliminar = devolver.Substring(devolver.LastIndexOf("|||", devolver.IndexOf("||||")), longitud + 1);
                devolver = devolver.Replace(eliminar, "");
                mLoggingService.GuardarLogError($"La propiedad {devolver.Replace("|", "")} no tiene valor en el grafo. Por favor reviselo.");
            }
            return devolver;
        }

        /// <summary>
        /// Obtiene los títulos de los representantes de las entidades que pueden ser valor de la propiedad actual.
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        private void ObtenerTitulosRepresentatesEntidadesValor(SemanticPropertyModel pSemPropModel)
        {
            if (!pSemPropModel.EntityParent.SemanticResourceModel.ReadMode)
            {
                pSemPropModel.OntologyPropInfo.RepresentativeEntityTitles = new List<string>();
                ElementoOntologia entidadASelecionarTitulo = mOntologia.GetEntidadTipo(pSemPropModel.Element.Propiedad.Rango, false);

                if (entidadASelecionarTitulo.EspecifEntidad.Representantes.Count > 0)
                {
                    foreach (Representante representante in entidadASelecionarTitulo.EspecifEntidad.Representantes)
                    {
                        Propiedad propiedadEntASel = representante.Propiedad;
                        propiedadEntASel.ElementoOntologia = entidadASelecionarTitulo;
                        string valorColumna = "";

                        if (propiedadEntASel is PropiedadPlantilla)
                        {
                            valorColumna = mUtilIdiomas.GetText("CONFIGURARARCHIVOPLANTILLA", "TIPOENTIDAD");
                        }
                        else
                        {
                            valorColumna = propiedadEntASel.EspecifPropiedad.NombrePropiedad(false);

                            if (valorColumna == null)
                            {
                                valorColumna = propiedadEntASel.Nombre;
                            }
                        }

                        valorColumna = valorColumna.Replace("_", " ");
                        pSemPropModel.OntologyPropInfo.RepresentativeEntityTitles.Add(valorColumna);
                    }
                }
                else
                {
                    pSemPropModel.OntologyPropInfo.RepresentativeEntityTitles.Add("");
                }
            }
        }

        /// <summary>
        /// Obtiene el texto para una propiedad representante.
        /// </summary>
        /// <param name="pRepresentante">Representante</param>
        /// <param name="pEntidad">Entidad a la que pertenece la propiedad</param>
        /// <param name="pNumeracionSiCodigoRepresentacionIgualMenosUno">Numeración si el codigo de representacion es igual
        /// o menos que uno</param>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        /// <returns>Texto para una propiedad representante</returns>
        private string ObtenerTextoPropiedadRepresentante(Representante pRepresentante, SemanticEntityModel pEntidadModel, ref int pNumeracionSiCodigoRepresentacionIgualMenosUno, SemanticPropertyModel pSemPropModel)
        {
            string textoColumna = "";
            ElementoOntologia entidad = pEntidadModel.Entity;

            if (pRepresentante.Propiedad is PropiedadPlantilla)
            {
                textoColumna = entidad.TipoEntidad.Replace("_", " ");
            }
            else
            {
                int numCaractRecortar = 0;

                if (pRepresentante.Propiedad.ElementoOntologia == null)
                {
                    pRepresentante.Propiedad.ElementoOntologia = entidad;
                }

                List<string> valores = new List<string>();

                if (pRepresentante.Propiedad.ListaValoresIdioma.Count > 0)
                {
                    string aux = null;
                    valores = ObtenerValorSegunIdioma(pRepresentante.Propiedad, out aux, pSemPropModel);
                }
                else
                {
                    valores.AddRange(pRepresentante.Propiedad.ValoresUnificados.Keys);
                }

                if (pRepresentante.Propiedad.EspecifPropiedad.SelectorEntidad != null && pRepresentante.Propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "Tesauro" && pRepresentante.Propiedad.EspecifPropiedad.SelectorEntidad.TipoPresentacion == "Arbol")
                {
                    //Problema edición de un ODE, se agregaban los campos desordenados e impedían guardar el documento.
                    //Si es un tesauro y además con un orden de árbol, necesitamos que estén ordenados
                    valores.Sort();
                }

                foreach (string valorLista in valores)
                {
                    if (pRepresentante.Propiedad.EspecifPropiedad.SelectorEntidad != null)
                    {
                        if (pRepresentante.Propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "Combo")
                        {
                            SemanticPropertyModel propModel = pEntidadModel.GetProperty(pRepresentante.Propiedad.Nombre);

                            if (propModel != null && propModel.OntologyPropInfo.EntitySelector.EditionEntitiesValues != null && propModel.OntologyPropInfo.EntitySelector.EditionEntitiesValues.ContainsKey(valorLista))
                            {
                                textoColumna = propModel.OntologyPropInfo.EntitySelector.EditionEntitiesValues[valorLista];
                            }
                            else
                            {
                                textoColumna = valorLista;
                            }
                        }
                        else if (pSemPropModel.EntityParent.SemanticResourceModel.AuxiliaryCategoryTesSemNameInfo.Contains(valorLista + "|"))
                        {
                            string nombreCat = pSemPropModel.EntityParent.SemanticResourceModel.AuxiliaryCategoryTesSemNameInfo;
                            nombreCat = nombreCat.Substring(nombreCat.IndexOf(valorLista + "|") + valorLista.Length + 1);
                            nombreCat = nombreCat.Substring(0, nombreCat.IndexOf("|||"));
                            textoColumna += nombreCat;
                        }
                        else
                        {
                            textoColumna += valorLista;
                        }

                        if (pRepresentante.Propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "Tesauro")
                        {
                            textoColumna += " > ";
                            numCaractRecortar = 3;
                        }
                        else
                        {
                            break;//Cogo solo el 1º valor
                        }
                    }
                    else
                    {
                        //Cogo solo el 1º valor:
                        textoColumna = FormatearSegunPropiedadValor(pRepresentante.Propiedad, valorLista, false);

                        if (textoColumna.EndsWith(" 00:00:00"))
                        {
                            textoColumna = textoColumna.Replace(" 00:00:00", "");
                        }
                        break;
                    }
                }

                if (numCaractRecortar > 0)
                {
                    textoColumna = textoColumna.Substring(0, textoColumna.Length - numCaractRecortar);
                }
            }

            if (pRepresentante.TipoRepres == TipoRepresentacion.TipoEntidadMasID)
            {
                pNumeracionSiCodigoRepresentacionIgualMenosUno++;
                textoColumna = entidad.TipoEntidad + pNumeracionSiCodigoRepresentacionIgualMenosUno.ToString();
            }
            else if (pRepresentante.TipoRepres == TipoRepresentacion.SoloID)
            {
                pNumeracionSiCodigoRepresentacionIgualMenosUno++;
                textoColumna = pNumeracionSiCodigoRepresentacionIgualMenosUno.ToString();
            }
            else if (pRepresentante.TipoRepres == TipoRepresentacion.TodosLosCaracteres)
            {
                //Vacío, esta bien con el valor que tenga.
            }
            else if (pRepresentante.TipoRepres == TipoRepresentacion.NumCaracteresExactos)
            {
                try
                {
                    int nSubString = pRepresentante.NumCaracteres;
                    if (nSubString < textoColumna.Length)
                    {
                        textoColumna = textoColumna.Substring(0, nSubString);
                    }
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex);
                }
            }

            return textoColumna;
        }

        #endregion

        #region DataProperty

        /// <summary>
        /// Agrega una propiedad de tipo datos.
        /// </summary>
        /// <param name="pElemOrdenado">Elemento ordenado del modelo de propiedad</param>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        private void AgregarDataProperty(ElementoOrdenado pElemOrdenado, SemanticPropertyModel pSemPropModel, Guid pIdentidadID)
        {
            Propiedad propiedad = pSemPropModel.Element.Propiedad;
            pSemPropModel.OntologyPropInfo.FieldType = propiedad.EspecifPropiedad.TipoCampo;

            if (!pSemPropModel.EntityParent.SemanticResourceModel.ReadMode)
            {
                if ((propiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.Video || propiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.Imagen || propiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.Archivo || propiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.ArchivoLink))
                {//Así funcionan los fileUpload
                    pSemPropModel.OntologyPropInfo.ControlID = "Campo_" + propiedad.NombreGeneracionIDs + "_ent_" + propiedad.ElementoOntologia.TipoEntidadGeneracionIDs;
                }
                else
                {
                    pSemPropModel.OntologyPropInfo.ControlID = "Campo_" + propiedad.NombreGeneracionIDs + Guid.NewGuid();
                }

                RegistrarIDControlPropiedad(propiedad, pSemPropModel.OntologyPropInfo.ControlID);
            }

            if (propiedad.EspecifPropiedad.ValorDefectoNoSeleccionable != null)
            {
                pSemPropModel.OntologyPropInfo.DefaultUnselectableValue = UtilCadenas.ObtenerTextoDeIdioma(propiedad.EspecifPropiedad.ValorDefectoNoSeleccionable, Ontologia.IdiomaUsuario, null);
            }

            pSemPropModel.OntologyPropInfo.UniqueValue = (propiedad.FunctionalProperty || propiedad.CardinalidadMenorOIgualUno || propiedad.EspecifPropiedad.ValoresSepComas || pElemOrdenado.SoloPrimerValor);
            pSemPropModel.OntologyPropInfo.MinCardinality = propiedad.CardinalidadMinima;
            pSemPropModel.OntologyPropInfo.MaxCardinality = propiedad.CardinalidadMaxima;
            pSemPropModel.OntologyPropInfo.FunctionalProperty = propiedad.FunctionalProperty;
            string idiomaValorSeleccionado = null;
            pSemPropModel.OntologyPropInfo.PropertyValues = new List<SemanticPropertyModel.PropertyValue>();

            if (pSemPropModel.OntologyPropInfo.UniqueValue)
            {
                string valorGuardado = ObtenerValorGuardadoPropiedadDatos(propiedad, out idiomaValorSeleccionado, pSemPropModel);
                EstablecerValorPropiedadDatos(pSemPropModel, valorGuardado, idiomaValorSeleccionado, pIdentidadID);
            }
            else
            {
                pSemPropModel.OntologyPropInfo.ItIsPossibleToSddMoreValues = SePuedeAgregarMasValoresAPropiedad(propiedad);

                foreach (string valorGuardado in ObtenerValoresGuardadoPropiedadDatos(propiedad, out idiomaValorSeleccionado, pSemPropModel))
                {
                    EstablecerValorPropiedadDatos(pSemPropModel, valorGuardado, idiomaValorSeleccionado, pIdentidadID);
                }
            }

            DarTextoCorrectoTituloProp(pSemPropModel);

            pSemPropModel.OntologyPropInfo.MultiLanguage = PropiedadEsMultidioma(propiedad);

            if (pSemPropModel.OntologyPropInfo.MultiLanguage)
            {
                pSemPropModel.OntologyPropInfo.MultiLanguageWithTabs = (EsEntidadPrincipalMirandoHerencias(propiedad.ElementoOntologia) || propiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.Archivo || propiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.ArchivoLink);
                EstablecerValoresMultiIdiomaPropiedadDatos(pSemPropModel);
            }

            if (pElemOrdenado.MensajeAyuda != null)
            {
                pSemPropModel.OntologyPropInfo.HelpText = UtilCadenas.ObtenerTextoDeIdioma(pElemOrdenado.MensajeAyuda, Ontologia.IdiomaUsuario, null);
            }

            ConfigurarAutocompletarTitDesc(pSemPropModel);

            AgregarRDFAYMicrodatos(pSemPropModel);
        }

        /// <summary>
        /// Configura el autocompletar para los campos configurados como título o descripción del recurso.
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        private void ConfigurarAutocompletarTitDesc(SemanticPropertyModel pSemPropModel)
        {
            Propiedad propiedad = pSemPropModel.Element.Propiedad;

            //Titulo representante Doc
            if (!mSemRecModel.ReadMode && propiedad.Ontologia.ConfiguracionPlantilla.PropiedadTitulo.Key == propiedad.Nombre && (propiedad.Ontologia.ConfiguracionPlantilla.PropiedadTitulo.Value == propiedad.ElementoOntologia.TipoEntidad || propiedad.ElementoOntologia.SuperclasesUtiles.Contains(propiedad.Ontologia.ConfiguracionPlantilla.PropiedadTitulo.Value)))
            {
                if (string.IsNullOrEmpty(mTxtTituloDocSemID))
                {
                    mTxtTituloDocSemID = pSemPropModel.OntologyPropInfo.ControlID;
                }
                else
                {
                    mTxtTituloDocSemID += "," + pSemPropModel.OntologyPropInfo.ControlID;
                }
            }

            //Descripcion representante Doc
            if (!mSemRecModel.ReadMode && propiedad.Ontologia.ConfiguracionPlantilla.PropiedadDescripcion.Key == propiedad.Nombre && (propiedad.Ontologia.ConfiguracionPlantilla.PropiedadDescripcion.Value == propiedad.ElementoOntologia.TipoEntidad || propiedad.ElementoOntologia.SuperclasesUtiles.Contains(propiedad.Ontologia.ConfiguracionPlantilla.PropiedadDescripcion.Value)))
            {
                if (string.IsNullOrEmpty(mTxtDescripcionDocSemID))
                {
                    mTxtDescripcionDocSemID = pSemPropModel.OntologyPropInfo.ControlID;
                }
                else
                {
                    mTxtDescripcionDocSemID += "," + pSemPropModel.OntologyPropInfo.ControlID;
                }
            }
        }

        /// <summary>
        /// Comprueba si una entidad o alguna herencia de la misma es entidad principal.
        /// </summary>
        /// <param name="pTipoEntidad">Tipo de entidad</param>
        private bool EsEntidadPrincipalMirandoHerencias(ElementoOntologia pEntidad)
        {
            if (mEntidades.Contains(pEntidad))
            {
                return true;
            }

            foreach (string superClase in pEntidad.SuperclasesUtiles)
            {
                foreach (ElementoOntologia entidadPrin in mEntidades)
                {
                    if (entidadPrin.TipoEntidad == superClase)
                    {
                        return true;
                    }
                    else
                    {
                        foreach (string superClaseEntPrinc in entidadPrin.SuperclasesUtiles)
                        {
                            if (superClaseEntPrinc == superClase)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Registra el ID de una propiedad para manejarlo con JS.
        /// </summary>
        /// <param name="pPropiedad">Propiedad</param>
        /// <param name="pID">ID control</param>
        private void RegistrarIDControlPropiedad(Propiedad pPropiedad, string pID)
        {
            if (string.IsNullOrEmpty(mSemRecModel.AuxiliaryIDRegisterInfo))
            {
                mSemRecModel.AuxiliaryIDRegisterInfo = "|";
            }
            string idElem = pPropiedad.ElementoOntologia.TipoEntidad + "," + pPropiedad.Nombre + ",";
            if (!mSemRecModel.AuxiliaryIDRegisterInfo.Contains(idElem))
            {
                mSemRecModel.AuxiliaryIDRegisterInfo += idElem + pID + "|";
            }
            else
            {
                string antesProp = mSemRecModel.AuxiliaryIDRegisterInfo.Substring(0, mSemRecModel.AuxiliaryIDRegisterInfo.IndexOf(idElem));
                string despuesProp = mSemRecModel.AuxiliaryIDRegisterInfo.Substring(mSemRecModel.AuxiliaryIDRegisterInfo.IndexOf(idElem));
                despuesProp = despuesProp.Substring(despuesProp.IndexOf("|") + 1);

                mSemRecModel.AuxiliaryIDRegisterInfo = antesProp + idElem + pID + "|" + despuesProp;
            }
        }

        /// <summary>
        /// Establece los valores multiIdioma de la propiedad
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        private void EstablecerValoresMultiIdiomaPropiedadDatos(SemanticPropertyModel pSemPropModel)
        {
            pSemPropModel.OntologyPropInfo.PropertyLanguageValues = new Dictionary<string, List<SemanticPropertyModel.PropertyValue>>();

            foreach (string idioma in pSemPropModel.Element.Propiedad.ListaValoresIdioma.Keys)
            {
                List<SemanticPropertyModel.PropertyValue> propValues = new List<SemanticPropertyModel.PropertyValue>();

                foreach (string valor in pSemPropModel.Element.Propiedad.ListaValoresIdioma[idioma].Keys)
                {
                    SemanticPropertyModel.PropertyValue propValue = new SemanticPropertyModel.PropertyValue();
                    propValue.Property = pSemPropModel;
                    propValue.Value = valor;

                    propValues.Add(propValue);
                }

                pSemPropModel.OntologyPropInfo.PropertyLanguageValues.Add(idioma, propValues);
            }
        }

        /// <summary>
        /// Establece el valor de la propiedad de tipo datos.
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        /// <param name="pValor">Valor</param>
        /// <param name="pIdiomaValor">Idioma del valor</param>
        private void EstablecerValorPropiedadDatos(SemanticPropertyModel pSemPropModel, string pValor, string pIdiomaValor, Guid pIdentidadID)
        {
            Propiedad propiedad = pSemPropModel.Element.Propiedad;
            SemanticPropertyModel.PropertyValue propertyValue = new SemanticPropertyModel.PropertyValue();
            propertyValue.Property = pSemPropModel;

            if (pSemPropModel.ReadMode)
            {
                if (propiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.Archivo)
                {
                    propertyValue.DownloadUrl = ObtenerLinkDescargaArchivos(pValor, pIdiomaValor, propiedad.ElementoOntologia, pIdentidadID);
                    pValor = EliminarGuidControlArchivosDeValor(pValor);

                }
                else if (propiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.ArchivoLink)
                {
                    propertyValue.DownloadUrl = mBaseURLContent + "/" + pValor;
                    pValor = EliminarGuidControlArchivosDeValor(pValor);
                }
                else if (propiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.Tiny)
                {
                    #region Imagen captura en descripción

                    if (mOntologia.ConfiguracionPlantilla.PropiedadDescripcion.Key != null && mOntologia.ConfiguracionPlantilla.PropiedadDescripcion.Key == propiedad.Nombre && (mOntologia.ConfiguracionPlantilla.PropiedadDescripcion.Value == propiedad.ElementoOntologia.TipoEntidad || propiedad.ElementoOntologia.SuperclasesUtiles.Contains(mOntologia.ConfiguracionPlantilla.PropiedadDescripcion.Value)) && mDocumento != null && mDocumento.FilaDocumento.NombreCategoriaDoc != null && (mDocumento.FilaDocumento.NombreCategoriaDoc.ToLower().Contains("/" + UtilArchivos.ContentImgCapSemanticasAntiguo + "/") || mDocumento.FilaDocumento.NombreCategoriaDoc.ToLower().Contains("/" + UtilArchivos.ContentImgCapSemanticas + "/")))
                    {
                        bool pintarImg = true;
                        List<KeyValuePair<string, string>> urlCaptura = mOntologia.ConfiguracionPlantilla.PropiedadImagenFromURL;

                        if (urlCaptura != null)
                        {
                            Propiedad propUrlCaptura = null;
                            string tipoEntPropcapt = null;

                            foreach (KeyValuePair<string, string> propCap in urlCaptura)
                            {
                                foreach (ElementoOntologia entidadPrinc in mEntidades)
                                {
                                    propUrlCaptura = GestionOWL.ObtenerPropiedadDeTipoEntidad(propCap.Key, propCap.Value, entidadPrinc);

                                    if (propUrlCaptura != null)
                                    {
                                        break;
                                    }
                                }

                                if (propUrlCaptura != null && propUrlCaptura.ValoresUnificados.Count > 0)
                                {
                                    tipoEntPropcapt = propCap.Value;
                                    break;
                                }
                            }

                            if (tipoEntPropcapt != null)
                            {
                                if (propUrlCaptura.ElementoOntologia == null)
                                {
                                    propUrlCaptura.ElementoOntologia = mOntologia.GetEntidadTipo(tipoEntPropcapt, false);
                                }

                                pintarImg = (propUrlCaptura.EspecifPropiedad.TipoCampo != TipoCampoOntologia.EmbebedLink);
                            }
                        }

                        if (pintarImg)
                        {
                            string urlImg = ControladorDocumentacion.ObtenerRutaImagenDocumento(mDocumento, mBaseURLContent, mBaseURLStatic, true, null, null, "240", ProyectoSeleccionado.Clave);
                            pValor = "<p class='miniatura'><a href=\"#\"><img title='" + mDocumento.Titulo + "' alt='" + mDocumento.Titulo + "' src='" + urlImg + "'></a></p>" + pValor;
                        }
                    }

                    #endregion
                }
                else if (propiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.EmbebedLink)
                {
                    ObtenerLinkEmbebedLink(pValor, propertyValue);
                }
                else if (propiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.EmbebedObject)
                {
                    propertyValue.EmbebedObject = propiedad.EspecifPropiedad.HtmlObjeto.Replace("@1@", pValor);
                }
                else if (propiedad.EspecifPropiedad.GrafoDependiente != null && propiedad.ValorUnico)
                {
                    pValor = mDatosEntidadesGrafoDep[propiedad.EspecifPropiedad.GrafoDependiente][pValor];
                }
                else if (propiedad.ElementoOntologia.EspecifEntidad.CampoRepresentanteOrden != null && propiedad.ElementoOntologia.EspecifEntidad.CampoRepresentanteOrden == propiedad.Nombre)
                {
                    pValor = propiedad.ElementoOntologia.OrdenEntiadTexto + pValor;
                }
                else if (propiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.Link || !string.IsNullOrEmpty(propiedad.EspecifPropiedad.UrlLinkDelValor))
                {
                    string url = pValor;

                    if (!string.IsNullOrEmpty(propiedad.EspecifPropiedad.UrlLinkDelValor))
                    {
                        url = propiedad.EspecifPropiedad.UrlLinkDelValor;
                    }

                    if (url != null && url.IndexOf("http://") != 0 && url.IndexOf("https://") != 0)
                    {
                        url = "http://" + url;
                    }

                    propertyValue.DownloadUrl = url;
                }
            }
            else
            {
                if (propiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.Imagen || propiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.Video || propiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.Archivo || propiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.Checks || propiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.ArchivoLink)
                {
                    if (!string.IsNullOrEmpty(pValor))
                    {
                        if (propiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.Checks && !propiedad.FunctionalProperty && !propiedad.CardinalidadMenorOIgualUno)
                        {
                            pValor += ",";
                        }
                    }
                    else if (propiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.Checks && (propiedad.FunctionalProperty || propiedad.CardinalidadMenorOIgualUno))
                    {
                        pValor = propiedad.ListaValoresPermitidos[0];
                    }
                }
                else if (propiedad.EspecifPropiedad.GrafoDependiente != null && propiedad.ValorUnico && propiedad.EspecifPropiedad.TipoDependiente == "AutoCompletar")
                {
                    pSemPropModel.OntologyPropInfo.PropDependentGraph = mUrlIntragnoss + propiedad.EspecifPropiedad.GrafoDependiente;

                    if (!string.IsNullOrEmpty(pValor))
                    {
                        if (mDatosEntidadesGrafoDep.ContainsKey(propiedad.EspecifPropiedad.GrafoDependiente) && mDatosEntidadesGrafoDep[propiedad.EspecifPropiedad.GrafoDependiente].ContainsKey(pValor))
                        {
                            pSemPropModel.OntologyPropInfo.AuxiliaryControlValue = mDatosEntidadesGrafoDep[propiedad.EspecifPropiedad.GrafoDependiente][pValor];
                        }
                        else
                        {
                            pSemPropModel.OntologyPropInfo.AuxiliaryControlValue = "";
                        }
                    }
                    else if (propiedad.EspecifPropiedad.PropDependiente.Key != null)
                    {
                        Propiedad propDep = null;

                        foreach (ElementoOntologia entidad in mEntidades)
                        {
                            propDep = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(propiedad.EspecifPropiedad.PropDependiente.Key, propiedad.EspecifPropiedad.PropDependiente.Value, entidad);

                            if (propDep != null)
                            {
                                break;
                            }
                        }

                        pSemPropModel.OntologyPropInfo.AuxiliaryControlDisabled = !(propDep != null && propDep.ValoresUnificados.Count > 0);
                    }
                }
                else if (propiedad.ElementoOntologia.EspecifEntidad.CampoRepresentanteOrden != null && propiedad.ElementoOntologia.EspecifEntidad.CampoRepresentanteOrden == propiedad.Nombre)
                {
                    pValor = propiedad.ElementoOntologia.OrdenEntiadTexto + pValor;
                }
            }

            if (pSemPropModel.Element.Link != null)
            {
                string urlLink = pSemPropModel.Element.Link;

                if (urlLink.IndexOf("@") == 0)
                {
                    Propiedad prop = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(urlLink.Substring(1), mEntidades[0]);

                    if (prop != null)
                    {
                        urlLink = prop.PrimerValorPropiedad;
                    }
                }
                else if (urlLink.IndexOf("%") == 0)
                {
                    Propiedad prop = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(urlLink.Substring(1), mEntidades[0]);

                    if (prop != null)
                    {
                        urlLink = prop.PrimerValorPropiedad;
                    }

                    if (!string.IsNullOrEmpty(urlLink))
                    {
                        urlLink = ObtenerLinkDescargaArchivos(urlLink, pIdiomaValor, prop.ElementoOntologia, pIdentidadID);
                    }
                }
                else if (urlLink.IndexOf("#") == 0)
                {
                    urlLink = ObtenerLinkABusquedaFiltrada(pValor, pSemPropModel.Element.Link, pIdiomaValor);
                }

                if (!string.IsNullOrEmpty(urlLink))
                {
                    if (!urlLink.StartsWith("http://") && !urlLink.StartsWith("https://"))
                    {
                        urlLink = "http://" + urlLink;
                    }

                    propertyValue.UrlAuxiliaryLinkControl = urlLink;
                }
            }

            if (!string.IsNullOrEmpty(pValor))
            {
                propertyValue.Value = pValor;
                propertyValue.LanguageOfValue = pIdiomaValor;
                pSemPropModel.OntologyPropInfo.PropertyValues.Add(propertyValue);
            }
        }

        /// <summary>
        /// Elimina el GUID de control que forma parte del valor del archivo.
        /// </summary>
        /// <param name="pValor">Valor del archivo</param>
        /// <returns>Valor del archivo si el GUID de control</returns>
        private string EliminarGuidControlArchivosDeValor(string pValor)
        {
            if (pValor.Contains(".") && pValor.Contains("_"))
            {
                string guidValor = pValor.Substring(0, pValor.LastIndexOf("."));
                guidValor = guidValor.Substring(guidValor.LastIndexOf("_") + 1);
                Guid aux = Guid.Empty;

                if (Guid.TryParse(guidValor, out aux))
                {
                    pValor = pValor.Substring(0, pValor.LastIndexOf("_")) + pValor.Substring(pValor.LastIndexOf("."));
                }
            }

            return pValor;
        }

        /// <summary>
        /// Agregar los atributos RDFA y microdatos.
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        private void AgregarRDFAYMicrodatos(SemanticPropertyModel pSemPropModel)
        {
            if (!mSemRecModel.ReadMode || string.IsNullOrEmpty(pSemPropModel.Element.Propiedad.ElementoOntologia.Uri))
            {
                return;
            }

            string superPropiedad = "";

            foreach (string superProp in pSemPropModel.Element.Propiedad.SuperPropiedades)
            {
                KeyValuePair<string, string> urlProp = GestionOWL.SepararURLDeElemento(superProp);

                if (urlProp.Key != null && pSemPropModel.Element.Propiedad.Ontologia.NamespacesDefinidos.ContainsKey(urlProp.Key))
                {
                    superPropiedad += " " + pSemPropModel.Element.Propiedad.Ontologia.NamespacesDefinidos[urlProp.Key] + ":" + urlProp.Value;
                }
            }

            pSemPropModel.OntologyPropInfo.PropertyRDFA = pSemPropModel.Element.Propiedad.NombreConNamespace + superPropiedad;
            pSemPropModel.OntologyPropInfo.AboutRDFA = pSemPropModel.Element.Propiedad.ElementoOntologia.Uri;

            if (pSemPropModel.OntologyPropInfo.PropertyRDFA.Contains("http://"))
            {
                string urlName = null;
                string tipo = null;

                if (pSemPropModel.OntologyPropInfo.PropertyRDFA.Contains("#"))
                {
                    urlName = pSemPropModel.OntologyPropInfo.PropertyRDFA.Substring(0, pSemPropModel.OntologyPropInfo.PropertyRDFA.LastIndexOf("#") + 1);
                    tipo = pSemPropModel.OntologyPropInfo.PropertyRDFA.Substring(pSemPropModel.OntologyPropInfo.PropertyRDFA.LastIndexOf("#") + 1);
                }
                else if (pSemPropModel.OntologyPropInfo.PropertyRDFA.Contains("/"))
                {
                    urlName = pSemPropModel.OntologyPropInfo.PropertyRDFA.Substring(0, pSemPropModel.OntologyPropInfo.PropertyRDFA.LastIndexOf("/") + 1);
                    tipo = pSemPropModel.OntologyPropInfo.PropertyRDFA.Substring(pSemPropModel.OntologyPropInfo.PropertyRDFA.LastIndexOf("/") + 1);
                }

                if (!string.IsNullOrEmpty(urlName) && !string.IsNullOrEmpty(tipo) && Ontologia.NamespacesDefinidos.ContainsKey(urlName))
                {
                    pSemPropModel.OntologyPropInfo.PropertyRDFA = Ontologia.NamespacesDefinidos[urlName] + ":" + tipo;
                }
            }
        }

        /// <summary>
        /// Asigna el título correcto a la propiedad.
        /// </summary>
        /// <param name="pSemPropModel">Modelo de propiedad</param>
        private void DarTextoCorrectoTituloProp(SemanticPropertyModel pSemPropModel)
        {
            string posibleNombrePropiedad = pSemPropModel.Element.Propiedad.EspecifPropiedad.NombrePropiedad(pSemPropModel.EntityParent.SemanticResourceModel.ReadMode);

            if (posibleNombrePropiedad != null)
            {
                if (posibleNombrePropiedad != "")
                {
                    pSemPropModel.OntologyPropInfo.LabelTitle = posibleNombrePropiedad;
                }
            }
            else
            {
                pSemPropModel.OntologyPropInfo.LabelTitle = pSemPropModel.Element.Propiedad.Nombre; // + ": ";
                pSemPropModel.OntologyPropInfo.LabelTitle = pSemPropModel.OntologyPropInfo.LabelTitle.Replace("_", " ");
            }
        }

        /// <summary>
        /// Obtiene la URL Del link embebido de la propiedad.
        /// </summary>
        /// <param name="pValor">Valor de la propiedad</param>
        /// <param name="pPropertyValue">Modelo de valor de propiedad</param>
        /// <returns>URL Del link embebido de la propiedad</returns>
        private void ObtenerLinkEmbebedLink(string pValor, SemanticPropertyModel.PropertyValue pPropertyValue)
        {
            if (pValor.StartsWith("http://www.youtube.com") || pValor.StartsWith("http://youtube.com") || pValor.StartsWith("https://www.youtube.com") || pValor.StartsWith("https://youtube.com") || pValor.StartsWith("www.youtube.com") || pValor.StartsWith("youtu.be/") || pValor.StartsWith("http://youtu.be/") || pValor.StartsWith("https://youtu.be/"))
            {
                string v = "";
                if (pValor.Contains("//youtu.be/"))
                {
                    v = pValor.Replace("http://youtu.be/", "").Replace("https://youtu.be/", "");

                    if (v.Contains("/"))
                    {
                        v = v.Substring(0, v.IndexOf("/"));
                    }
                }
                else
                {
                    v = HttpUtility.ParseQueryString(new Uri(pValor).Query).Get("v");
                }

                //si el enlace viene de youtube y el parámetro v no es null
                if (v != null)
                {
                    pPropertyValue.EmbebedLinkYoutube = mHttpContextAccessor.HttpContext.Request.Scheme + "://www.youtube.com/embed/" + v;
                }
            }
            else if (pValor.StartsWith("http://www.vimeo.com") || pValor.StartsWith("http://vimeo.com") || pValor.StartsWith("https://www.vimeo.com") || pValor.StartsWith("https://vimeo.com") || pValor.StartsWith("www.vimeo.com"))
            {
                string v = (new Uri(pValor)).AbsolutePath;
                int idVideo;
                int inicio = v.LastIndexOf("/");
                bool exito = int.TryParse(v.Substring(inicio + 1, v.Length - inicio - 1), out idVideo);

                if (exito)
                {
                    pPropertyValue.EmbebedLinkVimeo = mHttpContextAccessor.HttpContext.Request.Scheme + "://player.vimeo.com/video/" + idVideo.ToString();
                }
            }
            else if (pValor.StartsWith("http://www.slideshare.net") || pValor.StartsWith("http://slideshare.net") || pValor.StartsWith("https://www.slideshare.net") || pValor.StartsWith("https://slideshare.net") || pValor.StartsWith("www.slideshare.net") || pValor.StartsWith("slideshare.net") || pValor.Contains("slideshare.net"))
            {
                string api_key = "KL6JLkEq";
                string shared_secret = "Nnmk51ur";

                //timestamp               
                string timestamp = Convert.ToInt32(Math.Floor((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds)).ToString();

                //hash
                byte[] buffer = Encoding.UTF8.GetBytes(shared_secret + timestamp);
                SHA1CryptoServiceProvider cryptoTransformSHA1 =
                new SHA1CryptoServiceProvider();
                string hash = BitConverter.ToString(
                    cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "").ToLower();

                string embedCode = "No disponible";
                string ruta = "https://www.slideshare.net/api/2/get_slideshow?api_key=" + api_key + "&ts=" + timestamp + "&hash=" + hash + "&slideshow_url=" + pValor;

                try
                {
                    using (XmlTextReader reader = new XmlTextReader(ruta))
                    {
                        reader.MoveToContent();
                        reader.ReadStartElement();
                        while (reader.Read())
                        {
                            //codigo embebido
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Embed")
                            {
                                embedCode = reader.ReadString();

                                string titulo = embedCode.Substring(embedCode.IndexOf("<strong style"), embedCode.IndexOf("</strong>", embedCode.IndexOf("<strong style")) - embedCode.IndexOf("<strong style") + 9);

                                //Eliminamos el título del código embebido
                                embedCode = embedCode.Replace(titulo, "");

                                //Modificamos el tamaño para que sea más pequeño
                                embedCode = embedCode.Replace("width=\"425\"", "width=\"360\"");
                                embedCode = embedCode.Replace("width:425", "width:360");
                                embedCode = embedCode.Replace("height=\"355\"", "height=\"257\"");
                                embedCode = embedCode.Replace("height:355", "height:257");

                                int indexOfEmbed = embedCode.IndexOf("<embed");

                                embedCode = embedCode.Substring(0, indexOfEmbed) + "<param name=\"wmode\" value=\"transparent\">" + embedCode.Substring(indexOfEmbed, 6) + " wmode=\"transparent\" " + embedCode.Substring(indexOfEmbed + 6);

                            }
                        }
                        reader.Close();
                    }
                }
                catch (Exception)
                {
                }


                try
                {
                    string src = embedCode.Substring(embedCode.IndexOf("src=") + 5);
                    src = src.Substring(0, src.IndexOf("\""));

                    pPropertyValue.EmbebedLinkSlideshare = src;
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Devuelve el link para descargar el fichero de la propiedad.
        /// </summary>
        /// <param name="pValor">Valor de la propiedad</param>
        /// <param name="pIdiomaValor">Idioma del valor</param>
        /// <param name="pEntidad">Entidad que contiene la propiedad con el valor del archivo</param>
        /// <returns>Link para descargar el fichero de la propiedad</returns>
        private string ObtenerLinkDescargaArchivos(string pValor, string pIdiomaValor, ElementoOntologia pEntidad, Guid pIdentidadID)
        {
            string extension = null;
            string archivo = null;

            if (pValor != null && pValor != "")
            {
                extension = pValor.Substring(pValor.LastIndexOf(".")).ToLower();
                archivo = pValor.Substring(0, pValor.LastIndexOf("."));
            }

            Guid documentoDescargaID = mDocumentoID;
            string extraDesc = "";

            if (pEntidad != null && pEntidad.Ontologia.OntoAuxiliarInventada && pEntidad.ID.Contains("_"))
            {
                string docExt = pEntidad.ID.Substring(0, pEntidad.ID.LastIndexOf("_"));

                if (docExt.Contains("_"))
                {
                    docExt = docExt.Substring(docExt.LastIndexOf("_") + 1);
                    Guid.TryParse(docExt, out documentoDescargaID);
                    extraDesc = "&dscr=true";
                }
            }

            if (mDocumento.ProyectoID != mProyectoActual.Clave)
            {
                extraDesc += "&proyectoID=" + mProyectoActual.Clave.ToString();
            }

            //Codificamos el nombre el fichero para no tener problemas con simbolos como el + (se convierte en espacio)
            string baseUrl = UtilDominios.ObtenerDominioUrl(ProyectoSeleccionado.UrlPropia(UtilIdiomas.LanguageCode), true);
            string urlComunidad = UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto);
            string enlace = $"{urlComunidad}/download-file?doc={documentoDescargaID}&ext={extension}&archivoAdjuntoSem={archivo}&ontologiaAdjuntoSem={mOntologia.OntologiaID}&ID={pIdentidadID}&proy={mDocumento.ProyectoID}{extraDesc}";
            if (!string.IsNullOrEmpty(pIdiomaValor))
            {
                enlace += $"&idiomaFichero={pIdiomaValor}";
            }

            return enlace;
        }

        /// <summary>
        /// Obtiene el link para realizar un búsqueda filtrada por el valor de la propiedad actual.
        /// </summary>
        /// <param name="pValorGuardado">Valor guardado</param>
        /// <param name="pLink">Link</param>
        /// <param name="pIdiomaValorSeleccionado">Idioma del valor seleccionado</param>
        private string ObtenerLinkABusquedaFiltrada(string pValorGuardado, string pLink, string pIdiomaValorSeleccionado)
        {
            if (string.IsNullOrEmpty(pValorGuardado) || mDocumento == null || !mDocumento.GestorDocumental.ListaDocumentos.ContainsKey(mOntologia.OntologiaID) /*|| mDatosPeticion.EsBot*/)
            {
                return "";
            }

            string nombreSem = "";
            string nombreProy;
            string rdf = mDocumento.GestorDocumental.ListaDocumentos[mOntologia.OntologiaID].Enlace;
            rdf = rdf.Substring(0, rdf.IndexOf(".owl"));

            DataWrapperProyecto pestanyasProyecto;

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            nombreProy = mProyectoActual.NombreCorto;
            pestanyasProyecto = proyCL.ObtenerPestanyasProyecto(mProyectoActual.Clave);
            proyCL.Dispose();

            foreach (AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaBusqueda filaPestanyaBusqueda in pestanyasProyecto.ListaProyectoPestanyaBusqueda)
            {
                if (filaPestanyaBusqueda.CampoFiltro.Contains("rdf:type=" + rdf))
                {
                    string idiomaSeleccionado = mUtilIdiomas.LanguageCode;
                    if (!string.IsNullOrEmpty(pIdiomaValorSeleccionado))
                    {
                        idiomaSeleccionado = pIdiomaValorSeleccionado;
                    }

                    nombreSem = filaPestanyaBusqueda.ProyectoPestanyaMenu.Ruta;
                    nombreSem = UtilCadenas.ObtenerTextoDeIdioma(nombreSem, idiomaSeleccionado, null);
                    break;
                }
            }

            string idioma = null;

            if (!string.IsNullOrEmpty(pIdiomaValorSeleccionado))
            {
                idioma = "@" + pIdiomaValorSeleccionado;
            }

            FacetaCL facetaCL = new FacetaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionFacetas gestorFacetas = new GestionFacetas(facetaCL.ObtenerFacetasDeProyecto(null, mProyectoActual.Clave, false), mLoggingService);
            facetaCL.Dispose();

            string valorGuardado = pValorGuardado.ToLower();

            if (gestorFacetas != null && gestorFacetas.ListaFacetasPorClave != null && gestorFacetas.ListaFacetasPorClave.ContainsKey(pLink.Replace("#", "")))
            {
                Faceta faceta = gestorFacetas.ListaFacetasPorClave[pLink.Replace("#", "")];

                if (faceta.TipoPropiedad.Equals(TipoPropiedadFaceta.TextoInvariable))
                {
                    valorGuardado = pValorGuardado;
                }
            }

            return UrlsSemanticas.GetURLBaseRecursos(mBaseURLIdioma, mUtilIdiomas, nombreProy, null, false, nombreSem) + pLink.Replace("#", "?") + "=" + valorGuardado + idioma;
        }

        /// <summary>
        /// Obtiene el valor de la propiedad en el idioma adecuado según la situación y el idioma del usuario.
        /// </summary>
        /// <param name="pIdiomaValorSeleccionado">Idioma el valor guardado seleccionado</param>
        /// <param name="pPropiedad">Propiedad</param>
        /// <param name="pSemPropModel">Modelo de propiedad</param>
        /// <returns>Valor de la propiedad en el idioma adecuado</returns>
        private List<string> ObtenerValorSegunIdioma(Propiedad pPropiedad, out string pIdiomaValorSeleccionado, SemanticPropertyModel pSemPropModel)
        {
            string idiomaDefecto = mConfigService.ObtenerIdiomaDefecto();
            if (!pSemPropModel.ReadMode)
            {                
                if (string.IsNullOrEmpty(idiomaDefecto))
                {
                    throw new Exception("El recurso es multiIdioma, pero la comunidad no hay idiomas configurados.");
                }

                pIdiomaValorSeleccionado = idiomaDefecto;

                if (pPropiedad.ListaValoresIdioma.ContainsKey(idiomaDefecto))
                {
                    return new List<string>(pPropiedad.ListaValoresIdioma[idiomaDefecto].Keys);
                }
            }
            else if (pPropiedad.ListaValoresIdioma.ContainsKey(Ontologia.IdiomaUsuario))
            {
                pIdiomaValorSeleccionado = Ontologia.IdiomaUsuario;
                return new List<string>(pPropiedad.ListaValoresIdioma[Ontologia.IdiomaUsuario].Keys);
            }
            else if ((!pSemPropModel.ReadMode || idiomaDefecto != null) && pPropiedad.ListaValoresIdioma.ContainsKey(idiomaDefecto))
            {
                pIdiomaValorSeleccionado = idiomaDefecto;
                return new List<string>(pPropiedad.ListaValoresIdioma[idiomaDefecto].Keys);
            }
            else //Devuelvo el 1º valor del 1º idioma:
            {
                if (pPropiedad.ListaValoresIdioma.Any())
                {
                    pIdiomaValorSeleccionado = pPropiedad.ListaValoresIdioma.First().Key;
                    return new List<string>(pPropiedad.ListaValoresIdioma[pIdiomaValorSeleccionado].Keys);
                }
            }

            pIdiomaValorSeleccionado = null;
            return new List<string>();
        }

        /// <summary>
        /// Obtiene el valor de la propiedad de tipo datos.
        /// </summary>
        /// <param name="pIdiomaValorSeleccionado">Idioma el valor guardado seleccionado</param>
        /// <param name="pPropiedad">Propiedad</param>
        /// <param name="pSemPropModel">Modelo de la propiedad</param>
        /// <returns>Valor de la propiedad</returns>
        private List<string> ObtenerValoresGuardadoPropiedadDatos(Propiedad pPropiedad, out string pIdiomaValorSeleccionado, SemanticPropertyModel pSemPropModel)
        {
            List<string> valoresGuardado;
            string idiomaValorSeleccionado = null;

            if (pPropiedad.ListaValoresIdioma.Count > 0)
            {
                valoresGuardado = ObtenerValorSegunIdioma(pPropiedad, out idiomaValorSeleccionado, pSemPropModel);
            }
            else
            {
                valoresGuardado = new List<string>();
                valoresGuardado.AddRange(pPropiedad.ListaValores.Keys);
            }

            if (valoresGuardado.Count == 0 && pPropiedad.EspecifPropiedad.ValorPorDefecto != null)
            {
                valoresGuardado.Add(pPropiedad.EspecifPropiedad.ValorPorDefecto);
            }

            if (valoresGuardado.Count > 0)
            {
                List<string> listAux = new List<string>();

                foreach (string valor in valoresGuardado)
                {
                    listAux.Add(FormatearSegunPropiedadValor(pPropiedad, valor, pSemPropModel.ReadMode));
                }

                valoresGuardado = listAux;
            }

            pIdiomaValorSeleccionado = idiomaValorSeleccionado;
            return valoresGuardado;
        }

        /// <summary>
        /// Obtiene el valor de la propiedad de tipo datos.
        /// </summary>
        /// <param name="pIdiomaValorSeleccionado">Idioma el valor guardado seleccionado</param>
        /// <param name="pPropiedad">Propiedad</param>
        /// <param name="pSemPropModel">Modelo de la propiedad</param>
        /// <returns>Valor de la propiedad</returns>
        private string ObtenerValorGuardadoPropiedadDatos(Propiedad pPropiedad, out string pIdiomaValorSeleccionado, SemanticPropertyModel pSemPropModel)
        {
            string valorGuardado = null;
            string idiomaValorSeleccionado = null;

            if (pPropiedad.ListaValoresIdioma.Count > 0)
            {
                List<string> valoresGuard = ObtenerValorSegunIdioma(pPropiedad, out idiomaValorSeleccionado, pSemPropModel);

                if (valoresGuard.Count > 0)
                {
                    valorGuardado = valoresGuard[0];
                }
            }
            else
            {
                valorGuardado = pPropiedad.UnicoValor.Key;

                if (!pPropiedad.FunctionalProperty && valorGuardado == null)
                {
                    //Es una propiedad no funcional con cardinalidad-máxima = 1 -> Tiene 1 valor o ningunao.
                    if (pPropiedad.ListaValores.Count > 0)
                    {
                        string separador = "";
                        valorGuardado = "";
                        //Agregamos el 1º valor:
                        foreach (string valor in pPropiedad.ListaValores.Keys)
                        {
                            if (!string.IsNullOrEmpty(valor))
                            {
                                valorGuardado += separador + valor;
                                separador = ", ";
                            }
                        }
                    }
                }
            }

            if (valorGuardado == null && pPropiedad.EspecifPropiedad.ValorPorDefecto != null)
            {
                valorGuardado = pPropiedad.EspecifPropiedad.ValorPorDefecto;
            }

            valorGuardado = FormatearSegunPropiedadValor(pPropiedad, valorGuardado, pSemPropModel.ReadMode);

            pIdiomaValorSeleccionado = idiomaValorSeleccionado;
            return valorGuardado;
        }

        /// <summary>
        /// Formatea el valor de una propiedad según las características de ésta.
        /// </summary>
        /// <param name="pPropiedad">Propiedad</param>
        /// <param name="pValor">Valor</param>
        /// <param name="pReadMode">Indica si estamos en modo lectura</param>
        /// <returns>Valor formateado de una propiedad según las características de ésta</returns>
        private string FormatearSegunPropiedadValor(Propiedad pPropiedad, string pValor, bool pReadMode)
        {
            if (pValor != null)
            {
                pValor = pValor.Trim();

                if (pPropiedad.EspecifPropiedad.GuardarFechaComoEntero)
                {
                    try
                    {
                        string ano = pValor.Substring(0, 4);
                        string mes = pValor.Substring(4, 2);
                        string dia = pValor.Substring(6, 2);
                        string hora = null;

                        if (pPropiedad.EspecifPropiedad.FechaConHora && pValor.Length == 14)
                        {
                            hora = " " + pValor.Substring(8, 2) + ":" + pValor.Substring(10, 2) + ":" + pValor.Substring(12, 2);
                        }

                        pValor = dia + "/" + mes + "/" + ano + hora;
                    }
                    catch (Exception ex)
                    {
                        GuardarMensajeErrorAdmin("Fecha incorreta al convertirla de entero en propiedad '" + pPropiedad.Nombre + "'.", ex);
                        GuardarLogErrorAJAX(ex.ToString());
                        throw new Exception("Fecha incorreta al convertirla de entero en propiedad '" + pPropiedad.Nombre + "'.");
                    }
                }
                else if (pPropiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.Boleano)
                {
                    if (!pReadMode && pValor != "false" && pValor != "true")
                    {
                        if (pValor.ToLower() == mUtilIdiomas.GetText("CONTROLESDOCUMENTACION", "NO").ToLower())
                        {
                            pValor = "false";
                        }
                        else
                        {
                            pValor = "true";
                        }
                    }
                    else if (pReadMode && (pValor == "false" || pValor == "true"))
                    {
                        if (pValor == "false")
                        {
                            pValor = mUtilIdiomas.GetText("CONTROLESDOCUMENTACION", "NO");
                        }
                        else
                        {
                            pValor = mUtilIdiomas.GetText("CONTROLESDOCUMENTACION", "SI");
                        }
                    }
                }

                if (pReadMode && pPropiedad.EspecifPropiedad.ExpresionRegular != null)
                {
                    string valorAntiguo = pValor;
                    pValor = "";

                    Regex regExpr = new Regex(pPropiedad.EspecifPropiedad.ExpresionRegular);
                    MatchCollection collection = regExpr.Matches(valorAntiguo);

                    foreach (Match match in collection)
                    {
                        if (match.Groups.Count > 1)
                        {
                            for (int i = 1; i < match.Groups.Count; i++)
                            {
                                pValor += match.Groups[i];

                                if (pPropiedad.EspecifPropiedad.PrimeraCoincidenciaExpresionRegular)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            pValor += match.Value;
                        }

                        if (pPropiedad.EspecifPropiedad.PrimeraCoincidenciaExpresionRegular)
                        {
                            break;
                        }
                    }
                }
            }

            return pValor;
        }

        #endregion

        /// <summary>
        /// Indica si se pueden agregar más valores de los que tiene a una propiedad.
        /// </summary>
        /// <param name="pPropiedad">Propiedad</param>
        /// <returns></returns>
        private bool SePuedeAgregarMasValoresAPropiedad(Propiedad pPropiedad)
        {
            if (pPropiedad.ElementoOntologia.Restricciones.Count > 0)
            {
                bool cardinalidadCorrecta = true;
                foreach (Restriccion restriccion in pPropiedad.ElementoOntologia.Restricciones)
                {
                    if (pPropiedad.Nombre.Equals(restriccion.Propiedad) && (restriccion.TipoRestriccion == TipoRestriccion.Cardinality || restriccion.TipoRestriccion == TipoRestriccion.MaxCardinality))
                    {
                        cardinalidadCorrecta = (pPropiedad.ListaValores.Count < int.Parse(restriccion.Valor));
                        break;
                    }
                }
                return (cardinalidadCorrecta);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Comprueba si se debe pintar una propiedad.
        /// </summary>
        /// <param name="pPropiedad">Propiedad</param>
        /// <param name="pSemPropModel">Modelo de la propiedad</param>
        /// <returns>TRUE si se debe pintar una propiedad, FALSE en caso contrario</returns>
        private bool PintarPropiedad(Propiedad pPropiedad, SemanticPropertyModel pSemPropModel, Guid pIdentidadID)
        {
            if (PropiedadesOmitirPintado.ContainsKey(pPropiedad.Nombre) && PropiedadesOmitirPintado[pPropiedad.Nombre].Contains(pPropiedad.ElementoOntologia.TipoEntidad))
            {
                return false;
            }

            bool pintar = (!pSemPropModel.ReadMode || !string.IsNullOrEmpty(pPropiedad.UnicoValor.Key) || pPropiedad.ListaValores.Count > 1 || (pPropiedad.ListaValores.Count == 1 && !string.IsNullOrEmpty(pPropiedad.PrimerValorPropiedad)) || pPropiedad.ListaValoresIdioma.Count > 0 || (pPropiedad.EspecifPropiedad.SelectorEntidad != null && pPropiedad.EspecifPropiedad.SelectorEntidad.Reciproca));

            try
            {
                if (!pintar && pPropiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.Imagen && mDocumento.GestorDocumental.ListaDocumentos[mDocumento.ElementoVinculadoID].FilaDocumento.NombreCategoriaDoc != null && !string.IsNullOrEmpty(mDocumento.GestorDocumental.ListaDocumentos[mDocumento.ElementoVinculadoID].FilaDocumento.NombreCategoriaDoc) && pPropiedad.Ontologia.ConfiguracionPlantilla.PropiedadImagenRepre.Key == pPropiedad.Nombre && (pPropiedad.Ontologia.ConfiguracionPlantilla.PropiedadImagenRepre.Value == pPropiedad.ElementoOntologia.TipoEntidad || (pPropiedad.ElementoOntologia.SuperclasesUtiles.Count > 0 && pPropiedad.ElementoOntologia.SuperclasesUtiles.Contains(pPropiedad.Ontologia.ConfiguracionPlantilla.PropiedadImagenRepre.Value))))
                {
                    string foto = mDocumento.GestorDocumental.ListaDocumentos[mDocumento.ElementoVinculadoID].FilaDocumento.NombreCategoriaDoc;

                    if (foto.Contains(","))
                    {
                        string tamanyo = foto.Substring(0, foto.LastIndexOf(","));
                        foto = foto.Substring(foto.LastIndexOf(",") + 1);

                        if (tamanyo.Contains(","))
                        {
                            tamanyo = tamanyo.Substring(tamanyo.LastIndexOf(",") + 1);
                        }

                        foto = foto.Replace(".jpg", "_" + tamanyo + ".jpg");
                    }

                    if (pPropiedad.FunctionalProperty)
                    {
                        pPropiedad.UnicoValor = new KeyValuePair<string, ElementoOntologia>(foto, null);
                    }
                    else
                    {
                        pPropiedad.ListaValores.Add(foto, null);
                    }

                    pintar = true;
                }
                else if (pintar && mSemRecModel.MassiveResourceLoad && ((pPropiedad.EspecifPropiedad.TieneValor_TipoCampo && (pPropiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.Archivo || pPropiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.Imagen || pPropiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.Video || pPropiedad.EspecifPropiedad.TipoCampo == TipoCampoOntologia.ArchivoLink)) || (!EditandoRecursoCargaMasiva && ((mOntologia.ConfiguracionPlantilla.PropiedadTitulo.Key != null && pPropiedad.Nombre == mOntologia.ConfiguracionPlantilla.PropiedadTitulo.Key && (pPropiedad.ElementoOntologia.TipoEntidad == mOntologia.ConfiguracionPlantilla.PropiedadTitulo.Value || pPropiedad.ElementoOntologia.SuperclasesUtiles.Contains(mOntologia.ConfiguracionPlantilla.PropiedadTitulo.Value))) || (mOntologia.ConfiguracionPlantilla.PropiedadDescripcion.Key != null && pPropiedad.Nombre == mOntologia.ConfiguracionPlantilla.PropiedadDescripcion.Key && (pPropiedad.ElementoOntologia.TipoEntidad == mOntologia.ConfiguracionPlantilla.PropiedadDescripcion.Value || pPropiedad.ElementoOntologia.SuperclasesUtiles.Contains(mOntologia.ConfiguracionPlantilla.PropiedadDescripcion.Value)))))))//Si es una carga masiva y la propiedad es de tipo archivo no la pintamos. Además si no es de tipo archivo y no estamos editando un recurso pre-creado de la carga masiva y la propiedad es el título o la descripción del recurso, no lo pintamos
                {
                    pintar = false;
                }
            }
            catch (Exception ex)
            {
                GuardarLogErrorAJAX(ex.ToString());
            }

            if (pintar && mSemRecModel.ReadMode && pPropiedad.EspecifPropiedad.PrivadoPrivadoParaMiembrosComunidad && pIdentidadID.Equals(UsuarioAD.Invitado))
            {
                pintar = false;
            }

            //Si es la ficha del recurso y no se está editanto.
            if (pintar && mSemRecModel.ReadMode && pPropiedad.EspecifPropiedad.PrivadoParaGrupoEditores != null)
            {
                pintar = false;
                IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                //Buscar si la identidad participa en alguno de los grupos
                if (identCN.ParticipaIdentidadEnGrupo(pIdentidadID, pPropiedad.EspecifPropiedad.PrivadoParaGrupoEditores))
                {
                    pintar = true;
                }
                identCN.Dispose();
            }

            if (pintar && !mSemRecModel.ReadMode && PropiedadIdiomaBusquedaComunidad == pPropiedad.Nombre)
            {
                pintar = false;
            }

            if (pintar && mSemRecModel.ReadMode && pSemPropModel.Element.SoloIdiomaNavegacion && !pPropiedad.ListaValoresIdioma.ContainsKey(mUtilIdiomas.LanguageCode))
            {
                pintar = false;
            }

            if (pintar && mSemRecModel.ReadMode && PropiedadIdiomaBusquedaComunidad != null)
            {
                pintar = PropiedadTieneValorEnIdiomaNavegacion(pSemPropModel.Element.Propiedad);
            }

            return pintar;
        }

        /// <summary>
        /// Indica si la propiedad no tiene multiidioma o si lo tiene lo tiene en el idioma de navegación.
        /// </summary>
        /// <param name="pPropiedad">Propiedad</param>
        /// <returns>TRUE si la propiedad no tiene multiidioma o si lo tiene lo tiene en el idioma de navegación, FALSE si no</returns>
        private bool PropiedadTieneValorEnIdiomaNavegacion(Propiedad pPropiedad)
        {
            if (pPropiedad.Tipo == TipoPropiedad.DatatypeProperty)
            {
                if (pPropiedad.ListaValoresIdioma.Count > 0 && !pPropiedad.ListaValoresIdioma.ContainsKey(mUtilIdiomas.LanguageCode))
                {
                    return false;
                }
            }
            //else if (pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad != null && pSemPropModel.Element.Propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "Tesauro")
            else if (pPropiedad.EspecifPropiedad.SelectorEntidad == null && pPropiedad.ValoresUnificados.Count > 0)
            {
                ElementoOntologia primeraEntHija = pPropiedad.ValoresUnificados.Values.ToList()[0];

                if (primeraEntHija.EsEntidadPathTesSemantico)
                {
                    KeyValuePair<string, string> claveEntExt = new KeyValuePair<string, string>(primeraEntHija.PropiedadNodoTesSemantico, primeraEntHija.TipoEntidad);

                    if (mDatosEntidadesExternas.ContainsKey(claveEntExt) && ((FacetadoDS)mDatosEntidadesExternas[claveEntExt][3]) != null &&((FacetadoDS)mDatosEntidadesExternas[claveEntExt][3]).Tables["SelectPropEnt"].Select("p='" + EstiloPlantilla.PrefLabel_TesSem + "' AND (idioma='' OR idioma='" + mUtilIdiomas.LanguageCode + "')").Length == 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Completa el modelo de propiedad para un dato especial.
        /// </summary>
        /// <param name="pElemOrdenado">Elemento ordenado del modelo de propiedad</param>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        /// <returns>TRUE si el especial tiene que agregarse, FALSE si no</returns>
        private bool CompletarModeloElementoOrdenadoEspecial(ElementoOrdenado pElemOrdenado, SemanticPropertyModel pSemPropModel, Guid pIdentidadID)
        {
            if (pSemPropModel.ReadMode && (string)pElemOrdenado.DatosEspecial["TipoEspecial"] == "Faceta")
            {
                string idPanel = "panFac_" + Guid.NewGuid();
                pSemPropModel.EspecialPropInfo = new SemanticPropertyModel.EspecialProp();
                pSemPropModel.EspecialPropInfo.Facet = (string)pElemOrdenado.DatosEspecial["NombreFaceta"];
                pSemPropModel.EspecialPropInfo.Graph = (string)pElemOrdenado.DatosEspecial["Grafo"];
                string parametros = "";

                foreach (string parametro in ((string)pElemOrdenado.DatosEspecial["Parametros"]).Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (parametro.Contains("["))
                    {
                        string parte1 = parametro.Substring(0, parametro.IndexOf("=") + 1);
                        string parte2 = parametro.Substring(parametro.IndexOf("[") + 1);
                        parte2 = parte2.Substring(0, parte2.IndexOf("]"));
                        string[] propEnt = parte2.Split(',');

                        foreach (ElementoOntologia entidad in mEntidades)
                        {
                            Propiedad propiedad = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(propEnt[0], propEnt[1], entidad);
                            if (propiedad != null && propiedad.ValoresUnificados.Count > 0)
                            {
                                parametros += parte1 + new List<string>(propiedad.ValoresUnificados.Keys)[0].ToLower() + "|";
                            }
                        }
                    }
                    else
                    {
                        parametros += parametro + "|";
                    }
                }

                if (parametros.Length > 0)
                {
                    parametros = parametros.Substring(0, parametros.Length - 1);
                }

                pSemPropModel.EspecialPropInfo.Parameters = parametros;

                return true;
            }
            else if (pSemPropModel.ReadMode && (string)pElemOrdenado.DatosEspecial["TipoEspecial"] == "Boton")
            {
                if (!pElemOrdenado.DatosEspecial.ContainsKey("Accion"))
                {
                    return false;
                }

                if (pElemOrdenado.DatosEspecial.ContainsKey("Condicion") && !CumpleRecursoCondicion((string)pElemOrdenado.DatosEspecial["Condicion"], pIdentidadID))
                {
                    return false;
                }

                pSemPropModel.EspecialPropInfo = new SemanticPropertyModel.EspecialProp();
                pSemPropModel.EspecialPropInfo.ActionID = (string)pElemOrdenado.DatosEspecial["Accion"];
                pSemPropModel.EspecialPropInfo.ButtonName = UtilCadenas.ObtenerTextoDeIdioma(pElemOrdenado.NombrePropiedad.Key, mUtilIdiomas.LanguageCode, null);
                pSemPropModel.EspecialPropInfo.ActionEntityID = pSemPropModel.EntityParent.Entity.Uri;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Indica si el recurso cumple una determinada condición declarada.
        /// </summary>
        /// <param name="pCondicion">ID de la condición</param>
        /// <returns>TRUE si el recurso cumple una determinada condición declarada, FALSE si no</returns>
        public bool CumpleRecursoCondicion(string pCondicionID, Guid pIdentidadID)
        {
            if (!Ontologia.ConfiguracionPlantilla.Condiciones.ContainsKey(pCondicionID))
            {
                return false;
            }

            return CumpleRecursoClausulaCondicion(Ontologia.ConfiguracionPlantilla.Condiciones[pCondicionID].Clausula, Ontologia.ConfiguracionPlantilla.Condiciones[pCondicionID], pIdentidadID);
        }

        /// <summary>
        /// Indica si el recurso cumple una determinada clausula de unacondición declarada.
        /// </summary>
        /// <param name="pClausula">Clausula de una condición</param>
        /// <param name="pCondicion">Condición a la que pertenece la clausula</param>
        /// <returns>TRUE si el recurso cumple una determinada clausula de unacondición declarada, FALSE si no</returns>
        private bool CumpleRecursoClausulaCondicion(CondicionSemCms.ClausulaSemCms pClausula, CondicionSemCms pCondicion, Guid pIdentidadID)
        {
            bool condicion = (pClausula.Tipo == "Igual");

            switch (pClausula.Tipo)
            {
                case "Or":
                case "And":
                    if (pClausula.Clausulas.Count == 0)
                    {
                        return false;
                    }

                    foreach (CondicionSemCms.ClausulaSemCms claus in pClausula.Clausulas)
                    {
                        bool cumpleClau = CumpleRecursoClausulaCondicion(claus, pCondicion, pIdentidadID);

                        if (pClausula.Tipo == "Or" && cumpleClau)
                        {
                            return true;
                        }
                        else if (pClausula.Tipo == "And" && !cumpleClau)
                        {
                            return false;
                        }
                    }

                    return (pClausula.Tipo == "And");
                case "Igual":
                case "Distinto":

                    List<string> valores = new List<string>();

                    foreach (string valorConf in pClausula.Valores)
                    {
                        if (valorConf.StartsWith("@"))
                        {
                            Propiedad propClau = null;
                            List<ElementoOntologia> entidadesBuscar = Entidades;

                            foreach (string propEnt in pCondicion.VariablesProp[valorConf].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                propClau = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(propEnt.Split(',')[0], propEnt.Split(',')[1], entidadesBuscar);

                                if (propClau.Tipo == TipoPropiedad.ObjectProperty)
                                {
                                    entidadesBuscar = new List<ElementoOntologia>(propClau.ValoresUnificados.Values);
                                }
                            }

                            if (propClau != null)
                            {
                                string valorProp = "";

                                foreach (string valor in propClau.ValoresUnificados.Keys)
                                {
                                    valorProp += valor + "[&&&]";
                                }

                                if (!string.IsNullOrEmpty(valorProp))
                                {
                                    valorProp = valorProp.Substring(0, valorProp.Length - 5);
                                    valores.Add(valorProp);
                                }
                            }
                        }
                        else
                        {
                            valores.Add(valorConf);
                        }
                    }

                    if (valores.Count != 2)
                    {
                        return !condicion;
                    }

                    if (valores[0].Contains("[&&&]") && !valores[1].Contains("[&&&]"))
                    {
                        List<string> contenedor = new List<string>(valores[0].Split(new string[] { "[&&&]" }, StringSplitOptions.RemoveEmptyEntries));
                        if (contenedor.Contains(valores[1]))
                        {
                            return condicion;
                        }
                    }
                    else if (!valores[0].Contains("[&&&]") && valores[1].Contains("[&&&]"))
                    {
                        List<string> contenedor = new List<string>(valores[1].Split(new string[] { "[&&&]" }, StringSplitOptions.RemoveEmptyEntries));
                        if (contenedor.Contains(valores[0]))
                        {
                            return condicion;
                        }
                    }
                    else if (valores[0].Contains("[&&&]") && valores[1].Contains("[&&&]"))
                    {
                        List<string> contenedor = new List<string>(valores[0].Split(new string[] { "[&&&]" }, StringSplitOptions.RemoveEmptyEntries));

                        foreach (string valor in valores[1].Split(new string[] { "[&&&]" }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (contenedor.Contains(valor))
                            {
                                return condicion;
                            }
                        }
                    }
                    else if (valores[0] == valores[1])
                    {
                        return condicion;
                    }

                    break;
                case "PerteneceAGrupo":
                    if (pClausula.Valores.Count == 0)
                    {
                        return false;
                    }

                    List<Guid> gruposIDS = new List<Guid>();

                    foreach (string valor in pClausula.Valores)
                    {
                        gruposIDS.Add(new Guid(valor));
                    }

                    IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    //Buscar si la identidad participa en alguno de los grupos
                    bool pertenece = identCN.ParticipaIdentidadEnGrupo(pIdentidadID, gruposIDS);
                    identCN.Dispose();

                    return pertenece;
                default:
                    return false;
            }

            return !condicion;
        }

        /// <summary>
        /// Lista de propiedades ordenadas.
        /// </summary>
        /// <param name="pEntModel">Modelo de la entidad</param>
        /// <returns>Lista de propiedades ordenadas</returns>
        private List<ElementoOrdenado> PropiedadesOrdenadas(SemanticEntityModel pEntModel, Guid pIdentidadID)
        {
            List<ElementoOrdenado> propiedadesOrdenadas = new List<ElementoOrdenado>();

            if (pEntModel.SpecificationEntity.ElementosOrdenadosPorCondicion != null)
            {
                foreach (string condicion in pEntModel.SpecificationEntity.ElementosOrdenadosPorCondicion.Keys)
                {
                    if (CumpleRecursoCondicion(condicion, pIdentidadID))
                    {
                        propiedadesOrdenadas.AddRange(pEntModel.SpecificationEntity.ElementosOrdenadosPorCondicion[condicion]);
                        EstiloPlantillaEspecifEntidad.DarValorPropiedadAElementosOrdenados(propiedadesOrdenadas, pEntModel.Entity);
                        break;
                    }
                }
            }

            if (propiedadesOrdenadas.Count == 0)
            {
                if (pEntModel.SpecificationEntity.ElementosOrdenados.Count > 0)
                {
                    EstiloPlantillaEspecifEntidad.DarValorPropiedadAElementosOrdenados(pEntModel.SpecificationEntity.ElementosOrdenados, pEntModel.Entity);
                    propiedadesOrdenadas.AddRange(pEntModel.SpecificationEntity.ElementosOrdenados);
                }
                else
                {
                    foreach (Propiedad propiedad in pEntModel.Entity.Propiedades)
                    {
                        ElementoOrdenado elemSinOrd = new ElementoOrdenado();
                        elemSinOrd.NombrePropiedad = new KeyValuePair<string, Propiedad>(propiedad.Nombre, propiedad);
                        propiedadesOrdenadas.Add(elemSinOrd);
                    }
                }
            }

            return propiedadesOrdenadas;
        }

        /// <summary>
        /// Lista de propiedades ordenadas para la lectura.
        /// </summary>
        /// <param name="pEntModel">Modelo de la entidad</param>
        /// <returns>Lista de propiedades ordenadas</returns>
        private List<ElementoOrdenado> PropiedadesOrdenadasLectura(SemanticEntityModel pEntModel, Guid pIdentidadID)
        {
            List<ElementoOrdenado> propiedadesOrdenadasLectura = new List<ElementoOrdenado>();

            if (pEntModel.SpecificationEntity.ElementosOrdenadosLecturaPorCondicion != null)
            {
                foreach (string condicion in pEntModel.SpecificationEntity.ElementosOrdenadosLecturaPorCondicion.Keys)
                {
                    if (CumpleRecursoCondicion(condicion, pIdentidadID))
                    {
                        propiedadesOrdenadasLectura.AddRange(pEntModel.SpecificationEntity.ElementosOrdenadosLecturaPorCondicion[condicion]);
                        EstiloPlantillaEspecifEntidad.DarValorPropiedadAElementosOrdenados(propiedadesOrdenadasLectura, pEntModel.Entity);
                        break;
                    }
                }
            }

            if (propiedadesOrdenadasLectura.Count == 0)
            {
                if (pEntModel.SpecificationEntity.ElementosOrdenadosLectura.Count > 0)
                {
                    if (pEntModel.SpecificationEntity.ElementosOrdenadosLectura.Count > 0)
                    {
                        EstiloPlantillaEspecifEntidad.DarValorPropiedadAElementosOrdenados(pEntModel.SpecificationEntity.ElementosOrdenadosLectura, pEntModel.Entity);
                        propiedadesOrdenadasLectura.AddRange(pEntModel.SpecificationEntity.ElementosOrdenadosLectura);
                    }
                    else
                    {
                        foreach (Propiedad propiedad in pEntModel.Entity.Propiedades)
                        {
                            ElementoOrdenado elemSinOrd = new ElementoOrdenado();
                            elemSinOrd.NombrePropiedad = new KeyValuePair<string, Propiedad>(propiedad.Nombre, propiedad);
                            propiedadesOrdenadasLectura.Add(elemSinOrd);
                        }
                    }
                }
                else
                {
                    propiedadesOrdenadasLectura = PropiedadesOrdenadas(pEntModel, pIdentidadID);
                }
            }

            return propiedadesOrdenadasLectura;
        }

        /// <summary>
        /// Completa el modelo de propiedad para un selector de grupo.
        /// </summary>
        /// <param name="pElemOrdenado">Elemento ordenado del modelo de propiedad</param>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        /// <returns>TRUE si el selector de grupo tiene que agregarse, FALSE si no</returns>
        private bool CompletarModeloElementoOrdenadoSelectorGrupo(ElementoOrdenado pElemOrdenado, SemanticPropertyModel pSemPropModel)
        {
            int count = 0;

            foreach (string idOpcion in pElemOrdenado.OpcionesSelectorGrupo.Keys)
            {
                if (count > 0)
                {
                    IDGruposOcultos.Add(idOpcion);
                }

                count++;
            }

            return true;
        }

        /// <summary>
        /// Completa el modelo de propiedad para un literal.
        /// </summary>
        /// <param name="pElemOrdenado">Elemento ordenado del modelo de propiedad</param>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        /// <returns>TRUE si el literal tiene que agregarse SI o SI porque es importante, FALSE si no</returns>
        private bool CompletarModeloElementoOrdenadoLiteral(ElementoOrdenado pElemOrdenado, SemanticPropertyModel pSemPropModel)
        {
            pSemPropModel.LiteralInfo = new SemanticPropertyModel.Literal();

            if (pElemOrdenado.Link != null)
            {
                string urlLink = pElemOrdenado.Link;

                if (urlLink.IndexOf("@") == 0)
                {
                    Propiedad propLink = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(urlLink.Substring(1), mEntidades[0]);

                    if (propLink != null)
                    {
                        urlLink = propLink.PrimerValorPropiedad;
                    }
                    else
                    {
                        urlLink = "";
                    }
                }

                if (!string.IsNullOrEmpty(urlLink))
                {
                    if (!urlLink.StartsWith("http://") && !urlLink.StartsWith("https://"))
                    {
                        urlLink = "http://" + urlLink;
                    }

                    pSemPropModel.LiteralInfo.LiteralLink = urlLink;
                }
            }

            pSemPropModel.LiteralInfo.LiteralName = UtilCadenas.ObtenerTextoDeIdioma(pElemOrdenado.NombrePropiedad.Key, Ontologia.IdiomaUsuario, null);

            return pElemOrdenado.LiteralImportante;
        }

        /// <summary>
        /// Completa el modelo de propiedad para un grupo.
        /// </summary>
        /// <param name="pElemOrdenado">Elemento ordenado del modelo de propiedad</param>
        /// <param name="pSemPropModel">Modelo de propiedad para un grupo</param>
        /// <returns>TRUE si el grupo tiene contenido dentro, FALSE si no</returns>
        private bool CompletarModeloElementoOrdenadoGrupo(ElementoOrdenado pElemOrdenado, SemanticPropertyModel pSemPropModel, Guid pIdentidadID)
        {
            pSemPropModel.GroupInfo = new SemanticPropertyModel.Group();

            if (pElemOrdenado.IdGrupoLectura != null && IDGruposOcultos.Contains(pElemOrdenado.IdGrupoLectura))
            {
                pSemPropModel.Hidden = true;
            }

            if (pSemPropModel.ReadMode && pElemOrdenado.TipoGrupo != null && pElemOrdenado.TipoGrupo == "titulo" && mOntologia.ConfiguracionPlantilla.IncluirIconoGnoss && mOntologia.ConfiguracionPlantilla.OcultarTituloDescpEImg && mDocumento != null)
            {
                pSemPropModel.GroupInfo.IncludeGnossIcon = true;
                pSemPropModel.GroupInfo.GnossIconIsPrivate = (mDocumento.FilaDocumentoWebVinBR != null && mDocumento.FilaDocumentoWebVinBR.PrivadoEditores || (!mDocumento.FilaDocumento.Publico && mProyectoActual.Clave.Equals(ProyectoAD.MetaProyecto)));
                pSemPropModel.GroupInfo.GnossIconName = new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication).ObtenerNombreIconoDocumentoSem(mDocumento);
            }

            int profundidadHijos = pSemPropModel.Depth;

            if (pElemOrdenado.NombrePropiedad.Key != null)
            {
                string[] nombreTitulo = pElemOrdenado.NombrePropiedad.Key.Split(new string[] { "|||" }, StringSplitOptions.None);

                if (!string.IsNullOrEmpty(nombreTitulo[2]))
                {
                    pSemPropModel.GroupInfo.GroupNameTag = nombreTitulo[2];
                }

                profundidadHijos++;

                pSemPropModel.GroupInfo.GroupName = ObtenerNobreGrupoElemOrdenado(nombreTitulo[0]);

                if (!string.IsNullOrEmpty(nombreTitulo[1]))
                {
                    pSemPropModel.GroupInfo.GroupNameClass = nombreTitulo[1];
                }
            }

            string tipoCampoLetura = null;
            if (pElemOrdenado.TipoGrupo != null && (pElemOrdenado.TipoGrupo == "subtitulo" || pElemOrdenado.TipoGrupo == "titulo"))
            {
                tipoCampoLetura = "span";
            }

            pSemPropModel.Properties = new List<SemanticPropertyModel>();

            foreach (ElementoOrdenado elemOrdHijo in pElemOrdenado.Hijos)
            {
                if (tipoCampoLetura != null && elemOrdHijo.Propiedad != null)
                {
                    elemOrdHijo.Propiedad.ElementoOntologia = pSemPropModel.EntityParent.Entity;
                    elemOrdHijo.Propiedad.EspecifPropiedad.TipoCampoLectura = tipoCampoLetura;
                }

                if (tipoCampoLetura == null && elemOrdHijo.Propiedad != null && pElemOrdenado.TipoGrupo != null && pElemOrdenado.TipoGrupo.ToLower() != "dl")
                {
                    elemOrdHijo.Propiedad.ElementoOntologia = pSemPropModel.EntityParent.Entity;

                    if (elemOrdHijo.Propiedad.EspecifPropiedad.TipoCampoLectura == null)
                    {
                        elemOrdHijo.Propiedad.EspecifPropiedad.TipoCampoLectura = "span";
                    }
                }

                SemanticPropertyModel elemHijoModel = OntenerModeloElementoOrdenado(elemOrdHijo, pSemPropModel.EntityParent, profundidadHijos, pIdentidadID);

                if (elemHijoModel != null)
                {
                    pSemPropModel.Properties.Add(elemHijoModel);
                }
            }

            return (pSemPropModel.Properties.Count > 0);
        }

        /// <summary>
        /// Obtiene el nombre de un grupo elemento ordenado según el idioma del usuario.
        /// </summary>
        /// <param name="pNombreIdiomaGrupo">Nombre en varios idiomas de los grupos</param>
        /// <returns>Nombre de un grupo elemento ordenado según el idioma del usuario</returns>
        private string ObtenerNobreGrupoElemOrdenado(string pNombreIdiomaGrupo)
        {
            if (pNombreIdiomaGrupo.Contains("@" + Ontologia.IdiomaUsuario + "[||]"))
            {
                string texto = pNombreIdiomaGrupo.Substring(0, pNombreIdiomaGrupo.IndexOf("@" + Ontologia.IdiomaUsuario + "[||]"));
                if (texto.Contains("[||]"))
                {
                    texto = texto.Substring(texto.LastIndexOf("[||]") + 4);
                }

                return texto;
            }
            else
            {
                string texto = pNombreIdiomaGrupo.Substring(0, pNombreIdiomaGrupo.IndexOf("[||]"));

                if (texto.Contains("@"))
                {
                    texto = texto.Substring(0, texto.LastIndexOf("@"));
                }

                return texto;
            }
        }

        private void CargarIdiomasModel()
        {   
            if (mOntologia.ConfiguracionPlantilla != null && mOntologia.ConfiguracionPlantilla.MultiIdioma && !(ParametrosGeneralesRow.IdiomaDefecto == null) && !string.IsNullOrEmpty(ParametrosGeneralesRow.IdiomaDefecto))
            {//Es multiidioma:
                IdiomaDefecto = ParametrosGeneralesRow.IdiomaDefecto;
                IdiomasDisponibles = mConfigService.ObtenerListaIdiomasDictionary();
            }

            if (IdiomaDefecto != null)
            {
                mSemRecModel.DefaultLanguage = IdiomaDefecto;
                mSemRecModel.AvailableLanguages = IdiomasDisponibles;
            }
        }

        /// <summary>
        /// Obtiene los valores de las entidades grafo dependientes.
        /// </summary>
        private void ObtenerValoresDeEntGrafoDependientes()
        {
            try
            {
                if (!ConfigGenXml.HayValoresGrafoDependienets)
                {
                    return;
                }

                mLoggingService.AgregarEntrada("FormSem Inicio GeneradorPantillaOWL.ObtenerValoresDeEntGrafoDependientes()");

                if (!mSemRecModel.ReadMode)
                {
                    mSemRecModel.AuxiliaryDependentGraphValuesInfo = "";
                }

                mDatosEntidadesGrafoDep = new Dictionary<string, Dictionary<string, string>>();

                //Preparo que datos hay que cargar:
                foreach (ElementoOntologia elemPrinc in mEntidades)
                {
                    ObtenerPropiedadesGrafoDependientesDeEntidad(elemPrinc);
                }

                FacetadoCN facetadoCN = new FacetadoCN(mUrlIntragnoss, true, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

                foreach (string grafo in mDatosEntidadesGrafoDep.Keys)
                {
                    FacetadoDS faceDS = facetadoCN.ObtenerValoresGrafoDependientesDeEntidades(grafo, new List<string>(mDatosEntidadesGrafoDep[grafo].Keys));

                    foreach (DataRow fila in faceDS.Tables[0].Rows)
                    {
                        mDatosEntidadesGrafoDep[grafo][(string)fila[0]] = (string)fila[1];

                        if (mSemRecModel.AuxiliaryDependentGraphValuesInfo != null)
                        {
                            mSemRecModel.AuxiliaryDependentGraphValuesInfo += (string)fila[0] + "," + (string)fila[1] + "|";
                        }
                    }
                }

                facetadoCN.Dispose();

                mLoggingService.AgregarEntrada("FormSem Fin GeneradorPantillaOWL.ObtenerValoresDeEntGrafoDependientes()");
            }
            catch (Exception ex)
            {
                GuardarMensajeErrorAdmin("Error al obtener los valores de propiedades configuradas como dependientes de otras y autocompletadas de un grafo.", ex);
                GuardarLogErrorAJAX("Error docSem '" + mDocumentoID + "' al obtener valores grafodependientes" + Environment.NewLine + ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Obtiene las propiedades que son selección de entidades externas.
        /// </summary>
        /// <param name="pEntidad">Entidad sobre la que se busca</param>
        private void ObtenerPropiedadesGrafoDependientesDeEntidad(ElementoOntologia pEntidad)
        {
            foreach (Propiedad propiedad in pEntidad.Propiedades)
            {
                propiedad.ElementoOntologia = pEntidad;

                if (propiedad.Tipo == TipoPropiedad.ObjectProperty)
                {
                    if (propiedad.EspecifPropiedad.SelectorEntidad == null)
                    {
                        foreach (ElementoOntologia entidad in propiedad.ValoresUnificados.Values)
                        {
                            ObtenerPropiedadesGrafoDependientesDeEntidad(entidad);
                        }
                    }
                }
                else
                {
                    if (propiedad.EspecifPropiedad.GrafoDependiente != null && propiedad.ValorUnico && propiedad.ValoresUnificados.Count > 0)
                    {
                        if (!mDatosEntidadesGrafoDep.ContainsKey(propiedad.EspecifPropiedad.GrafoDependiente))
                        {
                            mDatosEntidadesGrafoDep.Add(propiedad.EspecifPropiedad.GrafoDependiente, new Dictionary<string, string>());
                        }

                        string valor = new List<string>(propiedad.ValoresUnificados.Keys)[0];
                        mDatosEntidadesGrafoDep[propiedad.EspecifPropiedad.GrafoDependiente].Add(valor, null);

                        mLoggingService.AgregarEntrada("FormSem Agregado ID entidad grafodependiente '" + propiedad.EspecifPropiedad.GrafoDependiente + "' con ID '" + valor + "'");

                    }

                    propiedad.ElementoOntologia = null;
                    propiedad.EspecifPropiedad = null;
                }
            }
        }

        #region Log

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        /// <param name="pError">Cadena de texto con el error</param>
        private void GuardarLogErrorAJAX(string pError)
        {
            mLoggingService.GuardarLogError(pError);
        }

        /// <summary>
        /// Guarda el mensaje de error para el administrador.
        /// </summary>
        /// <param name="pMensaje">Mensaje</param>
        /// <param name="ex">Excepción</param>
        public void GuardarMensajeErrorAdmin(string pMensaje, Exception ex)
        {
            if (string.IsNullOrEmpty(mSemRecModel.AdminGenerationError))
            {
                mSemRecModel.AdminGenerationError = pMensaje;

                if (ex != null)
                {
                    mSemRecModel.AdminGenerationError += Environment.NewLine + ex.Message;
                }
            }
        }

        #endregion

        #endregion

        #region Ayuda generación SEMCMS ReadMode

        /// <summary>
        /// Obtiene el controlador del SEMCMS.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pProyecto">Proyecto actual</param>
        /// <param name="pIdentidadActual">Identidad actual conectada</param>
        /// <param name="pBaseURLFormulariosSem">Base Url para formularios semánticos</param>
        /// <param name="pUtilIdiomas">Util idiomas</param>
        /// <param name="pBaseURL">Base Url</param>
        /// <param name="pBaseURLIdioma">Base Url con Idioma</param>
        /// <param name="pBaseURLContent">Url del Content</param>
        /// <param name="pBaseURLStatic">Url Static</param>
        /// <param name="pUrlIntragnoss">Url de intragnoss</param>
        /// <param name="pVistaPersonalizada">Indica si la vista que se va usar para el controlador de documento es personalizada</param>
        /// <param name="pParametroProyecto">Parámetros del proyecto</param>
        /// <param name="pParamSemCms">Parámetro especial para el sem cms</param>
        /// <returns>Controlador del SEMCMS</returns>
        public SemCmsController ObtenerControladorSemCMS(Documento pDocumento, Proyecto pProyecto, Identidad pIdentidadActual, string pBaseURLFormulariosSem, UtilIdiomas pUtilIdiomas, string pBaseURL, string pBaseURLIdioma, string pBaseURLContent, string pBaseURLStatic, string pUrlIntragnoss, bool pVistaPersonalizada, Dictionary<string, string> pParametroProyecto, string pParamSemCms, bool pUsarAfinidad = false)
        {
            return ObtenerControladorSemCMSInt(pDocumento, pProyecto, pIdentidadActual, pBaseURLFormulariosSem, pUtilIdiomas, pBaseURL, pBaseURLIdioma, pBaseURLContent, pBaseURLStatic, pUrlIntragnoss, pVistaPersonalizada, pParametroProyecto, null, null, 0, pParamSemCms, pUsarAfinidad);
        }

        /// <summary>
        /// Obtiene el controlador del SEMCMS.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pProyecto">Proyecto actual</param>
        /// <param name="pIdentidadActual">Identidad actual conectada</param>
        /// <param name="pBaseURLFormulariosSem">Base Url para formularios semánticos</param>
        /// <param name="pUtilIdiomas">Util idiomas</param>
        /// <param name="pBaseURL">Base Url</param>
        /// <param name="pBaseURLIdioma">Base Url con Idioma</param>
        /// <param name="pBaseURLContent">Url del Content</param>
        /// <param name="pBaseURLStatic">Url Static</param>
        /// <param name="pUrlIntragnoss">Url de intragnoss</param>
        /// <param name="pVistaPersonalizada">Indica si la vista que se va usar para el controlador de documento es personalizada</param>
        /// <param name="pParametroProyecto">Parámetros del proyecto</param>
        /// <param name="pEntidadPag">Entidad que contiene la propiedad en el caso de que solo quiera obtenerse una propiedad para la paginación</param>
        /// <param name="pPropiedadPag">Nombre de la propiedad en el caso de que solo quiera obtenerse una propiedad para la paginación</param>
        /// <param name="pInicioPag">Inicio de la paginación</param>
        /// <param name="pParamSemCms">Parámetro especial para el sem cms</param>
        /// <returns>Controlador del SEMCMS</returns>
        private SemCmsController ObtenerControladorSemCMSInt(Documento pDocumento, Proyecto pProyecto, Identidad pIdentidadActual, string pBaseURLFormulariosSem, UtilIdiomas pUtilIdiomas, string pBaseURL, string pBaseURLIdioma, string pBaseURLContent, string pBaseURLStatic, string pUrlIntragnoss, bool pVistaPersonalizada, Dictionary<string, string> pParametroProyecto, string pEntidadPag, string pPropiedadPag, int pInicioPag, string pParamSemCms, bool pUsarAfinidad = false)
        {
            SemanticResourceModel semRecModel = new SemanticResourceModel();

            Guid ontologiaID = pDocumento.ElementoVinculadoID;

            string nombreOntologia;
            Ontologia ontologia;
            string rdfTexto;

            List<ElementoOntologia> instanciasPrincipales = ObtenerEntidadesPrincipalesRecursoDeBD(pDocumento, ontologiaID, pBaseURLFormulariosSem, pUrlIntragnoss, pUtilIdiomas, pProyecto, out nombreOntologia, out ontologia, out rdfTexto, pParamSemCms, pUsarAfinidad);

            if (pDocumento.GestorDocumental.ListaDocumentos[ontologiaID].FilaDocumento.VersionFotoDocumento.HasValue && pDocumento.GestorDocumental.ListaDocumentos[ontologiaID].FilaDocumento.VersionFotoDocumento > 0)
            {
                semRecModel.OntologyCSS = pBaseURLContent + "/" + UtilArchivos.ContentOntologias + "/Archivos/" + ontologiaID.ToString().Substring(0, 3) + "/" + ontologiaID + ".css?v=" + pDocumento.GestorDocumental.ListaDocumentos[ontologiaID].FilaDocumento.VersionFotoDocumento;
                semRecModel.OntologyJS = pBaseURLContent + "/" + UtilArchivos.ContentOntologias + "/Archivos/" + ontologiaID.ToString().Substring(0, 3) + "/" + ontologiaID + ".js?v=" + pDocumento.GestorDocumental.ListaDocumentos[ontologiaID].FilaDocumento.VersionFotoDocumento;
            }

            SemCmsController semController = new SemCmsController(semRecModel, ontologia, pDocumento, instanciasPrincipales, pProyecto, pIdentidadActual, pUtilIdiomas, pBaseURL, pBaseURLIdioma, pBaseURLContent, pBaseURLStatic, pUrlIntragnoss, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mEntityContextBASE, mServicesUtilVirtuosoAndReplication);
            semController.VistaPersonalizada = pVistaPersonalizada;
            semController.ResourceRDF = rdfTexto;

            if (!string.IsNullOrEmpty(pEntidadPag))
            {
                semController.PropiedadCallbackTraerMas = new KeyValuePair<string, KeyValuePair<string, int>>(pPropiedadPag, new KeyValuePair<string, int>(pEntidadPag, pInicioPag));
            }

            if (pParametroProyecto != null && pParametroProyecto.ContainsKey(ParametroAD.PropiedadContenidoMultiIdioma))
            {
                semController.PropiedadIdiomaBusquedaComunidad = pParametroProyecto[ParametroAD.PropiedadContenidoMultiIdioma];
            }

            try
            {
                semController.ObtenerModeloSemCMSLectura(pIdentidadActual.Clave, pUsarAfinidad);
            }
            catch (Exception)
            {
                if (string.IsNullOrEmpty(semRecModel.AdminGenerationError))
                {
                    throw;
                }
            }
            return semController;
        }

        /// <summary>
        /// Obtiene las entidades principales de un recurso.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pProyecto">Proyecto actual</param>
        /// <param name="pBaseURLFormulariosSem">Base Url para formularios semánticos</param>
        /// <param name="pUtilIdiomas">Util idiomas</param>
        /// <param name="pUrlIntragnoss">Url de intragnoss</param>
        /// <param name="pOntologiaID">ID de la ontología del recurso</param>
        /// <param name="pNombreOntologia">Nombre la ontogía del recurso</param>
        /// <param name="pOntologia">Ontología del recurso</param>
        /// <param name="pRdfTexto">RDF semántico que se obtendrá</param>
        /// <param name="pParamSemCms">Parámetro especial para el sem cms</param>
        /// <returns></returns>
        public List<ElementoOntologia> ObtenerEntidadesPrincipalesRecursoDeBD(Documento pDocumento, Guid pOntologiaID, string pBaseURLFormulariosSem, string pUrlIntragnoss, UtilIdiomas pUtilIdiomas, Proyecto pProyecto, out string pNombreOntologia, out Ontologia pOntologia, out string pRdfTexto, string pParamSemCms, bool pUsarAfinidad = false)
        {
            ControladorDocumentacion controladorDocumentacion = new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);

            if (!pDocumento.GestorDocumental.ListaDocumentos.ContainsKey(pOntologiaID))
            {
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                pDocumento.GestorDocumental.DataWrapperDocumentacion.Merge(docCN.ObtenerDocumentoPorID(pOntologiaID));
                docCN.Dispose();

                pDocumento.GestorDocumental.CargarDocumentos(false);
            }

            //Agrego namespaces y urls:
            string nombreOntologia = pDocumento.GestorDocumental.ListaDocumentos[pOntologiaID].FilaDocumento.Enlace;
            string urlOntologia = pBaseURLFormulariosSem + "/Ontologia/" + nombreOntologia + "#";

            //GestionOWL gestorOWL = new GestionOWL();
            //gestorOWL.UrlOntologia = urlOntologia;
            //gestorOWL.NamespaceOntologia = GestionOWL.NAMESPACE_ONTO_GNOSS;
            string namespaceOnto = GestionOWL.NAMESPACE_ONTO_GNOSS;

            //Obtengo la ontología y su archivo de configuración:
            Dictionary<string, List<EstiloPlantilla>> listaEstilos = new Dictionary<string, List<EstiloPlantilla>>();
            byte[] arrayOntologia = null;

            Ontologia ontologia = null;
            string rdfTexto = null;
            bool rdfObtenido = false;

            Dictionary<string, string> listaPropiedades = UtilCadenas.ObtenerPropiedadesDeTexto(pDocumento.GestorDocumental.ListaDocumentos[pOntologiaID].FilaDocumento.NombreElementoVinculado);
            Guid xmlID = controladorDocumentacion.ObtenerIDXmlOntologia(pOntologiaID, pProyecto.Clave, null);

            string urlEntPrincipal = null;
            string claveOnto = null;

            try
            {
                if (listaPropiedades.ContainsKey(PropiedadesOntologia.xmlTroceado.ToString()) && listaPropiedades[PropiedadesOntologia.xmlTroceado.ToString()].ToLower() == "true")
                {
                    bool obtenidoVirtuoso = false;
                    rdfTexto = ObtenerRDFDocumento(pDocumento.Clave, pDocumento.FilaDocumento.ProyectoID.Value, namespaceOnto, urlOntologia, nombreOntologia, ontologia, pUrlIntragnoss, false, out obtenidoVirtuoso, pUsarAfinidad);
                    rdfObtenido = !obtenidoVirtuoso;
                    urlEntPrincipal = ObtenerTipoEntidadPrincipalRDF(rdfTexto);
                    claveOnto = pOntologiaID + urlEntPrincipal;

                    if (!EstiloPlantilla.OntologiasTrozosCargadas.ContainsKey(claveOnto) || EstiloPlantilla.OntologiasTrozosCargadas[claveOnto].Key != xmlID)
                    {
                        if (EstiloPlantilla.OntologiasTrozosCargadas.ContainsKey(claveOnto))
                        {
                            EstiloPlantilla.OntologiasTrozosCargadas.Remove(claveOnto);
                        }

                        arrayOntologia = controladorDocumentacion.ObtenerOntologiaFraccionada(pOntologiaID, nombreOntologia, urlEntPrincipal, out listaEstilos, pProyecto.Clave, null);

                        //Leo la ontología:
                        ontologia = new Ontologia(arrayOntologia, true);
                        ontologia.LeerOntologia();
                        ontologia.EstilosPlantilla = listaEstilos;
                        ontologia.IdiomaUsuario = pUtilIdiomas.LanguageCode;
                        ontologia.OntologiaID = pOntologiaID;

                        try
                        {
                            EstiloPlantilla.OntologiasTrozosCargadas.Add(claveOnto, new KeyValuePair<Guid, Ontologia>(xmlID, ontologia));
                        }
                        catch (Exception) { }
                    }
                    else
                    {
                        controladorDocumentacion.CargarEstilosOntologiaFraccionadoSegunEntidad(pOntologiaID, nombreOntologia, nombreOntologia.Replace(".", "") + ":" + urlEntPrincipal, out listaEstilos, pProyecto.Clave, null);
                    }
                }
                else
                {
                    if (!EstiloPlantilla.OntologiasCargadas.ContainsKey(pOntologiaID) || EstiloPlantilla.OntologiasCargadas[pOntologiaID].Key != xmlID)
                    {
                        if (EstiloPlantilla.OntologiasCargadas.ContainsKey(pOntologiaID))
                        {
                            EstiloPlantilla.OntologiasCargadas.Remove(pOntologiaID);
                        }

                        arrayOntologia = controladorDocumentacion.ObtenerOntologia(pOntologiaID, out listaEstilos, pProyecto.Clave, null, pParamSemCms);

                        //Leo la ontología:
                        ontologia = new Ontologia(arrayOntologia, true);
                        ontologia.LeerOntologia();
                        ontologia.EstilosPlantilla = listaEstilos;
                        ontologia.IdiomaUsuario = pUtilIdiomas.LanguageCode;
                        ontologia.OntologiaID = pOntologiaID;

                        try
                        {
                            EstiloPlantilla.OntologiasCargadas.Add(pOntologiaID, new KeyValuePair<Guid, Ontologia>(xmlID, ontologia));
                        }
                        catch (Exception) { }
                    }
                    else
                    {
                        controladorDocumentacion.CargarEstilosOntologia(pOntologiaID, out listaEstilos, pProyecto.Clave, null, pParamSemCms);
                    }
                }

                if (urlEntPrincipal == null)
                {
                    ontologia = (Ontologia)EstiloPlantilla.OntologiasCargadas[pOntologiaID].Value.Clone();
                }
                else
                {
                    ontologia = (Ontologia)EstiloPlantilla.OntologiasTrozosCargadas[claveOnto].Value.Clone();
                }

                ontologia.IdiomaUsuario = pUtilIdiomas.LanguageCode;
                ontologia.EstilosPlantilla = listaEstilos;
            }
            catch (Exception ex)
            {
                if (urlEntPrincipal == null)
                {
                    throw new Exception("La ontología " + nombreOntologia + " con ID " + pOntologiaID + " o su archivo de configuración no son correctos:" + Environment.NewLine + ex.ToString());
                }
                else
                {
                    throw new Exception("La ontología troceada " + nombreOntologia + " con ID " + pOntologiaID + " y trozo '" + urlEntPrincipal + "' o su archivo de configuración no son correctos:" + Environment.NewLine + ex.ToString());
                }
            }

            if (((EstiloPlantillaConfigGen)listaEstilos["[" + LectorXmlConfig.NodoConfigGen + "]"][0]).Namespace != null)
            {
                namespaceOnto = ((EstiloPlantillaConfigGen)listaEstilos["[" + LectorXmlConfig.NodoConfigGen + "]"][0]).Namespace;
            }

            //Obtengo el identificador del documento semántico asociado al CV:
            //Guid documentoID = pDocumento.Clave;

            if (!rdfObtenido)
            {
                rdfTexto = ObtenerRDFDocumento(pDocumento.Clave, pDocumento.FilaDocumento.ProyectoID.Value, namespaceOnto, urlOntologia, pDocumento.GestorDocumental.ListaDocumentos[pOntologiaID].Enlace, ontologia, pUrlIntragnoss, false, out rdfObtenido, pUsarAfinidad);
            }

            List<ElementoOntologia> instanciasPrincipales = ObtenerInstanciasPrincipalesDocumento(pDocumento.Clave, pDocumento.FilaDocumento.ProyectoID.Value, namespaceOnto, urlOntologia, pDocumento.GestorDocumental.ListaDocumentos[pOntologiaID].Enlace, ontologia, pUrlIntragnoss, rdfTexto, false);

            if (rdfObtenido)
            {
                //string ruta = GuardarRdfEnFicheroTemporal(urlOntologia, namespaceOnto, ontologia, instanciasPrincipales);
                Stream streamRDF = ObtenerRDF(urlOntologia, namespaceOnto, ontologia, instanciasPrincipales);
                rdfTexto = new StreamReader(streamRDF).ReadToEnd();

                controladorDocumentacion.GuardarRDFEnBDRDF(rdfTexto, pDocumento.Clave, pDocumento.FilaDocumento.ProyectoID.Value, pDocumento.GestorDocumental.RdfDS);
            }

            pNombreOntologia = nombreOntologia;
            pOntologia = ontologia;
            pRdfTexto = rdfTexto;

            return instanciasPrincipales;
        }

        ///// <summary>
        ///// Guarda el RDF del recurso en un fichero temporal.
        ///// </summary>
        ///// <returns>Ruta del fichero temporal</returns>
        //private static string GuardarRdfEnFicheroTemporal(string mUrlOntologia, string mNamespaceOntologia, Ontologia mOntologia, List<ElementoOntologia> mEntidadesGuardar)
        //{
        //    string nombreTemporal = Path.GetRandomFileName() + ".rdf";
        //    string ruta = Path.GetTempPath() + nombreTemporal;

        //    GestionOWL gestionOWL = new GestionOWL();
        //    gestionOWL.UrlOntologia = mUrlOntologia;
        //    gestionOWL.NamespaceOntologia = mNamespaceOntologia;
        //    gestionOWL.PasarOWL(ruta, mOntologia, mEntidadesGuardar, null, null);

        //    return ruta;
        //}

        /// <summary>
        /// Obtener el RDF del recurso
        /// </summary>
        /// <returns>Stream del fichero temporal</returns>
        private static Stream ObtenerRDF(string mUrlOntologia, string mNamespaceOntologia, Ontologia mOntologia, List<ElementoOntologia> mEntidadesGuardar)
        {
            GestionOWL gestionOWL = new GestionOWL();
            gestionOWL.UrlOntologia = mUrlOntologia;
            gestionOWL.NamespaceOntologia = mNamespaceOntologia;
            Stream stream = gestionOWL.PasarOWL(null, mOntologia, mEntidadesGuardar, null, null);
            stream.Position = 0;
            return stream;
        }

        //private byte[] StreamToByte(Stream input)
        //{
        //    byte[] buffer = new byte[16 * 1024];
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        int read;
        //        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
        //        {
        //            ms.Write(buffer, 0, read);
        //        }
        //        return ms.ToArray();
        //    }
        //}

        /// <summary>
        /// Obtiene el tipo de la entidad principal de un RDF en texto.
        /// </summary>
        /// <param name="pRdf">RDF</param>
        /// <returns>URL de la entidad principal de un RDF en texto</returns>
        public static string ObtenerTipoEntidadPrincipalRDF(string pRdf)
        {
            string urlEntPrincipal = ObtenerUrlEntidadPrincipalRDF(pRdf);

            if (string.IsNullOrEmpty(urlEntPrincipal))
            {
                throw new Exception("El rdf no tiene entidad principal: " + pRdf + ".");
            }

            urlEntPrincipal = urlEntPrincipal.Substring(urlEntPrincipal.LastIndexOf("/") + 1);
            urlEntPrincipal = urlEntPrincipal.Substring(0, urlEntPrincipal.LastIndexOf("_"));
            urlEntPrincipal = urlEntPrincipal.Substring(0, urlEntPrincipal.LastIndexOf("_"));

            return urlEntPrincipal;
        }

        /// <summary>
        /// Obtiene la URL de la entidad principal de un RDF en texto.
        /// </summary>
        /// <param name="pRdf">RDF</param>
        /// <returns>URL de la entidad principal de un RDF en texto</returns>
        public static string ObtenerUrlEntidadPrincipalRDF(string pRdf)
        {
            string entPrincID = null;
            XmlDocument docXml = new XmlDocument();
            docXml.Load(new StringReader(pRdf));

            XmlNamespaceManager ns = new XmlNamespaceManager(docXml.NameTable);
            ns.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");

            XmlNodeList nodos = docXml.SelectNodes("//rdf:Description", ns);
            string primeraEntidad = null;

            if (nodos.Count > 0)
            {
                foreach (XmlNode nodo in nodos)
                {
                    if (nodo.Attributes["rdf:about"] != null)
                    {
                        string entID = nodo.Attributes["rdf:about"].Value;

                        if (pRdf.IndexOf(entID) == pRdf.LastIndexOf(entID))
                        {
                            entPrincID = entID;
                            break;
                        }
                        else if (primeraEntidad == null)
                        {
                            primeraEntidad = entID;
                        }
                    }
                }
            }
            else
            {
                string rdf = pRdf;
                while (rdf.Contains("rdf:about=\""))
                {
                    string entID = rdf.Substring(rdf.IndexOf("rdf:about=\"") + 11);
                    rdf = entID.Substring(entID.IndexOf("\"") + 1);
                    entID = entID.Substring(0, entID.IndexOf("\""));

                    if (pRdf.IndexOf(entID) == pRdf.LastIndexOf(entID))
                    {
                        entPrincID = entID;
                        break;
                    }
                    else if (primeraEntidad == null)
                    {
                        primeraEntidad = entID;
                    }
                }
            }


            if (string.IsNullOrEmpty(entPrincID) && !string.IsNullOrEmpty(primeraEntidad))
            {
                entPrincID = primeraEntidad;
            }

            docXml = null;
            return entPrincID;
        }

        /// <summary>
        /// Obtiene el RDF de un docuemnto.
        /// </summary>
        /// <param name="pDocumentoID">ID del documento</param>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pNamespaceOnto">Namespace de la ontología</param>
        /// <param name="pUrlOnto">Url de la ontología</param>
        /// <param name="pEnlaceOnto">Enlace de la ontología</param>
        /// <param name="pOntologia">Ontología</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        /// <param name="pEdicion">Indica si estamos editando el recurso o no</param>
        /// <returns>RDF de un docuemnto</returns>
        public string ObtenerRDFDocumento(Guid pDocumentoID, Guid pProyectoID, string pNamespaceOnto, string pUrlOnto, string pEnlaceOnto, Ontologia pOntologia, string pUrlIntragnoss, bool pEdicion, out bool pObtenidoDeVirtuoso, bool pUsarAfinidad = false)
        {
            ControladorDocumentacion controladorDocumentacion = new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
            //Leo las entidades pricipales del documento RDF:
            string rdfTexto = controladorDocumentacion.ObtenerTextoRDFDeBDRDF(pDocumentoID, pProyectoID, pNamespaceOnto);
            pObtenidoDeVirtuoso = false;

            if (pEdicion && !string.IsNullOrEmpty(rdfTexto))
            {
                if (!rdfTexto.Contains(pNamespaceOnto + ":") && !rdfTexto.Contains("xmlns:" + pNamespaceOnto + "="))
                {
                    try
                    {
                        string namespaceGuardado = rdfTexto.Substring(0, rdfTexto.IndexOf("=\"" + pUrlOnto + "\""));
                        namespaceGuardado = namespaceGuardado.Substring(namespaceGuardado.LastIndexOf("xmlns:") + 6);
                        rdfTexto = rdfTexto.Replace("xmlns:" + namespaceGuardado + "=", "xmlns:" + pNamespaceOnto + "=");
                        rdfTexto = rdfTexto.Replace("<" + namespaceGuardado + ":", "<" + pNamespaceOnto + ":").Replace("</" + namespaceGuardado + ":", "</" + pNamespaceOnto + ":");
                    }
                    catch (Exception)
                    {//Que vaya a virtuoso
                        rdfTexto = null;
                    }
                }
            }

            if (string.IsNullOrEmpty(rdfTexto))
            {
                pObtenidoDeVirtuoso = true;
                mLoggingService.AgregarEntrada("FormSem RDF NO está en sqlServer, vamos a virtuoso a por él");

                byte[] rdf = controladorDocumentacion.ObtenerRDFDeVirtuoso(pDocumentoID, pEnlaceOnto, pUrlIntragnoss, pUrlOnto, pNamespaceOnto, pOntologia, pUsarAfinidad);

                if (rdf == null)
                {
                    //GuardarLogError("El recurso " + pDocumento.Clave + " no tiene datos en virtuoso.", null);
                    throw new Exception("El recurso " + pDocumentoID + " no tiene datos en virtuoso.");
                }

                MemoryStream buffer = new MemoryStream(rdf);
                StreamReader reader = new StreamReader(buffer);
                rdfTexto = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();

                try
                {
                    if (pEdicion)
                    {
                        controladorDocumentacion.BorrarRDFDeBDRDF(pDocumentoID);
                    }

                    ////No lo guardamos en virtuoso, porque el RDF es diferente al que genera en la edición
                    //if (pOntologia != null)//Para que no se guarde un RDF malo sin los namespaces de la ontología.
                    //{
                    //    ControladorDocumentacion.GuardarRDFEnBDRDF(rdfTexto, pDocumentoID, pProyectoID);
                    //}
                }
                catch (Exception)
                {
                    //GuardarLogErrorAJAX("Error al guardar el RDF en SqlServer: " + ex.ToString());
                }
            }

            mLoggingService.AgregarEntrada("FormSem RDF de documento: " + rdfTexto);

            return rdfTexto;
        }

        /// <summary>
        /// 
        /// </summary>
        ///<param name="pDocumentoID">ID del documento</param>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pNamespaceOnto">Namespace de la ontología</param>
        /// <param name="pUrlOnto">Url de la ontología</param>
        /// <param name="pEnlaceOnto">Enlace de la ontología</param>
        /// <param name="pOntologia">Ontología</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        /// <param name="pRdfTexto">RDF del documento</param>
        /// <param name="pEdicion">Indica si estamos editando el recurso o no</param>
        /// <returns></returns>
        private List<ElementoOntologia> ObtenerInstanciasPrincipalesDocumento(Guid pDocumentoID, Guid pProyectoID, string pNamespaceOnto, string pUrlOnto, string pEnlaceOnto, Ontologia pOntologia, string pUrlIntragnoss, string pRdfTexto, bool pEdicion)
        {
            List<ElementoOntologia> instanciasPrincipales = null;

            GestionOWL gestorOWL = new GestionOWL();
            gestorOWL.UrlOntologia = pUrlOnto;
            gestorOWL.NamespaceOntologia = pNamespaceOnto;
            string urlIntragnoss = null;
            try
            {
                urlIntragnoss = GestionOWL.URLIntragnoss;
            }
            catch { }
            if (string.IsNullOrEmpty(urlIntragnoss))
            {
                GestionOWL.URLIntragnoss = ListaParametrosAplicacion.FirstOrDefault(item => item.Parametro.Equals(TiposParametrosAplicacion.UrlIntragnoss))?.Valor;
            }

            try
            {
                instanciasPrincipales = gestorOWL.LeerFicheroRDF(pOntologia, pRdfTexto, true);
            }
            catch (Exception ex)
            {
                //GuardarMensajeErrorAdmin("El RDF del recurso " + pDocumento.Clave + " no es correcto. " + ex.Message, rdfTexto);
                throw new Exception("El RDF del recurso " + pDocumentoID + " no es correcto. " + ex.Message + Environment.NewLine + pRdfTexto);
            }

            return instanciasPrincipales;
        }

        /// <summary>
        /// Obtiene una propiedad del controlador del SEMCMS.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pProyecto">Proyecto actual</param>
        /// <param name="pIdentidadActual">Identidad actual conectada</param>
        /// <param name="pBaseURLFormulariosSem">Base Url para formularios semánticos</param>
        /// <param name="pUtilIdiomas">Util idiomas</param>
        /// <param name="pBaseURL">Base Url</param>
        /// <param name="pBaseURLIdioma">Base Url con Idioma</param>
        /// <param name="pBaseURLContent">Url del Content</param>
        /// <param name="pBaseURLStatic">Url Static</param>
        /// <param name="pUrlIntragnoss">Url de intragnoss</param>
        /// <param name="pVistaPersonalizada">Indica si la vista que se va usar para el controlador de documento es personalizada</param>
        /// <param name="pParametroProyecto">Parámetros del proyecto</param>
        /// <param name="pEntidadPag">Entidad que contiene la propiedad</param>
        /// <param name="pPropiedadPag">Nombre de la propiedad</param>
        /// <param name="pInicioPag">Inicio de la paginación</param>
        /// <returns>Controlador del SEMCMS</returns>
        public SemanticPropertyModel ObtenerPropiedadControladorSemCMS(Documento pDocumento, Proyecto pProyecto, Identidad pIdentidadActual, string pBaseURLFormulariosSem, UtilIdiomas pUtilIdiomas, string pBaseURL, string pBaseURLIdioma, string pBaseURLContent, string pBaseURLStatic, string pUrlIntragnoss, bool pVistaPersonalizada, Dictionary<string, string> pParametroProyecto, string pEntidadPag, string pPropiedadPag, int pInicioPag)
        {
            SemCmsController semCms = ObtenerControladorSemCMSInt(pDocumento, pProyecto, pIdentidadActual, pBaseURLFormulariosSem, pUtilIdiomas, pBaseURL, pBaseURLIdioma, pBaseURLContent, pBaseURLStatic, pUrlIntragnoss, pVistaPersonalizada, pParametroProyecto, pEntidadPag, pPropiedadPag, pInicioPag, null);

            foreach (SemanticEntityModel entModel in semCms.SemanticResourceModel.RootEntities)
            {
                SemanticPropertyModel propMopdel = ObtenerPropiedadModeloACualquierNivel(pEntidadPag, pPropiedadPag, entModel, entModel.Properties);

                if (propMopdel != null)
                {
                    return propMopdel;
                }
            }

            throw new Exception("No se ha encontrado la propiedad '" + pPropiedadPag + "' de la entidad '" + pEntidadPag + "' en la ontología.");
        }

        /// <summary>
        /// Obtiene un modelo de propiedad a cualquier nivel dado una entidad, el nombre de la propiedad y el modelo de entidad a buscar.
        /// </summary>
        /// <param name="pEntidad">ID de la entidad</param>
        /// <param name="pPropiedad">Nombre de la propiedad</param>
        /// <param name="pModeloEntidad">Modelo de entiad sobre el que buscar</param>
        /// <param name="pProperties">Propiedades sobre las que buscar</param>
        /// <returns>Modelo de propiedad a cualquier nivel</returns>
        private static SemanticPropertyModel ObtenerPropiedadModeloACualquierNivel(string pEntidad, string pPropiedad, SemanticEntityModel pModeloEntidad, List<SemanticPropertyModel> pProperties)
        {
            foreach (SemanticPropertyModel prop in pProperties)
            {
                if (prop.OntologyPropInfo != null)
                {
                    if (pModeloEntidad.Key == pEntidad)
                    {
                        if (prop.Element.Propiedad.Nombre == pPropiedad)
                        {
                            return prop;
                        }
                    }
                    else if (prop.Element.Propiedad.Tipo == TipoPropiedad.ObjectProperty && prop.PropertyValues.Count > 0)
                    {
                        foreach (SemanticPropertyModel.PropertyValue propValue in prop.PropertyValues)
                        {
                            SemanticPropertyModel propHija = ObtenerPropiedadModeloACualquierNivel(pEntidad, pPropiedad, propValue.RelatedEntity, propValue.RelatedEntity.Properties);

                            if (propHija != null)
                            {
                                return propHija;
                            }
                        }
                    }
                }
                else if (prop.Properties != null && prop.Properties.Count > 0)
                {
                    SemanticPropertyModel propHija = ObtenerPropiedadModeloACualquierNivel(pEntidad, pPropiedad, pModeloEntidad, prop.Properties);

                    if (propHija != null)
                    {
                        return propHija;
                    }
                }
            }

            return null;
        }

        #endregion

        #region Acciones Sem Cms

        /// <summary>
        /// Obtiene los datos le los selectores de entidad dependientes de otro.
        /// </summary>
        /// <returns>Datos de selectores separados por '[[|||]]' y separando los datos de cada selector por '[[|]]'</returns>
        public string ObtenerDatosSelectoresEntidadDependientes(string pPropiedad, string pTipoEntidad, string pValor)
        {
            string datos = "";
            List<string> entidades = new List<string>();
            string tipoEntidadBis = pTipoEntidad;
            int indiceBis = pTipoEntidad.IndexOf("_bis");
            if (indiceBis > 0)
            {
                tipoEntidadBis = pTipoEntidad.Substring(0, indiceBis);
            }


            foreach (string valor in pValor.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!entidades.Contains(valor))
                {
                    entidades.Add(valor);
                }
            }

            if (entidades.Count > 0)
            {
                FacetadoCN facetadoCN = new FacetadoCN(mUrlIntragnoss, true, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

                List<KeyValuePair<string, string>> propsSelectEntDependientes = new List<KeyValuePair<string, string>>();

                if (Ontologia.ConfiguracionPlantilla.PropsSelecEntDependientes.ContainsKey(new KeyValuePair<string, string>(pPropiedad, pTipoEntidad)))
                {
                    propsSelectEntDependientes = Ontologia.ConfiguracionPlantilla.PropsSelecEntDependientes[new KeyValuePair<string, string>(pPropiedad, pTipoEntidad)];
                }
                else if (Ontologia.ConfiguracionPlantilla.PropsSelecEntDependientes.ContainsKey(new KeyValuePair<string, string>(pPropiedad, tipoEntidadBis)))
                {
                    propsSelectEntDependientes = Ontologia.ConfiguracionPlantilla.PropsSelecEntDependientes[new KeyValuePair<string, string>(pPropiedad, tipoEntidadBis)];
                }

                foreach (KeyValuePair<string, string> propEnt in propsSelectEntDependientes)
                {
                    ElementoOntologia entidad = Ontologia.GetEntidadTipo(propEnt.Value, false);
                    Propiedad propiedad = entidad.ObtenerPropiedad(propEnt.Key);
                    propiedad.ElementoOntologia = entidad;

                    //Busco la propiedad de la que depende y le doy los valores introducidos en ella en el formulario:
                    Propiedad propDeLaQueDepende = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(propiedad.EspecifPropiedad.SelectorEntidad.PropiedadDeLaQueDepende.Key, propiedad.EspecifPropiedad.SelectorEntidad.PropiedadDeLaQueDepende.Value, Entidades);
                    propDeLaQueDepende.LimpiarValor();

                    foreach (string entidadID in entidades)
                    {
                        propDeLaQueDepende.DarValor(entidadID, null);
                    }

                    FacetadoDS facetadoDS = null;
                    Dictionary<string, List<KeyValuePair<string, string>>> triplesExt = null;
                    SortedDictionary<string, List<string>> aux = null;
                    SemanticPropertyModel semPropModel = new SemanticPropertyModel();
                    semPropModel.Element = new ElementoOrdenado();
                    semPropModel.Element.NombrePropiedad = new KeyValuePair<string, Propiedad>(propiedad.Nombre, propiedad);
                    Dictionary<Guid, string> nombresGrupos = null;
                    DataWrapperIdentidad perfilesGnossDW = null;

                    CargaDatosValoresSelectorEntidadYaAgregados(semPropModel, out facetadoDS, out triplesExt, out aux, out nombresGrupos, out perfilesGnossDW, true);

                    if (datos.Equals(""))
                    {
                        datos = string.Concat(propEnt.Key, "[[|]]", pTipoEntidad, "[[|]]");
                    }
                    else
                    {
                        datos = string.Concat(datos, propEnt.Key, "[[|]]", propEnt.Value, "[[|]]");
                    }


                    foreach (string entidadID in triplesExt.Keys)
                    {
                        string text = "";

                        foreach (string propiedadEdicion in semPropModel.SpecificationProperty.SelectorEntidad.PropiedadesEdicion)
                        {
                            foreach (KeyValuePair<string, string> valorPropiedad in triplesExt[entidadID].Where(item => item.Key.Equals(propiedadEdicion)))
                            {
                                text += $"{valorPropiedad.Value} ";
                            }
                        }

                        foreach (KeyValuePair<string, string> valorProp in triplesExt[entidadID].Where(item => !semPropModel.SpecificationProperty.SelectorEntidad.PropiedadesEdicion.Contains(item.Key)))
                        {
                            text += $"{valorProp.Value} ";
                        }

                        if (text.Length > 0)
                        {
                            text = text.Substring(0, text.Length - 1);
                        }

                        datos = string.Concat(datos, entidadID, "[[|]]", text, "[[|]]");
                    }

                    datos = string.Concat(datos, "[[|||]]");

                    if (facetadoDS != null)
                    {
                        facetadoDS.Dispose();
                    }
                }

                facetadoCN.Dispose();
            }

            return datos;
        }

        #endregion

        #region Guardado SEMCMS

        /// <summary>
        /// Recoge los valores rdf almacenados en el txtHack.
        /// </summary>
        /// <param name="pTexto">Texto RDF a guardar</param>
        /// <param name="pIDsEntidadesAux">IDs de las entidades auxiliares</param>
        /// <returns>Devuelve la lista con las entidades pricipales de la ontología</returns>
        public List<ElementoOntologia> RecogerValoresRdf(string pTexto, string pIDsEntidadesAux)
        {
            if (pTexto.Contains("<||>"))
            {
                pTexto = pTexto.Substring(0, pTexto.IndexOf("<||>"));
            }

            if (pTexto.Contains("<></>"))
            {
                string error = "El RDF que se va a guardar contiene alguna entidad vacía debido a una mala configuración.";
                GuardarMensajeErrorAdmin(error, null);
                throw new Exception(error);
            }

            string textoIDSEntidadesAux = null;

            if (!string.IsNullOrEmpty(pIDsEntidadesAux))
            {
                textoIDSEntidadesAux = pIDsEntidadesAux;
            }

            foreach (string apano in TipoEntidadesApanyadas.Keys.Reverse())
            {
                pTexto = pTexto.Replace(apano, TipoEntidadesApanyadas[apano]);

                if (textoIDSEntidadesAux != null)
                {
                    textoIDSEntidadesAux = textoIDSEntidadesAux.Replace(apano, TipoEntidadesApanyadas[apano]);

                    string apanoAux = apano;
                    string sustitutoApanoAux = TipoEntidadesApanyadas[apano];
                    if ((apanoAux.Contains("#") || apanoAux.Contains("/")) && (sustitutoApanoAux.Contains("#") || sustitutoApanoAux.Contains("/")))
                    {
                        if (apanoAux.LastIndexOf("#") > apanoAux.LastIndexOf("/"))
                        {
                            apanoAux = apanoAux.Substring(apanoAux.LastIndexOf("#") + 1);
                        }
                        else
                        {
                            apanoAux = apanoAux.Substring(apanoAux.LastIndexOf("/") + 1);
                        }
                        if (sustitutoApanoAux.LastIndexOf("#") > sustitutoApanoAux.LastIndexOf("/"))
                        {
                            sustitutoApanoAux = sustitutoApanoAux.Substring(sustitutoApanoAux.LastIndexOf("#") + 1);
                        }
                        else
                        {
                            sustitutoApanoAux = sustitutoApanoAux.Substring(sustitutoApanoAux.LastIndexOf("/") + 1);
                        }
                        textoIDSEntidadesAux = textoIDSEntidadesAux.Replace(apanoAux, sustitutoApanoAux);
                    }
                }
            }

            List<ElementoOntologia> listaEntidadesPrinc = new List<ElementoOntologia>();
            List<string> entidadesYaAgregadas = new List<string>();
            List<string> idiomasUsados = new List<string>();

            while (pTexto.Length > 0)
            {
                listaEntidadesPrinc.Add(AgregarEntidadDeXML(ExtraerElementoXML(ref pTexto), null, entidadesYaAgregadas, idiomasUsados, textoIDSEntidadesAux));
            }

            if (!string.IsNullOrEmpty(PropiedadIdiomaBusquedaComunidad))
            {
                Propiedad propIdio = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(PropiedadIdiomaBusquedaComunidad, null, listaEntidadesPrinc);

                if (propIdio != null)
                {
                    propIdio.LimpiarValor();

                    if (idiomasUsados.Count > 0)
                    {
                        foreach (string idioma in idiomasUsados)
                        {
                            propIdio.DarValor(idioma, null);
                        }
                    }
                    else if (!string.IsNullOrEmpty(IdiomaDefecto))
                    {
                        propIdio.DarValor(IdiomaDefecto, null);
                    }
                    else
                    {
                        propIdio.DarValor(Ontologia.IdiomaUsuario, null);
                    }
                }
            }

            ConvertirPerfilesPropsSelectPersonasEnUsuarios();

            return listaEntidadesPrinc;
        }

        /// <summary>
        /// Convierte los perfiles guardados en los selectores de personas en  IDs de usuario.
        /// </summary>
        private void ConvertirPerfilesPropsSelectPersonasEnUsuarios()
        {
            if (mPropsSelectorPersonas != null)
            {
                List<Guid> perfiles = new List<Guid>();

                foreach (Propiedad propiedad in mPropsSelectorPersonas)
                {
                    foreach (string valor in propiedad.ValoresUnificados.Keys)
                    {
                        Guid perfilID = new Guid(valor);

                        if (!perfiles.Contains(perfilID))
                        {
                            perfiles.Add(perfilID);
                        }
                    }
                }

                IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                Dictionary<Guid, Guid> perfilesUsusID = identCN.ObtenerUsuariosIDPorPerfilID(perfiles);
                identCN.Dispose();

                foreach (Propiedad propiedad in mPropsSelectorPersonas)
                {
                    List<string> listaValores = new List<string>(propiedad.ValoresUnificados.Keys);
                    propiedad.LimpiarValor();

                    foreach (string valor in listaValores)
                    {
                        Guid perfilID = Guid.Empty;

                        if (Guid.TryParse(valor, out perfilID))
                        {
                            if (perfilesUsusID.ContainsKey(perfilID))
                            {
                                propiedad.AgregarValor("http://gnoss/" + perfilesUsusID[perfilID].ToString().ToUpper());
                            }
                        }
                        else if (valor.StartsWith("http://gnoss/"))
                        {
                            propiedad.AgregarValor(valor);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Agrega una entidad almacenada en una cadena XML.
        /// </summary>
        /// <param name="pTexto">Texto con formato XML</param>
        /// <param name="pPropiedadPadre">Propiedad padre</param>
        /// <param name="pEntidadesYaAgregadas">Entidades ya agregadas</param>
        /// <param name="pIdiomasUsados">Idiomas usados</param>
        /// <param name="pTextoIDSEntidadesAux">Texto con los IDs de de las entidades auxiliares</param>
        /// <returns>Entidad extraida de la cadena</returns>
        private ElementoOntologia AgregarEntidadDeXML(string pTexto, Propiedad pPropiedadPadre, List<string> pEntidadesYaAgregadas, List<string> pIdiomasUsados, string pTextoIDSEntidadesAux)
        {
            string tipoEntidad = ExtraerRaizXML(ref pTexto);
            ElementoOntologia instanciaEntidad = mOntologia.GetEntidadTipo(tipoEntidad, true);

            if (pTextoIDSEntidadesAux == null)
            {
                instanciaEntidad.ID = ObtenerIDEntidadAntigua(instanciaEntidad, pPropiedadPadre, pEntidadesYaAgregadas);
            }
            else
            {
                string tipoEntidadTxtIDs = null;
                string nodoID = "";

                if (pTextoIDSEntidadesAux.Length > 0)
                {
                    tipoEntidadTxtIDs = ExtraerRaizXML(ref pTextoIDSEntidadesAux);
                }

                if (pTextoIDSEntidadesAux.Length > 0)
                {
                    nodoID = ExtraerElementoXML(ref pTextoIDSEntidadesAux);
                }

                if (tipoEntidadTxtIDs == tipoEntidad && nodoID.StartsWith("<id>"))
                {
                    nodoID = nodoID.Replace("<id>", "").Replace("</id>", "");
                    instanciaEntidad.ID = nodoID;
                }
                else
                {
                    instanciaEntidad.ID = instanciaEntidad.TipoEntidadGeneracionClases + "_" + mDocumentoID.ToString() + "_" + Guid.NewGuid();
                }
            }

            while (pTexto.Length > 0)
            {
                AgregarPropiedadAEntidadXML(instanciaEntidad, ExtraerElementoXML(ref pTexto), pEntidadesYaAgregadas, pIdiomasUsados, ref pTextoIDSEntidadesAux);
            }

            return instanciaEntidad;
        }

        /// <summary>
        /// Agrega a una entidad un propiedad almacenada en una cadena con formato XML.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pTexto">Cadena con formato XML con la propiedad y su valor</param>
        /// <param name="pEntidadesYaAgregadas">Entidades ya agregadas</param>
        /// <param name="pIdiomasUsados">Idiomas usados</param>
        /// <param name="pTextoIDSEntidadesAux">Texto con los IDs de de las entidades auxiliares</param>
        private void AgregarPropiedadAEntidadXML(ElementoOntologia pEntidad, string pTexto, List<string> pEntidadesYaAgregadas, List<string> pIdiomasUsados, ref string pTextoIDSEntidadesAux)
        {
            string nombreProp = ExtraerRaizXML(ref pTexto);

            #region Propiedades repetidas

            if (ConfigGenXml.PropsRepetidas != null && nombreProp.Contains("_Rep_"))
            {
                nombreProp = nombreProp.Substring(0, nombreProp.IndexOf("_Rep_"));
            }

            #endregion

            Propiedad propiedad = pEntidad.ObtenerPropiedad(nombreProp);
            propiedad.ElementoOntologia = pEntidad;

            if (propiedad.Tipo == TipoPropiedad.DatatypeProperty)
            {
                string valor = pTexto.Replace("[--C]", "<").Replace("[C--]", ">");
                if (!string.IsNullOrEmpty(valor))
                {
                    if (propiedad.EspecifPropiedad.TipoCampo.Equals(TipoCampoOntologia.DateTime))
                    {
                        DateTime fechaCorrecta = new DateTime();
                        if (!DateTime.TryParse(valor, out fechaCorrecta))
                        {
                            try
                            {
                                fechaCorrecta = DateTime.ParseExact(valor, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                            }
                            catch (Exception ex)
                            {
                                GuardarMensajeErrorAdmin("La fecha '" + valor + "' no es una fecha correcta.", ex);
                                GuardarLogErrorAJAX("La fecha '" + valor + "' no es una fecha correcta.");
                                throw new Exception("Fecha incorreta.", ex);
                            }
                        }
                    }
                    #region Modficar valor según propiedad

                    if (propiedad.EspecifPropiedad.GuardarFechaComoEntero)
                    {
                        long fechAux;
                        if (valor.Contains("/") || (valor.Length != 14 || !long.TryParse(valor, out fechAux)))
                        {
                            try
                            {
                                string[] diaMesAno = valor.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                                if (!propiedad.EspecifPropiedad.FechaConHora || !valor.Contains(" "))
                                {
                                    valor = diaMesAno[2] + diaMesAno[1] + diaMesAno[0] + "000000";
                                }
                                else
                                {
                                    string[] hora = diaMesAno[2].Split(' ')[1].Split(':');
                                    valor = diaMesAno[2].Split(' ')[0] + diaMesAno[1] + diaMesAno[0] + hora[0] + hora[1] + hora[2];
                                }

                                if (long.Parse(valor).ToString().Length != 14)
                                {
                                    throw new Exception("La fecha '" + valor + "' no tiene el formato correto.");
                                }
                            }
                            catch (Exception ex)
                            {
                                GuardarMensajeErrorAdmin("Fecha incorreta al convertirla a entero. Propiedad '" + propiedad.Nombre + "'", ex);
                                GuardarLogErrorAJAX(ex.ToString());
                                throw new Exception("Fecha incorreta al convertirla a entero.");
                            }
                        }
                    }

                    #endregion

                    if (string.IsNullOrEmpty(IdiomaDefecto) || !valor.Contains("[|lang|]"))
                    {
                        if (propiedad.FunctionalProperty) //Solo puede tomar un valor.
                        {
                            propiedad.UnicoValor = new KeyValuePair<string, ElementoOntologia>(valor, null);
                        }
                        else //Puede tomar varios valores.
                        {
                            if (!propiedad.ListaValores.ContainsKey(valor))
                            {
                                propiedad.ListaValores.Add(valor, null);
                            }
                        }

                        if (propiedad.EspecifPropiedad.GrafoAutocompletar != null && propiedad.EspecifPropiedad.GuardarValoresAutocompletar)
                        {
                            AgregarValorGrafoAutocompletar(propiedad.EspecifPropiedad.GrafoAutocompletar, valor);
                        }
                    }
                    else //MultiIdioma
                    {
                        List<string> idiomasSinAgregar = new List<string>(IdiomasDisponibles.Keys);
                        string valorPorDefectoIdioma = "";

                        foreach (string valorIdioma in valor.Split(new string[] { "[|lang|]" }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            string idioma = valorIdioma.Substring(valorIdioma.LastIndexOf("@") + 1);
                            string valorSinIdioma = valorIdioma.Substring(0, valorIdioma.LastIndexOf("@"));

                            idiomasSinAgregar.Remove(idioma);

                            if (!pIdiomasUsados.Contains(idioma))
                            {
                                pIdiomasUsados.Add(idioma);
                            }

                            if (!propiedad.ListaValoresIdioma.ContainsKey(idioma))
                            {
                                propiedad.ListaValoresIdioma.Add(idioma, new Dictionary<string, ElementoOntologia>());
                            }
                            else if (propiedad.FunctionalProperty || propiedad.CardinalidadMenorOIgualUno)
                            {
                                //Cuando se edita un documento, se está agregando 2 veces la descripción. Hay que eliminar la vieja y cargar la nueva.
                                propiedad.ListaValoresIdioma.Remove(idioma);
                                propiedad.ListaValoresIdioma.Add(idioma, new Dictionary<string, ElementoOntologia>());
                            }

                            propiedad.ListaValoresIdioma[idioma].Add(valorSinIdioma, null);

                            if (idioma == IdiomaDefecto)
                            {
                                valorPorDefectoIdioma = valorSinIdioma;
                            }
                        }

                        if (string.IsNullOrEmpty(PropiedadIdiomaBusquedaComunidad))
                        {
                            foreach (string idioma in idiomasSinAgregar)
                            {
                                if (!propiedad.ListaValoresIdioma.ContainsKey(idioma))
                                {
                                    propiedad.ListaValoresIdioma.Add(idioma, new Dictionary<string, ElementoOntologia>());
                                    //Evitamos que si el idioma ya está agregado se agrege un valor vacio.
                                    propiedad.ListaValoresIdioma[idioma].Add(valorPorDefectoIdioma, null);
                                }
                            }
                        }
                    }
                }
            }
            else
            {//Es ObjectPropert

                if (propiedad.EspecifPropiedad.SelectorEntidad != null && propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion != "Edicion")
                {
                    if (!string.IsNullOrEmpty(pTexto))
                    {
                        if (propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "GruposGnoss" && pTexto.StartsWith("g_"))
                        {
                            pTexto = "http://gnoss/" + pTexto.Substring(2).ToUpper();
                        }
                        else if (propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "PersonaGnoss" && !pTexto.StartsWith("http://"))
                        {
                            if (mPropsSelectorPersonas == null)
                            {
                                mPropsSelectorPersonas = new List<Propiedad>();
                            }

                            mPropsSelectorPersonas.Add(propiedad);
                        }
                        else if (propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "UrlRecursoSemantico" && !pTexto.StartsWith(mUrlIntragnoss + "items/"))
                        {
                            if (mUrlRecursoSemEntidadPrincipal == null)
                            {
                                mUrlRecursoSemEntidadPrincipal = new Dictionary<string, string>();
                            }

                            if (mUrlRecursoSemEntidadPrincipal.ContainsKey(pTexto))
                            {
                                pTexto = mUrlRecursoSemEntidadPrincipal[pTexto];
                            }
                            else
                            {
                                string entidadRecurso = ObtenerEntidadPrincipalRecursoSemAPartirUrl(pTexto, propiedad);

                                if (string.IsNullOrEmpty(entidadRecurso))
                                {
                                    string mens = "La URL del recurso '" + pTexto + "' insertada en la propiedad '" + propiedad.Nombre + "' de la entidad '" + propiedad.ElementoOntologia.TipoEntidad + "' no es una URL de un recurso semántico.";
                                    GuardarMensajeErrorAdmin(mens, null);
                                    throw new Exception(mens);
                                }

                                mUrlRecursoSemEntidadPrincipal.Add(pTexto, entidadRecurso);
                                pTexto = entidadRecurso;
                            }
                        }

                        if (propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "Tesauro")
                        {
                            if (pTexto.LastIndexOf('|') == (pTexto.Length - 1))
                            {
                                pTexto = pTexto.Substring(0, pTexto.Length - 1);
                            }

                            if (propiedad.FunctionalProperty)
                            {
                                propiedad.FunctionalProperty = false; //Para que guarde lista valores
                            }

                            foreach (string cat in pTexto.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                if (!propiedad.ListaValores.ContainsKey(cat))
                                {
                                    propiedad.ListaValores.Add(cat, null);
                                }
                            }
                        }
                        else if (propiedad.FunctionalProperty) //Solo puede tomar un valor.
                        {
                            propiedad.UnicoValor = new KeyValuePair<string, ElementoOntologia>(pTexto, null);
                        }
                        else //Puede tomar varios valores.
                        {
                            if (!propiedad.ListaValores.ContainsKey(pTexto))
                            {
                                propiedad.ListaValores.Add(pTexto, null);
                            }
                        }
                    }
                }
                else
                {
                    string textoIDSEntidadesAux = null;

                    if (pTextoIDSEntidadesAux != null)
                    {
                        textoIDSEntidadesAux = "";
                        string nombrePropTxtIDs = "";

                        if (pTextoIDSEntidadesAux.Length > 0)
                        {
                            textoIDSEntidadesAux = ExtraerElementoXML(ref pTextoIDSEntidadesAux, nombreProp);

                            if (!string.IsNullOrEmpty(textoIDSEntidadesAux))
                            {
                                nombrePropTxtIDs = ExtraerRaizXML(ref textoIDSEntidadesAux);
                            }
                        }

                        #region Propiedades repetidas

                        //TODO Revisar con repetidos
                        //if (ConfigGenXml.PropsRepetidas != null && nombrePropTxtIDs.Contains("_Rep_"))
                        //{
                        //    nombrePropTxtIDs = nombrePropTxtIDs.Substring(0, nombrePropTxtIDs.IndexOf("_Rep_"));
                        //}

                        #endregion

                        if (nombrePropTxtIDs != nombreProp)
                        {
                            textoIDSEntidadesAux = "";
                        }
                    }

                    ElementoOntologia entidadHija = AgregarEntidadDeXML(pTexto, propiedad, pEntidadesYaAgregadas, pIdiomasUsados, textoIDSEntidadesAux);

                    if (propiedad.FunctionalProperty || entidadHija.TienePropiedadesConValor)
                    {
                        if (!pEntidad.EntidadesRelacionadas.Contains(entidadHija))
                        {
                            pEntidad.EntidadesRelacionadas.Add(entidadHija);
                        }

                        if (propiedad.FunctionalProperty) //Solo puede tomar un valor.
                        {
                            propiedad.UnicoValor = new KeyValuePair<string, ElementoOntologia>(entidadHija.ID, entidadHija);
                        }
                        else //Puede tomar varios valores.
                        {
                            propiedad.ListaValores.Add(entidadHija.ID, entidadHija);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene el ID de una entidad antigua de un tipo concreto según la propiedad padre.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pPropiedadPadre">Propiedad padre</param>
        /// <param name="pEntidadesYaAgregadas">Entidades ya agregadas</param>
        /// <returns>ID de una entidad antigua de un tipo concreto según la propiedad padre</returns>
        private string ObtenerIDEntidadAntigua(ElementoOntologia pEntidad, Propiedad pPropiedadPadre, List<string> pEntidadesYaAgregadas)
        {
            string entidadID = null;
            foreach (ElementoOntologia entidadPrin in mEntidades)
            {
                if (pPropiedadPadre == null)
                {
                    if (entidadPrin.TipoEntidad == pEntidad.TipoEntidad)
                    {
                        entidadID = entidadPrin.ID;
                        break;
                    }
                }
                else
                {
                    Propiedad propAux = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(pPropiedadPadre.Nombre, pPropiedadPadre.ElementoOntologia.TipoEntidad, entidadPrin);

                    if (propAux != null && propAux.Rango == pPropiedadPadre.Rango)
                    {
                        if (pPropiedadPadre.FunctionalProperty && propAux.UnicoValor.Key != null && pPropiedadPadre.UnicoValor.Key == null)
                        {
                            if (propAux.UnicoValor.Value.TipoEntidad == pEntidad.TipoEntidad)
                            {
                                entidadID = propAux.UnicoValor.Key;
                            }
                        }
                        else if (!pPropiedadPadre.FunctionalProperty && propAux.ListaValores.Count > pPropiedadPadre.ListaValores.Count)
                        {
                            entidadID = new List<string>(propAux.ListaValores.Keys)[pPropiedadPadre.ListaValores.Count];

                            if (EntidadesIDProhibidoUsar != null && EntidadesIDProhibidoUsar.Contains(entidadID))
                            {
                                propAux.LimpiarValor(entidadID);

                                if (propAux.ListaValores.Count > pPropiedadPadre.ListaValores.Count)
                                {
                                    entidadID = new List<string>(propAux.ListaValores.Keys)[pPropiedadPadre.ListaValores.Count];
                                }
                                else
                                {
                                    entidadID = null;
                                }
                            }
                        }

                        break;
                    }
                }
            }

            if (entidadID != null && !pEntidadesYaAgregadas.Contains(entidadID) && !entidadID.Contains("http://"))
            {
                pEntidadesYaAgregadas.Add(entidadID);
            }
            else
            {
                entidadID = pEntidad.TipoEntidadGeneracionClases + "_" + mDocumentoID.ToString() + "_" + Guid.NewGuid();
            }

            return entidadID;
        }

        /// <summary>
        /// Extrae el siguiente elemento con todos sus hijos de una cadena XML, eliminandola de la cadena.
        /// </summary>
        /// <param name="pTexto">Cadena XML</param>
        /// <returns>Siguiente elemento con todos sus hijos de una cadena XML</returns>
        private string ExtraerElementoXML(ref string pTexto)
        {
            string elemento = pTexto.Substring(1, pTexto.IndexOf(">") - 1);
            string elemCierreXML = "</" + elemento + ">";
            int indiceCierre = pTexto.IndexOf(elemCierreXML) + elemCierreXML.Length;

            string trozo = pTexto.Substring(0, indiceCierre);
            pTexto = pTexto.Substring(indiceCierre);
            return trozo;
        }

        /// <summary>
        /// Extrae el siguiente elemento con todos sus hijos de una cadena XML, eliminandola de la cadena.
        /// </summary>
        /// <param name="pTexto">Cadena XML</param>
        /// <param name="">Elemento a extraer</param>
        /// <returns>Siguiente elemento con todos sus hijos de una cadena XML</returns>
        private string ExtraerElementoXML(ref string pTexto, string pElemento)
        {
            string elemInicioXML = "<" + pElemento + ">";
            string elemCierreXML = "</" + pElemento + ">";
            string trozo = "";

            int indiceApertura = pTexto.IndexOf(elemInicioXML);

            if (indiceApertura != -1)
            {
                string trozo1 = pTexto.Substring(0, indiceApertura);
                pTexto = pTexto.Substring(indiceApertura);
                trozo = pTexto.Substring(0, pTexto.IndexOf(elemCierreXML) + elemCierreXML.Length);
                pTexto = trozo1 + pTexto.Substring(pTexto.IndexOf(elemCierreXML) + elemCierreXML.Length);
            }

            return trozo;
        }

        /// <summary>
        /// Extrae el elemnto raiz de una cadena con formato XML y modifica el texto para que devolver el interior del elemento raiz.
        /// </summary>
        /// <param name="pTexto">Cadena con formato XML</param>
        /// <returns>Elemnto raiz de una cadena con formato XML</returns>
        private string ExtraerRaizXML(ref string pTexto)
        {
            string textoOriginal = pTexto;
            string raiz = "";
            string elemCierreXML = "";
            try
            {
                raiz = pTexto.Substring(1, pTexto.IndexOf(">") - 1);
                elemCierreXML = "</" + raiz + ">";
                pTexto = pTexto.Substring(pTexto.IndexOf(">") + 1);
                pTexto = pTexto.Substring(0, pTexto.IndexOf(elemCierreXML));
                return raiz;
            }
            catch (Exception ex)
            {
                GuardarLogErrorAJAX($"Error al intentar extraer la raíz: {ex.Message}. \r\n\r\nTexto Original: {textoOriginal}\r\n\r\nTexto con modificaciones: {pTexto}\r\n\r\nRaíz: {raiz}\r\n\r\nElemCierreXML: {elemCierreXML}");
                throw;
            }
        }

        /// <summary>
        /// Agrega a la lista de grafos, un nuevo grafo con un valor.
        /// </summary>
        /// <param name="pGrafo">Grafo</param>
        /// <param name="pValor">Valor</param>
        private void AgregarValorGrafoAutocompletar(string pGrafo, string pValor)
        {
            if (string.IsNullOrEmpty(pGrafo) || string.IsNullOrEmpty(pValor))
            {
                return;
            }

            if (ValoresGrafoAutocompletar.ContainsKey(pGrafo))
            {
                if (!ValoresGrafoAutocompletar[pGrafo].Contains(pValor))
                {
                    ValoresGrafoAutocompletar[pGrafo].Add(pValor);
                }
            }
            else
            {
                List<string> listaValores = new List<string>();
                listaValores.Add(pValor);
                ValoresGrafoAutocompletar.Add(pGrafo, listaValores);
            }
        }

        /// <summary>
        /// Obtiene la entidad principal de un recurso semántico dada su URL.
        /// </summary>
        /// <param name="pUrlRec">URL del recurso semántio</param>
        /// <param name="pPropiedad">Propiedad con el selector</param>
        private string ObtenerEntidadPrincipalRecursoSemAPartirUrl(string pUrlRec, Propiedad pPropiedad)
        {
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Guid ontologiaID = docCN.ObtenerOntologiaAPartirNombre(mProyectoActual.Clave, pPropiedad.EspecifPropiedad.SelectorEntidad.Grafo);

            if (ontologiaID == Guid.Empty)
            {
                string mens = "Selector de entidad de la propiedad '" + pPropiedad.Nombre + "' de la entidad '" + pPropiedad.ElementoOntologia.TipoEntidad + "' está mal configurado, el grafo '" + pPropiedad.EspecifPropiedad.SelectorEntidad.Grafo + "' es incorrecto.";
                GuardarMensajeErrorAdmin(mens, null);
                throw new Exception(mens);
            }

            try
            {
                if (pUrlRec.EndsWith("/"))
                {
                    pUrlRec = pUrlRec.Substring(0, pUrlRec.Length - 1);
                }

                Guid docID = new Guid(pUrlRec.Substring(pUrlRec.LastIndexOf("/") + 1));
                GestorDocumental gestorDoc = new GestorDocumental(docCN.ObtenerDocumentoPorID(docID), mLoggingService, mEntityContext);
                docCN.Dispose();

                if (gestorDoc.ListaDocumentos[docID].ElementoVinculadoID != ontologiaID)
                {
                    throw new Exception("El recurso '" + pUrlRec + "' no pertenece a la ontología '" + pPropiedad.EspecifPropiedad.SelectorEntidad.Grafo + "'.");
                }

                string nombreOntologia;
                Ontologia ontologia;
                string rdfTexto;
                List<ElementoOntologia> entidadesPrinc = ObtenerEntidadesPrincipalesRecursoDeBD(gestorDoc.ListaDocumentos[docID], ontologiaID, BaseURLFormulariosSem, mUrlIntragnoss, mUtilIdiomas, mProyectoActual, out nombreOntologia, out ontologia, out rdfTexto, null);
                gestorDoc.Dispose();
                return entidadesPrinc[0].Uri;

            }
            catch (Exception ex)
            {
                string mens = "El recurso '" + pUrlRec + "' introducido en la propiedad '" + pPropiedad.Nombre + "' de la entidad '" + pPropiedad.ElementoOntologia.TipoEntidad + "' no pertenece a la ontología con grafo '" + pPropiedad.EspecifPropiedad.SelectorEntidad.Grafo + "'.";
                GuardarMensajeErrorAdmin(mens, null);
                GuardarLogErrorAJAX(ex.ToString());
                throw new Exception(mens);
            }
        }

        #endregion

        #endregion

        #region Propiedades

        /// <summary>
        /// Modelo del SEM CMS.
        /// </summary>
        public SemanticResourceModel SemanticResourceModel
        {
            get
            {
                return mSemRecModel;
            }
        }

        /// <summary>
        /// Ontología a la que pertenecerá el recurso.
        /// </summary>
        public Ontologia Ontologia
        {
            get
            {
                return mOntologia;
            }
        }

        /// <summary>
        /// Entidades semánticas raíz del recurso
        /// </summary>
        public List<ElementoOntologia> Entidades
        {
            get
            {
                return mEntidades;
            }
        }

        /// <summary>
        /// Tipo de las entidades apañandas, apaño - verdadero valor.
        /// </summary>
        private Dictionary<string, string> TipoEntidadesApanyadas
        {
            get
            {
                if (mTipoEntidadesApanyadas == null)
                {
                    mTipoEntidadesApanyadas = new Dictionary<string, string>();
                }

                return mTipoEntidadesApanyadas;
            }
        }

        /// <summary>
        /// ID de los grupos ocultos.
        /// </summary>
        private List<string> IDGruposOcultos
        {
            get
            {
                if (mIDGruposOcultos == null)
                {
                    mIDGruposOcultos = new List<string>();
                }

                return mIDGruposOcultos;
            }
        }

        /// <summary>
        /// Indica si se está editando un recurso pre-creado de la carga masiva.
        /// </summary>
        public bool EditandoRecursoCargaMasiva
        {
            get
            {
                return mEditandoRecursoCargaMasiva;
            }
            set
            {
                mEditandoRecursoCargaMasiva = value;
            }
        }

        /// <summary>
        /// Idioma por defecto si hay multiIdioma.
        /// </summary>
        public string IdiomaDefecto
        {
            get
            {
                return mIdiomaDefecto;
            }
            set
            {
                mIdiomaDefecto = value;
            }
        }

        /// <summary>
        /// Lista de idiomas de la forma (es, Español)
        /// </summary>
        public Dictionary<string, string> IdiomasDisponibles
        {
            get
            {
                return mIdiomasDisponibles;
            }
            set
            {
                mIdiomasDisponibles = value;
            }
        }

        /// <summary>
        /// Indica si se debe ocultar la información a las personas que no son miembros de la comunidad.
        /// </summary>
        public bool OcultarInfoNoEsMiembro(Guid pIdentidadID)
        {
            return mOntologia.ConfiguracionPlantilla.OcultarRecursoAUsuarioInvitado && pIdentidadID.Equals(UsuarioAD.Invitado);
        }

        /// <summary>
        /// Configuración general del Xml.
        /// </summary>
        public EstiloPlantillaConfigGen ConfigGenXml
        {
            get
            {
                return (EstiloPlantillaConfigGen)mOntologia.EstilosPlantilla["[" + LectorXmlConfig.NodoConfigGen + "]"][0];
            }
        }

        /// <summary>
        /// Lista con los valores que hay que insertar en un determinado grafo para autocompletar.
        /// </summary>
        public Dictionary<string, List<string>> ValoresGrafoAutocompletar
        {
            get
            {
                if (mValoresGrafoAutocompletar == null)
                {
                    mValoresGrafoAutocompletar = new Dictionary<string, List<string>>();
                }

                return mValoresGrafoAutocompletar;
            }
        }

        /// <summary>
        /// ID y tipo de entidad de la propiedad cuyo valor está condicionado por otra propiedad y entidad.
        /// </summary>
        public Dictionary<KeyValuePair<string, string>, KeyValuePair<string, string>> PropsEntGrafoDependientes
        {
            get
            {
                if (mPropsEntGrafoDependientes == null)
                {
                    mPropsEntGrafoDependientes = new Dictionary<KeyValuePair<string, string>, KeyValuePair<string, string>>();
                    foreach (string keyEstilo in mOntologia.EstilosPlantilla.Keys)
                    {
                        foreach (EstiloPlantilla estilo in mOntologia.EstilosPlantilla[keyEstilo])
                        {
                            if (estilo is EstiloPlantillaEspecifProp && ((EstiloPlantillaEspecifProp)estilo).GrafoDependiente != null && ((EstiloPlantillaEspecifProp)estilo).PropDependiente.Key != null)
                            {
                                mPropsEntGrafoDependientes.Add(new KeyValuePair<string, string>(((EstiloPlantillaEspecifProp)estilo).PropDependiente.Key, ((EstiloPlantillaEspecifProp)estilo).PropDependiente.Value), new KeyValuePair<string, string>(((EstiloPlantillaEspecifProp)estilo).NombreRealPropiedad, ((EstiloPlantillaEspecifProp)estilo).NombreEntidad));
                            }
                        }
                    }
                }

                return mPropsEntGrafoDependientes;
            }
        }

        /// <summary>
        /// Indica si la vista que se va usar para el controlador de documento es personalizada.
        /// </summary>
        public bool VistaPersonalizada
        {
            get { return mVistaPersonalizada; }
            set { mVistaPersonalizada = value; }
        }

        /// <summary>
        /// Url de la propiedad que debe contener al menos un idioma para realizar la busqueda por el mismo.
        /// </summary>
        public string PropiedadIdiomaBusquedaComunidad
        {
            get
            {
                return mPropiedadIdiomaBusquedaComunidad;
            }
            set
            {
                string valor = value;
                if (valor.StartsWith("dce:"))
                {
                    valor = valor.Replace("dce:", "dc:");
                }

                if (valor.Contains(":") && Ontologia.NamespacesDefinidosInv.ContainsKey(valor.Split(':')[0]))
                {
                    mPropiedadIdiomaBusquedaComunidad = Ontologia.NamespacesDefinidosInv[valor.Split(':')[0]] + valor.Substring(valor.IndexOf(":") + 1);
                }
                else
                {
                    mPropiedadIdiomaBusquedaComunidad = valor;
                }
            }
        }

        /// <summary>
        /// Propiedad, ID de entidad y Número de inicio de página de la propiedad sobre la que se está haciendo callback para obtener más datos.
        /// </summary>
        public KeyValuePair<string, KeyValuePair<string, int>> PropiedadCallbackTraerMas
        {
            get { return mPropiedadCallbackTraerMas; }
            set { mPropiedadCallbackTraerMas = value; }
        }

        /// <summary>
        /// RDF del recurso.
        /// </summary>
        public string ResourceRDF { get; set; }

        private AD.EntityModel.Models.PersonaDS.Persona FilaPersona
        {
            get
            {
                AD.EntityModel.Models.PersonaDS.Persona filaPersona = null;

                if (mIdentidadActual != null && mIdentidadActual.Persona != null)
                {
                    filaPersona = mIdentidadActual.Persona.FilaPersona;
                }
                return filaPersona;
            }
        }

        /// <summary>
        /// Propiedades que no deben pintarse.
        /// </summary>
        public Dictionary<string, List<string>> PropiedadesOmitirPintado
        {
            get
            {
                if (mPropiedadesOmitirPintado == null)
                {
                    mPropiedadesOmitirPintado = new Dictionary<string, List<string>>();
                }

                return mPropiedadesOmitirPintado;
            }
            set
            {
                mPropiedadesOmitirPintado = value;
            }
        }

        /// <summary>
        /// IDs de entidades externas editables y su ID de documento que el corresponde.
        /// </summary>
        public Dictionary<string, EntidadExtEditableDoc> EntidadesExtEditablesDocID { get; set; }

        /// <summary>
        /// IDs de entidades auxiliares (externas editables) que no se pueden usar al recuperar los antiguos IDs.
        /// </summary>
        public List<string> EntidadesIDProhibidoUsar { get; set; }

        #endregion
    }
}
