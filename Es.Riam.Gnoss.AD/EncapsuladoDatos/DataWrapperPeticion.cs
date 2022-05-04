using Es.Riam.Gnoss.AD.EntityModel.Models.Peticion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.AD.EncapsuladoDatos
{
    [Serializable]
    public class DataWrapperPeticion : DataWrapperBase
    {
        public List<EntityModel.Models.Peticion.Peticion> ListaPeticion { get; set; }
        public List<PeticionInvitacionComunidad> ListaPeticionInvitacionComunidad { get; set; }
        public List<PeticionInvitaContacto> ListaPeticionInvitacionContacto { get; set; }
        public List<PeticionInvitacionGrupo> ListaPeticionInvitacionGrupo { get; set; }
        public List<PeticionNuevoProyecto> ListaPeticionNuevoProyecto { get; set; }
        public List<PeticionOrgInvitaPers> ListaPeticionOrgInvitaPers { get; set; }

        public DataWrapperPeticion()
        {
            ListaPeticion = new List<EntityModel.Models.Peticion.Peticion>();
            ListaPeticionInvitacionComunidad = new List<PeticionInvitacionComunidad>();
            ListaPeticionInvitacionContacto = new List<PeticionInvitaContacto>();
            ListaPeticionInvitacionGrupo = new List<PeticionInvitacionGrupo>();
            ListaPeticionOrgInvitaPers = new List<PeticionOrgInvitaPers>();
            ListaPeticionNuevoProyecto = new List<PeticionNuevoProyecto>();
        }
        
        public override void Merge(DataWrapperBase pDataWrapper)
        {
            DataWrapperPeticion dataWrapperPeticion = (DataWrapperPeticion)pDataWrapper;

            ListaPeticion = ListaPeticion.Union(dataWrapperPeticion.ListaPeticion).ToList();
            ListaPeticionInvitacionComunidad = ListaPeticionInvitacionComunidad.Union(dataWrapperPeticion.ListaPeticionInvitacionComunidad).ToList();
            ListaPeticionInvitacionContacto = ListaPeticionInvitacionContacto.Union(dataWrapperPeticion.ListaPeticionInvitacionContacto).ToList();
            ListaPeticionInvitacionGrupo = ListaPeticionInvitacionGrupo.Union(dataWrapperPeticion.ListaPeticionInvitacionGrupo).ToList();
            ListaPeticionOrgInvitaPers = ListaPeticionOrgInvitaPers.Union(dataWrapperPeticion.ListaPeticionOrgInvitaPers).ToList();
            ListaPeticionNuevoProyecto = ListaPeticionNuevoProyecto.Union(dataWrapperPeticion.ListaPeticionNuevoProyecto).ToList();
        }
    }
}
