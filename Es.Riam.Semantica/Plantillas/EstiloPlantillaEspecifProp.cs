using System;
using System.Collections.Generic;
using Es.Riam.Semantica.OWL;
using System.Runtime.Serialization;

namespace Es.Riam.Semantica.Plantillas
{
    #region Enumeraciones

    /// <summary>
    /// Tipo de campo de una ontología: entero, texto, etc.
    /// </summary>
    public enum TipoCampoOntologia
    {
        Texto = 0,
        Entero = 1,
        Numerico = 2,
        Boleano = 3,
        DateTime = 4,
        Date = 5,
        ListaTexto = 6,
        ListaEnteros = 7,
        ListaNumeros = 8,
        ListaBoleanos = 9,
        ListaDateTimes = 10,
        ListaDates = 11,
        Imagen = 12,
        Video = 13,
        Tiny = 14,
        Time = 15,
        ListaTimes = 16,
        Archivo = 17,
        Checks = 18,
        TextArea = 19,
        Link = 20,
        EmbebedLink = 21,
        ImagenExterna = 22,
        EmbebedObject = 23,
        ArchivoLink = 24
    }

    #endregion

    /// <summary>
    /// Clase para gestionar los estilos de un elemento de la plantilla.
    /// </summary>
    [Serializable]
    public class EstiloPlantillaEspecifProp : EstiloPlantilla, ISerializable
    {
        #region Miembros

        /// <summary>
        /// Nombre de la entidad.
        /// </summary>
        private string mNombreEntidad;

        /// <summary>
        /// Nombre de la propiedad.
        /// </summary>
        private string mNombreRealPropiedad;

        /// <summary>
        /// Tipo del campo de la propiedad.
        /// </summary>
        private TipoCampoOntologia? mTipoCampo;

        /// <summary>
        /// Propiedad ontológica.
        /// </summary>
        private Propiedad mPropiedad;

        /// <summary>
        /// Comprueba si deben mostrarse checks y radioButtoms para una propiedad.
        /// </summary>
        private bool mEsPropiedadConValoresCheck;

        /// <summary>
        /// Lista con los valores permitidos para la propiedad.
        /// </summary>
        private List<string> mListaValoresPermitidos;

        /// <summary>
        /// Contiene el valor por defecto que debe tener la propiedad, pero que no es correcto para la misma (Ej: valor gris combo).
        /// </summary>
        private string mValorDefectoNoSeleccionable;

        /// <summary>
        /// Restricción del número de caracteres.
        /// </summary>
        private RestriccionNumCaracteres mRestrNumCaract;

        /// <summary>
        /// Texto que se debe mostrar a la hora de eliminar un elemento seleccionado.
        /// </summary>
        private string mTextoEliminarElemSel;

        /// <summary>
        /// Imagen mini de la propiedad.
        /// </summary>
        private ImagenMini mImagenMini;

        /// <summary>
        /// Establece si la imagen se recorta con jCrop.
        /// </summary>
        private bool mUsarJcrop = false;

        /// <summary>
        /// Anchura y altura mínimas para el recorte del Jcrop.
        /// </summary>
        private KeyValuePair<int, int> mMinSizeJcrop;

        /// <summary>
        /// Anchura y altura máximas para el recorte del Jcrop.
        /// </summary>
        private KeyValuePair<int, int> mMaxSizeJcrop;

        /// <summary>
        /// Parametros de galería si la propiedad debe ser una galería de imágenes, NULL en caso contrario.
        /// </summary>
        private string mGaleriaImagenes;

        /// <summary>
        /// Propiedad ancho y propiedad alto para procesar OpenSeaDragon en el caso de que lo tenga.
        /// </summary>
        private KeyValuePair<string, string> mOpenSeaDragon;

        /// <summary>
        /// Clase CSS para el título de la propiedad.
        /// </summary>
        private string mClaseCssPanelTitulo;

        /// <summary>
        /// Nombre del tag para el título en edición.
        /// </summary>
        private string mTagNameTituloEdicion;

        /// <summary>
        /// Nombre del tag para el título en lectura.
        /// </summary>
        private string mTagNameTituloLectura;

        /// <summary>
        /// Nombre de la propiedad en edición.
        /// </summary>
        private Dictionary<string, string> mAtrNombre;

        /// <summary>
        /// Nombre de la propiedad en lectura.
        /// </summary>
        private Dictionary<string, string> mAtrNombreLectura;

        /// <summary>
        /// Nombre de la propiedad en edición.
        /// </summary>
        private string mNombre;

        /// <summary>
        /// Nombre de la propiedad en lectura.
        /// </summary>
        private string mNombreLectura;

        /// <summary>
        /// Indica que los valores de la propiedad se introducirán separados por comas.
        /// </summary>
        private bool mValoresSepComas;

        /// <summary>
        /// Indica si el formato de la fecha es mes-año.
        /// </summary>
        private bool mFechaMesAnio;

        /// <summary>
        /// Indica si el formato de la fecha es libre, para introducir lo que quieras.
        /// </summary>
        private bool mFechaLibre;

        /// <summary>
        /// Indica si el formato de la fecha debe incluir la hora.
        /// </summary>
        private bool mFechaConHora;

        /// <summary>
        /// Indica si la fecha debe guardarse en formato entero (26/11/2011 14:18:20 -> 20111126141820).
        /// </summary>
        private bool mGuardarFechaComoEntero;

        /// <summary>
        /// Valor del grafo autocompletar del campo.
        /// </summary>
        private string mGrafoAutocompletar;

        /// <summary>
        /// Valor del tipo de resultado autocompletar del campo.
        /// </summary>
        private string mTipoResulAutocompletar;

        /// <summary>
        /// Indica si se deben guardasr los nuevos valores para proximas llamadas a autocompletar.
        /// </summary>
        private bool mGuardarValoresAutocompletar;

        /// <summary>
        /// Indica si no se deben permitir los nuevos valores que no devuelva el autocompletar.
        /// </summary>
        private bool mNoPermitirNuevosValores;

        /// <summary>
        /// Tipo de campo para lectura.
        /// </summary>
        private string mTipoCampoLectura;

        /// <summary>
        /// Indica si el campo debe aparecer deshabilitado.
        /// </summary>
        private bool mCampoDeshabilitado;

        /// <summary>
        /// Clase CSS para el campo de la propiedad.
        /// </summary>
        private string mClaseCss;

        /// <summary>
        /// Texto para el botón de agregar elemento.
        /// </summary>
        private string mTextoAgregarElem;

        /// <summary>
        /// Texto para el botón de guardar elemento.
        /// </summary>
        private string mTextoBotonAceptarElemento;

        /// <summary>
        /// Indica is hay que mostrar la vista previa de la propiedad en edición.
        /// </summary>
        private bool mVistaPrevEnEdicion;

        /// <summary>
        /// Clase panel contenedor css.
        /// </summary>
        private string mClaseCssPanel;

        /// <summary>
        /// Texto para el botón de cancelar elemento.
        /// </summary>
        private string mTextoCancelarElem;

        /// <summary>
        /// Texto para el botón de editar elemento.
        /// </summary>
        private string mTextoEdicionEntSel;

        /// <summary>
        /// Valor de microdatos.
        /// </summary>
        private string mMicrodatos;

        /// <summary>
        /// Valor de CapturarFlash.
        /// </summary>
        private KeyValuePair<string, string> mCapturarFlash;

        /// <summary>
        /// Valor del Html plantilla para el objeto incrustado.
        /// </summary>
        private string mHtmlObjeto;

        /// <summary>
        /// Valor de microformatos.
        /// </summary>
        private Dictionary<string, string> mMicroformatos;

        /// <summary>
        /// Selector de entidad.
        /// </summary>
        private SelectorEntidad mSelectorEntidad;

        /// <summary>
        /// Link que debe tener el valor de la propiedad.
        /// </summary>
        private string mUrlLinkDelValor;

        /// <summary>
        /// Indica si el link al recurso que la contiene debe abrirse en una nueva pestaña.
        /// </summary>
        private bool mNuevaPestanya = true;

        /// <summary>
        /// Nombre del grafo del que dependen el valor de la propiedad.
        /// </summary>
        private string mGrafoDependiente;

        /// <summary>
        /// Tipo de la entidad de la que dependen el valor de la propiedad.
        /// </summary>
        private string mTipoEntDependiente;

        /// <summary>
        /// ID y tipo de entidad de la propiedad cuyo valor condiciona el de la actual que dependen el valor de la propiedad.
        /// </summary>
        private KeyValuePair<string, string> mPropDependiente;

        /// <summary>
        /// Tipo del campo (Autocompletar, combo).
        /// </summary>
        private string mTipoDependiente;

        /// <summary>
        /// Lista con las propiedades auxiliares de la propiedad.
        /// </summary>
        private List<EstiloPlantillaEspecifProp> mPropiedadesAuxiliares;

        /// <summary>
        /// Elemento ordenado auxiliar de la propiedad.
        /// </summary>
        private ElementoOrdenado mElementoOrdenadoAuxiliar;

