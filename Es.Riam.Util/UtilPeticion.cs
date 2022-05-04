using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Es.Riam.Util
{
    /// <summary>
    /// Clase de utilidades para la petición actual
    /// </summary>
    public class UtilPeticion
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UtilPeticion(IHttpContextAccessor httpContextAccessor = null)
        {

            _httpContextAccessor = httpContextAccessor;
        }
        #region Miembros estáticos

        /// <summary>
        /// Lista de Items que se usa para aplicaciones que no son Web. Hay una lista por thread de la aplicación
        /// </summary>
        private static ConcurrentDictionary<int, Dictionary<string, object>> mListaItemsPorThread = new ConcurrentDictionary<int, Dictionary<string, object>>();

        #endregion

        #region Metodos de HttpContext.Current.Items

        /// <summary>
        /// Obtiene la lista de Items de un thread
        /// </summary>
        /// <param name="pThreadID">Identificador del Thread</param>
        /// <returns>Lista de Items del thread actual</returns>
        private static Dictionary<string, object> ObtenerListaItemsDeThread(int pThreadID)
        {
            if (!mListaItemsPorThread.ContainsKey(pThreadID))
            {
                mListaItemsPorThread.TryAdd(pThreadID, new Dictionary<string, object>());
            }
            return mListaItemsPorThread[pThreadID];
        }

        /// <summary>
        /// Agrega un objeto a la petición actual. Dicho objeto sólo estará disponible para ésta petición
        /// </summary>
        /// <param name="pClave">Clave del objeto</param>
        /// <param name="pObjeto">Objeto a almacenar</param>
        public void AgregarObjetoAPeticionActual(string pClave, object pObjeto)
        {
            try
            {
                if (_httpContextAccessor != null && _httpContextAccessor.HttpContext!= null && _httpContextAccessor.HttpContext.Items != null)
                {
                    if (_httpContextAccessor.HttpContext.Items.ContainsKey(pClave))
                    {
                        _httpContextAccessor.HttpContext.Items[pClave] = pObjeto;
                    }
                    else
                    {
                        _httpContextAccessor.HttpContext.Items.Add(pClave, pObjeto);
                    }
                }
                else
                {
                    Dictionary<string, object> listaItems = ObtenerListaItemsDeThread(Thread.CurrentThread.ManagedThreadId);
                    if (listaItems.ContainsKey(pClave))
                    {
                        listaItems[pClave] = pObjeto;
                    }
                    else
                    {
                        listaItems.Add(pClave, pObjeto);
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Obtiene un objeto que se ha almacenado para la petición actual (Null si no hay nada con esa clave)
        /// </summary>
        /// <param name="pClave">Clave del objeto almacenado</param>
        /// <returns>Objeto que se corresponde con pClave</returns>
        public object ObtenerObjetoDePeticion(string pClave)
        {
            object resultado = null;

            try
            {
                if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Items != null)
                {
                    if (_httpContextAccessor.HttpContext.Items.ContainsKey(pClave))
                    {
                        resultado = _httpContextAccessor.HttpContext.Items[pClave];
                    }
                    
                }
                else
                {
                    Dictionary<string, object> listaItems = ObtenerListaItemsDeThread(Thread.CurrentThread.ManagedThreadId);
                    if (listaItems.ContainsKey(pClave))
                    {
                        resultado = listaItems[pClave];
                    }
                }
            }
            catch { }

            return resultado;
        }

        /// <summary>
        /// Elimina un objeto que se ha almacenado para la petición actual
        /// </summary>
        /// <param name="pClave">Clave del objeto almacenado</param>
        public void EliminarObjetoDePeticion(string pClave)
        {
            try
            {
                if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Items != null)
                {
                    if (_httpContextAccessor.HttpContext.Items.ContainsKey(pClave))
                    {
                        _httpContextAccessor.HttpContext.Items.Remove(pClave);
                    }
                }
                else
                {
                    Dictionary<string, object> listaItems = ObtenerListaItemsDeThread(Thread.CurrentThread.ManagedThreadId);
                    if (listaItems.ContainsKey(pClave))
                    {
                        listaItems.Remove(pClave);
                    }
                }


            }
            catch { }
        }

        public static void EliminarObjetosDeHilo(int pThreadId)
        {
            Dictionary<string, object> listaItems = ObtenerListaItemsDeThread(pThreadId);
            foreach (object item in listaItems.Values)
            {
                if (item is IDisposable)
                {
                    try
                    {
                        ((IDisposable)item).Dispose();
                    }
                    catch { }
                }
            }
            listaItems.Clear();
        }

        /// <summary>
        /// Comprueba si existe un objeto que se ha almacenado para la petición actual
        /// </summary>
        /// <param name="pClave">Clave del objeto almacenado</param>
        /// <returns>Verdad si hay un objeto que se corresponde con pClave</returns>
        public bool ExisteObjetoDePeticion(string pClave)
        {
            bool resultado = false;

            try
            {
                if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Items != null)
                {
                    resultado = _httpContextAccessor.HttpContext.Items.ContainsKey(pClave);
                }
                else
                {
                    Dictionary<string, object> listaItems = ObtenerListaItemsDeThread(Thread.CurrentThread.ManagedThreadId);
                    resultado = listaItems.ContainsKey(pClave);
                }
            }
            catch { }

            return resultado;
        }

        #endregion

    }
}
