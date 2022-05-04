using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro
{
    [Serializable]
    [Table("CategoriaTesauro")]
    public partial class CategoriaTesauro
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CategoriaTesauro()
        {
            CategoriaTesauroSugerencia = new HashSet<CategoriaTesauroSugerencia>();
            CatTesauroAgCatTesauroInferior = new HashSet<CatTesauroAgCatTesauro>();
            CatTesauroAgCatTesauroSuperior = new HashSet<CatTesauroAgCatTesauro>();
            DocumentoWebAgCatTesauro = new HashSet<DocumentoWebAgCatTesauro>();
        }

        [Column(Order = 0)]
        public Guid TesauroID { get; set; }

        [Column(Order = 1)]
        public Guid CategoriaTesauroID { get; set; }

        [Required]
        [StringLength(1000)]
        public string Nombre { get; set; }

        public short Orden { get; set; }

        public int NumeroRecursos { get; set; }

        public int NumeroPreguntas { get; set; }

        public int NumeroDebates { get; set; }

        public int NumeroDafos { get; set; }

        public bool TieneFoto { get; set; }

        public short VersionFoto { get; set; }

        public short Estructurante { get; set; }
        [ForeignKey("TesauroID")]
        public virtual Tesauro Tesauro { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CategoriaTesauroSugerencia> CategoriaTesauroSugerencia { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CatTesauroAgCatTesauro> CatTesauroAgCatTesauroInferior { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CatTesauroAgCatTesauro> CatTesauroAgCatTesauroSuperior { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CatTesauroCompartida> CatTesauroCompartida { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CatTesauroCompartida> CatTesauroCompartida1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentoWebAgCatTesauro> DocumentoWebAgCatTesauro { get; set; }
    }
}

