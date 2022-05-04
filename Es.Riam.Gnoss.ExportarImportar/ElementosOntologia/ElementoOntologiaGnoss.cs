using System.Collections.Generic;
using Es.Riam.Semantica.OWL;

namespace Es.Riam.Gnoss.ExportarImportar.ElementosOntologia
{
    #region Quitar
    ///// <summary>
    ///// Enumeración de tipos
    ///// </summary>
    //public enum TipoElementoGnoss
    //{
    //    /// <summary>
    //    /// Entidad tipo de proceso
    //    /// </summary>
    //    TipoProceso = 0,

    //    /// <summary>
    //    /// Entidad tipo de grupo funcional
    //    /// </summary>
    //    TipoGrupoFuncional,

    //    /// <summary>
    //    /// Entidad gnoss
    //    /// </summary>
    //    EntidadGnoss,


    //    /// <summary>
    //    /// Entidad escala metas
    //    /// </summary>
    //    EscalaMetas,


    //    /// <summary>
    //    /// Entidad estructura
    //    /// </summary>
    //    Estructura,


    //    /// <summary>
    //    /// Entidad indicador
    //    /// </summary>
    //    Indicador,


    //    /// <summary>
    //    /// Entidad modo de métrica
    //    /// </summary>
    //    ModoMetrica,


    //    /// <summary>
    //    /// Entidad grupo
    //    /// </summary>
    //    Grupo,


    //    /// <summary>
    //    /// Entidad objetivo
    //    /// </summary>
    //    Objetivo,


    //    /// <summary>
    //    /// Entidad conjunto de objetivos
    //    /// </summary>
    //    ConjuntoObjetivos,


    //    /// <summary>
    //    /// Entidad miniLibro
    //    /// </summary>
    //    MiniLibro,


    //    /// <summary>
    //    /// Entidad proceso
    //    /// </summary>
    //    Proceso,


    //    /// <summary>
    //    /// Entidad proyecto
    //    /// </summary>
    //    Proyecto,


    //    /// <summary>
    //    /// Entidad elemento de estructura
    //    /// </summary>
    //    ElementoEstructura,


    //    /// <summary>
    //    /// Entidad no exportable
    //    /// </summary>
    //    EntidadNoExportable,


    //    /// <summary>
    //    /// Entidad metaestructura
    //    /// </summary>
    //    MetaEstructura,


    //    /// <summary>
    //    /// Entidad norma
    //    /// </summary>
    //    Norma,


    //    /// <summary>
    //    /// Entidad figura
    //    /// </summary>
    //    Figura,


    //    /// <summary>
    //    /// Entidad forma
    //    /// </summary>
    //    Forma,


    //    /// <summary>
    //    /// Entidad Persona
    //    /// </summary>
    //    Persona,

    //    /// <summary>
    //    /// Entidad ocupación
    //    /// </summary>
    //    Ocupacion,


    //    /// <summary>
    //    /// Entidad dimensión competencia
    //    /// </summary>
    //    DimensionCompetencia,


    //    /// <summary>
    //    /// Entidad tipo de dimensión
    //    /// </summary>
    //    TipoDimensionCompetencia,

    //    /// <summary>
    //    /// Entidad exportable
    //    /// </summary>
    //    EntidadExportable,

    //    /// <summary>
    //    /// Entidad libro
    //    /// </summary>
    //    Libro,

    //    /// <summary>
    //    /// Entidad metrica
    //    /// </summary>
    //    Metrica,

    //    /// <summary>
    //    /// Entidad subestructura
    //    /// </summary>
    //    SubEstructura,

    //    /// <summary>
    //    /// Entidad tipo de grupo
    //    /// </summary>
    //    TipoGrupo,

    //    /// <summary>
    //    /// Entidad competencia
    //    /// </summary>
    //    Competencia,

    //    /// <summary>
    //    /// Entidad entrada
    //    /// </summary>
    //    Entrada,

    //    /// <summary>
    //    /// Entidad salida
    //    /// </summary>
    //    Salida,

    //    /// <summary>
    //    /// Entidad control
    //    /// </summary>
    //    Control,

    //    /// <summary>
    //    /// Entidad mecanismo
    //    /// </summary>
    //    Mecanismo,

    //    /// <summary>
    //    /// Entidad elemento de modo
    //    /// </summary>
    //    ElementoNivelModo,

    //    /// <summary>
    //    /// Entidad organización
    //    /// </summary>
    //    Organizacion,

    //    /// <summary>
    //    /// Entidad perspectiva
    //    /// </summary>
    //    Perspectiva,

    //    /// <summary>
    //    /// Entidad documento
    //    /// </summary>
    //    Documento,

    //    /// <summary>
    //    /// Entidad categoría de documentación
    //    /// </summary>
    //    CategoriaDocumentacion,

    //    /// <summary>
    //    /// Entidad definición
    //    /// </summary>
    //    Definicion,

