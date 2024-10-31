namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AdministrarSeoGoogleViewModel
    {
        /// <summary>
        /// Contenido de la meta Robots que aparecera en las páginas de búsqueda que se guardará en la tabla ParametroProyecto con valor 0, 1 o vacio
        /// </summary>
        public string ValorRobotsBusqueda { get; set; }
        /// <summary>
        /// Contenido de la meta Robots que aparecera en las páginas de búsqueda [all; noindex,nofollow]
        /// </summary>
        public string RobotsBusqueda { get; set; }
        /// <summary>
        /// Código de Google Analytics de la cuenta en la que se van a registrar las visitas de esta comunidad (ej: UA-XXXXXXXX-1)
        /// </summary>
        public string CodigoGoogleAnalytics { get; set; }
        /// <summary>
        /// Script de Google Analytics prodio (diferente al de por defecto y el de la plataforma)
        /// </summary>
        public string ScriptGoogleAnalyticsPropio { get; set; }
        /// <summary>
        /// Script de Google Analytics de la plataforma
        /// </summary>
        public string ScriptGoogleAnalyticsPlataforma { get; set; }
        /// <summary>
        /// Flag que indica si la configuración de los Robots esta en la tabla ParametroProyecto
        /// </summary>
        public bool ConfiguracionEnParametroProyecto { get; set; }
        /// <summary>
        /// Flag que indica si la se utiliza un script de google analytics propio o el configurado en la plataforma
        /// </summary>
        public bool EsScriptGoogleAnalitycsPropio { get; set; }
        /// <summary>
        /// Script de google analytics por defecto de GNOSS
        /// </summary>
        public string ScriptPorDefecto { get; set; }
    }
}
