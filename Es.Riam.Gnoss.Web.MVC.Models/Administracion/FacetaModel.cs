using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    /// <summary>
    /// 
    /// </summary>
    public enum TipoFaceta
    {
        /// <summary>
        /// 
        /// </summary>
        Texto = 0,
        /// <summary>
        /// 
        /// </summary>
        Numero = 1,
        /// <summary>
        /// 
        /// </summary>
        Fecha = 2,
        /// <summary>
        /// 
        /// </summary>
        Tesauro = 3,
        /// <summary>
        /// 
        /// </summary>
        TextoInvariable = 4,
        /// <summary>
        /// 
        /// </summary>
        Siglo = 5,
    }

    /// <summary>
    /// Modelo de facetas de una comunidad
    /// </summary>
    [Serializable]
    public partial class FacetaModel
    {
        /// <summary>
        /// Nombre de la facetas
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Nombre de la facetas
        /// </summary>
        public TipoFaceta Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public short Orden { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Deleted { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ClaveFaceta { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Reciprocidad { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ClaveFacetaYReprocidad { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> ObjetosConocimiento { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public short Presentacion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public short Disenyo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public short AlgoritmoTransformacion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Filtros { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> ListFiltro { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public short NumElementosVisibles { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool EsSemantica { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Autocompletar { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public short Comportamiento { get; set; } // TipoMostrarSoloCaja
        /// <summary>
        /// 
        /// </summary>
        public bool Excluyente { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool OcultaEnFacetas { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool OcultaEnFiltros { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<Guid, string> PrivacidadGrupos { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool ComportamientoOr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Condicion { get; set; }
        /// <summary>
        /// Indica si la faceta afecta al orden de los resultados, priorizando los resultados que contengan el valor de la faceta en el título
        /// </summary>
        public bool PriorizarOrdenResultados { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Inmutable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid? AgrupacionID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<FiltrosFacetas> ListaFiltrosFacetas { get; set; }

        /// <summary>
        /// Indica si la faceta esta eliminada o no
        /// </summary>
        public bool Modified { get; set; }

        /// <summary>
        /// Indica la fecha de creacion/modificacion de la faceta
        /// </summary>
        public DateTime FechaModificacion { get; set; }
		/// <summary>
		/// Indica el ID de identificación en el caso que la faceta sea una faceta propuesta
		/// </summary> 
		public Guid SuggestedID { get; set; }
        /// <summary>
        /// Consulta de la faceta
        /// </summary>
        public string Query { get; set; }
        /// <summary>
        /// Indica si debe traer o no los contadores de la faceta
        /// </summary>
        public bool MostrarContador { get; set; } = true;

        [Serializable]
        public partial class FiltrosFacetas
        {
            /// <summary>
            /// 
            /// </summary>
            public string Key { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public short Orden { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public bool Deleted { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Nombre { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Condicion { get; set; }

        }

    }
}
