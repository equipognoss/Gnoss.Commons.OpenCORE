using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public partial class ObjetoConocimientoModel
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Deleted { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Ontologia { get; set; }
        /// <summary>
        /// Nombre
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Nombre corto de la ontología
        /// </summary>
        public string ShortNameOntology { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Namespace { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NamespaceExtra { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NombreTesauroExclusivo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool CachearDatosSemanticos { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool EsBuscable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> Subtipos { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PresentacionModel PresentacionListado { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PresentacionModel PresentacionMosaico { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PresentacionModel PresentacionMapa { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PresentacionModel PresentacionRelacionados { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PresentacionPersonalizadoModel PresentacionPersonalizado { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public partial class PresentacionModel
        {
            public List<PropiedadModel> ListaPropiedades { get; set; }
            public bool MostrarDescripcion { get; set; }
            public bool MostrarPublicador { get; set; }
            public bool MostrarEtiquetas { get; set; }
            public bool MostrarCategorias { get; set; }

            public partial class PropiedadModel
            {
                public string Propiedad { get; set; }
                public string Presentacion { get; set; }
                public short Orden { get; set; }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public partial class PresentacionPersonalizadoModel
        {
            public List<PropiedadPersonalizadoModel> ListaPropiedades { get; set; }

            public partial class PropiedadPersonalizadoModel
            {
                public string Identificador { get; set; }
                public string Select { get; set; }
                public string Where { get; set; }
                public short Orden { get; set; }
            }
        }
    }
}
