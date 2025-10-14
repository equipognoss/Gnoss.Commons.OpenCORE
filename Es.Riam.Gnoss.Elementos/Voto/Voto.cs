using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using System;

namespace Es.Riam.Gnoss.Elementos.Voto
{
    /// <summary>
    /// Clase de voto
    /// </summary>
    public class Voto : ElementoGnoss
    {
        #region Miembros

        /// <summary>
        /// Gestor de votos
        /// </summary>
        private GestorVotos mGestionVoto;

        private AD.EntityModel.Models.Voto.Voto mFilaVoto;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary> 
        public Voto()
            : base()
        {
        }

        /// <summary>
        /// Constructor a partir de una fila de voto y del gestor de votos
        /// </summary>
        /// <param name="pFilaVoto">Fila de voto</param>
        /// <param name="pGestionVoto">Gestor de votos</param>
        public Voto(AD.EntityModel.Models.Voto.Voto pFilaVoto, GestorVotos pGestionVoto)
             : base()
        {
            mFilaVoto = pFilaVoto;
            mGestionVoto = pGestionVoto;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el gestor de votos
        /// </summary>
        public GestorVotos GestionVoto
        {
            get
            {
                return mGestionVoto;
            }
            set
            {
                mGestionVoto = value;
            }
        }

        /// <summary>
        /// Obtiene o establece la fila del voto
        /// </summary>
        public AD.EntityModel.Models.Voto.Voto FilaVoto
        {
            get
            {
                return mFilaVoto;
            }
            set
            {
                mFilaVoto = value;
            }
        }

        /// <summary>
        /// Obtiene el identificador del voto
        /// </summary>
        public override Guid Clave
        {
            get
            {
                return mFilaVoto.VotoID;
            }
        }

        /// <summary>
        /// Obtiene o establece el identificador de la identidad que ha emitido el voto
        /// </summary>
        public Guid IdentidadID
        {
            get
            {
                return mFilaVoto.IdentidadID;
            }
            set
            {
                mFilaVoto.IdentidadID = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el identificador de la identidad que ha recibido el voto
        /// </summary>
        public Guid IdentidadVotadaID
        {
            get
            {
                return mFilaVoto.IdentidadVotadaID;
            }
            set
            {
                mFilaVoto.IdentidadVotadaID = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el identificador del elemento que ha sido votado
        /// </summary>
        public Guid ElementoID
        {
            get
            {
                return mFilaVoto.ElementoID;
            }
            set
            {
                mFilaVoto.ElementoID = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el valor del voto
        /// </summary>
        public double Valor
        {
            get
            {
                return mFilaVoto.Voto1;
            }
            set
            {
                mFilaVoto.Voto1 = value;
            }
        }

        #endregion
    }
}
