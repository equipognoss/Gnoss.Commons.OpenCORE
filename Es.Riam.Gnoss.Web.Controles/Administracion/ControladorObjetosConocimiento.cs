using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Documentacion;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Tesauro;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Semantica.OWL;
using Es.Riam.Semantica.Plantillas;
using Es.Riam.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static Es.Riam.Gnoss.Web.MVC.Models.Administracion.ObjetoConocimientoModel;

namespace Es.Riam.Gnoss.Web.Controles.Administracion
{
    public class ControladorObjetosConocimiento
    {
        private DataWrapperProyecto mDataWrapperProyecto;
        private Elementos.ServiciosGenerales.Proyecto ProyectoSeleccionado = null;
        private Dictionary<string, string> ParametroProyecto = null;

        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private RedisCacheWrapper mRedisCacheWrapper;
        private GnossCache mGnossCache;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        /// <summary>
        /// DataSet de proyecto con la configuración semántica.
        /// </summary>
        private DataWrapperProyecto mProyectoConfigSemDataWrapperProyecto;

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public ControladorObjetosConocimiento(Elementos.ServiciosGenerales.Proyecto pProyecto, Dictionary<string, string> pParametroProyecto, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ControladorObjetosConocimiento> logger, ILoggerFactory loggerFactory)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mGnossCache = gnossCache;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            ProyectoSeleccionado = pProyecto;
            ParametroProyecto = pParametroProyecto;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #endregion

        #region Metodos de Carga

        public ObjetoConocimientoModel CargarObjetoConocimiento(string pNombreOntologia)
        {
            ObjetoConocimientoModel resultado = null;
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
            using (DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory))
            {
                mLoggingService.AgregarEntrada($"EliminarObjetoConocimiento - Buscando Objeto conocimiento en BD");
                documentacionCN.ObtenerDatasetConOntologiaAPartirNombre(ProyectoSeleccionado.Clave, pNombreOntologia, dataWrapperDocumentacion);
                if (dataWrapperDocumentacion.ListaDocumento.Count != 0)
                {
                    mLoggingService.AgregarEntrada($"EliminarObjetoConocimiento - Se ha encontrado un objeto de conocimiento");
                    resultado = CargarObjetoConocimiento(dataWrapperDocumentacion.ListaDocumento.FirstOrDefault());
                    mLoggingService.AgregarEntrada($"EliminarObjetoConocimiento - Se ha cargado el objeto de conocimiento {resultado.Name}");
                }
            }
            return resultado;
        }

        public ObjetoConocimientoModel CargarObjetoConocimiento(AD.EntityModel.Models.Documentacion.Documento filaDoc, bool pCargarCompartidas = false)
        {
            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);

            string ontologiaProyecto = filaDoc.Enlace.Replace(".owl", "");
            OntologiaProyecto filaOntologia = DataWrapperProyecto.ListaOntologiaProyecto.FirstOrDefault(onto => onto.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && onto.ProyectoID.Equals(ProyectoSeleccionado.Clave) && onto.OntologiaProyecto1.Equals(ontologiaProyecto, StringComparison.InvariantCultureIgnoreCase));

            Guid ontologiaID = filaDoc.DocumentoID;

            if (filaOntologia != null)
            {
                TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<TesauroCN>(), mLoggerFactory);

                ObjetoConocimientoModel objetoConocimiento = new ObjetoConocimientoModel();
                objetoConocimiento.DocumentoID = ontologiaID;
                objetoConocimiento.Ontologia = filaOntologia.OntologiaProyecto1;
                objetoConocimiento.Name = filaOntologia.NombreOnt;
                objetoConocimiento.ShortNameOntology = filaOntologia.NombreCortoOnt;
                objetoConocimiento.Namespace = filaOntologia.Namespace;
                objetoConocimiento.NamespaceExtra = filaOntologia.NamespacesExtra;
                objetoConocimiento.RecursosVinculados = documentacionCN.ExistenVinculadosAOntologiaProyecto(ontologiaID, ProyectoSeleccionado.Clave);
                objetoConocimiento.NombreTesauroExclusivo = tesauroCN.ObtenerNombreTesauroProyOnt(ProyectoSeleccionado.Clave, ontologiaID.ToString());
                objetoConocimiento.CachearDatosSemanticos = filaOntologia.CachearDatosSemanticos;
                objetoConocimiento.EsBuscable = filaOntologia.EsBuscable;
                objetoConocimiento.GrafoActual = filaDoc.Enlace;
                objetoConocimiento.FechaCreacion = filaDoc.FechaCreacion ?? DateTime.Parse("01/01/2024");
                objetoConocimiento.FechaModificacion = filaDoc.FechaModificacion ?? DateTime.Parse("01/01/2024");
                if (!string.IsNullOrEmpty(filaDoc.NombreCategoriaDoc) && filaDoc.NombreCategoriaDoc.Contains(".jpg"))
                {
                    objetoConocimiento.Image = filaDoc.NombreCategoriaDoc.Split(',')[1].Replace(".jpg", "_" + filaDoc.NombreCategoriaDoc.Split(',')[0] + ".jpg");
                }

