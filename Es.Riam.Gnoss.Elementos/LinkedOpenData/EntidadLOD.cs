using System.Collections.Generic;

namespace Es.Riam.Gnoss.Elementos.LinkedOpenData
{
    public class EntidadLOD
    {

        #region Miembros
        
        string mNombre = "";

        string mDescripcion = "";

        string mUrl = "";

        List<string> mListaEntidadesSameAs = null;

        private List<string> mNombresAlternativos = null;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public EntidadLOD()
        {
            mListaEntidadesSameAs = new List<string>();
            mNombresAlternativos = new List<string>();
        }

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="pNombre">Nombre de la entidad</param>
        /// <param name="pUrl">Url de la entidad</param>
        public EntidadLOD(string pNombre, string pUrl)
        {
            mListaEntidadesSameAs = new List<string>();
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene el Nombre de esta entidad
        /// </summary>
        public string Nombre
        {
            get
            {
                return mNombre;
            }
            set
            {
                this.mNombre = value;
            }
        }

        /// <summary>
        /// Obtiene el Nombre de esta entidad
        /// </summary>
        public string Descripcion
        {
            get
            {
                return mDescripcion;
            }
            set
            {
                this.mDescripcion = value;
            }
        }

        /// <summary>
        /// Obtiene la URL de esta entidad
        /// </summary>
        public string Url
        {
            get
            {
                return mUrl;
            }
            set
            {
                this.mUrl = value;
            }
        }

        /// <summary>
        /// Obtiene la lista de entidades (urls) de la nube que son la misma qu esta
        /// </summary>
        public List<string> ListaEntidadesSameAs
        {
            get
            {
                return mListaEntidadesSameAs;
            }
        }

        /// <summary>
        /// Obtiene la lista de nombres alternativos para una misma entidad (Ejemplo: Don Quijote -> Don Quijote de la mancha, cervantes -> Miguel de Cervantes...)
        /// </summary>
        public List<string> NombresAlternativos
        {
            get
            {
                return mNombresAlternativos;
            }
        }

        #endregion
    }
}
