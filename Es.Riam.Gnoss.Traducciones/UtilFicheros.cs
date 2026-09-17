using ClosedXML.Excel;
using Es.Riam.Gnoss.Util.Configuracion;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;

namespace Es.Riam.Gnoss.Traducciones
{
    public class UtilFicheros
    {
        private static readonly char[] CaracteresPeligrososFormulaExcel = { '=', '+', '-', '@', '|', '\t', '\r', '\n' };

        /// <summary>
        /// Indica si un valor empieza por un caracter que Excel/Calc puede interpretar como inicio de fórmula.
        /// </summary>
        public static bool EmpiezaPorCaracterFormulaPeligroso(string valor)
        {
            return !string.IsNullOrEmpty(valor) && Array.IndexOf(CaracteresPeligrososFormulaExcel, valor[0]) >= 0;
        }

        /// <summary>
        /// Agregamos una comilla simple a los valores que empiezan por un caracter de fórmula para evitar ataques de tipo formula injection al exportar a Excel.
        /// </summary>
        public static string SanitizarCelda(string valor)
        {
            if (EmpiezaPorCaracterFormulaPeligroso(valor))
            {
                return "'" + valor;
            }

            return valor;
        }

        public static DataSet LeerExcelDeRutaADataSet(Stream stream)
        {
			System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
			using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true,
                    }
                });
                return result;
            }
        }

        public static void ConstruirExcel(XLWorkbook mExcel, string nombreHoja, Dictionary<string, Dictionary<string, string>> mDiccionario, ConfigService configService)
        {
            List<string> mListaIdiomas = configService.ObtenerListaIdiomas();
            
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.TableName = nombreHoja;
            ds.Tables.Add(dt);
            dt.Columns.Add("Clave");
            foreach (string idioma in mListaIdiomas)
            {
                dt.Columns.Add(idioma);
            }

            List<string> listaClaves = new List<string>(mDiccionario.Keys);
            string[] listaClavesAux = listaClaves.ToArray();
            int contador = 0;

            List<object> listaAux = new List<object>();
            foreach (Dictionary<string, string> entrada in mDiccionario.Values)
            {
                listaAux.Add(SanitizarCelda(listaClavesAux[contador]));
                foreach (string idiomaDisponible in mListaIdiomas)
                {
                    if (entrada.ContainsKey(idiomaDisponible))
                    {
                        listaAux.Add(SanitizarCelda(HttpUtility.HtmlDecode(entrada[idiomaDisponible])));
                    }
                    else
                    {
                        listaAux.Add(string.Empty);
                    }
                }
                ds.Tables[nombreHoja].Rows.Add(listaAux.ToArray());
                contador++;
                listaAux.Clear();
            }

            mExcel.Worksheets.Add(ds);
        }
    }
}
