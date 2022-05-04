using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Es.Riam.Semantica;
using Es.Riam.Semantica.OWL;
using Es.Riam.Semantica.Plantillas;

namespace Es.Riam.Gnoss.Web.MVC.Models
{
    /// <summary>
    /// Modelo para editar recurso.
    /// </summary>
    [Serializable]
    public partial class EditResourceModel
    {
        /// <summary>
        /// Tipo de página que se va a cargar, se debe mostrar una vista u otra en función del tipo de página.
        /// </summary>
        public TypePageEditResource TypePage { get; set; }

        /// <summary>
        /// Nombre de la pestaña actual.
        /// </summary>
        public string TabName { get; set; }

        /// <summary>
        /// Url de la pestaña actual.
        /// </summary>
        public string UrlPestanya { get; set; }

        /// <summary>
        /// Modelo para subir recurso.
        /// </summary>
        public CreateResourceModel CreateResourceModel { get; set; }

        /// <summary>
        /// Modelo para editar recurso.
        /// </summary>
        public ModifyResourceModel ModifyResourceModel { get; set; }

        /// <summary>
        /// Tipos de página para editar recurso.
        /// </summary>
        public enum TypePageEditResource
        {
            /// <summary>
            /// Página de subir recurso.
            /// </summary>
            CreateResource = 0,
            /// <summary>
            /// Página para 2º parte de subir recurso.
            /// </summary>
            CreateResource2 = 1,
            /// <summary>
            /// Página para editar recurso.
            /// </summary>
            EditResource = 2,
            /// <summary>
            /// Página para editar recurso semántico.
            /// </summary>
            CreateSemanticResource = 3,
            /// <summary>
            /// Página para editar recurso semántico.
            /// </summary>
            EditSemanticResource = 4,
            /// <summary>
            /// Página para añadir a GNOSS.
            /// </summary>
            AddToGnossResource = 5,
            /// <summary>
            /// Página para añadir a Comunidad.
            /// </summary>
            AddToCommunityResource = 6,
            /// <summary>
            /// Página de subir recurso completo.
            /// </summary>
            CreateResourceComplete = 7,
            /// <summary>
            /// Página de subir recurso completo.
            /// </summary>
            ModifyResourceComplete = 8,
        }
    }

    /// <summary>
    /// Modelo para subir recurso.
    /// </summary>
    [Serializable]
    public partial class CreateResourceModel
    {
        /// <summary>
        /// Lista con los nombres url de las ontologías disponibles para subir recurso.
        /// </summary>
        public SortedDictionary<string, KeyValuePair<string, string>> OntologyNameUrls { get; set; }

        /// <summary>
        /// Indica si está disponible la subida de ficheros.
        /// </summary>
        public bool FileAvailable { get; set; }

        /// <summary>
        /// Indica si está disponible la subida de referencias a recursos.
        /// </summary>
        public bool DocumentReferenceAvailable { get; set; }

        /// <summary>
        /// Indica si está disponible la subida de hipervinculos.
        /// </summary>
        public bool LinkAvailable { get; set; }

        /// <summary>
        /// Indica si está disponible la subida de videos de BrightCove.
        /// </summary>
        public bool BrightcoveVideoAvailable { get; set; }

        /// <summary>
        /// Indica si está disponible la subida de videos de Top.
        /// </summary>
        public bool TOPVideoAvailable { get; set; }

        /// <summary>
        /// Indica si está disponible la subida de notas.
        /// </summary>
        public bool NoteAvailable { get; set; }

        /// <summary>
        /// Indica si está disponible la subida de wikis.
        /// </summary>
        public bool WikiAvailable { get; set; }

        /// <summary>
        /// Indica si está disponible la subida de semánticos.
        /// </summary>
        public bool SemanticResourceAvailable { get; set; }

        /// <summary>
        /// Src para el iframe que se genera para Brightcove.
        /// </summary>
        public string SrcIframeBrightcove { get; set; }

        /// <summary>
        /// Url para video que se genera para Brightcove y TOP.
        /// </summary>
        public string UrlVideoIframe { get; set; }

        /// <summary>
        /// Url para audio que se genera para Brightcove y TOP.
        /// </summary>
        public string UrlAudioIframe { get; set; }

        /// <summary>
        /// Src para el iframe que se genera para TOP.
        /// </summary>
        public string SrcIframeTOP { get; set; }
    }

    /// <summary>
    /// Modelo para la acción de selección de un tipo de recurso para crearlo.
    /// </summary>
    [Serializable]
    public class SelectResourceModel
    {
        public TypeResource TypeResourceSelected { get; set; }

        /// <summary>
        /// Archivo adjunto al recurso.
        /// </summary>
        public IFormFile File { get; set; }

        /// <summary>
        /// Nombre del archivo temporal que se almacenará en el servidor. Debe comenzar por un GUID aleatorio seguido del nombre del fichero (Ej: Si tenemos el fichero "prueba.txt" será "f5e51472-1e3d-47d3-b035-f8170cef3da3prueba.txt".
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Enlace del recurso. Según el tipo del recurso será:
        /// 
        /// Hipervinculo: Enlace a la web.
        /// Referecia a documento fisico: Ubicación del documento.
        /// Archivo fisico: Nombre del adjunto al recurso.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Indica si se deben omitir las alertas por repetición de atributos del recurso actual y el cambio de privacidad que impiden guardar.
        /// Debería adquirir el valor TRUE cuando el usuario acepte la advertencia.
        /// </summary>
        public bool SkipRepeat { get; set; }

        /// <summary>
        /// Texto extra para pintar el panel de repeticióin si el archivo ya está en la comunidad.
        /// </summary>
        public string ExtraFile { get; set; }

        /// <summary>
        /// Tipos de recursos disponibles para la acción de seleccionar el tipo.
        /// </summary>
        public enum TypeResource
        {
            /// <summary>
            /// Tipo nota. No es necesario incluir ningún parámetro más.
            /// </summary>
            Note = 0,
            /// <summary>
            /// Tipo hipervinculo. Es obligatorio enviar los parámetros "Link" y "SkipRepeat".
            /// </summary>
            Link = 1,
            /// <summary>
            /// Referencia a documento. Es obligatorio enviar los parámetros "Link" y "SkipRepeat".
            /// </summary>
            DocumentReference = 2,
            /// <summary>
            /// Wiki. Es obligatorio enviar el parámetro "Link".
            /// </summary>
            Wiki = 3,
            /// <summary>
            /// Archivo adjunto. En principio hay que enviar los parámetros "File" y "FileName" con el archivo y el nombre del mismo. En caso de que el servidor envie que el archivo ya está en la comunidad y esta permita la repetición de archivo, habría que enviar "SkipRepeat" a TRUE y "ExtraFile" que proporcione el modelo de "UploadResourceReplayPanelModel" para la vista del panel de repetición.
            /// </summary>
            File = 4
        }
    }

    /// <summary>
    /// Modelo para el panel de repetición de subir recurso.
    /// </summary>
    [Serializable]
    public partial class UploadResourceReplayPanelModel
    {
        /// <summary>
        /// Indica si se puede repetir el recurso.
        /// </summary>
        public bool CanRepeatResource { get; set; }

        /// <summary>
        /// Url del recurso repetido, en caso de que esté repetido.
        /// </summary>
        public string RepeatedResourceUrl { get; set; }

        /// <summary>
        /// Url del recurso repetido.
        /// </summary>
        public string RepeatedResourceName { get; set; }

        /// <summary>
        /// Tipo de recurso repetido: 0 (ReferenciaADoc), 1(Hipervinculo), 2(Archivo), 3 (Generico para todos los tipos al editar), 4 (Aviso cambio de privacidad recurso), 5 (Aviso cambio de privacidad en debate), 6 (Cocurrencia).
        /// </summary>
        public int RepeatedResourceType { get; set; }

        /// <summary>
        /// Enlace repetido.
        /// </summary>
        public string RepetitionLink { get; set; }

        /// <summary>
        /// Información extra para el archivo.
        /// </summary>
        public string ExtraFile { get; set; }

        /// <summary>
        /// Nombre del perfil que causa la concurrencia.
        /// </summary>
        public string ProfileConcurrencyName { get; set; }

        /// <summary>
        /// Indica si se debe crear versión o no en caso de que haya concurrencia.
        /// </summary>
        public bool CreateVersionIfConcurrency { get; set; }

        /// <summary>
        /// Indica que es la página de añadir a GNOSS.
        /// </summary>
        public bool AddToGnossPage { get; set; }
    }

    /// <summary>
    /// Modelo para editar recurso.
    /// </summary>
    [Serializable]
    public partial class ModifyResourceModel
    {
        /// <summary>
        /// Modelo superior de edición de recurso.
        /// </summary>
        public EditResourceModel EditResourceModel { get; set; }

        /// <summary>
        /// Indica el sitio actual de la página para un subMenu.
        /// </summary>
        public string CurrentSiteMenuPulg { get; set; }

        /// <summary>
        /// Tipo de documento que se está modificando.
        /// </summary>
        public Es.Riam.Gnoss.Web.MVC.Models.ResourceModel.DocumentType DocumentType { get; set; }

        /// <summary>
        /// Indica si es presentación incrustada.
        /// </summary>
        public bool ItIsEmbeddedPresentation { get; set; }

        /// <summary>
        /// Indica si es video incrustado.
        /// </summary>
        public bool ItIsEmbeddedVideo { get; set; }

        /// <summary>
        /// Modelo de documento de edición.
        /// </summary>
        public DocumentEditionModel DocumentEditionModel { get; set; }

        /// <summary>
        /// Editor del tesauro actual.
        /// </summary>
        public ThesaurusEditorModel ThesaurusEditorModel { get; set; }

        /// <summary>
        /// Indica si la autoría del recurso está disponible.
        /// </summary>
        public bool CopyrightAvailable { get; set; }

