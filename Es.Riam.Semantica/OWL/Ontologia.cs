using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using Es.Riam.Semantica.Plantillas;
using System.Linq;

namespace Es.Riam.Semantica.OWL
{
    /// <summary>
    /// Representa una ontología leída desde un archivo en formato OWL.
    /// </summary>
    [Serializable]
    public class Ontologia: IDisposable, ISerializable, ICloneable
    {
        #region Miembros

        /// <summary>
        /// Entidades relacionadas con la entidad principal.
        /// </summary>
        private List<ElementoOntologia> mEntidades;

        /// <summary>
        /// Tipo de entidades relacionadas con la entidad principal.
        /// </summary>
        private List<string> mTiposEntidades;

        /// <summary>
        /// Fichero que posee la ontología.
        /// </summary>
        private StreamReader mFicheroOntologia=null;

        /// <summary>
        /// Ficheros que poseen las ontologías.
        /// </summary>
        private List<string> mRutasOntologias;

        /// <summary>
        /// Bytes con los ficheros que poseen la ontologías.
        /// </summary>
        private List<byte[]> mBytesFicherosOntologia;

        /// <summary>
        /// Gestor de OWL
        /// </summary>
        protected GestionOWL mGestionOwl;

        /// <summary>
        /// Lista con los namespaces definidos para la ontología, clave-valor.
        /// </summary>
        protected Dictionary<string, string> mNamespacesDefinidos;

        /// <summary>
        /// Lista con los namespaces definidos para la ontología, clave-valor, 1º namespace, 2º url.
        /// </summary>
        protected Dictionary<string, string> mNamespacesDefinidosInv;

        /// <summary>
        /// Lista con los namespaces extra para la ontología, clave-valor, 1º namespace, 2º url.
        /// </summary>
        protected Dictionary<string, string> mNamespacesDefinidosExtra;

        /// <summary>
        /// Lista con los valores de namespaces que hacen referencia a namespaces definidos para la ontología, valorReferencia-clave.
        /// </summary>
        protected Dictionary<string, string> mValorVerdaderNamespacesReferencia;

        /// <summary>
        /// RDF de un CV semántico para incluirlo en el de una persona.
        /// </summary>
        protected string mRDFCVSemIncluido;

        /// <summary>
        /// Valor que indicas si en la ontología se usan IDs relativos o no.
        /// </summary>
        protected bool mUsoIDsRelativos;

        /// <summary>
        /// Url de las ontologías importadas.
        /// </summary>
        protected List<string> mUrlOntologiasImportadas;

        /// <summary>
        /// Estilos de la plantilla actual.
        /// </summary>
        private Dictionary<string, List<EstiloPlantilla>> mEstilosPlantilla;

        /// <summary>
        /// Configuración de la plantilla.
        /// </summary>
        private EstiloPlantillaConfigGen mConfiguracionPlantilla;

        /// <summary>
        /// Genarar namespaces si hay Urls huerfanas.
        /// </summary>
        private bool mGenararNamespacesHuerfanos;

        /// <summary>
        /// Idioma del usuario.
        /// </summary>
        private string mIdiomaUsuario;

        /// <summary>
        /// Identificador ontologia
        /// </summary>
        private Guid mOntologiaID;

        /// <summary>
        /// Lista con las ontologías externas relacionadas con la actual.
        /// </summary>
        private Dictionary<string, Ontologia> mOntologiasExternas;

        /// <summary>
        /// Indica que es una ontología auxiliar inventada para los selectores de entidad.
        /// </summary>
        private bool mOntoAuxiliarInventada;

        private List<ElementoOntologia> mEntidadesAuxiliares = null;

        #endregion

        #region Constructores

        /// <summary>
        /// Crea una ontología a partir de la ruta del fichero de ontología.
        /// </summary>
        /// <param name="pRutaOntologiaCompleta">Directorio en el que se encuentra el fichero de ontología.</param>
        public Ontologia(string pRutaOntologiaCompleta) : this(pRutaOntologiaCompleta, false)
        {
        }

        /// <summary>
        /// Crea una ontología a partir de la ruta del fichero de ontología.
        /// </summary>
        /// <param name="pRutaOntologiaCompleta">Directorio en el que se encuentra el fichero de ontología.</param>
        /// <param name="pUsoIDsRelativos">Valor que indicas si en la ontología se usan IDs relativos o no.</param>
        public Ontologia(string pRutaOntologiaCompleta, bool pUsoIDsRelativos)
        {
            this.mEntidades = new List<ElementoOntologia>();
            this.mUsoIDsRelativos = pUsoIDsRelativos;

            StreamReader reader = new StreamReader(pRutaOntologiaCompleta);
            string archivo = reader.ReadToEnd();
            reader.Close();
            reader.Dispose();
            reader = null;

            byte[] byteArray = Encoding.UTF8.GetBytes(archivo);
            MemoryStream stream = new MemoryStream(byteArray);
            this.mFicheroOntologia = new StreamReader(stream);
        }

