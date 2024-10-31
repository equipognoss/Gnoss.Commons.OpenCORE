using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.ExportarImportar.ElementosOntologia;
using Es.Riam.Interfaces;
using Es.Riam.Semantica.OWL;
using Es.Riam.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Es.Riam.Gnoss.ExportarImportar
{
    #region Enumeraciones

    /// <summary>
    /// Enumeración con los estados posibles de finalización de los procesos de importación o exportación
    /// </summary>
    public enum EstadoFinalizacion
    {
        /// <summary>
        /// El proceso finalizó con éxito
        /// </summary>
        FinalizadoConExito,
        /// <summary>
        /// El proceso finalizó con errors
        /// </summary>
        FinalizadoConErrores,
        /// <summary>
        /// El proceso no pudo finalizar
        /// </summary>
        NoFinalizo
    }

    #endregion

    /// <summary>
    /// Clase de utilidades para importar y exportar
    /// </summary>
    public class UtilImportarExportar
    {
        #region Constantes

        #region Propiedades OWL

        #region Propiedades de Entidad

        /// <summary>
        /// Propiedad Documentación de Entidad
        /// </summary>
        public const string PROPIEDAD_DOCUMENTACION_ENTIDAD = "DocumentacionEntidad";

        /// <summary>
        /// Propiedad Documento de una entidad vinculado a una categoría de documentación
        /// </summary>
        public const string PROPIEDAD_DOCUMENTO_VINCULADO = "DocumentoVinculado";

        /// <summary>
        /// Propiedad Categoría de documentación
        /// </summary>
        public const string PROPIEDAD_CATEGORIA_DOCUMENTACION = "CategoriaDocumentacionVinculada";

        /// <summary>
        /// Editores
        /// </summary>
        public const string PROPIEDAD_EDITORES = "Editores";

        #endregion

        #region Propiedades de Estructura

        /// <summary>
        /// Propiedad Elemento de estructura secundario
        /// </summary>
        public const string PROPIEDAD_ELEMENTO_ESTRUCTURA_SECUNDARIO = "ElementoEstructuraSecundario";

        /// <summary>
        /// Propiedad Tipo de metaestructura
        /// </summary>
        public const string PROPIEDAD_TIPO_METAESTRUCTURA = "TipoMetaEstructura";

        /// <summary>
        /// Propiedad Tipo de forma
        /// </summary>
        public const string PROPIEDAD_TIPO_FORMA = "TipoForma";

        /// <summary>
        /// Propiedad de participantes de una figura
        /// </summary>
        public const string PROPIEDAD_PARTICIPANTES_FIGURA = "Personas";

        /// <summary>
        /// Propiedad de participantes de una figura
        /// </summary>
        public const string PROPIEDAD_FIGURA = "Figuras";

        /// <summary>
        /// Propiedad de ocupaciones de una figura
        /// </summary>
        public const string PROPIEDAD_OCUPACIONES = "Ocupaciones";

        /// <summary>
        /// Propiedad de una figura que indica si es la responsable de la forma superior
        /// </summary>
        public const string PROPIEDAD_ES_RESPONSABLE_FORMA_SUPERIOR = "EsResponsableFormaSuperior";

        /// <summary>
        /// Propiedad de dedicación de una ocupación en una figura
        /// </summary>
        public const string PROPIEDAD_DEDICACION = "Dedicacion";

        /// <summary>
        /// Propiedad de organización de una figura
        /// </summary>
        public const string PROPIEDAD_ORGANIZACION = "OrganizacionID";

        /// <summary>
        /// Propiedad puesto de trabajo de un elemento de estructura
        /// </summary>
        public const string PROPIEDAD_PUESTO_TRABAJO = "PuestoTrabajoElementoEstructura";

        /// <summary>
        /// Propiedad de una figura que indica los procesos en los que es participante
        /// </summary>
        public const string PROPIEDAD_PROCESOS_PARTICIPANTE = "ProcesosParticipante";

        /// <summary>
        /// Propiedad de competencias asociadas a un elemento de estrucutra
        /// </summary>
        public const string PROPIEDAD_COMPETENCIAS_ASOCIADAS = "CompetenciasAsociadas";

        /// <summary>
        /// Propiedad de datos de la persona libre (cuando no está vinculada a ninguna organización)
        /// </summary>
        public const string PROPIEDAD_DATOS_TRABAJO_PERSONA_LIBRE = "DatosPersonaLibre";

        /// <summary>
        /// Propiedad de datos de la vinculación de la persona a una organización
        /// </summary>
        public const string PROPIEDAD_PERSONA_VINCULO_ORGANIZACION = "OrganizacionVinculada";

        /// <summary>
        /// Propiedad de la organización vinculada a una persona
        /// </summary>
        public const string PROPIEDAD_ORGANIZACION_DE_PERSONA = "OrganizacionDePersona";

        /// <summary>
        /// Propiedad de personal de la organizacion
        /// </summary>
        public const string PROPIEDAD_PERSONAL_ORGANIZACION = "PersonalOrganizacion";

        #endregion

        #region Propiedades de Documento

        ///// <summary>
        ///// Autor del documento
        ///// </summary>
        //public const string PROPIEDAD_DC_AUTOR = "http://purl.org/dc/terms/creator";

        /// <summary>
        /// Descripción del documento
        /// </summary>
        public const string PROPIEDAD_SIOC_ITEM_DESCRIPCION = "http://rdfs.org/sioc/ns#content";

        /// <summary>
        /// Título del documento
        /// </summary>
        public const string PROPIEDAD_DC_TITULO = "http://purl.org/dc/terms/title";

        /// <summary>
        /// Tags del documento
        /// </summary>
        public const string PROPIEDAD_SIOC_TOPIC = "http://rdfs.org/sioc/ns#topic";

        /// <summary>
        /// Comentarios del documento
        /// </summary>
        public const string PROPIEDAD_SIOC_COMENTARIOS = "http://rdfs.org/sioc/ns#has_discussion";

        /// <summary>
        /// Tipo del documento
        /// </summary>
        public const string PROPIEDAD_DC_TIPO = "http://purl.org/dc/terms/type";

        /// <summary>
        /// Enlace
        /// </summary>
        public const string PROPIEDAD_ENLACE = "http://purl.org/dc/terms/source";

        /// <summary>
        /// Creador
        /// </summary>
        public const string PROPIEDAD_SIOC_CREADOR = "http://rdfs.org/sioc/ns#has_creator";

        /// <summary>
        /// Fecha de publicación
        /// </summary>
        public const string PROPIEDAD_DC_FECHA = "http://purl.org/dc/terms/created";

        /// <summary>
        /// Fecha de publicación
        /// </summary>
        public const string PROPIEDAD_DC_AUTOR = "http://purl.org/dc/elements/1.1/creator";

        /// <summary>
        /// Propiedad Comunidades
        /// </summary>
        public const string PROPIEDAD_SIOC_COMUNIDADES = "http://rdfs.org/sioc/ns#has_space";

        /// <summary>
        /// Propiedad Reply_OF (Respuesta para comentarios).
        /// </summary>
        public const string PROPIEDAD_SIOC_REPLY_OF = "http://rdfs.org/sioc/ns#reply_of";

        /// <summary>
        /// Propiedad Comunidades
        /// </summary>
        public const string PROPIEDAD_DOC_SEM = "has_docSem";

        /// <summary>
        /// Recurso vincunculado.
        /// </summary>
        public const string PROPIEDAD_SIOC_ATTACHMENT = "http://rdfs.org/sioc/ns#attachment";

        /// <summary>
        /// Recurso vincunculado.
        /// </summary>
        public const string PROPIEDAD_CC_LICENSE = "http://creativecommons.org/ns#license";

        /// <summary>
        /// Autor de un recurso.
        /// </summary>
        public const string PROPIEDAD_FOAF_MAKER = "http://xmlns.com/foaf/0.1/maker";

        /// <summary>
        /// Recursos relacionado.
        /// </summary>
        public const string PROPIEDAD_SIOC_RELATED_TO = "http://rdfs.org/sioc/ns#related_to";

        #endregion

        #region Propiedades de Definición

        /// <summary>
        /// Fuente
        /// </summary>
        public const string PROPIEDAD_FUENTE = "Fuente";

        /// <summary>
        /// Definiciones de palabra
        /// </summary>
        public const string PROPIEDAD_DEFINICIONES_PALABRA = "Definiciones";

        #endregion

        #region Propiedades de Proyecto

        /// <summary>
        /// Parametros
        /// </summary>
        public const string PROPIEDAD_PARAMETROS_PROYECTO = "ParametrosProyecto";

        #endregion

        #region Propiedades de Tesauro

        /// <summary>
        /// Tesauro de proyecto
        /// </summary>
        public const string PROPIEDAD_TESAURO = "TesauroProyecto";

        /// <summary>
        /// Categorías de Tesauro
        /// </summary>
        public const string PROPIEDAD_CATEGORIA_TESAURO_INFERIOR = "CategoriasTesauro";

        /// <summary>
        /// Categorías de Tesauro
        /// </summary>
        public const string PROPIEDAD_CATEGORIA_PRIMER_NIVEL = "http://www.w3.org/2004/02/skos/core#hasTopConcept";

        /// <summary>
        /// Categorías de Tesauro
        /// </summary>
        public const string PROPIEDAD_CATEGORIA_INFERIOR_DE_OTRA = "http://www.w3.org/2004/02/skos/core#narrower";

        /// <summary>
        /// Categorías de Tesauro
        /// </summary>
        public const string PROPIEDAD_CATEGORIA_NIETA_DE_OTRA = "http://www.w3.org/2004/02/skos/core#narrowerTransitive";

        /// <summary>
        /// Categorías de Tesauro
        /// </summary>
        public const string PROPIEDAD_CATEGORIA_PADRE = "http://www.w3.org/2004/02/skos/core#broader";

        /// <summary>
        /// Categorías de Tesauro
        /// </summary>
        public const string PROPIEDAD_CATEGORIA_ABUELA = "http://www.w3.org/2004/02/skos/core#broaderTransitive";

        /// <summary>
        /// Categorías de Tesauro
        /// </summary>
        public const string PROPIEDAD_CATEGORIAS_HERMANAS = "http://www.w3.org/2004/02/skos/core#semanticRelation";

        /// <summary>
        /// Categorías de Tesauro
        /// </summary>
        public const string PROPIEDAD_PREFLABEL = "http://www.w3.org/2004/02/skos/core#prefLabel";

        /// <summary>
        /// Título de la comunidad.
        /// </summary>
        public const string PROPIEDAD_SIOC_TITULO = "http://rdfs.org/sioc/ns#title";

        #endregion

        #region Propiedades Parámetros de proyecto

        /// <summary>
        /// Mensaje Bienvenida
        /// </summary>
        public const string PROPIEDAD_MENSAJE_BIENVENIDA = "MensajeBienvenida";

        /// <summary>
        /// Aviso Legal
        /// </summary>
        public const string PROPIEDAD_AVISO_LEGAL = "AvisoLegal";

        /// <summary>
        /// Logotipo Proyecto
        /// </summary>
        public const string PROPIEDAD_lOGOTIPO_PROYECTO = "LogoProyecto";

        /// <summary>
        /// Wiki Disponible
        /// </summary>
        public const string PROPIEDAD_WIKI_DISPONIBLE = "WikiDisponible";

        /// <summary>
        /// Dafo Disponible
        /// </summary>
        public const string PROPIEDAD_DAFO_DISPONIBLE = "DafoDisponible";

        /// <summary>
        /// Base de Recursos Disponible
        /// </summary>
        public const string PROPIEDAD_BASE_RECURSOS_DISPONIBLE = "BaseRecursosDisponible";

        /// <summary>
        /// Invitaciones Disponibles
        /// </summary>
        public const string PROPIEDAD_INVITACIONES_DISPONIBLE = "InvitacionesDisponibles";

        /// <summary>
        /// Determinacion Propietarios
        /// </summary>
        public const string PROPIEDAD_DET_PROPIETARIOS = "UmbralDetPropietariosç";

        /// <summary>
        /// Determinacion Criticas
        /// </summary>
        public const string PROPIEDAD_DET_CRITICAS = "UmbralDeterminacionCriticas";

        /// <summary>
        /// Determinacion Estrategicas
        /// </summary>
        public const string PROPIEDAD_DET_ESTRATEGICAS = "UmbralDetEstrategicas";

        /// <summary>
        /// Determinacion Estrategicas por Persona
        /// </summary>
        public const string PROPIEDAD_DET_ESTRATEGICAS_PERSONA = "UmbralDetEstrategicasPersonas";

        #endregion

        #region Propiedades compartidas

        /// <summary>
        /// Tipo
        /// </summary>
        public const string PROPIEDAD_TIPO = "Tipo";

        /// <summary>
        /// Fecha
        /// </summary>
        public const string PROPIEDAD_FECHA = "Fecha";

        /// <summary>
        /// Fecha Inicio
        /// </summary>
        public const string PROPIEDAD_FECHA_INICIO = "FechaInicio";

        /// <summary>
        /// Fecha Inicio
        /// </summary>
        public const string PROPIEDAD_FECHA_FIN = "FechaFin";

        /// <summary>
        /// Tipo
        /// </summary>
        public const string PROPIEDAD_TAGS = "Tags";

        /// <summary>
        /// Descripción
        /// </summary>
        public const string PROPIEDAD_DESCRIPCION = "Descripcion";

        /// <summary>
        /// Nombre
        /// </summary>
        public const string PROPIEDAD_NOMBRE = "Nombre";

        /// <summary>
        /// Título
        /// </summary>
        public const string PROPIEDAD_TITULO = "Titulo";

        /// <summary>
        /// Padre
        /// </summary>
        public const string PROPIEDAD_PADRE = "Padre";

        #endregion

        #region Tipos de Metaestructura

        /// <summary>
        /// Metaestructura orgánica
        /// </summary>
        public const string METAESTRUCTURA_ORGANICA = "orgánica";

        /// <summary>
        /// Metaestructura secundaria
        /// </summary>
        public const string METAESTRUCTURA_SECUNDARIA = "secundaria";

        #endregion

        #region Propiedades de Currículum

        /// <summary>
        /// Propiedad evento
        /// </summary>
        public const string PROPIEDAD_EVENTOS = "Eventos";

        /// <summary>
        /// Propiedad curriculum personalizado
        /// </summary>
        public const string PROPIEDAD_PERSONALIZACIOINES = "Personalizaciones";

        /// <summary>
        /// Propiedad autor publicacion
        /// </summary>
        public const string PROPIEDAD_AUTOR_PUBLICACION = "AutorPublicacion";

        /// <summary>
        /// Propiedad cvLibro
        /// </summary>
        public const string PROPIEDAD_CV_LIBRO = "CVLibro";

        /// <summary>
        /// Propiedad cvLibro
        /// </summary>
        public const string PROPIEDAD_PUESTO_EXP_LAB = "PuestoExpLab";

        /// <summary>
        /// Propiedad cvLibro
        /// </summary>
        public const string PROPIEDAD_DOCENCIA = "Docencia";

        /// <summary>
        /// Propiedad Comunidades
        /// </summary>
        public const string PROPIEDAD_COMUNIDADES = "Comunidades";

        /// <summary>
        /// Propiedad CVPerfilPersona
        /// </summary>
        public const string PROPIEDAD_CVPERFILPERSONA = "has_cv";

        /// <summary>
        /// Propiedad PersonasOrganizacion
        /// </summary>
        public const string PROPIEDAD_PERSONASORGANIZACION = "PersonasOrganizacion";

        /// <summary>
        /// Propiedad 
        /// </summary>
        public const string PROPIEDAD_CVPERFILORGANIZACION = "has_history";

        /// <summary>
        /// Propiedad CVPerfilPersona
        /// </summary>
        public const string PROPIEDAD_CVSEMANTICO = "CV_HR_XML";

        /// <summary>
        /// Propiedad CVPerfilPersona
        /// </summary>
        public const string PROPIEDAD_CVSEMANTICO_PERSONA = "person_sem_cv";

        /// <summary>
        /// Propiedad de aficiones del CV de una persona
        /// </summary>
        public const string PROPIEDAD_CVPERSONA_AFICIONES = "has_hobbies";

        /// <summary>
        /// Propiedad de ahora soy del CV de una persona
        /// </summary>
        public const string PROPIEDAD_CVPERSONA_AHORASOY = "current_job";

        /// <summary>
        /// Propiedad de biografía del CV de una persona
        /// </summary>
        public const string PROPIEDAD_CVPERSONA_BIOGRAFIA = "biography";

        /// <summary>
        /// Propiedad de Me Interesa del CV de una persona
        /// </summary>
        public const string PROPIEDAD_CVPERSONA_MEINTERESA = "has_interest";

        /// <summary>
        /// Propiedad de Objetivos Profesionales del CV de una persona
        /// </summary>
        public const string PROPIEDAD_CVPERSONA_OBJETIVOS_PROFESIONALES = "has_career_goals";

        /// <summary>
        /// Propiedad de Puedo Ofrecer del CV de una persona
        /// </summary>
        public const string PROPIEDAD_CVPERSONA_PUEDO_OFRECER = "offers";

        /// <summary>
        /// Propiedad de Trayectoria del CV de una organización
        /// </summary>
        public const string PROPIEDAD_CVORG_TRAYECTORIA = "organization_history";

        /// <summary>
        /// Propiedad de Nos Dedicamos del CV de una organización
        /// </summary>
        public const string PROPIEDAD_CVORG_NOS_DEDICAMOS = "is_dedicated";

        #endregion

        #region Tipos de Filtro

        /// <summary>
        /// Tipo de filtro de categoría.
        /// </summary>
        public const string TIPO_FILTRO_CATEGORIA = "Categoría";

        /// <summary>
        /// Tipo de filtro de comunidad.
        /// </summary>
        public const string TIPO_FILTRO_COMUNIDAD = "Comunidad";

        /// <summary>
        /// Tipo de filtro de fecha desde.
        /// </summary>
        public const string TIPO_FILTRO_FECHA_DESDE = "Fecha desde";

        /// <summary>
        /// Tipo de filtro de fecha hasta.
        /// </summary>
        public const string TIPO_FILTRO_FECHA_HASTA = "Fecha hasta";

        /// <summary>
        /// Tipo de filtro de los elementos sobre los que se realiza la búsqueda.
        /// </summary>
        public const string TIPO_FILTRO_BUSCAR_CONTENIDO_EN = "Buscar contenido en";

        /// <summary>
        /// Tipo de filtro de tipo de documento.
        /// </summary>
        public const string TIPO_FILTRO_TIPO_DOC = "Tipo de documento";

        /// <summary>
        /// Tipo de filtro de pais.
        /// </summary>
        public const string TIPO_FILTRO_PAIS = "Pais";

        /// <summary>
        /// Tipo de filtro de provincia.
        /// </summary>
        public const string TIPO_FILTRO_PROVINCIA = "Provincia";

        /// <summary>
        /// Tipo de filtro de provincia.
        /// </summary>
        public const string TIPO_FILTRO_AUTOR = "Autor";

        /// <summary>
        /// Tipo de filtro de provincia.
        /// </summary>
        public const string TIPO_FILTRO_NIVEL_CERTIFICACION = "Nivel de certificación";

        /// <summary>
        /// Número de elementos por página del resultados de una búsqueda.
        /// </summary>
        public const string TIPO_FILTRO_EXTENSION = "Extension";

        /// <summary>
        /// Número de elementos por página del resultados de una búsqueda.
        /// </summary>
        public const string TIPO_FILTRO_PUBLICADO_POR = "Publicado por";

        /// <summary>
        /// Número de elementos por página del resultados de una búsqueda.
        /// </summary>
        public const string TIPO_FILTRO_ESTADO_DOC = "Estado documento";

        #endregion

        #region ListaResultados

        /// <summary>
        /// Resultados de una búsqueda.
        /// </summary>
        public const string PROPIEDAD_RESULTADOS = "Results";

        /// <summary>
        /// Resultados de una búsqueda.
        /// </summary>
        public const string PROPIEDAD_FILTROS = "Filter";

        /// <summary>
        /// Número de la página actual de los resultados de una búsqueda.
        /// </summary>
        public const string PROPIEDAD_NUM_PAGINA = "Page";

        /// <summary>
        /// Número de elementos por página del resultados de una búsqueda.
        /// </summary>
        public const string PROPIEDAD_NUM_RESULT_POR_PAG = "ItemsPerPage";

        #endregion

        #region Propiedades de personas Foaf

        /// <summary>
        /// Nombre foaf.
        /// </summary>
        public const string PROPIEDAD_FOAF_FIRSTNAME = "http://xmlns.com/foaf/0.1/firstName";

        /// <summary>
        /// Apellidos foaf.
        /// </summary>
        public const string PROPIEDAD_FOAF_FAMILYNAME = "http://xmlns.com/foaf/0.1/familyName";

        /// <summary>
        /// Sexo foaf.
        /// </summary>
        public const string PROPIEDAD_FOAF_GENDER = "http://xmlns.com/foaf/0.1/gender";

        #region Propiedades de listas

        /// <summary>
        /// Propiedad First de una lista, que representa al primer elemento
        /// </summary>
        public const string PROPIEDAD_FIRST = "http://www.w3.org/1999/02/22-rdf-syntax-ns#first";

        /// <summary>
        /// Propiedad Rest de una lista, que representa el resto de elementos de una lista
        /// </summary>
        public const string PROPIEDAD_REST = "http://www.w3.org/1999/02/22-rdf-syntax-ns#rest";

        /// <summary>
        /// Propiedad Miembro de lista de la clase OrderedCollection
        /// </summary>
        public const string PROPIEDAD_MEMBER_LIST = "http://www.w3.org/2004/02/skos/core#memberList";

        #endregion

        /// <summary>
        /// Cuenta de usuario
        /// </summary>
        public const string PROPIEDAD_CUENTA_USUARIO = "http://xmlns.com/foaf/0.1/account";

        /// <summary>
        /// Grupo de usuarios
        /// </summary>
        public const string PROPIEDAD_GRUPO_DE_USUARIO = "http://rdfs.org/sioc/ns#member_of";

        /// <summary>
        /// Comunidad de grupo de usuarios
        /// </summary>
        public const string PROPIEDAD_COMUNIDAD_DE_GRUPO_USUARIOS = "http://rdfs.org/sioc/ns#usergroup_of";

        #endregion

        #region Propiedades de FoafOrganization

        /// <summary>
        /// name foaf.
        /// </summary>
        public const string PROPIEDAD_FOAF_NAME = "http://xmlns.com/foaf/0.1/name";

        /// <summary>
        /// Member foaf.
        /// </summary>
        public const string PROPIEDAD_FOAF_MEMBER = "http://xmlns.com/foaf/0.1/member";

        #endregion

        #region Tipos de documentos según Dublin Core

        /// <summary>
        /// Tipo de documento interactivo (una página web, un applet, un chat...)
        /// </summary>
        public const string DC_INTERACTIVERESOURCE = "http://purl.org/dc/dcmitype/InteractiveResource";

        /// <summary>
        /// Tipo de documento de referencia a un objeto físico
        /// </summary>
        public const string DC_PHYSICALOBJECT = "http://purl.org/dc/dcmitype/PhysicalObject";

        /// <summary>
        /// Tipo de documento imagen en movimiento (presentación, animación, video...)
        /// </summary>
        public const string DC_MOVINGIMAGE = "http://purl.org/dc/dcmitype/MovingImage";

        /// <summary>
        /// Tipo de documento software (aplicacion.exe, java...)
        /// </summary>
        public const string DC_SOFTWARE = "http://purl.org/dc/dcmitype/Software";

        /// <summary>
        /// Tipo de documento texto
        /// </summary>
        public const string DC_TEXT = "http://purl.org/dc/dcmitype/Text";

        /// <summary>
        /// Tipo de documento sonido o audio
        /// </summary>
        public const string DC_SOUND = "http://purl.org/dc/dcmitype/Sound";

        /// <summary>
        /// Tipo de documento Imagen
        /// </summary>
        public const string DC_IMAGE = "http://purl.org/dc/dcmitype/Image";

        #endregion

        #endregion

        #endregion

        #region Métodos generales

        /// <summary>
        /// Pasa una fecha a su representación según el estandar ISO-8601
        /// </summary>
        /// <returns></returns>
        public static string PasarFechaEnFormatoEstandar(DateTime pFecha)
        {
            DateTime fechaNueva = new DateTime(pFecha.Ticks, DateTimeKind.Local);
            return fechaNueva.ToString("yyyy-MM-ddTHH:mm:ss%K");
        }

        #region Métodos de propiedades

        /// <summary>
        /// Devuelve la propiedad de nombre pNombrePropiedad
        /// </summary>
        /// <param name="pNombrePropiedad">Nombre de la propiedad solicitada</param>
        /// <param name="pListaPropiedades">Lista de propiedades en la que se encuentra.</param>
        /// <returns></returns>
        public static Propiedad ObtenerPropiedadDeNombre(string pNombrePropiedad, IList<Propiedad> pListaPropiedades)
        {
            return UtilSemantica.ObtenerPropiedadDeNombre(pNombrePropiedad, pListaPropiedades);
        }

        /// <summary>
        /// Devuelve la propiedad de nombre pNombrePropiedad
        /// </summary>
        /// <param name="pNombrePropiedad">Nombre de la propiedad solicitada</param>
        /// <param name="pOntologia">Ontologia</param>
        /// <param name="pTipoEntidad">Tipo de entidad</param>
        /// <returns></returns>
        public static string ObtenerNombreRealPropiedad(string pNombrePropiedad, Ontologia pOntologia, string pTipoEntidad)
        {
            string nombre = pNombrePropiedad;

            ElementoOntologia entidad = pOntologia.GetEntidadTipo(pTipoEntidad);

            if (entidad != null)
            {
                Propiedad propiedad = UtilImportarExportar.ObtenerPropiedadDeNombre(pNombrePropiedad, entidad.Propiedades);

                if (propiedad != null)
                    nombre = propiedad.NombreReal;
            }
            return nombre;
        }

        /// <summary>
        /// Devuelve la propiedad inversa a pPropiedad que contiene pEntidad.
        /// </summary>
        /// <param name="pPropiedad">Propiedad de la cual se busca su inversa.</param>
        /// <param name="pEntidad">Entidad que posee la propiedad inversa.</param>
        /// <returns></returns>
        public static Propiedad ObtenerPropiedadInversa(Propiedad pPropiedad, Es.Riam.Semantica.OWL.ElementoOntologia pEntidad)
        {
            foreach (Propiedad propiedad in pPropiedad.ListaPropiedadesInversas)
            {
                foreach (Propiedad propiedadInversa in pEntidad.Propiedades)
                {
                    if (propiedad.Equals(propiedadInversa))
                        return propiedadInversa;
                }
            }
            return null;
        }

        /// <summary>
        /// Devuelve el nombre real de una propiedad.
        /// </summary>
        /// <param name="pEntidad">Entidad que posee la propiedad.</param>
        /// <param name="pPropiedad">Propiedad de la que se desea obtener el nombre.</param>
        /// <param name="pPadre">Entidad padre de la entidad (puede ser null).</param>
        public static void ObtenerNombreRealPropiedad(Es.Riam.Semantica.OWL.ElementoOntologia pEntidad, Es.Riam.Semantica.OWL.ElementoOntologia pPadre, Propiedad pPropiedad)
        {
        }

        /// <summary>
        /// comprueba si la propiedad corresponde a una propiedad de inferiores
        /// </summary>
        /// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        /// <returns>TRUE si la propiedad corresponde a una propiedad inferior, FALSE en caso contrario</returns>
        public static bool ComprobarPropiedadInferior(string pNombrePropiedad)
        {
            return false;
        }

        /// <summary>
        /// Comprueba si la propiedad corresponde a una propiedad de inferiores
        /// </summary>
        /// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        /// <param name="pEntidad">Entidad padre de la propiedad</param>
        /// <returns>TRUE si la propiedad corresponde a una propiedad inferior, FALSE en caso contrario</returns>
        public static bool ComprobarPropiedadInferior(string pNombrePropiedad, Es.Riam.Semantica.OWL.ElementoOntologia pEntidad)
        {
            switch (pNombrePropiedad)
            {
                case UtilImportarExportar.PROPIEDAD_SIOC_TOPIC:
                    if (pEntidad.TipoEntidad.Equals(TipoElementoGnoss.CategoriasTesauro))
                        return true;
                    return false;
                case UtilImportarExportar.PROPIEDAD_CATEGORIA_TESAURO_INFERIOR:
                    if (pEntidad.TipoEntidad.Equals(TipoElementoGnoss.CategoriasTesauro))
                        return true;
                    return false;
                default:
                    return ComprobarPropiedadInferior(pNombrePropiedad);
            }
        }

        /// <summary>
        /// Comprueba si el valor de la propiedad es una entidad del mismo tipo que la entidad que posee la propiedad
        /// </summary>
        /// <param name="pNombrePropiedad">Nombre de la propiedad.</param>
        /// <returns>TRUE si el valor de la propiedad es una entidad del mismo tipo que la entidad que posee la propiedad, FALSE en caso contrario</returns>
        public static bool ComprobarPropiedadMismoTipo(string pNombrePropiedad)
        {
            if (ComprobarPropiedadInferior(pNombrePropiedad))
                return true;
            else
            {
                switch (pNombrePropiedad)
                {
                    case "GrupoCliente":
                        return true;
                    case "GrupoProveedor":
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Selecciona o deselecciona un valor de la propiedad.
        /// </summary>
        /// <param name="pPropiedad">Propiedad de la que se debe deseleccionar uno de sus valores</param>
        /// <param name="pEntidadID">ID de la entidad que se debe deseleccionar</param>
        /// <param name="pSeleccionada">TRUE si se debe de seleccionar el valor, FALSE en caso contario</param>
        public static void MarcarValorPropiedadVisible(Propiedad pPropiedad, string pEntidadID, bool pSeleccionada)
        {
            //foreach (string valor in pPropiedad.ListaValores.Keys)
            //{
            //    if (valor.Equals(pEntidadID))
            //    {
            //        pPropiedad.ValoresSeleccionados[pEntidadID] = pSeleccionada;
            //        return;
            //    }
            //}
        }

        /// <summary>
        /// Calcula el número de valores de la propiedad que han sido seleccionados
        /// </summary>
        /// <param name="pPropiedad">Propiedad</param>
        /// <returns>Número de valores seleccionados de la propiedad</returns>
        public static int NumeroValoresSeleccionados(Propiedad pPropiedad)
        {
            //int numeroValores = 0;

            //foreach (string valor in pPropiedad.ListaValores.Keys)
            //{
            //    if (pPropiedad.ValoresSeleccionados[valor])
            //        numeroValores++;
            //}
            //return numeroValores;

            return pPropiedad.ListaValores.Count;
        }

        /// <summary>
        /// Obtiene el ID de la entidad licencia según el cógido de licencia de un documento.
        /// </summary>
        /// <param name="pLicencia">Código de licencia</param>
        /// <returns>ID de la entidad licencia según el cógido de licencia de un documento</returns>
        public static string ObtnerIDLicenciaDeDocumento(string pLicencia)
        {
            if (pLicencia == "00")
            {
                return "http://creativecommons.org/licenses/by/3.0/";
            }
            else if (pLicencia == "01")
            {
                return "http://creativecommons.org/licenses/by-sa/3.0/";
            }
            else if (pLicencia == "02")
            {
                return "http://creativecommons.org/licenses/by-nd/3.0/";
            }
            else if (pLicencia == "10")
            {
                return "http://creativecommons.org/licenses/by-nc/3.0/";
            }
            else if (pLicencia == "11")
            {
                return "http://creativecommons.org/licenses/by-nc-sa/3.0/";
            }
            else if (pLicencia == "12")
            {
                return "http://creativecommons.org/licenses/by-nc-nd/3.0/";
            }

            return null;
        }

        /// <summary>
        /// Obtiene el RDF de la licencia de un documento.
        /// </summary>
        /// <param name="pLicencia">Código de licencia</param>
        /// <param name="pLicenciaID">ID de la entidad licencia</param>
        /// <returns>RDF de la licencia de un documento</returns>
        public static string ObtnerRDFLicenciaDeDocumento(string pLicencia, string pLicenciaID)
        {
            string rdf = "\r\n  <cc:License rdf:about=\"" + pLicenciaID + "\">";

            if (pLicencia == "00")
            {
                rdf += "\r\n    <cc:requires rdf:resource=\"http://creativecommons.org/ns#Attribution\" />\r\n    <cc:permits rdf:resource=\"http://creativecommons.org/ns#Reproduction\" />\r\n    <cc:permits rdf:resource=\"http://creativecommons.org/ns#Distribution\" />\r\n    <cc:permits rdf:resource=\"http://creativecommons.org/ns#DerivativeWorks\" />\r\n    <cc:requires rdf:resource=\"http://creativecommons.org/ns#Notice\" />";
            }
            else if (pLicencia == "01")
            {
                rdf += "\r\n    <cc:requires rdf:resource=\"http://creativecommons.org/ns#Attribution\" />\r\n    <cc:permits rdf:resource=\"http://creativecommons.org/ns#Reproduction\" />\r\n    <cc:permits rdf:resource=\"http://creativecommons.org/ns#Distribution\" />\r\n    <cc:permits rdf:resource=\"http://creativecommons.org/ns#DerivativeWorks\" />\r\n    <cc:requires rdf:resource=\"http://creativecommons.org/ns#ShareAlike\" />\r\n    <cc:requires rdf:resource=\"http://creativecommons.org/ns#Notice\" />";
            }
            else if (pLicencia == "02")
            {
                rdf += "\r\n    <cc:requires rdf:resource=\"http://creativecommons.org/ns#Attribution\" />\r\n    <cc:permits rdf:resource=\"http://creativecommons.org/ns#Reproduction\" />\r\n    <cc:permits rdf:resource=\"http://creativecommons.org/ns#Distribution\" />\r\n    <cc:requires rdf:resource=\"http://creativecommons.org/ns#Notice\" />";
            }
            else if (pLicencia == "10")
            {
                rdf += "\r\n    <cc:requires rdf:resource=\"http://creativecommons.org/ns#Attribution\" />\r\n    <cc:permits rdf:resource=\"http://creativecommons.org/ns#Reproduction\" />\r\n    <cc:permits rdf:resource=\"http://creativecommons.org/ns#Distribution\" />\r\n    <cc:permits rdf:resource=\"http://creativecommons.org/ns#DerivativeWorks\" />\r\n    <cc:prohibits rdf:resource=\"http://creativecommons.org/ns#CommercialUse\" />\r\n    <cc:requires rdf:resource=\"http://creativecommons.org/ns#Notice\" />";
            }
            else if (pLicencia == "11")
            {
                rdf += "\r\n    <cc:requires rdf:resource=\"http://creativecommons.org/ns#Attribution\" />\r\n    <cc:permits rdf:resource=\"http://creativecommons.org/ns#Reproduction\" />\r\n    <cc:permits rdf:resource=\"http://creativecommons.org/ns#Distribution\" />\r\n    <cc:permits rdf:resource=\"http://creativecommons.org/ns#DerivativeWorks\" />\r\n    <cc:requires rdf:resource=\"http://creativecommons.org/ns#ShareAlike\" />\r\n    <cc:prohibits rdf:resource=\"http://creativecommons.org/ns#CommercialUse\" />\r\n    <cc:requires rdf:resource=\"http://creativecommons.org/ns#Notice\" />";
            }
            else if (pLicencia == "12")
            {
                rdf += "\r\n    <cc:requires rdf:resource=\"http://creativecommons.org/ns#Attribution\" /> \r\n    <cc:permits rdf:resource=\"http://creativecommons.org/ns#Reproduction\" />\r\n    <cc:permits rdf:resource=\"http://creativecommons.org/ns#Distribution\" />\r\n    <cc:prohibits rdf:resource=\"http://creativecommons.org/ns#CommercialUse\" />\r\n    <cc:requires rdf:resource=\"http://creativecommons.org/ns#Notice\" />";
            }

            rdf += "\r\n  </cc:License>\r\n";

            return rdf;
        }

        #endregion

        #region Métodos de entidades

        private static object ObtenerValorPropiedadPorReflection(object pObjeto, string pNombrePropiedad)
        {
            object resultado = null;

            Type type = pObjeto.GetType();
            PropertyInfo propertyInfo = type.GetProperty(pNombrePropiedad);

            if (propertyInfo != null)
            {
                resultado = propertyInfo.GetValue(pObjeto);
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene los atributos de la entidad
        /// </summary>
        /// <param name="pEntidadBuscada">Entidad de la cual se buscan sus atributos</param>
        /// <param name="pFila">Fila que representa la entidad buscada</param>
        public static void ObtenerAtributosEntidad(ElementoOntologia pEntidadBuscada, object pObjeto)
        {
            if (pObjeto is DataRow)
            {
                ObtenerAtributosEntidadDataRow(pEntidadBuscada, (DataRow)pObjeto);
            }
            else
            {
                if (pObjeto != null)
                {
                    Type type = pObjeto.GetType();

                    List<Propiedad> atributos = pEntidadBuscada.ObtenerAtributos();

                    //Recorremos los atributos
                    foreach (Propiedad atributo in atributos)
                    {
                        atributo.ElementoOntologia = pEntidadBuscada;
                        PropertyInfo propertyInfo = type.GetProperty(atributo.Nombre);

                        if (propertyInfo != null)
                        {
                            object valorPropiedad = propertyInfo.GetValue(pObjeto);
                            string valor = "";


                            if (propertyInfo.PropertyType.Equals(typeof(Byte[])))
                            {
                                if (((byte[])valorPropiedad).Length > 0)
                                    valor = UtilImportarExportar.BytesToString((byte[])valorPropiedad);
                            }
                            else if (propertyInfo.PropertyType.Equals(typeof(DateTime)))
                                valor = PasarFechaEnFormatoEstandar((DateTime)valorPropiedad);
                            else
                                valor = valorPropiedad.ToString();

                            atributo.UnicoValor = new KeyValuePair<string, Es.Riam.Semantica.OWL.ElementoOntologia>(valor, null);

                            if ((atributo.Nombre.Equals(UtilImportarExportar.PROPIEDAD_NOMBRE)) || (atributo.Nombre.Equals(UtilImportarExportar.PROPIEDAD_DESCRIPCION)) || (atributo.Nombre.Equals(UtilImportarExportar.PROPIEDAD_TITULO)) || (atributo.Nombre.ToLower().Equals("nombreperfil")) || (atributo.Nombre.ToLower().Equals(UtilImportarExportar.PROPIEDAD_SIOC_TITULO)) || (atributo.Nombre.ToLower().Equals(UtilImportarExportar.PROPIEDAD_PREFLABEL)))
                            {
                                //UtilImportarExportar.PROPIEDAD_SIOC_TITULO
                                //Asigno la descripción de la entidad con su nombre o su descripción
                                if ((atributo.UnicoValor.Key != null) && (!string.IsNullOrEmpty(atributo.UnicoValor.Key)))
                                    pEntidadBuscada.Descripcion = atributo.UnicoValor.Key;
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(pEntidadBuscada.ID))
                        ObtenerID(pEntidadBuscada, pObjeto, null);
                }
            }
        }

        /// <summary>
        /// Obtiene los atributos de la entidad
        /// </summary>
        /// <param name="pEntidadBuscada">Entidad de la cual se buscan sus atributos</param>
        /// <param name="pFila">Fila que representa la entidad buscada</param>
        public static void ObtenerAtributosEntidadDataRow(Es.Riam.Semantica.OWL.ElementoOntologia pEntidadBuscada, DataRow pFila)
        {
            if ((pFila != null) && (!pFila.RowState.Equals(DataRowState.Deleted)))
            {
                List<Propiedad> atributos = pEntidadBuscada.ObtenerAtributos();

                //Recorremos los atributos
                foreach (Propiedad atributo in atributos)
                {
                    atributo.ElementoOntologia = pEntidadBuscada;

                    if (pFila.Table.Columns.Contains(atributo.Nombre))
                    {
                        string valor = "";

                        if (pFila[atributo.Nombre] is Byte[])
                        {
                            if (((byte[])pFila[atributo.Nombre]).Length > 0)
                                valor = UtilImportarExportar.BytesToString((byte[])pFila[atributo.Nombre]);
                        }
                        else if (pFila[atributo.Nombre] is Guid)
                            valor = pFila[atributo.Nombre].ToString();
                        else if (pFila[atributo.Nombre] is DateTime)
                            valor = PasarFechaEnFormatoEstandar((DateTime)pFila[atributo.Nombre]);
                        else
                            valor = pFila[atributo.Nombre].ToString();

                        atributo.UnicoValor = new KeyValuePair<string, Es.Riam.Semantica.OWL.ElementoOntologia>(valor, null);

                        if ((atributo.Nombre.Equals(UtilImportarExportar.PROPIEDAD_NOMBRE)) || (atributo.Nombre.Equals(UtilImportarExportar.PROPIEDAD_DESCRIPCION)) || (atributo.Nombre.Equals(UtilImportarExportar.PROPIEDAD_TITULO)) || (atributo.Nombre.ToLower().Equals("nombreperfil")) || (atributo.Nombre.ToLower().Equals(UtilImportarExportar.PROPIEDAD_SIOC_TITULO)) || (atributo.Nombre.ToLower().Equals(UtilImportarExportar.PROPIEDAD_PREFLABEL)))
                        {
                            //UtilImportarExportar.PROPIEDAD_SIOC_TITULO
                            //Asigno la descripción de la entidad con su nombre o su descripción
                            if ((atributo.UnicoValor.Key != null) && (!string.IsNullOrEmpty(atributo.UnicoValor.Key)))
                                pEntidadBuscada.Descripcion = atributo.UnicoValor.Key;
                        }
                    }
                }
                if (string.IsNullOrEmpty(pEntidadBuscada.ID))
                    ObtenerID(pEntidadBuscada, pFila, null);
            }
        }

        /// <summary>
        /// Obtiene el identificador de la entidad pasada por parámetro contenido en la fila pasada por parámetro
        /// </summary>
        /// <param name="pEntidad">Entidad de la que se busca el identificador</param>
        /// <param name="pFila">Fila que contiene el identificador de la entidad</param>
        /// <param name="pElemento">Elemento</param>
        public static void ObtenerID(Es.Riam.Semantica.OWL.ElementoOntologia pEntidad, object pObjeto, IElementoGnoss pElemento)
        {
            if (pObjeto is DataRow)
            {
                ObtenerIDDataRow(pEntidad, pObjeto, pElemento);
            }
            else
            {
                //ObtenerValorPropiedadPorReflection
                if (pEntidad.Elemento == null)
                {
                    pEntidad.Elemento = pElemento;
                }

                switch (pEntidad.TipoEntidad)
                {
                    case TipoElementoGnoss.Documento:
                    case TipoElementoGnoss.Pregunta:
                    case TipoElementoGnoss.Debate:
                        object id = ObtenerValorPropiedadPorReflection(pObjeto, "DocumentoID");
                        if (id != null)
                        {
                            pEntidad.ID = id.ToString();
                        }
                        break;
                    case TipoElementoGnoss.DocumentoEntidad:
                        //pEntidad.ID = pFila["DocumentoID"].ToString();
                        object documentoID = ObtenerValorPropiedadPorReflection(pObjeto, "DocumentoID");
                        if (documentoID != null)
                        {
                            pEntidad.ID = documentoID.ToString();
                        }
                        break;
                    case TipoElementoGnoss.CategoriaDocumentacion:
                        //pEntidad.ID = pFila["CategoriaDocumentacionID"].ToString();
                        object categoriaID = ObtenerValorPropiedadPorReflection(pObjeto, "CategoriaDocumentacionID");
                        if (categoriaID != null)
                        {
                            pEntidad.ID = categoriaID.ToString();
                        }
                        break;
                    case TipoElementoGnoss.EntidadNoExportable:
                    case "":
                        pEntidad.ID = new Guid().ToString();
                        break;
                    case TipoElementoGnoss.Organizacion:
                        //pEntidad.ID = pFila["OrganizacionID"].ToString();
                        object organizacionID = ObtenerValorPropiedadPorReflection(pObjeto, "OrganizacionID");
                        if (organizacionID != null)
                        {
                            pEntidad.ID = organizacionID.ToString();
                        }
                        break;
                    case TipoElementoGnoss.Proyecto:
                        //pEntidad.ID = pFila["ProyectoID"].ToString();
                        object proyectoID = ObtenerValorPropiedadPorReflection(pObjeto, "ProyectoID");
                        if (proyectoID != null)
                        {
                            pEntidad.ID = proyectoID.ToString();
                        }
                        break;
                    case TipoElementoGnoss.Tesauro:
                        //pEntidad.ID = pFila["TesauroID"].ToString();
                        object tesauroID = ObtenerValorPropiedadPorReflection(pObjeto, "TesauroID");
                        if (tesauroID != null)
                        {
                            pEntidad.ID = tesauroID.ToString();
                        }
                        break;
                    case TipoElementoGnoss.CategoriasTesauro:
                    case TipoElementoGnoss.CategoriasTesauroSkos:
                        //pEntidad.ID = pFila["CategoriaTesauroID"].ToString();
                        object categoriaTesauroID = ObtenerValorPropiedadPorReflection(pObjeto, "CategoriaTesauroID");
                        if (categoriaTesauroID != null)
                        {
                            pEntidad.ID = categoriaTesauroID.ToString();
                        }
                        break;
                    case TipoElementoGnoss.NivelPrioridadProyecto:
                        //pEntidad.ID = pFila["NivelPrioridadID"].ToString();
                        object nivelPrioridadID = ObtenerValorPropiedadPorReflection(pObjeto, "NivelPrioridadID");
                        if (nivelPrioridadID != null)
                        {
                            pEntidad.ID = nivelPrioridadID.ToString();
                        }
                        break;
                    case TipoElementoGnoss.PerfilOrganizacion:
                    //case TipoElementoGnoss.PerfilOrganizacionFoaf:
                    //    if (pFila is IdentidadDS.IdentidadRow)
                    //    {
                    //        pEntidad.ID = ((IdentidadDS.IdentidadRow)pFila).PerfilRow.NombreCortoOrg;
                    //    }
                    //    else
                    //    {
                    //        pEntidad.ID = ((IdentidadDS.PerfilRow)pFila).NombreCortoOrg;
                    //    }
                    //    break;
                    case TipoElementoGnoss.PerfilPersona:
                    //case TipoElementoGnoss.PerfilPersonaFoaf:
                    //    if (pFila is IdentidadDS.IdentidadRow)
                    //    {
                    //        pEntidad.ID = ((IdentidadDS.IdentidadRow)pFila).PerfilRow.NombreCortoUsu;
                    //    }
                    //    else
                    //    {
                    //        pEntidad.ID = ((IdentidadDS.PerfilRow)pFila).NombreCortoUsu;
                    //    }
                    //    break;
                    //case TipoElementoGnoss.PerfilPersonaOrg:
                    //    if (pFila is IdentidadDS.IdentidadRow)
                    //    {
                    //        IdentidadDS.IdentidadRow filaIdent = (IdentidadDS.IdentidadRow)pFila;

                    //        if (filaIdent.Tipo.Equals((short)TiposIdentidad.ProfesionalCorporativo))
                    //        {
                    //            pEntidad.ID = filaIdent.PerfilRow.NombreCortoOrg;
                    //        }
                    //        else
                    //        {
                    //            pEntidad.ID = filaIdent.PerfilRow.NombreCortoUsu;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        pEntidad.ID = ((IdentidadDS.PerfilRow)pFila).NombreCortoUsu;
                    //    }
                    //    break;
                    case TipoElementoGnoss.PerfilPersonaFoaf:
                        object identidadID = ObtenerValorPropiedadPorReflection(pObjeto, "IdentidadID");
                        if(identidadID != null)
                        {
                            pEntidad.ID = identidadID.ToString();
                        }
                        break;
                    case TipoElementoGnoss.CVPersona:
                    case TipoElementoGnoss.CVOrganizacion:
                    case TipoElementoGnoss.CVSemantico:
                        //pEntidad.ID = pFila["Titulo"].ToString();
                        object titulo = ObtenerValorPropiedadPorReflection(pObjeto, "Titulo");
                        if (titulo != null)
                        {
                            pEntidad.ID = titulo.ToString();
                        }
                        break;
                    case TipoElementoGnoss.Comunidad:
                    case TipoElementoGnoss.ComunidadSioc:
                        //pEntidad.ID = pFila["NombreCorto"].ToString();
                        object nombreCorto = ObtenerValorPropiedadPorReflection(pObjeto, "NombreCorto");
                        if (nombreCorto != null)
                        {
                            pEntidad.ID = nombreCorto.ToString();
                        }
                        break;
                    case TipoElementoGnoss.USER_ACCOUNT:
                    //IdentidadDS.IdentidadRow filaIdentUser = (IdentidadDS.IdentidadRow)pFila;

                    //if ((filaIdentUser.Tipo > 1) && (filaIdentUser.Tipo < 4))
                    //{
                    //    pEntidad.ID = filaIdentUser.PerfilRow.NombreCortoOrg + "/user";
                    //}
                    //else
                    //{
                    //    pEntidad.ID = filaIdentUser.PerfilRow.NombreCortoUsu + "/user";
                    //}
                    //break;
                    case TipoElementoGnoss.USER_GROUP:
                        //if (pFila is ProyectoDS.ProyectoRow)
                        //{
                        //    pEntidad.ID = "User-group/" + pFila["NombreCorto"].ToString();
                        //}
                        //else //Es organización:
                        //{
                        //    pEntidad.ID = pFila["NombreCorto"].ToString();
                        //}
                        break;
                    case TipoElementoGnoss.ListaResultados:
                        //pEntidad.ID = pFila["ListaResultadosID"].ToString();
                        object listaResultadosID = ObtenerValorPropiedadPorReflection(pObjeto, "ListaResultadosID");
                        if (listaResultadosID != null)
                        {
                            pEntidad.ID = listaResultadosID.ToString();
                        }
                        break;
                    case TipoElementoGnoss.Filtro:
                        //pEntidad.ID = pFila["Nombre"].ToString();
                        object nombre = ObtenerValorPropiedadPorReflection(pObjeto, "Nombre");
                        if (nombre != null)
                        {
                            pEntidad.ID = nombre.ToString();
                        }
                        break;
                    case TipoElementoGnoss.Comentario:
                    case TipoElementoGnoss.ComentarioSioc:
                        //Comprobar si sirve igual para todos los que usan comentarios
                        //pEntidad.ID = pFila["ComentarioID"].ToString();
                        object comentarioID = ObtenerValorPropiedadPorReflection(pObjeto, "ComentarioID");
                        if (comentarioID != null)
                        {
                            pEntidad.ID = comentarioID.ToString();
                        }
                        break;
                    default:
                        //if (pObjeto != null && pFila.Table.Columns.Contains(pEntidad.TipoEntidad + "ID"))
                        //{
                        //    pEntidad.ID = pFila[pEntidad.TipoEntidad + "ID"].ToString();
                        //}
                        object tipoEntidad = ObtenerValorPropiedadPorReflection(pObjeto, pEntidad.TipoEntidad + "ID");
                        if (tipoEntidad != null)
                        {
                            pEntidad.ID = tipoEntidad.ToString();
                        }
                        break;
                }

                pEntidad.EstablecerID(pEntidad.ID);
            }
        }

        /// <summary>
        /// Obtiene el identificador de la entidad pasada por parámetro contenido en la fila pasada por parámetro
        /// </summary>
        /// <param name="pEntidad">Entidad de la que se busca el identificador</param>
        /// <param name="pFila">Fila que contiene el identificador de la entidad</param>
        /// <param name="pElemento">Elemento</param>
        public static void ObtenerIDDataRow(Es.Riam.Semantica.OWL.ElementoOntologia pEntidad, object pFila, IElementoGnoss pElemento)
        {
            if (pEntidad.Elemento == null)
            {
                pEntidad.Elemento = pElemento;
            }

            switch (pEntidad.TipoEntidad)
            {
                case TipoElementoGnoss.Documento:
                case TipoElementoGnoss.Pregunta:
                case TipoElementoGnoss.Debate:
                    pEntidad.ID = ((Guid)UtilReflection.GetValueReflection(pFila, "DocumentoID")).ToString();
                    break;
                case TipoElementoGnoss.DocumentoEntidad:
                    pEntidad.ID = ((Guid)UtilReflection.GetValueReflection(pFila, "DocumentoID")).ToString();
                    break;
                case TipoElementoGnoss.CategoriaDocumentacion:
                    pEntidad.ID = ((Guid)UtilReflection.GetValueReflection(pFila, "CategoriaDocumentacionID")).ToString();
                    break;
                case TipoElementoGnoss.EntidadNoExportable:
                case "":
                    pEntidad.ID = new Guid().ToString();
                    break;
                case TipoElementoGnoss.Organizacion:
                    pEntidad.ID = ((Guid)UtilReflection.GetValueReflection(pFila, "OrganizacionID")).ToString();
                    break;
                case TipoElementoGnoss.Proyecto:
                    pEntidad.ID = ((Guid)UtilReflection.GetValueReflection(pFila, "ProyectoID")).ToString();
                    break;
                case TipoElementoGnoss.Tesauro:
                    pEntidad.ID = ((Guid)UtilReflection.GetValueReflection(pFila, "TesauroID")).ToString();
                    break;
                case TipoElementoGnoss.CategoriasTesauro:
                case TipoElementoGnoss.CategoriasTesauroSkos:
                    pEntidad.ID = ((Guid)UtilReflection.GetValueReflection(pFila, "CategoriaTesauroID")).ToString();
                    break;
                case TipoElementoGnoss.NivelPrioridadProyecto:
                    pEntidad.ID = ((Guid)UtilReflection.GetValueReflection(pFila, "NivelPrioridadID")).ToString();
                    break;
                case TipoElementoGnoss.PerfilOrganizacion:
                case TipoElementoGnoss.PerfilOrganizacionFoaf:
                    if (pFila is AD.EntityModel.Models.IdentidadDS.Identidad)
                    {
                        pEntidad.ID = ((AD.EntityModel.Models.IdentidadDS.Identidad)pFila).Perfil.NombreCortoOrg;
                    }
                    else
                    {
                        pEntidad.ID = ((AD.EntityModel.Models.IdentidadDS.Perfil)pFila).NombreCortoOrg;
                    }
                    break;
                case TipoElementoGnoss.PerfilPersona:
                case TipoElementoGnoss.PerfilPersonaFoaf:
                    if (pFila is AD.EntityModel.Models.IdentidadDS.Identidad)
                    {
                        pEntidad.ID = ((AD.EntityModel.Models.IdentidadDS.Identidad)pFila).Perfil.NombreCortoUsu;
                    }
                    else
                    {
                        pEntidad.ID = ((AD.EntityModel.Models.IdentidadDS.Perfil)pFila).NombreCortoUsu;
                    }
                    break;
                case TipoElementoGnoss.PerfilPersonaOrg:
                    if (pFila is AD.EntityModel.Models.IdentidadDS.Identidad)
                    {
                        AD.EntityModel.Models.IdentidadDS.Identidad filaIdent = (AD.EntityModel.Models.IdentidadDS.Identidad)pFila;

                        if (filaIdent.Tipo.Equals((short)TiposIdentidad.ProfesionalCorporativo))
                        {
                            pEntidad.ID = filaIdent.Perfil.NombreCortoOrg;
                        }
                        else
                        {
                            pEntidad.ID = filaIdent.Perfil.NombreCortoUsu;
                        }
                    }
                    else
                    {
                        pEntidad.ID = ((AD.EntityModel.Models.IdentidadDS.Perfil)pFila).NombreCortoUsu;
                    }
                    break;
                case TipoElementoGnoss.CVPersona:
                case TipoElementoGnoss.CVOrganizacion:
                case TipoElementoGnoss.CVSemantico:
                    pEntidad.ID = ((string)UtilReflection.GetValueReflection(pFila, "Titulo")).ToString();
                    break;
                case TipoElementoGnoss.Comunidad:
                case TipoElementoGnoss.ComunidadSioc:
                    pEntidad.ID = ((string)UtilReflection.GetValueReflection(pFila, "NombreCorto")).ToString();
                    break;
                case TipoElementoGnoss.USER_ACCOUNT:
                    AD.EntityModel.Models.IdentidadDS.Identidad filaIdentUser = (AD.EntityModel.Models.IdentidadDS.Identidad)pFila;

                    if ((filaIdentUser.Tipo > 1) && (filaIdentUser.Tipo < 4))
                    {
                        pEntidad.ID = filaIdentUser.Perfil.NombreCortoOrg + "/user";
                    }
                    else
                    {
                        pEntidad.ID = filaIdentUser.Perfil.NombreCortoUsu + "/user";
                    }
                    break;
                case TipoElementoGnoss.USER_GROUP:
                    if (pFila is Proyecto)
                    {
                        pEntidad.ID = "User-group/" + ((string)UtilReflection.GetValueReflection(pFila, "NombreCorto")).ToString();
                    }
                    else //Es organización:
                    {
                        pEntidad.ID = ((string)UtilReflection.GetValueReflection(pFila, "NombreCorto")).ToString();
                    }
                    break;
                case TipoElementoGnoss.ListaResultados:
                    pEntidad.ID = ((DataRow)pFila)["ListaResultadosID"].ToString();
                    break;
                case TipoElementoGnoss.Filtro:
                    pEntidad.ID = ((DataRow)pFila)["Nombre"].ToString();
                    break;
                case TipoElementoGnoss.Comentario:
                case TipoElementoGnoss.ComentarioSioc:
                    //Comprobar si sirve igual para todos los que usan comentarios
                    pEntidad.ID = ((Guid)UtilReflection.GetValueReflection(pFila, "ComentarioID")).ToString();
                    break;
                default:
                    pEntidad.ID = ((Guid)UtilReflection.GetValueReflection(pFila, pEntidad.TipoEntidad + "ID")).ToString();
                    break;
            }

            pEntidad.EstablecerID(pEntidad.ID);
        }

        /// <summary>
        /// Obtiene el identificador del elemento
        /// </summary>
        /// <param name="pTipoEntidad">Tipo de entidad</param>
        /// <param name="pElemento">Elemento</param>
        /// <returns>Identificador del elemento</returns>
        public static string ObtenerID(string pTipoEntidad, IElementoGnoss pElemento)
        {
            string ID = new Guid().ToString();

            return GestionOWLGnoss.ObtenerUrlEntidad(pTipoEntidad, pElemento, ID);
        }

        /// <summary>
        /// Obtiene el identificador de la entidad pasada por parámetro contenido en la fila pasada por parámetro 
        /// </summary>
        /// <param name="pTipoEntidad">Tipo de entidad de la que se busca el identificador</param>
        /// <param name="pFila">Fila que contiene el identificador de la entidad</param>
        /// <returns>Identificador de la entidad</returns>
        public static string ObtenerID(string pTipoEntidad, DataRow pFila)
        {
            string ID = new Guid().ToString();

            if (pFila != null)
            {
                switch (pTipoEntidad)
                {
                    case TipoElementoGnoss.Documento:
                    case TipoElementoGnoss.Pregunta:
                    case TipoElementoGnoss.Debate:
                        if (pFila.Table.Columns.Contains("DocumentoID"))
                        {
                            ID = pFila["DocumentacionID"].ToString();
                        }
                        break;
                    case TipoElementoGnoss.CategoriasTesauro:
                    case TipoElementoGnoss.CategoriasTesauroSkos:
                        ID = pFila["CategoriaTesauroID"].ToString();
                        break;
                    case TipoElementoGnoss.CategoriaDocumentacion:
                        if (pFila.Table.Columns.Contains("CategoriaDocumentacionID"))
                            ID = pFila["CategoriaDocumentacionID"].ToString();
                        else if (pFila.Table.Columns.Contains("ModoID"))
                            ID = pFila["ModoID"].ToString();
                        break;
                    case TipoElementoGnoss.EntidadNoExportable:
                    case "":
                        ID = new Guid().ToString();
                        break;
                    case TipoElementoGnoss.Comentario:
                    case TipoElementoGnoss.ComentarioSioc:
                        //Comprobar si sirve igual para todos los que usan comentarios
                        ID = pFila[2].ToString();
                        break;
                    case TipoElementoGnoss.TagSioc:
                        ID = pFila["Nombre"].ToString();
                        break;
                    case TipoElementoGnoss.ListaResultados:
                        ID = pFila["ListaResultadosID"].ToString();
                        break;
                    case TipoElementoGnoss.Filtro:
                        ID = pFila["Nombre"].ToString();
                        break;
                    default:
                        if (pFila != null && pFila.Table.Columns.Contains(pTipoEntidad + "ID"))
                            ID = pFila[pTipoEntidad + "ID"].ToString();
                        break;
                }
            }
            return ID;
        }

        /// <summary>
        /// Determina si una entidad es válida
        /// </summary>
        /// <param name="pEntidad">Entidad que hay que determinar</param>
        /// <param name="pNombrePropiedad">Nombre de la propiedad de la que proviene la entidad</param>
        public static void DeterminarEntidadValida(Es.Riam.Semantica.OWL.ElementoOntologia pEntidad, string pNombrePropiedad)
        {
            //Pongo la entidad como no válida para obligar a crearla, no permite seleccionar una existente
            switch (pNombrePropiedad)
            {
                //case "GrupoInferior":
                //    pEntidad.EntidadValida = false;
                //    break;
                //case "Procesos inferiores":
                //    pEntidad.EntidadValida = false;
                //    break;
                //case "Objetivos inferiores":
                //    pEntidad.EntidadValida = false;
                //    break;
                //case "Competencias inferiores":
                //    pEntidad.EntidadValida = false;
                //    break;
                case "Actuaciones inferiores":
                    pEntidad.EntidadValida = false;
                    break;
                case "Tareas inferiores":
                    pEntidad.EntidadValida = false;
                    break;
                case "ElementoEstructuraSecundario":
                    pEntidad.EntidadValida = false;
                    break;
                case "Elementos de estructura secundarios":
                    pEntidad.EntidadValida = false;
                    break;
                    //case "Documentacion":
                    //    pEntidad.EntidadValida = false;
                    //    break;
                    //case UtilImportarExportar.PROPIEDAD_ENTRADAS:
                    //    pEntidad.EntidadValida = false;
                    //    break;
                    //case UtilImportarExportar.PROPIEDAD_MECANISMOS:
                    //    pEntidad.EntidadValida = false;
                    //    break;
                    //case UtilImportarExportar.PROPIEDAD_CONTROLES:
                    //    pEntidad.EntidadValida = false;
                    //    break;
                    //case UtilImportarExportar.PROPIEDAD_SALIDAS:
                    //    pEntidad.EntidadValida = false;
                    //    break;
            }
        }

        #endregion

        #region Métodos útiles

        /// <summary>
        /// Transforma una lista de bytes en una cadena de caracteres
        /// </summary>
        /// <param name="pBytes">Array de bytes</param>
        /// <returns>Cadena de caracteres</returns>
        private static string BytesToString(Byte[] pBytes)
        {
            return System.Convert.ToBase64String(pBytes);
        }

        /// <summary>
        /// Transforma una cadena de caracteres en un array de bytes
        /// </summary>
        /// <param name="pBytes">Cadena de caracteres</param>
        /// <returns>Array de bytes</returns>
        private static Byte[] StringToByte(string pBytes)
        {
            return System.Convert.FromBase64String(pBytes);
        }

        /// <summary>
        /// Añade a la lista el padre y todos sus hijos
        /// </summary>
        /// <param name="pLista">Lista de elementos</param>
        /// <param name="pPadre">Padre de todos los elementos que se van a añadir</param>
        public static void AniadirHijosALista(IList<IElementoGnoss> pLista, IElementoGnoss pPadre)
        {
            pLista.Add(pPadre);

            if (pPadre.Hijos.Count > 0)
            {
                foreach (IElementoGnoss hijo in pPadre.Hijos)
                {
                    AniadirHijosALista(pLista, hijo);
                }
            }
        }

        /// <summary>
        /// Añade a la lista el padre y todos sus hijos
        /// </summary>
        /// <param name="pLista">Lista de elementos</param>
        /// <param name="pPadre">Padre de todos los elementos que se van a añadir</param>
        /// <param name="pTipo">Tipo de entidad</param>
        public static void AniadirHijosALista(IList<IElementoGnoss> pLista, IElementoGnoss pPadre, string pTipo)
        {
            if (pPadre.GetType().Name.Equals(pTipo))
                pLista.Add(pPadre);

            if (pPadre.Hijos.Count > 0)
            {
                foreach (IElementoGnoss hijo in pPadre.Hijos)
                {
                    AniadirHijosALista(pLista, hijo, pTipo);
                }
            }
        }

        /// <summary>
        /// Transforma la lista de metadatos en una cadena de caracteres
        /// </summary>
        /// <param name="pMetadatos">Diccionario con los metadatos de la organización</param>
        /// <returns>Cadena de caracteres</returns>
        public static string MetadatosToString(Dictionary<string, string> pMetadatos)
        {
            string resultado = "";

            foreach (string clave in pMetadatos.Keys)
            {
                resultado += clave + ": " + pMetadatos[clave] + "\r\n";
            }
            return resultado;
        }

        #endregion

        #endregion
    }
}