        /// <summary>
        /// Indica si la edición del adjunto del recurso está disponible.
        /// </summary>
        public bool EditAttachedAvailable { get; set; }

        /// <summary>
        /// Indica si la edición de las propiedades del recurso está disponible.
        /// </summary>
        public bool EditPropertiesAvailable { get; set; }

        /// <summary>
        /// Indica si la edición del fichero del recurso está disponible.
        /// </summary>
        public bool EditFileAvailable { get; set; }

        /// <summary>
        /// Indica si la edición de la ubicación del recurso está disponible.
        /// </summary>
        public bool EditLocationAvailable { get; set; }

        /// <summary>
        /// Indica si la edición de la url del recurso está disponible.
        /// </summary>
        public bool EditUrlAvailable { get; set; }

        /// <summary>
        /// Indica si la edición del título del recurso está disponible.
        /// </summary>
        public bool EditTitleAvailable { get; set; }

        /// <summary>
        /// Indica si la edición de la descripción del recurso está disponible.
        /// </summary>
        public bool EditDescriptionAvailable { get; set; }

        /// <summary>
        /// Indica si la edición de las respuestas de las encuestas está disponible.
        /// </summary>
        public bool EditPollAnswersAvailable { get; set; }

        /// <summary>
        /// Indica si se está creando una versión del documento.
        /// </summary>
        public bool CreatingVersion { get; set; }

        /// <summary>
        /// Indica si hay multiples usuarios editando el recurso. En caso de ser TRUE se le debe presentar al usuario un mensaje advirtiendole de ello en el caso de que el recurso tenga un adjunto y se reemplece éste.
        /// </summary>
        public bool MultipleEditors { get; set; }

        /// <summary>
        /// Indica si se permite que los recursos sean privados.
        /// </summary>
        public bool PrivateResourcesAvailable { get; set; }

        /// <summary>
        /// Indica si se permiten lectores de comunidad.
        /// </summary>
        public bool CommunityReadersAvailable { get; set; }

        /// <summary>
        /// Indica si se permite que los recursos sean abiertos.
        /// </summary>
        public bool OpenResourcesAvailable { get; set; }

        /// <summary>
        /// Modelo para el selector de usuario para edición.
        /// </summary>
        public UsersSelectorModel UsersSelectorEditionModel { get; set; }

        /// <summary>
        /// Modelo para el selector de usuario para lectra.
        /// </summary>
        public UsersSelectorModel UsersSelectorReadingModel { get; set; }

        /// <summary>
        /// Indica que la visibilidad actual del recurso es para miembros de la comunidad. En función de esta visibilidad, de si el recurso es privado para editores y de si la comunidad permite recursos abiertos, se calculará la visibilidad actual del recurso.
        /// </summary>
        public bool VisibilityMembersCommunity { get; set; }

        /// <summary>
        /// Indica si se permite que se pueda editar los permisos de edición del recurso.
        /// </summary>
        public bool SetPermissionsEditionAvailable { get; set; }

        /// <summary>
        /// Indica si se permite el selector de editores dentro de la configuráción de permisos de edición.
        /// </summary>
        public bool SelectorEditionAvailable { get; set; }

        /// <summary>
        /// Indica si se permite la protección del recurso.
        /// </summary>
        public bool ResourceProtectionAvailable { get; set; }

        /// <summary>
        /// Indica si las propiedades del recurso están disponibles
        /// </summary>
        public bool ResourcePropertiesAvailable { get; set; }

        /// <summary>
        /// Indica si compartir está disponible.
        /// </summary>
        public bool ShareAvailable { get; set; }

        /// <summary>
        /// Modelo para la edición de licencias.
        /// </summary>
        public LicenseEditorModel LicenseEditorModel { get; set; }

        /// <summary>
        /// Nombre del adjunto al recurso subido por un addin como el de Office o Añadir a Gnoss.
        /// </summary>
        public string UploadedAttachedNameByAddin { get; set; }

        /// <summary>
        /// Url para el botón cancelar.
        /// </summary>
        public string UrlCancelButton { get; set; }

        /// <summary>
        /// Url para el botón ir a la home.
        /// </summary>
        public string UrlGoHomeButton { get; set; }

        /// <summary>
        /// Modelo de respuestas para la encuesta.
        /// </summary>
        public PollAnswersModel PollAnswersModel { get; set; }

        /// <summary>
        /// Modelo para editar recursos semánticos. Solo aplica a recursos semánticos.
        /// </summary>
        public SemanticResourceModel SemanticResourceModel { get; set; }

        /// <summary>
        /// Lista con los IDs y nombres de las bases de recursos donde se puede compartir un recurso add to Gnoss.
        /// </summary>
        public Dictionary<Guid, string> AddToGnossShareSites { get; set; }

        /// <summary>
        /// Mensjae que indica que hay una nueva versión del add to Gnoss disponible.
        /// </summary>
        public string NewVersionMessageAddToGnossAvailable { get; set; }

        /// <summary>
        /// Mensjae que indica que hay una nueva versión del adding de Office disponible.
        /// </summary>
        public string NewVersionMessageAddToGnossOfficeAvailable { get; set; }

        /// <summary>
        /// Token del vídeo top
        /// </summary>
        public string TOPTokenID { get; set; }

        /// <summary>
        /// Tipo de documento que es el vídeo/audio del tipo top
        /// </summary>
        public int TOPDocType { get; set; }

        /// <summary>
        /// URL SRC del iframe del vídeo / audio top
        /// </summary>
        public string TOPIframeSRC { get; set; }

        /// <summary>
        /// Subida unificada
        /// </summary>
        public bool SubidaUnificada { get; set; }

        /// <summary>
        /// Edicion unificada.
        /// </summary>
        public bool EdicionUnificada { get; set; }

        /// <summary>
        /// Lista de los recursos vinculados.
        /// </summary>
        public List<String> Vinculados { get; set; }
    }

    /// <summary>
    /// Modelo para editar recurso.
    /// </summary>
    [Serializable]
    public partial class DocumentEditionModel
    {
        #region Propiedades para generar vista y para la acción guardar recurso

        /// <summary>
        /// Título del recurso.
        /// </summary>
        public string Title { get; set; } = "";

        /// <summary>
        /// Descripción del recurso. En el caso de se esté enviando la descripción mediante la acción de guardar recurso, esta debe enviarse codificada.
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// Palabras claves que se pueden asociar a un recurso.
        /// </summary>
        public string Tags { get; set; } = "";

        /// <summary>
        /// Enlace del recurso. Según el tipo del recurso será:
        /// 
        /// Hipervinculo: Enlace a la web.
        /// Referecia a documento fisico: Ubicación del documento.
        /// Archivo fisico: Nombre del adjunto al recurso.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Indica si se puede compartir el recurso. Si es TRUE, el recurso podrá ser compartido y visto en otras comunidades del ecosistema.
        /// </summary>
        public bool ShareAllowed { get; set; }

        /// <summary>
        /// Autores del recurso. Texto en el que se especifican autores del recurso separados por coma.
        /// </summary>
        public string Authors { get; set; } = "";

        /// <summary>
        /// Indica si el recurso es borrador. Si es borrador solo podrá ser visible por el creador del recurso hasta que se publique y deje de ser borrador, entonces será visible según la configuración elegida.
        /// Este elemento se usa para generar la vista y se recoge en la acción de guardar recurso.
        /// </summary>
        public bool Draft { get; set; }

        /// <summary>
        /// Indica si el creador del recurso es el autor del mismo. Cuando se marca un recurso de esta forma, es posible editar la licencia del mismo ya que el creador es la persona que está editando el recurso y tiene derecho intelectual sobre su obra.
        /// </summary>
        public bool CreatorIsAuthor { get; set; }

        /// <summary>
        /// Licencia Creative Commons del recurso. Ségun el valor del string será una licencia u otra:
        /// "00": by
        /// "01": by-sa
        /// "02": by-nd
        /// "10": by-nc
        /// "11": by-nc-sa
        /// "12": by-nc-nd
        /// </summary>
        public string License { get; set; }

        /// <summary>
        /// Indica si el documento está protegido. Si es asi, solo quien lo ha protegido podrá editarlo. Los demás editores si podrán crear una versión del mismo.
        /// </summary>
        public bool Protected { get; set; }

        #endregion

        #region Propiedades Solo Montar Vista

        /// <summary>
        /// ID del recurso. GUID que identifica el recurso actual de manera única.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Indica si el creador del recurso es la identidad actual conectada. En este caso la identidad tendrá funciones extra sobre el recurso como editar la autoría o seleccionar la licencia del recurso en caso de que éste permita licencias.
        /// </summary>
        public bool ActualIdentityIsCreator { get; set; }

        /// <summary>
        /// Etiquetas que se generan automáticamente para el título. Estas palabras clave serán sugeridas para agregarlas a los Tags del recurso.
        /// </summary>
        public string AutomaticTagsTitle { get; set; }

        /// <summary>
        /// Indica si el documento solo está compartido en un proyecto y este es privado o reservado. En caso de que sea TRUE, el panel para editar la licencia del recurso solo aparecerá en el caso de que el documento tenga la compartición permitida, además de si se indica que el creador es el autor del recurso. Si es FALSE solo se tendrá en cuenta la autoría.
        /// </summary>
        public bool SharedDocumentJustInPrivateProject { get; set; }

        /// <summary>
        /// Indica si el documento de tipo Wiki posee un autoguardado que se puede recuperar. El usuario seleccionará si lo recupera o no.
        /// </summary>
        public bool AutosaveAvailable { get; set; }

        /// <summary>
        /// Url que permite recuperar un autoguardado de un Wiki en el caso en el que exista. Si el usuario elige recurperar el autoGuardado habría que llevarle a esta Url.
        /// </summary>
        public string UrlRecoverAutosave { get; set; }



        /// <summary>
        /// Indica si el recurso es privado para editores. Si es así, solo los editores y lectores del recurso podrán ver el recurso.
        /// </summary>
        public bool PrivateEditors { get; set; }

