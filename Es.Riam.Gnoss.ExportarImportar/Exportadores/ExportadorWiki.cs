using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos;
using Es.Riam.Gnoss.Elementos.Comentario;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.ExportarImportar.ElementosOntologia;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Interfaces;
using Es.Riam.Metagnoss.ExportarImportar;
using Es.Riam.Metagnoss.ExportarImportar.Exportadores;
using Es.Riam.Semantica.OWL;
using Es.Riam.Semantica.Plantillas;
using Es.Riam.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Es.Riam.Gnoss.ExportarImportar.Exportadores
{
    /// <summary>
    /// Clase del exportador de Wiki
    /// </summary>
    public class ExportadorWiki : ExportadorElementoGnoss, IDisposable
    {

        #region Miembros

        /// <summary>
        /// Verdad si se está obteniendo el tesauro completo o solo categorías sueltas
        /// </summary>
        private bool mObtenerTesauroCompleto = false;

        /// <summary>
        /// Array con la ontología del documento semántico.
        /// </summary>
        public byte[] ARRAY_ONTOLOGIA_DOCSEM;

        /// <summary>
        /// URL de la ontología del documento semántico.
        /// </summary>
        public string URL_ONTOLOGIA_DOCSEM;

        /// <summary>
        /// Namespace de la ontología del documento semántico.
        /// </summary>
        public string NAMESPACE_DOCSEM;

        /// <summary>
        /// nombre de la ontología de un doc semántico.
        /// </summary>
        public string NOMBRE_ONTOLOGIA_DOCSEM;

        /// <summary>
        /// URL de intragnoss.
        /// </summary>
        public string URLINTRAGNOSS;

        /// <summary>
        /// ID del xml actual de la ontología.
        /// </summary>
        public Guid XML_ID;

        /// <summary>
        /// Ontología del recurso.
        /// </summary>
        public Ontologia ONTOLOGIA;

        /// <summary>
        /// Instancias principales del recurso.
        /// </summary>
        public List<ElementoOntologia> INSTANCIAS_PRINCIPALES;

        /// <summary>
        /// Clave del trozo de la ontologia ya que está está troceada.
        /// </summary>
        public string CLAVE_ONTO_TROCEADA;

        /// <summary>
        /// Indica si se han agregado ya los documentos vinculados del actual.
        /// </summary>
        private bool mDocumentosVinculadosAgregados;

        /// <summary>
        /// Configuración Xml del formulario semántico.
        /// </summary>
        public Dictionary<string, List<EstiloPlantilla>> CONFIG_XML_DOCSEM;

        /// <summary>
        /// DataSet de proyecto con la presentación para los recursos.
        /// </summary>
        private DataWrapperProyecto mDataWrapperProyecto;

        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private UtilSemCms mUtilSemCms;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private VirtuosoAD mVirtuosoAd;

        #endregion

        #region Constructor

        /// <summary>
        /// Crea un nuevo exportador de documentos
        /// </summary>
        /// <param name="pOntologia">Ontología</param>
        public ExportadorWiki(Ontologia pOntologia, string pIdiomaUsuario, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, UtilSemCms utilSemCms, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, VirtuosoAD virtuosoAd)
            : base(pOntologia, pIdiomaUsuario, loggingService, entityContext, configService, redisCacheWrapper, utilSemCms, servicesUtilVirtuosoAndReplication, virtuosoAd)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mUtilSemCms = utilSemCms;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        #endregion

        #region Métodos generales

        #region Métodos protegidos

        /// <summary>
        /// Obtiene los atributos de la entidad.
        /// </summary>
        /// <param name="pEntidadBuscada">Entidad de la que hay que obtener sus atribtos.</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad.</param>
        public override void ObtenerAtributosEntidad(ElementoOntologia pEntidadBuscada, IElementoGnoss pElementoGnoss)
        {
            if (pElementoGnoss is CategoriaTesauro)
            {
                //UtilImportarExportar.ObtenerAtributosEntidad(pEntidadBuscada, ((CategoriaTesauro)pElementoGnoss).FilaCategoria);

                Propiedad prefLabel = UtilImportarExportar.ObtenerPropiedadDeNombre(UtilImportarExportar.PROPIEDAD_PREFLABEL, pEntidadBuscada.Propiedades);

                string nombreCategoriaIdioma = ((CategoriaTesauro)pElementoGnoss).Nombre[IdiomaUsuario];

                if (prefLabel.FunctionalProperty)
                {
                    prefLabel.UnicoValor = new KeyValuePair<string, ElementoOntologia>(nombreCategoriaIdioma, null);
                }
                else
                {
                    prefLabel.ListaValores.Add(nombreCategoriaIdioma, null);
                }
                pEntidadBuscada.Descripcion = nombreCategoriaIdioma;
            }
            else if ((pElementoGnoss is Documento) && ((pEntidadBuscada.TipoEntidad.Equals(TipoElementoGnoss.Documento)) || (pEntidadBuscada.TipoEntidad.Equals(TipoElementoGnoss.Debate)) || (pEntidadBuscada.TipoEntidad.Equals(TipoElementoGnoss.Pregunta))))
            {
                //UtilImportarExportar.ObtenerAtributosEntidad(pEntidadBuscada, ((Documento)pElementoGnoss).FilaDocumento);
                Documento doc = (Documento)pElementoGnoss;
                string valor = null;
                //Obtengo los atributos del documento
                foreach (Propiedad propiedad in pEntidadBuscada.ObtenerAtributos())
                {
                    valor = null;

                    switch (propiedad.Nombre)
                    {
                        //case UtilImportarExportar.PROPIEDAD_DC_AUTOR:
                        //    valor = doc.Autor;
                        //    break;
                        case UtilImportarExportar.PROPIEDAD_SIOC_ITEM_DESCRIPCION:
                            valor = doc.Descripcion;
                            break;
                        case UtilImportarExportar.PROPIEDAD_DC_FECHA:
                            ConfigurarFechaPublicacionDocumento(doc);
                            valor = UtilImportarExportar.PasarFechaEnFormatoEstandar(doc.FilaDocumento.FechaModificacion.Value);
                            break;
                        case UtilImportarExportar.PROPIEDAD_DC_TITULO:
                            valor = doc.Titulo;
                            pEntidadBuscada.Descripcion = UtilCadenas.ObtenerTextoDeIdioma(valor, IdiomaUsuario, null);
                            break;
                        case UtilImportarExportar.PROPIEDAD_ENLACE:
                            if (doc.TipoDocumentacion.Equals(TiposDocumentacion.FicheroServidor) || doc.TipoDocumentacion.Equals(TiposDocumentacion.Imagen) || doc.TipoDocumentacion.Equals(TiposDocumentacion.Video))
                            {
                                valor = GestionOWLGnoss.ObtenerUrlEntidad(pEntidadBuscada.TipoEntidad, doc, Guid.Empty.ToString());
                            }
                            else
                            {
                                valor = UtilCadenas.ObtenerTextoDeIdioma(doc.Enlace, IdiomaUsuario, null);

                                if (doc.Enlace.EndsWith(".rdf"))
                                {
                                    valor += ".rdf";
                                }
                            }
                            break;
                            //case UtilImportarExportar.propiedad
                    }

                    if (valor != null)
                    {
                        Dictionary<string, string> listaIdiomaTextos = UtilCadenas.ObtenerTextoPorIdiomas(valor);

                        if (listaIdiomaTextos.Count == 0)
                        {
                            if (propiedad.FunctionalProperty)
                            {
                                propiedad.UnicoValor = new KeyValuePair<string, ElementoOntologia>(valor, null);
                            }
                            else
                            {
                                propiedad.ListaValores.Add(valor, null);
                            }
                        }
                        else
                        {
                            foreach (string idioma in listaIdiomaTextos.Keys)
                            {
                                propiedad.AgregarValorConIdioma(listaIdiomaTextos[idioma], idioma);
                            }
                        }
                    }
                }

                Propiedad propTipo = UtilImportarExportar.ObtenerPropiedadDeNombre(UtilImportarExportar.PROPIEDAD_DC_TIPO, pEntidadBuscada.Propiedades);
                valor = "";
                TiposDocumentacion tipo = (TiposDocumentacion)doc.FilaDocumento.Tipo;

                switch (tipo)
                {
                    case TiposDocumentacion.DafoProyecto:
                    case TiposDocumentacion.Hipervinculo:
                        valor = UtilImportarExportar.DC_INTERACTIVERESOURCE;
                        break;
                    case TiposDocumentacion.EntradaBlog:
                    case TiposDocumentacion.EntradaBlogTemporal:
                    case TiposDocumentacion.Newsletter:
                    case TiposDocumentacion.Nota:
                    case TiposDocumentacion.Ontologia:
                    case TiposDocumentacion.Semantico:
                    case TiposDocumentacion.Wiki:
                    case TiposDocumentacion.WikiTemporal:
                        valor = UtilImportarExportar.DC_TEXT;
                        break;
                    case TiposDocumentacion.FicheroServidor:
                        List<string> listaExtensionesAudio = new List<string>();
                        listaExtensionesAudio.Add(".wmp");
                        listaExtensionesAudio.Add(".wav");
                        listaExtensionesAudio.Add(".mp3");
                        listaExtensionesAudio.Add(".mp2");

                        List<string> listaExtensionesTexto = new List<string>();
                        listaExtensionesTexto.Add(".xls");
                        listaExtensionesTexto.Add(".xlsx");
                        listaExtensionesTexto.Add(".doc");
                        listaExtensionesTexto.Add(".docx");
                        listaExtensionesTexto.Add(".txt");
                        listaExtensionesTexto.Add(".pdf");

                        List<string> listaExtensionesVideoAnimacion = new List<string>();
                        listaExtensionesVideoAnimacion.Add(".ppt");
                        listaExtensionesVideoAnimacion.Add(".pps");
                        listaExtensionesVideoAnimacion.Add(".pptx");
                        listaExtensionesVideoAnimacion.Add(".ppsx");

                        if (listaExtensionesAudio.Contains(doc.Extension))
                        {
                            valor = UtilImportarExportar.DC_SOUND;
                        }
                        else if (listaExtensionesTexto.Contains(doc.Extension))
                        {
                            valor = UtilImportarExportar.DC_TEXT;
                        }
                        else if (listaExtensionesVideoAnimacion.Contains(doc.Extension))
                        {
                            valor = UtilImportarExportar.DC_MOVINGIMAGE;
                        }
                        else
                        {
                            valor = UtilImportarExportar.DC_SOFTWARE;
                        }
                        break;
                    case TiposDocumentacion.Imagen:
                    case TiposDocumentacion.ImagenWiki:
                        valor = UtilImportarExportar.DC_IMAGE;
                        break;
                    case TiposDocumentacion.ReferenciaADoc:
                        valor = UtilImportarExportar.DC_PHYSICALOBJECT;
                        break;
                    case TiposDocumentacion.Video:
                        valor = UtilImportarExportar.DC_MOVINGIMAGE;
                        break;
                }

                if (propTipo != null)
                {
                    if (propTipo.FunctionalProperty)
                    {
                        propTipo.UnicoValor = new KeyValuePair<string, ElementoOntologia>(valor, null);
                    }
                    else
                    {
                        propTipo.ListaValores.Add(valor, null);
                    }
                }

                if (doc.TipoDocumentacion == TiposDocumentacion.Semantico)
                {
                    Propiedad CvSem = UtilImportarExportar.ObtenerPropiedadDeNombre(UtilImportarExportar.PROPIEDAD_DOC_SEM, pEntidadBuscada.Propiedades);
                    //Obtenermos el RDF del CV y el ID de su elemento candidate.
                    string elementoRaiz = ObtenerRDFDocSemantico(pEntidadBuscada, (Documento)pElementoGnoss);

                    if (elementoRaiz != null)
                    {
                        CvSem.ListaValores.Add(elementoRaiz, null);
                    }
                }

                if (!string.IsNullOrEmpty(doc.FilaDocumento.Licencia))
                {
                    Propiedad licencia = UtilImportarExportar.ObtenerPropiedadDeNombre(UtilImportarExportar.PROPIEDAD_CC_LICENSE, pEntidadBuscada.Propiedades);
                    string idLicencia = UtilImportarExportar.ObtnerIDLicenciaDeDocumento(doc.FilaDocumento.Licencia);
                    licencia.ListaValores.Add(idLicencia, null);

                    if (pEntidadBuscada.Ontologia.RDFCVSemIncluido == null)
                    {
                        pEntidadBuscada.Ontologia.RDFCVSemIncluido = "";
                    }

                    if (!pEntidadBuscada.Ontologia.RDFCVSemIncluido.Contains(idLicencia))
                    {
                        pEntidadBuscada.Ontologia.RDFCVSemIncluido += UtilImportarExportar.ObtnerRDFLicenciaDeDocumento(doc.FilaDocumento.Licencia, idLicencia);
                    }
                }

                if (!string.IsNullOrEmpty(doc.FilaDocumento.Autor))
                {
                    Propiedad propAutor = UtilImportarExportar.ObtenerPropiedadDeNombre(UtilImportarExportar.PROPIEDAD_DC_AUTOR, pEntidadBuscada.Propiedades);

                    if (propAutor != null)
                    {
                        char[] separador = { ',' };
                        string[] autores = doc.FilaDocumento.Autor.Split(separador, StringSplitOptions.RemoveEmptyEntries);

                        int count = 0;
                        foreach (string autor in autores)
                        {
                            if (count != 0 || !doc.FilaDocumento.CreadorEsAutor)
                            {
                                if (!propAutor.ListaValores.ContainsKey(autor.Trim()))
                                {
                                    propAutor.ListaValores.Add(autor.Trim(), null);
                                }
                            }
                            count++;
                        }
                    }
                }

                if (doc.UrlRecursosRelacionadosRDF.Count > 0)
                {
                    Propiedad propRecRel = UtilImportarExportar.ObtenerPropiedadDeNombre(UtilImportarExportar.PROPIEDAD_SIOC_RELATED_TO, pEntidadBuscada.Propiedades);
                    foreach (string urlRecRel in doc.UrlRecursosRelacionadosRDF)
                    {
                        if (!propRecRel.ListaValores.ContainsKey(urlRecRel.Trim()))
                        {
                            propRecRel.ListaValores.Add(urlRecRel.Trim(), null);
                        }
                    }
                }
            }
            else if (pEntidadBuscada.TipoEntidad.Equals(TipoElementoGnoss.Comentario) || pEntidadBuscada.TipoEntidad.Equals(TipoElementoGnoss.ComentarioSioc))
            {
                Comentario com = (Comentario)pElementoGnoss;

                //Obtengo los atributos del documento
                foreach (Propiedad propiedad in pEntidadBuscada.ObtenerAtributos())
                {
                    string valor = null;

                    switch (propiedad.Nombre)
                    {
                        case UtilImportarExportar.PROPIEDAD_SIOC_ITEM_DESCRIPCION:
                            valor = com.FilaComentario.Descripcion;
                            pEntidadBuscada.Descripcion = com.FilaComentario.Descripcion;
                            break;
                        case UtilImportarExportar.PROPIEDAD_DC_FECHA:
                            valor = UtilImportarExportar.PasarFechaEnFormatoEstandar(com.FilaComentario.Fecha);
                            break;
                    }

                    if (valor != null)
                    {
                        if (propiedad.FunctionalProperty)
                        {
                            propiedad.UnicoValor = new KeyValuePair<string, ElementoOntologia>(valor, null);
                        }
                        else
                        {
                            propiedad.ListaValores.Add(valor, null);
                        }
                    }
                }
            }
            else if (pEntidadBuscada.TipoEntidad.Equals(TipoElementoGnoss.CategoriasTesauro))
                UtilImportarExportar.ObtenerAtributosEntidad(pEntidadBuscada, ((CategoriaTesauro)pElementoGnoss).FilaCategoria);
            else if (pElementoGnoss is ElementoGnoss)
                UtilImportarExportar.ObtenerAtributosEntidad(pEntidadBuscada, ((ElementoGnoss)pElementoGnoss).FilaElemento);
            else
                base.ObtenerAtributosEntidad(pEntidadBuscada, pElementoGnoss);

            /*Propiedad propiedadDescripcion = UtilImportarExportar.ObtenerPropiedadDeNombre(UtilImportarExportar.PROPIEDAD_DESCRIPCION, pEntidadBuscada.Propiedades);

            if (propiedadDescripcion.UnicoValor.Key.Equals(string.Empty))
            {
                if (pElementoGnoss is Competencia)
                {
                    propiedadDescripcion.UnicoValor = new KeyValuePair<string, ElementoOntologia>(pElementoGnoss.Nombre, null);
                    pEntidadBuscada.Descripcion = pElementoGnoss.Nombre;
                }
            }*/
        }

        /// <summary>
        /// Generaliza una entidad para obtener los atributos del padre.
        /// </summary>
        /// <param name="pEntidad">Entidad a generalizar</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad.</param>
        /// <param name="pFilaElemento">Fila del elemento</param>
        /// <param name="pGestor"></param>
        protected override void GeneralizarEntidad(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, object pFilaElemento, GestionGnoss pGestor)
        {
            if (pEntidad.TipoEntidad.Equals(TipoElementoGnoss.ComentarioSioc))
                base.GeneralizarEntidad(pEntidad, pElementoGnoss, pGestor, ((Comentario)pElementoGnoss).FilaComentario);
            else if ((pEntidad.TipoEntidad.Equals(TipoElementoGnoss.Debate)) && (pElementoGnoss is Documento))
                base.GeneralizarEntidad(pEntidad, ((Documento)pElementoGnoss).FilaDocumento);
            else if (pEntidad.TipoEntidad.Equals(TipoElementoGnoss.Pregunta))
                base.GeneralizarEntidad(pEntidad, ((Documento)pElementoGnoss).FilaDocumento);
            else
                base.GeneralizarEntidad(pEntidad, pFilaElemento);
        }

        /// <summary>
        /// Trata los casos especiales de competencias.
        /// </summary>
        /// <param name="pEntidad">Entidad que posee las propiedades</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad.</param>
        /// <param name="pPropiedad">Propiedad a tratar.</param>
        /// <param name="pGestor">Gestor de competencias.</param>
        /// <returns></returns>
        protected override bool TratarCasoEspecial(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor)
        {
            bool resultado = false;
            switch (pPropiedad.Nombre)
            {
                case UtilImportarExportar.PROPIEDAD_SIOC_TOPIC:
                    if (pElementoGnoss is Documento)
                    {
                        ObtenerCategoriaTesauro(pEntidad, pElementoGnoss, pPropiedad, pGestor);
                    }
                    resultado = true;
                    break;
                case UtilImportarExportar.PROPIEDAD_SIOC_COMENTARIOS:
                    if ((pElementoGnoss is Documento) && ((pEntidad.TipoEntidad == TipoElementoGnoss.Documento) || (pEntidad.TipoEntidad == TipoElementoGnoss.Debate) || (pEntidad.TipoEntidad == TipoElementoGnoss.Pregunta)))
                    {
                        ObtenerComentariosDocumento(pEntidad, pElementoGnoss, pPropiedad, pGestor);
                    }
                    resultado = true;
                    break;
                case UtilImportarExportar.PROPIEDAD_SIOC_CREADOR:
                    ObtenerCreadorDocumento(pEntidad, pElementoGnoss, pPropiedad, pGestor);
                    resultado = true;
                    break;
                case UtilImportarExportar.PROPIEDAD_DC_FECHA:
                    ConfigurarFechaPublicacionDocumento((Documento)pElementoGnoss);
                    resultado = true;
                    break;
                case UtilImportarExportar.PROPIEDAD_SIOC_COMUNIDADES:
                    if (pElementoGnoss is Documento)
                    {
                        ObtenerComunidadesDocumentoCompartido(pEntidad, pElementoGnoss, pPropiedad, pGestor);
                    }
                    resultado = true;
                    break;
                case UtilImportarExportar.PROPIEDAD_CATEGORIA_INFERIOR_DE_OTRA:
                    if (mObtenerTesauroCompleto)
                    {
                        ObtenerCategoriasTesauroOrdenadas(pEntidad, pElementoGnoss, pPropiedad, pGestor);
                    }
                    resultado = true;
                    break;
                case UtilImportarExportar.PROPIEDAD_CATEGORIA_PRIMER_NIVEL:
                    mObtenerTesauroCompleto = true;
                    ObtenerCategoriasTesauroOrdenadas(pEntidad, pElementoGnoss, pPropiedad, pGestor);
                    resultado = true;
                    break;
                case UtilImportarExportar.PROPIEDAD_CATEGORIA_ABUELA:
                case UtilImportarExportar.PROPIEDAD_CATEGORIA_NIETA_DE_OTRA:
                case UtilImportarExportar.PROPIEDAD_CATEGORIAS_HERMANAS:
                    resultado = true;
                    break;
                case UtilImportarExportar.PROPIEDAD_SIOC_REPLY_OF:
                    if (pElementoGnoss is Comentario && ((Comentario)pElementoGnoss).FilaComentario.ComentarioSuperiorID.HasValue)
                    {
                        Comentario comentario = ((Comentario)pElementoGnoss).GestorComentarios.ListaComentarios[((Comentario)pElementoGnoss).FilaComentario.ComentarioSuperiorID.Value];

                        AgregarRelacionComentarioEntidad(pEntidad, pPropiedad, pGestor, comentario);
                    }
                    resultado = true;
                    break;
                case UtilImportarExportar.PROPIEDAD_SIOC_ATTACHMENT:
                    if (!mDocumentosVinculadosAgregados && pElementoGnoss is Documento)
                    {
                        mDocumentosVinculadosAgregados = true;
                        ObtenerRecursosVinculadosDocumento(pEntidad, pElementoGnoss, pPropiedad, pGestor);
                    }
                    break;
                default:
                    resultado = true;
                    break;
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene la categoría de Tesauro a la que pertenece un elemento
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad</param>
        /// <param name="pPropiedad">Propiedad a tratar</param>
        /// <param name="pGestor">Gestor</param>
        private void ObtenerCategoriasTesauroOrdenadas(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor)
        {
            //Obtengo las categorías
            List<ElementoGnoss> listaCategorias = new List<ElementoGnoss>();

            if (pElementoGnoss is CategoriaTesauro)
            {
                foreach (CategoriaTesauro cat in ((CategoriaTesauro)pElementoGnoss).SubCategorias)
                {
                    listaCategorias.Add(cat);
                }
            }
            else if (pElementoGnoss is GestionTesauro)
            {
                foreach (CategoriaTesauro cat in ((GestionTesauro)pElementoGnoss).ListaCategoriasTesauroPrimerNivel.Values)
                {
                    listaCategorias.Add(cat);
                }
            }

            ObtenerColeccionOrdenada(pEntidad, pElementoGnoss, pPropiedad, pGestor, TipoElementoGnoss.CategoriasTesauroSkos, listaCategorias);

        }

        /// <summary>
        /// Obtiene la categoría de Tesauro a la que pertenece un elemento
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad</param>
        /// <param name="pPropiedad">Propiedad a tratar</param>
        /// <param name="pGestor">Gestor</param>
        private void ObtenerCategoriaTesauro(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor)
        {
            //Obtengo las categorías

            if (((Documento)pElementoGnoss).GestorDocumental.GestorTesauro == null)
            {
                return;
            }

            foreach (CategoriaTesauro elemento in ((Documento)pElementoGnoss).CategoriasTesauroPublicas.Values)
            {
                ElementoOntologia categoriaTesauro = new ElementoOntologiaGnoss(Ontologia.GetEntidadTipo(TipoElementoGnoss.CategoriasTesauroSkos));
                UtilImportarExportar.ObtenerID(categoriaTesauro, elemento.FilaCategoria, elemento);
                ElementoOntologia categoriaTesauroAuxiliar = ComprobarEntidadIncluida(categoriaTesauro.ID);

                if ((categoriaTesauroAuxiliar == null) || (!categoriaTesauroAuxiliar.EstaCompleta))
                {
                    if (categoriaTesauroAuxiliar != null)
                        categoriaTesauro = categoriaTesauroAuxiliar;

                    //Obtengo la entidad
                    AgregarEntidadRelacionada(pEntidad, pPropiedad, categoriaTesauro);
                    ObtenerEntidad(categoriaTesauro, elemento, true, elemento.GestorTesauro);
                }
                else
                {
                    //asigno la entidad ya creada
                    categoriaTesauro = categoriaTesauroAuxiliar;
                    AgregarEntidadRelacionada(pEntidad, pPropiedad, categoriaTesauro);
                }
            }
        }


        /// <summary>
        /// Obtiene los comentario de documento.
        /// </summary>
        /// <param name="pEntidad">Elemento de la ontología</param>
        /// <param name="pElementoGnoss">Elemento GNOSS</param>
        /// <param name="pPropiedad">Propiedad</param>
        /// <param name="pGestor">Gestor documental</param>
        public void ObtenerCreadorDocumento(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor)
        {
            Identidad creador = null;
            GestionIdentidades gestorIdentidades = null;
            Guid creadorID = Guid.Empty;

            if (pElementoGnoss is Documento)
            {
                Documento documento = (Documento)pElementoGnoss;
                GestorDocumental gestorDocumentacion = (GestorDocumental)pGestor;
                //"DocumentoID = '" + documento.Clave + "' AND BaseRecursosID = '" + gestorDocumentacion.BaseRecursosIDActual + "'"
                List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filas = gestorDocumentacion.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(documento.Clave) && doc.BaseRecursosID.Equals(gestorDocumentacion.BaseRecursosIDActual)).ToList();
                if (filas.Count > 0 && filas.First().IdentidadPublicacionID.HasValue)
                {
                    creadorID = filas.First().IdentidadPublicacionID.Value;
                }
                else
                {
                    creadorID = documento.CreadorID;
                }

                gestorIdentidades = documento.GestorDocumental.GestorIdentidades;
            }
            else if (pElementoGnoss is Comentario)
            {
                Comentario comentario = (Comentario)pElementoGnoss;
                creadorID = comentario.FilaComentario.IdentidadID;
                gestorIdentidades = ((GestorDocumental)pGestor).GestorIdentidades;
            }

            if (gestorIdentidades == null)
            {
                gestorIdentidades = new GestionIdentidades(new DataWrapperIdentidad(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            }

            if (!gestorIdentidades.ListaIdentidades.ContainsKey(creadorID))
            {
                IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                gestorIdentidades.DataWrapperIdentidad.Merge(identCN.ObtenerIdentidadPorID(creadorID, true));
                identCN.Dispose();

                gestorIdentidades.RecargarHijos();
            }

            if (gestorIdentidades.ListaIdentidades.ContainsKey(creadorID))
            {
                creador = gestorIdentidades.ListaIdentidades[creadorID];
            }

            if (creador != null)
            {
                ElementoOntologia entidadCreador = null;
                if (creador.Tipo != TiposIdentidad.Organizacion && creador.Tipo != TiposIdentidad.ProfesionalCorporativo)
                {
                    entidadCreador = new ElementoOntologiaGnoss(this.Ontologia.GetEntidadTipo(TipoElementoGnoss.PerfilPersonaFoaf));
                }
                else
                {
                    entidadCreador = new ElementoOntologiaGnoss(this.Ontologia.GetEntidadTipo(TipoElementoGnoss.PerfilOrganizacionFoaf));
                }

                ExportadorCurriculum exportadorCv = new ExportadorCurriculum(Ontologia, creador.GestorIdentidades, IdiomaUsuario, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mUtilSemCms, mServicesUtilVirtuosoAndReplication, mVirtuosoAd);

                entidadCreador.Descripcion = creador.Nombre();
                AD.EntityModel.Models.IdentidadDS.Identidad creadorRow = creador.FilaIdentidad;

                UtilImportarExportar.ObtenerID(entidadCreador, creadorRow, creador);
                ElementoOntologia entidadAuxiliar = ComprobarEntidadIncluida(entidadCreador.ID);

                //Obtengo las definiciones
                if ((entidadAuxiliar == null) || (!entidadAuxiliar.EstaCompleta))
                {
                    if (entidadAuxiliar != null)
                    {
                        entidadCreador = entidadAuxiliar;
                    }
                    //Obtengo la entidad
                    AgregarEntidadRelacionada(pEntidad, pPropiedad, entidadCreador);
                    exportadorCv.ObtenerEntidad(entidadCreador, creador, false, pGestor);
                }
                else
                {
                    //asigno la entidad ya creada
                    entidadCreador = entidadAuxiliar;
                    AgregarEntidadRelacionada(pEntidad, pPropiedad, entidadCreador);
                }

                if (pElementoGnoss is Documento && ((Documento)pElementoGnoss).FilaDocumento.CreadorEsAutor)
                {
                    Propiedad propAutorPropio = UtilImportarExportar.ObtenerPropiedadDeNombre(UtilImportarExportar.PROPIEDAD_FOAF_MAKER, pEntidad.Propiedades);
                    AgregarEntidadRelacionada(pEntidad, propAutorPropio, entidadCreador);
                }
            }
        }

        /// <summary>
        /// Obtiene las comunidades en las que esta compartido el documento
        /// </summary>
        /// <param name="pEntidad"></param>
        /// <param name="pElementoGnoss"></param>
        /// <param name="pPropiedad"></param>
        /// <param name="pGestor"></param>
        public void ObtenerComunidadesDocumentoCompartido(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor)
        {
            Documento documento = (Documento)pElementoGnoss;
            List<Guid> listaProyectos = new List<Guid>();

            foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos fila in documento.FilaDocumento.DocumentoWebVinBaseRecursos)
            {
                List<AD.EntityModel.Models.Documentacion.BaseRecursosProyecto> listaBaseRecursosProyecto = documento.GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosProyecto.Where(doc => doc.BaseRecursosID.Equals(fila.BaseRecursosID)).ToList();
                if (listaBaseRecursosProyecto.Count > 0)
                {
                    listaProyectos.Add(listaBaseRecursosProyecto.First().BaseRecursosID);
                }
            }

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionProyecto gestProy = new GestionProyecto(proyCN.ObtenerProyectosPorIDsCargaLigera(listaProyectos), mLoggingService, mEntityContext);
            proyCN.Dispose();

            //ExportadorComunidad exportadorComunidad = new ExportadorComunidad(Ontologia,IdiomaUsuario);

            foreach (Elementos.ServiciosGenerales.Proyecto proy in gestProy.ListaProyectos.Values)
            {
                ElementoOntologia entidadComunidad = new ElementoOntologiaGnoss(this.Ontologia.GetEntidadTipo(TipoElementoGnoss.ComunidadSioc));

                entidadComunidad.Descripcion = proy.Nombre;

                UtilImportarExportar.ObtenerID(entidadComunidad, proy.FilaProyecto, proy);
                ElementoOntologia entidadAuxiliar = ComprobarEntidadIncluida(entidadComunidad.ID);

                //Obtengo las definiciones
                if ((entidadAuxiliar == null) || (!entidadAuxiliar.EstaCompleta))
                {
                    if (entidadAuxiliar != null)
                    {
                        entidadComunidad = entidadAuxiliar;
                    }
                    //Obtengo la entidad
                    AgregarEntidadRelacionada(pEntidad, pPropiedad, entidadComunidad);
                    //exportadorComunidad.ObtenerEntidad(entidadComunidad, proy, false, pGestor);
                }
                else
                {
                    //asigno la entidad ya creada
                    entidadComunidad = entidadAuxiliar;
                    AgregarEntidadRelacionada(pEntidad, pPropiedad, entidadComunidad);
                }
            }
        }

        /// <summary>
        /// Obtiene todos los comentarios de un documento
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad</param>
        /// <param name="pPropiedad">Propiedad a tratar</param>
        /// <param name="pGestor">Gestor</param>
        private void ObtenerComentariosDocumento(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor)
        {
            Documento documento = (Documento)pElementoGnoss;

            foreach (Comentario comentario in documento.Comentarios)
            {
                AgregarRelacionComentarioEntidad(pEntidad, pPropiedad, pGestor, comentario);
            }
        }

        /// <summary>
        /// Obtiene los tags de documento.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad</param>
        /// <param name="pPropiedad">Propiedad a tratar</param>
        /// <param name="pGestor">Gestor</param>
        private void ObtenerTagsDocumento(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor)
        {
            Documento documento = (Documento)pElementoGnoss;
            List<string> listaTags = new List<string>();

            if (!string.IsNullOrEmpty(documento.FilaDocumento.Tags))
            {
                foreach (string tag in UtilCadenas.SepararTexto(documento.FilaDocumento.Tags))
                {
                    listaTags.Add(tag);
                }
            }

            AgregarRelacionTagsEntidad(pEntidad, pElementoGnoss, pPropiedad, documento.GestorDocumental, listaTags);
        }

        /// <summary>
        /// Obtiene la fecha de publicación de un documento.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        private void ConfigurarFechaPublicacionDocumento(Documento pDocumento)
        {
            if (pDocumento.GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursos.Count == 0)
            {
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                if (pDocumento.FilaDocumento.ProyectoID != ProyectoAD.MetaProyecto)
                {
                    docCN.ObtenerBaseRecursosProyecto(pDocumento.GestorDocumental.DataWrapperDocumentacion, pDocumento.FilaDocumento.ProyectoID.Value);
                }
                else if (pDocumento.FilaDocumento.OrganizacionID != ProyectoAD.MetaProyecto)
                {
                    docCN.ObtenerBaseRecursosOrganizacion(pDocumento.GestorDocumental.DataWrapperDocumentacion, pDocumento.FilaDocumento.CreadorID);
                }
                else
                {
                    List<Guid> listaIdent = new List<Guid>() { pDocumento.FilaDocumento.CreadorID };

                    UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    List<AD.EntityModel.Models.UsuarioDS.Usuario> listaUsuarios = usuCN.ObtenerUsuariosPorIdentidadesCargaLigera(listaIdent);
                    usuCN.Dispose();

                    Guid usuarioID = listaUsuarios.First().UsuarioID;

                    docCN.ObtenerBaseRecursosUsuario(pDocumento.GestorDocumental.DataWrapperDocumentacion, usuarioID);
                }

                docCN.Dispose();
            }
        }

        /// <summary>
        /// Obtiene el RDF de un documento semántico para su inclusión.
        /// </summary>
        /// <param name="pEntidad">Entidad de la ontología</param>
        /// <param name="pDocumento">Documento semántico</param>
        /// <returns>ID de la entidad principal del RDF del documento semántico</returns>
        private string ObtenerRDFDocSemantico(ElementoOntologia pEntidad, Documento pDocumento)
        {
            if (URL_ONTOLOGIA_DOCSEM == null /*|| ARRAY_ONTOLOGIA_DOCSEM == null*/)
            {
                return null;
            }

            Guid documentoID = pDocumento.Clave;
            string elementoRaizID = null;

            GestionOWL gestorOWL = new GestionOWL();

            //string namespaceSalvado = GestionOWL.NamespaceOntologia;
            //string urlSalvada = GestionOWL.UrlOntologia;

            //GestionOWL.NamespaceOntologia = GestionOWL.NAMESPACE_ONTO_GNOSS;
            //GestionOWL.UrlOntologia = URL_ONTOLOGIA_DOCSEM;
            string namespaceOnto = GestionOWL.NAMESPACE_ONTO_GNOSS;

            if (NAMESPACE_DOCSEM != null)
            {
                namespaceOnto = NAMESPACE_DOCSEM;
            }

            gestorOWL.NamespaceOntologia = namespaceOnto;

            #region Obtener RDF de la BD
            Guid ontologiaID = Guid.Empty;
            if (pDocumento.FilaDocumento.ElementoVinculadoID.HasValue)
            {
                ontologiaID = pDocumento.FilaDocumento.ElementoVinculadoID.Value;
            }

            Ontologia ontologia = null;
            Guid xmlID = XML_ID;

            //Dictionary<string, List<EstiloPlantilla>> estilosAux = new Dictionary<string, List<EstiloPlantilla>>(CONFIG_XML_DOCSEM);
            Dictionary<string, List<EstiloPlantilla>> estilosAux = CONFIG_XML_DOCSEM;

            if (ONTOLOGIA == null)
            {
                if (!string.IsNullOrEmpty(CLAVE_ONTO_TROCEADA))
                {
                    if (!EstiloPlantilla.OntologiasTrozosCargadas.ContainsKey(CLAVE_ONTO_TROCEADA) || EstiloPlantilla.OntologiasTrozosCargadas[CLAVE_ONTO_TROCEADA].Key != xmlID)
                    {
                        if (EstiloPlantilla.OntologiasTrozosCargadas.ContainsKey(CLAVE_ONTO_TROCEADA))
                        {
                            EstiloPlantilla.OntologiasTrozosCargadas.Remove(CLAVE_ONTO_TROCEADA);
                        }

                        ontologia = new Ontologia(ARRAY_ONTOLOGIA_DOCSEM, true);

                        ontologia.LeerOntologia();
                        ontologia.EstilosPlantilla = estilosAux;
                        ontologia.IdiomaUsuario = IdiomaUsuario;
                        ontologia.OntologiaID = pDocumento.ElementoVinculadoID;

                        try
                        {
                            EstiloPlantilla.OntologiasTrozosCargadas.Add(CLAVE_ONTO_TROCEADA, new KeyValuePair<Guid, Ontologia>(xmlID, ontologia));
                        }
                        catch (Exception) { }
                    }

                    ontologia = (Ontologia)EstiloPlantilla.OntologiasTrozosCargadas[CLAVE_ONTO_TROCEADA].Value.Clone();
                }
                else
                {
                    if (!EstiloPlantilla.OntologiasCargadas.ContainsKey(ontologiaID) || EstiloPlantilla.OntologiasCargadas[ontologiaID].Key != xmlID)
                    {
                        if (EstiloPlantilla.OntologiasCargadas.ContainsKey(ontologiaID))
                        {
                            EstiloPlantilla.OntologiasCargadas.Remove(ontologiaID);
                        }

                        ontologia = new Ontologia(ARRAY_ONTOLOGIA_DOCSEM, true);

                        ontologia.LeerOntologia();
                        ontologia.EstilosPlantilla = estilosAux;
                        ontologia.IdiomaUsuario = IdiomaUsuario;
                        ontologia.OntologiaID = pDocumento.ElementoVinculadoID;

                        try
                        {
                            EstiloPlantilla.OntologiasCargadas.Add(ontologiaID, new KeyValuePair<Guid, Ontologia>(xmlID, ontologia));
                        }
                        catch (Exception) { }
                    }

                    ontologia = (Ontologia)EstiloPlantilla.OntologiasCargadas[ontologiaID].Value.Clone();
                }

                ontologia.EstilosPlantilla = estilosAux;
                ontologia.IdiomaUsuario = IdiomaUsuario;
            }
            else
            {
                ontologia = ONTOLOGIA;
                ontologia.EstilosPlantilla = estilosAux;
            }

            ontologia.GenararNamespacesHuerfanos = true;
            List<ElementoOntologia> instanciasPrincipales = null;
            string lineaRDF = null;

            if (INSTANCIAS_PRINCIPALES == null)
            {
                if (pDocumento.FilaDocumento.ProyectoID.HasValue)
                {
                    lineaRDF = ObtenerRdfDeDocumento(documentoID, pDocumento.FilaDocumento.ProyectoID.Value, namespaceOnto);
                }

                instanciasPrincipales = gestorOWL.LeerFicheroRDF(ontologia, lineaRDF, true);
            }
            else
            {
                instanciasPrincipales = INSTANCIAS_PRINCIPALES;
            }

            try
            {
                Dictionary<KeyValuePair<string, string>, object[]> datosEntExternas = new Dictionary<KeyValuePair<string, string>, object[]>();
                AD.EntityModel.Models.PersonaDS.Persona filaPers = null;
                AD.EntityModel.Models.ProyectoDS.Proyecto filaProy = null;

                if (IdentidadActual != null && IdentidadActual.Persona != null)
                {
                    filaPers = IdentidadActual.Persona.FilaPersona;
                }

                if (ProyectoSeleccionado != null)
                {
                    filaProy = ProyectoSeleccionado.FilaProyecto;
                }

                //Preparo que datos hay que cargar:
                foreach (ElementoOntologia elemPrinc in instanciasPrincipales)
                {
                    mUtilSemCms.ObtenerPropiedadesSelecEntDeEntidad(elemPrinc, datosEntExternas, URLINTRAGNOSS, filaPers, filaProy, instanciasPrincipales);
                }

                if (datosEntExternas.Count > 0)
                {
                    mUtilSemCms.ObtenerDatosEntidadesExternas(datosEntExternas, URLINTRAGNOSS, filaPers, filaProy, instanciasPrincipales);

                    foreach (ElementoOntologia entidad in instanciasPrincipales)
                    {
                        AgregarPropiedadesEntidadesExternas(entidad, datosEntExternas, null);
                    }
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, "Error al traer datos entidades externas en RDF.");
            }
            if (pDocumento.FilaDocumento.ElementoVinculadoID.HasValue)
            {
                ExcluirPropiedadesNoProceden(instanciasPrincipales, pDocumento.FilaDocumento.ElementoVinculadoID.Value);
            }


            //Establezco de nuevo el namespace por si se ha machacado:
            gestorOWL.NamespaceOntologia = namespaceOnto;

            //Establezco la entidad principal:
            elementoRaizID = instanciasPrincipales[0].Uri;

            MemoryStream mStream = (MemoryStream)gestorOWL.PasarOWL(null, ontologia, instanciasPrincipales, null, null);
            mStream.Position = 0;
            StreamReader stream = new StreamReader(mStream);
            lineaRDF = stream.ReadToEnd();
            stream.Close();
            stream.Dispose();
            stream = null;

            //Elimino del RDF la cabecera y el elemento RDF:
            int indiceInicioRDF = lineaRDF.IndexOf("<rdf:RDF");
            int indiceFinalInicioRDF = indiceInicioRDF + lineaRDF.Substring(indiceInicioRDF).IndexOf(">");
            lineaRDF = lineaRDF.Substring(indiceFinalInicioRDF + 1).Replace("</rdf:RDF>", "");
            lineaRDF = lineaRDF.Trim();


            if (pEntidad.Ontologia.RDFCVSemIncluido == null)
            {
                pEntidad.Ontologia.RDFCVSemIncluido = "";
            }

            pEntidad.Ontologia.RDFCVSemIncluido += "\r\n" + lineaRDF + "\r\n";
            FusionarListaNamespaces(pEntidad.Ontologia.NamespacesDefinidosExtra, ontologia.NamespacesDefinidosInv);

            //Agrego los namespaces definidos de la ontología del recurso a la GNOSS:
            foreach (string key in ontologia.NamespacesDefinidos.Keys)
            {
                if (!pEntidad.Ontologia.NamespacesDefinidos.ContainsKey(key))
                {
                    pEntidad.Ontologia.NamespacesDefinidos.Add(key, ontologia.NamespacesDefinidos[key]);
                }
            }

            #endregion

            ////Devuelvo el namespace y url de la ontología a su ser:
            //GestionOWL.NamespaceOntologia = namespaceSalvado;
            //GestionOWL.UrlOntologia = urlSalvada;

            foreach (ElementoOntologia entidad in instanciasPrincipales)
            {
                entidad.Dispose();
            }

            instanciasPrincipales.Clear();
            instanciasPrincipales = null;
            ontologia = null;

            return elementoRaizID;
        }

        /// <summary>
        /// Fusiona dos listas de namespaces.
        /// </summary>
        /// <param name="pListaFinal">Lista final</param>
        /// <param name="pListaFusionar">Lista a fusionar</param>
        private void FusionarListaNamespaces(Dictionary<string, string> pListaFinal, Dictionary<string, string> pListaFusionar)
        {
            foreach (string key in pListaFusionar.Keys)
            {
                if (!pListaFinal.ContainsKey(key))
                {
                    pListaFinal.Add(key, pListaFusionar[key]);
                }
            }
        }

        /// <summary>
        /// Excluye del RDF las propiedades que no deben pintarse.
        /// </summary>
        /// <param name="pInstanciasPrincipales">Instancias principales del recurso</param>
        /// <param name="pOntologiaID">ID de ontología</param>
        private void ExcluirPropiedadesNoProceden(List<ElementoOntologia> pInstanciasPrincipales, Guid pOntologiaID)
        {
            if (ProyectoPresentacionDS != null)
            {
                foreach (ElementoOntologia entidad in pInstanciasPrincipales)
                {
                    ExcluirPropiedadesNoProceden(entidad, pOntologiaID);
                }
            }
        }

        /// <summary>
        /// Excluye de la entidad las propiedades que no deben pintarse.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pOntologiaID">ID de ontología</param>
        private void ExcluirPropiedadesNoProceden(ElementoOntologia pEntidad, Guid pOntologiaID)
        {
            List<Propiedad> propsEliminar = new List<Propiedad>();

            foreach (Propiedad propiedad in pEntidad.Propiedades)
            {
                bool propBuena = false;
                foreach (PresentacionListadoSemantico filaPre in ProyectoPresentacionDS.ListaPresentacionListadoSemantico.Where(presentacion => presentacion.OntologiaID.Equals(pOntologiaID)).ToList())
                {
                    if (filaPre.Propiedad.Contains(propiedad.Nombre) || filaPre.Propiedad.Contains(propiedad.NombreConNamespace))
                    {
                        propBuena = true;
                        break;
                    }
                }

                if (!propBuena)
                {
                    foreach (PresentacionMosaicoSemantico filaPre in ProyectoPresentacionDS.ListaPresentacionMosaicoSemantico.Where(presentacion => presentacion.OntologiaID.Equals(pOntologiaID)).ToList())
                    {
                        if (filaPre.Propiedad.Contains(propiedad.Nombre) || filaPre.Propiedad.Contains(propiedad.NombreConNamespace))
                        {
                            propBuena = true;
                            break;
                        }
                    }
                }

                if (!propBuena)
                {
                    foreach (PresentacionMapaSemantico filaPre in ProyectoPresentacionDS.ListaPresentacionMapaSemantico.Where(presentacion => presentacion.OntologiaID.Equals(pOntologiaID)).ToList())
                    {
                        if (filaPre.Propiedad.Contains(propiedad.Nombre) || filaPre.Propiedad.Contains(propiedad.NombreConNamespace))
                        {
                            propBuena = true;
                            break;
                        }
                    }
                }

                if (!propBuena)
                {
                    propsEliminar.Add(propiedad);
                    continue;
                }

                if (propiedad.Tipo == TipoPropiedad.ObjectProperty)
                {
                    if (propiedad.EspecifPropiedad.SelectorEntidad != null)
                    {
                        foreach (ElementoOntologia hijo in pEntidad.EntidadesRelacionadas)
                        {
                            ExcluirPropiedadesNoProceden(hijo, pOntologiaID);
                        }
                    }
                    else
                    {
                        foreach (ElementoOntologia hijo in propiedad.ValoresUnificados.Values)
                        {
                            if (hijo != null)
                            {
                                ExcluirPropiedadesNoProceden(hijo, pOntologiaID);
                            }
                        }
                    }
                }
            }

            foreach (Propiedad propElm in propsEliminar)
            {
                pEntidad.Propiedades.Remove(propElm);
            }
        }

        /// <summary>
        /// Agrega las propiedades de las entidades externas.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pDatosEntExternas">Datos externos</param>
        /// <param name="pFacetadoAuxDS">DataSet con los datos externos</param>
        private void AgregarPropiedadesEntidadesExternas(ElementoOntologia pEntidad, Dictionary<KeyValuePair<string, string>, object[]> pDatosEntExternas, FacetadoDS pFacetadoAuxDS)
        {
            foreach (Propiedad propiedad in pEntidad.Propiedades)
            {
                if (propiedad.Tipo == TipoPropiedad.ObjectProperty)
                {
                    if (propiedad.EspecifPropiedad.SelectorEntidad != null && propiedad.EspecifPropiedad.SelectorEntidad.Grafo != "geonames" && propiedad.ValoresUnificados.Count > 0)
                    {
                        if (propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "PersonaGnoss" || propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "GruposGnoss")
                        {
                            continue;
                        }
                        else if (propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "UrlRecurso")
                        {
                            #region Acido

                            DataWrapperDocumentacion dataWrapperDocumentacion = null;
                            Dictionary<Guid, string> listaRec = mUtilSemCms.GenerarOntologiaAuxiliarDocumentosExternos(propiedad, pDatosEntExternas, out dataWrapperDocumentacion);

                            if (dataWrapperDocumentacion.ListaBaseRecursos.Count == 0 && dataWrapperDocumentacion.ListaDocumento.Count > 0 && dataWrapperDocumentacion.ListaDocumento.First().ProyectoID != ProyectoAD.MetaProyecto)
                            {
                                Guid proy = dataWrapperDocumentacion.ListaDocumento.First().ProyectoID.Value;
                                bool mismoProy = true;

                                foreach (AD.EntityModel.Models.Documentacion.Documento filaDoc in dataWrapperDocumentacion.ListaDocumento)
                                {
                                    if (filaDoc.ProyectoID != proy)
                                    {
                                        mismoProy = false;
                                        break;
                                    }
                                }

                                if (mismoProy)
                                {
                                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                                    docCN.ObtenerBaseRecursosProyecto(dataWrapperDocumentacion, dataWrapperDocumentacion.ListaDocumento.First().ProyectoID.Value);
                                    docCN.Dispose();
                                }
                            }

                            DataWrapperDocumentacion docPorRecDW = new DataWrapperDocumentacion();
                            docPorRecDW.Merge(dataWrapperDocumentacion);

                            GestorDocumental gestorDocumental = new GestorDocumental(docPorRecDW, mLoggingService, mEntityContext);

                            foreach (Guid docID in listaRec.Keys)
                            {
                                List<AD.EntityModel.Models.Documentacion.Documento> filasDoc = dataWrapperDocumentacion.ListaDocumento.Where(doc => doc.DocumentoID.Equals(docID)).ToList();
                                if (filasDoc.Count == 0)
                                {
                                    continue;
                                }

                                Documento documento = gestorDocumental.ListaDocumentos[docID];

                                string tipo = TipoElementoGnoss.Documento;
                                if (documento.TipoDocumentacion.Equals(TiposDocumentacion.Debate))
                                {
                                    tipo = TipoElementoGnoss.Debate;
                                }
                                else if (documento.TipoDocumentacion.Equals(TiposDocumentacion.Pregunta))
                                {
                                    tipo = TipoElementoGnoss.Pregunta;
                                }

                                ElementoOntologia entidadResultado = new ElementoOntologiaGnoss(this.Ontologia.GetEntidadTipo(tipo));
                                ExportadorElementoGnoss exportador = new ExportadorWiki(Ontologia, IdiomaUsuario, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mUtilSemCms, mServicesUtilVirtuosoAndReplication, mVirtuosoAd);

                                exportador.TipoElementoExportar = TipoElementoExportar;

                                UtilImportarExportar.ObtenerID(entidadResultado, documento.FilaDocumento, documento);
                                ElementoOntologia entidadAuxiliar = ComprobarEntidadIncluida(entidadResultado.ID);
                                //Obtengo las definiciones
                                if ((entidadAuxiliar == null) || (!entidadAuxiliar.EstaCompleta))
                                {
                                    if (entidadAuxiliar != null)
                                        entidadResultado = entidadAuxiliar;

                                    //Obtengo la entidad
                                    AgregarEntidadRelacionada(pEntidad, propiedad, entidadResultado);
                                    exportador.ObtenerEntidad(entidadResultado, documento, false, gestorDocumental);
                                }
                                else
                                {
                                    //asigno la entidad ya creada
                                    entidadResultado = entidadAuxiliar;
                                    AgregarEntidadRelacionada(pEntidad, propiedad, entidadResultado);
                                }
                            }

                            #endregion
                        }
                        else
                        {
                            #region Virtuoso

                            FacetadoDS facetadoDS = null;
                            EstiloPlantillaEspecifProp estiloPropAux = null;
                            Ontologia ontologiaAux = null;

                            List<string> entidadesPintar = UtilSemCms.GenerarOntologiaAuxiliarEntExternas(propiedad, pDatosEntExternas, pFacetadoAuxDS, IdiomaUsuario, out facetadoDS, out estiloPropAux, out ontologiaAux);

                            ontologiaAux.GenararNamespacesHuerfanos = true;
                            string urlOntologiaNueva = propiedad.EspecifPropiedad.SelectorEntidad.Grafo;

                            if (!urlOntologiaNueva.Contains("http://"))
                            {
                                urlOntologiaNueva = URLINTRAGNOSS + "Ontologia/" + urlOntologiaNueva + "#";
                            }

                            if (propiedad.EspecifPropiedad.SelectorEntidad.NamespaceGrafo != null && !pEntidad.Ontologia.NamespacesDefinidos.ContainsKey(urlOntologiaNueva) && !pEntidad.Ontologia.NamespacesDefinidosInv.ContainsKey(propiedad.EspecifPropiedad.SelectorEntidad.NamespaceGrafo))
                            {
                                pEntidad.Ontologia.NamespacesDefinidos.Add(urlOntologiaNueva, propiedad.EspecifPropiedad.SelectorEntidad.NamespaceGrafo);
                                pEntidad.Ontologia.NamespacesDefinidosInv.Add(propiedad.EspecifPropiedad.SelectorEntidad.NamespaceGrafo, urlOntologiaNueva);
                            }

                            ontologiaAux.NamespacesDefinidos = pEntidad.Ontologia.NamespacesDefinidos;
                            ontologiaAux.NamespacesDefinidosInv = pEntidad.Ontologia.NamespacesDefinidosInv;

                            foreach (string entidadID in entidadesPintar)
                            {
                                ElementoOntologia entidadAux = ontologiaAux.GetEntidadTipo(estiloPropAux.NombreEntidad, true);
                                entidadAux.ID = entidadID;
                                pEntidad.EntidadesRelacionadas.Add(entidadAux);

                                foreach (EstiloPlantillaEspecifProp estilo in propiedad.EspecifPropiedad.SelectorEntidad.PropiedadesLectura)
                                {
                                    if (estilo.NombreRealPropiedad == null)
                                    {
                                        continue;
                                    }

                                    AgregarPropiedadEntidadExternas(entidadAux, pDatosEntExternas, facetadoDS, estilo);
                                }
                            }

                            #endregion
                        }
                    }
                    else
                    {
                        foreach (ElementoOntologia entidad in propiedad.ValoresUnificados.Values)
                        {
                            if (entidad != null)
                            {
                                AgregarPropiedadesEntidadesExternas(entidad, pDatosEntExternas, pFacetadoAuxDS);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Agrega una propiedad a una entidad externa.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pDatosEntExternas">Datos externos</param>
        /// <param name="pFacetadoDS">DataSet con los datos externos</param>
        /// <param name="pEstiloProp">Estilo de la propiedad a agregar</param>
        private void AgregarPropiedadEntidadExternas(ElementoOntologia pEntidad, Dictionary<KeyValuePair<string, string>, object[]> pDatosEntExternas, FacetadoDS pFacetadoDS, EstiloPlantillaEspecifProp pEstiloProp)
        {
            if (pEstiloProp.PropiedadesAuxiliares != null && pEstiloProp.PropiedadesAuxiliares.Count > 0)
            {
                Propiedad propiedad = pEntidad.ObtenerPropiedad(pEstiloProp.NombreRealPropiedad);
                propiedad.ElementoOntologia = pEntidad;

                DataRow[] filasProp = pFacetadoDS.Tables[0].Select("s='" + pEntidad.ID + "' AND p='" + pEstiloProp.NombreRealPropiedad + "'");

                foreach (DataRow filaProp in filasProp)
                {
                    string entidadHijaID = (string)filaProp["o"];
                    ElementoOntologia instanciaEntidad = pEntidad.Ontologia.GetEntidadTipo(propiedad.Rango, true);
                    instanciaEntidad.ID = entidadHijaID;
                    instanciaEntidad.EspecifEntidad.NombreLectura = "";
                    propiedad.ListaValores.Add(entidadHijaID, instanciaEntidad);
                    pEntidad.EntidadesRelacionadas.Add(instanciaEntidad);

                    foreach (EstiloPlantillaEspecifProp estiloHijo in pEstiloProp.PropiedadesAuxiliares)
                    {
                        AgregarPropiedadEntidadExternas(instanciaEntidad, pDatosEntExternas, pFacetadoDS, estiloHijo);
                    }
                }
            }
            else
            {
                DataRow[] filasProp = pFacetadoDS.Tables[0].Select("s='" + pEntidad.ID + "' AND p='" + pEstiloProp.NombreRealPropiedad + "'");

                if (filasProp.Length > 0)
                {
                    Propiedad propiedadAux = pEntidad.ObtenerPropiedad(pEstiloProp.NombreRealPropiedad);
                    propiedadAux.ElementoOntologia = pEntidad;

                    Dictionary<string, List<string>> listaValores = FacetadoCN.ObtenerObjetosDataSetSegunPropiedadConIdioma(pFacetadoDS, pEntidad.ID, pEstiloProp.NombreRealPropiedad);

                    foreach (string idioma in listaValores.Keys)
                    {
                        foreach (string valor in listaValores[idioma])
                        {
                            if (string.IsNullOrEmpty(idioma))
                            {
                                propiedadAux.ListaValores.Add(valor, null);
                            }
                            else
                            {
                                if (!propiedadAux.ListaValoresIdioma.ContainsKey(idioma))
                                {
                                    propiedadAux.ListaValoresIdioma.Add(idioma, new Dictionary<string, ElementoOntologia>());
                                }

                                propiedadAux.ListaValoresIdioma[idioma].Add(valor, null);
                            }
                        }
                    }

                    if (pEstiloProp.SelectorEntidad != null)
                    {
                        propiedadAux.Tipo = TipoPropiedad.ObjectProperty;
                        AgregarPropiedadesEntidadesExternas(pEntidad, pDatosEntExternas, pFacetadoDS);
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene todos los recursos vinculados de un documento
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad</param>
        /// <param name="pPropiedad">Propiedad a tratar</param>
        /// <param name="pGestor">Gestor</param>
        private void ObtenerRecursosVinculadosDocumento(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor)
        {
            Documento documento = (Documento)pElementoGnoss;

            foreach (Guid documentoVincID in documento.DocumentosVinculados)
            {
                if (documento.GestorDocumental.ListaDocumentos.ContainsKey(documentoVincID))
                {
                    AgregarRelacionDocumentoVinculadoEntidad(pEntidad, pPropiedad, pGestor, documento.GestorDocumental.ListaDocumentos[documentoVincID]);
                }
            }
        }

        #endregion

        #region Métodos públicos

        /// <summary>
        /// Obtiene la entidad pEntidad y todas sus relaciones.
        /// </summary>
        /// <param name="pEntidad">Entidad que se va a obtener.</param>
        /// <param name="pElementoGnoss">Elemento gnoss que representa la entidad</param>
        /// <param name="pEspecializacion">Indica si la entidad será especilización de otra.</param>
        /// <param name="pGestor">Gestor de entidades</param>
        public override void ObtenerEntidad(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, bool pEspecializacion, GestionGnoss pGestor)
        {
            if (pElementoGnoss is CategoriaTesauro)
                base.ObtenerEntidad(pEntidad, pElementoGnoss, ((CategoriaTesauro)pElementoGnoss).FilaCategoria, pEspecializacion, pGestor);
            else if (pElementoGnoss is Documento)
            {
                ConfigurarFechaPublicacionDocumento((Documento)pElementoGnoss);
                base.ObtenerEntidad(pEntidad, pElementoGnoss, ((Documento)pElementoGnoss).FilaDocumento, pEspecializacion, pGestor);
                ObtenerTagsDocumento(pEntidad, pElementoGnoss, UtilImportarExportar.ObtenerPropiedadDeNombre(UtilImportarExportar.PROPIEDAD_SIOC_TOPIC, pEntidad.Propiedades), pGestor);
            }
            else if (pElementoGnoss is ElementoGnoss)
                base.ObtenerEntidad(pEntidad, pElementoGnoss, ((ElementoGnoss)pElementoGnoss).FilaElemento, pEspecializacion, pGestor);
        }

        #endregion

        #endregion

        #region Propiedades

        /// <summary>
        /// DataSet de proyecto con la presentación para los recursos.
        /// </summary>
        public DataWrapperProyecto ProyectoPresentacionDS
        {
            get
            {
                return mDataWrapperProyecto;
            }
            set
            {
                mDataWrapperProyecto = value;
            }
        }

        /// <summary>
        /// Identidad actual conectada
        /// </summary>
        public Identidad IdentidadActual { get; set; }

        /// <summary>
        /// Proyecto actual.
        /// </summary>
        public Elementos.ServiciosGenerales.Proyecto ProyectoSeleccionado { get; set; }

        #endregion

        #region Dispose

        /// <summary>
        /// Destructor
        /// </summary>
        ~ExportadorWiki()
        {
            //Libero los recursos
            Dispose(false);
        }

        #endregion
    }
}