        /// <summary>
        /// Valor por defecto para la propiedad.
        /// </summary>
        private string mValorPorDefecto;

        /// <summary>
        /// Expresión regular para aplicar al valor de una propiedad.
        /// </summary>
        private string mExpresionRegular;

        /// <summary>
        /// Indica si solo se debe mostrar la primera coincidencia de la expresión regular.
        /// </summary>
        private bool mPrimeraCoincidenciaExpresionRegular;

        /// <summary>
        /// Indica si NO hay multiIdioma.
        /// </summary>
        private bool mNoMultiIdioma;

        /// <summary>
        /// Indica que es privada para los miembros de la comunidad.
        /// </summary>
        private bool mPrivadoPrivadoParaMiembrosComunidad;

        /// <summary>
        /// Propiedad que definie la privacidad de algún nodo
        /// </summary>
        private List<Guid> mPrivadoParaGrupoEditores;

        /// <summary>
        /// Número de elementos por página para la paginación de las entidades auxiliares de la propiedad.
        /// </summary>
        private int mNumElemPorPag;

        /// <summary>
        /// Ruta de la vista personalizada para la paginación en caso de que la haya.
        /// </summary>
        private string mVistaPersonalizadaPaginacion;

        /// <summary>
        /// Indica que es un selector de entidad dentro de otro selector de entidad. NO SE AGREGA AL CONSTRUCTOR.
        /// </summary>
        private bool mEsSelectorEntidadInterno;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor sin parámetros.
        /// </summary>
        public EstiloPlantillaEspecifProp()
        {
        }

