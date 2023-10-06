using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.Live;
using Es.Riam.Gnoss.AD.Notificacion;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Suscripcion;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Suscripcion;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Live;
using Es.Riam.Gnoss.Logica.Notificacion;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Suscripcion;
using Es.Riam.Gnoss.Logica.Tesauro;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Web.Controles.Suscripcion
{
    /// <summary>
    /// Controlador de suscripciones
    /// </summary>
    public class ControladorSuscripciones
    {
        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        #region Constructor

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ControladorSuscripciones(LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Carga las identidades a las que está suscrita la identidad pasada por parámetro
        /// </summary>
        /// <param name="pIdentidad">Identidad de la que se deben cargar las suscripciones</param>
        public void CargarIdentidadesSuscritas(Identidad pIdentidad)
        {
            SuscripcionCN mSuscripcionCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperSuscripcion mSuscripcionDW = mSuscripcionCN.ObtenerSuscripcionesDeIdentidad(pIdentidad.IdentidadMyGNOSS.Clave, false);
            mSuscripcionCN.Dispose();

            List<Guid> listaUsuarios = new List<Guid>();

            foreach (AD.EntityModel.Models.Suscripcion.SuscripcionTesauroUsuario filaSuscripcion in mSuscripcionDW.ListaSuscripcionTesauroUsuario)
            {
                if (!listaUsuarios.Contains(filaSuscripcion.UsuarioID))
                {
                    listaUsuarios.Add(filaSuscripcion.UsuarioID);
                }
            }

            List<Guid> listaIdentidades = new List<Guid>();
            foreach (AD.EntityModel.Models.Suscripcion.SuscripcionIdentidadProyecto filaSuscripcion in mSuscripcionDW.ListaSuscripcionIdentidadProyecto)
            {
                if (!listaIdentidades.Contains(filaSuscripcion.IdentidadID))
                {
                    listaIdentidades.Add(filaSuscripcion.IdentidadID);
                }
            }

            UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<AD.EntityModel.Models.UsuarioDS.Usuario> usuarios = usuarioCN.ObtenerUsuariosPorIdentidadesCargaLigera(listaIdentidades);
            usuarioCN.Dispose();

            foreach (AD.EntityModel.Models.UsuarioDS.Usuario filaUusuario in usuarios)
            {
                listaUsuarios.Add(filaUusuario.UsuarioID);
            }

            //IdentidadCN identidadCN = new IdentidadCN();
            //pIdentidad.ListaPerfilesSuscritos = identidadCN.ObtenerListaPerfilPersonalPorUsuarioID(listaUsuarios);
            //identidadCN.Dispose();

            if (pIdentidad.GestionSuscripcion == null)
            {
                pIdentidad.GestionSuscripcion = new GestionSuscripcion(mSuscripcionDW, mLoggingService, mEntityContext);
            }
            else
            {
                pIdentidad.GestionSuscripcion.SuscripcionDW.Merge(mSuscripcionDW);
            }
        }

        /// <summary>
        /// Carga las identidades a las que está suscrita la identidad pasada por parámetro
        /// </summary>
        /// <param name="pIdentidad">Identidad de la que se deben cargar las suscripciones</param>
        /// <param name="pProyecto">Id del proyecto</param>
        public void CargarIdentidadesSuscritasEnProyecto(Identidad pIdentidad, Guid pProyecto)
        {
            SuscripcionCN mSuscripcionCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperSuscripcion mSuscripcionDW = mSuscripcionCN.ObtenerSuscripcionesDeIdentidad(pIdentidad.IdentidadMyGNOSS.Clave, false);
            mSuscripcionCN.Dispose();

            List<Guid> listaPerfiles = new List<Guid>();


            //Lista de identidades a las q esta suscrita
            List<Guid> listaIdentidades = new List<Guid>();
            foreach (AD.EntityModel.Models.Suscripcion.SuscripcionIdentidadProyecto filaSuscripcion in mSuscripcionDW.ListaSuscripcionIdentidadProyecto)
            {
                if (!listaIdentidades.Contains(filaSuscripcion.IdentidadID))
                {
                    listaIdentidades.Add(filaSuscripcion.IdentidadID);
                }
            }

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionIdentidades gestorIden = new GestionIdentidades(identidadCN.ObtenerIdentidadesPorID(listaIdentidades, true), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);


            //Lista de prefiles suscritas en ese proyecto 
            List<Guid> listaPerfilesEnProyecto = new List<Guid>();

            if (pProyecto == ProyectoAD.MyGnoss)
            {
                foreach (Identidad identidad in gestorIden.ListaIdentidades.Values)
                {
                    if ((identidad.FilaIdentidad.ProyectoID == ProyectoAD.MyGnoss) && !listaPerfilesEnProyecto.Contains(identidad.Clave))
                    {
                        listaPerfilesEnProyecto.Add(identidad.FilaIdentidad.PerfilID);
                    }
                }
            }
            else
            {
                foreach (Identidad identidad in gestorIden.ListaIdentidades.Values)
                {
                    if ((identidad.FilaIdentidad.ProyectoID == pProyecto || identidad.FilaIdentidad.ProyectoID == ProyectoAD.MyGnoss) && !listaPerfilesEnProyecto.Contains(identidad.Clave))
                    {
                        listaPerfilesEnProyecto.Add(identidad.FilaIdentidad.PerfilID);
                    }
                }
            }


            //Metemos en ListaPerfilesSuscritos todos los perfiles a los que estamos suscritos en ese proyecto
            pIdentidad.ListaPerfilesSuscritos = listaPerfilesEnProyecto;

            if (pIdentidad.GestionSuscripcion == null)
            {
                pIdentidad.GestionSuscripcion = new GestionSuscripcion(mSuscripcionDW, mLoggingService, mEntityContext);
            }
            else
            {
                pIdentidad.GestionSuscripcion.SuscripcionDW.Merge(mSuscripcionDW);
            }
        }

        /// <summary>
        /// Método para suscribirse a un perfil.
        /// </summary>
        /// <param name="pIdentidadActual">Identidad actual</param>
        /// <param name="pProyectoSeleccionado">Proyecto actual</param>
        /// <param name="UrlIntragnoss">Url de intragnoss</param>
        /// <param name="pIdentidadID">Identidad a la que me suscribo</param>
        /// <param name="pSuscribirmeComunidad">Indica si se suscribe a la comunidad</param>
        /// <param name="pSuscribirmeTodaActividad">Indica si se suscribe a toda sus actividad</param>
        public void SuscribirmePerfil(Identidad pIdentidadActual, Proyecto pProyectoSeleccionado, string BaseURL, string UrlIntragnoss, Identidad pIdentidadSuscripcion, bool? pSuscribirmeComunidad, bool? pSuscribirmeTodaActividad, bool? pSuscribirmeTesauroUsusario, string pLanguageCode)
        {
            SuscribirmePerfil(pIdentidadActual, pProyectoSeleccionado, BaseURL, UrlIntragnoss, pIdentidadSuscripcion, pSuscribirmeComunidad, pSuscribirmeTodaActividad, pSuscribirmeTesauroUsusario, 0, pLanguageCode);
        }


        /// <summary>
        /// Método para suscribirse a un perfil.
        /// </summary>
        /// <param name="pIdentidadActual">Identidad actual</param>
        /// <param name="pProyectoSeleccionado">Proyecto actual</param>
        /// <param name="UrlIntragnoss">Url de intragnoss</param>
        /// <param name="pIdentidadID">Identidad a la que me suscribo</param>
        /// <param name="pSuscribirmeComunidad">Indica si se suscribe a la comunidad</param>
        /// <param name="pSuscribirmeTodaActividad">Indica si se suscribe a toda sus actividad</param>
        public void SuscribirmePerfil(Identidad pIdentidadActual, Proyecto pProyectoSeleccionado, string BaseURL, string UrlIntragnoss, Identidad pIdentidadSuscripcion, bool? pSuscribirmeComunidad, bool? pSuscribirmeTodaActividad, bool? pSuscribirmeTesauroUsusario, int pPeriodicidad, string pLanguageCode)
        {
            //Guid identidadID = pIdentidadID;

            //IdentidadCN identidadCN = new IdentidadCN();
            //PersonaCN personaCN = new PersonaCN();
            //GestionIdentidades gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerIdentidadPorID(identidadID, true), new GestionPersonas((PersonaDS)personaCN.ObtenerPersonaPorIdentidadCargaLigera(identidadID).Table.DataSet), null);

            //Identidad identidadInvitado = gestorIdentidades.ListaIdentidades[identidadID];

            bool incluirBRPersonal = pIdentidadSuscripcion.Tipo != TiposIdentidad.Organizacion;

            SuscripcionCN suscripcionCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionSuscripcion gestorSuscripciones = new GestionSuscripcion(suscripcionCN.ObtenerSuscripcionDePerfilAPerfil(pIdentidadActual.PerfilID, pIdentidadSuscripcion.PerfilID, false, incluirBRPersonal), mLoggingService, mEntityContext);

            //Si tiene el parámetro SeguirEnTodaLaActividad=true solo se puede seguir en toda la actividad
            ParametroAplicacionCL parametroAplicacionCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            //ParametroAplicacionDS parametroAplicacionDS = parametroAplicacionCL.ObtenerParametrosAplicacion();
            List<AD.EntityModel.ParametroAplicacion> parametroAplicacionDS = parametroAplicacionCL.ObtenerParametrosAplicacionPorContext();
            List<AD.EntityModel.ParametroAplicacion> busqueda = parametroAplicacionDS.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.SeguirEnTodaLaActividad)).ToList();
            //bool seguirEnTodaLaActividad = parametroAplicacionDS.Select("Parametro = '" + TiposParametrosAplicacion.SeguirEnTodaLaActividad + "'").Length > 0 && bool.Parse((string)parametroAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.SeguirEnTodaLaActividad + "'")[0]["Valor"]);
            bool seguirEnTodaLaActividad = busqueda.Count > 0 && bool.Parse(busqueda.First().Valor);
            parametroAplicacionCL.Dispose();

            if (seguirEnTodaLaActividad)
            {
                if (pSuscribirmeComunidad.HasValue)
                {
                    pSuscribirmeTodaActividad = pSuscribirmeComunidad;
                    pSuscribirmeComunidad = null;
                }
                if (pSuscribirmeTesauroUsusario.HasValue)
                {
                    pSuscribirmeTodaActividad = pSuscribirmeTesauroUsusario;
                    pSuscribirmeTesauroUsusario = null;
                }
            }

            #region  SuscribirmeTodaActividad
            if (pSuscribirmeTodaActividad.HasValue)
            {
                //ID de la suscripción
                Guid suscripcionID = Guid.Empty;

                if (gestorSuscripciones.ListaSuscripciones.Count == 0)
                {
                    //añadimos la suscripcion
                    suscripcionID = gestorSuscripciones.AgregarNuevaSuscripcion(pIdentidadActual, 1);

                    DataWrapperNotificacion notificacionDS = new DataWrapperNotificacion();
                    GestionNotificaciones mGestionNotificaciones = new GestionNotificaciones(notificacionDS, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
					//bool noEnviarNotificacion = mConfigService.ObtenerNoEnviarCorreoSuscripcion();
					bool noEnviarNotificacion = false;
					ParametroAplicacionCN paramCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
					string noEnviar = paramCN.ObtenerParametroAplicacion(TiposParametrosAplicacion.NoEnviarCorreoSeguirPerfil);
                    bool.TryParse(paramCN.ObtenerParametroAplicacion(TiposParametrosAplicacion.NoEnviarCorreoSeguirPerfil), out noEnviarNotificacion);
					
                    if (!noEnviarNotificacion)
                    {
                        mGestionNotificaciones.AgregarNotificacionCorreo(pIdentidadActual, pIdentidadSuscripcion.IdentidadMyGNOSS, TiposNotificacion.SeguirPerfil, BaseURL, pProyectoSeleccionado, pLanguageCode);
                        NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        notificacionCN.ActualizarNotificacion();
                    }
                    
                }
                else
                {
                    suscripcionID = ((gestorSuscripciones.SuscripcionDW.ListaSuscripcion[0])).SuscripcionID;
                }

                #region obtenemos el usuario del Perfil
                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                Guid idUsuarioPerfil = usuarioCN.ObtenerUsuarioIDPorIDPerfil(pIdentidadSuscripcion.PerfilID).Value;
                usuarioCN.Dispose();
                #endregion

                #region obtenemos el tesauro del perfil al que nos suscribimos
                TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                Guid idTesauroPerfil = tesauroCN.ObtenerIDTesauroDeUsuario(idUsuarioPerfil);
                tesauroCN.Dispose();
                #endregion

                #region Eliminamos todas las suscripciones a comunidades de la suscripcion  (si las hay)

                if (gestorSuscripciones.ListaSuscripciones.Count > 0 && gestorSuscripciones.ListaSuscripciones[suscripcionID].FilaSuscripcionIdentidadProyecto != null)
                {
                    List<AD.EntityModel.Models.Suscripcion.SuscripcionIdentidadProyecto> listaSuscripcionIdentidadProyecto = gestorSuscripciones.ListaSuscripciones[suscripcionID].FilaSuscripcionIdentidadProyecto.ToList();
                    //gestorSuscripciones.ListaSuscripciones.Remove(suscripcionID);

                    foreach (AD.EntityModel.Models.Suscripcion.SuscripcionIdentidadProyecto suscripcionIdentidadProyecto in listaSuscripcionIdentidadProyecto)
                    {
                        mEntityContext.EliminarElemento(suscripcionIdentidadProyecto);
                    }
                }

                #endregion

                #region Eliminamos la suscripcion a tesaurousuario (si la hay)

                if (gestorSuscripciones.ListaSuscripciones.Count > 0 && gestorSuscripciones.ListaSuscripciones[suscripcionID].FilaSuscripcionTesauroUsuario != null)
                {
                    AD.EntityModel.Models.Suscripcion.SuscripcionTesauroUsuario suscripcionTesauroUsuario = gestorSuscripciones.ListaSuscripciones[suscripcionID].FilaSuscripcionTesauroUsuario;
                    //gestorSuscripciones.ListaSuscripciones.Remove(suscripcionID);

                    mEntityContext.EliminarElemento(suscripcionTesauroUsuario);
                }

                if (gestorSuscripciones.ListaSuscripciones.Count > 0 && (gestorSuscripciones.ListaSuscripciones[suscripcionID].FilaSuscripcionTesauroUsuario != null || gestorSuscripciones.ListaSuscripciones[suscripcionID].FilaSuscripcionIdentidadProyecto != null))
                {
                    gestorSuscripciones.ListaSuscripciones.Remove(suscripcionID);
                }
                #endregion

                if (pSuscribirmeTodaActividad.Value)
                {
                    //Añadimos una suscripcion a la identidad en MyGnoss (con la que estaremos suscritos a todos los proyectos)
                    gestorSuscripciones.AgregarSuscripcionAUsuarioEnProyecto(pIdentidadSuscripcion.IdentidadMyGNOSS.Clave, ProyectoAD.MetaProyecto, ProyectoAD.MetaOrganizacion, pPeriodicidad, suscripcionID);

                    //insertamos en virtuoso
                    FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, false, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    facetadoCN.InsertarNuevoSeguidor(pIdentidadActual.PerfilID.ToString(), pIdentidadSuscripcion.PerfilID.ToString(), ProyectoAD.MetaProyecto.ToString());
                    facetadoCN.InsertarNuevoSeguidorProyecto(pIdentidadActual.Clave.ToString(), pIdentidadSuscripcion.Clave.ToString(), pProyectoSeleccionado.Clave.ToString());
                    facetadoCN.Dispose();

                    //Añadimos una suscripcion a tesaurousuario (si está disponible)
                    if (incluirBRPersonal)
                    {
                        gestorSuscripciones.AgregarSuscripcionAUsuario(idUsuarioPerfil, idTesauroPerfil, pPeriodicidad, suscripcionID);
                    }
                }
                else
                {
                    //Eliminamos la suscripcion
                    if (gestorSuscripciones.ListaSuscripciones.Count > 0 && gestorSuscripciones.ListaSuscripciones[suscripcionID].FilaSuscripcion != null)
                    {
                        AD.EntityModel.Models.Suscripcion.Suscripcion suscripcion = gestorSuscripciones.ListaSuscripciones[suscripcionID].FilaSuscripcion;

                        gestorSuscripciones.ListaSuscripciones.Remove(suscripcionID);
                        mEntityContext.EliminarElemento(suscripcion);

                    }
                }
            }
            #endregion

            #region SuscribirmeComunidad
            if (pSuscribirmeComunidad.HasValue)
            {
                //ID de la suscripción
                Guid suscripcionID = Guid.Empty;

                if (gestorSuscripciones.ListaSuscripciones.Count == 0)
                {
                    //añadimos la suscripcion
                    suscripcionID = gestorSuscripciones.AgregarNuevaSuscripcion(pIdentidadActual, 1);

                    DataWrapperNotificacion notificacionDS = new DataWrapperNotificacion();
                    GestionNotificaciones mGestionNotificaciones = new GestionNotificaciones(notificacionDS, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
					//bool noEnviarNotificacion = mConfigService.ObtenerNoEnviarCorreoSuscripcion();
					bool noEnviarNotificacion = false;
					ParametroAplicacionCN paramCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
					string noEnviar = paramCN.ObtenerParametroAplicacion(TiposParametrosAplicacion.NoEnviarCorreoSeguirPerfil);
					bool.TryParse(paramCN.ObtenerParametroAplicacion(TiposParametrosAplicacion.NoEnviarCorreoSeguirPerfil), out noEnviarNotificacion);
					if (!noEnviarNotificacion)
                    {
                        mGestionNotificaciones.AgregarNotificacionCorreo(pIdentidadActual, pIdentidadSuscripcion.IdentidadMyGNOSS, TiposNotificacion.SeguirPerfilComunidad, BaseURL, pProyectoSeleccionado, pLanguageCode);
                        NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        notificacionCN.ActualizarNotificacion();
                    }                  
                }
                else
                {
                    suscripcionID = gestorSuscripciones.SuscripcionDW.ListaSuscripcion[0].SuscripcionID;
                }

                #region obtenemos el usuario del Perfil
                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                Guid idUsuarioPerfil = usuarioCN.ObtenerUsuarioIDPorIDPerfil(pIdentidadSuscripcion.PerfilID).Value;
                usuarioCN.Dispose();
                #endregion

                #region obtenemos el tesauro del perfil al que nos suscribimos
                TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                Guid idTesauroPerfil = tesauroCN.ObtenerIDTesauroDeUsuario(idUsuarioPerfil);
                tesauroCN.Dispose();
                #endregion

                #region Eliminamos la suscripcion al proyecto seleccionado (si existe)

                if (gestorSuscripciones.ListaSuscripciones.Count > 0 && gestorSuscripciones.ListaSuscripciones[suscripcionID].FilaSuscripcionIdentidadProyecto != null)
                {
                    List<AD.EntityModel.Models.Suscripcion.SuscripcionIdentidadProyecto> listaSuscripcionIdentidadProyecto = gestorSuscripciones.ListaSuscripciones[suscripcionID].FilaSuscripcionIdentidadProyecto.ToList();
                    gestorSuscripciones.ListaSuscripciones.Remove(suscripcionID);

                    foreach (AD.EntityModel.Models.Suscripcion.SuscripcionIdentidadProyecto suscripcionIdentidadProyecto in listaSuscripcionIdentidadProyecto)
                    {
                        mEntityContext.EliminarElemento(suscripcionIdentidadProyecto);
                    }
                }

                #endregion

                //Comprobamos si esta suscrito a toda la actividad
                bool estaSuscritoTodaActividad = false;
                if (gestorSuscripciones.ListaSuscripciones.Count > 0)
                {
                    List<AD.EntityModel.Models.Suscripcion.SuscripcionIdentidadProyecto> filasSuscripcionTodaActividad = gestorSuscripciones.SuscripcionDW.ListaSuscripcionIdentidadProyecto.Where(item => item.ProyectoID.Equals(ProyectoAD.MetaProyecto)).ToList();

                    if (filasSuscripcionTodaActividad.Count > 0)
                    {
                        estaSuscritoTodaActividad = true;
                    }
                }

                if (pSuscribirmeComunidad.Value)
                {
                    if (!estaSuscritoTodaActividad)
                    {
                        //Añadimos una suscripcion a la identidad en MyGnoss (con la que estaremos suscritos a todos los proyectos)
                        gestorSuscripciones.AgregarSuscripcionAUsuarioEnProyecto(pIdentidadSuscripcion.Clave, pProyectoSeleccionado.Clave, pProyectoSeleccionado.FilaProyecto.OrganizacionID, pPeriodicidad, suscripcionID);

                        //insertamos en virtuoso
                        FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, false, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                        facetadoCN.InsertarNuevoSeguidor(pIdentidadActual.PerfilID.ToString(), pIdentidadSuscripcion.PerfilID.ToString(), pProyectoSeleccionado.Clave.ToString());
                        facetadoCN.InsertarNuevoSeguidorProyecto(pIdentidadActual.Clave.ToString(), pIdentidadSuscripcion.Clave.ToString(), pProyectoSeleccionado.Clave.ToString());
                        facetadoCN.Dispose();
                    }
                }
                else
                {
                    //borramos las tripletas de virtuoso
                    FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, false, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    facetadoCN.BorrarSeguidor(pIdentidadActual.PerfilID.ToString(), pIdentidadSuscripcion.PerfilID.ToString(), ProyectoAD.MetaProyecto.ToString());
                    facetadoCN.BorrarSeguidorProyecto(pIdentidadActual.Clave.ToString(), pIdentidadSuscripcion.Clave.ToString(), ProyectoAD.MetaProyecto.ToString());

                    if (estaSuscritoTodaActividad)
                    {
                        //Eliminamos la suscripcion a toda la actividad y creamos la de todos los proyectos
                        gestorSuscripciones.EliminarSuscripcionAIdentidadEnProyecto(suscripcionID, ProyectoAD.MetaProyecto);

                        //Cargamos los proyecto comunes
                        GestionProyecto gestorProyectosComunes;
                        ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        if (pIdentidadSuscripcion.Tipo == TiposIdentidad.Personal)
                        {
                            gestorProyectosComunes = new GestionProyecto(proyectoCN.ObtenerListaProyectosComunesParticipanPerfilesUsuarios(pIdentidadActual.PerfilID, pIdentidadActual.Tipo, pIdentidadSuscripcion.PerfilID, pIdentidadSuscripcion.Tipo, true), mLoggingService, mEntityContext);
                        }
                        else
                        {
                            gestorProyectosComunes = new GestionProyecto(proyectoCN.ObtenerListaProyectosComunesParticipanPerfilesUsuarios(pIdentidadActual.PerfilID, pIdentidadActual.Tipo, pIdentidadSuscripcion.PerfilID, pIdentidadSuscripcion.Tipo, false), mLoggingService, mEntityContext);
                        }
                        proyectoCN.Dispose();

                        IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                        //Cargamos las identidades del perfil                        
                        GestionIdentidades gestorIdentidadesInvitado = new GestionIdentidades(identidadCN.ObtenerIdentidadesDePerfil(pIdentidadSuscripcion.PerfilID), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                        identidadCN.Dispose();

                        //Recorremos los proyectos comunes
                        foreach (Guid proyectoID in gestorProyectosComunes.ListaProyectos.Keys)
                        {
                            gestorSuscripciones.EliminarSuscripcionAIdentidadEnProyecto(suscripcionID, proyectoID);
                            facetadoCN.BorrarSeguidor(pIdentidadActual.PerfilID.ToString(), pIdentidadSuscripcion.PerfilID.ToString(), proyectoID.ToString());
                            facetadoCN.BorrarSeguidorProyecto(pIdentidadActual.Clave.ToString(), pIdentidadSuscripcion.Clave.ToString(), proyectoID.ToString());

                            //Recorremos las identidades del perfil del invitado
                            foreach (Identidad identidadALaQueSuscribirse in gestorIdentidadesInvitado.ListaIdentidades.Values)
                            {
                                if (proyectoID != pProyectoSeleccionado.Clave && proyectoID != ProyectoAD.MetaProyecto && proyectoID == identidadALaQueSuscribirse.FilaIdentidad.ProyectoID)
                                {
                                    //Si se trata de una identidad en un proyecto en comun, agregamos la suscripción
                                    if (gestorSuscripciones.ListaSuscripciones.Count == 0)
                                    {
                                        suscripcionID = gestorSuscripciones.AgregarNuevaSuscripcion(pIdentidadActual, 1);
                                    }
                                    else
                                    {
                                        suscripcionID = gestorSuscripciones.SuscripcionDW.ListaSuscripcion[0].SuscripcionID;
                                    }
                                    gestorSuscripciones.AgregarSuscripcionAUsuarioEnProyecto(identidadALaQueSuscribirse.Clave, identidadALaQueSuscribirse.FilaIdentidad.ProyectoID, identidadALaQueSuscribirse.FilaIdentidad.OrganizacionID, 0, suscripcionID);

                                    //insertamos en virtuoso
                                    facetadoCN.InsertarNuevoSeguidor(pIdentidadActual.PerfilID.ToString(), identidadALaQueSuscribirse.PerfilID.ToString(), identidadALaQueSuscribirse.FilaIdentidad.ProyectoID.ToString());
                                    facetadoCN.InsertarNuevoSeguidorProyecto(pIdentidadActual.Clave.ToString(), identidadALaQueSuscribirse.Clave.ToString(), identidadALaQueSuscribirse.FilaIdentidad.ProyectoID.ToString());
                                }
                            }
                        }
                        facetadoCN.Dispose();
                    }
                }
            }
            #endregion

            #region SuscribirmeTesauro
            if (pSuscribirmeTesauroUsusario.HasValue && incluirBRPersonal)
            {
                //ID de la suscripción
                Guid suscripcionID = Guid.Empty;

                if (gestorSuscripciones.ListaSuscripciones.Count == 0)
                {
                    //añadimos la suscripcion
                    suscripcionID = gestorSuscripciones.AgregarNuevaSuscripcion(pIdentidadActual, 1);
                }
                else
                {
                    suscripcionID = gestorSuscripciones.SuscripcionDW.ListaSuscripcion[0].SuscripcionID;
                }

                #region obtenemos el usuario del Perfil
                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                Guid idUsuarioPerfil = usuarioCN.ObtenerUsuarioIDPorIDPerfil(pIdentidadSuscripcion.PerfilID).Value;
                usuarioCN.Dispose();
                #endregion

                #region obtenemos el tesauro del perfil al que nos suscribimos
                TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                Guid idTesauroPerfil = tesauroCN.ObtenerIDTesauroDeUsuario(idUsuarioPerfil);
                tesauroCN.Dispose();
                #endregion

                #region Eliminamos la suscripcion a tesaurousuario (si la hay)

                if (gestorSuscripciones.ListaSuscripciones.Count > 0 && gestorSuscripciones.ListaSuscripciones[suscripcionID].FilaSuscripcionTesauroUsuario != null)
                {
                    AD.EntityModel.Models.Suscripcion.SuscripcionTesauroUsuario suscripcionTesauroUsuario = gestorSuscripciones.ListaSuscripciones[suscripcionID].FilaSuscripcionTesauroUsuario;
                    gestorSuscripciones.ListaSuscripciones.Remove(suscripcionID);

                    mEntityContext.EliminarElemento(suscripcionTesauroUsuario);
                }

                #endregion

                if (pSuscribirmeTesauroUsusario.Value)
                {
                    gestorSuscripciones.AgregarSuscripcionAUsuario(idUsuarioPerfil, idTesauroPerfil, pPeriodicidad, suscripcionID);
                }
            }
            #endregion

            suscripcionCN.ActualizarSuscripcion();
            suscripcionCN.Dispose();

            LiveUsuariosCN liveUsuariosCN = new LiveUsuariosCN("base", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            liveUsuariosCN.InsertarFilaEnColaUsuarios(pProyectoSeleccionado.Clave, pIdentidadActual.PerfilID, (int)AccionLive.SuscribirseUsuario, (int)TipoLive.Miembro, pIdentidadSuscripcion.PerfilID.ToString());
        }

        /// <summary>
        /// Método para suscribirse a un perfil.
        /// </summary>
        /// <param name="pIdentidadActual">Identidad actual</param>
        /// <param name="pProyectoSeleccionado">Proyecto actual</param>
        /// <param name="UrlIntragnoss">Url de intragnoss</param>
        /// <param name="pIdentidadID">Identidad a la que me suscribo</param>
        /// <param name="pSuscribirme">Indica si se suscribe</param>
        /// <param name="pPeriodicidad"></param>
        public void SuscribirmeOrganizacion(Identidad pIdentidadActual, Proyecto pProyectoSeleccionado, string UrlIntragnoss, Identidad pIdentidadSuscripcion, bool pSuscribirme, int pPeriodicidad)
        {
            if (pIdentidadSuscripcion.OrganizacionID.HasValue)
            {
                SuscripcionCN suscripcionCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                GestionSuscripcion GestorSuscripciones = new GestionSuscripcion(suscripcionCN.ObtenerSuscripcionDePerfilAOrganizacion(pIdentidadActual.PerfilID, pIdentidadSuscripcion.OrganizacionID.Value, false), mLoggingService, mEntityContext);

                //ID de la suscripción
                Guid SuscripcionID = Guid.Empty;

                if (GestorSuscripciones.ListaSuscripciones.Count == 0)
                {
                    //añadimos la suscripcion
                    SuscripcionID = GestorSuscripciones.AgregarNuevaSuscripcion(pIdentidadActual, 1);
                }
                else
                {
                    SuscripcionID = GestorSuscripciones.SuscripcionDW.ListaSuscripcion[0].SuscripcionID;
                }

                #region obtenemos el tesauro de la organizacion a la que nos suscribimos

                TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                Guid idTesauroOrganizacion = tesauroCN.ObtenerIDTesauroDeOrganizacion(pIdentidadSuscripcion.OrganizacionID.Value);
                tesauroCN.Dispose();

                #endregion


                #region Eliminamos la suscripcion a tesaurousuario (si la hay)

                if (GestorSuscripciones.ListaSuscripciones.Count > 0 && GestorSuscripciones.ListaSuscripciones[SuscripcionID].FilaSuscripcionTesauroOrganizacion != null)
                {
                    AD.EntityModel.Models.Suscripcion.SuscripcionTesauroOrganizacion suscripcionTesauroOrganizacion = GestorSuscripciones.ListaSuscripciones[SuscripcionID].FilaSuscripcionTesauroOrganizacion;
                    GestorSuscripciones.ListaSuscripciones.Remove(SuscripcionID);
                    mEntityContext.EliminarElemento(suscripcionTesauroOrganizacion);
                }

                #endregion

                if (pSuscribirme)
                {
                    GestorSuscripciones.AgregarSuscripcionAOrganizacion(pIdentidadSuscripcion.OrganizacionID.Value, idTesauroOrganizacion, pPeriodicidad, SuscripcionID);

                }
                suscripcionCN.ActualizarSuscripcion();
                suscripcionCN.Dispose();

                if (pSuscribirme)
                {
                    LiveUsuariosCN liveUsuariosCN = new LiveUsuariosCN("base", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    liveUsuariosCN.InsertarFilaEnColaUsuarios(pProyectoSeleccionado.Clave, pIdentidadActual.PerfilID, (int)AccionLive.SuscribirseUsuario, (int)TipoLive.Miembro, pIdentidadSuscripcion.PerfilID.ToString());
                }
            }
        }

        /// <summary>
        /// Método para suscribirse a un perfil.
        /// </summary>
        /// <param name="pIdentidadActual">Identidad actual</param>
        /// <param name="pProyectoSeleccionado">Proyecto actual</param>
        /// <param name="UrlIntragnoss">Url de intragnoss</param>
        /// <param name="pIdentidadID">Identidad a la que me suscribo</param>
        /// <param name="pSuscribirmeComunidad">Indica si se suscribe a la comunidad</param>
        /// <param name="pSuscribirmeTodaActividad">Indica si se suscribe a toda sus actividad</param>
        public bool EstaSuscritoAPerfil(Identidad pIdentidadActual, Proyecto pProyectoSeleccionado, Guid pIdentidadID)
        {
            Guid identidadID = pIdentidadID;

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionIdentidades gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerIdentidadPorID(identidadID, false), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            identidadCN.Dispose();

            Identidad identidadInvitado = gestorIdentidades.ListaIdentidades[identidadID];

            SuscripcionCN suscripcionCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionSuscripcion GestorSuscripciones = new GestionSuscripcion(suscripcionCN.ObtenerSuscripcionDePerfilAPerfil(pIdentidadActual.PerfilID, identidadInvitado.PerfilID, false, identidadInvitado.Tipo == TiposIdentidad.Personal), mLoggingService, mEntityContext);

            //Comprobamos si esta suscrito a toda la actividad
            bool estaSuscritoTodaActividad = false;

            if (GestorSuscripciones.ListaSuscripciones.Count > 0)
            {
                List<AD.EntityModel.Models.Suscripcion.SuscripcionIdentidadProyecto> filasSuscripcionTodaActividad = GestorSuscripciones.SuscripcionDW.ListaSuscripcionIdentidadProyecto.Where(item => item.ProyectoID.Equals(ProyectoAD.MetaProyecto)).ToList();

                if (filasSuscripcionTodaActividad.Count > 0)
                {
                    estaSuscritoTodaActividad = true;
                }
            }

            //Comprobamos si esta suscrito a la comunidad
            bool estaSuscritoComunidad = false;

            if (GestorSuscripciones.ListaSuscripciones.Count > 0)
            {
                List<AD.EntityModel.Models.Suscripcion.SuscripcionIdentidadProyecto> filasSuscripcionComunidad = GestorSuscripciones.SuscripcionDW.ListaSuscripcionIdentidadProyecto.Where(item => item.ProyectoID.Equals(pProyectoSeleccionado.Clave)).ToList();

                if (filasSuscripcionComunidad.Count > 0)
                {
                    estaSuscritoComunidad = true;
                }
            }

            GestorSuscripciones.Dispose();
            suscripcionCN.Dispose();

            return (estaSuscritoTodaActividad || estaSuscritoComunidad);
        }

        #endregion

        #region Métodos estáticos

        /// <summary>
        /// Pone a 0 el contador de nuevas sucripciones.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public void ResetearContadorNuevasSucripciones(Guid pPerfilID)
        {
            LiveCN liveCN = new LiveCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            liveCN.ResetearContadorNuevasSuscripciones(pPerfilID);
            liveCN.Dispose();
        }

        /// <summary>
        /// Agrega una suscripción al tesauro de la comunidad para la identidad
        /// </summary>
        /// <param name="pGestorSuscripcion">Gestor de suscripciones</param>
        /// <param name="pIdentidadID">Identidad que se va a suscribir</param>
        /// <param name="pTesauroID">Tesauro al que se va a agregar la suscripción</param>
        /// <param name="pOrganizacionID">Identificador de la organización de la comunidad</param>
        /// <param name="pProyectoID">Identificador de la comunidad</param>
        /// <param name="pPeriodicidadSuscripcion">Periodicidad con la que se va a recibir el boletín de suscripción(Diaria/Semanal/Noenviar)</param>
        /// <returns>Devuelve la suscripción</returns>
        public Elementos.Suscripcion.Suscripcion CrearSuscripcionTesauroProyecto(GestionSuscripcion pGestorSuscripcion, Guid pIdentidadID, Guid pTesauroID, Guid pOrganizacionID, Guid pProyectoID, PeriodicidadSuscripcion pPeriodicidadSuscripcion)
        {
            if (pGestorSuscripcion == null)
            {
                SuscripcionCN suscCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                DataWrapperSuscripcion suscDW = suscCN.ObtenerSuscripcionesDeIdentidad(pIdentidadID, false);
                suscCN.Dispose();
                pGestorSuscripcion = new GestionSuscripcion(suscDW, mLoggingService, mEntityContext);
            }

            Elementos.Suscripcion.Suscripcion suscr = pGestorSuscripcion.CrearSuscripcion(pIdentidadID);
            suscr.FilaSuscripcion.Periodicidad = (short)pPeriodicidadSuscripcion;
            pGestorSuscripcion.CrearSuscripcionTesauroProyecto(suscr.FilaSuscripcion.SuscripcionID, pOrganizacionID, pProyectoID, pTesauroID);

            return suscr;
        }

        /// <summary>
        /// Suscribe una identidad al tesauro de la comunidad si la comunidad está configurada como SuscribirATodaComunidad
        /// </summary>
        /// <param name="pGestorSuscripcion">Gestor de suscripciones</param>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pProyectoID">Identificador de la comunidad</param>
        public void SuscribirIdentidadATesauroProyecto(GestionSuscripcion pGestorSuscripcion, Guid pIdentidadID, Guid pProyectoID)
        {
            ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            Dictionary<string, string> parametroProyecto = proyectoCL.ObtenerParametrosProyecto(pProyectoID);
            proyectoCL.Dispose();

            //cargamos las suscripciones de la identidad
            SuscripcionCN suscCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            if (pGestorSuscripcion == null)
            {
                DataWrapperSuscripcion suscDW = suscCN.ObtenerSuscripcionesDeIdentidad(pIdentidadID, false);
                pGestorSuscripcion = new GestionSuscripcion(suscDW, mLoggingService, mEntityContext);
            }
            else
            {
                pGestorSuscripcion.SuscripcionDW.Merge(suscCN.ObtenerSuscripcionesDeIdentidad(pIdentidadID, false));
            }
            suscCN.Dispose();

            if (parametroProyecto.ContainsKey(ParametroAD.SuscribirATodaComunidad) && parametroProyecto[ParametroAD.SuscribirATodaComunidad].ToLower().Equals("true"))
            {
                //obtenemos las suscripciones de la identidad en el proyecto
                Elementos.Suscripcion.Suscripcion suscripcionProyecto = pGestorSuscripcion.ObtenerSuscripcionAProyecto(pProyectoID);

                TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                Guid tesauroID = tesauroCN.ObtenerIDTesauroDeProyecto(pProyectoID);
                GestionTesauro gestorTesauro = new GestionTesauro(tesauroCN.ObtenerTesauroCompletoPorID(tesauroID), mLoggingService, mEntityContext);
                tesauroCN.Dispose();

                if (suscripcionProyecto == null)
                {
                    PeriodicidadSuscripcion periodicidadSuscripcion = PeriodicidadSuscripcion.Diaria;
                    if (parametroProyecto.ContainsKey(ParametroAD.PeriodicidadSuscripcion) && !parametroProyecto[ParametroAD.PeriodicidadSuscripcion].ToLower().Equals("noenviar"))
                    {
                        periodicidadSuscripcion = PeriodicidadSuscripcion.Diaria;
                        if (parametroProyecto[ParametroAD.PeriodicidadSuscripcion].Equals("7"))
                        {
                            periodicidadSuscripcion = PeriodicidadSuscripcion.Semanal;
                        }
                    }
                    else
                    {
                        periodicidadSuscripcion = PeriodicidadSuscripcion.NoEnviar;
                    }

                    suscripcionProyecto = CrearSuscripcionTesauroProyecto(pGestorSuscripcion, pIdentidadID, tesauroID, ProyectoAD.MetaOrganizacion, pProyectoID, periodicidadSuscripcion);

                    foreach (Guid catID in gestorTesauro.ListaCategoriasTesauro.Keys)
                    {
                        pGestorSuscripcion.VincularCategoria(suscripcionProyecto, gestorTesauro.ListaCategoriasTesauro[catID]);
                    }
                }
            }
        }

        #endregion
    }
}
