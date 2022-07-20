using Es.Riam.Gnoss.Util.General;
using Es.Riam.InterfacesOpenArchivos;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.FileManager
{
    public class GestionArchivos
    {
        #region Constructores
        private readonly LoggingService _loggingService;
        private readonly IUtilArchivos _utilArchivos;

        public GestionArchivos(LoggingService loggingService, IUtilArchivos utilArchivos)
        {
            _loggingService = loggingService;
            _utilArchivos = utilArchivos;
        }

        public GestionArchivos(LoggingService loggingService, IUtilArchivos utilArchivos, string pRutaArchivos = null, string pAzureStorageConnectionString = null)
        {
            _loggingService = loggingService;
            _utilArchivos = utilArchivos;
            RutaFicheros = pRutaArchivos;
            AzureStorageConnectionString = pAzureStorageConnectionString;
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
                            //contenido = UtilArchivos.DesencriptarArchivo(contenido);
                        }
                    }
                    catch (Exception ex)
                    {
                        _loggingService.GuardarLogError(ex, $"Error al descargar el fichero {pNombreArchivo} en {pRuta}. Ruta ficheros: {RutaFicheros}. Ruta completa: {ruta}");
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
                    _loggingService.GuardarLog($"No existe el recurso {pNombreArchivo} en {pRuta}. Ruta ficheros: {RutaFicheros}. Ruta completa: {ruta}");
                }
            }
            else
            {
                contenido = await AzureStorageClient.DescargarDocumento(pRuta, pNombreArchivo);

                if (pArchivoEncriptado)
                {
                    contenido = _utilArchivos.DesencriptarArchivo(contenido);
                    //contenido = UtilArchivos.DesencriptarArchivo(contenido);
                }
            }

            if (contenido == null || contenido.Length == 0)
            {
                _loggingService.GuardarLogError($"DescargarFichero: No se ha encontrado el fichero {pNombreArchivo} en {pRuta}. Ruta ficheros: {RutaFicheros}. Ruta completa: {ruta}");
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
            _loggingService.GuardarLog($"parametros de la llamada RutaFicheros: {RutaFicheros} -- pRuta: {pRuta} -- pNombreArchivo: {pNombreArchivo} -- pExtension: {pExtension}");
            ruta = Path.Combine(RutaFicheros, pRuta, pNombreArchivo + pExtension);
            _loggingService.GuardarLog($"Ruta al hacer el Path.Combine: {ruta}");
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
                    if(cryptoStream == null)
                    {
                        pArchivoEncriptado = false;
                    }
                    int num = 0;
                    try
                    {
                        if (pArchivoEncriptado)
                        {
                            rutaAux = Path.Combine(RutaFicheros, pRuta, $"{pNombreArchivo}_aux{pExtension}");
                            fileStreamAux = new FileStream(rutaAux, FileMode.OpenOrCreate, FileAccess.Write);
                            cryptoStream.CopyTo(fileStreamAux, buffer.Length);
                            fileStreamAux.Flush();
                            fileStreamAux.Close();
                            fileStream = new FileStream(rutaAux, FileMode.Open, FileAccess.Read);
                            pArchivoEncriptado = false;
                            cryptoStream.Close();
                        }
                    }
                    catch (Exception ex)
                    {
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
                    _loggingService.GuardarLogError(ex, $"Error al descargar el fichero {pNombreArchivo} en {pRuta}. Ruta ficheros: {RutaFicheros}. Ruta completa: {ruta}");
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
                _loggingService.GuardarLog($"No existe el recurso {pNombreArchivo} en {pRuta}. Ruta ficheros: {RutaFicheros}. Ruta completa: {ruta}");
            }

            //if (contenido == null || contenido.Length == 0)
            //{
            //    _loggingService.GuardarLogError($"DescargarFichero: No se ha encontrado el fichero {pNombreArchivo} en {pRuta}. Ruta ficheros: {RutaFicheros}. Ruta completa: {ruta}");
            //}

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
                        _loggingService.GuardarLogError(ex, $"Error al descargar el fichero {pNombreArchivo} en {pRuta}");
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
                contenido = await AzureStorageClient.DescargarDocumento(pRuta, pNombreArchivo);
            }

            if (contenido == null || contenido.Length == 0)
            {
                _loggingService.GuardarLogError($"DescargarFicheroSinEncriptar: No se ha encontrado el fichero {pNombreArchivo} en {pRuta}");
            }

            //Envío el contenido.
            return contenido;
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

                            AzureStorageClient.DescomprimirFichero(pRuta, entry.FullName, bytes);
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
                return Directory.Exists(pRuta);
            }
            else
            {
                return await AzureStorageClient.ExisteDirectorio(pRuta);
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
                return await AzureStorageClient.ExisteFichero(pRuta, pArchivo);
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
                    AzureStorageClient.CrearDirectorio(pRuta);
                }
            }
            catch (Exception ex)
            {
                string mensajeExtra = $"Error al crear el directorio: {pRuta}";
                _loggingService.GuardarLogError(ex, mensajeExtra);
                throw new FileManagerException(mensajeExtra, ex);
            }
        }

        public void CrearFicheroFisico(string pRuta, string pNombreArchivo, byte[] pBytes, bool pEncriptarFichero = false)
        {
            try
            {
                if (string.IsNullOrEmpty(AzureStorageConnectionString))
                {
                    _loggingService.GuardarLogError("Se va a subir el fichero");
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

                    if (pEncriptarFichero)
                    {
                        pBytes = _utilArchivos.EncriptarArchivo(pBytes);
                        //pBytes = UtilArchivos.EncriptarArchivo(pBytes);
                    }
                    _loggingService.GuardarLogError($"Nombre del fichero a crear: {infoFichero.FullName}");
                    FileStream fileStream = new FileStream(infoFichero.FullName, FileMode.Create, FileAccess.Write);
                    fileStream.Write(pBytes, 0, pBytes.Length);
                    fileStream.Flush();
                    fileStream.Close();
                    _loggingService.GuardarLogError($"Ha subido el fichero");
                }
                else
                {
                    AzureStorageClient.SubirDocumento(pRuta, pNombreArchivo, pBytes);
                }
            }
            catch (Exception ex)
            {
                string mensajeExtra = $"Error al crear el fichero {pNombreArchivo} en la ruta {pRuta}";
                _loggingService.GuardarLogError(ex, mensajeExtra);
                throw new FileManagerException(mensajeExtra, ex);
            }
        }

        public void CrearFicheroFisicoDesdeStream(string pRuta, string pNombreArchivo, Stream stream, bool pEncriptarFichero = false)
        {
            try
            {
                _loggingService.GuardarLogError("Se va a subir el fichero");
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
                _loggingService.GuardarLogError($"Nombre del fichero a crear: {infoFichero.FullName}");
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
                _loggingService.GuardarLogError($"Ha subido el fichero");

            }
            catch (Exception ex)
            {
                string mensajeExtra = $"Error al crear el fichero {pNombreArchivo} en la ruta {pRuta}";
                _loggingService.GuardarLogError(ex, mensajeExtra);
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
                    AzureStorageClient.SubirDocumento(pRuta, pNombreArchivo, pStream);
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
                AzureStorageClient.SubirDocumento(pRuta, pNombreArchivo, pBytes);
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
                var resultado = await AzureStorageClient.ObtenerFicherosDeDirectorioYSubDirectorios(pRuta, pRuta);

                if (!string.IsNullOrEmpty(pFiltroPorNombre))
                {
                    resultado = resultado.Where(fichero => fichero.StartsWith(pFiltroPorNombre)).ToArray();
                }

                return resultado;
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
                var resultado = await AzureStorageClient.ObtenerFicherosDeDirectorio(pRuta);

                if (!string.IsNullOrEmpty(pFiltroPorNombre))
                {
                    resultado = resultado.Where(fichero => fichero.StartsWith(pFiltroPorNombre)).ToArray();
                }

                return resultado;
            }
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
                return await AzureStorageClient.ObtenerSubdirectoriosDeDirectorio(pRuta);
            }
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
                AzureStorageClient.EliminarDirectorioEnCascada(pRuta);
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
                AzureStorageClient.EliminarFicheroFisico(pRuta, pNombreArchivo);
            }
        }

        public void CopiarArchivo(string pRutaOrigen, string pRutaDestino, string pNombreArchivoOrigen, bool pCopiar, string pExtension = null, string pNombreArchivoDestino = null)
        {
            if (string.IsNullOrEmpty(pNombreArchivoDestino))
            {
                pNombreArchivoDestino = pNombreArchivoOrigen;
            }

            if (!string.IsNullOrEmpty(pExtension))
            {
                pNombreArchivoOrigen = pNombreArchivoOrigen + pExtension;
                pNombreArchivoDestino = pNombreArchivoDestino + pExtension;
            }

            if (string.IsNullOrEmpty(AzureStorageConnectionString))
            {
                pRutaOrigen = Path.Combine(RutaFicheros, pRutaOrigen, pNombreArchivoOrigen);
                pRutaDestino = Path.Combine(RutaFicheros, pRutaDestino, pNombreArchivoDestino);

                FileInfo fichOrigen = new FileInfo(pRutaOrigen);
                if (pCopiar)
                {//Copiar:
                    fichOrigen.CopyTo(pRutaDestino);
                }
                else
                {//Cortar:
                    fichOrigen.MoveTo(pRutaDestino);
                }
            }
            else
            {
                AzureStorageClient.CopiarArchivo(pRutaOrigen, pRutaDestino, pNombreArchivoOrigen, pCopiar, pNombreArchivoDestino);
            }
        }

        public async void CopiarArchivosDeDirectorio(string pRutaOrigen, string pRutaDestino, bool pCopiarSubdirectorios = false)
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
                        CopiarArchivo(pRutaOrigen, pRutaDestino, fichero, true);
                    }
                }

                if (pCopiarSubdirectorios)
                {
                    string[] directorios = await ObtenerSubdirectoriosDeDirectorio(pRutaOrigen);
                    foreach (string directorio in directorios)
                    {
                        if (!directorio.Equals("historial"))
                        {
                            CopiarArchivosDeDirectorio(Path.Combine(pRutaOrigen, directorio), Path.Combine(pRutaDestino, directorio), true);
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
                return await AzureStorageClient.ObtenerTamanioArchivo(pRuta, pNombreArchivo);
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
                return await AzureStorageClient.ObtenerFechaUltimaModificacionArchivo(pRuta, pNombreArchivo);
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

        #endregion

        #region Propiedades

        public string RutaFicheros
        {
            get; set;
        }

        public string AzureStorageConnectionString { get; set; }

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

        #endregion
    }
}