    //    /// <summary>
    //    /// Entidad nivel de libro
    //    /// </summary>
    //    NivelLibro,

    //    /// <summary>
    //    /// Entidad nivel de modo
    //    /// </summary>
    //    NivelModo,

    //    /// <summary>
    //    /// Entidad de modo de libro
    //    /// </summary>
    //    ModoLibro,

    //    /// <summary>
    //    /// Entidad que representa un elemento de IDEF0
    //    /// </summary>
    //    ElementoIDEF0,

    //    /// <summary>
    //    /// Entidad que representa un eje de metas
    //    /// </summary>
    //    EjeMeta,

    //    /// <summary>
    //    /// Entidad que representa una fila de metas
    //    /// </summary>
    //    FilaMeta,

    //    /// <summary>
    //    /// Entidad que representa una celda de metas
    //    /// </summary>
    //    CeldaMeta,

    //    /// <summary>
    //    /// Entidad que representa la meta que tiene una competencia o dimensión con una figura
    //    /// </summary>
    //    Meta,

    //    /// <summary>
    //    /// Entidad que representa un puesto de trabajo
    //    /// </summary>
    //    PuestoTrabajo,

    //    /// <summary>
    //    /// Entidad que representa un grupo funcinal
    //    /// </summary>
    //    GrupoFuncional,

    //    /// <summary>
    //    /// Entidad que representa un gestor de procesos
    //    /// </summary>
    //    GestionProcesos,

    //    /// <summary>
    //    /// Entidad que representa un gestor de objetivos
    //    /// </summary>
    //    GestionObjetivos,

    //    /// <summary>
    //    /// Entidad que representa un Gestor de grupos funcionales
    //    /// </summary>
    //    GestionGruposFuncionales,

    //    /// <summary>
    //    /// Entidad que representa las categorias del tesauro
    //    /// </summary>
    //    CategoriasTesauro,

    //    /// <summary>
    //    /// Entidad que representa la wiki 
    //    /// </summary>
    //    Wiki,

    //    /// <summary>
    //    /// Entidad que representa un comentario
    //    /// </summary>
    //    Comentario,

    //    /// <summary>
    //    /// Entidad que representa un nivel de prioridad del proyecto
    //    /// </summary>
    //    NivelPrioridadProyecto,

    //    /// <summary>
    //    /// Entidad que representa un tesauro
    //    /// </summary>
    //    Tesauro,

    //    /// <summary>
    //    /// Entidad que representa los parametros del proyecto
    //    /// </summary>
    //    ParametrosProyecto,

    //    /// <summary>
    //    /// Entidad que representa los parametros del proyecto
    //    /// </summary>
    //    Tag,

    //    /// <summary>
    //    /// Entidad que representa un dafo
    //    /// </summary>
    //    DafoProyecto,

    //    /// <summary>
    //    /// Entidad que representa un factor de dafo
    //    /// </summary>
    //    FactorDafoProyecto,

    //    Evento,

    //    Personalizacion,

    //    Curriculum,



    //}

