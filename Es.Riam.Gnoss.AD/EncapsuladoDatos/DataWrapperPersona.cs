using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.PersonaDS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.AD.EncapsuladoDatos
{
    [Serializable]
    public class DataWrapperPersona : DataWrapperBase
    {
        public List<ConfiguracionGnossPersona> ListaConfigGnossPersona;
        public List<Persona> ListaPersona;
        public List<PersonaVinculoOrganizacion> ListaPersonaVinculoOrganizacion;
        public List<DatosTrabajoPersonaLibre> ListaDatosTrabajoPersonaLibre;
        public DataWrapperPersona()
        {
            ListaConfigGnossPersona = new List<ConfiguracionGnossPersona>();
            ListaPersona = new List<Persona>();
            ListaPersonaVinculoOrganizacion = new List<PersonaVinculoOrganizacion>();
            ListaDatosTrabajoPersonaLibre = new List<DatosTrabajoPersonaLibre>();
        }

        public override void Merge(DataWrapperBase pDataWrapper)
        {
            DataWrapperPersona dataWrapperPersona = (DataWrapperPersona)pDataWrapper;
            ListaConfigGnossPersona=this.ListaConfigGnossPersona.Union(dataWrapperPersona.ListaConfigGnossPersona).ToList();
            ListaPersona = this.ListaPersona.Union(dataWrapperPersona.ListaPersona).ToList();
            ListaPersonaVinculoOrganizacion = this.ListaPersonaVinculoOrganizacion.Union(dataWrapperPersona.ListaPersonaVinculoOrganizacion).ToList();
            ListaDatosTrabajoPersonaLibre = this.ListaDatosTrabajoPersonaLibre.Union(dataWrapperPersona.ListaDatosTrabajoPersonaLibre).ToList();
        }

        public void CargaRelacionesPerezosasCache()
        {
            foreach(ConfiguracionGnossPersona configPersona in ListaConfigGnossPersona)
            {
                configPersona.Persona = ListaPersona.FirstOrDefault(persona => persona.PersonaID.Equals(configPersona.PersonaID));
            }

            foreach(Persona persona in ListaPersona)
            {
                persona.ConfiguracionGnossPersona = ListaConfigGnossPersona.FirstOrDefault(config => config.PersonaID.Equals(persona.PersonaID));
                persona.DatosTrabajoPersonaLibre = ListaDatosTrabajoPersonaLibre.FirstOrDefault(datos => datos.PersonaID.Equals(persona.PersonaID));
            }

            foreach(DatosTrabajoPersonaLibre datosTrabajo in ListaDatosTrabajoPersonaLibre)
            {
                datosTrabajo.Persona = ListaPersona.FirstOrDefault(persona => persona.PersonaID.Equals(datosTrabajo.PersonaID));
            }
        }


    }

}
