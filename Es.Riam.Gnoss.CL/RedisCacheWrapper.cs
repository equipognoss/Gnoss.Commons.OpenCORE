using BeetleX.Redis;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace Es.Riam.Gnoss.CL
{
    public class RedisCacheWrapper : IDisposable
    {
        private ConcurrentDictionary<string, RedisDB> redisClientLectura;
        private ConcurrentDictionary<string, RedisDB> redisClientEscritura;
        public IMemoryCache Cache { get; set; }
        public RedisCacheWrapper(IMemoryCache cache)
        {
            Cache = cache;
            redisClientLectura  = new ConcurrentDictionary<string, RedisDB>();
            redisClientEscritura = new ConcurrentDictionary<string, RedisDB>();
        }
        public RedisCacheWrapper()
        {
            redisClientLectura  = new ConcurrentDictionary<string, RedisDB>();
            redisClientEscritura = new ConcurrentDictionary<string, RedisDB>();
        }
        public RedisDB RedisLectura(string pPoolName)
        {
            if (redisClientLectura.TryGetValue(pPoolName, out RedisDB db))
            {
                return db;
            }
            return null;
        }

        public RedisDB RedisEscritura(string pPoolName)
        {
            if (redisClientEscritura.TryGetValue(pPoolName, out RedisDB db))
            {
                return db;
            }
            return null;
        }

        public void AddRedisEscritura(string pPoolName, RedisDB redisClient)
        {
            RedisDB anterior = null;
            redisClientEscritura.AddOrUpdate(pPoolName, redisClient, (clave, previo) =>
            {
                anterior = previo;
                return redisClient;
            });
            if (anterior != null && !ReferenceEquals(anterior, redisClient))
            {
                anterior.Dispose();
            }
        }
        public void AddRedisLectura(string pPoolName, RedisDB redisClient)
        {
            RedisDB anterior = null;
            redisClientLectura.AddOrUpdate(pPoolName, redisClient, (clave, previo) =>
            {
                anterior = previo;
                return redisClient;
            });
            if (anterior != null && !ReferenceEquals(anterior, redisClient))
            {
                anterior.Dispose();
            }
        }

        public void CerrarConexionesEscritura()
        {
            foreach (var item in redisClientEscritura.Values)
            {
                item.Dispose();
            }
        }

        public void CerrarConexionesLectura()
        {
            foreach (var item in redisClientLectura.Values)
            {
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
