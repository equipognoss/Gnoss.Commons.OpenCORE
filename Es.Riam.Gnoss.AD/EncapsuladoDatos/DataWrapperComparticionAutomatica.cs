using Es.Riam.Gnoss.AD.EntityModel.Models.ComparticionAutomatica;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.AD.EncapsuladoDatos
{
    [Serializable]
    public class DataWrapperComparticionAutomatica : DataWrapperBase
    {
        public List<EntityModel.Models.ComparticionAutomatica.ComparticionAutomatica> ListaComparticionAutomatica { get; set; }
        public List<ComparticionAutomaticaMapping> ListaComparticionAutomaticaMapping { get; set; }
        public List<ComparticionAutomaticaReglas> ListaComparticionAutomaticaReglas { get; set; }

        public DataWrapperComparticionAutomatica()
        {
            ListaComparticionAutomatica = new List<EntityModel.Models.ComparticionAutomatica.ComparticionAutomatica>();
            ListaComparticionAutomaticaMapping = new List<ComparticionAutomaticaMapping>();
            ListaComparticionAutomaticaReglas = new List<ComparticionAutomaticaReglas>();
        }

        public override void Merge(DataWrapperBase pDataWrapper)
        {
            DataWrapperComparticionAutomatica dataWrapperComparticionAutomatica = (DataWrapperComparticionAutomatica)pDataWrapper;

            ListaComparticionAutomatica = ListaComparticionAutomatica.Union(dataWrapperComparticionAutomatica.ListaComparticionAutomatica).ToList();
            ListaComparticionAutomaticaMapping = ListaComparticionAutomaticaMapping.Union(dataWrapperComparticionAutomatica.ListaComparticionAutomaticaMapping).ToList();
            ListaComparticionAutomaticaReglas = ListaComparticionAutomaticaReglas.Union(dataWrapperComparticionAutomatica.ListaComparticionAutomaticaReglas).ToList();
        }
    }
}
