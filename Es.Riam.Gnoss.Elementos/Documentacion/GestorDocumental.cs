using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.RDF.Model;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Documentacion.AddToGnoss;
using Es.Riam.Gnoss.Elementos.Documentacion.FichaDocumento;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.Flujos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Interfaces;
using Es.Riam.Interfaces.Observador;
using Es.Riam.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Es.Riam.Gnoss.Elementos.Documentacion
{
    /// <summary>
    /// Clase para gestionar los documentos en gnoss
    /// </summary>
    [Serializable]
    public class GestorDocumental : GestionGnoss, ISerializable, IDisposable
    {
        #region Constantes

        private string COLA_NEWSLETTER = "ColaNewsletter";
        private string EXCHANGE = "";

        #endregion

        #region Miembros estáticos

        /// <summary>
        /// Prefijo para el valor de los combos de un identificador de usuario.
        /// </summary>
        public static string prefijoBRUsuario = "U_";

        /// <summary>
        /// Prefijo para el valor de los combos de un identificador de organización.
        /// </summary>
        public static string prefijoBROrganizacion = "O_";

        #endregion

        #region Miembros

        /// <summary>
        /// Lista de palabras
        /// </summary>
        private Dictionary<Guid, Documento> mListaDocumentos;

        /// <summary>
        /// Gestor de identidades para los documentos
        /// </summary>
        private GestionIdentidades mGestorIdentidades;

        /// <summary>
        /// DataSet de RDF.
        /// </summary>
        private RdfDS mRdfDS;

        /// <summary>
        /// Gestor de comentarios de documento.
        /// </summary>
        private GestionComentariosDocumento mGestionComentariosDocumento;

        /// <summary>
        /// Gestor de votos de documento.
        /// </summary>
        private GestionVotosDocumento mGestorVotos;

        /// <summary>
        /// Gestor de tesauro
        /// </summary>
        private GestionTesauro mGestorTesauro;

        /// <summary>
        /// Lista de documentos destacados.
        /// </summary>
        private List<Guid> mListaDocumentosDestacados;

        /// <summary>
        /// Lista (ID Documento antigüo)-(ID Documento Nuevo) de los documentos que han sido duplicados dentro de la función DuplicarDocumentos
        /// </summary>
        private List<KeyValuePair<Guid, Guid>> mListaDocumentosCopiados = new List<KeyValuePair<Guid, Guid>>();

        /// <summary>
        /// Lista con los documento que se han subido al servidor
        /// </summary>
        private Dictionary<Guid, KeyValuePair<string, TiposDocumentacion>> mListaDocumentosSubidosServidor;

        /// <summary>
        /// Lista con los elementos que están subidos al servidor y deben eliminarse
        /// </summary>
        private Dictionary<Guid, KeyValuePair<string, TiposDocumentacion>> mListaDocumentosAEliminarDelServidor;

        /// <summary>
        /// Lista de documentos de tipo Web.
        /// </summary>
        private Dictionary<Guid, DocumentoWeb> mListaDocumentosWeb;

        private Guid? mCategoriaTesauroPrivadaID = null;

        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructores

        /// <summary>
        /// Constructor a partir de un dataset de documentación
        /// </summary>
        /// <param name="pDocumentacionDW">Dataset de la documentación</param>
        public GestorDocumental(DataWrapperDocumentacion pDocumentacionDW, LoggingService loggingService, EntityContext entityContext, ILogger<GestorDocumental> logger, ILoggerFactory loggerFactory)
            : base(pDocumentacionDW)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            CargarGestor();
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="pInfo">Datos serializados</param>
        /// <param name="pContext">Contexto de serialización</param>
        protected GestorDocumental(SerializationInfo pInfo, StreamingContext pContext)
            : base(pInfo, pContext)
        {
            //mLoggingService = loggingService;
            //mEntityContext = entityContext;
            mGestionComentariosDocumento = (GestionComentariosDocumento)pInfo.GetValue("GestionComentarios", typeof(GestionComentariosDocumento));
            mGestorTesauro = (GestionTesauro)pInfo.GetValue("GestorTesauro", typeof(GestionTesauro));
            mGestorVotos = (GestionVotosDocumento)pInfo.GetValue("GestorVotos", typeof(GestionVotosDocumento));
            mGestorIdentidades = (GestionIdentidades)pInfo.GetValue("GestorIdentidades", typeof(GestionIdentidades));

            CargarGestor();
        }

        #endregion

        #region Métodos

        #region Documento Generales

        /// <summary>
        /// Agrega documentación
        /// </summary>
        /// <param name="pRuta">Ruta del documento</param>
        /// <param name="pTitulo">Título del documento</param>
        /// <param name="pDescripcion">Descripción del documento</param>
        /// <param name="pListaTags">Lista de tags del documento</param>
        /// <param name="pTipo">Tipo de documento</param>
        /// <param name="pTipoEntidad">Tipo de la entidad a la que está vinculado el documento</param>
        /// <param name="pElementoVinculadoID">Identificador del elemento al que está vinculado el documento</param>
        /// <param name="pBorrador">TRUE si es borrador</param>
        /// <param name="pCreadorEsAutor">TRUE si el creador es también el autor del documento</param>
        /// <param name="pAutor">Autor del documento</param>
        /// <param name="pDocPublico">TRUE si es un documento público</param>
        /// <returns>Documento</returns>
        public Documento AgregarDocumento(string pRuta, string pTitulo, string pDescripcion, string pListaTags, TiposDocumentacion pTipo, TipoEntidadVinculadaDocumento pTipoEntidad, Guid pElementoVinculadoID, bool pBorrador, bool pCreadorEsAutor, string pAutor, bool pDocPublico, Guid pOrganizacionID, Guid pIdentidadID)
        {
            return AgregarDocumento(pRuta, pTitulo, pDescripcion, pListaTags, pTipo, pTipoEntidad, true, pElementoVinculadoID, pBorrador, pCreadorEsAutor, pAutor, pDocPublico, pOrganizacionID, pIdentidadID);
        }

        /// <summary>
        /// Agrega documentación
        /// </summary>
        /// <param name="pRuta">Ruta del documento</param>
        /// <param name="pTitulo">Título del documento</param>
        /// <param name="pDescripcion">Descripción del documento</param>
        /// <param name="pListaTags">Lista de tags del documento</param>
        /// <param name="pTipo">Tipo de documento</param>
        /// <param name="pTipoEntidad">Tipo de la entidad a la que está vinculado el documento</param>
        /// <param name="pElementoVinculadoID">Identificador del elemento al que está vinculado el documento</param>
        /// <param name="pCompartir">TRUE si se puede compartir el documento</param>
        /// <param name="pBorrador">TRUE si es borrador</param>
        /// <param name="pCreadorEsAutor">TRUE si el creador es también el autor del documento</param>
        /// <param name="pAutor">Autor del documento</param>
        /// <param name="pDocPublico">TRUE si es un documento público</param>
        /// <returns>Documento</returns>
        public Documento AgregarDocumento(string pRuta, string pTitulo, string pDescripcion, string pListaTags, TiposDocumentacion pTipo, TipoEntidadVinculadaDocumento pTipoEntidad, Guid pElementoVinculadoID, bool pCompartir, bool pBorrador, bool pCreadorEsAutor, string pAutor, bool pDocPublico, Guid pOrganizacionID, Guid pIdentidadID)
        {
            return AgregarDocumento(pRuta, pTitulo, pDescripcion, pListaTags, pTipo, pTipoEntidad, true, pElementoVinculadoID, pCompartir, pBorrador, pCreadorEsAutor, pAutor, pDocPublico, pOrganizacionID, pIdentidadID);
        }

        /// <summary>
        /// Agrega un documento temporal (NO lo agrega al DataSet)
        /// </summary>
        /// <param name="pRuta">Ruta del documento</param>
        /// <param name="pTitulo">Título del documento</param>
        /// <param name="pDescripcion">Descripción del documento</param>
        /// <param name="pListaTags">Lista de tags del documento</param>
        /// <param name="pTipo">Tipo de documento</param>
        /// <param name="pTipoEntidad">Tipo de la entidad a la que está vinculado el documento</param>
        /// <param name="pElementoVinculadoID">Identificador del elemento al que está vinculado el documento</param>
        /// <param name="pBorrador">TRUE si es borrador</param>
        /// <param name="pCreadorEsAutor">TRUE si el creador es también el autor del documento</param>
        /// <param name="pAutor">Autor del documento</param>
        /// <param name="pDocPublico">TRUE si es un documento público</param>
        /// <returns>Documento</returns>
        public Documento AgregarDocumentoTemporal(string pRuta, string pTitulo, string pDescripcion, string pListaTags, TiposDocumentacion pTipo, TipoEntidadVinculadaDocumento pTipoEntidad, Guid pElementoVinculadoID, bool pBorrador, bool pCreadorEsAutor, string pAutor, bool pDocPublico, Guid pOrganizacionID, Guid pIdentidadID)
        {
            return AgregarDocumento(pRuta, pTitulo, pDescripcion, pListaTags, pTipo, pTipoEntidad, false, pElementoVinculadoID, true, pBorrador, pCreadorEsAutor, pAutor, pDocPublico, pOrganizacionID, pIdentidadID);
        }

        /// <summary>
        /// Agrega un documento temporal (NO lo agrega al DataSet)
        /// </summary>
        /// <param name="pID">ID del documento</param>
        /// <param name="pRuta">Ruta del documento</param>
        /// <param name="pTitulo">Título del documento</param>
        /// <param name="pDescripcion">Descripción del documento</param>
        /// <param name="pListaTags">Lista de tags del documento</param>
        /// <param name="pTipo">Tipo de documento</param>
        /// <param name="pTipoEntidad">Tipo de la entidad a la que está vinculado el documento</param>
        /// <param name="pElementoVinculadoID">Identificador del elemento al que está vinculado el documento</param>
        /// <param name="pBorrador">TRUE si es borrador</param>
        /// <param name="pCreadorEsAutor">TRUE si el creador es también el autor del documento</param>
        /// <param name="pAutor">Autor del documento</param>
        /// <param name="pDocPublico">TRUE si es un documento público</param>
        /// <returns>Documento</returns>
        public Documento AgregarDocumentoTemporal(Guid pID, string pRuta, string pTitulo, string pDescripcion, string pListaTags, TiposDocumentacion pTipo, TipoEntidadVinculadaDocumento pTipoEntidad, Guid pElementoVinculadoID, bool pBorrador, bool pCreadorEsAutor, string pAutor, bool pDocPublico, Guid pOrganizacionID, Guid pProyectoID)
        {
            return AgregarDocumento(pID, pRuta, pTitulo, pDescripcion, pListaTags, pTipo, pTipoEntidad, false, pElementoVinculadoID, true, pBorrador, pCreadorEsAutor, pAutor, pDocPublico, pOrganizacionID, pProyectoID);
        }

        /// <summary>
        /// Agrega documentación
        /// </summary>
        /// <param name="pRuta">Ruta del documento</param>
        /// <param name="pTitulo">Título del documento</param>
        /// <param name="pDescripcion">Descripción del documento</param>
        /// <param name="pListaTags">Lista de tags del documento</param>
        /// <param name="pTipo">Tipo de documento</param>
        /// <param name="pTipoEntidad">Tipo de la entidad a la que está vinculado el documento</param>
        /// <param name="pAgregarAlDataSet">TRUE si se debe agregar al dataset</param>
        /// <param name="pElementoVinculadoID">Identificador del elemento al que está vinculado el documento</param>
        /// <param name="pBorrador">TRUE si es borrador</param>
        /// <param name="pCreadorEsAutor">TRUE si el creador es también el autor del documento</param>
        /// <param name="pAutor">Autor del documento</param>
        /// <param name="pDocPublico">TRUE si es un documento público</param>
        /// <returns>Documento</returns>
        private Documento AgregarDocumento(string pRuta, string pTitulo, string pDescripcion, string pListaTags, TiposDocumentacion pTipo, TipoEntidadVinculadaDocumento pTipoEntidad, bool pAgregarAlDataSet, Guid pElementoVinculadoID, bool pBorrador, bool pCreadorEsAutor, string pAutor, bool pDocPublico, Guid pOrganizacionID, Guid pIdentidadID)
        {
            return AgregarDocumento(pRuta, pTitulo, pDescripcion, pListaTags, pTipo, pTipoEntidad, pAgregarAlDataSet, pElementoVinculadoID, true, pBorrador, pCreadorEsAutor, pAutor, pDocPublico, pOrganizacionID, pIdentidadID);
        }

        /// <summary>
        /// Agrega documentación
        /// </summary>
        /// <param name="pRuta">Ruta del documento</param>
        /// <param name="pTitulo">Título del documento</param>
        /// <param name="pDescripcion">Descripción del documento</param>
        /// <param name="pListaTags">Lista de tags del documento</param>
        /// <param name="pTipo">Tipo de documento</param>
        /// <param name="pTipoEntidad">Tipo de la entidad a la que está vinculado el documento</param>
        /// <param name="pAgregarAlDataSet">TRUE si se debe agregar al dataset</param>
        /// <param name="pElementoVinculadoID">Identificador del elemento al que está vinculado el documento</param>
        /// <param name="pCompartir">TRUE si se puede compartir el documento</param>
        /// <param name="pBorrador">TRUE si es borrador</param>
        /// <param name="pCreadorEsAutor">TRUE si el creador es también el autor del documento</param>
        /// <param name="pAutor">Autor del documento</param>
        /// <param name="pDocPublico">TRUE si es un documento público</param>
        /// <returns>Documento</returns>
        public Documento AgregarDocumento(string pRuta, string pTitulo, string pDescripcion, string pListaTags, TiposDocumentacion pTipo, TipoEntidadVinculadaDocumento pTipoEntidad, bool pAgregarAlDataSet, Guid pElementoVinculadoID, bool pCompartir, bool pBorrador, bool pCreadorEsAutor, string pAutor, bool pDocPublico, Guid pOrganizacionID, Guid pIdentidadID)
        {
            return AgregarDocumento(Guid.NewGuid(), pRuta, pTitulo, pDescripcion, pListaTags, pTipo, pTipoEntidad, pAgregarAlDataSet, pElementoVinculadoID, pCompartir, pBorrador, pCreadorEsAutor, pAutor, pDocPublico, pOrganizacionID, pIdentidadID);
        }

        /// <summary>
        /// Agrega documentación
        /// </summary>
        /// <param name="pID">Identificador del documento</param>
        /// <param name="pRuta">Ruta del documento</param>
        /// <param name="pTitulo">Título del documento</param>
        /// <param name="pDescripcion">Descripción del documento</param>
        /// <param name="pListaTags">Lista de tags del documento</param>
        /// <param name="pTipo">Tipo de documento</param>
        /// <param name="pTipoEntidad">Tipo de la entidad a la que está vinculado el documento</param>
        /// <param name="pAgregarAlDataSet">TRUE si se debe agregar al dataset</param>
        /// <param name="pElementoVinculadoID">Identificador del elemento al que está vinculado el documento</param>
        /// <param name="pCompartir">TRUE si se puede compartir el documento</param>
        /// <param name="pBorrador">TRUE si es borrador</param>
        /// <param name="pCreadorEsAutor">TRUE si el creador es también el autor del documento</param>
        /// <param name="pAutor">Autor del documento</param>
        /// <param name="pDocPublico">TRUE si es un documento público</param>
        /// <param name="pOrganizacionIdentidadID">Organizacion de la identidad que agrega el documento</param>
        /// <param name="pIdentidadID">Identidad que agrega el documento</param>
        /// <returns>Documento</returns>
        public Documento AgregarDocumento(Guid pID, string pRuta, string pTitulo, string pDescripcion, string pListaTags, TiposDocumentacion pTipo, TipoEntidadVinculadaDocumento pTipoEntidad, bool pAgregarAlDataSet, Guid pElementoVinculadoID, bool pCompartir, bool pBorrador, bool pCreadorEsAutor, string pAutor, bool pDocPublico, Guid pOrganizacionIdentidadID, Guid pIdentidadID)
        {
            List<string> listaTagsTemp = UtilCadenas.SepararTexto(pListaTags);
            List<string> listaTagsLimpios = new List<string>();
            pListaTags = "";

            foreach (string tag in listaTagsTemp)
            {
                string tagLimpio = tag.Replace("http://", "").Replace("https://", "");
                if (tagLimpio.Contains("/"))
                {
                    tagLimpio = tagLimpio.Substring(0, tagLimpio.IndexOf('/'));
                }
                if (!listaTagsLimpios.Contains(tagLimpio))
                {
                    pListaTags += tagLimpio + ",";
                    listaTagsLimpios.Add(tagLimpio);
                }
            }

            if (pListaTags.Length > 0)
            {
                pListaTags = pListaTags.Substring(0, pListaTags.Length - 1);
            }

            AD.EntityModel.Models.Documentacion.Documento documento = new AD.EntityModel.Models.Documentacion.Documento();

            documento.DocumentoID = pID;

            documento.OrganizacionID = pOrganizacionIdentidadID;
            documento.Titulo = pTitulo;
            documento.Tipo = (short)pTipo;
            documento.Descripcion = pDescripcion;
            documento.NombreElementoVinculado = "Wiki2";
            documento.Protegido = false;

            if (pElementoVinculadoID != Guid.Empty)
            {
                documento.ElementoVinculadoID = pElementoVinculadoID;
            }
            documento.TipoEntidad = (short)pTipoEntidad;

            if (pTipo == TiposDocumentacion.FicheroServidor || pTipo == TiposDocumentacion.Imagen || pTipo == TiposDocumentacion.Video)
            {
                documento.Enlace = Path.GetFileName(pRuta);
            }
            else
            {
                documento.Enlace = pRuta;
            }
            documento.FechaCreacion = DateTime.Now;
            documento.CreadorID = pIdentidadID;
            documento.CompartirPermitido = pCompartir;

            documento.Borrador = pBorrador;
            documento.CreadorEsAutor = pCreadorEsAutor;
            documento.NumeroTotalConsultas = 0;
            documento.NumeroTotalDescargas = 0;
            documento.NumeroTotalVotos = 0;
            documento.NumeroComentariosPublicos = 0;
            documento.Publico = pDocPublico;
            documento.Valoracion = 0;
            documento.Visibilidad = 0;

            if (pAutor == null || pAutor == string.Empty)
            {
                documento.Autor = null;
            }
            else
            {
                documento.Autor = pAutor;
            }
            documento.FechaModificacion = documento.FechaCreacion;
            documento.UltimaVersion = true;
            documento.Eliminado = false;

            Documento doc = new Documento(documento, this);
            doc.Tags = pListaTags;

            if (!ListaDocumentos.ContainsKey(doc.Clave))
            {
                this.ListaDocumentos.Add(doc.Clave, doc);
            }

            if (doc.TipoDocumentacion == TiposDocumentacion.Hipervinculo)
            {
                if (doc.EsVideoIncrustado)
                {
                    doc.FilaDocumento.Tipo = (short)TiposDocumentacion.Video;
                }
                else if (doc.EsPresentacionIncrustada)
                {
                    doc.FilaDocumento.Tipo = (short)TiposDocumentacion.FicheroServidor;
                }
            }

            if (pAgregarAlDataSet)
            {
                DataWrapperDocumentacion.ListaDocumento.Add(documento);
                mEntityContext.Documento.Add(documento);
            }
            return doc;
        }

        /// <summary>
        /// Agrega las newsletter
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>       
        /// <param name="pNewsletter">Código de la newsletter</param>
        /// <returns>Documento</returns>
        public void AgregarNewsletterDocumento(Guid pDocumentoID, string pNewsletter, string pNewsletterTemporal)
        {
            AD.EntityModel.Models.Documentacion.DocumentoNewsletter dnr = new AD.EntityModel.Models.Documentacion.DocumentoNewsletter();

            dnr.DocumentoID = pDocumentoID;
            dnr.Newsletter = pNewsletter;
            dnr.NewsletterTemporal = pNewsletterTemporal;

            DataWrapperDocumentacion.ListaDocumentoNewsLetter.Add(dnr);
            mEntityContext.DocumentoNewsletter.Add(dnr);
        }

        /// <summary>
        /// Agrega un documento temporal al DataSet
        /// </summary>
        /// <param name="pDocumento">Documento que se agrega</param>
        public void AgregarDocumentoAlDataSet(Documento pDocumento)
        {
            DataWrapperDocumentacion.ListaDocumento.Add(pDocumento.FilaDocumento);
            mEntityContext.Documento.Add(pDocumento.FilaDocumento);
        }

        /// <summary>
        /// Elimina un documento de la documentación
        /// </summary>
        /// <param name="pDocumento">Documento que se debe eliminar</param>
        public void EliminarDocumento(Documento pDocumento)
        {
            Guid id = pDocumento.Clave;
            // David: Siempre se eliminan todas las agregaciones con tesauro que tuviera el documento
            foreach (AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro fila in DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(doc => doc.DocumentoID.Equals(pDocumento.Clave)))
            {
                if (GestorTesauro != null && GestorTesauro.ListaCategoriasTesauro.ContainsKey(fila.CategoriaTesauroID))
                {
                    GestorTesauro.ListaCategoriasTesauro[fila.CategoriaTesauroID].Documentos.Remove(ListaDocumentos[pDocumento.Clave]);
                }
                mEntityContext.EliminarElemento(fila);
            }

            // David: Siempre se desvincula el documento de la base de recursos del proyecto donde estuviera
            foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos fila in pDocumento.FilaDocumento.DocumentoWebVinBaseRecursos)
            {
                mEntityContext.EliminarElemento(fila);
            }

            if (mEntityContext.Entry(pDocumento.FilaDocumento).State == EntityState.Added || pDocumento.TipoEntidadVinculada == TipoEntidadVinculadaDocumento.Temporal)
            {
                // David: Si la fila se ha añadido en pantalla, se hace una eliminación física
                BorrarFichaBibliografica(pDocumento);
                EliminarPermisosDocumento(pDocumento);
                BorrarHistorialDocumento(pDocumento);

                EliminarVersionesDocumento(pDocumento, false);

                mEntityContext.EliminarElemento(pDocumento.FilaDocumento);
            }
            else
            {
                // David: Si la fila se ha cargado de la base de datos, se debe hacer una eliminación lógica
                EliminarDocumentoLogicamente(pDocumento);
            }

            // David: Lo último que se haces se borrar de las listas ya que estas se utilizan en los subprocedimientos
            //        y el objeto pDocumento debe estar en la lista
            ListaDocumentos.Remove(id);
            Hijos.Remove(pDocumento);
        }

        /// <summary>
        /// Establece la Base de Recursos actual.
        /// </summary>
        /// <param name="pBaseRecursosID">ID de la Base de Recursos</param>
        public void EstablecerBaseRecursosActual(Guid pBaseRecursosID)
        {
            DataWrapperDocumentacion docAuxDW = new DataWrapperDocumentacion();
            docAuxDW.Merge(DataWrapperDocumentacion);
            this.DataWrapperDocumentacion.ListaBaseRecursos.Remove(docAuxDW.ListaBaseRecursos.First(baseRec => baseRec.BaseRecursosID.Equals(pBaseRecursosID)));

            this.DataWrapperDocumentacion.ListaBaseRecursos.Insert(0, docAuxDW.ListaBaseRecursos.First(baseRec => baseRec.BaseRecursosID.Equals(pBaseRecursosID)));
        }

        #endregion

        #region Ficha Bibliografica

        /// <summary>
        /// Agregar la ficha bibliografica a un documento.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pFichaBibliografica">Ficha bibliográfica</param>
        public void AgregarFichaBibliografica(Documento pDocumento, FichaBibliografica pFichaBibliografica)
        {
            foreach (AtributoBibliografico atributoBiblio in pFichaBibliografica.Atributos.Values)
            {
                AD.EntityModel.Models.Documentacion.DocumentoAtributoBiblio filaAtributoBiblo = new AD.EntityModel.Models.Documentacion.DocumentoAtributoBiblio();
                filaAtributoBiblo.AtributoID = atributoBiblio.AtributoID;
                filaAtributoBiblo.DocumentoID = pDocumento.Clave;
                filaAtributoBiblo.Valor = atributoBiblio.Valor;
                filaAtributoBiblo.FichaBibliograficaID = pFichaBibliografica.FichaBibliograficaID;

                DataWrapperDocumentacion.ListaDocumentoAtributoBiblio.Add(filaAtributoBiblo);
            }

            List<AD.EntityModel.Models.Documentacion.Documento> filasDocumentos = DataWrapperDocumentacion.ListaDocumento.Where(doc => doc.DocumentoID.Equals(pDocumento.Clave)).ToList();
            if (filasDocumentos.Count > 0)
            {
                filasDocumentos[0].FichaBibliograficaID = pFichaBibliografica.FichaBibliograficaID;
            }



        }

        /// <summary>
        /// Agregar la ficha bibliografica a un documento.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        public void BorrarFichaBibliografica(Documento pDocumento)
        {
            List<AD.EntityModel.Models.Documentacion.DocumentoAtributoBiblio> filasAtributos = DataWrapperDocumentacion.ListaDocumentoAtributoBiblio.Where(item => item.DocumentoID.Equals(pDocumento.Clave)).ToList();

            foreach (AD.EntityModel.Models.Documentacion.DocumentoAtributoBiblio filaAtributo in filasAtributos)
            {
                DataWrapperDocumentacion.ListaDocumentoAtributoBiblio.Remove(filaAtributo);
                mEntityContext.EliminarElemento(filaAtributo);
            }

            List<AD.EntityModel.Models.Documentacion.Documento> filasDocumentos = DataWrapperDocumentacion.ListaDocumento.Where(doc => doc.DocumentoID.Equals(pDocumento.Clave)).ToList();
            if (filasDocumentos.Count > 0)
            {
                filasDocumentos.First().FichaBibliograficaID = null;
            }

        }

        #endregion

        #region Permisos Documentos

        /// <summary>
        /// Elimina todas las relaciones de un documento con permisos.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        public void EliminarPermisosDocumento(Documento pDocumento)
        {
            List<AD.EntityModel.Models.Documentacion.DocumentoGrupoUsuario> filasDocGrupoUser = DataWrapperDocumentacion.ListaDocumentoGrupoUsuario.Where(item => item.DocumentoID.Equals(pDocumento.Clave)).ToList();
            foreach (AD.EntityModel.Models.Documentacion.DocumentoGrupoUsuario filaDocGrupoUser in filasDocGrupoUser)
            {
                DataWrapperDocumentacion.ListaDocumentoGrupoUsuario.Remove(filaDocGrupoUser);
                mEntityContext.EliminarElemento(filaDocGrupoUser);
            }

            List<AD.EntityModel.Models.Documentacion.DocumentoRolIdentidad> filasDocRolIdent = DataWrapperDocumentacion.ListaDocumentoRolIdentidad.Where(doc => doc.DocumentoID.Equals(pDocumento.Clave)).ToList();
            foreach (AD.EntityModel.Models.Documentacion.DocumentoRolIdentidad filaDocRolIdent in filasDocRolIdent)
            {
                mEntityContext.EliminarElemento(filaDocRolIdent);
            }
        }

        #endregion

        #region Historial documento

        /// <summary>
        /// Agregar al historial del documento los tags agregados o eliminados.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pListaTags">Tags a agregar</param>
        /// <param name="pAccion">Declara si deben agregarse los tag al historial como eliminados o como agregados</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que se realiza la acción</param>
        public void AgregarEliminarTagsHistorial(Documento pDocumento, List<string> pListaTags, AccionHistorialDocumento pAccion, Guid pProyectoID)
        {
            AgregarEliminarTagsHistorial(pDocumento, pListaTags, pAccion, pProyectoID, Guid.Empty);
        }

        /// <summary>
        /// Agregar al historial del documento los tags agregados o eliminados.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pListaTags">Tags a agregar</param>
        /// <param name="pAccion">Declara si deben agregarse los tag al historial como eliminados o como agregados</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que se realiza la acción</param>
        /// <param name="pIdentidad">Identidad</param>
        public void AgregarEliminarTagsHistorial(Documento pDocumento, List<string> pListaTags, AccionHistorialDocumento pAccion, Guid pProyectoID, Guid pIdentidad)
        {
            if (pDocumento.TipoEntidadVinculada != TipoEntidadVinculadaDocumento.Temporal)
            {
                foreach (string tag in pListaTags)
                {
                    AgregarEliminarHistorialDocumento(pDocumento.Clave, Guid.Empty, pAccion, tag, pProyectoID, pIdentidad);
                }
            }
        }

        /// <summary>
        /// Agrega una categoría del tesauro al historial de un documento agregada o eliminada.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pCategoriaTesauroID">Categoría del tesauro agregada a un documento</param>
        /// <param name="pAccion">Declara si debe agregarse la categoría al historial como eliminada o como agregada</param>
        /// <param name="pIdentidadID"></param>
        /// <param name="pProyectoID"></param>
        public void AgregarEliminarCategoriaTesauroHistorial(Documento pDocumento, Guid pCategoriaTesauroID, AccionHistorialDocumento pAccion, Guid pIdentidadID, Guid pProyectoID)
        {
            if (pDocumento.TipoEntidadVinculada != TipoEntidadVinculadaDocumento.Temporal)
            {
                AgregarEliminarHistorialDocumento(pDocumento.Clave, pCategoriaTesauroID, pAccion, null, pProyectoID, pIdentidadID);
            }
        }

        /// <summary>
        /// Agrega la acción de compartir un documento al historial.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pProyectoID">Proyecto con el que se comparte</param>
        public void AgregarComparticionDocumentoHistorial(Documento pDocumento, Guid pProyectoID, Guid pIdentidadID)
        {
            if (pDocumento.TipoEntidadVinculada != TipoEntidadVinculadaDocumento.Temporal)
            {
                AgregarEliminarHistorialDocumento(pDocumento.Clave, Guid.Empty, AccionHistorialDocumento.CompartirDoc, null, pProyectoID, pIdentidadID);
            }
        }

        /// <summary>
        /// Agrega la acción de compartir un documento al historial.
        /// </summary>
        /// <param name="pDocumentoID">Documento</param>
        /// <param name="pProyectoID">Proyecto del que se descomparte</param>
        public void AgregarDesComparticionDocumentoHistorial(Guid pDocumentoID, Guid pProyectoID, Guid pIdentidadID)
        {
            if (DataWrapperDocumentacion.ListaDocumento.First(doc => doc.DocumentoID.Equals(pDocumentoID)).TipoEntidad != (short)TipoEntidadVinculadaDocumento.Temporal)
            {
                AgregarEliminarHistorialDocumento(pDocumentoID, Guid.Empty, AccionHistorialDocumento.DesCompartirDoc, null, pProyectoID, pIdentidadID);
            }
        }

        /// <summary>
        /// Agrega una entrada al historial indicando que se ha guardado un documento.
        /// </summary>
        /// <param name="pDocumento">Documento guardado</param>
        public void AgregarGuardadoDocumentoHistorial(Documento pDocumento, GnossIdentity pUsuarioActual)
        {
            if (pDocumento.TipoEntidadVinculada != TipoEntidadVinculadaDocumento.Temporal)
            {
                AgregarEliminarHistorialDocumento(pDocumento.Clave, Guid.Empty, AccionHistorialDocumento.GuardarDocumento, null, pUsuarioActual.ProyectoID, pUsuarioActual);
            }
        }

        /// <summary>
        /// Agrega una categoría del tesauro o unos tags al historial de un documento agregados o eliminados.
        /// </summary>
        /// <param name="pDocumentoID">Documento</param>
        /// <param name="pCategoriaTesauroID">Categoría del tesauro agregada a un documento</param>
        /// <param name="pAccion">Declara si debe agregarse la categoría al historial como eliminada o como agregada</param>
        /// <param name="pTag">Tag que se agrega</param>
        /// <param name="pProyectoID">Proyecto en el que se realiza la acción</param>
        private void AgregarEliminarHistorialDocumento(Guid pDocumentoID, Guid pCategoriaTesauroID, AccionHistorialDocumento pAccion, string pTag, Guid pProyectoID, GnossIdentity pUsuarioActual)
        {
            AgregarEliminarHistorialDocumento(pDocumentoID, pCategoriaTesauroID, pAccion, pTag, pProyectoID, pUsuarioActual.IdentidadID);
        }

        /// <summary>
        /// Agrega una categoría del tesauro o unos tags al historial de un documento agregados o eliminados.
        /// </summary>
        /// <param name="pDocumentoID">Documento</param>
        /// <param name="pCategoriaTesauroID">Categoría del tesauro agregada a un documento</param>
        /// <param name="pAccion">Declara si debe agregarse la categoría al historial como eliminada o como agregada</param>
        /// <param name="pTag">Tag que se agrega</param>
        /// <param name="pProyectoID">Proyecto en el que se realiza la acción</param>
        /// <param name="pIdentidadID">Identidad que realiza la acción</param>
        private void AgregarEliminarHistorialDocumento(Guid pDocumentoID, Guid pCategoriaTesauroID, AccionHistorialDocumento pAccion, string pTag, Guid pProyectoID, Guid pIdentidadID)
        {
            AD.EntityModel.Models.Documentacion.HistorialDocumento filaHistorialDoc = new AD.EntityModel.Models.Documentacion.HistorialDocumento();
            filaHistorialDoc.HistorialDocumentoID = Guid.NewGuid();
            filaHistorialDoc.DocumentoID = pDocumentoID;
            filaHistorialDoc.IdentidadID = pIdentidadID;
            filaHistorialDoc.Fecha = DateTime.Now;

            if (pCategoriaTesauroID != Guid.Empty)
            {
                filaHistorialDoc.CategoriaTesauroID = pCategoriaTesauroID;
            }
            else if (pTag != null)
            {
                filaHistorialDoc.TagNombre = pTag;
            }
            filaHistorialDoc.Accion = (int)pAccion;
            filaHistorialDoc.ProyectoID = pProyectoID;
            DataWrapperDocumentacion.ListaHistorialDocumento.Add(filaHistorialDoc);
            mEntityContext.HistorialDocumento.Add(filaHistorialDoc);
        }

        /// <summary>
        /// Elimina el historial de un documento.
        /// </summary>
        /// <param name="pDocumento">Documento del que se debe eliminar el historial.</param>
        public void BorrarHistorialDocumento(Documento pDocumento)
        {
            List<AD.EntityModel.Models.Documentacion.HistorialDocumento> filasHistoDoc = DataWrapperDocumentacion.ListaHistorialDocumento.Where(doc => doc.DocumentoID.Equals(pDocumento.Clave)).ToList();

            foreach (AD.EntityModel.Models.Documentacion.HistorialDocumento filaHistoDoc in filasHistoDoc)
            {
                mEntityContext.EliminarElemento(filaHistoDoc);
                DataWrapperDocumentacion.ListaHistorialDocumento.Remove(filaHistoDoc);
            }
        }

        #endregion

        #region Versiones Documentos

        /// <summary>
        /// Crea una nueva versión a partir de un documento.
        /// </summary>
        /// <param name="pDocumento">Documento del que se va a hacer versión</param>
        /// <param name="pIdentidadActual">Identidad actual</param>
        /// <returns>Version creada</returns>
        public Documento CrearNuevaVersionDocumento(Documento pDocumento, Identidad.Identidad pIdentidadActual, Guid? pDocNuevaVersionID = null, bool pRestaurando = false, bool pEsMejora = false)
        {
            return CrearNuevaVersionDocumento(pDocumento, true, pIdentidadActual, pDocNuevaVersionID, pRestaurando, pEsMejora);
        }

        /// <summary>
        /// Crea una nueva versión a partir de un documento.
        /// </summary>
        /// <param name="pDocumento">Documento del que se va a hacer versión</param>
        /// <param name="pEstablecerRelacionVersion">Indica si se debe establecer la relación de versión entre el documento antiguo y el nuevo, es decir, marcar uno como "NO última version" y el otro como sí" </param>
        /// <param name="pIdentidadActual">Identidad actual</param>
        /// <returns>Version creada</returns>
        public Documento CrearNuevaVersionDocumento(Documento pDocumento, bool pEstablecerRelacionVersion, Identidad.Identidad pIdentidadActual, Guid? pDocNuevaVersionID = null, bool pRestaurando = false, bool pEsMejora = false)
        {
            Guid nuevoDocumentoID = Guid.NewGuid();

            if (pDocNuevaVersionID.HasValue && pDocNuevaVersionID != Guid.Empty)
            {
                nuevoDocumentoID = pDocNuevaVersionID.Value;
            }
            Guid antiguoDocumentoID = pDocumento.Clave;

            //Versiono tabla Documento:
            AD.EntityModel.Models.Documentacion.Documento filaDocumento = new AD.EntityModel.Models.Documentacion.Documento();

            filaDocumento.DocumentoID = nuevoDocumentoID;
            filaDocumento.OrganizacionID = pDocumento.OrganizacionID;
            filaDocumento.CompartirPermitido = pDocumento.CompartirPermitido;

            if (pDocumento.FilaDocumento.ElementoVinculadoID.HasValue)
            {
                filaDocumento.ElementoVinculadoID = pDocumento.ElementoVinculadoID;
            }
            filaDocumento.Titulo = pDocumento.Titulo;
            filaDocumento.Descripcion = pDocumento.Descripcion;
            filaDocumento.Tipo = (short)pDocumento.TipoDocumentacion;
            if (UtilCadenas.EsEnlaceSharepoint(pDocumento.Enlace, "True"))
            {
                filaDocumento.Enlace = pDocumento.FilaDocumento.Enlace;
                pDocumento.FilaDocumento.UltimaVersion = false;
                mEntityContext.Documento.Update(pDocumento.FilaDocumento);
            }
            else if (!filaDocumento.Tipo.Equals(TiposDocumentacion.Hipervinculo))
            {
                filaDocumento.Enlace = filaDocumento.Enlace = pDocumento.FilaDocumento.Enlace; // David: El enlace en las versiones no se mantiene ya que es obligatorio cambiarlo
            }
            filaDocumento.FechaCreacion = DateTime.Now;
            filaDocumento.CreadorID = pDocumento.FilaDocumento.CreadorID;
            filaDocumento.TipoEntidad = (short)pDocumento.TipoEntidadVinculada;
            filaDocumento.NombreCategoriaDoc = pDocumento.FilaDocumento.NombreCategoriaDoc;
            filaDocumento.NombreElementoVinculado = pDocumento.NombreEntidadVinculada;
            filaDocumento.Protegido = false;
            filaDocumento.IdentidadProteccionID = null;
            if (pDocumento.FilaDocumento.ProyectoID.HasValue)
            {
                filaDocumento.ProyectoID = pDocumento.FilaDocumento.ProyectoID;
            }
            filaDocumento.Publico = pDocumento.FilaDocumento.Publico;
            filaDocumento.Borrador = pDocumento.FilaDocumento.Borrador;
            filaDocumento.NumeroTotalDescargas = pDocumento.FilaDocumento.NumeroTotalDescargas;
            filaDocumento.NumeroTotalConsultas = pDocumento.FilaDocumento.NumeroTotalConsultas;
            filaDocumento.NumeroTotalVotos = pDocumento.FilaDocumento.NumeroTotalVotos;
            filaDocumento.NumeroComentariosPublicos = 0;

            if (pDocumento.FilaDocumento.FichaBibliograficaID.HasValue)
            {
                filaDocumento.FichaBibliograficaID = pDocumento.FilaDocumento.FichaBibliograficaID;
            }
            filaDocumento.CreadorEsAutor = pDocumento.FilaDocumento.CreadorEsAutor;
            filaDocumento.Valoracion = pDocumento.FilaDocumento.Valoracion;

            if (pDocumento.FilaDocumento.Autor != null)
            {
                filaDocumento.Autor = pDocumento.Autor;
            }
            filaDocumento.FechaModificacion = DateTime.Now;
            filaDocumento.UltimaVersion = true;
            filaDocumento.Eliminado = false;
            filaDocumento.Visibilidad = 0;
            filaDocumento.EstadoID = pDocumento.FilaDocumento.EstadoID;

            DataWrapperDocumentacion.ListaDocumento.Add(filaDocumento);
            mEntityContext.Documento.Add(filaDocumento);


            Documento nuevoDocumento = new Documento(filaDocumento, this);
            List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> listaDocumentoWebVinBaseRecursos = DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(antiguoDocumentoID)).ToList();
            foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaAuxBaseRecursos in listaDocumentoWebVinBaseRecursos)
            {
                AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaDocWebVinBR = new AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos();
                filaDocWebVinBR.TipoPublicacion = filaAuxBaseRecursos.TipoPublicacion;
                filaDocWebVinBR.LinkAComunidadOrigen = filaAuxBaseRecursos.LinkAComunidadOrigen;
                filaDocWebVinBR.DocumentoID = nuevoDocumentoID;
                filaDocWebVinBR.FechaPublicacion = DateTime.Now;

                filaDocWebVinBR.BaseRecursosID = filaAuxBaseRecursos.BaseRecursosID;
                filaDocWebVinBR.Eliminado = filaAuxBaseRecursos.Eliminado;
                filaDocWebVinBR.NumeroComentarios = 0;
                filaDocWebVinBR.NumeroVotos = 0;
                filaDocWebVinBR.PermiteComentarios = filaAuxBaseRecursos.PermiteComentarios;
                filaDocWebVinBR.IndexarRecurso = true;
                if (!UtilCadenas.EsEnlaceSharepoint(pDocumento.Enlace, "True"))
                {
                    filaDocWebVinBR.PrivadoEditores = pDocumento.FilaDocumentoWebVinBR.PrivadoEditores;
                }
                else
                {
                    filaDocWebVinBR.PrivadoEditores = filaAuxBaseRecursos.PrivadoEditores;
                }

                if (filaAuxBaseRecursos.NivelCertificacionID.HasValue)
                {
                    filaDocWebVinBR.NivelCertificacionID = filaAuxBaseRecursos.NivelCertificacionID;
                    filaDocWebVinBR.FechaCertificacion = filaAuxBaseRecursos.FechaCertificacion;
                }

                if (filaAuxBaseRecursos.PublicadorOrgID.HasValue)
                {
                    filaDocWebVinBR.PublicadorOrgID = filaAuxBaseRecursos.PublicadorOrgID;
                }

                // David: Si el documento se ha compartido en una comunidad, se mantiene la persona que lo ha hecho
                //        En cambio aparecerá como publicado por la persona que ha creado la versión
                if (filaDocWebVinBR.TipoPublicacion > 0)
                {
                    filaDocWebVinBR.IdentidadPublicacionID = filaAuxBaseRecursos.IdentidadPublicacionID;
                }
                else
                {
                    //Obtenemos la identidad de la persona que está editando el recurso en dicha comunidad:
                    Guid identidadEditora = Guid.Empty;
                    //"BaseRecursosID='" + filaAuxBaseRecursos.BaseRecursosID + "'"
                    List<AD.EntityModel.Models.Documentacion.BaseRecursosProyecto> filasBRProy = DataWrapperDocumentacion.ListaBaseRecursosProyecto.Where(baseRec => baseRec.BaseRecursosID.Equals(filaAuxBaseRecursos.BaseRecursosID)).ToList();

                    if (filasBRProy.Count > 0 && pIdentidadActual != null)
                    {
                        identidadEditora = pIdentidadActual.ObtenerIdentidadEnProyectoDeIdentidad((filasBRProy[0]).ProyectoID);
                    }

                    if (identidadEditora != Guid.Empty)
                    {
                        filaDocWebVinBR.IdentidadPublicacionID = identidadEditora;
                        CalcularDatosDesnormalizadosDeDocumentoWebVinBaseRecursos(filaDocWebVinBR, pIdentidadActual.GestorIdentidades.ListaIdentidades[identidadEditora]);
                    }
                    else
                    {
                        filaDocWebVinBR.IdentidadPublicacionID = filaAuxBaseRecursos.IdentidadPublicacionID;
                    }
                }
                mEntityContext.DocumentoWebVinBaseRecursos.Add(filaDocWebVinBR);
                DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Add(filaDocWebVinBR);

                //Creamos también la fila DocumentoWebVinBaseRecursosExtra
                AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursosExtra filaDocWebVinBRExtra = new AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursosExtra();
                filaDocWebVinBRExtra.DocumentoID = nuevoDocumentoID;
                filaDocWebVinBRExtra.BaseRecursosID = filaAuxBaseRecursos.BaseRecursosID;

                filaDocWebVinBRExtra.NumeroConsultas = 0;
                filaDocWebVinBRExtra.NumeroDescargas = 0;
                mEntityContext.DocumentoWebVinBaseRecursosExtra.Add(filaDocWebVinBRExtra);
                DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursosExtra.Add(filaDocWebVinBRExtra);
            }

            //DocumentoWebAgCatTesauro
            List<AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro> listaDocumentoWebAgCatTesauro = DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(doc => doc.DocumentoID.Equals(antiguoDocumentoID)).ToList();
            foreach (AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro filaAux in listaDocumentoWebAgCatTesauro)
            {
                AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro filaDocWebAgCatTes = new AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro();
                filaDocWebAgCatTes.CategoriaTesauroID = filaAux.CategoriaTesauroID;
                filaDocWebAgCatTes.DocumentoID = nuevoDocumentoID;
                filaDocWebAgCatTes.Fecha = DateTime.Now;
                filaDocWebAgCatTes.TesauroID = filaAux.TesauroID;
                filaDocWebAgCatTes.BaseRecursosID = filaAux.BaseRecursosID;
                mEntityContext.DocumentoWebAgCatTesauro.Add(filaDocWebAgCatTes);
                DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Add(filaDocWebAgCatTes);
            }

            //Versiono tabla DocumentoComentario

            // DAVID: Lo dejo comentado porque por ahora se ha decidido no copiar comentarios, pero a saber.... 
            //Altu: Coincido contigo, a saber...

            ////foreach (DocumentacionDS.DocumentoComentarioRow filaAux in (DocumentacionDS.DocumentoComentarioRow[])DocumentacionDS.DocumentoComentario.Select("DocumentoID='" + antiguoDocumentoID + "'"))
            ////{
            ////    DocumentacionDS.DocumentoComentarioRow filaDocComentario = DocumentacionDS.DocumentoComentario.NewDocumentoComentarioRow();
            ////    filaDocComentario.ComentarioID = filaAux.ComentarioID;
            ////    filaDocComentario.DocumentoID = nuevoDocumentoID;
            ////    DocumentacionDS.DocumentoComentario.AddDocumentoComentarioRow(filaDocComentario);
            ////}

            //Versiono tabla DocumentoGrupoUsuario:
            List<AD.EntityModel.Models.Documentacion.DocumentoGrupoUsuario> listaDocumentosGrupoUsuario = DataWrapperDocumentacion.ListaDocumentoGrupoUsuario.Where(item => item.DocumentoID.Equals(antiguoDocumentoID)).ToList();
            foreach (AD.EntityModel.Models.Documentacion.DocumentoGrupoUsuario filaAux in listaDocumentosGrupoUsuario)
            {
                AD.EntityModel.Models.Documentacion.DocumentoGrupoUsuario filaGrupoUsuario = new AD.EntityModel.Models.Documentacion.DocumentoGrupoUsuario();
                filaGrupoUsuario.GrupoUsuarioID = filaAux.GrupoUsuarioID;
                filaGrupoUsuario.DocumentoID = nuevoDocumentoID;
                DataWrapperDocumentacion.ListaDocumentoGrupoUsuario.Add(filaGrupoUsuario);
                mEntityContext.DocumentoGrupoUsuario.Add(filaGrupoUsuario);
            }

            //Versiono tabla DocumentoRolIdentidad:
            List<AD.EntityModel.Models.Documentacion.DocumentoRolIdentidad> listaDocumentoRolIdentidad = DataWrapperDocumentacion.ListaDocumentoRolIdentidad.Where(doc => doc.DocumentoID.Equals(antiguoDocumentoID)).ToList();
            foreach (AD.EntityModel.Models.Documentacion.DocumentoRolIdentidad filaAux in listaDocumentoRolIdentidad)
            {
                AD.EntityModel.Models.Documentacion.DocumentoRolIdentidad filaRolIdent = new AD.EntityModel.Models.Documentacion.DocumentoRolIdentidad();
                filaRolIdent.PerfilID = filaAux.PerfilID;
                filaRolIdent.DocumentoID = nuevoDocumentoID;
                filaRolIdent.Editor = filaAux.Editor;
                filaRolIdent.Documento = filaDocumento;
                mEntityContext.DocumentoRolIdentidad.Add(filaRolIdent);
                DataWrapperDocumentacion.ListaDocumentoRolIdentidad.Add(filaRolIdent);
            }

            //Versiono tabla DocumentoRolGrupoIdentidades:
            List<AD.EntityModel.Models.Documentacion.DocumentoRolGrupoIdentidades> listaDocumentoRolGrupoIdentidades = DataWrapperDocumentacion.ListaDocumentoRolGrupoIdentidades.Where(item => item.DocumentoID.Equals(antiguoDocumentoID)).ToList();
            foreach (AD.EntityModel.Models.Documentacion.DocumentoRolGrupoIdentidades filaAux in listaDocumentoRolGrupoIdentidades)
            {
                AD.EntityModel.Models.Documentacion.DocumentoRolGrupoIdentidades filaRolIdent = new AD.EntityModel.Models.Documentacion.DocumentoRolGrupoIdentidades();
                filaRolIdent.GrupoID = filaAux.GrupoID;
                filaRolIdent.DocumentoID = nuevoDocumentoID;
                filaRolIdent.Editor = filaAux.Editor;
                DataWrapperDocumentacion.ListaDocumentoRolGrupoIdentidades.Add(filaRolIdent);
                mEntityContext.DocumentoRolGrupoIdentidades.Add(filaRolIdent);
            }

            //Versiono tabla DocumentoAtributoBiblio:
            List<AD.EntityModel.Models.Documentacion.DocumentoAtributoBiblio> listaDocumentoAtributoBiblio = DataWrapperDocumentacion.ListaDocumentoAtributoBiblio.Where(item => item.DocumentoID.Equals(antiguoDocumentoID)).ToList();
            foreach (AD.EntityModel.Models.Documentacion.DocumentoAtributoBiblio filaAux in listaDocumentoAtributoBiblio)
            {
                AD.EntityModel.Models.Documentacion.DocumentoAtributoBiblio filaDocAtribuBiblio = new AD.EntityModel.Models.Documentacion.DocumentoAtributoBiblio();
                filaDocAtribuBiblio.DocumentoID = nuevoDocumentoID;
                filaDocAtribuBiblio.AtributoID = filaAux.AtributoID;
                filaDocAtribuBiblio.Valor = filaAux.Valor;
                filaDocAtribuBiblio.FichaBibliograficaID = filaAux.FichaBibliograficaID;
                DataWrapperDocumentacion.ListaDocumentoAtributoBiblio.Add(filaDocAtribuBiblio);
                mEntityContext.DocumentoAtributoBiblio.Add(filaDocAtribuBiblio);
            }

            List<AD.EntityModel.Models.Documentacion.VotoDocumento> listaVotoDocumento = DataWrapperDocumentacion.ListaVotoDocumento.Where(item => item.DocumentoID.Equals(antiguoDocumentoID)).ToList();

            //Versiono tabla VotoDocumento:
            foreach (AD.EntityModel.Models.Documentacion.VotoDocumento filaAux in listaVotoDocumento)
            {
                AD.EntityModel.Models.Documentacion.VotoDocumento filaDocVoto = new AD.EntityModel.Models.Documentacion.VotoDocumento();
                filaDocVoto.DocumentoID = nuevoDocumentoID;
                filaDocVoto.VotoID = filaAux.VotoID;
                filaDocVoto.ProyectoID = pDocumento.ProyectoID;
                DataWrapperDocumentacion.ListaVotoDocumento.Add(filaDocVoto);
                mEntityContext.VotoDocumento.Add(filaDocVoto);

                // David: Se borran los votos de las versiones viejas ya que se han pasado a la nueva versión
                mEntityContext.EliminarElemento(filaAux);
                DataWrapperDocumentacion.ListaVotoDocumento.Remove(filaAux);
            }

            //Versiono tabla DocumentoTipologia:
            List<AD.EntityModel.Models.Documentacion.DocumentoTipologia> listaDocumentoTipologia = DataWrapperDocumentacion.ListaDocumentoTipologia.Where(item => item.DocumentoID.Equals(antiguoDocumentoID)).ToList();
            foreach (AD.EntityModel.Models.Documentacion.DocumentoTipologia filaAux in listaDocumentoTipologia)
            {
                AD.EntityModel.Models.Documentacion.DocumentoTipologia filaDocTipologia = new AD.EntityModel.Models.Documentacion.DocumentoTipologia();
                filaDocTipologia.DocumentoID = nuevoDocumentoID;
                filaDocTipologia.TipologiaID = filaAux.TipologiaID;
                filaDocTipologia.AtributoID = filaAux.AtributoID;
                filaDocTipologia.Valor = filaAux.Valor;
                DataWrapperDocumentacion.ListaDocumentoTipologia.Add(filaDocTipologia);
                mEntityContext.DocumentoTipologia.Add(filaDocTipologia);
            }

            //Versiono tabla HistorialDocumento:
            AD.EntityModel.Models.Documentacion.HistorialDocumento filaHistorialDoc = new AD.EntityModel.Models.Documentacion.HistorialDocumento();
            filaHistorialDoc.HistorialDocumentoID = Guid.NewGuid();
            filaHistorialDoc.IdentidadID = pIdentidadActual.Clave;
            filaHistorialDoc.Fecha = DateTime.Now;
            if (pRestaurando)
            {
                filaHistorialDoc.DocumentoID = nuevoDocumentoID;
                filaHistorialDoc.Accion = (short)AccionHistorialDocumento.RestaurarVersion;
            }
            else
            {
                filaHistorialDoc.DocumentoID = antiguoDocumentoID;
                filaHistorialDoc.Accion = (short)AccionHistorialDocumento.CrearVersion;
            }
            filaHistorialDoc.ProyectoID = pIdentidadActual.FilaIdentidad.ProyectoID;
            mEntityContext.HistorialDocumento.Add(filaHistorialDoc);
            DataWrapperDocumentacion.ListaHistorialDocumento.Add(filaHistorialDoc);
            if (pEstablecerRelacionVersion)
            {
                Guid identidadActualID = pIdentidadActual.Clave;

                EstablecerVersionDocumento(pDocumento, nuevoDocumento, identidadActualID, pEsMejora);
            }
            //TagDocumento
            nuevoDocumento.Tags = pDocumento.Tags;

            //Trato los documentosVinculados:

            List<AD.EntityModel.Models.Documentacion.DocumentoVincDoc> listaDocumentoVinDoc = DataWrapperDocumentacion.ListaDocumentoVincDoc.Where(item => item.DocumentoID.Equals(pDocumento.Clave)).ToList();
            foreach (AD.EntityModel.Models.Documentacion.DocumentoVincDoc filaDocVinc in listaDocumentoVinDoc)
            {
                mEntityContext.EliminarElemento(filaDocVinc);
                DataWrapperDocumentacion.ListaDocumentoVincDoc.Remove(filaDocVinc);
                AD.EntityModel.Models.Documentacion.DocumentoVincDoc newFilaDocVinc = new AD.EntityModel.Models.Documentacion.DocumentoVincDoc();
                newFilaDocVinc.DocumentoID = nuevoDocumento.Clave;
                newFilaDocVinc.DocumentoVincID = filaDocVinc.DocumentoVincID;
                newFilaDocVinc.Fecha = filaDocVinc.Fecha;
                newFilaDocVinc.IdentidadID = filaDocVinc.IdentidadID;

                DataWrapperDocumentacion.ListaDocumentoVincDoc.Add(newFilaDocVinc);
                mEntityContext.DocumentoVincDoc.Add(newFilaDocVinc);
            }

            listaDocumentoVinDoc = DataWrapperDocumentacion.ListaDocumentoVincDoc.Where(item => item.DocumentoVincID.Equals(pDocumento.Clave)).ToList();
            foreach (AD.EntityModel.Models.Documentacion.DocumentoVincDoc filaDocVinc in listaDocumentoVinDoc)
            {
                mEntityContext.EliminarElemento(filaDocVinc);
                DataWrapperDocumentacion.ListaDocumentoVincDoc.Remove(filaDocVinc);
                AD.EntityModel.Models.Documentacion.DocumentoVincDoc newFilaDocVinc = new AD.EntityModel.Models.Documentacion.DocumentoVincDoc();

                newFilaDocVinc.DocumentoVincID = nuevoDocumento.Clave;
                newFilaDocVinc.DocumentoID = filaDocVinc.DocumentoID;
                newFilaDocVinc.Fecha = filaDocVinc.Fecha;
                newFilaDocVinc.IdentidadID = filaDocVinc.IdentidadID;
                if (UtilCadenas.EsEnlaceSharepoint(pDocumento.Enlace, "True"))
                {
                    AD.EntityModel.Models.Documentacion.DocumentoVincDoc newFilaDocVincSP = new AD.EntityModel.Models.Documentacion.DocumentoVincDoc();

                    newFilaDocVincSP.DocumentoVincID = filaDocVinc.DocumentoID;
                    newFilaDocVincSP.DocumentoID = nuevoDocumento.Clave;
                    newFilaDocVincSP.Fecha = filaDocVinc.Fecha;
                    newFilaDocVincSP.IdentidadID = filaDocVinc.IdentidadID;

                    DataWrapperDocumentacion.ListaDocumentoVincDoc.Add(newFilaDocVincSP);
                    mEntityContext.DocumentoVincDoc.Add(newFilaDocVincSP);
                }
                DataWrapperDocumentacion.ListaDocumentoVincDoc.Add(newFilaDocVinc);
                mEntityContext.DocumentoVincDoc.Add(newFilaDocVinc);
            }
            mEntityContext.SaveChanges();
            return nuevoDocumento;
        }

        /// <summary>
        /// Establece la realición de versión entre el documento original y el nuevo documento.
        /// </summary>
        /// <param name="pDocumentoOriginal">Documento original del que se a hecho la versión</param>
        /// <param name="pNuevoDocumento">Nueva versión del documento</param>
        /// <param name="pIdentidadActualID"></param>
        public void EstablecerVersionDocumento(Documento pDocumentoOriginal, Documento pNuevoDocumento, Guid pIdentidadActualID, bool pEsMejora = false)
        {
            Guid nuevoDocumentoID = pNuevoDocumento.Clave;
            Guid antiguoDocumentoID = pDocumentoOriginal.Clave;

            pDocumentoOriginal.FilaDocumento.UltimaVersion = false;
            pDocumentoOriginal.FilaDocumento.FechaModificacion = DateTime.Now;

            //Versiono tabla VersionDocumento:
            //DocumentacionDS.VersionDocumentoRow[] filasAnteriorVersionDoc = (DocumentacionDS.VersionDocumentoRow[])DocumentacionDS.VersionDocumento.Select("DocumentoID='" + antiguoDocumentoID + "'");

            List<AD.EntityModel.Models.Documentacion.VersionDocumento> filasAnteriorVersionDoc = DataWrapperDocumentacion.ListaVersionDocumento.OrderBy(versDoc => versDoc.Version).ToList();

            AD.EntityModel.Models.Documentacion.VersionDocumento filaVersionDoc = new AD.EntityModel.Models.Documentacion.VersionDocumento();
            filaVersionDoc.DocumentoID = nuevoDocumentoID;
            filaVersionDoc.IdentidadID = pIdentidadActualID;
            int numeroVersion = 1;

            if (filasAnteriorVersionDoc.Count > 0)
            {
                //Ya había versiones del documento a versionar
                foreach (AD.EntityModel.Models.Documentacion.VersionDocumento filaAnteriorVersionDoc in filasAnteriorVersionDoc)
                {
                    if (filaAnteriorVersionDoc.Version >= numeroVersion)
                    {
                        numeroVersion = filaAnteriorVersionDoc.Version + 1;
                    }
                    //Actualizamos las versiones anteriores para que dejen de ser "Última versión"
                    //DocumentacionDS.Documento.FindByDocumentoID(filaAnteriorVersionDoc.DocumentoID).UltimaVersion = false;
                }
                filaVersionDoc.DocumentoOriginalID = filasAnteriorVersionDoc[0].DocumentoOriginalID;
            }
            else
            {
                //NO había versiones del documento a versionar
                filaVersionDoc.DocumentoOriginalID = antiguoDocumentoID;
            }
            filaVersionDoc.Version = numeroVersion;

            filaVersionDoc.EstadoVersion = (short)EstadoVersion.Vigente;
            filaVersionDoc.EsMejora = false;
            if (pDocumentoOriginal.FilaDocumento.EstadoID.HasValue)
            {
                filaVersionDoc.EstadoID = pDocumentoOriginal.FilaDocumento.EstadoID.Value;
			}
            
            if (pEsMejora)
            {
                filaVersionDoc.EstadoVersion = (short)EstadoVersion.Pendiente;
                filaVersionDoc.EsMejora = true; 
				if (pDocumentoOriginal.FilaDocumento.EstadoID != null)
                {
                    bool estaEnEstadoFinal = mEntityContext.Estado.Where(x => x.EstadoID.Equals(pDocumentoOriginal.FilaDocumento.EstadoID)).Select(x => x.Tipo).FirstOrDefault() == (short) TipoEstado.Final;
                    // Si esta en estado final, se esta iniciando la mejora
                    if (estaEnEstadoFinal)
                    {
						// le ponemos el estado inicial para que comience de nuevo todo el flujo de trabajo
						Guid flujoID = mEntityContext.Estado.Where(x => x.EstadoID.Equals(pNuevoDocumento.FilaDocumento.EstadoID)).Select(x => x.FlujoID).FirstOrDefault();
						Guid estadoInicial = mEntityContext.Estado.Where(x => x.FlujoID.Equals(flujoID) && x.Tipo.Equals((short)TipoEstado.Inicial)).Select(x => x.EstadoID).FirstOrDefault();
						filaVersionDoc.EstadoID = estadoInicial;
                        filaVersionDoc.MejoraID = Guid.NewGuid();
                        pNuevoDocumento.FilaDocumento.EstadoID = estadoInicial;

                        // marcamos la ultima version a true porque el anterior es el recurso original
						pDocumentoOriginal.FilaDocumento.UltimaVersion = true;                        
					}
                    else
                    {
                        // Le ponemos el mismo estado que tenia y la ultima version a false, ya que sigue siendo una mejora y no el recurso original                        
                        filaVersionDoc.EstadoID = pDocumentoOriginal.FilaDocumento.EstadoID;
						Guid? mejoraID = mEntityContext.VersionDocumento.Where(x => x.DocumentoID.Equals(pDocumentoOriginal.Clave)).Select(x => x.MejoraID).FirstOrDefault();
						if (mejoraID.HasValue && mejoraID != Guid.Empty)
                        {
                            filaVersionDoc.MejoraID = mejoraID;
						}
                        
                        pNuevoDocumento.FilaDocumento.EstadoID = filaVersionDoc.EstadoID;
						pDocumentoOriginal.FilaDocumento.UltimaVersion = false;
                        pDocumentoOriginal.FilaDocumento.VersionDocumento.EstadoVersion = (short)EstadoVersion.Historico;
                    }
					pNuevoDocumento.FilaDocumento.UltimaVersion = false;
				}
            }
            else if(pDocumentoOriginal.FilaDocumento.VersionDocumento != null)
            {
                pDocumentoOriginal.FilaDocumento.VersionDocumento.EstadoVersion = (short)EstadoVersion.Historico;
            }

            mEntityContext.VersionDocumento.Add(filaVersionDoc);
            DataWrapperDocumentacion.ListaVersionDocumento.Add(filaVersionDoc);
        }

        /// <summary>
        /// Crea una nueva versión del documento asignandole los valores
        /// </summary>
        /// <param name="pVersionOrigen">Documento de origen (la última versión activa)</param>
        /// <param name="pVersionDestino">Documento de destino (los valores qeu se asignarán a las versiones nuevas)</param>
        /// <param name="pEntidadVinculadaDoc">Entidad vinculada al documento si la tiene</param>
        /// <param name="pIdentidadActual">Identidad actual</param>
        /// <returns>Documento creado con los valores de VersionDestino</returns>
        public Documento RestaurarVersion(Documento pVersionDestino, Documento pUltimaVersionActual, Identidad.Identidad pIdentidadActual)
        {
            // Crear la nueva versión a partir del documento de origen
            Documento nuevaVersion = CrearNuevaVersionDocumento(pVersionDestino, pIdentidadActual, pRestaurando: true);

            // Establecemos la ultima version anterior a false
            pUltimaVersionActual.FilaDocumento.UltimaVersion = false;
            // Mantenemos el estado id que tenia asignado la ultima version actual
            nuevaVersion.FilaDocumento.EstadoID = pUltimaVersionActual.FilaDocumento.EstadoID;

            mEntityContext.SaveChanges();

            return nuevaVersion;
        }

        /// <summary>
        /// Elimina la información de una versión en concreto
        /// </summary>
        /// <param name="pDocumento">Versión a eliminar</param>
        /// <param name="pEsUltimaVersion">TRUE si es la ultima versión</param>
        public void EliminarVersionDocumento(Documento pDocumento, bool pEsUltimaVersion)
        {
            if (pEsUltimaVersion)
            {
                Documento docVersionAnterior = pDocumento.ListaDocumentosVersionesAnteriores[0];

                // Si es la última versión del documento se deben éliminar las categorias de tesauro de la versión antigua
                // para vincularlo con las de la que era la última versión activa
                foreach (AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro filaAux in DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(doc => doc.DocumentoID.Equals(docVersionAnterior.Clave)))
                {
                    mEntityContext.EliminarElemento(filaAux);
                }

                foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaAux in DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(docVersionAnterior.Clave)))
                {
                    mEntityContext.EliminarElemento(filaAux);
                }

                foreach (AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro filaAux in DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(doc => doc.DocumentoID.Equals(pDocumento.Clave)))
                {
                    AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro filaDocWebAgCatTes = new AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro();
                    filaDocWebAgCatTes.CategoriaTesauroID = filaAux.CategoriaTesauroID;
                    filaDocWebAgCatTes.DocumentoID = docVersionAnterior.Clave;
                    filaDocWebAgCatTes.Fecha = filaAux.Fecha;
                    filaDocWebAgCatTes.TesauroID = filaAux.TesauroID;
                    filaDocWebAgCatTes.BaseRecursosID = filaAux.BaseRecursosID;
                    mEntityContext.DocumentoWebAgCatTesauro.Add(filaDocWebAgCatTes);
                }

                foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaAux in DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(pDocumento.Clave)))
                {
                    AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaDocWebVinBR = new AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos();
                    filaDocWebVinBR.TipoPublicacion = filaAux.TipoPublicacion;
                    filaDocWebVinBR.LinkAComunidadOrigen = filaAux.LinkAComunidadOrigen;
                    filaDocWebVinBR.DocumentoID = docVersionAnterior.Clave;
                    filaDocWebVinBR.FechaPublicacion = filaAux.FechaPublicacion;
                    filaDocWebVinBR.IdentidadPublicacionID = filaAux.IdentidadPublicacionID;
                    filaDocWebVinBR.BaseRecursosID = filaAux.BaseRecursosID;
                    filaDocWebVinBR.Eliminado = filaAux.Eliminado;
                    filaDocWebVinBR.NumeroComentarios = 0;
                    filaDocWebVinBR.NumeroVotos = 0;
                    filaDocWebVinBR.PublicadorOrgID = filaAux.PublicadorOrgID;
                    filaDocWebVinBR.PermiteComentarios = filaAux.PermiteComentarios;
                    filaDocWebVinBR.IndexarRecurso = true;

                    mEntityContext.DocumentoWebVinBaseRecursos.Add(filaDocWebVinBR);
                }

                foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaAux in DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(pDocumento.Clave)))
                {
                    AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursosExtra filaDocWebVinBRExtra = new AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursosExtra();
                    filaDocWebVinBRExtra.DocumentoID = docVersionAnterior.Clave;
                    filaDocWebVinBRExtra.BaseRecursosID = filaAux.BaseRecursosID;

                    filaDocWebVinBRExtra.NumeroConsultas = 0;
                    filaDocWebVinBRExtra.NumeroDescargas = 0;
                    mEntityContext.DocumentoWebVinBaseRecursosExtra.Add(filaDocWebVinBRExtra);
                }

                foreach (CategoriaTesauro categoria in pDocumento.CategoriasTesauro.Values)
                {
                    categoria.Documentos.Remove(pDocumento);
                    categoria.Documentos.Add(docVersionAnterior);
                }

                // Poner la versión anterior como activa
                docVersionAnterior.FilaDocumento.UltimaVersion = true;
                pDocumento.FilaDocumento.UltimaVersion = false;
            }

            // Eliminar la version
            EliminarDocumento(pDocumento);
        }
        public void EliminarVersionDocumento(Documento pDocumento)
        {
            EliminarDocumentoLogicamente(pDocumento);
            VersionDocumento filaVersionDocumento = mEntityContext.VersionDocumento.FirstOrDefault(v => v.DocumentoID.Equals(pDocumento.Clave));
            if(filaVersionDocumento != null)
            {
                mEntityContext.EliminarElemento(filaVersionDocumento);
            }
            mEntityContext.SaveChanges();

            // Reordenar el resto de versiones
            int version = 0;
            foreach(VersionDocumento versionDocumento in mEntityContext.VersionDocumento.Where(v => v.DocumentoOriginalID.Equals(pDocumento.VersionOriginalID)).OrderBy(v => v.Version))
            {
                versionDocumento.Version = version;
                version++;
            }
            mEntityContext.SaveChanges();
        }

        #endregion

        #region Cargas y otros

        /// <summary>
        /// Carga el gestor completo
        /// </summary>
        public new void CargarGestor()
        {
            //Cargo las listas
            foreach (Documento documento in Hijos)
            {
                List<IElementoGnoss> lista = documento.Hijos;
            }
        }

        /// <summary>
        /// Limpia las listas de documentos
        /// </summary>
        public void LimpiarDocumentos()
        {
            if (mHijos != null)
                this.mHijos.Clear();

            if (mListaDocumentos != null)
                this.mListaDocumentos.Clear();

            this.mHijos = null;
            this.mListaDocumentos = null;
        }

        /// <summary>
        /// Carga todos los documentos
        /// </summary>
        public void CargarDocumentos()
        {
            CargarDocumentos(true);
        }

        /// <summary>
        /// Carga todos los documentos.
        /// <param name="pOrdenarPorFecha">True si se ordena por fecha, false en caso contrario</param>
        /// </summary>
        public void CargarDocumentos(bool pOrdenarPorFecha)
        {
            if (mHijos == null)
            {
                mHijos = new List<IElementoGnoss>();
            }

            if (mListaDocumentos == null)
            {
                mListaDocumentos = new Dictionary<Guid, Documento>();
            }

            if (pOrdenarPorFecha)
            {
                if (DataWrapperDocumentacion != null)
                {
                    foreach (AD.EntityModel.Models.Documentacion.Documento filaDocumento in DataWrapperDocumentacion.ListaDocumento.OrderByDescending(doc => doc.FechaCreacion.Value))
                    {
                        Documento documento = new Documento(filaDocumento, this);

                        if (!mHijos.Contains(documento))
                        {
                            mHijos.Add(documento);
                        }

                        if (!mListaDocumentos.ContainsKey(documento.Clave))
                        {
                            mListaDocumentos.Add(documento.Clave, documento);
                        }
                    }
                }
            }
            else
            {
                foreach (AD.EntityModel.Models.Documentacion.Documento filaDocumento in DataWrapperDocumentacion.ListaDocumento)
                {
                    Documento documento = new Documento(filaDocumento, this);

                    if (!mHijos.Contains(documento))
                    {
                        mHijos.Add(documento);
                    }

                    if (!mListaDocumentos.ContainsKey(documento.Clave))
                    {
                        mListaDocumentos.Add(documento.Clave, documento);
                    }
                }
            }
        }

        /// <summary>
        /// Carga los documentos web.
        /// </summary>
        public void CargarDocumentosWeb()
        {
            mListaDocumentosWeb = new Dictionary<Guid, DocumentoWeb>();

            CargarDocumentos(false);
            List<Documento> listaDocAux = new List<Documento>();
            listaDocAux.AddRange(ListaDocumentos.Values);

            foreach (Documento doc in listaDocAux)
            {
                DocumentoWeb documentoWeb = new DocumentoWeb(doc.FilaDocumento, this);

                if (!mListaDocumentosWeb.ContainsKey(documentoWeb.Clave))
                {
                    mListaDocumentosWeb.Add(documentoWeb.Clave, documentoWeb);
                }
            }
        }

        /// <summary>
        /// Carga los documentos que no hayan sido cargados ya
        /// </summary>
        public void CargarNuevosHijos()
        {
            foreach (AD.EntityModel.Models.Documentacion.Documento filaDocumento in DataWrapperDocumentacion.ListaDocumento)
            {//(filaDocumento.RowState != DataRowState.Deleted && filaDocumento.RowState != DataRowState.Detached)
                if (!mListaDocumentos.ContainsKey(filaDocumento.DocumentoID))
                {
                    Documento documento = new Documento(filaDocumento, this);
                    mHijos.Add(documento);
                    mListaDocumentos.Add(documento.Clave, documento);
                }
            }
        }

        /// <summary>
        /// Busca un documento en la documentación
        /// </summary>
        /// <param name="pTitulo">Título del documento que hay que buscar</param>
        /// <returns></returns>
        public Documento BuscarPalabra(string pTitulo)
        {
            Documento documento = null;
            List<AD.EntityModel.Models.Documentacion.Documento> filas = this.DataWrapperDocumentacion.ListaDocumento.Where(doc => doc.Titulo.Equals(pTitulo.Trim().ToLower())).ToList();

            if (filas.Count > 0)
            {
                documento = this.ListaDocumentos[filas.First().DocumentoID];
            }
            return documento;
        }

        /// <summary>
        /// Devuelve true si algún hijo del elemento tiene documentos.
        /// </summary>
        /// <param name="pElemento">Elemento a revisar</param>
        /// <returns>Devuelve true si algún hijo del elemento tiene documentos</returns>
        public bool TienenDocumentacionLosHijos(ElementoGnoss pElemento)
        {
            if (pElemento.Hijos.Count == 0)
            {
                return false;
            }
            bool hijosTienen = false;

            foreach (ElementoGnoss hijo in pElemento.Hijos)
            {
                hijosTienen = hijosTienen || TieneDocumentacionElementoEHijos(hijo);

                if (hijosTienen)
                {
                    break;
                }
            }
            return hijosTienen;
        }

        /// <summary>
        /// Devuelve true si el elemento o algún hijo del elemento tiene documentos.
        /// </summary>
        /// <param name="pElemento">Elemento a revisar</param>
        /// <returns>Devuelve TRUE si algún hijo del elemento tiene documentos</returns>
        public bool TieneDocumentacionElementoEHijos(ElementoGnoss pElemento)
        {
            return TienenDocumentacionLosHijos(pElemento);
        }

        /// <summary>
        /// Obtiene un nuevo valor para la privacidad de un documento según donde se encuentre compartido
        /// </summary>
        /// <param name="pFilaDoc">DocumentacionDS.DocumentoRow del Documento</param>
        /// <param name="pFilaCreador">Datarow de la persona que ha creado el documento</param>
        /// <param name="pFilaOrganizacion">Datarow de la organización donde se está el documento</param>
        /// <param name="pDataWrapperDocumentacion">Dataset temporal de documentación</param>
        /// <param name="pGestionTesauro">Gestor de tesauro con el tesauro del usuario y organización</param>
        /// <param name="pDataWrapperProyecto">Dataset de proyectos</param>
        /// <returns>TRUE si debe ser público, FALSE en caso contrario</returns>
        public bool ObtenerNuevoValorPrivacidadDocumento(AD.EntityModel.Models.Documentacion.Documento pFilaDoc, AD.EntityModel.Models.PersonaDS.Persona pFilaCreador, AD.EntityModel.Models.OrganizacionDS.Organizacion pFilaOrganizacion, DataWrapperDocumentacion pDataWrapperDocumentacion, GestionTesauro pGestionTesauro, DataWrapperProyecto pDataWrapperProyecto)
        {
            bool publico = false;

            // Recorrer los proyectos para ver si están vinculados al documento
            // Si está vinculado en algún proyecto con tipo de acceso Publico o Restringido, entonces el documento será público
            foreach (AD.EntityModel.Models.Documentacion.BaseRecursosProyecto filaBRProy in pDataWrapperDocumentacion.ListaBaseRecursosProyecto)
            {//"DocumentoID = '" + pFilaDoc.DocumentoID + "' AND BaseRecursosID = '" + filaBRProy.BaseRecursosID + "'"
                List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filaDocVinBR = pDataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(pFilaDoc.DocumentoID) && doc.BaseRecursosID.Equals(filaBRProy.BaseRecursosID)).ToList();

                if (filaDocVinBR.Count > 0 && !filaDocVinBR[0].Eliminado)
                {
                    // Obtener el tipo de acceso del proyecto
                    AD.EntityModel.Models.ProyectoDS.Proyecto filaProyecto = pDataWrapperProyecto.ListaProyecto.Where(proyecto => proyecto.OrganizacionID.Equals(filaBRProy.OrganizacionID) && proyecto.ProyectoID.Equals(filaBRProy.ProyectoID)).FirstOrDefault();//FindByOrganizacionIDProyectoID(filaBRProy.OrganizacionID, filaBRProy.ProyectoID);
                    if (filaProyecto != null)
                    {
                        if ((filaProyecto.TipoAcceso == (short)TipoAcceso.Publico || filaProyecto.TipoAcceso == (short)TipoAcceso.Restringido) && filaProyecto.Estado != (short)EstadoProyecto.Cerrado && filaProyecto.Estado != (short)EstadoProyecto.CerradoTemporalmente)
                        {
                            //El proyecto debe ser público o restringido y además no debe estar cerrado ni cerrado temporalmente.
                            publico = true;
                            break;
                        }
                    }
                }
            }

            if (!publico)
            {
                if (pDataWrapperDocumentacion.ListaBaseRecursosUsuario.Count > 0)
                {
                    // Revisar el tesauro del usuario que ha creado el documento
                    // Si no está en ninguna categoria pública, entonces el documento es privado
                    // Si está en alguna categoría privada el valor de la privacidad dependerá de si la persona es buscable
                    Guid IDBaseRecursos = Guid.Empty;
                    List<AD.EntityModel.Models.Documentacion.BaseRecursosUsuario> listaBaseRecursosUsuario = pDataWrapperDocumentacion.ListaBaseRecursosUsuario.Where(baseRec => baseRec.UsuarioID.Equals(pFilaCreador.UsuarioID)).ToList();
                    if (pFilaCreador != null && listaBaseRecursosUsuario.Count > 0)
                    {
                        IDBaseRecursos = listaBaseRecursosUsuario.First().BaseRecursosID;
                    }//"DocumentoID = '" + pFilaDoc.DocumentoID + "' AND BaseRecursosID = '" + IDBaseRecursos + "'"
                    List<AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro> filasDocAgCat = pDataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(doc => doc.DocumentoID.Equals(pFilaDoc.DocumentoID) && doc.BaseRecursosID.Equals(IDBaseRecursos)).ToList();

                    if (pGestionTesauro.TesauroDW.ListaTesauroUsuario.Count > 0)
                    {
                        Guid clavePublica = Guid.Empty;

                        if (pFilaCreador != null && pGestionTesauro.TesauroDW.ListaTesauroUsuario.Any(item => item.UsuarioID.Equals(pFilaCreador.UsuarioID)))
                        {
                            clavePublica = pGestionTesauro.TesauroDW.ListaTesauroUsuario.FirstOrDefault(item => item.UsuarioID.Equals(pFilaCreador.UsuarioID)).CategoriaTesauroPublicoID.Value;
                        }

                        foreach (Guid categoriaTesauro in filasDocAgCat.Select(x => x.CategoriaTesauroID))
                        {
                            Guid categoriaID = categoriaTesauro;

                            if (pGestionTesauro.ListaCategoriasTesauro.ContainsKey(categoriaID) && pGestionTesauro.ListaCategoriasTesauro[categoriaID].PadreNivelRaiz.Clave == clavePublica)
                            {
                                // Al estar en una categoria pública, se comprueba si la persona es buscable
                                if (pFilaCreador != null)
                                {
                                    publico = pFilaCreador.EsBuscable;
                                }
                                break;
                            }
                        }
                    }
                }
            }

            if (!publico)
            {
                // Recorrer las organizaciones cargadas para comprobar si el documento está en el tesauro de alguna de ellas
                foreach (AD.EntityModel.Models.Documentacion.BaseRecursosOrganizacion filaBROrganizacion in pDataWrapperDocumentacion.ListaBaseRecursosOrganizacion)
                {
                    // Revisar el tesauro de las organizaciones en las que está vinculado el documento
                    // Si no está en ninguna categoria pública, entonces el documento es privado
                    // En caso contrario, el documento es público
                    Guid IDBaseRecursos = filaBROrganizacion.BaseRecursosID;

                    List<AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro> filasDocAgCat = pDataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(doc => doc.DocumentoID.Equals(pFilaDoc.DocumentoID) && doc.BaseRecursosID.Equals(IDBaseRecursos)).ToList();

                    if (pGestionTesauro.TesauroDW.ListaTesauroOrganizacion.Count > 0)
                    {
                        Guid clavePublica = Guid.Empty;

                        if (pGestionTesauro.TesauroDW.ListaTesauroOrganizacion.Where(item => item.OrganizacionID.Equals(filaBROrganizacion.OrganizacionID)).ToList().Count > 0)
                        {
                            clavePublica = pGestionTesauro.TesauroDW.ListaTesauroOrganizacion.Where(item => item.OrganizacionID.Equals(filaBROrganizacion.OrganizacionID)).FirstOrDefault().CategoriaTesauroPublicoID.Value;
                        }

                        foreach (AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro filaDocAgCat in filasDocAgCat)
                        {
                            Guid categoriaID = filaDocAgCat.CategoriaTesauroID;

                            if (pGestionTesauro.ListaCategoriasTesauro.ContainsKey(categoriaID) && pGestionTesauro.ListaCategoriasTesauro[categoriaID].PadreNivelRaiz.Clave == clavePublica)
                            {
                                // Al estar en una categoria pública, se comprueba si la organización es buscable
                                if (pFilaOrganizacion != null)
                                {
                                    publico = pFilaOrganizacion.EsBuscable;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            return publico;
        }

        /// <summary>
        /// Devuelve una lista de IDs de organizacion donde se encuentran vinculados una lista de documentos pasada como parámetro
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">Dataset de documentación</param>
        /// <param name="pListaDocumentos">Lista de IDs de documentos</param>
        /// <returns>Lista con IDs de organización donse se encuentran vinculados los documentos</returns>
        public List<Guid> ObtenerOrganizacionesVinculadasDocumentos(DataWrapperDocumentacion pDataWrapperDocumentacion, List<Guid> pListaDocumentos)
        {
            List<Guid> resultado = new List<Guid>();

            foreach (Guid ID in pListaDocumentos)
            {
                foreach (AD.EntityModel.Models.Documentacion.BaseRecursosOrganizacion filaBROrganizacion in pDataWrapperDocumentacion.ListaBaseRecursosOrganizacion)
                {
                    List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filaDocVinBR = pDataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(ID) && doc.BaseRecursosID.Equals(filaBROrganizacion.BaseRecursosID)).ToList();

                    if (filaDocVinBR.Count > 0 && !filaDocVinBR[0].Eliminado && !resultado.Contains(filaBROrganizacion.OrganizacionID))
                    {
                        resultado.Add(filaBROrganizacion.OrganizacionID);
                    }
                }
            }
            return resultado;
        }

        /// <summary>
        /// Devuelve una lista de IDs de proyecto donde se encuentran vinculados una lista de documentos pasada como parámetro
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">Dataset de documentación</param>
        /// <param name="pListaDocumentos">Lista de IDs de documentos</param>
        /// <returns>Lista con IDs de proyecto donse se encuentran vinculados los documentos</returns>
        public List<Guid> ObtenerProyectosVinculadosDocumentos(DataWrapperDocumentacion pDataWrapperDocumentacion, List<Guid> pListaDocumentos)
        {
            List<Guid> listaProyectos = new List<Guid>();

            foreach (Guid ID in pListaDocumentos)
            {
                foreach (AD.EntityModel.Models.Documentacion.BaseRecursosProyecto filaBRProy in pDataWrapperDocumentacion.ListaBaseRecursosProyecto)
                {
                    List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filaDocVinBR = pDataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(ID) && doc.BaseRecursosID.Equals(filaBRProy.BaseRecursosID)).ToList();

                    if (filaDocVinBR.Count > 0 && !filaDocVinBR[0].Eliminado && !listaProyectos.Contains(filaBRProy.ProyectoID))
                    {
                        listaProyectos.Add(filaBRProy.ProyectoID);
                    }
                }
            }
            return listaProyectos;
        }

        #endregion

        #region Documentos Web

        /// <summary>
        /// Vincula un documento a una categoria
        /// </summary>
        /// <param name="pCategoriaTesauro">Categoria</param>
        /// <param name="pFilaDocumento">Fila del documento</param>
        public void VincularDocumentoACategoria(CategoriaTesauro pCategoriaTesauro, AD.EntityModel.Models.Documentacion.Documento pFilaDocumento)
        {
            VincularDocumentoACategoria(pCategoriaTesauro, pFilaDocumento, BaseRecursosIDActual);
        }

        /// <summary>
        /// Vincula un documento a varias categorías.
        /// </summary>
        /// <param name="pCategoriaTesauro">Lista con las categorías del tesauro</param>
        /// <param name="pDocumento">Documento</param>
        public void VincularDocumentoACategorias(List<CategoriaTesauro> pCategoriaTesauro, Documento pDocumento, Guid pIdentidadID, Guid pProyectoID)
        {
            VincularDocumentoACategorias(pCategoriaTesauro, pDocumento, BaseRecursosIDActual, pIdentidadID, pProyectoID);
        }

        /// <summary>
        /// Vincula un documento a varias categorías.
        /// </summary>
        /// <param name="pCategoriaTesauro">Lista con las categorías del tesauro</param>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pBaseRecursosVinculo">Identificador de la BR</param>
        /// <param name="pIdentidadID"></param>
        /// <param name="pProyectoID"></param>
        public void VincularDocumentoACategorias(List<CategoriaTesauro> pCategoriaTesauro, Documento pDocumento, Guid pBaseRecursosVinculo, Guid pIdentidadID, Guid pProyectoID)
        {
            foreach (CategoriaTesauro categoria in pCategoriaTesauro)
            {
                if ((pDocumento is DocumentoWeb) && ((DocumentoWeb)pDocumento).Categorias.ContainsKey(categoria.Clave))
                {
                    continue;
                }
                else if (pDocumento.CategoriasTesauro.ContainsKey(categoria.Clave))
                {
                    continue;
                }
                VincularDocumentoACategoria(categoria, pDocumento.FilaDocumento, pBaseRecursosVinculo);
                AgregarEliminarCategoriaTesauroHistorial(pDocumento, categoria.Clave, AccionHistorialDocumento.Agregar, pIdentidadID, pProyectoID);

                if (!this.ListaDocumentos.ContainsKey(pDocumento.Clave))
                {
                    this.ListaDocumentos.Add(pDocumento.Clave, pDocumento);
                }
                //Agrego a la categoría de tesauro el documento
                if (!categoria.Documentos.Contains(pDocumento))
                {
                    categoria.Documentos.Add(pDocumento);
                }
            }
        }

        /// <summary>
        /// Vincula un documento a una categoria
        /// </summary>
        /// <param name="pCategoriaTesauro">Categoria</param>
        /// <param name="pFilaDocumento">Fila del documento</param>
        /// <param name="pBaseRecursosVinculo">Identificador de la base de recursos</param>
        public void VincularDocumentoACategoria(CategoriaTesauro pCategoriaTesauro, AD.EntityModel.Models.Documentacion.Documento pFilaDocumento, Guid pBaseRecursosVinculo)
        {
            AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro drAgregacion = new AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro();

            drAgregacion.DocumentoID = pFilaDocumento.DocumentoID;
            drAgregacion.BaseRecursosID = pBaseRecursosVinculo;
            drAgregacion.TesauroID = pCategoriaTesauro.FilaCategoria.TesauroID;
            drAgregacion.CategoriaTesauroID = pCategoriaTesauro.FilaCategoria.CategoriaTesauroID;
            drAgregacion.Fecha = DateTime.Now;

            mEntityContext.DocumentoWebAgCatTesauro.Add(drAgregacion);
            DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Add(drAgregacion);
        }

        /// <summary>
        /// Desvincula un documento de varias categorías.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pCategorias">Lista Categorías</param>
        public void DesvincularDocumentoDeCategorias(Documento pDocumento, List<CategoriaTesauro> pCategorias, Guid pIdentidadID, Guid pProyectoID)
        {
            foreach (CategoriaTesauro categoria in pCategorias)
            {
                AgregarEliminarCategoriaTesauroHistorial(pDocumento, categoria.Clave, AccionHistorialDocumento.Eliminar, pIdentidadID, pProyectoID);
                DesvincularDocumentoDeCategoria(pDocumento.FilaDocumento, categoria);
            }
        }

        /// <summary>
        /// Desvincula un documento de una categoria
        /// </summary>
        /// <param name="pFilaDocumento">Fila de documento</param>
        /// <param name="pCategoria">Categoria</param>
        public void DesvincularDocumentoDeCategoria(AD.EntityModel.Models.Documentacion.Documento pFilaDocumento, CategoriaTesauro pCategoria)
        {
            List<AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro> filaDocumentoAgregadoCategoria = DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(doc => doc.DocumentoID.Equals(pFilaDocumento.DocumentoID) && doc.CategoriaTesauroID.Equals(pCategoria.Clave) && doc.TesauroID.Equals(pCategoria.FilaCategoria.TesauroID) && doc.BaseRecursosID.Equals(BaseRecursosIDActual)).ToList();

            if (filaDocumentoAgregadoCategoria.Count > 0)
            {
                mEntityContext.EliminarElemento(filaDocumentoAgregadoCategoria.First());
                DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Remove(filaDocumentoAgregadoCategoria.First());
            }
            pCategoria.Documentos.Remove(ListaDocumentos[pFilaDocumento.DocumentoID]);

            pCategoria.Notificar(new MensajeObservador(AccionesObservador.Recargar, pCategoria));
        }

        /// <summary>
        /// Agrega un documento a una o varias categorias del tesauro (una como mínimo)
        /// </summary>
        /// <param name="pCategoriaTesauro">Categoría de tesauro</param>
        /// <param name="pDocumento">Documento</param>
        public void AgregarDocumento(List<CategoriaTesauro> pCategoriaTesauro, Documento pDocumento, bool pPrivado, Guid pIdentidadID, Guid pProyectoID)
        {
            AgregarDocumento(pCategoriaTesauro, pDocumento, pIdentidadID, pPrivado, pProyectoID);
        }

        /// <summary>
        /// Agrega un documento a una o varias categorias del tesauro (una como mínimo)
        /// </summary>
        /// <param name="pCategoriaTesauro">Categoría de tesauro</param>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pIdentidadAgregadora">Identidad que agrega el documento</param>
        public void AgregarDocumento(List<CategoriaTesauro> pCategoriaTesauro, Documento pDocumento, Guid pIdentidadAgregadora, bool pPrivado, Guid pProyectoID)
        {
            AgregarDocumento(pCategoriaTesauro, pDocumento, pIdentidadAgregadora, BaseRecursosIDActual, pPrivado, pProyectoID);
        }

        /// <summary>
        /// Agrega un documento a una o varias categorias del tesauro (una como mínimo)
        /// </summary>
        /// <param name="pCategoriaTesauro">Categoría de tesauro</param>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pIdentidadAgregadora">Identidad que agrega el documento</param>
        /// <param name="pBaseRecursosID">Base de recursos a la que se agrega el documento</param>
        public void AgregarDocumento(List<CategoriaTesauro> pCategoriaTesauro, Documento pDocumento, Guid pIdentidadAgregadora, Guid pBaseRecursosID, bool pPrivado, Guid pProyectoID)
        {
            pDocumento.FilaDocumento.TipoEntidad = (short)TipoEntidadVinculadaDocumento.Web;
            pDocumento.FilaDocumento.NombreElementoVinculado = "Wiki2";

            if (!this.ListaDocumentos.ContainsKey(pDocumento.Clave))
            {
                this.ListaDocumentos.Add(pDocumento.Clave, pDocumento);
            }
            DocumentoWeb documentoWeb = new DocumentoWeb(pDocumento.FilaDocumento, this);

            VincularDocumentoABaseRecursos(documentoWeb, pBaseRecursosID, TipoPublicacion.Publicado, pIdentidadAgregadora, pPrivado);
            VincularDocumentoACategorias(pCategoriaTesauro, documentoWeb, pBaseRecursosID, pIdentidadAgregadora, pProyectoID);
        }

        /// <summary>
        /// Agrega un documento a una o varias categorias del tesauro (una como mínimo)
        /// </summary>
        /// <param name="pCategoriaTesauro">Categoría de tesauro</param>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pIdentidadAgregadora">Identidad que agrega el documento</param>
        /// <param name="pBaseRecursosID">Base de recursos a la que se agrega el documento</param>
        /// <param name="pProyectoID"></param>
        public void AgregarDocumento(List<CategoriaTesauro> pCategoriaTesauro, Documento pDocumento, Guid pIdentidadAgregadora, Guid pBaseRecursosID, Guid pProyectoID, bool pPrivado)
        {
            pDocumento.FilaDocumento.TipoEntidad = (short)TipoEntidadVinculadaDocumento.Web;
            pDocumento.FilaDocumento.NombreElementoVinculado = "Wiki2";

            if (!this.ListaDocumentos.ContainsKey(pDocumento.Clave))
            {
                this.ListaDocumentos.Add(pDocumento.Clave, pDocumento);
            }
            DocumentoWeb documentoWeb = new DocumentoWeb(pDocumento.FilaDocumento, this);

            VincularDocumentoABaseRecursos(documentoWeb, pBaseRecursosID, TipoPublicacion.Publicado, pIdentidadAgregadora, pPrivado);
            VincularDocumentoACategorias(pCategoriaTesauro, documentoWeb, pBaseRecursosID, pIdentidadAgregadora, pProyectoID);
        }

        /// <summary>
        /// Vincula un documento a la base de recursos correspondiente.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pBaseRecursos">Identificador de la base de recursos a la que se vinculará el documento</param>
        /// <param name="pCompartido">Indica si el documento es compartido en la base de recursos o no</param>
        /// <param name="pPrivado">Indica si el documento es privado en la base de recursos</param>
        /// <param name="pIdentidadID">Identidad que comparte el documento</param>
        public void VincularDocumentoABaseRecursos(Documento pDocumento, Guid pBaseRecursos, TipoPublicacion pTipoPublicacion, Guid pIdentidadID, bool pPrivado)
        {
            // David: Pequeño apaño que se utiliza desde el cliente para no tener que mirar si existe el documento en la base de recursos del proyecto
            if (DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.FirstOrDefault(doc => doc.DocumentoID.Equals(pDocumento.Clave) && doc.BaseRecursosID.Equals(pBaseRecursos)) == null)
            {
                AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaVinculacionBR = new AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos();

                filaVinculacionBR.DocumentoID = pDocumento.Clave;
                filaVinculacionBR.BaseRecursosID = pBaseRecursos;
                filaVinculacionBR.IdentidadPublicacionID = pIdentidadID;
                filaVinculacionBR.FechaPublicacion = DateTime.Now;
                filaVinculacionBR.TipoPublicacion = (short)pTipoPublicacion;
                filaVinculacionBR.LinkAComunidadOrigen = false;
                filaVinculacionBR.Eliminado = false;
                filaVinculacionBR.NumeroComentarios = 0;
                filaVinculacionBR.NumeroVotos = 0;
                filaVinculacionBR.PermiteComentarios = true;
                filaVinculacionBR.IndexarRecurso = true;
                filaVinculacionBR.PrivadoEditores = pPrivado;
                CalcularDatosDesnormalizadosDeDocumentoWebVinBaseRecursos(filaVinculacionBR, pIdentidadID);

                mEntityContext.DocumentoWebVinBaseRecursos.Add(filaVinculacionBR);
                DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Add(filaVinculacionBR);

                AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursosExtra filaDocWebVinBRExtra = new AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursosExtra();
                filaDocWebVinBRExtra.DocumentoID = pDocumento.Clave;
                filaDocWebVinBRExtra.BaseRecursosID = pBaseRecursos;

                filaDocWebVinBRExtra.NumeroConsultas = 0;
                filaDocWebVinBRExtra.NumeroDescargas = 0;

                mEntityContext.DocumentoWebVinBaseRecursosExtra.Add(filaDocWebVinBRExtra);
                DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursosExtra.Add(filaDocWebVinBRExtra);

                if (pDocumento is DocumentoWeb)
                {
                    DocumentoWeb documentoWeb = (DocumentoWeb)pDocumento;

                    if (!documentoWeb.BaseRecursos.Contains(pBaseRecursos))
                    {
                        documentoWeb.BaseRecursos.Add(pBaseRecursos);
                    }
                }
            }
        }

        /// <summary>
        /// Calcula los datos desnormalizados en DocumentoWebVinBaseRecursos
        /// </summary>
        /// <param name="pFilaDocWebVinBR">Fila de la que hay que calcular los datos</param>
        /// <param name="pIdentidadID">Identificador de la identidad para la desnormalización</param>
        public void CalcularDatosDesnormalizadosDeDocumentoWebVinBaseRecursos(AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos pFilaDocWebVinBR, Guid pIdentidadID)
        {
            Identidad.Identidad identidad = GestorIdentidades.ListaIdentidades[pIdentidadID];
            CalcularDatosDesnormalizadosDeDocumentoWebVinBaseRecursos(pFilaDocWebVinBR, identidad);
        }

        /// <summary>
        /// Calcula los datos desnormalizados en DocumentoWebVinBaseRecursos
        /// </summary>
        /// <param name="pFilaDocWebVinBR">Fila de la que hay que calcular los datos</param>
        /// <param name="pIdentidad">Identidad para la desnormalización</param>
        public void CalcularDatosDesnormalizadosDeDocumentoWebVinBaseRecursos(AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos pFilaDocWebVinBR, Identidad.Identidad pIdentidad)
        {
            if (pIdentidad.TrabajaConOrganizacion)
            {
                pFilaDocWebVinBR.PublicadorOrgID = pIdentidad.PerfilUsuario.FilaPerfil.OrganizacionID;
            }
        }

        /// <summary>
        /// Desvincula un documento de una base de recursos.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pBaseRecursos">Identificador de la base de recursos de la que se desvinculará el documento</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que se descomparte</param>
        public void DesVincularDocumentoDeBaseRecursos(Guid pDocumentoID, Guid pBaseRecursos, Guid pProyectoID, Guid pIdentidadID)
        {
            List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filasDoc = DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(pDocumentoID) && doc.BaseRecursosID.Equals(pBaseRecursos)).ToList();

            if (filasDoc.Count > 0)
            {
                filasDoc[0].Eliminado = true;
                AgregarDesComparticionDocumentoHistorial(pDocumentoID, pProyectoID, pIdentidadID);
            }
        }

        /// <summary>
        /// Elimina un documento lógicamente
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        public void EliminarDocumentoLogicamente(Documento pDocumento)
        {
            pDocumento.FilaDocumento.Eliminado = true;
            pDocumento.FilaDocumento.EstadoID = null;
        }

        /// <summary>
        /// Elimina un documento de todas las categorias de tesauro a las que está vinculado
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        public void EliminarDocumentoWeb(Documento pDocumento)
        {
            EliminarDocumentoWeb(pDocumento, true);
        }

        /// <summary>
        /// Elimina un documento de todas las categorias de tesauro a las que está vinculado
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pEliminarVersionesDocumentos">TRUE si se deben eliminar las versiones del documento</param>
        public void EliminarDocumentoWeb(Documento pDocumento, bool pEliminarVersionesDocumentos)
        {
            //Elimino todas las versiones del documento así como los datos de la tabla que las guarda:
            EliminarVersionesDocumento(pDocumento, pEliminarVersionesDocumentos);

            //Eliminar las vinculaciones con la wiki
            List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filasVinculacionesWiki = this.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(pDocumento.Clave)).ToList();

            foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos fila in filasVinculacionesWiki)
            {
                mEntityContext.EliminarElemento(fila);
            }
            //Eliminar las categorias vinculadas
            List<AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro> filasAgregaciones = this.DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(doc => doc.DocumentoID.Equals(pDocumento.Clave)).ToList();

            foreach (AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro fila in filasAgregaciones)
            {
                if (ListaCategoriasTesauro.ContainsKey(fila.CategoriaTesauroID))
                {
                    CategoriaTesauro categoria = this.ListaCategoriasTesauro[fila.CategoriaTesauroID];

                    EliminarDocumento(categoria, pDocumento);
                }
                else
                {
                    mEntityContext.EliminarElemento(fila);
                }
            }
            this.EliminarDocumento(pDocumento);
        }

        /// <summary>
        /// Elimina un documento de una categoria de tesauro
        /// </summary>
        /// <param name="pCategoria">Categoría de tesauro</param>
        /// <param name="pDocumento">Documento</param>
        public void EliminarDocumento(CategoriaTesauro pCategoria, Documento pDocumento)
        {
            pCategoria.Documentos.Remove(pDocumento);

            List<AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro> fila = DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(doc => doc.BaseRecursosID.Equals(BaseRecursosIDActual) && doc.TesauroID.Equals(pCategoria.FilaCategoria.TesauroID) && doc.CategoriaTesauroID.Equals(pCategoria.FilaCategoria.CategoriaTesauroID) && doc.DocumentoID.Equals(pDocumento.Clave)).ToList();

            if (fila.Count > 0)
            {
                mEntityContext.EliminarElemento(fila.First());
                DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Remove(fila.First());
            }
            pDocumento.Notificar(new MensajeObservador(AccionesObservador.Eliminar, pCategoria));
        }

        /// <summary>
        /// /Compartir un Documento con otra comunidad
        /// </summary>
        /// <param name="pComunidadID">Identificador de la comunidad donde se incluirá el documento</param>
        /// <param name="pListaCategorias">Categorias a las que se vincula de la nueva comunidad</param>
        /// <param name="pDocumento">Documento que se comparte</param>
        /// <param name="pGestorDocumentalCompartirRecurso">Gestor documental usado para compartir el documento</param>
        /// <param name="pIdentidadCompartidora">Identidad con la que se comparte el documento</param>
        /// <returns>Par con la lista de proyectos a actualizar y un valor que indica si el documento estaba descompartido o no</returns>
        public KeyValuePair<List<Guid>, bool> CompartirRecurso(string pComunidadID, Dictionary<Guid, CategoriaTesauro> pListaCategorias, Documento pDocumento, GestorDocumental pGestorDocumentalCompartirRecurso, Guid pIdentidadCompartidora)
        {
            if (pDocumento != null && pGestorDocumentalCompartirRecurso != null)
            {
                if (!(pDocumento is DocumentoWeb))
                {
                    pDocumento = ObtenerDocumentoWeb(pDocumento);
                }

                //es posible que hay que usar el gestordocumental de destino!!
                Guid proyectoID = ProyectoAD.MetaProyecto;
                if (pComunidadID.Substring(0, 2) != prefijoBRUsuario && pComunidadID.Substring(0, 2) != prefijoBROrganizacion)
                {
                    proyectoID = new Guid(pComunidadID);
                }

                List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filaDocVinBR = DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.BaseRecursosID.Equals(pGestorDocumentalCompartirRecurso.BaseRecursosIDActual) && doc.Eliminado && doc.DocumentoID.Equals(pDocumento.Clave)).ToList();
                bool modificando = false;

                if (filaDocVinBR.Count > 0)
                {
                    filaDocVinBR[0].Eliminado = false;
                    filaDocVinBR[0].IdentidadPublicacionID = pIdentidadCompartidora;
                    filaDocVinBR[0].FechaPublicacion = DateTime.Now;
                    modificando = true;

                    this.AgregarComparticionDocumentoHistorial(pDocumento, proyectoID, pIdentidadCompartidora);

                    //Castillo: Se ha quitado para que cuando se comparta un documento no se modifique su fecha y no genere resultdos de suscripciones
                    //pDocumento.FilaDocumento.FechaModificacion = DateTime.Now;
                }
                else if (!((DocumentoWeb)pDocumento).BaseRecursos.Contains(pGestorDocumentalCompartirRecurso.BaseRecursosIDActual))
                {
                    //Si ya esta compartido en esta BR no lo hago
                    pGestorDocumentalCompartirRecurso.VincularDocumentoABaseRecursos(pDocumento, pGestorDocumentalCompartirRecurso.BaseRecursosIDActual, TipoPublicacion.Compartido, pIdentidadCompartidora, false);

                    if (pComunidadID.Substring(0, 2) != prefijoBRUsuario && pComunidadID.Substring(0, 2) != prefijoBROrganizacion)
                    {
                        this.AgregarComparticionDocumentoHistorial(pDocumento, new Guid(pComunidadID), pIdentidadCompartidora);
                    }
                    else
                    {
                        this.AgregarComparticionDocumentoHistorial(pDocumento, ProyectoAD.MetaProyecto, pIdentidadCompartidora);
                    }
                    //Castillo: Se ha quitado para que cuando se comparta un documento no se modifique su fecha y no genere resultdos de suscripciones
                    //pDocumento.FilaDocumento.FechaModificacion = DateTime.Now;
                }
                List<Guid> listaProyectosActualizarNumProy = new List<Guid>();

                if (pComunidadID.Substring(0, 2) != prefijoBRUsuario && pComunidadID.Substring(0, 2) != prefijoBROrganizacion && pComunidadID != ProyectoAD.MetaProyecto.ToString())
                {
                    listaProyectosActualizarNumProy.Add(new Guid(pComunidadID));
                }
                KeyValuePair<List<Guid>, bool> listaProyConindicacionGuardado = new KeyValuePair<List<Guid>, bool>();

                if (!modificando)
                {
                    //Meto la fila del gestor actual en el de compartición para que se actualice la fecha del documento.
                    pGestorDocumentalCompartirRecurso.DataWrapperDocumentacion.Merge(pDocumento.GestorDocumental.DataWrapperDocumentacion);

                    //si es compartición por defecto el diccionario llegará vacío y no hay que vincular el recurso a ninguna categoría
                    //lo hará el base con el mapeo de la comunidad de destino
                    if (pListaCategorias.Keys.Count > 0)
                    {
                        //Vincular categorias
                        List<CategoriaTesauro> listaCategorias = new List<CategoriaTesauro>();

                        foreach (CategoriaTesauro categoria in pListaCategorias.Values)
                        {
                            listaCategorias.Add(categoria);
                        }
                        pGestorDocumentalCompartirRecurso.VincularDocumentoACategorias(listaCategorias, pDocumento, pIdentidadCompartidora, pIdentidadCompartidora, proyectoID);
                    }
                }
                else
                {
                    //Vincular categorias
                    List<CategoriaTesauro> listaCategorias = new List<CategoriaTesauro>();

                    foreach (CategoriaTesauro categoria in pListaCategorias.Values)
                    {//"BaseRecursosID='" + pGestorDocumentalCompartirRecurso.BaseRecursosIDActual + "' AND DocumentoID='" + pDocumento.Clave + "' AND CategoriaTesauroID='" + categoria.Clave + "'"
                        List<AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro> filasDocVinAgCat = DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(doc => doc.BaseRecursosID.Equals(pGestorDocumentalCompartirRecurso.BaseRecursosIDActual) && doc.DocumentoID.Equals(pDocumento.Clave) && doc.CategoriaTesauroID.Equals(categoria.Clave)).ToList();

                        if (filasDocVinAgCat.Count == 0)
                        {
                            listaCategorias.Add(categoria);
                        }
                    }
                    VincularDocumentoACategorias(listaCategorias, pDocumento, pGestorDocumentalCompartirRecurso.BaseRecursosIDActual, pIdentidadCompartidora, proyectoID);
                }
                listaProyConindicacionGuardado = new KeyValuePair<List<Guid>, bool>(listaProyectosActualizarNumProy, modificando);

                return listaProyConindicacionGuardado;
            }
            return new KeyValuePair<List<Guid>, bool>(null, false);
        }

        /// <summary>
        /// Compartir una Ontologia con otra comunidad
        /// </summary>
        /// <param name="pComunidadID">Identificador de la comunidad donde se incluirá el documento</param>
        /// <param name="pDocumento">Documento que se comparte</param>
        /// <param name="pGestorDocumentalCompartirRecurso">Gestor documental usado para compartir el documento</param>
        /// <param name="pIdentidadCompartidora">Identidad con la que se comparte el documento</param>
        /// <returns>Par con la lista de proyectos a actualizar y un valor que indica si el documento estaba descompartido o no</returns>
        public KeyValuePair<List<Guid>, bool> CompartirOntologia(string pComunidadID, Documento pDocumento, GestorDocumental pGestorDocumentalCompartirRecurso, Guid pIdentidadCompartidora)
        {
            if (pDocumento != null && pGestorDocumentalCompartirRecurso != null)
            {
                if (!(pDocumento is DocumentoWeb))
                {
                    pDocumento = ObtenerDocumentoWeb(pDocumento);
                }

                Guid proyectoID = ProyectoAD.MetaProyecto;
                if (pComunidadID.Substring(0, 2) != prefijoBRUsuario && pComunidadID.Substring(0, 2) != prefijoBROrganizacion)
                {
                    proyectoID = new Guid(pComunidadID);
                }

                List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filaDocVinBR = DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.BaseRecursosID.Equals(pGestorDocumentalCompartirRecurso.BaseRecursosIDActual) && doc.Eliminado && doc.DocumentoID.Equals(pDocumento.Clave)).ToList();
                bool modificando = false;

                if (filaDocVinBR.Count > 0)
                {
                    filaDocVinBR[0].Eliminado = false;
                    filaDocVinBR[0].IdentidadPublicacionID = pIdentidadCompartidora;
                    filaDocVinBR[0].FechaPublicacion = DateTime.Now;
                    modificando = true;

                    this.AgregarComparticionDocumentoHistorial(pDocumento, proyectoID, pIdentidadCompartidora);

                    //Castillo: Se ha quitado para que cuando se comparta un documento no se modifique su fecha y no genere resultdos de suscripciones
                    //pDocumento.FilaDocumento.FechaModificacion = DateTime.Now;
                }
                else if (!((DocumentoWeb)pDocumento).BaseRecursos.Contains(pGestorDocumentalCompartirRecurso.BaseRecursosIDActual))
                {
                    //Si ya esta compartido en esta BR no lo hago
                    pGestorDocumentalCompartirRecurso.VincularDocumentoABaseRecursos(pDocumento, pGestorDocumentalCompartirRecurso.BaseRecursosIDActual, TipoPublicacion.Compartido, pIdentidadCompartidora, false);

                    this.AgregarComparticionDocumentoHistorial(pDocumento, proyectoID, pIdentidadCompartidora);

                    //Castillo: Se ha quitado para que cuando se comparta un documento no se modifique su fecha y no genere resultdos de suscripciones
                    //pDocumento.FilaDocumento.FechaModificacion = DateTime.Now;
                }
                List<Guid> listaProyectosActualizarNumProy = new List<Guid>();

                if (pComunidadID.Substring(0, 2) != prefijoBRUsuario && pComunidadID.Substring(0, 2) != prefijoBROrganizacion && pComunidadID != ProyectoAD.MetaProyecto.ToString())
                {
                    listaProyectosActualizarNumProy.Add(new Guid(pComunidadID));
                }
                KeyValuePair<List<Guid>, bool> listaProyConindicacionGuardado = new KeyValuePair<List<Guid>, bool>();
                listaProyConindicacionGuardado = new KeyValuePair<List<Guid>, bool>(listaProyectosActualizarNumProy, modificando);

                return listaProyConindicacionGuardado;
            }
            return new KeyValuePair<List<Guid>, bool>(null, false);
        }

        /// <summary>
        /// /Compartir un Documento con otra comunidad
        /// </summary>
        /// <param name="pListaDocumentos">Documentos que se comparten</param>
        /// <param name="pGestorAddToGnoss">Gestor de Add to Gnoss usado para compartir el documento</param>
        public void CompartirRecursoEnVariasComunidades(List<Documento> pListaDocumentos, GestorAddToGnoss pGestorAddToGnoss)
        {
            foreach (Documento documento in pListaDocumentos)
            {
                CompartirRecursoEnVariasComunidades(documento, pGestorAddToGnoss);
            }
        }

        /// <summary>
        /// /Compartir un Documento con otra comunidad
        /// </summary>
        /// <param name="pDocumento">Documento que se comparte</param>
        /// <param name="pGestorAddToGnoss">Gestor de Add to Gnoss usado para compartir el documento</param>
        public bool CompartirRecursoEnVariasComunidades(Documento pDocumento, GestorAddToGnoss pGestorAddToGnoss)
        {//DocumentacionDS.DocumentoWebVinBaseRecursos.Select("BaseRecursosID='" + pGestorAddToGnoss.GestorDocumental.BaseRecursosIDActual + "' AND Eliminado=0 AND DocumentoID='" + pDocumento.Clave + "'").Length == 0
            if (!DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Any(docWebVin => docWebVin.BaseRecursosID.Equals(pGestorAddToGnoss.GestorDocumental.BaseRecursosIDActual) && !docWebVin.Eliminado && docWebVin.Equals(pDocumento.Clave)))
            {
                List<CategoriaTesauro> listaCategoriasCompartir = new List<CategoriaTesauro>();

                if (!(pDocumento is DocumentoWeb))
                {
                    pDocumento = new DocumentoWeb(pDocumento.FilaDocumento, pDocumento.GestorDocumental);
                }

                if (pDocumento.TipoDocumentacion.Equals(TiposDocumentacion.Wiki))
                {
                    AD.EntityModel.Models.Documentacion.Documento filaDoc = new AD.EntityModel.Models.Documentacion.Documento();


                    //mEntityContext.Documento.
                    //foreach (DataColumn columna in DocumentacionDS.Documento.Columns)
                    foreach (PropertyInfo property in filaDoc.GetType().GetProperties())
                    {
                        if (property.Name != nameof(filaDoc.DocumentoID))
                        {
                            UtilReflection.SetValueReflection(filaDoc, property.Name, UtilReflection.GetValueReflection(pDocumento.FilaDocumento, property.Name));
                        }
                    }
                    filaDoc.FechaCreacion = DateTime.Now;
                    filaDoc.FechaModificacion = DateTime.Now;
                    filaDoc.Protegido = false;
                    filaDoc.IdentidadProteccionID = null;
                    filaDoc.FechaProteccion = null;
                    filaDoc.UltimaVersion = true;
                    filaDoc.Eliminado = false;
                    filaDoc.CreadorID = pGestorAddToGnoss.IdentidadEnProyecto;
                    filaDoc.NumeroComentariosPublicos = 0;
                    filaDoc.NumeroTotalConsultas = 0;
                    filaDoc.NumeroTotalDescargas = 0;
                    filaDoc.NumeroTotalVotos = 0;
                    filaDoc.DocumentoID = Guid.NewGuid();
                    filaDoc.ProyectoID = pGestorAddToGnoss.ProyectoBRID;

                    DataWrapperDocumentacion.ListaDocumento.Add(filaDoc);
                    mEntityContext.Documento.Add(filaDoc);

                    pDocumento = new Documento(filaDoc, pDocumento.GestorDocumental);

                    AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaDocVinBR = new AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos();

                    filaDocVinBR.DocumentoID = filaDoc.DocumentoID;
                    filaDocVinBR.BaseRecursosID = pGestorAddToGnoss.GestorDocumental.BaseRecursosIDActual;
                    filaDocVinBR.PrivadoEditores = pGestorAddToGnoss.PrivadoEditores;
                    filaDocVinBR.TipoPublicacion = (short)TipoPublicacion.Compartido;
                    filaDocVinBR.LinkAComunidadOrigen = false;
                    filaDocVinBR.Eliminado = false;
                    filaDocVinBR.IdentidadPublicacionID = pGestorAddToGnoss.IdentidadEnProyecto;
                    filaDocVinBR.FechaPublicacion = DateTime.Now;
                    filaDocVinBR.NumeroComentarios = 0;
                    filaDocVinBR.NumeroVotos = 0;
                    filaDocVinBR.PermiteComentarios = true;
                    filaDocVinBR.IndexarRecurso = true;

                    CalcularDatosDesnormalizadosDeDocumentoWebVinBaseRecursos(filaDocVinBR, pGestorAddToGnoss.IdentidadEnProyecto);

                    mEntityContext.DocumentoWebVinBaseRecursos.Add(filaDocVinBR);
                    DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Add(filaDocVinBR);

                    AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursosExtra filaDocWebVinBRExtra = new AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursosExtra();
                    filaDocWebVinBRExtra.DocumentoID = filaDoc.DocumentoID;
                    filaDocWebVinBRExtra.BaseRecursosID = pGestorAddToGnoss.GestorDocumental.BaseRecursosIDActual;

                    filaDocWebVinBRExtra.NumeroConsultas = 0;
                    filaDocWebVinBRExtra.NumeroDescargas = 0;

                    DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursosExtra.Add(filaDocWebVinBRExtra);
                    mEntityContext.DocumentoWebVinBaseRecursosExtra.Add(filaDocWebVinBRExtra);


                    foreach (CategoriaTesauro categoria in pGestorAddToGnoss.ListaCategorias)
                    {
                        listaCategoriasCompartir.Add(categoria);
                    }
                }
                else
                {
                    List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filaDocVinBR = DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.BaseRecursosID.Equals(pGestorAddToGnoss.GestorDocumental.BaseRecursosIDActual) && doc.Eliminado && doc.DocumentoID.Equals(pDocumento.Clave)).ToList();

                    if (filaDocVinBR.Count > 0)
                    {
                        filaDocVinBR[0].Eliminado = false;
                        filaDocVinBR[0].IdentidadPublicacionID = pGestorAddToGnoss.IdentidadEnProyecto;
                        filaDocVinBR[0].FechaPublicacion = DateTime.Now;
                        filaDocVinBR[0].PrivadoEditores = pGestorAddToGnoss.PrivadoEditores;

                        //Elimino las categorías que tenía anteriormente:
                        foreach (AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro filaDocVinCat in DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(doc => doc.BaseRecursosID.Equals(pGestorAddToGnoss.GestorDocumental.BaseRecursosIDActual) && doc.DocumentoID.Equals(pDocumento.Clave)))
                        {
                            mEntityContext.EliminarElemento(filaDocVinCat);
                            DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Remove(filaDocVinCat);
                        }

                        //Agrego a una lista las nuevas categorías para vincularlas:
                        foreach (CategoriaTesauro categoria in pGestorAddToGnoss.ListaCategorias)
                        {
                            listaCategoriasCompartir.Add(categoria);
                        }
                    }
                    else
                    {
                        VincularDocumentoABaseRecursos(pDocumento, pGestorAddToGnoss.GestorDocumental.BaseRecursosIDActual, TipoPublicacion.Compartido, pGestorAddToGnoss.IdentidadEnProyecto, pGestorAddToGnoss.PrivadoEditores);

                        listaCategoriasCompartir = pGestorAddToGnoss.ListaCategorias;
                    }
                    this.AgregarComparticionDocumentoHistorial(pDocumento, pGestorAddToGnoss.ProyectoBRID, pGestorAddToGnoss.IdentidadEnProyecto);
                }
                //IMPORTANTE: Hay que dar la fecha a la fila del dataSet actual, aparte de a pDocumento, ya que este último puede tener 
                //un GestorDocumental diferente y a la hora de guardar los datos de está modificación se despreciaría:
                //Castillo: Se ha quitado para que cuando se comparta un documento no se modifique su fecha y no genere resultdos de suscripciones
                //pDocumento.FilaDocumento.FechaModificacion = DateTime.Now;
                //DataRow[] filasDoc = DocumentacionDS.Documento.Select("DocumentoID='" + pDocumento.Clave + "'");

                //if (filasDoc.Length > 0)
                //{
                //    ((DocumentacionDS.DocumentoRow)filasDoc[0]).FechaModificacion = pDocumento.FilaDocumento.FechaModificacion;
                //}
                VincularDocumentoACategorias(listaCategoriasCompartir, pDocumento, pGestorAddToGnoss.GestorDocumental.BaseRecursosIDActual, pGestorAddToGnoss.IdentidadEnProyecto, pGestorAddToGnoss.ProyectoBRID);

                if (pDocumento.TipoDocumentacion == TiposDocumentacion.Semantico)
                {
                    //Si es un documento semántico y se ha agregado a tu BR Personal
                    AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos documentoWebVinBaseRecursos = DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.FirstOrDefault(doc => doc.BaseRecursosID.Equals(pGestorAddToGnoss.GestorDocumental.BaseRecursosIDActual));
                    if (documentoWebVinBaseRecursos != null)
                    {
                        documentoWebVinBaseRecursos.LinkAComunidadOrigen = true;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// A partir de un documento devuelve un documento de tipo Web.
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <returns>Documento Web</returns>
        public DocumentoWeb ObtenerDocumentoWeb(Documento pDocumento)
        {
            DocumentoWeb documentoWeb = null;
            List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filaDocumentoWeb = DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(pDocumento.Clave)).ToList();

            if (filaDocumentoWeb.Count > 0 && this.ListaDocumentos.ContainsKey(pDocumento.Clave))
            {
                if (this.ListaDocumentos[pDocumento.Clave] is DocumentoWeb)
                {
                    documentoWeb = (DocumentoWeb)this.ListaDocumentos[pDocumento.Clave];
                }
                else
                {
                    documentoWeb = new DocumentoWeb(pDocumento.FilaDocumento, this);
                    this.ListaDocumentos.Remove(pDocumento.Clave);
                    this.ListaDocumentos.Add(documentoWeb.Clave, documentoWeb);
                }
            }
            return documentoWeb;
        }

        #region VersionDocumento

        public void CrearPrimeraVersionDocumento(Documento pDocumento)
        {
            VersionDocumento filaVersionDocumento = new VersionDocumento();
            filaVersionDocumento.DocumentoID = pDocumento.Clave;
            filaVersionDocumento.Version = 0;
            filaVersionDocumento.EstadoVersion = (short)EstadoVersion.Vigente;
            filaVersionDocumento.EstadoID = pDocumento.FilaDocumento.EstadoID;
            filaVersionDocumento.IdentidadID = pDocumento.CreadorID;
            filaVersionDocumento.EsMejora = false;
            filaVersionDocumento.DocumentoOriginalID = pDocumento.Clave;
            mEntityContext.VersionDocumento.Add(filaVersionDocumento);
        }

        /// <summary>
        /// Elimina todos los documentos de la versiones anteriores a la actual.
        /// </summary>
        /// <param name="pDocumento">Documento actual</param>
        /// <param name="pEliminarTodos">Especifica si deben eliminarse todos los documento de versiones anteriores o no</param>
        private void EliminarVersionesDocumento(Documento pDocumento, bool pEliminarTodos)
        {
            AD.EntityModel.Models.Documentacion.VersionDocumento filasVersionDoc = DataWrapperDocumentacion.ListaVersionDocumento.Find(doc => doc.DocumentoID.Equals(pDocumento.Clave));

            if (filasVersionDoc != null)
            {
                if (pEliminarTodos)
                {
                    Guid documentoOriginalID = filasVersionDoc.DocumentoOriginalID;
                    List<Guid> listaDocumentoVersionados = ObtenerListaDocumentosVersionadosDe(documentoOriginalID);

                    foreach (Guid documentoID in listaDocumentoVersionados)
                    {
                        if (documentoID != pDocumento.Clave)
                        {
                            Documento documento = ListaDocumentos[documentoID];
                            EliminarDocumentoWeb(documento, false);
                        }
                    }
                    //Elimino el documento original:
                    Documento documentoOriginal = ListaDocumentos[documentoOriginalID];
                    EliminarDocumentoWeb(documentoOriginal, false);
                }
                //Borro la fila del VersionDocumento
                mEntityContext.EliminarElemento(filasVersionDoc);
                DataWrapperDocumentacion.ListaVersionDocumento.Remove(filasVersionDoc);
            }
        }

        /// <summary>
        /// Obtiene una lista con todas las antiguas versiones de un documento.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento original</param>
        /// <returns>Lista con todas las antiguas versiones de un documento.</returns>
        public List<Guid> ObtenerListaDocumentosVersionadosDe(Guid pDocumentoID)
        {
            List<AD.EntityModel.Models.Documentacion.VersionDocumento> filasVersionDoc = DataWrapperDocumentacion.ListaVersionDocumento.Where(versionDoc => versionDoc.DocumentoOriginalID.Equals(pDocumentoID)).ToList();
            List<Guid> listaDocumentoVersionados = new List<Guid>();

            foreach (AD.EntityModel.Models.Documentacion.VersionDocumento filaVerDoc in filasVersionDoc)
            {
                listaDocumentoVersionados.Add(filaVerDoc.DocumentoID);
            }
            return listaDocumentoVersionados;
        }

        #endregion

        #region Base de recursos

        /// <summary>
        /// Obtiene el identificador del proyecto al que pertenece una determinada base de recursos.
        /// </summary>
        /// <param name="pBaseRecursosID">Identificador de base de recursos</param>
        /// <returns>Identificador del proyecto al que pertenece una determinada base de recursos</returns>
        public Guid ObtenerProyectoID(Guid pBaseRecursosID)
        {
            Guid proyectoID = Guid.Empty;
            List<AD.EntityModel.Models.Documentacion.BaseRecursosProyecto> filasBRP = DataWrapperDocumentacion.ListaBaseRecursosProyecto.Where(baseRec => baseRec.BaseRecursosID.Equals(pBaseRecursosID)).ToList();

            if (filasBRP.Count > 0)
            {
                proyectoID = filasBRP[0].ProyectoID;
            }
            else
            {//Esta en baBaseRecursosUsuario
                proyectoID = ProyectoAD.MetaProyecto;
            }
            //TODO: JAVIER -> ¿Meter aqui la base de recursos de organización?

            return proyectoID;
        }

        /// <summary>
        /// Obtiene el identificador de organizacion al que pertenece una determinada base de recursos.
        /// </summary>
        /// <param name="pBaseRecursosID">Identificador de base de recursos</param>
        /// <returns>Identificador de la organizacion al que pertenece una determinada base de recursos</returns>
        public Guid ObtenerOrganizacionID(Guid pBaseRecursosID)
        {
            Guid organizacionID = Guid.Empty;

            AD.EntityModel.Models.Documentacion.BaseRecursosOrganizacion fila = DataWrapperDocumentacion.ListaBaseRecursosOrganizacion.Where(item => item.BaseRecursosID.Equals(pBaseRecursosID)).FirstOrDefault();

            if (fila != null)
            {
                organizacionID = fila.OrganizacionID;
            }
            else
            {
                organizacionID = ProyectoAD.MetaOrganizacion;
            }


            return organizacionID;
        }

        /// <summary>
        /// Devuelve el tipo de base de recursos pasando el id como parámetro
        /// </summary>
        /// <param name="pBaseRecursosID"></param>
        /// <returns></returns>
        public TipoBaseRecursos ObtenerTipoBaseRecursos(Guid pBaseRecursosID)
        {
            TipoBaseRecursos resultado = TipoBaseRecursos.BRNoDefinida;

            if (DataWrapperDocumentacion.ListaBaseRecursosOrganizacion.Any(baseRec => baseRec.BaseRecursosID.Equals(pBaseRecursosID)))
            {
                resultado = TipoBaseRecursos.BROrganizacion;
            }
            else if (DataWrapperDocumentacion.ListaBaseRecursosProyecto.Any(baseRec => baseRec.BaseRecursosID.Equals(pBaseRecursosID)))
            {
                resultado = TipoBaseRecursos.BRProyectos;
            }
            else if (DataWrapperDocumentacion.ListaBaseRecursosUsuario.Any(baseRec => baseRec.BaseRecursosID.Equals(pBaseRecursosID)))
            {
                resultado = TipoBaseRecursos.BRUsuario;
            }
            return resultado;
        }

        #endregion

        #endregion

        #region Base de recursos de usuario y organización

        /// <summary>
        /// Crea la base de recursos del usuario pasado por parámetro
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        public void AgregarBRDeUsuario(AD.EntityModel.Models.UsuarioDS.Usuario pUsuario)
        {
            //BaseRecursos
            AD.EntityModel.Models.Documentacion.BaseRecursos filaBaseRecursos = new AD.EntityModel.Models.Documentacion.BaseRecursos();
            filaBaseRecursos.BaseRecursosID = Guid.NewGuid();

            DataWrapperDocumentacion.ListaBaseRecursos.Add(filaBaseRecursos);
            mEntityContext.BaseRecursos.Add(filaBaseRecursos);

            //BaseRecursosUsuario
            AD.EntityModel.Models.Documentacion.BaseRecursosUsuario filaBRUsuario = new AD.EntityModel.Models.Documentacion.BaseRecursosUsuario();
            filaBRUsuario.UsuarioID = pUsuario.UsuarioID;
            filaBRUsuario.BaseRecursos = filaBaseRecursos;
            filaBRUsuario.EspacioMaxMyGnossMB = 1024;
            filaBRUsuario.EspacioActualMyGnossMB = 0;
            //filaBRUsuario.Usuario = pUsuario;
            //pUsuario.BaseRecursosUsuario.Add(filaBRUsuario);
            DataWrapperDocumentacion.ListaBaseRecursosUsuario.Add(filaBRUsuario);
            mEntityContext.BaseRecursosUsuario.Add(filaBRUsuario);
        }

        /// <summary>
        /// Crea una base de recursos para la organziación pasada por parámetro
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        public void AgregarBRDeOrganizacion(Guid pOrganizacionID)
        {//TODO MIGRAR EF
            //BaseRecursos
            AD.EntityModel.Models.Documentacion.BaseRecursos filaBaseRecursos = new AD.EntityModel.Models.Documentacion.BaseRecursos();
            filaBaseRecursos.BaseRecursosID = Guid.NewGuid();

            DataWrapperDocumentacion.ListaBaseRecursos.Add(filaBaseRecursos);
            mEntityContext.BaseRecursos.Add(filaBaseRecursos);

            //BaseRecursosOrganizacion
            AD.EntityModel.Models.Documentacion.BaseRecursosOrganizacion filaBROrganizacion = new AD.EntityModel.Models.Documentacion.BaseRecursosOrganizacion();
            filaBROrganizacion.OrganizacionID = pOrganizacionID;
            filaBROrganizacion.BaseRecursos = filaBaseRecursos;
            filaBROrganizacion.EspacioMaxMyGnossMB = 1024;
            filaBROrganizacion.EspacioActualMyGnossMB = 0;

            DataWrapperDocumentacion.ListaBaseRecursosOrganizacion.Add(filaBROrganizacion);
            mEntityContext.BaseRecursosOrganizacion.Add(filaBROrganizacion);
        }

        /// <summary>
        /// Elimina la base de recursos cuyo ID se pasa como parámetro
        /// </summary>
        /// <param name="pBaseRecursosID">Identificador de la base de recursos</param>
        public void EliminarBaseRecursos(Guid pBaseRecursosID)
        {
            // Eliminar registros base de recursos de usuario
            foreach (AD.EntityModel.Models.Documentacion.BaseRecursosUsuario fila in DataWrapperDocumentacion.ListaBaseRecursosUsuario.Where(baseRec => baseRec.BaseRecursosID.Equals(pBaseRecursosID)))
            {
                mEntityContext.EliminarElemento(fila);
            }

            // Eliminar registros base de recursos de proyecto
            foreach (AD.EntityModel.Models.Documentacion.BaseRecursosProyecto fila in DataWrapperDocumentacion.ListaBaseRecursosProyecto.Where(baseRec => baseRec.BaseRecursosID.Equals(pBaseRecursosID)))
            {
                mEntityContext.EliminarElemento(fila);
            }

            // Eliminar registros base de recursos de organizacion
            foreach (AD.EntityModel.Models.Documentacion.BaseRecursosOrganizacion fila in DataWrapperDocumentacion.ListaBaseRecursosOrganizacion.Where(baseRec => baseRec.BaseRecursosID.Equals(pBaseRecursosID)))
            {
                mEntityContext.EliminarElemento(fila);
            }

            // Eliminar registros documentos web agregación categoria tesauro
            foreach (AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro fila in DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(baseRec => baseRec.BaseRecursosID.Equals(pBaseRecursosID)))
            {
                mEntityContext.EliminarElemento(fila);
            }

            // Eliminar registros documentos web vinculados base de recursos
            foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos fila in DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(baseRec => baseRec.BaseRecursosID.Equals(pBaseRecursosID)))
            {
                mEntityContext.EliminarElemento(fila);
            }

            // Eliminar el registro de base de recursos
            AD.EntityModel.Models.Documentacion.BaseRecursos baseRecursosDelete = DataWrapperDocumentacion.ListaBaseRecursos.FirstOrDefault(baseRec => baseRec.BaseRecursosID.Equals(pBaseRecursosID));
            if (baseRecursosDelete != null)
            {
                DataWrapperDocumentacion.ListaBaseRecursos.Remove(baseRecursosDelete);
                mEntityContext.EliminarElemento(baseRecursosDelete);
            }
        }

        #endregion

        #region Ordenar

        /// <summary>
        /// Compara dos documentos por fecha
        /// </summary>
        /// <param name="x">Documento x</param>
        /// <param name="y">Documento y</param>
        /// <returns></returns>
        public static int CompararDocumentosPorFecha(Documento x, Documento y)
        {
            return y.Fecha.CompareTo(x.Fecha);
        }

        /// <summary>
        /// Compara dos documentos por fecha de manera descendente
        /// </summary>
        /// <param name="x">Documento x</param>
        /// <param name="y">Documento y</param>
        /// <returns></returns>
        public static int CompararDocumentosPorFechaDesc(Documento x, Documento y)
        {
            return x.Fecha.CompareTo(y.Fecha);
        }

        /// <summary>
        /// Compara dos documentos por nombre
        /// </summary>
        /// <param name="x">Documento x</param>
        /// <param name="y">Documento y</param>
        /// <returns></returns>
        public static int CompararDocumentosPorNombre(Documento x, Documento y)
        {
            return x.Titulo.CompareTo(y.Titulo);
        }

        /// <summary>
        /// Compara dos documentos por nombre descendente
        /// </summary>
        /// <param name="x">Documento x</param>
        /// <param name="y">Documento y</param>
        /// <returns></returns>
        public static int CompararDocumentosPorNombreDesc(Documento x, Documento y)
        {
            return y.Titulo.CompareTo(x.Titulo);
        }

        #endregion

        #region Metas

        /// <summary>
        /// Agregar el metadato del titulo al recurso
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pMetaTitulo">Titulo para el metatitulo</param>
        public AD.EntityModel.Models.Documentacion.DocumentoMetaDatos AgregarMetaTitulo(Documento pDocumento, string pMetaTitulo)
        {
            //Agregar a BD estos datos.
            AD.EntityModel.Models.Documentacion.DocumentoMetaDatos documentoMetaDatos = mEntityContext.DocumentoMetaDatos.FirstOrDefault(x => x.DocumentoID == pDocumento.Clave);
            if (documentoMetaDatos == null)
            {
                documentoMetaDatos = new AD.EntityModel.Models.Documentacion.DocumentoMetaDatos();
                documentoMetaDatos.MetaTitulo = pMetaTitulo;
                documentoMetaDatos.DocumentoID = pDocumento.Clave;
                documentoMetaDatos.Documento = pDocumento.FilaDocumento;
                mEntityContext.DocumentoMetaDatos.Add(documentoMetaDatos);
            }
            else
            {
                documentoMetaDatos.MetaTitulo = pMetaTitulo;
            }

            mEntityContext.SaveChanges();
            return documentoMetaDatos;
        }

        /// <summary>
        /// Agregar el metadato de la descripcion al recurso
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pMetaDescripcion">Descripcion para la metadescripcion</param>
        public AD.EntityModel.Models.Documentacion.DocumentoMetaDatos AgregarMetaDescripcion(Documento pDocumento, string pMetaDescripcion)
        {
            //Agregar a BD estos datos.
            AD.EntityModel.Models.Documentacion.DocumentoMetaDatos documentoMetaDatos = mEntityContext.DocumentoMetaDatos.FirstOrDefault(x => x.DocumentoID == pDocumento.Clave);
            if (documentoMetaDatos == null)
            {
                documentoMetaDatos = new AD.EntityModel.Models.Documentacion.DocumentoMetaDatos();
                documentoMetaDatos.MetaDescripcion = pMetaDescripcion;
                documentoMetaDatos.DocumentoID = pDocumento.Clave;
                documentoMetaDatos.Documento = pDocumento.FilaDocumento;
                mEntityContext.DocumentoMetaDatos.Add(documentoMetaDatos);
            }
            else
            {
                documentoMetaDatos.MetaDescripcion = pMetaDescripcion;
            }

            mEntityContext.SaveChanges();
            return documentoMetaDatos;
        }

        #endregion

        #region Metodos de editores

        /// <summary>
        /// Agrega un editor a un recurso
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pPerfilEditorID">Identificador del perfil del editor</param>
        /// <returns></returns>
        public EditorRecurso AgregarEditorARecurso(Guid pDocumentoID, Guid pPerfilEditorID)
        {
            return AgregarLectorEditorARecurso(pDocumentoID, pPerfilEditorID, true);
        }

        /// <summary>
        /// Agrega un lector a un recurso
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pPerfilEditorID">Identificador del perfil del editor</param>
        /// <returns></returns>
        public EditorRecurso AgregarLectorARecurso(Guid pDocumentoID, Guid pPerfilEditorID)
        {
            return AgregarLectorEditorARecurso(pDocumentoID, pPerfilEditorID, false);
        }

        /// <summary>
        /// Agrega un grupo editor a un recurso
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pGrupoEditorID">Identificador del grupo editor</param>
        /// <returns>GrupoEditorRecurso añadido. Null si el grupo ya existía</returns>
        public GrupoEditorRecurso AgregarGrupoEditorARecurso(Guid pDocumentoID, Guid pGrupoEditorID)
        {
            return AgregarGrupoLectorEditorARecurso(pDocumentoID, pGrupoEditorID, true);
        }

        /// <summary>
        /// Agrega un lector editor a un recurso
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pGrupoEditorID">Identificador del grupo editor</param>
        /// <returns>GrupoEditorRecurso añadido. Null si el grupo ya existía</returns>
        public GrupoEditorRecurso AgregarGrupoLectorARecurso(Guid pDocumentoID, Guid pGrupoEditorID)
        {
            return AgregarGrupoLectorEditorARecurso(pDocumentoID, pGrupoEditorID, false);
        }

        /// <summary>
        /// Elimina un grupo editor a un recurso
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pGrupoEditorID">Identificador del grupo editor</param>
        /// <returns>GrupoEditorRecurso añadido. Null si el grupo ya existía</returns>
        public void EliminarGrupoEditorARecurso(Guid pDocumentoID, Guid pGrupoEditorID)
        {
            EliminarGrupoLectorEditorARecurso(pDocumentoID, pGrupoEditorID, true);
        }

        /// <summary>
        /// Agrega un lector a un recurso
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pGrupoEditorID">Identificador del grupo editor</param>
        /// <returns>GrupoEditorRecurso añadido. Null si el grupo ya existía</returns>
        public void EliminarLectorEditorARecurso(Guid pDocumentoID, Guid pPerfilEditorID)
        {
            EliminarLectorEditorARecurso(pDocumentoID, pPerfilEditorID, false);
        }

        /// <summary>
        /// Actualiza el número de comentarios de un documento
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pEliminado">Verdad si se a eliminado un comentario, falso en caso contrario</param>
        /// <param name="pEnPrivado">Verdad si el comentario se ha hecho en una comunidad privada</param>
        public void ActualizarNumeroComentariosDocumento(Guid pDocumentoID, bool pEliminado, bool pEnPrivado)
        {
            AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaDocVinBR = null;

            List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filasDocVinBR = DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(pDocumentoID) && doc.BaseRecursosID.Equals(BaseRecursosIDActual)).ToList();

            if ((filasDocVinBR != null) && (filasDocVinBR.Count > 0))
            {
                filaDocVinBR = filasDocVinBR.First();
            }

            if (pEliminado)
            {
                if (!pEnPrivado)
                {
                    ListaDocumentos[pDocumentoID].FilaDocumento.NumeroComentariosPublicos--;
                }

                if (filaDocVinBR != null)
                {
                    filaDocVinBR.NumeroComentarios--;
                }
            }
            else
            {
                if (!pEnPrivado)
                {
                    ListaDocumentos[pDocumentoID].FilaDocumento.NumeroComentariosPublicos++;
                }

                if (filaDocVinBR != null)
                {
                    filaDocVinBR.NumeroComentarios++;
                }
            }
        }

        /// <summary>
        /// Agrega un editor a un recurso
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pPerfilEditorID">Identificador del perfil del editor</param>
        /// <param name="pEsEditor">Verdad si es editor, falso si sólo es lector</param>
        /// <returns></returns>
        private EditorRecurso AgregarLectorEditorARecurso(Guid pDocumentoID, Guid pPerfilEditorID, bool pEsEditor)
        {
            AD.EntityModel.Models.Documentacion.DocumentoRolIdentidad filaEditor = DataWrapperDocumentacion.ListaDocumentoRolIdentidad.FirstOrDefault(doc => doc.DocumentoID.Equals(pDocumentoID) && doc.PerfilID.Equals(pPerfilEditorID));
            EditorRecurso editor = null;

            if (filaEditor == null)
            {
                //Creo la fila
                filaEditor = new AD.EntityModel.Models.Documentacion.DocumentoRolIdentidad();
                filaEditor.DocumentoID = pDocumentoID;
                filaEditor.PerfilID = pPerfilEditorID;
                filaEditor.Editor = pEsEditor;

                //Creo el objeto editor y lo agrego a la lista de editores del documento
                editor = new EditorRecurso(filaEditor, this);
                if (ListaDocumentos.ContainsKey(pDocumentoID))
                {
                    ListaDocumentos[pDocumentoID].ListaPerfilesEditores.Add(filaEditor.PerfilID, editor);

                    if (pEsEditor && !ListaDocumentos[pDocumentoID].ListaPerfilesEditoresSinLectores.ContainsKey(filaEditor.PerfilID))
                    {
                        ListaDocumentos[pDocumentoID].ListaPerfilesEditoresSinLectores.Add(filaEditor.PerfilID, editor);
                    }
                    else if (!pEsEditor && !ListaDocumentos[pDocumentoID].ListaPerfilesLectores.ContainsKey(filaEditor.PerfilID))
                    {
                        LectorRecurso lector = new LectorRecurso(filaEditor, this);
                        ListaDocumentos[pDocumentoID].ListaPerfilesLectores.Add(filaEditor.PerfilID, lector);
                    }
                }

                //Agrego la fila del editor al dataset
                DataWrapperDocumentacion.ListaDocumentoRolIdentidad.Add(filaEditor);
                mEntityContext.DocumentoRolIdentidad.Add(filaEditor);

            }
            else
            {
                Documento doc = ListaDocumentos[pDocumentoID];
                editor = doc.ListaPerfilesEditores[pPerfilEditorID];

                if (editor.FilaEditor.Editor != pEsEditor)
                {
                    editor.FilaEditor.Editor = pEsEditor;
                    if (pEsEditor)
                    {
                        // Era lector, le hago editor
                        doc.ListaPerfilesLectores.Remove(pPerfilEditorID);
                        doc.ListaPerfilesEditoresSinLectores.Add(pPerfilEditorID, editor);
                    }
                    else
                    {
                        // Era editor, le hago lector
                        doc.ListaPerfilesEditoresSinLectores.Remove(pPerfilEditorID);
                        doc.ListaPerfilesLectores.Add(pPerfilEditorID, new LectorRecurso(filaEditor, this));
                    }
                }
            }

            return editor;
        }

        /// <summary>
        /// Agrega un grupo editor a un recurso
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pGrupoEditorID">Identificador del grupo editor</param>
        /// <param name="pEsEditor">Verdad si es editor, falso si sólo es lector</param>
        /// <returns></returns>
        private GrupoEditorRecurso AgregarGrupoLectorEditorARecurso(Guid pDocumentoID, Guid pGrupoEditorID, bool pEsEditor)
        {
            GrupoEditorRecurso grupoEditor = null;
            AD.EntityModel.Models.Documentacion.DocumentoRolGrupoIdentidades documentoRolGrupoIdentidades = mEntityContext.DocumentoRolGrupoIdentidades.FirstOrDefault(doc => doc.DocumentoID.Equals(pDocumentoID) && doc.GrupoID.Equals(pGrupoEditorID));
            if (DataWrapperDocumentacion.ListaDocumentoRolGrupoIdentidades.Find(doc => doc.DocumentoID.Equals(pDocumentoID) && doc.GrupoID.Equals(pGrupoEditorID)) == null)
            {
                //Creo la fila
                AD.EntityModel.Models.Documentacion.DocumentoRolGrupoIdentidades filaGrupoEditor = new AD.EntityModel.Models.Documentacion.DocumentoRolGrupoIdentidades();
                filaGrupoEditor.DocumentoID = pDocumentoID;
                filaGrupoEditor.GrupoID = pGrupoEditorID;
                filaGrupoEditor.Editor = pEsEditor;

                //Creo el objeto grupo editor y lo agrego a la lista de grupos de editores del documento
                grupoEditor = new GrupoEditorRecurso(filaGrupoEditor, this);
                if (ListaDocumentos.ContainsKey(pDocumentoID) && !ListaDocumentos[pDocumentoID].ListaGruposEditores.ContainsKey(filaGrupoEditor.GrupoID))
                {
                    ListaDocumentos[pDocumentoID].ListaGruposEditores.Add(filaGrupoEditor.GrupoID, grupoEditor);

                    if (pEsEditor && !ListaDocumentos[pDocumentoID].ListaGruposEditoresSinLectores.ContainsKey(filaGrupoEditor.GrupoID))
                    {
                        ListaDocumentos[pDocumentoID].ListaGruposEditoresSinLectores.Add(filaGrupoEditor.GrupoID, grupoEditor);
                    }
                    else if (!pEsEditor && !ListaDocumentos[pDocumentoID].ListaGruposLectores.ContainsKey(filaGrupoEditor.GrupoID))
                    {
                        GrupoLectorRecurso grupoLector = new GrupoLectorRecurso(filaGrupoEditor, this);
                        ListaDocumentos[pDocumentoID].ListaGruposLectores.Add(filaGrupoEditor.GrupoID, grupoLector);
                    }
                }


                mEntityContext.DocumentoRolGrupoIdentidades.Add(filaGrupoEditor);
                DataWrapperDocumentacion.ListaDocumentoRolGrupoIdentidades.Add(filaGrupoEditor);
            }
            else
            {
                AD.EntityModel.Models.Documentacion.DocumentoRolGrupoIdentidades filaGrupoEditor = DataWrapperDocumentacion.ListaDocumentoRolGrupoIdentidades.Find(doc => doc.DocumentoID.Equals(pDocumentoID) && doc.GrupoID.Equals(pGrupoEditorID));

                if (filaGrupoEditor.Editor != pEsEditor)
                {
                    filaGrupoEditor.Editor = pEsEditor;
                }

                if (ListaDocumentos.ContainsKey(pDocumentoID) && ListaDocumentos[pDocumentoID].ListaGruposEditores.ContainsKey(filaGrupoEditor.GrupoID))
                {
                    grupoEditor = ListaDocumentos[pDocumentoID].ListaGruposEditores[filaGrupoEditor.GrupoID];
                }
            }

            return grupoEditor;
        }

        /// <summary>
        /// Agrega un grupo editor a un recurso
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pGrupoEditorID">Identificador del grupo editor</param>
        /// <param name="pEsEditor">Verdad si es editor, falso si sólo es lector</param>
        /// <returns></returns>
        private void EliminarGrupoLectorEditorARecurso(Guid pDocumentoID, Guid pGrupoEditorID, bool pEsEditor)
        {
            AD.EntityModel.Models.Documentacion.DocumentoRolGrupoIdentidades filaGrupo = DataWrapperDocumentacion.ListaDocumentoRolGrupoIdentidades.Find(doc => doc.DocumentoID.Equals(pDocumentoID) && doc.GrupoID.Equals(pGrupoEditorID));
            if (filaGrupo != null)
            {
                mEntityContext.EliminarElemento(filaGrupo);
                DataWrapperDocumentacion.ListaDocumentoRolGrupoIdentidades.Remove(filaGrupo);
            }
            else if (filaGrupo == null)
            {
                throw new ArgumentException("The editor doesn't exists");
            }
            else if (filaGrupo.Editor.Equals(pEsEditor))
            {
                throw new ArgumentException("The user is a reader, isn't a editor");
            }
        }

        /// <summary>
        /// Elimina un lector editor a un recurso
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pGrupoEditorID">Identificador del grupo editor</param>
        /// <param name="pEsEditor">Verdad si es editor, falso si sólo es lector</param>
        /// <returns></returns>
        private void EliminarLectorEditorARecurso(Guid pDocumentoID, Guid pPerfilEditorID, bool pEsEditor)
        {//FindByDocumentoIDPerfilID(pDocumentoID, pPerfilEditorID);
            var filaEditor = DataWrapperDocumentacion.ListaDocumentoRolIdentidad.Find(doc => doc.DocumentoID.Equals(pDocumentoID) && doc.PerfilID.Equals(pPerfilEditorID));
            if (filaEditor != null && filaEditor.Editor.Equals(pEsEditor))
            {
                DataWrapperDocumentacion.ListaDocumentoRolIdentidad.Remove(filaEditor);
                mEntityContext.EliminarElemento(filaEditor);
            }
            else if (filaEditor == null)
            {
                throw new ArgumentException("The editor doesn't exists");
            }
            else if (filaEditor.Editor.Equals(pEsEditor))
            {
                throw new ArgumentException("The user is a reader, isn't a editor");
            }
        }

        /// <summary>
        /// Agrega una respuesta a un recurso
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pRespuestaID">Identificador de la respuesta</param>
        /// <param name="pRespuesta">Respuesta</param>
        /// <returns></returns>
        public RespuestaRecurso AgregarRespuestaARecurso(Guid pDocumentoID, Guid pRespuestaID, string pRespuesta, short pOrden)
        {
            RespuestaRecurso respuesta = null;
            //Creo la fila
            if (DataWrapperDocumentacion != null) //TODO Migrar EF
            {
                AD.EntityModel.Models.Documentacion.DocumentoRespuesta filaRespuesta = new AD.EntityModel.Models.Documentacion.DocumentoRespuesta();
                filaRespuesta.DocumentoID = pDocumentoID;
                filaRespuesta.RespuestaID = pRespuestaID;
                filaRespuesta.Descripcion = pRespuesta;
                filaRespuesta.NumVotos = 0;
                filaRespuesta.Orden = pOrden;

                //Creo el objeto respuesta y lo agrego a la lista de respuestas del documento
                respuesta = new RespuestaRecurso(filaRespuesta, this);
                if (ListaDocumentos.ContainsKey(pDocumentoID))
                {
                    ListaDocumentos[pDocumentoID].ListaRespuestas.Add(filaRespuesta.RespuestaID, respuesta);
                }

                DataWrapperDocumentacion.ListaDocumentoRespuesta.Add(filaRespuesta);
                mEntityContext.DocumentoRespuesta.Add(filaRespuesta);
            }

            return respuesta;
        }

        /// <summary>
        /// Elimina una respuesta de un recurso tipo encuesta
        /// </summary>
        /// <param name="pRespuesta">Respuesta a eliminar</param>
        public void QuitarRespuestaDeRecurso(RespuestaRecurso pRespuesta)
        {
            if ((pRespuesta.DocumentoEditado != null) && (pRespuesta.DocumentoEditado.ListaRespuestas.ContainsKey(pRespuesta.FilaRespuesta.RespuestaID)))
            {
                pRespuesta.DocumentoEditado.ListaRespuestas.Remove(pRespuesta.FilaRespuesta.RespuestaID);
            }

            mEntityContext.EliminarElemento(pRespuesta.FilaRespuesta);
            DataWrapperDocumentacion.ListaDocumentoRespuesta.Remove(pRespuesta.FilaRespuesta);
        }

        /// <summary>
        /// Agrega un voto a una respuesta de recurso(encuesta)
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pRespuestaID">Identificador de la respuesta</param>
        /// <param name="pIdentidadID">Identificador del votante</param>
        /// <returns></returns>
        public void AgregarVotoARespuestaDeRecurso(Guid pDocumentoID, Guid pRespuestaID, Guid pIdentidadID)
        {
            //Creo la fila
            if (DataWrapperDocumentacion != null) //TODO Migrar EF
            {
                AD.EntityModel.Models.Documentacion.DocumentoRespuestaVoto filaRespuestaVoto = new AD.EntityModel.Models.Documentacion.DocumentoRespuestaVoto();
                filaRespuestaVoto.DocumentoID = pDocumentoID;
                filaRespuestaVoto.RespuestaID = pRespuestaID;
                filaRespuestaVoto.IdentidadID = pIdentidadID;

                AD.EntityModel.Models.Documentacion.DocumentoRespuesta filaRespuesta = DataWrapperDocumentacion.ListaDocumentoRespuesta.Where(item => item.RespuestaID.Equals(pRespuestaID)).FirstOrDefault();
                filaRespuesta.NumVotos++;

                //Agrego la fila del voto
                DataWrapperDocumentacion.ListaDocumentoRespuestaVoto.Add(filaRespuestaVoto);
                mEntityContext.DocumentoRespuestaVoto.Add(filaRespuestaVoto);
            }

        }

        /// <summary>
        /// Limpia los grupos editores de un documento
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        public void LimpiarGruposEditores(Guid pDocumentoID)
        {
            List<AD.EntityModel.Models.Documentacion.DocumentoRolGrupoIdentidades> listaDocumentoRolGrupoIdentidades = DataWrapperDocumentacion.ListaDocumentoRolGrupoIdentidades.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).ToList();
            foreach (AD.EntityModel.Models.Documentacion.DocumentoRolGrupoIdentidades filaLimpiar in listaDocumentoRolGrupoIdentidades)
            {
                mEntityContext.EliminarElemento(filaLimpiar);
                DataWrapperDocumentacion.ListaDocumentoRolGrupoIdentidades.Remove(filaLimpiar);
            }
        }


        /// <summary>
        /// Limpia los editores de un documento
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        public void LimpiarEditores(Guid pDocumentoID)
        {
            List<AD.EntityModel.Models.Documentacion.DocumentoRolIdentidad> listaDocumentoRolIdentidad = DataWrapperDocumentacion.ListaDocumentoRolIdentidad.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).ToList();
            foreach (AD.EntityModel.Models.Documentacion.DocumentoRolIdentidad filaLimpiar in listaDocumentoRolIdentidad)
            {
                mEntityContext.EliminarElemento(filaLimpiar);
                DataWrapperDocumentacion.ListaDocumentoRolIdentidad.Remove(filaLimpiar);
            }
        }

        /// <summary>
        /// Limpia los editores de un documento
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns></returns>
        public void LimpiarSoloEditores(Guid pDocumentoID)
        {
            List<AD.EntityModel.Models.Documentacion.DocumentoRolIdentidad> listaDocumentoRolIdentidad = DataWrapperDocumentacion.ListaDocumentoRolIdentidad.Where(doc => doc.DocumentoID.Equals(pDocumentoID) && doc.Editor).ToList();
            foreach (AD.EntityModel.Models.Documentacion.DocumentoRolIdentidad filaLimpiar in listaDocumentoRolIdentidad)
            {
                mEntityContext.EliminarElemento(filaLimpiar);
                DataWrapperDocumentacion.ListaDocumentoRolIdentidad.Remove(filaLimpiar);
            }
        }

        /// <summary>
        /// Limpia los editores de un documento
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns></returns>
        public void LimpiarSoloLectores(Guid pDocumentoID)
        {
            List<AD.EntityModel.Models.Documentacion.DocumentoRolIdentidad> listaDocumentoRolIdentidad = DataWrapperDocumentacion.ListaDocumentoRolIdentidad.Where(doc => doc.DocumentoID.Equals(pDocumentoID) && doc.Editor == false).ToList();
            foreach (AD.EntityModel.Models.Documentacion.DocumentoRolIdentidad filaLimpiar in listaDocumentoRolIdentidad)
            {
                mEntityContext.EliminarElemento(filaLimpiar);
                DataWrapperDocumentacion.ListaDocumentoRolIdentidad.Remove(filaLimpiar);
            }
        }

        /// <summary>
        /// Limpia los editores de un documento
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns></returns>
        public void LimpiarSoloGruposEditores(Guid pDocumentoID)
        {
            if (DataWrapperDocumentacion != null)
            {
                List<AD.EntityModel.Models.Documentacion.DocumentoRolGrupoIdentidades> listaDocRolGrupoIdentidades = DataWrapperDocumentacion.ListaDocumentoRolGrupoIdentidades.Where(item => item.DocumentoID.Equals(pDocumentoID) && item.Editor.Equals(true)).ToList();
                foreach (AD.EntityModel.Models.Documentacion.DocumentoRolGrupoIdentidades filaLimpiar in listaDocRolGrupoIdentidades)
                {
                    mEntityContext.EliminarElemento(filaLimpiar);
                    DataWrapperDocumentacion.ListaDocumentoRolGrupoIdentidades.Remove(filaLimpiar);
                }
            }

        }

        /// <summary>
        /// Limpia los editores de un documento
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns></returns>
        public void LimpiarSoloGruposLectores(Guid pDocumentoID)
        {
            if (DataWrapperDocumentacion != null)
            {
                List<AD.EntityModel.Models.Documentacion.DocumentoRolGrupoIdentidades> listaDocumentoRolGrupoIdentidades = DataWrapperDocumentacion.ListaDocumentoRolGrupoIdentidades.Where(item => item.DocumentoID.Equals(pDocumentoID) && item.Editor.Equals(false)).ToList();
                foreach (AD.EntityModel.Models.Documentacion.DocumentoRolGrupoIdentidades filaLimpiar in listaDocumentoRolGrupoIdentidades)
                {
                    DataWrapperDocumentacion.ListaDocumentoRolGrupoIdentidades.Remove(filaLimpiar);
                    mEntityContext.EliminarElemento(filaLimpiar);
                }
            }

        }

        /// <summary>
        /// Elimina a un usuario como editor de un recurso
        /// </summary>
        /// <param name="pEditor">Editor a eliminar</param>
        public void QuitarEditorDeRecurso(EditorRecurso pEditor)
        {
            if ((pEditor.DocumentoEditado != null) && (pEditor.DocumentoEditado.ListaPerfilesEditores.ContainsKey(pEditor.FilaEditor.PerfilID)))
            {
                pEditor.DocumentoEditado.ListaPerfilesEditores.Remove(pEditor.FilaEditor.PerfilID);
            }
            mEntityContext.EliminarElemento(pEditor.FilaEditor);
        }

        /// <summary>
        /// Elimina a un grupo como editor de un recurso
        /// </summary>
        /// <param name="pEditor">Editor a eliminar</param>
        public void QuitarGrupoEditorDeRecurso(GrupoEditorRecurso pGrupoEditor)
        {
            if ((pGrupoEditor.DocumentoEditado != null) && (pGrupoEditor.DocumentoEditado.ListaPerfilesEditores.ContainsKey(pGrupoEditor.FilaGrupoEditor.GrupoID)))
            {
                pGrupoEditor.DocumentoEditado.ListaPerfilesEditores.Remove(pGrupoEditor.FilaGrupoEditor.GrupoID);
            }

            mEntityContext.EliminarElemento(pGrupoEditor.FilaGrupoEditor);
            DataWrapperDocumentacion.ListaDocumentoRolGrupoIdentidades.Remove(pGrupoEditor.FilaGrupoEditor);
        }

        #endregion

        #region NewsLetter

        /// <summary>
        /// Agrega una fila de envio de newsLetter a la tabla.
        /// </summary>
        /// <param name="pDocumento">Documento NewsLetter</param>
        /// <param name="pIdioma">Idioma en el que se envia la newsLetter</param>
        public AD.EntityModel.Models.Documentacion.DocumentoEnvioNewsLetter AgregarEnvioNewsLetter(Documento pDocumento, string pIdioma, List<Guid> pListaGruposID, Guid pIdentidadID)
        {
            string listaIDs = string.Empty;
            if (pListaGruposID != null && pListaGruposID.Count > 0)
            {
                string separador = string.Empty;
                foreach (Guid grupoID in pListaGruposID)
                {
                    listaIDs += separador + grupoID.ToString();
                    separador = ",";
                }
            }
            if (DataWrapperDocumentacion != null)
            {
                AD.EntityModel.Models.Documentacion.DocumentoEnvioNewsLetter filaEnvioNews = new AD.EntityModel.Models.Documentacion.DocumentoEnvioNewsLetter();

                filaEnvioNews.DocumentoID = pDocumento.Clave;
                filaEnvioNews.Fecha = DateTime.Now;
                filaEnvioNews.IdentidadID = pIdentidadID;
                filaEnvioNews.Idioma = pIdioma;
                filaEnvioNews.EnvioRealizado = false;
                filaEnvioNews.EnvioSolicitado = false;
                filaEnvioNews.Grupos = listaIDs;
                DataWrapperDocumentacion.ListaDocumentoEnvioNewsLetter.Add(filaEnvioNews);
                mEntityContext.DocumentoEnvioNewsLetter.Add(filaEnvioNews);

                return filaEnvioNews;
            }

            return null;
        }

        #endregion

        #region Comentarios

        /// <summary>
        /// Obtiene el proyecto en el que se ha hecho un comentario a un documento.
        /// </summary>
        /// <param name="pComentarioID">Identificador del comentario</param>
        /// <param name="pDocumentoID">Identificador del proyecto</param>
        /// <returns>Identificador del proyecto donde se ha hecho el comentario</returns>
        public Guid ObtenerProyectoDeComentario(Guid pComentarioID, Guid pDocumentoID)
        {
            if (DataWrapperDocumentacion != null) //TODO Migrar EF
            {
                List<AD.EntityModel.Models.Documentacion.DocumentoComentario> filasDocComent = DataWrapperDocumentacion.ListaDocumentoComentario.Where(item => item.DocumentoID.Equals(pDocumentoID) && item.ComentarioID.Equals(pComentarioID)).ToList();

                if (filasDocComent.Count > 0)
                {
                    return filasDocComent.FirstOrDefault().ProyectoID.Value;
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Obtiene el número de comentarios realizados a un recurso en un proyecto
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <returns>Número de comentarios</returns>
        public int ObtenerNumeroComentariosDeDocumentodEnProyecto(Guid pDocumentoID)
        {
            List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filas = DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(pDocumentoID) && doc.BaseRecursosID.Equals(BaseRecursosIDActual)).ToList();

            if (filas.Count > 0)
            {
                return filas.First().NumeroComentarios;
            }
            return 0;
        }

        #endregion

        #region Documentos Vinculados

        /// <summary>
        /// Vincula un documento a otro.
        /// </summary>
        /// <param name="pDocumento1ID">ID del documento base</param>
        /// <param name="pDocumento2ID">ID del documento vinculado</param>
        /// <param name="pIdentidadActual">ID del autor de la vinculación</param>
        public void VincularDocumentos(Guid pDocumento1ID, Guid pDocumento2ID, Guid pIdentidadActual)
        {
            VincularDocumentoADocumento(pDocumento1ID, pDocumento2ID, pIdentidadActual);
            VincularDocumentoADocumento(pDocumento2ID, pDocumento1ID, pIdentidadActual);
        }

        /// <summary>
        /// Vincula un documento a otro.
        /// </summary>
        /// <param name="pDocumentoID">ID del documento base</param>
        /// <param name="pDocumentoVinculadoID">ID del documento vinculado</param>
        /// <param name="pIdentidadActual">ID del autor de la vinculación</param>
        private void VincularDocumentoADocumento(Guid pDocumentoID, Guid pDocumentoVinculadoID, Guid pIdentidadActual)
        {
            if (DataWrapperDocumentacion != null)
            {
                if (DataWrapperDocumentacion.ListaDocumentoVincDoc.FirstOrDefault(item => item.DocumentoID.Equals(pDocumentoID) && item.DocumentoVincID.Equals(pDocumentoVinculadoID)) == null)
                {
                    AD.EntityModel.Models.Documentacion.DocumentoVincDoc filaDocVinc = new AD.EntityModel.Models.Documentacion.DocumentoVincDoc();

                    filaDocVinc.DocumentoID = pDocumentoID;
                    filaDocVinc.DocumentoVincID = pDocumentoVinculadoID;
                    filaDocVinc.IdentidadID = pIdentidadActual;
                    filaDocVinc.Fecha = DateTime.Now;

                    DataWrapperDocumentacion.ListaDocumentoVincDoc.Add(filaDocVinc);
                    mEntityContext.DocumentoVincDoc.Add(filaDocVinc);
                }
            }

        }

        /// <summary>
        /// Desvincula un documento de otro.
        /// </summary>
        /// <param name="pDocumento1ID">ID del documento base</param>
        /// <param name="pDocumento2ID">ID del documento vinculado</param>
        public void DesVincularDocumentos(Guid pDocumento1ID, Guid pDocumento2ID)
        {
            DesVincularDocumentoDeDocumento(pDocumento1ID, pDocumento2ID);
            DesVincularDocumentoDeDocumento(pDocumento2ID, pDocumento1ID);
        }

        /// <summary>
        /// Desvincula un documento de otro.
        /// </summary>
        /// <param name="pDocumentoID">ID del documento base</param>
        /// <param name="pDocumentoVinculadoID">ID del documento vinculado</param>
        private void DesVincularDocumentoDeDocumento(Guid pDocumentoID, Guid pDocumentoVinculadoID)
        {
            List<AD.EntityModel.Models.Documentacion.DocumentoVincDoc> listaBorrar = DataWrapperDocumentacion.ListaDocumentoVincDoc.Where(doc => doc.DocumentoID.Equals(pDocumentoID) && doc.DocumentoVincID.Equals(pDocumentoVinculadoID)).ToList();
            foreach (AD.EntityModel.Models.Documentacion.DocumentoVincDoc filaDocVinc in listaBorrar)
            {
                mEntityContext.EliminarElemento(filaDocVinc);
                DataWrapperDocumentacion.ListaDocumentoVincDoc.Remove(filaDocVinc);
            }
        }

        /// <summary>
        /// Comprueba si una determinada identidad puede desvincular dos documentos.
        /// </summary>
        /// <param name="pDocumento1ID">Documento 1</param>
        /// <param name="pDocumento2ID">Documento 2</param>
        /// <param name="pIdentidad">Identidad a comprobar</param>
        /// <param name="pIdentidadOrganizacion">Identidad de organización en caso de estar conectado como tal</param>
        /// <returns>TRUE si una determinada identidad puede desvincular dos documentos, FALSE en caso contrario</returns>
        public bool TienePermisosIdentidadDesvincularRecursos(Documento pDocumento1ID, Documento pDocumento2ID, Identidad.Identidad pIdentidad, Identidad.Identidad pIdentidadOrganizacion, Elementos.ServiciosGenerales.Proyecto pProyecto, Guid pUsuarioID, bool pEsAdministradorDeOrganizacion)
        {
            List<AD.EntityModel.Models.Documentacion.DocumentoVincDoc> filasDocVin = DataWrapperDocumentacion.ListaDocumentoVincDoc.Where(doc => doc.DocumentoID.Equals(pDocumento1ID.Clave) && doc.DocumentoVincID.Equals(pDocumento2ID.Clave)).ToList();

            if (filasDocVin.Count > 0)
            {
                if (pIdentidad.ListaTodosIdentidadesDeIdentidad.Contains(filasDocVin.First().IdentidadID))
                {
                    return true;
                }
            }

            return pDocumento1ID.TienePermisosEdicionIdentidad(pIdentidad, pIdentidadOrganizacion, pProyecto, pUsuarioID, pEsAdministradorDeOrganizacion) || pDocumento2ID.TienePermisosEdicionIdentidad(pIdentidad, pIdentidadOrganizacion, pProyecto, pUsuarioID, pEsAdministradorDeOrganizacion);
        }

        /// <summary>
        /// ID de la identidad que vinculó dos recursos.
        /// </summary>
        /// <param name="pDocumentoID">ID del documento base</param>
        /// <param name="pDocumentoVinculadoID">ID del documento vinculado</param>
        /// <returns>ID de la identidad que vinculó dos recursos</returns>
        public Guid ObtenerIDVinculadorRecursos(Guid pDocumentoID, Guid pDocumentoVinculadoID)
        {
            if (DataWrapperDocumentacion != null)
            {
                List<AD.EntityModel.Models.Documentacion.DocumentoVincDoc> filasDocVin = DataWrapperDocumentacion.ListaDocumentoVincDoc.Where(item => item.DocumentoID.Equals(pDocumentoID) && item.DocumentoVincID.Equals(pDocumentoVinculadoID)).ToList();

                if (filasDocVin.Count > 0)
                {
                    return filasDocVin.FirstOrDefault().IdentidadID;
                }
            }


            return Guid.Empty;
        }


        #endregion

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el DataWrapperDocumentacion
        /// </summary>
        public DataWrapperDocumentacion DataWrapperDocumentacion
        {
            get
            {
                return (DataWrapperDocumentacion)DataWrapper;
            }
            set
            {
                DataWrapper = value;
            }
        }
        /// <summary>
        /// Hijos del gestor, los documentos
        /// </summary>
        public override List<IElementoGnoss> Hijos
        {
            get
            {
                if (mHijos == null)
                {
                    CargarDocumentos();
                }
                return mHijos;
            }
        }

        /// <summary>
        /// Lista de documentos de tipo Web.
        /// </summary>
        public Dictionary<Guid, DocumentoWeb> ListaDocumentosWeb
        {
            get
            {
                if (mListaDocumentosWeb == null)
                {
                    CargarDocumentosWeb();
                }
                return mListaDocumentosWeb;
            }
        }

        /// <summary>
        /// Lista de documentos
        /// </summary>
        public Dictionary<Guid, Documento> ListaDocumentos
        {
            get
            {
                if (mListaDocumentos == null)
                {
                    mListaDocumentos = new Dictionary<Guid, Documento>();
                    if (DataWrapperDocumentacion != null)
                    {
                        CargarDocumentos(true);
                    }
                }
                return mListaDocumentos;
            }
        }

        /// <summary>
        /// Devuelve una lista con los documentos que se han subido al servidor, junto con la extensión y el tipo de archivo
        /// </summary>
        public Dictionary<Guid, KeyValuePair<string, TiposDocumentacion>> ListaDocumentosSubidosServidor
        {
            get
            {
                if (mListaDocumentosSubidosServidor == null)
                {
                    mListaDocumentosSubidosServidor = new Dictionary<Guid, KeyValuePair<string, TiposDocumentacion>>();
                }
                return mListaDocumentosSubidosServidor;
            }
        }

        /// <summary>
        /// Devuelve una lista con los documentos que están subidos al servidor y deben eliminarse, junto con la extensión y el tipo de archivo
        /// </summary>
        public Dictionary<Guid, KeyValuePair<string, TiposDocumentacion>> ListaDocumentosAEliminarDelServidor
        {
            get
            {
                if (mListaDocumentosAEliminarDelServidor == null)
                {
                    mListaDocumentosAEliminarDelServidor = new Dictionary<Guid, KeyValuePair<string, TiposDocumentacion>>();
                }
                return mListaDocumentosAEliminarDelServidor;
            }
        }

        /// <summary>
        /// Devuelve o establece el gestor de identidades de los documentos
        /// </summary>
        public GestionIdentidades GestorIdentidades
        {
            get
            {
                return mGestorIdentidades;
            }
            set
            {
                mGestorIdentidades = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el dataset de RDF.
        /// </summary>
        public RdfDS RdfDS
        {
            get
            {
                return mRdfDS;
            }
            set
            {
                mRdfDS = value;
            }
        }

        #region Documentos Web

        /// <summary>
        /// Obtiene la lista de caregorías que contiene el tesauro
        /// </summary>
        public SortedList<Guid, CategoriaTesauro> ListaCategoriasTesauro
        {
            get
            {
                if (this.GestorTesauro != null)
                {
                    return this.GestorTesauro.ListaCategoriasTesauro;
                }

                return new SortedList<Guid, CategoriaTesauro>();
            }
        }

        /// <summary>
        /// Obtiene el identificador de la wiki actual (del proyecto o de la organización)
        /// </summary>
        public Guid BaseRecursosIDActual
        {
            get
            {
                if ((DataWrapperDocumentacion != null) && (DataWrapperDocumentacion.ListaBaseRecursos.Count > 0))
                {
                    return DataWrapperDocumentacion.ListaBaseRecursos.First().BaseRecursosID;
                }
                else
                {
                    throw new Exception("No tiene Base de recurso, si quiere una póngase en contacto con su administrador.");
                }
            }
        }

        /// <summary>
        /// Indica si la propiedad 'BaseRecursosIDActual' devuelve el valor correcto.
        /// </summary>
        public bool EsBaseRecursosIDActualCorrecta(Guid pProyectoID)
        {
            if (pProyectoID == ProyectoAD.MetaProyecto)
            {
                return (DataWrapperDocumentacion.ListaBaseRecursosProyecto.Any(baserec => baserec.BaseRecursosID.Equals(BaseRecursosIDActual)));
            }
            else
            {
                return (DataWrapperDocumentacion.ListaBaseRecursosProyecto.Any(baserec => baserec.BaseRecursosID.Equals(BaseRecursosIDActual) && baserec.ProyectoID.Equals(pProyectoID)));
            }
        }

        /// <summary>
        /// Devueleve TRUE si hay una base de recursos cargada en el dataSet, FALSE en caso contrario.
        /// </summary>
        public bool HayBaseRecursosCargada
        {
            get
            {
                return (DataWrapperDocumentacion.ListaBaseRecursos.Count > 0);
            }
        }

        /// <summary>
        /// Espacion actual en la base de recursos de My Gnoss.
        /// </summary>
        public double EspacioActualBaseRecursos
        {
            get
            {
                if (DataWrapperDocumentacion != null)
                {
                    if ((DataWrapperDocumentacion != null) && (DataWrapperDocumentacion.ListaBaseRecursosUsuario.Count > 0))
                    {
                        return DataWrapperDocumentacion.ListaBaseRecursosUsuario.FirstOrDefault().EspacioActualMyGnossMB.Value;
                    }
                    else if ((DataWrapperDocumentacion != null) && (DataWrapperDocumentacion.ListaBaseRecursosOrganizacion.Count > 0))
                    {
                        return DataWrapperDocumentacion.ListaBaseRecursosOrganizacion.FirstOrDefault().EspacioActualMyGnossMB.Value;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    return -1;
                }

            }
            set
            {
                if (DataWrapperDocumentacion != null)
                {
                    if (DataWrapperDocumentacion != null && DataWrapperDocumentacion.ListaBaseRecursosUsuario.Count > 0)
                    {
                        DataWrapperDocumentacion.ListaBaseRecursosUsuario.FirstOrDefault().EspacioActualMyGnossMB = value;
                    }
                    else if (DataWrapperDocumentacion != null && DataWrapperDocumentacion.ListaBaseRecursosOrganizacion.Count > 0)
                    {
                        DataWrapperDocumentacion.ListaBaseRecursosOrganizacion.FirstOrDefault().EspacioActualMyGnossMB = value;
                    }
                }

            }
        }

        /// <summary>
        /// Espacion actual en la base de recursos de My Gnoss.
        /// </summary>
        public double EspacioMaximoBaseRecursos
        {
            get
            {
                if (DataWrapperDocumentacion != null)
                {
                    if (DataWrapperDocumentacion != null && DataWrapperDocumentacion.ListaBaseRecursosUsuario.Count > 0)
                    {
                        return DataWrapperDocumentacion.ListaBaseRecursosUsuario.FirstOrDefault().EspacioMaxMyGnossMB.Value;
                    }
                    else if (DataWrapperDocumentacion != null && DataWrapperDocumentacion.ListaBaseRecursosOrganizacion.Count > 0)
                    {
                        return DataWrapperDocumentacion.ListaBaseRecursosOrganizacion.FirstOrDefault().EspacioMaxMyGnossMB.Value;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    return -1;
                }

            }
            set
            {
                if (DataWrapperDocumentacion != null)
                {
                    if (DataWrapperDocumentacion != null && DataWrapperDocumentacion.ListaBaseRecursosUsuario.Count > 0)
                    {
                        DataWrapperDocumentacion.ListaBaseRecursosUsuario.FirstOrDefault().EspacioMaxMyGnossMB = value;
                    }
                    else if (DataWrapperDocumentacion != null && DataWrapperDocumentacion.ListaBaseRecursosOrganizacion.Count > 0)
                    {
                        DataWrapperDocumentacion.ListaBaseRecursosOrganizacion.FirstOrDefault().EspacioMaxMyGnossMB = value;
                    }
                }

            }
        }

        /// <summary>
        /// Obtiene o establece el gestor de tesauro
        /// </summary>
        public GestionTesauro GestorTesauro
        {
            get
            {
                return mGestorTesauro;
            }
            set
            {
                mGestorTesauro = value;

                if (mGestorTesauro != null && mGestorTesauro.GestorDocumental == null)
                {
                    mGestorTesauro.GestorDocumental = this;
                }
            }
        }

        #endregion

        #region Comentarios y Votos

        /// <summary>
        /// Devuelve o esblece el gestor de comentarios de documento.
        /// </summary>
        public GestionComentariosDocumento GestionComentarios
        {
            get
            {
                return mGestionComentariosDocumento;
            }
            set
            {
                mGestionComentariosDocumento = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el gestor de votos de documento.
        /// </summary>
        public GestionVotosDocumento GestorVotos
        {
            get
            {
                return mGestorVotos;
            }
            set
            {
                mGestorVotos = value;
                if (mGestorVotos.GestorDocumental == null)
                {
                    mGestorVotos.GestorDocumental = this;
                }
            }
        }

        #endregion

        #region Categorias Tesauro

        /// <summary>
        /// Devuelve la categoría privada de la base de recursos cargada.
        /// </summary>
        public Guid CategoriaTesauroPrivadaID
        {
            get
            {
                if (mCategoriaTesauroPrivadaID == null)
                {
                    mCategoriaTesauroPrivadaID = Guid.Empty;
                    if (GestorTesauro != null)
                    {
                        if (this.GestorTesauro.TesauroDW.ListaTesauroUsuario.Count > 0)
                        {
                            mCategoriaTesauroPrivadaID = (this.GestorTesauro.TesauroDW.ListaTesauroUsuario.FirstOrDefault()).CategoriaTesauroPrivadoID.Value;
                        }
                        else if (this.GestorTesauro.TesauroDW.ListaTesauroOrganizacion.Count > 0)
                        {
                            mCategoriaTesauroPrivadaID = this.GestorTesauro.TesauroDW.ListaTesauroOrganizacion.FirstOrDefault().CategoriaTesauroPrivadoID;
                        }
                    }
                }
                return mCategoriaTesauroPrivadaID.Value;
            }
        }

        /// <summary>
        /// Lista (ID Documento antigüo)-(ID Documento Nuevo) de los documentos que han sido duplicados dentro de la función DuplicarDocumentos
        /// </summary>
        public List<KeyValuePair<Guid, Guid>> ListaDocumentosCopiados
        {
            get
            {
                return mListaDocumentosCopiados;
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
        ~GestorDocumental()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                try
                {
                    if (disposing)
                    {
                        //Libero todos los recursos administrados que he añadido a esta clase

                        if (mListaDocumentos != null)
                        {
                            foreach (Documento doc in mListaDocumentos.Values)
                            {
                                doc.Dispose();
                            }
                            mListaDocumentos.Clear();
                        }

                        if (mListaDocumentosCopiados != null)
                        {
                            mListaDocumentosCopiados.Clear();
                        }   

                        if (mListaDocumentosDestacados != null)
                        {
                            mListaDocumentosDestacados.Clear();
                        }   

                        if (mListaDocumentosSubidosServidor != null)
                        {
                            mListaDocumentosSubidosServidor.Clear();
                        }   

                        if (mListaDocumentosAEliminarDelServidor != null)
                        {
                            mListaDocumentosAEliminarDelServidor.Clear();
                        }   

                        if (mRdfDS != null)
                        {
                            mRdfDS.Dispose();
                        }   
                    }
                }
                finally
                {
                    mListaDocumentos = null;
                    mListaDocumentosCopiados = null;
                    mListaDocumentosDestacados = null;
                    mListaDocumentosSubidosServidor = null;
                    mListaDocumentosAEliminarDelServidor = null;
                    mRdfDS = null;

                    // Llamo al dispose de la clase base
                    base.Dispose(disposing);
                }
            }
        }

        #endregion

        #region Miembros de ISerializable

        /// <summary>
        /// Metodo para serializar el objeto
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("GestionComentarios", GestionComentarios);
            info.AddValue("GestorTesauro", GestorTesauro);
            info.AddValue("GestorVotos", GestorVotos);
            info.AddValue("GestorIdentidades", GestorIdentidades);
        }

        #endregion
    }
}
