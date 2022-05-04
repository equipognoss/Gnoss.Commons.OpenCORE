using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    /// <summary>
    /// ViewModel de la pagina de administrar opciones del meta-administrador
    /// </summary>
    [Serializable]
    public class AdministrarOpcionesMetaadministradorViewModel
    {
        /// <summary>
        /// Indica si el CMS está activado
        /// </summary>
        public bool CMSActivado { get; set; }
        /// <summary>
        /// Indica si en la comunidad esta permita la administración de las páginas
        /// </summary>
        public bool AdministracionPaginasPermitido { get; set; }
        /// <summary>
        /// Indica si en la comunidad esta permita la administración de la semantica
        /// </summary>
        public bool AdministracionSemanticaPermitido { get; set; }
        /// <summary>
        /// Indica si en la comunidad esta permita la administración de las vistas
        /// </summary>
        public bool AdministracionVistasPermitido { get; set; }
        /// <summary>
        /// Indica si en la comunidad esta permita la administración de las páginas de desarrolladores
        /// </summary>
        public bool AdministracionDesarrolladoresPermitido { get; set; }
        /// <summary>
        /// Indica si las vistas estan activadas en la comunidad
        /// </summary>
        public bool VistasActivadas { get; set; }
        /// <summary>
        /// Identificador de la personalización de las vistas
        /// </summary>
        public Guid PersonalizacionIDVistas { get; set; }
        /// <summary>
        /// Lista de proyectos con personalización de las vistas
        /// </summary>
        public Dictionary<Guid, KeyValuePair<string, Guid>> ListaProyectosVistas { get; set; }
        /// <summary>
        /// Indica si el ecosistema tiene una personalización de vistas
        /// </summary>
        public bool TieneVistasEcosistema { get; set; }
        /// <summary>
        /// Indica si la comunidad va a usar las vistas personalizadas del Ecosistema
        /// </summary>
        public bool NoUsarVistasDelEcosistema { get; set; }
        /// <summary>
        /// Indica si el proyecto tiene las etiquetas enlazadas con Linked Open Data
        /// </summary>
        public bool EtiquetasConLOD { get; set; }
        /// <summary>
        /// Indica si los nuevos usuarios se van a agrupar en la home del proyecto
        /// </summary>
        public bool AgruparEventosNuevosUsuarios { get; set; }
        /// <summary>
        /// Indica si el sitemap esta activado
        /// </summary>
        public bool SiteMapActivado { get; set; }
        /// <summary>
        /// Indica si es una comunidad enlazada
        /// </summary>
        public bool CabeceraSimple { get; set; }

        /// <summary>
        /// Indica si los usuarios pueden tener una descripción corta en la comunidad
        /// </summary>
        public bool BiosCortas { get; set; }

        /// <summary>
        /// Indica si en la comunidad se van a poder descargar los RDFs de las búsquedas
        /// </summary>
        public bool RdfDisponibles { get; set; }

        /// <summary>
        /// Indica si se van a permitir cargas masivas de recursos
        /// </summary>
        public bool CargasMasivasDisponibles { get; set; }

        /// <summary>
        /// Indica si el usuario debe introducir su fecha de nacimiento obligatoriamente
        /// </summary>
        public bool FechaNacimientoObligatoria { get; set; }

        /// <summary>
        /// Indica si el usuario debe seleccionar el nivel de privacidad que quiere para su perfil
        /// </summary>
        public bool PrivacidadObligatoria { get; set; }

        /// <summary>
        /// Indica si está disponible la opción de configurar eventos
        /// </summary>
        public bool EventosDisponibles { get; set; }

        /// <summary>
        /// En una comunidad privada, indica si se va a ir a buscar la cookie del usuario al servicio de login
        /// </summary>
        public bool SolicitarCookieLogin { get; set; }

        /// <summary>
        /// Indica si se puede invitar a la comunidad a tus contactos en la plataforma
        /// </summary>
        public bool InvitacionesPorContactoDisponibles { get; set; }

        /// <summary>
        /// Indica si se puede realizar consultas personalizadas
        /// </summary>
        public bool ConsultaSparql { get; set; }

        /// <summary>
        /// Indica si se pueden hacer preguntas TIC
        /// </summary>
        public bool PreguntaTIC { get; set; }

        /// <summary>
        /// Indica si se puede hacer busquedas SPARQL
        /// </summary>
        public bool BuscadorSPARQL { get; set; }

        /// <summary>
        /// Indica si se puede hacer una ficha de descipción del documento
        /// </summary>
        public bool FichaDescripcionDocumento { get; set; }

        /// <summary>
        /// Indica si se puede hacer consultas con SQLServer
        /// </summary>
        public bool ConsultaSQLServer { get; set; }

        /// <summary>
        /// Indica si se debe mostrar la notificación de que la comunidad usa cookies
        /// </summary>
        public bool AvisoCookie { get; set; }

        /// <summary>
        /// Indica la versión de los estilos de esta comunidad
        /// </summary>
        public int VersionCSS { get; set; }

        /// <summary>
        /// Indica la versión del javascript de esta comunidad
        /// </summary>
        public int VersionJS { get; set; }

        /// <summary>
        /// Indica el número de recursos relacionados que aparece en el contexto de recursos relacionados (por defecto 5)
        /// </summary>
        public int NumeroRecursosRelacionados { get; set; }

        /// <summary>
        /// Copyright de la comunidad (ej: RIAM I+L lab)
        /// </summary>
        public string Copyright { get; set; }

        /// <summary>
        /// Enlace que va a aparecer en el pie de página como contacto
        /// </summary>
        public string EnlaceContactoPiePagina { get; set; }

        /// <summary>
        /// Define las propiedades y sus pesos en el algoritmo de personas recomendadas
        /// </summary>
        public string AlgoritmoPersonasRecomendadas { get; set; }

        /// <summary>
        /// Define si los usuarios van a estar suscritos por defecto a la newsletter o no
        /// </summary>
        public bool RecibirNewsletterDefecto { get; set; }

        /// <summary>
        /// Indica si se va enviar email de suscripciones
        /// </summary>
        public bool EnviarNotificacionesDeSuscripciones { get; set; }

        /// <summary>
        /// Indica si se va a suscribir automáticamente a cada usuario que entre a la comunidad a todas sus categorías
        /// </summary>
        public bool SuscribirATodaComunidad { get; set; }

        /// <summary>
        /// Indica si los recursos de esta comunidad se guardan en GoogleDrive
        /// </summary>
        public bool GoogleDrive { get; set; }

        /// <summary>
        /// Indica si se quieren ocultar las facetas de otras ontologías y mostrar sólo las comunes de recursos en las páginas de búsqueda donde aparezcan más de un objeto de conocimiento
        /// </summary>
        public bool OcultarFacetasDeOntologiasEnRecursosCuandoEsMultiple { get; set; }

        /// <summary>
        /// Indica si la tercera petición de facetas vienen sin desplegar, hasta que el usuario pinche en la faceta
        /// </summary>
        public bool TerceraPeticionFacetasPlegadas { get; set; }

        /// <summary>
        /// Indica si el proyecto actual tiene grafo dbpedia en los recursos
        /// </summary>
        public bool TieneGrafoDbPedia { get; set; }

        /// <summary>
        /// Indica si esta comunidad va a funcionar en el dominio sin /comunidad/larioja
        /// </summary>
        public bool ProyectoSinNombreCortoEnURL { get; set; }

        /// <summary>
        /// Define, para una comunidad privada, si no es necesario una invitación para entrar a la comunidad. 
        /// </summary>
        public bool RegistroAbierto { get; set; }

        /// <summary>
        /// Permite la descarga de adjuntos a usuarios no logueados
        /// </summary>
        public bool PermitirDescargaIdentidadInvitada { get; set; }

        /// <summary>
        /// Indica si se deben cargar los editores y lectores de recursos en las páginas de búsqueda
        /// </summary>
        public bool CargarEditoresLectoresEnBusqueda { get; set; }

        /// <summary>
        /// Día de la semana en el que se envían las suscripciones. Valores del 1 al 7 (1 Lunes - 7 Domingo)
        /// </summary>
        public int DiaEnvioSuscripcion { get; set; }

        /// <summary>
        /// Segundos que se dejan de intervalo entre envío de una newsletter y otro
        /// </summary>
        public int SegundosDormirNewsletterPorCorreo { get; set; }

        /// <summary>
        /// Número de facetas que se van a obtener en la primera petición de facetas
        /// </summary>
        public int NumeroFacetasPrimeraPeticion { get; set; }

        /// <summary>
        /// Número de facetas que se van a obtener en la segunda petición de facetas
        /// </summary>
        public int NumeroFacetasSegundaPeticion { get; set; }

        /// <summary>
        /// Alto al que deben hacerse las capturas. Ej: 280
        /// </summary>
        public int CapturasImgSizeAlto { get; set; }
        /// <summary>
        /// Ancho al que deben hacerse las capturas. Ej: 280
        /// </summary>
        public int CapturasImgSizeAncho { get; set; }

        /// <summary>
        /// Indica el client id y el client secret de la aplicación de Facebook. Ej: id|||579839168702303@@@clientsecret|||b959a4485t0e88182834e4149b5d47c7
        /// </summary>
        public string LoginFacebook { get; set; }

        /// <summary>
        /// Indica el client id y el client secret de la aplicación de Google. Ej: id|||880006438141-r84hn261rtct92aqe3d5adjlod4umacn.apps.googleusercontent.com@@@clientsecret|||0d8fxeZDl-qV0XRiTnCCB54n
        /// </summary>
        public string LoginGoogle { get; set; }

        /// <summary>
        /// Indica el client id y el client secret de la aplicación de Twitter. Ej: consumerkey|||lUwbKMGlYF9H6lPNtm4Gkj4AY@@@consumersecret|||2vY81oR1GFCAL818jafYyRglC8NBZaDYUl47xf1Z1qq5sUA6jf
        /// </summary>
        public string LoginTwitter { get; set; }

        /// <summary>
        /// Lista de facetas lentas que se van a obtener de manera asíncrona para que la tercera petición de facetas llegue antes. Ej: ecidoc:p108i_E12_p126_employed_support@@@pm:supportNode|ecidoc:p62_E52_p79_has_time-span_beginning|ecidoc:has_school@@@pm:schoolNode
        /// </summary>
        public string FacetasCostosasTerceraPeticion { get; set; }

        /// <summary>
        /// Define la propiedad que especifica los idiomas en los que un recurso está descrito. Si está configurada, los usuarios que lleguen en un idioma que no está definido en un recurso, no podrán encontrarlo en las búsquedas ni acceder a su ficha (salvo que se cambien de idioma). Ej: dce:language
        /// </summary>
        public string PropiedadContenidoMultiIdioma { get; set; }

        /// <summary>
        /// Define la propiedad que especifica los idiomas en los que un componente del CMS está descrito. Si está configurada, los usuarios que lleguen en un idioma que no está definido en un componente del CMS, no podrán verlo (salvo que se cambien de idioma).
        /// </summary>
        public bool PropiedadCMSMultiIdioma { get; set; }

        /// <summary>
        /// Lista de propiedades en las que se deben buscar entidades de dbpedia. Normalmente, sólo se busca en los tags. Ej: ernews:has_metadata@@@bbvao:name|ernews:has_metadata@@@bbvao:dbpediaUri|bbvao:name|bbvao:dbpediaUri
        /// </summary>
        public string PropiedadesConEnlacesDbpedia { get; set; }

        /// <summary>
        ///  Condición para excluir recursos cuando se busque desde un móvil. Ej: MINUS { ?s skos:ConceptID ?skosConceptIDexcluida.  FILTER(?skosConceptIDexcluida=gnoss:DA6D3DFD-CA97-4E5B-9D40-7C3BAFD80C16) } 
        /// </summary>
        public string ExcepcionBusquedaMovil { get; set; }

        /// <summary>
        /// Indica si esta comunidad el texto de las propiedades del tesauro semántico permanecerá invariable (se respetarán las mayúsculas y minúsculas)
        /// </summary>
        public bool TextoInvariableTesauroSemantico { get; set; }

        /// <summary>
        /// Indica los proyectos en los cuales el registro es obligatorio.
        /// </summary>
        public Dictionary<string, bool> ProyectosRegistroObligatorio { get; set; }

        /// <summary>
        /// Indica la ruta de Estilos que tiene esa comunidad.
        /// </summary>
        public string RutaEstilos { get; set; }

        /// <summary>
        /// Indica los proyectos en los cuales el registro es obligatorio.
        /// </summary>
        public string ProyectosView { get; set; }

        /// <summary>
        /// Indica el tipo de Mensaje de Bienvenida
        /// </summary>
        public int TipoEnviarMensajeBienvenida { get; set; }

        /// <summary>
        /// Indica los dias de caducidad de la contraseña
        /// </summary>
        public int CaducidadPassword { get; set; }

        /// <summary>
        /// Indica la periodicidad de la Suscripcion
        /// </summary>
        public int PeriodicidadSuscripcion { get; set; }

        /// <summary>
        /// Indica el tipo de cacheado de facetas
        /// </summary>
        public bool CacheFacetas { get; set; }

        /// <summary>
        /// Indica si se debe ocultar la contraseña cuando se cambie
        /// </summary>
        public bool OcultarCambiarPassword { get; set; }

        /// <summary>
        /// Indica si se pueden duplicar los recursos disponibles.
        /// </summary>
        public bool DuplicarRecursosDisponible { get; set; }

        /// <summary>
        /// Indica si se agrupan los usuarios por proyecto
        /// </summary>
        //public bool AgruparRegistrosUsuariosEnProyecto { get; set; }

        /// <summary>
        /// Indica el Nombre de la politica de Cookies
        /// </summary>
        public string NombrePoliticaCookies { get; set; }

        /// <summary>
        /// Indica los segundos a esperar para la siguiente comprobación en ServiceBus
        /// </summary>
        public int ServiceBusSegundos { get; set; }
        /// <summary>
        /// Indica los reintentos para coger el fichero configurado con ServiceBus
        /// </summary>
        public int ServiceBusReintentos { get; set; }
        /// <summary>
        /// Indica si esta permitida la replicacion convencional de Gnoss
        /// </summary>
        public bool Replicacion { get; set; }
        /// <summary>
        /// Indica los items que se muestran en la actividad reciente
        /// </summary>
        public int FilasPorPagina { get; set; }
        /// <summary>
        /// Indica si se debe usar la misma variable para entidades en facetas
        /// Si su valor es 1, siempre que se haga referencia a una entidad en una consulta de facetas, la variable usada será la misma.
        /// </summary>
        public bool RegistroAbiertoEnComunidad { get; set; }
        /// <summary>
        /// Indica si se puden permitir las mayúsculas en el grafo de búsqueda
        /// </summary>
        public bool PermitirMayusculas { get; set; }

    }
}
