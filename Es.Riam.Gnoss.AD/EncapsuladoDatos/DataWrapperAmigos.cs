using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EncapsuladoDatos
{
    [Serializable]
    public class DataWrapperAmigos: DataWrapperBase
    {
        public List<Amigo> ListaAmigo { get; set; }
        public List<GrupoAmigos> ListaGrupoAmigos { get; set; }
        public List<AmigoAgGrupo> ListaAmigoAgGrupo { get; set; }
        public List<PermisoAmigoOrg> ListaPermisoAmigoOrg { get; set; }
        public List<PermisoGrupoAmigoOrg> ListaPermisoGrupoAmigoOrg { get; set; }
        public DataWrapperAmigos()
        {
            ListaAmigo = new List<Amigo>();
            ListaGrupoAmigos = new List<GrupoAmigos>();
            ListaAmigoAgGrupo = new List<AmigoAgGrupo>();
            ListaPermisoAmigoOrg = new List<PermisoAmigoOrg>();
            ListaPermisoGrupoAmigoOrg = new List<PermisoGrupoAmigoOrg>();
        }

        public override void Merge(DataWrapperBase pDataWrapper)
        {
            DataWrapperAmigos pDataWrapperAmigos = (DataWrapperAmigos)pDataWrapper;
            ListaAmigo = ListaAmigo.Union(pDataWrapperAmigos.ListaAmigo).ToList();
            ListaGrupoAmigos = ListaGrupoAmigos.Union(pDataWrapperAmigos.ListaGrupoAmigos).ToList();
            ListaAmigoAgGrupo = ListaAmigoAgGrupo.Union(pDataWrapperAmigos.ListaAmigoAgGrupo).ToList();
            ListaPermisoAmigoOrg = ListaPermisoAmigoOrg.Union(pDataWrapperAmigos.ListaPermisoAmigoOrg).ToList();
            ListaPermisoGrupoAmigoOrg = ListaPermisoGrupoAmigoOrg.Union(pDataWrapperAmigos.ListaPermisoGrupoAmigoOrg).ToList();
        }
       

    }
}
