using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.ExportarImportar;
using Es.Riam.Gnoss.ExportarImportar.ElementosOntologia;
using Es.Riam.Gnoss.ExportarImportar.Exportadores;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces;
using Es.Riam.Semantica.OWL;
using Es.Riam.Util;
using System;
using System.Collections.Generic;

namespace Es.Riam.Metagnoss.ExportarImportar.Exportadores
{
    /// <summary>
    /// Exportador para comunidades Web.
    /// </summary>
    public class ExportadorComunidad : ExportadorElementoGnoss, IDisposable
    {
        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private UtilSemCms mUtilSemCms;

        #region Constructor

        /// <summary>
        /// Crea un nuevo exportador de blogs y entradas de blog.
        /// </summary>
        /// <param name="pOntologia">Ontología</param>
        public ExportadorComunidad(Ontologia pOntologia, string pIdiomaUsuario, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, UtilSemCms utilSemCms, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pOntologia, pIdiomaUsuario, loggingService, entityContext, configService, redisCacheWrapper, utilSemCms, servicesUtilVirtuosoAndReplication)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
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
            if (pEntidadBuscada.TipoEntidad.Equals(TipoElementoGnoss.Comunidad))
                UtilImportarExportar.ObtenerAtributosEntidad(pEntidadBuscada, ((Proyecto)pElementoGnoss).FilaProyecto);
            else if (pEntidadBuscada.TipoEntidad.Equals(TipoElementoGnoss.ComunidadSioc))
            {
                Propiedad title = UtilImportarExportar.ObtenerPropiedadDeNombre(UtilImportarExportar.PROPIEDAD_DC_TITULO, pEntidadBuscada.Propiedades);

                if (title.FunctionalProperty)
                {
                    title.UnicoValor = new KeyValuePair<string, ElementoOntologia>(pElementoGnoss.Nombre, null);
                }
                else if (!title.ListaValores.ContainsKey(pElementoGnoss.Nombre))
                {
                    title.ListaValores.Add(pElementoGnoss.Nombre, null);
                }
                pEntidadBuscada.Descripcion = pElementoGnoss.Nombre;
            }
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
                //case UtilImportarExportar.PROPIEDAD_CATEGORIA_TESAURO_INFERIOR:
                case UtilImportarExportar.PROPIEDAD_SIOC_TOPIC:
                    ObtenerCategoriaTesauro(pEntidad, pElementoGnoss, pPropiedad, pGestor);
                    resultado = true;
                    break;
                default:
                    break;
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene todos los tags de la comunidad.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad</param>
        /// <param name="pPropiedad">Propiedad a tratar</param>
        /// <param name="pGestor">Gestor</param>
        private void ObtenerTagsComunidad(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor)
        {
            List<string> listaTags = new List<string>();

            if (pElementoGnoss is Proyecto)
            {
                listaTags = UtilCadenas.SepararTexto(((Proyecto)pElementoGnoss).FilaProyecto.Tags);
            }

            AgregarRelacionTagsEntidad(pEntidad, pElementoGnoss, pPropiedad, pGestor, listaTags);
        }

        /// <summary>
        /// Obtiene la categoría de Tesauro a la que pertenece un elemento
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pElementoGnoss">Elemento que representa la entidad</param>
        /// <param name="pPropiedad">Propiedad a tratar</param>
        /// <param name="pGestor">Gestor</param>
        private void ObtenerCategoriaTesauro(ElementoOntologia pEntidad, IElementoGnoss pElementoGnoss, Propiedad pPropiedad, GestionGnoss pGestor)
        {
            List<CategoriaTesauro> listaCategorias = new List<CategoriaTesauro>();

            if (pElementoGnoss is Proyecto)
            {
                listaCategorias.AddRange(((Proyecto)pElementoGnoss).ListaCategoriasTesauro);
            }

            foreach (CategoriaTesauro categoria in listaCategorias)
            {
                ElementoOntologia entidadCategoriaTesauro = new ElementoOntologiaGnoss(Ontologia.GetEntidadTipo(TipoElementoGnoss.CategoriasTesauroSkos));

                ExportadorWiki exportadorWiki = new ExportadorWiki(Ontologia, IdiomaUsuario, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mUtilSemCms, mServicesUtilVirtuosoAndReplication);

                entidadCategoriaTesauro.Descripcion = categoria.Nombre[IdiomaUsuario];
                Gnoss.AD.EntityModel.Models.Tesauro.CategoriaTesauro editorRow = categoria.FilaCategoria;

                UtilImportarExportar.ObtenerID(entidadCategoriaTesauro, categoria.FilaCategoria, categoria);
                ElementoOntologia categoriaTesauroAuxiliar = ComprobarEntidadIncluida(entidadCategoriaTesauro.ID);

                if ((categoriaTesauroAuxiliar == null) || (!categoriaTesauroAuxiliar.EstaCompleta))
                {
                    if (categoriaTesauroAuxiliar != null)
                        entidadCategoriaTesauro = categoriaTesauroAuxiliar;

                    //Obtengo la entidad
                    AgregarEntidadRelacionada(pEntidad, pPropiedad, entidadCategoriaTesauro);
                    exportadorWiki.ObtenerEntidad(entidadCategoriaTesauro, categoria, true, categoria.GestorTesauro);
                }
                else
                {
                    //asigno la entidad ya creada
                    entidadCategoriaTesauro = categoriaTesauroAuxiliar;
                    AgregarEntidadRelacionada(pEntidad, pPropiedad, entidadCategoriaTesauro);
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
            if (pElementoGnoss is Proyecto)
            {
                base.ObtenerEntidadProyecto(pEntidad, pElementoGnoss, ((Proyecto)pElementoGnoss).FilaProyecto, pEspecializacion, pGestor);

                if (pEntidad.TipoEntidad == TipoElementoGnoss.Comunidad || pEntidad.TipoEntidad == TipoElementoGnoss.ComunidadSioc)
                {
                    ObtenerTagsComunidad(pEntidad, pElementoGnoss, UtilImportarExportar.ObtenerPropiedadDeNombre(UtilImportarExportar.PROPIEDAD_SIOC_TOPIC, pEntidad.Propiedades), pGestor);
                }
            }
            else if (pElementoGnoss is ElementoGnoss)
                base.ObtenerEntidad(pEntidad, pElementoGnoss, ((ElementoGnoss)pElementoGnoss).FilaElemento, pEspecializacion, pGestor);
        }

        /// <summary>
        /// Busca la competencia que está seleccionada dentro del gestor
        /// <param name="pGestor">Gestor de entidades</param>
        /// <param name="pTipoEntidad">Tipo de entidad</param>
        /// </summary>
        public static IElementoGnoss ObtenerElementoSeleccionado(GestionProyecto pGestor, string pTipoEntidad)
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

        #region Dispose

        /// <summary>
        /// Destructor
        /// </summary>
        ~ExportadorComunidad()
        {
            //Libero los recursos
            Dispose(false);
        }

        #endregion
    }
}
