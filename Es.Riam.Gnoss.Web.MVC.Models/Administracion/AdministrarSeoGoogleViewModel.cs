﻿namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
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
        /// Script de Google Analytics que viene desde la vista para el guardado
        /// </summary>
        public string ScriptGoogleAnalytics { get; set; }
        /// <summary>
        /// Script de Google Analytics de la plataforma
        /// </summary>
        public string ScriptGoogleAnalyticsPlataforma { get; set; }
        /// <summary>
        /// Flag que indica si la configuración de los Robots esta en la tabla ParametroProyecto
        /// </summary>
        public bool ConfiguracionEnParametroProyecto { get; set; }
        /// <summary>
        /// Flag que indica si la configuración del script de Google Analytics esta en la tabla ParametroGeneral
        /// </summary>
        public bool GooglaAnalitycsScriptEnParametroGeneral { get; set; }
        public string ScriptPorDefecto { get; set; }
    }
}
