using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace Es.Riam.Util
{
    /// <summary>
    /// Útiles para codificar / decodificar textos con formato RTF
    /// </summary>
    public class UtilRtf
    {
        #region Métodos estáticos

        #region HTML to RTF

        /// <summary>
        /// Convierte un texto HTML en RTF.
        /// </summary>
        /// <param name="pHtml">Texto en HTML</param>
        /// <returns>Texto en RTF </returns>
        public static string HtmlToRtf(string pHtml)
        {
            string rtf = pHtml.Replace("<p>", "").Replace("</p>", "").Replace("<br/>", "");
            return rtf;
        }

        #endregion

        /// <summary>
        /// Convierte un texto RTF en HTML
        /// </summary>
        /// <param name="pRtf">Texto en RTF</param>
        /// <returns>Texto en HTML </returns>
        public static string RtfToHtml(string pRtf)
        {
            // Saltos de línea
            pRtf = ReemplazarSaltosLineaRTF(pRtf);

            #region Cabeceras y pie de rtf

            pRtf = ReemplazarCabeceraYPieRTF(pRtf);

            #endregion

            // Carácteres especificos de HTML
            pRtf = UtilCadenas.HtmlEncode(pRtf);

            #region Caracteres especiales y acentos

            pRtf = ReemplazarCaracteresEspecialesRTF(pRtf);

            #endregion


            #region Formato texto

            pRtf = ReemplazarEtiquetasFormatoRTF(pRtf, true);

            #endregion

            #region Imágenes

            pRtf = ReemplazarEtiquetasImagenesRTF(pRtf);

            #endregion

            #region Otras etiquetas y etiquetas rtf no convertibles a HTML

            pRtf = QuitarEtiquetasRTFSobrantes(pRtf, true);

            #endregion

            return pRtf;
        }

        /// <summary>
        /// Quita las etiquetas de salto de línea en formato RTF
        /// </summary>
        /// <param name="pRtf">Texto en formato RTF</param>
        /// <returns>Texto sin saltos de línea en RTF</returns>
        public static string ReemplazarSaltosLineaRTF(string pRtf)
        {
            return Regex.Replace(pRtf, "[\n\r\f]", "");
        }

        /// <summary>
        /// Reemplaza la cabecera y el pie de un texto con formato RTF
        /// </summary>
        /// <param name="pRtf">Texto en formato RTF</param>
        /// <returns>Texto sin cabecera y pie en RTF</returns>
        public static string ReemplazarCabeceraYPieRTF(string pRtf)
        {
            pRtf = Regex.Replace(pRtf, @"{\\rtf[^}]+}", "");
            pRtf = Regex.Replace(pRtf, @"{\\rtf[^}]+}}", "");
            pRtf = Regex.Replace(pRtf, @"{\\rtf1[^}]+}[^}]+}*", "");
            pRtf = Regex.Replace(pRtf, @"\\deff0{\\fonttbl{\\f\d+[^}]+}}", "");
            pRtf = Regex.Replace(pRtf, @"\\deff0{\\fonttbl{\\nilcharset[\d]*[^}]+}}", "");
            pRtf = Regex.Replace(pRtf, @"\\roman\\fprq[\d]?\\fcharset[\d]+[^}]+}", "");
            pRtf = Regex.Replace(pRtf, @"\\fnil\\fcharset[\d]*[^}]+}", "");
            pRtf = Regex.Replace(pRtf, @"\\fswiss\\fprq[\d]?\\fcharset[\d]+[^}]+}", "");
            pRtf = Regex.Replace(pRtf, @"\\fnil", "");
            pRtf = Regex.Replace(pRtf, @"\\fswiss", "");
            pRtf = Regex.Replace(pRtf, @"\\fchartset[\d]*", "");
            pRtf = Regex.Replace(pRtf, @"\\fpqr[\d]*", "");
            pRtf = Regex.Replace(pRtf, @"{\\colortbl ;(\\red\d+\\green\d+\\blue\d+;)+}", "");
            pRtf = Regex.Replace(pRtf, @"\\viewkind4\\uc1", "");

            return pRtf;
        }

        /// <summary>
        /// Reemplaza los acentos y caracteres especiales del texto en formato RTF pasado por parámetro
        /// </summary>
        /// <param name="pRtf">Texto en formato RTF</param>
        /// <returns>Texto sin caracteres especiales en RTF</returns>
        public static string ReemplazarCaracteresEspecialesRTF(string pRtf)
        {
            string hex = "";
            int dec = 0;

            foreach (Match m in Regex.Matches(pRtf, @"\\'(?<hex>[a-f0-9][a-f0-9])"))
            {
                hex = m.Groups["hex"].Value;
                dec = Int32.Parse(hex, System.Globalization.NumberStyles.HexNumber);
                pRtf = Regex.Replace(pRtf, @"\\'" + hex, "&#" + dec + ";");
            }

            pRtf = Regex.Replace(pRtf, @"\\bullet", "&#149");
            pRtf = Regex.Replace(pRtf, @"\\lquote", "&#145");
            pRtf = Regex.Replace(pRtf, @"\\rquote", "&#146");
            pRtf = Regex.Replace(pRtf, @"\\ldblquote", "&#34;");//&#147
            pRtf = Regex.Replace(pRtf, @"\\rdblquote", "&#34;");//&#148
            pRtf = Regex.Replace(pRtf, @"\\lang[\d]*", "");

            return pRtf;
        }

        
        /// <summary>
        /// Sustituye etiquetas de formato RTF por etiquetas de formato HTML
        /// </summary>
        /// <param name="pRtf">Texto en formato RTF</param>
        /// <param name="pHtml">TRUE si deben reemplazarse las etiquetas RTF por etiquetas HTML</param>
        /// <returns>Texto en formato RTF con las etiquetas de formato cambiadas</returns>
        public static string ReemplazarEtiquetasFormatoRTF(string pRtf, bool pHtml)
        {
            if (pHtml)
            {
                pRtf = Regex.Replace(pRtf, @"\\b([^}]+?)\\b0", "<b>$1</b>");
                pRtf = Regex.Replace(pRtf, @"\\i([^}]+?)\\i0", "<i>$1</i>");
                pRtf = Regex.Replace(pRtf, @"\\ul([^}]+?)\\ulnone", "<u>$1</u>");
                pRtf = Regex.Replace(pRtf, @"\\strike([^}]+?)\\strike0", "<strike>$1</strike>");
                pRtf = Regex.Replace(pRtf, @"\\b", "");
            }
            else
            {
                pRtf = Regex.Replace(pRtf, @"\\b([^}]+?)\\b0", "$1");
                pRtf = Regex.Replace(pRtf, @"\\i([^}]+?)\\i0", "$1");
                pRtf = Regex.Replace(pRtf, @"\\ul([^}]+?)\\ulnone", "$1");
                pRtf = Regex.Replace(pRtf, @"\\strike([^}]+?)\\strike0", "$1");
                pRtf = Regex.Replace(pRtf, @"\\b", "");
            }
            return pRtf;
        }

        /// <summary>
        /// Quita las etiquetas RTF que no son necesarias del texto en formato RTF pasado por parámetro
        /// </summary>
        /// <param name="pRtf">Texto en formato RTF</param>
        /// <param name="pHtml">TRUE si deben reemplazarse las etiquetas RTF por etiquetas HTML</param>
        /// <returns>Texto en formato RTF sin etiquetas sobrantes</returns>
        public static string QuitarEtiquetasRTFSobrantes(string pRtf, bool pHtml)
        {
            string tabSpaces = "&nbsp;&nbsp;&nbsp;&nbsp;";

            pRtf = Regex.Replace(pRtf, @"\\pard", "");

            if (pHtml)
            {
                pRtf = Regex.Replace(pRtf, @"\\tab\s", tabSpaces);
                pRtf = Regex.Replace(pRtf, @"\\tab", tabSpaces);
                pRtf = Regex.Replace(pRtf, @"\\nowidctlpar", "<br/>" + Environment.NewLine);
                pRtf = Regex.Replace(pRtf, @"\\pard\\qc([^}]+?)\\par\\pard", "<p align=\"center\">$1</p>");
                pRtf = Regex.Replace(pRtf, @"\\pard\\qr([^}]+?)\\par\\pard", "<p align=\"right\">$1</p>");
                pRtf = Regex.Replace(pRtf, @"\\ltrpar", "<br/>" + Environment.NewLine);
                pRtf = Regex.Replace(pRtf, @"\\par", "<br/>" + Environment.NewLine);
            }
            else
            {
                pRtf = Regex.Replace(pRtf, @"\\tab\s", "");
                pRtf = Regex.Replace(pRtf, @"\\tab", "");
                pRtf = Regex.Replace(pRtf, @"\\nowidctlpar", Environment.NewLine);
                pRtf = Regex.Replace(pRtf, @"\\pard\\qc([^}]+?)\\par\\pard", "$1");
                pRtf = Regex.Replace(pRtf, @"\\pard\\qr([^}]+?)\\par\\pard", "$1");
                pRtf = Regex.Replace(pRtf, @"\\ltrpar", Environment.NewLine);
                pRtf = Regex.Replace(pRtf, @"\\par", "");
            }

            pRtf = Regex.Replace(pRtf, @"\\hyphpar[\d]*", "");
            pRtf = Regex.Replace(pRtf, @"\\qj", "");
            pRtf = Regex.Replace(pRtf, @"\\qc", "");

            //Enumeraciones
            pRtf = Regex.Replace(pRtf, @"{\\\*\\pn\\pnlvlblt\\pnf[\d]*\\pnindent[\d]*", "");

            if (pHtml)
            {
                pRtf = Regex.Replace(pRtf, @"{\\pntext\\f[\d]*\\'B7}", " &nbsp;&nbsp;&#149 ");
                pRtf = Regex.Replace(pRtf, @"{\\pntxtb\\'B7}", " &nbsp;&nbsp;&#149 ");
            }
            else
            {
                pRtf = Regex.Replace(pRtf, @"{\\pntext\\f[\d]*\\'B7}", "");
                pRtf = Regex.Replace(pRtf, @"{\\pntxtb\\'B7}", "");
            }

            //Quitamos las etiquetas que referencian enumeraciones pero no lo son
            pRtf = Regex.Replace(pRtf, @"{\\pntext\\f[\d]*\\'B7", "");
            pRtf = Regex.Replace(pRtf, @"{\\pntxtb\\'B7", "");

            pRtf = Regex.Replace(pRtf, @"\\sb[\d]*\s?", "");//SpaceBefore
            pRtf = Regex.Replace(pRtf, @"\\sa[\d]*\s?", "");//SpaceAfter
            pRtf = Regex.Replace(pRtf, @"\\tx[\d]*\s?", "");//Tab position in twips(1/20 th of a printers point) from the left margin
            pRtf = Regex.Replace(pRtf, @"\\expndtw-?[\d]*\s?", "");//Espacio entre caracteres en twips
            pRtf = Regex.Replace(pRtf, @"\\expnd-?[\d]*\s?", "");//Espacio entre caracteres

            pRtf = Regex.Replace(pRtf, @"tw[\d]*\s?", "");
            pRtf = Regex.Replace(pRtf, @"tw-[\d]*\s?", "");

            pRtf = Regex.Replace(pRtf, @"\\fi-?[\d]*\s?", "");//Left indent for just the first line
            pRtf = Regex.Replace(pRtf, @"\\ri-?[\d]*\s?", "");//Right indent

            pRtf = Regex.Replace(pRtf, @"\\li-?[\d]*\s?", "");//Left indent
            pRtf = Regex.Replace(pRtf, @"\\slmult-?[\d]*\s?", "");
            pRtf = Regex.Replace(pRtf, @"\\sl-?[\d]*\s?", "");//Space between lines
            pRtf = Regex.Replace(pRtf, @"\\keepn[\d]*\s?", "");
            pRtf = Regex.Replace(pRtf, @"\\keep[\d]*\s?", "");
            pRtf = Regex.Replace(pRtf, @"[{]?\\fs[\d]*\s?", "");
            pRtf = Regex.Replace(pRtf, @"{\\f[\d]*\s?[^}]*}", ""); //
            pRtf = Regex.Replace(pRtf, @"[{]?\\f[\d]*\s?", ""); //
            pRtf = Regex.Replace(pRtf, @"(\\cf\d+)\\cb\d+\\highlight\d+\s", "$1 ");
            pRtf = Regex.Replace(pRtf, @"\\cb\d+\\highlight\d+\s", "");
            pRtf = Regex.Replace(pRtf, @"\\cb[\d]*\s?", "");//BackGround Color
            pRtf = Regex.Replace(pRtf, @"\\cf[\d]*\s?", "");

            if (pHtml)
            {
                // fix up orphaned spans at start and end
                pRtf = Regex.Replace(pRtf, @"(^.*?)</span>", "$1");
                pRtf = pRtf + "</span>";

                // Convertir espacios de un formato a otro
                pRtf = Regex.Replace(pRtf, "  ", "&nbsp;&nbsp;");
            }

            pRtf = Regex.Replace(pRtf, @"}$", "");
            pRtf = Regex.Replace(pRtf, @"}*", "");

            return pRtf;
        }

        /// <summary>
        /// Reemplaza las etiquetas de imagen de formato RTF por etiquetas de imagen HTML
        /// </summary>
        /// <param name="pRtf">Texto en formato RTF</param>
        /// <returns>Texto en formato RTF con las etiquetas de imágenes cambiadas</returns>
        public static string ReemplazarEtiquetasImagenesRTF(string pRtf)
        {
            string pattern = @"{\\pict\\[^\\]*\\picw(?<width>[\d]*)\\pich(?<height>[\d]*)\\picwgoal(?<wgoal>[\d]*)\\pichgoal(?<hgoal>[\d]*)\s(?<image>[^}]*)";

            foreach (Match m in Regex.Matches(pRtf, pattern))
            {
                int width = int.Parse(m.Groups["width"].Value);
                int height = int.Parse(m.Groups["height"].Value);
                byte[] imagen = ToByteArray(m.Groups["image"].ToString());
                pRtf = pRtf.Replace(m.Value, "<img alt=\"Imagen\" src=\"data:image/png;base64:" + UtilImages.CodificarImagen(imagen) + "\"/>");
            }
            if (Regex.Matches(pRtf, @"\\obje").Count > 0)
            {
            }
            return pRtf;
        }

        /// <summary>
        /// Convierte una cadena hexadecimal a un array de bytes
        /// </summary>
        /// <param name="HexString">Cadena en hexadecimal</param>
        /// <returns>Array de bytes</returns>
        public static byte[] ToByteArray(String HexString)
        {
            int NumberChars = HexString.Length;
            byte[] bytes = new byte[NumberChars / 2];

            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(HexString.Substring(i, 2), 16);
            }
            return bytes;
        }

        #endregion
    }
}
