using ClosedXML.Excel;
using ExcelDataReader;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace Es.Riam.Gnoss.Traducciones
{
    public class UtilFicheros
    {

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

        public static void ConstruirExcel(XLWorkbook mExcel, string nombreHoja, Dictionary<string, Dictionary<string, string>> mDiccionario)
        {

            List<string> mListaIdiomas =  mDiccionario.Values.Select(item => item.Keys.FirstOrDefault()).Distinct().ToList();
            
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
                listaAux.Add(listaClavesAux[contador]);                
                foreach (string idiomaDisponible in mListaIdiomas)
                {
                    if (entrada.ContainsKey(idiomaDisponible))
                    {
                        listaAux.Add(HttpUtility.HtmlDecode(entrada[idiomaDisponible]));
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

       /* private static int BuscarColumnaIdioma(string pIdioma, DataSet dsBuscar)
        {
            int numeroColumnaIdioma = 1;
            bool encontrado = false;
            while (!encontrado)
            {
                numeroColumnaIdioma++;
                if (string.IsNullOrEmpty(dsBuscar.Cells[1, numeroColumnaIdioma].Value as string) || dsBuscar.Cells[1, numeroColumnaIdioma].Value.Equals(pIdioma))
                {
                    dsBuscar.Cells[1, numeroColumnaIdioma].Value = pIdioma;
                    encontrado = true;
                }
            }
            return numeroColumnaIdioma;
        }*/
    }
}
