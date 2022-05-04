using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces;
using Es.Riam.Interfaces.Observador;
using Es.Riam.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Es.Riam.Gnoss.Elementos
{
    /// <summary>
    /// Elemento Gnoss
    /// </summary>
    public class ElementoGnoss : ISujeto, ICortarCopiarPegar
    {
        #region Miembros

        /// <summary>
        /// TRUE si el elemento tiene algún hijo visible
        /// </summary>
        private bool mTieneHijosVisibles;

        /// <summary>
        /// TRUE si se ha comprobado la visibilidad de los hijos del elemento
        /// </summary>
        private bool mVisibilidadHijosComprobada;

        /// <summary>
        /// DataRow del elemento
        /// </summary>
        private DataRow mFilaElemento;

        private object mFilaElementoEntity;
        /// <summary>
        /// Lista de hijos
        /// </summary>
        protected List<IElementoGnoss> mHijos;

        /// <summary>
        /// Padre del elemento
        /// </summary>
        protected IElementoGnoss mPadre;

        /// <summary>
        /// Lista de observadores
        /// </summary>
        private List<IObservador> mObservadores = new List<IObservador>();

        /// <summary>
        /// Gestor GNOSS
        /// </summary>
        private GestionGnoss mGestionGnoss;

        /// <summary>
        /// Estado de visibilidad
        /// </summary>
        private bool mEstaVisible = true;

        /// <summary>
        /// Determina si está seleccionado
        /// </summary>
        private bool mEstaSeleccionado = false;

        /// <summary>
        /// Determina si está contraído
        /// </summary>
        private bool mEstaContraido = true;

        /// <summary>
        /// Determina si un elemento está cortado
        /// </summary>
        private bool mEstaCortado = false;

        /// <summary>
        /// Determina si el padre está cortado
        /// </summary>
        private bool mEstaCortadoPadre;

        /// <summary>
        /// Lista de incompletitudes del elemento
        /// </summary>
        private List<Incompletitud> mIncompletitudes = new List<Incompletitud>();

        /// <summary>
        /// Lista de tags del documento
        /// </summary>
        private List<string> mListaTags;

        private LoggingService mLoggingService;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ElementoGnoss(LoggingService loggingService)
        {
            mLoggingService = loggingService;
        }

        public ElementoGnoss(GestionGnoss pGestionGnoss, LoggingService loggingService)
        {
            mGestionGnoss = pGestionGnoss;
            mLoggingService = loggingService;
        }
        /// <summary>
        /// Constructor a partir de la fila del elemento
        /// </summary>
        /// <param name="pFilaElementoEntity">Fila del elemento</param>
        public ElementoGnoss(object pFilaElementoEntity, LoggingService loggingService)
        {
            mFilaElementoEntity = pFilaElementoEntity;
            mLoggingService = loggingService;
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected ElementoGnoss(SerializationInfo info, StreamingContext context)
        {
        }
        /// <summary>
        /// Metodo para serializar el objeto
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter
        = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            
        }
        /// <summary>
        /// Constructor a partir de la fila del elemento y del gestor
        /// </summary>
        /// <param name="pFilaElemento">Fila del elemento</param>
        /// <param name="pGestionGnoss">Gestor GNOSS</param>
        public ElementoGnoss(DataRow pFilaElemento, GestionGnoss pGestionGnoss, LoggingService loggingService)
        {
            mLoggingService = loggingService;
            mFilaElemento = pFilaElemento;
            mGestionGnoss = pGestionGnoss;
        }

        /// <summary>
        /// Constructor a partir de la fila del elemento y del gestor
        /// </summary>
        /// <param name="pFilaElemento">Fila del elemento</param>
        /// <param name="pGestionGnoss">Gestor GNOSS</param>
        public ElementoGnoss(object pFilaElemento, GestionGnoss pGestionGnoss, LoggingService loggingService)
        {
            mLoggingService = loggingService;
            mFilaElementoEntity = pFilaElemento;
            mGestionGnoss = pGestionGnoss;
        }
        #endregion

        #region Miembros de IElementoGnoss

        /// <summary>
        /// Devuelve si la fila del elemento ha sida eliminada
        /// </summary>
        public virtual bool EstaEliminado
        {
            get
            {
                if (FilaElemento == null)
                    return false;
                return FilaElemento.RowState == System.Data.DataRowState.Deleted || FilaElemento.RowState == System.Data.DataRowState.Detached;
            }
        }

        /// <summary>
        /// Obtiene o establece el nombre del elemento
        /// </summary>
        public virtual string Nombre
        {
            get
            {
                return "";
            }
            set
            {
                Notificar(new MensajeObservador(AccionesObservador.Invalidar, this));
            }
        }

        /// <summary>
        /// Obtiene o establece la fecha
        /// </summary>
        public virtual DateTime Fecha
        {
            get
            {
                return DateTime.Now;
            }
            set
            {
            }
        }

        /// <summary>
        /// Obtiene o establece la fila del elemento
        /// </summary>
        public virtual DataRow FilaElemento
        {
            get
            {
                return mFilaElemento;
            }
            set
            {
                mFilaElemento = value;
            }
        }

        /// <summary>
        /// Obtiene o establece la fila del elemento
        /// </summary>
        public virtual object FilaElementoEntity
        {
            get
            {
                return mFilaElementoEntity;
            }
            set
            {
                mFilaElementoEntity = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el elemento padre
        /// </summary>
        public virtual IElementoGnoss Padre
        {
            get { return mPadre; }
            set
            {
                mPadre = value;
            }
        }

        /// <summary>
        /// Obtiene la lista de elementos hijos
        /// </summary>
        public virtual List<IElementoGnoss> Hijos
        {
            get
            {
                return new List<IElementoGnoss>();
            }
        }

        /// <summary>
        /// Obtiene o establece el índice del elemento
        /// </summary>
        public virtual short Indice
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        /// Obtiene o establece el color del elemento
        /// </summary>
        public virtual Color Color
        {
            get { return Color.Empty; }
            set
            {
                if (value != null)
                    Notificar(new MensajeObservador(AccionesObservador.Invalidar, this));
            }
        }

        /// <summary>
        /// Devuelve si el elemento se puede editar
        /// </summary>
        public virtual bool EsEditable
        {
            get { return false; }
        }

        /// <summary>
        /// Devuelve si el elemento se puede seleccionar
        /// </summary>
        public virtual bool EsSeleccionable
        {
            get { return true; }
        }

        /// <summary>
        /// Devuelve si se puede eliminar el elemento
        /// </summary>
        public virtual bool SePuedeEliminar
        {
            get { return EsEditable; }
        }

        /// <summary>
        /// Devuelve o establece si el elemento está seleccionado
        /// </summary>
        public virtual bool EstaSeleccionado
        {
            get
            {
                return mEstaSeleccionado;
            }
            set
            {
                if (value != mEstaSeleccionado)
                {
                    mEstaSeleccionado = value;

                    //Si está seleccionado lo asigno como elemento seleccionado
                    if ((value) && (GestorGnoss.ElementoSeleccionado != this))
                    {
                        GestorGnoss.ElementoSeleccionado = this;
                    }
                    //Si está seleccionado lanzo el evento de que el elemento seleccionado ha cambiado
                    if ((value) && (GestorGnoss.ElementoSeleccionado != this))
                    {
                        Notificar(new MensajeObservador(AccionesObservador.Invalidar, this));
                    }
                }
            }
        }

        /// <summary>
        /// Develve o establece si el elemento está visible
        /// </summary>
        public virtual bool EstaVisible
        {
            get
            {
                return mEstaVisible;
            }
            set
            {
                mEstaVisible = value;
            }
        }

        /// <summary>
        /// Devuelve si el elemento es extensible
        /// </summary>
        public virtual bool EsExtensible
        {
            get { return (Hijos.Count > 0); }
        }

        /// <summary>
        /// Devuelve o establece si está contraído
        /// </summary>
        public virtual bool EstaContraido
        {
            get
            {
                return mEstaContraido;
            }
            set
            {
                if (mEstaContraido != value)
                {
                    mEstaContraido = value;

                    if (!(GestorGnoss != null && GestorGnoss.OperacionConjunta))
                        Notificar(new MensajeObservador(AccionesObservador.Ajustar, this));
                }
            }
        }

        /// <summary>
        /// Devuelve el código del elemento
        /// </summary>
        public virtual string Codigo
        {
            get
            {
                if (this is GestionGnoss)
                    return "";
                else if (Padre == null || Padre == GestorGnoss)
                    return Convert.ToString(Indice + 1);
                else
                {
                    string codigoPadre = ((IElementoGnoss)Padre).Codigo;

                    if (codigoPadre != "")
                        codigoPadre += ".";
                    return (codigoPadre + (Indice + 1));
                }
            }
        }

        #endregion

        #region Miembros de ISujeto

        /// <summary>
        /// Agrega un elemento a la lista de observadores del elemento
        /// </summary>
        /// <param name="pObs">Observador</param>
        public virtual void AgregarObservador(IObservador pObs)
        {
            if (!mObservadores.Contains(pObs))
            {
                mObservadores.Add(pObs);
            }
        }

        /// <summary>
        /// Elimina un elemento de la lista de observadores del elemento
        /// </summary>
        /// <param name="pObs">Observador</param>
        public virtual void EliminarObservador(IObservador pObs)
        {
            if ((mObservadores != null) && (mObservadores.Contains(pObs)))
            {
                mObservadores.Remove(pObs);
            }
        }

        /// <summary>
        /// Notifica a los observadores
        /// </summary>
        /// <param name="pMensaje">Mensaje</param>
        public virtual void Notificar(MensajeObservador pMensaje)
        {
            //Emisor del mensaje
            if (pMensaje.Emisor == null)
            {
                pMensaje.Emisor = this;
            }

            if (mObservadores != null)
            {
                List<IObservador> disposed = new List<IObservador>();

                for (int i = 0; i < mObservadores.Count; i++)
                {
                    try
                    {
                        if (mObservadores[i].EstaDisposed)
                        {
                            disposed.Add(mObservadores[i]);
                        }
                        else
                        {
                            if (mObservadores[i].EstaObservando)
                            {
                                mObservadores[i].Avisar(pMensaje);
                            }
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        disposed.Add(mObservadores[i]);
                    }
                }

                if (!GestorGnoss.OperacionConjunta && !(this is GestionGnoss))
                {
                    GestorGnoss.Notificar(pMensaje);
                }

                foreach (IObservador observador in disposed)
                {
                    EliminarObservador(observador);
                }
            }
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Limpia la coleción de hijos
        /// </summary>
        public virtual void LimpiarHijos()
        {
            mHijos = null;
        }

        /// <summary>
        /// Comprueba si un elemento es hijo de otro
        /// </summary>
        /// <param name="pElementoPadre">Elemento padre de referencia</param>
        /// <returns>Devuelve si este elemento es hijo de pElementoPadre</returns>
        public bool EsHijoDe(IElementoGnoss pElementoPadre)
        {
            IElementoGnoss padre = this;

            while (padre != null)
            {
                if (padre == pElementoPadre)
                {
                    return true;
                }
                padre = padre.Padre;
            }
            return false;
        }

        /// <summary>
        /// Actualiza el orden de los hijos del elemento
        /// </summary>
        /// <param name="pComienzo">Índice del hijo por el que comenzar</param>
        /// <param name="pIncremento">Incremento del orden. Normalmente +1 o -1</param>
        public virtual void ActualizarOrdenHijos(short pComienzo, short pIncremento)
        {
            if (pComienzo >= 0 && pComienzo < Hijos.Count)
            {
                for (short i = pComienzo; i < Hijos.Count; i++)
                {
                    Hijos[i].Indice = (short)(Hijos[i].Indice + pIncremento);
                }
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece si el elemento tiene algún hijo visible
        /// </summary>
        public bool TieneHijosVisibles
        {
            get
            {
                return mTieneHijosVisibles;
            }
            set
            {
                mTieneHijosVisibles = value;
            }
        }

        /// <summary>
        /// Obtiene o establece si se ha comprobado la visibilidad de los hijos del elemento
        /// </summary>
        public bool VisibilidadHijosComprobada
        {
            get
            {
                return mVisibilidadHijosComprobada;
            }
            set
            {
                mVisibilidadHijosComprobada = value;
            }
        }

        /// <summary>
        /// Obtiene el identificador del elemento
        /// </summary>
        public virtual Guid Clave
        {
            get
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Obtiene el gestor Gnoss
        /// </summary>
        public virtual GestionGnoss GestorGnoss
        {
            get
            {
                return mGestionGnoss;
            }
        }

        /// <summary>
        /// Devuelve si el elemento ha sido completado
        /// </summary>
        public virtual bool EstaCompleto
        {
            get
            {
                return (mIncompletitudes.Count == 0);
            }
        }

        /// <summary>
        /// Devuelve si los hijos del elemento han sido ya completados
        /// </summary>
        public bool EstaCompletoHijos
        {
            get
            {
                if (!EstaCompleto)
                {
                    return false;
                }

                foreach (IElementoGnoss hijo in Hijos)
                {
                    if (!((ElementoGnoss)hijo).EstaCompletoHijos)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Obtiene la lista de incompletitudes
        /// </summary>
        public List<Incompletitud> Incompletitudes
        {
            get
            {
                return mIncompletitudes;
            }
        }

        /// <summary>
        /// Obtiene la lista de tags manuales del elemento 
        /// ATENCIÓN: Esta lista es de sólo lectura, los cambios realizados aquí no van al DataSet
        /// Para modificar los tags, accede a la propiedad Tags
        /// </summary>
        public List<string> ListaTagsSoloLectura
        {
            get
            {
                if (mListaTags == null)
                {
                    mListaTags = new List<string>();

                    char[] separadores = { ',' };

                    foreach (string tag in (Tags).Split(separadores, StringSplitOptions.RemoveEmptyEntries))
                    {
                        mListaTags.Add(tag.Trim());
                    }
                }
                return mListaTags;
            }
            set
            {
                mListaTags = value;
            }
        }

        /// <summary>
        /// Obtiene los tags del elemento 
        /// </summary>
        public virtual string Tags
        {
            get
            {
                string tags = "";
                if (FilaElementoEntity != null && UtilReflection.ContainsValue(FilaElementoEntity, "Tags") && !string.IsNullOrEmpty((string)UtilReflection.GetValueReflection(FilaElementoEntity, "Tags")))
                {
                    tags = (string)UtilReflection.GetValueReflection(FilaElementoEntity, "Tags");
                }
                return tags;
            }
            set
            {
                if (FilaElementoEntity != null && UtilReflection.ContainsValue(FilaElementoEntity, "Tags"))
                {
                    UtilReflection.SetValueReflection(FilaElementoEntity, "Tags", value);
                    mListaTags = null;
                }
            }
        }

        #endregion

        #region Miembros de ICortarCopiarPegar

        /// <summary>
        /// Corta el elemento
        /// </summary>
        public virtual void Cortar()
        {
            EstaCortado = true;
        }

        /// <summary>
        /// Pega el elemento en el índice especificado
        /// </summary>
        /// <param name="pElementoAPegar">Elemento</param>
        /// <param name="pIndice">Índice</param>
        public virtual void Pegar(IElementoGnoss pElementoAPegar, short pIndice)
        {
            if (pElementoAPegar is ElementoGnoss && GestorGnoss != null)
            {
                if (pElementoAPegar != this)
                {
                    ElementoGnoss elementoAPegar = null;

                    //CORTAR
                    if (((ElementoGnoss)pElementoAPegar).EstaCortado)
                    {
                        elementoAPegar = (ElementoGnoss)pElementoAPegar;
                        //Si esta cortado lo elimino del origen
                        elementoAPegar.Notificar(new MensajeObservador(AccionesObservador.Eliminar, elementoAPegar));
                    }
                    //COPIAR
                    else
                    {
                        elementoAPegar = GestorGnoss.CopiarElemento((ElementoGnoss)pElementoAPegar);

                        //Le copio la lista de hijos
                        elementoAPegar.Hijos.AddRange(pElementoAPegar.Hijos);
                    }
                    short indice = 0;

                    if (pIndice >= 0)
                    {
                        indice = pIndice;
                    }
                    else if (this.Hijos.Count > 0)
                    {
                        //IMPORTANTE: Sin esto no funciona al pegar ya que el índice lo pone mal sobre el mismo padre. Si estoy moviendo sobre el mismo padre y colocándolo antes de lo que estaba, pongo el desfase a 1 para pegar en una posición anterior. 
                        short incremento = 1;

                        if (elementoAPegar.Padre == this && elementoAPegar.Indice <= this.Hijos.Count - 1)
                        {
                            incremento = 0;
                        }
                        indice = (short)(this.Hijos[this.Hijos.Count - 1].Indice + incremento);
                    }
                    //Realizo el pegado
                    GestorGnoss.RealizarPegado(this, indice, elementoAPegar);

                    //Notifico que se ha pegado un elemento
                    ((ElementoGnoss)elementoAPegar.Padre).Notificar(new MensajeObservador(AccionesObservador.Pegar, elementoAPegar));
                }
                else
                {
                    this.EstaCortado = false;
                }
            }
        }

        /// <summary>
        /// Copia el elemento
        /// </summary>
        public void Copiar()
        {
            EstaCortado = false;
            GestorGnoss.ElementoCortadoCopiado = this;
        }

        /// <summary>
        /// Devuelve o establece si el elemento está cortado
        /// </summary>
        public virtual bool EstaCortado
        {
            get
            {
                return mEstaCortado;
            }
            set
            {
                if (EstaCortado != value)
                {
                    mEstaCortado = value;

                    if (value)
                    {
                        GestorGnoss.ElementoCortadoCopiado = this;
                    }
                    else if (GestorGnoss.ElementoCortadoCopiado != null && GestorGnoss.ElementoCortadoCopiado == this)
                    {
                        GestorGnoss.ElementoCortadoCopiado = null;
                    }
                    #region Establezco la propiedad EstaCortadoPadre a todos los hijos

                    if (GestorGnoss != null)
                    {
                        this.GestorGnoss.OperacionConjunta = true;
                    }
                    List<IElementoGnoss> listaHijos = new List<IElementoGnoss>();
                    ElementoGnoss hijo;
                    listaHijos.AddRange(Hijos);

                    while (listaHijos.Count > 0)
                    {
                        hijo = (ElementoGnoss)listaHijos[0];
                        listaHijos.RemoveAt(0);

                        hijo.EstaCortadoPadre = value;
                        hijo.Notificar(new MensajeObservador(AccionesObservador.Invalidar, hijo));

                        listaHijos.AddRange(hijo.Hijos);
                    }
                    if (GestorGnoss != null)
                    {
                        this.GestorGnoss.OperacionConjunta = false; ;
                    }
                    #endregion

                    Notificar(new MensajeObservador(AccionesObservador.Invalidar, this));
                }
            }
        }

        /// <summary>
        /// Obtiene o establece si el padre está cortado
        /// </summary>
        public bool EstaCortadoPadre
        {
            get
            {
                return mEstaCortadoPadre;
            }
            set
            {
                mEstaCortadoPadre = value;
            }
        }

        /// <summary>
        /// Devuelve si el elemento se puede cortar o arrastrar
        /// </summary>
        public virtual bool SePuedeCortarArrastrar
        {
            get
            {
                return EsEditable;
            }
        }

        /// <summary>
        /// Mueve un elemento a la posición especificada
        /// </summary>
        /// <param name="pPadre">Padre</param>
        /// <param name="pPosicion">Posición</param>
        /// <returns>Resultado</returns>
        public virtual bool Mover(ICortarCopiarPegar pPadre, short pPosicion)
        {
            if (SePuedeCortarArrastrar && pPadre is ElementoGnoss && pPadre.AdmiteHijo(this))
            {
                ElementoGnoss padre = (ElementoGnoss)pPadre;

                //Corto el grupo
                Cortar();

                //IMPORTANTE: Sin esto no funciona al pegar ya que el índice lo pone mal sobre el mismo padre. Si estoy moviendo sobre el mismo padre y colocándolo antes de lo que estaba, pongo el desfase a 1 para pegar en una posición anterior. 
                short desfase = 0;

                if (Padre == padre && Indice < pPosicion)
                {
                    desfase = 1;
                }
                //Pego el grupo en el nuevo padre
                padre.Pegar(this, (short)(pPosicion - desfase));

                return true;
            }
            return false;
        }

        /// <summary>
        /// Determina si el elemento admite un hijo
        /// </summary>
        /// <param name="pHijoCandidato">Hijo candidato</param>
        /// <returns></returns>
        public virtual bool AdmiteHijo(IElementoGnoss pHijoCandidato)
        {
            return false;
        }

        /// <summary>
        /// Devuelve si se puede copiar
        /// </summary>
        public virtual bool SePuedeCopiar
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~ElementoGnoss()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            //impido que se finalice dos veces este objeto
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.disposed = true;

                try
                {
                    if (disposing)
                    {
                        //Libero todos los recursos administrados que he añadido a esta clase
                        if (mHijos != null)
                        {
                            foreach (ElementoGnoss elemento in this.mHijos)
                            {
                                elemento.Dispose();
                            }

                            mHijos.Clear();
                        }

                        if (this.mObservadores != null)
                        {
                            foreach (IObservador observador in this.mObservadores)
                            {
                                if ((observador != null) && (observador is IDisposable))
                                    ((IDisposable)observador).Dispose();
                            }
                            mObservadores.Clear();
                        }
                    }
                    //Libero todos los recursos nativos sin administrar que he añadido a esta clase

                    //Libero las variables grandes
                    this.mFilaElemento = null;
                    this.mGestionGnoss = null;
                    this.mHijos = null;
                    this.mPadre = null;
                    this.mObservadores = null;
                }
                catch (Exception e)
                {
                    mLoggingService.GuardarLogError(e);
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Argumentos para elemento Gnoss
    /// </summary>
    public class ElementoGnossEventArgs : EventArgs
    {
        #region Miembros privados

        /// <summary>
        /// ElementoGnoss
        /// </summary>
        private ElementoGnoss mElemento;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pElemento">ElementoGnoss</param>
        public ElementoGnossEventArgs(ElementoGnoss pElemento)
        {
            mElemento = pElemento;
        }

        #endregion

        #region Propiedades públicas

        /// <summary>
        /// Obtiene el elemento Gnoss
        /// </summary>
        public ElementoGnoss ElementoGnoss
        {
            get
            {
                return mElemento;
            }
        }

        #endregion
    }
}
