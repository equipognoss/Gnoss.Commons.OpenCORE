using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ListaResultados;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.ExportarImportar;
using Es.Riam.Gnoss.ExportarImportar.ElementosOntologia;
using Es.Riam.Gnoss.ExportarImportar.Exportadores;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Interfaces;
using Es.Riam.Semantica.OWL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Data;

namespace Es.Riam.Metagnoss.ExportarImportar.Exportadores
{
    /// <summary>
    /// Exportador para la lista de resultados.
    /// </summary>
    public class ExportadorListaResultados : ExportadorElementoGnoss, IDisposable
    {
        #region Miembros

        /// <summary>
        /// Exportadores para documentos semánticos auxiliares.
        /// </summary>
        private Dictionary<Guid, ExportadorWiki> mExportDocSemAux;

        /// <summary>
        /// DataSet de proyecto con la presentación para los recursos.
        /// </summary>
        private DataWrapperProyecto mDataWrapperProyecto;

        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private UtilSemCms mUtilSemCms;

        #endregion

        #region Constructor

        /// <summary>
        /// Crea un nuevo exportador de blogs y entradas de blog.
        /// </summary>
        /// <param name="pOntologia">Ontología</param>
        public ExportadorListaResultados(Ontologia pOntologia, string pIdiomaUsuario,  LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, UtilSemCms utilSemCms, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, VirtuosoAD virtuosoAd)
            : base(pOntologia, pIdiomaUsuario, loggingService, entityContext, configService, redisCacheWrapper, utilSemCms, servicesUtilVirtuosoAndReplication, virtuosoAd)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mVirtuosoAD = virtuosoAd;
            mUtilSemCms = utilSemCms;
        }

        #endregion

        #region Métodos generales

        #region Métodos protegidos

        /// <summary>
        /// Obtiene los atributos de la entidad.
        /// </summary>
        /// <param name="pEntidadBuscada">Entidad de la que hay que obtener sus atribtos.</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad.</param>
        public override void ObtenerAtributosEntidad(ElementoOntologia pEntidadBuscada, IElementoGnoss pElementoGnoss)
        {
            if (pEntidadBuscada.TipoEntidad.Equals(TipoElementoGnoss.ListaResultados))
                UtilImportarExportar.ObtenerAtributosEntidad(pEntidadBuscada, ((ListaResultados)pElementoGnoss).FilaElemento);
            else if (pEntidadBuscada.TipoEntidad.Equals(TipoElementoGnoss.Filtro))
                UtilImportarExportar.ObtenerAtributosEntidad(pEntidadBuscada, ((Filtro)pElementoGnoss).FilaElemento);
            else
                base.ObtenerAtributosEntidad(pEntidadBuscada, pElementoGnoss);
        }

        /// <summary>
        /// Generaliza una entidad para obtener los atributos del padre.
        /// </summary>
        /// <param name="pEntidad">Entidad a generalizar</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad.</param>
        /// <param name="pFilaElemento">Fila del elemento</param>
        /// <param name="pGestor"></param>
        protected override void GeneralizarEntidad(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, object pFilaElemento, GestionGnoss pGestor)
        {
            base.GeneralizarEntidad(pEntidad, pFilaElemento);
        }

