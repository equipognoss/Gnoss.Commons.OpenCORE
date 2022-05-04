using Es.Riam.Gnoss.AD.Amigos;
using Es.Riam.Gnoss.AD.Amigos.Model;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Elementos.Amigos
{
    /// <summary>
    /// Grupo de amigos
    /// </summary>
    public class GrupoAmigos : ElementoGnoss
    {
        #region Miembros

        private Dictionary<Guid, Identidad.Identidad> mListaAmigos;

        /// <summary>
        /// Gestor de Amigos
        /// </summary>
        private GestionAmigos mGestionAmigos;

        private List<Guid> mListaAmigosID;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pFilaGrupoAmigos">Fila del grupo de amigos</param>
        /// <param name="pGestionAmigos">Gestor de amigos</param>
        public GrupoAmigos(AD.EntityModel.Models.IdentidadDS.GrupoAmigos pFilaGrupoAmigos, GestionAmigos pGestionAmigos, LoggingService loggingService)
            : base(pFilaGrupoAmigos, pGestionAmigos, loggingService)
        {
            mGestionAmigos = pGestionAmigos;
        }

        #endregion

        #region Métodos

        #region Permiso Amigos

        /// <summary>
        /// Devuelve la lista con las identidades de una organización que tienes permiso sobre la actual y la 2º con las que tienen
        /// permiso de edición.
        /// </summary>
        /// <param name="pIdentidadOrganizacionID">Identificador de la identidad de la organización a la que pertenece cada contacto</param>
        /// <returns>Lista con las identidades de una organización que tienes permiso sobre la actual.</returns>
        public KeyValuePair<List<Guid>,List<Guid>> AmigosPermisoSobreGrupoActual(Guid pIdentidadOrganizacionID)
        {
            List<Guid> listaIdentidadesSeleccionadas = new List<Guid>();
            List<Guid> listaIdentidadesSeleccionadasYCheckeadas = new List<Guid>();
            foreach (PermisoGrupoAmigoOrg filaPerAmiOrg in GestionAmigos.AmigosDW.ListaPermisoGrupoAmigoOrg.Where(item => item.GrupoID.Equals(Clave) && item.IdentidadOrganizacionID.Equals(pIdentidadOrganizacionID)).ToList())
            {
                listaIdentidadesSeleccionadas.Add(filaPerAmiOrg.IdentidadUsuarioID);
                if (filaPerAmiOrg.PermisoEdicion)
                {
                    listaIdentidadesSeleccionadasYCheckeadas.Add(filaPerAmiOrg.IdentidadUsuarioID);
                }
            }

            return new KeyValuePair<List<Guid>,List<Guid>>(listaIdentidadesSeleccionadas, listaIdentidadesSeleccionadasYCheckeadas);
        }

        #endregion

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la clave del grupo de amigos
        /// </summary>
        public override Guid Clave
        {
            get
            {
                return FilaGrupoAmigos.GrupoID;
            }
        }

        /// <summary>
        /// Devuelve el tipo del grupo de amigos
        /// </summary>
        public TipoGrupoAmigos Tipo
        {
            get
            {
                return (TipoGrupoAmigos)FilaGrupoAmigos.Tipo;
            }
        }

        /// <summary>
        /// Obtiene la fila del grupo de amigos
        /// </summary>
        public AmigosDS.GrupoAmigosRow FilaGrupoAmigos
        {
            get
            {
                return (AmigosDS.GrupoAmigosRow)FilaElemento;
            }
        }

        /// <summary>
        /// Obtiene la lista de identidades de amigos
        /// </summary>
        public Dictionary<Guid, Identidad.Identidad> ListaAmigos
        {
            get
            {
                if (mListaAmigos == null)
                {
                    mListaAmigos = new Dictionary<Guid, Identidad.Identidad>();
                    
                    foreach (AmigosDS.AmigoAgGrupoRow filaAmigoAgGrupo in FilaGrupoAmigos.GetAmigoAgGrupoRows())
                    {
                        if (GestionAmigos.GestionIdentidades != null && GestionAmigos.GestionIdentidades.ListaIdentidades.ContainsKey(filaAmigoAgGrupo.IdentidadAmigoID))
                        {
                            mListaAmigos.Add(filaAmigoAgGrupo.IdentidadAmigoID, GestionAmigos.GestionIdentidades.ListaIdentidades[filaAmigoAgGrupo.IdentidadAmigoID]);
                        }
                    }
                }
                return mListaAmigos;
            }
        }

        /// <summary>
        /// Obtiene la lista de identidades de amigos
        /// </summary>
        public List<Guid> ListaAmigosID
        {
            get
            {
                if (mListaAmigosID == null)
                {
                    mListaAmigosID = new List<Guid>();

                    foreach (AmigosDS.AmigoAgGrupoRow filaAmigoAgGrupo in FilaGrupoAmigos.GetAmigoAgGrupoRows())
                    {
                        
                            mListaAmigosID.Add(filaAmigoAgGrupo.IdentidadAmigoID);
                       
                    }
                }
                return mListaAmigosID;
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor de amigos
        /// </summary>
        public GestionAmigos GestionAmigos
        {
            get
            {
                return mGestionAmigos;
            }
            set
            {
                mGestionAmigos = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el nombre del grupo
        /// </summary>
        public override string Nombre
        {
            get
            {
                return FilaGrupoAmigos.Nombre;
            }
            set
            {
                FilaGrupoAmigos.Nombre = value;
            }
        }

        #endregion
    }
}
