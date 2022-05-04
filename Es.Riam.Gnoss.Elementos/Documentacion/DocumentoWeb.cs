using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Elementos
{
    /// <summary>
    /// Clase que representa un documento Web.
    /// </summary>
    public class DocumentoWeb : Documento
    {
        #region Miembros

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor para el documento
        /// </summary>
        /// <param name="pFilaDocumento">Fila del documento</param>
        /// <param name="pGestorDocumental">Gestor documental</param>
        public DocumentoWeb(AD.EntityModel.Models.Documentacion.Documento pFilaDocumento, GestorDocumental pGestorDocumental, LoggingService loggingService)
            : base(pFilaDocumento, pGestorDocumental, loggingService)
        {
            //Reemplazo el documento por el documentoweb
            if (!pGestorDocumental.Hijos.Contains(pGestorDocumental.ListaDocumentos[pFilaDocumento.DocumentoID]))
            {
                pGestorDocumental.Hijos.Add(pGestorDocumental.ListaDocumentos[pFilaDocumento.DocumentoID]);
            }
            else
            {
                pGestorDocumental.Hijos[pGestorDocumental.Hijos.IndexOf(pGestorDocumental.ListaDocumentos[pFilaDocumento.DocumentoID])] = this;
            }
            pGestorDocumental.ListaDocumentos[pFilaDocumento.DocumentoID] = this;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Devuelve si el documento está compartido o no.
        /// </summary>
        public bool Compartido
        {
            get
            {
                List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filasDoc = GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(Clave) && doc.BaseRecursosID.Equals(GestorDocumental.BaseRecursosIDActual)).ToList();
                if (filasDoc.Count > 0)
                {
                    return filasDoc[0].TipoPublicacion != (short)TipoPublicacion.Publicado;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Devuelve si el documento está compartido o no.
        /// </summary>
        public bool CompartidoEnProyectoActual(Guid pProyectoID)
        {
            try
            {// "ProyectoID='" + Usuario.UsuarioActual.ProyectoID + "'"
                List<AD.EntityModel.Models.Documentacion.BaseRecursosProyecto> filasBRProy = GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosProyecto.Where(baseRec => baseRec.ProyectoID.Equals(pProyectoID)).ToList();

                if (filasBRProy.Count > 0)
                {
                    List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filasDoc = GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(Clave) && doc.BaseRecursosID.Equals(filasBRProy[0].BaseRecursosID)).ToList();
                    if (filasDoc.Count > 0)
                    {
                        return filasDoc[0].TipoPublicacion != (short)TipoPublicacion.Publicado;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// Devuelve la identidad y la fecha con las que está compartido el documento o null si no está compartido.
        /// </summary>
        public KeyValuePair<Guid, DateTime> CompartidoIdentidaFechaEnProyectoActual(Guid pProyectoID)
        {
            KeyValuePair<Guid, DateTime> identidadFecha = new KeyValuePair<Guid, DateTime>();

            List<AD.EntityModel.Models.Documentacion.BaseRecursosProyecto> filasBRProy = GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosProyecto.Where(doc => doc.ProyectoID.Equals(pProyectoID)).ToList();

            if (filasBRProy.Count > 0)
            {
                List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filasDoc = GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(Clave) && doc.BaseRecursosID.Equals(filasBRProy[0].BaseRecursosID)).ToList();
                if (filasDoc.Count > 0 && filasDoc.First().IdentidadPublicacionID.HasValue && filasDoc.First().FechaPublicacion.HasValue)
                {
                    identidadFecha = new KeyValuePair<Guid, DateTime>(filasDoc.First().IdentidadPublicacionID.Value, filasDoc.First().FechaPublicacion.Value);
                }
            }
            else if (pProyectoID == ProyectoAD.MetaProyecto)
            {
                AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaDocVinBR = GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(Clave) && doc.FechaPublicacion.HasValue && doc.IdentidadPublicacionID.HasValue && doc.FechaPublicacion.HasValue).OrderBy(doc => doc.FechaPublicacion.Value).FirstOrDefault();
                if (filaDocVinBR != null && filaDocVinBR.IdentidadPublicacionID.HasValue && filaDocVinBR.FechaPublicacion.HasValue)
                {
                    identidadFecha = new KeyValuePair<Guid, DateTime>(filaDocVinBR.IdentidadPublicacionID.Value, filaDocVinBR.FechaPublicacion.Value);
                }
            }


            return identidadFecha;

        }

        #endregion

        #region Métodos

        /// <summary>
        /// Obtiene la fila DocumentoWebVinBaseRecursosRow de una base de recursos determinada.
        /// </summary>
        /// <param name="pBaseRecursosID">Identificador de base de recursos</param>
        /// <returns>Fila DocumentoWebVinBaseRecursosRow de una base de recursos determinada.</returns>
        public AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos ObtenerFilaDocumentoCompartido(Guid pBaseRecursosID)
        {//"DocumentoID='" + this.Clave + "' AND BaseRecursosID = '" + pBaseRecursosID + "'"
            List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> compartidos = this.GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(this.Clave) && doc.BaseRecursosID.Equals(pBaseRecursosID)).ToList();

            if (compartidos.Count > 0)
            {
                return compartidos.First();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Comprueba si una identidad puede descompartir de una comunidad el documento.
        /// </summary>
        /// <param name="pBaseRecursosID">Identificador de la base de recursos de una comunidad</param>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <returns>True si se puede descompartir, false si no</returns>
        public bool PuedeIdentidadDesCompartir(Guid pBaseRecursosID, Guid pIdentidadID)
        {
            List<AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filasDoc = GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(doc => doc.DocumentoID.Equals(Clave) && doc.BaseRecursosID.Equals(pBaseRecursosID) && doc.IdentidadPublicacionID.Equals(pIdentidadID)).ToList();/*AND Compartido=1*/

            return filasDoc.Count > 0;
        }

        #endregion
    }

    /// <summary>
    /// Clase que da valor a los votos de los documentos.
    /// </summary>
    public class VotoDocumento
    {
        /// <summary>
        /// Valor de voto positivo
        /// </summary>
        public static double Positivo = 1;

        /// <summary>
        /// Valor del voto negativo.
        /// </summary>
        public static double Negativo = -1;
    }
}
