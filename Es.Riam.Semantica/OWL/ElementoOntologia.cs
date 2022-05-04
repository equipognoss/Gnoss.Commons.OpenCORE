using System;
using System.Collections.Generic;
using Es.Riam.Interfaces;
using System.Runtime.Serialization;
using Es.Riam.Semantica.Plantillas;

namespace Es.Riam.Semantica.OWL
{
    /// <summary>
    /// Representa un elemento de una ontología
    /// </summary>
    [Serializable]
    public class ElementoOntologia : IEquatable<ElementoOntologia>, IDisposable, ISerializable
    {
        #region Miembros

        /// <summary>
        /// Tipo de la entidad
        /// </summary>
        protected string mTipoEntidad;

        /// <summary>
        /// Tipo de entidad si la URL completa, es decir, relavito.
        /// </summary>
        protected string mTipoEntidadRelativo;

        /// <summary>
        /// Propiedades que posee la entidad.
        /// </summary>
        private List<Propiedad> mPropiedades;

        /// <summary>
        /// Propiedades que posee la entidad.
        /// </summary>
        private List<Propiedad> mPropiedadesOrdenadas;

        /// <summary>
        /// Restricciones sobre las propiedades.
        /// </summary>
        private List<Restriccion> mRestricciones;

        /// <summary>
        /// Superclases de la entidad
        /// </summary>
        private List<string> mSuperclases;

        /// <summary>
        /// Subclases de la entidad.
        /// </summary>
        private List<string> mSubclases;

        /// <summary>
        /// Obtiene o establece las superclases útiles de la entidad
        /// </summary>
        private List<string> mSuperclasesUtiles;

        /// <summary>
        /// Relaciones que especializan la entidad.
        /// </summary>
        private List<string> mEspecializaciones;

        /// <summary>
        /// Relación que generaliza la entidad.
        /// </summary>
        private string mGeneralizacion;

        /// <summary>
        /// Lista de entidades relacionadas con ésta entidad mediante propiedades.
        /// </summary>
        protected List<ElementoOntologia> mEntidadesRelacionadas;

        /// <summary>
        /// ID de la entidad.
        /// </summary>
        protected string mID;

        /// <summary>
        /// Almacena la descripción o el nombre del elemento que facilita la identificación del mismo por el usuario
        /// </summary>
        private string mDescripcion;

        /// <summary>
        /// Verdadero si la entidad es valida y se debe exportar/importar.
        /// </summary>
        private bool mEntidadValida;

        /// <summary>
        /// Elemento gnoss que representa la entidad
        /// </summary>
        private IElementoGnoss mElemento;

        /// <summary>
        /// Padre de la entidad.
        /// </summary>
        private IElementoGnoss mPadre;

        /// <summary>
        /// Verdad si la entidad puede tener un padre.
        /// </summary>
        private bool mPermitePadre;

        /// <summary>
        /// Almacena la lista de propiedades imprimibles
        /// </summary>
        private List<Propiedad> mListaPropiedadesImprimibles = new List<Propiedad>();

        /// <summary>
        /// Verdad si se han obtenido las entidades relacionadas con esta entidad
        /// </summary>
        private bool mEstaCompleta = false;

        /// <summary>
        /// Url de la ontología.
        /// </summary>
        private string mUrlOntologia;

        /// <summary>
        /// Namespace de la ontología.
        /// </summary>
        private string mNamespaceOntologia;

        /// <summary>
        /// Ontología a la que pertenece el elemento.
        /// </summary>
        private Ontologia mOntologia;

        /// <summary>
        /// almacenar para cada Entidad sus Entidades relacionadas con owl:SameAs
        /// </summary>
        private List<string> mOWLSameAs;

        /// <summary>
        /// Configuración de la plantilla.
        /// </summary>
        private EstiloPlantillaEspecifEntidad mEspecifEntidad;

        /// <summary>
        /// Label.
        /// </summary>
        private string mLabel;

        /// <summary>
        /// Label.
        /// </summary>
        private Dictionary<string, string> mLabelIdioma;

        /// <summary>
        /// Indica si la entidad es path de un tesauro semántico.
        /// </summary>
        private bool? mEsEntidadPathTesSemantico;

        /// <summary>
        /// Nombre de la propiedad nodo del tesauro semántico, ya que esta entidad es un path del mismo.
        /// </summary>
        private string mPropiedadNodoTesSemantico;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ElementoOntologia()
        {
        }

