using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    [NotMapped]
    public class ObtenerRecursoMVC
    {
        public Guid DocumentoID { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public short Tipo { get; set; }
        public string Enlace { get; set; }
        public string Tags { get; set; }
        public int? VersionFotoDocumento { get; set; }
        public string NombreCategoriaDoc { get; set; }
        public Guid? ElementoVinculadoID { get; set; }
        public bool CompartirPermitido { get; set; }
        public bool Borrador { get; set; }
        public bool UltimaVersion { get; set; }
        public Guid? ProyectoID { get; set; }
        public string NombreElementoVinculado { get; set; }
        public Guid? IdentidadPublicacionID { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public short TipoPublicacion { get; set; }
        public bool PrivadoEditores { get; set; }
        public int NumeroComentarios { get; set; }
        public int NumeroVotos { get; set; }
        public int NumeroDescargas { get; set; }
        public int NumeroConsultas { get; set; }
        public string DocOntolologiaEnlace { get; set; }

        public override bool Equals(object obj)
        {
            ObtenerRecursoMVC objetoParametro = null;
            if (obj.GetType() != typeof(ObtenerRecursoMVC))
            {
                return false;
            }
            else if (ReferenceEquals(null, obj))
            {
                return false;
            }
            else
            {
                objetoParametro = (ObtenerRecursoMVC)obj;
            }

            if (ProyectoID.Equals(objetoParametro.ProyectoID) && DocumentoID.Equals(objetoParametro.DocumentoID) && Titulo.Equals(objetoParametro.Titulo) && (NombreElementoVinculado.Equals(objetoParametro.NombreElementoVinculado) || NombreElementoVinculado.Equals("")) && Descripcion.Equals(objetoParametro.Descripcion) && Enlace.Equals(objetoParametro.Enlace) && Tags.Equals(objetoParametro.Tags) && (VersionFotoDocumento.Equals(objetoParametro.VersionFotoDocumento) || !objetoParametro.VersionFotoDocumento.HasValue) && (NombreCategoriaDoc.Equals(objetoParametro.NombreCategoriaDoc) || NombreCategoriaDoc.Equals("")) && Tags.Equals(objetoParametro.Tags) && ElementoVinculadoID.Equals(objetoParametro.ElementoVinculadoID) && CompartirPermitido.Equals(objetoParametro.CompartirPermitido) && Borrador.Equals(objetoParametro.Borrador) && UltimaVersion.Equals(objetoParametro.UltimaVersion) && IdentidadPublicacionID.Equals(objetoParametro.IdentidadPublicacionID) && FechaPublicacion.Equals(objetoParametro.FechaPublicacion) && TipoPublicacion.Equals(objetoParametro.TipoPublicacion) && PrivadoEditores.Equals(objetoParametro.PrivadoEditores) && NumeroComentarios.Equals(objetoParametro.NumeroComentarios) && NumeroVotos.Equals(objetoParametro.NumeroVotos) && NumeroDescargas.Equals(objetoParametro.NumeroDescargas) && NumeroConsultas.Equals(objetoParametro.NumeroConsultas) && DocOntolologiaEnlace.Equals(objetoParametro.DocOntolologiaEnlace))
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
