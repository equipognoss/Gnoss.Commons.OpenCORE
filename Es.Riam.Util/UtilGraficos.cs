using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Drawing2D;
//using System.Drawing.Imaging;
using System.IO;

namespace Es.Riam.Util
{
    /// <summary>
    /// Útiles de gráficos
    /// </summary>
    public class UtilGraficos
    {
        #region Enumeraciones

        /// <summary>
        /// Estado de las uniones entre elementos
        /// </summary>
        public enum EstadoUnion
        {
            /// <summary>
            /// Vinculaciones fijas entre dos entidades
            /// </summary>
            Fijo = 0,
            /// <summary>
            /// Vinculaciones añadidas que todavía no se consideran fijas
            /// </summary>
            TemporalAñadido = 1,
            /// <summary>
            /// Vinculaciones marcadas para eliminar
            /// </summary>
            TemporalEliminado = 2
        }

        /// <summary>
        /// Especifica las posibles orientaciones de una línea redondeada
        /// </summary>
        public enum OrientacionesLineaRedondeada
        {
            /// <summary>
            /// Orientación Norte
            /// </summary>
            Norte,
            /// <summary>
            /// Orientación sur
            /// </summary>
            Sur,
            /// <summary>
            /// Orientación este
            /// </summary>
            Este,
            /// <summary>
            /// Orientación oeste
            /// </summary>
            Oeste,
            /// <summary>
            /// No es ninguna de las anteriores por lo tanto no es correcta
            /// </summary>
            NingunaCorrecta
        }

        /// <summary>
        /// Especifica la forma de mostrar un texto
        /// </summary>
        public enum OrientacionesTexto
        {
            /// <summary>
            /// Orientación norte
            /// </summary>
            Norte,
            /// <summary>
            /// Orientación sur
            /// </summary>
            Sur,
            /// <summary>
            /// Orientación este
            /// </summary>
            Este,
            /// <summary>
            /// Orientación oeste
            /// </summary>
            Oeste,
            /// <summary>
            /// Orientación noreste
            /// </summary>
            NorEste,
            /// <summary>
            /// Orientación noroeste
            /// </summary>
            NorOeste,
            /// <summary>
            /// Orientación sudeste
            /// </summary>
            SudEste,
            /// <summary>
            /// Orientación sudoeste
            /// </summary>
            SudOeste
        }

        /// <summary>
        /// Especifica si el elemento está seleccionado o no y en qué posición está, si es para un cambio de tamaño vertical, o un cambio diagonal...
        /// </summary>
        public enum TipoSeleccionElementoGráfico
        {
            /// <summary>
            /// Ninguno
            /// </summary>
            Ninguno,
            /// <summary>
            /// Centro
            /// </summary>
            Centro,
            /// <summary>
            /// Izquierda arriba
            /// </summary>
            IzquierdaArriba,
            /// <summary>
            /// Arriba
            /// </summary>
            Arriba,
            /// <summary>
            /// Derecha arriba
            /// </summary>
            DerechaArriba,
            /// <summary>
            /// Derecha
            /// </summary>
            Derecha,
            /// <summary>
            /// Derecha abajo
            /// </summary>
            DerechaAbajo,
            /// <summary>
            /// Abajo
            /// </summary>
            Abajo,
            /// <summary>
            /// Izquierda abajo
            /// </summary>
            IzquierdaAbajo,
            /// <summary>
            /// Izquierda
            /// </summary>
            Izquierda,
            /// <summary>
            /// Posición más-menos
            /// </summary>
            MasMenos
        }

        /// <summary>
        /// Especifica la orientación posible de una línea
        /// </summary>
        public enum OrientacionesLinea
        {
            /// <summary>
            /// Línea horizontal
            /// </summary>
            Horizontal,
            /// <summary>
            /// Línea vertical
            /// </summary>
            Vertical
        }

        #endregion

        #region Miembros

        #region Privados

        private static Image mIconoMas;

        private static Image mIconoMenos;

        /// <summary>
        /// Almacena la trama de líneas diagonales
        /// </summary>
        private static Brush mTramaLineasDiagonales = null;

        /// <summary>
        /// Formato de cadena centrada
        /// </summary>
        private static StringFormat mCadenaCentrada;

        /// <summary>
        /// Formato de cadena alineada a la izquierda en horizontal y centrada en vertical
        /// </summary>
        private static StringFormat mCadenaIzquierda;

        private static Font mVerdana6 = new Font("verdana", 6);

        private static Font mVerdana6Negrita = new Font("verdana", 6, FontStyle.Bold);

        /// <summary>
        /// Fuente verdana 7
        /// </summary>
        private static Font mVerdana7 = new Font("verdana", 7);

        private static Font mVerdana8 = new Font("verdana", 8);

        private static Font mVerdana8Negrita = new Font("verdana", 8, FontStyle.Bold);

        private static Font mVerdana10 = new Font("verdana", 10);

        private static Font mVerdana10Negrita = new Font("verdana", 10, FontStyle.Bold);

        private static Font mFuenteCelda = new Font("verdana", 8);

        private static Pen mTrazoNegro;

        private static Pen mTrazoNegroFino;

        private static Pen mTrazoGuionesNegro;

        private static Pen mTrazoPuntosNegro;

        private static Pen mTrazoPuntosNegroEscalado;

        private static Pen mLapizColorSeleccionado;

        private static Pen mLapizColorSeleccionado4;

        private static Pen mLapizColorArrastreFlecha;

        private static SolidBrush mBrochaColorArrastreFlecha;

        private static SolidBrush mBrochaColorSeleccionado;

        private static LinearGradientBrush mDegradadoCeldaSeleccionada = null;

        /// <summary>
        /// Lista de colores básicos
        /// </summary>
        private static List<Color> mColoresBasicos;

        #endregion

        #region Públicos

        /// <summary>
        /// Tamaño del rectángulo de selección
        /// </summary>
        public static int TamañoRectanguloSeleccion = 4;

        /// <summary>
        /// Color del diagrama idef0
        /// </summary>
        public static Color ColorDiagramaIdef0 = Color.FromArgb(245, 245, 245);
        /// <summary>
        /// Color de las entradas idef0
        /// </summary>
        public static Color ColorEntradaIdef0 = Color.Gray;
        /// <summary>
        /// Color de las salidas idef0
        /// </summary>
        public static Color ColorSalidaIdef0 = Color.Green;
        /// <summary>
        /// Color de los mecanismos idef0
        /// </summary>
        public static Color ColorMecanismoIdef0 = Color.Fuchsia;
        /// <summary>
        /// Color de los controles idef0
        /// </summary>
        public static Color ColorControlIdef0 = Color.Red;

        #endregion

        #endregion

        #region Propiedades

