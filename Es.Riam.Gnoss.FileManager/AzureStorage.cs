/*
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.FileManager
{
    public class AzureStorage
    {
        private CloudFileShare mShare;

        private CloudFileDirectory mDirectorioRaiz;
        private string[] mPartesConexion;
        private string mConnectionString;

        public AzureStorage(string pConnectionString)
        {
            mConnectionString = pConnectionString;
            mPartesConexion = pConnectionString.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);

            if (mPartesConexion.Length < 2)
            {
                throw new Exception(string.Format("La cadena de conexión debe contener el almacen, separado por tres ||| de la cadena de conexión. {0}", pConnectionString));
            }

            string cadenaConexion = mPartesConexion[0];
            string nombreAlmacen = mPartesConexion[1];

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(cadenaConexion);

            CloudFileClient fileClient = storageAccount.CreateCloudFileClient();

            // Get a reference to the file share we created previously.
            mShare = fileClient.GetShareReference(nombreAlmacen);
        }

        public async void InitializeAzureStorage()
        {
            bool exist = await mShare.ExistsAsync();
            if (!exist)
            {
                throw new Exception(string.Format("El nombre del almacenamiento {0} no existe", mConnectionString));
            }

            mDirectorioRaiz = mShare.GetRootDirectoryReference();

            if (mPartesConexion.Length > 2)
            {
                //Hay una carpeta inicial
                mDirectorioRaiz = await ObtenerDirectorio(mPartesConexion[2], false);

                if (mDirectorioRaiz == null)
                {
                    throw new Exception(string.Format("La carpeta inicial configurada en el almacenamiento {0} no existe", mConnectionString));
                }
            }
        }

        public async void SubirDocumento(string pRuta, string pNombreArchivo, byte[] pBytes)
        {
            // Get a reference to the root directory for the share.
            CloudFileDirectory directorioPadre = await ObtenerDirectorio(pRuta, true);

            // Ensure that the directory exists.
            // Get a reference to the file we created previously.
            CloudFile file = directorioPadre.GetFileReference(pNombreArchivo);

            await file.UploadFromByteArrayAsync(pBytes, 0, pBytes.Length);
        }

        public async void SubirDocumento(string pRuta, string pNombreArchivo, System.IO.Stream pStream)
        {
            // Get a reference to the root directory for the share.
            CloudFileDirectory directorioPadre = await ObtenerDirectorio(pRuta, true);

            // Ensure that the directory exists.
            // Get a reference to the file we created previously.
            CloudFile file = directorioPadre.GetFileReference(pNombreArchivo);
            FileRequestOptions options = new FileRequestOptions();
            TimeSpan timespanUnDia = new TimeSpan(1, 0, 0, 0);
            options.MaximumExecutionTime = timespanUnDia;
            options.ServerTimeout = timespanUnDia;
            await file.UploadFromStreamAsync(pStream, null, options,null);
        }

        public async Task<byte[]> DescargarDocumento(string pRuta, string pNombreArchivo)
        {
            // Get a reference to the root directory for the share.
            CloudFileDirectory directorioPadre = await ObtenerDirectorio(pRuta, true);

            // Ensure that the directory exists.
            // Get a reference to the file we created previously.
            CloudFile file = directorioPadre.GetFileReference(pNombreArchivo);

            byte[] bytes = null;
            if (await file.ExistsAsync())
            {
                bytes = new byte[file.Properties.Length];

                if (file.Properties.Length > 0)
                {
                    await file.DownloadRangeToByteArrayAsync(bytes, 0, 0, file.Properties.Length);
                }
            }
            return bytes;
        }

        public async Task<byte[]> DescargarDocumentoSubdirectorios(string pRuta, string pNombreArchivo)
        {
            CloudFileDirectory directorioPadre = null;
            string[] carpetas = pNombreArchivo.Split(new string[] { "\\", "/" }, StringSplitOptions.RemoveEmptyEntries);
            string carpeta = null;
            string nombreArchivo = "";
            for (int i = 0; i < carpetas.Count() - 1; i++)
            {
                carpeta += carpetas[i];
                if (i != carpetas.Count() - 2)
                {
                    carpeta += Path.DirectorySeparatorChar;
                }
            }
            if (carpetas.Count() > 1)
            {
                // Get a reference to the root directory for the share.
                directorioPadre = await ObtenerDirectorio(pRuta + Path.DirectorySeparatorChar + carpeta, false);
                nombreArchivo = carpetas[carpetas.Count() - 1];
            }
            else
            {
                // Get a reference to the root directory for the share.
                directorioPadre = await ObtenerDirectorio(pRuta, false);
                nombreArchivo = pNombreArchivo;
            }

            // Ensure that the directory exists.
            // Get a reference to the file we created previously.
            CloudFile file = directorioPadre.GetFileReference(nombreArchivo);

            byte[] bytes = null;
            if (await file.ExistsAsync())
            {
                bytes = new byte[file.Properties.Length];

                if (file.Properties.Length > 0)
                {
                    await file.DownloadRangeToByteArrayAsync(bytes, 0, 0, file.Properties.Length);
                }
            }
            return bytes;
        }

        public void DescomprimirFichero(string pRuta, string fullName, Byte[] pBytes)
        {
            string[] carpetas = fullName.Split(new string[] { "\\", "/" }, StringSplitOptions.RemoveEmptyEntries);
            string carpeta = pRuta;
            for (int i = 0; i < carpetas.Count() - 1; i++)
            {
                carpeta += Path.DirectorySeparatorChar + carpetas[i];
            }

            string nombreFichero = fullName;
            if (carpetas.Count() > 1)
            {
                nombreFichero = carpetas[carpetas.Count() - 1];
            }
            SubirDocumento(carpeta, nombreFichero, pBytes);
        }


        public async Task<CloudFileDirectory> ObtenerDirectorio(string pRuta, bool pCrearSiNoExiste)
        {
            string[] carpetas = pRuta.Split(new string[] { "\\", "/" }, StringSplitOptions.RemoveEmptyEntries);

            // Creo el árbol de directorios
            CloudFileDirectory directorioPadre = mDirectorioRaiz;
            foreach (string carpeta in carpetas)
            {
                CloudFileDirectory directorioHijo = directorioPadre.GetDirectoryReference(carpeta);
                bool exist = await directorioHijo.ExistsAsync();
                if (!exist)
                {
                    if (pCrearSiNoExiste)
                    {
                        await directorioHijo.CreateAsync();
                    }
                    else
                    {
                        return null;
                    }
                }

                directorioPadre = directorioHijo;
            }

            return directorioPadre;
        }

        public void CrearDirectorio(string pRuta)
        {
            ObtenerDirectorio(pRuta, true);
        }

        /// <summary>
        /// Comprueba si existe un directorio en el almacenamiento de Azure
        /// </summary>
        /// <param name="pRuta">Ruta a comprobar</param>
        /// <returns>True si existe el directorio, falso en caso contrario</returns>
        public async Task<bool> ExisteDirectorio(string pRuta)
        {
            CloudFileDirectory directorio = await ObtenerDirectorio(pRuta, false);

            return directorio != null;
        }

        /// <summary>
        /// Comprueba si existe un fichero en el almacenamiento de Azure
        /// </summary>
        /// <param name="pRuta">Ruta a comprobar</param>
        /// <param name="pArchivo">Archivo a comprobar</param>
        /// <returns>True si existe el directorio, falso en caso contrario</returns>
        public async Task<bool> ExisteFichero(string pRuta, string pArchivo)
        {
            CloudFileDirectory directorio = await ObtenerDirectorio(pRuta, false);
            CloudFile fichero = null;

            if (directorio != null)
            {
                fichero = directorio.GetFileReference(pArchivo);
            }
            return fichero != null && await fichero.ExistsAsync();
        }

        public async Task<string[]> ObtenerFicherosDeDirectorio(string pRuta)
        {
            CloudFileDirectory directorio = await ObtenerDirectorio(pRuta, false);

            if (directorio != null)
            {
                var lista = await ListFilesAndDirectories(directorio);
                return lista.Where(fichero => fichero is CloudFile).Select(fichero => ((CloudFile)fichero).Name).ToArray();
            }

            return new string[] { };
        }

        public async Task<string[]> ObtenerFicherosDeDirectorioYSubDirectorios(string pRuta, string pRutaInicial)
        {
            CloudFileDirectory directorio = await ObtenerDirectorio(pRuta, false);
            //Codigo para obtener la carpeta en la que va a ir el fichero a buscar
            string pNombreDirectorio = pRuta.Replace(pRutaInicial, "");
            if (!string.IsNullOrEmpty(pNombreDirectorio))
            {
                pNombreDirectorio = pNombreDirectorio.Remove(0, 1);
            }
            else
            {
                pNombreDirectorio = null;
            }

            string[] vuelta = new string[] { };

            if (directorio != null)
            {
                var dire = await ListFilesAndDirectories(directorio);
                var lista = dire.Where(fichero => fichero is CloudFile).Select(fichero => ((CloudFile)fichero).Name).ToList();
                //Si tiene carpeta padre la lista que genero le pongo ese path
                if (pNombreDirectorio != null)
                {
                    List<string> listaFicherosConRuta = new List<string>();
                    foreach (string nombreFichero in lista)
                    {
                        listaFicherosConRuta.Add(Path.Combine(pNombreDirectorio, nombreFichero));
                    }

                    lista = listaFicherosConRuta;
                }
                var directorios = dire.Where(fichero => fichero is CloudFileDirectory).Select(fichero => ((CloudFileDirectory)fichero).Name).ToList();
                //Si tiene directorios llamo a esta misma funcion para volver a empezar el proceso.
                if (directorios != null)
                {
                    foreach (string nombreDirectorio in directorios)
                    {
                        if (!nombreDirectorio.Equals("historial"))
                        {
                            lista.AddRange(await ObtenerFicherosDeDirectorioYSubDirectorios(Path.Combine(pRuta, nombreDirectorio), pRutaInicial));
                        }
                    }
                }
                else
                {
                    return lista.ToArray();
                }
                return lista.ToArray();
            }
            return new string[] { };
        }

        public async Task<string[]> ObtenerSubdirectoriosDeDirectorio(string pRuta)
        {
            CloudFileDirectory directorio = await ObtenerDirectorio(pRuta, false);

            if (directorio != null)
            {
                var lista = await ListFilesAndDirectories(directorio);
                return lista.Where(fichero => fichero is CloudFileDirectory).Select(fichero => ((CloudFileDirectory)fichero).Name).ToArray();
            }

            return new string[] { };
        }

        public async void EliminarDirectorioEnCascada(string pRuta)
        {
            CloudFileDirectory directorio = await ObtenerDirectorio(pRuta, false);

            if (directorio != null)
            {
                EliminarDirectorioEnCascada(directorio, false);
            }
        }

        private async void EliminarDirectorioEnCascada(CloudFileDirectory pDirectorio, bool eliminarDirectorioInicial)
        {
            var listaObjetos = await ListFilesAndDirectories(pDirectorio);

            foreach (var objeto in listaObjetos)
            {
                if (objeto is CloudFile)
                {
                    await ((CloudFile)objeto).DeleteAsync();
                }
                else
                {
                    EliminarDirectorioEnCascada((CloudFileDirectory)objeto, true);
                }
            }

            if (eliminarDirectorioInicial)
            {
                await pDirectorio.DeleteIfExistsAsync();
            }
        }

        private async Task<List<IListFileItem>> ListFilesAndDirectories(CloudFileDirectory pDirectorio)
        {
            FileContinuationToken continuationToken = null;
            List<IListFileItem> fileResultSegments = new List<IListFileItem>(); 
            do
            {
                var response = await pDirectorio.ListFilesAndDirectoriesSegmentedAsync(continuationToken);
                continuationToken = response.ContinuationToken;
                fileResultSegments.AddRange(response.Results);
            }
            while (continuationToken != null);
            return fileResultSegments;
        }

        public async void CopiarArchivo(string pRutaOrigen, string pRutaDestino, string pNombreArchivo, bool pCopiar, string pNombreArchivoDestino)
        {
            // Get a reference to the root directory for the share.
            CloudFileDirectory directorioOrigen = await ObtenerDirectorio(pRutaOrigen, false);

            if (directorioOrigen != null)
            {
                // Ensure that the directory exists.
                // Get a reference to the file we created previously.
                CloudFile archivoOrigen = directorioOrigen.GetFileReference(pNombreArchivo);
                bool exist = await archivoOrigen.ExistsAsync();
                if (exist)
                {
                    CloudFileDirectory directorioDestino = await ObtenerDirectorio(pRutaDestino, true);

                    CloudFile archivoDestino = directorioDestino.GetFileReference(pNombreArchivoDestino);

                    await archivoDestino.StartCopyAsync(archivoOrigen);

                    if (!pCopiar)
                    {
                        await archivoOrigen.DeleteAsync();
                    }
                }
            }
        }

        public async Task<long> ObtenerTamanioArchivo(string pRuta, string pNombreArchivo)
        {
            CloudFileDirectory directorioDestino = await ObtenerDirectorio(pRuta, false);

            if (directorioDestino != null)
            {
                CloudFile file = directorioDestino.GetFileReference(pNombreArchivo);
                bool exist = await file.ExistsAsync();
                if (exist)
                {
                    return file.Properties.Length;
                }
            }

            return 0;
        }

        public async Task<DateTime?> ObtenerFechaUltimaModificacionArchivo(string pRuta, string pNombreArchivo)
        {
            CloudFileDirectory directorioDestino = await ObtenerDirectorio(pRuta, false);

            if (directorioDestino != null)
            {
                CloudFile file = directorioDestino.GetFileReference(pNombreArchivo);
                bool exist = await file.ExistsAsync();
                if (exist)
                {
                    return file.Properties.LastModified?.DateTime;
                }
            }

            return null;
        }

        public async Task<bool> EliminarFicheroFisico(string pRuta, string pNombreArchivo)
        {
            CloudFileDirectory directorioDestino = await ObtenerDirectorio(pRuta, false);
            bool deleted = false;
            if (directorioDestino != null)
            {
                CloudFile file = directorioDestino.GetFileReference(pNombreArchivo);
                deleted = await file.DeleteIfExistsAsync();       
            }
            return deleted;
        }
    }
}
*/