        /// <summary>
        /// Información acerca de quien ha protegido el recurso si así es: Nombre del protector, Fecha protección.
        /// </summary>
        public KeyValuePair<string, string> ProtectionInfo { get; set; }

        /// <summary>
        /// Indica si la modificación de la protección del recurso está disponible para el usuario actual.
        /// </summary>
        public bool ModificationProtectionAvailable { get; set; }

        /// <summary>
        /// Url de descarga del archivo adjunto al recurso.
        /// </summary>
        public string UrlDownloadAttached { get; set; }

        /// <summary>
        /// Indica si el recurso permite tener licencia. Si es así, y el usuario marca que es el autor del recurso aparecerá el editor de licencia.
        /// </summary>
        public bool AllowsLicense { get; set; }

        #endregion

        #region Propiedades para la acción guardar recurso

        /// <summary>
        /// Indica si se deben omitir las alertas por repetición de atributos del recurso actual y el cambio de privacidad que impiden guardar.
        /// Debería adquirir el valor TRUE cuando el usuario acepte la advertencia.
        /// </summary>
        public bool SkipRepeat { get; set; }

        /// <summary>
        /// Indica si se va a hacer una redirección por concurrencia. Se debe producir cuando dos usuarios está editando el mismo recurso a la vez y el 2º usuario elige que quiere sobrescribir el mismo.
        /// </summary>
        public bool? RedirectionByConcurrency { get; set; }

        /// <summary>
        /// Indica si se va a crear una versión a causa de concurrencia. Se debe producir cuando dos usuarios está editando el mismo recurso a la vez y el 2º usuario elige que quiere crear una nueva versión del mismo.
        /// </summary>
        public bool? CreateVersionByConcurrency { get; set; }

        /// <summary>
        /// Indica si el guardado que se está haciendo es un autoGuardado de Wiki.
        /// </summary>
        public bool? AutoSave { get; set; }

        /// <summary>
        /// Nombre del adjunto temporal previamente subido al recurso. Debe comenzar por un GUID aleatorio seguido del nombre del fichero (Ej: Si tenemos el fichero "prueba.txt" será "f5e51472-1e3d-47d3-b035-f8170cef3da3prueba.txt".
        /// </summary>
        public string TemporalFileName { get; set; }

        /// <summary>
        /// Categorías del tesauro seleccionadas. Deberá contener los GUIDs de las categorías separadas por comas.
        /// </summary>
        public string SelectedCategories { get; set; }

        /// <summary>
        /// Indica si los editores del recurso son unos especificados (TRUE) o si es el publicador del recurso (FALSE).
        /// </summary>
        public bool SpecificResourceEditors { get; set; }

        /// <summary>
        /// Editores específicos del recurso separados por comas.
        /// Los editores pueden ser perfiles o grupos.
        /// Para el perfil editor se guardará un GUID con el ID del perfil y en el caso de un grupo, el GUID de grupo editor precedido de los caracteres "g_" (Ej: "g_f5e51472-1e3d-47d3-b035-f8170cef3da3").
        /// </summary>
        public string ResourceEditors { get; set; }

        /// <summary>
        /// Visibilidad del recurso en la comunidad.
        /// </summary>
        public ResourceVisibility ResourceVisibility { get; set; }

        /// <summary>
        /// Lectores específicos del recurso separados por comas.
        /// Los lectores pueden ser perfiles o grupos.
        /// Para el perfil lector se guardará un GUID con el ID del perfil y en el caso de un grupo, el GUID de grupo lector precedido de los caracteres "g_" (Ej: "g_f5e51472-1e3d-47d3-b035-f8170cef3da3").
        /// </summary>
        public string ResourceReaders { get; set; }

        /// <summary>
        /// Indica si se debe crear una nueva versión del recurso a causa de que el documento tiene un adjunto, tiene varios editores, el editor actual ha reemplazado el archivo y tras aparecerle un mensaje indicandole que no es el único editor del recuro ha dedicido crear una nueva versión.
        /// </summary>
        public bool CreateVersionByReplaceAttachment { get; set; }

        /// <summary>
        /// Enlaces automáticos que se generar para los tags separados por "&&&".
        /// </summary>
        public string TagsLinks { get; set; }

        /// <summary>
        /// Indica si la Newsletter es manual, es decir con texto plano rellenado por el usuario en la descripción. En caso de ser FALSE, indica que se ha subido un archivo de html como adjunto a la Newsletter.
        /// </summary>
        public bool NewsletterManual { get; set; }

        /// <summary>
        /// Indica si se quiere proteger la nueva versión de un documento que ser va a crear porque el original estaba previamente protegido.
        /// </summary>
        public bool? ProtectDocumentProtected { get; set; }

        /// <summary>
        /// Respuestas de la encuesta separadas por "[[&]]".
        /// </summary>
        public string PollResponses { get; set; }

        /// <summary>
        /// Valor RDF del recurso en un formato especial para el guardado GNOSS. Solo se aplica a recursos de tipo semántico.
        /// </summary>
        public string RdfValue { get; set; }

        /// <summary>
        /// Valor de la imagen representante del recurso. Contendrá la url de la imagen con sus tamaños. Solo se aplica a recursos de tipo semántico.
        /// </summary>
        public string ImageRepresentativeValue { get; set; }

        /// <summary>
        /// Indica si se deben omitir las alertas por repetición de los valores de las propiedades configuradas en el XML de la ontología como de único valor.
        /// Debería adquirir el valor TRUE cuando el usuario acepte la advertencia.
        /// </summary>
        public bool SkipSemanticPropertyRepeat { get; set; }

        /// <summary>
        /// ID del recurso masivo que se está editando dentro de una carga masiva.
        /// </summary>
        public Guid EditingMassiveResourceID { get; set; }

        /// <summary>
        /// Información auxiliar acerca de los recursos masivos que se están editando dentro de una carga masiva.
        /// </summary>
        public string MassiveResourceLoadInfo { get; set; }

        /// <summary>
        /// Información de las imágenes con su ancho y alto separados por '|' para procesarlas con OpenSeaDragon.
        /// </summary>
        public string OpenSeaDragonInfo { get; set; }

        /// <summary>
        /// Información auxiliar acerca de las ontologías externas editables dentro del recurso para la edición del SEM CMS.
        /// </summary>
        public string SubOntologiesExtInfo { get; set; }

        /// <summary>
        /// Información auxiliar con los IDs de las entidades en la edición del SEM CMS.
        /// </summary>
        public string EntityIDRegisterInfo { get; set; }

        #endregion
    }

    /// <summary>
    /// Modelo para la selección de usuarios.
    /// </summary>
    [Serializable]
    public partial class UsersSelectorModel
    {
        #region Miembros

        /// <summary>
        /// IDs con los perfiles seleccionados.
        /// </summary>
        private Dictionary<Guid, string> mSelectedProfilesList;

        /// <summary>
        /// IDs con los grupos seleccionados.
        /// </summary>
        private Dictionary<Guid, string> mSelectedGroupsList;

        #endregion

        /// <summary>
        /// Texto informativo para el selector de usuarios.
        /// </summary>
        public string TextInfoSelectUsers { get; set; }

        /// <summary>
        /// Texto informativo los usuarios seleccionados.
        /// </summary>
        public string TextSelectUsers { get; set; }

        /// <summary>
        /// IDs con los perfiles seleccionados.
        /// </summary>
        public Dictionary<Guid, string> SelectedProfilesList
        {
            get
            {
                if (mSelectedProfilesList == null)
                {
                    mSelectedProfilesList = new Dictionary<Guid, string>();
                }
                return mSelectedProfilesList;
            }
        }

        /// <summary>
        /// IDs con los grupos seleccionados.
        /// </summary>
        public Dictionary<Guid, string> SelectedGroupsList
        {
            get
            {
                if (mSelectedGroupsList == null)
                {
                    mSelectedGroupsList = new Dictionary<Guid, string>();
                }
                return mSelectedGroupsList;
            }
        }
    }

    /// <summary>
    /// Modelo para la edición de licencia.
    /// </summary>
    [Serializable]
    public partial class LicenseEditorModel
    {
        /// <summary>
        /// Indica si la licencia no es editable.
        /// </summary>
        public bool NotEditable { get; set; }

        /// <summary>
        /// Licencia por defecto para el editor.
        /// </summary>
        public string DefaultLicense { get; set; }

        /// <summary>
        /// Mensaje de la comunidad para la licencia por defecto.
        /// </summary>
        public string MessageDefaultLicense { get; set; }

        /// <summary>
        /// Licencia actual.
        /// </summary>
        public string License { get; set; }

        /// <summary>
        /// Nombre del ecosistema del proyecto.
        /// </summary>
        public string EcosystemProjectName { get; set; }
    }

    /// <summary>
    /// Modelo para la edición de las categorías del tesauro para el recurso.
    /// </summary>
    [Serializable]
    public partial class ThesaurusEditorModel
    {
        /// <summary>
        /// Categorías del tesauro disponibles del editor.
        /// </summary>
        public List<CategoryModel> ThesaurusCategories;
        /// <summary>
        /// Categorías del tesauro disponibles del editor.
        /// </summary>
        public List<CategoryModel> SuggestedThesaurusCategories;

        /// <summary>
        /// Lista con los IDs de las categorías seleccionadas.
        /// </summary>
        public List<Guid> SelectedCategories;

        /// <summary>
        /// Lista con los IDs de las categorías deshabilitadas.
        /// </summary>
        public List<Guid> DisabledCategories;

        /// <summary>
        /// Indica si se debe ocultar el selector de árbol o lista.
        /// </summary>
        public bool HideTreeListSelector;

        /// <summary>
        /// Categorías que deben pintarse expandidas en el tesauro.
        /// </summary>
        public List<Guid> ExpandedCategories;

        /// <summary>
        /// Categorías que deben pintarse expandidas en el tesauro.
        /// </summary>
        public List<Guid> SharedCategories;

