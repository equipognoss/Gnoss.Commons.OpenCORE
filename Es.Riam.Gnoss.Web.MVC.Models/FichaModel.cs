using Es.Riam.Gnoss.Web.MVC.Models.Flujos;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models
{
    /// <summary>
    /// Enumeración para distinguir tipos de documentación
    /// </summary>
    public enum TiposDocumentacion
    {
        /// <summary>
        /// Hipervínculo
        /// </summary>
        Hipervinculo = 0,
        /// <summary>
        /// Referencia a documento
        /// </summary>
        ReferenciaADoc = 1,
        /// <summary>
        /// Vídeo
        /// </summary>
        Video = 2,
        /// <summary>
        /// Fichero de servidor
        /// </summary>
        FicheroServidor = 3,
        /// <summary>
        /// Imagen wiki
        /// </summary>
        ImagenWiki = 4,
        /// <summary>
        /// Documento semántico
        /// </summary>
        Semantico = 5,
        /// <summary>
        /// Imagen
        /// </summary>
        Imagen = 6,
        /// <summary>
        /// Ontología
        /// </summary>
        Ontologia = 7,
        /// <summary>
        /// Nota
        /// </summary>
        Nota = 8,
        /// <summary>
        /// Artículo de wiki
        /// </summary>
        Wiki = 9,
        /// <summary>
        /// Entrada de blog
        /// </summary>
        EntradaBlog = 10,
        /// <summary>
        /// Newsletter
        /// </summary>
        Newsletter = 11,
        /// <summary>
        /// Artículo de wiki temporal
        /// </summary>
        WikiTemporal = 12,
        /// <summary>
        /// Entrada de blog temporal
        /// </summary>
        EntradaBlogTemporal = 13,
        /// <summary>
        /// Se utiliza para controlar los permisos de usuarios al agregar dafos a comunidades
        /// </summary>
        DafoProyecto = 14,
        /// <summary>
        /// Preguntas
        /// </summary>
        Pregunta = 15,
        /// <summary>
        /// Debates
        /// </summary>
        Debate = 16,
        /// <summary>
        /// Blogs
        /// </summary>
        Blog = 17,
        /// <summary>
        /// Encuestas
        /// </summary>
        Encuesta = 18,
        /// <summary>
        /// Videos de Brightcove
        /// </summary>
        VideoBrightcove = 19,
        /// <summary>
        /// Audios de Brightcove
        /// </summary>
        AudioBrightcove = 20,
        /// <summary>
        /// Audio
        /// </summary>
        Audio = 21,
        /// <summary>
        /// CargasMasivas
        /// </summary>
        CargasMasivas = 22,
        /// <summary>
        /// Ontología secundaria.
        /// </summary>
        OntologiaSecundaria = 23,
        /// <summary>
        /// Videos de Top
        /// </summary>
        VideoTOP = 24,
        /// <summary>
        /// Audios de Top
        /// </summary>
        AudioTOP = 25
    }

    public enum EstadoVersion
    {
        Pendiente = 1,
        Vigente = 2,
        Cancelada = 3,
        Historico = 4
    }

    /// <summary>
    /// Modelo de un recurso
    /// </summary>
    [Serializable]
    public partial class ResourceModel : ObjetoBuscadorModel
    {
        public const int LastCacheVersion = 1;

        /// <summary>
        /// Tipo de publicacion
        /// </summary>
        public enum PublicationType
        {
            /// <summary>
            /// Publicado
            /// </summary>
            Published = 0,
            /// <summary>
            /// Compartido
            /// </summary>
            Shared = 1,
            /// <summary>
            /// Compartido automaticamente
            /// </summary>
            SharedAutomatic = 2
        }

        /// <summary>
        /// Tipo de documento
        /// </summary>
        public enum DocumentType
        {
            Hipervinculo = 0,
            ReferenciaADoc = 1,
            Video = 2,
            FicheroServidor = 3,
            ImagenWiki = 4,
            Semantico = 5,
            Imagen = 6,
            Ontologia = 7,
            Nota = 8,
            Wiki = 9,
            EntradaBlog = 10,
            Newsletter = 11,
            WikiTemporal = 12,
            EntradaBlogTemporal = 13,
            DafoProyecto = 14,
            Pregunta = 15,
            Debate = 16,
            Blog = 17,
            Encuesta = 18,
            VideoBrightcove = 19,
            AudioBrightcove = 20,
            Audio = 21,
            CargasMasivas = 22,
            OntologiaSecundaria = 23,
            VideoTOP = 24,
            AudioTOP = 25
        }

        /// <summary>
        /// Identificador del recurso
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Si es un documento versionado es la clave del documento original
        /// </summary>
        public Guid OriginalKey { get; set; }

        /// <summary>
        /// Título del recurso
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Descripción del recurso
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Fecha de publicación del recurso
        /// </summary>
        public DateTime PublishDate { get; set; }

        /// <summary>
        /// Fecha de modificación del recurso
        /// </summary>
        public DateTime ModificationDate { get; set; }
        /// <summary>
        /// Url a la ficha completa del recurso
        /// </summary>
        public string CompletCardLink { get; set; }

        /// <summary>
        // Url a la ficha completa del recurso con el id original
        /// </summary>
        public string CompleteOriginalCardLink { get; set; }

        /// <summary>
        /// Url a la version de la ficha completa del recurso
        /// </summary>
        public string VersionCardLink { get; set; }

        /// <summary>
        /// Url a la edicion del recrso
        /// </summary>
        public string EditCardLink { get; set; }

        /// <summary>
        /// Url para crear una nueva versión del recurso
        /// </summary>
        public string UrlNewVersion { get; set; }

		/// <summary>
		/// Url de descarga del recurso / url del video / url imagen / url del hipervinculo
		/// </summary>
		public string UrlDocument { get; set; }

        /// <summary>
        /// Url de la previsualizacion / url de la imagen reducida / url de la captura de un web site / url de un video
        /// </summary>
        public string UrlPreview { get; set; }

        /// <summary>
        /// Indica si el documento esta configurado para abrirse en una ventana nueva
        /// </summary>
        public bool OpenInNewWindow { get; set; }

        /// <summary>
        /// Indica si el documento es privado
        /// </summary>
        public bool Private { get; set; }

        /// <summary>
        /// Tipo de documento
        /// </summary>
        public DocumentType TypeDocument { get; set; }

        /// <summary>
        /// Nombre de la imagen que indica el tipo de recurso
        /// (Este dato deberia obtenerse en la vista)
        /// </summary>
        public string NameImage { get; set; }
        /// <summary>
        /// Enlace del documento
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Lista de redes sociales
        /// La 'Key' del 'Dictionary' es el nombre de la red social, el 'Value' es una lista de atributos del enlace que debemos construir: onclick, class, id...
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> SocialNetworks { get; set; }

        /// <summary>
        /// Identificador del perfil publicador
        /// </summary>
        public Guid PublisherKey { get; set; }

        /// <summary>
        /// Modelo de perfil del publicador
        /// </summary>
        public ProfileModel Publisher { get; set; }

        /// <summary>
        /// Lista de autores del recurso : nombre y url
        /// </summary>
        public Dictionary<string, string> Authors { get; set; }

        /// <summary>
        /// Lista de editores del recurso : nombre y url
        /// </summary>
        public Dictionary<string, string> Editors { get; set; }

        /// <summary>
        /// Lista de lectores del recurso : nombre y url
        /// </summary>
        public Dictionary<string, string> Readers { get; set; }

        /// <summary>
        /// Lista de etiquetas del recurso
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// Lista de categorias del recurso
        /// </summary>
        public List<CategoryModel> Categories { get; set; }

        /// <summary>
        /// Lista de Bases de Recursos en las que esta compartido el documento
        /// </summary>
        public List<SharedBRModel> Shareds { get; set; }

        /// <summary>
        /// Lista de Bases de Recursos en las que esta compartido el documento
        /// </summary>
        public List<SharedBRModel> Duplicates { get; set; }

        /// <summary>
        /// Augmented title of the resource
        /// </summary>
        public string AugmentedTitle { get; set; }

        /// <summary>
        /// Augmented description of the resource
        /// </summary>
        public string AugmentedDescription { get; set; }

        /// <summary>
        /// Indica si el recurso es de tipo enlace y apunta a SharePoint
        /// </summary>
        public bool EsEnlaceSharepoint { get; set; }
        
        /// <summary>
        /// Indica si el recurso de tipo enlace SharePoint coincide con el fichero al que apunta en SharePoint
        /// </summary>
        public bool EstaAlineadoConSharepoint { get; set; }

        public EstadoModel Estado { get; set; }

        /// <summary>
        /// Identificador del estado en el que esta el recurso
        /// </summary>
        public Guid? EstadoID { get; set; }

        public List<HistorialTransicionModel> HistorialTransiciones { get; set; }

        /// <summary>
        /// Modelo de una Base de Recursos. Puede ser una comunidad, un perfil o una organización
        /// </summary>
        [Serializable]
        public partial class SharedBRModel
        {
            /// <summary>
            /// Identificador de la Base de Recursos
            /// </summary>
            public Guid Key { get; set; }
            /// <summary>
            /// Nombre de la Base de Recursos
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Url de la Base de Recursos
            /// </summary>
            public string Url { get; set; }
            /// <summary>
            /// Indica si la base de recursos es privada (para poner un candado en diseño)
            /// </summary>
            public bool Private { get; set; }
            /// <summary>
            /// Identificador del proyecto en el que esta compartida
            /// </summary>
            public Guid ProyectKey { get; set; }
            /// <summary>
            /// Identificador de la organización en la que esta compartida
            /// </summary>
            public Guid OrganizationKey { get; set; }

            /// <summary>
            /// Indica si la eliminación está disponible.
            /// </summary>
            public bool DeleteAvailable { get; set; }
        }

        /// <summary>
        /// Licencia del recurso
        /// </summary>
        public string Licence { get; set; }

        /// <summary>
        /// Certificacion del recurso. Clave de certificación y Nombre
        /// </summary>
        public KeyValuePair<Guid, string> Certification { get; set; }

        /// <summary>
        /// Indica si el recurso permite votaciones
        /// </summary>
        public bool AllowVotes { get; set; }

        /// <summary>
        /// Indica si el recurso permite comentarios
        /// </summary>
        public bool AllowComments { get; set; }

        /// <summary>
        /// Almacena el metatitulo del recurso
        /// </summary>
        public string MetaTitle { get; set; }

        /// <summary>
        /// Almacena la metadescripcion del recurso
        /// </summary>
        public string MetaDescription { get; set; }

        public UrlActions ListActions { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public partial class UrlActions
        {
            /// <summary>
            /// 
            /// </summary>
            public string UrlLoadGraph { get; set; }
            /// <summary>
            /// Url para cargar más entidades de un selector de entidades del SEMCMS.
            /// </summary>
            public string UrlLoadMoreEntitiesSelector { get; set; }

            /// <summary>
            /// Url para realizar una acción desde la ficha del SEMCMS.
            /// </summary>
            public string UrlActionSemCms { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string UrlCallbackGraph { get; set; }

            /// <summary>
            /// Indica la Url de creacion de un comentario
            /// </summary>
            public string UrlCreateComment { get; set; }

            /// <summary>
            /// Indica la Url para cargar los recursos vinculados
            /// </summary>
            public string UrlLoadLinkedResources { get; set; }

            /// <summary>
            /// Indica la Url de vincular un recurso
            /// </summary>
            public string UrlLinkResource { get; set; }

            /// <summary>
            /// Indica la Url de vincular un recurso a SharePoint
            /// </summary>
            public string UrlLinkResourceSP { get; set; }

            /// <summary>
            /// Indica la Url de desvincular un recurso a SharePoint
            /// </summary>
            public string UrlUnlinkResourceSP { get; set; }

            /// <summary>
            /// Indica la Url para accionar modal de vincular con SharePoint
            /// </summary>
            public string UrlLoadActionLinkResourceSP { get; set; }

            /// <summary>
            /// Indica la Url para accionar modal de desvincular de SharePoint
            /// </summary>
            public string UrlLoadActionUnlinkResourceSP { get; set; }

            /// <summary>
            /// Indica la Url de desvincular un recurso
            /// </summary>
            public string UrlUnLinkResource { get; set; }

            /// <summary>
            /// Indica la Url de añadir meta titulo
            /// </summary>
            public string UrlAddMetaTitle { get; set; }

            /// <summary>
            /// Indica la Url de añadir meta descripcion
            /// </summary>
            public string UrlAddMetaDescripcion { get; set; }

            /// <summary>
            /// Indica la Url para guardar en el espacio personal
            /// </summary>
            public string UrlAddToPersonalSpace { get; set; }

            /// <summary>
            /// Indica la Url para guardar en el espacio personal
            /// </summary>
            public string UrlAddToPersonalSpacePrivate { get; set; }

            /// <summary>
            /// Indica la Url para agregar una categoría al espacio personal
            /// </summary>
            public string UrlAddACategoryToPersonalSpace { get; set; }

            /// <summary>
            /// Indica la Url de añadir tags a un recurso
            /// </summary>
            public string UrlAddTags { get; set; }

            /// <summary>
            /// Indica la Url de añadir categorias a un recurso
            /// </summary>
            public string UrlAddCategories { get; set; }

            /// <summary>
            /// Indica la Url de bloquear los comentarios de un recurso
            /// </summary>
            public string UrlLockComments { get; set; }

            /// <summary>
            /// Indica la Url de desbloquear los comentarios de un recurso
            /// </summary>
            public string UrlUnlockComments { get; set; }

            /// <summary>
            /// Url para restaurar la version de un recurso
            /// </summary>
            public string UrlRestoreVersion { get; set; }
            /// <summary>
            /// Url para eliminar la version de un recurso
            /// </summary>
            public string UrlDeleteVersion { get; set; }
            /// <summary>
            /// Url para iniciar una mejora
            /// </summary>
            public string UrlStartImprovement { get; set; }
			/// <summary>
			/// Url para aprobar una mejora de un recurso con versión
			/// </summary>
			public string UrlApplyImprovement { get; set; }
            /// <summary>
            /// Url para cancelar una mejora de un recurso con versión
            /// </summary>
            public string UrlCancelImprovement { get; set; }

            /// <summary>
            /// Url para reportar contenido inadecuado
            /// </summary>
            public string UrlReportPage { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string UrlShare { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string UrlDuplicate { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string UrlUnshare { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string UrlDelete { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string UrlDeleteSelective { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string UrlCertify { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string UrlSendNewsletter { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string UrlSendNewsletterGroups { get; set; }

            /// <summary>
            /// Indica la Url para cargar la accion de vincular un recurso
            /// </summary>
            public string UrlLoadActionLinkResource { get; set; }

            /// <summary>
            /// Indica la Url para cargar la accion de guardar en el espacio personal
            /// </summary>
            public string UrlLoadActionAddToPersonalSpace { get; set; }

            /// <summary>
            /// Indica la Url para cargar la accion de añadir tags a un recurso
            /// </summary>
            public string UrlLoadActionAddTags { get; set; }

            /// <summary>
            /// Indica la Url para cargar la accion de añadir categorias a un recurso
            /// </summary>
            public string UrlLoadActionAddCategories { get; set; }

            /// <summary>
            /// Indica la url para cargar la accion de restaurar una version de un recurso
            /// </summary>
            public string UrlLoadActionRestoreVersion { get; set; }
            /// <summary>
            /// Indica la url para cargar la accion de eliminar una version de un recurso
            /// </summary>
            public string UrlLoadActionDeleteVersion { get; set; }

            /// <summary>
            /// Url para mostrar el panel de reportar contenido inadecuado
            /// </summary>
            public string UrlLoadActionReportPage { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string UrlLoadActionDelete { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string UrlLoadActionDeleteSelective { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string UrlLoadActionCertify { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string UrlLoadActionShare { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string UrlLoadActionShareChange { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string UrlLoadActionDuplicate { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string UrlLoadActionSendNewsletter { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string UrlLoadActionSendNewsletterGroups { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string UrlLoadActionHistory { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string UrlLoadActionLockComments { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string UrlLoadActionSendLink { get; set; }
            public string UrlLoadActionAddMetaTitle { get; set; }
            public string UrlLoadActionAddMetaDescripcion { get; set; }
            public string UrlTransition { get; set; }
            public string UrlTransitionModal { get; set; }
            public string UrlTransitionHistory { get; set; }
			public string UrlLoadActionStartImprovement { get; set; }
            public string UrlLoadActionApplyImprovement { get; set; }
            public string UrlLoadActionCancelImprovement { get; set; }
            public string UrlImprovement { get; set; }
        }

        /// <summary>
        /// Identificador de la ultima versión del documento, puede ser él mismo.
        /// </summary>
        public Guid LastVersion { get; set; }

        /// <summary>
        /// Número de versión del documento
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Número de visitas del recurso
        /// </summary>
        public int NumVisits { get; set; }

        /// <summary>
        /// Número de votos del recurso
        /// </summary>
        public int NumVotes { get; set; }

        /// <summary>
        /// Número de comentarios del recurso
        /// </summary>
        public int NumComments { get; set; }

        /// <summary>
        /// Número de descargas del recurso
        /// </summary>
        public int NumDownloads { get; set; }

        /// <summary>
        /// Modelo de los votos
        /// </summary>
        public VotesModel Votes { get; set; }

        /// <summary>
        /// Lista de comentarios del recurso
        /// </summary>
        public List<CommentModel> Comments { get; set; }

        /// <summary>
        /// Lista de recursos relacionados
        /// </summary>
        public List<ResourceModel> RelatedResources { get; set; }

        /// <summary>
        /// Número de recursos relacionados
        /// </summary>
        public int NumRelatedResources { get; set; }

        /// <summary>
        /// Acciones permitidas en el recurso
        /// </summary>
        public ActionsModel Actions { get; set; }

        /// <summary>
        /// Acciones permitidas en el recurso
        /// </summary>
        [Serializable]
        public partial class ActionsModel
        {
            /// <summary>
            /// Indica si el el usuario puede agregar el recurso a su espacio personal
            /// </summary>
            public bool AddToMyPersonalSpace { get; set; }
            /// <summary>
            /// Indica si el usuario puede restaurar el recurso
            /// (No es la ultima versión y tienes permiso para restaurarla)
            /// </summary>
            public bool Restore { get; set; }
            /// <summary>
            /// Indica si el usuario puede agregar categorias al recurso
            /// </summary>
            public bool AddCategories { get; set; }
            /// <summary>
            /// Indica si el usuario puede agregar etiquetas al recurso
            /// </summary>
            public bool AddTags { get; set; }
            /// <summary>
            /// Indica si el usuario puede compartir el recurso
            /// </summary>
            public bool Share { get; set; }
            /// <summary>
            /// Indica si el usuario puede duplicar el recurso en otra comunidad
            /// </summary>
            public bool Duplicate { get; set; }

            /// <summary>
            /// Indica si el usuario puede editar el recurso
            /// </summary>
            public bool Edit { get; set; }

            public bool EditImprovement { get; set; }

            /// <summary>
            /// Indica si el usuario puede crear una versión del recurso
            /// </summary>
            public bool CreateVersion { get; set; }
            /// <summary>
            /// Indica si el usuario puede ver el historial del recurso
            /// </summary>
            public bool ViewHistory { get; set; }
            /// <summary>
            /// Indica si el usuario puede bloquear los comentarios del recurso
            /// </summary>
            public bool BlockComments { get; set; }
            /// <summary>
            /// Indica si el usuario puede enviar enlaces del recurso
            /// </summary>
            public bool SendLink { get; set; }
            /// <summary>
            /// Indica si el usuario puede enviar el recurso como newsletter
            /// (Solamente se puede en un recurso de tipo Newsletter)
            /// </summary>
            public bool SendNewsletter { get; set; }
            /// <summary>
            /// Indica si el usuario puederecurso como newsletter a grupos
            /// (Solamente se puede en un recurso de tipo Newsletter)
            /// </summary>
            public bool SendNewsletterGroups { get; set; }
            /// <summary>
            /// Indica si el usuario puede vincular el recurso con otro
            /// </summary>
            public bool LinkUp { get; set; }
            /// <summary>
            /// Indica si el usuario puede desvincular el recurso de otro
            /// </summary>
            public bool UnLinkUp { get; set; }
            /// <summary>
            /// Indica si el usuario puede certificar el recurso
            /// </summary>
            public bool Certify { get; set; }
            /// <summary>
            /// Indica si el usuario puede eliminar el recurso
            /// </summary>
            public bool Delete { get; set; }
            /// <summary>
            /// Indica si el usuario puede realizar una transición concreta
            /// </summary>
            public bool Transition { get; set; }
        }

        /// <summary>
        /// Tipo de publicaciónejecutar una transicion
        /// </summary>
        public PublicationType TypePublication { get; set; }

        /// <summary>
        /// Contiene datos especiales si el recurso es de tipo encuesta
        /// </summary>
        public PollModel Poll { get; set; }

        /// <summary>
        /// Modelo de una Encuesta
        /// </summary>
        [Serializable]
        public partial class PollModel
        {
            /// <summary>
            /// Indica si el usuario ya ha votado el recurso
            /// </summary>
            public bool Voted { get; set; }

            /// <summary>
            /// Indica si el usuario puede ver los resultados de la encuesta
            /// </summary>
            public bool ViewPollResults { get; set; }

            /// <summary>
            /// Lista con las posibles opciones que tiene la encuesta
            /// </summary>
            public List<PollOptionsModel> PollOptions { get; set; }

            /// <summary>
            /// Modelo de las opciones de la encuesta
            /// </summary>
            [Serializable]
            public partial class PollOptionsModel
            {
                /// <summary>
                /// Identificador de la opción
                /// </summary>
                public Guid Key { get; set; }
                /// <summary>
                /// Texto de la opción
                /// </summary>
                public string Name { get; set; }
                /// <summary>
                /// Numero de votos que ha recibido la opción
                /// </summary>
                public int NumberOfVotes { get; set; }
            }
        }

        /// <summary>
        /// Contiene datos de la configuración de las vistas de un recurso semantico en las busquedas y contextos
        /// (Si no esta configurado, el objeto es null)
        /// </summary>
        public ViewSettingResorceModel ViewSettings { get; set; }

        /// <summary>
        /// Lista de gadgets del recurso
        /// </summary>
        public List<GadgetModel> Gadgets { get; set; }

        /// <summary>
        /// Indica si el recurso esta cargado por completo 
        /// (Este parametro es para desarrollo)
        /// </summary>
        public bool FullyLoaded { get; set; }

        /// <summary>
        /// Nombre para identificar el tipo de un recurso semantico
        /// </summary>
        public string RdfType { get; set; }

        /// <summary>
        /// Nombre para mostrar el tipo de un recurso semantico
        /// </summary>
        public string RdfTypeName { get; set; }

        /// <summary>
        /// Identificador de la ontologia vinculada al recurso
        /// </summary>
        public Guid ItemLinked { get; set; }

        /// <summary>
        /// Version del javascript y css de la ontologia vinculada al recurso
        /// </summary>
        public int ItemLinkedFotoVersion { get; set; }

        /// <summary>
        /// Lista de grafos que se pintan en un recurso
        /// </summary>
        public List<GrafoRecurso> Graphs { get; set; }

        /// <summary>
        /// Modelo de Grafo de un recurso
        /// </summary>
        [Serializable]
        public partial class GrafoRecurso
        {
            /// <summary>
            /// Propiedad por la cual se enlazan los recursos
            /// </summary>
            public string PropEnlace { get; set; }
            /// <summary>
            /// Número de nodos a partir del cual no se van a hacer más peticiones
            /// </summary>
            public int NodosLimiteNivel { get; set; }
            /// <summary>
            /// Datos extra para montar el grafo
            /// </summary>
            public string Extra { get; set; }
            /// <summary>
            /// Url sobre la que se realizan las busquedas al hacer click sobre los nodos
            /// </summary>
            public string UrlBusqueda { get; set; }
            /// <summary>
            /// Url sobre la que se realizan las busquedas al hacer click sobre los nodos
            /// </summary>
            public string UrlBusquedaGrafo { get; set; }
            /// <summary>
            /// RDF type del recurso
            /// </summary>
            public string TipoRecurso { get; set; }
        }

        /// <summary>
        /// Indica que la ficha que se debe pintar es de tipo mapa.
        /// </summary>
        public bool MapView { get; set; }

        /// <summary>
        /// Indica si el check de selección está disponible.
        /// </summary>
        public bool SelectionCheckAvailable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool AllowShare { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDraft { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsLastVersion { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsInMyPersonalSpace { get; set; }

        /// <summary>
        /// Identificador del proyecto origen del recurso
        /// </summary>
        public Guid ProjectID { get; set; }

        public Dictionary<string, bool> PropertiesDifferences { get; set; }

        public bool IsImprovement { get; set; }

        public EstadoVersion VersionState { get; set; }
        public bool IsInProcessOfImprovement { get; set; }
    }

    /// <summary>
    /// Modelo de un gadget
    /// </summary>
    [Serializable]
    public abstract class GadgetModel
    {
        /// <summary>
        /// Identificador del gadget
        /// </summary>
        public Guid Key { get; set; }
        /// <summary>
        /// Titulo del gadget
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Nombre corto del gadget(necesario para identificar un gadget en una vista)
        /// </summary>
        public string ShortName { get; set; }
        /// <summary>
        /// Clase que se le da al gadget para poder ser identificado desde javascript
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// Orden en el que se pinta el gadget
        /// </summary>
        public int Order { get; set; }
    }

    /// <summary>
    /// Modelo de un gadget de tipo HTML
    /// </summary>
    [Serializable]
    public partial class GadgetHtmlModel : GadgetModel
    {
        /// <summary>
        /// Html del contenido del gadget
        /// </summary>
        public string Html { get; set; }
    }

    /// <summary>
    /// Modelo de un gadget de tipo CMS
    /// </summary>
    [Serializable]
    public partial class GadgetCMSModel : GadgetModel
    {
        /// <summary>
        /// Objeto CMS
        /// </summary>
        public CMSComponent CMSComponent { get; set; }
    }

    /// Modelo de un gadget de tipo Lista de recursos
    [Serializable]
    public partial class GadgetResourceListModel : GadgetModel
    {
        /// <summary>
        /// Lista de recursos
        /// </summary>
        public List<ResourceModel> Resources { get; set; }

        /// <summary>
        /// Lista de recursos de la segunda pagina
        /// </summary>
        public List<ResourceModel> ResourcesPagers { get; set; }

        /// <summary>
        /// Nombre da la vista de los recursos
        /// </summary>
        public string ViewNameResources { get; set; }

        /// <summary>
        /// Enlace a ver mas recursos
        /// </summary>
        public string UrlViewMore { get; set; }
    }

    /// <summary>
    /// Modelo de un gadget de tipo Lista de comunidades
    /// </summary>
    [Serializable]
    public partial class GadgetCommunitiesListModel : GadgetModel
    {
        /// <summary>
        /// Lista de comunidades
        /// </summary>
        public List<CommunityModel> Communities { get; set; }
    }

    /// <summary>
    /// Modelo de la configuración de las vistas de un recurso semantico en las busquedas y contextos
    /// </summary>
    [Serializable]
    public partial class ViewSettingResorceModel
    {
        #region Miembros

        private ViewResorceModel mListView;
        private ViewResorceModel mMosaicView;
        private ViewResorceModel mContextView;
        private ViewResorceModel mMapView;

        #endregion

        #region Propiedades

        /// <summary>
        /// Indica las propiedades que se deben mostrar en la vista Listado
        /// </summary>
        public ViewResorceModel ListView
        {
            get 
            {
                if(mListView == null)
                {
                    mListView = new ViewResorceModel();
                }
                return mListView;
            }
            set 
            { 
                mListView = value; 
            }
        }

        /// <summary>
        /// Indica las propiedades que se deben mostrar en la vista Mosaico
        /// </summary>
        public ViewResorceModel MosaicView 
        {
            get
            {
                if (mMosaicView == null)
                {
                    mMosaicView = new ViewResorceModel();
                }
                return mMosaicView;
            }
            set
            {
                mMosaicView = value;
            }
        }

        /// <summary>
        /// Indica las propiedades que se deben mostrar en la vista Contexto
        /// </summary>
        public ViewResorceModel ContextView 
        {
            get
            {
                if (mContextView == null)
                {
                    mContextView = new ViewResorceModel();
                }
                return mContextView;
            }
            set
            {
                mContextView = value;
            }
        }

        /// <summary>
        /// Indica las propiedades que se deben mostrar en la vista Mapa
        /// </summary>
        public ViewResorceModel MapView 
        {
            get
            {
                if (mMapView == null)
                {
                    mMapView = new ViewResorceModel();
                }
                return mMapView;
            }
            set
            {
                mMapView = value;
            }
        }

        /// <summary>
        /// Lista de propiedades semánticas del recurso
        /// </summary>
        public Dictionary<string, List<SemanticPropertieModel>> SemanticProperties { get; set; }

        /// <summary>
        /// Lista de propiedades semánticas personalizasdas del recurso
        /// </summary>
        public List<CustomSemanticPropertiesModel> CustomSemanticProperties { get; set; }

        /// <summary>
        /// Indica el tipo de vista que está por defecto
        /// </summary>
        public TipoVista VistaDefecto { get; set; }

        #endregion
    }

    [Serializable]
    public partial class ViewResorceModel
    {
        /// <summary>
        /// Indica si se debe mostrar la descripcion
        /// </summary>
        public bool ShowDescription { get; set; }
        /// <summary>
        /// Indica si se deben mostrar las categorias
        /// </summary>
        public bool ShowCategories { get; set; }
        /// <summary>
        /// Indica si se deben mostrar las etiquetas
        /// </summary>
        public bool ShowTags { get; set; }
        /// <summary>
        /// Indica si se debe mostrar el publicador
        /// </summary>
        public bool ShowPublisher { get; set; }
        /// <summary>
        /// Html de los datos semanticos del recurso
        /// </summary>
        public string InfoExtra { get; set; }
    }

    public enum TipoVista
    {
        /// <summary>
        /// Lista
        /// </summary>
        Lista = 0,
        /// <summary>
        /// Mosaico
        /// </summary>
        Mosaico = 1,
        /// <summary>
        /// Mapa
        /// </summary>
        Mapa = 2,
        /// <summary>
        /// Contexto
        /// </summary>
        Contexto = 3
    }

    /// <summary>
    /// Modelo de una propiedad semántica para ViewSettingResorceModel
    /// </summary>
    [Serializable]
    public partial class SemanticPropertieModel
    {
        /// <summary>
        /// Nombre de la propiedad
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Url del filtro de la propiedad
        /// </summary>
        public string Url { get; set; }
    }

    /// <summary>
    /// Modelo de una propiedad semántica para ViewSettingResorceModel
    /// </summary>
    [Serializable]
    public partial class CustomSemanticPropertiesModel
    {
        /// <summary>
        /// Identificador
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Filas
        /// </summary>
        public List<Dictionary<string, string>> Rows { get; set; }
    }



    /// <summary>
    /// Modelo de eventos de un recurso
    /// </summary>
    [Serializable]
    public partial class ResourceEventModel
    {
        /// <summary>
        /// Tipo de evento
        /// </summary>
        public enum EventType
        {
            /// <summary>
            /// Votado
            /// </summary>
            Voted = 0,
            /// <summary>
            /// Certificado
            /// </summary>
            Certified = 1,
            /// <summary>
            /// Commentado
            /// </summary>
            Commented = 2
        }


        /// <summary>
        /// Indica si es un evento de comentario, de voto o de certificado
        /// </summary>
        public EventType Type { get; set; }
        /// <summary>
        /// Fecha en la que se ha realizado el evento
        /// </summary>
        public DateTime Date { get; set; }

    }

    [Serializable]
    public partial class ResourceEventCertifyModel : ResourceEventModel
    {
        public string Description { get; set; }
    }

    [Serializable]
    public partial class ResourceEventCommentModel : ResourceEventModel
    {
        public Guid CommentKey { get; set; }
        public CommentModel Comment { get; set; }
    }

    /// <summary>
    /// Modelo de votos
    /// </summary>
    [Serializable]
    public partial class VotesModel
    {
        /// <summary>
        /// Número de votos positivos
        /// </summary>
        public int NumPositiveVotes { get; set; }
        /// <summary>
        /// Número de votos Negativos
        /// </summary>
        public int NumNegativeVotes { get; set; }
        /// <summary>
        /// Indica si has votado positivo
        /// </summary>
        public bool IsVotedPositive { get; set; }
        /// <summary>
        /// Indica si has votado Negativo
        /// </summary>
        public bool IsVotedNegative { get; set; }
        /// <summary>
        /// Indica si eres el creador del objeto que se esta votando
        /// </summary>
        public bool IsOwnedAuthor { get; set; }
        /// <summary>
        /// Indica si se pueden mostrar los votantes
        /// </summary>
        public bool ShowVoters { get; set; }
        /// <summary>
        /// Indica si se puede votar negativo
        /// </summary>
        public bool AllowNegativeVotes { get; set; }
        /// <summary>
        /// Número de votos totales 
        /// </summary>
        public int NumVotes { get; set; }
        /// <summary>
        /// Lista de votantes
        /// </summary>
        public List<VoterModel> Voters { get; set; }
        /// <summary>
        /// Indica la url para votar positivo
        /// </summary>
        public string UrlVotePositive { get; set; }
        /// <summary>
        /// Indica la url para votar negativo
        /// </summary>
        public string UrlVoteNegative { get; set; }
        /// <summary>
        /// Indica la url para eliminar un voto
        /// </summary>
        public string UrlDeleteVote { get; set; }
        /// <summary>
        /// Modelo de un votante
        /// </summary>
        [Serializable]
        public partial class VoterModel
        {
            /// <summary>
            /// Nombre
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Url del perfil
            /// </summary>
            public string Url { get; set; }
            /// <summary>
            /// Foto del votante
            /// </summary>
            public string Image { get; set; }
            /// <summary>
            /// Voto
            /// </summary>
            public int Vote { get; set; }
        }
    }

    /// <summary>
    /// Modelo de un Comentario
    /// </summary>
    [Serializable]
    public partial class CommentModel
    {
        /// <summary>
        /// Identificador del comentario
        /// </summary>
        public Guid Key { get; set; }
        /// <summary>
        /// Texto del comentario
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Fecha de publicación del comentario
        /// </summary>
        public DateTime PublishDate { get; set; }
        /// <summary>
        /// Publicador del comentario
        /// </summary>
        public ProfileModel PublisherCard { get; set; }
        /// <summary>
        /// Votos del comentario
        /// </summary>
        public VotesModel Votes { get; set; }
        /// <summary>
        /// Respuestas a el comentario
        /// </summary>
        public List<CommentModel> Replies { get; set; }
        /// <summary>
        /// Acciones permitidas en el comentario
        /// </summary>
        public ActionsModel Actions { get; set; }

        /// <summary>
        /// Modelo de acciones de un comentario
        /// </summary>
        [Serializable]
        public partial class ActionsModel
        {
            /// <summary>
            /// Indica si el usuario se puede eliminar el comentario
            /// </summary>
            public bool Delete { get; set; }
            /// <summary>
            /// Url de la accion de eliminar un comentario
            /// </summary>
            public string UrlDelete { get; set; }
            /// <summary>
            /// Indica si el usuario se puede editar el comentario
            /// </summary>
            public bool Edit { get; set; }
            /// <summary>
            /// Url de la accion de editar un comentario
            /// </summary>
            public string UrlEdit { get; set; }
            /// <summary>
            /// Indica si el usuario se puede responder el comentario
            /// </summary>
            public bool Reply { get; set; }
            /// <summary>
            /// Url de la accion de responder un comentario
            /// </summary>
            public string UrlReply { get; set; }
            /// <summary>
            /// Url de la accion de votar positivo un comentario
            /// </summary>
            public string UrlVotePositive { get; set; }
            /// <summary>
            /// Url de la accion de votar negativo un comentario
            /// </summary>
            public string UrlVoteNegative { get; set; }
            /// <summary>
            /// Url de la accion de eliminar el voto de un comentario
            /// </summary>
            public string UrlDeleteVote { get; set; }

        }
    }

    /// <summary>
    /// Modelo para las respuestas JSON de un Servicio Externo.
    /// </summary>
    [Serializable]
    public class JsonExtServResponse
    {
        /// <summary>
        /// Estado de la respuesta: '0' KO. '1' OK.
        /// </summary>
        public short Status { get; set; }
        /// <summary>
        /// Mensaje de la respuesta.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Acción que debe realizarse.
        /// </summary>
        public JsonAction Action { get; set; }
    }

    /// <summary>
    /// Modelo para las acciones de las respuestas JSON de un Servicio Externo.
    /// </summary>
    [Serializable]
    public class JsonAction
    {
        /// <summary>
        /// Indica si debe redirigirse a la home.
        /// </summary>
        public bool RedirectCommunityHome { get; set; }
        /// <summary>
        /// Indica si debe redirigirse a la ficha del recurso actual.
        /// </summary>
        public bool RedirectResource { get; set; }
        /// <summary>
        /// Indica si debe redirigirse a la ficha de un recurso en particular, contiene el GUID del recuro.
        /// </summary>
        public Guid RedirectSpecificResource { get; set; }
        /// <summary>
        /// Indica si debe redirigirse a una URL concreta.
        /// </summary>
        public string RedirectUrl { get; set; }
        /// <summary>
        /// Indica si debe ejecutar el código javascript de la variable.
        /// </summary>
        public string RunJavaScript { get; set; }
    }
}