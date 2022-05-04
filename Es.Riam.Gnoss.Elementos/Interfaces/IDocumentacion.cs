using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Interfaces;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Elementos.Interfaces
{
    /// <summary>
    /// Documentación común
    /// </summary>
    public interface IDocumentacionComun : IElementoGnoss
    {
        /// <summary>
        /// Devuelve el creador de la entidad
        /// </summary>
        SortedList<Guid, Documento> Documentos
        {
            get;
        }
    }

    /// <summary>
    /// Documentación
    /// </summary>
    public interface IDocumentacion : IDocumentacionComun
    {        
        /// <summary>
        /// Agrega un documento
        /// </summary>
        /// <param name="pDocumento">Documento a agregar al elemento</param>
        void AgregarDocumento(Documento pDocumento);

        /// <summary>
        /// Elimina un documento
        /// </summary>
        /// <param name="pDocumento">Documento a eliminar</param>
        void EliminarDocumento(Documento pDocumento);
    }
}
