using ClosedXML.Excel;
using Es.Riam.Gnoss.Util.Configuracion;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

//hecho

namespace Es.Riam.Gnoss.Traducciones
{
    public class TraduccionJavaScript
    {
        //hecho
        public void JavascriptToExcel(XLWorkbook mExcel, Dictionary<string, Stream> listaFicheros, ConfigService configService)
        {
            string nombreHoja = "JavaScript";
            Dictionary<string, Dictionary<string, string>> mDiccionario = new Dictionary<string, Dictionary<string, string>>();

            foreach (string idioma in listaFicheros.Keys)
            {
                StreamReader sr = new StreamReader(listaFicheros[idioma]);

                string line;
                string clave1 = "";
                string clave2 = "";
                string texto = "";

                while ((line = sr.ReadLine()) != null)
                {
                    if (line.IndexOf("var") != -1 && line.IndexOf("=") != -1)
                    {
                        clave1 = line.Substring(line.IndexOf("var") + 4, (line.IndexOf("=") - 5));
                    }
                    else if (line.IndexOf(":") != -1)
                    {
                        string[] auxLine = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        clave2 = auxLine[0].Trim();
                        texto = auxLine[1].Trim(' ', ',').Trim('\'');
                        string claveCompleta = clave1 + "." + clave2;
                        if (!mDiccionario.ContainsKey(claveCompleta))
                        {
                            mDiccionario.Add(claveCompleta, new Dictionary<string, string>());
                        }
                        if (!mDiccionario[claveCompleta].ContainsKey(idioma))
                        {
                            mDiccionario[claveCompleta].Add(idioma, texto);
                        }
                    }
                }
            }

            UtilFicheros.ConstruirExcel(mExcel, nombreHoja, mDiccionario, configService);
        }

        public void ExcelToJavascript(DataTable table, Dictionary<string, MemoryStream> listMStream)
        {
            List<string> mListaClaves = new List<string>();
            Dictionary<string, StringBuilder> listaFicheros = new Dictionary<string, StringBuilder>();

            //Añade las columnas de los idiomas
            for (int i = 1; i < table.Columns.Count; i++)
            {
                listaFicheros.Add(table.Columns[i].ColumnName, new StringBuilder());
            }

            //Guarda en un Dictionary<string, Stringbuilder> en la string la clave idioma y el Stringbuilder el texto de la columna por cada idioma
            foreach (DataRow fila in table.Rows)
            {
                string claveFila = fila[0].ToString();

                if (claveFila.Contains("."))
                {
                    string[] auxLine = claveFila.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    string clave1 = auxLine[0].Trim();
                    string clave2 = auxLine[1].Trim();

                    for (int i = 1; i < table.Columns.Count ; i++)
                    {
                        string idioma = table.Columns[i].ColumnName;

                        if (!mListaClaves.Contains(clave1))
                        {
                            if (!String.IsNullOrEmpty(listaFicheros[idioma].ToString()))
                            {
                                listaFicheros[idioma].Remove(listaFicheros[idioma].Length - 3, 3);
                                listaFicheros[idioma].AppendLine();
                                listaFicheros[idioma].AppendLine("};");
                            }

                            listaFicheros[idioma].Append($"var {clave1} = {{ ");
                            listaFicheros[idioma].AppendLine();
                        }

                        listaFicheros[idioma].AppendLine($"\t{clave2}: '{fila[idioma].ToString()}',");
                        mListaClaves.Add(clave2);
                    }
                    mListaClaves.Add(clave1);
                }
            }

            Dictionary<string, MemoryStream> listaFicherosIdiomas = new Dictionary<string, MemoryStream>();

            //Escribir JS
            foreach (string idioma in listaFicheros.Keys)
            {
                StringBuilder sb = listaFicheros[idioma];
                string nombreArchivo = idioma+"_JavaScript.js";

                MemoryStream st = new MemoryStream();
                StreamWriter writer = new StreamWriter(st);
                writer.Write(sb.ToString());
                writer.Flush();
                st.Position = 0;

                listMStream.Add(nombreArchivo, st);
            }
        }
    }
}