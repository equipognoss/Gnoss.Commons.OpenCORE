using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.RDF.Model;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Documentacion.FichaDocumento;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Es.Riam.Gnoss.Elementos.Documentacion
{
    #region Enumeraciones

    /// <summary>
    /// Estados de fichero
    /// </summary>
    public enum EstadoFichero
    {
        /// <summary>
        /// Estado normal
        /// </summary>
        Normal = 0,
        /// <summary>
        /// El fichero se está subiendo al servidor
        /// </summary>
        Subiendo = 1,
        /// <summary>
        /// El fichero se está bajando del servidor
        /// </summary>
        Bajando = 2
    }

    #endregion

    /// <summary>
    /// Documento que no está en una categoría
    /// </summary>
    public class Documento : ElementoGnoss, IDisposable
    {
        #region Miembros

        /// <summary>
        /// Lista de editores del documento
        /// </summary>
        private Dictionary<Guid, EditorRecurso> mListaPerfilesEditores;

        /// <summary>
        /// Lista de lectores del documento
        /// </summary>
        private Dictionary<Guid, LectorRecurso> mListaPerfilesLectores;

        /// <summary>
        /// Lista de editores del documento, sin los lectores.
        /// </summary>
        private Dictionary<Guid, EditorRecurso> mListaPerfilesEditoresSinLectores;

        /// <summary>
        /// Lista de grupos editores del documento
        /// </summary>
        private Dictionary<Guid, GrupoEditorRecurso> mListaGruposEditores;

        /// <summary>
        /// Lista de grupos lectores del documento
        /// </summary>
        private Dictionary<Guid, GrupoLectorRecurso> mListaGruposLectores;

        /// <summary>
        /// Lista de grupos editores del documento, sin los lectores.
        /// </summary>
        private Dictionary<Guid, GrupoEditorRecurso> mListaGruposEditoresSinLectores;

        /// <summary>
        /// Lista de respuestas del documento
        /// </summary>
        private Dictionary<Guid, RespuestaRecurso> mListaRespuestas;

        /// <summary>
        /// variable temporal para saber si el documento se está subiendo al servidor o no
        /// </summary>
        private EstadoFichero mEstadoFichero;

        /// <summary>
        /// Contiene la ficha bibliografica del documento.
        /// </summary>
        protected FichaBibliografica mFichaBibliografica;

        /// <summary>
        /// True si el enlace del documento a sido cambiado.
        /// </summary>
        private bool mEnlaceDocumentoCambiado = false;

        /// <summary>
        /// Lista de votos del documento.
        /// </summary>
        private Dictionary<Guid, AD.EntityModel.Models.Voto.Voto> mListaVotos;

        /// <summary>
        /// Lista de comentarios de un documento.
        /// </summary>
        private List<Comentario.Comentario> mListaComentarios;

        /// <summary>
        /// Referencia al documento que es última versión del actual.
        /// </summary>
        private Documento mUltimaVersion = null;

        /// <summary>
        /// Lista de categorias de tesauro públicas a las que está agregado el documento
        /// </summary>
        private SortedList<Guid, CategoriaTesauro> mCategoriasTesauroPublicas;

        /// <summary>
        /// Indica que el nombre debe ser el del usuario actual.
        /// </summary>
        public static string NombreUsuarioActual = "UsuarioActual";

        /// <summary>
        /// Lista con los IDs de los documentos vinculados al actual.
        /// </summary>
        private List<Guid> mDocumentosVinculados;

        /// <summary>
        /// Lista con los proyectos a lo que pertenece el documento más su tipo de acceso.
        /// </summary>
        private List<Guid> mListaProyectos;

        /// <summary>
        /// Devuelve el número de tags que tiene el documento respecto a la búsqueda realizada.
        /// </summary>
        private int mNumeroTagCoinciden;

        /// <summary>
        /// Número de tags NO Coincidentes que tiene el documento respecto a la búsqueda realizada.
        /// </summary>
        private int mNumeroTagNoCoinciden;

        /// <summary>
        /// Lista de categorias
        /// </summary>
        private SortedList<Guid, CategoriaTesauro> mListaCategorias;

        /// <summary>
        /// Base de recursos donde están compartidos los documentos.
        /// </summary>
        private List<Guid> mBaseRecursos;

        /// <summary>
        /// Url de los recursos relacionados o contextos que deben aparecer en el RDF del documento.
        /// </summary>
        private List<string> mUrlRecursosRelacionadosRDF;

        private LoggingService mLoggingService;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor para el documento
        /// </summary>
        /// <param name="pDocumento">Fila del documento</param>
        /// <param name="pGestor">Gestor</param>
        public Documento(AD.EntityModel.Models.Documentacion.Documento pDocumento, GestionGnoss pGestor, LoggingService loggingService)
            : base(pDocumento, pGestor, loggingService)
        {
            mLoggingService = loggingService;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la lista de editores de este documento
        /// </summary>
        public Dictionary<Guid, EditorRecurso> ListaPerfilesEditores
        {
            get
            {
                if (mListaPerfilesEditores == null)
                {
                    mListaPerfilesEditores = new Dictionary<Guid, EditorRecurso>();

                    foreach (AD.EntityModel.Models.Documentacion.DocumentoRolIdentidad filaEditor in GestorDocumental.DataWrapperDocumentacion.ListaDocumentoRolIdentidad.Where(doc => doc.DocumentoID.Equals(this.Clave)))
                    {
                        mListaPerfilesEditores.Add(filaEditor.PerfilID, new EditorRecurso(filaEditor, this.GestorDocumental, mLoggingService));
                    }
                }
                return mListaPerfilesEditores;
            }
        }

        /// <summary>
        /// Obtiene la lista de lectores de este documento
        /// </summary>
        public Dictionary<Guid, LectorRecurso> ListaPerfilesLectores
        {
            get
            {
                if (mListaPerfilesLectores == null)
                {
                    mListaPerfilesLectores = new Dictionary<Guid, LectorRecurso>();
                    foreach (AD.EntityModel.Models.Documentacion.DocumentoRolIdentidad filaLector in GestorDocumental.DataWrapperDocumentacion.ListaDocumentoRolIdentidad.Where(doc => doc.DocumentoID.Equals(this.Clave) && !doc.Editor))
                    {
                        mListaPerfilesLectores.Add(filaLector.PerfilID, new LectorRecurso(filaLector, this.GestorDocumental, mLoggingService));
                    }
                }
                return mListaPerfilesLectores;
            }
        }

        /// <summary>
        /// Obtiene la lista de editores de este documento omitiendo los selectore.
        /// </summary>
        public Dictionary<Guid, EditorRecurso> ListaPerfilesEditoresSinLectores
        {
            get
            {
                if (mListaPerfilesEditoresSinLectores == null)
                {
                    mListaPerfilesEditoresSinLectores = new Dictionary<Guid, EditorRecurso>();
                    //Select("DocumentoID = '" + this.Clave + "' AND Editor=true")
                    foreach (AD.EntityModel.Models.Documentacion.DocumentoRolIdentidad filaEditor in GestorDocumental.DataWrapperDocumentacion.ListaDocumentoRolIdentidad.Where(doc => doc.DocumentoID.Equals(this.Clave) && doc.Editor))
                    {
                        mListaPerfilesEditoresSinLectores.Add(filaEditor.PerfilID, new EditorRecurso(filaEditor, this.GestorDocumental, mLoggingService));
                    }
                }
                return mListaPerfilesEditoresSinLectores;
            }
        }

        /// <summary>
        /// Obtiene la lista de grupos editores de este documento
        /// </summary>
        public Dictionary<Guid, GrupoEditorRecurso> ListaGruposEditores
        {
            get
            {
                if (mListaGruposEditores == null)
                {
                    mListaGruposEditores = new Dictionary<Guid, GrupoEditorRecurso>();

                    foreach (AD.EntityModel.Models.Documentacion.DocumentoRolGrupoIdentidades filaGrupoEditor in GestorDocumental.DataWrapperDocumentacion.ListaDocumentoRolGrupoIdentidades.Where(doc => doc.DocumentoID.Equals(this.Clave)).ToList())
                    {
                        mListaGruposEditores.Add(filaGrupoEditor.GrupoID, new GrupoEditorRecurso(filaGrupoEditor, this.GestorDocumental, mLoggingService));
                    }
                }
                return mListaGruposEditores;
            }

            set
            {
                mListaGruposEditores = value;
            }
        }

        /// <summary>
        /// Obtiene la lista de grupos lectores de este documento
        /// </summary>
        public Dictionary<Guid, GrupoLectorRecurso> ListaGruposLectores
        {
            get
            {
                if (mListaGruposLectores == null)
                {//"DocumentoID = '" + this.Clave + "' and Editor=false"
                    mListaGruposLectores = new Dictionary<Guid, GrupoLectorRecurso>();

                    foreach (AD.EntityModel.Models.Documentacion.DocumentoRolGrupoIdentidades filaGrupoLector in GestorDocumental.DataWrapperDocumentacion.ListaDocumentoRolGrupoIdentidades.Where(doc => doc.DocumentoID.Equals(this.Clave) && doc.Editor == false))
                    {
                        mListaGruposLectores.Add(filaGrupoLector.GrupoID, new GrupoLectorRecurso(filaGrupoLector, this.GestorDocumental, mLoggingService));
                    }
                }
                return mListaGruposLectores;
            }

            set
            {
                mListaGruposLectores = value;
            }
        }

        /// <summary>
        /// Obtiene la lista de grupos editores de este documento omitiendo los selectore.
        /// </summary>
        public Dictionary<Guid, GrupoEditorRecurso> ListaGruposEditoresSinLectores
        {
            get
            {
                if (mListaGruposEditoresSinLectores == null)
                {
                    mListaGruposEditoresSinLectores = new Dictionary<Guid, GrupoEditorRecurso>();

                    foreach (AD.EntityModel.Models.Documentacion.DocumentoRolGrupoIdentidades filaGrupoEditor in GestorDocumental.DataWrapperDocumentacion.ListaDocumentoRolGrupoIdentidades.Where(doc => doc.DocumentoID.Equals(this.Clave) && doc.Editor))
                    {
                        mListaGruposEditoresSinLectores.Add(filaGrupoEditor.GrupoID, new GrupoEditorRecurso(filaGrupoEditor, this.GestorDocumental, mLoggingService));
                    }
                }
                return mListaGruposEditoresSinLectores;
            }
        }

        /// <summary>
        /// Obtiene la lista de respuestas de este documento
        /// </summary>
        public Dictionary<Guid, RespuestaRecurso> ListaRespuestas
        {
            get
            {
                if (mListaRespuestas == null)
                {
                    mListaRespuestas = new Dictionary<Guid, RespuestaRecurso>();
                    if (GestorDocumental.DataWrapperDocumentacion != null)
                    {
                        foreach (AD.EntityModel.Models.Documentacion.DocumentoRespuesta filaRespuesta in GestorDocumental.DataWrapperDocumentacion.ListaDocumentoRespuesta.Where(item => item.DocumentoID.Equals(this.Clave)).OrderBy(item => item.Orden))
                        {
                            mListaRespuestas.Add(filaRespuesta.RespuestaID, new RespuestaRecurso(filaRespuesta, this.GestorDocumental, mLoggingService));
                        }
                    }

                }
                return mListaRespuestas;
            }
        }

        /// <summary>
        /// TRUE si el documento permite tener versiones
        /// </summary>
        public bool PermiteVersiones
        {
            get
            {
                return TipoDocumentacion != TiposDocumentacion.Ontologia && !EsBorrador && !EsVideoIncrustado;
            }
        }

        /// <summary>
        /// TRUE si el documento es un video incrustado (youtube o vimeo)
        /// </summary>
        public bool EsVideoIncrustado
        {
            get
            {
                if (TipoDocumentacion == TiposDocumentacion.VideoBrightcove || TipoDocumentacion == TiposDocumentacion.VideoTOP)
                {
                    return true;
                }
                else if (!string.IsNullOrEmpty(Enlace))
                {
                    return CompruebaUrlEsVideo(Enlace);
                }
                return false;
            }
        }

        /// <summary>
        /// TRUE si el documento es un audio incrustado (audio brightcove)
        /// </summary>
        public bool EsAudioIncrustado
        {
            get
            {
                if (TipoDocumentacion == TiposDocumentacion.AudioBrightcove || TipoDocumentacion == TiposDocumentacion.AudioTOP)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// TRUE si el documento es una presentacion incrustada (Slideshare)
        /// </summary>
        public bool EsPresentacionIncrustada
        {
            get
            {
                return CompruebaUrlEsPresentacion(Enlace);
            }
        }

        /// <summary>
        /// TRUE si el documento es un archivo digital (fichero subido al servidor)
        /// </summary>
        public bool EsFicheroDigital
        {
            get
            {
                return ((TipoDocumentacion == TiposDocumentacion.FicheroServidor || TipoDocumentacion == TiposDocumentacion.Video || TipoDocumentacion == TiposDocumentacion.Imagen) && !EsVideoIncrustado && !EsPresentacionIncrustada);
            }
        }

        /// <summary>
        /// TRUE si el documento permite comentarios, FALSE en caso contrario
        /// </summary>
        public bool PermiteComentarios
        {
            get
            {
                if (FilaDocumentoWebVinBR != null)
                {
                    return FilaDocumentoWebVinBR.PermiteComentarios;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Fila de la vinculación del documento con la BR actual del gestor documental.
        /// </summary>
        public AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos FilaDocumentoWebVinBR
        {
            get
            {
                if (!GestorDocumental.HayBaseRecursosCargada)
                {
                    return null;
                }
                //("DocumentoID = '" + this.Clave + "'" + " AND BaseRecursosID='" + GestorDocumental.BaseRecursosIDActual + "'")
                List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filas = GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(this.Clave) && doc.BaseRecursosID.Equals(GestorDocumental.BaseRecursosIDActual)).ToList();
                if (filas.Count > 0)
                {
                    return filas.First();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Fila de la vinculación del documento con la BR actual del gestor documental.
        /// </summary>
        public AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursosExtra FilaDocumentoWebVinBRExtra
        {
            get
            {
                if (!GestorDocumental.HayBaseRecursosCargada)
                {
                    return null;
                }
                //FindByDocumentoIDBaseRecursosID(this.Clave, GestorDocumental.BaseRecursosIDActual);
                AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursosExtra fila = GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursosExtra.FirstOrDefault(doc => doc.DocumentoID.Equals(this.Clave) && doc.BaseRecursosID.Equals(GestorDocumental.BaseRecursosIDActual));

                return fila;
            }
        }

        /// <summary>
        /// Fila de la vinculación del documento con la Identidad del publicadorID (CreadorID)
        /// </summary>
        public AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos FilaDocumentoWebVinBR_Publicador
        {
            get
            {
                if (!GestorDocumental.HayBaseRecursosCargada)
                {
                    return null;
                }
                //"DocumentoID = '" + this.Clave + "' AND TipoPublicacion = 0"
                List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filas = GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(this.Clave) && doc.TipoPublicacion == 0).ToList();
                if (filas.Count > 0)
                {
                    return filas[0];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Fila de la vinculación del documento que no ha sido compartido todavía.
        /// </summary>
        public AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos FilaDocumentoWebVinBR_SinCompartir
        {
            get
            {
                if (!GestorDocumental.HayBaseRecursosCargada)
                {
                    return null;
                }

                List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filas = GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(this.Clave) && doc.TipoPublicacion.Equals((short)TipoPublicacion.Publicado)).ToList();
                if (filas.Count > 0)
                {
                    return filas.First();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Lista con los proyectos a lo que pertenece el documento más su tipo de acceso.
        /// </summary>
        public List<Guid> ListaProyectos
        {
            get
            {
                if (mListaProyectos == null)
                {
                    mListaProyectos = new List<Guid>();
                    //"DocumentoID='" + Clave + "' AND Eliminado=0"
                    foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaDocWeb in GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(Clave) && !doc.Eliminado))
                    {
                        List<AD.EntityModel.Models.Documentacion.BaseRecursosProyecto> filasBRProy = GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosProyecto.Where(doc => doc.BaseRecursosID.Equals(filaDocWeb.BaseRecursosID)).ToList();
                        if (filasBRProy.Count > 0)
                        {
                            mListaProyectos.Add(filasBRProy.First().ProyectoID);
                        }
                        else if (!mListaProyectos.Contains(ProyectoAD.MetaProyecto))
                        {
                            mListaProyectos.Add(ProyectoAD.MetaProyecto);
                        }
                    }
                }

                return mListaProyectos;
            }
        }

        /// <summary>
        /// Devuelve el documento que es la versión anterior al actual o NULL en caso contrario
        /// </summary>
        public Documento VersionAnterior
        {
            get
            {
                if (Version > 0)
                {
                    AD.EntityModel.Models.Documentacion.VersionDocumento filaVersion = FilaDocumento.VersionDocumento;
                    //"DocumentoOriginalID = '" + filaVersion.DocumentoOriginalID + "' AND Version = " + (filaVersion.Version - 1)
                    List<AD.EntityModel.Models.Documentacion.VersionDocumento> filasVersionAnterior = GestorDocumental.DataWrapperDocumentacion.ListaVersionDocumento.Where(doc => doc.DocumentoOriginalID.Equals(filaVersion.DocumentoOriginalID) && doc.Version.Equals(filaVersion.Version - 1)).ToList();

                    if (filasVersionAnterior.Count == 0)
                    {
                        if (GestorDocumental.ListaDocumentos.ContainsKey(filaVersion.DocumentoOriginalID))
                        {
                            // La versión anterior es el documento original
                            return GestorDocumental.ListaDocumentos[filaVersion.DocumentoOriginalID];
                        }
                    }
                    else if (GestorDocumental.ListaDocumentos.ContainsKey(filasVersionAnterior.First().DocumentoID))
                    {
                        return GestorDocumental.ListaDocumentos[filasVersionAnterior.First().DocumentoID];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Devuelve el identificador de la versión original del documento o la Clave actual si es el presente.
        /// </summary>
        public Guid VersionOriginalID
        {
            get
            {
                List<AD.EntityModel.Models.Documentacion.VersionDocumento> filaVerDoc = GestorDocumental.DataWrapperDocumentacion.ListaVersionDocumento.Where(doc => doc.DocumentoID.Equals(Clave)).ToList();

                if (filaVerDoc.Count > 0)
                {
                    return filaVerDoc.First().DocumentoOriginalID;
                }
                else
                {
                    return Clave;
                }
            }
        }

        /// <summary>
        /// Lista de categorias de tesauro a las que está agregado el documento
        /// </summary>
        public SortedList<Guid, CategoriaTesauro> CategoriasTesauro
        {
            get
            {
                SortedList<Guid, CategoriaTesauro> listaCategorias = new SortedList<Guid, CategoriaTesauro>();
                foreach (AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro filaCategoria in GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(doc => doc.DocumentoID.Equals(Clave)))
                {
                    if (GestorDocumental.ListaCategoriasTesauro.ContainsKey(filaCategoria.CategoriaTesauroID) && !listaCategorias.ContainsKey(filaCategoria.CategoriaTesauroID))
                    {
                        listaCategorias.Add(filaCategoria.CategoriaTesauroID, GestorDocumental.ListaCategoriasTesauro[filaCategoria.CategoriaTesauroID]);
                    }
                }
                return listaCategorias;
            }
        }

        /// <summary>
        /// Lista de categorias de tesauro a las que está agregado el documento ordenadas por nombre en funcion del idioma.
        /// </summary>
        public Dictionary<string, List<CategoriaTesauro>> CategoriasTesauroOrdenadas
        {
            get
            {
                Dictionary<string, List<CategoriaTesauro>> listaCategoriasTesauroOrdenadas = new Dictionary<string, List<CategoriaTesauro>>();

                foreach (IdiomasCategorias idioma in Enum.GetValues(typeof(IdiomasCategorias)))
                {
                    List<CategoriaTesauro> listaCategorias = new List<CategoriaTesauro>();

                    SortedDictionary<string, List<CategoriaTesauro>> listaCategoriasOrdenadas = new SortedDictionary<string, List<CategoriaTesauro>>();


                    foreach (CategoriaTesauro categoria in CategoriasTesauro.Values)
                    {
                        if (listaCategoriasOrdenadas.ContainsKey(categoria.Nombre[idioma.ToString()]))
                        {
                            listaCategoriasOrdenadas[categoria.Nombre[idioma.ToString()]].Add(categoria);
                        }
                        else
                        {
                            List<CategoriaTesauro> categorias = new List<CategoriaTesauro>();
                            categorias.Add(categoria);

                            listaCategoriasOrdenadas.Add(categoria.Nombre[idioma.ToString()], categorias);
                        }
                    }

                    foreach (string nombreCategoria in listaCategoriasOrdenadas.Keys)
                    {
                        foreach (CategoriaTesauro categoria in listaCategoriasOrdenadas[nombreCategoria])
                        {
                            listaCategorias.Add(categoria);
                        }
                    }

                    listaCategoriasTesauroOrdenadas.Add(idioma.ToString(), listaCategorias);
                }

                return listaCategoriasTesauroOrdenadas;
            }
        }

        /// <summary>
        /// Lista de categorias de tesauro públicas a las que está agregado el documento
        /// </summary>
        public SortedList<Guid, CategoriaTesauro> CategoriasTesauroPublicas
        {
            get
            {
                if (mCategoriasTesauroPublicas == null)
                {
                    if (GestorDocumental.GestorTesauro.TesauroDeUsuario)
                    {
                        SortedList<Guid, CategoriaTesauro> listaCategorias = new SortedList<Guid, CategoriaTesauro>();

                        foreach (AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro filaCategoria in GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(doc => doc.DocumentoID.Equals(Clave)))
                        {
                            if (GestorDocumental.ListaCategoriasTesauro.ContainsKey(filaCategoria.CategoriaTesauroID) && !listaCategorias.ContainsKey(filaCategoria.CategoriaTesauroID) && GestorDocumental.ListaCategoriasTesauro[filaCategoria.CategoriaTesauroID].PadreNivelRaiz.Clave == GestorDocumental.GestorTesauro.CategoriaPublicaID)
                            {
                                listaCategorias.Add(filaCategoria.CategoriaTesauroID, GestorDocumental.ListaCategoriasTesauro[filaCategoria.CategoriaTesauroID]);
                            }
                        }
                        mCategoriasTesauroPublicas = listaCategorias;
                    }
                    else
                    {
                        mCategoriasTesauroPublicas = CategoriasTesauro;
                    }
                }
                return mCategoriasTesauroPublicas;
            }
        }

        /// <summary>
        /// Fila del documento
        /// </summary>
        public AD.EntityModel.Models.Documentacion.Documento FilaDocumento
        {
            get
            {
                return (AD.EntityModel.Models.Documentacion.Documento)FilaElementoEntity;
            }
        }

        /// <summary>
        /// Devuelve o establece el gestor documental del documento
        /// </summary>
        public GestorDocumental GestorDocumental
        {
            get
            {
                return (GestorDocumental)this.GestorGnoss;
            }
        }

        /// <summary>
        /// Ficha bibliográfica
        /// </summary>
        public FichaBibliografica FichaBibliografica
        {
            get
            {
                if (mFichaBibliografica == null)
                {
                    mFichaBibliografica = ObtenerFichaBibliografica();
                }
                return mFichaBibliografica;
            }
            set
            {
                mFichaBibliografica = value;
            }
        }

        /// <summary>
        /// Devuelve la ficha bibliográfica del documento.
        /// </summary>
        /// <returns>Ficha bibliográfica del doc si la tiene, null si no</returns>
        public FichaBibliografica ObtenerFichaBibliografica()
        {
            if (mFichaBibliografica == null)
            {
                if (GestorDocumental.DataWrapperDocumentacion != null)
                {
                    FichaBibliografica fichaBiblio = null;
                    List<AD.EntityModel.Models.Documentacion.DocumentoAtributoBiblio> filasDocumentoAtributo = GestorDocumental.DataWrapperDocumentacion.ListaDocumentoAtributoBiblio.Where(item => item.DocumentoID.Equals(Clave)).ToList();

                    if (filasDocumentoAtributo.Count > 0)
                    {
                        //Tiene ficha bibliografica:
                        fichaBiblio = new FichaBibliografica(filasDocumentoAtributo[0].FichaBibliograficaID, mLoggingService);

                        foreach (AD.EntityModel.Models.Documentacion.DocumentoAtributoBiblio filaDocAtributo in filasDocumentoAtributo)
                        {
                            AD.EntityModel.Models.Documentacion.AtributoFichaBibliografica filaAtributoBiblio = GestorDocumental.DataWrapperDocumentacion.ListaAtributoFichaBibliografica.Where(item => item.AtributoID.Equals(filaDocAtributo.AtributoID)).FirstOrDefault();
                            //string valorAtributo = null;
                            //if ((TipoAtributosCampos)filaAtributoBiblio.Tipo == TipoAtributosCampos.Titulo && filaDocAtributo.IsNull("Valor"))
                            //{
                            //    valorAtributo = Titulo;
                            //}
                            fichaBiblio.Atributos.Add(filaAtributoBiblio.Orden, new AtributoBibliografico(filaAtributoBiblio.FichaBibliograficaID, filaAtributoBiblio.AtributoID, filaAtributoBiblio.Nombre, filaAtributoBiblio.Descripcion, filaAtributoBiblio.Tipo, filaAtributoBiblio.Orden, filaDocAtributo.Valor, mLoggingService));
                        }
                    }
                    mFichaBibliografica = fichaBiblio;
                }

            }
            return mFichaBibliografica;
        }

        /// <summary>
        /// Título del recurso
        /// </summary>
        public string Titulo
        {
            get
            {
                // Juan: Comento esto, no tiene ningún sentido que esté aquí. 
                //return UtilCadenas.SepararPalabrasDemasiadoLargas(FilaDocumento.Titulo, 30);
                return FilaDocumento.Titulo;
            }
            set
            {
                if (value != FilaDocumento.Titulo)
                {
                    FilaDocumento.Titulo = value;
                    Notificar(new Es.Riam.Interfaces.Observador.MensajeObservador(Es.Riam.Interfaces.Observador.AccionesObservador.Invalidar, this));
                }
            }
        }

        /// <summary>
        /// Nombre del documento
        /// </summary>
        public override string Nombre
        {
            get
            {
                return Titulo;
            }
            set
            {
                Titulo = value;
            }
        }

        public static string ObtenerNombreSemantico(string pNombre, string pIdioma)
        {
            pNombre = UtilCadenas.EliminarCaracteresUrlSem(UtilCadenas.ObtenerTextoDeIdioma(pNombre, pIdioma, null));

            //Elimino los puntos del final:
            while (pNombre.Length > 0 && pNombre[pNombre.Length - 1] == '.')
            {
                pNombre = pNombre.Remove(pNombre.Length - 1);
            }

            if (string.IsNullOrEmpty(pNombre))
            {
                pNombre = "no-name";
            }

            return pNombre;
        }

        /// <summary>
        /// Obtiene el nombre semantico del documento
        /// </summary>
        public string NombreSem(string pIdioma)
        {
            return ObtenerNombreSemantico(Titulo, pIdioma);
        }

        /// <summary>
        /// Nombre del documento
        /// </summary>
        public string NombreDocumento
        {
            get
            {
                string nombreDoc = Enlace;
                try
                {
                    nombreDoc = Path.GetFileName(Enlace);
                }
                catch (Exception)
                {
                    //Error.GuardarLogError(e); Es una cansa la Web venga a escribir esto y total no sirve para nada -> Fuera de aquí
                }
                return nombreDoc;
            }
        }

        /// <summary>
        /// Devuelve o establece la fecha en la que se subió el fichero
        /// </summary>
        public override DateTime Fecha
        {
            get
            {
                return FilaDocumento.FechaCreacion.Value;

            }
            set
            {
                if (value != FilaDocumento.FechaCreacion)
                {
                    FilaDocumento.FechaCreacion = value;
                    Notificar(new Es.Riam.Interfaces.Observador.MensajeObservador(Es.Riam.Interfaces.Observador.AccionesObservador.Invalidar, this));
                }
            }
        }

        /// <summary>
        /// Devuelve o establece la descripción del documento
        /// </summary>
        public string Descripcion
        {
            get
            {
                return FilaDocumento.Descripcion;
            }
            set
            {
                if (value != FilaDocumento.Descripcion)
                {
                    FilaDocumento.Descripcion = value;
                    Notificar(new Es.Riam.Interfaces.Observador.MensajeObservador(Es.Riam.Interfaces.Observador.AccionesObservador.Invalidar, this));
                }
            }
        }

        /// <summary>
        /// Clave
        /// </summary>
        public override Guid Clave
        {
            get
            {
                return FilaDocumento.DocumentoID;
            }
        }

        /// <summary>
        /// Tipo de documentación
        /// </summary>
        public TiposDocumentacion TipoDocumentacion
        {
            get
            {
                return (TiposDocumentacion)((short)FilaDocumento.Tipo);
            }
        }

        /// <summary>
        /// Enlace del documento
        /// </summary>
        public string Enlace
        {
            get
            {
                return FilaDocumento.Enlace;
            }
            set
            {
                if (value != FilaDocumento.Enlace)
                {
                    FilaDocumento.Enlace = value;
                    Notificar(new Es.Riam.Interfaces.Observador.MensajeObservador(Es.Riam.Interfaces.Observador.AccionesObservador.Invalidar, this));
                }
            }
        }

        /// <summary>
        /// Devuelve o establece la clave de la organización del documento.
        /// </summary>
        public Guid OrganizacionID
        {
            get
            {
                return FilaDocumento.OrganizacionID;
            }
            set
            {
                if (value != FilaDocumento.OrganizacionID)
                {
                    FilaDocumento.OrganizacionID = value;
                    Notificar(new Es.Riam.Interfaces.Observador.MensajeObservador(Es.Riam.Interfaces.Observador.AccionesObservador.Invalidar, this));
                }
            }
        }

        /// <summary>
        /// Devuelve o establece la clave de la Identidad que sube el fichero
        /// </summary>
        public Guid CreadorID
        {
            get
            {
                if (FilaDocumento.CreadorID.HasValue)
                {
                    return FilaDocumento.CreadorID.Value;
                }
                else
                {
                    return Guid.Empty;
                }

            }
            set
            {
                if (value != FilaDocumento.CreadorID)
                {
                    FilaDocumento.CreadorID = value;
                    Notificar(new Es.Riam.Interfaces.Observador.MensajeObservador(Es.Riam.Interfaces.Observador.AccionesObservador.Invalidar, this));
                }
            }
        }

        /// <summary>
        /// Devuelve una lista ordenada de documentos que son las versiones anteriores del actual
        /// </summary>
        public List<Documento> ListaDocumentosVersionesAnteriores
        {
            get
            {
                SortedList<short, Documento> listaOrdenada = new SortedList<short, Documento>();
                List<Documento> resultado = new List<Documento>();

                // David: Obtener la version actual del documento (si es NULL, el documento es el original)
                AD.EntityModel.Models.Documentacion.VersionDocumento filaVersionActual = GestorDocumental.DataWrapperDocumentacion.ListaVersionDocumento.FirstOrDefault(doc => doc.DocumentoID.Equals(Clave));

                if (filaVersionActual != null)
                {
                    foreach (AD.EntityModel.Models.Documentacion.VersionDocumento filaVersion in GestorDocumental.DataWrapperDocumentacion.ListaVersionDocumento.Where(version => version.DocumentoOriginalID.Equals(filaVersionActual.DocumentoOriginalID) && !version.DocumentoID.Equals(Clave) && version.Version < filaVersionActual.Version))
                    {
                        if (GestorDocumental.ListaDocumentos.ContainsKey(filaVersion.DocumentoID) && !listaOrdenada.ContainsKey((short)filaVersion.Version))
                        {
                            listaOrdenada.Add((short)filaVersion.Version, GestorDocumental.ListaDocumentos[filaVersion.DocumentoID]);
                        }
                    }

                    // Añadir el documento original
                    if (GestorDocumental.ListaDocumentos.ContainsKey(filaVersionActual.DocumentoOriginalID))
                    {
                        listaOrdenada.Add(0, GestorDocumental.ListaDocumentos[filaVersionActual.DocumentoOriginalID]);
                    }
                }

                for (int i = listaOrdenada.Count - 1; i >= 0; i--)
                {
                    if (!listaOrdenada.Values[i].FilaDocumento.Eliminado)
                    {
                        resultado.Add(listaOrdenada.Values[i]);
                    }
                }
                return resultado;
            }
        }

        /// <summary>
        /// Devuelve el Nº de version del documento (0 si es la original)
        /// </summary>
        public short Version
        {
            get
            {
                if (FilaDocumento.VersionDocumento != null)
                {
                    return (short)FilaDocumento.VersionDocumento.Version;
                }
                return 0;
            }
        }

        /// <summary>
        /// Devuelve o establece el nombre de la entidad vinculada a un documento.
        /// </summary>
        public string NombreEntidadVinculada
        {
            get
            {
                return FilaDocumento.NombreElementoVinculado;
            }
            set
            {
                FilaDocumento.NombreElementoVinculado = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el ID del elemento vinculado a un documento.
        /// </summary>
        public Guid ElementoVinculadoID
        {
            get
            {
                if (!FilaDocumento.ElementoVinculadoID.HasValue)
                {
                    return Guid.Empty;
                }
                return FilaDocumento.ElementoVinculadoID.Value;
            }
            set
            {
                FilaDocumento.ElementoVinculadoID = value;
            }
        }

        /// <summary>
        /// Devuelve el ID del Proyecto al que pertence el documento.
        /// </summary>
        public Guid ProyectoID
        {
            get
            {
                if (!FilaDocumento.ProyectoID.HasValue)
                {
                    return Guid.Empty;
                }
                return FilaDocumento.ProyectoID.Value;
            }
        }

        /// <summary>
        /// Devuelve o establece si el documento se está subiendo al servidor o no
        /// </summary>
        public EstadoFichero EstadoFichero
        {
            get
            {
                return mEstadoFichero;
            }
            set
            {
                mEstadoFichero = value;
            }
        }

        /// <summary>
        /// Devuelve el tipo de entidad vinculada al documento.
        /// </summary>
        public TipoEntidadVinculadaDocumento TipoEntidadVinculada
        {
            get
            {
                return (TipoEntidadVinculadaDocumento)FilaDocumento.TipoEntidad;
            }
        }

        /// <summary>
        /// Devuelve o estable de autor del documento.
        /// </summary>
        public string Autor
        {
            get
            {
                if (FilaDocumento.Autor == null)
                {
                    return null;
                }
                else
                {
                    return FilaDocumento.Autor;
                }
            }
            set
            {
                FilaDocumento.Autor = value;
            }
        }

        /// <summary>
        /// Establece o devuelve si el enlace del doc ha sido modificado.
        /// </summary>
        public bool EnlaceCambiado
        {
            get
            {
                return mEnlaceDocumentoCambiado;
            }
            set
            {
                mEnlaceDocumentoCambiado = value;
            }
        }

        /// <summary>
        /// Devuelve si el documento puede compartirse o no.
        /// </summary>
        public bool CompartirPermitido
        {
            get
            {
                return FilaDocumento.CompartirPermitido;
            }
        }

        /// <summary>
        /// Devuelve si el documento es borrador o no.
        /// </summary>
        public bool EsBorrador
        {
            get
            {
                return FilaDocumento.Borrador;
            }
        }

        /// <summary>
        /// Devuelve la extensión del enlace del documento
        /// </summary>
        public string Extension
        {
            get
            {
                try
                {
                    if (EsPresentacionIncrustada)
                    {
                        return ".ppt";
                    }
                    if (Enlace != null && Enlace != "")
                    {
                        return Path.GetExtension(Enlace).ToLower();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception)
                {
                    //Nombre contiene caracteres no válidos, lo hacemos a la antigua:
                    int indiceUltimoPunto = Enlace.LastIndexOf(".");

                    if (indiceUltimoPunto != -1)
                    {
                        return Enlace.Substring(indiceUltimoPunto);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// TRUE si el usuario actual es el creador del documento
        /// </summary>
        public bool EsAutor(Guid pIdentidadID)
        {
            return pIdentidadID.Equals(FilaDocumento.CreadorID);
        }

        /// <summary>
        /// Devuelve la última versión activa de la lista de versiones de un documento
        /// </summary>
        public Documento UltimaVersion
        {
            get
            {
                if (FilaDocumento.UltimaVersion == false)
                {
                    if (mUltimaVersion == null)
                    {
                        Guid docOriginalID;

                        if (this.FilaDocumento.VersionDocumento == null)
                        {
                            docOriginalID = this.Clave;
                        }
                        else
                        {
                            docOriginalID = this.FilaDocumento.VersionDocumento.DocumentoOriginalID;
                        }
                        var fila = GestorDocumental.DataWrapperDocumentacion.ListaVersionDocumento.FirstOrDefault(version => version.DocumentoOriginalID.Equals(docOriginalID) && version.Documento != null && version.Documento.UltimaVersion);

                        if (fila != null)
                        {
                            mUltimaVersion = new Documento(fila.Documento, this.GestorDocumental, mLoggingService);
                        }
                    }
                }
                return mUltimaVersion;
            }
        }

        /// <summary>
        /// Devuelve el ID del la última versión cargada del documento.
        /// </summary>
        public Guid UltimaVersionID
        {
            get
            {
                if (FilaDocumento.UltimaVersion == false)
                {
                    Guid docUltimoID = Guid.Empty;

                    Guid docOriginalID = Guid.Empty;

                    if (this.FilaDocumento.VersionDocumento == null)
                    {
                        docOriginalID = this.Clave;
                    }
                    else
                    {
                        docOriginalID = this.FilaDocumento.VersionDocumento.DocumentoOriginalID;
                    }
                    //"DocumentoOriginalID = '" + docOriginalID + "'", "Version desc"
                    List<AD.EntityModel.Models.Documentacion.VersionDocumento> filasVersion = GestorDocumental.DataWrapperDocumentacion.ListaVersionDocumento.Where(version => version.DocumentoOriginalID.Equals(docOriginalID)).OrderByDescending(version => version.Version).ToList();

                    if (filasVersion != null && filasVersion.Count > 0)
                    {
                        docUltimoID = filasVersion[0].DocumentoID;
                    }

                    return docUltimoID;
                }
                else
                {
                    return Clave;
                }
            }
        }

        /// <summary>
        /// Devuelve la fila DocumentoWebVinBaseRecursosRow o null si no está compartido.
        /// </summary>
        public AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos IdentidadCompardido
        {
            get
            {
                List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filasDoc = GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(Clave) && doc.BaseRecursosID.Equals(GestorDocumental.BaseRecursosIDActual)).ToList();

                if (filasDoc.Count > 0)
                {
                    return filasDoc[0];
                }
                return null;
            }
        }

        /// <summary>
        /// Devuelve un diccionario  con clave BaseRecursos y valor la fila DocumentoWebVinBaseRecursosRow o null si no está compartido.
        /// </summary>
        public Dictionary<Guid, AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> IdentidadCompardidoPorBaseRecursos
        {
            get
            {
                Dictionary<Guid, AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> identidadCompardidoPorBaseRecursos = new Dictionary<Guid, AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos>();
                List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filasDoc = GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(Clave)).ToList();

                foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaDoc in filasDoc)
                {
                    identidadCompardidoPorBaseRecursos.Add(filaDoc.BaseRecursosID, filaDoc);
                }
                return identidadCompardidoPorBaseRecursos;
            }
        }



        /// <summary>
        /// Devuelve si el documento permite tener licencia o no.
        /// </summary>
        public bool PermiteLicencia
        {
            get
            {
                return (TipoDocumentacion == TiposDocumentacion.Nota || TipoDocumentacion == TiposDocumentacion.FicheroServidor || TipoDocumentacion == TiposDocumentacion.Semantico || TipoDocumentacion == TiposDocumentacion.Newsletter || TipoDocumentacion == TiposDocumentacion.VideoBrightcove || TipoDocumentacion == TiposDocumentacion.Imagen || (TipoDocumentacion == TiposDocumentacion.Video && !EsVideoIncrustado));
            }
        }

        /// <summary>
        /// Obtiene si el documento semántico tiene imagen propia o un icono Gnoss.
        /// </summary>
        /// <param name="pUrlImagen">Url de la imagen del documento</param>
        /// <returns>TRUE si el documento semántico tiene imagen propia, FALSE si tiene un icono Gnoss</returns>
        public bool TieneImagenSemantica
        {
            get
            {
                return (TipoDocumentacion == TiposDocumentacion.Semantico || !string.IsNullOrEmpty(FilaDocumento.NombreCategoriaDoc));
            }
        }

        #region Voto documento y Comentarios

        /// <summary>
        /// Devuelve la lista de todos los votos del documento.
        /// </summary>
        public Dictionary<Guid, AD.EntityModel.Models.Voto.Voto> ListaVotos
        {
            get
            {
                if (mListaVotos == null)
                {
                    mListaVotos = new Dictionary<Guid, AD.EntityModel.Models.Voto.Voto>();
                    if (GestorDocumental.DataWrapperDocumentacion != null)
                    {
                        List<AD.EntityModel.Models.Documentacion.VotoDocumento> filasVotosDoc = GestorDocumental.DataWrapperDocumentacion.ListaVotoDocumento.Where(item => item.DocumentoID.Equals(this.Clave)).ToList();

                        foreach (AD.EntityModel.Models.Documentacion.VotoDocumento filaVotodoc in filasVotosDoc)
                        {
                            if (GestorDocumental.GestorVotos.ListaVotos.ContainsKey(filaVotodoc.VotoID))
                            {
                                AD.EntityModel.Models.Voto.Voto voto = GestorDocumental.GestorVotos.ListaVotos[filaVotodoc.VotoID];
                                mListaVotos.Add(voto.VotoID, voto);
                            }
                        }
                    }

                }
                return mListaVotos;
            }
            set
            {
                mListaVotos = value;
            }
        }

        /// <summary>
        /// Lista de comentarios de un documento.
        /// </summary>
        public List<Comentario.Comentario> Comentarios
        {
            get
            {
                if (mListaComentarios == null)
                {
                    RecargarComentariosDocumentos();
                }
                return mListaComentarios;
            }
            set
            {
                mListaComentarios = value;
            }
        }

        #endregion

        #region Nombres y tipos identidad

        /// <summary>
        /// Devuelve el nombre del creador o publicador del documento.
        /// </summary>
        //public string NombreCreador
        //{
        //    get
        //    {
        //        if ((FilaDocumento.Table.Columns.Contains("TipoPerfil")) && (!FilaDocumento.IsNull("TipoPerfil")))
        //        {
        //            if ((short)FilaDocumento["TipoPerfil"] == (short)Es.Riam.Gnoss.AD.Identidad.TiposIdentidad.Personal || (short)FilaDocumento["TipoPerfil"] == (short)Es.Riam.Gnoss.AD.Identidad.TiposIdentidad.Profesor)
        //            {
        //                return (string)FilaDocumento["NombreCreador"];
        //            }
        //            else if ((short)FilaDocumento["TipoPerfil"] == (short)Es.Riam.Gnoss.AD.Identidad.TiposIdentidad.ProfesionalPersonal)
        //            {
        //                return (string)FilaDocumento["NombreCreador"] + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + (string)FilaDocumento["NombreOrganizacion"];
        //            }
        //            else if ((short)FilaDocumento["TipoPerfil"] == (short)Es.Riam.Gnoss.AD.Identidad.TiposIdentidad.ProfesionalCorporativo)
        //            {
        //                return (string)FilaDocumento["NombreOrganizacion"];
        //            }
        //            else
        //            {
        //                //Es de organización
        //                return (string)FilaDocumento["NombreOrganizacion"];
        //            }
        //        }
        //        else
        //        {
        //            if (CreadorID == Usuario.UsuarioActual.IdentidadID)
        //            {
        //                return NombreUsuarioActual;
        //            }
        //            else
        //            {
        //                return null;
        //            }
        //        }
        //    }
        //}


        /// <summary>
        /// Identificador de la organización del creador del documento si esta en modo profesional o organización.
        /// </summary>
        public Guid OrganizacionPerfilCreador
        {
            get
            {
                try
                {
                    if (GestorDocumental != null && FilaDocumento.CreadorID.HasValue && GestorDocumental.GestorIdentidades != null && GestorDocumental.GestorIdentidades.ListaIdentidades != null && FilaDocumento != null && GestorDocumental.GestorIdentidades.ListaIdentidades.ContainsKey(FilaDocumento.CreadorID.Value) && GestorDocumental.GestorIdentidades.ListaIdentidades[FilaDocumento.CreadorID.Value].OrganizacionID.HasValue)
                    {
                        return GestorDocumental.GestorIdentidades.ListaIdentidades[FilaDocumento.CreadorID.Value].OrganizacionID.Value;
                    }
                    else
                    {
                        return Guid.Empty;
                    }
                }
                catch (Exception)
                {
                    return Guid.Empty;
                }
            }
        }

        #endregion

        #region Vinculación con documentos

        /// <summary>
        /// Lista con los IDs de los documentos vinculados al actual.
        /// </summary>
        public List<Guid> DocumentosVinculados
        {
            get
            {
                if (mDocumentosVinculados == null)
                {
                    mDocumentosVinculados = new List<Guid>();
                    if (GestorDocumental.DataWrapperDocumentacion != null)
                    {
                        foreach (AD.EntityModel.Models.Documentacion.DocumentoVincDoc filaDocVinc in GestorDocumental.DataWrapperDocumentacion.ListaDocumentoVincDoc.Where(item => item.DocumentoID.Equals(Clave)).OrderByDescending(item => item.Fecha))
                        {
                            mDocumentosVinculados.Add(filaDocVinc.DocumentoVincID);
                        }
                    }

                }

                return mDocumentosVinculados;
            }
        }

        #endregion

        #region Categorias

        /// <summary>
        /// Obtiene la lista de categorías
        /// </summary>
        public SortedList<Guid, CategoriaTesauro> Categorias
        {
            get
            {
                if (mListaCategorias == null)
                {
                    RecargarCategoriasTesauro();
                }
                return mListaCategorias;
            }
        }

        #endregion

        #region Base de recursos

        /// <summary>
        /// Obtiene la lista de bases de recursos donde el documento está compartido
        /// </summary>
        public List<Guid> BaseRecursos
        {
            get
            {
                if (mBaseRecursos == null)
                {
                    RecargarBasesRecursos();
                }
                return mBaseRecursos;
            }
        }

        #endregion

        #region RDF

        /// <summary>
        /// Devuelve el RDF de documento semántico.
        /// </summary>
        public string RdfSemantico
        {
            get
            {
                if (GestorDocumental.RdfDS != null)
                {
                    Guid proyectoID = ProyectoAD.MetaProyecto;
                    if (FilaDocumento.ProyectoID.HasValue)
                    {
                        proyectoID = FilaDocumento.ProyectoID.Value;
                    }
                    DataRow[] filasRdf = GestorDocumental.RdfDS.RdfDocumento.Select("DocumentoID='" + Clave + "' AND ProyectoID='" + proyectoID + "'");

                    if (filasRdf.Length > 0)
                    {
                        return ((RdfDS.RdfDocumentoRow)filasRdf[0]).RdfSem;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Url de los recursos relacionados o contextos que deben aparecer en el RDF del documento.
        /// </summary>
        public List<string> UrlRecursosRelacionadosRDF
        {
            get
            {
                if (mUrlRecursosRelacionadosRDF == null)
                {
                    mUrlRecursosRelacionadosRDF = new List<string>();
                }

                return mUrlRecursosRelacionadosRDF;
            }
        }

        #endregion

        #endregion

        #region Métodos

        #region Comentarios

        /// <summary>
        /// Obtiene el proyecto en el que se ha hecho un comentario a el documento actual.
        /// </summary>
        /// <param name="pComentarioID">Identificador del comentario</param>
        /// <returns>Identificador del proyecto donde se ha hecho el comentario</returns>
        public Guid ObtenerProyectoDeComentario(Guid pComentarioID)
        {
            return GestorDocumental.ObtenerProyectoDeComentario(pComentarioID, Clave);
        }

        /// <summary>
        /// Recarga la lista de comentarios de un documento de forma que aparezcan ordenados por fecha descendentemente.
        /// </summary>
        public void RecargarComentariosDocumentos()
        {
            mListaComentarios = new List<Comentario.Comentario>();
            Dictionary<Guid, Comentario.Comentario> listaComentariosAux = new Dictionary<Guid, Es.Riam.Gnoss.Elementos.Comentario.Comentario>();
            if (GestorDocumental.DataWrapperDocumentacion != null)
            {
                List<AD.EntityModel.Models.Documentacion.DocumentoComentario> filasComentarioDoc = GestorDocumental.DataWrapperDocumentacion.ListaDocumentoComentario.Where(item => item.DocumentoID.Equals(this.Clave)).ToList();

                SortedDictionary<DateTime, List<Guid>> listaComentariosOrderFecha = new SortedDictionary<DateTime, List<Guid>>();

                if (GestorDocumental.GestionComentarios != null)
                {
                    foreach (AD.EntityModel.Models.Documentacion.DocumentoComentario filaComentarioDoc in filasComentarioDoc)
                    {
                        if (GestorDocumental.GestionComentarios.ListaComentarios.ContainsKey(filaComentarioDoc.ComentarioID))
                        {
                            Comentario.Comentario comentario = GestorDocumental.GestionComentarios.ListaComentarios[filaComentarioDoc.ComentarioID];
                            if (!comentario.FilaComentario.Eliminado)
                            {
                                if (listaComentariosOrderFecha.ContainsKey(comentario.Fecha))
                                {
                                    listaComentariosOrderFecha[comentario.Fecha].Add(comentario.Clave);
                                }
                                else
                                {
                                    List<Guid> listaIDComent = new List<Guid>();
                                    listaIDComent.Add(comentario.Clave);

                                    listaComentariosOrderFecha.Add(comentario.Fecha, listaIDComent);
                                }
                            }
                        }
                    }
                }

                foreach (List<Guid> listaComentariosPorFecha in listaComentariosOrderFecha.Values)
                {
                    foreach (Guid comentarioID in listaComentariosPorFecha)
                    {
                        mListaComentarios.Add(GestorDocumental.GestionComentarios.ListaComentarios[comentarioID]);
                    }
                }
            }



            mListaComentarios.Reverse();
        }

        #endregion

        #region Permisos

        /// <summary>
        /// Indica si una identidad tiene permisos de edición sobre el documento actual.
        /// </summary>
        /// <param name="pIdentidad">Identidad a comprobar</param>
        /// <param name="pIdentidadOrganizacion">Identidad de la organización del recurso en caso de haberla</param>
        /// <param name="pProyectoActual">Proyecto actual</param>
        /// <returns>TRUE si tiene persmisos de edición, FALSE en caso contario</returns>
        public bool TienePermisosEdicionIdentidad(Identidad.Identidad pIdentidad, Identidad.Identidad pIdentidadOrganizacion, Elementos.ServiciosGenerales.Proyecto pProyectoActual, Guid pUsuarioID, bool pEsAdministradorDeOrganizacion)
        {
            if (TipoDocumentacion != TiposDocumentacion.EntradaBlog)
            {
                Guid identidadCreador = pIdentidad.Clave;

                if (pIdentidad.TrabajaConOrganizacion)
                {
                    //Puede editarlo si es administrador de la organizacion del creador       
                    if (GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursos.Count > 0)
                    {
                        List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filasDocVinBR = GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(Clave) && doc.BaseRecursosID.Equals(GestorDocumental.BaseRecursosIDActual)).ToList();
                        if (filasDocVinBR.Count > 0 && filasDocVinBR[0].TipoPublicacion == (short)TipoPublicacion.Publicado && pIdentidad.Tipo != TiposIdentidad.Personal && pIdentidad.OrganizacionID == OrganizacionPerfilCreador && pEsAdministradorDeOrganizacion)
                        {
                            return true;
                        }
                    }

                    //Puede editarlo si es administrador del proyecto y miembro de la organizacion del creador
                    if (GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursos.Count > 0)
                    {
                        List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filasDocVinBR = GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(Clave) && doc.BaseRecursosID.Equals(GestorDocumental.BaseRecursosIDActual)).ToList();
                        if (filasDocVinBR.Count > 0 && filasDocVinBR[0].TipoPublicacion == (short)TipoPublicacion.Publicado && pIdentidad.Tipo != TiposIdentidad.Personal && pIdentidad.OrganizacionID == OrganizacionPerfilCreador && pProyectoActual.EsAdministradorUsuario(pUsuarioID))
                        {
                            return true;
                        }
                    }

                    if (pIdentidadOrganizacion != null && pEsAdministradorDeOrganizacion)
                    {
                        identidadCreador = pIdentidadOrganizacion.Clave;
                    }
                }

                if (this.ListaPerfilesEditores.Count == 0) //Si no hay editores, que lo pueda editar el creador.
                {
                    if (CreadorID == identidadCreador || pIdentidad.ListaTodosIdentidadesDeIdentidad.Contains(CreadorID))
                    {
                        return true;
                    }
                }

                bool esAdminComunidadActualSinCompartir = false;
                if (pProyectoActual != null)
                {
                    esAdminComunidadActualSinCompartir = (pProyectoActual.EsCatalogo && pProyectoActual.Clave == this.ProyectoID && pProyectoActual.EsAdministradorUsuario(pUsuarioID));
                }
                //NO BORRAR: Puede que se vuelva a usar
                //if (Usuario.UsuarioActual != null)
                //{
                //    esAdminComunidadActualSinCompartir = (Usuario.UsuarioActual.ProyectoID == this.ProyectoID && pIdentidad.EsSupervisor && this.FilaDocumento.GetDocumentoWebVinBaseRecursosRows().Length == 1);
                //}
                bool docEsWiki = (TipoDocumentacion.Equals(TiposDocumentacion.Wiki));
                bool esEditorDoc = EsEditoraIdentidad(pIdentidad, true);

                return esAdminComunidadActualSinCompartir || docEsWiki || esEditorDoc;

            }
            return false;
        }

        /// <summary>
        /// Comprueba si una identidad tiene permisos para eliminar un recurso.
        /// </summary>
        /// <param name="pIdentidad">Identidad a comprobar</param>
        /// <param name="pBaseRecursosID">Identificador de la base de recursos</param>
        /// <returns>TRUE si tiene persmisos de eliminación, FALSE en caso contario</returns>
        public bool TienePermisosIdentidadEliminarRecursoEnBR(Identidad.Identidad pIdentidad, Guid pBaseRecursosID, Elementos.ServiciosGenerales.Proyecto pProyecto, Guid pUsuarioID, Guid pProyectoID, bool pEsAdministradorDeOrganizacion, bool pEsSupervisorProyecto)
        {
            if (TienePermisosEdicionIdentidad(pIdentidad, null, pProyecto, pUsuarioID, pEsAdministradorDeOrganizacion))
            {
                return true;
            }

            foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaDocWeb in GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(this.Clave) && doc.BaseRecursosID.Equals(pBaseRecursosID)))
            {
                if (filaDocWeb.IdentidadPublicacionID.HasValue && pIdentidad.Clave == filaDocWeb.IdentidadPublicacionID.Value || pIdentidad.ListaTodosIdentidadesDeIdentidad.Contains(filaDocWeb.IdentidadPublicacionID.Value))
                {
                    return true;
                }
                else
                {
                    foreach (AD.EntityModel.Models.Documentacion.BaseRecursosProyecto filaBRProy in GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosProyecto.Where(doc => doc.BaseRecursosID.Equals(pBaseRecursosID)))
                    {
                        //Si es admininstrador del proyecto en el que estamos:
                        if (filaBRProy.ProyectoID == pProyectoID && pEsSupervisorProyecto)
                        {
                            return true;
                        }
                        bool esAdminOrgDeOrgDeIdentidadPublicadorDoc = (pIdentidad.Tipo != TiposIdentidad.Personal && pIdentidad.OrganizacionID == OrganizacionPerfilCreador && FilaDocumento.Publico && pEsAdministradorDeOrganizacion);

                        //Si es administrador de la organización a la que pertenece al identidad compartidora del proyecto actual:
                        if (filaBRProy.ProyectoID == pProyectoID && esAdminOrgDeOrgDeIdentidadPublicadorDoc)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Indica si la identidad actual tiene permisos de editar las categorías y los tags del documento en el proyectoActual.
        /// </summary>
        /// <param name="pIdentidad">Identidad</param>
        /// <param name="pBaseRecursosID">Identificador de la base de recursos</param>
        /// <returns>TRUE si la identidad actual tiene permisos de editar las categorías 
        /// y los tags del documento en el proyectoActual, FALSE en caso contrario</returns>
        public bool TienePermisosIdentidadEditarTagsYCategoriasEnBR(Identidad.Identidad pIdentidad, Guid pBaseRecursosID, Guid pProyectoID, bool pEsSupervisorProyecto, bool pEsAdministrador)
        {
            foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaDocWeb in GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(this.Clave) && doc.BaseRecursosID.Equals(pBaseRecursosID)))
            {
                if (pIdentidad.Clave == filaDocWeb.IdentidadPublicacionID)
                {
                    return true;
                }
                else
                {
                    foreach (AD.EntityModel.Models.Documentacion.BaseRecursosProyecto filaBRProy in GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosProyecto.Where(baseRecProy => baseRecProy.BaseRecursosID.Equals(pBaseRecursosID)))
                    {
                        //Si es admininstrador del proyecto en el que estamos:
                        if (filaBRProy.ProyectoID == pProyectoID && pEsSupervisorProyecto)
                        {
                            return true;
                        }
                        bool esAdminOrgDeOrgDeIdentidadPublicadorDoc = pIdentidad.Tipo != TiposIdentidad.Personal && pIdentidad.OrganizacionID == OrganizacionPerfilCreador && FilaDocumento.Publico && pEsAdministrador;

                        //Si es administrador de la organización a la que pertenece al identidad compartidora del proyecto actual:
                        if (filaBRProy.ProyectoID == pProyectoID && esAdminOrgDeOrgDeIdentidadPublicadorDoc)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Indica si un perfil es editor o lector de un documento.
        /// </summary>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <returns>TRUE si un perfil es editor o lector de un documento, FALSE en caso contrario</returns>
        public bool EsEditoraOLectoraIdentidad(Identidad.Identidad pIdentidad)
        {
            return EsEditoraIdentidad(pIdentidad, false);
        }

        /// <summary>
        /// Indica si un perfil es editor o lector de un documento.
        /// </summary>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <param name="pSoloEditor">Comprobar solo si es editor</param>
        /// <returns>TRUE si un perfil es editor o lector de un documento, FALSE en caso contrario</returns>
        public bool EsEditoraIdentidad(Identidad.Identidad pIdentidad, bool pSoloEditor)
        {
            if (!pSoloEditor && FilaDocumentoWebVinBR != null && !FilaDocumentoWebVinBR.PrivadoEditores)
            {
                //No es privado
                return true;
            }
            else
            {
                if (ListaPerfilesEditores.ContainsKey(pIdentidad.PerfilID) && (!pSoloEditor || ListaPerfilesEditoresSinLectores.ContainsKey(pIdentidad.PerfilID)))
                {
                    //El perfil es editor
                    return true;
                }
                else//Comprobanmos si pertenece a un grupo editor
                {
                    if (pSoloEditor)
                    {
                        return pIdentidad.ListaProyectosPerfilActual.Values.Any(p => ListaGruposEditoresSinLectores.Values.Any(grupoEditor => grupoEditor.listaIdentidadedsParticipacion.Any(i => i.Equals(p))));
                    }
                    else
                    {
                        return pIdentidad.ListaProyectosPerfilActual.Values.Any(p => ListaGruposEditores.Values.Any(grupoEditor => grupoEditor.listaIdentidadedsParticipacion.Any(i => i.Equals(p))));
                    }
                }
            }
        }

        /// <summary>
        /// Indica si un determinada identidad puede bloquear y desbloquedar los comentario del documento.
        /// </summary>
        /// <param name="pIdentidad">Identidad</param>
        /// <returns>TRUE si la identidad puede bloquear y desbloquedar los comentario del documento, FALSE en caso contario</returns>
        public bool TienePermisosIdentidadBloquearDesbloquearComentarios(Identidad.Identidad pIdentidad, Elementos.ServiciosGenerales.Proyecto pProyectoActual, Guid pUsuarioID, bool pEsAdministradorDeOrganizacion)
        {
            return (FilaDocumento.UltimaVersion && !EsBorrador && ((ProyectoID == pIdentidad.FilaIdentidad.ProyectoID && TienePermisosEdicionIdentidad(pIdentidad, null, pProyectoActual, pUsuarioID, pEsAdministradorDeOrganizacion)) || pProyectoActual.EsAdministradorUsuario(pUsuarioID)));
        }

        /// <summary>
        /// Comprueba si un enlace pertenece a una plataforma web de vídeos (youtube, vimeo...)
        /// </summary>
        /// <param name="pEnlace">Enlace a comprobar</param>
        /// <returns></returns>
        public bool CompruebaUrlEsVideo(string pEnlace)
        {
            string enlaceSinHttp = pEnlace.Replace("https://", "").Replace("http://", "");

            if ((enlaceSinHttp.StartsWith("www.youtube.com") || enlaceSinHttp.StartsWith("youtube.com")) && enlaceSinHttp.Contains("/watch?") || enlaceSinHttp.StartsWith("www.youtube.com") || enlaceSinHttp.StartsWith("youtube.com") || enlaceSinHttp.StartsWith("youtu.be/"))
            {
                string v = "";
                if (enlaceSinHttp.StartsWith("youtu.be/"))
                {
                    v = enlaceSinHttp.Replace("youtu.be/", "");

                    if (v.Contains("/"))
                    {
                        v = v.Substring(0, v.IndexOf("/"));
                    }
                }
                else
                {
                    v = System.Web.HttpUtility.ParseQueryString(new Uri(pEnlace).Query).Get("v");
                }

                return (!string.IsNullOrEmpty(v));
            }
            else if (enlaceSinHttp.StartsWith("www.vimeo.com") || enlaceSinHttp.StartsWith("vimeo.com"))
            {
                string v = (new Uri(pEnlace)).AbsolutePath;
                int idVideo;
                int inicio = v.LastIndexOf("/");
                return (int.TryParse(v.Substring(inicio + 1, v.Length - inicio - 1), out idVideo));
            }
            else if (pEnlace.StartsWith("http://www.ted.com/talks/") || pEnlace.StartsWith("www.ted.com/talks/") || pEnlace.StartsWith("ted.com/talks/") || pEnlace.StartsWith("http://tedxtalks.ted.com/video/") || pEnlace.StartsWith("tedxtalks.ted.com/video/"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Comprueba si un enlace pertenece a una presentación de slideshare
        /// </summary>
        /// <param name="pEnlace">Enlace a comprobar</param>
        /// <returns></returns>
        public bool CompruebaUrlEsPresentacion(string pEnlace)
        {
            if (!string.IsNullOrEmpty(pEnlace) && (pEnlace.StartsWith("http://www.slideshare.net") || pEnlace.StartsWith("http://es.slideshare.net") || pEnlace.StartsWith("www.slideshare.net") || pEnlace.StartsWith("https://www.slideshare.net") || pEnlace.StartsWith("https://es.slideshare.net") || pEnlace.Contains("slideshare.net")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Generales

        /// <summary>
        /// Obtiene la identidad publicadora en la BR actual.
        /// </summary>
        /// <returns>Identidad publicadora en la BR actual</returns>
        public Guid ObtenerPublicadorEnBR()
        {
            return ObtenerPublicadorEnBR(GestorDocumental.BaseRecursosIDActual);
        }

        /// <summary>
        /// Obtiene la identidad publicadora en una BR.
        /// </summary>
        /// <param name="pBaseRecursosID">Identificdor de la BR</param>
        /// <returns>Identidad publicadora en una BR</returns>
        public Guid ObtenerPublicadorEnBR(Guid pBaseRecursosID)
        {
            Guid identidad = Guid.Empty;

            List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filaDocVinBR = GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.BaseRecursosID.Equals(pBaseRecursosID)).ToList();
            if (filaDocVinBR.Count > 0 && filaDocVinBR.First().IdentidadPublicacionID.HasValue)
            {
                identidad = filaDocVinBR.First().IdentidadPublicacionID.Value;
            }

            return identidad;
        }

        /// <summary>
        /// Comprueba si pertenece el documento a unos proyectos determinados.
        /// </summary>
        /// <param name="pProyectosID">Identificadores de loes proyectos</param>
        /// <returns>TRUE si pertenece el documento a unos proyectos determinados, FALSE en caso contrario</returns>
        public bool PerteneceDocumentoAComunidades(List<Guid> pProyectosID)
        {
            foreach (Guid proyectoID in pProyectosID)
            {
                if (ListaProyectos.Contains(proyectoID))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Comprueba si pertenece el documento a un proyecto determinado.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>TRUE si pertenece el documento a un proyecto determinado, FALSE en caso contrario</returns>
        public bool PerteneceDocumentoAComunidad(Guid pProyectoID)
        {
            return (ListaProyectos.Contains(pProyectoID));
        }

        #endregion

        #region votos

        /// <summary>
        /// Devuelve la lista de los los votos de una detyerminada comunidad
        /// </summary>
        public Dictionary<Guid, AD.EntityModel.Models.Voto.Voto> ListaVotosPorComunidad(Guid pProyectoID)
        {
            Dictionary<Guid, AD.EntityModel.Models.Voto.Voto> ListaVotos = new Dictionary<Guid, AD.EntityModel.Models.Voto.Voto>();
            if (GestorDocumental.DataWrapperDocumentacion != null)
            {
                List<AD.EntityModel.Models.Documentacion.VotoDocumento> filasVotosDoc = GestorDocumental.DataWrapperDocumentacion.ListaVotoDocumento.Where(item => item.DocumentoID.Equals(this.Clave) && item.ProyectoID.Value.Equals(pProyectoID)).ToList();

                foreach (AD.EntityModel.Models.Documentacion.VotoDocumento filaVotodoc in filasVotosDoc)
                {
                    if (GestorDocumental.GestorVotos.ListaVotos.ContainsKey(filaVotodoc.VotoID))
                    {
                        AD.EntityModel.Models.Voto.Voto voto = GestorDocumental.GestorVotos.ListaVotos[filaVotodoc.VotoID];
                        ListaVotos.Add(voto.VotoID, voto);
                    }
                }
            }


            return ListaVotos;

        }

        #endregion

        #region Categorias

        /// <summary>
        /// Recarga las categorías del tesauro del dataSet.
        /// </summary>
        public void RecargarCategoriasTesauro()
        {
            mListaCategorias = new SortedList<Guid, CategoriaTesauro>();

            if (this.GestorDocumental.ListaCategoriasTesauro != null)
            {
                foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaComunidad in FilaDocumento.DocumentoWebVinBaseRecursos)
                {
                    var filasCategorias = GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(doc => doc.DocumentoID.Equals(filaComunidad.DocumentoID) && doc.BaseRecursosID.Equals(filaComunidad.BaseRecursosID));
                    //DocumentacionDS.DocumentoWebAgCatTesauroRow[] categorias = filaComunidad.DocumentoWebAgCatTesauro;

                    foreach (AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro filaCategoria in filasCategorias)
                    {
                        if (GestorDocumental.ListaCategoriasTesauro.ContainsKey(filaCategoria.CategoriaTesauroID) && !mListaCategorias.ContainsKey(filaCategoria.CategoriaTesauroID))
                        {
                            CategoriaTesauro categoria = GestorDocumental.ListaCategoriasTesauro[(Guid)filaCategoria.CategoriaTesauroID];
                            mListaCategorias.Add(categoria.Clave, categoria);
                        }
                    }
                }
            }
        }

        #endregion

        #region Base de recursos

        /// <summary>
        /// Recarga las bases de recursos del documento.
        /// </summary>
        public void RecargarBasesRecursos()
        {
            mBaseRecursos = new List<Guid>();

            foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaComunidad in FilaDocumento.DocumentoWebVinBaseRecursos)
            {
                if (!filaComunidad.Eliminado)
                {
                    mBaseRecursos.Add(filaComunidad.BaseRecursosID);
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
        ~Documento()
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
            if (!this.disposed)
            {
                this.disposed = true;
                try
                {
                    if (disposing)
                    {

                    }

                    //Libero todos los recursos nativos sin administrar que he añadido a esta clase
                }
                finally
                {
                    // Llamo al dispose de la clase base
                    base.Dispose(disposing);
                }
            }
        }

        #endregion
    }
}
