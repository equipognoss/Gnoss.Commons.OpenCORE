using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetVips;

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
        /// <returns>Devuelve la imagen ajustada</returns>
        public static Image AjustarImagen(Image pImagen, float pAncho, float pAlto)
        {
            if (pImagen == null)
                return null;

            double scale = Math.Min(
                pAncho / pImagen.Width,
                pAlto / pImagen.Height
            );

            return pImagen.Resize(scale);
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
            // Crop en NetVips (extract_area)
            var recortada = pImagen.Crop(pCoordenadaX, pCoordenadaY, pAnchura, pAltura);

            // Exportar a buffer (PNG)
            return recortada.WriteToBuffer(".png");
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
            if (pArrayByte == null || pArrayByte.Length == 0)
                return null;

            try
            {
                return Image.NewFromBuffer(pArrayByte);
            }
            catch
            {
                return null;
            }
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
            if (imageFile == null || imageFile.Length == 0)
                return null;

            var imagenOriginal = Image.NewFromBuffer(imageFile);

            // Asegurar que el recorte no se sale de la imagen
            targetX = Math.Max(0, targetX);
            targetY = Math.Max(0, targetY);

            int cropWidth = Math.Min(targetW, imagenOriginal.Width - targetX);
            int cropHeight = Math.Min(targetH, imagenOriginal.Height - targetY);

            // Crop
            var cropped = imagenOriginal.Crop(targetX, targetY, cropWidth, cropHeight);

            // Resize al tamaño final deseado
            double scaleX = (double)targetW / cropWidth;
            double scaleY = (double)targetH / cropHeight;

            return cropped.Resize(scaleX, vscale: scaleY);
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
        /// <summary>
        /// Recorta y redimensiona una imagen. Coordenadas en píxeles originales (viene de jCrop).
        /// </summary>
        public static byte[] CropImageFile(
            byte[] imageFile, int targetW, int targetH, int targetX, int targetY)
        {
            using var imagen = Image.NewFromBuffer(imageFile,
                access: Enums.Access.Sequential);

            // Validar que el recorte no se salga de los límites
            int anchoRecorte = Math.Min(targetW, imagen.Width - targetX);
            int altoRecorte = Math.Min(targetH, imagen.Height - targetY);

            if (anchoRecorte <= 0 || altoRecorte <= 0)
                throw new ArgumentException("Las coordenadas de recorte están fuera de la imagen.");

            var resultado = imagen
                .Crop(targetX, targetY, anchoRecorte, altoRecorte)
                .Resize((double)targetW / anchoRecorte);

            return resultado.WriteToBuffer(".png");
        }

        /// <summary>
        /// Sobrecarga con formato de salida explícito.
        /// </summary>
        public static byte[] CropImageFile(
            byte[] imageFile, int targetW, int targetH, int targetX, int targetY,
            string extensionArchivo)
        {
            using var imagen = Image.NewFromBuffer(imageFile,
                access: Enums.Access.Sequential);

            int anchoRecorte = Math.Min(targetW, imagen.Width - targetX);
            int altoRecorte = Math.Min(targetH, imagen.Height - targetY);

            if (anchoRecorte <= 0 || altoRecorte <= 0)
                throw new ArgumentException("Las coordenadas de recorte están fuera de la imagen.");

            var resultado = imagen
                .Crop(targetX, targetY, anchoRecorte, altoRecorte)
                .Resize((double)targetW / anchoRecorte);

            string extension = extensionArchivo.ToLower().TrimStart('.');
            string formato = ".webp";
            /*
            string formato = extension switch
            {
                "jpg" or "jpeg" => ".jpg",
                "png" => ".png",
                "bmp" => ".bmp",
                "gif" => ".gif",
                "webp" => ".webp",
                _ => throw new NotSupportedException($"Formato no soportado: {extensionArchivo}")
            };
            */
            return resultado.WriteToBuffer(formato);
        }

        public static byte[] ImageToBytePng(Image image)
        {
            if (image == null)
                return null;

            return image.WriteToBuffer(".png");
        }

        public static byte[] ImageToByteJpg(Image image)
        {
            if (image == null)
                return null;

            return image.WriteToBuffer(".jpg");
        }

        public static Image RecortarImagenACuadrada(Image pImagen, float tamaño)
        {
            if (pImagen == null)
                return null;

            int targetSize = (int)tamaño;
            float alto = pImagen.Height;
            float ancho = pImagen.Width;
            float proporcion = ancho / alto;

            Image working;

            if (Math.Abs(proporcion - 1f) < float.Epsilon)
            {
                working = RedimensionarDadoAncho(pImagen, tamaño);
            }
            else if (proporcion < 1f)
            {
                if (ancho >= tamaño)
                {
                    working = RedimensionarDadoAncho(pImagen, tamaño);
                    int cropHeight = Math.Min(working.Height, targetSize);
                    working = working.Crop(0, 0, targetSize, cropHeight);
                }
                else
                {
                    int cropHeight = Math.Min((int)alto, targetSize);
                    working = pImagen.Crop(0, 0, (int)ancho, cropHeight);
                }
            }
            else
            {
                if (alto >= tamaño)
                {
                    working = RedimensionarDadoAlto(pImagen, tamaño);
                    int offsetX = (working.Width - targetSize) / 2;
                    offsetX = Math.Max(0, offsetX);
                    int cropWidth = Math.Min(working.Width - offsetX, targetSize);
                    working = working.Crop(offsetX, 0, cropWidth, targetSize);
                }
                else
                {
                    int offsetX = ((int)ancho - targetSize) / 2;
                    offsetX = Math.Max(0, offsetX);
                    int cropWidth = Math.Min((int)ancho - offsetX, targetSize);
                    working = pImagen.Crop(offsetX, 0, cropWidth, (int)alto);
                }
            }

            return working;
        }

        public static Image RedimensionarDadoAncho(Image pImagen, float pAnchoPixeles)
        {
            if (pAnchoPixeles >= pImagen.Width)
                return pImagen; 

            double scale = pAnchoPixeles / pImagen.Width;
            return pImagen.Resize(scale);
        }

        public static Image RedimensionarDadoAlto(Image pImagen, float pAltoPixeles)
        {
            if (pAltoPixeles >= pImagen.Height)
                return pImagen; 

            double scale = pAltoPixeles / pImagen.Height;
            // Resize de NetVips escala por ancho; usamos vscale para forzar por alto
            return pImagen.Resize(scale, vscale: scale);
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
        /// Redimensionamos una imagen cumpliendo el límite de ancho y alto pasado por parámetro y manteniendo la proporcion.
        /// En caso de que uno sea -1 se redimensionará la imagen en función del otro parámetro manteniendo la relación de aspecto.
        /// </summary>
        /// <param name="pAncho">Ancho deseado para la redimensión</param>
        /// <param name="pAlto">Alto deseado para la redimensión</param>
        /// <param name="pImagenOriginal">Imagen que queremos redimensionar</param>
        /// <returns>Array de bytes con el contenido de la imagen redimensionada en PNG</returns>
        /// <exception cref="InvalidDataException">Cuando ambos parámetros son -1</exception>
        public static byte[] RedimensionarAnchoAlto(int pAncho, int pAlto, Image pImagenOriginal)
        {
            if (pAncho == -1 && pAlto == -1)
                throw new InvalidDataException("No se ha configurado ni el alto ni el ancho del recorte, hay que configurar al menos un valor");

            float factorRedimension = ObtenerFactorRedimension(pAncho, pAlto, pImagenOriginal.Width, pImagenOriginal.Height);

            int anchoFinal = (int)(pImagenOriginal.Width / factorRedimension);
            int altoFinal = (int)(pImagenOriginal.Height / factorRedimension);

            // NetVips Resize trabaja con scale, no con dimensiones absolutas.
            // Calculamos el scale horizontal; vscale fuerza la altura exacta.
            double hscale = (double)anchoFinal / pImagenOriginal.Width;
            double vscale = (double)altoFinal / pImagenOriginal.Height;

            var resized = pImagenOriginal.Resize(hscale, vscale: vscale);

            return resized.WebpsaveBuffer(q: 75);
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
