using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;

namespace Es.Riam.Gnoss.ExportarImportar
{
    /// <summary>
    /// Representa la clase que lleva a cabo la encriptacion y desencriptación de ficheros.
    /// </summary>
    public class Encriptacion
    {
        #region Miembros

        /// <summary>
        /// Clave usada para encriptar los ficheros
        /// </summary>
        private static byte[] mClave = { 184, 33, 45, 157, 53, 108, 220, 10, 32, 0, 225, 201, 112, 88, 250, 56, 148, 27, 213, 70, 1, 55, 198, 209 };

        /// <summary>
        /// Vector de inicialización para generar la clave publica.
        /// </summary>
        private static byte[] mVectorInicializacion = GenerarVectorInicializacion();

        #endregion

        #region Métodos generales

        #region Públicos

        private static byte[] GenerarVectorInicializacion()
        {
            //La cadena será la del nombre de usuario corporativo
            char[] vector = "RiamEsde".ToCharArray();
            byte[] vectorCodigoAscii = new byte[vector.Length];
            int i = 0;
            foreach (char caracter in vector)
            {
                vectorCodigoAscii[i++] = byte.Parse(((int)caracter).ToString());
            }
            return vectorCodigoAscii;
        }

        /// <summary>
        /// Encripta un texto
        /// </summary>
        /// <param name="pTexto">Texto a encriptar</param>
        /// <returns></returns>
        public static string EncriptarTexto(string pTexto)
        {
            string textoEncriptado = string.Empty;
            try
            {
                // Creo un proveedor criptográfico para el algoritmo simétrico TripleDES
                TripleDESCryptoServiceProvider CSProvider =
                            new TripleDESCryptoServiceProvider();
                // Creo un stream para escibir el texto encriptado
                MemoryStream stream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(
                    stream, CSProvider.CreateEncryptor(mClave, mVectorInicializacion),
                    CryptoStreamMode.Write);
                StreamWriter streamWriter = new StreamWriter(cryptoStream);

                // Escribo el texto a encriptar
                streamWriter.Write(pTexto);

                // Cierro todos los streams
                streamWriter.Flush();
                streamWriter.Close();
                cryptoStream.Close();
                stream.Close();

                textoEncriptado = Convert.ToBase64String(stream.ToArray());
            }
            catch (Exception ex)
            {
                textoEncriptado = ex.Message;
            }
            return textoEncriptado;

            /*RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            byte[] Enc = new byte[pTexto.Length];

            for (int i = 0; i < pTexto.Length; i++)
            {
                Enc[i] = (byte)pTexto[i];
            }

            Enc = RSA.Encrypt(Enc, true);

            return Enc;*/

            /*flujo = File.OpenRead("c:\\encriptado.txt");
            for (int i = 0; i < flujo.Length; i++)
                Enc[i] = (byte)flujo.ReadByte();
            flujo.Close();

            flujo = File.Create("c:\\progs\\original.txt");
            Enc = RSA.Decrypt(Enc, true);
            for (int i = 0; i < Enc.Length; i++)
                flujo.WriteByte(Enc[i]);
            flujo.Close();*/
        }

        /// <summary>
        /// Encripta una secuencia de bytes
        /// </summary>
        /// <param name="pSecuencia">Bytes a encriptar</param>
        /// <returns></returns>
        public static byte[] EncriptarBytes(byte[] pSecuencia)
        {
            byte[] bytesEncriptados = null;

            // Creo un proveedor criptográfico para el algoritmo simétrico TripleDES
            TripleDESCryptoServiceProvider CSProvider =
                        new TripleDESCryptoServiceProvider();

            // Creo un stream para escibir el texto encriptado
            MemoryStream stream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(
                stream, CSProvider.CreateEncryptor(mClave, mVectorInicializacion),
                CryptoStreamMode.Write);
            StreamWriter streamWriter = new StreamWriter(cryptoStream);

            // Escribo el texto a encriptar
            streamWriter.BaseStream.Write(pSecuencia, 0, pSecuencia.Length);

            bytesEncriptados = stream.ToArray();

            // Cierro todos los streams
            streamWriter.Flush();
            streamWriter.Close();
            cryptoStream.Close();
            stream.Close();

            return bytesEncriptados;
        }