        /// <summary>
        /// Constructor a partir de otro objeto.
        /// </summary>
        /// <param name="pEstiloRef">Objeto de referencia</param>
        public EstiloPlantillaEspecifProp(EstiloPlantillaEspecifProp pEstiloRef)
        {
            NombreEntidad = pEstiloRef.NombreEntidad;
            NombreRealPropiedad = pEstiloRef.NombreRealPropiedad;

            if (pEstiloRef.TieneValor_TipoCampo)
            {
                mTipoCampo = pEstiloRef.TipoCampo;
                if (mTipoCampo.HasValue)
                {
                    mTipoCampoINT = (int)mTipoCampo.Value;
                }
            }
            //Propiedad;
            EsPropiedadConValoresCheck = pEstiloRef.EsPropiedadConValoresCheck;
            ListaValoresPermitidos = pEstiloRef.ListaValoresPermitidos;
            ValorDefectoNoSeleccionable = pEstiloRef.ValorDefectoNoSeleccionable;
            RestrNumCaract = pEstiloRef.RestrNumCaract;
            TextoEliminarElemSel = pEstiloRef.TextoEliminarElemSel;
            ImagenMini = pEstiloRef.ImagenMini;
            UsarJcrop = pEstiloRef.UsarJcrop;
            MinSizeJcrop = pEstiloRef.MinSizeJcrop;
            MaxSizeJcrop = pEstiloRef.MaxSizeJcrop;
            GaleriaImagenes = pEstiloRef.GaleriaImagenes;
            OpenSeaDragon = pEstiloRef.OpenSeaDragon;
            ClaseCssPanelTitulo = pEstiloRef.ClaseCssPanelTitulo;
            TagNameTituloEdicion = pEstiloRef.TagNameTituloEdicion;
            TagNameTituloLectura = pEstiloRef.TagNameTituloLectura;
            AtrNombre = pEstiloRef.AtrNombre;
            AtrNombreLectura = pEstiloRef.AtrNombreLectura;
            //Nombre;
            //NombreLectura;
            mValoresSepComas = pEstiloRef.ValoresSepComasAlmacenado;
            FechaMesAnio = pEstiloRef.FechaMesAnio;
            FechaLibre = pEstiloRef.FechaLibre;
            FechaConHora = pEstiloRef.FechaConHora;
            GuardarFechaComoEntero = pEstiloRef.GuardarFechaComoEntero;
            GrafoAutocompletar = pEstiloRef.GrafoAutocompletar;
            TipoResulAutocompletar = pEstiloRef.TipoResulAutocompletar;
            GuardarValoresAutocompletar = pEstiloRef.GuardarValoresAutocompletar;
            NoPermitirNuevosValores = pEstiloRef.NoPermitirNuevosValores;
            TipoCampoLectura = pEstiloRef.TipoCampoLectura;
            CampoDeshabilitado = pEstiloRef.CampoDeshabilitado;
            ClaseCss = pEstiloRef.ClaseCss;
            TextoAgregarElem = pEstiloRef.TextoAgregarElem;
            TextoBotonAceptarElemento = pEstiloRef.TextoBotonAceptarElemento;
            VistaPrevEnEdicion = pEstiloRef.VistaPrevEnEdicion;
            ClaseCssPanel = pEstiloRef.ClaseCssPanel;
            TextoCancelarElem = pEstiloRef.TextoCancelarElem;
            TextoEdicionEntSel = pEstiloRef.TextoEdicionEntSel;
            Microdatos = pEstiloRef.Microdatos;
            Microformatos = pEstiloRef.Microformatos;
            SelectorEntidad = pEstiloRef.SelectorEntidad;
            UrlLinkDelValor = pEstiloRef.UrlLinkDelValor;
            CapturarFlash = pEstiloRef.CapturarFlash;
            HtmlObjeto = pEstiloRef.HtmlObjeto;
            NuevaPestanya = pEstiloRef.NuevaPestanya;
            GrafoDependiente = pEstiloRef.GrafoDependiente;
            TipoEntDependiente = pEstiloRef.TipoEntDependiente;
            PropDependiente = pEstiloRef.PropDependiente;
            TipoDependiente = pEstiloRef.TipoDependiente;
            PropiedadesAuxiliares = pEstiloRef.PropiedadesAuxiliares;
            ElementoOrdenadoAuxiliar = pEstiloRef.ElementoOrdenadoAuxiliar;
            ValorPorDefecto = pEstiloRef.ValorPorDefecto;
            ExpresionRegular = pEstiloRef.ExpresionRegular;
            PrimeraCoincidenciaExpresionRegular = pEstiloRef.PrimeraCoincidenciaExpresionRegular;
            NoMultiIdioma = pEstiloRef.NoMultiIdioma;
            PrivadoPrivadoParaMiembrosComunidad = pEstiloRef.PrivadoPrivadoParaMiembrosComunidad;
            PrivadoParaGrupoEditores = pEstiloRef.PrivadoParaGrupoEditores;
            NumElemPorPag = pEstiloRef.NumElemPorPag;
            VistaPersonalizadaPaginacion = pEstiloRef.VistaPersonalizadaPaginacion;
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected EstiloPlantillaEspecifProp(SerializationInfo info, StreamingContext context)
        {
            mAtrNombre = (Dictionary<string, string>)info.GetValue("AtrNombre", typeof(Dictionary<string, string>));
            mAtrNombreLectura = (Dictionary<string, string>)info.GetValue("AtrNombreLectura", typeof(Dictionary<string, string>));
            mCampoDeshabilitado = (bool)info.GetValue("CampoDeshabilitado", typeof(bool));
            mCapturarFlash = (KeyValuePair<string, string>)info.GetValue("CapturarFlash", typeof(KeyValuePair<string, string>));
            mClaseCss = (string)info.GetValue("ClaseCss", typeof(string));
            mClaseCssPanel = (string)info.GetValue("ClaseCssPanel", typeof(string));
            mClaseCssPanelTitulo = (string)info.GetValue("ClaseCssPanelTitulo", typeof(string));
            mElementoOrdenadoAuxiliar = (ElementoOrdenado)info.GetValue("ElementoOrdenadoAuxiliar", typeof(ElementoOrdenado));
            mEsPropiedadConValoresCheck = (bool)info.GetValue("EsPropiedadConValoresCheck", typeof(bool));
            mEsSelectorEntidadInterno = (bool)info.GetValue("EsSelectorEntidadInterno", typeof(bool));
            mExpresionRegular = (string)info.GetValue("ExpresionRegular", typeof(string));
            mFechaConHora = (bool)info.GetValue("FechaConHora", typeof(bool));
            mFechaLibre = (bool)info.GetValue("FechaLibre", typeof(bool));
            mFechaMesAnio = (bool)info.GetValue("FechaMesAnio", typeof(bool));
            mGaleriaImagenes = (string)info.GetValue("GaleriaImagenes", typeof(string));
            mGrafoAutocompletar = (string)info.GetValue("GrafoAutocompletar", typeof(string));
            mGrafoDependiente = (string)info.GetValue("GrafoDependiente", typeof(string));
            mGuardarFechaComoEntero = (bool)info.GetValue("GuardarFechaComoEntero", typeof(bool));
            mGuardarValoresAutocompletar = (bool)info.GetValue("GuardarValoresAutocompletar", typeof(bool));
            mHtmlObjeto = (string)info.GetValue("HtmlObjeto", typeof(string));
            mImagenMini = (ImagenMini)info.GetValue("ImagenMini", typeof(ImagenMini));
            mListaValoresPermitidos = (List<string>)info.GetValue("ListaValoresPermitidos", typeof(List<string>));
            mMaxSizeJcrop = (KeyValuePair<int, int>)info.GetValue("MaxSizeJcrop", typeof(KeyValuePair<int, int>));
            mMicrodatos = (string)info.GetValue("Microdatos", typeof(string));
            mMicroformatos = (Dictionary<string, string>)info.GetValue("Microformatos", typeof(Dictionary<string, string>));
            mMinSizeJcrop = (KeyValuePair<int, int>)info.GetValue("MinSizeJcrop", typeof(KeyValuePair<int, int>));
            mClaseCssPanelTitulo = (string)info.GetValue("ClaseCssPanelTitulo", typeof(string));
            mNombreEntidad = (string)info.GetValue("NombreEntidad", typeof(string));
            mNombreRealPropiedad = (string)info.GetValue("NombreRealPropiedad", typeof(string));
            mNoMultiIdioma = (bool)info.GetValue("NoMultiIdioma", typeof(bool));
            mNoPermitirNuevosValores = (bool)info.GetValue("NoPermitirNuevosValores", typeof(bool));
            mNuevaPestanya = (bool)info.GetValue("NuevaPestanya", typeof(bool));
            mNumElemPorPag = (int)info.GetValue("NumElemPorPag", typeof(int));
            mOpenSeaDragon = (KeyValuePair<string, string>)info.GetValue("OpenSeaDragon", typeof(KeyValuePair<string, string>));
            mPrimeraCoincidenciaExpresionRegular = (bool)info.GetValue("PrimeraCoincidenciaExpresionRegular", typeof(bool));
            mPrivadoParaGrupoEditores = (List<Guid>)info.GetValue("PrivadoParaGrupoEditores", typeof(List<Guid>));
            mPrivadoPrivadoParaMiembrosComunidad = (bool)info.GetValue("PrivadoPrivadoParaMiembrosComunidad", typeof(bool));
            mPropDependiente = (KeyValuePair<string, string>)info.GetValue("PropDependiente", typeof(KeyValuePair<string, string>));
            mPropiedadesAuxiliares = (List<EstiloPlantillaEspecifProp>)info.GetValue("PropiedadesAuxiliares", typeof(List<EstiloPlantillaEspecifProp>));
            mRestrNumCaract = (RestriccionNumCaracteres)info.GetValue("RestrNumCaract", typeof(RestriccionNumCaracteres));
            mSelectorEntidad = (SelectorEntidad)info.GetValue("SelectorEntidad", typeof(SelectorEntidad));
            mTagNameTituloEdicion = (string)info.GetValue("TagNameTituloEdicion", typeof(string));
            mTagNameTituloLectura = (string)info.GetValue("TagNameTituloLectura", typeof(string));
            mTextoAgregarElem = (string)info.GetValue("TextoAgregarElem", typeof(string));
            mTextoBotonAceptarElemento = (string)info.GetValue("TextoBotonAceptarElemento", typeof(string));
            mTextoCancelarElem = (string)info.GetValue("TextoCancelarElem", typeof(string));
            mTextoEdicionEntSel = (string)info.GetValue("TextoEdicionEntSel", typeof(string));
            mTextoEliminarElemSel = (string)info.GetValue("TextoEliminarElemSel", typeof(string));
            TipoCampoOntologia mTipoCampo = TipoCampoOntologia.Texto;
            int? TipoCampo = null;
            try
            {
                TipoCampo = (int?)info.GetValue("TipoCampo", typeof(int));
            }
            catch { }

            if (TipoCampo != null)
            {
                mTipoCampo = (TipoCampoOntologia)TipoCampo.Value;
                mTipoCampoINT = (int)info.GetValue("TipoCampoINT", typeof(int));
            }

            mTipoCampoLectura = (string)info.GetValue("TipoCampoLectura", typeof(string));
            mTipoDependiente = (string)info.GetValue("TipoDependiente", typeof(string));
            mTipoEntDependiente = (string)info.GetValue("TipoEntDependiente", typeof(string));
            mTipoResulAutocompletar = (string)info.GetValue("TipoResulAutocompletar", typeof(string));
            mUrlLinkDelValor = (string)info.GetValue("UrlLinkDelValor", typeof(string));
            mUsarJcrop = (bool)info.GetValue("UsarJcrop", typeof(bool));
            mValorDefectoNoSeleccionable = (string)info.GetValue("ValorDefectoNoSeleccionable", typeof(string));
            mValoresSepComas = (bool)info.GetValue("ValoresSepComas", typeof(bool));
            mValorPorDefecto = (string)info.GetValue("ValorPorDefecto", typeof(string));
            mVistaPersonalizadaPaginacion = (string)info.GetValue("VistaPersonalizadaPaginacion", typeof(string));
            mVistaPrevEnEdicion = (bool)info.GetValue("VistaPrevEnEdicion", typeof(bool));
        }


        #endregion

        #region Propiedades

        /// <summary>
        /// Nombre de la entidad.
        /// </summary>
        public string NombreEntidad
        {
            get
            {
                return mNombreEntidad;
            }
            set
            {
                mNombreEntidad = value;
            }
        }

        /// <summary>
        /// Nombre de la propiedad.
        /// </summary>
        public string NombreRealPropiedad
        {
            get
            {
                return mNombreRealPropiedad;
            }
            set
            {
                mNombreRealPropiedad = value;
            }
        }

        /// <summary>
        /// Tipo del campo de la propiedad que puede ser null. 
        /// Se usa para poder deserializar TipoCampo.
        /// </summary>
        public TipoCampoOntologia? TipoCampoSetter
        {
            get
            {
                if (!mTipoCampo.HasValue && mPropiedad != null)
                {
                    mTipoCampo = ObtenerTipoCampo();
                }

                if (!mTipoCampo.HasValue && mPropiedad == null)
                {
                    mTipoCampo = (TipoCampoOntologia?)mTipoCampoINT;
                }

                if (mTipoCampo.HasValue)
                {
                    mTipoCampoINT = (int)mTipoCampo.Value;
                }

                return mTipoCampo.Value;
            }
            set
            {
                mTipoCampo = value;
                if (value.HasValue)
                {
                    mTipoCampoINT = (int)value.Value;
                }
            }
        }

        /// <summary>
        /// Valor del tipo del campo de la propiedad. 
        /// Se usa para poder obtener el valor de TipoCampo al deserializar.
        /// </summary>
        public int mTipoCampoINT;
        public int TipoCampoINT
        {
            get
            {
                return mTipoCampoINT;
            }
            set
            {
                mTipoCampoINT = value;
            }
        }

        /// <summary>
        /// Tipo del campo de la propiedad.
        /// </summary>
        public TipoCampoOntologia TipoCampo
        {
            get
            {
                if (!mTipoCampo.HasValue && mPropiedad != null)
                {
                    mTipoCampo = ObtenerTipoCampo();
                }

                // añadido para serializar
                if (!mTipoCampo.HasValue && mPropiedad == null)
                {
                    mTipoCampo = (TipoCampoOntologia?)mTipoCampoINT;
                }

                if (mTipoCampo.HasValue)
                {
                    mTipoCampoINT = (int)mTipoCampo.Value;
                }
                //
                return mTipoCampo.Value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool TieneValor_TipoCampo
        {
            get
            {
                return mTipoCampo.HasValue;
            }
        }

        /// <summary>
        /// Propiedad ontológica.
        /// </summary>
        public Propiedad Propiedad
        {
            get
            {
                return mPropiedad;
            }
            set
            {
                mPropiedad = value;
            }
        }

        /// <summary>
        /// Comprueba si deben mostrarse checks y radioButtoms para una propiedad.
        /// </summary>
        public bool EsPropiedadConValoresCheck
        {
            get
            {
                return mEsPropiedadConValoresCheck;
            }
            set
            {
                mEsPropiedadConValoresCheck = value;
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
                    mListaValoresPermitidos = new List<string>();
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
                return mValorDefectoNoSeleccionable;
            }
            set
            {
                mValorDefectoNoSeleccionable = value;
            }
        }

        /// <summary>
        /// Restricción del número de caracteres.
        /// </summary>
        public RestriccionNumCaracteres RestrNumCaract
        {
            get
            {
                return mRestrNumCaract;
            }
            set
            {
                mRestrNumCaract = value;
            }
        }

        /// <summary>
        /// Texto que se debe mostrar a la hora de eliminar un elemento seleccionado.
        /// </summary>
        public string TextoEliminarElemSel
        {
            get
            {
                return mTextoEliminarElemSel;
            }
            set
            {
                mTextoEliminarElemSel = value;
            }
        }

        /// <summary>
        /// Establece si la imagen se recorta con jCrop.
        /// </summary>
        public bool UsarJcrop
        {
            get
            {
                return mUsarJcrop;
            }
            set
            {
                mUsarJcrop = value;
            }
        }

        /// <summary>
        /// Anchura y altura mínimas para el recorte del Jcrop. Por defecto es '75,75'.
        /// </summary>
        public KeyValuePair<int, int> MinSizeJcrop
        {
            get
            {
                return mMinSizeJcrop;
            }
            set
            {
                mMinSizeJcrop = value;
            }
        }

        /// <summary>
        /// Anchura y altura máximas para el recorte del Jcrop. No hay máximo por defeto.
        /// </summary>
        public KeyValuePair<int, int> MaxSizeJcrop
        {
            get
            {
                return mMaxSizeJcrop;
            }
            set
            {
                mMaxSizeJcrop = value;
            }
        }

        /// <summary>
        /// Imagen mini de la propiedad.
        /// </summary>
        public ImagenMini ImagenMini
        {
            get
            {
                return mImagenMini;
            }
            set
            {
                mImagenMini = value;
            }
        }

        /// <summary>
        /// Parametros de galería si la propiedad debe ser una galería de imágenes, NULL en caso contrario.
        /// </summary>
        public string GaleriaImagenes
        {
            get
            {
                return mGaleriaImagenes;
            }
            set
            {
                mGaleriaImagenes = value;
            }
        }

        /// <summary>
        /// Propiedad ancho y propiedad alto para procesar OpenSeaDragon en el caso de que lo tenga.
        /// </summary>
        public KeyValuePair<string, string> OpenSeaDragon
        {
            get
            {
                return mOpenSeaDragon;
            }
            set
            {
                mOpenSeaDragon = value;
            }
        }

        /// <summary>
        /// Clase CSS para el título de la propiedad.
        /// </summary>
        public string ClaseCssPanelTitulo
        {
            get
            {
                return mClaseCssPanelTitulo;
            }
            set
            {
                mClaseCssPanelTitulo = value;
            }
        }

        /// <summary>
        /// Nombre del tag para el título en edición.
        /// </summary>
        public string TagNameTituloEdicion
        {
            get
            {
                return mTagNameTituloEdicion;
            }
            set
            {
                mTagNameTituloEdicion = value;
            }
        }

        /// <summary>
        /// Nombre del tag para el título en lectura.
        /// </summary>
        public string TagNameTituloLectura
        {
            get
            {
                return mTagNameTituloLectura;
            }
            set
            {
                mTagNameTituloLectura = value;
            }
        }

        /// <summary>
        /// Nombre de la propiedad en edición.
        /// </summary>
        public Dictionary<string, string> AtrNombre
        {
            get
            {
                if (mAtrNombre == null)
                {
                    mAtrNombre = new Dictionary<string, string>();
                }

                return mAtrNombre;
            }
            set
            {
                mAtrNombre = value;
            }
        }

        /// <summary>
        /// Nombre de la propiedad en lectura.
        /// </summary>
        public Dictionary<string, string> AtrNombreLectura
        {
            get
            {
                if (mAtrNombreLectura == null)
                {
                    mAtrNombreLectura = new Dictionary<string, string>();
                }

                return mAtrNombreLectura;
            }
            set
            {
                mAtrNombreLectura = value;
            }
        }

        /// <summary>
        /// Nombre de la propiedad en edición.
        /// </summary>
        public string Nombre
        {
            get
            {
                if (mNombre == null)
                {
                    if (AtrNombre.ContainsKey(Propiedad.Ontologia.IdiomaUsuario))
                    {
                        mNombre = AtrNombre[Propiedad.Ontologia.IdiomaUsuario];
                    }
                    else
                    {
                        foreach (string valor in AtrNombre.Values)
                        {
                            mNombre = valor;
                            break;
                        }
                    }
                }

                return mNombre;
            }
            set
            {
                mNombre = value;
            }
        }

        /// <summary>
        /// Nombre de la propiedad en edición.
        /// </summary>
        public string NombreLectura
        {
            get
            {
                if (mNombreLectura == null)
                {
                    if (AtrNombreLectura.ContainsKey(Propiedad.Ontologia.IdiomaUsuario))
                    {
                        mNombreLectura = AtrNombreLectura[Propiedad.Ontologia.IdiomaUsuario];
                    }
                    else
                    {
                        foreach (string valor in AtrNombreLectura.Values)
                        {
                            mNombreLectura = valor;
                            break;
                        }
                    }
                }

                return mNombreLectura;
            }
            set
            {
                mNombreLectura = value;
            }
        }

        /// <summary>
        /// Indica que los valores de la propiedad se introducirán separados por comas.
        /// </summary>
        public bool ValoresSepComas
        {
            get
            {
                if (!mPropiedad.FunctionalProperty && mPropiedad.Tipo == TipoPropiedad.DatatypeProperty)
                {
                    if (mValoresSepComas)
                    {
                        return true;
                    }

                    if (!Propiedad.CardinalidadMenorOIgualUno)
                    {
                        return EsPropiedadConValoresCheck;
                    }
                }

                return mValoresSepComas;
            }
            set
            {
                mValoresSepComas = value;
            }
        }

        /// <summary>
        /// Indica que los valores de la propiedad se introducirán separados por comas.
        /// </summary>
        public bool ValoresSepComasAlmacenado
        {
            get
            {
                return mValoresSepComas;
            }
        }

        /// <summary>
        /// Indica si el formato de la fecha es mes-año.
        /// </summary>
        public bool FechaMesAnio
        {
            get
            {
                return mFechaMesAnio;
            }
            set
            {
                mFechaMesAnio = value;
            }
        }

        /// <summary>
        /// Indica si el formato de la fecha es libre, para introducir lo que quieras.
        /// </summary>
        public bool FechaLibre
        {
            get
            {
                return mFechaLibre;
            }
            set
            {
                mFechaLibre = value;
            }
        }

        /// <summary>
        /// Indica si el formato de la fecha debe incluir la hora.
        /// </summary>
        public bool FechaConHora
        {
            get
            {
                return mFechaConHora;
            }
            set
            {
                mFechaConHora = value;
            }
        }

        /// <summary>
        /// Indica si la fecha debe guardarse en formato entero (26/11/2011 14:18:20 -> 20111126141820).
        /// </summary>
        public bool GuardarFechaComoEntero
        {
            get
            {
                return mGuardarFechaComoEntero;
            }
            set
            {
                mGuardarFechaComoEntero = value;
            }
        }

        /// <summary>
        /// Valor del grafo autocompletar del campo.
        /// </summary>
        public string GrafoAutocompletar
        {
            get
            {
                return mGrafoAutocompletar;
            }
            set
            {
                mGrafoAutocompletar = value;
            }
        }

        /// <summary>
        /// Valor del tipo de resultado autocompletar del campo.
        /// </summary>
        public string TipoResulAutocompletar
        {
            get
            {
                return mTipoResulAutocompletar;
            }
            set
            {
                mTipoResulAutocompletar = value;
            }
        }

        /// <summary>
        /// Indica si se deben guardasr los nuevos valores para proximas llamadas a autocompletar.
        /// </summary>
        public bool GuardarValoresAutocompletar
        {
            get
            {
                return mGuardarValoresAutocompletar;
            }
            set
            {
                mGuardarValoresAutocompletar = value;
            }
        }

        /// <summary>
        /// Indica si no se deben permitir los nuevos valores que no devuelva el autocompletar.
        /// </summary>
        public bool NoPermitirNuevosValores
        {
            get
            {
                return mNoPermitirNuevosValores;
            }
            set
            {
                mNoPermitirNuevosValores = value;
            }
        }

        /// <summary>
        /// Tipo de campo para lectura.
        /// </summary>
        public string TipoCampoLectura
        {
            get
            {
                return mTipoCampoLectura;
            }
            set
            {
                mTipoCampoLectura = value;
            }
        }

        /// <summary>
        /// Indica si el campo debe aparecer deshabilitado.
        /// </summary>
        public bool CampoDeshabilitado
        {
            get
            {
                return mCampoDeshabilitado;
            }
            set
            {
                mCampoDeshabilitado = value;
            }
        }

        /// <summary>
        /// Clase CSS para el campo de la propiedad.
        /// </summary>
        public string ClaseCss
        {
            get
            {
                return mClaseCss;
            }
            set
            {
                mClaseCss = value;
            }
        }

        /// <summary>
        /// Texto para el botón de agregar elemento.
        /// </summary>
        public string TextoAgregarElem
        {
            get
            {
                return mTextoAgregarElem;
            }
            set
            {
                mTextoAgregarElem = value;
            }
        }

        /// <summary>
        /// Texto para el botón de guardar elemento.
        /// </summary>
        public string TextoBotonAceptarElemento
        {
            get
            {
                return mTextoBotonAceptarElemento;
            }
            set
            {
                mTextoBotonAceptarElemento = value;
            }
        }

        /// <summary>
        /// Indica is hay que mostrar la vista previa de la propiedad en edición.
        /// </summary>
        public bool VistaPrevEnEdicion
        {
            get
            {
                return mVistaPrevEnEdicion;
            }
            set
            {
                mVistaPrevEnEdicion = value;
            }
        }

        /// <summary>
        /// Clase panel contenedor css.
        /// </summary>
        public string ClaseCssPanel
        {
            get
            {
                return mClaseCssPanel;
            }
            set
            {
                mClaseCssPanel = value;
            }
        }

        /// <summary>
        /// Texto para el botón de cancelar elemento.
        /// </summary>
        public string TextoCancelarElem
        {
            get
            {
                return mTextoCancelarElem;
            }
            set
            {
                mTextoCancelarElem = value;
            }
        }

        /// <summary>
        /// Texto para el botón de editar elemento.
        /// </summary>
        public string TextoEdicionEntSel
        {
            get
            {
                return mTextoEdicionEntSel;
            }
            set
            {
                mTextoEdicionEntSel = value;
            }
        }

        /// <summary>
        /// Valor de microdatos.
        /// </summary>
        public string Microdatos
        {
            get
            {
                return mMicrodatos;
            }
            set
            {
                mMicrodatos = value;
            }
        }

        /// <summary>
        /// Valor de CapturarFlash.
        /// </summary>
        public KeyValuePair<string, string> CapturarFlash
        {
            get
            {
                return mCapturarFlash;
            }
            set
            {
                mCapturarFlash = value;
            }
        }

        /// <summary>
        /// Valor del Html plantilla para el objeto incrustado.
        /// </summary>
        public string HtmlObjeto
        {
            get
            {
                return mHtmlObjeto;
            }
            set
            {
                mHtmlObjeto = value;
            }
        }

        /// <summary>
        /// Valor de microformatos.
        /// </summary>
        public Dictionary<string, string> Microformatos
        {
            get
            {
                if (mMicroformatos == null)
                {
                    mMicroformatos = new Dictionary<string, string>();
                }

                return mMicroformatos;
            }
            set
            {
                mMicroformatos = value;
            }
        }

        /// <summary>
        /// Selector de entidad.
        /// </summary>
        public SelectorEntidad SelectorEntidad
        {
            get
            {
                return mSelectorEntidad;
            }
            set
            {
                mSelectorEntidad = value;
            }
        }

        /// <summary>
        /// Link que debe tener el valor de la propiedad.
        /// </summary>
        public string UrlLinkDelValor
        {
            get
            {
                return mUrlLinkDelValor;
            }
            set
            {
                mUrlLinkDelValor = value;
            }
        }

        /// <summary>
        /// Indica si el link al recurso que la contiene debe abrirse en una nueva pestaña.
        /// </summary>
        public bool NuevaPestanya
        {
            get
            {
                return mNuevaPestanya;
            }
            set
            {
                mNuevaPestanya = value;
            }
        }

        /// <summary>
        /// Nombre del grafo del que dependen el valor de la propiedad.
        /// </summary>
        public string GrafoDependiente
        {
            get
            {
                return mGrafoDependiente;
            }
            set
            {
                mGrafoDependiente = value;
            }
        }

        /// <summary>
        /// Tipo de la entidad de la que dependen el valor de la propiedad.
        /// </summary>
        public string TipoEntDependiente
        {
            get
            {
                return mTipoEntDependiente;
            }
            set
            {
                mTipoEntDependiente = value;
            }
        }

        /// <summary>
        /// ID y tipo de entidad de la propiedad cuyo valor condiciona el de la actual que dependen el valor de la propiedad.
        /// </summary>
        public KeyValuePair<string, string> PropDependiente
        {
            get
            {
                return mPropDependiente;
            }
            set
            {
                mPropDependiente = value;
            }
        }

        /// <summary>
        /// Tipo del campo (Autocompletar, combo).
        /// </summary>
        public string TipoDependiente
        {
            get
            {
                return mTipoDependiente;
            }
            set
            {
                mTipoDependiente = value;
            }
        }

        /// <summary>
        /// Lista con las propiedades auxiliares de la propiedad.
        /// </summary>
        public List<EstiloPlantillaEspecifProp> PropiedadesAuxiliares
        {
            get
            {
                return mPropiedadesAuxiliares;
            }
            set
            {
                mPropiedadesAuxiliares = value;
            }
        }

        /// <summary>
        /// Elemento ordenado auxiliar de la propiedad.
        /// </summary>
        public ElementoOrdenado ElementoOrdenadoAuxiliar
        {
            get
            {
                return mElementoOrdenadoAuxiliar;
            }
            set
            {
                mElementoOrdenadoAuxiliar = value;
            }
        }

        /// <summary>
        /// Valor por defecto para la propiedad.
        /// </summary>
        public string ValorPorDefecto
        {
            get
            {
                return mValorPorDefecto;
            }
            set
            {
                mValorPorDefecto = value;
            }
        }

        /// <summary>
        /// Expresión regular para aplicar al valor de una propiedad.
        /// </summary>
        public string ExpresionRegular
        {
            get
            {
                return mExpresionRegular;
            }
            set
            {
                mExpresionRegular = value;
            }
        }

        /// <summary>
        /// Indica si solo se debe mostrar la primera coincidencia de la expresión regular.
        /// </summary>
        public bool PrimeraCoincidenciaExpresionRegular
        {
            get
            {
                return mPrimeraCoincidenciaExpresionRegular;
            }
            set
            {
                mPrimeraCoincidenciaExpresionRegular = value;
            }
        }

        /// <summary>
        /// Indica si NO hay multiIdioma.
        /// </summary>
        public bool NoMultiIdioma
        {
            get
            {
                return mNoMultiIdioma;
            }
            set
            {
                mNoMultiIdioma = value;
            }
        }

        /// <summary>
        /// Indica que es privada para los miembros de la comunidad.
        /// </summary>
        public bool PrivadoPrivadoParaMiembrosComunidad
        {
            get
            {
                return mPrivadoPrivadoParaMiembrosComunidad;
            }
            set
            {
                mPrivadoPrivadoParaMiembrosComunidad = value;
            }
        }

        public List<Guid> PrivadoParaGrupoEditores
        {
            get
            {
                return mPrivadoParaGrupoEditores;
            }
            set
            {
                mPrivadoParaGrupoEditores = value;
            }
        }

        /// <summary>
        /// Número de elementos por página para la paginación de las entidades auxiliares de la propiedad.
        /// </summary>
        public int NumElemPorPag
        {
            get
            {
                return mNumElemPorPag;
            }
            set
            {
                mNumElemPorPag = value;
            }
        }

        /// <summary>
        /// Ruta de la vista personalizada para la paginación en caso de que la haya.
        /// </summary>
        public string VistaPersonalizadaPaginacion
        {
            get
            {
                return mVistaPersonalizadaPaginacion;
            }
            set
            {
                mVistaPersonalizadaPaginacion = value;
            }
        }

        /// <summary>
        /// Indica que es un selector de entidad dentro de otro selector de entidad. NO SE AGREGA AL CONSTRUCTOR.
        /// </summary>
        public bool EsSelectorEntidadInterno
        {
            get
            {
                return mEsSelectorEntidadInterno;
            }
            set
            {
                mEsSelectorEntidadInterno = value;
            }
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Devuelve el nombre de la entidad.
        /// </summary>
        /// <param name="pVistaPrevia">Indica si estamos en vista previa o no</param>
        /// <returns>nombre de la entidad</returns>
        public string NombrePropiedad(bool pVistaPrevia)
        {
            if (pVistaPrevia)
            {
                return NombreLectura;
            }
            else
            {
                return Nombre;
            }
        }

        /// <summary>
        /// Devuelve el nombre de la entidad en un idioma concreto.
        /// </summary>
        /// <param name="pVistaPrevia">Indica si estamos en vista previa o no</param>
        /// <param name="pIdioma">Idioma</param>
        /// <returns>nombre de la entidad</returns>
        public string NombrePropiedad(bool pVistaPrevia, string pIdioma)
        {
            if (pVistaPrevia)
            {
                if (AtrNombreLectura.ContainsKey(pIdioma))
                {
                    return AtrNombreLectura[pIdioma];
                }
                else
                {
                    foreach (string valor in AtrNombreLectura.Values)
                    {
                        return valor;
                    }
                }
            }
            else
            {
                if (AtrNombre.ContainsKey(pIdioma))
                {
                    return AtrNombre[pIdioma];
                }
                else
                {
                    foreach (string valor in AtrNombre.Values)
                    {
                        return valor;
                    }
                }
            }

            return null;
        }

        #region TipoCampo

        /// <summary>
        /// Devuelve el tipo de campo depediendo que el rango de una propiedad.
        /// </summary>
        /// <returns> El TipoCampoOntologia que corresponde al rango de una propiedad</returns>
        private TipoCampoOntologia ObtenerTipoCampo()
        {
            if (mPropiedad.ListaValoresPermitidos != null && mPropiedad.ListaValoresPermitidos.Count > 0)
            {//La propiedad podrá tomar solo un grupo de valores (One Of)
                if (mPropiedad.RangoRelativo == "" || mPropiedad.RangoRelativo.ToLower() == "string")//Sin rango o String
                {
                    if (EsPropiedadConValoresCheck)
                    {
                        return TipoCampoOntologia.Checks;
                    }

                    return TipoCampoOntologia.ListaTexto;
                }
                else if (mPropiedad.RangoRelativo.ToLower() == "int" || mPropiedad.RangoRelativo.ToLower() == "integer")
                {
                    return TipoCampoOntologia.ListaEnteros;
                }
                else if (mPropiedad.RangoRelativo.ToLower() == "float")
                {
                    return TipoCampoOntologia.ListaNumeros;
                }
                else if (mPropiedad.RangoRelativo.ToLower() == "boolean" || mPropiedad.RangoRelativo.ToLower() == "bool")
                {
                    return TipoCampoOntologia.ListaBoleanos;
                }
                else if (mPropiedad.RangoRelativo.ToLower() == "datetime")
                {
                    return TipoCampoOntologia.ListaDateTimes;
                }
                else if (mPropiedad.RangoRelativo.ToLower() == "date")
                {
                    return TipoCampoOntologia.ListaDates;
                }
                else if (mPropiedad.RangoRelativo.ToLower() == "time")
                {
                    return TipoCampoOntologia.ListaTimes;
                }
            }
            else
            {//La propiedad podrá adoptar cualquier valor de su tipo.
                if (mPropiedad.RangoRelativo == "" || mPropiedad.RangoRelativo.ToLower() == "string")//Sin rango o String
                {
                    return TipoCampoOntologia.Texto;
                }
                else if (mPropiedad.RangoRelativo.ToLower() == "int" || mPropiedad.RangoRelativo.ToLower() == "integer")
                {
                    return TipoCampoOntologia.Entero;
                }
                else if (mPropiedad.RangoRelativo.ToLower() == "float")
                {
                    return TipoCampoOntologia.Numerico;
                }
                else if (mPropiedad.RangoRelativo.ToLower() == "boolean" || mPropiedad.RangoRelativo.ToLower() == "bool")
                {
                    return TipoCampoOntologia.Boleano;
                }
                else if (mPropiedad.RangoRelativo.ToLower() == "datetime")
                {
                    return TipoCampoOntologia.DateTime;
                }
                else if (mPropiedad.RangoRelativo.ToLower() == "date")
                {
                    return TipoCampoOntologia.Date;
                }
                else if (mPropiedad.RangoRelativo.ToLower() == "time")
                {
                    return TipoCampoOntologia.Time;
                }
            }

            //Nunca alcanzará esta linea (se pone para que compile):
            return TipoCampoOntologia.Texto;
        }

        #endregion

        #endregion

        #region Dispose

        /// <summary>
        /// Destructor
        /// </summary>
        ~EstiloPlantillaEspecifProp()
        {
            //Libero los recursos
            Dispose(false, false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        /// <param name="pEliminarListasComunes">Indica si hay que eliminar listas comunes entre elementos ontologías copiados</param>
        protected override void Dispose(bool disposing, bool pEliminarListasComunes)
        {
            if (!disposed)
            {
                disposed = true;
                //if (disposing)
                //{
                //Libero todos los recursos administrados que he añadido a esta clase
                //}

                mPropiedad = null;
                mListaValoresPermitidos = null;
                mPrivadoParaGrupoEditores = null;
                mAtrNombre = null;
                mAtrNombreLectura = null;
                mMicroformatos = null;
                mElementoOrdenadoAuxiliar = null;
                mImagenMini = null;
                mPropiedadesAuxiliares = null;
                mRestrNumCaract = null;
                mSelectorEntidad = null;
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
            info.AddValue("AtrNombre", mAtrNombre);
            info.AddValue("AtrNombreLectura", mAtrNombreLectura);
            info.AddValue("CampoDeshabilitado", mCampoDeshabilitado);
            info.AddValue("CapturarFlash", mCapturarFlash);
            info.AddValue("ClaseCss", mClaseCss);
            info.AddValue("ClaseCssPanel", mClaseCssPanel);
            info.AddValue("ClaseCssPanelTitulo", mClaseCssPanelTitulo);
            info.AddValue("ElementoOrdenadoAuxiliar", mElementoOrdenadoAuxiliar);
            info.AddValue("EsPropiedadConValoresCheck", mEsPropiedadConValoresCheck);
            info.AddValue("EsSelectorEntidadInterno", mEsSelectorEntidadInterno);
            info.AddValue("ExpresionRegular", mExpresionRegular);
            info.AddValue("FechaConHora", mFechaConHora);
            info.AddValue("FechaLibre", mFechaLibre);
            info.AddValue("FechaMesAnio", mFechaMesAnio);
            info.AddValue("GaleriaImagenes", mGaleriaImagenes);
            info.AddValue("GrafoAutocompletar", mGrafoAutocompletar);
            info.AddValue("GrafoDependiente", mGrafoDependiente);
            info.AddValue("GuardarFechaComoEntero", mGuardarFechaComoEntero);
            info.AddValue("GuardarValoresAutocompletar", mGuardarValoresAutocompletar);
            info.AddValue("HtmlObjeto", mHtmlObjeto);
            info.AddValue("ImagenMini", mImagenMini);
            info.AddValue("ListaValoresPermitidos", mListaValoresPermitidos);
            info.AddValue("MaxSizeJcrop", mMaxSizeJcrop);
            info.AddValue("Microdatos", mMicrodatos);
            info.AddValue("Microformatos", mMicroformatos);
            info.AddValue("MinSizeJcrop", mMinSizeJcrop);
            info.AddValue("NombreEntidad", mNombreEntidad);
            info.AddValue("NombreRealPropiedad", mNombreRealPropiedad);
            info.AddValue("NoMultiIdioma", mNoMultiIdioma);
            info.AddValue("NoPermitirNuevosValores", mNoPermitirNuevosValores);
            info.AddValue("NuevaPestanya", mNuevaPestanya);
            info.AddValue("NumElemPorPag", mNumElemPorPag);
            info.AddValue("OpenSeaDragon", mOpenSeaDragon);
            info.AddValue("PrimeraCoincidenciaExpresionRegular", mPrimeraCoincidenciaExpresionRegular);
            info.AddValue("PrivadoParaGrupoEditores", mPrivadoParaGrupoEditores);
            info.AddValue("PrivadoPrivadoParaMiembrosComunidad", mPrivadoPrivadoParaMiembrosComunidad);
            info.AddValue("PropDependiente", mPropDependiente);
            info.AddValue("PropiedadesAuxiliares", mPropiedadesAuxiliares);
            info.AddValue("RestrNumCaract", mRestrNumCaract);
            info.AddValue("SelectorEntidad", mSelectorEntidad);
            info.AddValue("TagNameTituloEdicion", mTagNameTituloEdicion);
            info.AddValue("TagNameTituloLectura", mTagNameTituloLectura);
            info.AddValue("TextoAgregarElem", mTextoAgregarElem);
            info.AddValue("TextoBotonAceptarElemento", mTextoBotonAceptarElemento);
            info.AddValue("TextoCancelarElem", mTextoCancelarElem);
            info.AddValue("TextoEdicionEntSel", mTextoEdicionEntSel);
            info.AddValue("TextoEliminarElemSel", mTextoEliminarElemSel);
            info.AddValue("TipoCampoINT", mTipoCampoINT);
            info.AddValue("TipoCampo", mTipoCampo);
            info.AddValue("TipoCampoLectura", mTipoCampoLectura);
            info.AddValue("TipoDependiente", mTipoDependiente);
            info.AddValue("TipoEntDependiente", mTipoEntDependiente);
            info.AddValue("TipoResulAutocompletar", mTipoResulAutocompletar);
            info.AddValue("UrlLinkDelValor", mUrlLinkDelValor);
            info.AddValue("UsarJcrop", mUsarJcrop);
            info.AddValue("ValorDefectoNoSeleccionable", mValorDefectoNoSeleccionable);
            info.AddValue("ValoresSepComas", mValoresSepComas);
            info.AddValue("ValorPorDefecto", mValorPorDefecto);
            info.AddValue("VistaPersonalizadaPaginacion", mVistaPersonalizadaPaginacion);
            info.AddValue("VistaPrevEnEdicion", mVistaPrevEnEdicion);
        }

        #endregion
    }

    /// <summary>
    /// Restricción del número de caracteres de una propiedad.
    /// </summary>
    [Serializable]
    public class RestriccionNumCaracteres
    {
        #region Restricción número caracteres

        /// <summary>
        /// Valor para la restricción de los caracteres: Menor.
        /// </summary>
        public static string RestriCaract_Menor = "<";

        /// <summary>
        /// Valor para la restricción de los caracteres: Menor.
        /// </summary>
        public static string RestriCaract_Mayor = ">";

        /// <summary>
        /// Valor para la restricción de los caracteres: Menor.
        /// </summary>
        public static string RestriCaract_Igual = "=";

        /// <summary>
        /// Valor para la restricción de los caracteres: Menor.
        /// </summary>
        public static string RestriCaract_Entre = "-";

        #endregion

        #region Miembros

        /// <summary>
        /// Tipo de restricción.
        /// </summary>
        private string mTipoRestricion;

        /// <summary>
        /// Valor restricción
        /// </summary>
        private int mValor;

        /// <summary>
        /// Valor máximo para la restricción entre X e Y.
        /// </summary>
        private int mValorHasta = -1;

        #endregion

        #region Propiedades

        /// <summary>
        /// Tipo de restricción.
        /// </summary>
        public string TipoRestricion
        {
            get
            {
                return mTipoRestricion;
            }
            set
            {
                mTipoRestricion = value;
            }
        }

        /// <summary>
        /// Valor restricción
        /// </summary>
        public int Valor
        {
            get
            {
                return mValor;
            }
            set
            {
                mValor = value;
            }
        }

        /// <summary>
        /// Valor máximo para la restricción entre X e Y.
        /// </summary>
        public int ValorHasta
        {
            get
            {
                return mValorHasta;
            }
            set
            {
                mValorHasta = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// Imagén miniatura para un propiedad de tipo imagen.
    /// </summary>

    [Serializable]
    public class ImagenMini
    {
        #region Miembros

        /// <summary>
        /// Tamaños de la imágenes
        /// </summary>
        private Dictionary<int, int> mTamanios;

        /// <summary>
        /// Tipo de recorte o redimensión.
        /// </summary>
        private Dictionary<int, string> mTipo;

        #endregion

        #region Propiedades

        /// <summary>
        /// Tamaños de la imágenes
        /// </summary>
        public Dictionary<int, int> Tamanios
        {
            get
            {
                if (mTamanios == null)
                {
                    mTamanios = new Dictionary<int, int>();
                }

                return mTamanios;
            }
            set
            {
                mTamanios = value;
            }
        }

        /// <summary>
        /// Tipo de recorte o redimensión.
        /// </summary>
        public Dictionary<int, string> Tipo
        {
            get
            {
                if (mTipo == null)
                {
                    mTipo = new Dictionary<int, string>();
                }

                return mTipo;
            }
            set
            {
                mTipo = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// Selector de una entidad como valor de una propiedad.
    /// </summary>

    [Serializable]
    public class SelectorEntidad
    {
        #region Miembros

        /// <summary>
        /// Url de la entiad contenedora de la instancia buscada.
        /// </summary>
        private string mUrlEntContenedora;

        /// <summary>
        /// Url de la propiedad para la consulta
        /// </summary>
        private string mUrlPropiedad;

        /// <summary>
        /// Url del tipo de entidad solicitada.
        /// </summary>
        private string mUrlTipoEntSolicitada;

        /// <summary>
        /// Propiedades para llenar el combo.
        /// </summary>
        private List<string> mPropiedadesEdicion;

        /// <summary>
        /// Propiedades que se mostrarán en la vista previa del formulario.
        /// </summary>
        private List<EstiloPlantillaEspecifProp> mPropiedadesLectura;

        /// <summary>
        /// Grafo de la entidad solicitada.
        /// </summary>
        private string mGrafo;

        /// <summary>
        /// Namespace del grafo de la entidad solicitada.
        /// </summary>
        private string mNamespaceGrafo;

        /// <summary>
        /// Texto para el 1º elemento de la selección.
        /// </summary>
        private string mTextoElemento0;

        /// <summary>
        /// Tipo de presentación.
        /// </summary>
        private string mTipoPresentacion;

        /// <summary>
        /// Tipo de selector.
        /// </summary>
        private string mTipoSeleccion;

        /// <summary>
        /// Indica si el selecctor es multiidioma o no.
        /// </summary>
        private bool mMultiIdioma;

        /// <summary>
        /// Tipo de selector.
        /// </summary>
        private bool mAnidamientoGnoss;

        /// <summary>
        /// Indica si la entidad debe ser un link al recurso que la contiene.
        /// </summary>
        private bool mLinkARecurso;

        /// <summary>
        /// Indica el tipo de entidades en las que se aplica LinkARecurso.
        /// </summary>
        private List<string> mTipoEntLinkARecurso;

        /// <summary>
        /// Indica las propiedades en las que se aplica LinkARecurso.
        /// </summary>
        private List<string> mPropLinkARecurso;

        /// <summary>
        /// Indica si el link a recurso debe llevar a la comunidad actual o no.
        /// </summary>
        private bool mLinkARecursoVaAComunidad;

        /// <summary>
        /// Atributos de un recurso.
        /// </summary>
        private List<string> mAtributosRecurso;

        /// <summary>
        /// Indica si el link al recurso que la contiene debe abrirse en una nueva pestaña.
        /// </summary>
        private bool mNuevaPestanya = true;

        /// <summary>
        /// Indica si la relación de entidades externas es recíproca.
        /// </summary>
        private bool mReciproca;

        /// <summary>
        /// Propiedad por la que se debe ordenar la consulta recíproca y el tipo de orden.
        /// </summary>
        private KeyValuePair<string, string> mPropOrdenRecipocidad;

        // <summary>
        /// Indica el nombre de la propiedad que tendrán las entidades externas recíprocas.
        /// </summary>
        private string mPropiedadReciproca;

        /// <summary>
        /// Indica el nombre de la propiedad de la entidad externa que enlaza con la edición de la propiedad actual.
        /// </summary>
        private string mPropiedadEdicionReciproca;

        /// <summary>
        /// Indica el tipo de la entidad externa que enlaza con la edición de la propiedad actual.
        /// </summary>
        private string mEntidadEdicionReciproca;

        /// <summary>
        /// Consulta para obtener las entidades externas recíprocas.
        /// </summary>
        private string mConsultaReciproca;

        /// <summary>
        /// Consulta para obtener las entidades externas.
        /// </summary>
        private string mConsulta;

        /// <summary>
        /// Consulta de edición para obtener las entidades externas.
        /// </summary>
        private string mConsultaEdicion;

        /// <summary>
        /// Cadena extra del where de la consulta de autocompletar para obtener las entidades externas.
        /// </summary>
        private string mExtraWhereAutocompletar;

        /// <summary>
        /// Cadenas extra de extra del where de la consulta de autocompletar para obtener las entidades externas.
        /// </summary>
        private List<string> mExtraWhereAutocompletarExtras;

        /// <summary>
        /// Mensaje para mostrar cuando no hay resultados en la selección de entidad.
        /// </summary>
        private string mMensajeNoResultados;

        /// <summary>
        /// Número de elementos por página para el selector de entidad.
        /// </summary>
        private int mNumElemPorPag;

        /// <summary>
        /// Ruta de la vista personalizada para la paginación en caso de que la haya.
        /// </summary>
        private string mVistaPersonalizadaPaginacion;

        /// <summary>
        /// Consulta que se debe ejecutar para obtener los sujetos que cumplen la dependecia del selector.
        /// </summary>
        private string mConsultaDependiente;

        /// <summary>
        /// ID y Tipo de entidad de la propiedad de la que depende el selector.
        /// </summary>
        private KeyValuePair<string, string> mPropiedadDeLaQueDepende;

        /// <summary>
        /// Indica si los contenidos relacionados se mostrarán únicamente en el idioma de navegación del usuario
        /// </summary>
        private bool mSoloIdiomaUsuario;

        #endregion

        #region Propiedades

        /// <summary>
        /// Url de la entiad contenedora de la instancia buscada.
        /// </summary>
        public string UrlEntContenedora
        {
            get
            {
                return mUrlEntContenedora;
            }
            set
            {
                mUrlEntContenedora = value;
            }
        }

        /// <summary>
        /// Url de la propiedad para la consulta
        /// </summary>
        public string UrlPropiedad
        {
            get
            {
                return mUrlPropiedad;
            }
            set
            {
                mUrlPropiedad = value;
            }
        }

        /// <summary>
        /// Url del tipo de entidad solicitada.
        /// </summary>
        public string UrlTipoEntSolicitada
        {
            get
            {
                return mUrlTipoEntSolicitada;
            }
            set
            {
                mUrlTipoEntSolicitada = value;
            }
        }

        /// <summary>
        /// Propiedades para llenar el combo.
        /// </summary>
        public List<string> PropiedadesEdicion
        {
            get
            {
                if (mPropiedadesEdicion == null)
                {
                    mPropiedadesEdicion = new List<string>();
                }

                return mPropiedadesEdicion;
            }
            set
            {
                mPropiedadesEdicion = value;
            }
        }

        /// <summary>
        /// Propiedades que se mostrarán en la vista previa del formulario.
        /// </summary>
        public List<EstiloPlantillaEspecifProp> PropiedadesLectura
        {
            get
            {
                if (mPropiedadesLectura == null)
                {
                    mPropiedadesLectura = new List<EstiloPlantillaEspecifProp>();
                }

                return mPropiedadesLectura;
            }
            set
            {
                mPropiedadesLectura = value;
            }
        }

        /// <summary>
        /// Grafo de la entidad solicitada.
        /// </summary>
        public string Grafo
        {
            get
            {
                return mGrafo;
            }
            set
            {
                mGrafo = value;
            }
        }

        /// <summary>
        /// Namespace del grafo de la entidad solicitada.
        /// </summary>
        public string NamespaceGrafo
        {
            get
            {
                return mNamespaceGrafo;
            }
            set
            {
                mNamespaceGrafo = value;
            }
        }

        /// <summary>
        /// Texto para el 1º elemento de la selección.
        /// </summary>
        public string TextoElemento0
        {
            get
            {
                return mTextoElemento0;
            }
            set
            {
                mTextoElemento0 = value;
            }
        }

        /// <summary>
        /// Tipo de presentación.
        /// </summary>
        public string TipoPresentacion
        {
            get
            {
                return mTipoPresentacion;
            }
            set
            {
                mTipoPresentacion = value;
            }
        }

        /// <summary>
        /// Tipo de selector.
        /// </summary>
        public string TipoSeleccion
        {
            get
            {
                return mTipoSeleccion;
            }
            set
            {
                mTipoSeleccion = value;
            }
        }

        public bool Cache { get; set; } = false;

        /// <summary>
        /// Indica si el selecctor es multiidioma o no.
        /// </summary>
        public bool MultiIdioma
        {
            get
            {
                return mMultiIdioma;
            }
            set
            {
                mMultiIdioma = value;
            }
        }

        /// <summary>
        /// Tipo de selector.
        /// </summary>
        public bool AnidamientoGnoss
        {
            get
            {
                return mAnidamientoGnoss;
            }
            set
            {
                mAnidamientoGnoss = value;
            }
        }

        /// <summary>
        /// Indica si la entidad debe ser un link al recurso que la contiene.
        /// </summary>
        public bool LinkARecurso
        {
            get
            {
                return mLinkARecurso;
            }
            set
            {
                mLinkARecurso = value;
            }
        }

        /// <summary>
        /// Indica el tipo de entidades en las que se aplica LinkARecurso.
        /// </summary>
        public List<string> TipoEntLinkARecurso
        {
            get
            {
                return mTipoEntLinkARecurso;
            }
            set
            {
                mTipoEntLinkARecurso = value;
            }
        }

        /// <summary>
        /// Indica las propiedades en las que se aplica LinkARecurso.
        /// </summary>
        public List<string> PropLinkARecurso
        {
            get
            {
                return mPropLinkARecurso;
            }
            set
            {
                mPropLinkARecurso = value;
            }
        }

        /// <summary>
        /// Indica si el link a recurso debe llevar a la comunidad actual o no.
        /// </summary>
        public bool LinkARecursoVaAComunidad
        {
            get
            {
                return mLinkARecursoVaAComunidad;
            }
            set
            {
                mLinkARecursoVaAComunidad = value;
            }
        }

        /// <summary>
        /// Atributos de un recurso.
        /// </summary>
        public List<string> AtributosRecurso
        {
            get
            {
                return mAtributosRecurso;
            }
            set
            {
                mAtributosRecurso = value;
            }
        }

        /// <summary>
        /// Indica si el link al recurso que la contiene debe abrirse en una nueva pestaña.
        /// </summary>
        public bool NuevaPestanya
        {
            get
            {
                return mNuevaPestanya;
            }
            set
            {
                mNuevaPestanya = value;
            }
        }

        /// <summary>
        /// Indica si la relación de entidades externas es recíproca.
        /// </summary>
        public bool Reciproca
        {
            get
            {
                return mReciproca;
            }
            set
            {
                mReciproca = value;
            }
        }

        /// <summary>
        /// Propiedad por la que se debe ordenar la consulta recíproca y el tipo de orden.
        /// </summary>
        public KeyValuePair<string, string> PropOrdenRecipocidad
        {
            get
            {
                return mPropOrdenRecipocidad;
            }
            set
            {
                mPropOrdenRecipocidad = value;
            }
        }

        /// <summary>
        /// Indica el nombre de la propiedad que tendrán las entidades externas recíprocas.
        /// </summary>
        public string PropiedadReciproca
        {
            get
            {
                return mPropiedadReciproca;
            }
            set
            {
                mPropiedadReciproca = value;
            }
        }

        /// <summary>
        /// Indica el nombre de la propiedad de la entidad externa que enlaza con la edición de la propiedad actual.
        /// </summary>
        public string PropiedadEdicionReciproca
        {
            get
            {
                return mPropiedadEdicionReciproca;
            }
            set
            {
                mPropiedadEdicionReciproca = value;
            }
        }

        /// <summary>
        /// Indica el tipo de la entidad externa que enlaza con la edición de la propiedad actual.
        /// </summary>
        public string EntidadEdicionReciproca
        {
            get
            {
                return mEntidadEdicionReciproca;
            }
            set
            {
                mEntidadEdicionReciproca = value;
            }
        }

        /// <summary>
        /// Consulta para obtener las entidades externas recíprocas.
        /// </summary>
        public string ConsultaReciproca
        {
            get
            {
                return mConsultaReciproca;
            }
            set
            {
                mConsultaReciproca = value;
            }
        }

        /// <summary>
        /// Consulta para obtener las entidades externas.
        /// </summary>
        public string Consulta
        {
            get
            {
                return mConsulta;
            }
            set
            {
                mConsulta = value;
            }
        }

        /// <summary>
        /// Consulta de edición para obtener las entidades externas.
        /// </summary>
        public string ConsultaEdicion
        {
            get
            {
                return mConsultaEdicion;
            }
            set
            {
                mConsultaEdicion = value;
            }
        }

        /// <summary>
        /// Cadena extra del where de la consulta de autocompletar para obtener las entidades externas.
        /// </summary>
        public string ExtraWhereAutocompletar
        {
            get
            {
                return mExtraWhereAutocompletar;
            }
            set
            {
                mExtraWhereAutocompletar = value;
            }
        }

        /// <summary>
        /// Cadenas extra de extra del where de la consulta de autocompletar para obtener las entidades externas.
        /// </summary>
        public List<string> ExtraWhereAutocompletarExtras
        {
            get
            {
                return mExtraWhereAutocompletarExtras;
            }
            set
            {
                mExtraWhereAutocompletarExtras = value;
            }
        }

        /// <summary>
        /// Mensaje para mostrar cuando no hay resultados en la selección de entidad.
        /// </summary>
        public string MensajeNoResultados
        {
            get
            {
                return mMensajeNoResultados;
            }
            set
            {
                mMensajeNoResultados = value;
            }
        }

        /// <summary>
        /// Número de elementos por página para el selector de entidad.
        /// </summary>
        public int NumElemPorPag
        {
            get
            {
                return mNumElemPorPag;
            }
            set
            {
                mNumElemPorPag = value;
            }
        }

        /// <summary>
        /// Ruta de la vista personalizada para la paginación en caso de que la haya.
        /// </summary>
        public string VistaPersonalizadaPaginacion
        {
            get
            {
                return mVistaPersonalizadaPaginacion;
            }
            set
            {
                mVistaPersonalizadaPaginacion = value;
            }
        }

        /// <summary>
        /// Consulta que se debe ejecutar para obtener los sujetos que cumplen la dependecia del selector.
        /// </summary>
        public string ConsultaDependiente
        {
            get
            {
                return mConsultaDependiente;
            }
            set
            {
                mConsultaDependiente = value;
            }
        }

        /// <summary>
        /// ID y Tipo de entidad de la propiedad de la que depende el selector.
        /// </summary>
        public KeyValuePair<string, string> PropiedadDeLaQueDepende
        {
            get
            {
                return mPropiedadDeLaQueDepende;
            }
            set
            {
                mPropiedadDeLaQueDepende = value;
            }
        }

        /// <summary>
        /// Indica si los contenidos relacionados se mostrarán únicamente en el idioma de navegación del usuario
        /// </summary>
        public bool SoloIdiomaUsuario
        {
            get
            {
                return mSoloIdiomaUsuario;
            }
            set
            {
                mSoloIdiomaUsuario = value;
            }
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Propiedades lectura más las propiedades Auxiliares de cada prop jerarquicamente.
        /// </summary>
        public static List<EstiloPlantillaEspecifProp> PropiedadesLecturaYAuxiliares(List<EstiloPlantillaEspecifProp> pPropiedadesLectura)
        {
            List<EstiloPlantillaEspecifProp> listaEsp = new List<EstiloPlantillaEspecifProp>();
            listaEsp.AddRange(pPropiedadesLectura);

            foreach (EstiloPlantillaEspecifProp especif in pPropiedadesLectura)
            {
                if (especif.PropiedadesAuxiliares != null && especif.PropiedadesAuxiliares.Count > 0)
                {
                    listaEsp.AddRange(PropiedadesLecturaYAuxiliares(especif.PropiedadesAuxiliares));
                }
            }

            return listaEsp;
        }

        #endregion
    }
}
