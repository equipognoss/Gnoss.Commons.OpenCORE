using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Elementos.Documentacion
{
    /// <summary>
    /// Editor de un recurso
    /// </summary>
    public class EditorRecurso : ElementoGnoss
    {

        #region Constructores

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pFilaEditor">Fila del editor</param>
        /// <param name="pGestorDocumental">Gestor documental</param>
        public EditorRecurso(AD.EntityModel.Models.Documentacion.DocumentoRolIdentidad pFilaEditor, GestorDocumental pGestorDocumental, LoggingService loggingService)
            : base(pFilaEditor, pGestorDocumental, loggingService)
        {
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene el Identificador de la identidad del editor
        /// </summary>
        public override Guid Clave
        {
            get
            {
                return FilaEditor.PerfilID;
            }
        }

        /// <summary>
        /// Obtiene el documento editado
        /// </summary>
        public Documento DocumentoEditado
        {
            get
            {
                if (GestionDocumental.ListaDocumentos.ContainsKey(FilaEditor.DocumentoID))
                {
                    return GestionDocumental.ListaDocumentos[FilaEditor.DocumentoID];
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
            return ObtenerIdentidadEditorEnProyecto(pProyectoID);
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
                    List<AD.EntityModel.Models.IdentidadDS.Identidad> filasIdent = GestionDocumental.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(ident => ident.PerfilID.Equals(Clave)).ToList();

                    if (filasIdent.Count > 0 && GestionDocumental.GestorIdentidades.ListaIdentidades.ContainsKey(filasIdent.First().IdentidadID))
                    {
                        return GestionDocumental.GestorIdentidades.ListaIdentidades[filasIdent.First().IdentidadID];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Obtiene la fila del editor
        /// </summary>
        public AD.EntityModel.Models.Documentacion.DocumentoRolIdentidad FilaEditor
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
        /// Devuelve la identidad del editor en un proyecto determinado o NULL si no pertenece a ese proyecto.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>La identidad del editor en un proyecto determinado o NULL si no pertenece a ese proyecto</returns>
        public Identidad.Identidad ObtenerIdentidadEditorEnProyecto(Guid pProyectoID)
        {
            if (GestionDocumental.GestorIdentidades != null)
            {//"PerfilID='" + Clave + "' AND ProyectoID='" + pProyectoID + "' AND FechaBaja is NULL"
                return GestionDocumental.GestorIdentidades.ListaIdentidades.FirstOrDefault(ident => ident.Value.PerfilID.Equals(Clave) && ident.Value.FilaIdentidad.ProyectoID.Equals(pProyectoID) && !ident.Value.FilaIdentidad.FechaBaja.HasValue).Value;
            }
            return null;
        }

        #endregion

    }
}
