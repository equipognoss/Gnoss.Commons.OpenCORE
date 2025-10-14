using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using System;

namespace Es.Riam.Gnoss.Elementos.Documentacion.FichaDocumento
{
    /// <summary>
    /// Representa un atributo bibliográfico.
    /// </summary>
    public class AtributoBibliografico : ElementoGnoss
    {
        #region Miembros

        /// <summary>
        /// Identificador de la ficha bibliográfica al que pertenece el atributo.
        /// </summary>
        private Guid mFichaBibliograficaID;

        /// <summary>
        /// Identificador del atributo.
        /// </summary>
        private Guid mAtributoID;

        /// <summary>
        /// Nombre del atributo.
        /// </summary>
        private string mNombre;

        /// <summary>
        /// Valor del atributo.
        /// </summary>
        private string mValor;

        /// <summary>
        /// Descripción del atributo.
        /// </summary>
        private string mDescripcion;

        /// <summary>
        /// Tipo del atributo.
        /// </summary>
        private TipoAtributosCampos mTipo;

        /// <summary>
        /// Orden del atributo.
        /// </summary>
        private int mOrden;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor de atributo bibliográfico a partir de los datos pasados por parámetro
        /// </summary>
        /// <param name="pFichaBibliograficaID">Identificador de ficha bibliográfica</param>
        /// <param name="pAtributoID">Identificador de atributo</param>
        /// <param name="pNombre">Nombre</param>
        /// <param name="pDescripcion">Descripción</param>
        /// <param name="pTipo">Tipo de atributo</param>
        /// <param name="pOrden">Orden</param>
        public AtributoBibliografico(Guid pFichaBibliograficaID, Guid pAtributoID, string pNombre, string pDescripcion, int pTipo, int pOrden)
            : base()
        {
            mFichaBibliograficaID = pFichaBibliograficaID;
            mAtributoID = pAtributoID;
            mNombre = pNombre;
            mDescripcion = pDescripcion;
            mTipo = (TipoAtributosCampos)pTipo;
            mOrden = pOrden;
        }

        /// <summary>
        /// Constructor de atributo bibliográfico a partir de los datos pasados por parámetro
        /// </summary>
        /// <param name="pFichaBibliograficaID">Identificador de ficha bibliográfica</param>
        /// <param name="pAtributoID">Identificador de atributo</param>
        /// <param name="pNombre">Nombre</param>
        /// <param name="pDescripcion">Descripción</param>
        /// <param name="pTipo">Tipo de atributo</param>
        /// <param name="pOrden">Orden</param>
        /// <param name="pValor">Valor del atributo</param>
        public AtributoBibliografico(Guid pFichaBibliograficaID, Guid pAtributoID, string pNombre, string pDescripcion, int pTipo, int pOrden, string pValor)
            : base()
        {
            mFichaBibliograficaID = pFichaBibliograficaID;
            mAtributoID = pAtributoID;
            mNombre = pNombre;
            mDescripcion = pDescripcion;
            mTipo = (TipoAtributosCampos)pTipo;
            mOrden = pOrden;
            mValor = pValor;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Devuelve o establece el identificador de la ficha bibliográfica al que pertenece el atributo.
        /// </summary>
        public Guid FichaBibliograficaID
        {
            get
            {
                return mFichaBibliograficaID;
            }
            set
            {
                mFichaBibliograficaID = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el identificador del atributo.
        /// </summary>
        public Guid AtributoID
        {
            get
            {
                return mAtributoID;
            }
            set
            {
                mAtributoID = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el nombre del atributo.
        /// </summary>
        public override string Nombre
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
        /// Devuelve o establece el valor del atributo.
        /// </summary>
        public string Valor
        {
            get
            {
                return mValor;
            }
            set
            {
                mValor = value;
            }
        }

        /// <summary>
        /// Devuelve o establece la descripción del atributo.
        /// </summary>
        public string Descripcion
        {
            get
            {
                return mDescripcion;
            }
            set
            {
                mDescripcion = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el tipo del atributo.
        /// </summary>
        public TipoAtributosCampos Tipo
        {
            get
            {
                return mTipo;
            }
            set
            {
                mTipo = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el orden del atributo.
        /// </summary>
        public int Orden
        {
            get
            {
                return mOrden;
            }
            set
            {
                mOrden = value;
            }
        }

        #endregion
    }
}
