using Es.Riam.Util;
using Es.Riam.Util.AnalisisSintactico;
using System.Text.RegularExpressions;
using System.Web;

namespace Gnoss.Web.LabelerService
{
    public class UtilLabelerService
    {
        
        public string ObtenerEtiquetasDeTituloYDescripcion(string titulo, string descripcion, out string pEtiquetasPropuestas)
        {
            int numMaxTagsPropuestos = 20;
            int numMaxTagsDevueltos = 10;

            string resultados = "";
            pEtiquetasPropuestas = "";
            string comaResultados = "";
            string comaResultadosPropuestos = "";

            List<string> listaTagsDevuletos = new List<string>();
            List<string> listaTagsPropuestos = new List<string>();

            //Obtenemos los tags del título
            List<string> listaTagsTitulo = ObtenerTagsNegritaSubrayadosEnlaces(titulo, true);
            ObtenerTagsPropuestosYDevueltos(ref listaTagsPropuestos, ref listaTagsDevuletos, listaTagsTitulo, numMaxTagsPropuestos, numMaxTagsDevueltos);

            if (listaTagsDevuletos.Count < numMaxTagsDevueltos || listaTagsPropuestos.Count < numMaxTagsPropuestos)
            {
                //Obtenemos los tags de la descripción
                List<string> listaTagsDescripcion = ObtenerTagsNegritaSubrayadosEnlaces(descripcion, false);
                ObtenerTagsPropuestosYDevueltos(ref listaTagsPropuestos, ref listaTagsDevuletos, listaTagsDescripcion, numMaxTagsPropuestos, numMaxTagsDevueltos);
            }

            if (listaTagsDevuletos.Count < numMaxTagsDevueltos || listaTagsPropuestos.Count < numMaxTagsPropuestos)
            {
                Dictionary<string, double> listaTagsDbpedia = new Dictionary<string, double>();
                foreach (string tagActual in listaTagsDbpedia.Keys)
                {
                    string tag = tagActual;

                    bool propuesto = false;

                    double score = listaTagsDbpedia[tag];

                    double corteScorePropuesto = 0.1;
                    double corteScore = ObtenerCorteScoreSegunLongitudTexto(descripcion.Length, out corteScorePropuesto);

                    if (score > corteScorePropuesto && score < corteScore)
                    {
                        //Lo proponemos, no tiene score suficiente como para suponer que vaya a ser un tag bueno
                        propuesto = true;
                    }
                    else if (score <= corteScorePropuesto)
                    {
                        // Si este tag tiene menos score de 0.1, el resto ya no nos interesan
                        break;
                    }

                    tag = tag.ToLower();
                    if (!string.IsNullOrEmpty(tag) && !AnalizadorSintactico.NoiseES.Contains(tag) && !AnalizadorSintactico.NoiseEN.Contains(tag) && !AnalizadorSintactico.ListaSeparadores.Contains(tag) && !AnalizadorSintactico.ListaOtrosSignos.Contains(tag))
                    {
                        if (!listaTagsDevuletos.Contains(tag) && listaTagsDevuletos.Count < 10 && !propuesto)
                        {

                            listaTagsDevuletos.Add(tag);

                            if (listaTagsPropuestos.Contains(tag))
                            {
                                //Si lo hemos detectado como tag directo, lo quito de propuesto
                                listaTagsPropuestos.Remove(tag);
                            }
                        }
                        else if (!listaTagsPropuestos.Contains(tag) && listaTagsPropuestos.Count < 20)
                        {
                            if (!listaTagsDevuletos.Contains(tag))
                            {
                                listaTagsPropuestos.Add(tag);
                            }
                        }
                        else
                        {
                            //Salimos del bucle, ya hemos cogido todos los tags directos y propuestos que nos interesaban
                            break;
                        }
                    }
                }
            }

            foreach (string tag in listaTagsDevuletos)
            {
                resultados += comaResultados + tag;
                comaResultados = ", ";
            }

            foreach (string tag in listaTagsPropuestos)
            {
                pEtiquetasPropuestas += comaResultadosPropuestos + tag;
                comaResultadosPropuestos = ", ";
            }

            return resultados;
        }
        private double ObtenerCorteScoreSegunLongitudTexto(int pLongitud, out double corteScorePropuesto)
        {
            double peso = 8;
            corteScorePropuesto = 4;

            if (pLongitud > 500 && pLongitud < 600)
            {
                peso = 6;
                corteScorePropuesto = 3;
            }
            else if (pLongitud > 600 && pLongitud < 700)
            {
                peso = 5.5;
                corteScorePropuesto = 2;
            }
            else if (pLongitud > 600 && pLongitud < 700)
            {
                peso = 5;
                corteScorePropuesto = 1;
            }
            else if (pLongitud > 700 && pLongitud < 800)
            {
                peso = 4.5;
                corteScorePropuesto = 0.8;
            }
            else if (pLongitud > 800 && pLongitud < 900)
            {
                peso = 4;
                corteScorePropuesto = 0.6;
            }
            else if (pLongitud > 900 && pLongitud < 1000)
            {
                peso = 3.5;
                corteScorePropuesto = 0.4;
            }
            else if (pLongitud > 1000)
            {
                peso = 3;
                corteScorePropuesto = 0.1;
            }

            return peso;
        }
        private void ObtenerTagsPropuestosYDevueltos(ref List<string> pListaTagsPropuestos, ref List<string> pListaTagsDevuletos, List<string> plistaTagsAnalizar, int pNumMaxPropuestos, int pNumMaxDevueltos)
        {
            foreach (string tagAnalizar in plistaTagsAnalizar)
            {
                string tag = tagAnalizar.ToLower();

                if (!string.IsNullOrEmpty(tag) && !AnalizadorSintactico.NoiseES.Contains(tag) && !AnalizadorSintactico.NoiseEN.Contains(tag) && !AnalizadorSintactico.ListaSeparadores.Contains(tag) && !AnalizadorSintactico.ListaOtrosSignos.Contains(tag))
                {
                    if (!pListaTagsDevuletos.Contains(tag) && pListaTagsDevuletos.Count < pNumMaxDevueltos)
                    {

                        pListaTagsDevuletos.Add(tag);

                        if (pListaTagsPropuestos.Contains(tag))
                        {
                            //Si lo hemos detectado como tag directo, lo quito de propuesto
                            pListaTagsPropuestos.Remove(tag);
                        }
                    }
                    else if (!pListaTagsPropuestos.Contains(tag) && pListaTagsPropuestos.Count < pNumMaxPropuestos)
                    {
                        if (!pListaTagsDevuletos.Contains(tag))
                        {
                            pListaTagsPropuestos.Add(tag);
                        }
                    }
                    else
                    {
                        //Salimos del bucle, ya hemos cogido todos los tags directos y propuestos que nos interesaban
                        break;
                    }
                }
            }
        }
        public List<string> ObtenerTagsNegritaSubrayadosEnlaces(string descripcion, bool estitulo)
        {
            List<string> listaTags = new List<string>();
            string[] frases;

            descripcion = descripcion.Replace("&amp;", "&");

            //Obtenemos los tags en cursiva y negrita
            char salto = (char)13;
            char salto1 = (char)10;
            string descripcionSinPuntos = descripcion.Replace(". ", " ");
            descripcionSinPuntos = descripcionSinPuntos.Replace(" - ", " ");

            // Tratamiento palabras en Comillas
            BuscarPalabrasResaltadas(UtilCadenas.EliminarHtmlDeTexto(descripcionSinPuntos), "\"", "\"", listaTags);

            // Tratamiento palabras como vinculo
            BuscarPalabrasResaltadas(descripcionSinPuntos, "<a ", "</a>", listaTags);

            // Tratamiento palabras en Itálica
            BuscarPalabrasResaltadas(descripcionSinPuntos, "<em", "</em>", listaTags);

            // Tratamiento palabras en Negrita
            BuscarPalabrasResaltadas(descripcionSinPuntos, "<strong", "</strong>", listaTags);

            // Tratamiento palabras subrayadas
            BuscarPalabrasResaltadas(descripcionSinPuntos, "<u", "</u>", listaTags);
            //obtenemos el resto de tags

            char[] separadoresfrases = { salto, salto1, ':', '?', '¿', '!', '¡', '.' };

            descripcion = limpiarTexto(descripcion.Replace(" - ", " "));
            frases = descripcion.Split(separadoresfrases, StringSplitOptions.RemoveEmptyEntries);

            foreach (string frase in frases)
            {

                string[] palabras0;
                List<string> palabras1 = new List<string>();
                List<string> palabras2 = new List<string>();
                string descAux = limpiarTexto(frase.Trim() + " palabrafinal");
                descAux = descAux.Replace("¡", "").Replace("!", "").Replace("¿", "").Replace("?", "").Replace(":", "");

                char[] separadores = { ' ', (char)160 };
                //string[] separadores = new string[AnalizadorSintactico.SEPARADORES.Length + 2];
                //AnalizadorSintactico.SEPARADORES.CopyTo(separadores, 0);
                ////añado los separadores de espacio
                //separadores[AnalizadorSintactico.SEPARADORES.Length] = " ";
                //separadores[AnalizadorSintactico.SEPARADORES.Length + 1] = ((char)160).ToString();

                palabras0 = descAux.Split(separadores, StringSplitOptions.RemoveEmptyEntries);

                //llevamos las palabras del array a la lista
                for (int x = 0; x < palabras0.Length; x++)
                {
                    palabras1.Add(palabras0[x]);
                }

                //Obtenemos una lista con las palabras:

                for (int i = 0; i < palabras1.Count; i++)
                {
                    string palabra = palabras1[i].ToLower();

                    string palabraanterior = "NOEXISTE";

                    if (i > 0)
                    {
                        palabraanterior = palabras1[i - 1];
                    }
                    //Si es nombre propio, se añade a la lista 2 de palabras, para que la siguiente palabra
                    // en caso de ser un apellido se una.

                    bool esarticulopalabrainicial = false;

                    if (i == 0)
                    {
                        string palabrainicial = palabras1[i].ToLower();
                        int desechados = 0;
                        List<string> tagsComprobar = AnalizadorSintactico.ObtenerTagsFrase(palabrainicial, out desechados);
                        if (tagsComprobar.Count == 0)
                        {
                            esarticulopalabrainicial = true;
                        }
                    }

                    if (i == 0 && !estitulo)
                    {
                        string palabrainicial = palabras1[i].ToLower();

                        if (!esarticulopalabrainicial)
                        {
                            if (esNomPropio(palabrainicial))
                            {
                                if (estitulo || (palabras1.Count > 0))
                                { palabras2.Add(palabras1[i]); }

                                if (palabras1.Count > 0 && !(esNomPropio(palabras1[1])))
                                { listaTags.Add(palabrainicial); }

                            }
                            else
                            { if (estitulo) { listaTags.Add(palabrainicial); } }
                        }
                    }
                    else if (!esarticulopalabrainicial && (esNomPropio(palabras1[i]) || (palabra.Equals("of") && esNomPropio(palabraanterior)) || (palabra.Equals("de") && esNomPropio(palabraanterior)) || (palabra.Equals("del") && esNomPropio(palabraanterior)) || (palabra.Equals("la") && palabraanterior.Equals("de")) || (palabra.Equals("los") && palabraanterior.Equals("de")) || (palabra.Equals("the") && palabraanterior.Equals("of")) || (palabra.Equals("lo") && palabraanterior.Equals("de")) || (palabra.Equals("las") && palabraanterior.Equals("de")))) //&& !palabra.Contains(".") && !palabra.Contains(",") && !palabra.Contains(":")
                    {

                        palabras2.Add(palabras1[i]);
                    }
                    else if (!esarticulopalabrainicial) //Si no es propio, si hay palabras en palabras2, las unimos y las ponemos como un tag y lo llevamos a listaTags
                    {
                        bool añadoTag = false;

                        //Si hay palabras en palabras 2
                        if (palabras2.Count > 0)
                        {
                            string tag = "";
                            if ((esNomPropio(palabras2[0])))
                            {
                                tag += (" " + palabras2[0]);
                            }
                            for (int j = 1; j < palabras2.Count - 1; j++)
                            {
                                if ((esNomPropio(palabras2[j])) || (esNomPropio(palabras2[j - 1]) || palabras2[j - 1].Equals("de") || palabras2[j - 1].Equals("of")))
                                {
                                    tag += (" " + palabras2[j]);

                                }
                            }
                            if ((palabras2.Count - 1 != 0) && (esNomPropio(palabras2[palabras2.Count - 1])))
                            {
                                tag += (" " + palabras2[palabras2.Count - 1]);
                            }

                            if (estitulo && (palabras2[palabras2.Count - 1].Equals("de") || palabras2[palabras2.Count - 1].Equals("of") || palabras2[palabras2.Count - 1].Equals("la") || palabras2[palabras2.Count - 1].Equals("lo") || palabras2[palabras2.Count - 1].Equals("los") || palabras2[palabras2.Count - 1].Equals("las") || palabras2[palabras2.Count - 1].Equals("the")) && !palabra.Equals("the") && !palabra.Equals("la") && !palabra.Equals("los") && !palabra.Equals("las") && !palabra.Equals("lo"))
                            {
                                if (!listaTags.Contains(palabra))
                                    listaTags.Add(palabra);
                            }

                            //coger segunda palabra del titulo cuando no es mayúscula
                            if (i == 1 && estitulo && !esNomPropio(palabra))
                            {
                                if (!listaTags.Contains(palabra))
                                    listaTags.Add(palabra);
                            }

                            //Aunque sea la primera palabra si esta en el diccionario no se elimina
                            //Quitamos el primer espacio del tag
                            if (!String.IsNullOrEmpty(tag))
                                tag = tag.Substring(1);
                            //if (!dic.Keys.Contains(tag) && tag.Contains("."))
                            //{ tag = tag.Replace(palabras2[0], ""); }

                            tag = tag.Trim();
                            //  tag = tag.Replace(".", "");

                            if (tag.Length > 2 && (tag.Substring(tag.Length - 3).Equals(" of") || tag.Substring(tag.Length - 3).Equals(" de")))

                            { tag = tag.Replace(" of", "").Replace(" de", ""); }
                            //vaciamos la lista
                            palabras2.Clear();
                            int desechados = 0;
                            if (!tag.Contains(" "))
                            {
                                tag = tag.ToLower();
                                List<string> tagsComprobar = AnalizadorSintactico.ObtenerTagsFrase(tag, out desechados);
                                if (tagsComprobar.Count > 0)
                                {
                                    if (!listaTags.Contains(tag))
                                    {
                                        if (tag.Substring(tag.Length - 1).Equals(".") || tag.Substring(tag.Length - 1).Equals(";") || tag.Substring(tag.Length - 1).Equals(":") || tag.Substring(tag.Length - 1).Equals(",") || tag.Substring(tag.Length - 1).Equals(" "))//comprobamos que no tenga un separador al final
                                        {
                                            tag = tag.Substring(0, tag.Length - 1);
                                        }
                                        int n;
                                        if (!int.TryParse(tag, out n))
                                        {
                                            listaTags.Add(tag);
                                            añadoTag = true;
                                        }
                                    }
                                }
                            }
                            //Llevamos el nuevo tag a la listaTags
                            if (!tag.Equals("") && (tag.Substring(tag.Length - 1).Equals(".") || tag.Substring(tag.Length - 1).Equals(";") || tag.Substring(tag.Length - 1).Equals(":") || tag.Substring(tag.Length - 1).Equals(",") || tag.Substring(tag.Length - 1).Equals(" ")))//comprobamos que no tenga un separador al final
                            {
                                tag = tag.Substring(0, tag.Length - 1);
                            }
                            else if (!listaTags.Contains(tag))
                            {
                                int n;
                                if (!int.TryParse(tag, out n))
                                {

                                    listaTags.Add(tag);
                                    añadoTag = true;
                                }
                            }
                        }
                        if (estitulo && !añadoTag)
                        {
                            //Caso en el que no hay ninguna palabra añadida en palabras2 (palabras con una mayuscula...
                            string tag = palabras1[i];
                            //vaciamos la lista
                            palabras2.Clear();

                            int desechados = 0;
                            List<string> tagsComprobar = AnalizadorSintactico.ObtenerTagsFrase(tag, out desechados);
                            if (tagsComprobar.Count > 0)
                            {
                                if (!listaTags.Contains(tag.Replace(".", "")))
                                {
                                    if (tag.Substring(tag.Length - 1).Equals(".") || tag.Substring(tag.Length - 1).Equals(";") || tag.Substring(tag.Length - 1).Equals(":") || tag.Substring(tag.Length - 1).Equals(",") || tag.Substring(tag.Length - 1).Equals(" "))//comprobamos que no tenga un separador al final
                                    {
                                        tag = tag.Substring(0, tag.Length - 1);
                                    }
                                    listaTags.Add(tag.Replace(".", ""));
                                }
                            }
                        }
                    }
                }
            }

            while (listaTags.Contains("palabrafinal"))
            {
                listaTags.Remove("palabrafinal");
            }

            //}
            return listaTags;
        }