        /// <summary>
        /// Propiedades extra de las categorias semánticas.
        /// </summary>
        public Dictionary<string, Propiedad> ExtraPropertiesCategories;
    }

    /// <summary>
    /// Modelo para las respuestas de una encuesta.
    /// </summary>
    [Serializable]
    public partial class PollAnswersModel
    {
        /// <summary>
        /// Respuestas de la encuesta.
        /// </summary>
        private List<string> mAnswers;

        /// <summary>
        /// Respuestas de la encuesta.
        /// </summary>
        public List<string> Answers
        {
            get
            {
                if (mAnswers == null)
                {
                    mAnswers = new List<string>();
                }

                return mAnswers;
            }
        }
    }

    /// <summary>
    /// Modelo para adjuntar archivos al recurso.
    /// </summary>
    [Serializable]
    public class AttachmentModel
    {
        /// <summary>
        /// Archivo adjunto al recurso.
        /// </summary>
        public IFormFile File { get; set; }

        /// <summary>
        /// Nombre del archivo temporal que se almacenará en el servidor. Debe comenzar por un GUID aleatorio seguido del nombre del fichero (Ej: Si tenemos el fichero "prueba.txt" será "f5e51472-1e3d-47d3-b035-f8170cef3da3prueba.txt".
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Indica si el archivo subido pertenece a una NewsLetter. Si el usuario selecciona que su newletter contiene un archivo deberá subirlo con esta acción.
        /// </summary>
        public bool IsNewsLetter { get; set; }

        /// <summary>
        /// Información extra para el SEMCMS. Sólo se aplica para recursos de tipo semántico. Contiene separdos por '|':
        /// 1: Nombre_Propiedad + ',' + Nombre_Entidad
        /// 2: Tipo de archivo configurado para la propiedad
        /// 3: ID para el archivo o nombre del archivo si no es imágen
        /// 4: ID para el archivo solo en el caso de que no sea imagen
        /// </summary>
        public string ExtraSemCms { get; set; }
    }

    /// <summary>
    /// Visibilidad de un recurso en una comunidad.
    /// </summary>
    public enum ResourceVisibility
    {
        /// <summary>
        /// Abierto, todo el mundo podrá verlo, incluso los que no sean miembros de la comunidad.
        /// </summary>
        Open = 0,
        /// <summary>
        /// Miembros comunidad, solo los miembros de la comunidad podrán ver el recurso.
        /// </summary>
        CommunityMembers = 1,
        /// <summary>
        /// Solo editores, únicamente los editores del recurso tendrán acceso al mismo.
        /// </summary>
        OnlyEditors = 2,
        /// <summary>
        /// Lectores específicos, solo podrán ver el recurso un conjunto de lectores especificados y los editores.
        /// </summary>
        SpecificReaders = 3
    }

    /// <summary>
    /// Modelo para editar recurso semántico.
    /// </summary>
    [Serializable]
    public class SemanticResourceModel
    {
        /// <summary>
        /// Indica si se está haciendo una carga masiva de recursos semánticos.
        /// </summary>
        public bool MassiveResourceLoad { get; set; }

        /// <summary>
        /// Indica si se está editando un recurso dentro de una carga masiva de recursos semánticos.
        /// </summary>
        public bool EditingMassiveResourceLoad { get; set; }

        /// <summary>
        /// ID del recurso que se está editando dentro de una carga masiva de recursos semánticos.
        /// </summary>
        public Guid EditingMassiveResourceID { get; set; }

        /// <summary>
        /// Url para editar las categorías del tesauro en otra comunidad. Tendrá valor en caso de que no se puedan editar las categoriás en la comunidad actual y hay que editarlas en la url indicada.
        /// </summary>
        public string OtherCommunityEditCategoriesUrl { get; set; }

        /// <summary>
        /// Indica que las categorias del tesauro no son obligatorias, por lo que no deberá pintarse el control para seleccionar categorías.
        /// </summary>
        public bool ThesaurusCategoryNotRequired { get; set; }

        /// <summary>
        /// Indica que el formulario semánico contiene el título y la descripción del
        /// </summary>
        public bool SemCmsContainsTitleAndDescription { get; set; }

        /// <summary>
        /// Indica si se debe generar el SEM CMS para su lectura, es decir, en la ficha del recurso o para su edición si es FALSE.
        /// </summary>
        public bool ReadMode { get; set; }

        /// <summary>
        /// Indica si se debe ocultar la información puesto que el usuario actual no es miembro de la comunidad y no debe ver el SEM CMS.
        /// </summary>
        public bool HideInfoIsNotMember { get; set; }

        /// <summary>
        /// Título de la información para los no miembros de la comunidad
        /// </summary>
        public string TitleInfoIsNotMember { get; set; }

        /// <summary>
        /// Link de registro de la información para los no miembros de la comunidad
        /// </summary>
        public string RegisterLinkInfoIsNotMember { get; set; }

        /// <summary>
        /// Namespace de la ontología.
        /// </summary>
        public string OntologyNamespace { get; set; }

        /// <summary>
        /// Url de la ontología.
        /// </summary>
        public string OntologyUrl { get; set; }

        /// <summary>
        /// Namespaces definidos en la ontología.
        /// </summary>
        public Dictionary<string, string> OntologyNamespaces { get; set; }

        /// <summary>
        /// Propiedad RDF que vincula un formulario semántico con un recurso.
        /// </summary>
        public string DocSemCmsProperty { get; set; }

        /// <summary>
        /// Entidades raíz del SEM CMS. Son las que no son hijas de ninguna otra entidad de la ontología.
        /// </summary>
        public List<SemanticEntityModel> RootEntities { get; set; }

        /// <summary>
        /// Valor de la imagen representante del recurso. Contendrá la url de la imagen con sus tamaños.
        /// </summary>
        public string ImageRepresentativeValue { get; set; }

        /// <summary>
        /// Idioma por defecto de la ontología, solo tiene valor si la edición es multiIdioma.
        /// </summary>
        public string DefaultLanguage { get; set; }

        /// <summary>
        /// Idioma disponibles para la ontología, solo tiene valor si la edición es multiIdioma.
        /// </summary>
        public Dictionary<string, string> AvailableLanguages { get; set; }

        /// <summary>
        /// Título de la página configurado en el XML de configuración de la ontología.
        /// </summary>
        public string PageTitle { get; set; }

        /// <summary>
        /// Información extra de las características de los elementos para la edición del SEM CMS.
        /// </summary>
        public string AuxiliaryElementsFeaturesInfo { get; set; }

        /// <summary>
        /// RDF auxiliar para la edición del SEM CMS.
        /// </summary>
        public string AuxiliaryRDFInfo { get; set; }

        /// <summary>
        /// Herencias entre entidades para la edición del SEM CMS.
        /// </summary>
        public string AuxiliaryInheritancesInfo { get; set; }

        /// <summary>
        /// Información auxiliar acerca de los IDs de los controles para la edición del SEM CMS.
        /// </summary>
        public string AuxiliaryIDRegisterInfo { get; set; }

        /// <summary>
        /// Información auxiliar acerca de los nombre de las categorias de tesauros semánticos para la edición del SEM CMS.
        /// </summary>
        public string AuxiliaryCategoryTesSemNameInfo { get; set; }

        /// <summary>
        /// Información auxiliar acerca de los valores grafo dependientes para la edición del SEM CMS.
        /// </summary>
        public string AuxiliaryDependentGraphValuesInfo { get; set; }

        /// <summary>
        /// Información auxiliar acerca de las ontologías externas editables dentro del recurso para la edición del SEM CMS.
        /// </summary>
        public string AuxiliarySubOntologiesExtInfo { get; set; }

        /// <summary>
        /// Información auxiliar acerca de los IDs de las entidades en la edición del SEM CMS.
        /// </summary>
        public string AuxiliaryEntityIDRegisterInfo { get; set; }

        /// <summary>
        /// Indica si el SEMCMS debe pintarse encima del menú de la ficha del recurso.
        /// </summary>
        public bool SemCmsDrawOverMenu { get; set; }

        /// <summary>
        /// Indica si se debe ocultar el título del recurso GNOSS.
        /// </summary>
        public bool HideResourceTitle { get; set; }

        /// <summary>
        /// Título del recurso al que está vinculado el SEMCMS.
        /// </summary>
        public string DocumentTitle { get; set; }

        /// <summary>
        /// IDs separados por comas de los controles de las propiedades que están configuradas como título del recurso.
        /// </summary>
        public string TitlePropretyIDs { get; set; }

        /// <summary>
        /// IDs separados por comas de los controles de las propiedades que están configuradas como descripción del recurso.
        /// </summary>
        public string DescriptionPropretyIDs { get; set; }

        /// <summary>
        /// Error en la generación del SEM CMS para el administrador de la comunidad.
        /// </summary>
        public string AdminGenerationError { get; set; }

        /// <summary>
        /// Indica si alguna propiedad del SEM CMS tiene configurado el JCROP.
        /// </summary>
        public bool JcropAvailable { get; set; }

        /// <summary>
        /// Indica que hay alguna propiedad en la ontología en la que se muestra un selector de fecha con hora.
        /// </summary>
        public bool DateWithTimeAvailable { get; set; }

        /// <summary>
        /// Indica si el formulario es vitual, por lo que los datos RDF se guardarán en un servicio configurado externo a GNOSS.
        /// </summary>
        public bool VirtualForm { get; set; }

        /// <summary>
        /// Url del javascript de la ontología.
        /// </summary>
        public string OntologyJS { get; set; }

        /// <summary>
        /// Url del CSS de la ontología.
        /// </summary>
        public string OntologyCSS { get; set; }

        /// <summary>
        /// Url para realizar las acciones de MCV.
        /// </summary>
        public string MvcActionsUrl { get; set; }

        /// <summary>
        /// Indica si tiene agregado un mapa
        /// </summary>
        public bool MapAgregated { get; set; }

