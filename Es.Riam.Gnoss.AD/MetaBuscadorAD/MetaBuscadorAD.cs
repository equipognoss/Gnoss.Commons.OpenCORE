using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.AD.MetaBuscadorAD
{
    #region Enumeraciones

    /// <summary>
    /// Lista de posibles resultados que puede obtener el meta buscador
    /// </summary>
    public enum TiposResultadosMetaBuscador
    {
        /// <summary>
        /// Vacío
        /// </summary>
        Empty = 0,
        /// <summary>
        /// Comunidad
        /// </summary>
        Comunidad,
        /// <summary>
        /// Identidad de Persona
        /// </summary>
        IdentidadPersona,
        /// <summary>
        /// Identidad de Organización
        /// </summary>
        IdentidadOrganizacion,
        /// <summary>
        /// Persona
        /// </summary>
        Persona,
        /// <summary>
        /// Organización
        /// </summary>
        Organizacion,
        /// <summary>
        /// Documento
        /// </summary>
        Documento,
        /// <summary>
        /// Documento 
        /// </summary>
        DocumentoBRPrivada,
        /// <summary>
        /// Documento
        /// </summary>
        DocumentoBRPersonal,
        /// <summary>
        /// Blog
        /// </summary>
        Blog,
        /// <summary>
        /// Entrada de un blog
        /// </summary>
        EntradaBlog,
        /// <summary>
        /// Dafo
        /// </summary>
        Dafo,
        /// <summary>
        /// Dafo SoyCreador
        /// </summary>
        DafoSoyCreador,
        /// <summary>
        /// Dafo SoyEditor
        /// </summary>
        DafoSoyEditor,
        /// <summary>
        /// Dafo SoyParticipante
        /// </summary>
        DafoSoyParticipante,
        /// <summary>
        /// Dafo Otros
        /// </summary>
        DafoOtros,
        /// <summary>
        /// Proceso
        /// </summary>
        Proceso,
        /// <summary>
        /// Objetivo
        /// </summary>
        Objetivo,
        /// <summary>
        /// Competencia
        /// </summary>
        Competencia,
        /// <summary>
        /// Pregunta
        /// </summary>
        Pregunta,
        /// <summary>
        /// Debate
        /// </summary>
        Debate,
        /// <summary>
        /// Encuesta
        /// </summary>
        Encuesta,
        /// <summary>
        /// Comentario
        /// </summary>
        Comentario,
        /// <summary>
        /// Grupo.
        /// </summary>
        Grupo,
        /// <summary>
        /// Mensaje.
        /// </summary>
        Mensaje,
        /// <summary>
        /// Invitacion.
        /// </summary>
        Invitacion,
        /// <summary>
        /// Suscripción.
        /// </summary>
        Suscripcion,
        /// <summary>
        /// GrupoContacto
        /// </summary>
        GrupoContacto,
        /// <summary>
        /// OrgContacto
        /// </summary>
        OrgContacto,
        /// <summary>
        /// PerContacto
        /// </summary>
        PerContacto,
        /// <summary>
        /// Pagina del CMS.
        /// </summary>
        PaginaCMS
    }

    /// <summary>
    /// Lista de posibles campos por los que se puede ordenar
    /// </summary>
    public enum TipoCampoAOrdenar
    {
        /// <summary>
        /// Relevancia
        /// </summary>
        Relevancia = 0,
        /// <summary>
        /// Importancia
        /// </summary>
        Importancia = 1,
        /// <summary>
        /// Nombre
        /// </summary>
        Nombre = 2,
        /// <summary>
        /// Apellidos
        /// </summary>
        Apellidos = 3
    }

    /// <summary>
    /// Tipo de consulta a lanzar.
    /// </summary>
    public enum TipoCosulta
    {
        /// <summary>
        /// Consulta traerá el número de resultados totales y los resultados.
        /// </summary>
        NumResultados_Y_Resultados = 0,
        /// <summary>
        /// Consulta traerá el número de resultados totales.
        /// </summary>
        NumResultados = 1,
        /// <summary>
        /// Consulta traerá los resultados.
        /// </summary>
        Resultados = 2,
        /// <summary>
        /// Consulta traerá el número de resultados coincidentes con los filtros(destacados) y los resultados.
        /// </summary>
        NumResulCoincidentes_Y_Resultados = 3,
        /// <summary>
        /// Consulta traerá el número de resultados coincidentes con los filtros(destacados).
        /// </summary>
        NumResulCoincidentes = 4,
    }

    #endregion

    /// <summary>
    /// Acceso a datos del metabuscador
    /// </summary>
    public class MetaBuscadorAD : BaseAD
    {
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public MetaBuscadorAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuración de la conexión a la base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de la conexión a la base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public MetaBuscadorAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
        }

        #endregion

        #region Métodos generales

        /// <summary>
        /// Crea la condición para el where de comunidades (lo devuelve de la manera: " Identidad.ProyectoID = 'p1' " o " Identidad.ProyectoID IN ('p1', 'p2', 'p3'...) ")
        /// </summary>
        /// <param name="pComunidades">Lista de comunidades filtradas</param>
        /// <param name="pCampoProyectoID">Campo ProyectoID (ej: "Identidad.ProyectoID")</param>
        /// <returns>devuelve un string de la manera: " Identidad.ProyectoID = 'p1' " o " Identidad.ProyectoID IN ('p1', 'p2', 'p3'...) "</returns>
        private string CrearCondicionWhereComunidades(List<Guid> pComunidades, string pCampoProyectoID, bool pQuitarMyGnoss)
        {
            string separador = pCampoProyectoID + " = ";
            string whereProy = "";
            string parentesis = "";

            if (pComunidades.Count > 0)
            {
                if (pComunidades.Count > 1)
                {
                    whereProy += pCampoProyectoID + " IN (";
                    separador = "";
                    parentesis = ")";
                }
                foreach (Guid comunidadID in pComunidades)
                {
                    if ((!pQuitarMyGnoss) || (comunidadID != ProyectoAD.MetaProyecto))
                    {
                        whereProy += separador + IBD.GuidValor(comunidadID);
                        separador += ", ";
                    }
                }
            }

            if (!string.IsNullOrEmpty(whereProy))
            {
                whereProy += parentesis + " ";
            }

            return whereProy;
        }

        #region Estáticos

        /// <summary>
        /// Indica, según la lista de opciones, si se debe buscar o no recursos.
        /// </summary>
        /// <param name="pOpcionesBusqueda">Opciones de búsqueda</param>
        /// <returns>Indica, según la lista de opciones, si se debe buscar o no recursos</returns>
        public static bool BucarRecursos(List<TiposResultadosMetaBuscador> pOpcionesBusqueda)
        {
            return (pOpcionesBusqueda.Contains(TiposResultadosMetaBuscador.Debate) || pOpcionesBusqueda.Contains(TiposResultadosMetaBuscador.Documento) || pOpcionesBusqueda.Contains(TiposResultadosMetaBuscador.Pregunta) || pOpcionesBusqueda.Contains(TiposResultadosMetaBuscador.DocumentoBRPersonal) || pOpcionesBusqueda.Contains(TiposResultadosMetaBuscador.DocumentoBRPrivada));
        }

        /// <summary>
        /// Indica, según la lista de opciones, si se debe buscar o no dafos.
        /// </summary>
        /// <param name="pOpcionesBusqueda">Opciones de búsqueda</param>
        /// <returns>Indica, según la lista de opciones, si se debe buscar o no dafos</returns>
        public static bool BucarDafos(List<TiposResultadosMetaBuscador> pOpcionesBusqueda)
        {
            return (pOpcionesBusqueda.Contains(TiposResultadosMetaBuscador.Dafo) || pOpcionesBusqueda.Contains(TiposResultadosMetaBuscador.DafoOtros) || pOpcionesBusqueda.Contains(TiposResultadosMetaBuscador.DafoSoyCreador) || pOpcionesBusqueda.Contains(TiposResultadosMetaBuscador.DafoSoyEditor) || pOpcionesBusqueda.Contains(TiposResultadosMetaBuscador.DafoSoyParticipante));
        }

        /// <summary>
        /// Indica, según la lista de opciones, si se debe buscar o no personas y organizaciones.
        /// </summary>
        /// <param name="pOpcionesBusqueda">Opciones de búsqueda</param>
        /// <returns>Indica, según la lista de opciones, si se debe buscar o no personas y organizaciones</returns>
        public static bool BucarPersonasYOrganizaciones(List<TiposResultadosMetaBuscador> pOpcionesBusqueda)
        {
            return (pOpcionesBusqueda.Contains(TiposResultadosMetaBuscador.IdentidadOrganizacion) || pOpcionesBusqueda.Contains(TiposResultadosMetaBuscador.IdentidadPersona) || pOpcionesBusqueda.Contains(TiposResultadosMetaBuscador.Persona) || pOpcionesBusqueda.Contains(TiposResultadosMetaBuscador.Organizacion));
        }

        /// <summary>
        /// Indica, según la lista de opciones, si se debe buscar o no blogs y entradas de blogs.
        /// </summary>
        /// <param name="pOpcionesBusqueda">Opciones de búsqueda</param>
        /// <returns>Indica, según la lista de opciones, si se debe buscar o no blogs y entradas de blogs</returns>
        public static bool BucarBlogs(List<TiposResultadosMetaBuscador> pOpcionesBusqueda)
        {
            return (pOpcionesBusqueda.Contains(TiposResultadosMetaBuscador.Blog) || pOpcionesBusqueda.Contains(TiposResultadosMetaBuscador.EntradaBlog));
        }

        #endregion

        #endregion
    }
}
