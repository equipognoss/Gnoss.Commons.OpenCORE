using System;
using System.Collections.Generic;
using System.Text;

namespace Es.Riam.Semantica.OWL
{
    /// <summary>
    /// Utilidades para la gestión de clases semánticas.
    /// </summary>

    [Serializable]
    public class UtilSemantica
    {
        #region Métodos estáticos

        #region Métodos de Entidades

        /// <summary>
        /// Devuelve la entidad con el id dado.
        /// </summary>
        /// <param name="pEntidadID">ID de la entidad solicitada.</param>
        /// <param name="pListaEntidades">Lista de entidades en la que se encuentra la entidad.</param>
        /// <returns></returns>
        public static ElementoOntologia ObtenerEntidadPorID(string pEntidadID, Dictionary<string, ElementoOntologia> pEntidadesPorId, Dictionary<string, ElementoOntologia> pEntidadesPorUri)
        {
            if(pEntidadID != null)
            {
                if (pEntidadesPorId.ContainsKey(pEntidadID))
                {
                    return pEntidadesPorId[pEntidadID];
                }
                else if (pEntidadesPorUri.ContainsKey(pEntidadID))
                {
                    return pEntidadesPorUri[pEntidadID];
                }
            }

            return null;
        }

        /// <summary>
        /// Elimina una entidad relacionada con otra
        /// </summary>
        /// <param name="pEntidadAEliminar">Entidad a eliminar</param>
        /// <param name="pEntidad">Entidad que posee la relación</param>
        /// <param name="pPropiedad">Propiedad que relaciona las entidades</param>
        public static void EliminarEntidadRelacionada(ElementoOntologia pEntidadAEliminar, ElementoOntologia pEntidad, Propiedad pPropiedad)
        {
            //if ((pEntidadAEliminar != null) && (pEntidadAEliminar.Elemento != null))
                //pEntidadAEliminar.SeDebeImprimir = false;
            
            if (pEntidad.EntidadesRelacionadas.Contains(pEntidadAEliminar))
            {
                pEntidad.EntidadesRelacionadas.Remove(pEntidadAEliminar);
                if (pPropiedad.FunctionalProperty)
                    pPropiedad.UnicoValor = new KeyValuePair<string, ElementoOntologia>(pPropiedad.UnicoValor.Key, null);
                else
                    pPropiedad.ListaValores[pEntidadAEliminar.ID] = null;
            }
        }

        /// <summary>
        /// Elimina una entidad relacionada con otra
        /// </summary>
        /// <param name="pEntidadID">ID de la entidad a eliminar</param>
        /// <param name="pEntidad">Entidad que posee la relación</param>
        /// <param name="pPropiedad">Propiedad que relaciona las entidades</param>
        public static void EliminarEntidadRelacionada(string pEntidadID, ElementoOntologia pEntidad, Propiedad pPropiedad)
        {
            bool encontrado = false;
            int contador = 0;
            while ((!encontrado) && (contador < pEntidad.EntidadesRelacionadas.Count))
            {
                if (pEntidad.EntidadesRelacionadas[contador].ID.Equals(pEntidadID))
                {
                    EliminarEntidadRelacionada(pEntidad.EntidadesRelacionadas[contador], pEntidad, pPropiedad);
                    encontrado = true;
                }
                contador++;
            }
        }

        #endregion

        #region Métodos de propiedades

        /// <summary>
        /// Devuelve la propiedad de nombre pNombrePropiedad
        /// </summary>
        /// <param name="pNombrePropiedad">Nombre de la propiedad solicitada</param>
        /// <param name="pListaPropiedades">Lista de propiedades en la que se encuentra.</param>
        /// <returns></returns>
        public static Propiedad ObtenerPropiedadDeNombre(string pNombrePropiedad, IList<Propiedad> pListaPropiedades)
        {
            foreach (Propiedad propiedad in pListaPropiedades)
            {
                if ((propiedad.Nombre.Equals(pNombrePropiedad)) || (propiedad.NombreConNamespace.Equals(pNombrePropiedad)) || (propiedad.NombreReal.Equals(pNombrePropiedad)))
                    return propiedad;
            }
            return null;
        }

        #endregion

        #endregion
    }
}