        #region Métodos ayuda vistas

        /// <summary>
        /// Obtiene una propiedad que contienen las entidades principales del SEMCMS.
        /// </summary>
        /// <param name="pName">Nombre de la propiedad</param>
        /// <returns></returns>
        public SemanticPropertyModel GetMainProperty(string pName)
        {
            foreach (SemanticEntityModel entModel in RootEntities)
            {
                SemanticPropertyModel propModel = entModel.GetProperty(pName);

                if (propModel != null)
                {
                    return propModel;
                }
            }

            return null;
        }

        /// <summary>
        /// Obtiene una propiedad que contienen alguna de las entidades del SEMCMS indicando el nivel de la propiedad por un path separando las propiedades por los caracteres '@@@'. Ejemplo: http://www.cidoc-crm.org/cidoc-crm#p14_carried_out_by@@@http://www.pradomuseum/201403#author@@@http://www.ecidoc/201403#p131_E82_p102_has_title.
        /// </summary>
        /// <param name="pPath">Path con los nombres de las propiedades</param>
        /// <returns>Propiedad que contienen alguna de las entidades del SEMCMS, en cualquier nivel de jerarquia de la propiedad indicado por un path</returns>
        public SemanticPropertyModel GetPropertyByPath(string pPath)
        {
            foreach (SemanticEntityModel entModel in RootEntities)
            {
                SemanticPropertyModel propModel = entModel.GetPropertyByPath(pPath);

                if (propModel != null)
                {
                    return propModel;
                }
            }

            return null;
        }

        public void HidePropertyByPath(string pPath)
        {
            foreach (SemanticEntityModel entModel in RootEntities)
            {
                SemanticPropertyModel propModel = entModel.GetPropertyByPath(pPath);
                if (propModel != null)
                {
                    propModel.Hidden = true;
                    return;
                }
            }

        }

        /// <summary>
        /// Obtiene el primer valor de la propiedad que contienen alguna de las entidades del SEMCMS indicando el nivel de la propiedad por un path separando las propiedades por los caracteres '@@@'. Ejemplo: http://www.cidoc-crm.org/cidoc-crm#p14_carried_out_by@@@http://www.pradomuseum/201403#author@@@http://www.ecidoc/201403#p131_E82_p102_has_title.
        /// </summary>
        /// <param name="pPath">Path con los nombres de las propiedades</param>
        /// <returns>Primer valor de la propiedad que contienen alguna de las entidades del SEMCMS, en cualquier nivel de jerarquia de la propiedad indicado por un path</returns>
        public string GetFirstValuePropertyByPath(string pPath)
        {
            SemanticPropertyModel propModel = null;
            foreach (SemanticEntityModel entModel in RootEntities)
            {
                propModel = entModel.GetPropertyByPath(pPath);

                if (propModel != null)
                {
                    break;
                }
            }

            if (propModel != null && propModel.FirstPropertyValue != null)
            {
                return propModel.FirstPropertyValue.Value;
            }

            return null;
        }

        /// <summary>
        /// Obtiene el RDFa de la propiedad que contienen alguna de las entidades del SEMCMS indicando el nivel de la propiedad por un path separando las propiedades por los caracteres '@@@'. Ejemplo: http://www.cidoc-crm.org/cidoc-crm#p14_carried_out_by@@@http://www.pradomuseum/201403#author@@@http://www.ecidoc/201403#p131_E82_p102_has_title.
        /// </summary>
        /// <param name="pPath">Path con los nombres de las propiedades</param>
        /// <returns>RDFa de la propiedad que contienen alguna de las entidades del SEMCMS, en cualquier nivel de jerarquia de la propiedad indicado por un path. Ejemplo: about="http://a..." property="dc:title".</returns>
        public string GetRDFAPropertyByPath(string pPath)
        {
            return GetRDFAProperty(GetPropertyByPath(pPath));
        }

        /// <summary>
        /// Obtiene el RDFa de una propiedad.
        /// </summary>
        /// <param name="pPropiedad">Propiedad</param>
        /// <returns>RDFa de la propiedad</returns>
        public string GetRDFAProperty(SemanticPropertyModel pPropiedad)
        {
            if (pPropiedad != null)
            {
                return pPropiedad.GetRDFA();
            }

            return null;
        }

        /// <summary>
        /// Obtiene el RDFa de la entidad.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <returns>RDFa de la entidad. Ejemplo: about="http://a..." typeof="http://b...".</returns>
        public string GetRDFAEntity(SemanticEntityModel pEntidad)
        {
            if (pEntidad != null)
            {
                return pEntidad.GetRDFA();
            }

            return null;
        }

        /// <summary>
        /// Obtiene los namespaces del RDFa del recurso.
        /// </summary>
        /// <returns>Namespaces del RDFa del recurso. Ejemplo: xmlns:arecipe="http://gnoss.co... xmlns:gnoss="http://gnos....</returns>
        public string GetRDFANamespaces()
        {
            string xmlns = "xmlns:" + OntologyNamespace + "=\"" + OntologyUrl + "\"";

            foreach (string keyName in OntologyNamespaces.Keys)
            {
                if (OntologyNamespaces[keyName] != "rdf" && OntologyNamespaces[keyName] != "owl" && OntologyNamespaces[keyName] != "xsd" && OntologyNamespaces[keyName] != "rdfs")
                {
                    xmlns = string.Concat(xmlns, " xmlns:", OntologyNamespaces[keyName], "=\"", keyName, "\"");
                }
            }

            xmlns = string.Concat(xmlns, "xmlns:sioc=\"http://rdfs.org/sioc/ns#\" xmlns:gnoss=\"http://gnoss.com/gnoss.owl#\"");

            return xmlns;
        }

        /// <summary>
        /// Obtiene una propiedad que contienen alguna de las entidades del SEMCMS, en cualquier nivel de jerarquia de la propiedad.
        /// </summary>
        /// <param name="pName">Nombre de la propiedad</param>
        /// <returns>Propiedad que contienen alguna de las entidades del SEMCMS, en cualquier nivel de jerarquia de la propiedad</returns>
        public SemanticPropertyModel GetPropertyAtAnyLevel(string pName)
        {
            return GetPropertyAtAnyLevel(pName, null);
        }

        /// <summary>
        /// Obtiene una propiedad que contienen alguna de las entidades del SEMCMS, en cualquier nivel de jerarquia de la propiedad.
        /// </summary>
        /// <param name="pName">Nombre de la propiedad</param>
        /// <param name="pEntityType">Tipo de la entidad que contiene la propiedad</param>
        /// <returns>Propiedad que contienen alguna de las entidades del SEMCMS, en cualquier nivel de jerarquia de la propiedad</returns>
        public SemanticPropertyModel GetPropertyAtAnyLevel(string pName, string pEntityType)
        {
            foreach (SemanticEntityModel entModel in RootEntities)
            {
                SemanticPropertyModel propModel = GetPropertyAtAnyLevel(pName, null, entModel);

                if (propModel != null)
                {
                    return propModel;
                }
            }

            return null;
        }

        /// <summary>
        /// Obtiene una propiedad que contienen alguna de las entidades del SEMCMS, en cualquier nivel de jerarquia de la propiedad.
        /// </summary>
        /// <param name="pName">Nombre de la propiedad</param>
        /// <param name="pEntityType">Tipo de la entidad que contiene la propiedad</param>
        /// <param name="pEntityModel">Entidad modelo sobre la que buscar</param>
        /// <returns>Propiedad que contienen alguna de las entidades del SEMCMS, en cualquier nivel de jerarquia de la propiedad</returns>
        public SemanticPropertyModel GetPropertyAtAnyLevel(string pName, string pEntityType, SemanticEntityModel pEntityModel)
        {
            if (pEntityModel != null && pEntityModel.Entity != null)
            {
                foreach (SemanticPropertyModel propiedad in pEntityModel.Properties)
                {
                    if (propiedad.OntologyPropInfo == null)
                    {
                        continue;
                    }

                    if ((propiedad.Element.Propiedad.Nombre == pName || propiedad.Element.Propiedad.NombreFormatoUri == pName) && EstiloPlantilla.EsPropiedadDeTipoEntidad(propiedad.Element.Propiedad, pEntityModel.Entity, pEntityType))
                    {
                        return propiedad;
                    }

                    if (propiedad.Element.Propiedad.Tipo == TipoPropiedad.ObjectProperty)
                    {
                        foreach (SemanticPropertyModel.PropertyValue propValue in propiedad.PropertyValues)
                        {
                            SemanticPropertyModel propAux = GetPropertyAtAnyLevel(pName, pEntityType, propValue.RelatedEntity);
                            if (propAux != null)
                            {
                                return propAux;
                            }
                        }
                    }
                }
            }

            return null;
        }

        #endregion
    }

    /// <summary>
    /// Modelo que representa una entidad del SEM CMS. Se corresponde con una entidad ontológica.
    /// </summary>
    [Serializable]
    public class SemanticEntityModel
    {
        #region Miembros

        /// <summary>
        /// Título que representa a la entidad actual según su tipo.
        /// </summary>
        private string mEntityNameForTitle;

        /// <summary>
        /// Modelo semántico del SEM CMS.
        /// </summary>
        public SemanticResourceModel mSemanticResourceModel;

        #endregion

        /// <summary>
        /// Modelo semántico del SEM CMS.
        /// </summary>
        public SemanticResourceModel SemanticResourceModel
        {
            get
            {
                return mSemanticResourceModel;
            }
            set
            {
                mSemanticResourceModel = value;
            }
        }

        /// <summary>
        /// Entidad semántica a la que representa el modelo.
        /// </summary>
        public ElementoOntologia Entity { get; set; }

        /// <summary>
        /// ID de la entidad.
        /// </summary>
        public string Key { get { return Entity.ID; } }

        /// <summary>
        /// Configuración de la plantilla para esta entidad.
        /// </summary>
        public EstiloPlantillaEspecifEntidad SpecificationEntity
        {
            get
            {
                return Entity.EspecifEntidad;
            }
        }

