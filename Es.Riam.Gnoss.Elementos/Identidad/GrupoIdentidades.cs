using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Elementos.Identidad
{

    public class GrupoIdentidades : ElementoGnoss
    {
        #region Miembros

        private bool? mEsGrupoDeProyecto = null;

        private Dictionary<Guid, Identidad> mParticipantes = null;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pFilaPerfil">Fila del perfil</param>
        /// <param name="pGestorIdentidades">Gestor de identidades</param>
        public GrupoIdentidades(GrupoIdentidades pFilaGrupoIdentidades, GestionIdentidades pGestorIdentidades, LoggingService loggingService)
            : base(pFilaGrupoIdentidades, pGestorIdentidades, loggingService)
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pFilaPerfil">Fila del perfil</param>
        /// <param name="pGestorIdentidades">Gestor de identidades</param>
        public GrupoIdentidades(AD.EntityModel.Models.IdentidadDS.GrupoIdentidades pFilaGrupoIdentidades, GestionIdentidades pGestorIdentidades, LoggingService loggingService)
            : base(pFilaGrupoIdentidades, pGestorIdentidades, loggingService)
        {
        }
        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la persona relacionada con el perfil
        /// </summary>
        public bool EsGrupoDeProyecto
        {
            get
            {
                if(mEsGrupoDeProyecto == null)
                {
                    mEsGrupoDeProyecto = FilaGrupoProyecto != null;
                }
                return (bool)mEsGrupoDeProyecto;
            }
        }

        public Dictionary<Guid, Identidad> Participantes
        {
            get
            {
                if (mParticipantes == null)
                {
                    mParticipantes = new Dictionary<Guid, Identidad>();

                    foreach (AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion fila in FilasGrupoIdentidadesParticipacion)
                    {
                        if (!mParticipantes.ContainsKey(fila.IdentidadID) && GestorIdentidades.ListaIdentidades.ContainsKey(fila.IdentidadID))
                        {
                            mParticipantes.Add(fila.IdentidadID, GestorIdentidades.ListaIdentidades[fila.IdentidadID]);
                        }
                    }
                }
                return mParticipantes;

            }
        }

        /// <summary>
        /// Obtiene la clave de la identidad
        /// </summary>
        public override Guid Clave
        {
            get
            {
                return FilaGrupoIdentidades.GrupoID;
            }
        }

        /// <summary>
        /// Obtiene el gestor de identidades
        /// </summary>
        public GestionIdentidades GestorIdentidades
        {
            get
            {
                return (GestionIdentidades)GestorGnoss;
            }
        }

        /// <summary>
        /// Obtiene la fila del perfil
        /// </summary>
        public AD.EntityModel.Models.IdentidadDS.GrupoIdentidades FilaGrupoIdentidades
        {
            get
            {
                return (AD.EntityModel.Models.IdentidadDS.GrupoIdentidades)FilaElementoEntity;
            }
        }

        /// <summary>
        /// Obtiene la fila del perfil
        /// </summary>
        public AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesProyecto FilaGrupoProyecto
        {//"GrupoID = '" + Clave.ToString() + "'"
            get
            {
                AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesProyecto filasGrupoIdentidadesProyecto = GestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidadesProyecto.Where(grupoIdents => grupoIdents.GrupoID.Equals(Clave)).FirstOrDefault();

                return filasGrupoIdentidadesProyecto;
            }
        }

        /// <summary>
        /// Obtiene la fila de Grupo Organizacion
        /// </summary>
        public AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesOrganizacion FilaGrupoOrganizacion
        {
            get
            {
                AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesOrganizacion filasGrupoIdentidadesOrganizacion = GestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidadesOrganizacion.Where(grupoIdentOrg => grupoIdentOrg.GrupoID.Equals(Clave)).FirstOrDefault();

                return filasGrupoIdentidadesOrganizacion;

            }
        }


        public List<AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesParticipacion> FilasGrupoIdentidadesParticipacion
        {
            get
            {
                return GestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidadesParticipacion.Where(grupoIdentPart => grupoIdentPart.GrupoID.Equals(Clave)).ToList();
            }
        }

        /// <summary>
        /// Obtiene el nombre de una persona que trabaja con una organización en modo PERSONAL
        /// </summary>
        public override string Nombre
        {
            get
            {
                return FilaGrupoIdentidades.Nombre;
            }
        }

        /// <summary>
        /// Obtiene el nombre de una persona que trabaja con una organización en modo PERSONAL
        /// </summary>
        public string NombreCorto
        {
            get
            {
                return FilaGrupoIdentidades.NombreCorto;
            }
        }

        /// <summary>
        /// Obtiene el nombre de una persona que trabaja con una organización en modo PERSONAL
        /// </summary>
        public string Descripcion
        {
            get
            {
                return FilaGrupoIdentidades.Descripcion;
            }
        }

        /// <summary>
        /// Obtiene si el proyecto es publico
        /// </summary>
        public bool EsPublico
        {
            get
            {
                return FilaGrupoIdentidades.Publico;
            }
        }

        /// <summary>
        /// Obtiene si el proyecto es publico
        /// </summary>
        public bool PermitirEnviarMensajes
        {
            get
            {
                return FilaGrupoIdentidades.PermitirEnviarMensajes;
            }
        }

        #endregion
    }
}
