using System;
using System.Collections.Generic;
using Es.Riam.Interfaces;
using System.Runtime.Serialization;
using Es.Riam.Semantica.Plantillas;
using System.Linq;

namespace Es.Riam.Semantica.OWL
{
    #region Enumeraciones

    /// <summary>
    /// Enumeración de tipos de propiedad.
    /// </summary>
    public enum TipoPropiedad
    {
        /// <summary>
        /// Tipo de propiedad cuyos valores van a ser un tipo de valor conocido
        /// </summary>
        DatatypeProperty,

        /// <summary>
        /// Tipo de propiedad cuyos valores van a ser un tipo de valor definido en la ontología
        /// </summary>
        ObjectProperty
    }

    #endregion

    /// <summary>
    /// Representa el tipo de una propiedad.
    /// </summary>

    [Serializable]
    public class UtilTipoPropiedad
    {
        #region Métodos generales

        #region Públicos

        /// <summary>
        /// Devuelve la propiedad del tipo de propiedad solicitado.
        /// </summary>
        /// <param name="pTipoPropiedad">tipo de propiedad</param>
        /// <returns></returns>
        public static TipoPropiedad getTipoPropiedad(string pTipoPropiedad)
        {
            TipoPropiedad tipoPropiedad = TipoPropiedad.DatatypeProperty;
            switch (pTipoPropiedad)
            {
                case "DatatypeProperty":
                    tipoPropiedad = TipoPropiedad.DatatypeProperty;
                    break;
                case "ObjectProperty":
                    tipoPropiedad = TipoPropiedad.ObjectProperty;
                    break;
            }
            return tipoPropiedad;
        }

