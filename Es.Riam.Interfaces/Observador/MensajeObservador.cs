namespace Es.Riam.Interfaces.Observador
{
    /// <summary>
    /// Mensaje para un observador
    /// </summary>
    public class MensajeObservador
    {
        #region Miembros

        AccionesObservador mAccion;
        ISujeto mSujeto;
        ISujeto mEmisor;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor de mensaje para observador a partir de la acción y del sujeto
        /// </summary>
        /// <param name="pAccion">Acción que se comunica al observador</param>
        /// <param name="pSujeto">Sujeto de la acción</param>
        public MensajeObservador(AccionesObservador pAccion, ISujeto pSujeto)
        {
            mAccion = pAccion;
            mSujeto = pSujeto;
        }

        /// <summary>
        /// Constructor de mensaje para observador a partir de la acción, el sujeto y el emisor
        /// </summary>
        /// <param name="pAccion">Acción que se comunica al observador</param>
        /// <param name="pSujeto">Sujeto de la acción</param>
        /// <param name="pEmisor">Emisor del mensaje</param>
        public MensajeObservador(AccionesObservador pAccion, ISujeto pSujeto, ISujeto pEmisor)
        {
            mAccion = pAccion;
            mEmisor = pEmisor;
            mSujeto = pSujeto;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Establece o devuelve la acción que se avisa en el mensaje
        /// </summary>
        public AccionesObservador Accion
        {
            get
            {
                return mAccion;
            }
            set
            {
                mAccion = value;
            }
        }

        /// <summary>
        /// Establece o devuelve el sujeto de la acción en el mensaje
        /// </summary>
        public ISujeto Sujeto
        {
            get
            {
                return mSujeto;
            }
            set
            {
                mSujeto = value;
            }
        }

        /// <summary>
        /// Establece o devuelve el emisor de la acción en el mensaje
        /// </summary>
        public ISujeto Emisor
        {
            get
            {
                return mEmisor;
            }
            set
            {
                mEmisor = value;
            }
        }

        #endregion
    }
}
