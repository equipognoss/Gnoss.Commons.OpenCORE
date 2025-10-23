using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public enum TiposContenidos
    {
        Link = 0,
        Video = 2,
        Adjunto = 3,
        RecursoSemantico = 5,
        Nota = 8,
        Debate = 16,
        Encuesta = 18,
        PaginaCMS = 19,
        ComponenteCMS = 20,
    }

    public enum TipoEstado
    {
        Inicial,
        Intermedio,
        Final
    }
    public class AdministrarFlujosViewModel
    {
        public Dictionary<string, string> Idiomas { get; set; }
        public string IdiomaPorDefecto { get; set; }
        public List<FlujoViewModel> ListaFlujos { get; set; }

    }

    public class FlujoViewModel
    {
        public FlujoViewModel()
        {
            Estados = new List<EstadoViewModel>();
            Transiciones = new List<TransicionViewModel>();
            TiposRecursos = new Dictionary<TiposContenidos, bool> { { TiposContenidos.Nota, false }, { TiposContenidos.Adjunto, false }, { TiposContenidos.Video, false }, { TiposContenidos.Link, false }, { TiposContenidos.Encuesta, false }, { TiposContenidos.Debate, false }, { TiposContenidos.PaginaCMS, false }, { TiposContenidos.ComponenteCMS, false }, { TiposContenidos.RecursoSemantico, false }, };
            OntologiasProyecto = new Dictionary<Guid, string>();
            OntologiasProyectoNombre = new List<string>();
        }
        public Guid FlujoID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public Guid OrganizacionID { get; set; }
        public Guid ProyectoID { get; set; }
        public DateTime Fecha {  get; set; }
        public List<EstadoViewModel> Estados { get; set; }
        public List<TransicionViewModel> Transiciones { get; set; }
        public Dictionary<TiposContenidos, bool> TiposRecursos { get; set; }
        public List<string> OntologiasProyectoNombre { get; set; }
        public Dictionary<Guid, string> OntologiasProyecto { get; set; }
        public bool Nuevo {  get; set; } = false;
    }

    public class EstadoViewModel
    {
        public Guid EstadoID { get; set; }
        public Guid FlujoID { get; set; }
        public string Nombre { get; set; }
        public bool Publico { get; set; } = true;
        public List<EstadoIdentidadViewModel> ListaEstadoIdentidad { get; set; } = new List<EstadoIdentidadViewModel>();
        public List<EstadoGrupoViewModel> ListaEstadoGrupo{ get; set; } = new List<EstadoGrupoViewModel>();
        public bool Eliminado { get; set; } = false;
        public TipoEstado TipoEstado { get; set; }
        public string Color { get; set; }
    }

    public class EstadoIdentidadViewModel
    {
        public Guid EstadoID { get; set; }
        public Guid IdentidadID { get; set; }
        public Guid PerfilID { get; set; }
        public string Nombre { get; set; }
        public bool Editor { get; set; }
    }
    public class EstadoGrupoViewModel
    {
        public Guid EstadoID { get; set; }
        public Guid GrupoID { get; set; }
        public bool Editor { get; set; }
        public string Nombre { get; set; }
    }

    public class TransicionViewModel
    {
        public Guid TransicionID { get; set; }
        public string Nombre { get; set; }
        public EstadoViewModel EstadoOrigen { get; set; }
        public EstadoViewModel EstadoDestino { get; set; }
        public List<Guid> ListaTransicionIdentidadPerfiles { get; set; } = new List<Guid>();
        public List<Guid> ListaTransicionGrupo { get; set; } = new List<Guid>();
        /// <summary>
        /// Guid -> Perfil obtenido a partir de la identidad de BD
        /// String -> Nombre del perfil
        /// </summary>
        public Dictionary<Guid, string> ListaNombreTransicionIdentidad { get; set; } = new Dictionary<Guid, string>();
        public Dictionary<Guid, string> ListaNombreTransicionGrupo { get; set; } = new Dictionary<Guid, string>();
        public bool Eliminado { get; set; } = false;
    }

}
