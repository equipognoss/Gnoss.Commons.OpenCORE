using Es.Riam.Gnoss.CL;

namespace Es.Riam.Gnoss.CL.Test
{
    public class RedisMultiplexerPoolTests
    {
        // Puerto 1 en loopback no tiene nada escuchando. "Obtener" interpola pIP tal cual en la
        // cadena de conexi�n, as� que se aprovecha para colar un connectTimeout corto (el pool
        // en producci�n no lo fija, por eso el default de 5s) y que el test no se quede
        // esperando. Con abortConnect=false (a�adido por el propio pool) Connect() no lanza y
        // devuelve la instancia igualmente, reintentando en segundo plano.
        private const string IP_SIN_SERVIDOR = "127.0.0.1:1,connectTimeout=200";

        [Test]
        public void Obtener_MismaIPYDB_DevuelveLaMismaInstancia()
        {
            var primero = RedisMultiplexerPool.Obtener(IP_SIN_SERVIDOR, 0);
            var segundo = RedisMultiplexerPool.Obtener(IP_SIN_SERVIDOR, 0);

            Assert.That(segundo, Is.SameAs(primero));
        }

        [Test]
        public void Obtener_MismaIPDistintaDB_DevuelveInstanciasDistintas()
        {
            var db0 = RedisMultiplexerPool.Obtener(IP_SIN_SERVIDOR, 0);
            var db1 = RedisMultiplexerPool.Obtener(IP_SIN_SERVIDOR, 1);

            Assert.That(db1, Is.Not.SameAs(db0));
        }

        [Test]
        public void Obtener_Concurrencia_MismaClaveDevuelveUnaSolaInstancia()
        {
            // Reproduce el escenario que motiva el pool: muchos "call-sites" pidiendo el
            // multiplexor de la misma conexi�n al mismo tiempo deben compartir una sola
            // instancia, no crear un ConnectionMultiplexer por llamada.
            const string ip = "127.0.0.1:2,connectTimeout=200";
            const int hilos = 32;

            var instancias = new StackExchange.Redis.ConnectionMultiplexer[hilos];
            System.Threading.Tasks.Parallel.For(0, hilos, i =>
            {
                instancias[i] = RedisMultiplexerPool.Obtener(ip, 0);
            });

            Assert.That(instancias, Has.All.SameAs(instancias[0]));
        }
    }
}
