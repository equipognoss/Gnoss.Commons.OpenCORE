﻿using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.IntegracionContinua;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Util;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models
{
    /// <summary>
    /// Modelo de comunidad
    /// </summary>
    [Serializable]
    public partial class CommunityModel : ObjetoBuscadorModel
    {
        private bool mDeshabilitarGuardarAdministracionIC;
        
        public string NameMetaComunidad { get; set; }
        public const int LastCacheVersion = 3;

        

		/// <summary>
		/// Enumeración para distinguir tipos de proyectos
		/// </summary>
		public enum TypeProyect
        {
            /// <summary>
            /// Proyecto de organización
            /// </summary>
            DeOrganizacion = 0,
            /// <summary>
            /// Comunidad web
            /// </summary>
            Comunidad = 1,
            /// <summary>
            /// Metacomunidad
            /// </summary>
            MetaComunidad = 2,
            /// <summary>
            /// Universidad 2.0
            /// </summary>
            Universidad20 = 3,
            /// <summary>
            /// Educacion Expandida
            /// </summary>
            EducacionExpandida = 4,
            /// <summary>
            /// Educacion Expandida
            /// </summary>
            Catalogo = 5,
            /// <summary>
            /// Educación primaria
            /// </summary>
            EducacionPrimaria = 6,
            /// <summary>
            /// Catalogo no social publico con un unico tipo de recurso
            /// </summary>
            CatalogoNoSocialConUnTipoDeRecurso = 7,
            /// <summary>
            /// Catalogo no social
            /// </summary>
            CatalogoNoSocial = 8
        }

        /// <summary>
        /// Enumeración para distinguir tipos de acceso
        /// </summary>
        public enum TypeAccessProject
        {
            /// <summary>
            /// Proyecto público
            /// </summary>
            Public = 0,
            /// <summary>
            /// Proyecto privado
            /// </summary>
            Private = 1,
            /// <summary>
            /// Proyecto restringido
            /// </summary>
            Restricted = 2,
            /// <summary>
            /// Proyecto reservado
            /// </summary>
            Reserved = 3
        }

        /// <summary>
        /// Enumeración para distinguir tipos de estado
        /// </summary>
        public enum StateProject
        {
            /// <summary>
            /// Proyecto bloqueado/cerrado
            /// </summary>
            Close = 0,
            /// <summary>
            /// Proyecto bloqueado temporalmente x mantenimiento
            /// </summary>
            CloseTemporaly = 1,
            /// <summary>
            /// Proyecto en definicion
            /// </summary>
            Definition = 2,
            /// <summary>
            /// Proyecto abierto
            /// </summary>
            Open = 3,
            /// <summary>
            /// Proyecto que se esta cerrando y esta unos dias en estado de gracia
            /// </summary>
            Closing = 4
        }

        /// <summary>
        /// Identificador de la comunidad
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Nombre de la comunidad
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Nombre corto de la comunidad
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Nombre corto para presentar la comunidad
        /// </summary>
        public string PresentationName { get; set; }

        /// <summary>
        /// Numero de recursos publicos de la comunidad
        /// </summary>
        public int NumberOfResources { get; set; }

        /// <summary>
        /// Número de personas miembros de la comunidad
        /// </summary>
        public int NumberOfPerson { get; set; }

        /// <summary>
        /// Número de organizaciones miembros de la organización
        /// </summary>
        public int NumberOfOrganizations { get; set; }

        /// <summary>
        /// Descripción de la comunidad
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Ruta de la comunidad
        /// </summary>
        public string Url { get; set; }

        public string UrlMyGnoss { get; set; }

        public string UrlPropia { get; set; }

        /// <summary>
        /// Logotipo de la comunidad
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// Etiquetas de la comunidad
        /// </summary>
        public List<string> Tags { get; set; }
        /// <summary>
        /// Registro automático
        /// </summary>
        public bool RegistroAutomatico { get; set; }

        /// <summary>
        /// Categorias de la comunidad
        /// </summary>
        public List<CategoryModel> Categories { get; set; }

        /// <summary>
        /// Tipo de proyecto
        /// </summary>
        public TypeProyect ProyectType { get; set; }

        /// <summary>
        /// Estado del proyecto
        /// </summary>
        public StateProject ProjectState { get; set; }

        /// <summary>
        /// Fecha de apertura de la comunidad en caso de cerrada
        /// </summary>
        public DateTime OpenDate { get; set; }

        /// <summary>
        /// Periodo de gracia de la comunidad en caso de cerrandose
        /// </summary>
        public DateTime PeriodoDeGracia { get; set; }

        /// <summary>
        /// Tipo de acceso del proyecto: privado, publico....
        /// </summary>
        public TypeAccessProject AccessType { get; set; }

        /// <summary>
        /// Pestañas configuradas en el proyecto
        /// </summary>
        public List<TabModel> Tabs { get; set; }

        /// <summary>
        /// Versión de la hoja de estilos propia de la comunidad
        /// </summary>
        public int? VersionCSS { get; set; }

        /// <summary>
        /// Versión del fichero javascript de la comunidad
        /// </summary>
        public int? VersionJS { get; set; }

        /// <summary>
        /// Versión del fichero javascript de la administración de la comunidad
        /// </summary>
        public int? VersionJSAdmin { get; set; }

        /// <summary>
        /// Versión de la hoja de estilos de la administración de la comunidad
        /// </summary>
        public int? VersionCSSAdmin { get; set; }

        /// <summary>
        /// Indica si es una comunidad GNOSS, Un tipo de comunidad especial
        /// </summary>
        public bool GNOSSCommunity { get; set; }

        /// <summary>
        /// Indica si es una comunidad GNOSS, Un tipo de comunidad especial
        /// </summary>
        public bool MetaProyect { get; set; }

        /// <summary>
        /// Permisos del usuario en la comunidad
        /// </summary>
        public PermissionsModel Permissions { get; set; }

        /// <summary>
        /// Indica si es una comunidad de Gnoss Organiza, un tipo de comunidad especial
        /// </summary>
        public bool IsGnossOrganiza { get; set; }

        /// <summary>
        /// Indica si hay que mostrar al usuario el aviso de que usamos cookies
        /// </summary>
        public bool CookieNotice { get; set; }

        /// <summary>
        /// Url externa del aviso de cookies
        /// </summary>
        public string ExternalUrlCookieNotice { get; set; }

        /// <summary>
        /// Texto del copyright
        /// </summary>
        public string Copyright { get; set; }

        /// <summary>
        /// Indica si la comunidad tiene cláusulas de registro
        /// </summary>
        public bool ClausesRegister { get; set; }

        /// <summary>
        /// Enlace a la pagina de contacto de la comunidad
        /// </summary>
        public string ContactLink { get; set; }

        /// <summary>
        /// Texto configurable para indicar que la comunidad esta en beta
        /// </summary>
        public string BetaProject { get; set; }

        /// <summary>
        /// Indica si la comunidad tiene configurado un menu compactado
        /// </summary>
        public bool CompactedMenu { get; set; }

        /// <summary>
        /// Lista de los perfiles de los administradores
        /// </summary>
        public List<ProfileModel> Administrators { get; set; }

        /// <summary>
        /// Lista de los niveles de certificaciones de la comunidad
        /// </summary>
        public List<string> CertificationLevels { get; set; }

        ///<summary>
        /// Indica si la integración continua está o no activada
        /// </summary>
        public Boolean IntegracionContinuaActivada { get; set; }

        /// <summary>
        /// Rama en la que estamos trabajando en Git
        /// </summary>
        public string RamaActualGit { get; set; }

        ///<summary>
        /// Indice la versión de la rama release
        ///</summary>
        public string VersionRamaRelease { get; set; }

        /// <summary>
        /// Indica si la integración continua está activada pero no se está trabajando en ninguna rama
        /// </summary>
        public bool IntContActivadaSinRamaEnUso { get; set; }

        /// <summary>
        /// Indica si la integración continua está activada pero no se está trabajando en ninguna rama
        /// </summary>
        public bool UsuarioDadoAltaIntCont { get; set; }

        /// <summary>
        /// Indica si estamos en el entorno de preproducción
        /// </summary>
        public bool EntornoEsPre { get; set; }

        /// <summary>
        /// Indica si estamos en el entorno de preproducción
        /// </summary>
        public bool EntornoEsPro { get; set; }

        /// <summary>
        /// Indica si el entorno actual esta bloqueado
        /// </summary>
        public bool EntornoBloqueado { get; set; }

        /// <summary>
        /// Indica si el entorno actual esta bloqueado
        /// </summary>
        public Guid? ParentKey { get; set; }

        /// <summary>
        /// Pestañas de una comunidad, para la metabusqueda
        /// </summary>
        public List<PestaniaModelMetabusqueda> ModelMetabusqueda {get;set;}

        /// <summary>
        /// Pestañas de una comunidad, para la metabusqueda
        /// </summary>
         public string ModelMetabusquedaJson {get;set;}

        /// <summary>
        /// Rdf types no buscables
        /// </summary>
        public string RdfTypesExcluidos { get; set; }

        /// <summary>
        /// Modelo de pestañas de una comunidad, para la metabusqueda
        /// </summary>
        [Serializable]
        public partial class PestaniaModelMetabusqueda
        {
            /// <summary>
            /// Nombre de la pestaña
            /// </summary>
            public string Nombre { get; set; }

            /// <summary>
            /// Campo del Filtro
            /// </summary>
            public string CampoFiltro { get; set; }

            /// <summary>
            /// Tipo de Busqueda
            /// </summary>
            public int TipoBusqueda { get; set; }

            /// <summary>
            /// Pestanya ID
            /// </summary>
            public Guid PestanyaID { get; set; }
        }

        /// <summary>
        /// Modelo de pestañas de una comunidad
        /// </summary>
        [Serializable]
        public partial class TabModel
        {
            /// <summary>
            /// Identificador de la pestañá
            /// </summary>
            public Guid Key { get; set; }
            /// <summary>
            /// Nombre de la pestaña
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Url de la pestaña
            /// </summary>
            public string Url { get; set; }
            /// <summary>
            /// Indica si la pestaña esta activa o no
            /// </summary>
            public bool Active { get; set; }
            /// <summary>
            /// Indica si la pestaña esta visible o no
            /// </summary>
            public bool Visible { get; set; }
            /// <summary>
            /// Indica si la pestaña se abre en la pestaña actual o en una nueva
            /// </summary>
            public bool OpenInNewWindow { get; set; }
            /// <summary>
            /// Pestañas hijas de la pestaña actual
            /// </summary>
            public List<TabModel> SubTab { get; set; }
        }

        /// <summary>
        /// Modelo con los permisos de un usuario en la comunidad
        /// </summary>
        [Serializable]
        public partial class PermissionsModel
        {
            /// <summary>
            /// Indica si el usuario tiene permiso para crear un widget en la comunidad
            /// </summary>
            public bool CreateWidget { get; set; }

            /// <summary>
            /// Indica si el usuario tiene permiso para crear un recurs en la comunidad
            /// </summary>
            public bool CreateResource { get; set; }

            /// <summary>
            /// Indica si el usuario tiene permiso para crear una pregunta en la comunidad
            /// </summary>
            public bool CreateQuestion { get; set; }

            /// <summary>
            /// Indica si el usuario tiene permiso para crear un debate en la comunidad
            /// </summary>
            public bool CreateDebate { get; set; }

            /// <summary>
            /// Indica si el usuario tiene permiso para crear una encuesta en la comunidad
            /// </summary>
            public bool CreatePoll { get; set; }

            /// <summary>
            /// Indica si el usuario tiene permiso para invitar a la comunidad
            /// </summary>
            public bool Invite { get; set; }

            /// <summary>
            /// Indica si se ha excedido el numero maximo de usuarios en la comunidad, en las comunidades en definición hay configurado un número maximo de usuarios
            /// </summary>
            public bool MaxMembersExceeded { get; set; }

            /// <summary>
            /// Indica si el usuario tiene permiso para editar una bio en la comunidad
            /// </summary>
            public bool EditBio { get; set; }

            /// <summary>
            /// Indica si el usuario tiene permiso para suscribirse a la comunidad
            /// </summary>
            public bool Subscribe { get; set; }

            /// <summary>
            /// Indica si el usuario tiene permiso para administrar la comunidad
            /// </summary>
            public bool Manage { get; set; }

            /// <summary>
            /// Indica si el usuario tiene permiso para administrar el CMS de la comunidad
            /// </summary>
            public bool ManageCMS { get; set; }

            /// <summary>
            /// Indica si el usuario tiene permiso para crear grupos en la comunidad
            /// </summary>
            public bool CreateGroup { get; set; }

            /// <summary>
            /// Indica si el usuario tiene permiso para administrar peticiones de acceso a la comunidad
            /// </summary>
            public bool ManageRequestAccess { get; set; }

            /// <summary>
            /// Indica si el usuario tiene permiso para administrar peticiones de grupos en la comunidad
            /// </summary>
            public bool ManageRequestGroup { get; set; }

            /// <summary>
            /// Indica si el usuario tiene permiso enviar newsletter en la comunidad
            /// </summary>
            public bool SendNewsletter { get; set; }

            /// <summary>
            /// Indica si el usuario tiene permiso para administrar las fuentes RSS de la comunidad
            /// </summary>
            public bool ManageRSSFeeds { get; set; }

            /// <summary>
            /// Indica si el usuario tiene permiso para abandonar la comunidad
            /// </summary>
            public bool LeaveCommunity { get; set; }

            /// <summary>
            /// Lista con los tipos de documento que tiene permisos para crear en la comunidad
            /// </summary>
            public List<ResourceModel.DocumentType> DocumentPermissions { get; set; }

            /// <summary>
            /// Lista de ontologias que tiene permiso el usuario para crear en la comunidad
            /// </summary>
            public SortedDictionary<string, KeyValuePair<string, string>> OntologyPermissionsNameUrls { get; set; }
        }

        /// <summary>
        /// Enlace de la cuenta de twitter de la comunidad
        /// </summary>
        public string TwitterLink { get; set; }

        /// <summary>
        /// Lista con las posibles cetificaciones de recursos que tiene la comunidad
        /// </summary>
        public Dictionary<Guid, string> ListaCertificaciones { get; set; }

        /// <summary>
        /// Numero de solicitudes de acceso pendientes en la comunidad
        /// </summary>
        public int SolicitudesPendientes { get; set; }

        /// <summary>
        /// Numero de solicitudes de grupos pendientes en la comunidad
        /// </summary>
        public int SolicitudesGrupoPendientes { get; set; }

        /// <summary>
        /// Numero de recursos RSS pendientes en la comunidad
        /// </summary>
        public int RecursosRSSPendientes { get; set; }

        /// <summary>
        /// Lista de vistas personalizadas para la comunidad
        /// </summary>
        public List<string> ListaPersonalizaciones { get; set; }

        /// <summary>
        /// Lista de vistas personalizadas para el ecosistema
        /// </summary>
        public List<string> ListaPersonalizacionesEcosistema { get; set; }

        /// <summary>
        /// Lista de vistas personalizadas para el dominio
        /// </summary>
        public List<string> ListaPersonalizacionesDominio { get; set; }

        /// <summary>
        /// Lista de vistas personalizadas para la comunidad
        /// </summary>
        public Guid PersonalizacionProyectoID { get; set; }

        /// <summary>
        /// HTML personalizado para el Login.
        /// </summary>
        public string ProjectLoginConfiguration { get; set; }

        /// <summary>
        /// Nos indica si esta permitido guardar en las páginas cuyas configuraciones
        /// se controlan con Integración Continua
        /// </summary>
        public bool DeshabilitarGuardarAdministracionIC 
        { 
            get 
            {
                return mDeshabilitarGuardarAdministracionIC;
			} 
            
            set 
            {
                mDeshabilitarGuardarAdministracionIC = value;
            } 
        }

        /// <summary>
        /// Contiene el mensaje de aviso con el estado del entorno en relación a la Integración Continua
        /// </summary>
        public string MensajeAvisoAdministracionIC { get; set; }

        /// <summary>
        /// Indica si en el aviso del estado del entorno en ralción a la Integración Continua
        /// hay que pintar el botón con el enlace a AdministrarDespliegues
        /// </summary>
        public bool MostrarBotonAdministrarDespliegues { get; set; }

        /// <summary>
        /// Indica si en el aviso del estado del entorno en ralción a la Integración Continua
        /// hay que pintar el botón con el enlace a AdministrarRamas
        /// </summary>
        public bool MostrarBotonAdministrarRamas { get; set; }

        /// <summary>
        /// Indica si la comunidad es catálogo.
        /// </summary>
        public bool IsCatalog
        {
            get
            {
                return (ProyectType == TypeProyect.Catalogo || ProyectType == TypeProyect.CatalogoNoSocialConUnTipoDeRecurso || ProyectType == TypeProyect.CatalogoNoSocial);
            }
        }

        /// <summary>
        /// Contiene el html extra de la tabla ProyectoElementoHtml
        /// </summary>
        public List<HtlmExtraElements> ProjectExtraHTMLList { get; set; }
    }

    /// <summary>
    /// Modelo de categorias
    /// </summary>
    [Serializable]
    public partial class CategoryModel
    {
        private string mNombre = null;
        private string mLanguageName = null;
        private string mIdioma = null;

        /// <summary>
        /// Idioma del usuario
        /// </summary>
        public string Lang { set { mIdioma = value; } }
        /// <summary>
        /// Identificador de la categoria
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Nombre de la categoria con todos los idiomas
        /// </summary>
        public string LanguageName
        {
            get
            {
                if (!string.IsNullOrEmpty(mLanguageName))
                {
                    return mLanguageName;
                }
                else
                {
                    return mNombre;
                }
            }
            set
            {
                mLanguageName = value;
            }
        }

        /// <summary>
        /// Nombre de la categoria
        /// </summary>
        public string Name
        {
            get
            {
                string textoIdioma = UtilCadenas.ObtenerTextoDeIdioma(mNombre, mIdioma, null, true);

                if (string.IsNullOrEmpty(textoIdioma))
                {
                    string lang = mIdioma;
                    if (lang.Contains('-'))
                    {
                        lang = lang.Substring(0, lang.IndexOf('-'));
                        textoIdioma = UtilCadenas.ObtenerTextoDeIdioma(mNombre, lang, null, true);
                    }

                    if (string.IsNullOrEmpty(textoIdioma))
                    {
                        textoIdioma = UtilCadenas.ObtenerTextoDeIdioma(mNombre, lang, null, false);
                    }
                }

                return textoIdioma;
            }
            set
            {
                mNombre = value;
            }
        }

        /// <summary>
        /// Orden de la categoria
        /// </summary>
        public short Order { get; set; }

        /// <summary>
        /// Indica si la categoria es obligatoria
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Identificador de la categoría padre
        /// </summary>
        public Guid ParentCategoryKey { get; set; }
        /// <summary>
        /// Identificador string de la categoría padre
        /// </summary>
        public string ParentCategoryStringKey { get; set; }
        /// <summary>
        /// Número de recursos de la categoria
        /// </summary>
        public int NumResources { get; set; }
        /// <summary>
        /// Indica si la categoria esta selecccionada
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// Identificador de la categoria en texto.
        /// </summary>
        public string StringKey { get; set; }

        /// <summary>
        /// Lista de categorías hijas
        /// </summary>
        public List<CategoryModel> Subcategories { get; set; }
    }

    [Serializable]
    public class PintarCategoriasEditarSuscripcion
    {
        public CategoryModel categoryModel { get; set; }
        public EditSuscriptionViewModel editSuscriptionView { get; set; }
    }

    [Serializable]
    public class PintarCategoriasFichaRecurso
    {
        public CategoryModel categoryModel { get; set; }
        public ResourceViewModel resourceView { get; set; }
    }

    [Serializable]
    public class PintarSharedCommunity
    {
        public List<ResourceModel.SharedBRModel> listaBaseRecursos { get; set; }
        public int position { get; set; }
        public bool pintarNumerLimitado { get; set; }
        public ResourceModel resourceModel { get; set; }
    }

    [Serializable]
    public class PintarCategoriasIndice
    {
        public CategoryModel categoryModel { get; set; }
        public IndexViewModel indexView{ get; set; }
    }

    [Serializable]
    public class PintarComunidad
    {
        public MyCommunitiesViewModel.MyCommunityModel communityModel { get; set; }
        public MyCommunitiesViewModel myCommunitiesView { get; set; }
    }

    [Serializable]
    public class PintarCategoriaRegistroDatos
    {
        public Dictionary<Guid, KeyValuePair<string, Dictionary<Guid, string>>> listaPreferencias { get; set; }
        public Guid catID { get; set; }
        public int numCat { get; set; }
    }


    [Serializable]
    public class SharedSemCms
    {
        public SemanticPropertyModel semanticPropertyModel { get; set; }
        public SemanticPropertyModel.PropertyValue propertyValue { get; set; }
        public string pIdioma { get; set; }
        public int pNumValor { get; set; }
        public string pValor { get; set; }
        public bool pTesauroSemSimple { get; set; }
        public SemanticPropertyModel.ResourceLinkedToEntitySelector pRecLink { get; set; }
        public short pTipoCampo { get; set; }
        public List<SemanticPropertyModel.PropertyValue> pValores { get; set; }
    }

    [Serializable]
    public class PintarCategoriaEditorTesauro
    {
        public ThesaurusEditorModel thesaurusEditor { get; set; }
        public CategoryModel categoryModel { get; set; }
    }

    [Serializable]
    public class PintarPestanyaCabecera
    {
        public CommunityModel.TabModel pPestanya { get; set; }
        public bool pPintarEnlaces { get; set; }
    }

    [Serializable]
    public class PintarCategoriaCabecera
    {
        public EditSuscriptionViewModel editSuscriptionView { get; set; }
        public CategoryModel pCategoria { get; set; }
    }

    [Serializable]
    public class PintarComboAdministrarCategorias
    {
        public CategoryModel pCategoria { get; set; }
        public string nombreCombo { get; set; }
        public bool pExcluirSeleccionadas { get; set; }
        public ThesaurusEditorModel Thesaurus { get; set; }
        public Guid pCategoriaID { get; set; }
        public KeyValuePair<Guid, Dictionary<string, string>> Categoria { get; set; }
        public bool MultiLanguaje { get; set; }
        public List<CategoryModel> CategoriasCompartir { get; set; }
        // Nuevas propiedades para poder pasar información a través de modales para pintado de ComboBox en Gestión de Categorías
        public bool ExistenRecursosNoHuerfanos { get; set; }
        public string IdiomaDefecto { get; set; }        
        public string IdiomaTesauro { get; set; }

    }

    [Serializable]
    public class PintarPestanyaAdministrar
    {
        public TabModel pestanya { get; set; }
        public List<TabModel> ListaPestanyas { get; set; }

    }

    [Serializable]
    public class PintarModalCrearSubCategoria
    {
        public string NombreCategoriaPadre { get; set; }
        public bool MultiLanguaje { get; set; }
        public string IdiomaTesauro { get; set; }
        public Guid CategoriaId { get; set; }
    }

    [Serializable]
    public class DibujarTablaFusionCambios
    {
        public IEnumerable<KeyValuePair<DateTime, List<ChangesContinuosIntegration>>> pModelo;
        public bool pPintamosEnlace;
        public string pMensaje;
        public string pNombreEntonro;
    }

    [Serializable]
    public class PintarNombresIdiomasCharts
    {
        public KeyValuePair<string, string> idioma;
        public string nombreChartIdiomaUsuario;
        public Dictionary<string, string> diccionarioNombres;
        public string chartID;
        public int Contador;
    }

    [Serializable]
    public class PintarCategoriaAdministrarComunidadGeneral
    {
        public List<CategoryModel> EcosistemaCategories { get; set; }
        public CategoryModel pCategoriaTesauro { get; set; }
        public List<Guid> SelectedCategories { get; set; }
    }

    [Serializable]
    public partial class HtlmExtraElements
    {
        public string Content { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public string TagName { get; set; }
        public UbicacionHtmlProyecto Ubication { get; set; }
        public List<int> ElementIDUserList { get; set; }
        public int HeadElementID { get; set; }
        public bool CookiesControl { get; set; }
        public bool HasPrivateElementsInCommunity { get; set; }
    }

    /// <summary>
    /// Enumeración de ubicaciónes de trozos de html dentro de la página
    /// </summary>
    public enum UbicacionHtmlProyecto
    {
        /// <summary>
        /// Al final de la etiqueta head
        /// </summary>
        EndHead = 0,

        /// <summary>
        /// Al final de la etiqueta body
        /// </summary>
        EndBody = 1,

        /// <summary>
        /// Al inicio de la etiqueta head
        /// </summary>
        BeginHead = 2,

        /// <summary>
        /// Al inicio de la etiqueta body
        /// </summary>
        BeginBody = 3
    }
}