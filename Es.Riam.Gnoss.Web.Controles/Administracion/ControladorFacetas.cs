using Es.Riam.AbstractsOpen;
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
using Es.Riam.Gnoss.Servicios.ControladoresServiciosWeb;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Util;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static Es.Riam.Gnoss.Web.MVC.Models.Administracion.FacetaModel;

namespace Es.Riam.Gnoss.Web.Controles.Administracion
{
    public class ControladorFacetas
    {
        private DataWrapperFacetas mFacetaDW = null;
        private Proyecto ProyectoSeleccionado = null;
        private Dictionary<string, string> ParametroProyecto = null;
        private Dictionary<string, string> ListaOntologias = null;

        private bool CrearFilasPropiedadesExportacion = false;
        private List<IntegracionContinuaPropiedad> propiedadesIntegracionContinua = null;

        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private RedisCacheWrapper mRedisCacheWrapper;
        private GnossCache mGnossCache;

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public ControladorFacetas(Proyecto pProyecto, Dictionary<string, string> pParametroProyecto, Dictionary<string, string> pListaOntologias, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication pServicesVirtuosoAndReplication, bool pCrearFilasPropiedadesExportacion = false)
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

            CrearFilasPropiedadesExportacion = pCrearFilasPropiedadesExportacion;
        }

        #endregion

        #region Metodos de Carga

        public List<FacetaModel> CargarListadoFacetas()
        {
            List<FacetaModel> ListaFacetas = new List<FacetaModel>();

            List<string> facetasEscluidas = new List<string>();

            foreach (FacetaExcluida filaFacetaExcluida in FacetaDW.ListaFacetaExcluida)
            {
                if (!facetasEscluidas.Contains(filaFacetaExcluida.Faceta))
                {
                    facetasEscluidas.Add(filaFacetaExcluida.Faceta);
                }
            }

            propiedadesIntegracionContinua = new List<IntegracionContinuaPropiedad>();

            foreach (FacetaObjetoConocimientoProyecto filaFacetaOCProyecto in FacetaDW.ListaFacetaObjetoConocimientoProyecto)
            {
                List<FacetaFiltroProyecto> filasFiltros = filaFacetaOCProyecto.FacetaFiltroProyecto.ToList();

                string filtrosFaceta = "";
                foreach (FacetaFiltroProyecto filaFiltro in filasFiltros.OrderBy(filtro => filtro.Orden))
                {
                    filtrosFaceta += filaFiltro.Filtro + ',';
                }
                if (!string.IsNullOrEmpty(filtrosFaceta))
                {
                    filtrosFaceta.TrimEnd(',');
                }


                //FacetaModel faceta = ListaFacetas.Find(fac => fac.ClaveFacetaYReprocidad == filaFacetaOCProyecto.Faceta && fac.Filtros == filtrosFaceta);
                FacetaModel faceta = null;
                if (filaFacetaOCProyecto.AgrupacionID.HasValue)
                {
                    faceta = ListaFacetas.Find(fac => (fac.ClaveFaceta == filaFacetaOCProyecto.Faceta || (filaFacetaOCProyecto.Reciproca > 0 && fac.ClaveFacetaYReprocidad == filaFacetaOCProyecto.Faceta)) && fac.Filtros == filtrosFaceta && fac.AgrupacionID.Value.Equals(filaFacetaOCProyecto.AgrupacionID));
                }
                else
                {
                    faceta = ListaFacetas.Find(fac => (fac.ClaveFaceta == filaFacetaOCProyecto.Faceta || (filaFacetaOCProyecto.Reciproca > 0 && fac.ClaveFacetaYReprocidad == filaFacetaOCProyecto.Faceta)) && fac.Filtros == filtrosFaceta);
                }

                if (faceta == null)
                {
                    faceta = CargarFacetaProy(filaFacetaOCProyecto);

                    if (CrearFilasPropiedadesExportacion)
                    {
                        if (faceta.Type.Equals(TipoFaceta.Tesauro) && !string.IsNullOrEmpty(faceta.Filtros))
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
                    }
                    ListaFacetas.Add(faceta);
                }
                else
                {
                    faceta.ObjetosConocimiento.Add(filaFacetaOCProyecto.ObjetoConocimiento.ToLower());
                }
            }

            foreach (FacetaObjetoConocimiento filaFacetaOC in FacetaDW.ListaFacetaObjetoConocimiento)
            {
                if (!facetasEscluidas.Contains(filaFacetaOC.Faceta))
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
            }

            if (CrearFilasPropiedadesExportacion)
            {
                try
                {
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    proyCN.CrearFilasIntegracionContinuaParametro(propiedadesIntegracionContinua, ProyectoSeleccionado.Clave, TipoObjeto.Faceta);
                    proyCN.Dispose();
                }
                catch
                {

                }
            }

            return ListaFacetas;
        }


