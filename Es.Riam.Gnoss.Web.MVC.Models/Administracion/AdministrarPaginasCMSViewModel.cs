using Es.Riam.Gnoss.Web.MVC.Models.Flujos;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    /// <summary>
    /// Enumeración para distinguir tipos de propiedad de los de campos de un Menu
    /// </summary>
    public enum TipoPropiedadMenu
    {
        Nombre = 0,
        Enlace = 1,
        Nivel = 2
    }

    public class CambiarEstadoPaginasCMSViewModel
    {
		public Guid PestanyaID { get; set; }
		public Guid TransicionID { get; set; }
		public string Nombre { get; set; }
	}

	public class HistorialTransicionesPaginasCMSViewModel
	{
		public Guid PestanyaID { get; set; }
		public List<HistorialTransicionModel> ListaTransiciones { get; set; }
		public string NombrePagina { get; set; }
		public DateTime Fecha { get; set; }
		public string NombreEditor { get; set; }
	}

	/// <summary>
	/// ViewModel de la página de administrar pestañas
	/// </summary>
	[Serializable]
    public class AdministrarPaginasCMSViewModel
    {
        /// <summary>
        /// Identificador de la página
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Tipo de página
        /// </summary>
        public short Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<RowCMSModel> Rows { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool MostrarSoloCuerpo { get; set; }

        public List<TipoComponenteCMS> ListaComponentesPrivados { get; set; }

        public List<CMSComponentModel> ListaComponenteComunidad { get; set; }

        public bool ContieneMultiplesComponentes { get; set; }

        public DateTime FechaModificacion { get; set; }

		public EstadoModel Estado { get; set; }

		public List<HistorialTransicionModel> HistorialTransiciones { get; set; }

		/// <summary>
		/// Modelo de bloque de una página
		/// </summary>
		[Serializable]
        public partial class RowCMSModel
        {
            /// <summary>
            /// Identificador de la fila
            /// </summary>
            public Guid Key { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Attributes { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<ColCMSModel> Cols { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public partial class ColCMSModel
            {
                /// <summary>
                /// Identificador de la columna
                /// </summary>
                public Guid Key { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public string Class { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public List<ComponentCMSModel> Components { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public partial class ComponentCMSModel
                {
                    /// <summary>
                    /// Identificador de la columna
                    /// </summary>
                    public Guid Key { get; set; }
                    /// <summary>
                    /// 
                    /// </summary>
                    public string Type { get; set; }
                    /// <summary>
                    /// 
                    /// </summary>
                    public string Name { get; set; }
                    /// <summary>
                    /// 
                    /// </summary>
                    public Dictionary<short, KeyValuePair<string, bool>> Options { get; set; }
                }
            }
        }

        [Serializable]
        public partial class CMSComponentModel
        {
            public string Name { get; set; }

            public Guid Key { get; set; }
            public string Type { get; set; }
        }
    }
}
