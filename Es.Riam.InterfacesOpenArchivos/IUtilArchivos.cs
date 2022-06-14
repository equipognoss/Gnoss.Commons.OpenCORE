using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.InterfacesOpenArchivos
{
    public interface IUtilArchivos
    {
        public byte[] EncriptarArchivo(byte[] pFichero);

        public CryptoStream ObtenerEncriptador(Stream stream);

        public byte[] DesencriptarArchivo(byte[] pFichero);

        public CryptoStream ObtenerDesencriptador(Stream pStream);

    }
}
