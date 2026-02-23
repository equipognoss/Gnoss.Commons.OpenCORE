using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.RabbitMQ;
using Es.Riam.Gnoss.Servicios.ControladoresServiciosWeb;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using static Es.Riam.Gnoss.Web.MVC.Models.Administracion.FacetaModel;

namespace Es.Riam.Gnoss.Web.Controles.Administracion
{
    public class ControladorFacetas
    {
        private DataWrapperFacetas mFacetaDW = null;
        private readonly Proyecto ProyectoSeleccionado = null;
        private readonly Dictionary<string, string> ParametroProyecto = null;
        private readonly Dictionary<string, string> ListaOntologias = null;

        private readonly bool CrearFilasPropiedadesExportacion = false;

        private readonly LoggingService mLoggingService;
        private readonly VirtuosoAD mVirtuosoAD;
        private readonly EntityContext mEntityContext;
        private readonly ConfigService mConfigService;
        private readonly IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private readonly RedisCacheWrapper mRedisCacheWrapper;
        private readonly GnossCache mGnossCache;
        private readonly ILogger mLogger;
        private readonly ILoggerFactory mLoggerFactory;

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public ControladorFacetas(Proyecto pProyecto, Dictionary<string, string> pParametroProyecto, Dictionary<string, string> pListaOntologias, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication pServicesVirtuosoAndReplication, ILogger<ControladorFacetas> logger, ILoggerFactory loggerFactory, bool pCrearFilasPropiedadesExportacion = false)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mServicesUtilVirtuosoAndReplication = pServicesVirtuosoAndReplication;
            mRedisCacheWrapper = redisCacheWrapper;
            mGnossCache = gnossCache;

            ProyectoSeleccionado = pProyecto;
            ParametroProyecto = pParametroProyecto;
            ListaOntologias = pListaOntologias;
            mLogger = logger;
            mLoggerFactory = loggerFactory;
            CrearFilasPropiedadesExportacion = pCrearFilasPropiedadesExportacion;
        }

        #endregion

        #region Metodos de Carga

        public List<FacetaModel> CargarListadoFacetas()
        {
            List<FacetaModel> ListaFacetas = new List<FacetaModel>();

            List<string> facetasExcluidas = FacetaDW.ListaFacetaExcluida.Select(item => item.Faceta).Distinct().ToList();

            List<IntegracionContinuaPropiedad> propiedadesIntegracionContinua = new List<IntegracionContinuaPropiedad>();

            foreach (FacetaObjetoConocimientoProyecto filaFacetaOCProyecto in FacetaDW.ListaFacetaObjetoConocimientoProyecto)
            {
                List<FacetaFiltroProyecto> filasFiltros = filaFacetaOCProyecto.FacetaFiltroProyecto.ToList();

                string filtrosFaceta = String.Join(",", filasFiltros.OrderBy(filtro => filtro.Orden).Select(filaFiltro => filaFiltro.Filtro));

                FacetaModel faceta = null;
                if (filaFacetaOCProyecto.AgrupacionID.HasValue)
                {
                    faceta = ListaFacetas.Find(fac => (fac.ClaveFaceta == filaFacetaOCProyecto.Faceta || (filaFacetaOCProyecto.Reciproca > 0 && fac.ClaveFacetaYReprocidad == filaFacetaOCProyecto.Faceta)) && fac.Filtros == filtrosFaceta && fac.AgrupacionID.HasValue && fac.AgrupacionID.Value.Equals(filaFacetaOCProyecto.AgrupacionID));
                }
                else
                {
                    faceta = ListaFacetas.Find(fac => (fac.ClaveFaceta == filaFacetaOCProyecto.Faceta || (filaFacetaOCProyecto.Reciproca > 0 && fac.ClaveFacetaYReprocidad == filaFacetaOCProyecto.Faceta)) && fac.Filtros == filtrosFaceta);
                }

                if (faceta == null)
                {
                    faceta = CargarFacetaProy(filaFacetaOCProyecto);

                    if (CrearFilasPropiedadesExportacion && faceta.Type.Equals(TipoFaceta.Tesauro) && !string.IsNullOrEmpty(faceta.Filtros))
                    {
                        //Crear las filas de las porpiedades de Integracion Continua
                        IntegracionContinuaPropiedad propiedadRutaPagina = new IntegracionContinuaPropiedad();
                        propiedadRutaPagina.ProyectoID = ProyectoSeleccionado.Clave;
                        propiedadRutaPagina.TipoObjeto = (short)TipoObjeto.Faceta;
                        if (!string.IsNullOrEmpty(faceta.ClaveFacetaYReprocidad))
                        {
                            propiedadRutaPagina.ObjetoPropiedad = faceta.ClaveFacetaYReprocidad;
                        }
                        else
                        {
                            propiedadRutaPagina.ObjetoPropiedad = faceta.ClaveFaceta;
                        }

                        propiedadRutaPagina.TipoPropiedad = (short)TipoPropiedad.FiltrosFaceta;
                        propiedadRutaPagina.ValorPropiedad = faceta.Filtros;
                        propiedadesIntegracionContinua.Add(propiedadRutaPagina);
                    }
                    ListaFacetas.Add(faceta);
                }
                else
                {
                    faceta.ObjetosConocimiento.Add(filaFacetaOCProyecto.ObjetoConocimiento);
                }
            }

            foreach (FacetaObjetoConocimiento filaFacetaOC in FacetaDW.ListaFacetaObjetoConocimiento.Where(item => !facetasExcluidas.Contains(item.Faceta)))
            {
                FacetaModel faceta = ListaFacetas.Find(fac => fac.ClaveFaceta == filaFacetaOC.Faceta);

                if (faceta == null)
                {
                    ListaFacetas.Add(CargarFaceta(filaFacetaOC));
                }
                else
                {
                    faceta.ObjetosConocimiento.Add(filaFacetaOC.ObjetoConocimiento.ToLower());
                }
            }

            if (CrearFilasPropiedadesExportacion)
            {
                try
                {
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                    proyCN.CrearFilasIntegracionContinuaParametro(propiedadesIntegracionContinua, ProyectoSeleccionado.Clave, TipoObjeto.Faceta);
                    proyCN.Dispose();
                }
                catch
                {
                    // Si da error al crear las filas de las propiedades de integración continua, no se interrumpe la carga de las facetas
                }
            }

            return ListaFacetas;
        }


