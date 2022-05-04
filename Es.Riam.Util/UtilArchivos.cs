using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

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
        public const string ContentImagenesPersonas = "Personas";
        public const string ContentImagenesPersona_Organizacion = "Persona_Organizacion";
        public const string ContentImagenesOrganizaciones = "Organizaciones";
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
        /// 
        /// </summary>
        /// <param name="pFichero"></param>
        /// <returns></returns>
        public static byte[] EncriptarArchivo(byte[] pFichero)
        {
            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = new MemoryStream();

            RijndaelManaged encriptador = new RijndaelManaged();
            encriptador.KeySize = 256;

            byte[] key = { 55, 13, 86, 175, 186, 91, 43, 99, 211, 194, 143, 138, 228, 43, 80, 64, 249, 222, 192, 217, 14, 105, 194, 18, 252, 43, 43, 46, 24, 68, 231, 45 };
            byte[] iv = { 210, 45, 163, 172, 80, 194, 209, 4, 113, 160, 118, 151, 234, 43, 52, 81 };

            //Establecer la clave secreta para el algoritmo DES. 
            encriptador.Key = key;
            //Establecer el vector de inicialización. 
            encriptador.IV = iv;

            ICryptoTransform encrypt = encriptador.CreateEncryptor();

            // Define cryptographic stream (always use Write mode for encryption).
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encrypt, CryptoStreamMode.Write);
            // Start encrypting.
            cryptoStream.Write(pFichero, 0, pFichero.Length);

            // Finish encrypting.
            cryptoStream.FlushFinalBlock();

            // Convert our encrypted data from a memory stream into a byte array.
            byte[] buff = memoryStream.ToArray();

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            return buff;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pFichero"></param>
        /// <returns></returns>
        public static byte[] DesencriptarArchivo(byte[] pFichero)
        {
            // Generate decryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key 
            // bytes.

            RijndaelManaged encriptador = new RijndaelManaged();
            encriptador.KeySize = 256;

            byte[] key = { 55, 13, 86, 175, 186, 91, 43, 99, 211, 194, 143, 138, 228, 43, 80, 64, 249, 222, 192, 217, 14, 105, 194, 18, 252, 43, 43, 46, 24, 68, 231, 45 };
            byte[] iv = { 210, 45, 163, 172, 80, 194, 209, 4, 113, 160, 118, 151, 234, 43, 52, 81 };

            //Establecer la clave secreta para el algoritmo DES. 
            encriptador.Key = key;
            //Establecer el vector de inicialización. 
            encriptador.IV = iv;

            ICryptoTransform decryptor = encriptador.CreateDecryptor();

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = new MemoryStream(pFichero);

            // Define cryptographic stream (always use Read mode for encryption).
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

            // Since at this point we don't know what the size of decrypted data
            // will be, allocate the buffer long enough to hold ciphertext;
            // plaintext is never longer than ciphertext.
            byte[] buff = new byte[memoryStream.Length];

            byte[] buff2 = null;

            try
            {
                //Si el fichero no esta encriptado da error, devolvemos el original
                // Start decrypting.
                int decryptedByteCount = cryptoStream.Read(buff, 0, buff.Length);
                buff2 = new byte[decryptedByteCount];
                List<byte> lista = new List<byte>(buff);
                lista.CopyTo(0, buff2, 0, decryptedByteCount);

                // Close both streams.
                cryptoStream.Close();
            }
            catch (Exception)
            {
                buff2 = pFichero;
            }
            finally
            {
                memoryStream.Close();
            }

            return buff2;
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
