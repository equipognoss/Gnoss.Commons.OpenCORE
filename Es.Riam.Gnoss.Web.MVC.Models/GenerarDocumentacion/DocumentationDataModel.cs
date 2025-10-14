using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.GenerarDocumentacion
{
    public class DocumentationDataModel
    {
        #region Miembros
        #region publicos
        public string CommunityURL { get; set; }
        public string communityShortName { get; set; }
        public string ProyectId { get; set; }
        public string Description { get; set; }

        #endregion publicos
        #region privados
        #endregion privados
        #endregion Miembros
        public DocumentationDataModel() { }

    }
}
