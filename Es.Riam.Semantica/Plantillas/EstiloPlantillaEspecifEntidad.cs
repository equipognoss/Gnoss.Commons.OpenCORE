using System;
using System.Collections.Generic;
using Es.Riam.Semantica.OWL;
using System.Runtime.Serialization;
using System.Linq;

namespace Es.Riam.Semantica.Plantillas
{
    #region Enumeraciones

    /// <summary>
    /// Tipo de representación de las entidades.
    /// </summary>
    public enum TipoRepresentacion
    {
        /// <summary>
        /// TipoEntidadMasID.
        /// </summary>
        TipoEntidadMasID = 0,
        /// <summary>
        /// SoloID.
        /// </summary>
        SoloID = 1,
        /// <summary>
        /// TodosLosCaracteres.
        /// </summary>
        TodosLosCaracteres = 2,
        /// <summary>
        /// NumCaracteresExactos
        /// </summary>
        NumCaracteresExactos = 3

    }

    #endregion

    /// <summary>
    /// Clase para gestionar los estilos de un elemento de la plantilla.
    /// </summary>
    [Serializable]
    public class EstiloPlantillaEspecifEntidad : EstiloPlantilla, ISerializable
    {
        #region Miembros

        /// <summary>
        /// Entidad.
        /// </summary>
        private ElementoOntologia mEntidad;

        /// <summary>
        /// Lista elementos ordenados.
        /// </summary>
        private List<ElementoOrdenado> mElementosOrdenados;

        /// <summary>
        /// Lista elementos ordenados para la lectura.
        /// </summary>
        private List<ElementoOrdenado> mElementosOrdenadosLectura;

        /// <summary>
        /// Lista elementos ordenados.
        /// </summary>
        private Dictionary<string, List<ElementoOrdenado>> mElementosOrdenadosPorCondicion;

        /// <summary>
        /// Lista elementos ordenados para la lectura.
        /// </summary>
        private Dictionary<string, List<ElementoOrdenado>> mElementosOrdenadosLecturaPorCondicion;

        /// <summary>
        /// Atributos representantes.
        /// </summary>
        private List<Representante> mAtrRepresentantes;

        /// <summary>
        /// Representantes de la entidad.
        /// </summary>
        private List<Representante> mRepresentantes;

        /// <summary>
        /// Clase CSS para el panel de la entidad.
        /// </summary>
        private string mClaseCssPanel;

        /// <summary>
        /// Clase CSS para el título de la entidad.
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
        /// Nombre de la entidad en edición.
        /// </summary>
        private Dictionary<string,string> mAtrNombre;

        /// <summary>
        /// Nombre de la entidad en lectura.
        /// </summary>
        private Dictionary<string, string> mAtrNombreLectura;

        /// <summary>
        /// Nombre de la entidad en edición.
        /// </summary>
        private string mNombre;

        /// <summary>
        /// Nombre de la entidad en lectura.
        /// </summary>
        private string mNombreLectura;

        /// <summary>
        /// Texto del link editar despliegue.
        /// </summary>
        private string mTextoLinkEditarDespliegue;

        /// <summary>
        /// Propiedad que vincula la entidad actual con alguna hija.
        /// </summary>
        private string mPropiedadVinculanteConEntidadHija;

        /// <summary>
        /// Indica si el div que contiene la entidad es desplegable.
        /// </summary>
        private bool mDivEntidadDesplegable;

        /// <summary>
        /// Valor de microdatos.
        /// </summary>
        private string mMicrodatos;

        /// <summary>
        /// Valor de microformatos.
        /// </summary>
        private Dictionary<string, string> mMicroformatos;

        /// <summary>
        /// Pinta un mapa.
        /// </summary>
        private bool mEsMapaGoogle;

        /// <summary>
        /// Propiedades con la latitud y longitud para el mapa.
        /// </summary>
        private KeyValuePair<string,string> mPropiedadesDatosMapa;

        /// <summary>
        /// Propiedad de la ruta de los mapas.
        /// </summary>
        private KeyValuePair<string, string> mPropiedadesDatosMapaRuta;

        /// <summary>
        /// Indica si hay que sustituir la entidad en el mapa de google.
        /// </summary>
        private bool mNoSustituirEntidadEnMapaGoogle;

        /// <summary>
        /// Campo por el que se debe ordenar la entidad.
        /// </summary>
        private string mCampoOrden;

        /// <summary>
        /// Campo en el que se debe pintar el orden que tiene la entidad.
        /// </summary>
        private string mCampoRepresentanteOrden;

        /// <summary>
        /// Propiedades con sus valores según los cuales debe o no pintarse una entidad.
        /// </summary>
        private Dictionary<string, KeyValuePair<bool,List<string>>> mPropsCondicionPintarEntSegunValores;

        /// <summary>
        /// Propiedad que definie la privacidad de algún nodo
        /// </summary>
        private List<Guid> mPrivadoParaGrupoEditores;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor sin parámetros.
        /// </summary>
        public EstiloPlantillaEspecifEntidad()
        {
        }

