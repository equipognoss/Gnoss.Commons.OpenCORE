using System;
using System.Collections.Generic;
using System.Text;
using Es.Riam.Semantica.OWL;
using System.Runtime.Serialization;

namespace Es.Riam.Semantica.Plantillas
{
    /// <summary>
    /// Clase para gestionar los estilos de un elemento de la plantilla.
    /// </summary>
    [Serializable]
    public class EstiloPlantillaConfigGen : EstiloPlantilla, ISerializable
    {
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros.
        /// </summary>
        public EstiloPlantillaConfigGen()
        {
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected EstiloPlantillaConfigGen(SerializationInfo info, StreamingContext context)
        {
            mAcciones = (Dictionary<string, AccionSemCms>)info.GetValue("Acciones", typeof(Dictionary<string, AccionSemCms>));
            mCategoriasPorDefecto = (List<string>)info.GetValue("CategoriasPorDefecto", typeof(List<string>));
            mCategorizacionTesauroGnossNoObligatoria = (bool)info.GetValue("CategorizacionTesauroGnossNoObligatoria", typeof(bool));
            mEtiquetacionGnossNoObligatoria= (bool)info.GetValue("EtiquetacionGnossNoObligatoria", typeof(bool));
            mCKEditorComentariosCompleto = (bool)info.GetValue("CKEditorComentariosCompleto", typeof(bool));
            mCondiciones = (Dictionary<string, CondicionSemCms>)info.GetValue("Condiciones", typeof(Dictionary<string, CondicionSemCms>));
            mGrafosSimplesAutocompletar = (List<string>)info.GetValue("GrafosSimplesAutocompletar", typeof(List<string>));
            mGruposEditoresFijos = (Dictionary <string, List<string>>)info.GetValue("GruposEditoresFijos", typeof(Dictionary<string, List<string>>));
            mGruposEditoresPrivacidad = (Dictionary<string, List<string>>)info.GetValue("GruposEditoresPrivacidad", typeof(Dictionary<string, List<string>>));
            mGruposLectoresFijos = (Dictionary<string, List<string>>)info.GetValue("GruposLectoresFijos", typeof(Dictionary<string, List<string>>));
            mHayCampoOrden = (bool)info.GetValue("HayCampoOrden", typeof(bool));
            mHayEntidadesSelecc = (bool)info.GetValue("HayEntidadesSelecc", typeof(bool));
            mHayEntidadesSeleccEditables = (bool)info.GetValue("HayEntidadesSeleccEditables", typeof(bool));
            mHayFechaConHora = (bool)info.GetValue("HayFechaConHora", typeof(bool));
            mHayJcrop = (bool)info.GetValue("HayJcrop", typeof(bool));
            mHayValoresGrafoDependienets = (bool)info.GetValue("HayValoresGrafoDependienets", typeof(bool));
            mHtmlNuevo = (short)info.GetValue("HtmlNuevo", typeof(short));
            mIncluirIconoGnoss = (bool)info.GetValue("IncluirIconoGnoss", typeof(bool));
            mIndexRobots = (string)info.GetValue("IndexRobots", typeof(string));
            mListaIdiomas = (List<string>)info.GetValue("ListaIdiomas", typeof(List<string>));
            mMenuDocumentoAbajo = (bool?)info.GetValue("MenuDocumentoAbajo", typeof(bool?));
            mMetasHTMLOntologia = (Dictionary<string, List<Dictionary<string, string>>>)info.GetValue("MetasHTMLOntologia", typeof(Dictionary<string, List<Dictionary<string, string>>>));
            mMostrarFechaRec = (bool)info.GetValue("MostrarFechaRec", typeof(bool));
            mMultiIdioma = (bool)info.GetValue("MultiIdioma", typeof(bool));
            mNamespace = (string)info.GetValue("Namespace", typeof(string));
            mOrdenAutocompletar = (string)info.GetValue("OrdenAutocompletar", typeof(string));
            mOrdenAutocompletarIsAsc = (bool)info.GetValue("OrdenAutocompletarIsAsc", typeof(bool));
            mOcultarAccionesDoc = (bool)info.GetValue("OcultarAccionesDoc", typeof(bool));
            mOcultarAutoresDoc = (bool)info.GetValue("OcultarAutoresDoc", typeof(bool));
            mOcultarAutoria = (bool)info.GetValue("OcultarAutoria", typeof(bool));
            mOcultarBloqueCompartirEdicion = (bool)info.GetValue("OcultarBloqueCompartirEdicion", typeof(bool));
            mOcultarBloquePrivacidadSeguridadEdicion = (bool)info.GetValue("OcultarBloquePrivacidadSeguridadEdicion", typeof(bool));
            mOcultarBloquePropiedadIntelectualEdicion = (bool)info.GetValue("OcultarBloquePropiedadIntelectualEdicion", typeof(bool));
            mOcultarBotonAgregarCategoriaDoc = (bool)info.GetValue("OcultarBotonAgregarCategoriaDoc", typeof(bool));
            mOcultarBotonAgregarEtiquetasDoc = (bool)info.GetValue("OcultarBotonAgregarEtiquetasDoc", typeof(bool));
            mOcultarBotonBloquearComentariosDoc = (bool)info.GetValue("OcultarBotonBloquearComentariosDoc", typeof(bool));
            mOcultarBotonCertificarDoc = (bool)info.GetValue("OcultarBotonCertificarDoc", typeof(bool));
            mOcultarBotonCrearVersionDoc = (bool)info.GetValue("OcultarBotonCrearVersionDoc", typeof(bool));
            mOcultarBotonEditarDoc = (bool)info.GetValue("OcultarBotonEditarDoc", typeof(bool));
            mOcultarBotonEliminarDoc = (bool)info.GetValue("OcultarBotonEliminarDoc", typeof(bool));
            mOcultarBotonEliminarDocCondicion = (string)info.GetValue("OcultarBotonEliminarDocCondicion", typeof(string));
            mOcultarBotonEnviarEnlaceDoc = (bool)info.GetValue("OcultarBotonEnviarEnlaceDoc", typeof(bool));
            mOcultarBotonHistorialDoc = (bool)info.GetValue("OcultarBotonHistorialDoc", typeof(bool));
            mOcultarBotonRestaurarVersionDoc = (bool)info.GetValue("OcultarBotonRestaurarVersionDoc", typeof(bool));
            mOcultarBotonVincularDoc = (bool)info.GetValue("OcultarBotonVincularDoc", typeof(bool));
            mOcultarCategoriasDoc = (bool)info.GetValue("OcultarCategoriasDoc", typeof(bool));
            mOcultarComentarios = (bool)info.GetValue("OcultarComentarios", typeof(bool));
            mOcultarCompartidoDoc = (bool)info.GetValue("OcultarCompartidoDoc", typeof(bool));
            mOcultarCompartirEspecioPersonal = (bool)info.GetValue("OcultarCompartirEspecioPersonal", typeof(bool));
            mOcultarEditoresDoc = (bool)info.GetValue("OcultarEditoresDoc", typeof(bool));
            mOcultarEtiquetasDoc = (bool)info.GetValue("OcultarEtiquetasDoc", typeof(bool));
            mOcultarFechaRec = (bool)info.GetValue("OcultarFechaRec", typeof(bool));
            mOcultarLicenciaDoc = (bool)info.GetValue("OcultarLicenciaDoc", typeof(bool));
            mOcultarPublicadorDoc = (bool)info.GetValue("OcultarPublicadorDoc", typeof(bool));
            mOcultarRecursoAUsuarioInvitado = (bool)info.GetValue("OcultarRecursoAUsuarioInvitado", typeof(bool));
            mOcultarTesauro = (bool)info.GetValue("OcultarTesauro", typeof(bool));
            mOcultarTituloDescpEImg = (bool)info.GetValue("OcultarTituloDescpEImg", typeof(bool));
            mOcultarUtilsDoc = (bool)info.GetValue("OcultarUtilsDoc", typeof(bool));
            mOcultarVersionDoc = (bool)info.GetValue("OcultarVersionDoc", typeof(bool));
            mOcultarVisitasDoc = (bool)info.GetValue("OcultarVisitasDoc", typeof(bool));
            mOcultarVotosDoc = (bool)info.GetValue("OcultarVotosDoc", typeof(bool));
            mPrivadoParaGrupoEditores = (List<Guid>)info.GetValue("PrivadoParaGrupoEditores", typeof(List<Guid>));
            mPropiedadArchivoCargaMasiva = (KeyValuePair<string, string>)info.GetValue("PropiedadArchivoCargaMasiva", typeof(KeyValuePair<string, string>));
            mPropiedadDescripcion = (KeyValuePair<string, string>)info.GetValue("PropiedadDescripcion", typeof(KeyValuePair<string, string>));
            mPropiedadesOntologia = (Dictionary<string, string>)info.GetValue("PropiedadesOntologia", typeof(Dictionary<string, string>));
            mPropiedadesOpenSeaDragon = (List<KeyValuePair<string, string>>)info.GetValue("PropiedadesOpenSeaDragon", typeof(List<KeyValuePair<string, string>>));
            mPropiedadesTesSem = (List<KeyValuePair<string, string>>)info.GetValue("PropiedadesTesSem", typeof(List<KeyValuePair<string, string>>));
            mPropiedadesTitulo = (Dictionary<string, string>)info.GetValue("PropiedadesTitulo", typeof(Dictionary<string, string>));
            mPropiedadImagenFromURL = (List<KeyValuePair<string, string>>)info.GetValue("PropiedadImagenFromURL", typeof(List<KeyValuePair<string, string>>));
            mPropiedadImagenRepre = (KeyValuePair<string, string>)info.GetValue("PropiedadImagenRepre", typeof(KeyValuePair<string, string>));
            mPropiedadTitulo = (KeyValuePair<string, string>)info.GetValue("PropiedadTitulo", typeof(KeyValuePair<string, string>));
            mPropsComprobarRepeticion = (Dictionary<KeyValuePair<string, string>, KeyValuePair<bool, short>>)info.GetValue("PropsComprobarRepeticion", typeof(Dictionary<KeyValuePair<string, string>, KeyValuePair<bool, short>>));
            mPropsRepetidas = (Dictionary<KeyValuePair<string, string>, List<string>>)info.GetValue("PropsRepetidas", typeof(Dictionary<KeyValuePair<string, string>, List<string>>));
            mPropsSelecEntDependientes = (Dictionary<KeyValuePair<string, string>, List<KeyValuePair<string, string>>>)info.GetValue("PropsSelecEntDependientes", typeof(Dictionary<KeyValuePair<string, string>, List<KeyValuePair<string, string>>>));
            mTipoVisiblidadEdicionRec = (string)info.GetValue("TipoVisiblidadEdicionRec", typeof(string));
            mTituloSoloMiembros = (Dictionary<string, string>)info.GetValue("TituloSoloMiembros", typeof(Dictionary<string, string>));
        }

        #endregion

        #region Miembros

        /// <summary>
        /// Namespace
        /// </summary>
        private string mNamespace;

        /// <summary>
        /// Orden que se va a emplear en el autocompletado complejo
        /// </summary>
        private string mOrdenAutocompletar;

        /// <summary>
        /// tipo de orden que se va a utilizar, true para orden ascendente, false para descendente
        /// </summary>
        private bool mOrdenAutocompletarIsAsc;

        /// <summary>
        /// Lista de idiomas
        /// </summary>
        private List<string> mListaIdiomas;

        /// <summary>
        /// Nombre de la propiedad que es titulo.
        /// </summary>
        private KeyValuePair<string, string> mPropiedadTitulo;

        /// <summary>
        /// Nombre de la propiedad que es titulo.
        /// </summary>
        private Dictionary<string,string> mPropiedadesTitulo;

        /// <summary>
        /// Nombre de la propiedad que es descripción. 
        /// </summary>
        private KeyValuePair<string, string> mPropiedadDescripcion;

        /// <summary>
        /// Nombre de la propiedad que es imagen representante formulario. 
        /// </summary>
        private KeyValuePair<string, string> mPropiedadImagenRepre;

        /// <summary>
        /// Nombre de la propiedad que es imagen a partir de una URL.
        /// </summary>
        private List<KeyValuePair<string, string>> mPropiedadImagenFromURL;

        /// <summary>
        /// Nombre de las propiedades que contendrán imágenes para realizar el procesado OpenSeaDragon.
        /// </summary>
        private List<KeyValuePair<string, string>> mPropiedadesOpenSeaDragon;

        /// <summary>
        /// Ontología.
        /// </summary>
        private Ontologia mOntologia;

        /// <summary>
        /// Indica si debe ocultarse el titulo, la descripción y la imagen en la ficha del recurso.
        /// </summary>
        private bool mOcultarTituloDescpEImg;

        /// <summary>
        /// Indica si debe ocultarse el recurso
        /// </summary>
        private bool mOcultarRecursoAUsuarioInvitado;
        

        /// <summary>
        /// Indica si debe ocultarse la fecha de la ficha del recurso.
        /// </summary>
        private bool mOcultarFechaRec;

        /// <summary>
        /// Idica si se debe ocultar la autoría.
        /// </summary>
        private bool mOcultarAutoria;

        /// <summary>
        /// Indica si debe mostrarse la fecha de la ficha del recurso.
        /// </summary>
        private bool mMostrarFechaRec;

        /// <summary>
        /// Indica si debe mostrarse el menú del recurso abajo.
        /// </summary>
        private bool? mMenuDocumentoAbajo;

        /// <summary>
        /// Categorías del tesauro por defecto para los documentos de esta plantilla.
        /// </summary>
        private List<string> mCategoriasPorDefecto;

        /// <summary>
        /// Indica si hay que ocultar el Tesauro
        /// </summary>
        private bool mOcultarTesauro;

        /// <summary>
        /// Indica si se debe incluir el icono de GNOSS o no.
        /// </summary>
        private bool mIncluirIconoGnoss;

        /// <summary>
        /// Indica si el CKEditor de comentario debe ser completo o no.
        /// </summary>
        private bool mCKEditorComentariosCompleto;

        /// <summary>
        /// Nombre de los grupos de editores que deben serlo y el id de organizacion si es un grupo de organizacion(si es de proyecto, NULL).
        /// </summary>
        private Dictionary<string, List<string>> mGruposEditoresFijos;

        /// <summary>
        /// Nombre de los grupos de editores que deben serlo y el id de organizacion si es un grupo de organizacion(si es de proyecto, NULL).
        /// </summary>
        private Dictionary<string, List<string>> mGruposEditoresPrivacidad;

        /// <summary>
        /// Tipo de visibilidad de la edición del recurso.
        /// </summary>
        private string mTipoVisiblidadEdicionRec;

        /// <summary>
        /// Nombre de los grupos de lectores que deben serlo y el id de organizacion si es un grupo de organizacion(si es de proyecto, NULL).
        /// </summary>
        private Dictionary<string, List<string>> mGruposLectoresFijos;

        /// <summary>
        /// Indica si hay que usar el HTML nuevo o no y que versión del mismo.
        /// </summary>
        private short mHtmlNuevo;

        /// <summary>
        /// Indica si no es obligatorio categorizar sobre el tesauro de GNOSS.
        /// </summary>
        private bool mCategorizacionTesauroGnossNoObligatoria;

        /// <summary>
        /// Indica si no es obligatoria etiquetar un recurso semantico de GNOSS.
        /// </summary>
        private bool mEtiquetacionGnossNoObligatoria;

        /// <summary>
        /// Nombre de la propiedad que es archivo para la carga masiva. 
        /// </summary>
        private KeyValuePair<string, string> mPropiedadArchivoCargaMasiva;

        /// <summary>
        /// Indica si el formulario es multiidioma.
        /// </summary>
        private bool mMultiIdioma;

        /// <summary>
        /// Indexado para los bots.
        /// </summary>
        private string mIndexRobots;

        /// <summary>
        /// Diccionario con listas de diccionarios con las propiedades de las etiquetas HTML meta de la ontología por idioma.
        /// </summary>
        private Dictionary<string, List<Dictionary<string, string>>> mMetasHTMLOntologia;

        /// <summary>
        /// Titulo para los que no son miembros de la comunidad.
        /// </summary>
        private Dictionary<string, string> mTituloSoloMiembros;




        /// <summary>
        /// Indica si debe ocultarse el publicador del recurso.
        /// </summary>
        private bool mOcultarPublicadorDoc;

        /// <summary>
        /// Indica si debe ocultarse utils del recurso.
        /// </summary>
        private bool mOcultarUtilsDoc;

        /// <summary>
        /// Indica si debe ocultarse las acciones del recurso.
        /// </summary>
        private bool mOcultarAccionesDoc;

        /// <summary>
        /// Indica si debe ocultarse las categorías del recurso.
        /// </summary>
        private bool mOcultarCategoriasDoc;

        /// <summary>
        /// Indica si debe ocultarse las etiquetas del recurso.
        /// </summary>
        private bool mOcultarEtiquetasDoc;

        /// <summary>
        /// Indica si debe ocultarse los editores del recurso.
        /// </summary>
        private bool mOcultarEditoresDoc;

        /// <summary>
        /// Indica si debe ocultarse los autores del recurso.
        /// </summary>
        private bool mOcultarAutoresDoc;

        /// <summary>
        /// Indica si debe ocultarse las visitas del recurso.
        /// </summary>
        private bool mOcultarVisitasDoc;

        /// <summary>
        /// Indica si debe ocultarse los votos del recurso.
        /// </summary>
        private bool mOcultarVotosDoc;

        /// <summary>
        /// Indica si debe ocultarse el compartido en del recurso.
        /// </summary>
        private bool mOcultarCompartidoDoc;

        /// <summary>
        /// Indica si debe ocultarse el compartido en del recurso.
        /// </summary>
        private bool mOcultarCompartidoEnDoc;

        /// <summary>
        /// Indica si debe ocultarse los votos del recurso.
        /// </summary>
        private bool mOcultarLicenciaDoc;

        /// <summary>
        /// Indica si debe ocultarse la versión del recurso.
        /// </summary>
        private bool mOcultarVersionDoc;

        /// <summary>
        /// Indica si debe ocultarse el botón editar del recurso.
        /// </summary>
        private bool mOcultarBotonEditarDoc;

        /// <summary>
        /// Indica si debe ocultarse el botón crear versión del recurso.
        /// </summary>
        private bool mOcultarBotonCrearVersionDoc;

        /// <summary>
        /// Indica si debe ocultarse el botón enviar enlace del recurso.
        /// </summary>
        private bool mOcultarBotonEnviarEnlaceDoc;

        /// <summary>
        /// Indica si debe ocultarse el botón vincular del recurso.
        /// </summary>
        private bool mOcultarBotonVincularDoc;

        /// <summary>
        /// Indica si debe ocultarse el botón eliminar del recurso.
        /// </summary>
        private bool mOcultarBotonEliminarDoc;

        /// <summary>
        /// Devuelve la condición que debe evaluarse para ocultar el botón eliminar del recurso.
        /// </summary>
        private string mOcultarBotonEliminarDocCondicion;

        /// <summary>
        /// Indica si debe ocultarse el botón restaurar versión del recurso.
        /// </summary>
        private bool mOcultarBotonRestaurarVersionDoc;

        /// <summary>
        /// Indica si debe ocultarse el botón agregar categoría del recurso.
        /// </summary>
        private bool mOcultarBotonAgregarCategoriaDoc;

        /// <summary>
        /// Indica si debe ocultarse el botón agregar etiquetas del recurso.
        /// </summary>
        private bool mOcultarBotonAgregarEtiquetasDoc;

        /// <summary>
        /// Indica si debe ocultarse el botón historial del recurso.
        /// </summary>
        private bool mOcultarBotonHistorialDoc;

        /// <summary>
        /// Indica si debe ocultarse el botón bloquear comentarios del recurso.
        /// </summary>
        private bool mOcultarBotonBloquearComentariosDoc;

        /// <summary>
        /// Indica si debe ocultarse el botón certificar del recurso.
        /// </summary>
        private bool mOcultarBotonCertificarDoc;

        /// <summary>
        /// Indica si debe ocultarse el botón añadir a mi espacio personal.
        /// </summary>
        private bool mOcultarCompartirEspecioPersonal;

        /// <summary>
        /// Indica si deben ocultarse los comentarios del recurso.
        /// </summary>
        private bool mOcultarComentarios;

        /// <summary>
        /// Indica si debe ocultarse el bloque de privacidad y seguridad en la edición del recurso
        /// </summary>
        private bool mOcultarBloquePrivacidadSeguridadEdicion;

        /// <summary>
        /// Indica si se debe ocultar el bloque de compartir en la edición del recurso
        /// </summary>
        private bool mOcultarBloqueCompartirEdicion;

        /// <summary>
        /// Indica si se debe ocultar el bloque de propiedad intelectual en la edición del recurso
        /// </summary>
        private bool mOcultarBloquePropiedadIntelectualEdicion;
        
        /// <summary>
        /// Indica si hay entidades seleccionables.
        /// </summary>
        private bool mHayEntidadesSelecc;

         /// <summary>
        /// Indica si hay entidades seleccionables editables desde el propio recurso.
        /// </summary>
        private bool mHayEntidadesSeleccEditables;

        /// <summary>
        /// Indica si hay valores grafodependientes.
        /// </summary>
        private bool mHayValoresGrafoDependienets;

        /// <summary>
        /// Propiedades repetidas para que se duplique el campo.
        /// </summary>
        private Dictionary<KeyValuePair<string, string>, List<string>> mPropsRepetidas;

        /// <summary>
        /// Propiedades que son Tesauros Semánticos.
        /// </summary>
        private List<KeyValuePair<string, string>> mPropiedadesTesSem;

        /// <summary>
        /// Devuelve o establece si hay jcrop en la plantilla
        /// </summary>
        private bool mHayJcrop = false;

        /// <summary>
        /// Indica si hay una propiedad de fecha con hora.
        /// </summary>
        private bool mHayFechaConHora;

        /// <summary>
        /// Propiedades que debe comprobarse si están ya introduccidas en la ontología o en la comunidad.
        /// </summary>
        private Dictionary<KeyValuePair<string, string>, KeyValuePair<bool, short>> mPropsComprobarRepeticion;

        /// <summary>
        /// Propiedad que definie la privacidad de algún nodo
        /// </summary>
        private List<Guid> mPrivadoParaGrupoEditores;

        /// <summary>
        /// Lista con las Propiedad,Entidad de las que dependen la lista de Propiedad,Entidad.
        /// </summary>
        private Dictionary<KeyValuePair<string, string>, List<KeyValuePair<string, string>>> mPropsSelecEntDependientes;

        /// <summary>
        /// Lista con los grafos simples para autocompletar.
        /// </summary>
        private List<string> mGrafosSimplesAutocompletar;

        /// <summary>
        /// Diccionario con las propiedades que puede tener la ontología
        /// </summary>
        private Dictionary<string, string> mPropiedadesOntologia;

        /// <summary>
        /// Condiciones de la plantilla.
        /// </summary>
        private Dictionary<string, CondicionSemCms> mCondiciones;

        /// <summary>
        /// Acciones de la plantilla.
        /// </summary>
        private Dictionary<string, AccionSemCms> mAcciones;

        /// <summary>
        /// Indica si hay configurado algún campo orden en alguna entidad auxiliar.
        /// </summary>
        private bool mHayCampoOrden;

        #endregion

        #region Propiedades

        /// <summary>
        /// Namespace
        /// </summary>
        public string Namespace
        {
            get
            {
                return mNamespace;
            }
            set
            {
                mNamespace = value;
            }
        }

        /// <summary>
        /// Campo de orden que se va a utilizar en el autocompletado
        /// </summary>
        public string OrdenAutocompletar
        {
            get
            {
                return mOrdenAutocompletar;
            }
            set
            {
                mOrdenAutocompletar = value;
            }
        }

        /// <summary>
        /// Tipo de orden que se va a usar en el autocompletado, true para ascendente, false para descendente
        /// </summary>
        public bool OrdenAutocompletarIsAsc
        {
            get
            {
                return mOrdenAutocompletarIsAsc;
            }
            set
            {
                mOrdenAutocompletarIsAsc = value;
            }
        }

        /// <summary>
        /// Lista de idiomas
        /// </summary>
        public List<string> ListaIdiomas
        {
            get
            {
                if (mListaIdiomas == null)
                {
                    mListaIdiomas = new List<string>();
                }

                return mListaIdiomas;
            }
            set
            {
                mListaIdiomas = value;
            }
        }

        /// <summary>
        /// Nombre de la propiedad que es titulo (Propiedad / Entidad).
        /// </summary>
        public KeyValuePair<string, string> PropiedadTitulo
        {
            get
            {
                return mPropiedadTitulo;
            }
            set
            {
                mPropiedadTitulo = value;
            }
        }

        /// <summary>
        /// Nombre de la propiedad que es titulo (Propiedad / Entidad).
        /// </summary>
        public Dictionary<string, string> PropiedadesTitulo
        {
            get
            {
                if (mPropiedadesTitulo == null)
                {
                    mPropiedadesTitulo = new Dictionary<string, string>(); 
                }

                return mPropiedadesTitulo;
            }
            set
            {
                mPropiedadesTitulo = value;
            }
        }

        /// <summary>
        /// Nombre de la propiedad que es descripción. 
        /// </summary>
        public KeyValuePair<string, string> PropiedadDescripcion
        {
            get
            {
                return mPropiedadDescripcion;
            }
            set
            {
                mPropiedadDescripcion = value;
            }
        }

        /// <summary>
        /// Nombre de la propiedad que es imagen representante formulario. 
        /// </summary>
        public KeyValuePair<string, string> PropiedadImagenRepre
        {
            get
            {
                return mPropiedadImagenRepre;
            }
            set
            {
                mPropiedadImagenRepre = value;
            }
        }

        /// <summary>
        /// Nombre de la propiedad que es imagen a partir de una URL.
        /// </summary>
        public List<KeyValuePair<string, string>> PropiedadImagenFromURL
        {
            get
            {
                return mPropiedadImagenFromURL;
            }
            set
            {
                mPropiedadImagenFromURL = value;
            }
        }

        /// <summary>
        /// Nombre de las propiedades que contendrán imágenes para realizar el procesado OpenSeaDragon.
        /// </summary>
        public List<KeyValuePair<string, string>> PropiedadesOpenSeaDragon
        {
            get
            {
                return mPropiedadesOpenSeaDragon;
            }
            set
            {
                mPropiedadesOpenSeaDragon = value;
            }
        }

        /// <summary>
        /// Ontología.
        /// </summary>
        public Ontologia Ontologia
        {
            get
            {
                return mOntologia;
            }
            set
            {
                mOntologia = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse el titulo, la descripción y la imagen en la ficha del recurso.
        /// </summary>
        public bool OcultarTituloDescpEImg
        {
            get
            {
                return mOcultarTituloDescpEImg;
            }
            set
            {
                mOcultarTituloDescpEImg = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse el recruso.
        /// </summary>
        public bool OcultarRecursoAUsuarioInvitado
        {
            get
            {
                return mOcultarRecursoAUsuarioInvitado;
            }
            set
            {
                mOcultarRecursoAUsuarioInvitado = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse la fecha de la ficha del recurso.
        /// </summary>
        public bool OcultarFechaRec
        {
            get
            {
                return mOcultarFechaRec;
            }
            set
            {
                mOcultarFechaRec = value;
            }
        }

        /// <summary>
        /// Idica si se debe ocultar la autoría.
        /// </summary>
        public bool OcultarAutoria
        {
            get
            {
                return mOcultarAutoria;
            }
            set
            {
                mOcultarAutoria = value;
            }
        }

        /// <summary>
        /// Indica si debe mostrarse la fecha de la ficha del recurso.
        /// </summary>
        public bool MostrarFechaRec
        {
            get
            {
                return mMostrarFechaRec;
            }
            set
            {
                mMostrarFechaRec = value;
            }
        }

        /// <summary>
        /// Indica si debe mostrarse el menú del recurso abajo.
        /// </summary>
        public bool? MenuDocumentoAbajo
        {
            get
            {
                return mMenuDocumentoAbajo;
            }
            set
            {
                mMenuDocumentoAbajo = value;
            }
        }

        /// <summary>
        /// Categorías del tesauro por defecto para los documentos de esta plantilla.
        /// </summary>
        public List<string> CategoriasPorDefecto
        {
            get
            {
                return mCategoriasPorDefecto;
            }
            set
            {
                mCategoriasPorDefecto = value;
            }
        }

        /// <summary>
        /// Indica si hay que ocultar el Tesauro
        /// </summary>
        public bool OcultarTesauro
        {
            get
            {
                return mOcultarTesauro;
            }
            set
            {
                mOcultarTesauro = value;
            }
        }
        

        /// <summary>
        /// Indica si se debe incluir el icono de GNOSS o no.
        /// </summary>
        public bool IncluirIconoGnoss
        {
            get
            {
                return mIncluirIconoGnoss;
            }
            set
            {
                mIncluirIconoGnoss = value;
            }
        }

        /// <summary>
        /// Indica si el CKEditor de comentario debe ser completo o no.
        /// </summary>
        public bool CKEditorComentariosCompleto
        {
            get
            {
                return mCKEditorComentariosCompleto;
            }
            set
            {
                mCKEditorComentariosCompleto = value;
            }
        }

        /// <summary>
        /// Nombre de los grupos de editores que deben serlo.
        /// </summary>
        public Dictionary<string, List<string>> GruposEditoresFijos
        {
            get
            {
                return mGruposEditoresFijos;
            }
            set
            {
                mGruposEditoresFijos = value;
            }
        }

        /// <summary>
        /// Nombre de los grupos de editores que deben serlo.
        /// </summary>
        public Dictionary<string, List<string>> GruposEditoresPrivacidad
        {
            get
            {
                return mGruposEditoresPrivacidad;
            }
            set
            {
                mGruposEditoresPrivacidad = value;
            }
        }

        /// <summary>
        /// Tipo de visibilidad de la edición del recurso.
        /// </summary>
        public string TipoVisiblidadEdicionRec
        {
            get
            {
                return mTipoVisiblidadEdicionRec;
            }
            set
            {
                mTipoVisiblidadEdicionRec = value;
            }
        }

        /// <summary>
        /// Nombre de los grupos de lectores que deben serlo.
        /// </summary>
        public Dictionary<string, List<string>> GruposLectoresFijos
        {
            get
            {
                return mGruposLectoresFijos;
            }
            set
            {
                mGruposLectoresFijos = value;
            }
        }

        /// <summary>
        /// Indica si hay que usar el HTML nuevo o no.
        /// </summary>
        public short HtmlNuevo
        {
            get
            {
                return mHtmlNuevo;
            }
            set
            {
                mHtmlNuevo = value;
            }
        }

        /// <summary>
        /// Indica si no es obligatorio categorizar sobre el tesauro de GNOSS.
        /// </summary>
        public bool CategorizacionTesauroGnossNoObligatoria
        {
            get
            {
                return mCategorizacionTesauroGnossNoObligatoria;
            }
            set
            {
                mCategorizacionTesauroGnossNoObligatoria = value;
            }
        }

        /// <summary>
        /// Indica si no es obligatorio etiquetar un recurso semantico.
        /// </summary>
        public bool EtiquetacionGnossNoObligatoria
        {
            get
            {
                return mEtiquetacionGnossNoObligatoria;
            }
            set
            {
                mEtiquetacionGnossNoObligatoria = value;
            }
        }

        /// <summary>
        /// Nombre de la propiedad que es archivo para la carga masiva. 
        /// </summary>
        public KeyValuePair<string, string> PropiedadArchivoCargaMasiva
        {
            get
            {
                return mPropiedadArchivoCargaMasiva;
            }
            set
            {
                mPropiedadArchivoCargaMasiva = value;
            }
        }

        /// <summary>
        /// Diccionario con listas de diccionarios con las propiedades de las etiquetas HTML meta de la ontología por idioma.
        /// </summary>
        public Dictionary<string, List<Dictionary<string, string>>> MetasHTMLOntologia
        {
            get
            {
                return mMetasHTMLOntologia;
            }
            set
            {
                mMetasHTMLOntologia = value;
            }
        }

        /// <summary>
        /// Indica si el formulario es multiidioma.
        /// </summary>
        public bool MultiIdioma
        {
            get
            {
                return mMultiIdioma;
            }
            set
            {
                mMultiIdioma = value;
            }
        }

        /// <summary>
        /// Indexado para los bots.
        /// </summary>
        public string IndexRobots
        {
            get
            {
                return mIndexRobots;
            }
            set
            {
                mIndexRobots = value;
            }
        }

        /// <summary>
        /// Titulo para los que no son miembros de la comunidad.
        /// </summary>
        public Dictionary<string, string> TituloSoloMiembros
        {
            get
            {
                if (mTituloSoloMiembros == null)
                {
                    mTituloSoloMiembros = new Dictionary<string, string>();
                }

                return mTituloSoloMiembros;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse el publicador del recurso.
        /// </summary>
        public bool OcultarPublicadorDoc
        {
            get
            {
                return mOcultarPublicadorDoc;
            }
            set
            {
                mOcultarPublicadorDoc = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse utils del recurso.
        /// </summary>
        public bool OcultarUtilsDoc
        {
            get
            {
                return mOcultarUtilsDoc;
            }
            set
            {
                mOcultarUtilsDoc = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse las acciones del recurso.
        /// </summary>
        public bool OcultarAccionesDoc
        {
            get
            {
                return mOcultarAccionesDoc;
            }
            set
            {
                mOcultarAccionesDoc = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse las categorías del recurso.
        /// </summary>
        public bool OcultarCategoriasDoc
        {
            get
            {
                return mOcultarCategoriasDoc;
            }
            set
            {
                mOcultarCategoriasDoc = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse las etiquetas del recurso.
        /// </summary>
        public bool OcultarEtiquetasDoc
        {
            get
            {
                return mOcultarEtiquetasDoc;
            }
            set
            {
                mOcultarEtiquetasDoc = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse los editores del recurso.
        /// </summary>
        public bool OcultarEditoresDoc
        {
            get
            {
                return mOcultarEditoresDoc;
            }
            set
            {
                mOcultarEditoresDoc = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse los autores del recurso.
        /// </summary>
        public bool OcultarAutoresDoc
        {
            get
            {
                return mOcultarAutoresDoc;
            }
            set
            {
                mOcultarAutoresDoc = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse las visitas del recurso.
        /// </summary>
        public bool OcultarVisitasDoc
        {
            get
            {
                return mOcultarVisitasDoc;
            }
            set
            {
                mOcultarVisitasDoc = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse los votos del recurso.
        /// </summary>
        public bool OcultarVotosDoc
        {
            get
            {
                return mOcultarVotosDoc;
            }
            set
            {
                mOcultarVotosDoc = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse el compartido en del recurso.
        /// </summary>
        public bool OcultarCompartidoDoc
        {
            get
            {
                return mOcultarCompartidoDoc;
            }
            set
            {
                mOcultarCompartidoDoc = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse el compartido en del recurso.
        /// </summary>
        public bool OcultarCompartidoEnDoc
        {
            get
            {
                return mOcultarCompartidoEnDoc;
            }
            set
            {
                mOcultarCompartidoEnDoc = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse los votos del recurso.
        /// </summary>
        public bool OcultarLicenciaDoc
        {
            get
            {
                return mOcultarLicenciaDoc;
            }
            set
            {
                mOcultarLicenciaDoc = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse la versión del recurso.
        /// </summary>
        public bool OcultarVersionDoc
        {
            get
            {
                return mOcultarVersionDoc;
            }
            set
            {
                mOcultarVersionDoc = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse el botón editar del recurso.
        /// </summary>
        public bool OcultarBotonEditarDoc
        {
            get
            {
                return mOcultarBotonEditarDoc;
            }
            set
            {
                mOcultarBotonEditarDoc = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse el botón crear versión del recurso.
        /// </summary>
        public bool OcultarBotonCrearVersionDoc
        {
            get
            {
                return mOcultarBotonCrearVersionDoc;
            }
            set
            {
                mOcultarBotonCrearVersionDoc = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse el botón enviar enlace del recurso.
        /// </summary>
        public bool OcultarBotonEnviarEnlaceDoc
        {
            get
            {
                return mOcultarBotonEnviarEnlaceDoc;
            }
            set
            {
                mOcultarBotonEnviarEnlaceDoc = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse el botón vincular del recurso.
        /// </summary>
        public bool OcultarBotonVincularDoc
        {
            get
            {
                return mOcultarBotonVincularDoc;
            }
            set
            {
                mOcultarBotonVincularDoc = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse el botón eliminar del recurso.
        /// </summary>
        public bool OcultarBotonEliminarDoc
        {
            get
            {
                return mOcultarBotonEliminarDoc;
            }
            set
            {
                mOcultarBotonEliminarDoc = value;
            }
        }

        /// <summary>
        /// Devuelve la condición que debe evaluarse para ocultar el botón eliminar del recurso.
        /// </summary>
        public string OcultarBotonEliminarDocCondicion
        {
            get
            {
                return mOcultarBotonEliminarDocCondicion;
            }
            set
            {
                mOcultarBotonEliminarDocCondicion = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse el botón restaurar versión del recurso.
        /// </summary>
        public bool OcultarBotonRestaurarVersionDoc
        {
            get
            {
                return mOcultarBotonRestaurarVersionDoc;
            }
            set
            {
                mOcultarBotonRestaurarVersionDoc = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse el botón agregar categoría del recurso.
        /// </summary>
        public bool OcultarBotonAgregarCategoriaDoc
        {
            get
            {
                return mOcultarBotonAgregarCategoriaDoc;
            }
            set
            {
                mOcultarBotonAgregarCategoriaDoc = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse el botón agregar etiquetas del recurso.
        /// </summary>
        public bool OcultarBotonAgregarEtiquetasDoc
        {
            get
            {
                return mOcultarBotonAgregarEtiquetasDoc;
            }
            set
            {
                mOcultarBotonAgregarEtiquetasDoc = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse el botón historial del recurso.
        /// </summary>
        public bool OcultarBotonHistorialDoc
        {
            get
            {
                return mOcultarBotonHistorialDoc;
            }
            set
            {
                mOcultarBotonHistorialDoc = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse el botón bloquear comentarios del recurso.
        /// </summary>
        public bool OcultarBotonBloquearComentariosDoc
        {
            get
            {
                return mOcultarBotonBloquearComentariosDoc;
            }
            set
            {
                mOcultarBotonBloquearComentariosDoc = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse el botón certificar del recurso.
        /// </summary>
        public bool OcultarBotonCertificarDoc
        {
            get
            {
                return mOcultarBotonCertificarDoc;
            }
            set
            {
                mOcultarBotonCertificarDoc = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse el botón añadir a mi espacio personal.
        /// </summary>
        public bool OcultarCompartirEspecioPersonal
        {
            get
            {
                return mOcultarCompartirEspecioPersonal;
            }
            set
            {
                mOcultarCompartirEspecioPersonal = value;
            }
        }

        /// <summary>
        /// Indica si deben ocultarse los comentarios del recurso.
        /// </summary>
        public bool OcultarComentarios
        {
            get
            {
                return mOcultarComentarios;
            }
            set
            {
                mOcultarComentarios = value;
            }
        }

        /// <summary>
        /// Indica si debe ocultarse el bloque de privacidad y seguridad en la edición del recurso
        /// </summary>
        public bool OcultarBloquePrivacidadSeguridadEdicion
        {
            get
            {
                return mOcultarBloquePrivacidadSeguridadEdicion;
            }
            set
            {
                mOcultarBloquePrivacidadSeguridadEdicion = value;
            }
        }

        /// <summary>
        /// Indica si se debe ocultar el bloque de compartir en la edición del recurso
        /// </summary>
        public bool OcultarBloqueCompartirEdicion
        {
            get
            {
                return mOcultarBloqueCompartirEdicion;
            }
            set
            {
                mOcultarBloqueCompartirEdicion = value;
            }
        }

        /// <summary>
        /// Indica si se debe ocultar el bloque de propiedad intelectual en la edición del recurso
        /// </summary>
        public bool OcultarBloquePropiedadIntelectualEdicion
        {
            get
            {
                return mOcultarBloquePropiedadIntelectualEdicion;
            }
            set
            {
                mOcultarBloquePropiedadIntelectualEdicion = value;
            }
        }

        /// <summary>
        /// Indica si hay entidades seleccionables.
        /// </summary>
        public bool HayEntidadesSelecc
        {
            get
            {
                return mHayEntidadesSelecc;
            }
            set
            {
                mHayEntidadesSelecc = value;
            }
        }

        /// <summary>
        /// Indica si hay entidades seleccionables editables desde el propio recurso.
        /// </summary>
        public bool HayEntidadesSeleccEditables
        {
            get
            {
                return mHayEntidadesSeleccEditables;
            }
            set
            {
                mHayEntidadesSeleccEditables = value;
            }
        }

        /// <summary>
        /// Indica si hay valores grafodependientes.
        /// </summary>
        public bool HayValoresGrafoDependienets
        {
            get
            {
                return mHayValoresGrafoDependienets;
            }
            set
            {
                mHayValoresGrafoDependienets = value;
            }
        }

        /// <summary>
        /// Propiedades repetidas para que se duplique el campo.
        /// </summary>
        public Dictionary<KeyValuePair<string,string>, List<string>> PropsRepetidas
        {
            get
            {
                return mPropsRepetidas;
            }
            set
            {
                mPropsRepetidas = value;
            }
        }

        /// <summary>
        /// Propiedades que son Tesauros Semánticos.
        /// </summary>
        public List<KeyValuePair<string, string>> PropiedadesTesSem
        {
            get
            {
                if (mPropiedadesTesSem == null)
                {
                    mPropiedadesTesSem = new List<KeyValuePair<string, string>>();
                }

                return mPropiedadesTesSem;
            }
            set
            {
                mPropiedadesTesSem = value;
            }
        }

        /// <summary>
        /// Devuelve o establece si hay jcrop en la plantilla
        /// </summary>
        public bool HayJcrop
        {
            get
            {
                return mHayJcrop;
            }
            set
            {
                mHayJcrop = value;
            }
        }

        /// <summary>
        /// Indica si hay una propiedad de fecha con hora.
        /// </summary>
        public bool HayFechaConHora
        {
            get
            {
                return mHayFechaConHora;
            }
            set
            {
                mHayFechaConHora = value;
            }
        }

        /// <summary>
        /// Propiedades que debe comprobarse si están ya introduccidas en la ontología o en la comunidad.
        /// </summary>
        public Dictionary<KeyValuePair<string, string>, KeyValuePair<bool, short>> PropsComprobarRepeticion
        {
            get
            {
                if (mPropsComprobarRepeticion == null)
                {
                    mPropsComprobarRepeticion = new Dictionary<KeyValuePair<string, string>, KeyValuePair<bool, short>>();
                }

                return mPropsComprobarRepeticion;
            }
            set
            {
                mPropsComprobarRepeticion = value;
            }
        }

        public List<Guid> PrivadoParaGrupoEditores
        {
            get
            {
                return mPrivadoParaGrupoEditores;
            }
            set
            {
                mPrivadoParaGrupoEditores = value;
            }
        }

        /// <summary>
        /// Lista con las Propiedad,Entidad de las que dependen la lista de Propiedad,Entidad.
        /// </summary>
        public Dictionary<KeyValuePair<string, string>, List<KeyValuePair<string, string>>> PropsSelecEntDependientes
        {
            get
            {
                return mPropsSelecEntDependientes;
            }
            set
            {
                mPropsSelecEntDependientes = value;
            }
        }

        /// <summary>
        /// Lista con los grafos simples para autocompletar.
        /// </summary>
        public List<string> GrafosSimplesAutocompletar
        {
            get
            {
                if (mGrafosSimplesAutocompletar == null)
                {
                    mGrafosSimplesAutocompletar = new List<string>();
                }

                return mGrafosSimplesAutocompletar;
            }
            set
            {
                mGrafosSimplesAutocompletar = value;
            }
        }

        /// <summary>
        /// Diccionario con las propiedades que puede tener la ontología
        /// </summary>
        public Dictionary<string, string> PropiedadesOntologia
        {
            get
            {
                if (mPropiedadesOntologia == null)
                {
                    mPropiedadesOntologia = new Dictionary<string, string>();
                }

                return mPropiedadesOntologia;
            }
            set
            {
                mPropiedadesOntologia = value;
            }
        }

        /// <summary>
        /// Condiciones de la plantilla.
        /// </summary>
        public Dictionary<string, CondicionSemCms> Condiciones
        {
            get
            {
                return mCondiciones;
            }
            set
            {
                mCondiciones = value;
            }
        }

        /// <summary>
        /// Acciones de la plantilla.
        /// </summary>
        public Dictionary<string, AccionSemCms> Acciones
        {
            get
            {
                return mAcciones;
            }
            set
            {
                mAcciones = value;
            }
        }

        /// <summary>
        /// Indica si hay configurado algún campo orden en alguna entidad auxiliar.
        /// </summary>
        public bool HayCampoOrden
        {
            get
            {
                return mHayCampoOrden;
            }
            set
            {
                mHayCampoOrden = value;
            }
        }

        #endregion

        #region Métodos

        #region Imágenes

        /// <summary>
        /// Obtiene los valores de los anchos de los tamaños de la imagen mini para la vista previa de una propiedad.
        /// </summary>
        /// <param name="pPropiedad">Propiedad</param>
        /// <param name="pEntidad">Entidad</param>
        /// <returns>Valores de los anchos de los tamaños de la imagen mini para la vista previa de una propiedad</returns>
        public string ObtenerTamaniosTextoPropiedadImagenMini(string pPropiedad, string pEntidad)
        {
            ImagenMini imagenMini = ObtenerTamanioPropiedadImagenMini(pPropiedad, pEntidad);
            string tamaniosTexto = "";

            if (imagenMini == null)
            {
                throw new Exception("La imagen principal debe tener al menos una miniatura configurada");
            }
            
            foreach (int key in imagenMini.Tamanios.Keys)
            {
                if (key > 0)
                {
                    tamaniosTexto += key.ToString() + ",";
                }
                else
                {
                    tamaniosTexto += imagenMini.Tamanios[key].ToString() + ",";
                }
            }

            return tamaniosTexto;
        }

        /// <summary>
        /// Comprueba si la propiedad es imagen mini.
        /// </summary>
        /// <param name="pPropiedad">Propiedad</param>
        /// <param name="pEntidad">Entidad</param>
        /// <returns>TRUE si la propiedad es imagen mini, FALSE en caso contrario</returns>
        public bool EsPropiedadImagenMini(string pPropiedad, string pEntidad)
        {
            foreach (EstiloPlantilla estilo in mOntologia.EstilosPlantilla[pPropiedad])
            {
                if (((EstiloPlantillaEspecifProp)estilo).NombreEntidad == pEntidad)
                {
                    return (((EstiloPlantillaEspecifProp)estilo).ImagenMini != null);
                }
            }

            return false;
        }

        /// <summary>
        /// Comprueba si la propiedad es UsarJcrop
        /// </summary>
        /// <param name="pPropiedad">Propiedad</param>
        /// <param name="pEntidad">Entidad</param>
        /// <returns>TRUE si la propiedad es UsarJcrop, FALSE en caso contrario</returns>
        public bool EsPropiedadUsarJcrop(string pPropiedad, string pEntidad)
        {
            pEntidad = ElementoOntologia.ObtenerTiposEntidadLimpiaDeApanioRepeticiones(pEntidad);

            foreach (EstiloPlantilla estilo in mOntologia.EstilosPlantilla[pPropiedad])
            {
                if (((EstiloPlantillaEspecifProp)estilo).NombreEntidad == pEntidad)
                {
                    return (((EstiloPlantillaEspecifProp)estilo).UsarJcrop);
                }
            }

            return false;
        }

        /// <summary>
        /// Obtiene el valor del tamaño de la imagen mini para la vista previa de una propiedad.
        /// </summary>
        /// <param name="pPropiedad">Propiedad</param>
        /// <param name="pEntidad">Entidad</param>
        /// <returns>Valor del tamaño de la imagen para la vista previa de una propiedad</returns>
        public ImagenMini ObtenerTamanioPropiedadImagenMini(string pPropiedad, string pEntidad)
        {
            foreach (EstiloPlantilla estilo in mOntologia.EstilosPlantilla[pPropiedad])
            {
                if (((EstiloPlantillaEspecifProp)estilo).NombreEntidad == pEntidad)
                {
                    return ((EstiloPlantillaEspecifProp)estilo).ImagenMini;
                }
            }

            return null;
        }

        #endregion

        /// <summary>
        /// Obtiene las entidades principales ordenadas según el archivo de configuración.
        /// </summary>
        /// <param name="pEntidades">Entidades principales sin ordenar</param>
        /// <returns>Lista ordenada</returns>
        public List<ElementoOntologia> ObtenerEntidadesPrincipalesOrdenadas(List<ElementoOntologia> pEntidades)
        {
            if (mOntologia.EstilosPlantilla.ContainsKey("DocumentoGnoss"))
            {
                foreach (EstiloPlantilla estilo in mOntologia.EstilosPlantilla["DocumentoGnoss"])
                {
                    if (estilo is EstiloPlantillaEspecifEntidad)
                    {
                        List<ElementoOntologia> listaOrdenadaDevolver = new List<ElementoOntologia>();

                        foreach (ElementoOrdenado elemOrd in ((EstiloPlantillaEspecifEntidad)estilo).ElementosOrdenados)
                        {
                            if (!elemOrd.EsGrupo)
                            {
                                foreach (ElementoOntologia entidad in pEntidades)
                                {
                                    if (entidad.TipoEntidad == elemOrd.NombrePropiedad.Key)
                                    {
                                        listaOrdenadaDevolver.Add(entidad);
                                        break;
                                    }
                                }
                            }
                        }

                        foreach (ElementoOntologia entidad in pEntidades)
                        {
                            if (!listaOrdenadaDevolver.Contains(entidad))
                            {
                                listaOrdenadaDevolver.Add(entidad);
                            }
                        }

                        return listaOrdenadaDevolver;
                    }
                }
            }

            return pEntidades;
        }

        /// <summary>
        /// Obtiene el título para no miembros a partir del idioma.
        /// </summary>
        /// <param name="pIdioma">Idioma</param>
        /// <returns>Título para no miembros a partir del idioma</returns>
        public string ObtenerTituloSoloMiembros(string pIdioma)
        {
            if (TituloSoloMiembros.ContainsKey(pIdioma))
            {
                return TituloSoloMiembros[pIdioma];
            }
            else if (TituloSoloMiembros.ContainsKey("es"))
            {
                return TituloSoloMiembros["es"];
            }

            return null;
        }

        #endregion

        #region Miembros de ISerializable

        /// <summary>
        /// Método para serializar el objeto
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Acciones", mAcciones);
            info.AddValue("CategoriasPorDefecto", mCategoriasPorDefecto);
            info.AddValue("CategorizacionTesauroGnossNoObligatoria", mCategorizacionTesauroGnossNoObligatoria);
            info.AddValue("EtiquetacionGnossNoObligatoria", mEtiquetacionGnossNoObligatoria);
            info.AddValue("CKEditorComentariosCompleto", mCKEditorComentariosCompleto);
            info.AddValue("Condiciones", mCondiciones);
            info.AddValue("GrafosSimplesAutocompletar", mGrafosSimplesAutocompletar);
            info.AddValue("GruposEditoresFijos", mGruposEditoresFijos);
            info.AddValue("GruposEditoresPrivacidad", mGruposEditoresPrivacidad);
            info.AddValue("GruposLectoresFijos", mGruposLectoresFijos);
            info.AddValue("HayCampoOrden", mHayCampoOrden);
            info.AddValue("HayEntidadesSelecc", mHayEntidadesSelecc);
            info.AddValue("HayEntidadesSeleccEditables", mHayEntidadesSeleccEditables);
            info.AddValue("HayFechaConHora", mHayFechaConHora);
            info.AddValue("HayJcrop", mHayJcrop);
            info.AddValue("HayValoresGrafoDependienets", mHayValoresGrafoDependienets);
            info.AddValue("HtmlNuevo", mHtmlNuevo);
            info.AddValue("IncluirIconoGnoss", mIncluirIconoGnoss);
            info.AddValue("IndexRobots", mIndexRobots);
            info.AddValue("ListaIdiomas", mListaIdiomas);
            info.AddValue("MenuDocumentoAbajo", mMenuDocumentoAbajo);
            info.AddValue("MetasHTMLOntologia", mMetasHTMLOntologia);
            info.AddValue("MostrarFechaRec", mMostrarFechaRec);
            info.AddValue("MultiIdioma", mMultiIdioma);
            info.AddValue("Namespace", mNamespace);
            info.AddValue("OrdenAutocompletar", mOrdenAutocompletar);
            info.AddValue("OrdenAutocompletarIsAsc", mOrdenAutocompletarIsAsc);
            info.AddValue("OcultarAccionesDoc", mOcultarAccionesDoc);
            info.AddValue("OcultarAutoresDoc", mOcultarAutoresDoc);
            info.AddValue("OcultarAutoria", mOcultarAutoria);
            info.AddValue("OcultarBloqueCompartirEdicion", mOcultarBloqueCompartirEdicion);
            info.AddValue("OcultarBloquePrivacidadSeguridadEdicion", mOcultarBloquePrivacidadSeguridadEdicion);
            info.AddValue("OcultarBloquePropiedadIntelectualEdicion", mOcultarBloquePropiedadIntelectualEdicion);
            info.AddValue("OcultarBotonAgregarCategoriaDoc", mOcultarBotonAgregarCategoriaDoc);
            info.AddValue("OcultarBotonAgregarEtiquetasDoc", mOcultarBotonAgregarEtiquetasDoc);
            info.AddValue("OcultarBotonBloquearComentariosDoc", mOcultarBotonBloquearComentariosDoc);
            info.AddValue("OcultarBotonCertificarDoc", mOcultarBotonCertificarDoc);
            info.AddValue("OcultarBotonCrearVersionDoc", mOcultarBotonCrearVersionDoc);
            info.AddValue("OcultarBotonEditarDoc", mOcultarBotonEditarDoc);
            info.AddValue("OcultarBotonEliminarDoc", mOcultarBotonEliminarDoc);
            info.AddValue("OcultarBotonEliminarDocCondicion", mOcultarBotonEliminarDocCondicion);
            info.AddValue("OcultarBotonEnviarEnlaceDoc", mOcultarBotonEnviarEnlaceDoc);
            info.AddValue("OcultarBotonHistorialDoc", mOcultarBotonHistorialDoc);
            info.AddValue("OcultarBotonRestaurarVersionDoc", mOcultarBotonRestaurarVersionDoc);
            info.AddValue("OcultarBotonVincularDoc", mOcultarBotonVincularDoc);
            info.AddValue("OcultarCategoriasDoc", mOcultarCategoriasDoc);
            info.AddValue("OcultarComentarios", mOcultarComentarios);
            info.AddValue("OcultarCompartidoDoc", mOcultarCompartidoDoc);
            info.AddValue("OcultarCompartidoEnDoc", mOcultarCompartidoEnDoc);
            info.AddValue("OcultarCompartirEspecioPersonal", mOcultarCompartirEspecioPersonal);
            info.AddValue("OcultarEditoresDoc", mOcultarEditoresDoc);
            info.AddValue("OcultarEtiquetasDoc", mOcultarEtiquetasDoc);
            info.AddValue("OcultarFechaRec", mOcultarFechaRec);
            info.AddValue("OcultarLicenciaDoc", mOcultarLicenciaDoc);
            info.AddValue("OcultarPublicadorDoc", mOcultarPublicadorDoc);
            info.AddValue("OcultarRecursoAUsuarioInvitado", mOcultarRecursoAUsuarioInvitado);
            info.AddValue("OcultarTesauro", mOcultarTesauro);
            info.AddValue("OcultarTituloDescpEImg", mOcultarTituloDescpEImg);
            info.AddValue("OcultarUtilsDoc", mOcultarUtilsDoc);
            info.AddValue("OcultarVersionDoc", mOcultarVersionDoc);
            info.AddValue("OcultarVisitasDoc", mOcultarVisitasDoc);
            info.AddValue("OcultarVotosDoc", mOcultarVotosDoc);
            info.AddValue("PrivadoParaGrupoEditores", mPrivadoParaGrupoEditores);
            info.AddValue("PropiedadArchivoCargaMasiva", mPropiedadArchivoCargaMasiva);
            info.AddValue("PropiedadDescripcion", mPropiedadDescripcion);
            info.AddValue("PropiedadesOntologia", mPropiedadesOntologia);
            info.AddValue("PropiedadesOpenSeaDragon", mPropiedadesOpenSeaDragon);
            info.AddValue("PropiedadesTesSem", mPropiedadesTesSem);
            info.AddValue("PropiedadesTitulo", mPropiedadesTitulo);
            info.AddValue("PropiedadImagenFromURL", mPropiedadImagenFromURL);
            info.AddValue("PropiedadImagenRepre", mPropiedadImagenRepre);
            info.AddValue("PropiedadTitulo", mPropiedadTitulo);
            info.AddValue("PropsComprobarRepeticion", mPropsComprobarRepeticion);
            info.AddValue("PropsRepetidas", mPropsRepetidas);
            info.AddValue("PropsSelecEntDependientes", mPropsSelecEntDependientes);
            info.AddValue("TipoVisiblidadEdicionRec", mTipoVisiblidadEdicionRec);
            info.AddValue("TituloSoloMiembros", mTituloSoloMiembros);
        }

        #endregion
    }

    /// <summary>
    /// Representa una condición para evaluar en el SEM CMS.
    /// </summary>
    [Serializable]
    public class CondicionSemCms
    {
        #region Propiedades

        /// <summary>
        /// ID de la condición.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Variables a partir de las propiedades. Key = Nombre de la variable. Value = Propiedades con entidad separadas por coma y separado todo ello por | si hay jerarquia de propiedades.
        /// </summary>
        public Dictionary<string, string> VariablesProp { get; set; }

        /// <summary>
        /// Clausula para que se cumplan las condiciones.
        /// </summary>
        public ClausulaSemCms Clausula { get; set; }

        #endregion

        /// <summary>
        /// Clausula para la condición.
        /// </summary>
        [Serializable]
        public class ClausulaSemCms
        {
            /// <summary>
            /// Tipo de clausula. Puede ser: 'Or', 'And', 'Igual', 'Distinto', 'PerteneceAGrupo'.
            /// </summary>
            public string Tipo { get; set; }

            /// <summary>
            /// Clausulas hijas de la clausula actual.
            /// </summary>
            public List<ClausulaSemCms> Clausulas { get; set; }

            public List<string> Valores { get; set; }
        }
    }

    /// <summary>
    /// Representa una acción para evaluar en el SEM CMS.
    /// </summary>
    [Serializable]
    public class AccionSemCms
    {
        #region Propiedades

        /// <summary>
        /// ID de la Acción.
        /// </summary>
        public string ID { get; set; }

        public List<Accion> Acciones { get; set; }

        #endregion

        /// <summary>
        /// Acción interna de una acción del SEMCMS.
        /// </summary>
        [Serializable]
        public class Accion
        {
            public TipoAccion TipoAccion { get; set; }

            #region Servicio Externo

            /// <summary>
            /// Url del servicio externo.
            /// </summary>
            public string UrlServExterno { get; set; }

            #endregion
        }

        /// <summary>
        /// Tipo de acción interna de una acción del SEMCMS.
        /// </summary>
        [Serializable]
        public enum TipoAccion
        {
            /// <summary>
            /// Acción de enviar un documento a un servicio externo.
            /// </summary>
            EnviarServExterno=0
        }
    }
}
