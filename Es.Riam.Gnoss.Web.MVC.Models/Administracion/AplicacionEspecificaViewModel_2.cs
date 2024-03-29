﻿using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AplicacionEspecificaViewModel_2
    {
        public AplicacionEspecifica aplicaciones { get; set; }
        public List<Variables> variables { get; set; }
        // Nombre corto de la personalización
        public string NombreCortoPersonalizacion { get; set; }
        public string UrlRepositorio { get; set; }
        public Guid EntornoID { get; set; }
        public string UrlBaseServicios { get; set; }
    }
}
