using System;
using System.Collections.Generic;
using System.Data;

namespace Es.Riam.Util
{
    /// <summary>
    /// Utilidades de datos
    /// </summary>
    public class UtilDatos
    {
        #region Miembros

        /// <summary>
		/// Campo por el que se ordenará la lista
		/// </summary>
		private static string mCampoComparacion = "";

        #endregion

        #region Métodos

        /// <summary>
		/// Ordena el array de DataRow segun un campo del mismo. 
		/// El campo ha de ser de tipo entero
		/// </summary>
		/// <param name="pLista">Array de Datos que se quieren ordenar</param>
		/// <param name="pCampo">Campo por el que se van a ordenar los datos, el campo debe ser de tipo entero</param>
		/// <returns>Devuelve el array equivalente al original pero ordenado</returns>
		public static DataRow[] ListaOrdenadaPorCampoEntero(DataRow[] pLista, string pCampo)
        {
            if (pLista.Length < 2)
            {
                return pLista;
            }
            List<DataRow> lista = new List<DataRow>();
            lista.AddRange(pLista);
            mCampoComparacion = pCampo;
            lista.Sort(CompararDataRowCampoEntero);

            DataRow[] resp = (DataRow[])pLista.Clone();
            int i = 0;

            foreach (DataRow elem in lista)
            {
                resp[i] = elem;
                i++;
            }
            return resp;
        }

        /// <summary>
        /// Ordena el array de DataRow segun un campo del mismo. 
        /// El campo ha de ser de tipo entero
        /// </summary>
        /// <param name="pTabla">Tabla que se quieren ordenar</param>
        /// <param name="pCampo">Campo por el que se van a ordenar los datos, el campo debe ser de tipo entero</param>
        /// <returns>Devuelve el array equivalente al original pero ordenado</returns>
        public static DataRow[] ListaOrdenadaPorCampoEntero(DataTable pTabla, string pCampo)
        {
            return ListaOrdenadaPorCampoEntero(pTabla.Select(), pCampo);
        }

        /// <summary>
        /// Comparador de Datarows para ordenar
        /// </summary>
        /// <param name="pX">Primer dato a comparar</param>
        /// <param name="pY">Segundo dato a comparar</param>
        /// <returns>Devuelve 0 si son iguales, 1 si pX es mayor, -1 si pY es mayor</returns>
        private static int CompararDataRowCampoEntero(DataRow pX, DataRow pY)
        {
            if (pX == null)
            {
                if (pY == null)
                {
                    // Si ambos son nulos, son inguales
                    return 0;
                }
                else
                {
                    // Si x es nulo e y no lo es, y es mayor
                    return -1;
                }
            }
            else
            {
                if (pY == null)
                {
                    // Si x no es nulo e y sí lo es, y es menor
                    return 1;
                }
                else
                {
                    // Ambos son no nulos, comparo sus campos
                    long objX = Convert.ToInt64(pX[mCampoComparacion]);
                    long objY = Convert.ToInt64(pY[mCampoComparacion]);
                    return objX.CompareTo(objY);
                }
            }
        }

        /// <summary>
        /// Ordena el array de dataRow según un campo del mismo.
        /// El campo ha de ser de tipo alfanumérico
        /// </summary>
        /// <param name="pLista">Array de Datos que se quieren ordenar</param>
        /// <param name="pCampo">Campo por el que se van a ordenar los datos, el campo debe ser de tipo alfanumérico</param>
        /// <returns>Devuelve el array equivalente al original pero ordenado</returns>
        public static DataRow[] ListaOrdenadaPorCampoAlfanumerico(DataRow[] pLista, string pCampo)
        {
            if (pLista.Length < 2)
            {
                return pLista;
            }
            List<DataRow> lista = new List<DataRow>();
            lista.AddRange(pLista);
            mCampoComparacion = pCampo;
            lista.Sort(CompararDataRowCampoAlfanumerico);

            DataRow[] resp = (DataRow[])pLista.Clone();
            int i = 0;

            foreach (DataRow elem in lista)
            {
                resp[i] = elem;
                i++;
            }
            return resp;
        }

