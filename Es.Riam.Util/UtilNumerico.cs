using System.Collections.Generic;

namespace Es.Riam.Util
{
    /// <summary>
    /// Clase con utilidades para realizar operaciones matemáticas.
    /// </summary>
    public class UtilNumerico
    {
        /// <summary>
        /// Devuelve el entero mayor de una lista de enteros.
        /// </summary>
        /// <param name="listaEnteros">Lista de enteros</param>
        /// <returns>El mayor entero de la lista</returns>
        public static int Maximo(List<int> listaEnteros)
        {
            int aux = 0;
            foreach (int elemento in listaEnteros)
            {
                if (elemento > aux)
                {
                    aux = elemento;
                }
            }
            return aux;
        }
    }
}