        /// <summary>
        /// Profunidad dentro de la jerarquia de entiades. Dicha profundidad se obtiene mediante su relación através de sus propiedades.
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Título que representa a la entidad actual según su tipo.
        /// </summary>
        public string EntityNameForTitle
        {
            get
            {
                if (mEntityNameForTitle == null)
                {
                    string posibleNombreTipoEntidad = SpecificationEntity.NombreEntidad(SemanticResourceModel.ReadMode);
                    mEntityNameForTitle = Entity.TipoEntidad;

                    if (posibleNombreTipoEntidad != null)
                    {
                        mEntityNameForTitle = posibleNombreTipoEntidad;
                    }
                    else
                    {
                        if (!Entity.Descripcion.Equals(Entity.TipoEntidad))
                        {
                            mEntityNameForTitle += " '" + Entity.Descripcion + "'";
                        }

                        mEntityNameForTitle = mEntityNameForTitle.Replace("_", " ");
                    }
                }

                return mEntityNameForTitle;
            }
            set
            {
                mEntityNameForTitle = value;
            }
        }

        /// <summary>
        /// Instancias de las subclases de la entidad actual.
        /// </summary>
        public List<SemanticEntityModel> SubEntities { get; set; }

        /// <summary>
        /// Instancia de la superclase de la entidad actual.
        /// </summary>
        public SemanticEntityModel SuperEntity { get; set; }

        /// <summary>
        /// En caso de que la entiad tenga subEntidades, si alguna está seleccionada se indica con esta propiedad. Si no será la primera por defecto.
        /// </summary>
        public SemanticEntityModel SelectedSubEntity { get; set; }

        /// <summary>
        /// Indica si el contenedor que representa la entiadad debe pintarse oculto. Por ejemplo si es una subEntidad no seleccionada.
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// Propiedades que contiene el modelo de la entidad actual.
        /// </summary>
        public List<SemanticPropertyModel> Properties { get; set; }

        /// <summary>
        /// Valor de typeof para el RDFa.
        /// </summary>
        public string TypeofRDFA { get; set; }

        /// <summary>
        /// Valor de about para el RDFa.
        /// </summary>
        public string AboutRDFA { get; set; }

        /// <summary>
        /// Mapa Google que representa la entidad. Sólo tendrá valor en el caso de que se configure que la entidad es un mapa Google.
        /// </summary>
        public GoogleMap GoogleMapInfo { get; set; }

        /// <summary>
        /// Indica que se debe mostrar en modo lectura.
        /// </summary>
        public bool ReadMode { get; set; }

        /// <summary>
        /// Clase que representa un mapa de Google.
        /// </summary>
        [Serializable]
        public class GoogleMap
        {
            /// <summary>
            /// Latitud del mapa. Si hay latitud debe haber logitud. Habiendo estas se descarta el valor de ruta y color.
            /// </summary>
            public string Latitude { get; set; }

            /// <summary>
            /// Longitud del mapa. Si hay logitud debe haber latitud. Habiendo estas se descarta el valor de ruta y color.
            /// </summary>
            public string Longitude { get; set; }

            /// <summary>
            /// Ruta del mapa. Solo se aplica si la latitud y logitud son nulos.
            /// </summary>
            public string Route { get; set; }

            /// <summary>
            /// Color de la ruta del mapa. Solo es aplicable si hay ruta.
            /// </summary>
            public string RouteColor { get; set; }

            /// <summary>
            /// Clave del API Javascript de Google.
            /// </summary>
            public string JsApiGoogleKey { get; set; }
        }

        #region Métodos ayuda vistas