        /// <summary>
        /// Ordena el array de DataRow segun un campo del mismo. 
        /// El campo ha de ser de tipo alfanumérico
        /// </summary>
        /// <param name="pTabla">Tabla que se quieren ordenar</param>
        /// <param name="pCampo">Campo por el que se van a ordenar los datos, el campo debe ser de tipo alfanumérico</param>
        /// <returns>Devuelve el array equivalente al original pero ordenado</returns>
        public static DataRow[] ListaOrdenadaPorCampoAlfanumerico(DataTable pTabla, string pCampo)
        {
            //return ListaOrdenadaPorCampoAlfanumerico(pTabla.Select(), pCampo);
            return pTabla.Select("", pCampo + " DESC");
        }

        /// <summary>
        /// Comparador de datarows para ordenar
        /// </summary>
        /// <param name="pX">Primer dato a comparar</param>
        /// <param name="pY">Segundo dato a comparar</param>
        /// <returns>Devuelve 0 si son iguales, 1 si pX es mayor, -1 si pY es mayor</returns>
        private static int CompararDataRowCampoAlfanumerico(DataRow pX, DataRow pY)
        {
            if (pX == null)
            {
                if (pY == null)
                {
                    // Si ambos son nulos, son inguales
                    return 0;
                }
                else
                {
                    // Si x es nulo e y no lo es, y es mayor
                    return -1;
                }
            }
            else
            {
                if (pY == null)
                {
                    // Si x no es nulo e y sí lo es, y es menor
                    return 1;
                }
                else
                {
                    // Ambos son no nulos, comparo sus campos
                    string objX = (string)pX[mCampoComparacion];
                    string objY = (string)pY[mCampoComparacion];
                    return objX.CompareTo(objY);
                }
            }
        }

        /// <summary>
        /// Ordena el array de DataRow segun un campo del mismo. 
        /// El campo ha de ser de tipo alfanumérico
        /// </summary>
        /// <param name="pLista">Lista que se quiere ordenar</param>
        /// <param name="pCampo">Campo por el que se van a ordenar los datos, el campo debe ser de tipo alfanumérico</param>
        /// <returns>Devuelve el array equivalente al original pero ordenado</returns>
        public static DataRow[] ListaOrdenadaPorCampoFecha(DataRow[] pLista, string pCampo)
        {
            if (pLista.Length < 2)
            {
                return pLista;
            }
            List<DataRow> lista = new List<DataRow>();
            lista.AddRange(pLista);
            mCampoComparacion = pCampo;
            lista.Sort(CompararDataRowCampoFecha);

            DataRow[] resp = (DataRow[])pLista.Clone();
            int i = 0;

            foreach (DataRow elem in lista)
            {
                resp[i] = elem;
                i++;
            }
            return resp;
        }

        /// <summary>
        /// Ordena el array de DataRow segun un campo del mismo. 
        /// El campo ha de ser de tipo alfanumérico
        /// </summary>
        /// <param name="pLista">Lista que se quiere ordenar</param>
        /// <param name="pCampo">Campo por el que se van a ordenar los datos, el campo debe ser de tipo alfanumérico</param>
        /// <returns>Devuelve el array equivalente al original pero ordenado</returns>
        public static DataRow[] ListaOrdenadaPorCampoFechaDesc(DataRow[] pLista, string pCampo)
        {
            if (pLista.Length < 2)
            {
                return pLista;
            }
            List<DataRow> lista = new List<DataRow>();
            lista.AddRange(pLista);
            mCampoComparacion = pCampo;
            lista.Sort(CompararDataRowCampoFechaDesc);

            DataRow[] resp = (DataRow[])pLista.Clone();
            int i = 0;

            foreach (DataRow elem in lista)
            {
                resp[i] = elem;
                i++;
            }
            return resp;
        }

        /// <summary>
        /// Ordena el array de DataRow segun un campo del mismo. 
        /// El campo ha de ser de tipo alfanumérico
        /// </summary>
        /// <param name="pTabla">Tabla que se quieren ordenar</param>
        /// <param name="pCampo">Campo por el que se van a ordenar los datos, el campo debe ser de tipo alfanumérico</param>
        /// <returns>Devuelve el array equivalente al original pero ordenado</returns>
        public static DataRow[] ListaOrdenadaPorCampoFechaDesc(DataTable pTabla, string pCampo)
        {
            //return ListaOrdenadaPorCampoAlfanumerico(pTabla.Select(), pCampo);
            return pTabla.Select("", pCampo + " DESC");
        }

