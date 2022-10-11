using Es.Riam.InterfacesOpenArchivos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.OpenArchivos
{
    public class UtilArchivosOpen : IUtilArchivos
    {
        public byte[] DesencriptarArchivo(byte[] pFichero)
        {
            return pFichero;
        }

        public byte[] EncriptarArchivo(byte[] pFichero)
        {
            return pFichero;
        }

        public CryptoStream ObtenerDesencriptador(Stream pStream)
        {
            return null;
        }

        public CryptoStream ObtenerEncriptador(Stream pStream)
        {
            return null;
        }
    }
}
