using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Es.Riam.Gnoss.Web.Controles.GeneradorPlantillasOWL
{
    /// <summary>
    /// Representa una clase CSS.
    /// </summary>
    [Serializable]
    public class ClaseCSS : ISerializable
    {
        #region Miembros

        /// <summary>
        /// Nombre de la clase.
        /// </summary>
        private string mNombre;

        /// <summary>
        /// Atributos de la clase.
        /// </summary>
        private Dictionary<string, string> mAtributos; 

        #endregion

        #region Constructor

        public ClaseCSS(string pNombre, Dictionary<string, string> pAtributos)
        {
            mNombre = pNombre;
            mAtributos = pAtributos;
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected ClaseCSS(SerializationInfo info, StreamingContext context)
        {
            mNombre = (string)info.GetValue("Nombre", typeof(string));
            mAtributos = (Dictionary<string, string>)info.GetValue("Atributos", typeof(Dictionary<string, string>));
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Devuelve o establece el nombre de la clase.
        /// </summary>
        public string Nombre
        {
            get
            {
                return mNombre;
            }
            set
            {
                mNombre = value;
            }
        }

        /// <summary>
        /// Devuelve o establece los atributos de la clase.
        /// </summary>
        public Dictionary<string, string> Atributos
        {
            get
            {
                if (mAtributos == null)
                {
                    mAtributos = new Dictionary<string, string>();
                }
                return mAtributos;
            }
            set
            {
                mAtributos = value;
            }
        }

        #endregion

        #region Miembros de ISerializable

        /// <summary>
        /// Método para serializar el objeto
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Nombre", mNombre);
            info.AddValue("Atributos", mAtributos);
        }

        #endregion
    }
}
