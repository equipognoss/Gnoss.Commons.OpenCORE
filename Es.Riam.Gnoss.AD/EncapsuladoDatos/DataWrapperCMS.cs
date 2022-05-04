using Es.Riam.Gnoss.AD.EntityModel.Models.CMS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.AD.EncapsuladoDatos
{
    [Serializable]
    public class DataWrapperCMS : DataWrapperBase
    {
        public List<CMSBloque> ListaCMSBloque;
        public List<CMSBloqueComponente> ListaCMSBloqueComponente;
        public List<CMSBloqueComponentePropiedadComponente> ListaCMSBloqueComponentePropiedadComponente;
        public List<CMSComponente> ListaCMSComponente;
        public List<CMSComponentePrivadoProyecto> ListaCMSComponentePrivadoProyecto;
        public List<CMSComponenteRolGrupoIdentidades> ListaCMSComponenteRolGrupoIdentidades;
        public List<CMSComponenteRolIdentidad> ListaCMSComponenteRolIdentidad;
        public List<CMSPagina> ListaCMSPagina;
        public List<CMSPropiedadComponente> ListaCMSPropiedadComponente;
        public List<CMSRolGrupoIdentidades> ListaCMSRolGrupoIdentidades;
        public List<CMSRolIdentidad> ListaCMSRolIdentidad;

        public DataWrapperCMS()
        {
            ListaCMSBloque = new List<CMSBloque>();
            ListaCMSBloqueComponente = new List<CMSBloqueComponente>();
            ListaCMSBloqueComponentePropiedadComponente = new List<CMSBloqueComponentePropiedadComponente>();
            ListaCMSComponente = new List<CMSComponente>();
            ListaCMSComponentePrivadoProyecto = new List<CMSComponentePrivadoProyecto>();
            ListaCMSComponenteRolGrupoIdentidades = new List<CMSComponenteRolGrupoIdentidades>();
            ListaCMSComponenteRolIdentidad = new List<CMSComponenteRolIdentidad>();
            ListaCMSPagina = new List<CMSPagina>();
            ListaCMSPropiedadComponente = new List<CMSPropiedadComponente>();
            ListaCMSRolGrupoIdentidades = new List<CMSRolGrupoIdentidades>();
            ListaCMSRolIdentidad = new List<CMSRolIdentidad>();
        }

        public override void Merge(DataWrapperBase pDataWrapper)
        {
            DataWrapperCMS dataWrapperCMS = (DataWrapperCMS)pDataWrapper;

            this.ListaCMSBloque = this.ListaCMSBloque.Union(dataWrapperCMS.ListaCMSBloque).ToList();
            this.ListaCMSBloqueComponente = this.ListaCMSBloqueComponente.Union(dataWrapperCMS.ListaCMSBloqueComponente).ToList();
            this.ListaCMSBloqueComponentePropiedadComponente = this.ListaCMSBloqueComponentePropiedadComponente.Union(dataWrapperCMS.ListaCMSBloqueComponentePropiedadComponente).ToList();
            this.ListaCMSComponente = this.ListaCMSComponente.Union(dataWrapperCMS.ListaCMSComponente).ToList();
            this.ListaCMSComponentePrivadoProyecto = this.ListaCMSComponentePrivadoProyecto.Union(dataWrapperCMS.ListaCMSComponentePrivadoProyecto).ToList();
            this.ListaCMSComponenteRolGrupoIdentidades = this.ListaCMSComponenteRolGrupoIdentidades.Union(dataWrapperCMS.ListaCMSComponenteRolGrupoIdentidades).ToList();
            this.ListaCMSComponenteRolIdentidad = this.ListaCMSComponenteRolIdentidad.Union(dataWrapperCMS.ListaCMSComponenteRolIdentidad).ToList();
            this.ListaCMSPagina = this.ListaCMSPagina.Union(dataWrapperCMS.ListaCMSPagina).ToList();
            this.ListaCMSPropiedadComponente = this.ListaCMSPropiedadComponente.Union(dataWrapperCMS.ListaCMSPropiedadComponente).ToList();
            this.ListaCMSRolGrupoIdentidades = this.ListaCMSRolGrupoIdentidades.Union(dataWrapperCMS.ListaCMSRolGrupoIdentidades).ToList();
            this.ListaCMSRolIdentidad = this.ListaCMSRolIdentidad.Union(dataWrapperCMS.ListaCMSRolIdentidad).ToList();
        }
    }
}
