using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.MVC
{
    [Serializable]
    public class ComentarioDocumentoProyecto
    {
        public Guid ComentarioID { get; set; }
        public Guid IdentidadID { get; set; }
        public Guid DocumentoID { get; set; }
        public Guid ProyectoID { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
        public string NombreCorto { get; set; }
        public string Titulo { get; set; }
        public int? VersionFotoDocumento { get; set; }
        public short Tipo { get; set; }
        public string NombreCategoriaDoc { get; set; }
        public Guid? ElementoVinculadoID { get; set; }
        public string NombreElementoVinculado { get; set; }
        public string Enlace { get; set; }

        public override bool Equals(object obj)
        {
            ComentarioDocumentoProyecto objetoParametro = null;
            if (obj.GetType() != typeof(ComentarioDocumentoProyecto))
            {
                return false;
            }
            else if (ReferenceEquals(null, obj))
            {
                return false;
            }
            else
            {
                objetoParametro = (ComentarioDocumentoProyecto)obj;
            }

            if (ComentarioID.Equals(objetoParametro.ComentarioID) && IdentidadID.Equals(objetoParametro.IdentidadID) && DocumentoID.Equals(objetoParametro.DocumentoID) && ProyectoID.Equals(objetoParametro.ProyectoID)&& Fecha.Equals(objetoParametro.Fecha) && Descripcion.Equals(objetoParametro.Descripcion) && NombreCorto.Equals(objetoParametro.NombreCorto) && Titulo.Equals(objetoParametro.Titulo) && (VersionFotoDocumento.Equals(objetoParametro.VersionFotoDocumento) || !VersionFotoDocumento.HasValue) && Tipo.Equals(objetoParametro.Tipo) && NombreCategoriaDoc.Equals(objetoParametro.NombreCategoriaDoc) && (ElementoVinculadoID.Value.Equals(objetoParametro.ElementoVinculadoID) || !ElementoVinculadoID.HasValue) && NombreElementoVinculado.Equals(objetoParametro.NombreElementoVinculado) && Enlace.Equals(objetoParametro.Enlace))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
