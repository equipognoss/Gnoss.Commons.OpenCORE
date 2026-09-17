using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Es.Riam.Gnoss.CL
{
    /// <summary>
    /// Pool estático de ConnectionMultiplexer (StackExchange.Redis) por cadena de conexión.
    /// ConnectionMultiplexer está diseñado para vivir toda la vida del proceso; las instancias
    /// que sirve este pool no se disponen nunca desde el llamador.
    /// </summary>
    public static class RedisMultiplexerPool
    {
        private static readonly ConcurrentDictionary<string, Lazy<ConnectionMultiplexer>> mPool = new();

        public static ConnectionMultiplexer Obtener(string pIP, int pDB)
        {
            string clave = $"{pIP},defaultDatabase={pDB},abortConnect=false";
            return mPool.GetOrAdd(clave, k => new Lazy<ConnectionMultiplexer>(
                () => ConnectionMultiplexer.Connect(k),
                LazyThreadSafetyMode.ExecutionAndPublication)).Value;
        }
    }
}