        /// <summary>
        /// Ordena el array de DataRow segun un campo del mismo. 
        /// El campo ha de ser de tipo alfanumérico
        /// </summary>
        /// <param name="pTabla">Tabla que se quieren ordenar</param>
        /// <param name="pCampo">Campo por el que se van a ordenar los datos, el campo debe ser de tipo alfanumérico</param>
        /// <returns>Devuelve el array equivalente al original pero ordenado</returns>
        public static DataRow[] ListaOrdenadaPorCampoFecha(DataTable pTabla, string pCampo)
        {
            //return ListaOrdenadaPorCampoAlfanumerico(pTabla.Select(), pCampo);
            return pTabla.Select("", pCampo);
        }

        /// <summary>
        /// Comparador de Datarows para ordenar
        /// </summary>
        /// <param name="pX">Primer dato a comparar</param>
        /// <param name="pY">Segundo dato a comparar</param>
        /// <returns>Devuelve 0 si son iguales, 1 si pX es mayor, -1 si pY es mayor</returns>
        private static int CompararDataRowCampoFecha(DataRow pX, DataRow pY)
        {
            if (pX == null)
            {
                if (pY == null)
                {
                    // Si ambos son nulos, son inguales
                    return 0;
                }
                else
                {
                    // Si x es nulo e y no lo es, y es mayor
                    return -1;
                }
            }
            else
            {
                if (pY == null)
                {
                    // Si x no es nulo e y sí lo es, y es menor
                    return 1;
                }
                else
                {
                    // Ambos son no nulos, comparo sus campos
                    DateTime objX = (DateTime)pX[mCampoComparacion];
                    DateTime objY = (DateTime)pY[mCampoComparacion];
                    //return objX.CompareTo(objY);
                    return objY.CompareTo(objX);
                }
            }
        }

        /// <summary>
        /// Comparador de Datarows para ordenar
        /// </summary>
        /// <param name="pX">Primer dato a comparar</param>
        /// <param name="pY">Segundo dato a comparar</param>
        /// <returns>Devuelve 0 si son iguales, 1 si pX es mayor, -1 si pY es mayor</returns>
        private static int CompararDataRowCampoFechaDesc(DataRow pX, DataRow pY)
        {
            if (pX == null)
            {
                if (pY == null)
                {
                    // Si ambos son nulos, son inguales
                    return 0;
                }
                else
                {
                    // Si x es nulo e y no lo es, y es mayor
                    return -1;
                }
            }
            else
            {
                if (pY == null)
                {
                    // Si x no es nulo e y sí lo es, y es menor
                    return 1;
                }
                else
                {
                    // Ambos son no nulos, comparo sus campos
                    DateTime objX = (DateTime)pX[mCampoComparacion];
                    DateTime objY = (DateTime)pY[mCampoComparacion];
                    //return objX.CompareTo(objY);
                    return objX.CompareTo(objY);
                }
            }
        }

        /// <summary>
        /// Devuelve la diferencia entro dos fechas, indicando el tipo de diferencia y el valor de la misma.
        /// </summary>
        /// <param name="pFecha1">Fecha mayor</param>
        /// <param name="pFecha2">Fecha menor</param>
        /// <returns>Tipo de diferencia (0 años, 1 meses, 2 semanas, 3 días, 4 horas, 5 minutos, 6 segundos) y el valor de la misma</returns>
        public static KeyValuePair<int, int> ObtenerDiferenciaEntreFechas(DateTime pFecha1, DateTime pFecha2)
        {
            if (pFecha2 > pFecha1)
            {
                DateTime fechaAux = pFecha1;
                pFecha1 = pFecha2;
                pFecha2 = fechaAux;
            }

            DateTime fechaConDiferencia = DateTime.MinValue.Add(pFecha1 - pFecha2);

            if (fechaConDiferencia.Year > 1)
            {
                return new KeyValuePair<int, int>(0, fechaConDiferencia.Year - 1);
            }
            else if (fechaConDiferencia.Month > 1)
            {
                return new KeyValuePair<int, int>(1, fechaConDiferencia.Month - 1);
            }
            else if (fechaConDiferencia.Day > 1)
            {
                if (fechaConDiferencia.Day > 7)
                {
                    int dias = (fechaConDiferencia.Day - 1);
                    int semanas = 1;

                    if (dias > 28)
                    {
                        semanas = 5;
                    }
                    else if (dias > 21)
                    {
                        semanas = 4;
                    }
                    else if (dias > 14)
                    {
                        semanas = 3;
                    }
                    else if (dias > 7)
                    {
                        semanas = 2;
                    }
                    return new KeyValuePair<int, int>(2, semanas);
                }
                else
                {
                    return new KeyValuePair<int, int>(3, fechaConDiferencia.Day - 1);
                }
            }
            else if (fechaConDiferencia.Hour > 0)
            {
                return new KeyValuePair<int, int>(4, fechaConDiferencia.Hour);
            }
            else if (fechaConDiferencia.Minute > 0)
            {
                return new KeyValuePair<int, int>(5, fechaConDiferencia.Minute);
            }
            else
            {
                return new KeyValuePair<int, int>(6, fechaConDiferencia.Second);
            }
        }

