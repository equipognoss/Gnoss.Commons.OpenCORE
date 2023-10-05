using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.Tesauro;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces;
using Es.Riam.Interfaces.Observador;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Es.Riam.Gnoss.Elementos.Tesauro
{
    /// <summary>
    /// Gestor de tesauro
    /// </summary>
    [Serializable]
    public class GestionTesauro : GestionGnoss, ISerializable
    {
        #region Constantes

        /// <summary>
        /// Nombre para la categoría nueva
        /// </summary>
        public const string NOMBRE_CATEGORIA_NUEVA = "<Nueva categoría de tesauro>";

        /// <summary>
        /// Nombre de la categoría de tesauro pública
        /// </summary>
        public const string NOMBRE_CATEGORIA_PUBLICA = "Recursos públicos";

        /// <summary>
        /// Nombre de la categoría de tesauro privada
        /// </summary>
        public const string NOMBRE_CATEGORIA_PRIVADA = "Recursos privados";

        #endregion

        #region Miembros

        /// <summary>
        /// Gestor documental.
        /// </summary>
        public GestorDocumental mGestorDocumental;

        /// <summary>
        /// Lista de categorías de tesauro
        /// </summary>
        private SortedList<Guid, CategoriaTesauro> mListaCategoriasTesauro;


        private Dictionary<Guid, AD.EntityModel.Models.Tesauro.CategoriaTesauroPropiedades> mFilasPropiedadesPorCategoria = null;

        /// <summary>
        /// Lista de categorías de tesauro ordenas por nombre en funcion del idioma.
        /// </summary>
        private Dictionary<string, List<CategoriaTesauro>> mListaCategoriasTesauroOrdenadasPorNombre;

        /// <summary>
        /// Lista de categorías de tesauro de primer nivel
        /// </summary>
        private Dictionary<Guid, CategoriaTesauro> mListaCategoriasTesauroPrimerNivel = new Dictionary<Guid, CategoriaTesauro>();

        /// <summary>
        /// Lista de categorías de tesauro sugeridas (identificador, categoría) de primer nivel.
        /// </summary>
        public Dictionary<Guid, CategoriaTesauroSugerencia> mListaCategoriasSugeridasPrimerNivel;

        /// <summary>
        /// Lista de categorías de tesauro sugeridas (identificador, categoría) de primer nivel.
        /// </summary>
        public List<ElementoGnoss> mListaCategoriasTesauroYCategoriasSugeridasPrimerNivel;

        /// <summary>
        /// Fila del tesauro actual
        /// </summary>
        private AD.EntityModel.Models.Tesauro.Tesauro mFilaTesauroActual;

        /// <summary>
        /// Lista de los elementos hijos del gestor de tesauro más las sugerencias.
        /// </summary>
        private List<IElementoGnoss> mHijosConSugerencias;

        /// <summary>
        /// Número de elementos de la categoría.
        /// </summary>
        private Dictionary<Guid, int> mNumElementosCategoria;

        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pTesauroDW">DataWrapper de tesauro</param>
        public GestionTesauro(DataWrapperTesauro pTesauroDW, LoggingService loggingService, EntityContext entityContext)
            : base(pTesauroDW, loggingService)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            
            this.mListaCategoriasTesauro = new SortedList<Guid, CategoriaTesauro>();
            CargarCategorias();
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected GestionTesauro(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //mLoggingService = loggingService;
            //mEntityContext = entityContext;
            
            mGestorDocumental = (GestorDocumental)info.GetValue("GestorDocumental", typeof(GestorDocumental));

            this.mListaCategoriasTesauro = new SortedList<Guid, CategoriaTesauro>();
            CargarCategorias();
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el dataset de tesauro
        /// </summary>
        public DataWrapperTesauro TesauroDW
        {
            get
            {
                return (DataWrapperTesauro)base.DataWrapper;
            }
            set
            {
                if (value != DataWrapper)
                {
                    this.DataWrapper = value;

                    CargarCategorias();

                    if (((DataWrapperTesauro)DataWrapper).ListaTesauro.Count == 0)
                    {
                        AD.EntityModel.Models.Tesauro.Tesauro filaNueva = new AD.EntityModel.Models.Tesauro.Tesauro();

                        //filaNueva.WikiGnossID = (Guid)this.WikiGnossDS.WikiGnoss.Rows[0]["WikiGnossID"];
                        filaNueva.TesauroID = Guid.NewGuid();

                        ((DataWrapperTesauro)DataWrapper).ListaTesauro.Add(filaNueva);
                        mEntityContext.Tesauro.Add(filaNueva);
                    }
                    mFilaTesauroActual = ((DataWrapperTesauro)DataWrapper).ListaTesauro.FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// Obtiene la lista de categorías (identificador, categoría) que contiene el tesauro
        /// </summary>
        public SortedList<Guid, CategoriaTesauro> ListaCategoriasTesauro
        {
            get
            {
                return this.mListaCategoriasTesauro;
            }
        }

        /// <summary>
        /// Obtiene la lista de categorías ordenadas por nombre que contiene el tesauro en funcion del idioma
        /// </summary>
        public Dictionary<string, List<CategoriaTesauro>> ListaCategoriasTesauroOrdenadasPorNombre
        {
            get
            {
                if (mListaCategoriasTesauroOrdenadasPorNombre == null)
                {
                    mListaCategoriasTesauroOrdenadasPorNombre = new Dictionary<string, List<CategoriaTesauro>>();
                    SortedDictionary<string, List<CategoriaTesauro>> listaOrdenadaNombreCat = new SortedDictionary<string, List<CategoriaTesauro>>();
                    foreach (IdiomasCategorias idioma in Enum.GetValues(typeof(IdiomasCategorias)))
                    {

                        foreach (CategoriaTesauro cat in ListaCategoriasTesauro.Values)
                        {
                            if (listaOrdenadaNombreCat.ContainsKey(cat.Nombre[idioma.ToString()]))
                            {
                                listaOrdenadaNombreCat[cat.Nombre[idioma.ToString()]].Add(cat);
                            }
                            else
                            {
                                List<CategoriaTesauro> listaElemAux = new List<CategoriaTesauro>();
                                listaElemAux.Add(cat);
                                listaOrdenadaNombreCat.Add(cat.Nombre[idioma.ToString()], listaElemAux);
                            }
                        }

                        foreach (List<CategoriaTesauro> listaElem in listaOrdenadaNombreCat.Values)
                        {
                            if (mListaCategoriasTesauroOrdenadasPorNombre.ContainsKey(idioma.ToString()))
                            {
                                mListaCategoriasTesauroOrdenadasPorNombre[idioma.ToString()].AddRange(listaElem);
                            }
                            else
                            {
                                mListaCategoriasTesauroOrdenadasPorNombre.Add(idioma.ToString(), listaElem);
                            }
                        }
                    }
                }
                return this.mListaCategoriasTesauroOrdenadasPorNombre;
            }
        }

        /// <summary>
        /// Obtiene la lista de categorías de tesauro (identificador, categoría) de primer nivel
        /// </summary>
        public Dictionary<Guid, CategoriaTesauro> ListaCategoriasTesauroPrimerNivel
        {
            get
            {
                return this.mListaCategoriasTesauroPrimerNivel;
            }
        }

        /// <summary>
        /// Obtiene la lista de categorías de tesauro sugeridas (identificador, categoría) de primer nivel.
        /// </summary>
        public Dictionary<Guid, CategoriaTesauroSugerencia> ListaCategoriasSugeridasPrimerNivel
        {
            get
            {
                return mListaCategoriasSugeridasPrimerNivel;
            }
        }

        /// <summary>
        /// Obtiene la lista de categorías de tesauro sugeridas (identificador, categoría) de primer nivel.
        /// </summary>
        public List<ElementoGnoss> ListaCategoriasTesauroYCategoriasSugeridasPrimerNivel
        {
            get
            {
                if (mListaCategoriasTesauroYCategoriasSugeridasPrimerNivel == null)
                {
                    mListaCategoriasTesauroYCategoriasSugeridasPrimerNivel = new List<ElementoGnoss>();

                    foreach (CategoriaTesauro catTes in ListaCategoriasTesauroPrimerNivel.Values)
                    {
                        mListaCategoriasTesauroYCategoriasSugeridasPrimerNivel.Add(catTes);
                    }

                    if (ListaCategoriasSugeridasPrimerNivel != null)
                    {
                        foreach (CategoriaTesauroSugerencia catTesSug in ListaCategoriasSugeridasPrimerNivel.Values)
                        {
                            mListaCategoriasTesauroYCategoriasSugeridasPrimerNivel.Add(catTesSug);
                        }
                    }
                }
                return mListaCategoriasTesauroYCategoriasSugeridasPrimerNivel;
            }
        }

        /// <summary>
        /// Obtiene la fila del tesauro actual
        /// </summary>
        public AD.EntityModel.Models.Tesauro.Tesauro FilaTesauroActual
        {
            get
            {
                if (mFilaTesauroActual == null)
                {
                    if (TesauroDW.ListaTesauro.Count == 0)
                    {
                        AD.EntityModel.Models.Tesauro.Tesauro filaNueva = new AD.EntityModel.Models.Tesauro.Tesauro();

                        filaNueva.TesauroID = Guid.NewGuid();
                        TesauroDW.ListaTesauro.Add(filaNueva);
                        mEntityContext.Tesauro.Add(filaNueva);
                    }

                    mFilaTesauroActual = (AD.EntityModel.Models.Tesauro.Tesauro)TesauroDW.ListaTesauro.FirstOrDefault();
                }
                return this.mFilaTesauroActual;
            }
        }

        /// <summary>
        /// Obtiene la lista de los elementos hijos del gestor de tesauro
        /// </summary>
        public override List<IElementoGnoss> Hijos
        {
            get
            {
                if (mHijos == null)
                    CargarCategorias();
                return mHijos;
            }
        }

        /// <summary>
        /// Obtiene la lista de los elementos hijos del gestor de tesauro
        /// </summary>
        public List<IElementoGnoss> HijosConSugerencias
        {
            get
            {
                return mHijosConSugerencias;
            }
        }

        /// <summary>
        /// Obtiene o establece el nombre del gestor de tesauro
        /// </summary>
        public override string Nombre
        {
            get
            {
                return "Tesauro";
            }
            set
            {
            }
        }

        /// <summary>
        /// Devuelve si el gestor de tesauro es editable
        /// </summary>
        public override bool EsEditable
        {
            get
            {
                return PuedeEditar;
            }
        }

        /// <summary>
        /// Obtiene si el gestor de tesauro se puede eliminar
        /// </summary>
        public override bool SePuedeEliminar
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Devuelve o establece el gestor documental
        /// </summary>
        public GestorDocumental GestorDocumental
        {
            get
            {
                return mGestorDocumental;
            }
            set
            {
                this.mGestorDocumental = value;
            }
        }

        /// <summary>
        /// Obtiene el identificador del tesauro actual
        /// </summary>
        public Guid TesauroActualID
        {
            get
            {
                return (TesauroDW.ListaTesauro.FirstOrDefault()).TesauroID;
            }
        }

        /// <summary>
        /// Número de elementos de la categoría.
        /// </summary>
        public Dictionary<Guid, int> NumElementosCategoria
        {
            get
            {
                if (mNumElementosCategoria == null)
                {
                    mNumElementosCategoria = new Dictionary<Guid, int>();
                }

                return mNumElementosCategoria;
            }
        }

        #region Tesauro Usuario y Organización

        /// <summary>
        /// Devuelve el identificador de la categoría publica del tesauro actual.
        /// </summary>
        public Guid CategoriaPublicaID
        {
            get
            {
                Guid clavePublica = Guid.Empty;
                if (TesauroDW.ListaTesauroUsuario.Count > 0)
                {
                    clavePublica = (TesauroDW.ListaTesauroUsuario.FirstOrDefault()).CategoriaTesauroPublicoID.Value;
                }
                else if (TesauroDW.ListaTesauroOrganizacion.Count > 0)
                {
                    clavePublica = (TesauroDW.ListaTesauroOrganizacion.FirstOrDefault()).CategoriaTesauroPublicoID.Value;
                }
                return clavePublica;
            }
        }

        /// <summary>
        /// Devuelve el identificador de la categoría privada del tesauro actual.
        /// </summary>
        public Guid CategoriaPrivadaID
        {
            get
            {
                Guid clavePrivada = Guid.Empty;
                if (TesauroDW.ListaTesauroUsuario.Count > 0)
                {
                    clavePrivada = (TesauroDW.ListaTesauroUsuario.FirstOrDefault()).CategoriaTesauroPrivadoID.Value;
                }
                else if (TesauroDW.ListaTesauroOrganizacion.Count > 0)
                {
                    clavePrivada = (TesauroDW.ListaTesauroOrganizacion.FirstOrDefault()).CategoriaTesauroPrivadoID.Value;
                }
                return clavePrivada;
            }
        }

        /// <summary>
        /// Devuelve el identificador de la categoría privada del tesauro actual.
        /// </summary>
        public Guid CategoriaImagenesID
        {
            get
            {
                Guid claveImagenes = Guid.Empty;
                if (TesauroDW.ListaTesauroUsuario.Count > 0 && (TesauroDW.ListaTesauroUsuario.FirstOrDefault()).CategoriaTesauroMisImagenesID.HasValue)
                {
                    claveImagenes = (TesauroDW.ListaTesauroUsuario.FirstOrDefault()).CategoriaTesauroMisImagenesID.Value;
                }
                //else if (TesauroDS.TesauroOrganizacion.Rows.Count > 0)
                //{
                //    claveImagenes = ((TesauroDS.TesauroOrganizacionRow)TesauroDS.TesauroOrganizacion.Rows[0]).CategoriaTesauroMisImagenesID;
                //}
                return claveImagenes;
            }
        }

        /// <summary>
        /// Devuelve el identificador de la categoría privada del tesauro actual.
        /// </summary>
        public Guid CategoriaVideosID
        {
            get
            {
                Guid claveVideos = Guid.Empty;
                if (TesauroDW.ListaTesauroUsuario.Count > 0 && (TesauroDW.ListaTesauroUsuario.FirstOrDefault()).CategoriaTesauroMisVideosID.HasValue)
                {
                    claveVideos = (TesauroDW.ListaTesauroUsuario.FirstOrDefault()).CategoriaTesauroMisVideosID.Value;
                }
                //else if (TesauroDS.TesauroOrganizacion.Rows.Count > 0)
                //{
                //    claveVideos = ((TesauroDS.TesauroOrganizacionRow)TesauroDS.TesauroOrganizacion.Rows[0])..CategoriaTesauroMisVideosID;
                //}
                return claveVideos;
            }
        }

        /// <summary>
        /// TRUE si el tesauro pertenece a un usuario.
        /// </summary>
        public bool TesauroDeUsuario
        {
            get
            {
                return (TesauroDW.ListaTesauroUsuario.Where(item => item.TesauroID.Equals(TesauroActualID)).Count() > 0);
            }
        }

        #endregion

        #endregion

        #region Métodos generales


        public void RecargarGestor()
        {
            this.mListaCategoriasTesauro = new SortedList<Guid, CategoriaTesauro>();
            CargarCategorias();
        }

        /// <summary>
        /// Crea un nuevo tesauro para el usuario pasado por parámetro
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pNombreCategoriaPublica">Nombre de la categoría de tesauro pública</param>
        /// <param name="pNombreCategoriaPrivada">Nombre de la categoría de tesauro privada</param>
        /// <returns>Nueva fila de tesauro</returns>
        public AD.EntityModel.Models.Tesauro.Tesauro AgregarTesauroUsuario(Guid pUsuarioID, string pNombreCategoriaPublica, string pNombreCategoriaPrivada)
        {
            //Tesauro
            AD.EntityModel.Models.Tesauro.Tesauro filaTesauro = new AD.EntityModel.Models.Tesauro.Tesauro();
            filaTesauro.TesauroID = Guid.NewGuid();

            TesauroDW.ListaTesauro.Add(filaTesauro);
            mEntityContext.Tesauro.Add(filaTesauro);

            //TesauroUsuario
            Guid categoriaPublica = Guid.NewGuid();
            Guid categoriaPrivada = Guid.NewGuid();
            TesauroUsuario filaTesauroUsuario = new TesauroUsuario();
            filaTesauroUsuario.Tesauro = filaTesauro;
            filaTesauroUsuario.UsuarioID = pUsuarioID;
            filaTesauroUsuario.CategoriaTesauroPublicoID = categoriaPublica;
            filaTesauroUsuario.CategoriaTesauroPrivadoID = categoriaPrivada;

            TesauroDW.ListaTesauroUsuario.Add(filaTesauroUsuario);
            mEntityContext.TesauroUsuario.Add(filaTesauroUsuario);

            //Categoría del tesauro de usuario
            AD.EntityModel.Models.Tesauro.CategoriaTesauro filaCatTesPub = new AD.EntityModel.Models.Tesauro.CategoriaTesauro();
            filaCatTesPub.CategoriaTesauroID = categoriaPublica;
            filaCatTesPub.Nombre = pNombreCategoriaPublica;
            filaCatTesPub.Orden = 0;
            filaCatTesPub.TesauroID = filaTesauro.TesauroID;
            filaCatTesPub.NumeroRecursos = 0;
            filaCatTesPub.NumeroPreguntas = 0;
            filaCatTesPub.NumeroDebates = 0;
            filaCatTesPub.NumeroDafos = 0;
            filaCatTesPub.TieneFoto = false;
            filaCatTesPub.VersionFoto = 0;
            filaCatTesPub.Estructurante = 0;
            filaCatTesPub.Tesauro = filaTesauro;
            TesauroDW.ListaCategoriaTesauro.Add(filaCatTesPub);
            mEntityContext.CategoriaTesauro.Add(filaCatTesPub);

            AD.EntityModel.Models.Tesauro.CategoriaTesauro filaCatTesPriv = new AD.EntityModel.Models.Tesauro.CategoriaTesauro();
            filaCatTesPriv.CategoriaTesauroID = categoriaPrivada;
            filaCatTesPriv.Nombre = pNombreCategoriaPrivada;
            filaCatTesPriv.Orden = 1;
            filaCatTesPriv.TesauroID = filaTesauro.TesauroID;
            filaCatTesPriv.NumeroRecursos = 0;
            filaCatTesPriv.NumeroPreguntas = 0;
            filaCatTesPriv.NumeroDebates = 0;
            filaCatTesPriv.NumeroDafos = 0;
            filaCatTesPriv.TieneFoto = false;
            filaCatTesPriv.VersionFoto = 0;
            filaCatTesPriv.Estructurante = 0;
            TesauroDW.ListaCategoriaTesauro.Add(filaCatTesPriv);
            mEntityContext.CategoriaTesauro.Add(filaCatTesPriv);

            return filaTesauro;
        }

        /// <summary>
        /// Crea un nuevo tesauro para la organización pasada por parámetro
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pNombreCategoriaPublica">Nombre de la categoría de tesauro pública</param>
        /// <param name="pNombreCategoriaPrivada">Nombre de la categoría de tesauro privada</param>
        /// <returns>Nueva fila de tesauro</returns>
        public AD.EntityModel.Models.Tesauro.Tesauro AgregarTesauroOrganizacion(Guid pOrganizacionID, string pNombreCategoriaPublica, string pNombreCategoriaPrivada)
        {
            //Tesauro
            AD.EntityModel.Models.Tesauro.Tesauro filaTesauro = new AD.EntityModel.Models.Tesauro.Tesauro();
            filaTesauro.TesauroID = Guid.NewGuid();

            TesauroDW.ListaTesauro.Add(filaTesauro);
            mEntityContext.Tesauro.Add(filaTesauro);

            //TesauroOrganizacion
            Guid categoriaPublica = Guid.NewGuid();
            Guid categoriaPrivada = Guid.NewGuid();
            AD.EntityModel.Models.Tesauro.TesauroOrganizacion filaTesauroOrganizacion = new AD.EntityModel.Models.Tesauro.TesauroOrganizacion();
            filaTesauroOrganizacion.Tesauro = filaTesauro;
            filaTesauroOrganizacion.OrganizacionID = pOrganizacionID;
            filaTesauroOrganizacion.CategoriaTesauroPublicoID = categoriaPublica;
            filaTesauroOrganizacion.CategoriaTesauroPrivadoID = categoriaPrivada;

            TesauroDW.ListaTesauroOrganizacion.Add(filaTesauroOrganizacion);
            mEntityContext.TesauroOrganizacion.Add(filaTesauroOrganizacion);

            //Categoría del tesauro de usuario
            AD.EntityModel.Models.Tesauro.CategoriaTesauro filaCatTesPub = new AD.EntityModel.Models.Tesauro.CategoriaTesauro();
            filaCatTesPub.CategoriaTesauroID = categoriaPublica;
            filaCatTesPub.Nombre = pNombreCategoriaPublica;
            filaCatTesPub.Orden = 0;
            filaCatTesPub.TesauroID = filaTesauro.TesauroID;
            filaCatTesPub.NumeroRecursos = 0;
            filaCatTesPub.NumeroPreguntas = 0;
            filaCatTesPub.NumeroDebates = 0;
            filaCatTesPub.NumeroDafos = 0;
            filaCatTesPub.TieneFoto = false;
            filaCatTesPub.VersionFoto = 0;
            filaCatTesPub.Estructurante = 0;

            TesauroDW.ListaCategoriaTesauro.Add(filaCatTesPub);
            mEntityContext.CategoriaTesauro.Add(filaCatTesPub);

            AD.EntityModel.Models.Tesauro.CategoriaTesauro filaCatTesPriv = new AD.EntityModel.Models.Tesauro.CategoriaTesauro();
            filaCatTesPriv.CategoriaTesauroID = categoriaPrivada;
            filaCatTesPriv.Nombre = pNombreCategoriaPrivada;
            filaCatTesPriv.Orden = 1;
            filaCatTesPriv.TesauroID = filaTesauro.TesauroID;
            filaCatTesPriv.NumeroRecursos = 0;
            filaCatTesPriv.NumeroPreguntas = 0;
            filaCatTesPriv.NumeroDebates = 0;
            filaCatTesPriv.NumeroDafos = 0;
            filaCatTesPriv.TieneFoto = false;
            filaCatTesPriv.VersionFoto = 0;
            filaCatTesPriv.Estructurante = 0;

            TesauroDW.ListaCategoriaTesauro.Add(filaCatTesPriv);
            mEntityContext.CategoriaTesauro.Add(filaCatTesPriv);

            return filaTesauro;
        }

        /// <summary>
        /// Eliminar el tesauro cuyo identificador se pasa como parámetro
        /// </summary>
        /// <param name="pTesauroID">Identificador del tesauro</param>
        public void EliminarTesauro(Guid pTesauroID)
        {
            // Eliminar tesauro de usuario
            foreach (TesauroUsuario fila in TesauroDW.ListaTesauroUsuario.Where(item => item.TesauroID.Equals(pTesauroID)).ToList())
            {
                TesauroDW.ListaTesauroUsuario.Remove(fila);
                mEntityContext.EliminarElemento(fila);
            }

            // Eliminar tesauro de proyecto
            foreach (TesauroProyecto fila in TesauroDW.ListaTesauroProyecto.Where(item => item.TesauroID.Equals(pTesauroID)).ToList())
            {
                TesauroDW.ListaTesauroProyecto.Remove(fila);
                mEntityContext.EliminarElemento(fila);
            }

            // Eliminar tesauro de organización
            foreach (TesauroOrganizacion fila in TesauroDW.ListaTesauroOrganizacion.Where(item => item.TesauroID.Equals(pTesauroID)).ToList())
            {
                TesauroDW.ListaTesauroOrganizacion.Remove(fila);
                mEntityContext.EliminarElemento(fila);
            }

            // Eliminar agregaciones de categorias de tesauro
            foreach (CatTesauroAgCatTesauro fila in TesauroDW.ListaCatTesauroAgCatTesauro.Where(item => item.TesauroID.Equals(pTesauroID)).ToList())
            {
                TesauroDW.ListaCatTesauroAgCatTesauro.Remove(fila);
                mEntityContext.EliminarElemento(fila);
            }

            // Eliminar categorias de tesauro
            foreach (AD.EntityModel.Models.Tesauro.CategoriaTesauro fila in TesauroDW.ListaCategoriaTesauro.Where(item => item.TesauroID.Equals(pTesauroID)).ToList())
            {
                // David: Borrar las vinculaciones con versiones obsoletas de documentos
                if (GestorDocumental != null)
                {
                    foreach (AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro filaVinculo in GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(doc => doc.CategoriaTesauroID.Equals(fila.CategoriaTesauroID)).ToList())
                    {
                        GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Remove(filaVinculo);
                        mEntityContext.EliminarElemento(filaVinculo);
                    }
                }
                TesauroDW.ListaCategoriaTesauro.Remove(fila);
                mEntityContext.EliminarElemento(fila);
            }

            // Eliminar la fila de tesauro
            AD.EntityModel.Models.Tesauro.Tesauro tesauro = TesauroDW.ListaTesauro.FirstOrDefault(item => item.TesauroID.Equals(pTesauroID));
            TesauroDW.ListaTesauro.Remove(tesauro);
            mEntityContext.EliminarElemento(tesauro);

        }



        /// <summary>
        /// Determina si el gestor de tesauro admite como hijo al elemento pasado por parámetro
        /// </summary>
        /// <param name="pHijoCandidato">Elemento candidato a ser hijo</param>
        /// <returns>TRUE si el gestor de tesauro admite como hijo al elemento</returns>
        public override bool AdmiteHijo(IElementoGnoss pHijoCandidato)
        {
            if ((pHijoCandidato != null) && (pHijoCandidato is CategoriaTesauro))
                return true;
            else
                return false;
        }

        internal Dictionary<Guid, List<object>> ListaCategoriasInferioresPorCategoriaID = new Dictionary<Guid, List<object>>();

        /// <summary>
        /// Carga las categorías de tesauro
        /// </summary>
        public void CargarCategorias()
        {
            mListaCategoriasTesauro.Clear();
            mListaCategoriasTesauroPrimerNivel.Clear();
			ListaCategoriasInferioresPorCategoriaID.Clear();
			mHijos = new List<IElementoGnoss>();

            var categoriasInferiores = this.TesauroDW.ListaCatTesauroAgCatTesauro.ToDictionary(item => item.CategoriaInferiorID);

            foreach (AD.EntityModel.Models.Tesauro.CategoriaTesauro filaCategoria in this.TesauroDW.ListaCategoriaTesauro.OrderBy(cat => cat.Orden))
            {
                CatTesauroAgCatTesauro catTesauroAgCatTesauro = null;
                if (categoriasInferiores.ContainsKey(filaCategoria.CategoriaTesauroID))
                {
                    catTesauroAgCatTesauro = categoriasInferiores[filaCategoria.CategoriaTesauroID];
                }
                CatTesauroCompartida catTesauroCompartida = null;
                if (catTesauroAgCatTesauro != null)
                {
                    catTesauroCompartida = this.TesauroDW.ListaCatTesauroCompartida.FirstOrDefault(cat2 => cat2.CategoriaOrigenID.Equals(filaCategoria.CategoriaTesauroID) && cat2.CategoriaSupDestinoID.HasValue);
                }

                if (catTesauroAgCatTesauro == null && catTesauroCompartida == null)
                {
                    //Es categoría de primer nivel
                    CategoriaTesauro categoria = new CategoriaTesauro(filaCategoria, this, mLoggingService);
                    this.mListaCategoriasTesauroPrimerNivel.Add(filaCategoria.CategoriaTesauroID, categoria);
                    this.mListaCategoriasTesauro.Add(filaCategoria.CategoriaTesauroID, categoria);

                    categoria.Padre = this;

                    this.Hijos.Add(categoria);
                }
                else
                {
                    CategoriaTesauro categoria = new CategoriaTesauro(filaCategoria, this, mLoggingService);
                    this.mListaCategoriasTesauro.Add(filaCategoria.CategoriaTesauroID, categoria);

                    if (catTesauroAgCatTesauro != null)
                    {
                        if (!ListaCategoriasInferioresPorCategoriaID.ContainsKey(catTesauroAgCatTesauro.CategoriaSuperiorID))
                        {
                            ListaCategoriasInferioresPorCategoriaID.Add(catTesauroAgCatTesauro.CategoriaSuperiorID, new List<object>());
                        }
                        ListaCategoriasInferioresPorCategoriaID[catTesauroAgCatTesauro.CategoriaSuperiorID].Add(catTesauroAgCatTesauro);
                    }
                    else if (catTesauroCompartida != null)
                    {
                        if (!ListaCategoriasInferioresPorCategoriaID.ContainsKey(catTesauroCompartida.CategoriaSupDestinoID.Value))
                        {
                            ListaCategoriasInferioresPorCategoriaID.Add(catTesauroCompartida.CategoriaSupDestinoID.Value, new List<object>());
                        }
                        ListaCategoriasInferioresPorCategoriaID[catTesauroCompartida.CategoriaSupDestinoID.Value].Add(catTesauroCompartida);
                    }
                }
            }

            foreach (Guid categoriaID in ListaCategoriasInferioresPorCategoriaID.Keys)
            {
                if (mListaCategoriasTesauro.ContainsKey(categoriaID))
                {
                    mListaCategoriasTesauro[categoriaID].CargarSubcategorias();
                }                
            }
        }

        /// <summary>
        /// Carga las categorias sugeridas de una comunidad.
        /// </summary>
        public void CargarCategoriasSugerencia()
        {
            mListaCategoriasSugeridasPrimerNivel = new Dictionary<Guid, CategoriaTesauroSugerencia>();
            mHijosConSugerencias = new List<IElementoGnoss>();
            mHijosConSugerencias.AddRange(Hijos);

            foreach (AD.EntityModel.Models.Tesauro.CategoriaTesauroSugerencia filaCatSug in TesauroDW.ListaCategoriaTesauroSugerencia)
            {
                if (filaCatSug.Estado == (short)EstadoSugerenciaCatTesauro.Espera)
                {
                    CategoriaTesauroSugerencia catSugerencia = new CategoriaTesauroSugerencia(filaCatSug, this, mLoggingService);

                    if (filaCatSug.CategoriaTesauroPadreID == null)
                    {
                        mHijosConSugerencias.Add(catSugerencia);
                        mListaCategoriasSugeridasPrimerNivel.Add(filaCatSug.SugerenciaID, catSugerencia);
                    }
                    else
                    {
                        if (ListaCategoriasTesauro.ContainsKey(filaCatSug.CategoriaTesauroPadreID.Value))
                        {
                            ListaCategoriasTesauro[filaCatSug.CategoriaTesauroPadreID.Value].CategoriasSugeridasHijas.Add(filaCatSug.SugerenciaID, catSugerencia);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Agrega una categoría al tesauro
        /// </summary>
        /// <param name="pNombre">Nombre de la nueva categoría</param>
        /// <returns>Nueva categoría de tesauro</returns>
        public CategoriaTesauro AgregarCategoria(string pNombre, short pOrden)
        {
            CategoriaTesauro nuevaCategoria = null;

            //Creo la fila nueva de categoría
            AD.EntityModel.Models.Tesauro.CategoriaTesauro filaNuevaCategoria = new AD.EntityModel.Models.Tesauro.CategoriaTesauro();

            filaNuevaCategoria.CategoriaTesauroID = Guid.NewGuid();
            filaNuevaCategoria.Nombre = pNombre;
            filaNuevaCategoria.Tesauro = FilaTesauroActual;
            filaNuevaCategoria.Orden = pOrden;
            filaNuevaCategoria.NumeroRecursos = 0;
            filaNuevaCategoria.NumeroPreguntas = 0;
            filaNuevaCategoria.NumeroDebates = 0;
            filaNuevaCategoria.NumeroDafos = 0;
            filaNuevaCategoria.TieneFoto = false;
            filaNuevaCategoria.VersionFoto = 0;
            filaNuevaCategoria.Estructurante = 0;
            this.TesauroDW.ListaCategoriaTesauro.Add(filaNuevaCategoria);
            mEntityContext.CategoriaTesauro.Add(filaNuevaCategoria);

            nuevaCategoria = new CategoriaTesauro(filaNuevaCategoria, this, mLoggingService);

            this.mListaCategoriasTesauro.Add(nuevaCategoria.Clave, nuevaCategoria);

            return nuevaCategoria;
        }

        /// <summary>
        /// Agrega una subcategoría a una categoría existente
        /// </summary>
        /// <param name="pCategoriaSuperior">Categoría de tesauro superior</param>
        /// <param name="pNombre">Nombre de la nueva categoría</param>
        /// <returns>Nueva categoría de tesauro</returns>
        public CategoriaTesauro AgregarSubcategoria(CategoriaTesauro pCategoriaSuperior, string pNombre)
        {
            //Creo la nueva categoría
            CategoriaTesauro nuevaCategoria = AgregarCategoria(pNombre, (short)pCategoriaSuperior.Hijos.Count);

            // David: Pongo esta linea porque desde el cliente en las pantallas con documentación, es posible tener varios tesauros creados
            //        y surgia el error de que las categorias se asignaban a tesauros diferentes
            nuevaCategoria.FilaCategoria.Tesauro = pCategoriaSuperior.FilaCategoria.Tesauro;

            //La agrego a su categoría superior
            AgregarSubcategoriaACategoria(nuevaCategoria, pCategoriaSuperior);


            pCategoriaSuperior.Notificar(new MensajeObservador(AccionesObservador.Aniadir, nuevaCategoria));

            return nuevaCategoria;
        }

        /// <summary>
        /// Crea una nueva categoría de tesauro de primer nivel
        /// </summary>
        /// <param name="pNombre">Nombre para la nueva categoría de tesauro</param>
        /// <returns>Nueva categoría de tesauro</returns>
        public CategoriaTesauro AgregarCategoriaPrimerNivel(string pNombre)
        {
            CategoriaTesauro categoriaNueva = AgregarCategoria(pNombre, 0);

            categoriaNueva.FilaCategoria.Orden = (short)this.Hijos.Count;

            this.Hijos.Add(categoriaNueva);
            categoriaNueva.Padre = this;

            this.Notificar(new MensajeObservador(AccionesObservador.Aniadir, categoriaNueva));

            return categoriaNueva;
        }

        /// <summary>
        /// Agrega una subcategoría a la categoria superior
        /// </summary>
        /// <param name="pSubCategoria">Subcategoría de tesauro</param>
        /// <param name="pCategoriaSuperior">Categoría de tesauro superior</param>
        /// <returns>El DataRow de agregación creado</returns>
        public CatTesauroAgCatTesauro AgregarSubcategoriaACategoria(CategoriaTesauro pSubCategoria, CategoriaTesauro pCategoriaSuperior)
        {
            CatTesauroAgCatTesauro filaNuevaAgregacion = this.TesauroDW.ListaCatTesauroAgCatTesauro.FirstOrDefault(cat => cat.TesauroID.Equals(pSubCategoria.FilaCategoria.TesauroID) && cat.CategoriaSuperiorID.Equals(pCategoriaSuperior.FilaCategoria.CategoriaTesauroID) && cat.CategoriaInferiorID.Equals(pSubCategoria.FilaCategoria.CategoriaTesauroID));

            if (filaNuevaAgregacion == null)
            {
                //Creo la relación entre la categoría superior y la subcategoría
                filaNuevaAgregacion = new CatTesauroAgCatTesauro();

                filaNuevaAgregacion.CategoriaSuperiorID = pCategoriaSuperior.FilaCategoria.CategoriaTesauroID;
                filaNuevaAgregacion.CategoriaInferiorID = pSubCategoria.FilaCategoria.CategoriaTesauroID;
                filaNuevaAgregacion.Orden = (short)pCategoriaSuperior.Hijos.Count;                
                filaNuevaAgregacion.TesauroID = pSubCategoria.FilaCategoria.TesauroID;
                filaNuevaAgregacion.CategoriaTesuaro1 = pCategoriaSuperior.FilaCategoria;
                filaNuevaAgregacion.CategoriaTesauro = pSubCategoria.FilaCategoria;

                pCategoriaSuperior.FilaCategoria.CatTesauroAgCatTesauroSuperior.Add(filaNuevaAgregacion);
                pSubCategoria.FilaCategoria.CatTesauroAgCatTesauroInferior.Add(filaNuevaAgregacion);

                if (!this.TesauroDW.ListaCatTesauroAgCatTesauro.Any(cat => cat.TesauroID.Equals(filaNuevaAgregacion.TesauroID) && cat.CategoriaSuperiorID.Equals(filaNuevaAgregacion.CategoriaSuperiorID) && cat.CategoriaInferiorID.Equals(filaNuevaAgregacion.CategoriaInferiorID)))
                {
                    this.TesauroDW.ListaCatTesauroAgCatTesauro.Add(filaNuevaAgregacion);
                    mEntityContext.CatTesauroAgCatTesauro.Add(filaNuevaAgregacion);
                }
            }

            pCategoriaSuperior.Hijos.Add(pSubCategoria);
            pSubCategoria.Padre = pCategoriaSuperior;
            pSubCategoria.FilaAgregacion = filaNuevaAgregacion;

            return filaNuevaAgregacion;
        }

        /// <summary>
        /// Agrega una sugerencia de categoría debajo de una ya existente.
        /// </summary>
        /// <param name="pCategoriaPadreID">Categoría padre bajo la que se sugire como hija una nueva o Guid.Empty si está en la raiz</param>
        /// <param name="pTesauroID">Tesauro en el que se sugiere</param>
        /// <param name="pNombre">Nombre de la categoría sugerida</param>
        /// <param name="pIdentidadSugiere">Identidad que sugiere la categoría</param>
        public void AgregarSugerenciaCategoria(Guid pCategoriaPadreID, Guid pTesauroID, string pNombre, Guid pIdentidadSugiere)
        {
            AD.EntityModel.Models.Tesauro.CategoriaTesauroSugerencia filaNuevaCatSug = new AD.EntityModel.Models.Tesauro.CategoriaTesauroSugerencia();
            if (pCategoriaPadreID != Guid.Empty)
            {//La sugerencia cuelga de la raiz.
                filaNuevaCatSug.CategoriaTesauroPadreID = pCategoriaPadreID;
                filaNuevaCatSug.TesauroCatPadreID = pTesauroID;
            }
            filaNuevaCatSug.Estado = (short)EstadoSugerenciaCatTesauro.Espera;
            filaNuevaCatSug.IdentidadID = pIdentidadSugiere;
            filaNuevaCatSug.Nombre = pNombre;
            filaNuevaCatSug.SugerenciaID = Guid.NewGuid();
            filaNuevaCatSug.TesauroSugerenciaID = pTesauroID;

            TesauroDW.ListaCategoriaTesauroSugerencia.Add(filaNuevaCatSug);
            mEntityContext.CategoriaTesauroSugerencia.Add(filaNuevaCatSug);
        }

        /// <summary>
        /// Quita de la lista de subcategorías de la categoría superior una categoría
        /// </summary>
        /// <param name="pSubCategoria">Subcategoría a quitar</param>
        /// <param name="pCategoriaSuperior">Categoría superior que contiene la subcategoría</param>
        public void DesasignarSubcategoriaDeCategoria(CategoriaTesauro pSubCategoria, CategoriaTesauro pCategoriaSuperior)
        {
            CatTesauroAgCatTesauro catTesauroAgCatTesauro = this.TesauroDW.ListaCatTesauroAgCatTesauro.Where(item => item.CategoriaSuperiorID.Equals(pCategoriaSuperior.FilaCategoria.CategoriaTesauroID) && item.CategoriaInferiorID.Equals(pSubCategoria.FilaCategoria.CategoriaTesauroID)).FirstOrDefault();

            if (catTesauroAgCatTesauro != null)
            {
                pSubCategoria.FilaCategoria.CatTesauroAgCatTesauroInferior.Remove(catTesauroAgCatTesauro);
                mEntityContext.EliminarElemento(catTesauroAgCatTesauro);
                TesauroDW.ListaCatTesauroAgCatTesauro.Remove(catTesauroAgCatTesauro);
            }
        }

        /// <summary>
        /// Elimina la categoría del tesauro y sus hijos
        /// </summary>
        /// <param name="pCategoria">Categoría de tesauro a eliminar</param>
        public void EliminarCategoriaEHijos(CategoriaTesauro pCategoria)
        {
            List<IElementoGnoss> listaHijos = new List<IElementoGnoss>();
            listaHijos.AddRange(pCategoria.Hijos);

            foreach (CategoriaTesauro hijo in listaHijos)
            {
                EliminarCategoriaEHijos(hijo);
            }
            EliminarCategoria(pCategoria);
        }

        /// <summary>
        /// Elimina la categoría del tesauro
        /// </summary>
        /// <param name="pCategoria">Categoría a eliminar</param>
        public void EliminarCategoria(CategoriaTesauro pCategoria)
        {
            pCategoria.Notificar(new MensajeObservador(AccionesObservador.Eliminar, pCategoria));

            short orden = pCategoria.Indice;

            pCategoria.Padre.Hijos.Remove(pCategoria);
            ((ElementoGnoss)pCategoria.Padre).ActualizarOrdenHijos(orden, -1);

            foreach (CatTesauroAgCatTesauro elemento in TesauroDW.ListaCatTesauroAgCatTesauro.Where(item => item.CategoriaSuperiorID.Equals(pCategoria.FilaCategoria.CategoriaTesauroID)).ToList())
            {
                TesauroDW.ListaCatTesauroAgCatTesauro.Remove(elemento);
                mEntityContext.EliminarElemento(elemento);
            }

            foreach (CatTesauroAgCatTesauro elemento in TesauroDW.ListaCatTesauroAgCatTesauro.Where(item => item.CategoriaInferiorID.Equals(pCategoria.FilaCategoria.CategoriaTesauroID)).ToList())
            {
                TesauroDW.ListaCatTesauroAgCatTesauro.Remove(elemento);
                mEntityContext.EliminarElemento(elemento);
            }

            //Borra las sugerencias que cuelgan de la categoría (aceptadas o sin aceptar)

            foreach (AD.EntityModel.Models.Tesauro.CategoriaTesauroSugerencia filaSugerencia in TesauroDW.ListaCategoriaTesauroSugerencia.ToList())
            {
                if (filaSugerencia.CategoriaTesauroPadreID != null && filaSugerencia.CategoriaTesauroPadreID == pCategoria.Clave)
                {
                    TesauroDW.ListaCategoriaTesauroSugerencia.Remove(filaSugerencia);
                }
            }

            // David: Borrar las vinculaciones con versiones anteriores a documentos
            if (GestorDocumental != null)
            {
                foreach (AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro filaVinculo in GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(doc => doc.CategoriaTesauroID.Equals(pCategoria.Clave)).ToList())
                {
                    GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Remove(filaVinculo);
                    mEntityContext.EliminarElemento(filaVinculo);
                }
            }

            this.mListaCategoriasTesauro.Remove(pCategoria.Clave);

            TesauroDW.ListaCategoriaTesauro.Remove(pCategoria.FilaCategoria);
            mEntityContext.EliminarElemento(pCategoria.FilaCategoria);
        }

        /// <summary>
        /// Comprueba si al categoría está vinculada
        /// </summary>
        /// <param name="pCategoria">Categoría a comprobar</param>
        /// <returns>TRUE si la categoría está vinculada, FALSE en caso contrario</returns>
        public bool CategoriaEHijasVinculada(CategoriaTesauro pCategoria)
        {
            bool vinculadosHijos = false;

            foreach (CategoriaTesauro hijo in pCategoria.Hijos)
            {
                vinculadosHijos = (vinculadosHijos || CategoriaEHijasVinculada(hijo));
            }
            return (pCategoria.Documentos.Count > 0 || vinculadosHijos);
        }

        /// <summary>
        /// Carga el nº de elementos de cada categoría.
        /// </summary>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        public void CargarNumElementosCategorias(FacetadoDS pFacetadoDS)
        {
            mNumElementosCategoria = new Dictionary<Guid, int>();

            foreach (DataRow fila in pFacetadoDS.Tables["skos:ConceptID"].Rows)
            {
                string categoriaID = ((string)fila[0]).Substring(((string)fila[0]).LastIndexOf("/") + 1);

                if (mNumElementosCategoria.ContainsKey(new Guid(categoriaID)))
                {
                    mNumElementosCategoria[new Guid(categoriaID)] = mNumElementosCategoria[new Guid(categoriaID)] + int.Parse((string)fila[1]);
                }
                else
                {
                    mNumElementosCategoria.Add(new Guid(categoriaID), int.Parse((string)fila[1]));
                }
            }
        }

        /// <summary>
        /// Elimina las categorías que no están permitidas por un tipo de documento y su ontología si la tiene.
        /// </summary>
        /// <param name="pTipoDoc">Tipo de documento para revisar</param>
        /// <param name="pOntologiaID">ID de la ontología a revisar o NULL si no hay ontología</param>
        public void EliminarCategoriasNoPermitidasPorTipoDoc(short pTipoDoc, Guid? pOntologiaID)
        {
            if (TesauroDW.ListaCatTesauroPermiteTipoRec.Where(item => item.TipoRecurso.Equals((short)pTipoDoc)).Count() > 0)
            {
                List<Guid> catMantener = new List<Guid>();

                List<CatTesauroPermiteTipoRec> listaProvisional = TesauroDW.ListaCatTesauroPermiteTipoRec.Where(item => item.TipoRecurso.Equals((short)pTipoDoc)).ToList();

                foreach (CatTesauroPermiteTipoRec fila in listaProvisional)
                {
                    if (!pOntologiaID.HasValue || fila.OntologiasID == null || fila.OntologiasID.Contains(pOntologiaID.Value.ToString()))
                    {
                        catMantener.Add(fila.CategoriaTesauroID);
                    }
                }

                if (catMantener.Count > 0)
                {
                    List<IElementoGnoss> hijosAux = new List<IElementoGnoss>(Hijos);

                    foreach (CategoriaTesauro cat in hijosAux)
                    {
                        if (!catMantener.Contains(cat.Clave))
                        {
                            Hijos.Remove(cat);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Elimina las categorías que son públicas cuando el metaEspacio es GNOSS.
        /// </summary>
        public void EliminarCategoriasPublicasSiEsMetaEspacioGNOSS(string pLanguageCode)
        {

            //1 borramos todas las categorias tesauro de los Hijos
            List<CategoriaTesauro> catsRemovidas = new List<CategoriaTesauro>();

            List<IElementoGnoss> hijosAux = new List<IElementoGnoss>(Hijos);
            foreach (CategoriaTesauro cat in hijosAux)
            {
                if (cat.GestorTesauro.CategoriaPublicaID.Equals(cat.PadreNivelRaiz.Clave))
                {
                    if (!catsRemovidas.Contains(cat))
                    {
                        catsRemovidas.AddRange(EliminarCategoriasHijas(cat));
                        catsRemovidas.Add(cat);
                    }
                    Hijos.Remove(cat);
                }
            }

            //2 borramos las categorías tesauro del resto de listas del gestor tesauro
            foreach (CategoriaTesauro catTesauro in catsRemovidas)
            {
                if (ListaCategoriasTesauro != null)
                {
                    ListaCategoriasTesauroYCategoriasSugeridasPrimerNivel.Remove(catTesauro);
                }
                if (ListaCategoriasTesauroYCategoriasSugeridasPrimerNivel != null)
                {
                    ListaCategoriasTesauro.Remove(catTesauro.Clave);
                }

                if (ListaCategoriasTesauroPrimerNivel != null)
                {
                    ListaCategoriasTesauroPrimerNivel.Remove(catTesauro.Clave);
                }

                if (ListaCategoriasTesauroOrdenadasPorNombre != null)
                {
                    ListaCategoriasTesauroOrdenadasPorNombre.Remove(catTesauro.Nombre[pLanguageCode]);
                }

                if (ListaCategoriasSugeridasPrimerNivel != null)
                {
                    ListaCategoriasSugeridasPrimerNivel.Remove(catTesauro.Clave);
                }

                if (ListaCategoriasTesauroYCategoriasSugeridasPrimerNivel != null)
                {
                    foreach (ElementoGnoss eg in ListaCategoriasTesauroYCategoriasSugeridasPrimerNivel)
                    {
                        if (eg.Clave.Equals(catTesauro.Clave))
                        {
                            ListaCategoriasTesauroYCategoriasSugeridasPrimerNivel.Remove(eg);
                        }
                    }
                }

                TesauroDW.ListaCategoriaTesauro.Remove(TesauroDW.ListaCategoriaTesauro.Where(item => item.CategoriaTesauroID.Equals(catTesauro.Clave)).FirstOrDefault());
            }
        }

        private List<CategoriaTesauro> EliminarCategoriasHijas(CategoriaTesauro pPadre)
        {
            List<CategoriaTesauro> catsRemovidas = new List<CategoriaTesauro>();

            List<IElementoGnoss> hijosAux = new List<IElementoGnoss>(pPadre.Hijos);
            foreach (CategoriaTesauro hijo in hijosAux)
            {
                catsRemovidas.AddRange(EliminarCategoriasHijas(hijo));
                catsRemovidas.Add(hijo);
                pPadre.Hijos.Remove(hijo);
            }
            return catsRemovidas;
        }

        /// <summary>
        /// Obtiene las categorías que tengan un determinado nombre.
        /// </summary>
        /// <param name="pNombre">Nombre de categoría</param>
        /// <returns>Categorías que tengan un determinado nombre</returns>
        public List<Guid> ObtenerCategoriasIDPorNombre(string pNombre)
        {
            List<Guid> listaCat = new List<Guid>();

            foreach (AD.EntityModel.Models.Tesauro.CategoriaTesauro filaCat in TesauroDW.ListaCategoriaTesauro.Where(item => item.Nombre.Contains(pNombre)))
            {
                listaCat.Add(filaCat.CategoriaTesauroID);
            }

            return listaCat;
        }

        public Dictionary<Guid, CategoriaTesauroPropiedades> FilasPropiedadesPorCategoria
        {
            get
            {
                if (mFilasPropiedadesPorCategoria == null)
                {
                    try
                    {
                        mFilasPropiedadesPorCategoria = new Dictionary<Guid, CategoriaTesauroPropiedades>();
                    }
                    catch
                    {
                        mLoggingService.GuardarLogError("GestionTesauro->FilasPropiedadesPorCategoria: No se ha podido inicializar mFilasPropiedadesPorCategoria");
                    }
                    if (TesauroDW != null)
                    {
                        if (TesauroDW.ListaCategoriaTesauroPropiedades != null)
                        {
                            foreach (CategoriaTesauroPropiedades fila in TesauroDW.ListaCategoriaTesauroPropiedades)
                            {
                                if (fila != null)
                                {
                                    if (fila.CategoriaTesauroID != null)
                                    {
                                        mFilasPropiedadesPorCategoria.Add(fila.CategoriaTesauroID, fila);
                                    }
                                    else
                                    {
                                        mLoggingService.GuardarLogError("GestionTesauro->FilasPropiedadesPorCategoria: fila.CategoriaTesauroID es nulo");
                                    }
                                }
                                else
                                {
                                    mLoggingService.GuardarLogError("GestionTesauro->FilasPropiedadesPorCategoria: fila de CategoriaTesauroPropiedades es nulo");
                                }

                            }
                        }
                        else
                        {
                            mLoggingService.GuardarLogError("GestionTesauro->FilasPropiedadesPorCategoria: TesauroDW.ListaCategoriaTesauroPropiedades es nulo");
                        }
                    }
                    else
                    {
                        mLoggingService.GuardarLogError("GestionTesauro->FilasPropiedadesPorCategoria: TesauroDW es nulo");
                    }
                }

                return mFilasPropiedadesPorCategoria;
            }
        }


        public List<CategoriaTesauro> ComprobarCategoriasObligatoriasMarcadas(List<Guid> pListaCategorias)
        {
            List<CategoriaTesauro> categoriasNoMarcadas = new List<CategoriaTesauro>();

            if (TesauroDW.ListaCategoriaTesauroPropiedades.Count > 0)
            {
                foreach (Guid categoriaID in ListaCategoriasTesauroPrimerNivel.Keys)
                {
                    CategoriaTesauro categoria = ListaCategoriasTesauro[categoriaID];
                    if (FilasPropiedadesPorCategoria.ContainsKey(categoriaID))
                    {
                        if (FilasPropiedadesPorCategoria[categoriaID].Obligatoria.Equals(true))
                        {
                            if (!ComprobarCategoriaObligatoriasMarcada(categoria, pListaCategorias))
                            {
                                categoriasNoMarcadas.Add(categoria);
                            }
                        }
                    }
                    categoriasNoMarcadas.AddRange(ComprobarCategoriasHijasObligatoriasMarcadas(pListaCategorias, categoria.SubCategorias));
                }
            }
            return categoriasNoMarcadas;
        }

        private List<CategoriaTesauro> ComprobarCategoriasHijasObligatoriasMarcadas(List<Guid> pListaCategorias, List<CategoriaTesauro> pListaCategoriasDelTesauro)
        {
            List<CategoriaTesauro> categoriasNoMarcadas = new List<CategoriaTesauro>();

            if (TesauroDW.ListaCategoriaTesauroPropiedades.Count > 0)
            {
                foreach (CategoriaTesauro categoria in pListaCategoriasDelTesauro)
                {
                    Guid categoriaID = categoria.Clave;
                    if (FilasPropiedadesPorCategoria.ContainsKey(categoriaID))
                    {
                        if (FilasPropiedadesPorCategoria[categoriaID].Obligatoria.Equals(true))
                        {
                            if (!ComprobarCategoriaObligatoriasMarcada(categoria, pListaCategorias))
                            {
                                categoriasNoMarcadas.Add(categoria);
                            }
                        }
                    }
                    categoriasNoMarcadas.AddRange(ComprobarCategoriasHijasObligatoriasMarcadas(pListaCategorias, categoria.SubCategorias));
                }
            }
            return categoriasNoMarcadas;
        }

        private bool ComprobarCategoriaObligatoriasMarcada(CategoriaTesauro pCategoria, List<Guid> pListaCategorias)
        {
            if (pListaCategorias.Contains(pCategoria.Clave))
            {
                return true;
            }
            else
            {
                foreach (CategoriaTesauro categoriaHija in pCategoria.SubCategorias)
                {
                    if (ComprobarCategoriaObligatoriasMarcada(categoriaHija, pListaCategorias))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool mDisposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~GestionTesauro()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="pDisposing">Determina si se está llamando desde el Dispose()</param>
        protected override void Dispose(bool pDisposing)
        {
            if (!this.mDisposed)
            {
                mDisposed = true;

                try
                {
                    if (pDisposing)
                    {
                        //Libero todos los recursos administrados que he añadido a esta clase
                        foreach (ElementoGnoss elemento in this.mListaCategoriasTesauro.Values)
                        {
                            elemento.Dispose();
                        }
                        if (this.mListaCategoriasTesauro != null)
                        {
                            this.mListaCategoriasTesauro.Clear();
                        }
                    }
                }
                finally
                {
                    mListaCategoriasTesauro = null;
                    mGestorDocumental = null;
                    mFilaTesauroActual = null;

                    // Llamo al dispose de la clase base
                    base.Dispose(pDisposing);
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
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("GestorDocumental", GestorDocumental);
        }

        #endregion
    }
}
