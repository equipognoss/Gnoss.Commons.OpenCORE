using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BeetleX.Redis;
using Es.Riam.Gnoss.CL;

namespace Es.Riam.Gnoss.CL.Test
{
    public class RedisCacheWrapperTests
    {
        // RedisDB.Dispose() no es virtual (no se puede interceptar con override/spy) y es idempotente
        // mediante el campo privado "mIsDisposed". Al no exponer un IsDisposed p�blico, se comprueba
        // ese campo por reflexi�n para verificar si el fix realmente dispuso el cliente sustituido.
        private static bool EstaDispuesto(RedisDB cliente)
        {
            var campo = typeof(RedisDB).GetField("mIsDisposed", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(campo, Is.Not.Null, "RedisDB ya no expone el campo mIsDisposed; revisar el test tras un cambio de versi�n de BeetleX.Redis.");
            return (bool)campo!.GetValue(cliente)!;
        }

        private static RedisDB NuevoClienteSinHost(int db) => new RedisDB(db, new JsonFormater());

        [Test]
        public void AddRedisEscritura_SustituyeClienteExistente_DisponeElAnterior()
        {
            var wrapper = new RedisCacheWrapper();
            var primero = NuevoClienteSinHost(0);
            var segundo = NuevoClienteSinHost(1);

            wrapper.AddRedisEscritura("pool", primero);
            wrapper.AddRedisEscritura("pool", segundo);

            Assert.That(EstaDispuesto(primero), Is.True, "El cliente sustituido debe disponerse.");
            Assert.That(EstaDispuesto(segundo), Is.False, "El cliente vigente no debe disponerse.");
            Assert.That(wrapper.RedisEscritura("pool"), Is.SameAs(segundo));
        }

        [Test]
        public void AddRedisLectura_SustituyeClienteExistente_DisponeElAnterior()
        {
            var wrapper = new RedisCacheWrapper();
            var primero = NuevoClienteSinHost(0);
            var segundo = NuevoClienteSinHost(1);

            wrapper.AddRedisLectura("pool", primero);
            wrapper.AddRedisLectura("pool", segundo);

            Assert.That(EstaDispuesto(primero), Is.True, "El cliente sustituido debe disponerse.");
            Assert.That(EstaDispuesto(segundo), Is.False, "El cliente vigente no debe disponerse.");
            Assert.That(wrapper.RedisLectura("pool"), Is.SameAs(segundo));
        }

        [Test]
        public void AddRedisEscritura_MismoClienteYaRegistrado_NoSeDisponeASiMismo()
        {
            var wrapper = new RedisCacheWrapper();
            var cliente = NuevoClienteSinHost(0);

            wrapper.AddRedisEscritura("pool", cliente);
            wrapper.AddRedisEscritura("pool", cliente);

            Assert.That(EstaDispuesto(cliente), Is.False,
                "Re-registrar el mismo cliente (ReferenceEquals) no debe autodisponerlo.");
        }

        [Test]
        public void AddRedisEscritura_Concurrencia_SoloElClienteSustituidoSeDispone()
        {
            // Reproduce el escenario de riesgo del fix: varios hilos registran clientes distintos bajo la
            // misma clave de pool a la vez. AddOrUpdate puede invocar el updateValueFactory m�s de una vez
            // por reintento de CAS; el fix captura el "anterior" fuera del delegado para no depender de
            // cu�ntas veces se invoque. El resultado esperado: exactamente un cliente queda vivo (el
            // almacenado en el wrapper) y todos los dem�s quedan dispuestos.
            var wrapper = new RedisCacheWrapper();
            const int hilos = 32;
            var clientes = Enumerable.Range(0, hilos).Select(NuevoClienteSinHost).ToArray();

            Parallel.ForEach(clientes, cliente => wrapper.AddRedisEscritura("pool", cliente));

            var ganador = wrapper.RedisEscritura("pool");
            Assert.That(ganador, Is.Not.Null);
            Assert.That(clientes, Does.Contain(ganador));
            Assert.That(EstaDispuesto(ganador!), Is.False, "El cliente que queda registrado no debe disponerse.");

            foreach (var cliente in clientes.Where(c => !ReferenceEquals(c, ganador)))
            {
                Assert.That(EstaDispuesto(cliente), Is.True,
                    "Todo cliente sustituido debe quedar dispuesto (sin importar cu�ntas veces se reintent� AddOrUpdate).");
            }
        }
    }
}
