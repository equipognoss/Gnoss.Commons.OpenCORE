using Es.Riam.Gnoss.AD.EntityModel.Models.Solicitud;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.AD.EncapsuladoDatos
{
    [Serializable]
    public class DataWrapperSolicitud : DataWrapperBase
    {
        public List<DatoExtraEcosistemaOpcionSolicitud> ListaDatoExtraEcosistemaOpcionSolicitud { get; set; }
        public List<DatoExtraEcosistemaVirtuosoSolicitud> ListaDatoExtraEcosistemaVirtuosoSolicitud { get; set; }
        public List<DatoExtraProyectoOpcionSolicitud> ListaDatoExtraProyectoOpcionSolicitud { get; set; }
        public List<Solicitud> ListaSolicitud { get; set; }
        public List<SolicitudGrupo> ListaSolicitudGrupo { get; set; }
        public List<SolicitudNuevaOrganizacion> ListaSolicitudNuevaOrganizacion { get; set; }
        public List<SolicitudNuevaOrgEmp> ListaSolicitudNuevaOrgEmp { get; set; }
        public List<SolicitudNuevoUsuario> ListaSolicitudNuevoUsuario { get; set; }
        public List<SolicitudOrganizacion> ListaSolicitudOrganizacion { get; set; }
        public List<SolicitudUsuario> ListaSolicitudUsuario { get; set; }
        public List<DatoExtraProyectoVirtuosoSolicitud> ListaDatoExtraProyectoVirtuosoSolicitud { get; set; }
        public List<SolicitudNuevoProfesor> ListaSolicitudNuevoProfesor { get; set; }

        public DataWrapperSolicitud()
        {
            ListaDatoExtraEcosistemaOpcionSolicitud = new List<DatoExtraEcosistemaOpcionSolicitud>();
            ListaDatoExtraEcosistemaVirtuosoSolicitud = new List<DatoExtraEcosistemaVirtuosoSolicitud>();
            ListaDatoExtraProyectoOpcionSolicitud = new List<DatoExtraProyectoOpcionSolicitud>();
            ListaSolicitud = new List<Solicitud>();
            ListaSolicitudGrupo = new List<SolicitudGrupo>();
            ListaSolicitudNuevaOrganizacion = new List<SolicitudNuevaOrganizacion>();
            ListaSolicitudNuevaOrgEmp = new List<SolicitudNuevaOrgEmp>();
            ListaSolicitudNuevoUsuario = new List<SolicitudNuevoUsuario>();
            ListaSolicitudOrganizacion = new List<SolicitudOrganizacion>();
            ListaSolicitudUsuario = new List<SolicitudUsuario>();
            ListaDatoExtraProyectoVirtuosoSolicitud = new List<DatoExtraProyectoVirtuosoSolicitud>();
            ListaSolicitudNuevoProfesor = new List<SolicitudNuevoProfesor>();
        }

        public override void Merge(DataWrapperBase pDataWrapper)
        {
            DataWrapperSolicitud dataWrapperSolicitud = (DataWrapperSolicitud)pDataWrapper;

            ListaDatoExtraEcosistemaOpcionSolicitud = ListaDatoExtraEcosistemaOpcionSolicitud.Union(dataWrapperSolicitud.ListaDatoExtraEcosistemaOpcionSolicitud).ToList();
            ListaDatoExtraEcosistemaVirtuosoSolicitud = ListaDatoExtraEcosistemaVirtuosoSolicitud.Union(dataWrapperSolicitud.ListaDatoExtraEcosistemaVirtuosoSolicitud).ToList();
            ListaDatoExtraProyectoOpcionSolicitud = ListaDatoExtraProyectoOpcionSolicitud.Union(dataWrapperSolicitud.ListaDatoExtraProyectoOpcionSolicitud).ToList();
            ListaSolicitud = ListaSolicitud.Union(dataWrapperSolicitud.ListaSolicitud).ToList();
            ListaSolicitudGrupo = ListaSolicitudGrupo.Union(dataWrapperSolicitud.ListaSolicitudGrupo).ToList();
            ListaSolicitudNuevaOrganizacion = ListaSolicitudNuevaOrganizacion.Union(dataWrapperSolicitud.ListaSolicitudNuevaOrganizacion).ToList();
            ListaSolicitudNuevaOrgEmp = ListaSolicitudNuevaOrgEmp.Union(dataWrapperSolicitud.ListaSolicitudNuevaOrgEmp).ToList();
            ListaSolicitudNuevoUsuario = ListaSolicitudNuevoUsuario.Union(dataWrapperSolicitud.ListaSolicitudNuevoUsuario).ToList();
            ListaSolicitudOrganizacion = ListaSolicitudOrganizacion.Union(dataWrapperSolicitud.ListaSolicitudOrganizacion).ToList();
            ListaSolicitudUsuario = ListaSolicitudUsuario.Union(dataWrapperSolicitud.ListaSolicitudUsuario).ToList();
            ListaDatoExtraProyectoVirtuosoSolicitud = ListaDatoExtraProyectoVirtuosoSolicitud.Union(dataWrapperSolicitud.ListaDatoExtraProyectoVirtuosoSolicitud).ToList();
        }
    }
}
