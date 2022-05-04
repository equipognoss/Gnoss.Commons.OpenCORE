using System;

namespace Es.Riam.Gnoss.AD.EncapsuladoDatos
{
    [Serializable]
    public abstract class DataWrapperBase : IDisposable
    {
        public abstract void Merge(DataWrapperBase pDataWrapper);

        #region Dispose

        /// <summary>
        /// Destructor
        /// </summary>
        ~DataWrapperBase()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            //impido que se finalice dos veces este objeto
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected virtual void Dispose(bool disposing)
        {
            
        }

        #endregion
    }
}
