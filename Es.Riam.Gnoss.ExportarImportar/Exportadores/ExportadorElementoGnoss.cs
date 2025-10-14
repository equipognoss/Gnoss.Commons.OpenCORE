using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.RDF.Model;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.LinkedOpenDataCL;
using Es.Riam.Gnoss.Elementos;
using Es.Riam.Gnoss.Elementos.Comentario;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.LinkedOpenData;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.ExportarImportar.ElementosOntologia;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.RDF;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces;
using Es.Riam.Metagnoss.ExportarImportar;
using Es.Riam.Semantica.OWL;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SemWeb;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Es.Riam.Gnoss.ExportarImportar.Exportadores
{
    #region Enumeraciones

    /// <summary>
    /// Enumeración de modos de exportación.
    /// </summary>
    public enum EModosExportacion
    {
        /// <summary>
        /// Solo se exportan sus atributos.
        /// </summary>
        SoloAtributos = 0,
        /// <summary>
        /// Se exportan los atributos y las entidades relacionadas.
        /// </summary>
        AtributosYPropiedades
    }

    #endregion

    /// <summary>
    /// Exportador de elemento gnoss
    /// </summary>
    public abstract class ExportadorElementoGnoss : IDisposable
    {
        #region Miembros

        #region privados

        /// <summary>
        /// Ontología a seguir
        /// </summary>
        private Ontologia mOntologia;

        /// <summary>
        /// Indica si la exportación se va a realizar a Metagnoss
        /// </summary>
        private bool mExportacionMetagnoss;

        /// <summary>
        /// Lista de dataset por cada entidad
        /// </summary>
        private Dictionary<string, DataSet> mListaDataSet;

        /// <summary>
        /// Lista de tipos de elementos que se permite exportar (null -> exporta todo)
        /// </summary>
        List<string> mTipoElementoExportar = null;

        /// <summary>
        /// Idioma del usuario
        /// </summary>
        string mIdiomaUsuario = "es";

        #endregion

        /// <summary>
        /// Lista de entidades ya obtenidas
        /// </summary>
        public List<ElementoOntologia> ListaEntidadesCreadas;

        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private UtilSemCms mUtilSemCms;
        protected VirtuosoAD mVirtuosoAd;
        protected IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private Microsoft.Extensions.Logging.ILogger mlogger;
        protected ILoggerFactory mloggerFactory;
        #endregion

        #region Constructor

        /// <summary>
        /// Crea un nuevo exportador de elemento gnoss
        /// </summary>
        /// <param name="pOntologia">Ontología</param>
        public ExportadorElementoGnoss(Ontologia pOntologia, string pIdiomaUsuario, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, UtilSemCms utilSemCms, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, VirtuosoAD virtuosoAd,Microsoft.Extensions.Logging.ILogger logger,ILoggerFactory loggerFactory)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;

            mRedisCacheWrapper = redisCacheWrapper;
            mUtilSemCms = utilSemCms;
            mVirtuosoAd = virtuosoAd;
            mlogger = logger;
            mloggerFactory = loggerFactory;
            this.Ontologia = pOntologia;
            mIdiomaUsuario = pIdiomaUsuario;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            if (ListaEntidadesCreadas == null)
            {
                ListaEntidadesCreadas = new List<ElementoOntologia>();
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el tipo de exportación (a metagnoss o en local)
        /// </summary>
        public string IdiomaUsuario
        {
            get
            {
                return this.mIdiomaUsuario;
            }
            set
            {
                this.mIdiomaUsuario = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el tipo de exportación (a metagnoss o en local)
        /// </summary>
        public List<string> TipoElementoExportar
        {
            get
            {
                return this.mTipoElementoExportar;
            }
            set
            {
                this.mTipoElementoExportar = value;
            }
        }
        /// <summary>
        /// Obtiene o establece el tipo de exportación (a metagnoss o en local)
        /// </summary>

        public bool ExportacionMetagnoss
        {
            get
            {
                return this.mExportacionMetagnoss;
            }
            set
            {
                this.mExportacionMetagnoss = value;
            }
        }

        /// <summary>
        /// Obtiene o establece la ontología
        /// </summary>
        public Ontologia Ontologia
        {
            get
            {
                return this.mOntologia;
            }
            set
            {
                this.mOntologia = value;
            }
        }

        /// <summary>
        /// Obtiene o establece la lista de datasets por cada entidad
        /// </summary>
        public Dictionary<string, DataSet> ListaDataSet
        {
            get
            {
                return mListaDataSet;
            }
            set
            {
                mListaDataSet = value;
            }
        }

        #endregion

        #region Métodos generales

        #region Públicos

        /// <summary>
        /// Obtiene la entidad y todas sus relaciones
        /// </summary>
        /// <param name="pEntidad">Entidad que se va a obtener</param>
        /// <param name="pElementoGnoss">Elemento Gnoss que representa a la entidad</param>
        /// <param name="pEspecializacion">Indica si la entidad será especilización de otra</param>
        /// <param name="pGestor">Gestor de entidades</param>
        public abstract void ObtenerEntidad(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, bool pEspecializacion, GestionGnoss pGestor);


        /// <summary>
        /// Obtiene la entidad pEntidad y todas sus relaciones.
        /// </summary>
        /// <param name="pEntidad">Entidad que se va a obtener.</param>
        /// <param name="pElementoGnoss">Elemento gnoss que representa la entidad</param>
        /// <param name="pFilaElemento">Fila del elemento gnoss que representa la entidad</param>
        /// <param name="pEspecializacion">Indica si la entidad será especilización de otra.</param>
        /// <param name="pGestor">Gestor de entidades</param>
        public void ObtenerEntidad(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, object pFilaElemento, bool pEspecializacion, GestionGnoss pGestor)
        {
            pEntidad.Elemento = pElementoGnoss;

            CompletarEntidad(pElementoGnoss, pGestor);

            pEntidad.EstaCompleta = true;

            //Obtenemos los atributos de la entidad
            ComprobarEspecializacion(pEntidad, pElementoGnoss, pGestor, EModosExportacion.AtributosYPropiedades);

            ObtenerAtributosEntidad(pEntidad, pElementoGnoss);

            //Obtenemos la lista de entidades relacionadas con la entidad
            ListaEntidadesCreadas.Add(pEntidad);
            List<Propiedad> entidades = pEntidad.ObtenerEntidadesRelacionadas();

            foreach (Propiedad propiedad in entidades)
            {
                propiedad.ElementoOntologia = pEntidad;
                ObtenerEntidadesRelacionadas(pEntidad, pElementoGnoss, propiedad, pGestor, false);
            }
            //Captar los atributos de la entidad superior
            GeneralizarEntidad(pEntidad, pElementoGnoss, pFilaElemento, pGestor);

            //Obtenemos el padre de la entidad
            ObtenerPadreEntidad(pEntidad, pElementoGnoss);
        }

        /// <summary>
        /// Obtiene la entidad pEntidad y todas sus relaciones.
        /// </summary>
        /// <param name="pEntidad">Entidad que se va a obtener.</param>
        /// <param name="pElementoGnoss">Elemento gnoss que representa la entidad</param>
        /// <param name="pFilaElemento">Fila del elemento gnoss que representa la entidad</param>
        /// <param name="pEspecializacion">Indica si la entidad será especilización de otra.</param>
        /// <param name="pGestor">Gestor de entidades</param>
        public void ObtenerEntidad(Es.Riam.Semantica.OWL.ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, AD.EntityModel.Models.PersonaDS.Persona pFilaElemento, bool pEspecializacion, GestionGnoss pGestor)
        {
            pEntidad.Elemento = pElementoGnoss;

            CompletarEntidad(pElementoGnoss, pGestor);

            pEntidad.EstaCompleta = true;

            //Obtenemos los atributos de la entidad
            ComprobarEspecializacion(pEntidad, pElementoGnoss, pGestor, EModosExportacion.AtributosYPropiedades);

            ObtenerAtributosEntidad(pEntidad, pElementoGnoss);

            //Obtenemos la lista de entidades relacionadas con la entidad
            ListaEntidadesCreadas.Add(pEntidad);
            List<Propiedad> entidades = pEntidad.ObtenerEntidadesRelacionadas();

            foreach (Propiedad propiedad in entidades)
            {
                propiedad.ElementoOntologia = pEntidad;
                ObtenerEntidadesRelacionadas(pEntidad, pElementoGnoss, propiedad, pGestor, false);
            }
            //Captar los atributos de la entidad superior
            GeneralizarEntidad(pEntidad, pElementoGnoss, pFilaElemento, pGestor);

            //Obtenemos el padre de la entidad
            ObtenerPadreEntidad(pEntidad, pElementoGnoss);
        }

        /// <summary>
        /// Obtiene la entidad pEntidad y todas sus relaciones.
        /// </summary>
        /// <param name="pEntidad">Entidad que se va a obtener.</param>
        /// <param name="pElementoGnoss">Elemento gnoss que representa la entidad</param>
        /// <param name="pFilaElemento">Fila del elemento gnoss que representa la entidad</param>
        /// <param name="pEspecializacion">Indica si la entidad será especilización de otra.</param>
        /// <param name="pGestor">Gestor de entidades</param>
        public void ObtenerEntidadProyecto(Es.Riam.Semantica.OWL.ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, AD.EntityModel.Models.ProyectoDS.Proyecto pFilaElemento, bool pEspecializacion, GestionGnoss pGestor)
        {
            pEntidad.Elemento = pElementoGnoss;

            CompletarEntidad(pElementoGnoss, pGestor);

            pEntidad.EstaCompleta = true;

            //Obtenemos los atributos de la entidad
            ComprobarEspecializacion(pEntidad, pElementoGnoss, pGestor, EModosExportacion.AtributosYPropiedades);

            ObtenerAtributosEntidad(pEntidad, pElementoGnoss);

            //Obtenemos la lista de entidades relacionadas con la entidad
            ListaEntidadesCreadas.Add(pEntidad);
            List<Propiedad> entidades = pEntidad.ObtenerEntidadesRelacionadas();

            foreach (Propiedad propiedad in entidades)
            {
                propiedad.ElementoOntologia = pEntidad;
                ObtenerEntidadesRelacionadas(pEntidad, pElementoGnoss, propiedad, pGestor, false);
            }
            //Captar los atributos de la entidad superior
            GeneralizarEntidad(pEntidad, pElementoGnoss, pFilaElemento, pGestor);

            //Obtenemos el padre de la entidad
            ObtenerPadreEntidad(pEntidad, pElementoGnoss);
        }

        /// <summary>
        /// Obtiene los atributos de la entidad relacionada con la entidad pasada por parámetro y crea la relación
        /// </summary>
        /// <param name="pEntidadBuscada">Entidad de la cual se buscan sus atributos</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad buscada</param>
        public virtual void ObtenerAtributosEntidad(ElementoOntologia pEntidadBuscada, IElementoGnoss pElementoGnoss)
        {
            pEntidadBuscada.Elemento = pElementoGnoss;
        }

        /// <summary>
        /// Obtiene los atributos de la entidad relacionada con la entidad pasada por parámetro y crea la relación
        /// </summary>
        /// <param name="pEntidadBuscada">Entidad de la cual se buscan sus atributos</param>
        /// <param name="pEntidad">Entidad que posee la propiedad</param>
        /// <param name="pFila">Fila que representa la entidad buscada</param>
        /// <param name="pPropiedad">Propiedad que vincula las entidades</param>
        public void ObtenerAtributosEntidad(ElementoOntologia pEntidadBuscada, Es.Riam.Semantica.OWL.ElementoOntologia pEntidad, DataRow pFila, Propiedad pPropiedad)
        {
            if ((!pEntidad.ID.Equals(pEntidadBuscada.ID)) && (!pEntidad.EntidadesRelacionadas.Contains(pEntidadBuscada)))
            {
                //Obtenemos los atributos
                UtilImportarExportar.ObtenerAtributosEntidad(pEntidadBuscada, pFila);

                //La relacionamos con la entidad
                pEntidad.EntidadesRelacionadas.Add(pEntidadBuscada);
            }
            pPropiedad.ListaValores.Add(pEntidadBuscada.ID, pEntidadBuscada);

            //Añadimos la entidad a la lista de creadas
            if (ComprobarEntidadIncluida(pEntidadBuscada.ID) == null)
                ListaEntidadesCreadas.Add(pEntidadBuscada);
        }

        /// <summary>
        /// Completa la entidad y todas sus entidades relacionadas
        /// </summary>
        /// <param name="pEntidadPrincipal">Entidad que se va a exportar</param>
        /// <param name="pGestor">Gestor de la entidad que se va a exportar</param>
        public void CompletarEntidad(IElementoGnoss pEntidadPrincipal, GestionGnoss pGestor)
        {
            Dictionary<Guid, AD.EntityModel.Models.PersonaDS.Persona> listaclaves = new Dictionary<Guid, AD.EntityModel.Models.PersonaDS.Persona>();
            List<IElementoGnoss> listaElementos = new List<IElementoGnoss>();

            ComprobarEntidadCompleta(pEntidadPrincipal, listaclaves, listaElementos);

            CompletarEntidades(listaElementos, listaclaves, pGestor);
        }

        /// <summary>
        /// Obtiene el elemento que está seleccionado actualmente en la pantalla
        /// </summary>
        /// <param name="pGestor">Gestor de entidades</param>
        /// <param name="pEntidadPrincipal">Entidad principal para obtener</param>
        /// <param name="pOntologia">Ontología</param>
        /// <param name="pExportador">Exportador de elementos Gnoss</param>
        /// <param name="pTipoEntidad">Tipo de entidad</param>
        /// <returns>Elemento Gnoss seleccionado</returns>
        public static IElementoGnoss ObtenerElementoSeleccionado(GestionGnoss pGestor, ref ElementoOntologia pEntidadPrincipal, Ontologia pOntologia, ExportadorElementoGnoss pExportador, string pTipoEntidad)
        {
            switch (pTipoEntidad)
            {
                case TipoElementoGnoss.Proyecto:
                    pEntidadPrincipal = new ElementoOntologiaGnoss(pOntologia.GetEntidadTipo(TipoElementoGnoss.Proyecto));
                    return pGestor.ElementoSeleccionado;
                case TipoElementoGnoss.Tesauro:
                    pEntidadPrincipal = new ElementoOntologiaGnoss(pOntologia.GetEntidadTipo(TipoElementoGnoss.Tesauro));
                    return pGestor.ElementoSeleccionado;
                case TipoElementoGnoss.CategoriasTesauro:
                    pEntidadPrincipal = new ElementoOntologiaGnoss(pOntologia.GetEntidadTipo(TipoElementoGnoss.CategoriasTesauro));
                    return pGestor.ElementoSeleccionado;
                case TipoElementoGnoss.Organizacion:
                    pEntidadPrincipal = new ElementoOntologiaGnoss(pOntologia.GetEntidadTipo(TipoElementoGnoss.Organizacion));
                    return pGestor.ElementoSeleccionado;
                case TipoElementoGnoss.ListaResultados:
                    pEntidadPrincipal = new ElementoOntologiaGnoss(pOntologia.GetEntidadTipo(TipoElementoGnoss.ListaResultados));
                    if (pGestor is GestionOrganizaciones)
                    {
                        pEntidadPrincipal.Descripcion = "Todas las organizaciones";
                    }
                    else if (pGestor is GestionPersonas)
                    {
                        pEntidadPrincipal.Descripcion = "Todas las personas";
                    }
                    return pGestor;
            }
            return null;
        }

        /// <summary>
        /// Carga el exportador correcto
        /// </summary>
        /// <param name="pTipoEntidad">Tipo de entidad</param>
        /// <param name="pOntologia">Ontologia</param>
        /// <param name="pGestor">Gestor de entidades</param>
        /// <returns>Exportador de elemento Gnoss</returns>
        public ExportadorElementoGnoss CargarExportador(string pTipoEntidad, Ontologia pOntologia, GestionGnoss pGestor, string pIdiomaUsuario)
        {
            switch (pTipoEntidad)
            {
                case TipoElementoGnoss.Proyecto:
                    return new ExportadorProyecto(pOntologia, pIdiomaUsuario, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mUtilSemCms, mServicesUtilVirtuosoAndReplication, mVirtuosoAd, mloggerFactory.CreateLogger<ExportadorProyecto>(), mloggerFactory);
                case TipoElementoGnoss.Organizacion:
                    return new ExportadorOrganizacion(pOntologia, pIdiomaUsuario, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mUtilSemCms, mServicesUtilVirtuosoAndReplication, mVirtuosoAd, mloggerFactory.CreateLogger<ExportadorOrganizacion>(), mloggerFactory);
                case TipoElementoGnoss.CategoriasTesauro:
                    return new ExportadorWiki(pOntologia, pIdiomaUsuario, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mUtilSemCms, mServicesUtilVirtuosoAndReplication, mVirtuosoAd,mloggerFactory.CreateLogger<ExportadorWiki>(), mloggerFactory);
                case TipoElementoGnoss.Tesauro:
                    return new ExportadorWiki(pOntologia, pIdiomaUsuario, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mUtilSemCms, mServicesUtilVirtuosoAndReplication, mVirtuosoAd, mloggerFactory.CreateLogger<ExportadorWiki>(), mloggerFactory);
                case TipoElementoGnoss.ListaResultados:
                    if (pGestor is GestionOrganizaciones)
                    {
                        return new ExportadorOrganizacion(pOntologia, pIdiomaUsuario, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mUtilSemCms, mServicesUtilVirtuosoAndReplication, mVirtuosoAd, mloggerFactory.CreateLogger<ExportadorOrganizacion>(), mloggerFactory);
                    }
                    else if (pGestor is GestionPersonas)
                    {
                        return new ExportadorPersona(pOntologia, pIdiomaUsuario, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mUtilSemCms, mServicesUtilVirtuosoAndReplication, mVirtuosoAd, mloggerFactory.CreateLogger<ExportadorPersona>(), mloggerFactory);
                    }
                    break;
            }
            return null;
        }

        #region Formularios semánticos

        /// <summary>
        /// Obtiene la ruta del fichero RDF de un documento semántico almacenado en la BD RDF. En caso de no estar en la bd lo obtendrá de virtuoso
        /// </summary>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pNamespaceOntologia">Namespace de la ontología</param>
        public string ObtenerRdfDeDocumento(Guid pDocumentoID, Guid pProyectoID, string pNamespaceOntologia)
        {
            string rdfText = string.Empty;
            RdfDS rdfDS;

            try
            {
                RdfCN rdfCN = new RdfCN("rdf", pDocumentoID.ToString().Substring(0, 3), mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mloggerFactory.CreateLogger<RdfCN>(), mloggerFactory);
                rdfDS = rdfCN.ObtenerRdfPorDocumentoID(pDocumentoID, pProyectoID);
                rdfCN.Dispose();
            }
            catch (Exception)
            {
                rdfDS = new RdfDS();
            }

            if (rdfDS.RdfDocumento.Count > 0)
            {
                rdfText = rdfDS.RdfDocumento[0].RdfSem;
            }

            if (string.IsNullOrEmpty(rdfText))
            {
                mLoggingService.AgregarEntrada("FormSem RDF NO está en sqlServer, vamos a virtuoso a por él");
                DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mloggerFactory.CreateLogger<DocumentacionCN>(), mloggerFactory);
                ParametroAplicacionCN parametroAplicacion = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mloggerFactory.CreateLogger<ParametroAplicacionCN>(), mloggerFactory);

                string nombreGrafo = documentacionCN.ObtenerElementoVinculadoDeDocumento(pDocumentoID).Enlace;
                string urlIntragnoss = parametroAplicacion.ObtenerParametroAplicacion(TiposParametrosAplicacion.UrlIntragnoss);
                if(!urlIntragnoss.EndsWith('/'))
                {
                    urlIntragnoss += '/';
                }
                
                string urlOntologia = $"{urlIntragnoss}Ontologia/{nombreGrafo}#";
                
                byte[] rdf = ObtenerRDFDeVirtuoso(pDocumentoID.ToString(), nombreGrafo, urlIntragnoss, urlOntologia, pNamespaceOntologia, Ontologia, true);

                MemoryStream buffer = new MemoryStream(rdf);
                StreamReader reader = new StreamReader(buffer);
                rdfText = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();
            }

            rdfDS.Dispose();

            if (!string.IsNullOrEmpty(pNamespaceOntologia))
            {
                rdfText = rdfText.Replace(GestionOWL.NAMESPACE_ONTO_GNOSS + ":", pNamespaceOntologia + ":").Replace(GestionOWL.NAMESPACE_ONTO_GNOSS + "=", pNamespaceOntologia + "=");
            }

            return rdfText;
        }

        /// <summary>
        /// Obtiene los bytes del fichero RDF de un documento semántico almacenado en virtuoso.
        /// </summary>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <param name="pNombreGrafo">Nombre del grafo en virtuoso</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        /// <param name="pUrlOntologia">URL de la ontología</param>
        /// <param name="pNamespaceOntologia">Namespace de la ontología</param>
        /// <param name="pOntologia">Ontología del documento</param>
        public byte[] ObtenerRDFDeVirtuoso(string pDocumentoID, string pNombreGrafo, string pUrlIntragnoss, string pUrlOntologia, string pNamespaceOntologia, Ontologia pOntologia, bool pTraerEntidadesExternas, bool pUsarAfinidad = false)
        {
            FacetadoCN facetadoCN = null;
            facetadoCN = new FacetadoCN(pUrlIntragnoss, "", mEntityContext, mLoggingService, mConfigService, mVirtuosoAd, mServicesUtilVirtuosoAndReplication, mloggerFactory.CreateLogger<FacetadoCN>(), mloggerFactory);

            FacetadoDS facetadoDS = facetadoCN.ObtenerRDFXMLdeFormulario(pNombreGrafo.ToLower(), pDocumentoID, pUsarAfinidad);

            if (facetadoDS.Tables[0].Rows.Count == 0)
            {
                return Array.Empty<byte>();
            }

            List<FacetaEntidadesExternas> EntExt = null;
            if (pTraerEntidadesExternas)
            {
                //Obtenemos el Proyecto al que pertenece el documento
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mloggerFactory.CreateLogger<DocumentacionCN>(), mloggerFactory);
                Guid proyID = docCN.ObtenerProyectoIDPorDocumentoID(new Guid(pDocumentoID));

                DataWrapperFacetas facetasDW = new DataWrapperFacetas();
                FacetaCN facCN = new FacetaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mloggerFactory.CreateLogger<FacetaCN>(), mloggerFactory);
                facCN.CargarFacetasEntidadesExternas(ProyectoAD.MetaOrganizacion, proyID, facetasDW);
                EntExt = facetasDW.ListaFacetaEntidadesExternas.Where(item => item.ProyectoID.Equals(proyID)).ToList();

            }
            List<string> listaEntidadesExternas = new List<string>();

            MemoryStore store = new MemoryStore();
            //foreach (DataRow fila in facetadoDS.Tables[0].Rows)

            string[] delimiter = { "/@/" };
            string type = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type";
            string label = "http://www.w3.org/2000/01/rdf-schema#label";
            Dictionary<string, string> dicSujetosTypeLabel = ObtenerDiccionarioSujetosTypeLabel(facetadoDS.Tables[0], delimiter, type, label);

            foreach (DataRow fila in facetadoDS.Tables[0].Select("", "s"))
            {
                AgregarDatosAlStore(fila, store, dicSujetosTypeLabel, delimiter, type, label);

                //Obtener las entidades externas del proyecto
                if (EntExt != null && Uri.IsWellFormedUriString(fila["o"].ToString(), UriKind.Absolute) && !listaEntidadesExternas.Contains(fila["o"].ToString()) && facetadoDS.Tables[0].Select("s = '" + fila["o"].ToString().Replace("'", "''") + "'").Length == 0)
                {
                    listaEntidadesExternas.Add(fila["o"].ToString());
                }
            }

            if (pTraerEntidadesExternas)
            {
                FacetadoDS facDS = new FacetadoDS();
                foreach (string entidadExterna in listaEntidadesExternas)
                {
                    //Por cada entidad externa del diccionario, debemos obtener sus triples de virtuoso y añadirlos al store.
                    if (EntExt != null)
                    {
                        for (int i = 0; i < EntExt.Count; i++)
                        {
                            if (entidadExterna.ToLower().Contains(EntExt[i].EntidadID.ToLower()))
                            {
                                //Cargamos el DS
                                facDS.Merge(facetadoCN.ObtieneTripletasOtrasEntidadesDS(entidadExterna, EntExt[i].Grafo, EntExt));
                            }
                        }
                    }
                }

                //Agrupar por sujeto
                //Por cada bloque, agregar al store primero el type y después el label
                //Rezar para que el store no desordene las triples....

                Dictionary<string, string> dicSujetosTypeLabelEntidades = ObtenerDiccionarioSujetosTypeLabel(facDS.Tables["OtrasEntidades"], delimiter, type, label);
                foreach (DataRow fila in facDS.Tables["OtrasEntidades"].Select("", "s"))
                {
                    AgregarDatosAlStore(fila, store, dicSujetosTypeLabelEntidades, delimiter, type, label);
                }
            }

            facetadoDS.Dispose();

            MemoryStream archivo = new MemoryStream();
            System.Xml.XmlWriter textWriter = System.Xml.XmlWriter.Create(archivo);

            RdfWriter writer = new RdfXmlWriter(textWriter);
            writer.Namespaces.AddNamespace("http://www.w3.org/1999/02/22-rdf-syntax-ns#", "rdf");
            writer.Namespaces.AddNamespace("http://www.gnoss.net/ontologia.owl#", "gnoss");
            writer.Namespaces.AddNamespace("http://www.w3.org/2001/XMLSchema#", "xsd");
            writer.Namespaces.AddNamespace("http://www.w3.org/2000/01/rdf-schema#", "rdfs");
            writer.Namespaces.AddNamespace("http://www.w3.org/2002/07/owl#", "owl");
            writer.Namespaces.AddNamespace(pUrlOntologia, pNamespaceOntologia);

            if (pOntologia != null)
            {
                foreach (string ns in pOntologia.NamespacesDefinidos.Keys)
                {
                    if (pOntologia.NamespacesDefinidos[ns] != "rdf" && pOntologia.NamespacesDefinidos[ns] != "gnoss" && pOntologia.NamespacesDefinidos[ns] != "xsd" && pOntologia.NamespacesDefinidos[ns] != "rdfs" && pOntologia.NamespacesDefinidos[ns] != "owl" && pOntologia.NamespacesDefinidos[ns] != pNamespaceOntologia)
                    {
                        writer.Namespaces.AddNamespace(ns, pOntologia.NamespacesDefinidos[ns]);
                    }
                }
            }

            writer.Write(store);
            writer.Close();
            store.Dispose();

            return archivo.ToArray();
        }

        private static void AgregarDatosAlStore(DataRow pFila, MemoryStore pStore, Dictionary<string, string> pDicSujetosTypeLabel, string[] pDelimiter, string pType, string pLabel)
        {
            string sujeto = (string)pFila[0];
            if (pDicSujetosTypeLabel.ContainsKey(sujeto) && !string.IsNullOrEmpty(pDicSujetosTypeLabel[sujeto]))
            {
                string[] delimiter = { "|" };

                string[] typeYLabel = pDicSujetosTypeLabel[sujeto].Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                foreach (string triples in typeYLabel)
                {
                    string[] predYObjt = triples.Split(pDelimiter, StringSplitOptions.RemoveEmptyEntries);
                    string pred = predYObjt[0];
                    string objt = predYObjt[1];
                    //Agregamos al store 
                    pStore.Add(new Statement(new Entity(sujeto), new Entity(pred), new SemWeb.Literal(objt, null, null)));
                }

                //Borramos para no agregarlo en la siguiente llamada a este método.
                pDicSujetosTypeLabel[sujeto] = "";
            }

            string idioma = null;

            if (pFila.ItemArray.Length > 3 && !pFila.IsNull(3) && !string.IsNullOrEmpty((string)pFila[3]))
            {
                idioma = (string)pFila[3];
            }

            if (pDicSujetosTypeLabel.ContainsKey(sujeto) && !((string)pFila[1] == pType || (string)pFila[1] == pLabel))
            {
                string objeto = "";
                if (!pFila.IsNull(2))
                {
                    objeto = (string)pFila[2];
                }

                pStore.Add(new Statement(new Entity(sujeto), new Entity((string)pFila[1]), new SemWeb.Literal(objeto, idioma, null)));
            }
        }

        private static Dictionary<string, string> ObtenerDiccionarioSujetosTypeLabel(DataTable pDataTable, string[] pDelimiter, string pType, string pLabel)
        {
            Dictionary<string, string> dicSujetosTypeLabel = new Dictionary<string, string>();

            foreach (DataRow fila in pDataTable.Select("", "s"))
            {
                string sujeto = (string)fila[0];
                string predicado = (string)fila[1];
                string objeto = "";

                if (!fila.IsNull(2))
                {
                    objeto = (string)fila[2];
                }

                string objetoYPredicado = predicado + pDelimiter[0] + objeto;

                if (predicado.Equals(pType) || predicado.Equals(pLabel))
                {
                    if (dicSujetosTypeLabel.ContainsKey(sujeto))
                    {
                        if (dicSujetosTypeLabel[sujeto].Contains(pType))
                        {
                            dicSujetosTypeLabel[sujeto] += "|" + objetoYPredicado;
                        }
                        else if (dicSujetosTypeLabel[sujeto].Contains(pLabel))
                        {
                            dicSujetosTypeLabel[sujeto] = objetoYPredicado + "|" + dicSujetosTypeLabel[sujeto];
                        }
                    }
                    else
                    {
                        dicSujetosTypeLabel.Add(sujeto, objetoYPredicado);
                    }
                }
            }

            return dicSujetosTypeLabel;
        }       

        #endregion

        #endregion

        #region Protegidos

        /// <summary>
        /// Comprueba se es necesario completar una entidad
        /// </summary>
        /// <param name="pEntidad">Entidad a completar</param>
        /// <param name="pListaACompletar">Lista de entidades a completar</param>
        /// <param name="pListaElementos">Lista de elementos completos</param>
        protected virtual void ComprobarEntidadCompleta(IElementoGnoss pEntidad, Dictionary<Guid, AD.EntityModel.Models.PersonaDS.Persona> pListaACompletar, List<IElementoGnoss> pListaElementos)
        {
        }

        /// <summary>
        /// Completa una lista de entidades
        /// </summary>
        /// <param name="pListaACompletar">Lista de entidades a completar</param>
        /// <param name="pListaClaves">Lista de claves de elementos a completar</param>
        /// <param name="pGestor">Gestor GNOSS</param>
        protected virtual void CompletarEntidades(List<IElementoGnoss> pListaACompletar, Dictionary<Guid, AD.EntityModel.Models.PersonaDS.Persona> pListaClaves, GestionGnoss pGestor)
        {
        }

        /// <summary>
        /// Comprueba si la entidad está especializada por medio de alguna otra entidad
        /// Si lo está, se remplaza la entidad por la especializada con todas sus propiedades
        /// </summary>
        /// <param name="pEntidad">Entidad de la que se desea comprobar su especialización.</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad</param>
        /// <param name="pGestor">Gestor de entidades</param>
        /// <param name="pModoExportacion">Modo de exportación</param>
        /// <returns>TRUE si la entidad está especializada, FALSE en caso contrario</returns>
        protected virtual bool ComprobarEspecializacion(Es.Riam.Semantica.OWL.ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, GestionGnoss pGestor, EModosExportacion pModoExportacion)
        {
            bool resultado = false;

            foreach (string subclase in pEntidad.Subclases)
            {
                if ((pElementoGnoss != null) && (pElementoGnoss.GetType().Name.Equals(subclase)))
                {
                    //Especializamos la entidad
                    EspecializarEntidad(pEntidad, pElementoGnoss, pGestor, subclase, pModoExportacion);
                    break;
                }
            }
            return resultado;
        }

        /// <summary>
        /// Especializa una entidad en una de sus subclases
        /// </summary>
        /// <param name="pEntidad">Entidad a especializar</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad</param>
        /// <param name="pGestor">Gestor de la pantalla</param>
        /// <param name="pSubclase">Subclase en la que hay que especializar al entidad</param>
        /// <param name="pModoExportacion">Modo en el que se ha de exportar la entidad</param>
        protected void EspecializarEntidad(Es.Riam.Semantica.OWL.ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, GestionGnoss pGestor, string pSubclase, EModosExportacion pModoExportacion)
        {
            Es.Riam.Semantica.OWL.ElementoOntologia entidad = new ElementoOntologiaGnoss(this.Ontologia.GetEntidadTipo(pSubclase));

            //Copiamos las propiedades especializadas
            CopiarPropiedadesEspecializadas(pEntidad, entidad);

            //Comprobamos si se puede especializar un nivel más
            ComprobarEspecializacion(entidad, pElementoGnoss, pGestor, pModoExportacion);

            //Clonamos la entidad especializada
            pEntidad.ClonarEntidad(entidad);
        }

        /// <summary>
        /// Copia todas las propiedades, el identificador y la descripción de una entidad en otra que la especializa
        /// </summary>
        /// <param name="pSuperclase">Entidad padre.</param>
        /// <param name="pSubclase">Entidad especializada.</param>
        protected void CopiarPropiedadesEspecializadas(Es.Riam.Semantica.OWL.ElementoOntologia pSuperclase, Es.Riam.Semantica.OWL.ElementoOntologia pSubclase)
        {
            //Añade a la entidad especializada las propiedades del padre
            List<Propiedad> atributos = pSuperclase.Propiedades;

            //Recorremos los atributos
            foreach (Propiedad propiedad in atributos)
            {
                int contador = 0;

                while (!propiedad.Nombre.Equals(pSubclase.Propiedades[contador].Nombre))
                {
                    contador++;
                }

                if (pSubclase.Propiedades[contador].Heredada)
                {
                    //Copiamos los valores de la propiedad
                    UtilImportarExportar.ObtenerNombreRealPropiedad(pSubclase, pSuperclase, propiedad);
                    pSubclase.Propiedades[contador].NombreReal = propiedad.NombreReal;

                    foreach (string valor in propiedad.ListaValores.Keys)
                    {
                        pSubclase.Propiedades[contador].ListaValores.Add(valor, propiedad.ListaValores[valor]);
                    }

                    if (propiedad.UnicoValor.Key != null)
                    {
                        pSubclase.Propiedades[contador].UnicoValor = propiedad.UnicoValor;
                    }

                    /*Es.Riam.Gnoss.Win.Principal.Util.Impresion.OpcionImpresion opcion = UtilImportarExportar.PropiedadToOpcionImpresion(propiedad.Nombre);
                    propiedad.OpcionImpresion = opcion;
                    if (opcion != null)
                    {
                        entidad.ListaOpcionesImpresion.Add(opcion);

                        if ((opcion.OpcionMetagnoss) && (propiedad.Tipo.Equals(TipoPropiedad.ObjectProperty)))
                        {
                            entidad.ListaPropiedadesImprimibles.Add(propiedad);
                        }
                    }*/
                }
            }
            //Copiamos el identificador
            if (pSubclase.ID == "")
                pSubclase.ID = pSuperclase.ID;

            //Copiamos la descripción si ésta todavía no ha sido asignada
            if (!pSuperclase.Descripcion.Equals(pSuperclase.TipoEntidad))
                pSubclase.Descripcion = pSuperclase.Descripcion;

            //Copiamos las entidades relacionadas
            foreach (Es.Riam.Semantica.OWL.ElementoOntologia entidad in pSuperclase.EntidadesRelacionadas)
            {
                pSubclase.EntidadesRelacionadas.Add(entidad);
            }
        }

        /// <summary>
        /// Trata un caso especial de relación
        /// </summary>
        /// <param name="pEntidad">Entidad que posee la relación</param>
        /// <param name="pElementoGnoss">Fila que representa a la entidad</param>
        /// <param name="pPropiedad">Propiedad que representa a la relación</param>
        /// <param name="pGestor">Gestor de entidades</param>
        /// <returns></returns>
        protected abstract bool TratarCasoEspecial(Es.Riam.Semantica.OWL.ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor);

        protected abstract void GeneralizarEntidad(Es.Riam.Semantica.OWL.ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, object pFilaElemento, GestionGnoss pGestor);

        /// <summary>
        /// Generaliza la entidad pasada por parámetro en caso de que tenga entidad superior
        /// </summary>
        /// <param name="pEntidad">Entidad para generalizar</param>
        /// <param name="pElementoGnoss">Elemento que representa a la entidad</param>
        /// <param name="pGestor">Gestor de entidades</param>
        /// <param name="pFilaPadre">Fila que representa al padre</param>
        protected virtual void GeneralizarEntidad(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, GestionGnoss pGestor, DataRow pFilaPadre)
        {
            foreach (string superClase in pEntidad.Superclases)
            {
                if (superClase != "" && superClase != TipoElementoGnoss.EntidadNoExportable && pFilaPadre != null)
                {
                    ElementoOntologia padre = new ElementoOntologiaGnoss(this.Ontologia.GetEntidadTipo(superClase));

                    //Obtenemos el padre
                    UtilImportarExportar.ObtenerID(padre, pFilaPadre, pElementoGnoss);
                    UtilImportarExportar.ObtenerAtributosEntidad(padre, pFilaPadre);

                    //Generalizamos si es posible al padre
                    GeneralizarEntidad(padre, pElementoGnoss, pFilaPadre, pGestor);
                    List<Propiedad> entidades = padre.ObtenerEntidadesRelacionadas();

                    foreach (Propiedad propiedad in entidades)
                    {
                        ObtenerEntidadesRelacionadas(padre, pElementoGnoss, propiedad, pGestor, true);
                    }
                    //Copiamos sus propiedades al hijo
                    CopiarPropiedadesEspecializadas(padre, pEntidad);
                }
            }
        }

        /// <summary>
        /// Generaliza la entidad pasada por parámetro en caso de que tenga entidad superior
        /// </summary>
        /// <param name="pEntidad">Entidad para generalizar</param>
        /// <param name="pElementoGnoss">Elemento que representa a la entidad</param>
        /// <param name="pGestor">Gestor de entidades</param>
        /// <param name="pFilaPadre">Fila que representa al padre</param>
        protected virtual void GeneralizarEntidad(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, GestionGnoss pGestor, object pFilaPadre)
        {
            foreach (string superClase in pEntidad.Superclases)
            {
                if (superClase != "" && superClase != TipoElementoGnoss.EntidadNoExportable && pFilaPadre != null)
                {
                    ElementoOntologia padre = new ElementoOntologiaGnoss(this.Ontologia.GetEntidadTipo(superClase));

                    //Obtenemos el padre
                    UtilImportarExportar.ObtenerID(padre, pFilaPadre, pElementoGnoss);
                    UtilImportarExportar.ObtenerAtributosEntidad(padre, pFilaPadre);

                    //Generalizamos si es posible al padre
                    GeneralizarEntidad(padre, pElementoGnoss, pFilaPadre, pGestor);
                    List<Propiedad> entidades = padre.ObtenerEntidadesRelacionadas();

                    foreach (Propiedad propiedad in entidades)
                    {
                        ObtenerEntidadesRelacionadas(padre, pElementoGnoss, propiedad, pGestor, true);
                    }
                    //Copiamos sus propiedades al hijo
                    CopiarPropiedadesEspecializadas(padre, pEntidad);
                }
            }
        }


        /// <summary>
        /// Generaliza una entidad en caso de que tenga entidad superior
        /// </summary>
        /// <param name="pEntidad">Entidad a generalizar</param>
        /// <param name="fila">Fila que representa la entidad</param>
        protected void GeneralizarEntidad(Es.Riam.Semantica.OWL.ElementoOntologia pEntidad, object pFila)
        {
            DataRow fila = pFila as DataRow;
            if (fila != null)
            {
                foreach (string superClase in pEntidad.Superclases)
                {
                    if (superClase != "" && superClase != TipoElementoGnoss.EntidadNoExportable && superClase != TipoElementoGnoss.EntidadExportable && pEntidad.Generalizacion != null)
                    {
                        Es.Riam.Semantica.OWL.ElementoOntologia padre = new ElementoOntologiaGnoss(this.Ontologia.GetEntidadTipo(superClase));

                        //Obtenemos el padre
                        DataRow filaPadre = null;
                        if (fila != null)
                        {
                            filaPadre = fila.GetParentRow(pEntidad.Generalizacion);

                            UtilImportarExportar.ObtenerID(padre, filaPadre, null);
                        }
                        UtilImportarExportar.ObtenerAtributosEntidad(padre, filaPadre);
                        GeneralizarEntidad(padre, filaPadre);

                        //Copiamos sus propiedades al hijo
                        CopiarPropiedadesEspecializadas(padre, pEntidad);
                    }
                }
            }
        }


        /// <summary>
        /// Agrega una entidad relacionada a la entidad
        /// </summary>
        /// <param name="pEntidad">Entidad que posee la relación</param>
        /// <param name="pPropiedad">Propiedad que vincula las entidades</param>
        /// <param name="pEntidadRelacionada">Entidad que se va a relacionar</param>
        /// <param name="pElemento">Elemento que representa la entidad relacionada</param>
        /// <param name="pFilaElemento">Fila del elemento que representa la entidad relacionada</param>
        /// <param name="pEspecializacion">TRUE si se debe especializar el elemento</param>
        /// <param name="pGestor">Gestor de entidades</param>
        /// <returns>Entidad obtenida</returns>
        protected Es.Riam.Semantica.OWL.ElementoOntologia AgregarYObtenerEntidadRelacionada(Es.Riam.Semantica.OWL.ElementoOntologia pEntidad, Propiedad pPropiedad, Es.Riam.Semantica.OWL.ElementoOntologia pEntidadRelacionada, IElementoGnoss pElemento, DataRow pFilaElemento, bool pEspecializacion, GestionGnoss pGestor)
        {
            Es.Riam.Semantica.OWL.ElementoOntologia entidadAuxiliar = ComprobarEntidadIncluida(pEntidadRelacionada.ID);

            if ((entidadAuxiliar == null) || (!entidadAuxiliar.EstaCompleta))
            {
                if (entidadAuxiliar != null)
                    pEntidadRelacionada = entidadAuxiliar;

                //Obtenemos la entidad
                AgregarEntidadRelacionada(pEntidad, pPropiedad, pEntidadRelacionada);
                if (pElemento != null)
                    ObtenerEntidad(pEntidadRelacionada, pElemento, pEspecializacion, pGestor);
            }
            else
            {
                //Asignamos la entidad ya creada
                pEntidadRelacionada = entidadAuxiliar;
                AgregarEntidadRelacionada(pEntidad, pPropiedad, entidadAuxiliar);
            }
            return pEntidadRelacionada;
        }

        /// <summary>
        /// Agrega una entidad relacionada a la entidad
        /// </summary>
        /// <param name="pEntidad">Entidad que posee la relación</param>
        /// <param name="pPropiedad">Propiedad que vincula las entidades</param>
        /// <param name="pEntidadRelacionada">Entidad que se va a relacionar</param>
        /// <param name="pElemento">Elemento que representa la entidad relacionada</param>
        /// <param name="pFilaElemento">Fila del elemento que representa la entidad relacionada</param>
        /// <param name="pEspecializacion">TRUE si se debe especializar el elemento</param>
        /// <param name="pGestor">Gestor de entidades</param>
        /// <returns>Entidad obtenida</returns>
        protected Es.Riam.Semantica.OWL.ElementoOntologia AgregarYObtenerEntidadRelacionada(Es.Riam.Semantica.OWL.ElementoOntologia pEntidad, Propiedad pPropiedad, Es.Riam.Semantica.OWL.ElementoOntologia pEntidadRelacionada, IElementoGnoss pElemento, object pFilaElemento, bool pEspecializacion, GestionGnoss pGestor)
        {
            Es.Riam.Semantica.OWL.ElementoOntologia entidadAuxiliar = ComprobarEntidadIncluida(pEntidadRelacionada.ID);

            if ((entidadAuxiliar == null) || (!entidadAuxiliar.EstaCompleta))
            {
                if (entidadAuxiliar != null)
                    pEntidadRelacionada = entidadAuxiliar;

                //Obtenemos la entidad
                AgregarEntidadRelacionada(pEntidad, pPropiedad, pEntidadRelacionada);
                if (pElemento != null)
                    ObtenerEntidad(pEntidadRelacionada, pElemento, pEspecializacion, pGestor);
            }
            else
            {
                //Asignamos la entidad ya creada
                pEntidadRelacionada = entidadAuxiliar;
                AgregarEntidadRelacionada(pEntidad, pPropiedad, entidadAuxiliar);
            }
            return pEntidadRelacionada;
        }

        /// <summary>
        /// Agrega una entidad relacionada a la entidad
        /// </summary>
        /// <param name="pEntidad">Entidad que posee la relación</param>
        /// <param name="pPropiedad">Propiedad que vincula las entidades</param>
        /// <param name="pEntidadRelacionada">Entidad que se va a relacionar</param>
        /// <param name="pElemento">Elemento que representa la entidad relacionada</param>
        /// <param name="pFilaElemento">Fila del elemento que representa la entidad relacionada</param>
        /// <param name="pEspecializacion">TRUE si se debe especializar el elemento</param>
        /// <param name="pGestor">Gestor de entidades</param>
        /// <returns>Entidad obtenida</returns>
        protected Es.Riam.Semantica.OWL.ElementoOntologia AgregarYObtenerEntidadRelacionadaProyecto(Es.Riam.Semantica.OWL.ElementoOntologia pEntidad, Propiedad pPropiedad, Es.Riam.Semantica.OWL.ElementoOntologia pEntidadRelacionada, IElementoGnoss pElemento, AD.EntityModel.Models.ProyectoDS.Proyecto pFilaElemento, bool pEspecializacion, GestionGnoss pGestor)
        {
            Es.Riam.Semantica.OWL.ElementoOntologia entidadAuxiliar = ComprobarEntidadIncluida(pEntidadRelacionada.ID);

            if ((entidadAuxiliar == null) || (!entidadAuxiliar.EstaCompleta))
            {
                if (entidadAuxiliar != null)
                    pEntidadRelacionada = entidadAuxiliar;

                //Obtenemos la entidad
                AgregarEntidadRelacionada(pEntidad, pPropiedad, pEntidadRelacionada);
                if (pElemento != null)
                    ObtenerEntidad(pEntidadRelacionada, pElemento, pEspecializacion, pGestor);
            }
            else
            {
                //Asignamos la entidad ya creada
                pEntidadRelacionada = entidadAuxiliar;
                AgregarEntidadRelacionada(pEntidad, pPropiedad, entidadAuxiliar);
            }
            return pEntidadRelacionada;
        }

        /// <summary>
        /// Agrega una entidad relacionada a la entidad
        /// </summary>
        /// <param name="pEntidad">Entidad que posee la relación</param>
        /// <param name="pPropiedad">Propiedad que vincula las entidades</param>
        /// <param name="pEntidadRelacionada">Entidad que se va a relacionar</param>
        protected void AgregarEntidadRelacionada(Es.Riam.Semantica.OWL.ElementoOntologia pEntidad, Propiedad pPropiedad, Es.Riam.Semantica.OWL.ElementoOntologia pEntidadRelacionada)
        {
            pEntidad.EntidadesRelacionadas.Add(pEntidadRelacionada);

            if (!pPropiedad.FunctionalProperty)
            {
                if (!string.IsNullOrEmpty(pEntidadRelacionada.ID) && !pPropiedad.ListaValores.ContainsKey(pEntidadRelacionada.ID))
                {
                    pPropiedad.ListaValores.Add(pEntidadRelacionada.ID, pEntidadRelacionada);
                }
            }
            else
                pPropiedad.UnicoValor = new KeyValuePair<string, Es.Riam.Semantica.OWL.ElementoOntologia>(pEntidadRelacionada.ID, pEntidadRelacionada);
        }

        /// <summary>
        /// Comprueba si la entidad ya ha sido creada
        /// </summary>
        /// <param name="pGuid">Identificador de la entidad</param>
        /// <returns>Elemento de la ontología</returns>
        protected Es.Riam.Semantica.OWL.ElementoOntologia ComprobarEntidadIncluida(string pGuid)
        {
            foreach (Es.Riam.Semantica.OWL.ElementoOntologia entidad in ListaEntidadesCreadas)
            {
                if (entidad.ID.Equals(pGuid))
                    return entidad;
            }
            return null;
        }

        /// <summary>
        /// Obtiene una colección ordenada de elementos
        /// </summary>
        /// <param name="pEntidad">Entidad padre de la lista ordenada</param>
        /// <param name="pElementoGnoss">Elemento padre de la lista</param>
        /// <param name="pPropiedad">Propiedad </param>
        /// <param name="pGestor">Gestor de las entidades</param>
        /// <param name="pTipoElementoEnLista">Tipo de elemento de los hijos</param>
        /// <param name="pListaElementos">Lista de elementos hijos</param>
        protected void ObtenerColeccionOrdenada(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor, string pTipoElementoEnLista, List<ElementoGnoss> pListaElementos)
        {
            if (pListaElementos.Count > 0)
            {
                if (pListaElementos.Count > 1)
                {
                    string cadenaLista = "_lista";
                    int numLista = 0;

                    //Creo el elemento orderedCollection
                    ElementoOntologia listaSuperior = new ElementoOntologiaGnoss(Ontologia.GetEntidadTipo(TipoElementoGnoss.OrderedCollection));
                    listaSuperior.ID = pEntidad.ID + cadenaLista + numLista++;

                    AgregarEntidadRelacionada(pEntidad, pPropiedad, listaSuperior);

                    //Busco la propiedad MemberList para empezar a crear las listas de elementos
                    Propiedad propLista = UtilImportarExportar.ObtenerPropiedadDeNombre(UtilImportarExportar.PROPIEDAD_MEMBER_LIST, listaSuperior.Propiedades);

                    foreach (ElementoGnoss elemento in pListaElementos)
                    {
                        ElementoOntologia lista = new ElementoOntologiaGnoss(Ontologia.GetEntidadTipo(TipoElementoGnoss.List));
                        lista.ID = pEntidad.ID + cadenaLista + numLista++;
                        AgregarEntidadRelacionada(listaSuperior, propLista, lista);
                        Propiedad propFirst = UtilImportarExportar.ObtenerPropiedadDeNombre(UtilImportarExportar.PROPIEDAD_FIRST, lista.Propiedades);

                        ElementoOntologia elemHijo = new ElementoOntologiaGnoss(Ontologia.GetEntidadTipo(pTipoElementoEnLista));
                        UtilImportarExportar.ObtenerID(elemHijo, elemento.FilaElemento, elemento);
                        AgregarYObtenerEntidadRelacionada(lista, propFirst, elemHijo, elemento, elemento.FilaElemento, true, pGestor);

                        propLista = UtilImportarExportar.ObtenerPropiedadDeNombre(UtilImportarExportar.PROPIEDAD_REST, lista.Propiedades);
                        listaSuperior = lista;
                    }

                    propLista.UnicoValor = new KeyValuePair<string, ElementoOntologia>("rdf:nil", null);

                }
                else
                {
                    ElementoGnoss elemento = pListaElementos[0];
                    ElementoOntologia elemHijo = new ElementoOntologiaGnoss(Ontologia.GetEntidadTipo(pTipoElementoEnLista));
                    UtilImportarExportar.ObtenerID(elemHijo, elemento.FilaElemento, elemento);

                    AgregarYObtenerEntidadRelacionada(pEntidad, pPropiedad, elemHijo, elemento, elemento.FilaElemento, true, pGestor);
                }
            }
        }

        /// <summary>
        /// Agrega a la propiedad Tags de una entidad todos los tags de esta.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad</param>
        /// <param name="pPropiedad">Propiedad Tag</param>
        /// <param name="pGestor">Gestor del elemento</param>
        /// <param name="pListaTags">Lista de tags</param>
        protected void AgregarRelacionTagsEntidad(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor, List<string> pListaTags)
        {
            //Obtenemos las entidades en freebase que hacen referencia a estos tags
            //GnossFreebaseDS pDataSet = new GnossFreebaseDS();
            //ProyectoCN proyCN = new ProyectoCN();
            //BaseComunidadCN baseComunidadCN = new BaseComunidadCN("base", proyCN.ObtenerTablaBaseProyectoIDProyectoPorID(Usuario.UsuarioActual.ProyectoID));
            //proyCN.Dispose();
            //baseComunidadCN.ObtenerEntidadFreebase(pDataSet, pListaTags);

            //pEntidad.Elemento = pElementoGnoss;

            //foreach (string tag in pListaTags)
            //{
            //    ElementoOntologia entidadTag = new ElementoOntologiaGnoss(Ontologia.GetEntidadTipo(TipoElementoGnoss.TagSioc));
            //    entidadTag.EstablecerID(tag);
            //    if (entidadTag.ID.Contains("+"))
            //    {
            //        entidadTag.ID = entidadTag.ID.Replace("+", "%20");
            //    }
            //    entidadTag.Descripcion = tag;
            //    ElementoOntologia tagAuxiliar = ComprobarEntidadIncluida(entidadTag.ID);

            //    //Para cada entidad equivalente en freebase, ponemos una etiqueta owl:sameAs en la entidad
            //    DataRow[] myRows = pDataSet.GnossToFreebase.Select("Tag='" + tag.Replace("'", "''") + "'");
            //    if (myRows.Length > 0) 
            //    {
            //        foreach (GnossFreebaseDS.GnossToFreebaseRow MyRow in myRows)
            //        {
            //            if (!string.IsNullOrEmpty(MyRow.Ruta))
            //            { 
            //                string rutaRDF = baseComunidadCN.GeneraFreeBaseURLParaRDF(MyRow.GUIDFreebase);
            //                entidadTag.OWLSameAs.Add(rutaRDF);
            //            }
            //            if (!string.IsNullOrEmpty(MyRow.RutaNYT))
            //            {
            //                string rutaRDF = baseComunidadCN.GeneraNYTUrlParaRDF(MyRow.GUIDFreebase);
            //                entidadTag.OWLSameAs.Add(rutaRDF);
            //            }
            //            if (!string.IsNullOrEmpty(MyRow.RutaGeonames))
            //            {
            //                string rutaRDF = baseComunidadCN.GeneraGeonamesUrlParaRDF(MyRow.RutaGeonames);
            //                entidadTag.OWLSameAs.Add(rutaRDF);
            //            }
            //            if (!string.IsNullOrEmpty(MyRow.RutaDbpedia))
            //            {
            //                entidadTag.OWLSameAs.Add(MyRow.RutaDbpedia);
            //            }
            //        }
            //    }
            //    if (tagAuxiliar == null)
            //    {
            //        //Obtengo la entidad
            //        AgregarEntidadRelacionada(pEntidad, pPropiedad, entidadTag);
            //        ObtenerEntidad(entidadTag, null, true, pGestor);
            //    }
            //    else
            //    {
            //        //asigno la entidad ya creada
            //        entidadTag = tagAuxiliar;
            //        AgregarEntidadRelacionada(pEntidad, pPropiedad, entidadTag);
            //    }
            //}

            LinkedOpenDataCL LodCL = new LinkedOpenDataCL("lod", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mloggerFactory.CreateLogger<LinkedOpenDataCL>(), mloggerFactory);
            Dictionary<string, EntidadLOD> listaSameAs = LodCL.ObtenerListaResourcesDeListaResultados(((ElementoGnoss)pElementoGnoss).Clave);
            //Dictionary<string, string> listaResultados = LodCL.ObtenerNombreUriDeEntidadesRelacionadasPorID(((ElementoGnoss)pElementoGnoss).Clave);

            pEntidad.Elemento = pElementoGnoss;

            foreach (string tag in pListaTags)
            {
                ElementoOntologia entidadTag = new ElementoOntologiaGnoss(Ontologia.GetEntidadTipo(TipoElementoGnoss.TagSioc));
                entidadTag.EstablecerID(tag);
                if (entidadTag.ID.Contains("+"))
                {
                    entidadTag.ID = entidadTag.ID.Replace("+", "%20");
                }
                entidadTag.Descripcion = tag;
                ElementoOntologia tagAuxiliar = ComprobarEntidadIncluida(entidadTag.ID);

                //Para cada entidad equivalente en freebase, ponemos una etiqueta owl:sameAs en la entidad

                if (listaSameAs.ContainsKey(tag))
                {
                    EntidadLOD entidad = listaSameAs[tag];
                    entidadTag.OWLSameAs.Add(entidad.Url);

                    foreach (string sameAs in entidad.ListaEntidadesSameAs)
                    {
                        entidadTag.OWLSameAs.Add(sameAs);
                    }
                }
                if (tagAuxiliar == null)
                {
                    //Obtengo la entidad
                    AgregarEntidadRelacionada(pEntidad, pPropiedad, entidadTag);
                    IElementoGnoss elementoGnoss = null;
                    ObtenerEntidad(entidadTag, elementoGnoss, true, pGestor);
                }
                else
                {
                    //asigno la entidad ya creada
                    entidadTag = tagAuxiliar;
                    AgregarEntidadRelacionada(pEntidad, pPropiedad, entidadTag);
                }
            }
        }

        /// <summary>
        /// Agrega a la propiedad comentario de una entidad uno comentario.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pPropiedad">Propiedad Tag</param>
        /// <param name="pGestor">Gestor del elemento</param>
        /// <param name="pComentario">Comentario</param>
        protected void AgregarRelacionComentarioEntidad(ElementoOntologia pEntidad, Propiedad pPropiedad, GestionGnoss pGestor, Comentario pComentario)
        {
            ElementoOntologia entidadComentario = new ElementoOntologiaGnoss(this.Ontologia.GetEntidadTipo(TipoElementoGnoss.ComentarioSioc));
            entidadComentario.Descripcion = pComentario.Nombre;
            AD.EntityModel.Models.Comentario.Comentario comentarioRow = pComentario.FilaComentario;
            UtilImportarExportar.ObtenerID(entidadComentario, comentarioRow, pComentario);
            ElementoOntologia entidadAuxiliar = ComprobarEntidadIncluida(entidadComentario.ID);

            //Obtengo las definiciones
            if ((entidadAuxiliar == null) || (!entidadAuxiliar.EstaCompleta))
            {
                if (entidadAuxiliar != null)
                    entidadComentario = entidadAuxiliar;

                //Obtengo la entidad
                AgregarEntidadRelacionada(pEntidad, pPropiedad, entidadComentario);
                ObtenerEntidad(entidadComentario, pComentario, true, pGestor);
            }
            else
            {
                //asigno la entidad ya creada
                entidadComentario = entidadAuxiliar;
                AgregarEntidadRelacionada(pEntidad, pPropiedad, entidadComentario);
            }
        }

        /// <summary>
        /// Agrega a la propiedad documento vinculado de un documento.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pPropiedad">Propiedad Tag</param>
        /// <param name="pGestor">Gestor del elemento</param>
        /// <param name="pDocumento">Documento vinculado</param>
        protected void AgregarRelacionDocumentoVinculadoEntidad(ElementoOntologia pEntidad, Propiedad pPropiedad, GestionGnoss pGestor, Documento pDocumento)
        {
            ElementoOntologia entidadDocVinc = new ElementoOntologiaGnoss(this.Ontologia.GetEntidadTipo(TipoElementoGnoss.Documento));
            entidadDocVinc.Descripcion = pDocumento.Titulo;
            AD.EntityModel.Models.Documentacion.Documento documentoRow = pDocumento.FilaDocumento;
            UtilImportarExportar.ObtenerID(entidadDocVinc, documentoRow, pDocumento);

            pPropiedad.ListaValores.Add(entidadDocVinc.ID, null);

            //Descomentar si se quiere sacar la información de los rec vinculados:

            //ElementoOntologia entidadDocVinc = new ElementoOntologiaGnoss(this.Ontologia.GetEntidadTipo(TipoElementoGnoss.Documento));
            //entidadDocVinc.Descripcion = pDocumento.Titulo;
            //DataRow documentoRow = pDocumento.FilaDocumento;
            //UtilImportarExportar.ObtenerID(entidadDocVinc, documentoRow, pDocumento);
            //ElementoOntologia entidadAuxiliar = ComprobarEntidadIncluida(entidadDocVinc.ID);

            ////Obtengo las definiciones
            //if ((entidadAuxiliar == null) || (!entidadAuxiliar.EstaCompleta))
            //{
            //    if (entidadAuxiliar != null)
            //        entidadDocVinc = entidadAuxiliar;

            //    //Obtengo la entidad
            //    AgregarEntidadRelacionada(pEntidad, pPropiedad, entidadDocVinc);
            //    ObtenerEntidad(entidadDocVinc, pDocumento, true, pGestor);
            //}
            //else
            //{
            //    //asigno la entidad ya creada
            //    entidadDocVinc = entidadAuxiliar;
            //    AgregarEntidadRelacionada(pEntidad, pPropiedad, entidadDocVinc);
            //}
        }

        #endregion

        #region Privados

        /// <summary>
        /// Obtiene todas las entidades relacionadas con la entidad
        /// </summary>
        /// <param name="pEntidad">Entidad de la cual se buscan sus entidades relacionadas</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad</param>
        /// <param name="pPropiedad">Propiedad que relaciona la entidad con otras entidades</param>
        /// <param name="pGestor">Gestor de entidades</param>
        /// <param name="pMontarPropHeredadas">Indica si se deben mostrar las propiedades heredadas</param>
        private void ObtenerEntidadesRelacionadas(Es.Riam.Semantica.OWL.ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor, bool pMontarPropHeredadas)
        {
            if ((pElementoGnoss != null) && (!pPropiedad.Heredada || pMontarPropHeredadas) && (pPropiedad.Seleccionada) && EsPropiedadParaExportar(pEntidad, pPropiedad) && pEntidad.ID != Guid.Empty.ToString())
            {
                //Si es un caso especial, lo trata una función específica
                if (!TratarCasoEspecial(pEntidad, pElementoGnoss, pPropiedad, pGestor))
                {
                    List<IElementoGnoss> listaHijos = pElementoGnoss.Hijos;

                    //Recorremos los hijos del elemento
                    foreach (IElementoGnoss elemento in listaHijos)
                    {
                        if (elemento != null)
                        {
                            //Obtenemos el hijo
                            Es.Riam.Semantica.OWL.ElementoOntologia entidad = new ElementoOntologiaGnoss(this.Ontologia.GetEntidadTipo(pPropiedad.Rango));
                            entidad.ID = UtilImportarExportar.ObtenerID(entidad.TipoEntidad, elemento);
                            entidad.Elemento = elemento;
                            Es.Riam.Semantica.OWL.ElementoOntologia entidadAuxiliar = ComprobarEntidadIncluida(entidad.ID);

                            if ((entidadAuxiliar == null) || (!entidadAuxiliar.EstaCompleta))
                            {
                                if (entidadAuxiliar != null)
                                    entidad = entidadAuxiliar;

                                entidad.Descripcion = "- Sin codificar -";
                                UtilImportarExportar.ObtenerNombreRealPropiedad(entidad, pEntidad, pPropiedad);
                                ObtenerEntidad(entidad, elemento, false, pGestor);
                            }
                            else
                                entidad = entidadAuxiliar;

                            //Lo relacionamos con la entidad
                            pEntidad.EntidadesRelacionadas.Add(entidad);

                            if (!pPropiedad.ListaValores.ContainsKey(entidad.ID))
                            {
                                pPropiedad.ListaValores.Add(entidad.ID, entidad);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Comprueba si la propiedad de la entidad ha sido seleccionada por el usuario para exportar
        /// </summary>
        /// <param name="pEntidad">Entidad que se va a exportar</param>
        /// <param name="pPropiedad">Propiedad que se debe comprobar</param>
        /// <returns>TRUE si ha sido seleccionada para exportar, FALSE en caso contrario</returns>
        private bool EsPropiedadParaExportar(Es.Riam.Semantica.OWL.ElementoOntologia pEntidad, Propiedad pPropiedad)
        {
            if (mListaDataSet != null && mListaDataSet.ContainsKey(pEntidad.TipoEntidad))
            {
                foreach (DataRow fila in ListaDataSet[pEntidad.TipoEntidad].Tables[0].Rows)
                {
                    if (fila["NombreReal"].ToString() == pPropiedad.Nombre && !(bool)fila["CheckBoxPropiedad"])
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Obtiene el padre de una entidad.
        /// </summary>
        /// <param name="pEntidad">Entidad de la cual se ha de obtener el nombre.</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad.</param>
        private void ObtenerPadreEntidad(Es.Riam.Semantica.OWL.ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss)
        {
            if ((pElementoGnoss != null) && (pElementoGnoss.Padre != null))
            {
                string TipoPadre = pElementoGnoss.Padre.GetType().Name;

                //Obtengo el padre
                Es.Riam.Semantica.OWL.ElementoOntologia padre = new ElementoOntologiaGnoss(this.Ontologia.GetEntidadTipo(TipoPadre));

                //if ((pEntidad.Superclase != "") && (!padre.TipoEntidad.Equals(TipoElementoGnoss.EntidadNoExportable)))
                if (pEntidad.Superclases.Count > 0 && !padre.TipoEntidad.Equals(TipoElementoGnoss.EntidadNoExportable))
                {
                    Propiedad propiedadPadre = UtilImportarExportar.ObtenerPropiedadDeNombre(UtilImportarExportar.PROPIEDAD_PADRE, pEntidad.Propiedades);
                    padre.ID = UtilImportarExportar.ObtenerID(padre.TipoEntidad, pElementoGnoss.Padre);

                    //Agregado por Javier: En las categorías del tesauro (tienen atributo Padre) pone como valor de propiedad Padre al gestor de tesauro, eso no puede ser:
                    if (padre.ID == Guid.Empty.ToString())
                        return;

                    Es.Riam.Semantica.OWL.ElementoOntologia entidadAuxiliar = ComprobarEntidadIncluida(padre.ID);
                    propiedadPadre.UnicoValor = new KeyValuePair<string, Es.Riam.Semantica.OWL.ElementoOntologia>(padre.ID, padre);

                    if (entidadAuxiliar == null)
                    {
                        ObtenerAtributosEntidad(padre, pElementoGnoss.Padre);
                        ListaEntidadesCreadas.Add(padre);
                    }
                    else
                    {
                        padre = entidadAuxiliar;
                    }
                    //Lo relaciono con la entidad
                    pEntidad.EntidadesRelacionadas.Add(padre);
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
        ~ExportadorElementoGnoss()
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
                    if (Ontologia != null)
                        Ontologia.Dispose();
                }
                Ontologia = null;
            }
        }

        #endregion
    }
}