        /// <summary>
        /// Copia los datos de una fila a otra con los mismos campos
        /// </summary>
        /// <param name="pFilaOrigen">Fila de origen</param>
        /// <param name="pFilaDestino">Fila de destino</param>
        public static void CopiarDatosFila(DataRow pFilaOrigen, DataRow pFilaDestino)
        {
            //Copio la fila a la original
            foreach (DataColumn columna in pFilaOrigen.Table.Columns)
            {
                pFilaDestino[columna.ColumnName] = pFilaOrigen[columna.ColumnName];
            }
        }

        /// <summary>
        /// Devuelve un nombre junto con un índice necesario para que no esté repetido
        /// </summary>
        /// <param name="pNombre">Nombre raiz del elemento</param>
        /// <param name="pLista">Nombre raiz del elemento</param>
        public static string NuevoNombreNoRepetido(string pNombre, List<string> pLista)
        {
            int num = 0;
            int i = 0;
            bool terminar = false;

            while (i < pLista.Count && !terminar)
            {
                bool encontrado = false;

                //Compruebo que no hay ningún elemento con el mismo nombre
                if (i == 0)
                {
                    foreach (string nombre in pLista)
                    {
                        if (pNombre == nombre)
                        {
                            num = 1;
                        }
                    }
                }
                //En caso de haber más elementos obtengo el número que le corresponde
                else
                {
                    foreach (string nombre in pLista)
                    {
                        if (nombre == pNombre + i)
                        {
                            encontrado = true;
                        }
                    }
                    if (!encontrado)
                    {
                        num = i;
                        terminar = true;
                    }
                }
                i++;
            }

            if (!terminar)
            {
                num = i;
            }

            //Si no existe ningún elemento con el mismo nombre no le añado nada
            if (num == 0)
            {
                return pNombre;
            }
            //En caso de ser un nombre repetido le añado el índice
            else if (num > 0)
            {
                return pNombre + num;
            }
            return pNombre;
        }

        /// <summary>
        /// Método que devuelve un texto único para añadir en una columna. Para ello añade cardinales al texto que se le envía.
        /// </summary>
        /// <param name="pTabla">Tabla donde se va a añadir el texto</param>
        /// <param name="pColumna">Columna donde se introduce pTexto</param>
        /// <param name="pTexto">Texto que se pone en pColumna</param>
        /// <returns>Devuelve el texto enviado formateado para que sea único</returns>
        public static string TextoNuevaFila(System.Data.DataTable pTabla, string pColumna, string pTexto)
        {
            return TextoNuevaFila(pTabla.Select(null, null), pColumna, pTexto);
        }

        /// <summary>
        /// Método que devuelve un texto único para añadir en una columna. Para ello añade cardinales al texto que se le envía.
        /// </summary>
        /// <param name="pTabla">Tabla donde se va a añadir el texto</param>
        /// <param name="pColumna">Columna donde se introduce pTexto</param>
        /// <param name="pTexto">Texto que se pone en pColumna</param>
        /// <returns>Devuelve el texto enviado formateado para que sea único</returns>
        public static string TextoNuevaFilaSinEspacio(System.Data.DataTable pTabla, string pColumna, string pTexto)
        {
            return TextoNuevaFilaSinEspacio(pTabla.Select(null, null), pColumna, pTexto);
        }