    ///// <summary>
    ///// Tipo de entidad
    ///// </summary>
    //public class UtilTipoElementoGnoss
    //{
    //    #region metodos publicos
    //    /// <summary>
    //    /// Convierte un tipo de entidad en string
    //    /// </summary>
    //    /// <param name="pTipoEntidad">Tipo de entidad</param>
    //    /// <returns></returns>
    //    public static string TipoEntidadToString(TipoElementoGnoss pTipoEntidad)
    //    {
    //        switch (pTipoEntidad)
    //        {
    //            case TipoElementoGnoss.TipoProceso:
    //                return "TipoProceso";
    //            case TipoElementoGnoss.TipoGrupoFuncional:
    //                return "TipoGrupoFuncional";
    //            case TipoElementoGnoss.Competencia:
    //                return "Competencia";
    //            case TipoElementoGnoss.EntidadGnoss:
    //                return "EntidadGnoss";
    //            case TipoElementoGnoss.EscalaMetas:
    //                return "EscalaMetas";
    //            case TipoElementoGnoss.Estructura:
    //                return "Estructura";
    //            case TipoElementoGnoss.Indicador:
    //                return "Indicador";
    //            case TipoElementoGnoss.ModoMetrica:
    //                return "ModoMetrica";
    //            case TipoElementoGnoss.Grupo:
    //                return "Grupo";
    //            case TipoElementoGnoss.Objetivo:
    //                return "Objetivo";
    //            case TipoElementoGnoss.GrupoFuncional:
    //                return "GrupoFuncional";
    //            case TipoElementoGnoss.ConjuntoObjetivos:
    //                return "ConjuntoObjetivos";
    //            case TipoElementoGnoss.MiniLibro:
    //                return "MiniLibro";
    //            case TipoElementoGnoss.Proceso:
    //                return "Proceso";
    //            case TipoElementoGnoss.Proyecto:
    //                return "Proyecto";
    //            case TipoElementoGnoss.ElementoEstructura:
    //                return "ElementoEstructura";
    //            case TipoElementoGnoss.EntidadNoExportable:
    //                return "EntidadNoExportable";
    //            case TipoElementoGnoss.MetaEstructura:
    //                return "MetaEstructura";
    //            case TipoElementoGnoss.Norma:
    //                return "Norma";
    //            case TipoElementoGnoss.Figura:
    //                return "Figura";
    //            case TipoElementoGnoss.Persona:
    //                return "Persona";
    //            case TipoElementoGnoss.Ocupacion:
    //                return "Ocupacion";
    //            case TipoElementoGnoss.Forma:
    //                return "Forma";
    //            case TipoElementoGnoss.DimensionCompetencia:
    //                return "DimensionCompetencia";
    //            case TipoElementoGnoss.TipoDimensionCompetencia:
    //                return "TipoDimensionCompetencia";
    //            case TipoElementoGnoss.EntidadExportable:
    //                return "EntidadExportable";
    //            case TipoElementoGnoss.Libro:
    //                return "Libro";
    //            case TipoElementoGnoss.Metrica:
    //                return "Metrica";
    //            case TipoElementoGnoss.SubEstructura:
    //                return "SubEstructura";
    //            case TipoElementoGnoss.TipoGrupo:
    //                return "TipoGrupo";
    //            case TipoElementoGnoss.Entrada:
    //                return "Entrada";
    //            case TipoElementoGnoss.Salida:
    //                return "Salida";
    //            case TipoElementoGnoss.Control:
    //                return "Control";
    //            case TipoElementoGnoss.Mecanismo:
    //                return "Mecanismo";
    //            case TipoElementoGnoss.ElementoNivelModo:
    //                return "ElementoNivelModo";
    //            case TipoElementoGnoss.Organizacion:
    //                return "Organizacion";
    //            case TipoElementoGnoss.Perspectiva:
    //                return "Perspectiva";
    //            case TipoElementoGnoss.Documento:
    //                return "Documento";
    //            case TipoElementoGnoss.CategoriaDocumentacion:
    //                return "CategoriaDocumentacion";
    //            case TipoElementoGnoss.Definicion:
    //                return "Definicion";
    //            case TipoElementoGnoss.NivelLibro:
    //                return "NivelLibro";
    //            case TipoElementoGnoss.NivelModo:
    //                return "NivelModo";
    //            case TipoElementoGnoss.ElementoIDEF0:
    //                return "ElementoIDEF0";
    //            case TipoElementoGnoss.ModoLibro:
    //                return "ModoLibro";
    //            case TipoElementoGnoss.EjeMeta:
    //                return "EjeMeta";
    //            case TipoElementoGnoss.CeldaMeta:
    //                return "CeldaMeta";
    //            case TipoElementoGnoss.FilaMeta:
    //                return "FilaMeta";
    //            case TipoElementoGnoss.Meta:
    //                return "MetaAsignadaElementoEstructura";
    //            case TipoElementoGnoss.PuestoTrabajo:
    //                return "PuestoTrabajo";
    //            case TipoElementoGnoss.GestionProcesos:
    //                return "GestionProcesos";
    //            case TipoElementoGnoss.GestionObjetivos:
    //                return "GestionObjetivos";
    //            case TipoElementoGnoss.GestionGruposFuncionales:
    //                return "GestionGruposFuncionales";
    //            case TipoElementoGnoss.CategoriasTesauro:
    //                return "CategoriaTesauro";
    //            case TipoElementoGnoss.Wiki:
    //                return "Wiki";
    //            case TipoElementoGnoss.Comentario:
    //                return "Comentario";
    //            case TipoElementoGnoss.NivelPrioridadProyecto:
    //                return "NivelPrioridad";
    //            case TipoElementoGnoss.Tesauro:
    //                return "Tesauro";
    //            case TipoElementoGnoss.ParametrosProyecto:
    //                return "Parametros";
    //            case TipoElementoGnoss.Tag:
    //                return "Tag";
    //            case TipoElementoGnoss.DafoProyecto:
    //                return "Dafo";
    //            case TipoElementoGnoss.FactorDafoProyecto:
    //                return "Factor";
    //            case TipoElementoGnoss.Curriculum:
    //                return "Curriculum";
    //            case TipoElementoGnoss.Evento:
    //                return "Evento";
    //            case TipoElementoGnoss.Personalizacion:
    //                return "Personalizacion";

    //            default:
    //                return "EntidadNoExportable";
    //        }
    //    }

