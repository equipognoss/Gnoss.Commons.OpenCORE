using Es.Riam.Util.AnalisisSintactico;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Es.Riam.Util
{
    /// <summary>
    /// Utiles para cadenas
    /// </summary>
    public static class UtilCadenas
    {

        #region Miembros estáticos

        private static Dictionary<int, string> mListaCaracteresNoReconocidosJavascript = new Dictionary<int, string>();

        private static Regex mReplace_a_Accents = new Regex("[á|à|ä|â]", RegexOptions.Compiled);
        private static Regex mReplace_e_Accents = new Regex("[é|è|ë|ê]", RegexOptions.Compiled);
        private static Regex mReplace_i_Accents = new Regex("[í|ì|ï|î]", RegexOptions.Compiled);
        private static Regex mReplace_o_Accents = new Regex("[ó|ò|ö|ô]", RegexOptions.Compiled);
        private static Regex mReplace_u_Accents = new Regex("[ú|ù|ü|û]", RegexOptions.Compiled);
        private static Regex mReplace_A_Accents = new Regex("[Á|À|Ä|Â]", RegexOptions.Compiled);
        private static Regex mReplace_E_Accents = new Regex("[É|È|Ë|Ê]", RegexOptions.Compiled);
        private static Regex mReplace_I_Accents = new Regex("[Í|Ì|Ï|Î]", RegexOptions.Compiled);
        private static Regex mReplace_O_Accents = new Regex("[Ó|Ò|Ö|Ô]", RegexOptions.Compiled);
        private static Regex mReplace_U_Accents = new Regex("[Ú|Ù|Ü|Û]", RegexOptions.Compiled);
        private static Regex mRegexQuitarHtml = new Regex(@"<(.|\n)*?>", RegexOptions.Compiled);


        private static Regex mRegexQuitarCaracteresInvalidosNombreArchivo = new Regex(@"[\\:\?<>\|]+", RegexOptions.Compiled);
        private static Regex mRegexQuitarBarraBajaNombreArchivo = new Regex(@"[_]+", RegexOptions.Compiled);
        private static Regex mRegexQuitarDosPuntosNombreArchivo = new Regex(@"[:]+", RegexOptions.Compiled);

        private static string[] mListaIdiomasPosibles = new string[] { "es", "en", "eu", "pt", "ca", "de", "fr", "gl", "it" };
        public static bool LowerStringGraph { get; set; } = false;

        #endregion

        #region Constantes

        /// <summary>
        /// Longitud máxima del texto
        /// </summary>
        private const int LONGITUD_TEXTO = 50;

        /// <summary>
        /// Constante con las letras del abecedario
        /// </summary>
        public const string LETRAS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Lista de palabras que se descartarán para el tratamiento de cadenas.
        /// </summary>
        public const string PALABRAS_DESCARTADAS = ",de,en,y,del,";

        public const string CONECTORES_INGLES = "(^| )and[ .,]|(^| )not[ .,]|(^| )only[ .,]|(^| )but[ .,]|(^| )also[ .,]|(^| )as[ .,]|(^| )well[ .,]|(^| )both[ .,]|(^| )no[ .,]|(^| )sooner[ .,]|(^| )than[ .,]|(^| )or[ .,]|(^| )either[ .,]|(^| )neither[ .,]|(^| )nor[ .,]|(^| )whether[ .,]|(^| )else[ .,]|(^| )otherwise[ .,]|(^| )if[ .,]|(^| )unless[ .,]|(^| )long[ .,]|(^| )in[ .,]|(^| )case[ .,]|(^| )of[ .,]|(^| )although[ .,]|(^| )though[ .,]|(^| )even[ .,]|(^| )despite[ .,]|(^| )spite[ .,]|(^| )regardless[ .,]|(^| )therefore[ .,]|(^| )hence[ .,]|(^| )thus[ .,]|(^| )so[ .,]|(^| )consequently[ .,]|(^| )then[ .,]|(^| )moreover[ .,]|(^| )furthermore[ .,]|(^| )besides[ .,]|(^| )addition[ .,]|(^| )to[ .,]|(^| )however[ .,]|(^| )nonetheless[ .,]|(^| )yet[ .,]|(^| )still[ .,]|(^| )on[ .,]|(^| )the[ .,]|(^| )other[ .,]|(^| )hand[ .,]|(^| )instead[ .,]|(^| )contrary[ .,]|(^| )because[ .,]|(^| )for[ .,]|(^| )result[ .,]|(^| )due[ .,]|(^| )owing[ .,]|(^| )since[ .,]|(^| )just[ .,]|(^| )that[ .,]|(^| )order[ .,]";

        public const string CONECTORES_ESPANOL = "(^| )ademas[ .,]|(^| )de[ .,]|(^| )lo[ .,]|(^| )contrario[ .,]|(^| )en[ .,]|(^| )cuanto[ .,]|(^| )seguida[ .,]|(^| )por[ .,]|(^| )otra[ .,]|(^| )parte[ .,]|(^| )ultimo[ .,]|(^| )una[ .,]|(^| )sobre[ .,]|(^| )todo[ .,]|(^| )causa[ .,]|(^| )ahora[ .,]|(^| )que[ .,]|(^| )como[ .,]|(^| )debido[ .,]|(^| )gracias[ .,]|(^| )culpa[ .,]|(^| )porque[ .,]|(^| )puesto[ .,]|(^| )visto[ .,]|(^| )dado[ .,]|(^| )ya[ .,]|(^| )menos[ .,]|(^| )con[ .,]|(^| )la[ .,]|(^| )condicion[ .,]|(^| )tal[ .,]|(^| )caso[ .,]|(^| )si[ .,]|(^| )siempre[ .,]|(^| )suponiendo[ .,]|(^| )consecuencia[ .,]|(^| )fin[ .,]|(^| )asi[ .,]|(^| )manera[ .,]|(^| )entonces[ .,]|(^| )para[ .,]|(^| )consiguiente[ .,]|(^| )eso[ .,]|(^| )tanto[ .,]|(^| )ante[ .,]|(^| )antes[ .,]|(^| )nada[ .,]|(^| )despues[ .,]|(^| )primer[ .,]|(^| )segundo[ .,]|(^| )lugar[ .,]|(^| )finalmente[ .,]|(^| )luego[ .,]|(^| )mas[ .,]|(^| )tarde[ .,]|(^| )concluir[ .,]|(^| )empezar[ .,]|(^| )terminar[ .,]|(^| )primero[ .,]|(^| )efecto[ .,]|(^| )otras[ .,]|(^| )palabras[ .,]|(^| )es[ .,]|(^| )decir[ .,]|(^| )o[ .,]|(^| )sea[ .,]|(^| )ejemplo[ .,]|(^| )pesar[ .,]|(^| )al[ .,]|(^| )aunque[ .,]|(^| )cambio[ .,]|(^| )mientras[ .,]|(^| )obstante[ .,]|(^| )pero[ .,]|(^| )sin[ .,]|(^| )embargo[ .,]|(^| )respecto[ .,]|(^| )esta[ .,]|(^| )esto[ .,]|(^| )este[ .,]|(^| )respecta[ .,]|(^| )segun[ .,]|(^| )pocas[ .,]|(^| )resumen[ .,]|(^| )resumir[ .,]|(^| )actualmente[ .,]|(^| )final[ .,]|(^| )principio[ .,]|(^| )apenas[ .,]|(^| )cuando[ .,]|(^| )desde[ .,]|(^| )durante[ .,]|(^| )nuestros[ .,]|(^| )dias[ .,]|(^| )otro[ .,]|(^| )tiempo[ .,]|(^| )hasta[ .,]|(^| )hoy[ .,]|(^| )dia[ .,]|(^| )tan[ .,]|(^| )pronto[ .,]|(^| )vez[ .,]";

        public static Regex RegExConectoresIngles = new Regex(CONECTORES_INGLES, RegexOptions.Compiled);
        public static Regex RegExConectoresEspanol = new Regex(CONECTORES_ESPANOL, RegexOptions.Compiled);

        private static Dictionary<string, string> CONVERSOR_ACUTE = new Dictionary<string, string>()
        {
            { "&ntilde;", "ñ" },
            { "&Ntilde;", "Ñ" },
            { "&Agrave;", "À" },
            { "&Aacute;","Á" },
            { "&Acirc;", "Â" },
            { "&Atilde;", "Ã" },
            { "&Auml;", "Ä" },
            { "&Aring;", "Å" },
            { "&AElig;", "Æ" },
            { "&Ccedil;", "Ç" },
            { "&Egrave;", "È" },
            { "&Eacute;", "É" },
            { "&Ecirc;", "Ê" },
            { "&Euml;", "Ë" },
            { "&Igrave;", "Ì" },
            { "&Iacute;", "Í" },
            { "&Icirc;", "Î" },
            { "&Iuml;", "Ï" },
            { "&ETH;", "Ð" },
            { "&Ograve;", "Ò" },
            { "&Oacute;", "Ó" },
            { "&Ocirc;", "Ô" },
            { "&Otilde;", "Õ" },
            { "&Ouml;", "Ö" },
            { "&Oslash;", "Ø" },
            { "&Ugrave;", "Ù" },
            { "&Uacute;", "Ú" },
            { "&Ucirc;", "Û" },
            { "&Uuml;", "Ü" },
            { "&Yacute;", "Ý" },
            { "&THORN;", "Þ" },
            { "&szlig;", "ß" },
            { "&agrave;", "à" },
            { "&aacute;", "á" },
            { "&acirc;", "â" },
            { "&atilde;", "ã" },
            { "&auml;", "ä" },
            { "&aring;", "å" },
            { "&aelig;", "æ" },
            { "&ccedil;", "ç" },
            { "&egrave;", "è" },
            { "&eacute;", "é" },
            { "&ecirc;", "ê" },
            { "&euml;", "ë" },
            { "&igrave;", "ì" },
            { "&iacute;", "í" },
            { "&icirc;", "î" },
            { "&iuml;", "ï" },
            { "&eth;", "ð" },
            { "&ograve;", "ò" },
            { "&oacute;", "ó" },
            { "&ocirc;", "ô" },
            { "&otilde;", "õ" },
            { "&ouml;", "ö" },
            { "&oslash;", "ø" },
            { "&ugrave;", "ù" },
            { "&uacute;", "ú" },
            { "&ucirc;", "û" },
            { "&uuml;", "ü" },
            { "&yacute;", "ý" },
            { "&thorn;", "þ" },
            { "&yuml;", "ÿ" },
        };

        #endregion

        #region Métodos de comparación lógica (no ASCII) entre cadenas

        /// <summary>
        /// Compara dos cadenas de forma natural, p.ej. si la cadena tiene números :
        /// cad100, cad110, cad200, cad15 -> las ordena cad15 cad100 cad110 cad200
        /// </summary>
        public static int ComparaCadenasLogica(string s1, string s2)
        {
            if ((s1 == null) && (s2 == null)) return 0;
            else if (s1 == null) return -1;
            else if (s2 == null) return 1;

            if ((s1.Equals(string.Empty) && (s2.Equals(string.Empty)))) return 0;
            else if (s1.Equals(string.Empty)) return -1;
            else if (s2.Equals(string.Empty)) return -1;

            bool sp1 = Char.IsLetterOrDigit(s1, 0);
            bool sp2 = Char.IsLetterOrDigit(s2, 0);

            if (sp1 && !sp2) return -1;
            if (!sp1 && sp2) return 1;

            int i1 = 0, i2 = 0;
            int r = 0;

            while (true)
            {
                bool c1 = Char.IsDigit(s1, i1);
                bool c2 = Char.IsDigit(s2, i2);

                if (!c1 && !c2)
                {
                    bool letter1 = Char.IsLetter(s1, i1);
                    bool letter2 = Char.IsLetter(s2, i2);

                    if ((letter1 && letter2) || (!letter1 && !letter2))
                    {
                        if (letter1 && letter2)
                            r = Char.ToLower(s1[i1]).CompareTo(Char.ToLower(s2[i2]));
                        else
                            r = s1[i1].CompareTo(s2[i2]);

                        if (r != 0) return r;
                    }
                    else if (!letter1 && letter2) return -1;
                    else if (letter1 && !letter2) return 1;
                }
                else if (c1 && c2)
                {
                    r = CompareNum(s1, ref i1, s2, ref i2);
                    if (r != 0) return r;
                }
                else if (c1)
                    return -1;
                else if (c2)
                    return 1;

                i1++;
                i2++;

                if ((i1 >= s1.Length) && (i2 >= s2.Length))
                    return 0;
                else if (i1 >= s1.Length)
                    return -1;
                else if (i2 >= s2.Length)
                    return -1;
            }
        }

        private static int CompareNum(string s1, ref int i1, string s2, ref int i2)
        {
            int nzStart1 = i1, nzStart2 = i2;
            int end1 = i1, end2 = i2;

            ScanNumEnd(s1, i1, ref end1, ref nzStart1);
            ScanNumEnd(s2, i2, ref end2, ref nzStart2);
            int start1 = i1; i1 = end1 - 1;
            int start2 = i2; i2 = end2 - 1;

            int nzLength1 = end1 - nzStart1;
            int nzLength2 = end2 - nzStart2;

            if (nzLength1 < nzLength2) return -1;
            else if (nzLength1 > nzLength2) return 1;

            for (int j1 = nzStart1, j2 = nzStart2; j1 <= i1; j1++, j2++)
            {
                int r = s1[j1].CompareTo(s2[j2]);
                if (r != 0) return r;
            }

            int length1 = end1 - start1;
            int length2 = end2 - start2;
            if (length1 == length2) return 0;
            if (length1 > length2) return -1;
            return 1;
        }

        private static void ScanNumEnd(string s, int start, ref int end, ref int nzStart)
        {
            nzStart = start;
            end = start;
            bool countZeros = true;

            while (Char.IsDigit(s, end))
            {
                if (countZeros && s[end].Equals('0'))
                    nzStart++;
                else countZeros = false;
                end++;
                if (end >= s.Length) break;
            }
        }

        #endregion

        #region Métodos de codificación/decodificación HTML


        /// <summary>
        /// Método para codificar en HTML
        /// </summary>
        /// <param name="pTexto">Texto a codificar</param>
        /// <returns>Texto codificado</returns>
        public static string HtmlEncode(string pTexto)
        {
            return HttpUtility.HtmlEncode(pTexto).Replace("&#39;", "'");
        }

        /// <summary>
        /// Método para decodificar HTML
        /// </summary>
        /// <param name="pTexto">Texto a decodificar</param>
        /// <returns>Texto decodificado</returns>
        public static string HtmlDecode(string pTexto)
        {
            string[] caracteresExcluidos = new string[] { "&nbsp;", "&laquo;", "&raquo;", "&ldquo;", "&rdquo;", "&hellip;", "&iexcl;", "&cent;", "&pound;", "&curren;", "&yen;", "&brvbar;", "&sect;", "&uml;", "&copy;", "&ordf;", "&not;", "&shy;", "&reg;", "&macr;", "&deg;", "&plusmn;", "&sup2;", "&sup3;", "&acute;", "&micro;", "&para;", "&middot;", "&cedil;", "&sup1;", "&ordm;", "&frac14;", "&frac12;", "&frac34;", "&iquest;" };

            foreach (string caracter in caracteresExcluidos)
            {
                pTexto = pTexto.Replace(caracter, " ");
            }

            return HttpUtility.HtmlDecode(pTexto).Replace("&#39;", "'");
        }


        #endregion

        #region SQL

        /// <summary>
        /// Controla los caracteres especiales para que no se produzca inyección de código
        /// </summary>
        /// <param name="valor">Caden a comprobar</param>
        /// <returns>Cadena formateada</returns>
        public static string ToSql(string valor)
        {
            string s = valor;

            s = s.Replace("'", "''");
            s = s.Replace("\"\"", "");
            s = s.Replace("--", "");

            return s;
        }

        /// <summary>
        /// Controla los caracteres especiales para que no se produzca inyección de código
        /// </summary>
        /// <param name="valor">Caden a comprobar</param>
        /// <returns>Cadena formateada</returns>
        public static string ToSparql(string valor)
        {
            string s = valor;

            s = s.Replace("\\", "\\\\");
            s = s.Replace("'", "\\'");
            s = s.Replace("\"\"", "");
            s = s.Replace("--", "");
            s = s.Replace("\"", "\\\"");

            return s;
        }

        /// <summary>
        /// Controla los caracteres especiales para que no se produzca inyección de código en consultas con like
        /// </summary>
        /// <param name="valor">Cadena comprobar</param>
        /// <returns>Cadena formateada</returns>
        public static string ToSqlConLike(string valor)
        {
            string s = valor;

            s = s.Replace("'", "''");
            s = s.Replace("\"\"", "");
            s = s.Replace("--", "");
            s = s.Replace("[", "[[]");
            s = s.Replace("%", "[%]");
            s = s.Replace("_", "[_]");

            return s;
        }

        #endregion

        #region Conversión de texto

        /// <summary>
        /// Convierte un string a su representación uri
        /// </summary>
        /// <param name="pTexto">Cadena de texto de entrada para convertir</param>
        /// <returns>Cadena de texto de salida con formato de representación URI</returns>
        public static string ConvertirTextoAUri(string pTexto)
        {
            string[] caracteres = { "%", "?", "&", "=", " ", "\"" };

            for (int i = 0; i <= caracteres.Length - 1; i++)
            {
                pTexto = pTexto.Replace(caracteres[i], Uri.EscapeDataString(caracteres[i]));
            }
            return pTexto;
        }

        /// <summary>
        /// Quita los acentos de la cadena de texto pasada por parámetro
        /// </summary>
        /// <param name="pInputString">Cadena de texto de entrada</param>
        /// <returns>Cadena de texto</returns>
        public static string RemoveAccentsWithRegEx(string pInputString)
        {
            pInputString = mReplace_a_Accents.Replace(pInputString, "a");
            pInputString = mReplace_e_Accents.Replace(pInputString, "e");
            pInputString = mReplace_i_Accents.Replace(pInputString, "i");
            pInputString = mReplace_o_Accents.Replace(pInputString, "o");
            pInputString = mReplace_u_Accents.Replace(pInputString, "u");
            pInputString = mReplace_A_Accents.Replace(pInputString, "A");
            pInputString = mReplace_E_Accents.Replace(pInputString, "E");
            pInputString = mReplace_I_Accents.Replace(pInputString, "I");
            pInputString = mReplace_O_Accents.Replace(pInputString, "O");
            pInputString = mReplace_U_Accents.Replace(pInputString, "U");

            return pInputString;
        }

        /// <summary>
        /// Remplaza caractes de HTML que representan letras acentuadas por sus caracteres originales ("í", "ñ", etc)
        /// </summary>
        /// <param name="pInputString">Cadena de texto de entrada</param>
        /// <returns>Cadena de texto</returns>
        public static string ReemplazarCaracteresHTML(string pInputString)
        {
            pInputString = pInputString.Replace("&amp;", "&");
            pInputString = pInputString.Replace("&aacute;", "á");
            pInputString = pInputString.Replace("&Aacute;", "Á");
            pInputString = pInputString.Replace("&eacute;", "é");
            pInputString = pInputString.Replace("&Eacute;", "É");
            pInputString = pInputString.Replace("&iacute;", "í");
            pInputString = pInputString.Replace("&Iacute;", "Í");
            pInputString = pInputString.Replace("&oacute;", "ó");
            pInputString = pInputString.Replace("&Oacute;", "Ó");
            pInputString = pInputString.Replace("&uacute;", "ú");
            pInputString = pInputString.Replace("&Uacute;", "Ú");
            pInputString = pInputString.Replace("&uuml;", "ü");
            pInputString = pInputString.Replace("&Uuml;", "Ü");
            pInputString = pInputString.Replace("&ntilde;", "ñ");
            pInputString = pInputString.Replace("&Ntilde;", "Ñ");
            pInputString = pInputString.Replace("&nbsp;", " ");
            pInputString = pInputString.Replace("&", "&amp;");

            return pInputString;
        }

        /// <summary>
        /// Quita los caracteres como '$','%','/' y el ampersand de una cadena
        /// </summary>
        /// <param name="pInputString">Cadena de texto de entrada</param>
        /// <returns>Cadena de texto</returns>
        public static string EliminarCaracteresUrlSem(string pInputString)
        {
            //Si el string es multiIdioma cojo el valor del 1º idioma:
            int indiceSepIdio = pInputString.IndexOf("|||");

            if (indiceSepIdio != -1 && pInputString[indiceSepIdio - 3] == '@')
            {
                pInputString = pInputString.Substring(0, indiceSepIdio);
                pInputString = pInputString.Substring(0, pInputString.LastIndexOf("@"));
            }

            pInputString = HttpUtility.UrlDecode(pInputString);
            pInputString = EliminarHtmlDeTexto(pInputString);

            pInputString = RemoveAccentsWithRegEx(pInputString);

            Regex distintoDeAlfanumerico = new Regex("[^a-zA-Z0-9 -]");

            pInputString = distintoDeAlfanumerico.Replace(pInputString, "");

            while (pInputString.IndexOf("  ") > -1)
            {
                pInputString = pInputString.Replace("  ", " ");
            }

            if (pInputString.Length > 50)
            {
                pInputString = pInputString.Substring(0, 50);

                int ultimoEspacio = pInputString.LastIndexOf(" ");
                if (ultimoEspacio > 0)
                {
                    pInputString = pInputString.Substring(0, ultimoEspacio);
                }
            }

            Regex espacio = new Regex("[ ]");
            pInputString = espacio.Replace(pInputString, "-");

            return pInputString.ToLower();
        }

        /// <summary>
        /// Limpia los caracteres extraños que pueda contener un nombre de archivo
        /// </summary>
        /// <param name="pFileName">Nombre del archivo que puede contener caracteres extraños</param>
        /// <returns></returns>
        public static string EliminarCaracetresInvalidosNombreArchivo(string pFileName)
        {
            return mRegexQuitarCaracteresInvalidosNombreArchivo.Replace(pFileName, string.Empty);
        }
        /// <summary>
        /// Reemplaza los caracteres extraños que pueda contener un nombre de archivo
        /// </summary>
        /// <param name="pFileName">Nombre del archivo que puede contener caracteres extraños</param>
        /// <param name="remplazo">Valor por el que reemplazaremos esos caracteres</param>
        /// <returns></returns>
        public static string ReemplazarCaracetresInvalidosNombreArchivo(string pFileName, string remplazo)
        {
            return mRegexQuitarCaracteresInvalidosNombreArchivo.Replace(pFileName, remplazo);
        }

        /// <summary>
        /// Reemplaza el caracter : en el nombre de archivo
        /// </summary>
        /// <param name="pFileName">Nombre del archivo que puede contener caracteres extraños</param>
        /// <param name="remplazo">Valor por el que reemplazaremos esos caracteres</param>
        /// <returns></returns>
        public static string ReemplazarDosPuntosNombreArchivo(string pFileName, string remplazo)
        {
            return mRegexQuitarDosPuntosNombreArchivo.Replace(pFileName, remplazo);
        }
        /// <summary>
        /// Reemplaza el caracter _ en el nombre de archivo
        /// </summary>
        /// <param name="pFileName">Nombre del archivo que puede contener caracteres extraños</param>
        /// <param name="remplazo">Valor por el que reemplazaremos esos caracteres</param>
        /// <returns></returns>
        public static string ReemplazarBarraBajaNombreArchivo(string pFileName, string remplazo)
        {
            return mRegexQuitarBarraBajaNombreArchivo.Replace(pFileName, remplazo);
        }

        public static string ReemplazarCadenaCaracteresNoAlfanumericos(string pCadena, string pCadenaRemplazo)
        {
            return Regex.Replace(pCadena, "[^a-zA-Z0-9_.]+", pCadenaRemplazo, RegexOptions.Compiled);
        }
        /// <summary>
        /// Sustituye los guiones por espacios en blanco
        /// </summary>
        /// <param name="pInputString">Cadena de texto de entrada</param>
        /// <returns>Cadena de texto</returns>
        public static string DecodificarUrlSem(string pInputString)
        {
            Regex separador = new Regex("[-]");

            pInputString = separador.Replace(pInputString, " ");

            return pInputString;
        }

        #endregion

        #region Texto

        /// <summary>
        /// Valida que el formato y letra del CIF son correctos
        /// </summary>
        /// <param name="pCif">CIF de empresa a validar</param>
        /// <returns>Verdadero si el CIF es correcto, falso en caso contrario</returns>
        public static bool ValidarNIFyCIF(string pCif)
        {
            try
            {
                //Quito espacios
                pCif = pCif.Replace(" ", "");
                //Se pasa a mayusculas
                pCif = pCif.ToUpper();
                //Se limpia de los caracteres '-' y '/' 

                pCif = pCif.Replace("-", "");
                pCif = pCif.Replace("/", "");

                if (pCif.Length == 9)
                {
                    // comprobamos que sea un NIF
                    TipoDocumentoNIF tipoNIF = TipoDocumentoNIF.Desconocido;
                    if (ValidarNIF(ref pCif, out tipoNIF))
                        return true;
                    //comprobamos que sea un CIF
                    else
                    {
                        //Se comprueba que la letra que designa el tipo de cif sea correcta
                        string[] caracteres = { "A", "B", "C", "D", "E", "F", "G", "H", "P", "Q", "S", "K", "L", "M", "X" };
                        bool encontrado = false;
                        int i = 0;

                        while (!encontrado && i < caracteres.Length)
                        {
                            encontrado = pCif.Contains(caracteres[i]);
                            i = i + 1;
                        }
                        if (encontrado)
                        {
                            //Se comprueba si es extranjero en ese caso se calcula el nif, cambiando la X, por 0
                            if (pCif.StartsWith("X"))
                            {
                                pCif = "0" + pCif.Remove(0, 1);
                                return ValidarNIF(ref pCif, out tipoNIF);
                            }
                            else
                            {
                                int suma = System.Convert.ToInt32(pCif.Substring(2, 1)) + System.Convert.ToInt32(pCif.Substring(4, 1)) + System.Convert.ToInt32(pCif.Substring(6, 1));
                                for (int j = 1; j <= 4; j++)
                                {
                                    suma = suma + (2 * System.Convert.ToInt32(pCif.Substring(2 * j - 1, 1)) % 10) + (2 * System.Convert.ToInt32(pCif.Substring(2 * j - 1, 1)) / 10);
                                }
                                int control = 10 - (suma % 10);
                                //Se comprueba si es de tipo 'P' o 'S', es decir, 
                                //Corporaciones Locales (Ayuntamientos, etc.)y Organismos públicos.
                                if (pCif.Contains("P") || pCif.Contains("S"))
                                    return (pCif[8]) == (char)(64 + control);
                                else
                                //Resto de tipos de CIF
                                {
                                    if (control == 10)
                                        control = 0;
                                    return (System.Convert.ToInt32(pCif.Substring(8, 1)) == control);
                                }
                            }
                        }
                        else return false;
                    }
                }
                else return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Valida si el formato y letra del NIF son correctos
        /// </summary>
        /// <param name="pNif">NIF a validar</param>
        /// <returns>Verdadero si el NIF es correcto, falso en caso contrario</returns>
        public static bool ValidarNIF(ref string pNif, out TipoDocumentoNIF pTipoDocumento)
        {
            pTipoDocumento = TipoDocumentoNIF.Desconocido;
            string letras = "TRWAGMYFPDXBNJZSQVHLCKE";

            // Formatos del NIF
            // DNI: 12345678A | 12345678-A | 1234567A (ERROR: FALTAN DIGITOS, puede que el 0) | 
            // NIE: X12345678 | Y-1234567-8 | Z-1234567-A | Z1234567A | Z234567A | Z-234567-A

            // ^\d{8}[TRWAGMYFPDXBNJZSQVHLCKEtrwagmyfpdxbnjzsqvhlcke]$

            /*
             * Expresiones regulares
             * DNI con o sin guion
             * ^(\d{8})([-]?)([TRWAGMYFPDXBNJZSQVHLCKEtrwagmyfpdxbnjzsqvhlcke]{1})$
             * 
             * NIE con o sin guiones
             * ^([XYZxyz]{1})([-]?)(\d{7})([-]?)([TRWAGMYFPDXBNJZSQVHLCKEtrwagmyfpdxbnjzsqvhlcke]{1}?)$
             * */

            // Se usan comprobaciones sin guiones para tener la misma estructura siempre.
            bool esDNIValido = Regex.IsMatch(pNif, @"(^[0-9])(\d{7})([" + letras.ToUpper() + letras.ToLower() + "]{1})$");
            bool esNIEValido = Regex.IsMatch(pNif, @"(^[XYZxyz]{1})(\d{7})([" + letras.ToUpper() + letras.ToLower() + "]{1}?)$");

            if (!(esDNIValido || esNIEValido))
                return false;

            if (esDNIValido)
            {
                pTipoDocumento = TipoDocumentoNIF.DNI;
                return ComprobacionLetraDNI(pNif, letras);
            }
            if (esNIEValido)
            {
                pTipoDocumento = TipoDocumentoNIF.NIE;
                return ComprobacionLetrasNIE(pNif, letras);
            }
            return false;
        }

        private static bool ComprobacionLetrasNIE(string pNif, string pLetras)
        {
            try
            {
                string primeraLetra = pNif.Substring(0, 1).ToUpper();
                // Quitamos la primera letra
                pNif = pNif.Substring(1);
                switch (primeraLetra)
                {
                    case "X":
                        primeraLetra = "0";
                        break;
                    case "Y":
                        primeraLetra = "1";
                        break;
                    case "Z":
                        primeraLetra = "2";
                        break;
                }
                pNif = $"{primeraLetra}{pNif}";

                int nifNum = int.Parse(pNif.Substring(0, pNif.Length - 1));
                int wordPosition = 8;

                if (string.Compare(pNif[wordPosition].ToString(), pLetras[nifNum % 23].ToString(), true) == 0)
                {
                    if (char.IsLower(pNif, wordPosition))
                        pNif = pNif.Replace(pNif[wordPosition], char.ToUpper(pNif[wordPosition]));
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                // TODO: Guardar Log, NIE con formato novalido, falla al convertir a string o algún otro error similar
                // Escrbir el NIE
            }

            return false;
        }

        private static bool ComprobacionLetraDNI(string pNif, string pLetras)
        {
            try
            {
                int nifNum = int.Parse(pNif.Substring(0, pNif.Length - 1));

                if (string.Compare(pNif[8].ToString(), pLetras[nifNum % 23].ToString(), true) == 0)
                {
                    if (char.IsLower(pNif, 8))
                        pNif = pNif.Replace(pNif[8], char.ToUpper(pNif[8]));
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                // TODO: Guardar Log, DNI con formato novalido, falla al convertir a string o algún otro error similar
                // Escrbir el DNI

            }

            return false;
        }

        /// <summary>
        /// Valida que el formato de correo electrónico sea correcto
        /// </summary>
        /// <param name="pEmail">Email para validar</param>
        /// <returns>True en caso de validación correcta, falso en caso contrario</returns>
        public static bool ValidarEmail(string pEmail)
        {
            if (!Regex.IsMatch(pEmail, @"\w+([-+.])*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
            {
                return false;
            }
            else
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(pEmail);
                    return true;
                }
                catch
                {
                    return false;
                }
            }     
        }

        /// <summary>
        /// Valida que el formato del código postal sea correcto
        /// </summary>
        /// <param name="pCP">Código postal para validar</param>
        /// <returns>Devuelve true en caso de ser correcta la validación, falso en caso contrario</returns>
        public static bool ValidarCP(string pCP)
        {
            if (!Regex.IsMatch(pCP, @"^\d{5}$"))
                return false;
            return true;
        }

        /// <summary>
        /// Forma el prural de una palabra
        /// </summary>
        /// <param name="pPalabra">Palabra en singular</param>
        /// <returns>Plural de la palabra pasada como parámetro</returns>
        public static string FormarPlural(string pPalabra)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(pPalabra);
            char ultima = pPalabra[pPalabra.Length - 1];

            //Si acaba en vocal se le agrega -s
            if (ultima == 'a' || ultima == 'e' || ultima == 'i' || ultima == 'o' || ultima == 'u')
                sb.Append('s');
            //Si acaba en consonante se añade -es
            else
            {
                sb.Append("es");
                //Reemplazar acentos
                sb.Replace('á', 'a');
                sb.Replace('é', 'e');
                sb.Replace('í', 'i');
                sb.Replace('ó', 'o');
                sb.Replace('ú', 'u');
            }
            return sb.ToString();
        }

        /// <summary>
        /// Devuelve el texto adaptado a la longitud máxima
        /// </summary>
        /// <param name="pTexto">Texto original</param>
        /// <returns>Texto ajustado</returns>
        public static string TextoCortado(string pTexto)
        {
            return TextoCortado(pTexto, LONGITUD_TEXTO);
        }

        /// <summary>
        /// Comprueba si texto1 contiene a texto 2
        /// </summary>
        /// <param name="pTexto1">Texto donde buscar</param>
        /// <param name="pTexto2">Texto a buscar</param>
        /// <param name="pCoincidirMayusMinus">Detrmina si tienen que coincidir mayúsculas y minúsculas</param>
        /// <param name="pSoloPalabrasCompletas">Derermina si tiene que estar la palabra completa</param>
        /// <returns>Devuelve el resultado de la búsqueda</returns>
        public static bool ContieneTexto(string pTexto1, string pTexto2, bool pCoincidirMayusMinus, bool pSoloPalabrasCompletas)
        {
            string texto1 = pTexto1;
            string texto2 = pTexto2;

            if (!pCoincidirMayusMinus)
            {
                texto1 = texto1.ToLower();
                texto2 = texto2.ToLower();
            }
            if (texto1.Contains(texto2))
            {
                if (pSoloPalabrasCompletas)
                {
                    int i = 0;
                    //Obtengo el indice donde comienza la cadena
                    i = texto1.IndexOf(texto2, i);

                    if (i == -1)
                    {
                        //No son iguales
                        return false;
                    }
                    if (i == 0 || texto1[i - 1] == ' ')
                    {
                        //El principio de las cadenas es igual
                        int k = i + texto2.Length;

                        if (k == texto1.Length || texto1[k] == ' ')
                        {
                            //El final de las cadenas también es igual
                            return true;
                        }
                    }
                }
                else
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Devuelve el texto adaptado a la longitud máxima
        /// </summary>
        /// <param name="pTexto">Texto original</param>
        /// <param name="pLongitud">Longitud máxima</param>
        /// <returns>Texto ajustado</returns>
        public static string TextoCortado(string pTexto, int pLongitud)
        {
            if (pTexto.Length > pLongitud)
                return pTexto.Substring(0, pLongitud) + "...";
            return pTexto;
        }

        /// <summary>
        /// Devuelve el texto adaptado a la longitud máxima sin cortar ninguna palabra
        /// </summary>
        /// <param name="pTexto">Texto original</param>
        /// <param name="pLongitud">Longitud máxima</param>
        /// <returns>Texto ajustado</returns>
        public static string TextoCortadoSinCortarPalabras(string pTexto, int pLongitud)
        {
            string nuevaCadena = pTexto;
            if (pTexto.Length > pLongitud)
            {
                nuevaCadena = pTexto.Substring(0, pLongitud);
                if (pTexto.Substring(pLongitud, 1) != " " && pTexto.LastIndexOf(' ') > 0)
                {
                    int fin = nuevaCadena.LastIndexOf(' ');
                    if (fin > -1)
                    {
                        nuevaCadena = nuevaCadena.Substring(0, fin);
                    }
                }
                nuevaCadena = nuevaCadena + "...";
            }
            return nuevaCadena;
        }

        /// <summary>
        /// Devuelve una cadena formateada a partir de una lista de strings
        /// simplemente los une con una coma entre medio y un espacio
        /// </summary>
        /// <param name="pListaTextos">Lista de strings</param>
        /// <returns>Cadena de texto formateada</returns>
        public static string CadenaFormatoTexto(List<string> pListaTextos)
        {
            string aux = "";

            foreach (string texto in pListaTextos)
            {
                if (aux != "")
                    aux += ", ";
                aux += texto;
            }
            return aux;
        }

        /// <summary>
        /// Devuelve una cadena formateada a partir de una lista de strings
        /// simplemente los une con una coma entre medio y SIN espacio
        /// </summary>
        /// <param name="pListaTextos">Lista de strings</param>
        /// <returns>Cadena de texto formateada</returns>
        public static string ComponerTextoSepComasDeLista(List<string> pListaTextos)
        {
            string aux = "";

            foreach (string texto in pListaTextos)
            {
                if (aux != "")
                    aux += ",";
                aux += texto;
            }
            return aux;
        }

        /// <summary>
        /// Separa una cadena de texto convirtiéndola en una lista de strings
        /// </summary>
        /// <param name="pTexto">Cadena de texto separada por comas</param>
        /// <returns>Lista de strings</returns>
        public static List<string> SepararTexto(string pTexto)
        {
            if (pTexto == null)
            {
                return new List<string>();
            }
            //Quito carácteres extraños de la lista de tags

            foreach (int codigo in UtilCadenas.ListaCaracteresNoReconocidosJavascript.Keys)
            {
                pTexto = pTexto.Replace(((char)codigo).ToString(), "");
            }

            char separador = ',';
            List<string> lista = new List<string>();


            if (pTexto != "")
            {
                string[] arrayTextos = pTexto.Split(new char[] { separador });

                foreach (string texto in arrayTextos)
                {
                    if (texto.Trim() != "" && !lista.Contains(texto.Trim()))
                        lista.Add(texto.Trim());
                }
            }
            return lista;
        }

        /// <summary>
        /// Igual que la funcion TrimEnd pero con admitiendo cadenas
        /// </summary>
        /// <param name="pCadenaOriginal"></param>
        /// <param name="pTextoATrimar"></param>
        /// <returns></returns>
        public static string TrimEndCadena(string pCadenaOriginal, string pTextoATrimar)
        {
            while (pCadenaOriginal.EndsWith(pTextoATrimar))
            {
                pCadenaOriginal = pCadenaOriginal.Substring(0, pCadenaOriginal.Length - pTextoATrimar.Length);
            }
            return pCadenaOriginal;
        }

        /// <summary>
        /// Igual que la funcion TrimStart pero con admitiendo cadenas
        /// </summary>
        /// <param name="pCadenaOriginal"></param>
        /// <param name="pTextoATrimar"></param>
        /// <returns></returns>
        public static string TrimStartCadena(string pCadenaOriginal, string pTextoATrimar)
        {
            while (pCadenaOriginal.StartsWith(pTextoATrimar))
            {
                pCadenaOriginal = pCadenaOriginal.Substring(pTextoATrimar.Length);
            }
            return pCadenaOriginal;
        }

        /// <summary>
        /// Igual que la funcion Trim pero con admitiendo cadenas
        /// </summary>
        /// <param name="pCadenaOriginal"></param>
        /// <param name="pTextoATrimar"></param>
        /// <returns></returns>
        public static string TrimCadena(string pCadenaOriginal, string pTextoATrimar)
        {
            return UtilCadenas.TrimStartCadena(UtilCadenas.TrimEndCadena(pCadenaOriginal, pTextoATrimar), pTextoATrimar);
        }


        /// <summary>
        /// Se comprueba el tamaño de cada palabra en la frase y si esta excede de pTamanioMaximo se separa en dos
        /// </summary>
        /// <param name="pTexto">Cadena de texto de entrada para convertir</param>
        /// <param name="pTamanioMaximo">tamaño maximo de cada palabra</param>
        /// <returns>Cadena de texto de salida con formato de representación URI</returns>
        public static string SepararPalabrasDemasiadoLargas(string pTexto, int pTamanioMaximo)
        {
            List<string> listaPalabras = new List<string>();
            string aux = pTexto;
            string aux2 = pTexto;

            int posicionEspacio = aux.IndexOf(" ");
            if (posicionEspacio == -1)
            {
                listaPalabras.Add(aux);
            }
            else
            {
                while (posicionEspacio > -1)
                {
                    listaPalabras.Add(aux.Substring(0, posicionEspacio));
                    aux = aux.Substring(posicionEspacio + 1);
                    posicionEspacio = aux.IndexOf(" ");
                }
            }

            foreach (string palabra in listaPalabras)
            {
                if (palabra != "" && palabra != " " && !palabra.Contains(" ") && palabra.Length >= pTamanioMaximo)
                {
                    string palabraOriginal = palabra;
                    string palabraFinal = "";

                    //Si una palabra es demasiado larga la intentamos acortar separando con criterios "_" --> " " o separando por la mitad
                    if (palabra.Contains("_"))
                    {
                        palabraFinal = palabra.Replace("_", " ");
                    }
                    else
                    {
                        palabraFinal = palabra.Substring(0, palabra.Length / 2) + " " + palabra.Substring(palabra.Length / 2);
                    }

                    aux2 = aux2.Replace(palabraOriginal, palabraFinal);
                }

            }
            listaPalabras.Clear();
            return aux2;
        }

        /// <summary>
        /// Devuelve el tag pasado como parámetro limpio de caracteres no permitidos.
        /// </summary>
        /// <param name="pTag">Tag original</param>
        /// <returns>Tag limpio de caracteres no permitidos</returns>
        public static string LimpiarTag(string pTag)
        {
            string tagLimpio = pTag;

            while (tagLimpio.Length > 0 && (tagLimpio[0] == '.' || tagLimpio[0] == ' '))
            {
                tagLimpio = tagLimpio.Substring(1);
            }

            while (tagLimpio.Length > 0 && (tagLimpio[tagLimpio.Length - 1] == '.' || tagLimpio[tagLimpio.Length - 1] == ' '))
            {
                tagLimpio = tagLimpio.Substring(0, tagLimpio.Length - 1);
            }

            tagLimpio = tagLimpio.Replace("|", "");
            tagLimpio = tagLimpio.Replace("\"", "");
            tagLimpio = tagLimpio.Replace(":", "");

            return tagLimpio;
        }

        /// <summary>
        /// Convierte la 1º letra de cada palabra a mayúsculas.
        /// </summary>
        /// <param name="pTexto">Texto a convertir</param>
        /// <returns>Texto con la 1º letra de cada palabra a mayúsculas</returns>
        public static string ConvertirPrimeraLetraPalabraAMayusculas(string pTexto)
        {
            string[] separadores = new string[AnalizadorSintactico.SEPARADORES.Length + 3];
            AnalizadorSintactico.SEPARADORES.CopyTo(separadores, 0);
            separadores[AnalizadorSintactico.SEPARADORES.Length] = " ";
            separadores[AnalizadorSintactico.SEPARADORES.Length + 1] = ".";
            separadores[AnalizadorSintactico.SEPARADORES.Length + 2] = "-";

            string[] palabras = pTexto.Split(separadores, StringSplitOptions.RemoveEmptyEntries);

            string textoFinal = "";

            string palabra2;

            int contador = 0;

            foreach (string palabra in palabras)
            {
                palabra2 = palabra;
                if (palabra.Contains("+") && palabra.Length >= palabra.IndexOf("+") + 2)
                {
                    palabra2 = palabra.Substring(0, palabra.IndexOf("+") + 1) + palabra.Substring(palabra.IndexOf("+") + 1, 1).ToUpper() + palabra.Substring(palabra.IndexOf("+") + 2) + " ";
                }

                //Pongo los símbolos intermedios que hay entre palabra y palabra (espacios, comas...)
                while (contador < pTexto.Length && !pTexto[contador].Equals(palabra[0]))
                {
                    textoFinal += pTexto[contador];
                    contador++;
                }

                if (AnalizadorSintactico.RegExSiglos.IsMatch(palabra2))
                {
                    textoFinal += palabra2.ToUpper();
                }
                else if (palabra2.Length > 1)
                {
                    textoFinal += palabra2.Substring(0, 1).ToUpper() + palabra2.Substring(1);
                }
                else if (palabra2.Length == 1)
                {
                    textoFinal += palabra2.ToUpper();
                }

                contador += palabra.Length;
            }

            //Pongo los símbolos del final de la frase (puntos, cierre de paréntesis...)
            while (contador < pTexto.Length)
            {
                textoFinal += pTexto[contador];
                contador++;
            }

            return textoFinal;
        }
        public static string PrimerCaracterAMayuscula(string pPalabra)
        {
            char[] primerCaracter = pPalabra.ToCharArray();
            primerCaracter[0] = Char.ToUpper(primerCaracter[0]);
            return new string(primerCaracter);
        }

        public static string PrimerCaracterAMinuscula(string pPalabra)
        {
            char[] primerCaracter = pPalabra.ToCharArray();
            primerCaracter[0] = Char.ToLower(primerCaracter[0]);
            return new string(primerCaracter);
        }

        /// <summary>
        /// Convierte la 1º letra de la frase a mayúsculas
        /// </summary>
        /// <param name="pFrase">Texto a convertir</param>
        /// <returns>Texto con la 1º letra de cada palabra a mayúsculas</returns>
        public static string ConvertirPrimeraLetraDeFraseAMayúsculas(string pFrase)
        {
            if (!string.IsNullOrEmpty(pFrase))
            {
                bool esalfabetico = false;
                int i = 0;
                char primerCaracter = ' ';
                while (i < pFrase.Length && !esalfabetico)
                {
                    primerCaracter = (char)pFrase[i];
                    esalfabetico = Char.IsLetter(primerCaracter);
                    i++;
                }


                pFrase = pFrase.Remove(i - 1, 1);
                pFrase = pFrase.Insert(i - 1, char.ToUpper(primerCaracter).ToString());
            }

            return pFrase;
        }

        /// <summary>
        /// Convierte la 1º letra de cada palabra a mayúsculas.
        /// </summary>
        /// <param name="pTexto">Texto a convertir</param>
        /// <returns>Texto con la 1º letra de cada palabra a mayúsculas</returns>
        public static string ConvertirPrimeraLetraPalabraAMayusculasExceptoArticulos(string pTexto)
        {
            string[] separadores = new string[AnalizadorSintactico.SEPARADORES.Length + 3];
            AnalizadorSintactico.SEPARADORES.CopyTo(separadores, 0);
            separadores[AnalizadorSintactico.SEPARADORES.Length] = " ";
            separadores[AnalizadorSintactico.SEPARADORES.Length + 1] = ".";
            separadores[AnalizadorSintactico.SEPARADORES.Length + 2] = "-";

            string[] palabras = pTexto.Split(separadores, StringSplitOptions.RemoveEmptyEntries);

            string textoFinal = "";

            string palabra2;

            int contador = 0;

            foreach (string palabra in palabras)
            {
                palabra2 = palabra;
                if (palabra.Contains("+") && palabra.Length >= palabra.IndexOf("+") + 2)
                {
                    palabra2 = palabra.Substring(0, palabra.IndexOf("+") + 1) + palabra.Substring(palabra.IndexOf("+") + 1, 1).ToUpper() + palabra.Substring(palabra.IndexOf("+") + 2) + " ";
                }

                //Pongo los símbolos intermedios que hay entre palabra y palabra (espacios, comas...)
                while (contador < pTexto.Length && !pTexto[contador].Equals(palabra[0]))
                {
                    textoFinal += pTexto[contador];
                    contador++;
                }

                if (AnalizadorSintactico.RegExSiglos.IsMatch(palabra2))
                {
                    textoFinal += palabra2.ToUpper();
                }
                else if (!AnalizadorSintactico.EsArticuloOConjuncionOPreposicionesComunes(palabra2))
                {
                    if (palabra2.Length > 1)
                    {
                        textoFinal += palabra2.Substring(0, 1).ToUpper() + palabra2.Substring(1);
                    }
                    else if (palabra2.Length == 1)
                    {
                        textoFinal += palabra2.ToUpper();
                    }
                }
                else
                {
                    textoFinal += palabra2;
                }

                contador += palabra.Length;
            }

            //Pongo los símbolos del final de la frase (puntos, cierre de paréntesis...)
            while (contador < pTexto.Length)
            {
                textoFinal += pTexto[contador];
                contador++;
            }

            return textoFinal;
        }

        /// <summary>
        /// Convierte la frase a mayúsculas
        /// </summary>
        /// <param name="pFrase">Texto a convertir</param>
        /// <returns>Texto convertido a mayúsculas</returns>
        public static string ConvertirAMayúsculas(string pFrase)
        {
            return pFrase.ToUpper();
        }

        /// <summary>
        /// Limpia el nombre para los caracteres estén en la siguiente lista: [a-zA-Z0-9ñÑüÜáéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛ'´`çÇ-]{4,30}
        /// </summary>
        /// <param name="pNombre"></param>
        /// <returns></returns>
        public static string LimpiarCaracteresNombreCortoRegistro(string pNombre)
        {
            string nombreLimpio = "";
            List<char> nombreLimpioArray = new List<char>();
            char[] cartPermi = { 'ñ', 'Ñ', 'ü', 'Ü', 'á', 'é', 'í', 'ó', 'ú', 'Á', 'É', 'Í', 'Ó', 'Ú', 'à', 'è', 'ì', 'ò', 'ù', 'À', 'È', 'Ì', 'Ò', 'Ù', 'â', 'ê', 'î', 'ô', 'û', 'Â', 'Ê', 'Î', 'Ô', 'Û', '\'', '´', '`', 'ç', 'Ç', '-' };//ñÑüÜáéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛ'´`çÇ-
            List<char> caracteresPermitidos = new List<char>(cartPermi);

            foreach (char caracter in pNombre)
            {
                if ((caracter >= 'a' && caracter <= 'z') || (caracter >= 'A' && caracter <= 'Z') || (caracter >= '0' && caracter <= '9') || caracteresPermitidos.Contains(caracter))
                {
                    nombreLimpioArray.Add(caracter);
                }
                else if (caracter == ' ')
                {
                    nombreLimpioArray.Add('-');
                }
            }

            nombreLimpio = new string(nombreLimpioArray.ToArray()).ToLower();

            return nombreLimpio;
        }

        public static string ObtenerUrlPropiaDeIdioma(string pUrl, string pIdioma)
        {
            string urlReturn = "";

            string[] urls = pUrl.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string urlPropia in urls)
            {
                string idioma = "es";
                string[] urlIdioma = urlPropia.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);

                if (urlIdioma.Length > 1) { idioma = urlIdioma[1]; }

                if (string.IsNullOrEmpty(urlReturn) || idioma.Equals(pIdioma))
                {
                    urlReturn = urlIdioma[0];

                    if (idioma.Equals(pIdioma))
                    {
                        break;
                    }
                }
            }

            return urlReturn;
        }


        /// <summary>
        /// Obtiene el texto asociado a un idioma de un texto con el siguiente formato nombre@es|||nombre@en|||....
        /// </summary>
        /// <param name="pTexto">Texto completo con todos los idiomas</param>
        /// <param name="pIdioma">Idioma que queremos coger</param>
        /// <param name="pIdiomaDefecto">Idioma que seleccionaremos si no está el idioma deseado</param>
        /// <returns></returns>
        public static string ObtenerTextoDeIdioma(string pTexto, string pIdioma, string pIdiomaDefecto, bool pSoloIdiomaIndicado = false)
        {
            if (!string.IsNullOrEmpty(pTexto) && pIdioma != null)
            {
                //Quitamos los saltos de linea finales
                pTexto = pTexto.Replace("\r\n", "");

                string[] nombres = pTexto.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);

                //Unimos las partes que no contengan idioma
                List<string> listaNombresConcatenados = new List<string>();
                string nombreConcatenado = "";
                foreach (string nombre in nombres)
                {
                    if (string.IsNullOrEmpty(nombreConcatenado))
                    {
                        nombreConcatenado += nombre;
                    }
                    else
                    {
                        nombreConcatenado += $"|||{nombre}";
                    }
                    if (nombreConcatenado.Contains("@"))
                    {
                        listaNombresConcatenados.Add(nombreConcatenado);
                        nombreConcatenado = "";
                    }
                }
                if (!string.IsNullOrEmpty(nombreConcatenado))
                {
                    listaNombresConcatenados.Add(nombreConcatenado);
                }
                nombres = listaNombresConcatenados.ToArray();
                //Si esta el idioma elegido lo seleccionamos
                string nombreFinal = nombres.FirstOrDefault(texto => texto.EndsWith("@" + pIdioma));
                if (!string.IsNullOrEmpty(nombreFinal))
                {
                    nombreFinal = nombreFinal.Substring(0, nombreFinal.LastIndexOf("@" + pIdioma));
                    return nombreFinal;
                }

                //Si no ha encontrado texto con el idioma elegido y el idioma tiene región, lo buscamos sin region.
                if (pIdioma.Contains('-'))
                {
                    string idioma = pIdioma.Substring(0, pIdioma.IndexOf('-'));
                    nombreFinal = nombres.FirstOrDefault(texto => texto.EndsWith("@" + idioma));
                    if (!string.IsNullOrEmpty(nombreFinal))
                    {
                        nombreFinal = nombreFinal.Substring(0, nombreFinal.LastIndexOf("@" + idioma));
                        return nombreFinal;
                    }
                }

                //Sino ha encontrado texto, y tenemos un idioma por defecto, lo buscamos con el idioma por defecto.
                if (!string.IsNullOrEmpty(pIdiomaDefecto))
                {
                    nombreFinal = nombres.FirstOrDefault(texto => texto.EndsWith("@" + pIdiomaDefecto));
                    if (!string.IsNullOrEmpty(nombreFinal))
                    {
                        nombreFinal = nombreFinal.Substring(0, nombreFinal.LastIndexOf("@" + pIdiomaDefecto));
                        return nombreFinal;
                    }
                }

                //Si aun no tiene ningún idioma seleccionado, seleccionamos el primero
                nombreFinal = nombres[0];
                if (nombreFinal.Contains("@"))
                {
                    string idioma = nombreFinal.Substring(nombreFinal.LastIndexOf('@') + 1);
                    //Comprobamos que el idioma tenga un formato decente (es, es-es)
                    if (idioma.Length == 2 || (idioma.Contains("-") && idioma.Length == 5))
                    {
                        if (!pSoloIdiomaIndicado)
                        {
                            nombreFinal = nombreFinal.Substring(0, nombreFinal.LastIndexOf("@" + idioma));
                            return nombreFinal;
                        }
                        else
                        {
                            return "";
                        }
                    }
                }
                return nombreFinal;
            }
            return pTexto;
        }

        /// <summary>
        /// Obtiene un diccionario con los idiomas del texto y cada texto.
        /// </summary>
        /// <param name="pTexto">texto con formato nombre@es|||nombre@en|||....</param>
        /// <returns>Diccionario con los idiomas del texto y cada texto</returns>
        public static Dictionary<string, string> ObtenerTextoPorIdiomas(string pTexto)
        {
            Dictionary<string, string> listaIdioma = new Dictionary<string, string>();


            foreach (string texto in pTexto.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (texto.Length > 2 && texto.LastIndexOf("@") == texto.Length - 3)
                {
                    string idioma = texto.Substring(texto.LastIndexOf("@") + 1);

                    if (mListaIdiomasPosibles.Contains(idioma) && !listaIdioma.ContainsKey(idioma))
                    {
                        listaIdioma.Add(idioma, texto.Substring(0, texto.LastIndexOf("@")));
                    }
                }
            }


            return listaIdioma;
        }

        /// <summary>
        /// Obtiene un diccionario con una lista de clavess/valor obtenidas de un texto con el siguiente formato: 'clave=valor|||clave=valor|||....'
        /// </summary>
        /// <param name="pTexto">Texto del que extraer las propiedades</param>
        /// <returns></returns>
        public static Dictionary<string, string> ObtenerPropiedadesDeTexto(string pTexto)
        {
            Dictionary<string, string> listaPropiedades = new Dictionary<string, string>();
            if (pTexto != null)
            {
                string[] propiedades = pTexto.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string propiedad in propiedades)
                {
                    string[] claveValor = propiedad.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

                    if (claveValor.Length == 2 && !listaPropiedades.ContainsKey(claveValor[0]))
                    {
                        listaPropiedades.Add(claveValor[0], claveValor[1]);
                    }
                }
            }
            return listaPropiedades;
        }

        /// <summary>
        /// Método inverso de ObtenerPropiedadesDeTexto
        /// </summary>
        /// <param name="pPropiedades">Diccionario con propiedades</param>
        /// <returns></returns>
        public static string ObtenerTextoDePropiedades(Dictionary<string, string> pPropiedades)
        {
            string texto = "";
            foreach (string propiedadClave in pPropiedades.Keys)
            {
                if (!string.IsNullOrEmpty(texto))
                {
                    texto += "|||";
                }
                texto += propiedadClave + "=" + pPropiedades[propiedadClave];
            }
            return texto;
        }

        /// <summary>
        /// recibe la cadena del asunto de un mensaje y la limpia de RE: y Fwd:
        /// </summary>
        /// <param name="asunto">cadena de asunto del mensaje</param>
        /// <returns>cadena sin RE: ni Fwd:</returns>
        public static string LimpiarAsunto(string asunto)
        {
            string asuntoLimpio = asunto;

            if (asuntoLimpio.Contains("RE:"))
            {
                asuntoLimpio = asuntoLimpio.Replace("RE:", "");
            }
            if (asuntoLimpio.Contains("Fwd:"))
            {
                asuntoLimpio = asuntoLimpio.Replace("Fwd:", "");
            }
            return asuntoLimpio;
        }

        /// <summary>
        /// Encoding ANSI
        /// </summary>
        public static Encoding EncodingANSI = Encoding.GetEncoding("iso8859-1");

        /// <summary>
        /// Pasa una cadena de texto a UTF8.
        /// </summary>
        /// <param name="cadena">Cadena</param>
        /// <returns>cadena de texto en UTF8</returns>
        public static string PasarAUtf8(string pCadena)
        {
            return EncodingANSI.GetString(Encoding.UTF8.GetBytes(pCadena));
        }

        /// <summary>
        /// Pasa una cadena de texto a ANSI.
        /// </summary>
        /// <param name="cadena">Cadena</param>
        /// <returns>cadena de texto en UTF8</returns>
        public static string PasarAANSI(string pCadena)
        {
            return Encoding.UTF8.GetString(EncodingANSI.GetBytes(pCadena));
        }

        public static string PasarDataSetToString(DataSet pDataSet)
        {
            if (pDataSet != null)
            {
                return pDataSet.GetXml().Replace("\n", "").Replace("\r", "").Replace(">    <", "><").Replace(">  <", "><");
            }
            return string.Empty;
        }

        /// <summary>
        /// Rellena un dataset con los valores que contiene un xml
        /// </summary>
        /// <param name="pXmlDataSet">XML con los datos con los que se quiere rellenar el dataset</param>
        /// <param name="pDataSet">DataSet inicializado</param>
        public static void PasarStringToDataSet(string pXmlDataSet, DataSet pDataSet)
        {
            StringReader sr = new StringReader(pXmlDataSet);
            pDataSet.ReadXml(sr);
            sr.Close();
            sr.Dispose();
            sr = null;
        }

        /// <summary>
        /// Pasa una fecha en formato yyyymmdd hhmmss a un datetime
        /// </summary>
        /// <param name="pFecha">fecha</param>
        /// <returns></returns>
        public static DateTime PasarFechaAnioMesDiaADatetime(string pFecha)
        {
            DateTime fecha = DateTime.MinValue;

            if (pFecha != null && pFecha.Length >= 14)
            {
                int año = int.Parse(pFecha.Substring(0, 4));
                int mes = int.Parse(pFecha.Substring(4, 2));
                int dia = int.Parse(pFecha.Substring(6, 2));
                int hora = int.Parse(pFecha.Substring(9, 2));
                int min = int.Parse(pFecha.Substring(11, 2));
                int seg = int.Parse(pFecha.Substring(13, 2));
                fecha = new DateTime(año, mes, dia, hora, min, seg);
            }

            return fecha;
        }

        public static string AcortarDescripcionHtml(string pTexto, int pNumCaracteres)
        {
            string descripcionAcortada = "";
            if (!string.IsNullOrEmpty(pTexto))
            {
                string description = pTexto.Trim().Replace("<li>", "</p><p>").Replace("</li>", "</p><p>").Replace("<ul>", "</p><p>").Replace("<ol>", "</p><p>").Replace("</ul>", "</p><p>").Replace("</ol>", "</p><p>");
                //description = EliminarTablas(pTexto);

                List<string> parrafos = ObtenerParrafos(description);
                int numCaracteres = 0;
                foreach (string parrafo in parrafos)
                {
                    if (parrafo.Length > (pNumCaracteres - numCaracteres))
                    {
                        descripcionAcortada += "<p>" + AcortarTexto(parrafo, pNumCaracteres - numCaracteres) + "</p>";
                        //descripcionAcortada += "<p>" + AcortarTexto(parrafo, pNumCaracteres - numCaracteres).Replace("\n", "</p><p>").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;") + "</p>";
                        break;
                    }
                    else
                    {
                        descripcionAcortada += "<p>" + parrafo + "</p>";
                        numCaracteres = numCaracteres + parrafo.Length;
                    }
                }
            }

            return descripcionAcortada;
        }

        public static string AcortarDescripcionHtmlPorNumeroParrafos(string pTexto, int pNumParrafos)
        {
            string description = pTexto.Trim();
            //description = EliminarTablas(pTexto);

            List<string> parrafos = ObtenerParrafos(description);
            int numParrafos = 0;

            string descripcionAcortada = "";

            foreach (string parrafo in parrafos)
            {
                if (numParrafos < pNumParrafos)
                {
                    descripcionAcortada += "<p>" + parrafo + "</p>";
                    numParrafos++;
                }
                else
                {
                    break;
                }
            }

            return descripcionAcortada;
        }

        public static string AcortarTexto(string pTexto, int pNumCaracteres)
        {
            string textoAcortado = Es.Riam.Util.UtilCadenas.EliminarHtmlDeTexto(pTexto);
            if (textoAcortado.Length > pNumCaracteres)
            {
                textoAcortado = textoAcortado.Substring(0, pNumCaracteres) + " ...";
            }
            return textoAcortado;
        }
        /// <summary>
        /// Elimina los caracteres invalidos de el nombre de un archivo
        /// </summary>
        /// <param name="pNombreArchivo">Nombre del archivo</param>
        /// <returns>Devuelve el nombre del archivo sin caracteres especiales</returns>
        public static string ValidarNombreArchivo(string pNombreArchivo, bool pPermitirSeparador)
        {
            pNombreArchivo = mRegexQuitarCaracteresInvalidosNombreArchivo.Replace(pNombreArchivo, "").Replace("\"", "");

            if (!pPermitirSeparador)
            {
                pNombreArchivo = pNombreArchivo.Replace("/", "");
            }
            //if (pNombreArchivo!=null)
            //{
            //    foreach (char c in Path.GetInvalidFileNameChars())
            //    {
            //        pNombreArchivo = pNombreArchivo.Replace(c, '_');
            //    }
            //}
            return pNombreArchivo;
        }

        private static List<string> ObtenerParrafos(string pDescripcion)
        {
            List<string> parrafos = new List<string>();

            if ((pDescripcion.Contains("<p ") || pDescripcion.Contains("<p>")) && pDescripcion.Contains("</p>"))
            {
                string textoAntesParrafo = Es.Riam.Util.UtilCadenas.EliminarHtmlDeTexto(pDescripcion.Substring(0, pDescripcion.IndexOf('>') + 1));
                if (textoAntesParrafo.Length > 0)
                {
                    parrafos.Add(textoAntesParrafo);
                }

                pDescripcion = pDescripcion.Substring(pDescripcion.IndexOf('>') + 1);
                string parrafo1 = pDescripcion.Substring(0, pDescripcion.IndexOf("</p"));
                pDescripcion = pDescripcion.Substring(pDescripcion.IndexOf("</p>") + 4);

                parrafos.Add(Es.Riam.Util.UtilCadenas.EliminarHtmlDeTexto(parrafo1));

                parrafos.AddRange(ObtenerParrafos(pDescripcion.Trim()));
            }
            else if (pDescripcion != "")
            {
                parrafos.Add(Es.Riam.Util.UtilCadenas.EliminarHtmlDeTexto(pDescripcion));
            }

            return parrafos;
        }

        public static bool EsMultiIdioma(string pValor)
        {
            return (pValor.Contains("@") && ((pValor.Substring(pValor.LastIndexOf("@")).Length == 3) || (pValor.Substring(pValor.LastIndexOf("@")).Length == 6 && pValor.Contains('-') && pValor.Substring(pValor.LastIndexOf("-")).Length == 3)));
        }

        public static string AcuteToAcento(string pDescription)
        {
            foreach (string clave in CONVERSOR_ACUTE.Keys)
            {
                pDescription = pDescription.Replace(clave, CONVERSOR_ACUTE[clave]);
            }

            return pDescription;
        }

        public static string AcentoToAcute(string pDescription)
        {
            foreach (string clave in CONVERSOR_ACUTE.Keys)
            {
                pDescription = pDescription.Replace(CONVERSOR_ACUTE[clave], clave);
            }

            return pDescription;
        }

        public static string ToLowerSearchGraph(this string cadena)
        {
            if (LowerStringGraph)
            {
                return cadena.ToLower();
            }
            return cadena;
        }

        public static bool EsEnlaceSharepoint(string pUrl, string pOneDrivePermitido)
        {
            string tipoSharepoint = "riamlab.sharepoint.com";
            string tipoOneDrive = "riamlab-my.sharepoint.com";
            bool oneDrivePermitido = bool.Parse(pOneDrivePermitido);

            if (pUrl.Contains(tipoSharepoint))
            {
                return true;
            }
            else if (pUrl.Contains(tipoOneDrive))
            {
                if (oneDrivePermitido)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region HTML

        /// <summary>
        /// Elimina de una cadena HTML los videos e imágenes.
        /// </summary>
        /// <param name="pHtml">Html</param>
        /// <returns>Html sin videos ni imágenes</returns>
        public static string EliminarVideosEImagenesDeHtml(string pHtml)
        {
            try
            {
                string htmlLimpio = pHtml;
                while (htmlLimpio.Length > 0 && htmlLimpio.Contains("<img"))
                {
                    int indiceImg = htmlLimpio.IndexOf("<img");
                    int indiceCierreImg = htmlLimpio.Substring(indiceImg).IndexOf(">");
                    if (indiceCierreImg > -1)
                    {
                        htmlLimpio = htmlLimpio.Substring(0, indiceImg) + htmlLimpio.Substring(indiceImg + indiceCierreImg + 1);
                    }
                    else
                    {
                        break;
                    }
                }

                while (htmlLimpio.Length > 0 && htmlLimpio.Contains("<object"))
                {
                    int indiceObject = htmlLimpio.IndexOf("<object");
                    int indiceCierreObject = htmlLimpio.Substring(indiceObject).IndexOf("/object>");
                    if (indiceCierreObject > 0)
                    {
                        htmlLimpio = htmlLimpio.Substring(0, indiceObject) + htmlLimpio.Substring(indiceObject + indiceCierreObject + 8);
                    }
                    else
                    {
                        htmlLimpio = htmlLimpio.Substring(0, indiceObject);
                    }
                }

                while (htmlLimpio.Length > 0 && htmlLimpio.Contains("<iframe"))
                {
                    int indiceIframe = htmlLimpio.IndexOf("<iframe");
                    int indiceCierreIframe = htmlLimpio.Substring(indiceIframe).IndexOf("/iframe>");
                    if (indiceCierreIframe > 0)
                    {
                        htmlLimpio = htmlLimpio.Substring(0, indiceIframe) + htmlLimpio.Substring(indiceIframe + indiceCierreIframe + 8);
                    }
                    else
                    {
                        htmlLimpio = htmlLimpio.Substring(0, indiceIframe);
                    }
                }

                return htmlLimpio;
            }
            catch (Exception)
            {
                return pHtml;
            }
        }

        /// <summary>
        /// Elimina de una cadena HTML los enlaces.
        /// </summary>
        /// <param name="pHtml">Html</param>
        /// <returns>Html sin enlaces</returns>
        public static string EliminarEnlacesDeHtml(string pHtml)
        {
            try
            {
                string htmlLimpio = pHtml;

                while (htmlLimpio.Length > 0 && htmlLimpio.Contains("<a"))
                {
                    int indiceImg = htmlLimpio.IndexOf("<a");
                    int indiceCierreImg = htmlLimpio.Substring(indiceImg).IndexOf(">");
                    htmlLimpio = htmlLimpio.Substring(0, indiceImg) + htmlLimpio.Substring(indiceImg + indiceCierreImg + 1);
                }

                htmlLimpio = htmlLimpio.Replace("</a>", "");

                return htmlLimpio;
            }
            catch (Exception)
            {
                return pHtml;
            }
        }

        /// <summary>
        /// Extrae de una cadana Html el número de párrafos indicados.
        /// </summary>
        /// <param name="pHtml">Html</param>
        /// <param name="pNumParrafos">Número de párrafos a extraer</param>
        /// <returns>cadana Html con solo el número de párrafos indicados</returns>
        public static string ExtraerParrafosDeHtml(string pHtml, int pNumParrafos)
        {
            string parrafos = "";

            string pHtmlEdicion = pHtml.Replace("<div>", "").Replace("</div>", ""); ;

            if (!pHtmlEdicion.Contains("<p>"))
            {
                return pHtml;
            }

            try
            {
                int count = 0;
                while (count < pNumParrafos && pHtmlEdicion.Length > 0 && pHtmlEdicion.Contains("<p>"))
                {
                    int idiceP = pHtmlEdicion.IndexOf("<p>");
                    int idiceCierreP = pHtmlEdicion.IndexOf("</p>");
                    int idiceSiguienteP = pHtmlEdicion.Substring(idiceP + 3).IndexOf("<p>");

                    if (idiceSiguienteP > 0)
                    {
                        idiceSiguienteP += idiceP + 3;
                    }

                    if (idiceP > 0)
                    {
                        parrafos += pHtmlEdicion.Substring(0, idiceP);
                    }

                    if (idiceCierreP < idiceSiguienteP || idiceSiguienteP < 0)
                    {
                        parrafos += pHtmlEdicion.Substring(idiceP, idiceCierreP + 4 - idiceP);
                        pHtmlEdicion = pHtmlEdicion.Substring(idiceCierreP + 4);
                    }
                    else
                    {
                        parrafos += pHtmlEdicion.Substring(idiceP, idiceSiguienteP - idiceP) + "</p>";
                        pHtmlEdicion = pHtmlEdicion.Substring(idiceSiguienteP);
                    }

                    count++;
                }
            }
            catch (Exception)
            {
                return pHtml;
            }

            return parrafos;
        }

        /// <summary>
        /// Quita todas las etiquetas HTML de un texto
        /// </summary>
        /// <param name="pTexto">Texto a limpiar</param>
        /// <returns></returns>
        public static string EliminarHtmlDeTexto(string pTexto)
        {
            if (string.IsNullOrEmpty(pTexto))
            {
                return string.Empty;
            }
            pTexto = pTexto.Replace("&amp;", "&");
            //pTexto = UtilCadenas.HtmlDecode(pTexto);
            pTexto = mRegexQuitarHtml.Replace(pTexto, string.Empty);
            pTexto = pTexto.Replace("&nbsp;", " ");
            //pTexto = pTexto.Replace("\r", "");
            //pTexto = pTexto.Replace("\n", "");
            //pTexto = pTexto.Replace("\t", "");

            return pTexto;
        }

        /// <summary>
        /// Quita todas las etiquetas HTML de un texto y sustituirlas por espacios
        /// </summary>
        /// <param name="pTexto">Texto a limpiar</param>
        /// <returns></returns>
        public static string EliminarHtmlDeTextoPorEspacios(string pTexto)
        {
            if (string.IsNullOrEmpty(pTexto))
            {
                return string.Empty;
            }
            pTexto = pTexto.Replace("&amp;", "&");
            pTexto = UtilCadenas.HtmlDecode(pTexto);
            pTexto = mRegexQuitarHtml.Replace(pTexto, " ");
            pTexto = pTexto.Replace("\r", " ");
            pTexto = pTexto.Replace("\n", " ");
            pTexto = pTexto.Replace("\t", " ");

            while (pTexto.Contains("  "))
            {
                pTexto = pTexto.Replace("  ", " ");
            }

            return pTexto;
        }

        /// <summary>
        /// Quita todas las etiquetas HTML de un texto
        /// </summary>
        /// <param name="pTexto">Texto a limpiar</param>
        /// <returns></returns>
        public static string EliminarHtmlParaMensajes(string pTexto)
        {
            if (string.IsNullOrEmpty(pTexto))
            {
                return string.Empty;
            }

            pTexto = pTexto.Replace("<br>", "\n");
            pTexto = pTexto.Replace("<br/>", "\n");
            pTexto = pTexto.Replace("<br />", "\n");
            pTexto = pTexto.Replace("</p>", "\r");

            pTexto = pTexto.Replace("&ordf;", "ª");
            pTexto = pTexto.Replace("&ordm;", "º");
            pTexto = pTexto.Replace("&ntilde;", "ñ");
            pTexto = pTexto.Replace("&aacute;", "á");
            pTexto = pTexto.Replace("&eacute;", "é");
            pTexto = pTexto.Replace("&iacute;", "í");
            pTexto = pTexto.Replace("&oacute;", "ó");
            pTexto = pTexto.Replace("&uacute;", "ú");
            pTexto = pTexto.Replace("&Ntilde;", "Ñ");
            pTexto = pTexto.Replace("&Aacute;", "Á");
            pTexto = pTexto.Replace("&Eacute;", "É");
            pTexto = pTexto.Replace("&Iacute;", "Í");
            pTexto = pTexto.Replace("&Oacute;", "Ó");
            pTexto = pTexto.Replace("&Uacute;", "Ú");
            pTexto = pTexto.Replace("&iquest;", "¿");
            pTexto = pTexto.Replace("&amp;", "&");
            pTexto = pTexto.Replace("&auml;", "ä");
            pTexto = pTexto.Replace("&euml;", "ë");
            pTexto = pTexto.Replace("&iuml;", "ï");
            pTexto = pTexto.Replace("&ouml;", "ö");
            pTexto = pTexto.Replace("&uuml;", "ü");
            pTexto = pTexto.Replace("&acirc;", "â");
            pTexto = pTexto.Replace("&acirc;", "ê");
            pTexto = pTexto.Replace("&acirc;", "î");
            pTexto = pTexto.Replace("&ocirc;", "ô");
            pTexto = pTexto.Replace("&ucirc;", "û");
            pTexto = pTexto.Replace("&agrave;", "à");
            pTexto = pTexto.Replace("&egrave;", "è");
            pTexto = pTexto.Replace("&igrave;", "ì");
            pTexto = pTexto.Replace("&ograve;", "ò");
            pTexto = pTexto.Replace("&ugrave;", "ù");
            pTexto = pTexto.Replace("&copy;", "©");

            pTexto = pTexto.Replace("href", ">href");

            string expReg = "<a [^>]*>href=\"([^\"]*)\"[^>]*>([^<]*)</a>";
            pTexto = Regex.Replace(pTexto, @"" + expReg + "", "$2 ($1) ");

            pTexto = mRegexQuitarHtml.Replace(pTexto, string.Empty);

            return pTexto;
        }

        /// <summary>
        /// Comprueba cuál es el idioma mayoritario de un texto, si inglés o español
        /// </summary>
        /// <param name="pTexto">Texto a comprobar el idioma</param>
        /// <returns></returns>
        public static Idioma ComprobarIdiomaTexto(string pTexto)
        {
            Idioma idioma = Idioma.IdiomaInglesEspanol;

            int numeroConectoresEspanol = RegExConectoresEspanol.Matches(pTexto).Count;
            int numeroConectoresIngles = RegExConectoresIngles.Matches(pTexto).Count;

            if (numeroConectoresEspanol == 0 && numeroConectoresIngles > 0)
            {
                //El texto está escrito en inglés
                idioma = Idioma.IdiomaIngles;
            }
            else if (numeroConectoresIngles == 0 && numeroConectoresEspanol > 0)
            {
                //El texto está escrito en castellano
                idioma = Idioma.IdiomaEspanol;
            }
            else
            {
                //Compruebo cuál es el idioma mayoritario
                if (numeroConectoresIngles > numeroConectoresEspanol && numeroConectoresIngles / numeroConectoresEspanol > 3)
                {
                    //El texto contiene los dos idiomas, pero el idioma mayoritario es el inglés
                    idioma = Idioma.IdiomaIngles;
                }
                else if (numeroConectoresEspanol > numeroConectoresIngles && numeroConectoresEspanol / numeroConectoresIngles > 3)
                {
                    //El texto contiene los dos idiomas, pero el idioma mayoritario es el español
                    idioma = Idioma.IdiomaEspanol;
                }
                else
                {
                    //No podemos determinar cuál es el idioma del texto, ambos idiomas están presentes a niveles parecidos
                    idioma = Idioma.IdiomaInglesEspanol;
                }
            }

            return idioma;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la lista de caracteres no reconocidos por javascript de la manera: caracter ascii -> código hexadecimal (8232 -> \u2028)
        /// </summary>
        public static Dictionary<int, string> ListaCaracteresNoReconocidosJavascript
        {
            get
            {
                if (mListaCaracteresNoReconocidosJavascript.Count == 0)
                {
                    mListaCaracteresNoReconocidosJavascript.Add(0, "\u0000");
                    mListaCaracteresNoReconocidosJavascript.Add(173, "\u00ad");
                    mListaCaracteresNoReconocidosJavascript.Add(1536, "\u0600");
                    mListaCaracteresNoReconocidosJavascript.Add(1540, "\u0604");
                    mListaCaracteresNoReconocidosJavascript.Add(1807, "\u070f");
                    mListaCaracteresNoReconocidosJavascript.Add(6068, "\u17b4");
                    mListaCaracteresNoReconocidosJavascript.Add(6069, "\u17b5");
                    mListaCaracteresNoReconocidosJavascript.Add(8204, "\u200c");
                    mListaCaracteresNoReconocidosJavascript.Add(8207, "\u200f");
                    mListaCaracteresNoReconocidosJavascript.Add(8232, "\u2028");
                    mListaCaracteresNoReconocidosJavascript.Add(8239, "\u202f");
                    mListaCaracteresNoReconocidosJavascript.Add(8288, "\u2060");
                    mListaCaracteresNoReconocidosJavascript.Add(8303, "\u206f");
                    mListaCaracteresNoReconocidosJavascript.Add(65279, "\ufeff");
                    mListaCaracteresNoReconocidosJavascript.Add(65520, "\ufff0");
                    mListaCaracteresNoReconocidosJavascript.Add(65535, "\uffff");

                }
                return mListaCaracteresNoReconocidosJavascript;
            }
        }

        #endregion
    }

    /// <summary>
    /// Determina el idioma de un texto
    /// </summary>
    public enum Idioma
    {
        /// <summary>
        /// El texto está en español
        /// </summary>
        IdiomaEspanol = 0,

        /// <summary>
        /// El texto está en inglés
        /// </summary>
        IdiomaIngles,

        /// <summary>
        /// El texto tiene ambos idiomas, inglés y español 
        /// </summary>
        IdiomaInglesEspanol
    }

    public enum TipoDocumentoNIF
    {
        Desconocido,
        DNI,
        NIE,
        Pasaporte
    }
}
