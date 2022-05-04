using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.Elementos.Voto;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Es.Riam.Gnoss.Elementos.Comentario
{
    /// <summary>
    /// Clase para controlar los votos
    /// </summary>
    [Serializable]
    public class GestorVotosComentario : GestorVotos, ISerializable
    {
        #region Miembros

        private GestionComentarios mGestionComentarios;

        private EntityContext mEntityContext;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor vacío
        /// </summary>
        public GestorVotosComentario( LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            :base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
        }

        /// <summary>
        /// Constructor a partir de un dataset de votos y un gestor de comentarios
        /// </summary>
        /// <param name="pVotoDS">Dataset de votos</param>
        /// <param name="pGestionComentarios">Gestor de comentarios</param>
        public GestorVotosComentario(DataWrapperVoto pVotoDS, GestionComentarios pGestionComentarios,  LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication) 
            : base(pVotoDS, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mGestionComentarios = pGestionComentarios;
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected GestorVotosComentario(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
            mGestionComentarios = (GestionComentarios)info.GetValue("GestionComentarios", typeof(GestionComentarios));
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Añade un nuevo voto
        /// </summary>
        /// <param name="pVoto">Valor del voto</param>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pElementoID">Identificador del elemento</param>
        /// <param name="pIdentidadVotadaID">Identificador</param>
        /// <param name="pComentario"></param>
        /// <returns></returns>
        public AD.EntityModel.Models.Voto.Voto AgregarVoto(double pVoto, Guid pIdentidadID, Guid pElementoID, Guid pIdentidadVotadaID, Comentario pComentario)
        {
            AD.EntityModel.Models.Voto.Voto voto = base.AgregarVoto(pVoto, pIdentidadID, pElementoID, pIdentidadVotadaID, (short)TiposVotos.Comentario);

            AD.EntityModel.Models.Comentario.VotoComentario filaVotoComentario = new AD.EntityModel.Models.Comentario.VotoComentario();
            filaVotoComentario.VotoID = voto.VotoID;
            filaVotoComentario.ComentarioID = pComentario.Clave;

            GestionComentarios.ComentarioDW.ListaVotoComentario.Add(filaVotoComentario);
            mEntityContext.VotoComentario.Add(filaVotoComentario);

            if (!pComentario.ListaVotos.ContainsKey(voto.VotoID))
            {
                pComentario.ListaVotos.Add(voto.VotoID, voto);
            }
            return voto;
        }

        /// <summary>
        /// Comprueba si una entrada de blog ya ha sido votada por una identidad
        /// </summary>
        /// <param name="pComentario">Comentario</param>
        /// <param name="pIdentidad">Identidad</param>
        /// <returns>TRUE si la entrada de blog ya ha sido votada por la identidad, FALSE en caso contrario</returns>
        public bool EstaVotado(Comentario pComentario, Identidad.Identidad pIdentidad)
        {
            bool estaVotado = false;

            if (pIdentidad.ListaTodosIdentidadesDeIdentidad.Contains(pComentario.FilaComentario.IdentidadID))
            {
                return true;
            }

            foreach (AD.EntityModel.Models.Voto.Voto voto in pComentario.ListaVotos.Values)
            {
                if (pIdentidad.ListaTodosIdentidadesDeIdentidad.Contains(voto.IdentidadID))
                {
                    estaVotado = true;
                    break;
                }
            }
            return estaVotado;
        }

        /// <summary>
        /// Obtiene la media de los votos del comentario pasado por parámetro
        /// </summary>
        /// <param name="pComentario">Comentario</param>
        /// <returns></returns>
        public double ObtenerMedia(Comentario pComentario)
        {
            double total = 0;
           
            if (pComentario.ListaVotos.Count > 0)
            {
                foreach (AD.EntityModel.Models.Voto.Voto voto in pComentario.ListaVotos.Values)
                {
                    total += voto.Voto1;
                }
            }
            return total;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el gestor de comentarios
        /// </summary>
        public GestionComentarios GestionComentarios
        {
            get
            {
                return mGestionComentarios;
            }
            set
            {
                mGestionComentarios = value;
            }
        }

        #endregion

        #region Miembros de ISerializable

        /// <summary>
        /// Metodo para serializar el objeto
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("GestionComentarios", GestionComentarios);
        }

        #endregion
    }
}
