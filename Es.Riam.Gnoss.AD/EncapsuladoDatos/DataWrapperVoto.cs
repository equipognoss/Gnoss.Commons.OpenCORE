using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.AD.EncapsuladoDatos
{
    [Serializable]
    public class DataWrapperVoto : DataWrapperBase
    {
        public List<EntityModel.Models.Voto.Voto> ListaVotos { get; set; }

        public DataWrapperVoto()
        {
            ListaVotos = new List<EntityModel.Models.Voto.Voto>();
        }

        public override void Merge(DataWrapperBase pDataWrapper)
        {
            DataWrapperVoto dataWrapperVoto = (DataWrapperVoto)pDataWrapper;
            ListaVotos = ListaVotos.Union(dataWrapperVoto.ListaVotos).ToList();
        }
    }
}
