using MessagePack;
using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json.Serialization;
using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;

namespace Es.Riam.Util
{
    /// <summary>
    /// Representa un fichero comprimido
    /// </summary>
    public class UtilZip
    {

        #region Metodos Generales

        #region publicos

        /// <summary>
        /// Descomprime un unico archivo comprimido en un fichero zip en la ruta especificada.
        /// </summary>
        /// <param name="pRutaFicheroZip">Ruta del fichero a descomprimir</param>
        /// <param name="pRutaCarpetaDescompresion">Carpeta en la que se descomprimira el fichero</param>
        /// <param name="pNombreFicheroDescompresion">Nombre del fichero que hay que descomprimir</param>
        /// <returns>Stream de lectura del archivo descomprimido</returns>
        public static Stream DescomprimirFichero(string pRutaFicheroZip, string pRutaCarpetaDescompresion, string pNombreFicheroDescompresion)
        {
            ZipArchive zip = null;

            try
            {
                zip = ZipFile.Open(pRutaFicheroZip, ZipArchiveMode.Update);

                DirectoryInfo directorio = new DirectoryInfo(pRutaCarpetaDescompresion);
                if (!directorio.Exists)
                {
                    directorio.Create();
                }

                ZipArchiveEntry entrada = zip.Entries.FirstOrDefault(item => item.Name.Equals(pNombreFicheroDescompresion));
                entrada.ExtractToFile(pRutaCarpetaDescompresion);
                FileInfo fichero = new FileInfo(pNombreFicheroDescompresion);

                return new StreamReader(pRutaCarpetaDescompresion + fichero.Name).BaseStream;
            }
            catch (Exception) { }
            finally
            {
                if (zip != null)
                {
                    zip.Dispose();
                }
            }

            return null;
        }

        /// <summary>
        /// Descomprime un fichero zip en la ruta especificada.
        /// </summary>
        /// <param name="pRutaFicheroZip">Ruta del fichero a descomprimir</param>
        /// <param name="pRutaCarpetaDescompresion">Carpeta en la que se descomprimira el fichero</param>
        /// <returns>Verdad si la descompresión se realizo con éxito, falso en caso contrario.</returns>
        public static bool Descomprimir(string pRutaFicheroZip, string pRutaCarpetaDescompresion)
        {
            ZipArchive zip = null;
            bool exito = true;

            try
            {
                zip = ZipFile.Open(pRutaFicheroZip, ZipArchiveMode.Update);
                // recorro todos los fichero del archivo zip
                //while (zip.MoveNext())
                foreach (ZipArchiveEntry entrada in zip.Entries)
                {
                    //ZipEntry entrada = zip.Current;
                    DirectoryInfo directorio = null;

                    string fileName = entrada.FullName;

                    if (ComprobarFicherosNuget(entrada.FullName))
                    {

                        if (fileName.Contains(Path.DirectorySeparatorChar))
                        {
                            directorio = new DirectoryInfo(Path.Combine(pRutaCarpetaDescompresion, fileName.Substring(0, fileName.LastIndexOf(Path.DirectorySeparatorChar))));
                        }
                        else
                        {
                            directorio = new DirectoryInfo(pRutaCarpetaDescompresion);
                        }

                        //if ((entrada.IsDirectory) && (!Directory.Exists(pRutaCarpetaDescompresion + entrada.Name)))
                        if (!directorio.Exists)
                        {
                            directorio.Create();
                            //Directory.CreateDirectory(pRutaCarpetaDescompresion + fichero.Directory);
                        }

                        try
                        {
                            entrada.ExtractToFile(directorio.FullName + Path.DirectorySeparatorChar);
                        }
                        catch (Exception) { }
                    }
                }
            }
            catch (Exception)
            {
                exito = false;
            }
            finally
            {
                if (zip != null)
                {
                    zip.Dispose();
                }
            }
            return exito;
        }