        /// <summary>
        /// Constructor a partir de otro objeto.
        /// </summary>
        /// <param name="pEstiloRef">Objeto de referencia</param>
        public EstiloPlantillaEspecifEntidad(EstiloPlantillaEspecifEntidad pEstiloRef)
        {
            Entidad = pEstiloRef.Entidad;

            foreach (ElementoOrdenado elemOrd in pEstiloRef.ElementosOrdenados)
            {
                ElementosOrdenados.Add(new ElementoOrdenado(elemOrd));
            }

            foreach (ElementoOrdenado elemOrd in pEstiloRef.ElementosOrdenadosLectura)
            {
                ElementosOrdenadosLectura.Add(new ElementoOrdenado(elemOrd));
            }

            if (pEstiloRef.ElementosOrdenadosPorCondicion != null)
            {
                ElementosOrdenadosPorCondicion = new Dictionary<string, List<ElementoOrdenado>>();

                foreach (string condicion in pEstiloRef.ElementosOrdenadosPorCondicion.Keys)
                {
                    if (!ElementosOrdenadosPorCondicion.ContainsKey(condicion))
                    {
                        ElementosOrdenadosPorCondicion.Add(condicion, new List<ElementoOrdenado>());

                        foreach (ElementoOrdenado elemOrd in pEstiloRef.ElementosOrdenadosPorCondicion[condicion])
                        {
                            ElementosOrdenadosPorCondicion[condicion].Add(new ElementoOrdenado(elemOrd));
                        }
                    }
                }
            }

            if (pEstiloRef.ElementosOrdenadosLecturaPorCondicion != null)
            {
                ElementosOrdenadosLecturaPorCondicion = new Dictionary<string, List<ElementoOrdenado>>();

                foreach (string condicion in pEstiloRef.ElementosOrdenadosLecturaPorCondicion.Keys)
                {
                    if (!ElementosOrdenadosLecturaPorCondicion.ContainsKey(condicion))
                    {
                        ElementosOrdenadosLecturaPorCondicion.Add(condicion, new List<ElementoOrdenado>());

                        foreach (ElementoOrdenado elemOrd in pEstiloRef.ElementosOrdenadosLecturaPorCondicion[condicion])
                        {
                            ElementosOrdenadosLecturaPorCondicion[condicion].Add(new ElementoOrdenado(elemOrd));
                        }
                    }
                }
            }

            //PropiedadesOrdenadas;
            //PropiedadesOrdenadasLectura
            AtrRepresentantes = pEstiloRef.AtrRepresentantes;
            //Representantes;
            ClaseCssPanel = pEstiloRef.ClaseCssPanel;
            ClaseCssPanelTitulo = pEstiloRef.ClaseCssPanelTitulo;
            TagNameTituloEdicion = pEstiloRef.TagNameTituloEdicion;
            TagNameTituloLectura = pEstiloRef.TagNameTituloLectura;
            AtrNombre = pEstiloRef.AtrNombre;
            AtrNombreLectura = pEstiloRef.AtrNombreLectura;
            //Nombre;
            //NombreLectura;
            TextoLinkEditarDespliegue = pEstiloRef.TextoLinkEditarDespliegue;
            PropiedadVinculanteConEntidadHija = pEstiloRef.PropiedadVinculanteConEntidadHija;
            DivEntidadDesplegable = pEstiloRef.DivEntidadDesplegable;
            Microdatos = pEstiloRef.Microdatos;
            Microformatos = pEstiloRef.Microformatos;
            EsMapaGoogle = pEstiloRef.EsMapaGoogle;
            PropiedadesDatosMapa = pEstiloRef.PropiedadesDatosMapa;
            PropiedadesDatosMapaRuta = pEstiloRef.PropiedadesDatosMapaRuta;
            NoSustituirEntidadEnMapaGoogle = pEstiloRef.NoSustituirEntidadEnMapaGoogle;
            CampoOrden = pEstiloRef.CampoOrden;
            CampoRepresentanteOrden = pEstiloRef.CampoRepresentanteOrden;
            PropsCondicionPintarEntSegunValores = pEstiloRef.PropsCondicionPintarEntSegunValores;
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected EstiloPlantillaEspecifEntidad(SerializationInfo info, StreamingContext context)
        {
            mAtrNombre = (Dictionary<string, string>)info.GetValue("AtrNombre", typeof(Dictionary<string, string>));
            mAtrNombreLectura = (Dictionary<string, string>)info.GetValue("AtrNombreLectura", typeof(Dictionary<string, string>));
            mAtrRepresentantes = (List<Representante>)info.GetValue("AtrRepresentantes", typeof(List<Representante>));
            mCampoOrden = (string)info.GetValue("CampoOrden", typeof(string));
            mCampoRepresentanteOrden = (string)info.GetValue("CampoRepresentanteOrden", typeof(string));
            mClaseCssPanel = (string)info.GetValue("ClaseCssPanel", typeof(string));
            mClaseCssPanelTitulo = (string)info.GetValue("ClaseCssPanelTitulo", typeof(string));
            mDivEntidadDesplegable = (bool)info.GetValue("DivEntidadDesplegable", typeof(bool));
            mElementosOrdenados = (List<ElementoOrdenado>)info.GetValue("ElementosOrdenados", typeof(List<ElementoOrdenado>));
            mElementosOrdenadosLectura = (List<ElementoOrdenado>)info.GetValue("ElementosOrdenadosLectura", typeof(List<ElementoOrdenado>));
            mElementosOrdenadosLecturaPorCondicion = (Dictionary<string, List<ElementoOrdenado>>)info.GetValue("ElementosOrdenadosLecturaPorCondicion", typeof(Dictionary<string, List<ElementoOrdenado>>));
            mElementosOrdenadosPorCondicion = (Dictionary<string, List<ElementoOrdenado>>)info.GetValue("ElementosOrdenadosPorCondicion", typeof(Dictionary<string, List<ElementoOrdenado>>));
            mEsMapaGoogle = (bool)info.GetValue("EsMapaGoogle", typeof(bool));
            mMicrodatos = (string)info.GetValue("Microdatos", typeof(string));
            mMicroformatos = (Dictionary<string, string>)info.GetValue("Microformatos", typeof(Dictionary<string, string>));
            mNoSustituirEntidadEnMapaGoogle = (bool)info.GetValue("NoSustituirEntidadEnMapaGoogle", typeof(bool));
            mPrivadoParaGrupoEditores = (List<Guid>)info.GetValue("PrivadoParaGrupoEditores", typeof(List<Guid>));
            mPropiedadesDatosMapa = (KeyValuePair<string, string>)info.GetValue("PropiedadesDatosMapa", typeof(KeyValuePair<string, string>));
            mPropiedadVinculanteConEntidadHija = (string)info.GetValue("PropiedadVinculanteConEntidadHija", typeof(string));
            mPropsCondicionPintarEntSegunValores = (Dictionary<string, KeyValuePair<bool, List<string>>>)info.GetValue("PropsCondicionPintarEntSegunValores", typeof(Dictionary<string, KeyValuePair<bool, List<string>>>));
            mRepresentantes = (List<Representante>)info.GetValue("Representantes", typeof(List<Representante>));
            mTagNameTituloEdicion = (string)info.GetValue("TagNameTituloEdicion", typeof(string));
            mTagNameTituloLectura = (string)info.GetValue("TagNameTituloLectura", typeof(string));
            mTextoLinkEditarDespliegue = (string)info.GetValue("TextoLinkEditarDespliegue", typeof(string));
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Entidad.
        /// </summary>
        public ElementoOntologia Entidad
        {
            get
            {
                return mEntidad;
            }
            set
            {
                mEntidad = value;
            }
        }

        /// <summary>
        /// Lista elementos ordenados.
        /// </summary>
        public List<ElementoOrdenado> ElementosOrdenados
        {
            get
            {
                if (mElementosOrdenados == null)
                {
                    mElementosOrdenados = new List<ElementoOrdenado>();
                }

                return mElementosOrdenados;
            }
            set
            {
                mElementosOrdenados = value;
            }
        }

        /// <summary>
        /// Lista elementos ordenados para la lectura.
        /// </summary>
        public List<ElementoOrdenado> ElementosOrdenadosLectura
        {
            get
            {
                if (mElementosOrdenadosLectura == null)
                {
                    mElementosOrdenadosLectura = new List<ElementoOrdenado>();
                }

                return mElementosOrdenadosLectura;
            }
            set
            {
                mElementosOrdenadosLectura = value;
            }
        }

        /// <summary>
        /// Lista elementos ordenados.
        /// </summary>
        public Dictionary<string, List<ElementoOrdenado>> ElementosOrdenadosPorCondicion
        {
            get
            {
                return mElementosOrdenadosPorCondicion;
            }
            set
            {
                mElementosOrdenadosPorCondicion = value;
            }
        }

        /// <summary>
        /// Lista elementos ordenados para la lectura.
        /// </summary>
        public Dictionary<string, List<ElementoOrdenado>> ElementosOrdenadosLecturaPorCondicion
        {
            get
            {
                return mElementosOrdenadosLecturaPorCondicion;
            }
            set
            {
                mElementosOrdenadosLecturaPorCondicion = value;
            }
        }

        /// <summary>
        /// Atribrutos representantes de la entidad.
        /// </summary>
        public List<Representante> AtrRepresentantes
        {
            get
            {
                if (mAtrRepresentantes == null)
                {
                    mAtrRepresentantes = new List<Representante>();
                }

                return mAtrRepresentantes;
            }
            set
            {
                mAtrRepresentantes = value;
            }
        }

        /// <summary>
        /// Representantes de la entidad.
        /// </summary>
        public List<Representante> Representantes
        {
            get
            {
                if (mRepresentantes == null)
                {
                    mRepresentantes = new List<Representante>();

                    foreach (Representante repreAtr in AtrRepresentantes)
                    {
                        Representante representante = new Representante();
                        representante.TipoRepres = repreAtr.TipoRepres;

                        if (Entidad.Superclases.Count > 0 && repreAtr.NombrePropiedad.Trim() == Entidad.Superclases[0]) //Hay que agregar el tipo de entidad
                        {
                            representante.Propiedad = new PropiedadPlantilla(Entidad.Superclases[0], Entidad.Ontologia);
                        }
                        else
                        {
                            if (repreAtr.NombrePropiedad[0] != '*' && repreAtr.NombrePropiedad[0] != '#')
                            {
                                Propiedad propiedad = Entidad.Propiedades.Where(item => item.Nombre.Equals(repreAtr.NombrePropiedad)).FirstOrDefault();
                                if(propiedad != null)
                                {
                                    representante.Propiedad = propiedad;
                                    propiedad.TipoEntidadRepresenta = Entidad.TipoEntidad;
                                }
                                else
                                {
                                    throw new Exception($"La entidad {Entidad} no contiene la propiedad {propiedad.Nombre}. Revisa la sección config/EspefEntidad/Entidad/Representantes de esta entidad en el XML de la ontología.");                                    
                                }
                            }
                            else if (repreAtr.NombrePropiedad[0] == '*')
                            {
                                //Hay que buscar en las propiedades de las entidades de los rangos de las propiedades la entidad:
                                Propiedad propiedad = ObtenerPropiedadACualquierNivelPorNombre(repreAtr.NombrePropiedad.Substring(1), Entidad);
                                if (propiedad != null)
                                {
                                    representante.Propiedad = propiedad;
                                    propiedad.TipoEntidadRepresenta = Entidad.TipoEntidad;
                                }
                            }
                            //else if (nombrePropiedad[0] == '#' && pEntidadVinculada != null)
                            //{
                            //    if (nombrePropiedad[1] != '*')
                            //    {
                            //        foreach (Propiedad propiedad in pEntidadVinculada.Propiedades)
                            //        {
                            //            if (propiedad.Nombre == nombrePropiedad.Substring(1))
                            //            {
                            //                listaPropiedadesRepr.Add(propiedad, codigoRepresentacion);
                            //                propiedad.TipoEntidadRepresenta = pEntidadVinculada.TipoEntidad;
                            //                break;
                            //            }
                            //        }
                            //    }
                            //    else
                            //    {
                            //        //Hay que buscar en las propiedades de la entidad vinculada:
                            //        Propiedad propiedad = ObtenerPropiedadACualquierNivelPorNombre(nombrePropiedad.Substring(2), pEntidadVinculada);
                            //        if (propiedad != null)
                            //        {
                            //            listaPropiedadesRepr.Add(propiedad, codigoRepresentacion);
                            //            propiedad.TipoEntidadRepresenta = pEntidadVinculada.TipoEntidad;
                            //        }
                            //    }
                            //}
                        }

                        mRepresentantes.Add(representante);
                    }
                }

                return mRepresentantes;
            }
            set
            {
                mRepresentantes = value;
            }
        }

        /// <summary>
        /// Clase CSS para el panel de la entidad.
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
        /// Clase CSS para el título de la entidad.
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
        /// Nombre de la entidad en edición.
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

        public bool PermitirScript
        {
            get; set;
        }

        /// <summary>
        /// Nombre de la entidad en lectura.
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
        /// Nombre de la entidad en edición.
        /// </summary>
        public string Nombre
        {
            get
            {
                if (mNombre == null)
                {
                    if (AtrNombre.ContainsKey(Entidad.Ontologia.IdiomaUsuario))
                    {
                        mNombre = AtrNombre[Entidad.Ontologia.IdiomaUsuario];
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
        }

        /// <summary>
        /// Nombre de la entidad en edición.
        /// </summary>
        public string NombreLectura
        {
            get
            {
                if (mNombreLectura == null)
                {
                    if (AtrNombreLectura.ContainsKey(Entidad.Ontologia.IdiomaUsuario))
                    {
                        mNombreLectura = AtrNombreLectura[Entidad.Ontologia.IdiomaUsuario];
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
        /// Texto del link editar despliegue.
        /// </summary>
        public string TextoLinkEditarDespliegue
        {
            get
            {
                return mTextoLinkEditarDespliegue;
            }
            set
            {
                mTextoLinkEditarDespliegue = value;
            }
        }

        /// <summary>
        /// Propiedad que vincula la entidad actual con alguna hija.
        /// </summary>
        public string PropiedadVinculanteConEntidadHija
        {
            get
            {
                return mPropiedadVinculanteConEntidadHija;
            }
            set
            {
                mPropiedadVinculanteConEntidadHija = value;
            }
        }

        /// <summary>
        /// Indica si el div que contiene la entidad es desplegable.
        /// </summary>
        public bool DivEntidadDesplegable
        {
            get
            {
                return mDivEntidadDesplegable;
            }
            set
            {
                mDivEntidadDesplegable = value;
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
        /// Pinta un mapa.
        /// </summary>
        public bool EsMapaGoogle
        {
            get
            {
                return mEsMapaGoogle;
            }
            set
            {
                mEsMapaGoogle = value;
            }
        }

        /// <summary>
        /// Propiedades con la latitud y longitud para el mapa.
        /// </summary>
        public KeyValuePair<string, string> PropiedadesDatosMapa
        {
            get
            {
                return mPropiedadesDatosMapa;
            }
            set
            {
                mPropiedadesDatosMapa = value;
            }
        }


        /// <summary>
        /// Propiedad de la ruta de los mapas y su color.
        /// </summary>
        public KeyValuePair<string, string> PropiedadesDatosMapaRuta
        {
            get
            {
                return mPropiedadesDatosMapaRuta;
            }
            set
            {
                mPropiedadesDatosMapaRuta = value;
            }
        }

        /// <summary>
        /// Indica si hay que sustituir la entidad en el mapa de google.
        /// </summary>
        public bool NoSustituirEntidadEnMapaGoogle
        {
            get
            {
                return mNoSustituirEntidadEnMapaGoogle;
            }
            set
            {
                mNoSustituirEntidadEnMapaGoogle = value;
            }
        }

        /// <summary>
        /// Campo por el que se debe ordenar la entidad.
        /// </summary>
        public string CampoOrden
        {
            get
            {
                return mCampoOrden;
            }
            set
            {
                mCampoOrden = value;
            }
        }

        /// <summary>
        /// Campo en el que se debe pintar el orden que tiene la entidad.
        /// </summary>
        public string CampoRepresentanteOrden
        {
            get
            {
                return mCampoRepresentanteOrden;
            }
            set
            {
                mCampoRepresentanteOrden = value;
            }
        }

        /// <summary>
        /// Propiedades con sus valores según los cuales debe o no pintarse una entidad.
        /// </summary>
        public Dictionary<string, KeyValuePair<bool, List<string>>> PropsCondicionPintarEntSegunValores
        {
            get
            {
                if (mPropsCondicionPintarEntSegunValores == null)
                {
                    mPropsCondicionPintarEntSegunValores = new Dictionary<string, KeyValuePair<bool, List<string>>>();
                }

                return mPropsCondicionPintarEntSegunValores;
            }
            set
            {
                mPropsCondicionPintarEntSegunValores = value;
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
        
        #endregion

        #region Métodos

        /// <summary>
        /// Devuelve el nombre de la entidad.
        /// </summary>
        /// <param name="pVistaPrevia">Indica si estamos en vista previa o no</param>
        /// <returns>nombre de la entidad</returns>
        public string NombreEntidad(bool pVistaPrevia)
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
        /// Da valor a las propiedades de los elementos ordenados.
        /// </summary>
        /// <param name="pElementos">Elementos ordenados</param>
        public static void DarValorPropiedadAElementosOrdenados(List<ElementoOrdenado> pElementos, ElementoOntologia pEntidad)
        {
            foreach (ElementoOrdenado elementoOrd in pElementos)
            {
                if (elementoOrd.EsGrupo)
                {
                    DarValorPropiedadAElementosOrdenados(elementoOrd.Hijos, pEntidad);
                }
                else if (elementoOrd.PropDeEntHija != null)
                {
                    elementoOrd.NombrePropiedad = new KeyValuePair<string, Propiedad>(elementoOrd.NombrePropiedad.Key, EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(elementoOrd.NombrePropiedad.Key, pEntidad));
                }
                else if (!elementoOrd.EsLiteral && !elementoOrd.EsSelectorGrupo && !elementoOrd.EsEspecial)
                {
                    foreach (Propiedad propiedad in pEntidad.Propiedades)
                    {
                        if (elementoOrd.NombrePropiedad.Key == propiedad.Nombre)
                        {
                            elementoOrd.NombrePropiedad = new KeyValuePair<string, Propiedad>(propiedad.Nombre, propiedad);
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Destructor
        /// </summary>
        ~EstiloPlantillaEspecifEntidad()
        {
            //Libero los recursos
            Dispose(false,false);
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
                if (disposing)
                {
                    //Libero todos los recursos administrados que he añadido a esta clase

                    if (mElementosOrdenados != null)
                    {
                        foreach (ElementoOrdenado elemOrd in mElementosOrdenados)
                        {
                            elemOrd.Dispose();
                        }

                        mElementosOrdenados.Clear();
                    }

                    if (mElementosOrdenadosLectura != null)
                    {
                        foreach (ElementoOrdenado elemOrd in mElementosOrdenadosLectura)
                        {
                            elemOrd.Dispose();
                        }

                        mElementosOrdenadosLectura.Clear();
                    }

                    if (mElementosOrdenadosPorCondicion != null)
                    {
                        foreach (string key in mElementosOrdenadosPorCondicion.Keys)
                        {
                            foreach (ElementoOrdenado elemOrd in mElementosOrdenadosPorCondicion[key])
                            {
                                elemOrd.Dispose();
                            }
                        }

                        mElementosOrdenadosPorCondicion.Clear();
                    }

                    if (mElementosOrdenadosLecturaPorCondicion != null)
                    {
                        foreach (string key in mElementosOrdenadosLecturaPorCondicion.Keys)
                        {
                            foreach (ElementoOrdenado elemOrd in mElementosOrdenadosLecturaPorCondicion[key])
                            {
                                elemOrd.Dispose();
                            }
                        }

                        mElementosOrdenadosLecturaPorCondicion.Clear();
                    }

                    if (mPrivadoParaGrupoEditores != null)
                    {
                        mPrivadoParaGrupoEditores.Clear();
                    }
                }

                mElementosOrdenados = null;
                mElementosOrdenadosLectura = null;
                mElementosOrdenadosPorCondicion = null;
                mElementosOrdenadosLecturaPorCondicion = null;
                mEntidad = null;
                mPrivadoParaGrupoEditores = null;
                mAtrNombre = null;
                mAtrNombreLectura = null;
                mAtrRepresentantes = null;
                mMicroformatos = null;
                mPropsCondicionPintarEntSegunValores = null;
                mRepresentantes = null;
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
            info.AddValue("AtrRepresentantes", mAtrRepresentantes);
            info.AddValue("CampoOrden", mCampoOrden);
            info.AddValue("CampoRepresentanteOrden", mCampoRepresentanteOrden);
            info.AddValue("ClaseCssPanel", mClaseCssPanel);
            info.AddValue("ClaseCssPanelTitulo", mClaseCssPanelTitulo);
            info.AddValue("DivEntidadDesplegable", mDivEntidadDesplegable);
            info.AddValue("ElementosOrdenados", mElementosOrdenados);
            info.AddValue("ElementosOrdenadosLectura", mElementosOrdenadosLectura);
            info.AddValue("ElementosOrdenadosLecturaPorCondicion", mElementosOrdenadosLecturaPorCondicion);
            info.AddValue("ElementosOrdenadosPorCondicion", mElementosOrdenadosPorCondicion);
            info.AddValue("EsMapaGoogle", mEsMapaGoogle);
            info.AddValue("Microdatos", mMicrodatos);
            info.AddValue("Microformatos", mMicroformatos);
            info.AddValue("NoSustituirEntidadEnMapaGoogle", mNoSustituirEntidadEnMapaGoogle);
            info.AddValue("PrivadoParaGrupoEditores", mPrivadoParaGrupoEditores);
            info.AddValue("PropiedadesDatosMapa", mPropiedadesDatosMapa);
            info.AddValue("PropiedadVinculanteConEntidadHija", mPropiedadVinculanteConEntidadHija);
            info.AddValue("PropsCondicionPintarEntSegunValores", mPropsCondicionPintarEntSegunValores);
            info.AddValue("Representantes", mRepresentantes);
            info.AddValue("TagNameTituloEdicion", mTagNameTituloEdicion);
            info.AddValue("TagNameTituloLectura", mTagNameTituloLectura);
            info.AddValue("TextoLinkEditarDespliegue", mTextoLinkEditarDespliegue);
        }

        #endregion
    }

    /// <summary>
    /// Representante entidad.
    /// </summary>
    /// 
    [Serializable]
    public class Representante
    {
        #region Miembros

        /// <summary>
        /// Propiedad.
        /// </summary>
        private Propiedad mPropiedad;

        /// <summary>
        /// Nombre de la Propiedad.
        /// </summary>
        private string mNombrePropiedad;

        /// <summary>
        /// Tipo de representación de las entidades.
        /// </summary>
        private TipoRepresentacion mTipoRepres;

        /// <summary>
        /// Número de caracteres del recorte.
        /// </summary>
        private int mNumCaracteres;

        #endregion

        #region Propiedades

        /// <summary>
        /// Propiedad.
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
        /// Nombre de la Propiedad.
        /// </summary>
        public string NombrePropiedad
        {
            get
            {
                return mNombrePropiedad;
            }
            set
            {
                mNombrePropiedad = value;
            }
        }

        /// <summary>
        /// Tipo de representación de las entidades.
        /// </summary>
        public TipoRepresentacion TipoRepres
        {
            get
            {
                return mTipoRepres;
            }
            set
            {
                mTipoRepres = value;
            }
        }

        /// <summary>
        /// Número de caracteres del recorte.
        /// </summary>
        public int NumCaracteres
        {
            get
            {
                return mNumCaracteres;
            }
            set
            {
                mNumCaracteres = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// Elemento propiedad o grupo de propiedades ordenado.
    /// </summary>
    /// 
    [Serializable]
    public class ElementoOrdenado : IDisposable
    {
        #region Miembros

        /// <summary>
        /// Indica si es grupo o no.
        /// </summary>
        private bool mEsGrupo;

        /// <summary>
        /// Nombre de las propiedades del elemento.
        /// </summary>
        private KeyValuePair<string, Propiedad> mNombrePropiedad;

        /// <summary>
        /// Clase del grupo.
        /// </summary>
        private string mClaseGrupo;

        /// <summary>
        /// ID para el grupo.
        /// </summary>
        private string mIdGrupo;

        /// <summary>
        /// Clase del grupo para la lectura.
        /// </summary>
        private string mClaseGrupoLectura;

        /// <summary>
        /// ID para el grupo para la lectura.
        /// </summary>
        private string mIdGrupoLectura;

        /// <summary>
        /// Propiedad de entidad hija (tipo entidad hija).
        /// </summary>
        private string mPropDeEntHija;

        /// <summary>
        /// Indica que solo debe pintarse el 1º valor de la propiedad.
        /// </summary>
        private bool mSoloPrimerValor;

        /// <summary>
        /// Hijo del grupo
        /// </summary>
        private List<ElementoOrdenado> mHijos;

        /// <summary>
        /// Tipo de grupo.
        /// </summary>
        private string mTipoGrupo;

        /// <summary>
        /// Indica si es literal.
        /// </summary>
        private bool mEsLiteral;

        /// <summary>
        /// Indica si es literal es importante y debe pintarse siempre.
        /// </summary>
        private bool mLiteralImportante;

        /// <summary>
        /// Indica si es un especial.
        /// </summary>
        private bool mEsEspecial;

        /// <summary>
        /// Datos del elemento especial.
        /// </summary>
        private Dictionary<string, object> mDatosEspecial;

        /// <summary>
        ///Indica si es un selector de grupo.
        /// </summary>
        private bool mEsSelectorGrupo;

        /// <summary>
        /// Opciones de selector de grupo.
        /// </summary>
        private Dictionary<string, string> mOpcionesSelectorGrupo;

        /// <summary>
        /// Indica que no se debe pintar el título.
        /// </summary>
        private bool mSinTitulo;

        /// <summary>
        /// Tipo de presentación.
        /// </summary>
        private string mTipoPresentacion;

        /// <summary>
        /// Elemento padre.
        /// </summary>
        private ElementoOrdenado mElementoPadre;

        /// <summary>
        /// Link a otro lugar.
        /// </summary>
        private string mLink;

        /// <summary>
        /// Target del link a otro lugar.
        /// </summary>
        private string mTargetLink;

        /// <summary>
        /// Tamaño de la foto.
        /// </summary>
        private string mSizeFoto;

        /// <summary>
        /// Size de la foto.
        /// </summary>
        private string mSizeAumentoFoto;

        /// <summary>
        /// Mensaje de ayuda del campo.
        /// </summary>
        private string mMensajeAyuda;

        /// <summary>
        /// Indica si una propiedad solo debe mostrar su valor en el idioma de navegación del usuario o no mostrarlo.
        /// </summary>
        private bool mSoloIdiomaNavegacion;

        /// <summary>
        /// Indica que el elemento no es editable en la vista de edición.
        /// </summary>
        private bool? mNoEditable;

        /// <summary>
        /// Propiedades con sus valores según los cuales debe o no pintarse una entidad.
        /// </summary>
        private Dictionary<string, KeyValuePair<bool, List<string>>> mPropsCondicionPintarEntSegunValores;

        #endregion

        #region Propiedades

        /// <summary>
        /// Indica si es grupo o no.
        /// </summary>
        public bool EsGrupo
        {
            get
            {
                return mEsGrupo;
            }
            set
            {
                mEsGrupo = value; 
            }
        }

        /// <summary>
        /// Propiedades del elemento.
        /// </summary>
        public Propiedad Propiedad
        {
            get
            {
                return NombrePropiedad.Value;
            }
        }

        /// <summary>
        /// Nombre de las propiedades del elemento.
        /// </summary>
        public KeyValuePair<string, Propiedad> NombrePropiedad
        {
            get
            {
                return mNombrePropiedad;
            }
            set
            {
                mNombrePropiedad = value;
            }
        }

        /// <summary>
        /// Clase del grupo.
        /// </summary>
        public string ClaseGrupo
        {
            get
            {
                return mClaseGrupo;
            }
            set
            {
                mClaseGrupo = value;
            }
        }

        /// <summary>
        /// ID para el grupo.
        /// </summary>
        public string IdGrupo
        {
            get
            {
                return mIdGrupo;
            }
            set
            {
                mIdGrupo = value;
            }
        }

        /// <summary>
        /// Clase del grupo para la lectura.
        /// </summary>
        public string ClaseGrupoLectura
        {
            get
            {
                return mClaseGrupoLectura;
            }
            set
            {
                mClaseGrupoLectura = value;
            }
        }

        /// <summary>
        /// ID para el grupo para la lectura.
        /// </summary>
        public string IdGrupoLectura
        {
            get
            {
                return mIdGrupoLectura;
            }
            set
            {
                mIdGrupoLectura = value;
            }
        }

        /// <summary>
        /// Propiedad de entidad hija (tipo entidad hija).
        /// </summary>
        public string PropDeEntHija
        {
            get
            {
                return mPropDeEntHija;
            }
            set
            {
                mPropDeEntHija = value;
            }
        }

        /// <summary>
        /// Indica que solo debe pintarse el 1º valor de la propiedad.
        /// </summary>
        public bool SoloPrimerValor
        {
            get
            {
                return mSoloPrimerValor;
            }
            set
            {
                mSoloPrimerValor = value;
            }
        }

        /// <summary>
        /// Hijo del grupo
        /// </summary>
        public List<ElementoOrdenado> Hijos
        {
            get
            {
                if (mHijos == null)
                {
                    mHijos = new List<ElementoOrdenado>();
                }

                return mHijos;
            }
            set
            {
                mHijos = value;
            }
        }

        /// <summary>
        /// Tipo de grupo.
        /// </summary>
        public string TipoGrupo
        {
            get
            {
                return mTipoGrupo;
            }
            set
            {
                mTipoGrupo = value;
            }
        }

        /// <summary>
        /// Indica si es literal.
        /// </summary>
        public bool EsLiteral
        {
            get
            {
                return mEsLiteral;
            }
            set
            {
                mEsLiteral = value;
            }
        }

        /// <summary>
        /// Indica si es literal es importante y debe pintarse siempre.
        /// </summary>
        public bool LiteralImportante
        {
            get
            {
                return mLiteralImportante;
            }
            set
            {
                mLiteralImportante = value;
            }
        }

        /// <summary>
        /// Indica si es un especial.
        /// </summary>
        public bool EsEspecial
        {
            get
            {
                return mEsEspecial;
            }
            set
            {
                mEsEspecial = value;
            }
        }

        /// <summary>
        /// Datos del elemento especial.
        /// </summary>
        public Dictionary<string, object> DatosEspecial
        {
            get
            {
                return mDatosEspecial;
            }
            set
            {
                mDatosEspecial = value;
            }
        }

        /// <summary>
        ///Indica si es un selector de grupo.
        /// </summary>
        public bool EsSelectorGrupo
        {
            get
            {
                return mEsSelectorGrupo;
            }
            set
            {
                mEsSelectorGrupo = value;
            }
        }

        /// <summary>
        /// Opciones de selector de grupo.
        /// </summary>
        public Dictionary<string, string> OpcionesSelectorGrupo
        {
            get
            {
                if (mOpcionesSelectorGrupo == null)
                {
                    mOpcionesSelectorGrupo = new Dictionary<string, string>();
                }

                return mOpcionesSelectorGrupo;
            }
            set
            {
                mOpcionesSelectorGrupo = value;
            }
        }

        /// <summary>
        /// Indica que no se debe pintar el título.
        /// </summary>
        public bool SinTitulo
        {
            get
            {
                return mSinTitulo;
            }
            set
            {
                mSinTitulo = value;
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
        /// Elemento padre.
        /// </summary>
        public ElementoOrdenado ElementoPadre
        {
            get
            {
                return mElementoPadre;
            }
            set
            {
                mElementoPadre = value;
            }
        }

        /// <summary>
        /// Link a otro lugar.
        /// </summary>
        public string Link
        {
            get
            {
                return mLink;
            }
            set
            {
                mLink = value;
            }
        }

        /// <summary>
        /// Target del link a otro lugar.
        /// </summary>
        public string TargetLink
        {
            get
            {
                return mTargetLink;
            }
            set
            {
                mTargetLink = value;
            }
        }

        /// <summary>
        /// Size de la foto.
        /// </summary>
        public string SizeFoto
        {
            get
            {
                return mSizeFoto;
            }
            set
            {
                mSizeFoto = value;
            }
        }

        /// <summary>
        /// Size de la foto.
        /// </summary>
        public string SizeAumentoFoto
        {
            get
            {
                return mSizeAumentoFoto;
            }
            set
            {
                mSizeAumentoFoto = value;
            }
        }

        /// <summary>
        /// Mensaje de ayuda del campo.
        /// </summary>
        public string MensajeAyuda
        {
            get
            {
                return mMensajeAyuda;
            }
            set
            {
                mMensajeAyuda = value;
            }
        }

        /// <summary>
        /// Indica si una propiedad solo debe mostrar su valor en el idioma de navegación del usuario o no mostrarlo.
        /// </summary>
        public bool SoloIdiomaNavegacion
        {
            get
            {
                return mSoloIdiomaNavegacion;
            }
            set
            {
                mSoloIdiomaNavegacion = value;
            }
        }

        /// <summary>
        /// Indica que el elemento no es editable en la vista de edición.
        /// </summary>
        public bool? NoEditable
        {
            get
            {
                return mNoEditable;
            }
            set
            {
                mNoEditable = value;
            }
        }

        /// <summary>
        /// Propiedades con sus valores según los cuales debe o no pintarse una entidad.
        /// </summary>
        public Dictionary<string, KeyValuePair<bool, List<string>>> PropsCondicionPintarEntSegunValores
        {
            get
            {
                if (mPropsCondicionPintarEntSegunValores == null)
                {
                    mPropsCondicionPintarEntSegunValores = new Dictionary<string, KeyValuePair<bool, List<string>>>();
                }

                return mPropsCondicionPintarEntSegunValores;
            }
            set
            {
                mPropsCondicionPintarEntSegunValores = value;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor sin parámetros.
        /// </summary>
        public ElementoOrdenado()
        {
        }

        /// <summary>
        /// Constructor a partir de otro elemento.
        /// </summary>
        /// <param name="pElemento">Elemento</param>
        public ElementoOrdenado(ElementoOrdenado pElemento)
        {
            mEsGrupo = pElemento.EsGrupo;
            mNombrePropiedad = new  KeyValuePair<string,Propiedad>(pElemento.NombrePropiedad.Key, null);
            mClaseGrupo = pElemento.ClaseGrupo;
            mIdGrupo = pElemento.IdGrupo;
            mClaseGrupoLectura = pElemento.ClaseGrupoLectura;
            mIdGrupoLectura = pElemento.IdGrupoLectura;
            mPropDeEntHija = pElemento.PropDeEntHija;
            mSoloPrimerValor = pElemento.SoloPrimerValor;
            foreach (ElementoOrdenado elemOrdHijo in pElemento.Hijos)
            {
                Hijos.Add(new ElementoOrdenado(elemOrdHijo));
            }
            mTipoGrupo = pElemento.TipoGrupo;
            mEsLiteral = pElemento.EsLiteral;
            mLiteralImportante = pElemento.LiteralImportante;
            mEsEspecial = pElemento.EsEspecial;
            mDatosEspecial = pElemento.DatosEspecial;
            mEsSelectorGrupo = pElemento.EsSelectorGrupo;
            mOpcionesSelectorGrupo = pElemento.OpcionesSelectorGrupo;
            mSinTitulo = pElemento.SinTitulo;
            mTipoPresentacion = pElemento.TipoPresentacion;
            mElementoPadre = pElemento.ElementoPadre;//TODO: Cuidado con las referencias
            mLink = pElemento.Link;
            mTargetLink = pElemento.TargetLink;
            mSizeFoto = pElemento.SizeFoto;
            mSizeAumentoFoto = pElemento.SizeAumentoFoto;
            mMensajeAyuda = pElemento.MensajeAyuda;
            mSoloIdiomaNavegacion = pElemento.SoloIdiomaNavegacion;
            mNoEditable = pElemento.NoEditable;
            mPropsCondicionPintarEntSegunValores = pElemento.PropsCondicionPintarEntSegunValores;
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Comprueba si una entidad cumple las condiciones impuestas por el elemento ordenado para ser mostrada.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <returns>TRUE si una entidad cumple las condiciones impuestas por el elemento ordenado para ser mostrada, FALSE si no</returns>
        public bool CumpleEntidadCondicionesMostrar(ElementoOntologia pEntidad)
        {
            return CumpleEntidadCondicionesMostrar(pEntidad, PropsCondicionPintarEntSegunValores);
        }

        /// <summary>
        /// Comprueba si una entidad cumple las condiciones impuestas por el elemento ordenado para ser mostrada.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <returns>TRUE si una entidad cumple las condiciones impuestas por el elemento ordenado para ser mostrada, FALSE si no</returns>
        public static bool CumpleEntidadCondicionesMostrar(ElementoOntologia pEntidad, Dictionary<string, KeyValuePair<bool, List<string>>> pPropsCondicionPintarEntSegunValores)
        {
            foreach (string nombreProp in pPropsCondicionPintarEntSegunValores.Keys)
            {
                Propiedad prop = pEntidad.ObtenerPropiedad(nombreProp);

                if (prop != null)
                {
                    bool cumpleCondicion = false;

                    foreach (string valor in pPropsCondicionPintarEntSegunValores[nombreProp].Value)
                    {
                        if (prop.ValoresUnificados.ContainsKey(valor))
                        {
                            cumpleCondicion = true;
                            break;
                        }
                    }

                    if ((pPropsCondicionPintarEntSegunValores[nombreProp].Key && !cumpleCondicion) || (!pPropsCondicionPintarEntSegunValores[nombreProp].Key && cumpleCondicion)) //Se tiene que cumplir y no lo cumple Ó no se tiene que cumplir y lo cumple.
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        protected bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~ElementoOrdenado()
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
        protected void Dispose(bool disposing, bool pEliminarListasComunes)
        {
            if (!disposed)
            {
                disposed = true;
                if (disposing)
                {
                    //Libero todos los recursos administrados que he añadido a esta clase

                    if (mHijos != null)
                    {
                        foreach (ElementoOrdenado elemOrd in mHijos)
                        {
                            elemOrd.Dispose();
                        }

                        mHijos.Clear();
                    }
                }

                mHijos = null;
                mDatosEspecial = null;
                mOpcionesSelectorGrupo = null;
                mPropsCondicionPintarEntSegunValores = null;
            }
        }

        #endregion
    }
}
