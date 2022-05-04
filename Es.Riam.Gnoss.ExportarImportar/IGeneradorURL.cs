using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Tesauro;
using System;

namespace Es.Riam.Metagnoss.ExportarImportar
{
    /// <summary>
    /// Interfaz para el generador de URLs
    /// </summary>
    public interface IGeneradorURL
    {
        /// <summary>
        /// Obtiene la URL del recurso pasado por par�metro
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <returns>URL del recurso</returns>
        string ObtenerUrlRecurso(Documento pDocumento);

        /// <summary>
        /// Obtiene la URL de descarga del recurso pasado por par�metro
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <returns>URL del recurso</returns>
        string ObtenerUrlDescargaRecurso(Documento pDocumento);

        /// <summary>
        /// Obtiene la URL de un documento.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de un documento</param>
        /// <param name="pGestorDocumental">gestor documentaci�n</param>
        /// <returns>Url del recurso</returns>
        string ObtenerUrlRecurso(Guid pDocumentoID, GestorDocumental pGestorDocumental);

        /// <summary>
        /// Obtiene la URL de la persona pasada por par�metro
        /// </summary>
        /// <param name="pPersona">Persona</param>
        /// <returns>URL de la persona</returns>
        string ObtenerUrlPersona(Persona pPersona);

        /// <summary>
        /// Obtiene la URL de un tag
        /// </summary>
        /// <param name="pTag">Tag a obtener</param>
        /// <returns>URL del tag</returns>
        string ObtenerUrlTag(string pTag);

        /// <summary>
        /// Obtiene la URL de la identidad pasada por par�metro
        /// </summary>
        /// <param name="pIdentidad">Identidad</param>
        /// <returns>URL de la identidad</returns>
        string ObtenerUrlIdentidad(Identidad pIdentidad);

        /// <summary>
        /// Obtiene la URL del comentario pasado por par�metro
        /// </summary>
        /// <param name="pComentarioID">Identificador de comentario</param>
        /// <returns>URL del comentario</returns>
        string ObtenerUrlComentario(Guid pComentarioID);

        /// <summary>
        /// Obtiene la URL de la comunidad pasada por par�metro
        /// </summary>
        /// <param name="pNombreCortoProyecto">Nombre corto de comunidad</param>
        /// <returns>URL de la comunidad</returns>
        string ObtenerUrlComunidad(string pNombreCortoProyecto);

        /// <summary>
        /// Obtiene la URL de la biograf�a pasada por par�metro
        /// </summary>
        /// <param name="pBioID">Identificador de biograf�a</param>
        /// <param name="pTipoBio">Tipo de biograf�a</param>
        /// <returns>URL de la biograf�a</returns>
        string ObtenerUrlBiografia(Guid pBioID, string pTipoBio);

        /// <summary>
        /// Obtiene la url para una categoria de tesauro.
        /// </summary>
        /// <param name="pCategoriaTesauro">Categoria de tesauro</param>
        /// <returns>URL</returns>
        string ObtenerUrlCategoriaTesauro(CategoriaTesauro pCategoriaTesauro);

        /// <summary>
        /// Devuelve la url base.
        /// </summary>
        string UrlBase();

        /// <summary>
        /// Devuelve la url de la p�gina actual.
        /// </summary>
        /// <returns></returns>
        string UrlActual();

        /// <summary>
        /// Devuelve la url de la p�gina actual sin query.
        /// </summary>
        /// <returns></returns>
        string UrlActualSinQuery();
    }
}