        /// <summary>
        /// Devuelve una cadena de caracteres que representa el tipo de propiedad.
        /// </summary>
        /// <param name="pTipoPropiedad">Tipo de propiedad.</param>
        /// <returns></returns>
        public static string TipoPropiedadToString(TipoPropiedad pTipoPropiedad)
        {
            switch (pTipoPropiedad)
            {
                case TipoPropiedad.DatatypeProperty:
                    return "DatatypeProperty";
                case TipoPropiedad.ObjectProperty:
                    return "ObjectProperty";
                default:
                    return "DatatypeProperty";
            }
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Propiedad de entidades.
    /// </summary>
    [Serializable]
    public class Propiedad : IEquatable<Propiedad>, IDisposable, ISerializable
    {
        private const string NAMESPACE_XMLSCHEMA = "http://www.w3.org/2001/XMLSchema#";

        #region Miembros

        /// <summary>
        /// Tipo de propiedad
        /// </summary>
        private TipoPropiedad mTipo;

        /// <summary>
        /// Nombre real de la propiedad.
        /// </summary>
        protected string mNombreReal;

        /// <summary>
        /// Nombre de la propiedad.
        /// </summary>
        private string mNombre;

        /// <summary>
        /// Valores que adquiere la propiedad en una determinada entidad.
        /// </summary>
        private Dictionary<string, ElementoOntologia> mListaValores;

        /// <summary>
        /// Lista de valores usados por la propiedad.
        /// </summary>
        private List<string> mListaValoresUsados;

        /// <summary>
        /// Propiedad de la cual se repite la actual.
        /// </summary>
        private Propiedad mPropiedadRepetidaDe;

        /// <summary>
        /// Propiedad de la cual se repite la actual.
        /// </summary>
        private string mNombrePropiedadRepetidaDe;

        /// <summary>
        /// Dominios de entidades que puede tener esta propiedad.
        /// </summary>
        private List<string> mDominios;

        /// <summary>
        /// Tipo de dato al que pertenecerá el valor de la propiedad.
        /// </summary>
        private string mRango;

        /// <summary>
        /// Obtiene o establece el tipo RELATIVO (Sin #) de dato al que pertenecerá el valor de la propiedad.
        /// </summary>
        public string mRangoRelativo;

        /// <summary>
        /// Indica si la propiedad es funcional o no.
        /// </summary>
        private bool mFunctionalProperty;

        /// <summary>
        /// Indica si la propiedad está seleccionada y se exportará con la entidad.
        /// </summary>
        private bool mSeleccionada;

        /// <summary>
        /// Indica si la propiedad ha sido heredada de una entidad superior.
        /// </summary>
        private bool mHeredada;

        /// <summary>
        /// Indica si la propiedad va a ser visible para el usuario.
        /// </summary>
        private bool mVisible;

        /// <summary>
        /// Almacena la propiedad inversa a ésta.
        /// </summary>
        private Propiedad mPropiedadInversa;

        /// <summary>
        /// Almacena todas las propiedades pertenecientes a otras entidades que son inversas a ésta
        /// </summary>
        private IList<Propiedad> mListaPropiedadesInversas;

        /// <summary>
        /// Entidad que posee la propiedad.
        /// </summary>
        protected ElementoOntologia mElementoOntologia;

        /// <summary>
        /// Propiedad equivalente
        /// </summary>
        private Propiedad mPropiedadEquivalente;

        /// <summary>
        /// Valor de la propieda en caso de que ésta se funcional
        /// </summary>
        private KeyValuePair<string, ElementoOntologia> mUnicoValor;

        /// <summary>
        /// Obtiene los valores según el idioma seleccionado.
        /// </summary>
        private Dictionary<string, Dictionary<string, ElementoOntologia>> mListaValoresIdioma;

        /// <summary>
        /// Obtiene o establece el valor que indica si el elemento ya se imprimió en el documento
        /// </summary>
        private bool mElementoImpreso;

        /// <summary>
        /// Lista con los valores permitidos para la propiedad.
        /// </summary>
        private List<string> mListaValoresPermitidos;

        /// <summary>
        /// Contiene el valor por defecto que debe tener la propiedad, pero que no es correcto para la misma (Ej: valor gris combo).
        /// </summary>
        private string mValorDefectoNoSeleccionable;

        /// <summary>
        /// TRUE si la cardinalidad es menor o igual que uno, FALSE si no posee la restriccion o la cardinalidad es diferente.
        /// </summary>
        private bool? mCardinalidadMenorOIgualUno;

        /// <summary>
        /// Devuelve el numero de cardinalidad minima 
        /// </summary>
        private int? mCardinalidadMinima;

        /// <summary>
        ///Devuelve el numero de cardinalidad máxima 
        /// </summary>
        private int? mCardinalidadMaxima;

        /// <summary>
        /// Devuelve el tipo de entidad al que representa la propiedad en un formulario semántico.
        /// </summary>
        private string mTipoEntidadRepresenta;

        /// <summary>
        /// Ontología a la que pertenece el elemento.
        /// </summary>
        private Ontologia mOntologia;

        /// <summary>
        /// SuperPropiedades de la propiedad.
        /// </summary>
        private List<string> mSuperPropiedades;

        /// <summary>
        /// Configuración de la plantilla.
        /// </summary>
        private EstiloPlantillaEspecifProp mEspecifPropiedad;

        /// <summary>
        /// Label.
        /// </summary>
        private string mLabel;

        /// <summary>
        /// Label.
        /// </summary>
        private Dictionary<string, string> mLabelIdioma;

        #endregion

        #region Constructores

        /// <summary>
        /// Crea una propiedad a partir de sus atributos.
        /// </summary>
        /// <param name="pNombre">nombre</param>
        /// <param name="pTipo">tipo de propiedad</param>
        /// <param name="pDominio">dominio</param>
        /// <param name="pRango">rango</param>
        /// <param name="pFunctionalProperty">verdad si la propiedad es funcional.</param>
        /// <param name="pOntologia">Ontología a la que pertenece el elemento</param>
        public Propiedad(string pNombre, TipoPropiedad pTipo, List<string> pDominio, string pRango, bool pFunctionalProperty, Ontologia pOntologia)
        {
            if ((pNombre != "") && (pDominio != null) && (pRango != null))
            {
                this.mTipo = pTipo;
                this.mListaValores = new Dictionary<string, ElementoOntologia>();
                this.mUnicoValor = new KeyValuePair<string,ElementoOntologia>();
                this.mListaValoresIdioma = new Dictionary<string, Dictionary<string, ElementoOntologia>>();
                this.mDominios = pDominio;
                this.mRango = pRango;
                this.mFunctionalProperty = pFunctionalProperty;
                this.mNombre = pNombre;
                
                this.mSeleccionada = true;
                this.mHeredada = false;
                this.mNombreReal = this.Nombre;
                this.mVisible = true;
                this.mPropiedadInversa = null;
                this.mListaPropiedadesInversas = new List<Propiedad>();
                this.mElementoOntologia = null;
                this.mPropiedadEquivalente = null;
                this.mOntologia = pOntologia;
                this.mSuperPropiedades = new List<string>();
            }
            else
                throw new ArgumentException("Argumento inválido.");
        }

        /// <summary>
        /// Crea una propiedad a partir de sus atributos.
        /// </summary>
        /// <param name="pNombre">nombre</param>
        /// <param name="pTipo">tipo de propiedad</param>
        /// <param name="pOntologia">Ontología a la que pertenece el elemento</param>
        public Propiedad(string pNombre, TipoPropiedad pTipo, Ontologia pOntologia)
        {
            if (pNombre != "")
            {
                this.mTipo = pTipo;
                this.mListaValores = new Dictionary<string, ElementoOntologia>();
                this.mUnicoValor = new KeyValuePair<string,ElementoOntologia>();
                this.mListaValoresIdioma = new Dictionary<string, Dictionary<string, ElementoOntologia>>();
                this.mDominios = new List<string>();
                this.mRango = "";
                this.mFunctionalProperty = false;
                this.mNombre = pNombre;
                
                this.mSeleccionada = true;
                this.mHeredada = false;
                this.mNombreReal = this.Nombre;
                this.mVisible = true;
                this.mPropiedadInversa = null;
                this.mListaPropiedadesInversas = new List<Propiedad>();
                this.mElementoOntologia = null;
                this.mPropiedadEquivalente = null;
                this.mOntologia = pOntologia;
                this.mSuperPropiedades = new List<string>();
            }
            else
                throw new ArgumentException("Argumento inválido.");
        }

        /// <summary>
        /// Crea una propiedad a partir de otra.
        /// </summary>
        /// <param name="pPropiedad">propiedad que se tomará como referencia.</param>
        public Propiedad(Propiedad pPropiedad)
        {
            if (pPropiedad != null)
            {
                this.mTipo = pPropiedad.Tipo;
                this.mListaValores = new Dictionary<string,ElementoOntologia>();
                foreach (string valor in pPropiedad.ListaValores.Keys)
                {
                    this.mListaValores.Add(valor, pPropiedad.ListaValores[valor]);
                }
                this.mUnicoValor = pPropiedad.UnicoValor;
                this.mListaValoresIdioma = new Dictionary<string, Dictionary<string, ElementoOntologia>>();
                foreach (string valor in pPropiedad.ListaValoresIdioma.Keys)
                {
                    this.mListaValoresIdioma.Add(valor, new Dictionary<string,ElementoOntologia>());

                    foreach (string valorInt in pPropiedad.ListaValoresIdioma[valor].Keys)
                    {
                        pPropiedad.ListaValoresIdioma[valor].Add(valorInt, pPropiedad.ListaValoresIdioma[valor][valorInt]);
                    }
                }
                //this.mDominios = new List<string>(pPropiedad.Dominio);
                this.mDominios = pPropiedad.Dominio;
                this.mRango = pPropiedad.Rango;
                this.mFunctionalProperty = pPropiedad.FunctionalProperty;
                this.mNombre = pPropiedad.Nombre;
                this.mSeleccionada = true;
                this.mHeredada = pPropiedad.Heredada;
                //this.mNombreReal = pPropiedad.NombreReal;
                this.mVisible = pPropiedad.Visible;
                this.mPropiedadInversa = pPropiedad.PropiedadInversa;
                this.mListaPropiedadesInversas = pPropiedad.ListaPropiedadesInversas;
                this.mElementoOntologia = pPropiedad.ElementoOntologia;
                this.mPropiedadEquivalente = pPropiedad.PropiedadEquivalente;
                this.mListaValoresPermitidos = pPropiedad.ListaValoresPermitidos;
                this.mOntologia = pPropiedad.Ontologia;
                this.mSuperPropiedades = pPropiedad.SuperPropiedades;
                this.mNombrePropiedadRepetidaDe = pPropiedad.NombrePropiedadRepetidaDe;
                this.mListaValoresUsados = pPropiedad.ListaValoresUsados;
                this.mLabel = pPropiedad.Label;
                this.mLabelIdioma = pPropiedad.LabelIdioma;
            }
            else
                throw new ArgumentNullException("pPropiedad", "El argumento no puede ser nulo.");
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected Propiedad(SerializationInfo info, StreamingContext context)
        {
            mCardinalidadMenorOIgualUno = (bool?)info.GetValue("CardinalidadMenorOIgualUno", typeof(bool?));
            mDominios = (List<string>)info.GetValue("Dominios", typeof(List<string>));
            mElementoImpreso = (bool)info.GetValue("ElementoImpreso", typeof(bool));
            mElementoOntologia = (ElementoOntologia)info.GetValue("ElementoOntologia", typeof(ElementoOntologia));
            mFunctionalProperty = (bool)info.GetValue("FunctionalProperty", typeof(bool));
            mHeredada = (bool)info.GetValue("Heredada", typeof(bool));
            mListaPropiedadesInversas = (IList<Propiedad>)info.GetValue("ListaPropiedadesInversas", typeof(IList<Propiedad>));
            mListaValores = (Dictionary<string, ElementoOntologia>)info.GetValue("ListaValores", typeof(Dictionary<string, ElementoOntologia>));
            mListaValoresPermitidos = (List<string>)info.GetValue("ListaValoresPermitidos", typeof(List<string>));
            mNombre = (string)info.GetValue("Nombre", typeof(string));
            mNombreReal = (string)info.GetValue("NombreReal", typeof(string));
            mOntologia = (Ontologia)info.GetValue("Ontologia", typeof(Ontologia));  
            mPropiedadEquivalente = (Propiedad)info.GetValue("PropiedadEquivalente", typeof(Propiedad));
            mPropiedadInversa = (Propiedad)info.GetValue("PropiedadInversa", typeof(Propiedad));
            mRango = (string)info.GetValue("Rango", typeof(string));
            mRangoRelativo = (string)info.GetValue("RangoRelativo", typeof(string));
            mSeleccionada = (bool)info.GetValue("Seleccionada", typeof(bool));
            mSuperPropiedades = (List<string>)info.GetValue("SuperPropiedades", typeof(List<string>));
            mTipo = (TipoPropiedad)info.GetValue("Tipo", typeof(TipoPropiedad));
            mTipoEntidadRepresenta = (string)info.GetValue("TipoEntidadRepresenta", typeof(string));
            mUnicoValor = (KeyValuePair<string, ElementoOntologia>)info.GetValue("UnicoValor", typeof(KeyValuePair<string, ElementoOntologia>));
            mListaValoresIdioma = (Dictionary<string, Dictionary<string, ElementoOntologia>>)info.GetValue("ListaValoresIdioma", typeof(Dictionary<string, Dictionary<string, ElementoOntologia>>));
            mValorDefectoNoSeleccionable = (string)info.GetValue("ValorDefectoNoSeleccionable", typeof(string));
            mVisible = (bool)info.GetValue("Visible", typeof(bool));
            mLabel = (string)info.GetValue("Label", typeof(string));
            mLabelIdioma = (Dictionary<string, string>)info.GetValue("LabelIdioma", typeof(Dictionary<string, string>));

            mEspecifPropiedad = (EstiloPlantillaEspecifProp)info.GetValue("EspecifPropiedad", typeof(EstiloPlantillaEspecifProp));
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el tipo de propiedad
        /// </summary>
        public TipoPropiedad Tipo
        {
            set
            {
                this.mTipo = value;
            }
            get
            {
                return mTipo;
            }
        }

        /// <summary>
        /// Indica si la propiedad será visible para el usuario.
        /// </summary>
        public bool Visible
        {
            set
            {
                this.mVisible = value;
            }
            get
            {
                return this.mVisible;
            }
        }

        /// <summary>
        /// Obtiene o establece el nombre real de la propiedad, con acentos y espacios.
        /// </summary>
        public virtual string NombreReal
        {
            set
            {
                this.mNombreReal = value;
            }
            get
            {
                mNombreReal = Nombre;
                return this.mNombreReal;
            }
        }

        /// <summary>
        /// Indica si la propiedad ha sido heredada de una entidad superior.
        /// </summary>
        public bool Heredada
        {
            set
            {
                this.mHeredada = value;
            }
            get
            {
                return this.mHeredada;
            }
        }

        /// <summary>
        /// Obtiene o establece los valores que adquiere la propiedad en una determinada entidad.
        /// </summary>
        public Dictionary<string, ElementoOntologia> ListaValores
        {
            get
            {
                if (NombrePropiedadRepetidaDe != null)
                {
                    if (PropiedadRepetidaDe == null && ElementoOntologia != null)
                    {
                        PropiedadRepetidaDe = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(NombrePropiedadRepetidaDe, ElementoOntologia);
                    }

                    if (PropiedadRepetidaDe != null)
                    {
                        Dictionary<string, ElementoOntologia> listaValoresAux = new Dictionary<string, ElementoOntologia>();

                        foreach (string key in PropiedadRepetidaDe.ListaValores.Keys)
                        {
                            if (!PropiedadRepetidaDe.ListaValoresUsados.Contains(key))
                            {
                                listaValoresAux.Add(key, PropiedadRepetidaDe.ListaValores[key]);
                            }
                        }

                        return listaValoresAux;
                    }
                }

                return mListaValores;
            }
            set
            {
                mListaValores = value;
            }
        }

        /// <summary>
        /// Obtiene o establece los valores que adquiere la propiedad en una determinada entidad.
        /// </summary>
        public Dictionary<string, ElementoOntologia> ListaValoresOrdCampoEntidad
        {
            get
            {
                if (EntidadesHijasConOrden)
                {
                    Dictionary<string, ElementoOntologia> listaValOrd = new Dictionary<string, ElementoOntologia>();
                    SortedDictionary<int, List<string>> listaOrden = new SortedDictionary<int, List<string>>();

                    foreach (string entidadID in ListaValores.Keys)
                    {
                        int orden = ListaValores[entidadID].OrdenEntiad;

                        if (!listaOrden.ContainsKey(orden))
                        {
                            listaOrden.Add(orden, new List<string>());
                        }

                        listaOrden[orden].Add(entidadID);
                    }

                    foreach (int orden in listaOrden.Keys)
                    {
                        foreach (string entidadID in listaOrden[orden])
                        {
                            listaValOrd.Add(entidadID, ListaValores[entidadID]);
                        }
                    }

                    return listaValOrd;
                }
                else
                {
                    return ListaValores;
                }
            }
        }

        /// <summary>
        /// Indica si las entiades hijas de la propiedad actual se ordenan por una propiedad o no.
        /// </summary>
        public bool EntidadesHijasConOrden
        {
            get
            {
                if (ListaValores.Count > 0)
                {
                    foreach (string entidadID in ListaValores.Keys)
                    {
                        if (ListaValores[entidadID] != null && ListaValores[entidadID].EspecifEntidad.CampoOrden != null)
                        {
                            return true;
                        }
                        break;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Obtiene el único valor de la propiedad en caso de que ésta sea funcional
        /// </summary>
        public KeyValuePair<string, ElementoOntologia> UnicoValor
        {
            get
            {
                if (NombrePropiedadRepetidaDe != null)
                {
                    if (PropiedadRepetidaDe == null && ElementoOntologia != null)
                    {
                        PropiedadRepetidaDe = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(NombrePropiedadRepetidaDe, ElementoOntologia);
                    }

                    if (PropiedadRepetidaDe != null && PropiedadRepetidaDe.UnicoValor.Key != null)
                    {
                        KeyValuePair<string, ElementoOntologia> listaValoresAux = new KeyValuePair<string, ElementoOntologia>(PropiedadRepetidaDe.UnicoValor.Key, PropiedadRepetidaDe.UnicoValor.Value);
                        return listaValoresAux;
                    }
                }

                return this.mUnicoValor;
            }
            set
            {
                this.mUnicoValor = value;
            }
        }

        /// <summary>
        /// Obtiene los valores según el idioma seleccionado.
        /// </summary>
        public Dictionary<string, Dictionary<string, ElementoOntologia>> ListaValoresIdioma
        {
            get
            {
                if (mListaValoresIdioma == null)
                {
                    mListaValoresIdioma = new Dictionary<string, Dictionary<string, ElementoOntologia>>();
                }

                return mListaValoresIdioma;
            }
            set
            {
                mListaValoresIdioma = value;
            }
        }

        /// <summary>
        /// Devuelve el string que corresponde al 1º valor de la propiedad sea del tipo que sea.
        /// </summary>
        public string PrimerValorPropiedad
        {
            get
            {
                foreach (string valor in ValoresUnificados.Keys)
                {
                    return valor;
                }

                return null;
            }
        }

        /// <summary>
        /// Lista de valores usados por la propiedad.
        /// </summary>
        public List<string> ListaValoresUsados
        {
            get
            {
                if (mListaValoresUsados == null)
                {
                    mListaValoresUsados = new List<string>();
                }

                return mListaValoresUsados;
            }
            set
            {
                mListaValoresUsados = value;
            }
        }

        /// <summary>
        /// Propiedad de la cual se repite la actual.
        /// </summary>
        public string NombrePropiedadRepetidaDe
        {
            get
            {
                return mNombrePropiedadRepetidaDe;
            }
            set
            {
                mNombrePropiedadRepetidaDe = value;
            }
        }
        
        /// <summary>
        /// Propiedad de la cual se repite la actual.
        /// </summary>
        public Propiedad PropiedadRepetidaDe
        {
            get
            {
                return mPropiedadRepetidaDe;
            }
            set
            {
                mPropiedadRepetidaDe = value;
            }
        }

        /// <summary>
        /// Obtiene o establece los dominios de entidades que puede tener esta propiedad.
        /// </summary>
        public List<string> Dominio
        {
            get
            {
                return mDominios;
            }
            set
            {
                mDominios = value;
            }
        }

        ///// <summary>
        ///// Obtiene los dominios de entidades que puede tener esta propiedad.
        ///// </summary>
        //public List<string> DominioConNamespace
        //{
        //    get
        //    {
        //        List<string> listaAux = new List<string>();
        //        foreach (string dominio in Dominio)
        //        {
        //            listaAux.Add(mNamespaceOntologia + ":" + dominio);
        //        }
        //        return listaAux;
        //    }
        //}

        /// <summary>
        /// Obtiene o establece el tipo de dato al que pertenecerá el valor de la propiedad.
        /// </summary>
        public string Rango
        {
            get
            {
                return mRango;
            }
            set
            {
                mRango = value;
            }
        }

        /// <summary>
        /// Indica si esta propiedad tiene un selector de entidad definido
        /// </summary>
        public bool TieneSelectorEntidad
        {
            get
            {
                return string.IsNullOrEmpty(mRango);
            }
        }

        /// <summary>
        /// Obtiene o establece el tipo RELATIVO (Sin #) de dato al que pertenecerá el valor de la propiedad.
        /// </summary>
        public string RangoRelativo
        {
            get
            {
                if (mRangoRelativo == null)
                {
                    mRangoRelativo = Rango;

                    if (mRangoRelativo.Contains("#"))
                    {
                        mRangoRelativo = mRangoRelativo.Substring(mRangoRelativo.LastIndexOf("#") + 1);
                    }
                    else if (mRangoRelativo.Contains("/"))
                    {
                        mRangoRelativo = mRangoRelativo.Substring(mRangoRelativo.LastIndexOf("/") + 1);
                    }
                }

                return mRangoRelativo;
            }
        }

        ///// <summary>
        ///// Obtiene el tipo de dato al que pertenecerá el valor de la propiedad.
        ///// </summary>
        //public string RangoConNamespace
        //{
        //    get
        //    {
        //        return mNamespaceOntologia + ":" + mRango;
        //    }
        //}

        /// <summary>
        /// Obtiene o establece si la propiedad es funcional o no
        /// </summary>
        public bool FunctionalProperty
        {
            get
            {
                return mFunctionalProperty;
            }
            set
            {
                mFunctionalProperty = value;
            }
        }

        /// <summary>
        /// Obtiene el nombre de la propiedad
        /// </summary>
        public virtual string Nombre
        {
            get
            {
                return this.mNombre;
            }
        }

        /// <summary>
        /// Obtiene el nombre de la propiedad incluyendo el namespace
        /// </summary>
        public virtual string NombreConNamespace
        {
            get
            {
                if (!mNombre.Contains("#") && !mNombre.Contains("/"))
                {
                    return mOntologia.GestorOWL.NamespaceOntologia + ":" + this.mNombre;
                }
                else
                {
                    string urlNamespace = null;
                    string nombrePropRelativo = null;

                    if (mNombre.Contains("#"))
                    {
                        urlNamespace = mNombre.Substring(0, mNombre.LastIndexOf("#") + 1);
                        nombrePropRelativo = mNombre.Substring(mNombre.LastIndexOf("#") + 1);
                    }
                    else //Contiene '/'
                    {
                        urlNamespace = mNombre.Substring(0, mNombre.LastIndexOf("/") + 1);
                        nombrePropRelativo = mNombre.Substring(mNombre.LastIndexOf("/") + 1);
                    }

                    if (Ontologia.GenararNamespacesHuerfanos && !Ontologia.NamespacesDefinidos.ContainsKey(urlNamespace))
                    {
                        Ontologia.GenararNamespaceUrl(urlNamespace);
                    }

                    if (Ontologia.NamespacesDefinidos.ContainsKey(urlNamespace))
                    {
                        return Ontologia.NamespacesDefinidos[urlNamespace] + ":" + nombrePropRelativo;
                    }
                    else
                    {
                        return mNombre;
                    }
                }
            }
        }

        /// <summary>
        /// Devuelve el nombre con formato de URI.
        /// </summary>
        public string NombreFormatoUri
        {
            get
            {
                if (Nombre.StartsWith("http"))
                {
                    return Nombre;
                }
                else
                {
                    return Ontologia.GestorOWL.UrlOntologia + Nombre;
                }
            }
        }

        /// <summary>
        /// Obtiene el nombre de la propiedad para generar los ids de las propiedades.
        /// </summary>
        public virtual string NombreGeneracionIDs
        {
            get
            {
                return Nombre.Replace("/", "_").Replace(":", "_").Replace(".", "_").Replace("#", "_");
            }
        }

        /// <summary>
        /// Obtiene el nombre de la propiedad para generar el nombre de las clases.
        /// </summary>
        public virtual string NombreGeneracionClases
        {
            get
            {
                if (Nombre.Contains("#"))
                {
                    return Nombre.Substring(Nombre.LastIndexOf("#") + 1);
                }
                else if (Nombre.Contains("/"))
                {
                    return Nombre.Substring(Nombre.LastIndexOf("/") + 1);
                }
                else
                {
                    return Nombre;
                }
            }
        }

        /// <summary>
        /// Obtiene o establece el estado de selección de la propiedad.
        /// </summary>
        public bool Seleccionada
        {
            get
            {
                return this.mSeleccionada;
            }
            set
            {
                this.mSeleccionada = value;
            }
        }

        /// <summary>
        /// Obtiene o establece la propiedad inversa de ésta.
        /// </summary>
        public Propiedad PropiedadInversa
        {
            set
            {
                this.mPropiedadInversa = value;
            }
            get
            {
                return this.mPropiedadInversa;
            }
        }

        /// <summary>
        /// Obtiene todas las propiedades pertenecientes a otras entidades que son inversas a ésta
        /// </summary>
        public IList<Propiedad> ListaPropiedadesInversas
        {
            get
            {
                return this.mListaPropiedadesInversas;
            }
        }

        /// <summary>
        /// Obtiene o establece el elemento de ontología al que pertenece.
        /// </summary>
        public ElementoOntologia ElementoOntologia
        {
            get
            {
                return this.mElementoOntologia;
            }
            set
            {
                this.mElementoOntologia = value;

                //Reinicio cardinalidad, ya que la entidad ha cambiado:
                this.mCardinalidadMenorOIgualUno = null;
            }
        }

        /// <summary>
        /// Obtiene o establece la propiedad equivalente a ésta.
        /// </summary>
        public Propiedad PropiedadEquivalente
        {
            set
            {
                this.mPropiedadEquivalente = value;
            }
            get
            {
                return this.mPropiedadEquivalente;
            }
        }

        /// <summary>
        /// Devuelve o establece la lista de valores permitidos de la propiedad (One Of).
        /// </summary>
        public List<string> ListaValoresPermitidos
        {
            get
            {
                if (mListaValoresPermitidos == null)
                {
                    mListaValoresPermitidos = EspecifPropiedad.ListaValoresPermitidos;
                    mValorDefectoNoSeleccionable = EspecifPropiedad.ValorDefectoNoSeleccionable;
                }

                return mListaValoresPermitidos;
            }
            set
            {
                mListaValoresPermitidos = value;
            }
        }

        /// <summary>
        /// Contiene el valor por defecto que debe tener la propiedad, pero que no es correcto para la misma (Ej: valor gris combo).
        /// </summary>
        public string ValorDefectoNoSeleccionable
        {
            get
            {
                if (mListaValoresPermitidos == null)
                {
                    mListaValoresPermitidos = EspecifPropiedad.ListaValoresPermitidos;
                    mValorDefectoNoSeleccionable = EspecifPropiedad.ValorDefectoNoSeleccionable;
                }
                return mValorDefectoNoSeleccionable;
            }
            set
            {
                mValorDefectoNoSeleccionable = value;
            }
        }

        /// <summary>
        /// Indica si el rango de la propiedad es numérico.
        /// </summary>
        public bool RangoEsNumerico
        {
            get
            {
                return (Rango.StartsWith(NAMESPACE_XMLSCHEMA) && (RangoRelativo.ToLower() == "int" || RangoRelativo.ToLower() == "integer" || RangoRelativo.ToLower() == "float"));
            }
        }

        /// <summary>
        /// Indica si el rango de la propiedad es entero.
        /// </summary>
        public bool RangoEsEntero
        {
            get
            {
                return (Rango.StartsWith(NAMESPACE_XMLSCHEMA) && (RangoRelativo.ToLower() == "int" || RangoRelativo.ToLower() == "integer"));
            }
        }

        /// <summary>
        /// Indica si el rango de la propiedad es un número real.
        /// </summary>
        public bool RangoEsFloat
        {
            get
            {
                return (Rango.StartsWith(NAMESPACE_XMLSCHEMA) && (RangoRelativo.ToLower() == "float"));
            }
        }

        /// <summary>
        /// Indica si el rango de la propiedad es de tipo fecha.
        /// </summary>
        public bool RangoEsFecha
        {
            get
            {
                return (Rango.StartsWith(NAMESPACE_XMLSCHEMA) && (RangoRelativo.ToLower() == "datetime" || RangoRelativo.ToLower() == "date" || RangoRelativo.ToLower() == "time"));
            }
        }

        /// <summary>
        /// Obtiene o establece un valor que indica si el elemento se imprimirá
        /// </summary>
        public bool SeDebeImprimir
        {
            get
            {
                return this.mSeleccionada;
            }
            set
            {
                this.mSeleccionada = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el valor que indica si el elemento ya se imprimió en el documento
        /// </summary>
        public bool EstaImpreso
        {
            get
            {
                return this.mElementoImpreso;
            }
            set
            {
                this.mElementoImpreso = value;
            }
        }

        /// <summary>
        /// Devuelve TRUE si la cardinalidad es menor o igual que uno, FALSE si no posee la restriccion o la cardinalidad es diferente.
        /// </summary>
        public bool CardinalidadMenorOIgualUno
        {
            get
            {
                if (mCardinalidadMenorOIgualUno == null)
                {
                    mCardinalidadMenorOIgualUno = false;

                    if (ElementoOntologia.Restricciones.Count > 0)
                    {
                        foreach (Restriccion restriccion in ElementoOntologia.Restricciones)
                        {
                            if (Nombre.Equals(restriccion.Propiedad) && (restriccion.TipoRestriccion == TipoRestriccion.Cardinality || restriccion.TipoRestriccion == TipoRestriccion.MaxCardinality))
                            {
                                mCardinalidadMenorOIgualUno = (restriccion.Valor == "1");
                                break;
                            }
                        }
                    }
                }

                return mCardinalidadMenorOIgualUno.Value;
            }
            set
            {
                mCardinalidadMenorOIgualUno = value;
            }
        }



        public int CardinalidadMinima
        {
            get
            {
                if (mCardinalidadMinima == null)
                {
                    mCardinalidadMinima = 0;

                    if (ElementoOntologia.Restricciones.Any())
                    {
                        Restriccion restriccion = ElementoOntologia.Restricciones.FirstOrDefault(x => (Nombre.Equals(x.Propiedad) && (x.TipoRestriccion == TipoRestriccion.Cardinality || x.TipoRestriccion == TipoRestriccion.MinCardinality)));
                        if (restriccion != null)
                        {
                            mCardinalidadMinima = int.Parse(restriccion.Valor);
                        }
                    }
                }

                return mCardinalidadMinima.Value;
            }
            set
            {
                mCardinalidadMinima = value;
            }
        }

        public int CardinalidadMaxima
        {
            get
            {
                if (mCardinalidadMaxima == null)
                {
                    mCardinalidadMaxima = -1;

                    if (ElementoOntologia.Restricciones.Any())
                    {
                        Restriccion restriccion = ElementoOntologia.Restricciones.FirstOrDefault(x => (Nombre.Equals(x.Propiedad) && (x.TipoRestriccion == TipoRestriccion.Cardinality || x.TipoRestriccion == TipoRestriccion.MaxCardinality)));
                        if (restriccion != null)
                        {
                            mCardinalidadMaxima = int.Parse(restriccion.Valor);
                        }
                    }
                }

                return mCardinalidadMaxima.Value;
            }
            set
            {
                mCardinalidadMaxima = value;
            }
        }

        /// <summary>
        /// Indica si la propiedad solo puede tener un valor.
        /// </summary>
        /// 

        public bool ValorUnico
        {
            get
            {
                return (FunctionalProperty || CardinalidadMenorOIgualUno);
            }
        }

        /// <summary>
        /// Devuelve el tipo de entidad al que representa la propiedad en un formulario semántico.
        /// </summary>
        public string TipoEntidadRepresenta
        {
            get
            {
                return mTipoEntidadRepresenta;
            }
            set
            {
                mTipoEntidadRepresenta = value;
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
        /// SuperPropiedades de la propiedad.
        /// </summary>
        public List<string> SuperPropiedades
        {
            get
            {
                return mSuperPropiedades;
            }
            set
            {
                mSuperPropiedades = value;
            }
        }

        /// <summary>
        /// Obtiene o establece los valores que adquiere la propiedad en una determinada entidad.
        /// </summary>
        public Dictionary<string, ElementoOntologia> ValoresUnificados
        {
            get
            {
                if (FunctionalProperty)
                {
                    Dictionary<string, ElementoOntologia> valores = new Dictionary<string, ElementoOntologia>();

                    if (UnicoValor.Key != null)
                    {
                        valores.Add(UnicoValor.Key, UnicoValor.Value);
                    }

                    return valores;
                }
                else
                {
                    return ListaValores;
                }
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

        #region Estilos plantilla

        /// <summary>
        /// Configuración de la plantilla.
        /// </summary>
        public EstiloPlantillaEspecifProp EspecifPropiedad
        {
            get
            {
                if (mEspecifPropiedad == null || mEspecifPropiedad.Propiedad == null)
                {
                    try
                    {
                        string nombreBuscar = Nombre;

                        if (nombreBuscar.Contains("_Rep_") && !mOntologia.EstilosPlantilla.ContainsKey(nombreBuscar))
                        {
                            nombreBuscar = nombreBuscar.Substring(0, nombreBuscar.LastIndexOf("_Rep_"));
                        }

                        if (mOntologia != ElementoOntologia.Ontologia)
                        {
                            mOntologia = ElementoOntologia.Ontologia;
                        }

                        if (mOntologia.EstilosPlantilla != null && mOntologia.EstilosPlantilla.ContainsKey(nombreBuscar))
                        {
                            foreach (EstiloPlantilla estilo in mOntologia.EstilosPlantilla[nombreBuscar])
                            {
                                if (estilo is EstiloPlantillaEspecifProp)
                                {
                                    EstiloPlantillaEspecifProp estiloProp = (EstiloPlantillaEspecifProp)estilo;
                                    if (estiloProp.NombreEntidad == ElementoOntologia.TipoEntidad || ElementoOntologia.SuperclasesUtiles.Contains(estiloProp.NombreEntidad) || (ElementoOntologia.TipoEntidad.Contains("_bis") && estiloProp.NombreEntidad == ElementoOntologia.TipoEntidad.Substring(0, ElementoOntologia.TipoEntidad.IndexOf("_bis"))))
                                    {
                                        mEspecifPropiedad = new EstiloPlantillaEspecifProp((EstiloPlantillaEspecifProp)estilo);
                                        break;
                                    }
                                }
                            }

                            if (mEspecifPropiedad == null)
                            {
                                mEspecifPropiedad = new EstiloPlantillaEspecifProp((EstiloPlantillaEspecifProp)mOntologia.EstilosPlantilla[nombreBuscar][0]);
                            }
                        }
                        else
                        {
                            mEspecifPropiedad = new EstiloPlantillaEspecifProp();
                        }
                    }
                    catch (Exception)
                    {
                        mEspecifPropiedad = new EstiloPlantillaEspecifProp();
                    }

                    mEspecifPropiedad.Propiedad = this;

                    if (mListaValoresPermitidos != null && mListaValoresPermitidos.Count == 0)
                    {
                        mListaValoresPermitidos = null;
                    }
                }

                return mEspecifPropiedad;
            }
            set
            {
                mEspecifPropiedad = value;
            }
        }

        #endregion

        #endregion

        #region Métodos generales

        #region Públicos

        /// <summary>
        /// añade un dominio a la lista mDominio;
        /// </summary>
        /// <param name="pTipoEntidad"></param>
        public void AddDominio(string pTipoEntidad)
        {
            this.mDominios.Add(pTipoEntidad);
        }

        /// <summary>
        /// Agrega un valor a la propiedad
        /// </summary>
        /// <param name="pEntidad">Entidad a añadir</param>
        public void AgregarValor(ElementoOntologia pEntidad)
        {
            if (FunctionalProperty)
            {
                this.UnicoValor = new KeyValuePair<string, ElementoOntologia>(pEntidad.ID, pEntidad);
            }
            else
            {
                this.mListaValores.Add(pEntidad.ID, pEntidad);
            }

            ElementoOntologia.EntidadesRelacionadas.Add(pEntidad);
        }

        /// <summary>
        /// Agrega un valor a la propiedad
        /// </summary>
        /// <param name="pValor">Valor a agregar</param>
        public void AgregarValor(string pValor) 
        {
            if (FunctionalProperty)
            {
                this.UnicoValor = new KeyValuePair<string, ElementoOntologia>(pValor, null);
            }
            else
            {
                if (!this.ListaValores.ContainsKey(pValor))
                {
                    this.ListaValores.Add(pValor, null);
                }
            }
        }

        /// <summary>
        /// Devuelve el nombre de la propiedad como cadena de caracteres
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Nombre.ToString();
        }

        /// <summary>
        /// Da un determinado valor a la propiedad.
        /// </summary>
        /// <param name="pKey">Key del valor</param>
        /// <param name="pValue">Value del valor</param>
        public void DarValor(string pKey, ElementoOntologia pValue)
        {
            if (FunctionalProperty)
            {
                UnicoValor = new KeyValuePair<string, ElementoOntologia>(pKey, pValue);
            }
            else
            {
                ListaValores.Add(pKey, pValue);
            }
        }

        /// <summary>
        /// Limpia el valor o los valores de la propiedad.
        /// </summary>
        public void LimpiarValor()
        {
            if (ListaValoresIdioma.Count > 0)
            {
                ListaValoresIdioma.Clear();
            }
            
            if (FunctionalProperty)
            {
                UnicoValor = new KeyValuePair<string, ElementoOntologia>(null, null);
            }
            else
            {
                ListaValores.Clear();
            }
        }

        /// <summary>
        /// Limpia el valor o los valores de la propiedad.
        /// </summary>
        public void LimpiarValor(string pValor)
        {
            if (FunctionalProperty)
            {
                UnicoValor = new KeyValuePair<string, ElementoOntologia>(null, null);
            }
            else
            {
                ListaValores.Remove(pValor);
            }
        }

        /// <summary>
        /// Limpia el valor o los valores de la propiedad.
        /// </summary>
        public void LimpiarValor(ElementoOntologia pValor)
        {
            if (FunctionalProperty)
            {
                UnicoValor = new KeyValuePair<string, ElementoOntologia>(null, null);
            }
            else
            {
                ListaValores.Remove(pValor.ID);
            }

            ElementoOntologia.EntidadesRelacionadas.Remove(pValor);
        }

        /// <summary>
        /// Agrega un valor a la lista de valores usados por la propiedad.
        /// </summary>
        /// <param name="pValor">Valor</param>
        public void AgregarValorPropiedadUsado(string pValor)
        {
            if (PropiedadRepetidaDe != null)
            {
                PropiedadRepetidaDe.ListaValoresUsados.Add(pValor);
            }
            else
            {
                ListaValoresUsados.Add(pValor);
            }
        }

        /// <summary>
        /// Agrega un valor en un idioma concreto.
        /// </summary>
        /// <param name="pValor">Valor</param>
        /// <param name="pIdioma">Idioma</param>
        public void AgregarValorConIdioma(string pValor, string pIdioma)
        {
            if (!ListaValoresIdioma.ContainsKey(pIdioma))
            {
                ListaValoresIdioma.Add(pIdioma, new Dictionary<string, ElementoOntologia>());
            }

            if (!ListaValoresIdioma[pIdioma].ContainsKey(pValor))
            {
                ListaValoresIdioma[pIdioma].Add(pValor, null);
            }
        }

        /// <summary>
        /// Obtiene la lista de propiedades del idioma solicitado. Si no lo contiene devuelve el primer idioma con valor.
        /// </summary>
        /// <param name="pIdioma">Código de idioma</param>
        /// <returns>lista de valores de propiedades</returns>
        public List<string> OntenerValoresIdiomaUsuarioUOtrosSiVacio(string pIdioma)
        {
            if (ListaValoresIdioma.ContainsKey(pIdioma) && ListaValoresIdioma[pIdioma].Count > 0)
            {
                return new List<string>(ListaValoresIdioma[pIdioma].Keys);
            }
            else
            {
                foreach (Dictionary<string, ElementoOntologia> valores in ListaValoresIdioma.Values)
                {
                    if (valores.Count > 0)
                    {
                        return new List<string>(valores.Keys);
                    }
                }
            }

            return new List<string>();
        }

        /// <summary>
        /// Obtiene el primer valor de la propiedad en el idioma idicado si está, si no en otro y si no sin idioma.
        /// </summary>
        /// <param name="pIdioma">Idioma</param>
        /// <returns>Primer valor de la propiedad en el idioma idicado si está, si no en otro y si no sin idioma</returns>
        public string ObtenerPrimerValorDeIdiomaOSinEl(string pIdioma)
        {
            if (ListaValoresIdioma.Count > 0)
            {
                List<string> listaIdio = OntenerValoresIdiomaUsuarioUOtrosSiVacio(pIdioma);

                if (listaIdio.Count > 0)
                {
                    return listaIdio[0];
                }
            }
            else
            {
                foreach (string valor in ValoresUnificados.Keys)
                {
                    return valor;
                }
            }

            return null;
        }

        #endregion

        #endregion

        #region Miembros de IEquatable<Propiedad>

        /// <summary>
        /// Devuelve verdad si las propiedades son iguales
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Propiedad other)
        {
            return (other.Nombre.Equals(this.Nombre));
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
        ~Propiedad()
        {
            //Libero los recursos
            Dispose(false,false);
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
                    if (this.mListaValores != null)
                        mListaValores.Clear();

                    if (mListaValoresIdioma != null)
                    {
                        mListaValoresIdioma.Clear();
                    }

                    if (mListaValoresUsados != null)
                    {
                        mListaValoresUsados.Clear();
                    }

                    if (mEspecifPropiedad != null)
                    {
                        mEspecifPropiedad.Dispose();
                    }

                    if (pEliminarListasComunes)
                    {
                        if (mDominios != null)
                            mDominios.Clear();

                        if (this.mListaPropiedadesInversas != null)
                            mListaPropiedadesInversas.Clear();

                        mDominios.Clear();

                        if (mListaValoresPermitidos != null)
                        {
                            mListaValoresPermitidos.Clear();
                        }

                        if (mSuperPropiedades != null)
                        {
                            mSuperPropiedades.Clear();
                        }

                        if (mLabelIdioma != null)
                        {
                            mLabelIdioma.Clear();
                        }
                    }
                }

                mListaPropiedadesInversas = null;
                mListaValores = null;
                mElementoOntologia = null;
                mPropiedadEquivalente = null;
                mPropiedadInversa = null;
                mListaValoresIdioma = null;
                mListaValoresPermitidos = null;
                mSuperPropiedades = null;
                mListaValoresUsados = null;
                mOntologia = null;
                mLabelIdioma = null;
                mElementoOntologia = null;
                mEspecifPropiedad = null;
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
            info.AddValue("CardinalidadMenorOIgualUno", mCardinalidadMenorOIgualUno);
            info.AddValue("Dominios", mDominios);
            info.AddValue("ElementoImpreso", mElementoImpreso);
            info.AddValue("ElementoOntologia", mElementoOntologia);
            info.AddValue("FunctionalProperty", mFunctionalProperty);
            info.AddValue("Heredada", mHeredada);
            info.AddValue("ListaPropiedadesInversas", mListaPropiedadesInversas);
            info.AddValue("ListaValores", mListaValores);
            info.AddValue("ListaValoresPermitidos", mListaValoresPermitidos);
            info.AddValue("Nombre", mNombre);
            info.AddValue("NombreReal", mNombreReal);
            info.AddValue("Ontologia", mOntologia);
            info.AddValue("PropiedadEquivalente", mPropiedadEquivalente);
            info.AddValue("PropiedadInversa", mPropiedadInversa);
            info.AddValue("Rango", mRango);
            info.AddValue("RangoRelativo", mRangoRelativo);
            info.AddValue("Seleccionada", mSeleccionada);
            info.AddValue("SuperPropiedades", mSuperPropiedades);
            info.AddValue("Tipo", mTipo);
            info.AddValue("TipoEntidadRepresenta", mTipoEntidadRepresenta);
            info.AddValue("UnicoValor", mUnicoValor);
            info.AddValue("ListaValoresIdioma", mListaValoresIdioma);
            info.AddValue("ValorDefectoNoSeleccionable", mValorDefectoNoSeleccionable);
            info.AddValue("Visible", mVisible);
            info.AddValue("Label", mLabel);
            info.AddValue("LabelIdioma", mLabelIdioma);
            
            info.AddValue("EspecifPropiedad", mEspecifPropiedad);
        }

        public static explicit operator Propiedad(string v)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
