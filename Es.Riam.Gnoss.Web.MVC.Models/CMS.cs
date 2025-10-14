using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using System;
using System.Collections.Generic;
using System.Data;

namespace Es.Riam.Gnoss.AD.CMS
{
    #region Enumeraciones

    /// <summary>
    /// Enumeración para distinguir tipos de Ubicaciones para el CMS
    /// </summary>
    public enum TipoUbicacionCMS
    {
        /// <summary>
        /// Home de la comunidad para un miembro de la comunidad
        /// </summary>
        HomeProyectoMiembro = 0,
        /// <summary>
        /// Home de la comunidad para alguien que no es miembro de la comunidad
        /// </summary>
        HomeProyectoNoMiembro = 1,
        /// <summary>
        /// Home de la comunidad para todo el mundo
        /// </summary>
        HomeProyecto = 2
    }

    /// <summary>
    /// Enumeración para distinguir tipos de caducidades de los Componetes disponibles para el CMS
    /// </summary>

    /// <summary>
    /// Enumeración para distinguir tipos de presenaciones que se sirven con el CMS
    /// </summary>
    public enum TipoPresentacionRecursoCMS
    {
        /// <summary>
        /// Solo título y foto
        /// </summary>
        SoloIconoYTitulo = 0,
        /// <summary>
        /// Titulo, fecha, descripcion (resumida 150 caracteres) y foto.
        /// </summary>
        BreveConFoto = 1,
        /// <summary>
        /// Titulo,  descripcion , etiquetas , publicador y fechas.
        /// </summary>
        ListadoSinFoto = 2,
        /// <summary>
        /// Completa (como aparece en las páginas de búsquedas)
        /// </summary>
        Completa = 3,
        /// <summary>
        /// Titulo, fecha, descripcion (resumida 1 parrafo)  y foto.
        /// </summary>
        BreveConFotoAUnParrafo = 4,
        /// <summary>
        /// Titulo y descripción completa
        /// </summary>
        DescripcionCompleta = 5,
        /// <summary>
        /// Últimos recursos
        /// </summary>
        UltimosRecursos = 6,
        /// <summary>
        /// Destacado
        /// </summary>
        Destacado = 7,
    }

    /// <summary>
    /// Enumeración para distinguir tipos de presentaciones para grupos de componentes
    /// </summary>
    public enum TipoPresentacionGrupoComponentesCMS
    {
        /// <summary>
        /// Presentación en pestañas horizontales
        /// </summary>
        PestanyasHorizontales = 0,
        /// <summary>
        /// Presentación con paginacion
        /// </summary>
        Paginacion = 1,
        /// <summary>
        /// Presentación en pestañas personalizadas
        /// </summary>
        PestanyasPersonalizadas = 2,
        /// <summary>
        /// Presentación en pestañas verticales
        /// </summary>
        PestanyasVerticales = 3,
    }

    /// <summary>
    /// Enumeración para distinguir tipos de presentaciones para listados de recursos
    /// </summary>
    public enum TipoPresentacionListadoRecursosCMS
    {
        /// <summary>
        /// Presentación en pestañas
        /// </summary>
        ListadoRecursosPestanyas = 0,
        /// <summary>
        /// Presentación con paginacion
        /// </summary>
        ListadoRecursosPaginacion = 1,
        /// <summary>
        /// Presentación normal
        /// </summary>
        ListadoRecursosNormal = 2,
        /// <summary>
        /// Presentación destacados
        /// </summary>
        ListadoDestacados = 3,
    }

    /// <summary>
    /// Enumeración para distinguir tipos de presentaciones para listados de usuarios
    /// </summary>
    public enum TipoPresentacionListadoUsuariosCMS
    {
        /// <summary>
        /// Solo foto
        /// </summary>
        MosaicoConFoto = 0,
        /// <summary>
        /// Foto y nombre
        /// </summary>
        ListadoConFotoYNombre = 1
    }

    /// <summary>
    /// Enumeración para distinguir tipos de presentaciones para facetas
    /// </summary>
    public enum TipoPresentacionFacetas
    {
        /// <summary>
        /// Normal
        /// </summary>
        Normal = 0,
        /// <summary>
        /// Barras
        /// </summary>
        Barras = 1,
        /// <summary>
        /// Sectores
        /// </summary>
        Sectores = 2
    }

    /// <summary>
    /// Enumeración para distinguir tipos de listados de usuarios
    /// </summary>
    public enum TipoListadoUsuariosCMS
    {
        /// <summary>
        /// Mas activos (mas recursos)
        /// </summary>
        MasActivos = 0,
        /// <summary>
        /// Ultimos registrados
        /// </summary>
        UltimosRegistrados = 1,
        /// <summary>
        /// Mas populares
        /// </summary>
        MasPopulares = 2
    }