    //    /// <summary>
    //    /// Devuelve el tipo de entidad a partir de un string
    //    /// </summary>
    //    /// <param name="pTipoEntidad">Tipo de entidad</param>
    //    /// <returns></returns>
    //    public static TipoElementoGnoss getTipoEntidad(string pTipoEntidad)
    //    {
    //        TipoElementoGnoss tipoEntidad = TipoElementoGnoss.EntidadNoExportable;
    //        switch (pTipoEntidad)
    //        {
    //            case "TipoProceso":
    //                tipoEntidad = TipoElementoGnoss.TipoProceso;
    //                break;
    //            case "TipoGrupoFuncional":
    //                tipoEntidad = TipoElementoGnoss.TipoGrupoFuncional;
    //                break;
    //            case "Competencia":
    //                tipoEntidad = TipoElementoGnoss.Competencia;
    //                break;
    //            case "EntidadGnoss":
    //                tipoEntidad = TipoElementoGnoss.EntidadGnoss;
    //                break;
    //            case "EscalaMetas":
    //                tipoEntidad = TipoElementoGnoss.EscalaMetas;
    //                break;
    //            case "Estructura":
    //                tipoEntidad = TipoElementoGnoss.Estructura;
    //                break;
    //            case "Indicador":
    //                tipoEntidad = TipoElementoGnoss.Indicador;
    //                break;
    //            case "ModoMetrica":
    //                tipoEntidad = TipoElementoGnoss.ModoMetrica;
    //                break;
    //            case "Grupo":
    //                tipoEntidad = TipoElementoGnoss.Grupo;
    //                break;
    //            case "Objetivo":
    //                tipoEntidad = TipoElementoGnoss.Objetivo;
    //                break;
    //            case "GrupoFuncional":
    //                tipoEntidad = TipoElementoGnoss.GrupoFuncional;
    //                break;
    //            case "ConjuntoObjetivos":
    //                tipoEntidad = TipoElementoGnoss.ConjuntoObjetivos;
    //                break;
    //            case "MiniLibro":
    //                tipoEntidad = TipoElementoGnoss.MiniLibro;
    //                break;
    //            case "Proceso":
    //                tipoEntidad = TipoElementoGnoss.Proceso;
    //                break;
    //            case "Proyecto":
    //                tipoEntidad = TipoElementoGnoss.Proyecto;
    //                break;
    //            case "ElementoEstructura":
    //                tipoEntidad = TipoElementoGnoss.ElementoEstructura;
    //                break;
    //            case "EntidadNoExportable":
    //                tipoEntidad = TipoElementoGnoss.EntidadNoExportable;
    //                break;
    //            case "MetaEstructura":
    //                tipoEntidad = TipoElementoGnoss.MetaEstructura;
    //                break;
    //            case "Norma":
    //                tipoEntidad = TipoElementoGnoss.Norma;
    //                break;
    //            case "Figura":
    //                tipoEntidad = TipoElementoGnoss.Figura;
    //                break;
    //            case "Forma":
    //                tipoEntidad = TipoElementoGnoss.Forma;
    //                break;
    //            case "Persona":
    //                tipoEntidad = TipoElementoGnoss.Persona;
    //                break;
    //            case "Ocupacion":
    //                tipoEntidad = TipoElementoGnoss.Ocupacion;
    //                break;
    //            case "DimensionCompetencia":
    //                tipoEntidad = TipoElementoGnoss.DimensionCompetencia;
    //                break;
    //            case "TipoDimensionCompetencia":
    //                tipoEntidad = TipoElementoGnoss.TipoDimensionCompetencia;
    //                break;
    //            case "EntidadExportable":
    //                tipoEntidad = TipoElementoGnoss.EntidadExportable;
    //                break;
    //            case "Libro":
    //                tipoEntidad = TipoElementoGnoss.Libro;
    //                break;
    //            case "Metrica":
    //                tipoEntidad = TipoElementoGnoss.Metrica;
    //                break;
    //            case "SubEstructura":
    //                tipoEntidad = TipoElementoGnoss.SubEstructura;
    //                break;
    //            case "TipoGrupo":
    //                tipoEntidad = TipoElementoGnoss.TipoGrupo;
    //                break;
    //            case "Entrada":
    //                tipoEntidad = TipoElementoGnoss.Entrada;
    //                break;
    //            case "Salida":
    //                tipoEntidad = TipoElementoGnoss.Salida;
    //                break;
    //            case "Mecanismo":
    //                tipoEntidad = TipoElementoGnoss.Mecanismo;
    //                break;
    //            case "Control":
    //                tipoEntidad = TipoElementoGnoss.Control;
    //                break;
    //            case "ElementoNivelModo":
    //                tipoEntidad = TipoElementoGnoss.ElementoNivelModo;
    //                break;
    //            case "Organizacion":
    //                tipoEntidad = TipoElementoGnoss.Organizacion;
    //                break;
    //            case "Perspectiva":
    //                tipoEntidad = TipoElementoGnoss.Perspectiva;
    //                break;
    //            case "LibroSinClasificar":
    //                tipoEntidad = TipoElementoGnoss.Libro;
    //                break;
    //            case "ElementoModo":
    //                tipoEntidad = TipoElementoGnoss.ElementoNivelModo;
    //                break;
    //            case "CategoriaDocumentacion":
    //                tipoEntidad = TipoElementoGnoss.CategoriaDocumentacion;
    //                break;
    //            case "Documento":
    //                tipoEntidad = TipoElementoGnoss.Documento;
    //                break;
    //            case "Definicion":
    //                tipoEntidad = TipoElementoGnoss.Definicion;
    //                break;
    //            case "NivelModo":
    //                tipoEntidad = TipoElementoGnoss.NivelModo;
    //                break;
    //            case "NivelLibro":
    //                tipoEntidad = TipoElementoGnoss.NivelLibro;
    //                break;
    //            case "ElementoIDEF0":
    //                tipoEntidad = TipoElementoGnoss.ElementoIDEF0;
    //                break;
    //            case "ModoLibro":
    //                tipoEntidad = TipoElementoGnoss.ModoLibro;
    //                break;
    //            case "EjeMeta":
    //                tipoEntidad = TipoElementoGnoss.EjeMeta;
    //                break;
    //            case "FilaMeta":
    //                tipoEntidad = TipoElementoGnoss.FilaMeta;
    //                break;
    //            case "CeldaMeta":
    //                tipoEntidad = TipoElementoGnoss.CeldaMeta;
    //                break;
    //            case "MetaAsignadaElementoEstructura":
    //                tipoEntidad = TipoElementoGnoss.Meta;
    //                break;
    //            case "PuestoTrabajo":
    //                tipoEntidad = TipoElementoGnoss.PuestoTrabajo;
    //                break;
    //            case "GestionProcesos":
    //                tipoEntidad = TipoElementoGnoss.GestionProcesos;
    //                break;
    //            case "GestionObjetivos":
    //                tipoEntidad = TipoElementoGnoss.GestionObjetivos;
    //                break;
    //            case "GestionGruposFuncionales":
    //                tipoEntidad = TipoElementoGnoss.GestionGruposFuncionales;
    //                break;
    //            case "CategoriaTesauro":
    //                tipoEntidad = TipoElementoGnoss.CategoriasTesauro;
    //                break;
    //            case "Wiki":
    //                tipoEntidad = TipoElementoGnoss.Wiki;
    //                break;
    //            case "Comentario":
    //                tipoEntidad = TipoElementoGnoss.Comentario;
    //                break;
    //            case "NivelPrioridad":
    //                tipoEntidad = TipoElementoGnoss.NivelPrioridadProyecto;
    //                break;
    //            case "Tesauro":
    //                tipoEntidad = TipoElementoGnoss.Tesauro;
    //                break;
    //            case "Parametros":
    //                tipoEntidad = TipoElementoGnoss.ParametrosProyecto;
    //                break;
    //            case "Tag":
    //                tipoEntidad = TipoElementoGnoss.Tag;
    //                break;
    //            case "Dafo":
    //                tipoEntidad = TipoElementoGnoss.DafoProyecto;
    //                break;
    //            case "Factor":
    //                tipoEntidad = TipoElementoGnoss.FactorDafoProyecto;
    //                break;
    //            case "Evento":
    //                tipoEntidad = TipoElementoGnoss.Evento;
    //                break;
    //            case "Curriculum":
    //                tipoEntidad = TipoElementoGnoss.Curriculum;
    //                break;
    //            case "Personalizacion":
    //                tipoEntidad = TipoElementoGnoss.Personalizacion;
    //                break;

