using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    /// <summary>
    /// View Model para el controlador AdministrarOpcionesAvanzadas
    /// </summary>
    [Serializable]
    public class AdministrarOpcionesAvanzadasViewModel
    {
        /// <summary>
        /// Lista de proyectos que administra el mismo usuario, con sus ontologías
        /// </summary>
        public Dictionary<Guid, string> ProyectosConOntologias { get; set; }

        /// <summary>
        /// Indica si el CMS está activado
        /// </summary>
        public bool CMSActivado { get; set; }

        /// <summary>
        /// Grupos con permiso para seleccionar privacidad de recurso abierto
        /// </summary>
        public Dictionary<Guid, string> GruposVisibilidadAbierto { get; set; }

        /// <summary>
        /// Guid del proyecto del que se van a usar las ontologías
        /// </summary>
        public Guid OntologiaOtroProyecto { get; set; }

        /// <summary>
        /// Verdad si siempre se va a intentar autocompletar desde virtuoso
        /// </summary>
        public bool AutocompletarSiempreVirtuoso { get; set; }

        /// <summary>
        /// Verdad si la búsqueda en todo el ecosistema está disponible
        /// </summary>
        public bool BuscarTodoEcosistema { get; set; }

        /// <summary>
        /// Verdad si la búsqueda en todo el proyecto está disponible
        /// </summary>
        public bool BuscarTodoProyecto { get; set; }

        /// <summary>
        /// Diccionario con el Id y el nombre de las pestañas
        /// </summary>
        public Dictionary<Guid, string> PestanyasDeBusqueda { get; set; }

        /// <summary>
        /// Guid con el tipo de pestaña seleccionada
        /// </summary>
        public Guid? PestanyasSeleccionadas { get; set; }
        /// <summary>
        /// Verdad si se permiten recursos privados
        /// </summary>
        public bool PermitirRecursosPrivados { get; set; }

        /// <summary>
        /// Verdad si las invitaciones están disponibles
        /// </summary>
        public bool InvitacionesDisponibles { get; set; }

        /// <summary>
        /// Verdad si las votaciones están disponibles
        /// </summary>
        public bool VotacionesDisponibles { get; set; }

        /// <summary>
        /// Verdad si se deben mostrar las votaciones en los recursos
        /// </summary>
        public bool MostrarVotaciones { get; set; }

        /// <summary>
        /// Verdad si se permiten votaciones negativas
        /// </summary>
        public bool PermitirVotacionesNegativas { get; set; }

        /// <summary>
        /// Verdad si los comentarios están disponibles
        /// </summary>
        public bool ComentariosDisponibles { get; set; }

        /// <summary>
        /// Verdad si los supervisores pueden administrar los grupos de usuarios
        /// </summary>
        public bool SupervisoresPuedenAdministrarGrupos { get; set; }

        /// <summary>
        /// Cuenta de twitter en la que publica la comunidad
        /// </summary>
        public string CuentaTwitter { get; set; }

        /// <summary>
        /// Hastags de twitter que se usa para publicar en twitter
        /// </summary>
        public string HasTagTwitter { get; set; }

        /// <summary>
        /// URL de envío a twitter
        /// </summary>
        public string UrlEnvioTwitter { get; set; }

        /// <summary>
        /// Token de twitter
        /// </summary>
        public string TokenTwitter { get; set; }

        /// <summary>
        /// Token secreto de twitter
        /// </summary>
        public string TokenSecretTwitter { get; set; }

        /// <summary>
        /// Verdad si se deben mostrar las acciones en los listados
        /// </summary>
        public bool MostrarAccionesListados { get; set; }

        /// <summary>
        /// Contenido de la meta Robots que aparecera en las páginas de búsqueda
        /// </summary>
        //public string RobotsBusqueda { get; set; }

        /// <summary>
        /// Verdad si se debe de incluir el script de google search que google muestre la caja de búsqueda de esta comunidad cuando aparezca en los resultados
        /// </summary>
        public bool IncluirGoogleSearch { get; set; }

        /// <summary>
        /// Configuración del correo para esta comunidad
        /// </summary>
        public ConfiguradorCorreo ConfiguracionCorreo { get; set; }

        /// <summary>
        /// Número de caracteres máximo de la descripcion de cada recurso en el boletín de suscripciones
        /// </summary>
        public string NumeroCaracteresDescripcionSuscripcion { get; set; }

        /// <summary>
        /// Parámetros extra para los vídeos de youtube (ej: &modestbranding=1)
        /// </summary>
        public string ParametrosExtraYoutube { get; set; }

        /// <summary>
        /// Verdad si los usuarios pueden compartir recursos
        /// </summary>
        public bool CompartirRecursoPermitido { get; set; }

        /// <summary>
        /// Código de Google Analytics de la cuenta en la que se van a registrar las visitas de esta comunidad (ej: UA-XXXXXXXX-1)
        /// </summary>
        //public string CodigoGoogleAnalytics { get; set; }

        /// <summary>
        /// Script de Google Analytics, si se quiere usar un script distinto al de por defecto
        /// </summary>
        //public string ScriptGoogleAnalytics { get; set; }
    }

    /// <summary>
    /// Clase que identifica la configuración de un buzón de correo
    /// </summary>
    public class ConfiguradorCorreo
    {
        /// <summary>
        /// Email de la cuenta de correo
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Servidor SMTP
        /// </summary>
        public string SMTP { get; set; }

        /// <summary>
        /// Puerto del servidor
        /// </summary>
        public short Port { get; set; }

        /// <summary>
        /// Usuario de la cuenta de correo
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Contraseña del usuario
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Tipo de cuenta (SMTP o EWS)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Verdad si el servidor requiere SSL
        /// </summary>
        public bool SSL { get; set; }

        /// <summary>
        /// Email de sugerencias
        /// </summary>
        public string SuggestEmail { get; set; }

		/// <summary>
		/// Email al que se envia el correo de prueba
		/// </summary>
		public string Destinatario { get; set; }  
    }
}
