using System;
using System.Security.Cryptography;
using System.Text;

namespace Es.Riam.Util
{
    /// <summary>
    /// Calcula y verifica un hash criptográfico. Usado para almacenar y recuperar las claves de los usuarios.
    /// </summary>
    public class HashHelper
	{
		private const int _longitudSalt = 4;
		private const int _longitudPasswordMax = 12;

		/// <summary>
		/// Calcula la encriptación para una password
		/// </summary>
		/// <param name="password">Password</param>
		/// <param name="p256">Verdad si se desea utilizar el algoritmo de 256 bits</param>
		/// <returns>Password encriptada</returns>
		public static string CalcularHash(string password, bool p256)
		{
			byte[] passwordBinaria;
			byte[] salt;
			byte[] passwordHashedYHash;

			// Recorto la password si supera el máximo
			if (password.Length > _longitudPasswordMax)
				password = password.Substring(0, _longitudPasswordMax);

			passwordBinaria = System.Text.Encoding.Unicode.GetBytes(password);
			salt = CrearSalt(_longitudSalt);

			passwordHashedYHash = CalcularHash(passwordBinaria, salt, p256);

			// Convertimos a texto y devolvemos
			return Convert.ToBase64String(passwordHashedYHash);
		}

		/// <summary>
		/// Valida que las dos passwords sean identicas
		/// </summary>
		/// <param name="password1">Password para validar</param>
		/// <param name="password2Hashed">Password autentica con la que se compara</param>
		/// <param name="p256">Verdad si el algoritmo con el que se a encriptado la password es de 256 bits</param>
		/// <returns>Verdad en caso de ser idénticas</returns>
		public static bool ValidarPassword(string password1, string password2Hashed, bool p256)
		{
			byte[] salt;
			int saltOffset;
			byte[] password2HashedBinaria;
			byte[] password1Hashed;

			// Recuperamos el hash almacenado
			password2HashedBinaria = Convert.FromBase64String(password2Hashed);
			salt = new byte[_longitudSalt];
			saltOffset = password2HashedBinaria.Length - _longitudSalt;
			for (int i = 0; i < _longitudSalt; i++)
				salt[i] = password2HashedBinaria[saltOffset + i];

			// Realizamos el hash para compararlo
			password1Hashed = CalcularHash(System.Text.Encoding.Unicode.GetBytes(password1), salt, p256);

			return CompararPasswords(password1Hashed, password2HashedBinaria);
		}

		/// <summary>
		/// Génera código aleatorio para la encriptación
		/// </summary>
		/// <param name="size">Tamaño</param>
		/// <returns></returns>
		private static byte[] CrearSalt(int size)
		{
			// Generamos un código aleatorio para la encriptación.
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			byte[] buff = new byte[size];

			rng.GetBytes(buff);
			return buff;
		}

		/// <summary>
		/// Calcula el hash para una password
		/// </summary>
		/// <param name="password">Password para calcular</param>
		/// <param name="salt">Valor</param>
		/// <param name="p256">Verdad si se quiere calcular el HASH con el algoritmo de encriptación de 256 bits</param>
		/// <returns>Hash calculado para password</returns>
		private static byte[] CalcularHash(byte[] password, byte[] salt, bool p256)
		{
			byte[] passwordSalted;
			byte[] passwordHashed;
			byte[] passwordHashedYHash;

			// Concatenamos password y valor salt
			passwordSalted = new byte[password.Length + salt.Length];
			password.CopyTo(passwordSalted, 0);
			salt.CopyTo(passwordSalted, password.Length);

			// Generamos el hash de la unión

			if (p256)
			{
				//Usando SHA256
				SHA256 mySHA256 = SHA256Managed.Create();
				passwordHashed = mySHA256.ComputeHash(passwordSalted);
			}
			else
			{
				HashAlgorithm ha = new SHA1CryptoServiceProvider();
				passwordHashed = ha.ComputeHash(passwordSalted);
			}

			// Añadimos el salt al hash en texto claro
			passwordHashedYHash = new byte[passwordHashed.Length + salt.Length];
			passwordHashed.CopyTo(passwordHashedYHash, 0);
			salt.CopyTo(passwordHashedYHash, passwordHashed.Length);

			return passwordHashedYHash;
		}

		/// <summary>
		/// Compara que dos passwords sean idénticas
		/// </summary>
		/// <param name="password1">Password</param>
		/// <param name="password2">Password</param>
		/// <returns>Verdad en caso de ser idénticas, falso en caso contrario</returns>
		private static bool CompararPasswords(byte[] password1, byte[] password2)
		{
			if (password1.Length != password2.Length)
				return false;

			for (int i = 0; i < password1.Length; i++)
			{
				if (password1[i] != password2[i])
					return false;
			}

			return true;
		}

		/// <summary>
		/// Genera un resumen MD5 a partir de un string
		/// </summary>
		/// <param name="pCadena">string a resumir</param>
		/// <returns>string resumido</returns>
		public static string GenerarMD5(string pCadena)
		{
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

			byte[] byteArray = Encoding.ASCII.GetBytes(pCadena);

			byteArray = md5.ComputeHash(byteArray);

			string hashedValue = "";

			foreach (byte b in byteArray)
			{
				hashedValue += b.ToString("x2");
			}

			return hashedValue;
		}

	}
}