    //            default:
    //                tipoEntidad = TipoElementoGnoss.EntidadNoExportable;
    //                break;
    //        }
    //        return tipoEntidad;
    //    }
    //    #endregion
    //}

    #endregion

    /// <summary>
    /// Tipo de entidades que hay en Gnoss.
    /// </summary>
    public class TipoElementoGnoss
    {
        /// <summary>
        /// Tipo de Indicador
        /// </summary>
        public const string Indicador = "Indicador";

        /// <summary>
        /// Tipo de Proyecto
        /// </summary>
        public const string Proyecto = "Proyecto";
        
        /// <summary>
        /// Tipo de Entidad No Exportable
        /// </summary>
        public const string EntidadNoExportable = "EntidadNoExportable";
        /// <summary>
        /// Tipo de Persona
        /// </summary>
        public const string Persona = "Persona";
        /// <summary>
        /// Tipo de Persona
        /// </summary>
        public const string FoafPerson = "http://xmlns.com/foaf/0.1/Person";

        /// <summary>
        /// Tipo de Entidad Exportable
        /// </summary>
        public const string EntidadExportable = "EntidadExportable";
        
        /// <summary>
        /// Tipo de Organizacion
        /// </summary>
        public const string Organizacion = "Organizacion";
        /// <summary>
        /// Tipo de Documento
        /// </summary>
        public const string Documento = "http://rdfs.org/sioc/ns#Item";

        /// <summary>
        /// Tipo de Debate
        /// </summary>
        public const string Debate = "http://rdfs.org/sioc/ns#Post";

        /// <summary>
        /// Tipo de Pregunta
        /// </summary>
        public const string Pregunta = "http://rdfs.org/sioc/types#Question";

