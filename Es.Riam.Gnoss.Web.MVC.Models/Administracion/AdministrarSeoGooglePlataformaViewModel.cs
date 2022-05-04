namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AdministrarSeoGooglePlataformaViewModel
    {
        /// <summary>
        /// Contenido de la meta Robots que aparecera en las páginas de búsqueda [all; noindex,nofollow]
        /// </summary>
        public string RobotsBusqueda { get; set; }
        /// <summary>
        /// Código de Google Analytics de la cuenta en la que se van a registrar las visitas de esta comunidad (ej: UA-XXXXXXXX-1)
        /// </summary>
        public string CodigoGoogleAnalytics { get; set; }
        /// <summary>
        /// Script de Google Analytics, si se quiere usar un script distinto al de por defecto
        /// </summary>
        public string ScriptGoogleAnalytics { get; set; }
        /// <summary>
        /// Flag que indica si la configuración se indica en la tabla ParametroAplicacion
        /// </summary>
        public bool ConfiguracionEnParametroAplicacion { get; set; }
        /// <summary>
        /// Contenido de la meta Robots del archivo de configuración project.config
        /// </summary>
        public string RobotsEnConfig { get; set; }
        public string ScriptPorDefecto { get; set; }
    }
}
