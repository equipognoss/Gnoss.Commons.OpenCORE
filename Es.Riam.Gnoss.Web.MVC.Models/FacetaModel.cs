using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models
{
    /// <summary>
    /// Enumeración con los tipos de cajas de una faceta
    /// </summary>
    [Serializable]
    public enum SearchBoxType
    {
        /// <summary>
        /// Sin caja de búsqeuda
        /// </summary>
        None = 0,
        /// <summary>
        /// Caja simple
        /// </summary>
        Simple = 1,
        /// <summary>
        /// Caja fechas desde-hasta
        /// </summary>
        FromToDates = 2,
        /// <summary>
        /// Caja rangos desde-hasta
        /// </summary>
        FromToRank = 3,
        /// <summary>
        /// Caja calendario
        /// </summary>
        Calendar = 4,
        /// <summary>
        /// Caja arbol-lista (por defecto siempre Arbol primero)
        /// </summary>
        TreeList = 5,
        /// <summary>
        /// Caja Rango Desde
        /// </summary>
        FromRank = 6,
        /// <summary>
        /// Caja Rango Hasta
        /// </summary>
        ToRank = 7,
        /// <summary>
        /// Caja calendario con rangos
        /// </summary>
        RankCalendar = 8,
        /// <summary>
        /// Caja arbol-lista
        /// </summary>
        ListTree = 9
    }

    /// <summary>
    /// Tipo del autocompletar
    /// </summary>
    [Serializable]
    public enum AutocompleteTypeSearchBox
    {
        /// <summary>
        /// Sin autocompletar
        /// </summary>
        None = 0,
        /// <summary>
        /// Autocompleta en la Bandeja del usuaro (mensaje, invitaciones...)
        /// </summary>
        AutocompleteUser = 1,
        /// <summary>
        /// Busca en sqlserver, se activa cuando no hay filtros
        /// </summary>
        AutocompleteTipedTags = 2,
        /// <summary>
        /// Autocompleta una faceta desde virtuoso cuando hay filtros (En el MetaProyecto)
        /// </summary>
        AutocompleteGeneric = 3,
        /// <summary>
        /// Autocompleta una faceta desde virtuoso cuando hay filtros (En una comunidad que no sea el MetaProyecto)
        /// </summary>
        AutocompleteGenericWithContextFilter = 4
    }

    /// <summary>
    /// Behavior of the autocomplete text box
    /// </summary>
    [Serializable]
    public enum AutocompleteBehaviours
    {
        /// <summary>
        /// Default behaviour
        /// </summary>
        Default = 0,
        /// <summary>
        /// Show only the autocomplete text box, without items
        /// </summary>
        OnlyTextBox = 1,
    }

    /// <summary>
    /// Modelo del buscador facetado
    /// </summary>
    [Serializable]
    public partial class FacetedModel
    {
        /// <summary>
        /// Lista de facetas
        /// </summary>
        public List<FacetModel> FacetList { get; set; }
        /// <summary>
        /// Lista de filtros
        /// </summary>
        public List<FacetItemModel> FilterList { get; set; }
    }

    /// <summary>
    /// Modelo de una faceta
    /// </summary>
    [Serializable]
    public partial class FacetModel
    {
        /// <summary>
        /// ID del tesauro
        /// </summary>
        public Guid ThesaurusID { get; set; }
        /// <summary>
        /// Indica si hay que mostrar un ver mas
        /// </summary>
        public bool SeeMore { get; set; }
        /// <summary>
        /// ID de la faceta
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Nombre de la faceta
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Identificador de la faceta
        /// </summary>
        public string FacetKey { get; set; }
        /// <summary>
        /// Orden
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Indica si se trata de una faceta MultiIdioma
        /// </summary>
        public string Multilanguage { get; set; }
        /// <summary>
        /// Tipo de la caja de búsqueda
        /// </summary>
        public SearchBoxType SearchBoxType { get; set; }
        /// <summary>
        /// Tipo de autocompletar de la caja de búsqueda
        /// </summary>
        public AutocompleteTypeSearchBox AutocompleteTypeSearchBox { get; set; }
        /// <summary>
        /// Listado de los items de la facetas
        /// </summary>
        public List<FacetItemModel> FacetItemList { get; set; }
        /// <summary>
        /// Listado de grupos agrupados
        /// </summary>
        public Dictionary<string, List<string>> GroupedGroups { get; set; }
        /// <summary>
        /// Indica si es una faceta agrupada
        /// </summary>
        public bool FacetGrouped { get; set; }

        public AutocompleteBehaviours AutocompleteBehaviour { get; set; }

        /// <summary>
        /// Indica si la faceta se muestra aunque no contenga ningún item
        /// </summary>
        public bool ShowWithoutItems { get; set; }

        /// <summary>
        /// Indica si la petición era para traerse una única faceta
        /// </summary>
        public bool OneFacetRequest { get; set; }

        /// <summary>
        /// Si la faceta está dividida, aquí se almacena el valor del filtro que cumplen los valores de esta faceta.
        /// </summary>
        public string Filter { get; set; }

    }

    /// <summary>
    /// Item de una faceta
    /// </summary>
    [Serializable]
    public partial class FacetItemModel
    {
        /// <summary>
        /// Título
        /// </summary>
        public string Tittle { get; set; }
        /// <summary>
        /// Indica si está seleccinado en los filtros el ítem de la faceta
        /// </summary>
        public bool Selected { get; set; }
        /// <summary>
        /// Indica el número de resultados con ese filtro
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// Filtro a aplicar
        /// </summary>
        public string Filter { get; set; }
        /// <summary>
        /// Nombre
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Listadeo de ítem dentro del propio ítem(facetas anidadas)
        /// </summary>
        public List<FacetItemModel> FacetItemlist { get; set; }

        /// <summary>
        /// Propiedad que indica si la faceta es hija de otra faceta (Recursividad)
        /// </summary>
        public bool IsChildren { get; set; }
    }
}