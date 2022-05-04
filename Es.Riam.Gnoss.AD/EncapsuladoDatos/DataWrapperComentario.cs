using Es.Riam.Gnoss.AD.EntityModel.Models.Comentario;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.AD.EncapsuladoDatos
{
    [Serializable]
    public class DataWrapperComentario : DataWrapperBase
    {
        public List<EntityModel.Models.Comentario.Comentario> ListaComentario { get; set; }
        public List<VotoComentario> ListaVotoComentario { get; set; }
        public List<ComentarioBlog> ListaComentarioBlog { get; set; }
        public List<ComentarioCuestion> ListaComentarioCuestion { get; set; }

        public DataWrapperComentario()
        {
            ListaComentario = new List<EntityModel.Models.Comentario.Comentario>();
            ListaVotoComentario = new List<VotoComentario>();
            ListaComentarioBlog = new List<ComentarioBlog>();
            ListaComentarioCuestion = new List<ComentarioCuestion>();
        }

        public override void Merge(DataWrapperBase pDataWrapper)
        {
            DataWrapperComentario dataWrapperComentario = (DataWrapperComentario)pDataWrapper;

            ListaComentario = ListaComentario.Union(dataWrapperComentario.ListaComentario).ToList();
            ListaVotoComentario = ListaVotoComentario.Union(dataWrapperComentario.ListaVotoComentario).ToList();
            ListaComentarioBlog = ListaComentarioBlog.Union(dataWrapperComentario.ListaComentarioBlog).ToList();
            ListaComentarioCuestion = ListaComentarioCuestion.Union(dataWrapperComentario.ListaComentarioCuestion).ToList();
        }
    }
}
