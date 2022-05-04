using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    /// <summary>
    /// View model de la pagina "BorradoMasivo"
    /// </summary>
    [Serializable]
    public class BorradoMasivoViewModel
    {
        public Dictionary<Guid, string> OntologiaABorrar { get; set; }

        public List<Guid> OntologiaSeleccionada { get; set; }

    }
}
