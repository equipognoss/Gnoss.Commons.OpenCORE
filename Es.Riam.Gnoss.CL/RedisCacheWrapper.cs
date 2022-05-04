using BeetleX.Redis;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;

namespace Es.Riam.Gnoss.CL
{
    public class RedisCacheWrapper : IDisposable
    {
        private Dictionary<string, RedisDB> redisClientLectura;
        private Dictionary<string, RedisDB> redisClientEscritura;
        public IMemoryCache Cache { get; set; }
        public RedisCacheWrapper(IMemoryCache cache)
        {
            Cache = cache;
            redisClientLectura = new Dictionary<string, RedisDB>();
            redisClientEscritura = new Dictionary<string, RedisDB>();
        }
        public RedisCacheWrapper()
        {
            redisClientLectura = new Dictionary<string, RedisDB>();
            redisClientEscritura = new Dictionary<string, RedisDB>();
        }
        public RedisDB RedisLectura(string pPoolName)
        {
            if (redisClientLectura.ContainsKey(pPoolName))
            {
                return redisClientLectura[pPoolName];
            }
            return null;
        }

        public RedisDB RedisEscritura(string pPoolName)
        {
            if (redisClientEscritura.ContainsKey(pPoolName))
            {
                return redisClientEscritura[pPoolName];
            }
            return null;
        }

        public void AddRedisEscritura(string pPoolName, RedisDB redisClient)
        {
            redisClientEscritura.Add(pPoolName, redisClient);
        }
        public void AddRedisLectura(string pPoolName, RedisDB redisClient)
        {
            redisClientLectura.Add(pPoolName, redisClient);
        }

        public void CerrarConexionesEscritura()
        {
            foreach (var item in redisClientEscritura.Values)
            {
                //GuardarLog($"Dispose escritura: El DB es: {item.DB}");
                item.Dispose();
            }
        }

        public void CerrarConexionesLectura()
        {
            foreach (var item in redisClientLectura.Values)
            {
                //GuardarLog($"Dispose lectura: El DB es: {item.DB}");
                item.Dispose();
            }
        }

        public void Dispose()
        {
            
            CerrarConexionesEscritura();
            CerrarConexionesLectura();
        }

        private static object BLOQUEO_LOG = new object();

        public static void GuardarLog(string pMensaje)
        {
            lock (BLOQUEO_LOG)
            {
                FileInfo info = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "redisConnections.txt"));
                if (!info.Exists)
                {
                    if (!info.Directory.Exists)
                    {
                        info.Directory.Create();
                    }
                    info.Create();
                }
                using (StreamWriter sw = new StreamWriter(info.FullName, true))
                {
                    sw.WriteLine(pMensaje);
                }
            }
        }

    }
}
