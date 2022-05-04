using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Util
{
    /// <summary>
    /// Clase que implementa un índice que va rotando por RoundRobin
    /// </summary>
    public class RoundRobin
    {

        #region Miembros

        /// <summary>
        /// Índice que cambia cada vez que se consulta. 
        /// La palabra clave volatile significa que varios subprocesos pueden modificar la variable sin problemas 
        /// </summary>
        private volatile int mIndice = 0;

        /// <summary>
        /// Total de elementos en la lista
        /// </summary>
        private volatile int mTotal;

        /// <summary>
        /// Clave del RoundRobin
        /// </summary>
        private string mClave;

        /// <summary>
        /// Fecha de caducidad del RoundRobin
        /// </summary>
        private DateTime mFechaCaducidad;

        private static ConcurrentDictionary<string, RoundRobin> mRoundRobinDeClavesCompuestas = new ConcurrentDictionary<string, RoundRobin>();

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor de la clase RoundRobin a partir del total de elementos de una lista
        /// </summary>
        /// <param name="pTotal">Total de elementos de los que se quiere crear el índice RoundRobin</param>
        /// <param name="pClave">Clave del RoundRobin</param>
        /// <param name="pFechaCaducidad">Fecha de caducidad de este RoundRobin</param>
        public RoundRobin(string pClave, int pTotal, DateTime pFechaCaducidad)
        {
            mClave = pClave;
            mTotal = pTotal;
            mFechaCaducidad = pFechaCaducidad;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene el siguiente elemento de la lista y aumenta en uno el índice
        /// </summary>
        public int Siguiente
        {
            get
            {
                if (mIndice >= mTotal)
                {
                    mIndice = 0;
                }
                return mIndice++;
            }
        }

        /// <summary>
        /// Obtiene el total de elementos en la lista
        /// </summary>
        public int Total
        {
            get
            {
                return mTotal;
            }
        }

        /// <summary>
        /// Obtiene la fecha de caducidad de este objeto
        /// </summary>
        public DateTime FechaCaducidad
        {
            get
            {
                return mFechaCaducidad;
            }
        }

        #endregion

        #region Metodos generales

        /// <summary>
        /// Actualiza la fecha de caducidad de una clave
        /// </summary>
        /// <param name="pTotalElementos">Total de número de elementos que tiene esta lista</param>
        /// <param name="pSegundos">Segundos que va a durar la clave</param>
        /// <returns></returns>
        public void ActualizarRoundRobin(int pTotalElementos, int pSegundos)
        {
            this.mFechaCaducidad = DateTime.Now.AddSeconds(pSegundos);
            if (pTotalElementos != Total)
            {
                // El total ha cambiado (se ha quitado o se a añadido algún servidor al balanceador)
                this.mTotal = pTotalElementos;

                //Volvemos a inicializar el índice
                this.mIndice = 0;
            }
        }

        /// <summary>
        /// Obtiene el siguiente elemento de la lista
        /// </summary>
        /// <param name="pListaElementos">Lista de elementos</param>
        /// <param name="pValoresInvalidos">Lista de valores inválidos</param>
        /// <returns></returns>
        public string ObtenerSiguienteElemento(List<string> pListaElementos, List<string> pValoresInvalidos, bool pIgnorarRoundRobin = false)
        {
            string resultado = "";

            if (pListaElementos != null && pListaElementos.Count > 0)
            {
                if (pListaElementos.Count == 1)
                {
                    //Si sólo hay un elemento, lo devuelvo
                    resultado = pListaElementos[0];
                }
                else
                {
                    if (pValoresInvalidos != null && pValoresInvalidos.Count > 0 && pValoresInvalidos.Count < pListaElementos.Count)
                    {
                        // Hay valores que no se quieren obtener (porque ha fallado alguna conexión a esos servidores)
                        // Recorro con un foreach para obtener uno que todavía no se haya usado
                        foreach (string parametro in pListaElementos)
                        {
                            if (!pValoresInvalidos.Contains(parametro))
                            {
                                resultado = parametro;
                                break;
                            }
                        }
                    }
                    else if (!pIgnorarRoundRobin)
                    {
                        int indice = Siguiente;
                        if (indice >= pListaElementos.Count && indice > 0)
                        {
                            indice = 0;
                        }
                        resultado = pListaElementos[indice];
                    }
                    else
                    {
                        resultado = pListaElementos[0];
                    }
                }
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene el siguiente elemento de la lista
        /// </summary>
        /// <param name="pListaElementos">Lista de elementos</param>
        /// <param name="pValoresInvalidos">Lista de valores inválidos</param>
        /// <returns></returns>
        public object ObtenerSiguienteElemento(IEnumerable<object> pListaElementos)
        {
            object resultado = "";

            if (pListaElementos != null && pListaElementos.Any())
            {
                if (pListaElementos.Count() == 1)
                {
                    //Si sólo hay un elemento, lo devuelvo
                    resultado = pListaElementos.First();
                }
                else
                {
                    int indice = Siguiente;
                    if (indice >= pListaElementos.Count() && indice > 0)
                    {
                        indice = 0;
                    }
                    resultado = pListaElementos.ElementAt(indice);
                }
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene el siguiente elemento de la lista
        /// </summary>
        /// <param name="pListaElementos">Lista de elementos</param>
        /// <param name="pValoresInvalidos">Lista de valores inválidos</param>
        /// <returns></returns>
        public KeyValuePair<string, int> ObtenerSiguienteElemento(List<KeyValuePair<string, int>> pListaElementos)
        {
            KeyValuePair<string, int> resultado = new KeyValuePair<string, int>();

            if (pListaElementos != null && pListaElementos.Count > 0)
            {
                if (pListaElementos.Count == 1)
                {
                    //Si sólo hay un elemento, lo devuelvo
                    resultado = pListaElementos[0];
                }
                else
                {
                    int indice = Siguiente;
                    if (indice >= pListaElementos.Count && indice > 0)
                    {
                        indice = 0;
                    }
                    resultado = pListaElementos[indice];
                }
            }
            return resultado;
        }

        /// <summary>
        /// Obtiene el siguiente elemento de la lista
        /// </summary>
        /// <param name="pListaElementos">Lista de elementos</param>
        /// <param name="pValoresInvalidos">Lista de valores inválidos</param>
        /// <returns></returns>
        public KeyValuePair<string, int> ObtenerSiguienteElemento(List<KeyValuePair<string, int>> pListaElementos, List<string> pValoresInvalidos)
        {
            KeyValuePair<string, int> resultado = new KeyValuePair<string, int>();

            if (pListaElementos != null && pListaElementos.Count > 0)
            {
                if (pListaElementos.Count == 1)
                {
                    foreach (KeyValuePair<string, int> listaIndependientes in pListaElementos)
                    {
                        if (!pValoresInvalidos.Contains(listaIndependientes.Key))
                        {
                            //Si sólo hay un elemento, lo devuelvo
                            resultado = listaIndependientes;
                            break;
                        }
                    }
                }
                else
                {
                    if (pValoresInvalidos != null && pValoresInvalidos.Count > 0 && pValoresInvalidos.Count < pListaElementos.Count)
                    {
                        // Hay valores que no se quieren obtener (porque ha fallado alguna conexión a esos servidores)
                        // Recorro con un foreach para obtener uno que todavía no se haya usado
                        foreach (KeyValuePair<string, int> listaIndependientes in pListaElementos)
                        {
                            if (!pValoresInvalidos.Contains(listaIndependientes.Key))
                            {
                                resultado = listaIndependientes;
                                break;
                            }
                        }
                    }
                    else
                    {
                        int indice = Siguiente;
                        if (indice >= pListaElementos.Count && indice > 0)
                        {
                            indice = 0;
                        }
                        resultado = pListaElementos[indice];
                    }
                }
            }
            return resultado;
        }

        /// <summary>
        /// Crea la caducidad por Defecto y un indice RoundRobin para un parámetro compuesto
        /// </summary>
        /// <param name="pClave">Clave del elemento</param>
        /// <param name="pNumeroTotalElementos">Numero total de elementos que tiene la lista</param>
        /// <param name="pFechaCaducidad">Fecha de caducidad</param>
        private static void CrearRoundRobinParaParametroCompuesto(string pClave, int pNumeroTotalElementos, DateTime pFechaCaducidad)
        {
            if (!mRoundRobinDeClavesCompuestas.ContainsKey(pClave))
            {
                try
                {
                    //Añado el RoundRobin inicial
                    mRoundRobinDeClavesCompuestas.TryAdd(pClave, new RoundRobin(pClave, pNumeroTotalElementos, pFechaCaducidad));
                }
                catch (Exception)
                {
                    // No hace falta capturar esta excepción, 
                    // si ha saltado es porque otro subproceso a añadido a la vez el mismo elemento a la lista
                }
            }
        }

        /// <summary>
        /// Obtiene un objeto RoundRobin para esta clave (si no existe, crea uno)
        /// </summary>
        /// <param name="pClave">Clave de la lista</param>
        /// <param name="pNumeroTotalElementos">Número total de elementos de la clave</param>
        /// <param name="pSegundos">Segundos que va a durar este objeto</param>
        /// <returns></returns>
        public static RoundRobin ObtenerRoundRobinDeClave(string pClave, int pNumeroTotalElementos, int pSegundos)
        {
            if (!mRoundRobinDeClavesCompuestas.ContainsKey(pClave))
            {
                CrearRoundRobinParaParametroCompuesto(pClave, pNumeroTotalElementos, DateTime.Now.AddSeconds(pSegundos));
            }

            if (mRoundRobinDeClavesCompuestas.ContainsKey(pClave))
            {
                return mRoundRobinDeClavesCompuestas[pClave];
            }

            // No debería llega aquí, pero por si ha habido problemas de concurrencia al añadir al diccionario, devuelvo un RoundRobin nuevo. 
            return new RoundRobin(pClave, pNumeroTotalElementos, DateTime.Now.AddSeconds(pSegundos));
        }


        /// <summary>
        /// Actualiza la fecha de caducidad de una clave
        /// </summary>
        /// <param name="pClave">Clave de la lista</param>
        /// <param name="pTotalElementos">Total de número de elementos que tiene esta lista</param>
        /// <param name="pSegundos">Segundos que va a durar la clave</param>
        /// <returns></returns>
        public static void ActualizarRoundRobin(string pClave, int pTotalElementos, int pSegundos)
        {
            if (mRoundRobinDeClavesCompuestas.ContainsKey(pClave))
            {
                RoundRobin roundRobin = mRoundRobinDeClavesCompuestas[pClave];

                roundRobin.mFechaCaducidad = DateTime.Now.AddSeconds(pSegundos);
                if (pTotalElementos != roundRobin.Total)
                {
                    // El total ha cambiado (se ha quitado o se a añadido algún servidor al balanceador)
                    roundRobin.mTotal = pTotalElementos;

                    //Volvemos a inicializar el índice
                    roundRobin.mIndice = 0;
                }
            }
        }

        #endregion
    }
}