        /// <summary>
        /// Tipo de DocumentoEntidad (vinculación de una entidad con un documento y una categoría de documentación)
        /// </summary>
        public const string DocumentoEntidad = "DocumentoEntidad";
        /// <summary>
        /// Tipo de Categoria Documentacion
        /// </summary>
        public const string CategoriaDocumentacion = "CategoriaDocumentacion";
       
        /// <summary>
        /// Tipo de Puesto Trabajo
        /// </summary>
        public const string PuestoTrabajo = "PuestoTrabajo";

        /// <summary>
        /// Tipo de Categorias Tesauro de Skos
        /// </summary>
        public const string CategoriasTesauroSkos = "http://www.w3.org/2004/02/skos/core#Concept";

        /// <summary>
        /// Tipo de Categorias Tesauro
        /// </summary>
        public const string CategoriasTesauro = "CategoriaTesauro";

        /// <summary>
        /// Tipo de Wiki
        /// </summary>
        public const string Wiki = "Wiki";
        /// <summary>
        /// Tipo de Comentario
        /// </summary>
        public const string Comentario = "Comentario";

        /// <summary>
        /// Tipo de Comentario de la ontología SIOC
        /// </summary>
        public const string ComentarioSioc = "http://rdfs.org/sioc/types#Comment";

        /// <summary>
        /// Tipo de Tag de la ontología SIOC
        /// </summary>
        public const string TagSioc = "http://rdfs.org/sioc/types#Tag";

        /// <summary>
        /// Tipo de Nivel Prioridad Proyecto
        /// </summary>
        public const string NivelPrioridadProyecto = "NivelPrioridad";
        /// <summary>
        /// Tipo de Tesauro
        /// </summary>
        public const string Tesauro = "http://www.w3.org/2004/02/skos/core#ConceptScheme";
        /// <summary>
        /// Tipo de Parametros Proyecto
        /// </summary>
        public const string ParametrosProyecto = "Parametros";
        /// <summary>
        /// Tipo de Tag
        /// </summary>
        public const string Tag = "Tag";
       
        /// <summary>
        /// Tipo de Curriculum
        /// </summary>
        public const string Curriculum = "Cv";
        /// <summary>
        /// Tipo de Evento
        /// </summary>
        public const string Evento = "Evento";
        /// <summary>
        /// Tipo de Personalizacion
        /// </summary>
        public const string Personalizacion = "Personalizacion";
        /// <summary>
        /// Tipo de datos de trabajo de persona libre
        /// </summary>
        public const string DatosPersonaLibre = "DatosTrabajoPersonaLibre";
        /// <summary>
        /// Tipo de datos del vínculo entre persona y organización
        /// </summary>
        public const string PersonaVinculoOrganizacion = "PersonaVinculoOrganizacion";
        /// <summary>
        /// Tipo de datos de perfil de una persona
        /// </summary>
        public const string PerfilPersona = "PerfilPersona";
        /// <summary>
        /// Tipo de datos de perfil de una persona
        /// </summary>
        public const string PerfilPersonaFoaf = "http://xmlns.com/foaf/0.1/Person";
        /// <summary>
        /// Perfil de una organización
        /// </summary>
        public const string PerfilOrganizacion = "PerfilOrganizacion";
        /// <summary>
        /// Perfil de una organización
        /// </summary>
        public const string PerfilOrganizacionFoaf = "http://xmlns.com/foaf/0.1/Group";
        /// <summary>
        /// Perfil de una organización
        /// </summary>
        public const string PerfilPersonaOrg = "PerfilPersonaOrg";
        /// <summary>
        /// Comunidad
        /// </summary>
        public const string Comunidad = "Comunidad";
        /// <summary>
        /// Comunidad SIOC.
        /// </summary>
        public const string ComunidadSioc = "http://rdfs.org/sioc/ns#Space";
        /// <summary>
        /// CV de una persona
        /// </summary>
        public const string CVPersona = "PersonCv";
        /// <summary>
        /// CV de una organización
        /// </summary>
        public const string CVOrganizacion = "OrganizationCv";
        /// <summary>
        /// CV de una organización
        /// </summary>
        public const string CVSemantico = "SemanticCv";
        /// <summary>
        /// Lista de resultados de una búsqueda
        /// </summary>
        public const string ListaResultados = "ResultSet";
        /// <summary>
        /// Filtro de una búsqueda
        /// </summary>
        public const string Filtro = "Filtro";

        /// <summary>
        /// Colección ordenada
        /// </summary>
        public const string OrderedCollection = "skos:OrderedCollection";

        /// <summary>
        /// Lista de elementos
        /// </summary>
        public const string List = "rdf:List";

        /// <summary>
        /// Cuenta de usuario
        /// </summary>
        public const string USER_ACCOUNT = "http://rdfs.org/sioc/ns#UserAccount";

        /// <summary>
        /// Grupo de usuarios
        /// </summary>
        public const string USER_GROUP = "http://rdfs.org/sioc/ns#Usergroup";

    }