                objetoConocimiento.Subtipos = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(filaOntologia.SubTipos))
                {
                    foreach (string datosSubTipo in filaOntologia.SubTipos.Split(new string[] { "[|||]" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string subTipo = datosSubTipo.Substring(0, datosSubTipo.IndexOf("|||"));
                        string nombre = datosSubTipo.Substring(datosSubTipo.IndexOf("|||") + 3);
                        if (!objetoConocimiento.Subtipos.ContainsKey(subTipo))
                        {
                            objetoConocimiento.Subtipos.Add(subTipo, nombre);
                        }
                    }
                }

                objetoConocimiento.EsObjetoPrimario = true;
                objetoConocimiento.PresentacionListado = CargarPresentacionListado(ontologiaID);
                objetoConocimiento.PresentacionMosaico = CargarPresentacionMosaico(ontologiaID);
                objetoConocimiento.PresentacionMapa = CargarPresentacionMapa(ontologiaID);
                objetoConocimiento.PresentacionRelacionados = CargarPresentacionRecRelacionados(ontologiaID);
                objetoConocimiento.PresentacionPersonalizado = CargarPresentacionPersonalizado(ontologiaID);
                objetoConocimiento.ListaAutocompletadoPresentacionModel = CargarAutocompletadoDePropiedades(ontologiaID);
                return objetoConocimiento;
            }
            else if (filaDoc.Tipo == (short)TiposDocumentacion.OntologiaSecundaria)
            {
                ObjetoConocimientoModel objetoConocimiento = new ObjetoConocimientoModel();
                objetoConocimiento.DocumentoID = ontologiaID;
                objetoConocimiento.Name = filaDoc.Titulo.Replace(".owl", string.Empty);
                objetoConocimiento.EsObjetoPrimario = false;
                objetoConocimiento.Ontologia = filaDoc.Enlace.Replace(".owl", string.Empty);
                objetoConocimiento.GrafoActual = filaDoc.Enlace;
                objetoConocimiento.FechaCreacion = filaDoc.FechaCreacion ?? DateTime.Parse("01/01/2024"); ;
                objetoConocimiento.FechaModificacion = filaDoc.FechaModificacion ?? DateTime.Parse("01/01/2024");

                return objetoConocimiento;
            }
            else if (pCargarCompartidas && filaDoc.Tipo == (short)TiposDocumentacion.Ontologia)
            {
				ObjetoConocimientoModel objetoConocimiento = new ObjetoConocimientoModel();
				objetoConocimiento.DocumentoID = ontologiaID;
				objetoConocimiento.Name = filaDoc.Titulo.Replace(".owl", string.Empty);
				objetoConocimiento.EsObjetoPrimario = true;
				objetoConocimiento.Ontologia = filaDoc.Enlace.Replace(".owl", string.Empty);
				objetoConocimiento.GrafoActual = filaDoc.Enlace;
				objetoConocimiento.FechaCreacion = filaDoc.FechaCreacion ?? DateTime.Parse("01/01/2024"); ;
				objetoConocimiento.FechaModificacion = filaDoc.FechaModificacion ?? DateTime.Parse("01/01/2024");
                objetoConocimiento.EsOntologiaCompartida = true;

                return objetoConocimiento;
			}

            return null;
        }

        private ObjetoConocimientoModel.PresentacionModel CargarPresentacionListado(Guid pOntologiaID)
        {
            ObjetoConocimientoModel.PresentacionModel PresentacionListado = new ObjetoConocimientoModel.PresentacionModel();

            PresentacionListado.ListaPropiedades = new List<ObjetoConocimientoModel.PresentacionModel.PropiedadModel>();

            foreach (PresentacionListadoSemantico filaListadoSem in DataWrapperProyecto.ListaPresentacionListadoSemantico.Where(presentacion => presentacion.OntologiaID == pOntologiaID))
            {
                if (string.IsNullOrEmpty(filaListadoSem.Ontologia))
                {
                    if (filaListadoSem.Propiedad == "descripcion")
                    {
                        PresentacionListado.MostrarDescripcion = true;
                    }
                    else if (filaListadoSem.Propiedad == "publicador")
                    {
                        PresentacionListado.MostrarPublicador = true;
                    }
                    else if (filaListadoSem.Propiedad == "etiquetas")
                    {
                        PresentacionListado.MostrarEtiquetas = true;
                    }
                    else if (filaListadoSem.Propiedad == "categorias")
                    {
                        PresentacionListado.MostrarCategorias = true;
                    }
                }
                else
                {
                    ObjetoConocimientoModel.PresentacionModel.PropiedadModel propiedad = new ObjetoConocimientoModel.PresentacionModel.PropiedadModel();
                    propiedad.Propiedad = filaListadoSem.Propiedad;
                    propiedad.Presentacion = filaListadoSem.Nombre;
                    propiedad.Orden = filaListadoSem.Orden;
                    PresentacionListado.ListaPropiedades.Add(propiedad);
                }
            }
            return PresentacionListado;
        }

        private ObjetoConocimientoModel.PresentacionModel CargarPresentacionMosaico(Guid pOntologiaID)
        {
            ObjetoConocimientoModel.PresentacionModel PresentacionMosaico = new ObjetoConocimientoModel.PresentacionModel();

            PresentacionMosaico.ListaPropiedades = new List<ObjetoConocimientoModel.PresentacionModel.PropiedadModel>();

            foreach (PresentacionMosaicoSemantico filaMosaicoSem in DataWrapperProyecto.ListaPresentacionMosaicoSemantico.Where(presentacion => presentacion.OntologiaID == pOntologiaID))
            {
                if (string.IsNullOrEmpty(filaMosaicoSem.Ontologia))
                {
                    if (filaMosaicoSem.Propiedad == "descripcion")
                    {
                        PresentacionMosaico.MostrarDescripcion = true;
                    }
                    else if (filaMosaicoSem.Propiedad == "publicador")
                    {
                        PresentacionMosaico.MostrarPublicador = true;
                    }
                    else if (filaMosaicoSem.Propiedad == "etiquetas")
                    {
                        PresentacionMosaico.MostrarEtiquetas = true;
                    }
                    else if (filaMosaicoSem.Propiedad == "categorias")
                    {
                        PresentacionMosaico.MostrarCategorias = true;
                    }
                }
                else
                {
                    ObjetoConocimientoModel.PresentacionModel.PropiedadModel propiedad = new ObjetoConocimientoModel.PresentacionModel.PropiedadModel();
                    propiedad.Propiedad = filaMosaicoSem.Propiedad;
                    propiedad.Presentacion = filaMosaicoSem.Nombre;
                    propiedad.Orden = filaMosaicoSem.Orden;
                    PresentacionMosaico.ListaPropiedades.Add(propiedad);
                }
            }
            return PresentacionMosaico;
        }
        private List<ObjetoConocimientoModel.AutocompletadoPresentacionModel> CargarAutocompletadoDePropiedades(Guid pOntologiaID)
        {
            List<ObjetoConocimientoModel.AutocompletadoPresentacionModel> autocompletadoPresentacionModels = new List<ObjetoConocimientoModel.AutocompletadoPresentacionModel>();
            var listaPresentaciones = DataWrapperProyecto.ListaPresentacionMapaSemantico.Where(presentacion => presentacion.OntologiaID == pOntologiaID).Select(item => new { item.Propiedad, item.MostrarEnAutocompletar})
                .Union(DataWrapperProyecto.ListaPresentacionListadoSemantico.Where(presentacion => presentacion.OntologiaID == pOntologiaID).Select(item => new { item.Propiedad, item.MostrarEnAutocompletar }))
                .Union(DataWrapperProyecto.ListaPresentacionMosaicoSemantico.Where(presentacion => presentacion.OntologiaID == pOntologiaID).Select(item => new { item.Propiedad, item.MostrarEnAutocompletar })).Distinct().ToList();
            foreach(var presentacion in listaPresentaciones)
            {
                ObjetoConocimientoModel.AutocompletadoPresentacionModel autocompletadoPresentacionModel = new ObjetoConocimientoModel.AutocompletadoPresentacionModel()
                {
                    MostrarEnAutocompletar = presentacion.MostrarEnAutocompletar,
                    Propiedad = presentacion.Propiedad
                };
				autocompletadoPresentacionModels.Add(autocompletadoPresentacionModel);
			}

			return autocompletadoPresentacionModels;
		}

		private ObjetoConocimientoModel.PresentacionPersonalizadoModel CargarPresentacionPersonalizado(Guid pOntologiaID)
        {
            ObjetoConocimientoModel.PresentacionPersonalizadoModel PresentacionPersonalizado = new ObjetoConocimientoModel.PresentacionPersonalizadoModel();

            PresentacionPersonalizado.ListaPropiedades = new List<ObjetoConocimientoModel.PresentacionPersonalizadoModel.PropiedadPersonalizadoModel>();

            foreach (PresentacionPersonalizadoSemantico filaPresentacionPersonalizado in DataWrapperProyecto.ListaPresentacionPersonalizadoSemantico.Where(presentacion => presentacion.OntologiaID == pOntologiaID))
            {
                ObjetoConocimientoModel.PresentacionPersonalizadoModel.PropiedadPersonalizadoModel propiedad = new ObjetoConocimientoModel.PresentacionPersonalizadoModel.PropiedadPersonalizadoModel();
                propiedad.Identificador = filaPresentacionPersonalizado.ID;
                propiedad.Select = filaPresentacionPersonalizado.Select;
                propiedad.Where = filaPresentacionPersonalizado.Where;
                propiedad.Orden = filaPresentacionPersonalizado.Orden;
                PresentacionPersonalizado.ListaPropiedades.Add(propiedad);
            }
            return PresentacionPersonalizado;
        }

        private ObjetoConocimientoModel.PresentacionModel CargarPresentacionMapa(Guid pOntologiaID)
        {
            ObjetoConocimientoModel.PresentacionModel PresentacionMapa = new ObjetoConocimientoModel.PresentacionModel();

            PresentacionMapa.ListaPropiedades = new List<ObjetoConocimientoModel.PresentacionModel.PropiedadModel>();

            foreach (PresentacionMapaSemantico filaMapaSem in DataWrapperProyecto.ListaPresentacionMapaSemantico.Where(presentacion => presentacion.OntologiaID == pOntologiaID))
            {
                if (string.IsNullOrEmpty(filaMapaSem.Ontologia))
                {
                    if (filaMapaSem.Propiedad == "descripcion")
                    {
                        PresentacionMapa.MostrarDescripcion = true;
                    }
                    else if (filaMapaSem.Propiedad == "publicador")
                    {
                        PresentacionMapa.MostrarPublicador = true;
                    }
                    else if (filaMapaSem.Propiedad == "etiquetas")
                    {
                        PresentacionMapa.MostrarEtiquetas = true;
                    }
                    else if (filaMapaSem.Propiedad == "categorias")
                    {
                        PresentacionMapa.MostrarCategorias = true;
                    }
                }
                else
                {
                    ObjetoConocimientoModel.PresentacionModel.PropiedadModel propiedad = new ObjetoConocimientoModel.PresentacionModel.PropiedadModel();
                    propiedad.Propiedad = filaMapaSem.Propiedad;
                    propiedad.Presentacion = filaMapaSem.Nombre;
                    propiedad.Orden = filaMapaSem.Orden;
                    PresentacionMapa.ListaPropiedades.Add(propiedad);
                }
            }
            return PresentacionMapa;
        }

        private ObjetoConocimientoModel.PresentacionModel CargarPresentacionRecRelacionados(Guid pOntologiaID)
        {
            ObjetoConocimientoModel.PresentacionModel PresentacionRelacionados = new ObjetoConocimientoModel.PresentacionModel();

            PresentacionRelacionados.ListaPropiedades = new List<ObjetoConocimientoModel.PresentacionModel.PropiedadModel>();

            foreach (RecursosRelacionadosPresentacion filaRecRelacionado in DataWrapperProyecto.ListaRecursosRelacionadosPresentacion.Where(presentacion => presentacion.OntologiaID == pOntologiaID))
            {
                if (string.IsNullOrEmpty(filaRecRelacionado.Ontologia))
                {
                    if (filaRecRelacionado.Propiedad == "descripcion")
                    {
                        PresentacionRelacionados.MostrarDescripcion = true;
                    }
                    else if (filaRecRelacionado.Propiedad == "publicador")
                    {
                        PresentacionRelacionados.MostrarPublicador = true;
                    }
                    else if (filaRecRelacionado.Propiedad == "etiquetas")
                    {
                        PresentacionRelacionados.MostrarEtiquetas = true;
                    }
                    else if (filaRecRelacionado.Propiedad == "categorias")
                    {
                        PresentacionRelacionados.MostrarCategorias = true;
                    }
                }
                else
                {
                    ObjetoConocimientoModel.PresentacionModel.PropiedadModel propiedad = new ObjetoConocimientoModel.PresentacionModel.PropiedadModel();
                    propiedad.Propiedad = filaRecRelacionado.Propiedad;
                    propiedad.Presentacion = filaRecRelacionado.Nombre;
                    propiedad.Orden = filaRecRelacionado.Orden;
                    PresentacionRelacionados.ListaPropiedades.Add(propiedad);
                }
            }
            return PresentacionRelacionados;
        }

        #endregion



        #region Métodos de guardado

        /// <summary>
        /// Crea un nuevo objeto de conocimiento
        /// </summary>
        /// <returns>ActionResult</returns>        
        public ObjetoConocimientoModel NuevoObjetoConocimento(string ontology)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            documentacionCN.ObtenerOntologiasProyecto(ProyectoSeleccionado.Clave, dataWrapperDocumentacion, false, true, true);
            documentacionCN.Dispose();

            AD.EntityModel.Models.Documentacion.Documento filaDoc = dataWrapperDocumentacion.ListaDocumento.FirstOrDefault(doc => (doc.Tipo == (short)TiposDocumentacion.Ontologia || doc.Tipo == (short)TiposDocumentacion.OntologiaSecundaria) && doc.Enlace.ToLower().Equals(ontology.ToLower()));
            Dictionary<string, List<EstiloPlantilla>> listaEstilos = new Dictionary<string, List<EstiloPlantilla>>();
            ControladorDocumentacion controladorDocumentacion = new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, null, mVirtuosoAD, null, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDocumentacion>(), mLoggerFactory);
            byte[] arrayOnto = controladorDocumentacion.ObtenerOntologia(filaDoc.DocumentoID, out listaEstilos, filaDoc.ProyectoID.Value, null, null, false);
            Ontologia ontologia = new Ontologia(arrayOnto, true);
            ontologia.LeerOntologia();

            ObjetoConocimientoModel objetoConocimiento = null;
            if (filaDoc != null)
            {
                objetoConocimiento = new ObjetoConocimientoModel();
                objetoConocimiento.Ontologia = ontology.Replace(".owl", string.Empty).ToLower();
                objetoConocimiento.Name = filaDoc.Titulo.Replace(".owl", string.Empty);
                objetoConocimiento.EsCreacion = true;
                objetoConocimiento.CachearDatosSemanticos = true;
                objetoConocimiento.EsBuscable = true;
                objetoConocimiento.GrafoActual = filaDoc.Enlace;
                objetoConocimiento.Subtipos = new Dictionary<string, string>();
                objetoConocimiento.PresentacionListado = new PresentacionModel();
                objetoConocimiento.PresentacionListado.ListaPropiedades = new List<PresentacionModel.PropiedadModel>();
                objetoConocimiento.PresentacionMosaico = new PresentacionModel();
                objetoConocimiento.PresentacionMosaico.ListaPropiedades = new List<PresentacionModel.PropiedadModel>();
                objetoConocimiento.PresentacionMapa = new PresentacionModel();
                objetoConocimiento.PresentacionMapa.ListaPropiedades = new List<PresentacionModel.PropiedadModel>();
                objetoConocimiento.PresentacionRelacionados = new PresentacionModel();
                objetoConocimiento.PresentacionRelacionados.ListaPropiedades = new List<PresentacionModel.PropiedadModel>();
                objetoConocimiento.PresentacionPersonalizado = new PresentacionPersonalizadoModel();
                objetoConocimiento.PresentacionPersonalizado.ListaPropiedades = new List<PresentacionPersonalizadoModel.PropiedadPersonalizadoModel>();
                objetoConocimiento.ListaAutocompletadoPresentacionModel = new List<AutocompletadoPresentacionModel>();

                if (!string.IsNullOrEmpty(filaDoc.NombreCategoriaDoc) && filaDoc.NombreCategoriaDoc.Contains(".jpg"))
                {
                    objetoConocimiento.Image = filaDoc.NombreCategoriaDoc.Split(',')[1].Replace(".jpg", $"_{filaDoc.NombreCategoriaDoc.Split(',')[0]}.jpg");
                }

                Dictionary<string, string> namespaces = ontologia.NamespacesDefinidos;
                string valor = string.Empty;
                string namespaceExtra = string.Empty;
                string namespacesFinal = string.Empty;
                foreach (string clave in namespaces.Keys)
                {
                    namespaces.TryGetValue(clave, out valor);
                    if (!valor.Equals("rdf") && !valor.Equals("xsd") && !valor.Equals("rdfs") && !valor.Equals("owl") && !valor.Equals("base") && !string.IsNullOrEmpty(valor))
                    {
                        namespaceExtra = $"{valor}:{clave}";
                        if (string.IsNullOrEmpty(namespacesFinal))
                        {
                            namespacesFinal = $"{namespaceExtra}";
                        }
                        else
                        {
                            namespacesFinal = $"{namespacesFinal}|{namespaceExtra}";
                        }
                    }
                }
                objetoConocimiento.NamespaceExtra = namespacesFinal;
                objetoConocimiento.Namespace = objetoConocimiento.Name;
                AgregarObjetoConocimientoNuevo(filaDoc.DocumentoID, objetoConocimiento);
            }

            return objetoConocimiento;
        }

        /// <summary>
        /// Añade permisos para editar la ontología recien creada al administrador
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento de la ontología</param>
        public void AgregarPermisosAdministradorOntologia(Guid pDocumentoID)
        {
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);

            if (!proyectoCN.ExisteTipoDocDispRolUsuarioProySemantico(ProyectoSeleccionado.FilaProyecto.ProyectoID))
            {
                TipoDocDispRolUsuarioProy permisoEditDocsSemasAdmin = new TipoDocDispRolUsuarioProy();
                permisoEditDocsSemasAdmin.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                permisoEditDocsSemasAdmin.ProyectoID = ProyectoSeleccionado.FilaProyecto.ProyectoID;
                permisoEditDocsSemasAdmin.TipoDocumento = (short)TiposDocumentacion.Semantico;
                permisoEditDocsSemasAdmin.RolUsuario = (short)TipoRolUsuario.Administrador;

                mEntityContext.TipoDocDispRolUsuarioProy.Add(permisoEditDocsSemasAdmin);
            }

            if (!proyectoCN.ExisteTipoOntoDispRolUsuarioProy(ProyectoSeleccionado.FilaProyecto.ProyectoID, pDocumentoID))
            {
                TipoOntoDispRolUsuarioProy permisoEditOntoAdmin = new TipoOntoDispRolUsuarioProy();
                permisoEditOntoAdmin.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                permisoEditOntoAdmin.ProyectoID = ProyectoSeleccionado.FilaProyecto.ProyectoID;
                permisoEditOntoAdmin.OntologiaID = pDocumentoID;
                permisoEditOntoAdmin.RolUsuario = (short)TipoRolUsuario.Administrador;

                mEntityContext.TipoOntoDispRolUsuarioProy.Add(permisoEditOntoAdmin);
            }

            mEntityContext.SaveChanges();
        }

        public void GuardarObjetoConocimiento(ObjetoConocimientoModel pObjetoConocimiento)
        {
            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            OntologiaProyecto ontologiaProyecto = documentacionCN.ObtenerOntologiaProyectoPorOntologia(pObjetoConocimiento.Ontologia, ProyectoSeleccionado.Clave);

            if (pObjetoConocimiento.GrafoNuevo != pObjetoConocimiento.GrafoActual || !pObjetoConocimiento.EsObjetoPrimario)
            {
                AD.EntityModel.Models.Documentacion.Documento ontologia = documentacionCN.ObtenerDocumentoPorID(pObjetoConocimiento.DocumentoID).ListaDocumento.FirstOrDefault();

                if (documentacionCN.ObtenerDocumentosIDVinculadosAOntologiaProyecto(pObjetoConocimiento.DocumentoID, ProyectoSeleccionado.Clave).Count <= 0 && pObjetoConocimiento.GrafoNuevo != pObjetoConocimiento.GrafoActual)
                {
                    ontologia.Enlace = pObjetoConocimiento.GrafoNuevo;
                    if (documentacionCN.ObtenerOntologiaProyectoPorOntologia(pObjetoConocimiento.GrafoNuevo, ProyectoSeleccionado.Clave) != null)
                    {
                        ontologiaProyecto.OntologiaProyecto1 = pObjetoConocimiento.GrafoNuevo.Replace(".owl", "");
                    }
                    else
                    {
                        throw new Exception("Ya existe una ontología apuntando al grafo indicado");
                    }
                }
                else
                {
                    ontologia.Titulo = pObjetoConocimiento.Name;
                    mEntityContext.SaveChanges();
                }
            }

            if (pObjetoConocimiento.EsObjetoPrimario)
            {
                GuardarDatosObjetoConocimiento(pObjetoConocimiento.DocumentoID, ontologiaProyecto, pObjetoConocimiento);
            }
        }

        public void GuardarObjetosConocimiento(List<ObjetoConocimientoModel> pListaObjetosConocimiento)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            Guid proyectoIDPatronOntologias = Guid.Empty;
            if (ParametroProyecto.ContainsKey("ProyectoIDPatronOntologias"))
            {
                Guid.TryParse(ParametroProyecto["ProyectoIDPatronOntologias"], out proyectoIDPatronOntologias);
                documentacionCN.ObtenerOntologiasProyecto(proyectoIDPatronOntologias, dataWrapperDocumentacion, false, false, true);
            }
            documentacionCN.ObtenerOntologiasProyecto(ProyectoSeleccionado.Clave, dataWrapperDocumentacion, false, false, true);
            documentacionCN.Dispose();

            List<AD.EntityModel.Models.Documentacion.Documento> filasDoc = dataWrapperDocumentacion.ListaDocumento.Where(documento => documento.Tipo.Equals((short)TiposDocumentacion.Ontologia)).ToList();

            Dictionary<string, Guid> listaObjetosConocimiento = new Dictionary<string, Guid>();
            List<Guid> listaObjetosConocimientoNuevos = new List<Guid>();

            //Añadir los nuevos
            foreach (ObjetoConocimientoModel objetoConocimiento in pListaObjetosConocimiento)
            {
                AD.EntityModel.Models.Documentacion.Documento filaOC = filasDoc.FirstOrDefault(doc => doc.Enlace.ToLower().Equals(objetoConocimiento.Ontologia.ToLower() + ".owl"));

                if (filaOC != null)
                {
                    //objetoConocimiento.Ontologia = filasOC[0].Enlace.Replace(".owl", "");

                    Guid ontologiaID = filaOC.DocumentoID;
                    if (!listaObjetosConocimiento.ContainsKey(objetoConocimiento.Ontologia))
                    {
                        listaObjetosConocimiento.Add(objetoConocimiento.Ontologia, ontologiaID);
                    }
                    else
                    {
                        string mensaje = $"Hay Objetos de conocimiento reptidos. La ontologia Repetida es: {objetoConocimiento.Ontologia}";
                        throw new ExcepcionGeneral(mensaje);
                    }

                    if (!objetoConocimiento.Deleted)
                    {
                        OntologiaProyecto filaOntologia = DataWrapperProyecto.ListaOntologiaProyecto.FirstOrDefault(onto => onto.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && onto.ProyectoID.Equals(ProyectoSeleccionado.Clave) && onto.OntologiaProyecto1.Equals(objetoConocimiento.Ontologia));
                        if (filaOntologia == null)
                        {
                            listaObjetosConocimientoNuevos.Add(ontologiaID);

                            AgregarObjetoConocimientoNuevo(ontologiaID, objetoConocimiento);
                        }
                    }
                }
            }

            //Modificar los que tienen cambios
            foreach (ObjetoConocimientoModel objetoConocimiento in pListaObjetosConocimiento)
            {
                if (listaObjetosConocimiento.ContainsKey(objetoConocimiento.Ontologia))
                {
                    Guid ontologiaID = listaObjetosConocimiento[objetoConocimiento.Ontologia];

                    if (!objetoConocimiento.Deleted && !listaObjetosConocimientoNuevos.Contains(ontologiaID))
                    {
                        OntologiaProyecto filaOntologia = DataWrapperProyecto.ListaOntologiaProyecto.FirstOrDefault(onto => onto.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && onto.ProyectoID.Equals(ProyectoSeleccionado.Clave) && onto.OntologiaProyecto1.Equals(objetoConocimiento.Ontologia));

                        GuardarDatosObjetoConocimiento(ontologiaID, filaOntologia, objetoConocimiento);
                    }
                }
            }

            List<OntologiaProyecto> filasNoEliminadas = DataWrapperProyecto.ListaOntologiaProyecto.Where(fila => mEntityContext.Entry(fila).State != EntityState.Deleted).ToList();

            //Eliminar los que no se encuentran
            foreach (OntologiaProyecto filaOntologia in filasNoEliminadas)
            {
                if (!pListaObjetosConocimiento.Any(objetoConocimiento => objetoConocimiento.Ontologia.Equals(filaOntologia.OntologiaProyecto1)))
                {
                    AD.EntityModel.Models.Documentacion.Documento filaDocOntologia = filasDoc.FirstOrDefault(doc => doc.Enlace.ToLower().Equals(filaOntologia.OntologiaProyecto1.ToLower() + ".owl"));

                    if (filaDocOntologia != null)
                    {
                        EliminarObjetoConocimiento(filaDocOntologia.DocumentoID, filaOntologia);
                    }
                }
            }

            using (ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory))
            {
                proyCN.ActualizarProyectos();
            }
        }

        public void AgregarObjetoConocimientoNuevo(Guid pOntologiaID, ObjetoConocimientoModel pObjetoConocimiento)
        {
            OntologiaProyecto filaOntologiaNueva = new OntologiaProyecto();
            filaOntologiaNueva.ProyectoID = ProyectoSeleccionado.Clave;
            filaOntologiaNueva.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
            filaOntologiaNueva.OntologiaProyecto1 = pObjetoConocimiento.Ontologia;
            filaOntologiaNueva.EsBuscable = pObjetoConocimiento.EsBuscable;
            filaOntologiaNueva.CachearDatosSemanticos = pObjetoConocimiento.CachearDatosSemanticos;
            filaOntologiaNueva.Namespace = pObjetoConocimiento.Name;

            GuardarDatosObjetoConocimiento(pOntologiaID, filaOntologiaNueva, pObjetoConocimiento);

            if (mEntityContext.OntologiaProyecto.FirstOrDefault(onto => onto.OrganizacionID.Equals(filaOntologiaNueva.OrganizacionID) && onto.ProyectoID.Equals(filaOntologiaNueva.ProyectoID) && onto.OntologiaProyecto1.Equals(filaOntologiaNueva.OntologiaProyecto1)) == null)
            {
                DataWrapperProyecto.ListaOntologiaProyecto.Add(filaOntologiaNueva);
                mEntityContext.OntologiaProyecto.Add(filaOntologiaNueva);
            }
            /*
            TipoOntoDispRolUsuarioProy tolUsuario = new TipoOntoDispRolUsuarioProy();
            tolUsuario.OntologiaID = pOntologiaID;
            tolUsuario.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
            tolUsuario.ProyectoID = ProyectoSeleccionado.Clave;
            tolUsuario.RolUsuario = (short)UserRol.Administrator;
            if (!mEntityContext.TipoOntoDispRolUsuarioProy.Any(tipo => tipo.OrganizacionID.Equals(tolUsuario.OrganizacionID) && tipo.ProyectoID.Equals(tolUsuario.ProyectoID) && tipo.OntologiaID.Equals(tolUsuario.OntologiaID)))
            {
                DataWrapperProyecto.ListaTipoOntoDispRolUsuarioProy.Add(tolUsuario);
                mEntityContext.TipoOntoDispRolUsuarioProy.Add(tolUsuario);
            }*/
        }

        private void GuardarDatosObjetoConocimiento(Guid pOntologiaID, OntologiaProyecto pFilaOntologia, ObjetoConocimientoModel pObjetoConocimiento)
        {
            pFilaOntologia.NombreOnt = pObjetoConocimiento.Name;
            pFilaOntologia.NombreCortoOnt = string.IsNullOrEmpty(pObjetoConocimiento.ShortNameOntology) ? string.Empty : pObjetoConocimiento.ShortNameOntology;
            pFilaOntologia.Namespace = pObjetoConocimiento.Namespace;
            pFilaOntologia.NamespacesExtra = string.IsNullOrEmpty(pObjetoConocimiento.NamespaceExtra) ? string.Empty : pObjetoConocimiento.NamespaceExtra;
            pFilaOntologia.CachearDatosSemanticos = pObjetoConocimiento.CachearDatosSemanticos;
            pFilaOntologia.EsBuscable = pObjetoConocimiento.EsBuscable;

            if (pObjetoConocimiento.Subtipos != null)
            {
                pFilaOntologia.SubTipos = string.Empty;
                foreach (string subTipo in pObjetoConocimiento.Subtipos.Keys.Distinct())
                {
                    pFilaOntologia.SubTipos += $"{subTipo}|||{pObjetoConocimiento.Subtipos[subTipo]}[|||]";
                }
            }
            else
            {
                pFilaOntologia.SubTipos = string.Empty;
            }

            GuardarDatosDocumento(pOntologiaID, pObjetoConocimiento);
            GuardarDatosPresentacionListado(pOntologiaID, pObjetoConocimiento);
            GuardarDatosPresentacionMosaico(pOntologiaID, pObjetoConocimiento);
            GuardarDatosPresentacionMapa(pOntologiaID, pObjetoConocimiento);
            GuardarDatosPresentacionPersonalizado(pOntologiaID, pObjetoConocimiento);
            GuardarDatosPresentacionRelacionado(pOntologiaID, pObjetoConocimiento);
            GuardarDatosPropiedadesAutocompletar(pOntologiaID, pObjetoConocimiento);
            GuardarCambios();
        }

        private void GuardarDatosPropiedadesAutocompletar(Guid pOntologiaID, ObjetoConocimientoModel pObjetoConocimientoModel)
		{
            if(pObjetoConocimientoModel.ListaAutocompletadoPresentacionModel == null)
            {
                return;
            }

            foreach (AutocompletadoPresentacionModel propiedadAutocompletado in pObjetoConocimientoModel.ListaAutocompletadoPresentacionModel) 
            {
				PresentacionListadoSemantico presentacionListado = DataWrapperProyecto.ListaPresentacionListadoSemantico.FirstOrDefault(presentacion => presentacion.OntologiaID.Equals(pOntologiaID) && presentacion.Propiedad.Equals(propiedadAutocompletado.Propiedad));
                if(presentacionListado != null)
                {
                    if (presentacionListado.MostrarEnAutocompletar != propiedadAutocompletado.MostrarEnAutocompletar)
                    {
                        presentacionListado.MostrarEnAutocompletar = propiedadAutocompletado.MostrarEnAutocompletar;
                    }
				}
                else
                {
					PresentacionMapaSemantico presentacionMapa = DataWrapperProyecto.ListaPresentacionMapaSemantico.FirstOrDefault(presentacion => presentacion.OntologiaID.Equals(pOntologiaID) && presentacion.Propiedad.Equals(propiedadAutocompletado.Propiedad));
                    if(presentacionMapa != null)
                    {
                        if (presentacionMapa.MostrarEnAutocompletar != propiedadAutocompletado.MostrarEnAutocompletar)
                        {
                            presentacionMapa.MostrarEnAutocompletar = propiedadAutocompletado.MostrarEnAutocompletar;
                        }
					}
                    else
                    {
						PresentacionMosaicoSemantico presentacionMosaico = DataWrapperProyecto.ListaPresentacionMosaicoSemantico.FirstOrDefault(presentacion => presentacion.OntologiaID.Equals(pOntologiaID) && presentacion.Propiedad.Equals(propiedadAutocompletado.Propiedad));
						if (presentacionMosaico != null)
						{
                            if (presentacionMosaico.MostrarEnAutocompletar != propiedadAutocompletado.MostrarEnAutocompletar)
                            {
                                presentacionMosaico.MostrarEnAutocompletar = propiedadAutocompletado.MostrarEnAutocompletar;
                            }
						}
					}
				}
            }
		}

		/// <summary>
		/// Aplicamos los cambios hechos en la administración a la tabla Documento (solamente es necesario el título)
		/// </summary>
		/// <param name="pOntologiaID">Identificador de la ontología</param>
		/// <param name="pObjetoConocimientoModel">Modelo con los cambios devuelto por la vista</param>
		private void GuardarDatosDocumento(Guid pOntologiaID, ObjetoConocimientoModel pObjetoConocimientoModel)
        {
            AD.EntityModel.Models.Documentacion.Documento documento = mEntityContext.Documento.Where(item => item.DocumentoID.Equals(pOntologiaID)).First();
            documento.Titulo = pObjetoConocimientoModel.Name;
        }

        private void GuardarDatosPresentacionListado(Guid pOntologiaID, ObjetoConocimientoModel pObjetoConocimiento)
        {
            AgregarPresentacionListadoGenerico(pOntologiaID, "descripcion", 10001, pObjetoConocimiento.PresentacionListado.MostrarDescripcion);
            AgregarPresentacionListadoGenerico(pOntologiaID, "publicador", 10002, pObjetoConocimiento.PresentacionListado.MostrarPublicador);
            AgregarPresentacionListadoGenerico(pOntologiaID, "etiquetas", 10003, pObjetoConocimiento.PresentacionListado.MostrarEtiquetas);
            AgregarPresentacionListadoGenerico(pOntologiaID, "categorias", 10004, pObjetoConocimiento.PresentacionListado.MostrarCategorias);

            List<PresentacionListadoSemantico> listaRecorrer = DataWrapperProyecto.ListaPresentacionListadoSemantico.Where(presentacion => presentacion.OntologiaID == pOntologiaID && presentacion.Ontologia != "").ToList();
            foreach (PresentacionListadoSemantico filaListadoSem in listaRecorrer)
            {
                if (pObjetoConocimiento.PresentacionListado.ListaPropiedades == null || !pObjetoConocimiento.PresentacionListado.ListaPropiedades.Any(item => item.Orden == filaListadoSem.Orden) && mEntityContext.Entry(filaListadoSem).State != EntityState.Deleted)
                {
                    mEntityContext.Entry(filaListadoSem).State = EntityState.Deleted;
                    DataWrapperProyecto.ListaPresentacionListadoSemantico.Remove(filaListadoSem);
                }
            }

            if (pObjetoConocimiento.PresentacionListado.ListaPropiedades != null)
            {
                foreach (ObjetoConocimientoModel.PresentacionModel.PropiedadModel propiedad in pObjetoConocimiento.PresentacionListado.ListaPropiedades)
                {
                    if (string.IsNullOrEmpty(propiedad.Presentacion))
                    {
                        propiedad.Presentacion = "";
                    }

                    AgregarPresentacionListado(pOntologiaID, pObjetoConocimiento.Ontologia, propiedad.Orden, propiedad.Propiedad, propiedad.Presentacion);
                }
            }
        }

        private void AgregarPresentacionListadoGenerico(Guid pOntologiaID, string pPropiedad, short pOrden, bool pMostrarPropiedad)
        {
            PresentacionListadoSemantico filasListadoSem = DataWrapperProyecto.ListaPresentacionListadoSemantico.FirstOrDefault(presentacion => presentacion.OntologiaID == pOntologiaID && presentacion.Propiedad == pPropiedad);

            //Guardar la presentacion
            if (pMostrarPropiedad && filasListadoSem == null)
            {
                AgregarPresentacionListado(pOntologiaID, "", pOrden, pPropiedad, "");
            }
            else if (!pMostrarPropiedad && filasListadoSem != null && mEntityContext.Entry(filasListadoSem).State != EntityState.Deleted)
            {
                mEntityContext.EliminarElemento(filasListadoSem);
                DataWrapperProyecto.ListaPresentacionListadoSemantico.Remove(filasListadoSem);
            }
            else if (filasListadoSem != null)
            {
                if (filasListadoSem.Orden != pOrden)
                {
                    mEntityContext.EliminarElemento(filasListadoSem);
                    DataWrapperProyecto.ListaPresentacionListadoSemantico.Remove(filasListadoSem);
                    AgregarPresentacionListado(pOntologiaID, "", pOrden, pPropiedad, "");
                }
            }
        }

        private void AgregarPresentacionListado(Guid pOntologiaID, string pNombreOnto, short pOrden, string pPropiedad, string pPresentacion)
        {
            PresentacionListadoSemantico presentacionListadoBD = mEntityContext.PresentacionListadoSemantico.Where(presentacion => presentacion.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && presentacion.ProyectoID.Equals(ProyectoSeleccionado.Clave) && presentacion.OntologiaID.Equals(pOntologiaID) && presentacion.Orden.Equals(pOrden)).FirstOrDefault();

            if (presentacionListadoBD == null)
            {
                presentacionListadoBD = new PresentacionListadoSemantico();
                presentacionListadoBD.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                presentacionListadoBD.ProyectoID = ProyectoSeleccionado.Clave;
                presentacionListadoBD.OntologiaID = pOntologiaID;
                presentacionListadoBD.Orden = pOrden;
                DataWrapperProyecto.ListaPresentacionListadoSemantico.Add(presentacionListadoBD);
                mEntityContext.PresentacionListadoSemantico.Add(presentacionListadoBD);
            }

            if (!string.IsNullOrEmpty(pNombreOnto))
            {
                presentacionListadoBD.Ontologia = $"http://gnoss.com/Ontologia{pNombreOnto}.owl#";
            }
            else
            {
                presentacionListadoBD.Ontologia = string.Empty;
            }
            presentacionListadoBD.Propiedad = pPropiedad;
            presentacionListadoBD.Nombre = HttpUtility.UrlDecode(pPresentacion);
        }

        private void GuardarDatosPresentacionMosaico(Guid pOntologiaID, ObjetoConocimientoModel pObjetoConocimiento)
        {
            AgregarPresentacionMosaicoGenerico(pOntologiaID, "descripcion", 10001, pObjetoConocimiento.PresentacionMosaico.MostrarDescripcion);
            AgregarPresentacionMosaicoGenerico(pOntologiaID, "publicador", 10002, pObjetoConocimiento.PresentacionMosaico.MostrarPublicador);
            AgregarPresentacionMosaicoGenerico(pOntologiaID, "etiquetas", 10003, pObjetoConocimiento.PresentacionMosaico.MostrarEtiquetas);
            AgregarPresentacionMosaicoGenerico(pOntologiaID, "categorias", 10004, pObjetoConocimiento.PresentacionMosaico.MostrarCategorias);

            List<PresentacionMosaicoSemantico> listaPresentacoin = DataWrapperProyecto.ListaPresentacionMosaicoSemantico.Where(presentacion => presentacion.OntologiaID == pOntologiaID && !string.IsNullOrEmpty(presentacion.Ontologia)).ToList();
            foreach (PresentacionMosaicoSemantico filaMosaicoSem in listaPresentacoin)
            {
                if (pObjetoConocimiento.PresentacionMosaico.ListaPropiedades == null || !pObjetoConocimiento.PresentacionMosaico.ListaPropiedades.Any(item => item.Orden == filaMosaicoSem.Orden) && mEntityContext.Entry(filaMosaicoSem).State != EntityState.Deleted)
                {
                    mEntityContext.Entry(filaMosaicoSem).State = EntityState.Deleted;
                    DataWrapperProyecto.ListaPresentacionMosaicoSemantico.Remove(filaMosaicoSem);
                }
            }

            if (pObjetoConocimiento.PresentacionMosaico.ListaPropiedades != null)
            {
                foreach (ObjetoConocimientoModel.PresentacionModel.PropiedadModel propiedad in pObjetoConocimiento.PresentacionMosaico.ListaPropiedades)
                {
                    if (string.IsNullOrEmpty(propiedad.Presentacion))
                    {
                        propiedad.Presentacion = string.Empty;
                    }

                    AgregarPresentacionMosaico(pOntologiaID, pObjetoConocimiento.Ontologia, propiedad.Orden, propiedad.Propiedad, propiedad.Presentacion);
                }
            }
        }

        private void AgregarPresentacionMosaicoGenerico(Guid pOntologiaID, string pPropiedad, short pOrden, bool pMostrarPropiedad)
        {
            PresentacionMosaicoSemantico filasMosaicoSem = DataWrapperProyecto.ListaPresentacionMosaicoSemantico.FirstOrDefault(presentacion => mEntityContext.Entry(presentacion).State != EntityState.Deleted && presentacion.OntologiaID == pOntologiaID && presentacion.Propiedad == pPropiedad);

            if (pMostrarPropiedad && filasMosaicoSem == null)
            {
                AgregarPresentacionMosaico(pOntologiaID, "", pOrden, pPropiedad, "");
            }
            else if (!pMostrarPropiedad && filasMosaicoSem != null)
            {
                mEntityContext.EliminarElemento(filasMosaicoSem);
                DataWrapperProyecto.ListaPresentacionMosaicoSemantico.Remove(filasMosaicoSem);
            }
            else if (filasMosaicoSem != null)
            {
                filasMosaicoSem.Orden = pOrden;
            }
        }

        private void AgregarPresentacionMosaico(Guid pOntologiaID, string pNombreOnto, short pOrden, string pPropiedad, string pPresentacion)
        {
            PresentacionMosaicoSemantico filaPresentacionMosaico = mEntityContext.PresentacionMosaicoSemantico.FirstOrDefault(presentacion => presentacion.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && presentacion.ProyectoID.Equals(ProyectoSeleccionado.Clave) && presentacion.OntologiaID.Equals(pOntologiaID) && presentacion.Orden.Equals(pOrden));

            if (filaPresentacionMosaico == null)
            {
                filaPresentacionMosaico = new PresentacionMosaicoSemantico();
                filaPresentacionMosaico.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                filaPresentacionMosaico.ProyectoID = ProyectoSeleccionado.Clave;
                filaPresentacionMosaico.OntologiaID = pOntologiaID;
                filaPresentacionMosaico.Orden = pOrden;
                DataWrapperProyecto.ListaPresentacionMosaicoSemantico.Add(filaPresentacionMosaico);
                mEntityContext.PresentacionMosaicoSemantico.Add(filaPresentacionMosaico);
            }


            if (!string.IsNullOrEmpty(pNombreOnto))
            {
                filaPresentacionMosaico.Ontologia = $"http://gnoss.com/Ontologia{pNombreOnto}.owl#";
            }
            else
            {
                filaPresentacionMosaico.Ontologia = string.Empty;
            }
            filaPresentacionMosaico.Propiedad = pPropiedad;
            filaPresentacionMosaico.Nombre = HttpUtility.UrlDecode(pPresentacion);
        }

        private void GuardarDatosPresentacionMapa(Guid pOntologiaID, ObjetoConocimientoModel pObjetoConocimiento)
        {
            AgregarPresentacionMapaGenerico(pOntologiaID, "descripcion", 10001, pObjetoConocimiento.PresentacionMapa.MostrarDescripcion);
            AgregarPresentacionMapaGenerico(pOntologiaID, "publicador", 10002, pObjetoConocimiento.PresentacionMapa.MostrarPublicador);
            AgregarPresentacionMapaGenerico(pOntologiaID, "etiquetas", 10003, pObjetoConocimiento.PresentacionMapa.MostrarEtiquetas);
            AgregarPresentacionMapaGenerico(pOntologiaID, "categorias", 10004, pObjetoConocimiento.PresentacionMapa.MostrarCategorias);

            List<PresentacionMapaSemantico> listaRecorrer = DataWrapperProyecto.ListaPresentacionMapaSemantico.Where(presentacion => presentacion.OntologiaID == pOntologiaID && !string.IsNullOrEmpty(presentacion.Ontologia)).ToList();
            foreach (PresentacionMapaSemantico filaMapaSem in listaRecorrer)
            {
                if (pObjetoConocimiento.PresentacionMapa.ListaPropiedades == null || !pObjetoConocimiento.PresentacionMapa.ListaPropiedades.Any(item => item.Orden == filaMapaSem.Orden) && mEntityContext.Entry(filaMapaSem).State != EntityState.Deleted)
                {
                    mEntityContext.Entry(filaMapaSem).State = EntityState.Deleted;
                    DataWrapperProyecto.ListaPresentacionMapaSemantico.Remove(filaMapaSem);
                }
            }

            if (pObjetoConocimiento.PresentacionMapa.ListaPropiedades != null)
            {
                foreach (ObjetoConocimientoModel.PresentacionModel.PropiedadModel propiedad in pObjetoConocimiento.PresentacionMapa.ListaPropiedades)
                {
                    if (propiedad.Presentacion == null)
                    {
                        propiedad.Presentacion = string.Empty;
                    }

                    AgregarPresentacionMapa(pOntologiaID, pObjetoConocimiento.Ontologia, propiedad.Orden, propiedad.Propiedad, propiedad.Presentacion);
                }
            }
        }

        private void GuardarDatosPresentacionPersonalizado(Guid pOntologiaID, ObjetoConocimientoModel pObjetoConocimiento)
        {
            List<PresentacionPersonalizadoSemantico> listaRecorrer = DataWrapperProyecto.ListaPresentacionPersonalizadoSemantico.Where(presentacion => presentacion.OntologiaID.Equals(pOntologiaID) && !string.IsNullOrEmpty(presentacion.Ontologia)).ToList();

            foreach (PresentacionPersonalizadoSemantico filaPersonalizadoSem in listaRecorrer)
            {
                if (pObjetoConocimiento.PresentacionPersonalizado == null || !pObjetoConocimiento.PresentacionPersonalizado.ListaPropiedades.Any(item => item.Orden == filaPersonalizadoSem.Orden) && mEntityContext.Entry(filaPersonalizadoSem).State != EntityState.Deleted)
                {
                    mEntityContext.Entry(filaPersonalizadoSem).State = EntityState.Deleted;
                    DataWrapperProyecto.ListaPresentacionPersonalizadoSemantico.Remove(filaPersonalizadoSem);
                }
            }

            if (pObjetoConocimiento.PresentacionPersonalizado != null && pObjetoConocimiento.PresentacionPersonalizado.ListaPropiedades != null)
            {
                foreach (ObjetoConocimientoModel.PresentacionPersonalizadoModel.PropiedadPersonalizadoModel propiedad in pObjetoConocimiento.PresentacionPersonalizado.ListaPropiedades)
                {
                    AgregarPresentacionPersonalizado(pOntologiaID, pObjetoConocimiento.Ontologia, propiedad.Orden, propiedad.Identificador, propiedad.Select, propiedad.Where);
                }
            }
        }

        private void AgregarPresentacionMapaGenerico(Guid pOntologiaID, string pPropiedad, short pOrden, bool pMostrarPropiedad)
        {
            PresentacionMapaSemantico filasMapaSem = DataWrapperProyecto.ListaPresentacionMapaSemantico.FirstOrDefault(presentacion => mEntityContext.Entry(presentacion).State != EntityState.Deleted && presentacion.OntologiaID == pOntologiaID && presentacion.Propiedad == pPropiedad);

            //Guardar la presentacion
            if (pMostrarPropiedad && filasMapaSem == null)
            {
                AgregarPresentacionMapa(pOntologiaID, "", pOrden, pPropiedad, "");
            }
            else if (!pMostrarPropiedad && filasMapaSem != null)
            {
                mEntityContext.EliminarElemento(filasMapaSem);
                DataWrapperProyecto.ListaPresentacionMapaSemantico.Remove(filasMapaSem);
            }
            else if (filasMapaSem != null)
            {
                filasMapaSem.Orden = pOrden;
            }
        }

        private void AgregarPresentacionMapa(Guid pOntologiaID, string pNombreOnto, short pOrden, string pPropiedad, string pPresentacion)
        {
            AD.EntityModel.Models.ProyectoDS.PresentacionMapaSemantico filaPresentacionMapa = mEntityContext.PresentacionMapaSemantico.FirstOrDefault(presentacion => presentacion.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && presentacion.ProyectoID.Equals(ProyectoSeleccionado.Clave) && presentacion.OntologiaID.Equals(pOntologiaID) && presentacion.Orden.Equals(pOrden));
            bool nuevaEntrada = false;

            if (filaPresentacionMapa == null)
            {
                filaPresentacionMapa = new AD.EntityModel.Models.ProyectoDS.PresentacionMapaSemantico();
                nuevaEntrada = true;
                filaPresentacionMapa.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                filaPresentacionMapa.ProyectoID = ProyectoSeleccionado.Clave;
                filaPresentacionMapa.OntologiaID = pOntologiaID;
                filaPresentacionMapa.Orden = pOrden;
            }

            if (!string.IsNullOrEmpty(pNombreOnto))
            {
                filaPresentacionMapa.Ontologia = $"http://gnoss.com/Ontologia{pNombreOnto}.owl#";
            }
            else
            {
                filaPresentacionMapa.Ontologia = string.Empty;
            }
            filaPresentacionMapa.Propiedad = pPropiedad;
            filaPresentacionMapa.Nombre = HttpUtility.UrlDecode(pPresentacion);

            if (nuevaEntrada)
            {
                DataWrapperProyecto.ListaPresentacionMapaSemantico.Add(filaPresentacionMapa);
                mEntityContext.PresentacionMapaSemantico.Add(filaPresentacionMapa);
            }
        }

        private void AgregarPresentacionPersonalizado(Guid pOntologiaID, string pNombreOnto, short pOrden, string pIdentificador, string pSelect, string pWhere)
        {
            PresentacionPersonalizadoSemantico filaPresentacionPersonalizado = mEntityContext.PresentacionPersonalizadoSemantico.FirstOrDefault(presentacion => presentacion.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && presentacion.ProyectoID.Equals(ProyectoSeleccionado.Clave) && presentacion.OntologiaID.Equals(pOntologiaID) && presentacion.Orden.Equals(pOrden));

            if (filaPresentacionPersonalizado == null)
            {
                filaPresentacionPersonalizado = new PresentacionPersonalizadoSemantico();
                filaPresentacionPersonalizado.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                filaPresentacionPersonalizado.ProyectoID = ProyectoSeleccionado.Clave;
                filaPresentacionPersonalizado.OntologiaID = pOntologiaID;
                filaPresentacionPersonalizado.Orden = pOrden;
                DataWrapperProyecto.ListaPresentacionPersonalizadoSemantico.Add(filaPresentacionPersonalizado);
                mEntityContext.PresentacionPersonalizadoSemantico.Add(filaPresentacionPersonalizado);
            }

            if (!string.IsNullOrEmpty(pNombreOnto))
            {
                filaPresentacionPersonalizado.Ontologia = $"http://gnoss.com/Ontologia{pNombreOnto}.owl#";
            }
            else
            {
                filaPresentacionPersonalizado.Ontologia = string.Empty;
            }
            filaPresentacionPersonalizado.ID = HttpUtility.UrlDecode(pIdentificador);
            filaPresentacionPersonalizado.Select = HttpUtility.UrlDecode(pSelect);
            filaPresentacionPersonalizado.Where = HttpUtility.UrlDecode(pWhere);
        }

        private void GuardarDatosPresentacionRelacionado(Guid pOntologiaID, ObjetoConocimientoModel pObjetoConocimiento)
        {
            AgregarPresentacionRelacionadoGenerico(pOntologiaID, "descripcion", 10001, pObjetoConocimiento.PresentacionRelacionados.MostrarDescripcion);
            AgregarPresentacionRelacionadoGenerico(pOntologiaID, "publicador", 10002, pObjetoConocimiento.PresentacionRelacionados.MostrarPublicador);
            AgregarPresentacionRelacionadoGenerico(pOntologiaID, "etiquetas", 10003, pObjetoConocimiento.PresentacionRelacionados.MostrarEtiquetas);
            AgregarPresentacionRelacionadoGenerico(pOntologiaID, "categorias", 10004, pObjetoConocimiento.PresentacionRelacionados.MostrarCategorias);

            List<RecursosRelacionadosPresentacion> listaRecorrer = DataWrapperProyecto.ListaRecursosRelacionadosPresentacion.Where(presentacion => presentacion.OntologiaID.Equals(pOntologiaID) && !string.IsNullOrEmpty(presentacion.Ontologia)).ToList();

            foreach (RecursosRelacionadosPresentacion filaRelacionadoSem in listaRecorrer)
            {
                if (pObjetoConocimiento.PresentacionRelacionados.ListaPropiedades == null || !pObjetoConocimiento.PresentacionRelacionados.ListaPropiedades.Any(item => item.Orden == filaRelacionadoSem.Orden) && mEntityContext.Entry(filaRelacionadoSem).State != EntityState.Deleted)
                {
                    mEntityContext.Entry(filaRelacionadoSem).State = EntityState.Deleted;
                    DataWrapperProyecto.ListaRecursosRelacionadosPresentacion.Remove(filaRelacionadoSem);
                }
            }

            if (pObjetoConocimiento.PresentacionRelacionados.ListaPropiedades != null)
            {
                foreach (ObjetoConocimientoModel.PresentacionModel.PropiedadModel propiedad in pObjetoConocimiento.PresentacionRelacionados.ListaPropiedades)
                {
                    if (string.IsNullOrEmpty(propiedad.Presentacion))
                    {
                        propiedad.Presentacion = "";
                    }

                    AgregarPresentacionRelacionado(pOntologiaID, pObjetoConocimiento.Ontologia, propiedad.Orden, propiedad.Propiedad, propiedad.Presentacion);
                }
            }
        }

        private void AgregarPresentacionRelacionadoGenerico(Guid pOntologiaID, string pPropiedad, short pOrden, bool pMostrarPropiedad)
        {
            RecursosRelacionadosPresentacion filasRelacionadoSem = DataWrapperProyecto.ListaRecursosRelacionadosPresentacion.FirstOrDefault(presentacion => mEntityContext.Entry(presentacion).State != EntityState.Deleted && presentacion.OntologiaID == pOntologiaID && presentacion.Propiedad == pPropiedad);

            //Guardar la presentacion
            if (pMostrarPropiedad && filasRelacionadoSem == null)
            {
                AgregarPresentacionRelacionado(pOntologiaID, "", pOrden, pPropiedad, "");
            }
            else if (!pMostrarPropiedad && filasRelacionadoSem != null)
            {
                mEntityContext.EliminarElemento(filasRelacionadoSem);
                DataWrapperProyecto.ListaRecursosRelacionadosPresentacion.Remove(filasRelacionadoSem);
            }
            else if (filasRelacionadoSem != null)
            {
                filasRelacionadoSem.Orden = pOrden;
            }
        }

        private void AgregarPresentacionRelacionado(Guid pOntologiaID, string pNombreOnto, short pOrden, string pPropiedad, string pPresentacion)
        {
            RecursosRelacionadosPresentacion filaPresentacionRelacionado = mEntityContext.RecursosRelacionadosPresentacion.FirstOrDefault(presentacion => presentacion.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && presentacion.ProyectoID.Equals(ProyectoSeleccionado.Clave) && presentacion.OntologiaID.Equals(pOntologiaID) && presentacion.Orden.Equals(pOrden));

            if (filaPresentacionRelacionado == null)
            {
                filaPresentacionRelacionado = new RecursosRelacionadosPresentacion();

                filaPresentacionRelacionado.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                filaPresentacionRelacionado.ProyectoID = ProyectoSeleccionado.Clave;
                filaPresentacionRelacionado.OntologiaID = pOntologiaID;
                filaPresentacionRelacionado.Orden = pOrden;

                DataWrapperProyecto.ListaRecursosRelacionadosPresentacion.Add(filaPresentacionRelacionado);
                mEntityContext.RecursosRelacionadosPresentacion.Add(filaPresentacionRelacionado);
            }

            filaPresentacionRelacionado.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
            filaPresentacionRelacionado.ProyectoID = ProyectoSeleccionado.Clave;
            filaPresentacionRelacionado.OntologiaID = pOntologiaID;
            filaPresentacionRelacionado.Orden = pOrden;
            if (!string.IsNullOrEmpty(pNombreOnto))
            {
                filaPresentacionRelacionado.Ontologia = $"http://gnoss.com/Ontologia{pNombreOnto}.owl#";
            }
            else
            {
                filaPresentacionRelacionado.Ontologia = string.Empty;
            }
            filaPresentacionRelacionado.Propiedad = pPropiedad;
            filaPresentacionRelacionado.Nombre = HttpUtility.UrlDecode(pPresentacion);
        }

        public EditOntologyViewModel EliminarObjetoConocimientoOntologia(Guid pOntologiaID, string pObjetoConocimientoID, bool pEsPrincipal)
        {
            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            if (!documentacionCN.OntologiaTieneRecursos(pOntologiaID))
            {
                mLoggingService.AgregarEntrada($"EliminarObjetoConocimiento - El objeto de conocimiento no tiene recursos asociados");
                ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoAD>(), mLoggerFactory);
                bool transaccionIniciada = proyAD.IniciarTransaccion(true);

                try
                {
                    if (transaccionIniciada)
                    {
                        mEntityContext.NoConfirmarTransacciones = false;
                    }
                    
                    if (pEsPrincipal)
                    {
                        mLoggingService.AgregarEntrada($"EliminarObjetoConocimiento - Preparado para obtener ontología de BD --> ObjetoConocimientoID: {pObjetoConocimientoID} -- ProyectoID: {ProyectoSeleccionado.Clave}");
                        OntologiaProyecto ontologiaProyecto = documentacionCN.ObtenerOntologiaProyectoPorOntologia(pObjetoConocimientoID, ProyectoSeleccionado.Clave);
                        mLoggingService.AgregarEntrada($"EliminarObjetoConocimiento - Obtenida ontología {ontologiaProyecto.NombreOnt} correctamente");
                        EliminarObjetoConocimiento(pOntologiaID, ontologiaProyecto);
                        mLoggingService.AgregarEntrada($"EliminarObjetoConocimiento - Eliminado objeto conocimiento correctamente");
                    }

                    mLoggingService.AgregarEntrada($"EliminarObjetoConocimiento - Entrando a eliminar Documento y dependencias {pOntologiaID}");
                    EditOntologyViewModel modeloEdicionOntologia = EliminarOntologia(pOntologiaID);
                    mLoggingService.AgregarEntrada($"EliminarObjetoConocimiento - Documento y dependencias eliminadas correctamente");

                    GuardarCambios();
                    mLoggingService.AgregarEntrada($"EliminarObjetoConocimiento - Cambios guardados correctamente");

                    if (transaccionIniciada)
                    {
                        proyAD.TerminarTransaccion(true);
                    }
                    return modeloEdicionOntologia;
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex);
                    proyAD.TerminarTransaccion(false);
                    throw new Exception("Ha ocurrido un error al eliminar la ontología", ex);
                }
            }
            else
            {
                string mensajeError = "No se puede eliminar la ontología porque tiene recursos asociados.";
                mLoggingService.GuardarLogError(mensajeError);
                throw new Exception(mensajeError);
            }
        }

        private void EliminarObjetoConocimiento(Guid pOntologiaID, OntologiaProyecto pFilaOntologia)
        {
            foreach (PresentacionListadoSemantico filaPresentacionListado in DataWrapperProyecto.ListaPresentacionListadoSemantico.Where(presentacion => mEntityContext.Entry(presentacion).State != EntityState.Deleted && presentacion.OntologiaID.Equals(pOntologiaID) && presentacion.ProyectoID.Equals(pFilaOntologia.ProyectoID)).ToList())
            {
                mEntityContext.EliminarElemento(filaPresentacionListado);
                DataWrapperProyecto.ListaPresentacionListadoSemantico.Remove(filaPresentacionListado);
            }
            foreach (PresentacionMosaicoSemantico filaPresentacionMosaico in DataWrapperProyecto.ListaPresentacionMosaicoSemantico.Where(presentacion => mEntityContext.Entry(presentacion).State != EntityState.Deleted && presentacion.OntologiaID.Equals(pOntologiaID) && presentacion.ProyectoID.Equals(pFilaOntologia.ProyectoID)).ToList())
            {
                mEntityContext.EliminarElemento(filaPresentacionMosaico);
                DataWrapperProyecto.ListaPresentacionMosaicoSemantico.Remove(filaPresentacionMosaico);
            }
            foreach (PresentacionMapaSemantico filaPresentacionMapa in DataWrapperProyecto.ListaPresentacionMapaSemantico.Where(presentacion => mEntityContext.Entry(presentacion).State != EntityState.Deleted && presentacion.OntologiaID.Equals(pOntologiaID) && presentacion.ProyectoID.Equals(pFilaOntologia.ProyectoID)).ToList())
            {
                mEntityContext.EliminarElemento(filaPresentacionMapa);
                DataWrapperProyecto.ListaPresentacionMapaSemantico.Remove(filaPresentacionMapa);
            }
            foreach (RecursosRelacionadosPresentacion filaPresentacionRelacionados in DataWrapperProyecto.ListaRecursosRelacionadosPresentacion.Where(presentacion => mEntityContext.Entry(presentacion).State != EntityState.Deleted && presentacion.OntologiaID.Equals(pOntologiaID) && presentacion.ProyectoID.Equals(pFilaOntologia.ProyectoID)).ToList())
            {
                mEntityContext.EliminarElemento(filaPresentacionRelacionados);
                DataWrapperProyecto.ListaRecursosRelacionadosPresentacion.Remove(filaPresentacionRelacionados);
            }
            foreach(ProyectoPestanyaBusquedaPesoOC proyectoPestanyaBusquedaPesoOC in DataWrapperProyecto.ListaProyectoPestanyaBusquedaPesoOC.Where(item => item.OntologiaProyecto1.Equals(pFilaOntologia.OntologiaProyecto1)).ToList())
            {
                mEntityContext.EliminarElemento(proyectoPestanyaBusquedaPesoOC);
                DataWrapperProyecto.ListaProyectoPestanyaBusquedaPesoOC.Remove(proyectoPestanyaBusquedaPesoOC);
            }
            mEntityContext.EliminarElemento(pFilaOntologia);
            DataWrapperProyecto.ListaOntologiaProyecto.Remove(pFilaOntologia);
        }

        private EditOntologyViewModel EliminarOntologia(Guid pOntologiaID)
        {
            using (DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory))
            {                
                AD.EntityModel.Models.Documentacion.Documento documento = docCN.ObtenerDocumentoPorIdentificador(pOntologiaID);

                ProyectoConfigExtraSem filasConfig = ProyectoConfigSemDataWrapperProyecto.ListaProyectoConfigExtraSem.Where(proy => proy.UrlOntologia.Equals(documento.Enlace)).FirstOrDefault();

                if (filasConfig != null)
                {
                    ProyectoConfigSemDataWrapperProyecto.ListaProyectoConfigExtraSem.Remove(filasConfig);
                    mEntityContext.EliminarElemento(filasConfig);
                }

                DocumentoWebVinBaseRecursos documentoWebVinBaseRecursos = docCN.ObtenerDocumentoWebVinBaseRecursoPorDocumentoID(pOntologiaID);
                documentoWebVinBaseRecursos.Eliminado = true;
                documento.Eliminado = true;

                EditOntologyViewModel ontologyBorrar = new EditOntologyViewModel();

                using (GestorDocumental gestorDoc = new GestorDocumental(docCN.ObtenerDocumentoPorID(pOntologiaID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestorDocumental>(), mLoggerFactory))
                {
                    ontologyBorrar.Name = documento.Titulo;
                    ontologyBorrar.Deleted = true;
                }

                return ontologyBorrar;
            }            
        }

        /// <summary>
        /// Guarda el documento en la base de datos.
        /// </summary>
        private void GuardarCambios()
        {
            mEntityContext.SaveChanges();

            DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
            docCL.InvalidarOntologiasProyecto(ProyectoSeleccionado.Clave);

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
            proyCL.InvalidarOntologiasEcosistema();
        }

        #endregion

        #region Invalidar caches

        public void InvalidarCaches(string UrlIntragnoss)
        {
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
            proyCL.InvalidarFilaProyecto(ProyectoSeleccionado.Clave);
            proyCL.InvalidarComunidadMVC(ProyectoSeleccionado.Clave);
            proyCL.InvalidarCabeceraMVC(ProyectoSeleccionado.Clave);
            proyCL.InvalidarPresentacionSemantico(ProyectoSeleccionado.Clave);

            FacetaCL facetaCL = new FacetaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetaCL>(), mLoggerFactory);
            bool cachearFacetas = !(ParametroProyecto.ContainsKey("CacheFacetas") && ParametroProyecto["CacheFacetas"].Equals("0"));
            facetaCL.InvalidarCacheFacetasProyecto(ProyectoSeleccionado.Clave, cachearFacetas);
            facetaCL.InvalidarOntologiasProyecto(ProyectoSeleccionado.Clave);

            FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCL>(), mLoggerFactory);
            facetadoCL.InvalidarResultadosYFacetasDeBusquedaEnProyecto(ProyectoSeleccionado.Clave, "*");

            mGnossCache.VersionarCacheLocal(ProyectoSeleccionado.Clave);
        }

        #endregion

        #region Metodos de errores
        public string ComprobarErrores(List<ObjetoConocimientoModel> pListaObjetosConocimiento)
        {
            string errores = string.Empty;

            foreach (ObjetoConocimientoModel objetoConocimiento in pListaObjetosConocimiento)
            {
                errores += ComprobarErroresObjetoConocimiento(objetoConocimiento);
            }

            return errores;
        }

        public string ComprobarErroresObjetoConocimiento(ObjetoConocimientoModel pObjetoConocimiento)
        {
            //todo
            string errores = "";

            return errores;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// 
        /// </summary>
        private DataWrapperProyecto ProyectoConfigSemDataWrapperProyecto
        {
            get
            {
                if (mProyectoConfigSemDataWrapperProyecto == null)
                {
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                    mProyectoConfigSemDataWrapperProyecto = proyCN.ObtenerConfiguracionSemanticaExtraDeProyecto(ProyectoSeleccionado.Clave);
                    proyCN.Dispose();
                }
                return mProyectoConfigSemDataWrapperProyecto;
            }
        }

        private DataWrapperProyecto DataWrapperProyecto
        {
            get
            {
                if (mDataWrapperProyecto == null)
                {
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                    mDataWrapperProyecto = proyCN.ObtenerProyectoPorID(ProyectoSeleccionado.Clave);
                    mDataWrapperProyecto.Merge(proyCN.ObtenerPresentacionSemantico(ProyectoSeleccionado.Clave));
                }

                return mDataWrapperProyecto;
            }
        }

        #endregion
    }
}