        /// <summary>
        /// Crea una ontología a partir de los bytes del fichero de la ontología.
        /// </summary>
        /// <param name="pContenidoOntologia">Bytes del fichero de ontología.</param>
        public Ontologia(byte[] pContenidoOntologia) : this(pContenidoOntologia, false)
        {
        }

        /// <summary>
        /// Crea una ontología a partir de los bytes del fichero de la ontología.
        /// </summary>
        /// <param name="pContenidoOntologia">Bytes del fichero de ontología.</param>
        /// <param name="pUsoIDsRelativos">Valor que indicas si en la ontología se usan IDs relativos o no.</param>
        public Ontologia(byte[] pContenidoOntologia, bool pUsoIDsRelativos)
        {
            this.mEntidades = new List<ElementoOntologia>();
            this.mUsoIDsRelativos = pUsoIDsRelativos;

            MemoryStream memory = new MemoryStream(pContenidoOntologia);
            this.mFicheroOntologia = new StreamReader(memory);
        }

        /// <summary>
        /// Crea una ontología a partir de una lista de rutas, las rutas de los ficheros de ontología.
        /// </summary>
        /// <param name="pRutasOntologias">Ficheros de ontología.</param>
        public Ontologia(List<string> pRutasOntologias)
        {
            this.mEntidades = new List<ElementoOntologia>();
            mRutasOntologias = pRutasOntologias;
        }

