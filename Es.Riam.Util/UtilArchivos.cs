using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Linq;
using System.Text;

namespace Es.Riam.Util
{
    #region Enumeraciones

    /// <summary>
    /// Tipo de archivo según su extensión.
    /// </summary>
    public enum TipoArchivo
    {
        /// <summary>
        /// Archivo de imagen
        /// </summary>
        Imagen = 0,
        /// <summary>
        /// Archivo de vídeo
        /// </summary>
        Video = 1,
        /// <summary>
        /// Archivo de sonido
        /// </summary>
        Audio = 2,
        /// <summary>
        /// Otros tipos de archivo
        /// </summary>
        Otros = 3
    }

    #endregion

    /// <summary>
    /// Utilidades para el manejo de archivos
    /// </summary>
    public class UtilArchivos
    {
        public const string ContentImg = "img";
        public const string ContentImgIconos = "iconos";
        public const string ContentImagenes = "imagenes";
        public const string ContentMiniaturas = "miniaturas";
        public const string ContentImagenesPersonas = "Personas";
        public const string ContentImagenesPersona_Organizacion = "Persona_Organizacion";
        public const string ContentImagenesOrganizaciones = "organizaciones";
        public const string ContentImagenesProyectos = "proyectos";
        public const string ContentImagenesSolicitudes = "solicitudes";
        public const string ContentImagenesCategorias = "categorias";
        public const string ContentImagenesUsuarios = "Usuarios";
        public const string ContentImagenesDocumentos = "Documentos";

        public const string ContentImagenesEnlaces = "imagenesEnlaces";
        public const string ContentImagenesSemanticas = "imgsem";
        public const string ContentImagenesSemanticasAntiguo = "ImagenesSemanticas";
        public const string ContentImgCapSemanticasAntiguo = "capturassemanticas";
        public const string ContentImgCapSemanticas = "capsem";

		public const string ContentDocumentosSem = "docsem";
        public const string ContentDocumentosSemAntiguo = "DocumentosSemanticos";

        public const string ContentOntologias = "Ontologias";
        public const string ContentVideos = "videos";
        public const string ContentVideosSemanticos = "VideosSemanticos";       
        public const string ContentDLLs = "dll";
        public const string ContentTemporales = "temporales";
        /// <summary>
        /// Content para los documentos que son links.
        /// </summary>
        public const string ContentDocLinks = "doclinks";

        #region Métodos estáticos

        /// <summary>
        /// Obtiene el tipo de archivo de un fichero según su extensión.
        /// </summary>
        /// <param name="pFichero">Nombre del fichero</param>
        /// <returns>Tipo de archivo de un fichero según su extensión.</returns>
        public static TipoArchivo ObtenerTipoArchivo(string pFichero)
        {
            TipoArchivo tipoArchivo = TipoArchivo.Otros;
            switch (System.IO.Path.GetExtension(pFichero).ToLower())
            {
                #region Video
                case ".avi":
                case ".mpg":
                case ".mpge":
                case ".mpeg":
                case ".wmv":
                case ".flv":
                    tipoArchivo = TipoArchivo.Video;
                    break;
                #endregion

                #region Imagen
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".gif":
                case ".bmp":
                case ".tif":
                    tipoArchivo = TipoArchivo.Imagen;
                    break;
                #endregion

                #region Audio
                case ".mp2":
                case ".mp3":
                case ".wav":
                case ".wma":
                case ".midi":
                    tipoArchivo = TipoArchivo.Audio;
                    break;
                #endregion

                default:
                    tipoArchivo = TipoArchivo.Otros;
                    break;
            }
            return tipoArchivo;
        }

        /// <summary>
        /// TRUE si el archivo es accesible para lectura
        /// </summary>
        /// <param name="pRuta">Ruta del fichero</param>
        public static bool FicheroAccesible(string pRuta)
        {
            try
            {
                FileStream archivo = File.OpenRead(pRuta);
                archivo.Close();
                archivo.Dispose();
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Devuelve el subdirectorio de un recurso a partir de su ID.
        /// </summary>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <returns>Subdirectorio de un recurso a partir de su ID sin barras ni al principio ni al final</returns>
        public static string DirectorioDocumento(Guid pDocumentoID)
        {
            string identificador = pDocumentoID.ToString();
            return Path.Combine(identificador.Substring(0, 2), identificador.Substring(0, 4), identificador);
        }

        /// <summary>
        /// Devuelve el subdirectorio de un recurso a partir de su ID.
        /// </summary>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <returns>Subdirectorio de un recurso a partir de su ID sin barras ni al principio ni al final</returns>
        public static string DirectorioDocumentoFileSystem(Guid pDocumentoID)
        {
            string doc = pDocumentoID.ToString();
            return Path.Combine(doc.Substring(0, 2), doc.Substring(0, 4), doc);
        }

        #endregion
    }
}