        private static bool ComprobarFicherosNuget(string pFileName)
        {
            if (pFileName.Contains("_rels/.rels")) { return false; }
            else if (pFileName.Contains("package/services/metadata/core-properties")) { return false; }
            else if (pFileName.Contains("[Content_Types].xml")) { return false; }
            else if (pFileName.EndsWith(".nuspec")) { return false; }
            else { return true; }

        }

        /// <summary>
        /// Comprime un fichero gnoss junto con su documentación.
        /// </summary>
        /// <param name="pRutas">rutas de la documentacion asociada al fichero gnoss.</param>
        /// <param name="pRutaFicheroZip">ruta del fichero zip</param>
        /// <param name="pRutaFicheroOwl">ruta del fichero owl</param>
        /// <returns>Rutas de los ficheros que no se han podido comprimir.</returns>
        public static ArrayList ComprimirFicheroGnoss(string[] pRutas, string pRutaFicheroZip, string pRutaFicheroOwl)
        {
            ArrayList resultado = new ArrayList();
            //Crear fichero zip
            //ZipWriter zip = new ZipWriter(pRutaFicheroZip);
            ZipArchive zip = ZipFile.Open(pRutaFicheroZip, ZipArchiveMode.Update);

            //comprimir fichero owl
            FileInfo ficheroOWL = new FileInfo(pRutaFicheroOwl);
            string nombreFichpdf = pRutaFicheroOwl.Substring(0, pRutaFicheroOwl.Length - 4) + ".pdf";
            if ((ComprimirFichero(pRutaFicheroOwl, Path.Combine("OWL", ficheroOWL.Name), zip)) && ((ComprimirFichero(nombreFichpdf, Path.Combine("PDF", ficheroOWL.Name.Substring(0, ficheroOWL.Name.Length - 4) + ".pdf"), zip)) || true))
            {

                //Comprimir documentacion
                resultado = Comprimir(pRutas, zip, "Documentacion");
            }
            else
            {
                resultado.Add(pRutaFicheroOwl);
                resultado.AddRange(pRutas);
            }
            zip.Dispose();

            return resultado;
        }

        /// <summary>
        /// Crea un fichero zip que contiene todos los ficheros enviados como parametros.
        /// </summary>
        /// <param name="pRutas">rutas de los documentos a comprimir</param>
        /// <param name="pRutaFicheroZip">ruta completa del fichero zip</param>
        /// <returns>Rutas de los ficheros que no se han podido comprimir.</returns>
        public static ArrayList Comprimir(string[] pRutas, string pRutaFicheroZip)
        {
            ArrayList resultado = new ArrayList();
            //Crear fichero zip
            //ZipWriter zip = new ZipWriter(pRutaFicheroZip);
            ZipArchive zip = ZipFile.Open(pRutaFicheroZip, ZipArchiveMode.Update);

            //Comprimir ficheros
            resultado = Comprimir(pRutas, zip, null);

            zip.Dispose();

            return resultado;
        }

        #region Compresión

        /// <summary>
        /// Comprime un objeto en un array de bytes.
        /// </summary>
        /// <param name="pObjeto">Objeto</param>
        /// <returns>Array de bytes comprimido</returns>
        public static byte[] Zip(object pObjeto)
        {
            #region GZip

            //MemoryStream stream = null;
            //byte[] arrayComprimido = null;

            //try
            //{
            //    stream = new MemoryStream();
            //    BinaryFormatter formatter = new BinaryFormatter();

            //    formatter.Serialize(stream, pObjeto);

            //    byte[] buffer = stream.ToArray();
            //    MemoryStream ms = new MemoryStream();
            //    using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
            //    {
            //       zip.Write(buffer, 0, buffer.Length);
            //    }

            //    ms.Position = 0;
            //    MemoryStream outStream = new MemoryStream();

            //    byte[] compressed = new byte[ms.Length];
            //    ms.Read(compressed, 0, compressed.Length);

            //    byte[] gzBuffer = new byte[compressed.Length + 4];
            //    System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            //    System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);

            //    arrayComprimido = gzBuffer;
            //}
            //catch (Exception e)
            //{
            //}
            //finally
            //{
            //    if (stream != null)
            //        stream.Dispose();
            //    stream = null;
            //}

            //return arrayComprimido;

            #endregion

            byte[] arrayComprimido = null;
            
            try
            {
                byte[] buffer = MessagePackSerializer.Typeless.Serialize(pObjeto);

                MemoryStream ms = new MemoryStream();
                using (ICSharpCode.SharpZipLib.Zip.ZipOutputStream zip = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(ms))
                {
                    zip.PutNextEntry(new ICSharpCode.SharpZipLib.Zip.ZipEntry("compr"));
                    zip.Write(buffer, 0, buffer.Length);
                    zip.Finish();
                    arrayComprimido = ms.ToArray();
                    zip.Close();
                }

                buffer = null;
            }
            catch (Exception ex) { }

            return arrayComprimido;
        }

