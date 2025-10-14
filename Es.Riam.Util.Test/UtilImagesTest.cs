using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Util.Test
{
    internal class UtilImagesTest : UtilImages
    {
        [SetUp]
        public void Setup()
        { }

        [Test]
        [TestCase(600, 400, 599, 400, 4501, 3001)]
        [TestCase(600, 100, 149, 100, 4501, 3001)]
        [TestCase(100, 400, 100, 66, 4501, 3001)]
        [TestCase(8000, 6500, 4501, 3001, 4501, 3001)]
        [TestCase(-1, 300, 449, 300, 4501, 3001)]
        public void RedimensionarImagen(int anchoLimite, int altoLimite, int anchoEsperado, int altoEsperado, int anchoImagenOriginal, int altoImagenOriginal)
        {
            float factorRedimension = ObtenerFactorRedimension(anchoLimite, altoLimite, anchoImagenOriginal, altoImagenOriginal);

            int anchoFinal = (int)(anchoImagenOriginal / factorRedimension);
            int altoFinal = (int)(altoImagenOriginal / factorRedimension);

            Assert.That(anchoFinal, Is.EqualTo(anchoEsperado));
            Assert.That(altoFinal, Is.EqualTo(altoEsperado));            
        }
    }
}
