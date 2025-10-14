using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Util
{
    public class UtilImages
    {
        /// <summary>
        /// Ajusta una imagen proporcionalmente
        /// </summary>
        /// <param name="pImagen">Imagen a ajustar</param>
        /// <param name="pAncho">Ancho de ajuste</param>
        /// <param name="pAlto">Alto de ajuste</param>
        /// <param name="pCalcularTamañoProporcionado">TRUE si se debe calcular el tamaño proporcionadamente</param>
        /// <returns>Devuelve la imagen ajustada</returns>
        public static Image AjustarImagen(Image pImagen, float pAncho, float pAlto, bool pCalcularTamañoProporcionado)
        {
            Size tamanio = new Size((int)pAncho, (int)pAlto);

            if (pCalcularTamañoProporcionado)
            {
                SizeF tamanioF = CalcularTamanioProporcionado(pImagen, pAncho, pAlto);
                tamanio = Size.Truncate(tamanioF);
            }

            Image imageClone = pImagen.Clone(c => c.Resize(tamanio));
            return imageClone;
        }

        /// <summary>
        /// Ajusta una imagen proporcionalmente
        /// </summary>M
        /// <param name="pImagen">Imagen a ajustar</param>
        /// <param name="pAncho">Ancho de ajuste</param>
        /// <param name="pAlto">Alto de ajuste</param>
        /// <returns>Devuelve la imagen ajustada</returns>
        public static Image AjustarImagen(Image pImagen, float pAncho, float pAlto)
        {
            return AjustarImagen(pImagen, pAncho, pAlto, true);
        }

        /// <summary>
        /// Calcula el tamaño proporcional en el que quedará 
        /// </summary>
        /// <param name="pImagen">Imagen que se va a proporcionar</param>
        /// <param name="pAlto">Alto a ajustar</param>
        /// <param name="pAncho">Ancho a ajustar</param>
        /// <returns>Tamaño ajustado</returns>
        public static SizeF CalcularTamanioProporcionado(Image pImagen, float pAncho, float pAlto)
        {
            if (pImagen != null)
            {
                float Alto = pImagen.Height;
                float Ancho = pImagen.Width;
                float AltoFinal;
                float AnchoFinal;

                if ((Alto / Ancho) >= (pAlto / pAncho))
                {
                    AnchoFinal = Ancho / (Alto / pAlto);
                    AltoFinal = pAlto;
                }
                else
                {
                    AltoFinal = Alto / (Ancho / pAncho);
                    AnchoFinal = pAncho;
                }

                return new SizeF(AnchoFinal, AltoFinal);
            }
            return new SizeF(pAncho, pAlto);
        }

        /// <summary>
        /// Ajusta una imagen proporcionalmente
        /// </summary>
        /// <param name="pImagen">Imagen a ajustar</param>
        /// <param name="pAncho">Ancho de ajuste</param>
        /// <param name="pAlto">Alto de ajuste</param>
        /// <returns>Imagen ajustada</returns>
        public static Image AjustarImagen(Image pImagen, int pAncho, int pAlto)
        {
            return AjustarImagen(pImagen, (float)pAncho, (float)pAlto);
        }

        /// <summary>
        /// Ajusta una imagen proporcionalmente
        /// </summary>
        /// <param name="pImagen">Imagen a ajustar</param>
        /// <param name="pTamano">Tamaño final de la imagen</param>
        /// <returns>Imagen ajustada</returns>
        public static Image AjustarImagen(Image pImagen, Size pTamano)
        {
            return AjustarImagen(pImagen, pTamano.Width, pTamano.Height);
        }

        /// <summary>
        /// Recorta de una imágen un trozo con la altura y anchura deseada.
        /// </summary>
        /// <param name="pImagen">Imágen a recortar</param>
        /// <param name="pAnchura">Anchura del recorte</param>
        /// <param name="pAltura">Altura del recorte</param>
        /// <param name="pCoordenadaX">Punto del eje de coordenadas X desde el que se empieza a recortar</param>
        /// <param name="pCoordenadaY">Punto del eje de coordenadas y desde el que se empieza a recortar</param>
        /// <returns>Array con los bytes de la nueva imagen recortada</returns>
        public static byte[] RecortarImagen(Image pImagen, int pAnchura, int pAltura, int pCoordenadaX, int pCoordenadaY)
        {
            pImagen.Clone(i => i.Crop(new Rectangle(pCoordenadaX, pCoordenadaY, pAnchura, pAltura)));
            byte[] bufferRecortado;
            using (var ms = new MemoryStream())
            {
                pImagen.Save(ms, PngFormat.Instance);
                bufferRecortado = ms.ToArray();
            }
            return bufferRecortado;
        }

        /// <summary>
        /// Codifica una imagen a BASE64
        /// </summary>
        /// <param name="pImagen">Array de bytes que representa la imagen</param>
        /// <returns>Cadena de texto en formato BASE64 que representa la imagen</returns>
        public static string CodificarImagen(byte[] pImagen)
        {
            return Convert.ToBase64String(pImagen);
        }

        /// <summary>
        /// Convierte un array de bytes en una imagen
        /// </summary>
        /// <param name="pArrayByte">Array de bytes a convertir</param>
        /// <returns>Imagen del array de bytes</returns>
        public static Image ConvertirArrayBytesEnImagen(byte[] pArrayByte)
        {
            if (pArrayByte.Length.Equals(0))
                return null;

            MemoryStream ms = new MemoryStream(pArrayByte);
            Image returnImage = Image.Load(ms);
            ms.Close();
            ms.Dispose();

            return returnImage;
        }

        /// <summary>
        /// Recorta una imagen pasada por parámetro
        /// </summary>
        /// <param name="imageFile">Array de bytes que representa a la imagen</param>
        /// <param name="targetW">Anchura</param>
        /// <param name="targetH">Altura</param>
        /// <param name="targetX">Coordenada X</param>
        /// <param name="targetY">Coordenada Y</param>
        /// <returns>Array de bytes</returns>
        public static Image CropImage(byte[] imageFile, int targetW, int targetH, int targetX, int targetY)
        {
            Image imagenOriginal = Image.Load(new MemoryStream(imageFile));
            Rectangle rectanguloRecorte = new Rectangle(targetX, targetY, targetW - targetX, targetH - targetY);

            var imagenCrop = imagenOriginal.Clone(x => x.Crop(rectanguloRecorte).Resize(targetW, targetH));

            return imagenCrop;
        }

        /// <summary>
        /// Recorta una imagen pasada por parámetro
        /// </summary>
        /// <param name="imageFile">Array de bytes que representa a la imagen</param>
        /// <param name="targetW">Anchura</param>
        /// <param name="targetH">Altura</param>
        /// <param name="targetX">Coordenada X</param>
        /// <param name="targetY">Coordenada Y</param>
        /// <returns>Array de bytes</returns>
        public static byte[] CropImageFile(byte[] imageFile, int targetW, int targetH, int targetX, int targetY)
        {
            var cropImage = CropImage(imageFile, targetW, targetH, targetX, targetY);
            return ImageToBytePng(cropImage);
        }

        public static byte[] CropImageFile(byte[] imageFile, int targetW, int targetH, int targetX, int targetY, string extensionArchivo)
        {
            using (MemoryStream stream = new MemoryStream(imageFile))
            {
                Image imagenOriginal = Image.Load(new MemoryStream(imageFile));
                Rectangle rectanguloRecorte = new Rectangle(targetX, targetY, targetW, targetH);

                var imagenRecortada = imagenOriginal.Clone(x => x.Crop(rectanguloRecorte).Resize(targetW, targetH));

                MemoryStream mm = new MemoryStream();
                if (extensionArchivo.ToLower().Equals("jpeg") || string.Compare(extensionArchivo, "jpg", true) == 0)
                {
                    imagenRecortada.SaveAsJpeg(mm);
                }
                if (extensionArchivo.ToLower().Equals("png"))
                {
                    imagenRecortada.SaveAsPng(mm);
                }
                if (extensionArchivo.ToLower().Equals("bmp"))
                {
                    imagenRecortada.SaveAsBmp(mm);
                }
                if (extensionArchivo.ToLower().Equals("gif"))
                {
                    imagenRecortada.SaveAsGif(mm);
                }
                return mm.GetBuffer();
            }
        }

        public static byte[] ImageToBytePng(Image image)
        {
            MemoryStream stream = new MemoryStream();
            image.SaveAsPng(stream);
            return stream.GetBuffer();
        }
        public static byte[] ImageToByteJpg(Image image)
        {
            MemoryStream stream = new MemoryStream();
            image.SaveAsJpeg(stream);
            return stream.GetBuffer();
        }

        public static Image RecortarImagenACuadrada(Image pImagen, float tamaño)
        {
            float alto = pImagen.Height;
            float ancho = pImagen.Width;
            bool esVertical = false;
            bool esHorizontal = false;
            bool esCuadrada = false;
            float proporcionAnchoAlto = ancho / alto;
            Image imageClone = pImagen;
            if (proporcionAnchoAlto == 1)
            {
                esCuadrada = true;
            }
            else if (proporcionAnchoAlto > 1)
            {
                esHorizontal = true;
            }
            else if (proporcionAnchoAlto < 1)
            {
                esVertical = true;
            }

            if (esVertical)
            {
                if (ancho >= tamaño)
                {
                    Image imagen = RedimensionarDadoAncho(pImagen, tamaño);
                    Point origen = new Point(0, 0);
                    Size size = Size.Truncate(new SizeF(tamaño, tamaño));
                    Rectangle rectangulo = new Rectangle(origen, size);
                    imageClone = imagen.Clone(x => x.Crop(rectangulo));
                }
                else if (ancho < tamaño && alto > tamaño)
                {
                    Point origen = new Point(0, 0);
                    Size size = Size.Truncate(new SizeF(ancho, tamaño));
                    Rectangle rectangulo = new Rectangle(origen, size);
                    imageClone = pImagen.Clone(x => x.Crop(rectangulo));
                }

            }
            else if (esHorizontal)
            {
                if (alto >= tamaño)
                {
                    Image imagen = RedimensionarDadoAlto(pImagen, tamaño);
                    Point origen = new Point(Convert.ToInt32(imagen.Width - tamaño) / 2, 0);
                    Size size = Size.Truncate(new SizeF(tamaño, tamaño));
                    Rectangle rectangulo = new Rectangle(origen, size);
                    imageClone = imagen.Clone(x => x.Crop(rectangulo));
                }
                else if (alto <= tamaño && ancho > tamaño)
                {
                    Point origen = new Point(Convert.ToInt32(pImagen.Width - tamaño) / 2, 0);
                    Size size = Size.Truncate(new SizeF(tamaño, alto));
                    Rectangle rectangulo = new Rectangle(origen, size);
                    imageClone = pImagen.Clone(x => x.Crop(rectangulo));
                }
            }
            else if (esCuadrada)
            {
                Image imagenCuadrada = RedimensionarDadoAncho(pImagen, tamaño);

                imageClone = imagenCuadrada;
            }

            return imageClone;
        }

        public static Image RedimensionarDadoAncho(Image pImagen, float pAnchoPixeles)
        {
            float alto = pImagen.Height;
            float ancho = pImagen.Width;
            Image imageClone = pImagen;
            float proporcionAnchoAlto = ancho / alto;

            // Sólo redimensionamos la imagen si el ancho es menor que el pedido
            if (pAnchoPixeles < ancho)
            {
                float nuevoAlto = pAnchoPixeles / proporcionAnchoAlto;
                Size size = Size.Truncate(new SizeF(pAnchoPixeles, nuevoAlto));
                imageClone = pImagen.Clone(x => x.Resize(size));
            }
            return imageClone;
        }

        public static Image RedimensionarDadoAlto(Image pImagen, float pAltoPixeles)
        {
            float alto = pImagen.Height;
            float ancho = pImagen.Width;
            float proporcionAnchoAlto = ancho / alto;
            Image imageClone = pImagen;
            // Sólo redimensionamos la imagen si el alto es menor que el pedido
            if (pAltoPixeles < alto)
            {
                float nuevoAncho = pAltoPixeles * proporcionAnchoAlto;
                Size size = Size.Truncate(new SizeF(nuevoAncho, pAltoPixeles));
                imageClone = pImagen.Clone(x => x.Resize(size));
            }
            return imageClone;
        }

        /// <summary>
        /// Realizamos un recorte cuadrado de la imagen pasada por parámetro del tamaño pasado por parámetro
        /// </summary>
        /// <param name="pRecorte">Tamaño que tendrá el recorte de la imagen</param>
        /// <param name="pImagenOriginal">Imagen que queremos recortar</param>
        /// <returns>Array de bytes con el contenido de la imagen recortada</returns>
        public static byte[] RealizarRecorteCuadrado(int pRecorte, Image pImagenOriginal)
        {
            Image imagenPeque = RecortarImagenACuadrada(pImagenOriginal, pRecorte);

            if (imagenPeque.Width > pImagenOriginal.Width || imagenPeque.Height > pImagenOriginal.Height)
            {
                imagenPeque = pImagenOriginal;
            }

            return ImageToBytePng(imagenPeque);
        }

        /// <summary>
        /// Redimensionamos una imagen cumpliendo el límite de ancho y alto pasado por parámetro y manteniendo la proporcion. En caso de que uno sea -1 se redimensionará la imagen en función
        /// del otro parámetro manteniendo la relación de aspecto.
        /// </summary>
        /// <param name="pAncho">Ancho deseado para la redimensión</param>
        /// <param name="pAlto">Alto deseado para la redimensión</param>
        /// <param name="pImagenOriginal">Imagen que queremos redimensionar</param>
        /// <returns>Array de bytes con el contenido de la imagen redimensionada</returns>
        /// <exception cref="InvalidDataException">Devolvemos InvalidDataException en caso de que ambos parámetros sean -1</exception>
        public static byte[] RedimensionarAnchoAlto(int pAncho, int pAlto, Image pImagenOriginal)
        {
            byte[] bytesImagenRedimensionada = null;

            if (pAncho == -1 && pAlto == -1)
            {
                throw new InvalidDataException("No se ha configurado ni el alto ni el ancho del recorte, hay que configurar al menos un valor");
            }

            float factorRedimension = ObtenerFactorRedimension(pAncho, pAlto, pImagenOriginal.Width, pImagenOriginal.Height);

            int anchoFinal = (int)(pImagenOriginal.Width / factorRedimension);
            int altoFinal = (int)(pImagenOriginal.Height / factorRedimension);            
            
            Image imagenNueva = pImagenOriginal.Clone(x => x.Resize(anchoFinal, altoFinal));

            using (var ms = new MemoryStream())
            {
                PngEncoder pngEncoder = new PngEncoder() { CompressionLevel = PngCompressionLevel.Level9 };
                imagenNueva.Save(ms, pngEncoder);
                bytesImagenRedimensionada = ms.ToArray();
            }

            return bytesImagenRedimensionada;
        }

        /// <summary>
        /// Calculamos el factor de redimensión que deberemos aplicar a partir de un ancho y alto límite y el ancho y alto de la imagen original.
        /// </summary>
        /// <param name="pAnchoLimite">Ancho límite del recorte</param>
        /// <param name="pAltoLimite">Alto límite del recorte</param>
        /// <param name="pAnchoImagen">Ancho de la imagen original</param>
        /// <param name="pAltoImagen">Alto de la imagen original</param>
        /// <returns></returns>
        protected static float ObtenerFactorRedimension(int pAnchoLimite, int pAltoLimite, int pAnchoImagen, int pAltoImagen)
        {
            float factorAncho = (float)pAnchoImagen / pAnchoLimite;
            float factorAlto = (float)pAltoImagen / pAltoLimite;

            float factorRedimension = factorAlto > factorAncho ? factorAlto : factorAncho;

            return factorRedimension > 1 ? factorRedimension : 1;
        }
    }
}
