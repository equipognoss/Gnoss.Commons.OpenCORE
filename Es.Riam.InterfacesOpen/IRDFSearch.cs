using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.MetaBuscadorAD;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Tesauro;
using Es.Riam.Gnoss.Elementos;
using Es.Riam.Gnoss.Elementos.Comentario;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.ExportarImportar;
using Es.Riam.Gnoss.ExportarImportar.ElementosOntologia;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.MetaBuscador;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.Metagnoss.ExportarImportar;
using Es.Riam.Semantica.OWL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.InterfacesOpen
{
    public abstract class IRDFSearch
    {
        

        protected LoggingService mLoggingService;
        protected EntityContext mEntityContext;
        protected ConfigService mConfigService;
        protected RedisCacheWrapper mRedisCacheWrapper;
        protected IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        protected VirtuosoAD mVirtuosoAD;
        protected IHttpContextAccessor mHttpContextAccessor;
        protected GnossCache mGnossCache;
        protected EntityContextBASE mEntityContextBASE;

        protected GestionOrganizaciones mGestionOrganizaciones;
        protected GestionPersonas mGestionPersonas;
        protected GestionComentarios mGestionComentarios;
        protected GestorDocumental mGestionDocumentacion;
        protected GestionIdentidades mGestorIdentidades;
        protected GestionProyecto mGestorProyecto;
        

        public IRDFSearch(EntityContext entityContext, LoggingService loggingService, ConfigService configService, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IHttpContextAccessor httpContextAccessor, GnossCache gnossCache, EntityContextBASE entityContextBASE)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mVirtuosoAD = virtuosoAD;
            mHttpContextAccessor = httpContextAccessor;
            mGnossCache = gnossCache;
            mEntityContextBASE = entityContextBASE;
        }

        private ControladorDocumentacion mControladorDocumentacion;

        public ControladorDocumentacion ControladorDocumentacion
        {
            get
            {
                if (mControladorDocumentacion == null)
                {
                    mControladorDocumentacion = new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);
                }
                return mControladorDocumentacion;
            }
        }

        /// <summary>
        /// URL base de los formularios semánticos.
        /// </summary>
        private string mBaseURLFormulariosSem;

        /// <summary>
        /// Obtiene la URL base de los formularios semánticos.
        /// </summary>
        public string BaseURLFormulariosSem
        {
            get
            {
                if (mBaseURLFormulariosSem == null)
                {
                    string url = mUrlIntragnoss;

                    if (url[url.Length - 1] == '/')
                    {
                        url = url.Substring(0, url.Length - 1);
                    }

                    mBaseURLFormulariosSem = url.Replace("www.", "");
                }

                return mBaseURLFormulariosSem;
            }
        }

        /// <summary>
        /// URL para la ontología basada en una plantilla semántica de GNOSS.
        /// </summary>
        public string mUrlOntologiaPlantillaRDF { get; set; }

        /// <summary>
        /// Namespace para la ontología basada en una plantilla semántica de GNOSS.
        /// </summary>
        public string mNamespacePlantillaRDF { get; set; }

        public string mUrlIntragnoss { get; set; }

        /// <summary>
        /// Url de la ontología de curriculum.
        /// </summary>
        public string UrlOntologiaCurriculum
        {
            get
            {
                return BaseURLFormulariosSem + "/Ontologia/Curriculum.owl#";
            }
        }

        public abstract byte[] CargarRDFListaResultados(ResultadoModel pListaResultados, Proyecto ProyectoSeleccionado, Ontologia OntologiaGnoss, string IdiomaUsuario, UtilSemCms UtilSemCms, UtilIdiomas UtilIdiomas, string BaseURLIdioma, string BaseURLContent, string UrlPerfil, string UrlPagina, GnossUrlsSemanticas UrlsSemanticas, Guid ProyectoPestanyaActual, string BaseURLFormulariosSem, Identidad IdentidadActual, GnossIdentity UsuarioActual, Guid ProyectoPrincipalUnico, List<string> ListaItemsBusquedas);


        protected void CargarGeneradorURLs(Proyecto ProyectoSeleccionado, UtilIdiomas UtilIdiomas, string BaseURLIdioma, string BaseURLContent, string UrlPerfil, string UrlPagina, GnossUrlsSemanticas UrlsSemanticas)
        {
            string urlProyecto = "";

            if ((ProyectoSeleccionado != null) && (!ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto)))
            {
                urlProyecto = ProyectoSeleccionado.NombreCorto;
            }

            string urlPagConParametros = UrlPagina;

            if (!string.IsNullOrEmpty(mHttpContextAccessor.HttpContext.Request.GetDisplayUrl()) && mHttpContextAccessor.HttpContext.Request.GetDisplayUrl().Contains("?"))
            {
                urlPagConParametros += mHttpContextAccessor.HttpContext.Request.GetDisplayUrl().Substring(mHttpContextAccessor.HttpContext.Request.GetDisplayUrl().IndexOf("?"));

                if (urlPagConParametros.Contains("?rdf&"))
                {
                    urlPagConParametros = urlPagConParametros.Replace("?rdf&", "?");
                }
                else if (urlPagConParametros.Contains("?rdf"))
                {
                    urlPagConParametros = urlPagConParametros.Replace("?rdf", "");
                }
                else if (urlPagConParametros.Contains("&rdf"))
                {
                    urlPagConParametros = urlPagConParametros.Replace("&rdf", "");
                }
            }

            GestionOWLGnoss.GeneradorUrls = new GnossGeneradorUrlsRDF(UtilIdiomas, BaseURLIdioma, BaseURLContent, UrlPerfil, urlProyecto, UrlPagina, urlPagConParametros, null, UrlsSemanticas, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor);
        }

        /// <summary>
        /// Obtiene la busqueda según los filtros
        /// </summary>
        protected List<ElementoGnoss> Buscar(ResultadoModel pListaResultados, Identidad IdentidadActual, GnossIdentity UsuarioActual, Proyecto ProyectoSeleccionado, Guid ProyectoPrincipalUnico, List<string> ListaItemsBusqueda)
        {
            List<Guid> listaMisIdentidades = new List<Guid>();
            listaMisIdentidades.Add(IdentidadActual.Clave);
            bool esComunidadPrivadaAccRestrOMyGnoss = (ProyectoSeleccionado.TipoAcceso == TipoAcceso.Privado || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Reservado);

            bool puedeBuscar = true;

            if ((UsuarioActual.EsUsuarioInvitado) && (!ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto)) && (!ProyectoPrincipalUnico.Equals(ProyectoAD.MetaProyecto)))
            {
                puedeBuscar = (!ListaItemsBusqueda.Contains(FacetadoAD.BUSQUEDA_ORGANIZACION) && !ListaItemsBusqueda.Contains(FacetadoAD.BUSQUEDA_PERSONA));
            }
            MetaBuscadorCN metaBuscadorCN = new MetaBuscadorCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            if (puedeBuscar)
            {
                Dictionary<string, TiposResultadosMetaBuscador> dicTemp = new Dictionary<string, TiposResultadosMetaBuscador>();
                foreach (ObjetoBuscadorModel resultado in pListaResultados.ListaResultados)
                {
                    if (resultado is ResourceModel)
                    {
                        dicTemp.Add(((ResourceModel)resultado).Key.ToString(), TiposResultadosMetaBuscador.Documento);
                    }
                    else if (resultado is ProfileModel)
                    {
                        ProfileModel profile = (ProfileModel)resultado;
                        if (profile.KeyPerson == null || profile.KeyPerson.Equals(Guid.Empty))
                        {
                            dicTemp.Add(((ProfileModel)resultado).Key.ToString(), TiposResultadosMetaBuscador.Organizacion);
                        }
                        else
                        {
                            dicTemp.Add(((ProfileModel)resultado).Key.ToString(), TiposResultadosMetaBuscador.Persona);
                        }
                    }
                    else if (resultado is BlogModel)
                    {
                        dicTemp.Add(((BlogModel)resultado).Key.ToString(), TiposResultadosMetaBuscador.Blog);
                    }
                    else if (resultado is CommunityModel)
                    {
                        dicTemp.Add(((CommunityModel)resultado).Key.ToString(), TiposResultadosMetaBuscador.Comunidad);
                    }
                }

                ////Método para cambiar la lista de elementos de Guid a String por cambios de Javi (Alberto)

                //foreach (Guid g in mListaIdsResultado.Keys)
                //{
                //    dicTemp.Add(g.ToString(), mListaIdsResultado[g]);
                //}

                //metaBuscadorCN.ListaOrdenadaElementos = mListaIdsResultado;
                metaBuscadorCN.ListaOrdenadaElementos = dicTemp;
                metaBuscadorCN.NumResultados = pListaResultados.NumeroResultadosTotal;

                metaBuscadorCN.BuscarContenidos(UsuarioActual.ProyectoID, IdentidadActual.PerfilID, UsuarioActual.EsIdentidadInvitada);
            }

            TesauroCL tesauroCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);

            if (IdentidadActual.PerfilUsuario.OrganizacionID != null && metaBuscadorCN.OrganizacionDW != null && metaBuscadorCN.OrganizacionDW.ListaOrganizacion.Any(item => item.OrganizacionID.Equals(IdentidadActual.PerfilUsuario.OrganizacionID.Value)))
            {
                //Obtengo la organización del usuario actual para que pinte a todas las personas que son de mi organización:
                OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                DataWrapperOrganizacion dataWrapperOrganizacionAux = new DataWrapperOrganizacion();
                dataWrapperOrganizacionAux.ListaOrganizacion.Add(orgCN.ObtenerNombreOrganizacionPorID(IdentidadActual.PerfilUsuario.OrganizacionID.Value));
                metaBuscadorCN.OrganizacionDW.Merge(dataWrapperOrganizacionAux);
                orgCN.Dispose();
            }
            mGestionOrganizaciones = new GestionOrganizaciones(metaBuscadorCN.OrganizacionDW, mLoggingService, mEntityContext);
            mGestionPersonas = new GestionPersonas(metaBuscadorCN.DataWrapperPersona, mLoggingService, mEntityContext);
            if (metaBuscadorCN.ComentarioDW != null)
            {
                mGestionComentarios = new GestionComentarios(metaBuscadorCN.ComentarioDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            }
            if (metaBuscadorCN.DocumentacionDW != null)
            {
                mGestionDocumentacion = new GestorDocumental(metaBuscadorCN.DocumentacionDW, mLoggingService, mEntityContext);

                mGestionDocumentacion.GestorTesauro = new GestionTesauro(tesauroCL.ObtenerTesauroDeProyecto(UsuarioActual.ProyectoID), mLoggingService, mEntityContext);
            }

            if (metaBuscadorCN.DataWrapperIdentidad != null)
            {
                mGestionPersonas = new GestionPersonas(metaBuscadorCN.DataWrapperPersona, mLoggingService, mEntityContext);
                mGestionOrganizaciones = new GestionOrganizaciones(metaBuscadorCN.OrganizacionDW, mLoggingService, mEntityContext);

                if (metaBuscadorCN.DataWrapperPersona == null)
                {
                    mGestionPersonas = new GestionPersonas(new DataWrapperPersona(), mLoggingService, mEntityContext);
                }

                if (metaBuscadorCN.OrganizacionDW == null)
                {
                    mGestionOrganizaciones = new GestionOrganizaciones(new DataWrapperOrganizacion(), mLoggingService, mEntityContext);
                }
                mGestorIdentidades = new GestionIdentidades(metaBuscadorCN.DataWrapperIdentidad, mGestionPersonas, mGestionOrganizaciones, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            }

            if (metaBuscadorCN.DataWrapperProyecto != null)
            {
                mGestorProyecto = new GestionProyecto(metaBuscadorCN.DataWrapperProyecto, mLoggingService, mEntityContext);
                mGestorProyecto.GestionTesauro = new GestionTesauro(tesauroCL.ObtenerTesauroDeProyectoMyGnoss(), mLoggingService, mEntityContext);
                List<Guid> listaProy = new List<Guid>();
                foreach (Gnoss.AD.EntityModel.Models.ProyectoDS.Proyecto filaProy in metaBuscadorCN.DataWrapperProyecto.ListaProyecto)
                {
                    if (!listaProy.Contains(filaProy.ProyectoID))
                    {
                        listaProy.Add(filaProy.ProyectoID);
                    }
                }
            }

            List<ElementoGnoss> mListadoGenerico = new List<ElementoGnoss>();

            foreach (string ID in metaBuscadorCN.ListaOrdenadaElementos.Keys)
            {
                ElementoGnoss elemento;

                switch (metaBuscadorCN.ListaOrdenadaElementos[ID])
                {
                    case TiposResultadosMetaBuscador.IdentidadPersona:
                        if (mGestorIdentidades.ListaIdentidades.ContainsKey(new Guid(ID)))
                        {
                            elemento = mGestorIdentidades.ListaIdentidades[new Guid(ID)];
                            mListadoGenerico.Add(elemento);
                        }
                        break;
                    case TiposResultadosMetaBuscador.IdentidadOrganizacion:
                        if (mGestorIdentidades.ListaIdentidades.ContainsKey(new Guid(ID)))
                        {
                            elemento = mGestorIdentidades.ListaIdentidades[new Guid(ID)];
                            mListadoGenerico.Add(elemento);
                        }
                        break;
                    case TiposResultadosMetaBuscador.Documento:
                    case TiposResultadosMetaBuscador.Pregunta:
                    case TiposResultadosMetaBuscador.Debate:
                    case TiposResultadosMetaBuscador.Encuesta:
                        if (mGestionDocumentacion.ListaDocumentos.ContainsKey(new Guid(ID)))
                        {
                            elemento = mGestionDocumentacion.ListaDocumentos[new Guid(ID)];
                            mListadoGenerico.Add(elemento);
                        }
                        break;
                    case TiposResultadosMetaBuscador.Comentario:
                        if (mGestionComentarios.ListaComentarios.ContainsKey(new Guid(ID)))
                        {
                            elemento = mGestionComentarios.ListaComentarios[new Guid(ID)];
                            mListadoGenerico.Add(elemento);
                        }
                        break;
                    case TiposResultadosMetaBuscador.Comunidad:
                        if (mGestorProyecto.ListaProyectos.ContainsKey(new Guid(ID)))
                        {
                            elemento = mGestorProyecto.ListaProyectos[new Guid(ID)];
                            mListadoGenerico.Add(elemento);
                        }
                        break;
                }
            }

            if (mGestionDocumentacion != null)
            {
                List<Guid> listaIdentidadesURLSem = new List<Guid>();

                foreach (Gnoss.AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaDocVinBR in mGestionDocumentacion.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos)
                {
                    if (filaDocVinBR.IdentidadPublicacionID.HasValue && !listaIdentidadesURLSem.Contains(filaDocVinBR.IdentidadPublicacionID.Value))
                    {
                        listaIdentidadesURLSem.Add(filaDocVinBR.IdentidadPublicacionID.Value);
                    }
                }
                GestionPersonas gestorPersonas = new GestionPersonas(new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication).ObtenerPersonasPorIdentidadesCargaLigera(listaIdentidadesURLSem), mLoggingService, mEntityContext);
                GestionOrganizaciones gestorOrg = new GestionOrganizaciones(new DataWrapperOrganizacion(), mLoggingService, mEntityContext);

                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                mGestionDocumentacion.GestorIdentidades = new GestionIdentidades(identidadCN.ObtenerIdentidadesPorID(listaIdentidadesURLSem, false), gestorPersonas, gestorOrg, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                identidadCN.Dispose();
            }

            metaBuscadorCN.Dispose();

            return mListadoGenerico;
        }

        /// <summary>
        /// Monta una entidad en formato OWL en la página actual
        /// </summary>
        /// <param name="pEntidad">Entidad a montar</param>
        /// <param name="pTipoElementoExportar">Lista con los tipos de elementos</param>
        /// <returns>RDF</returns>
        public byte[] MontarRDF(ElementoOntologia pEntidad, List<string> pTipoElementoExportar, Ontologia OntologiaGnoss, bool pBusqueda = false)
        {
            GestionOWL gestionOWL = new GestionOWL();
            gestionOWL.NamespaceOntologia = "gnoss";
            gestionOWL.UrlOntologia = null;

            //Estableco el RDF de Cv que hay que incluir en el caso de que haya que hacerlo.
            OntologiaGnoss.RDFCVSemIncluido = pEntidad.Ontologia.RDFCVSemIncluido;
            OntologiaGnoss.NamespacesDefinidosExtra = pEntidad.Ontologia.NamespacesDefinidosExtra;

            if (OntologiaGnoss.RDFCVSemIncluido != null)
            {
                //Agrego el namespace del CV semántico:
                OntologiaGnoss.NamespacesDefinidos.Add(UrlOntologiaCurriculum, GestionOWL.NAMESPACE_ONTO_HR_XML);

                if (mUrlOntologiaPlantillaRDF != null && !OntologiaGnoss.NamespacesDefinidos.ContainsKey(mUrlOntologiaPlantillaRDF))
                {
                    //Agrego el namespace para la ontología-plantilla a mostrar
                    if (mNamespacePlantillaRDF == null)
                    {
                        OntologiaGnoss.NamespacesDefinidos.Add(mUrlOntologiaPlantillaRDF, GestionOWL.NAMESPACE_ONTO_GNOSS);
                        OntologiaGnoss.NamespacesDefinidosInv.Add(GestionOWL.NAMESPACE_ONTO_GNOSS, mUrlOntologiaPlantillaRDF);
                    }
                    else
                    {
                        OntologiaGnoss.NamespacesDefinidos.Add(mUrlOntologiaPlantillaRDF, mNamespacePlantillaRDF);
                        OntologiaGnoss.NamespacesDefinidosInv.Add(mNamespacePlantillaRDF, mUrlOntologiaPlantillaRDF);
                    }
                }
            }

            if (!OntologiaGnoss.NamespacesDefinidos.ContainsKey(GestionOWL.URL_LICENCIA_CC))
            {
                OntologiaGnoss.NamespacesDefinidos.Add(GestionOWL.URL_LICENCIA_CC, GestionOWL.NAMESPACE_LICENCIA_CC);
            }
            //Decidir cuando viene de RDF
            Stream stream = gestionOWL.PasarOWL(null, OntologiaGnoss, pEntidad, pEntidad.EntidadesRelacionadas, new Dictionary<string, string>(), pTipoElementoExportar, pBusqueda);
            stream.Position = 0;

            BinaryReader sw = new BinaryReader(stream);

            byte[] byteArray = sw.ReadBytes((int)sw.BaseStream.Length);
            sw.Close();

            stream.Flush();

            return byteArray;
        }

    }
}
