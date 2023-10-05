using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.CMS;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.VistaVirtualDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.CMS;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.Elementos.CMS;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Logica.CMS;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.Web.Controles.ServicioImagenesWrapper;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Es.Riam.Gnoss.Web.Controles.Administracion
{
    public class ControladorComponenteCMS : ControladorBase
    {
        private Elementos.ServiciosGenerales.Proyecto ProyectoSeleccionado = null;
        private Dictionary<string, string> ParametroProyecto = null;

        private CMSComponente CMSComponente = null;
        private TipoComponenteCMS TipoComponenteCMSActual;

        public bool ActualizarEnBD = true;

        private bool CrearFilasPropiedadesExportacion = false;

        private List<IntegracionContinuaPropiedad> propiedadesIntegracionContinua = null;

        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private EntityContextBASE mEntityContextBASE;
        private GnossCache mGnossCache;

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public ControladorComponenteCMS(Elementos.ServiciosGenerales.Proyecto pProyecto, Dictionary<string, string> pParametroProyecto, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, EntityContextBASE entityContextBASE, VirtuosoAD virtuosoAD, GnossCache gnossCache, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, bool pCrearFilasPropiedadesExportacion = false)
        : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mEntityContextBASE = entityContextBASE;
            mGnossCache = gnossCache;

            ProyectoSeleccionado = pProyecto;
            ParametroProyecto = pParametroProyecto;

            CrearFilasPropiedadesExportacion = pCrearFilasPropiedadesExportacion;
        }

        #endregion

        #region Métodos de carga
        public CMSAdminComponenteEditarViewModel CargarComponente(Guid pComponenteKey)
        {
            CMSAdminComponenteEditarViewModel resultado = null;
            using (CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication))
            using (GestionCMS gestorCMS = new GestionCMS(cmsCN.ObtenerComponentePorID(pComponenteKey, ProyectoSeleccionado.Clave), mLoggingService, mEntityContext))
            {
                if (gestorCMS.CMSDW.ListaCMSComponente.Count != 0)
                {
                    TipoComponenteCMS tipoComponente = (TipoComponenteCMS)gestorCMS.CMSDW.ListaCMSComponente.FirstOrDefault().TipoComponente;
                    resultado = CargarComponente(tipoComponente, gestorCMS.ListaComponentes[pComponenteKey]);
                }
            }
            return resultado;
        }
        public CMSAdminComponenteEditarViewModel CargarComponente(TipoComponenteCMS pTipoComponenteCMSActual, CMSComponente pCMSComponente)
        {
            Guid? PersonalizacionID = null;

            CMSComponente = pCMSComponente;
            TipoComponenteCMSActual = pTipoComponenteCMSActual;


            CMSAdminComponenteEditarViewModel paginaModel = new CMSAdminComponenteEditarViewModel();

            paginaModel.Type = TipoComponenteCMSActual;

            paginaModel.Private = false;
            paginaModel.Active = true;
            paginaModel.AccesoPublicoComponente = false;
            if (CMSComponente != null)
            {
                PersonalizacionID = pCMSComponente.Personalizacion;

                paginaModel.Private = CMSComponente.Privado;
                if (CMSComponente.Privado)
                {
                    List<Guid> listaGrupos = new List<Guid>();
                    List<Guid> listaPerfiles = new List<Guid>();

                    foreach (CMSComponenteRolGrupoIdentidades grupo in CMSComponente.ListaRolGrupoIdentidades.Values)
                    {
                        listaGrupos.Add(grupo.GrupoID);
                    }
                    foreach (CMSComponenteRolIdentidad identidad in CMSComponente.ListaRolIdentidad.Values)
                    {
                        listaPerfiles.Add(identidad.PerfilID);
                    }

                    IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    paginaModel.GruposPrivacidad = identidadCN.ObtenerNombresDeGrupos(listaGrupos);
                    paginaModel.PerfilesPrivacidad = identidadCN.ObtenerNombresDePerfiles(listaPerfiles);
                    identidadCN.Dispose();
                }
                paginaModel.Name = CMSComponente.Nombre;
                paginaModel.ShortName = CMSComponente.NombreCortoComponente;
                paginaModel.Active = CMSComponente.Activo;
                paginaModel.AccesoPublicoComponente = CMSComponente.AccesoPublico;
                paginaModel.Styles = CMSComponente.Estilos;

                List<string> listaIdiomasDisponibles = new List<string>();
                List<string> listaIdiomas = mConfigService.ObtenerListaIdiomas();
                foreach (string idioma in listaIdiomas)
                {
                    if (!string.IsNullOrEmpty(CMSComponente.FilaComponente.IdiomasDisponibles) && UtilCadenas.ObtenerTextoDeIdioma(CMSComponente.FilaComponente.IdiomasDisponibles, idioma, null, true) == "true")
                    {
                        listaIdiomasDisponibles.Add(idioma);
                    }
                }
                paginaModel.ListaIdiomasDisponibles = listaIdiomasDisponibles;

            }

            if (PersonalizacionID.HasValue)
            {
                paginaModel.PersonalizacionSeleccionada = PersonalizacionID.Value;
            }

            paginaModel.Caducidades = new Dictionary<TipoCaducidadComponenteCMS, bool>();

            if (UtilComponentes.CaducidadesDisponiblesPorTipoComponente[TipoComponenteCMSActual].Count > 1)
            {
                foreach (TipoCaducidadComponenteCMS caducidad in UtilComponentes.CaducidadesDisponiblesPorTipoComponente[TipoComponenteCMSActual])
                {
                    bool selected = false;

                    if (CMSComponente != null && CMSComponente.TipoCaducidadComponenteCMS == caducidad)
                    {
                        selected = true;
                        paginaModel.CaducidadSeleccionada = caducidad;
                    }

                    paginaModel.Caducidades.Add(caducidad, selected);
                }
            }
            else if (UtilComponentes.CaducidadesDisponiblesPorTipoComponente[TipoComponenteCMSActual].Count == 1)
            {
                foreach (TipoCaducidadComponenteCMS caducidad in UtilComponentes.CaducidadesDisponiblesPorTipoComponente[TipoComponenteCMSActual])
                {
                    if (CMSComponente != null && CMSComponente.TipoCaducidadComponenteCMS == caducidad)
                    {
                        paginaModel.CaducidadSeleccionada = caducidad;
                    }
                }
            }

            paginaModel.Properties = new List<CMSAdminComponenteEditarViewModel.PropiedadComponente>();
            propiedadesIntegracionContinua = new List<IntegracionContinuaPropiedad>();

            Dictionary<TipoPropiedadCMS, bool> propiedadesComponente = UtilComponentes.PropiedadesDisponiblesPorTipoComponente[TipoComponenteCMSActual];
            foreach (TipoPropiedadCMS tipoPropiedad in propiedadesComponente.Keys)
            {
                CMSAdminComponenteEditarViewModel.PropiedadComponente propiedad = ObtenerPropiedad(tipoPropiedad, propiedadesComponente);

                paginaModel.Properties.Add(propiedad);

                if (CrearFilasPropiedadesExportacion)
                {
                    propiedadesIntegracionContinua = ObtenerPropiedadesIntegracionContinuaComponente(paginaModel, propiedadesIntegracionContinua, propiedad, true);
                }
            }

            if (CrearFilasPropiedadesExportacion)
            {
                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                proyCN.CrearFilasIntegracionContinuaParametro(propiedadesIntegracionContinua, ProyectoSeleccionado.Clave, TipoObjeto.Componente, paginaModel.ShortName);
                proyCN.Dispose();
            }

            return paginaModel;
        }

        public void CargarPropiedadIC(CMSAdminComponenteEditarViewModel pComponente)
        {
            foreach (CMSAdminComponenteEditarViewModel.PropiedadComponente propiedad in pComponente.Properties)
            {
                if (propiedad.Options == null)
                {
                    propiedad.Options = new Dictionary<string, string>();
                }
                VistaVirtualCL vistaVirtualCL = new VistaVirtualCL(mEntityContext, mLoggingService, mGnossCache, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                DataWrapperVistaVirtual vistaVirtualDW = vistaVirtualCL.ObtenerVistasVirtualPorProyectoID(ProyectoSeleccionado.Clave, PersonalizacionEcosistemaID, ComunidadExcluidaPersonalizacionEcosistema);
                vistaVirtualCL.Dispose();
                switch (propiedad.TipoPropiedadCMS)
                {
                    case TipoPropiedadCMS.TipoPresentacionRecurso:
                        string vistasRecursos = "/Views/CMSPagina/ListadoRecursos/Vistas/";

                        List<VistaVirtualCMS> filasRecursos = vistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.TipoComponente.StartsWith(vistasRecursos)).ToList();

                        Dictionary<TipoPresentacionRecursoCMS, string> diccionarioNombresGenericos = new Dictionary<TipoPresentacionRecursoCMS, string>();
                        Dictionary<Guid, string> diccionarioNombresPersonalizaciones = new Dictionary<Guid, string>();
                        if (filasRecursos.Count > 0)
                        {
                            foreach (VistaVirtualCMS filaVistaVirtualCMS in filasRecursos)
                            {
                                string nombre = filaVistaVirtualCMS.Nombre;
                                if (string.IsNullOrEmpty(nombre))
                                {
                                    nombre = filaVistaVirtualCMS.PersonalizacionComponenteID.ToString();
                                }
                                bool agregado = false;
                                foreach (TipoPresentacionRecursoCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionRecursoCMS)))
                                {
                                    if (filaVistaVirtualCMS.TipoComponente.EndsWith(tipoPresentacion.ToString() + ".cshtml"))
                                    {
                                        diccionarioNombresGenericos.Add(tipoPresentacion, nombre);
                                        agregado = true;
                                        break;
                                    }
                                }
                                if (!agregado)
                                {
                                    diccionarioNombresPersonalizaciones.Add(filaVistaVirtualCMS.PersonalizacionComponenteID, nombre);
                                }
                            }
                        }

                        foreach (TipoPresentacionRecursoCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionRecursoCMS)))
                        {
                            string nombre = UtilIdiomas.GetText("COMADMINCMS", "PRESENTACION_" + tipoPresentacion);
                            if (diccionarioNombresGenericos.ContainsKey(tipoPresentacion))
                            {
                                nombre = diccionarioNombresGenericos[tipoPresentacion];
                            }
                            propiedad.Options.Add(((short)tipoPresentacion).ToString(), nombre);
                        }

                        foreach (Guid idPersonalizacion in diccionarioNombresPersonalizaciones.Keys)
                        {
                            string nombre = diccionarioNombresPersonalizaciones[idPersonalizacion];
                            propiedad.Options.Add(idPersonalizacion.ToString(), nombre);
                        }

                        break;
                    case TipoPropiedadCMS.TipoPresentacionGrupoComponentes:
                        string vistasGruposComponmentes = "/Views/CMSPagina/GrupoComponentes/";

                        List<VistaVirtualCMS> filasGruposComponmentes = vistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.TipoComponente.StartsWith(vistasGruposComponmentes)).ToList();

                        Dictionary<TipoPresentacionGrupoComponentesCMS, string> diccionarioNombresGenericosVistaGrupos = new Dictionary<TipoPresentacionGrupoComponentesCMS, string>();
                        Dictionary<Guid, string> diccionarioNombresPersonalizacionesVistaGrupos = new Dictionary<Guid, string>();
                        if (filasGruposComponmentes.Count > 0)
                        {
                            foreach (VistaVirtualCMS filaVistaVirtualCMS in filasGruposComponmentes)
                            {
                                string nombre = filaVistaVirtualCMS.Nombre;
                                if (string.IsNullOrEmpty(nombre))
                                {
                                    nombre = filaVistaVirtualCMS.PersonalizacionComponenteID.ToString();
                                }
                                bool agregado = false;
                                foreach (TipoPresentacionGrupoComponentesCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionGrupoComponentesCMS)))
                                {
                                    if (filaVistaVirtualCMS.TipoComponente.EndsWith(tipoPresentacion.ToString() + ".cshtml"))
                                    {
                                        diccionarioNombresGenericosVistaGrupos.Add(tipoPresentacion, nombre);
                                        agregado = true;
                                        break;
                                    }
                                }
                                if (!agregado)
                                {
                                    diccionarioNombresPersonalizacionesVistaGrupos.Add(filaVistaVirtualCMS.PersonalizacionComponenteID, nombre);
                                }
                            }
                        }

                        foreach (TipoPresentacionGrupoComponentesCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionGrupoComponentesCMS)))
                        {
                            string nombre = UtilIdiomas.GetText("COMADMINCMS", "PRESENTACIONGRUPO_" + tipoPresentacion);
                            if (diccionarioNombresGenericosVistaGrupos.ContainsKey(tipoPresentacion))
                            {
                                nombre = diccionarioNombresGenericosVistaGrupos[tipoPresentacion];
                            }
                            propiedad.Options.Add(((short)tipoPresentacion).ToString(), nombre);
                        }

                        foreach (Guid idPersonalizacion in diccionarioNombresPersonalizacionesVistaGrupos.Keys)
                        {
                            string nombre = diccionarioNombresPersonalizacionesVistaGrupos[idPersonalizacion];
                            propiedad.Options.Add(idPersonalizacion.ToString(), nombre);
                        }

                        break;
                    case TipoPropiedadCMS.TipoActividadRecienteCMS:
                        bool homeUsuarioPermitida = false;
                        ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                        ParametroAplicacion busqueda = mEntityContext.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro.Equals("EcosistemaSinHomeUsuario"));
                        if (busqueda == null || busqueda.Valor == "false")
                        {
                            homeUsuarioPermitida = true;
                        }

                        foreach (TipoActividadReciente tipoActividadReciente in Enum.GetValues(typeof(TipoActividadReciente)))
                        {
                            if (tipoActividadReciente.Equals(TipoActividadReciente.HomeUsuario) && !homeUsuarioPermitida)
                            {
                                continue;
                            }
                            propiedad.Options.Add(((short)tipoActividadReciente).ToString(), tipoActividadReciente.ToString());
                        }

                        break;
                    case TipoPropiedadCMS.TipoPresentacionListadoRecursos:
                        string vistasListadoRecursos = "/Views/CMSPagina/ListadoRecursos/";

                        List<VistaVirtualCMS> filasListadoRecursos = vistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.TipoComponente.StartsWith(vistasListadoRecursos)).ToList();

                        Dictionary<TipoPresentacionListadoRecursosCMS, string> diccionarioNombresListadoGenericos = new Dictionary<TipoPresentacionListadoRecursosCMS, string>();
                        Dictionary<Guid, string> diccionarioNombresListadoPersonalizaciones = new Dictionary<Guid, string>();
                        if (vistasListadoRecursos.Length > 0)
                        {
                            foreach (VistaVirtualCMS filaVistaVirtualCMS in filasListadoRecursos)
                            {
                                if (filaVistaVirtualCMS.TipoComponente.Replace(vistasListadoRecursos, "").IndexOf("/") == -1)
                                {
                                    string nombre = filaVistaVirtualCMS.Nombre;
                                    if (string.IsNullOrEmpty(nombre))
                                    {
                                        nombre = filaVistaVirtualCMS.PersonalizacionComponenteID.ToString();
                                    }
                                    bool agregado = false;
                                    foreach (TipoPresentacionListadoRecursosCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionListadoRecursosCMS)))
                                    {
                                        if (filaVistaVirtualCMS.TipoComponente.EndsWith(tipoPresentacion.ToString() + ".cshtml"))
                                        {
                                            diccionarioNombresListadoGenericos.Add(tipoPresentacion, nombre);
                                            agregado = true;
                                            break;
                                        }
                                    }
                                    if (!agregado)
                                    {
                                        diccionarioNombresListadoPersonalizaciones.Add(filaVistaVirtualCMS.PersonalizacionComponenteID, nombre);
                                    }
                                }
                            }
                        }
                        foreach (TipoPresentacionListadoRecursosCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionListadoRecursosCMS)))
                        {
                            string nombre = UtilIdiomas.GetText("COMADMINCMS", "PRESENTACIONLISTADO_" + tipoPresentacion);
                            if (diccionarioNombresListadoGenericos.ContainsKey(tipoPresentacion))
                            {
                                nombre = diccionarioNombresListadoGenericos[tipoPresentacion];
                            }
                            propiedad.Options.Add(((short)tipoPresentacion).ToString(), nombre);
                        }
                        foreach (Guid idPersonalizacion in diccionarioNombresListadoPersonalizaciones.Keys)
                        {
                            string nombre = diccionarioNombresListadoPersonalizaciones[idPersonalizacion];
                            propiedad.Options.Add(idPersonalizacion.ToString(), nombre);
                        }
                        break;
                    case TipoPropiedadCMS.TipoPresentacionListadoUsuarios:
                        foreach (TipoPresentacionListadoUsuariosCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionListadoUsuariosCMS)))
                        {
                            string nombre = UtilIdiomas.GetText("COMADMINCMS", "PRESENTACIONLISTADOUSUARIOS_" + tipoPresentacion);
                            propiedad.Options.Add(((short)tipoPresentacion).ToString(), nombre);
                        }
                        break;
                    case TipoPropiedadCMS.TipoListadoUsuarios:
                        foreach (TipoListadoUsuariosCMS tipoListado in Enum.GetValues(typeof(TipoListadoUsuariosCMS)))
                        {
                            string nombre = UtilIdiomas.GetText("COMADMINCMS", "LISTADOUSUARIOS_" + tipoListado);
                            propiedad.Options.Add(((short)tipoListado).ToString(), nombre);
                        }
                        break;
                    case TipoPropiedadCMS.TipoPresentacionFaceta:
                        foreach (TipoPresentacionFacetas tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionFacetas)))
                        {
                            string nombre = UtilIdiomas.GetText("COMADMINCMS", "PRESENTACIONFACETAS_" + tipoPresentacion);
                            propiedad.Options.Add(((short)tipoPresentacion).ToString(), nombre);
                        }
                        break;
                    case TipoPropiedadCMS.TipoListadoProyectos:
                        foreach (TipoListadoProyectosCMS tipoListadoProyectosCMS in Enum.GetValues(typeof(TipoListadoProyectosCMS)))
                        {
                            string nombre = UtilIdiomas.GetText("COMADMINCMS", propiedad.TipoPropiedadCMS.ToString() + "_" + tipoListadoProyectosCMS.ToString());
                            propiedad.Options.Add(((short)tipoListadoProyectosCMS).ToString(), nombre);
                        }
                        break;
                }

                if (propiedad.TipoPropiedadCMS.Equals(TipoPropiedadCMS.ListaIDs) && TipoComponenteCMSActual == TipoComponenteCMS.ListadoProyectos)
                {
                    string[] listaIDs = CMSComponente.PropiedadesComponente[propiedad.TipoPropiedadCMS].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    List<Guid> listaProyectos = new List<Guid>();
                    foreach (string elemento in listaIDs)
                    {
                        listaProyectos.Add(new Guid(elemento));
                    }

                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    Dictionary<Guid, string> listaNombresCortos = proyCN.ObtenerNombresCortosProyectos(listaProyectos);
                    proyCN.Dispose();
                    //Recorremos esta lista para que no nos cambie el orden de los proyectos.
                    foreach (Guid proyectoID in listaProyectos)
                    {
                        propiedad.Value += $"{listaNombresCortos[proyectoID]},";
                    }
                }
            }
        }

        private CMSAdminComponenteEditarViewModel.PropiedadComponente ObtenerPropiedad(TipoPropiedadCMS tipoPropiedad, Dictionary<TipoPropiedadCMS, bool> propiedadesComponente)
        {
            CMSAdminComponenteEditarViewModel.PropiedadComponente propiedad = new CMSAdminComponenteEditarViewModel.PropiedadComponente();
            propiedad.TipoPropiedadCMS = tipoPropiedad;
            propiedad.Required = propiedadesComponente[tipoPropiedad];
            propiedad.Options = new Dictionary<string, string>();

            VistaVirtualCL vistaVirtualCL = new VistaVirtualCL(mEntityContext, mLoggingService, mGnossCache, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperVistaVirtual vistaVirtualDW = vistaVirtualCL.ObtenerVistasVirtualPorProyectoID(ProyectoSeleccionado.Clave, PersonalizacionEcosistemaID, ComunidadExcluidaPersonalizacionEcosistema);
            vistaVirtualCL.Dispose();
            switch (propiedad.TipoPropiedadCMS)
            {
                case TipoPropiedadCMS.TipoPresentacionRecurso:
                    string vistasRecursos = "/Views/CMSPagina/ListadoRecursos/Vistas/";
                    List<VistaVirtualCMS> filasRecursos = vistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.TipoComponente.StartsWith(vistasRecursos)).ToList();

                    Dictionary<TipoPresentacionRecursoCMS, string> diccionarioNombresGenericos = new Dictionary<TipoPresentacionRecursoCMS, string>();
                    Dictionary<Guid, string> diccionarioNombresPersonalizaciones = new Dictionary<Guid, string>();
                    if (filasRecursos.Count > 0)
                    {
                        foreach (VistaVirtualCMS filaVistaVirtualCMS in filasRecursos)
                        {
                            string nombre = filaVistaVirtualCMS.Nombre;
                            if (string.IsNullOrEmpty(nombre))
                            {
                                nombre = filaVistaVirtualCMS.PersonalizacionComponenteID.ToString();
                            }
                            bool agregado = false;
                            foreach (TipoPresentacionRecursoCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionRecursoCMS)))
                            {
                                if (filaVistaVirtualCMS.TipoComponente.EndsWith(tipoPresentacion.ToString() + ".cshtml") && !diccionarioNombresGenericos.ContainsKey(tipoPresentacion))
                                {
                                    diccionarioNombresGenericos.Add(tipoPresentacion, nombre);
                                    agregado = true;
                                    break;
                                }
                            }
                            if (!agregado)
                            {
                                diccionarioNombresPersonalizaciones.Add(filaVistaVirtualCMS.PersonalizacionComponenteID, nombre);
                            }
                        }
                    }

                    foreach (TipoPresentacionRecursoCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionRecursoCMS)))
                    {
                        string nombre = UtilIdiomas.GetText("COMADMINCMS", "PRESENTACION_" + tipoPresentacion);
                        if (diccionarioNombresGenericos.ContainsKey(tipoPresentacion))
                        {
                            nombre = diccionarioNombresGenericos[tipoPresentacion];
                        }
                        propiedad.Options.Add(((short)tipoPresentacion).ToString(), nombre);
                    }

                    foreach (Guid idPersonalizacion in diccionarioNombresPersonalizaciones.Keys)
                    {
                        string nombre = diccionarioNombresPersonalizaciones[idPersonalizacion];
                        propiedad.Options.Add(idPersonalizacion.ToString(), nombre);
                    }

                    break;
                case TipoPropiedadCMS.TipoPresentacionGrupoComponentes:
                    string vistasGruposComponmentes = "/Views/CMSPagina/GrupoComponentes/";

                    List<VistaVirtualCMS> filasGruposComponmentes = vistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.TipoComponente.StartsWith(vistasGruposComponmentes)).ToList();

                    Dictionary<TipoPresentacionGrupoComponentesCMS, string> diccionarioNombresGenericosVistaGrupos = new Dictionary<TipoPresentacionGrupoComponentesCMS, string>();
                    Dictionary<Guid, string> diccionarioNombresPersonalizacionesVistaGrupos = new Dictionary<Guid, string>();
                    if (filasGruposComponmentes.Count > 0)
                    {
                        foreach (VistaVirtualCMS filaVistaVirtualCMS in filasGruposComponmentes)
                        {
                            string nombre = filaVistaVirtualCMS.Nombre;
                            if (string.IsNullOrEmpty(nombre))
                            {
                                nombre = filaVistaVirtualCMS.PersonalizacionComponenteID.ToString();
                            }
                            bool agregado = false;
                            foreach (TipoPresentacionGrupoComponentesCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionGrupoComponentesCMS)))
                            {
                                if (filaVistaVirtualCMS.TipoComponente.EndsWith(tipoPresentacion.ToString() + ".cshtml") && !diccionarioNombresGenericosVistaGrupos.ContainsKey(tipoPresentacion))
                                {
                                    diccionarioNombresGenericosVistaGrupos.Add(tipoPresentacion, nombre);
                                    agregado = true;
                                    break;
                                }
                            }
                            if (!agregado)
                            {
                                diccionarioNombresPersonalizacionesVistaGrupos.TryAdd(filaVistaVirtualCMS.PersonalizacionComponenteID, nombre);
                            }
                        }
                    }

                    foreach (TipoPresentacionGrupoComponentesCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionGrupoComponentesCMS)))
                    {
                        string nombre = UtilIdiomas.GetText("COMADMINCMS", "PRESENTACIONGRUPO_" + tipoPresentacion);
                        if (diccionarioNombresGenericosVistaGrupos.ContainsKey(tipoPresentacion))
                        {
                            nombre = diccionarioNombresGenericosVistaGrupos[tipoPresentacion];
                        }
                        propiedad.Options.Add(((short)tipoPresentacion).ToString(), nombre);
                    }

                    foreach (Guid idPersonalizacion in diccionarioNombresPersonalizacionesVistaGrupos.Keys)
                    {
                        string nombre = diccionarioNombresPersonalizacionesVistaGrupos[idPersonalizacion];
                        propiedad.Options.Add(idPersonalizacion.ToString(), nombre);
                    }

                    break;
                case TipoPropiedadCMS.TipoActividadRecienteCMS:
                    bool homeUsuarioPermitida = false;
                    ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    ParametroAplicacion busqueda = mEntityContext.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro.Equals("EcosistemaSinHomeUsuario"));
                    if (busqueda == null || busqueda.Valor == "false")
                    {
                        homeUsuarioPermitida = true;
                    }

                    foreach (TipoActividadReciente tipoActividadReciente in Enum.GetValues(typeof(TipoActividadReciente)))
                    {
                        if (tipoActividadReciente.Equals(TipoActividadReciente.HomeUsuario) && !homeUsuarioPermitida)
                        {
                            continue;
                        }
                        propiedad.Options.Add(((short)tipoActividadReciente).ToString(), tipoActividadReciente.ToString());
                    }

                    break;
                case TipoPropiedadCMS.TipoPresentacionListadoRecursos:
                    string vistasListadoRecursos = "/Views/CMSPagina/ListadoRecursos/";

                    List<VistaVirtualCMS> filasListadoRecursos = vistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.TipoComponente.StartsWith(vistasListadoRecursos)).ToList();

                    Dictionary<TipoPresentacionListadoRecursosCMS, string> diccionarioNombresListadoGenericos = new Dictionary<TipoPresentacionListadoRecursosCMS, string>();
                    Dictionary<Guid, string> diccionarioNombresListadoPersonalizaciones = new Dictionary<Guid, string>();
                    if (vistasListadoRecursos.Length > 0)
                    {
                        foreach (VistaVirtualCMS filaVistaVirtualCMS in filasListadoRecursos)
                        {
                            if (filaVistaVirtualCMS.TipoComponente.Replace(vistasListadoRecursos, "").IndexOf("/") == -1)
                            {
                                string nombre = filaVistaVirtualCMS.Nombre;
                                if (string.IsNullOrEmpty(nombre))
                                {
                                    nombre = filaVistaVirtualCMS.PersonalizacionComponenteID.ToString();
                                }
                                bool agregado = false;
                                foreach (TipoPresentacionListadoRecursosCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionListadoRecursosCMS)))
                                {
                                    if (filaVistaVirtualCMS.TipoComponente.EndsWith(tipoPresentacion.ToString() + ".cshtml"))
                                    {
                                        if (!diccionarioNombresListadoGenericos.ContainsKey(tipoPresentacion))
                                        {
                                            diccionarioNombresListadoGenericos.Add(tipoPresentacion, nombre);
                                        }
                                        
                                        agregado = true;
                                        break;
                                    }
                                }
                                if (!agregado && !diccionarioNombresListadoPersonalizaciones.ContainsKey(filaVistaVirtualCMS.PersonalizacionComponenteID))
                                {
                                    diccionarioNombresListadoPersonalizaciones.Add(filaVistaVirtualCMS.PersonalizacionComponenteID, nombre);
                                }
                            }
                        }
                    }
                    foreach (TipoPresentacionListadoRecursosCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionListadoRecursosCMS)))
                    {
                        string nombre = UtilIdiomas.GetText("COMADMINCMS", "PRESENTACIONLISTADO_" + tipoPresentacion);
                        if (diccionarioNombresListadoGenericos.ContainsKey(tipoPresentacion))
                        {
                            nombre = diccionarioNombresListadoGenericos[tipoPresentacion];
                        }
                        propiedad.Options.Add(((short)tipoPresentacion).ToString(), nombre);
                    }
                    foreach (Guid idPersonalizacion in diccionarioNombresListadoPersonalizaciones.Keys)
                    {
                        string nombre = diccionarioNombresListadoPersonalizaciones[idPersonalizacion];
                        propiedad.Options.Add(idPersonalizacion.ToString(), nombre);
                    }
                    break;
                case TipoPropiedadCMS.TipoPresentacionListadoUsuarios:
                    foreach (TipoPresentacionListadoUsuariosCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionListadoUsuariosCMS)))
                    {
                        string nombre = UtilIdiomas.GetText("COMADMINCMS", "PRESENTACIONLISTADOUSUARIOS_" + tipoPresentacion);
                        propiedad.Options.Add(((short)tipoPresentacion).ToString(), nombre);
                    }
                    break;
                case TipoPropiedadCMS.TipoListadoUsuarios:
                    foreach (TipoListadoUsuariosCMS tipoListado in Enum.GetValues(typeof(TipoListadoUsuariosCMS)))
                    {
                        string nombre = UtilIdiomas.GetText("COMADMINCMS", "LISTADOUSUARIOS_" + tipoListado);
                        propiedad.Options.Add(((short)tipoListado).ToString(), nombre);
                    }
                    break;
                case TipoPropiedadCMS.TipoPresentacionFaceta:
                    foreach (TipoPresentacionFacetas tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionFacetas)))
                    {
                        string nombre = UtilIdiomas.GetText("COMADMINCMS", "PRESENTACIONFACETAS_" + tipoPresentacion);
                        propiedad.Options.Add(((short)tipoPresentacion).ToString(), nombre);
                    }
                    break;
                case TipoPropiedadCMS.TipoListadoProyectos:
                    foreach (TipoListadoProyectosCMS tipoListadoProyectosCMS in Enum.GetValues(typeof(TipoListadoProyectosCMS)))
                    {
                        string nombre = UtilIdiomas.GetText("COMADMINCMS", propiedad.TipoPropiedadCMS.ToString() + "_" + tipoListadoProyectosCMS.ToString());
                        propiedad.Options.Add(((short)tipoListadoProyectosCMS).ToString(), nombre);
                    }
                    break;
            }

            if (CMSComponente != null && CMSComponente.PropiedadesComponente.ContainsKey(tipoPropiedad))
            {
                if (tipoPropiedad.Equals(TipoPropiedadCMS.ListaIDs) && TipoComponenteCMSActual == TipoComponenteCMS.ListadoProyectos)
                {
                    string[] listaIDs = CMSComponente.PropiedadesComponente[tipoPropiedad].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    List<Guid> listaProyectos = new List<Guid>();
                    foreach (string elemento in listaIDs)
                    {
                        listaProyectos.Add(new Guid(elemento));
                    }

                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    Dictionary<Guid, string> listaNombresCortos = proyCN.ObtenerNombresCortosProyectos(listaProyectos);
                    proyCN.Dispose();
                    //Recorremos esta lista para que no nos cambie el orden de los proyectos.
                    foreach (Guid proyectoID in listaProyectos)
                    {
                        propiedad.Value += listaNombresCortos[proyectoID] + ",";
                    }
                }
                else
                {
                    propiedad.Value = CMSComponente.PropiedadesComponente[tipoPropiedad];
                }

            }
            propiedad.TypeComponent = TipoComponenteCMSActual;

            propiedad.MultiLang = EsPropiedadMultiIdioma(tipoPropiedad);

            return propiedad;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pTipoPropiedad"></param>
        /// <returns></returns>
        private bool EsPropiedadMultiIdioma(TipoPropiedadCMS pTipoPropiedad)
        {
            return UtilComponentes.ListaPropiedadesMultiIdioma.Contains(pTipoPropiedad);
        }

        #endregion

        #region Métodos de Guardado

        public string AgregarPropiedadesComponente(CMSComponente componenteEdicion, CMSAdminComponenteEditarViewModel Componente, GestionCMS gestorCMS, string UrlIntragnossServicios, string BaseURLContent)
        {
            Dictionary<TipoPropiedadCMS, string> PropiedadesCallback = obtenerListaPropiedadesComponente(Componente, UrlIntragnossServicios, BaseURLContent);

            string error = string.Empty;

            switch (componenteEdicion.TipoComponenteCMS)
            {
                case TipoComponenteCMS.HTML:
                    CMSComponenteHTML componenteHTML = (CMSComponenteHTML)componenteEdicion;
                    componenteHTML.Titulo = PropiedadesCallback[TipoPropiedadCMS.Titulo];
                    componenteHTML.HTML = PropiedadesCallback[TipoPropiedadCMS.HTML];
                    break;
                case TipoComponenteCMS.Destacado:
                    CMSComponenteDestacado componenteDestacado = (CMSComponenteDestacado)componenteEdicion;
                    componenteDestacado.Titulo = PropiedadesCallback[TipoPropiedadCMS.Titulo];
                    componenteDestacado.Subtitulo = PropiedadesCallback[TipoPropiedadCMS.Subtitulo];
                    componenteDestacado.Imagen = PropiedadesCallback[TipoPropiedadCMS.Imagen];
                    componenteDestacado.HTML = PropiedadesCallback[TipoPropiedadCMS.HTML];
                    componenteDestacado.Enlace = PropiedadesCallback[TipoPropiedadCMS.Enlace];
                    break;
                case TipoComponenteCMS.ListadoPorParametros:
                    string tipoPresentacionRecursoPorParametros = PropiedadesCallback[TipoPropiedadCMS.TipoPresentacionRecurso];
                    string tipoPresentacionListadoPorParametros = PropiedadesCallback[TipoPropiedadCMS.TipoPresentacionListadoRecursos];

                    CMSComponenteListadoPorParametros componenteListadoPorParametros = (CMSComponenteListadoPorParametros)componenteEdicion;
                    componenteListadoPorParametros.Titulo = PropiedadesCallback[TipoPropiedadCMS.Titulo];
                    componenteListadoPorParametros.TipoPresentacionRecurso = tipoPresentacionRecursoPorParametros;
                    componenteListadoPorParametros.TipoPresentacionListadoRecursos = tipoPresentacionListadoPorParametros;
                    break;
                case TipoComponenteCMS.ListadoEstatico:
                    //short numItemsEstatico = short.Parse(PropiedadesCallback[TipoPropiedadCMS.NumItemsMostrar]);
                    string tipoPresentacionRecursoEstatico = PropiedadesCallback[TipoPropiedadCMS.TipoPresentacionRecurso];
                    string tipoPresentacionListadoEstatico = PropiedadesCallback[TipoPropiedadCMS.TipoPresentacionListadoRecursos];
                    string[] listaIDs = PropiedadesCallback[TipoPropiedadCMS.ListaIDs].Split(',');
                    List<Guid> lista = new List<Guid>();
                    foreach (string id in listaIDs)
                    {
                        if (!string.IsNullOrEmpty(id))
                        {
                            lista.Add(new Guid(id));
                        }
                    }
                    CMSComponenteListadoEstatico componenteListadoEstatico = (CMSComponenteListadoEstatico)componenteEdicion;
                    componenteListadoEstatico.Titulo = PropiedadesCallback[TipoPropiedadCMS.Titulo];
                    componenteListadoEstatico.ListaGuids = lista;
                    //componenteListadoEstatico.NumeroItemsMostrar = numItemsEstatico;
                    //componenteListadoEstatico.URLVerMas = PropiedadesCallback[TipoPropiedadCMS.URLVerMas];
                    componenteListadoEstatico.TipoPresentacionRecurso = tipoPresentacionRecursoEstatico;
                    componenteListadoEstatico.TipoPresentacionListadoRecursos = tipoPresentacionListadoEstatico;
                    break;
                case TipoComponenteCMS.ListadoDinamico:
                    short numItemsDinamico = short.Parse(PropiedadesCallback[TipoPropiedadCMS.NumItems]);
                    string tipoPresentacionRecursoDinamico = PropiedadesCallback[TipoPropiedadCMS.TipoPresentacionRecurso];
                    string tipoPresentacionListadoDinamico = PropiedadesCallback[TipoPropiedadCMS.TipoPresentacionListadoRecursos];
                    CMSComponenteListadoDinamico componenteListadoDinamico = (CMSComponenteListadoDinamico)componenteEdicion;
                    componenteListadoDinamico.Titulo = PropiedadesCallback[TipoPropiedadCMS.Titulo];
                    componenteListadoDinamico.URLBusqueda = PropiedadesCallback[TipoPropiedadCMS.URLBusqueda];
                    componenteListadoDinamico.URLVerMas = PropiedadesCallback[TipoPropiedadCMS.URLVerMas];
                    componenteListadoDinamico.NumeroItems = numItemsDinamico;
                    componenteListadoDinamico.TipoPresentacionRecurso = tipoPresentacionRecursoDinamico;
                    componenteListadoDinamico.TipoPresentacionListadoRecursos = tipoPresentacionListadoDinamico;
                    break;
                case TipoComponenteCMS.ActividadReciente:
                    short numItemsActividadReciente = short.Parse(PropiedadesCallback[TipoPropiedadCMS.NumItems]);
                    CMSComponenteActividadReciente componenteActividadReciente = (CMSComponenteActividadReciente)componenteEdicion;
                    componenteActividadReciente.Titulo = PropiedadesCallback[TipoPropiedadCMS.Titulo];
                    componenteActividadReciente.NumeroItems = numItemsActividadReciente;
                    componenteActividadReciente.TipoActividadReciente = (TipoActividadReciente)short.Parse(PropiedadesCallback[TipoPropiedadCMS.TipoActividadRecienteCMS]);
                    break;
                case TipoComponenteCMS.GrupoComponentes:
                    string[] listaComponentesIDs = PropiedadesCallback[TipoPropiedadCMS.ListaIDs].Split(',');
                    string tipoPresentacionGrupoComponentes = PropiedadesCallback[TipoPropiedadCMS.TipoPresentacionGrupoComponentes];
                    List<Guid> listaComponentes = new List<Guid>();
                    foreach (string id in listaComponentesIDs)
                    {
                        if (id != "")
                        {
                            listaComponentes.Add(new Guid(id));
                        }
                    }
                    CMSComponenteGrupoComponentes componenteGrupoDeComponentes = (CMSComponenteGrupoComponentes)componenteEdicion;
                    componenteGrupoDeComponentes.Titulo = PropiedadesCallback[TipoPropiedadCMS.Titulo];
                    componenteGrupoDeComponentes.ListaGuids = listaComponentes;
                    componenteGrupoDeComponentes.TipoPresentacionGrupoComponentes = tipoPresentacionGrupoComponentes;
                    break;
                case TipoComponenteCMS.Tesauro:
                    CMSComponenteTesauro componenteTesauro = (CMSComponenteTesauro)componenteEdicion;
                    componenteTesauro.Titulo = PropiedadesCallback[TipoPropiedadCMS.Titulo];
                    componenteTesauro.ElementoID = new Guid(PropiedadesCallback[TipoPropiedadCMS.ElementoID]);
                    componenteTesauro.TieneImagen = bool.Parse(PropiedadesCallback[TipoPropiedadCMS.TieneImagen]);
                    if (PropiedadesCallback.ContainsKey(TipoPropiedadCMS.NumItemsMostrar))
                    {
                        componenteTesauro.NumeroItemsMostrar = short.Parse(PropiedadesCallback[TipoPropiedadCMS.NumItemsMostrar]);
                    }
                    break;
                case TipoComponenteCMS.DatosComunidad:
                    CMSComponenteDatosComunidad componenteDatosComunidad = (CMSComponenteDatosComunidad)componenteEdicion;
                    bool contarPersonasNoVisibles = bool.Parse(PropiedadesCallback[TipoPropiedadCMS.ContarPersonasNoVisibles]);
                    componenteDatosComunidad.ContarPersonasNoVisibles = contarPersonasNoVisibles;
                    componenteDatosComunidad.Titulo = PropiedadesCallback[TipoPropiedadCMS.Titulo];
                    break;
                case TipoComponenteCMS.CajaBuscador:
                    CMSComponenteCajaBuscador componenteCajaBuscador = (CMSComponenteCajaBuscador)componenteEdicion;
                    componenteCajaBuscador.Titulo = PropiedadesCallback[TipoPropiedadCMS.Titulo];
                    componenteCajaBuscador.TextoDefecto = PropiedadesCallback[TipoPropiedadCMS.TextoDefecto];
                    componenteCajaBuscador.URLBusqueda = PropiedadesCallback[TipoPropiedadCMS.URLBusqueda];
                    break;
                case TipoComponenteCMS.UsuariosRecomendados:
                    short numItemsUsuariosRecomendados = short.Parse(PropiedadesCallback[TipoPropiedadCMS.NumItems]);
                    CMSComponenteUsuariosRecomendados componenteUsuariosRecomendados = (CMSComponenteUsuariosRecomendados)componenteEdicion;
                    componenteUsuariosRecomendados.Titulo = PropiedadesCallback[TipoPropiedadCMS.Titulo];
                    componenteUsuariosRecomendados.NumeroItems = numItemsUsuariosRecomendados;
                    break;
                case TipoComponenteCMS.Faceta:
                    short tipoPresentacionFacetas = short.Parse(PropiedadesCallback[TipoPropiedadCMS.TipoPresentacionFaceta]);
                    CMSComponenteFaceta componenteFaceta = (CMSComponenteFaceta)componenteEdicion;
                    componenteFaceta.Titulo = PropiedadesCallback[TipoPropiedadCMS.Titulo];
                    componenteFaceta.Faceta = PropiedadesCallback[TipoPropiedadCMS.Faceta];
                    componenteFaceta.URLBusqueda = PropiedadesCallback[TipoPropiedadCMS.URLBusqueda];
                    componenteFaceta.TipoPresentacionFaceta = (TipoPresentacionFacetas)tipoPresentacionFacetas;
                    break;
                case TipoComponenteCMS.ListadoUsuarios:
                    short tipoPresentacionListadoUsuarios = short.Parse(PropiedadesCallback[TipoPropiedadCMS.TipoPresentacionListadoUsuarios]);
                    short tipoListadoUsuarios = short.Parse(PropiedadesCallback[TipoPropiedadCMS.TipoListadoUsuarios]);
                    short numItemsListadoUsuarios = short.Parse(PropiedadesCallback[TipoPropiedadCMS.NumItems]);
                    CMSComponenteListadoUsuarios componenteListadoUsuarios = (CMSComponenteListadoUsuarios)componenteEdicion;
                    componenteListadoUsuarios.Titulo = PropiedadesCallback[TipoPropiedadCMS.Titulo];
                    componenteListadoUsuarios.TipoPresentacionListadoUsuarios = (TipoPresentacionListadoUsuariosCMS)tipoPresentacionListadoUsuarios;
                    componenteListadoUsuarios.TipoListadoUsuarios = (TipoListadoUsuariosCMS)tipoListadoUsuarios;
                    componenteListadoUsuarios.NumeroItems = numItemsListadoUsuarios;
                    break;
                case TipoComponenteCMS.ListadoProyectos:
                    short numItemsListadoProyectos = 1;
                    short.TryParse(PropiedadesCallback[TipoPropiedadCMS.NumItems], out numItemsListadoProyectos);
                    short tipoListadoProyectos = short.Parse(PropiedadesCallback[TipoPropiedadCMS.TipoListadoProyectos]);
                    List<Guid> listaComponentesListadoProyectos = new List<Guid>();
                    if (PropiedadesCallback.ContainsKey(TipoPropiedadCMS.ListaIDs) && !string.IsNullOrEmpty(PropiedadesCallback[TipoPropiedadCMS.ListaIDs]))
                    {
                        string[] listaComponentesIDsListadoProyectos = PropiedadesCallback[TipoPropiedadCMS.ListaIDs].Split(',');
                        foreach (string id in listaComponentesIDsListadoProyectos)
                        {
                            if (!string.IsNullOrEmpty(id))
                            {
                                ProyectoCN proyCN3 = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                                listaComponentesListadoProyectos.Add(proyCN3.ObtenerProyectoIDPorNombre(id));
                                proyCN3.Dispose();
                            }
                        }
                    }
                    CMSComponenteListadoProyectos componenteListadoProyectos = (CMSComponenteListadoProyectos)componenteEdicion;
                    componenteListadoProyectos.Titulo = PropiedadesCallback[TipoPropiedadCMS.Titulo];
                    componenteListadoProyectos.TipoListadoProyectos = (TipoListadoProyectosCMS)tipoListadoProyectos;
                    switch ((TipoListadoProyectosCMS)tipoListadoProyectos)
                    {
                        case TipoListadoProyectosCMS.RecomendadosProyecto:
                        case TipoListadoProyectosCMS.RecomendadosUsuario:
                            componenteListadoProyectos.NumeroItems = numItemsListadoProyectos;
                            componenteListadoProyectos.ListaGuids = new List<Guid>();
                            break;
                        case TipoListadoProyectosCMS.Estaticos:
                            componenteListadoProyectos.NumeroItems = 0;
                            componenteListadoProyectos.ListaGuids = listaComponentesListadoProyectos;
                            if (listaComponentesListadoProyectos.Count == 0)
                            {
                                error = "<p>No existe la comunidad</p>";
                            }
                            break;
                        case TipoListadoProyectosCMS.ComunidadesUsuario:
                            componenteListadoProyectos.NumeroItems = numItemsListadoProyectos;                            
                            componenteListadoProyectos.ListaGuids = new List<Guid>();                           
                            break;
                    }
                    break;
                case TipoComponenteCMS.ResumenPerfil:
                    CMSComponenteResumenPerfil componenteResumenUsuario = (CMSComponenteResumenPerfil)componenteEdicion;
                    componenteResumenUsuario.Titulo = PropiedadesCallback[TipoPropiedadCMS.Titulo];
                    break;
                case TipoComponenteCMS.MasVistos:
                    short numItemsMasVistos = short.Parse(PropiedadesCallback[TipoPropiedadCMS.NumItems]);
                    CMSComponenteMasVistos componenteMasVistos = (CMSComponenteMasVistos)componenteEdicion;
                    componenteMasVistos.Titulo = PropiedadesCallback[TipoPropiedadCMS.Titulo];
                    componenteMasVistos.NumeroItems = numItemsMasVistos;
                    componenteMasVistos.TipoPresentacionRecurso = PropiedadesCallback[TipoPropiedadCMS.TipoPresentacionRecurso];
                    break;
                case TipoComponenteCMS.MasVistosEnXDias:
                    CMSComponenteMasVistosEnXDias componenteMasVistosEnXDias = (CMSComponenteMasVistosEnXDias)componenteEdicion;
                    componenteMasVistosEnXDias.Titulo = PropiedadesCallback[TipoPropiedadCMS.Titulo];
                    componenteMasVistosEnXDias.NumeroItems = short.Parse(PropiedadesCallback[TipoPropiedadCMS.NumItems]);
                    componenteMasVistosEnXDias.NumeroDias = short.Parse(PropiedadesCallback[TipoPropiedadCMS.NumDias]);
                    componenteMasVistosEnXDias.TipoPresentacionRecurso = PropiedadesCallback[TipoPropiedadCMS.TipoPresentacionRecurso];
                    componenteMasVistosEnXDias.TipoPresentacionListadoRecursos = PropiedadesCallback[TipoPropiedadCMS.TipoPresentacionListadoRecursos];
                    break;
                case TipoComponenteCMS.EnvioCorreo:
                    CMSComponenteEnvioCorreo componenteEnvioCorreo = (CMSComponenteEnvioCorreo)componenteEdicion;
                    componenteEnvioCorreo.Titulo = PropiedadesCallback[TipoPropiedadCMS.Titulo];

                    string listaCampos = PropiedadesCallback[TipoPropiedadCMS.ListaCamposEnvioCorreo];
                    Dictionary<short, Dictionary<TipoPropiedadEnvioCorreo, string>> dicListaCampos = new Dictionary<short, Dictionary<TipoPropiedadEnvioCorreo, string>>();

                    string[] campos = listaCampos.Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);
                    short i = 0;
                    foreach (string campo in campos)
                    {
                        string[] propiedades = campo.Split(new string[] { "&&&" }, StringSplitOptions.None);
                        Dictionary<TipoPropiedadEnvioCorreo, string> diccionarioPropiedades = new Dictionary<TipoPropiedadEnvioCorreo, string>();
                        diccionarioPropiedades.Add(TipoPropiedadEnvioCorreo.Nombre, propiedades[0]);
                        diccionarioPropiedades.Add(TipoPropiedadEnvioCorreo.Obligatorio, propiedades[1]);
                        diccionarioPropiedades.Add(TipoPropiedadEnvioCorreo.TipoCampo, propiedades[2]);
                        dicListaCampos.Add(i, diccionarioPropiedades);
                        i++;
                    }
                    componenteEnvioCorreo.ListaCamposEnvioCorreo = dicListaCampos;
                    componenteEnvioCorreo.TextoBoton = PropiedadesCallback[TipoPropiedadCMS.TextoBoton];
                    componenteEnvioCorreo.DestinatarioCorreo = PropiedadesCallback[TipoPropiedadCMS.DestinatarioCorreo];
                    componenteEnvioCorreo.TextoMensajeOK = PropiedadesCallback[TipoPropiedadCMS.TextoMensajeOK];
                    break;
                case TipoComponenteCMS.PreguntaTIC:
                    CMSComponentePreguntaTIC componentePreguntaTIC = (CMSComponentePreguntaTIC)componenteEdicion;
                    componentePreguntaTIC.Titulo = PropiedadesCallback[TipoPropiedadCMS.Titulo];
                    componentePreguntaTIC.OntologiaID = new Guid(PropiedadesCallback[TipoPropiedadCMS.ElementoID]);
                    break;
                case TipoComponenteCMS.Menu:
                    CMSComponenteMenu componenteMenu = (CMSComponenteMenu)componenteEdicion;
                    componenteMenu.Titulo = PropiedadesCallback[TipoPropiedadCMS.Titulo];

                    string listaOpcionesMenu = PropiedadesCallback[TipoPropiedadCMS.ListaOpcionesMenu];
                    Dictionary<short, KeyValuePair<short, Dictionary<TipoPropiedadMenu, string>>> dicListaOpcionesMenu = new Dictionary<short, KeyValuePair<short, Dictionary<TipoPropiedadMenu, string>>>();

                    string[] camposOpcionesMenu = listaOpcionesMenu.Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);

                    Dictionary<short, short> diccionarioOrdenNombreElementosMenu = new Dictionary<short, short>();

                    short orden = 0;
                    foreach (string campo in camposOpcionesMenu)
                    {
                        string[] propiedades = campo.Split(new string[] { "&&&" }, StringSplitOptions.None);
                        string nombre = string.Empty;
                        short posicionOriginal = orden;

                        if (propiedades.Length == 3)
                        {
                            Dictionary<TipoPropiedadMenu, string> diccionarioPropiedades = new Dictionary<TipoPropiedadMenu, string>();
                            diccionarioPropiedades.Add(TipoPropiedadMenu.Nombre, propiedades[1]);
                            diccionarioPropiedades.Add(TipoPropiedadMenu.Enlace, propiedades[2]);
                            dicListaOpcionesMenu.Add(orden, new KeyValuePair<short, Dictionary<TipoPropiedadMenu, string>>(0, diccionarioPropiedades));

                            nombre = propiedades[1];
                        }
                        else if (propiedades.Length == 4)
                        {
                            Dictionary<TipoPropiedadMenu, string> diccionarioPropiedades = new Dictionary<TipoPropiedadMenu, string>();
                            diccionarioPropiedades.Add(TipoPropiedadMenu.Nombre, propiedades[0]);
                            diccionarioPropiedades.Add(TipoPropiedadMenu.Enlace, propiedades[1]);
                            dicListaOpcionesMenu.Add(orden, new KeyValuePair<short, Dictionary<TipoPropiedadMenu, string>>(0, diccionarioPropiedades));
                            short.TryParse(propiedades[2], out posicionOriginal);

                            nombre = propiedades[0];
                        }
                        else if (propiedades.Length == 5)
                        {
                            Dictionary<TipoPropiedadMenu, string> diccionarioPropiedades = new Dictionary<TipoPropiedadMenu, string>();
                            diccionarioPropiedades.Add(TipoPropiedadMenu.Nombre, propiedades[1]);
                            diccionarioPropiedades.Add(TipoPropiedadMenu.Enlace, propiedades[2]);
                            dicListaOpcionesMenu.Add(orden, new KeyValuePair<short, Dictionary<TipoPropiedadMenu, string>>(short.Parse(propiedades[0]), diccionarioPropiedades));
                            short.TryParse(propiedades[3], out posicionOriginal);

                            nombre = propiedades[1];
                        }

                        // Chequear si CMSBloqueComponentePropiedadComponente
                        diccionarioOrdenNombreElementosMenu.Add(posicionOriginal, orden);

                        orden++;
                    }
                    componenteMenu.ListaOpcionesMenu = dicListaOpcionesMenu;

                    // Ordenamos el diccionario por orden ascendente
                    var items = from pair in diccionarioOrdenNombreElementosMenu
                                orderby pair.Key ascending
                                select pair;

                    // Cargar la tabla CMSBloqueComponentePropiedadComponente
                    CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    DataWrapperCMS cmsDWBloqueComponentePropiedadComponente = cmsCN.ObtenerCMSBloqueComponentePropiedadComponente(componenteMenu.ProyectoID, componenteMenu.Clave);

                    gestorCMS.CMSDW.Merge(cmsDWBloqueComponentePropiedadComponente);

                    // Actualizar el orden
                    foreach (KeyValuePair<short, short> posicionComponente in items)
                    {
                        short posicionOriginal = posicionComponente.Key;
                        short nuevaPosicion = posicionComponente.Value;

                        if (posicionOriginal != nuevaPosicion)
                        {
                            // Cambiar la posicion en la BBDD
                            var filasBorradorPublicado = gestorCMS.CMSDW.ListaCMSBloqueComponentePropiedadComponente.Where(fila => fila.ValorPropiedad.Equals(posicionOriginal.ToString()) && fila.ComponenteID.Equals(componenteMenu.Clave));

                            foreach (AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente fila in filasBorradorPublicado)
                            {
                                if (mEntityContext.Entry(fila).State != EntityState.Modified)
                                {
                                    fila.ValorPropiedad = nuevaPosicion.ToString();
                                }
                            }
                        }
                    }
                    break;
                case TipoComponenteCMS.Buscador:
                    short numItemsBuscador = short.Parse(PropiedadesCallback[TipoPropiedadCMS.NumItems]);
                    string tipoPresentacionRecursoBuscador = PropiedadesCallback[TipoPropiedadCMS.TipoPresentacionRecurso];
                    CMSComponenteBuscador componenteBuscador = (CMSComponenteBuscador)componenteEdicion;
                    componenteBuscador.Titulo = PropiedadesCallback[TipoPropiedadCMS.Titulo];
                    componenteBuscador.URLBusqueda = PropiedadesCallback[TipoPropiedadCMS.URLBusqueda];
                    componenteBuscador.TituloAtributoDeBusqueda = PropiedadesCallback[TipoPropiedadCMS.TituloAtributoDeBusqueda];
                    componenteBuscador.AtributoDeBusqueda = PropiedadesCallback[TipoPropiedadCMS.AtributoDeBusqueda];
                    componenteBuscador.NumeroItems = numItemsBuscador;
                    componenteBuscador.TipoPresentacionRecurso = tipoPresentacionRecursoBuscador;

                    break;
                case TipoComponenteCMS.BuscadorSPARQL:
                    short numItemsBuscadorSPARQL = short.Parse(PropiedadesCallback[TipoPropiedadCMS.NumItems]);
                    string tipoPresentacionBuscadorSPARQL = PropiedadesCallback[TipoPropiedadCMS.TipoPresentacionRecurso];
                    CMSComponenteBuscadorSPARQL componenteBuscadorSPARQL = (CMSComponenteBuscadorSPARQL)componenteEdicion;
                    componenteBuscadorSPARQL.Titulo = PropiedadesCallback[TipoPropiedadCMS.Titulo];
                    componenteBuscadorSPARQL.QuerySPARQL = PropiedadesCallback[TipoPropiedadCMS.QuerySPARQL];
                    componenteBuscadorSPARQL.NumeroItems = numItemsBuscadorSPARQL;
                    componenteBuscadorSPARQL.TipoPresentacionRecurso = tipoPresentacionBuscadorSPARQL;
                    break;
                case TipoComponenteCMS.UltimosRecursosVisitados:
                    short numItemsUltimosRecursosVisitados = short.Parse(PropiedadesCallback[TipoPropiedadCMS.NumItems]);
                    string tipoPresentacionUltimosRecursosVisitados = PropiedadesCallback[TipoPropiedadCMS.TipoPresentacionRecurso];
                    string tipoPresentacionListadoUltimosRecursosVisitados = PropiedadesCallback[TipoPropiedadCMS.TipoPresentacionListadoRecursos];
                    CMSComponenteUltimosRecursosVisitados componenteUltimosRecursosVisitados = (CMSComponenteUltimosRecursosVisitados)componenteEdicion;
                    componenteUltimosRecursosVisitados.Titulo = PropiedadesCallback[TipoPropiedadCMS.Titulo];
                    componenteUltimosRecursosVisitados.NumeroItems = numItemsUltimosRecursosVisitados;
                    componenteUltimosRecursosVisitados.TipoPresentacionRecurso = tipoPresentacionUltimosRecursosVisitados;
                    componenteUltimosRecursosVisitados.TipoPresentacionListadoRecursos = tipoPresentacionListadoUltimosRecursosVisitados;
                    break;
                case TipoComponenteCMS.FichaDescripcionDocumento:
                    CMSComponenteFichaDescripcionDocumento componenteFichaDescripcionDocumento = (CMSComponenteFichaDescripcionDocumento)componenteEdicion;
                    componenteFichaDescripcionDocumento.Titulo = PropiedadesCallback[TipoPropiedadCMS.Titulo];
                    componenteFichaDescripcionDocumento.DocumentoID = new Guid(PropiedadesCallback[TipoPropiedadCMS.ElementoID]);

                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    GestorDocumental gestorDoc = new GestorDocumental(docCN.ObtenerDocumentoPorID(componenteFichaDescripcionDocumento.DocumentoID), mLoggingService, mEntityContext);
                    docCN.Dispose();
                    if (gestorDoc.ListaDocumentos.ContainsKey(componenteFichaDescripcionDocumento.DocumentoID))
                    {
                        Documento documento = gestorDoc.ListaDocumentos[componenteFichaDescripcionDocumento.DocumentoID];
                        if (documento.TipoDocumentacion != TiposDocumentacion.Semantico)
                        {
                            error += "<p>El documento con ID ='" + componenteFichaDescripcionDocumento.DocumentoID + "' no es de tipo Semántico</p>";
                        }
                    }
                    else
                    {
                        error += "<p>El documento con ID ='" + componenteFichaDescripcionDocumento.DocumentoID + "' no es de tipo Semántico</p>";
                    }
                    break;
                case TipoComponenteCMS.ConsultaSPARQL:
                    CMSComponenteConsultaSPARQL componenteConsultaSPARQL = (CMSComponenteConsultaSPARQL)componenteEdicion;
                    componenteConsultaSPARQL.Titulo = PropiedadesCallback[TipoPropiedadCMS.Titulo];
                    componenteConsultaSPARQL.QuerySPARQL = PropiedadesCallback[TipoPropiedadCMS.QuerySPARQL];
                    break;
                case TipoComponenteCMS.ConsultaSQLSERVER:
                    CMSComponenteConsultaSQLSERVER componenteConsultaSQLSERVER = (CMSComponenteConsultaSQLSERVER)componenteEdicion;
                    componenteConsultaSQLSERVER.Titulo = PropiedadesCallback[TipoPropiedadCMS.Titulo];
                    componenteConsultaSQLSERVER.QuerySQLSERVER = PropiedadesCallback[TipoPropiedadCMS.QuerySQLSERVER];
                    break;
                default:
                    error = "Método de guardado no implementado para el componente" + TipoComponenteCMSActual;
                    break;
            }

            return error;
        }

		public string ComprobarErrorConcurrencia(CMSAdminComponenteEditarViewModel pComponenteEditado, CMSComponente pComponente)
		{
			string error = string.Empty;

			DateTime fechaCuandoEntraAdministracion = DateTimeRemoveMilliseconds(pComponenteEditado.FechaModificacion);
			DateTime fechaCuandoGuarda = DateTimeRemoveMilliseconds((DateTime)pComponente.FilaComponente.FechaUltimaActualizacion);

            if (fechaCuandoEntraAdministracion < fechaCuandoGuarda)
            {
                error = $"El componente \"{pComponenteEditado.Name}\" ha sido editado por otro usuario. Debes recargar la página y volver a editar.";
            }

			return error;
		}

		private Dictionary<TipoPropiedadCMS, string> obtenerListaPropiedadesComponente(CMSAdminComponenteEditarViewModel Componente, string UrlIntragnossServicios, string BaseURLContent)
        {
            Dictionary<TipoPropiedadCMS, string> propiedadesCallback = new Dictionary<TipoPropiedadCMS, string>();

            foreach (CMSAdminComponenteEditarViewModel.PropiedadComponente propiedad in Componente.Properties)
            {
                string valorPropiedad = HttpUtility.UrlDecode(propiedad.Value);

                if (propiedad.TipoPropiedadCMS.Equals(TipoPropiedadCMS.Imagen))
                {
                    valorPropiedad = ComprobarValorImagen(valorPropiedad, UrlIntragnossServicios, BaseURLContent);

                }

                propiedadesCallback.Add(propiedad.TipoPropiedadCMS, valorPropiedad);
            }

            return propiedadesCallback;
        }

        private string ComprobarValorImagen(string valorPropiedad, string UrlIntragnossServicios, string BaseURLContent)
        {
            string valorPropiedadDefinitivo = "";

            foreach (string idioma in mConfigService.ObtenerListaIdiomas())
            {
                string imagenIdioma = UtilCadenas.ObtenerTextoDeIdioma(valorPropiedad, idioma, null, true);

                if (!string.IsNullOrEmpty(imagenIdioma))
                {
                    string comienzoFile = "File:";
                    if (imagenIdioma.StartsWith(comienzoFile))
                    {
                        string cadenaControl = ";Data:";

                        string[] fichero = imagenIdioma.Split(new string[] { cadenaControl, comienzoFile }, StringSplitOptions.RemoveEmptyEntries);

                        string nombreFichero = UtilCadenas.RemoveAccentsWithRegEx(fichero[0]);
                        string base64Image = fichero[1];

                        List<string> listaExtensiones = new List<string>();
                        listaExtensiones.Add("jpg");
                        listaExtensiones.Add("jpeg");
                        listaExtensiones.Add("png");
                        listaExtensiones.Add("gif");

                        if (listaExtensiones.Contains(nombreFichero.Split('.').Last().ToLower()))
                        {
                            byte[] byteImage = Convert.FromBase64String(base64Image);

                            ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService);
                            servicioImagenes.Url = UrlIntragnossServicios.Replace("https://", "http://");

                            string ruta = $"{UtilArchivos.ContentImagenesProyectos}/personalizacion/{ProyectoSeleccionado.Clave.ToString().ToLower()}/cms/";

                            string primeroDisponible = servicioImagenes.ObtenerNombreDisponible(ruta + nombreFichero);
                            string nombrePrimeroDisponible = primeroDisponible.Substring(0, primeroDisponible.LastIndexOf("."));
                            string extensionPrimeroDisponible = primeroDisponible.Substring(primeroDisponible.LastIndexOf("."));

                            servicioImagenes.AgregarImagen(byteImage, ruta + nombrePrimeroDisponible, extensionPrimeroDisponible);

                            string nombreDefinitivo = ruta + nombrePrimeroDisponible + extensionPrimeroDisponible;

                            valorPropiedadDefinitivo += $"{BaseURLContent}/{UtilArchivos.ContentImagenes}/{nombreDefinitivo}@{idioma}|||";
                        }
                    }
                    else if (imagenIdioma.StartsWith("http://") || imagenIdioma.StartsWith("https://"))
                    {
                        valorPropiedadDefinitivo += $"{imagenIdioma}@{idioma}|||";
                    }
                }
            }

            return valorPropiedadDefinitivo;
        }

        public void GuardarComponente(CMSComponente componenteEdicion, CMSAdminComponenteEditarViewModel Componente)
        {
            bool accesoPublico = false;

            TipoCaducidadComponenteCMS tipoCaducidadComponente = TipoCaducidadComponenteCMS.NoCache;
            List<TipoCaducidadComponenteCMS> caducidadesDisponibles = UtilComponentes.CaducidadesDisponiblesPorTipoComponente[(TipoComponenteCMS)componenteEdicion.FilaComponente.TipoComponente];

            if (Componente.Caducidades != null)
            {
                foreach (KeyValuePair<TipoCaducidadComponenteCMS, bool> par in Componente.Caducidades)
                {
                    if (par.Value)
                    {
                        Componente.CaducidadSeleccionada = par.Key;
                    }
                }
            }

            if (caducidadesDisponibles.Count > 1 && caducidadesDisponibles.Contains(Componente.CaducidadSeleccionada))
            {
                tipoCaducidadComponente = Componente.CaducidadSeleccionada;
            }
            else if (caducidadesDisponibles.Count > 0)
            {
                tipoCaducidadComponente = caducidadesDisponibles[0];
            }

            componenteEdicion.Nombre = Componente.Name;
            componenteEdicion.Estilos = Componente.Styles;
            componenteEdicion.Activo = Componente.Active;
            componenteEdicion.TipoCaducidadComponenteCMS = tipoCaducidadComponente;
            componenteEdicion.AccesoPublico = accesoPublico;
            componenteEdicion.FilaComponente.FechaUltimaActualizacion = DateTime.Now;
            componenteEdicion.NombreCortoComponente = Componente.ShortName;

            #region Privacidad

            componenteEdicion.EliminarLectores();

            if (Componente.Private)
            {
                bool hayGruposPrivacidad = Componente.GruposPrivacidad != null && Componente.GruposPrivacidad.Count > 0;
                bool hayPerfilesPrivacidad = Componente.PerfilesPrivacidad != null && Componente.PerfilesPrivacidad.Count > 0;
                if (hayGruposPrivacidad)
                {
                    IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    Componente.GruposPrivacidad = identCN.ObtenerNombresDeGrupos(Componente.GruposPrivacidad.Keys.ToList());
                    foreach (Guid grupoID in Componente.GruposPrivacidad.Keys)
                    {
                        componenteEdicion.AgregarGrupoEditorAPagina(grupoID);
                    }
                }
                if (hayPerfilesPrivacidad)
                {
                    foreach (Guid perfilID in Componente.PerfilesPrivacidad.Keys)
                    {
                        componenteEdicion.AgregarEditorAPagina(perfilID);
                    }
                }
            }

            #endregion

            componenteEdicion.FilaComponente.IdiomasDisponibles = "";
            if ((ParametroProyecto.ContainsKey(ParametroAD.PropiedadContenidoMultiIdioma) || (ParametroProyecto.ContainsKey(ParametroAD.PropiedadCMSMultiIdioma) && ParametroProyecto[ParametroAD.PropiedadCMSMultiIdioma] == "1")) && Componente.ListaIdiomasDisponibles != null)
            {
                string idiomasDisponibles = "";

                foreach (string idioma in Componente.ListaIdiomasDisponibles)
                {
                    idiomasDisponibles += $"true@{idioma}|||";
                }
                componenteEdicion.FilaComponente.IdiomasDisponibles = idiomasDisponibles;
            }

            componenteEdicion.AccesoPublico = false;
            if (ProyectoSeleccionado.TipoAcceso.Equals(TipoAcceso.Privado) || ProyectoSeleccionado.TipoAcceso.Equals(TipoAcceso.Reservado))
            {
                componenteEdicion.AccesoPublico = Componente.AccesoPublicoComponente;
            }

            componenteEdicion.Personalizacion = null;
            if (!Componente.PersonalizacionSeleccionada.Equals(Guid.Empty) && Componente.PersonalizacionSeleccionada != Guid.Empty)
            {
                componenteEdicion.Personalizacion = Componente.PersonalizacionSeleccionada;
            }

            CMSCN CMSCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            if (componenteEdicion.FilaComponente.TipoComponente == (short)TipoComponenteCMS.HTML)
            {
                // Obtener en qué páginas se encuentra el componente y reprocesarlas por el base.
                DataWrapperCMS paginasComponente = CMSCN.ObtenerPaginasPerteneceComponentePorComponenteID(componenteEdicion.Clave, ProyectoSeleccionado.Clave);

                foreach (AD.EntityModel.Models.CMS.CMSPagina pagina in paginasComponente.ListaCMSPagina)
                {
                    List<ProyectoPestanyaCMS> filasProyPestCMS = ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaCMS.Where(proy => proy.Ubicacion.Equals(pagina.Ubicacion)).ToList();

                    if (filasProyPestCMS.Any())
                    {
                        // Obtenemos la pestanyaID de la página del CMS
                        ProyectoPestanyaCMS filaPestanya = filasProyPestCMS.First();

                        // Por cada página del CMS que contenga este componente enviar a reprocesar.
                        ControladorCMS controlador = new ControladorCMS(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD);
                        controlador.ActualizarModeloBaseSimple(filaPestanya.PestanyaID, ProyectoSeleccionado.Clave, AD.BASE_BD.PrioridadBase.Alta, false);
                    }
                }
            }

            if (ActualizarEnBD)
            {
                CMSCN.ActualizarCMS(componenteEdicion.GestorCMS.CMSDW);
            }

            CMSCN.Dispose();
        }

        #endregion

        #region IntegracionContinua
        public void CrearFilasPropiedadesIntegracionContinua(CMSAdminComponenteEditarViewModel pComponente)
        {
            List<IntegracionContinuaPropiedad> propiedadesIntegracionContinua = new List<IntegracionContinuaPropiedad>();

            try
            {
                foreach (CMSAdminComponenteEditarViewModel.PropiedadComponente propiedad in pComponente.Properties)
                {
                    propiedadesIntegracionContinua = ObtenerPropiedadesIntegracionContinuaComponente(pComponente, propiedadesIntegracionContinua, propiedad);
                }

                using (ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication))
                {
                    proyCN.CrearFilasIntegracionContinuaParametro(propiedadesIntegracionContinua, ProyectoSeleccionado.Clave, TipoObjeto.Componente, pComponente.ShortName);
                }
            }
            catch
            {
            }
        }

        public void ModificarFilasIntegracionContinuaEntornoSiguiente(CMSAdminComponenteEditarViewModel pComponente, string UrlApiDesplieguesEntornoSiguiente, Guid pUsuariID)
        {
            List<IntegracionContinuaPropiedad> propiedadesIntegracionContinua = new List<IntegracionContinuaPropiedad>();

            try
            {
                foreach (CMSAdminComponenteEditarViewModel.PropiedadComponente propiedad in pComponente.Properties)
                {
                    propiedadesIntegracionContinua = ObtenerPropiedadesIntegracionContinuaComponente(pComponente, propiedadesIntegracionContinua, propiedad);
                }
            }
            catch
            {
            }
            try
            {
                string peticion = $"{UrlApiDesplieguesEntornoSiguiente}/PropiedadesIntegracion?nombreProy={ProyectoSeleccionado.NombreCorto}&UsuarioID={pUsuariID}";
                string requestParameters = UtilWeb.WebRequestPostWithJsonObject(peticion, propiedadesIntegracionContinua, "");
            }
            catch
            {
            }
        }

        private List<IntegracionContinuaPropiedad> ObtenerPropiedadesIntegracionContinuaComponente(CMSAdminComponenteEditarViewModel pComponente, List<IntegracionContinuaPropiedad> propiedadesIntegracionContinua, CMSAdminComponenteEditarViewModel.PropiedadComponente propiedad, bool pEsCarga = false)
        {
            if (pComponente.Type.Equals(TipoComponenteCMS.ListadoEstatico) && propiedad.TipoPropiedadCMS.Equals(TipoPropiedadCMS.ListaIDs))
            {
                //Crear las filas de las porpiedades de Integracion Continua
                IntegracionContinuaPropiedad propiedadCMS = new IntegracionContinuaPropiedad();
                propiedadCMS.ProyectoID = ProyectoSeleccionado.Clave;
                propiedadCMS.TipoObjeto = (short)TipoObjeto.Componente;
                propiedadCMS.ObjetoPropiedad = pComponente.ShortName;
                propiedadCMS.TipoPropiedad = (short)TipoPropiedad.IdsRecursosComponente;
                propiedadCMS.ValorPropiedad = propiedad.Value;
                if (!pEsCarga)
                {
                    //propiedad.Value= UtilIntegracionContinua.ObtenerMascaraPropiedad(propiedadCMS);
                }
                propiedadesIntegracionContinua.Add(propiedadCMS);
            }
            else if (pComponente.Type.Equals(TipoComponenteCMS.Tesauro) && propiedad.TipoPropiedadCMS.Equals(TipoPropiedadCMS.ElementoID))
            {
                //Crear las filas de las porpiedades de Integracion Continua
                IntegracionContinuaPropiedad propiedadCMS = new IntegracionContinuaPropiedad();
                propiedadCMS.ProyectoID = ProyectoSeleccionado.Clave;
                propiedadCMS.TipoObjeto = (short)TipoObjeto.Componente;
                propiedadCMS.ObjetoPropiedad = pComponente.ShortName;
                propiedadCMS.TipoPropiedad = (short)TipoPropiedad.TesauroComponente;
                propiedadCMS.ValorPropiedad = propiedad.Value;
                if (!pEsCarga)
                {
                    //propiedad.Value = UtilIntegracionContinua.ObtenerMascaraPropiedad(propiedadCMS);
                }
                propiedadesIntegracionContinua.Add(propiedadCMS);
            }
            return propiedadesIntegracionContinua;
        }
        #endregion

        #region Invalidar Cache

        public void InvalidarCache(Guid componenteID)
        {
            using (CMSCL cmsCL = new CMSCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication))
            {
                cmsCL.InvalidarCacheDeComponentePorIDEnProyectoTodosIdiomas(ProyectoSeleccionado.Clave, componenteID);
                cmsCL.InvalidarCacheConfiguracionCMSPorProyecto(ProyectoSeleccionado.Clave);

                using (CMSCN CMSCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication))
                using (GestionCMS gestorCMS2 = new GestionCMS(CMSCN.ObtenerCMSDeProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext))
                {
                    if (gestorCMS2.ListaPaginasProyectos.ContainsKey(ProyectoSeleccionado.Clave))
                    {
                        foreach (short tipoPagina in gestorCMS2.ListaPaginasProyectos[ProyectoSeleccionado.Clave].Keys)
                        {
                            cmsCL.InvalidarCacheCMSDeUbicacionDeProyecto(tipoPagina, ProyectoSeleccionado.Clave);
                        }
                    }

                }
            }
        }

        #endregion

        #region Metodos de Borrado
        public void BorrarComponenteCrearFilasIntegracionContinua(Guid pComponenteID, CMSCN pCmsCN, GestionCMS pGestorCMS)
        {
            if (pGestorCMS.ListaComponentes.ContainsKey(pComponenteID))
            {
                string nombreCortoComponente = pGestorCMS.ListaComponentes[pComponenteID].NombreCortoComponente;

                List<string> listaPaginasVinculadasComponente = pCmsCN.PaginasVinculadasComponente(pComponenteID, ProyectoSeleccionado.Clave);
                if(listaPaginasVinculadasComponente != null && listaPaginasVinculadasComponente.Count > 0)
                {
                    string nombrePaginas = string.Empty;
                    foreach (string nombrePagina in listaPaginasVinculadasComponente.Distinct())
                    {
                        nombrePaginas += $"{UtilCadenas.ObtenerTextoDeIdioma(nombrePagina, UtilIdiomas.LanguageCode, IdiomaPorDefecto)}, ";
                    }

                    nombrePaginas = nombrePaginas.Substring(0, nombrePaginas.Length - 2);

                    throw new ErrorComponenteVinculadoPagina(UtilIdiomas.GetText("COMADMINCMS", "ELIMINARCOMPONENTEVINCULADOPAGINA", nombrePaginas));
                }

                pGestorCMS.EliminarComponente(pComponenteID);
                pCmsCN.ActualizarCMS(pGestorCMS.CMSDW);
                try
                {
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    proyCN.CrearFilasIntegracionContinuaParametro(new List<IntegracionContinuaPropiedad>(), ProyectoSeleccionado.Clave, TipoObjeto.Componente, nombreCortoComponente);
                    proyCN.Dispose();
                }
                catch { }
            }
        }

        #endregion
    }
}
