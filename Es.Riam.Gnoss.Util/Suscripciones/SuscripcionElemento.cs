using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Util.Suscripciones
{
    /// <summary>
    /// Clase que representa un suscripción a un elemento.
    /// </summary>
    public class SuscripcionElemento
    {
        #region Miembros

        /// <summary>
        /// ID de la suscripción.
        /// </summary>
        private Guid mSuscripcionID;

        /// <summary>
        /// ID del elemento generado por la suscripción.
        /// </summary>
        private Guid mElementoSuscripcionID;

        /// <summary>
        /// ID del perfil del suscriptor.
        /// </summary>
        private Guid mPerfilSuscriptorID;

        /// <summary>
        /// ID de la identidad de la BR personal a la que se está suscrita.
        /// </summary>
        private Guid mIdentidadIDBRSuscripcion;

        /// <summary>
        /// ID del tesauro al que se está suscrito.
        /// </summary>
        private Guid mTesauroIDSuscripcion;

        /// <summary>
        /// Categorías suscritas a las que está vinculado el elemento.
        /// </summary>
        private Dictionary<Guid, string> mCategoriasSuscritas;

        /// <summary>
        /// Identidad a la que se está suscrito en un proyecto.
        /// </summary>
        private Guid? mIdentidadEnProyectoSuscrita;

        /// <summary>
        /// Identidad que publica el recurso en un proyecto.
        /// </summary>
        private Guid? mAutorID;

        #endregion

        #region Propiedades

        /// <summary>
        /// ID de la suscripción.
        /// </summary>
        public Guid SuscripcionID
        {
            get
            {
                return mSuscripcionID;
            }
            set
            {
                mSuscripcionID = value;
            }
        }

        /// <summary>
        /// ID del elemento generado por la suscripción.
        /// </summary>
        public Guid ElementoSuscripcionID
        {
            get
            {
                return mElementoSuscripcionID;
            }
            set
            {
                mElementoSuscripcionID = value;
            }
        }

        /// <summary>
        /// ID del perfil del suscriptor.
        /// </summary>
        public Guid PerfilSuscriptorID
        {
            get
            {
                return mPerfilSuscriptorID;
            }
            set
            {
                mPerfilSuscriptorID = value;
            }
        }

        /// <summary>
        /// ID de la identidad de la BR personal a la que se está suscrita.
        /// </summary>
        public Guid IdentidadIDBRSuscripcion
        {
            get
            {
                return mIdentidadIDBRSuscripcion;
            }
            set
            {
                mIdentidadIDBRSuscripcion = value;
            }
        }

        /// <summary>
        /// ID del tesauro al que se está suscrito.
        /// </summary>
        public Guid TesauroIDSuscripcion
        {
            get
            {
                return mTesauroIDSuscripcion;
            }
            set
            {
                mTesauroIDSuscripcion = value;
            }
        }

        /// <summary>
        /// Categorías suscritas a las que está vinculado el elemento.
        /// </summary>
        public Dictionary<Guid, string> CategoriasSuscritas
        {
            get
            {
                if (mCategoriasSuscritas == null)
                {
                    mCategoriasSuscritas = new Dictionary<Guid, string>();
                }

                return mCategoriasSuscritas;
            }
            set
            {
                mCategoriasSuscritas = value;
            }
        }

        /// <summary>
        /// Identidad a la que se está suscrito en un proyecto.
        /// </summary>
        public Guid? IdentidadEnProyectoSuscrita
        {
            get
            {
                return mIdentidadEnProyectoSuscrita;
            }
            set
            {
                mIdentidadEnProyectoSuscrita = value;
            }
        }

        /// <summary>
        /// Identidad que publica el recurso en un proyecto.
        /// </summary>
        public Guid? AutorID
        {
            get
            {
                return mAutorID;
            }
            set
            {
                mAutorID = value;
            }
        }

        #endregion
    }
}