        /// <summary>
        /// Descomprime un objeto a partir de un array de bytes comprimido.
        /// </summary>
        /// <param name="pArray">Array de bytes comprimido</param>
        /// <returns>Objeto</returns>
        public static object UnZip(byte[] pArray)
        {
            #region GZip

            //MemoryStream stream = null;
            //object objeto = null;

            //try
            //{
            //    using (MemoryStream ms = new MemoryStream())
            //    {
            //        int msgLength = BitConverter.ToInt32(pArray, 0);
            //        ms.Write(pArray, 4, pArray.Length - 4);

            //        byte[] buffer = new byte[msgLength];

            //        ms.Position = 0;
            //        using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
            //        {
            //            zip.Read(buffer, 0, buffer.Length);
            //        }

            //        stream = new MemoryStream(buffer);
            //        BinaryFormatter formatter = new BinaryFormatter();
            //        objeto = formatter.Deserialize(stream);
            //    }
            //}
            //catch (Exception)
            //{
            //}
            //finally
            //{
            //    if (stream != null)
            //        stream.Dispose();
            //    stream = null;
            //}

            //return objeto;

            #endregion

            object objeto = null;
            MemoryStream ms = null;

            try
            {
                ms = new MemoryStream();

                int msgLength = BitConverter.ToInt32(pArray, 0);
                ms.Write(pArray, 0, pArray.Length);

                byte[] buffer = new byte[msgLength];
                MemoryStream escritor = new MemoryStream();

                ms.Position = 0;
                using (ICSharpCode.SharpZipLib.Zip.ZipInputStream zip = new ICSharpCode.SharpZipLib.Zip.ZipInputStream(ms))
                {
                    ICSharpCode.SharpZipLib.Zip.ZipEntry entry = zip.GetNextEntry();

                    byte[] data = new byte[2048];

                    while (true)
                    {
                        int size = zip.Read(data, 0, data.Length);

                        if (size > 0)
                        {
                            escritor.Write(data, 0, size);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                buffer = escritor.ToArray();

                escritor.Close();
                ms.Close();

                objeto = MessagePackSerializer.Typeless.Deserialize(buffer);
            }
            catch (Exception ex){}
            
            return objeto;
        }

        #endregion

        #endregion

        #region privados

        /// <summary>
        /// Comprime todos los ficheros que se encuentran dentro del subdirectorio.
        /// </summary>
        /// <param name="pDirectorio">directorio que se desea comprimir.</param>
        /// <param name="pZip">zip</param>
        /// <param name="pRutaVirtual">ruta virtual que debe figurar en el fichero zip.</param>
        private static ArrayList ComprimirFicherosDirectorio(DirectoryInfo pDirectorio, ZipArchive pZip, string pRutaVirtual)
        {
            ArrayList resultado = new ArrayList();
            try
            {
                foreach (DirectoryInfo directorio in pDirectorio.GetDirectories())
                {//comprimir subDirectorios
                    ArrayList ficherosErroneos = ComprimirFicherosDirectorio(directorio, pZip, Path.Combine(pRutaVirtual, directorio.Name));
                    if (ficherosErroneos.Count > 0)
                    {
                        resultado.AddRange(ficherosErroneos);
                    }
                }

                foreach (FileInfo fichero in pDirectorio.GetFiles())
                {//comprimir ficheros
                    if (!ComprimirFichero(fichero.FullName, Path.Combine(pRutaVirtual, fichero.Name), pZip))
                    {
                        resultado.Add(fichero.FullName);
                    }
                }
            }
            catch (Exception)
            {
                resultado.Add(pDirectorio.FullName);
            }
            return resultado;
        }

        /// <summary>
        /// Comprime un fichero
        /// </summary>
        /// <param name="pRuta">ruta física del docuemnto</param>
        /// <param name="pRutaVirtualDocumento">ruta relativa que debe aparecer en el fichero zip.</param>
        /// <param name="pZip">zip</param>
        private static bool ComprimirFichero(string pRuta, string pRutaVirtualDocumento, ZipArchive pZip)
        {
            bool comprimido = true;
            //Creo una nueva entrada
            /*ZipEntry entrada = new ZipEntry(pRutaVirtualDocumento);
            entrada.ModifiedTime = File.GetLastWriteTime(pRuta);
            FileStream fichero = null;

            //añado la entrada al zip
            pZip.AddEntry(entrada);
            try
            {
                //leo y comprimo el fichero
                fichero = File.OpenRead(pRuta);
                byte[] buffer = new byte[4096];
                int byteCount;

                while ((byteCount = fichero.Read(buffer, 0, buffer.Length)) > 0)
                {
                    pZip.Write(buffer, 0, byteCount);
                }
            }*/
            try
            {
                pZip.CreateEntryFromFile(pRuta, pRutaVirtualDocumento);
            }
            catch (Exception)
            {
                comprimido = false;
            }
            /*finally
            {
                if (fichero != null)
                {
                    fichero.Close();
                }
            }*/

            return comprimido;
        }

        /// <summary>
        /// Comprime todos los ficheros pasados como parametros
        /// </summary>
        /// <param name="pRutas">rutas de los ficheros a comprimir.</param>
        /// <param name="pZip">zip</param>
        /// <param name="pDirectorioInicio">directorio relativo en el que deben aparecer todos los archivos en el fichero zip.</param>
        private static ArrayList Comprimir(string[] pRutas, ZipArchive pZip, string pDirectorioInicio)
        {
            ArrayList resultado = new ArrayList();
            // Agrego los ficheros y directorios al archivo zip
            foreach (string ruta in pRutas)
            {
                try
                {
                    if (Directory.Exists(ruta))
                    {//comprimir directorio
                        DirectoryInfo directorio = new DirectoryInfo(ruta);
                        if ((pDirectorioInicio != null) && (!string.IsNullOrEmpty(pDirectorioInicio)))
                        {//comprimir en directorio realtivo
                            resultado.AddRange(ComprimirFicherosDirectorio(directorio, pZip, Path.Combine(pDirectorioInicio, directorio.Name)));
                        }
                        else
                        {//comprimir en raiz
                            resultado.AddRange(ComprimirFicherosDirectorio(directorio, pZip, directorio.Name));
                        }
                    }
                    else if (File.Exists(ruta))
                    {//comprimir fichero
                        FileInfo fichero = new FileInfo(ruta);
                        if ((pDirectorioInicio != null) && (!string.IsNullOrEmpty(pDirectorioInicio)))
                        {//comprimir en directorio realtivo
                            if (!ComprimirFichero(fichero.FullName, Path.Combine(pDirectorioInicio, fichero.Name), pZip))
                            {
                                resultado.Add(ruta);
                            }
                        }
                        else
                        {//comprimir en raiz
                            if (!ComprimirFichero(fichero.FullName, fichero.Name, pZip))
                            {
                                resultado.Add(ruta);
                            }
                        }
                    }
                    else
                    {
                        resultado.Add(ruta);
                    }
                }
                catch (Exception)
                {
                    resultado.Add(ruta);
                }
            }
            return resultado;
        }

        #endregion

        #endregion
    }
}