        /// <summary>
        /// Construye un elemento de ontología a partir de otro pasado por parámetro
        /// </summary>
        /// <param name="pEntidad">Elemento de ontología a partir del cual se creará uno nuevo</param>
        public ElementoOntologia(ElementoOntologia pEntidad)
        {
            this.mTipoEntidad = pEntidad.TipoEntidad;
            this.mPropiedades = new List<Propiedad>();

            foreach (Propiedad propiedad in pEntidad.Propiedades)
            {
                Propiedad propiedadNueva = new Propiedad(propiedad);
                this.mPropiedades.Add(propiedadNueva);
            }
            //this.mRestricciones = new List<Restriccion>();

            //foreach(Restriccion restriccion in pEntidad.Restricciones)
            //{
            //    this.mRestricciones.Add(restriccion);
            //}
            this.mRestricciones = pEntidad.Restricciones;
            this.mSuperclases = pEntidad.Superclases;
            this.mSuperclasesUtiles = pEntidad.SuperclasesUtiles;
            this.mEntidadesRelacionadas = new List<ElementoOntologia>();

            foreach (ElementoOntologia entidad in pEntidad.EntidadesRelacionadas)
            {
                this.mEntidadesRelacionadas.Add(new ElementoOntologia(entidad));
            }
            this.mID = pEntidad.ID;
            this.mDescripcion = pEntidad.Descripcion;
            this.mSubclases = pEntidad.Subclases;
            this.mEspecializaciones = pEntidad.Especializaciones;
            this.mGeneralizacion = pEntidad.Generalizacion;
            this.mEntidadValida = pEntidad.EntidadValida;
            this.mPadre = pEntidad.Padre;
            this.mPermitePadre = pEntidad.PermitePadre;
            this.mUrlOntologia = pEntidad.UrlOntologia;
            this.mNamespaceOntologia = pEntidad.NamespaceOntologia;
            this.mOntologia = pEntidad.Ontologia;
            this.mLabel = pEntidad.Label;
            this.mLabelIdioma = pEntidad.LabelIdioma;
        }

        /// <summary>
        /// Crea una entidad a partir de un tipo de entidad.
        /// </summary>
        /// <param name="pTipoEntidad">tipo de entidad.</param>
        /// <param name="pNamespaceOntologia">Namespace de la ontología</param>
        /// <param name="pUrlOntologia">Url de la ontología</param>
        /// <param name="pOntologia">Ontología a la que pertenece el elemento</param>
        public ElementoOntologia(string pTipoEntidad, string pUrlOntologia, string pNamespaceOntologia, Ontologia pOntologia)
        {
            this.mTipoEntidad = pTipoEntidad;
            this.mPropiedades = new List<Propiedad>();
            this.mRestricciones = new List<Restriccion>();
            this.mSuperclases = new List<string>();
            this.mEntidadesRelacionadas = new List<ElementoOntologia>();
            this.mID = "";
            this.mDescripcion = pTipoEntidad;
            this.mSubclases = new List<string>();
            this.mEspecializaciones = new List<string>();
            this.mGeneralizacion = null;
            this.mEntidadValida = true;
            this.Padre = null;
            this.mPermitePadre = true;
            this.mUrlOntologia = pUrlOntologia;
            this.mNamespaceOntologia = pNamespaceOntologia;
            this.mOntologia = pOntologia;
        }

