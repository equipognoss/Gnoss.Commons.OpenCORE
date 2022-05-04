using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("DatoExtraEcosistemaVirtuoso")]
    public partial class DatoExtraEcosistemaVirtuoso
    {
        [Key]
        public Guid DatoExtraID { get; set; }

        public int Orden { get; set; }

        [Required]
        [StringLength(500)]
        public string Titulo { get; set; }

        [Required]
        [StringLength(500)]
        public string InputID { get; set; }

        [Required]
        [StringLength(500)]
        public string InputsSuperiores { get; set; }

        [Required]
        [StringLength(500)]
        public string QueryVirtuoso { get; set; }

        [Required]
        [StringLength(500)]
        public string ConexionBD { get; set; }

        public bool Obligatorio { get; set; }

        public bool Paso1Registro { get; set; }

        public bool VisibilidadFichaPerfil { get; set; }

        [Required]
        [StringLength(500)]
        public string PredicadoRDF { get; set; }

        [Required]
        [StringLength(500)]
        public string NombreCampo { get; set; }

        [StringLength(500)]
        public string EstructuraHTMLFicha { get; set; }
    }
}
