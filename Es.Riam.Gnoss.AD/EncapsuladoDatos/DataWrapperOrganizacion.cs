using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;
using System;
using System.Collections.Generic;
using System.Linq;
using OrganizacionEmpresa = Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS.OrganizacionEmpresa;

namespace Es.Riam.Gnoss.AD.EncapsuladoDatos
{
    [Serializable]
    public class DataWrapperOrganizacion : DataWrapperBase

    {
        public List<Organizacion> ListaOrganizacion { get; set; }
        public List<OrganizacionParticipaProy> ListaOrganizacionParticipaProy { get; set; }
        public List<PersonaVinculoOrganizacion> ListaPersonaVinculoOrganizacion { get; set; }
        public List<AdministradorOrganizacion> ListaAdministradorOrganizacion { get; set; }
        public List<OrganizacionGnoss> ListaOrganizacionGnoss { get; set; }
        public List<ConfiguracionGnossOrg> ListaConfiguracionGnossOrg { get; set; }
        public List<PersonaVisibleEnOrg> ListaPersonaVisibleEnOrg { get; set; }
        public List<EntityModel.Models.OrganizacionDS.OrganizacionEmpresa> ListaOrganizacionEmpresa { get; set; }


        public override void Merge(DataWrapperBase pDataWrapper)
        {
        DataWrapperOrganizacion dataWrapperOrganizacion = (DataWrapperOrganizacion)pDataWrapper;
            
            ListaOrganizacion = ListaOrganizacion.Union(dataWrapperOrganizacion.ListaOrganizacion).ToList();
            ListaOrganizacionParticipaProy = ListaOrganizacionParticipaProy.Union(dataWrapperOrganizacion.ListaOrganizacionParticipaProy).ToList();
            ListaPersonaVinculoOrganizacion = ListaPersonaVinculoOrganizacion.Union(dataWrapperOrganizacion.ListaPersonaVinculoOrganizacion).ToList();
            ListaAdministradorOrganizacion = ListaAdministradorOrganizacion.Union(dataWrapperOrganizacion.ListaAdministradorOrganizacion).ToList();
            ListaOrganizacionGnoss = ListaOrganizacionGnoss.Union(dataWrapperOrganizacion.ListaOrganizacionGnoss).ToList();
            ListaConfiguracionGnossOrg = ListaConfiguracionGnossOrg.Union(dataWrapperOrganizacion.ListaConfiguracionGnossOrg).ToList();
            ListaPersonaVisibleEnOrg = ListaPersonaVisibleEnOrg.Union(dataWrapperOrganizacion.ListaPersonaVisibleEnOrg).ToList();
            ListaOrganizacionEmpresa = ListaOrganizacionEmpresa.Union(dataWrapperOrganizacion.ListaOrganizacionEmpresa).ToList();
            //this.organizacionDS.Merge(dataWrapperOrganizacion.organizacionDS);
        }

        public DataWrapperOrganizacion()
        {
            //organizacionDS = new EntityModel.Models.OrganizacionDS();
            ListaOrganizacion = new List<Organizacion>();
            ListaOrganizacionParticipaProy = new List<OrganizacionParticipaProy>();
            ListaPersonaVinculoOrganizacion = new List<PersonaVinculoOrganizacion>();
            ListaAdministradorOrganizacion = new List<AdministradorOrganizacion>();
            ListaOrganizacionGnoss = new List<OrganizacionGnoss>();
            ListaConfiguracionGnossOrg = new List<ConfiguracionGnossOrg>();
            ListaPersonaVisibleEnOrg = new List<PersonaVisibleEnOrg>();
            ListaOrganizacionEmpresa = new List<EntityModel.Models.OrganizacionDS.OrganizacionEmpresa>();
        }

        public void LlenarEntidadesCache()
        {
            foreach(Organizacion organizacion in ListaOrganizacion)
            {
                OrganizacionEmpresa organizacionEmpresa = ListaOrganizacionEmpresa.FirstOrDefault(orgEmpresa => orgEmpresa.OrganizacionID.Equals(organizacion.OrganizacionID));
                if(organizacionEmpresa != null)
                {
                    organizacion.OrganizacionEmpresa = organizacionEmpresa;
                    organizacionEmpresa.Organizacion = organizacion;
                }
                else
                {
                    OrganizacionGnoss organizacionGnoss = ListaOrganizacionGnoss.FirstOrDefault(orgEmpresa => orgEmpresa.OrganizacionID.Equals(organizacion.OrganizacionID));
                    if (organizacionGnoss != null)
                    {
                        organizacion.OrganizacionGnoss = organizacionGnoss;
                        organizacionGnoss.Organizacion = organizacion;
                    }
                        
                }
               
            }
        }
    }
}