        /// <summary>
        /// Crea una entidad a partir de los parametros de entrada
        /// </summary>
        /// <param name="pTipoEntidad">tipo de la entidad</param>
        /// <param name="pPropiedades">propiedades que adquirirá la entidad.</param>
        /// <param name="pRestricciones">restricciones que posee la entidad</param>
        /// <param name="pSuperclases">superclases a las que hace referencia la entidad.</param>
        /// <param name="pID">ID de la entidad.</param>
        /// <param name="pEntidadesRelacionadas">Entidades relacionadas con la entidad a crear.</param>
        /// <param name="pNamespaceOntologia">Namespace de la ontología</param>
        /// <param name="pUrlOntologia">Url de la ontología</param>
        /// <param name="pOntologia">Ontología a la que pertenece el elemento</param>
        public ElementoOntologia(string pTipoEntidad, List<Propiedad> pPropiedades, List<Restriccion> pRestricciones, List<string> pSuperclases, string pID, List<ElementoOntologia> pEntidadesRelacionadas, string pUrlOntologia, string pNamespaceOntologia, Ontologia pOntologia)
        {
            this.mTipoEntidad = pTipoEntidad;
            this.mPropiedades = pPropiedades;
            this.mRestricciones = pRestricciones;
            this.mSuperclases = pSuperclases;
            this.mID = pID;
            this.mEntidadesRelacionadas = pEntidadesRelacionadas;
            this.mDescripcion = pTipoEntidad;
            this.mSubclases = new List<string>();
            this.mEspecializaciones = new List<string>();
            this.mGeneralizacion = null;
            this.mEntidadValida = true;
            this.Padre = null;
            this.mPermitePadre = true;
            this.mUrlOntologia = pUrlOntologia;
            this.mNamespaceOntologia = pNamespaceOntologia;
            this.mOntologia = pOntologia;
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected ElementoOntologia(SerializationInfo info, StreamingContext context)
        {
            mDescripcion = (string)info.GetValue("Descripcion", typeof(string));
            mEntidadesRelacionadas = (List<ElementoOntologia>)info.GetValue("EntidadesRelacionadas", typeof(List<ElementoOntologia>));
            mEspecializaciones = (List<string>)info.GetValue("Especializaciones", typeof(List<string>));
            mEstaCompleta = (bool)info.GetValue("EstaCompleta", typeof(bool));
            mGeneralizacion = (string)info.GetValue("Generalizacion", typeof(string));
            mID = (string)info.GetValue("ID", typeof(string));
            mListaPropiedadesImprimibles = (List<Propiedad>)info.GetValue("ListaPropiedadesImprimibles", typeof(List<Propiedad>));
            mNamespaceOntologia = (string)info.GetValue("NamespaceOntologia", typeof(string));
            mOntologia = (Ontologia)info.GetValue("Ontologia", typeof(Ontologia));
            mPermitePadre = (bool)info.GetValue("PermitePadre", typeof(bool));
            mPropiedades = (List<Propiedad>)info.GetValue("Propiedades", typeof(List<Propiedad>));
            mPropiedadesOrdenadas = (List<Propiedad>)info.GetValue("PropiedadesOrdenadas", typeof(List<Propiedad>));
            mRestricciones = (List<Restriccion>)info.GetValue("Restricciones", typeof(List<Restriccion>));
            mSubclases = (List<string>)info.GetValue("Subclases", typeof(List<string>));
            mSuperclases = (List<string>)info.GetValue("Superclases", typeof(List<string>));
            mTipoEntidad = (string)info.GetValue("TipoEntidad", typeof(string));
            mTipoEntidadRelativo = (string)info.GetValue("TipoEntidadRelativo", typeof(string));
            mUrlOntologia = (string)info.GetValue("UrlOntologia", typeof(string));

            mEspecifEntidad = (EstiloPlantillaEspecifEntidad)info.GetValue("EspecifEntidad", typeof(EstiloPlantillaEspecifEntidad));
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Devuelve o establece la Url de la ontología.
        /// </summary>
        public string UrlOntologia
        {
            get
            {
                return mUrlOntologia;
            }
            set
            {
                mUrlOntologia = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el namespace de la ontología.
        /// </summary>
        public string NamespaceOntologia
        {
            get
            {
                return mNamespaceOntologia;
            }
            set
            {
                mNamespaceOntologia = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el tipo de la entidad
        /// </summary>
        public virtual string TipoEntidad
        {
            set
            {
                this.mTipoEntidad = value;
            }
            get
            {
                return this.mTipoEntidad;
            }
        }

        /// <summary>
        /// Obtiene o establece el tipo de la entidad
        /// </summary>
        public virtual string TipoEntidadGeneracionIDs
        {
            get
            {
                return this.mTipoEntidad.Replace("/", "_").Replace(":", "_").Replace(".", "_").Replace("#", "_"); ;
            }
        }

        /// <summary>
        /// Obtiene o establece el tipo de la entidad para generar el nombre de las clases.
        /// </summary>
        public virtual string TipoEntidadGeneracionClases
        {
            get
            {
                if (mTipoEntidad.Contains("#"))
                {
                    return mTipoEntidad.Substring(mTipoEntidad.LastIndexOf("#") + 1);
                }
                else if (mTipoEntidad.Contains("/"))
                {
                    return mTipoEntidad.Substring(mTipoEntidad.LastIndexOf("/") + 1);
                }
                else
                {
                    return mTipoEntidad;
                }
            }
        }

        /// <summary>
        /// Obtiene el tipo de la entidad sin apaños para repeticiones.
        /// </summary>
        public string TipoEntidadLimpioDeApanioRepeticiones
        {
            get
            {
                return ObtenerTiposEntidadLimpiaDeApanioRepeticiones(TipoEntidad);
            }
        }

        /// <summary>
        /// Indica si la entidad es path de un tesauro semántico.
        /// </summary>
        public bool EsEntidadPathTesSemantico
        {
            get
            {
                if (!mEsEntidadPathTesSemantico.HasValue)
                {
                    mEsEntidadPathTesSemantico = false;

                    foreach (Propiedad prop in Propiedades)
                    {
                        if (prop.ElementoOntologia == null)
                        {
                            prop.ElementoOntologia = this;
                        }

                        if (prop.EspecifPropiedad.SelectorEntidad != null && prop.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "Tesauro")
                        {
                            mEsEntidadPathTesSemantico = true;
                            mPropiedadNodoTesSemantico = prop.Nombre;
                            break;
                        }
                    }
                }

                return mEsEntidadPathTesSemantico.Value;
            }
        }

        /// <summary>
        /// Nombre de la propiedad nodo del tesauro semántico, ya que esta entidad es un path del mismo.
        /// </summary>
        public string PropiedadNodoTesSemantico
        {
            get
            {
                return mPropiedadNodoTesSemantico;
            }
        }

        /// <summary>
        /// Tipo de entidad si la URL completa, es decir, relavito.
        /// </summary>
        public string TipoEntidadRelativo
        {
            set
            {
                this.mTipoEntidadRelativo = value;
            }
            get
            {
                if (mOntologia == null)
                {
                    return TipoEntidad;
                }

                if (mTipoEntidadRelativo == null)
                {
                    string urlElemento = TipoEntidad;

                    if (urlElemento.Contains("#"))
                    {
                        urlElemento = urlElemento.Substring(0, urlElemento.LastIndexOf("#") + 1);
                    }
                    else if (urlElemento.Contains("/"))
                    {
                        urlElemento = urlElemento.Substring(0, urlElemento.LastIndexOf("/") + 1);
                    }

                    if (mOntologia.NamespacesDefinidos.ContainsKey(urlElemento))
                    {
                        //El tipo entidad tiene una URL absoluta por lo que hay que recortarla:
                        mTipoEntidadRelativo = TipoEntidad.Replace(urlElemento, "");
                    }
                    else
                    {
                        mTipoEntidadRelativo = TipoEntidad;
                    }
                }

                return this.mTipoEntidadRelativo;
            }
        }

        /// <summary>
        /// Obtiene o establece el tipo de la entidad
        /// </summary>
        public virtual string TipoEntidadConNamespace
        {
            set
            {
                this.mTipoEntidad = value;
            }
            get
            {
                if (!mTipoEntidad.Contains("#") && !mTipoEntidad.Contains("/"))
                {
                    return this.NamespaceOntologia + ":" + this.mTipoEntidad;
                }
                else
                {
                    string urlNamespace = null;
                    string tipoEntidadRelativo = null;

                    if (mTipoEntidad.Contains("#"))
                    {
                        urlNamespace = mTipoEntidad.Substring(0, mTipoEntidad.LastIndexOf("#") + 1);
                        tipoEntidadRelativo = mTipoEntidad.Substring(mTipoEntidad.LastIndexOf("#") + 1);
                    }
                    else //Contiene '/'
                    {
                        urlNamespace = mTipoEntidad.Substring(0, mTipoEntidad.LastIndexOf("/") + 1);
                        tipoEntidadRelativo = mTipoEntidad.Substring(mTipoEntidad.LastIndexOf("/") + 1);
                    }

                    if (Ontologia.GenararNamespacesHuerfanos && !Ontologia.NamespacesDefinidos.ContainsKey(urlNamespace))
                    {
                        Ontologia.GenararNamespaceUrl(urlNamespace);
                    }

                    if (Ontologia.NamespacesDefinidos.ContainsKey(urlNamespace))
                    {
                        return Ontologia.NamespacesDefinidos[urlNamespace] + ":" + tipoEntidadRelativo;
                    }
                    else
                    {
                        return this.mTipoEntidad;
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene o establece el tipo de la entidad
        /// </summary>
        public virtual string TipoEntidadCrearRdf
        {
            get
            {
                if (TipoEntidad.Contains("http://"))
                {
                    return TipoEntidadConNamespace;
                }
                else
                {
                    return TipoEntidad;
                }
            }
        }

        /// <summary>
        /// Obtiene o establece el elemento gnoss al que hace referencia.
        /// </summary>
        public IElementoGnoss Elemento
        {
            get
            {
                return this.mElemento;
            }
            set
            {
                this.mElemento = value;
            }
        }

        /// <summary>
        /// Obtiene las propiedades que posee la entidad
        /// </summary>
        public List<Propiedad> Propiedades
        {
            get
            {
                if (mPropiedades == null)
                {
                    mPropiedades = new List<Propiedad>();
                }
                if (mPropiedades.Count > 0 && (mPropiedades[0].ElementoOntologia == null || mPropiedades[0].ElementoOntologia != this))
                {
                    foreach (Propiedad propiedad in mPropiedades)
                    {
                        propiedad.ElementoOntologia = this;
                    }
                }
                return this.mPropiedades;
            }
        }

        /// <summary>
        /// Obtiene una lista ordenada (primero las funcionales de tipo DataProperty) de propiedades
        /// </summary>
        public List<Propiedad> PropiedadesOrdenadas
        {
            get
            {
                if (mPropiedadesOrdenadas == null)
                {
                    List<Propiedad> propiedadesOrdenadas = new List<Propiedad>();
                    List<Propiedad> propiedadesAux = new List<Propiedad>();
                    foreach (Propiedad propiedad in Propiedades)
                    {
                        if (propiedad.FunctionalProperty && propiedad.Tipo == TipoPropiedad.DatatypeProperty)
                        {
                            //Añado la que me interesa a la lista que devolveré:
                            propiedadesOrdenadas.Add(propiedad);
                        }
                        else
                        {
                            //El resto las guardo en una auxiliar:
                            propiedadesAux.Add(propiedad);
                        }
                    }
                    //Añado todos los elemento de la auxiliar a la principal:
                    propiedadesOrdenadas.AddRange(propiedadesAux);
                    mPropiedadesOrdenadas = propiedadesOrdenadas;
                }
                return mPropiedadesOrdenadas;
            }
        }

        /// <summary>
        /// Obtiene las restricciones sobre las propiedades
        /// </summary>
        public List<Restriccion> Restricciones
        {
            get
            {
                if (mRestricciones == null)
                    mRestricciones = new List<Restriccion>();
                return this.mRestricciones;
            }
        }

        /// <summary>
        /// Obtiene o establece las superclases de la entidad
        /// </summary>
        public virtual List<string> Superclases
        {
            set
            {
                this.mSuperclases = value;
            }
            get
            {
                if (mSuperclases == null)
                {
                    mSuperclases = new List<string>();
                }

                return this.mSuperclases;
            }
        }

        /// <summary>
        /// Obtiene o establece la lista de subclases de la entidad
        /// </summary>
        public List<string> Subclases
        {
            get
            {
                return this.mSubclases;
            }
            set
            {
                //mSubclases = new List<string>(value);
                mSubclases = value;
            }
        }

        /// <summary>
        /// Obtiene o establece las superclases útiles de la entidad
        /// </summary>
        public List<string> SuperclasesUtiles
        {
            get
            {
                if (mSuperclasesUtiles == null)
                {
                    mSuperclasesUtiles = new List<string>();

                    foreach (string superClase in Superclases)
                    {
                        if (superClase != "Thing" && !superClase.Contains("#Thing") && !EsClaseOntologiaImportada(superClase))
                        {
                            mSuperclasesUtiles.Add(superClase);
                        }
                    }
                }

                return this.mSuperclasesUtiles;
            }
        }

        /// <summary>
        /// Obtiene o establece la lista de relaciones que especializan a la entidad.
        /// </summary>
        public List<string> Especializaciones
        {
            get
            {
                return this.mEspecializaciones;
            }
            set
            {
                //mEspecializaciones = new List<string>(value);
                mEspecializaciones = value;
            }
        }

        /// <summary>
        /// Obtiene o establece la relación que generaliza la entidad.
        /// </summary>
        public string Generalizacion
        {
            set
            {
                this.mGeneralizacion = value;
            }
            get
            {
                return this.mGeneralizacion;
            }
        }

        /// <summary>
        /// Obtiene o establece el ID de la entidad.
        /// </summary>
        public string ID
        {
            set
            {
                this.mID = value;
            }
            get
            {
                return this.mID;
            }
        }

        /// <summary>
        /// Devuelve la Uri del elemento.
        /// </summary>
        public virtual string Uri
        {
            get
            {
                if (Ontologia.UsoIDsRelativos)
                {
                    return GestionOWL.ObtenerUrlEntidad(TipoEntidad) + ID;
                }
                else
                {
                    return ID;
                }
            }
            set
            {
                this.mID = value;
            }
        }

        /// <summary>
        /// Obtiene la lista de entidades que están relacionadas con la entidad.
        /// </summary>
        public List<ElementoOntologia> EntidadesRelacionadas
        {
            get
            {
                if (mEntidadesRelacionadas == null)
                    mEntidadesRelacionadas = new List<ElementoOntologia>();
                return this.mEntidadesRelacionadas;
            }
        }

        /// <summary>
        /// Obtiene o establece la descripción o el nombre del elemento que facilita la identificación del mismo por el usuario
        /// </summary>
        public string Descripcion
        {
            set
            {
                this.mDescripcion = value;
            }
            get
            {
                return this.mDescripcion;
            }
        }

        /// <summary>
        /// Obtiene o establece si la entidad se debe exportar/importar o no.
        /// </summary>
        public bool EntidadValida
        {
            set
            {
                this.mEntidadValida = value;
            }
            get
            {
                return this.mEntidadValida;
            }
        }

        /// <summary>
        /// Obtiene o establece el padre de la entidad.
        /// </summary>
        public IElementoGnoss Padre
        {
            set
            {
                this.mPadre = value;
            }
            get
            {
                return this.mPadre;
            }
        }

        /// <summary>
        /// Verdad si la entidad puede tener un padre.
        /// </summary>
        public bool PermitePadre
        {
            get
            {
                return this.mPermitePadre;
            }
            set
            {
                this.mPermitePadre = value;
            }
        }

        /// <summary>
        /// Obtiene la lista de propiedades imprimibles
        /// </summary>
        public List<Propiedad> ListaPropiedadesImprimibles
        {
            get
            {
                return this.mListaPropiedadesImprimibles;
            }
        }

        /// <summary>
        /// Verdad si se han obtenido todas las entidades relacionadas con esta entidad
        /// </summary>
        public bool EstaCompleta
        {
            get
            {
                return this.mEstaCompleta;
            }
            set
            {
                this.mEstaCompleta = value;
            }
        }

        /// <summary>
        /// Ontología a la que pertenece el elemento.
        /// </summary>
        public Ontologia Ontologia
        {
            get
            {
                return mOntologia;
            }
            set
            {
                mOntologia = value;
            }
        }

        /// <summary>
        /// Propiedades equivalentes con SameAs.
        /// </summary>
        public List<string> OWLSameAs
        {
            get
            {
                if (mOWLSameAs == null)
                {
                    mOWLSameAs = new List<string>();
                }
                return mOWLSameAs;
            }
            set
            {
                mOWLSameAs = value;
            }
        }

        /// <summary>
        /// Label.
        /// </summary>
        public string Label
        {
            get
            {
                return mLabel;
            }
            set
            {
                mLabel = value;
            }
        }

        /// <summary>
        /// Label.
        /// </summary>
        public Dictionary<string, string> LabelIdioma
        {
            get
            {
                if (mLabelIdioma == null)
                {
                    mLabelIdioma = new Dictionary<string, string>();
                }

                return mLabelIdioma;
            }
            set
            {
                mLabelIdioma = value;
            }
        }

        /// <summary>
        /// Indica si alguna propiedad de la entidad posee valor.
        /// </summary>
        public bool TienePropiedadesConValor
        {
            get
            {
                foreach (Propiedad prop in Propiedades)
                {
                    if (prop.ValoresUnificados.Count > 0 || prop.ListaValoresIdioma.Count > 0)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool IdiomaUsuarioEnAlgunaPropiedad { get; set; }

        public bool IdiomaNoUsuarioEnAlgunaPropiedad { get; set; }

        #region Estilos plantilla

        /// <summary>
        /// Configuración de la plantilla.
        /// </summary>
        public EstiloPlantillaEspecifEntidad EspecifEntidad
        {
            get
            {
                if (mEspecifEntidad == null)
                {
                    string tipoEnt = TipoEntidad;

                    if (tipoEnt.Contains("_bis"))
                    {
                        tipoEnt = tipoEnt.Substring(0, tipoEnt.IndexOf("_bis"));
                    }


                    if (mOntologia.EstilosPlantilla.ContainsKey(TipoEntidad))
                    {
                        mEspecifEntidad = new EstiloPlantillaEspecifEntidad((EstiloPlantillaEspecifEntidad)mOntologia.EstilosPlantilla[TipoEntidad][0]);
                    }
                    else if (mOntologia.EstilosPlantilla.ContainsKey(tipoEnt))
                    {
                        mEspecifEntidad = new EstiloPlantillaEspecifEntidad((EstiloPlantillaEspecifEntidad)mOntologia.EstilosPlantilla[tipoEnt][0]);
                    }
                    else
                    {
                        mEspecifEntidad = new EstiloPlantillaEspecifEntidad();
                    }

                    mEspecifEntidad.Entidad = this;
                }

                if (mEspecifEntidad.Entidad == null) // añadido para serializar
                {
                    mEspecifEntidad.Entidad = this;
                }

                return mEspecifEntidad;
            }
        }

        /// <summary>
        /// Orden de la entidad especificado por una de sus propiedades.
        /// </summary>
        public int OrdenEntiad
        {
            get
            {
                int orden = 10000;

                if (EspecifEntidad.CampoOrden != null)
                {
                    Propiedad propOrden = ObtenerPropiedad(EspecifEntidad.CampoOrden);

                    if (propOrden != null)
                    {
                        if (propOrden.UnicoValor.Key != null)
                        {
                            int.TryParse(propOrden.UnicoValor.Key, out orden);
                        }
                        else if (propOrden.ListaValores.Count > 0)
                        {
                            int.TryParse(new List<string>(propOrden.ListaValores.Keys)[0], out orden);
                        }
                    }
                }

                return orden;
            }
        }

        /// <summary>
        /// Orden de la entidad en texto especificado por una de sus propiedades.
        /// </summary>
        public string OrdenEntiadTexto
        {
            get
            {
                int orden = OrdenEntiad;

                if (orden == 10000)
                {
                    return "";
                }
                else
                {
                    return orden.ToString() + ". ";
                }
            }
        }

        /// <summary>
        /// Indica si la entidad cumple la condición de que alguna de sus propiedades tenga un determinado valor.
        /// </summary>
        public bool CumpleCondicionesMostrar
        {
            get
            {
                return ElementoOrdenado.CumpleEntidadCondicionesMostrar(this, EspecifEntidad.PropsCondicionPintarEntSegunValores);
            }
        }

        /// <summary>
        /// Indica si la entidad contiene una propiedad que es tesauro semántico y además está configurado con vista árbol.
        /// </summary>
        public bool ContienePropiedadTesSemArbol
        {
            get
            {
                foreach (Propiedad propiedad in Propiedades)
                {
                    propiedad.ElementoOntologia = this;

                    if (propiedad.EspecifPropiedad.SelectorEntidad != null && propiedad.EspecifPropiedad.SelectorEntidad.TipoSeleccion == "Tesauro" && propiedad.EspecifPropiedad.SelectorEntidad.TipoPresentacion == "Arbol")
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        #endregion


        #endregion

        #region Métodos generales

        #region Públicos

        /// <summary>
        /// Añade una propiedad a la lista de propiedades
        /// </summary>
        /// <param name="pPropiedad">Propiedad que se va a añadir</param>
        public virtual void AddPropiedad(Propiedad pPropiedad)
        {
            this.mPropiedades.Add(new Propiedad(pPropiedad));
        }

        /// <summary>
        /// Añade una restricción a al lista de propiedades
        /// </summary>
        /// <param name="pRestriccion">Restricción que se va a añadir</param>
        public void AddRestriccion(Restriccion pRestriccion)
        {
            this.mRestricciones.Add(pRestriccion);
        }

        /// <summary>
        /// Añade una super clase a la lista de super clases
        /// </summary>
        /// <param name="pSubclase">Subclase</param>
        public void AddSubclase(string pSubclase)
        {
            this.mSubclases.Add(pSubclase);
        }

        /// <summary>
        /// Devuelve la lista de atributos de la entidad.
        /// </summary>
        /// <returns>Lista de propiedades</returns>
        public List<Propiedad> ObtenerAtributos()
        {
            List<Propiedad> propiedades = new List<Propiedad>();
            foreach (Propiedad propiedad in this.Propiedades)
            {
                if (propiedad.Tipo.Equals(TipoPropiedad.DatatypeProperty))
                    propiedades.Add(propiedad);
            }
            return propiedades;
        }

        /// <summary>
        /// Obtiene todos los atributos visibles de la entidad.
        /// </summary>
        /// <returns></returns>
        public List<Propiedad> ObtenerAtributosVisibles()
        {
            List<Propiedad> atributos = ObtenerAtributos();
            List<Propiedad> atributosVisibles = new List<Propiedad>();

            foreach (Propiedad propiedad in atributos)
            {
                if (propiedad.Visible)
                    atributosVisibles.Add(propiedad);
            }
            return atributosVisibles;
        }

        /// <summary>
        /// Devuelve la lista de entidades relacionadas con la entidad.
        /// </summary>
        /// <returns>Lista de propiedades</returns>
        public List<Propiedad> ObtenerEntidadesRelacionadas()
        {
            List<Propiedad> propiedades = new List<Propiedad>();

            foreach (Propiedad propiedad in Propiedades)
            {
                if (propiedad.Tipo.Equals(TipoPropiedad.ObjectProperty))
                    propiedades.Add(propiedad);
            }
            return propiedades;
        }

        /// <summary>
        /// Devuelve el tipo de entidad en una cadena de caracteres
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.TipoEntidad;
        }

        /// <summary>
        /// Devuelve la entidad como una cadena de caracteres
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pEscribirEntidadesRelacionadas">TRUE si se deben incluir en la cadena las entidades relacionadas</param>
        /// <returns>Cadena de caracteres que representa a la entidad</returns>
        public static string ToString(ElementoOntologia pEntidad, bool pEscribirEntidadesRelacionadas)
        {
            if (pEscribirEntidadesRelacionadas)
                return pEntidad.ToString();
            else
            {
                string resultado = pEntidad.TipoEntidad + ": " + pEntidad.Descripcion + "\r\n";

                //Sólo los atributos
                List<Propiedad> atributos = pEntidad.ObtenerAtributos();

                foreach (Propiedad propiedad in atributos)
                {
                    if (propiedad.Visible)
                    {
                        foreach (string valor in propiedad.ListaValores.Keys)
                        {
                            resultado += propiedad.Nombre + ": ";
                            resultado += valor + "\r\n";
                        }
                    }
                }
                return resultado;
            }
        }

        /// <summary>
        /// Devuelve la entidad relacionada con el identificador pasado por parámetro
        /// </summary>
        /// <param name="pID">Identificador de la entidad relacionada</param>
        /// <returns>Entidad relacionada</returns>
        public ElementoOntologia getEntidadRelacionada(string pID)
        {
            foreach (ElementoOntologia entidad in this.EntidadesRelacionadas)
            {
                if (entidad.ID.Equals(pID))
                    return entidad;
            }
            return null;
        }

        /// <summary>
        /// Clona en ésta entidad la entidad pasada por parámetro
        /// </summary>
        /// <param name="pEntidad">Entidad que hay que clonar</param>
        public void ClonarEntidad(ElementoOntologia pEntidad)
        {
            Descripcion = pEntidad.Descripcion;
            mEntidadesRelacionadas = pEntidad.EntidadesRelacionadas;
            EntidadValida = pEntidad.EntidadValida;
            mEspecializaciones = pEntidad.Especializaciones;
            Generalizacion = pEntidad.Generalizacion;
            ID = pEntidad.ID;
            mPropiedades = pEntidad.Propiedades;
            mRestricciones = pEntidad.Restricciones;
            mSubclases = pEntidad.Subclases;
            Superclases = pEntidad.Superclases;
            mTipoEntidad = pEntidad.TipoEntidad;
            mListaPropiedadesImprimibles = pEntidad.mListaPropiedadesImprimibles;
        }

        /// <summary>
        /// Devuelve la propiedad de la entidad cuyo nombre coincide con el especificado.
        /// </summary>
        /// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        /// <returns>propiedad de la entidad cuyo nombre coincide con el especificado</returns>
        public Propiedad ObtenerPropiedad(string pNombrePropiedad)
        {
            foreach (Propiedad propiedad in Propiedades)
            {
                if (propiedad.Nombre == pNombrePropiedad)
                {
                    if (propiedad.ElementoOntologia == null)
                    {
                        propiedad.ElementoOntologia = this;
                    }

                    return propiedad;
                }
            }

            return null;
        }

        /// <summary>
        /// Devuelve la propiedad de la entidad cuyo nombre coincide con el especificado.
        /// </summary>
        /// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        /// <returns>propiedad de la entidad cuyo nombre coincide con el especificado</returns>
        public Propiedad ObtenerPropiedadPorNombreOUri(string pNombrePropiedad)
        {
            foreach (Propiedad propiedad in Propiedades)
            {
                if (propiedad.Nombre == pNombrePropiedad || propiedad.NombreFormatoUri == pNombrePropiedad)
                {
                    return propiedad;
                }
            }

            return null;
        }

        /// <summary>
        /// Devuelve la 1ª propiedad de la entidad cuyo rango coincide con el especificado.
        /// </summary>
        /// <param name="pRangoPropiedad">Rango de la propiedad</param>
        /// <returns>1ª propiedad de la entidad cuyo rango coincide con el especificado</returns>
        public Propiedad ObtenerPropiedadPorRango(string pRangoPropiedad)
        {
            foreach (Propiedad propiedad in Propiedades)
            {
                if (propiedad.Rango == pRangoPropiedad)
                {
                    return propiedad;
                }
            }

            return null;
        }

        /// <summary>
        /// Da valor a la propiedad ID de la entidad a partir de un ID relativo.
        /// </summary>
        /// <param name="pIDRelativo">Identificador relativo</param>
        public virtual void EstablecerID(string pIDRelativo)
        {
            this.ID = GestionOWL.ObtenerUrlEntidad(TipoEntidad) + pIDRelativo;
        }

        /// <summary>
        /// Obtiene el tipo de la entidad sin apaños para repeticiones.
        /// </summary>
        /// <param name="pTipoEntidad">Tipo entidad</param>
        /// <returns>Tipo de la entidad sin apaños para repeticiones</returns>
        public static string ObtenerTiposEntidadLimpiaDeApanioRepeticiones(string pTipoEntidad)
        {
            if (pTipoEntidad.Contains("_bis"))
            {
                return pTipoEntidad.Substring(0, pTipoEntidad.IndexOf("_bis"));
            }

            return pTipoEntidad;
        }

        #endregion

        #region Privados

        /// <summary>
        /// Indica si una clase pertenece a una ontología importada.
        /// </summary>
        /// <param name="pClase">Nombre de la clase</param>
        /// <returns>TRUE si la clase pertenece a una ontología importada, FALSE si no</returns>
        public bool EsClaseOntologiaImportada(string pClase)
        {
            foreach (string ontoImport in Ontologia.UrlOntologiasImportadas)
            {
                if (pClase.StartsWith(ontoImport))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #endregion

        #region Miembros de IEquatable<Entidad>

        /// <summary>
        /// Devuelve verdad si la entidad dada tiene el mismo GUID que la original.
        /// </summary>
        /// <param name="other">Entidad a comparar</param>
        /// <returns></returns>
        public bool Equals(ElementoOntologia other)
        {
            return this.ID.Equals(other.ID);
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
        ~ElementoOntologia()
        {
            //Libero los recursos
            Dispose(false, false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        public void Dispose()
        {
            Dispose(true, false);

            //impido que se finalice dos veces este objeto
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        /// <param name="pEliminarListasComunes">Indica si hay que eliminar listas comunes entre elementos ontologías copiados</param>
        protected virtual void Dispose(bool disposing, bool pEliminarListasComunes)
        {
            if (!disposed)
            {
                disposed = true;
                if (disposing)
                {
                    //Libero todos los recursos administrados que he añadido a esta clase

                    if (mEntidadesRelacionadas != null)
                    {
                        foreach (ElementoOntologia elemento in mEntidadesRelacionadas)
                        {
                            elemento.Dispose();
                        }
                        mEntidadesRelacionadas.Clear();

                    }

                    if (mPropiedadesOrdenadas != null)
                    {
                        mPropiedadesOrdenadas.Clear();
                    }

                    if (mPropiedades != null)
                    {
                        foreach (Propiedad propiedad in mPropiedades)
                        {
                            propiedad.Dispose();
                        }
                        mPropiedades.Clear();
                    }

                    if (mEspecifEntidad != null)
                    {
                        mEspecifEntidad.Dispose();
                    }

                    if (pEliminarListasComunes)
                    {
                        if (mEspecializaciones != null)
                            mEspecializaciones.Clear();

                        if (mListaPropiedadesImprimibles != null)
                            mListaPropiedadesImprimibles.Clear();

                        if (mRestricciones != null)
                        {
                            foreach (Restriccion restriccion in mRestricciones)
                            {
                                restriccion.Dispose();
                            }
                            mRestricciones.Clear();
                        }
                        mSubclases.Clear();

                        if (mLabelIdioma != null)
                        {
                            mLabelIdioma.Clear();
                        }

                        if (mSuperclases != null)
                        {
                            mSuperclases.Clear();
                        }

                        if (mSuperclasesUtiles != null)
                        {
                            mSuperclasesUtiles.Clear();
                        }
                    }
                }

                mPadre = null;
                mElemento = null;

                mEntidadesRelacionadas = null;
                mEspecializaciones = null;
                mListaPropiedadesImprimibles = null;

                mPropiedades = null;

                mRestricciones = null;

                mSubclases = null;
                mLabelIdioma = null;
                mSuperclases = null;
                mSuperclasesUtiles = null;
                mOntologia = null;
                mPropiedadesOrdenadas = null;
                mEspecifEntidad = null;
            }
        }

        #endregion

        #region Miembros de ISerializable

        /// <summary>
        /// Método para serializar el objeto
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Descripcion", Descripcion);
            info.AddValue("EntidadesRelacionadas", EntidadesRelacionadas);
            info.AddValue("Especializaciones", Especializaciones);
            info.AddValue("EstaCompleta", EstaCompleta);
            info.AddValue("Generalizacion", Generalizacion);
            info.AddValue("ID", ID);
            info.AddValue("ListaPropiedadesImprimibles", ListaPropiedadesImprimibles);
            info.AddValue("NamespaceOntologia", NamespaceOntologia);
            info.AddValue("Ontologia", Ontologia);
            info.AddValue("PermitePadre", PermitePadre);
            info.AddValue("Propiedades", Propiedades);
            info.AddValue("PropiedadesOrdenadas", PropiedadesOrdenadas);
            info.AddValue("Restricciones", Restricciones);
            info.AddValue("Subclases", Subclases);
            info.AddValue("Superclases", Superclases);
            info.AddValue("TipoEntidad", TipoEntidad);
            info.AddValue("TipoEntidadRelativo", TipoEntidadRelativo);
            info.AddValue("UrlOntologia", UrlOntologia);

            info.AddValue("EspecifEntidad", EspecifEntidad);
        }

        #endregion
    }
}