        /// <summary>
        /// Desencriptar fichero
        /// </summary>
        /// <param name="pRuta">Ruta del fichero</param>
        /// <returns></returns>
        public static string DesencriptarFichero(string pRuta)
        {
            //Crear el vector de inicialización a partir de el nombre de usuario corporativos.
            string textoDesencriptado = string.Empty;
            try
            {
                // Creo un proveedor criptográfico para el algoritmo simétrico TripleDES
                TripleDESCryptoServiceProvider CSProvider =
                    new TripleDESCryptoServiceProvider();
                // Creo un stream para escibir el texto desencriptado
                // MemoryStream mstream = new MemoryStream(Convert.FromBase64String(pTexto));
                Stream fichero = new FileStream(pRuta, FileMode.Open);
                CryptoStream cryptoStream = new CryptoStream(
                    fichero, CSProvider.CreateDecryptor(mClave, mVectorInicializacion),
                    CryptoStreamMode.Read);
                StreamReader streamReader = new StreamReader(cryptoStream);

                // Escribo el texto a desencriptar
                textoDesencriptado = streamReader.ReadToEnd();

                // Cierro todos los streams
                streamReader.Close();
                cryptoStream.Close();
                fichero.Close();
            }
            catch (Exception)
            {
                throw new Exception("El fichero no tinene un formato válido.");
            }
            return textoDesencriptado;
        }

        /// <summary>
        /// Desencripta un fichero y lo devuelve como una secuencia de bytes
        /// </summary>
        /// <param name="pRuta">Ruta del fichero</param>
        /// <returns></returns>
        public static byte[] DesencriptarBytesFichero(string pRuta)
        {
            //Crear el vector de inicialización a partir de el nombre de usuario corporativos.
            byte[] bytes = null;
            Stream fichero = null;
            CryptoStream cryptoStream = null;
            BinaryReader streamReader = null;
            int ese = 0;
            int i = 0;

            try
            {
                // Creo un proveedor criptográfico para el algoritmo simétrico TripleDES
                TripleDESCryptoServiceProvider CSProvider =
                    new TripleDESCryptoServiceProvider();
                // Creo un stream para escibir el texto desencriptado
                //MemoryStream mstream = new MemoryStream(Convert.FromBase64String(pTexto));
                fichero = new FileStream(pRuta, FileMode.Open);
                cryptoStream = new CryptoStream(
                    fichero, CSProvider.CreateDecryptor(mClave, mVectorInicializacion),
                    CryptoStreamMode.Read);
                streamReader = new BinaryReader(cryptoStream);

                //streamReader.Read("",1,fichero.Length - 10);
                // Leo el texto a desencriptar
                ArrayList listaBytes = new ArrayList();
                byte leido = 0;
                leido = streamReader.ReadByte();
                while ((streamReader.BaseStream.CanRead) && (ese != -1))
                {
                    listaBytes.Add(leido);
                    leido = streamReader.ReadByte();
                    //leido = (byte)ese;//streamReader.ReadByte();
                    i++;
                }
                bytes = new byte[listaBytes.Count];
                listaBytes.CopyTo(bytes);
                //bytes = streamReader.ReadBytes((int)streamReader.BaseStream.Length);
            }
            catch (Exception)
            {
                throw new Exception("El fichero no tiene un formato válido.");
            }
            finally
            {
                //cerrar todos
                if (streamReader != null)
                    streamReader.Close();

                if (cryptoStream != null)
                    cryptoStream.Close();

                if (fichero != null)
                    fichero.Close();
            }
            return bytes;
        }

        #endregion

        #endregion
    }
}
