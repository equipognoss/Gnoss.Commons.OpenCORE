using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSName;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Parametro;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.ParametroGeneralDSName;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.Controles.Administracion
{
    public class ControladorOpcionesAvanzadas
    {
        private Proyecto ProyectoSeleccionado = null;
        private Dictionary<string, string> mParametroProyecto = null;

        //private ParametroGeneralDS mParametrosGeneralesDS;
        //private ParametroGeneralDS.ParametroGeneralRow mFilaParametrosGenerales = null;
        private GestorParametroGeneral mParametrosGeneralesDS;
        private ParametroGeneral mFilaParametrosGenerales = null;

        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private EntityContext mEntityContext;
        private IHttpContextAccessor mHttpContextAccessor;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private GnossCache mGnossCache;
        private EntityContextBASE mEntityContextBASE;

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public ControladorOpcionesAvanzadas(Proyecto pProyecto, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, EntityContextBASE entityContextBASE, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mHttpContextAccessor = httpContextAccessor;
            mRedisCacheWrapper = redisCacheWrapper;
            mEntityContextBASE = entityContextBASE;

            ProyectoSeleccionado = pProyecto;
        }

        #endregion

        #region Métodos de carga

        public AdministrarOpcionesAvanzadasViewModel CargarOpcionesAvanzadas()
        {
            AdministrarOpcionesAvanzadasViewModel mPaginaModel = new AdministrarOpcionesAvanzadasViewModel();
            mPaginaModel.CMSActivado = FilaParametrosGenerales.CMSDisponible;
            if (FilaParametrosGenerales.CMSDisponible)
            {
                mPaginaModel.GruposVisibilidadAbierto = CargarGruposVisibilidadAbierto();

                mPaginaModel.AutocompletarSiempreVirtuoso = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "ConfigBBDDAutocompletarProyecto");
                mPaginaModel.MostrarAccionesListados = FilaParametrosGenerales.MostrarAccionesEnListados;
                mPaginaModel.IncluirGoogleSearch = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "IncluirGoogleSearch");

                string ontologiaPatron = ControladorProyecto.ObtenerParametroString(ParametroProyecto, ParametroAD.ProyectoIDPatronOntologias);
                mPaginaModel.OntologiaOtroProyecto = string.IsNullOrEmpty(ontologiaPatron) ? Guid.Empty : new Guid(ontologiaPatron);
            }

            // Buscar en todo el ecosistema y el proyecto
            bool buscarTodoEcosistema = true;
            bool buscarTodoProyecto = true;
            if (ParametrosGeneralesDS.ListaConfiguracionAmbitoBusquedaProyecto.Count > 0)
            {
                ConfiguracionAmbitoBusquedaProyecto filaAmbitoBusqueda =ParametrosGeneralesDS.ListaConfiguracionAmbitoBusquedaProyecto[0];
                buscarTodoEcosistema = filaAmbitoBusqueda.TodoGnoss;
                buscarTodoProyecto = filaAmbitoBusqueda.Metabusqueda;
            }
            mPaginaModel.BuscarTodoEcosistema = buscarTodoEcosistema;
            mPaginaModel.BuscarTodoProyecto = buscarTodoProyecto;
            mPaginaModel.PestanyasSeleccionadas = HayPestanyaSeleccionada(mPaginaModel);

            mPaginaModel.PermitirRecursosPrivados = FilaParametrosGenerales.PermitirRecursosPrivados;
            mPaginaModel.InvitacionesDisponibles = FilaParametrosGenerales.InvitacionesDisponibles;
            mPaginaModel.VotacionesDisponibles = FilaParametrosGenerales.VotacionesDisponibles;
            mPaginaModel.PermitirVotacionesNegativas = FilaParametrosGenerales.PermitirVotacionesNegativas;
            mPaginaModel.MostrarVotaciones = FilaParametrosGenerales.VerVotaciones;
            mPaginaModel.ComentariosDisponibles = FilaParametrosGenerales.ComentariosDisponibles;
            mPaginaModel.SupervisoresPuedenAdministrarGrupos = FilaParametrosGenerales.SupervisoresAdminGrupos;
            mPaginaModel.CuentaTwitter = ProyectoSeleccionado.FilaProyecto.UsuarioTwitter;
            mPaginaModel.HasTagTwitter = ProyectoSeleccionado.FilaProyecto.TagTwitter;
            //mPaginaModel.RobotsBusqueda = ControladorProyecto.ObtenerParametroString(ParametroProyecto, ParametroAD.RobotsComunidad);
            mPaginaModel.NumeroCaracteresDescripcionSuscripcion = ControladorProyecto.ObtenerParametroString(ParametroProyecto, ParametroAD.NumeroCaracteresDescripcion);
            mPaginaModel.ParametrosExtraYoutube = ControladorProyecto.ObtenerParametroString(ParametroProyecto, ParametroAD.ParametrosExtraYoutube);
            mPaginaModel.CompartirRecursoPermitido = FilaParametrosGenerales.CompartirRecursosPermitido;


            //if (!FilaParametrosGenerales.IsCodigoGoogleAnalyticsNull())
            //if (!(FilaParametrosGenerales.CodigoGoogleAnalytics==null))
            //{
            //    mPaginaModel.CodigoGoogleAnalytics = FilaParametrosGenerales.CodigoGoogleAnalytics;
            //}
            ////if (!FilaParametrosGenerales.IsScriptGoogleAnalyticsNull())
            //if (!(FilaParametrosGenerales.ScriptGoogleAnalytics==null))
            //{
            //    mPaginaModel.ScriptGoogleAnalytics = FilaParametrosGenerales.ScriptGoogleAnalytics;
            //}

            // Correo
            ParametroCN paramCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, null);
            ConfiguracionEnvioCorreo filaConfiguracionEnvioCorreo = paramCN.ObtenerFilaConfiguracionEnvioCorreo(ProyectoSeleccionado.Clave);
            paramCN.Dispose();

            if (filaConfiguracionEnvioCorreo != null)
            {
                mPaginaModel.ConfiguracionCorreo = new ConfiguradorCorreo();

                mPaginaModel.ConfiguracionCorreo.Email = filaConfiguracionEnvioCorreo.email;
                mPaginaModel.ConfiguracionCorreo.SMTP = filaConfiguracionEnvioCorreo.smtp;
                mPaginaModel.ConfiguracionCorreo.Port = filaConfiguracionEnvioCorreo.puerto;
                mPaginaModel.ConfiguracionCorreo.User = filaConfiguracionEnvioCorreo.usuario;
                mPaginaModel.ConfiguracionCorreo.Type = filaConfiguracionEnvioCorreo.tipo;
                mPaginaModel.ConfiguracionCorreo.SSL = filaConfiguracionEnvioCorreo.SSL.HasValue && filaConfiguracionEnvioCorreo.SSL.Value == true;
                mPaginaModel.ConfiguracionCorreo.SuggestEmail = filaConfiguracionEnvioCorreo.emailsugerencias;
                mPaginaModel.ConfiguracionCorreo.Password = filaConfiguracionEnvioCorreo.clave;
            }
            return mPaginaModel;
        }

        private Dictionary<Guid, string> CargarGruposVisibilidadAbierto()
        {
            string gruposVisibilidadAbierto = ControladorProyecto.ObtenerParametroString(ParametroProyecto, "GruposPermitidosSeleccionarPrivacidadRecursoAbierto");
            List<Guid> listaGrupos = new List<Guid>();

            if (!string.IsNullOrEmpty(gruposVisibilidadAbierto))
            {
                string[] arrayGrupos = gruposVisibilidadAbierto.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string grupo in arrayGrupos)
                {
                    Guid grupoID = Guid.Empty;
                    Guid.TryParse(grupo, out grupoID);

                    if (grupoID != Guid.Empty)
                    {
                        listaGrupos.Add(grupoID);
                    }
                }
            }

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, null);
            Dictionary<Guid, string> nombresGrupos = identidadCN.ObtenerNombresDeGrupos(listaGrupos);
            identidadCN.Dispose();

            return nombresGrupos;
        }

        #endregion

        #region Métodos de Guardado

        public void GuardarOpcionesAvanzadas(AdministrarOpcionesAvanzadasViewModel pOptions)
        {
            PasarDatosADataSet(pOptions);
            mEntityContext.NoConfirmarTransacciones = true;
            try
            {
                mEntityContext.SaveChanges();

                GuardarDatosConfiguracionCorreo(pOptions);
               
                mEntityContext.TerminarTransaccionesPendientes(true);
            }
            catch
            {
                mEntityContext.TerminarTransaccionesPendientes(false);
                throw;
            }
        }

        private void GuardarDatosConfiguracionCorreo(AdministrarOpcionesAvanzadasViewModel pOptions)
        {
            ParametroCN paramCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, null);
            ConfiguracionEnvioCorreo filaConfiguracionEnvioCorreo = paramCN.ObtenerFilaConfiguracionEnvioCorreo(ProyectoSeleccionado.Clave);

            bool existeConfiguracionAnterior = filaConfiguracionEnvioCorreo != null;
            if (!existeConfiguracionAnterior)
            {
                filaConfiguracionEnvioCorreo = new ConfiguracionEnvioCorreo();
            }

            if (pOptions.ConfiguracionCorreo != null && !string.IsNullOrEmpty(pOptions.ConfiguracionCorreo.Email))
            {
                if (!existeConfiguracionAnterior || !string.IsNullOrEmpty(pOptions.ConfiguracionCorreo.Password))
                {
                    filaConfiguracionEnvioCorreo.clave = pOptions.ConfiguracionCorreo.Password;
                }

                filaConfiguracionEnvioCorreo.ProyectoID = ProyectoSeleccionado.Clave;
                filaConfiguracionEnvioCorreo.email = pOptions.ConfiguracionCorreo.Email;
                filaConfiguracionEnvioCorreo.smtp = pOptions.ConfiguracionCorreo.SMTP;
                filaConfiguracionEnvioCorreo.puerto = pOptions.ConfiguracionCorreo.Port;
                filaConfiguracionEnvioCorreo.usuario = pOptions.ConfiguracionCorreo.User;
                filaConfiguracionEnvioCorreo.tipo = pOptions.ConfiguracionCorreo.Type;
                filaConfiguracionEnvioCorreo.SSL = pOptions.ConfiguracionCorreo.SSL;
                filaConfiguracionEnvioCorreo.emailsugerencias = pOptions.ConfiguracionCorreo.SuggestEmail;

                paramCN.GuardarFilaConfiguracionEnvioCorreo(filaConfiguracionEnvioCorreo, !existeConfiguracionAnterior);
            }
            else if (existeConfiguracionAnterior)
            {
                paramCN.BorrarFilaConfiguracionEnvioCorreo(ProyectoSeleccionado.Clave);

            }
            paramCN.Dispose();
        }

        private void PasarDatosADataSet(AdministrarOpcionesAvanzadasViewModel pOptions)
        {
            ControladorProyecto controlador = new ControladorProyecto(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, null);

            if (FilaParametrosGenerales.CMSDisponible)
            {
                string gruposVisibilidadAbierto = "";
                if (pOptions.GruposVisibilidadAbierto != null)
                {
                    foreach (Guid grupo in pOptions.GruposVisibilidadAbierto.Keys)
                    {
                        gruposVisibilidadAbierto += grupo.ToString() + "|||";
                    }
                }
                controlador.GuardarParametroString(ParametrosGeneralesDS, "GruposPermitidosSeleccionarPrivacidadRecursoAbierto", gruposVisibilidadAbierto);

                string ontologiaPatron = pOptions.OntologiaOtroProyecto == Guid.Empty ? "" : pOptions.OntologiaOtroProyecto.ToString();
                controlador.GuardarParametroString(ParametrosGeneralesDS, ParametroAD.ProyectoIDPatronOntologias, ontologiaPatron);

                FilaParametrosGenerales.MostrarAccionesEnListados = pOptions.MostrarAccionesListados;
                controlador.GuardarParametroBooleano(ParametrosGeneralesDS, "IncluirGoogleSearch", pOptions.IncluirGoogleSearch);
                controlador.GuardarParametroBooleano(ParametrosGeneralesDS, "ConfigBBDDAutocompletarProyecto", pOptions.AutocompletarSiempreVirtuoso);
            }

            //Guardar la opción seleccionada
           
            PestanyasBusquedaProyecto(pOptions);

            FilaParametrosGenerales.PermitirRecursosPrivados = pOptions.PermitirRecursosPrivados;
            FilaParametrosGenerales.InvitacionesDisponibles = pOptions.InvitacionesDisponibles;
            FilaParametrosGenerales.VotacionesDisponibles = pOptions.VotacionesDisponibles;
            FilaParametrosGenerales.PermitirVotacionesNegativas = pOptions.PermitirVotacionesNegativas;
            FilaParametrosGenerales.VerVotaciones = pOptions.MostrarVotaciones;
            FilaParametrosGenerales.ComentariosDisponibles = pOptions.ComentariosDisponibles;
            FilaParametrosGenerales.SupervisoresAdminGrupos = pOptions.SupervisoresPuedenAdministrarGrupos;
            FilaParametrosGenerales.CompartirRecursosPermitido = pOptions.CompartirRecursoPermitido;

            //if (!string.IsNullOrEmpty(pOptions.CodigoGoogleAnalytics))
            //{
            //    FilaParametrosGenerales.CodigoGoogleAnalytics = pOptions.CodigoGoogleAnalytics;
            //}
            ////else if (!FilaParametrosGenerales.IsCodigoGoogleAnalyticsNull())
            //else if (!(FilaParametrosGenerales.CodigoGoogleAnalytics==null))
            //{
            //    FilaParametrosGenerales.CodigoGoogleAnalytics = null; ;
            //    //FilaParametrosGenerales.SetCodigoGoogleAnalyticsNull();
            //}

            //if (!string.IsNullOrEmpty(pOptions.ScriptGoogleAnalytics))
            //{
            //    FilaParametrosGenerales.ScriptGoogleAnalytics = pOptions.ScriptGoogleAnalytics;
            //}
            ////else if (!FilaParametrosGenerales.IsScriptGoogleAnalyticsNull())
            //else if (!(FilaParametrosGenerales.ScriptGoogleAnalytics==null))
            //{
            //    FilaParametrosGenerales.ScriptGoogleAnalytics = null;
            //}

            //controlador.GuardarParametroString(ParametrosGeneralesDS, ParametroAD.RobotsComunidad, pOptions.RobotsBusqueda);
            controlador.GuardarParametroString(ParametrosGeneralesDS, ParametroAD.NumeroCaracteresDescripcion, pOptions.NumeroCaracteresDescripcionSuscripcion);
            controlador.GuardarParametroString(ParametrosGeneralesDS, ParametroAD.ParametrosExtraYoutube, pOptions.ParametrosExtraYoutube);

            PasarDatosAmbitoBusquedaADataSet(pOptions);
            PasarDatosTwitterADataSet(pOptions);
        }

        private void PasarDatosAmbitoBusquedaADataSet(AdministrarOpcionesAvanzadasViewModel pOptions)
        {
            ConfiguracionAmbitoBusquedaProyecto filaAmbitoBusqueda;
            if (ParametrosGeneralesDS.ListaConfiguracionAmbitoBusquedaProyecto.Count > 0)
            {
                filaAmbitoBusqueda = ParametrosGeneralesDS.ListaConfiguracionAmbitoBusquedaProyecto[0];
                filaAmbitoBusqueda.TodoGnoss = pOptions.BuscarTodoEcosistema;
                filaAmbitoBusqueda.Metabusqueda = pOptions.BuscarTodoProyecto;
            }
            else 
            {
                filaAmbitoBusqueda = new ConfiguracionAmbitoBusquedaProyecto();
                filaAmbitoBusqueda.ProyectoID = ProyectoSeleccionado.Clave;
                filaAmbitoBusqueda.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                filaAmbitoBusqueda.TodoGnoss = pOptions.BuscarTodoEcosistema;
                filaAmbitoBusqueda.Metabusqueda = pOptions.BuscarTodoProyecto;

                if (pOptions.PestanyasSeleccionadas.HasValue && !pOptions.PestanyasSeleccionadas.Value.Equals(Guid.Empty))
                {
                    filaAmbitoBusqueda.PestanyaDefectoID = pOptions.PestanyasSeleccionadas;
                }

                ParametrosGeneralesDS.ListaConfiguracionAmbitoBusquedaProyecto.Add(filaAmbitoBusqueda);
                ParametroGeneralGBD gestorController = new ParametroGeneralGBD(mEntityContext);
                gestorController.addAmbitoBusqueda(filaAmbitoBusqueda);
                //gestorController.saveChanges();
                //filaAmbitoBusqueda.SetPestanyaDefectoIDNull();
                //ParametrosGeneralesDS.ConfiguracionAmbitoBusquedaProyecto.AddConfiguracionAmbitoBusquedaProyectoRow(filaAmbitoBusqueda);
            }
        }

        private void PasarDatosTwitterADataSet(AdministrarOpcionesAvanzadasViewModel pOptions)
        {
            if (string.IsNullOrEmpty(pOptions.CuentaTwitter))
            {
                ProyectoSeleccionado.FilaProyecto.TieneTwitter = false;
                ProyectoSeleccionado.FilaProyecto.UsuarioTwitter = null;
                ProyectoSeleccionado.FilaProyecto.TagTwitter = null;

                ProyectoSeleccionado.FilaProyecto.TokenTwitter = null;
                ProyectoSeleccionado.FilaProyecto.TokenSecretoTwitter = null;
            }
            else
            {
                ProyectoSeleccionado.FilaProyecto.TieneTwitter = true;
                ProyectoSeleccionado.FilaProyecto.UsuarioTwitter = pOptions.CuentaTwitter;
                ProyectoSeleccionado.FilaProyecto.TagTwitter = pOptions.HasTagTwitter;

                if (!string.IsNullOrEmpty(pOptions.TokenTwitter))
                {
                    ProyectoSeleccionado.FilaProyecto.TokenTwitter = pOptions.TokenTwitter;
                    ProyectoSeleccionado.FilaProyecto.TokenSecretoTwitter = pOptions.TokenSecretTwitter;
                }
            }
        }
        private void PestanyasBusquedaProyecto(AdministrarOpcionesAvanzadasViewModel pOptions)
        {
            if (pOptions.PestanyasSeleccionadas.HasValue && !pOptions.PestanyasSeleccionadas.Value.Equals(Guid.Empty))
            {
                ParametroGeneralGBD gestorController = new ParametroGeneralGBD(mEntityContext);
                ConfiguracionAmbitoBusquedaProyecto confBusqueda = gestorController.ObtenerConfiguracionAmbitoBusqueda(ProyectoSeleccionado.Clave);
                confBusqueda.PestanyaDefectoID = pOptions.PestanyasSeleccionadas;
            }
        }
        private Guid? HayPestanyaSeleccionada(AdministrarOpcionesAvanzadasViewModel pOptions)
        {
            ParametroGeneralGBD gestorController = new ParametroGeneralGBD(mEntityContext);
            ConfiguracionAmbitoBusquedaProyecto confBusqueda = gestorController.ObtenerConfiguracionAmbitoBusqueda(ProyectoSeleccionado.Clave);
            if (confBusqueda != null)
            {
                return confBusqueda.PestanyaDefectoID;
            }
            else
            {
                return null;
            }
            
        }

        #endregion

        #region Invalidar Cache

        public void InvalidarCaches()
        {
            ParametroGeneralCL paramCL = new ParametroGeneralCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, null);
            paramCL.InvalidarCacheParametrosGeneralesDeProyecto(ProyectoSeleccionado.Clave);

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, null);
            proyCL.InvalidarParametrosProyecto(ProyectoSeleccionado.Clave, ProyectoSeleccionado.FilaProyecto.OrganizacionID);
            proyCL.InvalidarFilaProyecto(ProyectoSeleccionado.Clave);

            proyCL.InvalidarComunidadMVC(ProyectoSeleccionado.Clave);
            proyCL.InvalidarCabeceraMVC(ProyectoSeleccionado.Clave);

            proyCL.Dispose();
        }

        #endregion

        #region Propiedades

        private ParametroGeneral FilaParametrosGenerales
        {
            get
            {
                if (mFilaParametrosGenerales == null)
                {
                    //mFilaParametrosGenerales = ParametrosGeneralesDS.ParametroGeneral.FindByOrganizacionIDProyectoID(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave);
                    mFilaParametrosGenerales = ParametrosGeneralesDS.ListaParametroGeneral.Find(parametrosGenerales=> parametrosGenerales.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && parametrosGenerales.ProyectoID.Equals(ProyectoSeleccionado.Clave));
                }
                return mFilaParametrosGenerales;
            }
        }

        /// <summary>
        /// Parámetros de un proyecto.
        /// </summary>
        private Dictionary<string, string> ParametroProyecto
        {
            get
            {
                if (mParametroProyecto == null)
                {
                    ParametroCN parametroCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, null);
                    mParametroProyecto = parametroCN.ObtenerParametrosProyecto(ProyectoSeleccionado.Clave);
                    parametroCN.Dispose();
                }

                return mParametroProyecto;
            }
        }

        /// <summary>
        /// Obtiene el dataset de parámetros generales
        /// </summary>
        // private ParametroGeneralDS ParametrosGeneralesDS
        private GestorParametroGeneral ParametrosGeneralesDS
        {
            get
            {
                if (mParametrosGeneralesDS == null)
                {
                    //ParametroGeneralCN paramCN = new ParametroGeneralCN();
                    //mParametrosGeneralesDS = paramCN.ObtenerParametrosGeneralesDeProyecto(ProyectoSeleccionado.Clave);
                    //paramCN.Dispose();
                    ParametroGeneralGBD gestorController = new ParametroGeneralGBD(mEntityContext);
                    mParametrosGeneralesDS = new GestorParametroGeneral();
                    mParametrosGeneralesDS = gestorController.ObtenerParametrosGeneralesDeProyecto(mParametrosGeneralesDS, ProyectoSeleccionado.Clave); 
                    foreach (string parametro in ParametroProyecto.Keys)
                    {
                        ParametroProyecto parametroProyecto = new ParametroProyecto(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, parametro, ParametroProyecto[parametro]);
                        //mParametrosGeneralesDS.ParametroProyecto.AddParametroProyectoRow(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, parametro, ParametroProyecto[parametro]);
                        mParametrosGeneralesDS.ListaParametroProyecto.Add(parametroProyecto);
                    }
                    //gestorController.saveChanges();
                    //mParametrosGeneralesDS.ParametroProyecto.AcceptChanges();
                }
                return mParametrosGeneralesDS;
            }
        }

        #endregion
    }
}
