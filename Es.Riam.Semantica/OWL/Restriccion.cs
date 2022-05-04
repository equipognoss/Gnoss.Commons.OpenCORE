using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Es.Riam.Semantica.OWL
{
    #region Enumeraciones

    /// <summary>
    /// Enumeraci�n de tipos de restricci�n
    /// </summary>
    public enum TipoRestriccion
    {
        /// <summary>
        /// Todos los valores de un tipo
        /// </summary>
        AllValuesFrom,
        /// <summary>
        /// Cardinalidad de la propiedad
        /// </summary>
        Cardinality,
        /// <summary>
        /// M�xima cardinalidad
        /// </summary>
        MaxCardinality,
        /// <summary>
        /// M�nima cardinalidad
        /// </summary>
        MinCardinality,
        /// <summary>
        /// Algunos valores son del tipo
        /// </summary>
        SomeValuesFrom
    }

    #endregion

    /// <summary>
    /// Tipo de restricci�n
    /// </summary>

    [Serializable]
    public class UtilTipoRestriccion
    {
        #region M�todos generales

        #region P�blicos

        /// <summary>
        /// Obtiene un tipo de restricci�n concreto.
        /// </summary>
        /// <param name="pTipoRestriccion">tipo de restriccion que se desea coger.</param>
        /// <returns></returns>
        public static TipoRestriccion getTipoRestriccion(string pTipoRestriccion)
        {
            TipoRestriccion tipoRestriccion = TipoRestriccion.Cardinality;
            switch (pTipoRestriccion)
            {
                case "allValuesFrom":
                    tipoRestriccion = TipoRestriccion.AllValuesFrom;
                    break;
                case "cardinality":
                    tipoRestriccion = TipoRestriccion.Cardinality;
                    break;
                case "maxCardinality":
                    tipoRestriccion = TipoRestriccion.MaxCardinality;
                    break;
                case "minCardinality":
                    tipoRestriccion = TipoRestriccion.MinCardinality;
                    break;
                case "someValuesFrom":
                    tipoRestriccion = TipoRestriccion.SomeValuesFrom;
                    break;
            }
            return tipoRestriccion;
        }

        /// <summary>
        /// Obtiene una cadena de caracteres que representa el tipo de restricci�n
        /// </summary>
        /// <param name="pTipoRestriccion"></param>
        /// <returns></returns>
        public static string TipoRestriccionToString(TipoRestriccion pTipoRestriccion)
        {
            switch (pTipoRestriccion)
            {
                case TipoRestriccion.AllValuesFrom:
                    return "AllValuesFrom";
                case TipoRestriccion.Cardinality:
                    return "Cardinality";
                case TipoRestriccion.MaxCardinality:
                    return "MaxCardinality";
                case TipoRestriccion.MinCardinality:
                    return "MinCardinality";
                case TipoRestriccion.SomeValuesFrom:
                    return "SomeValuesFrom";
                default :
                    return "";
            }
        }

        #endregion

        #endregion
    }
    
    /// <summary>
    /// Representa una restricci�n sobre una propiedad.
    /// </summary>
    [Serializable]
    public class Restriccion: IDisposable, ISerializable
    {
        #region Miembros

        /// <summary>
        /// Propiedad sobra la que se impone la restricci�n.
        /// </summary>
        private string mPropiedad;

        /// <summary>
        /// Tipo de restricci�n.
        /// </summary>
        private TipoRestriccion mTipoRestriccion;

        /// <summary>
        /// Valor de la restricci�n.
        /// </summary>
        private string mValor;

        #endregion

        #region Constructores

        /// <summary>
        /// Crea una restricci�n a partir de una propiedad y un tipo de restricci�n.
        /// </summary>
        /// <param name="pPropiedad">nombre de la propiedad.</param>
        /// <param name="pTipoRestriccion">tipo de restricci�n.</param>
        public Restriccion(string pPropiedad, TipoRestriccion pTipoRestriccion)
        {
            if (!string.IsNullOrEmpty(pPropiedad))
            {
                this.mPropiedad = pPropiedad;
                this.mTipoRestriccion = pTipoRestriccion;
            }
            else
            {
                throw new ArgumentException("Argumento no v�lido.");
            }
        }

        /// <summary>
        /// Crea una restricci�n a partir de una propiedad, un tipo de restricci�n y su valor.
        /// </summary>
        /// <param name="pPropiedad">nombre de la propiedad</param>
        /// <param name="pTipoRestriccion">tipo de la restricci�n</param>
        /// <param name="pValor">valor</param>
        public Restriccion(string pPropiedad, TipoRestriccion pTipoRestriccion,string pValor)
        {
            if (!string.IsNullOrEmpty(pPropiedad))
            {
                this.mPropiedad = pPropiedad;
                this.mTipoRestriccion = pTipoRestriccion;
                this.mValor = pValor;
            }
            else
                throw new ArgumentException("Argumento no v�lido.");
        }

        /// <summary>
        /// Crea una restricci�n a partir de otra restricci�n.
        /// </summary>
        /// <param name="pRestriccion">restriccion que se tomar� como referencia.</param>
        public Restriccion(Restriccion pRestriccion)
        {
            if (pRestriccion != null)
            {
                this.mPropiedad = pRestriccion.Propiedad;
                this.mTipoRestriccion = pRestriccion.TipoRestriccion;
                this.mValor = pRestriccion.Valor;
            }
            else
                throw new ArgumentNullException("pRestricci�n", "El argumento no puede ser nulo.");
        }

        /// <summary>
        /// Constructor para la deseralizaci�n
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serializaci�n</param>
        protected Restriccion(SerializationInfo info, StreamingContext context)
        {
            mPropiedad = (string)info.GetValue("Propiedad", typeof(string));
            mTipoRestriccion = (TipoRestriccion)info.GetValue("TipoRestriccion", typeof(TipoRestriccion));
            mValor = (string)info.GetValue("Valor", typeof(string));
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la propiedad sobra la que se impone la restricci�n.
        /// </summary>
        public string Propiedad
        {
            get
            {
                return this.mPropiedad;
            }
        }

        /// <summary>
        /// Obtiene el tipo de restricci�n.
        /// </summary>
        public TipoRestriccion TipoRestriccion
        {
            get
            {
                return this.mTipoRestriccion;
            }
        }

        /// <summary>
        /// Obtiene o establece el valor de la restricci�n.
        /// </summary>
        public string Valor
        {
            set
            {
                this.mValor = value;
            }
            get
            {
                return this.mValor;
            }
        }

        #endregion

        #region Dispose


        /// <summary>
        /// Determina si est� disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~Restriccion()
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
        /// <param name="disposing">Determina si se est� llamando desde el Dispose()</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                //Libero todos los recursos administrados que he a�adido a esta clase

                mPropiedad = null;

                disposed = true;
            }
        }

        #endregion

        #region Miembros de ISerializable

        /// <summary>
        /// M�todo para serializar el objeto
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serializaci�n</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Propiedad", mPropiedad);
            info.AddValue("TipoRestriccion", mTipoRestriccion);
            info.AddValue("Valor", mValor);
        }

        #endregion
    }
}
