using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Models.Lucene
{
    public class AutocompletarLuceneModel
    { 
        public Guid DocumentoID { get; set; }
        public string Palabra { get; set; }
        //Tipo
        public string IdPagina { get; set; }
        //para la privacidad
        public Guid IdentidadID { get; set; }
        public string Idioma { get; set; }
        public string InfoExtra { get; set; }
        public string Faceta { get; set; }
        public bool MetaBusqueda { get; set; }
        public bool Agregar { get; set; }
        public string Imagen {  get; set; }
        public string SubtipoOntologia {  get; set; }
        public int Peso {  get; set; }
        public TipoDato TipoDato {  get; set; }
        public string UrlRecurso {  get; set; }
        public string PropiedadesExtra { get; set; }
        public double OrderAsc { get;set; }
        public double OrderDesc { get; set; }
        public string ClaveFaceta { get; set; }
        public string TextoBuscableFaceta {  get; set; }
        public string ObjetoConocimiento { get; set; }
        public string Nombrefaceta { get; set; }
    }

    public enum TipoDato
    {
        Faceta,
        TextoLibre,
        Documento
    }
}