using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public enum TipoCaducidadComponenteCMS
    {
        /// <summary>
        /// No caduca nunca
        /// </summary>
        NoCaducidad = 0,
        /// <summary>
        /// Caduca a la hora
        /// </summary>
        Hora = 1,
        /// <summary>
        /// Caduca al día
        /// </summary>
        Dia = 2,
        /// <summary>
        /// Caduca a la semana
        /// </summary>
        Semana = 3,
        /// <summary>
        /// Caduca al publicar/editrar/eliminar un recurso de la comunidad
        /// </summary>
        Recurso = 4,
        /// <summary>
        /// Caduca al registrar un usuario nuevo en la comunidad
        /// </summary>
        Persona = 5,
        /// <summary>
        /// No utiliza cache
        /// </summary>
        NoCache = 6,
    }


    /// <summary>
    /// Enumeración para distinguir tipos de Componetes disponibles para el CMS
    /// </summary>
    public enum TipoComponenteCMS
    {
        /// <summary>
        /// Representa un HTML plano
        /// </summary>
        HTML = 0,
        /// <summary>
        /// Representa un HTML destacado
        /// </summary>
        Destacado = 1,
        /// <summary>
        /// Representa un listado dinamico
        /// </summary>
        ListadoDinamico = 2,
        /// <summary>
        /// Representa un listado estático
        /// </summary>
        ListadoEstatico = 3,
        /// <summary>
        /// Representa la actividad reciente
        /// </summary>
        ActividadReciente = 4,
        /// <summary>
        /// Representa un grupo de componentes
        /// </summary>
        GrupoComponentes = 5,
        /// <summary>
        /// Representa una sección del tesauro de la comunidad
        /// </summary>
        Tesauro = 6,
        ///// <summary>
        ///// Representa los recursos destacados de la comunidad
        ///// </summary>
        //RecursosDestacados = 7,
        /// <summary>
        /// Representa los datos de la comunidad, numero de recursos y personas y organizaciones
        /// </summary>
        DatosComunidad = 8,
        /// <summary>
        /// Representa los usuarios recomendados
        /// </summary>
        UsuariosRecomendados = 9,
        /// <summary>
        /// Representa una caja de buscador
        /// </summary>
        CajaBuscador = 10,
        ///// <summary>
        ///// Representa los recursos destacados estaticos de la comunidad
        ///// </summary>
        //RecursosDestacadosEstatico = 11,
        ///// <summary>
        ///// Representa un refcurso destacado
        ///// </summary>
        //RecursoDestacado = 12,
        /// <summary>
        /// Representa una faceta
        /// </summary>
        Faceta = 13,
        /// <summary>
        /// ListadoUsuarios
        /// </summary>
        ListadoUsuarios = 14,
        /// <summary>
        /// ListadoProyectos
        /// </summary>
        ListadoProyectos = 15,
        /// <summary>
        /// ResumenPerfil
        /// </summary>
        ResumenPerfil = 16,
        /// <summary>
        /// MasVistos
        /// </summary>
        MasVistos = 17,
        /// <summary>
        /// EnvioCorreo
        /// </summary>
        EnvioCorreo = 18,
        /// <summary>
        /// PreguntaTIC
        /// </summary>
        PreguntaTIC = 19,
        /// <summary>
        /// Menu
        /// </summary>
        Menu = 20,
        /// <summary>
        /// Buscador
        /// </summary>
        Buscador = 21,
        /// <summary>
        /// BuscadorSPARQL
        /// </summary>
        BuscadorSPARQL = 22,
        /// <summary>
        /// UltimosRecursosVisitados
        /// </summary>
        UltimosRecursosVisitados = 23,
        /// <summary>
        /// Ficha descripción documento
        /// </summary>
        FichaDescripcionDocumento = 24,
        /// <summary>
        /// MasVistos en x dias
        /// </summary>
        MasVistosEnXDias = 25,
        /// <summary>
        /// ConsultaSPARQL
        /// </summary>
        ConsultaSPARQL = 26,
        /// <summary>
        /// ConsultaSQLSERVER
        /// </summary>
        ConsultaSQLSERVER = 27,
        /// <summary>
        /// Listado al que se le pasan los recursos por parametros
        /// </summary>
        ListadoPorParametros = 28
    }

    /// <summary>
    /// Enumeración para distinguir tipos de propiedades de los Componetes disponibles para el CMS
    /// </summary>
    public enum TipoPropiedadCMS
    {
        /// <summary>
        /// Representa un HTML plano
        /// </summary>
        HTML = 0,
        /// <summary>
        /// Representa el título
        /// </summary>
        Titulo = 1,
        /// <summary>
        /// URL de una imagen
        /// </summary>
        Imagen = 2,
        /// <summary>
        /// URL de un enlace
        /// </summary>
        Enlace = 3,
        /// <summary>
        /// URL de una búsqueda
        /// </summary>
        URLBusqueda = 4,
        /// <summary>
        /// Número de Itmes
        /// </summary>
        NumItems = 5,
        /// <summary>
        /// Listado de GUIDs separados por comas
        /// </summary>
        ListaIDs = 6,
        /// <summary>
        /// Tipo de busqueda
        /// </summary>
        //TipoDeBusqueda = 7,
        /// <summary>
        /// Guid de un elemento
        /// </summary>
        ElementoID = 8,
        /// <summary>
        /// Indica si tiene imagen
        /// </summary>
        TieneImagen = 9,
        /// <summary>
        /// Indica el tipo de actividad reciente
        /// </summary>
        TipoActividadRecienteCMS = 10,
        /// <summary>
        /// Número de Itmes para mostrar
        /// </summary>
        NumItemsMostrar = 11,
        /// <summary>
        /// Tipo presentación recursos
        /// </summary>
        TipoPresentacionRecurso = 12,
        /// <summary>
        /// Texto por defecto
        /// </summary>
        TextoDefecto = 13,
        /// <summary>
        /// Indica si tiene boton hazte miembro
        /// </summary>
        TieneBotonHazteMiembro = 14,
        /// <summary>
        /// Tipo presentación grupo de componentes
        /// </summary>
        TipoPresentacionGrupoComponentes = 15,
        /// <summary>
        /// Representa el Subtitulo
        /// </summary>
        Subtitulo = 16,
        /// <summary>
        /// Tipo presentación para listado de recursos
        /// </summary>
        TipoPresentacionListadoRecursos = 17,
        /// <summary>
        /// Tipo presentación para listado de usuarios
        /// </summary>
        TipoPresentacionListadoUsuarios = 18,
        /// <summary>
        /// Tipo de listado de usuarios
        /// </summary>
        TipoListadoUsuarios = 19,
        /// <summary>
        /// Pestanya
        /// </summary>
        //Pestanya = 20,
        /// <summary>
        /// Faceta
        /// </summary>
        Faceta = 21,
        /// <summary>
        /// Tipo de presentacion de faceta
        /// </summary>
        TipoPresentacionFaceta = 22,
        /// <summary>
        /// Ver mas
        /// </summary>
        VerMas = 23,
        /// <summary>
        /// Tipo de listado de proyectos
        /// </summary>
        TipoListadoProyectos = 24,
        /// <summary>
        /// Lista de campos de envío de correo
        /// </summary>
        ListaCamposEnvioCorreo = 25,
        /// <summary>
        /// Texto para un botón
        /// </summary>
        TextoBoton = 26,
        /// <summary>
        /// Texto con el destinatario de envío del correo
        /// </summary>
        DestinatarioCorreo = 27,
        /// <summary>
        /// Texto mensaje todo correcto
        /// </summary>
        TextoMensajeOK = 28,
        /// <summary>
        /// Para mostrar todas las personas
        /// </summary>
        ContarPersonasNoVisibles = 29,
        /// <summary>
        /// Lista de campos 
        /// </summary>
        ListaOpcionesMenu = 30,
        /// <summary>
        /// Valor seleccionado
        /// </summary>
        ValorSeleccionado = 31,
        /// <summary>
        /// Atributo de busqueda
        /// </summary>
        AtributoDeBusqueda = 32,
        /// <summary>
        /// Título del Atributo de busqueda
        /// </summary>
        TituloAtributoDeBusqueda = 33,
        /// <summary>
        /// Personalizacion
        /// </summary>
        Personalizacion = 34,
        /// <summary>
        /// Personalizacion
        /// </summary>
        URLVerMas = 35,
        /// <summary>
        /// QuerySPARQL
        /// </summary>
        QuerySPARQL = 36,
        /// <summary>
        /// NumDias
        /// </summary>
        NumDias = 37,
        /// <summary>
        /// QuerySQLSERVER
        /// </summary>
        QuerySQLSERVER = 38,
    }


    /// <summary>
    /// ViewModel de la página de administrar un componente del CMS
    /// </summary>
    [Serializable]
    public class CMSAdminComponenteEditarCheckListViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int Orden { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid Identificador { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Error { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UrlEnlace { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TextoEnlace { get; set; }
    }

    /// <summary>
    /// ViewModel de la página de administrar un componente del CMS
    /// </summary>
    [Serializable]
    public class CMSAdminComponenteEditarViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public bool AccesoPublicoComponente { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool ContenidoMultiIdioma { get; set; }
        /// <summary>
        /// Lista de idiomas de la plataforma
        /// </summary>
        public List<string> ListaIdiomasDisponibles { get; set; }
        /// <summary>
        /// Lista de idiomas de la plataforma
        /// </summary>
        public Dictionary<string, string> ListaIdiomas { get; set; }
        /// <summary>
        /// Idioma por defecto de la comunidad
        /// </summary>
        public string IdiomaPorDefecto { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UrlVuelta { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool EsEdicion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ShortName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime FechaModificacion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Private { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<Guid, string> GruposPrivacidad { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<Guid, string> PerfilesPrivacidad { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TipoComponenteCMS Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<TipoCaducidadComponenteCMS, bool> Caducidades { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TipoCaducidadComponenteCMS CaducidadSeleccionada { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Styles { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<Guid, string> Personalizaciones { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid PersonalizacionSeleccionada { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<PropiedadComponente> Properties { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class PropiedadComponente
        {
            /// <summary>
            /// 
            /// </summary>
            public TipoPropiedadCMS TipoPropiedadCMS { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public bool Required { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Dictionary<string, string> Options { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Value { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public bool MultiLang { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public TipoComponenteCMS TypeComponent { get; set; }

        }
    }
}
