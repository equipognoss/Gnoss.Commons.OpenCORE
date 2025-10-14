using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.Elementos.Facetado;
using Es.Riam.Gnoss.Elementos.Peticiones;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces;
using Es.Riam.Interfaces.Observador;
using Es.Riam.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Es.Riam.Gnoss.Elementos
{
    #region Delegados

    /// <summary>
    /// Delegado para el cambio de elemento seleccionado
    /// </summary>
    public delegate void ElementoGnossSeleccionadoCambiadoEventHandler(object sender, ElementoGnossEventArgs e);

    #endregion

    /// <summary>
    /// Gestor Gnoss
    /// </summary>
    [Serializable]
    public class GestionGnoss : ElementoGnoss, ISerializable
    {
        #region Constantes

        /// <summary>
        /// Prefijo de la copia
        /// </summary>
        protected const string TEXTO_COPIA = "Copia de ";

        #endregion

        #region Miembros

        /// <summary>
        /// DataSet
        /// </summary>
        private DataSet mDataSet;
        private DataWrapperBase mDataWrapper;

        /// <summary>
        /// Si se está realizando una operación entre varios elementos. Para que no notifique más que una vez al final
        /// </summary>
        private bool mOperacionConjunta;

        /// <summary>
        /// Parametros generales del proyecto GNOSS actual. Para poder acceder a ellos desde cualquier gestor
        /// </summary>
        private ParametroGeneral mParametrosGenerales;

        /// <summary>
        /// Campos relacionados con el análisis de completitud
        /// </summary>
        ///private ParametrosGeneralesDS.CamposAnalisisCompletitudRow mCamposAnalisisCompletitud;

        /// <summary>
        /// Entidad seleccionada
        /// </summary>
        private ElementoGnoss mElementoSeleccionado;

        /// <summary>
        /// Elemento cortado o copiado
        /// </summary>
        private ElementoGnoss mElementoCortadoCopiado = null;
        #endregion

        #region Constructores

        public GestionGnoss() { }


        /// <summary>
        /// Constructor a partir de un dataset
        /// </summary>
        /// <param name="pDataSet">Dataset</param>
        public GestionGnoss(DataSet pDataSet)
            : base(pDataSet)
        {
            mDataSet = pDataSet;
        }

        public GestionGnoss(DataWrapperBase pDataWrapper)
            : base(pDataWrapper)
        {
            mDataWrapper = pDataWrapper;
        }


        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected GestionGnoss(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            mDataSet = info.GetValue("DataSet", typeof(DataSet)) as DataSet;
            mDataWrapper = info.GetValue("DataWrapper", typeof(DataWrapperBase)) as DataWrapperBase;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Devuelve si el usuario puede ver todas los elementos de algún tipo
        /// </summary>
        public virtual bool PuedeVerTODOS
        {
            get
            {
                // David: Se debe sobreescribir donde haga falta con la comprobación correcta
                return false;
            }
        }

        /// <summary>
        /// Obtiene o establece el dataset
        /// </summary>
        public DataSet DataSet
        {
            get
            {
                return mDataSet;
            }
            set
            {
                mDataSet = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el DataWrapper
        /// </summary>
        public DataWrapperBase DataWrapper
        {
            get
            {
                return mDataWrapper;
            }
            set
            {
                mDataWrapper = value;
            }
        }


        /// <summary>
        /// Obtiene el gestor Gnoss
        /// </summary>
        public override GestionGnoss GestorGnoss
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Devuelve si el usuario actual puede editar
        /// </summary>
        public virtual bool PuedeEditar
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Devuelve si el usuario actual puede editar TODOS los elementos
        /// </summary>
        public virtual bool PuedeEditarTODOS
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene o establece si se está realizando una operación entre varios elementos. Para que no notifique más que una vez al final
        /// </summary>
        public bool OperacionConjunta
        {
            get
            {
                return mOperacionConjunta;
            }
            set
            {
                mOperacionConjunta = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el elemento seleccionado
        /// </summary>
        public virtual ElementoGnoss ElementoSeleccionado
        {
            get
            {
                return mElementoSeleccionado;
            }
            set
            {
                if (mElementoSeleccionado != value)
                {
                    if (mElementoSeleccionado != null)
                    {
                        ElementoGnoss anterior = mElementoSeleccionado;
                        mElementoSeleccionado = value;

                        if (!anterior.EstaEliminado)
                        {
                            anterior.EstaSeleccionado = false;
                        }
                    }
                    else
                    {
                        mElementoSeleccionado = value;
                    }
                    NotificarCambioElementoSeleccionado(mElementoSeleccionado);
                }
            }
        }

        /// <summary>
        /// Obtiene o establece el elemento que se ha cortado o copiado
        /// </summary>
        public virtual ElementoGnoss ElementoCortadoCopiado
        {
            get
            {
                return mElementoCortadoCopiado;
            }
            set
            {
                if (mElementoCortadoCopiado != value)
                {
                    if (mElementoCortadoCopiado != null)
                    {
                        //Lo asigno a una variable para que no se produzaca un bucle infinito
                        ElementoGnoss anterior = mElementoCortadoCopiado;
                        mElementoCortadoCopiado = value;
                        anterior.EstaCortado = false;
                    }
                    mElementoCortadoCopiado = value;
                }
            }
        }

        /// <summary>
        /// Obtiene o establece los paramétros generales del proyecto GNOSS actual
        /// </summary>
        public ParametroGeneral ParametrosGenerales
        {
            get
            {
                return mParametrosGenerales;
            }
            set
            {
                mParametrosGenerales = value;
            }
        }

        /// <summary>
        /// Obtiene o establece los campos relacionados con el análisis de completitud
        /// </summary>
        //public ParametroGeneralDSEspacio.CamposAnalisisCompletitudRow CamposAnalisisCompletitud
        //{
        //    get
        //    {
        //        return mCamposAnalisisCompletitud;
        //    }
        //    set
        //    {
        //        mCamposAnalisisCompletitud = value;
        //    }
        //}

        #endregion

        #region Métodos

        /// <summary>
        /// Carga todos los elementos del gestor
        /// </summary>
        public void CargarGestor()
        {
            IElementoGnoss hijo;
            List<IElementoGnoss> elementos = new List<IElementoGnoss>();
            elementos.AddRange(Hijos);

            while (elementos.Count > 0)
            {
                hijo = elementos[0];
                elementos.RemoveAt(0);

                elementos.AddRange(hijo.Hijos);
            }
        }

        /// <summary>
        /// Realiza la copia de un elemento
        /// </summary>
        /// <param name="pOriginal">Elemento a copiar</param>
        /// <returns>Copia del elemento</returns>
        public virtual ElementoGnoss CopiarElemento(ElementoGnoss pOriginal)
        {
            throw new Exception("Método sin implementar");
        }

        /// <summary>
        /// Predicado usado tan sólo en la función RealizarPegado, ya que no funcionaba correctamente el código anterior
        /// </summary>
        /// <param name="pIdentificador">Identificador del ElementoGnoss</param>
        /// <returns></returns>
        private Predicate<IElementoGnoss> PredicadoConParametro(Guid pIdentificador)
        {
            return new Predicate<IElementoGnoss>(delegate (IElementoGnoss target)
            {
                return ((ElementoGnoss)target).Clave == pIdentificador;
            });
        }

        /// <summary>
        /// Pega un elemento sobre el padre
        /// </summary>
        /// <param name="pPadre">Padre sobre el que se pega</param>
        /// <param name="pPosicion">Posición en la que se pega</param>
        /// <param name="pPegado">Elemento pegado</param>
        public virtual void RealizarPegado(ICortarCopiarPegar pPadre, short pPosicion, ElementoGnoss pPegado)
        {
            //Estas dos líneas sustituyen a la de abajo comentada, ya que no funcionaba correctamente
            ElementoGnoss elementoABorrar = ((ElementoGnoss)pPegado.Padre.Hijos.Find(PredicadoConParametro(pPegado.Clave)));
            pPegado.Padre.Hijos.Remove(elementoABorrar);

            //Cambio el padre del elemento que pego            
            //pPegado.Padre.Hijos.Remove(pPegado);

            //Cambio el índice de los hijos del padre original
            if (pPegado.EstaCortado)
            {
                ((ElementoGnoss)pPegado.Padre).ActualizarOrdenHijos((short)pPegado.Indice, -1);
            }
            short posicion = pPosicion;
            ((ElementoGnoss)pPadre).ActualizarOrdenHijos((short)posicion, 1);

            if (pPegado.EstaCortado)
            {
                ModificarIdCortar(pPegado, (ElementoGnoss)pPadre);
            }
            else
            {
                ModificarIdCopiar(pPegado, (ElementoGnoss)pPadre);
            }
            //Actualizo el padre del elemento
            pPegado.Padre = pPadre;
            pPegado.Indice = pPosicion;

            ((ElementoGnoss)pPadre).LimpiarHijos();

            //Pego los hijos
            RealizarPegadoHijos(pPegado, pPegado.EstaCortado);

            //Limpio los hijos para que cargue los nuevos objetos
            pPegado.LimpiarHijos();

            if (pPegado.EstaCortado)
            {
                pPegado.EstaCortado = false;
            }
            else
            {
                //Establezco el nuevo nombre del elemento copiado
                pPegado.Nombre = TEXTO_COPIA + pPegado.Nombre;

                //Quito el grupo copiado para evitar problemas
                ElementoCortadoCopiado = null;
            }
        }

        /// <summary>
        /// Actualiza los hijos de un pegado
        /// </summary>
        /// <param name="pPegado">Elemento a pegar</param>
        /// <param name="pCortar">TRUE si el elemento está cortado</param>
        public virtual void RealizarPegadoHijos(ElementoGnoss pPegado, bool pCortar)
        {
            ElementoGnoss hijo;

            for (int i = 0; i < pPegado.Hijos.Count; i++)
            {
                hijo = (ElementoGnoss)pPegado.Hijos[i];

                //Cortando
                if (pCortar)
                {
                    ModificarIdCortar(hijo, pPegado);
                    RealizarPegadoHijos(hijo, pCortar);
                }
                //Copiando
                else
                {
                    //si estoy copiando en vez de cortando copio todas las filas de los hijos y las cambio
                    ElementoGnoss copiaHijo = CopiarElemento(hijo);
                    ModificarIdCopiar(copiaHijo, pPegado);

                    copiaHijo.Hijos.AddRange(hijo.Hijos);
                    pPegado.Hijos.Remove(hijo);
                    pPegado.Hijos.Insert(i, copiaHijo);

                    RealizarPegadoHijos(copiaHijo, pCortar);
                }
            }
        }

        /// <summary>
        /// Modifica los identificadores necesarios para hacer la copia
        /// </summary>
        /// <param name="pPegado">Elemento pegado</param>
        /// <param name="pPadre">Elemento padre</param>
        protected virtual void ModificarIdCopiar(ElementoGnoss pPegado, ElementoGnoss pPadre)
        {
        }

        /// <summary>
        /// Modifica los identificadores necesarios para cortar
        /// </summary>
        /// <param name="pPegado">Elemento pegado</param>
        /// <param name="pPadre">Elemento padre</param>
        protected virtual void ModificarIdCortar(ElementoGnoss pPegado, ElementoGnoss pPadre)
        {
        }

        /// <summary>
        /// Lanza el evento de que se ha cambiado el elemento seleccionado
        /// </summary>
        /// <param name="pElemento">Elemento seleccionado</param>
        public void NotificarCambioElementoSeleccionado(ElementoGnoss pElemento)
        {
            if (ElementoGnossSeleccionadoCambiado != null)
            {
                ElementoGnossSeleccionadoCambiado(this, new ElementoGnossEventArgs(pElemento));
            }
        }

        /// <summary>
        /// Comprueba si se puede eliminar un elemento
        /// </summary>
        /// <param name="pElemento">Elemento que se va a comprobar</param>
        /// <returns>TRUE si se puede eliminar</returns>
        public virtual bool ComprobarSePuedeEliminarElemento(ElementoGnoss pElemento)
        {
            return pElemento.EsEditable;
        }

        /// <summary>
        /// Construye una lista de elementos compuesta por todos los descendientes de un elemento padre a nivel de fila
        /// </summary>
        /// <param name="pPadre">Elemento del que se quieren obtener sus hijos</param>
        /// <returns>Lista compuesta por elementos en orden ascendente</returns>
        public virtual List<ElementoGnoss> ConstruirArbolCompletoHijosNivelFila(ElementoGnoss pPadre)
        {
            return new List<ElementoGnoss>();
        }

        /// <summary>
        /// Elimina un elemento. Se encarga de todo el proceso
        /// </summary>
        /// <param name="pElemento">Elemento a partir del cual se va a eliminar</param>
        public void Eliminar(ElementoGnoss pElemento)
        {
            if (pElemento != null && !pElemento.EstaEliminado)
            {
                OperacionConjunta = true;
                ElementoGnoss seleccionadoAnterior = pElemento;

                // I-BUG736
                // Cargar la lista con todos los hijos de este elemento
                // para hacer la eliminacion en orden ascendente
                List<ElementoGnoss> ArbolHijos = ConstruirArbolCompletoHijos(pElemento);
                ArbolHijos.Add(pElemento); // Añadir después de todos sus hijos, el elemento a borrar

                foreach (ElementoGnoss elemento in ArbolHijos)
                {
                    EliminarElemento(elemento);
                }
                // F-BUG736
                OperacionConjunta = false;
                Notificar(new MensajeObservador(AccionesObservador.Eliminar, seleccionadoAnterior));
            }
        }

        /// <summary>
        /// Construye una lista de elementos compuesta por todos los descendientes de un elemento padre.
        /// </summary>
        /// <param name="pPadre">Elemento del que se quieren obtener sus hijos</param>
        /// <returns>Lista compuesta por elementos en orden ascendente</returns>
        public List<ElementoGnoss> ConstruirArbolCompletoHijos(ElementoGnoss pPadre)
        {
            List<ElementoGnoss> listaHijos = new List<ElementoGnoss>();

            foreach (IElementoGnoss hijo in pPadre.Hijos)
            {
                // Recorrer los hijos del elemento actual y obtener a su vez los hijos de cada uno de ellos
                if (hijo is ElementoGnoss)
                {
                    // Primero se añaden a la lista todos los hijos y despues el elemento en sí
                    // La lista quedará ordenada por <hijos1><padre1><hijos2><padre2 (y hermano de padre1)>, etc.
                    listaHijos.AddRange(ConstruirArbolCompletoHijos((ElementoGnoss)hijo));
                    listaHijos.Add((ElementoGnoss)hijo);
                }
            }
            // David:
            // Este procedimiento solo devuelve los hijos que cuelgan directamente del elemento y accesibles para el usuario
            // Por ejemplo, las mejoras hijas del padre seleccionado, si no son visibles para este, no aparecen en la lista
            return listaHijos;
        }

        /// <summary>
        /// Elimina sólo el elemento
        /// </summary>
        /// <param name="pElemento">Elemento a eliminar</param>
        public virtual void EliminarElemento(ElementoGnoss pElemento)
        {
            //Si es el elemento seleccionado lo pongo a null para que no intente acceder a él
            if (ElementoSeleccionado == pElemento)
            {
                ElementoSeleccionado = null;
            }

            //Si es el elemento que está cortado o copiado lo pongo a null
            if (ElementoCortadoCopiado == pElemento)
            {
                ElementoCortadoCopiado = null;
            }
        }

        /// <summary>
        /// Devuelve la profundidad del elemento, es decir el número de niveles de hijos que tiene.
        /// </summary>
        /// <param name="pElemento">Elemento a revisar</param>
        /// <returns>Número de niveles de hijos que tiene</returns>
        public int ProfundidadElemento(ElementoGnoss pElemento)
        {
            if (pElemento.Hijos.Count == 0)
            {
                return 1;
            }
            else
            {
                return 1 + UtilNumerico.Maximo(ProfundidadElementosHijo(pElemento));
            }
        }

        /// <summary>
        /// Devuelve una lista con la profundidad de cada uno de los elementos del hijo.
        /// </summary>
        /// <param name="pElemento">Elemento padre</param>
        /// <returns>Lista con la profundidad de cada uno de los elementos del hijo</returns>
        public List<int> ProfundidadElementosHijo(ElementoGnoss pElemento)
        {
            List<int> profundidadesHijos = new List<int>();

            foreach (ElementoGnoss hijo in pElemento.Hijos)
            {
                profundidadesHijos.Add(ProfundidadElemento(hijo));
            }
            return profundidadesHijos;
        }

        #region Ordenar

        /// <summary>
        /// Compara dos elementos por fecha
        /// </summary>
        /// <param name="x">Elemento x</param>
        /// <param name="y">Elemento y</param>
        /// <returns>1 si x>y, -1 en caso contrario</returns>
        public static int CompararElementosPorFecha(ElementoGnoss x, ElementoGnoss y)
        {
            return x.Fecha.Ticks.CompareTo(y.Fecha.Ticks);
        }

        /// <summary>
        /// Compara dos elementos por fecha de forma descendente
        /// </summary>
        /// <param name="x">Elemento x</param>
        /// <param name="y">Elemento y</param>
        /// <returns>1 si x>y, -1 en caso contrario</returns>
        public static int CompararElementosPorFechaDesc(ElementoGnoss x, ElementoGnoss y)
        {
            return y.Fecha.Ticks.CompareTo(x.Fecha.Ticks);
        }

        /// <summary>
        /// Compara dos elementos por nombre
        /// </summary>
        /// <param name="x">Elemento x</param>
        /// <param name="y">Elemento y</param>
        /// <returns>1 si x>y, -1 en caso contrario</returns>
        public static int CompararElementosPorNombre(ElementoGnoss x, ElementoGnoss y)
        {
            return x.Nombre.CompareTo(y.Nombre);
        }

        /// <summary>
        /// Compara dos elementos por nombre de forma descendente
        /// </summary>
        /// <param name="x">Elemento x</param>
        /// <param name="y">Elemento y</param>
        /// <returns>1 si x>y, -1 en caso contrario</returns>
        public static int CompararElementosPorNombreDesc(ElementoGnoss x, ElementoGnoss y)
        {
            return y.Nombre.CompareTo(x.Nombre);
        }

        #endregion

        #endregion

        #region Eventos

        /// <summary>
        /// Evento que se produce cuando se cambia el elemento Gnoss seleccionado
        /// </summary>
        public event ElementoGnossSeleccionadoCambiadoEventHandler ElementoGnossSeleccionadoCambiado;

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~GestionGnoss()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                disposed = true;
                try
                {
                    if (disposing)
                    {
                        if (mDataSet != null)
                            mDataSet.Dispose();

                        if (DataWrapper != null)
                        {
                            DataWrapper.Dispose();
                        }
                    }
                }
                finally
                {
                    //Libero las variables grandes
                    mDataSet = null;
                    DataWrapper = null;

                    // Llamo al dispose de la clase base
                    base.Dispose(disposing);
                }
            }
        }

        #endregion

        #region Miembros de ISerializable

        /// <summary>
        /// Metodo para serializar el objeto
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter
        = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("DataSet", mDataSet);
            info.AddValue("DataWrapper", DataWrapper);
        }

        #endregion
    }

    /// <summary>
    /// Clase para las incompletitudes de los elementos
    /// </summary>
    public class Incompletitud
    {
        #region Constructores

        /// <summary>
        /// Constructor
        /// </summary>
        public Incompletitud()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pNombre">Nombre</param>
        public Incompletitud(string pNombre)
        {
            Nombre = pNombre;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pNombre">Nombre</param>
        /// <param name="pDescripcion">Descripción</param>
        public Incompletitud(string pNombre, string pDescripcion)
        {
            Nombre = pNombre;
            Descripcion = pDescripcion;
        }

        #endregion

        #region Miembros

        /// <summary>
        /// Nombre de la incompletitud
        /// </summary>
        public string Nombre = "";

        /// <summary>
        /// Descripción de la incompletitud
        /// </summary>
        public string Descripcion = "";

        #endregion
    }


}
