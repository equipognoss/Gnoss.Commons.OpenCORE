using ExcelDataReader;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Es.Riam.Gnoss.Traducciones
{
    public class UtilFicheros
    {

        public static DataSet LeerExcelDeRutaADataSet(Stream stream)
        {
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

        public static void ConstruirExcel(ExcelPackage mExcel, string nombreHoja, Dictionary<string, Dictionary<string, string>> mDiccionario)
        {
            List<string> mListaClaves = new List<string>();

            ExcelWorksheet WorkSheetTextosPersonalizados = mExcel.Workbook.Worksheets.Add(nombreHoja);
            WorkSheetTextosPersonalizados.Cells[1, 1].Value = "Clave";

            int numeroFila = WorkSheetTextosPersonalizados.Dimension.Rows + 1;

            foreach (KeyValuePair<string, Dictionary<string, string>> item in mDiccionario)
            {
                string clave = item.Key;

                foreach (KeyValuePair<string, string> Traduccion in item.Value)
                {
                    string idioma = Traduccion.Key;
                    int numeroColumnaIdioma = BuscarColumnaIdioma(idioma, WorkSheetTextosPersonalizados);
                    string texto = Traduccion.Value;

                    if (!mListaClaves.Contains(clave))
                    {
                        WorkSheetTextosPersonalizados.Cells[numeroFila, 1].Value = clave;
                        WorkSheetTextosPersonalizados.Cells[numeroFila, numeroColumnaIdioma].Value = texto;
                        mListaClaves.Add(clave);
                        numeroFila++;
                    }
                    else
                    {
                        int indiceFila = mListaClaves.IndexOf(clave) + 2;
                        WorkSheetTextosPersonalizados.Cells[indiceFila, numeroColumnaIdioma].Value = texto;
                    }
                }
            }
        }


        private static int BuscarColumnaIdioma(string pIdioma, ExcelWorksheet WorkSheetTextosPersonalizados)
        {
            int numeroColumnaIdioma = 1;
            bool encontrado = false;
            while (!encontrado)
            {
                numeroColumnaIdioma++;
                if (string.IsNullOrEmpty(WorkSheetTextosPersonalizados.Cells[1, numeroColumnaIdioma].Value as string) || WorkSheetTextosPersonalizados.Cells[1, numeroColumnaIdioma].Value.Equals(pIdioma))
                {
                    WorkSheetTextosPersonalizados.Cells[1, numeroColumnaIdioma].Value = pIdioma;
                    encontrado = true;
                }
            }
            return numeroColumnaIdioma;
        }
    }
}
