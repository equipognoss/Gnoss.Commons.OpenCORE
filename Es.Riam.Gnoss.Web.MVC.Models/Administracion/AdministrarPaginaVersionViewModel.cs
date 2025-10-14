using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AdministrarPaginaVersionViewModel
    {
        public AdministrarPaginaVersionViewModel(TabModel pestanya, Guid versionID, int version, bool versionActual, DateTime fechaModificacion, string comentario, string autor)
        {
            Pestanya = pestanya;
            VersionID = versionID;
            Version = version;
            VersionActual = versionActual;
            FechaModificacion = fechaModificacion;
            Comentario = comentario;
            Autor = autor;
        }

        public TabModel Pestanya { get; set; }

        public Guid VersionID { get; set; }

        public int Version { get; set; }

        public bool VersionActual { get; set; }

        public DateTime FechaModificacion { get; set; }

        public string Comentario { get; set; }

        public string Autor { get; set; }
    }
}