        public FacetaModel CargarFacetaProy(FacetaObjetoConocimientoProyecto pFilaFacetaOC)
        {
            FacetaModel faceta = new FacetaModel();
            faceta.Name = pFilaFacetaOC.NombreFaceta;
            faceta.FechaModificacion = pFilaFacetaOC.FechaCreacion;

            if (pFilaFacetaOC.TipoPropiedad == (short)TipoPropiedadFaceta.Texto)
            {
                if (pFilaFacetaOC.AlgoritmoTransformacion == (short)TiposAlgoritmoTransformacion.TesauroSemantico || pFilaFacetaOC.AlgoritmoTransformacion == (short)TiposAlgoritmoTransformacion.TesauroSemanticoOrdenado || pFilaFacetaOC.AlgoritmoTransformacion == (short)TiposAlgoritmoTransformacion.Categoria || pFilaFacetaOC.AlgoritmoTransformacion == (short)TiposAlgoritmoTransformacion.CategoriaArbol)
                {
                    faceta.Type = TipoFaceta.Tesauro;
                }
                else
                {
                    faceta.Type = TipoFaceta.Texto;
                }
            }
            else if (pFilaFacetaOC.TipoPropiedad == (short)TipoPropiedadFaceta.TextoInvariable)
            {
                faceta.Type = TipoFaceta.TextoInvariable;
            }
            else if (pFilaFacetaOC.TipoPropiedad == (short)TipoPropiedadFaceta.Numero)
            {
                faceta.Type = TipoFaceta.Numero;
            }
            else if (pFilaFacetaOC.TipoPropiedad == (short)TipoPropiedadFaceta.Siglo)
            {
                faceta.Type = TipoFaceta.Siglo;
            }
            else
            {
                faceta.Type = TipoFaceta.Fecha;
            }

            faceta.ClaveFaceta = pFilaFacetaOC.Faceta;
            faceta.ComportamientoOr = pFilaFacetaOC.ComportamientoOr;
            string[] arrayFaceta = pFilaFacetaOC.Faceta.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);
            string claveFaceta = "";
            string reciproca = "";

            int i = 0;
            foreach (string trozo in arrayFaceta)
            {
                if (i < pFilaFacetaOC.Reciproca)
                {
                    if (!string.IsNullOrEmpty(reciproca))
                    {
                        reciproca += "@@@";
                    }
                    reciproca += trozo;
                }
                else
                {
                    if (!string.IsNullOrEmpty(claveFaceta))
                    {
                        claveFaceta += "@@@";
                    }
                    claveFaceta += trozo;
                }
                i++;
            }

            faceta.ClaveFaceta = claveFaceta;
            faceta.Reciprocidad = reciproca;
            if (!string.IsNullOrEmpty(faceta.Reciprocidad))
            {
                faceta.ClaveFacetaYReprocidad = faceta.Reciprocidad + "@@@" + faceta.ClaveFaceta;
            }

            faceta.ObjetosConocimiento = new List<string>();
            faceta.ObjetosConocimiento.Add(pFilaFacetaOC.ObjetoConocimiento.ToLower());
            faceta.Presentacion = pFilaFacetaOC.Mayusculas;
            faceta.Disenyo = pFilaFacetaOC.TipoDisenio;
            faceta.AlgoritmoTransformacion = pFilaFacetaOC.AlgoritmoTransformacion;
            faceta.NumElementosVisibles = pFilaFacetaOC.ElementosVisibles;

            faceta.EsSemantica = pFilaFacetaOC.EsSemantica;
            faceta.Autocompletar = pFilaFacetaOC.Autocompletar;
            faceta.Comportamiento = pFilaFacetaOC.Comportamiento;
            faceta.Excluyente = false;
            faceta.OcultaEnFacetas = pFilaFacetaOC.OcultaEnFacetas;
            faceta.OcultaEnFiltros = pFilaFacetaOC.OcultaEnFiltros;
            faceta.PriorizarOrdenResultados = pFilaFacetaOC.PriorizarOrdenResultados;

            List<FacetaFiltroProyecto> filasFiltros = pFilaFacetaOC.FacetaFiltroProyecto.ToList();

            //Lista para mostrar los filtros
            List<string> listaFiltros = new List<string>();
            List<FiltrosFacetas> listaFiltrosFiltro = new List<FiltrosFacetas>();

            StringBuilder filtrosFaceta = new StringBuilder();
            foreach (FacetaFiltroProyecto filaFiltro in filasFiltros.OrderBy(filtro => filtro.Orden))
            {
                filtrosFaceta.Append($"{filaFiltro.Filtro},");
                FiltrosFacetas filtro = new FiltrosFacetas();
                filtro.Nombre = filaFiltro.Filtro;
                filtro.Condicion = filaFiltro.Condicion;

                listaFiltros.Add(filaFiltro.Filtro);
                listaFiltrosFiltro.Add(filtro);
            }
            if (filtrosFaceta.Length > 0)
            {
                UtilCadenas.EliminarUltimosCaracteresStringBuilder(filtrosFaceta, ',');
            }
            faceta.Filtros = filtrosFaceta.ToString();

