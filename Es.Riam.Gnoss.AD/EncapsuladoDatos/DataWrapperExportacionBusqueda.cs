using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.AD.EncapsuladoDatos
{
    [Serializable]
    public class DataWrapperExportacionBusqueda : DataWrapperBase
    {
        public List<ProyectoPestanyaBusquedaExportacion> ListaProyectoPestanyaBusquedaExportacion { get; set; }
        public List<ProyectoPestanyaBusquedaExportacionPropiedad> ListaProyectoPestanyaBusquedaExportacionPropiedad { get; set; }
        public List<ProyectoPestanyaBusquedaExportacionExterna> ListaProyectoPestanyaBusquedaExportacionExterna { get; set; }

        public DataWrapperExportacionBusqueda()
        {
            ListaProyectoPestanyaBusquedaExportacion = new List<ProyectoPestanyaBusquedaExportacion>();
            ListaProyectoPestanyaBusquedaExportacionExterna = new List<ProyectoPestanyaBusquedaExportacionExterna>();
            ListaProyectoPestanyaBusquedaExportacionPropiedad = new List<ProyectoPestanyaBusquedaExportacionPropiedad>();
        }

        public override void Merge(DataWrapperBase pDataWrapper)
        {
            DataWrapperExportacionBusqueda dataWrapperExportacionBusqueda = (DataWrapperExportacionBusqueda)pDataWrapper;

            ListaProyectoPestanyaBusquedaExportacionPropiedad = ListaProyectoPestanyaBusquedaExportacionPropiedad.Union(dataWrapperExportacionBusqueda.ListaProyectoPestanyaBusquedaExportacionPropiedad).ToList();
            ListaProyectoPestanyaBusquedaExportacionExterna = ListaProyectoPestanyaBusquedaExportacionExterna.Union(dataWrapperExportacionBusqueda.ListaProyectoPestanyaBusquedaExportacionExterna).ToList();
            ListaProyectoPestanyaBusquedaExportacion = ListaProyectoPestanyaBusquedaExportacion.Union(dataWrapperExportacionBusqueda.ListaProyectoPestanyaBusquedaExportacion).ToList();
        }
    }
}
