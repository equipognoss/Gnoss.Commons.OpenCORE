using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.IntegracionContinua
{

    public class IntegracionModel
    {
        public string Nombre { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Estado { get; set; }
        public bool EsFusionable { get; set; }
        public List<string> TodasLasRamasRepositorio { get; set; }
    }
    public enum TipoRepositorio
    {
        BitbucketCloud,
        GitHub,
        BitbucketServer,
        GitLab
    }
    public enum UploadType
    {
        Pestanyas,
        Facetas,
        Gadgets,
        Contexto,
        ObjetosConocimiento,
        Ontologias,
        ComponentesCMS,
        PaginasCMS,
        Vistas,
        VistasCMS,
        Traducciones,
        Estilos,
        Utilidades,
        OpcionesAvanzadas,
        ObjetosMultimedia,
        EstilosCss,
        EstilosImagenes
    }
}