            faceta.ListFiltro = listaFiltros;
            faceta.ListaFiltrosFacetas = listaFiltrosFiltro;

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);

            // TODO: Guardar en el campo FacetaPrivadaParaGrupoEditores el nombre de la organización y obtener el grupo por proyecto o organización, según proceda
            List<Guid> grupos;
            if (!string.IsNullOrEmpty(pFilaFacetaOC.FacetaPrivadaParaGrupoEditores))
            {
                grupos = identidadCN.ObtenerGruposIDPorNombreCortoYProyecto(new List<string>(pFilaFacetaOC.FacetaPrivadaParaGrupoEditores.Split('|')), pFilaFacetaOC.ProyectoID);
                if (grupos == null || grupos.Count == 0)
                {
                    grupos = identidadCN.ObtenerGruposIDPorNombreCorto(new List<string>(pFilaFacetaOC.FacetaPrivadaParaGrupoEditores.Split('|')));
                }
                faceta.PrivacidadGrupos = identidadCN.ObtenerNombresDeGrupos(grupos);
            }
            else
            {
                faceta.PrivacidadGrupos = new Dictionary<Guid, string>();
            }

            identidadCN.Dispose();

            faceta.Condicion = pFilaFacetaOC.Condicion == null ? "" : pFilaFacetaOC.Condicion;
            faceta.Inmutable = pFilaFacetaOC.Inmutable;

            if (pFilaFacetaOC.AgrupacionID.HasValue)
            {
                faceta.AgrupacionID = pFilaFacetaOC.AgrupacionID;
            }

            faceta.MostrarContador = pFilaFacetaOC.MostrarContador;
            faceta.Orden = pFilaFacetaOC.Orden;
            return faceta;
        }

        public FacetaModel CargarFaceta(FacetaObjetoConocimiento pFilaFacetaOC)
        {
            FacetaModel faceta = new FacetaModel();
            faceta.Name = pFilaFacetaOC.NombreFaceta;

            if (pFilaFacetaOC.TipoPropiedad == (short)TipoPropiedadFaceta.Texto)
            {
                if (pFilaFacetaOC.AlgoritmoTransformacion == (short)TiposAlgoritmoTransformacion.Categoria || pFilaFacetaOC.AlgoritmoTransformacion == (short)TiposAlgoritmoTransformacion.CategoriaArbol)
                {
                    faceta.Type = TipoFaceta.Tesauro;
                }
                else
                {
                    faceta.Type = TipoFaceta.Texto;
                }
            }
            else if (pFilaFacetaOC.TipoPropiedad == (short)TipoPropiedadFaceta.TextoInvariable)
            {
                faceta.Type = TipoFaceta.TextoInvariable;
            }
            else if (pFilaFacetaOC.TipoPropiedad == (short)TipoPropiedadFaceta.Numero)
            {
                faceta.Type = TipoFaceta.Numero;
            }
            else
            {
                faceta.Type = TipoFaceta.Fecha;
            }

            faceta.ClaveFaceta = pFilaFacetaOC.Faceta;
            faceta.ObjetosConocimiento = [pFilaFacetaOC.ObjetoConocimiento.ToLower()];
            faceta.Presentacion = pFilaFacetaOC.Mayusculas;
            faceta.Disenyo = pFilaFacetaOC.TipoDisenio;
            faceta.AlgoritmoTransformacion = pFilaFacetaOC.AlgoritmoTransformacion;

            faceta.NumElementosVisibles = pFilaFacetaOC.ElementosVisibles;

            faceta.EsSemantica = pFilaFacetaOC.EsSemantica;
            faceta.Autocompletar = pFilaFacetaOC.Autocompletar;

            faceta.Comportamiento = 0;
            faceta.Excluyente = false;
            faceta.Reciprocidad = "";

            faceta.OcultaEnFacetas = false;
            faceta.OcultaEnFiltros = false;
            faceta.PrivacidadGrupos = new Dictionary<Guid, string>();
            faceta.Condicion = "";
            faceta.PriorizarOrdenResultados = false;
            faceta.Orden = pFilaFacetaOC.Orden;
            return faceta;
        }


        #endregion

        #region Metodos de Guardado

        public void GuardarFacetas(List<FacetaModel> pListaFacetas, bool pMantenerOrden = false)
        {
            List<string> listaFacetasAutocompletarRabbit = new List<string>();
            //Eliminar las que no vienen del modelo
            List<FacetaObjetoConocimientoProyecto> listaFacetas = FacetaDW.ListaFacetaObjetoConocimientoProyecto.ToList();

            foreach (FacetaObjetoConocimientoProyecto faceta in listaFacetas.Where(item => item.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && item.ProyectoID.Equals(ProyectoSeleccionado.Clave)))
            {
                FacetaModel filaFacetaModel;
                // Si los niveles de la faceta es igual al nivel de la reciprocidad, no hay clave de faceta y el contenido está en la propiedad Reciprocidad
                if ((faceta.Faceta.Count(item => item.Equals('@')) + 3) / 3 == faceta.Reciproca)
                {
                    filaFacetaModel = pListaFacetas.FirstOrDefault(item => string.IsNullOrEmpty(item.ClaveFaceta) && faceta.Faceta.Equals(item.Reciprocidad));
                }
                else
                {
                    filaFacetaModel = pListaFacetas.FirstOrDefault(item => !string.IsNullOrEmpty(item.ClaveFaceta) && (item.ClaveFaceta.Equals(faceta.Faceta) || (faceta.Reciproca > 0 && faceta.Faceta.Equals(item.Reciprocidad + "@@@" + item.ClaveFaceta))) && item.ObjetosConocimiento.Contains(faceta.ObjetoConocimiento.ToLower()));
                }

                if (filaFacetaModel == null)
                {
                    if (faceta.FacetaObjetoConocimientoProyectoPestanya != null && faceta.FacetaObjetoConocimientoProyectoPestanya.Count > 0)
                    {
                        foreach (FacetaObjetoConocimientoProyectoPestanya facetaObjetoConocimientoPestanya in faceta.FacetaObjetoConocimientoProyectoPestanya.ToList())
                        {
                            mEntityContext.EliminarElemento(facetaObjetoConocimientoPestanya);
                            FacetaDW.ListaFacetaObjetoConocimientoProyectoPenstanya.Remove(facetaObjetoConocimientoPestanya);
                        }
                    }
                    EliminarFaceta(faceta);
                    FacetaDW.ListaFacetaObjetoConocimientoProyecto.Remove(faceta);
                    listaFacetasAutocompletarRabbit.AddRange(AgregarFilasColaFacetasAutocompletar(ProyectoSeleccionado.FilaProyecto.TablaBaseProyectoID, faceta.Faceta, faceta.ObjetoConocimiento, 1));
                }
            }

            try
            {
                List<KeyValuePair<string, string>> listaFacetasNuevas = new List<KeyValuePair<string, string>>();

                List<string> facetasExcluidas = FacetaDW.ListaFacetaExcluida.Select(item => item.Faceta).Distinct().ToList();

                foreach (string faceta in FacetaDW.ListaFacetaObjetoConocimiento.Where(item => !facetasExcluidas.Contains(item.Faceta)).Select(item => item.Faceta))
                {
                    facetasExcluidas.Add(faceta);
                    //La excluimos
                    FacetaExcluida filaFacetaExcluida = new FacetaExcluida();
                    filaFacetaExcluida.ProyectoID = ProyectoSeleccionado.Clave;
                    filaFacetaExcluida.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                    filaFacetaExcluida.Faceta = faceta;
                    FacetaDW.ListaFacetaExcluida.Add(filaFacetaExcluida);
                    mEntityContext.FacetaExcluida.Add(filaFacetaExcluida);
                }

                //Añadir las nuevas
                foreach (FacetaModel faceta in pListaFacetas)
                {
                    if (!faceta.Deleted)
                    {
                        short reciproca = 0;

                        string reciprocaString = faceta.Reciprocidad;
                        if (!string.IsNullOrEmpty(reciprocaString))
                        {
                            if (short.TryParse(reciprocaString, out reciproca))
                            {
                                throw new ExcepcionGeneral("En un nodo 'Reciproca' no puede ir un número");
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(faceta.ClaveFaceta))
                                {
                                    faceta.ClaveFacetaYReprocidad = reciprocaString;
                                }
                                else
                                {
                                    faceta.ClaveFacetaYReprocidad = reciprocaString + "@@@" + faceta.ClaveFaceta;
                                }
                            }
                        }

                        if (!pMantenerOrden)
                        {
                            faceta.Orden = (short)(faceta.Orden * 100);
                        }

                        foreach (string objetoConocimiento in ListaOntologias.Keys.Where(item => faceta.ObjetosConocimiento.Contains(item)))
                        {
                            string columnaFaceta = faceta.ClaveFaceta;
                            if (string.IsNullOrEmpty(columnaFaceta))
                            {
                                columnaFaceta = faceta.Reciprocidad;
                            }
                            else if (!string.IsNullOrEmpty(faceta.Reciprocidad))
                            {
                                columnaFaceta = $"{faceta.Reciprocidad}@@@{columnaFaceta}";
                            }

                            FacetaObjetoConocimientoProyecto filaFaceta = FacetaDW.ListaFacetaObjetoConocimientoProyecto.FirstOrDefault(item => item.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.ObjetoConocimiento.ToLower().Equals(objetoConocimiento.ToLower()) && item.Faceta.Equals(columnaFaceta));

                            if (filaFaceta == null)
                            {
                                KeyValuePair<string, string> claveFacetaOC = new KeyValuePair<string, string>(columnaFaceta, objetoConocimiento);
                                listaFacetasNuevas.Add(claveFacetaOC);

                                listaFacetasAutocompletarRabbit.AddRange(AgregarFilaFacetaNueva(faceta, objetoConocimiento, true));
                            }
                        }
                    }
                }

                //Modificar las que tienen cambios
                foreach (FacetaModel faceta in pListaFacetas)
                {
                    if (!faceta.Deleted)
                    {
                        foreach (string objetoConocimiento in ListaOntologias.Keys.Where(item => faceta.ObjetosConocimiento.Contains(item)))
                        {
                            string columnaFaceta = faceta.ClaveFaceta;
                            if (string.IsNullOrEmpty(columnaFaceta))
                            {
                                columnaFaceta = faceta.Reciprocidad;
                            }
                            else if (!string.IsNullOrEmpty(faceta.Reciprocidad))
                            {
                                columnaFaceta = $"{faceta.Reciprocidad}@@@{columnaFaceta}";
                            }

                            KeyValuePair<string, string> claveFacetaOC = new KeyValuePair<string, string>(columnaFaceta, objetoConocimiento);

                            if (!listaFacetasNuevas.Contains(claveFacetaOC))
                            {
                                FacetaObjetoConocimientoProyecto filaFaceta = FacetaDW.ListaFacetaObjetoConocimientoProyecto.FirstOrDefault(item => item.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.ObjetoConocimiento.ToLower().Equals(objetoConocimiento) && item.Faceta.Equals(columnaFaceta));

                                if (filaFaceta != null)
                                {
                                    listaFacetasAutocompletarRabbit.AddRange(ModificarFilaFaceta(filaFaceta, faceta, true));
                                }
                            }
                        }
                    }
                }

                //Eliminar las eliminadas
                foreach (FacetaModel faceta in pListaFacetas)
                {
                    foreach (string objetoConocimiento in ListaOntologias.Keys)
                    {
                        if (faceta.Deleted || !faceta.ObjetosConocimiento.Contains(objetoConocimiento))
                        {
                            string claveFaceta = faceta.ClaveFaceta;
                            if (string.IsNullOrEmpty(claveFaceta))
                            {
                                claveFaceta = faceta.Reciprocidad;
                            }
                            else if (!string.IsNullOrEmpty(faceta.Reciprocidad))
                            {
                                claveFaceta = $"{faceta.Reciprocidad}@@@{claveFaceta}";
                            }

                            FacetaObjetoConocimientoProyecto filaFaceta = FacetaDW.ListaFacetaObjetoConocimientoProyecto.FirstOrDefault(item => item.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.ObjetoConocimiento.ToLower().Equals(objetoConocimiento) && item.Faceta.Equals(claveFaceta));

                            if (filaFaceta != null)
                            {
                                //Si estamos eliminando una faceta, pero hemos creado otra igual, no la borramos
                                //Por ejemplo, 
                                if (pListaFacetas.Find(fac => (fac.ClaveFaceta.Equals(filaFaceta.Faceta) && string.IsNullOrEmpty(fac.ClaveFacetaYReprocidad) || !string.IsNullOrEmpty(fac.ClaveFacetaYReprocidad) && fac.ClaveFacetaYReprocidad.Equals(filaFaceta.Faceta)) && !fac.Deleted && fac.ObjetosConocimiento.Contains(objetoConocimiento)) == null)
                                {
                                    if (filaFaceta.FacetaObjetoConocimientoProyectoPestanya != null && filaFaceta.FacetaObjetoConocimientoProyectoPestanya.Count > 0)
                                    {
                                        foreach (FacetaObjetoConocimientoProyectoPestanya facetaObjetoConocimientoPestanya in filaFaceta.FacetaObjetoConocimientoProyectoPestanya.ToList())
                                        {
                                            mEntityContext.EliminarElemento(facetaObjetoConocimientoPestanya);
                                            FacetaDW.ListaFacetaObjetoConocimientoProyectoPenstanya.Remove(facetaObjetoConocimientoPestanya);
                                        }
                                    }
                                    listaFacetasAutocompletarRabbit.AddRange(EliminarFaceta(filaFaceta));
                                }
                            }
                        }
                    }
                }

                //Eliminar las que no se encuentran (se ha cambiado la clave de la faceta)
                foreach (FacetaObjetoConocimientoProyecto filaFaceta in FacetaDW.ListaFacetaObjetoConocimientoProyecto)
                {
                    if (mEntityContext.Entry(filaFaceta).State == EntityState.Deleted && pListaFacetas.Any(faceta => faceta.ClaveFaceta == filaFaceta.Faceta))
                    {
                        if (filaFaceta.FacetaObjetoConocimientoProyectoPestanya != null && filaFaceta.FacetaObjetoConocimientoProyectoPestanya.Count > 0)
                        {
                            foreach (FacetaObjetoConocimientoProyectoPestanya facetaObjetoConocimientoPestanya in filaFaceta.FacetaObjetoConocimientoProyectoPestanya.ToList())
                            {
                                mEntityContext.EliminarElemento(facetaObjetoConocimientoPestanya);
                                FacetaDW.ListaFacetaObjetoConocimientoProyectoPenstanya.Remove(facetaObjetoConocimientoPestanya);
                            }
                        }
                        listaFacetasAutocompletarRabbit.AddRange(EliminarFaceta(filaFaceta));
                    }
                }

                FacetaCN facetaCN = new FacetaCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<FacetaCN>(), mLoggerFactory);
                facetaCN.Actualizar();
                facetaCN.Dispose();
                AgregarColaFacetasAutocompletarRabbit(listaFacetasAutocompletarRabbit);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mLogger);
                throw;
            }
        }


        private List<string> EliminarFaceta(FacetaObjetoConocimientoProyecto pFilaFaceta)
        {
            List<FacetaFiltroProyecto> filasFiltros = pFilaFaceta.FacetaFiltroProyecto.ToList();

            foreach (FacetaFiltroProyecto filaFiltro in filasFiltros)
            {
                pFilaFaceta.FacetaFiltroProyecto.Remove(filaFiltro);
                mEntityContext.EliminarElemento(filaFiltro);
                FacetaDW.ListaFacetaFiltroProyecto.Remove(filaFiltro);
            }

            FacetaDW.ListaFacetaObjetoConocimientoProyecto.Remove(pFilaFaceta);
            mEntityContext.EliminarElemento(pFilaFaceta);
            return AgregarFilasColaFacetasAutocompletar(ProyectoSeleccionado.FilaProyecto.TablaBaseProyectoID, pFilaFaceta.Faceta, pFilaFaceta.ObjetoConocimiento, 1);
        }

        private List<string> AgregarFilaFacetaNueva(FacetaModel pFaceta, string pObjetoConocimiento, bool pMantenerOrden = false)
        {
            FacetaObjetoConocimientoProyecto filaFacetaNueva = new FacetaObjetoConocimientoProyecto();
            try
            {
                filaFacetaNueva.ProyectoID = ProyectoSeleccionado.Clave;
                filaFacetaNueva.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                filaFacetaNueva.ObjetoConocimiento = pObjetoConocimiento;
                filaFacetaNueva.FechaCreacion = DateTime.Now;

                TipoPropiedadFaceta tipoPropiedad = TipoPropiedadFaceta.Texto;
                if (pFaceta.Type == TipoFaceta.Numero)
                {
                    tipoPropiedad = TipoPropiedadFaceta.Numero;
                }
                else if (pFaceta.Type == TipoFaceta.Fecha)
                {
                    tipoPropiedad = TipoPropiedadFaceta.Fecha;
                }
                else if (pFaceta.Type == TipoFaceta.TextoInvariable)
                {
                    tipoPropiedad = TipoPropiedadFaceta.TextoInvariable;
                }
                else if (pFaceta.Type == TipoFaceta.Siglo)
                {
                    pFaceta.AlgoritmoTransformacion = (short)TiposAlgoritmoTransformacion.Siglo;
                    tipoPropiedad = TipoPropiedadFaceta.Siglo;
                }

                filaFacetaNueva.TipoPropiedad = (short)tipoPropiedad;

                filaFacetaNueva.ComportamientoOr = false;
                if (pFaceta != null)
                {
                    filaFacetaNueva.Inmutable = pFaceta.Inmutable;
                }
                else
                {
                    filaFacetaNueva.Inmutable = false;
                }
                filaFacetaNueva.MostrarSoloCaja = false;

                ModificarFilaFaceta(filaFacetaNueva, pFaceta, pMantenerOrden);
                mEntityContext.FacetaObjetoConocimientoProyecto.Add(filaFacetaNueva);
                FacetaDW.ListaFacetaObjetoConocimientoProyecto.Add(filaFacetaNueva);

                return AgregarFilasColaFacetasAutocompletar(ProyectoSeleccionado.FilaProyecto.TablaBaseProyectoID, filaFacetaNueva.Faceta, filaFacetaNueva.ObjetoConocimiento, 0);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError("", mLogger);
                string mensaje = "Ha ocurrido un error al intentar guardar las facetas.";
                if (FacetaDW.ListaFacetaObjetoConocimientoProyecto.Any(item => item.OrganizacionID.Equals(filaFacetaNueva.OrganizacionID) && item.ProyectoID.Equals(filaFacetaNueva.ProyectoID) && item.Faceta.Equals(filaFacetaNueva.Faceta) && item.ObjetoConocimiento.Equals(filaFacetaNueva.ObjetoConocimiento)))
                {
                    mensaje = $"Ya existe una faceta con el mismo objeto de conocimiento ({filaFacetaNueva.ObjetoConocimiento}) y la misma faceta ({filaFacetaNueva.Faceta})";
                }
                throw new Exception(mensaje, ex);
            }
        }


        private List<string> ModificarFilaFaceta(FacetaObjetoConocimientoProyecto pFilaFaceta, FacetaModel pFaceta, bool pMantenerOrden = false)
        {
            List<string> listaFacetasAutocompletarRabbit = new List<string>();
            pFilaFaceta.NombreFaceta = pFaceta.Name;
            pFilaFaceta.FechaCreacion = DateTime.Now;

            short reciproca = 0;

            string reciprocaString = pFaceta.Reciprocidad;
            if (!string.IsNullOrEmpty(reciprocaString))
            {
                if (short.TryParse(reciprocaString, out reciproca))
                {
                    throw new ExcepcionGeneral("En un nodo 'Reciproca' no puede ir un número");
                }
                else
                {
                    reciproca = (short)reciprocaString.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries).Length;
                }
            }

            if (!string.IsNullOrEmpty(pFaceta.ClaveFacetaYReprocidad))
            {
                pFilaFaceta.Faceta = pFaceta.ClaveFacetaYReprocidad;
            }
            else
            {
                pFilaFaceta.Faceta = pFaceta.ClaveFaceta;
            }

            pFilaFaceta.Reciproca = reciproca;
            pFilaFaceta.MostrarContador = pFaceta.MostrarContador;

            pFilaFaceta.Mayusculas = pFaceta.Presentacion;
            pFilaFaceta.TipoDisenio = pFaceta.Disenyo;
            pFilaFaceta.AlgoritmoTransformacion = pFaceta.AlgoritmoTransformacion;
            pFilaFaceta.ElementosVisibles = pFaceta.NumElementosVisibles;

            pFilaFaceta.EsSemantica = true;
            pFilaFaceta.Autocompletar = pFaceta.Autocompletar;
            pFilaFaceta.ComportamientoOr = pFaceta.ComportamientoOr;
            pFilaFaceta.Comportamiento = pFaceta.Comportamiento;
            pFilaFaceta.Excluyente = false;
            pFilaFaceta.OcultaEnFacetas = pFaceta.OcultaEnFacetas;
            pFilaFaceta.OcultaEnFiltros = pFaceta.OcultaEnFiltros;
            pFilaFaceta.PriorizarOrdenResultados = pFaceta.PriorizarOrdenResultados;
            pFilaFaceta.Inmutable = pFaceta.Inmutable;

            if (pFaceta.AgrupacionID != null)
            {
                pFilaFaceta.AgrupacionID = pFaceta.AgrupacionID.Value;
            }

            string facetaPrivadaParaGrupoEditores = "";
            if (pFaceta.PrivacidadGrupos != null)
            {
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);

                pFaceta.PrivacidadGrupos = identidadCN.ObtenerNombresDeGrupos(pFaceta.PrivacidadGrupos.Keys.ToList());

                List<string> listaNombresCortos = identidadCN.ObtenerNombresCortosGruposPorID(pFaceta.PrivacidadGrupos.Keys.ToList());

                facetaPrivadaParaGrupoEditores = string.Join('|', listaNombresCortos);
            }

            pFilaFaceta.FacetaPrivadaParaGrupoEditores = facetaPrivadaParaGrupoEditores;

            pFilaFaceta.Condicion = "";
            if (!string.IsNullOrEmpty(pFaceta.Condicion))
            {
                pFilaFaceta.Condicion = pFaceta.Condicion;
            }

            if (!pMantenerOrden)
            {
                if (!pFaceta.Orden.Equals(pFilaFaceta.Orden))
                {
                    pFaceta.Orden = (short)(pFaceta.Orden * 100);
                    pFilaFaceta.Orden = pFaceta.Orden;
                }
                else
                {
                    pFilaFaceta.Orden = pFaceta.Orden;
                }
            }
            else
            {
                pFilaFaceta.Orden = pFaceta.Orden;
            }

            List<FacetaFiltroProyecto> filasFiltros = pFilaFaceta.FacetaFiltroProyecto.ToList();
            short contFiltros = 0;

            if (filasFiltros.Count > 0)
            {
                foreach (FacetaFiltroProyecto filaFiltro in filasFiltros)
                {
                    pFilaFaceta.FacetaFiltroProyecto.Remove(filaFiltro);
                    mEntityContext.EliminarElemento(filaFiltro);
                }
            }

            //Realizar aqui los metodos para insertar los Filtros. Mediante la lista.
            if (pFaceta.ListaFiltrosFacetas != null)
            {
                foreach (FiltrosFacetas filtroFaceta in pFaceta.ListaFiltrosFacetas)
                {
                    if (!filtroFaceta.Deleted)
                    {
                        FacetaFiltroProyecto filaFiltro = new FacetaFiltroProyecto();
                        filaFiltro.OrganizacionID = pFilaFaceta.OrganizacionID;
                        filaFiltro.ProyectoID = pFilaFaceta.ProyectoID;
                        filaFiltro.ObjetoConocimiento = pFilaFaceta.ObjetoConocimiento;
                        filaFiltro.Faceta = pFilaFaceta.Faceta;
                        filaFiltro.Filtro = filtroFaceta.Nombre;
                        filaFiltro.Condicion = filtroFaceta.Condicion;
                        filaFiltro.Orden = (short)(pFilaFaceta.Orden + contFiltros);

                        FacetaDW.ListaFacetaFiltroProyecto.Add(filaFiltro);
                        mEntityContext.FacetaFiltroProyecto.Add(filaFiltro);
                        contFiltros++;
                    }
                }
            }
            if (pFaceta.Modified)
            {
                return AgregarFilasColaFacetasAutocompletar(ProyectoSeleccionado.FilaProyecto.TablaBaseProyectoID, pFilaFaceta.Faceta, pFilaFaceta.ObjetoConocimiento, 2);
            }

            return listaFacetasAutocompletarRabbit;
        }

        public static string ObtenerFilaFaceta(int pTablaBaseProyectoID, string pfaceta, string pObjetoConocimiento, bool pAgregar)
        {
            int tipo = 0;
            if (!pAgregar)
            {
                tipo = 1;
            }
            string fila = $"[{pTablaBaseProyectoID},'{Constantes.FACETA}{pfaceta}{Constantes.FACETA},{Constantes.OBJETO_CONOCIMIENTO}{pObjetoConocimiento}{Constantes.OBJETO_CONOCIMIENTO}',{tipo},\"2022-01-26T10:43:12.3492277\", null]";
            return fila;
        }

        public static List<string> AgregarFilasColaFacetasAutocompletar(int pTablaBaseProyectoID, string pfaceta, string pObjetoConocimiento, short pTipo)
        {
            List<string> filasFacetas = new List<string>();
            if (pTipo.Equals(0))
            {//Añadir
                filasFacetas.Add(ObtenerFilaFaceta(pTablaBaseProyectoID, pfaceta, pObjetoConocimiento, true));
            }
            else if (pTipo.Equals(1))
            {//Eliminar
                filasFacetas.Add(ObtenerFilaFaceta(pTablaBaseProyectoID, pfaceta, pObjetoConocimiento, false));
            }
            else
            {//Modificar
                filasFacetas.Add(ObtenerFilaFaceta(pTablaBaseProyectoID, pfaceta, pObjetoConocimiento, false));
                filasFacetas.Add(ObtenerFilaFaceta(pTablaBaseProyectoID, pfaceta, pObjetoConocimiento, true));
            }
            return filasFacetas;
        }

        public void AgregarColaFacetasAutocompletarRabbit(List<string> pFilasFacetas)
        {
            using (RabbitMQClient rMQ = new RabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN, "ColaFacetasGeneradorAutocompletar", mLoggingService, mConfigService, mLoggerFactory.CreateLogger<RabbitMQClient>(), mLoggerFactory))
            {
                rMQ.AgregarElementosACola(pFilasFacetas);
            }
        }

        public void CrearFilasPropiedadesIntegracionContinua(List<FacetaModel> pListaFacetas)
        {
            List<IntegracionContinuaPropiedad> propiedadesIntegracionContinua = new List<IntegracionContinuaPropiedad>();

            try
            {
                foreach (FacetaModel faceta in pListaFacetas)
                {
                    if (!faceta.Deleted && faceta.Type.Equals(TipoFaceta.Tesauro) && !string.IsNullOrEmpty(faceta.Filtros))
                    {
                        //Crear las filas de las porpiedades de Integracion Continua
                        IntegracionContinuaPropiedad propiedadRutaPagina = new IntegracionContinuaPropiedad();
                        propiedadRutaPagina.ProyectoID = ProyectoSeleccionado.Clave;
                        propiedadRutaPagina.TipoObjeto = (short)TipoObjeto.Faceta;
                        propiedadRutaPagina.ObjetoPropiedad = faceta.ClaveFaceta;
                        propiedadRutaPagina.TipoPropiedad = (short)TipoPropiedad.FiltrosFaceta;
                        propiedadRutaPagina.ValorPropiedad = "";
                        foreach (FiltrosFacetas filtroFaceta in faceta.ListaFiltrosFacetas)
                        {
                            propiedadRutaPagina.ValorPropiedad = $"{propiedadRutaPagina.ValorPropiedad}{filtroFaceta.Nombre}|||{filtroFaceta.Condicion}&&&";
                        }
                        if (!propiedadesIntegracionContinua.Contains(propiedadRutaPagina))
                        {
                            propiedadesIntegracionContinua.Add(propiedadRutaPagina);
                        }
                    }

                }

                using (ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory))
                {
                    proyCN.CrearFilasIntegracionContinuaParametro(propiedadesIntegracionContinua, ProyectoSeleccionado.Clave, TipoObjeto.Faceta);
                }
            }
            catch
            {
                // Si falla al generar los datos de integración continua no rompemos la ejecución
            }
        }

        public void ModificarFilasIntegracionContinuaEntornoSiguiente(List<FacetaModel> pListaFacetas, string UrlApiDesplieguesEntornoSiguiente, Guid pUsuarioID)
        {
            List<IntegracionContinuaPropiedad> propiedadesIntegracionContinua = new List<IntegracionContinuaPropiedad>();
            try
            {
                foreach (FacetaModel faceta in pListaFacetas)
                {
                    if (!faceta.Deleted && faceta.Type.Equals(TipoFaceta.Tesauro) && !string.IsNullOrEmpty(faceta.Filtros))
                    {
                        //Crear las filas de las porpiedades de Integracion Continua
                        IntegracionContinuaPropiedad propiedadRutaPagina = new IntegracionContinuaPropiedad();
                        propiedadRutaPagina.ProyectoID = ProyectoSeleccionado.Clave;
                        propiedadRutaPagina.TipoObjeto = (short)TipoObjeto.Faceta;
                        propiedadRutaPagina.ObjetoPropiedad = faceta.ClaveFaceta;
                        propiedadRutaPagina.TipoPropiedad = (short)TipoPropiedad.FiltrosFaceta;
                        propiedadRutaPagina.ValorPropiedad = faceta.Filtros;
                        propiedadesIntegracionContinua.Add(propiedadRutaPagina);
                    }
                }

                string peticion = $"{UrlApiDesplieguesEntornoSiguiente}/PropiedadesIntegracion?nombreProy={ProyectoSeleccionado.NombreCorto}&UsuarioID={pUsuarioID}";
                _ = UtilWeb.WebRequestPostWithJsonObject(peticion, propiedadesIntegracionContinua, "");
            }
            catch
            {
                // Si falla al modificar los datos de integración continua no rompemos la ejecución
            }
        }


        public void GuardarFaceta(FacetaModel pFaceta, bool pMantenerOrden = false)
        {
            try
            {
                List<KeyValuePair<string, string>> listaFacetasNuevas = new List<KeyValuePair<string, string>>();

                //Añadir las nuevas
                if (!pFaceta.Deleted)
                {
                    short reciproca = 0;

                    string reciprocaString = pFaceta.Reciprocidad;
                    if (!string.IsNullOrEmpty(reciprocaString))
                    {
                        if (short.TryParse(reciprocaString, out reciproca))
                        {
                            throw new ExcepcionGeneral("En un nodo 'Reciproca' no puede ir un número");
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(pFaceta.ClaveFaceta))
                            {
                                pFaceta.ClaveFaceta = reciprocaString;
                                pFaceta.ClaveFacetaYReprocidad = reciprocaString;
                            }
                            else
                            {
                                pFaceta.ClaveFacetaYReprocidad = reciprocaString + "@@@" + pFaceta.ClaveFaceta;
                            }
                        }
                    }

                    if (!pMantenerOrden)
                    {
                        pFaceta.Orden = (short)(pFaceta.Orden * 100);
                    }

                    foreach (string objetoConocimiento in ListaOntologias.Keys.Where(item => pFaceta.ObjetosConocimiento.Contains(item)))
                    {
                        string claveFaceta = pFaceta.ClaveFaceta;
                        if (!string.IsNullOrEmpty(pFaceta.Reciprocidad))
                        {
                            claveFaceta = $"{pFaceta.Reciprocidad}@@@{claveFaceta}";
                        }
                        FacetaObjetoConocimientoProyecto filaFaceta = FacetaDW.ListaFacetaObjetoConocimientoProyecto.FirstOrDefault(item => item.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.ObjetoConocimiento.ToLower().Equals(objetoConocimiento) && item.Faceta.Equals(claveFaceta));
                        if (filaFaceta == null)
                        {
                            KeyValuePair<string, string> claveFacetaOC = new KeyValuePair<string, string>(claveFaceta, objetoConocimiento);
                            listaFacetasNuevas.Add(claveFacetaOC);

                            AgregarFilaFacetaNueva(pFaceta, objetoConocimiento, true);
                        }
                    }
                }

                //Modificar las que tienen cambios
                if (!pFaceta.Deleted)
                {
                    foreach (string objetoConocimiento in ListaOntologias.Keys.Where(item => pFaceta.ObjetosConocimiento.Contains(item)))
                    {
                        string claveFaceta = pFaceta.ClaveFaceta;
                        if (!string.IsNullOrEmpty(pFaceta.Reciprocidad))
                        {
                            claveFaceta = $"{pFaceta.Reciprocidad}@@@{claveFaceta}";
                        }
                        KeyValuePair<string, string> claveFacetaOC = new KeyValuePair<string, string>(claveFaceta, objetoConocimiento);

                        if (!listaFacetasNuevas.Contains(claveFacetaOC))
                        {
                            FacetaObjetoConocimientoProyecto filaFaceta = FacetaDW.ListaFacetaObjetoConocimientoProyecto.FirstOrDefault(item => item.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.ObjetoConocimiento.ToLower().Equals(objetoConocimiento) && item.Faceta.Equals(claveFaceta));
                            if (filaFaceta != null)
                            {
                                ModificarFilaFaceta(filaFaceta, pFaceta, true);
                            }
                        }
                    }
                }

                foreach (string objetoConocimiento in ListaOntologias.Keys)
                {
                    if (pFaceta.Deleted || !pFaceta.ObjetosConocimiento.Contains(objetoConocimiento))
                    {
                        string claveFaceta = pFaceta.ClaveFaceta;
                        if (!string.IsNullOrEmpty(pFaceta.Reciprocidad))
                        {
                            claveFaceta = $"{pFaceta.Reciprocidad}@@@{claveFaceta}";
                        }
                        FacetaObjetoConocimientoProyecto filaFaceta = FacetaDW.ListaFacetaObjetoConocimientoProyecto.FirstOrDefault(item => item.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.ObjetoConocimiento.ToLower().Equals(objetoConocimiento) && item.Faceta.Equals(claveFaceta));
                        if (filaFaceta != null)
                        {

                            EliminarFaceta(filaFaceta);
                        }
                    }
                }

                FacetaCN facetaCN = new FacetaCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<FacetaCN>(), mLoggerFactory);
                facetaCN.Actualizar();
                facetaCN.Dispose();
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mLogger);
            }
        }

        #endregion

        #region Gestion de Errores

        /// <summary>
        /// Comprueba que las facetas que se pretenden guardar desde la administración no tengan errores
        /// </summary>
        /// <param name="pListaFacetas">Lista de facetas que se quieren guardar</param>
        /// <returns>Un mensaje de error indicando el problema en caso de haberlo</returns>
        public string ComprobarErrores(List<FacetaModel> pListaFacetas)
        {
            string error = string.Empty;

            error += ComprobarErrorInmutable(pListaFacetas);
            error += ComprobarFacetasRepetidas(pListaFacetas);

            return error;
        }

        /// <summary>
        /// Se comprueba si se ha configurado alguna faceta inmutable sin marcar el comportamiento OR. 
        /// En caso de haberlo hecho se devuelve un mensaje de error indicándolo.
        /// </summary>
        /// <param name="pListaFacetas">Lista de facetas que se quieren guardar</param>
        /// <returns>Un mensaje de error indicando el problema en caso de haberlo</returns>
        private static string ComprobarErrorInmutable(List<FacetaModel> pListaFacetas)
        {
            foreach (FacetaModel faceta in pListaFacetas)
            {
                if (faceta.Inmutable && !faceta.ComportamientoOr)
                {
                    return "Si se elige una faceta inmutable, el comportamientoOR tiene que estar marcado también";
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Se comprueba que al entre las facetas guardadas no hay dos con la misma clave de faceta y objetos de conocimiento repetidos.
        /// En caso de hacerlo se devuelve un mensaje de error indicándolo.
        /// </summary>
        /// <param name="pListaFacetas">Lista de facetas que se quieren guardar</param>
        /// <returns>Un mensaje de error indicando el problema en caso de haberlo</returns>
        private static string ComprobarFacetasRepetidas(List<FacetaModel> pListaFacetas)
        {
            Dictionary<string, List<string>> facetasObjetosConocimiento = new Dictionary<string, List<string>>();
            foreach (FacetaModel faceta in pListaFacetas)
            {
                string datosFaceta = faceta.ClaveFaceta + faceta.Reciprocidad;
                if (!facetasObjetosConocimiento.ContainsKey(datosFaceta))
                {
                    facetasObjetosConocimiento.Add(datosFaceta, faceta.ObjetosConocimiento);
                }
                else
                {
                    foreach (string objetoConocimiento in faceta.ObjetosConocimiento)
                    {
                        if (facetasObjetosConocimiento[datosFaceta].Contains(objetoConocimiento))
                        {
                            return "No puede haber facetas repetidas con los mismos objetos de conocimiento y la misma reprocidad";
                        }
                        else
                        {
                            facetasObjetosConocimiento[datosFaceta].Add(objetoConocimiento);
                        }
                    }
                }
            }

            return string.Empty;
        }

        #endregion

        #region Invalidar Caches

        public void InvalidarCaches(string UrlIntragnoss)
        {
            FacetaCL facetaCL = new FacetaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, null, mLoggerFactory.CreateLogger<FacetaCL>(), mLoggerFactory);
            bool cachearFacetas = !(ParametroProyecto.ContainsKey("CacheFacetas") && ParametroProyecto["CacheFacetas"].Equals("0"));
            facetaCL.InvalidarCacheFacetasProyecto(ProyectoSeleccionado.Clave, cachearFacetas);

            FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, null, mLoggerFactory.CreateLogger<FacetadoCL>(), mLoggerFactory);
            facetadoCL.InvalidarResultadosYFacetasDeBusquedaEnProyecto(ProyectoSeleccionado.Clave, "*");

            mGnossCache.VersionarCacheLocal(ProyectoSeleccionado.Clave);

            CargadorFacetas cargadorFacetas = new CargadorFacetas();
            cargadorFacetas.Url = mConfigService.ObtenerUrlServicioFacetas();
            cargadorFacetas.InvalidarCacheLocalServicioFacetas(ProyectoSeleccionado.Clave);
        }

        #endregion

        #region Propiedades

        private DataWrapperFacetas FacetaDW
        {
            get
            {
                if (mFacetaDW == null)
                {
                    FacetaCN facetaCN = new FacetaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetaCN>(), mLoggerFactory);
                    mFacetaDW = facetaCN.ObtenerFacetasAdministrarProyecto(ProyectoSeleccionado.Clave);
                    facetaCN.Dispose();
                }
                return mFacetaDW;
            }
        }

        #endregion

    }
}