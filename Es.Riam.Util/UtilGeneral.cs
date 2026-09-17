using MessagePack;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Pipelines;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Es.Riam.Util
{
    /// <summary>
    /// Útiles generales
    /// </summary>
    public class UtilGeneral
    {
        private static readonly HttpClient mHttpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromMinutes(30)
        };

        private static readonly HttpClient mHttpClientNoRedirect = new HttpClient(new HttpClientHandler
        {
            AllowAutoRedirect = false
        })
        {
            Timeout = TimeSpan.FromMinutes(30)
        };

        public UtilGeneral()
        {
        }

        #region Métodos estáticos

        public static T Copiar<T>(T objeto)
        {
            //Verificamos que sea serializable antes de hacer la copia
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("El tipo de dato debe ser serializable.", "objeto");
            }
            if (Object.ReferenceEquals(objeto, null))
            {
                return default(T);
            }
            var options = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.All,
            };
            byte[] buffer = null;
            try
            {
                //string json = JsonSerializer.Serialize(obj, options);
                string json = JsonConvert.SerializeObject(objeto, options);
                buffer = BsonDocument.Parse(json).ToBson();
            }
            catch (Exception ex)
            {
                buffer = MessagePackSerializer.Serialize(objeto);
            }
            try
            {
                BsonDocument bsonDoc = BsonSerializer.Deserialize<BsonDocument>(buffer);
                string json = bsonDoc.ToJson();
                //obj = System.Text.Json.JsonSerializer.Deserialize(json, clase, options);
                return (T)JsonConvert.DeserializeObject(json, typeof(T), options);
            }
            catch (Exception ex)
            {
                try
                {
                    return (T)MessagePackSerializer.Deserialize(typeof(T), buffer);
                }
                catch
                {
                    //Ha Cambiado el modelo de datos de la cache, devolvemos null para que lo obtenga de Base de datos
                    return default(T);
                }
            }
        }

        /// <summary>
        /// Crea un nuevo objeto del mismo tipo que el del objeto pasado por parámetro
        /// </summary>
        /// <param name="pObjetoOriginal">Objeto del que se va a copiar el tipo</param>
        /// <returns>Objeto del mismo tipo</returns>
        public static object CrearObjetoDelMismoTipo(object pObjetoOriginal)
        {
            Type tipo = pObjetoOriginal.GetType();
            Assembly ensamblado = Assembly.GetAssembly(tipo);
            return ensamblado.CreateInstance(tipo.FullName);
        }

        /// <summary>
        /// Comprueba si dos arrays son iguales
        /// </summary>
        /// <param name="pArray1">Array 1</param>
        /// <param name="pArray2">Array 2</param>
        /// <returns>TRUE si son iguales</returns>
        public static bool ArraysIguales(Array pArray1, Array pArray2)
        {
            // Si alguno de los dos es nulo o tienen diferente longitud no son iguales
            if (pArray1 == null || pArray2 == null || !pArray1.Length.Equals(pArray2.Length))
                return false;

            // Comparar elemento con elemento
            for (int i = 0; i < pArray1.Length; i++)
            {
                if (!pArray1.GetValue(i).Equals(pArray2.GetValue(i)))
                    return false;
            }

            // Si se ha llegado hasta aquí, los arrays son iguales
            return true;
        }


        
        #endregion
    }

}
