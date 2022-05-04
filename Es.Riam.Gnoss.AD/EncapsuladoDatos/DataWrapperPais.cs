using Es.Riam.Gnoss.AD.EntityModel.Models.Pais;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.AD.EncapsuladoDatos
{
    [Serializable]
    public class DataWrapperPais : DataWrapperBase
    {
        public List<Pais> ListaPais { get; set; }
        public List<Provincia> ListaProvincia { get; set; }

        public DataWrapperPais()
        {
            ListaPais = new List<Pais>();
            ListaProvincia = new List<Provincia>();
        }

        public override void Merge(DataWrapperBase pDataWrapper)
        {
            DataWrapperPais dataWrapperPais = (DataWrapperPais)pDataWrapper;

            ListaPais = ListaPais.Union(dataWrapperPais.ListaPais).ToList();
            ListaProvincia = ListaProvincia.Union(dataWrapperPais.ListaProvincia).ToList();
        }
    }
}