        /// <summary>
        /// Busca las palabras resaltadas dentro de un texto (entre comillas, enlaces, negritas...)
        /// </summary>
        /// <param name="pCadena">Cadena a revisar</param>
        /// <param name="pInicioResalto">Inicio del resalto ({a, \"...)</param>
        /// <param name="pFinResalto">Fin del resalto</param>
        /// <param name="pListaTags">Lista de tags</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        private void BuscarPalabrasResaltadas(string pCadena, string pInicioResalto, string pFinResalto, List<string> pListaTags)
        {
            pCadena = pCadena.Trim();
            if (pCadena.Contains(pInicioResalto) && pCadena.Contains(pFinResalto))
            {
                string[] separador = { pInicioResalto };
                string[] enlaces = pCadena.Split(separador, StringSplitOptions.RemoveEmptyEntries);
                //Empiezo en el segundo item, ya que el primero es lo que se queda a la izquierda del primer <a
                int inicio = 1;
                if (pCadena.TrimStart().StartsWith(pInicioResalto))
                {
                    inicio = 0;
                }

                int salto = 2;
                if (pInicioResalto.StartsWith("<"))
                {
                    salto = 1;
                }

                for (int j = inicio; j < enlaces.Length; j += salto)
                {
                    string tag = enlaces[j];
                    int inicioTag = 0, finTag = tag.Length;

                    if (pInicioResalto.StartsWith("<"))
                    {
                        inicioTag = tag.IndexOf('>') + 1;
                        finTag = tag.IndexOf('<') - tag.IndexOf('>') - 1;

                        if (!tag.Contains(pFinResalto))
                        {
                            finTag = 0;
                        }
                    }

                    tag = tag.Substring(inicioTag, finTag);
                    if (!string.IsNullOrEmpty(tag))
                    {
                        tag = tag.Replace("(", "");
                        tag = tag.Replace(")", "");
                        tag = tag.Trim();
                        //if (tag.Substring(tag.Length - 1).Equals(" "))//comprobamos que no tenga un espacio al final
                        //{
                        //    tag = tag.Substring(0, tag.Length - 1);
                        //}
                        if (!string.IsNullOrEmpty(tag))
                        {

                            while (tag.Length > 0 && (tag.Substring(tag.Length - 1).Equals(".") || tag.Substring(tag.Length - 1).Equals(";") || tag.Substring(tag.Length - 1).Equals(":") || tag.Substring(tag.Length - 1).Equals(",") || tag.Substring(tag.Length - 1).Equals(" ") || tag.Substring(tag.Length - 1).Equals("-")))//comprobamos que no tenga un separador al final
                            {
                                tag = tag.Substring(0, tag.Length - 1);
                            }
                            if (!string.IsNullOrEmpty(tag))
                            {
                                tag = tag.ToLower();
                                if (!pListaTags.Contains(tag) && (!tieneMasDe4Palabras(tag)))//comprobamos que no sea un dominio
                                {
                                    //Si tiene más de 4 palabras, no lo tenemos en cuenta
                                    pListaTags.Add(tag);
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool tieneMasDe4Palabras(string frase)
        {
            frase = frase.Trim('\"');
            int desechados = 0;
            List<string> tagsComprobar = AnalizadorSintactico.ObtenerTagsFrase(frase, out desechados);
            bool tieneMasDe4Palabras = tagsComprobar.Count > 3;

            return tieneMasDe4Palabras;
        }

        public string limpiarTexto(string texto)
        {
            string descAux = texto;

            descAux = descAux.Replace("&amp;", "&");
            descAux = descAux.Replace("&ordf;", "ª");
            descAux = descAux.Replace("&ordm;", "º");
            descAux = descAux.Replace("&ntilde;", "ñ");
            descAux = descAux.Replace("&aacute;", "á");
            descAux = descAux.Replace("&eacute;", "é");
            descAux = descAux.Replace("&iacute;", "í");
            descAux = descAux.Replace("&oacute;", "ó");
            descAux = descAux.Replace("&uacute;", "ú");
            descAux = descAux.Replace("&Ntilde;", "Ñ");
            descAux = descAux.Replace("&Aacute;", "Á");
            descAux = descAux.Replace("&Eacute;", "É");
            descAux = descAux.Replace("&Iacute;", "Í");
            descAux = descAux.Replace("&Oacute;", "Ó");
            descAux = descAux.Replace("&Uacute;", "Ú");
            descAux = descAux.Replace("&iquest;", "¿");
            descAux = descAux.Replace("&quot;", "");
            descAux = descAux.Replace("&ldquo;", "");
            descAux = descAux.Replace("&rdquo;", "");
            descAux = descAux.Replace("&auml;", "ä");
            descAux = descAux.Replace("&euml;", "ë");
            descAux = descAux.Replace("&iuml;", "ï");
            descAux = descAux.Replace("&ouml;", "ö");
            descAux = descAux.Replace("&uuml;", "ü");
            descAux = descAux.Replace("&acirc;", "â");
            descAux = descAux.Replace("&acirc;", "ê");
            descAux = descAux.Replace("&acirc;", "î");
            descAux = descAux.Replace("&ocirc;", "ô");
            descAux = descAux.Replace("&ucirc;", "û");
            descAux = descAux.Replace("&agrave;", "à");
            descAux = descAux.Replace("&egrave;", "è");
            descAux = descAux.Replace("&igrave;", "ì");
            descAux = descAux.Replace("&ograve;", "ò");
            descAux = descAux.Replace("&ugrave;", "ù");
            descAux = descAux.Replace("&copy;", "©");
            descAux = descAux.Replace("<div", "\n <div");
            descAux = descAux.Replace("</div>", "</div> \n");
            descAux = descAux.Replace("<p", "\n <p");
            descAux = descAux.Replace("</p>", "</p> \n");
            descAux = descAux.Replace("<li", "\n <li");
            descAux = descAux.Replace("</li>", "</li> \n");
            descAux = descAux.Replace("<br", "\n <br");
            descAux = descAux.Replace("<label", "\n <label");
            descAux = descAux.Replace("</label>", "</label> \n");

            Regex mRegexQuitarHtml = new Regex(@"<(.|\n)*?>", RegexOptions.Compiled);

            descAux = mRegexQuitarHtml.Replace(HttpUtility.HtmlDecode(descAux), string.Empty);
            descAux = descAux.Replace("\"", "");
            descAux = descAux.Replace(";", " ;");
            descAux = descAux.Replace(":", " :");
            descAux = descAux.Replace(".", " .");
            descAux = descAux.Replace(",", " ,");
            descAux = descAux.Replace("(", ", ");
            descAux = descAux.Replace(")", " ,");
            descAux = descAux.Replace("[", ", ");
            descAux = descAux.Replace("]", " ,");
            descAux = descAux.Trim();

            return descAux;
        }

        public bool esNomPropio(string p)
        {
            return esNomPropio(p, null);

        }
        public bool esNomPropio(string p, string panterior)
        {
            bool esalfabetico = false;
            int i = 0;
            char c = ' ';
            while (i < p.Length && !esalfabetico)
            {
                c = (char)p[i];
                esalfabetico = Char.IsLetter(c);
                i++;
            }
            if (!esalfabetico) { return false; }

            else
            {
                string palabra = c.ToString();

                return palabra.ToUpper().Equals(palabra);
            }
        }
    }
}