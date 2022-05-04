using System;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{

    /// <summary>
    /// Modelo de cambios y conflictos de una comunidad
    /// </summary>
    [Serializable]
    public partial class ChangeModel
    {
        /// <summary>
        /// cadena con los atributos para el guardado
        /// </summary>
        public string CadenaAtributos { get; set; }
        /// <summary>
        /// Guardado
        /// </summary>
        public bool Guardar { get; set; }
    }
}