        /// <summary>
        /// Crea una ontología a partir de una lista de bytes de los ficheros de las ontologías.
        /// </summary>
        /// <param name="pContenidoOntologias">Lista con los Bytes de los ficheros de ontologías.</param>
        public Ontologia(List<byte[]> pContenidoOntologias)
        {
            this.mEntidades = new List<ElementoOntologia>();
            mBytesFicherosOntologia = pContenidoOntologias;
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected Ontologia(SerializationInfo info, StreamingContext context)
        {
            mEntidades = (List<ElementoOntologia>)info.GetValue("Entidades", typeof(List<ElementoOntologia>));
            mTiposEntidades = (List<string>)info.GetValue("TiposEntidades", typeof(List<string>));
            mNamespacesDefinidos = (Dictionary<string, string>)info.GetValue("NamespacesDefinidos", typeof(Dictionary<string, string>));
            mRDFCVSemIncluido = (string)info.GetValue("RDFCVSemIncluido", typeof(string));
            mRutasOntologias = (List<string>)info.GetValue("RutasOntologias", typeof(List<string>));
            mUsoIDsRelativos = (bool)info.GetValue("UsoIDsRelativos", typeof(bool));
            mUrlOntologiasImportadas = (List<string>)info.GetValue("UrlOntologiasImportadas", typeof(List<string>));
            mValorVerdaderNamespacesReferencia = (Dictionary<string, string>)info.GetValue("ValorVerdaderNamespacesReferencia", typeof(Dictionary<string, string>));
            
            mGestionOwl = (GestionOWL)info.GetValue("GestionOwl", typeof(GestionOWL));
            mEstilosPlantilla = (Dictionary<string, List<EstiloPlantilla>>)info.GetValue("EstilosPlantilla", typeof(Dictionary<string, List<EstiloPlantilla>>));
            mConfiguracionPlantilla = (EstiloPlantillaConfigGen)info.GetValue("ConfiguracionPlantilla", typeof(EstiloPlantillaConfigGen));
            mIdiomaUsuario = (string)info.GetValue("IdiomaUsuario", typeof(string));
            mOntologiaID = (Guid)info.GetValue("OntologiaID", typeof(Guid));
            mNamespacesDefinidosInv = (Dictionary<string, string>)info.GetValue("NamespacesDefinidosInv", typeof(Dictionary<string, string>));
        }

        /// <summary>
        /// Crea una ontología a partir de la ruta del fichero de ontología.
        /// </summary>>
        public Ontologia()
        {
            mEntidades = new List<ElementoOntologia>();
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene el gestorOWL
        /// </summary>
        public virtual GestionOWL GestorOWL
        {
            get
            {
                if (mGestionOwl == null)
                    mGestionOwl = new GestionOWL();
                if (mGestionOwl.Ontologia == null) // añadido para serializar
                    mGestionOwl.Ontologia = this;
                return mGestionOwl;
            }
            set
            {
                mGestionOwl = value;
            }
        }

        /// <summary>
        /// Obtiene la lista de entidades de la Ontología
        /// </summary>
        public List<ElementoOntologia> Entidades
        {
            get
            {
                return this.mEntidades;
            }
            set
            {
                this.mEntidades = value;
            }
        }

        public List<Propiedad> Propiedades { get; set; }

        public List<ElementoOntologia> EntidadesAuxiliares
        {
            get
            {
                if(mEntidadesAuxiliares == null)
                {
                    mEntidadesAuxiliares = new List<ElementoOntologia>();
                    foreach (Propiedad propAux in Propiedades.Where(prop => prop.Tipo.Equals(TipoPropiedad.ObjectProperty) && !string.IsNullOrEmpty(prop.Rango)).Distinct()) 
                    {
                        string rango = propAux.Rango;

                        ElementoOntologia entidadAuxiliar = GetEntidadTipo(rango);
                        if (!mEntidadesAuxiliares.Any(e => e.Descripcion.Equals(entidadAuxiliar.Descripcion)))
                        {
                            mEntidadesAuxiliares.Add(entidadAuxiliar);
                        }

                        if (entidadAuxiliar.Subclases.Any())
                        {
                            foreach(string rangoHija in entidadAuxiliar.Subclases)
                            {
                                ElementoOntologia entidadAuxiliarHija = GetEntidadTipo(rangoHija);
                                if (!mEntidadesAuxiliares.Any(e => e.Descripcion.Equals(entidadAuxiliarHija.Descripcion)))
                                {
                                    mEntidadesAuxiliares.Add(entidadAuxiliarHija);
                                }
                            }
                        }
                    }
                }
                return mEntidadesAuxiliares;
            }
        }

        /// <summary>
        /// Tipo de entidades relacionadas con la entidad principal.
        /// </summary>
        public List<string> TiposEntidades
        {
            get
            {
                if (mTiposEntidades == null)
                {
                    mTiposEntidades = new List<string>();
                }

                return mTiposEntidades;
            }
            set
            {
                mTiposEntidades = value;
            }
        }

        /// <summary>
        /// Lista con los namespaces definidos para la ontología, clave-valor, 1º url, 2º namespace.
        /// </summary>
        public Dictionary<string, string> NamespacesDefinidos
        {
            get
            {
                if (mNamespacesDefinidos == null)
                {
                    mNamespacesDefinidos = new Dictionary<string, string>();
                }
                return mNamespacesDefinidos;
            }
            set
            {
                mNamespacesDefinidos = value;
            }
        }

        /// <summary>
        /// Lista con los namespaces definidos para la ontología, clave-valor, 1º namespace, 2º url.
        /// </summary>
        public Dictionary<string, string> NamespacesDefinidosInv
        {
            get
            {
                if (mNamespacesDefinidosInv == null)
                {
                    mNamespacesDefinidosInv = new Dictionary<string, string>();
                }
                return mNamespacesDefinidosInv;
            }
            set
            {
                mNamespacesDefinidosInv = value;
            }
        }

        /// <summary>
        /// Lista con los namespaces extra para la ontología, clave-valor, 1º namespace, 2º url.
        /// </summary>
        public Dictionary<string, string> NamespacesDefinidosExtra
        {
            get
            {
                if (mNamespacesDefinidosExtra == null)
                {
                    mNamespacesDefinidosExtra = new Dictionary<string, string>();
                }
                return mNamespacesDefinidosExtra;
            }
            set
            {
                mNamespacesDefinidosExtra = value;
            }
        }

        /// <summary>
        /// Lista con los valores de namespaces que hacen referencia a namespaces definidos para la ontología, valorReferencia-clave.
        /// </summary>
        public Dictionary<string, string> ValorVerdaderNamespacesReferencia
        {
            get
            {
                if (mValorVerdaderNamespacesReferencia == null)
                {
                    mValorVerdaderNamespacesReferencia = new Dictionary<string, string>();
                }
                return mValorVerdaderNamespacesReferencia;
            }
            set
            {
                mValorVerdaderNamespacesReferencia = value;
            }
        }

        /// <summary>
        /// Devuelve o establce el RDF de un CV semántico para incluirlo en el de una persona.
        /// </summary>
        public string RDFCVSemIncluido
        {
            get
            {
                return mRDFCVSemIncluido;
            }
            set
            {
                mRDFCVSemIncluido = value;
            }
        }

        /// <summary>
        /// Devuelve o establce el valor que indicas si en la ontología se usan IDs relativos o no.
        /// </summary>
        public bool UsoIDsRelativos
        {
            get
            {
                return mUsoIDsRelativos;
            }
            set
            {
                mUsoIDsRelativos = value;
            }
        }

        /// <summary>
        /// Url de las ontologías importadas.
        /// </summary>
        public List<string> UrlOntologiasImportadas
        {
            get
            {
                if (mUrlOntologiasImportadas == null)
                {
                    mUrlOntologiasImportadas = new List<string>();
                }

                return mUrlOntologiasImportadas;
            }
            set
            {
                mUrlOntologiasImportadas = value;
            }
        }

        #region Estilo Plantillas

        /// <summary>
        /// Estilos de la plantilla actual.
        /// </summary>
        public Dictionary<string, List<EstiloPlantilla>> EstilosPlantilla
        {
            get
            {
                // añadido para serializar
                if (mEstilosPlantilla != null)
                {
                    foreach (string key in mEstilosPlantilla.Keys)
                    {
                        foreach (EstiloPlantilla estilo in mEstilosPlantilla[key])
                        {
                            if (estilo is EstiloPlantillaConfigGen && ((EstiloPlantillaConfigGen)estilo).Ontologia == null)
                            {
                                ((EstiloPlantillaConfigGen)estilo).Ontologia = this;
                            }
                        }
                    }
                    //
                }
                return mEstilosPlantilla;
            }
            set
            {
                mEstilosPlantilla = value;
            }
        }

        /// <summary>
        /// Configuración de la plantilla.
        /// </summary>
        public EstiloPlantillaConfigGen ConfiguracionPlantilla
        {
            get
            {
                if (mConfiguracionPlantilla == null)
                {
                    foreach (EstiloPlantilla estilo in EstilosPlantilla["[ConfiguracionGeneral]"])
                    {
                        if (estilo is EstiloPlantillaConfigGen)
                        {
                            mConfiguracionPlantilla = (EstiloPlantillaConfigGen)estilo;

                            if (!this.OntoAuxiliarInventada)
                            {
                                mConfiguracionPlantilla.Ontologia = this;
                            }

                            break;
                        }
                    }
                }

                if (mConfiguracionPlantilla.Ontologia == null && !this.OntoAuxiliarInventada) // añadido para serializar
                {
                    mConfiguracionPlantilla.Ontologia = this;
                }

                return mConfiguracionPlantilla;
            }
        }

        /// <summary>
        /// Genarar namespaces si hay Urls huerfanas.
        /// </summary>
        public bool GenararNamespacesHuerfanos
        {
            get
            {
                return mGenararNamespacesHuerfanos;
            }
            set
            {
                mGenararNamespacesHuerfanos = value;
            }
        }

        /// <summary>
        /// Idioma del usuario.
        /// </summary>
        public string IdiomaUsuario
        {
            get
            {
                return mIdiomaUsuario;
            }
            set
            {
                mIdiomaUsuario = value;
            }
        }

        /// <summary>
        /// Identificador ontologia
        /// </summary>
        public Guid OntologiaID
        {
            get
            {
                return mOntologiaID;
            }
            set
            {
                mOntologiaID = value;
            }
        }

        /// <summary>
        /// Lista con las ontologías externas relacionadas con la actual.
        /// </summary>
        public Dictionary<string, Ontologia> OntologiasExternas
        {
            get
            {
                return mOntologiasExternas;
            }
            set
            {
                mOntologiasExternas = value;
            }
        }

        /// <summary>
        /// Indica que es una ontología auxiliar inventada para los selectores de entidad.
        /// </summary>
        public bool OntoAuxiliarInventada
        {
            get
            {
                return mOntoAuxiliarInventada;
            }
            set
            {
                mOntoAuxiliarInventada = value;
            }
        }

        #endregion

        #endregion

        #region Métodos generales

        #region Públicos

        /// <summary>
        /// Lee el fichero de la ontología.
        /// </summary>
        public void LeerOntologia()
        {
            if (mFicheroOntologia.BaseStream.CanRead)
            {
                string contenidoOntologia = "";

                if (mFicheroOntologia != null)
                {
                    contenidoOntologia = mFicheroOntologia.ReadToEnd();
                    mFicheroOntologia.Close();
                    mFicheroOntologia.Dispose();
                }
                else if (mRutasOntologias != null) //Hay varios ficheros.
                {
                    foreach (string rutaOnto in mRutasOntologias)
                    {
                        mFicheroOntologia = new StreamReader(rutaOnto);
                        contenidoOntologia += mFicheroOntologia.ReadToEnd();

                        mFicheroOntologia.Close();
                        mFicheroOntologia.Dispose();
                        mFicheroOntologia = null;
                    }
                }
                else //Lista con los bytes de los ficheros de las ontologías.
                {
                    foreach (byte[] arrayOnto in mBytesFicherosOntologia)
                    {
                        this.mFicheroOntologia = new StreamReader(new MemoryStream(arrayOnto));

                        contenidoOntologia += mFicheroOntologia.ReadToEnd();

                        mFicheroOntologia.Close();
                        mFicheroOntologia.Dispose();
                        mFicheroOntologia = null;
                    }
                }

                GestorOWL.Ontologia = this;


                if (contenidoOntologia.LastIndexOf("<?xml") > 0)
                {
                    #region Más de una ontología en el mismo fichero

                    //Hay más de un owl guardado en el mismo archivo.
                    List<string> listaOntologias = new List<string>();
                    while (contenidoOntologia.LastIndexOf("<?xml") > 0)
                    {
                        listaOntologias.Add(contenidoOntologia.Substring(contenidoOntologia.LastIndexOf("<?xml")));
                        contenidoOntologia = contenidoOntologia.Substring(0, contenidoOntologia.LastIndexOf("<?xml"));
                    }

                    //Agrego la última tras salir del while:
                    listaOntologias.Add(contenidoOntologia);

                    //Le doy la vuelta para que se lea en el orden que está escrita:
                    listaOntologias.Reverse();

                    this.mEntidades = new List<ElementoOntologia>();
                    Propiedades = new List<Propiedad>();

                    foreach (string ontologia in listaOntologias)
                    {
                        //Leo y añado los namespaces a la ontología:
                        Dictionary<string, string> namespaces = GestionOWL.LeerNamespaces(this, ontologia);
                        foreach (string key in namespaces.Keys)
                        {
                            if (!NamespacesDefinidos.ContainsKey(key))
                            {
                                NamespacesDefinidos.Add(key, namespaces[key]);
                            }

                            if (!NamespacesDefinidosInv.ContainsKey(namespaces[key]))
                            {
                                NamespacesDefinidosInv.Add(namespaces[key], key);
                            }
                            //TODO: Revisar, pero tener encuenta que ha cambiado la lista lo que era key ahora es value y viceversa:

                            //if (!NamespacesDefinidos.ContainsKey(key))
                            //{
                            //    NamespacesDefinidos.Add(key, namespaces[key]);
                            //}
                            //else if (namespaces[key][0] != '&')
                            //{
                            //    NamespacesDefinidos[key] = namespaces[key];
                            //}
                            //else if (namespaces[key][0] == '&' && !ValorVerdaderNamespacesReferencia.ContainsKey(namespaces[key]))
                            //{
                            //    //Namespace que hace referencia a otro:
                            //     ValorVerdaderNamespacesReferencia.Add(namespaces[key], key);
                            //}
                        }
                    }

                    foreach (string ontologia in listaOntologias)
                    {
                        //Lee todas las entidades y las almacena en mEntidades.
                        this.mEntidades.AddRange(this.GestorOWL.LeerEntidades(ontologia));

                        foreach (ElementoOntologia entidad in mEntidades)
                        {
                            if (!TiposEntidades.Contains(entidad.TipoEntidad))
                            {
                                TiposEntidades.Add(entidad.TipoEntidad);
                            }
                        }

                        //Lee las todas las propiedades y las almacena en la variable local propiedades
                        Propiedades.AddRange(GestorOWL.LeerPropiedades(ontologia));
                    }

                    #endregion
                }
                else
                {
                    //Leo los namespaces:
                    //Leo y añado los namespaces a la ontología:
                    Dictionary<string, string> namespaces = GestionOWL.LeerNamespaces(this, contenidoOntologia);
                    foreach (string key in namespaces.Keys)
                    {
                        if (!NamespacesDefinidos.ContainsKey(key))
                        {
                            NamespacesDefinidos.Add(key, namespaces[key]);
                        }

                        if (!NamespacesDefinidosInv.ContainsKey(namespaces[key]))
                        {
                            NamespacesDefinidosInv.Add(namespaces[key], key);
                        }
                    }

                    GestionOWL.LeerOntologiasImportadas(this, contenidoOntologia);

                    //Desencripta el fichero
                    //this.mFicheroOntologia=new StreamReader( Encriptacion.DesencriptarFichero(this.mRutaOntologia));
                    //Lee todas las entidades y las almacena en mEntidades.
                    this.mEntidades = this.GestorOWL.LeerEntidades(contenidoOntologia);

                    //Ajustamos SuperClases y subClases:
                    AjustarSuperClasesSubClases();

                    foreach (ElementoOntologia entidad in mEntidades)
                    {
                        if (!TiposEntidades.Contains(entidad.TipoEntidad))
                        {
                            TiposEntidades.Add(entidad.TipoEntidad);
                        }
                    }

                    //Lee las todas las propiedades y las almacena en la variable local propiedades
                    Propiedades = GestorOWL.LeerPropiedades(contenidoOntologia);
                }

                //Recorre todas las propiedades y asigna cada una de ellas a todas las clases a las que pertenece.
                foreach (Propiedad propiedad in Propiedades)
                {
                    EstableceDominiosRangosPropiedadesHeredadas(propiedad, Propiedades);

                    foreach (string tipoEntidad in propiedad.Dominio)
                    {
                        if (!tipoEntidad.Equals(string.Empty))
                        {
                            foreach (ElementoOntologia entidad in this.mEntidades)
                            {
                                if (entidad.TipoEntidad.Equals(tipoEntidad))
                                {
                                    entidad.AddPropiedad(propiedad);
                                }
                                else if (entidad.TipoEntidadRelativo.Equals(tipoEntidad)) //Compruebo la entiad con su URL relativa
                                {
                                    entidad.AddPropiedad(propiedad);
                                }
                            }
                        }
                    }
                }
                //Recorre las clases y asigna las propiedades heredadas a cada sub clase
                foreach (ElementoOntologia ent in Entidades)
                {
                    if (ent.Superclases.Count > 0)
                    {
                        ObtenerPropiedadesHeredadas(ent, ent.Superclases);
                    }
                }
            }
        }

        /// <summary>
        /// Ajusta las superClases y subClases de las entidades.
        /// </summary>
        private void AjustarSuperClasesSubClases()
        {
            foreach (ElementoOntologia entidad in mEntidades)
            {
                foreach (string superClase in entidad.SuperclasesUtiles)
                {
                    foreach (ElementoOntologia entidadSuper in mEntidades)
                    {
                        if (entidadSuper.TipoEntidad == superClase)
                        {
                            if (!entidadSuper.Subclases.Contains(entidad.TipoEntidad))
                            {
                                entidadSuper.Subclases.Add(entidad.TipoEntidad);
                            }
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Establece el dominio y el rango de la propiedad según las propiedades de las que hereda.
        /// </summary>
        /// <param name="pPropiedad">Propiedad</param>
        /// <param name="pPropiedades">Lista con todas las propiedades</param>
        private void EstableceDominiosRangosPropiedadesHeredadas(Propiedad pPropiedad, List<Propiedad> pPropiedades)
        {
            if (pPropiedad.SuperPropiedades.Count > 0)
            {
                if (pPropiedad.Rango == "")
                {//No tiene rango. Lo cogemos del padre.
                    pPropiedad.Rango = ExtraerRangoPropiedadPadre(pPropiedad.Nombre, pPropiedades);
                }

                foreach (string dominio in ExtraerDominiosPropiedadPadre(pPropiedad.Nombre, pPropiedades))
                {
                    if (!pPropiedad.Dominio.Contains(dominio))
                    {
                        pPropiedad.Dominio.Add(dominio);
                    }
                }
            }
        }

        /// <summary>
        /// Extrae el rango de una propiedad que hereda de otras (rango de los padres).
        /// </summary>
        /// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        /// <param name="pPropiedades">Lista con todas las propiedades</param>
        /// <returns>Rango de la propiedad heredado o no.</returns>
        private string ExtraerRangoPropiedadPadre(string pNombrePropiedad, List<Propiedad> pPropiedades)
        {
            foreach (Propiedad propiedad in pPropiedades)
            {
                if (propiedad.Nombre == pNombrePropiedad)
                {
                    if (propiedad.Rango != "")
                    {
                        return propiedad.Rango;
                    }
                    else if (propiedad.SuperPropiedades.Count > 0) //Miro en los padres de esta propiedad:
                    {
                        foreach (string nombrePropiedaPadre in propiedad.SuperPropiedades)
                        {
                            string rangoPadre = ExtraerRangoPropiedadPadre(nombrePropiedaPadre, pPropiedades);

                            if (rangoPadre != "")
                            {
                                return rangoPadre;
                            }
                        }
                    }
                    else
                    {
                        return "";
                    }

                    break;
                }
            }

            return "";
        }

        /// <summary>
        /// Extrae los dominios las propiedades de las que hereda la actual. 
        /// </summary>
        /// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        /// <param name="pPropiedades">Lista con todas las propiedades</param>
        /// <returns></returns>
        private List<string> ExtraerDominiosPropiedadPadre(string pNombrePropiedad, List<Propiedad> pPropiedades)
        {
            List<string> dominios = new List<string>();

            foreach (Propiedad propiedad in pPropiedades)
            {
                if (propiedad.Nombre == pNombrePropiedad)
                {
                    dominios.AddRange(propiedad.Dominio);

                    if (propiedad.SuperPropiedades.Count > 0) //Miro en los padres de esta propiedad:
                    {
                        foreach (string nombrePropiedadPadre in propiedad.SuperPropiedades)
                        {
                            dominios.AddRange(ExtraerDominiosPropiedadPadre(nombrePropiedadPadre, pPropiedades));
                        }
                    }

                    break;
                }
            }

            return dominios;
        }

        /// <summary>
        /// Obtiene las propiedades heredadas de la entidad por sus superclases
        /// </summary>
        /// <param name="pEntidad">Entidad de la que se buscan sus propiedades heredadas</param>
        /// <param name="pTipoEntidadesSuperiores">Tipos de las entidades padre</param>
        private void ObtenerPropiedadesHeredadas(ElementoOntologia pEntidad, List<string> pTipoEntidadesSuperiores)
        {
            foreach (string tipoEntidadSuperior in pTipoEntidadesSuperiores)
            {
                ElementoOntologia entidadSuperior = this.GetEntidadTipo(tipoEntidadSuperior);

                if (entidadSuperior.Superclases.Count > 0)
                {
                    //Obtengo las propiedades superiores
                    ObtenerPropiedadesHeredadas(entidadSuperior, entidadSuperior.Superclases);
                }
                foreach (Propiedad propiedad in entidadSuperior.Propiedades)
                {
                    //Asigno las propiedades
                    if (!pEntidad.Propiedades.Contains(propiedad))
                    {
                        pEntidad.Propiedades.Add(GestorOWL.CrearPropiedad(propiedad));
                        pEntidad.Propiedades[pEntidad.Propiedades.Count - 1].Heredada = true;
                    }
                }

                foreach (Restriccion restriccion in entidadSuperior.Restricciones)
                {
                    foreach (Propiedad propiedad in pEntidad.Propiedades)
                    {
                        if (restriccion.Propiedad == propiedad.Nombre)
                        {
                            Restriccion restriccionHija = new Restriccion(restriccion);
                            pEntidad.AddRestriccion(restriccion);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Devuelve la entidad del tipo pTipoEntidad
        /// </summary>
        /// <param name="pTipoEntidad">Tipo de la entidad buscada</param>
        /// <returns>La entidad del tipo buscado</returns>
        public virtual ElementoOntologia GetEntidadTipo(string pTipoEntidad)
        {
            return GetEntidadTipo(pTipoEntidad, true);
        }

        /// <summary>
        /// Devuelve la entidad del tipo pTipoEntidad
        /// </summary>
        /// <param name="pTipoEntidad">Tipo de la entidad buscada</param>
        /// <param name="pCrearNuevaEntidad">True si debe crearse una nueva entidad, false si debe devolverse una existente</param>
        /// <returns>La entidad del tipo buscado</returns>
        public ElementoOntologia GetEntidadTipo(string pTipoEntidad, bool pCrearNuevaEntidad)
        {
            bool encontrado = false;
            int i = 0;
            ElementoOntologia entidad = null;

            //recorro todas las entdidades
            while ((!encontrado) && (i < this.Entidades.Count))
            {
                if (Entidades[i].TipoEntidad.Equals(pTipoEntidad) || Entidades[i].TipoEntidad.Equals(GetTipoEntidadSinNamespaces(pTipoEntidad)))
                {
                    encontrado = true;
                    entidad = this.Entidades[i];
                }
                i++;
            }

            if (entidad != null)
            {
                if (pCrearNuevaEntidad)
                    return GestorOWL.CrearElementoOntologia(entidad);
                return entidad;
            }
            else
                return GestorOWL.CrearElementoOntologia(pTipoEntidad, mGestionOwl.UrlOntologia, mGestionOwl.NamespaceOntologia);
        }

        /// <summary>
        /// Devuelve el tipo de entidad sin namespaces de la entidad pasada como parámetro.
        /// </summary>
        /// <param name="pTipoEntidad">Tipo de entidad</param>
        /// <returns>Tipo de entidad sin namespaces de la entidad pasada como parámetro</returns>
        public string GetTipoEntidadSinNamespaces(string pTipoEntidad)
        {
            if (!pTipoEntidad.Contains("://") && pTipoEntidad.Contains(":"))
            {
                string namesp = pTipoEntidad.Substring(0, pTipoEntidad.IndexOf(":"));

                if (this.GestorOWL.NamespaceOntologia == namesp)
                {
                    string tipoEnt = pTipoEntidad.Substring(pTipoEntidad.IndexOf(":") + 1);
                    return this.GestorOWL.UrlOntologia + tipoEnt;
                }
                else if (NamespacesDefinidosInv.ContainsKey(namesp))
                {
                    string tipoEnt = pTipoEntidad.Substring(pTipoEntidad.IndexOf(":") + 1);
                    return NamespacesDefinidosInv[namesp] + tipoEnt;
                }
            }

            return pTipoEntidad;
        }

        /// <summary>
        /// Obtiene la entidad superior la cual contiene jerárquicamente a todas las demás y no es contenida por ninguna.
        /// </summary>
        public List<ElementoOntologia> ObtenerElementosContenedorSuperior()
        {
            return GestionOWL.ObtenerElementosContenedorSuperior(Entidades);
        }

        /// <summary>
        /// Obtiene una entidad a partir de su identificador.
        /// </summary>
        /// <param name="pEntidadID">Identificador de la entidad</param>
        /// <returns>La entidad que tiene un identificador</returns>
        public ElementoOntologia ObtenerEntidadPorID(string pEntidadID)
        {
            foreach (ElementoOntologia entidad in Entidades)
            {
                if (entidad.ID == pEntidadID)
                {
                    return entidad;
                }
            }

            return null;
        }

        /// <summary>
        /// Genara un namespace para una Url.
        /// </summary>
        /// <param name="pUrl">Url</param>
        public void GenararNamespaceUrl(string pUrl)
        {
            string namespaceAux = pUrl;

            if (namespaceAux[namespaceAux.Length - 1] == '#' || namespaceAux[namespaceAux.Length - 1] == '/')
            {
                namespaceAux = namespaceAux.Substring(0, namespaceAux.Length - 1);
            }

            if (namespaceAux.Contains("/"))
            {
                namespaceAux = namespaceAux.Substring(namespaceAux.LastIndexOf("/") + 1);
            }

            if (namespaceAux.Length > 10)
            {
                namespaceAux = namespaceAux.Substring(0, 10);
            }

            namespaceAux = "aux" + namespaceAux;
            int count = 1;

            while (NamespacesDefinidosInv.ContainsKey(namespaceAux + count))
            {
                count++;
            }

            NamespacesDefinidos.Add(pUrl, namespaceAux + count);
            NamespacesDefinidosInv.Add(namespaceAux + count, pUrl);
        }

        #endregion

        #endregion

        #region Dispose


        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~Ontologia()
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
            if (!disposed)
            {
                disposed = true;
                if (disposing)
                {
                    //Libero todos los recursos administrados que he añadido a esta clase
                    if (mEntidades != null)
                    {
                        foreach (ElementoOntologia entidad in mEntidades)
                        {
                            entidad.Dispose();
                        }
                        mEntidades.Clear();
                    }
                }

                mEntidades = null;
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
            info.AddValue("Entidades", mEntidades);
            info.AddValue("TiposEntidades", mTiposEntidades);
            info.AddValue("NamespacesDefinidos", mNamespacesDefinidos);
            info.AddValue("RDFCVSemIncluido", mRDFCVSemIncluido);
            info.AddValue("RutasOntologias", mRutasOntologias);
            info.AddValue("UsoIDsRelativos", mUsoIDsRelativos);
            info.AddValue("UrlOntologiasImportadas", mUrlOntologiasImportadas);
            info.AddValue("ValorVerdaderNamespacesReferencia", mValorVerdaderNamespacesReferencia);
            
            info.AddValue("GestionOwl", mGestionOwl);
            info.AddValue("ConfiguracionPlantilla", mConfiguracionPlantilla);
            info.AddValue("EstilosPlantilla", mEstilosPlantilla);
            info.AddValue("IdiomaUsuario", mIdiomaUsuario);
            info.AddValue("OntologiaID", mOntologiaID);
            info.AddValue("NamespacesDefinidosInv", mNamespacesDefinidosInv);
        }

        #endregion

        #region Miembros de ICloneable

        public object Clone()
        {
            Ontologia ontologiaClonada = new Ontologia();

            ontologiaClonada.Entidades = mEntidades;
            ontologiaClonada.TiposEntidades = mTiposEntidades;
            ontologiaClonada.GestorOWL = mGestionOwl;
            ontologiaClonada.NamespacesDefinidos = mNamespacesDefinidos;
            ontologiaClonada.NamespacesDefinidosInv = mNamespacesDefinidosInv;
            ontologiaClonada.NamespacesDefinidosExtra = mNamespacesDefinidosExtra;
            ontologiaClonada.ValorVerdaderNamespacesReferencia = mValorVerdaderNamespacesReferencia;
            ontologiaClonada.RDFCVSemIncluido = mRDFCVSemIncluido;
            ontologiaClonada.UsoIDsRelativos = mUsoIDsRelativos;
            ontologiaClonada.UrlOntologiasImportadas = mUrlOntologiasImportadas;
            ontologiaClonada.EstilosPlantilla = mEstilosPlantilla;
            //ontologiaClonada.ConfiguracionPlantilla = mConfiguracionPlantilla; NO Hacer falta
            ontologiaClonada.GenararNamespacesHuerfanos = mGenararNamespacesHuerfanos;
            ontologiaClonada.IdiomaUsuario = mIdiomaUsuario;
            ontologiaClonada.OntologiaID = mOntologiaID;

            return ontologiaClonada;
        }

        #endregion
    }
}
