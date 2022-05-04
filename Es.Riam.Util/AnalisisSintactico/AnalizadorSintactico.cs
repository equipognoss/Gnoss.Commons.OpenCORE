using Es.Riam.Util.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Es.Riam.Util.AnalisisSintactico
{
    #region Enumeraciones para separadores

    /// <summary>
    /// Enumeración dé artículos definidos e indefinidos
    /// </summary>
    public enum ListaArticulos
    {
        /// <summary>
        /// el
        /// </summary>
        el = 0,

        /// <summary>
        /// la
        /// </summary>
        la,

        /// <summary>
        /// los
        /// </summary>
        los,

        /// <summary>
        /// las
        /// </summary>
        las,

        /// <summary>
        /// un
        /// </summary>
        un,

        /// <summary>
        /// una
        /// </summary>
        una,

        /// <summary>
        /// lo
        /// </summary>
        lo,

        /// <summary>
        /// unos
        /// </summary>
        unos,

        /// <summary>
        /// unas
        /// </summary>
        unas

    }

    /// <summary>
    /// Enumeración de conjunciones
    /// </summary>
    public enum ListaConjunciones
    {
        /// <summary>
        /// y
        /// </summary>
        y = 0,

        /// <summary>
        /// o
        /// </summary>
        o,

        /// <summary>
        /// u
        /// </summary>
        u,

        /// <summary>
        /// e
        /// </summary>
        e,

        /// <summary>
        /// ni
        /// </summary>
        ni,

        /// <summary>
        /// coma
        /// </summary>
        caracterComa,

        /// <summary>
        /// punto
        /// </summary>
        caracterDosPuntos,

        /// <summary>
        /// asi como
        /// </summary>
        asiComo,

        /// <summary>
        /// como
        /// </summary>
        como,

        /// <summary>
        /// como que
        /// </summary>
        comoQue,

        /// <summary>
        /// de modo que
        /// </summary>
        deModoQue,

        /// <summary>
        /// tal como
        /// </summary>
        talComo,

        /// <summary>
        /// que
        /// </summary>
        que,

        /// <summary>
        /// conforme
        /// </summary>
        conforme,

        /// <summary>
        /// de manera que
        /// </summary>
        deManeraQue,

    }

    /// <summary>
    /// Enumeración de preposiciones
    /// </summary>
    public enum ListaPreposiciones
    {
        /// <summary>
        /// a
        /// </summary>
        a = 0,

        /// <summary>
        /// ante
        /// </summary>
        ante,

        /// <summary>
        /// bajo
        /// </summary>
        bajo,

        /// <summary>
        /// con
        /// </summary>
        con,

        /// <summary>
        /// contra
        /// </summary>
        contra,

        /// <summary>
        /// de
        /// </summary>
        de,

        /// <summary>
        /// del
        /// </summary>
        del,

        /// <summary>
        /// desde
        /// </summary>
        desde,

        /// <summary>
        /// en
        /// </summary>
        en,

        /// <summary>
        /// entre
        /// </summary>
        entre,

        /// <summary>
        /// hacia
        /// </summary>
        hacia,

        /// <summary>
        /// hasta
        /// </summary>
        hasta,

        /// <summary>
        /// para
        /// </summary>
        para,

        /// <summary>
        /// por
        /// </summary>
        por,

        /// <summary>
        /// segun
        /// </summary>
        segun,

        /// <summary>
        /// sin
        /// </summary>
        sin,

        /// <summary>
        /// so
        /// </summary>
        so,

        /// <summary>
        /// sobre
        /// </summary>
        sobre,

        /// <summary>
        /// tras
        /// </summary>
        tras,

        /// <summary>
        /// durante
        /// </summary>
        durante,

        /// <summary>
        /// mediante
        /// </summary>
        mediante,

        /// <summary>
        /// al
        /// </summary>
        al,

        /// <summary>
        /// excepto
        /// </summary>
        excepto,

        /// <summary>
        /// salvo
        /// </summary>
        salvo
    }

    #endregion

    /// <summary>
    /// Proporciona métodos para comparar 2 frases.
    /// </summary>
    public class AnalizadorSintactico
    {
        #region Miembros

        private static List<string> mListaCONJUNCIONES = null;

        private static List<string> mListaCONJUNCIONESSeparadoras = null;

        private static List<string> mListaPRONOMBRES = null;

        private static List<string> mListaNEXOS = null;

        private static List<string> mListaADVERBIOS = null;

        private static List<string> mListaARTICULOS = null;

        private static List<string> mListaPREPOSICIONESMUYCOMUNES = null;

        private static List<string> mListaPalabrasNoRelevantes = null;

        private static List<string> mNoiseES = null;

        private static List<string> mNoiseEN = null;

        private static List<string> mListaSeparadores = null;

        private static List<string> mListaOtrosSignos = null;

        /// <summary>
        /// Lista de frases hechas
        /// </summary>
        public static string[] FRASES_HECHAS = { "de acuerdo" };

        /// <summary>
        /// Lista de separadores
        /// </summary>
        public static string[] SEPARADORES = { ","/*, "."*/, "...", ":", ";", "(", ")", "<", ">", "/", "|", " y ", " o ", " u ", " e ", $"{ConstantesDeSeparacion.SEPARACION_CONCATENADOR}", " .", ". ", " -", "- ", "[", "]", "{", "}" };

        public static string[] OTROS_SIGNOS = { "!", "¡", "¿", "?" };

        /// <summary>
        /// Lista de conjunciones.
        /// </summary>
        public static string[] CONJUNCIONES_SEPARADORAS = { " y ", " o ", " u ", " e ", " ni " };

        /// <summary>
        /// Lista de conjunciones.
        /// </summary>
        public static string[] CONJUNCIONES = { "y", "o", "u", "e", "ni" };

        /// <summary>
        /// Lista de pronombres
        /// </summary>
        public static string[] PRONOMBRES = { "mi", "mis", "tu", "tus", "su", "sus", "nuestro", "nuestros", "vuestro", "vuestros" };

        /// <summary>
        /// Lista de nexos
        /// </summary>
        public static string[] NEXOS = { "que", "cuanto", "como", "cual", "donde", "cuando", "quien", "qué", "cuánto", "cómo", "cuál", "dónde", "cuándo", "quién" };

        /// <summary>
        /// Lista de adverbios
        /// </summary>
        public static string[] ADVERBIOS = { "adonde", "aquí", "acá", "ahí", "allí", "allá", "cerca", "lejos", "actualmente", "ya", "aun", "aún", "todavía", "mientras", "tanto", "simultáneamente", "pues", "pronto", "repente", "repentinamente", "súbitamente", "rápido", "rápidamente", "velozmente", "aceleradamente", "aprisa", "deprisa", "ligero", "ligeramente", "apresuradamente", "apuradamente", "siquiera", "incluso", "inclusive", "inclusivamente", "exclusivamente", "salvo", "excepto", "sólo", "solamente", "únicamente", "absolutamente", "caro", "barato", "gratis", "económicamente", "muy", "demasiado", "tan", "cuan", "cuán", "así", "sic", "personalmente", "íntimamente", "públicamente", "admirablemente", "brillantemente", "notablemente", "originalmente", "curiosamente", "automáticamente", "mecánicamente", "técnicamente", "directamente", "indirectamente", "mentalmente", "demencialmente", "teóricamente", "consecuentemente", "sí", "claro", "junto", "encima", "arriba", "bajo", "abajo", "después", "luego", "entonces", "seguido", "seguida", "seguidamente", "próximo", "prósimamente", "posteriormente", "inmediatamente", "antes", "instantáneamente", "fugazmente", "resumidamente", "brevemente", "aveces", "raro", "raramente", "ocasionalemente", "eventualmente", "extraordinariamente", "parcialmente", "individualmente", "determinadamente", "concretamente", "tanto", "tantos", "mucho", "muchos", "bastante", "bastantes", "harto", "abundantemente", "considerablemente", "asimismo", "asina", "tal", "mejor", "peor", "bien", "buenísimo", "eminentemente", "convenientemente", "satisfactoriamente", "mal", "generosamente", "principalmente", "propiamente", "específicamente", "estrictamente", "voluntariamente", "libremente", "independientemente", "decididamente", "deliberadamente", "preferentemente", "espontáneamente", "intencionalmente", "lógicamente", "científicamente", "prácticamente", "experimentalmente", "exacto", "justo", "ya", "okey", "cierto", "ciertamente", "deveras", "efectivamente", "definitivamente", "decisivamente", "resolutivamente", "indudablemente", "indiscutiblemente", "también", "tampoco", "debajo", "dentro", "adentro", "antes", "delante", "adelante", "después", "frente", "antes", "antelación", "anticipación", "anteriormente", "anticipadamente", "previamente", "antemano", "primero", "primeramente", "inicialmente", "ayer", "anteayer", "repetidamente", "varias", "veces", "frecuentemente", "periódicamente", "comúnmente", "regular", "regularmente", "ordinariamente", "diariamente", "semanalmente", "mensualmente", "semestralmente", "anualmente", "excesivamente", "grandemente", "enormemente", "horrores", "poquito", "poco", "algo", "poca", "mismo", "igual", "per", "bis", "doble", "suma", "pésimo", "pésimamente", "malísimo", "inútilmente", "terriblemente", "perdidamente", "afortunadamente", "desgradiadamente", "fácilmente", "difícilmente", "efectivamente", "eficazmente", "eficientemente", "perfectamente", "regular", "culpabilidad", "culpablemente", "francamente", "sinceramente", "auténticamente", "abieramente", "atentamente", "cuidadosamente", "distraído", "celosamente", "seriamente", "tranquilamente", "nerviosamente", "duro", "duramente", "porqué", "porque", "asegún", "conforme", "no", "nunca", "jamás", "positivamente", "negativamente", "enrealidad", "realmente", "necesariamente", "seguro", "seguramente", "inevitablemente", "posiblemente", "probablemente", "potencialmente", "enfrente", "antaño", "atrás", "detrás", "paralelamente", "antiguamente", "hoy", "mañana", "siempre", "casi", "eternamente", "sempiternamente", "nunca", "jamás", "apenas", "generalmente", "sucesivamente", "sumamente", "máxime", "mayormente", "estadísticamente", "inversamente", "proporción", "gradualmente", "infinitamente", "indistitamente", "indefinidamente", "etcétera", "justo", "justamente", "exactamente", "invariablemente", "normalemente", "debidamente", "uniformemente", "fundamentalmente", "básicamente", "típicamente", "justamente", "injustamente", "adecuadamente", "literalmente", "sustancialmente", "esencialmente", "naturalmente", "materialmente", "frote", "fuerte", "fuertemente", "forzosamente", "recio", "bruscamente", "intensamente", "fijamente", "finamente", "firmemente", "sólidamente", "moderato", "levemente", "suavemente", "conmigo", "contra", "contigo", "relativamente", "comunalmente", "mutuamente", "supuestamente", "dizque", "quizá", "quizás", "acaso", "vez", "pronto", "ojalá", "contingentemente", "accidentalmente", "porcasualidad", "casualmente", "remotamente", "alrededor", "largamente", "dondequiera", "enmedio", "inmediatamente", "estrechamente", "ampliamente", "aparte", "separadamente", "profundamente", "formalmente", "nomás", "namás", "recién", "recientemente", "nuevamente", "día", "despacio", "lento", "lentamente", "pausadamente", "paulatinamente", "constantemente", "continuamente", "progresivamente", "temporalmente", "provisionalmente", "variablemente", "aproximadamente", "casi", "medio", "medianamente", "suficientemente", "todo", "total", "totalemente", "completamente", "enteramente", "íntegramente", "cabalmente", "espiritualmente", "sensiblemente", "vivamente", "activamente", "claro", "claramente", "avidentemente", "obviamente", "explícitamente", "expresamente", "aparentemente", "vagamente", "mero", "meramente", "simplemente", "débilmente", "alto", "bajo", "sabroso", "rico", "bonito", "lindo", "feo", "derecho", "cansado", "corriendo", "parejo", "altamente", "quedo", "radicalmente", "respectivamente", "recíprocamente", "correpondientemente", "subordinadamente", "coordinadamente", "metódicamente", "ordenadamente", "concertadamente", "junto", "conjuntamente", "sistemáticamente", "viceversa", "idénticamente", "verdaderamente", "falsamente", "correctamente", "acertadamente", "precisamente", "ahora", "detenidamente", "pronto", "extras", "menos", "plenamente", "globalmente", "nada", "puramente", "netamente", "sencillamente", "extremadamente", "especialmente", "sorprendentemente", "internamente", "externamente", "necesario", "necesarios", "necesaria", "necesarias", "demás" };

        /// <summary>
        /// Lista de articulos.
        /// </summary>
        public static string[] ARTICULOS = { "el", "la", "los", "las", "un", "una", "lo", "unos", "unas" };

        /// <summary>
        /// Lista de articulos.
        /// </summary>
        public static string[] PREPOSICIONESMUYCOMUNES = { "a", "ante", "bajo", "con", "contra", "de", "del", "desde", "en", "entre", "hacia", "hasta", "para", "por", "segun", "sin", "so", "sobre", "tras", "durante", "mediante", "al", "excepto", "salvo" };

        public static Regex RegExSiglos = new Regex(@"\bx{0,3}(i{1,3}|i[vx]|vi{0,3})\b", RegexOptions.IgnoreCase);

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public AnalizadorSintactico()
        {
        }

        #endregion

        #region Métodos generales

        #region Comparar frases

        /// <summary>
        /// Devuelve verdad si la palabra es un verbo en infinitivo.
        /// </summary>
        /// <param name="pPalabra">Palabra a comparar</param>
        /// <returns></returns>
        public static bool EsVerbo(string pPalabra)
        {
            pPalabra = pPalabra.ToLower(); // BUG559 Cuando el verbo está en mayusculas no lo procesa como frase

            // David: Ahora se tienen en cuenta también los verbos pronominales en tercera persona
            //        Y las palabras AR, ER ya no son verbos
            if ((pPalabra.EndsWith("ar") && !pPalabra.Equals("ar")) || (pPalabra.EndsWith("er") && !pPalabra.Equals("er")) || pPalabra.EndsWith("ir") || pPalabra.EndsWith("arse") || pPalabra.EndsWith("erse") || pPalabra.EndsWith("irse"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Compara dos frases.
        /// </summary>
        /// <param name="pFrase1">frase a comparar.</param>
        /// <param name="pFrase2">frase comparada.</param>
        /// <returns></returns>
        public static float CompararCadenas(List<string> pFrase1, List<string> pFrase2)
        {
            //Calculo el número de comparaciones máximas que se van ha hacer 
            int numeroComparaciones = 0;
            int numeroAciertos = 0;

            //Comparo cada cadena de la frase2 en toda la frase.
            foreach (string cadena2 in pFrase2)
            {
                //Comparo cada cadena de la frase2 en cada una de las cadenas de la frase1
                foreach (string cadena1 in pFrase1)
                {
                    int separador = 0;
                    int numeroPalabrasContenidas = 0;
                    int numeroPalabrasTotales = 0;

                    for (int indice = 0; separador < cadena1.Length; indice = separador + 1)
                    {
                        //Comparo si cada palabra de la cadena1 está contenida en la frase2.
                        numeroPalabrasTotales++;
                        separador = cadena1.IndexOf(' ', indice);

                        if (separador == -1)
                        {
                            separador = cadena1.Length;
                        }

                        if (cadena2.Contains(cadena1.Substring(indice, separador - indice)))
                        {
                            numeroPalabrasContenidas++;
                        }
                    }
                    numeroAciertos += numeroPalabrasContenidas;
                    numeroComparaciones += numeroPalabrasTotales;
                }
            }

            if (numeroComparaciones == 0)
            {
                numeroComparaciones = 1;
            }
            return (float)numeroAciertos * 100 / numeroComparaciones;
        }

        #endregion

        /// <summary>
        /// Obtiene los tags de una frase convencional
        /// </summary>
        /// <param name="pFrase">Frase de la que se desea obtener sus tags</param>
        /// <param name="pNumeroPalabrasDescartadas">Número de palabras descartadas</param>
        /// <returns>lista de tags</returns>
        public static List<string> ObtenerTagsFrase(string pFrase, out int pNumeroPalabrasDescartadas)
        {
            return ObtenerTagsFrase(pFrase, out pNumeroPalabrasDescartadas, " ", true);
        }

        /// <summary>
        /// Obtiene los tags de una frase convencional
        /// </summary>
        /// <param name="pFrase">Frase de la que se desea obtener sus tags</param>
        /// <param name="pNumeroPalabrasDescartadas">Número de palabras descartadas</param>
        /// <returns>lista de tags</returns>
        public static List<string> ObtenerTagsFrase(string pFrase, out int pNumeroPalabrasDescartadas, string pSeparador, bool pOmitirPalabrasNoRelevantesSearch)
        {
            List<string> listaTags = new List<string>();
            string[] listaSeparadores = new string[SEPARADORES.Length + 1];

            SEPARADORES.CopyTo(listaSeparadores, 0);
            listaSeparadores[SEPARADORES.Length] = pSeparador;
            pNumeroPalabrasDescartadas = 0;


            int i = 0;
            List<int> comillas = new List<int>();
            while (i > -1)
            {
                i = pFrase.IndexOf('"', i);
                if (i > -1)
                {
                    comillas.Add(i);
                    i++;
                }
            }

            List<string> frasesConComillas = new List<string>();
            int fraseActual = -1;
            string pFraseProcesada = pFrase;
            foreach (int apracionComillas in comillas)
            {
                if (fraseActual == -1)
                {
                    fraseActual = apracionComillas;
                }
                else
                {
                    string frase = pFrase.Substring(fraseActual, apracionComillas - fraseActual + 1);
                    pFraseProcesada = pFraseProcesada.Replace(frase, "");
                    frasesConComillas.Add(frase.Replace("\"", "").Trim());
                    fraseActual = -1;
                }
            }

            string[] subcadenas = pFraseProcesada.Split(listaSeparadores, StringSplitOptions.RemoveEmptyEntries);

            foreach (string palabra in subcadenas)
            {
                //Limpiamos la palabra para que no se confundan las palabras no relevantes.
                string palabraTratar = palabra;
                if (palabra.Contains("\"") || palabra.Contains("'"))
                {
                    if (palabra.IndexOf("\"") == 0 || palabra.EndsWith("\""))
                    {
                        palabraTratar = palabra.Replace("\"", "");
                    }
                    else if (palabra.IndexOf("\\'") == 0 || palabra.EndsWith("\\'"))
                    {
                        palabraTratar = palabra.Replace("\\'", "");
                    }
                    else if (palabra.IndexOf("'") == 0 || palabra.EndsWith("'"))
                    {
                        palabraTratar = palabra.Replace("'", "");
                    }
                }

                if ((!pOmitirPalabrasNoRelevantesSearch || !ListaPalabrasNoRelevantes.Contains(palabraTratar)) && (!listaTags.Contains(palabraTratar)))
                {
                    listaTags.Add(palabraTratar);
                }
                else
                {
                    pNumeroPalabrasDescartadas++;
                }
            }

            if (frasesConComillas.Count > 0)
            {
                frasesConComillas.Remove("");
                listaTags.AddRange(frasesConComillas);
            }

            //Lo quitamos porque fallan las busquedas que terminan en '.'. Ej (BuscaEmpresas s.l.)
            ////Eliminamos el punto de la ultima palabra

            return listaTags;
        }

        /// <summary>
        /// Copia un array a otro.
        /// </summary>
        /// <param name="pArray">Array a copiar</param>
        /// <param name="pIndice">Indice desde el que se comienza la copia</param>
        /// <returns></returns>
        private static object[] CopiarArray(object[] pArray, int pIndice)
        {
            object[] arrayAuxiliar = new object[100];
            pArray.CopyTo(arrayAuxiliar, pIndice);


            return arrayAuxiliar;
        }

        /// <summary>
        /// Devuelve verdad si el la cadena enviada es una preposición.
        /// </summary>
        /// <param name="pPreposicion">Preposición.</param>
        /// <returns></returns>
        public static bool CompararPreposicion(string pPreposicion)
        {
            return ListaPREPOSICIONESMUYCOMUNES.Contains(pPreposicion);
        }

        /// <summary>
        /// Devuelve verdad si la cadena enviada es una conjunción.
        /// </summary>
        /// <param name="pConjuncion">Conjunción.</param>
        /// <returns></returns>
        public static bool CompararConjunciones(string pConjuncion)
        {
            return ListaCONJUNCIONES.Contains(pConjuncion);
        }

        /// <summary>
        /// Devuelve verdad si la cadena enviada es un artículo.
        /// </summary>
        /// <param name="pArticulo">Artículo.</param>
        /// <returns></returns>
        public static bool CompararArticulo(string pArticulo)
        {
            return ListaARTICULOS.Contains(pArticulo);
        }

        /// <summary>
        /// Convierte una preposición en un string.
        /// </summary>
        /// <param name="pPreposicion">Preposición</param>
        /// <returns></returns>
        public static string PreposicionToString(ListaPreposiciones pPreposicion)
        {
            switch (pPreposicion)
            {
                case ListaPreposiciones.a:
                    return "a";
                case ListaPreposiciones.ante:
                    return "ante";
                case ListaPreposiciones.bajo:
                    return "bajo";
                case ListaPreposiciones.con:
                    return "con";
                case ListaPreposiciones.contra:
                    return "contra";
                case ListaPreposiciones.de:
                    return "de";
                case ListaPreposiciones.del:
                    return "del";
                case ListaPreposiciones.desde:
                    return "desde";
                case ListaPreposiciones.en:
                    return "en";
                case ListaPreposiciones.entre:
                    return "entre";
                case ListaPreposiciones.hacia:
                    return "hacia";
                case ListaPreposiciones.hasta:
                    return "hasta";
                case ListaPreposiciones.para:
                    return "para";
                case ListaPreposiciones.por:
                    return "por";
                case ListaPreposiciones.segun:
                    return "segun";
                case ListaPreposiciones.sin:
                    return "sin";
                case ListaPreposiciones.so:
                    return "so";
                case ListaPreposiciones.sobre:
                    return "sobre";
                case ListaPreposiciones.tras:
                    return "tras";
                case ListaPreposiciones.durante:
                    return "durante";
                case ListaPreposiciones.mediante:
                    return "mediante";
                case ListaPreposiciones.al:
                    return "al";
                case ListaPreposiciones.excepto:
                    return "excepto";
                case ListaPreposiciones.salvo:
                    return "salvo";
            }
            return "";
        }

        /// <summary>
        /// Convierte un articulo en un string.
        /// </summary>
        /// <param name="pArticulo">Articulo</param>
        /// <returns></returns>
        public static string ArticuloToString(ListaArticulos pArticulo)
        {
            switch (pArticulo)
            {
                case ListaArticulos.el:
                    return "el";
                case ListaArticulos.la:
                    return "la";
                case ListaArticulos.las:
                    return "las";
                case ListaArticulos.lo:
                    return "lo";
                case ListaArticulos.los:
                    return "los";
                case ListaArticulos.un:
                    return "un";
                case ListaArticulos.una:
                    return "una";
                case ListaArticulos.unas:
                    return "unas";
                case ListaArticulos.unos:
                    return "unos";
            }
            return "";
        }

        /// <summary>
        /// Elimina de la frase las posibles frases hechas que ésta contenga
        /// </summary>
        /// <param name="pFrase">Frase original</param>
        public static void EliminarFrasesHechas(ref string pFrase)
        {
            foreach (string fraseHecha in AnalizadorSintactico.FRASES_HECHAS)
            {
                if (pFrase.Contains(fraseHecha))
                {
                    pFrase = pFrase.Replace(fraseHecha, "");
                }
            }
        }

        /// <summary>
        /// Compara la palabra con las listas de palabras no relevantes
        /// </summary>
        /// <param name="pPalabra">palabra a comparar</param>
        /// <returns></returns>
        public static bool CompararPalabraNoRelevante(string pPalabra)
        {
            return (ListaNEXOS.Contains(pPalabra) || ListaPRONOMBRES.Contains(pPalabra) || ListaADVERBIOS.Contains(pPalabra));
        }

        /// <summary>
        /// Comprueba si la palabra es un artículo o una conjunción.
        /// </summary>
        /// <param name="pPalabra">Palabra a comprobar</param>
        /// <returns>TRUE si la palabra es un artículo o conjunción, FALSE en caso contrario</returns>
        public static bool EsArticuloOConjuncionOPreposicionesComunes(string pPalabra)
        {
            return (ListaARTICULOS.Contains(pPalabra) || ListaCONJUNCIONES.Contains(pPalabra) || ListaPREPOSICIONESMUYCOMUNES.Contains(pPalabra));
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Devuelve la lista de conjunciones, pero considerando que son separadoras (" y ", " o "...)
        /// </summary>
        private static List<string> ListaCONJUNCIONESSeparadoras
        {
            get
            {
                if (mListaCONJUNCIONESSeparadoras == null)
                {
                    mListaCONJUNCIONESSeparadoras = new List<string>(CONJUNCIONES_SEPARADORAS);
                }
                return mListaCONJUNCIONESSeparadoras;
            }
        }

        /// <summary>
        /// Devuelve la lista de conjunciones
        /// </summary>
        private static List<string> ListaCONJUNCIONES
        {
            get
            {
                if (mListaCONJUNCIONES == null)
                {
                    mListaCONJUNCIONES = new List<string>(CONJUNCIONES);
                }
                return mListaCONJUNCIONES;
            }
        }

        /// <summary>
        /// Devuelve la lista de pronombres
        /// </summary>
        private static List<string> ListaPRONOMBRES
        {
            get
            {
                if (mListaPRONOMBRES == null)
                {
                    mListaPRONOMBRES = new List<string>(PRONOMBRES);
                }
                return mListaPRONOMBRES;
            }
        }

        /// <summary>
        /// Devuelve la lista de nexos
        /// </summary>
        private static List<string> ListaNEXOS
        {
            get
            {
                if (mListaNEXOS == null)
                {
                    mListaNEXOS = new List<string>(NEXOS);
                }
                return mListaNEXOS;
            }
        }

        /// <summary>
        /// Devuelve la lista de adverbios
        /// </summary>
        private static List<string> ListaADVERBIOS
        {
            get
            {
                if (mListaADVERBIOS == null)
                {
                    mListaADVERBIOS = new List<string>(ADVERBIOS);
                }
                return mListaADVERBIOS;
            }
        }

        /// <summary>
        /// Devuelve la lista de artículos
        /// </summary>
        private static List<string> ListaARTICULOS
        {
            get
            {
                if (mListaARTICULOS == null)
                {
                    mListaARTICULOS = new List<string>(ARTICULOS);
                }
                return mListaARTICULOS;
            }
        }

        /// <summary>
        /// Devuelve la lista de preposiciones muy comunes
        /// </summary>
        private static List<string> ListaPREPOSICIONESMUYCOMUNES
        {
            get
            {
                if (mListaPREPOSICIONESMUYCOMUNES == null)
                {
                    mListaPREPOSICIONESMUYCOMUNES = new List<string>(PREPOSICIONESMUYCOMUNES);
                }
                return mListaPREPOSICIONESMUYCOMUNES;
            }
        }

        /// <summary>
        /// Devuelve la lista de palabras no relevantes
        /// </summary>
        public static List<string> ListaPalabrasNoRelevantes
        {
            get
            {
                if (mListaPalabrasNoRelevantes == null)
                {
                    mListaPalabrasNoRelevantes = new List<string>();

                    char[] sepraradores = { '\n', ' ', '\r' };

                    foreach (string palabra in Resources.noiseESN.Split(sepraradores, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!mListaPalabrasNoRelevantes.Contains(palabra))
                        {
                            mListaPalabrasNoRelevantes.Add(palabra);
                        }
                    }

                    foreach (string palabra in Resources.noiseENG.Split(sepraradores, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!mListaPalabrasNoRelevantes.Contains(palabra))
                        {
                            mListaPalabrasNoRelevantes.Add(palabra);
                        }
                    }

                    foreach (string palabra in Resources.noisePOR.Split(sepraradores, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!mListaPalabrasNoRelevantes.Contains(palabra))
                        {
                            mListaPalabrasNoRelevantes.Add(palabra);
                        }
                    }

                    foreach (string palabra in Resources.noiseEUSK.Split(sepraradores, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!mListaPalabrasNoRelevantes.Contains(palabra))
                        {
                            mListaPalabrasNoRelevantes.Add(palabra);
                        }
                    }
                }
                return mListaPalabrasNoRelevantes;
            }
        }

        /// <summary>
        /// Devuelve la lista de preposiciones muy comunes
        /// </summary>
        public static List<string> ListaSeparadores
        {
            get
            {
                if (mListaSeparadores == null)
                {
                    mListaSeparadores = new List<string>(SEPARADORES);
                }
                return mListaSeparadores;
            }
        }

        /// <summary>
        /// Devuelve la lista de preposiciones muy comunes
        /// </summary>
        public static List<string> ListaOtrosSignos
        {
            get
            {
                if (mListaOtrosSignos == null)
                {
                    mListaOtrosSignos = new List<string>(OTROS_SIGNOS);
                }
                return mListaOtrosSignos;
            }
        }

        public static List<string> NoiseEN
        {
            get
            {
                if (mNoiseEN == null)
                {
                    mNoiseEN = new List<string>();

                    StringReader stringReader = new StringReader(Resources.noiseENG);
                    string linea = stringReader.ReadLine();

                    while (!string.IsNullOrEmpty(linea))
                    {
                        mNoiseEN.Add(linea);
                        linea = stringReader.ReadLine();
                    }
                }

                return mNoiseEN;
            }
        }

        public static List<string> NoiseES
        {
            get
            {
                if (mNoiseES == null)
                {
                    mNoiseES = new List<string>();

                    StringReader stringReader = new StringReader(Resources.noiseESN);
                    string linea = stringReader.ReadLine();

                    while (!string.IsNullOrEmpty(linea))
                    {
                        mNoiseES.Add(linea);
                        linea = stringReader.ReadLine();
                    }
                }

                return mNoiseES;
            }
        }

        #endregion
    }
}