        /// <summary>
        /// Trata los casos especiales de blogs.
        /// </summary>
        /// <param name="pEntidad">Entidad que posee las propiedades</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad.</param>
        /// <param name="pPropiedad">Propiedad a tratar.</param>
        /// <param name="pGestor">Gestor de competencias.</param>
        /// <returns></returns>
        protected override bool TratarCasoEspecial(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor)
        {
            bool resultado = false;
            switch (pPropiedad.Nombre)
            {
                case UtilImportarExportar.PROPIEDAD_RESULTADOS:
                    ObtenerResultadosLista(pEntidad, pElementoGnoss, pPropiedad, pGestor);
                    resultado = true;
                    break;
                case UtilImportarExportar.PROPIEDAD_FILTROS:
                    ObtenerFiltrosListaResultados(pEntidad, pElementoGnoss, pPropiedad, pGestor);
                    resultado = true;
                    break;
                default:
                    break;
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene todos los resultados de la lista.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad</param>
        /// <param name="pPropiedad">Propiedad a tratar</param>
        /// <param name="pGestor">Gestor</param>
        private void ObtenerResultadosLista(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor)
        {
            foreach (IElementoGnoss resultado in ((ListaResultados)pElementoGnoss).Resultados)
            {
                ElementoOntologia entidadResultado = null;
                ExportadorElementoGnoss exportador = null;
                object resultadoRow = null;
                GestionGnoss gestorElemento = null;

                if (resultado is Documento)
                {
                    string tipo = TipoElementoGnoss.Documento;
                    if (((Documento)resultado).TipoDocumentacion.Equals(TiposDocumentacion.Debate))
                    {
                        tipo = TipoElementoGnoss.Debate;
                    }
                    else if (((Documento)resultado).TipoDocumentacion.Equals(TiposDocumentacion.Pregunta))
                    {
                        tipo = TipoElementoGnoss.Pregunta;
                    }

                    entidadResultado = new ElementoOntologiaGnoss(this.Ontologia.GetEntidadTipo(tipo));
                    exportador = new ExportadorWiki(Ontologia, IdiomaUsuario,  mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mUtilSemCms, mServicesUtilVirtuosoAndReplication, mVirtuosoAd);
                    resultadoRow = ((Documento)resultado).FilaDocumento;
                    gestorElemento = ((Documento)resultado).GestorDocumental;

                    Guid elementoVinID = ((Documento)resultado).ElementoVinculadoID;

                    if (ExportDocSemAux.ContainsKey(elementoVinID) && ExportDocSemAux[elementoVinID] != null)
                    {
                        ((ExportadorWiki)exportador).URL_ONTOLOGIA_DOCSEM = ExportDocSemAux[elementoVinID].URL_ONTOLOGIA_DOCSEM;
                        ((ExportadorWiki)exportador).ARRAY_ONTOLOGIA_DOCSEM = ExportDocSemAux[elementoVinID].ARRAY_ONTOLOGIA_DOCSEM;
                        ((ExportadorWiki)exportador).NOMBRE_ONTOLOGIA_DOCSEM = ExportDocSemAux[elementoVinID].NOMBRE_ONTOLOGIA_DOCSEM;
                        ((ExportadorWiki)exportador).URLINTRAGNOSS = ExportDocSemAux[elementoVinID].URLINTRAGNOSS;
                        ((ExportadorWiki)exportador).CONFIG_XML_DOCSEM = ExportDocSemAux[elementoVinID].CONFIG_XML_DOCSEM;
                        ((ExportadorWiki)exportador).NAMESPACE_DOCSEM = ExportDocSemAux[elementoVinID].NAMESPACE_DOCSEM;
                        ((ExportadorWiki)exportador).XML_ID = ExportDocSemAux[elementoVinID].XML_ID;
                        ((ExportadorWiki)exportador).ONTOLOGIA = ExportDocSemAux[elementoVinID].ONTOLOGIA;
                        ((ExportadorWiki)exportador).INSTANCIAS_PRINCIPALES = ExportDocSemAux[elementoVinID].INSTANCIAS_PRINCIPALES;
                        ((ExportadorWiki)exportador).CLAVE_ONTO_TROCEADA = ExportDocSemAux[elementoVinID].CLAVE_ONTO_TROCEADA;

                        ((ExportadorWiki)exportador).ProyectoPresentacionDS = ProyectoPresentacionDS;
                    }
                }
                else if (resultado is Proyecto)
                {
                    entidadResultado = new ElementoOntologiaGnoss(this.Ontologia.GetEntidadTipo(TipoElementoGnoss.ComunidadSioc));
                    exportador = new ExportadorComunidad(Ontologia, IdiomaUsuario,  mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mUtilSemCms, mServicesUtilVirtuosoAndReplication, mVirtuosoAD);
                    resultadoRow = ((Proyecto)resultado).FilaProyecto;
                    gestorElemento = ((Proyecto)resultado).GestorProyectos;
                }
                else if (resultado is Identidad)
                {
                    if (((Identidad)resultado).Tipo != TiposIdentidad.Organizacion && ((Identidad)resultado).Tipo != TiposIdentidad.ProfesionalCorporativo)
                    {
                        entidadResultado = new ElementoOntologiaGnoss(this.Ontologia.GetEntidadTipo(TipoElementoGnoss.PerfilPersonaFoaf));
                    }
                    else
                    {
                        entidadResultado = new ElementoOntologiaGnoss(this.Ontologia.GetEntidadTipo(TipoElementoGnoss.PerfilOrganizacionFoaf));
                    }
                    exportador = new ExportadorCurriculum(Ontologia, ((Identidad)resultado).GestorIdentidades, IdiomaUsuario,  mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mUtilSemCms, mServicesUtilVirtuosoAndReplication, mVirtuosoAD);
                    resultadoRow = ((Identidad)resultado).FilaIdentidad;
                    gestorElemento = ((Identidad)resultado).GestorIdentidades;
                }

                //entidadResultado.Descripcion = editor.Nombre;

                exportador.TipoElementoExportar = TipoElementoExportar;

                UtilImportarExportar.ObtenerID(entidadResultado, resultadoRow, resultado);
                ElementoOntologia entidadAuxiliar = ComprobarEntidadIncluida(entidadResultado.ID);
                //Obtengo las definiciones
                if ((entidadAuxiliar == null) || (!entidadAuxiliar.EstaCompleta) || (resultado is Proyecto))
                {
                    if (entidadAuxiliar != null)
                        entidadResultado = entidadAuxiliar;

                    //Obtengo la entidad
                    AgregarEntidadRelacionada(pEntidad, pPropiedad, entidadResultado);
                    exportador.ObtenerEntidad(entidadResultado, resultado, false, gestorElemento);
                }
                else
                {
                    //asigno la entidad ya creada
                    entidadResultado = entidadAuxiliar;
                    AgregarEntidadRelacionada(pEntidad, pPropiedad, entidadResultado);
                }
            }
        }

        /// <summary>
        /// Obtiene todos los filtros de la lista.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad</param>
        /// <param name="pPropiedad">Propiedad a tratar</param>
        /// <param name="pGestor">Gestor</param>
        private void ObtenerFiltrosListaResultados(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor)
        {
            foreach (Filtro filtro in ((ListaResultados)pElementoGnoss).Filtros)
            {
                ElementoOntologia entidadResultado = new ElementoOntologiaGnoss(this.Ontologia.GetEntidadTipo(TipoElementoGnoss.Filtro));
                ExportadorElementoGnoss exportador = new ExportadorListaResultados(Ontologia, IdiomaUsuario,  mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mUtilSemCms, mServicesUtilVirtuosoAndReplication, mVirtuosoAd);
                DataRow resultadoRow = filtro.FilaElemento;
                GestionGnoss gestorElemento = filtro.GestorGnoss;

                entidadResultado.Descripcion = filtro.Nombre;

                UtilImportarExportar.ObtenerID(entidadResultado, resultadoRow, filtro);
                ElementoOntologia entidadAuxiliar = ComprobarEntidadIncluida(entidadResultado.ID);
                //Obtengo las definiciones
                if ((entidadAuxiliar == null) || (!entidadAuxiliar.EstaCompleta))
                {
                    if (entidadAuxiliar != null)
                        entidadResultado = entidadAuxiliar;

                    //Obtengo la entidad
                    AgregarEntidadRelacionada(pEntidad, pPropiedad, entidadResultado);
                    exportador.ObtenerEntidad(entidadResultado, filtro, false, gestorElemento);
                }
                else
                {
                    //asigno la entidad ya creada
                    entidadResultado = entidadAuxiliar;
                    AgregarEntidadRelacionada(pEntidad, pPropiedad, entidadResultado);
                }
            }
        }

        /// <summary>
        /// Obtiene todos los tags de un blog o de una entrada de blog.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad</param>
        /// <param name="pPropiedad">Propiedad a tratar</param>
        /// <param name="pGestor">Gestor</param>
        private void ObtenerTagsListaResultados(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor)
        {
            List<string> listaTags = new List<string>();

            if (pElementoGnoss is ListaResultados)
            {
                listaTags = ((ListaResultados)pElementoGnoss).ListaTags;
            }

            AgregarRelacionTagsEntidad(pEntidad, pElementoGnoss, pPropiedad, pGestor, listaTags);
        }

        /// <summary>
        /// Obtiene el numeros de pagina actual y el de resultados por página de la lista de resultados.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad</param>
        /// <param name="pPropiedad">Propiedad a tratar</param>
        /// <param name="pGestor">Gestor</param>
        private void ObtenerNumerosPaginasResultados(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor)
        {
            if (pPropiedad.Nombre.Equals(UtilImportarExportar.PROPIEDAD_NUM_PAGINA))
            {
                if (((ListaResultados)pElementoGnoss).NumeroPaginaActual != 0)
                {
                    pPropiedad.ListaValores.Add(((ListaResultados)pElementoGnoss).NumeroPaginaActual.ToString(), null);
                }
            }
            else if (pPropiedad.Nombre.Equals(UtilImportarExportar.PROPIEDAD_NUM_RESULT_POR_PAG))
            {
                if (((ListaResultados)pElementoGnoss).NumeroResultadosPorPagina != 0)
                {
                    pPropiedad.ListaValores.Add(((ListaResultados)pElementoGnoss).NumeroResultadosPorPagina.ToString(), null);
                }
            }
        }

        #endregion

        #region Métodos públicos

        /// <summary>
        /// Obtiene la entidad pEntidad y todas sus relaciones.
        /// </summary>
        /// <param name="pEntidad">Entidad que se va a obtener.</param>
        /// <param name="pElementoGnoss">Elemento gnoss que representa la entidad</param>
        /// <param name="pEspecializacion">Indica si la entidad será especilización de otra.</param>
        /// <param name="pGestor">Gestor de entidades</param>
        public override void ObtenerEntidad(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, bool pEspecializacion, GestionGnoss pGestor)
        {
            if (pElementoGnoss is ListaResultados)
            {
                base.ObtenerEntidad(pEntidad, pElementoGnoss, ((ListaResultados)pElementoGnoss).FilaElemento, pEspecializacion, pGestor);

                Propiedad propiedadTopic = UtilImportarExportar.ObtenerPropiedadDeNombre(UtilImportarExportar.PROPIEDAD_SIOC_TOPIC, pEntidad.Propiedades);
                if (propiedadTopic != null)
                {
                    ObtenerTagsListaResultados(pEntidad, pElementoGnoss, propiedadTopic, pGestor);
                }

                ObtenerNumerosPaginasResultados(pEntidad, pElementoGnoss, UtilImportarExportar.ObtenerPropiedadDeNombre(UtilImportarExportar.PROPIEDAD_NUM_PAGINA, pEntidad.Propiedades), pGestor);
                ObtenerNumerosPaginasResultados(pEntidad, pElementoGnoss, UtilImportarExportar.ObtenerPropiedadDeNombre(UtilImportarExportar.PROPIEDAD_NUM_RESULT_POR_PAG, pEntidad.Propiedades), pGestor);
            }
            else if (pElementoGnoss is ElementoGnoss)
                base.ObtenerEntidad(pEntidad, pElementoGnoss, ((ElementoGnoss)pElementoGnoss).FilaElemento, pEspecializacion, pGestor);
        }

        /// <summary>
        /// Busca la competencia que está seleccionada dentro del gestor
        /// <param name="pGestor">Gestor de entidades</param>
        /// <param name="pTipoEntidad">Tipo de entidad</param>
        /// </summary>
        public static IElementoGnoss ObtenerElementoSeleccionado(GestorListaResultados pGestor, string pTipoEntidad)
        {
            switch (pTipoEntidad)
            {
                case TipoElementoGnoss.CategoriasTesauro:
                    {
                        break;
                    }
            }
            return null;
        }

        #endregion

        #endregion

        #region Propiedades

        /// <summary>
        /// Exportadores para documentos semánticos auxiliares.
        /// </summary>
        public Dictionary<Guid, ExportadorWiki> ExportDocSemAux
        {
            get
            {
                if (mExportDocSemAux == null)
                {
                    mExportDocSemAux = new Dictionary<Guid, ExportadorWiki>();
                }

                return mExportDocSemAux;
            }
            set
            {
                mExportDocSemAux = value;
            }
        }

        /// <summary>
        /// DataSet de proyecto con la presentación para los recursos.
        /// </summary>
        public DataWrapperProyecto ProyectoPresentacionDS
        {
            get
            {
                return mDataWrapperProyecto;
            }
            set
            {
                mDataWrapperProyecto = value;
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Destructor
        /// </summary>
        ~ExportadorListaResultados()
        {
            //Libero los recursos
            Dispose(false);
        }

        #endregion
    }
}
