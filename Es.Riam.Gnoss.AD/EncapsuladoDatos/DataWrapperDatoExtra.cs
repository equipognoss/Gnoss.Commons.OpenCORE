using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.AD.EncapsuladoDatos
{
    [Serializable]
    public class DataWrapperDatoExtra : DataWrapperBase
    {
        public List<Triples> ListaTriples { get; set; }
        public List<TripletasDatosExtraVirtuoso> ListaTripletasDatosExtraVirtuoso { get; set; }

        public DataWrapperDatoExtra()
        {
            ListaTriples = new List<Triples>();
            ListaTripletasDatosExtraVirtuoso = new List<TripletasDatosExtraVirtuoso>();
        }

        public override void Merge(DataWrapperBase pDataWrapper)
        {
            DataWrapperDatoExtra dataWrapperDatoExtra = (DataWrapperDatoExtra)pDataWrapper;

            ListaTriples = ListaTriples.Union(dataWrapperDatoExtra.ListaTriples).ToList();
            ListaTripletasDatosExtraVirtuoso = ListaTripletasDatosExtraVirtuoso.Union(dataWrapperDatoExtra.ListaTripletasDatosExtraVirtuoso).ToList();
        }
    }
}
