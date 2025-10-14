using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Elementos.Documentacion
{
    /// <summary>
    /// Lector de un recurso
    /// </summary>
    public class LectorRecurso : ElementoGnoss
    {

        #region Constructores

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pFilaLector">Fila del Lector</param>
        /// <param name="pGestorDocumental">Gestor documental</param>
        public LectorRecurso(AD.EntityModel.Models.Documentacion.DocumentoRolIdentidad pFilaLector, GestorDocumental pGestorDocumental)
            : base(pFilaLector, pGestorDocumental)
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
                return FilaLector.PerfilID;
            }
        }

        /// <summary>
        /// Obtiene el documento del que es lector
        /// </summary>
        public Documento DocumentoLeido
        {
            get
            {
                if (GestionDocumental.ListaDocumentos.ContainsKey(FilaLector.DocumentoID))
                {
                    return GestionDocumental.ListaDocumentos[FilaLector.DocumentoID];
                }
                return null;
            }
        }

        ///// <summary>
        ///// Identidad del usuario que edita el documento
        ///// </summary>
        //public Identidad.Identidad IdentidadUsuario
        //{
        //    get
        //    {
        //        if ((GestionDocumental.GestorIdentidades != null) && (GestionDocumental.GestorIdentidades.ListaIdentidades.ContainsKey(Clave)))
        //        {
        //            return GestionDocumental.GestorIdentidades.ListaIdentidades[Clave];
        //        }
        //        return null;
        //    }
        //}

        /// <summary>
        /// Identidad del usuario que edita el documento
        /// </summary>
        public Identidad.Identidad IdentidadEnProyectoActual(Guid pProyectoID)
        {
            return ObtenerIdentidadLectorEnProyecto(pProyectoID);
        }

        /// <summary>
        /// Identidad del usuario que edita el documento
        /// </summary>
        public Identidad.Identidad IdentidadEnCualquierProyecto
        {
            get
            {
                if (GestionDocumental.GestorIdentidades != null)
                {
                    List<AD.EntityModel.Models.IdentidadDS.Identidad> filasIdent = GestionDocumental.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.PerfilID.Equals(Clave)).ToList();

                    if (filasIdent.Count > 0 && GestionDocumental.GestorIdentidades.ListaIdentidades.ContainsKey(filasIdent.First().IdentidadID))
                    {
                        return GestionDocumental.GestorIdentidades.ListaIdentidades[filasIdent.First().IdentidadID];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Obtiene la fila del Lector
        /// </summary>
        public AD.EntityModel.Models.Documentacion.DocumentoRolIdentidad FilaLector
        {
            get
            {
                return (AD.EntityModel.Models.Documentacion.DocumentoRolIdentidad)FilaElementoEntity;
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

        #endregion

        #region Métodos

        /// <summary>
        /// Devuelve la identidad del Lector en un proyecto determinado o NULL si no pertenece a ese proyecto.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>La identidad del Lector en un proyecto determinado o NULL si no pertenece a ese proyecto</returns>
        public Identidad.Identidad ObtenerIdentidadLectorEnProyecto(Guid pProyectoID)
        {
            if (GestionDocumental.GestorIdentidades != null)
            {//"PerfilID='" + Clave + "' AND ProyectoID='" + pProyectoID + "' AND FechaBaja is NULL"
                List<AD.EntityModel.Models.IdentidadDS.Identidad> filasIdent = GestionDocumental.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.PerfilID.Equals(Clave) && identidad.ProyectoID.Equals(pProyectoID) && !identidad.FechaBaja.HasValue).ToList();

                if (filasIdent.Count > 0 && GestionDocumental.GestorIdentidades.ListaIdentidades.ContainsKey(filasIdent.First().IdentidadID))
                {
                    return GestionDocumental.GestorIdentidades.ListaIdentidades[filasIdent.First().IdentidadID];
                }
            }
            return null;
        }

        #endregion

    }
}
