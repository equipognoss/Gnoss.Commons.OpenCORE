using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;


namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    [Table("DetallesDocumentacion")]
    public partial class DetallesDocumentacion
    {
        [Key]
        [Column(Order = 0)]
        public Guid ProyectoID { get; set; }

        public string Licencia { get; set; }

        public bool Privado { get; set; } = false;

        public string PathImagen { get; set; }


        #region AtributosMultiidioma
        public string Titulo { get; set; }

        public  string  Autores { get; set; }

        public string OntologiasImportadas { get; set; }

        public string EnlaceOWL { get; set; }

        public string UrlLicencia { get; set; }

        public string Descripcion { get; set; }

        #endregion AtributosMultiidioma

    }
}