        /// <summary>
        /// Obtiene la propiedad de la entidad actual que coincide con un nombre.
        /// </summary>
        /// <param name="pName">Nombre de la propiedad</param>
        /// <returns>Propiedad de la entidad actual que coincide con un nombre</returns>
        public SemanticPropertyModel GetProperty(string pName)
        {
            foreach (SemanticPropertyModel propModel in Properties)
            {
                if (propModel.OntologyPropInfo != null)
                {
                    if (propModel.Element.Propiedad.Nombre == pName || propModel.Element.Propiedad.NombreFormatoUri == pName)
                    {
                        return propModel;
                    }
                }
                else
                {
                    SemanticPropertyModel propHija = propModel.GetProperty(pName);

                    if (propHija != null)
                    {
                        return propHija;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Obtiene una propiedad que contienen alguna de las entidades del SEMCMS indicando el nivel de la propiedad por un path separando las propiedades por los caracteres '@@@'. Ejemplo: http://www.cidoc-crm.org/cidoc-crm#p14_carried_out_by@@@http://www.pradomuseum/201403#author@@@http://www.ecidoc/201403#p131_E82_p102_has_title.
        /// </summary>
        /// <param name="pPath">Path con los nombres de las propiedades</param>
        /// <returns>Propiedad que contienen alguna de las entidades del SEMCMS, en cualquier nivel de jerarquia de la propiedad indicado por un path</returns>
        public SemanticPropertyModel GetPropertyByPath(string pPath)
        {
            if (pPath.Contains("@@@"))
            {
                string nombrePropActual = pPath.Substring(0, pPath.IndexOf("@@@"));
                SemanticPropertyModel propRelacion = GetProperty(nombrePropActual);

                if (propRelacion != null && propRelacion.Element.Propiedad.Tipo == TipoPropiedad.ObjectProperty && propRelacion.PropertyValues.Count > 0)
                {
                    return propRelacion.FirstPropertyValue.RelatedEntity.GetPropertyByPath(pPath.Substring(pPath.IndexOf("@@@") + 3));
                }
            }
            else
            {
                return GetProperty(pPath);
            }

            return null;
        }


        /// <summary>
        /// Obtiene el primer valor de la propiedad que contienen alguna de las entidades del SEMCMS indicando el nivel de la propiedad por un path separando las propiedades por los caracteres '@@@'. Ejemplo: http://www.cidoc-crm.org/cidoc-crm#p14_carried_out_by@@@http://www.pradomuseum/201403#author@@@http://www.ecidoc/201403#p131_E82_p102_has_title.
        /// </summary>
        /// <param name="pPath">Path con los nombres de las propiedades</param>
        /// <returns>Primer valor de la propiedad que contienen alguna de las entidades del SEMCMS, en cualquier nivel de jerarquia de la propiedad indicado por un path</returns>
        public string GetFirstValuePropertyByPath(string pPath)
        {
            SemanticPropertyModel propModel = GetPropertyByPath(pPath);

            if (propModel != null && propModel.FirstPropertyValue != null)
            {
                return propModel.FirstPropertyValue.Value;
            }

            return null;
        }

        /// <summary>
        /// Obtiene el RDFa de la propiedad que contienen alguna de las entidades del SEMCMS indicando el nivel de la propiedad por un path separando las propiedades por los caracteres '@@@'. Ejemplo: http://www.cidoc-crm.org/cidoc-crm#p14_carried_out_by@@@http://www.pradomuseum/201403#author@@@http://www.ecidoc/201403#p131_E82_p102_has_title.
        /// </summary>
        /// <param name="pPath">Path con los nombres de las propiedades</param>
        /// <returns>RDFa de la propiedad que contienen alguna de las entidades del SEMCMS, en cualquier nivel de jerarquia de la propiedad indicado por un path. Ejemplo: about="http://a..." property="dc:title".</returns>
        public string GetRDFAPropertyByPath(string pPath)
        {
            return SemanticResourceModel.GetRDFAProperty(GetPropertyByPath(pPath));
        }

        /// <summary>
        /// Obtiene el RDFa de la entidad.
        /// </summary>
        /// <returns>RDFa de la entidad. Ejemplo: about="http://a..." typeof="http://b...".</returns>
        public string GetRDFA()
        {
            if (AboutRDFA != null && TypeofRDFA != null)
            {
                return string.Concat("about=\"", AboutRDFA, "\" typeof=\"", TypeofRDFA, "\"");
            }

            return null;
        }

        #endregion
    }

    /// <summary>
    /// Modelo que representa una propiedad que posee un modelo de entidad de la ontología. Puede ser un propiedad ontológica, un grupo de propiedades, un literal, ect.
    /// </summary>
    [Serializable]
    public class SemanticPropertyModel
    {
        /// <summary>
        /// Elemento de la propiedad.
        /// </summary>
        public ElementoOrdenado Element { get; set; }

        /// <summary>
        /// Modelo de entidad padre del modelo actual.
        /// </summary>
        public SemanticEntityModel EntityParent { get; set; }

        /// <summary>
        /// Profunidad dentro de la jerarquia de entiades y propiedades. Dicha profundidad se obtiene mediante su relación através de sus propiedades.
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Indica si el contenedor que representa la propiedad debe pintarse oculto. Por ejemplo si es un grupo dentro de un selector de grupos y no está visible.
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// Propiedades que contiene el modelo de la propiedad actual.
        /// </summary>
        public List<SemanticPropertyModel> Properties { get; set; }

        /// <summary>
        /// Grupo que representa la propiedad actual. Sólo tendrá valor en el caso de que la propiedad actual sea un grupo.
        /// </summary>
        public Group GroupInfo { get; set; }

        /// <summary>
        /// Literal que representa la propiedad actual. Sólo tendrá valor en el caso de que la propiedad actual sea un Literal.
        /// </summary>
        public Literal LiteralInfo { get; set; }

        /// <summary>
        /// Dato especial que representa la propiedad actual. Sólo tendrá valor en el caso de que la propiedad actual sea un dato especial.
        /// </summary>
        public EspecialProp EspecialPropInfo { get; set; }

        /// <summary>
        /// Propiedad ontológica que representa la propiedad actual. Sólo tendrá valor en el caso de que la propiedad actual sea una propiedad ontológica.
        /// </summary>
        public OntologyProp OntologyPropInfo { get; set; }

        /// <summary>
        /// Indica que se debe mostrar en modo lectura.
        /// </summary>
        public bool ReadMode { get; set; }

        /// <summary>
        /// Especificación de la propiedad semántica actual. Sólo tendrá valor en el caso de que la propiedad actual sea una propiedad ontológica.
        /// </summary>
        public EstiloPlantillaEspecifProp SpecificationProperty
        {
            get
            {
                return Element.Propiedad?.EspecifPropiedad;
            }
        }

        /// <summary>
        /// Clase que aglutina información del grupo, que es la propiedad actual.
        /// </summary>
        [Serializable]
        public class Group
        {
            /// <summary>
            /// Indica si se debe incluir el icono de GNOSS en el grupo. Si el grupo es de tipo "titulo" puede incluirlo.
            /// </summary>
            public bool IncludeGnossIcon { get; set; }

            /// <summary>
            /// Indica que el icono de Gnoss debe reflejar que el recurso es privado para editores y lectores.
            /// </summary>
            public bool GnossIconIsPrivate { get; set; }

            /// <summary>
            /// Nombre semántico para el icono GNOSS.
            /// </summary>
            public string GnossIconName { get; set; }

            /// <summary>
            /// Nombre del grupo.
            /// </summary>
            public string GroupName { get; set; }

            /// <summary>
            /// Clase del nombre del grupo.
            /// </summary>
            public string GroupNameClass { get; set; }

            /// <summary>
            /// Etiqueta HTML del nombre del grupo.
            /// </summary>
            public string GroupNameTag { get; set; }
        }

        /// <summary>
        /// Clase que aglutina información del literal, que es la propiedad actual.
        /// </summary>
        [Serializable]
        public class Literal
        {
            /// <summary>
            /// Link que se debe agregar al literal.
            /// </summary>
            public string LiteralLink { get; set; }

            /// <summary>
            /// Texto del literal.
            /// </summary>
            public string LiteralName { get; set; }
        }

        /// <summary>
        /// Clase que aglutina información del dato especial, que es la propiedad actual.
        /// </summary>
        [Serializable]
        public class EspecialProp
        {
            #region Especial Faceta

            /// <summary>
            /// Nombre de la faceta del dato especial.
            /// </summary>
            public string Facet { get; set; }

            /// <summary>
            /// Grafo para la faceta del dato especial.
            /// </summary>
            public string Graph { get; set; }

            /// <summary>
            /// Parámetros para la búsqueda del dato especial.
            /// </summary>
            public string Parameters { get; set; }

            #endregion

            #region Especial Botón

            /// <summary>
            /// ID de la acción del botón.
            /// </summary>
            public string ActionID { get; set; }

            /// <summary>
            /// Nombre del botón.
            /// </summary>
            public string ButtonName { get; set; }

            /// <summary>
            /// ID de la entidad de la acción.
            /// </summary>
            public string ActionEntityID { get; set; }

            #endregion
        }

        /// <summary>
        /// Clase que aglutina información de la propiedad ontológica, que es la propiedad actual.
        /// </summary>
        [Serializable]
        public class OntologyProp
        {
            /// <summary>
            /// Devuelve el tipo de campo de la propiedad de una ontología.
            /// </summary>
            public TipoCampoOntologia FieldType { get; set; }

            /// <summary>
            /// ID del control que representa a la propiedad.
            /// </summary>
            public string ControlID { get; set; }

            /// <summary>
            /// Valores de la propiedad ontológica.
            /// En el caso de ser propiedad de tipo Tesauro Semántico en su vista por defecto, contiene los nombre de las categorías ordenadas de padré a último hijo de la jerarquía.
            /// </summary>
            public List<PropertyValue> PropertyValues { get; set; }

            /// <summary>
            /// Valores multiIdioma de la propiedad ontológica.
            /// </summary>
            public Dictionary<string, List<PropertyValue>> PropertyLanguageValues { get; set; }

            /// <summary>
            /// Grafo para una propiedad grafo dependiente. Solo se aplica a Propiedades ontológicas configuradas como GrafoDependiente.
            /// </summary>
            public string PropDependentGraph { get; set; }

            /// <summary>
            /// Valor del control auxiliar de la propiedad ontológica. El control auxiliar es necesario par propiedades como las GrafoDependiente.
            /// </summary>
            public string AuxiliaryControlValue { get; set; }

            /// <summary>
            /// Indica si el control auxiliar de la propiedad ontológica debe estar deshabilitado por defecto. El control auxiliar es necesario par propiedades como las GrafoDependiente.
            /// </summary>
            public bool AuxiliaryControlDisabled { get; set; }

            /// <summary>
            /// Valor por defecto no seleccionable.
            /// </summary>
            public string DefaultUnselectableValue { get; set; }

            /// <summary>
            /// Título para el label de la propiedad.
            /// </summary>
            public string LabelTitle { get; set; }

            /// <summary>
            /// Texto de ayuda de la propiedad.
            /// </summary>
            public string HelpText { get; set; }

            /// <summary>
            /// URL about del RDFA de la propiedad.
            /// </summary>
            public string AboutRDFA { get; set; }

            /// <summary>
            /// URL property del RDFA de la propiedad.
            /// </summary>
            public string PropertyRDFA { get; set; }

            /// <summary>
            /// Indica si la propiedad solo puede tener un solo valor.
            /// </summary>
            public bool UniqueValue { get; set; }

            public int MinCardinality { get; set; }
            public int MaxCardinality { get; set; }

            public bool FunctionalProperty { get; set; }

            /// <summary>
            /// Indica si la propiedad admite más valores de los que tiene.
            /// </summary>
            public bool ItIsPossibleToSddMoreValues { get; set; }

            /// <summary>
            /// Titulos de las entidades representantes de una propiedad de tipo objeto multiple.
            /// </summary>
            public List<string> RepresentativeEntityTitles { get; set; }

            /// <summary>
            /// Selector de entidad de un propiedad de tipo Objeto con un selector de entidad configurado.
            /// </summary>
            public EntitySelector EntitySelector { get; set; }

            /// <summary>
            /// Indica si la propiedad es multiitioma.
            /// </summary>
            public bool MultiLanguage { get; set; }

            /// <summary>
            /// Indica si la propiedad es multiitioma.
            /// </summary>
            public bool MultiLanguageWithTabs { get; set; }

            /// <summary>
            /// Número de entidades auxiliares por página o 0 si no hay paginación.
            /// </summary>
            public int NumEntitiesForPage { get; set; }

            /// <summary>
            /// Número total de entidades auxiliares para paginar o 0 si no hay paginación.
            /// </summary>
            public int TotalEntitiesPagination { get; set; }
        }

        /// <summary>
        /// Clase que representa el valor de una propiedad.
        /// </summary>
        [Serializable]
        public class PropertyValue
        {
            /// <summary>
            /// Texto con el valor de la propiedad ontológica.
            /// </summary>
            public string Value { get; set; }

            /// <summary>
            /// Idioma del valor seleccionado, NULL si no tiene idioma.
            /// </summary>
            public string LanguageOfValue { get; set; }

            /// <summary>
            /// Modelo de la entidad relacionado con la propiedad. Solo puede tener valor para propiedades de tipo Objeto.
            /// </summary>
            public SemanticEntityModel RelatedEntity { get; set; }

            /// <summary>
            /// URL de descarga del archivo adjunto a la propiedad. Solo se aplica a Propiedades ontológicas configuradas como adjunto (Archivo, ArchivoLink) o Link o una normal configurada con UrlLinkDelValor TRUE.
            /// </summary>
            public string DownloadUrl { get; set; }

            /// <summary>
            /// Url para el objeto embebido de Youtube. Solo se aplica a Propiedades ontológicas configuradas como EmbebedLink (Youtube, Vimeo y Slideshare).
            /// </summary>
            public string EmbebedLinkYoutube { get; set; }

            /// <summary>
            /// Url para el objeto embebido de Vimeo. Solo se aplica a Propiedades ontológicas configuradas como EmbebedLink (Youtube, Vimeo y Slideshare).
            /// </summary>
            public string EmbebedLinkVimeo { get; set; }

            /// <summary>
            /// Url para el objeto embebido de Slideshare. Solo se aplica a Propiedades ontológicas configuradas como EmbebedLink (Youtube, Vimeo y Slideshare).
            /// </summary>
            public string EmbebedLinkSlideshare { get; set; }

            /// <summary>
            /// Html del objeto embebido. Solo se aplica a Propiedades ontológicas configuradas como EmbebedObject.
            /// </summary>
            public string EmbebedObject { get; set; }

            /// <summary>
            /// Url para el control auxiliar link que debe envolver la propiedad principal.
            /// </summary>
            public string UrlAuxiliaryLinkControl { get; set; }

            /// <summary>
            /// Propiedad a la que pertenece el valor.
            /// </summary>
            public SemanticPropertyModel Property { get; set; }

            /// <summary>
            /// Titulos representantes de la entidad de esta propiedad. Solo aplicable si la propiedad es de tipo objeto multiple.
            /// </summary>
            public List<string> RepresentativeEntityTitles { get; set; }

            /// <summary>
            /// Valores hijos de un valor que es una categoría semántica. Solo se aplica el valor pertenece a una propiedad configurada como un selector de entidad de tipo tesuaro semántico y tipo de presentación árbol.
            /// </summary>
            public List<PropertyValue> ThesaurusSemanticTreeChildren { get; set; }
        }

        /// <summary>
        /// Clase que representa el selector de entidad de un propiedad de tipo Objeto con un selector de entidad configurado.
        /// </summary>
        [Serializable]
        public class EntitySelector
        {
            /// <summary>
            /// Valores de las entiadades del selector para la edición.
            /// </summary>
            public Dictionary<string, string> EditionEntitiesValues { get; set; }

            /// <summary>
            /// Título extra para el control de autocompletar del selector de entidad.
            /// </summary>
            public string ExtraTitleAutoComplete { get; set; }

            /// <summary>
            /// Grafo del selector de entidad.
            /// </summary>
            public string Graph { get; set; }

            /// <summary>
            /// Url de la entidad solicitada para el selector de entidad.
            /// </summary>
            public string EntityRequestedUrl { get; set; }

            /// <summary>
            /// Url de la propiedad que enlaza con la entidad solicitada para el selector de entidad.
            /// </summary>
            public string PropertyRequestedUrl { get; set; }

            /// <summary>
            /// Url del tipo de la entidad solicitada para el selector de entidad.
            /// </summary>
            public string EntityTypeRequestedUrl { get; set; }

            /// <summary>
            /// Propiedades de edición de la entidad.
            /// </summary>
            public List<string> EditionProperties { get; set; }

            /// <summary>
            /// Extra para el where del control de autocompletar del selector de entidad.
            /// </summary>
            public string ExtraWhereAutoComplete { get; set; }

            /// <summary>
            /// Idioma del selector de entidad.
            /// </summary>
            public string Language { get; set; }

            /// <summary>
            /// Título y wheres extras adicionales para el selector.
            /// </summary>
            public List<KeyValuePair<string, string>> AdditionalExtraTitleWhereAutoCompletes { get; set; }

            /// <summary>
            /// IDs de entidades que tienen hijos.
            /// </summary>
            public List<string> EntitiesWithChildren { get; set; }

            /// <summary>
            /// Valor ya agregado al tesauro semántico. Key: Valor original. Value: Valor tratado para la presentación al usuario.
            /// </summary>
            public KeyValuePair<string, string> SemanticThesaurusAddedValue { get; set; }

            /// <summary>
            /// Lista con los IDs y tipos entidad que dependen de la propiedad selector actual.
            /// </summary>
            public List<KeyValuePair<string, string>> DependentProperties { get; set; }

            #region Edicion Selector Personas y Grupos

            /// <summary>
            /// ID de la organización del perfil de usuario para el selector de usuarios y grupos.
            /// </summary>
            public string OrganizationID { get; set; }

            /// <summary>
            /// Tipo de consulta para el selector de usuarios y grupos.
            /// </summary>
            public string QueryType { get; set; }

            #endregion

            #region Lectura

            /// <summary>
            /// Recursos vinculados al selector de entidad, en el caso de que el selector sea de tipo "UrlRecurso".
            /// </summary>
            public List<ResourceLinkedToEntitySelector> LinkedResources { get; set; }

            /// <summary>
            /// Número de entidades por página o 0 si no hay paginación.
            /// </summary>
            public int NumEntitiesForPage { get; set; }

            /// <summary>
            /// Número total de entidades para paginar o 0 si no hay paginación.
            /// </summary>
            public int TotalEntitiesPagination { get; set; }

            #endregion
        }

        /// <summary>
        /// Clase que representa a los recursos vinculados a un selector de entidad.
        /// </summary>
        [Serializable]
        public class ResourceLinkedToEntitySelector
        {
            /// <summary>
            /// Key del recurso
            /// </summary>
            public Guid Key { get; set; }

            /// <summary>
            /// Link del recurso
            /// </summary>
            public string Link { get; set; }

            /// <summary>
            /// Título del recurso.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Texto del label título para el recurso.
            /// </summary>
            public string TitleLabel { get; set; }

            /// <summary>
            /// URl de la imagen del recurso.
            /// </summary>
            public string ImageUrl { get; set; }

            /// <summary>
            /// Texto del label imagen para el recurso.
            /// </summary>
            public string ImageUrlLabel { get; set; }

            /// <summary>
            /// Descripción del recurso.
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Texto del label descripción para el recurso.
            /// </summary>
            public string DescriptionLabel { get; set; }

            /// <summary>
            /// Autores del recurso. Key: Nombre del autor. Value: Link del autor.
            /// </summary>
            public Dictionary<string, string> Authors { get; set; }

            /// <summary>
            /// Texto del label autores para el recurso.
            /// </summary>
            public string AuthorsLabel { get; set; }
        }

        #region Propiedades auxiliares vistas

        /// <summary>
        /// Valores de la propiedad ontológica. Solo tendrá valores si la propiedad es ontológica y ésta tiene valores.
        /// En el caso de ser propiedad de tipo Tesauro Semántico en su vista por defecto, contiene los nombre de las categorías ordenadas de padré a último hijo de la jerarquía.
        /// </summary>
        public List<PropertyValue> PropertyValues
        {
            get
            {
                if (OntologyPropInfo != null)
                {
                    return OntologyPropInfo.PropertyValues;
                }
                else
                {
                    return new List<PropertyValue>();
                }
            }
        }

        /// <summary>
        /// Primer valor de la propiedad ontológica. Solo tendrá valor si la propiedad es ontológica y ésta tiene algún valor.
        /// En el caso de ser propiedad de tipo Tesauro Semántico en su vista por defecto, contiene los nombre de las categorías ordenadas de padré a último hijo de la jerarquía.
        /// </summary>
        public PropertyValue FirstPropertyValue
        {
            get
            {
                if (OntologyPropInfo?.PropertyValues?.Count > 0)
                {
                    return OntologyPropInfo.PropertyValues[0];
                }
                return null;
            }
        }

        /// <summary>
        /// Obtiene el RDFa de la entidad.
        /// </summary>
        /// <returns>RDFa de la entidad. Ejemplo: about="http://a..." typeof="http://b...".</returns>
        public string GetRDFA()
        {
            if (OntologyPropInfo.AboutRDFA != null && OntologyPropInfo.PropertyRDFA != null)
            {
                string rdfa = string.Concat("about=\"", OntologyPropInfo.AboutRDFA, "\" ");

                if (Element.Propiedad.Tipo == TipoPropiedad.ObjectProperty || SpecificationProperty.TipoCampo == TipoCampoOntologia.Imagen)
                {
                    rdfa = string.Concat(rdfa, "rel=\"", OntologyPropInfo.PropertyRDFA, "\"");
                }
                else
                {
                    rdfa = string.Concat(rdfa, "property=\"", OntologyPropInfo.PropertyRDFA, "\"");
                }

                return rdfa;
            }

            return null;
        }

        /// <summary>
        /// Obtiene la propiedad de la entidad actual que coincide con un nombre.
        /// </summary>
        /// <param name="pName">Nombre de la propiedad</param>
        /// <returns>Propiedad de la entidad actual que coincide con un nombre</returns>
        public SemanticPropertyModel GetProperty(string pName)
        {
            if (OntologyPropInfo == null && Properties != null)
            {
                foreach (SemanticPropertyModel propModel in Properties)
                {
                    if (propModel.OntologyPropInfo != null)
                    {
                        if (propModel.Element.Propiedad.Nombre == pName || propModel.Element.Propiedad.NombreFormatoUri == pName)
                        {
                            return propModel;
                        }
                    }
                    else
                    {
                        SemanticPropertyModel propHija = propModel.GetProperty(pName);

                        if (propHija != null)
                        {
                            return propHija;
                        }
                    }
                }
            }

            return null;
        }

        #endregion
    }

    /// <summary>
    /// Modelo para la acción de obtener los hijos de una categoría de un tesauro semántico.
    /// </summary>
    [Serializable]
    public class SemanticThesaurusCategoryChildren
    {
        /// <summary>
        /// Grafo que contiene el tesauro semántico".
        /// </summary>
        public string Graph { get; set; }

        /// <summary>
        /// Uri de la categoría del tesauro semántico pulsada".
        /// </summary>
        public string CategoryUri { get; set; }

        /// <summary>
        /// Nombre de la propiedad que relaciona las categorías.
        /// </summary>
        public string RequestedProperty { get; set; }

        /// <summary>
        /// Nombre de la propiedad que contiene el ID de las categorías.
        /// </summary>
        public string CategoryIdProperty { get; set; }

        /// <summary>
        /// Nombre de la propiedad que representa el nombre de las categorías.
        /// </summary>
        public string PropertyName { get; set; }
    }

    /// <summary>
    /// Modelo para la acción de guardar un Jcrop.
    /// </summary>
    [Serializable]
    public class SaveJcrop
    {
        /// <summary>
        /// Coordenada X del Jcrop.
        /// </summary>
        public int XCoord { get; set; }

        /// <summary>
        /// Coordenada Y del Jcrop.
        /// </summary>
        public int YCoord { get; set; }

        /// <summary>
        /// Ancho del Jcrop.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Alto del Jcrop.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Ruta de la imagen del Jcrop.
        /// </summary>
        public string ImgSrc { get; set; }

        /// <summary>
        /// Información extra del Jcrop.
        /// </summary>
        public string Extra { get; set; }
    }

    /// <summary>
    /// Modelo para la aceptación de los archivos de la carga masiva.
    /// </summary>
    public class SaveMasiveLoadFiles
    {
        /// <summary>
        /// Información extra acerca de los archivos.
        /// </summary>
        public string InfoFiles { get; set; }

        /// <summary>
        /// Información extra acerca de los recursos masivos ya publicados.
        /// </summary>
        public string InfoFilesAlreadyPublished { get; set; }
    }

    /// <summary>
    /// Modelo para la acción de obtener un tesauro de una base de recursos para el Add To Gnoss.
    /// </summary>
    public class GetThesauroAddToGnoss
    {
        /// <summary>
        /// ID de la base de recursos de la que se quiere obtener el tesauro.
        /// </summary>
        public Guid ResourceSpaceID { get; set; }
    }

    /// <summary>
    /// Modelo para la acción de obtención de selectores de entidad dependientes.
    /// </summary>
    public class GetDependentSelectorsEntities
    {
        /// <summary>
        /// Nombre de la propiedad de la que dependen otras.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Tipo de la entidad a la que pertenece la propiedad de la que dependen otras.
        /// </summary>
        public string EntityType { get; set; }

        /// <summary>
        /// Valor de la propiedad de la que dependen otras.
        /// </summary>
        public string PropertyValue { get; set; }
    }
}
