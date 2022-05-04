using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    /// <summary>
    /// View Model para el controlador AdmistrarServiciosExternos
    /// </summary>
    [Serializable]
    public class AdministrarServiciosExternosViewModel
    {
        /// <summary>
        ///Lista de servicios que me pasan
        /// </summary>
        public List<ServiceNameModel> ListaServicios { get; set; }
        /// <summary>
        ///Nombre que se la ha dado al servicio
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        ///Es cierto si el usuario que ha entrado es administrador del proyecto
        /// </summary>
        public bool EsAdministrador { get; set; }
        /// <summary>
        ///Es cierto si esta configurada la UrlBaseService
        /// </summary>
        public bool EstaConfigurado { get; set; }
        /// <summary>
        ///Lista de nombres del servicio
        /// </summary>
        public List<string> ListaNombres { get; set; }
        /// <summary>
        ///Lista de las url del servicio
        /// </summary>
        public List<string> UrlServicio { get; set; }

    }
}
