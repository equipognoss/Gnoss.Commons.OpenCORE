using Es.Riam.Gnoss.Util.General;
using Es.Riam.InterfacesOpenArchivos;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.FileManager
{
    public class GestionArchivos
    {
        #region Constructores
        private readonly LoggingService _loggingService;
        private readonly IUtilArchivos _utilArchivos;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public GestionArchivos(LoggingService loggingService, IUtilArchivos utilArchivos, ILogger<GestionArchivos> logger, ILoggerFactory loggerFactory)
        {
            _loggingService = loggingService;
            _utilArchivos = utilArchivos;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        public GestionArchivos(LoggingService loggingService, IUtilArchivos utilArchivos, ILogger<GestionArchivos> logger, ILoggerFactory loggerFactory, string pRutaArchivos = null, string pAzureStorageConnectionString = null)
        {
            _loggingService = loggingService;
            _utilArchivos = utilArchivos;
            RutaFicheros = pRutaArchivos;
            AzureStorageConnectionString = pAzureStorageConnectionString;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Devuelve el contenido de un fichero situado en la ruta pasada como parámetro
        /// </summary>
        /// <param name="pRuta">Ruta</param>
        /// <returns></returns>
        public async Task<byte[]> DescargarFichero(string pRuta, string pNombreArchivo, bool pArchivoEncriptado = false)
        {
            byte[] contenido = null;
            string ruta = "";
            if (string.IsNullOrEmpty(AzureStorageConnectionString))
            {
                ruta = Path.Combine(RutaFicheros, pRuta, pNombreArchivo);

                FileStream fileStream = null;

                if (File.Exists(ruta))
                {
                    try
                    {
                        //Leo el fichero
                        fileStream = new FileStream(ruta, FileMode.Open, FileAccess.Read);
                        int len = (int)fileStream.Length;
                        contenido = new byte[len];
                        fileStream.Read(contenido, 0, len);

                        if (pArchivoEncriptado)
                        {
                            contenido = _utilArchivos.DesencriptarArchivo(contenido);
                        }
                    }
                    catch (Exception ex)
                    {
                        _loggingService.GuardarLogError(ex, $"Error al descargar el fichero {pNombreArchivo} en {pRuta}. Ruta ficheros: {RutaFicheros}. Ruta completa: {ruta}", mlogger);
                        throw;
                    }
                    finally
                    {
                        if (fileStream != null)
                        {
                            fileStream.Close();
                        }
                    }
                }
                else
                {
                    _loggingService.GuardarLog($"No existe el recurso {pNombreArchivo} en {pRuta}. Ruta ficheros: {RutaFicheros}. Ruta completa: {ruta}", mlogger);
                }
            }
            else
            {
                //contenido = await AzureStorageClient.DescargarDocumento(pRuta, pNombreArchivo);

                //if (pArchivoEncriptado)
                //{
                //    contenido = _utilArchivos.DesencriptarArchivo(contenido);
                //    //contenido = UtilArchivos.DesencriptarArchivo(contenido);
                //}
            }

            if (contenido == null || contenido.Length == 0)
            {
                _loggingService.GuardarLogError($"DescargarFichero: No se ha encontrado el fichero {pNombreArchivo} en {pRuta}. Ruta ficheros: {RutaFicheros}. Ruta completa: {ruta}", mlogger);
            }

            //Envío el contenido.
            return contenido;
        }

        /// <summary>
        /// Devuelve el contenido de un fichero situado en la ruta pasada como parámetro
        /// </summary>
        /// <param name="pRuta">Ruta</param>
        /// <returns></returns>
        public void EscribirFicheroResponse(HttpResponse httpResponse, string pRuta, string pNombreArchivo, string pExtension, bool pArchivoEncriptado = false)
        {
            string ruta = "";
            _loggingService.GuardarLog($"parametros de la llamada RutaFicheros: {RutaFicheros} -- pRuta: {pRuta} -- pNombreArchivo: {pNombreArchivo} -- pExtension: {pExtension}", mlogger);
            ruta = Path.Combine(RutaFicheros, pRuta, pNombreArchivo + pExtension);
            _loggingService.GuardarLog($"Ruta al hacer el Path.Combine: {ruta}", mlogger);
            FileStream fileStream = null;
            string rutaAux = "";
            if (File.Exists(ruta))
            {
                try
                {
                    //Leo el fichero
                    fileStream = new FileStream(ruta, FileMode.Open, FileAccess.Read);
                    byte[] buffer = new byte[1048576];
                    fileStream.Seek(0, SeekOrigin.Begin);
                    FileStream fileStreamAux = null;
                    CryptoStream cryptoStream = null;
                    cryptoStream = _utilArchivos.ObtenerDesencriptador(fileStream);
                    if (cryptoStream == null)
                    {
                        pArchivoEncriptado = false;
                    }
                    int num = 0;
                    try
                    {
                        if (pArchivoEncriptado)
                        {
                            rutaAux = Path.Combine(RutaFicheros, pRuta, $"{pNombreArchivo}_aux{pExtension}");
                            try
                            {
                                fileStreamAux = new FileStream(rutaAux, FileMode.OpenOrCreate, FileAccess.Write);
                                cryptoStream.CopyTo(fileStreamAux, buffer.Length);
                                fileStreamAux.Flush();
                                fileStreamAux.Close();
                                fileStream = new FileStream(rutaAux, FileMode.Open, FileAccess.Read);
                                pArchivoEncriptado = false;
                                cryptoStream.Close();
                            }
                            catch (Exception ex)
                            {
                                _loggingService.GuardarLogError($"Ha ocurrido un error en el intento de desencriptado: {ex.Message}", mlogger);
                                throw;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _loggingService.GuardarLogError(ex.Message, mlogger);

                        pArchivoEncriptado = false;
                        cryptoStream.Close();
                        if (fileStreamAux != null)
                        {
                            fileStreamAux.Flush();
                            fileStreamAux.Close();
                        }
                        fileStream = new FileStream(ruta, FileMode.Open, FileAccess.Read);

                    }

                    if (!pArchivoEncriptado)
                    {
                        while ((num = fileStream.Read(buffer, 0, 1048576)) > 0)
                        {
                            httpResponse.Body.WriteAsync(buffer, 0, num);
                            //httpResponse.Body.Flush();
                        }
                        fileStream.Close();
                    }

                    if (!string.IsNullOrEmpty(rutaAux))
                    {
                        try
                        {
                            File.Delete(rutaAux);
                        }
                        catch (Exception) { }
                    }

                }
                catch (Exception ex)
                {
                    _loggingService.GuardarLogError(ex, $"Error al descargar el fichero {pNombreArchivo} en {pRuta}. Ruta ficheros: {RutaFicheros}. Ruta completa: {ruta}", mlogger);
                    throw;
                }
                finally
                {
                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }
                }
            }
            else
            {
                _loggingService.GuardarLog($"No existe el recurso {pNombreArchivo} en {pRuta}. Ruta ficheros: {RutaFicheros}. Ruta completa: {ruta}", mlogger);
            }

            //if (contenido == null || contenido.Length == 0)
            //{
            //    _loggingService.GuardarLogError($"DescargarFichero: No se ha encontrado el fichero {pNombreArchivo} en {pRuta}. Ruta ficheros: {RutaFicheros}. Ruta completa: {ruta}");
            //}

        }

        public void MoverContenidoDirectorio(string pOrigen, string pDestino)
        {
            _loggingService.AgregarEntrada($"Entra en MoverContenidoDirectorio({pOrigen}, {pDestino})");
            if (!Directory.Exists(pDestino))
            {
                _loggingService.AgregarEntrada($"No existe el directorio de destino '{pDestino}', lo creamos");
                Directory.CreateDirectory(pDestino);
            }
            _loggingService.AgregarEntrada($"Movemos los ficheros");
            // Mover los archivos del directorio raíz
            foreach (string file in Directory.GetFiles(pOrigen))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(pDestino, fileName);
                _loggingService.AgregarEntrada($"Vamos a copiar el fichero {file} a {destFile}");
                // Mover el archivo (copiar y eliminar)
                File.Copy(file, destFile, overwrite: true);
                _loggingService.AgregarEntrada($"Fichero copiado, ahora lo eliminamos");
                File.Delete(file);
                _loggingService.AgregarEntrada($"Fichero eliminado");
            }
            _loggingService.AgregarEntrada($"Movemos los directorios");
            // Mover los subdirectorios de forma recursiva
            foreach (string directory in Directory.GetDirectories(pOrigen))
            {
                string dirName = Path.GetFileName(directory);
                string destDir = Path.Combine(pDestino, dirName);
                _loggingService.AgregarEntrada($"Vamos a copiar el directorio {dirName} a {destDir}");
                // Llamada recursiva para mover subdirectorios
                MoverContenidoDirectorio(directory, destDir);
                _loggingService.AgregarEntrada($"Eliminamos el directorio {directory} despues de moverlo");
                // Eliminar el directorio vacío original
                Directory.Delete(directory);
                _loggingService.AgregarEntrada($"Directorio eliminado");
            }
        }

        /// <summary>
        /// Devuelve el contenido de un fichero situado en la ruta pasada como parámetro sin encriptar.
        /// </summary>
        /// <param name="pRuta">Ruta</param>
        /// <returns>Bytes del fichero</returns>
        public async Task<byte[]> DescargarFicheroSinEncriptar(string pRuta, string pNombreArchivo)
        {
            byte[] contenido = null;

            if (string.IsNullOrEmpty(AzureStorageConnectionString))
            {
                pRuta = Path.Combine(RutaFicheros, pRuta, pNombreArchivo);

                FileStream fileStream = null;

                if (File.Exists(pRuta))
                {
                    try
                    {
                        //Leo el fichero
                        fileStream = new FileStream(pRuta, FileMode.Open, FileAccess.Read);
                        int len = (int)fileStream.Length;
                        contenido = new Byte[len];
                        fileStream.Read(contenido, 0, len);
                    }
                    catch (Exception ex)
                    {
                        _loggingService.GuardarLogError(ex, $"Error al descargar el fichero {pNombreArchivo} en {pRuta}", mlogger);
                        throw;
                    }
                    finally
                    {
                        if (fileStream != null)
                        {
                            fileStream.Close();
                        }
                    }
                }
            }
            else
            {
                //contenido = await AzureStorageClient.DescargarDocumento(pRuta, pNombreArchivo);
            }

            if (contenido == null || contenido.Length == 0)
            {
                _loggingService.GuardarLogError($"DescargarFicheroSinEncriptar: No se ha encontrado el fichero {pNombreArchivo} en {pRuta}", mlogger);
            }

            //Envío el contenido.
            return contenido;
        }

        /// <summary>
        /// Obtiene la información relevante sobre todos los ficheros de un directorio
        /// </summary>
        /// <param name="pRuta">Ruta del directorio</param>
        /// <param name="pFiltroPorNombre">Nombre o filtro de búsqueda de uno  o más ficheros</param>
        /// <returns></returns>
        public async Task<List<FileInfoModel>> ObtenerInformacionFicherosDeDirectorio(string pRuta)
        {
            string rutaDocumentos = Path.Combine(ObtenerRutaDocumentos(), pRuta);
            pRuta = Path.Combine(RutaFicheros, pRuta);

            DirectoryInfo directoryInfoImage = new DirectoryInfo(pRuta);

            FileInfo[] ficherosDirectorio = directoryInfoImage.GetFiles();

            List<FileInfoModel> resultado = new List<FileInfoModel>();
            foreach (FileInfo fichero in ficherosDirectorio)
            {
                FileInfoModel fileInfoModel = new FileInfoModel();
                fileInfoModel.create_date = fichero.CreationTime;
                fileInfoModel.file_name = fichero.Name;
                fileInfoModel.size = fichero.Length;
                /*if (fichero.Extension == ".png" || fichero.Extension == ".jpg" || fichero.Extension == ".gif" || fichero.Extension == ".jpeg")
                {
                    
                    byte[] imageBytes = File.ReadAllBytes(fichero.FullName);
                    Image image = UtilImages.ConvertirArrayBytesEnImagen(imageBytes);
                    fileInfoModel.width = image.Width;
                    fileInfoModel.height = image.Height;
                }*/

                resultado.Add(fileInfoModel);
            }

            return resultado;
        }

        public void Descomprimir(Byte[] pBytesZip, string pRuta)
        {
            if (!string.IsNullOrEmpty(AzureStorageConnectionString))
            {
                using (var compressedFileStream = new MemoryStream(pBytesZip))
                {
                    using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Read, false))
                    {
                        foreach (ZipArchiveEntry entry in zipArchive.Entries)
                        {
                            Stream stream = entry.Open();
                            StreamReader streamReader = new StreamReader(stream);
                            var bytes = default(byte[]);

                            using (var memstream = new MemoryStream())
                            {
                                var buffer = new byte[2048];
                                var bytesRead = default(int);
                                while ((bytesRead = streamReader.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                                    memstream.Write(buffer, 0, bytesRead);
                                bytes = memstream.ToArray();
                            }

                            //AzureStorageClient.DescomprimirFichero(pRuta, entry.FullName, bytes);
                        }
                    }
                }
            }
        }

        public async Task<bool> ComprobarExisteDirectorio(string pRuta)
        {
            if (string.IsNullOrEmpty(AzureStorageConnectionString))
            {
                pRuta = Path.Combine(RutaFicheros, pRuta);
                _loggingService.GuardarLogError($"Comprobamos si existe la ruta {pRuta}", mlogger);
                return Directory.Exists(pRuta);
            }
            else
            {
                //return await AzureStorageClient.ExisteDirectorio(pRuta);
                return false;
            }
        }

        public bool EliminarDirectorio(string pRuta)
        {
            if (string.IsNullOrEmpty(AzureStorageConnectionString))
            {
                pRuta = Path.Combine(RutaFicheros, pRuta);
                _loggingService.GuardarLogError($"Comprobamos si existe el directorio {pRuta}", mlogger);
                if (Directory.Exists(pRuta))
                {
                    _loggingService.GuardarLogError($"Eliminamos el directorio {pRuta}", mlogger);
                    Directory.Delete(pRuta, true);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> ComprobarExisteArchivo(string pRuta, string pArchivo)
        {
            if (string.IsNullOrEmpty(AzureStorageConnectionString))
            {
                pRuta = Path.Combine(RutaFicheros, pRuta, pArchivo);
                return File.Exists(pRuta);
            }
            else
            {
                //return await AzureStorageClient.ExisteFichero(pRuta, pArchivo);
                return false;
            }
        }

        public void CrearDirectorioFisico(string pRuta)
        {
            CrearDirectorioFisico(pRuta, false);
        }

        public void CrearDirectorioFisico(string pRuta, bool pEsAbsoluta)
        {
            try
            {
                if (string.IsNullOrEmpty(AzureStorageConnectionString))
                {
                    if (!pEsAbsoluta)
                    {
                        pRuta = Path.Combine(RutaFicheros, pRuta);
                    }
                    Directory.CreateDirectory(pRuta);
                }
                else
                {
                    //AzureStorageClient.CrearDirectorio(pRuta);
                }
            }
            catch (Exception ex)
            {
                string mensajeExtra = $"Error al crear el directorio: {pRuta}";
                _loggingService.GuardarLogError(ex, mensajeExtra, mlogger);
                throw new FileManagerException(mensajeExtra, ex);
            }
        }

        public void CrearFicheroFisico(string pRuta, string pNombreArchivo, byte[] pBytes, bool pEncriptarFichero = false)
        {
            try
            {
                if (string.IsNullOrEmpty(AzureStorageConnectionString))
                {
                    if (!string.IsNullOrEmpty(pRuta))
                    {
                        pRuta = Path.Combine(RutaFicheros, TransformarRuta(pRuta));
                    }
                    else
                    {
                        pRuta = RutaFicheros;
                    }

                    FileInfo infoFichero = new FileInfo(Path.Combine(pRuta, pNombreArchivo));
                    _loggingService.AgregarEntrada($"Entra en GestionArchivos.CrearFicheroFisico con la imagen: \n\t -Nombre: {pNombreArchivo} \n\t -Ruta: {pRuta} \n\t -Tamaño fichero: {pBytes.Length}");

                    if (!infoFichero.Directory.Exists)
                    {
                        _loggingService.AgregarEntrada($"El directorio {infoFichero.Directory.FullName} no existe, se procede a crearse");
                        infoFichero.Directory.Create();
                    }

                    if (pEncriptarFichero)
                    {
                        _loggingService.AgregarEntrada($"Se encripta el fichero {infoFichero.Name}");
                        pBytes = _utilArchivos.EncriptarArchivo(pBytes);
                    }
                    FileStream fileStream = new FileStream(infoFichero.FullName, FileMode.Create, FileAccess.Write);
                    fileStream.Write(pBytes, 0, pBytes.Length);
                    fileStream.Flush();
                    fileStream.Close();
                    _loggingService.AgregarEntrada($"Se escribe correctamente el fichero {infoFichero.Name}");
                }
                else
                {
                    //AzureStorageClient.SubirDocumento(pRuta, pNombreArchivo, pBytes);
                }
            }
            catch (Exception ex)
            {
                string mensajeExtra = $"Error al crear el fichero {pNombreArchivo} en la ruta {pRuta}";
                _loggingService.GuardarLogError(ex, mensajeExtra, mlogger);
                throw new FileManagerException(mensajeExtra, ex);
            }
        }

        public void CrearFicheroFisicoDesdeStream(string pRuta, string pNombreArchivo, Stream stream, bool pEncriptarFichero = false)
        {
            try
            {
                _loggingService.GuardarLogError("Se va a subir el fichero", mlogger);
                if (!string.IsNullOrEmpty(pRuta))
                {
                    pRuta = Path.Combine(RutaFicheros, TransformarRuta(pRuta));
                }
                else
                {
                    pRuta = RutaFicheros;
                }

                FileInfo infoFichero = new FileInfo(Path.Combine(pRuta, pNombreArchivo));

                if (!infoFichero.Directory.Exists)
                {
                    infoFichero.Directory.Create();
                }
                FileStream fileStream = new FileStream(infoFichero.FullName, FileMode.Create, FileAccess.Write);
                byte[] buffer = new byte[1048576];
                stream.Seek(0, SeekOrigin.Begin);
                int num = 0;
                CryptoStream cryptoStream = null;
                cryptoStream = _utilArchivos.ObtenerEncriptador(fileStream);
                if (cryptoStream == null)
                {
                    pEncriptarFichero = false;
                }

                if (pEncriptarFichero)
                {
                    while ((num = stream.Read(buffer, 0, 1048576)) > 0)
                    {
                        cryptoStream.Write(buffer, 0, num);
                    }
                }
                else
                {
                    while ((num = stream.Read(buffer, 0, 1048576)) > 0)
                    {
                        fileStream.Write(buffer, 0, num);
                        fileStream.Flush();
                    }
                }
                if (pEncriptarFichero)
                {
                    cryptoStream.Flush();
                    cryptoStream.FlushFinalBlock();
                }
                fileStream.Close();

            }
            catch (Exception ex)
            {
                string mensajeExtra = $"Error al crear el fichero {pNombreArchivo} en la ruta {pRuta}";
                _loggingService.GuardarLogError(ex, mensajeExtra, mlogger);
                throw new FileManagerException(mensajeExtra, ex);
            }
        }

        public void CrearFicheroFisico(string pRuta, string pNombreArchivo, Stream pStream, bool pEncriptarFichero = false)
        {
            try
            {
                if (string.IsNullOrEmpty(AzureStorageConnectionString))
                {
                    throw new Exception("La configuración de 'AzureStorageConnectionString' no puede ser vacía. Este método sólo se puede usar en Azure.");
                }
                else
                {
                    //AzureStorageClient.SubirDocumento(pRuta, pNombreArchivo, pStream);
                }
            }
            catch (Exception ex)
            {
                throw new FileManagerException($"Error al crear el fichero {pNombreArchivo} en la ruta {pRuta}", ex);
            }
        }



        /// <summary>
        /// Crear el fichero pasado como parámetro sin encriptar.
        /// </summary>
        /// <param name="pRuta">Ruta del fichero</param>
        /// <param name="pNombreArchivo">Nombre del fichero</param>
        /// <param name="pBytes">Bytes del fichero</param>
        public void CrearFicheroFisicoSinEncriptar(string pRuta, string pNombreArchivo, byte[] pBytes)
        {
            if (string.IsNullOrEmpty(AzureStorageConnectionString))
            {
                pRuta = Path.Combine(RutaFicheros, pRuta);

                FileStream fileStream = new FileStream(Path.Combine(pRuta, pNombreArchivo), FileMode.Create, FileAccess.Write);
                fileStream.Write(pBytes, 0, pBytes.Length);
                fileStream.Flush();
                fileStream.Close();
            }
            else
            {
                //AzureStorageClient.SubirDocumento(pRuta, pNombreArchivo, pBytes);
            }

        }

        public async Task<string[]> ObtenerFicherosDeDirectorioYSubDirectorios(string pRuta, string pFiltroPorNombre = null)
        {
            if (string.IsNullOrEmpty(AzureStorageConnectionString))
            {
                pRuta = Path.Combine(RutaFicheros, pRuta);
                GuardarLogTest("La cadena de conexion a RutaFicheros es: " + RutaFicheros);
                GuardarLogTest("La cadena de conexion a de la ruta es: " + pRuta);


                DirectoryInfo directoryInfo = new DirectoryInfo(pRuta);

                FileInfo[] resultado = null;

                if (!string.IsNullOrEmpty(pFiltroPorNombre))
                {
                    resultado = directoryInfo.GetFiles(pFiltroPorNombre + "*", SearchOption.AllDirectories);
                }
                else
                {
                    resultado = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
                }

                return resultado.Select(fichero => fichero.FullName.Replace(pRuta + Path.DirectorySeparatorChar, "")).ToArray();
            }
            else
            {
                //TODO: esto no se si funcionara en azure
                //var resultado = await AzureStorageClient.ObtenerFicherosDeDirectorioYSubDirectorios(pRuta, pRuta);

                //if (!string.IsNullOrEmpty(pFiltroPorNombre))
                //{
                //    resultado = resultado.Where(fichero => fichero.StartsWith(pFiltroPorNombre)).ToArray();
                //}

                //return resultado;
                return null;
            }

        }
        public static void GuardarLogTest(string message)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "error_servicioInterno.txt"), true, System.Text.Encoding.Default))
                {
                    sw.WriteLine(Environment.NewLine + "Fecha: " + DateTime.Now + Environment.NewLine + Environment.NewLine);
                    // Escribo el error
                    sw.WriteLine(message);
                }
            }
            catch (Exception ex)
            { }
        }

        public async Task<string[]> ObtenerFicherosDeDirectorio(string pRuta, string pFiltroPorNombre = null)
        {
            if (string.IsNullOrEmpty(AzureStorageConnectionString))
            {
                pRuta = Path.Combine(RutaFicheros, pRuta);

                DirectoryInfo directoryInfo = new DirectoryInfo(pRuta);

                FileInfo[] resultado = null;

                if (!string.IsNullOrEmpty(pFiltroPorNombre))
                {
                    resultado = directoryInfo.GetFiles(pFiltroPorNombre + "*");
                }
                else
                {
                    resultado = directoryInfo.GetFiles();
                }

                return resultado.Select(fichero => fichero.Name).ToArray();
            }
            else
            {
                //var resultado = await AzureStorageClient.ObtenerFicherosDeDirectorio(pRuta);

                //if (!string.IsNullOrEmpty(pFiltroPorNombre))
                //{
                //    resultado = resultado.Where(fichero => fichero.StartsWith(pFiltroPorNombre)).ToArray();
                //}

                //return resultado;
                return null;
            }
        }

        /// <summary>
        /// Obtiene la información relevante sobre todos los ficheros de un directorio
        /// </summary>
        /// <param name="pRuta">Ruta del directorio</param>
        /// <param name="pFiltroPorNombre">Nombre o filtro de búsqueda de uno  o más ficheros</param>
        /// <returns></returns>
        public async Task<List<FileInfoModel>> ObtenerInformacionFicherosDeDirectorio(string pRuta, string pFiltroPorNombre = null)
        {
            string rutaDocumentos = Path.Combine(ObtenerRutaDocumentos(), pRuta);
            pRuta = Path.Combine(RutaFicheros, pRuta);

            DirectoryInfo directoryInfoImage = new DirectoryInfo(pRuta);
            DirectoryInfo directoryInfoDockLink = new DirectoryInfo(rutaDocumentos);
            FileInfo[] ficherosDirectorio = null;

            if (!string.IsNullOrEmpty(pFiltroPorNombre))
            {
                ficherosDirectorio = directoryInfoImage.GetFiles($"{pFiltroPorNombre}*");
                ficherosDirectorio = ficherosDirectorio.Union(directoryInfoDockLink.GetFiles($"{pFiltroPorNombre}*")).ToArray();
            }
            else
            {
                ficherosDirectorio = directoryInfoImage.GetFiles();
                ficherosDirectorio = ficherosDirectorio.Union(directoryInfoDockLink.GetFiles()).ToArray();
            }

            List<FileInfoModel> resultado = new List<FileInfoModel>();
            foreach (FileInfo fichero in ficherosDirectorio)
            {
                FileInfoModel fileInfoModel = new FileInfoModel();
                fileInfoModel.create_date = fichero.CreationTime;
                fileInfoModel.file_name = fichero.Name;
                fileInfoModel.size = fichero.Length;
                if (fichero.Extension == ".png" || fichero.Extension == ".jpg" || fichero.Extension == ".gif")
                {
                    byte[] imageBytes = File.ReadAllBytes(fichero.FullName);
                    SixLabors.ImageSharp.Image image = UtilImages.ConvertirArrayBytesEnImagen(imageBytes);
                    fileInfoModel.width = image.Width;
                    fileInfoModel.height = image.Height;
                }

                resultado.Add(fileInfoModel);
            }

            return resultado;
        }

        public string ObtenerRutaDirectorioZip(string pRuta)
        {
            pRuta = Path.Combine(RutaFicheros, pRuta);
            return pRuta;
        }

        public async Task<string[]> ObtenerSubdirectoriosDeDirectorio(string pRuta)
        {
            if (string.IsNullOrEmpty(AzureStorageConnectionString))
            {
                pRuta = Path.Combine(RutaFicheros, pRuta);

                DirectoryInfo directoryInfo = new DirectoryInfo(pRuta);

                return directoryInfo.GetDirectories().Select(directorio => directorio.Name).ToArray();
            }
            else
            {
                //return await AzureStorageClient.ObtenerSubdirectoriosDeDirectorio(pRuta);
                return new string[] { };
            }
        }

        public void EliminarFicherosDirectorio(string pRuta)
        {
            pRuta = Path.Combine(RutaFicheros, pRuta);
            DirectoryInfo dirInfoRaiz = new DirectoryInfo(pRuta);
            FileInfo[] ficheros = dirInfoRaiz.GetFiles();
            _loggingService.GuardarLogError($"Hay {ficheros.Length} ficheros en el directorio {pRuta}", mlogger);
            foreach (FileInfo fichero in ficheros)
            {
                if (fichero.Exists)
                {
                    _loggingService.GuardarLogError($"Eliminamos el fichero {fichero.FullName}", mlogger);
                    fichero.Delete();
                }
            }
            _loggingService.GuardarLogError($"Fin EliminarFicherosDirectorio", mlogger);
        }

        public void EliminarDirectorioEnCascada(string pRuta)
        {
            if (string.IsNullOrEmpty(AzureStorageConnectionString))
            {
                pRuta = Path.Combine(RutaFicheros, pRuta);
                DirectoryInfo dirInfoRaiz = new DirectoryInfo(pRuta);
                DirectoryInfo[] directorios = dirInfoRaiz.GetDirectories();
                foreach (DirectoryInfo dir in directorios)
                {
                    if (dir.Exists)
                    {
                        dir.Delete(true);
                    }
                }
            }
            else
            {
                //AzureStorageClient.EliminarDirectorioEnCascada(pRuta);
            }
        }

        public void EliminarFicheroFisico(string pRuta, string pNombreArchivo)
        {
            if (string.IsNullOrEmpty(AzureStorageConnectionString))
            {
                pRuta = Path.Combine(RutaFicheros, pRuta);

                FileInfo archivo = new FileInfo(Path.Combine(pRuta, pNombreArchivo));

                if (archivo.Exists)
                {
                    archivo.Delete();
                }
            }
            else
            {
                //AzureStorageClient.EliminarFicheroFisico(pRuta, pNombreArchivo);
            }
        }

        /// <summary>
        /// Se encarga de copiar o mover un archivo de la ruta origen indicada a la ruta destino indicada.
        /// En caso de que el fichero se pase sin extensión, se copiarán todos los ficheros del directorio origen al directorio destino.
        /// </summary>
        /// <param name="pRutaOrigen">Ruta donde se ubica el fichero a copiar</param>
        /// <param name="pRutaDestino">Ruta donde queremos copiar el fichero</param>
        /// <param name="pNombreArchivoOrigen">Nombre del archivo, puede llevar la extensión</param>
        /// <param name="pCopiar">Si queremos copiarlo, en caso de ser false, se moverá</param>
        /// <param name="pExtension">Extensión del archivo. Puede no tener porque está en el nombre o porque se quieren copiar todos los archivos que comiencen por ese texto</param>
        /// <param name="pNombreArchivoDestino">Nombre que se le quiere dar al archivo en el destino, si no se indica se le pondrá el nombre del origen</param>
        /// <param name="pSobreEscribir">Si se quiere sobreescribir los archivos en caso de haberlos en el destino</param>
        public void CopiarArchivo(string pRutaOrigen, string pRutaDestino, string pNombreArchivoOrigen, bool pCopiar, string pExtension = null, string pNombreArchivoDestino = null, bool pSobreEscribir = false)
        {
            if (string.IsNullOrEmpty(pNombreArchivoDestino))
            {
                pNombreArchivoDestino = pNombreArchivoOrigen;
            }

            if (!string.IsNullOrEmpty(pExtension))
            {
                if (!pNombreArchivoOrigen.EndsWith(pExtension))
                {
                    pNombreArchivoOrigen = pNombreArchivoOrigen + pExtension;
                }
                if (!pNombreArchivoDestino.EndsWith(pExtension))
                {
                    pNombreArchivoDestino = pNombreArchivoDestino + pExtension;
                }
            }

            if (string.IsNullOrEmpty(AzureStorageConnectionString))
            {
                pRutaOrigen = Path.Combine(RutaFicheros, pRutaOrigen);
                pRutaDestino = Path.Combine(RutaFicheros, pRutaDestino);

                if (Directory.Exists(pRutaOrigen))
                {
                    if (!Directory.Exists(pRutaDestino))
                    {
                        Directory.CreateDirectory(pRutaDestino);
                    }

                    if (string.IsNullOrEmpty(Path.GetExtension(pNombreArchivoOrigen)))
                    {
                        string[] filesNames = Directory.GetFiles(pRutaOrigen).Where(item => Path.GetFileName(item).StartsWith(pNombreArchivoOrigen)).ToArray();

                        foreach(string fileName in filesNames)
                        {
                            string rutaOrigen = Path.Combine(pRutaOrigen, Path.GetFileName(fileName));
                            string rutaDestino = Path.Combine(pRutaDestino, Path.GetFileName(fileName));

                            CopiarMoverArchivo(rutaOrigen, rutaDestino, pCopiar, pSobreEscribir);
                        }
                    }
                    else
                    {
                        pRutaOrigen = Path.Combine(pRutaOrigen, pNombreArchivoOrigen);
                        pRutaDestino = Path.Combine(pRutaDestino, pNombreArchivoDestino);

                        CopiarMoverArchivo(pRutaOrigen, pRutaDestino, pCopiar, pSobreEscribir);
                    }  
                }               
            }
            else
            {
                //AzureStorageClient.CopiarArchivo(pRutaOrigen, pRutaDestino, pNombreArchivoOrigen, pCopiar, pNombreArchivoDestino);
            }
        }

        private void CopiarMoverArchivo(string pRutaArchivoOrigen, string pRutaArchivoDestino, bool pCopiar, bool pSobreEscribir = false)
        {
            FileInfo fichOrigen = new FileInfo(pRutaArchivoOrigen);
            if (pCopiar)
            {//Copiar:
                fichOrigen.CopyTo(pRutaArchivoDestino, pSobreEscribir);
            }
            else
            {//Cortar:
                fichOrigen.MoveTo(pRutaArchivoDestino, pSobreEscribir);
            }
        }

        public void MoverArchivo(string pRutaOrigen, string pRutaDestino, bool pIgnorarDirectorioPrincipal = false)
        {
            string rutaOrigen = pRutaOrigen;
            string rutaDestino = pRutaDestino;

            if (!pIgnorarDirectorioPrincipal)
            {
                rutaOrigen = Path.Combine(RutaFicheros, pRutaOrigen);
                rutaDestino = Path.Combine(RutaFicheros, pRutaDestino);
            }

            FileInfo fichOrigen = new FileInfo(rutaOrigen);
            fichOrigen.MoveTo(rutaDestino, true);
        }

        public List<string> ObtenerFicherosDeDirectorioRecurso(string pPath)
        {
            List<string> ficheros = new List<string>();
            if (pPath == null)
            {
                return ficheros;
            }

            if (Directory.Exists(pPath))
            {
                ficheros = new DirectoryInfo(pPath).GetFiles().Select(x => x.FullName).ToList();
            }

            return ficheros;
        }

        public List<string> ObtenerDirectoriosDeDirectorio(string pPath)
        {
            List<string> folders = new List<string>();
            if (pPath == null)
            {
                return folders;
            }

            if (Directory.Exists(pPath))
            {
                folders = new DirectoryInfo(pPath).GetDirectories().Select(x => x.FullName).ToList();
            }

            return folders;
        }

        public static string ObtenerRutaFicherosDeRecursosTemporal(Guid pDocumentoID)
        {
            string ruta = Path.Combine(UtilArchivos.ContentTemporales, DateTime.Now.ToString("yyyyMMdd"), pDocumentoID.ToString().ToLower(), DateTime.Now.ToString("HHmmss"));
            return ruta;
        }

        public void MoverDirectorioDeRecursoAAlmacenamientoTemporal(string pRutaBase, string pDirectorioOrigen, string pRutaBaseTemporal)
        {
            string rutaOrigen = Path.Combine(pRutaBase, pDirectorioOrigen);
            string rutaDestino = Path.Combine(pRutaBaseTemporal, rutaOrigen);

            Directory.Move(rutaOrigen, rutaDestino);
        }

        public bool EsFicheroValido(string file, List<string> validResourceFiles)
        {
            string ficheroServidor = file.Substring(0, file.LastIndexOf('.'));
            foreach (string fich in validResourceFiles)
            {
                string ficheroUsadoEnRecurso = fich.Substring(0, fich.LastIndexOf('.'));
                if (ficheroServidor.Contains(ficheroUsadoEnRecurso))
                {
                    return true;
                }
            }

            return false;
        }

        public bool EsOpenSeaDragonValido(string folder, List<string> validResourceFiles)
        {
            string _regexIdOpenSeadragonFolder = "\\/[a-f0-9]{2}\\/[a-f0-9]{4}\\/[a-f0-9\\-]{36}\\/([a-f0-9\\-]{36})";
            if (Regex.IsMatch(folder, _regexIdOpenSeadragonFolder))
            {
                string idImage = Regex.Match(folder, _regexIdOpenSeadragonFolder).Groups[1].Value;
                return validResourceFiles.Any(x => x.Contains(idImage));
            }
            return false;
        }

        public bool EsMiniaturaValida(string file, List<string> validResourceFiles)
        {
            string _regexIdImagesemThumbnail = "\\/[a-f0-9]{2}\\/[a-f0-9]{4}\\/[a-f0-9\\-]{36}\\/([a-f0-9\\-]{36})_[0-9]+.[a-z0-9]+";
            if (Regex.IsMatch(file, _regexIdImagesemThumbnail))
            {
                string idImage = Regex.Match(file, _regexIdImagesemThumbnail).Groups[1].Value;
                return validResourceFiles.Any(x => x.Contains(idImage));
            }
            return false;
        }

        public async void CopiarArchivosDeDirectorio(string pRutaOrigen, string pRutaDestino, bool pCopiarSubdirectorios = false, bool pSobreEsribir = false)
        {
            bool existeRutaOrigen = await ComprobarExisteDirectorio(pRutaOrigen);
            if (existeRutaOrigen)
            {
                string[] ficheros = await ObtenerFicherosDeDirectorio(pRutaOrigen);

                if (ficheros.Length > 0)
                {
                    bool existeRutaDestino = await ComprobarExisteDirectorio(pRutaDestino);
                    if (!existeRutaDestino)
                    {
                        CrearDirectorioFisico(pRutaDestino);
                    }

                    foreach (string fichero in ficheros)
                    {
                        CopiarArchivo(pRutaOrigen, pRutaDestino, fichero, true, pSobreEscribir: pSobreEsribir);
                    }
                }

                if (pCopiarSubdirectorios)
                {
                    string[] directorios = await ObtenerSubdirectoriosDeDirectorio(pRutaOrigen);
                    foreach (string directorio in directorios)
                    {
                        if (!directorio.Equals("historial"))
                        {
                            CopiarArchivosDeDirectorio(Path.Combine(pRutaOrigen, directorio), Path.Combine(pRutaDestino, directorio), true, pSobreEsribir);
                        }
                    }
                }
            }
        }

        public async Task<long> ObtenerTamanioArchivo(string pRuta, string pNombreArchivo)
        {
            if (string.IsNullOrEmpty(AzureStorageConnectionString))
            {
                pRuta = Path.Combine(RutaFicheros, pRuta);

                FileInfo archivo = new FileInfo(Path.Combine(pRuta, pNombreArchivo));
                long espacio = 0;
                if (archivo.Exists)
                {
                    espacio = archivo.Length;
                }

                return espacio;
            }
            else
            {
                //return await AzureStorageClient.ObtenerTamanioArchivo(pRuta, pNombreArchivo);
                return 0;
            }
        }

        public async Task<DateTime?> ObtenerFechaCreacionArchivo(string pRuta, string pNombreArchivo)
        {
            if (string.IsNullOrEmpty(AzureStorageConnectionString))
            {
                pRuta = Path.Combine(RutaFicheros, pRuta, pNombreArchivo);

                return File.GetCreationTime(pRuta);
            }
            else
            {
                // No tenemos la propiedad Fecha de creación de un archivo, obtenemos la de última modificación
                //return await AzureStorageClient.ObtenerFechaUltimaModificacionArchivo(pRuta, pNombreArchivo);
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Calcula el tamaño de un directorio con y sus subdirectorios
        /// </summary>
        /// <param name="pDir"></param>
        /// <returns></returns>
        public long ObtenerTamanioDirectorio(string pRuta)
        {
            if (string.IsNullOrEmpty(AzureStorageConnectionString))
            {
                pRuta = Path.Combine(RutaFicheros, pRuta);

                DirectoryInfo dir = new DirectoryInfo(pRuta);
                return ObtenerTamanioDirectoryInfo(dir);
            }
            else
            {
                // TODO: Contar espacio en Azure
                return 0;
            }
        }

        private long ObtenerTamanioDirectoryInfo(DirectoryInfo pDir)
        {
            long size = 0;

            foreach (DirectoryInfo dir in pDir.GetDirectories())
            {
                size += ObtenerTamanioDirectoryInfo(dir);
            }

            foreach (FileInfo fich in pDir.GetFiles())
            {
                size += fich.Length;
            }

            return size;
        }

        /// <summary>
        /// Obtiene el directorio y el nombre del archivo, pasada una ruta a un archivo
        /// </summary>
        /// <param name="pNombre">Nombre del archivo, que puede contener parte de la ruta. Ej: /imagenes/proyectos/f22.png, f22.png...</param>
        /// <returns>Un array de dos posiciones, donde la primera posicion es la ruta y la segunda el nombre del archivo. Ej: /imagenes/proyectos/f22.png --> [\imagenes\proyectos, f22.png]; f22.png --> [, f22.png]</returns>
        public string[] ObtenerDirectorioYArchivoDeNombreArchivo(string pNombre)
        {
            string[] resultado = new string[2];

            if (pNombre.Contains("\\") || pNombre.Contains("/"))
            {
                if (pNombre.Contains("/") && !Path.DirectorySeparatorChar.Equals('/'))
                {
                    pNombre = pNombre.Replace('/', Path.DirectorySeparatorChar);
                }
                if (pNombre.Contains("\\") && !Path.DirectorySeparatorChar.Equals('\\'))
                {
                    pNombre = pNombre.Replace('\\', Path.DirectorySeparatorChar);
                }

                pNombre = pNombre.Trim(Path.DirectorySeparatorChar);

                resultado[0] = pNombre.Substring(0, pNombre.LastIndexOf(Path.DirectorySeparatorChar));
                resultado[1] = pNombre.Substring(pNombre.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            }
            else
            {
                resultado[0] = "";
                resultado[1] = pNombre;
            }

            return resultado;
        }

        /// <summary>
        /// Divide la ruta recibida por parametro y la vuelve a unir de manera que la adapte al sistema 
        /// operativo en el que se esta ejecutando el servicio. (Si hubiese peticiones entre un servidor Linux y otro
        /// Windows, la ruta no será la misma)
        /// </summary>
        /// <param name="pPath">Ruta a transformar</param>
        /// <returns>La ruta adaptada al sistema operativo en el que se esta ejecutando el servicio</returns>
        public static string TransformarRuta(string pPath)
        {
            string[] partesRuta = null;
            if (pPath.Contains("\\"))
            {
                partesRuta = pPath.Split("\\");
            }
            else
            {
                partesRuta = pPath.Split("/");
            }

            return Path.Combine(partesRuta);
        }

        private string ObtenerRutaDocumentos()
        {
            int indiceImagen = RutaFicheros.LastIndexOf(Path.DirectorySeparatorChar);

            return $"{RutaFicheros.Substring(0, indiceImagen)}{Path.DirectorySeparatorChar}doclinks";
        }

        #endregion

        #region Propiedades

        public string RutaFicheros
        {
            get; set;
        }

        public string AzureStorageConnectionString { get; set; }

        //TODO AzureStorage
        /*
        private AzureStorage mAzureStorageClient;

        private AzureStorage AzureStorageClient
        {
            get
            {
                if (mAzureStorageClient == null)
                {
                    mAzureStorageClient = new AzureStorage(AzureStorageConnectionString);
                    mAzureStorageClient.InitializeAzureStorage();
                }

                return mAzureStorageClient;
            }
        }
        */
        #endregion
    }

    #region Clases
    public class FileInfoModel
    {
        public string file_name { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public DateTime create_date { get; set; }
        public long size { get; set; }
    }

    public class MultimediaFileInfoModel
    {
        public FileInfoModel FileInfo { get; set; }
        public string Path { get; set; }
    }

    #endregion
}