        /// <summary>
        /// Método que devuelve un texto único para añadir en una columna. Para ello añade cardinales al texto que se le envía.
        /// </summary>
        /// <param name="pFilas">filas donde se va a añadir el texto</param>
        /// <param name="pColumna">Columna donde se introduce pTexto</param>
        /// <param name="pTexto">Texto que se pone en pColumna</param>
        /// <returns>Devuelve el texto enviado formateado para que sea único</returns>
        public static string TextoNuevaFila(System.Data.DataRow[] pFilas, string pColumna, string pTexto)
        {
            string texto = pTexto;
            string nombre;
            int ultimoInd = 0;

            for (int i = 0; i < pFilas.Length; i++)
            {
                if (pFilas[i].RowState != System.Data.DataRowState.Deleted)
                {
                    nombre = pFilas[i][pColumna].ToString();

                    if (nombre != null && nombre.ToString() == texto)
                    {
                        ultimoInd++;

                        if (ultimoInd == 0)
                        {
                            texto = pTexto;
                        }
                        else
                        {
                            texto = pTexto + " " + ultimoInd.ToString();
                        }
                        i = -1;
                    }
                }
            }
            return texto;
        }

        /// <summary>
        /// Método que devuelve un texto único para añadir en una columna. Para ello añade cardinales al texto que se le envía.
        /// </summary>
        /// <param name="pFilas">filas donde se va a añadir el texto</param>
        /// <param name="pColumna">Columna donde se introduce pTexto</param>
        /// <param name="pTexto">Texto que se pone en pColumna</param>
        /// <returns>Devuelve el texto enviado formateado para que sea único</returns>
        public static string TextoNuevaFilaSinEspacio(System.Data.DataRow[] pFilas, string pColumna, string pTexto)
        {
            string texto = pTexto;
            string nombre;
            int ultimoInd = 0;

            for (int i = 0; i < pFilas.Length; i++)
            {
                if (pFilas[i].RowState != System.Data.DataRowState.Deleted)
                {
                    nombre = pFilas[i][pColumna].ToString();

                    if (nombre != null && nombre.ToString() == texto)
                    {
                        ultimoInd++;

                        if (ultimoInd == 0)
                        {
                            texto = pTexto;
                        }
                        else
                        {
                            texto = pTexto + ultimoInd.ToString();
                        }
                        i = -1;
                    }
                }
            }
            return texto;
        }

        /// <summary>
        ///  Método que devuelve una fecha única para añadir en una columna. Para ello incrementa en uno la fecha que se le envía.
        /// </summary>
        /// <param name="pTabla">Tabla donde se va a añadir la fecha</param>
        /// <param name="pColumna">Columna donde se introduce pFecha</param>
        /// <param name="pFecha">Fecha que se pone en pColumna</param>
        /// <returns>Devuelve la fecha única</returns>
        public static DateTime FechaNuevaFila(System.Data.DataTable pTabla, string pColumna, DateTime pFecha)
        {
            return FechaNuevaFila(pTabla.Select(null, pColumna), pColumna, pFecha);
        }

        /// <summary>
        ///  Método que devuelve una fecha única para añadir en una columna. Para ello incrementa en uno la fecha que se le envía.
        /// </summary>
        /// <param name="pFilas">Filas donde se va a añadir la fecha</param>
        /// <param name="pColumna">Columna donde se introduce pFecha</param>
        /// <param name="pFecha">Fecha que se pone en pColumna</param>
        /// <returns>Devuelve la fecha única</returns>
        public static DateTime FechaNuevaFila(System.Data.DataRow[] pFilas, string pColumna, DateTime pFecha)
        {
            DateTime fecha = pFecha;
            DateTime aux;
            DataRow[] filas = pFilas;

            for (int i = 0; i < filas.Length; i++)
            {
                if (filas[i].RowState != System.Data.DataRowState.Deleted)
                {
                    aux = (DateTime)filas[i][pColumna];

                    if (aux != null && aux == fecha)
                    {
                        fecha = fecha.AddDays(1);
                    }
                }
            }
            return fecha;
        }

        #endregion
    }
}
