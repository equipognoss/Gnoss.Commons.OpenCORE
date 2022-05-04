using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models
{
    /// <summary>
    /// Modelo para el espacio personal de GNOSS.
    /// </summary>
    public class PersonalSpaceModel
    {
        /// <summary>
        /// Número de resultados total en el espaciopersonal.
        /// </summary>
        public int TotalNumberResults { get; set; }

        /// <summary>
        /// MegaBytes utilizados en el espacio personal.
        /// </summary>
        public decimal UsedMegaBytes { get; set; }

        /// <summary>
        /// MegaBytes libres en el espacio personal.
        /// </summary>
        public decimal FreeMegaBytes { get; set; }

        /// <summary>
        /// Url para administrar las categorías del espacio personal.
        /// </summary>
        public string AdminCategoriesUrl { get; set; }

        /// <summary>
        /// Url para añadir nuevo recurso.
        /// </summary>
        public string AddNewResourceUrl { get; set; }

        /// <summary>
        /// Modelo para la búsqueda.
        /// </summary>
        public SearchViewModel SearchViewModel { get; set; }

        /// <summary>
        /// Título para la página.
        /// </summary>
        public string PageTitle { get; set; }
    }

    /// <summary>
    /// Modelo para la acción de edición de categorías de los recursos en el espacio personal. Se usa para mostrar el selector de categorías y para aceptarlas.
    /// </summary>
    public class EditCategoriesPersonalSpaceModel
    {
        /// <summary>
        /// IDs de los recursos seleccionados separados por comas.
        /// </summary>
        public string SelectedResources { get; set; }

        /// <summary>
        /// IDs de las categorías seleccionadas separadas por comas. Solo debe enviarse en la acción de aceptar las categorías.
        /// </summary>
        public string SelectedCategories { get; set; }
    }

    /// <summary>
    /// Modelo para la página de administración de las categorías del espacio personal.
    /// </summary>
    public class AdminCatEspacioPersonalModel
    {
        /// <summary>
        /// Modelo para la edición del tesauro de la administración de categorías.
        /// </summary>
        public ThesaurusEditorModel ThesaurusEditorModel { get; set; }

        /// <summary>
        /// Lista de IDs mas nombre de categorías padre sobre las que se pueden crear nuevas categorías.
        /// </summary>
        public Dictionary<Guid, string> ParentCategoriesForCreateNewsCategories { get; set; }

        /// <summary>
        /// Url para volver atrás.
        /// </summary>
        public string BackUrl { get; set; }

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
        public Dictionary<Guid, string> ParentCategoriesForMoveCategories { get; set; }

        /// <summary>
        /// Lista de IDs mas nombre de categorías padre sobre las que se pueden ordenar (poner detrás) otras categorías.
        /// </summary>
        public Dictionary<Guid, string> ParentCategoriesForOrderCategories { get; set; }

        /// <summary>
        /// Lista de IDs mas nombre de categorías padre sobre las que se pueden mover los recursos de las categorías que se van a eliminar.
        /// </summary>
        public Dictionary<Guid, string> ParentCategoriesForDeleteCategories { get; set; }

        /// <summary>
        /// Nombres de las categorías que se van a ordenar.
        /// </summary>
        public List<string> CategoryNamesToOrder { get; set; }

        /// <summary>
        /// Nombres de las categorías que se van a eliminar.
        /// </summary>
        public List<string> CategoryNamesToDelete { get; set; }

        /// <summary>
        /// Indica si los recursos de las categorías que se van a eliminar son no huerfanos, es decir, que tienen otras categorías vinculadas a aparte de las que se van a eliminar.
        /// </summary>
        public bool ResourcesOfCategoriesDeletingAreNotOrphans { get; set; }
    }

    /// <summary>
    /// Modelo para las acciones de edición del tesauro personal.
    /// </summary>
    public class EditThesaurusPersonalSpaceModel
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
        public Guid SelectedCategory { get; set; }

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
}
