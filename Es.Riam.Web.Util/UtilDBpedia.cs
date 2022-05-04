using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace Es.Riam.Web.Util
{
    public class UtilDBpedia
    {
        static string mUrlServicio = "http://dbpedia.org/sparql";
        public static DataSet HacerConsulta(string pQuery, string pNombreTabla)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>();
            string respuesta = string.Empty;
            parametros.Add("default-graph-uri", "http://dbpedia.org");
            parametros.Add("query", pQuery);
            parametros.Add("format", "text/tab-separated-values");
            parametros.Add("timeout", "0");
            parametros.Add("debug", "on");
            try
            {
                respuesta = UtilHttpWeb.HacerPeticionPost(mUrlServicio, parametros);
            }
            catch(Exception)
            {
                HacerConsulta(pQuery, pNombreTabla);
            }
            return obtenerRespuestaDS(respuesta, pNombreTabla);
        }

        private static DataSet obtenerRespuestaDS(string pRespuesta, string pNombreTabla)
        {
            DataSet ds = new DataSet();
            DataTable table = new DataTable(pNombreTabla);
            //ds.Tables.Add(table);

            string[] delimiterSaltoLinea = new string[] { "\n" };
            string[] delimiterTabulacion = new string[] { "\t" };
            string[] filas = pRespuesta.Split(delimiterSaltoLinea, StringSplitOptions.RemoveEmptyEntries);
            string[] nombreColumnasTabla = null;

            int numFilas = 0;
            foreach (string fila in filas)
            {

                numFilas = numFilas + 1;
                string[] columnas = fila.Split(delimiterTabulacion, StringSplitOptions.None);
                if (columnas.Length > 3)
                {
                    //Hay tabuladores en alguno de los campos
                    Regex regex = new Regex("(\")(\t)");
                    columnas = regex.Split(fila);

                    List<string> listData = new List<string>();
                    foreach (string data in columnas)
                    {
                        if (data != "\"" && data != "\t")
                        {
                            listData.Add(data);
                        }
                    }
                    columnas = listData.ToArray();
                }

                if (numFilas == 1)
                {
                    List<string> nombreColumnas = null;
                    foreach (string columna in columnas)
                    {
                        if (nombreColumnas == null)
                        {
                            nombreColumnas = new List<string>();
                        }
                        table.Columns.Add(EliminarSeparadorFinal(EliminarSeparadorInicial(columna)));
                        nombreColumnas.Add(EliminarSeparadorFinal(EliminarSeparadorInicial(columna)));
                    }
                    nombreColumnasTabla = nombreColumnas.ToArray();
                }
                else
                {
                    DataRow dr = table.NewRow();
                    for (int i = 0; i < columnas.Length; i++)
                    {
                        dr[nombreColumnasTabla[i]] = EliminarSeparadorFinal(EliminarSeparadorInicial(columnas[i]));
                    }
                    table.Rows.Add(dr);
                }
            }
            ds.Tables.Add(table);
            return ds;
        }

        private static string EliminarSeparadorFinal(string pTexto, string pSeparador = "\"")
        {
            if (pTexto.EndsWith(pSeparador))
            {
                pTexto = pTexto.Substring(0, pTexto.Length - pSeparador.Length);
            }
            return pTexto;
        }

        private static string EliminarSeparadorInicial(string pTexto, string pSeparador = "\"")
        {
            if (pTexto.StartsWith(pSeparador))
            {
                pTexto = pTexto.Substring(pSeparador.Length);
            }
            return pTexto;
        }

        /// <summary>
        /// Limpia los carácteres extraños y pone 'and' en los espacios para realizar búsquedas de autores den DBPedia
        /// </summary>
        /// <param name="pnombreAutor">Nombre del autor con carácteres indeseados</param>
        /// <returns>Nombre limpio y separado por 'and'</returns>
        public static string quitarCaracteresNombreAutor(string pnombreAutor)
        {
            Regex regexNumbers = new Regex("[0-9]+");
            pnombreAutor = regexNumbers.Replace(pnombreAutor, " ");

            //pnombreAutor = pnombreAutor.Replace("XOX", "").Trim();
            pnombreAutor = pnombreAutor.Replace("á", "a").Trim();
            pnombreAutor = pnombreAutor.Replace("é", "e").Trim();
            pnombreAutor = pnombreAutor.Replace("í", "i").Trim();
            pnombreAutor = pnombreAutor.Replace("ó", "o").Trim();
            pnombreAutor = pnombreAutor.Replace("ú", "u").Trim();
            pnombreAutor = pnombreAutor.Replace("Á", "A").Trim();
            pnombreAutor = pnombreAutor.Replace("É", "E").Trim();
            pnombreAutor = pnombreAutor.Replace("Í", "I").Trim();
            pnombreAutor = pnombreAutor.Replace("Ó", "O").Trim();
            pnombreAutor = pnombreAutor.Replace("Ú", "U").Trim();
            pnombreAutor = pnombreAutor.Replace(".", "").Trim();
            pnombreAutor = pnombreAutor.Replace(";", " ").Trim();
            pnombreAutor = pnombreAutor.Replace("\"", "").Trim();
            pnombreAutor = pnombreAutor.Replace("ñ", "n").Trim();
            pnombreAutor = pnombreAutor.Replace(",", " ").Trim();
            pnombreAutor = pnombreAutor.Replace("Cª", "").Trim();
            pnombreAutor = pnombreAutor.Replace("&", " ").Trim();
            pnombreAutor = pnombreAutor.Replace("ü", "u").Trim();
            pnombreAutor = pnombreAutor.Replace(" -", " ").Trim();
            pnombreAutor = pnombreAutor.Replace(" / ", " ").Trim();
            pnombreAutor = pnombreAutor.Replace("´", " ").Trim();
            pnombreAutor = pnombreAutor.Replace("è", "e").Trim();
            pnombreAutor = pnombreAutor.Replace("ç", "c").Trim();
            pnombreAutor = pnombreAutor.Replace("ï", "i").Trim();
            pnombreAutor = pnombreAutor.Replace("â", "a").Trim();
            pnombreAutor = pnombreAutor.Replace("ò", "o").Trim();
            pnombreAutor = pnombreAutor.Replace("(?)", "").Trim();
            pnombreAutor = pnombreAutor.Replace("?", "").Trim();
            pnombreAutor = pnombreAutor.Replace("¿", "").Trim();
            pnombreAutor = pnombreAutor.Replace("ã", "a").Trim();
            pnombreAutor = pnombreAutor.Replace("\"", "").Trim();
            pnombreAutor = pnombreAutor.Replace("'", "").Trim();
            pnombreAutor = pnombreAutor.Replace("î", "i").Trim();
            pnombreAutor = pnombreAutor.Replace("ë", "e").Trim();
            pnombreAutor = pnombreAutor.Replace("ö", "o").Trim();
            pnombreAutor = pnombreAutor.Replace("õ", "o").Trim();
            pnombreAutor = pnombreAutor.Replace("(", "").Trim();
            pnombreAutor = pnombreAutor.Replace(")", "").Trim();
            pnombreAutor = pnombreAutor.Replace("-", " ").Trim();
            pnombreAutor = pnombreAutor.Replace("ê", "e").Trim();
            pnombreAutor = pnombreAutor.Replace("à", "a").Trim();
            pnombreAutor = pnombreAutor.Replace("Æ", "ae").Trim();
            pnombreAutor = pnombreAutor.Replace("ä", "a").Trim();
            pnombreAutor = pnombreAutor.Replace("ª", "").Trim();

            Regex regexAnds = new Regex("(  +)");
            pnombreAutor = regexAnds.Replace(pnombreAutor, " ");

            if (pnombreAutor.EndsWith(" "))
            {
                pnombreAutor = pnombreAutor.Substring(0, pnombreAutor.Length - 1);
            }

            pnombreAutor = pnombreAutor.Replace(" ", " and ");

            return pnombreAutor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAuthorName">Nombre del autor que se desea buscar (necesario llamar al método quitarCaracteresNombreAutor antes)</param>
        /// <param name="pNombreTabla">Nombre que se le da a la tabla de DS</param>
        /// <returns>DS con url, fecha y lugar de nacimiento y muerte, score (puntucación)</returns>
        public static DataSet consultaDBpediaAutor(string pAuthorName, string pNombreTabla)
        {
            DataSet ds = new DataSet();

            string query = "select distinct ?s ?artista ?birthDate ?deathDate ?placeOfBirth ?placeOfDeath ?sc where { ?s <http://www.w3.org/1999/02/22-rdf-syntax-ns#type>  ?tipo. ";
            query += "filter(?tipo in (<http://xmlns.com/foaf/0.1/Person>,dbpedia-owl:Artist)) ";
            query += "?s ?p ?o.  filter (?p in (<http://www.w3.org/2000/01/rdf-schema#label>, foaf:name)) ?o bif:contains '" + pAuthorName + "'  OPTION ( SCORE ?sc ) ";
            query += "OPTIONAL{?s dbpedia-owl:birthDate ?birthDate. ?s dbpedia-owl:deathDate  ?deathDate  .";
            query += " ?s dbpedia-owl:deathDate  ?deathDate.";
            query += "?s dbpprop:placeOfBirth  ?placeOfBirth.";
            query += "?s dbpprop:placeOfDeath  ?placeOfDeath .";
            query += "?s <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> ?artista. filter(?artista in (dbpedia-owl:Artist)) .";
            query += "} }  order by   desc (?sc)";

            return UtilDBpedia.HacerConsulta(query, pNombreTabla); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAuthorName">Nombre del autor que se desea buscarn (necesario llamar al método quitarCaracteresNombreAutor antes)</param>
        /// <param name="pNombreTabla">Nombre que se le da a la tabla de DS</param>
        /// <returns>DS con url, descripción y score (puntucación)</returns>
        public static DataSet consultaDBpediaAutorDescripcion(string pAuthorName, string pNombreTabla)
        {
            DataSet ds = new DataSet();

            string query = "select distinct ?s ?o2 ?sc where { ?s <http://www.w3.org/1999/02/22-rdf-syntax-ns#type>  ?tipo. ";
            query += " filter(?tipo in (<http://xmlns.com/foaf/0.1/Person>,dbpedia-owl:Artist))  ";
            query += "?s ?p ?o.  filter (?p in (<http://www.w3.org/2000/01/rdf-schema#label>, foaf:name)) ?o bif:contains '" + pAuthorName + "'  OPTION ( SCORE ?sc ) ";
            query += " OPTIONAL{{?s ?p2 ?o2. filter(?p2 in (dbpedia-owl:abstract)) filter(LANG(?o2)='es')} }  }";
            query += " order by   desc (?sc)";
            
            return UtilDBpedia.HacerConsulta(query, pNombreTabla);
        }

        public static Dictionary<string, List<string>> obtenerInformacionPorPredicado(string pDbpediaId)
        {
            string pNombreTabla = "Tabla";
            DataSet ds = consultaDBpedia(pDbpediaId, pNombreTabla);
            Dictionary<string, List<string>> dicPredicados = null;
            foreach (DataRow dr in ds.Tables[pNombreTabla].Rows)
            {
                if (dicPredicados == null)
                {
                    dicPredicados = new Dictionary<string, List<string>>();
                }
                if (!dicPredicados.ContainsKey(dr["p"].ToString().Trim()))
                {
                    dicPredicados.Add(dr["p"].ToString().Trim(), new List<string>() { dr["o"].ToString().Trim() });
                }
                else
                {
                    dicPredicados[dr["p"].ToString().Trim()].Add(dr["o"].ToString().Trim());
                }
            }
            return dicPredicados;
        }

        /// <summary>
        /// Devuelve todos los datos de un sujeto determinado
        /// </summary>
        /// <param name="pDBpediaId">Sujeto de dbpedia (Id de lo que se quiere buscar)</param>
        /// <param name="pNombreTabla">Nombre de la tabla donde insertar los datos.</param>
        /// <returns></returns>
        private static DataSet consultaDBpedia(string pDBpediaId, string pNombreTabla)
        {
            string query = "select * where {\n";
            query += "filter(?s=<" + pDBpediaId + ">)\n";
            query += "{?s ?p ?o.\n";
            query += "filter(?p in (dbpedia-owl:nationality,dbpedia-owl:wikiPageExternalLink,dbpedia-owl:education, dbpedia-owl:field,dbpedia-owl:movement, dcterms:subject, foaf:img, foaf:depiction,dbpedia-owl:wikiPageRedirects, dbpedia-owl:birthDate, dbpedia-owl:deathDate,dbpprop:placeOfBirth,dbpprop:placeOfDeath,foaf:name,foaf:surname,dbpprop:alterativeNames,dbpprop:occupation))\n";
            query += "}\n";

            query += "UNION\n";
            query += "{?s ?p ?o. filter(?p in (dbpprop:shortDescription, rdfs:comment, dbpedia-owl:abstract)) filter(LANG(?o)='es')}\n";

            query += "UNION\n";
            query += "{?o ?p ?s.filter(?p in (dbpedia-owl:wikiPageRedirects))}";
            query += "}";
            return UtilDBpedia.HacerConsulta(query, pNombreTabla);
        }
    }
}
