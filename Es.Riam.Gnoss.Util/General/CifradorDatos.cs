namespace Es.Riam.Gnoss.Util.General
{
    ///// <summary>
    ///// Descripción breve de CifradorDatos.
    ///// </summary>
    //public class CifradorDatos : Win32DPAPI
    //{
    //    /// <summary>
    //    /// Constructor
    //    /// </summary>
    //    /// <param name="tempStore"></param>
    //    public CifradorDatos(DPAPIStore tempStore) : base(tempStore)
    //    {
    //    }

    //    /// <summary>
    //    /// Cifra una cadena
    //    /// </summary>
    //    /// <param name="plainText"></param>
    //    /// <param name="optionalEntropy"></param>
    //    /// <returns></returns>
    //    public string Cifrar(string plainText, byte[] optionalEntropy)
    //    {
    //        byte[] data;
    //        byte[] dataCifrado;

    //        data = System.Text.Encoding.Unicode.GetBytes(plainText);
    //        Encryp
    //        dataCifrado = Encrypt(data, optionalEntropy);

    //        return Convert.ToBase64String(dataCifrado);
    //    }

    //    /// <summary>
    //    /// Descifra una cadena
    //    /// </summary>
    //    /// <param name="cipherText"></param>
    //    /// <param name="optionalEntropy"></param>
    //    /// <returns></returns>
    //    public string Descifrar(string cipherText, byte[] optionalEntropy)
    //    {
    //        try
    //        {
    //            byte[] dataCifrado;
    //            byte[] data;

    //            dataCifrado = Convert.FromBase64String(cipherText);
    //            data = Decrypt(dataCifrado, optionalEntropy);
    //            return System.Text.Encoding.Unicode.GetString(data);
    //        }
    //        catch (Exception x)
    //        {
    //            throw new ErrorFicheroConfig(x);
    //        }
    //    }
    //}
}