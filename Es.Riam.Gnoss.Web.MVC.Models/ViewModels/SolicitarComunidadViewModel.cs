﻿using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.ViewModels
{
    /// <summary>
    /// View model para la solicitud de comunidad
    /// </summary>
    public class SolicitarComunidadViewModel
    {
        /// <summary>
        /// Lista de comunidades privadas que administra el usuario actual
        /// </summary>
        public Dictionary<Guid, string> ComunidadesPrivadasAdministraUsuario { get; set; }

        public bool AceptacionAutomaticaDeComunidad { get; set; }

    }
}