        public FacetaModel CargarFacetaProy(FacetaObjetoConocimientoProyecto pFilaFacetaOC)
        {
            FacetaModel faceta = new FacetaModel();
            faceta.Name = pFilaFacetaOC.NombreFaceta;

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
            /*else
            {
                faceta.ClaveFacetaYReprocidad = faceta.ClaveFaceta;
            }*/

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

            string filtrosFaceta = "";
            foreach (FacetaFiltroProyecto filaFiltro in filasFiltros.OrderBy(filtro => filtro.Orden))
            {
                filtrosFaceta += filaFiltro.Filtro + ',';
                FiltrosFacetas filtro = new FiltrosFacetas();
                filtro.Nombre = filaFiltro.Filtro;
                filtro.Condicion = filaFiltro.Condicion;

                listaFiltros.Add(filaFiltro.Filtro);
                listaFiltrosFiltro.Add(filtro);
            }
            if (!string.IsNullOrEmpty(filtrosFaceta))
            {
                filtrosFaceta.TrimEnd(',');
            }
            faceta.Filtros = filtrosFaceta;

            faceta.ListFiltro = listaFiltros;
            faceta.ListaFiltrosFacetas = listaFiltrosFiltro;

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, null);

            // TODO: Guardar en el campo FacetaPrivadaParaGrupoEditores el nombre de la organización y obtener el grupo por proyecto o organización, según proceda
            List<Guid> grupos = new List<Guid>();
            if (!string.IsNullOrEmpty(pFilaFacetaOC.FacetaPrivadaParaGrupoEditores))
            {
                grupos = identidadCN.ObtenerGruposIDPorNombreCortoYProyecto(new List<string>(pFilaFacetaOC.FacetaPrivadaParaGrupoEditores.Split('|')), pFilaFacetaOC.ProyectoID);
                if (grupos == null || grupos.Count() == 0)
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

            faceta.Condicion = pFilaFacetaOC.Condicion;
            faceta.Inmutable = pFilaFacetaOC.Inmutable;

            if (pFilaFacetaOC.AgrupacionID.HasValue)
            {
                faceta.AgrupacionID = pFilaFacetaOC.AgrupacionID;
            }


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
            //faceta.EsFacetaGenerica = true;
            faceta.ObjetosConocimiento = new List<string>();
            faceta.ObjetosConocimiento.Add(pFilaFacetaOC.ObjetoConocimiento.ToLower());
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
            //Eliminar las que no vienen del modelo
            List<FacetaObjetoConocimientoProyecto> listaFacetas = FacetaDW.ListaFacetaObjetoConocimientoProyecto.ToList();
            foreach (FacetaObjetoConocimientoProyecto faceta in listaFacetas)
            {
                if (faceta.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && faceta.ProyectoID.Equals(ProyectoSeleccionado.Clave))
                {
                    FacetaModel filaFacetaModel;
                    // Si los niveles de la faceta es igual al nivel de la reciprocidad, no hay clave de faceta y todo está en la propiedad Reciprocidad
                    if ((faceta.Faceta.Count(item => item.Equals('@')) + 3) / 3 == faceta.Reciproca)
                    {
                        filaFacetaModel = pListaFacetas.FirstOrDefault(item => string.IsNullOrEmpty(item.ClaveFaceta) && faceta.Faceta.Equals(item.Reciprocidad));
                    }
                    else
                    {
                        if (faceta.AgrupacionID.HasValue)
                        {
                            filaFacetaModel = pListaFacetas.FirstOrDefault(item => (!string.IsNullOrEmpty(item.ClaveFaceta) && item.ClaveFaceta.Equals(faceta.Faceta) || (faceta.Reciproca > 0 && faceta.Faceta.Equals(item.Reciprocidad + "@@@" + item.ClaveFaceta))) && item.AgrupacionID.Value.Equals(faceta.AgrupacionID.Value));
                        }
                        else
                        {
                            filaFacetaModel = pListaFacetas.FirstOrDefault(item => !string.IsNullOrEmpty(item.ClaveFaceta) && item.ClaveFaceta.Equals(faceta.Faceta) || (faceta.Reciproca > 0 && faceta.Faceta.Equals(item.Reciprocidad + "@@@" + item.ClaveFaceta)));
                        }
                        if (filaFacetaModel != null && !filaFacetaModel.ObjetosConocimiento.Contains(faceta.ObjetoConocimiento.ToLower()))
                        {
                            filaFacetaModel = null;
                        }
                    }
                    if (filaFacetaModel == null)
                    {
                        if (faceta.FacetaObjetoConocimientoProyectoPestanya != null && faceta.FacetaObjetoConocimientoProyectoPestanya.Count() > 0)
                        {
                            foreach (FacetaObjetoConocimientoProyectoPestanya facetaObjetoConocimientoPestanya in faceta.FacetaObjetoConocimientoProyectoPestanya.ToList())
                            {
                                mEntityContext.EliminarElemento(facetaObjetoConocimientoPestanya);
                                FacetaDW.ListaFacetaObjetoConocimientoProyectoPenstanya.Remove(facetaObjetoConocimientoPestanya);
                            }
                        }
                        mEntityContext.EliminarElemento(faceta);
                        FacetaDW.ListaFacetaObjetoConocimientoProyecto.Remove(faceta);
                    }

                }
            }

            try
            {
                List<KeyValuePair<string, string>> listaFacetasNuevas = new List<KeyValuePair<string, string>>();

                List<string> facetasEscluidas = new List<string>();

                foreach (FacetaExcluida filaFacetaExcluida in FacetaDW.ListaFacetaExcluida)
                {
                    if (!facetasEscluidas.Contains(filaFacetaExcluida.Faceta))
                    {
                        facetasEscluidas.Add(filaFacetaExcluida.Faceta);
                    }
                }

                foreach (FacetaObjetoConocimiento filaFacetaOC in FacetaDW.ListaFacetaObjetoConocimiento)
                {
                    if (!facetasEscluidas.Contains(filaFacetaOC.Faceta))
                    {
                        facetasEscluidas.Add(filaFacetaOC.Faceta);
                        //La excluimos
                        FacetaExcluida filaFacetaExcluida = new FacetaExcluida();
                        filaFacetaExcluida.ProyectoID = ProyectoSeleccionado.Clave;
                        filaFacetaExcluida.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                        filaFacetaExcluida.Faceta = filaFacetaOC.Faceta;
                        FacetaDW.ListaFacetaExcluida.Add(filaFacetaExcluida);
                        mEntityContext.FacetaExcluida.Add(filaFacetaExcluida);
                    }
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
                                //throw new Exception("En un nodo 'Reciproca' no puede ir un número");
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(faceta.ClaveFaceta))
                                {
                                    //faceta.ClaveFaceta = reciprocaString;
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

                        foreach (string objetoConocimiento in ListaOntologias.Keys)
                        {
                            if (faceta.ObjetosConocimiento.Contains(objetoConocimiento))
                            {
                                string columnaFaceta = faceta.ClaveFaceta;
                                if (string.IsNullOrEmpty(columnaFaceta))
                                {
                                    columnaFaceta = faceta.Reciprocidad;
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(faceta.Reciprocidad))
                                    {
                                        columnaFaceta = $"{faceta.Reciprocidad}@@@{columnaFaceta}";
                                    }
                                }

                                FacetaObjetoConocimientoProyecto filaFaceta = null;
                                if (faceta.AgrupacionID.HasValue)
                                {
                                    filaFaceta = FacetaDW.ListaFacetaObjetoConocimientoProyecto.FirstOrDefault(item => item.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.ObjetoConocimiento.ToLower().Equals(objetoConocimiento.ToLower()) && item.Faceta.Equals(columnaFaceta) && item.AgrupacionID.HasValue && item.AgrupacionID.Value.Equals(faceta.AgrupacionID));
                                    if(filaFaceta == null)
                                    {
                                        filaFaceta = FacetaDW.ListaFacetaObjetoConocimientoProyecto.FirstOrDefault(item => item.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.ObjetoConocimiento.ToLower().Equals(objetoConocimiento.ToLower()) && item.Faceta.Equals(columnaFaceta));
                                    }
                                }
                                else
                                {
                                    filaFaceta = FacetaDW.ListaFacetaObjetoConocimientoProyecto.FirstOrDefault(item => item.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.ObjetoConocimiento.ToLower().Equals(objetoConocimiento.ToLower()) && item.Faceta.Equals(columnaFaceta));
                                }
                                if (filaFaceta == null)
                                {
                                    KeyValuePair<string, string> claveFacetaOC = new KeyValuePair<string, string>(columnaFaceta, objetoConocimiento);
                                    listaFacetasNuevas.Add(claveFacetaOC);

                                    AgregarFilaFacetaNueva(faceta, objetoConocimiento, true);
                                }
                            }
                        }
                    }
                }

