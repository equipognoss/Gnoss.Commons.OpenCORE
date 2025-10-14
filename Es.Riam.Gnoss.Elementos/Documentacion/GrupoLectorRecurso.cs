using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Elementos.Documentacion
{
    /// <summary>
    /// Grupo Lector de un recurso
    /// </summary>
    public class GrupoLectorRecurso : ElementoGnoss
    {

        #region Constructores

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pFilaGrupoLector">Fila del grupo Lector</param>
        /// <param name="pGestorDocumental">Gestor documental</param>
        public GrupoLectorRecurso(AD.EntityModel.Models.Documentacion.DocumentoRolGrupoIdentidades pFilaGrupoLector, GestorDocumental pGestorDocumental)
            : base(pFilaGrupoLector, pGestorDocumental)
        {

        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene el Identificador de la identidad del Lector
        /// </summary>
        public override Guid Clave
        {
            get
            {
                return FilaGrupoLector.GrupoID;
            }
        }

        /// <summary>
        /// Obtiene el documento editado
        /// </summary>
        public Documento DocumentoEditado
        {
            get
            {
                if (GestionDocumental.ListaDocumentos.ContainsKey(FilaGrupoLector.DocumentoID))
                {
                    return GestionDocumental.ListaDocumentos[FilaGrupoLector.DocumentoID];
                }
                return null;
            }
        }

        /// <summary>
        /// Obtiene la fila del Lector
        /// </summary>
        public AD.EntityModel.Models.Documentacion.DocumentoRolGrupoIdentidades FilaGrupoLector
        {
            get
            {
                return (AD.EntityModel.Models.Documentacion.DocumentoRolGrupoIdentidades)FilaElementoEntity;
            }
        }

        /// <summary>
        /// Obtiene el gestor de documentación
        /// </summary>
        public GestorDocumental GestionDocumental
        {
            get
            {
                return (GestorDocumental)GestorGnoss;
            }
        }

        /// <summary>
        /// Obtiene el nombre del elemento público
        /// </summary>
        public override string Nombre
        {
            get
            {
                string nombre = "";
                if (GestionDocumental != null && GestionDocumental.GestorIdentidades != null && GestionDocumental.GestorIdentidades.DataWrapperIdentidad != null && GestionDocumental.GestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidades != null)
                {
                    GrupoIdentidades fila = GestionDocumental.GestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidades.Where(item => item.GrupoID.Equals(Clave)).FirstOrDefault();
                    if (fila != null)
                    {
                        nombre = fila.Nombre;
                    }
                }
                return nombre;
            }
        }

        /// <summary>
        /// Obtiene la fila GrupoIdentidadProyecto
        /// </summary>
        public GrupoIdentidadesProyecto FilaGrupoIdentidadProyecto
        {
            get
            {
                GrupoIdentidadesProyecto fila = null;

                if (GestionDocumental != null && GestionDocumental.GestorIdentidades != null && GestionDocumental.GestorIdentidades.DataWrapperIdentidad != null && GestionDocumental.GestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidadesProyecto != null)
                {
                    fila = GestionDocumental.GestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidadesProyecto.Where(item => item.GrupoID.Equals(Clave)).FirstOrDefault();
                }
                return fila;
            }
        }

        /// <summary>
        /// Obtiene la fila GrupoIdentidadOrganizacion
        /// </summary>
        public GrupoIdentidadesOrganizacion FilaGrupoIdentidadOrganizacion
        {
            get
            {
                GrupoIdentidadesOrganizacion fila = null;

                if (GestionDocumental != null && GestionDocumental.GestorIdentidades != null && GestionDocumental.GestorIdentidades.DataWrapperIdentidad != null && GestionDocumental.GestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidadesOrganizacion != null)
                {
                    fila = GestionDocumental.GestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidadesOrganizacion.Where(item => item.GrupoID.Equals(Clave)).FirstOrDefault();                    
                }
                return fila;
            }
        }

        /// <summary>
        /// Obtiene la fila GrupoIdentidad
        /// </summary>
        public GrupoIdentidades FilaGrupoIdentidad
        {
            get
            {
                GrupoIdentidades fila = null;

                if (GestionDocumental != null && GestionDocumental.GestorIdentidades != null && GestionDocumental.GestorIdentidades.DataWrapperIdentidad != null && GestionDocumental.GestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidades != null)
                {
                    fila = GestionDocumental.GestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidades.Where(item => item.GrupoID.Equals(Clave)).FirstOrDefault();                    
                }
                return fila;
            }
        }




        /// <summary>
        /// Obtiene la lista de las identidades que participan
        /// </summary>
        public List<Guid> listaIdentidadedsParticipacion
        {
            get
            {
                List<Guid> listaIdentidades = new List<Guid>();

                if (GestionDocumental != null && GestionDocumental.GestorIdentidades != null && GestionDocumental.GestorIdentidades.DataWrapperIdentidad != null && GestionDocumental.GestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidadesParticipacion != null)
                {
                    List<GrupoIdentidadesParticipacion> filasGrupoIdentidadesParticipacion = GestionDocumental.GestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidadesParticipacion.Where(item => item.GrupoID.Equals(Clave)).ToList();

                    foreach (GrupoIdentidadesParticipacion filaParticipantes in filasGrupoIdentidadesParticipacion)
                    {
                        if (!filaParticipantes.FechaBaja.HasValue && !listaIdentidades.Contains(filaParticipantes.IdentidadID))
                        {
                            listaIdentidades.Add(filaParticipantes.IdentidadID);
                        }
                    }
                }
                return listaIdentidades;
            }
        }



        #endregion
    }
}
