using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Es.Riam.Gnoss.Web.MVC.Models
{
    /// <summary>
    /// Modelo del objeto buscador
    /// </summary>
    [Serializable]
    public abstract class ObjetoBuscadorModel
    {
        /// <summary>
        /// Url para las búsquedas
        /// </summary>
        public string UrlSearch { get; set; }

        public int CacheVersion { get; set; }
    }

    /// <summary>
    /// Modelo de resultados de una busqueda
    /// </summary>
    [Serializable]
    public partial class ResultadoModel
    {
        /// <summary>
        /// Tipos de búsqueda
        /// </summary>
        public enum TiposBusquedaMVC
        {
            /// <summary>
            /// Recursos
            /// </summary>
            Recursos = 0,
            /// <summary>
            /// Debates
            /// </summary>
            Debates,
            /// <summary>
            /// Preguntas
            /// </summary>
            Preguntas,
            /// <summary>
            /// Encuestas
            /// </summary>
            Encuestas,
            /// <summary>
            /// Dafos
            /// </summary>
            Dafos,
            /// <summary>
            /// Personas y organizaciones
            /// </summary>
            PersonasYOrganizaciones,
            /// <summary>
            /// Búsqueda avanzada
            /// </summary>
            BusquedaAvanzada,
            /// <summary>
            /// Comunidades
            /// </summary>
            Comunidades,
            /// <summary>
            /// Blogs
            /// </summary>
            Blogs,
            /// <summary>
            /// ArticuloBlogs
            /// </summary>
            ArticuloBlogs,
            /// <summary>
            /// EditarRecursosPerfil
            /// </summary>
            EditarRecursosPerfil,
            /// <summary>
            /// Contribuciones
            /// </summary>
            Contribuciones,
            /// <summary>
            /// Mensajes.
            /// </summary>
            Mensajes,
            /// <summary>
            /// Comentarios.
            /// </summary>
            Comentarios,
            /// <summary>
            /// Invitaciones.
            /// </summary>
            Invitaciones,
            /// <summary>
            /// Suscripciones.
            /// </summary>
            Suscripciones,
            /// <summary>
            /// Contactos
            /// </summary>
            Contactos,
            /// <summary>
            /// Recomendaciones.
            /// </summary>
            Recomendaciones,
            /// <summary>
            /// Recomendaciones de proyectos.
            /// </summary>
            RecomendacionesProys,
            /// <summary>
            /// VerRecursosPerfil
            /// </summary>
            VerRecursosPerfil,
            /// <summary>
            /// Notificaciones (comentarios e invitaciones juntas)
            /// </summary>
            Notificaciones

        }

        /// <summary>
        /// Lista de resultados de la busqueda
        /// </summary>
        public List<ObjetoBuscadorModel> ListaResultados { get; set; }

        /// <summary>
        /// Numero total de resultados de la busqueda. -1 si debe ocultarse el número.
        /// </summary>
        public int NumeroResultadosTotal { get; set; }
        /// <summary>
        /// Pagina actual de la busqueda
        /// </summary>
        public int NumeroPaginaActual { get; set; }
        /// <summary>
        /// Numero de resultados por pagina
        /// </summary>
        public int NumeroResultadosPagina { get; set; }
        /// <summary>
        /// Url de la busqueda
        /// </summary>
        public string UrlBusqueda { get; set; }
        /// <summary>
        /// TExto sin resultados
        /// </summary>
        public string TextoSinResultados { get; set; }
        /// <summary>
        /// Tipo de la busqueda
        /// </summary>
        public TiposBusquedaMVC TipoBusqueda { get; set; }

        /// <summary>
        /// Indica si la petición se ha realizado desde "backOffice / DevTools"
        /// </summary>
        public bool AdministradorVeTodasPersonas { get; set; }


        /// <summary>
        /// Indica que la búsqueda es de tipo mapa.
        /// </summary>
        public bool MapView { get; set; }
    }

    /// <summary>
    /// Lista de permiosos para las páginas de un usuario
    /// </summary>
    public class PermisosPaginaUsuarioModel
    {
        /// <summary>
        /// Identificador del usaurios
        /// </summary>
        public Guid IdentidadID { get; set; }

        /// <summary>
        /// Si tiene o no permisos para las páginas de diseño
        /// </summary>
        public bool Disenyo { get; set; }

        /// <summary>
        /// Si tiene o no permisos para la página de administrar páginas
        /// </summary>
        public bool Pagina { get; set; }

        /// <summary>
        /// Si tiene o no permisos para las páginas de administración semántica
        /// </summary>
        public bool Semantica { get; set; }

        /// <summary>
        /// Si tiene o no permisos para la página de administrar tesauros
        /// </summary>
        public bool Tesauro { get; set; }

        /// <summary>
        /// Si tiene o no permiso para adminsitrar los textos
        /// </summary>
        public bool Texto { get; set; }
    }
}
