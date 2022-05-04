using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.ViewModels
{
    /// <summary>
    /// View Model de la página de Administrar el XML del CMS
    /// </summary>
    [Serializable]
    public class ManageXMLViewModel
    {
        /// <summary>
        /// Lista de XML
        /// </summary>
        public SortedDictionary<DateTime, string> ListXMLVersion { get; set; }


        public string UrlActionFile { get; set; }
        public string UrlActionHistory { get; set; }
        public string UrlActionDownloadDynamicFile { get; set; }
        public string UrlActionDownloadFile { get; set; }
        public string OKMessage { get; set; }
        public string KOMessage { get; set; }
    }

    public class HistoryFile
    {
        public DateTime Fecha { get; set; }
        public string Name { get; set; }
        public string UrlDownload { get; set; }
    }
}
