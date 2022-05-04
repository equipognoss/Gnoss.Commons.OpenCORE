using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    public partial class ColaCargaRecursos
    {
        [Key]
        public int ColaID { get; set; }

        public Guid ID { get; set; }

        public Guid ProyectoID { get; set; }

        public Guid UsuarioID { get; set; }

        public DateTime Fecha { get; set; }

        public short Estado { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreFichImport { get; set; }
    }
}
