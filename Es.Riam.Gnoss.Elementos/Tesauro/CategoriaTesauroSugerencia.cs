using Es.Riam.Gnoss.Util.General;
using System;

namespace Es.Riam.Gnoss.Elementos.Tesauro
{
    /// <summary>
    /// Sugerencia de categoria del tesauro (By Altu).
    /// </summary>
    public class CategoriaTesauroSugerencia : ElementoGnoss
    {
        #region Miembros

        /// <summary>
        /// Categor�a del tesauro padre de la sugerencia.
        /// </summary>
        private CategoriaTesauro mCategoriaTesauroPadre;

        /// <summary>
        /// Identificador de la sugerencia de categor�a.
        /// </summary>
        private Guid mSugerenciaID;

        /// <summary>
        /// Identificador del tesauro en el que hace la sugerencia de categor�a.
        /// </summary>
        private Guid mTesauroID;

        /// <summary>
        /// Nombre de la sugerencia de categor�a.
        /// </summary>
        private string mNombre;

        /// <summary>
        /// Identificador de la identidad que ha realizado la sugerencia de categor�a.
        /// </summary>
        private Guid mIdentidadSugeridor;

        /// <summary>
        /// Gestor del tesauro que contiene el control.
        /// </summary>
        private GestionTesauro mGestorTesauro;

        /// <summary>
        /// Fila de la sugerencia de categor�a.
        /// </summary>
        private AD.EntityModel.Models.Tesauro.CategoriaTesauroSugerencia mFilaCategoriaTesauroSugerencia;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor p�blico.
        /// </summary>
        /// <param name="pFilaCatSugerencia">Fila de la sugerencia de categor�a</param>
        /// <param name="pGestorTesauro">Gestor de tesauro</param>
        public CategoriaTesauroSugerencia(AD.EntityModel.Models.Tesauro.CategoriaTesauroSugerencia pFilaCatSugerencia, GestionTesauro pGestorTesauro, LoggingService loggingService)
            : base(loggingService)
        {
            if (pFilaCatSugerencia.CategoriaTesauroPadreID.HasValue && pGestorTesauro.ListaCategoriasTesauro.ContainsKey(pFilaCatSugerencia.CategoriaTesauroPadreID.Value))
            {
                mCategoriaTesauroPadre = pGestorTesauro.ListaCategoriasTesauro[pFilaCatSugerencia.CategoriaTesauroPadreID.Value];
            }

            mSugerenciaID = pFilaCatSugerencia.SugerenciaID;
            mNombre = pFilaCatSugerencia.Nombre;
            mIdentidadSugeridor = pFilaCatSugerencia.IdentidadID;
            mTesauroID = pFilaCatSugerencia.TesauroSugerenciaID;
            mGestorTesauro = pGestorTesauro;
            mFilaCategoriaTesauroSugerencia = pFilaCatSugerencia;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Devuelve o establece la categor�a del tesauro padre de la sugerencia.
        /// </summary>
        public CategoriaTesauro CategoriaTesauroPadre
        {
            get
            {
                return mCategoriaTesauroPadre;
            }
        }

        /// <summary>
        /// Devuelve o establece el identificador de la sugerencia de categor�a.
        /// </summary>
        public Guid SugerenciaID
        {
            get
            {
                return mSugerenciaID;
            }
        }

        /// <summary>
        /// Devuelve o establece el identificador de la sugerencia de categor�a.
        /// </summary>
        public override Guid Clave
        {
            get
            {
                return mSugerenciaID;
            }
        }

        /// <summary>
        /// Devuelve o establece el nombre de la sugerencia de categor�a.
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
        /// Devuelve o establece el identificador de la identidad que ha realizado la sugerencia de categor�a.
        /// </summary>
        public Guid IdentidadSugeridor
        {
            get
            {
                return mIdentidadSugeridor;
            }
        }

        /// <summary>
        /// Devuelve o establece el identificador del tesauro en el que hace la sugerencia de categor�a.
        /// </summary>
        public Guid TesauroID
        {
            get
            {
                return mTesauroID;
            }
        }

        #endregion
    }
}
