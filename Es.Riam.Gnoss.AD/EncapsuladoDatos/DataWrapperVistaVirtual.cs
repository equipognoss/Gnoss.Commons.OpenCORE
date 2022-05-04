using Es.Riam.Gnoss.AD.EntityModel.Models.VistaVirtualDS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.AD.EncapsuladoDatos
{
    [Serializable]
    public class DataWrapperVistaVirtual : DataWrapperBase, IDisposable
    {
        public List<VistaVirtualPersonalizacion> ListaVistaVirtualPersonalizacion { get; set; }
        public List<VistaVirtualProyecto> ListaVistaVirtualProyecto { get; set; }
        public List<VistaVirtualRecursos> ListaVistaVirtualRecursos { get; set; }
        public List<VistaVirtual> ListaVistaVirtual { get; set; }
        public List<VistaVirtualCMS> ListaVistaVirtualCMS { get; set; }
        public List<VistaVirtualGadgetRecursos> ListaVistaVirtualGadgetRecursos { get; set; }

        public DataWrapperVistaVirtual()
        {
            ListaVistaVirtualPersonalizacion = new List<VistaVirtualPersonalizacion>();
            ListaVistaVirtualProyecto = new List<VistaVirtualProyecto>();
            ListaVistaVirtualRecursos = new List<VistaVirtualRecursos>();
            ListaVistaVirtual = new List<VistaVirtual>();
            ListaVistaVirtualCMS = new List<VistaVirtualCMS>();
            ListaVistaVirtualGadgetRecursos = new List<VistaVirtualGadgetRecursos>();
        }
        

        public override void Merge(DataWrapperBase pDataWrapper)
        {
            DataWrapperVistaVirtual dataWrapperVistaVirtual = (DataWrapperVistaVirtual)pDataWrapper;
            ListaVistaVirtualPersonalizacion = ListaVistaVirtualPersonalizacion.Union(dataWrapperVistaVirtual.ListaVistaVirtualPersonalizacion).ToList();
            ListaVistaVirtualProyecto = ListaVistaVirtualProyecto.Union(dataWrapperVistaVirtual.ListaVistaVirtualProyecto).ToList();
            ListaVistaVirtualRecursos = ListaVistaVirtualRecursos.Union(dataWrapperVistaVirtual.ListaVistaVirtualRecursos).ToList();
            ListaVistaVirtual = ListaVistaVirtual.Union(dataWrapperVistaVirtual.ListaVistaVirtual).ToList();
            ListaVistaVirtualCMS = ListaVistaVirtualCMS.Union(dataWrapperVistaVirtual.ListaVistaVirtualCMS).ToList();
            ListaVistaVirtualGadgetRecursos = ListaVistaVirtualGadgetRecursos.Union(dataWrapperVistaVirtual.ListaVistaVirtualGadgetRecursos).ToList();
        }

        public void CargaRelacionesPerezosasCache()
        {
            
        }

        public void Dispose()
        {
            //ListaVistaVirtualPersonalizacion.Clear();
            ListaVistaVirtualPersonalizacion = null;
            //ListaVistaVirtualProyecto.Clear();
            ListaVistaVirtualProyecto = null;
            //ListaVistaVirtualRecursos.Clear();
            ListaVistaVirtualRecursos = null;
            //ListaVistaVirtual.Clear();
            ListaVistaVirtual = null;
            //ListaVistaVirtualCMS.Clear();
            ListaVistaVirtualCMS = null;
            //ListaVistaVirtualGadgetRecursos.Clear();
            ListaVistaVirtualGadgetRecursos = null;
        }
        ~DataWrapperVistaVirtual()
        {
            Dispose();
        }
    }
}