    /// <summary>
    /// Enumeración para distinguir tipos de listados de proyectos
    /// </summary>
    public enum TipoListadoProyectosCMS
    {
        /// <summary>
        /// Los mas populares en los que no está el usuario
        /// </summary>
        RecomendadosUsuario = 0,
        /// <summary>
        /// Los mas afines al proyecto
        /// </summary>
        RecomendadosProyecto = 1,
        /// <summary>
        /// Listado de IDS con los proyectos
        /// </summary>
        Estaticos = 2,
        /// <summary>
        /// Comunidades a las que pertenece el usuario conectado
        /// </summary>
        ComunidadesUsuario = 3

    }

    /// <summary>
    /// Enumeración para distinguir tipos de campos de envio de correo
    /// </summary>
    public enum TipoCampoEnvioCorreo
    {
        Corta = 0,
        Larga = 1
    }

    /// <summary>
    /// Enumeración para distinguir tipos de propiedad de los de campos de envio de correo
    /// </summary>
    public enum TipoPropiedadEnvioCorreo
    {
        Nombre = 0,
        Obligatorio = 1,
        TipoCampo = 2
    }

    #endregion
}

namespace Es.Riam.Gnoss.Web.MVC.Models
{
    /// <summary>
    /// Representa un bloque en la estructura del CMS (Una página del CMS contendrá bloques que a su vez contendran componentes)
    /// </summary>
    [Serializable]
    public partial class CMSBlock
    {
        /// <summary>
        /// Identificador del bloque padre(si tiene)
        /// </summary>
        public Guid? ParentKey { get; set; }
        /// <summary>
        /// Identificador del bloque
        /// </summary>
        public Guid Key { get; set; }
        /// <summary>
        /// Lista de bloques contenidos dentro del bloque actual
        /// </summary>
        public List<CMSBlock> BlockList { get; set; }
        /// <summary>
        /// Lista de componentes contenidos dentro del bloque actual
        /// </summary>
        public List<CMSComponent> ComponentList { get; set; }
        /// <summary>
        /// Diccionario con atributos para incluir den el html bloque 
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; }
    }

    /// <summary>
    /// Clase abstracta de la que heredan todos los componentes del CMS
    /// </summary>
    [Serializable]
    public abstract class CMSComponent
    {
        /// <summary>
        /// Identificador del componentes
        /// </summary>
        public Guid Key { get; set; }
        /// <summary>
        /// Título del componente
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Estilos para incluir en el atributo class del componente 
        /// </summary>
        public string Styles { get; set; }
        /// <summary>
        /// Nombre de la vista que utiliza el componente
        /// </summary>
        public string ViewName { get; set; }
        /// <summary>
        /// Nombre de la vista que utilizan los recursos que aparezcan en el componente
        /// </summary>
        public string ViewNameResources { get; set; }
        /// <summary>
        /// Booleano que indica se el recurso se carga por AJAX
        /// </summary>
        public bool AJAX { get; set; }
    }

    /// <summary>
    /// Representa un componente del CMS de tipo HTMLLibre
    /// </summary>
    [Serializable]
    public class CMSComponentHTML : CMSComponent
    {
        /// <summary>
        /// Html libre para incluir en el componente
        /// </summary>
        public string HTML { get; set; }
    }

    /// <summary>
    /// Representa un componente del CMS de tipo Destacado
    /// </summary>
    [Serializable]
    public class CMSComponentHot : CMSComponent
    {
        /// <summary>
        /// Subtitulo del componente destacado
        /// </summary>
        public string Subtitle { get; set; }
        /// <summary>
        /// Url de la imagen del componente destacado
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// Html libre para incluir en el componente
        /// </summary>
        public string HTML { get; set; }
        /// <summary>
        /// Enlace al que apunta el componente
        /// </summary>
        public string Link { get; set; }
    }

    /// <summary>
    /// Representa un componente del CMS que es un listado de recursos
    /// </summary>
    [Serializable]
    public class CMSComponentResourceList : CMSComponent
    {
        /// <summary>
        /// Lista de recursos del componente
        /// </summary>
        public List<ResourceModel> ResourceList { get; set; }
        /// <summary>
        /// URL ver más
        /// </summary>
        public string URLSeeMore { get; set; }

    }

    /// <summary>
    /// Representa un componente del CMS que es un Grupo de componentes
    /// </summary>
    [Serializable]
    public class CMSComponentGroupComponents : CMSComponent
    {
        /// <summary>
        /// Lista de componentes que pertenecen al grupo
        /// </summary>
        public List<CMSComponent> ComponentList { get; set; }
    }

    /// <summary>
    /// Representa un componente del CMS de tipo Envío de correo
    /// </summary>
    [Serializable]
    public class CMSComponentMail : CMSComponent
    {
        /// <summary>
        /// Lista de campos del formulario
        /// </summary>
        public List<CMSFormFiled> FormFields { get; set; }

        /// <summary>
        /// Texto para el botón de envío de formulario
        /// </summary>
        public string TextButton { get; set; }

        /// <summary>
        /// Texto de notificación de envío correcto
        /// </summary>
        public string TextOK { get; set; }

        /// <summary>
        /// Url de envío de formulario
        /// </summary>
        public string UrlSendForm { get; set; }

        /// <summary>
        /// Representa un campo del formulario de envío de correo
        /// </summary>
        [Serializable]
        public class CMSFormFiled
        {
            #region Enumeraciones
            /// <summary>
            /// Enumeración para distinguir tipos de campos de envio de correo
            /// </summary>
            public enum CMSFormFiledType
            {
                Short = 0,
                Long = 1
            }
            #endregion

            /// <summary>
            /// Nombre del campo
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Orden del campo
            /// </summary>
            public int Order { get; set; }

            /// <summary>
            /// Indica si el campo es obligatorio
            /// </summary>
            public bool Required { get; set; }

            /// <summary>
            /// Indica el tipo de campo
            /// </summary>
            public CMSFormFiledType FormFiledType { get; set; }
        }
    }

    /// <summary>
    /// Representa un componente del CMS del tipo Menu
    /// </summary>
    [Serializable]
    public class CMSComponentMenu : CMSComponent
    {
        /// <summary>
        /// Representa un Item del componente menú
        /// </summary>
        [Serializable]
        public partial class CMSComponentMenuItem
        {
            /// <summary>
            /// Nombre del item del menu
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Enlace del item del menu
            /// </summary>
            public string Link { get; set; }
            /// <summary>
            /// Especifica si el item está activo
            /// </summary>
            public bool Active { get; set; }
            /// <summary>
            /// Lista de items
            /// </summary>
            public List<CMSComponentMenuItem> ItemList { get; set; }
        }
        /// <summary>
        /// Lista de items del componente
        /// </summary>
        public List<CMSComponentMenuItem> ItemList { get; set; }
    }

    /// <summary>
    /// Representa un componente del CMS del tipo buscador
    /// </summary>
    [Serializable]
    public class CMSComponentSearch : CMSComponent
    {
        /// <summary>
        /// Resultado de la busqueda del componente buscador
        /// </summary>
        public ResultadoModel Resultado { get; set; }

        /// <summary>
        /// Titulo del atributo de Búsqueda
        /// </summary>
        public string AttributeSearchTittle { get; set; }

        /// <summary>
        /// Filtro de la búsqueda
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// URL del buscador
        /// </summary>
        public string UrlSearcherCMS { get; set; }
    }

    /// <summary>
    /// Representa un componente del CMS del tipo Actividad reciente
    /// </summary>
    [Serializable]
    public class CMSComponentRecentActivity : CMSComponent
    {
        /// <summary>
        /// Modelo de actividad reciente
        /// </summary>
        public RecentActivity RecentActivity { get; set; }
    }

    /// <summary>
    /// Representa un componente del CMS del tipo Ficha descripción documento
    /// </summary>
    [Serializable]
    public class CMSComponentResourceDescription : CMSComponent
    {
        /// <summary>
        /// 
        /// </summary>
        public SemanticResourceModel SemanticResourceModel { get; set; }
    }

    /// <summary>
    /// Representa un componente del CMS del tipo Faceta
    /// </summary>
    [Serializable]
    public class CMSComponentFacet : CMSComponent
    {
        #region Enumeraciones
        /// <summary>
        /// Enumeración para distinguir tipos de presentacion de las facetas
        /// </summary>
        public enum CMSComponentFacetPresentation
        {
            /// <summary>
            /// Normal
            /// </summary>
            Normal = 0,
            /// <summary>
            /// Barras
            /// </summary>
            Bars = 1,
            /// <summary>
            /// Sectores
            /// </summary>
            Sectors = 2
        }
        #endregion

        /// <summary>
        /// Modelo de faceta
        /// </summary>
        public FacetModel FacetModel { get; set; }

        public CMSComponentFacetPresentation PresentatioType { get; set; }

        /// <summary>
        /// URL del buscador
        /// </summary>
        public string UrlSearcherCMS { get; set; }
    }

    /// <summary>
    /// Representa un componente del CMS que es una ConsultaSPARQL
    /// </summary>
    [Serializable]
    public class CMSComponentQuerySPARQL : CMSComponent
    {
        /// <summary>
        /// DATASET Result
        /// </summary>
        public DataSet DataSetResult { get; set; }

    }

    /// <summary>
    /// Representa un componente del CMS que es una ConsultaSQLSERVER
    /// </summary>
    [Serializable]
    public class CMSComponentQuerySQLSERVER : CMSComponent
    {
        /// <summary>
        /// DATASET Result
        /// </summary>
        public DataSet DataSetResult { get; set; }

    }

    /// <summary>
    /// Representa un componente del CMS del tipo Tesauro
    /// </summary>
    [Serializable]
    public class CMSComponentThesaurus : CMSComponent
    {
        /// <summary>
        /// Obtiene o establece si tiene imagen
        /// </summary>
        public bool Image { get; set; }

        /// <summary>
        /// Nombre de la categoría
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// Lista de categorías a mostrar
        /// </summary>
        public List<CategoryModel> Categories { get; set; }

        /// <summary>
        /// URL del índice
        /// </summary>
        public string UrlIndex { get; set; }

        /// <summary>
        /// URL base de las categorías
        /// </summary>
        public string UrlBaseCategories { get; set; }
    }

    /// <summary>
    /// Representa un componente del CMS del tipo Datos comunidadTesauro
    /// </summary>
    [Serializable]
    public class CMSComponentCommunityInfo : CMSComponent
    {
        public int ResourcesCount { get; set; }
        public int IdentitiesCount { get; set; }
        public string ResourcesUrl { get; set; }
        public string IdentitiesUrl { get; set; }
    }

    /// <summary>
    /// Representa un componente del CMS del tipo Usuarios Recomendados
    /// </summary>
    [Serializable]
    public class CMSComponentRecommendedUsers : CMSComponent
    {
        public List<ProfileModel> RecomendedUsers { get; set; }
    }

    /// <summary>
    /// Representa un componente del CMS del tipo Listado de Usuarios
    /// </summary>
    [Serializable]
    public class CMSComponentUserList : CMSComponent
    {
        public List<ProfileModel> Users { get; set; }
    }

    /// <summary>
    /// Representa un componente del CMS del tipo Listado de comunidades
    /// </summary>
    [Serializable]
    public class CMSComponentCommunityList : CMSComponent
    {
        public List<CommunityModel> Communities { get; set; }
    }

    /// <summary>
    /// Representa un componente del CMS del tipo Caja Buscador
    /// </summary>
    [Serializable]
    public class CMSComponentSearchBox : CMSComponent
    {
        public string DefaultText { get; set; }
        public Guid AutocompleteID { get; set; }
        public string UrlBusqueda { get; set; }
    }


    /// <summary>
    /// Representa un componente del CMS del tipo Resumen Perfil
    /// </summary>
    [Serializable]
    public class CMSComponentProfileSummary : CMSComponent
    {
        public string ProfileUrl { get; set; }
        public string ProfileName { get; set; }
        public string ProfileUrlImage { get; set; }
        public string ProfileFollowersUrl { get; set; }
        public int ProfileFollowersNumber { get; set; }
        public string ProfileFollowingUrl { get; set; }
        public int ProfileFollowingNumber { get; set; }
        public string ProfilePersonalResourcesUrl { get; set; }
        public int ProfilePersonalResourcesNumber { get; set; }
        public string ProfileCommunityResourcesUrl { get; set; }
        public int ProfileCommunityResourcesNumber { get; set; }

    }

    /// <summary>
    /// Representa un componente del CMS del tipo mas vistos
    /// </summary>
    [Serializable]
    public class CMSComponentMoreView : CMSComponent
    {
        /// <summary>
        /// Lista de recursos del componente
        /// </summary>
        public List<ResourceModel> MoreVisitedWeek { get; set; }
        /// <summary>
        /// Lista de recursos del componente
        /// </summary>
        public List<ResourceModel> MoreVisitedMonth { get; set; }
        /// <summary>
        /// Lista de recursos del componente
        /// </summary>
        public List<ResourceModel> MoreVisited { get; set; }

    }


    /// <summary>
    /// Representa un objeto multimedia
    /// </summary>
    [Serializable]
    public class CMSMultimediaModel : ObjetoBuscadorModel
    {
        /// <summary>
        /// Titulo
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Width (Only has value if is an image)
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height (Only has value if is an image)
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Size of the file
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Creation date
        /// </summary>
        public DateTime CreationDate { get; set; }
    }

    /// <summary>
    /// Representa un objeto componente
    /// </summary>
    [Serializable]
    public class CMSEditComponentModel : ObjetoBuscadorModel
    {
        /// <summary>
        /// Identificador del componente
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Titulo
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Activo
        /// </summary>
        public bool Activo { get; set; }

        /// <summary>
        /// Tipo de componente
        /// </summary>
        public TipoComponenteCMS CMSComponentType { get; set; }

        /// <summary>
        /// Fecha de modificación del componente
        /// </summary>
        public DateTime EditionDate { get; set; }

        /// <summary>
        /// Indica si se ha versionado por lo menos una vez el componente
        /// </summary>
        public bool Versionado { get; set; }
    }
}