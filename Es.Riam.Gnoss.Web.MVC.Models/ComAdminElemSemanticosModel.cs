using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models
{
    [Serializable]
    /// <summary>
    /// Modelo para administrar los elementos semátnicos de la comunidad.
    /// </summary>
    public class ComAdminSemanticElemModel
    {

        /// <summary>
        /// Lista de idiomas de la plataforma
        /// </summary>
        public Dictionary<string, string> ListaIdiomas { get; set; }

        public string IdiomaPorDefecto { get; set; }
        /// <summary>
        /// 
        /// </summary>


        /// <summary>
        /// Tipo de página de la administración.
        /// </summary>
        public ComAdminSemanticElemPage PageType;

        /// <summary>
        /// Modelo para la edición de tesauros semánticos.
        /// </summary>
        public ComAdminEditSemanticThesaurus SemanticThesaurus;

        /// <summary>
        /// Modelo para la edición de entidades secundarias.
        /// </summary>
        public ComAdminEditSecondaryEntities SecondaryEntities;

        /// <summary>
        /// Modelo para la edición de grafos simples.
        /// </summary>
        public ComAdminEditSimpleGraphs SimpleGraphs;

        /// <summary>
        /// Tipo de página de administración.
        /// </summary>
        public enum ComAdminSemanticElemPage
        {
            /// <summary>
            /// Página de administración de ontologías.
            /// </summary>
            OntologiesAdmin = 0,
            /// <summary>
            /// Página de edición de tesauros semánticos.
            /// </summary>
            SemanticThesaurusEdition = 1,
            /// <summary>
            /// Página de edición de entidades secundarias.
            /// </summary>
            SecondaryEntitiesEdition = 2,
            /// <summary>
            /// Página de edición de grafos simples.
            /// </summary>
            SimpleGraphsEdition = 3
        }
    }
    [Serializable]
    /// <summary>
    /// Modelo para la edición de tesauros semánticos.
    /// </summary>
    public class ComAdminEditSemanticThesaurus
    {
        /// <summary>
        /// Lista de Ontologias
        /// </summary>
        public Dictionary<string, string> ListaOntologias { get; set; }

        /// <summary>
        /// Tesauros semáticos editables. Url de la ontologia, Source, Nombre del tesauro.
        /// </summary>
        public Dictionary<KeyValuePair<string, string>, string> SemanticThesaurusEditables;

        /// <summary>
        /// Modelo para la edición del tesauro de la administración de categorías.
        /// </summary>
        public ThesaurusEditorModel ThesaurusEditorModel { get; set; }

        /// <summary>
        /// Lista de IDs mas nombre de categorías padre sobre las que se pueden crear nuevas categorías.
        /// </summary>
        public Dictionary<string, string> ParentCategoriesForCreateNewsCategories { get; set; }

        /// <summary>
        /// Backup de acciones realizadas hasta el momento.
        /// </summary>
        public string ActionsBackUp { get; set; }

        /// <summary>
        /// Nombres de las categorías que se van a mover.
        /// </summary>
        public List<string> CategoryNamesToMove { get; set; }

        /// <summary>
        /// Lista de IDs mas nombre de categorías padre sobre las que se pueden mover otras categorías.
        /// </summary>
        public Dictionary<string, string> ParentCategoriesForMoveCategories { get; set; }

        ///// <summary>
        ///// Lista de IDs mas nombre de categorías padre sobre las que se pueden ordenar (poner detrás) otras categorías.
        ///// </summary>
        //public Dictionary<Guid, string> ParentCategoriesForOrderCategories { get; set; }

        /// <summary>
        /// Lista de IDs mas nombre de categorías padre sobre las que se pueden mover los recursos de las categorías que se van a eliminar.
        /// </summary>
        public Dictionary<string, string> ParentCategoriesForDeleteCategories { get; set; }

        /// <summary>
        /// Nombres de las categorías que se van a ordenar.
        /// </summary>
        public List<string> CategoryNamesToOrder { get; set; }

        /// <summary>
        /// Nombres de las categorías que se van a eliminar.
        /// </summary>
        public List<string> CategoryNamesToDelete { get; set; }

        ///// <summary>
        ///// Indica si los recursos de las categorías que se van a eliminar son no huerfanos, es decir, que tienen otras categorías vinculadas a aparte de las que se van a eliminar.
        ///// </summary>
        //public bool ResourcesOfCategoriesDeletingAreNotOrphans { get; set; }

        /// <summary>
        /// Idiomas para el tesauro semántico.
        /// </summary>
        public Dictionary<string, string> SemThesaurusLanguajes { get; set; }

        /// <summary>
        /// Url de la ontología del tesauro. Ej: 'taxonomy.owl'.
        /// </summary>
        public string OntologyUrl { get; set; }

        /// <summary>
        /// Fuente del tesauro semántico.
        /// </summary>
        public string SourceSemanticThesaurus { get; set; }

        /// <summary>
        /// Valores de las propiedades extra de las categorias.
        /// </summary>
        public string ExtraSemanticPropertiesValuesBK { get; set; }
    }
	[Serializable]
	/// <summary>
	/// Modelo para la edición de entidades secundarias.
	/// </summary>
	public class ComAdminEditSecondaryEntities
    {
        /// <summary>
        /// Listado de entidades secundarias que son editables.
        /// </summary>
        public Dictionary<string, string> SecondaryEntitiesEditables { get; set; }

        /// <summary>
        /// Listado de entidades secundarias que son editables.
        /// </summary>
        public SortedDictionary<string, string> SecondaryInstancesEditables { get; set; }

        /// <summary>
        /// Nombre de la ontología secundaría seleccionada para la edición.
        /// </summary>
        public string SecondaryOntologyNameSelected { get; set; }

        /// <summary>
        /// Modelo para editar recursos semánticos. Solo aplica a recursos semánticos.
        /// </summary>
        public SemanticResourceModel SemanticResourceModel { get; set; }

        /// <summary>
        /// Indica si se está creando una nueva instancia secundaria o por el contrario se está editando una.
        /// </summary>
        public bool CreatingNewInstance { get; set; }

        /// <summary>
        /// Nombre de la propiedad que representa el título o el nombre de la ontología.
        /// </summary>
        public string PropertyNameRepresentOntologyTitle { get; set; }

    }
	[Serializable]
	/// <summary>
	/// Modelo para la edición de grafos simples.
	/// </summary>
	public class ComAdminEditSimpleGraphs
    {
        /// <summary>
        /// Listado de grafos simples que son editables.
        /// </summary>
        public Dictionary<string, string> SimpleGraphsEditables { get; set; }

        /// <summary>
        /// Listado de instancias de un grafo simple que son editables.
        /// </summary>
        public List<string> SimpleGraphsInstancesEditables { get; set; }

        /// <summary>
        /// Nombre del grafo seleccionado para la edición.
        /// </summary>
        public string SimpleGraphsNameSelected { get; set; }
    }
	[Serializable]
	/// <summary>
	/// Modelo de menú para administrar los elementos semátnicos de la comunidad.
	/// </summary>
	public class ComAdminSemanticElemModelMenu
    {
        /// <summary>
        /// Url para administrar las plantillas ontológicas.
        /// </summary>
        public string SemanticOntologyUrl;

        /// <summary>
        /// Url para editar los tesauros semánticos.
        /// </summary>
        public string SemanticThesaurusEditionUrl;

        /// <summary>
        /// Url para editar los tesauros semánticos.
        /// </summary>
        public string SecondaryEntitiesEditionUrl;

        /// <summary>
        /// Url para editar los grafos simples.
        /// </summary>
        public string SimpleGraphsEditionUrl;
    }
	[Serializable]
	/// <summary>
	/// Modelo para las acciones de edición del tesauro semántico.
	/// </summary>
	public class EditSemanticThesaurusModel
    {

        /// <summary>
        /// Acción realizada.
        /// </summary>
        public Action EditAction { get; set; }

        /// <summary>
        /// Backup de acciones realizadas hasta el momento.
        /// </summary>
        public string ActionsBackUp { get; set; }

        /// <summary>
        /// Categoría seleccionada para las acciones:
        /// - Crear categoría: Padre de la nueva categoría.
        /// - Renombrar categoría: Categoría que se va a renombrar.
        /// - Mover categorías: Categoría a la que se mueven las seleccionadas.
        /// - Ordenar categorías: Categoría tras la que se ordenan las seleccionadas.
        /// - Eliminar categorías: Categoría a la que se mueven los recursos que pertenecían a las categorías eliminadas.
        /// </summary>
        public string SelectedCategory { get; set; }

        /// <summary>
        /// Categorías seleccionadas para las acciones:
        /// - Preparar mover categorías: Categorías que se van a mover.
        /// - Mover categorías: Categorías que se van a mover.
        /// - Ordenar categorías: Categorías que se van a ordenar.
        /// - Eliminar categorías: Categorías que se van a eliminar.
        /// </summary>
        public string SelectedCategories { get; set; }

        /// <summary>
        /// Nuevo nombre la categoría para las acciones de crear categoría y cambiar de nombre una.
        /// </summary>
        public string NewCategoryName { get; set; }

        /// <summary>
        /// Nuevo indentificador la categoría para las acciones de crear categoría y cambiar de nombre una.
        /// </summary>
        public string NewCategoryIdentifier { get; set; }

        /// <summary>
        /// Url de la ontología del tesauro. Ej: 'taxonomy.owl'.
        /// </summary>
        public string OntologyUrl { get; set; }

        /// <summary>
        /// Fuente del tesauro semántico.
        /// </summary>
        public string SourceSemanticThesaurus { get; set; }

        /// <summary>
        /// Valores de las propiedades extra del tesauro.
        /// </summary>
        public string CategoryExtraPropertiesValues { get; set; }

        /// <summary>
        /// Valores de las propiedades extra de las categorias.
        /// </summary>
        public string ExtraSemanticPropertiesValuesBK { get; set; }

        /// <summary>
        /// Enumeración con las acciones que se pueden hacer al editar el tesauro.
        /// </summary>
        public enum Action
        {
            /// <summary>
            /// Acción de crear una categoría.
            /// </summary>
            CreateCategory = 0,
            /// <summary>
            /// Acción de renombrar una categoría.
            /// </summary>
            ReNameCategory = 1,
            /// <summary>
            /// Acción de preparar el movimiento de categorías.
            /// </summary>
            PrepareMoveCategories = 2,
            /// <summary>
            /// Acción de mover categorías.
            /// </summary>
            MoveCategories = 3,
            /// <summary>
            /// Acción de preparar la ordenación de categorías.
            /// </summary>
            PrepareOrderCategories = 4,
            /// <summary>
            /// Acción de ordenar categorías.
            /// </summary>
            OrderCategories = 5,
            /// <summary>
            /// Acción de preparar la eliminación de categorías.
            /// </summary>
            PrepareDeleteCategories = 6,
            /// <summary>
            /// Acción de eliminar categorías.
            /// </summary>
            DeleteCategories = 7,
            /// <summary>
            /// Acción de eliminar categorías y mover solo los recursos huerfanos.
            /// </summary>
            DeleteCategoriesAndMoveOnlyOrphansResources = 8,
            /// <summary>
            /// Acción de guardar los cambios en el tesauro.
            /// </summary>
            SaveThesaurus = 9,
            /// <summary>
            /// Acción de cargar el tesauro de inicio.
            /// </summary>
            LoadThesaurus = 10,
            /// <summary>
            /// Acción de editar las propiedades semánticas de una categoría.
            /// </summary>
            EditExtraProperties = 11
        }
    }
	[Serializable]
	/// <summary>
	/// Modelo para las acciones de edición de entidades secundarias.
	/// </summary>
	public class EditSecondaryEntityModel
    {
        /// <summary>
        /// Url de la ontología de la entidad secundaria. Ej: 'taxonomy.owl'.
        /// </summary>
        public string OntologyUrl { get; set; }

        /// <summary>
        /// Acción realizada.
        /// </summary>
        public Action EditAction { get; set; }

        /// <summary>
        /// Instancias de una ontología seleccionadas para su edición.
        /// </summary>
        public string SelectedInstances { get; set; }

        /// <summary>
        /// RDF de la instancia secundaría que se está guardando.
        /// </summary>
        public string RdfValue { get; set; }

        /// <summary>
        /// Sujeto de la entidad secundaria nueva o editada.
        /// </summary>
        public string EntitySubject { get; set; }

        /// <summary>
        /// Enumeración con las acciones que se pueden hacer al editar el tesauro.
        /// </summary>
        public enum Action
        {
            /// <summary>
            /// Acción de cargar las instancias de una ontología secundaria.
            /// </summary>
            LoadInstances = 0,
            /// <summary>
            /// Acción de crear una nueva instancia de una ontología secundaria.
            /// </summary>
            CreateNewInstance = 1,
            /// <summary>
            /// Acción de editar una instancia de una ontología secundaria.
            /// </summary>
            EditInstance = 2,
            /// <summary>
            /// Acción de eliminar una instancia de una ontología secundaria.
            /// </summary>
            DeleteInstance = 3,
            /// <summary>
            /// Acción de guardar una nueva instancia de una ontología secundaria.
            /// </summary>
            SaveNewInstance = 4,
            /// <summary>
            /// Acción de guardar una instancia de una ontología secundaria.
            /// </summary>
            SaveInstance = 5
        }
    }
	[Serializable]
	/// <summary>
	/// Modelo para las acciones de edición de grafos simples.
	/// </summary>
	public class EditSimpleGraphModel
    {
        /// <summary>
        /// Grafo editado.
        /// </summary>
        public string Graph { get; set; }

        /// <summary>
        /// Acción realizada.
        /// </summary>
        public Action EditAction { get; set; }

        /// <summary>
        /// Instancias de un grafo simple seleccionadas para su edición.
        /// </summary>
        public string SelectedInstances { get; set; }

        /// <summary>
        /// Nuevo elemento para el grafo.
        /// </summary>
        public string NewElement { get; set; }

        /// <summary>
        /// Enumeración con las acciones que se pueden hacer al editar el grafo.
        /// </summary>
        public enum Action
        {
            /// <summary>
            /// Acción de cargar las instancias de un grafo simple.
            /// </summary>
            LoadInstances = 0,
            /// <summary>
            /// Acción de crear una nueva instanciade un grafo simple.
            /// </summary>
            CreateNewInstance = 1,
            /// <summary>
            /// Acción de eliminar una instancia de de un grafo simple.
            /// </summary>
            DeleteInstance = 2,
        }
    }
}