        /// <summary>
        /// Devuelve o establece el icono de mas
        /// </summary>
        public static Image IconoMas
        {
            get
            {
                if (mIconoMas == null)
                    mIconoMas = new Bitmap(typeof(UtilGraficos), "Imagenes.Mas.bmp");
                return mIconoMas;
            }
            set
            {
                mIconoMas = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el icono de menos
        /// </summary>
        public static Image IconoMenos
        {
            get
            {
                if (mIconoMenos == null)
                    mIconoMenos = new Bitmap(typeof(UtilGraficos), "Imagenes.Menos.bmp");
                return mIconoMenos;
            }
            set
            {
                mIconoMenos = value;
            }
        }

        /// <summary>
        /// Obtiene el ancho de los separadores de las casillas de las celdas
        /// </summary>
        public static float GrosorBordeCasilla
        {
            get
            {
                return 1f;
            }
        }

        /// <summary>
        /// Obtiene el color del borde de las casillas de las celdas
        /// </summary>
        public static Color ColorBordeCasilla
        {
            get
            {
                return Color.White;
            }
        }

        /// <summary>
        /// Devuelve el color para un elemento seleccionado
        /// </summary>
        public static Color ColorSeleccionado
        {
            get { return Color.FromArgb(158, 190, 245); }
        }

        /// <summary>
        /// Devuelve el color para un arrastre de flecha
        /// </summary>
        public static Color ColorArrastreFlecha
        {
            get { return Color.FromArgb(185, 237, 161); }
        }

        /// <summary>
        /// Devuelve el formato de cadena centrada
        /// </summary>
        public static StringFormat CadenaCentrada
        {
            get
            {
                if (mCadenaCentrada == null)
                {
                    mCadenaCentrada = new StringFormat();
                    mCadenaCentrada.Alignment = StringAlignment.Center;
                    mCadenaCentrada.LineAlignment = StringAlignment.Center;
                }
                return mCadenaCentrada;
            }
        }

        /// <summary>
        /// Devuelve el formato de cadena alineada a la izquierda en horizontal y centrada en vertical
        /// </summary>
        public static StringFormat CadenaIzquierda
        {
            get
            {
                if (mCadenaIzquierda == null)
                {
                    mCadenaIzquierda = new StringFormat();
                    mCadenaIzquierda.Alignment = StringAlignment.Near;
                    mCadenaIzquierda.LineAlignment = StringAlignment.Center;
                }
                return mCadenaIzquierda;
            }
        }

        /// <summary>
        /// Obtiene la fuente verdana 6
        /// </summary>
        public static Font Verdana6
        {
            get
            {
                return mVerdana6;
            }
        }

        /// <summary>
        /// Obtiene la fuente verdana 6 en negrita
        /// </summary>
        public static Font Verdana6Negrita
        {
            get
            {
                return mVerdana6Negrita;
            }
        }

        /// <summary>
        /// Obtiene la fuente verdana 7
        /// </summary>
        public static Font Verdana7
        {
            get
            {
                return mVerdana7;
            }
        }

        /// <summary>
        /// Obtiene la fuente verdana 8
        /// </summary>
        public static Font Verdana8
        {
            get
            {
                return mVerdana8;
            }
        }

        /// <summary>
        /// Obtiene la fuente verdana 8 en negrita
        /// </summary>
        public static Font Verdana8Negrita
        {
            get
            {
                return mVerdana8Negrita;
            }
        }

        /// <summary>
        /// Obtiene la fuente verdana 10
        /// </summary>
        public static Font Verdana10
        {
            get
            {
                return mVerdana10;
            }
        }

        /// <summary>
        /// Obtiene la fuente verdana 10 en negrita
        /// </summary>
        public static Font Verdana10Negrita
        {
            get
            {
                return mVerdana10Negrita;
            }
        }

        /// <summary>
        /// Obtiene la fuente de una celda
        /// </summary>
        public static Font FuenteCelda
        {
            get
            {
                return mFuenteCelda;
            }
        }

        /// <summary>
        /// Obtiene la fuente de una fila
        /// </summary>
        public static Font FuenteFila
        {
            get
            {
                return mVerdana8;
            }
        }

        /// <summary>
        /// Obtiene el trazo negro
        /// </summary>
        public static Pen TrazoNegro
        {
            get
            {
                if (mTrazoNegro == null)
                    mTrazoNegro = new Pen(Color.Black, 1);
                return mTrazoNegro;
            }
        }

        /// <summary>
        /// Obtiene el trazo negro fino
        /// </summary>
        public static Pen TrazoNegroFino
        {
            get
            {
                if (mTrazoNegroFino == null)
                    mTrazoNegroFino = new Pen(Color.Gray, 0.1f);
                return mTrazoNegroFino;
            }
        }

        /// <summary>
        /// Obtiene el trazo negro con forma de guiones
        /// </summary>
        public static Pen TrazoGuionesNegro
        {
            get
            {
                if (mTrazoGuionesNegro == null)
                {
                    mTrazoGuionesNegro = new Pen(Color.Black, 1);
                    mTrazoGuionesNegro.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
                    mTrazoGuionesNegro.DashPattern = new float[] { 4.0F, 4.0F };
                }
                return mTrazoGuionesNegro;
            }
        }

        /// <summary>
        /// Obtiene el trazo negro con forma de puntos
        /// </summary>
        public static Pen TrazoPuntosNegro
        {
            get
            {
                if (mTrazoPuntosNegro == null)
                {
                    mTrazoPuntosNegro = new Pen(Color.Black, 1);
                    mTrazoPuntosNegro.DashStyle = DashStyle.Dot;
                    mTrazoPuntosNegro.DashPattern = new float[] { 2.0F, 2.0F };
                }
                return mTrazoPuntosNegro;
            }
        }

        /// <summary>
        /// Obtiene el trazo negro con forma de puntos escalado
        /// </summary>
        public static Pen TrazoPuntosNegroEscalado(float Escala)
        {
            if (mTrazoPuntosNegroEscalado == null)
            {
                mTrazoPuntosNegroEscalado = new Pen(Color.Black, 1f / Escala);
                mTrazoPuntosNegroEscalado.DashStyle = DashStyle.Dot;
                mTrazoPuntosNegroEscalado.DashPattern = new float[] { 2.0F, 2.0F };
            }
            return mTrazoPuntosNegroEscalado;
        }

        /// <summary>
        /// Obtiene el lápiz del color seleccionado
        /// </summary>
        public static Pen LapizColorSeleccionado
        {
            get
            {
                if (mLapizColorSeleccionado == null)
                    mLapizColorSeleccionado = new Pen(ColorSeleccionado);
                return mLapizColorSeleccionado;
            }
        }

        /// <summary>
        /// Obtiene el lápiz del color seleccionado y tamaño 4
        /// </summary>
        public static Pen LapizColorSeleccionado4
        {
            get
            {
                if (mLapizColorSeleccionado4 == null)
                    mLapizColorSeleccionado4 = new Pen(ColorSeleccionado, 4);
                return mLapizColorSeleccionado4;
            }
        }

        /// <summary>
        /// Obtiene el lápiz del color de arrastre de flecha
        /// </summary>
        public static Pen LapizColorArrastreFlecha
        {
            get
            {
                if (mLapizColorArrastreFlecha == null)
                    mLapizColorArrastreFlecha = new Pen(ColorArrastreFlecha);
                return mLapizColorArrastreFlecha;
            }
        }

        /// <summary>
        /// Obtiene la brocha del color de arrastre de flecha
        /// </summary>
        public static SolidBrush BrochaColorArrastreFlecha
        {
            get
            {
                if (mBrochaColorArrastreFlecha == null)
                    mBrochaColorArrastreFlecha = new SolidBrush(ColorArrastreFlecha);
                return mBrochaColorArrastreFlecha;
            }
        }

        /// <summary>
        /// Obtiene la brocha del color seleccionado
        /// </summary>
        public static SolidBrush BrochaColorSeleccionado
        {
            get
            {
                if (mBrochaColorSeleccionado == null)
                    mBrochaColorSeleccionado = new SolidBrush(ColorSeleccionado);
                return mBrochaColorSeleccionado;
            }
        }

        /// <summary>
        /// Obtiene una trama de líneas diagonales
        /// </summary>
        public static Brush TramaLineasDiagonales
        {
            get
            {
                if (UtilGraficos.mTramaLineasDiagonales == null)
                    UtilGraficos.mTramaLineasDiagonales = new HatchBrush(HatchStyle.ForwardDiagonal, Color.Gray, Color.White);
                return UtilGraficos.mTramaLineasDiagonales;
            }
        }

        /// <summary>
        /// Devuelve la lista de colores básicos
        /// </summary>
        public static List<Color> ColoresBasicos
        {
            get
            {
                if (mColoresBasicos == null)
                {
                    mColoresBasicos = new List<Color>();
                    mColoresBasicos.Add(Color.Blue);
                    mColoresBasicos.Add(Color.Orange);
                    mColoresBasicos.Add(Color.Green);
                    mColoresBasicos.Add(Color.Violet);
                }
                return mColoresBasicos;
            }
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Establece un borde a una imagen 
        /// </summary>
        /// <param name="imagen">imagen original</param>
        /// <param name="color">color del borde</param>
        /// <param name="grosor">grosor</param>
        /// <returns>imagen con borde (MemoryStream)</returns>
        public static MemoryStream PonerBorde(Stream imagen, Color color, float grosor)
        {
            System.Drawing.Image original = System.Drawing.Image.FromStream(imagen);
            System.Drawing.Image imgPhoto = System.Drawing.Image.FromStream(imagen);

            Bitmap bmPhoto = new Bitmap(original.Width + Convert.ToInt32(grosor * 2), original.Height + Convert.ToInt32(grosor * 2));
            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.SmoothingMode = SmoothingMode.HighQuality;
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;

            Rectangle recImage = new Rectangle(System.Convert.ToInt32(grosor), System.Convert.ToInt32(grosor), original.Width, original.Height);
            grPhoto.DrawImage(imgPhoto, recImage);
            Rectangle recBorde = new Rectangle(System.Convert.ToInt32(grosor / 2), System.Convert.ToInt32(grosor / 2), original.Width + System.Convert.ToInt32(grosor), original.Height + System.Convert.ToInt32(grosor));
            Pen pen = new Pen(color, grosor);
            grPhoto.DrawRectangle(pen, recBorde);

            MemoryStream mm = new MemoryStream();

            bmPhoto.Save(mm, System.Drawing.Imaging.ImageFormat.Jpeg);

            // Cerramos todo lo cerrable
            original.Dispose();
            imgPhoto.Dispose();
            bmPhoto.Dispose();
            grPhoto.Dispose();

            return mm;
        }

        /// <summary>
        /// Obtiene el degradado de un área pasada como parámetro
        /// </summary>
        /// <param name="pRectangulo">Área que se degrada</param>
        /// <returns>Área degradada</returns>
        public static LinearGradientBrush DegradadoCeldaSeleccionada(Rectangle pRectangulo)
        {
            if (mDegradadoCeldaSeleccionada == null)
                mDegradadoCeldaSeleccionada = new LinearGradientBrush(pRectangulo, UtilGraficos.ColorSeleccionado, Color.Transparent, LinearGradientMode.BackwardDiagonal);
            return mDegradadoCeldaSeleccionada;
        }

        /// <summary>
        /// Dibuja un texto teniendo en cuenta la orientación desde el punto de referencia
        /// </summary>
        /// <param name="pGraphics">Papel en el que se pinta</param>
        /// <param name="pTexto">Cadena a representar</param>
        /// <param name="pFuente">Fuente del texto</param>
        /// <param name="pBrocha">Brocha de pintado</param>
        /// <param name="pX">Coordenada X de referencia</param>
        /// <param name="pY">Coordenada Y de referencia</param>
        /// <param name="pOrientacion">Orientación</param>
        public static void DibujarTextoOrientado(Graphics pGraphics, string pTexto, Font pFuente, Brush pBrocha, float pX, float pY, OrientacionesTexto pOrientacion)
        {
            switch (pOrientacion)
            {
                case OrientacionesTexto.Norte:
                    pGraphics.DrawString(pTexto, pFuente, pBrocha, new PointF(pX - pGraphics.MeasureString(pTexto, pFuente).Width / 2, pY - pFuente.GetHeight(pGraphics)));
                    break;
                case OrientacionesTexto.Sur:
                    pGraphics.DrawString(pTexto, pFuente, pBrocha, new PointF(pX - pGraphics.MeasureString(pTexto, pFuente).Width / 2, pY));
                    break;
                case OrientacionesTexto.Este:
                    pGraphics.DrawString(pTexto, pFuente, pBrocha, new PointF(pX, pY - pFuente.GetHeight(pGraphics) / 2));
                    break;
                case OrientacionesTexto.Oeste:
                    pGraphics.DrawString(pTexto, pFuente, pBrocha, new PointF(pX - pGraphics.MeasureString(pTexto, pFuente).Width, pY - pFuente.GetHeight(pGraphics) / 2));
                    break;
                case OrientacionesTexto.NorEste:
                    pGraphics.DrawString(pTexto, pFuente, pBrocha, new PointF(pX, pY - pGraphics.MeasureString(pTexto, pFuente).Height));
                    break;
                case OrientacionesTexto.NorOeste:
                    pGraphics.DrawString(pTexto, pFuente, pBrocha, new PointF(pX - pGraphics.MeasureString(pTexto, pFuente).Width, pY - pFuente.GetHeight(pGraphics)));
                    break;
                case OrientacionesTexto.SudEste:
                    pGraphics.DrawString(pTexto, pFuente, pBrocha, new PointF(pX, pY));
                    break;
                case OrientacionesTexto.SudOeste:
                    pGraphics.DrawString(pTexto, pFuente, pBrocha, new PointF(pX - pGraphics.MeasureString(pTexto, pFuente).Width, pY));
                    break;
            }
        }

        /// <summary>
        /// Convierte una imagen a array de bytes
        /// </summary>
        /// <returns>Imagen en array de bytes</returns>
        public static byte[] ImagenBD(Image pImagen)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            pImagen.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] buffer = ms.GetBuffer();
            ms.Close();
            return buffer;
        }

        /// <summary>
        /// Dibuja un texto en vertical
        /// </summary>
        /// <param name="pGraphics">Papel en el que se dibuja</param>
        /// <param name="pTexto">Texto a dibujar</param>
        /// <param name="pFuente">Fuente del texto a dibujar</param>
        /// <param name="pColorTexto">Color</param>
        /// <param name="pRect">Área donde se tiene que dibujar</param>
        /// <param name="pFormatoTexto">Formato de alineación del texto</param>
        public static void DibujarTextoVertical(Graphics pGraphics, string pTexto, Font pFuente, Color pColorTexto, Rectangle pRect, StringFormat pFormatoTexto)
        {
            Rectangle rectGirado = new Rectangle(-pRect.Y - pRect.Height, pRect.X, pRect.Height, pRect.Width);

            GraphicsState anterior = pGraphics.Save();
            Matrix mx = new Matrix(1, 0, 0, 1, 0, 0);

            mx.Rotate(-90, MatrixOrder.Append);

            pGraphics.Transform = mx;

            SolidBrush brush = new SolidBrush(pColorTexto);
            pGraphics.DrawString(pTexto, pFuente, brush, rectGirado, pFormatoTexto);
            brush.Dispose();

            pGraphics.Restore(anterior);
        }

        /// <summary>
        /// Representa un porcentaje en un rectángulo
        /// </summary>
        /// <param name="pPorcentaje">Porcentaje a representar</param>
        /// <param name="pGrafico">Gráfico donde pintar</param>
        /// <param name="pRectangulo">Rectángulo donde pintar</param>
        /// <param name="pFuente">Fuente con la que pintar</param>
        /// <param name="pColor">Color con el que pintar</param>
        public static void PintarPorcentaje(short pPorcentaje, Graphics pGrafico, RectangleF pRectangulo, Font pFuente, Color pColor)
        {
            RectangleF recPorcentaje = new RectangleF(pRectangulo.Location, pRectangulo.Size);
            recPorcentaje.Width = pRectangulo.Width * ((float)pPorcentaje / 100f);
            pGrafico.FillRectangle(new SolidBrush(pColor), recPorcentaje);
            pGrafico.DrawString(pPorcentaje + "%", pFuente, new SolidBrush(ColorTexto(pColor)), pRectangulo, UtilGraficos.CadenaCentrada);
        }

        /// <summary>
        /// Dibuja un rectángulo redondeado
        /// </summary>
        /// <param name="pTapiz">Tapiz dónde se dibuja</param>
        /// <param name="pLapiz">Lapiz a usar</param>
        /// <param name="pRectangulo">Rectángulo recto</param>
        /// <param name="pPorcentaje">Porcentaje de redondeo</param>
        public static void DibujaRectanguloRedondeado(Graphics pTapiz, Pen pLapiz, Rectangle pRectangulo, float pPorcentaje)
        {
            int radio = Convert.ToInt32(pRectangulo.Width * pPorcentaje);
            if (radio < 3)
            {
                radio = 2;
            }
            else if (radio > 10)
            {
                radio = 10;
            }

            GraphicsPath path = new GraphicsPath();
            path.AddArc(new Rectangle(pRectangulo.Left, pRectangulo.Top, radio, radio), 180, 90);

            path.AddLine(pRectangulo.Left + radio, pRectangulo.Top, pRectangulo.Right - radio, pRectangulo.Top);
            path.AddArc(new Rectangle(pRectangulo.Right - radio, pRectangulo.Top, radio, radio), 270, 90);
            path.AddLine(pRectangulo.Right, pRectangulo.Top + radio, pRectangulo.Right, pRectangulo.Bottom - radio);
            path.AddArc(new Rectangle(pRectangulo.Right - radio, pRectangulo.Bottom - radio, radio, radio), 0, 90);
            path.AddLine(pRectangulo.Right - radio, pRectangulo.Bottom, pRectangulo.Left + radio, pRectangulo.Bottom);
            path.AddArc(new Rectangle(pRectangulo.Left, pRectangulo.Bottom - radio, radio, radio), 90, 90);
            //path.AddLine(pRectangulo.Left, pRectangulo.Bottom - radio, pRectangulo.Left, pRectangulo.Top + radio);
            path.CloseFigure();

            pTapiz.DrawPath(pLapiz, path);
            path.Dispose();

        }

        /// <summary>
        /// Rellena un rectángulo redondeado
        /// </summary>
        /// <param name="pTapiz">Tapiz dónde se dibuja</param>
        /// <param name="pBrocha">Brocha a usar</param>
        /// <param name="pRectangulo">Rectángulo recto</param>
        /// <param name="pPorcentaje">Porcentaje de redondeo</param>
        public static void RellenaRectanguloRedondeado(Graphics pTapiz, Brush pBrocha, Rectangle pRectangulo, float pPorcentaje)
        {
            int radio = Convert.ToInt32(pRectangulo.Width * pPorcentaje);
            if (radio < 3)
            {
                radio = 2;
            }
            else if (radio > 10)
            {
                radio = 10;
            }
            GraphicsPath path = new GraphicsPath();
            path.AddArc(new Rectangle(pRectangulo.Left, pRectangulo.Top, radio, radio), 180, 90);

            path.AddLine(pRectangulo.Left + radio, pRectangulo.Top, pRectangulo.Right - radio, pRectangulo.Top);
            path.AddArc(new Rectangle(pRectangulo.Right - radio, pRectangulo.Top, radio, radio), 270, 90);
            path.AddLine(pRectangulo.Right, pRectangulo.Top + radio, pRectangulo.Right, pRectangulo.Bottom - radio);
            path.AddArc(new Rectangle(pRectangulo.Right - radio, pRectangulo.Bottom - radio, radio, radio), 0, 90);
            path.AddLine(pRectangulo.Right - radio, pRectangulo.Bottom, pRectangulo.Left + radio, pRectangulo.Bottom);
            path.AddArc(new Rectangle(pRectangulo.Left, pRectangulo.Bottom - radio, radio, radio), 90, 90);
            //path.AddLine(pRectangulo.Left, pRectangulo.Bottom - radio, pRectangulo.Left, pRectangulo.Top + radio);
            path.CloseFigure();

            pTapiz.FillPath(pBrocha, path);
        }

        /// <summary>
        /// Comprueba si el punto pasado por parámetro está incluido en el array de puntos con un margen de aproximación
        /// </summary>
        /// <param name="pPuntos">Array de puntos que forman la línea dónde se comprueba si está contenido el punto</param>
        /// <param name="pPunto">Punto a comprobar</param>
        /// <returns>TRUE si el punto está contenido en el array, False en caso contrario</returns>
        public static bool ContienePunto(Point[] pPuntos, Point pPunto)
        {
            int puntoMinX, puntoMaxX, puntoMinY, puntoMaxY;

            for (int i = 0; i < pPuntos.Length - 1; i++)
            {
                puntoMinX = Math.Min(pPuntos[i].X, pPuntos[i + 1].X);
                puntoMaxX = Math.Max(pPuntos[i].X, pPuntos[i + 1].X);
                puntoMinY = Math.Min(pPuntos[i].Y, pPuntos[i + 1].Y);
                puntoMaxY = Math.Max(pPuntos[i].Y, pPuntos[i + 1].Y);

                if (new Rectangle(puntoMinX - 5, puntoMinY - 5, puntoMaxX - puntoMinX + 10, puntoMaxY - puntoMinY + 10).Contains(pPunto))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Devuelve cuál es el punto medio entre dos puntos
        /// </summary>
        /// <param name="pPunto1">punto1</param>
        /// <param name="pPunto2">punto2</param>
        /// <returns></returns>
        public static Point PuntoMedio(Point pPunto1, Point pPunto2)
        {
            return new Point((pPunto1.X + pPunto2.X) / 2, (pPunto2.Y + pPunto1.Y) / 2);
        }

        /// <summary>
        /// Aumenta el ancho y el alto mandados por parámetros al array de puntos originales y devuelve otro array con los nuevos puntos
        /// </summary>
        /// <param name="pPuntosOrigen">Array de puntos origen</param>
        /// <param name="pAncho">Ancho que se va a aumentar a los puntos</param>
        /// <param name="pAlto">Alto que se va a aumentar a los puntos</param>
        /// <returns>Nuevo array de puntos con los tamaños aumentados</returns>
        public static Point[] ActualizarPuntos(Point[] pPuntosOrigen, int pAncho, int pAlto)
        {
            Point[] puntosDestino = new Point[0];
            if (pPuntosOrigen != null)
            {
                puntosDestino = new Point[pPuntosOrigen.Length];
                int i = 0;
                foreach (Point punto in pPuntosOrigen)
                {
                    puntosDestino[i] = Point.Add(punto, new Size(pAncho, pAlto));
                    i++;
                }
            }
            return puntosDestino;
        }

        /// <summary>
        /// Rectángulo entre dos puntos, calculando cuál es el punto menor en X e Y
        /// </summary>
        /// <param name="punto1">punto 1</param>
        /// <param name="punto2">punto 2</param>
        /// <returns></returns>
        public static Rectangle RectanguloEntre2Puntos(Point punto1, Point punto2)
        {
            int valorXInf = Math.Min(punto1.X, punto2.X);
            int valorXSup = Math.Max(punto1.X, punto2.X);
            int valorYInf = Math.Min(punto1.Y, punto2.Y);
            int valorYSup = Math.Max(punto1.Y, punto2.Y);

            return (new Rectangle(valorXInf, valorYInf, Math.Abs(valorXSup - valorXInf), Math.Abs(valorYSup - valorYInf)));
        }

        /// <summary>
        /// Obtiene el rectángulo que ocupa una cadena
        /// </summary>
        /// <param name="pTapiz">Tapiz dónde se pinta</param>
        /// <param name="pCadena">Cadena</param>
        /// <param name="pFuente">fuente de la cadena</param>
        /// <param name="pCaracteresPorLinea">Caracteres que se quieren por línea</param>
        /// <returns>Ancho y alto del rectángulo calculado</returns>
        public static Size RectanguloOcupaCadena(Graphics pTapiz, string pCadena, Font pFuente, int pCaracteresPorLinea)
        {

            Size tamañoTexto = new Size(Convert.ToInt32(pTapiz.MeasureString(pCadena, pFuente).Width) + 5, Convert.ToInt32(pTapiz.MeasureString(pCadena, pFuente).Height));

            return tamañoTexto;
        }

        #region Colores

        /// <summary>
        /// Devuelve el color de texto de máximo contraste con el color de fondo
        /// </summary>
        /// <param name="pColorFondo">Color de fondo</param>
        /// <returns>Color de máximo contraste</returns>
        public static Color ColorTexto(Color pColorFondo)
        {
            if (pColorFondo.GetBrightness() >= 0.5)
                return Color.Black;
            else
                return Color.White;
        }

        /// <summary>
        /// Devuelve el color equivalente en escala de grises
        /// </summary>
        /// <param name="pColor">Color del que se solicita el equivalente en escala de grises</param>
        /// <returns>Color equivalente en escala de grises del color suministrado</returns>
        public static Color ColorBN(Color pColor)
        {
            int A = pColor.A;
            int R = pColor.R;
            int G = pColor.G;
            int B = pColor.B;
            int media = (R + G + B) / 3;
            return Color.FromArgb(A, media, media, media);
        }

        /// <summary>
        /// Devuelve una cadena de texto que representa en formato RGB el color pasado por parámetro
        /// </summary>
        /// <param name="pColor">Color</param>
        /// <returns>Cadena de texto que representa al color RGB</returns>
        public static string ColorRGB(Color pColor)
        {
            return "rgb(" + pColor.R + "," + pColor.G + "," + pColor.B + ")";
        }

        /// <summary>
        /// Devuelve el color equivalente oscurecido o aclarado según el factor indicado
        /// </summary>
        /// <param name="pOriginal">Color original</param>
        /// <param name="pFactor">Factor de brillo</param>
        /// <returns>Nuevo color</returns>
        public static Color ColorModificadoBrillo(Color pOriginal, float pFactor)
        {
            int A = pOriginal.A;
            int R = Math.Min((int)(pOriginal.R * pFactor), 255);
            int G = Math.Min((int)(pOriginal.G * pFactor), 255);
            int B = Math.Min((int)(pOriginal.B * pFactor), 255);
            return Color.FromArgb(A, R, G, B);
        }

        #endregion

        #region Imágenes

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
        /// Calcula el tamaño proporcional en el que quedará 
        /// </summary>
        /// <param name="pImagen">Imagen que se va a proporcionar</param>
        /// <param name="pSize">Tamaño al que ajustar</param>
        /// <returns>Tamaño ajustado</returns>
        public static SixLabors.ImageSharp.Size CalcularTamanioProporcionado(SixLabors.ImageSharp.Image pImagen, SixLabors.ImageSharp.Size pSize)
        {
            SixLabors.ImageSharp.SizeF tamanioF = CalcularTamanioProporcionado(pImagen, pSize.Width, pSize.Height);
            SixLabors.ImageSharp.Size tamanio = SixLabors.ImageSharp.Size.Truncate(tamanioF);
            return tamanio;
        }

        /// <summary>
        /// Calcula el tamaño proporcional en el que quedará 
        /// </summary>
        /// <param name="pImagen">Imagen que se va a proporcionar</param>
        /// <param name="pAlto">Alto a ajustar</param>
        /// <param name="pAncho">Ancho a ajustar</param>
        /// <returns>Tamaño ajustado</returns>
        public static SixLabors.ImageSharp.SizeF CalcularTamanioProporcionado(SixLabors.ImageSharp.Image pImagen, float pAncho, float pAlto)
        {
            if (pImagen != null)
            {
                float Alto = pImagen.Height;
                float Ancho = pImagen.Width;

                float AltoFinal = Alto;
                float AnchoFinal = Ancho;


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
                return new SixLabors.ImageSharp.SizeF(AnchoFinal, AltoFinal);
            }
            return new SixLabors.ImageSharp.SizeF(pAncho, pAlto);
        }

        /// <summary>
        /// Ajusta una imagen proporcionalmente
        /// </summary>
        /// <param name="pImagen">Imagen a ajustar</param>
        /// <param name="pAncho">Ancho de ajuste</param>
        /// <param name="pAlto">Alto de ajuste</param>
        /// <param name="pCalcularTamañoProporcionado">TRUE si se debe calcular el tamaño proporcionadamente</param>
        /// <returns>Devuelve la imagen ajustada</returns>
        public static SixLabors.ImageSharp.Image AjustarImagen(SixLabors.ImageSharp.Image pImagen, float pAncho, float pAlto, bool pCalcularTamañoProporcionado)
        {
            SixLabors.ImageSharp.Size tamanio = new SixLabors.ImageSharp.Size((int)pAncho, (int)pAlto);

            if (pCalcularTamañoProporcionado) 
            {
                SixLabors.ImageSharp.SizeF tamanioF = CalcularTamanioProporcionado(pImagen, pAncho, pAlto);
                tamanio = SixLabors.ImageSharp.Size.Truncate(tamanioF);
            }
            pImagen.Mutate(c => c.Resize(tamanio));
            return pImagen;
        }

        /// <summary>
        /// Ajusta una imagen proporcionalmente
        /// </summary>
        /// <param name="pImagen">Imagen a ajustar</param>
        /// <param name="pAncho">Ancho de ajuste</param>
        /// <param name="pAlto">Alto de ajuste</param>
        /// <returns>Devuelve la imagen ajustada</returns>
        public static SixLabors.ImageSharp.Image AjustarImagen(SixLabors.ImageSharp.Image pImagen, float pAncho, float pAlto)
        {
            return AjustarImagen(pImagen, pAncho, pAlto, true);
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
            Image imgPhoto = Image.FromStream(new MemoryStream(imageFile));
            Bitmap bmPhoto = new Bitmap(targetW, targetH/*, PixelFormat.Format24bppRgb*/);
            //bmPhoto.SetResolution(72, 72);
            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            //grPhoto.SmoothingMode = SmoothingMode.HighQuality;
            //grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;
            Rectangle recorte = new Rectangle(0, 0, targetW, targetH);
            grPhoto.DrawImage(imgPhoto, recorte, targetX, targetY, targetW, targetH, GraphicsUnit.Pixel);
            //grPhoto.DrawImage(imgPhoto, targetX, targetY, targetW, targetH);
            // Save out to memory and then to a file.  We dispose of all objects to make sure the files don't stay locked.
            MemoryStream mm = new MemoryStream();
            bmPhoto.Save(mm, System.Drawing.Imaging.ImageFormat.Png);
            imgPhoto.Dispose();
            bmPhoto.Dispose();
            grPhoto.Dispose();
            return mm.GetBuffer();
        }
        public static byte[] CropImageFile(byte[] imageFile, int targetW, int targetH, int targetX, int targetY, string extensionArchivo)
        {
            using (MemoryStream stream = new MemoryStream(imageFile))
            {
                Image imagenOriginal = Image.FromStream(new MemoryStream(imageFile));

                Bitmap imagenFinal = new Bitmap(targetW, targetH);
                Graphics grImagenFinal = Graphics.FromImage(imagenFinal);

                Rectangle rectanguloRecorte = new Rectangle(targetX, targetY, targetW, targetH);
                Rectangle rectanguloDestino = new Rectangle(0, 0, targetW, targetH);

                grImagenFinal.DrawImage(imagenOriginal, rectanguloDestino, rectanguloRecorte, GraphicsUnit.Pixel);

                MemoryStream mm = new MemoryStream();
                if (string.Compare(extensionArchivo, SixLabors.ImageSharp.Formats.Jpeg..Jpeg.ToString(), true) == 0 || string.Compare(extensionArchivo, "jpg", true) == 0)
                {
                    imagenFinal.Save(mm, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                if (string.Compare(extensionArchivo, ImageFormat.Png.ToString(), true) == 0)
                {
                    imagenFinal.Save(mm, System.Drawing.Imaging.ImageFormat.Png);
                }
                if (string.Compare(extensionArchivo, ImageFormat.Bmp.ToString(), true) == 0)
                {
                    imagenFinal.Save(mm, System.Drawing.Imaging.ImageFormat.Bmp);
                }
                if (string.Compare(extensionArchivo, ImageFormat.Gif.ToString(), true) == 0)
                {
                    imagenFinal.Save(mm, System.Drawing.Imaging.ImageFormat.Gif);
                }
                return mm.GetBuffer();
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
            Image imagenOriginal = Image.FromStream(new MemoryStream(imageFile));

            Bitmap imagenFinal = new Bitmap(targetW, targetH);
            Graphics grImagenFinal = Graphics.FromImage(imagenFinal);

            Rectangle rectanguloRecorte = new Rectangle(targetX, targetY, targetW - targetX, targetH - targetY);
            Rectangle rectanguloDestino = new Rectangle(0, 0, targetW, targetH);

            grImagenFinal.DrawImage(imagenOriginal, rectanguloDestino, rectanguloRecorte, GraphicsUnit.Pixel);

            return imagenFinal;
        }

        /// <summary>
        /// Ajusta una imagen proporcionalmente
        /// </summary>
        /// <param name="pImagen">Imagen a ajustar</param>
        /// <param name="pAncho">Ancho de ajuste</param>
        /// <param name="pAlto">Alto de ajuste</param>
        /// <returns>Imagen ajustada</returns>
        public static SixLabors.ImageSharp.Image AjustarImagen(SixLabors.ImageSharp.Image pImagen, int pAncho, int pAlto)
        {
            return AjustarImagen(pImagen, (float)pAncho, (float)pAlto);
        }

        /// <summary>
        /// Ajusta una imagen proporcionalmente
        /// </summary>
        /// <param name="pImagen">Imagen a ajustar</param>
        /// <param name="pTamano">Tamaño final de la imagen</param>
        /// <returns>Imagen ajustada</returns>
        public static SixLabors.ImageSharp.Image AjustarImagen(SixLabors.ImageSharp.Image pImagen, Size pTamano)
        {
            return AjustarImagen(pImagen, (float)pTamano.Width, (float)pTamano.Height);
        }

        /// <summary>
        /// Valida que el tamaño de la imagen sea menor que el tamaño máximo
        /// </summary>
        /// <param name="pRuta">Ruta donde se encuentra la imagen</param>
        /// <param name="pTamanoMaximoKB">Tamaño máximo en KB</param>
        /// <returns>TRUE si el tamaño es menor que el máximo, FALSE en caso contrario</returns>
        public static bool ValidarTamañoImagen(string pRuta, double pTamanoMaximoKB)
        {
            System.IO.FileInfo imgInfo = new FileInfo(pRuta);
            double tamanio = Convert.ToDouble(imgInfo.Length);
            double tamanioKB = Math.Round((tamanio / 1024), 2);

            return (tamanioKB < pTamanoMaximoKB);
        }

        /// <summary>
        /// Valida que el tamaño de la imagen sea menor que el tamaño máximo
        /// </summary>
        /// <param name="pLength">Tamaño de la imagen</param>
        /// <param name="pTamanoMaximoKB">Tamaño máximo en KB</param>
        /// <returns>TRUE si el tamaño es menor que el máximo, FALSE en caso contrario</returns>
        public static bool ValidarTamañoImagen(int pLength, double pTamanoMaximoKB)
        {
            double tamanio = pLength;
            double tamanioKB = Math.Round((tamanio / 1024), 2);

            return (tamanioKB < pTamanoMaximoKB);
        }

        /// <summary>
        /// Devuelve una imagen en escala de grises
        /// </summary>
        /// <param name="pImagen">Imagen a transformar</param>
        /// <returns>Imagen en escala de grises</returns>
        public static Image ImagenEscalaGrises(Image pImagen)
        {
            Bitmap bm = new Bitmap(pImagen.Width, pImagen.Height);
            for (int y = 0; y < bm.Height; y++)
            {
                for (int x = 0; x < bm.Width; x++)
                {
                    Color c = ((Bitmap)pImagen).GetPixel(x, y);
                    int luma = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                    bm.SetPixel(x, y, Color.FromArgb(luma, luma, luma));
                }
            }
            return bm;
        }

        /// <summary>
        /// Devuelve un icono en escala de grises
        /// </summary>
        /// <param name="pIcono">Icono a transformar</param>
        /// <param name="pFondo">Color de fondo</param>
        /// <returns>Imagen en escala de grises</returns>
        public static Image IconoEscalaGrises(Icon pIcono, Color pFondo)
        {
            return ImagenEscalaGrises(IconoColorFondo(pIcono, pFondo));
        }

        /// <summary>
        /// Devuelve un icono en escala de grises
        /// </summary>
        /// <param name="pIcono">Icono a transformar</param>
        /// <param name="pFondo">Color de fondo</param>
        /// <returns>Imagen en escala de grises</returns>
        public static Image IconoEscalaGrises(Image pIcono, Color pFondo)
        {
            return ImagenEscalaGrises(IconoColorFondo(pIcono, pFondo));
        }

        /// <summary>
        /// Pone color de fondo a un icono
        /// </summary>
        /// <param name="pIcono">Icono al cual se le va a poner fondo</param>
        /// <param name="pFondo">Color de fondo</param>
        /// <returns>Icono con fondo</returns>
        public static Image IconoColorFondo(Icon pIcono, Color pFondo)
        {
            Bitmap ico = new Bitmap(pIcono.Width, pIcono.Height);
            Graphics grafico = Graphics.FromImage(ico);
            if (pFondo != Color.Empty)
                grafico.FillRectangle(Brushes.White, 0, 0, ico.Width, ico.Height);

            grafico.DrawImage(pIcono.ToBitmap(), 0, 0);
            grafico.Dispose();
            return ico;
        }

        /// <summary>
        /// Pone color de fondo a un icono
        /// </summary>
        /// <param name="pIcono">Icono al cual se le va a poner fondo</param>
        /// <param name="pFondo">Color de fondo</param>
        /// <returns>Icono con fondo</returns>
        public static Image IconoColorFondo(Image pIcono, Color pFondo)
        {
            if (pFondo == Color.Empty)
                return pIcono;

            Bitmap ico = new Bitmap(pIcono.Width, pIcono.Height);
            Graphics grafico = Graphics.FromImage(ico);
            SolidBrush brocha = new SolidBrush(pFondo);
            grafico.FillRectangle(brocha, 0, 0, ico.Width, ico.Height);
            grafico.DrawImage(pIcono, 0, 0);
            brocha.Dispose();
            grafico.Dispose();

            return ico;
        }

        /// <summary>
        /// Convierte una imagen en un array de bytes (Byte[])
        /// </summary>
        /// <param name="pImagen">Imagen a convertir</param>
        /// <returns>Array de bytes de la imagen</returns>
        public static byte[] ConvertirImagenEnArrayBytes(System.Drawing.Image pImagen)
        {
            if (pImagen == null)
                return new byte[0];

            MemoryStream ms = new MemoryStream();
            try
            {
                pImagen.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception)
            {
            }
            byte[] resultado = ms.ToArray();
            ms.Close();
            ms.Dispose();
            return resultado;
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
            Image returnImage = Image.FromStream(ms);
            ms.Close();
            ms.Dispose();

            return returnImage;
        }

        /// <summary>
        /// Cambia colores de una imagen por otros
        /// </summary>
        /// <param name="pOriginal">Imagen original a cambiar</param>
        /// <param name="pColorTransparente">Color que se sustituirá por transparente</param>
        /// <param name="pColorSustituir">Color que se sustituirá</param>
        /// <param name="pColorCambio">Color de sustitución</param>
        /// <returns>Devuelve una imagen cambiada de colores</returns>
        public static Bitmap CambiarColorImagen(Image pOriginal, Color pColorTransparente, Color pColorSustituir, Color pColorCambio)
        {
            Bitmap bm = new Bitmap(pOriginal);
            Color transp = Color.Transparent;

            for (int y = 0; y < bm.Height; y++)
            {
                for (int x = 0; x < bm.Width; x++)
                {
                    Color c = bm.GetPixel(x, y);

                    if (c.ToArgb() == pColorTransparente.ToArgb())
                        bm.SetPixel(x, y, transp);

                    if (c.ToArgb() == pColorSustituir.ToArgb())
                        bm.SetPixel(x, y, pColorCambio);
                }
            }
            return bm;
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
            Bitmap bmpImagen = new Bitmap(pAnchura, pAltura);

            Graphics grImagen = Graphics.FromImage(bmpImagen);
            Rectangle rectOrigen = new Rectangle(0, 0, pAnchura, pAltura);
            Rectangle rectDestino = new Rectangle(pCoordenadaX, pCoordenadaY, pAnchura, pAltura);
            grImagen.DrawImage(pImagen, rectOrigen, rectDestino, GraphicsUnit.Pixel);

            byte[] bufferRecortado = (byte[])new ImageConverter().ConvertTo(bmpImagen, typeof(byte[]));

            return bufferRecortado;
        }

        public static Image RecortarImagenACuadrada(Image pImagen, float tamaño)
        {
            Image imgDevolver = null;
            float alto = pImagen.Height;
            float ancho = pImagen.Width;
            bool esVertical = false;
            bool esHorizontal = false;
            bool esCuadrada = false;
            float proporcionAnchoAlto = ancho / alto;
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
                    Size size = new SizeF(tamaño, tamaño).ToSize();
                    Rectangle rectangulo = new Rectangle(origen, size);
                    Bitmap imagenRecortada = ((Bitmap)imagen).Clone(rectangulo, imagen.PixelFormat);

                    imgDevolver = imagenRecortada;
                }
                else if (ancho < tamaño && alto > tamaño)
                {

                    Point origen = new Point(0, 0);
                    Size size = new SizeF(ancho, tamaño).ToSize();
                    Rectangle rectangulo = new Rectangle(origen, size);
                    Bitmap imagenRecortada = ((Bitmap)pImagen).Clone(rectangulo, pImagen.PixelFormat);
                    imgDevolver = imagenRecortada;
                }
                else
                {
                    imgDevolver = pImagen;
                }

            }
            else if (esHorizontal)
            {
                if (alto >= tamaño)
                {
                    Image imagen = RedimensionarDadoAlto(pImagen, tamaño);
                    Point origen = new Point(Convert.ToInt32(imagen.Width - tamaño) / 2, 0);
                    Size size = new SizeF(tamaño, tamaño).ToSize();
                    Rectangle rectangulo = new Rectangle(origen, size);
                    Bitmap imagenRecortada = ((Bitmap)imagen).Clone(rectangulo, imagen.PixelFormat);

                    imgDevolver = imagenRecortada;
                }
                else if (alto <= tamaño && ancho > tamaño)
                {
                    Point origen = new Point(Convert.ToInt32(pImagen.Width - tamaño) / 2, 0);
                    Size size = new SizeF(tamaño, alto).ToSize();
                    Rectangle rectangulo = new Rectangle(origen, size);
                    Bitmap imagenRecortada = ((Bitmap)pImagen).Clone(rectangulo, pImagen.PixelFormat);
                    imgDevolver = imagenRecortada;
                }
                else
                {
                    imgDevolver = pImagen;
                }
            }
            else if (esCuadrada)
            {
                Image imagenCuadrada = RedimensionarDadoAncho(pImagen, tamaño);

                imgDevolver = imagenCuadrada;
            }

            return imgDevolver;
        }

        public static Image RedimensionarDadoAncho(Image pImagen, float pAnchoPixeles)
        {
            float alto = pImagen.Height;
            float ancho = pImagen.Width;

            float proporcionAnchoAlto = ancho / alto;
            Image imagenRedimensionada = null;

            // Sólo redimensionamos la imagen si el ancho es menor que el pedido
            if (pAnchoPixeles < ancho)
            {
                float nuevoAlto = pAnchoPixeles / proporcionAnchoAlto;
                Size size = new SizeF(pAnchoPixeles, nuevoAlto).ToSize();
                imagenRedimensionada = new Bitmap(pImagen, size);
                //imagenRedimensionada.Save(@"C:\Cargas\PruebasImagenes\prueba.png", ImageFormat.Png);
            }
            else
            {
                imagenRedimensionada = pImagen;
            }
            return imagenRedimensionada;
        }

        public static Image RedimensionarDadoAlto(Image pImagen, float pAltoPixeles)
        {
            float alto = pImagen.Height;
            float ancho = pImagen.Width;
            float proporcionAnchoAlto = ancho / alto;
            Bitmap imagenRedimensionada = null;

            // Sólo redimensionamos la imagen si el alto es menor que el pedido
            if (pAltoPixeles < alto)
            {
                float nuevoAncho = pAltoPixeles * proporcionAnchoAlto;
                Size size = new SizeF(nuevoAncho, pAltoPixeles).ToSize();
                imagenRedimensionada = new Bitmap(pImagen, size);
                //imagenRedimensionada.Save(@"C:\Cargas\PruebasImagenes\prueba2.png", ImageFormat.Png);
            }
            return imagenRedimensionada;
        }

        public static byte[] BitmapToByteArray(Bitmap pBitmap)
        {
            MemoryStream ms = new MemoryStream();
            pBitmap.Save(ms, ImageFormat.Jpeg);
            byte[] buffer = ms.ToArray();
            //pBitmap.Dispose();
            ms.Dispose();
            return buffer;
        }

        #endregion

        #region Idef0

        /// <summary>
        /// Dibuja una rama de texto para idef0
        /// </summary>
        /// <param name="pTapiz">Tapiz dónde se dibuja</param>
        /// <param name="pLapiz">Lapiz a usar</param>
        /// <param name="pPuntoOrigen">Punto origen</param>
        /// <param name="pPuntoDestino">Punto destino</param>
        public static void DibujaRamaTexto(Graphics pTapiz, Pen pLapiz, Point pPuntoOrigen, Point pPuntoDestino)
        {
            Point[] puntos = new Point[4];
            puntos[0] = pPuntoOrigen;
            puntos[3] = pPuntoDestino;
            Point puntoMedio = new Point((pPuntoOrigen.X + pPuntoDestino.X) / 2, (pPuntoOrigen.Y + pPuntoDestino.Y) / 2);

            if (pPuntoOrigen.X <= pPuntoDestino.X && pPuntoOrigen.Y >= pPuntoDestino.Y) //1º cuadrante
            {
                puntos[1] = Point.Add(puntoMedio, new Size(-2, -2));
                puntos[2] = Point.Add(puntoMedio, new Size(2, 2));
            }
            else if (pPuntoOrigen.X >= pPuntoDestino.X && pPuntoOrigen.Y >= pPuntoDestino.Y) // 2º cuadrante
            {
                puntos[1] = Point.Add(puntoMedio, new Size(-2, -2));
                puntos[2] = Point.Add(puntoMedio, new Size(2, 2));
            }
            else if (pPuntoOrigen.X >= pPuntoDestino.X && pPuntoOrigen.Y <= pPuntoDestino.Y) // 3º cuadrante
            {
                puntos[1] = Point.Add(puntoMedio, new Size(-2, -2));
                puntos[2] = Point.Add(puntoMedio, new Size(2, -2));
            }
            else if (pPuntoOrigen.X <= pPuntoDestino.X && pPuntoOrigen.Y <= pPuntoDestino.Y) //4º cuadrante
            {
                puntos[1] = Point.Add(puntoMedio, new Size(-2, 2));
                puntos[2] = Point.Add(puntoMedio, new Size(2, -2));
            }
            pTapiz.DrawLines(pLapiz, puntos);
        }

        /// <summary>
        /// Orientación que sigue la flecha dados los dos puntos
        /// </summary>
        /// <param name="pPunto1">Punto origen</param>
        /// <param name="pPunto2">Punto destino</param>
        /// <returns></returns>
        public static OrientacionesLineaRedondeada OrientacionAlPunto(Point pPunto1, Point pPunto2)
        {
            if (pPunto1.X < pPunto2.X && pPunto1.Y == pPunto2.Y)
                return OrientacionesLineaRedondeada.Oeste;
            else if (pPunto1.X > pPunto2.X && pPunto1.Y == pPunto2.Y)
                return OrientacionesLineaRedondeada.Este;
            else if (pPunto1.X == pPunto2.X && pPunto1.Y > pPunto2.Y)
                return OrientacionesLineaRedondeada.Norte;
            else if (pPunto1.X == pPunto2.X && pPunto1.Y < pPunto2.Y)
                return OrientacionesLineaRedondeada.Sur;
            else
                return OrientacionesLineaRedondeada.NingunaCorrecta;
        }

        /// <summary>
        /// Dibuja una flecha
        /// </summary>
        /// <param name="pTapiz">tapiz</param>
        /// <param name="pLapiz">lapiz</param>
        /// <param name="pPuntos">puntos de la flecha</param>
        /// <param name="pEnPantalla">Verdad si se esta pintando en pantalla</param>
        public static void DibujaLineaRedondeada(Graphics pTapiz, Pen pLapiz, Point[] pPuntos, bool pEnPantalla)
        {
            AdjustableArrowCap flecha = new AdjustableArrowCap(Convert.ToInt32(Math.Min(2.5f, 3 * UtilGraficos.EscalaX(pTapiz.Transform))), Convert.ToInt32(Math.Min(3.5f, 5 * UtilGraficos.EscalaY(pTapiz.Transform))));

            //Cambio la punta de la flecha si no se está pintando en pantalla
            if (!pEnPantalla)
                flecha = new AdjustableArrowCap(4, 7);

            int radio = 4;

            Rectangle recAngulo = new Rectangle(new Point(), new Size(radio * 2, radio * 2)); //Rectangulo para dibujar el arco

            GraphicsPath path = new GraphicsPath();
            Point puntoAnterior = pPuntos[0];
            Point punto1;
            Point punto2;

            for (int i = 1; i < pPuntos.Length - 1; i++)
            {
                //Ajustamos los puntos donde la diferencia en las X sea menor que el radio
                if (Math.Abs(pPuntos[i].X - pPuntos[i + 1].X) < radio + 2)
                {
                    pPuntos[i + 1].X = pPuntos[i].X;

                    int j = i;
                    bool sigue = true;
                    while (j < pPuntos.Length - 1 && sigue)
                    {
                        if (Math.Abs(pPuntos[j].X - pPuntos[j + 1].X) < radio + 2)
                            pPuntos[j + 1].X = pPuntos[i].X;
                        else
                            sigue = false;
                        j++;
                    }
                }

                //Ajustamos los puntos donde la diferencia en las Y sea menor que el radio
                if (Math.Abs(pPuntos[i].Y - pPuntos[i + 1].Y) < radio + 2)
                {
                    pPuntos[i + 1].Y = pPuntos[i].Y;
                    int j = i;
                    bool sigue = true;
                    while (j < pPuntos.Length - 1 && sigue)
                    {
                        if (Math.Abs(pPuntos[j].Y - pPuntos[j + 1].Y) < radio + 2)
                            pPuntos[j + 1].Y = pPuntos[i].Y;
                        else
                            sigue = false;
                        j++;
                    }
                }

                punto1 = pPuntos[i];
                punto2 = pPuntos[i + 1];

                //Comparamos hacia donde va el siguiente punto

                //Si seguía trayectoria sur y va hacia el sur
                if (OrientacionAlPunto(puntoAnterior, punto1) == OrientacionesLineaRedondeada.Sur && OrientacionAlPunto(punto1, punto2) == OrientacionesLineaRedondeada.Sur)
                {
                    pTapiz.DrawLine(pLapiz, puntoAnterior, punto1);
                    puntoAnterior = punto1;
                }
                //Si seguía trayectoria norte y va hacia el norte
                else if (OrientacionAlPunto(puntoAnterior, punto1) == OrientacionesLineaRedondeada.Norte && OrientacionAlPunto(punto1, punto2) == OrientacionesLineaRedondeada.Norte)
                {
                    pTapiz.DrawLine(pLapiz, puntoAnterior, punto1);
                    puntoAnterior = punto1;
                }
                //Si seguía trayectoria este y va hacia el este
                else if (OrientacionAlPunto(puntoAnterior, punto1) == OrientacionesLineaRedondeada.Este && OrientacionAlPunto(punto1, punto2) == OrientacionesLineaRedondeada.Este)
                {
                    pTapiz.DrawLine(pLapiz, puntoAnterior, punto1);
                    puntoAnterior = punto1;
                }
                // Oeste - Oeste
                else if (OrientacionAlPunto(puntoAnterior, punto1) == OrientacionesLineaRedondeada.Oeste && OrientacionAlPunto(punto1, punto2) == OrientacionesLineaRedondeada.Oeste)
                {
                    pTapiz.DrawLine(pLapiz, puntoAnterior, punto1);
                    puntoAnterior = punto1;
                }
                //Si seguía trayectoria sur y va hacia el este
                else if (OrientacionAlPunto(puntoAnterior, punto1) == OrientacionesLineaRedondeada.Sur && OrientacionAlPunto(punto1, punto2) == OrientacionesLineaRedondeada.Este)
                {
                    punto1.Y -= radio;
                    recAngulo.Location = new Point(punto1.X - 2 * radio, punto1.Y - radio);
                    pTapiz.DrawLine(pLapiz, puntoAnterior, punto1);
                    pTapiz.DrawArc(pLapiz, recAngulo, 0, 90);

                    puntoAnterior = new Point(pPuntos[i].X - radio, pPuntos[i].Y);
                }
                //Si seguía trayectoria sur y va hacia el oeste
                else if (OrientacionAlPunto(puntoAnterior, punto1) == OrientacionesLineaRedondeada.Sur && OrientacionAlPunto(punto1, punto2) == OrientacionesLineaRedondeada.Oeste)
                {
                    punto1.Y -= radio;
                    recAngulo.Location = new Point(punto1.X, punto1.Y - radio);
                    pTapiz.DrawLine(pLapiz, puntoAnterior, punto1);
                    pTapiz.DrawArc(pLapiz, recAngulo, 90, 90);

                    puntoAnterior = new Point(pPuntos[i].X + radio, pPuntos[i].Y);
                }
                //Si seguía trayectoria norte y va hacia el este
                else if (OrientacionAlPunto(puntoAnterior, punto1) == OrientacionesLineaRedondeada.Norte && OrientacionAlPunto(punto1, punto2) == OrientacionesLineaRedondeada.Este)
                {
                    punto1.Y += radio;
                    recAngulo.Location = new Point(punto1.X - 2 * radio, punto1.Y - radio);
                    pTapiz.DrawLine(pLapiz, puntoAnterior, punto1);
                    pTapiz.DrawArc(pLapiz, recAngulo, 270, 90);

                    puntoAnterior = new Point(pPuntos[i].X - radio, pPuntos[i].Y);
                }
                //Si seguía trayectoria norte y va hacia el oeste
                else if (OrientacionAlPunto(puntoAnterior, punto1) == OrientacionesLineaRedondeada.Norte && OrientacionAlPunto(punto1, punto2) == OrientacionesLineaRedondeada.Oeste)
                {
                    punto1.Y += radio;
                    recAngulo.Location = new Point(punto1.X, punto1.Y - radio);
                    pTapiz.DrawLine(pLapiz, puntoAnterior, punto1);
                    pTapiz.DrawArc(pLapiz, recAngulo, 180, 90);

                    puntoAnterior = new Point(pPuntos[i].X + radio, pPuntos[i].Y);
                }
                //Si seguía trayectoria este y va hacia el sur
                else if (OrientacionAlPunto(puntoAnterior, punto1) == OrientacionesLineaRedondeada.Este && OrientacionAlPunto(punto1, punto2) == OrientacionesLineaRedondeada.Sur)
                {
                    punto1.X += radio;

                    recAngulo.Location = new Point(punto1.X - radio, punto1.Y);
                    pTapiz.DrawLine(pLapiz, puntoAnterior, punto1);
                    pTapiz.DrawArc(pLapiz, recAngulo, 180, 90);
                    puntoAnterior = new Point(pPuntos[i].X, pPuntos[i].Y + radio);
                }
                //Si seguía trayectoria este y va hacia el norte
                else if (OrientacionAlPunto(puntoAnterior, punto1) == OrientacionesLineaRedondeada.Este && OrientacionAlPunto(punto1, punto2) == OrientacionesLineaRedondeada.Norte)
                {
                    punto1.X += radio;

                    recAngulo.Location = new Point(punto1.X - radio, punto1.Y - 2 * radio);
                    pTapiz.DrawLine(pLapiz, puntoAnterior, punto1);
                    pTapiz.DrawArc(pLapiz, recAngulo, 90, 90);
                    puntoAnterior = new Point(pPuntos[i].X, pPuntos[i].Y - radio);
                }
                //Si seguía trayectoria oeste y va hacia el norte
                else if (OrientacionAlPunto(puntoAnterior, punto1) == OrientacionesLineaRedondeada.Oeste && OrientacionAlPunto(punto1, punto2) == OrientacionesLineaRedondeada.Norte)
                {
                    punto1.X -= radio;

                    recAngulo.Location = new Point(punto1.X - radio, punto1.Y - 2 * radio);
                    pTapiz.DrawLine(pLapiz, puntoAnterior, punto1);
                    pTapiz.DrawArc(pLapiz, recAngulo, 0, 90);
                    puntoAnterior = new Point(pPuntos[i].X, pPuntos[i].Y - radio);
                }
                //Si seguía trayectoria oeste y va hacia el sur
                else if (OrientacionAlPunto(puntoAnterior, punto1) == OrientacionesLineaRedondeada.Oeste && OrientacionAlPunto(punto1, punto2) == OrientacionesLineaRedondeada.Sur)
                {
                    punto1.X -= radio;

                    recAngulo.Location = new Point(punto1.X - radio, punto1.Y);
                    pTapiz.DrawLine(pLapiz, puntoAnterior, punto1);
                    pTapiz.DrawArc(pLapiz, recAngulo, 270, 90);
                    puntoAnterior = new Point(pPuntos[i].X, pPuntos[i].Y + radio);
                }
            }

            //pintamos el último tramo
            pLapiz.CustomEndCap = flecha;

            pTapiz.DrawLine(pLapiz, puntoAnterior, pPuntos[pPuntos.Length - 1]);

            path.Dispose();
            flecha.Dispose();
        }

        /// <summary>
        /// Dibuja los parénteis de tunelada de idef0
        /// </summary>
        /// <param name="pTapiz">Tapiz donde se pinta</param>
        /// <param name="pLapiz">Punta </param>
        /// <param name="pPunto1">punto1</param>
        /// <param name="pPunto2">punto2</param>
        public static void DibujaTunelada(Graphics pTapiz, Pen pLapiz, Point pPunto1, Point pPunto2)
        {
            if (pPunto1.X > pPunto2.X) //de izquierda a derecha
                DibujaTunelada(pTapiz, pLapiz, OrientacionesTexto.Este, pPunto1);
            else if (pPunto1.X < pPunto2.X) //de derecha a izquierda
                DibujaTunelada(pTapiz, pLapiz, OrientacionesTexto.Oeste, pPunto1);
            else if (pPunto1.Y < pPunto2.Y) //de arriba a abajo
                DibujaTunelada(pTapiz, pLapiz, OrientacionesTexto.Sur, pPunto1);
            else if (pPunto1.Y > pPunto2.Y) //de abajo a arriba
                DibujaTunelada(pTapiz, pLapiz, OrientacionesTexto.Norte, pPunto1);
        }

        /// <summary>
        /// Dibuja los parénteis de tunelada de idef0
        /// </summary>
        /// <param name="pTapiz">Tapiz donde se pinta</param>
        /// <param name="pLapiz">Punta</param>
        /// <param name="pOrientacion">Orientación de la tunelada</param>
        /// <param name="pPunto">punto</param>
        public static void DibujaTunelada(Graphics pTapiz, Pen pLapiz, OrientacionesTexto pOrientacion, Point pPunto)
        {
            const int ANCHO_NORMAL = 3;

            int ancho = ANCHO_NORMAL * Convert.ToInt32(EscalaX(pTapiz.Transform));
            if (ancho < 1)
                ancho = ANCHO_NORMAL;
            else if (ancho > ANCHO_NORMAL)
                ancho = ANCHO_NORMAL;

            int offset = 0;

            Point[] puntosCurva = new Point[4];

            if (pOrientacion == OrientacionesTexto.Este)
            {
                puntosCurva[0] = new Point(pPunto.X + offset, pPunto.Y - ancho);
                puntosCurva[1] = new Point(puntosCurva[0].X + ancho, puntosCurva[0].Y - ancho);
                puntosCurva[2] = new Point(puntosCurva[1].X + ancho, puntosCurva[1].Y);
                puntosCurva[3] = new Point(puntosCurva[2].X + ancho, puntosCurva[0].Y);
                pTapiz.DrawBezier(pLapiz, puntosCurva[0], puntosCurva[1], puntosCurva[2], puntosCurva[3]);
                puntosCurva = new Point[4];
                puntosCurva[0] = new Point(pPunto.X + offset, pPunto.Y + ancho);
                puntosCurva[1] = new Point(puntosCurva[0].X + ancho, puntosCurva[0].Y + ancho);
                puntosCurva[2] = new Point(puntosCurva[1].X + ancho, puntosCurva[1].Y);
                puntosCurva[3] = new Point(puntosCurva[2].X + ancho, puntosCurva[0].Y);
                pTapiz.DrawBezier(pLapiz, puntosCurva[0], puntosCurva[1], puntosCurva[2], puntosCurva[3]);
            }
            else if (pOrientacion == OrientacionesTexto.Oeste)
            {
                puntosCurva[0] = new Point(pPunto.X - offset, pPunto.Y - ancho);
                puntosCurva[1] = new Point(puntosCurva[0].X - ancho, puntosCurva[0].Y - ancho);
                puntosCurva[2] = new Point(puntosCurva[1].X - ancho, puntosCurva[1].Y);
                puntosCurva[3] = new Point(puntosCurva[2].X - ancho, puntosCurva[0].Y);
                pTapiz.DrawBezier(pLapiz, puntosCurva[0], puntosCurva[1], puntosCurva[2], puntosCurva[3]);
                puntosCurva = new Point[4];
                puntosCurva[0] = new Point(pPunto.X - offset, pPunto.Y + ancho);
                puntosCurva[1] = new Point(puntosCurva[0].X - ancho, puntosCurva[0].Y + ancho);
                puntosCurva[2] = new Point(puntosCurva[1].X - ancho, puntosCurva[1].Y);
                puntosCurva[3] = new Point(puntosCurva[2].X - ancho, puntosCurva[0].Y);
                pTapiz.DrawBezier(pLapiz, puntosCurva[0], puntosCurva[1], puntosCurva[2], puntosCurva[3]);
            }
            else if (pOrientacion == OrientacionesTexto.Sur)
            {
                puntosCurva[0] = new Point(pPunto.X - ancho, pPunto.Y + offset);
                puntosCurva[1] = new Point(puntosCurva[0].X - ancho, puntosCurva[0].Y + ancho);
                puntosCurva[2] = new Point(puntosCurva[1].X, puntosCurva[1].Y + ancho);
                puntosCurva[3] = new Point(puntosCurva[0].X, puntosCurva[2].Y + ancho);
                pTapiz.DrawBezier(pLapiz, puntosCurva[0], puntosCurva[1], puntosCurva[2], puntosCurva[3]);
                puntosCurva = new Point[4];
                puntosCurva[0] = new Point(pPunto.X + ancho, pPunto.Y + offset);
                puntosCurva[1] = new Point(puntosCurva[0].X + ancho, puntosCurva[0].Y + ancho);
                puntosCurva[2] = new Point(puntosCurva[1].X, puntosCurva[1].Y + ancho);
                puntosCurva[3] = new Point(puntosCurva[0].X, puntosCurva[2].Y + ancho);
                pTapiz.DrawBezier(pLapiz, puntosCurva[0], puntosCurva[1], puntosCurva[2], puntosCurva[3]);
            }
            else
            {
                puntosCurva[0] = new Point(pPunto.X - ancho, pPunto.Y - offset);
                puntosCurva[1] = new Point(puntosCurva[0].X - ancho, puntosCurva[0].Y - ancho);
                puntosCurva[2] = new Point(puntosCurva[1].X, puntosCurva[1].Y - ancho);
                puntosCurva[3] = new Point(puntosCurva[0].X, puntosCurva[2].Y - ancho);
                pTapiz.DrawBezier(pLapiz, puntosCurva[0], puntosCurva[1], puntosCurva[2], puntosCurva[3]);
                puntosCurva = new Point[4];
                puntosCurva[0] = new Point(pPunto.X + ancho, pPunto.Y - offset);
                puntosCurva[1] = new Point(puntosCurva[0].X + ancho, puntosCurva[0].Y - ancho);
                puntosCurva[2] = new Point(puntosCurva[1].X, puntosCurva[1].Y - ancho);
                puntosCurva[3] = new Point(puntosCurva[0].X, puntosCurva[2].Y - ancho);
                pTapiz.DrawBezier(pLapiz, puntosCurva[0], puntosCurva[1], puntosCurva[2], puntosCurva[3]);
            }
        }

        #endregion

        #region Matrices

        /// <summary>
        /// Devuelve el desplazamiento horizontal del graphics respecto al eje (0,0)
        /// </summary>
        /// <param name="pMatriz">Matriz de transformación</param>
        /// <returns>Desplazamiento horizontal</returns>
        public static int OffSetX(Matrix pMatriz)
        {
            return Convert.ToInt32(pMatriz.OffsetX);
        }

        /// <summary>
        /// Devuelve el desplazamiento vertical del graphics respecto al eje (0,0)
        /// </summary>
        /// <param name="pMatriz">Matriz de transformación</param>
        /// <returns>Desplazamiento vertical</returns>
        public static int OffSetY(Matrix pMatriz)
        {
            return Convert.ToInt32(pMatriz.OffsetY);
        }

        /// <summary>
        /// Devuelve la escala horizontal del graphics
        /// </summary>
        /// <param name="pMatriz">Matriz de transformación</param>
        /// <returns>Escala horizontal</returns>
        public static float EscalaX(Matrix pMatriz)
        {
            return (pMatriz.Elements[0]);
        }

        /// <summary>
        /// Devuelve la escala vertical del graphics
        /// </summary>
        /// <param name="pMatriz">Matriz de transformación</param>
        /// <returns>Escala vertical</returns>
        public static float EscalaY(Matrix pMatriz)
        {
            return pMatriz.Elements[3];
        }

        #endregion

        #endregion
    }
}