    /// <summary>
    /// Representa una entidad GNOSS.
    /// </summary>
    public class ElementoOntologiaGnoss : Es.Riam.Semantica.OWL.ElementoOntologia
    {
        #region Miembros

        //#region privados
        ///// <summary>
        ///// Tipo de la entidad
        ///// </summary>
        //private TipoElementoGnoss mTipoEntidad;

        ///// <summary>
        ///// Propiedades que posee la entidad.
        ///// </summary>
        //private List<Propiedad> mPropiedades;

        ///// <summary>
        ///// Restricciones sobre las propiedades.
        ///// </summary>
        //private List<Restriccion> mRestricciones;

        ///// <summary>
        ///// Superclase de la entidad
        ///// </summary>
        //private TipoElementoGnoss mSuperclase;

        ///// <summary>
        ///// Subclases de la entidad.
        ///// </summary>
        //private List<TipoElementoGnoss> mSubclases;

        ///// <summary>
        ///// Relaciones que especializan la entidad.
        ///// </summary>
        //private List<string> mEspecializaciones;

        ///// <summary>
        ///// Relación que generaliza la entidad.
        ///// </summary>
        //private string mGeneralizacion;

        ///// <summary>
        ///// Lista de entidades relacionadas con ésta entidad mediante propiedades.
        ///// </summary>
        //private List<ElementoOntologia> mEntidadesRelacionadas;

        ///// <summary>
        ///// ID de la entidad.
        ///// </summary>
        //private string mID;

        ///// <summary>
        ///// Almacena la descripción o el nombre del elemento que facilita la identificación del mismo por el usuario
        ///// </summary>
        //private string mDescripcion;

        ///// <summary>
        ///// Verdadero si la entidad es valida y se debe exportar/importar.
        ///// </summary>
        //private bool mEntidadValida;

        ///// <summary>
        ///// Elemento gnoss que representa la entidad
        ///// </summary>
        //private IElementoGnoss mElemento;

        ///// <summary>
        ///// Padre de la entidad.
        ///// </summary>
        //private IElementoGnoss mPadre;

        ///// <summary>
        ///// Verdad si la entidad puede tener un padre.
        ///// </summary>
        //private bool mPermitePadre;

        ///// <summary>
        ///// Verdad si el elemento se debe imprimir
        ///// </summary>
        //private bool mSeDebeImprimir = true;

        ///// <summary>
        ///// Lista de hijos de impresión
        ///// </summary>
        //private List<IImpresion> mHijosImpresion = new List<IImpresion>();

        ///// <summary>
        ///// Obtiene o establece el valor que indica si el elemento ya se imprimió en el documento
        ///// </summary>
        //private bool mElementoImpreso;

        ///// <summary>
        ///// Almacena la lista de propiedades imprimibles
        ///// </summary>
        //private List<Propiedad> mListaPropiedadesImprimibles = new List<Propiedad>();

        ///// <summary>
        ///// Verdad si se han obtenido las entidades relacionadas con esta entidad
        ///// </summary>
        //private bool mEstaCompleta = false;

        //#endregion

        #endregion

        #region Constructores

        /// <summary>
        /// Construye una entidad a partir de otra.
        /// </summary>
        /// <param name="pEntidad">entidad a partir de la cual se creará la nueva entidad.</param>
        public ElementoOntologiaGnoss(Es.Riam.Semantica.OWL.ElementoOntologia pEntidad) : base()
        {
            this.mTipoEntidad = pEntidad.TipoEntidad;

            foreach (Propiedad propiedad in pEntidad.Propiedades)
            {
                PropiedadGnoss propiedadNueva = new PropiedadGnoss(propiedad);
                Propiedades.Add(propiedadNueva);
            }

            foreach (Restriccion restriccion in pEntidad.Restricciones)
            {
                this.Restricciones.Add(restriccion);
            }

            this.Superclases = pEntidad.Superclases;

            foreach (ElementoOntologia entidad in pEntidad.EntidadesRelacionadas)
            {
                this.EntidadesRelacionadas.Add(new ElementoOntologiaGnoss(entidad));
            }

            this.ID = pEntidad.ID;
            this.Descripcion = pEntidad.Descripcion;

            this.Subclases = pEntidad.Subclases;
            this.Especializaciones = pEntidad.Especializaciones;
            this.Generalizacion = pEntidad.Generalizacion;
            this.EntidadValida = pEntidad.EntidadValida;
            this.Padre = pEntidad.Padre;
            this.PermitePadre = pEntidad.PermitePadre;
            this.UrlOntologia = pEntidad.UrlOntologia;
            this.NamespaceOntologia = pEntidad.NamespaceOntologia;
            this.Ontologia = pEntidad.Ontologia;
        }

