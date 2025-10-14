using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Models.Lucene
{
    public class ModelAutocomplete
    {
        public List<AutocompletarLuceneModel> ModelDocumentAutocomplete { get; set; }
        public Guid ProyectoId { get; set; }
    }
}