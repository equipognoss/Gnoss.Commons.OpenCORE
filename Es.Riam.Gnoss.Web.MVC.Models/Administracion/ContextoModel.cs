using System;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public enum TipoGadget
    {
        /// <summary>
        /// Html incrustado.
        /// </summary>
        HtmlIncrustado = 0,
        /// <summary>
        /// Html incrustado para la parte superior de la home de una comunidad. Quitar
        /// </summary>
        HtmlPrincipalComunidad = 1,
        /// <summary>
        /// Los más interesante.
        /// </summary>
        LoMasInteresante = 3,
        /// <summary>
        /// Últimos debates.
        /// </summary>
        UltDebates = 4,
        /// <summary>
        /// Últimas preguntas.
        /// </summary>
        UltPreguntas = 5,
        /// <summary>
        /// Consulta libre a virtuoso. Consulta a una url de busqueda. revisar
        /// </summary>
        Consulta = 6,
        /// <summary>
        /// Últimas encuestas.
        /// </summary>
        UltEncuestas = 7,
        /// <summary>
        /// Proyectos relacionados.
        /// </summary>
        ProyRelacionados = 9,
        /// <summary>
        /// Que está pasando.
        /// </summary>
        QueEstaPasando = 12,
        /// <summary>
        /// Contextos.
        /// </summary>
        RecursosContextos = 20,
        /// <summary>
        /// Recursos relacionados
        /// </summary>
        RecursosRelacionados = 21,
        /// <summary>
        /// Temas didactalia. Deberiamos quitarlo
        /// </summary>
        TemasRelacionadosDidactalia = 22,
        /// <summary>
        /// Contextos con HTML plano.(Quitar cuando se haga el desarrollo de los filtros de destino)
        /// </summary>
        RecursosContextosHTMLplano = 23,
        /// <summary>
        /// Recursos mas visitados
        /// </summary>
        RecursosMasVistos = 24,
        /// <summary>
        /// Componente CMS
        /// </summary>
        CMS = 25
    }


    /// <summary>
    /// Modelo de gadget de una comunidad
    /// </summary>
    [Serializable]
    public partial class ContextoModel
    {
        /// <summary>
        /// Identificador del gadget
        /// </summary>
        public Guid Key { get; set; }
        /// <summary>
        /// Nombre del gadget
        /// </summary>
        public string Name { get; set; }

        public string Clases { get; set; }

        public bool Deleted { get; set; }

        public int Orden { get; set; }

        public bool Visible { get; set; }

        public string Contenido { get; set; }

        public TipoGadget TipoGadget { get; set; }

        public ContextModel Contexto { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaModificacion { get; set; }

        /// <summary>
        /// Indica si el gadget se va a cargar vía Ajax
        /// </summary>
        public bool Ajax { get; set; }

        public string FiltrosDestino { get; set; }

        public string ShortName { get; set; }

        /// <summary>
        /// Modelo de contexto de una comunidad
        /// </summary>
        [Serializable]
        public partial class ContextModel
        {
            /// <summary>
            /// Nombre corto del contexto
            /// </summary>
            public string ShortName { get; set; }

            public string ComunidadOrigen { get; set; }

            public string FiltrosOrigen { get; set; }

            public string RelacionOrigenDestino { get; set; }

            public short NumResultados { get; set; }

            public string OrdenResultados { get; set; }

            public short Imagen { get; set; }

            public bool MostrarEnlaceOriginal { get; set; }

            public bool MostrarVerMas { get; set; }

            public bool AbrirEnPestanyaNueva { get; set; }

            public string NamespacesExtra { get; set; }

            public string ResultadosExcluir { get; set; }
        }
    }
}
