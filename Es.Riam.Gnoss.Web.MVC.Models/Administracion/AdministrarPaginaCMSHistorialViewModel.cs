using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AdministrarPaginaCMSHistorialViewModel
    {
        public List<AdministrarPaginaCMSVersionViewModel> PaginasVersiones { get; set; } = new List<AdministrarPaginaCMSVersionViewModel>();
    }
    public class AdministrarPaginaCMSComparadorViewModel
    {
        public AdministrarPaginaCMSVersionViewModel Modelo1 { get; set; }
        public AdministrarPaginaCMSVersionViewModel Modelo2 { get; set; }
        public bool Restaurando { get; set; }
        public HashSet<Guid> RowsDiff { get; set; }
        public HashSet<Guid> ColsDiff { get; set; }
        public HashSet<Guid> ComponentsDiff { get; set; }
        public Dictionary<Guid, List<string>> AttributesDiff { get; set; }
        public Dictionary<Guid,string> ComponentsDeleted { get; set; }


        public bool hasDiff(Guid pRowKey)
        {
            return RowsDiff != null && RowsDiff.Contains(pRowKey) ;
        }
        public bool hasComponentDiff(Guid pComponentKey)
        {
            return ComponentsDiff != null && ComponentsDiff.Contains(pComponentKey);
        }
        public bool hasColDiff(Guid pColKey)
        {
            return ColsDiff != null && ColsDiff.Contains(pColKey);
        }

        public bool HasAttributeDif(Guid pRowKey, string pAttribute)
        {
            return AttributesDiff != null && AttributesDiff.ContainsKey(pRowKey) && AttributesDiff[pRowKey].Contains(pAttribute);
        }

        public bool ComponentDeleted(Guid pComponentId)
        {
            return ComponentsDeleted != null && ComponentsDeleted.ContainsKey(pComponentId);
        }
    }
    public class AdministrarPaginaCMSVersionViewModel
    {
        public AdministrarPaginaCMSVersionViewModel(Guid pestanyaID, Guid versionID, int version, bool versionActual, DateTime fechaModificacion, string comentario, string autor)
        {
            PestanyaID = pestanyaID;
            VersionID = versionID;
            Version = version;
            VersionActual = versionActual;
            FechaModificacion = fechaModificacion;
            Comentario = comentario;
            Autor = autor;
        }
        public AdministrarPaginaCMSVersionViewModel(AdministrarPaginasCMSViewModel pestanyaCMS, Guid versionID, int version, bool versionActual, DateTime fechaModificacion, string comentario, string autor)
        {
            PestanyaID = pestanyaCMS.Key;
            PestanyaCMS = pestanyaCMS;
            VersionID = versionID;
            Version = version;
            VersionActual = versionActual;
            FechaModificacion = fechaModificacion;
            Comentario = comentario;
            Autor = autor;
        }

        public Guid PestanyaID { get; set; }

        public AdministrarPaginasCMSViewModel PestanyaCMS {  get; set; } 

        public Guid VersionID { get; set; }

        public int Version { get; set; }

        public bool VersionActual { get; set; }

        public DateTime FechaModificacion { get; set; }

        public string Comentario { get; set; }

        public string Autor { get; set; }
    }
}
