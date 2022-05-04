using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Es.Riam.Semantica.OWL
{
    #region Enumeraciones

    /// <summary>
    /// Enumeración de tipos de restricción
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
        /// Máxima cardinalidad
        /// </summary>
        MaxCardinality,
        /// <summary>
        /// Mínima cardinalidad
        /// </summary>
        MinCardinality,
        /// <summary>
        /// Algunos valores son del tipo
        /// </summary>
        SomeValuesFrom
    }

    #endregion

    /// <summary>
    /// Tipo de restricción
    /// </summary>

    [Serializable]
    public class UtilTipoRestriccion
    {
        #region Métodos generales

        #region Públicos

        /// <summary>
        /// Obtiene un tipo de restricción concreto.
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
        /// Obtiene una cadena de caracteres que representa el tipo de restricción
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
    /// Representa una restricción sobre una propiedad.
    /// </summary>
    [Serializable]
    public class Restriccion: IDisposable, ISerializable
    {
        #region Miembros

        /// <summary>
        /// Propiedad sobra la que se impone la restricción.
        /// </summary>
        private string mPropiedad;

        /// <summary>
        /// Tipo de restricción.
        /// </summary>
        private TipoRestriccion mTipoRestriccion;

        /// <summary>
        /// Valor de la restricción.
        /// </summary>
        private string mValor;

        #endregion

        #region Constructores

        /// <summary>
        /// Crea una restricción a partir de una propiedad y un tipo de restricción.
        /// </summary>
        /// <param name="pPropiedad">nombre de la propiedad.</param>
        /// <param name="pTipoRestriccion">tipo de restricción.</param>
        public Restriccion(string pPropiedad, TipoRestriccion pTipoRestriccion)
        {
            if (!string.IsNullOrEmpty(pPropiedad))
            {
                this.mPropiedad = pPropiedad;
                this.mTipoRestriccion = pTipoRestriccion;
            }
            else
            {
                throw new ArgumentException("Argumento no válido.");
            }
        }

        /// <summary>
        /// Crea una restricción a partir de una propiedad, un tipo de restricción y su valor.
        /// </summary>
        /// <param name="pPropiedad">nombre de la propiedad</param>
        /// <param name="pTipoRestriccion">tipo de la restricción</param>
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
                throw new ArgumentException("Argumento no válido.");
        }

        /// <summary>
        /// Crea una restricción a partir de otra restricción.
        /// </summary>
        /// <param name="pRestriccion">restriccion que se tomará como referencia.</param>
        public Restriccion(Restriccion pRestriccion)
        {
            if (pRestriccion != null)
            {
                this.mPropiedad = pRestriccion.Propiedad;
                this.mTipoRestriccion = pRestriccion.TipoRestriccion;
                this.mValor = pRestriccion.Valor;
            }
            else
                throw new ArgumentNullException("pRestricción", "El argumento no puede ser nulo.");
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected Restriccion(SerializationInfo info, StreamingContext context)
        {
            mPropiedad = (string)info.GetValue("Propiedad", typeof(string));
            mTipoRestriccion = (TipoRestriccion)info.GetValue("TipoRestriccion", typeof(TipoRestriccion));
            mValor = (string)info.GetValue("Valor", typeof(string));
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la propiedad sobra la que se impone la restricción.
        /// </summary>
        public string Propiedad
        {
            get
            {
                return this.mPropiedad;
            }
        }

        /// <summary>
        /// Obtiene el tipo de restricción.
        /// </summary>
        public TipoRestriccion TipoRestriccion
        {
            get
            {
                return this.mTipoRestriccion;
            }
        }

        /// <summary>
        /// Obtiene o establece el valor de la restricción.
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
        /// Determina si está disposed
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
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                //Libero todos los recursos administrados que he añadido a esta clase

                mPropiedad = null;

                disposed = true;
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
            info.AddValue("Propiedad", mPropiedad);
            info.AddValue("TipoRestriccion", mTipoRestriccion);
            info.AddValue("Valor", mValor);
        }

        #endregion
    }
}
