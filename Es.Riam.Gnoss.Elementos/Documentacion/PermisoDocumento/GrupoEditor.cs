using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Elementos.Documentacion.PermisoDocumento
{
    /// <summary>
    /// Representa un grupo de editores de un documento.
    /// </summary>
    public class GrupoEditor
    {
        #region Miembros

        /// <summary>
        /// Nombre del grupo.
        /// </summary>
        private string mNombre;

        /// <summary>
        /// Identificador de grupo.
        /// </summary>
        private Guid mGrupoEditorID;

        /// <summary>
        /// Miembros del grupo
        /// </summary>
        private List<Guid> mListaPersonas;

        /// <summary>
        /// Miembros del grupo eliminados.
        /// </summary>
        private List<Guid> mListaPersonasEliminadas;

        /// <summary>
        /// Miembros del grupo agregados.
        /// </summary>
        private List<Guid> mListaPersonasAgregadas;

        /// <summary>
        /// Contiene true si el grupo es de editores, false en caso contrario.
        /// </summary>
        private bool mEditor;

        #endregion

        #region Propiedades

        /// <summary>
        ///  Devuelve o establece el nombre del grupo.
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
        ///  Devuelve o establece el identificador de grupo.
        /// </summary>
        public Guid GrupoEditorID
        {
            get
            {
                return mGrupoEditorID;
            }
            set
            {
                mGrupoEditorID = value;
            }
        }

        /// <summary>
        /// Devuelve o establece los miembros del grupo
        /// </summary>
        public List<Guid> ListaPersonas
        {
            get
            {
                if (mListaPersonas == null)
                {
                    mListaPersonas = new List<Guid>();
                }
                return mListaPersonas;
            }
            set
            {
                mListaPersonas = value;
            }
        }

        /// <summary>
        /// Miembros del grupo eliminados.
        /// </summary>
        public List<Guid> ListaPersonasEliminadas
        {
            get
            {
                if (mListaPersonasEliminadas == null)
                {
                    mListaPersonasEliminadas = new List<Guid>();
                }
                return mListaPersonasEliminadas;
            }
            set
            {
                mListaPersonasEliminadas = value;
            }
        }

        /// <summary>
        /// Miembros del grupo agregados.
        /// </summary>
        public List<Guid> ListaPersonasAgregadas
        {
            get
            {
                if (mListaPersonasAgregadas == null)
                {
                    mListaPersonasAgregadas = new List<Guid>();
                }
                return mListaPersonasAgregadas;
            }
            set
            {
                mListaPersonasAgregadas = value;
            }
        }

        /// <summary>
        /// Contiene true si el grupo es de editores, false en caso contrario.
        /// </summary>
        public bool Editor
        {
            get
            {
                return mEditor;
            }
            set 
            {
                mEditor = value;
            }
        }

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor del grupo de editores a partir de su identificador y su nombre pasados por parámetro
        /// </summary>
        /// <param name="pGrupoEditorID">Identificador del grupo de editores</param>
        /// <param name="pNombre">Nombre del grupo de editores</param>
        public GrupoEditor(Guid pGrupoEditorID, string pNombre)
        {
            mGrupoEditorID = pGrupoEditorID;
            mNombre = pNombre;
        }

        #endregion
    }
}