        /// <summary>
        /// Crea una entidad a partir de un tipo de entidad.
        /// </summary>
        /// <param name="pTipoEntidad">tipo de entidad.</param>
        /// <param name="pOntologia">Ontología</param>
        public ElementoOntologiaGnoss(string pTipoEntidad, Ontologia pOntologia)
            : base(pTipoEntidad, GestionOWLGnoss.ObtenerUrlEntidad(pTipoEntidad), "gnoss", pOntologia)
        {
        }

        /// <summary>
        /// Crea una entidad a partir de los parametros de entrada
        /// </summary>
        /// <param name="pTipoEntidad">tipo de la entidad</param>
        /// <param name="pPropiedades">propiedades que adquirirá la entidad.</param>
        /// <param name="pRestricciones">restricciones que posee la entidad</param>
        /// <param name="pSuperclases">superclases a las que hace referencia la entidad.</param>
        /// <param name="pID">ID de la entidad.</param>
        /// <param name="pEntidadesRelacionadas">Entidades relacionadas con la entidad a crear.</param>
        /// <param name="pOntologia">Ontología</param>
        public ElementoOntologiaGnoss(string pTipoEntidad, List<Propiedad> pPropiedades, List<Restriccion> pRestricciones, List<string> pSuperclases, string pID, List<Es.Riam.Semantica.OWL.ElementoOntologia> pEntidadesRelacionadas, Ontologia pOntologia)
            : base(pTipoEntidad, pPropiedades, pRestricciones, pSuperclases, pID, pEntidadesRelacionadas, GestionOWLGnoss.ObtenerUrlEntidad(pTipoEntidad), "gnoss", pOntologia)
        {
        }

        /// <summary>
        /// Crea una entidad a partir de un tipo de entidad.
        /// </summary>
        /// <param name="pTipoEntidad">tipo de entidad.</param>
        /// <param name="pNamespaceOntologia">Namespace de la ontología</param>
        /// <param name="pUrlOntologia">Url de la ontología</param>
        /// <param name="pOntologia">Ontología</param>
        public ElementoOntologiaGnoss(string pTipoEntidad, string pUrlOntologia, string pNamespaceOntologia, Ontologia pOntologia)
            : base(pTipoEntidad, pUrlOntologia, pNamespaceOntologia, pOntologia)
        {
        }

        /// <summary>
        /// Construye una entidad a partir de otra.
        /// </summary>
        /// <param name="pEntidad">entidad a partir de la cual se creará la nueva entidad.</param>
        public ElementoOntologiaGnoss(ElementoOntologiaGnoss pEntidad) : base(pEntidad)
        {
            base.mEntidadesRelacionadas = new List<Es.Riam.Semantica.OWL.ElementoOntologia>();
            foreach (Es.Riam.Semantica.OWL.ElementoOntologia entidad in pEntidad.EntidadesRelacionadas)
            {
                base.mEntidadesRelacionadas.Add(new ElementoOntologiaGnoss(entidad));
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el tipo de la entidad
        /// </summary>
        public override string TipoEntidad
        {
            set
            {
                this.mTipoEntidad = value;
            }
            get
            {
                return base.TipoEntidad;
            }
        }

        /// <summary>
        /// Obtiene o establece el tipo de la entidad con namespace
        /// </summary>
        public override string TipoEntidadConNamespace
        {
            set
            {
                this.mTipoEntidad = value;
            }
            get
            {
                if (!mTipoEntidad.Contains("#") && !mTipoEntidad.Contains("/"))
                {
                    return "gnoss:" + this.mTipoEntidad;
                }
                else //Ya tiene namespace o tiene URL completa.
                {
                    return base.TipoEntidadConNamespace;
                }
            }
        }

        /// <summary>
        /// Obtiene o establece el tipo de la entidad
        /// </summary>
        public override string TipoEntidadCrearRdf
        {
            get
            {
                return TipoEntidadConNamespace;
            }
        }

        /// <summary>
        /// Obtiene o establece las superclases de la entidad
        /// </summary>
        public override List<string> Superclases
        {
            set
            {
                base.Superclases = value;
            }
            get
            {
                return base.Superclases;
            }
        }

        /// <summary>
        /// Devuelve la Uri del elemento Gnoss.
        /// </summary>
        public override string Uri
        {
            get
            {
                //return GestionOWLGnoss.ObtenerUrlEntidad(TipoEntidad, this);
                return ID;
            }
            set
            {
                this.mID = value;
            }
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Añade una propiedad a la lista de propiedades
        /// </summary>
        /// <param name="pPropiedad">Propiedad que se va a añadir</param>
        public override void AddPropiedad(Propiedad pPropiedad)
        {
            Propiedades.Add(new PropiedadGnoss(pPropiedad));
        }

        /// <summary>
        /// Da valor a la propiedad ID de la entidad a partir de un ID relativo.
        /// </summary>
        /// <param name="pIDRelativo">Identificador relativo</param>
        public override void EstablecerID(string pIDRelativo)
        {
            this.ID = GestionOWLGnoss.ObtenerUrlEntidad(TipoEntidad, this.Elemento, pIDRelativo);
        }

        #endregion
    }
}