                //Modificar las que tienen cambios
                foreach (FacetaModel faceta in pListaFacetas)
                {
                    if (!faceta.Deleted)
                    {
                        foreach (string objetoConocimiento in ListaOntologias.Keys)
                        {
                            if (faceta.ObjetosConocimiento.Contains(objetoConocimiento))
                            {
                                string columnaFaceta = faceta.ClaveFaceta;
                                if (string.IsNullOrEmpty(columnaFaceta))
                                {
                                    columnaFaceta = faceta.Reciprocidad;
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(faceta.Reciprocidad))
                                    {
                                        columnaFaceta = $"{faceta.Reciprocidad}@@@{columnaFaceta}";
                                    }
                                }

                                KeyValuePair<string, string> claveFacetaOC = new KeyValuePair<string, string>(columnaFaceta, objetoConocimiento);

                                if (!listaFacetasNuevas.Contains(claveFacetaOC))
                                {
                                    FacetaObjetoConocimientoProyecto filaFaceta = null;
                                    if (faceta.AgrupacionID.HasValue)
                                    {
                                        filaFaceta = FacetaDW.ListaFacetaObjetoConocimientoProyecto.FirstOrDefault(item => item.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.ObjetoConocimiento.ToLower().Equals(objetoConocimiento.ToLower()) && item.Faceta.Equals(columnaFaceta) && item.AgrupacionID.HasValue && item.AgrupacionID.Value.Equals(faceta.AgrupacionID));
                                        if (filaFaceta == null)
                                        {
                                            filaFaceta = FacetaDW.ListaFacetaObjetoConocimientoProyecto.FirstOrDefault(item => item.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.ObjetoConocimiento.ToLower().Equals(objetoConocimiento.ToLower()) && item.Faceta.Equals(columnaFaceta));
                                        }
                                    }
                                    else
                                    {
                                        filaFaceta = FacetaDW.ListaFacetaObjetoConocimientoProyecto.FirstOrDefault(item => item.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.ObjetoConocimiento.ToLower().Equals(objetoConocimiento) && item.Faceta.Equals(columnaFaceta));
                                    }
                                    if (filaFaceta != null)
                                    {
                                        ModificarFilaFaceta(filaFaceta, faceta, true);
                                    }
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

                            FacetaObjetoConocimientoProyecto filaFaceta = null;
                            if (faceta.AgrupacionID.HasValue)
                            {
                                filaFaceta = FacetaDW.ListaFacetaObjetoConocimientoProyecto.FirstOrDefault(item => item.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.ObjetoConocimiento.ToLower().Equals(objetoConocimiento) && item.Faceta.Equals(claveFaceta) && item.AgrupacionID.Value.Equals(faceta.AgrupacionID));
                            }
                            else
                            {
                                filaFaceta = FacetaDW.ListaFacetaObjetoConocimientoProyecto.FirstOrDefault(item => item.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.ObjetoConocimiento.ToLower().Equals(objetoConocimiento) && item.Faceta.Equals(claveFaceta));
                            }

                            if (filaFaceta != null)
                            {
                                //Si estamos eliminando una faceta, pero hemos creado otra igual, no la borramos
                                //Por ejemplo, 
                                if (pListaFacetas.Find(fac => fac.ClaveFaceta == filaFaceta.Faceta && fac.Deleted == false && fac.ObjetosConocimiento.Contains(objetoConocimiento)) == null)
                                {
                                    if (filaFaceta.FacetaObjetoConocimientoProyectoPestanya != null && filaFaceta.FacetaObjetoConocimientoProyectoPestanya.Count() > 0)
                                    {
                                        foreach (FacetaObjetoConocimientoProyectoPestanya facetaObjetoConocimientoPestanya in filaFaceta.FacetaObjetoConocimientoProyectoPestanya.ToList())
                                        {
                                            mEntityContext.EliminarElemento(facetaObjetoConocimientoPestanya);
                                            FacetaDW.ListaFacetaObjetoConocimientoProyectoPenstanya.Remove(facetaObjetoConocimientoPestanya);
                                        }
                                    }
                                    EliminarFaceta(filaFaceta);
                                }
                            }
                        }
                    }
                }

                //Eliminar las que no se encuentran (se ha cambiado la clave de la faceta)
                foreach (FacetaObjetoConocimientoProyecto filaFaceta in FacetaDW.ListaFacetaObjetoConocimientoProyecto)
                {
                    if (mEntityContext.Entry(filaFaceta).State == EntityState.Deleted && pListaFacetas.Where(faceta => faceta.ClaveFaceta == filaFaceta.Faceta).Count() == 0)
                    {
                        if (filaFaceta.FacetaObjetoConocimientoProyectoPestanya != null && filaFaceta.FacetaObjetoConocimientoProyectoPestanya.Count() > 0)
                        {
                            foreach (FacetaObjetoConocimientoProyectoPestanya facetaObjetoConocimientoPestanya in filaFaceta.FacetaObjetoConocimientoProyectoPestanya.ToList())
                            {
                                mEntityContext.EliminarElemento(facetaObjetoConocimientoPestanya);
                                FacetaDW.ListaFacetaObjetoConocimientoProyectoPenstanya.Remove(facetaObjetoConocimientoPestanya);
                            }
                        }
                        EliminarFaceta(filaFaceta);
                    }
                }

                FacetaCN facetaCN = new FacetaCN(mEntityContext, mLoggingService, mConfigService, null);
                facetaCN.Actualizar();
                facetaCN.Dispose();
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
                throw new Exception(ex.Message, ex);
            }
        }


        private void EliminarFaceta(FacetaObjetoConocimientoProyecto pFilaFaceta)
        {
            FacetaCN facetaCN = new FacetaCN(mEntityContext, mLoggingService, mConfigService, null);
            List<FacetaFiltroProyecto> filasFiltros = pFilaFaceta.FacetaFiltroProyecto.ToList();

            foreach (FacetaFiltroProyecto filaFiltro in filasFiltros)
            {
                pFilaFaceta.FacetaFiltroProyecto.Remove(filaFiltro);
                mEntityContext.EliminarElemento(filaFiltro);
                FacetaDW.ListaFacetaFiltroProyecto.Remove(filaFiltro);
            }
                        
            FacetaDW.ListaFacetaObjetoConocimientoProyecto.Remove(pFilaFaceta);
            mEntityContext.EliminarElemento(pFilaFaceta);
        }

        private void AgregarFilaFacetaNueva(FacetaModel pFaceta, string pObjetoConocimiento, bool pMantenerOrden = false)
        {
            FacetaObjetoConocimientoProyecto filaFacetaNueva = new FacetaObjetoConocimientoProyecto();
            try
            {
                filaFacetaNueva.ProyectoID = ProyectoSeleccionado.Clave;
                filaFacetaNueva.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                filaFacetaNueva.ObjetoConocimiento = pObjetoConocimiento;

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
            }
            catch(Exception ex)
            {
                mLoggingService.GuardarLogError("");
                string mensaje = "Ha ocurrido un error al intentar guardar las facetas.";
                if(FacetaDW.ListaFacetaObjetoConocimientoProyecto.Any(item => item.OrganizacionID.Equals(filaFacetaNueva.OrganizacionID) && item.ProyectoID.Equals(filaFacetaNueva.ProyectoID) && item.Faceta.Equals(filaFacetaNueva.Faceta) && item.ObjetoConocimiento.Equals(filaFacetaNueva.ObjetoConocimiento)))
                {
                    mensaje = $"Ya existe una faceta con el mismo objeto de conocimiento ({filaFacetaNueva.ObjetoConocimiento}) y la misma faceta ({filaFacetaNueva.Faceta})";
                }
                throw new Exception(mensaje, ex);
            }           
        }


        private void ModificarFilaFaceta(FacetaObjetoConocimientoProyecto pFilaFaceta, FacetaModel pFaceta, bool pMantenerOrden = false)
        {
            pFilaFaceta.NombreFaceta = pFaceta.Name;

            short reciproca = 0;

            string reciprocaString = pFaceta.Reciprocidad;
            if (!string.IsNullOrEmpty(reciprocaString))
            {
                if (short.TryParse(reciprocaString, out reciproca))
                {
                    //throw new Exception("En un nodo 'Reciproca' no puede ir un número");
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

            pFilaFaceta.Mayusculas = pFaceta.Presentacion;
            pFilaFaceta.TipoDisenio = pFaceta.Disenyo;
            pFilaFaceta.AlgoritmoTransformacion = pFaceta.AlgoritmoTransformacion;
            pFilaFaceta.ElementosVisibles = pFaceta.NumElementosVisibles;

            pFilaFaceta.EsSemantica = true;//pFaceta.EsSemantica;
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
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, null);

                pFaceta.PrivacidadGrupos = identidadCN.ObtenerNombresDeGrupos(pFaceta.PrivacidadGrupos.Keys.ToList());

                List<string> listaNombresCortos = identidadCN.ObtenerNombresCortosGruposPorID(pFaceta.PrivacidadGrupos.Keys.ToList());
                foreach (string nombreCortoGrupo in listaNombresCortos)
                {
                    facetaPrivadaParaGrupoEditores += nombreCortoGrupo + "|";
                }
            }
            pFilaFaceta.FacetaPrivadaParaGrupoEditores = facetaPrivadaParaGrupoEditores.TrimEnd('|');

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
                pFilaFaceta.Orden = (short)(pFaceta.Orden);
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

            //Metodo que cogia los valores de los filtros desde el textBox
            /*
            if (pFaceta.Filtros != null)
            {
                foreach (string filtroFaceta in pFaceta.Filtros.Split(','))
                {
                    string filtro = filtroFaceta.Trim();

                    if (!string.IsNullOrEmpty(filtro))
                    {
                        FacetaDS.FacetaFiltroProyectoRow filaFiltro = FacetaDS.FacetaFiltroProyecto.NewFacetaFiltroProyectoRow();
                        filaFiltro.OrganizacionID = pFilaFaceta.OrganizacionID;
                        filaFiltro.ProyectoID = pFilaFaceta.ProyectoID;
                        filaFiltro.ObjetoConocimiento = pFilaFaceta.ObjetoConocimiento;
                        filaFiltro.Faceta = pFilaFaceta.Faceta;
                        filaFiltro.Filtro = filtro;
                        filaFiltro.Orden = (short)(pFilaFaceta.Orden + contFiltros);
                        FacetaDS.FacetaFiltroProyecto.AddFacetaFiltroProyectoRow(filaFiltro);

                        contFiltros++;
                    }
                }
            }
            */





            //if (filasFiltros.Length == 0 && !string.IsNullOrEmpty(pFaceta.Filtros))
            //{
            //    foreach (string filtro in pFaceta.Filtros.Split(','))
            //    {
            //        if (!string.IsNullOrEmpty(filtro))
            //        {
            //            //Agregamos fila para el objeto de conocimiento actual
            //            FacetaDS.FacetaFiltroProyectoRow filaFiltro = FacetaDS.FacetaFiltroProyecto.NewFacetaFiltroProyectoRow();
            //            filaFiltro.OrganizacionID = pFilaFaceta.OrganizacionID;
            //            filaFiltro.ProyectoID = pFilaFaceta.ProyectoID;
            //            filaFiltro.ObjetoConocimiento = pFilaFaceta.ObjetoConocimiento;
            //            filaFiltro.Faceta = pFilaFaceta.Faceta;
            //            filaFiltro.Filtro = filtro;
            //            filaFiltro.Orden = pFilaFaceta.Orden;
            //            FacetaDS.FacetaFiltroProyecto.AddFacetaFiltroProyectoRow(filaFiltro);
            //        }
            //    }
            //}
            //else if (filasFiltros.Length > 0 && !string.IsNullOrEmpty(pFaceta.Filtros))
            //{
            //    filasFiltros[0].Filtro = pFaceta.Filtros;
            //    filasFiltros[0].Orden = pFilaFaceta.Orden;
            //}
            //else if(filasFiltros.Length > 0)
            //{
            //    filasFiltros[0].Delete();
            //}
        }

        public void CrearFilasPropiedadesIntegracionContinua(List<FacetaModel> pListaFacetas)
        {
            List<IntegracionContinuaPropiedad> propiedadesIntegracionContinua = new List<IntegracionContinuaPropiedad>();

            try
            {
                foreach (FacetaModel faceta in pListaFacetas)
                {
                    if (!faceta.Deleted)
                    {
                        if (faceta.Type.Equals(TipoFaceta.Tesauro) && !string.IsNullOrEmpty(faceta.Filtros))
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
                            propiedadesIntegracionContinua.Add(propiedadRutaPagina);
                            //faceta.Filtros = UtilIntegracionContinua.ObtenerMascaraPropiedad(propiedadRutaPagina);
                        }
                    }
                }

                using (ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, null))
                {
                    proyCN.CrearFilasIntegracionContinuaParametro(propiedadesIntegracionContinua, ProyectoSeleccionado.Clave, TipoObjeto.Faceta);
                }
            }
            catch
            {

            }
        }

        public void ModificarFilasIntegracionContinuaEntornoSiguiente(List<FacetaModel> pListaFacetas, string UrlApiDesplieguesEntornoSiguiente, Guid pUsuarioID)
        {
            List<IntegracionContinuaPropiedad> propiedadesIntegracionContinua = new List<IntegracionContinuaPropiedad>();
            try
            {
                foreach (FacetaModel faceta in pListaFacetas)
                {
                    if (!faceta.Deleted)
                    {
                        if (faceta.Type.Equals(TipoFaceta.Tesauro) && !string.IsNullOrEmpty(faceta.Filtros))
                        {
                            //Crear las filas de las porpiedades de Integracion Continua
                            IntegracionContinuaPropiedad propiedadRutaPagina = new IntegracionContinuaPropiedad();
                            propiedadRutaPagina.ProyectoID = ProyectoSeleccionado.Clave;
                            propiedadRutaPagina.TipoObjeto = (short)TipoObjeto.Faceta;
                            propiedadRutaPagina.ObjetoPropiedad = faceta.ClaveFaceta;
                            propiedadRutaPagina.TipoPropiedad = (short)TipoPropiedad.FiltrosFaceta;
                            propiedadRutaPagina.ValorPropiedad = faceta.Filtros;
                            propiedadesIntegracionContinua.Add(propiedadRutaPagina);
                            //faceta.Filtros = UtilIntegracionContinua.ObtenerMascaraPropiedad(propiedadRutaPagina);
                        }
                    }
                }
            }
            catch
            {

            }
            try
            {
                string peticion = $"{UrlApiDesplieguesEntornoSiguiente}/PropiedadesIntegracion?nombreProy={ProyectoSeleccionado.NombreCorto}&UsuarioID={pUsuarioID}";
                string requestParameters = UtilWeb.WebRequestPostWithJsonObject(peticion, propiedadesIntegracionContinua, "");
            }
            catch
            {
            }
        }


        public void GuardarFaceta(FacetaModel pFaceta, bool pMantenerOrden = false)
        {
            List<FacetaObjetoConocimientoProyecto> listaFacetas = FacetaDW.ListaFacetaObjetoConocimientoProyecto.ToList();
            try
            {
                List<KeyValuePair<string, string>> listaFacetasNuevas = new List<KeyValuePair<string, string>>();

                List<string> facetasEscluidas = new List<string>();

                //foreach (FacetaExcluida filaFacetaExcluida in FacetaDW.ListaFacetaExcluida)
                //{
                //    if (!facetasEscluidas.Contains(filaFacetaExcluida.Faceta))
                //    {
                //        facetasEscluidas.Add(filaFacetaExcluida.Faceta);
                //    }
                //}

                //foreach (FacetaObjetoConocimiento filaFacetaOC in FacetaDW.ListaFacetaObjetoConocimiento)
                //{
                //    if (!facetasEscluidas.Contains(filaFacetaOC.Faceta))
                //    {
                //        facetasEscluidas.Add(filaFacetaOC.Faceta);
                //        //La excluimos
                //        FacetaExcluida filaFacetaExcluida = new FacetaExcluida();
                //        filaFacetaExcluida.ProyectoID = ProyectoSeleccionado.Clave;
                //        filaFacetaExcluida.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                //        filaFacetaExcluida.Faceta = filaFacetaOC.Faceta;
                //        FacetaDW.ListaFacetaExcluida.Add(filaFacetaExcluida);
                //        EntityContext.Instance.FacetaExcluida.Add(filaFacetaExcluida);
                //    }
                //}

                //Añadir las nuevas

                if (!pFaceta.Deleted)
                {
                    short reciproca = 0;

                    string reciprocaString = pFaceta.Reciprocidad;
                    if (!string.IsNullOrEmpty(reciprocaString))
                    {
                        if (short.TryParse(reciprocaString, out reciproca))
                        {
                            //throw new Exception("En un nodo 'Reciproca' no puede ir un número");
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

                    foreach (string objetoConocimiento in ListaOntologias.Keys)
                    {
                        if (pFaceta.ObjetosConocimiento.Contains(objetoConocimiento))
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
                }


                //Modificar las que tienen cambios

                if (!pFaceta.Deleted)
                {
                    foreach (string objetoConocimiento in ListaOntologias.Keys)
                    {
                        if (pFaceta.ObjetosConocimiento.Contains(objetoConocimiento))
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


                FacetaCN facetaCN = new FacetaCN(mEntityContext, mLoggingService, mConfigService, null);
                facetaCN.Actualizar();
                facetaCN.Dispose();
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
            }
        }

        #endregion

        #region Gestion de Errores
        public string ComprobarErrores(List<FacetaModel> pListaFacetas)
        {
            string error = string.Empty;
            //Todo
            error = ComprobarErrorInmutable(pListaFacetas);

            return error;
        }
        private string ComprobarErrorInmutable(List<FacetaModel> pListaFacetas)
        {
            string error = "";
            foreach (FacetaModel faceta in pListaFacetas)
            {
                if (faceta.Inmutable && !faceta.ComportamientoOr)
                {
                    error = "Si se elige una faceta inmutable, el comportamientoOR tiene que estar marcado también";
                }
            }
            return error;
        }
        #endregion

        #region Invalidar Caches

        public void InvalidarCaches(string UrlIntragnoss)
        {
            FacetaCL facetaCL = new FacetaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, null);
            bool cachearFacetas = !(ParametroProyecto.ContainsKey("CacheFacetas") && ParametroProyecto["CacheFacetas"].Equals("0"));
            facetaCL.InvalidarCacheFacetasProyecto(ProyectoSeleccionado.Clave, cachearFacetas);

            FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, null);
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
                    FacetaCN facetaCN = new FacetaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mFacetaDW = facetaCN.ObtenerFacetasAdministrarProyecto(ProyectoSeleccionado.Clave);
                    facetaCN.Dispose();
                }
                return mFacetaDW;
            }
        }

        #endregion

    }
}
