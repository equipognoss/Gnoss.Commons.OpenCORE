using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Elementos.Documentacion.PermisoDocumento
{
    /// <summary>
    /// Clase para los permisos sobre un documento
    /// </summary>
    public class PermisoDocumento //: ElementoGnoss
    {
        #region Miembros

        /// <summary>
        /// Lista con los grupo del usuairo.
        /// </summary>
        private Dictionary<Guid, GrupoEditor> mListaGrupos;

        ///// <summary>
        ///// Lista con los grupo eliminados por el usuairo.
        ///// </summary>
        //private List<Guid> mListaGruposEliminados;

        ///// <summary>
        ///// Lista con los grupo agregados por el usuairo.
        ///// </summary>
        //private List<Guid> mListaGruposAgregados;

        /// <summary>
        /// Lista con las identidades editoras de un documento.
        /// </summary>
        private List<Guid> mListaIdentidadesEditores;

        /// <summary>
        /// Lista con las identidades lectores de un documento.
        /// </summary>
        private List<Guid> mListaIdentidadesLectores;

        #endregion

        #region Propiedades

        /// <summary>
        /// Devuelve o establece una lista con los grupo del usuairo.
        /// </summary>
        public Dictionary<Guid, GrupoEditor> ListaGrupos
        {
            get
            {
                if (mListaGrupos == null)
                    mListaGrupos = new Dictionary<Guid,GrupoEditor>();
                return mListaGrupos;
            }
            set
            {
                mListaGrupos = value;
            }
        }

        ///// <summary>
        ///// Devuelve o establece una lista con los grupo eliminados por el usuairo.
        ///// </summary>
        //public List<Guid> ListaGruposEliminados
        //{
        //    get
        //    {
        //        if (mListaGruposEliminados == null)
        //        {
        //            mListaGruposEliminados = new List<Guid>();
        //        }
        //        return mListaGruposEliminados;
        //    }
        //    set
        //    {
        //        mListaGruposEliminados =value;
        //    }
        //}

        ///// <summary>
        ///// Devuelve o establece una lista con los grupo agregados por el usuairo.
        ///// </summary>
        //public List<Guid> ListaGruposAgregados
        //{
        //    get
        //    {
        //        if (mListaGruposAgregados == null)
        //        {
        //            mListaGruposAgregados = new List<Guid>();
        //        }
        //        return mListaGruposAgregados;
        //    }
        //    set
        //    {
        //        mListaGruposAgregados = value;
        //    }
        //}

        /// <summary>
        /// Obtiene o establece una lista con las identidades editoras de un documento
        /// </summary>
        public List<Guid> ListaIdentidadesEditores
            {
            get
            {
                if (mListaIdentidadesEditores == null)
                    mListaIdentidadesEditores = new List<Guid>();
                return mListaIdentidadesEditores;
            }
            set
            {
                mListaIdentidadesEditores = value;
            }
        }

        /// <summary>
        /// Obtiene o establece una lista con las identidades lectores de un documento
        /// </summary>
        public List<Guid> ListaIdentidadesLectores
        {
            get
            {
                if (mListaIdentidadesLectores == null)
                    mListaIdentidadesLectores = new List<Guid>();
                return mListaIdentidadesLectores;
            }
            set
            {
                mListaIdentidadesLectores = value;
            }
        }

        #endregion
    }
}
